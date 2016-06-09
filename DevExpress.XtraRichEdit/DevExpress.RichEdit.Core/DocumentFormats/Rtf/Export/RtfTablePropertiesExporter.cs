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
#if !SL
using System.Drawing;
#else
using System.Windows.Media;
#endif
namespace DevExpress.XtraRichEdit.Export.Rtf {
	#region RtfTablePropertiesExporter
	public class RtfTablePropertiesExporter : RtfPropertiesExporter {
		#region Fields
		#endregion
		public RtfTablePropertiesExporter(DocumentModel documentModel, IRtfExportHelper rtfExportHelper, RtfBuilder rtfBuilder)
			: base(documentModel, rtfExportHelper, rtfBuilder) {
		}
		#region Properties
		protected virtual string TableTopBorder { get { return RtfExportSR.TableTopBorder; } }
		protected virtual string TableLeftBorder { get { return RtfExportSR.TableLeftBorder; } }
		protected virtual string TableBottomBorder { get { return RtfExportSR.TableBottomBorder; } }
		protected virtual string TableRightBorder { get { return RtfExportSR.TableRightBorder; } }
		protected virtual string TableHorizontalBorder { get { return RtfExportSR.TableHorizontalBorder; } }
		protected virtual string TableVerticalBorder { get { return RtfExportSR.TableVerticalBorder; } }
		protected virtual string TableCellMarginsLeftType { get { return RtfExportSR.TableCellMarginsLeftType; } }
		protected virtual string TableCellMarginsLeft { get { return RtfExportSR.TableCellMarginsLeft; } }
		protected virtual string TableCellMarginsBottomType { get { return RtfExportSR.TableCellMarginsBottomType; } }
		protected virtual string TableCellMarginsBottom { get { return RtfExportSR.TableCellMarginsBottom; } }
		protected virtual string TableCellMarginsRightType { get { return RtfExportSR.TableCellMarginsRightType; } }
		protected virtual string TableCellMarginsRight { get { return RtfExportSR.TableCellMarginsRight; } }
		protected virtual string TableCellMarginsTopType { get { return RtfExportSR.TableCellMarginsTopType; } }
		protected virtual string TableCellMarginsTop { get { return RtfExportSR.TableCellMarginsTop; } }
		#endregion
		#region WriteTableFloatingPosition
		internal void WriteTableFloatingPosition(TableFloatingPositionInfo floatingPosition) {
			if (floatingPosition.TextWrapping != TextWrapping.Around)
				return;
			TableFloatingPositionInfo defaultFloatingPosition = DocumentModel.Cache.TableFloatingPositionInfoCache.DefaultItem;
			if (floatingPosition.HorizontalAnchor != HorizontalAnchorTypes.Column)
				WriteTableHorizontalAnchor(floatingPosition.HorizontalAnchor);
			if (floatingPosition.TableHorizontalPosition != 0)
				WriteTableHorizontalPosition(floatingPosition.TableHorizontalPosition);
			else
				WriteTableHorizontalPosition(1);
			if (floatingPosition.HorizontalAlign != defaultFloatingPosition.HorizontalAlign)
				WriteTableHorizontalAlign(floatingPosition.HorizontalAlign);
			if (floatingPosition.VerticalAnchor != VerticalAnchorTypes.Margin)
				WriteTableVerticalAnchor(floatingPosition.VerticalAnchor);
			if (floatingPosition.TableVerticalPosition != 0)
				WriteTableVerticalPosition(floatingPosition.TableVerticalPosition);
			else
				WriteTableVerticalPosition(1);
			if (floatingPosition.VerticalAlign != defaultFloatingPosition.VerticalAlign)
				WriteTableVerticalAlign(floatingPosition.VerticalAlign);
			if (floatingPosition.LeftFromText != defaultFloatingPosition.LeftFromText)
				RtfBuilder.WriteCommand(RtfExportSR.TableRowLeftFromText, UnitConverter.ModelUnitsToTwips(floatingPosition.LeftFromText));
			if (floatingPosition.BottomFromText != defaultFloatingPosition.BottomFromText)
				RtfBuilder.WriteCommand(RtfExportSR.TableRowBottomFromText, UnitConverter.ModelUnitsToTwips(floatingPosition.BottomFromText));
			if (floatingPosition.RightFromText != defaultFloatingPosition.RightFromText)
				RtfBuilder.WriteCommand(RtfExportSR.TableRowRightFromText, UnitConverter.ModelUnitsToTwips(floatingPosition.RightFromText));
			if (floatingPosition.TopFromText != defaultFloatingPosition.TopFromText)
				RtfBuilder.WriteCommand(RtfExportSR.TableRowTopFromText, UnitConverter.ModelUnitsToTwips(floatingPosition.TopFromText));
		}
		internal void WriteTableHorizontalAnchor(HorizontalAnchorTypes value) {
			switch (value) {
				case HorizontalAnchorTypes.Column:
					RtfBuilder.WriteCommand(RtfExportSR.TableRowHorizontalAnchorColumn);
					break;
				case HorizontalAnchorTypes.Margin:
					RtfBuilder.WriteCommand(RtfExportSR.TableRowHorizontalAnchorMargin);
					break;
				case HorizontalAnchorTypes.Page:
					RtfBuilder.WriteCommand(RtfExportSR.TableRowHorizontalAnchorPage);
					break;
				default:
					Exceptions.ThrowInternalException();
					break;
			}
		}
		internal void WriteTableVerticalAnchor(VerticalAnchorTypes value) {
			switch (value) {
				case VerticalAnchorTypes.Margin:
					RtfBuilder.WriteCommand(RtfExportSR.TableRowVerticalAnchorMargin);
					break;
				case VerticalAnchorTypes.Page:
					RtfBuilder.WriteCommand(RtfExportSR.TableRowVerticalAnchorPage);
					break;
				case VerticalAnchorTypes.Paragraph:
					RtfBuilder.WriteCommand(RtfExportSR.TableRowVerticalAnchorParagraph);
					break;
				default:
					Exceptions.ThrowInternalException();
					break;
			}
		}
		internal void WriteTableHorizontalAlign(HorizontalAlignMode value) {
			switch (value) {
				case HorizontalAlignMode.Center:
					RtfBuilder.WriteCommand(RtfExportSR.TableRowHorizontalAlignCenter);
					break;
				case HorizontalAlignMode.Inside:
					RtfBuilder.WriteCommand(RtfExportSR.TableRowHorizontalAlignInside);
					break;
				case HorizontalAlignMode.Left:
					RtfBuilder.WriteCommand(RtfExportSR.TableRowHorizontalAlignLeft);
					break;
				case HorizontalAlignMode.Outside:
					RtfBuilder.WriteCommand(RtfExportSR.TableRowHorizontalAlignOutside);
					break;
				case HorizontalAlignMode.Right:
					RtfBuilder.WriteCommand(RtfExportSR.TableRowHorizontalAlignRight);
					break;
				case HorizontalAlignMode.None:
					break;
				default:
					Exceptions.ThrowInternalException();
					break;
			}
		}
		internal void WriteTableVerticalAlign(VerticalAlignMode value) {
			switch (value) {
				case VerticalAlignMode.Bottom:
					RtfBuilder.WriteCommand(RtfExportSR.TableRowVerticalAlignBottom);
					break;
				case VerticalAlignMode.Center:
					RtfBuilder.WriteCommand(RtfExportSR.TableRowVerticalAlignCenter);
					break;
				case VerticalAlignMode.Inline:
					RtfBuilder.WriteCommand(RtfExportSR.TableRowVerticalAlignInline);
					break;
				case VerticalAlignMode.Inside:
					RtfBuilder.WriteCommand(RtfExportSR.TableRowVerticalAlignInside);
					break;
				case VerticalAlignMode.Outside:
					RtfBuilder.WriteCommand(RtfExportSR.TableRowVerticalAlignOutside);
					break;
				case VerticalAlignMode.Top:
					RtfBuilder.WriteCommand(RtfExportSR.TableRowVerticalAlignTop);
					break;
				case VerticalAlignMode.None:
					break;
				default:
					Exceptions.ThrowInternalException();
					break;
			}
		}
		internal void WriteTableHorizontalPosition(int value) {
			if (value >= 0)
				RtfBuilder.WriteCommand(RtfExportSR.TableRowHorizontalPosition, UnitConverter.ModelUnitsToTwips(value));
			else
				RtfBuilder.WriteCommand(RtfExportSR.TableRowHorizontalPositionNeg, UnitConverter.ModelUnitsToTwips(value));
		}
		internal void WriteTableVerticalPosition(int value) {
			if (value >= 0)
				RtfBuilder.WriteCommand(RtfExportSR.TableRowVerticalPosition, UnitConverter.ModelUnitsToTwips(value));
			else
				RtfBuilder.WriteCommand(RtfExportSR.TableRowVerticalPositionNeg, UnitConverter.ModelUnitsToTwips(value));
		}
		#endregion
		#region WriteRowLeft
		internal void WriteRowLeft(int left) {
			RtfBuilder.WriteCommand(RtfExportSR.TableRowLeft, UnitConverter.ModelUnitsToTwips(left));
		}
		#endregion
		#region WriteTableBorders
		internal void WriteTableBorders(BorderInfo topBorder, BorderInfo leftBorder, BorderInfo bottomBorder, BorderInfo rightBorder, BorderInfo innerHorizontalBorder, BorderInfo innerVerticalBorder) {
			BorderInfo defaultBorder = DocumentModel.Cache.BorderInfoCache.DefaultItem;
			if (topBorder != defaultBorder) {
				RtfBuilder.WriteCommand(TableTopBorder);
				WriteBorderProperties(topBorder);
			}
			if (leftBorder != defaultBorder) {
				RtfBuilder.WriteCommand(TableLeftBorder);
				WriteBorderProperties(leftBorder);
			}
			if (bottomBorder != defaultBorder) {
				RtfBuilder.WriteCommand(TableBottomBorder);
				WriteBorderProperties(bottomBorder);
			}
			if (rightBorder != defaultBorder) {
				RtfBuilder.WriteCommand(TableRightBorder);
				WriteBorderProperties(rightBorder);
			}
			if (innerHorizontalBorder != defaultBorder) {
				RtfBuilder.WriteCommand(TableHorizontalBorder);
				WriteBorderProperties(innerHorizontalBorder);
			}
			if (innerVerticalBorder != defaultBorder) {
				RtfBuilder.WriteCommand(TableVerticalBorder);
				WriteBorderProperties(innerVerticalBorder);
			}
		}
		#endregion
		#region WriteTableWidth
		internal void WriteTableWidth(WidthUnitInfo preferredWidth) {
			WriteWidthUnit(preferredWidth, RtfExportSR.TablePreferredWidthType, RtfExportSR.TablePreferredWidth);
		}
		#endregion
		#region WriteTableLayout
		internal void WriteTableLayout(TableLayoutType value) {
			if (value != TableLayoutType.Fixed) {
				RtfBuilder.WriteCommand(RtfExportSR.TableLayout, (int)value);
			}
		}
		#endregion
		#region WriteTableCellMargins
		internal void WriteTableCellMargins(WidthUnitInfo leftMargin, WidthUnitInfo rightMargin, WidthUnitInfo bottomMargin, WidthUnitInfo topMargin) {
			if (ShouldExportCellMargin(leftMargin))
				WriteWidthUnitInTwips(leftMargin, TableCellMarginsLeftType, TableCellMarginsLeft);
			if (ShouldExportCellMargin(bottomMargin))
				WriteWidthUnitInTwips(bottomMargin, TableCellMarginsBottomType, TableCellMarginsBottom);
			if (ShouldExportCellMargin(rightMargin))
				WriteWidthUnitInTwips(rightMargin, TableCellMarginsRightType, TableCellMarginsRight);
			if (ShouldExportCellMargin(topMargin))
				WriteWidthUnitInTwips(topMargin, TableCellMarginsTopType, TableCellMarginsTop);
		}
		#endregion
		#region WriteTableLook
		internal void WriteTableLook(TableLookTypes value) {
			TableGeneralSettingsInfo defaultGeneralSettings = DocumentModel.Cache.TableGeneralSettingsInfoCache.DefaultItem;
			if (value == defaultGeneralSettings.TableLook)
				return;
			if ((value & TableLookTypes.ApplyFirstColumn) != 0)
				RtfBuilder.WriteCommand(RtfExportSR.TableApplyFirstColumn);
			if ((value & TableLookTypes.ApplyFirstRow) != 0)
				RtfBuilder.WriteCommand(RtfExportSR.TableApplyFirstRow);
			if ((value & TableLookTypes.ApplyLastColumn) != 0)
				RtfBuilder.WriteCommand(RtfExportSR.TableApplyLastColumn);
			if ((value & TableLookTypes.ApplyLastRow) != 0)
				RtfBuilder.WriteCommand(RtfExportSR.TableApplyLastRow);
			if ((value & TableLookTypes.DoNotApplyColumnBanding) != 0)
				RtfBuilder.WriteCommand(RtfExportSR.TableDoNotApplyColumnBanding);
			if ((value & TableLookTypes.DoNotApplyRowBanding) != 0)
				RtfBuilder.WriteCommand(RtfExportSR.TableDoNotApplyRowBanding);
		}
		#endregion
		#region WriteTableIndent
		internal void WriteTableIndent(WidthUnitInfo tableIndent) {
			WriteWidthUnit(tableIndent, RtfExportSR.TableIndentType, RtfExportSR.TableIndent, true);
		}
		#endregion
		#region WriteBandSizes
		internal void WriteBandSizes(TableGeneralSettingsInfo info, bool exportRowBand, bool exportColBand) {
			if (info.TableStyleRowBandSize != 0 && (exportRowBand || info.TableStyleRowBandSize > 1))
				RtfBuilder.WriteCommand(RtfExportSR.TableStyleRowBandSize, info.TableStyleRowBandSize);
			if (info.TableStyleColBandSize != 0 && (exportColBand || info.TableStyleColBandSize > 1))
				RtfBuilder.WriteCommand(RtfExportSR.TableStyleColumnBandSize, info.TableStyleColBandSize);
		}
		#endregion
	}
	#endregion
	#region RtfTableStyleTablePropertiesExporter
	public class RtfTableStyleTablePropertiesExporter : RtfTablePropertiesExporter {
		public RtfTableStyleTablePropertiesExporter(DocumentModel documentModel, IRtfExportHelper rtfExportHelper, RtfBuilder rtfBuilder)
			: base(documentModel, rtfExportHelper, rtfBuilder) {
		}
		#region Properties
		protected string TableStyleRowBandSize { get { return RtfExportSR.TableStyleRowBandSize; } }
		protected string TableStyleColumnBandSize { get { return RtfExportSR.TableStyleColumnBandSize; } }
		protected override string TableCellMarginsLeftType { get { return RtfExportSR.TableStyleTableLeftCellMarginUnitType; } }
		protected override string TableCellMarginsLeft { get { return RtfExportSR.TableStyleTableLeftCellMargin; } }
		protected override string TableCellMarginsBottomType { get { return RtfExportSR.TableStyleTableBottomCellMarginUnitType; } }
		protected override string TableCellMarginsBottom { get { return RtfExportSR.TableStyleTableBottomCellMargin; } }
		protected override string TableCellMarginsRightType { get { return RtfExportSR.TableStyleTableRightCellMarginUnitType; } }
		protected override string TableCellMarginsRight { get { return RtfExportSR.TableStyleTableRightCellMargin; } }
		protected override string TableCellMarginsTopType { get { return RtfExportSR.TableStyleTableTopCellMarginUnitType; } }
		protected override string TableCellMarginsTop { get { return RtfExportSR.TableStyleTableTopCellMargin; } }
		#endregion
		internal void ExportTableProperties(MergedTableProperties mergedTableProperties, bool exportRowBand, bool exportColBand) {
			CombinedTablePropertiesInfo info = mergedTableProperties.Info;
			TablePropertiesOptions options = mergedTableProperties.Options;
			WriteBandSizes(info.GeneralSettings, exportRowBand, exportColBand);
			WriteTableBorders(info.Borders.TopBorder, info.Borders.LeftBorder, info.Borders.BottomBorder, info.Borders.RightBorder, info.Borders.InsideHorizontalBorder, info.Borders.InsideVerticalBorder);
			WriteTableFloatingPosition(info.FloatingPosition);
			WriteTableWidth(info.PreferredWidth);
			WriteTableCellMargins(info.CellMargins.Left, info.CellMargins.Right, info.CellMargins.Bottom, info.CellMargins.Top);
			WriteTableLook(info.GeneralSettings.TableLook);
			WriteTableIndent(info.TableIndent);
		}
	}
	#endregion
}
