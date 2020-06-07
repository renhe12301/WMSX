using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using ApplicationCore.Entities.TaskManager;
using ApplicationCore.Interfaces;
using ApplicationCore.Misc;
using Quartz;
using ApplicationCore.Specifications;
using System.Net.Http;
using ApplicationCore.Entities.FlowRecord;

namespace Web.Jobs
{
    [DisallowConcurrentExecution]
    public class SendWcsTaskJob : IJob
    {
        private readonly IAsyncRepository<InOutTask> _inOutTaskRepository;
        private readonly ILogRecordService _logRecordService;
        private readonly string WCS_TASK_RECEIVE_URL = "/fromWms/taskReceive";
        private static readonly HttpClient client = new HttpClient();

        public SendWcsTaskJob()
        {
            _inOutTaskRepository = EnginContext.Current.Resolve<IAsyncRepository<InOutTask>>();
            _logRecordService = EnginContext.Current.Resolve<ILogRecordService>();
        }

        public async Task Execute(IJobExecutionContext context)
        {
            try
            {
                InOutTaskSpecification inOutTaskSpecification = new InOutTaskSpecification(null, null, null, null,
                    null, null, null,
                    (int) TASK_READ.未读, null, null, null, null, null, null, null, null);
                List<InOutTask> inOutTasks = await this._inOutTaskRepository.ListAsync(inOutTaskSpecification);
                List<InOutTask> orderTasks = inOutTasks.OrderBy(t => t.CreateTime).ToList();
                foreach (var ot in orderTasks)
                {
                    string wcsUrl = ot.PhyWarehouse.Memo + WCS_TASK_RECEIVE_URL;
                    try
                    {
                        dynamic taskGroup = new ExpandoObject();
                        taskGroup.groupId = Guid.NewGuid().ToString();
                        taskGroup.msgTime = DateTime.Now.Ticks.ToString();
                        List<dynamic> tasks = new List<dynamic>();
                        dynamic task = new ExpandoObject();
                        task.taskId = ot.Id.ToString();
                        task.taskType = ot.Type == Convert.ToInt32(TASK_TYPE.物料入库) ||
                                        ot.Type == Convert.ToInt32(TASK_TYPE.空托盘入库)
                            ? 0
                            : 1;
                        task.district = ot.ReservoirArea.Id.ToString();
                        task.startNode = ot.SrcId.ToString();
                        task.endNode = ot.TargetId.ToString();
                        task.barCode = ot.TrayCode;
                        tasks.Add(task);
                        taskGroup.tasks = tasks;
                        string sendJsonObj = Newtonsoft.Json.JsonConvert.SerializeObject(taskGroup);
                        var response = await client.PostAsync(wcsUrl, new StringContent(sendJsonObj));
                        if (response.IsSuccessStatusCode)
                        {
                            var result = await response.Content.ReadAsStringAsync();
                            dynamic resultObj = Newtonsoft.Json.JsonConvert.DeserializeObject(result);
                            if (resultObj.returnStatus == 1)
                                throw new Exception(result);
                            ot.IsRead = Convert.ToInt32(TASK_READ.已读);
                            ot.Step = Convert.ToInt32(TASK_STEP.已接收);
                            await this._inOutTaskRepository.UpdateAsync(ot);
                        }
                        else
                        {
                            throw new Exception(await response.Content.ReadAsStringAsync());
                        }

                    }
                    catch (Exception ex)
                    {
                        await this._logRecordService.AddLog(new LogRecord
                        {
                            LogType = Convert.ToInt32(LOG_TYPE.异常日志),
                            LogDesc = wcsUrl + " [发送WCS任务Job]:" + ex.Message,
                            CreateTime = DateTime.Now
                        });
                    }
                }

            }
            catch (Exception ex)
            {

            }

        }
    }
}