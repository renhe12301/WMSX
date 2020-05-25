using System;
using System.Collections.Generic;
using Quartz;
using System.Linq;
using System.Threading.Tasks;
using ApplicationCore.Misc;
using ApplicationCore.Specifications;
using ApplicationCore.Entities.OrderManager;
using ApplicationCore.Entities.StockManager;
using ApplicationCore.Interfaces;
using ApplicationCore.Entities.TaskManager;
using System.Net;
using System.Text;
using System.IO;
using Newtonsoft.Json;
using System.Transactions;

/// <summary>
/// 遍历inoutTask,根据任务数据，遍历发送对应的数据给到速锐信息
/// http协议
/// </summary>
namespace Web.Jobs
{
    
    public class TaskClass
    {
        public string taskId { set; get; }//任务流水ID
        public int taskType { set; get; }//任务类型
        public string district { set; get; }//库区
        public string startNode { set; get; }//起始点
        public string endNode { set; get; }//放货点
        public string barCode { set; get; }//托盘码
    }

    public class SuRuiTaskClass
    {
        public string groupId { set; get; }//组号
        public long msgTime { set; get; }//下发时间
        public int priorityCode { set; get; }//优先级

        public new TaskClass task { set; get; }//任务参数



    }


    public class SuRuiTaskReturn
    {
        public string groupId { set; get; }//组号
        public long msgTime { set; get; }//下发时间
        public long returnStatus { set; get; }//请求结果
        public long returnInfo { set; get; }//结果描述

    }

    public class WCSTaskJob : IJob
    {

        private readonly IAsyncRepository<InOutTask> _inOutTaskRepository;
        private readonly IAsyncRepository<WarehouseTray> _warehouseTrayRepository;

        public WCSTaskJob(IAsyncRepository<InOutTask> inOutTaskRepository,
                       IAsyncRepository<WarehouseTray> warehouseTrayRepository
            )
        {

            this._inOutTaskRepository = inOutTaskRepository;
            this._warehouseTrayRepository = warehouseTrayRepository;

        }



