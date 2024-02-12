create PROCEDURE [dbo].[spCallWebService]
AS
BEGIN
    Declare @Object as Int;
    Declare @ResponseText as Varchar(800),@sUrl nvarchar(max);
	set @sUrl ='http://192.168.1.193/WebAPI/ServiceCS.asmx/ExportTo?Data=ddd'
    Exec sp_OACreate 'MSXML2.XMLHTTP', @Object OUT;
    Exec sp_OAMethod @Object, 'open', NULL, 'get', @sUrl,'false'
    Exec sp_OAMethod @Object, 'send'
    Exec sp_OAMethod @Object, 'responseText', @ResponseText OUTPUT
    Select @ResponseText
    Exec sp_OADestroy @Object
END
go

