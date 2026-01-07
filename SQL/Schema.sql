USE [Forest_TreeProtect]
GO
/****** Object:  Table [dbo].[Tree_HealthRecord]    Script Date: 2025/8/19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Tree_HealthRecord](
        [healthID] [int] IDENTITY(1,1) NOT NULL, -- 健康檢測紀錄主鍵
        [treeID] [int] NOT NULL, -- 對應樹木基本資料編號
        [surveyDate] [date] NOT NULL, -- 調查日期
        [surveyor] [nvarchar](100) NULL, -- 調查人員姓名
        [dataStatus] [tinyint] NOT NULL CONSTRAINT [DF_Tree_HealthRecord_dataStatus] DEFAULT ((0)), -- 資料狀態代碼
        [memo] [nvarchar](max) NULL, -- 備註說明
        [treeSignStatus] [tinyint] NULL, -- 樹牌設置狀態
        [latitude] [decimal](10, 6) NULL, -- 緯度
        [longitude] [decimal](10, 6) NULL, -- 經度
        [treeHeight] [decimal](8, 2) NULL, -- 樹高(公尺)
        [canopyArea] [decimal](8, 2) NULL, -- 樹冠投影面積(平方公尺)
        [girth100] [nvarchar](200) NULL, -- 離地100公分處樹圍(公分)
        [diameter100] [nvarchar](200) NULL, -- 離地100公分處直徑(公分)
        [girth130] [nvarchar](200) NULL, -- 離地130公分處樹圍(公分)
        [diameter130] [nvarchar](200) NULL, -- 離地130公分處直徑(公分)
        [measureNote] [nvarchar](200) NULL, -- 量測備註
        [majorDiseaseBrownRoot] [bit] NULL, -- 主要病害-褐根病
        [majorDiseaseGanoderma] [bit] NULL, -- 主要病害-菇類/靈芝感染
        [majorDiseaseWoodDecayFungus] [bit] NULL, -- 主要病害-木腐菌
        [majorDiseaseCanker] [bit] NULL, -- 主要病害-潰瘍病
        [majorDiseaseOther] [bit] NULL, -- 主要病害-其他
        [majorDiseaseOtherNote] [nvarchar](200) NULL, -- 其他主要病害說明
        [majorPestRootTunnel] [bit] NULL, -- 主要害蟲-根部穿鑿
        [majorPestRootChew] [bit] NULL, -- 主要害蟲-根部啃食
        [majorPestRootLive] [bit] NULL, -- 主要害蟲-根部活蟲
        [majorPestBaseTunnel] [bit] NULL, -- 主要害蟲-基部穿鑿
        [majorPestBaseChew] [bit] NULL, -- 主要害蟲-基部啃食
        [majorPestBaseLive] [bit] NULL, -- 主要害蟲-基部活蟲
        [majorPestTrunkTunnel] [bit] NULL, -- 主要害蟲-樹幹穿鑿
        [majorPestTrunkChew] [bit] NULL, -- 主要害蟲-樹幹啃食
        [majorPestTrunkLive] [bit] NULL, -- 主要害蟲-樹幹活蟲
        [majorPestBranchTunnel] [bit] NULL, -- 主要害蟲-枝條穿鑿
        [majorPestBranchChew] [bit] NULL, -- 主要害蟲-枝條啃食
        [majorPestBranchLive] [bit] NULL, -- 主要害蟲-枝條活蟲
        [majorPestCrownTunnel] [bit] NULL, -- 主要害蟲-樹冠穿鑿
        [majorPestCrownChew] [bit] NULL, -- 主要害蟲-樹冠啃食
        [majorPestCrownLive] [bit] NULL, -- 主要害蟲-樹冠活蟲
        [majorPestOtherTunnel] [bit] NULL, -- 主要害蟲-其他部位穿鑿
        [majorPestOtherChew] [bit] NULL, -- 主要害蟲-其他部位啃食
        [majorPestOtherLive] [bit] NULL, -- 主要害蟲-其他部位活蟲
        [generalPestRoot] [nvarchar](200) NULL, -- 一般害蟲-根部情形
        [generalPestBase] [nvarchar](200) NULL, -- 一般害蟲-基部情形
        [generalPestTrunk] [nvarchar](200) NULL, -- 一般害蟲-樹幹情形
        [generalPestBranch] [nvarchar](200) NULL, -- 一般害蟲-枝條情形
        [generalPestCrown] [nvarchar](200) NULL, -- 一般害蟲-樹冠情形
        [generalPestOther] [nvarchar](200) NULL, -- 一般害蟲-其他情形
        [generalDiseaseRoot] [nvarchar](200) NULL, -- 一般病害-根部情形
        [generalDiseaseBase] [nvarchar](200) NULL, -- 一般病害-基部情形
        [generalDiseaseTrunk] [nvarchar](200) NULL, -- 一般病害-樹幹情形
        [generalDiseaseBranch] [nvarchar](200) NULL, -- 一般病害-枝條情形
        [generalDiseaseCrown] [nvarchar](200) NULL, -- 一般病害-樹冠情形
        [generalDiseaseOther] [nvarchar](200) NULL, -- 一般病害-其他情形
        [pestOtherNote] [nvarchar](200) NULL, -- 其他病蟲害補充說明
        [rootDecayPercent] [decimal](5, 2) NULL, -- 根部腐朽比例(%)
        [rootCavityMaxDiameter] [decimal](6, 2) NULL, -- 根部空洞最大直徑(公分)
        [rootWoundMaxDiameter] [decimal](6, 2) NULL, -- 根部傷口最大直徑(公分)
        [rootMechanicalDamage] [bit] NULL, -- 根部機械損傷
        [rootMowingInjury] [bit] NULL, -- 根部除草機傷害
        [rootInjury] [bit] NULL, -- 根部受損
        [rootGirdling] [bit] NULL, -- 根部纏勒情形
        [rootOtherNote] [nvarchar](200) NULL, -- 根部其他說明
        [baseDecayPercent] [decimal](5, 2) NULL, -- 基部腐朽比例(%)
        [baseCavityMaxDiameter] [decimal](6, 2) NULL, -- 基部空洞最大直徑(公分)
        [baseWoundMaxDiameter] [decimal](6, 2) NULL, -- 基部傷口最大直徑(公分)
        [baseMechanicalDamage] [bit] NULL, -- 基部機械損傷
        [baseMowingInjury] [bit] NULL, -- 基部除草機傷害
        [baseOtherNote] [nvarchar](200) NULL, -- 基部其他說明
        [trunkDecayPercent] [decimal](5, 2) NULL, -- 樹幹腐朽比例(%)
        [trunkCavityMaxDiameter] [decimal](6, 2) NULL, -- 樹幹空洞最大直徑(公分)
        [trunkWoundMaxDiameter] [decimal](6, 2) NULL, -- 樹幹傷口最大直徑(公分)
        [trunkMechanicalDamage] [bit] NULL, -- 樹幹機械損傷
        [trunkIncludedBark] [bit] NULL, -- 樹幹夾皮
        [trunkOtherNote] [nvarchar](200) NULL, -- 樹幹其他說明
        [branchDecayPercent] [decimal](5, 2) NULL, -- 枝條腐朽比例(%)
        [branchCavityMaxDiameter] [decimal](6, 2) NULL, -- 枝條空洞最大直徑(公分)
        [branchWoundMaxDiameter] [decimal](6, 2) NULL, -- 枝條傷口最大直徑(公分)
        [branchMechanicalDamage] [bit] NULL, -- 枝條機械損傷
        [branchIncludedBark] [bit] NULL, -- 枝條夾皮
        [branchDrooping] [bit] NULL, -- 枝條垂落
        [branchOtherNote] [nvarchar](200) NULL, -- 枝條其他說明
        [crownLeafCoveragePercent] [decimal](5, 2) NULL, -- 樹冠葉片覆蓋率(%)
        [crownDeadBranchPercent] [decimal](5, 2) NULL, -- 樹冠枯枝比例(%)
        [crownHangingBranch] [bit] NULL, -- 樹冠掛枝
        [crownOtherNote] [nvarchar](200) NULL, -- 樹冠其他說明
		[growthNote] [nvarchar](200) NULL, --　生長其他詳加說明
        [pruningWrongDamage] [nvarchar](50) NULL, -- 修剪不當損傷描述
        [pruningWoundHealing] [bit] NULL, -- 修剪傷口癒合狀況
        [pruningEpiphyte] [bit] NULL, -- 樹幹附生植物
        [pruningParasite] [bit] NULL, -- 寄生植物情形
        [pruningVine] [bit] NULL, -- 蔓藤纏繞情形
        [pruningOtherNote] [nvarchar](200) NULL, -- 修剪相關其他說明
        [supportCount] [int] NULL, -- 支撐設施數量
        [supportEmbedded] [bit] NULL, -- 支撐設施嵌入樹幹
        [supportOtherNote] [nvarchar](200) NULL, -- 支撐設施其他說明
        [siteCementPercent] [decimal](5, 2) NULL, -- 棲地水泥覆蓋率(%)
        [siteAsphaltPercent] [decimal](5, 2) NULL, -- 棲地柏油覆蓋率(%)
        [sitePlanter] [bit] NULL, -- 是否在花台內
        [siteRecreationFacility] [bit] NULL, -- 周邊是否有遊憩設施
        [siteDebrisStack] [bit] NULL, -- 是否堆置雜物
        [siteBetweenBuildings] [bit] NULL, -- 是否位於建物間
        [siteSoilCompaction] [bit] NULL, -- 土壤壓實情形
        [siteOverburiedSoil] [bit] NULL, -- 覆土過高情形
        [siteOtherNote] [nvarchar](200) NULL, -- 棲地其他說明
        [soilPh] [nvarchar](200) NULL, -- 土壤pH值
        [soilOrganicMatter] [nvarchar](200) NULL, -- 土壤有機質含量
        [soilEc] [nvarchar](200) NULL, -- 土壤電導度
        [managementStatus] [nvarchar](max) NULL, -- 管理狀態說明
        [priority] [nvarchar](50) NULL, -- 處置優先順序
        [treatmentDescription] [nvarchar](max) NULL, -- 處置或維護建議
        [sourceUnit] [nvarchar](200) NULL, -- 資料來源單位名稱
        [sourceUnitID] [int] NULL, -- 資料來源單位代碼
        [insertAccountID] [int] NOT NULL, -- 建立者帳號ID
        [insertDateTime] [datetime] NOT NULL CONSTRAINT [DF_Tree_HealthRecord_insertDateTime] DEFAULT (GETDATE()), -- 建立時間
        [updateAccountID] [int] NULL, -- 最後更新者帳號ID
        [updateDateTime] [datetime] NULL, -- 最後更新時間
        [removeDateTime] [datetime] NULL, -- 移除時間
        [removeAccountID] [int] NULL, -- 移除者帳號ID
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
        [photoID] [int] IDENTITY(1,1) NOT NULL, -- 照片主鍵編號
        [healthID] [int] NOT NULL, -- 健康檢測紀錄主鍵
        [fileName] [nvarchar](260) NOT NULL, -- 檔案名稱
        [filePath] [nvarchar](500) NOT NULL, -- 檔案儲存路徑
        [fileSize] [int] NULL, -- 檔案大小(位元組)
        [caption] [nvarchar](200) NULL, -- 照片或附件說明文字
        [insertAccountID] [int] NOT NULL, -- 建立者帳號ID
        [insertDateTime] [datetime] NOT NULL CONSTRAINT [DF_Tree_HealthPhoto_insertDateTime] DEFAULT (GETDATE()), -- 建立時間
        [removeDateTime] [datetime] NULL, -- 移除時間
        [removeAccountID] [int] NULL, -- 移除者帳號ID
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
        [attachmentID] [int] IDENTITY(1,1) NOT NULL, -- 附件主鍵編號
        [healthID] [int] NOT NULL, -- 健康檢測紀錄主鍵
        [fileName] [nvarchar](260) NOT NULL, -- 檔案名稱
        [filePath] [nvarchar](500) NOT NULL, -- 檔案儲存路徑
        [fileSize] [int] NULL, -- 檔案大小(位元組)
        [description] [nvarchar](200) NULL, -- 附件描述
        [insertAccountID] [int] NOT NULL, -- 建立者帳號ID
        [insertDateTime] [datetime] NOT NULL CONSTRAINT [DF_Tree_HealthAttachment_insertDateTime] DEFAULT (GETDATE()), -- 建立時間
        [removeDateTime] [datetime] NULL, -- 移除時間
        [removeAccountID] [int] NULL, -- 移除者帳號ID
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
/****** Object:  Table [dbo].[Tree_PatrolRecord]    Script Date: 2025/8/19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Tree_PatrolRecord](
        [patrolID] [int] IDENTITY(1,1) NOT NULL, -- 巡查紀錄主鍵
        [treeID] [int] NOT NULL, -- 對應樹木基本資料編號
        [patrolDate] [date] NOT NULL, -- 巡查日期
        [patroller] [nvarchar](100) NULL, -- 巡查人姓名
        [dataStatus] [tinyint] NOT NULL CONSTRAINT [DF_Tree_PatrolRecord_dataStatus] DEFAULT ((0)), -- 資料狀態代碼 (0=草稿,1=完稿)
        [memo] [nvarchar](max) NULL, -- 巡查備註
        [hasPublicSafetyRisk] [bit] NOT NULL CONSTRAINT [DF_Tree_PatrolRecord_hasPublicSafetyRisk] DEFAULT ((0)), -- 是否有危害公共安全風險或緊急狀況
        [sourceUnit] [nvarchar](200) NULL, -- 資料來源單位名稱
        [sourceUnitID] [int] NULL, -- 資料來源單位代碼
        [sourceDataID] [int] NULL, -- 資料來源ID
        [insertAccountID] [int] NOT NULL, -- 建立者帳號ID
        [insertDateTime] [datetime] NOT NULL CONSTRAINT [DF_Tree_PatrolRecord_insertDateTime] DEFAULT (GETDATE()), -- 建立時間
        [updateAccountID] [int] NULL, -- 最後更新者帳號ID
        [updateDateTime] [datetime] NULL, -- 最後更新時間
        [removeAccountID] [int] NULL, -- 移除者帳號ID
        [removeDateTime] [datetime] NULL, -- 移除時間
 CONSTRAINT [PK_Tree_PatrolRecord] PRIMARY KEY CLUSTERED
(
        [patrolID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
ALTER TABLE [dbo].[Tree_PatrolRecord]  WITH CHECK ADD  CONSTRAINT [FK_Tree_PatrolRecord_Tree_Record] FOREIGN KEY([treeID])
REFERENCES [dbo].[Tree_Record] ([treeID])
GO
ALTER TABLE [dbo].[Tree_PatrolRecord] CHECK CONSTRAINT [FK_Tree_PatrolRecord_Tree_Record]
GO
/****** Object:  Table [dbo].[Tree_PatrolPhoto]    Script Date: 2025/8/19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Tree_PatrolPhoto](
        [photoID] [int] IDENTITY(1,1) NOT NULL, -- 照片主鍵編號
        [patrolID] [int] NOT NULL, -- 巡查紀錄主鍵
        [fileName] [nvarchar](260) NOT NULL, -- 檔案名稱
        [filePath] [nvarchar](500) NOT NULL, -- 檔案儲存路徑
        [fileSize] [int] NULL, -- 檔案大小(位元組)
        [caption] [nvarchar](200) NULL, -- 照片或附件說明文字
        [insertAccountID] [int] NOT NULL, -- 建立者帳號ID
        [insertDateTime] [datetime] NOT NULL CONSTRAINT [DF_Tree_PatrolPhoto_insertDateTime] DEFAULT (GETDATE()), -- 建立時間
        [removeDateTime] [datetime] NULL, -- 移除時間
        [removeAccountID] [int] NULL, -- 移除者帳號ID
 CONSTRAINT [PK_Tree_PatrolPhoto] PRIMARY KEY CLUSTERED
(
        [photoID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[Tree_PatrolPhoto]  WITH CHECK ADD  CONSTRAINT [FK_Tree_PatrolPhoto_Tree_PatrolRecord] FOREIGN KEY([patrolID])
REFERENCES [dbo].[Tree_PatrolRecord] ([patrolID])
GO
ALTER TABLE [dbo].[Tree_PatrolPhoto] CHECK CONSTRAINT [FK_Tree_PatrolPhoto_Tree_PatrolRecord]
GO
/****** Object:  Table [dbo].[Tree_PatrolBatchSetting]    Script Date: 2025/8/19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Tree_PatrolBatchSetting](
        [settingID] [int] IDENTITY(1,1) NOT NULL, -- 暫存設定主鍵
        [accountID] [int] NOT NULL, -- 使用者帳號ID
        [treeID] [int] NOT NULL, -- 樹木基本資料編號
        [insertDateTime] [datetime] NOT NULL CONSTRAINT [DF_Tree_PatrolBatchSetting_insertDateTime] DEFAULT (GETDATE()), -- 建立時間
 CONSTRAINT [PK_Tree_PatrolBatchSetting] PRIMARY KEY CLUSTERED
(
        [settingID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[Tree_PatrolBatchSetting]  WITH CHECK ADD  CONSTRAINT [FK_Tree_PatrolBatchSetting_Tree_Record] FOREIGN KEY([treeID])
REFERENCES [dbo].[Tree_Record] ([treeID])
GO
ALTER TABLE [dbo].[Tree_PatrolBatchSetting] CHECK CONSTRAINT [FK_Tree_PatrolBatchSetting_Tree_Record]
GO
/****** Object:  Table [dbo].[System_Menu]    Script Date: 2025/8/19 下午 07:16:49 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[System_Menu](
	[menuID] [int] IDENTITY(1,1) NOT NULL, -- 選單項目主鍵
	[groupName] [nvarchar](50) NOT NULL, -- 選單群組名稱
	[menuName] [nvarchar](50) NOT NULL, -- 選單名稱
	[menuURL] [nvarchar](max) NOT NULL, -- 選單連結網址
	[orderBy_group] [int] NOT NULL, -- 群組排序序號
	[orderBy_menu] [int] NOT NULL, -- 選單排序序號
	[isActive] [bit] NOT NULL, -- 是否啟用
	[isShow] [bit] NULL, -- 是否顯示於前端
	[memo] [nvarchar](max) NULL, -- 備註說明
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
	[auTypeID] [int] NOT NULL, -- 權限類型ID
	[menuID] [int] NOT NULL -- 選單項目主鍵
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[System_Taiwan]    Script Date: 2025/8/19 下午 07:16:49 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[System_Taiwan](
        [twID] [int] IDENTITY(1,1) NOT NULL, -- 行政區資料主鍵
        [cityID] [int] NOT NULL, -- 城市代碼
        [city] [nvarchar](50) NOT NULL, -- 城市名稱
        [area] [nvarchar](50) NOT NULL, -- 區域名稱
        [cityCode] [nvarchar](20) NULL, -- 郵遞區號或市代碼
        [areaCode] [nvarchar](20) NULL, -- 區代碼
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
	[auTypeID] [int] IDENTITY(1,1) NOT NULL, -- 權限類型ID
	[auTypeName] [nvarchar](50) NOT NULL, -- 權限類型名稱
	[memo] [nvarchar](max) NULL, -- 備註說明
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
        [unitID] [int] IDENTITY(1,1) NOT NULL, -- 單位主鍵
        [unitName] [nvarchar](100) NOT NULL, -- 單位名稱
        [auTypeID] [int] NOT NULL, -- 權限類型ID
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
	[accountID] [int] IDENTITY(1,1) NOT NULL, -- 帳號主鍵編號
	[auTypeID] [int] NOT NULL, -- 權限類型ID
	[account] [nvarchar](50) NOT NULL, -- 登入帳號
	[password] [nvarchar](max) NULL, -- 密碼雜湊值
	[name] [nvarchar](50) NOT NULL, -- 使用者姓名
	[email] [nvarchar](max) NULL, -- 電子郵件
	[unit] [nvarchar](max) NULL, -- 隸屬單位名稱
	[mobile] [nvarchar](max) NULL, -- 行動電話
	[memo] [nvarchar](max) NULL, -- 備註說明
	[isActive] [bit] NOT NULL, -- 是否啟用
	[insertDateTime] [datetime] NOT NULL, -- 建立時間
	[insertAccountID] [int] NOT NULL, -- 建立者帳號ID
	[updateDateTime] [datetime] NULL, -- 最後更新時間
	[updateAccountID] [int] NULL, -- 最後更新者帳號ID
	[removeDateTime] [datetime] NULL, -- 移除時間
	[removeAccountID] [int] NULL, -- 移除者帳號ID
	[lastUpdatePWDateTime] [datetime] NULL, -- 最後修改密碼時間
	[SSOToken] [varchar](max) NULL, -- 單一登入Token
CONSTRAINT [PK_System_UserAccountInfo] PRIMARY KEY CLUSTERED 
(
	[accountID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[User_Area_Mapping]    Script Date: 2025/8/19 下午 07:16:49 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[User_Area_Mapping](
        [mappingID] [int] IDENTITY(1,1) NOT NULL, -- 帳號區域對應主鍵
        [accountID] [int] NOT NULL, -- 帳號主鍵編號
        [city] [nvarchar](50) NOT NULL, -- 可管轄城市代碼，全部為-1
        [area] [nvarchar](50) NOT NULL, -- 可管轄區域代碼，全部為-1
 CONSTRAINT [PK_User_Area_Mapping] PRIMARY KEY CLUSTERED 
(
        [mappingID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[User_Area_Mapping]  WITH CHECK ADD  CONSTRAINT [FK_User_Area_Mapping_User_Account] FOREIGN KEY([accountID])
REFERENCES [dbo].[User_Account] ([accountID])
GO
ALTER TABLE [dbo].[User_Area_Mapping] CHECK CONSTRAINT [FK_User_Area_Mapping_User_Account]
GO
CREATE UNIQUE NONCLUSTERED INDEX [UX_User_Area_Mapping_Account_City_Area] ON [dbo].[User_Area_Mapping]
(
        [accountID] ASC,
        [city] ASC,
        [area] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[User_Log]    Script Date: 2025/8/19 下午 07:16:49 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[User_Log](
	[logID] [int] IDENTITY(1,1) NOT NULL, -- 操作紀錄主鍵
	[accountID] [int] NOT NULL, -- 帳號主鍵編號
	[logDateTime] [datetime] NULL, -- 紀錄時間
	[IP] [nvarchar](max) NOT NULL, -- 操作IP位址
	[logItem] [nvarchar](50) NOT NULL, -- 紀錄項目
	[logType] [nvarchar](50) NOT NULL, -- 紀錄類型
	[memo] [nvarchar](max) NOT NULL, -- 備註說明
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
	[verifyID] [int] IDENTITY(1,1) NOT NULL, -- 驗證資料主鍵
	[accountID] [int] NOT NULL, -- 帳號主鍵編號
        [verifyMail] [varchar](50) NULL, -- 驗證電子郵件
        [verfiyCode] [varchar](50) NULL, -- 驗證碼
        [verifyType] [varchar](50) NULL, -- 驗證類型
        [expireDateTime] [datetime] NOT NULL, -- 驗證碼到期時間
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
        [userID] [int] NOT NULL, -- 使用者主鍵
        [password] [varchar](max) NOT NULL, -- 密碼雜湊值
        [setPasswordTime] [datetime] NOT NULL -- 設定密碼時間
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

/****** Object:  Table [dbo].[Tree_Species]    Script Date: 2025/8/19 下午 07:16:49 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Tree_Species](
        [speciesID] [int] IDENTITY(1,1) NOT NULL, -- 樹種主鍵
        [commonName] [nvarchar](100) NOT NULL, -- 樹種俗名
        [scientificName] [nvarchar](200) NULL, -- 樹種學名
        [aliasName] [nvarchar](200) NULL, -- 樹種別名
        [isNative] [bit] NOT NULL CONSTRAINT [DF_Tree_Species_isNative] DEFAULT ((0)), -- 是否原生種
        [isActive] [bit] NOT NULL CONSTRAINT [DF_Tree_Species_isActive] DEFAULT ((1)), -- 是否啟用
        [orderBy] [int] NULL, -- 排序序號
        [memo] [nvarchar](max) NULL, -- 備註說明
        [insertDateTime] [datetime] NOT NULL CONSTRAINT [DF_Tree_Species_insertDateTime] DEFAULT (GETDATE()), -- 建立時間
        [insertAccountID] [int] NOT NULL, -- 建立者帳號ID
        [updateDateTime] [datetime] NULL, -- 最後更新時間
        [updateAccountID] [int] NULL -- 最後更新者帳號ID
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
        [criterionCode] [nvarchar](50) NOT NULL, -- 認定基準代碼
        [criterionName] [nvarchar](200) NOT NULL, -- 認定基準名稱
        [orderNo] [int] NOT NULL, -- 顯示排序序號
        [isActive] [bit] NOT NULL CONSTRAINT [DF_Tree_RecognitionCriterion_isActive] DEFAULT ((1)), -- 是否啟用
        [insertDateTime] [datetime] NOT NULL CONSTRAINT [DF_Tree_RecognitionCriterion_insertDateTime] DEFAULT (GETDATE()), -- 建立時間
        [insertAccountID] [int] NOT NULL, -- 建立者帳號ID
        [updateDateTime] [datetime] NULL, -- 最後更新時間
        [updateAccountID] [int] NULL, -- 最後更新者帳號ID
        [memo] [nvarchar](max) NULL, -- 備註說明
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
        [treeID] [int] IDENTITY(1,1) NOT NULL, -- 對應樹木基本資料編號
        [systemTreeNo] [nvarchar](50) NULL, -- 系統樹籍編號
        [agencyTreeNo] [nvarchar](50) NULL, -- 主管機關樹木編號
        [agencyJurisdictionCode] [nvarchar](50) NULL, -- 主管機關轄區代碼
        [cityID] [int] NULL, -- 城市代碼
        [areaID] [int] NULL, -- 區域代碼
        [speciesID] [int] NULL, -- 樹種主鍵
        [manager] [nvarchar](100) NULL, -- 管理人員 
        [managerContact] [nvarchar](100) NULL, -- 管理人聯絡電話
        [surveyDate] [datetime] NULL, -- 調查日期
        [surveyor] [nvarchar](100) NULL, -- 調查人員
        [announcementDate] [datetime] NULL, -- 公告日期（已公告列管才會有公告日期）
        [isAnnounced] [bit] NOT NULL CONSTRAINT [DF_Tree_Record_isAnnounced] DEFAULT ((0)), -- 是否已公告列管（樹籍狀態為已公告列管才為1）
        [treeStatus] [nvarchar](20) NOT NULL CONSTRAINT [DF_Tree_Record_treeStatus] DEFAULT (N'其他'), -- 樹籍狀態（已公告列管／符合標準／其他）
        [editStatus] [int] NOT NULL CONSTRAINT [DF_Tree_Record_editStatus] DEFAULT ((0)), -- 編輯狀態（草稿／完稿）
        [treeCount] [int] NOT NULL CONSTRAINT [DF_Tree_Record_treeCount] DEFAULT ((1)), -- 數量
        [site] [nvarchar](200) NULL, -- 坐落地點
        [latitude] [nvarchar](max) NULL, -- 座標(WGS84)：緯度(N)
        [longitude] [nvarchar](max) NULL, -- 座標(WGS84)：經度(E)
        [landOwnership] [nvarchar](100) NULL, -- 土地權屬（國有／公有／私有／其他／無資料）
        [landOwnershipNote] [nvarchar](max) NULL, -- 土地權屬備註
        [facilityDescription] [nvarchar](max) NULL, -- 管理設施描述
        [recognitionCriteria] [nvarchar](max) NULL, -- 受保護認定理由（存放[Tree_RecognitionCriterion].[criterionCode]，使用逗號分隔多筆）
        [recognitionNote] [nvarchar](max) NULL, -- 認定理由備註說明
        [culturalHistoryIntro] [nvarchar](max) NULL, -- 文化歷史價值介紹
        [estimatedPlantingYear] [nvarchar](50) NULL, -- 推估種植年間
        [estimatedAgeNote] [nvarchar](max) NULL, -- 推估樹齡備註
        [groupGrowthInfo] [nvarchar](max) NULL, -- 群生竹木或行道樹生長資訊
        [treeHeight] [nvarchar](max) NULL, -- 樹高(公尺)
        [breastHeightDiameter] [nvarchar](max) NULL, -- 胸高直徑(公分)
        [breastHeightCircumference] [nvarchar](max) NULL, -- 胸高樹圍(公分)
        [canopyProjectionArea] [nvarchar](max) NULL, -- 樹冠投影面積(平方公尺)
        [healthCondition] [nvarchar](max) NULL, -- 樹木健康及生育地概況
        [epiphyteDescription] [nvarchar](max) NULL, -- 附生植物概況
        [parasiteDescription] [nvarchar](max) NULL, -- 寄生植物概況
        [climbingPlantDescription] [nvarchar](max) NULL, -- 纏勒植物概況
        [surveyOtherNote] [nvarchar](max) NULL, -- 其他備註
        [sourceUnit] [nvarchar](100) NULL, -- 資料來源單位名稱
        [sourceUnitID] [int] NULL, -- 資料來源單位代碼
        [insertAccountID] [int] NOT NULL, -- 建立者帳號ID
        [insertDateTime] [datetime] NOT NULL CONSTRAINT [DF_Tree_Record_insertDateTime] DEFAULT (GETDATE()), -- 建立時間
        [updateAccountID] [int] NULL, -- 最後更新者帳號ID
        [updateDateTime] [datetime] NULL, -- 最後更新時間
        [removeAccountID] [int] NULL, -- 移除者帳號ID
        [removeDateTime] [datetime] NULL, -- 移除時間
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
        [photoID] [int] IDENTITY(1,1) NOT NULL, -- 照片主鍵編號
        [treeID] [int] NOT NULL, -- 對應樹木基本資料編號
        [fileName] [nvarchar](255) NOT NULL, -- 檔案名稱
        [filePath] [nvarchar](500) NOT NULL, -- 檔案儲存路徑
        [caption] [nvarchar](200) NULL, -- 照片或附件說明文字
        [isCover] [bit] NOT NULL CONSTRAINT [DF_Tree_RecordPhoto_isCover] DEFAULT ((0)), -- 是否為封面照片
        [insertAccountID] [int] NOT NULL, -- 建立者帳號ID
        [insertDateTime] [datetime] NOT NULL CONSTRAINT [DF_Tree_RecordPhoto_insertDateTime] DEFAULT (GETDATE()), -- 建立時間
        [updateAccountID] [int] NULL, -- 最後更新者帳號ID
        [updateDateTime] [datetime] NULL, -- 最後更新時間
        [removeAccountID] [int] NULL, -- 移除者帳號ID
        [removeDateTime] [datetime] NULL, -- 移除時間
 CONSTRAINT [PK_Tree_RecordPhoto] PRIMARY KEY CLUSTERED
(
        [photoID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

/****** Object:  Table [dbo].[Tree_Log]    Script Date: 2025/8/19 下午 07:16:49 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Tree_Log](
        [logID] [int] IDENTITY(1,1) NOT NULL, -- 操作紀錄主鍵
        [functionType] [nvarchar](20) NOT NULL, -- 功能類型 (樹籍、健檢、巡查、養護)
        [dataID] [int] NOT NULL, -- 對應資料主鍵
        [actionType] [nvarchar](50) NOT NULL, -- 操作類型
        [memo] [nvarchar](max) NULL, -- 備註說明
        [ipAddress] [nvarchar](50) NULL, -- 操作IP位址
        [accountID] [int] NULL, -- 帳號主鍵編號
        [account] [nvarchar](50) NULL, -- 登入帳號
        [accountName] [nvarchar](100) NULL, -- 操作人姓名
        [accountUnit] [nvarchar](100) NULL, -- 操作人單位
        [logDateTime] [datetime] NOT NULL CONSTRAINT [DF_Tree_Log_logDateTime] DEFAULT (GETDATE()), -- 紀錄時間
 CONSTRAINT [PK_Tree_Log] PRIMARY KEY CLUSTERED
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
