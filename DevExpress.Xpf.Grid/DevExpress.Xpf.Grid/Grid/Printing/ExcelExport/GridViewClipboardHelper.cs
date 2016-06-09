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

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using DevExpress.Export.Xl;
using DevExpress.Xpf.Core.ConditionalFormatting.Native;
using DevExpress.Xpf.Grid.Native;
using DevExpress.XtraExport.Helpers;
using DevExpress.XtraPrinting;
namespace DevExpress.Xpf.Grid.Printing {
	public class GridViewClipboardHelper : GridViewExportHelperBase<ColumnWrapper, RowBaseWrapper>, IClipboardGridView<ColumnWrapper, RowBaseWrapper> {
		GridViewClipboardSelectionProvider provider;
		public GridViewClipboardHelper(TableView view, ExportTarget target = ExportTarget.Xlsx)
			: base(view, target) {
				this.provider = new GridViewClipboardSelectionProvider(this);
		}	   
		#region GridViewExportHelperBase<TCol, TRow> Members
		protected override IEnumerable<ISummaryItemEx> GetGroupHeaderSummary() {
			return Enumerable.Empty<ISummaryItemEx>();
		}
		protected override IEnumerable<ISummaryItemEx> GetGroupFooterSummary() {
			var groupRowSummary = GetGroupHeaderSummaryCore();
			var groupFooterSummary = PrintGroupFooters ? GetGroupFooterSummaryCore() : Enumerable.Empty<ISummaryItemEx>();
			return groupRowSummary.Concat(groupFooterSummary);
		}
		public override object GetRowCellValue(int rowHandle, int visibleIndex) {
			if(visibleIndex < 0 || visibleIndex >= View.VisibleColumns.Count)
				return null;
			ColumnBase column = View.VisibleColumns[visibleIndex];
			if((View.IsMultiRowSelection && View.IsRowSelected(rowHandle)) || provider.IsCellSelected(rowHandle, column))
				return View.GetExportValue(rowHandle, column) ?? string.Empty;
			return string.Empty;
		}
		#endregion
		#region IClipboardGridView<TCol,TRow> Members
		public bool CanCopyToClipboard() {
			return View.ClipboardMode == DevExpress.Xpf.Grid.ClipboardMode.Formatted;	
		}
		public Export.Xl.XlCellFormatting GetCellAppearance(RowBaseWrapper row, ColumnWrapper col) {
			if(col != null && row != null && provider.IsCellSelected(row.LogicalPosition, col.Column))
				return AppearanceHelper.GetCellAppearance(row.LogicalPosition, col.Column, View, FormatConditionsCore);
			return new XlCellFormatting() { Font = new XlFont(),  Fill = new XlFill(), Alignment= new XlCellAlignment() };
		}
		public string GetRowCellDisplayText(RowBaseWrapper row, string columnName) {
			int rowHandle = row.LogicalPosition;
			if((View.IsMultiRowSelection && View.IsRowSelected(rowHandle)) || provider.IsCellSelected(rowHandle, Grid.Columns[columnName]))
				return Grid.GetCellDisplayText(rowHandle, columnName);		  
			return string.Empty;
		}
		public void ProgressBarCallBack(int progress) {
		}
		#endregion
		public IEnumerable<RowBaseWrapper> GetSelectedRows() {
			return provider.GetSelectedRows();
		}
		public IEnumerable<ColumnWrapper> GetSelectedColumns() {
			return provider.GetSelectedColumnsList();
		}
		public int GetSelectedCellsCount() {
			return provider.GetSelectedCellsCountCore(null);
		}
		public bool UseHierarchyIndent(RowBaseWrapper row, ColumnWrapper col) {
			return false;
		}
	}
	public abstract class ClipboardSelectionProvider<TCol, TRow>
		where TCol : ColumnWrapper
		where TRow : IRowBase {
		public ClipboardSelectionProvider(DataViewBase view) {
			View = view;
		}
		protected DataViewBase View { get; private set; }
		protected DataControlBase DataControl { get { return View.DataControl; } }
		public string GetRowCellDisplayText(int rowHandle, string columnName) {
			if((View.IsMultiRowSelection && View.IsRowSelected(rowHandle)) || IsCellSelected(rowHandle, DataControl.ColumnsCore[columnName]))
				return DataControl.GetCellDisplayText(rowHandle, columnName);
			return string.Empty;
		}
		public IEnumerable<ColumnWrapper> GetSelectedColumns() {
			return GetSelectedColumnsList();
		}
		public int GetSelectedCellsCount() {
			return GetSelectedCellsCountCore(null);
		}
		Dictionary<ColumnBase, int> GetSelectedCells(int rowHandle) {
			return View.DataProviderBase.Selection.GetSelectedObject(rowHandle) as Dictionary<ColumnBase, int>;
		}
		public bool IsCellSelected(int rowHandle, ColumnBase column) {
			if(!View.IsMultiCellSelection)
				return View.IsRowSelected(rowHandle);
			Dictionary<ColumnBase, int> selectedColumns = GetSelectedCells(rowHandle);
			if(selectedColumns == null || !selectedColumns.ContainsKey(column))
				return false;
			return true;
		}
		public virtual int GetSelectedCellsCountCore(IGroupRow<TRow> groupRow) {
			int count = 0;
			IEnumerable<TRow> rows = null;
			if(groupRow == null)
				rows = GetSelectedRows();
			else
				rows = groupRow.GetAllRows();
			int countColumn = GetSelectedColumnsList().Count;
			foreach(TRow row in rows) {
				if(row is IGroupRow<TRow>)
					count += GetSelectedCellsCountCore(row as IGroupRow<TRow>);
				else
					count += countColumn;
			}
			return count;
		}
		IList<TCol> GetColumnsMultiCell() {
			int[] selectedRows = View.DataControl.GetSelectedRowHandles();
			IList<TCol> allColumn = GetColumns(DataControl.ColumnsCore);
			SortedList<int, TCol> result = new SortedList<int, TCol>();
			HashSet<BaseColumn> addedCol = new HashSet<BaseColumn>();
			for(int i = 0; i < selectedRows.Length; i++) {
				var selectedCellByRow = GetSelectedCells(selectedRows[i]);
				if(selectedCellByRow != null) {
					foreach(ColumnBase column in GetSelectedCells(selectedRows[i]).Keys) {
						if(column.Visible && !addedCol.Contains(column)) {
							result.Add(column.VisibleIndex, DataViewExportHelperBase<TCol, TRow>.CreateColumn(column, column.VisibleIndex));
							addedCol.Add(column);
						}
					}
					if(result.Count == allColumn.Count)
						return result.Values;
				}
			}
			if (result.Values.Count !=0 || selectedRows.Length == 0)
				return result.Values;
			if(selectedRows.Max() < 0) {
				allColumn = GetColumns(DataControl.ColumnsCore, true);
				foreach(var col in allColumn) {
					if(col.Column.Visible && !addedCol.Contains(col.Column) && col.Column.GroupIndexCore != -1 && !result.ContainsKey(col.Column.VisibleIndex)) {
						result.Add(col.Column.VisibleIndex, col);
						addedCol.Add(col.Column);
					}
				}
				return result.Values;
			}
			return new List<TCol>();
		}
		public IList<TCol> GetSelectedColumnsList() {
			if(View.IsMultiCellSelection)
				return GetColumnsMultiCell();
			return GetColumns(View.VisibleColumnsCore);
		}
		public abstract IEnumerable<TRow> GetSelectedRows();
		public abstract IList<TCol> GetColumns(IEnumerable collection, bool isGroupColumn = false);
	}
	public class GridViewClipboardSelectionProvider : ClipboardSelectionProvider<ColumnWrapper, RowBaseWrapper> {
		GridViewClipboardHelper helper;
		public GridViewClipboardSelectionProvider(GridViewClipboardHelper helper) : base(helper.View) {
			this.helper = helper;
		}
		protected new TableView View { get { return base.View as TableView; } }
		public override IList<ColumnWrapper> GetColumns(IEnumerable collection, bool isGroupColumn = false) {
			var res = new List<ColumnWrapper>();
			int gridColumnIndex = 0;
			foreach(GridColumn gridColumn in collection)
				if(CanAddColumn(gridColumn, isGroupColumn))
					res.Add(DataViewExportHelperBase<ColumnWrapper, RowBaseWrapper>.CreateColumn(gridColumn, gridColumnIndex++));
			return res;
		}
		public override IEnumerable<RowBaseWrapper> GetSelectedRows() {
			int[] selectedRows = View.DataControl.GetSelectedRowHandles();
			System.Array.Sort(selectedRows, (i1, i2) => View.DataControl.GetRowVisibleIndexByHandleCore(i1).CompareTo(View.DataControl.GetRowVisibleIndexByHandleCore(i2)));
			return selectedRows.Select(i => helper.GetRowByRowHandle(i)); 
		}
		bool CanAddColumn(GridColumn gridColumn, bool isGroupColumn) {
			bool canExportColumn = isGroupColumn || View.ShowGroupedColumns || gridColumn.GroupIndex == -1;
			if(gridColumn == View.CheckBoxSelectorColumn && !View.ShowCheckBoxSelectorColumn)
				return false;
			return canExportColumn;
		}
	}
	public static class AppearanceHelper {
		class ConditionalFormattingProperties{
			public const int DefaultFontSize = 11;
			public ConditionalFormattingProperties() {
				FontSize = DefaultFontSize;
				FontWeight = FontWeights.Normal;
				FontStyle = FontStyles.Normal;
				Background = null;
				Foreground = null;
				TextDecor = null;
			}
			public double FontSize {get;set;}
			public FontWeight FontWeight {get;set;}
			public FontStyle FontStyle {get;set;}
			public Brush Background {get;set;}
			public Brush Foreground {get;set;}
			public TextDecorationCollection TextDecor {get;set;}
		}
		public static System.Drawing.Color BrushToColor(Brush br) {
			if(br == null)
				return System.Drawing.Color.Empty;
			Color clr =  (Color)br.GetValue(SolidColorBrush.ColorProperty);
			byte a = clr.A;
			byte g = clr.G;
			byte r = clr.R;
			byte b = clr.B;
			return System.Drawing.Color.FromArgb((int)a, (int)r, (int)g, (int)b);
		}
		static IFormatInfoProvider CreateFormatInfoProvider(DataControlBase dataControl, int rowHandle) {
			return new DataTreeBuilderFormatInfoProvider<DataControlBase>(dataControl, x => x.DataView.VisualDataTreeBuilder, (x, fieldName) => x.GetCellValue(rowHandle, fieldName));
		}
		static FormatValueProvider CalcCondition(DataControlBase dataControl, int rowHandle, string fieldName) {
			return CreateFormatInfoProvider(dataControl, rowHandle).GetValueProvider(fieldName);
		}
		static ConditionalFormattingProperties GetValues(IList<FormatConditionBaseInfo> conditions, DataViewBase view, int rowHandle, string fieldName, ConditionalFormattingProperties oldProp) {
			if(conditions != null && conditions.Count > 0) {
				ConditionalFormattingProperties properties = oldProp ?? new ConditionalFormattingProperties();
				foreach(var condintionalInfo in conditions) {
					var provider = CalcCondition(view.DataControl, rowHandle, condintionalInfo.ActualFieldName);
					double sizeTemp = condintionalInfo.CoerceFontSize(11, provider);					
					FontWeight tempWeight = condintionalInfo.CoerceFontWeight(FontWeights.Normal, provider);
					FontStyle tempStyle = condintionalInfo.CoerceFontStyle(FontStyles.Normal, provider);
					properties.FontSize = sizeTemp != ConditionalFormattingProperties.DefaultFontSize ? sizeTemp : properties.FontSize;
					properties.FontWeight = tempWeight != FontWeights.Normal ? tempWeight : properties.FontWeight;
					properties.FontStyle = tempStyle != FontStyles.Normal ? tempStyle : properties.FontStyle;
					properties.Background = condintionalInfo.CoerceBackground(null, provider) ?? properties.Background;
					properties.Foreground = condintionalInfo.CoerceForeground(null, provider) ?? properties.Foreground;
					properties.TextDecor = condintionalInfo.CoerceTextDecorations(null, provider) ?? properties.TextDecor;
				}
				return properties;
			}
			return oldProp;
		}
		public static Export.Xl.XlCellFormatting GetCellAppearance(int rowHandle, ColumnBase col, DataViewBase view, FormatConditionCollection formatConditions) {		 
			ConditionalFormattingProperties properties = null;
			if(view.ViewBehavior != null && view.ViewBehavior.IsAlternateRow(rowHandle)) {
				properties = new ConditionalFormattingProperties();
				properties.Background = view.ViewBehavior.ActualAlternateRowBackground;			  
			}
			properties = AppearanceHelper.GetValues(formatConditions.GetInfoByFieldName(string.Empty), view, rowHandle, col.FieldName, properties);
			properties = AppearanceHelper.GetValues(formatConditions.GetInfoByFieldName(col.FieldName), view, rowHandle, col.FieldName,properties);
			if(properties != null)
				return AppearanceHelper.GetCellFormatting(properties);
			return new XlCellFormatting() { Font = new XlFont(), Fill = new XlFill(), Alignment= new XlCellAlignment() };
		}
		static XlFill SetFill(Brush background) {
			XlFill result = new XlFill();
			if(background != null) {
				XlColor clr = BrushToColor(background);
				result.BackColor = clr;
				result.ForeColor = clr;
				result.PatternType = XlPatternType.Solid;
			}						  
			return result;
		}
		static Export.Xl.XlCellFormatting GetCellFormatting(ConditionalFormattingProperties properties) {		   
			XlCellFormatting result = new XlCellFormatting();
			result.Font = new XlFont() {
				Bold = properties.FontWeight == FontWeights.Bold,
				Size = properties.FontSize,
				Italic = properties.FontStyle == FontStyles.Italic				
			};
			if(properties.Foreground != null)
				result.Font.Color = BrushToColor(properties.Foreground);
			if(properties.TextDecor != null) {
				foreach(var decor in properties.TextDecor) {
					switch(decor.Location) {
						case TextDecorationLocation.Strikethrough:
							result.Font.StrikeThrough = true;
							break;
						case TextDecorationLocation.Underline:
							result.Font.Underline = XlUnderlineType.Single;
							break;
						default:
							break;
					}
				}						  
			}
			result.Fill = SetFill(properties.Background);
			result.Alignment = new XlCellAlignment();
			return result;
		}
	}
}
