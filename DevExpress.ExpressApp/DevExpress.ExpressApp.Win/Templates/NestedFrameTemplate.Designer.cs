#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       eXpressApp Framework                                        }
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
namespace DevExpress.ExpressApp.Win.Templates {
#pragma warning disable 0618
	partial class NestedFrameTemplate {
#pragma warning restore 0618
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if(disposing && (components != null)) {
				modelSynchronizationManager.Dispose();
				actionContainersManager.Dispose();
				if(barManager != null) {
					cObjectsCreation = null;
					cRecordEdit = null;
					cWorkflow = null;
					cView = null;
					cEdit = null;
					cLink = null;
					cReports = null;
					cSave = null;
					cOpenObject = null;
					cPrint = null;
					viewSitePanel.Dispose();
					standardToolBar.Dispose();
					standardToolBar = null;
					barManager.Form = null;
					barManager.Dispose();
					barManager = null;
				}
			}
			base.Dispose(disposing);
		}
		#region Component Designer generated code
		private void InitializeComponent() {
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new XafComponentResourceManager(typeof(NestedFrameTemplate));
			this.viewSitePanel = new DevExpress.XtraEditors.PanelControl();
			this.cObjectsCreation = new DevExpress.ExpressApp.Win.Templates.ActionContainers.ActionContainerBarItem();
			this.cRecordEdit = new DevExpress.ExpressApp.Win.Templates.ActionContainers.ActionContainerBarItem();
			this.cWorkflow = new DevExpress.ExpressApp.Win.Templates.ActionContainers.ActionContainerBarItem();
			this.cEdit = new DevExpress.ExpressApp.Win.Templates.ActionContainers.ActionContainerBarItem();
			this.cLink = new DevExpress.ExpressApp.Win.Templates.ActionContainers.ActionContainerBarItem();
			this.cReports = new DevExpress.ExpressApp.Win.Templates.ActionContainers.ActionContainerBarItem();
			this.cSave = new DevExpress.ExpressApp.Win.Templates.ActionContainers.ActionContainerBarItem();
			this.cOpenObject = new DevExpress.ExpressApp.Win.Templates.ActionContainers.ActionContainerBarItem();
			this.cView = new DevExpress.ExpressApp.Win.Templates.ActionContainers.ActionContainerBarItem();
			this.cDefault = new DevExpress.ExpressApp.Win.Templates.ActionContainers.ActionContainerBarItem();
			this.cFilters = new DevExpress.ExpressApp.Win.Templates.ActionContainers.ActionContainerBarItem();
			this.cExport = new DevExpress.ExpressApp.Win.Templates.ActionContainers.ActionContainerBarItem();
			this.cDiagnostic = new DevExpress.ExpressApp.Win.Templates.ActionContainers.ActionContainerBarItem();
			this.cMenu = new DevExpress.ExpressApp.Win.Templates.ActionContainers.ActionContainerBarItem();
			this.cRecordsNavigation = new DevExpress.ExpressApp.Win.Templates.ActionContainers.ActionContainerBarItem();
			this.cPrint = new DevExpress.ExpressApp.Win.Templates.ActionContainers.ActionContainerBarItem();
			this.standardToolBar = new DevExpress.XtraBars.Bar();
			this.barManager = new DevExpress.ExpressApp.Win.Templates.Controls.XafBarManager(this.components);
			this.barDockControlTop = new DevExpress.XtraBars.BarDockControl();
			this.barDockControlBottom = new DevExpress.XtraBars.BarDockControl();
			this.barDockControlLeft = new DevExpress.XtraBars.BarDockControl();
			this.barDockControlRight = new DevExpress.XtraBars.BarDockControl();
			this.actionContainersManager = new DevExpress.ExpressApp.Win.Templates.ActionContainersManager(this.components);
			this.modelSynchronizationManager = new DevExpress.ExpressApp.Win.Templates.ModelSynchronizationManager(this.components);
			this.viewSiteManager = new DevExpress.ExpressApp.Win.Templates.ViewSiteManager(this.components);
			((System.ComponentModel.ISupportInitialize)(this.viewSitePanel)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.barManager)).BeginInit();
			this.SuspendLayout();
			resources.ApplyResources(this.viewSitePanel, "viewSitePanel");
			this.viewSitePanel.Name = "viewSitePanel";
			resources.ApplyResources(this.cObjectsCreation, "cObjectsCreation");
			this.cObjectsCreation.ContainerId = "ObjectsCreation";
			this.cObjectsCreation.Id = 0;
			this.cObjectsCreation.Name = "cObjectsCreation";
			this.cObjectsCreation.TargetPageCategoryColor = System.Drawing.Color.Empty;
			this.cObjectsCreation.TargetPageGroupCaption = null;
			resources.ApplyResources(this.cRecordEdit, "cRecordEdit");
			this.cRecordEdit.ContainerId = "RecordEdit";
			this.cRecordEdit.Id = 1;
			this.cRecordEdit.Name = "cRecordEdit";
			this.cRecordEdit.TargetPageCategoryColor = System.Drawing.Color.Empty;
			this.cRecordEdit.TargetPageGroupCaption = null;
			resources.ApplyResources(this.cWorkflow, "cWorkflow");
			this.cWorkflow.ContainerId = "Workflow";
			this.cWorkflow.Id = 1;
			this.cWorkflow.Name = "cWorkflow";
			this.cWorkflow.TargetPageCategoryColor = System.Drawing.Color.Empty;
			this.cWorkflow.TargetPageGroupCaption = null;
			resources.ApplyResources(this.cEdit, "cEdit");
			this.cEdit.ContainerId = "Edit";
			this.cEdit.Id = 1;
			this.cEdit.Name = "cEdit";
			this.cEdit.TargetPageCategoryColor = System.Drawing.Color.Empty;
			this.cEdit.TargetPageGroupCaption = null;
			resources.ApplyResources(this.cLink, "cLink");
			this.cLink.ContainerId = "Link";
			this.cLink.Id = 1;
			this.cLink.Name = "cLink";
			this.cLink.TargetPageCategoryColor = System.Drawing.Color.Empty;
			this.cLink.TargetPageGroupCaption = null;
			resources.ApplyResources(this.cReports, "cReports");
			this.cReports.ContainerId = "Reports";
			this.cReports.Id = 1;
			this.cReports.Name = "cReports";
			this.cReports.TargetPageCategoryColor = System.Drawing.Color.Empty;
			this.cReports.TargetPageGroupCaption = null;
			resources.ApplyResources(this.cSave, "cSave");
			this.cSave.ContainerId = "Save";
			this.cSave.Id = 1;
			this.cSave.Name = "cSave";
			this.cSave.TargetPageCategoryColor = System.Drawing.Color.Empty;
			this.cSave.TargetPageGroupCaption = null;
			resources.ApplyResources(this.cOpenObject, "cOpenObject");
			this.cOpenObject.ContainerId = "OpenObject";
			this.cOpenObject.Id = 8;
			this.cOpenObject.Name = "cOpenObject";
			this.cOpenObject.TargetPageCategoryColor = System.Drawing.Color.Empty;
			this.cOpenObject.TargetPageGroupCaption = null;
			resources.ApplyResources(this.cView, "cView");
			this.cView.ContainerId = "View";
			this.cView.Id = 2;
			this.cView.Name = "cView";
			this.cView.TargetPageCategoryColor = System.Drawing.Color.Empty;
			this.cView.TargetPageGroupCaption = null;
			resources.ApplyResources(this.cDefault, "cDefault");
			this.cDefault.ContainerId = "Unspecified";
			this.cDefault.Id = 3;
			this.cDefault.Name = "cDefault";
			this.cDefault.TargetPageCategoryColor = System.Drawing.Color.Empty;
			this.cDefault.TargetPageGroupCaption = null;
			resources.ApplyResources(this.cFilters, "cFilters");
			this.cFilters.ContainerId = "Filters";
			this.cFilters.Id = 4;
			this.cFilters.Name = "cFilters";
			this.cFilters.TargetPageCategoryColor = System.Drawing.Color.Empty;
			this.cFilters.TargetPageGroupCaption = null;
			resources.ApplyResources(this.cExport, "cExport");
			this.cExport.ContainerId = "Export";
			this.cExport.Id = 5;
			this.cExport.Name = "cExport";
			this.cExport.TargetPageCategoryColor = System.Drawing.Color.Empty;
			this.cExport.TargetPageGroupCaption = null;
			resources.ApplyResources(this.cDiagnostic, "cDiagnostic");
			this.cDiagnostic.ContainerId = "Diagnostic";
			this.cDiagnostic.Id = 6;
			this.cDiagnostic.Name = "cDiagnostic";
			this.cDiagnostic.TargetPageCategoryColor = System.Drawing.Color.Empty;
			this.cDiagnostic.TargetPageGroupCaption = null;
			resources.ApplyResources(this.cMenu, "cMenu");
			this.cMenu.ContainerId = "Menu";
			this.cMenu.Id = 7;
			this.cMenu.Name = "cMenu";
			this.cMenu.TargetPageCategoryColor = System.Drawing.Color.Empty;
			this.cMenu.TargetPageGroupCaption = null;
			resources.ApplyResources(this.cRecordsNavigation, "cRecordsNavigation");
			this.cRecordsNavigation.ContainerId = "RecordsNavigation";
			this.cRecordsNavigation.Id = 10;
			this.cRecordsNavigation.Name = "cRecordsNavigation";
			this.cRecordsNavigation.TargetPageCategoryColor = System.Drawing.Color.Empty;
			this.cRecordsNavigation.TargetPageGroupCaption = null;
			resources.ApplyResources(this.cPrint, "cPrint");
			this.cPrint.ContainerId = "Print";
			this.cPrint.Id = 7;
			this.cPrint.Name = "cPrint";
			this.cPrint.TargetPageCategoryColor = System.Drawing.Color.Empty;
			this.cPrint.TargetPageGroupCaption = null;
			this.standardToolBar.BarName = "ListView Toolbar";
			this.standardToolBar.DockCol = 0;
			this.standardToolBar.DockRow = 0;
			this.standardToolBar.DockStyle = DevExpress.XtraBars.BarDockStyle.Top;
			this.standardToolBar.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] {
			new DevExpress.XtraBars.LinkPersistInfo(this.cObjectsCreation, true),
			new DevExpress.XtraBars.LinkPersistInfo(this.cSave, true),
			new DevExpress.XtraBars.LinkPersistInfo(this.cLink, true),
			new DevExpress.XtraBars.LinkPersistInfo(this.cEdit, true),
			new DevExpress.XtraBars.LinkPersistInfo(this.cRecordEdit, true),
			new DevExpress.XtraBars.LinkPersistInfo(this.cOpenObject, true),
			new DevExpress.XtraBars.LinkPersistInfo(this.cWorkflow, true),			
			new DevExpress.XtraBars.LinkPersistInfo(this.cView, true),
			new DevExpress.XtraBars.LinkPersistInfo(this.cReports, true),
			new DevExpress.XtraBars.LinkPersistInfo(this.cDiagnostic, true),
			new DevExpress.XtraBars.LinkPersistInfo(this.cRecordsNavigation, true),
			new DevExpress.XtraBars.LinkPersistInfo(this.cDefault, true),
			new DevExpress.XtraBars.LinkPersistInfo(this.cFilters, true),
			new DevExpress.XtraBars.LinkPersistInfo(this.cExport, true),
			new DevExpress.XtraBars.LinkPersistInfo(this.cPrint, true)});
			resources.ApplyResources(this.standardToolBar, "standardToolBar");
			this.barManager.HideBarsWhenMerging = false;
			this.barManager.Bars.AddRange(new DevExpress.XtraBars.Bar[] {
			this.standardToolBar});
			this.barManager.DockControls.Add(this.barDockControlTop);
			this.barManager.DockControls.Add(this.barDockControlBottom);
			this.barManager.DockControls.Add(this.barDockControlLeft);
			this.barManager.DockControls.Add(this.barDockControlRight);
			this.barManager.Form = this;
			this.barManager.Items.AddRange(new DevExpress.XtraBars.BarItem[] {
			this.cObjectsCreation,
			this.cEdit,
			this.cLink,
			this.cReports,
			this.cSave,
			this.cOpenObject,
			this.cRecordEdit,
			this.cWorkflow,
			this.cView,
			this.cDefault,
			this.cFilters,
			this.cExport,
			this.cMenu,
			this.cRecordsNavigation,
			this.cPrint,
			this.cDiagnostic});
			this.barManager.MaxItemId = 7;
			resources.ApplyResources(this.barDockControlTop, "barDockControlTop");
			this.barDockControlTop.SizeChanged += new EventHandler(barDockControl_SizeChanged);
			resources.ApplyResources(this.barDockControlBottom, "barDockControlBottom");
			this.barDockControlBottom.SizeChanged += new EventHandler(barDockControl_SizeChanged);
			resources.ApplyResources(this.barDockControlLeft, "barDockControlLeft");
			this.barDockControlLeft.SizeChanged += new EventHandler(barDockControl_SizeChanged);
			resources.ApplyResources(this.barDockControlRight, "barDockControlRight");
			this.barDockControlRight.SizeChanged += new EventHandler(barDockControl_SizeChanged);
			this.actionContainersManager.ActionContainerComponents.Add(this.cObjectsCreation);
			this.actionContainersManager.ActionContainerComponents.Add(this.cRecordEdit);
			this.actionContainersManager.ActionContainerComponents.Add(this.cWorkflow);			
			this.actionContainersManager.ActionContainerComponents.Add(this.cEdit);
			this.actionContainersManager.ActionContainerComponents.Add(this.cLink);
			this.actionContainersManager.ActionContainerComponents.Add(this.cReports);
			this.actionContainersManager.ActionContainerComponents.Add(this.cSave);
			this.actionContainersManager.ActionContainerComponents.Add(this.cOpenObject);
			this.actionContainersManager.ActionContainerComponents.Add(this.cView);
			this.actionContainersManager.ActionContainerComponents.Add(this.cDefault);
			this.actionContainersManager.ActionContainerComponents.Add(this.cFilters);
			this.actionContainersManager.ActionContainerComponents.Add(this.cExport);
			this.actionContainersManager.ActionContainerComponents.Add(this.cDiagnostic);
			this.actionContainersManager.ActionContainerComponents.Add(this.cMenu);
			this.actionContainersManager.ActionContainerComponents.Add(this.cRecordsNavigation);
			this.actionContainersManager.ActionContainerComponents.Add(this.cPrint);
			this.actionContainersManager.ContextMenuContainers.Add(this.cObjectsCreation);
			this.actionContainersManager.ContextMenuContainers.Add(this.cRecordEdit);
			this.actionContainersManager.ContextMenuContainers.Add(this.cEdit);
			this.actionContainersManager.ContextMenuContainers.Add(this.cLink);
			this.actionContainersManager.ContextMenuContainers.Add(this.cReports);
			this.actionContainersManager.ContextMenuContainers.Add(this.cSave);
			this.actionContainersManager.ContextMenuContainers.Add(this.cOpenObject);
			this.actionContainersManager.ContextMenuContainers.Add(this.cView);
			this.actionContainersManager.ContextMenuContainers.Add(this.cFilters);
			this.actionContainersManager.ContextMenuContainers.Add(this.cExport);
			this.actionContainersManager.ContextMenuContainers.Add(this.cPrint);
			this.actionContainersManager.ContextMenuContainers.Add(this.cMenu);
			this.actionContainersManager.DefaultContainer = this.cDefault;
			this.actionContainersManager.Template = this;
			this.modelSynchronizationManager.ModelSynchronizableComponents.Add(this.barManager);
			this.viewSiteManager.UseDeferredLoading = true;
			this.viewSiteManager.ViewSiteControl = this.viewSitePanel;
			this.Controls.Add(this.viewSitePanel);
			this.Controls.Add(this.barDockControlLeft);
			this.Controls.Add(this.barDockControlRight);
			this.Controls.Add(this.barDockControlBottom);
			this.Controls.Add(this.barDockControlTop);
			this.Name = "NestedFrameTemplate";
			resources.ApplyResources(this, "$this");
			((System.ComponentModel.ISupportInitialize)(this.viewSitePanel)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.barManager)).EndInit();
			this.ResumeLayout(false);
		}
		private DevExpress.XtraBars.Bar standardToolBar;
		private DevExpress.ExpressApp.Win.Templates.ActionContainers.ActionContainerBarItem cObjectsCreation;
		private DevExpress.ExpressApp.Win.Templates.ActionContainers.ActionContainerBarItem cRecordEdit;
		private DevExpress.ExpressApp.Win.Templates.ActionContainers.ActionContainerBarItem cWorkflow;
		private DevExpress.ExpressApp.Win.Templates.ActionContainers.ActionContainerBarItem cEdit;
		private DevExpress.ExpressApp.Win.Templates.ActionContainers.ActionContainerBarItem cLink;
		private DevExpress.ExpressApp.Win.Templates.ActionContainers.ActionContainerBarItem cReports;
		private DevExpress.ExpressApp.Win.Templates.ActionContainers.ActionContainerBarItem cSave;
		private DevExpress.ExpressApp.Win.Templates.ActionContainers.ActionContainerBarItem cOpenObject;
		private DevExpress.ExpressApp.Win.Templates.ActionContainers.ActionContainerBarItem cView;
		private DevExpress.ExpressApp.Win.Templates.ActionContainers.ActionContainerBarItem cDefault;
		private DevExpress.ExpressApp.Win.Templates.ActionContainers.ActionContainerBarItem cFilters;
		private DevExpress.ExpressApp.Win.Templates.ActionContainers.ActionContainerBarItem cExport;
		private DevExpress.ExpressApp.Win.Templates.ActionContainers.ActionContainerBarItem cDiagnostic;
		private DevExpress.ExpressApp.Win.Templates.ActionContainers.ActionContainerBarItem cMenu;
		private DevExpress.ExpressApp.Win.Templates.ActionContainers.ActionContainerBarItem cRecordsNavigation;
		private DevExpress.ExpressApp.Win.Templates.ActionContainers.ActionContainerBarItem cPrint;
		private DevExpress.XtraBars.BarDockControl barDockControlTop;
		private DevExpress.XtraBars.BarDockControl barDockControlBottom;
		private DevExpress.XtraBars.BarDockControl barDockControlLeft;
		private DevExpress.XtraBars.BarDockControl barDockControlRight;
		private DevExpress.ExpressApp.Win.Templates.ModelSynchronizationManager modelSynchronizationManager;
		#endregion
		protected DevExpress.XtraEditors.PanelControl viewSitePanel;
		protected DevExpress.ExpressApp.Win.Templates.Controls.XafBarManager barManager;
		private DevExpress.ExpressApp.Win.Templates.ActionContainersManager actionContainersManager;
		private DevExpress.ExpressApp.Win.Templates.ViewSiteManager viewSiteManager;
	}
}
