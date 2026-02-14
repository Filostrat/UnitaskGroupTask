SET NOCOUNT ON;
IF DB_ID('CompanyDb') IS NULL
BEGIN
    PRINT 'Creating database CompanyDb...';
    CREATE DATABASE CompanyDb;
END
GO

USE CompanyDb;
GO

IF NOT EXISTS (SELECT * FROM sys.schemas WHERE name = 'hr')
BEGIN
    EXEC('CREATE SCHEMA hr');
END
GO

IF OBJECT_ID('hr.Employees','U') IS NULL
BEGIN
    CREATE TABLE hr.Employees
    (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        Name NVARCHAR(200) NOT NULL,
        ManagerId INT NULL,
        Enable BIT NOT NULL CONSTRAINT DF_Employees_Enable DEFAULT (1)
    );
END
GO

IF NOT EXISTS (SELECT 1 FROM hr.Employees)
BEGIN
    SET IDENTITY_INSERT hr.Employees ON;

    INSERT INTO hr.Employees (Id, Name, ManagerId, Enable) VALUES (1, N'Andrey', NULL, 1);

    INSERT INTO hr.Employees (Id, Name, ManagerId, Enable) VALUES (2, N'Alexey', 1, 1);
    INSERT INTO hr.Employees (Id, Name, ManagerId, Enable) VALUES (3, N'Roman', 1, 1);

    INSERT INTO hr.Employees (Id, Name, ManagerId, Enable) VALUES (4, N'Olga', 2, 1);
    INSERT INTO hr.Employees (Id, Name, ManagerId, Enable) VALUES (5, N'Igor', 2, 1);
    INSERT INTO hr.Employees (Id, Name, ManagerId, Enable) VALUES (6, N'Svetlana', 3, 1);

    INSERT INTO hr.Employees (Id, Name, ManagerId, Enable) VALUES (7, N'Max', 4, 1);

    INSERT INTO hr.Employees (Id, Name, ManagerId, Enable) VALUES (8, N'Olena', 9, 1);
    INSERT INTO hr.Employees (Id, Name, ManagerId, Enable) VALUES (9, N'Dmitry', 8, 1);

    INSERT INTO hr.Employees (Id, Name, ManagerId, Enable) VALUES (10, N'Sergei', 10, 1);


    SET IDENTITY_INSERT hr.Employees OFF;
END
GO