create function ThaiDate (@date datetime) returns nvarchar(100) 
as 
begin 
declare @d1 nvarchar(3),@m1 nvarchar(25), @y1 nvarchar(4) 
select @d1= convert(nvarchar(3),day(@date)) 
select @m1 = case   month(@date)     when 1 then N'มกราคม'                                   
                                             when 2 then N'กุมภาพันธ์'                                   
                                             when 3 then N'มีนาคม'                                   
                                             when 4 then N'เมษายน'                                   
                                             when 5 then N'พฤษภาคม'                                   
                                             when 6 then N'มิถุนายน'                                   
                                             when 7 then N'กรกฎาคม'                                   
                                             when 8 then N'สิงหาคม'                                   
                                             when 9 then N'กันยายน'                                    
                                             when 10 then N'ตุลาคม'                                   
                                             when 11 then N'พฤศจิกายน'                                   
                                             when 12 then N'ธันวาคม'                                  
                                             end 
     select @y1 = convert(nvarchar(4),year(@date) + 543 )    
     return @d1 + ' ' + @m1 + ' ' + @y1 
end
go

