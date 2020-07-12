
USE master
GO

IF EXISTS
(
    SELECT NAME
    FROM Sys.Databases
    WHERE Name = N'AuthorizationTests'
)
BEGIN
    DROP DATABASE [AuthorizationTests]
END
GO

CREATE DATABASE [AuthorizationTests]
GO

USE [AuthorizationTests]
GO

CREATE SCHEMA [Logs];
GO

CREATE SCHEMA [AccessControl];
GO

CREATE TABLE [AuthorizationTests].[Logs].[ApplicationLog]
(
    [ApplicationLogId] INT NOT NULL IDENTITY,
    [Type] INT NOT NULL,
    [UserId] VARCHAR(50),
    [Source] VARCHAR(256),
    [Message] VARCHAR(1024) NOT NULL,
    [Data] VARCHAR(1024),
    [Url] VARCHAR(512),
    [StackTrace] VARCHAR(2048),
    [HostIpAddress] VARCHAR(25),
    [UserIpAddress] VARCHAR(25),
    [UserAgent] VARCHAR(25),
    [When] DATETIME NOT NULL DEFAULT GETDATE()
    CONSTRAINT ApplicationLog_PK PRIMARY KEY ([ApplicationLogId])
);
GO

CREATE TABLE [AuthorizationTests].[AccessControl].[IdentityProvider]
(
    [IdentityProviderId] INT NOT NULL IDENTITY,
    [Name] VARCHAR(60) NOT NULL,
    [Uri] VARCHAR(512) NOT NULL,
    [CreatedBy] INT NOT NULL,
    [CreatedDateTime] DATETIME NOT NULL DEFAULT GETDATE(),
    [UpdatedBy] INT,
    [UpdatedDateTime] DATETIME
    CONSTRAINT IdentityProvider_PK PRIMARY KEY ([IdentityProviderId])
);
GO

CREATE TABLE [AuthorizationTests].[AccessControl].[User]
(
    [UserId] INT NOT NULL IDENTITY,
    [Email] NVARCHAR(256) NOT NULL,
    [NormalizedEmail] NVARCHAR(256) NOT NULL
    CONSTRAINT User_PK PRIMARY KEY ([UserId])
);
GO

CREATE INDEX [NormalizedEmailIndex]
    ON [AuthorizationTests].[AccessControl].[User]
    (
        [NormalizedEmail]
    );
GO

CREATE TABLE [AuthorizationTests].[AccessControl].[User_UserLogins]
(
    [UserId] INT NOT NULL,
    [Provider] VARCHAR(128) NOT NULL,
    [UserKey] VARCHAR(128) NOT NULL
);
GO

CREATE TABLE [AuthorizationTests].[AccessControl].[Role]
(
    [RoleId] INT NOT NULL IDENTITY,
    [Name] VARCHAR(256)
    CONSTRAINT Role_PK PRIMARY KEY ([RoleId])
);
GO

CREATE TABLE [AuthorizationTests].[AccessControl].[UserRole]
(
    [RoleId] INT NOT NULL,
    [UserId] INT NOT NULL,
    [CreatedBy] INT NOT NULL,
    [CreatedDateTime] DATETIME NOT NULL DEFAULT GETDATE(),
    [UpdatedBy] INT,
    [UpdatedDateTime] DATETIME
    CONSTRAINT UserRole_PK PRIMARY KEY ([RoleId], [UserId])
);
GO

ALTER TABLE [AuthorizationTests].[AccessControl].[User_UserLogins]
    ADD CONSTRAINT User_UserLogins_FK FOREIGN KEY ([UserId])
        REFERENCES [AuthorizationTests].[AccessControl].[User] ([UserId])
        ON UPDATE NO ACTION
        ON DELETE CASCADE;
GO

CREATE INDEX [User_UserLogins_FK_IX]
    ON [AuthorizationTests].[AccessControl].[User_UserLogins]
    (
        [UserId]
    );
GO

CREATE PROCEDURE [Logs].[pApplicationLog_Delete]
    @applicationLogId INT
