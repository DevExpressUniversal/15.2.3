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
using System.Drawing;
using System.Linq;
using DevExpress.Data;
using DevExpress.Data.XtraReports.ReportGeneration;
using DevExpress.Export.Xl;
using DevExpress.Office;
using DevExpress.XtraExport.Helpers;
using DevExpress.XtraPrinting;
using DevExpress.XtraReports.UI;
using DevExpress.XtraReports.ReportGeneration;
namespace DevExpress.XtraReports.ReportGeneration {
	struct Alignment {
		public XlVerticalAlignment VertAlignment;
		public XlHorizontalAlignment HorzAlignment;
	}
	struct RowContainer {
		public IRowBase Row;
		public int Id;
	}
	class SummaryItemCompare : IEqualityComparer<ISummaryItemEx> {
		public bool Equals(ISummaryItemEx x, ISummaryItemEx y) {
			if(x.SummaryType != y.SummaryType)
				return false;
			if(x.FieldName != y.FieldName)
				return false;
			if(x.DisplayFormat != y.DisplayFormat)
				return false;
			if(x.ShowInColumnFooterName != y.ShowInColumnFooterName)
				return false;
			if(x.SummaryValue != y.SummaryValue)
				return false;
			return true;
		}
		public int GetHashCode(ISummaryItemEx item) {
			return item.SummaryType.GetHashCode() ^ item.FieldName.GetHashCode();
		}
	}
	class XRGroupBandContainer {
		public XRGroupBandContainer(GroupHeaderBand headerband, GroupFooterBand footerband, IColumn column){
			HeaderBand = headerband;
			Column = column;
			FooterBand = footerband;
		}
		public GroupHeaderBand HeaderBand { get; private set; }
		public GroupFooterBand FooterBand { get; private set; }
		public IColumn Column { get; private set; }
	}
	class RowsProcessor <TCol, TRow>
		where TRow : IRowBase
		where TCol : IColumn {
		IGridView<TCol, TRow> view;
		Dictionary<int, List<RowContainer>> rowsByLevel;
		int rowId = 1;
		public RowsProcessor(IGridView<TCol, TRow> view){
			this.view = view;
		}
		public Dictionary<int, List<RowContainer>> RowsByLevel{
			get{
				if(rowsByLevel == null){
					rowsByLevel = new Dictionary<int, List<RowContainer>>();
					DepthFirstSearch(null, AddToCache);
				}
				return rowsByLevel;
			}
		}
		void DepthFirstSearch(IGroupRow<TRow> parent, Action<IGroupRow<TRow>, int> action){
			IEnumerable<TRow> iterator;
			if(parent == null) iterator = view.GetAllRows();
			else iterator = parent.GetAllRows();
			foreach(var row in iterator){
				IGroupRow<TRow> groupRow = row as IGroupRow<TRow>;
				if(groupRow != null){
					action(groupRow, rowId);
					rowId++;
					DepthFirstSearch(groupRow, action);
				}
			}
		}
		void AddToCache(IGroupRow<TRow> groupRow, int id){
			List<RowContainer> lst;
			if(rowsByLevel.TryGetValue(groupRow.GetRowLevel(), out lst)){
				lst.Add(new RowContainer{ Row = groupRow, Id = id });
			} else{
				lst = new List<RowContainer>();
				lst.Add(new RowContainer{ Row = groupRow, Id = id });
				rowsByLevel.Add(groupRow.GetRowLevel(), lst);
			}
		}
		public List<RowContainer> GetRowsByLevel(int level){
			List<RowContainer> rows;
			if(RowsByLevel.TryGetValue(level, out rows)) return rows;
			return null;
		}
	}
	static class Converter {
		static Dictionary<Alignment, TextAlignment> alignmentTable = GetAlignmentTable();
		static Dictionary<Alignment, TextAlignment> GetAlignmentTable(){
			var dict = new Dictionary<Alignment, TextAlignment>();
			dict.Add(new Alignment{
				HorzAlignment = XlHorizontalAlignment.Center,
				VertAlignment = XlVerticalAlignment.Bottom
			}, TextAlignment.BottomCenter);
			dict.Add(new Alignment{
				HorzAlignment = XlHorizontalAlignment.Left,
				VertAlignment = XlVerticalAlignment.Bottom
			}, TextAlignment.BottomLeft);
			dict.Add(new Alignment{
				HorzAlignment = XlHorizontalAlignment.Right,
				VertAlignment = XlVerticalAlignment.Bottom
			}, TextAlignment.BottomRight);
			dict.Add(new Alignment{
				HorzAlignment = XlHorizontalAlignment.Justify,
				VertAlignment = XlVerticalAlignment.Bottom
			}, TextAlignment.BottomJustify);
			dict.Add(new Alignment{
				HorzAlignment = XlHorizontalAlignment.Left,
				VertAlignment = XlVerticalAlignment.Center
			}, TextAlignment.MiddleLeft);
			dict.Add(new Alignment{
				HorzAlignment = XlHorizontalAlignment.Right,
				VertAlignment = XlVerticalAlignment.Center
			}, TextAlignment.MiddleRight);
			dict.Add(new Alignment{
				HorzAlignment = XlHorizontalAlignment.Center,
				VertAlignment = XlVerticalAlignment.Center
			}, TextAlignment.MiddleCenter);
			dict.Add(new Alignment{
				HorzAlignment = XlHorizontalAlignment.Justify,
				VertAlignment = XlVerticalAlignment.Center
			}, TextAlignment.MiddleJustify);
			dict.Add(new Alignment{
				HorzAlignment = XlHorizontalAlignment.Center,
				VertAlignment = XlVerticalAlignment.Top
			}, TextAlignment.TopCenter);
			dict.Add(new Alignment{
				HorzAlignment = XlHorizontalAlignment.Justify,
				VertAlignment = XlVerticalAlignment.Top
			}, TextAlignment.TopJustify);
			dict.Add(new Alignment{
				HorzAlignment = XlHorizontalAlignment.Left,
				VertAlignment = XlVerticalAlignment.Top
			}, TextAlignment.TopLeft);
			dict.Add(new Alignment{
				HorzAlignment = XlHorizontalAlignment.Right,
				VertAlignment = XlVerticalAlignment.Top
			}, TextAlignment.TopRight);
			return dict;
		}
		public static SummaryFunc GetXRSummaryFunc(SummaryItemType type){
			switch(type){
				case SummaryItemType.Average:
					return SummaryFunc.Avg;
				case SummaryItemType.Count:
					return SummaryFunc.Count;
				case SummaryItemType.Max:
					return SummaryFunc.Max;
				case SummaryItemType.Min:
					return SummaryFunc.Min;
				case SummaryItemType.Sum:
					return SummaryFunc.Sum;
				default:
					return SummaryFunc.Custom;
			}
		}
		public static TextAlignment ConvertAlignment(XlCellAlignment al){
			var defaultAl = TextAlignment.MiddleLeft;
			if (al == null) return defaultAl;
			TextAlignment result;
			var alignment = new Alignment{
				VertAlignment = al.VerticalAlignment,
				HorzAlignment = al.HorizontalAlignment
			};
			if(!alignmentTable.TryGetValue(alignment, out result)){
				return defaultAl;
			}
			return result;
		}
		public static void ConvertFont(XRControl control, XlFont font){
			if(!font.Equals(new XlFont())){
				control.Font = new Font(GetCorrectFontName(font), (float) font.Size, GetFontStyle(font));
				control.CanGrow = true;
			}
		}
		private static string GetCorrectFontName(XlFont font) {
			if(font.Name == "Calibri") return "Times New Roman";
			return font.Name;
		}
		public static FontStyle GetFontStyle(XlFont font){
			if(font.Italic) return FontStyle.Italic;
			if(font.Bold) return FontStyle.Bold;
			if(font.Underline != XlUnderlineType.None) return FontStyle.Underline;
			if(font.StrikeThrough) return FontStyle.Strikeout;
			return FontStyle.Regular;
		}
		public static void GetColorSettings(XRControl control, XlCellFormatting style){
			if(style.Fill != null){
				control.BackColor = style.Fill.BackColor;
			}
			control.ForeColor = style.Font.Color;
		}
		public static FieldType GetUnboundFieldType(UnboundColumnType type){
			switch(type){
				case UnboundColumnType.Boolean:
					return FieldType.Boolean;
				case UnboundColumnType.DateTime:
					return FieldType.DateTime;
				case UnboundColumnType.Decimal:
					return FieldType.Decimal;
				case UnboundColumnType.Integer:
					return FieldType.Int32;
				case UnboundColumnType.String:
					return FieldType.String;
				default:
					return FieldType.None;
			}
		}
		public static BorderDashStyle ConvertBorderStyle(XlBorderLineStyle bottomLineStyle){
			switch(bottomLineStyle){
				case XlBorderLineStyle.DashDot:
					return BorderDashStyle.DashDot;
				case XlBorderLineStyle.DashDotDot:
					return BorderDashStyle.DashDotDot;
				case XlBorderLineStyle.Dashed:
					return BorderDashStyle.Dash;
				case XlBorderLineStyle.Dotted:
					return BorderDashStyle.Dot;
				case XlBorderLineStyle.Double:
					return BorderDashStyle.Double;
				case XlBorderLineStyle.Medium:
					return BorderDashStyle.Solid;
				default:
					return BorderDashStyle.Solid;
			}
		}
		public static Color GetBorderColor(XlBorder border) {
			if(border != null) {
				return border.BottomColor;
			}
			return Color.Empty;
		}
		public static XRControlStyle ConvertToStyle(XlCellFormatting formatting,string styleName) {
			var style = new XRControlStyle();
			style.Name = styleName;
			if (formatting == null) return style;
			if (formatting.Border != null) {
				style.BorderColor = Converter.GetBorderColor(formatting.Border);
				style.BorderDashStyle = Converter.ConvertBorderStyle(formatting.Border.BottomLineStyle);
			}
			if(formatting.Fill != null) {
				style.BackColor = formatting.Fill.BackColor;
			}
			if(formatting.Font != null && !formatting.Font.Equals(new XlFont())) {
				style.ForeColor = formatting.Font.Color;
				style.Font = new Font(GetCorrectFontName(formatting.Font), GetCorrectFontSize(formatting), GetFontStyle(formatting.Font));
			}
			style.TextAlignment = Converter.ConvertAlignment(formatting.Alignment);
			return style;
		}
		private static float GetCorrectFontSize(XlCellFormatting formatting) {
			float fs = (float) formatting.Font.Size;
			return Math.Abs(fs - 11.0) < 0.01 ? 8.25f : fs;
		}
		public static int PaddingLeft { get { return 4; } }
		public static int PaddingRight { get { return 4; } }
		public static int PaddingTop { get { return 0; } }
		public static int PaddingBottom { get { return 0; } }
		public static PaddingInfo CalcPaddingInfo(ReportUnit unit,bool ignoreRigntPadding) {
			int rightPadding = 4;
			if(ignoreRigntPadding) rightPadding = 0;
			return new PaddingInfo(PaddingLeft, rightPadding, PaddingTop, PaddingBottom);
		}
	}
	internal static class GeneratorUtils {
		internal static void SetCellStyle(XRControl control, XlCellFormatting style){
			if(style == null) return;
			Converter.GetColorSettings(control, style);
			control.TextAlignment = Converter.ConvertAlignment(style.Alignment);
			control.WordWrap = style.Alignment != null && style.Alignment.WrapText;
			control.Padding = Converter.CalcPaddingInfo(ReportUnit.HundredthsOfAnInch,false);
			if(style.Border != null) {
				control.BorderColor = Converter.GetBorderColor(style.Border);
				control.BorderDashStyle = Converter.ConvertBorderStyle(style.Border.BottomLineStyle);
			}
			Converter.ConvertFont(control, style.Font);
		}
		internal static void CorrectCellTextAlingment(XRTableCell cell,IColumn col) {
			if(col.FormatSettings.ActualDataType == typeof(string) || col.FormatSettings.ActualDataType == typeof(Guid)) {
				cell.TextAlignment = TextAlignment.MiddleLeft;
			} else cell.TextAlignment = TextAlignment.MiddleRight;
		}
		internal static string GetDataMember(XtraReport report, string fieldName){
			if(string.IsNullOrEmpty(report.DataMember)) return fieldName;
			return report.DataMember + "." + fieldName;
		}
		public static string NormalizeFormatString(string formatstring){
			return "{0:" + formatstring + "}";
		}
		public static bool IsTrue(Utils.DefaultBoolean value) {
			return value == Utils.DefaultBoolean.True || value == Utils.DefaultBoolean.Default;
		}
	}
	class StylesExtensions<TCol, TRow> where TRow : IRowBase where TCol : IColumn {
		ReportGenerationModel<TCol, TRow> model;
		public StylesExtensions(ReportGenerationModel<TCol, TRow> model) {
			this.model = model;
		}
		public static XlCellFormatting GetDefaultFormat() {
			var defaultFormat = new XlCellFormatting();
			defaultFormat.Font = new XlFont();
			defaultFormat.Font.Bold = false;
			defaultFormat.Font.StrikeThrough = false;
			defaultFormat.Font.Underline = XlUnderlineType.None;
			defaultFormat.Font.Italic = false;
			defaultFormat.Font.FontFamily = XlFontFamily.Swiss;
			defaultFormat.Font.Name = "Calibri";
			defaultFormat.Font.Size = 11.0;
			defaultFormat.Font.SchemeStyle = XlFontSchemeStyles.Minor;
			defaultFormat.Alignment = new XlCellAlignment();
			defaultFormat.Alignment.HorizontalAlignment = XlHorizontalAlignment.General;
			defaultFormat.Alignment.VerticalAlignment = XlVerticalAlignment.Center;
			return defaultFormat;
		}
		public static bool Equals(XlCellFormatting obj1, XlCellFormatting obj2) {
			if (obj1 == null && obj2 == null) return true;
			if (obj1 == null || obj2 == null) return false;
			if (!AlignmentEquals(obj1.Alignment, obj2.Alignment)) return false;
			if (!BorderEquals(obj1.Border, obj2.Border)) return false;
			if (!FillEquals(obj1.Fill, obj2.Fill)) return false;
			if (!FormatStringEquals(obj1.NetFormatString,obj2.NetFormatString)) return false;
			if (!NumberFormatEquals(obj1.NumberFormat,obj2.NumberFormat)) return false;
			if (!Equals(obj1.IsDateTimeFormatString, obj2.IsDateTimeFormatString)) return false;
			return true;
		}
		private static bool NumberFormatEquals(XlNumberFormat nf1, XlNumberFormat nf2) {
			if (nf1 != null && nf2 == null) return false;
			if (nf1 == null && nf2 != null) return false;
			if (nf1 != null && nf2 != null) {
				if (nf1.FormatCode != nf2.FormatCode) return false;
				if (nf1.FormatId != nf2.FormatId) return false;
				if (nf1.IsDateTime != nf2.IsDateTime) return false;
			}
			return true;
		}
		private static bool FormatStringEquals(string s1, string s2) {
			if(!string.IsNullOrEmpty(s1) && string.IsNullOrEmpty(s2)) return false;
			if(string.IsNullOrEmpty(s1) && !string.IsNullOrEmpty(s2)) return false;
			if(!string.IsNullOrEmpty(s1) && !string.IsNullOrEmpty(s2)) {
				if(!string.Equals(s1, s2)) return false;
			}
			return true;
		}
		private static bool FillEquals(XlFill fill1, XlFill fill2) {
			if (fill1 != null && fill2 == null) return false;
			if (fill1 == null && fill2 != null) return false;
			if (fill1 != null && fill2 != null) {
				if (fill1.BackColor != fill2.BackColor) return false;
				if (fill1.ForeColor != fill2.ForeColor) return false;
				if (fill1.PatternType != fill2.PatternType) return false;
			}
			return true;
		}
		private static bool BorderEquals(XlBorder border1, XlBorder border2) {
			if (border1 != null && border2 == null) return false;
			if (border1 == null && border2 != null) return false;
			if(border1 != null && border2 != null) {
				if (border1.BottomColor != border2.BottomColor) return false;
				if (border1.BottomLineStyle != border2.BottomLineStyle) return false;
				if (border1.DiagonalColor != border2.DiagonalColor) return false;
				if (border1.DiagonalDown != border2.DiagonalDown) return false;
				if (border1.DiagonalDownLineStyle != border2.DiagonalDownLineStyle) return false;
				if (border1.DiagonalLineStyle != border2.DiagonalLineStyle) return false;
				if (border1.DiagonalUp != border2.DiagonalUp) return false;
				if (border1.DiagonalUpLineStyle != border2.DiagonalUpLineStyle) return false;
				if (border1.HorizontalColor != border2.HorizontalColor) return false;
				if (border1.HorizontalLineStyle != border2.HorizontalLineStyle) return false;
				if (border1.LeftColor != border2.LeftColor) return false;
				if (border1.LeftLineStyle != border2.LeftLineStyle) return false;
				if (border1.Outline != border2.Outline) return false;
				if (border1.RightColor != border2.RightColor) return false;
				if (border1.RightLineStyle != border2.RightLineStyle) return false;
				if (border1.TopColor != border2.TopColor) return false;
				if (border1.TopLineStyle != border2.TopLineStyle) return false;
				if (border1.VerticalColor != border2.VerticalColor) return false;
				if (border1.VerticalLineStyle != border2.VerticalLineStyle) return false;
			}
			return true;
		}
		private static bool AlignmentEquals(XlCellAlignment al1, XlCellAlignment al2) {
			if (al1 != null && al2 == null) return false;
			if (al1 == null && al2 != null) return false;
			if (al1 != null && al2 != null) {
				if (al1.VerticalAlignment != al2.VerticalAlignment) return false;
				if (al1.HorizontalAlignment != al2.HorizontalAlignment) return false;
			}
			return true;
		}
		XRControlStyle GetCustomStyle(string styleName, XlCellFormatting target,  bool useDefaultSettings, bool ignoreRightPadding) {
			var style = new XRControlStyle();
			style = Converter.ConvertToStyle(target, styleName);
			if(useDefaultSettings)
				SetDefaultStyleSettings(style, ignoreRightPadding);
			return style;
		}
		XRControlStyle GetDefaultStyle(string styleName, Color color, bool useDefaultSettings, bool ignoreRightPadding) {
			var style = new XRControlStyle();
			style.Name = styleName;
			style.BackColor = color;
			if(useDefaultSettings)
				SetDefaultStyleSettings(style, ignoreRightPadding);
			return style;
		}
		void SetDefaultStyleSettings(XRControlStyle style, bool ignoreRightPadding) {
			style.Padding = Converter.CalcPaddingInfo(ReportUnit.HundredthsOfAnInch, ignoreRightPadding);
			style.TextAlignment = TextAlignment.MiddleLeft;
		}
		XRControlStyle ConstructStyle(string styleName,bool option, XlCellFormatting target, Color defaultBackColor) {
			if(IsDefault(target)) {
				return GetDefaultStyle(styleName, defaultBackColor, true, false);
			}
			return option ? GetCustomStyle(styleName, target, true, false) : GetDefaultStyle(styleName, defaultBackColor, true, false);
		}
		private static bool IsDefault(XlCellFormatting target) {
			return Equals(target, GetDefaultFormat()) || Equals(target, new XlCellFormatting()) || target == null;
		}
		public IEnumerable<XRControlStyle> GetStyles() {
			var styles = new List<XRControlStyle>();
			styles.Add(GetHeaderStyle("ReportHeaderBandStyle"));
			styles.Add(GetGroupHeaderStyle("ReportGroupHeaderBandStyle"));
			styles.Add(GetDetailStyle("ReportDetailBandStyle"));
			styles.Add(GetGroupFooterStyle("ReportGroupFooterBandStyle"));
			styles.Add(GetFooterStyle("ReportFooterBandStyle"));
			styles.Add(GetOddStyle("ReportOddStyle"));
			styles.Add(GetEvenStyle("ReportEvenStyle"));
			return styles;
		}
		XRControlStyle GetDetailStyle(string styleName) {
			return ConstructStyle(styleName,true, model.AppearanceRow, Color.Transparent);
		}
		XRControlStyle GetGroupHeaderStyle(string styleName) {
			if(IsDefault(model.AppearanceGroupRow)) {
				return GetDefaultStyle(styleName, Color.FromArgb(206, 206, 206), true, true);
			}
			return GetCustomStyle(styleName, model.AppearanceGroupRow, true, true);
		}
		XRControlStyle GetFooterStyle(string styleName) {
			return ConstructStyle(styleName, true, model.AppearanceFooter, Color.FromArgb(206, 206, 206));
		}
		XRControlStyle GetGroupFooterStyle(string styleName) {
			return ConstructStyle(styleName, true, model.AppearanceGroupFooter, Color.FromArgb(206, 206, 206));
		}
		XRControlStyle GetHeaderStyle(string styleName) {
			return ConstructStyle(styleName, true, model.AppearanceHeader, Color.FromArgb(206, 206, 206));
		}
		XRControlStyle GetOddStyle(string styleName) {
			return ConstructStyle(styleName, model.EnableOddAppearance, model.AppearanceOddRow, Color.Transparent);
		}
		XRControlStyle GetEvenStyle(string styleName) {
			return ConstructStyle(styleName, model.EnableEvenAppearance, model.AppearanceEvenRow, Color.Transparent);
		}
	}
}
