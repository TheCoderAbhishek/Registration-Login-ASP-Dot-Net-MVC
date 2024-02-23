SELECT TOP (1000) [UserId]
      ,[Username]
      ,[Email]
      ,[PasswordHash]
  FROM [private].[dbo].[Users]

  DELETE FROM Users;

  DBCC CHECKIDENT ('Users', RESEED, 0);