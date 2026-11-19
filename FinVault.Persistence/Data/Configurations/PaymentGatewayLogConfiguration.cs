using FinVault.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FinVault.Persistence.Data.Configurations;

public class PaymentGatewayLogConfiguration : IEntityTypeConfiguration<PaymentGatewayLog>
{
    public void Configure(EntityTypeBuilder<PaymentGatewayLog> builder)
    {
        builder.ToTable("PaymentGatewayLogs", t =>
        {
            t.HasCheckConstraint("CK_PaymentGatewayLogs_Operation", "[Operation] IN ('Initiate','StatusCheck','Callback','Reversal','Refund')");
        });

        // Primary key
        builder.HasKey(e => e.PaymentGatewayLogId);

        builder.Property(e => e.PaymentGatewayLogId)
            .ValueGeneratedOnAdd();

        // Properties
        builder.Property(e => e.GatewayName)
            .IsRequired()
            .HasMaxLength(50);
        builder.Property(e => e.Operation)
            .IsRequired()
            .HasMaxLength(50);
        builder.Property(e => e.CorrelationId)
            .IsRequired()
            .HasMaxLength(64);
        builder.Property(e => e.RequestPayload)
            .HasColumnType("nvarchar(max)");
        builder.Property(e => e.ResponsePayload)
            .HasColumnType("nvarchar(max)");
        builder.Property(e => e.ResponseCode)
            .HasMaxLength(30);
        builder.Property(e => e.ResponseMessage)
            .HasMaxLength(500);
        builder.Property(e => e.ErrorMessage)
            .HasMaxLength(1000);

        // Audit
        builder.Property(e => e.CreatedAt)
            .HasDefaultValueSql("SYSUTCDATETIME()");

        // Indexes
        builder.HasIndex(e => new { e.PaymentRequestId, e.CreatedAt })
            .HasDatabaseName("IX_PaymentGatewayLogs_PaymentRequestId_CreatedAt");

        builder.HasIndex(e => e.CorrelationId)
            .HasDatabaseName("IX_PaymentGatewayLogs_CorrelationId");

        // Relationships
        builder.HasOne(e => e.PaymentRequest)
            .WithMany(p => p.GatewayLogs)
            .HasForeignKey(e => e.PaymentRequestId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
