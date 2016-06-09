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
		static List<ReallifeDemo> CreateAspReallifeDemos(Platform platform) {
			return new List<ReallifeDemo> {
				new ReallifeDemo(platform,
					name: "WebmailClient",
					displayName: "WebMail",
					launchPath: @"\ASP.NET\CS\WebmailClient.ur1",
					csSolutionPath: @"\ASP.NET\CS\WebmailClient\WebmailClient.sln",
					vbSolutionPath: @"\ASP.NET\VB\WebmailClient\WebmailClient.sln",
					showInDemoCenter: true,
					demoCenterPosition: 7,
					requirements: new Requirement[] {
						new DeveloperServerRequirement("RWA/WebmailClient", "ASP.NET"),
						new SqlServerRequirement()
					}
				),
				new ReallifeDemo(platform,
					name: "DevAV",
					displayName: "Outlook Inspired App",
					launchPath: @"\ASP.NET\CS\DevAV.ur1",
					csSolutionPath: @"\ASP.NET\CS\DevAV\DevAV.sln",
					vbSolutionPath: @"\ASP.NET\VB\DevAV\DevAV.sln",
					showInDemoCenter: true,
					demoCenterPosition: 3,
					requirements: new Requirement[] {
						new DeveloperServerRequirement("RWA/DevAV", "ASP.NET"),
						new SqlServerRequirement()
					}
				),
				new ReallifeDemo(platform,
					name: "EventRegistration",
					displayName: "Event Registration",
					launchPath: @"\ASP.NET\CS\EventRegistration.ur1",
					csSolutionPath: @"\ASP.NET\CS\EventRegistration\EventRegistration.sln",
					vbSolutionPath: @"\ASP.NET\VB\EventRegistration\EventRegistration.sln",
					showInDemoCenter: false,
					requirements: new Requirement[] {
						new DeveloperServerRequirement("RWA/EventRegistration", "ASP.NET")
					}
				),
				new ReallifeDemo(platform,
					name: "SalesViewer",
					displayName: "Sales Viewer",
					launchPath: @"\ASP.NET\CS\SalesViewer.ur1",
					csSolutionPath: @"\ASP.NET\CS\SalesViewer\SalesViewer.sln",
					vbSolutionPath: @"\ASP.NET\VB\SalesViewer\SalesViewer.sln",
					showInDemoCenter: false,
					requirements: new Requirement[] {
						new DeveloperServerRequirement("RWA/SalesViewer", "ASP.NET")
					}
				),
				new ReallifeDemo(platform,
					name: "HotelBooking.Desktop",
					displayName: "DXHotels for Desktop",
					launchPath: @"\ASP.NET\CS\HotelBooking.Desktop.ur1",
					csSolutionPath: @"\ASP.NET\CS\HotelBooking.Desktop\HotelBooking.Desktop.sln",
					vbSolutionPath: @"\ASP.NET\VB\HotelBooking.Desktop\HotelBooking.Desktop.sln",
					showInDemoCenter: false,
					requirements: new Requirement[] {
						new DeveloperServerRequirement("RWA/HotelBooking.Desktop", "ASP.NET"),
						new SqlServerRequirement()
					}
				),
				new ReallifeDemo(platform,
					name: "HotelBooking.Tablet",
					displayName: "DXHotels for Mobile",
					launchPath: @"\ASP.NET\CS\HotelBooking.Tablet.ur1?StartFolder=TabletViewer",
					csSolutionPath: @"\ASP.NET\CS\HotelBooking.Tablet\HotelBooking.Tablet.sln",
					vbSolutionPath: @"\ASP.NET\VB\HotelBooking.Tablet\HotelBooking.Tablet.sln",
					showInDemoCenter: false,
					requirements: new Requirement[] {
						new DeveloperServerRequirement("RWA/HotelBooking.Tablet", "ASP.NET"),
						new SqlServerRequirement()
					}
				),
				new ReallifeDemo(platform,
					name: "Documents",
					displayName: "Documents",
					launchPath: @"\ASP.NET\CS\Documents.ur1",
					csSolutionPath: @"\ASP.NET\CS\Documents\Documents.sln",
					vbSolutionPath: @"\ASP.NET\VB\Documents\Documents.sln",
					showInDemoCenter: false,
					requirements: new Requirement[] {
						new DeveloperServerRequirement("RWA/Documents", "ASP.NET"),
						new SqlServerRequirement()
					}
				)
			};
		}
	}
}
