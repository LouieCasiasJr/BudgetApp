using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace BudgetApp.DAL;

public partial class BudgetContext : DbContext
{
    public BudgetContext()
    {
    }

    public BudgetContext(DbContextOptions<BudgetContext> options)
        : base(options)
    {
    }

    public virtual DbSet<CapitalOneTransaction> CapitalOneTransactions { get; set; }

    public virtual DbSet<ChaseTransaction> ChaseTransactions { get; set; }

    public virtual DbSet<EstablishedLink> EstablishedLinks { get; set; }

    public virtual DbSet<MonthlyBudget> MonthlyBudgets { get; set; }

    public virtual DbSet<SpendingBucket> SpendingBuckets { get; set; }

    public virtual DbSet<Transaction> Transactions { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source=localhost;Initial Catalog=Budget;integrated security=True;trustservercertificate=True;MultipleActiveResultSets=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CapitalOneTransaction>(entity =>
        {
            entity.HasKey(e => e.CapitalOneTransactionId).HasName("PK_CapitalOneTransactionID");

            entity.Property(e => e.CapitalOneTransactionId).HasColumnName("CapitalOneTransactionID");
            entity.Property(e => e.Card)
                .HasMaxLength(4)
                .IsFixedLength();
            entity.Property(e => e.Category).HasMaxLength(50);
            entity.Property(e => e.Credit).HasColumnType("decimal(19, 2)");
            entity.Property(e => e.Debit).HasColumnType("decimal(19, 2)");
            entity.Property(e => e.Description).HasMaxLength(80);
        });

        modelBuilder.Entity<ChaseTransaction>(entity =>
        {
            entity.HasKey(e => e.ChaseTransactionId).HasName("PK_ChaseTransactionID");

            entity.Property(e => e.ChaseTransactionId).HasColumnName("ChaseTransactionID");
            entity.Property(e => e.Amount).HasColumnType("decimal(19, 2)");
            entity.Property(e => e.Description).HasMaxLength(80);
            entity.Property(e => e.Details)
                .HasMaxLength(10)
                .IsFixedLength();
            entity.Property(e => e.RefNumber)
                .HasMaxLength(10)
                .IsFixedLength();
            entity.Property(e => e.Type)
                .HasMaxLength(10)
                .IsFixedLength();
        });

        modelBuilder.Entity<EstablishedLink>(entity =>
        {
            entity.HasKey(e => e.LinkId).HasName("PK_LinkID");

            entity.Property(e => e.LinkId).HasColumnName("LinkID");
            entity.Property(e => e.BucketId).HasColumnName("BucketID");
            entity.Property(e => e.Category).HasMaxLength(50);
            entity.Property(e => e.ContainsText).HasMaxLength(50);

            entity.HasOne(d => d.Bucket).WithMany(p => p.EstablishedLinks)
                .HasForeignKey(d => d.BucketId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_EstablishedLinks_SpendingBuckets");
        });

        modelBuilder.Entity<MonthlyBudget>(entity =>
        {
            entity.HasKey(e => e.MonthlyBudgetId).HasName("PK_MonthlyBudgetID");

            entity.ToTable("MonthlyBudget");

            entity.Property(e => e.MonthlyBudgetId).HasColumnName("MonthlyBudgetID");
            entity.Property(e => e.Amount).HasColumnType("decimal(19, 2)");
            entity.Property(e => e.BucketId).HasColumnName("BucketID");

            entity.HasOne(d => d.Bucket).WithMany(p => p.MonthlyBudgets)
                .HasForeignKey(d => d.BucketId)
                .HasConstraintName("FK_MonthlyBudget_SpendingBuckets");
        });

        modelBuilder.Entity<SpendingBucket>(entity =>
        {
            entity.HasKey(e => e.BucketId).HasName("PK_BucketID");

            entity.Property(e => e.BucketId).HasColumnName("BucketID");
            entity.Property(e => e.BucketLabel).HasMaxLength(50);
            entity.Property(e => e.DefaultPriority).HasColumnName("DefaultPriority");
            entity.Property(e => e.DisplayOrder).HasColumnName("DisplayOrder");
        });

        modelBuilder.Entity<Transaction>(entity =>
        {
            entity.HasKey(e => e.TransactionId).HasName("PK_TransactionID");

            entity.Property(e => e.TransactionId).HasColumnName("TransactionID");
            entity.Property(e => e.Amount).HasColumnType("decimal(19, 2)");
            entity.Property(e => e.BucketId).HasColumnName("BucketID");
            entity.Property(e => e.Card)
                .HasMaxLength(8)
                .IsFixedLength();
            entity.Property(e => e.Category).HasMaxLength(50);
            entity.Property(e => e.Description).HasMaxLength(150);
            entity.Property(e => e.Reference).HasMaxLength(25);
            entity.Property(e => e.Priority).HasColumnName("Priority");

            entity.HasOne(d => d.Bucket).WithMany(p => p.Transactions)
                .HasForeignKey(d => d.BucketId)
                .HasConstraintName("FK_Transactions_SpendingBuckets");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
