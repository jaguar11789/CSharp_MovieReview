-- =============================================
-- 003_Create_TEmailVerifications.sql
-- =============================================

CREATE TABLE TEmailVerifications
(
    Id               BIGINT IDENTITY(1,1) PRIMARY KEY,
    UserId           BIGINT        NOT NULL,
    Email            NVARCHAR(255) NOT NULL,
    VerificationCode VARCHAR(6)    NOT NULL,
    ExpiresAt        DATETIME2     NOT NULL,

    VerifiedAt       DATETIME2     NULL,
    CreatedAt        DATETIME2     NOT NULL
        CONSTRAINT DF_TEmailVerifications_CreatedAt
        DEFAULT GETDATE(),

    CONSTRAINT FK_TEmailVerifications_TUsers
        FOREIGN KEY (UserId)
        REFERENCES TUsers(Id)
);