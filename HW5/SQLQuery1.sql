USE [master]
GO

IF DB_ID(N'Logistics') IS NOT NULL
BEGIN
    ALTER DATABASE [Logistics] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE [Logistics];
END

CREATE DATABASE [Logistics]
GO

USE [Logistics]
GO

CREATE TABLE [dbo].[Invoice](
    [Id] [uniqueidentifier] NOT NULL,
    [RecipientAddress] [nvarchar](max) NOT NULL,
    [SenderAddress] [nvarchar](max) NOT NULL,
    [RecipientPhoneNumber] [nvarchar](max) NULL,
    [SenderPhoneNumber] [nvarchar](max) NULL,
    CONSTRAINT [PK_Invoice] PRIMARY KEY CLUSTERED 
    (
        [Id] ASC
    )
)

CREATE TABLE [dbo].[Cargo](
    [Id] [uniqueidentifier] NOT NULL,
    [Volume] [float] NOT NULL,
    [Weight] [int] NOT NULL,
    [Code] [nvarchar](max) NOT NULL,
    [InvoiceId] [uniqueidentifier] NOT NULL,
    CONSTRAINT [PK_Cargo] PRIMARY KEY CLUSTERED 
    (
        [Id] ASC
    ),
    CONSTRAINT [FK_Cargo_Invoice] FOREIGN KEY([InvoiceId])
    REFERENCES [dbo].[Invoice] ([Id])
)

CREATE TABLE [dbo].[Vehicle](
    [Id] [int] NOT NULL,
    [Number] [nvarchar](max) NOT NULL,
    [MaxCargoWeightKg] [int] NOT NULL,
    [MaxCargoWeightPnd] [float] NOT NULL,
    [MaxCargoVolume] [float] NOT NULL,
    [CurrentCargoWeight] [int] NOT NULL,
    [CurrentCargoVolume] [float] NOT NULL,
    [Type] [nvarchar](max) NOT NULL,
    CONSTRAINT [PK_Vehicle] PRIMARY KEY CLUSTERED 
    (
        [Id] ASC
    )
)

CREATE TABLE [dbo].[Warehouse](
    [Id] [int] NOT NULL,
    CONSTRAINT [PK_Warehouse] PRIMARY KEY CLUSTERED 
    (
        [Id] ASC
    )
)

ALTER TABLE [dbo].[Vehicle] ADD CONSTRAINT [CK_Vehicle_Type] CHECK (Type IN ('Car', 'Ship', 'Plane','Train'))
ALTER TABLE [dbo].[Cargo] ADD CONSTRAINT [CK_Cargo_Weight] CHECK (Weight >= 0)
ALTER TABLE [dbo].[Cargo] ADD CONSTRAINT [CK_Cargo_Volume] CHECK (Volume >= 0)
ALTER TABLE [dbo].[Vehicle] ADD CONSTRAINT [CK_Vehicle_MaxCargoWeightKg] CHECK (MaxCargoWeightKg >= 0)
ALTER TABLE [dbo].[Vehicle] ADD CONSTRAINT [CK_Vehicle_MaxCargoWeightPnd] CHECK (MaxCargoWeightPnd >= 0)
ALTER TABLE [dbo].[Vehicle] ADD CONSTRAINT [CK_Vehicle_MaxCargoVolume] CHECK (MaxCargoVolume >= 0)
ALTER TABLE [dbo].[Vehicle] ADD CONSTRAINT [CK_Vehicle_CurrentCargoWeight] CHECK (CurrentCargoWeight >= 0)
