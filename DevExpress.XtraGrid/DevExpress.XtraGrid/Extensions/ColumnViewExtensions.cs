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

namespace DevExpress.XtraGrid.Extensions {
	using System;
	using System.Linq.Expressions;
	using DevExpress.XtraGrid.Columns;
	using DevExpress.XtraGrid.Views.Base;
	public interface IColumnViewSettings<TRow> {
		IGridColumnCollectionSettings<TRow> Columns { get; }
	}
	public interface IGridColumnCollectionSettings<TRow> {
		IGridColumnSettings this[Expression<Func<TRow, object>> expression] { get; }
		IGridColumnSettings Add<TValue>(Expression<Func<TRow, TValue>> expression);
		IGridColumnSettings Add<TValue>(Expression<Func<TRow, TValue>> expression, string caption);
		IGridColumnCollectionSettings<TRow> Add<TValue>(Expression<Func<TRow, TValue>> expression, Action<GridColumn> columnSettings = null);
	}
	public interface IGridColumnSettings {
		GridColumn AsColumn();
		IGridColumnSettings AsInvisible();
		IGridColumnSettings WithCaption(string caption);
		IGridColumnSettings With(Action<GridColumn> columnSettings);
		IGridColumnSettings WithGrouping(ColumnGroupInterval interval = ColumnGroupInterval.Default);
		IGridColumnSettings WithSorting(bool ascending = true);
		IGridColumnSettings WithSummary(DevExpress.Data.SummaryItemType summaryItemType, string displayFormat = null, IFormatProvider formatProvider = null);
		IGridColumnSettings WithGroupSummary(DevExpress.Data.SummaryItemType summaryItemType, string displayFormat = null, IFormatProvider formatProvider = null);
		IGridColumnSettings WithGroupFooterSummary(DevExpress.Data.SummaryItemType summaryItemType, string displayFormat = null, IFormatProvider formatProvider = null);
	}
	public static class ColumnViewExtension {
		public static void With<TRow>(this ColumnView gridView, Action<IColumnViewSettings<TRow>> settings) {
			if(settings != null) settings(GetColumnViewSettings<TRow>(gridView));
		}
		public static IColumnViewSettings<TRow> GetColumnViewSettings<TRow>(this ColumnView gridView) {
			return new ColumnViewSettings<TRow>(gridView);
		}
		public static IGridColumnCollectionSettings<TRow> GetGridColumnCollectionSettings<TRow>(this GridColumnCollection columns) {
			return new GridColumnCollectionSettings<TRow>(columns);
		}
		public static IGridColumnSettings GetGridColumnSettings(this GridColumn column) {
			return new GridColumnSettings(column);
		}
		class ColumnViewSettings<TRow> : IColumnViewSettings<TRow> {
			ColumnView columnView;
			public ColumnViewSettings(ColumnView columnView) {
				this.columnView = columnView;
			}
			IGridColumnCollectionSettings<TRow> IColumnViewSettings<TRow>.Columns {
				get { return GetGridColumnCollectionSettings<TRow>(columnView.Columns); }
			}
		}
		class GridColumnCollectionSettings<TRow> : IGridColumnCollectionSettings<TRow> {
			GridColumnCollection columns;
			internal GridColumnCollectionSettings(GridColumnCollection columns) {
				this.columns = columns;
			}
			IGridColumnSettings IGridColumnCollectionSettings<TRow>.this[Expression<Func<TRow, object>> expression] {
				get { return GetGridColumnSettings(columns[DevExpress.Utils.MVVM.ExpressionHelper.GetPropertyName(expression)]); }
			}
			IGridColumnSettings IGridColumnCollectionSettings<TRow>.Add<TValue>(Expression<Func<TRow, TValue>> expression) {
				return GetGridColumnSettings(AddVisible<TValue>(expression));
			}
			IGridColumnSettings IGridColumnCollectionSettings<TRow>.Add<TValue>(Expression<Func<TRow, TValue>> expression, string caption) {
				return GetGridColumnSettings(AddVisible<TValue>(expression, caption));
			}
			IGridColumnCollectionSettings<TRow> IGridColumnCollectionSettings<TRow>.Add<TValue>(Expression<Func<TRow, TValue>> expression, Action<GridColumn> columnSettings) {
				GetGridColumnSettings(AddVisible<TValue>(expression)).With(columnSettings);
				return this;
			}
			GridColumn AddVisible<TValue>(Expression<Func<TRow, TValue>> expression, string caption) {
				return columns.AddVisible(DevExpress.Utils.MVVM.ExpressionHelper.GetPropertyName(expression), caption);
			}
			GridColumn AddVisible<TValue>(Expression<Func<TRow, TValue>> expression) {
				return columns.AddVisible(DevExpress.Utils.MVVM.ExpressionHelper.GetPropertyName(expression));
			}
		}
		class GridColumnSettings : IGridColumnSettings {
			GridColumn column;
			public GridColumnSettings(GridColumn column) {
				this.column = column;
			}
			GridColumn IGridColumnSettings.AsColumn() {
				return column;
			}
			IGridColumnSettings IGridColumnSettings.WithCaption(string caption) {
				if(column != null)
					column.Caption = caption;
				return this;
			}
			IGridColumnSettings IGridColumnSettings.AsInvisible() {
				if(column != null)
					column.Visible = false;
				return this;
			}
			IGridColumnSettings IGridColumnSettings.With(Action<GridColumn> columnSettings) {
				if(columnSettings != null && column != null)
					columnSettings(column);
				return this;
			}
			IGridColumnSettings IGridColumnSettings.WithGrouping(ColumnGroupInterval interval) {
				if(column != null && column.View != null) {
					column.GroupIndex = column.View.GroupedColumns.Count;
					column.GroupInterval = interval;
				}
				return this;
			}
			IGridColumnSettings IGridColumnSettings.WithSorting(bool ascending) {
				if(column != null)
					column.SortOrder = ascending ? DevExpress.Data.ColumnSortOrder.Ascending : DevExpress.Data.ColumnSortOrder.Descending;
				return this;
			}
			IGridColumnSettings IGridColumnSettings.WithSummary(DevExpress.Data.SummaryItemType summaryItemType, string displayFormat, IFormatProvider formatProvider) {
				if(column != null) {
					if(displayFormat == null)
						column.Summary.Add(summaryItemType);
					else
						column.Summary.Add(summaryItemType, column.FieldName, displayFormat, formatProvider);
				}
				return this;
			}
			IGridColumnSettings IGridColumnSettings.WithGroupSummary(DevExpress.Data.SummaryItemType summaryItemType, string displayFormat, IFormatProvider formatProvider) {
				var gridView = (column != null) ? column.View as DevExpress.XtraGrid.Views.Grid.GridView : null;
				if(gridView != null) {
					if(displayFormat == null)
						gridView.GroupSummary.Add(summaryItemType, column.FieldName);
					else
						gridView.GroupSummary.Add(summaryItemType, column.FieldName, null, displayFormat, formatProvider);
				}
				return this;
			}
			IGridColumnSettings IGridColumnSettings.WithGroupFooterSummary(DevExpress.Data.SummaryItemType summaryItemType, string displayFormat, IFormatProvider formatProvider) {
				var gridView = (column != null) ? column.View as DevExpress.XtraGrid.Views.Grid.GridView : null;
				if(gridView != null) {
					if(displayFormat == null)
						gridView.GroupSummary.Add(summaryItemType, column.FieldName, column);
					else
						gridView.GroupSummary.Add(summaryItemType, column.FieldName, column, displayFormat, formatProvider);
				}
				return this;
			}
		}
	}
}
#if DEBUGTEST
namespace DevExpress.XtraGrid.Extensions.Tests {
	using System;
	using NUnit.Framework;
	[TestFixture]
	public class ColumnViewExtension_Tests {
		class Employee {
			public int ID { get; private set; }
			public string FirstName { get; set; }
			public string LastName { get; set; }
			public int Age { get; set; }
		}
		[Test]
		public void Test_AddEmployee_Settings() {
			using(var gridView = new DevExpress.XtraGrid.Views.Grid.GridView()) {
				gridView.With<Employee>(settings =>
				{
					settings.Columns.Add(p => p.ID).AsInvisible();
					settings.Columns
						.Add(p => p.FirstName, (column) =>
						{
							column.Caption = "First Name";
							column.SortOrder = DevExpress.Data.ColumnSortOrder.Ascending;
							column.SortIndex = 0;
						})
						.Add(p => p.LastName)
							.WithCaption("Last Name")
							.With((column) =>
							{
								column.SortOrder = DevExpress.Data.ColumnSortOrder.Descending;
								column.SortIndex = 1;
							});
					settings.Columns.Add(p => p.Age)
						.WithGrouping()
						.WithSummary(DevExpress.Data.SummaryItemType.Max)
						.WithGroupSummary(DevExpress.Data.SummaryItemType.Sum);
				});
				AssertColumns(gridView);
				AssertSummaryAndGrouping(gridView);
			}
		}
		[Test]
		public void Test_AddEmployee_FromView() {
			using(var gridView = new DevExpress.XtraGrid.Views.Grid.GridView()) {
				var settings = gridView.GetColumnViewSettings<Employee>();
				settings.Columns.Add(p => p.ID).AsInvisible();
				settings.Columns
					.Add(p => p.FirstName, (column) =>
					{
						column.Caption = "First Name";
						column.SortOrder = DevExpress.Data.ColumnSortOrder.Ascending;
						column.SortIndex = 0;
					})
					.Add(p => p.LastName)
						.WithCaption("Last Name")
						.With((column) =>
						{
							column.SortOrder = DevExpress.Data.ColumnSortOrder.Descending;
							column.SortIndex = 1;
						});
				settings.Columns.Add(p => p.Age).WithGrouping()
					.WithSummary(DevExpress.Data.SummaryItemType.Max)
					.WithGroupSummary(DevExpress.Data.SummaryItemType.Sum);
				AssertColumns(gridView);
				AssertSummaryAndGrouping(gridView);
			}
		}
		[Test]
		public void Test_AddEmployee_FromColumns() {
			using(var gridView = new DevExpress.XtraGrid.Views.Grid.GridView()) {
				var sColumns = gridView.Columns.GetGridColumnCollectionSettings<Employee>();
				sColumns.Add(p => p.ID).AsInvisible();
				sColumns
					.Add(p => p.FirstName, (column) =>
					{
						column.Caption = "First Name";
						column.SortOrder = DevExpress.Data.ColumnSortOrder.Ascending;
						column.SortIndex = 0;
					})
					.Add(p => p.LastName)
						.WithCaption("Last Name")
						.With((column) =>
						{
							column.SortOrder = DevExpress.Data.ColumnSortOrder.Descending;
							column.SortIndex = 1;
						});
				sColumns.Add(p => p.Age).AsColumn().GroupIndex = 0;
				AssertColumns(gridView);
			}
		}
		static void AssertColumns(Views.Grid.GridView gridView) {
			Assert.AreEqual(4, gridView.Columns.Count);
			Assert.IsFalse(gridView.Columns["ID"].Visible);
			Assert.AreEqual("First Name", gridView.Columns["FirstName"].Caption);
			Assert.AreEqual(DevExpress.Data.ColumnSortOrder.Ascending, gridView.Columns["FirstName"].SortOrder);
			Assert.AreEqual(0, gridView.Columns["FirstName"].SortIndex);
			Assert.AreEqual("Last Name", gridView.Columns["LastName"].Caption);
			Assert.AreEqual(DevExpress.Data.ColumnSortOrder.Descending, gridView.Columns["LastName"].SortOrder);
			Assert.AreEqual(1, gridView.Columns["LastName"].SortIndex);
			Assert.AreEqual(0, gridView.Columns["Age"].GroupIndex);
		}
		static void AssertSummaryAndGrouping(Views.Grid.GridView gridView) {
			Assert.AreEqual(1, gridView.Columns["Age"].Summary.Count);
			Assert.AreEqual(DevExpress.Data.SummaryItemType.Max, gridView.Columns["Age"].Summary[0].SummaryType);
			Assert.AreEqual(1, gridView.GroupSummary.Count);
			Assert.AreEqual(DevExpress.Data.SummaryItemType.Sum, gridView.GroupSummary[0].SummaryType);
		}
	}
}
#endif
