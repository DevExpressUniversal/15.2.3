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

namespace DevExpress.XtraPrinting.Design.RemoteDocumentSourceDesign.Views {
	partial class ChooseRemoteReportView {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if(disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Component Designer generated code
		private void InitializeComponent() {
			this.splitContainerControl1 = new DevExpress.XtraEditors.SplitContainerControl();
			this.categoriesList = new DevExpress.XtraTreeList.TreeList();
			this.treeListColumn1 = new DevExpress.XtraTreeList.Columns.TreeListColumn();
			this.reportsList = new DevExpress.XtraTreeList.TreeList();
			this.treeListColumn2 = new DevExpress.XtraTreeList.Columns.TreeListColumn();
			this.treeListColumn3 = new DevExpress.XtraTreeList.Columns.TreeListColumn();
			this.treeListColumn4 = new DevExpress.XtraTreeList.Columns.TreeListColumn();
			this.treeListColumn5 = new DevExpress.XtraTreeList.Columns.TreeListColumn();
			((System.ComponentModel.ISupportInitialize)(this.splitContainerControl1)).BeginInit();
			this.splitContainerControl1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.categoriesList)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.reportsList)).BeginInit();
			this.SuspendLayout();
			this.splitContainerControl1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainerControl1.Location = new System.Drawing.Point(0, 0);
			this.splitContainerControl1.Name = "splitContainerControl1";
			this.splitContainerControl1.Panel1.Controls.Add(this.categoriesList);
			this.splitContainerControl1.Panel1.Text = "Panel1";
			this.splitContainerControl1.Panel2.Controls.Add(this.reportsList);
			this.splitContainerControl1.Panel2.Text = "Panel2";
			this.splitContainerControl1.Size = new System.Drawing.Size(776, 422);
			this.splitContainerControl1.SplitterPosition = 174;
			this.splitContainerControl1.TabIndex = 0;
			this.splitContainerControl1.Text = "splitContainerControl1";
			this.categoriesList.Columns.AddRange(new DevExpress.XtraTreeList.Columns.TreeListColumn[] {
			this.treeListColumn1});
			this.categoriesList.Dock = System.Windows.Forms.DockStyle.Fill;
			this.categoriesList.Location = new System.Drawing.Point(0, 0);
			this.categoriesList.Name = "categoriesList";
			this.categoriesList.OptionsBehavior.AllowIncrementalSearch = true;
			this.categoriesList.OptionsBehavior.Editable = false;
			this.categoriesList.OptionsBehavior.EnableFiltering = true;
			this.categoriesList.OptionsFilter.FilterMode = DevExpress.XtraTreeList.FilterMode.Standard;
			this.categoriesList.OptionsLayout.AddNewColumns = false;
			this.categoriesList.OptionsMenu.EnableFooterMenu = false;
			this.categoriesList.OptionsSelection.EnableAppearanceFocusedCell = false;
			this.categoriesList.OptionsView.FocusRectStyle = XtraTreeList.DrawFocusRectStyle.None;
			this.categoriesList.OptionsView.ShowHorzLines = false;
			this.categoriesList.OptionsView.ShowIndicator = false;
			this.categoriesList.OptionsView.ShowRoot = false;
			this.categoriesList.OptionsView.ShowVertLines = false;
			this.categoriesList.Size = new System.Drawing.Size(174, 422);
			this.categoriesList.TabIndex = 1;
			this.categoriesList.FocusedNodeChanged += new DevExpress.XtraTreeList.FocusedNodeChangedEventHandler(this.categoriesList_FocusedNodeChanged);
			this.treeListColumn1.Caption = "Category";
			this.treeListColumn1.FieldName = "Name";
			this.treeListColumn1.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
			this.treeListColumn1.Name = "treeListColumn1";
			this.treeListColumn1.SortMode = DevExpress.XtraGrid.ColumnSortMode.DisplayText;
			this.treeListColumn1.SortOrder = System.Windows.Forms.SortOrder.Ascending;
			this.treeListColumn1.Visible = true;
			this.treeListColumn1.VisibleIndex = 0;
			this.reportsList.Columns.AddRange(new DevExpress.XtraTreeList.Columns.TreeListColumn[] {
			this.treeListColumn2,
			this.treeListColumn3,
			this.treeListColumn4,
			this.treeListColumn5});
			this.reportsList.Dock = System.Windows.Forms.DockStyle.Fill;
			this.reportsList.Location = new System.Drawing.Point(0, 0);
			this.reportsList.Name = "reportsList";
			this.reportsList.OptionsBehavior.AllowIncrementalSearch = true;
			this.reportsList.OptionsBehavior.Editable = false;
			this.reportsList.OptionsBehavior.EnableFiltering = true;
			this.reportsList.OptionsFind.AllowFindPanel = true;
			this.reportsList.OptionsSelection.EnableAppearanceFocusedCell = false;
			this.reportsList.OptionsView.FocusRectStyle = XtraTreeList.DrawFocusRectStyle.None;
			this.reportsList.OptionsView.ShowIndicator = false;
			this.reportsList.OptionsView.ShowRoot = false;
			this.reportsList.Size = new System.Drawing.Size(597, 422);
			this.reportsList.TabIndex = 2;
			this.reportsList.FocusedNodeChanged += new DevExpress.XtraTreeList.FocusedNodeChangedEventHandler(this.reportsList_FocusedNodeChanged);
			this.treeListColumn2.Caption = "Id";
			this.treeListColumn2.FieldName = "Id";
			this.treeListColumn2.Name = "treeListColumn2";
			this.treeListColumn2.Visible = true;
			this.treeListColumn2.VisibleIndex = 0;
			this.treeListColumn2.Width = 47;
			this.treeListColumn3.Caption = "Name";
			this.treeListColumn3.FieldName = "Name";
			this.treeListColumn3.Name = "treeListColumn3";
			this.treeListColumn3.SortOrder = System.Windows.Forms.SortOrder.Ascending;
			this.treeListColumn3.Visible = true;
			this.treeListColumn3.VisibleIndex = 1;
			this.treeListColumn3.Width = 151;
			this.treeListColumn4.Caption = "Modified Date Time";
			this.treeListColumn4.FieldName = "ModifiedDateTime";
			this.treeListColumn4.Format.FormatString = "dd-MM-yyyy HH:mm:ss";
			this.treeListColumn4.Format.FormatType = DevExpress.Utils.FormatType.DateTime;
			this.treeListColumn4.Name = "treeListColumn4";
			this.treeListColumn4.Visible = true;
			this.treeListColumn4.VisibleIndex = 2;
			this.treeListColumn4.Width = 168;
			this.treeListColumn5.Caption = "Description";
			this.treeListColumn5.FieldName = "Description";
			this.treeListColumn5.Name = "treeListColumn5";
			this.treeListColumn5.Visible = true;
			this.treeListColumn5.VisibleIndex = 3;
			this.treeListColumn5.Width = 167;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.splitContainerControl1);
			this.Name = "ChooseRemoteReportView";
			this.Size = new System.Drawing.Size(776, 422);
			((System.ComponentModel.ISupportInitialize)(this.splitContainerControl1)).EndInit();
			this.splitContainerControl1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.categoriesList)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.reportsList)).EndInit();
			this.ResumeLayout(false);
		}
		#endregion
		private XtraEditors.SplitContainerControl splitContainerControl1;
		private XtraTreeList.TreeList reportsList;
		private XtraTreeList.Columns.TreeListColumn treeListColumn2;
		private XtraTreeList.Columns.TreeListColumn treeListColumn3;
		private XtraTreeList.Columns.TreeListColumn treeListColumn4;
		private XtraTreeList.Columns.TreeListColumn treeListColumn5;
		private XtraTreeList.TreeList categoriesList;
		private XtraTreeList.Columns.TreeListColumn treeListColumn1;
	}
}
