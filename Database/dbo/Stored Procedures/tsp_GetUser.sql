CREATE PROCEDURE [dbo].[tsp_GetUser]
	@Id VARCHAR(255)
AS
	SELECT *
	FROM [dbo].[User]
	WHERE [Id] = @Id

RETURN 0
