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

using DevExpress.Data.Summary;
using DevExpress.Utils;
using DevExpress.Export.Xl;
using System;
using System.Collections.Generic;
using System.Drawing;
using DevExpress.Compatibility.System.Drawing;
using System.Windows.Forms;
using DevExpress.Compatibility.System.Windows.Forms;
using DevExpress.Data;
namespace DevExpress.XtraExport.Helpers {
	public enum FormatConditions {
		None, Equal, NotEqual, Between, NotBetween, Less, Greater,
		GreaterOrEqual, LessOrEqual, Expression
	}
	public enum ColumnEditTypes {
		Text,
		CheckBox,
		Number,
		ProgressBar,
		Lookup,
		Sparkline,
		Image,
		Hyperlink
	}
	public class FormatConditionObject {
		XlDifferentialFormatting appearance;
		FormatConditions condition;
		bool applyToRow;
		string columnName;
		string expression;
		object value1;
		object value2;
		#region Properties
		public XlDifferentialFormatting Appearance { get { return appearance; } set { appearance = value; } }
		public FormatConditions Condition { get { return condition; } set { condition = value; } }
		public bool ApplyToRow { get { return applyToRow; } set { applyToRow = value; } }
		public string ColumnName { get { return columnName; } set { columnName = value; } }
		public string Expression { get { return expression; } set { expression = value; } }
		public object Value1 { get { return value1; } set { value1 = value; } }
		public object Value2 { get { return value2; } set { value2 = value; } }
		#endregion Properties
	}
	public class FormatSettings {
		public string FormatString { get; set; }
		public FormatType FormatType { get; set; }
		public Type ActualDataType { get; set; }
	}
	public interface IFormatRuleBase {
		bool StopIfTrue { get; }
		bool Enabled { get; }
		bool ApplyToRow { get; }
		IColumn Column { get; }
		IColumn ColumnApplyTo { get; }
		string ColumnName { get; }
		string Name { get; }
		IFormatConditionRuleBase Rule { get; }
		object Tag { get; set; }
	}
	public interface IFormatConditionRuleBase {
	}
	public interface IFormatConditionRuleMinMaxBase {
		XlCondFmtValueObjectType MaxType { get; }
		XlCondFmtValueObjectType MinType { get; }
		object MaxValue { get; }
		object MinValue { get; }
	}
	public interface IFormatConditionRuleExpression :IFormatConditionRuleBase {
		XlDifferentialFormatting Appearance { get; }
		string Expression { get; }
	}
	public interface IFormatConditionRuleDataBar :IFormatConditionRuleBase, IFormatConditionRuleMinMaxBase {
		Color AxisColor { get; }
		Color FillColor { get; }
		Color BorderColor { get; }
		Color NegativeFillColor { get; }
		Color NegativeBorderColor { get; }
		bool AllowNegativeAxis { get; }
		bool ShowBarOnly { get; }
		bool GradientFill { get; }
		bool DrawAxis { get; }
		bool DrawAxisAtMiddle { get; }
		string PredefinedName { get; }
		int Direction { get; }
	}
	public interface IFormatConditionRuleValue :IFormatConditionRuleBase {
		XlDifferentialFormatting Appearance { get; }
		FormatConditions Condition { get; }
		string Expression { get; }
		object Value1 { get; }
		object Value2 { get; }
	}
	public interface IFormatConditionRuleAboveBelowAverage :IFormatConditionRuleBase {
		XlCondFmtAverageCondition Condition { get; }
		XlDifferentialFormatting Formatting { get; }
	}
	public interface IFormatConditionRuleTopBottom :IFormatConditionRuleBase {
		int Rank { get; }
		bool Bottom { get; }
		bool Percent { get; }
		XlDifferentialFormatting Appearance { get; }
	}
	public interface IFormatConditionRuleContains :IFormatConditionRuleBase {
		System.Collections.IList Values { get; }
		XlDifferentialFormatting Appearance { get; }
	}
	public interface IFormatConditionRuleUniqueDuplicate :IFormatConditionRuleBase {
		bool Unique { get; }
		bool Duplicate { get; }
		XlDifferentialFormatting Formatting { get; }
	}
	#region IconSets
	public interface IFormatConditionRuleIconSet :IFormatConditionRuleBase {
		bool Percent { get; }
		bool Reverse { get; }
		bool ShowValues { get; }
		XlCondFmtIconSetType IconSetType { get; }
		IList<XlCondFmtValueObject> Values { get; }
	}
	#endregion
	#region ColorScale
	public interface IFormatConditionRuleColorScaleBase :IFormatConditionRuleBase, IFormatConditionRuleMinMaxBase {
		Color MinColor { get; }
		Color MaxColor { get; }
	}
	public interface IFormatConditionRule2ColorScale :IFormatConditionRuleColorScaleBase { }
	public interface IFormatConditionRule3ColorScale :IFormatConditionRuleColorScaleBase {
		Color MidpointColor { get; }
		XlCondFmtValueObjectType MidpointType { get; }
		object MidpointValue { get; }
	}
	#endregion Color Scale
	#region DateOccuring
	public interface IFormatConditionRuleDateOccuring :IFormatConditionRuleBase {
		XlCondFmtTimePeriod DateType { get; }
		XlDifferentialFormatting Formatting { get; }
	}
	#endregion
	public interface ISummaryItemEx :ISummaryItem {
		object GetSummaryValueByGroupId(int groupId);
		object SummaryValue { get; }
		string ShowInColumnFooterName { get; }
	}
	public interface IGridViewBase<out TCol, out TRow, in TColIn, in TRowIn>
		where TRow : IRowBase
		where TCol : IColumn
		where TRowIn : IRowBase
		where TColIn : IColumn {
		object GetRowCellValue(TRowIn row, TColIn col);
		FormatSettings GetRowCellFormatting(TRowIn row, TColIn col);
		string GetRowCellHyperlink(TRowIn row, TColIn col);
		bool GetAllowMerge(TColIn col);
		int RaiseMergeEvent(int startRow, int rowLogicalPosition, TColIn col);
	}
	public interface IAdditionalSheetInfo {
		string Name { get; }
		XlSheetVisibleState VisibleState { get; }
	}
	public interface IGridView<TCol, TRow> :IGridViewBase<TCol, TRow, TCol, TRow>
		where TRow : IRowBase
		where TCol : IColumn {
		IEnumerable<TCol> GetGroupedColumns();
		IEnumerable<TCol> GetAllColumns();
		IEnumerable<TRow> GetAllRows();
		int RowHeight { get; }
		int FixedRowsCount { get; }
		long RowCount { get; }
		bool ShowFooter { get; }
		bool ShowGroupFooter { get; }
		bool ReadOnly { get; }
		bool IsCancelPending { get; }
		string GetViewCaption { get; }
		string FilterString { get; }
		bool ShowGroupedColumns { get; }
		IAdditionalSheetInfo AdditionalSheetInfo { get; }
		IEnumerable<ISummaryItemEx> GridGroupSummaryItemCollection { get; }
		IEnumerable<ISummaryItemEx> GridTotalSummaryItemCollection { get; }
		IEnumerable<FormatConditionObject> FormatConditionsCollection { get; }
		IEnumerable<IFormatRuleBase> FormatRulesCollection { get; }
		IEnumerable<ISummaryItemEx> GroupHeaderSummaryItems { get; }
		IEnumerable<ISummaryItemEx> FixedSummaryItems { get; }
		XlCellFormatting AppearanceGroupRow { get; }
		XlCellFormatting AppearanceEvenRow { get; }
		XlCellFormatting AppearanceOddRow { get; }
		XlCellFormatting AppearanceGroupFooter { get; }
		XlCellFormatting AppearanceHeader { get; }
		XlCellFormatting AppearanceFooter { get; }
		XlCellFormatting AppearanceRow { get; }
		IGridViewAppearancePrint AppearancePrint { get; }
	}
	public interface IGridViewAppearancePrint {
		XlCellFormatting AppearanceEvenRow { get; }
		XlCellFormatting AppearanceOddRow { get; }
		XlCellFormatting AppearanceGroupRow { get; }
		XlCellFormatting AppearanceFooterPanel { get; }
		XlCellFormatting AppearanceGroupFooter { get; }
		XlCellFormatting AppearanceRow { get; }
		XlCellFormatting AppearanceHeader { get; }
	}
	public interface IRowBase {
		int LogicalPosition { get; }
		int DataSourceRowIndex { get; }
		int GetRowLevel();
		bool IsGroupRow { get; }
		bool IsDataAreaRow { get; }
		FormatSettings FormatSettings { get; }
	}
	public interface IGroupRow<out TRow> :IRowBase
			where TRow : IRowBase {
		string GetGroupRowHeader();
		IEnumerable<TRow> GetAllRows();
		bool IsCollapsed { get; }
	}
	public interface IUnboundInfo {
		string UnboundExpression { get; }
		UnboundColumnType UnboundType { get; }
	}
	public interface ISparklineInfo {
		ColumnSortOrder PointSortOrder { get;}
		XlSparklineType SparklineType { get; }
		Color ColorSeries { get; }
		Color ColorNegative { get; }
		Color ColorMarker { get;}
		Color ColorFirst { get;}
		Color ColorLast { get; }
		Color ColorHigh { get;}
		Color ColorLow { get; }
		double LineWeight { get; }
		bool HighlightNegative { get; }
		bool HighlightFirst { get; }
		bool HighlightLast { get; }
		bool HighlightHighest { get; }
		bool HighlightLowest { get; }
		bool DisplayMarkers { get; }
	}
	public interface IColumn {
		string Header { get; }
		string Name { get; }
		string FieldName { get; }
		string HyperlinkEditorCaption { get; }
		string HyperlinkTextFormatString { get; }
		string GetGroupColumnHeader();
		int LogicalPosition { get; }
		int Width { get; }
		int GroupIndex { get; }
		bool IsVisible { get; }
		bool IsFixedLeft { get; }
		ISparklineInfo SparklineInfo { get; }
		ColumnSortOrder SortOrder { get; }
		IUnboundInfo UnboundInfo { get; }
		XlCellFormatting Appearance { get; }
		XlCellFormatting AppearanceHeader { get; }
		ColumnEditTypes ColEditType { get; }
		FormatSettings FormatSettings { get; }
		IEnumerable<object> DataValidationItems { get; }
		IEnumerable<IColumn> GetAllColumns();
		bool IsCollapsed { get; }
		bool IsGroupColumn { get; }
		int VisibleIndex { get; }
		int GetColumnGroupLevel();
	}
	public interface IGridViewFactory<TCol, TRow>
		where TRow : IRowBase
		where TCol : IColumn {
		IGridView<TCol, TRow> GetIViewImplementerInstance();
		void ReleaseIViewImplementerInstance(IGridView<TCol, TRow> instance);
		object GetDataSource();
		string GetDataMember();
	}
}
