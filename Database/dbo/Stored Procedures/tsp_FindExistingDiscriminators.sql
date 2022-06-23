CREATE PROCEDURE [dbo].[tsp_FindExistingDiscriminators]
	@Username VARCHAR(16)
AS
	SELECT [Discriminator]
	FROM [dbo].[User]
	WHERE [Username] = @Username

RETURN 0
