--TABLE
USE RentingDB;

CREATE TABLE AdvertPhoto(
	PhotoId INT NOT NULL IDENTITY(1, 1),
	AdvertId INT NOT NULL,
	PublicId INT NOT NULL,
	ImageURL NVARCHAR(MAX) NOT NULL,
	AddingDate DATETIME NOT NULL DEFAULT GETDATE(),
	PRIMARY KEY(PhotoId),
	FOREIGN KEY(AdvertId) REFERENCES Advert(AdvertId)
)
GO

--TYPE
CREATE TYPE [dbo].[AdvertPhotoType] AS TABLE
(
	[PhotoId] INT NOT NULL IDENTITY(1, 1),
	[PublicId] INT NOT NULL,
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
		t1.ImageURL
	FROM
		dbo.AdvertPhoto t1
GO

--PROCEDURES

--AddPhoto ?
CREATE PROCEDURE [dbo].[AdvertPhoto_AddPhoto]
    @AdvertPhoto AdvertPhotoType READONLY,
    @AdvertId INT
AS
    MERGE INTO [dbo].[AdvertPhoto] TARGET
    USING(
        SELECT
            PhotoId,
            @AdvertId as AdvertId,
            PublicId,
            ImageURL
        FROM
            @AdvertPhoto
    )AS SOURCE
    ON
    (
        TARGET.PhotoId = SOURCE.PhotoId AND TARGET.AdvertId = SOURCE.AdvertId
    )
    WHEN NOT MATCHED BY TARGET THEN
        INSERT(
            [AdvertId],
            [PublicId],
            [ImageURL]
        )
        VALUES(
            SOURCE.[AdvertId],
            SOURCE.[PublicId],
            SOURCE.[ImageURL]
        );
    SELECT CAST(SCOPE_IDENTITY() AS INT);
GO

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
		[ImageURL]
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
		[ImageURL]
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