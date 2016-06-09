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
		static List<Product> CreateFrameworksProducts(Platform p) {
			return new List<Product> {
				new Product(p,
					CreateXafDemos,
					name: "Xaf",
					displayName: ".NET Application Framework",
					componentName: "eXpressAppFramework",
					group: "Navigation & Layout",
					shortDescription: "XAF allows you to quickly and efficiently create multi-platform and feature rich business applications.",
					description: @"
                        <Paragraph>
                        Concentrate on business problems at hand without being distracted by the general activities of application development.
                        </Paragraph>
                        <Paragraph>
                        XAF is available in the Universal subscription which includes all DevExpress .NET Technologies - ORM Tool, Reporting Platform and Presentation Controls for WinForms, ASP.NET, WPF and Silverlight.
                        </Paragraph>",
					licenseInfo: 0x0000000010000000,
					isAvailableOnline: true
				),
				new Product(p,
					CreateXpoDemos,
					name: "Xpo",
					displayName: ".NET ORM Tool",
					componentName: "Windows Forms",
					group: "Office Inspired Control Suites",
					shortDescription: "Powerful Object-Relational Mapping Tool for .NET.",
					description: @"
                        <Paragraph>
                        Handle your data on a higher abstraction level - by writing code without having to deal with the database layer. Simply choose the required DBMS from 20 supported types.
                        </Paragraph>
                        <Paragraph>
                        XPO can be purchased as part of the Universal, DXperience, WinForms, ASP.NET, WPF or Silverlight Subscription.
                        </Paragraph>",
					licenseInfo: 0x0000000000008000,
					isAvailableOnline: true
				)
			};
		}
	}
}
