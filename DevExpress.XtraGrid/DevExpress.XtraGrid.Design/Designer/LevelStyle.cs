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
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using System.ComponentModel.Design;
using DevExpress.Utils.Frames;
using DevExpress.Utils.Design;
using DevExpress.XtraGrid.Design;
using DevExpress.XtraGrid.Views.Base;
namespace DevExpress.XtraGrid.Frames {
	[ToolboxItem(false)]
	public class LevelStyle : DevExpress.XtraEditors.Designer.Utils.XtraPGFrame {
		EditingGridInfo gridInfo;
		private System.ComponentModel.Container components = null;
		public DevExpress.XtraGrid.Design.DesignLevelSelector selector;
		#region Component Designer generated code
		private void InitializeComponent() {
			this.selector = new DevExpress.XtraGrid.Design.DesignLevelSelector();
			((System.ComponentModel.ISupportInitialize)(this.pnlControl)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlMain)).BeginInit();
			this.pnlMain.SuspendLayout();
			this.SuspendLayout();
			this.splMain.Location = new System.Drawing.Point(240, 60);
			this.splMain.Size = new System.Drawing.Size(4, 264);
			this.pgMain.Location = new System.Drawing.Point(244, 60);
			this.pgMain.Size = new System.Drawing.Size(216, 264);
			this.pnlControl.Size = new System.Drawing.Size(460, 32);
			this.lbCaption.Size = new System.Drawing.Size(460, 42);
			this.pnlMain.Controls.AddRange(new System.Windows.Forms.Control[] {
																				  this.selector});
			this.pnlMain.Size = new System.Drawing.Size(240, 264);
			this.horzSplitter.Size = new System.Drawing.Size(460, 4);
			this.selector.Dock = System.Windows.Forms.DockStyle.Fill;
			this.selector.EditingGrid = null;
			this.selector.Name = "selector";
			this.selector.Size = new System.Drawing.Size(240, 264);
			this.selector.TabIndex = 0;
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																		  this.pgMain,
																		  this.splMain,
																		  this.pnlMain,
																		  this.pnlControl,
																		  this.horzSplitter,
																		  this.lbCaption});
			this.Name = "LevelStyle";
			this.Size = new System.Drawing.Size(460, 324);
			((System.ComponentModel.ISupportInitialize)(this.pnlControl)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlMain)).EndInit();
			this.pnlMain.ResumeLayout(false);
			this.ResumeLayout(false);
		}
		#endregion
		protected override string DescriptionText { get { return DevExpress.XtraGrid.Design.Properties.Resources.LevelStyleDescription; } }
		public LevelStyle() : base(1) {
			this.gridInfo = null;
			InitializeComponent();
			pgMain.BringToFront();
			pnlControl.Visible = false;
		}
		protected override bool AllowGlobalStore { get { return false; } }
		private BaseView EditingView { get { return EditingObject as BaseView; } }
		protected EditingGridInfo GridInfo { get { return gridInfo; } }
		public override void InitComponent() {
			this.gridInfo = InfoObject as EditingGridInfo;
			selector.EditingGrid = GridInfo != null ? GridInfo.EditingGrid : EditingView.GridControl;
			selector.Tree.SelectionService = GridInfo;
			selector.Tree.ViewChanged += new CollectionChangeEventHandler(OnTree_ViewChanged);
			GridInfo.SelectionChanged += new EventHandler(OnGridInfo_SelectionChanged);
			RefreshPropertyGrid();
		}
		protected override object SelectedObject { get { return GridInfo.SelectedObject; } }
		protected override void Dispose(bool disposing) {
			if(disposing) {
				selector.Tree.ViewChanged -= new CollectionChangeEventHandler(OnTree_ViewChanged);
				if(GridInfo != null)
					GridInfo.SelectionChanged -= new EventHandler(OnGridInfo_SelectionChanged);
			}
			if(components != null) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		public override void RefreshFrame(bool b) {
		}
		protected virtual void OnTree_ViewChanged(object sender, CollectionChangeEventArgs e) {
			if(e.Action == CollectionChangeAction.Add) {
				GridInfo.SelectedObject = e.Element;
			} 
			if(e.Action == CollectionChangeAction.Remove) {
				if(GridInfo.SelectedView == e.Element as BaseView) {
					GridInfo.SelectedObject = GridInfo.EditingGrid.MainView;
				}
			}
		}
		protected virtual void OnGridInfo_SelectionChanged(object sender, EventArgs e) {
			RaiseRefreshWizard("", "ChangedView");
			RefreshPropertyGrid();
		}
		private void FireGridChanged(DevExpress.XtraGrid.GridControl editingGrid) {
			IComponentChangeService srv = editingGrid.InternalGetService(typeof(IComponentChangeService)) as IComponentChangeService;
			if(srv != null) {
				srv.OnComponentChanged(this, null, null, null);
			}
		}
	}
}
