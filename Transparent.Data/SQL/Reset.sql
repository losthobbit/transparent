-- Deletes some restrictive data to make testing easier.
DELETE FROM [Transparent].[dbo].[TestMarkings]
DECLARE @TestId int = (SELECT TOP(1) Id FROM [Transparent].[dbo].[Tickets] WHERE Body LIKE 'Name a logical fallacy in the following sentence:%')
DELETE FROM [Transparent].[dbo].[UserPoints] WHERE FkTestId IS NOT NULL AND FkTestId <> @TestId