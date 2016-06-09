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
		static List<Module> Create_DXDocking_MainDemo_Modules(Demo demo) {
			return new List<Module> {
				new WpfModule(demo,
					name: "IDEWorkspaces",
					displayName: @"IDE Workspaces",
					group: "Docking Samples",
					type: "DockingDemo.IDEWorkspaces",
					shortDescription: @"This example demonstrates how to switch between different dock layouts using WorkspaceManager.",
					description: @"
                        <Paragraph>
                        This example demonstrates how to switch between different dock layouts using the WorkspaceManager component. The WorkspaceManager allows you to add visualization effects when switching between layouts. To see this in action, select the transition effect and then click any workspace name. You can also create your custom layout, capture it and then restore to this layout later at any time.
                        </Paragraph>",
					allowTouchThemes: false,
					addedIn: KnownDXVersion.Before142,	
					links: new[] {
						new WpfModuleLink(
							title: "WPF and Silverlight Workspace Manager (coming in v2010.2)",
							type: WpfModuleLinkType.Blogs,
							url: "http://community.devexpress.com/blogs/ctodx/archive/2010/11/11/wpf-and-silverlight-workspace-manager-coming-in-v2010-2.aspx"),
						new WpfModuleLink(
							title: "Workspace Manager - Getting Started",
							type: WpfModuleLinkType.Video,
							url: "http://tv.devexpress.com/DXWorkspaceManager-GettingStarted-wpf.movie"),
						new WpfModuleLink(
							title: "WorkspaceManager",
							type: WpfModuleLinkType.KB,
							url: "http://search.devexpress.com/?q=workspacemanager&p=T0%7cP0%7c0")
					}
				),
				new WpfModule(demo,
					name: "PanelAnimation",
					displayName: @"Panel Animation",
					group: "Docking Samples",
					type: "DockingDemo.PanelAnimation",
					shortDescription: @"In this demo, dock panels are resized using an animation effect.",
					description: @"
                        <Paragraph>
                        In this demo, dock panels are resized using an animation effect. Dock panels display stock indicators. Click any panel to maximize it. See how it's resized using an animation effect. To resize panels in this way, extension methods are used (BeginWidthAnimation and BeginHeightAnimation)
                        </Paragraph>",
					allowSwitchingThemes: false,
					addedIn: KnownDXVersion.Before142,
					isFeatured: true
				),
				new WpfModule(demo,
					name: "VS2010Docking",
					displayName: @"VS2010 Docking",
					group: "Docking Samples",
					type: "DockingDemo.VS2010Docking",
					shortDescription: @"The VS2010 docking mode emulates the dock behavior found in Visual Studio 2010",
					description: @"
                        <Paragraph>
                        The VS2010 docking mode emulates the dock behavior found in Visual Studio 2010. Practice dragging and dropping dock panels and documents and see dock guides helping you position dock panels at the right point. To enable this dock mode, use the DockingStyle property.
                        </Paragraph>",
					allowTouchThemes: false,
					addedIn: KnownDXVersion.Before142,
					isFeatured: true
				),
				new WpfModule(demo,
					name: "IDEDockLayout",
					displayName: @"IDE Dock Layout",
					group: "Docking Samples",
					type: "DockingDemo.IDEDockLayout",
					shortDescription: @"This example demonstrates basic docking features.",
					description: @"
                        <Paragraph>
                        This demo illustrates the docking capabilities of dock panels. In this sample, dock panels are used to emulate the familiar IDE user interface. You can drag and drop dock panels, docking them to the dock container or other dock panels. Press CTRL+TAB to invoke the Document Selector window, allowing you to easily switch to a specific panel or document.
                        </Paragraph>",
					allowTouchThemes: false,
					addedIn: KnownDXVersion.Before142,	
					links: new[] {
						new WpfModuleLink(
							title: "Tabbed and Document Groups",
							type: WpfModuleLinkType.Video,
							url: "http://tv.devexpress.com/DXDockingTabbedDocGroups.movie"),
						new WpfModuleLink(
							title: "Document Selector",
							type: WpfModuleLinkType.KB,
							url: "http://search.devexpress.com/?q=DocumentSelector&p=T0%7cP0%7c0")
					}
				),
				new WpfModule(demo,
					name: "Dashboard",
					displayName: @"Dashboard",
					group: "Docking Samples",
					type: "DockingDemo.Dashboard",
					shortDescription: @"This demo demonstrates how to implement data binding for controls positioned within LayoutPanels.",
					description: @"
                        <Paragraph>
                        This demo demonstrates how to implement data binding for controls positioned within LayoutPanels.
                        </Paragraph>",
					addedIn: KnownDXVersion.Before142,	
					links: new[] {
						new WpfModuleLink(
							title: "How to build a layout of controls within LayoutPanels",
							type: WpfModuleLinkType.Documentation,
							url: "http://documentation.devexpress.com/#WPF/CustomDocument7226")
					}
				),
				new WpfModule(demo,
					name: "DragPanelsBetweenDockManagers",
					displayName: @"Drag Panels Between Dock Managers",
					group: "Docking Features",
					type: "DockingDemo.DragPanelsBetweenDockManagers",
					shortDescription: @"This demo shows how to implement dragging-and-dropping panels between different Dock Managers.",
					description: @"
                        <Paragraph>
                        This demo shows how to enable panel drag-and-drop between different Dock Managers (by default this feature is disabled). The DockLayoutManagerLinker.Link method is used to accomplish this task. This method connects two Dock Managers, making it possible for panels to change their parent Dock Manager during drag-and-drop. The DockLayoutManagerLinker.Link method is called by a service attached to each Dock Manager using a style.
                        </Paragraph>",
					addedIn: KnownDXVersion.V151	
				),
				new WpfModule(demo,
					name: "MVVMSerialization",
					displayName: @"MVVM Serialization",
					group: "Docking Features",
					type: "DockingDemo.MVVMSerialization",
					shortDescription: @"This example shows how to save and load the layout of dock panels according to the MVVM pattern.",
					description: @"
                        <Paragraph>
                        The layout of DockLayoutManager can be saved to a data store (an XML file or stream) and then can be restored later. For instance, you can save the layout, modify it, and then revert to the original layout using the Restore command. Practice using the buttons on the right to see this functionality in action.
                        </Paragraph>",
					addedIn: KnownDXVersion.Before142,	
					links: new[] {
						new WpfModuleLink(
							title: "IDEWorkspaces Demo",
							type: WpfModuleLinkType.Demos,
							url: "local:IDE Workspaces"),
						new WpfModuleLink(
							title: "Saving and Restoring the Layout of Dock Panels and Controls",
							type: WpfModuleLinkType.Documentation,
							url: "http://documentation.devexpress.com/#WPF/CustomDocument7059")
					}
				),
				new WpfModule(demo,
					name: "MVVMGroupLayout",
					displayName: @"MVVM Group Layout",
					group: "Docking Features",
					type: "DockingDemo.MVVMGroupLayout",
					shortDescription: @"This demo shows how to generate a content for a LayoutGroup according to the MVVM pattern.",
					description: @"
                        <Paragraph>
                        This example demonstrates how to use an ItemsSource collection according to the MVVM pattern. Tab headers and contents are defined via templates. Check out the Xaml mark-up to see how it works.
                        </Paragraph>",
					addedIn: KnownDXVersion.Before142
				),
				new WpfModule(demo,
					name: "MVVMDockLayout",
					displayName: @"MVVM Dock Layout",
					group: "Docking Features",
					type: "DockingDemo.MVVMDockLayout",
					shortDescription: @"This example demonstrates how to implement a dock UI by using the MVVM pattern.",
					description: @"
                        <Paragraph>
                        This example demonstrates how to use the ItemsSource collection for the DockLayoutManager object in order to apply an MVVM pattern for your application. All BarManager and DockLayoutManager items are defined in the data source. They are bound via the ItemsSource property and visualized via templates. Elements are added and organized in containers according to the attached TargetName property.
                        </Paragraph>",
					allowTouchThemes: false,
					addedIn: KnownDXVersion.Before142
				),
				new WpfModule(demo,
					name: "DockHintsVisualizer",
					displayName: @"Dock Hints Visualizer",
					group: "Docking Features",
					type: "DockingDemo.DockHintsVisualizer",
					shortDescription: @"This demo shows how to control the dock hints availability via events.",
					description: @"
                        <Paragraph>
                        This demo shows how to control the dock hints availability via events. A dedicated event is used to control which dock hints should be available to end-users when hovering over individual panels. You can temporarily suppress the custom logic via an option provided on the right.
                        </Paragraph>",
					addedIn: KnownDXVersion.Before142,	
					links: new[] {
						new WpfModuleLink(
							title: "ShowingDockHints",
							type: WpfModuleLinkType.KB,
							url: "http://search.devexpress.com/?q=ShowingDockHints&p=T0%7cP0%7c0")
					}
				),
				new WpfModule(demo,
					name: "AppearanceProperties",
					displayName: @"Appearance Properties",
					group: "Docking Features",
					type: "DockingDemo.AppearanceProperties",
					shortDescription: @"This example demonstrates appearance options available for dock panels.",
					description: @"
                        <Paragraph>
                        This example demonstrates the appearance options of dock panels. Using these options, you can quickly customize the appearance of a dock panel's header and content region without overriding templates.
                        </Paragraph>",
					addedIn: KnownDXVersion.Before142
				),
				new WpfModule(demo,
					name: "FloatingPanels",
					displayName: @"Floating Panels",
					group: "Docking Features",
					type: "DockingDemo.FloatingPanels",
					shortDescription: @"This example demonstrates two floating modes available for dock panels.",
					description: @"
                        <Paragraph>
                        This demo illustrates the two floating modes for dock panels. Practice dragging floating panels to see how they move inside dock containers.
                        </Paragraph>",
					addedIn: KnownDXVersion.Before142,	
					links: new[] {
						new WpfModuleLink(
							title: "WPF Docking - Floating Panels",
							type: WpfModuleLinkType.Video,
							url: "http://tv.devexpress.com/DXDockingFloatingPanels.movie")
					}
				),
				new WpfModule(demo,
					name: "AutoHiddenPanels",
					displayName: @"Auto-Hidden Panels",
					group: "Docking Features",
					type: "DockingDemo.AutoHiddenPanels",
					shortDescription: @"This demo demonstrates the auto-hide functionality.",
					description: @"
                        <Paragraph>
                        This demo demonstrates the auto-hide functionality. When this feature is enabled, a dock panel is minimized to the window's corresponding edge. You can hover over a minimized panel's label to open the panel. Click the panel's Pin button to restore the panel to its normal state. This demo shows two different modes to expand auto-hidden panels. Choose whether you want to expand panels on mouse hovering or mouse clicking. You can also specify whether an auto-hidden panel overlaps the layout when expanded, or pushes (shifts) the layout.
                        </Paragraph>",
					addedIn: KnownDXVersion.Before142,	
					updatedIn: KnownDXVersion.V151,
					links: new[] {
						new WpfModuleLink(
							title: "How to create auto-hidden panels",
							type: WpfModuleLinkType.Documentation,
							url: "http://documentation.devexpress.com/WPF/CustomDocument6835.aspx")
					}
				),
				new WpfModule(demo,
					name: "ClosedPanels",
					displayName: @"Closed Panels",
					group: "Docking Features",
					type: "DockingDemo.ClosedPanels",
					shortDescription: @"This example demonstrates a special bar designed to display links to closed panels.",
					description: @"
                        <Paragraph>
                        This example demonstrates a special bar designed to display links to closed panels. When a panel is closed, a link to the panel is added to the bar. You can click a link to open the panel. You can use the ""ClosedPanelsBar Position"" radio group to control the bar's position.
                        </Paragraph>",
					addedIn: KnownDXVersion.Before142,	
					links: new[] {
						new WpfModuleLink(
							title: "Create and Manage ClosedPanels",
							type: WpfModuleLinkType.Video,
							url: "http://tv.devexpress.com/DXDockingCreateManageClosedPanels.movie"),
						new WpfModuleLink(
							title: "How to create closed (hidden) panels",
							type: WpfModuleLinkType.Documentation,
							url: "http://documentation.devexpress.com/WPF/CustomDocument6836.aspx")
					}
				),
				new WpfModule(demo,
					name: "Serialization",
					displayName: @"Serialization",
					group: "Docking Features",
					type: "DockingDemo.Serialization",
					shortDescription: @"This example shows how to save and load the layout of dock panels.",
					description: @"
                        <Paragraph>
                        The layout of DockLayoutManager can be saved to a data store (an XML file or stream) and then can be restored later. For instance, you can save the layout, modify it, and then revert to the original layout using the Restore command. Practice using the buttons on the right to see this functionality in action.
                        </Paragraph>",
					addedIn: KnownDXVersion.Before142,	
					links: new[] {
						new WpfModuleLink(
							title: "IDEWorkspaces Demo",
							type: WpfModuleLinkType.Demos,
							url: "local:IDE Workspaces"),
						new WpfModuleLink(
							title: "Saving and Restoring the Layout of Dock Panels and Controls",
							type: WpfModuleLinkType.Documentation,
							url: "http://documentation.devexpress.com/#WPF/CustomDocument7059")
					}
				),
				new WpfModule(demo,
					name: "PanelOptions",
					displayName: @"Panel Options",
					group: "Docking Features",
					type: "DockingDemo.PanelOptions",
					shortDescription: @"This demo shows various options that allow you to customize the behavior of dock panel.",
					description: @"
                        <Paragraph>
                        This demo shows various options that allow you to customize the behavior of dock panel. Notice that panels provide context menus (invoked by right-clicking the panel's header) that contain various panel commands.
                        </Paragraph>",
					addedIn: KnownDXVersion.Before142
				),
				new WpfModule(demo,
					name: "PanelGroups",
					displayName: @"Panel Groups",
					group: "Docking Features",
					type: "DockingDemo.PanelGroups",
					shortDescription: @"This demo shows how to change the orientation of panels and enable splitters within a group.",
					description: @"
                        <Paragraph>
                        Dock panels can be combined into groups (LayoutGroup objects). This demo shows how to change the orientation of panels and enable splitters within a group.
                        </Paragraph>",
					addedIn: KnownDXVersion.Before142
				),
				new WpfModule(demo,
					name: "BarsIntegration",
					displayName: @"Bars Integration",
					group: "Docking Features",
					type: "DockingDemo.BarsIntegration",
					shortDescription: @"This demo demonstrates an automatic integration of bars from the DXBars library into dock panels.",
					description: @"
                        <Paragraph>
                        DXDocking supports automatic integration of bars from the DXBars library into dock panels. Dock panels have embedded bar containers along their top edges, allowing bars to be positioned. You can add a bar to this container by setting a single property.
                        </Paragraph>",
					addedIn: KnownDXVersion.Before142,	
					links: new[] {
						new WpfModuleLink(
							title: "How to embed a bar into a dock panel",
							type: WpfModuleLinkType.Documentation,
							url: "http://documentation.devexpress.com/WPF/CustomDocument6844.aspx")
					}
				),
				new WpfModule(demo,
					name: "DockLayoutManagerEvents",
					displayName: @"Dock Layout Manager Events ",
					group: "Docking Features",
					type: "DockingDemo.DockLayoutManagerEvents",
					shortDescription: @"This demo acquaints you with dock operations provided by the Dock Layout Manager.",
					description: @"
                        <Paragraph>
                        This demo acquaints you with dock operations provided by the Dock Layout Manager. All operations you perform on dock panels are logged in the panel on the right.
                        </Paragraph>",
					addedIn: KnownDXVersion.V152
				),
				new WpfModule(demo,
					name: "NWindLayout",
					displayName: @"NWindLayout",
					group: "Layout Samples",
					type: "DockingDemo.NWindLayout",
					shortDescription: @"This example demonstrates how to implement data binding for layout items.",
					description: @"
                        <Paragraph>
                        This example demonstrates how to implement data binding for layout items. The layout of items can be customized in Customization Mode. To invoke Customization Mode, right-click any item's label and select ""Begin customization"" Take note of splitters that allow you to quickly resize layout items. You can add additional splitters via the Customization Window (Right-click an empty space and select ""Begin customization"". Then switch to the ""Hidden Items"" page.)
                        </Paragraph>",
					allowTouchThemes: false,
					addedIn: KnownDXVersion.Before142
				),
				new WpfModule(demo,
					name: "LayoutItems",
					displayName: @"Layout Items",
					group: "Layout Features",
					type: "DockingDemo.LayoutItems",
					shortDescription: @"This example demonstrates various layout features provided by layout items.",
					description: @"
                        <Paragraph>
                        This example demonstrates various layout features provided by layout items: displaying/hiding controls and labels, customizing the position of images and images, and alignment of labels. Practise customizing the layout of items in Customization Mode. To invoke Customization Mode, right-click any item's label and select ""Begin customization"". In this mode, you can change display options of layout items via an item's context menu. In this version, we've added the ability to customize layout item templates. See an example of this item in the current demo.
                        </Paragraph>",
					allowTouchThemes: false,
					addedIn: KnownDXVersion.Before142
				),
				new WpfModule(demo,
					name: "LayoutGroups",
					displayName: @"Layout Groups",
					group: "Layout Features",
					type: "DockingDemo.LayoutGroups",
					shortDescription: @"This example demonstrates various layout features provided by layout groups.",
					description: @"
                        <Paragraph>
                        This example demonstrates various layout features provided by layout groups: vertical and horizontal orientation of items, nested groups, collapsible groups (the GroupBox view style) and tabbed interface.
                        </Paragraph>",
					allowTouchThemes: false,
					addedIn: KnownDXVersion.Before142
				),
				new WpfModule(demo,
					name: "LayoutPanels",
					displayName: @"Layout Panels",
					group: "Layout Features",
					type: "DockingDemo.LayoutPanels",
					shortDescription: @"This example demonstrates layout items used within Layout Panels.",
					description: @"
                        <Paragraph>
                        This example demonstrates layout items used within Layout Groups. Note that the controls are automatically aligned according to the labels of the layout items across the Layout Groups
                        </Paragraph>",
					addedIn: KnownDXVersion.Before142
				),
				new WpfModule(demo,
					name: "ItemsVisibility",
					displayName: @"Items Visibility",
					group: "Layout Features",
					type: "DockingDemo.ItemsVisibility",
					shortDescription: @"This example demonstrates the item visibility feature in action.",
					description: @"
                        <Paragraph>
                        This example demonstrates the item visibility feature in action. An item's visibility is dynamically changed depending on specific conditions. Click the Start Edit button to display invisible items. Click End Edit to hide them again. An end-user can access invisible items by opening the Customization Window and checking the ""Show invisible items"" check box.
                        </Paragraph>",
					allowTouchThemes: false,
					addedIn: KnownDXVersion.Before142
				),
				new WpfModule(demo,
					name: "MDIMenuMerging",
					displayName: @"MDI Menu Merging",
					group: "MDI Samples",
					type: "DockingDemo.MDIMenuMerging",
					shortDescription: @"This example demonstrates the bar merging feature available in MDI mode.",
					description: @"
                        <Paragraph>
                        This example shows the bar merging feature in MDI mode, implemented with the help of DocumentGroup and DocumentPanel controls included into the DXDocking Suite. The document panels and their container contain their own menus and bars. The menus and bars are merged when the merging mechanism is invoked: 1) in classic MDI mode, when a document panel is maximized; 2) in tabbed MDI mode, when switching to any panel.
                        </Paragraph>",
					addedIn: KnownDXVersion.Before142,
					isFeatured: true,	
					links: new[] {
						new WpfModuleLink(
							title: "MDI using the WPF Project Wizard",
							type: WpfModuleLinkType.Video,
							url: "http://tv.devexpress.com/DXDockingMDIProjectWizard.movie"),
						new WpfModuleLink(
							title: "MDI Image Viewer Demo",
							type: WpfModuleLinkType.Demos,
							url: "local:Image Viewer"),
						new WpfModuleLink(
							title: "How to enable MDI mode for a DocumentGroup",
							type: WpfModuleLinkType.Documentation,
							url: "http://documentation.devexpress.com/#WPF/CustomDocument8127"),
						new WpfModuleLink(
							title: "MDI Bar Merging",
							type: WpfModuleLinkType.Documentation,
							url: "http://documentation.devexpress.com/#WPF/CustomDocument9155")
					}
				),
				new WpfModule(demo,
					name: "MDIImageViewer",
					displayName: @"Image Viewer",
					group: "MDI Samples",
					type: "DockingDemo.MDIImageViewer",
					shortDescription: @"This example shows how to implement the MDI interface for a DocumentGroup.",
					description: @"
                        <Paragraph>
                        This example shows how to implement the MDI interface for a DocumentGroup. The MDI interface is invoked via the MDIStyle property. Practice clicking the window's header and selecting ""New horizontal tab group""
                        </Paragraph>",
					addedIn: KnownDXVersion.Before142,	
					links: new[] {
						new WpfModuleLink(
							title: "Loading external XAML content into DocumentPanels",
							type: WpfModuleLinkType.Blogs,
							url: "http://community.devexpress.com/blogs/theprogressbar/archive/2010/08/17/loading-external-xaml-content-into-documentpanels-docking-control-for-wpf-dxdocking.aspx"),
						new WpfModuleLink(
							title: "How to load an external Window or UserControl into a DocumentPanel",
							type: WpfModuleLinkType.Documentation,
							url: "http://documentation.devexpress.com/#WPF/CustomDocument8592")
					}
				),
				new WpfModule(demo,
					name: "MDIQuickNotes",
					displayName: @"Quick Notes",
					group: "MDI Samples",
					type: "DockingDemo.MDIQuickNotes",
					shortDescription: @"This example demonstrates the tabbed MDI interface applied to a DocumentGroup.",
					description: @"
                        <Paragraph>
                        This example demonstrates the tabbed MDI interface applied to a DocumentGroup. Note custom controls added to the first page.
                        </Paragraph>",
					allowTouchThemes: false,
					addedIn: KnownDXVersion.Before142,	
					links: new[] {
						new WpfModuleLink(
							title: "WPF Navigation Bar Control",
							type: WpfModuleLinkType.Documentation,
							url: "http://www.devexpress.com/Products/NET/Controls/WPF/Navbar/")
					}
				),
				new WpfModule(demo,
					name: "DocumentGroups",
					displayName: @"Document Groups",
					group: "MDI Features",
					type: "DockingDemo.DocumentGroups",
					shortDescription: @"This sample demonstrates various features supported by the MDI interface.",
					description: @"
                        <Paragraph>
                        This sample demonstrates various features supported by the MDI interface: changing the tab header location, orientation, layout, etc. Scroll buttons are displayed within the tab header when there are multiple tabs. Use the buttons at the bottom of the window to add new tabs.
                        </Paragraph>",
					addedIn: KnownDXVersion.Before142
				)
			};
		}
	}
}
