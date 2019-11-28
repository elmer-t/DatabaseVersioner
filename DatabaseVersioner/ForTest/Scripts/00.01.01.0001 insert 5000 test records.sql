

DECLARE
    @i          int,
    @Content    int;
SET @i = 5001;

WHILE @i > 5000 AND @i < 10000
BEGIN
    SET @Content = ROUND(((10000-5000)*RAND()+5000),0)
    INSERT INTO dbo.Test
		(Description)
    VALUES
		(convert(varchar, @Content));
    SET @i = @i + 1;
END