SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[User_Update]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[User_Update]
GO


CREATE PROCEDURE User_Update
@uid int,
@firstName nvarchar(45),
@lastName nvarchar(45)
AS
-- WARNING: This code is under source control. Do not modify directly.
update [dbo].[User] set firstName=@firstName, lastName = @lastName
where uid = @uid

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

