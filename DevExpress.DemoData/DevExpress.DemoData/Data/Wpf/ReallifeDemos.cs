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
		static List<ReallifeDemo> CreateWpfReallifeDemos(Platform platform) {
			return new List<ReallifeDemo> {
				new ReallifeDemo(platform,
					name: "OutlookInspiredDemo",
					displayName: "Outlook Inspired App",
					launchPath: @"\WPF\DevExpress.OutlookInspiredApp.Wpf\Bin\DevExpress.OutlookInspiredApp.Wpf.exe",
					csSolutionPath: @"\WPF\DevExpress.OutlookInspiredApp.Wpf\CS\DevExpress.OutlookInspiredApp.Wpf.sln",
					vbSolutionPath: @"\WPF\DevExpress.OutlookInspiredApp.Wpf\VB\DevExpress.OutlookInspiredApp.Wpf.sln",
					showInDemoCenter: true,
					demoCenterPosition: 6
				),
				new ReallifeDemo(platform,
					name: "HybridApp",
					displayName: "Touch-Enabled Hybrid App",
					launchPath: @"\WPF\DevExpress.HybridApp.Wpf\Bin\DevExpress.HybridApp.Wpf.exe",
					csSolutionPath: @"\WPF\DevExpress.HybridApp.Wpf\CS\DevExpress.HybridApp.Wpf.sln",
					vbSolutionPath: @"\WPF\DevExpress.HybridApp.Wpf\VB\DevExpress.HybridApp.Wpf.sln",
					showInDemoCenter: false
				),
				new ReallifeDemo(platform,
					name: "VideoRentalDemo",
					displayName: "Video Rental Demo",
					launchPath: @"\WPF\DevExpress.VideoRent.Wpf\Bin\DevExpress.VideoRent.Wpf.exe",
					csSolutionPath: @"\WPF\DevExpress.VideoRent.Wpf\CS\DevExpress.VideoRent.Wpf.sln",
					vbSolutionPath: null,
					isAvailableInClickonce: false,
					showInDemoCenter: false
				),
				new ReallifeDemo(platform,
					name: "RealtorWorldDemo",
					displayName: "Realtor World",
					launchPath: @"\WPF\DevExpress.RealtorWorld.Wpf\Bin\DevExpress.RealtorWorld.Xpf.exe",
					csSolutionPath: @"\WPF\DevExpress.RealtorWorld.Wpf\CS\DevExpress.RealtorWorld.Wpf.sln",
					vbSolutionPath: null,
					showInDemoCenter: false
				),
				new ReallifeDemo(platform,
					name: "StockMarketTraderDemo",
					displayName: "Stock Market",
					launchPath: @"\WPF\DevExpress.StockMarketTrader\Bin\DevExpress.StockMarketTrader.exe",
					csSolutionPath: @"\WPF\DevExpress.StockMarketTrader\CS\DevExpress.StockMarketTrader.sln",
					vbSolutionPath: null,
					showInDemoCenter: true,
					demoCenterPosition: 10
				),
				new ReallifeDemo(platform,
					name: "MailClientDemo",
					displayName: "Mail Client",
					launchPath: @"\WPF\DevExpress.MailClient.Wpf\Bin\DevExpress.MailClient.Xpf.exe",
					csSolutionPath: @"\WPF\DevExpress.MailClient.Wpf\CS\DevExpress.MailClient.Wpf.sln",
					vbSolutionPath: null,
					showInDemoCenter: false
				),
				new ReallifeDemo(platform,
					name: "SalesDemo",
					displayName: "Sales Dashboard",
					launchPath: @"\WPF\DevExpress.SalesDemo.Wpf\Bin\DevExpress.SalesDemo.Wpf.exe",
					csSolutionPath: @"\WPF\DevExpress.SalesDemo.Wpf\CS\DevExpress.SalesDemo.Wpf.sln",
					vbSolutionPath: @"\WPF\DevExpress.SalesDemo.Wpf\VB\DevExpress.SalesDemo.Wpf.sln",
					showInDemoCenter: true,
					demoCenterPosition: 4
				),
				new ReallifeDemo(platform,
					name: "ProductsDemo",
					displayName: "Build Your Own Office",
					launchPath: @"\WPF\DevExpress.ProductsDemo.Wpf\Bin\DevExpress.ProductsDemo.Wpf.exe",
					csSolutionPath: @"\WPF\DevExpress.ProductsDemo.Wpf\CS\DevExpress.ProductsDemo.Wpf.sln",
					vbSolutionPath: @"\WPF\DevExpress.ProductsDemo.Wpf\VB\DevExpress.ProductsDemo.Wpf.sln",
					showInDemoCenter: true,
					demoCenterPosition: 5,
					group: "Office Inspired Control Suites",
					color: "#FFEB3C00"
				)
			};
		}
	}
}
