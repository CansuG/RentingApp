CREATE DATABASE RentingDB;
USE RentingDB;

CREATE TABLE Account (
	ApplicationUserId INT NOT NULL IDENTITY(1,1),
	Username NVARCHAR(20) NOT NULL,
	NormalizedUsername NVARCHAR(20) NOT NULL,
	Email VARCHAR(40) NOT NULL,
	NormalizedEmail VARCHAR(40) NOT NULL,
	Fullname NVARCHAR(30) NULL,
	Gender VARCHAR(20) NOT NULL,
	PasswordHash NVARCHAR(MAX) NOT NULL,
	PRIMARY KEY(ApplicationUserId)
)

CREATE INDEX [IX_ApplicationUser_NormalizedUsername] ON [dbo].[Account] ([NormalizedUsername])

CREATE INDEX [IX_ApplicationUser_NormalizedEmail] ON [dbo].[Account] ([NormalizedEmail])

CREATE TABLE Advert(
	AdvertId INT NOT NULL IDENTITY(1,1),
	ApplicationUserId INT NOT NULL,
	Title VARCHAR(50) NOT NULL,
	Content VARCHAR(MAX) NOT NULL,
	PublishDate DATETIME NOT NULL DEFAULT GETDATE(),
	UpdateDate DATETIME NOT NULL DEFAULT GETDATE(),
	ActiveInd BIT NOT NULL DEFAULT CONVERT(BIT,1),
	PRIMARY KEY(AdvertId),
	FOREIGN KEY(ApplicationUserId) REFERENCES Account(ApplicationUserId)
)

CREATE TYPE [dbo].[AccountType] AS TABLE
(
	[Username] VARCHAR(20) NOT NULL,
	[NormalizedUsername] VARCHAR(20) NOT NULL,
	[Email] VARCHAR(40) NOT NULL,
	[NormalizedEmail] VARCHAR(40) NOT NULL,
	[Fullname] VARCHAR(30) NULL,
	[Gender] VARCHAR(20) NOT NULL,
	[PasswordHash] NVARCHAR(MAX) NOT NULL
)
GO

CREATE TYPE [dbo].[AdvertType] AS TABLE
(
	[AdvertId] INT NOT NULL,
	[Title] NVARCHAR(50) NOT NULL,
	[Content] NVARCHAR(MAX) NULL,
	[City] NVARCHAR(30) NOT NULL,
	[District] NVARCHAR(30) NOT NULL,
	[Neighbourhood] NVARCHAR(30) NOT NULL,
	[Rooms] NVARCHAR(30) NOT NULL,
	[Price] NVARCHAR(30) NOT NULL,
	[FloorArea] NVARCHAR(30) NOT NULL
)
GO

CREATE SCHEMA [aggregate]

CREATE VIEW [aggregate].[Advert]
AS 
	SELECT 
		t1.AdvertId,
		t1.ApplicationUserId,
		t2.Username,
		t1.Title,
		t1.Content,
		t1.PublishDate,
		t1.UpdateDate,
		t1.ActiveInd,
		t1.City,
		t1.District,
		t1.Neighbourhood,
		t1.Rooms,
		t1.Price,
		t1.FloorArea
	FROM
		dbo.Advert t1
	INNER JOIN
		dbo.Account t2 ON t1.ApplicationUserId = t2.ApplicationUserId
GO

CREATE PROCEDURE [dbo].[Account_GetByUsername]
	@NormalizedUsername VARCHAR(20)
AS
	SELECT
		t1.ApplicationUserId,
		t1.Username,
		t1.NormalizedUsername,
		t1.Email,
		t1.NormalizedEmail,
		t1.Fullname,
		t1.Gender,
		t1.PasswordHash
	FROM
		Account t1
	WHERE 
		t1.NormalizedUsername = @NormalizedUsername
GO

CREATE PROCEDURE [dbo].[Account_GetByEmail]
	@NormalizedEmail VARCHAR(40)
AS
	SELECT
		t1.ApplicationUserId,
		t1.Username,
		t1.NormalizedUsername,
		t1.Email,
		t1.NormalizedEmail,
		t1.Fullname,
		t1.Gender,
		t1.PasswordHash
	FROM
		Account t1
	WHERE 
		t1.NormalizedEmail = @NormalizedEmail
GO

CREATE PROCEDURE [dbo].[Account_Insert]
	@Account AccountType READONLY
