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
	partial class DetailFormV2 {
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
		#region Windows Form Designer generated code
		private void InitializeComponent() {
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new DevExpress.ExpressApp.Win.Templates.XafComponentResourceManager(typeof(DetailFormV2));
			this.barManager = new DevExpress.ExpressApp.Win.Templates.Bars.XafBarManagerV2(this.components);
			this.actionContainerObjectsCreation = new DevExpress.ExpressApp.Win.Templates.Bars.ActionControls.BarLinkActionControlContainer();
			this.barContainerObjectsCreation = new DevExpress.XtraBars.BarLinkContainerExItem();
			this.actionContainerFile = new DevExpress.ExpressApp.Win.Templates.Bars.ActionControls.BarLinkActionControlContainer();
			this.barContainerFile = new DevExpress.XtraBars.BarLinkContainerExItem();
			this.actionContainerClose = new DevExpress.ExpressApp.Win.Templates.Bars.ActionControls.BarLinkActionControlContainer();
			this.barContainerClose = new DevExpress.XtraBars.BarLinkContainerExItem();
			this.actionContainerSave = new DevExpress.ExpressApp.Win.Templates.Bars.ActionControls.BarLinkActionControlContainer();
			this.barContainerSave = new DevExpress.XtraBars.BarLinkContainerExItem();
			this.actionContainerExport = new DevExpress.ExpressApp.Win.Templates.Bars.ActionControls.BarLinkActionControlContainer();
			this.barContainerExport = new DevExpress.XtraBars.BarLinkContainerExItem();
			this.actionContainerPrint = new DevExpress.ExpressApp.Win.Templates.Bars.ActionControls.BarLinkActionControlContainer();
			this.barContainerPrint = new DevExpress.XtraBars.BarLinkContainerExItem();
			this.actionContainerUndoRedo = new DevExpress.ExpressApp.Win.Templates.Bars.ActionControls.BarLinkActionControlContainer();
			this.barContainerUndoRedo = new DevExpress.XtraBars.BarLinkContainerExItem();
			this.actionContainerEdit = new DevExpress.ExpressApp.Win.Templates.Bars.ActionControls.BarLinkActionControlContainer();
			this.barContainerEdit = new DevExpress.XtraBars.BarLinkContainerExItem();
			this.actionContainerRecordEdit = new DevExpress.ExpressApp.Win.Templates.Bars.ActionControls.BarLinkActionControlContainer();
			this.barContainerRecordEdit = new DevExpress.XtraBars.BarLinkContainerExItem();
			this.actionContainerWorkflow = new DevExpress.ExpressApp.Win.Templates.Bars.ActionControls.BarLinkActionControlContainer();
			this.barContainerWorkflow = new DevExpress.XtraBars.BarLinkContainerExItem();
			this.actionContainerOpenObject = new DevExpress.ExpressApp.Win.Templates.Bars.ActionControls.BarLinkActionControlContainer();
			this.barContainerOpenObject = new DevExpress.XtraBars.BarLinkContainerExItem();
			this.actionContainerRecordsNavigation = new DevExpress.ExpressApp.Win.Templates.Bars.ActionControls.BarLinkActionControlContainer();
			this.barContainerRecordsNavigation = new DevExpress.XtraBars.BarLinkContainerExItem();
			this.actionContainerView = new DevExpress.ExpressApp.Win.Templates.Bars.ActionControls.BarLinkActionControlContainer();
			this.barContainerView = new DevExpress.XtraBars.BarLinkContainerExItem();
			this.actionContainerReports = new DevExpress.ExpressApp.Win.Templates.Bars.ActionControls.BarLinkActionControlContainer();
			this.barContainerReports = new DevExpress.XtraBars.BarLinkContainerExItem();
			this.actionContainerSearch = new DevExpress.ExpressApp.Win.Templates.Bars.ActionControls.BarLinkActionControlContainer();
			this.barContainerSearch = new DevExpress.XtraBars.BarLinkContainerExItem();
			this.actionContainerFullTextSearch = new DevExpress.ExpressApp.Win.Templates.Bars.ActionControls.BarLinkActionControlContainer();
			this.barContainerFullTextSearch = new DevExpress.XtraBars.BarLinkContainerExItem();
			this.actionContainerFilters = new DevExpress.ExpressApp.Win.Templates.Bars.ActionControls.BarLinkActionControlContainer();
			this.barContainerFilters = new DevExpress.XtraBars.BarLinkContainerExItem();
			this.actionContainerTools = new DevExpress.ExpressApp.Win.Templates.Bars.ActionControls.BarLinkActionControlContainer();
			this.barContainerTools = new DevExpress.XtraBars.BarLinkContainerExItem();
			this.actionContainerOptions = new DevExpress.ExpressApp.Win.Templates.Bars.ActionControls.BarLinkActionControlContainer();
			this.barContainerOptions = new DevExpress.XtraBars.BarLinkContainerExItem();
			this.actionContainerDiagnostic = new DevExpress.ExpressApp.Win.Templates.Bars.ActionControls.BarLinkActionControlContainer();
			this.barContainerDiagnostic = new DevExpress.XtraBars.BarLinkContainerExItem();
			this.actionContainerAbout = new DevExpress.ExpressApp.Win.Templates.Bars.ActionControls.BarLinkActionControlContainer();
			this.barContainerAbout = new DevExpress.XtraBars.BarLinkContainerExItem();
			this.actionContainerMenu = new DevExpress.ExpressApp.Win.Templates.Bars.ActionControls.BarLinkActionControlContainer();
			this.barContainerMenu = new DevExpress.XtraBars.BarLinkContainerExItem();
			this.actionContainerNotifications = new DevExpress.ExpressApp.Win.Templates.Bars.ActionControls.BarLinkActionControlContainer();
			this.barContainerNotifications = new DevExpress.XtraBars.BarLinkContainerExItem();
			this.barContainerStatusMessages = new DevExpress.XtraBars.BarLinkContainerExItem();
			this._mainMenuBar = new DevExpress.XtraBars.Bar();
			this.barSubItemFile = new DevExpress.XtraBars.BarSubItem();
			this.barSubItemEdit = new DevExpress.XtraBars.BarSubItem();
			this.barSubItemView = new DevExpress.XtraBars.BarSubItem();
			this.barSubItemTools = new DevExpress.XtraBars.BarSubItem();
			this.barSubItemHelp = new DevExpress.XtraBars.BarSubItem();
			this.standardToolBar = new DevExpress.XtraBars.Bar();
			this._statusBar = new DevExpress.XtraBars.Bar();
			this.barDockControlTop = new DevExpress.XtraBars.BarDockControl();
			this.barDockControlBottom = new DevExpress.XtraBars.BarDockControl();
			this.barDockControlLeft = new DevExpress.XtraBars.BarDockControl();
			this.barDockControlRight = new DevExpress.XtraBars.BarDockControl();
			this.modelSynchronizationManager = new DevExpress.ExpressApp.Win.Templates.ModelSynchronizationManager(this.components);
			this.formStateModelSynchronizer = new DevExpress.ExpressApp.Win.Core.FormStateModelSynchronizer(this.components);
			this.viewSiteManager = new DevExpress.ExpressApp.Win.Templates.ViewSiteManager(this.components);
			this.viewSitePanel = new DevExpress.XtraEditors.PanelControl();
			this.contextMenu = new DevExpress.XtraBars.PopupMenu(this.components);
			((System.ComponentModel.ISupportInitialize)(this.barManager)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.actionContainerObjectsCreation)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.actionContainerFile)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.actionContainerClose)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.actionContainerSave)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.actionContainerExport)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.actionContainerPrint)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.actionContainerUndoRedo)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.actionContainerEdit)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.actionContainerRecordEdit)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.actionContainerWorkflow)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.actionContainerOpenObject)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.actionContainerRecordsNavigation)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.actionContainerView)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.actionContainerReports)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.actionContainerSearch)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.actionContainerFullTextSearch)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.actionContainerFilters)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.actionContainerTools)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.actionContainerOptions)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.actionContainerDiagnostic)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.actionContainerAbout)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.actionContainerMenu)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.actionContainerNotifications)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.viewSitePanel)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.contextMenu)).BeginInit();
			this.SuspendLayout();
			this.barManager.ActionContainers.Add(this.actionContainerObjectsCreation);
			this.barManager.ActionContainers.Add(this.actionContainerFile);
			this.barManager.ActionContainers.Add(this.actionContainerClose);
			this.barManager.ActionContainers.Add(this.actionContainerSave);
			this.barManager.ActionContainers.Add(this.actionContainerExport);
			this.barManager.ActionContainers.Add(this.actionContainerPrint);
			this.barManager.ActionContainers.Add(this.actionContainerUndoRedo);
			this.barManager.ActionContainers.Add(this.actionContainerEdit);
			this.barManager.ActionContainers.Add(this.actionContainerRecordEdit);
			this.barManager.ActionContainers.Add(this.actionContainerWorkflow);
			this.barManager.ActionContainers.Add(this.actionContainerOpenObject);
			this.barManager.ActionContainers.Add(this.actionContainerRecordsNavigation);
			this.barManager.ActionContainers.Add(this.actionContainerView);
			this.barManager.ActionContainers.Add(this.actionContainerReports);
			this.barManager.ActionContainers.Add(this.actionContainerSearch);
			this.barManager.ActionContainers.Add(this.actionContainerFullTextSearch);
			this.barManager.ActionContainers.Add(this.actionContainerFilters);
			this.barManager.ActionContainers.Add(this.actionContainerTools);
			this.barManager.ActionContainers.Add(this.actionContainerOptions);
			this.barManager.ActionContainers.Add(this.actionContainerDiagnostic);
			this.barManager.ActionContainers.Add(this.actionContainerAbout);
			this.barManager.ActionContainers.Add(this.actionContainerMenu);
			this.barManager.ActionContainers.Add(this.actionContainerNotifications);
			this.barManager.Bars.AddRange(new DevExpress.XtraBars.Bar[] {
			this._mainMenuBar,
			this.standardToolBar,
			this._statusBar});
			this.barManager.DockControls.Add(this.barDockControlTop);
			this.barManager.DockControls.Add(this.barDockControlBottom);
			this.barManager.DockControls.Add(this.barDockControlLeft);
			this.barManager.DockControls.Add(this.barDockControlRight);
			this.barManager.Form = this;
			this.barManager.Items.AddRange(new DevExpress.XtraBars.BarItem[] {
			this.barSubItemFile,
			this.barSubItemEdit,
			this.barSubItemView,
			this.barSubItemTools,
			this.barSubItemHelp,
			this.barContainerFile,
			this.barContainerObjectsCreation,
			this.barContainerClose,
			this.barContainerSave,
			this.barContainerEdit,
			this.barContainerOpenObject,
			this.barContainerUndoRedo,
			this.barContainerReports,
			this.barContainerPrint,
			this.barContainerExport,
			this.barContainerMenu,
			this.barContainerRecordEdit,
			this.barContainerWorkflow,
			this.barContainerRecordsNavigation,
			this.barContainerSearch,
			this.barContainerFullTextSearch,
			this.barContainerFilters,
			this.barContainerView,
			this.barContainerTools,
			this.barContainerOptions,
			this.barContainerDiagnostic,
			this.barContainerAbout,
			this.barContainerNotifications,
			this.barContainerStatusMessages});
			this.barManager.MainMenu = this._mainMenuBar;
			this.barManager.MaxItemId = 24;
			this.barManager.StatusBar = this._statusBar;
			this.actionContainerObjectsCreation.BarContainerItem = this.barContainerObjectsCreation;
			this.actionContainerObjectsCreation.ActionCategory = "ObjectsCreation";
			resources.ApplyResources(this.barContainerObjectsCreation, "barContainerObjectsCreation");
			this.barContainerObjectsCreation.Id = 17;
			this.barContainerObjectsCreation.MergeType = DevExpress.XtraBars.BarMenuMerge.MergeItems;
			this.barContainerObjectsCreation.Name = "barContainerObjectsCreation";
			this.actionContainerFile.BarContainerItem = this.barContainerFile;
			this.actionContainerFile.ActionCategory = "File";
			resources.ApplyResources(this.barContainerFile, "barContainerFile");
			this.barContainerFile.Id = 5;
			this.barContainerFile.MergeOrder = 2;
			this.barContainerFile.MergeType = DevExpress.XtraBars.BarMenuMerge.MergeItems;
			this.barContainerFile.Name = "barContainerFile";
			this.actionContainerClose.BarContainerItem = this.barContainerClose;
			this.actionContainerClose.ActionCategory = "Close";
			resources.ApplyResources(this.barContainerClose, "barContainerClose");
			this.barContainerClose.Id = 18;
			this.barContainerClose.MergeOrder = 2;
			this.barContainerClose.MergeType = DevExpress.XtraBars.BarMenuMerge.MergeItems;
			this.barContainerClose.Name = "barContainerClose";
			this.actionContainerSave.BarContainerItem = this.barContainerSave;
			this.actionContainerSave.ActionCategory = "Save";
			resources.ApplyResources(this.barContainerSave, "barContainerSave");
			this.barContainerSave.Id = 17;
			this.barContainerSave.MergeType = DevExpress.XtraBars.BarMenuMerge.MergeItems;
			this.barContainerSave.Name = "barContainerSave";
			this.actionContainerExport.BarContainerItem = this.barContainerExport;
			this.actionContainerExport.ActionCategory = "Export";
			resources.ApplyResources(this.barContainerExport, "barContainerExport");
			this.barContainerExport.Id = 7;
			this.barContainerExport.MergeType = DevExpress.XtraBars.BarMenuMerge.MergeItems;
			this.barContainerExport.Name = "barContainerExport";
			this.actionContainerPrint.BarContainerItem = this.barContainerPrint;
			this.actionContainerPrint.ActionCategory = "Print";
			resources.ApplyResources(this.barContainerPrint, "barContainerPrint");
			this.barContainerPrint.Id = 6;
			this.barContainerPrint.MergeType = DevExpress.XtraBars.BarMenuMerge.MergeItems;
			this.barContainerPrint.Name = "barContainerPrint";
			this.actionContainerUndoRedo.BarContainerItem = this.barContainerUndoRedo;
			this.actionContainerUndoRedo.ActionCategory = "UndoRedo";
			resources.ApplyResources(this.barContainerUndoRedo, "barContainerUndoRedo");
			this.barContainerUndoRedo.Id = 19;
			this.barContainerUndoRedo.MergeType = DevExpress.XtraBars.BarMenuMerge.MergeItems;
			this.barContainerUndoRedo.Name = "barContainerUndoRedo";
			this.actionContainerEdit.BarContainerItem = this.barContainerEdit;
			this.actionContainerEdit.ActionCategory = "Edit";
			resources.ApplyResources(this.barContainerEdit, "barContainerEdit");
			this.barContainerEdit.Id = 18;
			this.barContainerEdit.MergeType = DevExpress.XtraBars.BarMenuMerge.MergeItems;
			this.barContainerEdit.Name = "barContainerEdit";
			this.actionContainerRecordEdit.BarContainerItem = this.barContainerRecordEdit;
			this.actionContainerRecordEdit.ActionCategory = "RecordEdit";
			resources.ApplyResources(this.barContainerRecordEdit, "barContainerRecordEdit");
			this.barContainerRecordEdit.Id = 9;
			this.barContainerRecordEdit.MergeType = DevExpress.XtraBars.BarMenuMerge.MergeItems;
			this.barContainerRecordEdit.Name = "barContainerRecordEdit";
			this.actionContainerWorkflow.BarContainerItem = this.barContainerWorkflow;
			this.actionContainerWorkflow.ActionCategory = "Workflow";
			resources.ApplyResources(this.barContainerWorkflow, "barContainerWorkflow");
			this.barContainerWorkflow.Id = 9;
			this.barContainerWorkflow.MergeType = DevExpress.XtraBars.BarMenuMerge.MergeItems;
			this.barContainerWorkflow.Name = "barContainerWorkflow";
			this.actionContainerOpenObject.BarContainerItem = this.barContainerOpenObject;
			this.actionContainerOpenObject.ActionCategory = "OpenObject";
			resources.ApplyResources(this.barContainerOpenObject, "barContainerOpenObject");
			this.barContainerOpenObject.Id = 20;
			this.barContainerOpenObject.MergeType = DevExpress.XtraBars.BarMenuMerge.MergeItems;
			this.barContainerOpenObject.Name = "barContainerOpenObject";
			this.actionContainerRecordsNavigation.BarContainerItem = this.barContainerRecordsNavigation;
			this.actionContainerRecordsNavigation.ActionCategory = "RecordsNavigation";
			resources.ApplyResources(this.barContainerRecordsNavigation, "barContainerRecordsNavigation");
			this.barContainerRecordsNavigation.Id = 10;
			this.barContainerRecordsNavigation.MergeType = DevExpress.XtraBars.BarMenuMerge.MergeItems;
			this.barContainerRecordsNavigation.Name = "barContainerRecordsNavigation";
			this.actionContainerView.BarContainerItem = this.barContainerView;
			this.actionContainerView.ActionCategory = "View";
			resources.ApplyResources(this.barContainerView, "barContainerView");
			this.barContainerView.Id = 12;
			this.barContainerView.MergeType = DevExpress.XtraBars.BarMenuMerge.MergeItems;
			this.barContainerView.Name = "barContainerView";
			this.actionContainerReports.BarContainerItem = this.barContainerReports;
			this.actionContainerReports.ActionCategory = "Reports";
			resources.ApplyResources(this.barContainerReports, "barContainerReports");
			this.barContainerReports.Id = 20;
			this.barContainerReports.MergeType = DevExpress.XtraBars.BarMenuMerge.MergeItems;
			this.barContainerReports.Name = "barContainerReports";
			this.actionContainerSearch.BarContainerItem = this.barContainerSearch;
			this.actionContainerSearch.ActionCategory = "Search";
			resources.ApplyResources(this.barContainerSearch, "barContainerSearch");
			this.barContainerSearch.Id = 11;
			this.barContainerSearch.MergeType = DevExpress.XtraBars.BarMenuMerge.MergeItems;
			this.barContainerSearch.Name = "barContainerSearch";
			this.actionContainerFullTextSearch.BarContainerItem = this.barContainerFullTextSearch;
			this.actionContainerFullTextSearch.ActionCategory = "FullTextSearch";
			this.barContainerFullTextSearch.Alignment = DevExpress.XtraBars.BarItemLinkAlignment.Right;
			resources.ApplyResources(this.barContainerFullTextSearch, "barContainerFullTextSearch");
			this.barContainerFullTextSearch.Id = 12;
			this.barContainerFullTextSearch.MergeType = DevExpress.XtraBars.BarMenuMerge.MergeItems;
			this.barContainerFullTextSearch.Name = "barContainerFullTextSearch";
			this.actionContainerFilters.BarContainerItem = this.barContainerFilters;
			this.actionContainerFilters.ActionCategory = "Filters";
			resources.ApplyResources(this.barContainerFilters, "barContainerFilters");
			this.barContainerFilters.Id = 26;
			this.barContainerFilters.MergeType = DevExpress.XtraBars.BarMenuMerge.MergeItems;
			this.barContainerFilters.Name = "barContainerFilters";
			this.actionContainerTools.BarContainerItem = this.barContainerTools;
			this.actionContainerTools.ActionCategory = "Tools";
			this.actionContainerTools.IsMenuMode = true;
			resources.ApplyResources(this.barContainerTools, "barContainerTools");
			this.barContainerTools.Id = 13;
			this.barContainerTools.MergeType = DevExpress.XtraBars.BarMenuMerge.MergeItems;
			this.barContainerTools.Name = "barContainerTools";
			this.actionContainerOptions.BarContainerItem = this.barContainerOptions;
			this.actionContainerOptions.ActionCategory = "Options";
			this.actionContainerOptions.IsMenuMode = true;
			resources.ApplyResources(this.barContainerOptions, "barContainerOptions");
			this.barContainerOptions.Id = 14;
			this.barContainerOptions.MergeType = DevExpress.XtraBars.BarMenuMerge.MergeItems;
			this.barContainerOptions.Name = "barContainerOptions";
			this.actionContainerDiagnostic.BarContainerItem = this.barContainerDiagnostic;
			this.actionContainerDiagnostic.ActionCategory = "Diagnostic";
			this.actionContainerDiagnostic.IsMenuMode = true;
			resources.ApplyResources(this.barContainerDiagnostic, "barContainerDiagnostic");
			this.barContainerDiagnostic.Id = 16;
			this.barContainerDiagnostic.MergeType = DevExpress.XtraBars.BarMenuMerge.MergeItems;
			this.barContainerDiagnostic.Name = "barContainerDiagnostic";
			this.actionContainerAbout.BarContainerItem = this.barContainerAbout;
			this.actionContainerAbout.ActionCategory = "About";
			this.actionContainerAbout.IsMenuMode = true;
			resources.ApplyResources(this.barContainerAbout, "barContainerAbout");
			this.barContainerAbout.Id = 15;
			this.barContainerAbout.MergeType = DevExpress.XtraBars.BarMenuMerge.MergeItems;
			this.barContainerAbout.Name = "barContainerAbout";
			this.actionContainerMenu.BarContainerItem = this.barContainerMenu;
			this.actionContainerMenu.ActionCategory = "Menu";
			resources.ApplyResources(this.barContainerMenu, "barContainerMenu");
			this.barContainerMenu.Id = 8;
			this.barContainerMenu.MergeType = DevExpress.XtraBars.BarMenuMerge.MergeItems;
			this.barContainerMenu.Name = "barContainerMenu";
			this.actionContainerNotifications.BarContainerItem = this.barContainerNotifications;
			this.actionContainerNotifications.ActionCategory = "Notifications";
			resources.ApplyResources(this.barContainerNotifications, "barContainerNotifications");
			this.barContainerNotifications.Id = 28;
			this.barContainerNotifications.MergeType = DevExpress.XtraBars.BarMenuMerge.MergeItems;
			this.barContainerNotifications.Alignment = XtraBars.BarItemLinkAlignment.Right;
			this.barContainerNotifications.Name = "barContainerNotifications";
			resources.ApplyResources(this.barContainerStatusMessages, "barContainerStatusMessages");
			this.barContainerStatusMessages.Id = 27;
			this.barContainerStatusMessages.MergeType = DevExpress.XtraBars.BarMenuMerge.MergeItems;
			this.barContainerStatusMessages.Name = "barContainerStatusMessages";
			this._mainMenuBar.BarName = "Main Menu";
			this._mainMenuBar.DockCol = 0;
			this._mainMenuBar.DockRow = 0;
			this._mainMenuBar.DockStyle = DevExpress.XtraBars.BarDockStyle.Top;
			this._mainMenuBar.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] {
			new DevExpress.XtraBars.LinkPersistInfo(this.barSubItemFile),
			new DevExpress.XtraBars.LinkPersistInfo(this.barSubItemEdit),
			new DevExpress.XtraBars.LinkPersistInfo(this.barSubItemView),
			new DevExpress.XtraBars.LinkPersistInfo(this.barSubItemTools),
			new DevExpress.XtraBars.LinkPersistInfo(this.barSubItemHelp)});
			this._mainMenuBar.OptionsBar.MultiLine = true;
			this._mainMenuBar.OptionsBar.UseWholeRow = true;
			resources.ApplyResources(this._mainMenuBar, "_mainMenuBar");
			resources.ApplyResources(this.barSubItemFile, "barSubItemFile");
			this.barSubItemFile.HideWhenEmpty = true;
			this.barSubItemFile.Id = 0;
			this.barSubItemFile.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] {
			new DevExpress.XtraBars.LinkPersistInfo(this.barContainerObjectsCreation, true),
			new DevExpress.XtraBars.LinkPersistInfo(this.barContainerFile, true),
			new DevExpress.XtraBars.LinkPersistInfo(this.barContainerClose, true),
			new DevExpress.XtraBars.LinkPersistInfo(this.barContainerSave, true),
			new DevExpress.XtraBars.LinkPersistInfo(this.barContainerExport, true),
			new DevExpress.XtraBars.LinkPersistInfo(this.barContainerPrint, true)});
			this.barSubItemFile.MergeType = DevExpress.XtraBars.BarMenuMerge.MergeItems;
			this.barSubItemFile.Name = "barSubItemFile";
			resources.ApplyResources(this.barSubItemEdit, "barSubItemEdit");
			this.barSubItemEdit.HideWhenEmpty = true;
			this.barSubItemEdit.Id = 1;
			this.barSubItemEdit.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] {
			new DevExpress.XtraBars.LinkPersistInfo(this.barContainerUndoRedo, true),
			new DevExpress.XtraBars.LinkPersistInfo(this.barContainerEdit, true),
			new DevExpress.XtraBars.LinkPersistInfo(this.barContainerRecordEdit, true),
			new DevExpress.XtraBars.LinkPersistInfo(this.barContainerWorkflow, true),
			new DevExpress.XtraBars.LinkPersistInfo(this.barContainerOpenObject, true)});
			this.barSubItemEdit.MergeType = DevExpress.XtraBars.BarMenuMerge.MergeItems;
			this.barSubItemEdit.Name = "barSubItemEdit";
			resources.ApplyResources(this.barSubItemView, "barSubItemView");
			this.barSubItemView.HideWhenEmpty = true;
			this.barSubItemView.Id = 2;
			this.barSubItemView.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] {
			new DevExpress.XtraBars.LinkPersistInfo(this.barContainerRecordsNavigation, true),
			new DevExpress.XtraBars.LinkPersistInfo(this.barContainerView, true),
			new DevExpress.XtraBars.LinkPersistInfo(this.barContainerReports, true),
			new DevExpress.XtraBars.LinkPersistInfo(this.barContainerSearch, true),
			new DevExpress.XtraBars.LinkPersistInfo(DevExpress.XtraBars.BarLinkUserDefines.None, false, this.barContainerFullTextSearch, true),
			new DevExpress.XtraBars.LinkPersistInfo(DevExpress.XtraBars.BarLinkUserDefines.None, false, this.barContainerFilters, true)});
			this.barSubItemView.MergeType = DevExpress.XtraBars.BarMenuMerge.MergeItems;
			this.barSubItemView.Name = "barSubItemView";
			resources.ApplyResources(this.barSubItemTools, "barSubItemTools");
			this.barSubItemTools.HideWhenEmpty = true;
			this.barSubItemTools.Id = 3;
			this.barSubItemTools.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] {
			new DevExpress.XtraBars.LinkPersistInfo(this.barContainerTools, true),
			new DevExpress.XtraBars.LinkPersistInfo(this.barContainerOptions, true),
			new DevExpress.XtraBars.LinkPersistInfo(this.barContainerDiagnostic, true)});
			this.barSubItemTools.MergeType = DevExpress.XtraBars.BarMenuMerge.MergeItems;
			this.barSubItemTools.Name = "barSubItemTools";
			resources.ApplyResources(this.barSubItemHelp, "barSubItemHelp");
			this.barSubItemHelp.HideWhenEmpty = true;
			this.barSubItemHelp.Id = 4;
			this.barSubItemHelp.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] {
			new DevExpress.XtraBars.LinkPersistInfo(this.barContainerAbout, true)});
			this.barSubItemHelp.MergeType = DevExpress.XtraBars.BarMenuMerge.MergeItems;
			this.barSubItemHelp.Name = "barSubItemHelp";
			this.standardToolBar.BarName = "Main Toolbar";
			this.standardToolBar.DockCol = 0;
			this.standardToolBar.DockRow = 1;
			this.standardToolBar.DockStyle = DevExpress.XtraBars.BarDockStyle.Top;
			this.standardToolBar.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] {
			new DevExpress.XtraBars.LinkPersistInfo(this.barContainerObjectsCreation, true),
			new DevExpress.XtraBars.LinkPersistInfo(this.barContainerSave, true),
			new DevExpress.XtraBars.LinkPersistInfo(this.barContainerEdit, true),
			new DevExpress.XtraBars.LinkPersistInfo(this.barContainerUndoRedo, true),
			new DevExpress.XtraBars.LinkPersistInfo(this.barContainerRecordEdit, true),
			new DevExpress.XtraBars.LinkPersistInfo(this.barContainerOpenObject),
			new DevExpress.XtraBars.LinkPersistInfo(this.barContainerWorkflow, true),
			new DevExpress.XtraBars.LinkPersistInfo(this.barContainerView, true),
			new DevExpress.XtraBars.LinkPersistInfo(this.barContainerReports),
			new DevExpress.XtraBars.LinkPersistInfo(this.barContainerRecordsNavigation, true),
			new DevExpress.XtraBars.LinkPersistInfo(this.barContainerClose, true),
			new DevExpress.XtraBars.LinkPersistInfo(this.barContainerFilters, true),
			new DevExpress.XtraBars.LinkPersistInfo(this.barContainerSearch, true),
			new DevExpress.XtraBars.LinkPersistInfo(this.barContainerFullTextSearch)});
			this.standardToolBar.OptionsBar.UseWholeRow = true;
			resources.ApplyResources(this.standardToolBar, "standardToolBar");
			this._statusBar.BarName = "StatusBar";
			this._statusBar.CanDockStyle = DevExpress.XtraBars.BarCanDockStyle.Bottom;
			this._statusBar.DockCol = 0;
			this._statusBar.DockRow = 0;
			this._statusBar.DockStyle = DevExpress.XtraBars.BarDockStyle.Bottom;
			this._statusBar.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] {
			new DevExpress.XtraBars.LinkPersistInfo(this.barContainerStatusMessages),
			new DevExpress.XtraBars.LinkPersistInfo(this.barContainerNotifications)});
			this._statusBar.OptionsBar.AllowQuickCustomization = false;
			this._statusBar.OptionsBar.DisableClose = true;
			this._statusBar.OptionsBar.DisableCustomization = true;
			this._statusBar.OptionsBar.DrawDragBorder = false;
			this._statusBar.OptionsBar.DrawSizeGrip = true;
			this._statusBar.OptionsBar.UseWholeRow = true;
			resources.ApplyResources(this._statusBar, "_statusBar");
			this.barDockControlTop.CausesValidation = false;
			resources.ApplyResources(this.barDockControlTop, "barDockControlTop");
			this.barDockControlBottom.CausesValidation = false;
			resources.ApplyResources(this.barDockControlBottom, "barDockControlBottom");
			this.barDockControlLeft.CausesValidation = false;
			resources.ApplyResources(this.barDockControlLeft, "barDockControlLeft");
			this.barDockControlRight.CausesValidation = false;
			resources.ApplyResources(this.barDockControlRight, "barDockControlRight");
			this.modelSynchronizationManager.ModelSynchronizableComponents.Add(this.formStateModelSynchronizer);
			this.formStateModelSynchronizer.Form = this;
			this.viewSiteManager.ViewSiteControl = this.viewSitePanel;
			this.viewSitePanel.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			resources.ApplyResources(this.viewSitePanel, "viewSitePanel");
			this.viewSitePanel.Name = "viewSitePanel";
			this.contextMenu.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] {
			new DevExpress.XtraBars.LinkPersistInfo(this.barContainerObjectsCreation, true),
			new DevExpress.XtraBars.LinkPersistInfo(this.barContainerSave, true),
			new DevExpress.XtraBars.LinkPersistInfo(this.barContainerEdit, true),
			new DevExpress.XtraBars.LinkPersistInfo(this.barContainerOpenObject, true),
			new DevExpress.XtraBars.LinkPersistInfo(this.barContainerUndoRedo, true),
			new DevExpress.XtraBars.LinkPersistInfo(this.barContainerReports, true),
			new DevExpress.XtraBars.LinkPersistInfo(this.barContainerClose, true),
			new DevExpress.XtraBars.LinkPersistInfo(this.barContainerRecordEdit, true),
			new DevExpress.XtraBars.LinkPersistInfo(this.barContainerView, true),
			new DevExpress.XtraBars.LinkPersistInfo(this.barContainerPrint, true),
			new DevExpress.XtraBars.LinkPersistInfo(this.barContainerExport, true),
			new DevExpress.XtraBars.LinkPersistInfo(this.barContainerMenu, true)});
			this.contextMenu.Manager = this.barManager;
			this.contextMenu.Name = "contextMenu";
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.viewSitePanel);
			this.Controls.Add(this.barDockControlLeft);
			this.Controls.Add(this.barDockControlRight);
			this.Controls.Add(this.barDockControlBottom);
			this.Controls.Add(this.barDockControlTop);
			this.Name = "DetailFormV2";
			((System.ComponentModel.ISupportInitialize)(this.barManager)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.actionContainerObjectsCreation)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.actionContainerFile)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.actionContainerClose)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.actionContainerSave)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.actionContainerExport)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.actionContainerPrint)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.actionContainerUndoRedo)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.actionContainerEdit)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.actionContainerRecordEdit)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.actionContainerWorkflow)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.actionContainerOpenObject)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.actionContainerRecordsNavigation)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.actionContainerView)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.actionContainerReports)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.actionContainerSearch)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.actionContainerFullTextSearch)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.actionContainerFilters)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.actionContainerTools)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.actionContainerOptions)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.actionContainerDiagnostic)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.actionContainerAbout)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.actionContainerMenu)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.actionContainerNotifications)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.viewSitePanel)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.contextMenu)).EndInit();
			this.ResumeLayout(false);
		}
		#endregion
		private DevExpress.ExpressApp.Win.Templates.Bars.XafBarManagerV2 barManager;
		private DevExpress.XtraEditors.PanelControl viewSitePanel;
		private DevExpress.ExpressApp.Win.Core.FormStateModelSynchronizer formStateModelSynchronizer;
		private DevExpress.ExpressApp.Win.Templates.ModelSynchronizationManager modelSynchronizationManager;
		private DevExpress.ExpressApp.Win.Templates.ViewSiteManager viewSiteManager;
		private DevExpress.XtraBars.BarDockControl barDockControlTop;
		private DevExpress.XtraBars.BarDockControl barDockControlBottom;
		private DevExpress.XtraBars.BarDockControl barDockControlLeft;
		private DevExpress.XtraBars.BarDockControl barDockControlRight;
		private DevExpress.XtraBars.Bar _mainMenuBar;
		private DevExpress.XtraBars.Bar standardToolBar;
		private DevExpress.XtraBars.Bar _statusBar;
		private DevExpress.XtraBars.BarSubItem barSubItemFile;
		private DevExpress.XtraBars.BarSubItem barSubItemEdit;
		private DevExpress.XtraBars.BarSubItem barSubItemView;
		private DevExpress.XtraBars.BarSubItem barSubItemTools;
		private DevExpress.XtraBars.BarSubItem barSubItemHelp;
		private DevExpress.XtraBars.PopupMenu contextMenu;
		private DevExpress.XtraBars.BarLinkContainerExItem barContainerFile;
		private DevExpress.XtraBars.BarLinkContainerExItem barContainerObjectsCreation;
		private DevExpress.XtraBars.BarLinkContainerExItem barContainerClose;
		private DevExpress.XtraBars.BarLinkContainerExItem barContainerSave;
		private DevExpress.XtraBars.BarLinkContainerExItem barContainerEdit;
		private DevExpress.XtraBars.BarLinkContainerExItem barContainerOpenObject;
		private DevExpress.XtraBars.BarLinkContainerExItem barContainerUndoRedo;
		private DevExpress.XtraBars.BarLinkContainerExItem barContainerReports;
		private DevExpress.XtraBars.BarLinkContainerExItem barContainerPrint;
		private DevExpress.XtraBars.BarLinkContainerExItem barContainerExport;
		private DevExpress.XtraBars.BarLinkContainerExItem barContainerMenu;
		private DevExpress.XtraBars.BarLinkContainerExItem barContainerRecordEdit;
		private DevExpress.XtraBars.BarLinkContainerExItem barContainerWorkflow;
		private DevExpress.XtraBars.BarLinkContainerExItem barContainerRecordsNavigation;
		private DevExpress.XtraBars.BarLinkContainerExItem barContainerSearch;
		private DevExpress.XtraBars.BarLinkContainerExItem barContainerFullTextSearch;
		private DevExpress.XtraBars.BarLinkContainerExItem barContainerFilters;
		private DevExpress.XtraBars.BarLinkContainerExItem barContainerView;
		private DevExpress.XtraBars.BarLinkContainerExItem barContainerTools;
		private DevExpress.XtraBars.BarLinkContainerExItem barContainerOptions;
		private DevExpress.XtraBars.BarLinkContainerExItem barContainerAbout;
		private DevExpress.XtraBars.BarLinkContainerExItem barContainerDiagnostic;
		private DevExpress.XtraBars.BarLinkContainerExItem barContainerNotifications;
		private DevExpress.XtraBars.BarLinkContainerExItem barContainerStatusMessages;
		private DevExpress.ExpressApp.Win.Templates.Bars.ActionControls.BarLinkActionControlContainer actionContainerFile;
		private DevExpress.ExpressApp.Win.Templates.Bars.ActionControls.BarLinkActionControlContainer actionContainerObjectsCreation;
		private DevExpress.ExpressApp.Win.Templates.Bars.ActionControls.BarLinkActionControlContainer actionContainerClose;
		private DevExpress.ExpressApp.Win.Templates.Bars.ActionControls.BarLinkActionControlContainer actionContainerSave;
		private DevExpress.ExpressApp.Win.Templates.Bars.ActionControls.BarLinkActionControlContainer actionContainerEdit;
		private DevExpress.ExpressApp.Win.Templates.Bars.ActionControls.BarLinkActionControlContainer actionContainerOpenObject;
		private DevExpress.ExpressApp.Win.Templates.Bars.ActionControls.BarLinkActionControlContainer actionContainerUndoRedo;
		private DevExpress.ExpressApp.Win.Templates.Bars.ActionControls.BarLinkActionControlContainer actionContainerReports;
		private DevExpress.ExpressApp.Win.Templates.Bars.ActionControls.BarLinkActionControlContainer actionContainerPrint;
		private DevExpress.ExpressApp.Win.Templates.Bars.ActionControls.BarLinkActionControlContainer actionContainerExport;
		private DevExpress.ExpressApp.Win.Templates.Bars.ActionControls.BarLinkActionControlContainer actionContainerRecordEdit;
		private DevExpress.ExpressApp.Win.Templates.Bars.ActionControls.BarLinkActionControlContainer actionContainerWorkflow;
		private DevExpress.ExpressApp.Win.Templates.Bars.ActionControls.BarLinkActionControlContainer actionContainerRecordsNavigation;
		private DevExpress.ExpressApp.Win.Templates.Bars.ActionControls.BarLinkActionControlContainer actionContainerSearch;
		private DevExpress.ExpressApp.Win.Templates.Bars.ActionControls.BarLinkActionControlContainer actionContainerFullTextSearch;
		private DevExpress.ExpressApp.Win.Templates.Bars.ActionControls.BarLinkActionControlContainer actionContainerFilters;
		private DevExpress.ExpressApp.Win.Templates.Bars.ActionControls.BarLinkActionControlContainer actionContainerView;
		private DevExpress.ExpressApp.Win.Templates.Bars.ActionControls.BarLinkActionControlContainer actionContainerTools;
		private DevExpress.ExpressApp.Win.Templates.Bars.ActionControls.BarLinkActionControlContainer actionContainerOptions;
		private DevExpress.ExpressApp.Win.Templates.Bars.ActionControls.BarLinkActionControlContainer actionContainerAbout;
		private DevExpress.ExpressApp.Win.Templates.Bars.ActionControls.BarLinkActionControlContainer actionContainerDiagnostic;
		private DevExpress.ExpressApp.Win.Templates.Bars.ActionControls.BarLinkActionControlContainer actionContainerMenu;
		private DevExpress.ExpressApp.Win.Templates.Bars.ActionControls.BarLinkActionControlContainer actionContainerNotifications;
	}
}
