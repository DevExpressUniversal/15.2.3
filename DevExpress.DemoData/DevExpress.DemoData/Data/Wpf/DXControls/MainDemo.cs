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
		#region TabControl Demos
		const string TabControl_Description_SelectedTabBackgroundColor = @"<Paragraph>
<Span FontWeight=""Bold"">'Selected Tab Background Color'</Span> - specifies the Selected Tab's background.
</Paragraph>";
		const string TabControl_Description_SelectedTabBorderColor = @"<Paragraph>
<Span FontWeight=""Bold"">'Selected Tab Border Color'</Span> - specifies the Selected Tab's border color.
</Paragraph>";
		const string TabControl_Description_HeaderLocation = @"<Paragraph>
<Span FontWeight=""Bold"">'Header Location'</Span> - specifies the Tab Header Panel's location.
</Paragraph>";
		const string TabControl_Description_View = @"<Paragraph>
<Span FontWeight=""Bold"">'View'</Span> - specifies the Tab Control's View.
</Paragraph>";
		const string TabControl_Description_NewButton = @"<Paragraph>
<Span FontWeight=""Bold"">'New Button'</Span> - specifies the Tab Control New Button's location.
</Paragraph>";
		const string TabControl_Description_HideButton = @"<Paragraph>
<Span FontWeight=""Bold"">'Hide Button'</Span> - specifies the Tab Control Hide Button's location and its working logic.
</Paragraph>";
		const string TabControl_Description_ShowHeaderMenu = @"<Paragraph>
The header menu's availability is controlled by the view's <Span FontWeight=""Bold"">ShowHeaderMenu</Span> property. In this demo, this is controlled by the <Span FontWeight=""Bold"">'Show Header Menu'</Span> checkbox.
</Paragraph>";
		const string TabControl_Description_ShowVisibleTabsInHeaderMenu = @"<Paragraph>
<Span FontWeight=""Bold"">'Show Visible Tabs In Header Menu'</Span> - specifies whether to include visible tab items in the Header Menu.
</Paragraph>";
		const string TabControl_Description_ShowHiddenTabsInHeaderMenu = @"<Paragraph>
