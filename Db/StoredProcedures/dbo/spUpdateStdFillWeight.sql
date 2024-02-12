create PROCEDURE [dbo].[spUpdateStdFillWeight]
	-- Add the parameters for the stored procedure here
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
 
	declare @idx nvarchar(max)
	declare cur_Employee CURSOR FOR

	SELECT SAPCodeDigit from  StdFillWeight_temp  

	open cur_Employee

	FETCH NEXT FROM cur_Employee INTO @idx 
	WHILE @@FETCH_STATUS = 0
	BEGIN
		if(select count(*) from StdFillWeight where SAPCodeDigit=@idx)=0
		insert StdFillWeight
		select [SAPCodeDigit]
      ,[Name]
      ,[DW]
      ,[NetWeight]
      ,[FillWeight]
      ,[Unit] from StdFillWeight_temp where SAPCodeDigit=@idx
	  else
	  UPDATE a SET FillWeight = b.FillWeight
		FROM StdFillWeight a
		JOIN StdFillWeight_temp b
			ON a.SAPCodeDigit = b.SAPCodeDigit 
		WHERE a.SAPCodeDigit=@idx

		FETCH NEXT FROM cur_Employee INTO @idx 
	END

	CLOSE cur_Employee
	DEALLOCATE cur_Employee
END
go

