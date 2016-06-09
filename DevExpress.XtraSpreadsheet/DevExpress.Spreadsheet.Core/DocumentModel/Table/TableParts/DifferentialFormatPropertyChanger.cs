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
using DevExpress.XtraSpreadsheet.Model.History;
using System.Collections.Generic;
using DevExpress.XtraSpreadsheet.Services;
using DevExpress.XtraSpreadsheet.Services.Implementation;
using DevExpress.Utils;
using DevExpress.Office.Utils;
using DevExpress.XtraSpreadsheet.Utils;
using DevExpress.XtraSpreadsheet.Localization;
using DevExpress.Office;
using System.Runtime.InteropServices;
using DevExpress.Office.History;
using System.Drawing;
using DevExpress.Export.Xl;
using DevExpress.Compatibility.System.Drawing;
#if SL
using System.Windows.Media;
#endif
namespace DevExpress.XtraSpreadsheet.Model {
	public partial class Table : IDifferentialFormatPropertyChanger {
		#region IDifferentialFormatPropertyChanger Members
		#region FormatStringPropertyChanger
		string IDifferentialFormatPropertyChanger.GetFormatString(int elementIndex) {
			return GetDifferentialFormat(elementIndex).FormatString;
		}
		void IDifferentialFormatPropertyChanger.SetFormatString(int elementIndex, string value) {
			DifferentialFormat info = GetDifferentialFormat(elementIndex);
			if (info.FormatString == value && info.MultiOptionsInfo.ApplyNumberFormat)
				return;
			SetPropertyValue(GetDifferentialFormatIndexAccessor(elementIndex), SetFormatString, value);
		}
		DocumentModelChangeActions SetFormatString(FormatBase info, string value) {
			((DifferentialFormat)info).FormatString = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region FontInfoPropertyChanger
		#region FontName
		string IDifferentialFormatPropertyChanger.GetFontName(int elementIndex) {
			return GetDifferentialFormat(elementIndex).Font.Name;
		}
		void IDifferentialFormatPropertyChanger.SetFontName(int elementIndex, string value) {
			DifferentialFormat info = GetDifferentialFormat(elementIndex);
			if (info.Font.Name == value && info.MultiOptionsInfo.ApplyFontName)
				return;
			SetPropertyValue(GetDifferentialFormatIndexAccessor(elementIndex), SetFontNameCore, value);
		}
		DocumentModelChangeActions SetFontNameCore(FormatBase info, string value) {
			info.Font.Name = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region FontSize
		double IDifferentialFormatPropertyChanger.GetFontSize(int elementIndex) {
			return GetDifferentialFormat(elementIndex).Font.Size;
		}
		void IDifferentialFormatPropertyChanger.SetFontSize(int elementIndex, double value) {
			DifferentialFormat info = GetDifferentialFormat(elementIndex);
			if (info.Font.Size == value && info.MultiOptionsInfo.ApplyFontSize)
				return;
			SetPropertyValue(GetDifferentialFormatIndexAccessor(elementIndex), SetFontSizeCore, value);
		}
		DocumentModelChangeActions SetFontSizeCore(FormatBase info, double value) {
			info.Font.Size = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region FontBold
		bool IDifferentialFormatPropertyChanger.GetFontBold(int elementIndex) {
			return GetDifferentialFormat(elementIndex).Font.Bold;
		}
		void IDifferentialFormatPropertyChanger.SetFontBold(int elementIndex, bool value) {
			DifferentialFormat info = GetDifferentialFormat(elementIndex);
			if (info.Font.Bold == value && info.MultiOptionsInfo.ApplyFontBold)
				return;
			SetPropertyValue(GetDifferentialFormatIndexAccessor(elementIndex), SetFontBoldCore, value);
		}
		DocumentModelChangeActions SetFontBoldCore(FormatBase info, bool value) {
			info.Font.Bold = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region FontItalic
		bool IDifferentialFormatPropertyChanger.GetFontItalic(int elementIndex) {
			return GetDifferentialFormat(elementIndex).Font.Italic;
		}
		void IDifferentialFormatPropertyChanger.SetFontItalic(int elementIndex, bool value) {
			DifferentialFormat info = GetDifferentialFormat(elementIndex);
			if (info.Font.Italic == value && info.MultiOptionsInfo.ApplyFontItalic)
				return;
			SetPropertyValue(GetDifferentialFormatIndexAccessor(elementIndex), SetFontItalicCore, value);
		}
		DocumentModelChangeActions SetFontItalicCore(FormatBase info, bool value) {
			info.Font.Italic = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region FontStrikeThrough
		bool IDifferentialFormatPropertyChanger.GetFontStrikeThrough(int elementIndex) {
			return GetDifferentialFormat(elementIndex).Font.StrikeThrough;
		}
		void IDifferentialFormatPropertyChanger.SetFontStrikeThrough(int elementIndex, bool value) {
			DifferentialFormat info = GetDifferentialFormat(elementIndex);
			if (info.Font.StrikeThrough == value && info.MultiOptionsInfo.ApplyFontStrikeThrough)
				return;
			SetPropertyValue(GetDifferentialFormatIndexAccessor(elementIndex), SetFontStrikeThroughCore, value);
		}
		DocumentModelChangeActions SetFontStrikeThroughCore(FormatBase info, bool value) {
			info.Font.StrikeThrough = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region FontCondense
		bool IDifferentialFormatPropertyChanger.GetFontCondense(int elementIndex) {
			return GetDifferentialFormat(elementIndex).Font.Condense;
		}
		void IDifferentialFormatPropertyChanger.SetFontCondense(int elementIndex, bool value) {
			DifferentialFormat info = GetDifferentialFormat(elementIndex);
			if (info.Font.Condense == value && info.MultiOptionsInfo.ApplyFontCondense)
				return;
			SetPropertyValue(GetDifferentialFormatIndexAccessor(elementIndex), SetFontCondenseCore, value);
		}
		DocumentModelChangeActions SetFontCondenseCore(FormatBase info, bool value) {
			info.Font.Condense = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region FontExtend
		bool IDifferentialFormatPropertyChanger.GetFontExtend(int elementIndex) {
			return GetDifferentialFormat(elementIndex).Font.Extend;
		}
		void IDifferentialFormatPropertyChanger.SetFontExtend(int elementIndex, bool value) {
			DifferentialFormat info = GetDifferentialFormat(elementIndex);
			if (info.Font.Extend == value && info.MultiOptionsInfo.ApplyFontExtend)
				return;
			SetPropertyValue(GetDifferentialFormatIndexAccessor(elementIndex), SetFontExtendCore, value);
		}
		DocumentModelChangeActions SetFontExtendCore(FormatBase info, bool value) {
			info.Font.Extend = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region FontOutline
		bool IDifferentialFormatPropertyChanger.GetFontOutline(int elementIndex) {
			return GetDifferentialFormat(elementIndex).Font.Outline;
		}
		void IDifferentialFormatPropertyChanger.SetFontOutline(int elementIndex, bool value) {
			DifferentialFormat info = GetDifferentialFormat(elementIndex);
			if (info.Font.Outline == value && info.MultiOptionsInfo.ApplyFontOutline)
				return;
			SetPropertyValue(GetDifferentialFormatIndexAccessor(elementIndex), SetFontOutlineCore, value);
		}
		DocumentModelChangeActions SetFontOutlineCore(FormatBase info, bool value) {
			info.Font.Outline = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region FontShadow
		bool IDifferentialFormatPropertyChanger.GetFontShadow(int elementIndex) {
			return GetDifferentialFormat(elementIndex).Font.Shadow;
		}
		void IDifferentialFormatPropertyChanger.SetFontShadow(int elementIndex, bool value) {
			DifferentialFormat info = GetDifferentialFormat(elementIndex);
			if (info.Font.Shadow == value && info.MultiOptionsInfo.ApplyFontShadow)
				return;
			SetPropertyValue(GetDifferentialFormatIndexAccessor(elementIndex), SetFontShadowCore, value);
		}
		DocumentModelChangeActions SetFontShadowCore(FormatBase info, bool value) {
			info.Font.Shadow = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region FontCharset
		int IDifferentialFormatPropertyChanger.GetFontCharset(int elementIndex) {
			return GetDifferentialFormat(elementIndex).Font.Charset;
		}
		void IDifferentialFormatPropertyChanger.SetFontCharset(int elementIndex, int value) {
			DifferentialFormat info = GetDifferentialFormat(elementIndex);
			if (info.Font.Charset == value && info.MultiOptionsInfo.ApplyFontCharset)
				return;
			SetPropertyValue(GetDifferentialFormatIndexAccessor(elementIndex), SetFontCharsetCore, value);
		}
		DocumentModelChangeActions SetFontCharsetCore(FormatBase info, int value) {
			info.Font.Charset = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region FontFontFamily
		int IDifferentialFormatPropertyChanger.GetFontFontFamily(int elementIndex) {
			return GetDifferentialFormat(elementIndex).Font.FontFamily;
		}
		void IDifferentialFormatPropertyChanger.SetFontFontFamily(int elementIndex, int value) {
			DifferentialFormat info = GetDifferentialFormat(elementIndex);
			if (info.Font.FontFamily == value && info.MultiOptionsInfo.ApplyFontFamily)
				return;
			SetPropertyValue(GetDifferentialFormatIndexAccessor(elementIndex), SetFontFontFamilyCore, value);
		}
		DocumentModelChangeActions SetFontFontFamilyCore(FormatBase info, int value) {
			info.Font.FontFamily = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region FontUnderline
		XlUnderlineType IDifferentialFormatPropertyChanger.GetFontUnderline(int elementIndex) {
			return GetDifferentialFormat(elementIndex).Font.Underline;
		}
		void IDifferentialFormatPropertyChanger.SetFontUnderline(int elementIndex, XlUnderlineType value) {
			DifferentialFormat info = GetDifferentialFormat(elementIndex);
			if (info.Font.Underline == value && info.MultiOptionsInfo.ApplyFontUnderline)
				return;
			SetPropertyValue(GetDifferentialFormatIndexAccessor(elementIndex), SetFontUnderlineCore, value);
		}
		DocumentModelChangeActions SetFontUnderlineCore(FormatBase info, XlUnderlineType value) {
			info.Font.Underline = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region FontColor
		Color IDifferentialFormatPropertyChanger.GetFontColor(int elementIndex) {
			return GetDifferentialFormat(elementIndex).Font.Color;
		}
		void IDifferentialFormatPropertyChanger.SetFontColor(int elementIndex, Color value) {
			DifferentialFormat info = GetDifferentialFormat(elementIndex);
			if (info.Font.Color == value && info.MultiOptionsInfo.ApplyFontColor)
				return;
			SetPropertyValue(GetDifferentialFormatIndexAccessor(elementIndex), SetFontColorCore, value);
		}
		DocumentModelChangeActions SetFontColorCore(FormatBase info, Color value) {
			info.Font.Color = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region FontScript
		XlScriptType IDifferentialFormatPropertyChanger.GetFontScript(int elementIndex) {
			return GetDifferentialFormat(elementIndex).Font.Script;
		}
		void IDifferentialFormatPropertyChanger.SetFontScript(int elementIndex, XlScriptType value) {
			DifferentialFormat info = GetDifferentialFormat(elementIndex);
			if (info.Font.Script == value && info.MultiOptionsInfo.ApplyFontScript)
				return;
			SetPropertyValue(GetDifferentialFormatIndexAccessor(elementIndex), SetFontScriptCore, value);
		}
		DocumentModelChangeActions SetFontScriptCore(FormatBase info, XlScriptType value) {
			info.Font.Script = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region FontSchemeStyle
		XlFontSchemeStyles IDifferentialFormatPropertyChanger.GetFontSchemeStyle(int elementIndex) {
			return GetDifferentialFormat(elementIndex).Font.SchemeStyle;
		}
		void IDifferentialFormatPropertyChanger.SetFontSchemeStyle(int elementIndex, XlFontSchemeStyles value) {
			DifferentialFormat info = GetDifferentialFormat(elementIndex);
			if (info.Font.SchemeStyle == value && info.MultiOptionsInfo.ApplyFontSchemeStyle)
				return;
			SetPropertyValue(GetDifferentialFormatIndexAccessor(elementIndex), SetFontScriptCore, value);
		}
		DocumentModelChangeActions SetFontScriptCore(FormatBase info, XlFontSchemeStyles value) {
			info.Font.SchemeStyle = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#endregion
		#region FillInfoPropertyChanger
		#region FillPatternType
		XlPatternType IDifferentialFormatPropertyChanger.GetFillPatternType(int elementIndex) {
			return GetDifferentialFormat(elementIndex).Fill.PatternType;
		}
		void IDifferentialFormatPropertyChanger.SetFillPatternType(int elementIndex, XlPatternType value) {
			DifferentialFormat info = GetDifferentialFormat(elementIndex);
			if (info.Fill.PatternType == value && info.MultiOptionsInfo.ApplyFillPatternType)
				return;
			SetPropertyValue(GetDifferentialFormatIndexAccessor(elementIndex), SetFillPatternTypeCore, value);
		}
		DocumentModelChangeActions SetFillPatternTypeCore(FormatBase info, XlPatternType value) {
			info.Fill.PatternType = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region FillForeColor
		Color IDifferentialFormatPropertyChanger.GetFillForeColor(int elementIndex) {
			return GetDifferentialFormat(elementIndex).Fill.ForeColor;
		}
		void IDifferentialFormatPropertyChanger.SetFillForeColor(int elementIndex, Color value) {
			DifferentialFormat info = GetDifferentialFormat(elementIndex);
			if (info.Fill.ForeColor == value && info.MultiOptionsInfo.ApplyFillForeColor)
				return;
			SetPropertyValue(GetDifferentialFormatIndexAccessor(elementIndex), SetFillForeColorCore, value);
		}
		DocumentModelChangeActions SetFillForeColorCore(FormatBase info, Color value) {
			info.Fill.ForeColor = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region FillBackColor
		Color IDifferentialFormatPropertyChanger.GetFillBackColor(int elementIndex) {
			return GetDifferentialFormat(elementIndex).Fill.BackColor;
		}
		void IDifferentialFormatPropertyChanger.SetFillBackColor(int elementIndex, Color value) {
			DifferentialFormat info = GetDifferentialFormat(elementIndex);
			if (info.Fill.BackColor == value && info.MultiOptionsInfo.ApplyFillBackColor)
				return;
			SetPropertyValue(GetDifferentialFormatIndexAccessor(elementIndex), SetFillBackColorCore, value);
		}
		DocumentModelChangeActions SetFillBackColorCore(FormatBase info, Color value) {
			info.Fill.BackColor = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region GradientFill
		#region GradientStops
		IGradientStopCollection IDifferentialFormatPropertyChanger.GetGradientStops(int elementIndex) {
			return GetDifferentialFormat(elementIndex).Fill.GradientFill.GradientStops;
		}
		#endregion
		#region Type
		ModelGradientFillType IDifferentialFormatPropertyChanger.GetGradientFillType(int elementIndex) {
			return GetDifferentialFormat(elementIndex).Fill.GradientFill.Type;
		}
		void IDifferentialFormatPropertyChanger.SetGradientFillType(int elementIndex, ModelGradientFillType value) {
			DifferentialFormat info = GetDifferentialFormat(elementIndex);
			if (info.Fill.GradientFill.Type == value)
				return;
			SetPropertyValue(GetDifferentialFormatIndexAccessor(elementIndex), SetGradientTypeCore, value);
		}
		DocumentModelChangeActions SetGradientTypeCore(FormatBase info, ModelGradientFillType value) {
			info.Fill.GradientFill.Type = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region Degree
		double IDifferentialFormatPropertyChanger.GetDegree(int elementIndex) {
			return GetDifferentialFormat(elementIndex).Fill.GradientFill.Degree;
		}
		void IDifferentialFormatPropertyChanger.SetDegree(int elementIndex, double value) {
			DifferentialFormat info = GetDifferentialFormat(elementIndex);
			if (info.Fill.GradientFill.Degree == value)
				return;
			SetPropertyValue(GetDifferentialFormatIndexAccessor(elementIndex), SetDegreeCore, value);
		}
		DocumentModelChangeActions SetDegreeCore(FormatBase info, double value) {
			info.Fill.GradientFill.Degree = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region ConvergenceLeft
		float IDifferentialFormatPropertyChanger.GetConvergenceLeft(int elementIndex) {
			return GetDifferentialFormat(elementIndex).Fill.GradientFill.Convergence.Left;
		}
		void IDifferentialFormatPropertyChanger.SetConvergenceLeft(int elementIndex, float value) {
			DifferentialFormat info = GetDifferentialFormat(elementIndex);
			if (info.Fill.GradientFill.Convergence.Left == value)
				return;
			SetPropertyValue(GetDifferentialFormatIndexAccessor(elementIndex), SetConvergenceLeftCore, value);
		}
		DocumentModelChangeActions SetConvergenceLeftCore(FormatBase info, float value) {
			info.Fill.GradientFill.Convergence.Left = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region ConvergenceRight
		float IDifferentialFormatPropertyChanger.GetConvergenceRight(int elementIndex) {
			return GetDifferentialFormat(elementIndex).Fill.GradientFill.Convergence.Right;
		}
		void IDifferentialFormatPropertyChanger.SetConvergenceRight(int elementIndex, float value) {
			DifferentialFormat info = GetDifferentialFormat(elementIndex);
			if (info.Fill.GradientFill.Convergence.Right == value)
				return;
			SetPropertyValue(GetDifferentialFormatIndexAccessor(elementIndex), SetConvergenceRightCore, value);
		}
		DocumentModelChangeActions SetConvergenceRightCore(FormatBase info, float value) {
			info.Fill.GradientFill.Convergence.Right = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region ConvergenceTop
		float IDifferentialFormatPropertyChanger.GetConvergenceTop(int elementIndex) {
			return GetDifferentialFormat(elementIndex).Fill.GradientFill.Convergence.Top;
		}
		void IDifferentialFormatPropertyChanger.SetConvergenceTop(int elementIndex, float value) {
			DifferentialFormat info = GetDifferentialFormat(elementIndex);
			if (info.Fill.GradientFill.Convergence.Top == value)
				return;
			SetPropertyValue(GetDifferentialFormatIndexAccessor(elementIndex), SetConvergenceTopCore, value);
		}
		DocumentModelChangeActions SetConvergenceTopCore(FormatBase info, float value) {
			info.Fill.GradientFill.Convergence.Top = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region ConvergenceBottom
		float IDifferentialFormatPropertyChanger.GetConvergenceBottom(int elementIndex) {
			return GetDifferentialFormat(elementIndex).Fill.GradientFill.Convergence.Bottom;
		}
		void IDifferentialFormatPropertyChanger.SetConvergenceBottom(int elementIndex, float value) {
			DifferentialFormat info = GetDifferentialFormat(elementIndex);
			if (info.Fill.GradientFill.Convergence.Bottom == value)
				return;
			SetPropertyValue(GetDifferentialFormatIndexAccessor(elementIndex), SetConvergenceBottomCore, value);
		}
		DocumentModelChangeActions SetConvergenceBottomCore(FormatBase info, float value) {
			info.Fill.GradientFill.Convergence.Bottom = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#endregion
		#region FillType
		ModelFillType IDifferentialFormatPropertyChanger.GetFillType(int elementIndex) {
			return GetDifferentialFormat(elementIndex).Fill.FillType;
		}
		void IDifferentialFormatPropertyChanger.SetFillType(int elementIndex, ModelFillType value) {
			DifferentialFormat info = GetDifferentialFormat(elementIndex);
			if (info.Fill.FillType == value)
				return;
			SetPropertyValue(GetDifferentialFormatIndexAccessor(elementIndex), SetFillTypeCore, value);
		}
		DocumentModelChangeActions SetFillTypeCore(FormatBase info, ModelFillType value) {
			info.Fill.FillType = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#endregion
		#region BorderInfoPropertyChanger
		#region BorderLeftLineStyle
		XlBorderLineStyle IDifferentialFormatPropertyChanger.GetBorderLeftLineStyle(int elementIndex) {
			return GetDifferentialFormat(elementIndex).Border.LeftLineStyle;
		}
		void IDifferentialFormatPropertyChanger.SetBorderLeftLineStyle(int elementIndex, XlBorderLineStyle value) {
			DifferentialFormat info = GetDifferentialFormat(elementIndex);
			if (info.Border.LeftLineStyle == value && info.BorderOptionsInfo.ApplyLeftLineStyle)
				return;
			SetPropertyValue(GetDifferentialFormatIndexAccessor(elementIndex), SetBorderLeftLineStyleCore, value);
		}
		DocumentModelChangeActions SetBorderLeftLineStyleCore(FormatBase info, XlBorderLineStyle value) {
			info.Border.LeftLineStyle = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region BorderRightLineStyle
		XlBorderLineStyle IDifferentialFormatPropertyChanger.GetBorderRightLineStyle(int elementIndex) {
			return GetDifferentialFormat(elementIndex).Border.RightLineStyle;
		}
		void IDifferentialFormatPropertyChanger.SetBorderRightLineStyle(int elementIndex, XlBorderLineStyle value) {
			DifferentialFormat info = GetDifferentialFormat(elementIndex);
			if (info.Border.RightLineStyle == value && info.BorderOptionsInfo.ApplyRightLineStyle)
				return;
			SetPropertyValue(GetDifferentialFormatIndexAccessor(elementIndex), SetBorderRightLineStyleCore, value);
		}
		DocumentModelChangeActions SetBorderRightLineStyleCore(FormatBase info, XlBorderLineStyle value) {
			info.Border.RightLineStyle = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region BorderTopLineStyle
		XlBorderLineStyle IDifferentialFormatPropertyChanger.GetBorderTopLineStyle(int elementIndex) {
			return GetDifferentialFormat(elementIndex).Border.TopLineStyle;
		}
		void IDifferentialFormatPropertyChanger.SetBorderTopLineStyle(int elementIndex, XlBorderLineStyle value) {
			DifferentialFormat info = GetDifferentialFormat(elementIndex);
			if (info.Border.TopLineStyle == value && info.BorderOptionsInfo.ApplyTopLineStyle)
				return;
			SetPropertyValue(GetDifferentialFormatIndexAccessor(elementIndex), SetBorderTopLineStyleCore, value);
		}
		DocumentModelChangeActions SetBorderTopLineStyleCore(FormatBase info, XlBorderLineStyle value) {
			info.Border.TopLineStyle = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region BorderBottomLineStyle
		XlBorderLineStyle IDifferentialFormatPropertyChanger.GetBorderBottomLineStyle(int elementIndex) {
			return GetDifferentialFormat(elementIndex).Border.BottomLineStyle;
		}
		void IDifferentialFormatPropertyChanger.SetBorderBottomLineStyle(int elementIndex, XlBorderLineStyle value) {
			DifferentialFormat info = GetDifferentialFormat(elementIndex);
			if (info.Border.BottomLineStyle == value && info.BorderOptionsInfo.ApplyBottomLineStyle)
				return;
			SetPropertyValue(GetDifferentialFormatIndexAccessor(elementIndex), SetBorderBottomLineStyleCore, value);
		}
		DocumentModelChangeActions SetBorderBottomLineStyleCore(FormatBase info, XlBorderLineStyle value) {
			info.Border.BottomLineStyle = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region BorderHorizontalLineStyle
		XlBorderLineStyle IDifferentialFormatPropertyChanger.GetBorderHorizontalLineStyle(int elementIndex) {
			return GetDifferentialFormat(elementIndex).Border.HorizontalLineStyle;
		}
		void IDifferentialFormatPropertyChanger.SetBorderHorizontalLineStyle(int elementIndex, XlBorderLineStyle value) {
			DifferentialFormat info = GetDifferentialFormat(elementIndex);
			if (info.Border.HorizontalLineStyle == value && info.BorderOptionsInfo.ApplyHorizontalLineStyle)
				return;
			SetPropertyValue(GetDifferentialFormatIndexAccessor(elementIndex), SetBorderHorizontalLineStyleCore, value);
		}
		DocumentModelChangeActions SetBorderHorizontalLineStyleCore(FormatBase info, XlBorderLineStyle value) {
			info.Border.HorizontalLineStyle = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region BorderVerticalLineStyle
		XlBorderLineStyle IDifferentialFormatPropertyChanger.GetBorderVerticalLineStyle(int elementIndex) {
			return GetDifferentialFormat(elementIndex).Border.VerticalLineStyle;
		}
		void IDifferentialFormatPropertyChanger.SetBorderVerticalLineStyle(int elementIndex, XlBorderLineStyle value) {
			DifferentialFormat info = GetDifferentialFormat(elementIndex);
			if (info.Border.VerticalLineStyle == value && info.BorderOptionsInfo.ApplyVerticalLineStyle)
				return;
			SetPropertyValue(GetDifferentialFormatIndexAccessor(elementIndex), SetBorderVerticalLineStyleCore, value);
		}
		DocumentModelChangeActions SetBorderVerticalLineStyleCore(FormatBase info, XlBorderLineStyle value) {
			info.Border.VerticalLineStyle = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region BorderDiagonalUpLineStyle
		XlBorderLineStyle IDifferentialFormatPropertyChanger.GetBorderDiagonalUpLineStyle(int elementIndex) {
			return GetDifferentialFormat(elementIndex).Border.DiagonalUpLineStyle;
		}
		void IDifferentialFormatPropertyChanger.SetBorderDiagonalUpLineStyle(int elementIndex, XlBorderLineStyle value) {
			DifferentialFormat info = GetDifferentialFormat(elementIndex);
			if (info.Border.DiagonalUpLineStyle == value && info.BorderOptionsInfo.ApplyDiagonalLineStyle)
				return;
			SetPropertyValue(GetDifferentialFormatIndexAccessor(elementIndex), SetBorderDiagonalUpLineStyleCore, value);
		}
		DocumentModelChangeActions SetBorderDiagonalUpLineStyleCore(FormatBase info, XlBorderLineStyle value) {
			info.Border.DiagonalUpLineStyle = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region BorderDiagonalDownLineStyle
		XlBorderLineStyle IDifferentialFormatPropertyChanger.GetBorderDiagonalDownLineStyle(int elementIndex) {
			return GetDifferentialFormat(elementIndex).Border.DiagonalDownLineStyle;
		}
		void IDifferentialFormatPropertyChanger.SetBorderDiagonalDownLineStyle(int elementIndex, XlBorderLineStyle value) {
			DifferentialFormat info = GetDifferentialFormat(elementIndex);
			if (info.Border.DiagonalDownLineStyle == value && info.BorderOptionsInfo.ApplyDiagonalLineStyle)
				return;
			SetPropertyValue(GetDifferentialFormatIndexAccessor(elementIndex), SetBorderDiagonalDownLineStyleCore, value);
		}
		DocumentModelChangeActions SetBorderDiagonalDownLineStyleCore(FormatBase info, XlBorderLineStyle value) {
			info.Border.DiagonalDownLineStyle = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region BorderLeftColor
		Color IDifferentialFormatPropertyChanger.GetBorderLeftColor(int elementIndex) {
			return GetDifferentialFormat(elementIndex).Border.LeftColor;
		}
		void IDifferentialFormatPropertyChanger.SetBorderLeftColor(int elementIndex, Color value) {
			DifferentialFormat info = GetDifferentialFormat(elementIndex);
			if (info.Border.LeftColor == value && info.BorderOptionsInfo.ApplyLeftColor)
				return;
			SetPropertyValue(GetDifferentialFormatIndexAccessor(elementIndex), SetBorderLeftColorCore, value);
		}
		int IDifferentialFormatPropertyChanger.GetBorderLeftColorIndex(int elementIndex) {
			return GetDifferentialFormat(elementIndex).Border.LeftColorIndex;
		}
		void IDifferentialFormatPropertyChanger.SetBorderLeftColorIndex(int elementIndex, int value) {
			DifferentialFormat info = GetDifferentialFormat(elementIndex);
			if (info.Border.LeftColorIndex == value && info.BorderOptionsInfo.ApplyLeftColor)
				return;
			SetPropertyValue(GetDifferentialFormatIndexAccessor(elementIndex), SetBorderLeftColorIndexCore, value);
		}
		DocumentModelChangeActions SetBorderLeftColorCore(FormatBase info, Color value) {
			info.Border.LeftColor = value;
			return DocumentModelChangeActions.None; 
		}
		DocumentModelChangeActions SetBorderLeftColorIndexCore(FormatBase info, int value) {
			info.Border.LeftColorIndex = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region BorderRightColor
		Color IDifferentialFormatPropertyChanger.GetBorderRightColor(int elementIndex) {
			return GetDifferentialFormat(elementIndex).Border.RightColor;
		}
		void IDifferentialFormatPropertyChanger.SetBorderRightColor(int elementIndex, Color value) {
			DifferentialFormat info = GetDifferentialFormat(elementIndex);
			if (info.Border.RightColor == value && info.BorderOptionsInfo.ApplyRightColor)
				return;
			SetPropertyValue(GetDifferentialFormatIndexAccessor(elementIndex), SetBorderRightColorCore, value);
		}
		int IDifferentialFormatPropertyChanger.GetBorderRightColorIndex(int elementIndex) {
			return GetDifferentialFormat(elementIndex).Border.RightColorIndex;
		}
		void IDifferentialFormatPropertyChanger.SetBorderRightColorIndex(int elementIndex, int value) {
			DifferentialFormat info = GetDifferentialFormat(elementIndex);
			if (info.Border.RightColorIndex == value && info.BorderOptionsInfo.ApplyRightColor)
				return;
			SetPropertyValue(GetDifferentialFormatIndexAccessor(elementIndex), SetBorderRightColorIndexCore, value);
		}
		DocumentModelChangeActions SetBorderRightColorCore(FormatBase info, Color value) {
			info.Border.RightColor = value;
			return DocumentModelChangeActions.None; 
		}
		DocumentModelChangeActions SetBorderRightColorIndexCore(FormatBase info, int value) {
			info.Border.RightColorIndex = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region BorderTopColor
		Color IDifferentialFormatPropertyChanger.GetBorderTopColor(int elementIndex) {
			return GetDifferentialFormat(elementIndex).Border.TopColor;
		}
		void IDifferentialFormatPropertyChanger.SetBorderTopColor(int elementIndex, Color value) {
			DifferentialFormat info = GetDifferentialFormat(elementIndex);
			if (info.Border.TopColor == value && info.BorderOptionsInfo.ApplyTopColor)
				return;
			SetPropertyValue(GetDifferentialFormatIndexAccessor(elementIndex), SetBorderTopColorCore, value);
		}
		int IDifferentialFormatPropertyChanger.GetBorderTopColorIndex(int elementIndex) {
			return GetDifferentialFormat(elementIndex).Border.TopColorIndex;
		}
		void IDifferentialFormatPropertyChanger.SetBorderTopColorIndex(int elementIndex, int value) {
			DifferentialFormat info = GetDifferentialFormat(elementIndex);
			if (info.Border.TopColorIndex == value && info.BorderOptionsInfo.ApplyTopColor)
				return;
			SetPropertyValue(GetDifferentialFormatIndexAccessor(elementIndex), SetBorderTopColorIndexCore, value);
		}
		DocumentModelChangeActions SetBorderTopColorCore(FormatBase info, Color value) {
			info.Border.TopColor = value;
			return DocumentModelChangeActions.None; 
		}
		DocumentModelChangeActions SetBorderTopColorIndexCore(FormatBase info, int value) {
			info.Border.TopColorIndex = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region BorderBottomColor
		Color IDifferentialFormatPropertyChanger.GetBorderBottomColor(int elementIndex) {
			return GetDifferentialFormat(elementIndex).Border.BottomColor;
		}
		void IDifferentialFormatPropertyChanger.SetBorderBottomColor(int elementIndex, Color value) {
			DifferentialFormat info = GetDifferentialFormat(elementIndex);
			if (info.Border.BottomColor == value && info.BorderOptionsInfo.ApplyBottomColor)
				return;
			SetPropertyValue(GetDifferentialFormatIndexAccessor(elementIndex), SetBorderBottomColorCore, value);
		}
		int IDifferentialFormatPropertyChanger.GetBorderBottomColorIndex(int elementIndex) {
			return GetDifferentialFormat(elementIndex).Border.BottomColorIndex;
		}
		void IDifferentialFormatPropertyChanger.SetBorderBottomColorIndex(int elementIndex, int value) {
			DifferentialFormat info = GetDifferentialFormat(elementIndex);
			if (info.Border.BottomColorIndex == value && info.BorderOptionsInfo.ApplyBottomColor)
				return;
			SetPropertyValue(GetDifferentialFormatIndexAccessor(elementIndex), SetBorderBottomColorIndexCore, value);
		}
		DocumentModelChangeActions SetBorderBottomColorCore(FormatBase info, Color value) {
			info.Border.BottomColor = value;
			return DocumentModelChangeActions.None; 
		}
		DocumentModelChangeActions SetBorderBottomColorIndexCore(FormatBase info, int value) {
			info.Border.BottomColorIndex = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region BorderHorizontalColor
		Color IDifferentialFormatPropertyChanger.GetBorderHorizontalColor(int elementIndex) {
			return GetDifferentialFormat(elementIndex).Border.HorizontalColor;
		}
		void IDifferentialFormatPropertyChanger.SetBorderHorizontalColor(int elementIndex, Color value) {
			DifferentialFormat info = GetDifferentialFormat(elementIndex);
			if (info.Border.HorizontalColor == value && info.BorderOptionsInfo.ApplyHorizontalColor)
				return;
			SetPropertyValue(GetDifferentialFormatIndexAccessor(elementIndex), SetBorderHorizontalColorCore, value);
		}
		int IDifferentialFormatPropertyChanger.GetBorderHorizontalColorIndex(int elementIndex) {
			return GetDifferentialFormat(elementIndex).Border.HorizontalColorIndex;
		}
		void IDifferentialFormatPropertyChanger.SetBorderHorizontalColorIndex(int elementIndex, int value) {
			DifferentialFormat info = GetDifferentialFormat(elementIndex);
			if (info.Border.HorizontalColorIndex == value && info.BorderOptionsInfo.ApplyHorizontalColor)
				return;
			SetPropertyValue(GetDifferentialFormatIndexAccessor(elementIndex), SetBorderHorizontalColorIndexCore, value);
		}
		DocumentModelChangeActions SetBorderHorizontalColorCore(FormatBase info, Color value) {
			info.Border.HorizontalColor = value;
			return DocumentModelChangeActions.None; 
		}
		DocumentModelChangeActions SetBorderHorizontalColorIndexCore(FormatBase info, int value) {
			info.Border.HorizontalColorIndex = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region BorderVerticalColor
		Color IDifferentialFormatPropertyChanger.GetBorderVerticalColor(int elementIndex) {
			return GetDifferentialFormat(elementIndex).Border.VerticalColor;
		}
		void IDifferentialFormatPropertyChanger.SetBorderVerticalColor(int elementIndex, Color value) {
			DifferentialFormat info = GetDifferentialFormat(elementIndex);
			if (info.Border.VerticalColor == value && info.BorderOptionsInfo.ApplyVerticalColor)
				return;
			SetPropertyValue(GetDifferentialFormatIndexAccessor(elementIndex), SetBorderVerticalColorCore, value);
		}
		int IDifferentialFormatPropertyChanger.GetBorderVerticalColorIndex(int elementIndex) {
			return GetDifferentialFormat(elementIndex).Border.VerticalColorIndex;
		}
		void IDifferentialFormatPropertyChanger.SetBorderVerticalColorIndex(int elementIndex, int value) {
			DifferentialFormat info = GetDifferentialFormat(elementIndex);
			if (info.Border.VerticalColorIndex == value && info.BorderOptionsInfo.ApplyVerticalColor)
				return;
			SetPropertyValue(GetDifferentialFormatIndexAccessor(elementIndex), SetBorderVerticalColorIndexCore, value);
		}
		DocumentModelChangeActions SetBorderVerticalColorCore(FormatBase info, Color value) {
			info.Border.VerticalColor = value;
			return DocumentModelChangeActions.None; 
		}
		DocumentModelChangeActions SetBorderVerticalColorIndexCore(FormatBase info, int value) {
			info.Border.VerticalColorIndex = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region BorderDiagonalColor
		Color IDifferentialFormatPropertyChanger.GetBorderDiagonalColor(int elementIndex) {
			return GetDifferentialFormat(elementIndex).Border.DiagonalColor;
		}
		void IDifferentialFormatPropertyChanger.SetBorderDiagonalColor(int elementIndex, Color value) {
			DifferentialFormat info = GetDifferentialFormat(elementIndex);
			if (info.Border.DiagonalColor == value && info.BorderOptionsInfo.ApplyDiagonalColor)
				return;
			SetPropertyValue(GetDifferentialFormatIndexAccessor(elementIndex), SetBorderDiagonalColorCore, value);
		}
		int IDifferentialFormatPropertyChanger.GetBorderDiagonalColorIndex(int elementIndex) {
			return GetDifferentialFormat(elementIndex).Border.DiagonalColorIndex;
		}
		void IDifferentialFormatPropertyChanger.SetBorderDiagonalColorIndex(int elementIndex, int value) {
			DifferentialFormat info = GetDifferentialFormat(elementIndex);
			if (info.Border.DiagonalColorIndex == value && info.BorderOptionsInfo.ApplyDiagonalColor)
				return;
			SetPropertyValue(GetDifferentialFormatIndexAccessor(elementIndex), SetBorderDiagonalColorIndexCore, value);
		}
		DocumentModelChangeActions SetBorderDiagonalColorCore(FormatBase info, Color value) {
			info.Border.DiagonalColor = value;
			return DocumentModelChangeActions.None; 
		}
		DocumentModelChangeActions SetBorderDiagonalColorIndexCore(FormatBase info, int value) {
			info.Border.DiagonalColorIndex = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region BorderOutline
		bool IDifferentialFormatPropertyChanger.GetBorderOutline(int elementIndex) {
			return GetDifferentialFormat(elementIndex).Border.Outline;
		}
		void IDifferentialFormatPropertyChanger.SetBorderOutline(int elementIndex, bool value) {
			DifferentialFormat info = GetDifferentialFormat(elementIndex);
			if (info.Border.Outline == value && info.BorderOptionsInfo.ApplyOutline)
				return;
			SetPropertyValue(GetDifferentialFormatIndexAccessor(elementIndex), SetBorderOutlineCore, value);
		}
		DocumentModelChangeActions SetBorderOutlineCore(FormatBase info, bool value) {
			info.Border.Outline = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#endregion
		#region CellAlignmentPropertyChanger
		#region WrapText
		bool IDifferentialFormatPropertyChanger.GetCellAlignmentWrapText(int elementIndex) {
			return GetDifferentialFormat(elementIndex).Alignment.WrapText;
		}
		void IDifferentialFormatPropertyChanger.SetCellAlignmentWrapText(int elementIndex, bool value) {
			DifferentialFormat info = GetDifferentialFormat(elementIndex);
			if (info.Alignment.WrapText == value && info.MultiOptionsInfo.ApplyAlignmentWrapText)
				return;
			SetPropertyValue(GetDifferentialFormatIndexAccessor(elementIndex), SetCellAlignmentWrapTextCore, value);
		}
		DocumentModelChangeActions SetCellAlignmentWrapTextCore(FormatBase info, bool value) {
			info.Alignment.WrapText = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region JustifyLastLine
		bool IDifferentialFormatPropertyChanger.GetCellAlignmentJustifyLastLine(int elementIndex) {
			return GetDifferentialFormat(elementIndex).Alignment.JustifyLastLine;
		}
		void IDifferentialFormatPropertyChanger.SetCellAlignmentJustifyLastLine(int elementIndex, bool value) {
			DifferentialFormat info = GetDifferentialFormat(elementIndex);
			if (info.Alignment.JustifyLastLine == value && info.MultiOptionsInfo.ApplyAlignmentJustifyLastLine)
				return;
			SetPropertyValue(GetDifferentialFormatIndexAccessor(elementIndex), SetCellAlignmentJustifyLastLineCore, value);
		}
		DocumentModelChangeActions SetCellAlignmentJustifyLastLineCore(FormatBase info, bool value) {
			info.Alignment.JustifyLastLine = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region ShrinkToFit
		bool IDifferentialFormatPropertyChanger.GetCellAlignmentShrinkToFit(int elementIndex) {
			return GetDifferentialFormat(elementIndex).Alignment.ShrinkToFit;
		}
		void IDifferentialFormatPropertyChanger.SetCellAlignmentShrinkToFit(int elementIndex, bool value) {
			DifferentialFormat info = GetDifferentialFormat(elementIndex);
			if (info.Alignment.ShrinkToFit == value && info.MultiOptionsInfo.ApplyAlignmentShrinkToFit)
				return;
			SetPropertyValue(GetDifferentialFormatIndexAccessor(elementIndex), SetCellAlignmentShrinkToFitCore, value);
		}
		DocumentModelChangeActions SetCellAlignmentShrinkToFitCore(FormatBase info, bool value) {
			info.Alignment.ShrinkToFit = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region TextRotation
		int IDifferentialFormatPropertyChanger.GetCellAlignmentTextRotation(int elementIndex) {
			return GetDifferentialFormat(elementIndex).Alignment.TextRotation;
		}
		void IDifferentialFormatPropertyChanger.SetCellAlignmentTextRotation(int elementIndex, int value) {
			DifferentialFormat info = GetDifferentialFormat(elementIndex);
			if (info.Alignment.TextRotation == value && info.MultiOptionsInfo.ApplyAlignmentTextRotation)
				return;
			SetPropertyValue(GetDifferentialFormatIndexAccessor(elementIndex), SetCellAlignmentTextRotationCore, value);
		}
		DocumentModelChangeActions SetCellAlignmentTextRotationCore(FormatBase info, int value) {
			info.Alignment.TextRotation = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region Indent
		byte IDifferentialFormatPropertyChanger.GetCellAlignmentIndent(int elementIndex) {
			return GetDifferentialFormat(elementIndex).Alignment.Indent;
		}
		void IDifferentialFormatPropertyChanger.SetCellAlignmentIndent(int elementIndex, byte value) {
			DifferentialFormat info = GetDifferentialFormat(elementIndex);
			if (info.Alignment.Indent == value && info.MultiOptionsInfo.ApplyAlignmentIndent)
				return;
			SetPropertyValue(GetDifferentialFormatIndexAccessor(elementIndex), SetCellAlignmentIndentCore, value);
		}
		DocumentModelChangeActions SetCellAlignmentIndentCore(FormatBase info, byte value) {
			info.Alignment.Indent = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region RelativeIndent
		int IDifferentialFormatPropertyChanger.GetCellAlignmentRelativeIndent(int elementIndex) {
			return GetDifferentialFormat(elementIndex).Alignment.RelativeIndent;
		}
		void IDifferentialFormatPropertyChanger.SetCellAlignmentRelativeIndent(int elementIndex, int value) {
			DifferentialFormat info = GetDifferentialFormat(elementIndex);
			if (info.Alignment.RelativeIndent == value && info.MultiOptionsInfo.ApplyAlignmentRelativeIndent)
				return;
			SetPropertyValue(GetDifferentialFormatIndexAccessor(elementIndex), SetCellAlignmentRelativeIndentCore, value);
		}
		DocumentModelChangeActions SetCellAlignmentRelativeIndentCore(FormatBase info, int value) {
			info.Alignment.RelativeIndent = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region Horizontal
		XlHorizontalAlignment IDifferentialFormatPropertyChanger.GetCellAlignmentHorizontal(int elementIndex) {
			return GetDifferentialFormat(elementIndex).Alignment.Horizontal;
		}
		void IDifferentialFormatPropertyChanger.SetCellAlignmentHorizontal(int elementIndex, XlHorizontalAlignment value) {
			DifferentialFormat info = GetDifferentialFormat(elementIndex);
			if (info.Alignment.Horizontal == value && info.MultiOptionsInfo.ApplyAlignmentHorizontal)
				return;
			SetPropertyValue(GetDifferentialFormatIndexAccessor(elementIndex), SetCellAlignmentHorizontalCore, value);
		}
		DocumentModelChangeActions SetCellAlignmentHorizontalCore(FormatBase info, XlHorizontalAlignment value) {
			info.Alignment.Horizontal = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region Vertical
		XlVerticalAlignment IDifferentialFormatPropertyChanger.GetCellAlignmentVertical(int elementIndex) {
			return GetDifferentialFormat(elementIndex).Alignment.Vertical;
		}
		void IDifferentialFormatPropertyChanger.SetCellAlignmentVertical(int elementIndex, XlVerticalAlignment value) {
			DifferentialFormat info = GetDifferentialFormat(elementIndex);
			if (info.Alignment.Vertical == value && info.MultiOptionsInfo.ApplyAlignmentVertical)
				return;
			SetPropertyValue(GetDifferentialFormatIndexAccessor(elementIndex), SetCellAlignmentVerticalCore, value);
		}
		DocumentModelChangeActions SetCellAlignmentVerticalCore(FormatBase info, XlVerticalAlignment value) {
			info.Alignment.Vertical = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region XlReadingOrder
		XlReadingOrder IDifferentialFormatPropertyChanger.GetCellAlignmentReadingOrder(int elementIndex) {
			return GetDifferentialFormat(elementIndex).Alignment.ReadingOrder;
		}
		void IDifferentialFormatPropertyChanger.SetCellAlignmentReadingOrder(int elementIndex, XlReadingOrder value) {
			DifferentialFormat info = GetDifferentialFormat(elementIndex);
			if (info.Alignment.ReadingOrder == value && info.MultiOptionsInfo.ApplyAlignmentReadingOrder)
				return;
			SetPropertyValue(GetDifferentialFormatIndexAccessor(elementIndex), SetCellAlignmentReadingOrderCore, value);
		}
		DocumentModelChangeActions SetCellAlignmentReadingOrderCore(FormatBase info, XlReadingOrder value) {
			info.Alignment.ReadingOrder = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#endregion
		#region CellProtectionPropertyChanger
		#region Locked
		bool IDifferentialFormatPropertyChanger.GetCellProtectionLocked(int elementIndex) {
			return GetDifferentialFormat(elementIndex).Protection.Locked;
		}
		void IDifferentialFormatPropertyChanger.SetCellProtectionLocked(int elementIndex, bool value) {
			DifferentialFormat info = GetDifferentialFormat(elementIndex);
			if (info.Protection.Locked == value && info.MultiOptionsInfo.ApplyProtectionLocked)
				return;
			SetPropertyValue(GetDifferentialFormatIndexAccessor(elementIndex), SetCellProtectionLockedCore, value);
		}
		DocumentModelChangeActions SetCellProtectionLockedCore(FormatBase info, bool value) {
			info.Protection.Locked = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region Hidden
		bool IDifferentialFormatPropertyChanger.GetCellProtectionHidden(int elementIndex) {
			return GetDifferentialFormat(elementIndex).Protection.Hidden;
		}
		void IDifferentialFormatPropertyChanger.SetCellProtectionHidden(int elementIndex, bool value) {
			DifferentialFormat info = GetDifferentialFormat(elementIndex);
			if (info.Protection.Hidden == value && info.MultiOptionsInfo.ApplyProtectionLocked)
				return;
			SetPropertyValue(GetDifferentialFormatIndexAccessor(elementIndex), SetCellProtectionHiddenCore, value);
		}
		DocumentModelChangeActions SetCellProtectionHiddenCore(FormatBase info, bool value) {
			info.Protection.Hidden = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#endregion
		#endregion
		public bool GetApplyDifferentialFormat(int elementIndex) {
			DifferentialFormat format = GetDifferentialFormat(elementIndex);
			return format.MultiOptionsIndex != MultiOptionsInfo.DefaultIndex ||
				format.BorderOptionsIndex != BorderOptionsInfo.DefaultIndex;
		}
	}
}
