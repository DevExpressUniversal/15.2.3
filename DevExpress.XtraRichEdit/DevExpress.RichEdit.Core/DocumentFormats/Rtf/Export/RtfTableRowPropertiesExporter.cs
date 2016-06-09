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
using DevExpress.Utils;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.Office;
using DevExpress.Office.Utils;
using DevExpress.Compatibility.System.Drawing;
using Debug = System.Diagnostics.Debug;
#if !SL
using System.Drawing;
using System.Diagnostics;
#else
using System.Windows.Media;
#endif
namespace DevExpress.XtraRichEdit.Export.Rtf {
	#region RtfTableRowPropertiesExporter
	class RtfTableRowPropertiesExporter : RtfPropertiesExporter {
		public RtfTableRowPropertiesExporter(DocumentModel documentModel, IRtfExportHelper rtfExportHelper, RtfBuilder rtfBuilder)
			: base(documentModel, rtfExportHelper, rtfBuilder) {
		}
		#region WriteLastRowMark
		internal void WriteLastRowMark() {
			RtfBuilder.WriteCommand(RtfExportSR.TableLastRow);
		}
		#endregion
		#region WriteHalfSpaceBetweenCells
		internal void WriteHalfSpaceBetweenCells(int val) {
			if (val > 0)
				RtfBuilder.WriteCommand(RtfExportSR.TableHalfSpaceBetweenCells, val);
		}
		#endregion
		#region WriteRowAlignment
		internal void WriteRowAlignment(TableRowAlignment value) {
			TableRowGeneralSettingsInfo defaultGeneralSettings = DocumentModel.Cache.TableRowGeneralSettingsInfoCache.DefaultItem;
			if (value == defaultGeneralSettings.TableRowAlignment)
				return;
			switch (value) {
				case TableRowAlignment.Center:
					RtfBuilder.WriteCommand(RtfExportSR.TableRowCenterAlignment);
					break;
				case TableRowAlignment.Left:
					RtfBuilder.WriteCommand(RtfExportSR.TableRowLeftAlignment);
					break;
				case TableRowAlignment.Right:
					RtfBuilder.WriteCommand(RtfExportSR.TableRowRightAlignment);
					break;
				default:
					break;
			}
		}
		#endregion
		#region WriteRowHeaderAndCantSplit
		internal void WriteRowHeader(bool header) {
			TableRowGeneralSettingsInfo defaultGeneralSettings = DocumentModel.Cache.TableRowGeneralSettingsInfoCache.DefaultItem;
			if (header != defaultGeneralSettings.Header)
				WriteBoolCommand(RtfExportSR.TableRowHeader, header);
		}
		internal void WriteRowCantSplit(bool cantSplit) {
			TableRowGeneralSettingsInfo defaultGeneralSettings = DocumentModel.Cache.TableRowGeneralSettingsInfoCache.DefaultItem;
			if (cantSplit != defaultGeneralSettings.CantSplit)
				WriteBoolCommand(RtfExportSR.TableRowCantSplit, cantSplit);
		}
		#endregion
		#region WriteRowHeight
		internal void WriteRowHeight(HeightUnitInfo height) {
			HeightUnitInfo defaultHeight = DocumentModel.Cache.HeightUnitInfoCache.DefaultItem;
			if (height.Type == defaultHeight.Type)
				return;
			Debug.Assert(height.Value >= 0);
			switch (height.Type) {
				case HeightUnitType.Auto:
					RtfBuilder.WriteCommand(RtfExportSR.TableRowHeight, 0);
					break;
				case HeightUnitType.Minimum:
					RtfBuilder.WriteCommand(RtfExportSR.TableRowHeight, UnitConverter.ModelUnitsToTwips(height.Value));
					break;
				case HeightUnitType.Exact:
					RtfBuilder.WriteCommand(RtfExportSR.TableRowHeight, -UnitConverter.ModelUnitsToTwips(height.Value));
					break;
			}
		}
		#endregion
		#region WriteWidthBefore
		internal void WriteWidthBefore(WidthUnitInfo widthBefore) {
			WriteWidthUnit(widthBefore, RtfExportSR.TableRowWidthBeforeType, RtfExportSR.TableRowWidthBefore);
		}
		#endregion
		#region WriteWidthAfter
		internal void WriteWidthAfter(WidthUnitInfo widthAfter) {
			WriteWidthUnit(widthAfter, RtfExportSR.TableRowWidthAfterType, RtfExportSR.TableRowWidthAfter);
		}
		#endregion
		#region WriteRowCellSpacing
		internal void WriteRowCellSpacing(WidthUnitInfo cellSpacing) {
			WriteWidthUnitInTwips(cellSpacing, RtfExportSR.TableCellSpacingLeftType, RtfExportSR.TableCellSpacingLeft);
			WriteWidthUnitInTwips(cellSpacing, RtfExportSR.TableCellSpacingBottomType, RtfExportSR.TableCellSpacingBottom);
			WriteWidthUnitInTwips(cellSpacing, RtfExportSR.TableCellSpacingRightType, RtfExportSR.TableCellSpacingRight);
			WriteWidthUnitInTwips(cellSpacing, RtfExportSR.TableCellSpacingTopType, RtfExportSR.TableCellSpacingTop);
		}
		#endregion
	}
	#endregion
	#region RtfTableCellPropertiesExporter
	class RtfTableCellPropertiesExporter : RtfPropertiesExporter {
		public RtfTableCellPropertiesExporter(DocumentModel documentModel, IRtfExportHelper rtfExportHelper, RtfBuilder rtfBuilder)
			: base(documentModel, rtfExportHelper, rtfBuilder) {
		}
		#region Properties
		protected virtual string TableCellBackgroundColor { get { return RtfExportSR.TableCellBackgroundColor; } }
		protected virtual string TableCellForegroundColor { get { return RtfExportSR.TableCellForegroundColor; } }
		protected virtual string TableCellShading { get { return RtfExportSR.TableCellShading; } }
		protected virtual string TableCellNoWrap { get { return RtfExportSR.TableCellNoWrap; } }
		protected virtual string TableCellTextTopAlignment { get { return RtfExportSR.TableCellTextTopAlignment; } }
		protected virtual string TableCellTextCenterAlignment { get { return RtfExportSR.TableCellTextCenterAlignment; } }
		protected virtual string TableCellTextBottomAlignment { get { return RtfExportSR.TableCellTextBottomAlignment; } }
		protected virtual string TableCellUpperLeftToLowerRightBorder { get { return RtfExportSR.TableCellUpperLeftToLowerRightBorder; } }
		protected virtual string TableCellUpperRightToLowerLeftBorder { get { return RtfExportSR.TableCellUpperRightToLowerLeftBorder; } }
		protected virtual string CellTopBorder { get { return RtfExportSR.TableCellTopBorder; } }
		protected virtual string CellLeftBorder { get { return RtfExportSR.TableCellLeftBorder; } }
		protected virtual string CellBottomBorder { get { return RtfExportSR.TableCellBottomBorder; } }
		protected virtual string CellRightBorder { get { return RtfExportSR.TableCellRightBorder; } }
		#endregion
		#region WriteCellGeneralSettings
		internal void WriteCellBackgroundColor(TableCell cell) {
			WriteCellBackgroundColor(cell.BackgroundColor, cell.Table);
		}
		internal void WriteCellBackgroundColor(Color color) {
			WriteCellBackgroundColor(color, null);
		}
		internal void WriteCellForegroundColor(TableCell cell) {
			WriteCellForegroundColor(cell.ForegroundColor, cell.Table);
		}
		internal void WriteCellForegroundColor(Color color) {
			WriteCellBackgroundColor(color, null);
		}
		internal void WriteCellShading(TableCell cell) {
			WriteCellShading(cell.Shading);
		}
		internal void WriteCellBackgroundColor(Color color, Table table) {
			TableCellGeneralSettingsInfo defaultGeneralSetting = DocumentModel.Cache.TableCellGeneralSettingsInfoCache.DefaultItem;
			if (color != defaultGeneralSetting.BackgroundColor && color != DXColor.Empty && color != DXColor.Transparent)
				color = RtfExportHelper.BlendColor(color);
			else if (table != null && table.BackgroundColor != defaultGeneralSetting.BackgroundColor && table.BackgroundColor != DXColor.Empty && table.BackgroundColor != DXColor.Transparent)
				color = RtfExportHelper.BlendColor(table.BackgroundColor);
			else
				return;
			int colorIndex = RtfExportHelper.GetColorIndex(color);
			RtfBuilder.WriteCommand(TableCellBackgroundColor, colorIndex);
		}
		internal void WriteCellForegroundColor(Color color, Table table) {
			TableCellGeneralSettingsInfo defaultGeneralSetting = DocumentModel.Cache.TableCellGeneralSettingsInfoCache.DefaultItem;
			if (color != defaultGeneralSetting.ForegroundColor && color != DXColor.Empty && color != DXColor.Transparent)
				color = RtfExportHelper.BlendColor(color);
			else
				return;
			int colorIndex = RtfExportHelper.GetColorIndex(color);
			RtfBuilder.WriteCommand(TableCellForegroundColor, colorIndex);
		}
		internal void WriteCellShading(ShadingPattern shading) {
			TableCellGeneralSettingsInfo defaultGeneralSetting = DocumentModel.Cache.TableCellGeneralSettingsInfoCache.DefaultItem;
			if (shading == defaultGeneralSetting.ShadingPattern)
				return;
			int value = DevExpress.XtraRichEdit.Import.Rtf.DestinationPieceTable.GetShadingValue(shading);
			if (value == 0)
				return;
			RtfBuilder.WriteCommand(TableCellShading, value);
		}
		internal void WriteCellFitText(bool fitText) {
			TableCellGeneralSettingsInfo defaultGeneralSetting = DocumentModel.Cache.TableCellGeneralSettingsInfoCache.DefaultItem;
			if (fitText != defaultGeneralSetting.FitText)
				WriteBoolCommand(RtfExportSR.TableCellFitText, fitText);
		}
		internal void WriteCellNoWrap(bool noWrap) {
			TableCellGeneralSettingsInfo defaultGeneralSetting = DocumentModel.Cache.TableCellGeneralSettingsInfoCache.DefaultItem;
			if (noWrap != defaultGeneralSetting.NoWrap)
				WriteBoolCommand(TableCellNoWrap, noWrap);
		}
		internal void WriteCellHideCellMark(bool hideCellMark) {
			TableCellGeneralSettingsInfo defaultGeneralSetting = DocumentModel.Cache.TableCellGeneralSettingsInfoCache.DefaultItem;
			if (hideCellMark != defaultGeneralSetting.HideCellMark)
				WriteBoolCommand(RtfExportSR.TableCellHideMark, hideCellMark);
		}
		internal void WriteCellMerging(MergingState mergingState) {
			TableCellGeneralSettingsInfo defaultGeneralSetting = DocumentModel.Cache.TableCellGeneralSettingsInfoCache.DefaultItem;
			WriteCellVerticalMerging(mergingState, defaultGeneralSetting.VerticalMerging);
		}
		void WriteCellVerticalMerging(MergingState value, MergingState defaultValue) {
			if (value == defaultValue)
				return;
			if (value == MergingState.Restart)
				RtfBuilder.WriteCommand(RtfExportSR.TableCellStartVerticalMerging);
			else if (value == MergingState.Continue)
				RtfBuilder.WriteCommand(RtfExportSR.TableCellContinueVerticalMerging);
		}
		internal void WriteCellVerticalAlignment(VerticalAlignment verticalAlignment) {
			switch (verticalAlignment) {
				case VerticalAlignment.Top:
					RtfBuilder.WriteCommand(TableCellTextTopAlignment);
					break;
				case VerticalAlignment.Center:
					RtfBuilder.WriteCommand(TableCellTextCenterAlignment);
					break;
				case VerticalAlignment.Bottom:
					RtfBuilder.WriteCommand(TableCellTextBottomAlignment);
					break;
				default:
					Exceptions.ThrowInternalException();
					break;
			}
		}
		internal void WriteCellTextDirection(TextDirection value) {
			switch (value) {
				case TextDirection.LeftToRightTopToBottom:
					RtfBuilder.WriteCommand(RtfExportSR.TableCellLeftToRightTopToBottomTextDirection);
					break;
				case TextDirection.TopToBottomRightToLeft:
					RtfBuilder.WriteCommand(RtfExportSR.TableCellTopToBottomRightToLeftTextDirection);
					break;
				case TextDirection.BottomToTopLeftToRight:
					RtfBuilder.WriteCommand(RtfExportSR.TableCellBottomToTopLeftToRightTextDirection);
					break;
				case TextDirection.LeftToRightTopToBottomRotated:
					RtfBuilder.WriteCommand(RtfExportSR.TableCellLeftToRightTopToBottomVerticalTextDirection);
					break;
				case TextDirection.TopToBottomRightToLeftRotated:
					RtfBuilder.WriteCommand(RtfExportSR.TableCellTopToBottomRightToLeftVerticalTextDirection);
					break;
				default:
					break;
			}
		}
		#endregion
		#region WriteCellBorders
		internal virtual void WriteCellBasicBorders(BorderInfo topBorder, BorderInfo leftBorder, BorderInfo rightBorder, BorderInfo bottomBorder) {
			RtfBuilder.WriteCommand(CellTopBorder);
			WriteBorderProperties(topBorder);
			RtfBuilder.WriteCommand(CellLeftBorder);
			WriteBorderProperties(leftBorder);
			RtfBuilder.WriteCommand(CellBottomBorder);
			WriteBorderProperties(bottomBorder);
			RtfBuilder.WriteCommand(CellRightBorder);
			WriteBorderProperties(rightBorder);
		}
		#endregion
		#region WriteCellPreferredWidth
		internal void WriteCellPreferredWidth(WidthUnitInfo preferredWidth) {
			WriteWidthUnit(preferredWidth, RtfExportSR.TableCellPreferredWidthType, RtfExportSR.TableCellPreferredWidth);
		}
		#endregion
		#region WriteCellMargings
		internal void WriteCellMargings(TableCell cell) {
			WriteCellMargings(cell.GetActualTopMargin().Info, cell.GetActualLeftMargin().Info, cell.GetActualRightMargin().Info, cell.GetActualBottomMargin().Info);
		}
		internal void WriteCellMargings(WidthUnitInfo topMargin, WidthUnitInfo leftMargin, WidthUnitInfo rightMargin, WidthUnitInfo bottomMargin) {
			if (ShouldExportCellMargin(bottomMargin))
				WriteWidthUnitInTwips(bottomMargin, RtfExportSR.TableCellBottomMarginType, RtfExportSR.TableCellBottomMargin);
			if (ShouldExportCellMargin(topMargin))
				WriteWidthUnitInTwips(topMargin, RtfExportSR.TableCellLeftMarginType, RtfExportSR.TableCellLeftMargin);
			if (ShouldExportCellMargin(rightMargin))
				WriteWidthUnitInTwips(rightMargin, RtfExportSR.TableCellRightMarginType, RtfExportSR.TableCellRightMargin);
			if (ShouldExportCellMargin(leftMargin))
				WriteWidthUnitInTwips(leftMargin, RtfExportSR.TableCellTopMarginType, RtfExportSR.TableCellTopMargin);
		}
		#endregion
		#region WriteCellRight
		internal void WriteCellRight(int cellRight) {
			RtfBuilder.WriteCommand(RtfExportSR.TableCellRight, UnitConverter.ModelUnitsToTwips(cellRight));
		}
		#endregion
	}
	#endregion
	#region RtfTableStyleTableCellPropertiesExporter
	class RtfTableStyleTableCellPropertiesExporter : RtfTableCellPropertiesExporter {
		public RtfTableStyleTableCellPropertiesExporter(DocumentModel documentModel, IRtfExportHelper rtfExportHelper, RtfBuilder rtfBuilder)
			: base(documentModel, rtfExportHelper, rtfBuilder) {
		}
		#region Properties
		protected override string TableCellBackgroundColor { get { return RtfExportSR.TableStyleCellBackgroundColor; } }
		protected override string TableCellNoWrap { get { return RtfExportSR.TableStyleCellNoWrap; } }
		protected override string TableCellTextTopAlignment { get { return RtfExportSR.TableStyleCellVerticalAlignmentTop; } }
		protected override string TableCellTextCenterAlignment { get { return RtfExportSR.TableStyleCellVerticalAlignmentCenter; } }
		protected override string TableCellTextBottomAlignment { get { return RtfExportSR.TableStyleCellVerticalAlignmentBottom; } }
		protected override string TableCellUpperLeftToLowerRightBorder { get { return RtfExportSR.TableStyleUpperLeftToLowerRightBorder; } }
		protected override string TableCellUpperRightToLowerLeftBorder { get { return RtfExportSR.TableStyleUpperRightToLowerLeftBorder; } }
		protected override string CellTopBorder { get { return RtfExportSR.TableStyleTopCellBorder; } }
		protected override string CellLeftBorder { get { return RtfExportSR.TableStyleLeftCellBorder; } }
		protected override string CellBottomBorder { get { return RtfExportSR.TableStyleBottomCellBorder; } }
		protected override string CellRightBorder { get { return RtfExportSR.TableStyleRightCellBorder; } }
		#endregion
		#region WriteCellBorders
		internal override void WriteCellBasicBorders(BorderInfo topBorder, BorderInfo leftBorder, BorderInfo rightBorder, BorderInfo bottomBorder) {
			BorderInfo defaultBorder = DocumentModel.Cache.BorderInfoCache.DefaultItem;
			if (topBorder != defaultBorder) {
				RtfBuilder.WriteCommand(CellTopBorder);
				WriteBorderProperties(topBorder);
			}
			if (leftBorder != defaultBorder) {
				RtfBuilder.WriteCommand(CellLeftBorder);
				WriteBorderProperties(leftBorder);
			}
			if (bottomBorder != defaultBorder) {
				RtfBuilder.WriteCommand(CellBottomBorder);
				WriteBorderProperties(bottomBorder);
			}
			if (rightBorder != defaultBorder) {
				RtfBuilder.WriteCommand(CellRightBorder);
				WriteBorderProperties(rightBorder);
			}
		}
		#endregion
	}
	#endregion
}
