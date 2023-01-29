--TABLE
CREATE TABLE AdvertPhoto(
	PhotoId INT NOT NULL IDENTITY(1, 1),
	AdvertId INT NOT NULL,
	PublicId NVARCHAR(MAX) NOT NULL,
	ImageURL NVARCHAR(MAX) NOT NULL,
	AddingDate DATETIME NOT NULL DEFAULT GETDATE(),
	PRIMARY KEY(PhotoId),
	FOREIGN KEY(AdvertId) REFERENCES Advert(AdvertId)
)
GO

--TYPE
CREATE TYPE [dbo].[AdvertPhotoType] AS TABLE
(
	[PublicId] NVARCHAR(MAX) NOT NULL,
	[ImageURL] NVARCHAR(MAX) NOT NULL
)
GO

--VIEW
CREATE VIEW [aggregate].[AdvertPhoto]
AS 
	SELECT 
		t1.PhotoId,
		t1.AdvertId,
		t1.PublicId,
		t1.ImageURL,
		t1.AddingDate
	FROM
		dbo.AdvertPhoto t1
GO

--PROCEDURES


--DeletePhoto (BY PhotoId)
CREATE PROCEDURE [dbo].[AdvertPhoto_DeletePhotoById]
	@PhotoId int
AS
	DELETE FROM
		AdvertPhoto
	WHERE
		PhotoId = @PhotoId
GO

--GetPhoto (By PhotoId)
CREATE PROCEDURE [dbo].[AdvertPhoto_GetPhotoByPhotoId]
	@PhotoId INT
AS
	SELECT 
		[PhotoId],
		[AdvertId],
		[PublicId],
		[ImageURL],
		[AddingDate]
	FROM 
		[aggregate].[AdvertPhoto] t1
	WHERE
		t1.PhotoId = @PhotoId
GO

--GetPhoto (BY AdvertId)
CREATE PROCEDURE [dbo].[AdvertPhoto_GetPhotoByAdvertId]
	@AdvertId INT
AS
	SELECT 
		[PhotoId],
		[AdvertId],
		[PublicId],
		[ImageURL],
		[AddingDate]
	FROM 
		[aggregate].[AdvertPhoto] t1
	WHERE 
		t1.AdvertId = @AdvertId
GO

--ChechPhoto : Check photo num
--How to check? Ask
--We can add PhotoCount (Default 0) to the advert table
--By the time it becomes 10, AddPhoto method would be inactive
--If we cannot do that, frontend can prevent it too (temporarily)

select * from AdvertPhoto

USE RentingDB;

USE [RentingDB]
GO


CREATE PROCEDURE [dbo].[AdvertPhoto_AddPhoto]
    @AdvertPhoto AdvertPhotoType READONLY,
    @AdvertId INT
AS
	DECLARE @publicIdVariable NVARCHAR(MAX), @imageURLVariable NVARCHAR(MAX)
	SELECT @publicIdVariable = PublicId, @imageURLVariable = ImageURl FROM @AdvertPhoto

    INSERT INTO [dbo].[AdvertPhoto] (AdvertId, PublicId, ImageURL)
	VALUES (
		@AdvertId,
		@publicIdVariable,
		@imageURLVariable
	);

    SELECT CAST(SCOPE_IDENTITY() AS INT);

GO

