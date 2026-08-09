-- =============================================
-- 003_Create_TUserHistory.sql
-- =============================================

CREATE TABLE TUserHistory
(
    Id          BIGINT IDENTITY(1,1) PRIMARY KEY,
    UserId      BIGINT        NOT NULL,
    ActionCode  INT           NOT NULL,
    StatusCode  INT           NOT NULL,
    ChangedAt   DATETIME2     NOT NULL CONSTRAINT DF_TUserHistory_ChangedAt DEFAULT GETDATE(),

    Memo        NVARCHAR(500) NULL,

    CONSTRAINT FK_TUserHistory_TUsers FOREIGN KEY (UserId) REFERENCES TUsers(Id)
);