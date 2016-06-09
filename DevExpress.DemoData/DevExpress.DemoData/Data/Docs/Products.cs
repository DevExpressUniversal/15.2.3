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
		static List<Product> CreateDocsProducts(Platform p) {
			return new List<Product> {
				new Product(p,
					CreateDocumentServerForWinDemos,
					name: "DocumentServerForWin",
					displayName: "WinForms",
					componentName: "Docs",
					group: "Reporting and Analytics Controls",
					shortDescription: "Non-visual library for processing Word and Excel documents and Zip archives showcased within WinForms.",
					description: @"
                        <Paragraph>
                        The non-visual library contains engines for processing Word, Excel and Snap documents via a straightforward API. You can also take advantage of the bar code image library, unit conversion utilities and the data compression library.
                        </Paragraph>",
					licenseInfo: 0x0000000000000800,
					isAvailableOnline: false
				),
				new Product(p,
					CreateDocumentServerForAspDemos,
					name: "DocumentServerForAsp",
					displayName: "ASP.NET",
					componentName: "Docs",
					group: "Reporting and Analytics Controls",
					shortDescription: "Non-visual library for processing Word and Excel documents and Zip archives showcased within ASP.NET.",
					description: @"
                        <Paragraph>
                        The non-visual library contains engines for processing Word, Excel and Snap documents via a straightforward API. You can also take advantage of the bar code image library, unit conversion utilities and the data compression library.
                        </Paragraph>",
					licenseInfo: 0x0000000000000800,
					isAvailableOnline: false
				),
				new Product(p,
					CreateDocumentServerForAspMVCDemos,
					name: "DocumentServerForAspMVC",
					displayName: "ASP.NET MVC",
					componentName: "Docs",
					group: "Reporting and Analytics Controls",
					shortDescription: "Non-visual library for processing Word and Excel documents and Zip archives showcased within ASP.NET MVC.",
					description: @"
                        <Paragraph>
                        The non-visual library contains engines for processing Word, Excel and Snap documents via a straightforward API. You can also take advantage of the bar code image library, unit conversion utilities and the data compression library.
                        </Paragraph>",
					licenseInfo: 0x0000000000000800,
					isAvailableOnline: false
				)
			};
		}
	}
}
