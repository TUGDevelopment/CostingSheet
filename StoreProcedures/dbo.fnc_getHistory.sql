USE [DevCostingSheet]
GO

/****** Object:  UserDefinedFunction [dbo].[fnc_getHistory]    Script Date: 8/24/2023 3:13:53 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE FUNCTION [dbo].[fnc_getHistory](
	@tablename nvarchar(max),
	@user nvarchar(max),
	@id nvarchar(max))
RETURNS @Mytbl TABLE (id int, RequestNo nvarchar(max),Username nvarchar(max),StatusApp nvarchar(max),Remark nvarchar(max), Reason nvarchar(max), tablename nvarchar(max), CreateOn nvarchar(max), Title nvarchar(max))  
AS
BEGIN
	insert into @Mytbl 
	SELECT a.Id,
	a.RequestNo,
	a.Username,
	a.StatusApp,
	a.Remark,case when Title in('Approve','Accept') then '-' else a.Reason end 'Reason' ,a.tablename,
	format(a.CreateOn,'dd-MM-yyyy HH:mm:ss')CreateOn,b.Title from MasHistory a left join MasStepApproval b on b.Id=a.StatusApp
	where RequestNo=@Id and a.tablename=@tablename order by a.id desc
RETURN
END

GO


