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
		static List<Module> Create_DXWindowsUI_MainDemo_Modules(Demo demo) {
			return new List<Module> {
				new WpfModule(demo,
					name: "Windows8StyleMessageBox",
					displayName: @"Windows 8-style Message Box",
					group: "Demos",
					type: "WindowsUIDemo.Windows8StyleMessageBox",
					shortDescription: @"This example demonstrates the Windows 8 UI-style Message Box.",
					description: @"
                        <Paragraph>
                        This example demonstrates the Windows 8 UI-style Message Box. This Message Box features customizable float mode, allows you to display any text and images as content and embed various buttons into the dialog.
                        </Paragraph>",
					addedIn: KnownDXVersion.Before142
				),
				new WpfModule(demo,
					name: "NotificationsModule",
					displayName: @"Notifications",
					group: "Demos",
					type: "WindowsUIDemo.Notifications",
					shortDescription: @"This demo demonstrates the use of the DevExpress NotificationService to display Windows 8 style toast notifications.",
					description: @"
                        <Paragraph>
                        On Windows 8, the NotificationService uses the native mechanism to display toast notifications (Native notifications can be displaying above Windows Store apps).
                        On older versions of Windows, the NotificationService emulates Windows 8 style toast notifications. These notifications are only supported in desktop mode.
                        </Paragraph>
                        <Paragraph>
                        You can choose from four predefined notification templates or create a custom template if additional customization is required. Custom templates are supported in desktop mode only.
                        </Paragraph>",
					addedIn: KnownDXVersion.Before142,
					allowRtl: false,
					allowSwitchingThemes: false
				),
				new WpfModule(demo,
					name: "AppBarCommands",
					displayName: @"App Bar Commands",
					group: "Demos",
					type: "WindowsUIDemo.AppBarCommands",
					shortDescription: @"This example demonstrates an App Bar control displaying contextual commands to users on demand in the Windows 8 UI style.",
					description: @"
                        <Paragraph>
                        This example demonstrates an App Bar control displaying contextual commands to users on demand in the Windows 8 UI style.
                        You can right-click the application canvas to display or hide the App Bar. In the example, App Bar buttons are bound to commands that perform image-related operations.
                        <LineBreak/>
                        Flyout controls can be assigned to App Bar buttons. In the example, a MenuFlyout control is assigned to the Rotate button.
                        </Paragraph>",
					addedIn: KnownDXVersion.Before142,
					updatedIn: KnownDXVersion.V142
				),
				new WpfModule(demo,
					name: "FrameNavigation",
					displayName: @"Frame Navigation",
					group: "Demos",
					type: "WindowsUIDemo.FrameNavigation",
					shortDescription: @"This demo shows how to implement a Windows UI-style navigation between various views.",
					description: @"
                        <Paragraph>
                        This demo shows how to implement a Windows UI-style navigation between various views. The views are displayed using an animation effect. The AnimationType and AnimationSelector properties are used to apply predefined animation effects and custom animation respectively. All controls are placed within the NavigationFrame, which maintains the history of displayed views and provides built-in support for backward navigation. The PageAdornerControl is used to add a Back button to the application. In this demo, the PageAdornerControl also contains links for direct navigation to all available views.
                        </Paragraph>",
					addedIn: KnownDXVersion.Before142,
					updatedIn: KnownDXVersion.V142
				),
				new WpfModule(demo,
					name: "PageViewItemsSource",
					displayName: @"Page View",
					group: "Demos",
					type: "WindowsUIDemo.PageViewItemsSource",
					shortDescription: @"The Page View control allows you to present items as pages with page headers displayed at the top, left, right or bottom edge.",
					description: @"
                        <Paragraph>
                        The Page View control allows you to present items as pages with page headers displayed at the top, left, right or bottom edge. By default, page headers are clipped if they do not fit into the Page View width. To scroll through page headers, you can enable scroll buttons using the PageHeadersLayoutType property. In this demo, the Page View, which supports the MVVM design pattern, is populated with items from a bound collection. The Page View supports built-in animation when switching between items.
                        </Paragraph>",
					addedIn: KnownDXVersion.Before142,
					updatedIn: KnownDXVersion.V142
				),
				new WpfModule(demo,
					name: "SlideViewItemsSource",
					displayName: @"Slide View",
					group: "Demos",
					type: "WindowsUIDemo.SlideViewItemsSource",
					shortDescription: @"The Slide View control presents items as a horizontally scrollable list.",
					description: @"
                        <Paragraph>
                        The Slide View presents items as a horizontally scrollable list. In this demo, the Slide View, which supports the MVVM design pattern, is populated with items from a bound collection. Take note of the navigation to a detail view performed when clicking a person's name. Backward navigation is implemented with the help of the NavigationFrame object.
                        </Paragraph>",
					addedIn: KnownDXVersion.Before142
				),
				new WpfModule(demo,
					name: "FlipViewItemsSource",
					displayName: @"Flip View",
					group: "Demos",
					type: "WindowsUIDemo.FlipViewItemsSource",
					shortDescription: @"The Flip View control displays one item at a time, and enables ""flip"" behavior for traversing its collection of items.",
					description: @"
                        <Paragraph>
                        The Flip View control displays one item at a time, and enables a  ""flip"" behavior for traversing its collection of items. The control supports the MVVM design pattern, and here, it is populated with items from a bound collection.
                        </Paragraph>",
					addedIn: KnownDXVersion.Before142
				)
			};
		}
	}
}