AS
BEGIN
    DELETE FROM [AuthorizationTests].[Logs].[ApplicationLog]
    WHERE [ApplicationLogId] = @applicationLogId;

END;
GO

CREATE PROCEDURE [Logs].[pApplicationLog_DeleteOlderLogs]
    @when DATETIME
AS
BEGIN
    DELETE FROM [AuthorizationTests].[Logs].[ApplicationLog]
    WHERE [When] < @when;

END;
GO

CREATE PROCEDURE [Logs].[pApplicationLog_Insert]
    @type INT,
    @userId VARCHAR(50) = NULL,
    @source VARCHAR(256) = NULL,
    @message VARCHAR(1024),
    @data VARCHAR(1024) = NULL,
    @url VARCHAR(512) = NULL,
    @stackTrace VARCHAR(2048) = NULL,
    @hostIpAddress VARCHAR(25) = NULL,
    @userIpAddress VARCHAR(25) = NULL,
    @userAgent VARCHAR(25) = NULL
AS
BEGIN
    DECLARE @applicationLogOutputData TABLE
    (
        [ApplicationLogId] INT,
        [When] DATETIME
    );

    INSERT INTO [AuthorizationTests].[Logs].[ApplicationLog]
    (
        [Type],
        [UserId],
        [Source],
        [Message],
        [Data],
        [Url],
        [StackTrace],
        [HostIpAddress],
        [UserIpAddress],
        [UserAgent]
    )
    OUTPUT
        INSERTED.[ApplicationLogId],
        INSERTED.[When]
        INTO @applicationLogOutputData
    VALUES
    (
        @type,
        @userId,
        @source,
        @message,
        @data,
        @url,
        @stackTrace,
        @hostIpAddress,
        @userIpAddress,
        @userAgent
    );

    SELECT
        [ApplicationLogId],
        [When]
    FROM @applicationLogOutputData;

END;
GO

CREATE PROCEDURE [AccessControl].[pIdentityProvider_Delete]
    @identityProviderId INT
AS
BEGIN
    DELETE FROM [AuthorizationTests].[AccessControl].[IdentityProvider]
    WHERE [IdentityProviderId] = @identityProviderId;

END;
GO

CREATE PROCEDURE [AccessControl].[pIdentityProvider_Insert]
    @name VARCHAR(60),
    @uri VARCHAR(512),
    @createdBy INT
AS
BEGIN
    DECLARE @identityProviderOutputData TABLE
    (
        [IdentityProviderId] INT
    );

    INSERT INTO [AuthorizationTests].[AccessControl].[IdentityProvider]
    (
        [Name],
        [Uri],
        [CreatedBy]
    )
    OUTPUT
        INSERTED.[IdentityProviderId]
        INTO @identityProviderOutputData
    VALUES
    (
        @name,
        @uri,
        @createdBy
    );

    SELECT
        [IdentityProviderId]
    FROM @identityProviderOutputData;

END;
GO

CREATE PROCEDURE [AccessControl].[pRole_Delete]
    @roleId INT
AS
BEGIN
    DELETE FROM [AuthorizationTests].[AccessControl].[Role]
    WHERE [RoleId] = @roleId;

END;
GO

CREATE PROCEDURE [AccessControl].[pRole_Insert]
    @name VARCHAR(256) = NULL
AS
BEGIN
    DECLARE @roleOutputData TABLE
    (
        [RoleId] INT
    );

    INSERT INTO [AuthorizationTests].[AccessControl].[Role]
    (
        [Name]
    )
    OUTPUT
        INSERTED.[RoleId]
        INTO @roleOutputData
    VALUES
    (
        @name
    );

    SELECT
        [RoleId]
    FROM @roleOutputData;

END;
GO

CREATE PROCEDURE [AccessControl].[pRole_AddUsers]
    @roleId INT,
    @userId INT,
    @createdBy INT
AS
BEGIN
    INSERT INTO [AuthorizationTests].[AccessControl].[UserRole]
    (
        [RoleId],
        [UserId],
        [CreatedBy]
    )
    VALUES
    (
        @roleId,
        @userId,
        @createdBy
    );

