using ApplicationCore.Entities.BasicInformation;
using ApplicationCore.Interfaces;
using ApplicationCore.Specifications;
using Quartz;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Security.Cryptography;//引用Md5转换功能
using System.Text;
using System.Threading.Tasks;

namespace Web.Jobs
{
    /// <summary>
    /// 主数据同步定时任务
    /// </summary>
    public class BasicDataSyncJob:IJob
    {
        //关于主数据函数

        private readonly IAsyncRepository<Employee> _employeeyRepository;//人员信息
        private readonly IAsyncRepository<Organization> _organizationRepository;//部门信息
        private readonly IAsyncRepository<OU> _ouRepository;//业务实体信息
        private readonly IAsyncRepository<Warehouse> _warehouseRepository;//库存组织信息
        private readonly IAsyncRepository<MaterialDic> _materialDicRepository;//物料
        private readonly IAsyncRepository<ReservoirArea> _reservoirAreaRepository;//子库存信息
        private readonly IAsyncRepository<Supplier> _supplierRepository;//供应商地点信息
        private readonly IAsyncRepository<SupplierSite> _supplierSiteRepository;//供应商头信息

        private readonly IAsyncRepository<EBSProject> _EBSProjectRepository;//项目信息
        private readonly IAsyncRepository<EBSTask> _EBSTaskRepository;//任务信息

        public BasicDataSyncJob(IAsyncRepository<Employee> employeeyRepository, IAsyncRepository<Organization> organizationRepository,
                               IAsyncRepository<OU> ouRepository,  IAsyncRepository<Warehouse> warehouseRepository,
                               IAsyncRepository<MaterialDic> materialDicRepository, IAsyncRepository<ReservoirArea> reservoirAreaRepository,
                               IAsyncRepository<Supplier> supplierRepository, IAsyncRepository<SupplierSite> supplierSiteRepository,
                               IAsyncRepository<EBSProject> EBSProjectRepository, IAsyncRepository<EBSTask> EBSTaskRepository) 
        {
            this._employeeyRepository = employeeyRepository;//人员
            this._organizationRepository = organizationRepository;//部门
            this._ouRepository = ouRepository;//业务
            this._warehouseRepository = warehouseRepository;//库组织
            this._materialDicRepository = materialDicRepository;//物料
            this._reservoirAreaRepository = reservoirAreaRepository;//子库存
            this._supplierRepository = supplierRepository;//供应商地址
            this._supplierSiteRepository = supplierSiteRepository;//供应商头

            this._EBSProjectRepository = EBSProjectRepository;//项目
            this._EBSTaskRepository = EBSTaskRepository;//任务
        }

        public async Task Execute(IJobExecutionContext context)
        {
            try
            {
                /*
                 * 1.假设调用ebs接口 list<emplyees> serEmployees = service.call('20200218','20200219')，serEmployees为返回参数
                 * 
                 * 
                 * 
                */

                //[1].项目信息同步
                EBSProjectNew = DateTime.Now;
                EBSProjectSyn();

                //[2].任务信息同步
                EBSTaskSyn();

                //[3].业务实体信息同步
                OUSyn();

                //[4].部门信息同步
                OrganizationSyn();

                //[5].人员信息同步
                EmployeeSyn();

                //[6].库存组织信息同步
                WarehouseSyn();

                //[7].子库存信息同步
                ReservoirAreaSyn();

                //[8].物料信息同步
                MaterialDicSyn();

                //[9].供应商头信息同步
                SupplierSyn();

                //[10].供应商地址信息同步
                SupplierSiteSyn();

            }
            catch (Exception ex)
            {
                   
            }
        }
        
