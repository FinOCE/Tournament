CREATE PROCEDURE [dbo].[tsp_CreateUser]
	@Id VARCHAR(255),
	@Email VARCHAR(255),
	@Username VARCHAR(16),
	@Discriminator INT,
	@Password VARCHAR(255)
AS
	INSERT INTO [dbo].[User]
		([Id], [Email], [Username], [Discriminator], [Password])
	VALUES
		(@Id, @Email, @Username, @Discriminator, @Password)

	SELECT *
	FROM [dbo].[User]
	WHERE [Id] = @Id

RETURN 0
