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
using DevExpress.Data.Utils;
using DevExpress.Design.VSIntegration;
using DevExpress.LookAndFeel;
namespace DevExpress.DashboardWin.Design {
	public abstract class VSToolWindowItemBase : IToolItem {
		IServiceProvider serviceProvider;
		IVSToolWindowService toolWindowService;
		string caption;
		string bitmapResourceName;
		Guid kind;
		protected VSToolWindowItemBase(IServiceProvider serviceProvider, string caption, string bitmapResourceName, Guid kind) {
			this.caption = caption;
			this.serviceProvider = serviceProvider;
			this.bitmapResourceName = bitmapResourceName;
			this.toolWindowService = (IVSToolWindowService)serviceProvider.GetService(typeof(IVSToolWindowService));
			this.kind = kind;
		}
		public Guid Kind {
			get { return kind; }
		}
		protected abstract Panel Panel { get; set; }
		protected abstract Control Control { get; }
		protected IServiceProvider ServiceProvider { get { return serviceProvider; } }
		protected IVSToolWindowService ToolWindowService { get { return toolWindowService; } }
		protected string Caption { get { return caption; } }
		protected string BitmapResourceName { get { return bitmapResourceName; } }
		protected Control HostedControl {
			get { return Panel; }
			set { Panel = (Panel)value; }
		}
		public void InitTool() {
			CreateHostedControl();
			if(ToolWindow is VSDummyToolWindow)
				ToolWindow = CreateToolWindow();
			if(ToolWindow is VSToolWindow) {
				VSMenuService menuService = GetMenuService();
				if(menuService != null) {
					VSMenuItem menuItem = CreateMenuItem();
					menuService.RegisterMenuItem(menuItem);
				}
			}
			ToolWindow.HostControl(HostedControl);
		}
		public abstract void UpdateView();
		protected abstract IVSToolWindow ToolWindow { get; set; }
		protected abstract Guid ToolWindowGuid { get; }
		protected abstract VSMenuService GetMenuService();
		protected abstract void CreateControl(IServiceProvider serviceProvider);
		protected abstract VSMenuItem CreateMenuItem();
		protected virtual IVSToolWindow CreateToolWindow() {
			return toolWindowService == null ? null : toolWindowService.Create(serviceProvider, caption, ToolWindowGuid);
		}
		protected abstract void SetLookAndFeel(IServiceProvider serviceProvider);
		protected virtual void CreateHostedControl() {
			if(Panel == null) {
				Panel = new Panel();
				if(Control == null || Control.IsDisposed) {
					CreateControl(serviceProvider);
					SetLookAndFeel(serviceProvider);
				}
				Panel.Controls.Add(Control);
				Control.Dock = DockStyle.Fill;
			}
			UpdateView();
		}
		public void Dispose() {
		}
		public void Hide() {
			ToolWindow.Hide(serviceProvider);
		}
		public void Close() {
			ToolWindow.CloseFrame(serviceProvider);
			HostedControl.Dispose();
			HostedControl = null;
		}
		public void ShowNoActivate() {
			ToolWindow.ShowNoActivate(serviceProvider);
		}
		public void ShowActivate() {
			VSToolWindow toolWindow = ToolWindow as VSToolWindow;
			if(toolWindow != null) {
				toolWindow.ShowPersistently();
			}
		}
	}
}
