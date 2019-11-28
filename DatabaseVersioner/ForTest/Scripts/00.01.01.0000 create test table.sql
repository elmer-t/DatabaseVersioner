-- some update statements...

CREATE TABLE [dbo].[Test](
   [ID] [int] IDENTITY(1,1) NOT NULL,
   [Description] [varchar](255) NOT NULL,
 
    CONSTRAINT [PK_Test] 
        PRIMARY KEY CLUSTERED ([ID] ASC)
)