using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

#nullable disable

namespace RecycleSystem.DataEntity.Entities
{
    public partial class RecycleSystemDBContext : DbContext
    {
        public RecycleSystemDBContext()
        {
        }

        public RecycleSystemDBContext(DbContextOptions<RecycleSystemDBContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Categorylnfo> Categorylnfos { get; set; }
        public virtual DbSet<DemandOrderInfo> DemandOrderInfos { get; set; }
        public virtual DbSet<DepartmentInfo> DepartmentInfos { get; set; }
        public virtual DbSet<DeviceInfo> DeviceInfos { get; set; }
        public virtual DbSet<ImageInfo> ImageInfos { get; set; }
        public virtual DbSet<InputInfo> InputInfos { get; set; }
        public virtual DbSet<LoginLogInfo> LoginLogInfos { get; set; }
        public virtual DbSet<OperateLog> OperateLogs { get; set; }
        public virtual DbSet<OrderInfo> OrderInfos { get; set; }
        public virtual DbSet<PowerInfo> PowerInfos { get; set; }
        public virtual DbSet<RRolePowerInfo> RRolePowerInfos { get; set; }
        public virtual DbSet<RUserRoleInfo> RUserRoleInfos { get; set; }
        public virtual DbSet<RevenueBill> RevenueBills { get; set; }
        public virtual DbSet<RoleInfo> RoleInfos { get; set; }
        public virtual DbSet<UserInfo> UserInfos { get; set; }
        public virtual DbSet<UserType> UserTypes { get; set; }
        public virtual DbSet<WareHouseInfo> WareHouseInfos { get; set; }
        public virtual DbSet<WorkFlow> WorkFlows { get; set; }
        public virtual DbSet<WorkFlowStep> WorkFlowSteps { get; set; }
        public virtual DbSet<WorkFlowType> WorkFlowTypes { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Server=.;Database=RecycleSystemDB;Integrated Security=True");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "Chinese_PRC_CI_AS");

            modelBuilder.Entity<Categorylnfo>(entity =>
            {
                entity.ToTable("Categorylnfo");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.AddTime).HasColumnType("datetime");

                entity.Property(e => e.CategoryId).HasColumnName("CategoryID");

                entity.Property(e => e.CurrentPrice).HasColumnType("decimal(18, 0)");
            });

            modelBuilder.Entity<DemandOrderInfo>(entity =>
            {
                entity.ToTable("DemandOrderInfo");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.AddTime).HasColumnType("datetime");

                entity.Property(e => e.CategoryId).HasColumnName("CategoryID");

                entity.Property(e => e.EnterpriseId).HasColumnName("EnterpriseID");

                entity.Property(e => e.Oid).HasColumnName("OID");

                entity.Property(e => e.UserId).HasColumnName("UserID");
            });

            modelBuilder.Entity<DepartmentInfo>(entity =>
            {
                entity.ToTable("DepartmentInfo");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.AddTime).HasColumnType("datetime");

                entity.Property(e => e.DepartmentId).HasColumnName("DepartmentID");

                entity.Property(e => e.LeaderId).HasColumnName("LeaderID");

                entity.Property(e => e.ParentId).HasColumnName("ParentID");
            });

