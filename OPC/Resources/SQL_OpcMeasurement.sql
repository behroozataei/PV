USE [Irisa_Scada_Power]
GO
/****** Object:  Table [opc].[OPCMeasurement]    Script Date: 2021-03-06 08:56:58 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [opc].[OPCMeasurement](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[ScadaTagName] [nvarchar](255) NULL,
	[OPCTagName] [nvarchar](255) NULL,
	[Description] [nvarchar](255) NULL,
	[MessageConfiguration] [int] NULL,
	[MeasurementId] [uniqueidentifier] NULL,
	[TagType] [varchar](255) NULL,
 CONSTRAINT [PK_Digital$] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET IDENTITY_INSERT [opc].[OPCMeasurement] ON 
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (1, N'CSM_C01_CB', N'Channel1.LoadShedding.Information Data Block.CSM_C01_CB', N'PLC : Tripped by PLC Load Shedding', 6, N'cbf0afce-5773-432d-b274-038141808a41', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (2, N'CSM_110VDC_Battry_Charge', N'Channel1.LoadShedding.Information Data Block.CSM_110VDC_Battry_Charge', N'PLC : CSM-Monitoring 110  VDC Battry Charger Infeed  MCBs OK(F200,F400)', 17, N'228ec8f6-73f2-4235-a850-25869f0b4fcb', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (3, N'CSM_220VAC_MCB', N'Channel1.LoadShedding.Information Data Block.CSM_220VAC_MCB', N'PLC : CSM-Monitoring 24  VDC Power Supply for Cards _ Site (PS2)(G2)', 17, N'5757bd34-6e51-411b-b4b7-a9993366a3df', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (4, N'CSM_24VDC_AnalogInput', N'Channel1.LoadShedding.Information Data Block.CSM_24VDC_AnalogInput', N'PLC : CSM-Monitoring 24  VDC Supply for Analog Input Cards (F601-603)', 17, N'02d28d67-aff2-4dd2-858d-c327e6f4cd80', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (5, N'CSM_24VDC_Card_Site_PS1', N'Channel1.LoadShedding.Information Data Block.CSM_24VDC_Card_Site_PS1', N'PLC : CSM-Monitoring 24  VDC Power Supply for Cards _ Site (PS1)(G1)', 17, N'bd511cd2-b242-49e4-937a-63edb46db077', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (6, N'CSM_24VDC_Card_Site_PS2', N'Channel1.LoadShedding.Information Data Block.CSM_24VDC_Card_Site_PS2', N'PLC : CSM-Monitoring 24  VDC Power Supply for Cards _ Site (PS2)(G2)', 17, N'20645a4d-304c-46a4-97d5-b65a4e08fca5', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (7, N'CSM_24VDC_DI_1300_1301', N'Channel1.LoadShedding.Information Data Block.CSM_24VDC_DI_1300_1301', N'PLC : CSM-Monitoring 24 VDC Common for DI Signals (I1300-I1301)(FT1/1)', 17, N'405d6a1c-b1b3-45d9-85bc-73a76865efcf', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (8, N'CSM_24VDC_DI_1302_1303', N'Channel1.LoadShedding.Information Data Block.CSM_24VDC_DI_1302_1303', N'PLC : CSM-Monitoring 24 VDC Common for DI Signals (I1302-I1303)(FT1/2)', 17, N'6870f020-6b4c-4908-b87c-7c4ec058cc52', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (9, N'CSM_24VDC_DI_1304_1305', N'Channel1.LoadShedding.Information Data Block.CSM_24VDC_DI_1304_1305', N'PLC : CSM-Monitoring 24 VDC Common for DI Signals (I1304-I1305)(FT1/3)', 17, N'a3721438-9798-4d3b-a801-31d6f4536dcb', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (10, N'CSM_24VDC_DI_1306_1307', N'Channel1.LoadShedding.Information Data Block.CSM_24VDC_DI_1306_1307', N'PLC : CSM-Monitoring 24 VDC Common for DI Signals (I1306-I1307)(FT1/4)', 17, N'da39fd4d-02d3-421d-830a-3c0804b3d134', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (11, N'CSM_24VDC_DI_1308_1309', N'Channel1.LoadShedding.Information Data Block.CSM_24VDC_DI_1308_1309', N'PLC : CSM-Monitoring 24 VDC Common for DI Signals (I1308-I1309)(FT1/5)', 17, N'6f454ba4-a9ff-43a5-afff-6c8a2936fbea', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (12, N'CSM_24VDC_DI_1310_1311', N'Channel1.LoadShedding.Information Data Block.CSM_24VDC_DI_1310_1311', N'PLC : CSM-Monitoring 24 VDC Common for DI Signals (I1310-I1311)(FT1/6)', 17, N'4a75171e-0ae0-427c-aaf7-50038cad8552', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (13, N'CSM_24VDC_DI_1345_1346', N'Channel1.LoadShedding.Information Data Block.CSM_24VDC_DI_1345_1346', N'PLC : CSM-Monitoring 24 VDC Common for DI Signals (I1345-I1346)(FT1/7)', 17, N'20e60c95-8008-40ea-9181-29d38f8edd31', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (14, N'CSM_24VDC_DI_1347_1348', N'Channel1.LoadShedding.Information Data Block.CSM_24VDC_DI_1347_1348', N'PLC : CSM-Monitoring 24 VDC Common for DI Signals (I1347-I1348)(FT1/8)', 17, N'2055c49a-d5cf-4566-a80a-c6d50e64f14e', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (15, N'CSM_24VDC_Digital_Output', N'Channel1.LoadShedding.Information Data Block.CSM_24VDC_Digital_Output', N'PLC : CSM-Monitoring 24  VDC Supply for Digital Output Cards (F604-F615)', 17, N'8b10199c-b868-4532-bd51-a7bdb96cad30', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (16, N'Can_Not_Meet_F10', N'Channel1.LoadShedding.Information Data Block.Can_Not_Meet_F10', N'PLC : Can not meet total load to be tripped F10', 6, N'ea8522a0-c136-4021-b852-2324d5cffa72', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (17, N'Can_Not_Meet_F9', N'Channel1.LoadShedding.Information Data Block.Can_Not_Meet_F9', N'PLC : Can not meet total load to be tripped F9', 6, N'f32693be-ccbb-4991-9418-2ce9aad0a0c6', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (18, N'CSM_NET1_NOD6_Disconnect', N'Channel1.LoadShedding.Information Data Block.CSM_NET1_NOD6_Disconnect', N'PLC : CSM-PROFIBUS 1 NOD 6 DISCONNECT', 6, N'4b85c8b9-acb6-49a5-a047-3c89ceb5599c', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (19, N'CSM_NET2_NOD6_Disconnect', N'Channel1.LoadShedding.Information Data Block.CSM_NET2_NOD6_Disconnect', N'PLC : CSM-PROFIBUS 2 NOD 6 DISCONNECT', 6, N'0f49dc20-0662-475b-a9d0-04621cad655e', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (20, N'EFS_NET1_NOD7_Disconnect', N'Channel1.LoadShedding.Information Data Block.EFS_NET1_NOD7_Disconnect', N'PLC : EFS-PROFIBUS 1 NOD 7 DISCONNECT', 6, N'e738770d-d117-4367-b2bc-12d37db76d8d', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (21, N'EFS_NET2_NOD7_Disconnect', N'Channel1.LoadShedding.Information Data Block.EFS_NET2_NOD7_Disconnect', N'PLC : EFS-PROFIBUS 2 NOD 7 DISCONNECT', 6, N'ed34f21e-20fd-49e1-bdbc-1b4ba0f97092', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (22, N'EFSB_NET1_NOD10_Disconne', N'Channel1.LoadShedding.Information Data Block.EFSB_NET1_NOD10_Disconne', N'PLC : EFSB-PROFIBUS 1 NOD10 DISCONNECT', 6, N'c9691aba-f7e7-4925-98cf-6572b48f08b4', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (23, N'EFSB_NET2_NOD10_Disconne', N'Channel1.LoadShedding.Information Data Block.EFSB_NET2_NOD10_Disconne', N'PLC : EFSB-PROFIBUS 2 NOD10 DISCONNECT', 6, N'092f0256-ef42-4bee-90cd-082470c0d65f', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (24, N'EFSB_PS2_PS3', N'Channel1.LoadShedding.Information Data Block.EFSB_PS2_PS3', N'PLC : EFSB-Monitoring 24  VDC Power Supply for Cards & Site (PS2,PS3)(G2,G3)(F400-F403)', 17, N'21454223-fc57-4661-8644-dd56b5d75907', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (25, N'MS1_MAC_D1_Fault', N'Channel1.LoadShedding.Information Data Block.MS1_MAC_D1_Fault', N'PLC : CB MALFUNCTION', 17, N'cbe5d22a-5f89-4660-89b4-deb0f44188b9', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (26, N'MS1_MBD_D1_Fault', N'Channel1.LoadShedding.Information Data Block.MS1_MBD_D1_Fault', N'PLC : CB MALFUNCTION', 17, N'006aada7-f9e9-4aeb-9e1c-8e02e5c4f71b', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (27, N'Fault_9_Case_A', N'Channel1.LoadShedding.Information Data Block.Fault_9_Case_A', N'PLC : FAULT 9 CASE A', 6, N'36a65694-1b05-4f6f-8325-a78529030b4b', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (28, N'Fault_10_Case_A', N'Channel1.LoadShedding.Information Data Block.Fault_10_Case_A', N'PLC : FAULT 10 CASE A', 6, N'f2966ebf-a672-4926-b19c-1bfcad69a5af', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (29, N'Fault_1_Case_A', N'Channel1.LoadShedding.Information Data Block.Fault_1_Case_A', N'PLC : FAULT 1 CASE A', 6, N'05ca137f-c266-43aa-a6e1-2b1803d7cb03', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (30, N'Fault_2_Case_A', N'Channel1.LoadShedding.Information Data Block.Fault_2_Case_A', N'PLC : FAULT 2 CASE A', 6, N'b3565928-9451-40e5-9b98-1dcdbec0820b', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (31, N'Fault_3_Case_A', N'Channel1.LoadShedding.Information Data Block.Fault_3_Case_A', N'PLC : FAULT 3 CASE A', 6, N'b270980c-99fe-452b-aad0-2796cb692228', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (32, N'Fault_4_Case_A', N'Channel1.LoadShedding.Information Data Block.Fault_4_Case_A', N'PLC : FAULT 4 CASE A', 6, N'944c7be3-ac04-409a-8254-7f284967ee82', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (33, N'Fault_5_Case_A', N'Channel1.LoadShedding.Information Data Block.Fault_5_Case_A', N'PLC : FAULT 5 CASE A', 6, N'3dceb7c2-b8c5-4460-9c07-3d91f0b99c1f', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (34, N'Fault_6_Case_A', N'Channel1.LoadShedding.Information Data Block.Fault_6_Case_A', N'PLC : FAULT 6 CASE A', 6, N'f54ba882-7c6d-4ec7-bee1-b18b2ad1ea17', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (35, N'Fault_7_Case_A', N'Channel1.LoadShedding.Information Data Block.Fault_7_Case_A', N'PLC : FAULT 7 CASE A', 6, N'7d51f5f5-b14c-4633-a321-8f3db1acfc81', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (36, N'Fault_8_Case_A', N'Channel1.LoadShedding.Information Data Block.Fault_8_Case_A', N'PLC : FAULT 8 CASE A', 6, N'13a7e1f1-2a59-4d44-a8da-9126cd5e8fac', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (37, N'Fault_9_Case_B', N'Channel1.LoadShedding.Information Data Block.Fault_9_Case_B', N'PLC : FAULT 9 CASE B', 6, N'b1a86e61-bef6-44c1-bd61-d679eb78a6f6', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (38, N'Fault_10_Case_B', N'Channel1.LoadShedding.Information Data Block.Fault_10_Case_B', N'PLC : FAULT 10 CASE B', 6, N'dd27987e-8220-4a33-ae76-1db7da1af1d1', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (39, N'HSM_NET1_NOD5_Disconnect', N'Channel1.LoadShedding.Information Data Block.HSM_NET1_NOD5_Disconnect', N'PLC : HSM-PROFIBUS 1 NOD 5 DISCONNECT', 6, N'73242fb5-e80f-494e-99a5-1268149a918b', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (40, N'HSM_NET2_NOD5_Disconnect', N'Channel1.LoadShedding.Information Data Block.HSM_NET2_NOD5_Disconnect', N'PLC : HSM-PROFIBUS 2 NOD 5 DISCONNECT', 6, N'1d3b8690-f851-42d4-9b0d-b583e78b930b', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (41, N'MIS_NET1_NOD1_Disconnect', N'Channel1.LoadShedding.Information Data Block.MIS_NET1_NOD1_Disconnect', N'PLC : MIS-PROFIBUS 1 NOD 1 DISCONNECT', 6, N'2d048362-80aa-41bc-9778-74019c17cc36', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (42, N'MIS_NET1_NOD3_Disconnect', N'Channel1.LoadShedding.Information Data Block.MIS_NET1_NOD3_Disconnect', N'PLC : MIS-PROFIBUS 1 NOD 3 DISCONNECT', 6, N'9e95bd21-838c-4a3a-b8b7-26982b92b5ba', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (43, N'MIS_NET1_NOD4_Disconnect', N'Channel1.LoadShedding.Information Data Block.MIS_NET1_NOD4_Disconnect', N'PLC : MIS-PROFIBUS 1 NOD 4 DISCONNECT', 6, N'a1e4aeb7-c1ce-4671-9ca8-86ccc2cd9c58', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (44, N'Fault_1_Case_B', N'Channel1.LoadShedding.Information Data Block.Fault_1_Case_B', N'PLC : MIS-PROFIBUS 2 NOD 1 DISCONNECT', 6, N'b9ceb2fc-def6-423d-a25e-137333487af7', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (45, N'Fault_2_Case_B', N'Channel1.LoadShedding.Information Data Block.Fault_2_Case_B', N'PLC : MIS-PROFIBUS 2 NOD 3 DISCONNECT', 6, N'5c31d31c-d215-4264-a9bf-e766bd38df5c', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (46, N'Fault_3_Case_B', N'Channel1.LoadShedding.Information Data Block.Fault_3_Case_B', N'PLC : MIS-PROFIBUS 2 NOD4 DISCONNECT', 6, N'17452b90-50a4-44fe-84d9-f9f2f0f6ba99', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (47, N'Fault_4_Case_B', N'Channel1.LoadShedding.Information Data Block.Fault_4_Case_B', N'PLC : FAULT 4 CASE B', 6, N'd18e563d-dd68-49d4-9865-dde39ab9fe9b', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (48, N'Fault_5_Case_B', N'Channel1.LoadShedding.Information Data Block.Fault_5_Case_B', N'PLC : FAULT 5 CASE B', 6, N'77e65a0d-cbbe-4f9d-b5ff-c0f64bc87c7d', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (49, N'Fault_6_Case_B', N'Channel1.LoadShedding.Information Data Block.Fault_6_Case_B', N'PLC : FAULT 6 CASE B', 6, N'c032e41f-3766-42e1-9100-27748c66cd33', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (50, N'Fault_7_Case_B', N'Channel1.LoadShedding.Information Data Block.Fault_7_Case_B', N'PLC : FAULT 7 CASE B', 6, N'26f75b1c-f6db-44fe-a30c-9504a7ec3ba1', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (51, N'Fault_8_Case_B', N'Channel1.LoadShedding.Information Data Block.Fault_8_Case_B', N'PLC : FAULT 8 CASE B', 6, N'ef637032-a2f2-404d-8784-cdd8d42ea456', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (52, N'MIS_NET2_NOD1_Disconnect', N'Channel1.LoadShedding.Information Data Block.MIS_NET2_NOD1_Disconnect', N'PLC : MIS-PROFIBUS 2 NOD 1 DISCONNECT', 6, N'60467077-cc7a-406e-9a38-29e23bc20a1e', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (53, N'MIS_NET2_NOD3_Disconnect', N'Channel1.LoadShedding.Information Data Block.MIS_NET2_NOD3_Disconnect', N'PLC : MIS-PROFIBUS 2 NOD 3 DISCONNECT', 6, N'5ca482b4-328e-44d4-99d8-349127bbbf2b', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (54, N'MIS_NET2_NOD4_Disconnect', N'Channel1.LoadShedding.Information Data Block.MIS_NET2_NOD4_Disconnect', N'PLC : MIS-PROFIBUS 2 NOD4 DISCONNECT', 6, N'f4b6f90f-47ea-4ff1-b9ab-03897f27ac21', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (55, N'MIS_S7_400_Fan_Fault', N'Channel1.LoadShedding.Information Data Block.MIS_S7_400_Fan_Fault', N'PLC : MIS-Monitoring S7-400 Fan Fault', 17, N'be2d558c-ba08-4b0e-891c-82677e7408dc', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (56, N'NC_Bus_Tie_Closed', N'Channel1.LoadShedding.Information Data Block.NC_Bus_Tie_Closed', N'PLC : MAB IS CLOSED', 17, N'f9568830-71c5-4a69-991e-33d693574202', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (57, N'NC_check_con_T12578_pp', N'Channel1.LoadShedding.Information Data Block.NC_check_con_T12578_pp', N'PLC : Check connection of T1,T2,T5,T7 to PP', 6, N'a4d7b215-6490-4ea9-9bcb-4b1388f0bdb4', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (58, N'NC_MAC_MBD_Closed', N'Channel1.LoadShedding.Information Data Block.NC_MAC_MBD_Closed', N'PLC : MAC or MBD IS CLOSED', 17, N'292e10f2-5b5b-43ef-badd-1338e4cd25d3', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (59, N'NC_T3_T4_T6_Connected_PP', N'Channel1.LoadShedding.Information Data Block.NC_T3_T4_T6_Connected_PP', N'PLC : T3 or T4 or T6 Connected to PP, Check PP-Bus Connections', 6, N'e054ab45-052a-40c9-af4d-c0e2e990fbdb', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (60, N'PEL_NET1_NOD9_Disconnect', N'Channel1.LoadShedding.Information Data Block.PEL_NET1_NOD9_Disconnect', N'PLC : PEL-PROFIBUS 1 NOD 9 DISCONNECT', 6, N'25dadcdb-6bad-431f-addc-65b99f6afd77', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (61, N'PEL_NET2_NOD9_Disconnect', N'Channel1.LoadShedding.Information Data Block.PEL_NET2_NOD9_Disconnect', N'PLC : PEL-PROFIBUS 2 NOD 9 DISCONNECT', 6, N'b76e0b80-7481-47ad-9b8a-4413cbf4cf9b', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (62, N'PLC_LS_DISABLE', N'Channel1.LoadShedding.Information Data Block.PLC_LS_DISABLE', NULL, 6, N'ac4403cd-0a6c-4ad4-950d-e3b8bdd9cf2f', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (63, N'RED_NET1_NOD8_Disconnect', N'Channel1.LoadShedding.Information Data Block.RED_NET1_NOD8_Disconnect', N'PLC : RED-PROFIBUS 1 NOD 8 DISCONNECT', 6, N'527b69f4-be92-4149-8288-3196ac222235', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (64, N'RED_NET2_NOD8_Disconnect', N'Channel1.LoadShedding.Information Data Block.RED_NET2_NOD8_Disconnect', N'PLC : RED-PROFIBUS 2 NOD 8 DISCONNECT', 6, N'b12da4d5-395e-4d5c-b951-48609c4f12d9', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (65, N'CSM_24VDC_ET200', N'Channel1.LoadShedding.Information Data Block.CSM_24VDC_ET200', N'PLC : CSM-Monitoring 24 VDC Supply for ET200 Racks (F620-F622)', 17, N'a40ac10a-4bcf-42be-89aa-68fd0649b6d2', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (66, N'CSM_24VDC_ET200_PS3', N'Channel1.LoadShedding.Information Data Block.CSM_24VDC_ET200_PS3', N'PLC : CSM-Monitoring 24  VDC Power Supply for ET200 Racks (PS3)(G3)', 17, N'535a93ae-0722-469e-9233-755cdf65e174', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (67, N'CSM_24VDC_ET200_PS4', N'Channel1.LoadShedding.Information Data Block.CSM_24VDC_ET200_PS4', N'PLC : CSM-Monitoring 24  VDC Power Supply for ET200 Racks (PS4)(G4)', 17, N'6984afaa-1f74-420f-9036-c14073e84648', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (68, N'CSM_24VDC_Multiplyers', N'Channel1.LoadShedding.Information Data Block.CSM_24VDC_Multiplyers', N'PLC : CSM-Monitoring 24  VDC Supply for Multiplyers(F631-F634)', 17, N'54315d70-e508-428c-954f-b597a114e20c', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (69, N'CSM_C01_CB_Fault', N'Channel1.LoadShedding.Information Data Block.CSM_C01_CB_Fault', N'PLC : CB MALFUNCTION', 17, N'6d1b56c6-79b2-497f-84a9-e7e93418a3e2', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (70, N'CSM_C01B_CB', N'Channel1.LoadShedding.Information Data Block.CSM_C01B_CB', N'PLC : Tripped by PLC Load Shedding', 6, N'4368ca6c-d17c-4254-81cc-d55f214a0924', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (71, N'CSM_C01B_CB_Fault', N'Channel1.LoadShedding.Information Data Block.CSM_C01B_CB_Fault', N'PLC : CB MALFUNCTION', 17, N'21e4cf06-4636-44b4-944c-f9cf3a0829bf', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (72, N'CSM_C02_CB', N'Channel1.LoadShedding.Information Data Block.CSM_C02_CB', N'PLC : Tripped by PLC Load Shedding', 6, N'2eb9ab68-c1a0-48f5-8249-0cf17589c461', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (73, N'CSM_C02_CB_Fault', N'Channel1.LoadShedding.Information Data Block.CSM_C02_CB_Fault', N'PLC : CB MALFUNCTION', 17, N'6b6e092f-4b98-4902-8965-c3b302af5301', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (74, N'CSM_C02B_CB', N'Channel1.LoadShedding.Information Data Block.CSM_C02B_CB', N'PLC : Tripped by PLC Load Shedding', 6, N'0bba3486-12fb-4fd1-94cc-a158b4fb0894', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (75, N'CSM_C02B_CB_Fault', N'Channel1.LoadShedding.Information Data Block.CSM_C02B_CB_Fault', N'PLC : CB MALFUNCTION', 17, N'3dc6afe2-bf35-4024-a8d5-60accec82269', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (76, N'CSM_C03_CB', N'Channel1.LoadShedding.Information Data Block.CSM_C03_CB', N'PLC : Tripped by PLC Load Shedding', 6, N'16bfd6e9-8be9-42c5-b29e-4b7530244cdc', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (77, N'CSM_C03_CB_Fault', N'Channel1.LoadShedding.Information Data Block.CSM_C03_CB_Fault', N'PLC : CB MALFUNCTION', 17, N'dcbd34d5-7a60-45bc-8baa-fec3b93c20c7', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (78, N'CSM_C03B_CB', N'Channel1.LoadShedding.Information Data Block.CSM_C03B_CB', N'PLC : Tripped by PLC Load Shedding', 6, N'5d55d559-d4c1-48ff-b89c-f018f2a82fe7', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (79, N'CSM_C03B_CB_Fault', N'Channel1.LoadShedding.Information Data Block.CSM_C03B_CB_Fault', N'PLC : CB MALFUNCTION', 17, N'45fbb1c6-7bab-4e03-b733-2fe540e33b58', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (80, N'CSM_C04_CB', N'Channel1.LoadShedding.Information Data Block.CSM_C04_CB', N'PLC : Tripped by PLC Load Shedding', 6, N'61b1025e-9021-49e0-a568-53d5eae3437a', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (81, N'CSM_C04_CB_Fault', N'Channel1.LoadShedding.Information Data Block.CSM_C04_CB_Fault', N'PLC : CB MALFUNCTION', 17, N'3844f064-46b0-4049-8174-b00552af9043', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (82, N'CSM_C04B_CB', N'Channel1.LoadShedding.Information Data Block.CSM_C04B_CB', N'PLC : Tripped by PLC Load Shedding', 6, N'8674b120-e12d-4a8f-9298-201623d7e01a', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (83, N'CSM_C04B_CB_Fault', N'Channel1.LoadShedding.Information Data Block.CSM_C04B_CB_Fault', N'PLC : CB MALFUNCTION', 17, N'5b4d1f38-f9f6-4f78-9f77-746d0599d87a', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (84, N'CSM_C05_CB', N'Channel1.LoadShedding.Information Data Block.CSM_C05_CB', N'PLC : Tripped by PLC Load Shedding', 6, N'3ccc0a82-addc-4689-8eba-cd0136c6a949', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (85, N'CSM_C05_CB_Fault', N'Channel1.LoadShedding.Information Data Block.CSM_C05_CB_Fault', N'PLC : CB MALFUNCTION', 17, N'e28aa0b8-57c4-4ca1-8598-767963ae5681', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (86, N'CSM_C05B_CB', N'Channel1.LoadShedding.Information Data Block.CSM_C05B_CB', N'PLC : Tripped by PLC Load Shedding', 6, N'd90f37d3-3bc6-45bb-97bf-bfdce4aa4758', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (87, N'CSM_C05B_CB_Fault', N'Channel1.LoadShedding.Information Data Block.CSM_C05B_CB_Fault', N'PLC : CB MALFUNCTION', 17, N'f4aeddee-1d34-4a42-a796-5f095f024a74', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (88, N'CSM_C06B_CB', N'Channel1.LoadShedding.Information Data Block.CSM_C06B_CB', N'PLC : Tripped by PLC Load Shedding', 6, N'57d1c176-9447-47a9-816c-8eccabe42377', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (89, N'CSM_C06B_CB_Fault', N'Channel1.LoadShedding.Information Data Block.CSM_C06B_CB_Fault', N'PLC : CB MALFUNCTION', 17, N'bfb8de3b-7416-494f-a6c8-6e99a5d9a808', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (90, N'CSM_C07_CB', N'Channel1.LoadShedding.Information Data Block.CSM_C07_CB', N'PLC : Tripped by PLC Load Shedding', 6, N'9b6d51a0-d3dc-4f45-8358-db3ffced2522', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (91, N'CSM_C07_CB_Fault', N'Channel1.LoadShedding.Information Data Block.CSM_C07_CB_Fault', N'PLC : CB MALFUNCTION', 17, N'2500a5f4-db64-4f14-b1c9-456366af7e8d', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (92, N'CSM_C07B_CB', N'Channel1.LoadShedding.Information Data Block.CSM_C07B_CB', N'PLC : Tripped by PLC Load Shedding', 6, N'1e423f9c-7f2d-4aa3-b6ed-42b8ba62772c', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (93, N'CSM_C07B_CB_Fault', N'Channel1.LoadShedding.Information Data Block.CSM_C07B_CB_Fault', N'PLC : CB MALFUNCTION', 17, N'1a17851b-8942-4e42-bc4a-4a149133ffe9', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (94, N'CSM_C08_CB', N'Channel1.LoadShedding.Information Data Block.CSM_C08_CB', N'PLC : Tripped by PLC Load Shedding', 6, N'a4ac9406-ffe3-4afd-bed3-01c962d07b4f', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (95, N'CSM_C08_CB_Fault', N'Channel1.LoadShedding.Information Data Block.CSM_C08_CB_Fault', N'PLC : CB MALFUNCTION', 17, N'83907b46-f748-4206-b8dd-11593ab5a520', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (96, N'CSM_C09_CB', N'Channel1.LoadShedding.Information Data Block.CSM_C09_CB', N'PLC : Tripped by PLC Load Shedding', 6, N'9100e63a-fa38-42c4-913a-709aeeff9475', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (97, N'CSM_C09_CB_Fault', N'Channel1.LoadShedding.Information Data Block.CSM_C09_CB_Fault', N'PLC : CB MALFUNCTION', 17, N'da52acb2-c3ba-43d2-be15-aab76c99dc48', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (98, N'CSM_C10_CB', N'Channel1.LoadShedding.Information Data Block.CSM_C10_CB', N'PLC : Tripped by PLC Load Shedding', 6, N'2ec449c9-32c8-473f-982a-3ac35f5bda21', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (99, N'CSM_C10_CB_Fault', N'Channel1.LoadShedding.Information Data Block.CSM_C10_CB_Fault', N'PLC : CB MALFUNCTION', 17, N'af613e83-d047-4e5a-b551-dabd84421fd4', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (100, N'CSM_C11_CB', N'Channel1.LoadShedding.Information Data Block.CSM_C11_CB', N'PLC : Tripped by PLC Load Shedding', 6, N'7af8ba93-4d1d-4c08-bb94-e7c38a9bc821', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (101, N'CSM_C11_CB_Fault', N'Channel1.LoadShedding.Information Data Block.CSM_C11_CB_Fault', N'PLC : CB MALFUNCTION', 17, N'3b11a322-a3e3-4779-a7fd-a727a8375a1d', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (102, N'CSM_C15B_CB', N'Channel1.LoadShedding.Information Data Block.CSM_C15B_CB', N'PLC : Tripped by PLC Load Shedding', 6, N'a9c6ceb2-6fa8-4a3c-a50e-a96c671a290c', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (103, N'CSM_C15B_CB_Fault', N'Channel1.LoadShedding.Information Data Block.CSM_C15B_CB_Fault', N'PLC : CB MALFUNCTION', 17, N'7d8ed29a-b09a-4b9d-a21b-db3f57820f51', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (104, N'CSM_C16_CB', N'Channel1.LoadShedding.Information Data Block.CSM_C16_CB', N'PLC : Tripped by PLC Load Shedding', 6, N'e1235558-963b-4b50-bbc2-5cb12fbdf287', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (105, N'CSM_C16_CB_Fault', N'Channel1.LoadShedding.Information Data Block.CSM_C16_CB_Fault', N'PLC : CB MALFUNCTION', 17, N'28751525-12b1-4db5-aac5-25b7b162b573', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (106, N'CSM_C16B_CB', N'Channel1.LoadShedding.Information Data Block.CSM_C16B_CB', N'PLC : Tripped by PLC Load Shedding', 6, N'ae24a019-8296-4035-aed7-6e4b6057b413', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (107, N'CSM_C16B_CB_Fault', N'Channel1.LoadShedding.Information Data Block.CSM_C16B_CB_Fault', N'PLC : CB MALFUNCTION', 17, N'6656dda6-ca1c-4cfb-83a5-b0fc7590a8ad', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (108, N'CSM_C17_CB', N'Channel1.LoadShedding.Information Data Block.CSM_C17_CB', N'PLC : Tripped by PLC Load Shedding', 6, N'a16463e3-90f1-4a7f-b26a-2ce103f4893c', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (109, N'CSM_C17_CB_Fault', N'Channel1.LoadShedding.Information Data Block.CSM_C17_CB_Fault', N'PLC : CB MALFUNCTION', 17, N'49c1567b-edf3-4826-8963-cdaaaaf20580', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (110, N'CSM_C17B_CB', N'Channel1.LoadShedding.Information Data Block.CSM_C17B_CB', N'PLC : Tripped by PLC Load Shedding', 6, N'81a481dd-c770-431b-b645-2c997621cbc0', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (111, N'CSM_C17B_CB_Fault', N'Channel1.LoadShedding.Information Data Block.CSM_C17B_CB_Fault', N'PLC : CB MALFUNCTION', 17, N'cfcb6879-189b-41f1-8b2f-828fba884448', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (112, N'CSM_C18_CB', N'Channel1.LoadShedding.Information Data Block.CSM_C18_CB', N'PLC : Tripped by PLC Load Shedding', 6, N'c9ebde92-0d14-43f0-8c0e-364bf9c5c308', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (113, N'CSM_C18_CB_Fault', N'Channel1.LoadShedding.Information Data Block.CSM_C18_CB_Fault', N'PLC : CB MALFUNCTION', 17, N'b1ce5452-b324-4454-894f-6d13cdb24299', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (114, N'CSM_C18B_CB', N'Channel1.LoadShedding.Information Data Block.CSM_C18B_CB', N'PLC : Tripped by PLC Load Shedding', 6, N'6cd58109-33fc-4e20-8b63-4f40dde38760', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (115, N'CSM_C18B_CB_Fault', N'Channel1.LoadShedding.Information Data Block.CSM_C18B_CB_Fault', N'PLC : CB MALFUNCTION', 17, N'52ac2eb8-dfa7-4d85-91bf-349740aa5321', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (116, N'CSM_C19_CB', N'Channel1.LoadShedding.Information Data Block.CSM_C19_CB', N'PLC : Tripped by PLC Load Shedding', 6, N'b83671b3-3085-44aa-948b-79823bd00f4a', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (117, N'CSM_C19_CB_Fault', N'Channel1.LoadShedding.Information Data Block.CSM_C19_CB_Fault', N'PLC : CB MALFUNCTION', 17, N'2eba87e5-c0f5-4f4d-854c-44920e3d6f28', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (118, N'CSM_C19B_CB', N'Channel1.LoadShedding.Information Data Block.CSM_C19B_CB', N'PLC : Tripped by PLC Load Shedding', 6, N'64607fbe-4c0b-46d9-bba8-7b94659abdf3', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (119, N'CSM_C19B_CB_Fault', N'Channel1.LoadShedding.Information Data Block.CSM_C19B_CB_Fault', N'PLC : CB MALFUNCTION', 17, N'cb07eb67-ef97-4870-acab-6ee655298c80', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (120, N'CSM_C20_CB', N'Channel1.LoadShedding.Information Data Block.CSM_C20_CB', N'PLC : Tripped by PLC Load Shedding', 6, N'e29714af-a55f-4ffa-b4a3-e4ca91318c78', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (121, N'CSM_C20_CB_Fault', N'Channel1.LoadShedding.Information Data Block.CSM_C20_CB_Fault', N'PLC : CB MALFUNCTION', 17, N'4833d15e-bf5c-478c-a071-a31e9081edde', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (122, N'CSM_C20B_CB', N'Channel1.LoadShedding.Information Data Block.CSM_C20B_CB', N'PLC : Tripped by PLC Load Shedding', 6, N'3a7f8b7f-3cbb-4345-a99a-9a97d65748e2', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (123, N'CSM_C20B_CB_Fault', N'Channel1.LoadShedding.Information Data Block.CSM_C20B_CB_Fault', N'PLC : CB MALFUNCTION', 17, N'65db4d8f-d031-40e6-be9e-58290d6d96cd', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (124, N'CSM_C22_CB', N'Channel1.LoadShedding.Information Data Block.CSM_C22_CB', N'PLC : Tripped by PLC Load Shedding', 6, N'155de9ef-ca53-4df7-bde1-99f1be27b2e9', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (125, N'CSM_C22_CB_Fault', N'Channel1.LoadShedding.Information Data Block.CSM_C22_CB_Fault', N'PLC : CB MALFUNCTION', 17, N'30c96de2-552d-4795-8bd0-293fadebaeaa', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (126, N'CSM_C23_CB', N'Channel1.LoadShedding.Information Data Block.CSM_C23_CB', N'PLC : Tripped by PLC Load Shedding', 6, N'24608c8d-dabc-45c7-9e61-022f13957edf', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (127, N'CSM_C23_CB_Fault', N'Channel1.LoadShedding.Information Data Block.CSM_C23_CB_Fault', N'PLC : CB MALFUNCTION', 17, N'e97579c5-96c1-4f9f-82b4-dd9b939915e7', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (128, N'CSM_C24_CB', N'Channel1.LoadShedding.Information Data Block.CSM_C24_CB', N'PLC : Tripped by PLC Load Shedding', 6, N'e7ef52b4-2e79-4ac6-8249-32db02303bcb', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (129, N'CSM_C24_CB_Fault', N'Channel1.LoadShedding.Information Data Block.CSM_C24_CB_Fault', N'PLC : CB MALFUNCTION', 17, N'2805ae45-c225-4f4c-b518-e919f1ca6614', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (130, N'CSM_C25_CB', N'Channel1.LoadShedding.Information Data Block.CSM_C25_CB', N'PLC : Tripped by PLC Load Shedding', 6, N'e24e3805-4185-4c8e-bbe2-1004a9ee3572', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (131, N'CSM_C25_CB_Fault', N'Channel1.LoadShedding.Information Data Block.CSM_C25_CB_Fault', N'PLC : CB MALFUNCTION', 17, N'6029ba15-7a25-4dc4-9cfa-8da825979211', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (132, N'CSM_C26_CB', N'Channel1.LoadShedding.Information Data Block.CSM_C26_CB', N'PLC : Tripped by PLC Load Shedding', 6, N'b6bc4b4c-8fb6-4052-81f0-b2f4f8977b89', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (133, N'CSM_C26_CB_Fault', N'Channel1.LoadShedding.Information Data Block.CSM_C26_CB_Fault', N'PLC : CB MALFUNCTION', 17, N'745de0cd-52b5-46a9-9f24-396602b041ed', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (134, N'CSM_C5P_CB', N'Channel1.LoadShedding.Information Data Block.CSM_C5P_CB', N'PLC : Tripped by PLC Load Shedding', 6, N'53de7149-98d9-4cdf-9a24-9b6eb6204297', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (135, N'CSM_C5P_CB_Fault', N'Channel1.LoadShedding.Information Data Block.CSM_C5P_CB_Fault', N'PLC : CB MALFUNCTION', 17, N'8dc30dcd-dee1-4a63-8ecb-ab00cffbf814', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (136, N'CSM_C6P_CB', N'Channel1.LoadShedding.Information Data Block.CSM_C6P_CB', N'PLC : Tripped by PLC Load Shedding', 6, N'5579b0e1-e5ec-46d4-8b65-7ed6cfd0ae2d', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (137, N'CSM_C6P_CB_Fault', N'Channel1.LoadShedding.Information Data Block.CSM_C6P_CB_Fault', N'PLC : CB MALFUNCTION', 17, N'17437e78-0b41-46ab-b98b-842d99f80e0d', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (138, N'CSM_CFT_CB', N'Channel1.LoadShedding.Information Data Block.CSM_CFT_CB', N'PLC : Tripped by PLC Load Shedding', 6, N'57d5d22e-fd92-45b3-bf08-f194cf36b46c', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (139, N'CSM_CFT_CB_Fault', N'Channel1.LoadShedding.Information Data Block.CSM_CFT_CB_Fault', N'PLC : CB MALFUNCTION', 17, N'0c04a7c4-583b-4ff8-9b18-303bff6b2afd', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (140, N'CSM_M_0VDC_DO_relays', N'Channel1.LoadShedding.Information Data Block.CSM_M_0VDC_DO_relays', N'PLC : CSM-Monitoring M- (0 VDC) for load to be trip Do Relays(F701-F704)', 17, N'c8701373-a0ec-4ce0-90b2-5b93ec0ce6d6', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (141, N'EFS_110VDC_Battry_Charge', N'Channel1.LoadShedding.Information Data Block.EFS_110VDC_Battry_Charge', N'PLC : EFS-Monitoring 220 VAC Infeed MCB OK (F300)', 17, N'9e4820fc-c581-4ecb-9aa4-342353cdd9cd', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (142, N'EFS_220VAC_MCB', N'Channel1.LoadShedding.Information Data Block.EFS_220VAC_MCB', N'PLC : EFS-Monitoring 24  VDC Supply for Multiplyers(F631-F633)', 17, N'a6e3ae35-709e-4960-ae38-b311519676a1', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (143, N'EFS_24VDC_AnalogInput', N'Channel1.LoadShedding.Information Data Block.EFS_24VDC_AnalogInput', N'PLC : EFS-Monitoring 24  VDC Supply for Analog Input Cards (F601-602)', 17, N'ffa0a36f-68da-47f3-ab2f-677abf377940', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (144, N'EFS_24VDC_Card_Site_PS1', N'Channel1.LoadShedding.Information Data Block.EFS_24VDC_Card_Site_PS1', N'PLC : EFS-Monitoring 24  VDC Power Supply for Cards _ Site (PS1)(G1)', 17, N'4e161a06-d705-4dfe-800a-fb989141bde8', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (145, N'EFS_24VDC_Card_Site_PS2', N'Channel1.LoadShedding.Information Data Block.EFS_24VDC_Card_Site_PS2', N'PLC : EFS-Monitoring 24  VDC Power Supply for Cards _ Site (PS2)(G2)', 17, N'aee929ce-aacb-44b0-b085-849c3f81ddf6', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (146, N'EFS_24VDC_DI_1400_1401', N'Channel1.LoadShedding.Information Data Block.EFS_24VDC_DI_1400_1401', N'PLC : EFS-Monitoring 24 VDC Common for DI Signals (I1400-I1401)(FT1/1)', 17, N'c0ca04dc-c85c-4194-ac3c-578c69fed2c1', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (147, N'EFS_24VDC_DI_1402_1403', N'Channel1.LoadShedding.Information Data Block.EFS_24VDC_DI_1402_1403', N'PLC : EFS-Monitoring 24 VDC Common for DI Signals (I1402-I1403)(FT1/2)', 17, N'73499aa0-ec36-4234-bb04-4a4de993379d', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (148, N'EFS_24VDC_DI_1404_1405', N'Channel1.LoadShedding.Information Data Block.EFS_24VDC_DI_1404_1405', N'PLC : EFS-Monitoring 24 VDC Common for DI Signals (I1404-I1405)(FT1/3)', 17, N'3b0ddc3c-38a5-4507-b792-5195e2a5f559', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (149, N'EFS_24VDC_DI_1406_1407', N'Channel1.LoadShedding.Information Data Block.EFS_24VDC_DI_1406_1407', N'PLC : EFS-Monitoring 24 VDC Common for DI Signals (I1406-I1407)(FT1/4)', 17, N'7af5df2d-2c32-4bc5-996e-557e329629b9', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (150, N'EFS_24VDC_DI_1445_1446', N'Channel1.LoadShedding.Information Data Block.EFS_24VDC_DI_1445_1446', N'PLC : EFS-Monitoring 24 VDC Common for DI Signals (I1445-I1446)(FT1/5)', 17, N'3ceb5f62-3b08-4f38-8862-a2cfbb0b6ec6', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (151, N'EFS_24VDC_DI_1447_1448', N'Channel1.LoadShedding.Information Data Block.EFS_24VDC_DI_1447_1448', N'PLC : EFS-Monitoring 24 VDC Common for DI Signals (I1447-I1448)(FT1/6)', 17, N'5598d488-5569-4062-a268-e9eb7126f54a', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (152, N'EFS_24VDC_Digital_Output', N'Channel1.LoadShedding.Information Data Block.EFS_24VDC_Digital_Output', N'PLC : EFS-Monitoring 24  VDC Supply for Digital Output Cards (F603-F609)', 17, N'fbafda51-dced-403b-b6ec-e330d9d63804', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (153, N'EFS_24VDC_ET200', N'Channel1.LoadShedding.Information Data Block.EFS_24VDC_ET200', N'PLC : EFS-Monitoring 24 VDC Supply for ET200 Racks (F615-F617)', 17, N'b1de04d6-0945-49a9-9f90-f8f994050dcb', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (154, N'EFS_24VDC_ET200_PS3', N'Channel1.LoadShedding.Information Data Block.EFS_24VDC_ET200_PS3', N'PLC : EFS-Monitoring 24  VDC Power Supply for ET200 Racks (PS3)(G3)', 17, N'01082ca0-924b-42c4-bbe9-a5020de6ca44', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (155, N'EFS_24VDC_ET200_PS4', N'Channel1.LoadShedding.Information Data Block.EFS_24VDC_ET200_PS4', N'PLC : EFS-Monitoring 24  VDC Power Supply for ET200 Racks (PS4)(G4)', 17, N'f60a6f37-bf8e-4cb8-8b8f-18dc1957c428', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (156, N'EFS_24VDC_Multiplyers', N'Channel1.LoadShedding.Information Data Block.EFS_24VDC_Multiplyers', N'PLC : EFS-Monitoring 24  VDC Supply for Multiplyers(F631-F633)', 17, N'787fb371-467b-47eb-8f3d-0fcadc7a294a', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (157, N'EFS_C02_CB', N'Channel1.LoadShedding.Information Data Block.EFS_C02_CB', N'PLC : Tripped by PLC Load Shedding', 6, N'efa3070c-6ff1-4bb4-8feb-d2730d077b22', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (158, N'EFS_C02_CB_Fault', N'Channel1.LoadShedding.Information Data Block.EFS_C02_CB_Fault', N'PLC : CB MALFUNCTION', 17, N'f6f60dc5-595f-4004-900d-69c610f80563', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (159, N'EFS_C03_CB', N'Channel1.LoadShedding.Information Data Block.EFS_C03_CB', N'PLC : Tripped by PLC Load Shedding', 6, N'2d6e1fc4-de97-4e69-9196-0af8a1077f83', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (160, N'EFS_C03_CB_Fault', N'Channel1.LoadShedding.Information Data Block.EFS_C03_CB_Fault', N'PLC : CB MALFUNCTION', 17, N'060b3b1c-a9b3-4a8c-bced-8c4000753e11', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (161, N'EFS_C04_CB', N'Channel1.LoadShedding.Information Data Block.EFS_C04_CB', N'PLC : Tripped by PLC Load Shedding', 6, N'4db83f44-4ef1-4744-9337-a73114fc5a6f', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (162, N'EFS_C04_CB_Fault', N'Channel1.LoadShedding.Information Data Block.EFS_C04_CB_Fault', N'PLC : CB MALFUNCTION', 17, N'da33aada-01cc-40de-a39c-d846da0f12c0', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (163, N'EFS_C05_CB', N'Channel1.LoadShedding.Information Data Block.EFS_C05_CB', N'PLC : Tripped by PLC Load Shedding', 6, N'c361e672-1c80-4454-b9aa-dcf5da9283c8', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (164, N'EFS_C05_CB_Fault', N'Channel1.LoadShedding.Information Data Block.EFS_C05_CB_Fault', N'PLC : CB MALFUNCTION', 17, N'895ced0f-3890-4aed-aa18-851e1ab19868', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (165, N'EFS_C06_CB', N'Channel1.LoadShedding.Information Data Block.EFS_C06_CB', N'PLC : Tripped by PLC Load Shedding', 6, N'3205d937-b56a-4dc6-a134-8f01a0464ae8', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (166, N'EFS_C06_CB_Fault', N'Channel1.LoadShedding.Information Data Block.EFS_C06_CB_Fault', N'PLC : CB MALFUNCTION', 17, N'c02db15f-e4c4-4347-9054-0718491ed310', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (167, N'EFS_C08_CB', N'Channel1.LoadShedding.Information Data Block.EFS_C08_CB', N'PLC : Tripped by PLC Load Shedding', 6, N'651d70ed-0157-40b3-828d-3e134964cbe3', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (168, N'EFS_C08_CB_Fault', N'Channel1.LoadShedding.Information Data Block.EFS_C08_CB_Fault', N'PLC : CB MALFUNCTION', 17, N'2c9200b4-cf5a-4da7-bfe3-c81f42cfc3dc', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (169, N'EFS_C09_CB', N'Channel1.LoadShedding.Information Data Block.EFS_C09_CB', N'PLC : Tripped by PLC Load Shedding', 6, N'a48070b4-72a9-467b-973d-487e47dae0cd', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (170, N'EFS_C09_CB_Fault', N'Channel1.LoadShedding.Information Data Block.EFS_C09_CB_Fault', N'PLC : CB MALFUNCTION', 17, N'c5fccd3c-c5ca-4ebd-bce4-de061d182b03', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (171, N'EFS_C10_CB', N'Channel1.LoadShedding.Information Data Block.EFS_C10_CB', N'PLC : Tripped by PLC Load Shedding', 6, N'958930e5-da13-49c2-ac6c-49dba9a62647', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (172, N'EFS_C10_CB_Fault', N'Channel1.LoadShedding.Information Data Block.EFS_C10_CB_Fault', N'PLC : CB MALFUNCTION', 17, N'3c4074d5-4e32-42b1-bc57-1bb67b7030e7', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (173, N'EFS_C11_CB', N'Channel1.LoadShedding.Information Data Block.EFS_C11_CB', N'PLC : Tripped by PLC Load Shedding', 6, N'488740c9-337a-4c35-806f-187a93fe7a57', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (174, N'EFS_C11_CB_Fault', N'Channel1.LoadShedding.Information Data Block.EFS_C11_CB_Fault', N'PLC : CB MALFUNCTION', 17, N'3aae4541-4f09-4836-8684-8226ffe17ca4', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (175, N'EFS_C12_CB', N'Channel1.LoadShedding.Information Data Block.EFS_C12_CB', N'PLC : Tripped by PLC Load Shedding', 6, N'0ff63274-b71c-4467-aa08-a862bc5c8554', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (176, N'EFS_C12_CB_Fault', N'Channel1.LoadShedding.Information Data Block.EFS_C12_CB_Fault', N'PLC : CB MALFUNCTION', 17, N'62d3e95f-ccb7-4f67-9e66-c77efa38b9d7', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (177, N'EFS_C13_CB', N'Channel1.LoadShedding.Information Data Block.EFS_C13_CB', N'PLC : Tripped by PLC Load Shedding', 6, N'f72faa44-89e8-4f2d-9ffe-13786e6c8243', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (178, N'EFS_C13_CB_Fault', N'Channel1.LoadShedding.Information Data Block.EFS_C13_CB_Fault', N'PLC : CB MALFUNCTION', 17, N'e8235bca-50c5-4ddd-8c0c-9c6b0261d204', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (179, N'EFS_C19_CB', N'Channel1.LoadShedding.Information Data Block.EFS_C19_CB', N'PLC : Tripped by PLC Load Shedding', 6, N'3462f906-f080-451b-9c11-b24955054f42', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (180, N'EFS_C19_CB_Fault', N'Channel1.LoadShedding.Information Data Block.EFS_C19_CB_Fault', N'PLC : CB MALFUNCTION', 17, N'5047a813-3376-4500-9574-292f6d8149c7', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (181, N'EFS_C20_CB', N'Channel1.LoadShedding.Information Data Block.EFS_C20_CB', N'PLC : Tripped by PLC Load Shedding', 6, N'2eb6fc3d-474d-4f17-870d-072930160ab0', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (182, N'EFS_C20_CB_Fault', N'Channel1.LoadShedding.Information Data Block.EFS_C20_CB_Fault', N'PLC : CB MALFUNCTION', 17, N'57ff689d-d00b-49c8-9845-b000e93eb11f', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (183, N'EFS_C21_CB', N'Channel1.LoadShedding.Information Data Block.EFS_C21_CB', N'PLC : Tripped by PLC Load Shedding', 6, N'1d5a17cb-3e5d-4644-9f1f-96572f20e7de', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (184, N'EFS_C21_CB_Fault', N'Channel1.LoadShedding.Information Data Block.EFS_C21_CB_Fault', N'PLC : CB MALFUNCTION', 17, N'1cf853ce-4e30-4ebe-befa-8d507c81c4c1', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (185, N'EFS_C22_CB', N'Channel1.LoadShedding.Information Data Block.EFS_C22_CB', N'PLC : Tripped by PLC Load Shedding', 6, N'44410fa4-e655-4aed-9118-02ec3c970879', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (186, N'EFS_C22_CB_Fault', N'Channel1.LoadShedding.Information Data Block.EFS_C22_CB_Fault', N'PLC : CB MALFUNCTION', 17, N'15e9cd03-3241-40f4-adc8-86b01c0a4150', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (187, N'EFS_C23_CB', N'Channel1.LoadShedding.Information Data Block.EFS_C23_CB', N'PLC : Tripped by PLC Load Shedding', 6, N'db36ea8c-192c-43c6-9fed-96c83974adc7', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (188, N'EFS_C23_CB_Fault', N'Channel1.LoadShedding.Information Data Block.EFS_C23_CB_Fault', N'PLC : CB MALFUNCTION', 17, N'162c05c1-44ec-4eba-8ac7-4b74347276ae', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (189, N'EFS_C25_CB', N'Channel1.LoadShedding.Information Data Block.EFS_C25_CB', N'PLC : Tripped by PLC Load Shedding', 6, N'681232fc-7063-4a0b-b60e-8014ce20f743', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (190, N'EFS_C25_CB_Fault', N'Channel1.LoadShedding.Information Data Block.EFS_C25_CB_Fault', N'PLC : CB MALFUNCTION', 17, N'd5744bf1-ae02-4e02-aaa1-42168698243a', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (191, N'EFS_C26_CB', N'Channel1.LoadShedding.Information Data Block.EFS_C26_CB', N'PLC : Tripped by PLC Load Shedding', 6, N'd77662f0-f630-4f98-9566-f0473199f42e', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (192, N'EFS_C26_CB_Fault', N'Channel1.LoadShedding.Information Data Block.EFS_C26_CB_Fault', N'PLC : CB MALFUNCTION', 17, N'228dbebc-9309-4918-bb7e-d8760ea4f5c3', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (193, N'EFS_C27_CB', N'Channel1.LoadShedding.Information Data Block.EFS_C27_CB', N'PLC : Tripped by PLC Load Shedding', 6, N'f42e5f3c-e69f-4f2e-ad21-17627928b38d', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (194, N'EFS_C27_CB_Fault', N'Channel1.LoadShedding.Information Data Block.EFS_C27_CB_Fault', N'PLC : CB MALFUNCTION', 17, N'ac5e63a8-5e3d-47fa-a927-47f1a3373d97', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (195, N'EFS_C28_CB', N'Channel1.LoadShedding.Information Data Block.EFS_C28_CB', N'PLC : Tripped by PLC Load Shedding', 6, N'829c37a1-a823-40cb-9c72-bf7da50d5488', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (196, N'EFS_C28_CB_Fault', N'Channel1.LoadShedding.Information Data Block.EFS_C28_CB_Fault', N'PLC : CB MALFUNCTION', 17, N'3c141640-1e51-4bc1-b4f4-2a368ba57c30', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (197, N'EFS_C29_CB', N'Channel1.LoadShedding.Information Data Block.EFS_C29_CB', N'PLC : Tripped by PLC Load Shedding', 6, N'de039e84-5d99-4556-bc4e-68a08a444cc9', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (198, N'EFS_C29_CB_Fault', N'Channel1.LoadShedding.Information Data Block.EFS_C29_CB_Fault', N'PLC : CB MALFUNCTION', 17, N'fe3f6e9b-a2a1-4075-b0f4-c8e2248d585f', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (199, N'EFS_C35_CB', N'Channel1.LoadShedding.Information Data Block.EFS_C35_CB', N'PLC : Tripped by PLC Load Shedding', 6, N'439ecd37-1617-4ccd-bec5-5b0b46303316', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (200, N'EFS_C35_CB_Fault', N'Channel1.LoadShedding.Information Data Block.EFS_C35_CB_Fault', N'PLC : CB MALFUNCTION', 17, N'd6600311-8fce-4bd4-8e72-9a5fe4002e7c', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (201, N'EFS_C36_CB', N'Channel1.LoadShedding.Information Data Block.EFS_C36_CB', N'PLC : Tripped by PLC Load Shedding', 6, N'72ea9f91-7a3b-4bb4-9518-4c33edf37d3b', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (202, N'EFS_C36_CB_Fault', N'Channel1.LoadShedding.Information Data Block.EFS_C36_CB_Fault', N'PLC : CB MALFUNCTION', 17, N'a595b82a-a66a-4c16-bea3-968bb9b7d3c1', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (203, N'EFS_C43_CB', N'Channel1.LoadShedding.Information Data Block.EFS_C43_CB', N'PLC : Tripped by PLC Load Shedding', 6, N'c03b0505-66c0-4b0b-b6c6-ff8a8cf4c4d4', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (204, N'EFS_C43_CB_Fault', N'Channel1.LoadShedding.Information Data Block.EFS_C43_CB_Fault', N'PLC : CB MALFUNCTION', 17, N'd6691a4b-3128-469f-a29f-9a944ae6649c', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (205, N'EFS_C48_CB', N'Channel1.LoadShedding.Information Data Block.EFS_C48_CB', N'PLC : Tripped by PLC Load Shedding', 6, N'dc7dbdca-e2d0-4d80-bab7-8b1b6baa6513', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (206, N'EFS_C48_CB_Fault', N'Channel1.LoadShedding.Information Data Block.EFS_C48_CB_Fault', N'PLC : CB MALFUNCTION', 17, N'64d8a446-59d2-4871-875f-351386cf904e', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (207, N'EFS_C49_CB', N'Channel1.LoadShedding.Information Data Block.EFS_C49_CB', N'PLC : Tripped by PLC Load Shedding', 6, N'02a54402-f73a-4096-8fe1-e204466108e5', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (208, N'EFS_C49_CB_Fault', N'Channel1.LoadShedding.Information Data Block.EFS_C49_CB_Fault', N'PLC : CB MALFUNCTION', 17, N'565f12d9-c325-4b6a-9a5e-0d2701336e7f', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (209, N'EFS_C56_CB', N'Channel1.LoadShedding.Information Data Block.EFS_C56_CB', N'PLC : Tripped by PLC Load Shedding', 6, N'6faeb2aa-f536-4770-a5c5-f86130abcfc8', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (210, N'EFS_C56_CB_Fault', N'Channel1.LoadShedding.Information Data Block.EFS_C56_CB_Fault', N'PLC : CB MALFUNCTION', 17, N'cd28d4b1-afe5-44f2-8453-8e1a1cf489bc', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (211, N'EFS_E5P_CB', N'Channel1.LoadShedding.Information Data Block.EFS_E5P_CB', N'PLC : Tripped by PLC Load Shedding', 6, N'96c4b64a-6402-4983-bdb9-345f46eded4b', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (212, N'EFS_E5P_CB_Fault', N'Channel1.LoadShedding.Information Data Block.EFS_E5P_CB_Fault', N'PLC : CB MALFUNCTION', 17, N'7395b0de-6bd1-4e50-8fda-7c691808768a', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (213, N'EFS_M_0VDC_DO_relays', N'Channel1.LoadShedding.Information Data Block.EFS_M_0VDC_DO_relays', N'PLC : EFS-Monitoring M- (0 VDC) for load to be trip Do Relays(F701-F703)', 17, N'088e605d-068c-4991-98cb-abb70ee3d809', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (214, N'EFSB_110VDC_Battry_Charg', N'Channel1.LoadShedding.Information Data Block.EFSB_110VDC_Battry_Charg', N'PLC : EFSB-Monitoring 110 VDC Battry Charger Infeed MCBs OK (G1)(F201,F202)', 17, N'34aacc8f-209c-4163-8623-186d99c2d8f0', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (215, N'EFSB_220VAC_MCB', N'Channel1.LoadShedding.Information Data Block.EFSB_220VAC_MCB', N'PLC : EFSB-Monitoring 220 VAC Infeed MCB OK (F300)', 17, N'1d4186d0-49df-41a5-bda1-f1ac148b720a', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (216, N'EFSB_24VDC_Cards', N'Channel1.LoadShedding.Information Data Block.EFSB_24VDC_Cards', N'PLC : EFSB-Monitoring 24  VDC  Supply for Cards  (F601-610)', 17, N'795e7a36-4e31-489d-a5df-8bd53feecf00', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (217, N'EFSB_24VDC_DI_1408_1409', N'Channel1.LoadShedding.Information Data Block.EFSB_24VDC_DI_1408_1409', N'PLC : EFSB-Monitoring 24 VDC Common for DI Signals (I1408-I1409)(FT1/1)', 17, N'481247b7-7471-48f9-a925-6c3b7dc767e1', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (218, N'EFSB_24VDC_DI_1410_1411', N'Channel1.LoadShedding.Information Data Block.EFSB_24VDC_DI_1410_1411', N'PLC : EFSB-Monitoring 24 VDC Common for DI Signals (I1410-I1411)(FT1/2)', 17, N'498b4438-2a06-483e-a1c5-ca42d46264b8', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (219, N'EFSB_24VDC_DI_1435_1436', N'Channel1.LoadShedding.Information Data Block.EFSB_24VDC_DI_1435_1436', N'PLC : EFSB-Monitoring 24 VDC Common for DI Signals (I1435-I1436)(FT1/3)', 17, N'75f6cad4-251a-404d-9ec7-bca14f63acf7', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (220, N'EFSB_24VDC_DI_1437_1438', N'Channel1.LoadShedding.Information Data Block.EFSB_24VDC_DI_1437_1438', N'PLC : EFSB-Monitoring 24 VDC Common for DI Signals (I1437-I1438)(FT1/4)', 17, N'3b3ee520-f08c-4b99-97a6-e157deb28f07', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (221, N'EFSB_24VDC_Multiplyers', N'Channel1.LoadShedding.Information Data Block.EFSB_24VDC_Multiplyers', N'PLC : EFSB-Monitoring 24  VDC Supply for Multiplyers(F611-F613)', 17, N'3e692cd2-3360-499c-b17e-2310b373946e', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (222, N'EFSB_C60_CB', N'Channel1.LoadShedding.Information Data Block.EFSB_C60_CB', N'PLC : Tripped by PLC Load Shedding', 6, N'6921c1ae-40d5-43bf-b702-ac9725f679d6', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (223, N'EFSB_C60_CB_Fault', N'Channel1.LoadShedding.Information Data Block.EFSB_C60_CB_Fault', N'PLC : CB MALFUNCTION', 17, N'a64d5283-2482-4add-838d-76f4311e85ab', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (224, N'EFSB_C62_CB', N'Channel1.LoadShedding.Information Data Block.EFSB_C62_CB', N'PLC : Tripped by PLC Load Shedding', 6, N'61deda67-dccb-4b62-8d5c-aa89de2e6076', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (225, N'EFSB_C62_CB_Fault', N'Channel1.LoadShedding.Information Data Block.EFSB_C62_CB_Fault', N'PLC : CB MALFUNCTION', 17, N'deb5deed-2002-45d3-afba-70d590ae373d', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (226, N'EFSB_C64_CB', N'Channel1.LoadShedding.Information Data Block.EFSB_C64_CB', N'PLC : Tripped by PLC Load Shedding', 6, N'691e7a24-0483-4fd1-8aa4-bc633c473d71', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (227, N'EFSB_C64_CB_Fault', N'Channel1.LoadShedding.Information Data Block.EFSB_C64_CB_Fault', N'PLC : CB MALFUNCTION', 17, N'776f0cf6-c4a7-457a-83ce-3dfc1c797644', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (228, N'EFSB_C65_CB', N'Channel1.LoadShedding.Information Data Block.EFSB_C65_CB', N'PLC : Tripped by PLC Load Shedding', 6, N'b0a38164-fc01-43d2-bf41-03a82eb4b9f8', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (229, N'EFSB_C65_CB_Fault', N'Channel1.LoadShedding.Information Data Block.EFSB_C65_CB_Fault', N'PLC : CB MALFUNCTION', 17, N'8116fd1b-87e7-40fa-884f-d1305e005039', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (230, N'EFSB_C66_CB', N'Channel1.LoadShedding.Information Data Block.EFSB_C66_CB', N'PLC : Tripped by PLC Load Shedding', 6, N'd410bf8c-4361-4957-8a65-6f4538c3071b', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (231, N'EFSB_C66_CB_Fault', N'Channel1.LoadShedding.Information Data Block.EFSB_C66_CB_Fault', N'PLC : CB MALFUNCTION', 17, N'1ec3ca5a-925e-4dc9-a726-d97747108d0e', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (232, N'EFSB_C67_CB', N'Channel1.LoadShedding.Information Data Block.EFSB_C67_CB', N'PLC : Tripped by PLC Load Shedding', 6, N'd1761dc3-e402-470b-9a58-a89439cbc444', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (233, N'EFSB_C67_CB_Fault', N'Channel1.LoadShedding.Information Data Block.EFSB_C67_CB_Fault', N'PLC : CB MALFUNCTION', 17, N'ffd27b21-661a-4cfa-8726-2a1386fb8f09', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (234, N'EFSB_C73_CB', N'Channel1.LoadShedding.Information Data Block.EFSB_C73_CB', N'PLC : Tripped by PLC Load Shedding', 6, N'4319d711-b584-4f71-8894-3e2f2c6a2a56', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (235, N'EFSB_C73_CB_Fault', N'Channel1.LoadShedding.Information Data Block.EFSB_C73_CB_Fault', N'PLC : CB MALFUNCTION', 17, N'048cae49-9c13-4433-a22d-f64b5635f6db', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (236, N'EFSB_C74_CB', N'Channel1.LoadShedding.Information Data Block.EFSB_C74_CB', N'PLC : Tripped by PLC Load Shedding', 6, N'2aa10af2-13a7-45f8-a013-de9d85d6d684', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (237, N'EFSB_C74_CB_Fault', N'Channel1.LoadShedding.Information Data Block.EFSB_C74_CB_Fault', N'PLC : CB MALFUNCTION', 17, N'e27cbf97-54f2-45ba-8acd-e307dfdc2729', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (238, N'EFSB_C75_CB', N'Channel1.LoadShedding.Information Data Block.EFSB_C75_CB', N'PLC : Tripped by PLC Load Shedding', 6, N'546a894d-0037-4b90-b488-f8481cbca5de', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (239, N'EFSB_C75_CB_Fault', N'Channel1.LoadShedding.Information Data Block.EFSB_C75_CB_Fault', N'PLC : CB MALFUNCTION', 17, N'83ff28a6-bb98-4ba8-a5d3-16696a655508', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (240, N'EFSB_C76_CB', N'Channel1.LoadShedding.Information Data Block.EFSB_C76_CB', N'PLC : Tripped by PLC Load Shedding', 6, N'ddccaa48-1686-4597-8f76-d13ae221f53b', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (241, N'EFSB_C76_CB_Fault', N'Channel1.LoadShedding.Information Data Block.EFSB_C76_CB_Fault', N'PLC : CB MALFUNCTION', 17, N'e15ed3c4-9fca-4db7-8c83-e9e3e6a737b2', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (242, N'EFSB_C78_CB', N'Channel1.LoadShedding.Information Data Block.EFSB_C78_CB', N'PLC : Tripped by PLC Load Shedding', 6, N'db30e7e7-1875-409c-94f2-2c787d6d3fb9', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (243, N'EFSB_C78_CB_Fault', N'Channel1.LoadShedding.Information Data Block.EFSB_C78_CB_Fault', N'PLC : CB MALFUNCTION', 17, N'8f10328b-a3bb-418d-8e20-c4cac4ff91ea', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (244, N'EFSB_C80_CB', N'Channel1.LoadShedding.Information Data Block.EFSB_C80_CB', N'PLC : Tripped by PLC Load Shedding', 6, N'80623c01-c7fc-4f16-b4ff-8c009a0ec053', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (245, N'EFSB_C80_CB_Fault', N'Channel1.LoadShedding.Information Data Block.EFSB_C80_CB_Fault', N'PLC : CB MALFUNCTION', 17, N'34f74875-05b6-43a5-af8e-6e7c91fc45c9', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (246, N'EFSB_M_0VDC_DO_relays', N'Channel1.LoadShedding.Information Data Block.EFSB_M_0VDC_DO_relays', N'PLC : EFSB-Monitoring M- (0 VDC) for load to be trip Do Relays(F701-F702)', 17, N'bdd3d49d-db4c-47fd-96a1-a811bd244cbc', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (247, N'EFSB_RES6_CB', N'Channel1.LoadShedding.Information Data Block.EFSB_RES6_CB', N'PLC : Tripped by PLC Load Shedding', 6, N'552d4992-75e2-474b-88e6-bc36d64553fa', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (248, N'EFSB_RES6_CB_Fault', N'Channel1.LoadShedding.Information Data Block.EFSB_RES6_CB_Fault', N'PLC : CB MALFUNCTION', 17, N'f1d38d9a-e2a8-48f1-b03d-c96ab45ff36d', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (249, N'EFSB_RES7_CB', N'Channel1.LoadShedding.Information Data Block.EFSB_RES7_CB', N'PLC : Tripped by PLC Load Shedding', 6, N'60283d52-bed5-4afe-be0f-aaa94e4eab61', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (250, N'EFSB_RES7_CB_Fault', N'Channel1.LoadShedding.Information Data Block.EFSB_RES7_CB_Fault', N'PLC : CB MALFUNCTION', 17, N'58253a22-8926-4159-8855-7bd512df63db', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (251, N'EFSB_RES8_CB', N'Channel1.LoadShedding.Information Data Block.EFSB_RES8_CB', N'PLC : Tripped by PLC Load Shedding', 6, N'324a84df-b84c-4e3b-873b-f74779848a64', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (252, N'EFSB_RES8_CB_Fault', N'Channel1.LoadShedding.Information Data Block.EFSB_RES8_CB_Fault', N'PLC : CB MALFUNCTION', 17, N'64b5c9bd-4db0-484d-b702-01cd84081c55', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (253, N'EFSB_RES9_CB', N'Channel1.LoadShedding.Information Data Block.EFSB_RES9_CB', N'PLC : Tripped by PLC Load Shedding', 6, N'6732c929-93af-452c-9f7a-bbd87051d77d', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (254, N'EFSB_RES9_CB_Fault', N'Channel1.LoadShedding.Information Data Block.EFSB_RES9_CB_Fault', N'PLC : CB MALFUNCTION', 17, N'9d319235-f7ad-455b-9b42-d43533b5f1f4', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (255, N'HSM_110VDC_Battry_Charge', N'Channel1.LoadShedding.Information Data Block.HSM_110VDC_Battry_Charge', N'PLC : HSM-Monitoring 110  VDC Battry Charger Infeed  MCBs OK(F200,F400)', 17, N'6cb03a15-c11f-42ba-a716-be4b466baf66', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (256, N'HSM_220VAC_MCB', N'Channel1.LoadShedding.Information Data Block.HSM_220VAC_MCB', N'PLC : HSM-Monitoring 24  VDC Power Supply for Cards _ Site (PS2)(G2)', 17, N'e9c5d162-b3d4-43a3-a67a-88900264e5f2', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (257, N'HSM_24VDC_AnalogInput', N'Channel1.LoadShedding.Information Data Block.HSM_24VDC_AnalogInput', N'PLC : HSM-Monitoring 24  VDC Supply for Analog Input Cards (F601-602)', 17, N'ae13a1ae-f133-4c4f-9844-288e83ea8665', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (258, N'HSM_24VDC_Card_Site_PS1', N'Channel1.LoadShedding.Information Data Block.HSM_24VDC_Card_Site_PS1', N'PLC : HSM-Monitoring 24  VDC Power Supply for Cards _ Site (PS1)(G1)', 17, N'c0daf8ed-6d05-4c94-a83f-d2fb29b46eac', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (259, N'HSM_24VDC_Card_Site_PS2', N'Channel1.LoadShedding.Information Data Block.HSM_24VDC_Card_Site_PS2', N'PLC : HSM-Monitoring 24  VDC Power Supply for Cards _ Site (PS2)(G2)', 17, N'a6d124de-c728-4fdf-9330-92745b575bdd', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (260, N'HSM_24VDC_DI_1200_1201', N'Channel1.LoadShedding.Information Data Block.HSM_24VDC_DI_1200_1201', N'PLC : HSM-Monitoring 24 VDC Common for DI Signals (I1200-I1201)(FT1/1)', 17, N'f3118e14-2ac8-4b99-9189-9ff2d3912f86', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (261, N'HSM_24VDC_DI_1202_1203', N'Channel1.LoadShedding.Information Data Block.HSM_24VDC_DI_1202_1203', N'PLC : HSM-Monitoring 24 VDC Common for DI Signals (I1202-I1203)(FT1/2)', 17, N'cff769f8-2633-498a-bc68-00f57f431a91', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (262, N'HSM_24VDC_DI_1204_1205', N'Channel1.LoadShedding.Information Data Block.HSM_24VDC_DI_1204_1205', N'PLC : HSM-Monitoring 24 VDC Common for DI Signals (I1204-I1205)(FT1/3)', 17, N'3921ac6d-b53c-4f7a-9ad6-d251403b2fd7', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (263, N'HSM_24VDC_DI_1206_1207', N'Channel1.LoadShedding.Information Data Block.HSM_24VDC_DI_1206_1207', N'PLC : HSM-Monitoring 24 VDC Common for DI Signals (I1206-I1207)(FT1/4)', 17, N'b7463c22-5c41-43d1-8d13-008229340aab', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (264, N'HSM_24VDC_DI_1208_1209', N'Channel1.LoadShedding.Information Data Block.HSM_24VDC_DI_1208_1209', N'PLC : HSM-Monitoring 24 VDC Common for DI Signals (I1208-I1209)(FT1/5)', 17, N'4cc43576-2f15-49e4-a915-7e2651b5cd31', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (265, N'HSM_24VDC_DI_1210_1211', N'Channel1.LoadShedding.Information Data Block.HSM_24VDC_DI_1210_1211', N'PLC : HSM-Monitoring 24 VDC Common for DI Signals (I1210-I1211)(FT1/6)', 17, N'88634a94-5ad7-44d5-9a17-4fe0ed015924', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (266, N'HSM_24VDC_DI_1245_1246', N'Channel1.LoadShedding.Information Data Block.HSM_24VDC_DI_1245_1246', N'PLC : HSM-Monitoring 24 VDC Common for DI Signals (I1245-I1246)(FT1/7)', 17, N'716329b2-da03-4063-9b2d-e4774fa7d4f9', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (267, N'HSM_24VDC_DI_1247_1248', N'Channel1.LoadShedding.Information Data Block.HSM_24VDC_DI_1247_1248', N'PLC : HSM-Monitoring 24 VDC Common for DI Signals (I1247-I1248)(FT1/8)', 17, N'95e24d0e-8ca1-4603-8e9a-dabfd3c18ea4', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (268, N'HSM_24VDC_Digital_Output', N'Channel1.LoadShedding.Information Data Block.HSM_24VDC_Digital_Output', N'PLC : HSM-Monitoring 24  VDC Supply for Digital Output Cards (F603-F611)', 17, N'ff6a1cdd-1983-4af6-9c64-252dcf1c27da', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (269, N'HSM_24VDC_ET200', N'Channel1.LoadShedding.Information Data Block.HSM_24VDC_ET200', N'PLC : HSM-Monitoring 24 VDC Supply for ET200 Racks (F615-F617)', 17, N'c9530ca8-0dc1-45b0-9349-22bdc94b6a18', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (270, N'HSM_24VDC_ET200_PS3', N'Channel1.LoadShedding.Information Data Block.HSM_24VDC_ET200_PS3', N'PLC : HSM-Monitoring 24  VDC Power Supply for ET200 Racks (PS3)(G3)', 17, N'732c343c-4a5d-4e0b-8d28-e46706c29400', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (271, N'HSM_24VDC_ET200_PS4', N'Channel1.LoadShedding.Information Data Block.HSM_24VDC_ET200_PS4', N'PLC : HSM-Monitoring 24  VDC Power Supply for ET200 Racks (PS4)(G4)', 17, N'bc9e95d6-6c96-4d96-90fb-612ab3109d76', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (272, N'HSM_24VDC_Multiplyers', N'Channel1.LoadShedding.Information Data Block.HSM_24VDC_Multiplyers', N'PLC : HSM-Monitoring 24  VDC Supply for Multiplyers(F631-F634)', 17, N'c3ddd018-ebd6-499a-b9bc-2ba757e6f65b', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (273, N'HSM_C00_CB', N'Channel1.LoadShedding.Information Data Block.HSM_C00_CB', N'PLC : Tripped by PLC Load Shedding', 6, N'7cb77e26-6cbe-4bce-8a76-8baac2f67c4d', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (274, N'HSM_C00_CB_Fault', N'Channel1.LoadShedding.Information Data Block.HSM_C00_CB_Fault', N'PLC : CB MALFUNCTION', 17, N'54d52897-0028-43a9-8f0a-2025bac2710b', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (275, N'HSM_C01_CB', N'Channel1.LoadShedding.Information Data Block.HSM_C01_CB', N'PLC : Tripped by PLC Load Shedding', 6, N'27fb9320-2d23-4a5b-8236-1b3c69a7b8d2', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (276, N'HSM_C01_CB_Fault', N'Channel1.LoadShedding.Information Data Block.HSM_C01_CB_Fault', N'PLC : CB MALFUNCTION', 17, N'b6993b6c-1ae9-425f-8500-24442bffea01', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (277, N'HSM_C02_CB', N'Channel1.LoadShedding.Information Data Block.HSM_C02_CB', N'PLC : Tripped by PLC Load Shedding', 6, N'457cbc85-6dbb-4df9-8172-941e4d0920fe', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (278, N'HSM_C02_CB_Fault', N'Channel1.LoadShedding.Information Data Block.HSM_C02_CB_Fault', N'PLC : CB MALFUNCTION', 17, N'771c4844-898c-418b-972a-1aed6351aede', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (279, N'HSM_C03_CB', N'Channel1.LoadShedding.Information Data Block.HSM_C03_CB', N'PLC : Tripped by PLC Load Shedding', 6, N'b1aac32d-b65d-42e5-a520-9f62b9326dba', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (280, N'HSM_C03_CB_Fault', N'Channel1.LoadShedding.Information Data Block.HSM_C03_CB_Fault', N'PLC : CB MALFUNCTION', 17, N'309fabca-1843-41b7-9dc6-0dc479ea3bd2', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (281, N'HSM_C04_CB', N'Channel1.LoadShedding.Information Data Block.HSM_C04_CB', N'PLC : Tripped by PLC Load Shedding', 6, N'c0f2a230-72b8-4088-9cc0-b9ae61993bd8', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (282, N'HSM_C04_CB_Fault', N'Channel1.LoadShedding.Information Data Block.HSM_C04_CB_Fault', N'PLC : CB MALFUNCTION', 17, N'b2cd4bbd-0865-4f58-aaa1-0cbb8a13a2c6', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (283, N'HSM_C05_CB', N'Channel1.LoadShedding.Information Data Block.HSM_C05_CB', N'PLC : Tripped by PLC Load Shedding', 6, N'6ea71d11-42a8-4c68-8ff0-d1c788b3f920', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (284, N'HSM_C05_CB_Fault', N'Channel1.LoadShedding.Information Data Block.HSM_C05_CB_Fault', N'PLC : CB MALFUNCTION', 17, N'd9d70eee-582c-4cc6-a1af-8d517ba6e9df', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (285, N'HSM_C08_CB', N'Channel1.LoadShedding.Information Data Block.HSM_C08_CB', N'PLC : Tripped by PLC Load Shedding', 6, N'3990f4b2-73e2-499b-a59e-4864fa27cdf0', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (286, N'HSM_C08_CB_Fault', N'Channel1.LoadShedding.Information Data Block.HSM_C08_CB_Fault', N'PLC : CB MALFUNCTION', 17, N'e6406efe-18e2-4887-81d0-305362fbe1c9', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (287, N'HSM_C09_CB', N'Channel1.LoadShedding.Information Data Block.HSM_C09_CB', N'PLC : Tripped by PLC Load Shedding', 6, N'74d6f545-acb0-4767-b553-5c7b5c8d4fbd', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (288, N'HSM_C09_CB_Fault', N'Channel1.LoadShedding.Information Data Block.HSM_C09_CB_Fault', N'PLC : CB MALFUNCTION', 17, N'093bd655-600c-4223-8d57-7b744f853927', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (289, N'HSM_C15_CB', N'Channel1.LoadShedding.Information Data Block.HSM_C15_CB', N'PLC : Tripped by PLC Load Shedding', 6, N'369e9814-3b6f-4f39-94f5-868f0696a88c', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (290, N'HSM_C15_CB_Fault', N'Channel1.LoadShedding.Information Data Block.HSM_C15_CB_Fault', N'PLC : CB MALFUNCTION', 17, N'd83c4707-0e92-47b1-8f71-83a2fbe55283', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (291, N'HSM_C16_CB', N'Channel1.LoadShedding.Information Data Block.HSM_C16_CB', N'PLC : Tripped by PLC Load Shedding', 6, N'7a8b3e39-4971-4a15-a4d7-b494a87517fe', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (292, N'HSM_C16_CB_Fault', N'Channel1.LoadShedding.Information Data Block.HSM_C16_CB_Fault', N'PLC : CB MALFUNCTION', 17, N'73bc9720-4528-4b88-894c-1329122eec0f', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (293, N'HSM_C17_CB', N'Channel1.LoadShedding.Information Data Block.HSM_C17_CB', N'PLC : Tripped by PLC Load Shedding', 6, N'f89ae279-c762-46ef-ac6e-fbf8ecbdb2d0', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (294, N'HSM_C17_CB_Fault', N'Channel1.LoadShedding.Information Data Block.HSM_C17_CB_Fault', N'PLC : CB MALFUNCTION', 17, N'46a5baa6-84ff-4c62-a36f-43ad469eb52b', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (295, N'HSM_C19_CB', N'Channel1.LoadShedding.Information Data Block.HSM_C19_CB', N'PLC : Tripped by PLC Load Shedding', 6, N'0811071f-4763-4acd-901c-5b791539039a', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (296, N'HSM_C19_CB_Fault', N'Channel1.LoadShedding.Information Data Block.HSM_C19_CB_Fault', N'PLC : CB MALFUNCTION', 17, N'27750b06-e77c-4a4b-9353-a23de6567f40', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (297, N'HSM_C20_CB', N'Channel1.LoadShedding.Information Data Block.HSM_C20_CB', N'PLC : Tripped by PLC Load Shedding', 6, N'5c9caf4f-02db-4e0a-a308-3c521ab48d20', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (298, N'HSM_C20_CB_Fault', N'Channel1.LoadShedding.Information Data Block.HSM_C20_CB_Fault', N'PLC : CB MALFUNCTION', 17, N'3c813792-cec4-4779-9584-abb9136f3964', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (299, N'HSM_C21_CB', N'Channel1.LoadShedding.Information Data Block.HSM_C21_CB', N'PLC : Tripped by PLC Load Shedding', 6, N'6ec62438-a8f4-43d7-87a5-0dc6d8fa9b4f', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (300, N'HSM_C21_CB_Fault', N'Channel1.LoadShedding.Information Data Block.HSM_C21_CB_Fault', N'PLC : CB MALFUNCTION', 17, N'c42596eb-5cff-465f-a605-f43e48981284', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (301, N'HSM_C22_CB', N'Channel1.LoadShedding.Information Data Block.HSM_C22_CB', N'PLC : Tripped by PLC Load Shedding', 6, N'219be257-eb91-4b73-b228-50953a062cfb', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (302, N'HSM_C22_CB_Fault', N'Channel1.LoadShedding.Information Data Block.HSM_C22_CB_Fault', N'PLC : CB MALFUNCTION', 17, N'de9b4f46-2e63-40b7-b63e-515871216d2c', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (303, N'HSM_C23_CB', N'Channel1.LoadShedding.Information Data Block.HSM_C23_CB', N'PLC : Tripped by PLC Load Shedding', 6, N'cdb0bd9d-4e78-4f09-99ad-ba2877dfbc72', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (304, N'HSM_C23_CB_Fault', N'Channel1.LoadShedding.Information Data Block.HSM_C23_CB_Fault', N'PLC : CB MALFUNCTION', 17, N'60348a54-9627-4df0-a3a6-055f52a6ccae', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (305, N'HSM_C24_CB', N'Channel1.LoadShedding.Information Data Block.HSM_C24_CB', N'PLC : Tripped by PLC Load Shedding', 6, N'e34804da-2090-47be-aca9-17c7e1dfed22', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (306, N'HSM_C24_CB_Fault', N'Channel1.LoadShedding.Information Data Block.HSM_C24_CB_Fault', N'PLC : CB MALFUNCTION', 17, N'd9edc7ea-498c-4035-8395-eb294b2c44b1', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (307, N'HSM_H3P_CB', N'Channel1.LoadShedding.Information Data Block.HSM_H3P_CB', N'PLC : Tripped by PLC Load Shedding', 6, N'fd4b7dc6-9916-40f5-83be-50fa6efc4b33', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (308, N'HSM_H3P_CB_Fault', N'Channel1.LoadShedding.Information Data Block.HSM_H3P_CB_Fault', N'PLC : CB MALFUNCTION', 17, N'45da83c6-31f7-44d1-9bcb-859c394822ab', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (309, N'HSM_HF1_CB', N'Channel1.LoadShedding.Information Data Block.HSM_HF1_CB', N'PLC : Tripped by PLC Load Shedding', 6, N'cdd27429-c3d7-43ae-8563-7864dbb7ace4', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (310, N'HSM_HF1_CB_Fault', N'Channel1.LoadShedding.Information Data Block.HSM_HF1_CB_Fault', N'PLC : CB MALFUNCTION', 17, N'a9cf22cb-b5f1-4d7d-a8a9-ba4fa652e612', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (311, N'HSM_HF2_CB', N'Channel1.LoadShedding.Information Data Block.HSM_HF2_CB', N'PLC : Tripped by PLC Load Shedding', 6, N'1a6ace86-3ece-46a1-bba9-fa12486c1f73', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (312, N'HSM_HF2_CB_Fault', N'Channel1.LoadShedding.Information Data Block.HSM_HF2_CB_Fault', N'PLC : CB MALFUNCTION', 17, N'5e65b81a-9667-404c-b6f5-5d884e016b23', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (313, N'HSM_HF3_CB', N'Channel1.LoadShedding.Information Data Block.HSM_HF3_CB', N'PLC : Tripped by PLC Load Shedding', 6, N'1c7d046c-f4f9-4863-b056-d03eb47ebd30', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (314, N'HSM_HF3_CB_Fault', N'Channel1.LoadShedding.Information Data Block.HSM_HF3_CB_Fault', N'PLC : CB MALFUNCTION', 17, N'56456353-00ae-492e-9014-a6e1d369d6fa', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (315, N'HSM_HF4_CB', N'Channel1.LoadShedding.Information Data Block.HSM_HF4_CB', N'PLC : Tripped by PLC Load Shedding', 6, N'e0694f87-6a6f-46ce-a26a-c01f44d3346f', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (316, N'HSM_HF4_CB_Fault', N'Channel1.LoadShedding.Information Data Block.HSM_HF4_CB_Fault', N'PLC : CB MALFUNCTION', 17, N'225ed5ae-530a-4d46-92c0-c94a7fd53038', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (317, N'HSM_HF5_CB', N'Channel1.LoadShedding.Information Data Block.HSM_HF5_CB', N'PLC : Tripped by PLC Load Shedding', 6, N'25088bf7-f52b-4ff3-b3b7-2ead00bf69d5', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (318, N'HSM_HF5_CB_Fault', N'Channel1.LoadShedding.Information Data Block.HSM_HF5_CB_Fault', N'PLC : CB MALFUNCTION', 17, N'3d14a4c3-bd72-444d-9585-51cafe6019e6', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (319, N'HSM_HF6_CB', N'Channel1.LoadShedding.Information Data Block.HSM_HF6_CB', N'PLC : Tripped by PLC Load Shedding', 6, N'14238101-41d1-4dcc-845f-e4f66e85c2e4', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (320, N'HSM_HF6_CB_Fault', N'Channel1.LoadShedding.Information Data Block.HSM_HF6_CB_Fault', N'PLC : CB MALFUNCTION', 17, N'957a0be9-331e-4cfe-8220-4a2af46ba445', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (321, N'HSM_HF7_CB', N'Channel1.LoadShedding.Information Data Block.HSM_HF7_CB', N'PLC : Tripped by PLC Load Shedding', 6, N'8ec61b69-36c6-425d-9525-9621c64865a9', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (322, N'HSM_HF7_CB_Fault', N'Channel1.LoadShedding.Information Data Block.HSM_HF7_CB_Fault', N'PLC : CB MALFUNCTION', 17, N'a0492c69-e658-4607-bd4d-c318425e022e', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (323, N'HSM_HR1_CB', N'Channel1.LoadShedding.Information Data Block.HSM_HR1_CB', N'PLC : Tripped by PLC Load Shedding', 6, N'2c9e42b9-835d-4bd2-a8d2-29cfdd83c2f7', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (324, N'HSM_HR1_CB_Fault', N'Channel1.LoadShedding.Information Data Block.HSM_HR1_CB_Fault', N'PLC : CB MALFUNCTION', 17, N'69b3f97f-fc42-4794-aab1-25818ed1dc30', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (325, N'HSM_HR3_CB', N'Channel1.LoadShedding.Information Data Block.HSM_HR3_CB', N'PLC : Tripped by PLC Load Shedding', 6, N'ddf2046e-dcc2-43c7-8325-0073bf125f3d', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (326, N'HSM_HR3_CB_Fault', N'Channel1.LoadShedding.Information Data Block.HSM_HR3_CB_Fault', N'PLC : CB MALFUNCTION', 17, N'6592a47b-1f09-47d4-9ae0-f979b303c863', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (327, N'HSM_M_0VDC_DO_relays', N'Channel1.LoadShedding.Information Data Block.HSM_M_0VDC_DO_relays', N'PLC : HSM-Monitoring M- (0 VDC) for load to be trip Do Relays(F701-F704)', 17, N'494d9c3f-94ad-4986-a50e-f925fd7e53e8', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (328, N'MIS_110VDC_Battry_Charge', N'Channel1.LoadShedding.Information Data Block.MIS_110VDC_Battry_Charge', N'PLC : MIS-Monitoring 110  VDC Battry Charger Infeed  MCBs OK(F200,F400)', 17, N'16709b88-58d6-4c61-a8ad-b9f709933862', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (329, N'MIS_110VDC_S7_400_PS', N'Channel1.LoadShedding.Information Data Block.MIS_110VDC_S7_400_PS', N'PLC : MIS-Monitoring 110 VDC Infeed MCB for S7-400 Rack PS', 17, N'adcf8bd0-0734-46ca-ab93-e950063a33fb', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (330, N'MIS_220VAC_MCB', N'Channel1.LoadShedding.Information Data Block.MIS_220VAC_MCB', N'PLC : MIS-Monitoring 24  VDC Supply for Multiplyers(F670-F675)', 17, N'0502f756-a2c4-4803-99ca-6ab024cbadb1', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (331, N'MIS_24VDC_Analog_Input', N'Channel1.LoadShedding.Information Data Block.MIS_24VDC_Analog_Input', N'PLC : MIS-Monitoring 24  VDC Supply for Analog Input Cards (F601-605)', 17, N'b5f6089b-16ba-4c87-b716-182692183cb9', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (332, N'MIS_24VDC_Card_Site_PS1', N'Channel1.LoadShedding.Information Data Block.MIS_24VDC_Card_Site_PS1', N'PLC : MIS-Monitoring 24  VDC Power Supply for Cards & Site (PS1)(G1)', 17, N'da76c54d-13b9-4274-b0fd-fd1ce187c803', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (333, N'MIS_24VDC_Card_Site_PS2', N'Channel1.LoadShedding.Information Data Block.MIS_24VDC_Card_Site_PS2', N'PLC : MIS-Monitoring 24  VDC Power Supply for Cards & Site (PS2)(G2)', 17, N'2cefc38d-e576-4bdf-86cb-27b309c3352a', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (334, N'MIS_24VDC_DI_128', N'Channel1.LoadShedding.Information Data Block.MIS_24VDC_DI_128', N'PLC : MIS-Monitoring 24 VDC Common for DI Signals (I128)(F630)', 17, N'0cb12b6c-3c68-4c58-944f-9508638696a1', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (335, N'MIS_24VDC_DI_129', N'Channel1.LoadShedding.Information Data Block.MIS_24VDC_DI_129', N'PLC : MIS-Monitoring 24 VDC Common for DI Signals (I129)(F631)', 17, N'0ce7e7b8-1279-4fc6-9e8c-444244b00bb4', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (336, N'MIS_24VDC_DI_130', N'Channel1.LoadShedding.Information Data Block.MIS_24VDC_DI_130', N'PLC : MIS-Monitoring 24 VDC Common for DI Signals (I130)(F632)', 17, N'3d31f413-a942-4f14-b825-70ab6cfb15c3', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (337, N'MIS_24VDC_DI_131', N'Channel1.LoadShedding.Information Data Block.MIS_24VDC_DI_131', N'PLC : MIS-Monitoring 24 VDC Common for DI Signals (I131)(F633)', 17, N'2f35c5c8-ba84-4275-9a02-3de009fcb40c', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (338, N'MIS_24VDC_DI_132', N'Channel1.LoadShedding.Information Data Block.MIS_24VDC_DI_132', N'PLC : MIS-Monitoring 24 VDC Common for DI Signals (I132)(F634)', 17, N'332b2320-d83b-4457-a750-afbeb1093376', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (339, N'MIS_24VDC_DI_133', N'Channel1.LoadShedding.Information Data Block.MIS_24VDC_DI_133', N'PLC : MIS-Monitoring 24 VDC Common for DI Signals (I133)(F635)', 17, N'3fbc3125-5c12-4694-9d28-77a1b675e08b', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (340, N'MIS_24VDC_DI_134', N'Channel1.LoadShedding.Information Data Block.MIS_24VDC_DI_134', N'PLC : MIS-Monitoring 24 VDC Common for DI Signals (I135)(F637)', 17, N'dd14de31-840f-4603-bbf4-d92dd1da9339', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (341, N'MIS_24VDC_DI_135', N'Channel1.LoadShedding.Information Data Block.MIS_24VDC_DI_135', N'PLC : MIS-Monitoring 24 VDC Common for DI Signals (I134)(F636)', 17, N'c48b589f-a474-4370-8490-e71f712d2b51', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (342, N'MIS_24VDC_DI_136', N'Channel1.LoadShedding.Information Data Block.MIS_24VDC_DI_136', N'PLC : MIS-Monitoring 24 VDC Common for DI Signals (I136)(F638)', 17, N'708440e1-4900-4626-8583-e7bae5c19cc9', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (343, N'MIS_24VDC_DI_137', N'Channel1.LoadShedding.Information Data Block.MIS_24VDC_DI_137', N'PLC : MIS-Monitoring 24 VDC Common for DI Signals (I137)(F639)', 17, N'67f05639-0f9b-47ea-b5c9-4f4b61203e5e', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (344, N'MIS_24VDC_DI_138', N'Channel1.LoadShedding.Information Data Block.MIS_24VDC_DI_138', N'PLC : MIS-Monitoring 24 VDC Common for DI Signals (I138)(F640)', 17, N'59d41fb5-e19c-492b-bf3f-f54ac2cea18d', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (345, N'MIS_24VDC_DI_139', N'Channel1.LoadShedding.Information Data Block.MIS_24VDC_DI_139', N'PLC : MIS-Monitoring 24 VDC Common for DI Signals (I139)(F641)', 17, N'2a4cd13e-ef9f-43e8-95d8-b9627967a19b', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (346, N'MIS_24VDC_DI_140', N'Channel1.LoadShedding.Information Data Block.MIS_24VDC_DI_140', N'PLC : MIS-Monitoring 24 VDC Common for DI Signals (I140)(F642)', 17, N'698f42bb-c690-4aa5-9db8-0b066aa0a2e2', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (347, N'MIS_24VDC_DI_141', N'Channel1.LoadShedding.Information Data Block.MIS_24VDC_DI_141', N'PLC : MIS-Monitoring 24 VDC Common for DI Signals (I141)(F643)', 17, N'b6522ff3-061e-4eee-9832-3800e6df0c84', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (348, N'MIS_24VDC_DI_142', N'Channel1.LoadShedding.Information Data Block.MIS_24VDC_DI_142', N'PLC : MIS-Monitoring 24 VDC Common for DI Signals (I142)(F644)', 17, N'51ca3320-3fbe-4f62-9379-cb7272b1c855', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (349, N'MIS_24VDC_DI_143', N'Channel1.LoadShedding.Information Data Block.MIS_24VDC_DI_143', N'PLC : MIS-Monitoring 24 VDC Common for DI Signals (I143)(F645)', 17, N'e06a517d-d339-4bd8-9d47-5a3b6a56fc43', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (350, N'MIS_24VDC_DI_200_201', N'Channel1.LoadShedding.Information Data Block.MIS_24VDC_DI_200_201', N'PLC : MIS-Monitoring 24 VDC Common for DI Signals (I200-I201)(FT1/17)', 17, N'5da4c185-4556-4495-ad6e-caef283e5fd1', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (351, N'MIS_24VDC_DI_202_203', N'Channel1.LoadShedding.Information Data Block.MIS_24VDC_DI_202_203', N'PLC : MIS-Monitoring 24 VDC Common for DI Signals (I202-I203)(FT1/18)', 17, N'd7584b31-fcf9-43e8-a1c0-5ae9e5677484', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (352, N'MIS_24VDC_DI_204_205', N'Channel1.LoadShedding.Information Data Block.MIS_24VDC_DI_204_205', N'PLC : MIS-Monitoring 24 VDC Common for DI Signals (I204-I205)(FT1/19)', 17, N'ad4abb96-4569-468d-ae33-03177caa20a5', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (353, N'MIS_24VDC_DI_206_207', N'Channel1.LoadShedding.Information Data Block.MIS_24VDC_DI_206_207', N'PLC : MIS-Monitoring 24 VDC Common for DI Signals (I206-I207)(FT1/20)', 17, N'6beda335-f97f-4a1f-944c-9fe2babbf526', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (354, N'MIS_24VDC_DI_208_209', N'Channel1.LoadShedding.Information Data Block.MIS_24VDC_DI_208_209', N'PLC : MIS-Monitoring 24 VDC Common for DI Signals (I208-I209)(FT1/21)', 17, N'fb6f181c-ae56-4c0b-bec3-1f0d938eaca7', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (355, N'MIS_24VDC_DI_210_211', N'Channel1.LoadShedding.Information Data Block.MIS_24VDC_DI_210_211', N'PLC : MIS-Monitoring 24 VDC Common for DI Signals (I210-I211)(FT1/22)', 17, N'2f594110-6451-4db9-b00c-b7a5dbb5315a', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (356, N'MIS_24VDC_DI_400_401', N'Channel1.LoadShedding.Information Data Block.MIS_24VDC_DI_400_401', N'PLC : MIS-Monitoring 24 VDC Common for DI Signals (I400-I401)(FT1/1)', 17, N'1e02b79d-6010-40f0-ae41-e4673eaf42b4', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (357, N'MIS_24VDC_DI_402_403', N'Channel1.LoadShedding.Information Data Block.MIS_24VDC_DI_402_403', N'PLC : MIS-Monitoring 24 VDC Common for DI Signals (I402-I403)(FT1/2)', 17, N'56000377-bd82-4b53-bff4-ac0edfcf34ca', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (358, N'MIS_24VDC_DI_404_405', N'Channel1.LoadShedding.Information Data Block.MIS_24VDC_DI_404_405', N'PLC : MIS-Monitoring 24 VDC Common for DI Signals (I404-I405)(FT1/3)', 17, N'c608e8af-7d5c-418c-bbdd-34b1fd90d304', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (359, N'MIS_24VDC_DI_406_407', N'Channel1.LoadShedding.Information Data Block.MIS_24VDC_DI_406_407', N'PLC : MIS-Monitoring 24 VDC Common for DI Signals (I406-I407)(FT1/4)', 17, N'4c65cae4-eaba-4a76-a14e-697668383e72', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (360, N'MIS_24VDC_DI_408_409', N'Channel1.LoadShedding.Information Data Block.MIS_24VDC_DI_408_409', N'PLC : MIS-Monitoring 24 VDC Common for DI Signals (I408-I409)(FT1/5)', 17, N'024d7005-a0f5-40d8-9d63-60894bdc11f2', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (361, N'MIS_24VDC_DI_41_42', N'Channel1.LoadShedding.Information Data Block.MIS_24VDC_DI_41_42', N'PLC : MIS-Monitoring 24 VDC Common for DI Signals (I41-I42)(FT1/27)', 17, N'0754ad03-d956-4b73-8f73-ea58db353f7e', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (362, N'MIS_24VDC_DI_410_411', N'Channel1.LoadShedding.Information Data Block.MIS_24VDC_DI_410_411', N'PLC : MIS-Monitoring 24 VDC Common for DI Signals (I410-I411)(FT1/6)', 17, N'de8cedde-b0b1-4652-a9b5-76c430f60b7e', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (363, N'MIS_24VDC_DI_412_413', N'Channel1.LoadShedding.Information Data Block.MIS_24VDC_DI_412_413', N'PLC : MIS-Monitoring 24 VDC Common for DI Signals (I412-I413)(FT1/7)', 17, N'79313fb2-f00f-47f3-ae41-7e83b91d5c65', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (364, N'MIS_24VDC_DI_414_415', N'Channel1.LoadShedding.Information Data Block.MIS_24VDC_DI_414_415', N'PLC : MIS-Monitoring 24 VDC Common for DI Signals (I414-I415)(FT1/8)', 17, N'ddb0ca90-56b3-48bd-94a9-2a4dacdd7c8f', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (365, N'MIS_24VDC_DI_416_417', N'Channel1.LoadShedding.Information Data Block.MIS_24VDC_DI_416_417', N'PLC : MIS-Monitoring 24 VDC Common for DI Signals (I416-I417)(FT1/9)', 17, N'3d8fb686-7d7d-427d-9652-bba951c31bce', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (366, N'MIS_24VDC_DI_418_419', N'Channel1.LoadShedding.Information Data Block.MIS_24VDC_DI_418_419', N'PLC : MIS-Monitoring 24 VDC Common for DI Signals (I418-I419)(FT1/10)', 17, N'9c56e591-15a2-42c1-8aaf-90b78cb97b43', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (367, N'MIS_24VDC_DI_420_421', N'Channel1.LoadShedding.Information Data Block.MIS_24VDC_DI_420_421', N'PLC : MIS-Monitoring 24 VDC Common for DI Signals (I420-I421)(FT1/11)', 17, N'a386b48d-faa5-43ca-b38a-e2104cb9caae', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (368, N'MIS_24VDC_DI_422_423', N'Channel1.LoadShedding.Information Data Block.MIS_24VDC_DI_422_423', N'PLC : MIS-Monitoring 24 VDC Common for DI Signals (I422-I423)(FT1/12)', 17, N'7204ed95-91fa-41cb-8dc5-29b2c09edd27', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (369, N'MIS_24VDC_DI_424_425', N'Channel1.LoadShedding.Information Data Block.MIS_24VDC_DI_424_425', N'PLC : MIS-Monitoring 24 VDC Common for DI Signals (I424-I425)(FT1/13)', 17, N'709c51eb-2e79-41f1-8a5d-897c69d802d6', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (370, N'MIS_24VDC_DI_426_427', N'Channel1.LoadShedding.Information Data Block.MIS_24VDC_DI_426_427', N'PLC : MIS-Monitoring 24 VDC Common for DI Signals (I426-I427)(FT1/14)', 17, N'0f2cc0ca-de2d-4878-a437-da4acfc6cf88', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (371, N'MIS_24VDC_DI_428_429', N'Channel1.LoadShedding.Information Data Block.MIS_24VDC_DI_428_429', N'PLC : MIS-Monitoring 24 VDC Common for DI Signals (I428-I429)(FT1/15)', 17, N'3f0d61fe-9b45-4548-b6d4-e01fc78442e1', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (372, N'MIS_24VDC_DI_43_44', N'Channel1.LoadShedding.Information Data Block.MIS_24VDC_DI_43_44', N'PLC : MIS-Monitoring 24 VDC Common for DI Signals (I43-I44)(FT1/28)', 17, N'232374f1-7386-4d81-a509-0e35eed32aa0', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (373, N'MIS_24VDC_DI_430_431', N'Channel1.LoadShedding.Information Data Block.MIS_24VDC_DI_430_431', N'PLC : MIS-Monitoring 24 VDC Common for DI Signals (I430-I431)(FT1/16)', 17, N'eb804f07-476b-4d21-bcc9-bb74f2f2fcd4', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (374, N'MIS_24VDC_DI_45_46', N'Channel1.LoadShedding.Information Data Block.MIS_24VDC_DI_45_46', N'PLC : MIS-Monitoring 24 VDC Common for DI Signals (I45-I46)(FT1/23)', 17, N'661ced7a-5853-45ee-9e59-b06621064bf2', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (375, N'MIS_24VDC_DI_47_48', N'Channel1.LoadShedding.Information Data Block.MIS_24VDC_DI_47_48', N'PLC : MIS-Monitoring 24 VDC Common for DI Signals (I47-I48)(FT1/24)', 17, N'9becd39e-b9cb-4738-9b2a-0a122144eb37', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (376, N'MIS_24VDC_DI_49_50', N'Channel1.LoadShedding.Information Data Block.MIS_24VDC_DI_49_50', N'PLC : MIS-Monitoring 24 VDC Common for DI Signals (I49-I50)(FT1/25)', 17, N'1ad2851c-5199-4098-9220-fdf22cd1ff5a', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (377, N'MIS_24VDC_DI_51_52', N'Channel1.LoadShedding.Information Data Block.MIS_24VDC_DI_51_52', N'PLC : MIS-Monitoring 24 VDC Common for DI Signals (I51-I52)(FT1/26)', 17, N'ea4f5dfd-f36f-4d4c-a9fa-3f9271721aaf', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (378, N'MIS_24VDC_Digital_Output', N'Channel1.LoadShedding.Information Data Block.MIS_24VDC_Digital_Output', N'PLC : MIS-Monitoring 24  VDC Supply for Digital Output Cards (F606-F621)', 17, N'59ee7ee7-8b85-40dd-9dc7-6b08eb99b8a8', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (379, N'MIS_24VDC_ET200', N'Channel1.LoadShedding.Information Data Block.MIS_24VDC_ET200', N'PLC : MIS-Monitoring 24 VDC Supply for ET200 Racks (F660-F665)', 17, N'0043325e-54e3-4ff7-bd9f-3face09b369d', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (380, N'MIS_24VDC_ET200_PS3', N'Channel1.LoadShedding.Information Data Block.MIS_24VDC_ET200_PS3', N'PLC : MIS-Monitoring 24  VDC Power Supply for ET200 Racks (PS3)(G3)', 17, N'2ff9ef4a-507d-4066-b520-03b7cc748aba', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (381, N'MIS_24VDC_ET200_PS4', N'Channel1.LoadShedding.Information Data Block.MIS_24VDC_ET200_PS4', N'PLC : MIS-Monitoring 24  VDC Power Supply for ET200 Racks (PS4)(G4)', 17, N'e8346981-10ea-4138-a131-602582664e0c', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (382, N'MIS_24VDC_Multiplyers', N'Channel1.LoadShedding.Information Data Block.MIS_24VDC_Multiplyers', N'PLC : MIS-Monitoring 24  VDC Supply for Multiplyers(F670-F675)', 17, N'92647685-3ef8-4d7b-bf65-e42a0495915d', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (383, N'MIS_M_0VDC_DO_relays', N'Channel1.LoadShedding.Information Data Block.MIS_M_0VDC_DO_relays', N'PLC : MIS-Monitoring M- (0 VDC) for load to be trip Do Relays(F701-F706)', 17, N'1d28dc40-2ebc-4e29-9056-254d6471f301', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (384, N'MS1_C00_CB', N'Channel1.LoadShedding.Information Data Block.MS1_C00_CB', N'PLC : Tripped by PLC Load Shedding', 6, N'57f535e4-37fb-402c-b8ed-1be3c3c90c17', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (385, N'MS1_C00_CB_Fault', N'Channel1.LoadShedding.Information Data Block.MS1_C00_CB_Fault', N'PLC : CB MALFUNCTION', 17, N'43ed2b11-64e1-4340-be83-83835fdae9b2', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (386, N'MS1_C01_CB', N'Channel1.LoadShedding.Information Data Block.MS1_C01_CB', N'PLC : Tripped by PLC Load Shedding', 6, N'9c0d442d-789e-4582-bc67-94691effe2d9', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (387, N'MS1_C01_CB_Fault', N'Channel1.LoadShedding.Information Data Block.MS1_C01_CB_Fault', N'PLC : CB MALFUNCTION', 17, N'fa214e5a-c496-4eb4-9e19-811b8c9c8d3d', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (388, N'MS1_C02_CB', N'Channel1.LoadShedding.Information Data Block.MS1_C02_CB', N'PLC : Tripped by PLC Load Shedding', 6, N'c7da0ff3-9d86-4281-a9e1-d7c14c50b532', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (389, N'MS1_C02_CB_Fault', N'Channel1.LoadShedding.Information Data Block.MS1_C02_CB_Fault', N'PLC : CB MALFUNCTION', 17, N'cf8d7155-082e-4eff-8f75-308dae60c957', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (390, N'MS1_C03_CB', N'Channel1.LoadShedding.Information Data Block.MS1_C03_CB', N'PLC : Tripped by PLC Load Shedding', 6, N'd8f43925-1791-4c72-9c8f-fe6319785282', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (391, N'MS1_C03_CB_Fault', N'Channel1.LoadShedding.Information Data Block.MS1_C03_CB_Fault', N'PLC : CB MALFUNCTION', 17, N'a388cfe4-88b8-4e86-889d-8a30ca3f2d5c', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (392, N'MS1_C05_CB', N'Channel1.LoadShedding.Information Data Block.MS1_C05_CB', N'PLC : Tripped by PLC Load Shedding', 6, N'a5cb0788-8834-4a2d-8806-f50f7ae33ef3', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (393, N'MS1_C05_CB_Fault', N'Channel1.LoadShedding.Information Data Block.MS1_C05_CB_Fault', N'PLC : CB MALFUNCTION', 17, N'23b2b03e-794c-4e84-ae8a-39df02bb297f', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (394, N'MS1_C06_CB', N'Channel1.LoadShedding.Information Data Block.MS1_C06_CB', N'PLC : Tripped by PLC Load Shedding', 6, N'cbce49aa-4f63-4ad9-9bce-ef0e461a3e7a', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (395, N'MS1_C06_CB_Fault', N'Channel1.LoadShedding.Information Data Block.MS1_C06_CB_Fault', N'PLC : CB MALFUNCTION', 17, N'8ef3a42c-b696-4296-afb4-d86857ef2655', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (396, N'MS1_C07_CB', N'Channel1.LoadShedding.Information Data Block.MS1_C07_CB', N'PLC : Tripped by PLC Load Shedding', 6, N'8f0c9966-ab74-4a68-a3d2-4e8704f9cfaa', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (397, N'MS1_C07_CB_Fault', N'Channel1.LoadShedding.Information Data Block.MS1_C07_CB_Fault', N'PLC : CB MALFUNCTION', 17, N'27a6b98d-7883-4ed6-b4ab-8f2a08683abc', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (398, N'MS1_C12_CB', N'Channel1.LoadShedding.Information Data Block.MS1_C12_CB', N'PLC : Tripped by PLC Load Shedding', 6, N'5a133f7d-12ec-4eda-89a8-36c74ccd0d20', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (399, N'MS1_C12_CB_Fault', N'Channel1.LoadShedding.Information Data Block.MS1_C12_CB_Fault', N'PLC : CB MALFUNCTION', 17, N'f737a0e8-0545-4d1d-9941-aeaa75867fc8', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (400, N'MS1_C13_CB', N'Channel1.LoadShedding.Information Data Block.MS1_C13_CB', N'PLC : Tripped by PLC Load Shedding', 6, N'a9a152d8-7b59-456d-8130-6917f911896f', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (401, N'MS1_C13_CB_Fault', N'Channel1.LoadShedding.Information Data Block.MS1_C13_CB_Fault', N'PLC : CB MALFUNCTION', 17, N'6b69b57c-acb9-44bb-89db-cd6624ceb43f', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (402, N'MS1_C14_CB', N'Channel1.LoadShedding.Information Data Block.MS1_C14_CB', N'PLC : Tripped by PLC Load Shedding', 6, N'e0a6d457-51e8-4e59-96df-33eea5ce64b4', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (403, N'MS1_C14_CB_Fault', N'Channel1.LoadShedding.Information Data Block.MS1_C14_CB_Fault', N'PLC : CB MALFUNCTION', 17, N'f5b62c82-ef01-45ff-8b6f-69b7bac0ca6a', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (404, N'MS1_C16_CB', N'Channel1.LoadShedding.Information Data Block.MS1_C16_CB', N'PLC : Tripped by PLC Load Shedding', 6, N'18278c88-c0f8-4e87-b577-c78aa586286d', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (405, N'MS1_C16_CB_Fault', N'Channel1.LoadShedding.Information Data Block.MS1_C16_CB_Fault', N'PLC : CB MALFUNCTION', 17, N'21bf5c6d-4887-4770-a1cb-9044d96754dd', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (406, N'MS1_C17_CB', N'Channel1.LoadShedding.Information Data Block.MS1_C17_CB', N'PLC : Tripped by PLC Load Shedding', 6, N'a3b6d3ae-7d5e-4e75-9101-956087d35cbc', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (407, N'MS1_C17_CB_Fault', N'Channel1.LoadShedding.Information Data Block.MS1_C17_CB_Fault', N'PLC : CB MALFUNCTION', 17, N'62284745-3917-4c59-87a5-ac2ecbf272e3', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (408, N'MS1_C18_CB', N'Channel1.LoadShedding.Information Data Block.MS1_C18_CB', N'PLC : Tripped by PLC Load Shedding', 6, N'a4ff99e3-ba0d-4de1-a0ff-e2ab44f4459e', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (409, N'MS1_C18_CB_Fault', N'Channel1.LoadShedding.Information Data Block.MS1_C18_CB_Fault', N'PLC : CB MALFUNCTION', 17, N'2dcfcbf3-a407-44ad-8c6c-da609f2b96de', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (410, N'MS1_C19_CB', N'Channel1.LoadShedding.Information Data Block.MS1_C19_CB', N'PLC : Tripped by PLC Load Shedding', 6, N'b60e6ce8-ee53-42e3-8a6e-be4f48af8c19', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (411, N'MS1_C19_CB_Fault', N'Channel1.LoadShedding.Information Data Block.MS1_C19_CB_Fault', N'PLC : CB MALFUNCTION', 17, N'b3f51f64-fc54-45cd-814f-fb1d4711f654', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (412, N'MS1_M25_CB', N'Channel1.LoadShedding.Information Data Block.MS1_M25_CB', N'PLC : Tripped by PLC Load Shedding', 6, N'5a5dc9fe-c324-402e-b5a0-95eeb3de0739', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (413, N'MS1_M25_CB_Fault', N'Channel1.LoadShedding.Information Data Block.MS1_M25_CB_Fault', N'PLC : CB MALFUNCTION', 17, N'c2f7126e-0fed-4b30-b9b2-dd6efe7cd3e2', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (414, N'MS1_M3A_CB', N'Channel1.LoadShedding.Information Data Block.MS1_M3A_CB', N'PLC : Tripped by PLC Load Shedding', 6, N'78e00e26-fb60-4f81-ab6f-19cdfc0b9bfd', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (415, N'MS1_M3A_CB_Fault', N'Channel1.LoadShedding.Information Data Block.MS1_M3A_CB_Fault', N'PLC : CB MALFUNCTION', 17, N'e5551307-7dee-426b-af7f-aa62af820519', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (416, N'MS1_M46_CB', N'Channel1.LoadShedding.Information Data Block.MS1_M46_CB', N'PLC : Tripped by PLC Load Shedding', 6, N'eb970224-bd32-48aa-8e1a-2f220d767e37', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (417, N'MS1_M46_CB_Fault', N'Channel1.LoadShedding.Information Data Block.MS1_M46_CB_Fault', N'PLC : CB MALFUNCTION', 17, N'fe572fe0-dffd-47a7-afb9-57f0c0fcabae', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (418, N'MS1_M51_CB', N'Channel1.LoadShedding.Information Data Block.MS1_M51_CB', N'PLC : Tripped by PLC Load Shedding', 6, N'103cf6e3-c2df-4752-83dd-9d266dd0ab6e', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (419, N'MS1_M51_CB_Fault', N'Channel1.LoadShedding.Information Data Block.MS1_M51_CB_Fault', N'PLC : CB MALFUNCTION', 17, N'c5839177-4121-4f29-9b7c-cf9ac9f8e2c5', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (420, N'MS1_M57_CB', N'Channel1.LoadShedding.Information Data Block.MS1_M57_CB', N'PLC : Tripped by PLC Load Shedding', 6, N'066c42af-2583-4c63-97e8-675ae67830d6', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (421, N'MS1_M57_CB_Fault', N'Channel1.LoadShedding.Information Data Block.MS1_M57_CB_Fault', N'PLC : CB MALFUNCTION', 17, N'd7020eac-81fe-4783-9ae9-f2813f667cf9', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (422, N'MS1_M6B_CB', N'Channel1.LoadShedding.Information Data Block.MS1_M6B_CB', N'PLC : Tripped by PLC Load Shedding', 6, N'ebc1b92e-cd99-4801-9f63-a00153e1716c', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (423, N'MS1_M6B_CB_Fault', N'Channel1.LoadShedding.Information Data Block.MS1_M6B_CB_Fault', N'PLC : CB MALFUNCTION', 17, N'0e0cc37d-b3d2-47c7-979b-16a63a9eac46', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (424, N'MS1_M78_CB', N'Channel1.LoadShedding.Information Data Block.MS1_M78_CB', N'PLC : Tripped by PLC Load Shedding', 6, N'ced6c174-6d0a-4019-81c5-a4f3dd135126', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (425, N'MS1_M78_CB_Fault', N'Channel1.LoadShedding.Information Data Block.MS1_M78_CB_Fault', N'PLC : CB MALFUNCTION', 17, N'e52f85cf-bb62-4705-95cc-89c9ae0fb677', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (426, N'MS1_MF1_CB', N'Channel1.LoadShedding.Information Data Block.MS1_MF1_CB', N'PLC : Tripped by PLC Load Shedding', 6, N'9212d47f-5f0c-499c-b90c-cb671eb7005c', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (427, N'MS1_MF1_CB_Fault', N'Channel1.LoadShedding.Information Data Block.MS1_MF1_CB_Fault', N'PLC : CB MALFUNCTION', 17, N'242a1429-4366-441b-ab2c-a27fe877ed6c', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (428, N'MS1_MF2_CB', N'Channel1.LoadShedding.Information Data Block.MS1_MF2_CB', N'PLC : Tripped by PLC Load Shedding', 6, N'97357a6e-f6a5-48b1-94a5-5d84042852a0', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (429, N'MS1_MF2_CB_Fault', N'Channel1.LoadShedding.Information Data Block.MS1_MF2_CB_Fault', N'PLC : CB MALFUNCTION', 17, N'5476758a-f4b3-4599-97ae-5ee3dbccaadd', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (430, N'MS1_MF3_CB', N'Channel1.LoadShedding.Information Data Block.MS1_MF3_CB', N'PLC : Tripped by PLC Load Shedding', 6, N'8b04cd82-edde-4c0f-86b6-60fa49fa7795', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (431, N'MS1_MF3_CB_Fault', N'Channel1.LoadShedding.Information Data Block.MS1_MF3_CB_Fault', N'PLC : CB MALFUNCTION', 17, N'98e66749-94f0-4b18-90fd-058a37dffdab', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (432, N'MS1_MF4_CB', N'Channel1.LoadShedding.Information Data Block.MS1_MF4_CB', N'PLC : Tripped by PLC Load Shedding', 6, N'a6ebb8c3-f91f-49d0-b3b0-8ffa1a119be9', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (433, N'MS1_MF4_CB_Fault', N'Channel1.LoadShedding.Information Data Block.MS1_MF4_CB_Fault', N'PLC : CB MALFUNCTION', 17, N'd186f41a-00ee-43ec-9764-9994e62c6248', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (434, N'MS1_MF5_CB', N'Channel1.LoadShedding.Information Data Block.MS1_MF5_CB', N'PLC : Tripped by PLC Load Shedding', 6, N'66d0b39f-9d47-41f5-859e-3b9b0245e65c', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (435, N'MS1_MF5_CB_Fault', N'Channel1.LoadShedding.Information Data Block.MS1_MF5_CB_Fault', N'PLC : CB MALFUNCTION', 17, N'f9a866da-69b8-4f59-86ab-3b9f19ad6a66', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (436, N'MS1_MF6_CB', N'Channel1.LoadShedding.Information Data Block.MS1_MF6_CB', N'PLC : Tripped by PLC Load Shedding', 6, N'eee62e77-b371-46b8-82ce-8c776a544228', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (437, N'MS1_MF6_CB_Fault', N'Channel1.LoadShedding.Information Data Block.MS1_MF6_CB_Fault', N'PLC : CB MALFUNCTION', 17, N'838aca70-eb08-4193-a8fc-3319310126cc', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (438, N'MS1_MF7_CB', N'Channel1.LoadShedding.Information Data Block.MS1_MF7_CB', N'PLC : Tripped by PLC Load Shedding', 6, N'501fc698-beab-4fba-8076-1191c0b83534', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (439, N'MS1_MF7_CB_Fault', N'Channel1.LoadShedding.Information Data Block.MS1_MF7_CB_Fault', N'PLC : CB MALFUNCTION', 17, N'92ef19d6-52d5-4697-838b-72808dc427c8', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (440, N'MS1_MF8_CB', N'Channel1.LoadShedding.Information Data Block.MS1_MF8_CB', N'PLC : Tripped by PLC Load Shedding', 6, N'1a71156e-f857-4d7e-8f26-9018f013c928', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (441, N'MS1_MF8_CB_Fault', N'Channel1.LoadShedding.Information Data Block.MS1_MF8_CB_Fault', N'PLC : CB MALFUNCTION', 17, N'b91100aa-d6cd-45f6-9a3d-8213b5af7416', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (442, N'MS1_ML5_CB', N'Channel1.LoadShedding.Information Data Block.MS1_ML5_CB', N'PLC : Tripped by PLC Load Shedding', 6, N'7b799ff9-74fa-4e65-afd8-79c000358d6f', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (443, N'MS1_ML5_CB_Fault', N'Channel1.LoadShedding.Information Data Block.MS1_ML5_CB_Fault', N'PLC : CB MALFUNCTION', 17, N'2324ce16-eb67-429e-a662-f148bc343d37', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (444, N'MS1_ML6_CB', N'Channel1.LoadShedding.Information Data Block.MS1_ML6_CB', N'PLC : Tripped by PLC Load Shedding', 6, N'df903ef9-d061-45b0-8368-80602ffde594', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (445, N'MS1_ML6_CB_Fault', N'Channel1.LoadShedding.Information Data Block.MS1_ML6_CB_Fault', N'PLC : CB MALFUNCTION', 17, N'dab7b712-5ad7-4d47-a0b9-fd7a297a7102', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (446, N'MS1_ML7_CB', N'Channel1.LoadShedding.Information Data Block.MS1_ML7_CB', N'PLC : Tripped by PLC Load Shedding', 6, N'b0a999fd-d2f7-4829-babe-89e00e227a5d', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (447, N'MS1_ML7_CB_Fault', N'Channel1.LoadShedding.Information Data Block.MS1_ML7_CB_Fault', N'PLC : CB MALFUNCTION', 17, N'870c2b1e-643b-47c5-9194-da59bb3df6c8', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (448, N'MS1_ML8_CB', N'Channel1.LoadShedding.Information Data Block.MS1_ML8_CB', N'PLC : Tripped by PLC Load Shedding', 6, N'fa209b4d-b5ac-4d3a-9bae-bc439778436f', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (449, N'MS1_ML8_CB_Fault', N'Channel1.LoadShedding.Information Data Block.MS1_ML8_CB_Fault', N'PLC : CB MALFUNCTION', 17, N'391b0360-bbb2-4953-83f7-074277d8983f', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (450, N'MS1_MSA_CB', N'Channel1.LoadShedding.Information Data Block.MS1_MSA_CB', N'PLC : Tripped by PLC Load Shedding', 6, N'cf15fb99-5b26-42d5-a291-6b7223be6431', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (451, N'MS1_MSA_CB_Fault', N'Channel1.LoadShedding.Information Data Block.MS1_MSA_CB_Fault', N'PLC : CB MALFUNCTION', 17, N'b984b947-1b4e-4abd-827b-90afb05057fd', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (452, N'MS1_MSB_CB', N'Channel1.LoadShedding.Information Data Block.MS1_MSB_CB', N'PLC : Tripped by PLC Load Shedding', 6, N'd09394cd-5db9-4b60-9a4a-24bf7d76b696', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (453, N'MS1_MSB_CB_Fault', N'Channel1.LoadShedding.Information Data Block.MS1_MSB_CB_Fault', N'PLC : CB MALFUNCTION', 17, N'2aefb265-a06a-43b4-ae9e-40c0418b7661', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (454, N'MS1_MT1_CB', N'Channel1.LoadShedding.Information Data Block.MS1_MT1_CB', N'PLC : Tripped by PLC Load Shedding', 6, N'b8fee7a0-9ee7-40fc-aa4f-63a9f89c9273', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (455, N'MS1_MT1_CB_Fault', N'Channel1.LoadShedding.Information Data Block.MS1_MT1_CB_Fault', N'PLC : CB MALFUNCTION', 17, N'0a8c5cf2-c193-4ba1-b297-441815553750', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (456, N'MS1_MT2_CB', N'Channel1.LoadShedding.Information Data Block.MS1_MT2_CB', N'PLC : Tripped by PLC Load Shedding', 6, N'1628383d-f36d-477a-b726-04a5ac868578', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (457, N'MS1_MT2_CB_Fault', N'Channel1.LoadShedding.Information Data Block.MS1_MT2_CB_Fault', N'PLC : CB MALFUNCTION', 17, N'b01902f3-cc7f-45b7-852b-3762eb73fb0d', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (458, N'MS1_MT5_CB', N'Channel1.LoadShedding.Information Data Block.MS1_MT5_CB', N'PLC : Tripped by PLC Load Shedding', 6, N'5ac0d16a-b15f-4dc9-9434-6cbd2e93ef35', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (459, N'MS1_MT5_CB_Fault', N'Channel1.LoadShedding.Information Data Block.MS1_MT5_CB_Fault', N'PLC : CB MALFUNCTION', 17, N'9c8337f7-f100-4d2e-896d-4a0ac4f9d25a', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (460, N'MS1_MT7_CB', N'Channel1.LoadShedding.Information Data Block.MS1_MT7_CB', N'PLC : Tripped by PLC Load Shedding', 6, N'ddcf4fb3-354a-40a0-a5fc-bdd2a5d95d43', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (461, N'MS1_MT7_CB_Fault', N'Channel1.LoadShedding.Information Data Block.MS1_MT7_CB_Fault', N'PLC : CB MALFUNCTION', 17, N'9786a1fa-84ef-4d52-884f-7b354195d8ee', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (462, N'MS1_MV3_CB', N'Channel1.LoadShedding.Information Data Block.MS1_MV3_CB', N'PLC : Tripped by PLC Load Shedding', 6, N'72c22580-a2e5-43b0-afc0-ba379cbeb093', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (463, N'MS1_MV3_CB_Fault', N'Channel1.LoadShedding.Information Data Block.MS1_MV3_CB_Fault', N'PLC : CB MALFUNCTION', 17, N'1798f481-a7ce-4993-bb6d-b1d2ae01551d', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (464, N'MS1_MZ3_CB', N'Channel1.LoadShedding.Information Data Block.MS1_MZ3_CB', N'PLC : Tripped by PLC Load Shedding', 6, N'1615be95-9e58-40c5-aedd-3a2caeda67a1', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (465, N'MS1_MZ3_CB_Fault', N'Channel1.LoadShedding.Information Data Block.MS1_MZ3_CB_Fault', N'PLC : CB MALFUNCTION', 17, N'f91c1e73-9c46-40d1-964a-005d371f83f5', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (466, N'MS1_SV1_CB', N'Channel1.LoadShedding.Information Data Block.MS1_SV1_CB', N'PLC : Tripped by PLC Load Shedding', 6, N'8173fa2c-408f-49a6-b403-83210842d8d5', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (467, N'MS1_SV1_CB_Fault', N'Channel1.LoadShedding.Information Data Block.MS1_SV1_CB_Fault', N'PLC : CB MALFUNCTION', 17, N'eb21c748-0ee6-4c9e-9067-72d2728ff74d', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (468, N'MS1_SV2_CB', N'Channel1.LoadShedding.Information Data Block.MS1_SV2_CB', N'PLC : Tripped by PLC Load Shedding', 6, N'8b51964c-8c50-4b5f-abda-b94f821c2bdd', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (469, N'MS1_SV2_CB_Fault', N'Channel1.LoadShedding.Information Data Block.MS1_SV2_CB_Fault', N'PLC : CB MALFUNCTION', 17, N'8a698e49-d762-4e74-88dc-8dcf7a676c3b', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (470, N'MS2_FCB_CB', N'Channel1.LoadShedding.Information Data Block.MS2_FCB_CB', N'PLC : Tripped by PLC Load Shedding', 6, N'fb5bc78b-aedf-4e7d-ae0b-e1850786f8c6', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (471, N'MS2_FCB_CB_Fault', N'Channel1.LoadShedding.Information Data Block.MS2_FCB_CB_Fault', N'PLC : CB MALFUNCTION', 17, N'542bb8d1-fa3e-4135-92d7-2ee2ab47f6d2', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (472, N'MS2_LF1_CB', N'Channel1.LoadShedding.Information Data Block.MS2_LF1_CB', N'PLC : Tripped by PLC Load Shedding', 6, N'3141507e-b855-49c6-b7e9-dff6789f6b72', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (473, N'MS2_LF1_CB_Fault', N'Channel1.LoadShedding.Information Data Block.MS2_LF1_CB_Fault', N'PLC : CB MALFUNCTION', 17, N'b094c7a5-4c13-48ec-b6d2-2873f0d6c35b', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (474, N'MS2_LF2_CB', N'Channel1.LoadShedding.Information Data Block.MS2_LF2_CB', N'PLC : Tripped by PLC Load Shedding', 6, N'22a4f9d2-3286-4853-a29e-16d781063af6', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (475, N'MS2_LF2_CB_Fault', N'Channel1.LoadShedding.Information Data Block.MS2_LF2_CB_Fault', N'PLC : CB MALFUNCTION', 17, N'009bc01a-c239-4726-8b27-2f0508c31d9b', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (476, N'MS2_LF3_CB', N'Channel1.LoadShedding.Information Data Block.MS2_LF3_CB', N'PLC : Tripped by PLC Load Shedding', 6, N'ca20f1bd-f23c-44a0-a537-3f638a8460fa', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (477, N'MS2_LF3_CB_Fault', N'Channel1.LoadShedding.Information Data Block.MS2_LF3_CB_Fault', N'PLC : CB MALFUNCTION', 17, N'f5e90a93-45dc-4dbf-991c-c9aa1987ba54', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (478, N'MS2_LF4_CB', N'Channel1.LoadShedding.Information Data Block.MS2_LF4_CB', N'PLC : Tripped by PLC Load Shedding', 6, N'9f401f2d-f601-4ae6-9555-759004c489dd', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (479, N'MS2_LF4_CB_Fault', N'Channel1.LoadShedding.Information Data Block.MS2_LF4_CB_Fault', N'PLC : CB MALFUNCTION', 17, N'03bc10a5-4d1e-4d5f-9a3c-b6051c70bd89', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (480, N'MS2_M12_CB', N'Channel1.LoadShedding.Information Data Block.MS2_M12_CB', N'PLC : Tripped by PLC Load Shedding', 6, N'4152283a-b6a7-4893-a6aa-b404e74deecb', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (481, N'MS2_M12_CB_Fault', N'Channel1.LoadShedding.Information Data Block.MS2_M12_CB_Fault', N'PLC : CB MALFUNCTION', 17, N'708e2295-b7b0-43b1-bbf3-332c3aae431b', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (482, N'MS2_M1P_CB_Fault', N'Channel1.LoadShedding.Information Data Block.MS2_M1P_CB_Fault', N'PLC : CB MALFUNCTION', 17, N'162e2729-59d5-40a0-9894-634473ebd533', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (483, N'MS2_MC1_CB', N'Channel1.LoadShedding.Information Data Block.MS2_MC1_CB', N'PLC : Tripped by PLC Load Shedding', 6, N'94e160db-2399-4b2a-953a-8cc2f6f1e5a3', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (484, N'MS2_MC1_CB_Fault', N'Channel1.LoadShedding.Information Data Block.MS2_MC1_CB_Fault', N'PLC : CB MALFUNCTION', 17, N'9c551c84-a240-45c2-b6e6-e2f0d685d5df', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (485, N'MS2_MC2_CB', N'Channel1.LoadShedding.Information Data Block.MS2_MC2_CB', N'PLC : Tripped by PLC Load Shedding', 6, N'b2db96c1-37d4-4fe4-9c5f-5b7f5f650be4', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (486, N'MS2_MC2_CB_Fault', N'Channel1.LoadShedding.Information Data Block.MS2_MC2_CB_Fault', N'PLC : CB MALFUNCTION', 17, N'46f6b7cd-5d9e-42b0-8726-04c66d896091', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (487, N'MS2_ME1_CB', N'Channel1.LoadShedding.Information Data Block.MS2_ME1_CB', N'PLC : Tripped by PLC Load Shedding', 6, N'1ccadcab-ae7b-42f0-a6b4-71b59a016e39', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (488, N'MS2_ME1_CB_Fault', N'Channel1.LoadShedding.Information Data Block.MS2_ME1_CB_Fault', N'PLC : CB MALFUNCTION', 17, N'f545ec3f-3ef8-4a5f-8e80-280a768e8607', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (489, N'MS2_ME2_CB', N'Channel1.LoadShedding.Information Data Block.MS2_ME2_CB', N'PLC : Tripped by PLC Load Shedding', 6, N'902c7a72-9ad2-43d7-93ed-9e15e2f70ede', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (490, N'MS2_ME2_CB_Fault', N'Channel1.LoadShedding.Information Data Block.MS2_ME2_CB_Fault', N'PLC : CB MALFUNCTION', 17, N'5c281767-33b8-4f68-8574-3902eb62a325', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (491, N'MS2_MH1_CB', N'Channel1.LoadShedding.Information Data Block.MS2_MH1_CB', N'PLC : Tripped by PLC Load Shedding', 6, N'86f485b0-09bf-437e-9607-73e9c7f9bb4d', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (492, N'MS2_MH1_CB_Fault', N'Channel1.LoadShedding.Information Data Block.MS2_MH1_CB_Fault', N'PLC : CB MALFUNCTION', 17, N'815eb1d7-a617-45f0-8d4b-5de273133244', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (493, N'MS2_MH2_CB', N'Channel1.LoadShedding.Information Data Block.MS2_MH2_CB', N'PLC : Tripped by PLC Load Shedding', 6, N'fafbef06-5479-4c03-86a7-43879dd9094c', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (494, N'MS2_MH2_CB_Fault', N'Channel1.LoadShedding.Information Data Block.MS2_MH2_CB_Fault', N'PLC : CB MALFUNCTION', 17, N'0a403ed0-2453-4ba6-9af7-03d8540ac1a3', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (495, N'MS2_MK1_CB', N'Channel1.LoadShedding.Information Data Block.MS2_MK1_CB', N'PLC : Tripped by PLC Load Shedding', 6, N'095a6638-69f6-44db-a1f8-fb939fd7ec7a', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (496, N'MS2_MK1_CB_Fault', N'Channel1.LoadShedding.Information Data Block.MS2_MK1_CB_Fault', N'PLC : CB MALFUNCTION', 17, N'26ab44db-bcff-4ed4-80f8-5de4623420f8', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (497, N'MS2_MP1_CB', N'Channel1.LoadShedding.Information Data Block.MS2_MP1_CB', N'PLC : Tripped by PLC Load Shedding', 6, N'b7579673-c922-4014-97ee-42d636cdd9bd', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (498, N'MS2_MP1_CB_Fault', N'Channel1.LoadShedding.Information Data Block.MS2_MP1_CB_Fault', N'PLC : CB MALFUNCTION', 17, N'0f3d3d0d-b637-48f6-b044-084863d0ba0c', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (499, N'MS2_MP2_CB', N'Channel1.LoadShedding.Information Data Block.MS2_MP2_CB', N'PLC : Tripped by PLC Load Shedding', 6, N'c51d7e82-f93a-4829-9090-7b1891bd5f90', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (500, N'MS2_MP2_CB_Fault', N'Channel1.LoadShedding.Information Data Block.MS2_MP2_CB_Fault', N'PLC : CB MALFUNCTION', 17, N'760ea0ac-0641-4806-a0a4-b1419dedc5ad', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (501, N'MS2_MP4_CB', N'Channel1.LoadShedding.Information Data Block.MS2_MP4_CB', N'PLC : Tripped by PLC Load Shedding', 6, N'846e08ca-8606-44aa-8278-876068558948', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (502, N'MS2_MP4_CB_Fault', N'Channel1.LoadShedding.Information Data Block.MS2_MP4_CB_Fault', N'PLC : CB MALFUNCTION', 17, N'bde43030-5950-42c3-b8ba-ed3a016ae749', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (503, N'MS2_MR1_CB', N'Channel1.LoadShedding.Information Data Block.MS2_MR1_CB', N'PLC : Tripped by PLC Load Shedding', 6, N'e8366639-ff90-41f9-be31-bcb2fc46d25d', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (504, N'MS2_MR1_CB_Fault', N'Channel1.LoadShedding.Information Data Block.MS2_MR1_CB_Fault', N'PLC : CB MALFUNCTION', 17, N'bca218f7-96bc-4be8-baaa-1ae93dc21b62', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (505, N'MS2_MR2_CB', N'Channel1.LoadShedding.Information Data Block.MS2_MR2_CB', N'PLC : Tripped by PLC Load Shedding', 6, N'8ef58cd4-dab3-48be-abfb-a1bc80f3da42', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (506, N'MS2_MR2_CB_Fault', N'Channel1.LoadShedding.Information Data Block.MS2_MR2_CB_Fault', N'PLC : CB MALFUNCTION', 17, N'ca3a5c6e-ee32-48d3-9b94-9bf300779a0a', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (507, N'MS2_MS3_CB', N'Channel1.LoadShedding.Information Data Block.MS2_MS3_CB', N'PLC : Tripped by PLC Load Shedding', 6, N'a86ff242-1f9a-4255-80ab-582b78a049e8', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (508, N'MS2_MS3_CB_Fault', N'Channel1.LoadShedding.Information Data Block.MS2_MS3_CB_Fault', N'PLC : CB MALFUNCTION', 17, N'2ccd6615-f7b0-4c98-ab1d-672c74b54460', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (509, N'MS2_MT4_CB', N'Channel1.LoadShedding.Information Data Block.MS2_MT4_CB', N'PLC : Tripped by PLC Load Shedding', 6, N'4ea8ae77-685b-4c1e-8381-d3838cc7420a', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (510, N'MS2_MT4_CB_Fault', N'Channel1.LoadShedding.Information Data Block.MS2_MT4_CB_Fault', N'PLC : CB MALFUNCTION', 17, N'7494d1c6-402b-4607-a405-2dae3df5564f', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (511, N'MS2_MT6_CB', N'Channel1.LoadShedding.Information Data Block.MS2_MT6_CB', N'PLC : Tripped by PLC Load Shedding', 6, N'82498e62-f8af-44df-a8c2-d3d3fe1bb600', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (512, N'MS2_MT6_CB_Fault', N'Channel1.LoadShedding.Information Data Block.MS2_MT6_CB_Fault', N'PLC : CB MALFUNCTION', 17, N'f331e21d-c930-44f4-af34-0fc57b63b064', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (513, N'MS2_MT8_CB', N'Channel1.LoadShedding.Information Data Block.MS2_MT8_CB', N'PLC : Tripped by PLC Load Shedding', 6, N'726a7b05-b190-45cf-9bda-0b3bc3ba6119', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (514, N'MS2_MT8_CB_Fault', N'Channel1.LoadShedding.Information Data Block.MS2_MT8_CB_Fault', N'PLC : CB MALFUNCTION', 17, N'804fbba7-9f78-47d2-a985-e33c8bf764c3', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (515, N'MS2_MW1_CB', N'Channel1.LoadShedding.Information Data Block.MS2_MW1_CB', N'PLC : Tripped by PLC Load Shedding', 6, N'd092abf5-5e7e-45ba-a86d-9c5654feb891', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (516, N'MS2_MW1_CB_Fault', N'Channel1.LoadShedding.Information Data Block.MS2_MW1_CB_Fault', N'PLC : CB MALFUNCTION', 17, N'ffc21e49-d6cb-4a3a-9765-dd7969374db3', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (517, N'MS2_MW2_CB', N'Channel1.LoadShedding.Information Data Block.MS2_MW2_CB', N'PLC : Tripped by PLC Load Shedding', 6, N'5789e1ba-7ef3-4fd8-ad35-b7020c8befa6', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (518, N'MS2_MW2_CB_Fault', N'Channel1.LoadShedding.Information Data Block.MS2_MW2_CB_Fault', N'PLC : CB MALFUNCTION', 17, N'b7e5d3ee-1d33-4375-bfd2-5a16e69c289d', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (519, N'NS1_C1A_CB_Fault', N'Channel1.LoadShedding.Information Data Block.NS1_C1A_CB_Fault', N'PLC : CB MALFUNCTION', 17, N'fa1d4249-3599-4b0a-9bd2-9fc7930d8885', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (520, N'NS1_C1B_CB_Fault', N'Channel1.LoadShedding.Information Data Block.NS1_C1B_CB_Fault', N'PLC : CB MALFUNCTION', 17, N'21699744-5a6d-4b33-88eb-08a6ba92dd1b', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (521, N'NS1_C1C_CB_Fault', N'Channel1.LoadShedding.Information Data Block.NS1_C1C_CB_Fault', N'PLC : CB MALFUNCTION', 17, N'7ccd2077-7f2a-4085-9456-b71ab90f53e5', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (522, N'NS1_C2A_CB_Fault', N'Channel1.LoadShedding.Information Data Block.NS1_C2A_CB_Fault', N'PLC : CB MALFUNCTION', 17, N'886aed8c-89b3-4510-83aa-69f25fe47b2c', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (523, N'NS1_C2B_CB_Fault', N'Channel1.LoadShedding.Information Data Block.NS1_C2B_CB_Fault', N'PLC : CB MALFUNCTION', 17, N'722d5f47-c0c4-41af-baa6-fab5ef9abeb8', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (524, N'NS2_C2C_CB_Fault', N'Channel1.LoadShedding.Information Data Block.NS2_C2C_CB_Fault', N'PLC : CB MALFUNCTION', 17, N'f49d0140-6fba-47f8-b381-fb22530e06d5', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (525, N'NS2_C3A_CB_Fault', N'Channel1.LoadShedding.Information Data Block.NS2_C3A_CB_Fault', N'PLC : CB MALFUNCTION', 17, N'a0703b07-6c5c-42fb-95df-cdae8e7bb1f4', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (526, N'NS2_C3B_CB_Fault', N'Channel1.LoadShedding.Information Data Block.NS2_C3B_CB_Fault', N'PLC : CB MALFUNCTION', 17, N'225369d8-7d60-49d4-b45d-e9ebe7c750ad', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (527, N'NS2_C3C_CB_Fault', N'Channel1.LoadShedding.Information Data Block.NS2_C3C_CB_Fault', N'PLC : CB MALFUNCTION', 17, N'3f415a43-e798-4567-9505-52f97cc4c0c9', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (528, N'NS2_C4A_CB_Fault', N'Channel1.LoadShedding.Information Data Block.NS2_C4A_CB_Fault', N'PLC : CB MALFUNCTION', 17, N'9b9a24ef-e0f0-4879-a208-9515ef7b78ee', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (529, N'NS2_C4B_CB_Fault', N'Channel1.LoadShedding.Information Data Block.NS2_C4B_CB_Fault', N'PLC : CB MALFUNCTION', 17, N'b82f1ca0-6c9f-45d5-90f3-f6e34ea3f17c', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (530, N'NS2_C4C_CB_Fault', N'Channel1.LoadShedding.Information Data Block.NS2_C4C_CB_Fault', N'PLC : CB MALFUNCTION', 17, N'21b88d6c-9f7c-4d92-a4d7-3ce8831a8b9d', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (531, N'NS2_C5A_CB_Fault', N'Channel1.LoadShedding.Information Data Block.NS2_C5A_CB_Fault', N'PLC : CB MALFUNCTION', 17, N'2c32dd0c-2ba5-4b08-95b1-ead48cdbaae0', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (532, N'NS2_C5B_CB_Fault', N'Channel1.LoadShedding.Information Data Block.NS2_C5B_CB_Fault', N'PLC : CB MALFUNCTION', 17, N'e738c0be-55c9-4c7f-a13a-e29ef1bef451', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (533, N'NS2_C5C_CB_Fault', N'Channel1.LoadShedding.Information Data Block.NS2_C5C_CB_Fault', N'PLC : CB MALFUNCTION', 17, N'c8ffc504-cd10-4ced-ab04-19757feb35a7', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (534, N'PEL_110VDC_Battry_Charge', N'Channel1.LoadShedding.Information Data Block.PEL_110VDC_Battry_Charge', N'PLC : PEL-Monitoring 110  VDC Battry Charger Infeed  MCBs OK(F200,F400)', 17, N'8853ae3c-94d4-41e7-9bc9-d31b5a82045d', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (535, N'PEL_220VAC_MCB', N'Channel1.LoadShedding.Information Data Block.PEL_220VAC_MCB', N'PLC : PEL-Monitoring 24 VDC Common for DI Signals (I1645-I1646)(FT1/3)', 17, N'fbdff0d2-2d37-4cbb-9420-f927de2c57a5', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (536, N'PEL_24VDC_AnalogInput', N'Channel1.LoadShedding.Information Data Block.PEL_24VDC_AnalogInput', N'PLC : PEL-Monitoring 24  VDC Supply for Analog Input Cards (F601)', 17, N'11cf0e72-3e73-4521-ac21-115233335788', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (537, N'PEL_24VDC_Card_Site_PS1', N'Channel1.LoadShedding.Information Data Block.PEL_24VDC_Card_Site_PS1', N'PLC : PEL-Monitoring 24  VDC Power Supply for Cards _ Site (PS1)(G1)', 17, N'c6e65e5a-6f87-4d92-846e-3bdc8b90d187', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (538, N'PEL_24VDC_Card_Site_PS2', N'Channel1.LoadShedding.Information Data Block.PEL_24VDC_Card_Site_PS2', N'PLC : PEL-Monitoring 24  VDC Power Supply for Cards _ Site (PS2)(G2)', 17, N'4c64830c-87a2-4c81-b396-8c5c1331d7ce', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (539, N'PEL_24VDC_DI_1600_1601', N'Channel1.LoadShedding.Information Data Block.PEL_24VDC_DI_1600_1601', N'PLC : PEL-Monitoring 24 VDC Common for DI Signals (I1600-I1601)(FT1/1)', 17, N'a9f7a3c7-4036-4bd4-984c-6648282639e8', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (540, N'PEL_24VDC_DI_1602_1603', N'Channel1.LoadShedding.Information Data Block.PEL_24VDC_DI_1602_1603', N'PLC : PEL-Monitoring 24 VDC Common for DI Signals (I1602-I1603)(FT1/2)', 17, N'26ebe43e-a8d0-4452-bad5-9e1e73f4d378', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (541, N'PEL_24VDC_DI_1645_1646', N'Channel1.LoadShedding.Information Data Block.PEL_24VDC_DI_1645_1646', N'PLC : PEL-Monitoring 24 VDC Common for DI Signals (I1645-I1646)(FT1/3)', 17, N'a2757e4d-0c28-4fd8-82d4-2c41d1434d58', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (542, N'PEL_24VDC_DI_1647_1648', N'Channel1.LoadShedding.Information Data Block.PEL_24VDC_DI_1647_1648', N'PLC : PEL-Monitoring 24 VDC Common for DI Signals (I1647-I1648)(FT1/4)', 17, N'80c487ba-c617-4322-86fb-ce6a130afd4a', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (543, N'PEL_24VDC_Digital_Output', N'Channel1.LoadShedding.Information Data Block.PEL_24VDC_Digital_Output', N'PLC : PEL-Monitoring 24  VDC Supply for Digital Output Cards (F602-F607)', 17, N'960fc7a7-a52c-407e-9737-2880e6a81c43', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (544, N'PEL_24VDC_ET200', N'Channel1.LoadShedding.Information Data Block.PEL_24VDC_ET200', N'PLC : PEL-Monitoring 24 VDC Supply for ET200 Racks (F615-F617)', 17, N'8cb8bd9d-595b-4362-b094-60dda84a7333', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (545, N'PEL_24VDC_ET200_PS3', N'Channel1.LoadShedding.Information Data Block.PEL_24VDC_ET200_PS3', N'PLC : PEL-Monitoring 24  VDC Power Supply for ET200 Racks (PS3)(G3)', 17, N'7a36ac63-89c3-472d-833b-3421cee6e2ea', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (546, N'PEL_24VDC_ET200_PS4', N'Channel1.LoadShedding.Information Data Block.PEL_24VDC_ET200_PS4', N'PLC : PEL-Monitoring 24  VDC Power Supply for ET200 Racks (PS4)(G4)', 17, N'6fe991f7-e324-4ca0-b07e-1c73db68ec5d', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (547, N'PEL_24VDC_Multiplyers', N'Channel1.LoadShedding.Information Data Block.PEL_24VDC_Multiplyers', N'PLC : PEL-Monitoring 24  VDC Supply for Multiplyers(F631,F632)', 17, N'f1f97dc0-0efa-45b3-8d50-e10749d3a3d1', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (548, N'PEL_C01_CB', N'Channel1.LoadShedding.Information Data Block.PEL_C01_CB', N'PLC : Tripped by PLC Load Shedding', 6, N'aae65c7a-5e0f-4726-8f23-95cdf00b71db', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (549, N'PEL_C01_CB_Fault', N'Channel1.LoadShedding.Information Data Block.PEL_C01_CB_Fault', N'PLC : CB MALFUNCTION', 17, N'9b33b4c1-10bc-4d96-a1bf-84a57cee9339', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (550, N'PEL_C02_CB', N'Channel1.LoadShedding.Information Data Block.PEL_C02_CB', N'PLC : Tripped by PLC Load Shedding', 6, N'894814f5-19c5-42ac-b688-0248c96f6c5a', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (551, N'PEL_C02_CB_Fault', N'Channel1.LoadShedding.Information Data Block.PEL_C02_CB_Fault', N'PLC : CB MALFUNCTION', 17, N'172b9e1a-b6c5-4929-a09c-ae5c27db4c9d', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (552, N'PEL_C03_CB', N'Channel1.LoadShedding.Information Data Block.PEL_C03_CB', N'PLC : Tripped by PLC Load Shedding', 6, N'ae07bc7d-9f2d-43c8-b9dc-8897504c4c58', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (553, N'PEL_C03_CB_Fault', N'Channel1.LoadShedding.Information Data Block.PEL_C03_CB_Fault', N'PLC : CB MALFUNCTION', 17, N'f744fabe-0430-4c63-a9cd-ca0a5dced671', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (554, N'PEL_C04_CB', N'Channel1.LoadShedding.Information Data Block.PEL_C04_CB', N'PLC : Tripped by PLC Load Shedding', 6, N'1629e0ac-a272-4dcf-8696-8294f5533499', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (555, N'PEL_C04_CB_Fault', N'Channel1.LoadShedding.Information Data Block.PEL_C04_CB_Fault', N'PLC : CB MALFUNCTION', 17, N'5e773731-350a-4641-845e-21e7ac10f7be', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (556, N'PEL_C11_CB_Fault', N'Channel1.LoadShedding.Information Data Block.PEL_C11_CB_Fault', N'PLC : CB MALFUNCTION', 17, N'91c0da2f-d3ec-4956-9cd5-fcf8d8d05c38', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (557, N'PEL_C14_CB', N'Channel1.LoadShedding.Information Data Block.PEL_C14_CB', N'PLC : Tripped by PLC Load Shedding', 6, N'8e576a22-72b9-49bc-b660-87cd4dd7b3be', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (558, N'PEL_C14_CB_Fault', N'Channel1.LoadShedding.Information Data Block.PEL_C14_CB_Fault', N'PLC : CB MALFUNCTION', 17, N'e3be4c97-18e7-49e8-93c4-573ddd7570f6', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (559, N'PEL_C20_CB', N'Channel1.LoadShedding.Information Data Block.PEL_C20_CB', N'PLC : Tripped by PLC Load Shedding', 6, N'c6de8444-7683-43d3-ac44-6c44d6e41161', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (560, N'PEL_C20_CB_Fault', N'Channel1.LoadShedding.Information Data Block.PEL_C20_CB_Fault', N'PLC : CB MALFUNCTION', 17, N'ac9af285-24d5-490d-87f6-d0ef4786c081', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (561, N'PEL_C21_CB', N'Channel1.LoadShedding.Information Data Block.PEL_C21_CB', N'PLC : Tripped by PLC Load Shedding', 6, N'aa3e5999-7961-4637-943a-0091aab32902', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (562, N'PEL_C21_CB_Fault', N'Channel1.LoadShedding.Information Data Block.PEL_C21_CB_Fault', N'PLC : CB MALFUNCTION', 17, N'bf346514-a95a-4021-a98f-3dea6fdad668', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (563, N'PEL_C22_CB', N'Channel1.LoadShedding.Information Data Block.PEL_C22_CB', N'PLC : Tripped by PLC Load Shedding', 6, N'cc7fb2ae-061b-4019-8c10-3ece6c5cbacc', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (564, N'PEL_C22_CB_Fault', N'Channel1.LoadShedding.Information Data Block.PEL_C22_CB_Fault', N'PLC : CB MALFUNCTION', 17, N'4f55bad9-0e8a-4c95-a328-28843fb99023', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (565, N'PEL_C23_CB', N'Channel1.LoadShedding.Information Data Block.PEL_C23_CB', N'PLC : Tripped by PLC Load Shedding', 6, N'cf2f38e4-d93f-4e1f-88aa-07fd79e7a986', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (566, N'PEL_M_0VDC_DO_relays', N'Channel1.LoadShedding.Information Data Block.PEL_M_0VDC_DO_relays', N'PLC : PEL-Monitoring 24  VDC Supply for Analog Input Cards (F601)', 17, N'6095577e-0e30-458f-afa0-f360574282eb', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (567, N'RED_110VDC_Battry_Charge', N'Channel1.LoadShedding.Information Data Block.RED_110VDC_Battry_Charge', N'PLC : RED-Monitoring 110  VDC Battry Charger Infeed  MCBs OK(F200,F400)', 17, N'15068a93-7396-4d2f-a04c-873035d96929', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (568, N'RED_220VAC_MCB', N'Channel1.LoadShedding.Information Data Block.RED_220VAC_MCB', N'PLC : RED-Monitoring 24 VDC Supply for ET200 Racks (F615-F617)', 17, N'c6158b19-823a-464f-9096-a1b8d67804e8', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (569, N'RED_24VDC_AnalogInput', N'Channel1.LoadShedding.Information Data Block.RED_24VDC_AnalogInput', N'PLC : RED-Monitoring 24  VDC Supply for Analog Input Cards (F601-602)', 17, N'6f23b0dd-edca-4b74-9801-6a0c766c898a', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (570, N'RED_24VDC_Card_Site_PS1', N'Channel1.LoadShedding.Information Data Block.RED_24VDC_Card_Site_PS1', N'PLC : RED-Monitoring 24  VDC Power Supply for Cards _ Site (PS1)(G1)', 17, N'0700eaf7-ba38-4ab6-9a52-59c753ca6dd4', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (571, N'RED_24VDC_Card_Site_PS2', N'Channel1.LoadShedding.Information Data Block.RED_24VDC_Card_Site_PS2', N'PLC : RED-Monitoring 24  VDC Power Supply for Cards _ Site (PS2)(G2)', 17, N'298675d5-3a13-492e-94ef-35acef9cf102', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (572, N'RED_24VDC_DI_1500_1501', N'Channel1.LoadShedding.Information Data Block.RED_24VDC_DI_1500_1501', N'PLC : RED-Monitoring 24 VDC Common for DI Signals (I1500-I1501)(FT1/1)', 17, N'8511cf24-9295-485e-a7a9-31f32a679131', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (573, N'RED_24VDC_DI_1502_1503', N'Channel1.LoadShedding.Information Data Block.RED_24VDC_DI_1502_1503', N'PLC : RED-Monitoring 24 VDC Common for DI Signals (I1502-I1503)(FT1/2)', 17, N'548e91ba-958d-4280-a004-48b276879d38', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (574, N'RED_24VDC_DI_1504_1505', N'Channel1.LoadShedding.Information Data Block.RED_24VDC_DI_1504_1505', N'PLC : RED-Monitoring 24 VDC Common for DI Signals (I1504-I1505)(FT1/3)', 17, N'56759c17-f1fb-45d5-b453-4cd94069423e', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (575, N'RED_24VDC_DI_1506_1507', N'Channel1.LoadShedding.Information Data Block.RED_24VDC_DI_1506_1507', N'PLC : RED-Monitoring 24 VDC Common for DI Signals (I1506-I1507)(FT1/4)', 17, N'ae0a9d81-4a9e-4981-bf0a-d8a152afe22d', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (576, N'RED_24VDC_DI_1545_1546', N'Channel1.LoadShedding.Information Data Block.RED_24VDC_DI_1545_1546', N'PLC : RED-Monitoring 24 VDC Common for DI Signals (I1545-I1546)(FT1/5)', 17, N'1395047c-fa96-4735-8ac6-f1a5b860b7cb', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (577, N'RED_24VDC_DI_1547_1548', N'Channel1.LoadShedding.Information Data Block.RED_24VDC_DI_1547_1548', N'PLC : RED-Monitoring 24 VDC Common for DI Signals (I1547-I1548)(FT1/6)', 17, N'fe4ae83d-7624-4006-81e4-41419a53a9ba', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (578, N'RED_24VDC_Digital_Output', N'Channel1.LoadShedding.Information Data Block.RED_24VDC_Digital_Output', N'PLC : RED-Monitoring 24  VDC Supply for Digital Output Cards (F603-F610)', 17, N'5bf191ba-e599-48d7-8863-abc8d9f6ad92', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (579, N'RED_24VDC_ET200', N'Channel1.LoadShedding.Information Data Block.RED_24VDC_ET200', N'PLC : RED-Monitoring 24 VDC Supply for ET200 Racks (F615-F617)', 17, N'dedd03cf-093a-4d30-a11d-af1fe49af35a', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (580, N'RED_24VDC_ET200_PS3', N'Channel1.LoadShedding.Information Data Block.RED_24VDC_ET200_PS3', N'PLC : RED-Monitoring 24  VDC Power Supply for ET200 Racks (PS3)(G3)', 17, N'f3ab0496-4688-4257-a873-4d2712d3abe0', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (581, N'RED_24VDC_ET200_PS4', N'Channel1.LoadShedding.Information Data Block.RED_24VDC_ET200_PS4', N'PLC : RED-Monitoring 24  VDC Power Supply for ET200 Racks (PS4)(G4)', 17, N'b13ffcb7-2970-40b5-88f1-6c9da302d2f5', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (582, N'RED_24VDC_Multiplyers', N'Channel1.LoadShedding.Information Data Block.RED_24VDC_Multiplyers', N'PLC : RED-Monitoring 24  VDC Supply for Multiplyers(F631-F633)', 17, N'2d0c1eae-e92e-4358-b414-ec8b8bd69829', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (583, N'RED_C01A_CB', N'Channel1.LoadShedding.Information Data Block.RED_C01A_CB', N'PLC : Tripped by PLC Load Shedding', 6, N'2a57e337-4af1-479b-a09e-9dc0ba5029d8', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (584, N'RED_C01A_CB_Fault', N'Channel1.LoadShedding.Information Data Block.RED_C01A_CB_Fault', N'PLC : CB MALFUNCTION', 17, N'7b9992c4-899b-4778-bfb2-f4c61e8aade2', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (585, N'RED_C07A_CB', N'Channel1.LoadShedding.Information Data Block.RED_C07A_CB', N'PLC : Tripped by PLC Load Shedding', 6, N'8d2a37c0-9e5f-4d67-976f-26b84e527a6b', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (586, N'RED_C07A_CB_Fault', N'Channel1.LoadShedding.Information Data Block.RED_C07A_CB_Fault', N'PLC : CB MALFUNCTION', 17, N'298a48dd-2b4c-4ae6-ae64-616003e013f1', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (587, N'RED_C08A_CB', N'Channel1.LoadShedding.Information Data Block.RED_C08A_CB', N'PLC : Tripped by PLC Load Shedding', 6, N'8e51dfc0-835a-40c5-9f19-27f9636f1728', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (588, N'RED_C08A_CB_Fault', N'Channel1.LoadShedding.Information Data Block.RED_C08A_CB_Fault', N'PLC : CB MALFUNCTION', 17, N'10eaa431-fbb9-4ecd-9290-05783ccb3ede', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (589, N'RED_C11B_CB', N'Channel1.LoadShedding.Information Data Block.RED_C11B_CB', N'PLC : Tripped by PLC Load Shedding', 6, N'a84f771b-fc83-4bef-a769-972aaa668609', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (590, N'RED_C11B_CB_Fault', N'Channel1.LoadShedding.Information Data Block.RED_C11B_CB_Fault', N'PLC : CB MALFUNCTION', 17, N'9f11d448-27a5-4f35-a3f9-a455163c213e', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (591, N'RED_C17B_CB', N'Channel1.LoadShedding.Information Data Block.RED_C17B_CB', N'PLC : Tripped by PLC Load Shedding', 6, N'41ae566b-0ea0-4db4-a890-1baaff6225af', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (592, N'RED_C17B_CB_Fault', N'Channel1.LoadShedding.Information Data Block.RED_C17B_CB_Fault', N'PLC : CB MALFUNCTION', 17, N'06082ae6-511f-4dde-9842-f63cb9eb039c', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (593, N'RED_C18B_CB', N'Channel1.LoadShedding.Information Data Block.RED_C18B_CB', N'PLC : Tripped by PLC Load Shedding', 6, N'75fe7d6f-7679-41bc-8d67-8036e4820f70', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (594, N'RED_C18B_CB_Fault', N'Channel1.LoadShedding.Information Data Block.RED_C18B_CB_Fault', N'PLC : CB MALFUNCTION', 17, N'aa6c8ec9-f1b8-4355-ab8c-cecf7f026140', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (595, N'RED_C21C_CB', N'Channel1.LoadShedding.Information Data Block.RED_C21C_CB', N'PLC : Tripped by PLC Load Shedding', 6, N'cb47c1e4-e9b4-47fc-bf08-ac7d4b3e5f52', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (596, N'RED_C21C_CB_Fault', N'Channel1.LoadShedding.Information Data Block.RED_C21C_CB_Fault', N'PLC : CB MALFUNCTION', 17, N'aa3ef0ac-9eae-4fa7-bc2f-60df88b6bd5c', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (597, N'RED_C27C_CB', N'Channel1.LoadShedding.Information Data Block.RED_C27C_CB', N'PLC : Tripped by PLC Load Shedding', 6, N'79bbfc78-2cd9-4daa-9b13-897a32842c3c', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (598, N'RED_C27C_CB_Fault', N'Channel1.LoadShedding.Information Data Block.RED_C27C_CB_Fault', N'PLC : CB MALFUNCTION', 17, N'af4e407d-7837-48ac-a924-96b2b7727304', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (599, N'RED_C28C_CB', N'Channel1.LoadShedding.Information Data Block.RED_C28C_CB', N'PLC : Tripped by PLC Load Shedding', 6, N'ccdf68ff-179f-42a8-84ad-248f832b9264', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (600, N'RED_C28C_CB_Fault', N'Channel1.LoadShedding.Information Data Block.RED_C28C_CB_Fault', N'PLC : CB MALFUNCTION', 17, N'63645040-607b-4aef-970d-85be8195a37e', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (601, N'RED_C31D_CB', N'Channel1.LoadShedding.Information Data Block.RED_C31D_CB', N'PLC : Tripped by PLC Load Shedding', 6, N'79ea9753-5a8a-43dd-84c5-b31e4c248ffb', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (602, N'RED_C31D_CB_Fault', N'Channel1.LoadShedding.Information Data Block.RED_C31D_CB_Fault', N'PLC : CB MALFUNCTION', 17, N'9b3fc1f1-4cea-48e0-ab94-ebefc4b6d3a2', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (603, N'RED_C32D_CB', N'Channel1.LoadShedding.Information Data Block.RED_C32D_CB', N'PLC : Tripped by PLC Load Shedding', 6, N'31ce245f-ade3-4d7f-8761-153d626bce0e', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (604, N'RED_C32D_CB_Fault', N'Channel1.LoadShedding.Information Data Block.RED_C32D_CB_Fault', N'PLC : CB MALFUNCTION', 17, N'a6159ed1-5b3e-4a71-b279-02846eca4ffb', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (605, N'RED_C38D_CB', N'Channel1.LoadShedding.Information Data Block.RED_C38D_CB', N'PLC : Tripped by PLC Load Shedding', 6, N'2b1e55fc-20b5-4df0-aa3a-ae3de2b6a827', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (606, N'RED_C38D_CB_Fault', N'Channel1.LoadShedding.Information Data Block.RED_C38D_CB_Fault', N'PLC : CB MALFUNCTION', 17, N'cc6241c6-562a-4de7-8a45-7e1da1b3063b', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (607, N'RED_C39D_CB', N'Channel1.LoadShedding.Information Data Block.RED_C39D_CB', N'PLC : Tripped by PLC Load Shedding', 6, N'7abdd6f3-ce42-4f1a-ac49-1393b32a50d4', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (608, N'RED_C39D_CB_Fault', N'Channel1.LoadShedding.Information Data Block.RED_C39D_CB_Fault', N'PLC : CB MALFUNCTION', 17, N'1da88b12-91d1-48ac-ae00-d9e066befc26', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (609, N'RED_C41E_CB', N'Channel1.LoadShedding.Information Data Block.RED_C41E_CB', N'PLC : Tripped by PLC Load Shedding', 6, N'282005f4-65e1-4be0-88a5-08bef51fe6f4', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (610, N'RED_C41E_CB_Fault', N'Channel1.LoadShedding.Information Data Block.RED_C41E_CB_Fault', N'PLC : CB MALFUNCTION', 17, N'fa4bdd27-7fce-4849-9bd1-0b3c28fe7df0', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (611, N'RED_C47E_CB', N'Channel1.LoadShedding.Information Data Block.RED_C47E_CB', N'PLC : Tripped by PLC Load Shedding', 6, N'5ba61ce4-be91-4e6a-a70a-7d5bfd55ac52', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (612, N'RED_C47E_CB_Fault', N'Channel1.LoadShedding.Information Data Block.RED_C47E_CB_Fault', N'PLC : CB MALFUNCTION', 17, N'bbd3a39e-1263-4659-b5be-d7d8adb89006', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (613, N'RED_C51F_CB', N'Channel1.LoadShedding.Information Data Block.RED_C51F_CB', N'PLC : Tripped by PLC Load Shedding', 6, N'd797a4eb-c6e1-4fe8-afdd-e7349ec4fef9', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (614, N'RED_C51F_CB_Fault', N'Channel1.LoadShedding.Information Data Block.RED_C51F_CB_Fault', N'PLC : CB MALFUNCTION', 17, N'80663a55-a8cc-456f-b44f-1aeaa58622fa', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (615, N'RED_C52F_CB', N'Channel1.LoadShedding.Information Data Block.RED_C52F_CB', N'PLC : Tripped by PLC Load Shedding', 6, N'097b4e6c-e91c-49f1-9569-9cb9ebdeeaa9', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (616, N'RED_C52F_CB_Fault', N'Channel1.LoadShedding.Information Data Block.RED_C52F_CB_Fault', N'PLC : CB MALFUNCTION', 17, N'edf9ee78-cb42-4ec7-86b3-3cb3c89c39d5', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (617, N'RED_C53F_CB', N'Channel1.LoadShedding.Information Data Block.RED_C53F_CB', N'PLC : Tripped by PLC Load Shedding', 6, N'76d3d537-d3df-4640-8d29-e11f84c09ab0', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (618, N'RED_C53F_CB_Fault', N'Channel1.LoadShedding.Information Data Block.RED_C53F_CB_Fault', N'PLC : CB MALFUNCTION', 17, N'18d6951e-9c8f-442b-8787-6e3dc4d9d5b0', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (619, N'RED_C54F_CB', N'Channel1.LoadShedding.Information Data Block.RED_C54F_CB', N'PLC : Tripped by PLC Load Shedding', 6, N'2af60d66-5438-4b7e-bb62-d345a409e546', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (620, N'RED_C54F_CB_Fault', N'Channel1.LoadShedding.Information Data Block.RED_C54F_CB_Fault', N'PLC : CB MALFUNCTION', 17, N'3431a090-82d3-461e-9cc7-2c82bb271b5b', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (621, N'RED_C60F_CB', N'Channel1.LoadShedding.Information Data Block.RED_C60F_CB', N'PLC : Tripped by PLC Load Shedding', 6, N'562e19ac-f53f-44fc-a4e1-46e560187cea', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (622, N'RED_C60F_CB_Fault', N'Channel1.LoadShedding.Information Data Block.RED_C60F_CB_Fault', N'PLC : CB MALFUNCTION', 17, N'29f9b798-cda3-4771-8987-bb63e96c4a84', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (623, N'RED_M_0VDC_DO_relays', N'Channel1.LoadShedding.Information Data Block.RED_M_0VDC_DO_relays', N'PLC : RED-Monitoring M- (0 VDC) for load to be trip Do Relays(F701-F703)', 17, N'b63cb5f8-9ff6-48b9-92fa-891a739c0759', N'DMODigitalMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (624, N'Value_Load_Tripped_F10', N'Channel1.LoadShedding.Information Data Block.Value_Load_Tripped_F10', NULL, 0, N'e0620cb7-d336-4194-92f7-1f656b8a2b33', N'DMOAnalogMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (625, N'Value_Load_Tripped_F9', N'Channel1.LoadShedding.Information Data Block.Value_Load_Tripped_F9', NULL, 0, N'10f8a0e6-e746-4400-99c7-17aa2d534ec1', N'DMOAnalogMeasurement')
GO
INSERT [opc].[OPCMeasurement] ([ID], [ScadaTagName], [OPCTagName], [Description], [MessageConfiguration], [MeasurementId], [TagType]) VALUES (626, N'Value_Total_Load_Tripped', N'Channel1.LoadShedding.Information Data Block.Value_Total_Load_Tripped', NULL, 0, N'212bd1f9-2c03-4f89-90b2-35315602c24b', N'DMOAnalogMeasurement')
GO
SET IDENTITY_INSERT [opc].[OPCMeasurement] OFF
GO
