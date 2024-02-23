SELECT TOP (1000) [UserId]
      ,[Username]
      ,[Email]
      ,[PasswordHash]
  FROM [private].[dbo].[Users]

  -- Reset identity value for the UserId column in the Users table
DBCC CHECKIDENT ('[private].[dbo].[Users]', RESEED, 0);