END;
GO

CREATE PROCEDURE [AccessControl].[pRole_LinkUsers]
    @roleId INT,
    @userId INT,
    @createdBy INT
AS
BEGIN
    INSERT INTO [AuthorizationTests].[AccessControl].[UserRole]
    (
        [RoleId],
        [UserId],
        [CreatedBy]
    )
    VALUES
    (
        @roleId,
        @userId,
        @createdBy
    );

END;
GO

CREATE PROCEDURE [AccessControl].[pRole_UnlinkUsers]
    @roleId INT
AS
BEGIN
    DELETE FROM [AuthorizationTests].[AccessControl].[UserRole]
    WHERE [RoleId] = @roleId;

END;
GO

CREATE PROCEDURE [AccessControl].[pUser_Delete]
    @userId INT
AS
BEGIN

    EXECUTE [dbo].[SetUserContext] @userId;

    DELETE FROM [AuthorizationTests].[AccessControl].[User]
    WHERE [UserId] = @userId;

END;
GO

CREATE PROCEDURE [AccessControl].[pUser_Insert]
    @email NVARCHAR(256),
    @normalizedEmail NVARCHAR(256)
AS
BEGIN
    DECLARE @userOutputData TABLE
    (
        [UserId] INT
    );

    INSERT INTO [AuthorizationTests].[AccessControl].[User]
    (
        [Email],
        [NormalizedEmail]
    )
    OUTPUT
        INSERTED.[UserId]
        INTO @userOutputData
    VALUES
    (
        @email,
        @normalizedEmail
    );

    SELECT
        [UserId]
    FROM @userOutputData;

END;
GO

CREATE PROCEDURE [AccessControl].[pUser_AddRoles]
    @roleId INT,
    @userId INT,
    @createdBy INT
AS
BEGIN
    INSERT INTO [AuthorizationTests].[AccessControl].[UserRole]
    (
        [RoleId],
        [UserId],
        [CreatedBy]
    )
    VALUES
    (
        @roleId,
        @userId,
        @createdBy
    );

END;
GO

CREATE PROCEDURE [AccessControl].[pUser_LinkRoles]
    @roleId INT,
    @userId INT,
    @createdBy INT
AS
BEGIN
    INSERT INTO [AuthorizationTests].[AccessControl].[UserRole]
    (
        [RoleId],
        [UserId],
        [CreatedBy]
    )
    VALUES
    (
        @roleId,
        @userId,
        @createdBy
    );

END;
GO

CREATE PROCEDURE [AccessControl].[pUser_UnlinkRoles]
    @userId INT
AS
BEGIN

    EXECUTE [dbo].[SetUserContext] @userId;

    DELETE FROM [AuthorizationTests].[AccessControl].[UserRole]
    WHERE [UserId] = @userId;

END;
GO

CREATE PROCEDURE [AccessControl].[pUser_AddUserLogins]
    @userId INT,
    @provider VARCHAR(128),
    @userKey VARCHAR(128)
AS
BEGIN
    INSERT INTO [AuthorizationTests].[AccessControl].[User_UserLogins]
    (
        [UserId],
        [Provider],
        [UserKey]
    )
    VALUES
    (
        @userId,
        @provider,
        @userKey
    );

END;
GO

CREATE PROCEDURE [AccessControl].[pUser_DeleteUserLogins]
    @userId INT
AS
BEGIN

    EXECUTE [dbo].[SetUserContext] @userId;

    DELETE FROM [AuthorizationTests].[AccessControl].[User_UserLogins]
    WHERE [UserId] = @userId;

END;
GO

CREATE PROCEDURE [AccessControl].[pUserRole_Delete]
    @roleId INT,
    @userId INT
AS
BEGIN

    EXECUTE [dbo].[SetUserContext] @userId;

    DELETE FROM [AuthorizationTests].[AccessControl].[UserRole]
    WHERE [RoleId] = @roleId
    AND [UserId] = @userId;

END;
GO

CREATE PROCEDURE [AccessControl].[pUserRole_Insert]
    @roleId INT,
    @userId INT,
    @createdBy INT
