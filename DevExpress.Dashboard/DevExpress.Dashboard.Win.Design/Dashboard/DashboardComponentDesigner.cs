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

using DevExpress.DashboardCommon;
using DevExpress.DashboardCommon.Native;
using DevExpress.DashboardWin.Localization;
using DevExpress.DashboardWin.ServiceModel;
using DevExpress.Data.Entity;
using DevExpress.Data.Utils;
using DevExpress.DataAccess;
using DevExpress.DataAccess.Design;
using DevExpress.DataAccess.Native;
using DevExpress.DataAccess.Sql;
using DevExpress.DataAccess.Wizard.Services;
using DevExpress.Design.VSIntegration;
using DevExpress.Entity.ProjectModel;
using DevExpress.Utils;
using DevExpress.Utils.Design;
using DevExpress.Utils.Drawing;
using DevExpress.XtraBars;
using DevExpress.XtraDashboardLayout;
using DevExpress.XtraLayout;
using DevExpress.XtraLayout.Customization;
using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Drawing.Design;
using System.Linq;
using System.Windows.Forms;
namespace DevExpress.DashboardWin.Design {
	[
	ToolboxItemFilter(AssemblyInfo.SRAssemblyDashboardWin, ToolboxItemFilterType.Require)
	]
	public class DashboardComponentDesigner : ComponentDesigner, IToolShellProvider, IRootDesigner, IToolboxUser {
		const int SelectionBorderSize = 3;
		static ToolShellController toolShellController;
		static int dashboardCount = 0;
		static DashboardComponentDesigner() {
			SqlDataSource.DisableCustomQueryValidation = true;
		}
		static void OnDispose(IToolShell tool) {
			dashboardCount--;
			if(dashboardCount == 0) {
				tool.Close();
			}
		}
		BarManager barManager;
		DashboardDesigner designer;
		LayoutControl layoutControl;
		ServicesList servicesList;
		VSUIContextDebug uiContextDebug;
		IComponentChangeService changeService;
		IDesignerHost designerHost;
		IToolShell toolShell;
		bool activating;
		BarManager BarManager {
			get {
				if(barManager == null) {
					BarAndDockingController controller = new BarAndDockingController();
					controller.LookAndFeel.ParentLookAndFeel = Designer.LookAndFeel;
					barManager = new BarManager() { Controller = controller, Form = LayoutControl };
				}
				return barManager;
			}
		}
		public DashboardDesigner Designer { 
			get {
				if(designer == null) {
					designer = new DashboardDesigner(null);
					designer.CreateBars(false, BarManager);
				}
				return designer;
			} 
		}
		public LayoutControl LayoutControl {
			get {
				if(layoutControl == null) {
					layoutControl = Designer.Viewer.LayoutControl;
				}
				return layoutControl;
			}
		}
		IToolShell IToolShellProvider.ToolShell {
			get { return toolShell; }
		}
		Dashboard Dashboard { get { return (Dashboard)Component; } }
		public DashboardComponentDesigner() {
			dashboardCount++;
		}
		public override void Initialize(IComponent component) {
			base.Initialize(component);			
			changeService = (IComponentChangeService)GetService(typeof(IComponentChangeService));
			designerHost = (IDesignerHost)GetService(typeof(IDesignerHost));
			if (designerHost != null) {
				designerHost.Activated += OnActivate;
			}
			servicesList = new ServicesList(designerHost);
			Designer.ForceDashboardDesignTime(designerHost, new VSConnectionStringsService(designerHost), 
				new VSConnectionStorageService(designerHost), new VSSolutionTypesProvider(), 
				new ConnectionStringsService(designerHost), new SupportedDataSourceTypesServiceDesignTime(designerHost),
				GetService(typeof(IDTEService)) as IDTEService, new VSCustomQueryValidator()
				);
			Designer.Dashboard = Dashboard;
			VSLookAndFeelService.SetControlLookAndFeel(designerHost, designer, true);
			BitmapStorage.Initialize(designerHost);
			LayoutControl.AllowDrop = true;
			uiContextDebug = new VSUIContextDebug(designerHost);
			uiContextDebug.ActiveChanged += OnUIContextDebugActiveChanged;
			LockDesigner(uiContextDebug.IsActive);
			InitToolShell();
		}
		public bool GetToolSupported(ToolboxItem tool) {
			return true;
		}
		public void ToolPicked(ToolboxItem toolboxItem) {
			ToolPicked(toolboxItem, null);
		}
		public void ToolPicked(ToolboxItem toolboxItem, DashboardLayoutControlDragDropHelper dragHelper) {
			if(toolboxItem == null) {
				if(dragHelper != null)
					dragHelper.DoDragCancel();
				return;
			}
			DashboardItemComponent dashboardItemComponent = null;
			try {
				dashboardItemComponent = toolboxItem.CreateComponents()[0] as DashboardItemComponent;
			}
			catch {
			}
			DashboardItem dashboardItem = dashboardItemComponent != null ? dashboardItemComponent.CreateDashboardItem() : null;
			if(dashboardItem != null) {
				string transactionName = String.Format(DashboardWinLocalizer.GetString(DashboardWinStringId.HistoryItemInsertItem), dashboardItemComponent.GetType().Name);
				DesignerTransaction transaction = designerHost.CreateTransaction(transactionName);
				Dashboard.BeginUpdate();
				try {
					dashboardItem.ComponentName = Helper.CreateDashboardComponentName(Dashboard, dashboardItem.GetType());
					designerHost.Container.Add(dashboardItem, dashboardItem.ComponentName);
					PropertyDescriptor collectionPropertyDescriptor = null;
					DashboardItemGroup dashboardGroup = dashboardItem as DashboardItemGroup;
					if (dashboardGroup != null)
						collectionPropertyDescriptor = Helper.GetProperty(Dashboard, "Groups");
					else
						collectionPropertyDescriptor = Helper.GetProperty(Dashboard, "Items");
					changeService.OnComponentChanging(Dashboard, collectionPropertyDescriptor);
					if(dashboardGroup != null)
						Dashboard.Groups.Add(dashboardGroup);
					else
						Dashboard.Items.Add(dashboardItem);
					changeService.OnComponentChanged(Dashboard, collectionPropertyDescriptor, null, null);
					if(dragHelper != null) { 
						LayoutItemDragController dragController = dragHelper.GetDragController();
						if(dragController != null) {
							IDashboardLayoutUpdateService layoutUpdateService = (IDashboardLayoutUpdateService)GetService(typeof(IDashboardLayoutUpdateService));
							if(layoutUpdateService != null) {
								layoutUpdateService.BeginUpdate();
								layoutUpdateService.LockLayoutControlUpdate();
								try {
									layoutUpdateService.CreateLayoutItem(dashboardItem.ComponentName, Helper.GetDashboardItemType(dashboardItem), dragController);
								} finally {
									layoutUpdateService.UnlockLayoutControlUpdate(false);
									layoutUpdateService.EndUpdate();
								}
							}
							dragHelper.DoDragCancel(); 
						}
					} else { 
						Size clientSize = Designer.Viewer.GetClientSize();
						PropertyDescriptor layoutPropertyDescriptor =  Helper.GetProperty(Dashboard, "Layout");
						changeService.OnComponentChanging(Dashboard, layoutPropertyDescriptor);
						Dashboard.RebuildLayout(clientSize.Width, clientSize.Height);
						changeService.OnComponentChanged(Dashboard, layoutPropertyDescriptor, null, null);
					}
				} finally {
					Dashboard.EndUpdate();
					transaction.Commit();
				}
			}
		}
		protected override object GetService(Type serviceType) {
			if (designer != null) {
				object service = ((IServiceProvider)designer).GetService(serviceType);
				if (service != null)
					return service;
			}
			return base.GetService(serviceType);
		}
		protected override void Dispose(bool disposing) {
			if (disposing) {
				servicesList.Dispose();				
				uiContextDebug.ActiveChanged -= OnUIContextDebugActiveChanged;
				uiContextDebug.Dispose();
				uiContextDebug = null;
				if (designerHost != null) {
					designerHost.Activated -= OnActivate;
				}
				if (designer != null) {
					designer.Dispose();
					designer = null;
				}
				if (barManager != null) {
					barManager.Dispose();
					barManager = null;
				}
				OnDispose(toolShell);
				if (toolShell != null) {
					toolShell.Dispose();
					toolShell = null;
				}
			}
			base.Dispose(disposing);
		}	  
		void OnUIContextDebugActiveChanged(object sender, ActiveChangedEventArgs e) {
			LockDesigner(e.Active);
		}
		void InitToolShell() {
			DashboardMenu dashboardMenu = new DashboardMenu(this.designerHost);
			this.toolShell = new ToolShell();
			this.toolShell.AddToolItem(dashboardMenu);
			this.toolShell.AddToolItem(new DashboardVSFieldList(this.designerHost, "Field List"));
			this.toolShell.InitToolItems();
			dashboardMenu.AddItem(new EditDataSourcesMenuItem(this.designerHost));
			dashboardMenu.AddItem(new LoadDashboardMenuItem(this.designerHost));
			dashboardMenu.AddItem(new SaveDashboardAsMenuItem(this.designerHost));			
		}
		void OnActivate(object sender, EventArgs e) {
			IToolboxService toolboxService = GetService(typeof(IToolboxService)) as IToolboxService;
			if(toolboxService != null) {
				try {
					toolboxService.SelectedCategory = AssemblyInfo.DXTabDashboardItems;
				} catch { 
				}
			}
			if(!activating) {
				activating = true;
				try {
					if(toolShellController == null)
						toolShellController = new ToolShellController(designerHost);
				}
				finally {
					activating = false;
				}
			}
		}
		void LockDesigner(bool value) {
			Designer.Enabled = !value;
		}
		#region IRootDesigner members
		ViewTechnology[] IRootDesigner.SupportedTechnologies { get { return new ViewTechnology[] { ControlConstants.ViewTechnologyDefault }; } }
		object IRootDesigner.GetView(ViewTechnology technology) {
			if (technology == ControlConstants.ViewTechnologyDefault)
				return Designer;
			return null;
		}
		#endregion
	}
}
