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
		static List<Product> CreateDashboardsProducts(Platform p) {
			return new List<Product> {
				new Product(p,
					CreateDashboardForWinDemos,
					name: "DashboardForWin",
					displayName: "Sample Dashboards\r\nDesktop",
					componentName: "Dashboard",
					group: "Reporting and Analytics Controls",
					shortDescription: "Sample dashboards demonstrated in a Windows Forms application.",
					description: @"
                        <Paragraph>
                        Provides outstanding capabilities for designing and presenting dashboards, enabling you to easily create professional-looking interactive dashboards without writing a single line of code.
                        </Paragraph>
                        <Paragraph>
                        DevExpress Dashboard can be purchased as part of the Universal Subscription.
                        </Paragraph>",
					licenseInfo: 0x0000800000000000,
					isAvailableOnline: false
				),
				new Product(p,
					CreateDashboardForAspDemos,
					name: "DashboardForAsp",
					displayName: "Sample Dashboards\r\nWeb",
					componentName: "Dashboard",
					group: "Data Entry & Analysis",
					shortDescription: "Sample dashboards showcased on the Web.",
					description: @"
                        <Paragraph>
                        With DevExpress Dashboard, you can easily create professional dashboards in the Windows Forms Dashboard Designer and then publish them on the Web via the ASP.NET Viewer. And you can always be sure that your dashboards look the same and provide equal end-user capabilities in both platforms.
                        </Paragraph>
                        <Paragraph>
                        DevExpress Dashboard can be purchased as part of the Universal Subscription.
                        </Paragraph>",
					licenseInfo: 0x0000800000000000,
					isAvailableOnline: true
				),
				new Product(p,
					CreateDashboardForMvcDemos,
					name: "DashboardForMvc",
					displayName: "Sample Dashboards\r\nMvc",
					componentName: "Dashboard",
					group: "Data Entry & Analysis",
					shortDescription: "Sample dashboards showcased on the Web.",
					description: @"
                        <Paragraph>
                        With DevExpress Dashboard, you can easily create professional dashboards in the Windows Forms Dashboard Designer and then publish them on the Web via the ASP.NET MVC Viewer. And you can always be sure that your dashboards look the same and provide equal end-user capabilities in both platforms.
                        </Paragraph>
                        <Paragraph>
                        DevExpress Dashboard can be purchased as part of the Universal Subscription.
                        </Paragraph>",
					licenseInfo: 0x0000800000000000,
					isAvailableOnline: true
				)
			};
		}
	}
}
