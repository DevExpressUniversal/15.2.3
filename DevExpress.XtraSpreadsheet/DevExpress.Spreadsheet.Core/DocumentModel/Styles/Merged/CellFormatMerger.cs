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
using DevExpress.Utils;
#if !SL
using System.Drawing;
#else
using System.Windows.Media;
#endif
namespace DevExpress.XtraSpreadsheet.Model {
	#region CellFormatMerger
	public class CellFormatMerger {
		#region Fields
		MergedCellFormat mergedCellFormat;
		#endregion
		public CellFormatMerger() {
		}
		public CellFormatMerger(ICellFormat initialCellFormat)
			: this(new MergedCellFormat(initialCellFormat)) {
		}
		public CellFormatMerger(MergedCellFormat initialCellFormat) {
			Guard.ArgumentNotNull(initialCellFormat, "initialCellFormat");
			this.mergedCellFormat = initialCellFormat;
		}
		#region Properties
		public MergedCellFormat MergedCellFormat { get { return mergedCellFormat; } }
		#endregion
		protected internal void Merge(ICellFormat format) {
			if (mergedCellFormat == null)
				mergedCellFormat = new MergedCellFormat(format);
			MergeFormatString(format);
			MergeAlignment(format.ActualAlignment);
			MergeFont(format.ActualFont);
			MergeBorders(format.ActualBorder);
			MergeFill(format.ActualFill);
			MergeProtection(format.ActualProtection);
		}
		void MergeFormatString(IFormatStringAccessor owner) {
			MergedFormatStringInfo format = mergedCellFormat.FormatString;
			if (format.FormatString != null && StringExtensions.CompareInvariantCultureIgnoreCase(format.FormatString, owner.FormatString) != 0)
				format.FormatString = null;
		}
		void MergeAlignment(IActualCellAlignmentInfo owner) {
			MergedAlignmentInfo alignment = mergedCellFormat.Alignment;
			if (alignment.WrapText.HasValue && alignment.WrapText.Value != owner.WrapText)
				alignment.WrapText = null;
			if (alignment.JustifyLastLine.HasValue && alignment.JustifyLastLine.Value != owner.JustifyLastLine)
				alignment.JustifyLastLine = null;
			if (alignment.ShrinkToFit.HasValue && alignment.ShrinkToFit.Value != owner.ShrinkToFit)
				alignment.ShrinkToFit = null;
			if (alignment.TextRotation.HasValue && alignment.TextRotation.Value != owner.TextRotation)
				alignment.TextRotation = null;
			if (alignment.Indent.HasValue && alignment.Indent.Value != owner.Indent)
				alignment.Indent = null;
			if (alignment.RelativeIndent.HasValue && alignment.RelativeIndent.Value != owner.RelativeIndent)
				alignment.RelativeIndent = null;
			if (alignment.Horizontal.HasValue && alignment.Horizontal.Value != owner.Horizontal)
				alignment.Horizontal = null;
			if (alignment.Vertical.HasValue && alignment.Vertical.Value != owner.Vertical)
				alignment.Vertical = null;
			if (alignment.ReadingOrder.HasValue && alignment.ReadingOrder.Value != owner.ReadingOrder)
				alignment.ReadingOrder = null;
		}
		void MergeFont(IActualRunFontInfo owner) {
			MergedFontInfo font = mergedCellFormat.Font;
			MergeFontNameAndSize(owner, font);
			MergeFontStyle(owner, font);
			MergeFontEffects(owner, font);
			if (font.Color.HasValue && font.Color.Value != owner.Color)
				font.Color = null;
			if (font.Condense.HasValue && font.Condense.Value != owner.Condense)
				font.Condense = null;
			if (font.Extend.HasValue && font.Extend.Value != owner.Extend)
				font.Extend = null;
			if (font.Shadow.HasValue && font.Shadow.Value != owner.Shadow)
				font.Shadow = null;
			if (font.Charset.HasValue && font.Charset.Value != owner.Charset)
				font.Charset = null;
		}
		void MergeFontNameAndSize(IActualRunFontInfo owner, MergedFontInfo font) {
			if (font.Name != null && StringExtensions.CompareInvariantCultureIgnoreCase(font.Name, owner.Name) != 0)
				font.Name = null;
			if (font.FontFamily.HasValue && font.FontFamily.Value != owner.FontFamily)
				font.FontFamily = null;
			if (font.Size.HasValue && font.Size.Value != owner.Size)
				font.Size = null;
		}
		void MergeFontStyle(IActualRunFontInfo owner, MergedFontInfo font) {
			if (font.Bold.HasValue && font.Bold.Value != owner.Bold)
				font.Bold = null;
			if (font.Italic.HasValue && font.Italic.Value != owner.Italic)
				font.Italic = null;
			if (font.Outline.HasValue && font.Outline.Value != owner.Outline)
				font.Outline = null;
			if (font.Underline.HasValue && font.Underline.Value != owner.Underline)
				font.Underline = null;
			if (font.SchemeStyle.HasValue && font.SchemeStyle.Value != owner.SchemeStyle)
				font.SchemeStyle = null;
		}
		void MergeFontEffects(IActualRunFontInfo owner, MergedFontInfo font) {
			if (font.StrikeThrough.HasValue && font.StrikeThrough.Value != owner.StrikeThrough)
				font.StrikeThrough = null;
			if (font.Script.HasValue && font.Script.Value != owner.Script)
				font.Script = null;
		}
		void MergeBorders(IActualBorderInfo owner) {
			MergedBorderInfo border = mergedCellFormat.Border;
			if (border.Outline.HasValue && border.Outline.Value != owner.Outline)
				border.Outline = null;
			MergeLeftBorder(border, owner);
			MergeRightBorder(border, owner);
			MergeTopBorder(border, owner);
			MergeBottomBorder(border, owner);
			MergeDiagonalBorders(border, owner);
			MergeInsideHorizontalBorder(border, owner);
			MergeInsideVerticalBorder(border, owner);
		}
		void MergeLeftBorder(MergedBorderInfo border, IActualBorderInfo owner) {
			if (border.LeftLineStyle.HasValue && border.LeftLineStyle.Value != owner.LeftLineStyle)
				border.LeftLineStyle = null;
			if (border.LeftColor.HasValue && border.LeftColor.Value != owner.LeftColor)
				border.LeftColor = null;
			if (border.LeftColorIndex.HasValue && border.LeftColorIndex.Value != owner.LeftColorIndex)
				border.LeftColorIndex = null;
		}
		void MergeRightBorder(MergedBorderInfo border, IActualBorderInfo owner) {
			if (border.RightLineStyle.HasValue && border.RightLineStyle.Value != owner.RightLineStyle)
				border.RightLineStyle = null;
			if (border.RightColor.HasValue && border.RightColor.Value != owner.RightColor)
				border.RightColor = null;
			if (border.RightColorIndex.HasValue && border.RightColorIndex.Value != owner.RightColorIndex)
				border.RightColorIndex = null;
		}
		void MergeTopBorder(MergedBorderInfo border, IActualBorderInfo owner) {
			if (border.TopLineStyle.HasValue && border.TopLineStyle.Value != owner.TopLineStyle)
				border.TopLineStyle = null;
			if (border.TopColor.HasValue && border.TopColor.Value != owner.TopColor)
				border.TopColor = null;
			if (border.TopColorIndex.HasValue && border.TopColorIndex.Value != owner.TopColorIndex)
				border.TopColorIndex = null;
		}
		void MergeBottomBorder(MergedBorderInfo border, IActualBorderInfo owner) {
			if (border.BottomLineStyle.HasValue && border.BottomLineStyle.Value != owner.BottomLineStyle)
				border.BottomLineStyle = null;
			if (border.BottomColor.HasValue && border.BottomColor.Value != owner.BottomColor)
				border.BottomColor = null;
			if (border.BottomColorIndex.HasValue && border.BottomColorIndex.Value != owner.BottomColorIndex)
				border.BottomColorIndex = null;
		}
		void MergeDiagonalBorders(MergedBorderInfo border, IActualBorderInfo owner) {
			if (border.DiagonalUpLineStyle.HasValue && border.DiagonalUpLineStyle.Value != owner.DiagonalUpLineStyle)
				border.DiagonalUpLineStyle = null;
			if (border.DiagonalDownLineStyle.HasValue && border.DiagonalDownLineStyle.Value != owner.DiagonalDownLineStyle)
				border.DiagonalDownLineStyle = null;
			if (border.DiagonalColor.HasValue && border.DiagonalColor.Value != owner.DiagonalColor)
				border.DiagonalColor = null;
			if (border.DiagonalColorIndex.HasValue && border.DiagonalColorIndex.Value != owner.DiagonalColorIndex)
				border.DiagonalColorIndex = null;
		}
		void MergeInsideHorizontalBorder(MergedBorderInfo border, IActualBorderInfo owner) {
			if (border.HorizontalLineStyle.HasValue && border.HorizontalLineStyle.Value != owner.HorizontalLineStyle)
				border.HorizontalLineStyle = null;
			if (border.HorizontalColor.HasValue && border.HorizontalColor.Value != owner.HorizontalColor)
				border.HorizontalColor = null;
			if (border.HorizontalColorIndex.HasValue && border.HorizontalColorIndex.Value != owner.HorizontalColorIndex)
				border.HorizontalColorIndex = null;
		}
		void MergeInsideVerticalBorder(MergedBorderInfo border, IActualBorderInfo owner) {
			if (border.VerticalLineStyle.HasValue && border.VerticalLineStyle.Value != owner.VerticalLineStyle)
				border.VerticalLineStyle = null;
			if (border.VerticalColor.HasValue && border.VerticalColor.Value != owner.VerticalColor)
				border.VerticalColor = null;
			if (border.VerticalColorIndex.HasValue && border.VerticalColorIndex.Value != owner.VerticalColorIndex)
				border.VerticalColorIndex = null;
		}
		void MergeFill(IActualFillInfo owner) {
			MergeGradientFill(owner.GradientFill);
			MergedFillInfo fill = mergedCellFormat.Fill;
			if (fill.PatternType.HasValue && fill.PatternType.Value != owner.PatternType) {
				fill.PatternType = null;
				fill.ForeColor = null;
				fill.BackColor = null;
			}
			if (fill.ForeColor.HasValue && fill.ForeColor.Value != owner.ForeColor) 
				fill.ForeColor = null;
			if (fill.BackColor.HasValue && fill.BackColor.Value != owner.BackColor) 
				fill.BackColor = null;
			if (fill.FillType.HasValue && fill.FillType.Value != owner.FillType)
				fill.FillType = null;
		}
		void MergeGradientFill(IActualGradientFillInfo owner) {
			MergeConvergence(owner.Convergence);
			MergedGradientFill gradientFill = mergedCellFormat.Fill.GradientFill;
			if (gradientFill.Type.HasValue && gradientFill.Type.Value != owner.Type)
				gradientFill.Type = null;
			if (gradientFill.Degree.HasValue && gradientFill.Degree.Value != owner.Degree)
				gradientFill.Degree = null;
		}
		void MergeConvergence(IActualConvergenceInfo owner) {
			MergedConvergence convergence = mergedCellFormat.Fill.GradientFill.Convergence;
			if (convergence.Left.HasValue && convergence.Left.Value != owner.Left)
				convergence.Left = null;
			if (convergence.Right.HasValue && convergence.Right.Value != owner.Right)
				convergence.Right = null;
			if (convergence.Top.HasValue && convergence.Top.Value != owner.Top)
				convergence.Top = null;
			if (convergence.Bottom.HasValue && convergence.Bottom.Value != owner.Bottom)
				convergence.Bottom = null;
		}
		void MergeProtection(IActualCellProtectionInfo owner) {
			MergedCellProtectionInfo protection = mergedCellFormat.Protection;
			if (protection.Hidden.HasValue && protection.Hidden.Value != owner.Hidden)
				protection.Hidden = null;
			if (protection.Locked.HasValue && protection.Locked.Value != owner.Locked)
				protection.Locked = null;
		}
	}
	#endregion
}
