SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[User_Insert]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[User_Insert]
GO


CREATE PROCEDURE User_Insert
@firstName nvarchar(45),
@lastName nvarchar(45)
AS
-- WARNING: This code is under source control. Do not modify directly.
INSERT INTO [dbo].[User]
( firstName, lastName)
VALUES ( @firstName, @lastName)
RETURN @@Identity

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

