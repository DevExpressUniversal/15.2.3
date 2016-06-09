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

using DevExpress.XtraBars;
namespace DevExpress.DashboardWin.Native {
	partial class DataFieldsBrowser {
		private System.ComponentModel.IContainer components = null;
		#region Component Designer generated code
		private void InitializeComponent() {
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DataFieldsBrowser));
			this.treeList = new DevExpress.XtraTreeList.TreeList();
			this.tlcContent = new DevExpress.XtraTreeList.Columns.TreeListColumn();
			this.tlcCaption = new DevExpress.XtraTreeList.Columns.TreeListColumn();
			this.repositoryItemTextEdit = new DevExpress.XtraEditors.Repository.RepositoryItemTextEdit();
			this.barManager = new DevExpress.XtraBars.BarManager(this.components);
			this.toolsBar = new DevExpress.XtraBars.Bar();
			this.groupByTypeBarItem = new DevExpress.XtraBars.BarCheckItem();
			this.sortAscendingBarItem = new DevExpress.XtraBars.BarCheckItem();
			this.sortDescendingBarItem = new DevExpress.XtraBars.BarCheckItem();
			this.standaloneBarDockControl = new DevExpress.XtraBars.StandaloneBarDockControl();
			this.barDockControlTop = new DevExpress.XtraBars.BarDockControl();
			this.barDockControlBottom = new DevExpress.XtraBars.BarDockControl();
			this.barDockControlLeft = new DevExpress.XtraBars.BarDockControl();
			this.barDockControlRight = new DevExpress.XtraBars.BarDockControl();
			this.imageList = new System.Windows.Forms.ImageList(this.components);
			this.toolTipController1 = new DevExpress.Utils.ToolTipController(this.components);
			this.panelControl = new DevExpress.XtraEditors.PanelControl();
			this.refreshFieldListBarItem = new DevExpress.XtraBars.BarButtonItem();
			((System.ComponentModel.ISupportInitialize)(this.treeList)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.repositoryItemTextEdit)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.barManager)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.panelControl)).BeginInit();
			this.panelControl.SuspendLayout();
			this.SuspendLayout();
			resources.ApplyResources(this.treeList, "treeList");
			this.treeList.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.Simple;
			this.treeList.CausesValidation = false;
			this.treeList.Columns.AddRange(new DevExpress.XtraTreeList.Columns.TreeListColumn[] {
			this.tlcContent,
			this.tlcCaption});
			this.treeList.CustomizationFormBounds = new System.Drawing.Rectangle(60, 409, 216, 178);
			this.treeList.MenuManager = this.barManager;
			this.treeList.Name = "treeList";
			this.treeList.OptionsBehavior.Editable = false;
			this.treeList.OptionsSelection.InvertSelection = true;
			this.treeList.OptionsView.AutoWidth = true;
			this.treeList.OptionsView.ShowColumns = false;
			this.treeList.OptionsView.ShowHorzLines = false;
			this.treeList.OptionsView.ShowIndicator = false;
			this.treeList.OptionsView.ShowVertLines = false;
			this.treeList.BestFitVisibleOnly = true;
			this.treeList.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
			this.repositoryItemTextEdit});
			this.treeList.SelectImageList = this.imageList;
			this.treeList.ToolTipController = this.toolTipController1;
			this.treeList.BeforeExpand += new DevExpress.XtraTreeList.BeforeExpandEventHandler(this.treeList_BeforeExpand);
			this.treeList.AfterExpand += new DevExpress.XtraTreeList.NodeEventHandler(this.treeList_AfterExpand);
			this.treeList.AfterCollapse += new DevExpress.XtraTreeList.NodeEventHandler(this.treeList_AfterCollapse);
			this.treeList.FocusedNodeChanged += new DevExpress.XtraTreeList.FocusedNodeChangedEventHandler(this.treeList_FocusedNodeChanged);
			this.treeList.CustomDrawNodeCell += new DevExpress.XtraTreeList.CustomDrawNodeCellEventHandler(this.treeList_CustomDrawNodeCell);
			this.treeList.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.treeList_MouseDoubleClick);
			resources.ApplyResources(this.tlcContent, "tlcContent");
			this.tlcContent.FieldName = "Content";
			this.tlcContent.Name = "tlcContent";
			resources.ApplyResources(this.tlcCaption, "tlcCaption");
			this.tlcCaption.ColumnEdit = this.repositoryItemTextEdit;
			this.tlcCaption.FieldName = "Content";
			this.tlcCaption.Name = "tlcCaption";
			resources.ApplyResources(this.repositoryItemTextEdit, "repositoryItemTextEdit");
			this.repositoryItemTextEdit.Name = "repositoryItemTextEdit";
			this.barManager.AllowCustomization = false;
			this.barManager.AllowMoveBarOnToolbar = false;
			this.barManager.AllowShowToolbarsPopup = false;
			this.barManager.Bars.AddRange(new DevExpress.XtraBars.Bar[] {
			this.toolsBar});
			this.barManager.DockControls.Add(this.barDockControlTop);
			this.barManager.DockControls.Add(this.barDockControlBottom);
			this.barManager.DockControls.Add(this.barDockControlLeft);
			this.barManager.DockControls.Add(this.barDockControlRight);
			this.barManager.DockControls.Add(this.standaloneBarDockControl);
			this.barManager.Form = this;
			this.barManager.Items.AddRange(new DevExpress.XtraBars.BarItem[] {
			this.groupByTypeBarItem,
			this.sortAscendingBarItem,
			this.sortDescendingBarItem,
			this.refreshFieldListBarItem});
			this.barManager.MaxItemId = 6;
			this.toolsBar.BarName = "Tools";
			this.toolsBar.DockCol = 0;
			this.toolsBar.DockRow = 0;
			this.toolsBar.DockStyle = DevExpress.XtraBars.BarDockStyle.Standalone;
			this.toolsBar.FloatLocation = new System.Drawing.Point(410, 162);
			this.toolsBar.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] {
			new DevExpress.XtraBars.LinkPersistInfo(this.groupByTypeBarItem),
			new DevExpress.XtraBars.LinkPersistInfo(this.sortAscendingBarItem),
			new DevExpress.XtraBars.LinkPersistInfo(this.sortDescendingBarItem),
			new DevExpress.XtraBars.LinkPersistInfo(this.refreshFieldListBarItem)});
			this.toolsBar.OptionsBar.AllowQuickCustomization = false;
			this.toolsBar.OptionsBar.DrawBorder = false;
			this.toolsBar.OptionsBar.DrawDragBorder = false;
			this.toolsBar.OptionsBar.MultiLine = true;
			this.toolsBar.OptionsBar.UseWholeRow = true;
			this.toolsBar.StandaloneBarDockControl = this.standaloneBarDockControl;
			resources.ApplyResources(this.toolsBar, "toolsBar");
			this.groupByTypeBarItem.BindableChecked = true;
			this.groupByTypeBarItem.Checked = true;
			this.groupByTypeBarItem.Id = 0;
			this.groupByTypeBarItem.Name = "groupByTypeBarItem";
			this.groupByTypeBarItem.CheckedChanged += new DevExpress.XtraBars.ItemClickEventHandler(this.GroupAndSortBarItem_CheckedChanged);
			this.sortAscendingBarItem.BindableChecked = true;
			this.sortAscendingBarItem.Checked = true;
			this.sortAscendingBarItem.GroupIndex = 1;
			this.sortAscendingBarItem.Id = 1;
			this.sortAscendingBarItem.Name = "sortAscendingBarItem";
			this.sortAscendingBarItem.CheckedChanged += new DevExpress.XtraBars.ItemClickEventHandler(this.GroupAndSortBarItem_CheckedChanged);
			this.sortDescendingBarItem.GroupIndex = 1;
			this.sortDescendingBarItem.Id = 2;
			this.sortDescendingBarItem.Name = "sortDescendingBarItem";
			this.sortDescendingBarItem.CheckedChanged += new DevExpress.XtraBars.ItemClickEventHandler(this.GroupAndSortBarItem_CheckedChanged);
			this.standaloneBarDockControl.Appearance.BackColor = ((System.Drawing.Color)(resources.GetObject("standaloneBarDockControl.Appearance.BackColor")));
			this.standaloneBarDockControl.Appearance.Options.UseBackColor = true;
			this.standaloneBarDockControl.CausesValidation = false;
			resources.ApplyResources(this.standaloneBarDockControl, "standaloneBarDockControl");
			this.standaloneBarDockControl.Name = "standaloneBarDockControl";
			this.barDockControlTop.CausesValidation = false;
			resources.ApplyResources(this.barDockControlTop, "barDockControlTop");
			this.barDockControlBottom.CausesValidation = false;
			resources.ApplyResources(this.barDockControlBottom, "barDockControlBottom");
			this.barDockControlLeft.CausesValidation = false;
			resources.ApplyResources(this.barDockControlLeft, "barDockControlLeft");
			this.barDockControlRight.CausesValidation = false;
			resources.ApplyResources(this.barDockControlRight, "barDockControlRight");
			this.imageList.ColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit;
			resources.ApplyResources(this.imageList, "imageList");
			this.imageList.TransparentColor = System.Drawing.Color.Magenta;
			this.toolTipController1.GetActiveObjectInfo += new DevExpress.Utils.ToolTipControllerGetActiveObjectInfoEventHandler(this.toolTipController1_GetActiveObjectInfo);
			this.panelControl.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.panelControl.Controls.Add(this.standaloneBarDockControl);
			this.panelControl.Controls.Add(this.treeList);
			resources.ApplyResources(this.panelControl, "panelControl");
			this.panelControl.Name = "panelControl";
			this.refreshFieldListBarItem.Id = 5;
			this.refreshFieldListBarItem.Name = "RefreshBarItem";
			this.refreshFieldListBarItem.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.OnRefreshFieldListBarItemClick);
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.panelControl);
			this.Controls.Add(this.barDockControlLeft);
			this.Controls.Add(this.barDockControlRight);
			this.Controls.Add(this.barDockControlBottom);
			this.Controls.Add(this.barDockControlTop);
			this.DoubleBuffered = true;
			this.Name = "DataFieldsBrowser";
			((System.ComponentModel.ISupportInitialize)(this.treeList)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.repositoryItemTextEdit)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.barManager)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.panelControl)).EndInit();
			this.panelControl.ResumeLayout(false);
			this.ResumeLayout(false);
		}
		#endregion
		private DevExpress.XtraTreeList.TreeList treeList;
		private DevExpress.XtraTreeList.Columns.TreeListColumn tlcContent;
		private DevExpress.XtraEditors.Repository.RepositoryItemTextEdit repositoryItemTextEdit;
		private System.Windows.Forms.ImageList imageList;
		private DevExpress.XtraBars.BarManager barManager;
		private DevExpress.XtraBars.Bar toolsBar;
		private DevExpress.XtraBars.BarCheckItem groupByTypeBarItem;
		private DevExpress.XtraBars.BarCheckItem sortAscendingBarItem;
		private DevExpress.XtraBars.BarCheckItem sortDescendingBarItem;
		private DevExpress.XtraBars.BarButtonItem refreshFieldListBarItem;
		private DevExpress.XtraBars.BarDockControl barDockControlTop;
		private DevExpress.XtraBars.BarDockControl barDockControlBottom;
		private DevExpress.XtraBars.BarDockControl barDockControlLeft;
		private DevExpress.XtraBars.BarDockControl barDockControlRight;
		private DevExpress.XtraEditors.PanelControl panelControl;
		private XtraTreeList.Columns.TreeListColumn tlcCaption;
		private Utils.ToolTipController toolTipController1;
		private StandaloneBarDockControl standaloneBarDockControl;
	}
}
