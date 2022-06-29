CREATE PROCEDURE [dbo].[tsp_UpdateUser]
	@Id VARCHAR(255),
	@Email VARCHAR(255) = NULL,
	@Username VARCHAR(16) = NULL,
	@Password VARCHAR(255) = NULL,
	@Discriminator INT = NULL,
	@Icon VARCHAR(16) = NULL,
	@Permissions INT = NULL,
	@Verified TINYINT = NULL
AS
	-- Update supplied values
	UPDATE [dbo].[User]
	SET
		[Email] = ISNULL(@Email, [Email]),
		[Username] = ISNULL(@Username, [Username]),
		[Password] = ISNULL(@Password, [Password]),
		[Discriminator] = ISNULL(@Discriminator, [Discriminator]),
		[Icon] = ISNULL(@Icon, [Icon]),
		[Permissions] = ISNULL(@Permissions, [Permissions]),
		[Verified] = ISNULL(@Verified, [Verified])
	WHERE [Id] = @Id

	-- Return updated user
	SELECT *
	FROM [dbo].[User]
	WHERE [Id] = @Id
RETURN 0
