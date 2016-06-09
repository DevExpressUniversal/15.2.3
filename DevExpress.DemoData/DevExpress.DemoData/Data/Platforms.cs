#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{                                                                   }
{                                                                   }
{       Copyright (c) 2000-2015 Developer Express Inc.              }
{       ALL RIGHTS RESERVED                                         }
{                                                                   }
{   The entire contents of this file is protected by U.S. and       }
{   International Copyright Laws. Unauthorized reproduction,        }
{   reverse-engineering, and distribution of all or any portion of  }
{   the code contained in this file is strictly prohibited and may  }
{   result in severe civil and criminal penalties and will be       }
{   prosecuted to the maximum extent possible under the law.        }
{                                                                   }
{   RESTRICTIONS                                                    }
{                                                                   }
{   THIS SOURCE CODE AND ALL RESULTING INTERMEDIATE FILES           }
{   ARE CONFIDENTIAL AND PROPRIETARY TRADE                          }
{   SECRETS OF DEVELOPER EXPRESS INC. THE REGISTERED DEVELOPER IS   }
{   LICENSED TO DISTRIBUTE THE PRODUCT AND ALL ACCOMPANYING .NET    }
{   CONTROLS AS PART OF AN EXECUTABLE PROGRAM ONLY.                 }
{                                                                   }
{   THE SOURCE CODE CONTAINED WITHIN THIS FILE AND ALL RELATED      }
{   FILES OR ANY PORTION OF ITS CONTENTS SHALL AT NO TIME BE        }
{   COPIED, TRANSFERRED, SOLD, DISTRIBUTED, OR OTHERWISE MADE       }
{   AVAILABLE TO OTHER INDIVIDUALS WITHOUT EXPRESS WRITTEN CONSENT  }
{   AND PERMISSION FROM DEVELOPER EXPRESS INC.                      }
{                                                                   }
{   CONSULT THE END USER LICENSE AGREEMENT FOR INFORMATION ON       }
{   ADDITIONAL RESTRICTIONS.                                        }
{                                                                   }
{*******************************************************************}
*/
#endregion Copyright (c) 2000-2015 Developer Express Inc.

