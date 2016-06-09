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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using DevExpress.Mvvm.UI;
using DevExpress.Xpf.Bars;
using DevExpress.Xpf.Core.Native;
namespace DevExpress.Xpf.Core.Design.Services {
	public static class BehaviorInfoHelper {
		const string IconSuffix = "16x16.png";
		static Dictionary<string, string> toolTip = new Dictionary<string, string>();
		static BehaviorInfoHelper() {
			toolTip.Add(typeof(CurrentWindowService).FullName, "Provides a method to close a window.");
			toolTip.Add(typeof(DialogService).FullName, "A service that is used to show dialogs on the client.");
			toolTip.Add(typeof(DXMessageBoxService).FullName, "Allows you to display message boxes.");
			toolTip.Add(typeof(DXSplashScreenService).FullName, "Allows you to display splash screens.");
			toolTip.Add(GetFullName(XmlNamespaceConstants.WindowsUINavigationNamespace, "FrameNavigationService"), "The service that provides methods to navigate between Views within a NavigationFrame.");
			toolTip.Add(typeof(EventToCommand).FullName, "A trigger that invokes a command when a specific event is fired.");
			toolTip.Add(GetFullName(XmlNamespaceConstants.WindowsUINavigationNamespace, "FrameDocumentUIService"), "Implements the Windows 8-style navigation between different modules in your application.");
			toolTip.Add(typeof(NotificationService).FullName, "Provides methods to show toast notifications.");
			toolTip.Add(GetFullName(XmlNamespaceConstants.DockingNamespace, "TabbedDocumentUIService"), "Provides methods to create documents as tabs.");
			toolTip.Add(typeof(WindowedDocumentUIService).FullName, "Provides methods to create documents as windows.");
			toolTip.Add(GetFullName(XmlNamespaceConstants.WindowsUINamespace, "WinUIDialogService"), "Allows you to display Views as Windows 8-style dialogs.");
			toolTip.Add(GetFullName(XmlNamespaceConstants.WindowsUINamespace, "WinUIMessageBoxService"), "Allows you to display Windows 8-style message boxes.");
			toolTip.Add(typeof(ApplicationJumpListService).FullName, "Allows you to create and manage the Application Jump List.");
			toolTip.Add(typeof(TaskbarButtonService).FullName, "Allows you to manipulate the appearance of the application's taskbar button.");
			toolTip.Add(typeof(BarSplitItemThemeSelectorBehavior).FullName, "Creates a gallery with DevExpress themes in the item's popup and allows a user to select these themes at runtime.");
			toolTip.Add(typeof(BarSubItemThemeSelectorBehavior).FullName, "Creates a list of DevExpress themes in the menu and allows a user to apply these themes at runtime.");
			toolTip.Add(typeof(GalleryThemeSelectorBehavior).FullName, "Populates the gallery with DevExpress themes and allows a user to select these themes at runtime.");
			toolTip.Add(typeof(ConfirmationBehavior).FullName, "Allows you to show a confirmation message in response to user action.");
			toolTip.Add(GetFullName(XmlNamespaceConstants.RibbonNamespace, "RibbonGalleryItemThemeSelectorBehavior"), "Populates the item's gallery with DevExpress themes and allows a user to select these themes at runtime.");
			toolTip.Add(GetFullName(XmlNamespaceConstants.GridNamespace, "GridDragDropManager"), "Enables the Drag and Drop functionality in TableView.");
			toolTip.Add(GetFullName(XmlNamespaceConstants.GridNamespace, "TreeListDragDropManager"), "Enables the Drag and Drop functionality in TreeListView.");
			toolTip.Add(GetFullName(XmlNamespaceConstants.GridNamespace, "ListBoxDragDropManager"), "Enables the Drag and Drop functionality in ListBoxEdit.");
			toolTip.Add(GetFullName(XmlNamespaceConstants.ReportDesignerExtensionsNamespace, "GridReportManagerService"), "Allows generating reports using the Grid data source.");
			toolTip.Add(GetFullName(XmlNamespaceConstants.ReportDesignerExtensionsNamespace, "ReportManagerBehavior"), "Populates the button with actions for managing reports. Operates in conjunction with GridReportManagerService.");
		}
		static string GetFullName(string nameSpace, string typeName) {
			return string.Format("{0}.{1}", nameSpace, typeName);
		}
		public static string GetToolTip(Type type) {
			return GetToolTip(type.FullName);
		}
		public static string GetToolTip(string typeFullName) {
			if(String.IsNullOrEmpty(typeFullName))
				return null;
			return toolTip.ContainsKey(typeFullName) ? toolTip[typeFullName] : null;
		}
		static string[] resources;
		public static string[] Resources {
			get {
				if(resources == null)
					resources = GetResources();
				return resources;
			}
		}
		public static object GetIcon(string name) {
			Image img = null;
			string resourceName = string.Format("Images/SmartTag/Behaviors/{0}-{1}", name, IconSuffix).ToLower();
			var uri = new Uri(string.Format("pack://application:,,,/{0}.Design;component/{1}", AssemblyInfo.SRAssemblyXpfCore, resourceName), UriKind.RelativeOrAbsolute);
			if(Resources.Contains(resourceName)) {
				img = new Image() { Width = 16, Height = 16, Stretch = System.Windows.Media.Stretch.None };
				img.Source = new BitmapImage(uri);
			}
			return img;
		}
		public static string[] GetResources() {
			var assembly = Assembly.GetExecutingAssembly();
			string resourceName = assembly.GetName().Name + ".g.resources";
			using(var resourceStream = assembly.GetManifestResourceStream(resourceName)) {
				using(var resourceReader = new System.Resources.ResourceReader(resourceStream)) {
					return resourceReader.Cast<DictionaryEntry>().Select(entry =>
							 (string)entry.Key).ToArray();
				}
			}
		}
	}
}
