-- =============================================
-- 006_Create_TReviewHistory.sql
-- =============================================
CREATE TABLE TReviewHistory
(
    Id          BIGINT IDENTITY(1,1) PRIMARY KEY,
    ReviewId    BIGINT         NOT NULL,
    UserId      BIGINT         NOT NULL,
    ActionCode  INT            NOT NULL,
    Rating      INT            NULL,

    Content     NVARCHAR(1000) NULL,
    StatusCode  INT            NOT NULL,
    CreatedAt   DATETIME2      NOT NULL DEFAULT GETDATE(),
    Memo        NVARCHAR(500)  NULL,

    CONSTRAINT FK_TReviewHistory_TReviews FOREIGN KEY (ReviewId)
                                          REFERENCES  TReviews(Id),

    CONSTRAINT FK_TReviewHistory_TUsers   FOREIGN KEY (UserId)
                                          REFERENCES TUsers(Id)
);