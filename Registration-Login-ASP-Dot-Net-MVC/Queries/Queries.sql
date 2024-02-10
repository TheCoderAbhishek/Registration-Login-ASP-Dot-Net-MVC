SELECT TOP (1000) [UserId]
      ,[Username]
      ,[Email]
      ,[PasswordHash]
  FROM [private].[dbo].[Users]

  -- Delete all records from the table
DELETE FROM [private].[dbo].[Users];

-- Reset the UserId column to start from 1
DBCC CHECKIDENT ('[private].[dbo].[Users]', RESEED, 0);
