INSERT INTO [dbo].[Vehicle] ([Id],[Number],[MaxCargoWeightKg],[MaxCargoWeightPnd],[MaxCargoVolume],[CurrentCargoWeight],[CurrentCargoVolume],[Type]) 
VALUES (1, 3048, 24000, 50000, 85, 0, 0, 'Car')

INSERT INTO [dbo].[Vehicle] ([Id],[Number],[MaxCargoWeightKg],[MaxCargoWeightPnd],[MaxCargoVolume],[CurrentCargoWeight],[CurrentCargoVolume],[Type]) 
VALUES (2, 1350, 100000, 200000, 120, 0, 0, 'Ship')

INSERT INTO [dbo].[Vehicle] ([Id],[Number],[MaxCargoWeightKg],[MaxCargoWeightPnd],[MaxCargoVolume],[CurrentCargoWeight],[CurrentCargoVolume],[Type]) 
VALUES (3, 150250, 120000, 240000, 150, 0, 0, 'Plane')

INSERT INTO [dbo].[Vehicle] ([Id],[Number],[MaxCargoWeightKg],[MaxCargoWeightPnd],[MaxCargoVolume],[CurrentCargoWeight],[CurrentCargoVolume],[Type]) 
VALUES (4, 05258, 150000, 300000, 170, 0, 0, 'Train')

INSERT INTO [dbo].[Warehouse]([Id])
VALUES (1)

INSERT INTO [dbo].[Warehouse]([Id])
VALUES (2)

INSERT INTO [dbo].[Cargo] ([Id],[Volume], [Weight], [Code], [InvoiceId])
VALUES(NEWID(),30, 2000,'cement', NULL)

INSERT INTO [dbo].[Cargo] ([Id],[Volume], [Weight], [Code], [InvoiceId])
VALUES(NEWID(),25, 1700,'sand', NULL)

INSERT INTO [dbo].[Cargo] ([Id], [Volume], [Weight], [Code], [InvoiceId])
VALUES (NEWID(), 40, 4000, 'rubble', NULL)

INSERT INTO [dbo].[Cargo] ([Id], [Volume], [Weight], [Code], [InvoiceId])
VALUES (NEWID(), 23, 2800, 'putty', NULL)

INSERT INTO [dbo].[Invoice] ([Id], [RecipientAddress], [SenderAddress], [RecipientPhoneNumber], [SenderPhoneNumber])
VALUES (NEWID(), '12 Green Street', '45 Red Street', '380970011222', '380978866999')

INSERT INTO [dbo].[Invoice] ([Id], [RecipientAddress], [SenderAddress], [RecipientPhoneNumber], [SenderPhoneNumber])
VALUES (NEWID(), '12 Blue Street', '896 White Street', '380970011222', '380978866333')


UPDATE [dbo].[Cargo] SET [InvoiceId] = (SELECT TOP 1 [Id] FROM [dbo].[Invoice])
UPDATE [dbo].[Cargo] SET [InvoiceId] = (SELECT TOP 1 [Id] FROM [dbo].[Invoice] WHERE [RecipientAddress] LIKE '%Green Street%')
UPDATE [dbo].[Cargo] SET [InvoiceId] = (SELECT TOP 1 [Id] FROM [dbo].[Invoice] WHERE [RecipientAddress] LIKE '%45 Red Street%') WHERE [Code] = 'cement'

INSERT INTO [dbo].[Cargo] ([Id], [Volume], [Weight], [Code], [InvoiceId])
VALUES (NEWID(), 5, 500, 'sand', (SELECT TOP 1 [Id] FROM [dbo].[Invoice] WHERE [RecipientAddress] LIKE '%Blue Street%'))






