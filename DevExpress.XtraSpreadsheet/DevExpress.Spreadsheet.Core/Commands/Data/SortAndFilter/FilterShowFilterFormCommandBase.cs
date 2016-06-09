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
using DevExpress.Utils.Commands;
using DevExpress.XtraSpreadsheet.Forms;
using DevExpress.XtraSpreadsheet.Model;
namespace DevExpress.XtraSpreadsheet.Commands {
	public abstract class ShowFilterFormCommandBase<TViewModel> : SpreadsheetMenuItemSimpleCommand {
		readonly DataSortOrFilterAccessor accessor;
		protected ShowFilterFormCommandBase(ISpreadsheetControl control)
			: base(control) {
			this.accessor = new DataSortOrFilterAccessor(DocumentModel);
		}
		protected virtual bool IsDateTimeFilter { get { return false; } }
		protected SheetViewSelection Selection { get { return DocumentModel.ActiveSheet.Selection; } }
		protected DataSortOrFilterAccessor Accessor { get { return accessor; } }
		protected override void UpdateUIStateCore(ICommandUIState state) {
			ApplyCommandRestrictionOnEditableControl(state, DocumentCapability.Default, !InnerControl.IsAnyInplaceEditorActive && !ActiveSheet.ReadOnly);
			ApplyActiveSheetProtection(state, !Protection.AutoFiltersLocked);
		}
		public virtual bool Validate(TViewModel viewModel) {
			return true;
		}
		public virtual void ApplyChanges(TViewModel viewModel) {
			ApplyFilter(viewModel);
		}
		void ApplyFilter(TViewModel viewModel) {
			CellRange range = Accessor.GetSortOrFilterRange();
			if (Accessor.Filter == null)
				return;
			DocumentModel.BeginUpdate();
			try {
				ModifyFilter(range, viewModel);
				ApplyFilter(Accessor.Filter);
				DocumentModel.ApplyChanges(DocumentModelChangeActions.RaiseContentChanged | DocumentModelChangeActions.RaiseModifiedChanged | DocumentModelChangeActions.Redraw | DocumentModelChangeActions.ResetSelectionLayout | DocumentModelChangeActions.ResetAllLayout | DocumentModelChangeActions.ResetCachedContentVersions | DocumentModelChangeActions.ResetCachedTransactionVersions);
			}
			finally {
				DocumentModel.EndUpdate();
			}
		}
		protected AutoFilterColumn GetFilterColumn(AutoFilterBase autoFilter, CellRange range) {
			int columnId = Selection.ActiveCell.Column - range.TopLeft.Column;
			return autoFilter.FilterColumns[columnId];
		}
		protected virtual void ModifyFilter(CellRange range, TViewModel viewModel) {
			AutoFilterColumn filterColumn = GetFilterColumn(accessor.Filter, range);
			filterColumn.Clear();
			AppendCustomFilters(filterColumn.CustomFilters, viewModel);
			ModifyFilterColumn(filterColumn, viewModel);
		}
		protected virtual void ApplyFilter(AutoFilterBase filter) {
			filter.ReApplyFilter();
		}
		protected virtual void ModifyFilterColumn(AutoFilterColumn filterColumn, TViewModel viewModel) {
		}
		protected virtual void AppendCustomFilters(CustomFilterCollection filters, TViewModel viewModel) {
		}
		protected internal abstract TViewModel CreateViewModel();
	}
	public enum GenericFilterOperator {
		None,
		Equals,
		DoesNotEqual,
		Greater,
		GreaterOrEqual,
		Less,
		LessOrEqual,
		BeginsWith,
		DoesNotBeginWith,
		EndsWith,
		DoesNotEndWith,
		Contains,
		DoesNotContain,
	}
	public abstract class ShowCustomFilterFormCommandBase : ShowFilterFormCommandBase<GenericFilterViewModel> {
		protected ShowCustomFilterFormCommandBase(ISpreadsheetControl control)
			: base(control) {
		}
		protected abstract GenericFilterOperator FilterOperator { get; }
		protected virtual bool OperatorAnd { get { return true; } }
		protected internal override void ExecuteCore() {
			Control.ShowGenericFilterForm(CreateViewModel());
		}
		protected internal override GenericFilterViewModel CreateViewModel() {
			GenericFilterViewModel result = new GenericFilterViewModel(Control);
			result.Command = this;
			result.IsDateTimeFilter = this.IsDateTimeFilter;
			SetupViewModel(result);
			return result;
		}
		protected virtual void SetupViewModel(GenericFilterViewModel viewModel) {
			viewModel.OperatorAnd = OperatorAnd;
			viewModel.FilterOperator = FilterOperator;
		}
		protected override void AppendCustomFilters(CustomFilterCollection filters, GenericFilterViewModel viewModel) {
			filters.CriterionAnd = viewModel.OperatorAnd;
			if (viewModel.FilterOperator != GenericFilterOperator.None)
				AppendCustomFilter(filters, viewModel.FilterOperator, viewModel.FilterValue);
			if (viewModel.SecondaryFilterOperator != GenericFilterOperator.None)
				AppendCustomFilter(filters, viewModel.SecondaryFilterOperator, viewModel.SecondaryFilterValue);
		}
		protected void AppendCustomFilter(CustomFilterCollection filters, GenericFilterOperator filterOperator, string value) {
			CustomFilter filter = CreateCustomFilter(filterOperator, value);
			if (filter == null)
				return;
			filters.Add(filter);
		}
		protected CustomFilter CreateCustomFilter(GenericFilterOperator filterOperator, string value) {
			CustomFilter result = new CustomFilter();
			result.FilterOperator = ConvertOperator(filterOperator);
			result.Value = ConvertValue(filterOperator, value);
			result.UpdateNumericValue(DocumentModel.DataContext, IsDateTimeFilter);
			if (result.Value == null)
				return null;
			return result;
		}
		FilterComparisonOperator ConvertOperator(GenericFilterOperator filterOperator) {
			switch (filterOperator) {
				default:
				case GenericFilterOperator.None:
				case GenericFilterOperator.Equals:
					return FilterComparisonOperator.Equal;
				case GenericFilterOperator.DoesNotEqual:
					return FilterComparisonOperator.NotEqual;
				case GenericFilterOperator.Greater:
					return FilterComparisonOperator.GreaterThan;
				case GenericFilterOperator.GreaterOrEqual:
					return FilterComparisonOperator.GreaterThanOrEqual;
				case GenericFilterOperator.Less:
					return FilterComparisonOperator.LessThan;
				case GenericFilterOperator.LessOrEqual:
					return FilterComparisonOperator.LessThanOrEqual;
				case GenericFilterOperator.BeginsWith:
					return FilterComparisonOperator.Equal;
				case GenericFilterOperator.DoesNotBeginWith:
					return FilterComparisonOperator.NotEqual;
				case GenericFilterOperator.EndsWith:
					return FilterComparisonOperator.Equal;
				case GenericFilterOperator.DoesNotEndWith:
					return FilterComparisonOperator.NotEqual;
				case GenericFilterOperator.Contains:
					return FilterComparisonOperator.Equal;
				case GenericFilterOperator.DoesNotContain:
					return FilterComparisonOperator.NotEqual;
			}
		}
		string ConvertValue(GenericFilterOperator filterOperator, string value) {
			switch (filterOperator) {
				default:
				case GenericFilterOperator.None:
					return null;
				case GenericFilterOperator.Equals:
					return value;
				case GenericFilterOperator.DoesNotEqual:
					return value;
				case GenericFilterOperator.Greater:
					return value;
				case GenericFilterOperator.GreaterOrEqual:
					return value;
				case GenericFilterOperator.Less:
					return value;
				case GenericFilterOperator.LessOrEqual:
					return value;
				case GenericFilterOperator.BeginsWith:
					return AppendStarAtEnd(value);
				case GenericFilterOperator.DoesNotBeginWith:
					return AppendStarAtEnd(value);
				case GenericFilterOperator.EndsWith:
					return AppendStarAtStart(value);
				case GenericFilterOperator.DoesNotEndWith:
					return AppendStarAtStart(value);
				case GenericFilterOperator.Contains:
					return AppendStarAtEnd(AppendStarAtStart(value));
				case GenericFilterOperator.DoesNotContain:
					return AppendStarAtEnd(AppendStarAtStart(value));
			}
		}
		string AppendStarAtEnd(string value) {
			return value.TrimEnd('*') + '*';
		}
		string AppendStarAtStart(string value) {
			return '*' + value.TrimStart('*');
		}
		bool HasFilter() {
			if (Accessor == null)
				return false;
			AutoFilterBase filter = Accessor.GetFilter();
			if (filter == null)
				return false;
			return filter.Enabled && filter.FilterColumns.Count > 0;
		}
		protected bool IsSortedOrFiltered() {
			if (!HasFilter())
				return false;
			AutoFilterBase filter = Accessor.GetFilter();
			if (!filter.SortState.IsDefault)
				return true;
			foreach (AutoFilterColumn column in filter.FilterColumns)
				if (column.IsNonDefault)
					return true;
			return false;
		}
	}
}
