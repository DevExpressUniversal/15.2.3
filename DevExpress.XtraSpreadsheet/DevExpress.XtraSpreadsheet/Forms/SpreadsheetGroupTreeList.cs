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
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using DevExpress.Data;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.Utils.Menu;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraTreeList;
using DevExpress.XtraTreeList.Columns;
using DevExpress.XtraTreeList.Localization;
using DevExpress.XtraTreeList.Native;
using DevExpress.XtraTreeList.Nodes;
namespace DevExpress.XtraSpreadsheet.Forms {
	public class SpreadsheetGroupTreeList : TreeList {
		#region const
		public const string FieldName = "FieldName";
		public const string SortOrderField = "SortOrder";
		#endregion
		#region static
		static void InitSortOrderComboBox(RepositoryItemComboBox comboBox) {
			comboBox.Name = SortOrderField;
			comboBox.AllowFocused = false;
			comboBox.AutoHeight = false;
			comboBox.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
				new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
			comboBox.Items.AddRange(new DevExpress.XtraEditors.Controls.ComboBoxItem[] {
			new DevExpress.XtraEditors.Controls.ComboBoxItem(ColumnSortOrder.Ascending),
			new DevExpress.XtraEditors.Controls.ComboBoxItem(ColumnSortOrder.Descending)
			});
			comboBox.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
			comboBox.ReadOnly = false;
		}
		static void InitFiledNameComboBox(RepositoryItemComboBox comboBox) {
			comboBox.Name = FieldName;
			comboBox.AllowFocused = false;
			comboBox.AutoHeight = false;
			comboBox.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
			comboBox.ReadOnly = false;
		}
		#endregion
		#region inner classes
		class GroupNode :XtraListNode {
			EditGroupInfo info;
			public GroupNode(EditGroupInfo info, TreeListNodes owner)
				: base(info.DefinedName, owner) {
				this.info = info;
			}
			public EditGroupInfo GroupInfo { get { return info; } }
			public override object GetValue(object columnID) {
				switch ((columnID as TreeListColumn).FieldName) {
					case SpreadsheetGroupTreeList.FieldName:
						return info.FieldName;
					case SpreadsheetGroupTreeList.SortOrderField:
						return info.SortOrder;
				}
				return base.GetValue(columnID);
			}
			public override void SetValue(object columnID, object value) {
				switch ((columnID as TreeListColumn).FieldName) {
					case SpreadsheetGroupTreeList.FieldName:
						info.FieldName = value as string;
						return;
					case SpreadsheetGroupTreeList.SortOrderField:
						info.SortOrder = (ColumnSortOrder)value;
						return;
				}
			}
		}
		class FieldNameRepositoryItem :RepositoryItemComboBox {
			#region fields
			PopupContainerEdit edit;
			SpreadsheetFieldListTreeView fieldList;
			SpreadsheetGroupTreeList treeList;
			SpreadsheetControl control;
			string dataMember;
			#endregion
			public FieldNameRepositoryItem(SpreadsheetGroupTreeList treeList, SpreadsheetControl control, string dataMember) {
				edit = new PopupContainerEdit();
				edit.Properties.PopupControl = new PopupContainerControl();
				this.treeList = treeList;
				this.control = control;
				this.dataMember = dataMember;
				this.QueryPopUp += FieldNameRepositoryItem_QueryPopUp;
				this.Closed += FieldNameRepositoryItem_Closed;
				this.CustomDisplayText += FieldNameRepositoryItem_CustomDisplayText;
			}
			public override BaseEdit CreateEditor() {
				return edit;
			}
			protected override void Dispose(bool disposing) {
				if (disposing) {
					this.QueryPopUp -= FieldNameRepositoryItem_QueryPopUp;
					this.Closed -= FieldNameRepositoryItem_Closed;
					this.CustomDisplayText -= FieldNameRepositoryItem_CustomDisplayText;
				}
				base.Dispose(disposing);
			}
			protected SpreadsheetFieldListTreeView CreateFieldList() {
				SpreadsheetFieldListTreeView fieldList = new SpreadsheetFieldListTreeView();
				fieldList.Location = new System.Drawing.Point(0, 0);
				fieldList.Name = "fieldList";
				fieldList.Size = new System.Drawing.Size(260, 200);
				fieldList.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.Simple;
				fieldList.SpreadsheetControl = control;
				fieldList.NeedDoubleClick = false;
				fieldList.ShowComplexChildren = false;
				fieldList.DataMember = dataMember;
				fieldList.RefreshTreeList();
				return fieldList;
			}
			void FieldNameRepositoryItem_QueryPopUp(object sender, CancelEventArgs e) {
				bool cancel = true;
				try {
					if (fieldList == null || fieldList.IsDisposed) {
						fieldList = CreateFieldList();
						if (fieldList != null) {
							fieldList.Width = edit.Width;
							edit.Properties.PopupControl.Controls.Add(fieldList);
							fieldList.Dock = DockStyle.Fill;
							fieldList.SelectNode(edit.EditValue as string);
							fieldList.MouseDoubleClick += fieldList_MouseDoubleClick;
						}
					}
					cancel = false;
				}
				finally {
					e.Cancel = cancel;
				}
			}
			void FieldNameRepositoryItem_Closed(object sender, ClosedEventArgs e) {
				if (fieldList != null && !fieldList.SelectedNode.HasChildren) {
					SetFieldToEdit();
					fieldList.MouseDoubleClick -= fieldList_MouseDoubleClick;
					fieldList.Dispose();
				}
			}
			void FieldNameRepositoryItem_CustomDisplayText(object sender, CustomDisplayTextEventArgs e) {
				if (treeList != null)
					e.DisplayText = control.DocumentModel.GetMailMergeDisplayName((string)e.Value, false);
			}
			void fieldList_MouseDoubleClick(object sender, MouseEventArgs e) {
				SetFieldToEdit();
			}
			void SetFieldToEdit() {
				if (fieldList.SelectedNode.HasChildren)
					return;
				string[] result = fieldList.SelectedNodes();
				if (result.Length > 0)
					edit.EditValue = result[0];
			}
		}
		#endregion
		#region fields
		RepositoryItemComboBox sortOrderComboBox;
		RepositoryItemComboBox fieldNameComboBox;
		EditGroupInfo currentInfo;
		SpreadsheetControl control;
		string dataMember;
		TreeListColumn nameColumn;
		TreeListColumn sortOrderColumn;
		List<EditGroupInfo> groupInfo;
		#endregion
		public SpreadsheetGroupTreeList(SpreadsheetControl control, string dataMember)
			: base(null) {
			this.control = control;
			this.dataMember = dataMember;
			InitializeComponent();
			SubscribeEvents();
		}
		void InitializeComponent() {
			this.nameColumn = new DevExpress.XtraTreeList.Columns.TreeListColumn();
			this.sortOrderColumn = new DevExpress.XtraTreeList.Columns.TreeListColumn();
			this.fieldNameComboBox = new FieldNameRepositoryItem(this, control, dataMember);
			this.sortOrderComboBox = new RepositoryItemComboBox();
			((System.ComponentModel.ISupportInitialize)(sortOrderComboBox)).BeginInit();
			this.nameColumn.Caption = "Field Name";
			this.nameColumn.FieldName = SpreadsheetGroupTreeList.FieldName;
			this.nameColumn.Name = "nameColumn";
			this.nameColumn.OptionsColumn.AllowMove = false;
			this.nameColumn.OptionsColumn.AllowSort = false;
			this.nameColumn.Visible = true;
			this.nameColumn.VisibleIndex = 1;
			this.nameColumn.Width = 161;
			this.sortOrderColumn.Caption = "Sort Order";
			this.sortOrderColumn.FieldName = SpreadsheetGroupTreeList.SortOrderField;
			this.sortOrderColumn.Name = "sortOrderColumn";
			this.sortOrderColumn.OptionsColumn.AllowMove = false;
			this.sortOrderColumn.OptionsColumn.AllowSort = false;
			this.sortOrderColumn.Visible = true;
			this.sortOrderColumn.VisibleIndex = 2;
			this.sortOrderColumn.Width = 121;
			this.Columns.AddRange(new DevExpress.XtraTreeList.Columns.TreeListColumn[] {
			this.nameColumn,
			this.sortOrderColumn});
			InitFiledNameComboBox(fieldNameComboBox);
			InitSortOrderComboBox(sortOrderComboBox);
			this.OptionsView.ShowButtons = false;
			this.OptionsView.ShowRoot = false;
			this.OptionsView.ShowIndicator = false;
			this.OptionsBehavior.CloseEditorOnLostFocus = false;
			this.OptionsBehavior.AllowExpandOnDblClick = false;
			this.OptionsBehavior.ImmediateEditor = true;
			this.Appearance.FocusedRow.TextOptions.HAlignment = HorzAlignment.Near;
			this.Appearance.HideSelectionRow.TextOptions.HAlignment = HorzAlignment.Near;
			this.Appearance.Row.TextOptions.HAlignment = HorzAlignment.Near;
			this.OptionsView.AutoWidth = false;
			this.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
				fieldNameComboBox, sortOrderComboBox});
			((System.ComponentModel.ISupportInitialize)(sortOrderComboBox)).EndInit();
		}
		public void FillGroupInfo(List<EditGroupInfo> groupInfo) {
			this.groupInfo = groupInfo;
			FillGroupInfoCore(null);
		}
		public List<EditGroupInfo> GetGroupInfo() {
			return GetGroupInfo(this.Nodes);
		}
		public void MoveNodeDown() {
			EditGroupInfo info = (FocusedNode as GroupNode).GroupInfo;
			int index = groupInfo.IndexOf(info);
			if (index < groupInfo.Count - 1) {
				index++;
				groupInfo.Remove(info);
				groupInfo.Insert(index, info);
				FillGroupInfoCore(info);
			}
		}
		public void MoveNodeUp() {
			EditGroupInfo info = (FocusedNode as GroupNode).GroupInfo;
			int index = groupInfo.IndexOf(info);
			if (index > 0) {
				index--;
				groupInfo.Remove(info);
				groupInfo.Insert(index, info);
				FillGroupInfoCore(info);
			}
		}
		public void CreateNewNode() {
			groupInfo.Add(new EditGroupInfo());
			FillGroupInfoCore(null);
		}
		public void DeleteNode() {
			if (FocusedNode == null)
				return;
			groupInfo.Remove((FocusedNode as GroupNode).GroupInfo);
			FillGroupInfoCore(null);
		}
		protected override void RaiseCustomDrawColumnHeader(XtraTreeList.CustomDrawColumnHeaderEventArgs e) {
			base.RaiseCustomDrawColumnHeader(e);
			(e.ObjectArgs as ObjectInfoArgs).State = ObjectState.Normal;
		}
		protected override TreeListNode CreateNode(int nodeID, TreeListNodes owner, object tag) {
			return new GroupNode(currentInfo, owner);
		}
		protected override void SetNodeValue(TreeListNode node, object columnId, object val, bool initEdit) {
			node.SetValue(columnId, val);
			base.SetNodeValue(node, columnId, val, initEdit);
		}
		protected override void Dispose(bool disposing) {
			UnsubscribeEvents();
			base.Dispose(disposing);
		}
		void FillGroupInfoCore(EditGroupInfo selectedInfo) {
			this.Nodes.Clear();
			TreeListNode parentNode = null;
			foreach (EditGroupInfo info in groupInfo) {
				currentInfo = info;
				parentNode = AppendNode(null, parentNode);
				parentNode.Expanded = true;
				if (info == selectedInfo)
					SetFocusedNode(parentNode);
			}
		}
		List<EditGroupInfo> GetGroupInfo(TreeListNodes nodes) {
			List<EditGroupInfo> result = new List<EditGroupInfo>();
			foreach (GroupNode node in nodes) {
				result.Add(node.GroupInfo);
				if (node.HasChildren)
					result.AddRange(GetGroupInfo(node.Nodes));
			}
			return result;
		}
		void SubscribeEvents() {
			foreach (RepositoryItem item in this.RepositoryItems)
				item.EditValueChanged += item_EditValueChanged;
			this.PopupMenuShowing += SpreadsheetGroupTreeList_PopupMenuShowing;
			this.CustomNodeCellEdit += SpreadsheetGroupTreeList_CustomNodeCellEdit;
			this.NodeChanged += SpreadsheetGroupTreeList_NodeChanged;
			sortOrderComboBox.CustomDisplayText += sortOrderComboBox_CustomDisplayText;
		}
		void UnsubscribeEvents() {
			foreach (RepositoryItem item in this.RepositoryItems)
				item.EditValueChanged -= item_EditValueChanged;
			this.PopupMenuShowing -= SpreadsheetGroupTreeList_PopupMenuShowing;
			this.CustomNodeCellEdit -= SpreadsheetGroupTreeList_CustomNodeCellEdit;
			this.NodeChanged -= SpreadsheetGroupTreeList_NodeChanged;
			sortOrderComboBox.CustomDisplayText -= sortOrderComboBox_CustomDisplayText;
		}
		void item_EditValueChanged(object sender, EventArgs e) {
			this.PostEditor();
		}
		void SpreadsheetGroupTreeList_CustomNodeCellEdit(object sender, GetCustomNodeCellEditEventArgs e) {
			switch (e.Column.FieldName) {
				case FieldName:
					e.RepositoryItem = fieldNameComboBox;
					break;
				case SortOrderField:
					e.RepositoryItem = sortOrderComboBox;
					break;
				default:
					break;
			}
		}
		void SpreadsheetGroupTreeList_NodeChanged(object sender, NodeChangedEventArgs e) {
			if (e.Node.ParentNode != null && e.ChangeType == NodeChangeTypeEnum.Add)
				e.Node.ParentNode.Expanded = true;
		}
		void sortOrderComboBox_CustomDisplayText(object sender, CustomDisplayTextEventArgs e) {
			Enum value = e.Value as Enum;
			if (value != null)
				e.DisplayText = value.ToString();
		}
		void SpreadsheetGroupTreeList_PopupMenuShowing(object sender, XtraTreeList.PopupMenuShowingEventArgs e) {
			TreeListHitInfo hitInfo = CalcHitInfo(e.Point);
			if (hitInfo.HitInfoType != HitInfoType.Column && hitInfo.HitInfoType != HitInfoType.BehindColumn)
				return;
			List<TreeListStringId> ignoreItems = new List<TreeListStringId>(new TreeListStringId[] {
				TreeListStringId.MenuColumnColumnCustomization,
				TreeListStringId.MenuColumnSortAscending,
				TreeListStringId.MenuColumnSortDescending
			});
			foreach (DXMenuItem item in e.Menu.Items) {
				if (!(item.Tag is TreeListStringId))
					continue;
				TreeListStringId treeListStringId = (TreeListStringId)item.Tag;
				if (!ignoreItems.Contains(treeListStringId))
					continue;
				item.Enabled = false;
				item.Visible = false;
				item.BeginGroup = false;
				ignoreItems.Remove(treeListStringId);
				if (ignoreItems.Count == 0)
					break;
			}
			if (hitInfo.HitInfoType == HitInfoType.BehindColumn)
				e.Menu.Items[e.Menu.Items.Count - 1].BeginGroup = false;
		}
	}
}