        /// <summary>
        /// md5字符串转换
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public string GetMD5WithString(String input)
        {
            MD5 md5Hash = MD5.Create();
            // 将输入字符串转换为字节数组并计算哈希数据  
            byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));
            // 创建一个 Stringbuilder 来收集字节并创建字符串  
            StringBuilder str = new StringBuilder();
            // 循环遍历哈希数据的每一个字节并格式化为十六进制字符串  
            for (int i = 0; i < data.Length; i++)
            {
                str.Append(data[i].ToString("x2"));//加密结果"x2"结果为32位,"x3"结果为48位,"x4"结果为64位
            }
            // 返回十六进制字符串  
            return str.ToString();
        }

        /// <summary>
        /// 项目信息同步
        /// </summary>

        DateTime EBSProjectOld;
        DateTime EBSProjectNew;
        private async void EBSProjectSyn()
        {
            //1.判断是否需要更手动更新
            bool isEBSProjectSyn = false;

            //2.读取数据库，EBSProject 进行判断是否需要，手动更新


            //3.是否间隔5分钟
            TimeSpan ts1 = new TimeSpan(EBSProjectOld.Ticks); 
            TimeSpan ts2 = new TimeSpan(EBSProjectNew.Ticks); 
            TimeSpan ts = ts1.Subtract(ts2).Duration(); 
                
            if (isEBSProjectSyn == true|| ts.Minutes == 5)
            {
                #region 项目信息同步
                List<EBSProject> serEBSProject = new List<EBSProject> //假设主数据返回
                {
                    new EBSProject
                    {
                       ProjectCode="121",ProjectName="费用化项目",ProjectFullName="费用化项目",OUId=1
                    },new EBSProject
                    {
                        ProjectCode="121",ProjectName="李家峡发电分公司",ProjectFullName="费用化项目",OUId=1
                    }
                };

                EBSProjectIdSetSpecification EBSProjectIdSetSpec = new EBSProjectIdSetSpecification(serEBSProject.ConvertAll(e => e.Id));//本地查询
                List<EBSProject> localEBSProjects = await this._EBSProjectRepository.ListAsync(EBSProjectIdSetSpec);
                List<EBSProject> upEBSProject = new List<EBSProject>();//更新部门对象集合
                List<EBSProject> addEBSProject = new List<EBSProject>();//添加部门对象集合
                serEBSProject.ForEach(sm =>//循环对象集合--》操作返回数据集合，对比本地数据对象，是否更新
                {
                    EBSProject localEBSProject = localEBSProjects.Find(e => e.Id == sm.Id);//根据返回id-->本地对象
                    if (localEBSProject != null)//本地存在--》更新
                    {
                        var sebsMd5 = sm.MD5String();
                        var swmsMd5 = localEBSProject.MD5String();//本地MD5拼接
                        if (sebsMd5.Equals(swmsMd5))//是否有差别
                        {
                            //无差别 不操作
                        }
                        else
                        {
                            //dynamic dyp = new System.Dynamic.ExpandoObject();
                            //foreach (var L in OrganizationAnnotation)
                            //{
                            //    //此处不能写死****
                            //    localOrganization.OrgName = sm.OrgName;
                            //    localOrganization.OrgCode = sm.OrgCode;
                            //    localOrganization.OUId = sm.OUId;
                            //}

                            upEBSProject.Add(localEBSProject);
                        }

                    }
                    else //不存在--》添加 add
                    {
                        EBSProject addEBSProjectU = new EBSProject();
                        //不能写死  *****
                        //foreach (var L in OrganizationAnnotation)
                        //{
                        //    //此处不能写死****
                        //    addOrganizationU.OrgName = sm.OrgName;
                        //    addOrganizationU.OrgCode = sm.OrgCode;
                        //    addOrganizationU.OUId = sm.OUId;
                        //}
                        addEBSProject.Add(addEBSProjectU);
                    }
                });
                await this._EBSProjectRepository.UpdateAsync(upEBSProject);//一起更新
                await this._EBSProjectRepository.AddAsync(addEBSProject);//一起添加

                #endregion

                EBSProjectOld  = DateTime.Now;

                //4.回复数据库初始变量开关  isEBSProjectSyn，
            }
        }

        /// <summary>
        /// 任务信息同步
        /// </summary>
        /// 

        DateTime EBSTaskOld;
        DateTime EBSTaskNew;
        private async void EBSTaskSyn()
        {
            bool isEBSTaskSyn = false;

            TimeSpan ts1 = new TimeSpan(EBSTaskOld.Ticks);
            TimeSpan ts2 = new TimeSpan(EBSTaskNew.Ticks);
            TimeSpan ts = ts1.Subtract(ts2).Duration();

            if (isEBSTaskSyn == true || ts.Minutes == 5)
            {
                #region 任务信息同步
                List<EBSTask> serEBSTask = new List<EBSTask> //假设主数据返回
                {
                    new EBSTask
                    {
                       TaskCode="123"
                    },new EBSTask
                    {
                       TaskCode="123"
                    }
                };

                //List<string> EBSTaskAnnotation = new List<string>();
                //本地实体类注解得属性
                //PropertyInfo[] propertiesEBSTask = typeof(Organization).GetProperties(BindingFlags.Public | BindingFlags.Instance);
                //foreach (PropertyInfo property in propertiesEBSTask)
                //{
                //    object[] proDescrition = property.GetCustomAttributes(typeof(DescriptionAttribute), true);
                //    if (proDescrition.Length > 0)
                //    {
                //        EBSTaskAnnotation.Add(property.Name.ToString());
                //    }
                //}

                EBSTaskIdSetSpecification EBSTaskIdSetSpec = new EBSTaskIdSetSpecification(serEBSTask.ConvertAll(e => e.Id));//本地查询
                List<EBSTask> localEBSTasks = await this._EBSTaskRepository.ListAsync(EBSTaskIdSetSpec);//原函数代码好像返回值不对
                List<EBSTask> upEBSTask = new List<EBSTask>();//更新部门对象集合
                List<EBSTask> addEBSTask = new List<EBSTask>();//添加部门对象集合
                serEBSTask.ForEach(sm =>//循环对象集合--》操作返回数据集合，对比本地数据对象，是否更新
                {
                    EBSTask localEBSTask = localEBSTasks.Find(e => e.Id == sm.Id);//根据返回id-->本地对象
                    if (localEBSTask != null)//本地存在--》更新
                    {
                        //本地数据拼接属性
                        //PropertyInfo[] p = localEBSTask.GetType().GetProperties();
                        //string S_WMS = "";//拼接
                        //foreach (PropertyInfo pinfo in p)
                        //{
                        //    var isContain = EBSTaskAnnotation.Contains(pinfo.Name);
                        //    if (isContain == true) { S_WMS = S_WMS + pinfo.GetValue(localEBSTask, null); }
                        //}
                        //var swmsMd5 = GetMD5WithString(S_WMS);
                        var swmsMd5 = localEBSTask.MD5String();
                        //反馈数据拼接属性
                        //PropertyInfo[] pe = sm.GetType().GetProperties();
                        //string S_EBS = "";//拼接
                        //foreach (PropertyInfo pinfo in pe)
                        //{
                        //    var isContainPe = EBSTaskAnnotation.Contains(pinfo.Name);
                        //    if (isContainPe == true) { S_EBS = S_EBS + pinfo.GetValue(sm, null); }
                        //}
                        //var sebsMd5 = GetMD5WithString(S_EBS);

                        var sebsMd5 = sm.MD5String();
                        if (sebsMd5.Equals(swmsMd5))//是否有差别
                        {
                            //无差别 不操作
                        }
                        else
                        {
                            //dynamic dyp = new System.Dynamic.ExpandoObject();
                            //foreach (var L in OrganizationAnnotation)
                            //{
                            //    //此处不能写死****
                            //    localOrganization.OrgName = sm.OrgName;
                            //    localOrganization.OrgCode = sm.OrgCode;
                            //    localOrganization.OUId = sm.OUId;
                            //}

                            upEBSTask.Add(localEBSTask);
                        }
                    }
                    else //不存在--》添加 add
                    {
                        EBSTask addEBSTaskU = new EBSTask();
                        //不能写死  *****
                        //foreach (var L in OrganizationAnnotation)
                        //{
                        //    //此处不能写死****
                        //    addOrganizationU.OrgName = sm.OrgName;
                        //    addOrganizationU.OrgCode = sm.OrgCode;
                        //    addOrganizationU.OUId = sm.OUId;
                        //}
                        addEBSTask.Add(addEBSTaskU);
                    }
                });
                await this._EBSTaskRepository.UpdateAsync(upEBSTask);//一起更新
                await this._EBSTaskRepository.AddAsync(addEBSTask);//一起添加

                #endregion
            }

            EBSTaskOld = DateTime.Now;
        }

        /// <summary>
        /// 业务实体信息同步
        /// </summary>
        ///  
        DateTime OUOld;
        DateTime OUNew;
        private async void OUSyn()
        {
            bool isOUSyn = false;

            TimeSpan ts1 = new TimeSpan(OUOld.Ticks);
            TimeSpan ts2 = new TimeSpan(OUNew.Ticks);
            TimeSpan ts = ts1.Subtract(ts2).Duration();

            if (isOUSyn == true || ts.Minutes == 5)
            {

                #region 业务实体信息同步
                List<OU> serOU = new List<OU> //假设主数据返回
                {
                    new OU
                    {
                       OUName="李家峡生产部",OUCode="853",CompanyName="ww",PlateCode="w",PlateName="s"
                    },new OU
                    {
                       OUName="李家峡技术部",OUCode="856",CompanyName="xx",PlateCode="w",PlateName="s"
                    }
                };

                //List<string> OUAnnotation = new List<string>();
                ////本地实体类注解得属性
                //PropertyInfo[] propertiesOU = typeof(OU).GetProperties(BindingFlags.Public | BindingFlags.Instance);
                //foreach (PropertyInfo property in propertiesOU)
                //{
                //    object[] proDescrition = property.GetCustomAttributes(typeof(DescriptionAttribute), true);
                //    if (proDescrition.Length > 0)
                //    {
                //        OUAnnotation.Add(property.Name.ToString());
                //    }
                //}

                OUIdSetSpecification OUIdSetSpec = new OUIdSetSpecification(serOU.ConvertAll(e => e.Id));//本地查询
                List<OU> localOUs = await this._ouRepository.ListAsync(OUIdSetSpec);
                List<OU> updOU = new List<OU>();//更新部门对象集合
                List<OU> addOU = new List<OU>();//添加部门对象集合
                serOU.ForEach(sm =>//循环对象集合--》操作返回数据集合，对比本地数据对象，是否更新
                {
                    OU localOU = localOUs.Find(e => e.Id == sm.Id);//根据返回id-->本地对象
                    if (localOU != null)//本地存在--》更新
                    {
                        //本地数据拼接属性
                        //PropertyInfo[] p = localOU.GetType().GetProperties();
                        //string S_WMS = "";//拼接
                        //foreach (PropertyInfo pinfo in p)
                        //{
                        //    var isContain = OUAnnotation.Contains(pinfo.Name);
                        //    if (isContain == true) { S_WMS = S_WMS + pinfo.GetValue(localOU, null); }
                        //}
                        //var swmsMd5 = GetMD5WithString(S_WMS);
                        var swmsMd5 = localOU.MD5String();
                        //反馈数据拼接属性
                        //PropertyInfo[] pe = sm.GetType().GetProperties();
                        //string S_EBS = "";//拼接
                        //foreach (PropertyInfo pinfo in pe)
                        //{
                        //    var isContainPe = OUAnnotation.Contains(pinfo.Name);
                        //    if (isContainPe == true) { S_EBS = S_EBS + pinfo.GetValue(sm, null); }
                        //}
                        //var sebsMd5 = GetMD5WithString(S_EBS);
                        var sebsMd5 = sm.MD5String();
                        if (sebsMd5.Equals(swmsMd5))//是否有差别
                        {
                            //无差别 不操作
                        }
                        else
                        {
                            //dynamic dyp = new System.Dynamic.ExpandoObject();
                            //foreach (var L in OrganizationAnnotation)
                            //{
                            //    //此处不能写死****
                            //    //localOrganization.OrgName = sm.OrgName;
                            //    //localOrganization.OrgCode = sm.OrgCode;
                            //    //localOrganization.OUId = sm.OUId;
                            //}

                            updOU.Add(localOU);
                        }

                    }
                    else //不存在--》添加 add
                    {
                        OU addOUU = new OU();
                        //不能写死  *****
                        //foreach (var L in OUAnnotation)
                        //{
                        //    //此处不能写死****
                        //    //addOrganizationU.OrgName = sm.OrgName;
                        //    //addOrganizationU.OrgCode = sm.OrgCode;
                        //    //addOrganizationU.OUId = sm.OUId;
                        //}
                        addOU.Add(addOUU);
                    }
                });
                await this._ouRepository.UpdateAsync(updOU);//一起更新
                await this._ouRepository.AddAsync(addOU);//一起添加
                #endregion

            }

        }

        /// <summary>
        /// 部门信息同步
        /// </summary>
        /// 


        private async void OrganizationSyn()
        {
            #region 部门信息同步
            List<Organization> serOrganization = new List<Organization> //假设主数据返回
                {
                    new Organization
                    {
                        Id =1,OrgName="李家峡生产部",OrgCode="853",OUId=1
                    },new Organization
                    {
                       Id =1,OrgName="李家峡技术部",OrgCode="856",OUId=1
                    }
                };

            List<string> OrganizationAnnotation = new List<string>();
            //本地实体类注解得属性
            PropertyInfo[] propertiesOrganization = typeof(Organization).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo property in propertiesOrganization)
            {
                object[] proDescrition = property.GetCustomAttributes(typeof(DescriptionAttribute), true);
                if (proDescrition.Length > 0)
                {
                    OrganizationAnnotation.Add(property.Name.ToString());
                }
            }

            OrganizationIdSetSpecification organizationIdSetSpec = new OrganizationIdSetSpecification(serOrganization.ConvertAll(e => e.Id));//本地查询
            List<Organization> localOrganizations = await this._organizationRepository.ListAsync(organizationIdSetSpec);
            List<Organization> updOrganization = new List<Organization>();//更新部门对象集合
            List<Organization> addOrganization = new List<Organization>();//添加部门对象集合
            serOrganization.ForEach(sm =>//循环对象集合--》操作返回数据集合，对比本地数据对象，是否更新
            {
                Organization localOrganization = localOrganizations.Find(e => e.Id == sm.Id);//根据返回id-->本地对象
                if (localOrganization != null)//本地存在--》更新
                {
                    //本地数据拼接属性
                    PropertyInfo[] p = localOrganization.GetType().GetProperties();
                    string S_WMS = "";//拼接
                    foreach (PropertyInfo pinfo in p)
                    {
                        var isContain = OrganizationAnnotation.Contains(pinfo.Name);
                        if (isContain == true) { S_WMS = S_WMS + pinfo.GetValue(localOrganization, null); }
                    }
                    var swmsMd5 = GetMD5WithString(S_WMS);

                    //反馈数据拼接属性
                    PropertyInfo[] pe = sm.GetType().GetProperties();
                    string S_EBS = "";//拼接
                    foreach (PropertyInfo pinfo in pe)
                    {
                        var isContainPe = OrganizationAnnotation.Contains(pinfo.Name);
                        if (isContainPe == true) { S_EBS = S_EBS + pinfo.GetValue(sm, null); }
                    }
                    var sebsMd5 = GetMD5WithString(S_EBS);

                    if (sebsMd5.Equals(swmsMd5))//是否有差别
                    {
                        //无差别 不操作
                    }
                    else
                    {
                        //dynamic dyp = new System.Dynamic.ExpandoObject();
                        foreach (var L in OrganizationAnnotation)
                        {
                            //此处不能写死****
                            localOrganization.OrgName = sm.OrgName;
                            localOrganization.OrgCode = sm.OrgCode;
                            localOrganization.OUId = sm.OUId;
                        }

                        updOrganization.Add(localOrganization);
                    }

                }
                else //不存在--》添加 add
                {
                    Organization addOrganizationU = new Organization();
                    //不能写死  *****
                    foreach (var L in OrganizationAnnotation)
                    {
                        //此处不能写死****
                        addOrganizationU.OrgName = sm.OrgName;
                        addOrganizationU.OrgCode = sm.OrgCode;
                        addOrganizationU.OUId = sm.OUId;
                    }
                    addOrganization.Add(addOrganizationU);
                }
            });
            await this._organizationRepository.UpdateAsync(updOrganization);//一起更新
            await this._organizationRepository.AddAsync(addOrganization);//一起添加

            #endregion
        }

        /// <summary>
        /// 人员信息同步
        /// </summary>
        private async void EmployeeSyn()
        {
            #region 人员信息同步
            List<Employee> serEmployees = new List<Employee> //假设主数据返回
                {
                    new Employee
                    {
                        Id =1,UserName="10001",UserCode="123",Sex="a",Telephone="138654589456",Email="138656@qq.com",CreateTime=DateTime.ParseExact("2019/01/01" + " " + "11:11", "yyyy/MM/dd HH:mm", null),
                        EndTime=DateTime.ParseExact("2021/01/01" + " " + "11:11", "yyyy/MM/dd HH:mm", null),OrganizationId=1
                    },new Employee
                    {
                        Id =2,UserName="10002",UserCode="123",Sex="a",Telephone="138654589456",Email="138656@qq.com",CreateTime=DateTime.ParseExact("2019/01/01" + " " + "11:11", "yyyy/MM/dd HH:mm", null),
                        EndTime=DateTime.ParseExact("2021/01/01" + " " + "11:11", "yyyy/MM/dd HH:mm", null),OrganizationId=1
                    },new Employee
                    {
                        Id =3,UserName="10003",UserCode="123",Sex="a",Telephone="138654589456",Email="138656@qq.com",CreateTime=DateTime.ParseExact("2019/01/01" + " " + "11:11", "yyyy/MM/dd HH:mm", null),
                        EndTime=DateTime.ParseExact("2021/01/01" + " " + "11:11", "yyyy/MM/dd HH:mm", null),OrganizationId=1
                    },new Employee
                    {
                        Id =4,UserName="10004",UserCode="123",Sex="a",Telephone="138654589456",Email="138656@qq.com",CreateTime=DateTime.ParseExact("2019/01/01" + " " + "11:11", "yyyy/MM/dd HH:mm", null),
                        EndTime=DateTime.ParseExact("2021/01/01" + " " + "11:11", "yyyy/MM/dd HH:mm", null),OrganizationId=1
                    }
                };

            List<string> EmployeeAnnotation = new List<string>();
            //本地实体类注解得属性
            PropertyInfo[] propertiesEmployee = typeof(Employee).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo property in propertiesEmployee)
            {
                object[] proDescrition = property.GetCustomAttributes(typeof(DescriptionAttribute), true);
                if (proDescrition.Length > 0)
                {
                    EmployeeAnnotation.Add(property.Name.ToString());
                }
            }

            EmployeeIdSetSpecification employeeIdSetSpec = new EmployeeIdSetSpecification(serEmployees.ConvertAll(e => e.Id));//本地查询
            List<Employee> localEmployees = await this._employeeyRepository.ListAsync(employeeIdSetSpec);
            List<Employee> updEmployee = new List<Employee>();//更新部门对象集合
            List<Employee> addEmployee = new List<Employee>();//添加部门对象集合
            serEmployees.ForEach(sm =>//循环对象集合--》操作返回数据集合，对比本地数据对象，是否更新
            {
                Employee localEmployee = localEmployees.Find(e => e.Id == sm.Id);//根据返回id-->本地对象
                if (localEmployee != null)//本地存在--》更新
                {
                    //本地数据拼接属性
                    PropertyInfo[] p = localEmployee.GetType().GetProperties();
                    string S_WMS = "";//拼接
                    foreach (PropertyInfo pinfo in p)
                    {
                        var isContain = EmployeeAnnotation.Contains(pinfo.Name);
                        if (isContain == true) { S_WMS = S_WMS + pinfo.GetValue(localEmployee, null); }
                    }
                    var swmsMd5 = GetMD5WithString(S_WMS);

                    //反馈数据拼接属性
                    PropertyInfo[] pe = sm.GetType().GetProperties();
                    string S_EBS = "";//拼接
                    foreach (PropertyInfo pinfo in pe)
                    {
                        var isContainPe = EmployeeAnnotation.Contains(pinfo.Name);
                        if (isContainPe == true) { S_EBS = S_EBS + pinfo.GetValue(sm, null); }
                    }
                    var sebsMd5 = GetMD5WithString(S_EBS);

                    if (sebsMd5.Equals(swmsMd5))//是否有差别
                    {
                        //无差别 不操作
                    }
                    else
                    {
                        //dynamic dyp = new System.Dynamic.ExpandoObject();
                        //foreach (var L in OrganizationAnnotation)
                        //{
                        //    //此处不能写死****
                        //    localOrganization.OrgName = sm.OrgName;
                        //    localOrganization.OrgCode = sm.OrgCode;
                        //    localOrganization.OUId = sm.OUId;
                        //}

                        updEmployee.Add(localEmployee);
                    }

                }
                else //不存在--》添加 add
                {
                    Employee addEmployeeU = new Employee();
                    //不能写死  *****
                    //foreach (var L in OrganizationAnnotation)
                    //{
                    //    //此处不能写死****
                    //    addOrganizationU.OrgName = sm.OrgName;
                    //    addOrganizationU.OrgCode = sm.OrgCode;
                    //    addOrganizationU.OUId = sm.OUId;
                    //}
                    addEmployee.Add(addEmployeeU);
                }
            });
            await this._employeeyRepository.UpdateAsync(updEmployee);//一起更新
            await this._employeeyRepository.AddAsync(addEmployee);//一起添加

            #endregion
        }

        /// <summary>
        /// 库存组织信息同步
        /// </summary>
        private async void WarehouseSyn()
        {
            #region 库存组织信息同步
            List<Warehouse> serWarehouse = new List<Warehouse> //假设主数据返回
                {
                    new Warehouse
                    {
                        Id =1,WhName="李家峡生产部",WhCode="853",OUId=1
                    },new Warehouse
                    {
                       Id =1,WhName="李家峡技术部",WhCode="856",OUId=1
                    }
                };

            List<string> WarehouseAnnotation = new List<string>();
            //本地实体类注解得属性
            PropertyInfo[] propertiesWarehouse = typeof(Warehouse).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo property in propertiesWarehouse)
            {
                object[] proDescrition = property.GetCustomAttributes(typeof(DescriptionAttribute), true);
                if (proDescrition.Length > 0)
                {
                    WarehouseAnnotation.Add(property.Name.ToString());
                }
            }

            WarehouseIdSpecification warehouseIdSetSpec = new WarehouseIdSpecification(serWarehouse.ConvertAll(e => e.Id));//本地查询
            List<Warehouse> localWarehouses = await this._warehouseRepository.ListAsync(warehouseIdSetSpec);
            List<Warehouse> updWarehouse = new List<Warehouse>();//更新部门对象集合
            List<Warehouse> addWarehouse = new List<Warehouse>();//添加部门对象集合
            serWarehouse.ForEach(sm =>//循环对象集合--》操作返回数据集合，对比本地数据对象，是否更新
            {
                Warehouse localWarehouse = localWarehouses.Find(e => e.Id == sm.Id);//根据返回id-->本地对象
                if (localWarehouse != null)//本地存在--》更新
                {
                    //本地数据拼接属性
                    PropertyInfo[] p = localWarehouse.GetType().GetProperties();
                    string S_WMS = "";//拼接
                    foreach (PropertyInfo pinfo in p)
                    {
                        var isContain = WarehouseAnnotation.Contains(pinfo.Name);
                        if (isContain == true) { S_WMS = S_WMS + pinfo.GetValue(localWarehouse, null); }
                    }
                    var swmsMd5 = GetMD5WithString(S_WMS);

                    //反馈数据拼接属性
                    PropertyInfo[] pe = sm.GetType().GetProperties();
                    string S_EBS = "";//拼接
                    foreach (PropertyInfo pinfo in pe)
                    {
                        var isContainPe = WarehouseAnnotation.Contains(pinfo.Name);
                        if (isContainPe == true) { S_EBS = S_EBS + pinfo.GetValue(sm, null); }
                    }
                    var sebsMd5 = GetMD5WithString(S_EBS);

                    if (sebsMd5.Equals(swmsMd5))//是否有差别
                    {
                        //无差别 不操作
                    }
                    else
                    {
                        //dynamic dyp = new System.Dynamic.ExpandoObject();
                        //foreach (var L in WarehouseAnnotation)
                        //{
                        //    //此处不能写死****
                        //    localOrganization.OrgName = sm.OrgName;
                        //    localOrganization.OrgCode = sm.OrgCode;
                        //    localOrganization.OUId = sm.OUId;
                        //}

                        updWarehouse.Add(localWarehouse);
                    }

                }
                else //不存在--》添加 add
                {
                    Warehouse addWarehouseU = new Warehouse();
                    //不能写死  *****
                    //foreach (var L in OrganizationAnnotation)
                    //{
                    //    //此处不能写死****
                    //    addOrganizationU.OrgName = sm.OrgName;
                    //    addOrganizationU.OrgCode = sm.OrgCode;
                    //    addOrganizationU.OUId = sm.OUId;
                    //}
                    addWarehouse.Add(addWarehouseU);
                }
            });
            await this._warehouseRepository.UpdateAsync(updWarehouse);//一起更新
            await this._warehouseRepository.AddAsync(addWarehouse);//一起添加

            #endregion
        }

        /// <summary>
        /// 子库存信息同步
        /// </summary>
        private async void ReservoirAreaSyn()
        {
            #region 子库存信息同步
            List<ReservoirArea> serReservoirArea = new List<ReservoirArea> //假设主数据返回
                {
                    new ReservoirArea
                    {
                        Id =1
                    },new ReservoirArea
                    {
                       Id =1
                    }
                };

            List<string> ReservoirAreaAnnotation = new List<string>();
            //本地实体类注解得属性
            PropertyInfo[] propertiesReservoirArea = typeof(ReservoirArea).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo property in propertiesReservoirArea)
            {
                object[] proDescrition = property.GetCustomAttributes(typeof(DescriptionAttribute), true);
                if (proDescrition.Length > 0)
                {
                    ReservoirAreaAnnotation.Add(property.Name.ToString());
                }
            }

            ReservoirAreaIdSetSpecification reservoirAreaIdSetSpec = new ReservoirAreaIdSetSpecification(serReservoirArea.ConvertAll(e => e.Id));//本地查询
            List<ReservoirArea> localReservoirAreas = await this._reservoirAreaRepository.ListAsync(reservoirAreaIdSetSpec);
            List<ReservoirArea> updReservoirArea = new List<ReservoirArea>();//更新部门对象集合
            List<ReservoirArea> addReservoirArea = new List<ReservoirArea>();//添加部门对象集合
            serReservoirArea.ForEach(sm =>//循环对象集合--》操作返回数据集合，对比本地数据对象，是否更新
            {
                ReservoirArea localReservoirArea = localReservoirAreas.Find(e => e.Id == sm.Id);//根据返回id-->本地对象
                if (localReservoirArea != null)//本地存在--》更新
                {
                    //本地数据拼接属性
                    PropertyInfo[] p = localReservoirArea.GetType().GetProperties();
                    string S_WMS = "";//拼接
                    foreach (PropertyInfo pinfo in p)
                    {
                        var isContain = ReservoirAreaAnnotation.Contains(pinfo.Name);
                        if (isContain == true) { S_WMS = S_WMS + pinfo.GetValue(localReservoirArea, null); }
                    }
                    var swmsMd5 = GetMD5WithString(S_WMS);

                    //反馈数据拼接属性
                    PropertyInfo[] pe = sm.GetType().GetProperties();
                    string S_EBS = "";//拼接
                    foreach (PropertyInfo pinfo in pe)
                    {
                        var isContainPe = ReservoirAreaAnnotation.Contains(pinfo.Name);
                        if (isContainPe == true) { S_EBS = S_EBS + pinfo.GetValue(sm, null); }
                    }
                    var sebsMd5 = GetMD5WithString(S_EBS);

                    if (sebsMd5.Equals(swmsMd5))//是否有差别
                    {
                        //无差别 不操作
                    }
                    else
                    {
                        //dynamic dyp = new System.Dynamic.ExpandoObject();
                        //foreach (var L in ReservoirAreaAnnotation)
                        //{
                        //    //此处不能写死****
                        //    localOrganization.OrgName = sm.OrgName;
                        //    localOrganization.OrgCode = sm.OrgCode;
                        //    localOrganization.OUId = sm.OUId;
                        //}

                        updReservoirArea.Add(localReservoirArea);
                    }

                }
                else //不存在--》添加 add
                {
                    ReservoirArea addReservoirAreaU = new ReservoirArea();
                    //不能写死  *****
                    //foreach (var L in OrganizationAnnotation)
                    //{
                    //    //此处不能写死****
                    //    addOrganizationU.OrgName = sm.OrgName;
                    //    addOrganizationU.OrgCode = sm.OrgCode;
                    //    addOrganizationU.OUId = sm.OUId;
                    //}
                    addReservoirArea.Add(addReservoirAreaU);
                }
            });
            await this._reservoirAreaRepository.UpdateAsync(updReservoirArea);//一起更新
            await this._reservoirAreaRepository.AddAsync(addReservoirArea);//一起添加

            #endregion
        }

        /// <summary>
        /// 物料信息同步
        /// </summary>
        private async void MaterialDicSyn()
        {
            #region 物料信息同步
            List<MaterialDic> serMaterialDic = new List<MaterialDic> //假设主数据返回
                {
                    new MaterialDic
                    {
                        Id =1
                    },new MaterialDic
                    {
                       Id =1
                    }
                };

            List<string> MaterialDicAnnotation = new List<string>();
            //本地实体类注解得属性
            PropertyInfo[] propertiesMaterialDic = typeof(MaterialDic).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo property in propertiesMaterialDic)
            {
                object[] proDescrition = property.GetCustomAttributes(typeof(DescriptionAttribute), true);
                if (proDescrition.Length > 0)
                {
                    MaterialDicAnnotation.Add(property.Name.ToString());
                }
            }

            MaterialDicIdSetSpecification materialDicIdSetSpec = new MaterialDicIdSetSpecification(serMaterialDic.ConvertAll(e => e.Id));//本地查询
            List<MaterialDic> localMaterialDics = await this._materialDicRepository.ListAsync(materialDicIdSetSpec);
            List<MaterialDic> upMaterialDic = new List<MaterialDic>();//更新部门对象集合
            List<MaterialDic> addMaterialDic = new List<MaterialDic>();//添加部门对象集合
            serMaterialDic.ForEach(sm =>//循环对象集合--》操作返回数据集合，对比本地数据对象，是否更新
            {
                MaterialDic localMaterialDic = localMaterialDics.Find(e => e.Id == sm.Id);//根据返回id-->本地对象
                if (localMaterialDic != null)//本地存在--》更新
                {
                    //本地数据拼接属性
                    PropertyInfo[] p = localMaterialDic.GetType().GetProperties();
                    string S_WMS = "";//拼接
                    foreach (PropertyInfo pinfo in p)
                    {
                        var isContain = MaterialDicAnnotation.Contains(pinfo.Name);
                        if (isContain == true) { S_WMS = S_WMS + pinfo.GetValue(localMaterialDic, null); }
                    }
                    var swmsMd5 = GetMD5WithString(S_WMS);

                    //反馈数据拼接属性
                    PropertyInfo[] pe = sm.GetType().GetProperties();
                    string S_EBS = "";//拼接
                    foreach (PropertyInfo pinfo in pe)
                    {
                        var isContainPe = MaterialDicAnnotation.Contains(pinfo.Name);
                        if (isContainPe == true) { S_EBS = S_EBS + pinfo.GetValue(sm, null); }
                    }
                    var sebsMd5 = GetMD5WithString(S_EBS);

                    if (sebsMd5.Equals(swmsMd5))//是否有差别
                    {
                        //无差别 不操作
                    }
                    else
                    {
                        //dynamic dyp = new System.Dynamic.ExpandoObject();
                        //foreach (var L in OrganizationAnnotation)
                        //{
                        //    //此处不能写死****
                        //    localOrganization.OrgName = sm.OrgName;
                        //    localOrganization.OrgCode = sm.OrgCode;
                        //    localOrganization.OUId = sm.OUId;
                        //}

                        upMaterialDic.Add(localMaterialDic);
                    }

                }
                else //不存在--》添加 add
                {
                    MaterialDic addMaterialDicU = new MaterialDic();
                    //不能写死  *****
                    //foreach (var L in OrganizationAnnotation)
                    //{
                    //    //此处不能写死****
                    //    addOrganizationU.OrgName = sm.OrgName;
                    //    addOrganizationU.OrgCode = sm.OrgCode;
                    //    addOrganizationU.OUId = sm.OUId;
                    //}
                    addMaterialDic.Add(addMaterialDicU);
                }
            });
            await this._materialDicRepository.UpdateAsync(upMaterialDic);//一起更新
            await this._materialDicRepository.AddAsync(addMaterialDic);//一起添加

            #endregion
        }

        /// <summary>
        /// 供应商头信息同步
        /// </summary>
        private async void SupplierSyn()
        {
            #region 供应商头信息同步
            List<Supplier> serSupplier = new List<Supplier> //假设主数据返回
                {
                    new Supplier
                    {
                        Id =1
                    },new Supplier
                    {
                       Id =1
                    }
                };

            List<string> SupplierAnnotation = new List<string>();
            //本地实体类注解得属性
            PropertyInfo[] propertiesSupplier = typeof(Supplier).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo property in propertiesSupplier)
            {
                object[] proDescrition = property.GetCustomAttributes(typeof(DescriptionAttribute), true);
                if (proDescrition.Length > 0)
                {
                    SupplierAnnotation.Add(property.Name.ToString());
                }
            }

            SupplierIdSetSpecification supplierIdSetSpec = new SupplierIdSetSpecification(serSupplier.ConvertAll(e => e.Id));//本地查询
            List<Supplier> localSuppliers = await this._supplierRepository.ListAsync(supplierIdSetSpec);
            List<Supplier> updSupplier = new List<Supplier>();//更新部门对象集合
            List<Supplier> addSupplier = new List<Supplier>();//添加部门对象集合
            serSupplier.ForEach(sm =>//循环对象集合--》操作返回数据集合，对比本地数据对象，是否更新
            {
                Supplier localSupplier = localSuppliers.Find(e => e.Id == sm.Id);//根据返回id-->本地对象
                if (localSupplier != null)//本地存在--》更新
                {
                    //本地数据拼接属性
                    PropertyInfo[] p = localSupplier.GetType().GetProperties();
                    string S_WMS = "";//拼接
                    foreach (PropertyInfo pinfo in p)
                    {
                        var isContain = SupplierAnnotation.Contains(pinfo.Name);
                        if (isContain == true) { S_WMS = S_WMS + pinfo.GetValue(localSupplier, null); }
                    }
                    var swmsMd5 = GetMD5WithString(S_WMS);

                    //反馈数据拼接属性
                    PropertyInfo[] pe = sm.GetType().GetProperties();
                    string S_EBS = "";//拼接
                    foreach (PropertyInfo pinfo in pe)
                    {
                        var isContainPe = SupplierAnnotation.Contains(pinfo.Name);
                        if (isContainPe == true) { S_EBS = S_EBS + pinfo.GetValue(sm, null); }
                    }
                    var sebsMd5 = GetMD5WithString(S_EBS);

                    if (sebsMd5.Equals(swmsMd5))//是否有差别
                    {
                        //无差别 不操作
                    }
                    else
                    {
                        //dynamic dyp = new System.Dynamic.ExpandoObject();
                        //foreach (var L in OrganizationAnnotation)
                        //{
                        //    //此处不能写死****
                        //    localOrganization.OrgName = sm.OrgName;
                        //    localOrganization.OrgCode = sm.OrgCode;
                        //    localOrganization.OUId = sm.OUId;
                        //}

                        updSupplier.Add(localSupplier);
                    }

                }
                else //不存在--》添加 add
                {
                    Supplier addSupplierU = new Supplier();
                    //不能写死  *****
                    //foreach (var L in OrganizationAnnotation)
                    //{
                    //    //此处不能写死****
                    //    addOrganizationU.OrgName = sm.OrgName;
                    //    addOrganizationU.OrgCode = sm.OrgCode;
                    //    addOrganizationU.OUId = sm.OUId;
                    //}
                    addSupplier.Add(addSupplierU);
                }
            });
            await this._supplierRepository.UpdateAsync(updSupplier);//一起更新
            await this._supplierRepository.AddAsync(addSupplier);//一起添加

            #endregion
        }

        /// <summary>
        /// 供应商地址信息同步
        /// </summary>
        private async void SupplierSiteSyn()
        {
            #region 供应商地址信息同步
            List<SupplierSite> serSupplierSite = new List<SupplierSite> //假设主数据返回
                {
                    new SupplierSite
                    {
                        Id =1
                    },new SupplierSite
                    {
                       Id =1
                    }
                };

            List<string> SupplierSiteAnnotation = new List<string>();
            //本地实体类注解得属性
            PropertyInfo[] propertiesSupplierSite = typeof(SupplierSite).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo property in propertiesSupplierSite)
            {
                object[] proDescrition = property.GetCustomAttributes(typeof(DescriptionAttribute), true);
                if (proDescrition.Length > 0)
                {
                    SupplierSiteAnnotation.Add(property.Name.ToString());
                }
            }

            SupplierSiteIdSetSpecification supplierSiteIdSetSpec = new SupplierSiteIdSetSpecification(serSupplierSite.ConvertAll(e => e.Id));//本地查询
            List<SupplierSite> localSupplierSites = await this._supplierSiteRepository.ListAsync(supplierSiteIdSetSpec);
            List<SupplierSite> updSupplierSite = new List<SupplierSite>();//更新部门对象集合
            List<SupplierSite> addSupplierSite = new List<SupplierSite>();//添加部门对象集合
            serSupplierSite.ForEach(sm =>//循环对象集合--》操作返回数据集合，对比本地数据对象，是否更新
            {
                SupplierSite localSupplierSite = localSupplierSites.Find(e => e.Id == sm.Id);//根据返回id-->本地对象
                if (localSupplierSite != null)//本地存在--》更新
                {
                    //本地数据拼接属性
                    PropertyInfo[] p = localSupplierSite.GetType().GetProperties();
                    string S_WMS = "";//拼接
                    foreach (PropertyInfo pinfo in p)
                    {
                        var isContain = SupplierSiteAnnotation.Contains(pinfo.Name);
                        if (isContain == true) { S_WMS = S_WMS + pinfo.GetValue(localSupplierSite, null); }
                    }
                    var swmsMd5 = GetMD5WithString(S_WMS);

                    //反馈数据拼接属性
                    PropertyInfo[] pe = sm.GetType().GetProperties();
                    string S_EBS = "";//拼接
                    foreach (PropertyInfo pinfo in pe)
                    {
                        var isContainPe = SupplierSiteAnnotation.Contains(pinfo.Name);
                        if (isContainPe == true) { S_EBS = S_EBS + pinfo.GetValue(sm, null); }
                    }
                    var sebsMd5 = GetMD5WithString(S_EBS);

                    if (sebsMd5.Equals(swmsMd5))//是否有差别
                    {
                        //无差别 不操作
                    }
                    else
                    {
                        //dynamic dyp = new System.Dynamic.ExpandoObject();
                        //foreach (var L in OrganizationAnnotation)
                        //{
                        //    //此处不能写死****
                        //    localOrganization.OrgName = sm.OrgName;
                        //    localOrganization.OrgCode = sm.OrgCode;
                        //    localOrganization.OUId = sm.OUId;
                        //}

                        updSupplierSite.Add(localSupplierSite);
                    }

                }
                else //不存在--》添加 add
                {
                    SupplierSite addSupplierSiteU = new SupplierSite();
                    //不能写死  *****
                    //foreach (var L in OrganizationAnnotation)
                    //{
                    //    //此处不能写死****
                    //    addOrganizationU.OrgName = sm.OrgName;
                    //    addOrganizationU.OrgCode = sm.OrgCode;
                    //    addOrganizationU.OUId = sm.OUId;
                    //}
                    addSupplierSite.Add(addSupplierSiteU);
                }
            });
            await this._supplierSiteRepository.UpdateAsync(updSupplierSite);//一起更新
            await this._supplierSiteRepository.AddAsync(addSupplierSite);//一起添加

            #endregion
        }
   
    }
}