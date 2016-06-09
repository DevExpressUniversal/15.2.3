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
		static List<Module> Create_ApplicationUI_MainDemo_Modules(Demo demo) {
			return new List<Module> {
				new SimpleModule(demo,
					name: "About",
					displayName: @"DevExpress XtraBars %MarketingVersion%",
					group: "About",
					type: "DevExpress.ApplicationUI.Demos.About",
					description: @"",
					addedIn: KnownDXVersion.Before142
				),
				new SimpleModule(demo,
					name: "UtilityControlsAPI",
					displayName: @"Utility Controls API",
					group: "API Code Examples",
					type: "DevExpress.ApplicationUI.Demos.UtilityControlsModule",
					description: @"This demo illustrates main capabilities of the the FlyoutDialog and TransitionManager components. Each group contains several simple examples that you can open in your Visual Studio by clicking the corresponding Demo Center button.",
					addedIn: KnownDXVersion.V151,
					isCodeExample: true
				),
				new SimpleModule(demo,
					name: "DocumentManagerAPI",
					displayName: @"DocumentManager API",
					group: "API Code Examples",
					type: "DevExpress.ApplicationUI.Demos.DocumentManagerModule",
					description: @"This demo illustrates all features and capabilities of the DocumentManager component. The demo contains simple examples, covering this or that particular feature. All examples are united into groups in accordance to the aspect, common for these examples (e.g., the WindowsUIView).",
					addedIn: KnownDXVersion.V151,
					isCodeExample: true
				),
				new SimpleModule(demo,
					name: "DocumentManagerTabbedDocuments",
					displayName: @"Tabbed Documents",
					group: "Multiple Documents UI",
					type: "DevExpress.ApplicationUI.Demos.DocumentManagerTabbedDocuments",
					description: @"This demo demonstrates the Tabbed UI implemented via DocumentManager. The Tabbed View displays documents as tabs. An end-user can navigate through tabs by clicking their headers or by pressing Ctrl+Tab.",
					addedIn: KnownDXVersion.Before142,
					associatedFiles: new [] {
						@"\WinForms\CS\ApplicationUIMainDemo\Modules\DocumentManager\DocumentManagerTabbedDocuments.cs",
						@"\WinForms\VB\ApplicationUIMainDemo\Modules\DocumentManager\DocumentManagerTabbedDocuments.vb"
					}
				),
				new SimpleModule(demo,
					name: "DocumentManagerTabbedDocumentsFreeLayout",
					displayName: @"Free Layout In TabbedView",
					group: "Multiple Documents UI",
					type: "DevExpress.ApplicationUI.Demos.DocumentManagerTabbedDocumentsFreeLayout",
					description: @"In Free Layout mode, document groups are placed to docking containers. Each container is created dynamically and can host numerous child containers or one document group. This allows you to mix vertically and horizontally oriented groups. You can drag documents to create new or modify existing document groups.",
					addedIn: KnownDXVersion.V152,
					associatedFiles: new [] {
						@"\WinForms\CS\ApplicationUIMainDemo\Modules\DocumentManager\DocumentManagerTabbedDocumentsFreeLayout.cs",
						@"\WinForms\VB\ApplicationUIMainDemo\Modules\DocumentManager\DocumentManagerTabbedDocumentsFreeLayout.vb"
					}
				),
				new SimpleModule(demo,
					name: "DocumentManagerNative",
					displayName: @"Native Documents",
					group: "Multiple Documents UI",
					type: "DevExpress.ApplicationUI.Demos.DocumentManagerNative",
					description: @"This demo demonstrates the Native UI implemented via DocumentManager. The native MDI view displays Documents as floating panels. An end-user can navigate though documents, resize, rearrange, cascade or tile documents vertically/horizontally.",
					addedIn: KnownDXVersion.Before142,
					associatedFiles: new [] {
						@"\WinForms\CS\ApplicationUIMainDemo\Modules\DocumentManager\DocumentManagerNative.cs",
						@"\WinForms\VB\ApplicationUIMainDemo\Modules\DocumentManager\DocumentManagerNative.vb"
					}
				),
				new SimpleModule(demo,
					name: "DocumentManagerDeferredLoadDocuments",
					displayName: @"Deferred Load Documents",
					group: "Multiple Documents UI",
					type: "DevExpress.ApplicationUI.Demos.DocumentManagerDeferredLoadDocuments",
					description: @"This demo demonstrates the process of loading documents in deferred mode. At the application start-up only the first visible document is loaded and displayed. Further documents are loaded on demand - when a corresponding tab header has been clicked.",
					addedIn: KnownDXVersion.Before142,
					associatedFiles: new [] {
						@"\WinForms\CS\ApplicationUIMainDemo\Modules\DocumentManager\DocumentManagerDeferredLoadDocuments.cs",
						@"\WinForms\VB\ApplicationUIMainDemo\Modules\DocumentManager\DocumentManagerDeferredLoadDocuments.vb"
					}
				),
				new SimpleModule(demo,
					name: "DocumentManagerDocking",
					displayName: @"Docking UI Integration",
					group: "Multiple Documents UI",
					type: "DevExpress.ApplicationUI.Demos.DocumentManagerDocking",
					description: @"This demo demonstrates how to integrate DocumentManager with the DockManager component. The DockManager component allows you to add DockPanels to your form. When used along with the DocumentManager component in a single form (user control), it gains additional features(unified dock hints, document selector, etc.).",
					addedIn: KnownDXVersion.Before142,
					associatedFiles: new [] {
						@"\WinForms\CS\ApplicationUIMainDemo\Modules\DocumentManager\DocumentManagerDocking.cs",
						@"\WinForms\VB\ApplicationUIMainDemo\Modules\DocumentManager\DocumentManagerDocking.vb"
					}
				),
				new SimpleModule(demo,
					name: "DocumentManagerWindowsUISearchPanel",
					displayName: @"WindowsUI Search Panel",
					group: "Multiple Documents UI",
					type: "DevExpress.ApplicationUI.Demos.DocumentManagerWindowsUISearchPanel",
					description: @"This demo illustrates the use of a search panel within the Windows UI View. To invoke the panel, press the Ctrl+F key combination. The panel searches for text within the document, tile, container titles, content and search tags. With the use of search parameters, you can explicitly narrow the search process to values at specified locations (""tag: Accessories"", ""content: Texas"", ""text: Sales by state"").",
					addedIn: KnownDXVersion.V142,
					featuredPriority: 1,
					isFeatured: true,
					associatedFiles: new [] {
						@"\WinForms\CS\ApplicationUIMainDemo\Modules\DocumentManager\ucDocumentManagerWindowsUISearchPanel.cs",
						@"\WinForms\VB\ApplicationUIMainDemo\Modules\DocumentManager\ucDocumentManagerWindowsUISearchPanel.vb"
					}
				),
				new SimpleModule(demo,
					name: "DocumentManagerNoDocumentsMode",
					displayName: @"Non-Document Mode",
					group: "Multiple Documents UI",
					type: "DevExpress.ApplicationUI.Demos.DocumentManagerNoDocumentsMode",
					description: @"Normally, a Document Manager is used to manage multiple Documents. If you do not need to create multiple documents, you can still use the DocumentManager in Non-Document Mode to enhance your DockPanels with the certain features (unified dock hints, document selector, etc.).",
					addedIn: KnownDXVersion.Before142, 
					associatedFiles: new [] {
						@"\WinForms\CS\ApplicationUIMainDemo\Modules\DocumentManager\DocumentManagerNonMdiMode.cs",
						@"\WinForms\VB\ApplicationUIMainDemo\Modules\DocumentManager\DocumentManagerNonMdiMode.vb"
					}
				),
				new SimpleModule(demo,
					name: "WidgetView",
					displayName: @"Stock Price Widget View",
					group: "Widgets UI",
					type: "DevExpress.ApplicationUI.Demos.WidgetView",
					description: @"The DevExpress Document Manager for WinForms ships with a Widget View, allowing you to create dashboard-like applications with ease. Each card is a document and contains its own unique content. You can resize these widgets, maximize them via a double-click or double tap and re-arrange them by dragging widgets and their parent groups (in Stacked Layout mode). If the 'Colored Widgets' menu item is pressed, widgets are painted using custom colors.",
					addedIn: KnownDXVersion.Before142,
					associatedFiles: new [] {
						@"\WinForms\CS\ApplicationUIMainDemo\Modules\WidgetView\WidgetView.cs",
						@"\WinForms\VB\ApplicationUIMainDemo\Modules\WidgetView\WidgetView.vb",
						@"\WinForms\CS\ApplicationUIMainDemo\Modules\WidgetView\WidgetControl.cs",
						@"\WinForms\VB\ApplicationUIMainDemo\Modules\WidgetView\WidgetControl.vb"
					}
				),
				new SimpleModule(demo,
					name: "SalesPerformanceWidgets",
					displayName: @"Sales Performance Widget View",
					group: "Widgets UI",
					type: "DevExpress.ApplicationUI.Demos.SalesPerformanceWidgets",
					description: @"This demo illustrates the DevExpress WinForms Widget View and uses the table layout view mode. In this mode, the View is divided into rows and columns with absolute or relative lengths. These rows and columns build a table with cells, in which Widget Documents are organized. Documents themselves can be stretched across multiple neighboring columns or rows. Dragging a Document activates jitter animation, which helps to highlight all Documents that can be swapped with the current item. Both columns and rows can resized when the parent form resizes and allows you to create screen resolution independent user experiences.",
					addedIn: KnownDXVersion.Before142,
					isFeatured: true,
					associatedFiles: new [] {
						@"\WinForms\CS\ApplicationUIMainDemo\Modules\WidgetView\SalesPerformanceWidgets.cs",
						@"\WinForms\VB\ApplicationUIMainDemo\Modules\WidgetView\SalesPerformanceWidgets.vb"
					}
				),
				new SimpleModule(demo,
					name: "DashboardWidgets",
					displayName: @"Dashboard Widget View",
					group: "Widgets UI",
					type: "DevExpress.ApplicationUI.Demos.DashboardWidgets",
					description: @"The Widget View is part of the DevExpress Document Manager and was designed to give you a rich set of UI options so you can deliver experiences that are easy-to-use, easy-to-understand and ready for next generation touch enabled Windows devices. Each card is a document and contains its own unique content. You can resize these widgets, re-arrange them on screen or maximize them via a double-click or double-tap. In this demo, we illustrate multiple use case scenarios, including the display of date/time information, a to-do list, a calendar, weather information and a news ticker.",
					addedIn: KnownDXVersion.Before142,
					updatedIn: KnownDXVersion.V142,
					featuredPriority: 0,
					newUpdatedPriority: 0,
					isFeatured: true,
					associatedFiles: new [] {
						@"\WinForms\CS\ApplicationUIMainDemo\Modules\WidgetView\DashboardWidgets.cs",
						@"\WinForms\VB\ApplicationUIMainDemo\Modules\WidgetView\DashboardWidgets.vb"
					}
				),
				new SimpleModule(demo,
					name: "IDEWorkspaces",
					displayName: @"Workspace Manager",
					group: "Workspace Manager",
					type: "DevExpress.ApplicationUI.Demos.IDEWorkspaces",
					description: @"This sample application uses multiple DevExpress controls (Data Grid, Document Manager, Dock Manger etc.) and features the Workspace Manager component - designed to control the global layout of controls within an application simultaneously. The 'Wokspaces' menu item allows you to save and load a workspace or choose from one of three predefined workspace layout options. By using the property grid on the right, you can modify the Workspace Manager's settings (modify animation type and more).",
					addedIn: KnownDXVersion.V142,
					featuredPriority: 2,
					newUpdatedPriority: 1,
					isFeatured: true,
					associatedFiles: new [] {
						@"\WinForms\CS\ApplicationUIMainDemo\Modules\WorkspaceManager\IDEWorkspaces.cs",
						@"\WinForms\VB\ApplicationUIMainDemo\Modules\WorkspaceManager\IDEWorkspaces.vb"
					}
				),
				new SimpleModule(demo,
					name: "DockPanels",
					displayName: @"Dock Panels",
					group: "Docking UI",
					type: "DevExpress.ApplicationUI.Demos.DockPanels",
					description: @"This demo shows how to create dock panels and dock them to the desired edge of the container control (form). It also demonstrates how to enable the Auto-Hide feature of dock panels.",
					addedIn: KnownDXVersion.Before142,
					associatedFiles: new [] {
						@"\WinForms\CS\ApplicationUIMainDemo\Modules\DockPanels\DockPanels.cs",
						@"\WinForms\VB\ApplicationUIMainDemo\Modules\DockPanels\DockPanels.vb"
					}
				),
				new SimpleModule(demo,
					name: "CustomHeaderButtons",
					displayName: @"Custom Header Buttons",
					group: "Docking UI",
					type: "DevExpress.ApplicationUI.Demos.CustomHeaderButtons",
					description: @"This demo illustrates how to use different types of custom header buttons within a DockPanel. You can choose a custom button's type (Push or Check) and its icon (a static image or unique glyph for each of the button's states) via the radio group at the top of the DockPanel. The properties grid to the right of the panel provides options for the custom button's detailed tweaking.",
					addedIn: KnownDXVersion.Before142,
					associatedFiles: new [] {
						@"\WinForms\CS\ApplicationUIMainDemo\Modules\CustomHeaderButtons\CustomHeaderButtons.cs",
						@"\WinForms\VB\ApplicationUIMainDemo\Modules\CustomHeaderButtons\CustomHeaderButtons.vb",
						@"\WinForms\CS\ApplicationUIMainDemo\Modules\CustomHeaderButtons\ActionDockPanel.cs",
						@"\WinForms\VB\ApplicationUIMainDemo\Modules\CustomHeaderButtons\ActionDockPanel.vb"
					}
				),
				new SimpleModule(demo,
					name: "TransitionManager",
					displayName: @"Transition Manager",
					group: "Multi-Purpose Components",
					type: "DevExpress.ApplicationUI.Demos.ModuleTransitionManager",
					description: @"This demo illustrates the Transition Manager - the component that applies animated transitions to the target control. In this demo, the Transition Manager animates the PictureEdit control when it changes its currently displayed image. Transition effects, such as the type of the transition or the availability of the wait indicator, can be customized using the 'Settings' tab. The 'Images' tab contains image thumbnails. Clicking these, you can modify the currently displayed content using currently selected animation options.",
					addedIn: KnownDXVersion.Before142, 
					updatedIn: KnownDXVersion.V152,
					featuredPriority: 2,
					newUpdatedPriority: 1,
					isFeatured: true,
					associatedFiles: new [] {
						@"\WinForms\CS\ApplicationUIMainDemo\Modules\ModuleTransitionManager.cs",
						@"\WinForms\VB\ApplicationUIMainDemo\Modules\ModuleTransitionManager.vb"
					}
				),
				new SimpleModule(demo,
					name: "AlertControlTutorial",
					displayName: @"Alert Control",
					group: "Multi-Purpose Components",
					type: "DevExpress.ApplicationUI.Demos.AlertControlTutorial",
					description: @"This demo demonstrates the AlertControl component that allows notification windows to be displayed. Click the 'Show Alert Form' button to display a notification window. It will be displayed at the bottom right corner of the screen. However, you can change the form's position as well as many other options using the controls provided by the demo.",
					addedIn: KnownDXVersion.Before142,
					associatedFiles: new [] {
						@"\WinForms\CS\ApplicationUIMainDemo\Modules\AlertControl\AlertControlTutorial.cs",
						@"\WinForms\VB\ApplicationUIMainDemo\Modules\AlertControl\AlertControlTutorial.vb"
					}
				),
				new SimpleModule(demo,
					name: "RadialMenu",
					displayName: @"Radial Menu",
					group: "Multi-Purpose Components",
					type: "DevExpress.ApplicationUI.Demos.RadialMenu",
					description: @"This demo demonstrates the RadialMenu in action. Use the Ribbon controls above to change various RadialMenu options.",
					addedIn: KnownDXVersion.Before142,
					associatedFiles: new [] {
						@"\WinForms\CS\ApplicationUIMainDemo\Modules\RadialMenu.cs",
						@"\WinForms\VB\ApplicationUIMainDemo\Modules\RadialMenu.vb"
					}
				),
				new SimpleModule(demo,
					name: "PopupControlContainer",
					displayName: @"Popup Container",
					group: "Multi-Purpose Components",
					type: "DevExpress.ApplicationUI.Demos.PopupControlContainer",
					description: @"Links that correspond to Bar Button Items can function as a dropdown. This allows the dropdown control to popup when such a link is pressed. In this tutorial, the dropdown control which allows you to specify the label's alignment is shown when the 'Text Alignment' link's dropdown button is pressed.",
					addedIn: KnownDXVersion.Before142,
					associatedFiles: new [] {
						@"\WinForms\CS\ApplicationUIMainDemo\Modules\PopupControlContainer.cs",
						@"\WinForms\VB\ApplicationUIMainDemo\Modules\PopupControlContainer.vb"
					}
				),
				new SimpleModule(demo,
					name: "ModuleTaskbarAssistant",
					displayName: @"Taskbar Assistant",
					group: "Multi-Purpose Components",
					type: "DevExpress.ApplicationUI.Demos.ModuleTaskbarAssistant",
					description: @"The DevExpress WinForms Taskbar Assistant simplifies customization of an application’s taskbar button, the live preview window (displayed when hovering the button) and allows you to create jump lists (invoked when right-clicking the taskbar button). In this demo, you can apply thumbnail button’s to the preview window, display an overlay image on the taskbar, specify progress animation, create both task and custom categories. Give it a try and see how the taskbar button for this application is automatically updated once you make a selection within the demo.",
					addedIn: KnownDXVersion.Before142,
					associatedFiles: new [] {
						@"\WinForms\CS\ApplicationUIMainDemo\Modules\ModuleTaskbarAssistant.cs",
						@"\WinForms\VB\ApplicationUIMainDemo\Modules\ModuleTaskbarAssistant.vb"
					}
				),
				new SimpleModule(demo,
					name: "ModuleFlyoutPanel",
					displayName: @"Flyout Panel",
					group: "Multi-Purpose Components",
					type: "DevExpress.ApplicationUI.Demos.ModuleFlyoutPanel",
					description: @"This Windows 8 inspired control allows you to create panels and display them on-screen using one of two animation effects. The Flyout Panel can be displayed at a specified location, including the top, bottom, right, left and center of the screen. You can also position the Flyout relative to other controls on your form. In this demo, you can specify the anchor position of the Flyout Panel, the animation effect to use when it is displayed and whether to close the panel if a click event occurs outside the panel’s boundaries.",
					addedIn: KnownDXVersion.Before142,
					associatedFiles: new [] {
						@"\WinForms\CS\ApplicationUIMainDemo\Modules\ModuleFlyoutPanel.cs",
						@"\WinForms\VB\ApplicationUIMainDemo\Modules\ModuleFlyoutPanel.vb"
					}
				),
				new SimpleModule(demo,
					name: "ModuleAdornerUIManager",
					displayName: @"Adorner UI Manager",
					group: "Multi-Purpose Components",
					type: "DevExpress.ApplicationUI.Demos.ModuleAdornerUIManager",
					description: @"This demo illustrates the AdornerUIManager, that allows you to create a custom adorner layer to draw badges above UI elements and create other visual effects.",
					addedIn: KnownDXVersion.V151,
					associatedFiles: new [] {
						@"\WinForms\CS\ApplicationUIMainDemo\Modules\ModuleAdornerUIManager.cs",
						@"\WinForms\VB\ApplicationUIMainDemo\Modules\ModuleAdornerUIManager.vb"
					}
				),
				new SimpleModule(demo,
					name: "ModuleTileNavPane",
					displayName: @"Tile Navigation",
					group: "Navigation UI",
					type: "DevExpress.ApplicationUI.Demos.ModuleTileNavPane",
					description: @"The TileNavPane control allows you to implement the app's hierarchical navigation menu consisting of up to three levels of hierarchy. Menu elements are rendered as tiles and are accessible via dropdowns. End-users can keep track of their locations within the hierarchy with the built-in breadcrumbs.",
					addedIn: KnownDXVersion.Before142,
					associatedFiles: new [] {
						@"\WinForms\CS\ApplicationUIMainDemo\Modules\ModuleTileNavPane.cs",
						@"\WinForms\VB\ApplicationUIMainDemo\Modules\ModuleTileNavPane.vb"
					}
				),
				new SimpleModule(demo,
					name: "TabbedMDIStart",
					displayName: @"Tabbed MDI Manager",
					group: "MDI",
					type: "DevExpress.ApplicationUI.Demos.TabbedMDIStart",
					description: @"This demo demonstrates the tabbed MDI manager which provides centralized control over the MDI child forms that are parented to the form. When using the tabbed MDI manager, child forms are represented by tabbed pages. Click the 'Launch Sample' button to run the tutorial.",
					addedIn: KnownDXVersion.Before142,
					associatedFiles: new [] {
						@"\WinForms\CS\ApplicationUIMainDemo\Modules\TabbedMDI.cs",
						@"\WinForms\VB\ApplicationUIMainDemo\Modules\TabbedMDI.vb"
					}
				)
			};
		}
	}
}
