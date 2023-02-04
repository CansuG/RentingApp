USE RentingDB;

CREATE TABLE SavedAdvert(
	SavedAdvertId INT NOT NULL IDENTITY(1,1),
	SavingDate DATETIME NOT NULL DEFAULT GETDATE(),
	ApplicationUserId INT NOT NULL,
	AdvertId INT NOT NULL,
	PRIMARY KEY(SavedAdvertId),
	FOREIGN KEY(ApplicationUserId) REFERENCES Account(ApplicationUserId),
	FOREIGN KEY(AdvertId) REFERENCES Advert(AdvertId)
)

CREATE TYPE [dbo].[SavedAdvertType] AS TABLE
(
	[SavedAdvertId] INT NOT NULL,
	[ApplicationUserId] INT NOT NULL,
	[AdvertId] INT NOT NULL
)
GO

CREATE VIEW [aggregate].[SavedAdvert]
AS 
	SELECT 
		t1.SavedAdvertId,
		t1.SavingDate,
		t1.AdvertId,
		t2.ApplicationUserId,
		t2.Username,
		t3.Title,
		t3.Content,
		t3.PublishDate,
		t3.UpdateDate,
		t3.ActiveInd,
		t3.City,
		t3.District,
		t3.Neighbourhood,
		t3.Rooms,
		t3.Price,
		t3.FloorArea
	FROM
		dbo.SavedAdvert t1
	INNER JOIN
		dbo.Account t2 ON t1.ApplicationUserId = t2.ApplicationUserId
		dbo.Advert t3 ON t1.AdvertId = t3.AdvertId
GO

CREATE PROCEDURE [dbo].[SavedAdvert_SaveAdvert]
	@SavedAdvert SavedAdvertType READONLY
AS
	INSERT INTO [dbo].[SavedAdvert]
		(SavedAdvertId,
		ApplicationUserId,
		AdvertId)
	SELECT
		SavedAdvertId,
		ApplicationUserId,
		AdvertId
	FROM
		@SavedAdvert;
	SELECT CAST(SCOPE_IDENTITY() AS INT);
GO

CREATE PROCEDURE [dbo].[SavedAdvert_GetSavedAdvertByUserId]
	@ApplicationUserId INT
AS
	SELECT 
		[SavedAdvertId],
		[ApplicationUserId],
		[AdvertId]
	FROM 
		[aggregate].[SavedAdvert] t1
	WHERE 
		t1.ApplicationUserId = @ApplicationUserId AND 
		t1.[ActiveInd] = CONVERT(BIT, 1)
GO

CREATE PROCEDURE [dbo].[SavedAdvert_UnsaveAdvert]
    @SavedAdvertId INT        
AS 
BEGIN 
	SET NOCOUNT ON 
DELETE 
	FROM
		SavedAdvert
	WHERE
		SavedAdvertId = @SavedAdvertId

END
