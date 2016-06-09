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
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.Data;
using DevExpress.Data.Browsing;
using DevExpress.Data.Design;
using DevExpress.Snap.Core;
using DevExpress.Snap.Core.Forms;
using DevExpress.Snap.Localization;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraPrinting.Native;
using DevExpress.XtraReports.Native;
using DevExpress.XtraTreeList.Nodes;
#region FxCop Suppressions
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.Snap.Forms.SortingFormBase.checkEditShowGroupSortings")]
#endregion
namespace DevExpress.Snap.Forms {
	public partial class SortingFormBase : XtraForm {
		readonly ISnapControl control;
		readonly SortingFormControllerBase controller;
		RepositoryItemPopupContainerEdit repositoryItemP;
		RepositoryItemComboBox repositoryItemC;
		DataSourceNativeTreeList tl;
		public SortingFormBase() {
			InitializeComponent();
		}
		public SortingFormBase(SortingFormControllerBaseParameters controllerParameters) {
			Guard.ArgumentNotNull(controllerParameters, "controllerParameters");
			this.control = controllerParameters.Control;
			this.controller = CreateController(controllerParameters);
			InitializeComponent();
			InitializeTreeList();
			InitializeGridView();
			UpdateForm();
			SetDefaultSorting();
		}
		public ISnapControl Control { get { return control; } }
		public SortingFormControllerBase Controller { get { return controller; } }
		protected SortField DefaultSorting { get; set; }
		protected RepositoryItemPopupContainerEdit RepositoryItemPopup { get { return repositoryItemP; } }
		protected DataSourceNativeTreeList TreeList { get { return tl; } }
		protected internal virtual SortingFormControllerBase CreateController(SortingFormControllerBaseParameters controllerParameters) {
			throw new NotImplementedException();
		}
		protected internal virtual void UpdateForm() {
			UnsubscribeControlsEvents();
			try {
				UpdateFormCore();
			}
			finally {
				SubscribeControlsEvents();
			}
		}
		protected internal virtual void SubscribeControlsEvents() {
			gridView1.FocusedRowChanged += gridView1_FocusedRowChanged;
			gridView1.ShowingEditor += gridView1_ShowingEditor;
			gridView1.RowCellStyle += gridView1_RowCellStyle;
			gridView1.CustomDrawCell += gridView1_CustomDrawCell;
			repositoryItemP.QueryPopUp += repositoryItemP_QueryPopUp;
			repositoryItemP.QueryResultValue += repositoryItemP_QueryResultValue;
			tl.MouseUp += tl_MouseUp;
		}
		protected internal virtual void UnsubscribeControlsEvents() {
			gridView1.FocusedRowChanged -= gridView1_FocusedRowChanged;
			gridView1.ShowingEditor -= gridView1_ShowingEditor;
			gridView1.RowCellStyle -= gridView1_RowCellStyle;
			gridView1.CustomDrawCell -= gridView1_CustomDrawCell;
			repositoryItemP.QueryPopUp -= repositoryItemP_QueryPopUp;
			repositoryItemP.QueryResultValue -= repositoryItemP_QueryResultValue;
			tl.MouseUp -= tl_MouseUp;
		}
		protected internal virtual void UpdateFormCore() {
			this.Text = SnapLocalizer.GetString(SnapStringId.CustomSortForm_Text);
			checkEditShowGroupSortings.Text = SnapLocalizer.GetString(SnapStringId.ShowGroupSortingsCheckBox_Text);
			UpdateUpDownRemoveButtons();
		}
		private void UpdateUpDownRemoveButtons() {
			int index = gridView1.GetDataSourceRowIndex(gridView1.FocusedRowHandle);
			if(index < 0 || Controller.SortList[index].IsGroupSorting) {
				btnUp.Enabled = btnDown.Enabled = btnRemoveLevel.Enabled = false;
				btnRemoveLevel.ForeColor = Color.LightGray;
			}
			else {
				btnUp.Enabled = index > 0 && !Controller.SortList[index - 1].IsGroupSorting;
				btnDown.Enabled = index < Controller.SortList.Count - 1;
				btnRemoveLevel.Enabled = true;
				btnRemoveLevel.ForeColor = SystemColors.ControlText;
			}
		}
		private void InitializeTreeList() {
			tl = new DataSourceNativeTreeList(new TreeListPickManager(new DataContextOptions(true, true))) { Dock = DockStyle.Fill };
			UpdateDataSource();
			tl.ShowParametersNode = false;
			TreeListNode rootNode = tl.FindNode(i => { DataMemberListNodeBase node = i as DataMemberListNodeBase; return node != null ? node.DataMember == controller.ListDataMember : false; });
			TreeListNodes nodes;
			if (rootNode != null) {
				nodes = rootNode.Nodes;
				rootNode.Expanded = true;
				while (rootNode.ParentNode != null) {
					foreach (TreeListNode node in rootNode.ParentNode.Nodes)
						if (node != rootNode) node.Visible = false;
					rootNode = rootNode.ParentNode;
				}
			}
			else {
				if (tl.Nodes.Count > 0)
					nodes = tl.Nodes[0].Nodes;
				else
					return;
			}
			foreach(TreeListNode node in nodes) {
				DataMemberListNode dataMemberNode = node as DataMemberListNode;
				if(dataMemberNode != null && dataMemberNode.IsList)
					dataMemberNode.Visible = false;
			}
		}
		protected virtual void UpdateDataSource() {
			IDataSourceCollectorService dataSourceCollector = control.GetService<IDataSourceCollectorService>();
			if(dataSourceCollector != null) {
				tl.UpdateDataSource(control, dataSourceCollector.GetDataSources());
			}
		}
		private void InitializeGridView() {
			gridControl1.DataSource = Controller.SortList;
			gridView1.Columns[0].Caption = SnapLocalizer.GetString(SnapStringId.SortingForm_SortByColumnCaption);
			gridView1.Columns[1].Caption = SnapLocalizer.GetString(SnapStringId.SortingForm_OrderColumnCaption);
			for(int i = 2; i < gridView1.Columns.Count; i++)
				gridView1.Columns[i].Visible = false;
			gridView1.Columns[1].AppearanceCell.TextOptions.HAlignment = HorzAlignment.Near;
			gridView1.ActiveFilter.NonColumnFilter = "[IsGroupSorting] = 'false'";
			repositoryItemP = new RepositoryItemPopupContainerEdit();
			PopupContainerControl popupContainer = new PopupContainerControl();
			popupContainer.Height = 135;
			repositoryItemP.PopupControl = popupContainer;
			repositoryItemP.ShowDropDown = XtraEditors.Controls.ShowDropDown.Never;
			repositoryItemP.PopupControl.Controls.Add(tl);
			gridView1.Columns[0].ColumnEdit = repositoryItemP;
			repositoryItemC = new RepositoryItemComboBox();
			repositoryItemC.Items.Add(ColumnSortOrder.Ascending);
			repositoryItemC.Items.Add(ColumnSortOrder.Descending);
			repositoryItemC.TextEditStyle = XtraEditors.Controls.TextEditStyles.DisableTextEditor;
			gridView1.Columns[1].ColumnEdit = repositoryItemC;
		}
		private void SetDefaultSorting() {
			tl.FilterNodes();
			DefaultSorting = new SortField();
			DataMemberListNode dataMemberNode = (DataMemberListNode)tl.FindNode(n => (n.Nodes.Count == 0 && n.Visible && n as DataMemberListNode != null));
			if(dataMemberNode != null) {
				DefaultSorting.DisplayName = dataMemberNode.GetDisplayText(null);
				DefaultSorting.FieldName = dataMemberNode.DataMember;
			}
			if(Controller.SortList.IsEmpty())
				return;
			if(String.IsNullOrEmpty(Controller.SortList[Controller.SortList.Count - 1].DisplayName))
				Controller.SortList[Controller.SortList.Count - 1] = DefaultSorting.Clone();
		}
		private void btnOk_Click(object sender, EventArgs e) {
			Controller.RemoveEmptySortListElements();
			for(int i = 0; i < Controller.SortList.Count - 1; i++)
				for(int k = i + 1; k < Controller.SortList.Count; k++)
					if(Controller.SortList[i].DisplayName == Controller.SortList[k].DisplayName) {
						string message = Controller.SortList[i].IsGroupSorting ? SnapLocalizer.GetString(SnapStringId.Msg_FieldAlreadyDefinedAsGroupingCriterion) : SnapLocalizer.GetString(SnapStringId.Msg_FieldDefinedAsSortingCriterionMoreThanOnce);
						XtraMessageBox.Show(LookAndFeel, this, String.Format(message, Controller.SortList[i].DisplayName), this.Text = SnapLocalizer.GetString(SnapStringId.CustomSortForm_Text), MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
						return;
					}
			Controller.ApplyChanges();
			this.DialogResult = System.Windows.Forms.DialogResult.OK;
		}
		private void btnUp_Click(object sender, EventArgs e) {
			int newRowHandle = gridView1.FocusedRowHandle - 1;
			int index = gridView1.GetDataSourceRowIndex(gridView1.FocusedRowHandle);
			Controller.SwapSortListElements(index, index - 1);
			gridView1.FocusedRowHandle = newRowHandle;
			UpdateUpDownRemoveButtons();
		}
		private void btnDown_Click(object sender, EventArgs e) {
			int newRowHandle = gridView1.FocusedRowHandle + 1;
			int index = gridView1.GetDataSourceRowIndex(gridView1.FocusedRowHandle);
			Controller.SwapSortListElements(index, index + 1);
			gridView1.FocusedRowHandle = newRowHandle;
			UpdateUpDownRemoveButtons();
		}
		private void btnAddLevel_Click(object sender, EventArgs e) {
			Controller.SortList.Add(DefaultSorting.Clone());
			gridView1.MoveLast();
		}
		private void btnRemoveLevel_Click(object sender, EventArgs e) {
			gridView1.DeleteSelectedRows();
		}
		private void gridView1_FocusedRowChanged(object sender, FocusedRowChangedEventArgs e) {
			UpdateUpDownRemoveButtons();
		}
		private void gridView1_ShowingEditor(object sender, CancelEventArgs e) {
			int index = gridView1.GetDataSourceRowIndex(gridView1.FocusedRowHandle);
			if(gridView1.FocusedColumn.AbsoluteIndex == 0 && Controller.SortList[index].IsGroupSorting)
				e.Cancel = true;
		}
		void gridView1_RowCellStyle(object sender, RowCellStyleEventArgs e) {
			if(Controller.SortList[gridView1.GetDataSourceRowIndex(e.RowHandle)].IsGroupSorting) {
				e.Appearance.BackColor = Color.FromArgb(190, 190, 190);
			}
		}
		void gridView1_CustomDrawCell(object sender, RowCellCustomDrawEventArgs e) {
			if(e.Column.AbsoluteIndex != 0 || !Controller.SortList[gridView1.GetDataSourceRowIndex(e.RowHandle)].IsGroupSorting)
				return;
			e.Appearance.DrawBackground(e.Cache, new Rectangle(e.Bounds.X - 1, e.Bounds.Y - 1, e.Bounds.Width + 2, e.Bounds.Height + 2));
			e.Appearance.DrawString(e.Cache, e.DisplayText, e.Bounds);
			e.Handled = true;
		}
		private void repositoryItemP_QueryPopUp(object sender, CancelEventArgs e) {
			repositoryItemP.PopupControl.Width = gridView1.Columns[0].VisibleWidth - 4;
		}
		private void repositoryItemP_QueryResultValue(object sender, QueryResultValueEventArgs e) {
			if(tl.FocusedNode != null && tl.FocusedNode.Nodes.Count == 0) {
				DataMemberListNode dataMemberNode = tl.FocusedNode as DataMemberListNode;
				if(dataMemberNode != null) {
					string fieldName = Controller.GetFieldName(dataMemberNode.DataMember);
					gridView1.SetFocusedRowCellValue(gridView1.Columns[2], fieldName);
					e.Value = Controller.GetDisplayName(fieldName);
				}
				tl.FocusedNode = null;
			}
		}
		private void tl_MouseUp(object sender, MouseEventArgs e) {
			if(tl.FocusedNode != null && tl.FocusedNode.Nodes.Count == 0)
				repositoryItemP.PopupControl.OwnerEdit.ClosePopup();
		}
		private void CustomSortForm_FormClosed(object sender, FormClosedEventArgs e) {
			if(repositoryItemP == null || repositoryItemP.IsDisposed) return;
			repositoryItemP.Dispose();
		}
		private void checkEdit1_CheckedChanged(object sender, EventArgs e) {
			if(checkEditShowGroupSortings.Checked)
				gridView1.ActiveFilter.Clear();
			else
				gridView1.ActiveFilter.NonColumnFilter = "[IsGroupSorting] = 'false'";
			UpdateUpDownRemoveButtons();
		}
	}
}
