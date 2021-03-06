SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[User_Select]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[User_Select]
GO

/*
User_Select 2
*/

CREATE PROCEDURE User_Select
@uid int = -1
AS


	select * from [dbo].[User] where ((@uid <> -1 and uid = @uid) or (@uid =-1 and uid is not null))

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

