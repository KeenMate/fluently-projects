/****** Object:  Schema [LOGS]    Script Date: 31.07.2018 14:16:46 ******/
CREATE SCHEMA [LOGS]
GO
/****** Object:  Table [LOGS].[Logs]    Script Date: 31.07.2018 14:16:46 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [LOGS].[Logs](
	[LogId] [bigint] IDENTITY(1,1) NOT NULL,
	[LoggedOnDate] [datetime] NOT NULL,
	[Message] [nvarchar](1000) NOT NULL,
	[Level] [nvarchar](50) NOT NULL,
	[CallSite] [nvarchar](500) NOT NULL,
	[Type] [nvarchar](500) NOT NULL,
	[StackTrace] [ntext] NOT NULL,
	[InnerException] [ntext] NOT NULL,
	[AdditionalInfo] [nvarchar](3000) NOT NULL,
 CONSTRAINT [pk_logs] PRIMARY KEY CLUSTERED 
(
	[LogId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

ALTER TABLE [LOGS].[Logs] ADD  CONSTRAINT [df_logs_loggedondate]  DEFAULT (getutcdate()) FOR [LoggedOnDate]
GO
/****** Object:  StoredProcedure [LOGS].[InsertLog]    Script Date: 31.07.2018 14:16:46 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/*
	Create InsertLog stored procedure
*/

CREATE procedure [LOGS].[InsertLog] 
(
	@level nvarchar(50),
	@callSite nvarchar(500),
	@type nvarchar(500),
	@message nvarchar(1000),
	@stackTrace ntext,
	@innerException ntext,
	@additionalInfo nvarchar(3000)
)
as

insert into LOGS.Logs
(
	[Level],
	CallSite,
	[Type],
	[Message],
	StackTrace,
	InnerException,
	AdditionalInfo
)
values
(
	@level,
	@callSite,
	@type,
	@message,
	@stackTrace,
	@innerException,
	@additionalInfo
)

GO
