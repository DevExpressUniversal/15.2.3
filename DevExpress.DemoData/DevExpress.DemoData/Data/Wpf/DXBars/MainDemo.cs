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
		static List<Module> Create_DXBars_MainDemo_Modules(Demo demo) {
			return new List<Module> {
				new WpfModule(demo,
					name: "SimplePad",
					displayName: @"Menu, Toolbar & Status Bar UI",
					group: "Demos",
					type: "BarsDemo.SimplePad",
					shortDescription: @"A demo application that uses the DXBars library.",
					description: @"
                        <Paragraph>
                        A demo application that demonstrates the Toolbar and Menu system providing commands for a simple text editor.
                        </Paragraph>
                        <Paragraph>
                        In this demo, the <Span FontWeight=""Bold"">DevExpress Toolbar and Menu library</Span> is used to implement text editing commands in a simple text editor. Practice using bar commands to control the appearance of the editor's text.
                        </Paragraph>",
					addedIn: KnownDXVersion.Before142,
					updatedIn: KnownDXVersion.V142,
					isFeatured: true
				),
				new WpfModule(demo,
					name: "RadialContextMenu",
					displayName: @"Radial Context Menu",
					group: "Tutorials",
					type: "BarsDemo.RadialContextMenu",
					shortDescription: @"This example demonstrates use of the OneNote inspired radial context menu.",
					description: @"
                        <Paragraph>
                        This demo illustrates how to create a radial context menu and associate it to a control. <LineBreak/> The radial context menu, represented by the RadialContextMenu class, is activated on a right click or tap and hold gesture.
                        </Paragraph>",
					addedIn: KnownDXVersion.V142
				),
				new WpfModule(demo,
					name: "BarProperties",
					displayName: @"Toolbar & Menu: Properties",
					group: "Tutorials",
					type: "BarsDemo.BarProperties",
					shortDescription: @"This example demonstrates the customization features for bars.",
					description: @"
                        <Paragraph>
                        This example demonstrates customization features available for Toolbar and Menu components.
                        </Paragraph>
                        <Paragraph>
                        You can make a bar, a regular bar, a menu bar, or a status bar, and see how this affects the bar's appearance, position, and behavior.
                        </Paragraph>",
					addedIn: KnownDXVersion.Before142
				),
				new WpfModule(demo,
					name: "ItemProperties",
					displayName: @"Toolbar Items: Properties",
					group: "Tutorials",
					type: "BarsDemo.ItemProperties",
					shortDescription: @"This example demonstrates customization features available for individual bar items (commands).",
					description: @"
                        <Paragraph>
                        This demo illustrates the customization properties for bar items.
                        </Paragraph>
                        <Paragraph>
                        Select a bar item using the radio group, and then see what happens when you customize the item's settings. For different items different settings are available.
                        </Paragraph>",
					addedIn: KnownDXVersion.Before142
				),
				new WpfModule(demo,
					name: "ContainerItems",
					displayName: @"Container Items",
					group: "Tutorials",
					type: "BarsDemo.ContainerItems",
					shortDescription: @"This demo illustrates container bar items.",
					description: @"
                        <Paragraph>
                        This demo illustrates container bar items.
                        </Paragraph>
                        <Paragraph>
                        The BarLinkContainerItem maintains a list of bar items. Whenever you add a BarLinkContainerItem to a bar or menu, it's expanded to display the items it owns.
                        </Paragraph>
                        <Paragraph>
                        The same refers to the ToolbarListItem, which is a container item that automatically maintains a list of existing toolbars (and optionally, bar items). When you add this item to a bar or menu, it's expanded to display the names of the existing toolbars (and bar items), allowing an end-user to control the visibility of the bar objects.
                        </Paragraph>",
					addedIn: KnownDXVersion.Before142
				),
				new WpfModule(demo,
					name: "SaveAndRestoreLayout",
					displayName: @"Save & Restore Layout",
					group: "Tutorials",
					type: "BarsDemo.SaveAndRestoreLayout",
					shortDescription: @"This example demonstrate the layout persistence feature.",
					description: @"
                        <Paragraph>
                        The layout of bars can be saved to a data store (an XML file, stream, or the system registry) and then can be restored at a later stage.
                        </Paragraph>
                        <Paragraph>
                        For instance, you can save the layout, modify it, and then revert to the original layout using the Restore command. Practice using the buttons on the right to see this functionality in action.
                        </Paragraph>",
					addedIn: KnownDXVersion.Before142
				),
				new WpfModule(demo,
					name: "Commands",
					displayName: @"Bind Toolbar Items to Commands",
					group: "Tutorials",
					type: "BarsDemo.Commands",
					shortDescription: @"This demo shows how to bind commands to bar items.",
					description: @"
                        <Paragraph>
                        The DXBars library fully supports commands. <LineBreak/> You can bind any bar item to a command that will be automatically invoked when the item is clicked. In addition, an item's enabled state is automatically controlled by the corresponding command's setting.
                        </Paragraph>
                        <Paragraph>
                        In this demo, all the bar items are bound to commands on controls. Practice clicking bar items to see the bar item functionality in action.
                        </Paragraph>",
					addedIn: KnownDXVersion.Before142
				),
				new WpfModule(demo,
					name: "ContextMenu",
					displayName: @"Context Menu",
					group: "Tutorials",
					type: "BarsDemo.ContextMenu",
					shortDescription: @"This example demonstrates how to create a context menu for a control.",
					description: @"
                        <Paragraph>
                        This demo shows how to create a context menu for a control. <LineBreak/> The context menu, represented by the PopupMenu class, is automatically opened on right-clicking the control.
                        </Paragraph>",
					addedIn: KnownDXVersion.Before142
				),
				new WpfModule(demo,
					name: "MVVMBar",
					displayName: @"MVVM Bars",
					group: "MVVM",
					type: "BarsDemo.MVVMBar",
					shortDescription: @"This example shows how to implement the Bars UI by using the MVVM pattern.",
					description: @"
                        <Paragraph TextAlignment=""Justify"">
                        This example demonstrates how to design the Bars UI by using the MVVM pattern.
                        </Paragraph>
                        <Paragraph TextAlignment=""Justify"">
                        Dedicated properties, templates and template selectors are used to load contents from a model into BarManager.
                        </Paragraph>
                        <Paragraph TextAlignment=""Justify"">
                        For instance, see the XAML definition of a BarManager component where bars are loaded from a Bars collection of a ViewModel class.
                        </Paragraph>
                        <Paragraph TextAlignment=""Justify"">
                        A specific template (barTemplate) is used to specify how each bar should be presented on-screen: a bar's items are loaded from a Commands collection of a BarModel class, which is assigned to a bar's DataContext.
                        </Paragraph>
                        <Paragraph TextAlignment=""Justify"">
                        There are also templates for bar items and bar sub items.
                        </Paragraph>",
					addedIn: KnownDXVersion.V142
				),
				new WpfModule(demo,
					name: "ImplicitDataTemplates",
					displayName: @"Implicit Data Templates",
					group: "MVVM",
					type: "BarsDemo.ImplicitDataTemplates",
					shortDescription: @"This example demonstrates use of implicit data templates.",
					description: @"
                        <Paragraph>
                        Implicit Data Templates allows templates to be automatically applied to data items based on types. In this demo different data templates are associated with bar items based on the specified DataType property.
                        </Paragraph>",
					addedIn: KnownDXVersion.V142
				),
				new WpfModule(demo,
					name: "CustomMerging",
					displayName: @"Custom Merging",
					group: "Merging",
					type: "BarsDemo.CustomMerging",
					shortDescription: @"This example demonstrates custom bar merging using standard controls.",
					description: @"
                        <Paragraph>
                        The DevExpress Toolbar's custom merging feature provides a straightforward way to change the contents of toolbars based upon the active UI element. To enable it within your app, use the MergingProperties.AllowMerging attached property. <LineBreak/>In this demo, each tab item has an associated set of buttons. When merging is enabled, these buttons are displayed within the main menu bar. Switching between tab items changes the displayed button set.
                        </Paragraph>",
					addedIn: KnownDXVersion.V142
				),
				new WpfModule(demo,
					name: "DockMerging",
					displayName: @"Dock Merging",
					group: "Merging",
					type: "BarsDemo.DockMerging",
					shortDescription: @"This example demonstrates custom bar merging using the Docking suite.",
					description: @"
                        <Paragraph>
                        The MDIChild main menu can be automatically merged with the MDIParent main menu. The DockLayoutManager.Merge event allows you to customize menus and bars whenever a child MDI form is activated or maximized. The document panels and their containers have their own menus and bars which can be merged. The merging mechanism is applied: <LineBreak/>1) in classic MDI mode, when a document panel is maximized; <LineBreak/>2) in tabbed MDI mode, when switching to any panel.
                        </Paragraph>",
					addedIn: KnownDXVersion.V142
				),
				new WpfModule(demo,
					name: "NavigationFrameMerging",
					displayName: @"NavigationFrame Merging",
					group: "Merging",
					type: "BarsDemo.NavigationFrameMerging",
					shortDescription: @"This example demonstrates custom bar merging using the NavigationFrame control.",
					description: @"
                        <Paragraph>
                        The DevExpress Toolbar's custom merging feature provides a straightforward way to change the contents of toolbars based upon the active UI element. To enable it within your app, use the NavigationFrame.AllowMerging property. When enabled, switching between frames changes the buttons displayed within the main menu bar.
                        </Paragraph>",
					addedIn: KnownDXVersion.V142
				),
				new WpfModule(demo,
					name: "DXTabControlMerging",
					displayName: @"DXTabControl Merging",
					group: "Merging",
					type: "BarsDemo.DXTabControlMerging",
					shortDescription: @"This example demonstrates custom bar merging using the DXTabControl control.",
					description: @"
                        <Paragraph>
                        The DevExpress Toolbar's custom merging feature provides a straightforward way to change the contents of toolbars based upon the active UI element. To enable it within your app, use the DXTabControl.AllowMerging property. When merging is enabled, switching between tabs changes the buttons displayed within the main menu bar.         </Paragraph>",
					addedIn: KnownDXVersion.V142
				)
			};
		}
	}
}