AS
BEGIN
    INSERT INTO [AuthorizationTests].[AccessControl].[UserRole]
    (
        [RoleId],
        [UserId],
        [CreatedBy]
    )
    VALUES
    (
        @roleId,
        @userId,
        @createdBy
    );

END;
GO

CREATE PROCEDURE [AccessControl].[pUserRole_Update]
    @roleId INT,
    @userId INT,
    @updatedBy INT = NULL
AS
BEGIN
    UPDATE [AuthorizationTests].[AccessControl].[UserRole]
    SET
        [UpdatedBy] = @updatedBy,
        [UpdatedDateTime] = GETDATE()
    WHERE [RoleId] = @roleId
    AND [UserId] = @userId;

END;
GO

CREATE PROCEDURE [Logs].[pApplicationLog_Get]
    @$select NVARCHAR(MAX) = NULL,
    @$filter NVARCHAR(MAX) = NULL,
    @$orderby NVARCHAR(MAX) = NULL,
    @$skip NVARCHAR(10) = NULL,
    @$top NVARCHAR(10) = NULL,
    @count INT OUTPUT
AS
BEGIN
    EXEC [dbo].[pExecuteDynamicQuery]
        @$select = @$select,
        @$filter = @$filter,
        @$orderby = @$orderby,
        @$skip = @$skip,
        @$top = @$top,
        @selectList = N'    a.[ApplicationLogId] AS "Id",
    a.[Type] AS "Type",
    a.[UserId] AS "UserId",
    a.[Source] AS "Source",
    a.[Message] AS "Message",
    a.[Data] AS "Data",
    a.[Url] AS "Url",
    a.[StackTrace] AS "StackTrace",
    a.[HostIpAddress] AS "HostIpAddress",
    a.[UserIpAddress] AS "UserIpAddress",
    a.[UserAgent] AS "UserAgent",
    a.[When] AS "When"',
        @from = N'[AuthorizationTests].[Logs].[ApplicationLog] a',
        @count = @count OUTPUT

END;
GO

CREATE PROCEDURE [Logs].[pApplicationLog_GetById]
    @applicationLogId INT
AS
BEGIN
    SELECT
        a.[ApplicationLogId] AS "Id",
        a.[Type] AS "Type",
        a.[UserId] AS "UserId",
        a.[Source] AS "Source",
        a.[Message] AS "Message",
        a.[Data] AS "Data",
        a.[Url] AS "Url",
        a.[StackTrace] AS "StackTrace",
        a.[HostIpAddress] AS "HostIpAddress",
        a.[UserIpAddress] AS "UserIpAddress",
        a.[UserAgent] AS "UserAgent",
        a.[When] AS "When"
    FROM [AuthorizationTests].[Logs].[ApplicationLog] a
    WHERE a.[ApplicationLogId] = @applicationLogId;

END;
GO

CREATE PROCEDURE [AccessControl].[pIdentityProvider_GetById]
    @identityProviderId INT
AS
BEGIN
    SELECT
        i.[IdentityProviderId] AS "Id",
        i.[Name] AS "Name",
        i.[Uri] AS "Uri"
    FROM [AuthorizationTests].[AccessControl].[IdentityProvider] i
    WHERE i.[IdentityProviderId] = @identityProviderId;

END;
GO

CREATE PROCEDURE [AccessControl].[pRole_GetById]
    @roleId INT
AS
BEGIN
    SELECT
        r.[RoleId] AS "Id",
        r.[Name] AS "Name"
    FROM [AuthorizationTests].[AccessControl].[Role] r
    WHERE r.[RoleId] = @roleId;

END;
GO

CREATE PROCEDURE [AccessControl].[pUser_GetAllRoles]
    @userId INT
AS
BEGIN
    SELECT
        r.[RoleId] AS "Id",
        r.[Name] AS "Name"
    FROM [AuthorizationTests].[AccessControl].[Role] r
    INNER JOIN [AuthorizationTests].[AccessControl].[UserRole] u
        ON r.[RoleId] = u.[RoleId]
    WHERE u.[UserId] = @userId;

