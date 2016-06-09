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
using System.Collections.Generic;
using DevExpress.XtraSpreadsheet.Localization;
using DevExpress.XtraSpreadsheet.Commands;
using DevExpress.XtraSpreadsheet.Model;
namespace DevExpress.XtraSpreadsheet.Forms {
	#region GenericFilterViewModel
	public class GenericFilterViewModel : ViewModelBase {
		#region Fields
		readonly ISpreadsheetControl control;
		readonly List<FilterOperatorItem> filterOperatorDataSource;
		readonly List<string> uniqueFilterValues;
		GenericFilterOperator filterOperator;
		GenericFilterOperator secondaryFilterOperator;
		string filterValue = String.Empty;
		string secondaryFilterValue = String.Empty;
		string columnCaption = String.Empty;
		bool operatorAnd;
		bool isDateTimeFilter;
		#endregion
		public GenericFilterViewModel(ISpreadsheetControl control) {
			Guard.ArgumentNotNull(control, "control");
			this.control = control;
			this.filterOperatorDataSource = new List<FilterOperatorItem>();
			this.uniqueFilterValues = new List<string>();
			PopulateFilterOperatorValues();
			PopulateUniqueFilterValues(DocumentModel, uniqueFilterValues);
		}
		#region Properties
		public ISpreadsheetControl Control { get { return control; } }
		public DocumentModel DocumentModel { get { return Control.InnerControl.DocumentModel; } }
		public IList<FilterOperatorItem> FilterOperatorDataSource { get { return filterOperatorDataSource; } }
		public IList<string> UniqueFilterValues { get { return uniqueFilterValues; } }
		public GenericFilterOperator FilterOperator {
			get { return filterOperator; }
			set {
				if (FilterOperator == value)
					return;
				this.filterOperator = value;
				OnPropertyChanged("FilterOperator");
			}
		}
		public GenericFilterOperator SecondaryFilterOperator {
			get { return secondaryFilterOperator; }
			set {
				if (SecondaryFilterOperator == value)
					return;
				this.secondaryFilterOperator = value;
				OnPropertyChanged("SecondaryFilterOperator");
			}
		}
		public string FilterValue {
			get { return filterValue; }
			set {
				if (FilterValue == value)
					return;
				this.filterValue = value;
				OnPropertyChanged("FilterValue");
			}
		}
		public string SecondaryFilterValue {
			get { return secondaryFilterValue; }
			set {
				if (SecondaryFilterValue == value)
					return;
				this.secondaryFilterValue = value;
				OnPropertyChanged("SecondaryFilterValue");
			}
		}
		public string ColumnCaption {
			get { return columnCaption; }
			set {
				if (ColumnCaption == value)
					return;
				this.columnCaption = value;
				OnPropertyChanged("ColumnCaption");
			}
		}
		public bool OperatorAnd {
			get { return operatorAnd; }
			set {
				if (OperatorAnd == value)
					return;
				this.operatorAnd = value;
				OnPropertyChanged("OperatorAnd");
			}
		}
		public bool IsDateTimeFilter {
			get { return isDateTimeFilter; }
			set {
				if (IsDateTimeFilter == value)
					return;
				this.isDateTimeFilter = value;
				OnPropertyChanged("IsDateTimeFilter");
			}
		}
		public ShowCustomFilterFormCommandBase Command { get; set; }
		#endregion
		void PopulateFilterOperatorValues() {
			filterOperatorDataSource.Clear();
			AddFilterOperator(XtraSpreadsheetStringId.Caption_GenericFilterOperatorNone, GenericFilterOperator.None);
			AddFilterOperator(XtraSpreadsheetStringId.Caption_GenericFilterOperatorEquals, GenericFilterOperator.Equals);
			AddFilterOperator(XtraSpreadsheetStringId.Caption_GenericFilterOperatorDoesNotEqual, GenericFilterOperator.DoesNotEqual);
			if (IsDateTimeFilter) {
				AddFilterOperator(XtraSpreadsheetStringId.Caption_GenericFilterOperatorAfter, GenericFilterOperator.Greater);
				AddFilterOperator(XtraSpreadsheetStringId.Caption_GenericFilterOperatorAfterOrEqual, GenericFilterOperator.GreaterOrEqual);
				AddFilterOperator(XtraSpreadsheetStringId.Caption_GenericFilterOperatorBefore, GenericFilterOperator.Less);
				AddFilterOperator(XtraSpreadsheetStringId.Caption_GenericFilterOperatorBeforeOrEqual, GenericFilterOperator.LessOrEqual);
			}
			else {
				AddFilterOperator(XtraSpreadsheetStringId.Caption_GenericFilterOperatorGreater, GenericFilterOperator.Greater);
				AddFilterOperator(XtraSpreadsheetStringId.Caption_GenericFilterOperatorGreaterOrEqual, GenericFilterOperator.GreaterOrEqual);
				AddFilterOperator(XtraSpreadsheetStringId.Caption_GenericFilterOperatorLess, GenericFilterOperator.Less);
				AddFilterOperator(XtraSpreadsheetStringId.Caption_GenericFilterOperatorLessOrEqual, GenericFilterOperator.LessOrEqual);
			}
			AddFilterOperator(XtraSpreadsheetStringId.Caption_GenericFilterOperatorBeginsWith, GenericFilterOperator.BeginsWith);
			AddFilterOperator(XtraSpreadsheetStringId.Caption_GenericFilterOperatorDoesNotBeginWith, GenericFilterOperator.DoesNotBeginWith);
			AddFilterOperator(XtraSpreadsheetStringId.Caption_GenericFilterOperatorEndsWith, GenericFilterOperator.EndsWith);
			AddFilterOperator(XtraSpreadsheetStringId.Caption_GenericFilterOperatorDoesNotEndWith, GenericFilterOperator.DoesNotEndWith);
			AddFilterOperator(XtraSpreadsheetStringId.Caption_GenericFilterOperatorContains, GenericFilterOperator.Contains);
			AddFilterOperator(XtraSpreadsheetStringId.Caption_GenericFilterOperatorDoesNotContain, GenericFilterOperator.DoesNotContain);
		}
		void AddFilterOperator(XtraSpreadsheetStringId stringId, GenericFilterOperator filterOperator) {
			FilterOperatorItem item = new FilterOperatorItem();
			item.Text = XtraSpreadsheetLocalizer.GetString(stringId);
			item.Value = filterOperator;
			filterOperatorDataSource.Add(item);
		}
		internal static void PopulateUniqueFilterValues(DocumentModel documentModel, List<string> uniqueFilterValues) {
			HashSet<string> values = new HashSet<string>();
			DataSortOrFilterAccessor accessor = new DataSortOrFilterAccessor(documentModel);
			accessor.GetSortOrFilterRange();
			if (accessor.Filter != null) {
				CellRange range = accessor.Filter.GetFilterColumnDataRange(documentModel.ActiveSheet.Selection.ActiveCell.Column);
				foreach (ICellBase cellBase in range.GetExistingCellsEnumerable()) {
					string text = GetCellText(documentModel, cellBase);
					if (!values.Contains(text))
						values.Add(text);
				}
			}
			uniqueFilterValues.Clear();
			uniqueFilterValues.AddRange(values);
			uniqueFilterValues.Sort();
		}
		static string GetCellText(DocumentModel documentModel, ICellBase cellBase) {
			ICell cell = cellBase as ICell;
			if (cell != null) {
				if (cell.HasError)
					return CellErrorFactory.GetErrorName(cell.Error, cell.Context);
				VariantValue value = cell.Value;
				if (value.IsNumeric) {
					if (cell.ActualFormat.IsDateTime)
						return cell.Text;
					if (cell.ActualFormat.FormatCode.Trim().EndsWith("%")) {
						string displayText = cell.Text;
						if (displayText.Trim().EndsWith("%"))
							return (value.NumericValue * 100).ToString(cell.Context.Culture) + '%';
					}
				}
				return value.ToText(cell.Context).InlineTextValue;
			}
			return cellBase.Value.ToText(documentModel.DataContext).GetTextValue(documentModel.SharedStringTable);
		}
		public bool Validate() {
			if (Command == null)
				return true;
			else
				return Command.Validate(this);
		}
		public void ApplyChanges() {
			if (Command != null)
				Command.ApplyChanges(this);
		}
	}
	#endregion
	public class FilterOperatorItem {
		public string Text { get; set; }
		public GenericFilterOperator Value { get; set; }
		public override string ToString() {
			return Text;
		}
	}
}
