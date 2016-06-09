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
		static List<Module> Create_DXRibbon_MainDemo_Modules(Demo demo) {
			return new List<Module> {
				new WpfModule(demo,
					name: "RibbonWindow",
					displayName: @"Ribbon Window",
					group: "Demos",
					type: "RibbonDemo.RibbonWindow",
					shortDescription: @"This example demonstrates the DXRibbonWindow which provides built-in support for the Ribbon Control display.",
					description: @"
                        <Paragraph TextAlignment=""Justify"">
                        Click the ""Launch DXRibbonWindow"" button to open the demo in a separate window. The example demonstrates a DXRibbon control integrated into a DXRibbonWindow. Note that the Ribbon Control's Application Button and Quick Access Toolbar are part of the DXRibbonWindow.
                        </Paragraph>",
					addedIn: KnownDXVersion.Before142,
					updatedIn: KnownDXVersion.V142,
					isFeatured: true
				),
				new WpfModule(demo,
					name: "RibbonOffice2007",
					displayName: @"Office 2007 Style",
					group: "Demos",
					type: "RibbonDemo.RibbonOffice2007",
					shortDescription: @"This application demonstrates the ""Office 2007"" Ribbon style.",
					description: @"
                        <Paragraph TextAlignment=""Justify"">
                        This example demonstrates use of the DevExpress WPF Ribbon Control to implement text editing commands. You can select text within the editing region to see the Ribbon's contextual tabs in action. To switch between different Ribbon styles, use the Ribbon Style combo box.
                        </Paragraph>",
					addedIn: KnownDXVersion.Before142,
					isFeatured: true
				),
				new WpfModule(demo,
					name: "RibbonOffice2010",
					displayName: @"Office 2010 Style",
					group: "Demos",
					type: "RibbonDemo.RibbonOffice2010",
					shortDescription: @"This application demonstrates the ""Office 2010"" Ribbon style.",
					description: @"
                        <Paragraph TextAlignment=""Justify"">
                        This example demonstrates use of the DevExpress WPF Ribbon Control to implement text editing commands. You can select text within the editing region to see the Ribbon's contextual tabs in action. To switch between different Ribbon styles, use the Ribbon Style combo box.
                        </Paragraph>",
					addedIn: KnownDXVersion.Before142,
					isFeatured: true
				),
				new WpfModule(demo,
					name: "RibbonTabletOffice",
					displayName: @"Tablet Office Style",
					group: "Demos",
					type: "RibbonDemo.RibbonTabletOffice",
					shortDescription: @"This application demonstrates the ""Office for iOS"" Ribbon style.",
					description: @"
                        <Paragraph TextAlignment=""Justify"">
                        This example demonstrates use of the DevExpress WPF Ribbon Control to implement text editing commands. You can select text within the editing region to see the Ribbon's contextual tabs in action. To switch between different Ribbon styles, use the Ribbon Style combo box.
                        </Paragraph>",
					addedIn: KnownDXVersion.V142
				),
				new WpfModule(demo,
					name: "RibbonOfficeSlim",
					displayName: @"Office Slim Style",
					group: "Demos",
					type: "RibbonDemo.RibbonOfficeSlim",
					shortDescription: @"This application demonstrates the ""Office Universal"" Ribbon style.",
					description: @"
                        <Paragraph TextAlignment=""Justify"">
                        This example demonstrates use of the DevExpress WPF Ribbon Control to implement text editing commands. You can select text within the editing region to see the Ribbon's contextual tabs in action. To switch between different Ribbon styles, use the Ribbon Style combo box.
                        </Paragraph>",
					addedIn: KnownDXVersion.V142
				),
				new WpfModule(demo,
					name: "MVVMRibbon",
					displayName: @"MVVM Ribbon",
					group: "Tutorials",
					type: "RibbonDemo.MVVMRibbon",
					shortDescription: @"How to use RibbonControl with the MVVM pattern.",
					description: @"
                        <Paragraph TextAlignment=""Justify"">
                        This example demonstrates how to design the Ribbon UI by using the MVVM pattern.
                        </Paragraph>
                        <Paragraph TextAlignment=""Justify"">
                        Dedicated properties, templates and template selectors  are used to load contents from a model into RibbonControl.
                        </Paragraph>
                        <Paragraph TextAlignment=""Justify"">
                        For instance, see the XAML definition of RibbonControl where Ribbon categories are loaded from a Categories collection of a ViewModel class.
                        </Paragraph>
                        <Paragraph TextAlignment=""Justify"">
                        A specific template (categoryTemplate) is used to specify how each category item should be presented on-screen: pages for categories are loaded from the Pages collection of a CategoryModel class, which is assigned to a category's DataContext.
                        </Paragraph>
                        <Paragraph TextAlignment=""Justify"">
                        There are also templates for ApplicationMenu, pages, page groups, bar items and bar sub items.
                        </Paragraph>",
					addedIn: KnownDXVersion.Before142
				),
				new WpfModule(demo,
					name: "RibbonMergingUserControl",
					displayName: @"Ribbon Merging",
					group: "Tutorials",
					type: "RibbonDemo.RibbonMergingUserControl",
					shortDescription: @"This example shows DXRibbon's merging feature.",
					description: @"
                        <Paragraph TextAlignment=""Justify"">
                        Child panels are represented as floating windows each having a RibbonControl. In addition, the main window has its own RibbonControl. When you maximize a floating panel, its child RibbonControl is merged into the main RibbonControl. Click the 'Restore' button to unmerge Ribbon Controls and view child panels as floating ones.
                        </Paragraph>
                        <Paragraph TextAlignment=""Justify"">
                        Both the parent and child Ribbon Controls have 'Home' pages, so a merged Ribbon Control contains all commands in a single page.
                        </Paragraph>",
					addedIn: KnownDXVersion.Before142,
					isFeatured: true
				)
			};
		}
	}
}