END;
GO

CREATE PROCEDURE [AccessControl].[pUser_Get]
    @$select NVARCHAR(MAX) = NULL,
    @$filter NVARCHAR(MAX) = NULL,
    @$orderby NVARCHAR(MAX) = NULL,
    @$skip NVARCHAR(10) = NULL,
    @$top NVARCHAR(10) = NULL,
    @count INT OUTPUT
AS
BEGIN
    EXEC [dbo].[pExecuteDynamicQuery]
        @$select = @$select,
        @$filter = @$filter,
        @$orderby = @$orderby,
        @$skip = @$skip,
        @$top = @$top,
        @selectList = N'    u.[UserId] AS "Id",
    u.[Email] AS "Email",
    u.[NormalizedEmail] AS "NormalizedEmail"',
        @from = N'[AuthorizationTests].[AccessControl].[User] u',
        @count = @count OUTPUT

END;
GO

CREATE PROCEDURE [AccessControl].[pUser_GetByNormalizedEmail]
    @email NVARCHAR(256)
AS
BEGIN
    SELECT
        u.[UserId] AS "Id",
        u.[Email] AS "Email",
        u.[NormalizedEmail] AS "NormalizedEmail"
    FROM [AuthorizationTests].[AccessControl].[User] u
    WHERE u.[NormalizedEmail] = UPPER(@email);

END;
GO

CREATE PROCEDURE [AccessControl].[pUser_GetByUserLogin]
    @provider VARCHAR(128),
    @userKey VARCHAR(128)
AS
BEGIN
    SELECT
        u.[UserId] AS "Id",
        u.[Email] AS "Email",
        u.[NormalizedEmail] AS "NormalizedEmail"
    FROM [AuthorizationTests].[AccessControl].[User] u
    INNER JOIN [AuthorizationTests].[AccessControl].[User_UserLogins] ul
        ON u.[UserId] = ul.[UserId]
    WHERE ul.[Provider] = @provider
    AND ul.[UserKey] = @userKey;

END;
GO

CREATE PROCEDURE [AccessControl].[pRole_GetAllUsers]
    @roleId INT
AS
BEGIN
    SELECT
        u.[UserId] AS "Id",
        u.[Email] AS "Email",
        u.[NormalizedEmail] AS "NormalizedEmail"
    FROM [AuthorizationTests].[AccessControl].[User] u
    INNER JOIN [AuthorizationTests].[AccessControl].[UserRole] u1
        ON u.[UserId] = u1.[UserId]
    WHERE u1.[RoleId] = @roleId;

END;
GO

CREATE PROCEDURE [AccessControl].[pRole_GetUsers]
    @roleId INT,
    @$select NVARCHAR(MAX) = NULL,
    @$filter NVARCHAR(MAX) = NULL,
    @$orderby NVARCHAR(MAX) = NULL,
    @$skip NVARCHAR(10) = NULL,
    @$top NVARCHAR(10) = NULL,
    @count INT OUTPUT
AS
BEGIN
    IF @$filter IS NULL
    BEGIN
        SET @$filter = N'u1.[RoleId] = ' + CAST(@roleId AS NVARCHAR(10));
    END
    ELSE
    BEGIN
        SET @$filter = N'(' + N'u1.[RoleId] = ' + CAST(@roleId AS NVARCHAR(10)) + N') AND (' + @$filter + N')';
    END;

    EXEC [dbo].[pExecuteDynamicQuery]
        @$select = @$select,
        @$filter = @$filter,
        @$orderby = @$orderby,
        @$skip = @$skip,
        @$top = @$top,
        @selectList = N'    u.[UserId] AS "Id",
    u.[Email] AS "Email",
    u.[NormalizedEmail] AS "NormalizedEmail"',
        @from = N'[AuthorizationTests].[AccessControl].[User] u
INNER JOIN [AuthorizationTests].[AccessControl].[UserRole] u1
    ON u.[UserId] = u1.[UserId]',
        @count = @count OUTPUT

END;
GO

