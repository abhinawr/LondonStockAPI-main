using LondonStockApi.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace LondonStockApi.Data
{
    public class StockDbContext : DbContext
    {
        public StockDbContext(DbContextOptions<StockDbContext> options) : base(options) { }

        public DbSet<Trade> Trades { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Trade>(entity =>
            {
                entity.HasKey(e => e.TradeId);
                entity.Property(e => e.TickerSymbol).IsRequired().HasMaxLength(10);
                entity.Property(e => e.Price).HasColumnType("decimal(18,4)").IsRequired();
                entity.Property(e => e.Shares).HasColumnType("decimal(18,4)").IsRequired();
                entity.Property(e => e.BrokerId).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Timestamp).IsRequired();
                // For SQL Server when not using In-Memory
                // entity.Property(e => e.Timestamp).HasDefaultValueSql("GETUTCDATE()");


                entity.HasIndex(e => e.TickerSymbol).HasDatabaseName("IX_Trades_TickerSymbol");
                entity.HasIndex(e => new { e.TickerSymbol, e.Timestamp }).HasDatabaseName("IX_Trades_TickerSymbol_Timestamp");
                entity.HasIndex(e => e.Timestamp).HasDatabaseName("IX_Trades_Timestamp");
            });
        }
    }
}