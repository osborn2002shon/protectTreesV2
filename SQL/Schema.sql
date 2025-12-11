USE [Forest_TreeProtect]
GO
/****** Object:  Table [dbo].[Tree_HealthRecord]    Script Date: 2025/8/19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Tree_HealthRecord](
        [healthID] [int] IDENTITY(1,1) NOT NULL,
        [treeID] [int] NOT NULL,
        [surveyDate] [date] NOT NULL,
        [surveyor] [nvarchar](100) NULL,
        [dataStatus] [tinyint] NOT NULL CONSTRAINT [DF_Tree_HealthRecord_dataStatus] DEFAULT ((0)),
        [memo] [nvarchar](max) NULL,
        [treeSignStatus] [tinyint] NULL,
        [latitude] [decimal](10, 6) NULL,
        [longitude] [decimal](10, 6) NULL,
        [treeHeight] [decimal](8, 2) NULL,
        [canopyArea] [decimal](8, 2) NULL,
        [girth100] [decimal](8, 2) NULL,
        [diameter100] [decimal](8, 2) NULL,
        [girth130] [decimal](8, 2) NULL,
        [diameter130] [decimal](8, 2) NULL,
        [measureNote] [nvarchar](200) NULL,
        [majorDiseaseBrownRoot] [bit] NULL,
        [majorDiseaseGanoderma] [bit] NULL,
        [majorDiseaseWoodDecayFungus] [bit] NULL,
        [majorDiseaseCanker] [bit] NULL,
        [majorDiseaseOther] [bit] NULL,
        [majorDiseaseOtherNote] [nvarchar](200) NULL,
        [majorPestRootTunnel] [bit] NULL,
        [majorPestRootChew] [bit] NULL,
        [majorPestRootLive] [bit] NULL,
        [majorPestBaseTunnel] [bit] NULL,
        [majorPestBaseChew] [bit] NULL,
        [majorPestBaseLive] [bit] NULL,
        [majorPestTrunkTunnel] [bit] NULL,
        [majorPestTrunkChew] [bit] NULL,
        [majorPestTrunkLive] [bit] NULL,
        [majorPestBranchTunnel] [bit] NULL,
        [majorPestBranchChew] [bit] NULL,
        [majorPestBranchLive] [bit] NULL,
        [majorPestCrownTunnel] [bit] NULL,
        [majorPestCrownChew] [bit] NULL,
        [majorPestCrownLive] [bit] NULL,
        [majorPestOtherTunnel] [bit] NULL,
        [majorPestOtherChew] [bit] NULL,
        [majorPestOtherLive] [bit] NULL,
        [generalPestRoot] [nvarchar](200) NULL,
        [generalPestBase] [nvarchar](200) NULL,
        [generalPestTrunk] [nvarchar](200) NULL,
        [generalPestBranch] [nvarchar](200) NULL,
        [generalPestCrown] [nvarchar](200) NULL,
        [generalPestOther] [nvarchar](200) NULL,
        [generalDiseaseRoot] [nvarchar](200) NULL,
        [generalDiseaseBase] [nvarchar](200) NULL,
        [generalDiseaseTrunk] [nvarchar](200) NULL,
        [generalDiseaseBranch] [nvarchar](200) NULL,
        [generalDiseaseCrown] [nvarchar](200) NULL,
        [generalDiseaseOther] [nvarchar](200) NULL,
        [pestOtherNote] [nvarchar](200) NULL,
        [rootDecayPercent] [decimal](5, 2) NULL,
        [rootCavityMaxDiameter] [decimal](6, 2) NULL,
        [rootWoundMaxDiameter] [decimal](6, 2) NULL,
        [rootMechanicalDamage] [bit] NULL,
        [rootMowingInjury] [bit] NULL,
        [rootInjury] [bit] NULL,
        [rootGirdling] [bit] NULL,
        [rootOtherNote] [nvarchar](200) NULL,
        [baseDecayPercent] [decimal](5, 2) NULL,
        [baseCavityMaxDiameter] [decimal](6, 2) NULL,
        [baseWoundMaxDiameter] [decimal](6, 2) NULL,
        [baseMechanicalDamage] [bit] NULL,
        [baseMowingInjury] [bit] NULL,
        [baseOtherNote] [nvarchar](200) NULL,
        [trunkDecayPercent] [decimal](5, 2) NULL,
        [trunkCavityMaxDiameter] [decimal](6, 2) NULL,
        [trunkWoundMaxDiameter] [decimal](6, 2) NULL,
        [trunkMechanicalDamage] [bit] NULL,
        [trunkIncludedBark] [bit] NULL,
        [trunkOtherNote] [nvarchar](200) NULL,
        [branchDecayPercent] [decimal](5, 2) NULL,
        [branchCavityMaxDiameter] [decimal](6, 2) NULL,
        [branchWoundMaxDiameter] [decimal](6, 2) NULL,
        [branchMechanicalDamage] [bit] NULL,
        [branchIncludedBark] [bit] NULL,
        [branchDrooping] [bit] NULL,
        [branchOtherNote] [nvarchar](200) NULL,
        [crownLeafCoveragePercent] [decimal](5, 2) NULL,
        [crownDeadBranchPercent] [decimal](5, 2) NULL,
        [crownHangingBranch] [bit] NULL,
        [crownOtherNote] [nvarchar](200) NULL,
        [pruningWrongDamage] [nvarchar](50) NULL,
        [pruningWoundHealing] [bit] NULL,
        [pruningEpiphyte] [bit] NULL,
        [pruningParasite] [bit] NULL,
        [pruningVine] [bit] NULL,
        [pruningOtherNote] [nvarchar](200) NULL,
        [supportCount] [int] NULL,
        [supportEmbedded] [bit] NULL,
        [supportOtherNote] [nvarchar](200) NULL,
        [siteCementPercent] [decimal](5, 2) NULL,
        [siteAsphaltPercent] [decimal](5, 2) NULL,
        [sitePlanter] [bit] NULL,
        [siteRecreationFacility] [bit] NULL,
        [siteDebrisStack] [bit] NULL,
        [siteBetweenBuildings] [bit] NULL,
        [siteSoilCompaction] [bit] NULL,
        [siteOverburiedSoil] [bit] NULL,
        [siteOtherNote] [nvarchar](200) NULL,
        [soilPh] [decimal](5, 2) NULL,
        [soilOrganicMatter] [decimal](6, 2) NULL,
        [soilEc] [decimal](6, 2) NULL,
        [managementStatus] [nvarchar](max) NULL,
        [priority] [nvarchar](50) NULL,
        [treatmentDescription] [nvarchar](max) NULL,
        [sourceUnit] [nvarchar](200) NULL,
        [sourceUnitID] [int] NULL,
        [insertAccountID] [int] NOT NULL,
        [insertDateTime] [datetime] NOT NULL CONSTRAINT [DF_Tree_HealthRecord_insertDateTime] DEFAULT (GETDATE()),
        [updateAccountID] [int] NULL,
        [updateDateTime] [datetime] NULL,
        [removeDateTime] [datetime] NULL,
        [removeAccountID] [int] NULL,
 CONSTRAINT [PK_Tree_HealthRecord] PRIMARY KEY CLUSTERED
(
        [healthID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
ALTER TABLE [dbo].[Tree_HealthRecord]  WITH CHECK ADD  CONSTRAINT [FK_Tree_HealthRecord_Tree_Record] FOREIGN KEY([treeID])
REFERENCES [dbo].[Tree_Record] ([treeID])
GO
ALTER TABLE [dbo].[Tree_HealthRecord] CHECK CONSTRAINT [FK_Tree_HealthRecord_Tree_Record]
GO
/****** Object:  Table [dbo].[Tree_HealthPhoto]    Script Date: 2025/8/19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Tree_HealthPhoto](
        [photoID] [int] IDENTITY(1,1) NOT NULL,
        [healthID] [int] NOT NULL,
        [fileName] [nvarchar](260) NOT NULL,
        [filePath] [nvarchar](500) NOT NULL,
        [fileSize] [int] NULL,
        [caption] [nvarchar](200) NULL,
        [insertAccountID] [int] NOT NULL,
        [insertDateTime] [datetime] NOT NULL CONSTRAINT [DF_Tree_HealthPhoto_insertDateTime] DEFAULT (GETDATE()),
        [removeDateTime] [datetime] NULL,
        [removeAccountID] [int] NULL,
 CONSTRAINT [PK_Tree_HealthPhoto] PRIMARY KEY CLUSTERED
(
        [photoID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[Tree_HealthPhoto]  WITH CHECK ADD  CONSTRAINT [FK_Tree_HealthPhoto_Tree_HealthRecord] FOREIGN KEY([healthID])
REFERENCES [dbo].[Tree_HealthRecord] ([healthID])
GO
ALTER TABLE [dbo].[Tree_HealthPhoto] CHECK CONSTRAINT [FK_Tree_HealthPhoto_Tree_HealthRecord]
GO
/****** Object:  Table [dbo].[Tree_HealthAttachment]    Script Date: 2025/8/19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Tree_HealthAttachment](
        [attachmentID] [int] IDENTITY(1,1) NOT NULL,
        [healthID] [int] NOT NULL,
        [fileName] [nvarchar](260) NOT NULL,
        [filePath] [nvarchar](500) NOT NULL,
        [fileSize] [int] NULL,
        [description] [nvarchar](200) NULL,
        [insertAccountID] [int] NOT NULL,
        [insertDateTime] [datetime] NOT NULL CONSTRAINT [DF_Tree_HealthAttachment_insertDateTime] DEFAULT (GETDATE()),
        [removeDateTime] [datetime] NULL,
        [removeAccountID] [int] NULL,
 CONSTRAINT [PK_Tree_HealthAttachment] PRIMARY KEY CLUSTERED
(
        [attachmentID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[Tree_HealthAttachment]  WITH CHECK ADD  CONSTRAINT [FK_Tree_HealthAttachment_Tree_HealthRecord] FOREIGN KEY([healthID])
REFERENCES [dbo].[Tree_HealthRecord] ([healthID])
GO
ALTER TABLE [dbo].[Tree_HealthAttachment] CHECK CONSTRAINT [FK_Tree_HealthAttachment_Tree_HealthRecord]
GO
/****** Object:  Table [dbo].[System_Menu]    Script Date: 2025/8/19 下午 07:16:49 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[System_Menu](
	[menuID] [int] IDENTITY(1,1) NOT NULL,
	[groupName] [nvarchar](50) NOT NULL,
	[menuName] [nvarchar](50) NOT NULL,
	[menuURL] [nvarchar](max) NOT NULL,
	[orderBy_group] [int] NOT NULL,
	[orderBy_menu] [int] NOT NULL,
	[isActive] [bit] NOT NULL,
	[isShow] [bit] NULL,
	[memo] [nvarchar](max) NULL,
 CONSTRAINT [PK_System_Menu] PRIMARY KEY CLUSTERED 
(
	[menuID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[System_MenuAu]    Script Date: 2025/8/19 下午 07:16:49 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[System_MenuAu](
	[auTypeID] [int] NOT NULL,
	[menuID] [int] NOT NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[System_Taiwan]    Script Date: 2025/8/19 下午 07:16:49 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[System_Taiwan](
        [twID] [int] IDENTITY(1,1) NOT NULL,
        [cityID] [int] NOT NULL,
        [city] [nvarchar](50) NOT NULL,
        [area] [nvarchar](50) NOT NULL,
        [cityCode] [nvarchar](20) NULL,
        [areaCode] [nvarchar](20) NULL,
CONSTRAINT [PK_System_Taiwan] PRIMARY KEY CLUSTERED
(
        [twID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[System_UserAuType]    Script Date: 2025/8/19 下午 07:16:49 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[System_UserAuType](
	[auTypeID] [int] IDENTITY(1,1) NOT NULL,
	[auTypeName] [nvarchar](50) NOT NULL,
	[memo] [nvarchar](max) NULL,
 CONSTRAINT [PK_Sys_AuType] PRIMARY KEY CLUSTERED 
(
	[auTypeID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[System_UserUnit]    Script Date: 2024/1/1 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[System_UserUnit](
        [unitID] [int] IDENTITY(1,1) NOT NULL,
        [unitName] [nvarchar](100) NOT NULL,
        [auTypeID] [int] NOT NULL,
 CONSTRAINT [PK_System_UserUnit] PRIMARY KEY CLUSTERED
(
        [unitID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

/****** Object:  Table [dbo].[User_Account]    Script Date: 2025/8/19 下午 07:16:49 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[User_Account](
	[accountID] [int] IDENTITY(1,1) NOT NULL,
	[auTypeID] [int] NOT NULL,
	[account] [nvarchar](50) NOT NULL,
	[password] [nvarchar](max) NULL,
	[name] [nvarchar](50) NOT NULL,
	[email] [nvarchar](max) NULL,
	[unit] [nvarchar](max) NULL,
	[mobile] [nvarchar](max) NULL,
	[memo] [nvarchar](max) NULL,
	[isActive] [bit] NOT NULL,
	[insertDateTime] [datetime] NOT NULL,
	[insertAccountID] [int] NOT NULL,
	[updateDateTime] [datetime] NULL,
	[updateAccountID] [int] NULL,
	[removeDateTime] [datetime] NULL,
	[removeAccountID] [int] NULL,
	[lastUpdatePWDateTime] [datetime] NULL,
	[SSOToken] [varchar](max) NULL,
 CONSTRAINT [PK_System_UserAccountInfo] PRIMARY KEY CLUSTERED 
(
	[accountID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[User_Log]    Script Date: 2025/8/19 下午 07:16:49 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[User_Log](
	[logID] [int] IDENTITY(1,1) NOT NULL,
	[accountID] [int] NOT NULL,
	[logDateTime] [datetime] NULL,
	[IP] [nvarchar](max) NOT NULL,
	[logItem] [nvarchar](50) NOT NULL,
	[logType] [nvarchar](50) NOT NULL,
	[memo] [nvarchar](max) NOT NULL,
 CONSTRAINT [PK_System_UserLog] PRIMARY KEY CLUSTERED 
(
	[logID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[User_MailVerify]    Script Date: 2025/8/19 下午 07:16:49 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[User_MailVerify](
	[verifyID] [int] IDENTITY(1,1) NOT NULL,
	[accountID] [int] NOT NULL,
        [verifyMail] [varchar](50) NULL,
        [verfiyCode] [varchar](50) NULL,
        [verifyType] [varchar](50) NULL,
        [expireDateTime] [datetime] NOT NULL,
 CONSTRAINT [PK_User_MailVerify] PRIMARY KEY CLUSTERED 
(
	[verifyID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[User_PasswordHistory]    Script Date: 2025/8/19 下午 07:16:49 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[User_PasswordHistory](
        [userID] [int] NOT NULL,
        [password] [varchar](max) NOT NULL,
        [setPasswordTime] [datetime] NOT NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

/****** Object:  Table [dbo].[Tree_Species]    Script Date: 2025/8/19 下午 07:16:49 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Tree_Species](
        [speciesID] [int] IDENTITY(1,1) NOT NULL,
        [commonName] [nvarchar](100) NOT NULL,
        [scientificName] [nvarchar](200) NULL,
        [aliasName] [nvarchar](200) NULL,
        [isNative] [bit] NOT NULL CONSTRAINT [DF_Tree_Species_isNative] DEFAULT ((0)),
        [isActive] [bit] NOT NULL CONSTRAINT [DF_Tree_Species_isActive] DEFAULT ((1)),
        [orderBy] [int] NULL,
        [memo] [nvarchar](max) NULL,
        [insertDateTime] [datetime] NOT NULL CONSTRAINT [DF_Tree_Species_insertDateTime] DEFAULT (GETDATE()),
        [insertAccountID] [int] NOT NULL,
        [updateDateTime] [datetime] NULL,
        [updateAccountID] [int] NULL
 CONSTRAINT [PK_Tree_Species] PRIMARY KEY CLUSTERED
(
        [speciesID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

/****** Object:  Table [dbo].[Tree_RecognitionCriterion]    Script Date: 2025/8/19 下午 07:16:49 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Tree_RecognitionCriterion](
        [criterionCode] [nvarchar](50) NOT NULL,
        [criterionName] [nvarchar](200) NOT NULL,
        [orderNo] [int] NOT NULL,
        [isActive] [bit] NOT NULL CONSTRAINT [DF_Tree_RecognitionCriterion_isActive] DEFAULT ((1)),
        [insertDateTime] [datetime] NOT NULL CONSTRAINT [DF_Tree_RecognitionCriterion_insertDateTime] DEFAULT (GETDATE()),
        [insertAccountID] [int] NOT NULL,
        [updateDateTime] [datetime] NULL,
        [updateAccountID] [int] NULL,
        [memo] [nvarchar](max) NULL,
 CONSTRAINT [PK_Tree_RecognitionCriterion] PRIMARY KEY CLUSTERED
(
        [criterionCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

/****** Object:  Table [dbo].[Tree_Record]    Script Date: 2025/8/19 下午 07:16:49 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Tree_Record](
        [treeID] [int] IDENTITY(1,1) NOT NULL,
        [systemTreeNo] [nvarchar](50) NULL,
        [agencyTreeNo] [nvarchar](50) NULL,
        [agencyJurisdictionCode] [nvarchar](50) NULL,
        [cityID] [int] NULL,
        [cityName] [nvarchar](50) NULL,
        [areaID] [int] NULL,
        [areaName] [nvarchar](50) NULL,
        [speciesID] [int] NULL,
        [speciesCommonName] [nvarchar](100) NULL,
        [speciesScientificName] [nvarchar](200) NULL,
        [manager] [nvarchar](100) NULL,
        [managerContact] [nvarchar](100) NULL,
        [surveyDate] [datetime] NULL,
        [surveyor] [nvarchar](100) NULL,
        [announcementDate] [datetime] NULL,
        [isAnnounced] [bit] NOT NULL CONSTRAINT [DF_Tree_Record_isAnnounced] DEFAULT ((0)),
        [treeStatus] [nvarchar](20) NOT NULL CONSTRAINT [DF_Tree_Record_treeStatus] DEFAULT (N'其他'),
        [editStatus] [int] NOT NULL CONSTRAINT [DF_Tree_Record_editStatus] DEFAULT ((0)),
        [treeCount] [int] NOT NULL CONSTRAINT [DF_Tree_Record_treeCount] DEFAULT ((1)),
        [site] [nvarchar](200) NULL,
        [latitude] [decimal](10, 6) NULL,
        [longitude] [decimal](10, 6) NULL,
        [landOwnership] [nvarchar](100) NULL,
        [landOwnershipNote] [nvarchar](max) NULL,
        [facilityDescription] [nvarchar](max) NULL,
        [memo] [nvarchar](max) NULL,
        [keywords] [nvarchar](200) NULL,
        [recognitionCriteria] [nvarchar](max) NULL,
        [recognitionNote] [nvarchar](max) NULL,
        [culturalHistoryIntro] [nvarchar](max) NULL,
        [estimatedPlantingYear] [nvarchar](50) NULL,
        [estimatedAgeNote] [nvarchar](max) NULL,
        [groupGrowthInfo] [nvarchar](max) NULL,
        [treeHeight] [decimal](10, 2) NULL,
        [breastHeightDiameter] [decimal](10, 2) NULL,
        [breastHeightCircumference] [decimal](10, 2) NULL,
        [canopyProjectionArea] [decimal](10, 2) NULL,
        [healthCondition] [nvarchar](max) NULL,
        [hasEpiphyte] [bit] NULL,
        [epiphyteDescription] [nvarchar](max) NULL,
        [hasParasite] [bit] NULL,
        [parasiteDescription] [nvarchar](max) NULL,
        [hasClimbingPlant] [bit] NULL,
        [climbingPlantDescription] [nvarchar](max) NULL,
        [surveyOtherNote] [nvarchar](max) NULL,
        [sourceUnit] [nvarchar](100) NULL,
        [sourceUnitID] [int] NULL,
        [insertAccountID] [int] NOT NULL,
        [insertDateTime] [datetime] NOT NULL CONSTRAINT [DF_Tree_Record_insertDateTime] DEFAULT (GETDATE()),
        [updateAccountID] [int] NULL,
        [updateDateTime] [datetime] NULL,
        [removeAccountID] [int] NULL,
        [removeDateTime] [datetime] NULL,
 CONSTRAINT [PK_Tree_Record] PRIMARY KEY CLUSTERED
(
        [treeID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

/****** Object:  Table [dbo].[Tree_RecordPhoto]    Script Date: 2025/8/19 下午 07:16:49 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Tree_RecordPhoto](
        [photoID] [int] IDENTITY(1,1) NOT NULL,
        [treeID] [int] NOT NULL,
        [fileName] [nvarchar](255) NOT NULL,
        [filePath] [nvarchar](500) NOT NULL,
        [caption] [nvarchar](200) NULL,
        [isCover] [bit] NOT NULL CONSTRAINT [DF_Tree_RecordPhoto_isCover] DEFAULT ((0)),
        [insertAccountID] [int] NOT NULL,
        [insertDateTime] [datetime] NOT NULL CONSTRAINT [DF_Tree_RecordPhoto_insertDateTime] DEFAULT (GETDATE()),
        [updateAccountID] [int] NULL,
        [updateDateTime] [datetime] NULL,
        [removeAccountID] [int] NULL,
        [removeDateTime] [datetime] NULL,
 CONSTRAINT [PK_Tree_RecordPhoto] PRIMARY KEY CLUSTERED
(
        [photoID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

/****** Object:  Table [dbo].[Tree_RecordLog]    Script Date: 2025/8/19 下午 07:16:49 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Tree_RecordLog](
        [logID] [int] IDENTITY(1,1) NOT NULL,
        [treeID] [int] NOT NULL,
        [actionType] [nvarchar](50) NOT NULL,
        [memo] [nvarchar](max) NULL,
        [ipAddress] [nvarchar](50) NULL,
        [accountID] [int] NULL,
        [account] [nvarchar](50) NULL,
        [accountName] [nvarchar](100) NULL,
        [accountUnit] [nvarchar](100) NULL,
        [logDateTime] [datetime] NOT NULL CONSTRAINT [DF_Tree_RecordLog_logDateTime] DEFAULT (GETDATE()),
 CONSTRAINT [PK_Tree_RecordLog] PRIMARY KEY CLUSTERED
(
        [logID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

ALTER TABLE [dbo].[Tree_Record]  WITH CHECK ADD  CONSTRAINT [FK_Tree_Record_Tree_Species] FOREIGN KEY([speciesID])
REFERENCES [dbo].[Tree_Species] ([speciesID])
GO
ALTER TABLE [dbo].[Tree_Record] CHECK CONSTRAINT [FK_Tree_Record_Tree_Species]
GO

ALTER TABLE [dbo].[Tree_RecordPhoto]  WITH CHECK ADD  CONSTRAINT [FK_Tree_RecordPhoto_Tree_Record] FOREIGN KEY([treeID])
REFERENCES [dbo].[Tree_Record] ([treeID])
GO
ALTER TABLE [dbo].[Tree_RecordPhoto] CHECK CONSTRAINT [FK_Tree_RecordPhoto_Tree_Record]
GO
ALTER TABLE [dbo].[Tree_RecordLog]  WITH CHECK ADD  CONSTRAINT [FK_Tree_RecordLog_Tree_Record] FOREIGN KEY([treeID])
REFERENCES [dbo].[Tree_Record] ([treeID])
GO
ALTER TABLE [dbo].[Tree_RecordLog] CHECK CONSTRAINT [FK_Tree_RecordLog_Tree_Record]
GO

DELETE FROM [dbo].[Tree_RecognitionCriterion];
INSERT INTO [dbo].[Tree_RecognitionCriterion](criterionCode, criterionName, orderNo, isActive, insertDateTime, insertAccountID)
VALUES
('age_100', N'一、樹齡達一百年以上。', 1, 1, GETDATE(), 0),
('dbh_large', N'二、離地一點三公尺處（以下簡稱胸高），
闊葉樹之樹幹胸高直徑達一點五公尺以上或胸高樹圍達四點七公尺以上；
針葉樹之樹幹胸高直徑達零點七五公尺以上或胸高樹圍達二點四公尺以上。', 2, 1, GETDATE(), 0),
('canopy_400', N'三、樹冠投影面積達四百平方公尺以上。', 3, 1, GETDATE(), 0),
('biodiversity', N'四、樹木生育地，形成具生物多樣性豐富之生態環境。', 4, 1, GETDATE(), 0),
('regional_representative', N'五、為區域具地理上代表性樹木。', 5, 1, GETDATE(), 0),
('aesthetic_value', N'六、具重大美學欣賞價值之景觀。', 6, 1, GETDATE(), 0),
('cultural_connection', N'七、與當地居民生活、情感、祭祀、民俗或信仰具有重大連結性。', 7, 1, GETDATE(), 0),
('historical_event', N'八、與重大歷史事件具有關聯性。', 8, 1, GETDATE(), 0),
('research_education', N'九、具有人文、科學研究及自然教育價值。', 9, 1, GETDATE(), 0),
('shared_memory', N'十、當地居民之共同記憶場域。', 10, 1, GETDATE(), 0),
('other_significance', N'十一、具有其他重要意義。', 11, 1, GETDATE(), 0);
