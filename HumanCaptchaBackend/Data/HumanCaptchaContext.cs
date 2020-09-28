using Microsoft.EntityFrameworkCore;

namespace HumanCaptchaBackend.Data
{
    public class HumanCaptchaContext: DbContext
    {
        public HumanCaptchaContext()
        {

        }
        public HumanCaptchaContext(DbContextOptions<HumanCaptchaContext> options): base(options)
        {
        }

        public DbSet<Captcha> Captchas { get; set; }
        public DbSet<Token> Tokens { get; set; }
        public DbSet<Exception> Exceptions { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Captcha>(entity =>
            {
                entity.HasKey(e => e.ID);
                entity.Property(e => e.Value).IsRequired();
                entity.Property(e => e.Guid).IsRequired();
                entity.Property(e => e.Expires).IsRequired();
            });

            modelBuilder.Entity<Token>(entity =>
            {
                entity.HasKey(e => e.ID);
                entity.Property(e => e.Value).IsRequired();
                entity.Property(e => e.Used).IsRequired();
                entity.Property(e => e.Expires).IsRequired();
            });

            modelBuilder.Entity<Exception>(entity =>
            {
                entity.HasKey(e => e.ID);
                entity.Property(e => e.ExceptionTime).HasColumnType("datetime");
                entity.Property(e => e.Message).IsRequired().IsUnicode(false);
                entity.Property(e => e.TypeName).IsRequired().IsUnicode(false);
                entity.Property(e => e.Message).IsUnicode(false);
                entity.Property(e => e.InnerStack).IsUnicode(false);
                entity.Property(e => e.StackTrace).IsUnicode(false);
            });
        }
    }
}
