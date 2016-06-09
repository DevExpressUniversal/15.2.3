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

using DevExpress.XtraExport.Helpers;
using DevExpress.Export.Xl;
using DevExpress.Utils;
using System;
using System.Drawing;
using DevExpress.Export;
using DevExpress.XtraPrinting;
using DevExpress.Compatibility.System.Drawing;
namespace DevExpress.Printing.ExportHelpers {
	static class FormattingExtensions{
		public static void GetActual(this XlCellFormatting format,FormatSettings settings){
			if(settings == null) return;
			FormatExport.GetActualFormatString(format, settings.FormatString, settings.ActualDataType);
		}
		public static XlCellFormatting ConvertWith(this XlFormattingObject from, IDataAwareExportOptions options) {
			if(from == null) return null;
			XlCellFormatting result = new XlCellFormatting();
			result.Alignment = from.Alignment;
			GetFill(from, result);
			GetBorder(options, result);
			GetFont(from, result);
			GetFormat(from, result);
			return result;
		}
		static void GetBorder(IDataAwareExportOptions options, XlCellFormatting result){
			if(options != null)
				FormatExport.SetBorder(result, options); 
		}
		static void GetFont(XlFormattingObject from, XlCellFormatting result){
			result.Font = new XlFont();
			if(from.Font != null){
				result.Font = ConvertFont(from.Font);
			}
		}
		static void GetFill(XlFormattingObject from, XlCellFormatting result){
			if(!Equals(from.BackColor, Color.Empty)){
				result.Fill = new XlFill();
				result.Fill.ForeColor = GetForeColor(from);
				result.Fill.PatternType = XlPatternType.Solid;
			}
		}
		static void GetFormat(XlFormattingObject from, XlCellFormatting cellFormat){
			cellFormat.NumberFormat = from.NumberFormat;
			if(from.FormatType != FormatType.None){
				cellFormat.NetFormatString = from.FormatString;
			}
			if(from.FormatType == FormatType.DateTime){
				var formatstring = cellFormat.NetFormatString;
				string numberformat = string.Empty;
				if(cellFormat.NumberFormat != null){
					numberformat = cellFormat.NumberFormat.ToString();
				}
				if(string.IsNullOrEmpty(formatstring) && string.IsNullOrEmpty(numberformat))
					cellFormat.NetFormatString = "d";
				cellFormat.IsDateTimeFormatString = true;
			}
		}
		static Color GetForeColor(XlFormattingObject fromFormatting){
			return fromFormatting.BackColor != Color.Empty
				? fromFormatting.BackColor
				: Color.White;
		}
		static XlCellAlignment SetAlignment(IColumn col, XlCellAlignment alingment) {
			if(col == null) return new XlCellAlignment();
			return alingment ?? col.Appearance.Alignment;
		}
		static XlFont ConvertFont(XlCellFont xlCellFont) {
			return new XlFont {
				Bold = xlCellFont.Bold,
				Color = xlCellFont.Color,
				Size = xlCellFont.Size,
				Name = GetCorrectFontName(xlCellFont),
				Italic = xlCellFont.Italic,
				FontFamily = 0,
				Underline = xlCellFont.Underline,
				StrikeThrough = xlCellFont.StrikeThrough,
				SchemeStyle = XlFontSchemeStyles.None
			};
		}
		static string GetCorrectFontName(XlCellFont xlCellFont){
			return string.IsNullOrEmpty(xlCellFont.Name) ? "Calibri" : xlCellFont.Name;
		}
	}
	static class FormatExport{
		public static void SetBoldFont(XlCellFormatting formatting){ 
			if(formatting == null) return;
			if(formatting.Font.Equals(new XlFont()))
				formatting.Font.Bold = true;
		}
		public static void SetCellFormatting(XlCellFormatting cellFormatting, IDataAwareExportOptions options, string formatString) {
			SetBoldFont(cellFormatting);
			SetBorder(cellFormatting, options);
		}
		public static void SetCellFormatting(XlCellFormatting cellFormatting, IDataAwareExportOptions options, string formatString, IColumn col, bool setbold, bool condition) {
			if(setbold) SetBoldFont(cellFormatting);
			SetBorder(cellFormatting, options);
			if(condition) SetFormat(col, cellFormatting, formatString);
			else cellFormatting.NetFormatString = formatString;
		}
		public static void SetCellFormatting(XlCellFormatting cellFormatting, IDataAwareExportOptions options, IColumn col) {
			SetBorder(cellFormatting, options);
			SetFormat(col, cellFormatting, col.FormatSettings.FormatString);
		}
		public static void SetCellFormatting(XlCellFormatting cellFormatting,IDataAwareExportOptions options,bool condition){
			if(condition)SetBoldFont(cellFormatting);
			SetBorder(cellFormatting, options);
		}
		public static void SetCellFormatting(XlCellFormatting cellFormatting,IColumn gridColumn,IDataAwareExportOptions options,bool condition){
			if(condition && gridColumn != null){
				string columnFormatString = gridColumn.FormatSettings.FormatString;
				cellFormatting.NetFormatString = AllowSetFormatString(cellFormatting, columnFormatString) ? columnFormatString : string.Empty;
				SetBoldFont(cellFormatting);
			}
			SetBorder(cellFormatting, options);
		}
		static bool AllowSetFormatString(XlCellFormatting cellFormatting, string columnFormatString){
			return !string.IsNullOrEmpty(columnFormatString) && cellFormatting.NumberFormat == null;
		}
		public static void SetHyperlinkFormat(XlCellFormatting cellFormatting) {
			if(cellFormatting != null){
				cellFormatting.Font.Color = Color.Blue;
				cellFormatting.Font.Underline = XlUnderlineType.Single;
			}
		}
		public static void SetBorder(XlCellFormatting format, IDataAwareExportOptions options) {
			if(format == null) return;
			XlBorder border = format.Border;
			if(options.AllowHorzLines == DefaultBoolean.True) SetHorzLines(ref border);
			if(options.AllowVertLines == DefaultBoolean.True) SetVertLines(ref border);
			format.Border = border;
		}
		public static void SetHorzLines(ref XlBorder border){
			if(border == null) border = new XlBorder();
			border.BottomLineStyle = XlBorderLineStyle.Thin;
			border.BottomColor = Color.Black;
			border.TopLineStyle = XlBorderLineStyle.Thin;
			border.TopColor = Color.Black;
		}
		public static void SetVertLines(ref XlBorder border){
			if(border == null) border = new XlBorder();
			border.LeftLineStyle = XlBorderLineStyle.Thin;
			border.LeftColor = Color.Black;
			border.RightLineStyle = XlBorderLineStyle.Thin;
			border.RightColor = Color.Black;
		}
		public static XlCellFormatting GetColumnAppearanceFormGridColumn(IColumn col,bool notRawDataMode) {
			XlCellFormatting result = new XlCellFormatting();
			if(notRawDataMode) result.CopyFrom(col.Appearance);
			return result;
		}
		public static void PrimaryFormatColumn(IColumn col, XlCellFormatting format) {
			SetFormat(col, format, col.FormatSettings.FormatString);
		}
		static void SetFormat(IColumn gridColumn, XlCellFormatting format, string formatString){
			bool dateTimeCondition = CheckDateTimeType(gridColumn);
			Type colType = dateTimeCondition ? Nullable.GetUnderlyingType(gridColumn.FormatSettings.ActualDataType) : gridColumn.FormatSettings.ActualDataType;
			GetActualFormatString(format, formatString, colType);
		}
		static bool CheckDateTimeType(IColumn gridColumn){
			return gridColumn.FormatSettings.ActualDataType == typeof(DateTime?) || gridColumn.FormatSettings.ActualDataType == typeof(TimeSpan?);
		}
		public static void GetActualFormatString(XlCellFormatting format, string formatString, Type colType) {
			if(format == null) return;
			if(format.NumberFormat == null) {
				format.NetFormatString = GetFormatStringByType(formatString, colType);
			}
			if(CheckTypeIgnoreNullable(colType, typeof(DateTime))) {
				if(string.IsNullOrEmpty(format.NetFormatString)) format.NetFormatString = "d";
				format.IsDateTimeFormatString = true;
			}
			if(CheckTypeIgnoreNullable(colType, typeof(TimeSpan))) {
				if(string.IsNullOrEmpty(format.NetFormatString)) format.NetFormatString = "t";
				format.IsDateTimeFormatString = true;
			}
		}
		static bool CheckTypeIgnoreNullable(Type typeToCheck, Type isTypeof) {
			if(typeToCheck == null || isTypeof == null) return false;
			isTypeof = Nullable.GetUnderlyingType(isTypeof) != null ? Nullable.GetUnderlyingType(isTypeof) : isTypeof;
			typeToCheck = Nullable.GetUnderlyingType(typeToCheck) != null ? Nullable.GetUnderlyingType(typeToCheck) : typeToCheck;
			return typeToCheck == isTypeof;
		}
		static string GetFormatStringByType(string formatStr,Type type){
			if(!string.IsNullOrEmpty(formatStr)) return formatStr;
			if(type == typeof(int)) return "d";
			if(type == typeof(double)) return "n2";
			return string.Empty;
		}
	}
}
namespace DevExpress.Export {
	public enum SheetAreaType{
		DataArea,
		Header,
		GroupHeader,
		GroupFooter,
		TotalFooter
	}
	#region XlCellFont
	public class XlCellFont {
		public Color Color { get; set; }
		public string Name { get; set; }
		public double Size { get; set; }
		public XlUnderlineType Underline { get; set; }
		public bool Bold { get; set; }
		public bool StrikeThrough { get; set; }
		public bool Italic { get; set; }
	}
	#endregion
	#region XlFormattingObject
	public class XlFormattingObject{
		public XlCellFont Font { get; set; }
		public XlCellAlignment Alignment { get; set; }
		public Color BackColor { get; set; }
		public string FormatString { get; set; }
		public FormatType FormatType { get; set; }
		public XlNumberFormat NumberFormat { get; set; }
		public void CopyFrom(XlCellFormatting value,FormatType columnFormatType){
			if(value != null){
				Font = CreateXlCellFont(value);
				FormatString = value.NetFormatString;
				FormatType = columnFormatType;
				BackColor = GetBackColor(value);
			}
		}
		static Color GetBackColor(XlCellFormatting value){
			return value.Fill!=null ? (Color) value.Fill.ForeColor : Color.Empty;
		}
		private static XlCellFont CreateXlCellFont(XlCellFormatting value){
			return new XlCellFont{
				Name = value.Font.Name,
				Size = value.Font.Size,
				Bold = value.Font.Bold,
				Color = value.Font.Color,
				Italic = value.Font.Italic,
				Underline = value.Font.Underline,
				StrikeThrough = value.Font.StrikeThrough
			};
		}
	}
	#endregion XlFormattingObject
}
