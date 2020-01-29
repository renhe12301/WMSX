using System;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using ApplicationCore.Entities.OrganizationManager;
using ApplicationCore.Entities.BasicInformation;
using ApplicationCore.Entities.StockManager;

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
        public DbSet<EmployeeOrg> EmployeeOrg { get; set; }
        public DbSet<MaterialType> MaterialType { get; set; }
        public DbSet<MaterialDic> MaterialDic { get; set; }
        public DbSet<MaterialDicType> MaterialDicType { get; set; }
        public DbSet<MaterialUnit> MaterialUnit { get; set; }
        public DbSet<Location> Location { get; set; }
        public DbSet<WarehouseMaterial> WarehouseMaterial { get; set; }
        public DbSet<WarehouseTray> WarehouseTray { get; set; }
        public DbSet<MaterialDicTypeArea> MaterialDicTypeArea { get; set; }

        public BaseContext(DbContextOptions<BaseContext> options) : base(options)
        {
        }


    }
}
