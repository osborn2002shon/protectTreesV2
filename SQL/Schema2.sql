USE [Forest_TreeProtect]
GO
/****** Object:  Table [dbo].[System_Menu]    Script Date: 2026/1/16 下午 08:01:49 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[System_Menu](
	[menuID] [int] IDENTITY(1,1) NOT NULL,
	[groupName] [nvarchar](50) NOT NULL,
	[menuName] [nvarchar](50) NOT NULL,
	[menuURL] [nvarchar](max) NOT NULL,
	[iconClass] [nvarchar](50) NULL,
	[orderBy_group] [int] NOT NULL,
	[orderBy_menu] [int] NOT NULL,
	[isActive] [bit] NOT NULL,
	[isShow] [bit] NOT NULL,
	[memo] [nvarchar](max) NULL,
 CONSTRAINT [PK_System_Menu] PRIMARY KEY CLUSTERED 
(
	[menuID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[System_MenuAu]    Script Date: 2026/1/16 下午 08:01:49 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[System_MenuAu](
	[auTypeID] [int] NOT NULL,
	[menuID] [int] NOT NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[System_Taiwan]    Script Date: 2026/1/16 下午 08:01:49 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[System_Taiwan](
	[twID] [int] IDENTITY(1,1) NOT NULL,
	[cityID] [int] NOT NULL,
	[city] [nvarchar](50) NOT NULL,
	[area] [nvarchar](50) NOT NULL,
	[cityCode] [nvarchar](50) NULL,
	[areaCode] [nvarchar](50) NULL,
 CONSTRAINT [PK_System_Taiwan] PRIMARY KEY CLUSTERED 
(
	[twID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[System_Unit]    Script Date: 2026/1/16 下午 08:01:49 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[System_Unit](
	[unitID] [int] IDENTITY(1,1) NOT NULL,
	[auTypeID] [int] NULL,
	[unitGroup] [nvarchar](max) NULL,
	[unitName] [nvarchar](max) NULL,
 CONSTRAINT [PK_System_Unit] PRIMARY KEY CLUSTERED 
(
	[unitID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[System_UnitCityMapping]    Script Date: 2026/1/16 下午 08:01:49 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[System_UnitCityMapping](
	[mappingID] [int] IDENTITY(1,1) NOT NULL,
	[unitID] [int] NOT NULL,
	[twID] [int] NOT NULL,
 CONSTRAINT [PK_System_UnitCityMapping] PRIMARY KEY CLUSTERED 
(
	[mappingID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[System_UnitUnitMapping]    Script Date: 2026/1/16 下午 08:01:49 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[System_UnitUnitMapping](
	[mappingID] [int] IDENTITY(1,1) NOT NULL,
	[unitID] [int] NOT NULL,
	[manageUnitID] [int] NOT NULL,
 CONSTRAINT [PK_System_UnitUnitMapping] PRIMARY KEY CLUSTERED 
(
	[mappingID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[System_UserAccount]    Script Date: 2026/1/16 下午 08:01:49 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[System_UserAccount](
	[accountID] [int] IDENTITY(1,1) NOT NULL,
	[accountType] [nvarchar](50) NOT NULL,
	[auTypeID] [int] NOT NULL,
	[unitID] [int] NOT NULL,
	[account] [nvarchar](50) NOT NULL,
	[password] [nvarchar](max) NULL,
	[name] [nvarchar](50) NOT NULL,
	[email] [nvarchar](max) NULL,
	[mobile] [nvarchar](max) NULL,
	[memo] [nvarchar](max) NULL,
	[verifyStatus] [bit] NULL,
	[isActive] [bit] NOT NULL,
	[lastLoginDateTime] [datetime] NULL,
	[lastUpdatePWDateTime] [datetime] NULL,
	[insertDateTime] [datetime] NOT NULL,
	[insertAccountID] [int] NOT NULL,
	[updateDateTime] [datetime] NULL,
	[updateAccountID] [int] NULL,
	[removeDateTime] [datetime] NULL,
	[removeAccountID] [int] NULL,
	[APP_USER_NODE_UUID] [nvarchar](max) NULL,
	[APP_COMPANY_UUID] [nvarchar](max) NULL,
	[APP_COMPANY_UUID_n] [nvarchar](max) NULL,
	[APP_DEPT_NODE_UUID] [nvarchar](max) NULL,
	[APP_DEPT_NODE_UUID_n] [nvarchar](max) NULL,
	[APP_USER_LOGIN_ID] [nvarchar](max) NULL,
 CONSTRAINT [PK_System_UserAccountInfo] PRIMARY KEY CLUSTERED 
(
	[accountID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[System_UserAccountMailVerify]    Script Date: 2026/1/16 下午 08:01:49 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[System_UserAccountMailVerify](
	[autoID] [int] IDENTITY(1,1) NOT NULL,
	[email] [nvarchar](max) NOT NULL,
	[name] [nvarchar](max) NOT NULL,
	[mobile] [nvarchar](max) NOT NULL,
	[auTypeID] [int] NOT NULL,
	[unitID] [int] NOT NULL,
	[memo] [nvarchar](max) NULL,
	[IP] [nvarchar](max) NOT NULL,
	[insertDateTime] [datetime] NOT NULL,
	[verifyDateTime] [datetime] NULL,
	[hashCode] [nvarchar](max) NULL,
 CONSTRAINT [PK_System_UserAccountMailVerify] PRIMARY KEY CLUSTERED 
(
	[autoID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[System_UserAuType]    Script Date: 2026/1/16 下午 08:01:49 ******/
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
/****** Object:  Table [dbo].[System_UserLog]    Script Date: 2026/1/16 下午 08:01:49 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[System_UserLog](
	[logID] [int] IDENTITY(1,1) NOT NULL,
	[accountID] [int] NOT NULL,
	[logDateTime] [datetime] NULL,
	[IP] [nvarchar](max) NOT NULL,
	[logItem] [nvarchar](50) NOT NULL,
	[logType] [nvarchar](50) NOT NULL,
	[memo] [nvarchar](max) NULL,
 CONSTRAINT [PK_System_UserLog] PRIMARY KEY CLUSTERED 
(
	[logID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[System_UserLoginError]    Script Date: 2026/1/16 下午 08:01:49 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[System_UserLoginError](
	[errorID] [int] IDENTITY(1,1) NOT NULL,
	[loginAccount] [nvarchar](max) NOT NULL,
	[errorDateTime] [datetime] NOT NULL,
	[errorIP] [nvarchar](max) NOT NULL,
	[clearDateTime] [datetime] NULL,
 CONSTRAINT [PK_System_UserLoginErrorCount] PRIMARY KEY CLUSTERED 
(
	[errorID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Tree_BatchTask]    Script Date: 2026/1/16 下午 08:01:49 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Tree_BatchTask](
	[taskID] [int] IDENTITY(1,1) NOT NULL,
	[batchType] [varchar](20) NOT NULL,
	[taskName] [nvarchar](200) NULL,
	[totalCount] [int] NULL,
	[successCount] [int] NULL,
	[failCount] [int] NULL,
	[memo] [nvarchar](500) NULL,
	[insertAccountID] [int] NOT NULL,
	[insertDateTime] [datetime] NULL,
	[removeAccountID] [int] NULL,
	[removeDateTime] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[taskID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Tree_BatchTaskLog]    Script Date: 2026/1/16 下午 08:01:49 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Tree_BatchTaskLog](
	[logID] [int] IDENTITY(1,1) NOT NULL,
	[taskID] [int] NOT NULL,
	[refKey] [varchar](50) NULL,
	[refDate] [date] NULL,
	[sourceItem] [nvarchar](200) NULL,
	[isSuccess] [bit] NULL,
	[resultMsg] [nvarchar](500) NULL,
PRIMARY KEY CLUSTERED 
(
	[logID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Tree_CareBatchSetting]    Script Date: 2026/1/16 下午 08:01:49 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Tree_CareBatchSetting](
	[settingID] [int] IDENTITY(1,1) NOT NULL,
	[accountID] [int] NOT NULL,
	[treeID] [int] NOT NULL,
	[insertDateTime] [datetime] NOT NULL,
 CONSTRAINT [PK_Tree_CareBatchSetting] PRIMARY KEY CLUSTERED 
(
	[settingID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Tree_CarePhoto]    Script Date: 2026/1/16 下午 08:01:49 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Tree_CarePhoto](
	[photoID] [int] IDENTITY(1,1) NOT NULL,
	[careID] [int] NOT NULL,
	[itemName] [nvarchar](200) NULL,
	[beforeFileName] [nvarchar](260) NULL,
	[beforeFilePath] [nvarchar](500) NULL,
	[beforeFileSize] [int] NULL,
	[afterFileName] [nvarchar](260) NULL,
	[afterFilePath] [nvarchar](500) NULL,
	[afterFileSize] [int] NULL,
	[insertAccountID] [int] NOT NULL,
	[insertDateTime] [datetime] NOT NULL,
	[removeDateTime] [datetime] NULL,
	[removeAccountID] [int] NULL,
 CONSTRAINT [PK_Tree_CarePhoto] PRIMARY KEY CLUSTERED 
(
	[photoID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Tree_CareRecord]    Script Date: 2026/1/16 下午 08:01:49 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Tree_CareRecord](
	[careID] [int] IDENTITY(1,1) NOT NULL,
	[treeID] [int] NOT NULL,
	[careDate] [date] NOT NULL,
	[recorder] [nvarchar](100) NULL,
	[reviewer] [nvarchar](100) NULL,
	[dataStatus] [tinyint] NOT NULL,
	[crownStatus] [tinyint] NULL,
	[crownSeasonalDormant] [bit] NULL,
	[crownDeadBranch] [bit] NULL,
	[crownDeadBranchPercent] [decimal](5, 2) NULL,
	[crownPest] [bit] NULL,
	[crownForeignObject] [bit] NULL,
	[crownOtherNote] [nvarchar](200) NULL,
	[trunkStatus] [tinyint] NULL,
	[trunkBarkDamage] [bit] NULL,
	[trunkDecay] [bit] NULL,
	[trunkTermiteTrail] [bit] NULL,
	[trunkLean] [bit] NULL,
	[trunkFungus] [bit] NULL,
	[trunkGummosis] [bit] NULL,
	[trunkVine] [bit] NULL,
	[trunkOtherNote] [nvarchar](200) NULL,
	[rootStatus] [tinyint] NULL,
	[rootDamage] [bit] NULL,
	[rootDecay] [bit] NULL,
	[rootExpose] [bit] NULL,
	[rootRot] [bit] NULL,
	[rootSucker] [bit] NULL,
	[rootOtherNote] [nvarchar](200) NULL,
	[envStatus] [tinyint] NULL,
	[envPitSmall] [bit] NULL,
	[envPaved] [bit] NULL,
	[envDebris] [bit] NULL,
	[envSoilCover] [bit] NULL,
	[envCompaction] [bit] NULL,
	[envWaterlog] [bit] NULL,
	[envNearFacility] [bit] NULL,
	[envOtherNote] [nvarchar](200) NULL,
	[adjacentStatus] [tinyint] NULL,
	[adjacentBuilding] [bit] NULL,
	[adjacentWire] [bit] NULL,
	[adjacentSignal] [bit] NULL,
	[adjacentOtherNote] [nvarchar](200) NULL,
	[task1Status] [tinyint] NULL,
	[task1Note] [nvarchar](500) NULL,
	[task2Status] [tinyint] NULL,
	[task2Note] [nvarchar](500) NULL,
	[task3Status] [tinyint] NULL,
	[task3Note] [nvarchar](500) NULL,
	[task4Status] [tinyint] NULL,
	[task4Note] [nvarchar](500) NULL,
	[task5Status] [tinyint] NULL,
	[task5Note] [nvarchar](500) NULL,
	[insertAccountID] [int] NOT NULL,
	[insertDateTime] [datetime] NOT NULL,
	[updateAccountID] [int] NULL,
	[updateDateTime] [datetime] NULL,
	[removeDateTime] [datetime] NULL,
	[removeAccountID] [int] NULL,
 CONSTRAINT [PK_Tree_CareRecord] PRIMARY KEY CLUSTERED 
(
	[careID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Tree_HealthAttachment]    Script Date: 2026/1/16 下午 08:01:49 ******/
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
	[insertDateTime] [datetime] NOT NULL,
	[removeDateTime] [datetime] NULL,
	[removeAccountID] [int] NULL,
 CONSTRAINT [PK_Tree_HealthAttachment] PRIMARY KEY CLUSTERED 
