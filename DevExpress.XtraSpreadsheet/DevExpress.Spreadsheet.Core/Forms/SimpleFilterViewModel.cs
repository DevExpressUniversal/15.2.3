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
using DevExpress.Utils;
using System.Globalization;
using System.Collections.Generic;
using DevExpress.XtraSpreadsheet.Localization;
using DevExpress.XtraSpreadsheet.Commands;
using DevExpress.XtraSpreadsheet.Commands.Internal;
using DevExpress.XtraSpreadsheet.Model;
namespace DevExpress.XtraSpreadsheet.Forms {
	#region SimpleFilterViewModel
	public class SimpleFilterViewModel : ViewModelBase {
		static readonly List<DateTimePartStrategy> strategies = CreateStrategies();
		#region Fields
		readonly ISpreadsheetControl control;
		readonly FilterValueNode root = new FilterValueNode();
		readonly List<FilterValueNode> dataSource;
		readonly List<FilterValueViewModel> sortedValues;
		int nodeId;
		bool hasBlankValue;
		bool blankValueChecked;
		#endregion
		public SimpleFilterViewModel(ISpreadsheetControl control) {
			Guard.ArgumentNotNull(control, "control");
			this.control = control;
			this.dataSource = new List<FilterValueNode>();
			this.sortedValues = new List<FilterValueViewModel>();
		}
		#region Properties
		public ISpreadsheetControl Control { get { return control; } }
		public DocumentModel DocumentModel { get { return Control.InnerControl.DocumentModel; } }
		public Worksheet Sheet { get { return DocumentModel.ActiveSheet; } }
		public FilterValueNode Root { get { return root; } }
		public WorkbookDataContext Context { get { return DocumentModel.DataContext; } }
		public List<FilterValueNode> DataSource { get { return dataSource; } }
		public bool BlankValueChecked { get { return blankValueChecked; } set { blankValueChecked = value; } }
		public string BlankValue { get { return "(Blanks)"; } }
		#endregion
		#region CreateStrategies
		static List<DateTimePartStrategy> CreateStrategies() {
			List<DateTimePartStrategy> result = new List<DateTimePartStrategy>();
			DateTimePartStrategy strategy;
			strategy = new DateTimePartStrategy();
			strategy.GroupingAccessor = GetYear;
			strategy.DateTimeGrouping = DateTimeGroupingType.Year;
			result.Add(strategy);
			strategy = new DateTimePartStrategy();
			strategy.GroupingAccessor = GetMonth;
			strategy.DateTimeGrouping = DateTimeGroupingType.Month;
			result.Add(strategy);
			strategy = new DateTimePartStrategy();
			strategy.GroupingAccessor = GetDay;
			strategy.DateTimeGrouping = DateTimeGroupingType.Day;
			result.Add(strategy);
			strategy = new DateTimePartStrategy();
			strategy.GroupingAccessor = GetHour;
			strategy.DateTimeGrouping = DateTimeGroupingType.Hour;
			result.Add(strategy);
			strategy = new DateTimePartStrategy();
			strategy.GroupingAccessor = GetMinute;
			strategy.DateTimeGrouping = DateTimeGroupingType.Minute;
			result.Add(strategy);
			strategy = new DateTimePartStrategy();
			strategy.GroupingAccessor = GetSecond;
			strategy.DateTimeGrouping = DateTimeGroupingType.Second;
			result.Add(strategy);
			return result;
		}
		static int GetYear(DateTime value) { return value.Year; }
		static int GetMonth(DateTime value) { return value.Month; }
		static int GetDay(DateTime value) { return value.Day; }
		static int GetHour(DateTime value) { return value.Hour; }
		static int GetMinute(DateTime value) { return value.Minute; }
		static int GetSecond(DateTime value) { return value.Second; }
		#endregion
		#region SetupViewModel
		internal void SetupViewModel(AutoFilterBase autoFilter, AutoFilterColumn filteredColumn) {
			SetupValues(autoFilter, filteredColumn);
			PopulateTree();
			PopulateTreeDataSource(root.Children);
		}
		void SetupValues(AutoFilterBase autoFilter, AutoFilterColumn filteredColumn) {
			CellRange columnRange = filteredColumn.GetColumnRange(autoFilter);
			for (int i = columnRange.TopLeft.Row; i <= columnRange.BottomRight.Row; i++) {
				Row row = Sheet.TryGetRegisteredRow(i);
				if (row == null) {
					SetupValue(null, true);
					continue;
				}
				bool rowHidden = row.IsHidden;
				if (rowHidden && FilterRowVisibilityHelper.IsRowHiddenByOtherFilterOrColumns(autoFilter, row, filteredColumn.ColumnId, autoFilter.FilteringBehaviour))
					continue;
				ICell cell = Sheet.TryGetCell(columnRange.LeftColumnIndex, row.Index);
				if (SuggestedFilterTypeInfo.IsDateTimeCell(cell))
					SetupDateTimeValue(cell.Value, !rowHidden);
				else
					SetupValue(cell, !rowHidden);
			}
		}
		void SetupDateTimeValue(VariantValue value, bool isChecked) {
			if (WorkbookDataContext.IsErrorDateTimeSerial(value.NumericValue, Context.DateSystem))
				return;
			AppendDateToTree(Context.FromDateTimeSerial(value.NumericValue), isChecked);
		}
		void SetupValue(ICell cell, bool isChecked) {
			string text = SuggestedFilterTypeInfo.GetCellText(DocumentModel, cell);
			if (String.IsNullOrEmpty(text)) {
				hasBlankValue = true;
				blankValueChecked = isChecked;
			}
			else
				AddToSortedList(CreateFilterValue(cell.Value, text, isChecked));
		}
		void AddToSortedList(FilterValueViewModel value) {
			IComparable<FilterValueViewModel> comparable = new FilterValueViewModelComparable(value);
			int index = Algorithms.BinarySearch(sortedValues, comparable);
			if (index < 0)
				sortedValues.Insert(~index, value);
		}
		#region CreateFilterValue
		FilterValueViewModel CreateFilterValue(VariantValue value, string text, bool isChecked) {
			FilterValueViewModel result = new FilterValueViewModel();
			result.Type = GetFilterValueType(value.Type);
			result.NumericValue = value.IsNumeric ? value.NumericValue : 0;
			result.TextValue = text;
			result.IsChecked = isChecked;
			return result;
		}
		FilterValueType GetFilterValueType(VariantValueType type) {
			if (type == VariantValueType.Numeric)
				return FilterValueType.Numeric;
			else if (type == VariantValueType.Boolean)
				return FilterValueType.Boolean;
			else if (type == VariantValueType.Error)
				return FilterValueType.Error;
			return FilterValueType.Text;
		}
		#endregion
		#endregion
		#region PopulateTree
		void PopulateTree() {
			List<FilterValueNode> rootChildren = root.Children;
			foreach (FilterValueViewModel value in sortedValues)
				rootChildren.Add(CreateTextNode(value.TextValue, value.IsChecked));
			if (hasBlankValue)
				rootChildren.Add(CreateTextNode(BlankValue, blankValueChecked));
		}
		#region AppendDateToTree
		void AppendDateToTree(DateTime value, bool isChecked) {
			FilterValueNode node = root;
			int count = strategies.Count;
			for (int i = 0; i < count; i++) {
				FilterValueNode result = AppendDateToTree(node, value, strategies[i]);
				if (result == null)
					break;
				if (!result.IsChecked)
					result.IsChecked = isChecked;
				node = result;
			}
		}
		FilterValueNode AppendDateToTree(FilterValueNode rootNode, DateTime value, DateTimePartStrategy strategy) {
			List<FilterValueNode> children = rootNode.Children;
			int index = FindDateNode(children, value, strategy);
			if (index < 0) {
				FilterValueNode node = CreateDateNode(rootNode, value, strategy);
				if (node == null)
					return null;
				index = ~index;
				children.Insert(index, node);
			}
			return children[index];
		}
		FilterValueNode CreateDateNode(FilterValueNode rootNode, DateTime value, DateTimePartStrategy strategy) {
			nodeId++;
			FilterValueNode result = new FilterValueNode();
			result.ParentId = rootNode.Id;
			result.Id = nodeId;
			result.DateTime = value;
			result.DateTimeGrouping = strategy.DateTimeGrouping;
			result.Text = strategy.FormatValue(strategy.GroupingAccessor(value), Context.Culture);
			return result;
		}
		int FindDateNode(List<FilterValueNode> list, DateTime value, DateTimePartStrategy strategy) {
			IComparable<FilterValueNode> comparable = new DateGroupingViewModelComparable(strategy.GroupingAccessor, strategy.GroupingAccessor(value));
			return Algorithms.BinarySearch(list, comparable);
		}
		#endregion
		FilterValueNode CreateTextNode(string value, bool isChecked) {
			nodeId++;
			FilterValueNode result = new FilterValueNode();
			result.ParentId = root.Id;
			result.Id = nodeId;
			result.DateTimeGrouping = DateTimeGroupingType.None;
			result.Text = value;
			result.IsChecked = isChecked;
			return result;
		}
		#endregion
		#region PopulateTreeDataSource
		void PopulateTreeDataSource(List<FilterValueNode> parentNodes) {
			int count = parentNodes.Count;
			for (int i = 0; i < count; i++) {
				FilterValueNode node = parentNodes[i];
				dataSource.Add(node);
				PopulateTreeDataSource(node.Children);
			}
		}
		#endregion
		#region ModifyFilter
		internal void ModifyFilter(AutoFilterColumn column) {
			if (AllFiltersChecked(root.Children)) {
				if (hasBlankValue && !blankValueChecked)
					AddBlankCustomFilter(column.CustomFilters);
				return;
			}
			ModifyFilterCriteria(column.FilterCriteria, root.Children);
		}
		void AddBlankCustomFilter(CustomFilterCollection customFilters) {
			CustomFilter blank = new CustomFilter();
			blank.FilterOperator = FilterComparisonOperator.NotEqual;
			blank.Value = " ";
			blank.NumericValue = VariantValue.Empty;
			customFilters.Add(blank);
		}
		void ModifyFilterCriteria(FilterCriteria criteria, List<FilterValueNode> parentNodes) {
			int count = parentNodes.Count;
			for (int i = 0; i < count; i++) {
				FilterValueNode node = parentNodes[i];
				if (!node.IsChecked)
					continue;
				DateTimeGroupingType groupingType = node.DateTimeGrouping;
				if (groupingType == DateTimeGroupingType.None)
					ModifyFilterCriteriaCore(criteria, node.Text);
				else
					ModifyDateGroupings(criteria.DateGroupings, node.Children, groupingType);
			}
		}
		void ModifyFilterCriteriaCore(FilterCriteria criteria, string textValue) {
			if (textValue == BlankValue)
				criteria.FilterByBlank = true;
			else
				criteria.Filters.Add(textValue);
		}
		void ModifyDateGroupings(DateGroupingCollection dateGroupings, List<FilterValueNode> parentNodes, DateTimeGroupingType groupingType) {
			int count = parentNodes.Count;
			if (count == 0)
				return;
			bool allFiltersChecked = AllFiltersChecked(parentNodes);
			if (!allFiltersChecked)
				groupingType = parentNodes[0].DateTimeGrouping;
			for (int i = 0; i < count; i++) {
				FilterValueNode node = parentNodes[i];
				if (!node.IsChecked)
					continue;
				if (allFiltersChecked) {
					AddDateGrouping(dateGroupings, node, groupingType);
					return;
				}
				else if (node.DateTimeGrouping == DateTimeGroupingType.Second) {
					AddDateGrouping(dateGroupings, node, groupingType);
					continue;
				}
				ModifyDateGroupings(dateGroupings, node.Children, groupingType);
			}
		}
		void AddDateGrouping(DateGroupingCollection dateGroupings, FilterValueNode node, DateTimeGroupingType groupingType) {
			DateGrouping grouping = DateGrouping.Create(DocumentModel.ActiveSheet, node.DateTime, groupingType);
			dateGroupings.Add(grouping);
		}
		bool AllFiltersChecked(List<FilterValueNode> parentNodes) {
			bool result = true;
			int count = parentNodes.Count;
			for (int i = 0; i < count; i++) {
				FilterValueNode node = parentNodes[i];
				if (!node.IsChecked && node.Text != BlankValue)
					return false;
				if (!AllFiltersChecked(node.Children))
					return false;
			}
			return result;
		}
		#endregion
		public bool Validate() {
			FilterSimpleCommand command = new FilterSimpleCommand(Control);
			return command.Validate(this);
		}
		public void ApplyChanges() {
			FilterSimpleCommand command = new FilterSimpleCommand(Control);
			command.ApplyChanges(this);
		}
	}
	#endregion
	#region FilterValueNode
	public class FilterValueNode {
		readonly List<FilterValueNode> children = new List<FilterValueNode>();
		public int Id { get; set; }
		public int ParentId { get; set; }
		public bool IsChecked { get; set; }
		public string Text { get; set; }
		public DateTime DateTime { get; set; }
		public DateTimeGroupingType DateTimeGrouping { get; set; }
		public List<FilterValueNode> Children { get { return children; } }
		public override string ToString() {
			return Text;
		}
	}
	#endregion
	public delegate int GetDateGroupingSubValue(DateTime value);
	#region DateTimePartStrategy
	public class DateTimePartStrategy {
		public GetDateGroupingSubValue GroupingAccessor { get; set; }
		public DateTimeGroupingType DateTimeGrouping { get; set; }
		public string FormatValue(int value, CultureInfo culture) {
			switch (DateTimeGrouping) {
				default:
				case DateTimeGroupingType.Year:
					return value.ToString();
				case DateTimeGroupingType.Month:
					return culture.DateTimeFormat.GetMonthName(value);
				case DateTimeGroupingType.Day:
					return value.ToString("00");
				case DateTimeGroupingType.Hour:
					return value.ToString("00");
				case DateTimeGroupingType.Minute:
					return value.ToString(":00");
				case DateTimeGroupingType.Second:
					return value.ToString(":00");
			}
		}
	}
	#endregion
	#region FilterValueViewModel
	public class FilterValueViewModel {
		public string TextValue { get; set; }
		public double NumericValue { get; set; }
		public FilterValueType Type { get; set; }
		public bool IsChecked { get; set; }
	}
	public enum FilterValueType {
		Numeric,
		Text,
		Boolean,
		Error
	}
	public class FilterValueViewModelComparable : IComparable<FilterValueViewModel> {
		readonly FilterValueViewModel filterValue;
		public FilterValueViewModelComparable(FilterValueViewModel filterValue) {
			Guard.ArgumentNotNull(filterValue, "filterValue");
			this.filterValue = filterValue;
		}
		FilterValueType Type { get { return filterValue.Type; } }
		public int CompareTo(FilterValueViewModel other) {
			if (other == null)
				return 0;
			int result = other.Type.CompareTo(Type);
			if (result != 0)
				return result;
			if (Type == FilterValueType.Numeric)
				return other.NumericValue.CompareTo(filterValue.NumericValue);
			return other.TextValue.CompareTo(filterValue.TextValue);
		}
	}
	#endregion
	#region DateGroupingViewModelComparable
	public class DateGroupingViewModelComparable : IComparable<FilterValueNode> {
		readonly GetDateGroupingSubValue groupingAccessor;
		readonly int value;
		public DateGroupingViewModelComparable(GetDateGroupingSubValue groupingAccessor, int value) {
			this.groupingAccessor = groupingAccessor;
			this.value = value;
		}
		public int CompareTo(FilterValueNode other) {
			int result = groupingAccessor(other.DateTime) - value;
			return other.DateTimeGrouping == DateTimeGroupingType.Year ? -result : result;
		}
	}
	#endregion
}
