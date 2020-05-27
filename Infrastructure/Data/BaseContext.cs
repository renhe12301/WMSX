using System;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using ApplicationCore.Entities.AuthorityManager;
using ApplicationCore.Entities.BasicInformation;
using ApplicationCore.Entities.FlowRecord;
using ApplicationCore.Entities.OrderManager;
using ApplicationCore.Entities.StockManager;
using ApplicationCore.Entities.TaskManager;
using LogRecord = ApplicationCore.Entities.FlowRecord.LogRecord;
using ApplicationCore.Entities.SysManager;

namespace Infrastructure.Data
{
    public class BaseContext: DbContext
    {

        public DbSet<Organization> Organization { get; set; }
        public DbSet<OU> OU { get; set; }
        public DbSet<Warehouse> Warehouse { get; set; }
        public DbSet<SysRole> SysRole { get; set; }
        public DbSet<SysMenu> SysMenu { get; set; }
        public DbSet<ReservoirArea> ReservoirArea { get; set; }
        public DbSet<RoleMenu> RoleMenu { get; set; }
        public DbSet<Employee> Employee { get; set; }
        public DbSet<EmployeeRole> EmployeeRole { get; set; }
        public DbSet<MaterialType> MaterialType { get; set; }
        public DbSet<MaterialDic> MaterialDic { get; set; }
        public DbSet<Location> Location { get; set; }
        public DbSet<WarehouseMaterial> WarehouseMaterial { get; set; }
        public DbSet<WarehouseTray> WarehouseTray { get; set; }
        public DbSet<InOutTask> InOutTask { get; set; }
        public DbSet<Supplier> Supplier { get; set; }
        public DbSet<SupplierSite> SupplierSite { get; set; }
        public DbSet<PhyWarehouse> PhyWarehouse { get; set; }
        public DbSet<Order> Order { get; set; }
        public DbSet<OrderRow> OrderRow { get; set; }
        public DbSet<EBSProject> EBSProject { get; set; }
        public DbSet<EBSTask> EBSTask { get; set; }
        public DbSet<LogRecord> LogRecord { get; set; }
        public DbSet<SysConfig> SysConfig { get; set; }
        public DbSet<SubOrder> SubOrder { get; set; }
        public DbSet<SubOrderRow> SubOrderRow { get; set; }

        public BaseContext(DbContextOptions<BaseContext> options) : base(options)
        {
        }


    }
}
