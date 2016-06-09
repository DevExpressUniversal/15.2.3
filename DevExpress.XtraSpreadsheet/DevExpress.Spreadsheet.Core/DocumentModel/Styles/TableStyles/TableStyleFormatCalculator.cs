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

using DevExpress.Export.Xl;
namespace DevExpress.XtraSpreadsheet.Model {
	#region TableStyleFormatBuilderFactory
	public static class TableStyleFormatBuilderFactory {
		#region Fields
		static TableStyleFormatPropertyValueBuilder propertyBuilder = new TableStyleFormatPropertyValueBuilder();
		static TableStyleFormatDisplayBorderValueBuilder displayBorderBuilder = new TableStyleFormatDisplayBorderValueBuilder();
		static TableStyleFormatApplyPropertyValueBuilder applyPropertyBuilder = new TableStyleFormatApplyPropertyValueBuilder();
		static ConvertToRangeBuilder convertToRangeBuilder = new ConvertToRangeBuilder();
		#endregion
		#region Properties
		public static TableStyleFormatPropertyValueBuilder PropertyBuilder { get { return propertyBuilder; } }
		public static TableStyleFormatDisplayBorderValueBuilder DisplayBorderBuilder { get { return displayBorderBuilder; } }
		public static TableStyleFormatApplyPropertyValueBuilder ApplyPropertyBuilder { get { return applyPropertyBuilder; } }
		public static ConvertToRangeBuilder ConvertToRangeBuilder { get { return convertToRangeBuilder; } }
		#endregion
	}
	#endregion
	#region TableStyleFormatPropertyValueBuilderBase<TDescriptor> (abstract class)
	public abstract class TableStyleFormatPropertyValueBuilderBase<TDescriptor> {
		public TValue Build<TValue>(TValue actualCellFormatValue, bool actualApplyValue, TDescriptor descriptor, CellPosition cellPosition, Worksheet sheet) {
			if (actualApplyValue)
				return actualCellFormatValue;
			ITableBase table = sheet.TryGetTableBase(cellPosition);
			if (table == null)
				return actualCellFormatValue;
			return Build(table, descriptor, cellPosition, actualCellFormatValue);
		}
		public abstract TValue Build<TValue>(ITableBase table, TDescriptor descriptor, CellPosition cellPosition, TValue defaultValue);
	}
	#endregion
	#region TableStyleFormatPropertyValueBuilder
	public class TableStyleFormatPropertyValueBuilder : TableStyleFormatPropertyValueBuilderBase<DifferentialFormatPropertyDescriptor> {
		public override TValue Build<TValue>(ITableBase table, DifferentialFormatPropertyDescriptor descriptor, CellPosition cellPosition, TValue defaultValue) {
			ActualTableStyleCellFormatting formatting = table.GetActualCellFormatting(cellPosition);
			IDifferentialFormatPropertyAccessor<TValue> accessor = DifferentialFormatPropertyAccessors.GetPropertyBaseAccessor(descriptor) as IDifferentialFormatPropertyAccessor<TValue>;
			return formatting.CalculateCellFormatValue(table.WholeRange.Worksheet.Workbook.StyleSheet.Workbook.Cache.CellFormatCache, accessor, defaultValue);
		}
	}
	#endregion
	#region TableStyleFormatDisplayBorderValueBuilder
	public class TableStyleFormatDisplayBorderValueBuilder : TableStyleFormatPropertyValueBuilderBase<DifferentialFormatDisplayBorderDescriptor> {
		public override TValue Build<TValue>(ITableBase table, DifferentialFormatDisplayBorderDescriptor descriptor, CellPosition cellPosition, TValue defaultValue) {
			IDifferentialFormatDisplayBorderPropertyAccessor<TValue> targetAccessor = DifferentialFormatPropertyAccessors.GetDisplayBorderBaseAccessor(descriptor) as IDifferentialFormatDisplayBorderPropertyAccessor<TValue>;
			CellRangeBase wholeRange = table.WholeRange;
			ICellTable cellTable = wholeRange.Worksheet;
			CellPosition nearbyPosition = cellPosition.GetShiftedAny(targetAccessor.NearbyOffset, cellTable);
			bool checkValidNearbyPosition = !nearbyPosition.Equals(CellPosition.InvalidValue) && wholeRange.ContainsCell(nearbyPosition.Column, nearbyPosition.Row);
			CellFormatCache cache = cellTable.Workbook.StyleSheet.Workbook.Cache.CellFormatCache;
			ActualTableStyleCellFormatting targetFormatting = table.GetActualCellFormatting(cellPosition);
			if (targetFormatting.IsEmpty) {
				if (checkValidNearbyPosition) {
					ActualTableStyleCellFormatting nearbyFormatting = table.GetActualCellFormatting(nearbyPosition);
					if (!nearbyFormatting.IsEmpty) {
						int nearbyOrder;
						return nearbyFormatting.CalculateCellFormatValue(cache, targetAccessor.NearbyAccessor, defaultValue, out nearbyOrder);
					}
				}
				return defaultValue;
			}
			int targetOrder;
			TValue targetValue = targetFormatting.CalculateCellFormatValue(cache, targetAccessor, defaultValue, out targetOrder);
			if (checkValidNearbyPosition) {
				ActualTableStyleCellFormatting nearbyFormatting = table.GetActualCellFormatting(nearbyPosition);
				if (!nearbyFormatting.IsEmpty) {
					int nearbyOrder;
					TValue nerbyValue = nearbyFormatting.CalculateCellFormatValue(cache, targetAccessor.NearbyAccessor, defaultValue, out nearbyOrder);
					bool defaultTargetOrder = targetOrder == TableStyle.ElementsCount;
					bool defaultNearbyOrder = nearbyOrder == TableStyle.ElementsCount;
					if (defaultNearbyOrder && !defaultTargetOrder)
						return targetValue;
					if (!defaultNearbyOrder && defaultTargetOrder)
						return nerbyValue;
					if (!defaultNearbyOrder && !defaultTargetOrder)
						return targetOrder <= nearbyOrder ? targetValue : nerbyValue;
				}
			}
			return targetValue;
		}
	}
	#endregion
	#region TableStyleFormatApplyPropertyValueBuilder
	public class TableStyleFormatApplyPropertyValueBuilder {
		public bool Build(bool actualApplyValue, DifferentialFormatPropertyDescriptor descriptor, CellPosition cellPosition, Worksheet sheet) {
			if (actualApplyValue)
				return true;
			ITableBase table = sheet.TryGetTableBase(cellPosition);
			if (table == null)
				return false;
			return Build(table, descriptor, cellPosition);
		}
		public bool Build(ITableBase table, DifferentialFormatPropertyDescriptor descriptor, CellPosition cellPosition) {
			ActualTableStyleCellFormatting formatting = table.GetActualCellFormatting(cellPosition);
			IDifferentialFormatPropertyAccessorBase accessor = DifferentialFormatPropertyAccessors.GetPropertyBaseAccessor(descriptor);
			return formatting.CheckApplyCellFormatValue(table.WholeRange.Worksheet.Workbook.StyleSheet.Workbook.Cache.CellFormatCache, accessor);
		}
	}
	#endregion
	#region ConvertToRangeBuilder
	public class ConvertToRangeBuilder {
		public CellFormat Build(ITableBase table, CellPosition cellPosition, CellFormat defaultFormat) {
			DocumentCache cache = defaultFormat.DocumentModel.Cache;
			CellFormat cellFormat = defaultFormat.Clone();
			CellFormatFlagsInfo flags = cellFormat.CellFormatFlagsInfo;
			int alignmentIndex = cache.CellAlignmentInfoCache.AddItem(GetActualAlignmentInfo(table, cellPosition, defaultFormat.AlignmentInfo));
			cellFormat.AssignAlignmentIndex(alignmentIndex);
			flags.ApplyAlignment = defaultFormat.AlignmentIndex != alignmentIndex;
			int borderIndex = cache.BorderInfoCache.AddItem(GetActualDisplayBorderInfo(table, cellPosition, cellFormat.BorderInfo));
			cellFormat.AssignBorderIndex(borderIndex);
			flags.ApplyBorder = defaultFormat.BorderIndex != borderIndex;
			int fontIndex = cache.FontInfoCache.AddItem(GetActualFontInfo(table, cellPosition, cellFormat.FontInfo));
			cellFormat.AssignFontIndex(fontIndex);
			flags.ApplyFont = defaultFormat.FontIndex != fontIndex;
			int numberIndex = cache.NumberFormatCache.AddItem(GetActualNumberFormat(table, cellPosition, defaultFormat.NumberFormatIndex, defaultFormat.FormatString));
			cellFormat.AssignNumberFormatIndex(numberIndex);
			flags.ApplyNumberFormat = defaultFormat.NumberFormatIndex != numberIndex;
			ModelFillType fillType = GetActualFormatValue(table, DifferentialFormatPropertyDescriptor.FillType, cellPosition, ModelFillType.Pattern);
			if (fillType == ModelFillType.Pattern) {
				int patternFillIndex = cache.FillInfoCache.AddItem(GetActualPatternFillInfo(table, cellPosition, cellFormat.FillInfo));
				cellFormat.AssignFillIndex(patternFillIndex);
				flags.ApplyFill = defaultFormat.FillIndex != patternFillIndex;
			} else {
				int gradientFillIndex = cache.GradientFillInfoCache.AddItem(GetActualGradientFillInfo(table, cellPosition, cellFormat.GradientFillInfo));
				cellFormat.AssignGradientFillInfoIndex(gradientFillIndex);
				cellFormat.AssignGradientStopInfoCollection(GetActualGradientStopInfoCollection(table, cellPosition, cellFormat.GradientStopInfoCollection));
				flags.ApplyFill = true;
			}
			flags.FillType = fillType;
			bool defaultHidden = defaultFormat.Protection.Hidden;
			bool defaultLocked = defaultFormat.Protection.Locked;
			flags.Hidden = GetActualFormatValue(table, DifferentialFormatPropertyDescriptor.ProtectionHidden, cellPosition, defaultHidden);
			flags.Locked = GetActualFormatValue(table, DifferentialFormatPropertyDescriptor.ProtectionLocked, cellPosition, defaultLocked);
			flags.ApplyProtection = defaultHidden != flags.Hidden || defaultLocked != flags.Locked;
			cellFormat.AssignCellFormatFlagsIndex(flags.PackedValues);
			return cellFormat;
		}
		#region Internal
		T GetActualFormatValue<T>(ITableBase table, DifferentialFormatPropertyDescriptor descriptor, CellPosition cellPosition, T defaultValue) {
			return TableStyleFormatBuilderFactory.PropertyBuilder.Build(table, descriptor, cellPosition, defaultValue);
		}
		T GetActualDisplayBorderValue<T>(ITableBase table, DifferentialFormatDisplayBorderDescriptor descriptor, CellPosition cellPosition, T defaultValue) {
			return TableStyleFormatBuilderFactory.DisplayBorderBuilder.Build(table, descriptor, cellPosition, defaultValue);
		}
		CellAlignmentInfo GetActualAlignmentInfo(ITableBase table, CellPosition cellPosition, CellAlignmentInfo defaultItem) {
			CellAlignmentInfo info = defaultItem.Clone();
			info.HorizontalAlignment = GetActualFormatValue(table, DifferentialFormatPropertyDescriptor.AlignmentHorizontal, cellPosition, info.HorizontalAlignment);
			info.VerticalAlignment = GetActualFormatValue(table, DifferentialFormatPropertyDescriptor.AlignmentVertical, cellPosition, info.VerticalAlignment);
			info.WrapText = GetActualFormatValue(table, DifferentialFormatPropertyDescriptor.AlignmentWrapText, cellPosition, info.WrapText);
			info.JustifyLastLine = GetActualFormatValue(table, DifferentialFormatPropertyDescriptor.AlignmentJustifyLastLine, cellPosition, info.JustifyLastLine);
			info.ShrinkToFit = GetActualFormatValue(table, DifferentialFormatPropertyDescriptor.AlignmentShrinkToFit, cellPosition, info.ShrinkToFit);
			info.ReadingOrder = GetActualFormatValue(table, DifferentialFormatPropertyDescriptor.AlignmentReadingOrder, cellPosition, info.ReadingOrder);
			info.TextRotation = GetActualFormatValue(table, DifferentialFormatPropertyDescriptor.AlignmentTextRotation, cellPosition, info.TextRotation);
			info.Indent = GetActualFormatValue(table, DifferentialFormatPropertyDescriptor.AlignmentIndent, cellPosition, info.Indent);
			info.RelativeIndent = GetActualFormatValue(table, DifferentialFormatPropertyDescriptor.AlignmentRelativeIndent, cellPosition, info.RelativeIndent);
			return info;
		}
		FillInfo GetActualPatternFillInfo(ITableBase table, CellPosition cellPosition, FillInfo defaultItem) {
			FillInfo info = defaultItem.Clone();
			info.PatternType = GetActualFormatValue(table, DifferentialFormatPropertyDescriptor.FillPatternType, cellPosition, info.PatternType);
			info.BackColorIndex = GetActualFormatValue(table, DifferentialFormatPropertyDescriptor.FillBackColorIndex, cellPosition, info.BackColorIndex);
			info.ForeColorIndex = GetActualFormatValue(table, DifferentialFormatPropertyDescriptor.FillForeColorIndex, cellPosition, info.ForeColorIndex);
			return info;
		}
		GradientFillInfo GetActualGradientFillInfo(ITableBase table, CellPosition cellPosition, GradientFillInfo defaultItem) {
			GradientFillInfo info = defaultItem.Clone();
			info.Type = GetActualFormatValue(table, DifferentialFormatPropertyDescriptor.FillGradientFillType, cellPosition, info.Type);
			info.Degree = GetActualFormatValue(table, DifferentialFormatPropertyDescriptor.FillGradientFillDegree, cellPosition, info.Degree);
			info.Left = GetActualFormatValue(table, DifferentialFormatPropertyDescriptor.FillGradientFillCovergenceLeft, cellPosition, info.Left);
			info.Right = GetActualFormatValue(table, DifferentialFormatPropertyDescriptor.FillGradientFillCovergenceRight, cellPosition, info.Right);
			info.Top = GetActualFormatValue(table, DifferentialFormatPropertyDescriptor.FillGradientFillCovergenceTop, cellPosition, info.Top);
			info.Bottom = GetActualFormatValue(table, DifferentialFormatPropertyDescriptor.FillGradientFillCovergenceBottom, cellPosition, info.Bottom);
			return info;
		}
		GradientStopInfoCollection GetActualGradientStopInfoCollection(ITableBase table, CellPosition cellPosition, GradientStopInfoCollection defaultItem) {
			GradientStopInfoCollection info = defaultItem.Clone() as GradientStopInfoCollection;
			return GetActualFormatValue(table, DifferentialFormatPropertyDescriptor.FillGradientFillGradientStops, cellPosition, info);
		}
		RunFontInfo GetActualFontInfo(ITableBase table, CellPosition cellPosition, RunFontInfo defaultItem) {
			RunFontInfo info = defaultItem.Clone();
			info.Name = GetActualFormatValue(table, DifferentialFormatPropertyDescriptor.FontName, cellPosition, info.Name);
			info.ColorIndex = GetActualFormatValue(table, DifferentialFormatPropertyDescriptor.FontColorIndex, cellPosition, info.ColorIndex);
			info.Bold = GetActualFormatValue(table, DifferentialFormatPropertyDescriptor.FontBold, cellPosition, info.Bold);
			info.Condense = GetActualFormatValue(table, DifferentialFormatPropertyDescriptor.FontCondense, cellPosition, info.Condense);
			info.Extend = GetActualFormatValue(table, DifferentialFormatPropertyDescriptor.FontExtend, cellPosition, info.Extend);
			info.Italic = GetActualFormatValue(table, DifferentialFormatPropertyDescriptor.FontItalic, cellPosition, info.Italic);
			info.Outline = GetActualFormatValue(table, DifferentialFormatPropertyDescriptor.FontOutline, cellPosition, info.Outline);
			info.Shadow = GetActualFormatValue(table, DifferentialFormatPropertyDescriptor.FontShadow, cellPosition, info.Shadow);
			info.StrikeThrough = GetActualFormatValue(table, DifferentialFormatPropertyDescriptor.FontStrikeThrough, cellPosition, info.StrikeThrough);
			info.Charset = GetActualFormatValue(table, DifferentialFormatPropertyDescriptor.FontCharset, cellPosition, info.Charset);
			info.FontFamily = GetActualFormatValue(table, DifferentialFormatPropertyDescriptor.FontFamily, cellPosition, info.FontFamily);
			info.Size = GetActualFormatValue(table, DifferentialFormatPropertyDescriptor.FontSize, cellPosition, info.Size);
			info.SchemeStyle = GetActualFormatValue(table, DifferentialFormatPropertyDescriptor.FontSchemeStyle, cellPosition, info.SchemeStyle);
			info.Script = GetActualFormatValue(table, DifferentialFormatPropertyDescriptor.FontScript, cellPosition, info.Script);
			info.Underline = GetActualFormatValue(table, DifferentialFormatPropertyDescriptor.FontUnderline, cellPosition, info.Underline);
			return info;
		}
		BorderInfo GetActualDisplayBorderInfo(ITableBase table, CellPosition cellPosition, BorderInfo defaultInfo) {
			BorderInfo info = defaultInfo.Clone();
			info.LeftLineStyle = GetActualDisplayBorderValue(table, DifferentialFormatDisplayBorderDescriptor.LeftLineStyle, cellPosition, info.LeftLineStyle);
			info.RightLineStyle = GetActualDisplayBorderValue(table, DifferentialFormatDisplayBorderDescriptor.RightLineStyle, cellPosition, info.RightLineStyle);
			info.TopLineStyle = GetActualDisplayBorderValue(table, DifferentialFormatDisplayBorderDescriptor.TopLineStyle, cellPosition, info.TopLineStyle);
			info.BottomLineStyle = GetActualDisplayBorderValue(table, DifferentialFormatDisplayBorderDescriptor.BottomLineStyle, cellPosition, info.BottomLineStyle);
			info.LeftColorIndex = GetActualDisplayBorderValue(table, DifferentialFormatDisplayBorderDescriptor.LeftColorIndex, cellPosition, info.LeftColorIndex);
			info.RightColorIndex = GetActualDisplayBorderValue(table, DifferentialFormatDisplayBorderDescriptor.RightColorIndex, cellPosition, info.RightColorIndex);
			info.TopColorIndex = GetActualDisplayBorderValue(table, DifferentialFormatDisplayBorderDescriptor.TopColorIndex, cellPosition, info.TopColorIndex);
			info.BottomColorIndex = GetActualDisplayBorderValue(table, DifferentialFormatDisplayBorderDescriptor.BottomColorIndex, cellPosition, info.BottomColorIndex);
			return info;
		}
		int GetActualCellFormatFlagsInfoIndex(ITableBase table, CellPosition cellPosition, CellFormatFlagsInfo defaultInfo, IActualApplyInfo actualApplyInfo) {
			CellFormatFlagsInfo info = defaultInfo.Clone();
			if (actualApplyInfo.ApplyFill)
				info.FillType = GetActualFormatValue(table, DifferentialFormatPropertyDescriptor.FillType, cellPosition, info.FillType);
			if (actualApplyInfo.ApplyProtection) {
				info.Hidden = GetActualFormatValue(table, DifferentialFormatPropertyDescriptor.ProtectionHidden, cellPosition, info.Hidden);
				info.Locked = GetActualFormatValue(table, DifferentialFormatPropertyDescriptor.ProtectionLocked, cellPosition, info.Locked);
			}
			return info.PackedValues;
		}
		NumberFormat GetActualNumberFormat(ITableBase table, CellPosition cellPosition, int defaultNumberFormatIndex, string defaultFormatCode) {
			int id = GetActualFormatValue(table, DifferentialFormatPropertyDescriptor.NumberFormatIndex, cellPosition, defaultNumberFormatIndex);
			string formatCode = GetActualFormatValue(table, DifferentialFormatPropertyDescriptor.NumberFormat, cellPosition, defaultFormatCode);
			return new NumberFormat(id, formatCode);
		}
		#endregion
	}
	#endregion
} 
