-- =============================================
-- 001_Create_TReviews.sql
-- =============================================
CREATE TABLE TReviews
(
    Id          BIGINT IDENTITY(1,1) PRIMARY KEY,
    UserId      BIGINT         NOT NULL,
    MovieId     BIGINT         NOT NULL,
    Rating      INT            NOT NULL,
    Content     NVARCHAR(1000) NOT NULL,

    StatusCode  INT            NOT NULL DEFAULT 0,
    CreatedAt   DATETIME2      NOT NULL DEFAULT GETDATE(),
    UpdatedAt   DATETIME2      NULL,

    CONSTRAINT FK_TReviews_TUsers FOREIGN KEY (UserId)
                                  REFERENCES  TUsers(Id),

    CONSTRAINT CK_TReviews_Rating CHECK (Rating BETWEEN 1 AND 5)
);