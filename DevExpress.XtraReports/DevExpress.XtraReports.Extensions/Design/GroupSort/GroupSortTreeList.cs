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
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraReports.UI;
using DevExpress.XtraReports.Design.Tools;
using DevExpress.XtraTreeList;
using DevExpress.XtraTreeList.Nodes;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.XtraTreeList.Localization;
using DevExpress.Utils.Menu;
using DevExpress.XtraTreeList.Handler;
using DevExpress.XtraVerticalGrid;
using DevExpress.XtraTreeList.Menu;
using DevExpress.XtraEditors.Controls;
namespace DevExpress.XtraReports.Design {
	abstract class FieldNameRepositoryItem : RepositoryItemComboBox {
		protected PopupContainerEdit edit;
		IPopupFieldNamePicker picker;
		public FieldNameRepositoryItem() {
			edit = new PopupContainerEdit();
			edit.Properties.PopupControl = new PopupContainerControl();
			this.QueryPopUp += new CancelEventHandler(PopupRepositoryItemComboBox_QueryPopUp);
			this.Closed += new ClosedEventHandler(PopupRepositoryItemComboBox_Closed);
		}
		void PopupRepositoryItemComboBox_QueryPopUp(object sender, CancelEventArgs e) {
			bool cancel = true;
			try {
				if(picker == null || picker.IsDisposed) {
					picker = CreatePicker();
					if(picker != null) {
						picker.Width = edit.Width;
						StartPicker(picker);
					}
				}
				cancel = false;
			} finally {
				e.Cancel = cancel;
			}
		}
		protected virtual IPopupFieldNamePicker CreatePicker() {
			return new PopupFieldNamePicker();
		}
		protected abstract void StartPicker(IPopupFieldNamePicker picker);
		void PopupRepositoryItemComboBox_Closed(object sender, ClosedEventArgs e) {
			if(picker != null) {
				edit.EditValue = picker.EndFieldNamePicker();
				picker.Dispose();
			}
		}
		protected override void Dispose(bool disposing) {
			if(disposing) {
				this.QueryPopUp -= new CancelEventHandler(PopupRepositoryItemComboBox_QueryPopUp);
				this.Closed -= new ClosedEventHandler(PopupRepositoryItemComboBox_Closed);
				if(edit != null) {
					edit.Dispose();
					edit = null;
				}
			}
			base.Dispose(disposing);
		}
	}
}
namespace DevExpress.XtraReports.Design.GroupSort {
	[System.ComponentModel.ToolboxItem(false)]
	public class GroupSortTreeList : TreeList, ISupportController {
		#region inner classes
		class XRPopupRepositoryItemComboBox : FieldNameRepositoryItem {
			GroupSortTreeList treeList;
			public XRPopupRepositoryItemComboBox(GroupSortTreeList treeList) {
				this.treeList = treeList;
				this.CustomDisplayText += new CustomDisplayTextEventHandler(PopupRepositoryItemComboBox_CustomDisplayText);
			}
			protected override IPopupFieldNamePicker CreatePicker() {
				return Controller.CreatePopupFieldNamePicker();
			}
			protected override void StartPicker(IPopupFieldNamePicker picker) {
				XtraReportBase report = Controller.Report;
				string fieldName = GetFieldName(treeList.FocusedNode);
				picker.Start(Controller, report.GetEffectiveDataSource(), report.DataMember, fieldName, edit);
			}
			string GetFieldName(TreeListNode node) {
				return node != null && Controller != null ? 
					Controller.GetFieldName(node.Id) : string.Empty;
			}
			GroupSortController Controller {
				get { return treeList.GroupSortController; }
			}
			void PopupRepositoryItemComboBox_CustomDisplayText(object sender, CustomDisplayTextEventArgs e) {
				if(Controller != null)
					e.DisplayText = Controller.GetItemDisplayName((string)e.Value);
			}
			public override BaseEdit CreateEditor() {
				return edit;
			}
			protected override void Dispose(bool disposing) {
				if(disposing) {
					this.CustomDisplayText -= new CustomDisplayTextEventHandler(PopupRepositoryItemComboBox_CustomDisplayText);
				}
				base.Dispose(disposing);
			}
		}
		#endregion
		TreeListController activeController;
		RepositoryItemComboBox enabledSortOrderComboBox;
		RepositoryItemComboBox disabledSortOrderComboBox;
		RepositoryItemComboBox enabledFieldNameComboBox;
		RepositoryItemComboBox disabledFieldNameComboBox;
		RepositoryItemCheckEdit enabledShowHeaderCheckBox;
		RepositoryItemCheckEdit disabledShowHeaderCheckBox;
		RepositoryItemCheckEdit enabledShowFooterCheckBox;
		RepositoryItemCheckEdit disabledShowFooterCheckBox;
		bool lockFocusedNode;
		public GroupSortTreeList()
			: base(null) {
			InitializeComponent();
			SubscribeEvents();
		}
		public TreeListController ActiveController {
			get { return activeController; }
			set { activeController = value; }
		}
		public bool LockFocusedNode { get { return lockFocusedNode; } set { lockFocusedNode = value; } }
		public override bool CanShowEditor {
			get {
				bool result = true;
				if(this.FocusedColumn != null && this.FocusedColumn.FieldName == ColumnNames.ShowFooter)
					result = HeaderChecked(this.FocusedNode);
				return result && base.CanShowEditor;
			}
		}
		GroupSortController GroupSortController { get { return (GroupSortController)activeController; } }
		public TreeListController CreateController(IServiceProvider serviceProvider) {
			return new GroupSortController(serviceProvider);
		}
		public override int SetFocusedNode(TreeListNode node) {
			if(LockFocusedNode)
				return -1;
			return base.SetFocusedNode(node);
		}
		protected override void Dispose(bool disposing) {
			if(disposing) {
				if(activeController != null) {
					activeController.UnsubscribeTreeListEvents(this);
					activeController = null;
				}
			}
			UnsubscribeEvents();
			base.Dispose(disposing);
		}
		static void InitSortOrderComboBox(RepositoryItemComboBox comboBox, bool enabled) {
			comboBox.Name = ColumnNames.SortOrder;
			comboBox.AllowFocused = false;
			comboBox.AutoHeight = false;
			comboBox.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
				new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
			comboBox.Items.AddRange(new DevExpress.XtraEditors.Controls.ComboBoxItem[] {
			new DevExpress.XtraEditors.Controls.ComboBoxItem(DevExpress.XtraPrinting.Native.DisplayTypeNameHelper.GetDisplayTypeName(XRColumnSortOrder.Ascending)),
			new DevExpress.XtraEditors.Controls.ComboBoxItem(DevExpress.XtraPrinting.Native.DisplayTypeNameHelper.GetDisplayTypeName(XRColumnSortOrder.Descending))
			});
			comboBox.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
			comboBox.ReadOnly = !enabled;
		}
		static void InitFiledNameComboBox(RepositoryItemComboBox comboBox, bool enabled) {
			comboBox.Name = ColumnNames.FieldName;
			comboBox.AllowFocused = false;
			comboBox.AutoHeight = false;
			comboBox.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
			comboBox.ReadOnly = !enabled;
		}
		static void InitShowHeaderCheckBox(RepositoryItemCheckEdit checkEdit, bool enabled) {
			checkEdit.Name = ColumnNames.ShowHeader;
			checkEdit.AllowFocused = false;
			checkEdit.AutoHeight = false;
			checkEdit.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.Standard;
			checkEdit.ReadOnly = !enabled;
		}
		static void InitShowFooterCheckBox(RepositoryItemCheckEdit checkEdit, bool enabled) {
			checkEdit.Name = ColumnNames.ShowFooter;
			checkEdit.AllowFocused = false;
			checkEdit.AutoHeight = false;
			checkEdit.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.Standard;
			checkEdit.ReadOnly = !enabled;
		}
		void InitializeComponent() {
			enabledFieldNameComboBox = new XRPopupRepositoryItemComboBox(this);
			disabledFieldNameComboBox = new XRPopupRepositoryItemComboBox(this);
			enabledSortOrderComboBox = new RepositoryItemComboBox();
			disabledSortOrderComboBox = new RepositoryItemComboBox();
			enabledShowHeaderCheckBox = new RepositoryItemCheckEdit();
			disabledShowHeaderCheckBox = new RepositoryItemCheckEdit();
			enabledShowFooterCheckBox = new RepositoryItemCheckEdit();
			disabledShowFooterCheckBox = new RepositoryItemCheckEdit();
			((System.ComponentModel.ISupportInitialize)(enabledSortOrderComboBox)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(disabledSortOrderComboBox)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(enabledShowHeaderCheckBox)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(disabledShowHeaderCheckBox)).BeginInit();
			InitFiledNameComboBox(enabledFieldNameComboBox, true);
			InitFiledNameComboBox(disabledFieldNameComboBox, false);
			InitSortOrderComboBox(enabledSortOrderComboBox, true);
			InitSortOrderComboBox(disabledSortOrderComboBox, false);
			InitShowHeaderCheckBox(enabledShowHeaderCheckBox, true);
			InitShowHeaderCheckBox(disabledShowHeaderCheckBox, false);
			InitShowFooterCheckBox(enabledShowFooterCheckBox, true);
			InitShowFooterCheckBox(disabledShowFooterCheckBox, false);
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
			this.PopupMenuShowing += GroupSortTreeList_PopupMenuShowing;
			this.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
				enabledFieldNameComboBox, disabledFieldNameComboBox,
				enabledSortOrderComboBox, disabledSortOrderComboBox, 
				enabledShowHeaderCheckBox, disabledShowHeaderCheckBox,
				enabledShowFooterCheckBox, disabledShowFooterCheckBox});
			((System.ComponentModel.ISupportInitialize)(enabledSortOrderComboBox)).EndInit();
			((System.ComponentModel.ISupportInitialize)(disabledSortOrderComboBox)).EndInit();
			((System.ComponentModel.ISupportInitialize)(enabledShowHeaderCheckBox)).EndInit();
			((System.ComponentModel.ISupportInitialize)(disabledShowHeaderCheckBox)).EndInit();
		}
		protected override void RaiseCustomDrawColumnHeader(CustomDrawColumnHeaderEventArgs e) {
			base.RaiseCustomDrawColumnHeader(e);
			(e.ObjectArgs as ObjectInfoArgs).State = ObjectState.Normal;
		}
		void SubscribeEvents() {
			foreach(RepositoryItem item in this.RepositoryItems)
				item.EditValueChanged += new EventHandler(item_EditValueChanged);
			this.CustomNodeCellEdit += new GetCustomNodeCellEditEventHandler(GroupSortTreeList_CustomNodeCellEdit);
			this.NodeChanged += new DevExpress.XtraTreeList.NodeChangedEventHandler(GroupSortTreeList_NodeChanged);
			this.KeyDown += new KeyEventHandler(GroupSortTreeList_KeyDown);
			enabledShowHeaderCheckBox.EditValueChanging += new DevExpress.XtraEditors.Controls.ChangingEventHandler(ShowHeaderCheckBox_EditValueChanging);
			enabledShowFooterCheckBox.EditValueChanging += new DevExpress.XtraEditors.Controls.ChangingEventHandler(EnabledShowFooterCheckBox_EditValueChanging);
			enabledSortOrderComboBox.CustomDisplayText += new DevExpress.XtraEditors.Controls.CustomDisplayTextEventHandler(comboBox_CustomDisplayText);
			disabledSortOrderComboBox.CustomDisplayText += new DevExpress.XtraEditors.Controls.CustomDisplayTextEventHandler(comboBox_CustomDisplayText);
		}
		void UnsubscribeEvents() {
			foreach(RepositoryItem item in this.RepositoryItems)
				item.EditValueChanged -= new EventHandler(item_EditValueChanged);
			this.CustomNodeCellEdit -= new GetCustomNodeCellEditEventHandler(GroupSortTreeList_CustomNodeCellEdit);
			this.NodeChanged -= new DevExpress.XtraTreeList.NodeChangedEventHandler(GroupSortTreeList_NodeChanged);
			this.KeyDown -= new KeyEventHandler(GroupSortTreeList_KeyDown);
			enabledShowHeaderCheckBox.EditValueChanging -= new DevExpress.XtraEditors.Controls.ChangingEventHandler(ShowHeaderCheckBox_EditValueChanging);
			enabledShowFooterCheckBox.EditValueChanging -= new DevExpress.XtraEditors.Controls.ChangingEventHandler(EnabledShowFooterCheckBox_EditValueChanging);
			enabledSortOrderComboBox.CustomDisplayText -= new DevExpress.XtraEditors.Controls.CustomDisplayTextEventHandler(comboBox_CustomDisplayText);
			disabledSortOrderComboBox.CustomDisplayText -= new DevExpress.XtraEditors.Controls.CustomDisplayTextEventHandler(comboBox_CustomDisplayText);
		}
		void GroupSortTreeList_KeyDown(object sender, KeyEventArgs e) {
			if(e.KeyData == Keys.Delete)
				DeleteNode();
		}
		void item_EditValueChanged(object sender, EventArgs e) {
			this.PostEditor();
		}
		void GroupSortTreeList_CustomNodeCellEdit(object sender, GetCustomNodeCellEditEventArgs e) {
			if(GroupSortController == null)
				return;
			switch (e.Column.FieldName) {
				case ColumnNames.FieldName:
					e.RepositoryItem = GroupSortController.IsFieldNameEnabled() ? enabledFieldNameComboBox : disabledFieldNameComboBox;
					break;
				case ColumnNames.SortOrder:
					e.RepositoryItem = GroupSortController.IsSortOrderEnabled() ? enabledSortOrderComboBox : disabledSortOrderComboBox;
					break;
				case ColumnNames.ShowHeader:
					e.RepositoryItem = GroupSortController.IsShowHeaderEnabled() ? enabledShowHeaderCheckBox : disabledShowHeaderCheckBox;
					break;
				case ColumnNames.ShowFooter:
					bool check = HeaderChecked(e.Node);
					e.RepositoryItem = check && GroupSortController.IsShowFooterEnabled() ? enabledShowFooterCheckBox : disabledShowFooterCheckBox;
					break;
				default:
					break;
			}			
		}
		void GroupSortTreeList_NodeChanged(object sender, NodeChangedEventArgs e) {
			if(e.Node.ParentNode != null && e.ChangeType == NodeChangeTypeEnum.Add)
				e.Node.ParentNode.Expanded = true;
		}
		void ShowHeaderCheckBox_EditValueChanging(object sender, DevExpress.XtraEditors.Controls.ChangingEventArgs e) {
			if((bool)e.NewValue)
				return;
			e.Cancel = !GroupSortController.CanHideHeader();
		}
		void EnabledShowFooterCheckBox_EditValueChanging(object sender, DevExpress.XtraEditors.Controls.ChangingEventArgs e) {
			if((bool)e.NewValue)
				return;
			e.Cancel = !GroupSortController.CanHideFooter();
		}
		void comboBox_CustomDisplayText(object sender, DevExpress.XtraEditors.Controls.CustomDisplayTextEventArgs e) {
			Enum value = e.Value as Enum;
			if(value != null)
				e.DisplayText = DevExpress.XtraPrinting.Native.DisplayTypeNameHelper.GetDisplayTypeName(value);
		}
		void DeleteNode() {
			if(this.GroupSortController.CanDeleteSort())
				this.GroupSortController.DeleteSort();
		}
		bool HeaderChecked(TreeListNode node) {
			object check = node.GetValue(this.Columns[ColumnNames.ShowHeader]);
			return check == null ? false : (bool)check;
		}
		void GroupSortTreeList_PopupMenuShowing(object sender, PopupMenuShowingEventArgs e) {
			TreeListHitInfo hitInfo = CalcHitInfo(e.Point);
			if(hitInfo.HitInfoType != HitInfoType.Column && hitInfo.HitInfoType != HitInfoType.BehindColumn)
				return;
			List<TreeListStringId> ignoreItems = new List<TreeListStringId>(new TreeListStringId[] {
				TreeListStringId.MenuColumnColumnCustomization,
				TreeListStringId.MenuColumnSortAscending,
				TreeListStringId.MenuColumnSortDescending
			});
			foreach(DXMenuItem item in e.Menu.Items) {
				if(!(item.Tag is TreeListStringId))
					continue;
				TreeListStringId treeListStringId = (TreeListStringId)item.Tag;
				if(!ignoreItems.Contains(treeListStringId))
					continue;
				item.Enabled = false;
				item.Visible = false;
				item.BeginGroup = false;
				ignoreItems.Remove(treeListStringId);
				if(ignoreItems.Count == 0)
					break;
			}
			if(hitInfo.HitInfoType == HitInfoType.BehindColumn)
				e.Menu.Items[e.Menu.Items.Count - 1].BeginGroup = false;
		}
	}
}