        /// <summary>  
        /// DateTime时间格式转换为Unix时间戳格式  
        /// </summary>  
        /// <param name="time"> DateTime时间格式</param>  
        /// <returns>Unix时间戳格式</returns>  
        private static int ConvertDateTimeInt(System.DateTime time)
        {
            System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));
            return (int)(time - startTime).TotalSeconds;
        }


        /// <summary>
        /// Post数据接口
        /// </summary>
        /// <param name="postUrl">接口地址</param>
        /// <param name="paramData">提交json数据</param>
        /// <param name="dataEncode">编码方式(Encoding.UTF8)</param>
        /// <returns></returns>
        private static string PostWebRequest(string postUrl, string paramData, Encoding dataEncode)
        {
            string responseContent = string.Empty;
            try
            {
                byte[] byteArray = dataEncode.GetBytes(paramData); //转化
                HttpWebRequest webReq = (HttpWebRequest)WebRequest.Create(new Uri(postUrl));
                webReq.Method = "POST";
                //webReq.ContentType = "application/x-www-form-urlencoded"; 
                webReq.ContentType = "application/json";
                webReq.ContentLength = byteArray.Length;
                using (Stream reqStream = webReq.GetRequestStream())
                {
                    reqStream.Write(byteArray, 0, byteArray.Length);//写入参数
                    reqStream.Close();
                }
                using (HttpWebResponse response = (HttpWebResponse)webReq.GetResponse())
                {
                    //在这里对接收到的页面内容进行处理
                    using (StreamReader sr = new StreamReader(response.GetResponseStream(), Encoding.Default))
                    {
                        responseContent = sr.ReadToEnd().ToString();
                        sr.Close();
                        response.Close();
                        
                    }
                }

                


            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            return responseContent;
        }

        /// <summary>
        /// Post数据接口2
        /// </summary>
        /// <param name="content">文本</param>
        /// <param name="url">连接</param>
        /// <param name="contentType">文本传输格式</param>
        /// <returns></returns>
        public static string PostRequestTest(string content, string url, string contentType = "application/json")
        {
            try
            {
                var memStream = new MemoryStream();
                var cc = Encoding.UTF8.GetBytes(content);
                var request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "POST";
                request.ContentType = contentType;

                memStream.Write(cc, 0, cc.Length);

                request.ContentLength = memStream.Length;

                var requestStream = request.GetRequestStream();

                memStream.Position = 0;
                var tempBuffer = new byte[memStream.Length];
                memStream.Read(tempBuffer, 0, tempBuffer.Length);
                requestStream.Write(tempBuffer, 0, tempBuffer.Length);
                requestStream.Close();

                string responseTxt = string.Empty;

                WebResponse response = request.GetResponse();
                using (var reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8)) //Encoding.GetEncoding("gb2312")
                {
                    responseTxt = reader.ReadToEnd();
                }
                response.Close();

                return responseTxt;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task Execute(IJobExecutionContext context)
        {
            using (ModuleLock.GetAsyncLock().LockAsync())
            {
                try
                {

                    #region 参数定义
                    //定义需要更更改入库中状态的任务数据集合
                    List<WarehouseTray> updTrays = new List<WarehouseTray>();
                    //定义需要加入inoutTask中的数据集合
                    List<InOutTask> updTasks = new List<InOutTask>();
                    #endregion



                    //1.获得所有的inoutTask数据；
                    InOutTaskSpecification inOutTaskSpec = new InOutTaskSpecification(null,null,
                                                                                      new List<int>() {Convert.ToInt32(TASK_STATUS.待处理)},
                                                                                      new List<int>() {Convert.ToInt32(TASK_STEP.已接收)},
                                                                                      null,null,null,null,null,null,
                                                                                      null,null,null
                                                                                      );

                    List<InOutTask> inOutTasks = await this._inOutTaskRepository.ListAsync(inOutTaskSpec);



                    //出入库数量大于0
                    if (inOutTasks.Count > 0)
                    {




                        //按照时间进行排序
                        inOutTasks.OrderBy(t => t.CreateTime);

                        //2.循环生成任务
                        inOutTasks.ForEach(async (task) =>
                        {


                            //托盘增加更新
                            WarehouseTraySpecification warehouseTraySpec = new WarehouseTraySpecification(null, task.TrayCode,
                                                                                                          null, null, null, null,null,
                                                                                                          null, null, null, null,null);
                            //得到所有数据库中 托盘状态为入库申请的托盘信息
                            List<WarehouseTray> warehouseTrays = await this._warehouseTrayRepository.ListAsync(warehouseTraySpec);
                            //需要更改状态的托盘
                            WarehouseTray warehouseTray = warehouseTrays[0];
                            warehouseTray.TrayStep = Convert.ToInt32(TRAY_STEP.出库中未执行);                         
                            warehouseTray.Order = task.Order;
                            warehouseTray.OrderId = task.OrderId;
                            warehouseTray.Location.IsTask = Convert.ToInt32(LOCATION_TASK.有任务);
                            warehouseTray.Location.Status = Convert.ToInt32(LOCATION_STATUS.锁定);
                            updTrays.Add(warehouseTray);
                            
                            try
                            {
                                //2.0 获取当个任务数据 并组织成发送的数据
                                string param = string.Empty;        //参数

                                //SuRuiTaskClass suRuiTaskClass = new SuRuiTaskClass();

                                //suRuiTaskClass.groupId = Guid.NewGuid().ToString();//GroupID;
                                //suRuiTaskClass.msgTime = ConvertDateTimeInt(DateTime.Now);
                                //suRuiTaskClass.priorityCode = 1;
                                //suRuiTaskClass.task.taskId = task.Id.ToString();
                                //suRuiTaskClass.task.taskType = task.Type;
                                //suRuiTaskClass.task.district = task.ReservoirArea.AreaCode;
                                //suRuiTaskClass.task.startNode = task.SrcId;
                                //suRuiTaskClass.task.endNode = task.TargetId;
                                //suRuiTaskClass.task.barCode = task.TrayCode;

                                dynamic testClass = new System.Dynamic.ExpandoObject(); //动态类型字段 可读可写
                                testClass.groupId = Guid.NewGuid().ToString();//GroupID;
                                testClass.msgTime = ConvertDateTimeInt(DateTime.Now);
                                testClass.priorityCode = 1;
                                testClass.task.taskId = task.Id.ToString();
                                testClass.task.taskType = task.Type;
                                testClass.task.district = task.ReservoirArea.AreaCode;
                                testClass.task.startNode = task.SrcId;
                                testClass.task.endNode = task.TargetId;
                                testClass.task.barCode = task.TrayCode;




                                // param = JsonConvert.SerializeObject(suRuiTaskClass);
                                param = JsonConvert.SerializeObject(testClass);


                                //2.1.Http发送任务

                                string returnString = PostWebRequest("http://{ip}:{port}/fromWms/taskReceive", param, Encoding.UTF8);



                                //2.2.接受反馈确定下发成功后更改数据库状态

                                //这个需要引入Newtonsoft.Json这个DLL并using
                                //传入我们的实体类还有需要解析的JSON字符串这样就OK了。然后就可以通过实体类使用数据了。
                                dynamic suRuiTaskReturn = JsonConvert.DeserializeObject<dynamic>(returnString);
                                //SuRuiTaskReturn suRuiTaskReturn = JsonConvert.DeserializeObject<SuRuiTaskReturn>(returnString);
                                if (suRuiTaskReturn.groupId == testClass.groupId && suRuiTaskReturn.returnStatus == 0)
                                {
                                    task.Step = Convert.ToInt32(TASK_STEP.已接收);
                                    task.Status = Convert.ToInt32(TASK_STATUS.执行中);
                                    updTasks.Add(task);

                                    //反序列化 如果 groupid == groupid,retunstatus == 0 修改inouttask状态和warehouse状态，
                                    using (var scope = new TransactionScope(TransactionScopeOption.RequiresNew))
                                    {
                                        try
                                        {
                                           
                                            this._warehouseTrayRepository.Update(updTrays);
                                            this._inOutTaskRepository.Update(updTasks);
                                           
                                        }
                                        catch (Exception ex)
                                        {

                                            throw ex;
                                        }



                                    }

                                    //add数据清除
                                    try
                                    {
                                        updTrays.Clear();
                                        updTasks.Clear();

                                    }
                                    catch (Exception ex)
                                    {
                                        throw new Exception(ex.Message);
                                    }


                                }
                                else
                                {
                                    //反之，记录错误原因。
                                    if (suRuiTaskReturn.returnStatus != 0)
                                    {
                                        throw new Exception(string.Format("反馈结果出错，出错原因：{0}。", suRuiTaskReturn.returnInfo));
                                    }
                                    else
                                    {
                                        throw new Exception("反馈组号不同，请检查网络设置。");
                                    }
                                }

                              



                            }
                            catch (Exception)
                            {

                                throw;
                            }


                        });



                    }











                }
                catch (Exception ex)
                {
                    // todo 添加异常日志记录
                }
            }
        }


    }
}
