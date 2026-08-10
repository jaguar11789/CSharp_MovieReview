-- =============================================
-- 001_Create_TUsers.sql
-- =============================================

CREATE TABLE TUsers
(
    Id              BIGINT IDENTITY(1,1) PRIMARY KEY,
    UserId          NVARCHAR(50)  NOT NULL,
    PasswordHash    NVARCHAR(255) NULL,
    UserName        NVARCHAR(50)  NULL,
    Email           NVARCHAR(255) NULL,

    PhoneNumber     NVARCHAR(20)  NULL,
    Gender          NVARCHAR(10)  NULL,
    BirthDate       DATE          NULL,
    ZipCode         NVARCHAR(10)  NULL,
    BaseAddress     NVARCHAR(255) NULL,

    DetailAddress   NVARCHAR(255) NULL,
    Role            NVARCHAR(20)  NOT NULL CONSTRAINT DF_TUsers_Role DEFAULT 'User',
    StatusCode      INT           NOT NULL CONSTRAINT DF_TUsers_StatusCode DEFAULT 100,
    EmailVerified   BIT           NOT NULL CONSTRAINT DF_TUsers_EmailVerified DEFAULT 0,
    CreatedAt       DATETIME2     NOT NULL CONSTRAINT DF_TUsers_CreatedAt DEFAULT GETDATE(),

    UpdatedAt       DATETIME2     NOT NULL CONSTRAINT DF_TUsers_UpdatedAt DEFAULT GETDATE(),

    CONSTRAINT UQ_TUsers_UserId UNIQUE (UserId)
);