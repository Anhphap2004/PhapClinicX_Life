using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace PhapClinicX.Models;

public partial class ClinicManagementContext : DbContext
{
    public ClinicManagementContext()
    {
    }

    public ClinicManagementContext(DbContextOptions<ClinicManagementContext> options)
        : base(options)
    {
    }

    public virtual DbSet<About> Abouts { get; set; }

    public virtual DbSet<Blog> Blogs { get; set; }

    public virtual DbSet<BlogCategory> BlogCategories { get; set; }

    public virtual DbSet<BlogComment> BlogComments { get; set; }

    public virtual DbSet<Cart> Carts { get; set; }

    public virtual DbSet<ClinicAppointment> ClinicAppointments { get; set; }

    public virtual DbSet<Contact> Contacts { get; set; }

    public virtual DbSet<DoctorAppointment> DoctorAppointments { get; set; }

    public virtual DbSet<DoctorProfile> DoctorProfiles { get; set; }

    public virtual DbSet<Faq> Faqs { get; set; }

    public virtual DbSet<Invoice> Invoices { get; set; }

    public virtual DbSet<InvoiceDetail> InvoiceDetails { get; set; }

    public virtual DbSet<MedicalEquipment> MedicalEquipments { get; set; }

    public virtual DbSet<Menu> Menus { get; set; }

    public virtual DbSet<Payment> Payments { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<ProductCategory> ProductCategories { get; set; }

    public virtual DbSet<ProductComment> ProductComments { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Service> Services { get; set; }

    public virtual DbSet<ServicePackage> ServicePackages { get; set; }

    public virtual DbSet<ServiceReview> ServiceReviews { get; set; }

    public virtual DbSet<User> Users { get; set; }

   

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<About>(entity =>
        {
            entity.HasKey(e => e.AboutId).HasName("PK__About__D4C5C6DA85BFB051");

            entity.ToTable("About");

            entity.Property(e => e.AboutId).HasColumnName("about_id");
            entity.Property(e => e.Content).HasColumnName("content");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.Image)
                .HasMaxLength(255)
                .HasColumnName("image");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("is_active");
            entity.Property(e => e.Title)
                .HasMaxLength(255)
                .HasColumnName("title");
        });

        modelBuilder.Entity<Blog>(entity =>
        {
            entity.HasKey(e => e.BlogId).HasName("PK__Blogs__2975AA2827F44517");

            entity.Property(e => e.BlogId).HasColumnName("blog_id");
            entity.Property(e => e.AuthorId).HasColumnName("author_id");
            entity.Property(e => e.CategoryId).HasColumnName("category_id");
            entity.Property(e => e.Content).HasColumnName("content");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.ExtraContent).HasColumnName("extra_content");
            entity.Property(e => e.Image)
                .HasMaxLength(255)
                .HasColumnName("image");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("is_active");
            entity.Property(e => e.Title)
                .HasMaxLength(255)
                .HasColumnName("title");

            entity.HasOne(d => d.Author).WithMany(p => p.Blogs)
                .HasForeignKey(d => d.AuthorId)
                .HasConstraintName("FK__Blogs__author_id__5812160E");

            entity.HasOne(d => d.Category).WithMany(p => p.Blogs)
                .HasForeignKey(d => d.CategoryId)
                .HasConstraintName("FK__Blogs__category___571DF1D5");
        });

        modelBuilder.Entity<BlogCategory>(entity =>
        {
            entity.HasKey(e => e.CategoryId).HasName("PK__BlogCate__D54EE9B4B51A1C87");

            entity.Property(e => e.CategoryId).HasColumnName("category_id");
            entity.Property(e => e.CategoryName)
                .HasMaxLength(255)
                .HasColumnName("category_name");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("is_active");
        });

