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
using DevExpress.Export.Xl;
using DevExpress.Compatibility.System.Drawing;
#if !SL
using System.Drawing;
#else
using System.Windows.Media;
#endif
namespace DevExpress.XtraSpreadsheet.Model {
	#region CellFormatModifier
	public class CellFormatModifier {
		protected internal void Modify(ICellFormat accessor, MergedCellFormat value) {
			SetFormatString(accessor, value.FormatString);
			SetAlignment(accessor.Alignment, value.Alignment);
			SetFont(accessor.Font, value.Font);
			SetFill(accessor.Fill, value.Fill);
			SetProtection(accessor, value.Protection);
		}
		void SetFormatString(IFormatStringAccessor ownerFormatString, MergedFormatStringInfo mergedFormatString) {
			string formatString = mergedFormatString.FormatString;
			if (formatString != null)
				ownerFormatString.FormatString = formatString;
		}
		void SetAlignment(ICellAlignmentInfo ownerAlignment, MergedAlignmentInfo mergedAlignment) {
			XlHorizontalAlignment? horizontal = mergedAlignment.Horizontal;
			if (horizontal.HasValue)
				ownerAlignment.Horizontal = horizontal.Value;
			byte? indent = mergedAlignment.Indent;
			if (indent.HasValue)
				ownerAlignment.Indent = indent.Value;
			bool? justifyLastLine = mergedAlignment.JustifyLastLine;
			if (justifyLastLine.HasValue)
				ownerAlignment.JustifyLastLine = justifyLastLine.Value;
			XlReadingOrder? readingOrder = mergedAlignment.ReadingOrder;
			if (readingOrder.HasValue)
				ownerAlignment.ReadingOrder = readingOrder.Value;
			int? relativeIndent = mergedAlignment.RelativeIndent;
			if (relativeIndent.HasValue)
				ownerAlignment.RelativeIndent = relativeIndent.Value;
			bool? shrinkToFit = mergedAlignment.ShrinkToFit;
			if (shrinkToFit.HasValue)
				ownerAlignment.ShrinkToFit = shrinkToFit.Value;
			int? textRotation = mergedAlignment.TextRotation;
			if (textRotation.HasValue)
				ownerAlignment.TextRotation = textRotation.Value;
			XlVerticalAlignment? vertical = mergedAlignment.Vertical;
			if (vertical.HasValue)
				ownerAlignment.Vertical = vertical.Value;
			bool? wrapText = mergedAlignment.WrapText;
			if (wrapText.HasValue)
				ownerAlignment.WrapText = wrapText.Value;
		}
		void SetFont(IRunFontInfo ownerFont, MergedFontInfo mergedFont) {
			XlFontSchemeStyles? schemeStyle = mergedFont.SchemeStyle;
			if (schemeStyle.HasValue)
				ownerFont.SchemeStyle = schemeStyle.Value;
			bool? bold = mergedFont.Bold;
			if (bold.HasValue)
				ownerFont.Bold = bold.Value;
			int? charset = mergedFont.Charset;
			if (charset.HasValue)
				ownerFont.Charset = charset.Value;
			Color? color = mergedFont.Color;
			if (color.HasValue)
				ownerFont.Color = color.Value;
			bool? condense = mergedFont.Condense;
			if (condense.HasValue)
				ownerFont.Condense = condense.Value;
			bool? extend = mergedFont.Extend;
			if (extend.HasValue)
				ownerFont.Extend = extend.Value;
			int? fontFamily = mergedFont.FontFamily;
			if (fontFamily.HasValue)
				ownerFont.FontFamily = fontFamily.Value;
			bool? italic = mergedFont.Italic;
			if (italic.HasValue)
				ownerFont.Italic = italic.Value;
			string name = mergedFont.Name;
			if (!String.IsNullOrEmpty(name))
				ownerFont.Name = name;
			bool? outline = mergedFont.Outline;
			if (outline.HasValue)
				ownerFont.Outline = outline.Value;
			XlScriptType? script = mergedFont.Script;
			if (script.HasValue)
				ownerFont.Script = script.Value;
			bool? shadow = mergedFont.Shadow;
			if (shadow.HasValue)
				ownerFont.Shadow = shadow.Value;
			double? size = mergedFont.Size;
			if (size.HasValue)
				ownerFont.Size = size.Value;
			bool? strikeThrough = mergedFont.StrikeThrough;
			if (strikeThrough.HasValue)
				ownerFont.StrikeThrough = strikeThrough.Value;
			XlUnderlineType? underline = mergedFont.Underline;
			if (underline.HasValue)
				ownerFont.Underline = underline.Value;
		}
		void SetFill(IFillInfo ownerFill, MergedFillInfo mergedFill) {
			bool ownerFillHasNonePatternStyle = ownerFill.PatternType == XlPatternType.None;
			bool ownerFillHasSolidPatternStyle = ownerFill.PatternType == XlPatternType.Solid;
			bool ownerFillHasOtherPatternStyle = ownerFill.PatternType != XlPatternType.None && ownerFill.PatternType != XlPatternType.Solid;
			bool mergedFillHasNonePatternStyle = mergedFill.PatternType.HasValue && mergedFill.PatternType == XlPatternType.None;
			bool mergedFillHasSolidPatternStyle = mergedFill.PatternType.HasValue && mergedFill.PatternType == XlPatternType.Solid;
			bool mergedFillHasOtherPatternStyle = mergedFill.PatternType.HasValue && mergedFill.PatternType != XlPatternType.None && mergedFill.PatternType != XlPatternType.Solid;
			if (ownerFillHasNonePatternStyle && mergedFillHasNonePatternStyle)
				return;
			else if (ownerFillHasNonePatternStyle && !mergedFill.PatternType.HasValue) {
				ownerFill.PatternType = XlPatternType.Solid;
				SetReverseFillsColor(ownerFill, mergedFill);
			}
			else if (ownerFillHasSolidPatternStyle && !mergedFill.PatternType.HasValue)
				SetReverseFillsColor(ownerFill, mergedFill);
			else if ((ownerFillHasNonePatternStyle || ownerFillHasSolidPatternStyle) && mergedFillHasSolidPatternStyle) {
				ownerFill.PatternType = mergedFill.PatternType.Value;
				SetReverseFillsColor(ownerFill, mergedFill);
			}
			else if (ownerFillHasOtherPatternStyle && mergedFillHasSolidPatternStyle) {
				ownerFill.PatternType = mergedFill.PatternType.Value;
				SwapFillsColor(ownerFill, mergedFill);
				SetReverseFillsColor(ownerFill, mergedFill);
			}
			else if (ownerFillHasSolidPatternStyle && mergedFillHasOtherPatternStyle) {
				ownerFill.PatternType = mergedFill.PatternType.Value;
				SwapFillsColor(ownerFill, mergedFill);
				SetFillsColor(ownerFill, mergedFill);
			}
			else {
				if (mergedFill.PatternType.HasValue)
					ownerFill.PatternType = mergedFill.PatternType.Value;
				SetFillsColor(ownerFill, mergedFill);
			}
			ModelFillType? fillType = mergedFill.FillType;
			if (fillType.HasValue)
				ownerFill.FillType = fillType.Value;
			SetGradientFill(ownerFill.GradientFill, mergedFill.GradientFill);
		}
		void SwapFillsColor(IFillInfo ownerFill, MergedFillInfo mergedFill) {
			Color temp = ownerFill.BackColor;
			ownerFill.BackColor = ownerFill.ForeColor;
			ownerFill.ForeColor = temp;
		}
		void SetFillsColor(IFillInfo ownerFill, MergedFillInfo mergedFill) {
			if (mergedFill.BackColor.HasValue)
				ownerFill.BackColor = mergedFill.BackColor.Value;
			if (mergedFill.ForeColor.HasValue)
				ownerFill.ForeColor = mergedFill.ForeColor.Value;
		}
		void SetReverseFillsColor(IFillInfo ownerFill, MergedFillInfo mergedFill) {
			if (mergedFill.BackColor.HasValue)
				ownerFill.ForeColor = mergedFill.BackColor.Value;
			if (mergedFill.ForeColor.HasValue)
				ownerFill.BackColor = mergedFill.ForeColor.Value;
		}
		void SetGradientFill(IGradientFillInfo ownerGradientFill, MergedGradientFill mergedGradientFill) {
			double? degree = mergedGradientFill.Degree;
			if (degree.HasValue)
				ownerGradientFill.Degree = degree.Value;
			ModelGradientFillType? type = mergedGradientFill.Type;
			if (type.HasValue)
				ownerGradientFill.Type = type.Value;
			SetConvergence(ownerGradientFill.Convergence, mergedGradientFill.Convergence);
		}
		void SetConvergence(IConvergenceInfo ownerConvergence, MergedConvergence mergedConvergence) {
			float? bottom = mergedConvergence.Bottom;
			if (bottom.HasValue)
				ownerConvergence.Bottom = bottom.Value;
			float? left = mergedConvergence.Left;
			if (left.HasValue)
				ownerConvergence.Left = left.Value;
			float? right = mergedConvergence.Right;
			if (right.HasValue)
				ownerConvergence.Right = right.Value;
			float? top = mergedConvergence.Top;
			if (top.HasValue)
				ownerConvergence.Top = top.Value;
		}
		void SetProtection(ICellProtectionInfo ownerProtection, MergedCellProtectionInfo mergedProtection) {
			bool? hidden = mergedProtection.Hidden;
			if (hidden.HasValue)
				ownerProtection.Hidden = hidden.Value;
			bool? locked = mergedProtection.Locked;
			if (locked.HasValue)
				ownerProtection.Locked = locked.Value;
		}
	}
	#endregion
}
