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
		static List<Demo> CreateXtraBarsDemos(Product product) {
			return new List<Demo> {
				new SimpleDemo(product,
					p => new List<Module>(),
					name: "PhotoViewer",
					displayName: @"Photo Viewer",	
					csSolutionPath: @"\WinForms\CS\PhotoViewer\PhotoViewer.sln",
					vbSolutionPath: @"\WinForms\VB\PhotoViewer\PhotoViewer.sln",
					launchPath: @"\WinForms\Bin\PhotoViewer.exe"
				),
				new SimpleDemo(product,
					p => new List<Module>(),
					name: "RibbonSimplePad",
					displayName: @"Ribbon Simple Pad",	
					csSolutionPath: @"\WinForms\CS\RibbonSimplePad\RibbonSimplePad.sln",
					vbSolutionPath: @"\WinForms\VB\RibbonSimplePad\RibbonSimplePad.sln",
					launchPath: @"\WinForms\Bin\RibbonSimplePad.exe"
				),
				new SimpleDemo(product,
					p => new List<Module>(),
					name: "SimplePad",
					displayName: @"Simple Pad",	
					csSolutionPath: @"\WinForms\CS\SimplePad\SimplePad.sln",
					vbSolutionPath: @"\WinForms\VB\SimplePad\SimplePad.sln",
					launchPath: @"\WinForms\Bin\SimplePad.exe"
				),
				new SimpleDemo(product,
					p => new List<Module>(),
					name: "BrowserDemo",
					displayName: @"Browser",	
					csSolutionPath: @"\WinForms\CS\BrowserDemo\BrowserDemo.sln",
					vbSolutionPath: @"\WinForms\VB\BrowserDemo\BrowserDemo.sln",
					launchPath: @"\WinForms\Bin\BrowserDemo.exe"
				)
			};
		}
	}
}
