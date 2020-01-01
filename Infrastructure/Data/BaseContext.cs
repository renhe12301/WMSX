using System;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using ApplicationCore.Entities.OrganizationManager;
using ApplicationCore.Entities.BasicInformation;

namespace Infrastructure.Data
{
    public class BaseContext: DbContext
    {

        public DbSet<Organization> Organization { get; set; }
        public DbSet<Warehouse> Warehouse { get; set; }
        public DbSet<SysUser> SysUser { get; set; }
        public DbSet<SysRole> SysRole { get; set; }

        public BaseContext(DbContextOptions<BaseContext> options) : base(options)
        {
        }


    }
}
