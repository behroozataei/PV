USE [Irisa_Scada_Power]
GO
/****** Object:  Table [app].[OPC_Params]    Script Date: 2021-03-06 07:49:43 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [app].[OPC_Params](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](64) NULL,
	[IP] [nvarchar](32) NOT NULL,
	[Port] [nvarchar](32) NOT NULL,
	[Description] [nvarchar](256) NULL
) ON [PRIMARY]
GO
SET IDENTITY_INSERT [app].[OPC_Params] ON 
GO
INSERT [app].[OPC_Params] ([ID], [Name], [IP], [Port], [Description]) VALUES (1, N'Test', N'127.0.0.1', N'49320', N'Local OPCServer')
GO
SET IDENTITY_INSERT [app].[OPC_Params] OFF
GO
