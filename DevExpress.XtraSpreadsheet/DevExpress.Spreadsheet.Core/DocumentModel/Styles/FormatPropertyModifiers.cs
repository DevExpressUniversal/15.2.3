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

using System.Collections.Generic;
using DevExpress.Utils;
using DevExpress.Office;
using DevExpress.Office.Utils;
using DevExpress.Export.Xl;
using DevExpress.Compatibility.System.Drawing;
#if !SL
using System.Drawing;
using System;
#else
using System.Windows.Media;
#endif
namespace DevExpress.XtraSpreadsheet.Model {
	#region FormatBasePropertyModifierBase (abstract)
	public abstract class FormatBasePropertyModifierBase {
		public abstract void ModifyFormat(Model.IFormatBaseBatchUpdateable cell);
	}
	#endregion
	#region FormatBasePropertyModifier (abstract)
	public abstract class FormatBasePropertyModifier<T> : FormatBasePropertyModifierBase {
		readonly T newValue;
		protected FormatBasePropertyModifier(T newValue) {
			this.newValue = ValidateNewValue(newValue);
		}
		public T NewValue { get { return newValue; } }
		protected internal virtual T ValidateNewValue(T newValue) {
			return newValue;
		}
		public abstract T GetFormatPropertyValue(Model.IFormatBaseBatchUpdateable cell);
		public abstract T GetFormatPropertyValue(Model.FormatBase format);
	}
	#endregion
	public abstract class CellFormatPropertyModifierBase {
		public abstract void ModifyFormat(Model.ICell cell);
	}
	#region CellFormatPropertyModifier (abstract)
	public abstract class CellFormatPropertyModifier<T> : CellFormatPropertyModifierBase {
		readonly T newValue;
		protected CellFormatPropertyModifier(T newValue) {
			this.newValue = ValidateNewValue(newValue);
		}
		public T NewValue { get { return newValue; } }
		protected internal virtual T ValidateNewValue(T newValue) {
			return newValue;
		}
		public abstract T GetFormatPropertyValue(Model.ICell cell);
		public abstract T GetFormatPropertyValue(Model.FormatBase format);
	}
	#endregion
	#region NumberFormat
	public class NumberFormatPropertyModifier : FormatBasePropertyModifier<string> {
		public NumberFormatPropertyModifier(string val)
			: base(val) {
		}
		public override void ModifyFormat(Model.IFormatBaseBatchUpdateable cell) {
			cell.FormatString = NewValue;
		}
		public override string GetFormatPropertyValue(Model.IFormatBaseBatchUpdateable cell) {
			return cell.FormatString;
		}
		public override string GetFormatPropertyValue(Model.FormatBase format) {
			return format.FormatString;
		}
	}
	#endregion
	#region Alignment
	public class AlignmentHorizontalPropertyModifier : FormatBasePropertyModifier<XlHorizontalAlignment> {
		public AlignmentHorizontalPropertyModifier(XlHorizontalAlignment val)
			: base(val) {
		}
		public override void ModifyFormat(Model.IFormatBaseBatchUpdateable cell) {
			cell.Alignment.Horizontal = NewValue;
		}
		public override XlHorizontalAlignment GetFormatPropertyValue(Model.IFormatBaseBatchUpdateable cell) {
			return cell.ActualAlignment.Horizontal;
		}
		public override XlHorizontalAlignment GetFormatPropertyValue(Model.FormatBase format) {
			return format.Alignment.Horizontal;
		}
	}
	public class AlignmentVerticalPropertyModifier : FormatBasePropertyModifier<XlVerticalAlignment> {
		public AlignmentVerticalPropertyModifier(XlVerticalAlignment val)
			: base(val) {
		}
		public override void ModifyFormat(Model.IFormatBaseBatchUpdateable cell) {
			cell.Alignment.Vertical = NewValue;
		}
		public override XlVerticalAlignment GetFormatPropertyValue(Model.IFormatBaseBatchUpdateable cell) {
			return cell.ActualAlignment.Vertical;
		}
		public override XlVerticalAlignment GetFormatPropertyValue(Model.FormatBase format) {
			return format.Alignment.Vertical;
		}
	}
	public class AlignmentRotationAnglePropertyModifier : FormatBasePropertyModifier<int> {
		public AlignmentRotationAnglePropertyModifier(int val)
			: base(val) {
		}
		public override void ModifyFormat(Model.IFormatBaseBatchUpdateable cell) {
			cell.Alignment.TextRotation = NewValue;
		}
		public override int GetFormatPropertyValue(Model.IFormatBaseBatchUpdateable cell) {
			return cell.ActualAlignment.TextRotation;
		}
		public override int GetFormatPropertyValue(Model.FormatBase format) {
			return format.Alignment.TextRotation;
		}
	}
	public class AlignmentJustifyDistributedPropertyModifier : FormatBasePropertyModifier<bool> {
		public AlignmentJustifyDistributedPropertyModifier(bool val)
			: base(val) {
		}
		public override void ModifyFormat(Model.IFormatBaseBatchUpdateable cell) {
			cell.Alignment.JustifyLastLine = NewValue;
		}
		public override bool GetFormatPropertyValue(Model.IFormatBaseBatchUpdateable cell) {
			return cell.ActualAlignment.JustifyLastLine;
		}
		public override bool GetFormatPropertyValue(Model.FormatBase format) {
			return format.Alignment.JustifyLastLine;
		}
	}
	public class AlignmentIndentPropertyModifier : FormatBasePropertyModifier<byte> {
		public AlignmentIndentPropertyModifier(byte val)
			: base(val) {
		}
		public override void ModifyFormat(Model.IFormatBaseBatchUpdateable cell) {
			cell.Alignment.Indent = NewValue;
		}
		public override byte GetFormatPropertyValue(Model.IFormatBaseBatchUpdateable cell) {
			return cell.ActualAlignment.Indent;
		}
		public override byte GetFormatPropertyValue(Model.FormatBase format) {
			return format.Alignment.Indent;
		}
	}
	public class AlignmentShrinkToFitPropertyModifier : FormatBasePropertyModifier<bool> {
		public AlignmentShrinkToFitPropertyModifier(bool val)
			: base(val) {
		}
		public override void ModifyFormat(Model.IFormatBaseBatchUpdateable cell) {
			cell.Alignment.ShrinkToFit = NewValue;
		}
		public override bool GetFormatPropertyValue(Model.IFormatBaseBatchUpdateable cell) {
			return cell.ActualAlignment.ShrinkToFit;
		}
		public override bool GetFormatPropertyValue(Model.FormatBase format) {
			return format.Alignment.ShrinkToFit;
		}
	}
	public class AlignmentWrapTextPropertyModifier : FormatBasePropertyModifier<bool> {
		public AlignmentWrapTextPropertyModifier(bool val)
			: base(val) {
		}
		public override void ModifyFormat(Model.IFormatBaseBatchUpdateable cell) {
			cell.Alignment.WrapText = NewValue;
		}
		public override bool GetFormatPropertyValue(Model.IFormatBaseBatchUpdateable cell) {
			return cell.ActualAlignment.WrapText;
		}
		public override bool GetFormatPropertyValue(Model.FormatBase format) {
			return format.Alignment.WrapText;
		}
	}
	#endregion
	#region Font
	public class FontStylePropertyModifier : FormatBasePropertyModifier<DevExpress.Spreadsheet.SpreadsheetFontStyle> {
		public FontStylePropertyModifier(DevExpress.Spreadsheet.SpreadsheetFontStyle val)
			: base(val) {
		}
		public override void ModifyFormat(Model.IFormatBaseBatchUpdateable cell) {
			cell.BeginUpdate();
			try {
				switch (NewValue) {
					case Spreadsheet.SpreadsheetFontStyle.Regular:
						cell.Font.Bold = false;
						cell.Font.Italic = false;
						break;
					case Spreadsheet.SpreadsheetFontStyle.Italic:
						cell.Font.Bold = false;
						cell.Font.Italic = true;
						break;
					case Spreadsheet.SpreadsheetFontStyle.Bold:
						cell.Font.Bold = true;
						cell.Font.Italic = false;
						break;
					case Spreadsheet.SpreadsheetFontStyle.BoldItalic:
						cell.Font.Bold = true;
						cell.Font.Italic = true;
						break;
				}
			}
			finally {
				cell.EndUpdate();
			}
		}
		public override DevExpress.Spreadsheet.SpreadsheetFontStyle GetFormatPropertyValue(Model.IFormatBaseBatchUpdateable cell) {
			return ConvertToFontStyle(cell.ActualFont.Bold, cell.ActualFont.Italic);
		}
		public override DevExpress.Spreadsheet.SpreadsheetFontStyle GetFormatPropertyValue(Model.FormatBase format) {
			return ConvertToFontStyle(format.Font.Bold, format.Font.Italic);
		}
		DevExpress.Spreadsheet.SpreadsheetFontStyle ConvertToFontStyle(bool bold, bool italic) {
			bool isBold = bold;
			bool isItalic = italic;
			if (!isBold && !isItalic)
				return DevExpress.Spreadsheet.SpreadsheetFontStyle.Regular;
			else if (isBold && isItalic)
				return DevExpress.Spreadsheet.SpreadsheetFontStyle.BoldItalic;
			else if (isBold && !isItalic)
				return DevExpress.Spreadsheet.SpreadsheetFontStyle.Bold;
			return DevExpress.Spreadsheet.SpreadsheetFontStyle.Italic;
		}
	}
	public class FontUnderlineTypePropertyModifier : FormatBasePropertyModifier<XlUnderlineType> {
		public FontUnderlineTypePropertyModifier(XlUnderlineType val)
			: base(val) {
		}
		public override void ModifyFormat(Model.IFormatBaseBatchUpdateable cell) {
			cell.Font.Underline = NewValue;
		}
		public override XlUnderlineType GetFormatPropertyValue(Model.IFormatBaseBatchUpdateable cell) {
			return cell.ActualFont.Underline;
		}
		public override XlUnderlineType GetFormatPropertyValue(Model.FormatBase format) {
			return format.Font.Underline;
		}
	}
	public class FontItalicPropertyModifier : FormatBasePropertyModifier<bool> {
		public FontItalicPropertyModifier(bool val)
			: base(val) {
		}
		public override void ModifyFormat(Model.IFormatBaseBatchUpdateable cell) {
			cell.Font.Italic = NewValue;
		}
		public override bool GetFormatPropertyValue(Model.IFormatBaseBatchUpdateable cell) {
			return cell.ActualFont.Italic;
		}
		public override bool GetFormatPropertyValue(Model.FormatBase format) {
			return format.Font.Italic;
		}
	}
	public class FontBoldPropertyModifier : FormatBasePropertyModifier<bool> {
		public FontBoldPropertyModifier(bool val)
			: base(val) {
		}
		public override void ModifyFormat(Model.IFormatBaseBatchUpdateable cell) {
			cell.Font.Bold = NewValue;
		}
		public override bool GetFormatPropertyValue(Model.IFormatBaseBatchUpdateable cell) {
			return cell.ActualFont.Bold;
		}
		public override bool GetFormatPropertyValue(Model.FormatBase format) {
			return format.Font.Bold;
		}
	}
	public class FontScriptPropertyModifier : FormatBasePropertyModifier<XlScriptType> {
		public FontScriptPropertyModifier(XlScriptType val)
			: base(val) {
		}
		public override void ModifyFormat(Model.IFormatBaseBatchUpdateable cell) {
			cell.Font.Script = NewValue;
		}
		public override XlScriptType GetFormatPropertyValue(Model.IFormatBaseBatchUpdateable cell) {
			return cell.Font.Script;
		}
		public override XlScriptType GetFormatPropertyValue(Model.FormatBase format) {
			return format.Font.Script;
		}
	}
	public class FontOutlinePropertyModifier : FormatBasePropertyModifier<bool> {
		public FontOutlinePropertyModifier(bool newValue)
			: base(newValue) {
		}
		public override void ModifyFormat(Model.IFormatBaseBatchUpdateable cell) {
			cell.Font.Outline = NewValue;
		}
		public override bool GetFormatPropertyValue(Model.IFormatBaseBatchUpdateable cell) {
			return cell.ActualFont.Outline;
		}
		public override bool GetFormatPropertyValue(Model.FormatBase format) {
			return format.Font.Outline;
		}
	}
	public class FontSizePropertyModifier : FormatBasePropertyModifier<double> {
		public FontSizePropertyModifier(double val)
			: base(val) {
		}
		public override void ModifyFormat(Model.IFormatBaseBatchUpdateable cell) {
			cell.Font.Size = NewValue;
		}
		public override double GetFormatPropertyValue(Model.IFormatBaseBatchUpdateable cell) {
			return cell.ActualFont.Size;
		}
		public override double GetFormatPropertyValue(Model.FormatBase format) {
			return format.Font.Size;
		}
	}
	public class FontStrikeThroughPropertyModifier : FormatBasePropertyModifier<bool> {
		public FontStrikeThroughPropertyModifier(bool val)
			: base(val) {
		}
		public override void ModifyFormat(Model.IFormatBaseBatchUpdateable cell) {
			cell.Font.StrikeThrough = NewValue;
		}
		public override bool GetFormatPropertyValue(Model.IFormatBaseBatchUpdateable cell) {
			return cell.ActualFont.StrikeThrough;
		}
		public override bool GetFormatPropertyValue(Model.FormatBase format) {
			return format.Font.StrikeThrough;
		}
	}
	public class FontColorPropertyModifier : FormatBasePropertyModifier<Color> {
		public FontColorPropertyModifier(Color val)
			: base(val) {
		}
		public override void ModifyFormat(Model.IFormatBaseBatchUpdateable cell) {
			cell.Font.Color = NewValue;
		}
		public override Color GetFormatPropertyValue(Model.IFormatBaseBatchUpdateable cell) {
			return cell.ActualFont.Color;
		}
		public override Color GetFormatPropertyValue(Model.FormatBase format) {
			return format.Font.Color;
		}
	}
	public class FontNamePropertyModifier : FormatBasePropertyModifier<string> {
		public FontNamePropertyModifier(string val)
			: base(val) {
		}
		public override void ModifyFormat(Model.IFormatBaseBatchUpdateable cell) {
			cell.Font.Name = NewValue;
		}
		public override string GetFormatPropertyValue(Model.IFormatBaseBatchUpdateable cell) {
			return cell.ActualFont.Name;
		}
		public override string GetFormatPropertyValue(Model.FormatBase format) {
			return format.Font.Name;
		}
	}
	#endregion
	#region Protection
	#region ProtectionHiddenPropertyModifier
	public class ProtectionHiddenPropertyModifier : FormatBasePropertyModifier<bool> {
		public ProtectionHiddenPropertyModifier(bool val)
			: base(val) {
		}
		public override void ModifyFormat(Model.IFormatBaseBatchUpdateable cell) {
			cell.Protection.Hidden = NewValue;
		}
		public override bool GetFormatPropertyValue(Model.IFormatBaseBatchUpdateable cell) {
			return cell.ActualProtection.Hidden;
		}
		public override bool GetFormatPropertyValue(Model.FormatBase format) {
			return format.Protection.Hidden;
		}
	}
	#endregion
	#region ProtectionLockedPropertyModifier
	public class ProtectionLockedPropertyModifier : FormatBasePropertyModifier<bool> {
		public ProtectionLockedPropertyModifier(bool val)
			: base(val) {
		}
		public override void ModifyFormat(Model.IFormatBaseBatchUpdateable cell) {
			cell.Protection.Locked = NewValue;
		}
		public override bool GetFormatPropertyValue(Model.IFormatBaseBatchUpdateable cell) {
			return cell.ActualProtection.Locked;
		}
		public override bool GetFormatPropertyValue(Model.FormatBase format) {
			return format.Protection.Locked;
		}
	}
	#endregion
	#endregion
	#region Fill
	#region PatternFillBackColorPropertyModifier
	public class FillBackColorPropertyModifier : FormatBasePropertyModifier<Color> {
		public FillBackColorPropertyModifier(Color val)
			: base(val) {
		}
		public override void ModifyFormat(Model.IFormatBaseBatchUpdateable cell) {
			cell.Fill.BackColor = NewValue;
		}
		public override Color GetFormatPropertyValue(Model.IFormatBaseBatchUpdateable cell) {
			return cell.ActualFill.BackColor;
		}
		public override Color GetFormatPropertyValue(Model.FormatBase format) {
			return format.Fill.BackColor;
		}
	}
	#endregion
	#region PatternFillForeColorPropertyModifier
	public class FillForeColorPropertyModifier : FormatBasePropertyModifier<Color> {
		public FillForeColorPropertyModifier(Color val)
			: base(val) {
		}
		public override void ModifyFormat(Model.IFormatBaseBatchUpdateable cell) {
			cell.Fill.ForeColor = NewValue;
		}
		public override Color GetFormatPropertyValue(Model.IFormatBaseBatchUpdateable cell) {
			return cell.ActualFill.ForeColor;
		}
		public override Color GetFormatPropertyValue(Model.FormatBase format) {
			return format.Fill.ForeColor;
		}
	}
	#endregion
	#region PatternFillPatternTypePropertyModifier
	public class FillPatternTypePropertyModifier : FormatBasePropertyModifier<XlPatternType> {
		public FillPatternTypePropertyModifier(XlPatternType val)
			: base(val) {
		}
		public override void ModifyFormat(Model.IFormatBaseBatchUpdateable cell) {
			cell.Fill.PatternType = NewValue;
		}
		public override XlPatternType GetFormatPropertyValue(Model.IFormatBaseBatchUpdateable cell) {
			return cell.ActualFill.PatternType;
		}
		public override XlPatternType GetFormatPropertyValue(Model.FormatBase format) {
			return format.Fill.PatternType;
		}
	}
	#endregion
	#region FillTypePropertyModifier
	public class FillTypePropertyModifier : FormatBasePropertyModifier<Model.ModelFillType> {
		public FillTypePropertyModifier(Model.ModelFillType val)
			: base(val) {
		}
		public override void ModifyFormat(Model.IFormatBaseBatchUpdateable cell) {
			cell.Fill.FillType = NewValue;
		}
		public override Model.ModelFillType GetFormatPropertyValue(Model.IFormatBaseBatchUpdateable cell) {
			return cell.ActualFill.FillType;
		}
		public override Model.ModelFillType GetFormatPropertyValue(Model.FormatBase format) {
			return format.Fill.FillType;
		}
	}
	#endregion
	#region GradientFillTypePropertyModifier
	public class GradientFillTypePropertyModifier : FormatBasePropertyModifier<Model.ModelGradientFillType> {
		public GradientFillTypePropertyModifier(Model.ModelGradientFillType val)
			: base(val) {
		}
		public override void ModifyFormat(Model.IFormatBaseBatchUpdateable cell) {
			cell.Fill.GradientFill.Type = NewValue;
		}
		public override Model.ModelGradientFillType GetFormatPropertyValue(Model.IFormatBaseBatchUpdateable cell) {
			return cell.ActualFill.GradientFill.Type;
		}
		public override Model.ModelGradientFillType GetFormatPropertyValue(Model.FormatBase format) {
			return format.Fill.GradientFill.Type;
		}
	}
	#endregion
	#region GradientFillDegreePropertyModifier
	public class GradientFillDegreePropertyModifier : FormatBasePropertyModifier<double> {
		public GradientFillDegreePropertyModifier(double val)
			: base(val) {
		}
		public override void ModifyFormat(Model.IFormatBaseBatchUpdateable cell) {
			cell.Fill.GradientFill.Degree = NewValue;
		}
		public override double GetFormatPropertyValue(Model.IFormatBaseBatchUpdateable cell) {
			return cell.ActualFill.GradientFill.Degree;
		}
		public override double GetFormatPropertyValue(Model.FormatBase format) {
			return format.Fill.GradientFill.Degree;
		}
	}
	#endregion
	#region GradientFillRectangleLeftPropertyModifier
	public class GradientFillRectangleLeftPropertyModifier : FormatBasePropertyModifier<float> {
		public GradientFillRectangleLeftPropertyModifier(float val)
			: base(val) {
		}
		public override void ModifyFormat(Model.IFormatBaseBatchUpdateable cell) {
			cell.Fill.GradientFill.Convergence.Left = NewValue;
		}
		public override float GetFormatPropertyValue(Model.IFormatBaseBatchUpdateable cell) {
			return cell.ActualFill.GradientFill.Convergence.Left;
		}
		public override float GetFormatPropertyValue(Model.FormatBase format) {
			return format.Fill.GradientFill.Convergence.Left;
		}
	}
	#endregion
	#region GradientFillRectangleRightPropertyModifier
	public class GradientFillRectangleRightPropertyModifier : FormatBasePropertyModifier<float> {
		public GradientFillRectangleRightPropertyModifier(float val)
			: base(val) {
		}
		public override void ModifyFormat(Model.IFormatBaseBatchUpdateable cell) {
			cell.Fill.GradientFill.Convergence.Right = NewValue;
		}
		public override float GetFormatPropertyValue(Model.IFormatBaseBatchUpdateable cell) {
			return cell.ActualFill.GradientFill.Convergence.Right;
		}
		public override float GetFormatPropertyValue(Model.FormatBase format) {
			return format.Fill.GradientFill.Convergence.Right;
		}
	}
	#endregion
	#region GradientFillRectangleTopPropertyModifier
	public class GradientFillRectangleTopPropertyModifier : FormatBasePropertyModifier<float> {
		public GradientFillRectangleTopPropertyModifier(float val)
			: base(val) {
		}
		public override void ModifyFormat(Model.IFormatBaseBatchUpdateable cell) {
			cell.Fill.GradientFill.Convergence.Top = NewValue;
		}
		public override float GetFormatPropertyValue(Model.IFormatBaseBatchUpdateable cell) {
			return cell.ActualFill.GradientFill.Convergence.Top;
		}
		public override float GetFormatPropertyValue(Model.FormatBase format) {
			return format.Fill.GradientFill.Convergence.Top;
		}
	}
	#endregion
	#region GradientFillRectangleBottomPropertyModifier
	public class GradientFillRectangleBottomPropertyModifier : FormatBasePropertyModifier<float> {
		public GradientFillRectangleBottomPropertyModifier(float val)
			: base(val) {
		}
		public override void ModifyFormat(Model.IFormatBaseBatchUpdateable cell) {
			cell.Fill.GradientFill.Convergence.Bottom = NewValue;
		}
		public override float GetFormatPropertyValue(Model.IFormatBaseBatchUpdateable cell) {
			return cell.ActualFill.GradientFill.Convergence.Bottom;
		}
		public override float GetFormatPropertyValue(Model.FormatBase format) {
			return format.Fill.GradientFill.Convergence.Bottom;
		}
	}
	#endregion
	#region GradientFillStopsPropertyModifier
	public class GradientFillStopsPropertyModifier : FormatBasePropertyModifier<Model.IGradientStopCollection> {
		public GradientFillStopsPropertyModifier(Model.IGradientStopCollection val)
			: base(val) {
		}
		public override void ModifyFormat(Model.IFormatBaseBatchUpdateable cell) {
			cell.Fill.GradientFill.GradientStops.CopyFrom(NewValue);
		}
		public override Model.IGradientStopCollection GetFormatPropertyValue(Model.IFormatBaseBatchUpdateable cell) {
			return cell.ActualFill.GradientFill.GradientStops as GradientStopInfoCollection;
		}
		public override Model.IGradientStopCollection GetFormatPropertyValue(Model.FormatBase format) {
			return format.Fill.GradientFill.GradientStops;
		}
	}
	#endregion
	#endregion
	#region Borders
	#region LeftBorderColorPropertyModifier
	public class LeftBorderColorPropertyModifier : FormatBasePropertyModifier<Color> {
		public LeftBorderColorPropertyModifier(Color val)
			: base(val) {
		}
		public override void ModifyFormat(Model.IFormatBaseBatchUpdateable cell) {
			cell.Border.LeftColor = NewValue;
		}
		public override Color GetFormatPropertyValue(Model.IFormatBaseBatchUpdateable cell) {
			return cell.ActualBorder.LeftColor;
		}
		public override Color GetFormatPropertyValue(Model.FormatBase format) {
			return format.Border.LeftColor;
		}
	}
	#endregion
	#region LeftBorderLineStylePropertyModifier
	public class LeftBorderLineStylePropertyModifier : FormatBasePropertyModifier<XlBorderLineStyle> {
		public LeftBorderLineStylePropertyModifier(XlBorderLineStyle val)
			: base(val) {
		}
		public override void ModifyFormat(Model.IFormatBaseBatchUpdateable cell) {
			cell.Border.LeftLineStyle = NewValue;
		}
		public override XlBorderLineStyle GetFormatPropertyValue(Model.IFormatBaseBatchUpdateable cell) {
			return cell.ActualBorder.LeftLineStyle;
		}
		public override XlBorderLineStyle GetFormatPropertyValue(Model.FormatBase format) {
			return format.Border.LeftLineStyle;
		}
	}
	#endregion
	#region RightBorderColorPropertyModifier
	public class RightBorderColorPropertyModifier : FormatBasePropertyModifier<Color> {
		public RightBorderColorPropertyModifier(Color val)
			: base(val) {
		}
		public override void ModifyFormat(Model.IFormatBaseBatchUpdateable cell) {
			cell.Border.RightColor = NewValue;
		}
		public override Color GetFormatPropertyValue(Model.IFormatBaseBatchUpdateable cell) {
			return cell.ActualBorder.RightColor;
		}
		public override Color GetFormatPropertyValue(Model.FormatBase format) {
			return format.Border.RightColor;
		}
	}
	#endregion
	#region RightBorderLineStylePropertyModifier
	public class RightBorderLineStylePropertyModifier : FormatBasePropertyModifier<XlBorderLineStyle> {
		public RightBorderLineStylePropertyModifier(XlBorderLineStyle val)
			: base(val) {
		}
		public override void ModifyFormat(Model.IFormatBaseBatchUpdateable cell) {
			cell.Border.RightLineStyle = NewValue;
		}
		public override XlBorderLineStyle GetFormatPropertyValue(Model.IFormatBaseBatchUpdateable cell) {
			return cell.ActualBorder.RightLineStyle;
		}
		public override XlBorderLineStyle GetFormatPropertyValue(Model.FormatBase format) {
			return format.Border.RightLineStyle;
		}
	}
	#endregion
	#region TopBorderColorPropertyModifier
	public class TopBorderColorPropertyModifier : FormatBasePropertyModifier<Color> {
		public TopBorderColorPropertyModifier(Color val)
			: base(val) {
		}
		public override void ModifyFormat(Model.IFormatBaseBatchUpdateable cell) {
			cell.Border.TopColor = NewValue;
		}
		public override Color GetFormatPropertyValue(Model.IFormatBaseBatchUpdateable cell) {
			return cell.ActualBorder.TopColor;
		}
		public override Color GetFormatPropertyValue(Model.FormatBase format) {
			return format.Border.TopColor;
		}
	}
	#endregion
	#region TopBorderLineStylePropertyModifier
	public class TopBorderLineStylePropertyModifier : FormatBasePropertyModifier<XlBorderLineStyle> {
		public TopBorderLineStylePropertyModifier(XlBorderLineStyle val)
			: base(val) {
		}
		public override void ModifyFormat(Model.IFormatBaseBatchUpdateable cell) {
			cell.Border.TopLineStyle = NewValue;
		}
		public override XlBorderLineStyle GetFormatPropertyValue(Model.IFormatBaseBatchUpdateable cell) {
			return cell.ActualBorder.TopLineStyle;
		}
		public override XlBorderLineStyle GetFormatPropertyValue(Model.FormatBase format) {
			return format.Border.TopLineStyle;
		}
	}
	#endregion
	#region BottomBorderColorPropertyModifier
	public class BottomBorderColorPropertyModifier : FormatBasePropertyModifier<Color> {
		public BottomBorderColorPropertyModifier(Color val)
			: base(val) {
		}
		public override void ModifyFormat(Model.IFormatBaseBatchUpdateable cell) {
			cell.Border.BottomColor = NewValue;
		}
		public override Color GetFormatPropertyValue(Model.IFormatBaseBatchUpdateable cell) {
			return cell.ActualBorder.BottomColor;
		}
		public override Color GetFormatPropertyValue(Model.FormatBase format) {
			return format.Border.BottomColor;
		}
	}
	#endregion
	#region BottomBorderLineStylePropertyModifier
	public class BottomBorderLineStylePropertyModifier : FormatBasePropertyModifier<XlBorderLineStyle> {
		public BottomBorderLineStylePropertyModifier(XlBorderLineStyle val)
			: base(val) {
		}
		public override void ModifyFormat(Model.IFormatBaseBatchUpdateable cell) {
			cell.Border.BottomLineStyle = NewValue;
		}
		public override XlBorderLineStyle GetFormatPropertyValue(Model.IFormatBaseBatchUpdateable cell) {
			return cell.ActualBorder.BottomLineStyle;
		}
		public override XlBorderLineStyle GetFormatPropertyValue(Model.FormatBase format) {
			return format.Border.BottomLineStyle;
		}
	}
	#endregion
	#region DiagonalBorderColorPropertyModifier
	public class DiagonalBorderColorPropertyModifier : FormatBasePropertyModifier<Color> {
		public DiagonalBorderColorPropertyModifier(Color val)
			: base(val) {
		}
		public override void ModifyFormat(Model.IFormatBaseBatchUpdateable cell) {
			cell.Border.DiagonalColor = NewValue;
		}
		public override Color GetFormatPropertyValue(Model.IFormatBaseBatchUpdateable cell) {
			return cell.ActualBorder.DiagonalColor;
		}
		public override Color GetFormatPropertyValue(Model.FormatBase format) {
			return format.Border.DiagonalColor;
		}
	}
	#endregion
	#region DiagonalUpBorderLineStylePropertyModifier
	public class DiagonalUpBorderLineStylePropertyModifier : FormatBasePropertyModifier<XlBorderLineStyle> {
		public DiagonalUpBorderLineStylePropertyModifier(XlBorderLineStyle val)
			: base(val) {
		}
		public override void ModifyFormat(Model.IFormatBaseBatchUpdateable cell) {
			cell.Border.DiagonalUpLineStyle = NewValue;
		}
		public override XlBorderLineStyle GetFormatPropertyValue(Model.IFormatBaseBatchUpdateable cell) {
			return cell.ActualBorder.DiagonalUpLineStyle;
		}
		public override XlBorderLineStyle GetFormatPropertyValue(Model.FormatBase format) {
			return format.Border.DiagonalUpLineStyle;
		}
	}
	#endregion
	#region DiagonalDownBorderLineStylePropertyModifier
	public class DiagonalDownBorderLineStylePropertyModifier : FormatBasePropertyModifier<XlBorderLineStyle> {
		public DiagonalDownBorderLineStylePropertyModifier(XlBorderLineStyle val)
			: base(val) {
		}
		public override void ModifyFormat(Model.IFormatBaseBatchUpdateable cell) {
			cell.Border.DiagonalDownLineStyle = NewValue;
		}
		public override XlBorderLineStyle GetFormatPropertyValue(Model.IFormatBaseBatchUpdateable cell) {
			return cell.ActualBorder.DiagonalDownLineStyle;
		}
		public override XlBorderLineStyle GetFormatPropertyValue(Model.FormatBase format) {
			return format.Border.DiagonalDownLineStyle;
		}
	}
	#endregion
	#endregion
	#region ApplyAllPropertyModifier
	public class ApplyAllPropertyModifier : FormatBasePropertyModifier<bool> {
		public ApplyAllPropertyModifier(bool val)
			: base(val) {
		}
		public override void ModifyFormat(Model.IFormatBaseBatchUpdateable cell) {
			cell.ApplyNumberFormat = NewValue;
			cell.ApplyFont = NewValue;
			cell.ApplyFill = NewValue;
			cell.ApplyBorder = NewValue;
			cell.ApplyAlignment = NewValue;
			cell.ApplyProtection = NewValue;
		}
		public override bool GetFormatPropertyValue(Model.IFormatBaseBatchUpdateable cell) {
			return cell.ApplyNumberFormat
			&& cell.ApplyFont
			&& cell.ApplyFill
			&& cell.ApplyBorder
			&& cell.ApplyAlignment
			&& cell.ApplyProtection;
		}
		public override bool GetFormatPropertyValue(Model.FormatBase format) {
			return false;
		}
	}
	#endregion
	#region ApplyNumberPropertyModifier
	public class ApplyNumberPropertyModifier : FormatBasePropertyModifier<bool> {
		public ApplyNumberPropertyModifier(bool val)
			: base(val) {
		}
		public override void ModifyFormat(Model.IFormatBaseBatchUpdateable cell) {
			cell.ApplyNumberFormat = NewValue;
		}
		public override bool GetFormatPropertyValue(Model.IFormatBaseBatchUpdateable cell) {
			return cell.ApplyNumberFormat;
		}
		public override bool GetFormatPropertyValue(Model.FormatBase format) {
			return false ;
		}
	}
	#endregion
	#region ApplyAlignmentPropertyModifier
	public class ApplyAlignmentPropertyModifier : FormatBasePropertyModifier<bool> {
		public ApplyAlignmentPropertyModifier(bool val)
			: base(val) {
		}
		public override void ModifyFormat(Model.IFormatBaseBatchUpdateable cell) {
			cell.ApplyAlignment = NewValue;
		}
		public override bool GetFormatPropertyValue(Model.IFormatBaseBatchUpdateable cell) {
			return cell.ApplyAlignment;
		}
		public override bool GetFormatPropertyValue(Model.FormatBase format) {
			return false;
		}
	}
	#endregion
	#region ApplyFontPropertyModifier
	public class ApplyFontPropertyModifier : FormatBasePropertyModifier<bool> {
		public ApplyFontPropertyModifier(bool val)
			: base(val) {
		}
		public override void ModifyFormat(Model.IFormatBaseBatchUpdateable cell) {
			cell.ApplyFont = NewValue;
		}
		public override bool GetFormatPropertyValue(Model.IFormatBaseBatchUpdateable cell) {
			return cell.ApplyFont;
		}
		public override bool GetFormatPropertyValue(Model.FormatBase format) {
			return false;
		}
	}
#endregion
	#region ApplyBordersPropertyModifier
	public class ApplyBordersPropertyModifier : FormatBasePropertyModifier<bool> {
		public ApplyBordersPropertyModifier(bool val)
			: base(val) {
		}
		public override void ModifyFormat(Model.IFormatBaseBatchUpdateable cell) {
			cell.ApplyBorder = NewValue;
		}
		public override bool GetFormatPropertyValue(Model.IFormatBaseBatchUpdateable cell) {
			return cell.ApplyBorder;
		}
		public override bool GetFormatPropertyValue(Model.FormatBase format) {
			return false;
		}
	}
	#endregion
	#region ApplyFillPropertyModifier
	public class ApplyFillPropertyModifier : FormatBasePropertyModifier<bool> {
		public ApplyFillPropertyModifier(bool val)
			: base(val) {
		}
		public override void ModifyFormat(Model.IFormatBaseBatchUpdateable cell) {
			cell.ApplyFill = NewValue;
		}
		public override bool GetFormatPropertyValue(Model.IFormatBaseBatchUpdateable cell) {
			return cell.ApplyFill;
		}
		public override bool GetFormatPropertyValue(Model.FormatBase format) {
			return false;
		}
	}
	#endregion
	#region ApplyProtectionPropertyModifier
	public class ApplyProtectionPropertyModifier : FormatBasePropertyModifier<bool> {
		public ApplyProtectionPropertyModifier(bool val)
			: base(val) {
		}
		public override void ModifyFormat(Model.IFormatBaseBatchUpdateable cell) {
			cell.ApplyProtection = NewValue;
		}
		public override bool GetFormatPropertyValue(Model.IFormatBaseBatchUpdateable cell) {
			return cell.ApplyProtection;
		}
		public override bool GetFormatPropertyValue(Model.FormatBase format) {
			return false;
		}
	}
	#endregion
}
