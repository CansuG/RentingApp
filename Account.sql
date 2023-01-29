USE RentingDB;

CREATE TABLE Account (
	ApplicationUserId INT NOT NULL IDENTITY(1,1),
	Username NVARCHAR(20) NOT NULL,
	NormalizedUsername NVARCHAR(20) NOT NULL,
	Email VARCHAR(40) NOT NULL,
	NormalizedEmail VARCHAR(40) NOT NULL,
	Gender VARCHAR(20) NOT NULL,
	PasswordHash NVARCHAR(MAX) NOT NULL,
	FirstName NVARCHAR(20) NULL,
	LastName NVARCHAR(20) NULL,
	PRIMARY KEY(ApplicationUserId)
)

CREATE INDEX [IX_ApplicationUser_NormalizedUsername] ON [dbo].[Account] ([NormalizedUsername])

CREATE INDEX [IX_ApplicationUser_NormalizedEmail] ON [dbo].[Account] ([NormalizedEmail])

CREATE TYPE [dbo].[AccountType] AS TABLE
(
	[Username] VARCHAR(20) NOT NULL,
	[NormalizedUsername] VARCHAR(20) NOT NULL,
	[Email] VARCHAR(40) NOT NULL,
	[NormalizedEmail] VARCHAR(40) NOT NULL,
	[Gender] VARCHAR(20) NOT NULL,
	[PasswordHash] NVARCHAR(MAX) NOT NULL,
	[FirstName] NVARCHAR(20) NULL,
	[LastName] NVARCHAR(20) NULL,
	[PublicId] NVARCHAR(MAX) NULL,
	[ImageURL] NVARCHAR(MAX) NULL
)
GO

CREATE TYPE [dbo].[AccountUpdateType] AS TABLE
(
	[Username] VARCHAR(20) NOT NULL,
	[NormalizedUsername] VARCHAR(20) NOT NULL,
	[Gender] VARCHAR(20) NOT NULL,
	[FirstName] NVARCHAR(20) NULL,
	[LastName] NVARCHAR(20) NULL
)
GO

CREATE PROCEDURE [dbo].[Account_GetByUsername]
	@NormalizedUsername VARCHAR(20)
AS
	SELECT 
	*
	FROM
		Account t1
	WHERE 
		t1.NormalizedUsername = @NormalizedUsername
GO

CREATE PROCEDURE [dbo].[Account_GetByEmail]
	@NormalizedEmail VARCHAR(40)
AS
	SELECT
		*
	FROM
		Account t1
	WHERE 
		t1.NormalizedEmail = @NormalizedEmail
GO

CREATE PROCEDURE [dbo].[Account_GetByUserId]
	@ApplicationUserId INT
AS
	SELECT
		t1.ApplicationUserId,
		t1.Username,
		t1.Email,
		t1.Gender,
		t1.Firstname,
		t1.LastName,
		t1.PublicId,
		t1.ImageURL
	FROM
		Account t1
	WHERE 
		t1.ApplicationUserId = @ApplicationUserId
GO

CREATE PROCEDURE [dbo].[Account_GetEmails] 
AS
	SELECT Email FROM Account
GO

CREATE PROCEDURE [dbo].[Account_GetUsernames] 
AS
	SELECT Username FROM Account
GO

CREATE PROCEDURE [dbo].[Account_Insert]
	@Account AccountType READONLY
AS
	INSERT INTO [dbo].[Account]
           ([Username],
           [NormalizedUsername],
           [Email],
           [NormalizedEmail],
		   [Gender],
           [PasswordHash],
		   [FirstName],
		   [LastName],
		   [PublicId],
		   [ImageURL])
	SELECT 
		[Username],
        [NormalizedUsername],
		[Email],
        [NormalizedEmail],
		[Gender],
        [PasswordHash],
		[FirstName],
		[LastName],
		[PublicId],
		[ImageURL]
	FROM
		@Account;

	SELECT CAST(SCOPE_IDENTITY() AS INT);
GO

CREATE PROCEDURE [dbo].[Account_Update]
	@ApplicationUserId INT,
	@Username NVARCHAR(20),
	@NormalizedUsername NVARCHAR(20),
	@Gender VARCHAR(20),
	@FirstName NVARCHAR(20),
	@LastName NVARCHAR(20)
AS
	UPDATE dbo.Account
	SET
		Username = @Username,
		NormalizedUsername = @NormalizedUsername,
		Gender = @Gender,
		FirstName = @FirstName,
		LastName = @LastName
	WHERE ApplicationUserId = @ApplicationUserId
GO

select * from Account

ALTER TABLE dbo.Account
ADD PublicId NVARCHAR(MAX);

ALTER TABLE dbo.Account
ADD ImageURL NVARCHAR(MAX);


CREATE PROCEDURE [dbo].[Account_Update_Photo]
	@ApplicatİonUserId INT,
	@PublicId NVARCHAR(MAX),
	@ImageURL NVARCHAR(MAX)
AS
	UPDATE dbo.Account
	SET
		PublicId = @PublicId,
		ImageURL = @ImageURL
	WHERE ApplicationUserId = @ApplicationUserId
GO