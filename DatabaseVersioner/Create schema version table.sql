
/*

Execute this script on all database the tool needs to connect to.
If you've changed the name of the schema changes table name you should
make sure it matches up in this script.

*/
CREATE TABLE [dbo].[SchemaChanges](
   [ID] [int] IDENTITY(1,1) NOT NULL,
   [Major] [int] NOT NULL,
   [Minor] [int] NOT NULL,
   [Revision] [int] NOT NULL,
   [Build] [int] NOT NULL,
   [Description] [varchar](255) NOT NULL,
   [DateApplied] [datetime] NOT NULL,

	CONSTRAINT [PK_SchemaChanges] 
		PRIMARY KEY CLUSTERED ([ID] ASC)
)

/*
OPTIONAL: initialize the schema version table with 
a version suitable for your situation. You might want
to match this with your application version.
*/
INSERT INTO [dbo].[SchemaChanges] (
	Major, Minor, Revision, Build, DateApplied
) VALUES (
	1, 0, 0, 0, GETDATE()
)