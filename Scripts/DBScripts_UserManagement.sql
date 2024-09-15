IF NOT EXISTS(SELECT * FROM sys.databases WHERE name = 'UserManagementDB')
  BEGIN
    CREATE DATABASE UserManagementDB
END
GO

USE UserManagementDB

-- User
IF NOT EXISTS (
    SELECT 1
    FROM sys.objects
    WHERE object_id = OBJECT_ID('dbo.Users')
      AND type = 'U'  -- 'U' stands for user-defined tables
)
BEGIN
    CREATE TABLE [dbo].[Users](
	[Id] [int] PRIMARY KEY IDENTITY(1,1) NOT NULL,
	[Username] [nvarchar](max) NOT NULL,
	[Password] [nvarchar](max) NOT NULL,
	[CreatedBy] [nvarchar](max) NOT NULL,
	[CreatedDate] [datetime2](7) NOT NULL,
	[Email] [nvarchar](max) NOT NULL,
	[FirstName] [nvarchar](max) NOT NULL,
	[IsActive] [bit] NOT NULL,
	[LastName] [nvarchar](max) NOT NULL,
	[UpdatedBy] [nvarchar](max) NOT NULL,
	[UpdatedDate] [datetime2](7) NOT NULL
	)
END
GO

-- Roles
IF NOT EXISTS (
    SELECT 1
    FROM sys.objects
    WHERE object_id = OBJECT_ID('dbo.Roles')
      AND type = 'U'  -- 'U' stands for user-defined tables
)
BEGIN
   CREATE TABLE [dbo].[Roles](
	[Id] [int] PRIMARY KEY IDENTITY(1,1) NOT NULL,
	[RoleName] [nvarchar](max) NOT NULL
	)
END
GO

--  User Roles
IF NOT EXISTS (
    SELECT 1
    FROM sys.objects
    WHERE object_id = OBJECT_ID('dbo.UserRoles')
      AND type = 'U'  -- 'U' stands for user-defined tables
)
BEGIN   
CREATE TABLE [dbo].[UserRoles](
	[UserId] [int] NOT NULL,
	[RoleId] [int] NOT NULL,
 CONSTRAINT [PK_UserRoles] PRIMARY KEY CLUSTERED 
(
	[UserId] ASC,
	[RoleId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]


ALTER TABLE [dbo].[UserRoles]  WITH CHECK ADD  CONSTRAINT [FK_UserRoles_Roles_RoleId] FOREIGN KEY([RoleId])
REFERENCES [dbo].[Roles] ([Id])
ON DELETE CASCADE


ALTER TABLE [dbo].[UserRoles] CHECK CONSTRAINT [FK_UserRoles_Roles_RoleId]


ALTER TABLE [dbo].[UserRoles]  WITH CHECK ADD  CONSTRAINT [FK_UserRoles_Users_UserId] FOREIGN KEY([UserId])
REFERENCES [dbo].[Users] ([Id])
ON DELETE CASCADE


ALTER TABLE [dbo].[UserRoles] CHECK CONSTRAINT [FK_UserRoles_Users_UserId]
END
GO

-- Audit Logs
IF NOT EXISTS (
    SELECT 1
    FROM sys.objects
    WHERE object_id = OBJECT_ID('dbo.AuditLogs')
      AND type = 'U'  -- 'U' stands for user-defined tables
)
BEGIN
   
CREATE TABLE [dbo].[AuditLogs](
	[AuditId] [int] IDENTITY(1,1) NOT NULL,
	[UserId] [int] NOT NULL,
	[ActionType] [nvarchar](50) NOT NULL,
	[UpdatedBy] [nvarchar](50) NOT NULL,
	[UpdatedOn] [datetime2](7) NOT NULL,
	[OldValue] [nvarchar](max) NOT NULL,
	[NewValue] [nvarchar](max) NOT NULL,
 CONSTRAINT [PK_AuditLogs] PRIMARY KEY CLUSTERED 
(
	[AuditId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

ALTER TABLE [dbo].[AuditLogs]  WITH CHECK ADD  CONSTRAINT [FK_AuditLogs_Users_UserId] FOREIGN KEY([UserId])
REFERENCES [dbo].[Users] ([Id])
ON DELETE CASCADE

ALTER TABLE [dbo].[AuditLogs] CHECK CONSTRAINT [FK_AuditLogs_Users_UserId]

END
GO

-- Exception Handling
IF NOT EXISTS (
    SELECT 1
    FROM sys.objects
    WHERE object_id = OBJECT_ID('dbo.exceptionsLogs')
      AND type = 'U'  -- 'U' stands for user-defined tables
)
BEGIN
   
CREATE TABLE [dbo].[exceptionsLogs](
	[Id] [int] PRIMARY KEY IDENTITY(1,1) NOT NULL,
	[Timestamp] [datetime2](7) NOT NULL,
	[Message] [nvarchar](max) NOT NULL,
	[StackTrace] [nvarchar](max) NOT NULL,
	[ExceptionType] [nvarchar](max) NOT NULL)
END
GO

-- Trigger for auditing
DROP TRIGGER [dbo].[trg_AuditUsers]
GO

CREATE TRIGGER [dbo].[trg_AuditUsers]
ON [dbo].[Users]
AFTER INSERT, UPDATE, DELETE
AS
BEGIN
    SET NOCOUNT ON;

    -- Insert audit records for INSERT operations
    INSERT INTO AuditLogs (UserId, ActionType, UpdatedBy, UpdatedOn, OldValue, NewValue)
    SELECT 
        i.Id,
        'INSERT',
        i.Username, 
        GETDATE(),
		 (SELECT * FROM INSERTED i FOR JSON PATH, WITHOUT_ARRAY_WRAPPER),
        (SELECT * FROM INSERTED i FOR JSON PATH, WITHOUT_ARRAY_WRAPPER)
    FROM INSERTED i;

    -- Insert audit records for UPDATE operations
    INSERT INTO AuditLogs (UserId, ActionType, UpdatedBy, UpdatedOn, OldValue, NewValue)
    SELECT 
        i.Id,
        'UPDATE',
        i.Username,
        GETDATE(),
        (SELECT * FROM DELETED d FOR JSON PATH, WITHOUT_ARRAY_WRAPPER),
        (SELECT * FROM INSERTED i FOR JSON PATH, WITHOUT_ARRAY_WRAPPER)
    FROM INSERTED i
    JOIN DELETED d ON i.Id = d.Id;

    -- Insert audit records for DELETE operations
    INSERT INTO AuditLogs (UserId, ActionType, UpdatedBy, UpdatedOn, OldValue, NewValue)
    SELECT 
        d.Id,
        'DELETE',
        d.Username,
        GETDATE(),
        (SELECT * FROM DELETED d FOR JSON PATH, WITHOUT_ARRAY_WRAPPER),
        (SELECT * FROM DELETED d FOR JSON PATH, WITHOUT_ARRAY_WRAPPER)

    FROM DELETED d;
END

GO

ALTER TABLE [dbo].[Users] ENABLE TRIGGER [trg_AuditUsers]
GO