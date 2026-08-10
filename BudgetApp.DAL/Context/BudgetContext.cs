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

    public virtual DbSet<BudgetPriority> BudgetPriorities { get; set; }

    public virtual DbSet<EstablishedLink> EstablishedLinks { get; set; }

    public virtual DbSet<MonthlyBudget> MonthlyBudgets { get; set; }

    public virtual DbSet<SpendingBucket> SpendingBuckets { get; set; }

    public virtual DbSet<Transaction> Transactions { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            throw new InvalidOperationException("BudgetContext must be configured with a connection string via dependency injection.");
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<BudgetPriority>(entity =>
        {
            entity.HasKey(e => e.PriorityId).HasName("PK_PriorityID");

            entity.Property(e => e.PriorityId).HasColumnName("PriorityID");
            entity.Property(e => e.Description)
                .HasMaxLength(50)
                .IsRequired();
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
            entity.Property(e => e.Currency)
                .HasMaxLength(3).IsFixedLength()
                .IsRequired();

            entity.HasOne(d => d.Bucket).WithMany(p => p.MonthlyBudgets)
                .HasForeignKey(d => d.BucketId)
                .HasConstraintName("FK_MonthlyBudget_SpendingBuckets");
        });

        modelBuilder.Entity<SpendingBucket>(entity =>
        {
            entity.HasKey(e => e.BucketId).HasName("PK_BucketID");

            entity.Property(e => e.BucketId).HasColumnName("BucketID");
            entity.Property(e => e.BucketLabel)
                .HasMaxLength(50)
                .IsRequired();
            entity.Property(e => e.DefaultPriority);
            entity.Property(e => e.DisplayOrder);

            entity.HasOne(d => d.Priority).WithMany(p => p.Buckets)
                .HasForeignKey(d => d.DefaultPriority)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SpendingBuckets_BudgetPriorities");
        });

        modelBuilder.Entity<Transaction>(entity =>
        {
            entity.HasKey(e => e.TransactionId).HasName("PK_TransactionID");

            entity.Property(e => e.TransactionId).HasColumnName("TransactionID");
            entity.Property(e => e.TransactionDate).HasColumnType("date")
                .IsRequired();
            entity.Property(e => e.Debit).HasColumnType("bit")
                .IsRequired();
            entity.Property(e => e.Amount).HasColumnType("decimal(19, 2)")
                .IsRequired();
            entity.Property(e => e.BucketId).HasColumnName("BucketID");
            entity.Property(e => e.Card)
                .HasMaxLength(8);
            entity.Property(e => e.Category).HasMaxLength(50);
            entity.Property(e => e.Description).HasMaxLength(150);
            entity.Property(e => e.Reference).HasMaxLength(25);
            entity.Property(e => e.Priority);
            entity.Property(e => e.Currency)
                .HasMaxLength(3).IsFixedLength()
                .IsRequired();

            entity.HasOne(d => d.Bucket).WithMany(p => p.Transactions)
                .HasForeignKey(d => d.BucketId)
                .HasConstraintName("FK_Transactions_SpendingBuckets");

            
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
