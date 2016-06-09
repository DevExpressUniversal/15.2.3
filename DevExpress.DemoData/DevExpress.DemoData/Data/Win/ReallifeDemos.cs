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
		static List<ReallifeDemo> CreateWinReallifeDemos(Platform platform) {
			return new List<ReallifeDemo> {
				new ReallifeDemo(platform,
					name: "HybridDemo",
					displayName: "Touch-Enabled Hybrid App",
					launchPath: @"\WinForms\DevExpress.HybridApp.Win\Bin\DevExpress.HybridApp.Win.exe",
					csSolutionPath: @"\WinForms\DevExpress.HybridApp.Win\CS\DevExpress.HybridApp.Win.sln",
					vbSolutionPath: null,
					showInDemoCenter: true,
					demoCenterPosition: 0,
					requirements: new Requirement[] {
						new SqlServerRequirement()
					}
				),
				new ReallifeDemo(platform,
					name: "OutlookInspiredDemo",
					displayName: "Outlook Inspired App",
					launchPath: @"\WinForms\DevExpress.OutlookInspiredApp.Win\Bin\DevExpress.OutlookInspiredApp.Win.exe",
					csSolutionPath: @"\WinForms\DevExpress.OutlookInspiredApp.Win\CS\DevExpress.OutlookInspiredApp.Win.sln",
					vbSolutionPath: @"\WinForms\DevExpress.OutlookInspiredApp.Win\VB\DevExpress.OutlookInspiredApp.Win.sln",
					showInDemoCenter: true,
					demoCenterPosition: 1,
					requirements: new Requirement[] {
						new SqlServerRequirement()
					}
				),
				new ReallifeDemo(platform,
					name: "MailClientDemo",
					displayName: "Windows Mail Client",
					launchPath: @"\WinForms\DevExpress.MailClient.Win\Bin\DevExpress.MailClient.Win.exe",
					csSolutionPath: @"\WinForms\DevExpress.MailClient.Win\CS\DevExpress.MailClient.Win.sln",
					vbSolutionPath: @"\WinForms\DevExpress.MailClient.Win\VB\DevExpress.MailClient.Win.sln",
					showInDemoCenter: true,
					demoCenterPosition: 8
				),
				new ReallifeDemo(platform,
					name: "StockMarketTraderDemo",
					displayName: "Stock Market Trader",
					launchPath: @"\WinForms\DevExpress.StockMarketTrader.Win\Bin\DevExpress.StockMarketTrader.exe",
					csSolutionPath: @"\WinForms\DevExpress.StockMarketTrader.Win\CS\DevExpress.StockMarketTrader.Win.sln",
					vbSolutionPath: null,
					showInDemoCenter: false
				),
				new ReallifeDemo(platform,
					name: "VideoRentalDemo",
					displayName: "Office Inspired Business App",
					launchPath: @"\WinForms\DevExpress.VideoRent.Win\Bin\DevExpress.VideoRent.Win.exe",
					csSolutionPath: @"\WinForms\DevExpress.VideoRent.Win\CS\DevExpress.VideoRent.Win.sln",
					vbSolutionPath: @"\WinForms\DevExpress.VideoRent.Win\VB\DevExpress.VideoRent.Win.sln",
					showInDemoCenter: false
				),
				new ReallifeDemo(platform,
					name: "SalesDemo",
					displayName: "Sales Dashboard",
					launchPath: @"\WinForms\DevExpress.SalesDemo.Win\Bin\DevExpress.SalesDemo.Win.exe",
					csSolutionPath: @"\WinForms\DevExpress.SalesDemo.Win\CS\DevExpress.SalesDemo.Win.sln",
					vbSolutionPath: null,
					showInDemoCenter: true,
					demoCenterPosition: 9
				),
				new ReallifeDemo(platform,
					name: "ProductsDemo",
					displayName: "Build Your Own Office",
					launchPath: @"\WinForms\DevExpress.ProductsDemo.Win\Bin\DevExpress.ProductsDemo.Win.exe",
					csSolutionPath: @"\WinForms\DevExpress.ProductsDemo.Win\CS\DevExpress.ProductsDemo.Win.sln",
					vbSolutionPath: @"\WinForms\DevExpress.ProductsDemo.Win\VB\DevExpress.ProductsDemo.Win.sln",
					showInDemoCenter: true,
					demoCenterPosition: 2,
					group: "Office Inspired Control Suites",
					color: "#FFEB3C00"
				)
			};
		}
	}
}
