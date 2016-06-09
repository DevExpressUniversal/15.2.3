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

using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using DevExpress.DashboardCommon;
using DevExpress.DashboardCommon.Native;
using DevExpress.Data.Filtering;
using DevExpress.PivotGrid.OLAP;
using DevExpress.PivotGrid.QueryMode;
using DevExpress.Utils.Controls;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraPivotGrid;
using DevExpress.XtraPivotGrid.Data;
using DevExpress.XtraPivotGrid.FilterDropDown;
namespace DevExpress.DashboardWin.Native {
	public partial class OlapFilterForm : DashboardForm {
		class HierarchyItemWrapper {
			Dimension hierarchyItem;
			string caption;
			public HierarchyItemWrapper(Dimension hierarchyItem, string caption) {
				this.hierarchyItem = hierarchyItem;
				this.caption = caption;
			}
			public Dimension HierarchyItem { get { return hierarchyItem; } }
			public override string ToString() {
				return caption;
			}
		}
		Dictionary<Dimension, PivotFilterItemsBase> filters = new Dictionary<Dimension, PivotFilterItemsBase>();
		DashboardDesigner designer;
		DataDashboardItem item;
		IPivotOLAPDataSource dataSource;
		PivotGridData data = new PivotGridData();
		public OlapFilterForm() {
			InitializeComponent();
		}
		public OlapFilterForm(DashboardDesigner designer)
			: this() {
			this.designer = designer;
			this.item = ((DataDashboardItem)designer.SelectedDashboardItem);
			dataSource = ((DashboardOlapDataSource)item.DataSource).Connection.OlapDataSource;
			data.OptionsOLAP.FilterByUniqueName = true;
			data.PivotDataSource = dataSource;
			treeFilter.RetrieveChildElements += OnRetrieveChildElements;
			if(dataSource != null && dataSource.Connected)
				foreach(Dimension hierarchyItem in item.UniqueDimensions) {
					PivotGridFieldBase field = data.Fields.Add(hierarchyItem.DataMember, PivotArea.FilterArea);
					field.Name = item.DataItemRepository.GetActualID(hierarchyItem);
					if(dataSource.GetFieldHierarchyLevel(hierarchyItem.DataMember) < 2)
						cbDimension.Properties.Items.Add(new HierarchyItemWrapper(hierarchyItem, GetDataFieldCaption(hierarchyItem, item.DataSource)));
				}
			if(cbDimension.Properties.Items.Count > 0)
				cbDimension.SelectedIndex = 0;
			LookAndFeel.ParentLookAndFeel = designer.LookAndFeel;
		}
		string GetDataFieldCaption(Dimension dimension, IDashboardDataSource dataSource) {
			return dataSource != null ? dataSource.GetFieldCaption(dimension.DataMember,"") : dimension.DataMember;
		}
		void OnRetrieveChildElements(object sender, RetrieveChildElementsEventArgs e) {
			PivotGroupFilterPopupContainerForm.OnRetrieveChildElements(sender, e, (PivotGroupFilterItems)GetFilterItems(), false);
		}
		void OncbDimensionSelectedValueChanged(object sender, System.EventArgs e) {
			treeFilter.Items.Clear();
			if(cbDimension.SelectedItem == null) {
				return;
			}
			PivotFilterItemsBase items = GetFilterItems();
			bool isList = items is PivotGridFilterItems;
			treeFilter.IsList = isList;
			treeFilter.Items.Add(new CheckedTreeViewItem(items.ShowAllItemCaption, null, true, CheckedListBoxItem.GetCheckState(items.CheckState)));
			foreach(PivotGridFilterItem item in items)
				if(item.Level == 0)
					treeFilter.Items.Add(new CheckedTreeViewItem(item, null, isList, item.IsChecked));
		}
		PivotFilterItemsBase GetFilterItems() {
			Dimension hierarchyItem = ((HierarchyItemWrapper)cbDimension.SelectedItem).HierarchyItem;
			PivotFilterItemsBase items;
			if(!filters.TryGetValue(hierarchyItem, out items)) {
				PivotGridFieldBase parentField = data.Fields[hierarchyItem.DataMember];				
				CriteriaOperator itemCriteria = GetItemCriteria(parentField);
				items = parentField.Group == null ? (PivotFilterItemsBase)new FieldCriteriaFilterItems(data, parentField, false, false, false, itemCriteria) :
					new GroupCriteriaFilterItems(data, parentField, itemCriteria);
				items.CreateItems();
				filters.Add(hierarchyItem, items);
			}
			return items;
		}
		CriteriaOperator GetItemCriteria(PivotGridFieldBase field) {
			Dictionary<string, string> fields = new Dictionary<string, string>();
			if(field.Group == null) {
				fields.Add(field.Name, field.FieldName);
				fields.Add(field.FieldName, field.FieldName);
			} else
				foreach(PivotGridFieldBase child in field.Group) {
					fields.Add(child.Name, child.Name);
					fields.Add(child.FieldName, child.Name);
				}
			CriteriaOperator itemFilter;
			if(item.DataSource != null)
				itemFilter = DevExpress.DataAccess.Native.Sql.FilterHelper.GetCriteriaOperator(item.FilterString, item.Dashboard.Parameters);
			else
				itemFilter = CriteriaOperator.Parse(item.FilterString);
			return new QueryCriteriaOperatorVisitor(fields).Process(itemFilter);
		}
		void ApplyFilter() { 
			Dictionary<string, string> fields = new Dictionary<string,string>();
			foreach(Dimension field in item.Dimensions) {
				string name = item.DataItemRepository.GetActualID(field);
				fields.Add(name, name);
			}
			foreach(KeyValuePair<Dimension, PivotFilterItemsBase> filter in filters) {
				PivotGridFieldBase field = filter.Value.Field;
				if(field.Group == null)
					fields.Remove(field.Name);
				else
					foreach(PivotGridFieldBase child in field.Group)
						fields.Remove(child.Name);
			}
			CriteriaOperator criteria = new QueryCriteriaOperatorVisitor(fields).Process(CriteriaOperator.Parse(item.FilterString));
			foreach(KeyValuePair<Dimension, PivotFilterItemsBase> filter in filters) {
				CriteriaOperator fieldFilter = (((IFilterValues)filter.Value).GetActualCriteria());
				if(!ReferenceEquals(fieldFilter, null))
					if(ReferenceEquals(criteria, null))
						criteria = fieldFilter;
					else
						criteria = CriteriaOperator.And(criteria, fieldFilter);
			}
			btnApply.Enabled = false;
			if(ReferenceEquals(criteria, null) && string.IsNullOrWhiteSpace(item.FilterString))
				return;
			if(!ReferenceEquals(criteria, null) && criteria.ToString() == item.FilterString)
				return;
			FilterDashboardItemHistoryItem historyItem = new FilterDashboardItemHistoryItem(item, CriteriaOperator.ToString(criteria));
			historyItem.Redo(designer);
			designer.History.Add(historyItem);
		}
		void btnOK_Click(object sender, System.EventArgs e) {
			ApplyFilter();
			DialogResult = DialogResult.OK;
		}
		void btnApply_Click(object sender, System.EventArgs e) {
			ApplyFilter();
		}
		void btnCancel_Click(object sender, System.EventArgs e) {
			DialogResult = DialogResult.Cancel;
		}
		void treeFilter_ItemCheck(object sender, XtraEditors.Controls.ItemCheckEventArgs e) {
			if(e.Index != 0) {
				((IFilterItem)treeFilter.Items[e.Index].Value).IsChecked = e.State == CheckState.Indeterminate ? null : (bool?)(e.State == CheckState.Checked);
				btnApply.Enabled = true;
			} else {
				if(e.State == CheckState.Indeterminate)
					return;
				GetFilterItems().CheckAllItems(e.State == CheckState.Checked);
				if(e.State == CheckState.Checked)
					treeFilter.CheckAll();
				else
					treeFilter.UnCheckAll();
				btnApply.Enabled = false;
				foreach(KeyValuePair<Dimension, PivotFilterItemsBase> filter in filters) {
					if(filter.Value.CanAccept)
						btnApply.Enabled = true;
				}
			}
		}
	}
	[DXToolboxItem(false)]
	public class CheckedListTreeViewControl : CheckedTreeViewControl {
		public bool IsList { get; set; }
		public CheckedListTreeViewControl() {
		}
		protected override XtraEditors.ViewInfo.BaseStyleControlViewInfo CreateViewInfo() {
			return new CheckedListTreeViewInfo(this);
		}
	}
	public class CheckedListTreeViewInfo : CheckedTreeViewInfo {
		public new CheckedListTreeViewControl OwnerControl { get { return (CheckedListTreeViewControl)base.OwnerControl; } }
		protected override int LevelOffset { get { return OwnerControl.IsList ? 0 : base.LevelOffset; } }
		public override int FullOpenCloseButtonWidth { get { return OwnerControl.IsList ? 0 : base.FullOpenCloseButtonWidth; } }		
		public CheckedListTreeViewInfo(CheckedTreeViewControl treeView) : base(treeView) {
		}
	}
}
