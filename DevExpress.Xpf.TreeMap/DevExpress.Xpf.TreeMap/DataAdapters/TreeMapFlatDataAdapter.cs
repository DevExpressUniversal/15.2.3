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
using System.Windows;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using DevExpress.Data;
using DevExpress.Data.Helpers;
using DevExpress.Xpf.TreeMap.Native;
namespace DevExpress.Xpf.TreeMap {
	public class TreeMapFlatDataAdapter : TreeMapDataAdapterBase {
		public static readonly DependencyProperty DataSourceProperty = DependencyProperty.Register("DataSource",
			typeof(object), typeof(TreeMapFlatDataAdapter), new PropertyMetadata(null, DataSourcePropertyChanged));
		public static readonly DependencyProperty DataMemberProperty = DependencyProperty.Register("DataMember",
			typeof(string), typeof(TreeMapFlatDataAdapter), new PropertyMetadata(MemberPropertyChanged));
		public static readonly DependencyProperty ValueDataMemberProperty = DependencyProperty.Register("ValueDataMember",
			typeof(string), typeof(TreeMapFlatDataAdapter), new PropertyMetadata(string.Empty, MemberPropertyChanged));
		public static readonly DependencyProperty LabelDataMemberProperty = DependencyProperty.Register("LabelDataMember",
			typeof(string), typeof(TreeMapFlatDataAdapter), new PropertyMetadata(string.Empty, MemberPropertyChanged));
		public static readonly DependencyProperty GroupsProperty = DependencyProperty.Register("Groups",
			typeof(GroupDefinitionCollection), typeof(TreeMapFlatDataAdapter), new PropertyMetadata(MemberPropertyChanged));
		static void DataSourcePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			TreeMapFlatDataAdapter adapter = d as TreeMapFlatDataAdapter;
			if (adapter != null) {
				adapter.controller.DataSource = e.NewValue;
				adapter.UpdateData();
			}
		}
		static void MemberPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			TreeMapFlatDataAdapter adapter = d as TreeMapFlatDataAdapter;
			if (adapter != null)
				adapter.UpdateData();
		}
		public object DataSource {
			get { return GetValue(DataSourceProperty); }
			set { SetValue(DataSourceProperty, value); }
		}
		public string DataMember {
			get { return (string)GetValue(DataMemberProperty); }
			set { SetValue(DataMemberProperty, value); }
		}
		public string ValueDataMember {
			get { return (string)GetValue(ValueDataMemberProperty); }
			set { SetValue(ValueDataMemberProperty, value); }
		}
		public string LabelDataMember {
			get { return (string)GetValue(LabelDataMemberProperty); }
			set { SetValue(LabelDataMemberProperty, value); }
		}
		public GroupDefinitionCollection Groups {
			get { return (GroupDefinitionCollection)GetValue(GroupsProperty); }
			set { SetValue(GroupsProperty, value); }
		}
		readonly GroupController controller;
		readonly TreeMapItemCollection items;
		protected internal override TreeMapItemCollection ItemsCollection { get { return items; } }
		public TreeMapFlatDataAdapter() {
			controller = new GroupController(this);
			items = new TreeMapItemCollection();
			this.SetValue(GroupsProperty, new GroupDefinitionCollection());
		}
		void UpdateData() {
			controller.UpdateData();
		}
		protected override TreeMapDependencyObject CreateObject() {
			return new TreeMapFlatDataAdapter();
		}
	}
	public class TreeMapGroupDefinition : TreeMapDependencyObject {
		public static readonly DependencyProperty GroupDataMemberProperty = DependencyProperty.Register("GroupDataMember",
			typeof(string), typeof(TreeMapGroupDefinition), new PropertyMetadata(string.Empty));
		public static readonly DependencyProperty HeaderContentTemplateProperty = DependencyProperty.Register("HeaderContentTemplate",
			typeof(DataTemplate), typeof(TreeMapGroupDefinition));
		public string GroupDataMember {
			get { return (string)GetValue(GroupDataMemberProperty); }
			set { SetValue(GroupDataMemberProperty, value); }
		}
		public DataTemplate HeaderContentTemplate {
			get { return (DataTemplate)GetValue(HeaderContentTemplateProperty); }
			set { SetValue(HeaderContentTemplateProperty, value); }
		}
		public TreeMapGroupDefinition() { }
		protected override TreeMapDependencyObject CreateObject() {
			return new TreeMapGroupDefinition();
		}
	}
	public class GroupDefinitionCollection : FreezableCollection<TreeMapGroupDefinition> {
	}
}
namespace DevExpress.Xpf.TreeMap.Native {
	public class GroupController : ListSourceDataController, IDataControllerData, IDataControllerData2 {
		readonly TreeMapFlatDataAdapter dataAdapter;
		readonly BindingBehavior bindingBehavior;
		IList<TreeMapItem> Items { get { return dataAdapter.ItemsCollection; } }
		public object DataSource { get { return bindingBehavior.ActualDataSource; } set { bindingBehavior.UpdateActualDataSource(value); } }
		public GroupController(TreeMapFlatDataAdapter dataAdapter) {
			this.dataAdapter = dataAdapter;
			this.bindingBehavior = new BindingBehavior();
			this.bindingBehavior.ActualDataSourceChanged += OnActualDataSourceChanged;
			DataClient = this;
		}
		#region IDataControllerData implementation
		UnboundColumnInfoCollection IDataControllerData.GetUnboundColumns() { return null; }
		object IDataControllerData.GetUnboundData(int row, DataColumnInfo col, object value) { return null; }
		void IDataControllerData.SetUnboundData(int row, DataColumnInfo col, object value) { }
		#endregion
		#region IDataControllerData2 implementation
		void AddComplexColumnInfo(ComplexColumnInfoCollection collection, string item) {
			if (item.Contains(".") && Columns[item] == null && collection.IndexOf(item) < 0)
				collection.Add(item);
		}
		ComplexColumnInfoCollection IDataControllerData2.GetComplexColumns() {
			ComplexColumnInfoCollection collection = new ComplexColumnInfoCollection();
			AddComplexColumnInfo(collection, dataAdapter.ValueDataMember);
			AddComplexColumnInfo(collection, dataAdapter.LabelDataMember);
			return collection;
		}
		bool IDataControllerData2.CanUseFastProperties { get { return true; } }
		void IDataControllerData2.SubstituteFilter(SubstituteFilterEventArgs args) { }
		bool IDataControllerData2.HasUserFilter { get { return false; } }
		bool? IDataControllerData2.IsRowFit(int listSourceRow, bool fit) { return null; }
		PropertyDescriptorCollection IDataControllerData2.PatchPropertyDescriptorCollection(PropertyDescriptorCollection collection) { return collection; }
		#endregion
		void LoadGroupData(IList<TreeMapItem> items) {
			List<string> groups = new List<string>();
			foreach (TreeMapGroupDefinition group in dataAdapter.Groups)
				groups.Add(group.GroupDataMember);
			ApplySortGroup(groups, dataAdapter.ValueDataMember);
			CreateTreeMapItems(items);
		}
		void LoadData(IList<TreeMapItem> items) {
			for (int i = 0; i < ListSourceRowCount; i++) {
				TreeMapItem mapItem = CreateTreeMapItem(i);
				if (mapItem != null)
					items.Add(mapItem);
			}
		}
		void ApplySortGroup(List<string> members, string summaryColumn) {
			GroupSummary.Clear();
			GroupSummary.Add(new SummaryItem(Columns[summaryColumn], SummaryItemType.Sum));
			List<DataColumnSortInfo> sortInfo = new List<DataColumnSortInfo>();
			if (members.Count > 0)
				foreach (string member in members) 
					if (!string.IsNullOrEmpty(member))
						sortInfo.Add(new DataColumnSortInfo(Columns[member]));
			SortInfo.ClearAndAddRange(sortInfo.ToArray(), members.Count);
		}
		TreeMapItem CreateTreeMapItem(int rowIndex) {
			TreeMapItem item = new TreeMapItem();
			object row = GetRow(rowIndex);
			object valueObject = GetRowValue(rowIndex, dataAdapter.ValueDataMember);
			item.Value = Convert.ToDouble(valueObject, CultureInfo.InvariantCulture);
			object label = GetRowValue(rowIndex, dataAdapter.LabelDataMember);
			item.Label = label != null ? label.ToString() : string.Empty;
			item.Tag = row;
			return item;
		}
		void CreateTreeMapItems(IList<TreeMapItem> items) {
			foreach (GroupRowInfo groupInfo in GroupInfo)
				if (groupInfo.ParentGroup == null)
					AddTreeMapItem(items, groupInfo);
		}
		List<object> AddTreeMapItem(IList<TreeMapItem> items, GroupRowInfo groupInfo) {
			TreeMapItem item = new TreeMapItem();
			List<object> children = FillChildren(groupInfo, item.Children);
			item.Label = groupInfo.GroupValue.ToString();
			item.Tag = children;
			if (groupInfo.Level < dataAdapter.Groups.Count)
				item.HeaderContentTemplate = dataAdapter.Groups[groupInfo.Level].HeaderContentTemplate;
			items.Add(item);
			return children;
		}
		List<object> FillChildren(GroupRowInfo groupInfo, IList<TreeMapItem> items) {
			List<object> children = new List<object>();
			List<GroupRowInfo> childrenGroups = new List<GroupRowInfo>();
			GroupInfo.GetChildrenGroups(groupInfo, childrenGroups);
			if (childrenGroups.Count > 0)
				foreach (GroupRowInfo groupItem in childrenGroups)
					children.AddRange(AddTreeMapItem(items, groupItem));
			else
				children.AddRange(FillLeafs(groupInfo.ChildControllerRow, groupInfo.ChildControllerRowCount, items, groupInfo.Level));
			return children;
		}
		List<object> FillLeafs(int start, int count, IList<TreeMapItem> items, int groupLevel) {
			List<object> leafs = new List<object>();
			for (int i = start; i < start + count; i++) {
				TreeMapItem leafItem = CreateTreeMapItem(i);
				items.Add(leafItem);
				leafs.Add(leafItem.Tag);
			}
			return leafs;
		}
		void OnActualDataSourceChanged() {
			UpdateData();
		}
		protected override void OnBindingListChanged(ListChangedEventArgs e) {
			switch (e.ListChangedType) {
				case ListChangedType.ItemMoved:
					break;
				case ListChangedType.ItemDeleted:
				case ListChangedType.ItemAdded:
				case ListChangedType.Reset:
				default:
					UpdateData();
					break;
			}
		}
		internal void UpdateData() {
			Items.Clear();
			LoadData();
		}
		internal void LoadData() {
			if (Items != null && DataSource != null) {
				ListSource = DataHelper.GetList(DataSource, dataAdapter.DataMember);
				if (ListSource != null) {
					if (dataAdapter.Groups != null && dataAdapter.Groups.Count > 0)
						LoadGroupData(Items);
					else
						LoadData(Items);
				}
			}
		}
	}
}
