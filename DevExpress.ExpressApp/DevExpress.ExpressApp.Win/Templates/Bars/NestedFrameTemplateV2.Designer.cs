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

namespace DevExpress.ExpressApp.Win.Templates.Bars {
	partial class NestedFrameTemplateV2 {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if(disposing && (components != null)) {
				components.Dispose();
				this.barManager = null;
				this.barDockControlBottom = null;
				this.barDockControlLeft = null;
				this.barDockControlRight = null;
				this.barDockControlTop = null;
			}
			base.Dispose(disposing);
		}
		#region Component Designer generated code
		private void InitializeComponent() {
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new XafComponentResourceManager(typeof(DevExpress.ExpressApp.Win.Templates.Bars.NestedFrameTemplateV2));
			this.barManager = new DevExpress.ExpressApp.Win.Templates.Bars.XafBarManagerV2(this.components);
			this.actionContainerObjectsCreation = new DevExpress.ExpressApp.Win.Templates.Bars.ActionControls.BarLinkActionControlContainer();
			this.barContainerObjectsCreation = new DevExpress.XtraBars.BarLinkContainerExItem();
			this.actionContainerSave = new DevExpress.ExpressApp.Win.Templates.Bars.ActionControls.BarLinkActionControlContainer();
			this.barContainerSave = new DevExpress.XtraBars.BarLinkContainerExItem();
			this.actionContainerLink = new DevExpress.ExpressApp.Win.Templates.Bars.ActionControls.BarLinkActionControlContainer();
			this.barContainerLink = new DevExpress.XtraBars.BarLinkContainerExItem();
			this.actionContainerRecordEdit = new DevExpress.ExpressApp.Win.Templates.Bars.ActionControls.BarLinkActionControlContainer();
			this.barContainerRecordEdit = new DevExpress.XtraBars.BarLinkContainerExItem();
			this.actionContainerWorkflow = new DevExpress.ExpressApp.Win.Templates.Bars.ActionControls.BarLinkActionControlContainer();
			this.barContainerWorkflow = new DevExpress.XtraBars.BarLinkContainerExItem();
			this.actionContainerEdit = new DevExpress.ExpressApp.Win.Templates.Bars.ActionControls.BarLinkActionControlContainer();
			this.barContainerEdit = new DevExpress.XtraBars.BarLinkContainerExItem();
			this.actionContainerReports = new DevExpress.ExpressApp.Win.Templates.Bars.ActionControls.BarLinkActionControlContainer();
			this.barContainerReports = new DevExpress.XtraBars.BarLinkContainerExItem();
			this.actionContainerOpenObject = new DevExpress.ExpressApp.Win.Templates.Bars.ActionControls.BarLinkActionControlContainer();
			this.barContainerOpenObject = new DevExpress.XtraBars.BarLinkContainerExItem();
			this.actionContainerView = new DevExpress.ExpressApp.Win.Templates.Bars.ActionControls.BarLinkActionControlContainer();
			this.barContainerView = new DevExpress.XtraBars.BarLinkContainerExItem();
			this.actionContainerDefault = new DevExpress.ExpressApp.Win.Templates.Bars.ActionControls.BarLinkActionControlContainer();
			this.barContainerDefault = new DevExpress.XtraBars.BarLinkContainerExItem();
			this.actionContainerFilters = new DevExpress.ExpressApp.Win.Templates.Bars.ActionControls.BarLinkActionControlContainer();
			this.barContainerFilters = new DevExpress.XtraBars.BarLinkContainerExItem();
			this.actionContainerExport = new DevExpress.ExpressApp.Win.Templates.Bars.ActionControls.BarLinkActionControlContainer();
			this.barContainerExport = new DevExpress.XtraBars.BarLinkContainerExItem();
			this.actionContainerDiagnostic = new DevExpress.ExpressApp.Win.Templates.Bars.ActionControls.BarLinkActionControlContainer();
			this.barContainerDiagnostic = new DevExpress.XtraBars.BarLinkContainerExItem();
			this.actionContainerMenu = new DevExpress.ExpressApp.Win.Templates.Bars.ActionControls.BarLinkActionControlContainer();
			this.barContainerMenu = new DevExpress.XtraBars.BarLinkContainerExItem();
			this.actionContainerRecordsNavigation = new DevExpress.ExpressApp.Win.Templates.Bars.ActionControls.BarLinkActionControlContainer();
			this.barContainerRecordsNavigation = new DevExpress.XtraBars.BarLinkContainerExItem();
			this.actionContainerPrint = new DevExpress.ExpressApp.Win.Templates.Bars.ActionControls.BarLinkActionControlContainer();
			this.barContainerPrint = new DevExpress.XtraBars.BarLinkContainerExItem();
			this.standardToolBar = new DevExpress.XtraBars.Bar();
			this.barDockControlTop = new DevExpress.XtraBars.BarDockControl();
			this.barDockControlBottom = new DevExpress.XtraBars.BarDockControl();
			this.barDockControlLeft = new DevExpress.XtraBars.BarDockControl();
			this.barDockControlRight = new DevExpress.XtraBars.BarDockControl();
			this.contextMenu = new DevExpress.XtraBars.PopupMenu(this.components);
			this.viewSitePanel = new DevExpress.XtraEditors.PanelControl();
			this.viewSiteManager = new DevExpress.ExpressApp.Win.Templates.ViewSiteManager(this.components);
			((System.ComponentModel.ISupportInitialize)(this.barManager)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.actionContainerObjectsCreation)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.actionContainerSave)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.actionContainerLink)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.actionContainerRecordEdit)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.actionContainerWorkflow)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.actionContainerEdit)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.actionContainerReports)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.actionContainerOpenObject)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.actionContainerView)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.actionContainerDefault)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.actionContainerFilters)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.actionContainerExport)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.actionContainerDiagnostic)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.actionContainerMenu)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.actionContainerRecordsNavigation)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.actionContainerPrint)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.contextMenu)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.viewSitePanel)).BeginInit();
			this.SuspendLayout();
			this.barManager.ActionContainers.Add(this.actionContainerObjectsCreation);
			this.barManager.ActionContainers.Add(this.actionContainerSave);
			this.barManager.ActionContainers.Add(this.actionContainerLink);
			this.barManager.ActionContainers.Add(this.actionContainerRecordEdit);
			this.barManager.ActionContainers.Add(this.actionContainerWorkflow);
			this.barManager.ActionContainers.Add(this.actionContainerEdit);
			this.barManager.ActionContainers.Add(this.actionContainerReports);
			this.barManager.ActionContainers.Add(this.actionContainerOpenObject);
			this.barManager.ActionContainers.Add(this.actionContainerView);
			this.barManager.ActionContainers.Add(this.actionContainerDefault);
			this.barManager.ActionContainers.Add(this.actionContainerFilters);
			this.barManager.ActionContainers.Add(this.actionContainerExport);
			this.barManager.ActionContainers.Add(this.actionContainerDiagnostic);
			this.barManager.ActionContainers.Add(this.actionContainerMenu);
			this.barManager.ActionContainers.Add(this.actionContainerRecordsNavigation);
			this.barManager.ActionContainers.Add(this.actionContainerPrint);
			this.barManager.Bars.AddRange(new DevExpress.XtraBars.Bar[] {
			this.standardToolBar});
			this.barManager.DockControls.Add(this.barDockControlTop);
			this.barManager.DockControls.Add(this.barDockControlBottom);
			this.barManager.DockControls.Add(this.barDockControlLeft);
			this.barManager.DockControls.Add(this.barDockControlRight);
			this.barManager.Form = this;
			this.barManager.HideBarsWhenMerging = false;
			this.barManager.Items.AddRange(new DevExpress.XtraBars.BarItem[] {
			this.barContainerObjectsCreation,
			this.barContainerEdit,
			this.barContainerLink,
			this.barContainerReports,
			this.barContainerSave,
			this.barContainerOpenObject,
			this.barContainerRecordEdit,
			this.barContainerWorkflow,
			this.barContainerView,
			this.barContainerDefault,
			this.barContainerFilters,
			this.barContainerExport,
			this.barContainerMenu,
			this.barContainerRecordsNavigation,
			this.barContainerPrint,
			this.barContainerDiagnostic});
			this.barManager.MaxItemId = 15;
			this.actionContainerObjectsCreation.ActionCategory = "ObjectsCreation";
			this.actionContainerObjectsCreation.BarContainerItem = this.barContainerObjectsCreation;
			resources.ApplyResources(this.barContainerObjectsCreation, "barContainerObjectsCreation");
			this.barContainerObjectsCreation.Id = 0;
			this.barContainerObjectsCreation.MergeType = DevExpress.XtraBars.BarMenuMerge.MergeItems;
			this.barContainerObjectsCreation.Name = "barContainerObjectsCreation";
			this.actionContainerSave.ActionCategory = "Save";
			this.actionContainerSave.BarContainerItem = this.barContainerSave;
			resources.ApplyResources(this.barContainerSave, "barContainerSave");
			this.barContainerSave.Id = 1;
			this.barContainerSave.MergeType = DevExpress.XtraBars.BarMenuMerge.MergeItems;
			this.barContainerSave.Name = "barContainerSave";
			this.actionContainerLink.ActionCategory = "Link";
			this.actionContainerLink.BarContainerItem = this.barContainerLink;
			resources.ApplyResources(this.barContainerLink, "barContainerLink");
			this.barContainerLink.Id = 2;
			this.barContainerLink.MergeType = DevExpress.XtraBars.BarMenuMerge.MergeItems;
			this.barContainerLink.Name = "barContainerLink";
			this.actionContainerRecordEdit.ActionCategory = "RecordEdit";
			this.actionContainerRecordEdit.BarContainerItem = this.barContainerRecordEdit;
			resources.ApplyResources(this.barContainerRecordEdit, "barContainerRecordEdit");
			this.barContainerRecordEdit.Id = 3;
			this.barContainerRecordEdit.MergeType = DevExpress.XtraBars.BarMenuMerge.MergeItems;
			this.barContainerRecordEdit.Name = "barContainerRecordEdit";
			this.actionContainerWorkflow.ActionCategory = "Workflow";
			this.actionContainerWorkflow.BarContainerItem = this.barContainerWorkflow;
			resources.ApplyResources(this.barContainerWorkflow, "barContainerWorkflow");
			this.barContainerWorkflow.Id = 4;
			this.barContainerWorkflow.MergeType = DevExpress.XtraBars.BarMenuMerge.MergeItems;
			this.barContainerWorkflow.Name = "barContainerWorkflow";
			this.actionContainerEdit.ActionCategory = "Edit";
			this.actionContainerEdit.BarContainerItem = this.barContainerEdit;
			resources.ApplyResources(this.barContainerEdit, "barContainerEdit");
			this.barContainerEdit.Id = 5;
			this.barContainerEdit.MergeType = DevExpress.XtraBars.BarMenuMerge.MergeItems;
			this.barContainerEdit.Name = "barContainerEdit";
			this.actionContainerReports.ActionCategory = "Reports";
			this.actionContainerReports.BarContainerItem = this.barContainerReports;
			resources.ApplyResources(this.barContainerReports, "barContainerReports");
			this.barContainerReports.Id = 6;
			this.barContainerReports.MergeType = DevExpress.XtraBars.BarMenuMerge.MergeItems;
			this.barContainerReports.Name = "barContainerReports";
			this.actionContainerOpenObject.ActionCategory = "OpenObject";
			this.actionContainerOpenObject.BarContainerItem = this.barContainerOpenObject;
			resources.ApplyResources(this.barContainerOpenObject, "barContainerOpenObject");
			this.barContainerOpenObject.Id = 7;
			this.barContainerOpenObject.MergeType = DevExpress.XtraBars.BarMenuMerge.MergeItems;
			this.barContainerOpenObject.Name = "barContainerOpenObject";
			this.actionContainerView.ActionCategory = "View";
			this.actionContainerView.BarContainerItem = this.barContainerView;
			resources.ApplyResources(this.barContainerView, "barContainerView");
			this.barContainerView.Id = 8;
			this.barContainerView.MergeType = DevExpress.XtraBars.BarMenuMerge.MergeItems;
			this.barContainerView.Name = "barContainerView";
			this.actionContainerDefault.ActionCategory = "Default";
			this.actionContainerDefault.BarContainerItem = this.barContainerDefault;
			resources.ApplyResources(this.barContainerDefault, "barContainerDefault");
			this.barContainerDefault.Id = 9;
			this.barContainerDefault.MergeType = DevExpress.XtraBars.BarMenuMerge.MergeItems;
			this.barContainerDefault.Name = "barContainerDefault";
			this.actionContainerFilters.ActionCategory = "Filters";
			this.actionContainerFilters.BarContainerItem = this.barContainerFilters;
			resources.ApplyResources(this.barContainerFilters, "barContainerFilters");
			this.barContainerFilters.Id = 10;
			this.barContainerFilters.MergeType = DevExpress.XtraBars.BarMenuMerge.MergeItems;
			this.barContainerFilters.Name = "barContainerFilters";
			this.actionContainerExport.ActionCategory = "Export";
			this.actionContainerExport.BarContainerItem = this.barContainerExport;
			resources.ApplyResources(this.barContainerExport, "barContainerExport");
			this.barContainerExport.Id = 11;
			this.barContainerExport.MergeType = DevExpress.XtraBars.BarMenuMerge.MergeItems;
			this.barContainerExport.Name = "barContainerExport";
			this.actionContainerDiagnostic.ActionCategory = "Diagnostic";
			this.actionContainerDiagnostic.BarContainerItem = this.barContainerDiagnostic;
			this.actionContainerDiagnostic.IsMenuMode = true;
			resources.ApplyResources(this.barContainerDiagnostic, "barContainerDiagnostic");
			this.barContainerDiagnostic.Id = 12;
			this.barContainerDiagnostic.MergeType = DevExpress.XtraBars.BarMenuMerge.MergeItems;
			this.barContainerDiagnostic.Name = "barContainerDiagnostic";
			this.actionContainerMenu.ActionCategory = "Menu";
			this.actionContainerMenu.BarContainerItem = this.barContainerMenu;
			resources.ApplyResources(this.barContainerMenu, "barContainerMenu");
			this.barContainerMenu.Id = 13;
			this.barContainerMenu.Name = "barContainerMenu";
			this.actionContainerRecordsNavigation.ActionCategory = "RecordsNavigation";
			this.actionContainerRecordsNavigation.BarContainerItem = this.barContainerRecordsNavigation;
			resources.ApplyResources(this.barContainerRecordsNavigation, "barContainerRecordsNavigation");
			this.barContainerRecordsNavigation.Id = 14;
			this.barContainerRecordsNavigation.MergeType = DevExpress.XtraBars.BarMenuMerge.MergeItems;
			this.barContainerRecordsNavigation.Name = "barContainerRecordsNavigation";
			this.actionContainerPrint.ActionCategory = "Print";
			this.actionContainerPrint.BarContainerItem = this.barContainerPrint;
			resources.ApplyResources(this.barContainerPrint, "barContainerPrint");
			this.barContainerPrint.Id = 15;
			this.barContainerPrint.MergeType = DevExpress.XtraBars.BarMenuMerge.MergeItems;
			this.barContainerPrint.Name = "barContainerPrint";
			this.standardToolBar.BarName = "ListView Toolbar";
			this.standardToolBar.DockCol = 0;
			this.standardToolBar.DockRow = 0;
			this.standardToolBar.DockStyle = DevExpress.XtraBars.BarDockStyle.Top;
			this.standardToolBar.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] {
			new DevExpress.XtraBars.LinkPersistInfo(this.barContainerObjectsCreation, true),
			new DevExpress.XtraBars.LinkPersistInfo(this.barContainerSave, true),
			new DevExpress.XtraBars.LinkPersistInfo(this.barContainerLink, true),
			new DevExpress.XtraBars.LinkPersistInfo(this.barContainerEdit, true),
			new DevExpress.XtraBars.LinkPersistInfo(this.barContainerRecordEdit, true),
			new DevExpress.XtraBars.LinkPersistInfo(this.barContainerOpenObject, true),
			new DevExpress.XtraBars.LinkPersistInfo(this.barContainerWorkflow, true),
			new DevExpress.XtraBars.LinkPersistInfo(this.barContainerView, true),
			new DevExpress.XtraBars.LinkPersistInfo(this.barContainerReports, true),
			new DevExpress.XtraBars.LinkPersistInfo(this.barContainerDiagnostic, true),
			new DevExpress.XtraBars.LinkPersistInfo(this.barContainerRecordsNavigation, true),
			new DevExpress.XtraBars.LinkPersistInfo(this.barContainerDefault, true),
			new DevExpress.XtraBars.LinkPersistInfo(this.barContainerFilters, true),
			new DevExpress.XtraBars.LinkPersistInfo(this.barContainerExport, true),
			new DevExpress.XtraBars.LinkPersistInfo(this.barContainerPrint, true)});
			resources.ApplyResources(this.standardToolBar, "standardToolBar");
			this.barDockControlTop.CausesValidation = false;
			resources.ApplyResources(this.barDockControlTop, "barDockControlTop");
			this.barDockControlBottom.CausesValidation = false;
			resources.ApplyResources(this.barDockControlBottom, "barDockControlBottom");
			this.barDockControlLeft.CausesValidation = false;
			resources.ApplyResources(this.barDockControlLeft, "barDockControlLeft");
			this.barDockControlRight.CausesValidation = false;
			resources.ApplyResources(this.barDockControlRight, "barDockControlRight");
			this.contextMenu.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] {
			new DevExpress.XtraBars.LinkPersistInfo(this.barContainerObjectsCreation, true),
			new DevExpress.XtraBars.LinkPersistInfo(this.barContainerRecordEdit, true),
			new DevExpress.XtraBars.LinkPersistInfo(this.barContainerEdit, true),
			new DevExpress.XtraBars.LinkPersistInfo(this.barContainerLink, true),
			new DevExpress.XtraBars.LinkPersistInfo(this.barContainerReports, true),
			new DevExpress.XtraBars.LinkPersistInfo(this.barContainerSave, true),
			new DevExpress.XtraBars.LinkPersistInfo(this.barContainerOpenObject, true),
			new DevExpress.XtraBars.LinkPersistInfo(this.barContainerView, true),
			new DevExpress.XtraBars.LinkPersistInfo(this.barContainerFilters, true),
			new DevExpress.XtraBars.LinkPersistInfo(this.barContainerExport, true),
			new DevExpress.XtraBars.LinkPersistInfo(this.barContainerPrint, true),
			new DevExpress.XtraBars.LinkPersistInfo(this.barContainerMenu, true)});
			this.contextMenu.Manager = this.barManager;
			this.contextMenu.Name = "contextMenu";
			resources.ApplyResources(this.viewSitePanel, "viewSitePanel");
			this.viewSitePanel.Name = "viewSitePanel";
			this.viewSiteManager.UseDeferredLoading = true;
			this.viewSiteManager.ViewSiteControl = this.viewSitePanel;
			this.Controls.Add(this.viewSitePanel);
			this.Controls.Add(this.barDockControlLeft);
			this.Controls.Add(this.barDockControlRight);
			this.Controls.Add(this.barDockControlBottom);
			this.Controls.Add(this.barDockControlTop);
			this.Name = "NestedFrameTemplateV2";
			resources.ApplyResources(this, "$this");
			((System.ComponentModel.ISupportInitialize)(this.barManager)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.actionContainerObjectsCreation)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.actionContainerSave)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.actionContainerLink)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.actionContainerRecordEdit)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.actionContainerWorkflow)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.actionContainerEdit)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.actionContainerReports)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.actionContainerOpenObject)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.actionContainerView)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.actionContainerDefault)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.actionContainerFilters)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.actionContainerExport)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.actionContainerDiagnostic)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.actionContainerMenu)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.actionContainerRecordsNavigation)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.actionContainerPrint)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.contextMenu)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.viewSitePanel)).EndInit();
			this.ResumeLayout(false);
		}
		private DevExpress.ExpressApp.Win.Templates.Bars.XafBarManagerV2 barManager;
		private DevExpress.XtraBars.BarLinkContainerExItem barContainerObjectsCreation;
		private DevExpress.XtraBars.BarLinkContainerExItem barContainerSave;
		private DevExpress.XtraBars.BarLinkContainerExItem barContainerLink;
		private DevExpress.XtraBars.BarLinkContainerExItem barContainerEdit;
		private DevExpress.XtraBars.BarLinkContainerExItem barContainerRecordEdit;
		private DevExpress.XtraBars.BarLinkContainerExItem barContainerOpenObject;
		private DevExpress.XtraBars.BarLinkContainerExItem barContainerWorkflow;		 
		private DevExpress.XtraBars.BarLinkContainerExItem barContainerView;
		private DevExpress.XtraBars.BarLinkContainerExItem barContainerReports;
		private DevExpress.XtraBars.BarLinkContainerExItem barContainerDiagnostic;
		private DevExpress.XtraBars.BarLinkContainerExItem barContainerRecordsNavigation;
		private DevExpress.XtraBars.BarLinkContainerExItem barContainerDefault;
		private DevExpress.XtraBars.BarLinkContainerExItem barContainerFilters;
		private DevExpress.XtraBars.BarLinkContainerExItem barContainerExport;
		private DevExpress.XtraBars.BarLinkContainerExItem barContainerPrint;
		private DevExpress.XtraBars.BarLinkContainerExItem barContainerMenu;
		private DevExpress.ExpressApp.Win.Templates.Bars.ActionControls.BarLinkActionControlContainer actionContainerObjectsCreation;
		private DevExpress.ExpressApp.Win.Templates.Bars.ActionControls.BarLinkActionControlContainer actionContainerSave;
		private DevExpress.ExpressApp.Win.Templates.Bars.ActionControls.BarLinkActionControlContainer actionContainerLink;
		private DevExpress.ExpressApp.Win.Templates.Bars.ActionControls.BarLinkActionControlContainer actionContainerEdit;
		private DevExpress.ExpressApp.Win.Templates.Bars.ActionControls.BarLinkActionControlContainer actionContainerRecordEdit;
		private DevExpress.ExpressApp.Win.Templates.Bars.ActionControls.BarLinkActionControlContainer actionContainerWorkflow;
		private DevExpress.ExpressApp.Win.Templates.Bars.ActionControls.BarLinkActionControlContainer actionContainerReports;
		private DevExpress.ExpressApp.Win.Templates.Bars.ActionControls.BarLinkActionControlContainer actionContainerOpenObject;
		private DevExpress.ExpressApp.Win.Templates.Bars.ActionControls.BarLinkActionControlContainer actionContainerView;
		private DevExpress.ExpressApp.Win.Templates.Bars.ActionControls.BarLinkActionControlContainer actionContainerDefault;
		private DevExpress.ExpressApp.Win.Templates.Bars.ActionControls.BarLinkActionControlContainer actionContainerFilters;
		private DevExpress.ExpressApp.Win.Templates.Bars.ActionControls.BarLinkActionControlContainer actionContainerExport;
		private DevExpress.ExpressApp.Win.Templates.Bars.ActionControls.BarLinkActionControlContainer actionContainerDiagnostic;
		private DevExpress.ExpressApp.Win.Templates.Bars.ActionControls.BarLinkActionControlContainer actionContainerMenu;
		private DevExpress.ExpressApp.Win.Templates.Bars.ActionControls.BarLinkActionControlContainer actionContainerRecordsNavigation;
		private DevExpress.ExpressApp.Win.Templates.Bars.ActionControls.BarLinkActionControlContainer actionContainerPrint;
		private DevExpress.XtraBars.Bar standardToolBar;
		private DevExpress.XtraBars.PopupMenu contextMenu;
		private DevExpress.XtraBars.BarDockControl barDockControlTop;
		private DevExpress.XtraBars.BarDockControl barDockControlBottom;
		private DevExpress.XtraBars.BarDockControl barDockControlLeft;
		private DevExpress.XtraBars.BarDockControl barDockControlRight;
		private DevExpress.XtraEditors.PanelControl viewSitePanel;
		private DevExpress.ExpressApp.Win.Templates.ViewSiteManager viewSiteManager;
		#endregion
	}
}