<Span FontWeight=""Bold"">'Show Hidden Tabs In Header Menu'</Span> - specifies whether to include hidden tab items in the Header Menu.
</Paragraph>";
		static WpfModule CreateTabControlWebBrowserDemo(Demo demo) {
			return new WpfModule(demo,
				name: "ModuleWebBrowserView",
				displayName: @"Web Browser Inspired Tabs",
				group: "TabControl Demos",
				type: "ControlsDemo.TabControl.WebBrowser.MainView",
				shortDescription: @"This demo shows how to use the DXTabControl component to display tabs in the Web Browser style.",
				description: @"<Paragraph>
This demo uses the <Span FontWeight=""Bold"">TabControlStretchView</Span> component that shows tabs in the Web Browser style. All tabs can be reordered and moved into a separate window.
</Paragraph>",
				addedIn: KnownDXVersion.V151,
				isMvvm: true,
				isFeatured: true);
		}
		static WpfModule CreateTabControlColorizedTabsDemo(Demo demo) {
			return new WpfModule(demo,
				name: "ModuleColorizedTabs",
				displayName: @"Colorized Tabs",
				group: "TabControl Demos",
				type: "ControlsDemo.TabControl.ColorizedTabs.MainView",
				shortDescription: @"This demo shows how to customize colors in the DXTabControl component.",
				description: @"<Paragraph>
This demo uses the <Span FontWeight=""Bold"">DXTabItem.BackgroundColor</Span> and <Span FontWeight=""Bold"">DXTabItem.BorderColor</Span> properties to show colorized tabs.
</Paragraph>" + 
				TabControl_Description_SelectedTabBackgroundColor + 
				TabControl_Description_SelectedTabBorderColor +
				TabControl_Description_HeaderLocation,
				addedIn: KnownDXVersion.V151,
				isMvvm: true);
		}
		static WpfModule CreateTabControlCustomThemeDemo(Demo demo) {
			return new WpfModule(demo,
				name: "ModuleCustomTheme",
				displayName: @"Custom Theme",
				group: "TabControl Demos",
				type: "ControlsDemo.TabControl.CustomTheme.MainView",
				shortDescription: @"This demo shows how to create a custom appearance for the DXTabControl component.",
				description: @"<Paragraph>
This demo includes several templates that are used to display tabs in different states.
</Paragraph>" + TabControl_Description_HeaderLocation,
				addedIn: KnownDXVersion.V151,
				isMvvm: true,
				allowSwitchingThemes: false);
		}
		static WpfModule CreateTabControlViewsDemo(Demo demo) {
			return new WpfModule(demo,
				name: "Views",
				displayName: @"Views",
				group: "TabControl Demos",
				type: "ControlsDemo.TabControl.Views.MainView",
				shortDescription: @"This demo shows different Views of the DXTabControl component.",
				description: @"<Paragraph>
In the <Span FontWeight=""Bold"">Multi Line View</Span>, if the number of tab headers is too large to fit onto a single line, the headers are arranged in multiple lines. As a result, all tab headers are displayed on the screen.
</Paragraph>
<Paragraph>
In the <Span FontWeight=""Bold"">Scroll View</Span>, tab headers are displayed one after another in a single line. If the number of the tab headers is too large to fit into the Tab Header Panel, the <Span FontWeight=""Bold"">TabControl</Span> displays scroll buttons. These buttons allow you to scroll tab items.
</Paragraph>
<Paragraph>
<Span FontWeight=""Bold"">Stretch View</Span> arranges tab headers in a single line. This view provides the Drag and Drop functionality. 
</Paragraph>",
				isMvvm: true,
				addedIn: KnownDXVersion.V151, 
				updatedIn: KnownDXVersion.V152);
		}
		static WpfModule CreateTabControlPinnedTabsViewDemo(Demo demo) {
			return new WpfModule(demo,
				name: "PinnedTabs",
				displayName: @"Pinned Tabs",
				group: "TabControl Demos",
				type: "ControlsDemo.TabControl.PinnedTabs.MainView",
				shortDescription: @"This demo shows how to use the Pinned Tabs feature.",
				description: @"<Paragraph>
This demo uses the <Span FontWeight=""Bold"">TabControlStretchView</Span> component that can show pinned tabs.
</Paragraph>" + TabControl_Description_HeaderLocation,
				isMvvm: true,
				addedIn: KnownDXVersion.V152);
		}
		#endregion TabControl Demos
		static List<Module> Create_DXControls_MainDemo_Modules(Demo demo) {
			return new List<Module> {
				new WpfModule(demo,
					name: "TaskbarServices",
					displayName: @"Taskbar Services",
					group: "Controls",
					type: "ControlsDemo.TaskbarServices",
					shortDescription: "This example demonstrates the TaskbarButtonService and ApplicationJumpListService, which provides capabilities for the application's taskbar button,\r\nJump List and thumbnail preview customization.",
					description: @"
                        <Paragraph>
                        The <Span FontWeight=""Bold"">TaskbarButtonService</Span> provides methods to customize the application taskbar button and thumbnail
                        preview window. You can enable a progress indicator that allows an end-user to be aware of the progress of a custom operation performed by the application.
                        </Paragraph>
                        <Paragraph>
                        The <Span FontWeight=""Bold"">ApplicationJumpListService</Span> allows you to add tasks to the Application Jump List and enable the Frequent and Recent Jump List categories.
                        </Paragraph>",
					allowDarkThemes: false,
					allowSwitchingThemes: false,
					allowRtl: false,
					addedIn: KnownDXVersion.Before142
				),
				new WpfModule(demo,
					name: "TileNavPaneModule",
					displayName: @"TileNav Pane",
					group: "Controls",
					type: "ControlsDemo.TileNavPaneModule",
					shortDescription: @"The TileNavPane control is a touch-friendly version of traditional navigation elements used within Windows desktop apps.",
					updatedIn:KnownDXVersion.V152,
					description: @"
                        <Paragraph>
                        Designed to be positioned at the top of your application window (like a Ribbon), the TileNavPane can be thought of as a touch-friendly version of traditional navigation elements used within Windows desktop apps. In this demo, the TileNavPane control is populated with items from a bound collection. Take note of the navigation to a detail view performed when clicking any item in the TileNavPane. Forward and backward navigation is implemented with the help of the NavigationFrame control.
                        </Paragraph>",
					addedIn: KnownDXVersion.Before142
				),
				new WpfModule(demo,
					name: "TileBarModule",
					displayName: @"Tile Bar",
					group: "Controls",
					type: "ControlsDemo.TileBarModule",
					shortDescription: @"Implement a tile-based menu for your app using the TileBar control.",					
					description: @"
                        <Paragraph>
                        Navigate between app screens by clicking menu elements that are rendered as tiles. The application is hierarchically structured into categories and category items. The latter are accessible via dropdowns. Each screen has a 'Back' button on the top left that allows you to navigate back to the root screen.                            
                        </Paragraph>",
					addedIn: KnownDXVersion.V152
				),
				new WpfModule(demo,
					name: "BookCalendar",
					displayName: @"Book Calendar",
					group: "Controls",
					type: "ControlsDemo.BookCalendar",
					shortDescription: @"This demo shows how to use the Book control.",
					description: @"
                        <Paragraph>
                        This demo shows how to use the Book control to create an appointment desk calendar.
                        </Paragraph>",
					allowSwitchingThemes: false,
					addedIn: KnownDXVersion.Before142,
					isFeatured: true
				),
				new WpfModule(demo,
					name: "BookEmployees",
					displayName: @"Book Employees",
					group: "Controls",
					type: "ControlsDemo.BookEmployees",
					shortDescription: @"This demo shows how to use our Book control to present data as a book.",
					description: @"
                        <Paragraph>
                        This demo shows how to use our Book control to present data as a book. In this demo, data is obtained from the North Wind database. Records are represented as pages in a book. To navigate between records, turn over pages as you do when reading a book.
                        </Paragraph>",
					allowDarkThemes: false,
					allowSwitchingThemes: false,
					addedIn: KnownDXVersion.Before142
				),
				CreateTabControlWebBrowserDemo(demo),
				CreateTabControlColorizedTabsDemo(demo),
				CreateTabControlCustomThemeDemo(demo),
				CreateTabControlPinnedTabsViewDemo(demo),
				CreateTabControlViewsDemo(demo),
				new WpfModule(demo,
					name: "GalleryPhotoViewer",
					displayName: @"Gallery Photo Viewer",
					group: "GalleryControl Demos",
					type: "ControlsDemo.GalleryPhotoViewer",
					shortDescription: @"This application demonstrates the Gallery Control - shipped as part of the DXBars Suite.",
					description: @"
                        <Paragraph>
                        This example shows how to implement a Photo Viewer application using <Span FontWeight=""Bold"">Gallery Control</Span>.
                        </Paragraph>
                        <Paragraph>
                        In this example, <Span FontWeight=""Bold"">Gallery Control</Span> is used to present a set of images and categorize them in groups. By clicking any image, you can maximize and see it in full size.
                        </Paragraph>",
					allowTouchThemes: false,
					addedIn: KnownDXVersion.Before142,
					isFeatured: true
				)
			};
		}
	}
}