CREATE PROCEDURE [AccessControl].[pUser_GetUserLogins]
    @userId INT,
    @$select NVARCHAR(MAX) = NULL,
    @$filter NVARCHAR(MAX) = NULL,
    @$orderby NVARCHAR(MAX) = NULL,
    @$skip NVARCHAR(10) = NULL,
    @$top NVARCHAR(10) = NULL,
    @count INT OUTPUT
AS
BEGIN
    IF @$filter IS NULL
    BEGIN
        SET @$filter = N'u.[UserId] = ' + CAST(@userId AS NVARCHAR(10));
    END
    ELSE
    BEGIN
        SET @$filter = N'(' + N'u.[UserId] = ' + CAST(@userId AS NVARCHAR(10)) + N') AND (' + @$filter + N')';
    END;

    EXEC [dbo].[pExecuteDynamicQuery]
        @$select = @$select,
        @$filter = @$filter,
        @$orderby = @$orderby,
        @$skip = @$skip,
        @$top = @$top,
        @selectList = N'    u.[Provider] AS "Provider",
    u.[UserKey] AS "UserKey"',
        @from = N'[AuthorizationTests].[AccessControl].[User_UserLogins] u',
        @count = @count OUTPUT

END;
GO

CREATE PROCEDURE [AccessControl].[pUser_GetAllUserLogins]
    @userId INT
AS
BEGIN
    SELECT
        u.[Provider] AS "Provider",
        u.[UserKey] AS "UserKey"
    FROM [AuthorizationTests].[AccessControl].[User_UserLogins] u
    WHERE u.[UserId] = @userId;

END;
GO

CREATE PROCEDURE [pExecuteDynamicQuery]
	@$select NVARCHAR(MAX) = NULL,
	@$filter NVARCHAR(MAX) = NULL,
	@$orderby NVARCHAR(MAX) = NULL,
	@$skip NVARCHAR(10) = NULL,
	@$top NVARCHAR(10) = NULL,
	@selectList NVARCHAR(MAX),
	@from NVARCHAR(MAX),
	@count INT OUTPUT
AS
BEGIN

	DECLARE @sqlCommand NVARCHAR(MAX);
	DECLARE @paramDefinition NVARCHAR(100);

	SET @paramDefinition = N'@cnt INT OUTPUT'

	SET @sqlCommand = 
'
	SELECT
		 @cnt = COUNT(1)
	FROM ' + @from + '
';

	IF @$filter IS NOT NULL
	BEGIN 
		SET @sqlCommand = @sqlCommand + 
' 
	WHERE ' + @$filter;
	END

	SET @sqlCommand = @sqlCommand + 
'
	SELECT
	';

	IF ISNULL(@$select, '*') = '*'
	BEGIN
		SET @sqlCommand = @sqlCommand + @selectList;
	END
	ELSE
	BEGIN
		SET @sqlCommand = @sqlCommand + @$select;
	END

	SET @sqlCommand = @sqlCommand +
'
	FROM ' + @from + '
';

	IF @$filter IS NOT NULL
	BEGIN 
		SET @sqlCommand = @sqlCommand + 
' 
	WHERE ' + @$filter;
	END

	IF @$orderby IS NOT NULL
	BEGIN 
		SET @sqlCommand = @sqlCommand + 
' 
	ORDER BY ' + @$orderby;
	END
	ELSE
	BEGIN

	-- At least a dummy order by is required is $skip and $top are provided
		IF @$skip IS NOT NULL OR @$top IS NOT NULL
		BEGIN  
			SET @sqlCommand = @sqlCommand + 
' 
	ORDER BY 1 ASC';
		END
	END

	IF @$skip IS NOT NULL
	BEGIN 
		SET @sqlCommand = @sqlCommand + 
' 
	OFFSET ' + @$skip + ' ROWS';
	END

	IF @$top IS NOT NULL
	BEGIN 
		SET @sqlCommand = @sqlCommand + 
' 
	FETCH NEXT ' + @$top + ' ROWS ONLY';
	END

	EXECUTE sp_executesql @sqlCommand, @paramDefinition, @cnt = @count OUTPUT

END;
GO

