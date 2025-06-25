-- PAD 2.0 Database Creation Script
-- This script creates the main database and sets up initial configuration

USE master;
GO

-- Drop database if it exists (for development)
IF EXISTS (SELECT name FROM sys.databases WHERE name = 'PAD2_DB')
BEGIN
    ALTER DATABASE PAD2_DB SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE PAD2_DB;
END
GO

-- Create the database
CREATE DATABASE PAD2_DB
ON 
(
    NAME = 'PAD2_DB',
    FILENAME = 'C:\Program Files\Microsoft SQL Server\MSSQL15.MSSQLSERVER\MSSQL\DATA\PAD2_DB.mdf',
    SIZE = 100MB,
    MAXSIZE = 1GB,
    FILEGROWTH = 10MB
)
LOG ON 
(
    NAME = 'PAD2_DB_Log',
    FILENAME = 'C:\Program Files\Microsoft SQL Server\MSSQL15.MSSQLSERVER\MSSQL\DATA\PAD2_DB_Log.ldf',
    SIZE = 10MB,
    MAXSIZE = 100MB,
    FILEGROWTH = 5MB
);
GO

-- Set database options
ALTER DATABASE PAD2_DB SET RECOVERY FULL;
ALTER DATABASE PAD2_DB SET AUTO_SHRINK OFF;
ALTER DATABASE PAD2_DB SET AUTO_CREATE_STATISTICS ON;
ALTER DATABASE PAD2_DB SET AUTO_UPDATE_STATISTICS ON;
GO

USE PAD2_DB;
GO

-- Create database schema for organization
CREATE SCHEMA [pad] AUTHORIZATION [dbo];
GO

PRINT 'PAD 2.0 Database created successfully'; 