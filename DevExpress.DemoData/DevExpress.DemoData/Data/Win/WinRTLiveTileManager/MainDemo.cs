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
		static List<Module> Create_WinRTLiveTileManager_MainDemo_Modules(Demo demo) {
			return new List<Module> {
				new SimpleModule(demo,
					name: "About",
					displayName: @"WinRTLiveTileManager Features",
					group: "About",
					type: "DevExpress.WinRTLiveTileManager.Demos.About",
					description: @"",
					addedIn: KnownDXVersion.Before142
				),
				new SimpleModule(demo,
					name: "AboutLiveTiles",
					displayName: @"About Live Tiles",
					group: "Info",
					type: "DevExpress.WinRTLiveTileManager.Demos.AboutLiveTiles",
					description: @"",
					addedIn: KnownDXVersion.Before142,
					associatedFiles: new [] {
						@"\WinForms\CS\WinRTLiveTileManagerMainDemo\Modules\AddAndDeleteTiles.cs",
						@"\WinForms\VB\WinRTLiveTileManagerMainDemo\Modules\AddAndDeleteTiles.vb"
					}
				),
				new SimpleModule(demo,
					name: "KPI",
					displayName: @"Network Monitor",
					group: "Real life demos",
					type: "DevExpress.WinRTLiveTileManager.Demos.KPI",
					description: @"The LiveTileManager can create a Live Tile within the Start Screen to display a preview of your desktop application. This demo emulates network traffic and visualizes it using a Chart control. A Live Tile named ""Network Monitor"" is created in the Start Screen to display a graphical preview of data and additional traffic attributes.",
					addedIn: KnownDXVersion.Before142,
					associatedFiles: new [] {
						@"\WinForms\CS\WinRTLiveTileManagerMainDemo\Modules\ChartDemoRealtimeChart.cs",
						@"\WinForms\VB\WinRTLiveTileManagerMainDemo\Modules\ChartDemoRealtimeChart.vb"
					}
				),
				new SimpleModule(demo,
					name: "OutlookStyle",
					displayName: @"Outlook Style",
					group: "Real life demos",
					type: "DevExpress.WinRTLiveTileManager.Demos.OutlookStyle",
					description: @"This demo emulates an email client that periodically checks for mail. When new mail is received, alert windows are shown at the bottom right corner of the screen. Additionally, the same notification is displayed within a Live Tile in the Windows Start Screen.",
					addedIn: KnownDXVersion.Before142,
					associatedFiles: new [] {
						@"\WinForms\CS\WinRTLiveTileManagerMainDemo\Modules\OutlookStyle.cs",
						@"\WinForms\VB\WinRTLiveTileManagerMainDemo\Modules\OutlookStyle.vb"
					}
				)
			};
		}
	}
}