        modelBuilder.Entity<BlogComment>(entity =>
        {
            entity.HasKey(e => e.CommentId).HasName("PK__BlogComm__E7957687F6961B77");

            entity.Property(e => e.CommentId).HasColumnName("comment_id");
            entity.Property(e => e.BlogId).HasColumnName("blog_id");
            entity.Property(e => e.Comment).HasColumnName("comment");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Blog).WithMany(p => p.BlogComments)
                .HasForeignKey(d => d.BlogId)
                .HasConstraintName("FK__BlogComme__blog___5BE2A6F2");

            entity.HasOne(d => d.User).WithMany(p => p.BlogComments)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__BlogComme__user___5CD6CB2B");
        });

        modelBuilder.Entity<Cart>(entity =>
        {
            entity.HasKey(e => e.CartId).HasName("PK__Cart__2EF52A27C3454EDA");

            entity.ToTable("Cart");

            entity.Property(e => e.CartId).HasColumnName("cart_id");
            entity.Property(e => e.ProductId).HasColumnName("product_id");
            entity.Property(e => e.Quantity).HasColumnName("quantity");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Product).WithMany(p => p.Carts)
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("FK__Cart__product_id__778AC167");

            entity.HasOne(d => d.User).WithMany(p => p.Carts)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__Cart__user_id__76969D2E");
        });

        modelBuilder.Entity<ClinicAppointment>(entity =>
        {
            entity.HasKey(e => e.AppointmentId).HasName("PK__ClinicAp__A50828FC6F889F8F");

            entity.Property(e => e.AppointmentId).HasColumnName("appointment_id");
            entity.Property(e => e.ClinicId).HasColumnName("clinic_id");
            entity.Property(e => e.DateTime)
                .HasColumnType("datetime")
                .HasColumnName("date_time");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasColumnName("status");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.User).WithMany(p => p.ClinicAppointments)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__ClinicApp__user___6383C8BA");
        });

        modelBuilder.Entity<Contact>(entity =>
        {
            entity.HasKey(e => e.ContactId).HasName("PK__Contacts__024E7A860D8AC1FC");

            entity.Property(e => e.ContactId).HasColumnName("contact_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.Message).HasColumnName("message");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.User).WithMany(p => p.Contacts)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__Contacts__user_i__60A75C0F");
        });

        modelBuilder.Entity<DoctorAppointment>(entity =>
        {
            entity.HasKey(e => e.AppointmentId).HasName("PK__DoctorAp__A50828FCA18705B9");

            entity.Property(e => e.AppointmentId).HasColumnName("appointment_id");
            entity.Property(e => e.DateTime)
                .HasColumnType("datetime")
                .HasColumnName("date_time");
            entity.Property(e => e.DoctorId).HasColumnName("doctor_id");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasColumnName("status");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Doctor).WithMany(p => p.DoctorAppointmentDoctors)
                .HasForeignKey(d => d.DoctorId)
                .HasConstraintName("FK__DoctorApp__docto__6754599E");

            entity.HasOne(d => d.User).WithMany(p => p.DoctorAppointmentUsers)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__DoctorApp__user___66603565");
        });

        modelBuilder.Entity<DoctorProfile>(entity =>
        {
            entity.HasKey(e => e.DoctorId).HasName("PK__DoctorPr__F39935644BA7BC59");

            entity.Property(e => e.DoctorId)
                .ValueGeneratedNever()
                .HasColumnName("doctor_id");
            entity.Property(e => e.Education).HasColumnName("education");
            entity.Property(e => e.Experience).HasColumnName("experience");
            entity.Property(e => e.Specialization)
                .HasMaxLength(255)
                .HasColumnName("specialization");

            entity.HasOne(d => d.Doctor).WithOne(p => p.DoctorProfile)
                .HasForeignKey<DoctorProfile>(d => d.DoctorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__DoctorPro__docto__6A30C649");
        });

        modelBuilder.Entity<Faq>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__FAQ__3214EC27067D77BA");

            entity.ToTable("FAQ");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("ID");
            entity.Property(e => e.Answer).HasColumnType("text");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Question)
                .HasMaxLength(500)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Invoice>(entity =>
        {
            entity.HasKey(e => e.InvoiceId).HasName("PK__Invoices__F58DFD49C4B29A2D");

            entity.Property(e => e.InvoiceId).HasColumnName("invoice_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasColumnName("status");
            entity.Property(e => e.TotalAmount)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("total_amount");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.User).WithMany(p => p.Invoices)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__Invoices__user_i__6E01572D");
        });

        modelBuilder.Entity<InvoiceDetail>(entity =>
        {
            entity.HasKey(e => e.DetailId).HasName("PK__InvoiceD__38E9A22458F37023");

            entity.Property(e => e.DetailId).HasColumnName("detail_id");
            entity.Property(e => e.InvoiceId).HasColumnName("invoice_id");
            entity.Property(e => e.Price)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("price");
            entity.Property(e => e.Quantity).HasColumnName("quantity");
            entity.Property(e => e.ServiceId).HasColumnName("service_id");

            entity.HasOne(d => d.Invoice).WithMany(p => p.InvoiceDetails)
                .HasForeignKey(d => d.InvoiceId)
                .HasConstraintName("FK__InvoiceDe__invoi__70DDC3D8");
        });

        modelBuilder.Entity<MedicalEquipment>(entity =>
        {
            entity.HasKey(e => e.EquipmentId).HasName("PK__MedicalE__197068AF597B6F1E");

            entity.ToTable("MedicalEquipment");

            entity.Property(e => e.EquipmentId).HasColumnName("equipment_id");
            entity.Property(e => e.EquipmentName)
                .HasMaxLength(255)
                .HasColumnName("equipment_name");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("is_active");
            entity.Property(e => e.Quantity).HasColumnName("quantity");
        });

        modelBuilder.Entity<Menu>(entity =>
        {
            entity.HasKey(e => e.MenuId).HasName("PK__Menus__4CA0FADCC1D375CD");

            entity.Property(e => e.MenuId).HasColumnName("menu_id");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("is_active");
            entity.Property(e => e.MenuName)
                .HasMaxLength(255)
                .HasColumnName("menu_name");
            entity.Property(e => e.ParentId).HasColumnName("parent_id");
            entity.Property(e => e.Position).HasColumnName("position");
            entity.Property(e => e.Url)
                .HasMaxLength(255)
                .HasColumnName("url");

            entity.HasOne(d => d.Parent).WithMany(p => p.InverseParent)
                .HasForeignKey(d => d.ParentId)
                .HasConstraintName("FK__Menus__parent_id__3A81B327");
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(e => e.PaymentId).HasName("PK__Payments__ED1FC9EA3AF59D27");

            entity.Property(e => e.PaymentId).HasColumnName("payment_id");
            entity.Property(e => e.Amount)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("amount");
            entity.Property(e => e.InvoiceId).HasColumnName("invoice_id");
            entity.Property(e => e.Method)
                .HasMaxLength(50)
                .HasColumnName("method");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasColumnName("status");

            entity.HasOne(d => d.Invoice).WithMany(p => p.Payments)
                .HasForeignKey(d => d.InvoiceId)
                .HasConstraintName("FK__Payments__invoic__73BA3083");
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.ProductId).HasName("PK__Products__47027DF521AFC1FB");

            entity.Property(e => e.ProductId).HasColumnName("product_id");
            entity.Property(e => e.CategoryId).HasColumnName("category_id");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.Image)
                .HasMaxLength(255)
                .HasColumnName("image");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("is_active");
            entity.Property(e => e.Price)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("price");
            entity.Property(e => e.ProductName)
                .HasMaxLength(255)
                .HasColumnName("product_name");
            entity.Property(e => e.Specifications).HasColumnName("specifications");
            entity.Property(e => e.Warranty)
                .HasMaxLength(255)
                .HasColumnName("warranty");

            entity.HasOne(d => d.Category).WithMany(p => p.Products)
                .HasForeignKey(d => d.CategoryId)
                .HasConstraintName("FK__Products__catego__49C3F6B7");
        });

        modelBuilder.Entity<ProductCategory>(entity =>
        {
            entity.HasKey(e => e.CategoryId).HasName("PK__ProductC__D54EE9B42D25C44E");

            entity.Property(e => e.CategoryId).HasColumnName("category_id");
            entity.Property(e => e.CategoryName)
                .HasMaxLength(255)
                .HasColumnName("category_name");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("is_active");
        });

        modelBuilder.Entity<ProductComment>(entity =>
        {
            entity.HasKey(e => e.CommentId).HasName("PK__ProductC__E7957687754AEF5D");

            entity.Property(e => e.CommentId).HasColumnName("comment_id");
            entity.Property(e => e.Comment).HasColumnName("comment");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.ProductId).HasColumnName("product_id");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Product).WithMany(p => p.ProductComments)
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("FK__ProductCo__produ__7B5B524B");

            entity.HasOne(d => d.User).WithMany(p => p.ProductComments)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__ProductCo__user___7C4F7684");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("PK__Roles__760965CCBCE7299B");

            entity.Property(e => e.RoleId).HasColumnName("role_id");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.RoleName)
                .HasMaxLength(100)
                .HasColumnName("role_name");
        });

        modelBuilder.Entity<Service>(entity =>
        {
            entity.HasKey(e => e.ServiceId).HasName("PK__Services__3E0DB8AF345B94A6");

            entity.Property(e => e.ServiceId).HasColumnName("service_id");
            entity.Property(e => e.Detail)
                .HasMaxLength(255)
                .HasColumnName("detail");
            entity.Property(e => e.Image)
                .HasMaxLength(255)
                .HasColumnName("image");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("is_active");
            entity.Property(e => e.Price)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("price");
            entity.Property(e => e.ServiceName)
                .HasMaxLength(255)
                .HasColumnName("service_name");
        });

        modelBuilder.Entity<ServicePackage>(entity =>
        {
            entity.HasKey(e => e.PackageId).HasName("PK__ServiceP__63846AE8E83AB9B7");

            entity.Property(e => e.PackageId).HasColumnName("package_id");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("is_active");
            entity.Property(e => e.PackageName)
                .HasMaxLength(255)
                .HasColumnName("package_name");
            entity.Property(e => e.Price)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("price");
        });

        modelBuilder.Entity<ServiceReview>(entity =>
        {
            entity.HasKey(e => e.ReviewId).HasName("PK__ServiceR__60883D905B63FA21");

            entity.Property(e => e.ReviewId).HasColumnName("review_id");
            entity.Property(e => e.Comment).HasColumnName("comment");
            entity.Property(e => e.Rating).HasColumnName("rating");
            entity.Property(e => e.ServiceId).HasColumnName("service_id");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Service).WithMany(p => p.ServiceReviews)
                .HasForeignKey(d => d.ServiceId)
                .HasConstraintName("FK__ServiceRe__servi__00200768");

            entity.HasOne(d => d.User).WithMany(p => p.ServiceReviews)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__ServiceRe__user___01142BA1");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__Users__B9BE370FB195C7CB");

            entity.HasIndex(e => e.Email, "UQ__Users__AB6E616410D92C39").IsUnique();

            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.Address).HasColumnName("address");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.Email)
                .HasMaxLength(255)
                .HasColumnName("email");
            entity.Property(e => e.FullName)
                .HasMaxLength(255)
                .HasColumnName("full_name");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("is_active");
            entity.Property(e => e.PasswordHash)
                .HasMaxLength(255)
                .HasColumnName("password_hash");
            entity.Property(e => e.Phone)
                .HasMaxLength(20)
                .HasColumnName("phone");
            entity.Property(e => e.ProfileImage)
                .HasMaxLength(255)
                .HasColumnName("profile_image");
            entity.Property(e => e.RoleId).HasColumnName("role_id");

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .HasForeignKey(d => d.RoleId)
                .HasConstraintName("FK__Users__role_id__52593CB8");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
