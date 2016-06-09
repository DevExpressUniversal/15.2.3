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
		static List<Module> Create_DXNavBar_MainDemo_Modules(Demo demo) {
			return new List<Module> {
				new WpfModule(demo,
					name: "CustomTheming",
					displayName: @"Custom Theming",
					group: "Features",
					type: "NavBarDemo.CustomTheming",
					shortDescription: @"This example demonstrates how to create a custom theme for NavBar Control's SideBar view.",
					description: @"
                        <Paragraph>
                        This example demonstrates how to create a custom theme for NavBar Control's SideBar view. See how group headers are painted. Practice with scrolling group contents using scroll controls embedded in group headers.
                        </Paragraph>",
					allowSwitchingThemes: false,
					addedIn: KnownDXVersion.Before142,
					isFeatured: true
				),
				new WpfModule(demo,
					name: "GroupsAndItems",
					displayName: @"Groups and Items",
					group: "Features",
					type: "NavBarDemo.GroupsAndItems",
					shortDescription: @"This demo illustrates how to add and remove groups and items.",
					description: @"
                        <Paragraph>
                        This demo illustrates how to add and remove groups and items. Before adding an item, you can customize its image and image size.
                        </Paragraph>",
					addedIn: KnownDXVersion.Before142
				),
				new WpfModule(demo,
					name: "LayoutCustomization",
					displayName: @"Layout Customization",
					group: "Features",
					type: "NavBarDemo.LayoutCustomization",
					shortDescription: @"This example demonstrates various paint styles available for the NavBar Control.",
					description: @"
                        <Paragraph>
                        In this example, you can explore different DXNavBar views. Each view represents navigational information in its own specific way. You can experiment switching between views and changing various layout options.
                        </Paragraph>",
					addedIn: KnownDXVersion.Before142
				),
				new WpfModule(demo,
					name: "NavigationPaneOptions",
					displayName: @"Navigation Pane",
					group: "Features",
					type: "NavBarDemo.NavigationPaneOptions",
					shortDescription: @"This example demonstrates the Navigation Pane view.",
					description: @"
                        <Paragraph>
                        This example demonstrates the Navigation Pane view. This view allows you to collapse the DXNavBar, and shows the contents of the current group in the collapsed state. You can also change the number of visible group buttons at the bottom, and so on.
                        </Paragraph>",
					addedIn: KnownDXVersion.Before142
				),
				new WpfModule(demo,
					name: "DataBinding",
					displayName: @"Data Binding",
					group: "Features",
					type: "NavBarDemo.DataBinding",
					shortDescription: @"Demonstrates how to populate DXNavBar with groups and items from a data source",
					description: @"
                        <Paragraph>
                        This example demonstrates how to populate the DXNavBar control with groups and items from a data source.
                        </Paragraph>",
					addedIn: KnownDXVersion.Before142
				),
				new WpfModule(demo,
					name: "SelectionOptions",
					displayName: @"Selection",
					group: "Features",
					type: "NavBarDemo.SelectionOptions",
					shortDescription: @"This example demonstrates various item selection settings in different DXNavBar Views.",
					description: @"
                        <Paragraph>
                        This example demonstrates various item selection settings in different DXNavBar Views.
                        </Paragraph>",
					addedIn: KnownDXVersion.Before142
				),
				new WpfModule(demo,
					name: "ScrollingOptions",
					displayName: @"Scrolling",
					group: "Features",
					type: "NavBarDemo.ScrollingOptions",
					shortDescription: @"In this example, you can experiment with various scroll options available for the NavBar Control.",
					description: @"
                        <Paragraph>
                        In this example, you can experiment with various scroll options, such as scroll type, speed, and so on.
                        </Paragraph>",
					addedIn: KnownDXVersion.Before142
				),
				new WpfModule(demo,
					name: "Events",
					displayName: @"Events",
					group: "Features",
					type: "NavBarDemo.Events",
					shortDescription: @"This example shows main events provided by DXNavBar.",
					description: @"
                        <Paragraph>
                        Every time an end-user clicks, selects, or moves the mouse over DXNavBar, various events fire. This example shows main events supported by DXNavBar that allow you to respond to an end-user's actions.
                        </Paragraph>",
					addedIn: KnownDXVersion.Before142
				)
			};
		}
	}
}