AS
	INSERT INTO [dbo].[Account]
           ([Username],
           [NormalizedUsername],
           [Email],
           [NormalizedEmail],
           [Fullname],
		   [Gender],
           [PasswordHash])
	SELECT 
		[Username],
        [NormalizedUsername],
		[Email],
        [NormalizedEmail],
        [Fullname],
		[Gender],
        [PasswordHash]
	FROM
		@Account;

	SELECT CAST(SCOPE_IDENTITY() AS INT);
GO

CREATE PROCEDURE [dbo].[Advert_Get]
	@AdvertId INT
AS
	SELECT 
		[AdvertId],
		[ApplicationUserId],
		[Username],
		[Title],
		[Content],
		[PublishDate],
		[UpdateDate],
		[City],
		[District],
		[Neighbourhood],
		[Rooms],
		[Price],
		[FloorArea]
	FROM 
		[aggregate].[Advert] t1
	WHERE
		t1.[AdvertId] = @AdvertId AND 
		t1.ActiveInd = CONVERT(BIT, 1)
GO

CREATE PROCEDURE [dbo].[Advert_GetByUserId]
	@ApplicationUserId INT
AS
	SELECT 
		[AdvertId],
		[ApplicationUserId],
		[Username],
		[Title],
		[Content],
		[PublishDate],
		[UpdateDate],
		[City],
		[District],
		[Neighbourhood],
		[Rooms],
		[Price],
		[FloorArea]
	FROM 
		[aggregate].[Advert] t1
	WHERE 
		t1.ApplicationUserId = @ApplicationUserId AND 
		t1.[ActiveInd] = CONVERT(BIT, 1)
GO

CREATE PROCEDURE [dbo].[Advert_Upsert]
	@Advert AdvertType READONLY,
	@ApplicationUserId INT
AS
	MERGE INTO [dbo].[Advert] TARGET
	USING(
		SELECT
			AdvertId,
			@ApplicationUserId [ApplicationUserId],
			Title,
			Content,
			City,
			District,
			Neighbourhood,
			Rooms,
			Price,
			FloorArea
		FROM
			@Advert
	)AS SOURCE
	ON
	(
		TARGET.AdvertId = SOURCE.AdvertId AND TARGET.ApplicationUserId = SOURCE.ApplicationUserId
	)
	WHEN MATCHED THEN
		UPDATE SET
			TARGET.[Title] = SOURCE.[Title],
			TARGET.[Content] = SOURCE.[Content],
			TARGET.[UpdateDate] = GETDATE(),
			TARGET.[City] = SOURCE.[City],
			TARGET.[District] = SOURCE.[District],
			TARGET.[Neighbourhood] = SOURCE.[Neighbourhood],
			TARGET.[Rooms] = SOURCE.[Rooms],
			TARGET.[Price] = SOURCE.[Price],
			TARGET.[FloorArea] = SOURCE.[FloorArea]
		
	WHEN NOT MATCHED BY TARGET THEN 
		INSERT(
			[ApplicationUserId],
			[Title],
			[Content],
			[City],
			[District],
			[Neighbourhood],
			[Rooms],
			[Price],
			[FloorArea]
		)
		VALUES (
			SOURCE.[ApplicationUserId],
			SOURCE.[Title],
			SOURCE.[Content],
			SOURCE.[City],
			SOURCE.[District],
			SOURCE.[Neighbourhood],
			SOURCE.[Rooms],
			SOURCE.[Price],
			SOURCE.[FloorArea]
		);
	SELECT CAST(SCOPE_IDENTITY() AS INT);
GO

CREATE PROCEDURE [dbo].[Advert_Delete]
	@AdvertId INT 
AS
	UPDATE [dbo].[Advert]
	SET 
		[ActiveInd] = CONVERT(BIT, 0),
		[UpdateDate] = GETDATE()
	WHERE 
		[AdvertId] = @AdvertId;
GO

ALTER TABLE dbo.Advert
ADD City NVARCHAR(30) NOT NULL;

ALTER TABLE dbo.Advert
ADD District NVARCHAR(30) NOT NULL;

ALTER TABLE dbo.Advert
ADD Neighbourhood NVARCHAR(30) NOT NULL;

ALTER TABLE dbo.Advert
ADD Rooms NVARCHAR(30) NOT NULL;

ALTER TABLE dbo.Advert
ADD Price NVARCHAR(30) NOT NULL;

ALTER TABLE dbo.Advert
ADD FloorArea NVARCHAR(30) NOT NULL;

ALTER TABLE dbo.Advert
ALTER COLUMN Content NVARCHAR(MAX) NULL;