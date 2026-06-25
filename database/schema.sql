IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = N'LiquiLabsDB')
BEGIN
    CREATE DATABASE LiquiLabsDB;
END
GO

USE LiquiLabsDB;
GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Users]') AND type = 'U')
BEGIN
    CREATE TABLE dbo.Users (
        Id INT PRIMARY KEY,
        Name NVARCHAR(100) NOT NULL,
        Username NVARCHAR(100) NOT NULL UNIQUE,
        Email NVARCHAR(100) NOT NULL UNIQUE,
        CreatedAt DATETIME DEFAULT GETDATE()
    );
END
GO