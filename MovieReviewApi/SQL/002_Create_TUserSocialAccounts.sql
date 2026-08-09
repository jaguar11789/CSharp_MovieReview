-- =============================================
-- 002_Create_TUserSocialAccounts.sql
-- =============================================

CREATE TABLE TUserSocialAccounts
(
    Id              BIGINT IDENTITY(1,1) PRIMARY KEY,
    UserId          BIGINT        NOT NULL,
    Provider        NVARCHAR(20)  NOT NULL,
    ProviderUserId  NVARCHAR(255) NOT NULL,
    ProfileImageUrl NVARCHAR(500) NULL,

    CreatedAt       DATETIME2     NOT NULL CONSTRAINT DF_TUserSocialAccounts_CreatedAt DEFAULT GETDATE(),

    CONSTRAINT FK_TUserSocialAccounts_TUsers FOREIGN KEY (UserId) REFERENCES TUsers(Id),
    CONSTRAINT UQ_TUserSocialAccounts_Provider_User UNIQUE (Provider, ProviderUserId)
);