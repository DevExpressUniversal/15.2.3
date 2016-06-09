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
using System.Linq;
using DevExpress.XtraExport.Helpers;
using DevExpress.Xpf.Editors.Settings;
using DevExpress.Data;
using DevExpress.Export.Xl;
using System.Collections;
using System.Text.RegularExpressions;
using DevExpress.Xpf.Editors;
using System.Windows;
namespace DevExpress.Xpf.Grid.Printing {
	public class ColumnWrapper : IColumn {
		public ColumnBase Column { get; private set; }
		protected DataViewBase View { get { return Column.View as DataViewBase; } }
		protected GridColumn GridColumn { get { return Column as GridColumn; } }
		public string Name {
			get { return GetHashCode().ToString(); }
		}
		public string FieldName {
			get { return Column.FieldName; }
		}
		public ISparklineInfo SparklineInfo {
			get {
				return ColEditType == ColumnEditTypes.Sparkline
					  ? new SparklineInfo(Column.EditSettings as SparklineEditSettings)
					  : null;
			}
		}
		public bool IsGroupColumn { get { return false; } }
		public IEnumerable<IColumn> GetAllColumns (){ return null;}
		public bool IsCollapsed { get { return false; } }
		public int GetColumnGroupLevel() {
			return 0;
		}
		public string GetGroupColumnHeader(){
			return null;
		}
		public string HyperlinkEditorCaption { get { return string.Empty; } }
		public string HyperlinkTextFormatString{
			get { return string.Empty; }
		}
		public IUnboundInfo UnboundInfo{
			get { return new UnboundColumnInfoWrapper(this.Column); }
		}
		XlCellFormatting formatingCellCore = null;
		public XlCellFormatting Appearance {
			get {
				if(formatingCellCore == null) formatingCellCore = new XlCellFormatting {
					Font = new XlFont(), Alignment = GetReportAlignment(Column.ActualHorizontalContentAlignment)
				};
				return formatingCellCore; 
			}   
		}
		XlCellAlignment GetReportAlignment(HorizontalAlignment actualAlignment) {
			XlCellAlignment result = new XlCellAlignment();
			switch(actualAlignment) {
				case HorizontalAlignment.Center:
					result.HorizontalAlignment = XlHorizontalAlignment.Center;
					break;
				case HorizontalAlignment.Left:
					result.HorizontalAlignment = XlHorizontalAlignment.Left;
					break;
				case HorizontalAlignment.Right:
					result.HorizontalAlignment = XlHorizontalAlignment.Right;
					break;
				case HorizontalAlignment.Stretch:
					result.HorizontalAlignment = XlHorizontalAlignment.Fill;
					break;
			}
			result.VerticalAlignment = XlVerticalAlignment.Center;
			return result;
		}
		XlCellFormatting formatingHeaderCore = null;
		public XlCellFormatting AppearanceHeader {
			get {
				if(formatingHeaderCore == null) formatingHeaderCore = new XlCellFormatting {
					Font = new XlFont(), Alignment = GetReportAlignment(Column.HorizontalHeaderContentAlignment)
				};
				return formatingHeaderCore;
			}
		}
		public FormatSettings FormatSettings{
			get{
				return new FormatSettings{
					ActualDataType=((IDataColumnInfo)Column).FieldType,
					FormatType = DevExpress.Utils.FormatType.Custom,
					FormatString = GetFormatString()
				};
			}
		}
		public ColumnSortOrder SortOrder{
			get { return Column.SortOrder; }
		}
		public ColumnEditTypes ColEditType {
			get { return GetColumnEditType(); }
		}
		public Type ColumnType {
			get { return ((IDataColumnInfo)Column).FieldType; }
		}
		public int LogicalPosition { get; private set; }
		public int Width {
			get {
				if(double.IsInfinity(Column.ActualHeaderWidth) || double.IsNaN(Column.ActualHeaderWidth))
					return 0;
				return Convert.ToInt32(Column.ActualHeaderWidth);
			}
		}
		public int VisibleIndex {
			get { return Column.VisibleIndex; }
		}
		public bool HasGroupIndex {
			get { return GridColumn != null ? GridColumn.GroupIndex >= 0 : false; }
		}
		public int GroupIndex {
			get { return  GridColumn.GroupIndex; }
		}
		public bool IsVisible {
			get { return Column.Visible; }
		}
		public IEnumerable<object> DataValidationItems {
			get { return GetColumnItemsSource(); }
		}
		public bool IsFixedLeft {
			get { return GetIsFixed(); }
		}
		public string Header {
			get { return ((DevExpress.Data.IDataColumnInfo)Column).Caption; }
		}
		public ColumnWrapper(ColumnBase column, int logicalPosition) {
			Column = column;
			LogicalPosition = logicalPosition;
		}
		bool GetIsFixed() {
			if(Column.Fixed == FixedStyle.Left)
				return true;
			if(View == null || Column.ParentBand == null)
				return false;
			return View.DataControl.BandsLayoutCore.GetRootBand(Column.ParentBand).Fixed == FixedStyle.Left;
		}
		IEnumerable<object> GetColumnItemsSource() {
			LookUpEditSettingsBase settings = Column.EditSettings as LookUpEditSettingsBase;
			if(settings == null || !(settings.ItemsSource is IEnumerable))
				return null;
			IEnumerable<object> items = ((IEnumerable)settings.ItemsSource).Cast<object>();
			return items.Select(item => View.GetExportValueFromItem(item, Column));
		}
		ColumnEditTypes GetColumnEditType() {
			if(Column.ActualEditSettings is CheckEditSettings)
				return ColumnEditTypes.CheckBox;
			if(Column.ActualEditSettings is ImageEditSettings)
				return ColumnEditTypes.Image;
			if(Column.ActualEditSettings is LookUpEditSettingsBase)
				return ColumnEditTypes.Lookup;
			if(Column.ActualEditSettings is SparklineEditSettings)
				return ColumnEditTypes.Sparkline;
			if(Column.ActualEditSettings is ProgressBarEditSettings)
				return ColumnEditTypes.ProgressBar;
			if(GridColumnTypeHelper.IsNumericType(ColumnType))
				return ColumnEditTypes.Number;
			return ColumnEditTypes.Text;
		}
		string GetFormatString() {
			if(Column.ActualEditSettings == null)
				return string.Empty;
			string mask = GetFormatStringByMask(Column.ActualEditSettings as TextEditSettings);
			if(!string.IsNullOrEmpty(mask))
				return mask;
			return GetFormatStringByDisplayFormat(Column.ActualEditSettings.DisplayFormat);
		}
		string GetFormatStringByMask(TextEditSettings settings) {
			if(settings != null) {
				bool isExportableMaskType = settings.MaskType == MaskType.DateTime || settings.MaskType == MaskType.DateTimeAdvancingCaret || settings.MaskType == MaskType.Numeric;
				if(settings.MaskUseAsDisplayFormat && isExportableMaskType)
					return settings.Mask;
			}
			return string.Empty;
		}
		string GetFormatStringByDisplayFormat(string displayFormat) {
			string pattern = "{0:.*}";
			if(!Regex.IsMatch(displayFormat, pattern))
				return string.Empty;
			int startIndex = displayFormat.IndexOf(':') + 1;
			int endIndex = displayFormat.IndexOf('}') - 1;
			return displayFormat.Substring(startIndex, endIndex - startIndex + 1);
		}
	}
	public class UnboundColumnInfoWrapper : IUnboundInfo {
		ColumnBase column;
		public UnboundColumnInfoWrapper(ColumnBase column){
			this.column = column;
		}
		public string UnboundExpression{
			get { return column.UnboundExpression; }
		}
		public UnboundColumnType UnboundType{
			get { return column.UnboundType; }
		}
	}
}
