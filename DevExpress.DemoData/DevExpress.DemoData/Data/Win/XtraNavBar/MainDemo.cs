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
		static List<Module> Create_XtraNavBar_MainDemo_Modules(Demo demo) {
			return new List<Module> {
				new SimpleModule(demo,
					name: "About",
					displayName: @"DevExpress XtraNavBar %MarketingVersion%",
					group: "About",
					type: "DevExpress.XtraNavBar.Demos.About",
					description: @"",
					addedIn: KnownDXVersion.Before142
				),
				new SimpleModule(demo,
					name: "frmNavBarNavigationPane",
					displayName: @"Navigation Pane",
					group: "Views",
					type: "DevExpress.XtraNavBar.Demos.frmNavBarNavigationPane",
					description: @"This demo demonstrates Microsoft Office Navigation Pane style. This style supports all the features introduced in MS Outlook.",
					addedIn: KnownDXVersion.Before142
				),
				new SimpleModule(demo,
					name: "GroupContainer",
					displayName: @"Group Container",
					group: "Views",
					type: "DevExpress.XtraNavBar.Demos.GroupContainer",
					description: @"Setting a group's GroupStyle property to ControlContainer automatically creates a container control within the group's client area. This enables you to fill the group with any Windows Forms controls just by dragging them onto the group and managing their layout in the same manner as on any other container control.",
					addedIn: KnownDXVersion.Before142,
					associatedFiles: new [] {
						@"\WinForms\CS\NavBarMainDemo\GroupContainer\GroupContainer.cs",
						@"\WinForms\VB\NavBarMainDemo\GroupContainer\GroupContainer.vb"
					}
				),
				new SimpleModule(demo,
					name: "GroupStyles",
					displayName: @"Group Styles",
					group: "Views",
					type: "DevExpress.XtraNavBar.Demos.GroupStyles",
					description: @"This example displays all possible representation styles that can be applied to links in navbar control groups.",
					addedIn: KnownDXVersion.Before142,
					associatedFiles: new [] {
						@"\WinForms\CS\NavBarMainDemo\GroupStyles\GroupStyles.cs",
						@"\WinForms\VB\NavBarMainDemo\GroupStyles\GroupStyles.vb"
					}
				),
				new SimpleModule(demo,
					name: "NavBarHints",
					displayName: @"NavBar Hints",
					group: "Views",
					type: "DevExpress.XtraNavBar.Demos.NavBarHints",
					description: @"This example demonstrates how to implement hints into the NavBar control.",
					addedIn: KnownDXVersion.Before142,
					associatedFiles: new [] {
						@"\WinForms\CS\NavBarMainDemo\NavBarHints\NavBarHints.cs",
						@"\WinForms\VB\NavBarMainDemo\NavBarHints\NavBarHints.vb"
					}
				),
				new SimpleModule(demo,
					name: "frmNavBarDragDrop",
					displayName: @"Drag And Drop",
					group: "UI Customization",
					type: "DevExpress.XtraNavBar.Demos.frmNavBarDragDrop",
					description: @"This demo shows you how to implement internal and external Drag and Drop so that end-users can  move item links both to 'Items List' and 'Recycle Bin' and from 'Items List' to the NavBar.",
					addedIn: KnownDXVersion.Before142
				),
				new SimpleModule(demo,
					name: "frmNavBarProperties",
					displayName: @"Properties",
					group: "UI Customization",
					type: "DevExpress.XtraNavBar.Demos.frmNavBarProperties",
					description: @"This demo provides the means that allow you to change the visual and behavior settings of the NavBar control and its elements (items, groups). You can also right-click a link or group to activate an option menu for it.",
					addedIn: KnownDXVersion.Before142
				),
				new SimpleModule(demo,
					name: "AddGroups",
					displayName: @"Add Groups",
					group: "UI Customization",
					type: "DevExpress.XtraNavBar.Demos.AddGroups",
					description: @"This example demonstrates how to add/delete the navbar's groups.",
					addedIn: KnownDXVersion.Before142,
					associatedFiles: new [] {
						@"\WinForms\CS\NavBarMainDemo\AddGroups\AddGroups.cs",
						@"\WinForms\VB\NavBarMainDemo\AddGroups\AddGroups.vb"
					}
				),
				new SimpleModule(demo,
					name: "AddItemLinks",
					displayName: @"Add Item Links",
					group: "UI Customization",
					type: "DevExpress.XtraNavBar.Demos.AddItemLinks",
					description: @"This example demonstrates how to add/delete Items and ItemLinks in navbar control groups.",
					addedIn: KnownDXVersion.Before142,
					associatedFiles: new [] {
						@"\WinForms\CS\NavBarMainDemo\AddItemLinks\AddItemLinks.cs",
						@"\WinForms\VB\NavBarMainDemo\AddItemLinks\AddItemLinks.vb"
					}
				),
				new SimpleModule(demo,
					name: "CustomizableDistances",
					displayName: @"Customizable Distances",
					group: "UI Customization",
					type: "DevExpress.XtraNavBar.Demos.CustomizableDistances",
					description: @"Customizable Distances",
					addedIn: KnownDXVersion.Before142,
					associatedFiles: new [] {
						@"\WinForms\CS\NavBarMainDemo\CustomizableDistances\CustomizableDistances.cs",
						@"\WinForms\VB\NavBarMainDemo\CustomizableDistances\CustomizableDistances.vb"
					}
				),
				new SimpleModule(demo,
					name: "HitInfo",
					displayName: @"Hit Info",
					group: "UI Customization",
					type: "DevExpress.XtraNavBar.Demos.HitInfo",
					description: @"This example demonstrates how to obtain information on a navbar control element based on its coordinates.",
					addedIn: KnownDXVersion.Before142,
					associatedFiles: new [] {
						@"\WinForms\CS\NavBarMainDemo\HitInfo\HitInfo.cs",
						@"\WinForms\VB\NavBarMainDemo\HitInfo\HitInfo.vb"
					}
				),
				new SimpleModule(demo,
					name: "NavBarInfo",
					displayName: @"NavBar Info",
					group: "UI Customization",
					type: "DevExpress.XtraNavBar.Demos.NavBarInfo",
					description: @"This example demonstrates how to obtain information that relates to navbar control elements (Groups, Items, ItemLinks).",
					addedIn: KnownDXVersion.Before142,
					associatedFiles: new [] {
						@"\WinForms\CS\NavBarMainDemo\NavBarInfo\NavBarInfo.cs",
						@"\WinForms\VB\NavBarMainDemo\NavBarInfo\NavBarInfo.vb"
					}
				),
				new SimpleModule(demo,
					name: "FirstLookAccordionControl",
					displayName: @"Photo Studio",
					group: "Accordion Control",
					type: "DevExpress.XtraNavBar.Demos.AccordionControlMultimediaModule",
					description: @"This example demonstrates the use of Accordion Control for creating an advanced photo editing tool. The control contains the Properties and Image expandable groups. Expandable items in the Properties group display User Controls providing image editing capabilities.",
					addedIn: KnownDXVersion.V151,
					associatedFiles: new [] {
						@"\WinForms\CS\NavBarMainDemo\Modules\MultimediaModule.cs",
						@"\WinForms\VB\NavBarMainDemo\Modules\MultimediaModule.vb"
					}  
				),
				new SimpleModule(demo,
					name: "FirstLookAccordionControl",
					displayName: @"Auto Trader",
					group: "Accordion Control",
					type: "DevExpress.XtraNavBar.Demos.AccordionControlBusinessModule",
					description: @"This example incorporates the Accordion Control, which provides product filtering capabilities. Custom controls embedded in Accordion Control's items allow you to filter your search results.",
					addedIn: KnownDXVersion.V151,
					associatedFiles: new [] {
						@"\WinForms\CS\NavBarMainDemo\Modules\BusinessModule.cs",
						@"\WinForms\VB\NavBarMainDemo\Modules\BusinessModule.vb"
					}  
				)
			};
		}
	}
}