(
	[attachmentID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Tree_HealthBatchSetting]    Script Date: 2026/1/16 下午 08:01:49 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Tree_HealthBatchSetting](
	[settingID] [int] IDENTITY(1,1) NOT NULL,
	[accountID] [int] NOT NULL,
	[treeID] [int] NOT NULL,
	[insertDateTime] [datetime] NOT NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Tree_HealthPhoto]    Script Date: 2026/1/16 下午 08:01:49 ******/
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
	[insertDateTime] [datetime] NOT NULL,
	[removeDateTime] [datetime] NULL,
	[removeAccountID] [int] NULL,
 CONSTRAINT [PK_Tree_HealthPhoto] PRIMARY KEY CLUSTERED 
(
	[photoID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Tree_HealthRecord]    Script Date: 2026/1/16 下午 08:01:49 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Tree_HealthRecord](
	[healthID] [int] IDENTITY(1,1) NOT NULL,
	[treeID] [int] NOT NULL,
	[surveyDate] [date] NOT NULL,
	[surveyor] [nvarchar](100) NULL,
	[dataStatus] [tinyint] NOT NULL,
	[memo] [nvarchar](max) NULL,
	[treeSignStatus] [tinyint] NULL,
	[latitude] [decimal](10, 6) NULL,
	[longitude] [decimal](10, 6) NULL,
	[treeHeight] [decimal](8, 2) NULL,
	[canopyArea] [decimal](8, 2) NULL,
	[girth100] [nvarchar](200) NULL,
	[diameter100] [nvarchar](200) NULL,
	[girth130] [nvarchar](200) NULL,
	[diameter130] [nvarchar](200) NULL,
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
	[growthNote] [nvarchar](200) NULL,
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
	[soilPh] [nvarchar](50) NULL,
	[soilOrganicMatter] [nvarchar](50) NULL,
	[soilEc] [nvarchar](50) NULL,
	[managementStatus] [nvarchar](max) NULL,
	[priority] [nvarchar](50) NULL,
	[treatmentDescription] [nvarchar](max) NULL,
	[sourceUnit] [nvarchar](200) NULL,
	[sourceUnitID] [int] NULL,
	[insertAccountID] [int] NOT NULL,
	[insertDateTime] [datetime] NOT NULL,
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
/****** Object:  Table [dbo].[Tree_Log]    Script Date: 2026/1/16 下午 08:01:49 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Tree_Log](
	[logID] [int] IDENTITY(1,1) NOT NULL,
	[functionType] [nvarchar](20) NOT NULL,
	[dataID] [int] NOT NULL,
	[actionType] [nvarchar](50) NOT NULL,
	[memo] [nvarchar](max) NULL,
	[ipAddress] [nvarchar](50) NULL,
	[accountID] [int] NULL,
	[account] [nvarchar](50) NULL,
	[accountName] [nvarchar](100) NULL,
	[accountUnit] [nvarchar](100) NULL,
	[logDateTime] [datetime] NOT NULL,
 CONSTRAINT [PK_Tree_Log] PRIMARY KEY CLUSTERED 
(
	[logID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Tree_PatrolBatchSetting]    Script Date: 2026/1/16 下午 08:01:49 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Tree_PatrolBatchSetting](
	[settingID] [int] IDENTITY(1,1) NOT NULL,
	[accountID] [int] NOT NULL,
	[treeID] [int] NOT NULL,
	[insertDateTime] [datetime] NOT NULL,
 CONSTRAINT [PK_Tree_PatrolBatchSetting] PRIMARY KEY CLUSTERED 
(
	[settingID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Tree_PatrolPhoto]    Script Date: 2026/1/16 下午 08:01:49 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Tree_PatrolPhoto](
	[photoID] [int] IDENTITY(1,1) NOT NULL,
	[patrolID] [int] NOT NULL,
	[fileName] [nvarchar](260) NOT NULL,
	[filePath] [nvarchar](500) NOT NULL,
	[fileSize] [int] NULL,
	[caption] [nvarchar](200) NULL,
	[insertAccountID] [int] NOT NULL,
	[insertDateTime] [datetime] NOT NULL,
	[removeDateTime] [datetime] NULL,
	[removeAccountID] [int] NULL,
 CONSTRAINT [PK_Tree_PatrolPhoto] PRIMARY KEY CLUSTERED 
(
	[photoID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Tree_PatrolRecord]    Script Date: 2026/1/16 下午 08:01:49 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Tree_PatrolRecord](
	[patrolID] [int] IDENTITY(1,1) NOT NULL,
	[treeID] [int] NOT NULL,
	[patrolDate] [date] NOT NULL,
	[patroller] [nvarchar](100) NULL,
	[dataStatus] [tinyint] NOT NULL,
	[memo] [nvarchar](max) NULL,
	[hasPublicSafetyRisk] [bit] NOT NULL,
	[sourceUnit] [nvarchar](200) NULL,
	[sourceUnitID] [int] NULL,
	[sourceDataID] [int] NULL,
	[insertAccountID] [int] NOT NULL,
	[insertDateTime] [datetime] NOT NULL,
	[updateAccountID] [int] NULL,
	[updateDateTime] [datetime] NULL,
	[removeAccountID] [int] NULL,
	[removeDateTime] [datetime] NULL,
 CONSTRAINT [PK_Tree_PatrolRecord] PRIMARY KEY CLUSTERED 
(
	[patrolID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Tree_RecognitionCriterion]    Script Date: 2026/1/16 下午 08:01:49 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Tree_RecognitionCriterion](
	[criterionCode] [nvarchar](50) NOT NULL,
	[criterionName] [nvarchar](200) NOT NULL,
	[orderNo] [int] NOT NULL,
	[isActive] [bit] NOT NULL,
	[insertDateTime] [datetime] NOT NULL,
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
/****** Object:  Table [dbo].[Tree_Record]    Script Date: 2026/1/16 下午 08:01:49 ******/
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
	[areaID] [int] NULL,
	[speciesID] [int] NULL,
	[manager] [nvarchar](100) NULL,
	[managerContact] [nvarchar](100) NULL,
	[surveyDate] [datetime] NULL,
	[surveyor] [nvarchar](100) NULL,
	[announcementDate] [datetime] NULL,
	[isAnnounced] [bit] NOT NULL,
	[treeStatus] [nvarchar](20) NOT NULL,
	[editStatus] [int] NOT NULL,
	[treeCount] [int] NOT NULL,
	[breastHeightDiameter] [nvarchar](100) NULL,
	[breastHeightCircumference] [nvarchar](100) NULL,
	[canopyProjectionArea] [nvarchar](100) NULL,
	[healthCondition] [nvarchar](max) NULL,
	[hasEpiphyte] [bit] NULL,
	[epiphyteDescription] [nvarchar](max) NULL,
	[hasParasite] [bit] NULL,
	[parasiteDescription] [nvarchar](max) NULL,
	[hasClimbingPlant] [bit] NULL,
	[climbingPlantDescription] [nvarchar](max) NULL,
	[surveyOtherNote] [nvarchar](max) NULL,
	[site] [nvarchar](200) NULL,
	[latitude] [nvarchar](100) NULL,
	[longitude] [nvarchar](100) NULL,
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
	[treeHeight] [nvarchar](100) NULL,
	[sourceUnit] [nvarchar](100) NULL,
	[sourceUnitID] [int] NULL,
	[insertAccountID] [int] NOT NULL,
	[insertDateTime] [datetime] NOT NULL,
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
/****** Object:  Table [dbo].[Tree_Record_import]    Script Date: 2026/1/16 下午 08:01:49 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Tree_Record_import](
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
	[surveyDate] [nvarchar](100) NULL,
	[surveyor] [nvarchar](100) NULL,
	[announcementDate] [nvarchar](100) NULL,
	[isAnnounced] [bit] NOT NULL,
	[treeStatus] [nvarchar](20) NOT NULL,
	[editStatus] [int] NOT NULL,
	[treeCount] [nvarchar](100) NOT NULL,
	[breastHeightDiameter] [nvarchar](100) NULL,
	[breastHeightCircumference] [nvarchar](100) NULL,
	[canopyProjectionArea] [nvarchar](100) NULL,
	[healthCondition] [nvarchar](max) NULL,
	[hasEpiphyte] [bit] NULL,
	[epiphyteDescription] [nvarchar](max) NULL,
	[hasParasite] [bit] NULL,
	[parasiteDescription] [nvarchar](max) NULL,
	[hasClimbingPlant] [bit] NULL,
	[climbingPlantDescription] [nvarchar](max) NULL,
	[surveyOtherNote] [nvarchar](max) NULL,
	[site] [nvarchar](200) NULL,
	[latitude] [nvarchar](100) NULL,
	[longitude] [nvarchar](100) NULL,
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
	[treeHeight] [nvarchar](100) NULL,
	[sourceUnit] [nvarchar](100) NULL,
	[sourceUnitID] [int] NULL,
	[insertAccountID] [int] NOT NULL,
	[insertDateTime] [datetime] NOT NULL,
	[updateAccountID] [int] NULL,
	[updateDateTime] [datetime] NULL,
	[removeAccountID] [int] NULL,
	[removeDateTime] [datetime] NULL,
 CONSTRAINT [PK_Tree_Record_import] PRIMARY KEY CLUSTERED 
(
	[treeID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Tree_RecordPhoto]    Script Date: 2026/1/16 下午 08:01:49 ******/
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
	[isCover] [bit] NOT NULL,
	[insertAccountID] [int] NOT NULL,
	[insertDateTime] [datetime] NOT NULL,
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
/****** Object:  Table [dbo].[Tree_Species]    Script Date: 2026/1/16 下午 08:01:49 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Tree_Species](
	[speciesID] [int] IDENTITY(1,1) NOT NULL,
	[commonName] [nvarchar](100) NOT NULL,
	[scientificName] [nvarchar](200) NULL,
	[aliasName] [nvarchar](200) NULL,
	[isNative] [bit] NULL,
	[isActive] [bit] NOT NULL,
	[orderBy] [int] NULL,
	[memo] [nvarchar](max) NULL,
	[insertDateTime] [datetime] NOT NULL,
	[insertAccountID] [int] NOT NULL,
	[updateDateTime] [datetime] NULL,
	[updateAccountID] [int] NULL,
 CONSTRAINT [PK_Tree_Species] PRIMARY KEY CLUSTERED 
(
	[speciesID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  View [dbo].[View_UserInfo]    Script Date: 2026/1/16 下午 08:01:49 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO




CREATE VIEW [dbo].[View_UserInfo]
AS
	select 
		System_UserAccount.accountID,
		System_UserAccount.accountType,
		System_UserAccount.auTypeID,
		System_UserAccount.unitID,
		System_UserAccount.account,
		System_UserAccount.password,
		System_UserAccount.name,
		System_UserAccount.email,
		System_UserAccount.mobile,
		System_UserAccount.memo,
		System_UserAccount.verifyStatus,
		System_UserAccount.isActive,
		System_UserAccount.lastLoginDateTime,
		System_UserAccount.lastUpdatePWDateTime,
		insertDateTime, insertAccountID, updateDateTime, updateAccountID, removeDateTime, removeAccountID,
		APP_USER_NODE_UUID,
		APP_COMPANY_UUID,
		APP_COMPANY_UUID_n,
		APP_DEPT_NODE_UUID,
		APP_DEPT_NODE_UUID_n,
		APP_USER_LOGIN_ID,
		auTypeName, 
		unitGroup, unitName,
		cast(case when lastLoginDateTime is NULL then 1 else 0 end as bit) 'isFirstLogin',
		cast(case when GETDATE() >= DATEADD(DAY,181,CAST(lastLoginDateTime as date)) then 1 else 0 end as bit) 'isAutoStopLogin',
		cast(case when lastUpdatePWDateTime is null or GETDATE() >= DATEADD(DAY,181,CAST(lastUpdatePWDateTime as date)) then 1 else 0 end as bit) 'isNeedChangePW'
	from System_UserAccount
	left join System_UserAuType on System_UserAccount.auTypeID = System_UserAuType.auTypeID
	left join System_Unit on System_UserAccount.unitID = System_Unit.unitID
	where removeDateTime is null
GO
ALTER TABLE [dbo].[Tree_BatchTask] ADD  DEFAULT ((0)) FOR [totalCount]
GO
ALTER TABLE [dbo].[Tree_BatchTask] ADD  DEFAULT ((0)) FOR [successCount]
GO
ALTER TABLE [dbo].[Tree_BatchTask] ADD  DEFAULT ((0)) FOR [failCount]
GO
ALTER TABLE [dbo].[Tree_CareBatchSetting] ADD  CONSTRAINT [DF_Tree_CareBatchSetting_insertDateTime]  DEFAULT (getdate()) FOR [insertDateTime]
GO
ALTER TABLE [dbo].[Tree_CarePhoto] ADD  CONSTRAINT [DF_Tree_CarePhoto_insertDateTime]  DEFAULT (getdate()) FOR [insertDateTime]
GO
ALTER TABLE [dbo].[Tree_CareRecord] ADD  CONSTRAINT [DF_Tree_CareRecord_dataStatus]  DEFAULT ((0)) FOR [dataStatus]
GO
ALTER TABLE [dbo].[Tree_CareRecord] ADD  CONSTRAINT [DF_Tree_CareRecord_insertDateTime]  DEFAULT (getdate()) FOR [insertDateTime]
GO
ALTER TABLE [dbo].[Tree_HealthAttachment] ADD  CONSTRAINT [DF_Tree_HealthAttachment_insertDateTime]  DEFAULT (getdate()) FOR [insertDateTime]
GO
ALTER TABLE [dbo].[Tree_HealthBatchSetting] ADD  CONSTRAINT [DF_Tree_HealthBatchSetting_createTime]  DEFAULT (getdate()) FOR [insertDateTime]
GO
ALTER TABLE [dbo].[Tree_HealthPhoto] ADD  CONSTRAINT [DF_Tree_HealthPhoto_insertDateTime]  DEFAULT (getdate()) FOR [insertDateTime]
GO
ALTER TABLE [dbo].[Tree_HealthRecord] ADD  CONSTRAINT [DF_Tree_HealthRecord_dataStatus]  DEFAULT ((0)) FOR [dataStatus]
GO
ALTER TABLE [dbo].[Tree_HealthRecord] ADD  CONSTRAINT [DF_Tree_HealthRecord_insertDateTime]  DEFAULT (getdate()) FOR [insertDateTime]
GO
ALTER TABLE [dbo].[Tree_Log] ADD  CONSTRAINT [DF_Tree_Log_logDateTime]  DEFAULT (getdate()) FOR [logDateTime]
GO
ALTER TABLE [dbo].[Tree_PatrolBatchSetting] ADD  CONSTRAINT [DF_Tree_PatrolBatchSetting_insertDateTime]  DEFAULT (getdate()) FOR [insertDateTime]
GO
ALTER TABLE [dbo].[Tree_PatrolPhoto] ADD  CONSTRAINT [DF_Tree_PatrolPhoto_insertDateTime]  DEFAULT (getdate()) FOR [insertDateTime]
GO
ALTER TABLE [dbo].[Tree_PatrolRecord] ADD  CONSTRAINT [DF_Tree_PatrolRecord_dataStatus]  DEFAULT ((0)) FOR [dataStatus]
GO
ALTER TABLE [dbo].[Tree_PatrolRecord] ADD  CONSTRAINT [DF_Tree_PatrolRecord_hasPublicSafetyRisk]  DEFAULT ((0)) FOR [hasPublicSafetyRisk]
GO
ALTER TABLE [dbo].[Tree_PatrolRecord] ADD  CONSTRAINT [DF_Tree_PatrolRecord_insertDateTime]  DEFAULT (getdate()) FOR [insertDateTime]
GO
ALTER TABLE [dbo].[Tree_RecognitionCriterion] ADD  CONSTRAINT [DF_Tree_RecognitionCriterion_isActive]  DEFAULT ((1)) FOR [isActive]
GO
ALTER TABLE [dbo].[Tree_RecognitionCriterion] ADD  CONSTRAINT [DF_Tree_RecognitionCriterion_insertDateTime]  DEFAULT (getdate()) FOR [insertDateTime]
GO
ALTER TABLE [dbo].[Tree_Record] ADD  CONSTRAINT [DF_Tree_Record_isAnnounced]  DEFAULT ((0)) FOR [isAnnounced]
GO
ALTER TABLE [dbo].[Tree_Record] ADD  CONSTRAINT [DF_Tree_Record_treeStatus]  DEFAULT (N'其他') FOR [treeStatus]
GO
ALTER TABLE [dbo].[Tree_Record] ADD  CONSTRAINT [DF_Tree_Record_editStatus]  DEFAULT ((0)) FOR [editStatus]
GO
ALTER TABLE [dbo].[Tree_Record] ADD  CONSTRAINT [DF_Tree_Record_treeCount]  DEFAULT ((1)) FOR [treeCount]
GO
ALTER TABLE [dbo].[Tree_Record] ADD  CONSTRAINT [DF_Tree_Record_insertDateTime]  DEFAULT (getdate()) FOR [insertDateTime]
GO
ALTER TABLE [dbo].[Tree_RecordPhoto] ADD  CONSTRAINT [DF_Tree_RecordPhoto_isCover]  DEFAULT ((0)) FOR [isCover]
GO
ALTER TABLE [dbo].[Tree_RecordPhoto] ADD  CONSTRAINT [DF_Tree_RecordPhoto_insertDateTime]  DEFAULT (getdate()) FOR [insertDateTime]
GO
ALTER TABLE [dbo].[Tree_Species] ADD  CONSTRAINT [DF_Tree_Species_isActive]  DEFAULT ((1)) FOR [isActive]
GO
ALTER TABLE [dbo].[Tree_Species] ADD  CONSTRAINT [DF_Tree_Species_insertDateTime]  DEFAULT (getdate()) FOR [insertDateTime]
GO
ALTER TABLE [dbo].[Tree_CareBatchSetting]  WITH CHECK ADD  CONSTRAINT [FK_Tree_CareBatchSetting_Tree_Record] FOREIGN KEY([treeID])
REFERENCES [dbo].[Tree_Record] ([treeID])
GO
ALTER TABLE [dbo].[Tree_CareBatchSetting] CHECK CONSTRAINT [FK_Tree_CareBatchSetting_Tree_Record]
GO
ALTER TABLE [dbo].[Tree_CarePhoto]  WITH CHECK ADD  CONSTRAINT [FK_Tree_CarePhoto_Tree_CareRecord] FOREIGN KEY([careID])
REFERENCES [dbo].[Tree_CareRecord] ([careID])
GO
ALTER TABLE [dbo].[Tree_CarePhoto] CHECK CONSTRAINT [FK_Tree_CarePhoto_Tree_CareRecord]
GO
ALTER TABLE [dbo].[Tree_CareRecord]  WITH NOCHECK ADD  CONSTRAINT [FK_Tree_CareRecord_Tree_Record] FOREIGN KEY([treeID])
REFERENCES [dbo].[Tree_Record] ([treeID])
GO
ALTER TABLE [dbo].[Tree_CareRecord] CHECK CONSTRAINT [FK_Tree_CareRecord_Tree_Record]
GO
ALTER TABLE [dbo].[Tree_HealthAttachment]  WITH CHECK ADD  CONSTRAINT [FK_Tree_HealthAttachment_Tree_HealthRecord] FOREIGN KEY([healthID])
REFERENCES [dbo].[Tree_HealthRecord] ([healthID])
GO
ALTER TABLE [dbo].[Tree_HealthAttachment] CHECK CONSTRAINT [FK_Tree_HealthAttachment_Tree_HealthRecord]
GO
ALTER TABLE [dbo].[Tree_HealthPhoto]  WITH NOCHECK ADD  CONSTRAINT [FK_Tree_HealthPhoto_Tree_HealthRecord] FOREIGN KEY([healthID])
REFERENCES [dbo].[Tree_HealthRecord] ([healthID])
GO
ALTER TABLE [dbo].[Tree_HealthPhoto] CHECK CONSTRAINT [FK_Tree_HealthPhoto_Tree_HealthRecord]
GO
ALTER TABLE [dbo].[Tree_HealthRecord]  WITH NOCHECK ADD  CONSTRAINT [FK_Tree_HealthRecord_Tree_Record] FOREIGN KEY([treeID])
REFERENCES [dbo].[Tree_Record] ([treeID])
GO
ALTER TABLE [dbo].[Tree_HealthRecord] CHECK CONSTRAINT [FK_Tree_HealthRecord_Tree_Record]
GO
ALTER TABLE [dbo].[Tree_PatrolBatchSetting]  WITH CHECK ADD  CONSTRAINT [FK_Tree_PatrolBatchSetting_Tree_Record] FOREIGN KEY([treeID])
REFERENCES [dbo].[Tree_Record] ([treeID])
GO
ALTER TABLE [dbo].[Tree_PatrolBatchSetting] CHECK CONSTRAINT [FK_Tree_PatrolBatchSetting_Tree_Record]
GO
ALTER TABLE [dbo].[Tree_PatrolPhoto]  WITH CHECK ADD  CONSTRAINT [FK_Tree_PatrolPhoto_Tree_PatrolRecord] FOREIGN KEY([patrolID])
REFERENCES [dbo].[Tree_PatrolRecord] ([patrolID])
GO
ALTER TABLE [dbo].[Tree_PatrolPhoto] CHECK CONSTRAINT [FK_Tree_PatrolPhoto_Tree_PatrolRecord]
GO
ALTER TABLE [dbo].[Tree_PatrolRecord]  WITH CHECK ADD  CONSTRAINT [FK_Tree_PatrolRecord_Tree_Record] FOREIGN KEY([treeID])
REFERENCES [dbo].[Tree_Record] ([treeID])
GO
ALTER TABLE [dbo].[Tree_PatrolRecord] CHECK CONSTRAINT [FK_Tree_PatrolRecord_Tree_Record]
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
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[40] 4[20] 2[20] 3) )"
      End
      Begin PaneConfiguration = 1
         NumPanes = 3
         Configuration = "(H (1 [50] 4 [25] 3))"
      End
      Begin PaneConfiguration = 2
         NumPanes = 3
         Configuration = "(H (1 [50] 2 [25] 3))"
      End
      Begin PaneConfiguration = 3
         NumPanes = 3
         Configuration = "(H (4 [30] 2 [40] 3))"
      End
      Begin PaneConfiguration = 4
         NumPanes = 2
         Configuration = "(H (1 [56] 3))"
      End
      Begin PaneConfiguration = 5
         NumPanes = 2
         Configuration = "(H (2 [66] 3))"
      End
      Begin PaneConfiguration = 6
         NumPanes = 2
         Configuration = "(H (4 [50] 3))"
      End
      Begin PaneConfiguration = 7
         NumPanes = 1
         Configuration = "(V (3))"
      End
      Begin PaneConfiguration = 8
         NumPanes = 3
         Configuration = "(H (1[56] 4[18] 2) )"
      End
      Begin PaneConfiguration = 9
         NumPanes = 2
         Configuration = "(H (1 [75] 4))"
      End
      Begin PaneConfiguration = 10
         NumPanes = 2
         Configuration = "(H (1[66] 2) )"
      End
      Begin PaneConfiguration = 11
         NumPanes = 2
         Configuration = "(H (4 [60] 2))"
      End
      Begin PaneConfiguration = 12
         NumPanes = 1
         Configuration = "(H (1) )"
      End
      Begin PaneConfiguration = 13
         NumPanes = 1
         Configuration = "(V (4))"
      End
      Begin PaneConfiguration = 14
         NumPanes = 1
         Configuration = "(V (2))"
      End
      ActivePaneConfig = 0
   End
   Begin DiagramPane = 
      Begin Origin = 
         Top = 0
         Left = 0
      End
      Begin Tables = 
         Begin Table = "System_UserAccount"
            Begin Extent = 
               Top = 9
               Left = 57
               Bottom = 196
               Right = 382
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "System_UserAuType"
            Begin Extent = 
               Top = 9
               Left = 439
               Bottom = 171
               Right = 658
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "System_Unit"
            Begin Extent = 
               Top = 9
               Left = 715
               Bottom = 196
               Right = 934
            End
            DisplayFlags = 280
            TopColumn = 0
         End
      End
   End
   Begin SQLPane = 
   End
   Begin DataPane = 
      Begin ParameterDefaults = ""
      End
   End
   Begin CriteriaPane = 
      Begin ColumnWidths = 11
         Column = 1440
         Alias = 900
         Table = 1170
         Output = 720
         Append = 1400
         NewValue = 1170
         SortType = 1350
         SortOrder = 1410
         GroupBy = 1350
         Filter = 1350
         Or = 1350
         Or = 1350
         Or = 1350
      End
   End
End
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'View_UserInfo'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=1 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'View_UserInfo'
GO
