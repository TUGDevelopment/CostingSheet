
/************* json_toJson *********************/



CREATE  FUNCTION [dbo].[json_toJson](@data pJsonData READONLY,@node int)
returns nvarchar(max)
as begin
    declare @kind   nvarchar(max)
    declare @value  nvarchar(max)

    select @kind=kind,@value=value from @data where id=@node
    if (@kind='STRING') return '"'+@value+'"'
    if (@kind='NUMBER') return @value
    if (@kind='BOOL') begin
        if @value=1 return 'True' else return 'False'
        end
    if (@kind='OBJECT') begin
        set @value=''
        SELECT @value= @value+ ',"'+Name +'":'+dbo.json_toJson(@data,id) FROM @data where parent=@node
        return '{'+iif(@value='','',substring(@value,2,len(@value)-1))+'}'
        end
    if (@kind='ARRAY') begin
        set @value=''
        SELECT @value= @value+ ','+dbo.json_toJson(@data,id) FROM @data where parent=@node
        return '['+iif(@value='','',substring(@value,2,len(@value)-1))+']'
        end
    return cast('Unkown KIND in Json Data' as int);
    return '*ERROR*'
end

go

