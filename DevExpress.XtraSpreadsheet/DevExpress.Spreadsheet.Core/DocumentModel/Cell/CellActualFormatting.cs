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

using System.Drawing;
using DevExpress.Utils;
using DevExpress.XtraSpreadsheet.Layout;
using DevExpress.Export.Xl;
using DevExpress.Compatibility.System.Drawing;
using DevExpress.Office.Model;
#if SL
using System.Windows.Media;
#endif
namespace DevExpress.XtraSpreadsheet.Model {
	public partial class Cell {
		#region ConditionalFormatting support members
		#region Properties
		public int ConditionalFormattingStoppedAtPriority {
			get {
				if (conditionalFormatAccumulator != null)
					return conditionalFormatAccumulator.StoppedAtPriority;
				return 0;
			}
		}
		#endregion
		static ConditionalFormattingFormatAccumulator GetCondFmtAccumulator(Worksheet sheet, CellKey key) {
			if (sheet.ConditionalFormattings.CheckCell(key))
				return new ConditionalFormattingFormatAccumulator();
			return null;
		}
		#endregion
		#region ActualFormatString members
		public string ActualFormatString { get { return GetActualFormatStringValue(); } }
		#region GetActualFormatStringValue
		protected virtual string GetActualFormatStringValue() {
			return GetActualFormatValue(FormatInfo.ActualFormatString, ActualApplyInfo.ApplyNumberFormat, DifferentialFormatPropertyDescriptor.NumberFormat);
		}
		#endregion
		#endregion
		#region ActualFormat
		public NumberFormat ActualFormat { get { return Workbook.Cache.NumberFormatCache[ActualFormatIndex]; } }
		public int ActualFormatIndex { get { return GetActualFormatIndexValue(); } }
		protected virtual int GetActualFormatIndexValue() {
			if (conditionalFormatAccumulator != null) {
				conditionalFormatAccumulator.Update(this);
				if (conditionalFormatAccumulator.NumberFormatModified)
					return conditionalFormatAccumulator.NumberFormatIndex;
			}
			return GetActualFormatValue(FormatInfo.ActualFormatIndex, ActualApplyInfo.ApplyNumberFormat, DifferentialFormatPropertyDescriptor.NumberFormatIndex);
		}
		#endregion
		protected Color GetRgbColor(int colorIndex) {
			return GetColorModelInfo(colorIndex).ToRgb(DocumentModel.StyleSheet.Palette, DocumentModel.OfficeTheme.Colors);
		}
		ColorModelInfo GetColorModelInfo(int colorIndex) {
			return DocumentModel.Cache.ColorModelInfoCache[colorIndex];
		}
		#region GetActualFormatValue
		protected T GetActualFormatValue<T>(T cellActualValue, bool actualApply, DifferentialFormatPropertyDescriptor propertyDescriptor) {
			return TableStyleFormatBuilderFactory.PropertyBuilder.Build(cellActualValue, actualApply, propertyDescriptor, Position, Worksheet);
		}
		#endregion
		#region GetActualApplyFormatValue
		protected bool GetActualApplyFormatValue(bool actualApplyInfoValue, DifferentialFormatPropertyDescriptor propertyDescriptor) {
			return TableStyleFormatBuilderFactory.ApplyPropertyBuilder.Build(actualApplyInfoValue, propertyDescriptor, Position, Worksheet);
		}
		#endregion
		public bool HasVisibleFill {
			get {
				if (conditionalFormatAccumulator != null) {
					conditionalFormatAccumulator.Update(this);
					if (conditionalFormatAccumulator.HasVisibleFill(DocumentModel))
						return true;
				}
				return HasVisibleFillCore(ActualFill);
			}
		}
		protected virtual bool HasVisibleFillCore(IActualFillInfo actualFill) {
			if (FormatInfo.HasVisibleActualFill)
				return true;
			return TableStyleFormatBuilderFactory.ApplyPropertyBuilder.Build(false, DifferentialFormatPropertyDescriptor.HasVisibleFill, Position, Worksheet);
		}
		public XlPatternType ActualPatternType {
			get {
				if (conditionalFormatAccumulator != null) {
					conditionalFormatAccumulator.Update(this);
					if (conditionalFormatAccumulator.ColorScaleFillModified)
						return XlPatternType.Solid;
					if (conditionalFormatAccumulator.PatternFillAssigned) {
						FillInfo fillInfo = Workbook.Cache.FillInfoCache[conditionalFormatAccumulator.PatternFillIndex];
						if (fillInfo.BackColorIndex != ColorModelInfoCache.DefaultItemIndex) 
							return fillInfo.PatternType;
					}
				}
				return ActualFill.PatternType;
			}
		}
		public Color ActualBackgroundColor {
			get {
				if (conditionalFormatAccumulator != null) {
					conditionalFormatAccumulator.Update(this);
					if (conditionalFormatAccumulator.ColorScaleFillModified)
						return conditionalFormatAccumulator.ColorScaleFill;
					if (conditionalFormatAccumulator.PatternFillAssigned) {
						FillInfo fillInfo = Workbook.Cache.FillInfoCache[conditionalFormatAccumulator.PatternFillIndex];
						if (fillInfo.BackColorIndex != ColorModelInfoCache.DefaultItemIndex) 
							return GetRgbColor(fillInfo.BackColorIndex);
					}
				}
				return Cell.GetBackgroundColor(ActualFill);
			}
		}
		public Color ActualForegroundColor {
			get {
				if (conditionalFormatAccumulator != null) {
					conditionalFormatAccumulator.Update(this);
					if (conditionalFormatAccumulator.PatternFillAssigned) {
						FillInfo fillInfo = Workbook.Cache.FillInfoCache[conditionalFormatAccumulator.PatternFillIndex];
						if (fillInfo.ForeColorIndex != ColorModelInfoCache.DefaultItemIndex) 
							return GetRgbColor(fillInfo.ForeColorIndex);
					}
				}
				return Cell.GetForegroundColor(ActualFill);
			}
		}
		public override bool IsVisible() {
			if (HasFormula || !Value.IsEmpty)
				return true;
			if (HasVisibleFill)
				return true;
			if (!IsBorderEmpty(ActualBorder))
				return true;
			return false;
		}
		internal static bool IsLeftBorderEmpty(IActualBorderInfo border) {
			return (border.LeftLineStyle == XlBorderLineStyle.None);
		}
		internal static bool IsRightBorderEmpty(IActualBorderInfo border) {
			return (border.RightLineStyle == XlBorderLineStyle.None);
		}
		internal static bool IsTopBorderEmpty(IActualBorderInfo border) {
			return (border.TopLineStyle == XlBorderLineStyle.None);
		}
		internal static bool IsBottomBorderEmpty(IActualBorderInfo border) {
			return (border.BottomLineStyle == XlBorderLineStyle.None);
		}
		internal static bool IsBorderEmpty(IActualBorderInfo border) {
			if (!IsLeftBorderEmpty(border) || !IsRightBorderEmpty(border) || !IsTopBorderEmpty(border) || !IsBottomBorderEmpty(border))
				return false;
			if (border.HorizontalLineStyle != XlBorderLineStyle.None)
				return false;
			if (border.VerticalLineStyle != XlBorderLineStyle.None)
				return false;
			if (border.DiagonalUpLineStyle != XlBorderLineStyle.None)
				return false;
			if (border.DiagonalDownLineStyle != XlBorderLineStyle.None)
				return false;
			return true;
		}
	}
}
