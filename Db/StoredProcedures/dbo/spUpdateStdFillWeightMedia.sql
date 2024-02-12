CREATE PROCEDURE [dbo].[spUpdateStdFillWeightMedia]
	-- Add the parameters for the stored procedure here
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
 
	declare @idx nvarchar(max),@GroupType nvarchar(max),@Code nvarchar(max)
	declare cur_Employee CURSOR FOR

	SELECT SAPCodeDigit,GroupType,Code from  StdFillWeightMedia_temp  

	open cur_Employee

	FETCH NEXT FROM cur_Employee INTO @idx ,@GroupType,@Code
	WHILE @@FETCH_STATUS = 0
	BEGIN
		if(select count(*) from StdFillWeightMedia where SAPCodeDigit=@idx and GroupType=@GroupType and Code=@Code)=0
		insert StdFillWeightMedia
		select [SAPCodeDigit]
      ,[Name]
      ,[GroupType]
      ,[GroupDescription]
      ,[Code]
      ,[CodeName]
      ,[MediaWeight]
      ,[Unit] from StdFillWeightMedia_temp where SAPCodeDigit=@idx and GroupType=@GroupType and Code=@Code
	  else
	  UPDATE a SET MediaWeight = b.MediaWeight
		FROM StdFillWeightMedia a
		JOIN StdFillWeightMedia_temp b
			ON a.SAPCodeDigit = b.SAPCodeDigit 
		WHERE a.SAPCodeDigit=@idx and a.GroupType=@GroupType and a.Code=@Code

		FETCH NEXT FROM cur_Employee INTO @idx ,@GroupType,@Code
	END

	CLOSE cur_Employee
	DEALLOCATE cur_Employee
END
go

