USE [Forest_TreeProtect]
GO
/****** Object:  Table [dbo].[System_Menu]    Script Date: 2026/1/16 下午 08:01:49 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[System_Menu](
	[menuID] [int] IDENTITY(1,1) NOT NULL, -- 選單項目主鍵
	[groupName] [nvarchar](50) NOT NULL, -- 選單群組名稱
	[menuName] [nvarchar](50) NOT NULL, -- 選單名稱
	[menuURL] [nvarchar](max) NOT NULL, -- 選單連結網址
	[iconClass] [nvarchar](50) NULL,
	[orderBy_group] [int] NOT NULL, -- 群組排序序號
	[orderBy_menu] [int] NOT NULL, -- 選單排序序號
	[isActive] [bit] NOT NULL, -- 是否啟用
	[isShow] [bit] NOT NULL, -- 是否顯示於前端
	[memo] [nvarchar](max) NULL, -- 備註說明
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
	[auTypeID] [int] NOT NULL, -- 權限類型ID
	[menuID] [int] NOT NULL -- 選單項目主鍵
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[System_Taiwan]    Script Date: 2026/1/16 下午 08:01:49 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[System_Taiwan](
	[twID] [int] IDENTITY(1,1) NOT NULL, -- 行政區資料主鍵
	[cityID] [int] NOT NULL, -- 城市代碼
	[city] [nvarchar](50) NOT NULL, -- 城市名稱
	[area] [nvarchar](50) NOT NULL, -- 區域名稱
	[cityCode] [nvarchar](50) NULL, -- 郵遞區號或市代碼
	[areaCode] [nvarchar](50) NULL, -- 區代碼
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
	[unitID] [int] IDENTITY(1,1) NOT NULL, -- 單位主鍵
	[auTypeID] [int] NULL, -- 權限類型ID
	[unitGroup] [nvarchar](max) NULL, -- 權限類型名稱
	[unitName] [nvarchar](max) NULL, -- 單位名稱
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
	[accountID] [int] IDENTITY(1,1) NOT NULL, -- 帳號主鍵編號
	[accountType] [nvarchar](50) NOT NULL,
	[auTypeID] [int] NOT NULL, -- 權限類型ID
	[unitID] [int] NOT NULL,
	[account] [nvarchar](50) NOT NULL, -- 登入帳號
	[password] [nvarchar](max) NULL, -- 密碼雜湊值
	[name] [nvarchar](50) NOT NULL, -- 使用者姓名
	[email] [nvarchar](max) NULL, -- 電子郵件
	[mobile] [nvarchar](max) NULL, -- 行動電話
	[memo] [nvarchar](max) NULL, -- 備註說明
	[verifyStatus] [bit] NULL,
	[isActive] [bit] NOT NULL, -- 是否啟用
	[lastLoginDateTime] [datetime] NULL,
	[lastUpdatePWDateTime] [datetime] NULL, -- 最後修改密碼時間
	[insertDateTime] [datetime] NOT NULL, -- 建立時間
	[insertAccountID] [int] NOT NULL, -- 建立者帳號ID
	[updateDateTime] [datetime] NULL, -- 最後更新時間
	[updateAccountID] [int] NULL, -- 最後更新者帳號ID
	[removeDateTime] [datetime] NULL, -- 移除時間
	[removeAccountID] [int] NULL, -- 移除者帳號ID
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
	[auTypeID] [int] IDENTITY(1,1) NOT NULL, -- 權限類型ID
	[auTypeName] [nvarchar](50) NOT NULL, -- 權限類型名稱
	[memo] [nvarchar](max) NULL, -- 備註說明
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
	[logID] [int] IDENTITY(1,1) NOT NULL, -- 操作紀錄主鍵
	[accountID] [int] NOT NULL, -- 帳號主鍵編號
	[logDateTime] [datetime] NULL, -- 紀錄時間
	[IP] [nvarchar](max) NOT NULL, -- 操作IP位址
	[logItem] [nvarchar](50) NOT NULL, -- 紀錄項目
	[logType] [nvarchar](50) NOT NULL, -- 紀錄類型
	[memo] [nvarchar](max) NULL, -- 備註說明
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
	[settingID] [int] IDENTITY(1,1) NOT NULL, -- 暫存設定主鍵
	[accountID] [int] NOT NULL, -- 使用者帳號ID
	[treeID] [int] NOT NULL, -- 樹木基本資料編號
	[insertDateTime] [datetime] NOT NULL, -- 建立時間
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
	[photoID] [int] IDENTITY(1,1) NOT NULL, -- 照片主鍵編號
	[careID] [int] NOT NULL, -- 養護紀錄主鍵
	[itemName] [nvarchar](200) NULL, -- 施作項目名稱
	[beforeFileName] [nvarchar](260) NULL, -- 施作前檔案名稱
	[beforeFilePath] [nvarchar](500) NULL, -- 施作前檔案路徑
	[beforeFileSize] [int] NULL, -- 施作前檔案大小
	[afterFileName] [nvarchar](260) NULL, -- 施作後檔案名稱
	[afterFilePath] [nvarchar](500) NULL, -- 施作後檔案路徑
	[afterFileSize] [int] NULL, -- 施作後檔案大小
	[insertAccountID] [int] NOT NULL, -- 建立者帳號ID
	[insertDateTime] [datetime] NOT NULL, -- 建立時間
	[removeDateTime] [datetime] NULL, -- 移除時間
	[removeAccountID] [int] NULL, -- 移除者帳號ID
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
	[careID] [int] IDENTITY(1,1) NOT NULL, -- 養護紀錄主鍵
	[treeID] [int] NOT NULL, -- 對應樹木基本資料編號
	[careDate] [date] NOT NULL, -- 養護日期
	[recorder] [nvarchar](100) NULL, -- 記錄人員
	[reviewer] [nvarchar](100) NULL, -- 覆核人員
	[dataStatus] [tinyint] NOT NULL, -- 資料狀態代碼
	[crownStatus] [tinyint] NULL, -- 樹冠枝葉狀態
	[crownSeasonalDormant] [bit] NULL, -- 季節性休眠落葉
	[crownDeadBranch] [bit] NULL, -- 有枯枝
	[crownDeadBranchPercent] [decimal](5, 2) NULL, -- 現存枝葉量(%)
	[crownPest] [bit] NULL, -- 明顯病蟲害
	[crownForeignObject] [bit] NULL, -- 樹冠接觸電線或異物
	[crownOtherNote] [nvarchar](200) NULL, -- 樹冠其他說明
	[trunkStatus] [tinyint] NULL, -- 主莖幹狀態
	[trunkBarkDamage] [bit] NULL, -- 樹皮破損
	[trunkDecay] [bit] NULL, -- 莖幹損傷
	[trunkTermiteTrail] [bit] NULL, -- 有白蟻蟻道
	[trunkLean] [bit] NULL, -- 主莖傾斜搖晃
	[trunkFungus] [bit] NULL, -- 莖基部真菌子實體
	[trunkGummosis] [bit] NULL, -- 流膠或潰瘍
	[trunkVine] [bit] NULL, -- 纏勒植物
	[trunkOtherNote] [nvarchar](200) NULL, -- 主莖幹其他說明
	[rootStatus] [tinyint] NULL, -- 根部狀態
	[rootDamage] [bit] NULL, -- 根部損傷
	[rootDecay] [bit] NULL, -- 根部腐朽
	[rootExpose] [bit] NULL, -- 盤根或浮根
	[rootRot] [bit] NULL, -- 根部潰爛
	[rootSucker] [bit] NULL, -- 大量萌櫱
	[rootOtherNote] [nvarchar](200) NULL, -- 根部其他說明
	[envStatus] [tinyint] NULL, -- 生育地環境狀態
	[envPitSmall] [bit] NULL, -- 樹穴過小
	[envPaved] [bit] NULL, -- 鋪面封固
	[envDebris] [bit] NULL, -- 石塊或廢棄物推積
	[envSoilCover] [bit] NULL, -- 根領覆土過高
	[envCompaction] [bit] NULL, -- 土壤壓實
	[envWaterlog] [bit] NULL, -- 環境積水
	[envNearFacility] [bit] NULL, -- 緊鄰設施或建物
	[envOtherNote] [nvarchar](200) NULL, -- 生育地環境其他說明
	[adjacentStatus] [tinyint] NULL, -- 鄰接物狀態
	[adjacentBuilding] [bit] NULL, -- 接觸建築物
	[adjacentWire] [bit] NULL, -- 接觸電線或管線
	[adjacentSignal] [bit] NULL, -- 遮蔽路燈或號誌
	[adjacentOtherNote] [nvarchar](200) NULL, -- 鄰接物其他說明
	[task1Status] [tinyint] NULL, -- 危險枯枝清除完成情形
	[task1Note] [nvarchar](500) NULL, -- 危險枯枝清除說明
	[task2Status] [tinyint] NULL, -- 植栽基盤維護完成情形
	[task2Note] [nvarchar](500) NULL, -- 植栽基盤維護說明
	[task3Status] [tinyint] NULL, -- 樹木健康管理完成情形
	[task3Note] [nvarchar](500) NULL, -- 樹木健康管理說明
	[task4Status] [tinyint] NULL, -- 營養評估追肥完成情形
	[task4Note] [nvarchar](500) NULL, -- 營養評估追肥說明
	[task5Status] [tinyint] NULL, -- 安全衛生防護完成情形
	[task5Note] [nvarchar](500) NULL, -- 安全衛生防護說明
	[insertAccountID] [int] NOT NULL, -- 建立者帳號ID
	[insertDateTime] [datetime] NOT NULL, -- 建立時間
	[updateAccountID] [int] NULL, -- 最後更新者帳號ID
	[updateDateTime] [datetime] NULL, -- 最後更新時間
	[removeDateTime] [datetime] NULL, -- 移除時間
	[removeAccountID] [int] NULL, -- 移除者帳號ID
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
	[attachmentID] [int] IDENTITY(1,1) NOT NULL, -- 附件主鍵編號
	[healthID] [int] NOT NULL, -- 健康檢測紀錄主鍵
	[fileName] [nvarchar](260) NOT NULL, -- 檔案名稱
	[filePath] [nvarchar](500) NOT NULL, -- 檔案儲存路徑
	[fileSize] [int] NULL, -- 檔案大小(位元組)
	[description] [nvarchar](200) NULL, -- 附件描述
	[insertAccountID] [int] NOT NULL, -- 建立者帳號ID
	[insertDateTime] [datetime] NOT NULL, -- 建立時間
	[removeDateTime] [datetime] NULL, -- 移除時間
	[removeAccountID] [int] NULL, -- 移除者帳號ID
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
	[settingID] [int] IDENTITY(1,1) NOT NULL, -- 設定流水號
	[accountID] [int] NOT NULL, -- 帳號流水號
	[treeID] [int] NOT NULL, -- 樹籍流水號
	[insertDateTime] [datetime] NOT NULL -- 新增時間
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Tree_HealthPhoto]    Script Date: 2026/1/16 下午 08:01:49 ******/
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
	[insertDateTime] [datetime] NOT NULL, -- 建立時間
	[removeDateTime] [datetime] NULL, -- 移除時間
	[removeAccountID] [int] NULL, -- 移除者帳號ID
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
	[healthID] [int] IDENTITY(1,1) NOT NULL, -- 健康檢測紀錄主鍵
	[treeID] [int] NOT NULL, -- 對應樹木基本資料編號
	[surveyDate] [date] NOT NULL, -- 調查日期
	[surveyor] [nvarchar](100) NULL, -- 調查人員姓名
	[dataStatus] [tinyint] NOT NULL, -- 資料狀態代碼
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
	[crownLeafCoveragePercent] [nvarchar](50) NULL, -- 樹冠葉片覆蓋率(%)
	[crownDeadBranchPercent] [decimal](5, 2) NULL, -- 樹冠枯枝比例(%)
	[crownHangingBranch] [bit] NULL, -- 樹冠掛枝
	[crownOtherNote] [nvarchar](200) NULL, -- 樹冠其他說明
	[growthNote] [nvarchar](200) NULL, -- 生長其他詳加說明
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
	[soilPh] [nvarchar](50) NULL, -- 土壤pH值
	[soilOrganicMatter] [nvarchar](50) NULL, -- 土壤有機質含量
	[soilEc] [nvarchar](50) NULL, -- 土壤電導度
	[managementStatus] [nvarchar](max) NULL, -- 管理狀態說明
	[priority] [nvarchar](50) NULL, -- 處置優先順序
	[treatmentDescription] [nvarchar](max) NULL, -- 處置或維護建議
	[insertAccountID] [int] NOT NULL, -- 建立者帳號ID
	[insertDateTime] [datetime] NOT NULL, -- 建立時間
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
/****** Object:  Table [dbo].[Tree_Log]    Script Date: 2026/1/16 下午 08:01:49 ******/
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
	[logDateTime] [datetime] NOT NULL, -- 紀錄時間
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
	[settingID] [int] IDENTITY(1,1) NOT NULL, -- 暫存設定主鍵
	[accountID] [int] NOT NULL, -- 使用者帳號ID
	[treeID] [int] NOT NULL, -- 樹木基本資料編號
	[insertDateTime] [datetime] NOT NULL, -- 建立時間
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
	[photoID] [int] IDENTITY(1,1) NOT NULL, -- 照片主鍵編號
	[patrolID] [int] NOT NULL, -- 巡查紀錄主鍵
	[fileName] [nvarchar](260) NOT NULL, -- 檔案名稱
	[filePath] [nvarchar](500) NOT NULL, -- 檔案儲存路徑
	[fileSize] [int] NULL, -- 檔案大小(位元組)
	[caption] [nvarchar](200) NULL, -- 照片或附件說明文字
	[insertAccountID] [int] NOT NULL, -- 建立者帳號ID
	[insertDateTime] [datetime] NOT NULL, -- 建立時間
	[removeDateTime] [datetime] NULL, -- 移除時間
	[removeAccountID] [int] NULL, -- 移除者帳號ID
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
	[patrolID] [int] IDENTITY(1,1) NOT NULL, -- 巡查紀錄主鍵
	[treeID] [int] NOT NULL, -- 對應樹木基本資料編號
	[patrolDate] [date] NOT NULL, -- 巡查日期
	[patroller] [nvarchar](100) NULL, -- 巡查人姓名
	[dataStatus] [tinyint] NOT NULL, -- 資料狀態代碼 (0=草稿,1=定稿)
	[memo] [nvarchar](max) NULL, -- 巡查備註
	[hasPublicSafetyRisk] [bit] NOT NULL, -- 是否有危害公共安全風險或緊急狀況
	[sourceUnit] [nvarchar](200) NULL, -- 資料來源單位名稱
	[sourceUnitID] [int] NULL, -- 資料來源單位代碼
	[sourceDataID] [int] NULL, -- 資料來源ID
	[insertAccountID] [int] NOT NULL, -- 建立者帳號ID
	[insertDateTime] [datetime] NOT NULL, -- 建立時間
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
/****** Object:  Table [dbo].[Tree_RecognitionCriterion]    Script Date: 2026/1/16 下午 08:01:49 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Tree_RecognitionCriterion](
	[criterionCode] [nvarchar](50) NOT NULL, -- 認定基準代碼
	[criterionName] [nvarchar](200) NOT NULL, -- 認定基準名稱
	[orderNo] [int] NOT NULL, -- 顯示排序序號
	[isActive] [bit] NOT NULL, -- 是否啟用
	[insertDateTime] [datetime] NOT NULL, -- 建立時間
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
/****** Object:  Table [dbo].[Tree_Record]    Script Date: 2026/1/16 下午 08:01:49 ******/
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
	[isAnnounced] [bit] NOT NULL, -- 是否已公告列管（樹籍狀態為已公告列管才為1）
	[treeStatus] [nvarchar](20) NOT NULL, -- 樹籍狀態（已公告列管／符合標準／其他）
	[editStatus] [int] NOT NULL, -- 編輯狀態（草稿／定稿）
	[treeCount] [int] NOT NULL, -- 數量
	[breastHeightDiameter] [nvarchar](100) NULL, -- 胸高直徑(公分)
	[breastHeightCircumference] [nvarchar](100) NULL, -- 胸高樹圍(公分)
	[canopyProjectionArea] [nvarchar](100) NULL, -- 樹冠投影面積(平方公尺)
	[healthCondition] [nvarchar](max) NULL, -- 樹木健康及生育地概況
	[hasEpiphyte] [bit] NULL,
	[epiphyteDescription] [nvarchar](max) NULL, -- 附生植物概況
	[hasParasite] [bit] NULL,
	[parasiteDescription] [nvarchar](max) NULL, -- 寄生植物概況
	[hasClimbingPlant] [bit] NULL,
	[climbingPlantDescription] [nvarchar](max) NULL, -- 纏勒植物概況
	[surveyOtherNote] [nvarchar](max) NULL, -- 其他備註
	[site] [nvarchar](200) NULL, -- 坐落地點
	[latitude] [nvarchar](100) NULL, -- 座標(WGS84)：緯度(N)
	[longitude] [nvarchar](100) NULL, -- 座標(WGS84)：經度(E)
	[landOwnership] [nvarchar](100) NULL, -- 土地權屬（國有／公有／私有／其他／無資料）
	[landOwnershipNote] [nvarchar](max) NULL, -- 土地權屬備註
	[facilityDescription] [nvarchar](max) NULL, -- 管理設施描述
	[memo] [nvarchar](max) NULL,
	[keywords] [nvarchar](200) NULL,
	[recognitionCriteria] [nvarchar](max) NULL, -- 受保護認定理由（存放[Tree_RecognitionCriterion].[criterionCode]，使用逗號分隔多筆）
	[recognitionNote] [nvarchar](max) NULL, -- 認定理由備註說明
	[culturalHistoryIntro] [nvarchar](max) NULL, -- 文化歷史價值介紹
	[estimatedPlantingYear] [nvarchar](50) NULL, -- 推估種植年間
	[estimatedAgeNote] [nvarchar](max) NULL, -- 推估樹齡備註
	[groupGrowthInfo] [nvarchar](max) NULL, -- 群生竹木或行道樹生長資訊
	[treeHeight] [nvarchar](100) NULL, -- 樹高(公尺)
	[sourceUnit] [nvarchar](100) NULL, -- 資料來源單位名稱
	[sourceUnitID] [int] NULL, -- 資料來源單位代碼
	[insertAccountID] [int] NOT NULL, -- 建立者帳號ID
	[insertDateTime] [datetime] NOT NULL, -- 建立時間
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
	[photoID] [int] IDENTITY(1,1) NOT NULL, -- 照片主鍵編號
	[treeID] [int] NOT NULL, -- 對應樹木基本資料編號
	[fileName] [nvarchar](255) NOT NULL, -- 檔案名稱
	[filePath] [nvarchar](500) NOT NULL, -- 檔案儲存路徑
	[caption] [nvarchar](200) NULL, -- 照片或附件說明文字
	[isCover] [bit] NOT NULL, -- 是否為封面照片
	[insertAccountID] [int] NOT NULL, -- 建立者帳號ID
	[insertDateTime] [datetime] NOT NULL, -- 建立時間
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
/****** Object:  Table [dbo].[Tree_Species]    Script Date: 2026/1/16 下午 08:01:49 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Tree_Species](
	[speciesID] [int] IDENTITY(1,1) NOT NULL, -- 樹種主鍵
	[commonName] [nvarchar](100) NOT NULL, -- 樹種俗名
	[scientificName] [nvarchar](200) NULL, -- 樹種學名
	[aliasName] [nvarchar](200) NULL, -- 樹種別名
	[isNative] [bit] NULL, -- 是否原生種
	[isActive] [bit] NOT NULL, -- 是否啟用
	[orderBy] [int] NULL, -- 排序序號
	[memo] [nvarchar](max) NULL, -- 備註說明
	[insertDateTime] [datetime] NOT NULL, -- 建立時間
	[insertAccountID] [int] NOT NULL, -- 建立者帳號ID
	[updateDateTime] [datetime] NULL, -- 最後更新時間
	[updateAccountID] [int] NULL, -- 最後更新者帳號ID
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
		System_UserAccount.accountID,　-- 帳號流水號
		System_UserAccount.accountType, -- 帳號類型(default=一般使用者 / sso=單一登入使用者)
		System_UserAccount.auTypeID, -- 系統權限別ID
		System_UserAccount.unitID, -- 所屬單位ID
		System_UserAccount.account, -- 帳號
		System_UserAccount.password, -- 密碼(加密後)
		System_UserAccount.name, -- 使用者姓名
		System_UserAccount.email, -- 電子郵件
		System_UserAccount.mobile, -- 行動電話
		System_UserAccount.memo, -- 備註
		System_UserAccount.verifyStatus, --	審核狀態(null=待審,1=通過, 0=駁回)
		System_UserAccount.verifyDateTime, -- 審核時間
		cast(
			case 
				when System_UserAccount.isActive = 1
				 and GETDATE() >= DATEADD(DAY,181,CAST(System_UserAccount.lastLoginDateTime as date))
				then 1
				else 0
			end 
		as bit) as isActive,　-- 是否啟用(1=啟用,0=停用)
		System_UserAccount.lastLoginDateTime, -- 最後登入時間
		System_UserAccount.lastUpdatePWDateTime, - - 最後更新密碼時間
		insertDateTime, insertAccountID, updateDateTime, updateAccountID, removeDateTime, removeAccountID,
		APP_USER_NODE_UUID, -- SSO使用者節點UUID
		APP_COMPANY_UUID, -- SSO公司UUID
		APP_COMPANY_UUID_n, -- SSO公司UUID(無連字號)
		APP_DEPT_NODE_UUID, -- SSO部門節點UUID
		APP_DEPT_NODE_UUID_n, -- SSO部門節點UUID(無連字號)
		APP_USER_LOGIN_ID, -- SSO使用者登入ID
		auTypeName,  -- 系統權限別名稱
		unitGroup, -- 所屬單位群組
		unitName, -- 所屬單位名稱
		cast(case when lastLoginDateTime is NULL then 1 else 0 end as bit) 'isFirstLogin', -- 是否首次登入
		cast(case when GETDATE() >= DATEADD(DAY,181,CAST(lastLoginDateTime as date)) then 1 else 0 end as bit) 'isAutoStopLogin', -- 是否自動停用登入(超過180天未登入)
		cast(case when lastUpdatePWDateTime is null or GETDATE() >= DATEADD(DAY,181,CAST(lastUpdatePWDateTime as date)) then 1 else 0 end as bit) 'isNeedChangePW' -- 是否需要更改密碼(超過180天未更改密碼)
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
