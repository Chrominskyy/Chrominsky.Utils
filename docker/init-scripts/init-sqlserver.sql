-- SQL Server initialization script for Chrominsky.Utils
-- This script creates sample tables using the library's base entities

USE chrominsky_db;
GO

-- Create a sample users table with full metadata tracking
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Users]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[Users] (
        [Id] UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
        [Username] NVARCHAR(255) NOT NULL UNIQUE,
        [Email] NVARCHAR(255) NOT NULL UNIQUE,
        [PasswordHash] NVARCHAR(255) NOT NULL,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        [UpdatedAt] DATETIME2 NULL,
        [CreatedBy] NVARCHAR(255) NULL,
        [UpdatedBy] NVARCHAR(255) NULL,
        [Status] INT NOT NULL DEFAULT 0
    );
END
GO

-- Create object versioning table for tracking changes
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ObjectVersions]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[ObjectVersions] (
        [Id] UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
        [ObjectId] UNIQUEIDENTIFIER NOT NULL,
        [ObjectType] NVARCHAR(255) NOT NULL,
        [PropertyName] NVARCHAR(255) NOT NULL,
        [OldValue] NVARCHAR(MAX) NULL,
        [NewValue] NVARCHAR(MAX) NULL,
        [ChangedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        [ChangedBy] NVARCHAR(255) NULL
    );
END
GO

-- Create sample products table
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Products]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[Products] (
        [Id] UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
        [Name] NVARCHAR(255) NOT NULL,
        [Description] NVARCHAR(MAX) NULL,
        [Price] DECIMAL(18, 2) NOT NULL,
        [StockQuantity] INT NOT NULL DEFAULT 0,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        [UpdatedAt] DATETIME2 NULL,
        [CreatedBy] NVARCHAR(255) NULL,
        [UpdatedBy] NVARCHAR(255) NULL,
        [Status] INT NOT NULL DEFAULT 0
    );
END
GO

-- Create indexes for better performance
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Users_Email')
    CREATE INDEX IX_Users_Email ON [dbo].[Users]([Email]);
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Users_Status')
    CREATE INDEX IX_Users_Status ON [dbo].[Users]([Status]);
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_ObjectVersions_ObjectId')
    CREATE INDEX IX_ObjectVersions_ObjectId ON [dbo].[ObjectVersions]([ObjectId]);
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Products_Status')
    CREATE INDEX IX_Products_Status ON [dbo].[Products]([Status]);
GO

-- Insert sample data
IF NOT EXISTS (SELECT * FROM [dbo].[Users] WHERE [Email] = 'admin@example.com')
BEGIN
    INSERT INTO [dbo].[Users] ([Username], [Email], [PasswordHash], [CreatedBy], [Status])
    VALUES 
        ('admin', 'admin@example.com', '$2a$11$examplehashhere', 'system', 0),
        ('testuser', 'test@example.com', '$2a$11$examplehashhere', 'system', 0);
END
GO

IF NOT EXISTS (SELECT * FROM [dbo].[Products])
BEGIN
    INSERT INTO [dbo].[Products] ([Name], [Description], [Price], [StockQuantity], [CreatedBy], [Status])
    VALUES 
        ('Sample Product 1', 'Description for product 1', 29.99, 100, 'system', 0),
        ('Sample Product 2', 'Description for product 2', 49.99, 50, 'system', 0);
END
GO
