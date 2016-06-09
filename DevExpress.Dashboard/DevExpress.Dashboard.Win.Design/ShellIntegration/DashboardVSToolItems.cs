#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Dashboard                                                   }
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
using System.Windows.Forms;
using DevExpress.DashboardWin.Native;
using DevExpress.Design.VSIntegration;
namespace DevExpress.DashboardWin.Design {
	public static class DashboardToolItemKindHelper {
		public static Guid FieldList = new Guid("0FAA06DF-DA23-4D91-B392-5328E779D961");
		public static Guid Menu = new Guid("B0978112-D47F-44C9-A6CF-01D109E88995");
	}
	public class DashboardVSToolWindow : VSToolWindow {
		const string RegistryPath = @"Software\Developer Express\Dashboard\";
		protected override string RegistryKey {
			get { return RegistryPath + "ToolWindowsVisibility";; }
		}
		protected override Type ResFinderType {
			get { return typeof(DevExpress.DashboardWin.Design.ResFinder); }
		}
		public DashboardVSToolWindow(IServiceProvider servProvider, string caption, Guid toolWindowGuid, string bitmapResourceName) :
			base(servProvider, caption, toolWindowGuid, bitmapResourceName) {
		}
	}
	public abstract class DashboardVSToolWindowItem : VSToolWindowItemBase {
		protected DashboardVSToolWindowItem(IServiceProvider serviceProvider, string caption, string bitmapResourceName, Guid kind)
			: base(serviceProvider, caption, bitmapResourceName, kind) {
		}
		protected override VSMenuService GetMenuService() {
			return ServiceProvider.GetService(typeof(VSMenuService)) as VSMenuService;
		}
		protected override IVSToolWindow CreateToolWindow() {
			IVSToolWindow toolWindow = base.CreateToolWindow();
			if(toolWindow == null)
				toolWindow = new DashboardVSToolWindow(ServiceProvider, Caption, ToolWindowGuid, BitmapResourceName);
			return toolWindow;
		}
		protected override VSMenuItem CreateMenuItem() {
			return new DashboardVSMenuItem(Caption, BitmapResourceName, ToolWindow as DashboardVSToolWindow);
		}
		protected override void SetLookAndFeel(IServiceProvider serviceProvider) {
			VSLookAndFeelService.SetControlLookAndFeel(serviceProvider, Control, true);
		}
	}
	public class DashboardVSFieldList : DashboardVSToolWindowItem {
		static IVSToolWindow toolWindow = new VSDummyToolWindow();
		static Panel panel;
		static DataSourceBrowser browser;
		static DataSourceBrowserPresenter presenter;
		public DashboardVSFieldList(IServiceProvider serviceProvider, string caption)
			: base(serviceProvider, caption, BitmapStorage.GetResourceName("DashboardDesignFieldList"), DashboardToolItemKindHelper.FieldList) {
		}
		protected override Panel Panel { get { return panel; } set { panel = value; } }
		protected override Control Control { get { return browser; } }
		protected override IVSToolWindow ToolWindow {
			get { return toolWindow; } 
			set {
				if(value != null) {
					toolWindow = value;
				}
			}
		}
		protected override Guid ToolWindowGuid { get { return new Guid("2709096F-C324-42F5-A35D-AE8676D50918"); } }
		protected override void CreateControl(IServiceProvider serviceProvider) {
			browser = new DataSourceBrowser { AllowGlyphSkinning = !BitmapStorage.UseColors };
		}
		public override void UpdateView() {
			if (presenter != null)
				presenter.SetVew(null);
			SelectedContextService selectedContextService = ServiceProvider.GetService(typeof(SelectedContextService)) as SelectedContextService;
			presenter = selectedContextService.Designer.DataSourceBrowserPresenter;
			if (presenter != null)
				presenter.SetVew(browser);
		}
	}
}
