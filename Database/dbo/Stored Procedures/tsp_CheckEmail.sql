CREATE PROCEDURE [dbo].[tsp_CheckEmail]
	@Email VARCHAR(255)
AS
	DECLARE @Exists BIT

	IF EXISTS(
		SELECT *
		FROM [dbo].[User]
		WHERE [Email] = @Email
	)
		BEGIN
			-- Email is already in use
			SET @Exists = 1
		END
	ELSE
		BEGIN
			-- Email is available
			SET @Exists = 0
		END

	SELECT @Exists AS 'Exists'

RETURN 0
