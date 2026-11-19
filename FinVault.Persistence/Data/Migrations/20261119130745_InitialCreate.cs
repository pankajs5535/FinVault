using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FinVault.Persistence.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AccountHolders",
                columns: table => new
                {
                    AccountHolderId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HolderType = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    CanOperate = table.Column<bool>(type: "bit", nullable: false),
                    EffectiveFrom = table.Column<DateTime>(type: "date", nullable: false),
                    EffectiveTo = table.Column<DateTime>(type: "date", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    AccountId = table.Column<int>(type: "int", nullable: false),
                    CustomerId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<int>(type: "int", nullable: true),
                    UpdatedBy = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountHolders", x => x.AccountHolderId);
                    table.CheckConstraint("CK_AccountHolders_HolderType", "[HolderType] IN ('Primary','Joint','Guardian','AuthorizedSignatory')");
                });

            migrationBuilder.CreateTable(
                name: "Accounts",
                columns: table => new
                {
                    AccountId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AccountNumber = table.Column<string>(type: "nvarchar(18)", maxLength: 18, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    StatusReason = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    ModeOfOperation = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Balance = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    AvailableBalance = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    HoldAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    OverdraftLimit = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    OpenedOn = table.Column<DateTime>(type: "date", nullable: false),
                    ClosedOn = table.Column<DateTime>(type: "date", nullable: true),
                    LastTransactionAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    NomineeName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    NomineeRelationship = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    PrincipalAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    MaturityAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    ContractedInterestRate = table.Column<decimal>(type: "decimal(9,4)", precision: 9, scale: 4, nullable: true),
                    TenureMonths = table.Column<int>(type: "int", nullable: true),
                    MaturityDate = table.Column<DateTime>(type: "date", nullable: true),
                    AutoRenew = table.Column<bool>(type: "bit", nullable: false),
                    AccountTypeId = table.Column<int>(type: "int", nullable: false),
                    BranchId = table.Column<int>(type: "int", nullable: false),
                    CurrencyId = table.Column<int>(type: "int", nullable: false),
                    PayoutAccountId = table.Column<int>(type: "int", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<int>(type: "int", nullable: true),
                    UpdatedBy = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Accounts", x => x.AccountId);
                    table.CheckConstraint("CK_Accounts_HoldAmount", "[HoldAmount] >= 0");
                    table.CheckConstraint("CK_Accounts_ModeOfOperation", "[ModeOfOperation] IN ('Single','Joint','EitherOrSurvivor','FormerOrSurvivor')");
                    table.CheckConstraint("CK_Accounts_OverdraftLimit", "[OverdraftLimit] >= 0");
                    table.CheckConstraint("CK_Accounts_Status", "[Status] IN ('Pending','Active','Dormant','Inactive','Frozen','Closed')");
                    table.ForeignKey(
                        name: "FK_Accounts_Accounts_PayoutAccountId",
                        column: x => x.PayoutAccountId,
                        principalTable: "Accounts",
                        principalColumn: "AccountId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AccountTypes",
                columns: table => new
                {
                    AccountTypeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Category = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    MinimumBalance = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    MinimumAverageBalance = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    DailyTransactionLimit = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    AllowsOverdraft = table.Column<bool>(type: "bit", nullable: false),
                    IsInterestBearing = table.Column<bool>(type: "bit", nullable: false),
                    InterestPayoutFrequency = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    MinTenureMonths = table.Column<int>(type: "int", nullable: true),
                    MaxTenureMonths = table.Column<int>(type: "int", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<int>(type: "int", nullable: true),
                    UpdatedBy = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountTypes", x => x.AccountTypeId);
                    table.CheckConstraint("CK_AccountTypes_Category", "[Category] IN ('Savings','Current','FixedDeposit','RecurringDeposit','Internal')");
                    table.CheckConstraint("CK_AccountTypes_InterestPayoutFrequency", "[InterestPayoutFrequency] IN ('Monthly','Quarterly','HalfYearly','Yearly','AtMaturity')");
                });

            migrationBuilder.CreateTable(
                name: "Addresses",
                columns: table => new
                {
                    AddressId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AddressType = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    AddressLine1 = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    AddressLine2 = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Landmark = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    City = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    District = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    State = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    PinCode = table.Column<string>(type: "nvarchar(6)", maxLength: 6, nullable: false),
                    IsPrimary = table.Column<bool>(type: "bit", nullable: false),
                    IsVerified = table.Column<bool>(type: "bit", nullable: false),
                    VerifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CustomerId = table.Column<int>(type: "int", nullable: false),
                    CountryId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<int>(type: "int", nullable: true),
                    UpdatedBy = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Addresses", x => x.AddressId);
                    table.CheckConstraint("CK_Addresses_AddressType", "[AddressType] IN ('Residential','Permanent','Correspondence','Office','Registered')");
                    table.CheckConstraint("CK_Addresses_PinCode", "[PinCode] LIKE '[0-9][0-9][0-9][0-9][0-9][0-9]'");
                });

            migrationBuilder.CreateTable(
                name: "AIChatMessages",
                columns: table => new
                {
                    AIChatMessageId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SequenceNumber = table.Column<int>(type: "int", nullable: false),
                    SenderRole = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ModelName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    InputTokens = table.Column<int>(type: "int", nullable: true),
                    OutputTokens = table.Column<int>(type: "int", nullable: true),
                    LatencyMs = table.Column<int>(type: "int", nullable: true),
                    FinishReason = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    IsFlagged = table.Column<bool>(type: "bit", nullable: false),
                    FlagReason = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    AIConversationId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AIChatMessages", x => x.AIChatMessageId);
                    table.CheckConstraint("CK_AIChatMessages_SenderRole", "[SenderRole] IN ('System','User','Assistant','Tool')");
                });

            migrationBuilder.CreateTable(
                name: "AIConversations",
                columns: table => new
                {
                    AIConversationId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ConversationReference = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Channel = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Language = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Summary = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    StartedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastMessageAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EndedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    MessageCount = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    CustomerId = table.Column<int>(type: "int", nullable: true),
                    EscalatedToEmployeeId = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<int>(type: "int", nullable: true),
                    UpdatedBy = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AIConversations", x => x.AIConversationId);
                    table.CheckConstraint("CK_AIConversations_Channel", "[Channel] IN ('Web','Mobile','Voice')");
                    table.CheckConstraint("CK_AIConversations_Status", "[Status] IN ('Active','Closed','Archived','Escalated')");
                });

            migrationBuilder.CreateTable(
                name: "AIUsageLogs",
                columns: table => new
                {
                    AIUsageLogId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FeatureName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Provider = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ModelName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    RequestId = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    IsSuccess = table.Column<bool>(type: "bit", nullable: false),
                    ErrorCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    LatencyMs = table.Column<int>(type: "int", nullable: false),
                    InputTokens = table.Column<int>(type: "int", nullable: false),
                    OutputTokens = table.Column<int>(type: "int", nullable: false),
                    TotalTokens = table.Column<int>(type: "int", nullable: false),
                    EstimatedCostInr = table.Column<decimal>(type: "decimal(18,6)", precision: 18, scale: 6, nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: true),
                    AIConversationId = table.Column<int>(type: "int", nullable: true),
                    AIChatMessageId = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AIUsageLogs", x => x.AIUsageLogId);
                    table.ForeignKey(
                        name: "FK_AIUsageLogs_AIChatMessages_AIChatMessageId",
                        column: x => x.AIChatMessageId,
                        principalTable: "AIChatMessages",
                        principalColumn: "AIChatMessageId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AIUsageLogs_AIConversations_AIConversationId",
                        column: x => x.AIConversationId,
                        principalTable: "AIConversations",
                        principalColumn: "AIConversationId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AuditLogs",
                columns: table => new
                {
                    AuditLogId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Action = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    EntityName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    EntityId = table.Column<int>(type: "int", nullable: true),
                    Outcome = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    IsSecuritySensitive = table.Column<bool>(type: "bit", nullable: false),
                    OldValuesMasked = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NewValuesMasked = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IpAddress = table.Column<string>(type: "nvarchar(45)", maxLength: 45, nullable: true),
                    UserAgent = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CorrelationId = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    PreviousHash = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    EntryHash = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    UserId = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditLogs", x => x.AuditLogId);
                    table.CheckConstraint("CK_AuditLogs_Outcome", "[Outcome] IN ('Success','Failure')");
                });

            migrationBuilder.CreateTable(
                name: "Beneficiaries",
                columns: table => new
                {
                    BeneficiaryId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BeneficiaryName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Nickname = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    BeneficiaryType = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    AccountNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    IfscCode = table.Column<string>(type: "nvarchar(11)", maxLength: 11, nullable: true),
                    BankName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    BranchName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    UpiVpa = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    MobileNumber = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    ActivatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CoolingPeriodEndsAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsFavorite = table.Column<bool>(type: "bit", nullable: false),
                    PerTransactionLimit = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    CustomerId = table.Column<int>(type: "int", nullable: false),
                    InternalAccountId = table.Column<int>(type: "int", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<int>(type: "int", nullable: true),
                    UpdatedBy = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Beneficiaries", x => x.BeneficiaryId);
                    table.CheckConstraint("CK_Beneficiaries_BeneficiaryType", "[BeneficiaryType] IN ('IntraBank','InterBank','UPI')");
                    table.CheckConstraint("CK_Beneficiaries_PayeeIdentifier", "([BeneficiaryType] = 'UPI' AND [UpiVpa] IS NOT NULL) OR ([BeneficiaryType] <> 'UPI' AND [AccountNumber] IS NOT NULL AND [IfscCode] IS NOT NULL)");
                    table.CheckConstraint("CK_Beneficiaries_Status", "[Status] IN ('Pending','Active','Blocked','Removed')");
                    table.ForeignKey(
                        name: "FK_Beneficiaries_Accounts_InternalAccountId",
                        column: x => x.InternalAccountId,
                        principalTable: "Accounts",
                        principalColumn: "AccountId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Branches",
                columns: table => new
                {
                    BranchId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BranchCode = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    BranchName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    IfscCode = table.Column<string>(type: "nvarchar(11)", maxLength: 11, nullable: false),
                    MicrCode = table.Column<string>(type: "nvarchar(9)", maxLength: 9, nullable: true),
                    BranchType = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    AddressLine1 = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    AddressLine2 = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    City = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    District = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    State = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    PinCode = table.Column<string>(type: "nvarchar(6)", maxLength: 6, nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    OpenedOn = table.Column<DateTime>(type: "date", nullable: false),
                    ClosedOn = table.Column<DateTime>(type: "date", nullable: true),
                    CountryId = table.Column<int>(type: "int", nullable: false),
                    ParentBranchId = table.Column<int>(type: "int", nullable: true),
                    ManagerEmployeeId = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<int>(type: "int", nullable: true),
                    UpdatedBy = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Branches", x => x.BranchId);
                    table.CheckConstraint("CK_Branches_BranchType", "[BranchType] IN ('Regular','Main','Digital','Service')");
                    table.CheckConstraint("CK_Branches_IfscCode", "[IfscCode] LIKE '[A-Z][A-Z][A-Z][A-Z]0[A-Z0-9][A-Z0-9][A-Z0-9][A-Z0-9][A-Z0-9][A-Z0-9]'");
                    table.CheckConstraint("CK_Branches_PinCode", "[PinCode] LIKE '[0-9][0-9][0-9][0-9][0-9][0-9]'");
                    table.ForeignKey(
                        name: "FK_Branches_Branches_ParentBranchId",
                        column: x => x.ParentBranchId,
                        principalTable: "Branches",
                        principalColumn: "BranchId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Cards",
                columns: table => new
                {
                    CardId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CardNumberToken = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    CardNumberMasked = table.Column<string>(type: "nvarchar(19)", maxLength: 19, nullable: false),
                    CardHolderName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CardType = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Network = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    ExpiryDate = table.Column<DateTime>(type: "date", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    IssuedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ActivatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    BlockedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    BlockReason = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    IsContactlessEnabled = table.Column<bool>(type: "bit", nullable: false),
                    IsOnlineEnabled = table.Column<bool>(type: "bit", nullable: false),
                    IsAtmEnabled = table.Column<bool>(type: "bit", nullable: false),
                    IsPosEnabled = table.Column<bool>(type: "bit", nullable: false),
                    DailyAtmLimit = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    DailyPosLimit = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    DailyOnlineLimit = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    PinLastSetAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PinRetryCount = table.Column<int>(type: "int", nullable: false),
                    PinBlockedUntil = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreditLimit = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    AvailableCredit = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    OutstandingAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    BillingCycleDay = table.Column<int>(type: "int", nullable: true),
                    AccountId = table.Column<int>(type: "int", nullable: false),
                    CustomerId = table.Column<int>(type: "int", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<int>(type: "int", nullable: true),
                    UpdatedBy = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cards", x => x.CardId);
                    table.CheckConstraint("CK_Cards_CardType", "[CardType] IN ('Debit','Credit','Prepaid')");
                    table.CheckConstraint("CK_Cards_Network", "[Network] IN ('RuPay','Visa','Mastercard')");
                    table.CheckConstraint("CK_Cards_Status", "[Status] IN ('Pending','Active','Blocked','Hotlisted','Expired','Cancelled')");
                    table.ForeignKey(
                        name: "FK_Cards_Accounts_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Accounts",
                        principalColumn: "AccountId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CardTransactions",
                columns: table => new
                {
                    CardTransactionId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AuthorizationCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    RetrievalReferenceNumber = table.Column<string>(type: "nvarchar(12)", maxLength: 12, nullable: true),
                    AuthorizationStatus = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    DeclineReason = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    AuthorizedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    MerchantName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    MerchantCategoryCode = table.Column<string>(type: "nvarchar(4)", maxLength: 4, nullable: false),
                    MerchantCity = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    TerminalId = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Channel = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    SettlementAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    SettledAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CardId = table.Column<int>(type: "int", nullable: false),
                    TransactionId = table.Column<int>(type: "int", nullable: true),
                    CurrencyId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<int>(type: "int", nullable: true),
                    UpdatedBy = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CardTransactions", x => x.CardTransactionId);
                    table.CheckConstraint("CK_CardTransactions_Amount", "[Amount] > 0");
                    table.CheckConstraint("CK_CardTransactions_AuthorizationStatus", "[AuthorizationStatus] IN ('Authorized','Declined','Reversed','Settled','Expired')");
                    table.CheckConstraint("CK_CardTransactions_Channel", "[Channel] IN ('POS','ATM','Ecommerce','Contactless','Recurring')");
                    table.ForeignKey(
                        name: "FK_CardTransactions_Cards_CardId",
                        column: x => x.CardId,
                        principalTable: "Cards",
                        principalColumn: "CardId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ComplianceRecords",
                columns: table => new
                {
                    ComplianceRecordId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReferenceNumber = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    RecordType = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    RiskLevel = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    ReportingAuthority = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    FilingReference = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    DueDate = table.Column<DateTime>(type: "date", nullable: true),
                    FiledAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CustomerId = table.Column<int>(type: "int", nullable: true),
                    AccountId = table.Column<int>(type: "int", nullable: true),
                    TransactionId = table.Column<int>(type: "int", nullable: true),
                    FraudAlertId = table.Column<int>(type: "int", nullable: true),
                    FiledByEmployeeId = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<int>(type: "int", nullable: true),
                    UpdatedBy = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ComplianceRecords", x => x.ComplianceRecordId);
                    table.CheckConstraint("CK_ComplianceRecords_RecordType", "[RecordType] IN ('Str','Ctr','Ccr','Ntr','AmlScreening','PepCheck','SanctionsScreening','PeriodicKycReview','FatcaCrs')");
                    table.CheckConstraint("CK_ComplianceRecords_RiskLevel", "[RiskLevel] IN ('Low','Medium','High','Critical')");
                    table.CheckConstraint("CK_ComplianceRecords_Status", "[Status] IN ('Open','UnderReview','Filed','Cleared','Closed')");
                    table.ForeignKey(
                        name: "FK_ComplianceRecords_Accounts_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Accounts",
                        principalColumn: "AccountId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Countries",
                columns: table => new
                {
                    CountryId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IsoAlpha2Code = table.Column<string>(type: "nvarchar(2)", maxLength: 2, nullable: false),
                    IsoAlpha3Code = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    NumericCode = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    PhoneCode = table.Column<string>(type: "nvarchar(6)", maxLength: 6, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<int>(type: "int", nullable: true),
                    UpdatedBy = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Countries", x => x.CountryId);
                });

            migrationBuilder.CreateTable(
                name: "Currencies",
                columns: table => new
                {
                    CurrencyId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CurrencyCode = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    NumericCode = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Symbol = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: false),
                    DecimalPlaces = table.Column<int>(type: "int", nullable: false),
                    IsBaseCurrency = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CountryId = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<int>(type: "int", nullable: true),
                    UpdatedBy = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Currencies", x => x.CurrencyId);
                    table.ForeignKey(
                        name: "FK_Currencies_Countries_CountryId",
                        column: x => x.CountryId,
                        principalTable: "Countries",
                        principalColumn: "CountryId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Customers",
                columns: table => new
                {
                    CustomerId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CustomerNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    CustomerType = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    MiddleName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    LastName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DateOfBirth = table.Column<DateTime>(type: "date", nullable: false),
                    Gender = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    FatherOrSpouseName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    MobileNumber = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    AlternateMobileNumber = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Occupation = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    AnnualIncome = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    PreferredLanguage = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    RiskCategory = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    OnboardedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ClosedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UserId = table.Column<int>(type: "int", nullable: true),
                    HomeBranchId = table.Column<int>(type: "int", nullable: false),
                    RelationshipManagerId = table.Column<int>(type: "int", nullable: true),
                    NationalityCountryId = table.Column<int>(type: "int", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<int>(type: "int", nullable: true),
                    UpdatedBy = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Customers", x => x.CustomerId);
                    table.CheckConstraint("CK_Customers_CustomerType", "[CustomerType] IN ('Individual','Minor','Business')");
                    table.CheckConstraint("CK_Customers_Gender", "[Gender] IN ('Male','Female','Other')");
                    table.CheckConstraint("CK_Customers_RiskCategory", "[RiskCategory] IN ('Low','Medium','High')");
                    table.CheckConstraint("CK_Customers_Status", "[Status] IN ('Active','Inactive','Dormant','Blocked','Closed')");
                    table.ForeignKey(
                        name: "FK_Customers_Branches_HomeBranchId",
                        column: x => x.HomeBranchId,
                        principalTable: "Branches",
                        principalColumn: "BranchId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Customers_Countries_NationalityCountryId",
                        column: x => x.NationalityCountryId,
                        principalTable: "Countries",
                        principalColumn: "CountryId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Disputes",
                columns: table => new
                {
                    DisputeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DisputeNumber = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    DisputeType = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    DisputedAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Priority = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    RaisedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DueBy = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ResolvedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Resolution = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    ResolutionNotes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    TransactionId = table.Column<int>(type: "int", nullable: false),
                    CardTransactionId = table.Column<int>(type: "int", nullable: true),
                    CustomerId = table.Column<int>(type: "int", nullable: false),
                    AssignedToEmployeeId = table.Column<int>(type: "int", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<int>(type: "int", nullable: true),
                    UpdatedBy = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Disputes", x => x.DisputeId);
                    table.CheckConstraint("CK_Disputes_DisputedAmount", "[DisputedAmount] > 0");
                    table.CheckConstraint("CK_Disputes_DisputeType", "[DisputeType] IN ('UnauthorizedTransaction','DuplicateCharge','WrongAmount','ServiceNotReceived','AtmCashNotDispensed','FailedTransactionDebited','Other')");
                    table.CheckConstraint("CK_Disputes_Priority", "[Priority] IN ('Low','Medium','High','Critical')");
                    table.CheckConstraint("CK_Disputes_Resolution", "[Resolution] IN ('InFavourOfCustomer','InFavourOfBank','Withdrawn')");
                    table.CheckConstraint("CK_Disputes_Status", "[Status] IN ('Open','UnderInvestigation','ProvisionalCreditGiven','Resolved','Rejected','Escalated','Closed')");
                    table.ForeignKey(
                        name: "FK_Disputes_CardTransactions_CardTransactionId",
                        column: x => x.CardTransactionId,
                        principalTable: "CardTransactions",
                        principalColumn: "CardTransactionId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Disputes_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "CustomerId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Documents",
                columns: table => new
                {
                    DocumentId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DocumentType = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    DocumentNumberMasked = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    IssueDate = table.Column<DateTime>(type: "date", nullable: true),
                    ExpiryDate = table.Column<DateTime>(type: "date", nullable: true),
                    FileName = table.Column<string>(type: "nvarchar(260)", maxLength: 260, nullable: false),
                    ContentType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    FileSizeBytes = table.Column<long>(type: "bigint", nullable: false),
                    StoragePath = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    ContentHash = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    IsEncrypted = table.Column<bool>(type: "bit", nullable: false),
                    VerificationStatus = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    VerifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RejectionReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CustomerId = table.Column<int>(type: "int", nullable: false),
                    KYCDetailsId = table.Column<int>(type: "int", nullable: true),
                    LoanId = table.Column<int>(type: "int", nullable: true),
                    VerifiedByEmployeeId = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<int>(type: "int", nullable: true),
                    UpdatedBy = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Documents", x => x.DocumentId);
                    table.CheckConstraint("CK_Documents_DocumentType", "[DocumentType] IN ('Pan','Aadhaar','Passport','VoterId','DrivingLicence','Photo','Signature','AddressProof','IncomeProof','Form60','LoanAgreement','Other')");
                    table.CheckConstraint("CK_Documents_VerificationStatus", "[VerificationStatus] IN ('Pending','Verified','Rejected','Expired')");
                    table.ForeignKey(
                        name: "FK_Documents_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "CustomerId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Employees",
                columns: table => new
                {
                    EmployeeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmployeeCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Designation = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Department = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: true),
                    DateOfJoining = table.Column<DateTime>(type: "date", nullable: false),
                    DateOfLeaving = table.Column<DateTime>(type: "date", nullable: true),
                    EmploymentStatus = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    ApprovalLimit = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    BranchId = table.Column<int>(type: "int", nullable: false),
                    ReportsToEmployeeId = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<int>(type: "int", nullable: true),
                    UpdatedBy = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Employees", x => x.EmployeeId);
                    table.CheckConstraint("CK_Employees_EmploymentStatus", "[EmploymentStatus] IN ('Active','OnLeave','Suspended','Resigned','Terminated')");
                    table.ForeignKey(
                        name: "FK_Employees_Branches_BranchId",
                        column: x => x.BranchId,
                        principalTable: "Branches",
                        principalColumn: "BranchId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Employees_Employees_ReportsToEmployeeId",
                        column: x => x.ReportsToEmployeeId,
                        principalTable: "Employees",
                        principalColumn: "EmployeeId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Fees",
                columns: table => new
                {
                    FeeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CalculationMethod = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    Frequency = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    FlatAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    Percentage = table.Column<decimal>(type: "decimal(9,4)", precision: 9, scale: 4, nullable: true),
                    MinimumAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    MaximumAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    IsGstApplicable = table.Column<bool>(type: "bit", nullable: false),
                    GstRate = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    EffectiveFrom = table.Column<DateTime>(type: "date", nullable: false),
                    EffectiveTo = table.Column<DateTime>(type: "date", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    TransactionTypeId = table.Column<int>(type: "int", nullable: true),
                    PaymentMethodId = table.Column<int>(type: "int", nullable: true),
                    AccountTypeId = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<int>(type: "int", nullable: true),
                    UpdatedBy = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Fees", x => x.FeeId);
                    table.CheckConstraint("CK_Fees_CalculationMethod", "[CalculationMethod] IN ('Flat','Percentage','Slab')");
                    table.CheckConstraint("CK_Fees_Frequency", "[Frequency] IN ('PerTransaction','Monthly','Quarterly','Yearly','OneTime')");
                    table.ForeignKey(
                        name: "FK_Fees_AccountTypes_AccountTypeId",
                        column: x => x.AccountTypeId,
                        principalTable: "AccountTypes",
                        principalColumn: "AccountTypeId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "FraudAlerts",
                columns: table => new
                {
                    FraudAlertId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AlertNumber = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Severity = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Score = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    RaisedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ReviewedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReviewNotes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    ActionTaken = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    FraudRuleId = table.Column<int>(type: "int", nullable: false),
                    TransactionId = table.Column<int>(type: "int", nullable: true),
                    CustomerId = table.Column<int>(type: "int", nullable: true),
                    AccountId = table.Column<int>(type: "int", nullable: true),
                    CardId = table.Column<int>(type: "int", nullable: true),
                    RiskAssessmentId = table.Column<int>(type: "int", nullable: true),
                    ReviewedByEmployeeId = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<int>(type: "int", nullable: true),
                    UpdatedBy = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FraudAlerts", x => x.FraudAlertId);
                    table.CheckConstraint("CK_FraudAlerts_Severity", "[Severity] IN ('Low','Medium','High','Critical')");
                    table.CheckConstraint("CK_FraudAlerts_Status", "[Status] IN ('Open','UnderReview','ConfirmedFraud','FalsePositive','Closed')");
                    table.CheckConstraint("CK_FraudAlerts_Subject", "[TransactionId] IS NOT NULL OR [CustomerId] IS NOT NULL OR [AccountId] IS NOT NULL OR [CardId] IS NOT NULL");
                    table.ForeignKey(
                        name: "FK_FraudAlerts_Accounts_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Accounts",
                        principalColumn: "AccountId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FraudAlerts_Cards_CardId",
                        column: x => x.CardId,
                        principalTable: "Cards",
                        principalColumn: "CardId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FraudAlerts_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "CustomerId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FraudAlerts_Employees_ReviewedByEmployeeId",
                        column: x => x.ReviewedByEmployeeId,
                        principalTable: "Employees",
                        principalColumn: "EmployeeId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "FraudRules",
                columns: table => new
                {
                    FraudRuleId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RuleCode = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    RuleType = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Severity = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    RuleDefinitionJson = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ThresholdAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    TimeWindowMinutes = table.Column<int>(type: "int", nullable: true),
                    ActionOnMatch = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Priority = table.Column<int>(type: "int", nullable: false),
                    Version = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    EffectiveFrom = table.Column<DateTime>(type: "date", nullable: true),
                    EffectiveTo = table.Column<DateTime>(type: "date", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<int>(type: "int", nullable: true),
                    UpdatedBy = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FraudRules", x => x.FraudRuleId);
                    table.CheckConstraint("CK_FraudRules_ActionOnMatch", "[ActionOnMatch] IN ('Alert','Block','StepUpAuth','Review')");
                    table.CheckConstraint("CK_FraudRules_RuleType", "[RuleType] IN ('Velocity','Amount','Geo','Device','Behavioural','Blacklist','Pattern')");
                    table.CheckConstraint("CK_FraudRules_Severity", "[Severity] IN ('Low','Medium','High','Critical')");
                });

            migrationBuilder.CreateTable(
                name: "InterestCalculations",
                columns: table => new
                {
                    InterestCalculationId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CalculationType = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    PeriodStart = table.Column<DateTime>(type: "date", nullable: false),
                    PeriodEnd = table.Column<DateTime>(type: "date", nullable: false),
                    DayCount = table.Column<int>(type: "int", nullable: false),
                    PrincipalBase = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    AppliedRate = table.Column<decimal>(type: "decimal(9,4)", precision: 9, scale: 4, nullable: false),
                    InterestAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    TdsAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    CalculatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AccountId = table.Column<int>(type: "int", nullable: true),
                    LoanId = table.Column<int>(type: "int", nullable: true),
                    InterestRateId = table.Column<int>(type: "int", nullable: false),
                    JournalEntryId = table.Column<int>(type: "int", nullable: true),
                    TransactionId = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<int>(type: "int", nullable: true),
                    UpdatedBy = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InterestCalculations", x => x.InterestCalculationId);
                    table.CheckConstraint("CK_InterestCalculations_CalculationType", "[CalculationType] IN ('Accrual','Payout','Capitalisation')");
                    table.CheckConstraint("CK_InterestCalculations_Period", "[PeriodEnd] >= [PeriodStart]");
                    table.CheckConstraint("CK_InterestCalculations_Status", "[Status] IN ('Calculated','Posted','Reversed')");
                    table.CheckConstraint("CK_InterestCalculations_Subject", "([AccountId] IS NOT NULL AND [LoanId] IS NULL) OR ([AccountId] IS NULL AND [LoanId] IS NOT NULL)");
                    table.ForeignKey(
                        name: "FK_InterestCalculations_Accounts_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Accounts",
                        principalColumn: "AccountId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "InterestRates",
                columns: table => new
                {
                    InterestRateId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RateName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ProductCategory = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    LoanType = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    AnnualRate = table.Column<decimal>(type: "decimal(9,4)", precision: 9, scale: 4, nullable: false),
                    SeniorCitizenAdditionalRate = table.Column<decimal>(type: "decimal(9,4)", precision: 9, scale: 4, nullable: true),
                    CompoundingFrequency = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    MinAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    MaxAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    MinTenureMonths = table.Column<int>(type: "int", nullable: true),
                    MaxTenureMonths = table.Column<int>(type: "int", nullable: true),
                    BenchmarkName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    SpreadOverBenchmark = table.Column<decimal>(type: "decimal(9,4)", precision: 9, scale: 4, nullable: true),
                    EffectiveFrom = table.Column<DateTime>(type: "date", nullable: false),
                    EffectiveTo = table.Column<DateTime>(type: "date", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    AccountTypeId = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<int>(type: "int", nullable: true),
                    UpdatedBy = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InterestRates", x => x.InterestRateId);
                    table.CheckConstraint("CK_InterestRates_AnnualRate", "[AnnualRate] >= 0");
                    table.CheckConstraint("CK_InterestRates_CompoundingFrequency", "[CompoundingFrequency] IN ('Simple','Daily','Monthly','Quarterly','HalfYearly','Yearly')");
                    table.CheckConstraint("CK_InterestRates_ProductCategory", "[ProductCategory] IN ('Savings','Current','FixedDeposit','RecurringDeposit','Loan')");
                    table.ForeignKey(
                        name: "FK_InterestRates_AccountTypes_AccountTypeId",
                        column: x => x.AccountTypeId,
                        principalTable: "AccountTypes",
                        principalColumn: "AccountTypeId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "JournalEntries",
                columns: table => new
                {
                    JournalEntryId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    JournalNumber = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    JournalType = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    EntryDate = table.Column<DateTime>(type: "date", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    TotalDebit = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    TotalCredit = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    PostedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CurrencyId = table.Column<int>(type: "int", nullable: false),
                    TransactionId = table.Column<int>(type: "int", nullable: true),
                    PostedByUserId = table.Column<int>(type: "int", nullable: true),
                    ReversalOfJournalEntryId = table.Column<int>(type: "int", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<int>(type: "int", nullable: true),
                    UpdatedBy = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JournalEntries", x => x.JournalEntryId);
                    table.CheckConstraint("CK_JournalEntries_Balanced", "[Status] <> 'Posted' OR ([TotalDebit] = [TotalCredit] AND [TotalDebit] > 0)");
                    table.CheckConstraint("CK_JournalEntries_JournalType", "[JournalType] IN ('Transaction','Interest','Fee','Loan','Adjustment','Reversal')");
                    table.CheckConstraint("CK_JournalEntries_Status", "[Status] IN ('Draft','Posted','Reversed')");
                    table.ForeignKey(
                        name: "FK_JournalEntries_Currencies_CurrencyId",
                        column: x => x.CurrencyId,
                        principalTable: "Currencies",
                        principalColumn: "CurrencyId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_JournalEntries_JournalEntries_ReversalOfJournalEntryId",
                        column: x => x.ReversalOfJournalEntryId,
                        principalTable: "JournalEntries",
                        principalColumn: "JournalEntryId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "JournalEntryLines",
                columns: table => new
                {
                    JournalEntryLineId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LineNumber = table.Column<int>(type: "int", nullable: false),
                    DebitAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    CreditAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Narration = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    JournalEntryId = table.Column<int>(type: "int", nullable: false),
                    AccountId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    CreatedBy = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JournalEntryLines", x => x.JournalEntryLineId);
                    table.CheckConstraint("CK_JournalEntryLines_OneSided", "([DebitAmount] > 0 AND [CreditAmount] = 0) OR ([CreditAmount] > 0 AND [DebitAmount] = 0)");
                    table.ForeignKey(
                        name: "FK_JournalEntryLines_Accounts_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Accounts",
                        principalColumn: "AccountId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_JournalEntryLines_JournalEntries_JournalEntryId",
                        column: x => x.JournalEntryId,
                        principalTable: "JournalEntries",
                        principalColumn: "JournalEntryId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "KYCDetails",
                columns: table => new
                {
                    KYCDetailsId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    KycLevel = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    VerificationStatus = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    IsCurrent = table.Column<bool>(type: "bit", nullable: false),
                    VerifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RejectionReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    NextReKycDueDate = table.Column<DateTime>(type: "date", nullable: true),
                    PanEncrypted = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
                    PanHash = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    PanMasked = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    IsPanVerified = table.Column<bool>(type: "bit", nullable: false),
                    PanVerifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsForm60Submitted = table.Column<bool>(type: "bit", nullable: false),
                    AadhaarVaultReference = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    AadhaarLast4 = table.Column<string>(type: "nvarchar(4)", maxLength: 4, nullable: true),
                    AadhaarVerificationMethod = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    IsAadhaarVerified = table.Column<bool>(type: "bit", nullable: false),
                    AadhaarVerifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CkycNumber = table.Column<string>(type: "nvarchar(14)", maxLength: 14, nullable: true),
                    CkycStatus = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    CkycVerifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CustomerId = table.Column<int>(type: "int", nullable: false),
                    VerifiedByEmployeeId = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<int>(type: "int", nullable: true),
                    UpdatedBy = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KYCDetails", x => x.KYCDetailsId);
                    table.CheckConstraint("CK_KYCDetails_AadhaarVerificationMethod", "[AadhaarVerificationMethod] IN ('OtpEkyc','BiometricEkyc','OfflineXml','VirtualId')");
                    table.CheckConstraint("CK_KYCDetails_CkycStatus", "[CkycStatus] IN ('NotInitiated','Pending','Uploaded','Verified','Rejected')");
                    table.CheckConstraint("CK_KYCDetails_KycLevel", "[KycLevel] IN ('Minimum','Full','Video','EKyc')");
                    table.CheckConstraint("CK_KYCDetails_VerificationStatus", "[VerificationStatus] IN ('Pending','InProgress','Verified','Rejected','Expired')");
                    table.ForeignKey(
                        name: "FK_KYCDetails_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "CustomerId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_KYCDetails_Employees_VerifiedByEmployeeId",
                        column: x => x.VerifiedByEmployeeId,
                        principalTable: "Employees",
                        principalColumn: "EmployeeId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "LoanRepayments",
                columns: table => new
                {
                    LoanRepaymentId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RepaymentReference = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    RepaymentType = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    PaymentMode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    PaidAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AmountPaid = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    PrincipalComponent = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    InterestComponent = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    PenaltyComponent = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    FailureReason = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    LoanId = table.Column<int>(type: "int", nullable: false),
                    LoanScheduleId = table.Column<int>(type: "int", nullable: true),
                    TransactionId = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<int>(type: "int", nullable: true),
                    UpdatedBy = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LoanRepayments", x => x.LoanRepaymentId);
                    table.CheckConstraint("CK_LoanRepayments_AmountPaid", "[AmountPaid] > 0");
                    table.CheckConstraint("CK_LoanRepayments_Components", "[AmountPaid] = [PrincipalComponent] + [InterestComponent] + [PenaltyComponent]");
                    table.CheckConstraint("CK_LoanRepayments_PaymentMode", "[PaymentMode] IN ('AutoDebit','Nach','Online','Branch')");
                    table.CheckConstraint("CK_LoanRepayments_RepaymentType", "[RepaymentType] IN ('Emi','PartPayment','ForeClosure','Penalty')");
                    table.CheckConstraint("CK_LoanRepayments_Status", "[Status] IN ('Pending','Successful','Failed','Reversed')");
                });

            migrationBuilder.CreateTable(
                name: "Loans",
                columns: table => new
                {
                    LoanId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LoanNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    LoanType = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Purpose = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    RequestedAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    SanctionedAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    DisbursedAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    OutstandingPrincipal = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    OutstandingInterest = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    EmiAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    AnnualInterestRate = table.Column<decimal>(type: "decimal(9,4)", precision: 9, scale: 4, nullable: false),
                    InterestType = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    TenureMonths = table.Column<int>(type: "int", nullable: false),
                    ApplicationDate = table.Column<DateTime>(type: "date", nullable: false),
                    SanctionDate = table.Column<DateTime>(type: "date", nullable: true),
                    DisbursementDate = table.Column<DateTime>(type: "date", nullable: true),
                    FirstEmiDate = table.Column<DateTime>(type: "date", nullable: true),
                    MaturityDate = table.Column<DateTime>(type: "date", nullable: true),
                    ClosedOn = table.Column<DateTime>(type: "date", nullable: true),
                    DaysPastDue = table.Column<int>(type: "int", nullable: false),
                    AssetClassification = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    CustomerId = table.Column<int>(type: "int", nullable: false),
                    BranchId = table.Column<int>(type: "int", nullable: false),
                    DisbursementAccountId = table.Column<int>(type: "int", nullable: true),
                    RepaymentAccountId = table.Column<int>(type: "int", nullable: true),
                    InterestRateId = table.Column<int>(type: "int", nullable: true),
                    ApprovedByEmployeeId = table.Column<int>(type: "int", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<int>(type: "int", nullable: true),
                    UpdatedBy = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Loans", x => x.LoanId);
                    table.CheckConstraint("CK_Loans_Amounts", "[RequestedAmount] > 0 AND [DisbursedAmount] >= 0 AND [OutstandingPrincipal] >= 0");
                    table.CheckConstraint("CK_Loans_AssetClassification", "[AssetClassification] IN ('Standard','SMA0','SMA1','SMA2','SubStandard','Doubtful','Loss')");
                    table.CheckConstraint("CK_Loans_InterestType", "[InterestType] IN ('Fixed','Floating')");
                    table.CheckConstraint("CK_Loans_LoanType", "[LoanType] IN ('Personal','Home','Vehicle','Education','Business','Gold','AgainstDeposit')");
                    table.CheckConstraint("CK_Loans_Status", "[Status] IN ('Applied','Approved','Rejected','Disbursed','Active','Closed','WrittenOff')");
                    table.ForeignKey(
                        name: "FK_Loans_Accounts_DisbursementAccountId",
                        column: x => x.DisbursementAccountId,
                        principalTable: "Accounts",
                        principalColumn: "AccountId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Loans_Accounts_RepaymentAccountId",
                        column: x => x.RepaymentAccountId,
                        principalTable: "Accounts",
                        principalColumn: "AccountId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Loans_Branches_BranchId",
                        column: x => x.BranchId,
                        principalTable: "Branches",
                        principalColumn: "BranchId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Loans_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "CustomerId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Loans_Employees_ApprovedByEmployeeId",
                        column: x => x.ApprovedByEmployeeId,
                        principalTable: "Employees",
                        principalColumn: "EmployeeId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Loans_InterestRates_InterestRateId",
                        column: x => x.InterestRateId,
                        principalTable: "InterestRates",
                        principalColumn: "InterestRateId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "LoanSchedules",
                columns: table => new
                {
                    LoanScheduleId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InstallmentNumber = table.Column<int>(type: "int", nullable: false),
                    DueDate = table.Column<DateTime>(type: "date", nullable: false),
                    OpeningBalance = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    ClosingBalance = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    PrincipalDue = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    InterestDue = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    PenaltyAccrued = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    PrincipalPaid = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    InterestPaid = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    PenaltyPaid = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    PaidOn = table.Column<DateTime>(type: "date", nullable: true),
                    LoanId = table.Column<int>(type: "int", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<int>(type: "int", nullable: true),
                    UpdatedBy = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LoanSchedules", x => x.LoanScheduleId);
                    table.CheckConstraint("CK_LoanSchedules_Amounts", "[PrincipalDue] >= 0 AND [InterestDue] >= 0 AND [PrincipalPaid] >= 0 AND [InterestPaid] >= 0");
                    table.CheckConstraint("CK_LoanSchedules_Status", "[Status] IN ('Pending','PartiallyPaid','Paid','Overdue','Waived')");
                    table.ForeignKey(
                        name: "FK_LoanSchedules_Loans_LoanId",
                        column: x => x.LoanId,
                        principalTable: "Loans",
                        principalColumn: "LoanId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Notifications",
                columns: table => new
                {
                    NotificationId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Channel = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    Priority = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    RecipientAddress = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Subject = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Body = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    ScheduledAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    SentAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeliveredAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReadAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RetryCount = table.Column<int>(type: "int", nullable: false),
                    ProviderMessageId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    FailureReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    RelatedEntityType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    RelatedEntityId = table.Column<int>(type: "int", nullable: true),
                    CustomerId = table.Column<int>(type: "int", nullable: true),
                    UserId = table.Column<int>(type: "int", nullable: true),
                    NotificationTemplateId = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<int>(type: "int", nullable: true),
                    UpdatedBy = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notifications", x => x.NotificationId);
                    table.CheckConstraint("CK_Notifications_Channel", "[Channel] IN ('Sms','Email','Push','InApp','WhatsApp')");
                    table.CheckConstraint("CK_Notifications_Priority", "[Priority] IN ('Low','Normal','High','Urgent')");
                    table.CheckConstraint("CK_Notifications_Recipient", "[CustomerId] IS NOT NULL OR [UserId] IS NOT NULL");
                    table.CheckConstraint("CK_Notifications_Status", "[Status] IN ('Pending','Queued','Sent','Delivered','Failed','Read')");
                    table.ForeignKey(
                        name: "FK_Notifications_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "CustomerId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "NotificationTemplates",
                columns: table => new
                {
                    NotificationTemplateId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TemplateCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Channel = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    Language = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Category = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Subject = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    BodyTemplate = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DltTemplateId = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    Version = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<int>(type: "int", nullable: true),
                    UpdatedBy = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NotificationTemplates", x => x.NotificationTemplateId);
                    table.CheckConstraint("CK_NotificationTemplates_Category", "[Category] IN ('Transactional','Security','Promotional','Regulatory','Reminder')");
                    table.CheckConstraint("CK_NotificationTemplates_Channel", "[Channel] IN ('Sms','Email','Push','InApp','WhatsApp')");
                });

            migrationBuilder.CreateTable(
                name: "PaymentGatewayLogs",
                columns: table => new
                {
                    PaymentGatewayLogId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GatewayName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Operation = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CorrelationId = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    AttemptNumber = table.Column<int>(type: "int", nullable: false),
                    RequestPayload = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ResponsePayload = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsSuccess = table.Column<bool>(type: "bit", nullable: false),
                    HttpStatusCode = table.Column<int>(type: "int", nullable: true),
                    ResponseCode = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    ResponseMessage = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    DurationMs = table.Column<int>(type: "int", nullable: true),
                    ErrorMessage = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    PaymentRequestId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentGatewayLogs", x => x.PaymentGatewayLogId);
                    table.CheckConstraint("CK_PaymentGatewayLogs_Operation", "[Operation] IN ('Initiate','StatusCheck','Callback','Reversal','Refund')");
                });

            migrationBuilder.CreateTable(
                name: "PaymentMethods",
                columns: table => new
                {
                    PaymentMethodId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Rail = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    MinAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    MaxAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    DailyLimit = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    Is24x7 = table.Column<bool>(type: "bit", nullable: false),
                    SettlementType = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<int>(type: "int", nullable: true),
                    UpdatedBy = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentMethods", x => x.PaymentMethodId);
                    table.CheckConstraint("CK_PaymentMethods_Rail", "[Rail] IN ('UPI','NEFT','RTGS','IMPS','NACH','Cheque','Card','Internal')");
                    table.CheckConstraint("CK_PaymentMethods_SettlementType", "[SettlementType] IN ('RealTime','DeferredNet','Batch')");
                });

            migrationBuilder.CreateTable(
                name: "PaymentRequests",
                columns: table => new
                {
                    PaymentRequestId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RequestReference = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    IdempotencyKey = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    UtrNumber = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    PayeeName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    PayeeAccountNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    PayeeIfsc = table.Column<string>(type: "nvarchar(11)", maxLength: 11, nullable: true),
                    PayeeVpa = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Purpose = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    Remarks = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    Channel = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    FailureCode = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    FailureReason = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    RequestedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AuthorizedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ExpiresAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ScheduledFor = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PaymentMethodId = table.Column<int>(type: "int", nullable: false),
                    CustomerId = table.Column<int>(type: "int", nullable: false),
                    SourceAccountId = table.Column<int>(type: "int", nullable: false),
                    BeneficiaryId = table.Column<int>(type: "int", nullable: true),
                    CurrencyId = table.Column<int>(type: "int", nullable: false),
                    TransactionId = table.Column<int>(type: "int", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<int>(type: "int", nullable: true),
                    UpdatedBy = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentRequests", x => x.PaymentRequestId);
                    table.CheckConstraint("CK_PaymentRequests_Amount", "[Amount] > 0");
                    table.CheckConstraint("CK_PaymentRequests_Channel", "[Channel] IN ('Branch','NetBanking','MobileApp','UPI','Api')");
                    table.CheckConstraint("CK_PaymentRequests_Payee", "[BeneficiaryId] IS NOT NULL OR [PayeeAccountNumber] IS NOT NULL OR [PayeeVpa] IS NOT NULL");
                    table.CheckConstraint("CK_PaymentRequests_Status", "[Status] IN ('Created','Authorized','Submitted','Processing','Completed','Failed','Rejected','Expired','Cancelled')");
                    table.ForeignKey(
                        name: "FK_PaymentRequests_Accounts_SourceAccountId",
                        column: x => x.SourceAccountId,
                        principalTable: "Accounts",
                        principalColumn: "AccountId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PaymentRequests_Beneficiaries_BeneficiaryId",
                        column: x => x.BeneficiaryId,
                        principalTable: "Beneficiaries",
                        principalColumn: "BeneficiaryId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PaymentRequests_Currencies_CurrencyId",
                        column: x => x.CurrencyId,
                        principalTable: "Currencies",
                        principalColumn: "CurrencyId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PaymentRequests_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "CustomerId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PaymentRequests_PaymentMethods_PaymentMethodId",
                        column: x => x.PaymentMethodId,
                        principalTable: "PaymentMethods",
                        principalColumn: "PaymentMethodId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Permissions",
                columns: table => new
                {
                    PermissionId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ModuleName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<int>(type: "int", nullable: true),
                    UpdatedBy = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Permissions", x => x.PermissionId);
                });

            migrationBuilder.CreateTable(
                name: "ReconciliationBatches",
                columns: table => new
                {
                    ReconciliationBatchId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BatchNumber = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    ReconciliationType = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    BusinessDate = table.Column<DateTime>(type: "date", nullable: false),
                    SourceName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    FileName = table.Column<string>(type: "nvarchar(260)", maxLength: 260, nullable: true),
                    FileHash = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    TotalRecords = table.Column<int>(type: "int", nullable: false),
                    MatchedRecords = table.Column<int>(type: "int", nullable: false),
                    UnmatchedRecords = table.Column<int>(type: "int", nullable: false),
                    TotalAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    MatchedAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    DifferenceAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    StartedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PaymentMethodId = table.Column<int>(type: "int", nullable: true),
                    InitiatedByUserId = table.Column<int>(type: "int", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<int>(type: "int", nullable: true),
                    UpdatedBy = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReconciliationBatches", x => x.ReconciliationBatchId);
                    table.CheckConstraint("CK_ReconciliationBatches_ReconciliationType", "[ReconciliationType] IN ('Upi','Neft','Rtgs','Imps','CardSettlement','AtmSwitch','GeneralLedger')");
                    table.CheckConstraint("CK_ReconciliationBatches_Status", "[Status] IN ('Pending','InProgress','Completed','CompletedWithExceptions','Failed')");
                    table.ForeignKey(
                        name: "FK_ReconciliationBatches_PaymentMethods_PaymentMethodId",
                        column: x => x.PaymentMethodId,
                        principalTable: "PaymentMethods",
                        principalColumn: "PaymentMethodId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ReconciliationRecords",
                columns: table => new
                {
                    ReconciliationRecordId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ExternalReference = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ExternalAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    ExternalDate = table.Column<DateTime>(type: "date", nullable: false),
                    InternalAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    DifferenceAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    MatchStatus = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    ExceptionReason = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    ResolvedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ResolutionNotes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ReconciliationBatchId = table.Column<int>(type: "int", nullable: false),
                    TransactionId = table.Column<int>(type: "int", nullable: true),
                    ResolvedByUserId = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<int>(type: "int", nullable: true),
                    UpdatedBy = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReconciliationRecords", x => x.ReconciliationRecordId);
                    table.CheckConstraint("CK_ReconciliationRecords_MatchStatus", "[MatchStatus] IN ('Matched','Unmatched','AmountMismatch','MissingInBank','MissingInExternal','Manual')");
                    table.ForeignKey(
                        name: "FK_ReconciliationRecords_ReconciliationBatches_ReconciliationBatchId",
                        column: x => x.ReconciliationBatchId,
                        principalTable: "ReconciliationBatches",
                        principalColumn: "ReconciliationBatchId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RefreshTokens",
                columns: table => new
                {
                    RefreshTokenId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TokenHash = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    JwtId = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RevokedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RevocationReason = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    RevokedByIp = table.Column<string>(type: "nvarchar(45)", maxLength: 45, nullable: true),
                    CreatedByIp = table.Column<string>(type: "nvarchar(45)", maxLength: 45, nullable: false),
                    UserAgent = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
                    DeviceName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    ReplacedByTokenId = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RefreshTokens", x => x.RefreshTokenId);
                    table.ForeignKey(
                        name: "FK_RefreshTokens_RefreshTokens_ReplacedByTokenId",
                        column: x => x.ReplacedByTokenId,
                        principalTable: "RefreshTokens",
                        principalColumn: "RefreshTokenId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Refunds",
                columns: table => new
                {
                    RefundId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RefundReference = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    RefundType = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    RequestedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ProcessedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RejectionReason = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    OriginalTransactionId = table.Column<int>(type: "int", nullable: false),
                    RefundTransactionId = table.Column<int>(type: "int", nullable: true),
                    DisputeId = table.Column<int>(type: "int", nullable: true),
                    RequestedByUserId = table.Column<int>(type: "int", nullable: true),
                    ApprovedByUserId = table.Column<int>(type: "int", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<int>(type: "int", nullable: true),
                    UpdatedBy = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Refunds", x => x.RefundId);
                    table.CheckConstraint("CK_Refunds_Amount", "[Amount] > 0");
                    table.CheckConstraint("CK_Refunds_RefundType", "[RefundType] IN ('Full','Partial')");
                    table.CheckConstraint("CK_Refunds_Status", "[Status] IN ('Requested','Approved','Rejected','Processed','Failed')");
                    table.ForeignKey(
                        name: "FK_Refunds_Disputes_DisputeId",
                        column: x => x.DisputeId,
                        principalTable: "Disputes",
                        principalColumn: "DisputeId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ReportConfigurations",
                columns: table => new
                {
                    ReportConfigurationId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReportCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ReportType = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    OutputFormat = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    IsRegulatory = table.Column<bool>(type: "bit", nullable: false),
                    QueryDefinition = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ParametersJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsScheduled = table.Column<bool>(type: "bit", nullable: false),
                    ScheduleCron = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    LastRunAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    NextRunAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Recipients = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    RetentionDays = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    OwnerUserId = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<int>(type: "int", nullable: true),
                    UpdatedBy = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReportConfigurations", x => x.ReportConfigurationId);
                    table.CheckConstraint("CK_ReportConfigurations_OutputFormat", "[OutputFormat] IN ('Pdf','Excel','Csv','Json')");
                    table.CheckConstraint("CK_ReportConfigurations_ReportType", "[ReportType] IN ('Statement','Regulatory','Operational','Financial','Analytics')");
                });

            migrationBuilder.CreateTable(
                name: "RiskAssessments",
                columns: table => new
                {
                    RiskAssessmentId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AssessmentType = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    RiskScore = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    RiskLevel = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Decision = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    ModelVersion = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    FactorsJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AssessedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CustomerId = table.Column<int>(type: "int", nullable: true),
                    AccountId = table.Column<int>(type: "int", nullable: true),
                    TransactionId = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<int>(type: "int", nullable: true),
                    UpdatedBy = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RiskAssessments", x => x.RiskAssessmentId);
                    table.CheckConstraint("CK_RiskAssessments_AssessmentType", "[AssessmentType] IN ('Customer','Account','Transaction')");
                    table.CheckConstraint("CK_RiskAssessments_Decision", "[Decision] IN ('Approve','Review','Challenge','Decline')");
                    table.CheckConstraint("CK_RiskAssessments_RiskLevel", "[RiskLevel] IN ('Low','Medium','High','Critical')");
                    table.CheckConstraint("CK_RiskAssessments_RiskScore", "[RiskScore] BETWEEN 0 AND 100");
                    table.CheckConstraint("CK_RiskAssessments_Subject", "[CustomerId] IS NOT NULL OR [AccountId] IS NOT NULL OR [TransactionId] IS NOT NULL");
                    table.ForeignKey(
                        name: "FK_RiskAssessments_Accounts_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Accounts",
                        principalColumn: "AccountId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RiskAssessments_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "CustomerId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RolePermissions",
                columns: table => new
                {
                    RoleId = table.Column<int>(type: "int", nullable: false),
                    PermissionId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RolePermissions", x => new { x.RoleId, x.PermissionId });
                    table.ForeignKey(
                        name: "FK_RolePermissions_Permissions_PermissionId",
                        column: x => x.PermissionId,
                        principalTable: "Permissions",
                        principalColumn: "PermissionId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    RoleId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    IsSystemRole = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<int>(type: "int", nullable: true),
                    UpdatedBy = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.RoleId);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Username = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: true),
                    PasswordHash = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: false),
                    PasswordChangedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    SecurityStamp = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    UserType = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsEmailVerified = table.Column<bool>(type: "bit", nullable: false),
                    IsPhoneVerified = table.Column<bool>(type: "bit", nullable: false),
                    FailedLoginAttempts = table.Column<int>(type: "int", nullable: false),
                    LockoutEndAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsTwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    TwoFactorSecretEncrypted = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
                    LastLoginAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastLoginIp = table.Column<string>(type: "nvarchar(45)", maxLength: 45, nullable: true),
                    RoleId = table.Column<int>(type: "int", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<int>(type: "int", nullable: true),
                    UpdatedBy = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.UserId);
                    table.CheckConstraint("CK_Users_UserType", "[UserType] IN ('Customer','Employee','Admin','System')");
                    table.ForeignKey(
                        name: "FK_Users_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "RoleId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Users_Users_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Users_Users_UpdatedBy",
                        column: x => x.UpdatedBy,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TransactionTypes",
                columns: table => new
                {
                    TransactionTypeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Category = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    RequiresApproval = table.Column<bool>(type: "bit", nullable: false),
                    ApprovalThreshold = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    DailyLimit = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    IsReversible = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<int>(type: "int", nullable: true),
                    UpdatedBy = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransactionTypes", x => x.TransactionTypeId);
                    table.CheckConstraint("CK_TransactionTypes_Category", "[Category] IN ('Deposit','Withdrawal','Transfer','Payment','Fee','Interest','Refund','Reversal','Loan','Card')");
                    table.ForeignKey(
                        name: "FK_TransactionTypes_Users_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TransactionTypes_Users_UpdatedBy",
                        column: x => x.UpdatedBy,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Transactions",
                columns: table => new
                {
                    TransactionId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TransactionReference = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    IdempotencyKey = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    ExternalReference = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    FeeAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    TaxAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    SourceBalanceAfter = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Channel = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    TransactionAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ValueDate = table.Column<DateTime>(type: "date", nullable: false),
                    PostedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FailureReason = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    Remarks = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    TransactionTypeId = table.Column<int>(type: "int", nullable: false),
                    SourceAccountId = table.Column<int>(type: "int", nullable: true),
                    DestinationAccountId = table.Column<int>(type: "int", nullable: true),
                    CurrencyId = table.Column<int>(type: "int", nullable: false),
                    FeeId = table.Column<int>(type: "int", nullable: true),
                    InitiatedByUserId = table.Column<int>(type: "int", nullable: true),
                    ApprovedByUserId = table.Column<int>(type: "int", nullable: true),
                    ReversalOfTransactionId = table.Column<int>(type: "int", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<int>(type: "int", nullable: true),
                    UpdatedBy = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Transactions", x => x.TransactionId);
                    table.CheckConstraint("CK_Transactions_Accounts", "[SourceAccountId] IS NOT NULL OR [DestinationAccountId] IS NOT NULL");
                    table.CheckConstraint("CK_Transactions_Amount", "[Amount] > 0");
                    table.CheckConstraint("CK_Transactions_Channel", "[Channel] IN ('Branch','ATM','NetBanking','MobileApp','UPI','POS','Api','Batch')");
                    table.CheckConstraint("CK_Transactions_Fees", "[FeeAmount] >= 0 AND [TaxAmount] >= 0");
                    table.CheckConstraint("CK_Transactions_Status", "[Status] IN ('Initiated','Pending','Processing','Posted','Failed','Reversed','Cancelled','OnHold')");
                    table.ForeignKey(
                        name: "FK_Transactions_Accounts_DestinationAccountId",
                        column: x => x.DestinationAccountId,
                        principalTable: "Accounts",
                        principalColumn: "AccountId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Transactions_Accounts_SourceAccountId",
                        column: x => x.SourceAccountId,
                        principalTable: "Accounts",
                        principalColumn: "AccountId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Transactions_Currencies_CurrencyId",
                        column: x => x.CurrencyId,
                        principalTable: "Currencies",
                        principalColumn: "CurrencyId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Transactions_Fees_FeeId",
                        column: x => x.FeeId,
                        principalTable: "Fees",
                        principalColumn: "FeeId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Transactions_TransactionTypes_TransactionTypeId",
                        column: x => x.TransactionTypeId,
                        principalTable: "TransactionTypes",
                        principalColumn: "TransactionTypeId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Transactions_Transactions_ReversalOfTransactionId",
                        column: x => x.ReversalOfTransactionId,
                        principalTable: "Transactions",
                        principalColumn: "TransactionId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Transactions_Users_ApprovedByUserId",
                        column: x => x.ApprovedByUserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Transactions_Users_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Transactions_Users_InitiatedByUserId",
                        column: x => x.InitiatedByUserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Transactions_Users_UpdatedBy",
                        column: x => x.UpdatedBy,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AccountHolders_CreatedBy",
                table: "AccountHolders",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_AccountHolders_CustomerId",
                table: "AccountHolders",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_AccountHolders_UpdatedBy",
                table: "AccountHolders",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "UX_AccountHolders_Account_Customer_Active",
                table: "AccountHolders",
                columns: new[] { "AccountId", "CustomerId" },
                unique: true,
                filter: "[IsActive] = 1");

            migrationBuilder.CreateIndex(
                name: "UX_AccountHolders_Account_OnePrimary",
                table: "AccountHolders",
                column: "AccountId",
                unique: true,
                filter: "[HolderType] = 'Primary' AND [IsActive] = 1");

            migrationBuilder.CreateIndex(
                name: "IX_Accounts_AccountTypeId",
                table: "Accounts",
                column: "AccountTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Accounts_BranchId_Status",
                table: "Accounts",
                columns: new[] { "BranchId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_Accounts_CreatedBy",
                table: "Accounts",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Accounts_CurrencyId",
                table: "Accounts",
                column: "CurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_Accounts_MaturityDate",
                table: "Accounts",
                column: "MaturityDate");

            migrationBuilder.CreateIndex(
                name: "IX_Accounts_PayoutAccountId",
                table: "Accounts",
                column: "PayoutAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_Accounts_UpdatedBy",
                table: "Accounts",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "UX_Accounts_AccountNumber",
                table: "Accounts",
                column: "AccountNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AccountTypes_Category",
                table: "AccountTypes",
                column: "Category");

            migrationBuilder.CreateIndex(
                name: "IX_AccountTypes_CreatedBy",
                table: "AccountTypes",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_AccountTypes_UpdatedBy",
                table: "AccountTypes",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "UX_AccountTypes_Code",
                table: "AccountTypes",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Addresses_CountryId",
                table: "Addresses",
                column: "CountryId");

            migrationBuilder.CreateIndex(
                name: "IX_Addresses_CreatedBy",
                table: "Addresses",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Addresses_CustomerId_AddressType",
                table: "Addresses",
                columns: new[] { "CustomerId", "AddressType" });

            migrationBuilder.CreateIndex(
                name: "IX_Addresses_PinCode",
                table: "Addresses",
                column: "PinCode");

            migrationBuilder.CreateIndex(
                name: "IX_Addresses_UpdatedBy",
                table: "Addresses",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "UX_AIChatMessages_AIConversationId_SequenceNumber",
                table: "AIChatMessages",
                columns: new[] { "AIConversationId", "SequenceNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AIConversations_CreatedBy",
                table: "AIConversations",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_AIConversations_CustomerId",
                table: "AIConversations",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_AIConversations_EscalatedToEmployeeId",
                table: "AIConversations",
                column: "EscalatedToEmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_AIConversations_Status",
                table: "AIConversations",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_AIConversations_UpdatedBy",
                table: "AIConversations",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_AIConversations_UserId_StartedAt",
                table: "AIConversations",
                columns: new[] { "UserId", "StartedAt" });

            migrationBuilder.CreateIndex(
                name: "UX_AIConversations_ConversationReference",
                table: "AIConversations",
                column: "ConversationReference",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AIUsageLogs_AIChatMessageId",
                table: "AIUsageLogs",
                column: "AIChatMessageId");

            migrationBuilder.CreateIndex(
                name: "IX_AIUsageLogs_AIConversationId",
                table: "AIUsageLogs",
                column: "AIConversationId");

            migrationBuilder.CreateIndex(
                name: "IX_AIUsageLogs_FeatureName_CreatedAt",
                table: "AIUsageLogs",
                columns: new[] { "FeatureName", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_AIUsageLogs_RequestId",
                table: "AIUsageLogs",
                column: "RequestId");

            migrationBuilder.CreateIndex(
                name: "IX_AIUsageLogs_UserId_CreatedAt",
                table: "AIUsageLogs",
                columns: new[] { "UserId", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_CorrelationId",
                table: "AuditLogs",
                column: "CorrelationId");

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_EntityName_EntityId",
                table: "AuditLogs",
                columns: new[] { "EntityName", "EntityId" });

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_IsSecuritySensitive_CreatedAt",
                table: "AuditLogs",
                columns: new[] { "IsSecuritySensitive", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_UserId_CreatedAt",
                table: "AuditLogs",
                columns: new[] { "UserId", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_Beneficiaries_CreatedBy",
                table: "Beneficiaries",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Beneficiaries_CustomerId_Status",
                table: "Beneficiaries",
                columns: new[] { "CustomerId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_Beneficiaries_InternalAccountId",
                table: "Beneficiaries",
                column: "InternalAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_Beneficiaries_UpdatedBy",
                table: "Beneficiaries",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "UX_Beneficiaries_Customer_Account_Ifsc",
                table: "Beneficiaries",
                columns: new[] { "CustomerId", "AccountNumber", "IfscCode" },
                unique: true,
                filter: "[Status] <> 'Removed' AND [AccountNumber] IS NOT NULL AND [IfscCode] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "UX_Beneficiaries_Customer_Vpa",
                table: "Beneficiaries",
                columns: new[] { "CustomerId", "UpiVpa" },
                unique: true,
                filter: "[Status] <> 'Removed' AND [UpiVpa] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Branches_CountryId",
                table: "Branches",
                column: "CountryId");

            migrationBuilder.CreateIndex(
                name: "IX_Branches_CreatedBy",
                table: "Branches",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Branches_ManagerEmployeeId",
                table: "Branches",
                column: "ManagerEmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_Branches_ParentBranchId",
                table: "Branches",
                column: "ParentBranchId");

            migrationBuilder.CreateIndex(
                name: "IX_Branches_State_City",
                table: "Branches",
                columns: new[] { "State", "City" });

            migrationBuilder.CreateIndex(
                name: "IX_Branches_UpdatedBy",
                table: "Branches",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "UX_Branches_BranchCode",
                table: "Branches",
                column: "BranchCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UX_Branches_IfscCode",
                table: "Branches",
                column: "IfscCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UX_Branches_MicrCode",
                table: "Branches",
                column: "MicrCode",
                unique: true,
                filter: "[MicrCode] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Cards_AccountId_Status",
                table: "Cards",
                columns: new[] { "AccountId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_Cards_CreatedBy",
                table: "Cards",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Cards_CustomerId_Status",
                table: "Cards",
                columns: new[] { "CustomerId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_Cards_UpdatedBy",
                table: "Cards",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "UX_Cards_CardNumberToken",
                table: "Cards",
                column: "CardNumberToken",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CardTransactions_CardId_AuthorizedAt",
                table: "CardTransactions",
                columns: new[] { "CardId", "AuthorizedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_CardTransactions_CreatedBy",
                table: "CardTransactions",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_CardTransactions_CurrencyId",
                table: "CardTransactions",
                column: "CurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_CardTransactions_MerchantCategoryCode",
                table: "CardTransactions",
                column: "MerchantCategoryCode");

            migrationBuilder.CreateIndex(
                name: "IX_CardTransactions_TransactionId",
                table: "CardTransactions",
                column: "TransactionId");

            migrationBuilder.CreateIndex(
                name: "IX_CardTransactions_UpdatedBy",
                table: "CardTransactions",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "UX_CardTransactions_RetrievalReferenceNumber",
                table: "CardTransactions",
                column: "RetrievalReferenceNumber",
                unique: true,
                filter: "[RetrievalReferenceNumber] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_ComplianceRecords_AccountId",
                table: "ComplianceRecords",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_ComplianceRecords_CreatedBy",
                table: "ComplianceRecords",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_ComplianceRecords_CustomerId",
                table: "ComplianceRecords",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_ComplianceRecords_DueDate",
                table: "ComplianceRecords",
                column: "DueDate");

            migrationBuilder.CreateIndex(
                name: "IX_ComplianceRecords_FiledByEmployeeId",
                table: "ComplianceRecords",
                column: "FiledByEmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_ComplianceRecords_FraudAlertId",
                table: "ComplianceRecords",
                column: "FraudAlertId");

            migrationBuilder.CreateIndex(
                name: "IX_ComplianceRecords_RecordType_Status",
                table: "ComplianceRecords",
                columns: new[] { "RecordType", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_ComplianceRecords_TransactionId",
                table: "ComplianceRecords",
                column: "TransactionId");

            migrationBuilder.CreateIndex(
                name: "IX_ComplianceRecords_UpdatedBy",
                table: "ComplianceRecords",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "UX_ComplianceRecords_ReferenceNumber",
                table: "ComplianceRecords",
                column: "ReferenceNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Countries_CreatedBy",
                table: "Countries",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Countries_UpdatedBy",
                table: "Countries",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "UX_Countries_IsoAlpha2Code",
                table: "Countries",
                column: "IsoAlpha2Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UX_Countries_IsoAlpha3Code",
                table: "Countries",
                column: "IsoAlpha3Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UX_Countries_Name",
                table: "Countries",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Currencies_CountryId",
                table: "Currencies",
                column: "CountryId");

            migrationBuilder.CreateIndex(
                name: "IX_Currencies_CreatedBy",
                table: "Currencies",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Currencies_UpdatedBy",
                table: "Currencies",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "UX_Currencies_CurrencyCode",
                table: "Currencies",
                column: "CurrencyCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UX_Currencies_NumericCode",
                table: "Currencies",
                column: "NumericCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Customers_CreatedBy",
                table: "Customers",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Customers_Email",
                table: "Customers",
                column: "Email");

            migrationBuilder.CreateIndex(
                name: "IX_Customers_HomeBranchId",
                table: "Customers",
                column: "HomeBranchId");

            migrationBuilder.CreateIndex(
                name: "IX_Customers_LastName_FirstName",
                table: "Customers",
                columns: new[] { "LastName", "FirstName" });

            migrationBuilder.CreateIndex(
                name: "IX_Customers_MobileNumber",
                table: "Customers",
                column: "MobileNumber");

            migrationBuilder.CreateIndex(
                name: "IX_Customers_NationalityCountryId",
                table: "Customers",
                column: "NationalityCountryId");

            migrationBuilder.CreateIndex(
                name: "IX_Customers_RelationshipManagerId",
                table: "Customers",
                column: "RelationshipManagerId");

            migrationBuilder.CreateIndex(
                name: "IX_Customers_UpdatedBy",
                table: "Customers",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Customers_UserId",
                table: "Customers",
                column: "UserId",
                unique: true,
                filter: "[UserId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "UX_Customers_CustomerNumber",
                table: "Customers",
                column: "CustomerNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Disputes_AssignedToEmployeeId",
                table: "Disputes",
                column: "AssignedToEmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_Disputes_CardTransactionId",
                table: "Disputes",
                column: "CardTransactionId");

            migrationBuilder.CreateIndex(
                name: "IX_Disputes_CreatedBy",
                table: "Disputes",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Disputes_CustomerId_Status",
                table: "Disputes",
                columns: new[] { "CustomerId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_Disputes_Status_DueBy",
                table: "Disputes",
                columns: new[] { "Status", "DueBy" });

            migrationBuilder.CreateIndex(
                name: "IX_Disputes_TransactionId",
                table: "Disputes",
                column: "TransactionId");

            migrationBuilder.CreateIndex(
                name: "IX_Disputes_UpdatedBy",
                table: "Disputes",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "UX_Disputes_DisputeNumber",
                table: "Disputes",
                column: "DisputeNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Documents_ContentHash",
                table: "Documents",
                column: "ContentHash");

            migrationBuilder.CreateIndex(
                name: "IX_Documents_CreatedBy",
                table: "Documents",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Documents_CustomerId_DocumentType",
                table: "Documents",
                columns: new[] { "CustomerId", "DocumentType" });

            migrationBuilder.CreateIndex(
                name: "IX_Documents_KYCDetailsId",
                table: "Documents",
                column: "KYCDetailsId");

            migrationBuilder.CreateIndex(
                name: "IX_Documents_LoanId",
                table: "Documents",
                column: "LoanId");

            migrationBuilder.CreateIndex(
                name: "IX_Documents_UpdatedBy",
                table: "Documents",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Documents_VerifiedByEmployeeId",
                table: "Documents",
                column: "VerifiedByEmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_BranchId_EmploymentStatus",
                table: "Employees",
                columns: new[] { "BranchId", "EmploymentStatus" });

            migrationBuilder.CreateIndex(
                name: "IX_Employees_CreatedBy",
                table: "Employees",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_ReportsToEmployeeId",
                table: "Employees",
                column: "ReportsToEmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_UpdatedBy",
                table: "Employees",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_UserId",
                table: "Employees",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UX_Employees_EmployeeCode",
                table: "Employees",
                column: "EmployeeCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Fees_AccountTypeId",
                table: "Fees",
                column: "AccountTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Fees_CreatedBy",
                table: "Fees",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Fees_IsActive_EffectiveFrom",
                table: "Fees",
                columns: new[] { "IsActive", "EffectiveFrom" });

            migrationBuilder.CreateIndex(
                name: "IX_Fees_PaymentMethodId",
                table: "Fees",
                column: "PaymentMethodId");

            migrationBuilder.CreateIndex(
                name: "IX_Fees_TransactionTypeId",
                table: "Fees",
                column: "TransactionTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Fees_UpdatedBy",
                table: "Fees",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "UX_Fees_Code",
                table: "Fees",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FraudAlerts_AccountId",
                table: "FraudAlerts",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_FraudAlerts_CardId",
                table: "FraudAlerts",
                column: "CardId");

            migrationBuilder.CreateIndex(
                name: "IX_FraudAlerts_CreatedBy",
                table: "FraudAlerts",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_FraudAlerts_CustomerId_Status",
                table: "FraudAlerts",
                columns: new[] { "CustomerId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_FraudAlerts_FraudRuleId",
                table: "FraudAlerts",
                column: "FraudRuleId");

            migrationBuilder.CreateIndex(
                name: "IX_FraudAlerts_ReviewedByEmployeeId",
                table: "FraudAlerts",
                column: "ReviewedByEmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_FraudAlerts_RiskAssessmentId",
                table: "FraudAlerts",
                column: "RiskAssessmentId");

            migrationBuilder.CreateIndex(
                name: "IX_FraudAlerts_Status_RaisedAt",
                table: "FraudAlerts",
                columns: new[] { "Status", "RaisedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_FraudAlerts_TransactionId",
                table: "FraudAlerts",
                column: "TransactionId");

            migrationBuilder.CreateIndex(
                name: "IX_FraudAlerts_UpdatedBy",
                table: "FraudAlerts",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "UX_FraudAlerts_AlertNumber",
                table: "FraudAlerts",
                column: "AlertNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FraudRules_CreatedBy",
                table: "FraudRules",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_FraudRules_IsActive_Priority",
                table: "FraudRules",
                columns: new[] { "IsActive", "Priority" });

            migrationBuilder.CreateIndex(
                name: "IX_FraudRules_UpdatedBy",
                table: "FraudRules",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "UX_FraudRules_RuleCode",
                table: "FraudRules",
                column: "RuleCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_InterestCalculations_CreatedBy",
                table: "InterestCalculations",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_InterestCalculations_InterestRateId",
                table: "InterestCalculations",
                column: "InterestRateId");

            migrationBuilder.CreateIndex(
                name: "IX_InterestCalculations_JournalEntryId",
                table: "InterestCalculations",
                column: "JournalEntryId");

            migrationBuilder.CreateIndex(
                name: "IX_InterestCalculations_Status",
                table: "InterestCalculations",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_InterestCalculations_TransactionId",
                table: "InterestCalculations",
                column: "TransactionId");

            migrationBuilder.CreateIndex(
                name: "IX_InterestCalculations_UpdatedBy",
                table: "InterestCalculations",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "UX_InterestCalculations_Account_Period",
                table: "InterestCalculations",
                columns: new[] { "AccountId", "PeriodStart", "PeriodEnd", "CalculationType" },
                unique: true,
                filter: "[AccountId] IS NOT NULL AND [Status] <> 'Reversed'");

            migrationBuilder.CreateIndex(
                name: "UX_InterestCalculations_Loan_Period",
                table: "InterestCalculations",
                columns: new[] { "LoanId", "PeriodStart", "PeriodEnd", "CalculationType" },
                unique: true,
                filter: "[LoanId] IS NOT NULL AND [Status] <> 'Reversed'");

            migrationBuilder.CreateIndex(
                name: "IX_InterestRates_AccountTypeId_EffectiveFrom",
                table: "InterestRates",
                columns: new[] { "AccountTypeId", "EffectiveFrom" });

            migrationBuilder.CreateIndex(
                name: "IX_InterestRates_CreatedBy",
                table: "InterestRates",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_InterestRates_ProductCategory_IsActive",
                table: "InterestRates",
                columns: new[] { "ProductCategory", "IsActive" });

            migrationBuilder.CreateIndex(
                name: "IX_InterestRates_UpdatedBy",
                table: "InterestRates",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_JournalEntries_CreatedBy",
                table: "JournalEntries",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_JournalEntries_CurrencyId",
                table: "JournalEntries",
                column: "CurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_JournalEntries_EntryDate_Status",
                table: "JournalEntries",
                columns: new[] { "EntryDate", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_JournalEntries_PostedByUserId",
                table: "JournalEntries",
                column: "PostedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_JournalEntries_ReversalOfJournalEntryId",
                table: "JournalEntries",
                column: "ReversalOfJournalEntryId");

            migrationBuilder.CreateIndex(
                name: "IX_JournalEntries_TransactionId",
                table: "JournalEntries",
                column: "TransactionId");

            migrationBuilder.CreateIndex(
                name: "IX_JournalEntries_UpdatedBy",
                table: "JournalEntries",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "UX_JournalEntries_JournalNumber",
                table: "JournalEntries",
                column: "JournalNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_JournalEntryLines_AccountId",
                table: "JournalEntryLines",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_JournalEntryLines_CreatedBy",
                table: "JournalEntryLines",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "UX_JournalEntryLines_JournalEntryId_LineNumber",
                table: "JournalEntryLines",
                columns: new[] { "JournalEntryId", "LineNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_KYCDetails_CkycNumber",
                table: "KYCDetails",
                column: "CkycNumber");

            migrationBuilder.CreateIndex(
                name: "IX_KYCDetails_CreatedBy",
                table: "KYCDetails",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_KYCDetails_UpdatedBy",
                table: "KYCDetails",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_KYCDetails_VerifiedByEmployeeId",
                table: "KYCDetails",
                column: "VerifiedByEmployeeId");

            migrationBuilder.CreateIndex(
                name: "UX_KYCDetails_CustomerId_Current",
                table: "KYCDetails",
                column: "CustomerId",
                unique: true,
                filter: "[IsCurrent] = 1");

            migrationBuilder.CreateIndex(
                name: "UX_KYCDetails_PanHash_Current",
                table: "KYCDetails",
                column: "PanHash",
                unique: true,
                filter: "[IsCurrent] = 1 AND [PanHash] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_LoanRepayments_CreatedBy",
                table: "LoanRepayments",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_LoanRepayments_LoanId_PaidAt",
                table: "LoanRepayments",
                columns: new[] { "LoanId", "PaidAt" });

            migrationBuilder.CreateIndex(
                name: "IX_LoanRepayments_LoanScheduleId",
                table: "LoanRepayments",
                column: "LoanScheduleId");

            migrationBuilder.CreateIndex(
                name: "IX_LoanRepayments_TransactionId",
                table: "LoanRepayments",
                column: "TransactionId");

            migrationBuilder.CreateIndex(
                name: "IX_LoanRepayments_UpdatedBy",
                table: "LoanRepayments",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "UX_LoanRepayments_RepaymentReference",
                table: "LoanRepayments",
                column: "RepaymentReference",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Loans_ApprovedByEmployeeId",
                table: "Loans",
                column: "ApprovedByEmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_Loans_AssetClassification",
                table: "Loans",
                column: "AssetClassification");

            migrationBuilder.CreateIndex(
                name: "IX_Loans_BranchId",
                table: "Loans",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "IX_Loans_CreatedBy",
                table: "Loans",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Loans_CustomerId_Status",
                table: "Loans",
                columns: new[] { "CustomerId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_Loans_DisbursementAccountId",
                table: "Loans",
                column: "DisbursementAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_Loans_InterestRateId",
                table: "Loans",
                column: "InterestRateId");

            migrationBuilder.CreateIndex(
                name: "IX_Loans_RepaymentAccountId",
                table: "Loans",
                column: "RepaymentAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_Loans_UpdatedBy",
                table: "Loans",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "UX_Loans_LoanNumber",
                table: "Loans",
                column: "LoanNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LoanSchedules_CreatedBy",
                table: "LoanSchedules",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_LoanSchedules_DueDate_Status",
                table: "LoanSchedules",
                columns: new[] { "DueDate", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_LoanSchedules_UpdatedBy",
                table: "LoanSchedules",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "UX_LoanSchedules_LoanId_InstallmentNumber",
                table: "LoanSchedules",
                columns: new[] { "LoanId", "InstallmentNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_CreatedBy",
                table: "Notifications",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_CustomerId_CreatedAt",
                table: "Notifications",
                columns: new[] { "CustomerId", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_NotificationTemplateId",
                table: "Notifications",
                column: "NotificationTemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_Status_ScheduledAt",
                table: "Notifications",
                columns: new[] { "Status", "ScheduledAt" });

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_UpdatedBy",
                table: "Notifications",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_UserId_Status",
                table: "Notifications",
                columns: new[] { "UserId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_NotificationTemplates_CreatedBy",
                table: "NotificationTemplates",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_NotificationTemplates_UpdatedBy",
                table: "NotificationTemplates",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "UX_NotificationTemplates_TemplateCode_Channel_Language",
                table: "NotificationTemplates",
                columns: new[] { "TemplateCode", "Channel", "Language" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PaymentGatewayLogs_CorrelationId",
                table: "PaymentGatewayLogs",
                column: "CorrelationId");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentGatewayLogs_PaymentRequestId_CreatedAt",
                table: "PaymentGatewayLogs",
                columns: new[] { "PaymentRequestId", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_PaymentMethods_CreatedBy",
                table: "PaymentMethods",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentMethods_Rail",
                table: "PaymentMethods",
                column: "Rail");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentMethods_UpdatedBy",
                table: "PaymentMethods",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "UX_PaymentMethods_Code",
                table: "PaymentMethods",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PaymentRequests_BeneficiaryId",
                table: "PaymentRequests",
                column: "BeneficiaryId");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentRequests_CreatedBy",
                table: "PaymentRequests",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentRequests_CurrencyId",
                table: "PaymentRequests",
                column: "CurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentRequests_CustomerId_RequestedAt",
                table: "PaymentRequests",
                columns: new[] { "CustomerId", "RequestedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_PaymentRequests_PaymentMethodId",
                table: "PaymentRequests",
                column: "PaymentMethodId");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentRequests_SourceAccountId",
                table: "PaymentRequests",
                column: "SourceAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentRequests_Status",
                table: "PaymentRequests",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentRequests_TransactionId",
                table: "PaymentRequests",
                column: "TransactionId",
                unique: true,
                filter: "[TransactionId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentRequests_UpdatedBy",
                table: "PaymentRequests",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentRequests_UtrNumber",
                table: "PaymentRequests",
                column: "UtrNumber");

            migrationBuilder.CreateIndex(
                name: "UX_PaymentRequests_IdempotencyKey",
                table: "PaymentRequests",
                column: "IdempotencyKey",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UX_PaymentRequests_RequestReference",
                table: "PaymentRequests",
                column: "RequestReference",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Permissions_CreatedBy",
                table: "Permissions",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Permissions_ModuleName",
                table: "Permissions",
                column: "ModuleName");

            migrationBuilder.CreateIndex(
                name: "IX_Permissions_UpdatedBy",
                table: "Permissions",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "UX_Permissions_Code",
                table: "Permissions",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ReconciliationBatches_CreatedBy",
                table: "ReconciliationBatches",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_ReconciliationBatches_InitiatedByUserId",
                table: "ReconciliationBatches",
                column: "InitiatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ReconciliationBatches_PaymentMethodId",
                table: "ReconciliationBatches",
                column: "PaymentMethodId");

            migrationBuilder.CreateIndex(
                name: "IX_ReconciliationBatches_ReconciliationType_BusinessDate",
                table: "ReconciliationBatches",
                columns: new[] { "ReconciliationType", "BusinessDate" });

            migrationBuilder.CreateIndex(
                name: "IX_ReconciliationBatches_UpdatedBy",
                table: "ReconciliationBatches",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "UX_ReconciliationBatches_BatchNumber",
                table: "ReconciliationBatches",
                column: "BatchNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UX_ReconciliationBatches_FileHash",
                table: "ReconciliationBatches",
                column: "FileHash",
                unique: true,
                filter: "[FileHash] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_ReconciliationRecords_CreatedBy",
                table: "ReconciliationRecords",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_ReconciliationRecords_ExternalReference",
                table: "ReconciliationRecords",
                column: "ExternalReference");

            migrationBuilder.CreateIndex(
                name: "IX_ReconciliationRecords_ReconciliationBatchId_MatchStatus",
                table: "ReconciliationRecords",
                columns: new[] { "ReconciliationBatchId", "MatchStatus" });

            migrationBuilder.CreateIndex(
                name: "IX_ReconciliationRecords_ResolvedByUserId",
                table: "ReconciliationRecords",
                column: "ResolvedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ReconciliationRecords_TransactionId",
                table: "ReconciliationRecords",
                column: "TransactionId");

            migrationBuilder.CreateIndex(
                name: "IX_ReconciliationRecords_UpdatedBy",
                table: "ReconciliationRecords",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_RefreshTokens_JwtId",
                table: "RefreshTokens",
                column: "JwtId");

            migrationBuilder.CreateIndex(
                name: "IX_RefreshTokens_ReplacedByTokenId",
                table: "RefreshTokens",
                column: "ReplacedByTokenId");

            migrationBuilder.CreateIndex(
                name: "IX_RefreshTokens_UserId_ExpiresAt",
                table: "RefreshTokens",
                columns: new[] { "UserId", "ExpiresAt" });

            migrationBuilder.CreateIndex(
                name: "UX_RefreshTokens_TokenHash",
                table: "RefreshTokens",
                column: "TokenHash",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Refunds_ApprovedByUserId",
                table: "Refunds",
                column: "ApprovedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Refunds_CreatedBy",
                table: "Refunds",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Refunds_DisputeId",
                table: "Refunds",
                column: "DisputeId");

            migrationBuilder.CreateIndex(
                name: "IX_Refunds_OriginalTransactionId_Status",
                table: "Refunds",
                columns: new[] { "OriginalTransactionId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_Refunds_RefundTransactionId",
                table: "Refunds",
                column: "RefundTransactionId",
                unique: true,
                filter: "[RefundTransactionId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Refunds_RequestedByUserId",
                table: "Refunds",
                column: "RequestedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Refunds_UpdatedBy",
                table: "Refunds",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "UX_Refunds_RefundReference",
                table: "Refunds",
                column: "RefundReference",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ReportConfigurations_CreatedBy",
                table: "ReportConfigurations",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_ReportConfigurations_IsScheduled_NextRunAt",
                table: "ReportConfigurations",
                columns: new[] { "IsScheduled", "NextRunAt" });

            migrationBuilder.CreateIndex(
                name: "IX_ReportConfigurations_OwnerUserId",
                table: "ReportConfigurations",
                column: "OwnerUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ReportConfigurations_UpdatedBy",
                table: "ReportConfigurations",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "UX_ReportConfigurations_ReportCode",
                table: "ReportConfigurations",
                column: "ReportCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RiskAssessments_AccountId",
                table: "RiskAssessments",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_RiskAssessments_CreatedBy",
                table: "RiskAssessments",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_RiskAssessments_CustomerId_AssessedAt",
                table: "RiskAssessments",
                columns: new[] { "CustomerId", "AssessedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_RiskAssessments_RiskLevel",
                table: "RiskAssessments",
                column: "RiskLevel");

            migrationBuilder.CreateIndex(
                name: "IX_RiskAssessments_TransactionId",
                table: "RiskAssessments",
                column: "TransactionId");

            migrationBuilder.CreateIndex(
                name: "IX_RiskAssessments_UpdatedBy",
                table: "RiskAssessments",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_RolePermissions_PermissionId",
                table: "RolePermissions",
                column: "PermissionId");

            migrationBuilder.CreateIndex(
                name: "IX_Roles_CreatedBy",
                table: "Roles",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Roles_UpdatedBy",
                table: "Roles",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "UX_Roles_Name",
                table: "Roles",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_ApprovedByUserId",
                table: "Transactions",
                column: "ApprovedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_CreatedBy",
                table: "Transactions",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_CurrencyId",
                table: "Transactions",
                column: "CurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_DestinationAccountId_TransactionAt",
                table: "Transactions",
                columns: new[] { "DestinationAccountId", "TransactionAt" });

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_ExternalReference",
                table: "Transactions",
                column: "ExternalReference");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_FeeId",
                table: "Transactions",
                column: "FeeId");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_InitiatedByUserId",
                table: "Transactions",
                column: "InitiatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_ReversalOfTransactionId",
                table: "Transactions",
                column: "ReversalOfTransactionId");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_SourceAccountId_TransactionAt",
                table: "Transactions",
                columns: new[] { "SourceAccountId", "TransactionAt" });

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_Status_TransactionAt",
                table: "Transactions",
                columns: new[] { "Status", "TransactionAt" });

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_TransactionTypeId",
                table: "Transactions",
                column: "TransactionTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_UpdatedBy",
                table: "Transactions",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "UX_Transactions_IdempotencyKey",
                table: "Transactions",
                column: "IdempotencyKey",
                unique: true,
                filter: "[IdempotencyKey] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "UX_Transactions_TransactionReference",
                table: "Transactions",
                column: "TransactionReference",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TransactionTypes_Category",
                table: "TransactionTypes",
                column: "Category");

            migrationBuilder.CreateIndex(
                name: "IX_TransactionTypes_CreatedBy",
                table: "TransactionTypes",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_TransactionTypes_UpdatedBy",
                table: "TransactionTypes",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "UX_TransactionTypes_Code",
                table: "TransactionTypes",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_CreatedBy",
                table: "Users",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Users_PhoneNumber",
                table: "Users",
                column: "PhoneNumber");

            migrationBuilder.CreateIndex(
                name: "IX_Users_RoleId",
                table: "Users",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_UpdatedBy",
                table: "Users",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "UX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UX_Users_Username",
                table: "Users",
                column: "Username",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_AccountHolders_Accounts_AccountId",
                table: "AccountHolders",
                column: "AccountId",
                principalTable: "Accounts",
                principalColumn: "AccountId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AccountHolders_Customers_CustomerId",
                table: "AccountHolders",
                column: "CustomerId",
                principalTable: "Customers",
                principalColumn: "CustomerId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AccountHolders_Users_CreatedBy",
                table: "AccountHolders",
                column: "CreatedBy",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AccountHolders_Users_UpdatedBy",
                table: "AccountHolders",
                column: "UpdatedBy",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Accounts_AccountTypes_AccountTypeId",
                table: "Accounts",
                column: "AccountTypeId",
                principalTable: "AccountTypes",
                principalColumn: "AccountTypeId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Accounts_Branches_BranchId",
                table: "Accounts",
                column: "BranchId",
                principalTable: "Branches",
                principalColumn: "BranchId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Accounts_Currencies_CurrencyId",
                table: "Accounts",
                column: "CurrencyId",
                principalTable: "Currencies",
                principalColumn: "CurrencyId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Accounts_Users_CreatedBy",
                table: "Accounts",
                column: "CreatedBy",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Accounts_Users_UpdatedBy",
                table: "Accounts",
                column: "UpdatedBy",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AccountTypes_Users_CreatedBy",
                table: "AccountTypes",
                column: "CreatedBy",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AccountTypes_Users_UpdatedBy",
                table: "AccountTypes",
                column: "UpdatedBy",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Addresses_Countries_CountryId",
                table: "Addresses",
                column: "CountryId",
                principalTable: "Countries",
                principalColumn: "CountryId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Addresses_Customers_CustomerId",
                table: "Addresses",
                column: "CustomerId",
                principalTable: "Customers",
                principalColumn: "CustomerId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Addresses_Users_CreatedBy",
                table: "Addresses",
                column: "CreatedBy",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Addresses_Users_UpdatedBy",
                table: "Addresses",
                column: "UpdatedBy",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AIChatMessages_AIConversations_AIConversationId",
                table: "AIChatMessages",
                column: "AIConversationId",
                principalTable: "AIConversations",
                principalColumn: "AIConversationId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AIConversations_Customers_CustomerId",
                table: "AIConversations",
                column: "CustomerId",
                principalTable: "Customers",
                principalColumn: "CustomerId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AIConversations_Employees_EscalatedToEmployeeId",
                table: "AIConversations",
                column: "EscalatedToEmployeeId",
                principalTable: "Employees",
                principalColumn: "EmployeeId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AIConversations_Users_CreatedBy",
                table: "AIConversations",
                column: "CreatedBy",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AIConversations_Users_UpdatedBy",
                table: "AIConversations",
                column: "UpdatedBy",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AIConversations_Users_UserId",
                table: "AIConversations",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AIUsageLogs_Users_UserId",
                table: "AIUsageLogs",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AuditLogs_Users_UserId",
                table: "AuditLogs",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Beneficiaries_Customers_CustomerId",
                table: "Beneficiaries",
                column: "CustomerId",
                principalTable: "Customers",
                principalColumn: "CustomerId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Beneficiaries_Users_CreatedBy",
                table: "Beneficiaries",
                column: "CreatedBy",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Beneficiaries_Users_UpdatedBy",
                table: "Beneficiaries",
                column: "UpdatedBy",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Branches_Countries_CountryId",
                table: "Branches",
                column: "CountryId",
                principalTable: "Countries",
                principalColumn: "CountryId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Branches_Employees_ManagerEmployeeId",
                table: "Branches",
                column: "ManagerEmployeeId",
                principalTable: "Employees",
                principalColumn: "EmployeeId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Branches_Users_CreatedBy",
                table: "Branches",
                column: "CreatedBy",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Branches_Users_UpdatedBy",
                table: "Branches",
                column: "UpdatedBy",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Cards_Customers_CustomerId",
                table: "Cards",
                column: "CustomerId",
                principalTable: "Customers",
                principalColumn: "CustomerId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Cards_Users_CreatedBy",
                table: "Cards",
                column: "CreatedBy",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Cards_Users_UpdatedBy",
                table: "Cards",
                column: "UpdatedBy",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CardTransactions_Currencies_CurrencyId",
                table: "CardTransactions",
                column: "CurrencyId",
                principalTable: "Currencies",
                principalColumn: "CurrencyId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CardTransactions_Transactions_TransactionId",
                table: "CardTransactions",
                column: "TransactionId",
                principalTable: "Transactions",
                principalColumn: "TransactionId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CardTransactions_Users_CreatedBy",
                table: "CardTransactions",
                column: "CreatedBy",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CardTransactions_Users_UpdatedBy",
                table: "CardTransactions",
                column: "UpdatedBy",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ComplianceRecords_Customers_CustomerId",
                table: "ComplianceRecords",
                column: "CustomerId",
                principalTable: "Customers",
                principalColumn: "CustomerId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ComplianceRecords_Employees_FiledByEmployeeId",
                table: "ComplianceRecords",
                column: "FiledByEmployeeId",
                principalTable: "Employees",
                principalColumn: "EmployeeId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ComplianceRecords_FraudAlerts_FraudAlertId",
                table: "ComplianceRecords",
                column: "FraudAlertId",
                principalTable: "FraudAlerts",
                principalColumn: "FraudAlertId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ComplianceRecords_Transactions_TransactionId",
                table: "ComplianceRecords",
                column: "TransactionId",
                principalTable: "Transactions",
                principalColumn: "TransactionId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ComplianceRecords_Users_CreatedBy",
                table: "ComplianceRecords",
                column: "CreatedBy",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ComplianceRecords_Users_UpdatedBy",
                table: "ComplianceRecords",
                column: "UpdatedBy",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Countries_Users_CreatedBy",
                table: "Countries",
                column: "CreatedBy",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Countries_Users_UpdatedBy",
                table: "Countries",
                column: "UpdatedBy",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Currencies_Users_CreatedBy",
                table: "Currencies",
                column: "CreatedBy",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Currencies_Users_UpdatedBy",
                table: "Currencies",
                column: "UpdatedBy",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Customers_Employees_RelationshipManagerId",
                table: "Customers",
                column: "RelationshipManagerId",
                principalTable: "Employees",
                principalColumn: "EmployeeId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Customers_Users_CreatedBy",
                table: "Customers",
                column: "CreatedBy",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Customers_Users_UpdatedBy",
                table: "Customers",
                column: "UpdatedBy",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Customers_Users_UserId",
                table: "Customers",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Disputes_Employees_AssignedToEmployeeId",
                table: "Disputes",
                column: "AssignedToEmployeeId",
                principalTable: "Employees",
                principalColumn: "EmployeeId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Disputes_Transactions_TransactionId",
                table: "Disputes",
                column: "TransactionId",
                principalTable: "Transactions",
                principalColumn: "TransactionId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Disputes_Users_CreatedBy",
                table: "Disputes",
                column: "CreatedBy",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Disputes_Users_UpdatedBy",
                table: "Disputes",
                column: "UpdatedBy",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Documents_Employees_VerifiedByEmployeeId",
                table: "Documents",
                column: "VerifiedByEmployeeId",
                principalTable: "Employees",
                principalColumn: "EmployeeId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Documents_KYCDetails_KYCDetailsId",
                table: "Documents",
                column: "KYCDetailsId",
                principalTable: "KYCDetails",
                principalColumn: "KYCDetailsId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Documents_Loans_LoanId",
                table: "Documents",
                column: "LoanId",
                principalTable: "Loans",
                principalColumn: "LoanId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Documents_Users_CreatedBy",
                table: "Documents",
                column: "CreatedBy",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Documents_Users_UpdatedBy",
                table: "Documents",
                column: "UpdatedBy",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Employees_Users_CreatedBy",
                table: "Employees",
                column: "CreatedBy",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Employees_Users_UpdatedBy",
                table: "Employees",
                column: "UpdatedBy",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Employees_Users_UserId",
                table: "Employees",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Fees_PaymentMethods_PaymentMethodId",
                table: "Fees",
                column: "PaymentMethodId",
                principalTable: "PaymentMethods",
                principalColumn: "PaymentMethodId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Fees_TransactionTypes_TransactionTypeId",
                table: "Fees",
                column: "TransactionTypeId",
                principalTable: "TransactionTypes",
                principalColumn: "TransactionTypeId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Fees_Users_CreatedBy",
                table: "Fees",
                column: "CreatedBy",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Fees_Users_UpdatedBy",
                table: "Fees",
                column: "UpdatedBy",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_FraudAlerts_FraudRules_FraudRuleId",
                table: "FraudAlerts",
                column: "FraudRuleId",
                principalTable: "FraudRules",
                principalColumn: "FraudRuleId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_FraudAlerts_RiskAssessments_RiskAssessmentId",
                table: "FraudAlerts",
                column: "RiskAssessmentId",
                principalTable: "RiskAssessments",
                principalColumn: "RiskAssessmentId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_FraudAlerts_Transactions_TransactionId",
                table: "FraudAlerts",
                column: "TransactionId",
                principalTable: "Transactions",
                principalColumn: "TransactionId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_FraudAlerts_Users_CreatedBy",
                table: "FraudAlerts",
                column: "CreatedBy",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_FraudAlerts_Users_UpdatedBy",
                table: "FraudAlerts",
                column: "UpdatedBy",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_FraudRules_Users_CreatedBy",
                table: "FraudRules",
                column: "CreatedBy",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_FraudRules_Users_UpdatedBy",
                table: "FraudRules",
                column: "UpdatedBy",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_InterestCalculations_InterestRates_InterestRateId",
                table: "InterestCalculations",
                column: "InterestRateId",
                principalTable: "InterestRates",
                principalColumn: "InterestRateId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_InterestCalculations_JournalEntries_JournalEntryId",
                table: "InterestCalculations",
                column: "JournalEntryId",
                principalTable: "JournalEntries",
                principalColumn: "JournalEntryId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_InterestCalculations_Loans_LoanId",
                table: "InterestCalculations",
                column: "LoanId",
                principalTable: "Loans",
                principalColumn: "LoanId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_InterestCalculations_Transactions_TransactionId",
                table: "InterestCalculations",
                column: "TransactionId",
                principalTable: "Transactions",
                principalColumn: "TransactionId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_InterestCalculations_Users_CreatedBy",
                table: "InterestCalculations",
                column: "CreatedBy",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_InterestCalculations_Users_UpdatedBy",
                table: "InterestCalculations",
                column: "UpdatedBy",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_InterestRates_Users_CreatedBy",
                table: "InterestRates",
                column: "CreatedBy",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_InterestRates_Users_UpdatedBy",
                table: "InterestRates",
                column: "UpdatedBy",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_JournalEntries_Transactions_TransactionId",
                table: "JournalEntries",
                column: "TransactionId",
                principalTable: "Transactions",
                principalColumn: "TransactionId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_JournalEntries_Users_CreatedBy",
                table: "JournalEntries",
                column: "CreatedBy",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_JournalEntries_Users_PostedByUserId",
                table: "JournalEntries",
                column: "PostedByUserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_JournalEntries_Users_UpdatedBy",
                table: "JournalEntries",
                column: "UpdatedBy",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_JournalEntryLines_Users_CreatedBy",
                table: "JournalEntryLines",
                column: "CreatedBy",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_KYCDetails_Users_CreatedBy",
                table: "KYCDetails",
                column: "CreatedBy",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_KYCDetails_Users_UpdatedBy",
                table: "KYCDetails",
                column: "UpdatedBy",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_LoanRepayments_LoanSchedules_LoanScheduleId",
                table: "LoanRepayments",
                column: "LoanScheduleId",
                principalTable: "LoanSchedules",
                principalColumn: "LoanScheduleId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_LoanRepayments_Loans_LoanId",
                table: "LoanRepayments",
                column: "LoanId",
                principalTable: "Loans",
                principalColumn: "LoanId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_LoanRepayments_Transactions_TransactionId",
                table: "LoanRepayments",
                column: "TransactionId",
                principalTable: "Transactions",
                principalColumn: "TransactionId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_LoanRepayments_Users_CreatedBy",
                table: "LoanRepayments",
                column: "CreatedBy",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_LoanRepayments_Users_UpdatedBy",
                table: "LoanRepayments",
                column: "UpdatedBy",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Loans_Users_CreatedBy",
                table: "Loans",
                column: "CreatedBy",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Loans_Users_UpdatedBy",
                table: "Loans",
                column: "UpdatedBy",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_LoanSchedules_Users_CreatedBy",
                table: "LoanSchedules",
                column: "CreatedBy",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_LoanSchedules_Users_UpdatedBy",
                table: "LoanSchedules",
                column: "UpdatedBy",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_NotificationTemplates_NotificationTemplateId",
                table: "Notifications",
                column: "NotificationTemplateId",
                principalTable: "NotificationTemplates",
                principalColumn: "NotificationTemplateId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_Users_CreatedBy",
                table: "Notifications",
                column: "CreatedBy",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_Users_UpdatedBy",
                table: "Notifications",
                column: "UpdatedBy",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_Users_UserId",
                table: "Notifications",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_NotificationTemplates_Users_CreatedBy",
                table: "NotificationTemplates",
                column: "CreatedBy",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_NotificationTemplates_Users_UpdatedBy",
                table: "NotificationTemplates",
                column: "UpdatedBy",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PaymentGatewayLogs_PaymentRequests_PaymentRequestId",
                table: "PaymentGatewayLogs",
                column: "PaymentRequestId",
                principalTable: "PaymentRequests",
                principalColumn: "PaymentRequestId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PaymentMethods_Users_CreatedBy",
                table: "PaymentMethods",
                column: "CreatedBy",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PaymentMethods_Users_UpdatedBy",
                table: "PaymentMethods",
                column: "UpdatedBy",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PaymentRequests_Transactions_TransactionId",
                table: "PaymentRequests",
                column: "TransactionId",
                principalTable: "Transactions",
                principalColumn: "TransactionId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PaymentRequests_Users_CreatedBy",
                table: "PaymentRequests",
                column: "CreatedBy",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PaymentRequests_Users_UpdatedBy",
                table: "PaymentRequests",
                column: "UpdatedBy",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Permissions_Users_CreatedBy",
                table: "Permissions",
                column: "CreatedBy",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Permissions_Users_UpdatedBy",
                table: "Permissions",
                column: "UpdatedBy",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ReconciliationBatches_Users_CreatedBy",
                table: "ReconciliationBatches",
                column: "CreatedBy",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ReconciliationBatches_Users_InitiatedByUserId",
                table: "ReconciliationBatches",
                column: "InitiatedByUserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ReconciliationBatches_Users_UpdatedBy",
                table: "ReconciliationBatches",
                column: "UpdatedBy",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ReconciliationRecords_Transactions_TransactionId",
                table: "ReconciliationRecords",
                column: "TransactionId",
                principalTable: "Transactions",
                principalColumn: "TransactionId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ReconciliationRecords_Users_CreatedBy",
                table: "ReconciliationRecords",
                column: "CreatedBy",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ReconciliationRecords_Users_ResolvedByUserId",
                table: "ReconciliationRecords",
                column: "ResolvedByUserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ReconciliationRecords_Users_UpdatedBy",
                table: "ReconciliationRecords",
                column: "UpdatedBy",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RefreshTokens_Users_UserId",
                table: "RefreshTokens",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Refunds_Transactions_OriginalTransactionId",
                table: "Refunds",
                column: "OriginalTransactionId",
                principalTable: "Transactions",
                principalColumn: "TransactionId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Refunds_Transactions_RefundTransactionId",
                table: "Refunds",
                column: "RefundTransactionId",
                principalTable: "Transactions",
                principalColumn: "TransactionId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Refunds_Users_ApprovedByUserId",
                table: "Refunds",
                column: "ApprovedByUserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Refunds_Users_CreatedBy",
                table: "Refunds",
                column: "CreatedBy",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Refunds_Users_RequestedByUserId",
                table: "Refunds",
                column: "RequestedByUserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Refunds_Users_UpdatedBy",
                table: "Refunds",
                column: "UpdatedBy",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ReportConfigurations_Users_CreatedBy",
                table: "ReportConfigurations",
                column: "CreatedBy",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ReportConfigurations_Users_OwnerUserId",
                table: "ReportConfigurations",
                column: "OwnerUserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ReportConfigurations_Users_UpdatedBy",
                table: "ReportConfigurations",
                column: "UpdatedBy",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RiskAssessments_Transactions_TransactionId",
                table: "RiskAssessments",
                column: "TransactionId",
                principalTable: "Transactions",
                principalColumn: "TransactionId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RiskAssessments_Users_CreatedBy",
                table: "RiskAssessments",
                column: "CreatedBy",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RiskAssessments_Users_UpdatedBy",
                table: "RiskAssessments",
                column: "UpdatedBy",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RolePermissions_Roles_RoleId",
                table: "RolePermissions",
                column: "RoleId",
                principalTable: "Roles",
                principalColumn: "RoleId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Roles_Users_CreatedBy",
                table: "Roles",
                column: "CreatedBy",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Roles_Users_UpdatedBy",
                table: "Roles",
                column: "UpdatedBy",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Branches_Users_CreatedBy",
                table: "Branches");

            migrationBuilder.DropForeignKey(
                name: "FK_Branches_Users_UpdatedBy",
                table: "Branches");

            migrationBuilder.DropForeignKey(
                name: "FK_Countries_Users_CreatedBy",
                table: "Countries");

            migrationBuilder.DropForeignKey(
                name: "FK_Countries_Users_UpdatedBy",
                table: "Countries");

            migrationBuilder.DropForeignKey(
                name: "FK_Employees_Users_CreatedBy",
                table: "Employees");

            migrationBuilder.DropForeignKey(
                name: "FK_Employees_Users_UpdatedBy",
                table: "Employees");

            migrationBuilder.DropForeignKey(
                name: "FK_Employees_Users_UserId",
                table: "Employees");

            migrationBuilder.DropForeignKey(
                name: "FK_Roles_Users_CreatedBy",
                table: "Roles");

            migrationBuilder.DropForeignKey(
                name: "FK_Roles_Users_UpdatedBy",
                table: "Roles");

            migrationBuilder.DropForeignKey(
                name: "FK_Employees_Branches_BranchId",
                table: "Employees");

            migrationBuilder.DropTable(
                name: "AccountHolders");

            migrationBuilder.DropTable(
                name: "Addresses");

            migrationBuilder.DropTable(
                name: "AIUsageLogs");

            migrationBuilder.DropTable(
                name: "AuditLogs");

            migrationBuilder.DropTable(
                name: "ComplianceRecords");

            migrationBuilder.DropTable(
                name: "Documents");

            migrationBuilder.DropTable(
                name: "InterestCalculations");

            migrationBuilder.DropTable(
                name: "JournalEntryLines");

            migrationBuilder.DropTable(
                name: "LoanRepayments");

            migrationBuilder.DropTable(
                name: "Notifications");

            migrationBuilder.DropTable(
                name: "PaymentGatewayLogs");

            migrationBuilder.DropTable(
                name: "ReconciliationRecords");

            migrationBuilder.DropTable(
                name: "RefreshTokens");

            migrationBuilder.DropTable(
                name: "Refunds");

            migrationBuilder.DropTable(
                name: "ReportConfigurations");

            migrationBuilder.DropTable(
                name: "RolePermissions");

            migrationBuilder.DropTable(
                name: "AIChatMessages");

            migrationBuilder.DropTable(
                name: "FraudAlerts");

            migrationBuilder.DropTable(
                name: "KYCDetails");

            migrationBuilder.DropTable(
                name: "JournalEntries");

            migrationBuilder.DropTable(
                name: "LoanSchedules");

            migrationBuilder.DropTable(
                name: "NotificationTemplates");

            migrationBuilder.DropTable(
                name: "PaymentRequests");

            migrationBuilder.DropTable(
                name: "ReconciliationBatches");

            migrationBuilder.DropTable(
                name: "Disputes");

            migrationBuilder.DropTable(
                name: "Permissions");

            migrationBuilder.DropTable(
                name: "AIConversations");

            migrationBuilder.DropTable(
                name: "FraudRules");

            migrationBuilder.DropTable(
                name: "RiskAssessments");

            migrationBuilder.DropTable(
                name: "Loans");

            migrationBuilder.DropTable(
                name: "Beneficiaries");

            migrationBuilder.DropTable(
                name: "CardTransactions");

            migrationBuilder.DropTable(
                name: "InterestRates");

            migrationBuilder.DropTable(
                name: "Cards");

            migrationBuilder.DropTable(
                name: "Transactions");

            migrationBuilder.DropTable(
                name: "Customers");

            migrationBuilder.DropTable(
                name: "Accounts");

            migrationBuilder.DropTable(
                name: "Fees");

            migrationBuilder.DropTable(
                name: "Currencies");

            migrationBuilder.DropTable(
                name: "AccountTypes");

            migrationBuilder.DropTable(
                name: "PaymentMethods");

            migrationBuilder.DropTable(
                name: "TransactionTypes");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Roles");

            migrationBuilder.DropTable(
                name: "Branches");

            migrationBuilder.DropTable(
                name: "Countries");

            migrationBuilder.DropTable(
                name: "Employees");
        }
    }
}
