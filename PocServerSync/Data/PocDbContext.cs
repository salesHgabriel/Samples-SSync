using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using PocServerSync.Models;
using SSync.Server.LitebDB.Abstractions;

namespace PocServerSync.Data
{
    public class PocDbContext : DbContext, ISSyncDbContextTransaction
    {
        private IDbContextTransaction? _transaction;
        private readonly IConfiguration _configuration;

        public PocDbContext(DbContextOptions<PocDbContext> options, IConfiguration configuration) : base(options)
        {
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
            AppContext.SetSwitch("Npgsql.DisableDateTimeInfinityConversions", true);
            _configuration = configuration;
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Note> Notes { get; set; }
        public DbSet<Doc> Docs { get; set; }


        public async Task BeginTransactionSyncAsync()
            => _transaction = await Database.BeginTransactionAsync();

        public async Task CommitSyncAsync()
            => await Database.CommitTransactionAsync();

        public Task CommitTransactionSyncAsync()
        {
            ArgumentNullException.ThrowIfNull(_transaction);

            return _transaction.CommitAsync();
        }

        public Task RollbackTransactionSyncAsync()
        {
            ArgumentNullException.ThrowIfNull(_transaction);
            return _transaction.RollbackAsync();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql(_configuration.GetConnectionString("PocServerSync"));
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Note>()
                .HasOne(n => n.User)
                .WithMany(n => n.Notes)
                .OnDelete(DeleteBehavior.Restrict);


            
            modelBuilder.Entity<User>()
                .HasMany(n => n.Notes)
                .WithOne(n => n.User)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}