using System;
using System.Collections.Generic;
namespace DevExpress.DemoData.Model {
	public static partial class Repository {
		public static KnownDXVersion CurrentDXVersion = (KnownDXVersion)Enum.Parse(typeof(KnownDXVersion), "V" + AssemblyInfo.VersionId);
		public static Platform WinPlatform = new Platform(
			createProducts: CreateWinProducts,
			createReallifeDemos: CreateWinReallifeDemos,
			name: "Win",
			displayName: "WinForms",
			productListTitle: "WinForms Demos",
			productListSubtitle: "Over 120 Controls and Libraries",
			getStartedLink: "https://go.devexpress.com/Demo_GetStarted_WinForms.aspx",
			demoLauncherPath: @"Components\Bin\DevExpress.DemoChooser.v%Version%.exe",
			demoLauncherArgument: "Win");
		public static Platform AspPlatform = new Platform(
			createProducts: CreateAspProducts,
			createReallifeDemos: CreateAspReallifeDemos,
			name: "Asp",
			displayName: "ASP.NET",
			productListTitle: "ASP.NET Demos",
			productListSubtitle: "Over 90 Controls and Libraries",
			getStartedLink: "https://go.devexpress.com/Demo_GetStarted_ASP.aspx",
			demoLauncherPath: @"Components\Bin\DevExpress.DemoChooser.v%Version%.exe",
			demoLauncherArgument: "Asp");
		public static Platform MvcPlatform = new Platform(
			createProducts: CreateMvcProducts,
			createReallifeDemos: CreateMvcReallifeDemos,
			name: "Mvc",
			displayName: "ASP.NET MVC",
			productListTitle: "ASP.NET MVC Demos",
			productListSubtitle: "Over 50 Extensions",
			getStartedLink: "https://go.devexpress.com/Demo_GetStarted_ASP.aspx",
			demoLauncherPath: @"Components\Bin\DevExpress.DemoChooser.v%Version%.exe",
			demoLauncherArgument: "Mvc");
		public static Platform WpfPlatform = new Platform(
			createProducts: CreateWpfProducts,
			createReallifeDemos: CreateWpfReallifeDemos,
			name: "Wpf",
			displayName: "WPF",
			productListTitle: "WPF Demos",
			productListSubtitle: "Over 90 Controls and Libraries",
			getStartedLink: "https://go.devexpress.com/Demo_GetStarted_WPF.aspx",
			demoLauncherPath: @"\WPF\Bin\DemoLauncher.exe");
		public static Platform WinRTPlatform = new Platform(
			createProducts: CreateWinRTProducts,
			createReallifeDemos: p => new List<ReallifeDemo>(),
			name: "WinRT",
			displayName: "Windows 8 XAML",
			productListTitle: "Windows 8 XAML Demos",
			productListSubtitle: "",
			getStartedLink: "https://go.devexpress.com/Demo_GetStarted_WinRT.aspx",
			demoLauncherPath: @"Components\Bin\DemoLauncher.v%Version%.exe");
		public static Platform ReportingPlatform = new Platform(
			createProducts: CreateReportingProducts,
			createReallifeDemos: p => new List<ReallifeDemo>(),
			name: "Reporting",
			displayName: "Reporting",
			productListTitle: "Reporting Demos",
			productListSubtitle: "",
			getStartedLink: "https://go.devexpress.com/Demo_GetStarted_Reporting.aspx",
			demoLauncherPath: @"Components\Bin\DevExpress.DemoChooser.v%Version%.exe",
			demoLauncherArgument: "Reporting");
		public static Platform DashboardsPlatform = new Platform(
			createProducts: CreateDashboardsProducts,
			createReallifeDemos: CreateDashboardsReallifeDemos,
			name: "Dashboards",
			displayName: "Dashboard",
			productListTitle: "Dashboard Demos",
			productListSubtitle: "",
			getStartedLink: "https://go.devexpress.com/Demo_GetStarted_Dashboard.aspx",
			demoLauncherPath: @"Components\Bin\DevExpress.DemoChooser.v%Version%.exe",
			demoLauncherArgument: "Dashboards");
		public static Platform DocsPlatform = new Platform(
			createProducts: CreateDocsProducts,
			createReallifeDemos: p => new List<ReallifeDemo>(),
			name: "Docs",
			displayName: "Document Server",
			productListTitle: "Document Server Demos",
			productListSubtitle: "",
			getStartedLink: "https://go.devexpress.com/Demo_GetStarted_DocumentServer.aspx",
			demoLauncherPath: @"Components\Bin\DevExpress.DemoChooser.v%Version%.exe",
			demoLauncherArgument: "Docs");
		public static Platform FrameworksPlatform = new Platform(
			createProducts: CreateFrameworksProducts,
			createReallifeDemos: p => new List<ReallifeDemo>(),
			name: "Frameworks",
			displayName: "XAF",
			productListTitle: "eXpressApp Framework Demos",
			productListSubtitle: "",
			getStartedLink: "https://go.devexpress.com/Demo_GetStarted_XAF.aspx",
			demoLauncherPath: @"Components\Bin\DevExpress.DemoChooser.v%Version%.exe",
			demoLauncherArgument: "Frameworks");
		public static Platform DevExtremePlatform = new Platform(
			createProducts: CreateDevExtremeProducts,
			createReallifeDemos: p => new List<ReallifeDemo>(),
			name: "DevExtreme",
			displayName: "DevExtreme",
			productListTitle: "DevExtreme Demos",
			productListSubtitle: "",
			getStartedLink: "",
			demoLauncherPath: @"..\DevExtreme\Bin\DemoLauncher.v%Version%.exe");
		public static List<Platform> Platforms = new List<Platform> {
			WinPlatform,
			AspPlatform,
			MvcPlatform,
			WpfPlatform,
			WinRTPlatform,
			ReportingPlatform,
			DashboardsPlatform,
			DocsPlatform,
			FrameworksPlatform,
			DevExtremePlatform
		};
	}
}
