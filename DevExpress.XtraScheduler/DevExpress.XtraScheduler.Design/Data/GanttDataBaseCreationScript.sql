CREATE TABLE [dbo].[TaskDependencies](
    [Id] [int] IDENTITY(1,1) NOT NULL,
    [ParentId] [int] NULL,
    [DependentId] [int] NULL,
    [Type] [int] NOT NULL,
 CONSTRAINT [PK_TaskDependencies] PRIMARY KEY CLUSTERED 
(
    [Id] ASC
)
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[Resources](
    [Id] [int] IDENTITY(1,1) NOT NULL,
    [IdSort] [int] NULL,
    [ParentId] [int] NULL,
    [Description] [nvarchar](max) NULL,
    [Color] [int] NULL,
    [Image] [image] NULL,
    [CustomField1] [nvarchar](max) NULL,
 CONSTRAINT [PK_Resources] PRIMARY KEY CLUSTERED 
(
    [Id] ASC
)
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

CREATE TABLE [dbo].[Appointments](
    [UniqueId] [int] IDENTITY(1,1) NOT NULL,
    [Type] [int] NULL,
    [StartDate] [smalldatetime] NULL,
    [EndDate] [smalldatetime] NULL,
    [AllDay] [bit] NULL,
    [Subject] [nvarchar](max) NULL,
    [Location] [nvarchar](max) NULL,
    [Description] [nvarchar](max) NULL,
    [Status] [int] NULL,
    [Label] [int] NULL,
    [ResourceId] [int] NULL,
    [ResourceIds] [nvarchar](max) NULL,
    [ReminderInfo] [nvarchar](max) NULL,
    [RecurrenceInfo] [nvarchar](max) NULL,
    [PercentComplete] [int] NULL,
    [TimeZoneId] [nvarchar](max) NULL,
    [CustomField1] [nvarchar](max) NULL,
 CONSTRAINT [PK_Appointments] PRIMARY KEY CLUSTERED 
(
    [UniqueId] ASC
)
) ON [PRIMARY]
GO