            modelBuilder.Entity<DeviceInfo>(entity =>
            {
                entity.ToTable("DeviceInfo");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.DeviceId).HasColumnName("DeviceID");
            });

            modelBuilder.Entity<ImageInfo>(entity =>
            {
                entity.ToTable("ImageInfo");

                entity.Property(e => e.Id).HasColumnName("ID");
            });

            modelBuilder.Entity<InputInfo>(entity =>
            {
                entity.ToTable("InputInfo");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.AddTime).HasColumnType("datetime");

                entity.Property(e => e.CategoryId).HasColumnName("CategoryID");

                entity.Property(e => e.InstanceId).HasColumnName("InstanceID");
            });

            modelBuilder.Entity<LoginLogInfo>(entity =>
            {
                entity.ToTable("LoginLogInfo");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.HappenTime).HasColumnType("datetime");

                entity.Property(e => e.Ip).HasColumnName("IP");

                entity.Property(e => e.IsLoginSuccess).HasColumnName("isLoginSuccess");

                entity.Property(e => e.UserId).HasColumnName("UserID");
            });

            modelBuilder.Entity<OperateLog>(entity =>
            {
                entity.ToTable("OperateLog");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.AddTime).HasColumnType("datetime");

                entity.Property(e => e.OperatorId).HasColumnName("OperatorID");
            });

            modelBuilder.Entity<OrderInfo>(entity =>
            {
                entity.ToTable("OrderInfo");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.AddTime).HasColumnType("datetime");

                entity.Property(e => e.CategoryId).HasColumnName("CategoryID");

                entity.Property(e => e.InstanceId).HasColumnName("InstanceID");

                entity.Property(e => e.EnterpriseId).HasColumnName("EnterpriseId");

                entity.Property(e => e.ReceiverId).HasColumnName("ReceiverId");
            });

            modelBuilder.Entity<PowerInfo>(entity =>
            {
                entity.ToTable("PowerInfo");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.ParentId).HasColumnName("ParentID");

                entity.Property(e => e.PowerId).HasColumnName("PowerID");
            });

            modelBuilder.Entity<RRolePowerInfo>(entity =>
            {
                entity.ToTable("R_RolePowerInfo");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.PowerId).HasColumnName("PowerID");

                entity.Property(e => e.RoleId).HasColumnName("RoleID");
            });

            modelBuilder.Entity<RUserRoleInfo>(entity =>
            {
                entity.ToTable("R_UserRoleInfo");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.RoleId).HasColumnName("RoleID");

                entity.Property(e => e.UserId).HasColumnName("UserID");
            });

            modelBuilder.Entity<RevenueBill>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.AddTime).HasColumnType("datetime");

                entity.Property(e => e.Money).HasColumnType("decimal(18, 0)");

                entity.Property(e => e.Oid).HasColumnName("OID");

                entity.Property(e => e.Zid).HasColumnName("ZID");
            });

            modelBuilder.Entity<RoleInfo>(entity =>
            {
                entity.ToTable("RoleInfo");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.AddTime).HasColumnType("datetime");

                entity.Property(e => e.DelTime).HasColumnType("datetime");

                entity.Property(e => e.RoleId).HasColumnName("RoleID");
            });

            modelBuilder.Entity<UserInfo>(entity =>
            {
                entity.ToTable("UserInfo");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.AddTime).HasColumnType("datetime");

                entity.Property(e => e.DelTime).HasColumnType("datetime");

                entity.Property(e => e.DepartmentId).HasColumnName("DepartmentID");

                entity.Property(e => e.UserId).HasColumnName("UserID");

                entity.Property(e => e.UserTypeId).HasColumnName("UserTypeID");
            });

            modelBuilder.Entity<UserType>(entity =>
            {
                entity.ToTable("UserType");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.TypeName).IsUnicode(false);
            });

            modelBuilder.Entity<WareHouseInfo>(entity =>
            {
                entity.ToTable("WareHouseInfo");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.AddTime).HasColumnType("datetime");

                entity.Property(e => e.CategoryId).HasColumnName("CategoryID");

                entity.Property(e => e.InstanceId).HasColumnName("InstanceID");

                entity.Property(e => e.IsPress).HasColumnName("isPress");
            });

            modelBuilder.Entity<WorkFlow>(entity =>
            {
                entity.ToTable("WorkFlow");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.AddTime).HasColumnType("datetime");

                entity.Property(e => e.InstanceId).HasColumnName("InstanceID");

                entity.Property(e => e.TypeId).HasColumnName("TypeID");

                entity.Property(e => e.UserId).HasColumnName("UserID");
            });

            modelBuilder.Entity<WorkFlowStep>(entity =>
            {
                entity.ToTable("WorkFlowStep");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.InstanceId).HasColumnName("InstanceID");

                entity.Property(e => e.ReviewTime).HasColumnType("datetime");

                entity.Property(e => e.ReviewerId).HasColumnName("ReviewerID");
            });

            modelBuilder.Entity<WorkFlowType>(entity =>
            {
                entity.ToTable("WorkFlowType");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.AddTime).HasColumnType("datetime");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
