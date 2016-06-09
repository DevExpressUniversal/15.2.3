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
using DevExpress.Utils;
using DevExpress.Office;
using DevExpress.Office.History;
using DevExpress.Office.Utils;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Tables;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.XtraRichEdit.Import.Rtf.Tables;
using DevExpress.XtraRichEdit.Export.Rtf;
using System.Diagnostics;
using Debug = System.Diagnostics.Debug;
namespace DevExpress.XtraRichEdit.Import.Rtf {
	#region DestinationPieceTable (partial)
	public partial class DestinationPieceTable {
		#region ShadingPattern
		public static readonly Tuple<int, ShadingPattern>[] ShadingPatterns = new Tuple<int, ShadingPattern>[] {
			new Tuple<int, ShadingPattern>(0, ShadingPattern.Clear),
			new Tuple<int, ShadingPattern>(500, ShadingPattern.Pct5),
			new Tuple<int, ShadingPattern>(1000, ShadingPattern.Pct10),
			new Tuple<int, ShadingPattern>(1250, ShadingPattern.Pct12),
			new Tuple<int, ShadingPattern>(1500, ShadingPattern.Pct15),
			new Tuple<int, ShadingPattern>(2000, ShadingPattern.Pct20),
			new Tuple<int, ShadingPattern>(2500, ShadingPattern.Pct25),
			new Tuple<int, ShadingPattern>(3000, ShadingPattern.Pct30),
			new Tuple<int, ShadingPattern>(3500, ShadingPattern.Pct35),
			new Tuple<int, ShadingPattern>(3750, ShadingPattern.Pct37),
			new Tuple<int, ShadingPattern>(4000, ShadingPattern.Pct40),
			new Tuple<int, ShadingPattern>(4500, ShadingPattern.Pct45),
			new Tuple<int, ShadingPattern>(5000, ShadingPattern.Pct50),
			new Tuple<int, ShadingPattern>(5500, ShadingPattern.Pct55),
			new Tuple<int, ShadingPattern>(6000, ShadingPattern.Pct60),
			new Tuple<int, ShadingPattern>(6250, ShadingPattern.Pct62),
			new Tuple<int, ShadingPattern>(6500, ShadingPattern.Pct65),
			new Tuple<int, ShadingPattern>(7000, ShadingPattern.Pct70),
			new Tuple<int, ShadingPattern>(7500, ShadingPattern.Pct75),
			new Tuple<int, ShadingPattern>(8000, ShadingPattern.Pct80),
			new Tuple<int, ShadingPattern>(8500, ShadingPattern.Pct85),
			new Tuple<int, ShadingPattern>(8750, ShadingPattern.Pct87),
			new Tuple<int, ShadingPattern>(9000, ShadingPattern.Pct90),
			new Tuple<int, ShadingPattern>(9500, ShadingPattern.Pct95),
			new Tuple<int, ShadingPattern>(10000, ShadingPattern.Solid),
		};
		public static ShadingPattern GetShadingPattern(int value) {
			for (int i = ShadingPatterns.Length - 1; i >= 0; i--)
				if (value >= ShadingPatterns[i].Item1)
					return ShadingPatterns[i].Item2;
			return ShadingPattern.Clear;
		}
		public static int GetShadingValue(ShadingPattern pattern) {
			for (int i = ShadingPatterns.Length - 1; i >= 0; i--)
				if (pattern == ShadingPatterns[i].Item2)
					return ShadingPatterns[i].Item1;
			return 0;
		}
		#endregion
		public static void AppendTableKeywords(KeywordTranslatorTable table) {
			#region Tables
			table.Add("intbl", OnInTableParagraphKeyword);
			table.Add("row", OnRowKeyword);
			table.Add("cell", OnCellKeyword);
			table.Add("nestcell", OnNestedCellKeyword);
			table.Add("nestrow", OnNestedRowKeyword);
			table.Add("nesttableprops", OnNestedTablePropertiesKeyword);
			table.Add("nonesttables", OnNoNestedTablesKeyword);
			table.Add("itap", OnItapKeyword);
			AppendTablePropertiesKeywords(table);
		}
		public static void AppendTablePropertiesKeywords(KeywordTranslatorTable table) {
			table.Add("trowd", OnTableRowDefaultsKeyword);
			table.Add("ts", OnTableStyleKeyword);
			table.Add("cellx", OnCellxKeyword);
			table.Add("clwWidth", OnCellPreferredWidthKeyword);
			table.Add("clftsWidth", OnWidthUnitTypeKeyword);
			table.Add("clmgf", OnFirstHorizontalMergedCellKeyword);
			table.Add("clmrg", OnNextHorizontalMergedCellKeyword);
			table.Add("clvmgf", OnFirstVerticalMergedCellKeyword);
			table.Add("clvmrg", OnNextVerticalMergedCellKeyword);
			table.Add("clFitText", OnCellFitTextKeyword);
			table.Add("clNoWrap", OnCellNoWrapKeyword);
			table.Add("tsvertalt", OnCellVerticalAlignmentTopKeyword);
			table.Add("tsvertalc", OnCellVerticalAlignmentCenterKeyword);
			table.Add("tsvertalb", OnCellVerticalAlignmentBottomKeyword);
			table.Add("clhidemark", OnCellHideMarkKeyword);
			table.Add("clpadb", OnCellBottomCellMarginKeyword);
			table.Add("clpadl", OnCellLeftCellMarginKeyword);
			table.Add("clpadr", OnCellRightCellMarginKeyword);
			table.Add("clpadt", OnCellTopCellMarginKeyword);
			table.Add("clpadfb", OnCellBottomCellMarginUnitTypeKeyword);
			table.Add("clpadfl", OnCellLeftCellMarginUnitTypeKeyword);
			table.Add("clpadfr", OnCellRightCellMarginUnitTypeKeyword);
			table.Add("clpadft", OnCellTopCellMarginUnitTypeKeyword);
			table.Add("clvertalt", OnCellTextTopAlignmentKeyword);
			table.Add("clvertalc", OnCellTextCenterVerticalAlignmentKeyword);
			table.Add("clvertalb", OnCellTextBottomAlignmentKeyword);
			table.Add("cltxlrtb", OnCellLeftToRightTopToBottomTextDirectionKeyword);
			table.Add("cltxtbrl", OnCellTopToBottomRightToLeftTextDirectionKeyword);
			table.Add("cltxbtlr", OnCellBottomToTopLeftToRightTextDirectionKeyword);
			table.Add("cltxlrtbv", OnCellLeftToRightTopToBottomVerticalTextDirectionKeyword);
			table.Add("cltxtbrlv", OnCellTopToBottomRightToLeftVerticalTextDirectionKeyword);
			table.Add("clcbpat", OnCellBackgroundColor);
			table.Add("clcfpat", OnCellForegroundColor);
			table.Add("clcfpatraw", OnCellForegroundColor);
			table.Add("clshdng", OnCellShading);
			table.Add("clshdngraw", OnCellShading);
			table.Add("brdrtbl", OnNoTableBorderKeyword);
			table.Add("clbrdrb", OnBottomCellBorderKeyword);
			table.Add("clbrdrt", OnTopCellBorderKeyword);
			table.Add("clbrdrl", OnLeftCellBorderKeyword);
			table.Add("clbrdrr", OnRightCellBorderKeyword);
			table.Add("cldglu", OnUpperLeftToLowerRightBorderKeyword);
			table.Add("cldgll", OnUpperRightToLowerLeftBorderKeyword);
			table.Add("trleft", OnRowLeftKeyword);
			table.Add("trhdr", OnRowHeaderKeyword);
			table.Add("trrh", OnRowHeightKeyword);
			table.Add("trkeep", OnRowKeepKeyword);
			table.Add("trqr", OnTableRightAlignmentKeyword);
			table.Add("trql", OnTableLeftAlignmentKeyword);
			table.Add("trqc", OnTableCenterAlignmentKeyword);
			table.Add("trwWidthB", OnWidthBeforeKeyword);
			table.Add("trftsWidthB", OnWidthBeforeUnitTypeKeyword);
			table.Add("trwWidthA", OnWidthAfterKeyword);
			table.Add("trftsWidthA", OnWidthAfterUnitTypeKeyword);
			table.Add("trgaph", OnSpaceBetweenCellsKeyword);
			table.Add("trpaddb", OnTableBottomCellMarginKeyword);
			table.Add("trpaddl", OnTableLeftCellMarginKeyword);
			table.Add("trpaddr", OnTableRightCellMarginKeyword);
			table.Add("trpaddt", OnTableTopCellMarginKeyword);
			table.Add("trpaddfb", OnTableBottomCellMarginUnitTypeKeyword);
			table.Add("trpaddfl", OnTableLeftCellMarginUnitTypeKeyword);
			table.Add("trpaddfr", OnTableRightCellMarginUnitTypeKeyword);
			table.Add("trpaddft", OnTableTopCellMarginUnitTypeKeyword);
			table.Add("trspdb", OnTableBottomCellSpacingKeyword);
			table.Add("trspdl", OnTableLeftCellSpacingKeyword);
			table.Add("trspdr", OnTableRightCellSpacingKeyword);
			table.Add("trspdt", OnTableTopCellSpacingKeyword);
			table.Add("trspdfb", OnTableBottomCellSpacingUnitTypeKeyword);
			table.Add("trspdfl", OnTableLeftCellSpacingUnitTypeKeyword);
			table.Add("trspdfr", OnTableRightCellSpacingUnitTypeKeyword);
			table.Add("trspdft", OnTableTopCellSpacingUnitTypeKeyword);
			table.Add("trwWidth", OnTablePreferredWidthKeyword);
			table.Add("trftsWidth", OnTablePreferredWidthUnitTypeKeyword);
			table.Add("tblind", OnTableIndentKeyword);
			table.Add("tblindtype", OnTableIndentUnitType);
			table.Add("tabsnoovrlp", OnTableOverlapKeyword);
			table.Add("tdfrmtxtLeft", OnTableLeftFromTextKeyword);
			table.Add("tdfrmtxtRight", OnTableRightFromTextKeyword);
			table.Add("tdfrmtxtTop", OnTableTopFromTextKeyword);
			table.Add("tdfrmtxtBottom", OnTableBottomFromTextKeyword);
			table.Add("tphcol", OnColHorizontalAnchorKeyword);
			table.Add("tphmrg", OnMarginHorizontalAnchorKeyword);
			table.Add("tphpg", OnPageHorizontalAnchorKeyword);
			table.Add("tpvmrg", OnMarginVerticalAnchorKeyword);
			table.Add("tpvpara", OnParagraphVerticalAnchorKeword);
			table.Add("tpvpg", OnPageVerticalAnchorKeyword);
			table.Add("tposx", OnTableHorizontalPositionKeyword);
			table.Add("tposnegx", OnTableHorizontalPositionKeyword);
			table.Add("tposy", OnTableVerticalPositionKeyword);
			table.Add("tposnegy", OnTableVerticalPositionKeyword);
			table.Add("tposxc", OnCenterTableHorizontalAlignKeyword);
			table.Add("tposxi", OnInsideTableHorizontalAlignKeyword);
			table.Add("tposxl", OnLeftTableHorizontalAlignKeyword);
			table.Add("tposxo", OnOutsideTableHorizontalAlignKeyword);
			table.Add("tposxr", OnRightTableHorizontalAlignKeyword);
			table.Add("tposyb", OnBottomTableVerticalAlignKeyword);
			table.Add("tposyc", OnCenterTableVerticalAlignKeyword);
			table.Add("tposyil", OnInlineTableVerticalAlignKeyword);
			table.Add("tposyin", OnInsideTableVerticalAlignKeyword);
			table.Add("tposyout", OnOutsideTableVerticalAlignKeyword);
			table.Add("tposyt", OnTopTableVerticalAlignKeyword);
			table.Add("trautofit", OnTableAutoFitKeyword);
			table.Add("tscbandsh", OnRowBandSizeKeyword);
			table.Add("tscbandsv", OnColumnBandSizeKeyword);
			table.Add("tscellcbpat", OnCellBackgroundColor);
			table.Add("tsbrdrt", OnTopCellBorderKeyword);
			table.Add("tsbrdrl", OnLeftCellBorderKeyword);
			table.Add("tsbrdrb", OnBottomCellBorderKeyword);
			table.Add("tsbrdrr", OnRightCellBorderKeyword);
			table.Add("tsnowrap", OnCellNoWrapKeyword);
			table.Add("tscellpaddb", OnTableBottomCellMarginKeyword);
			table.Add("tscellpaddl", OnTableLeftCellMarginKeyword);
			table.Add("tscellpaddr", OnTableRightCellMarginKeyword);
			table.Add("tscellpaddt", OnTableTopCellMarginKeyword);
			table.Add("tscellpaddfb", OnTableBottomCellMarginUnitTypeKeyword);
			table.Add("tscellpaddfl", OnTableLeftCellMarginUnitTypeKeyword);
			table.Add("tscellpaddfr", OnTableRightCellMarginUnitTypeKeyword);
			table.Add("tscellpaddft", OnTableTopCellMarginUnitTypeKeyword);
			table.Add("tsbrdrdgl", OnUpperLeftToLowerRightBorderKeyword);
			table.Add("tsbrdrdgr", OnUpperRightToLowerLeftBorderKeyword);
			table.Add("tsrowd", OnTableRowDefaultsKeyword);
			table.Add("trbrdrt", OnTopTableBorderKeyword);
			table.Add("trbrdrl", OnLeftTableBorderKeyword);
			table.Add("trbrdrb", OnBottomTableBorderKeword);
			table.Add("trbrdrr", OnRightTableBorderKeyword);
			table.Add("trbrdrh", OnHorizontalTableBorderKeyword);
			table.Add("trbrdrv", OnVerticalTableBorderKeyword);
			table.Add("tbllkhdrrows", OnApplyFirstRowConditionalFormattingKeyword);
			table.Add("tbllklastrow", OnApplyLastRowConditionalFormattingKeyword);
			table.Add("tbllkhdrcols", OnApplyFirstColumnContitionalFormattingKeyword);
			table.Add("tbllklastcol", OnApplyLastColumnConditionalFormattingKeyword);
			table.Add("tbllknorowband", OnDoNotApplyRowBandingConditionalFormattingKeyword);
			table.Add("tbllknocolband", OnDoNotApplyColumnBandingConditionalFormattingKeyword);
			#endregion
			#region Border
			table.Add("brdrnil", OnNoBorderKeyword);
			table.Add("brdrw", OnBorderWidthKeyword);
			table.Add("brdrcf", OnBorderColorKeyword);
			table.Add("brdrframe", OnFrameBorderKeyword);
			table.Add("brsp", OnBorderSpaceKeyword);
			table.Add("brdrs", OnSingleThicknessBorderTypeKeyword);
			table.Add("brdrth", OnDoubleThicknessBorderTypeKeyword);
			table.Add("brdrsh", OnShadowedBorderTypeKeyword);
			table.Add("brdrdb", OnDoubleBorderTypeKeyword);
			table.Add("brdrdot", OnDottedBorderTypeKeyword);
			table.Add("brdrdash", OnDashedBorderTypeKeyword);
			table.Add("brdrhair", OnHairlineBorderTypeKeyword);
			table.Add("brdrdashsm", OnSmallDashedBorderTypeKeyword);
			table.Add("brdrdashd", OnDotDashedBorderTypeKeyword);
			table.Add("brdrdashdd", OnDotDotDashedBorderTypeKeyword);
			table.Add("brdrdashdot", OnDotDashedBorderTypeKeyword);
			table.Add("brdrdashdotdot", OnDotDotDashedBorderTypeKeyword);
			table.Add("brdrinset", OnInsetBorderTypeKeyword);
			table.Add("brdrnone", OnNoneBorderTypeKeyword);
			table.Add("brdroutset", OnOutsetBorderTypeKeyword);
			table.Add("brdrtriple", OnTripletBorderTypeKeyword);
			table.Add("brdrtnthsg", OnSmallThickThinBorderTypeKeyword);
			table.Add("brdrthtnsg", OnSmallThinThickBorderTypeKeyword);
			table.Add("brdrtnthtnsg", OnSmallThinThickThinBorderTypeKeyword);
			table.Add("brdrtnthmg", OnMediumThickThinBorderTypeKeyword);
			table.Add("brdrthtnmg", OnMediumThinThickBorderTypeKeyword);
			table.Add("brdrtnthtnmg", OnMediumThinThickThinBorderTypeKeyword);
			table.Add("brdrtnthlg", OnLargeThickThinBorderTypeKeyword);
			table.Add("brdrthtnlg", OnLargeThinThickBorderTypeKeyword);
			table.Add("brdrtnthtnlg", OnLargeThinThickThinBorderTypeKeyword);
			table.Add("brdrwavy", OnWavyBorderTypeKeyword);
			table.Add("brdrwavydb", OnDoubleWavyBorderTypeKeyword);
			table.Add("brdrdashdotstr", OnStripedBorderTypeKeyword);
			table.Add("brdremboss", OnEmbossedBorderTypeKeyword);
			table.Add("brdrengrave", OnEngravedBorderTypeKeyword);
			table.Add("brdrart", OnBorderArtIndex);
			#endregion
		}
		#region Handlers
		#region Tables
		static void OnCellxKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			importer.TableReader.OnCellxProperty(parameterValue);
		}
		static void OnCellPreferredWidthKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			if (parameterValue < 0 || !hasParameter)
				return;
			importer.TableReader.CellProperties.PreferredWidth.Value = parameterValue;
		}
		static void OnWidthUnitTypeKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			if (!hasParameter)
				return;
			importer.TableReader.CellProperties.PreferredWidth.Type = GetWidthUnitType(parameterValue);
		}
		static WidthUnitType GetWidthUnitType(int parameterValue) {
			WidthUnitType unitType = WidthUnitType.Auto;
			switch (parameterValue) {
				case 0:
					unitType = WidthUnitType.Nil;
					break;
				case 1:
					unitType = WidthUnitType.Auto;
					break;
				case 2:
					unitType = WidthUnitType.FiftiethsOfPercent;
					break;
				case 3:
					unitType = WidthUnitType.ModelUnits;
					break;
				default:
					goto case 0;
			}
			return unitType;
		}
		static void OnFirstHorizontalMergedCellKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			if (importer.TableReader.CellProperties.HorizontalMerging == MergingState.None)
				importer.TableReader.CellProperties.HorizontalMerging = MergingState.Restart;
		}
		static void OnNextHorizontalMergedCellKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			importer.TableReader.CellProperties.HorizontalMerging = MergingState.Continue;
		}
		static void OnFirstVerticalMergedCellKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			importer.TableReader.CellProperties.VerticalMerging = MergingState.Restart;
		}
		static void OnNextVerticalMergedCellKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			importer.TableReader.CellProperties.VerticalMerging = MergingState.Continue;
		}
		static void OnRowLeftKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			importer.TableReader.RowProperties.Left = parameterValue;
		}
		static void OnRowHeaderKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			if (!hasParameter || parameterValue != 0) {
				importer.TableReader.RowProperties.Header = true;
			}
		}
		static void OnRowHeightKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			if (!hasParameter)
				return;
			int val = importer.UnitConverter.TwipsToModelUnits(parameterValue);
			RtfTableRowProperties rowProperties = importer.TableReader.RowProperties;
			rowProperties.Height.Value = Math.Abs(val);
			if (parameterValue < 0)
				rowProperties.Height.Type = HeightUnitType.Exact;
			else if (parameterValue > 0)
				rowProperties.Height.Type = HeightUnitType.Minimum;
			else
				rowProperties.Height.Type = HeightUnitType.Auto;
		}
		static void OnRowKeepKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			if (!hasParameter || parameterValue != 0) {
				importer.TableReader.RowProperties.CantSplit = true;
			}
		}
		static void OnTableRightAlignmentKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			importer.TableReader.RowProperties.TableRowAlignment = TableRowAlignment.Right;
		}
		static void OnTableLeftAlignmentKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			importer.TableReader.RowProperties.TableRowAlignment = TableRowAlignment.Left;
		}
		static void OnTableCenterAlignmentKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			importer.TableReader.RowProperties.TableRowAlignment = TableRowAlignment.Center;
		}
		static void OnSpaceBetweenCellsKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			if (parameterValue < 0 || !hasParameter)
				return;
			importer.TableReader.TableProperties.HalfSpace = parameterValue;
		}
		static void OnTableBottomCellMarginKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			if (parameterValue < 0 || !hasParameter)
				return;
			importer.TableReader.TableProperties.CellMargins.Bottom.Value = parameterValue;
		}
		static void OnTableLeftCellMarginKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			if (parameterValue < 0 || !hasParameter)
				return;
			importer.TableReader.TableProperties.CellMargins.Left.Value = parameterValue;
		}
		static void OnTableRightCellMarginKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			if (parameterValue < 0 || !hasParameter)
				return;
			importer.TableReader.TableProperties.CellMargins.Right.Value = parameterValue;
		}
		static void OnTableTopCellMarginKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			if (parameterValue < 0 || !hasParameter)
				return;
			importer.TableReader.TableProperties.CellMargins.Top.Value = parameterValue;
		}
		static void OnTableBottomCellMarginUnitTypeKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			if (!hasParameter)
				return;
			AssignWidthUnitInfo(importer.TableReader.TableProperties.CellMargins.Bottom, parameterValue);
		}
		static void AssignWidthUnitInfo(WidthUnit unitInfo, int value) {
			if (value == 3)
				unitInfo.Type = WidthUnitType.ModelUnits;
			else if (value == 0)
				unitInfo.Type = WidthUnitType.Nil;
		}
		static void OnTableLeftCellMarginUnitTypeKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			if (!hasParameter)
				return;
			AssignWidthUnitInfo(importer.TableReader.TableProperties.CellMargins.Left, parameterValue);
		}
		static void OnTableRightCellMarginUnitTypeKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			if (!hasParameter)
				return;
			AssignWidthUnitInfo(importer.TableReader.TableProperties.CellMargins.Right, parameterValue);
		}
		static void OnTableTopCellMarginUnitTypeKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			if (!hasParameter)
				return;
			AssignWidthUnitInfo(importer.TableReader.TableProperties.CellMargins.Top, parameterValue);
		}
		static void OnTableBottomCellSpacingKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			if (parameterValue < 0 || !hasParameter)
				return;
			importer.TableReader.TableProperties.CellSpacing.Bottom.Value = parameterValue;
		}
		static void OnTableLeftCellSpacingKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			if (parameterValue < 0 || !hasParameter)
				return;
			importer.TableReader.TableProperties.CellSpacing.Left.Value = parameterValue;
		}
		static void OnTableRightCellSpacingKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			if (parameterValue < 0 || !hasParameter)
				return;
			importer.TableReader.TableProperties.CellSpacing.Right.Value = parameterValue;
		}
		static void OnTableTopCellSpacingKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			if (parameterValue < 0 || !hasParameter)
				return;
			importer.TableReader.TableProperties.CellSpacing.Top.Value = parameterValue;
		}
		static void OnTableBottomCellSpacingUnitTypeKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			if (!hasParameter)
				return;
			AssignWidthUnitInfo(importer.TableReader.TableProperties.CellSpacing.Bottom, parameterValue);
		}
		static void OnTableLeftCellSpacingUnitTypeKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			if (!hasParameter)
				return;
			AssignWidthUnitInfo(importer.TableReader.TableProperties.CellSpacing.Left, parameterValue);
		}
		static void OnTableRightCellSpacingUnitTypeKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			if (!hasParameter)
				return;
			AssignWidthUnitInfo(importer.TableReader.TableProperties.CellSpacing.Right, parameterValue);
		}
		static void OnTableTopCellSpacingUnitTypeKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			if (!hasParameter)
				return;
			AssignWidthUnitInfo(importer.TableReader.TableProperties.CellSpacing.Top, parameterValue);
		}
		static void OnTablePreferredWidthKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			if (parameterValue < 0 || !hasParameter)
				return;
			importer.TableReader.TableProperties.PreferredWidth.Value = parameterValue;
		}
		static void OnTablePreferredWidthUnitTypeKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			importer.TableReader.TableProperties.PreferredWidth.Type = GetWidthUnitType(parameterValue);
		}
		static void OnWidthBeforeKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			if (parameterValue < 0 || !hasParameter)
				return;
			importer.TableReader.RowProperties.WidthBefore.Value = parameterValue;
		}
		static void OnWidthBeforeUnitTypeKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			importer.TableReader.RowProperties.WidthBefore.Type = GetWidthUnitType(parameterValue);
		}
		static void OnWidthAfterKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			if (parameterValue < 0 || !hasParameter)
				return;
			importer.TableReader.RowProperties.WidthAfter.Value = parameterValue;
		}
		static void OnWidthAfterUnitTypeKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			importer.TableReader.RowProperties.WidthAfter.Type = GetWidthUnitType(parameterValue);
		}
		static void OnTableIndentKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			if (!hasParameter)
				return;
			importer.TableReader.TableProperties.TableIndent.Value = parameterValue;
		}
		static void OnTableIndentUnitType(RtfImporter importer, int parameterValue, bool hasParameter) {
			if (!hasParameter)
				return;
			importer.TableReader.TableProperties.TableIndent.Type = GetWidthUnitType(parameterValue);
		}
		static void OnCellFitTextKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			if (!hasParameter || parameterValue != 0) {
				importer.TableReader.CellProperties.FitText = true;
			}
		}
		static void OnCellNoWrapKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			if (!hasParameter || parameterValue != 0) {
				importer.TableReader.CellProperties.NoWrap = true;
			}
		}
		static void OnCellVerticalAlignmentTopKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			importer.TableReader.CellProperties.VerticalAlignment = VerticalAlignment.Top;
		}
		static void OnCellVerticalAlignmentCenterKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			importer.TableReader.CellProperties.VerticalAlignment = VerticalAlignment.Center;
		}
		static void OnCellVerticalAlignmentBottomKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			importer.TableReader.CellProperties.VerticalAlignment = VerticalAlignment.Bottom;
		}
		static void OnCellHideMarkKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			if (!hasParameter || parameterValue != 0) {
				importer.TableReader.CellProperties.HideCellMark = true;
			}
		}
		static void OnCellBottomCellMarginKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			if (parameterValue < 0 || !hasParameter)
				return;
			importer.TableReader.CellProperties.CellMargins.Bottom.Value = parameterValue;
		}
		static void OnCellLeftCellMarginKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			if (parameterValue < 0 || !hasParameter)
				return;
			importer.TableReader.CellProperties.CellMargins.Top.Value = parameterValue;
		}
		static void OnCellRightCellMarginKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			if (parameterValue < 0 || !hasParameter)
				return;
			importer.TableReader.CellProperties.CellMargins.Right.Value = parameterValue;
		}
		static void OnCellTopCellMarginKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			if (parameterValue < 0 || !hasParameter)
				return;
			importer.TableReader.CellProperties.CellMargins.Left.Value = parameterValue;
		}
		static void OnCellBottomCellMarginUnitTypeKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			if (!hasParameter)
				return;
			AssignWidthUnitInfo(importer.TableReader.CellProperties.CellMargins.Bottom, parameterValue);
		}
		static void OnCellLeftCellMarginUnitTypeKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			if (!hasParameter)
				return;
			AssignWidthUnitInfo(importer.TableReader.CellProperties.CellMargins.Top, parameterValue);
		}
		static void OnCellRightCellMarginUnitTypeKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			if (!hasParameter)
				return;
			AssignWidthUnitInfo(importer.TableReader.CellProperties.CellMargins.Right, parameterValue);
		}
		static void OnCellTopCellMarginUnitTypeKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			if (!hasParameter)
				return;
			AssignWidthUnitInfo(importer.TableReader.CellProperties.CellMargins.Left, parameterValue);
		}
		static void OnTableOverlapKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			if (!hasParameter || parameterValue != 0)
				importer.TableReader.TableProperties.IsTableOverlap = false;
		}
		static void OnTableLeftFromTextKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			if (!hasParameter)
				return;
			importer.TableReader.RowProperties.FloatingPosition.LeftFromText = importer.UnitConverter.TwipsToModelUnits(parameterValue);
		}
		static void OnTableRightFromTextKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			if (!hasParameter)
				return;
			importer.TableReader.RowProperties.FloatingPosition.RightFromText = importer.UnitConverter.TwipsToModelUnits(parameterValue);
		}
		static void OnTableTopFromTextKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			if (!hasParameter)
				return;
			importer.TableReader.RowProperties.FloatingPosition.TopFromText = importer.UnitConverter.TwipsToModelUnits(parameterValue);
		}
		static void OnTableBottomFromTextKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			if (!hasParameter)
				return;
			importer.TableReader.RowProperties.FloatingPosition.BottomFromText = importer.UnitConverter.TwipsToModelUnits(parameterValue);
		}
		static void OnColHorizontalAnchorKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			importer.TableReader.RowProperties.FloatingPosition.HorizontalAnchor = HorizontalAnchorTypes.Column;
		}
		static void OnMarginHorizontalAnchorKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			importer.TableReader.RowProperties.FloatingPosition.TextWrapping = TextWrapping.Around;
			importer.TableReader.RowProperties.FloatingPosition.HorizontalAnchor = HorizontalAnchorTypes.Margin;
		}
		static void OnPageHorizontalAnchorKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			importer.TableReader.RowProperties.FloatingPosition.TextWrapping = TextWrapping.Around;
			importer.TableReader.RowProperties.FloatingPosition.HorizontalAnchor = HorizontalAnchorTypes.Page;
		}
		static void OnTableHorizontalPositionKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			if (!hasParameter)
				return;
			importer.TableReader.RowProperties.FloatingPosition.TableHorizontalPosition = importer.UnitConverter.TwipsToModelUnits(parameterValue);
			if (parameterValue != 0)
				importer.TableReader.RowProperties.FloatingPosition.TextWrapping = TextWrapping.Around;
		}
		static void OnTableVerticalPositionKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			if (!hasParameter)
				return;
			if (parameterValue != 0)
				importer.TableReader.RowProperties.FloatingPosition.TextWrapping = TextWrapping.Around;
			importer.TableReader.RowProperties.FloatingPosition.TableVerticalPosition = importer.UnitConverter.TwipsToModelUnits(parameterValue);
		}
		static void OnCenterTableHorizontalAlignKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			importer.TableReader.RowProperties.FloatingPosition.HorizontalAlign = HorizontalAlignMode.Center;
		}
		static void OnInsideTableHorizontalAlignKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			importer.TableReader.RowProperties.FloatingPosition.HorizontalAlign = HorizontalAlignMode.Inside;
		}
		static void OnLeftTableHorizontalAlignKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			importer.TableReader.RowProperties.FloatingPosition.HorizontalAlign = HorizontalAlignMode.Left;
		}
		static void OnOutsideTableHorizontalAlignKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			importer.TableReader.RowProperties.FloatingPosition.HorizontalAlign = HorizontalAlignMode.Outside;
		}
		static void OnRightTableHorizontalAlignKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			importer.TableReader.RowProperties.FloatingPosition.HorizontalAlign = HorizontalAlignMode.Right;
		}
		static void OnBottomTableVerticalAlignKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			importer.TableReader.RowProperties.FloatingPosition.VerticalAlign = VerticalAlignMode.Bottom;
		}
		static void OnCenterTableVerticalAlignKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			importer.TableReader.RowProperties.FloatingPosition.VerticalAlign = VerticalAlignMode.Center;
		}
		static void OnInlineTableVerticalAlignKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			importer.TableReader.RowProperties.FloatingPosition.VerticalAlign = VerticalAlignMode.Inline;
		}
		static void OnInsideTableVerticalAlignKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			importer.TableReader.RowProperties.FloatingPosition.VerticalAlign = VerticalAlignMode.Inside;
		}
		static void OnOutsideTableVerticalAlignKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			importer.TableReader.RowProperties.FloatingPosition.VerticalAlign = VerticalAlignMode.Outside;
		}
		static void OnTopTableVerticalAlignKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			importer.TableReader.RowProperties.FloatingPosition.VerticalAlign = VerticalAlignMode.Top;
		}
		static void OnMarginVerticalAnchorKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			importer.TableReader.RowProperties.FloatingPosition.VerticalAnchor = VerticalAnchorTypes.Margin;
		}
		static void OnParagraphVerticalAnchorKeword(RtfImporter importer, int parameterValue, bool hasParameter) {
			importer.TableReader.RowProperties.FloatingPosition.TextWrapping = TextWrapping.Around;
			importer.TableReader.RowProperties.FloatingPosition.VerticalAnchor = VerticalAnchorTypes.Paragraph;
		}
		static void OnPageVerticalAnchorKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			importer.TableReader.RowProperties.FloatingPosition.TextWrapping = TextWrapping.Around;
			importer.TableReader.RowProperties.FloatingPosition.VerticalAnchor = VerticalAnchorTypes.Page;
		}
		static void OnTopTableBorderKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			importer.TableReader.ProcessedBorder = importer.TableReader.TableProperties.Borders.TopBorder;
		}
		static void OnLeftTableBorderKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			importer.TableReader.ProcessedBorder = importer.TableReader.TableProperties.Borders.LeftBorder;
		}
		static void OnBottomTableBorderKeword(RtfImporter importer, int parameterValue, bool hasParameter) {
			importer.TableReader.ProcessedBorder = importer.TableReader.TableProperties.Borders.BottomBorder;
		}
		static void OnRightTableBorderKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			importer.TableReader.ProcessedBorder = importer.TableReader.TableProperties.Borders.RightBorder;
		}
		static void OnHorizontalTableBorderKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			importer.TableReader.ProcessedBorder = importer.TableReader.TableProperties.Borders.InsideHorizontalBorder;
		}
		static void OnVerticalTableBorderKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			importer.TableReader.ProcessedBorder = importer.TableReader.TableProperties.Borders.InsideVerticalBorder;
		}
		static void OnNoTableBorderKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			if (importer.TableReader.ProcessedBorder == null)
				return;
			importer.TableReader.ProcessedBorder.Style = BorderLineStyle.None;
			importer.TableReader.ProcessedBorder = null;
		}
		static void OnBottomCellBorderKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			importer.TableReader.ProcessedBorder = importer.TableReader.CellProperties.Borders.BottomBorder;
		}
		static void OnTopCellBorderKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			importer.TableReader.ProcessedBorder = importer.TableReader.CellProperties.Borders.TopBorder;
		}
		static void OnLeftCellBorderKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			importer.TableReader.ProcessedBorder = importer.TableReader.CellProperties.Borders.LeftBorder;
		}
		static void OnRightCellBorderKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			importer.TableReader.ProcessedBorder = importer.TableReader.CellProperties.Borders.RightBorder;
		}
		static void OnUpperLeftToLowerRightBorderKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			importer.TableReader.ProcessedBorder = importer.TableReader.CellProperties.Borders.TopLeftDiagonalBorder;
		}
		static void OnUpperRightToLowerLeftBorderKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			importer.TableReader.ProcessedBorder = importer.TableReader.CellProperties.Borders.TopRightDiagonalBorder;
		}
		static void OnCellTextTopAlignmentKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			importer.TableReader.CellProperties.VerticalAlignment = VerticalAlignment.Top;
		}
		static void OnCellTextCenterVerticalAlignmentKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			importer.TableReader.CellProperties.VerticalAlignment = VerticalAlignment.Center;
		}
		static void OnCellTextBottomAlignmentKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			importer.TableReader.CellProperties.VerticalAlignment = VerticalAlignment.Bottom;
		}
		static void OnCellLeftToRightTopToBottomTextDirectionKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			importer.TableReader.CellProperties.TextDirection = TextDirection.LeftToRightTopToBottom;
		}
		static void OnCellTopToBottomRightToLeftTextDirectionKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			importer.TableReader.CellProperties.TextDirection = TextDirection.TopToBottomRightToLeft;
		}
		static void OnCellBottomToTopLeftToRightTextDirectionKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			importer.TableReader.CellProperties.TextDirection = TextDirection.BottomToTopLeftToRight;
		}
		static void OnCellLeftToRightTopToBottomVerticalTextDirectionKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			importer.TableReader.CellProperties.TextDirection = TextDirection.LeftToRightTopToBottomRotated;
		}
		static void OnCellTopToBottomRightToLeftVerticalTextDirectionKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			importer.TableReader.CellProperties.TextDirection = TextDirection.TopToBottomRightToLeftRotated;
		}
		static void OnCellBackgroundColor(RtfImporter importer, int parameterValue, bool hasParameter) {
			RtfColorCollection colorTable = importer.DocumentProperties.Colors;
			if (!hasParameter || parameterValue > colorTable.Count - 1)
				return;
			importer.TableReader.CellProperties.BackgroundColor = colorTable.GetRtfColorById(parameterValue);
		}
		static void OnCellForegroundColor(RtfImporter importer, int parameterValue, bool hasParameter) {
			RtfColorCollection colorTable = importer.DocumentProperties.Colors;
			if (!hasParameter || parameterValue > colorTable.Count - 1)
				return;
			importer.TableReader.CellProperties.ForegroundColor = colorTable.GetRtfColorById(parameterValue);
		}
		static void OnCellShading(RtfImporter importer, int parameterValue, bool hasParameter) {
			if (!hasParameter)
				return;
			importer.TableReader.CellProperties.ShadingPattern = GetShadingPattern(parameterValue);
		}
		static void OnTableAutoFitKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			int val = Math.Abs(parameterValue);
			if (val > 1)
				return;
			if (val == 1)
				importer.TableReader.TableProperties.TableLayout = TableLayoutType.Autofit;
			else
				importer.TableReader.TableProperties.TableLayout = TableLayoutType.Fixed;
		}
		static void OnApplyFirstRowConditionalFormattingKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			importer.TableReader.TableProperties.TableLook |= TableLookTypes.ApplyFirstRow;
		}
		static void OnApplyLastRowConditionalFormattingKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			importer.TableReader.TableProperties.TableLook |= TableLookTypes.ApplyLastRow;
		}
		static void OnApplyFirstColumnContitionalFormattingKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			importer.TableReader.TableProperties.TableLook |= TableLookTypes.ApplyFirstColumn;
		}
		static void OnApplyLastColumnConditionalFormattingKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			importer.TableReader.TableProperties.TableLook |= TableLookTypes.ApplyLastColumn;
		}
		static void OnDoNotApplyRowBandingConditionalFormattingKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			importer.TableReader.TableProperties.TableLook |= TableLookTypes.DoNotApplyRowBanding;
		}
		static void OnDoNotApplyColumnBandingConditionalFormattingKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			importer.TableReader.TableProperties.TableLook |= TableLookTypes.DoNotApplyColumnBanding;
		}
		static void OnRowBandSizeKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			if (parameterValue < 0 || !hasParameter)
				return;
			importer.TableReader.TableProperties.TableStyleRowBandSize = parameterValue;
		}
		static void OnColumnBandSizeKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			if (parameterValue < 0 || !hasParameter)
				return;
			importer.TableReader.TableProperties.TableStyleColBandSize = parameterValue;
		}
		#endregion
		#region Borders
		static void OnNoBorderKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			SetNoBorderType(importer, BorderLineStyle.Nil);
		}
		static void OnBorderSpaceKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			if (!hasParameter || parameterValue < 0)
				return;
			BorderBase border = importer.TableReader.ProcessedBorder;
			if (border != null)
				border.Offset = importer.UnitConverter.TwipsToModelUnits(parameterValue);
			BorderInfo paragraphBorder = importer.Position.ParagraphFormattingInfo.ProcessedBorder;
			if (paragraphBorder != null)
				paragraphBorder.Offset = importer.UnitConverter.TwipsToModelUnits(parameterValue);
		}
		static void AssignDefaultBorderWidth(BorderBase border) {
		}
		private static void SetBorderType(RtfImporter importer, BorderLineStyle style) {
			BorderBase border = importer.TableReader.ProcessedBorder;
			if (border != null) {
				AssignDefaultBorderWidth(border);
				border.Style = style;
			}
			BorderInfo paragraphBorder = importer.Position.ParagraphFormattingInfo.ProcessedBorder;
			if (paragraphBorder != null)
				paragraphBorder.Style = style;
		}
		private static void SetNoBorderType(RtfImporter importer, BorderLineStyle style) {
			BorderBase border = importer.TableReader.ProcessedBorder;
			if (border != null)
				border.Style = style;
			importer.TableReader.ProcessedBorder = null;
			BorderInfo paragraphBorder = importer.Position.ParagraphFormattingInfo.ProcessedBorder;
			if (paragraphBorder != null)
				paragraphBorder.Style = style;
			importer.Position.ParagraphFormattingInfo.ProcessedBorder = null;
		}
		static void OnSingleThicknessBorderTypeKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			SetBorderType(importer, BorderLineStyle.Single);
		}
		static void OnDoubleThicknessBorderTypeKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			BorderBase border = importer.TableReader.ProcessedBorder;
			if (border != null) {
				AssignDefaultBorderWidth(border);
				border.Width *= 2;
				border.Style = BorderLineStyle.Single;
			}
			BorderInfo paragraphBorder = importer.Position.ParagraphFormattingInfo.ProcessedBorder;
			if (paragraphBorder != null) {
				paragraphBorder.Width *= 2;
				paragraphBorder.Style = BorderLineStyle.Single;
			}
		}
		static void OnBorderWidthKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			BorderBase border = importer.TableReader.ProcessedBorder;
			BorderInfo paragraphBorder = importer.Position.ParagraphFormattingInfo.ProcessedBorder;
			if (border == null && paragraphBorder == null)
				return;
			const int maxBorderWidth = 255;
			int val = 0;
			if (parameterValue > 0)
				val = parameterValue > maxBorderWidth ? maxBorderWidth : parameterValue;
			else {
			}
			val = importer.UnitConverter.TwipsToModelUnits(val);
			if (border != null)
				border.Width = val > 0 ? val : 1;
			if (paragraphBorder != null)
				paragraphBorder.Width = val > 0 ? val : 1;
		}
		static void OnBorderColorKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			if (!hasParameter || parameterValue < 0)
				return;
			BorderBase border = importer.TableReader.ProcessedBorder;
			BorderInfo paragraphBorder = importer.Position.ParagraphFormattingInfo.ProcessedBorder;
			if (border == null && paragraphBorder == null)
				return;
			RtfColorCollection colorTable = importer.DocumentProperties.Colors;
			if (parameterValue > colorTable.Count - 1)
				return;
			if (border != null)
				border.Color = colorTable.GetRtfColorById(parameterValue);
			if (paragraphBorder != null)
				paragraphBorder.Color = colorTable.GetRtfColorById(parameterValue);
		}
		static void OnDoubleBorderTypeKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			SetBorderType(importer, BorderLineStyle.Double);
		}
		static void OnDottedBorderTypeKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			SetBorderType(importer, BorderLineStyle.Dotted);
		}
		static void OnDashedBorderTypeKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			SetBorderType(importer, BorderLineStyle.Dashed);
		}
		static void OnHairlineBorderTypeKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			BorderBase border = importer.TableReader.ProcessedBorder;
			if (border != null) {
				border.Style = BorderLineStyle.Single;
				border.Width = 1;
			}
			BorderInfo paragraphBorder = importer.Position.ParagraphFormattingInfo.ProcessedBorder;
			if (paragraphBorder != null) {
				paragraphBorder.Style = BorderLineStyle.Single;
				paragraphBorder.Width = 1;
			}
		}
		static void OnSmallDashedBorderTypeKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			SetBorderType(importer, BorderLineStyle.DashSmallGap);
		}
		static void OnDotDashedBorderTypeKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			SetBorderType(importer, BorderLineStyle.DotDash);
		}
		static void OnDotDotDashedBorderTypeKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			SetBorderType(importer, BorderLineStyle.DotDotDash);
		}
		static void OnInsetBorderTypeKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			SetBorderType(importer, BorderLineStyle.Inset);
		}
		static void OnNoneBorderTypeKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			SetNoBorderType(importer, BorderLineStyle.None);
		}
		static void OnNilBorderTypeKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			SetNoBorderType(importer, BorderLineStyle.Nil);
		}
		static void OnOutsetBorderTypeKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			SetBorderType(importer, BorderLineStyle.Outset);
		}
		static void OnTripletBorderTypeKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			SetBorderType(importer, BorderLineStyle.Triple);
		}
		static void OnSmallThickThinBorderTypeKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			SetBorderType(importer, BorderLineStyle.ThickThinSmallGap);
		}
		static void OnSmallThinThickThinBorderTypeKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			SetBorderType(importer, BorderLineStyle.ThinThickThinSmallGap);
		}
		static void OnMediumThickThinBorderTypeKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			SetBorderType(importer, BorderLineStyle.ThickThinMediumGap);
		}
		static void OnMediumThinThickBorderTypeKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			SetBorderType(importer, BorderLineStyle.ThinThickMediumGap);
		}
		static void OnMediumThinThickThinBorderTypeKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			SetBorderType(importer, BorderLineStyle.ThinThickThinMediumGap);
		}
		static void OnLargeThickThinBorderTypeKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			SetBorderType(importer, BorderLineStyle.ThickThinLargeGap);
		}
		static void OnLargeThinThickBorderTypeKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			SetBorderType(importer, BorderLineStyle.ThinThickLargeGap);
		}
		static void OnLargeThinThickThinBorderTypeKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			SetBorderType(importer, BorderLineStyle.ThinThickThinLargeGap);
		}
		static void OnWavyBorderTypeKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			SetBorderType(importer, BorderLineStyle.Wave);
		}
		static void OnDoubleWavyBorderTypeKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			SetBorderType(importer, BorderLineStyle.DoubleWave);
		}
		static void OnStripedBorderTypeKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			SetBorderType(importer, BorderLineStyle.DashDotStroked);
		}
		static void OnEmbossedBorderTypeKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			SetBorderType(importer, BorderLineStyle.ThreeDEmboss);
		}
		static void OnEngravedBorderTypeKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			SetBorderType(importer, BorderLineStyle.ThreeDEngrave);
		}
		static void OnSmallThinThickBorderTypeKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			SetBorderType(importer, BorderLineStyle.ThinThickSmallGap);
		}
		static void OnBorderArtIndex(RtfImporter importer, int parameterValue, bool hasParameter) {
			SetBorderType(importer, RtfArtBorderConverter.GetBorderLineStyle(parameterValue));
		}
		static void OnFrameBorderKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			BorderBase border = importer.TableReader.ProcessedBorder;
			BorderInfo paragraphBorder = importer.Position.ParagraphFormattingInfo.ProcessedBorder;
			if (border == null && paragraphBorder == null)
				return;
			if (!hasParameter || parameterValue != 0) {
				if (border != null)
					border.Frame = true;
				if (paragraphBorder != null)
					paragraphBorder.Frame = true;
			}
		}
		static void OnShadowedBorderTypeKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			BorderBase border = importer.TableReader.ProcessedBorder;
			BorderInfo paragraphBorder = importer.Position.ParagraphFormattingInfo.ProcessedBorder;
			if (border == null && paragraphBorder == null)
				return;
			if (!hasParameter || parameterValue != 0) {
				if (border != null)
					border.Shadow = true;
				if (paragraphBorder != null)
					paragraphBorder.Shadow = true;
			}
		}
		#endregion
		#endregion
	}
	#endregion
	#region SkipNestedTableDestination
	public class SkipNestedTableDestination : SkipDestination {
		#region CreateKeywordTable
		static readonly KeywordTranslatorTable keywordHT = CreateKeywordTable();
		static KeywordTranslatorTable CreateKeywordTable() {
			KeywordTranslatorTable table = new KeywordTranslatorTable();
			table.Add("par", OnParKeyword);
			return table;
		}
		#endregion
		static void OnParKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			importer.Destination = importer.StateManager.CreateDefaultDestination();
		}
		public SkipNestedTableDestination(RtfImporter rtfImporter)
			: base(rtfImporter) {
		}
		protected override KeywordTranslatorTable KeywordHT { get { return keywordHT; } }
		protected override DestinationBase CreateClone() {
			return new SkipNestedTableDestination(Importer);
		}
	}
	#endregion
	public class RtfOldListLevelInfo {
		bool skipNumbering;
		bool includeInformationFromPreviousLevel;
		CharacterFormattingBase characterProperties;
		ListLevelInfo listLevelProperties;
		string textBefore;
		string textAfter;
		int indent;
		public RtfOldListLevelInfo(DocumentModel documentModel) {
			this.characterProperties = new CharacterFormattingBase(documentModel.MainPieceTable, documentModel, CharacterFormattingInfoCache.DefaultItemIndex, CharacterFormattingOptionsCache.EmptyCharacterFormattingOptionIndex);
			this.characterProperties.BeginUpdate();
			this.characterProperties.FontName = "Times New Roman";
			this.characterProperties.DoubleFontSize = 24;
			this.characterProperties.ResetUse(CharacterFormattingOptions.Mask.UseAll);
			this.characterProperties.EndUpdate();
			this.listLevelProperties = new ListLevelInfo();
			this.textBefore = String.Empty;
			this.textAfter = String.Empty;
		}
		public bool SkipNumbering { get { return skipNumbering; } set { skipNumbering = value; } }
		public bool IncludeInformationFromPreviousLevel { get { return includeInformationFromPreviousLevel; } set { includeInformationFromPreviousLevel = value; } }
		public CharacterFormattingBase CharacterProperties { get { return characterProperties; } }
		public ListLevelInfo ListLevelProperties { get { return listLevelProperties; } }
		public string TextBefore {
			get { return textBefore; }
			set { textBefore = value; }
		}
		public string TextAfter { get { return textAfter; } set { textAfter = value; } }
		public int Indent { get { return indent; } set { indent = value; } }
		public virtual void CopyFrom(RtfOldListLevelInfo source) {
			SkipNumbering = source.SkipNumbering;
			IncludeInformationFromPreviousLevel = source.IncludeInformationFromPreviousLevel;
			CharacterProperties.CopyFrom(source.CharacterProperties);
			ListLevelProperties.CopyFrom(source.ListLevelProperties);
			TextBefore = source.TextBefore;
			TextAfter = source.TextAfter;
			Indent = source.Indent;
		}
		public virtual RtfOldListLevelInfo Clone() {
			RtfOldListLevelInfo result = new RtfOldListLevelInfo((DocumentModel)CharacterProperties.DocumentModel);
			result.CopyFrom(this);
			return result;
		}
	}
	public class RtfOldListLevelInfoCollection {
		readonly List<RtfOldListLevelInfo> listLevelInfo;
		readonly DocumentModel documentModel;
		public RtfOldListLevelInfoCollection(DocumentModel documentModel) {
			Guard.ArgumentNotNull(documentModel, "documentModel");
			this.documentModel = documentModel;
			this.listLevelInfo = new List<RtfOldListLevelInfo>();
		}
		protected DocumentModel DocumentModel { get { return documentModel; } }
		public RtfOldListLevelInfo this[int index] {
			get {
				if (listLevelInfo.Count <= index)
					EnsureLevelIndex(index);
				return listLevelInfo[index];
			}
		}
		protected virtual void EnsureLevelIndex(int index) {
			int count = listLevelInfo.Count;
			for (int i = count; i <= index; i++) {
				listLevelInfo.Add(new RtfOldListLevelInfo(DocumentModel));
			}
		}
	}
	#region BorderWidthType
	public enum BorderWidthType {
		None,
		Single,
		Double
	}
	#endregion
	#region RtfBorderFormattingInfo
	public class RtfBorderFormattingInfo {
		#region Fields
		const int DefaultBorderWidth = 3; 
		readonly BorderInfo borderInfo;
		BorderWidthType borderWidthType;
		#endregion
		public RtfBorderFormattingInfo() {
			this.borderInfo = new BorderInfo();
		}
		#region Properties
		public BorderInfo Border { get { return borderInfo; } }
		public BorderWidthType BorderWidthType { get { return borderWidthType; } set { borderWidthType = value; } }
		#endregion
		public void InitializeBorderInfo(DocumentModel documentModel) {
			this.borderInfo.CopyFrom(documentModel.Cache.BorderInfoCache[0]);
			this.borderWidthType = BorderWidthType.None;
		}
		public void CopyFrom(RtfBorderFormattingInfo info) {
			Border.CopyFrom(info.Border);
			BorderWidthType = info.BorderWidthType;
		}
		public RtfBorderFormattingInfo Clone() {
			RtfBorderFormattingInfo result = new RtfBorderFormattingInfo();
			result.CopyFrom(this);
			return result;
		}
		public void EnsureBorderWidthNotNil() {
			if (Border.Width == 0)
				Border.Width = DefaultBorderWidth;
		}
	}
	#endregion
	#region RtfInputPositionState
	public class RtfInputPositionState {
		RtfFormattingInfo rtfFormattingInfo;
		RtfParagraphFormattingInfo paragraphFormattingInfo;
		RtfSectionFormattingInfo sectionFormattingInfo;
		RtfBorderFormattingInfo borderFormattingInfo;
		CharacterFormattingBase characterFormatting;
		RtfOldListLevelInfo oldListLevelInfo;
		RtfOldListLevelInfoCollection oldListLevelInfoCollection;
		int characterStyleIndex;
		NumberingListIndex currentOldSimpleListIndex;
		NumberingListIndex currentOldMultiLevelListIndex;
		int currentOldListLevelNumber;
		RtfFieldInfo fieldInfo;
		bool currentOldListSkipNumbering;
		bool currentOldSimpleList;
		int tableStyleIndex;
		string doubleByteCharactersFontName;
		string lowAnsiCharactersFontName;
		string highAnsiCharactersFontName;
		RtfFontType fontType;
		FrameProperties frameProperties;
		public RtfFormattingInfo RtfFormattingInfo { get { return rtfFormattingInfo; } set { rtfFormattingInfo = value; } }
		public RtfParagraphFormattingInfo ParagraphFormattingInfo { get { return paragraphFormattingInfo; } set { paragraphFormattingInfo = value; } }
		public RtfSectionFormattingInfo SectionFormattingInfo { get { return sectionFormattingInfo; } set { sectionFormattingInfo = value; } }
		public RtfBorderFormattingInfo BorderFormattingInfo { get { return borderFormattingInfo; } set { borderFormattingInfo = value; } }
		public CharacterFormattingBase CharacterFormatting { get { return characterFormatting; } set { characterFormatting = value; } }
		public RtfOldListLevelInfo OldListLevelInfo { get { return oldListLevelInfo; } set { oldListLevelInfo = value; } }
		public RtfOldListLevelInfoCollection OldListLevelInfoCollection { get { return oldListLevelInfoCollection; } set { oldListLevelInfoCollection = value; } }
		public int CharacterStyleIndex { get { return characterStyleIndex; } set { characterStyleIndex = value; } }
		public NumberingListIndex CurrentOldMultiLevelListIndex { get { return currentOldMultiLevelListIndex; } set { currentOldMultiLevelListIndex = value; } }
		public NumberingListIndex CurrentOldSimpleListIndex { get { return currentOldSimpleListIndex; } set { currentOldSimpleListIndex = value; } }
		public int CurrentOldListLevelNumber { get { return currentOldListLevelNumber; } set { currentOldListLevelNumber = value; } }
		public bool CurrentOldListSkipNumbering { get { return currentOldListSkipNumbering; } set { currentOldListSkipNumbering = value; } }
		public bool CurrentOldSimpleList { get { return currentOldSimpleList; } set { currentOldSimpleList = value; } }
		public RtfFieldInfo FieldInfo { get { return fieldInfo; } set { fieldInfo = value; } }
		public int TableStyleIndex { get { return tableStyleIndex; } set { tableStyleIndex = value; } }
		public string DoubleByteCharactersFontName { get { return doubleByteCharactersFontName; } set { doubleByteCharactersFontName = value; } }
		public string LowAnsiCharactersFontName { get { return lowAnsiCharactersFontName; } set { lowAnsiCharactersFontName = value; } }
		public string HighAnsiCharactersFontName { get { return highAnsiCharactersFontName; } set { highAnsiCharactersFontName = value; } }
		public RtfFontType FontType { get { return fontType; } set { fontType = value; } }
		public FrameProperties FrameProperties { get { return frameProperties; } set { frameProperties = value; } }
	}
	#endregion
	#region RtfFontType
	public enum RtfFontType {
		Undefined,
		DoubleByteCharactersFont,
		LowAnsiCharactersFont,
		HighAnsiCharactersFont
	}
	#endregion
	#region RtfInputPosition
	public class RtfInputPosition : InputPosition {
		#region Fields
		RtfFormattingInfo rtfFormattingInfo;
		RtfParagraphFormattingInfo paragraphFormattingInfo;
		RtfSectionFormattingInfo sectionFormattingInfo;
		RtfBorderFormattingInfo borderFormattingInfo;
		RtfOldListLevelInfo oldListLevelInfo;
		RtfOldListLevelInfoCollection oldListLevelInfoCollection;
		bool currentOldListSkipNumbering;
		bool currentOldSimpleList;
		RtfFieldInfo fieldInfo;
		NumberingListIndex currentOldMultiLevelListIndex;
		NumberingListIndex currentOldSimpleListIndex;
		int currentOldListLevelNumber;
		int tableStyleIndex;
		string doubleByteCharactersFontName;
		string lowAnsiCharactersFontName;
		string highAnsiCharactersFontName;
		RtfFontType fontType;
		bool useLowAnsiCharactersFontName;
		bool useHighAnsiCharactersFontName;
		bool useDoubleByteCharactersFontName;
		ParagraphFrameFormattingInfo paragraphFrameFormattingInfo;
		#endregion
		public RtfInputPosition(PieceTable pieceTable)
			: base(pieceTable) {
			this.rtfFormattingInfo = new RtfFormattingInfo();
			this.fieldInfo = new RtfFieldInfo();
			this.CharacterFormatting.BeginUpdate();
			this.CharacterFormatting.FontName = "Times New Roman";
			this.CharacterFormatting.DoubleFontSize = 24;
			this.CharacterFormatting.ResetUse(CharacterFormattingOptions.Mask.UseAll);
			this.CharacterFormatting.EndUpdate();
			ParagraphFormattingInfo = new RtfParagraphFormattingInfo();
			SectionFormattingInfo = new RtfSectionFormattingInfo(pieceTable.DocumentModel);
			SectionFormattingInfo.InitializeDefault(pieceTable.DocumentModel);
			BorderFormattingInfo = new RtfBorderFormattingInfo();
			BorderFormattingInfo.InitializeBorderInfo(pieceTable.DocumentModel);
			OldListLevelInfoCollection = new RtfOldListLevelInfoCollection(pieceTable.DocumentModel);
			currentOldMultiLevelListIndex = NumberingListIndex.ListIndexNotSetted;
			currentOldSimpleListIndex = NumberingListIndex.ListIndexNotSetted;
		}
		protected internal RtfFormattingInfo RtfFormattingInfo { get { return rtfFormattingInfo; } set { rtfFormattingInfo = value; } }
		protected internal RtfParagraphFormattingInfo ParagraphFormattingInfo { get { return paragraphFormattingInfo; } set { paragraphFormattingInfo = value; } }
		protected internal RtfSectionFormattingInfo SectionFormattingInfo { get { return sectionFormattingInfo; } set { sectionFormattingInfo = value; } }
		protected internal RtfBorderFormattingInfo BorderFormattingInfo { get { return borderFormattingInfo; } set { borderFormattingInfo = value; } }
		protected internal RtfOldListLevelInfo OldListLevelInfo { get { return oldListLevelInfo; } set { oldListLevelInfo = value; } }
		protected internal RtfOldListLevelInfoCollection OldListLevelInfoCollection { get { return oldListLevelInfoCollection; } set { oldListLevelInfoCollection = value; } }
		protected internal RtfFieldInfo FieldInfo { get { return fieldInfo; } set { fieldInfo = value; } }
		public NumberingListIndex CurrentOldMultiLevelListIndex { get { return currentOldMultiLevelListIndex; } set { currentOldMultiLevelListIndex = value; } }
		public NumberingListIndex CurrentOldSimpleListIndex { get { return currentOldSimpleListIndex; } set { currentOldSimpleListIndex = value; } }
		public bool CurrentOldListSkipNumbering { get { return currentOldListSkipNumbering; } set { currentOldListSkipNumbering = value; } }
		public bool CurrentOldSimpleList { get { return currentOldSimpleList; } set { currentOldSimpleList = value; } }
		public int CurrentOldListLevelNumber { get { return currentOldListLevelNumber; } set { currentOldListLevelNumber = value; } }
		public int TableStyleIndex { get { return tableStyleIndex; } set { tableStyleIndex = value; } }
		public string DoubleByteCharactersFontName { get { return doubleByteCharactersFontName; } set { doubleByteCharactersFontName = value; } }
		public string LowAnsiCharactersFontName { get { return lowAnsiCharactersFontName; } set { lowAnsiCharactersFontName = value; } }
		public string HighAnsiCharactersFontName { get { return highAnsiCharactersFontName; } set { highAnsiCharactersFontName = value; } }
		public RtfFontType FontType { get { return fontType; } set { fontType = value; } }
		public bool UseLowAnsiCharactersFontName { get { return useLowAnsiCharactersFontName; } set { useLowAnsiCharactersFontName = value; } }
		public bool UseHighAnsiCharactersFontName { get { return useHighAnsiCharactersFontName; } set { useHighAnsiCharactersFontName = value; } }
		public bool UseDoubleByteCharactersFontName { get { return useDoubleByteCharactersFontName; } set { useHighAnsiCharactersFontName = value; } }
		protected internal ParagraphFrameFormattingInfo ParagraphFrameFormattingInfo { get { return paragraphFrameFormattingInfo; } set { paragraphFrameFormattingInfo = value; } }
		public RtfInputPositionState GetState() {
			RtfInputPositionState result = new RtfInputPositionState();
			result.BorderFormattingInfo = BorderFormattingInfo.Clone();
			result.CharacterFormatting = CharacterFormatting.Clone();
			result.CharacterStyleIndex = CharacterStyleIndex;
			result.ParagraphFormattingInfo = ParagraphFormattingInfo.Clone();
			result.RtfFormattingInfo = RtfFormattingInfo.Clone();
			result.SectionFormattingInfo = SectionFormattingInfo;
			result.OldListLevelInfoCollection = OldListLevelInfoCollection;
			result.OldListLevelInfo = OldListLevelInfo != null ? OldListLevelInfo.Clone() : null;
			result.CurrentOldMultiLevelListIndex = CurrentOldMultiLevelListIndex;
			result.CurrentOldSimpleListIndex = CurrentOldSimpleListIndex;
			result.CurrentOldSimpleList = CurrentOldSimpleList;
			result.CurrentOldListLevelNumber = CurrentOldListLevelNumber;
			result.CurrentOldListSkipNumbering = CurrentOldListSkipNumbering;
			result.TableStyleIndex = TableStyleIndex;
			result.FontType = FontType;
			result.DoubleByteCharactersFontName = DoubleByteCharactersFontName;
			result.LowAnsiCharactersFontName = LowAnsiCharactersFontName;
			result.HighAnsiCharactersFontName = HighAnsiCharactersFontName;
			return result;
		}
		public void SetState(RtfInputPositionState state) {
			BorderFormattingInfo = state.BorderFormattingInfo;
			CharacterFormatting.CopyFrom(state.CharacterFormatting);
			CharacterStyleIndex = state.CharacterStyleIndex;
			ParagraphFormattingInfo = state.ParagraphFormattingInfo;
			RtfFormattingInfo = state.RtfFormattingInfo;
			SectionFormattingInfo = state.SectionFormattingInfo;
			oldListLevelInfoCollection = state.OldListLevelInfoCollection;
			OldListLevelInfo = state.OldListLevelInfo;
			currentOldMultiLevelListIndex = state.CurrentOldMultiLevelListIndex;
			currentOldSimpleListIndex = state.CurrentOldSimpleListIndex;
			CurrentOldListLevelNumber = state.CurrentOldListLevelNumber;
			CurrentOldListSkipNumbering = state.CurrentOldListSkipNumbering;
			CurrentOldSimpleList = state.CurrentOldSimpleList;
			TableStyleIndex = state.TableStyleIndex;
			FontType = state.FontType;
			DoubleByteCharactersFontName = state.DoubleByteCharactersFontName;
			LowAnsiCharactersFontName = state.LowAnsiCharactersFontName;
			HighAnsiCharactersFontName = state.HighAnsiCharactersFontName;
			RecalcUseAssociatedProperties();
		}
		protected internal void SetFont(string fontName) {
			if (FontType == RtfFontType.Undefined) {
				CharacterFormatting.FontName = fontName;
				ResetUseAssociatedProperties();
			}
			else {
				if (FontType == RtfFontType.DoubleByteCharactersFont)
					DoubleByteCharactersFontName = fontName;
				else if (FontType == RtfFontType.HighAnsiCharactersFont)
					HighAnsiCharactersFontName = fontName;
				else if (FontType == RtfFontType.LowAnsiCharactersFont)
					LowAnsiCharactersFontName = fontName;
				RecalcUseAssociatedProperties();
				FontType = RtfFontType.Undefined;
			}
		}
		protected internal void ResetUseAssociatedProperties() {
			this.highAnsiCharactersFontName = String.Empty;
			this.lowAnsiCharactersFontName = String.Empty;
			this.doubleByteCharactersFontName = String.Empty;
			this.useHighAnsiCharactersFontName = false;
			this.useLowAnsiCharactersFontName = false;
			this.useDoubleByteCharactersFontName = false;
		}
		protected internal void RecalcUseAssociatedProperties() {
			this.useHighAnsiCharactersFontName = !string.IsNullOrEmpty(this.highAnsiCharactersFontName);
			this.useLowAnsiCharactersFontName = !string.IsNullOrEmpty(this.lowAnsiCharactersFontName);
			this.useDoubleByteCharactersFontName = !string.IsNullOrEmpty(this.doubleByteCharactersFontName);
		}
	}
	#endregion
	#region RtfTableCollection
	public class RtfTableCollection : List<RtfTable> {
		public RtfTableCollection(IEnumerable<RtfTable> tables)
			: base(tables) {
		}
		public RtfTableCollection() {
		}
		public RtfTable First { get { return Count > 0 ? this[0] : null; } }
		public RtfTable Last { get { return Count > 0 ? this[Count - 1] : null; } }
	}
	#endregion
	#region ReadOutBorder
	public class ReadOutBorder {
		BorderBase border;
		public BorderBase Border { get { return border; } set { border = value; } }
		public void Reset() {
			this.border = null;
		}
		public void FinishBorder(RtfImporter importer) {
			if (Border == null)
				return;
			RtfBorderFormattingInfo borderFormattingInfo = importer.Position.BorderFormattingInfo;
			Border.CopyFrom(borderFormattingInfo.Border);
			borderFormattingInfo.InitializeBorderInfo(importer.DocumentModel);
			Reset();
		}
	}
	#endregion
	#region RtfTableCellPropertiesCollection
	public class RtfTableCellPropertiesCollection : List<RtfTableCellProperties> {
		public RtfTableCellProperties Last {
			get {
				if (Count > 0)
					return this[Count - 1];
				return null;
			}
		}
	}
	#endregion
	#region RtfTableState
	public class RtfTableState {
		RtfTable table;
		RtfTableProperties tableProperties;
		RtfTableRowProperties rowProperties;
		RtfTableCellPropertiesCollection cellPropertiesCollection;
		public RtfTableState(RtfTable table, RtfTableReader reader) {
			this.table = table;
			this.tableProperties = reader.TableProperties;
			this.rowProperties = reader.RowProperties;
			this.cellPropertiesCollection = reader.CellPropertiesCollection;
		}
		public RtfTable Table { get { return table; } }
		public RtfTableProperties TableProperties { get { return tableProperties; } }
		public RtfTableRowProperties RowProperties { get { return rowProperties; } }
		public RtfTableCellPropertiesCollection CellPropertiesCollection { get { return cellPropertiesCollection; } }
	}
	#endregion
	public class RtfParentCellMap : Dictionary<RtfTableCell, RtfTableCollection> {
	}
	#region RtfTableReader
	public class RtfTableReader : ICellPropertiesOwner {
		#region Fields
		RtfTableReaderStateBase state;
		readonly RtfImporter importer;
		readonly Stack<RtfTableState> tableStack;
		readonly RtfTableController tableController;
		readonly RtfTableCollection tables;
		RtfTableProperties tableProperties;
		RtfTableRowProperties rowProperties;
		RtfTableCellPropertiesCollection cellPropertiesCollection;
		RtfTableCellProperties cellProperties;
		bool isNestedTableProperetiesReading = false;
		RtfParentCellMap parentCellMap;
		BorderBase processedBorder;
		#endregion
		public RtfTableReader(RtfImporter importer) {
			this.state = new NoTableRtfTableReaderState(this);
			this.importer = importer;
			this.tableStack = new Stack<RtfTableState>();
			this.tables = new RtfTableCollection();
			this.tableController = CreateTableController();
			this.parentCellMap = new RtfParentCellMap();
			ResetProperties();
		}
		#region Properties
		internal RtfTableReaderStateBase State { get { return state; } }
		internal RtfParentCellMap ParentCellMap { get { return parentCellMap; } }
		public RtfImporter Importer { get { return importer; } }
		protected DocumentModel DocumentModel { get { return Importer.DocumentModel; } }
		protected internal Stack<RtfTableState> TableStack { get { return tableStack; } }
		public RtfTableController TableController { get { return tableController; } }
		public RtfTableCollection Tables { get { return tables; } }
		public BorderBase ProcessedBorder { get { return processedBorder; } set { processedBorder = value; } }
		public bool IsNestedTableProperetiesReading { get { return isNestedTableProperetiesReading; } }
		public RtfTableProperties TableProperties {
			get {
				if (tableProperties == null)
					tableProperties = new RtfTableProperties(Importer.PieceTable);
				return tableProperties;
			}
		}
		public RtfTableRowProperties RowProperties {
			get {
				if (rowProperties == null)
					rowProperties = new RtfTableRowProperties(Importer.PieceTable);
				return rowProperties;
			}
		}
		public RtfTableCellProperties CellProperties {
			get {
				if (cellProperties == null)
					CreateCellProperties();
				return cellProperties;
			}
		}
		protected internal RtfTableCellPropertiesCollection CellPropertiesCollection { get { return cellPropertiesCollection; } }
		#endregion
		protected internal virtual void RestoreProperties(RtfTableState state) {
			this.tableProperties = state.TableProperties;
			this.rowProperties = state.RowProperties;
			this.cellPropertiesCollection = state.CellPropertiesCollection;
			if (cellPropertiesCollection.Count > 0)
				this.cellProperties = cellPropertiesCollection.Last;
			else
				CreateCellProperties();
		}
		protected internal virtual void ResetProperties() {
			this.tableProperties = null;
			this.rowProperties = null;
			this.cellPropertiesCollection = new RtfTableCellPropertiesCollection();
			this.cellProperties = null;
			this.processedBorder = null;
		}
		protected internal virtual void CreateCellProperties() {
			this.cellProperties = new RtfTableCellProperties(Importer.PieceTable, this);
		}
		protected internal virtual RtfTableController CreateTableController() {
			return new RtfTableController(this);
		}
		public virtual void OnStartNestedTableProperties() {
			isNestedTableProperetiesReading = true;
			State.OnStartNestedTableProperties();
		}
		public virtual void OnEndParagraph() {
			State.OnEndParagraph(Importer.Position.ParagraphFormattingInfo);
		}
		public virtual void OnEndRow() {
			State.OnEndRow();
		}
		public virtual void OnEndCell() {
			State.OnEndCell();
		}
		public virtual void OnEndNestedRow() {
			isNestedTableProperetiesReading = false;
			State.OnEndNestedRow();
		}
		public virtual void OnEndNestedCell() {
			State.OnEndNestedCell();
		}
		public virtual void OnTableRowDefaults() {
			State.OnTableRowDefaults();
			ResetProperties();
		}
		public virtual void OnCellxProperty(int value) {
			CellProperties.Right = value;
			if (cellPropertiesCollection.Count == 0 || cellPropertiesCollection.Last != CellProperties)
				this.cellPropertiesCollection.Add(CellProperties);
			CreateCellProperties();
			ProcessedBorder = null;
		}
		public virtual void ResetState() {
			ChangeState(new NoTableRtfTableReaderState(this));
			TableController.Reset();
		}
		public virtual void ChangeState(RtfTableReaderStateBase newState) {
			this.state = newState;
		}
		public virtual void InsertTables() {
#if DEBUGTEST && !SL
			DocumentModel.ActivePieceTable.ForceCheckTablesIntegrity();
#endif
			if (Tables.Count == 0)
				return;
			if (DocumentModel.DocumentCapabilities.TablesAllowed) {
				RtfTableConverter converter = new RtfTableConverter(this);
				converter.ConvertTables(Tables, Importer.Options.CopySingleCellAsText);
			}
			Tables.Clear();
#if DEBUGTEST
			CheckTablesIntegrity(Importer.PieceTable);
#endif
		}
		void CheckTablesIntegrity(PieceTable pieceTable) {
			TableCollection tables = pieceTable.Tables;
			for (int i = 0; i < tables.Count; i++) {
				Table table = tables[i];
				Debug.Assert(table.Rows.Count > 0);
				if (table.ParentCell != null) {
					Table rootTable = table.ParentCell.Row.Table;
					Debug.Assert(tables.Contains(rootTable));
				}
				for (int j = 0; j < table.Rows.Count; j++) {
					TableRow row = table.Rows[j];
					Debug.Assert(table == row.Table);
					Debug.Assert(row.Cells.Count > 0);
					for (int k = 0; k < row.Cells.Count; k++) {
						TableCell cell = row.Cells[k];
						Debug.Assert(row == cell.Row);
						Debug.Assert(cell.StartParagraphIndex >= ParagraphIndex.Zero);
						Debug.Assert(cell.EndParagraphIndex >= cell.StartParagraphIndex);
						if (k > 0) {
							ParagraphIndex prevCellParIndex = row.Cells[k - 1].EndParagraphIndex;
							Debug.Assert(cell.StartParagraphIndex == prevCellParIndex + 1);
						}
						else if (j > 0) {
							ParagraphIndex prevCellParIndex = table.Rows[j - 1].Cells.Last.EndParagraphIndex;
							Debug.Assert(cell.StartParagraphIndex == prevCellParIndex + 1);
						}
						Debug.Assert(cell.Properties.ColumnSpan > 0);
					}
				}
			}
		}
		#region ICellPropertiesOwner Members
		IndexChangedHistoryItemCore<DocumentModelChangeActions> ICellPropertiesOwner.CreateCellPropertiesChangedHistoryItem(TableCellProperties properties) {
			return new IndexChangedHistoryItem<DocumentModelChangeActions>(properties.PieceTable, properties);
		}
		#endregion
	}
	#endregion
	#region RtfTableReaderStateBase (abstract class)
	public abstract class RtfTableReaderStateBase {
		readonly RtfTableReader reader;
		protected RtfTableReaderStateBase(RtfTableReader reader) {
			Guard.ArgumentNotNull(reader, "reader");
			this.reader = reader;
		}
		public RtfTableReader Reader { get { return reader; } }
		protected RtfImporter Importer { get { return Reader.Importer; } }
		protected DocumentModel DocumentModel { get { return Importer.DocumentModel; } }
		protected RtfTableController TableController { get { return Reader.TableController; } }
		public abstract void OnStartNestedTableProperties();
		public abstract void OnEndParagraph(RtfParagraphFormattingInfo paragraphFormattingInfo);
		public abstract void OnEndRow();
		public abstract void OnEndCell();
		public abstract void OnEndNestedRow();
		public abstract void OnEndNestedCell();
		public abstract void OnTableRowDefaults();
	}
	#endregion
	#region NoTableRtfTableReaderState
	public class NoTableRtfTableReaderState : RtfTableReaderStateBase {
		const int DefaultNestingLevel = 1;
		public NoTableRtfTableReaderState(RtfTableReader reader)
			: base(reader) {
		}
		public override void OnEndParagraph(RtfParagraphFormattingInfo paragraphFormattingInfo) {
			if (!paragraphFormattingInfo.InTableParagraph)
				return;
			TableRtfTableManagerState newState = ChangeState();
			newState.OnEndParagraph(paragraphFormattingInfo);
		}
		public override void OnStartNestedTableProperties() {
			RtfImporter.ThrowInvalidRtfFile();
		}
		public override void OnEndRow() {
			RtfImporter.ThrowInvalidRtfFile();
		}
		public override void OnEndCell() {
			TableRtfTableManagerState newState = ChangeState();
			newState.OnEndCell();
		}
		public override void OnEndNestedRow() {
			RtfImporter.ThrowInvalidRtfFile();
		}
		public override void OnEndNestedCell() {
			TableRtfTableManagerState newState = ChangeState();
			newState.OnEndNestedCell();
		}
		public override void OnTableRowDefaults() {
		}
		protected TableRtfTableManagerState ChangeState() {
			TableRtfTableManagerState newState = new TableRtfTableManagerState(Reader);
			Reader.ChangeState(newState);
			return newState;
		}
	}
	#endregion
	#region TableRtfTableManagerState
	public class TableRtfTableManagerState : RtfTableReaderStateBase {
		public TableRtfTableManagerState(RtfTableReader reader)
			: base(reader) {
			Reader.TableController.CreateCurrentTable();
		}
		public override void OnEndParagraph(RtfParagraphFormattingInfo paragraphFormattingInfo) {
			if (IsParagraphInTable(paragraphFormattingInfo) || IsCurrentTableNotComplete())
				OnEndInTableParagraph(paragraphFormattingInfo.NestingLevel);
			else {
				ValidateCurrentTable();
				Reader.ResetState();
			}
		}
		protected internal virtual void ValidateCurrentTable() {
			RtfTable currentTable = TableController.CurrentTable;
			if (currentTable.Rows.Count == 0) {
				Reader.Tables.Remove(currentTable);
				return;
			}
			RtfTableRow lastRow = currentTable.Rows.Last;
			if (lastRow.Cells.Count > 0)
				return;
			currentTable.Rows.Remove(lastRow);
			ValidateCurrentTable();
		}
		protected internal virtual void OnEndInTableParagraph(int nestingLevel) {
			RtfTableController tableController = Reader.TableController;
			if (tableController.CurrentTable.NestingLevel != nestingLevel)
				tableController.ChangeTable(nestingLevel);
			ParagraphIndex index = Importer.Position.ParagraphIndex;
			tableController.RowController.CellController.SetParagraphIndex(index);
		}
		bool IsParagraphInTable(RtfParagraphFormattingInfo paragraphFormattingInfo) {
			return paragraphFormattingInfo.InTableParagraph || paragraphFormattingInfo.NestingLevel > 0;
		}
		bool IsCurrentTableNotComplete() {
			return Reader.TableController.RowController.IsCurrentRowNotComplete() ||
				(Reader.TableController.RowController.IsCurrentRowValid() &&
				Reader.TableController.RowController.CurrentRow.Cells.Count < Reader.CellPropertiesCollection.Count);
		}
		public override void OnEndRow() {
			if (TableController.CurrentTable.NestingLevel > 1)
				RtfImporter.ThrowInvalidRtfFile();
			OnEndRowCore();
		}
		protected internal virtual void OnEndRowCore() {
			RtfTableRowController rowController = Reader.TableController.RowController;
			rowController.FinishRow();
			rowController.StartNewRow();
		}
		public override void OnEndCell() {
			OnEndCellCore(1);
		}
		protected internal virtual void OnEndCellCore(int nestingLevel) {
			RtfTableCellController cellController = Reader.TableController.RowController.CellController;
			OnEndInTableParagraph(nestingLevel);
			cellController.FinishCell();
			cellController.StartNewCell();
		}
		public override void OnEndNestedRow() {
			if (TableController.CurrentTable.NestingLevel == 1)
				RtfImporter.ThrowInvalidRtfFile();
			OnEndRowCore();
		}
		public override void OnEndNestedCell() {
			OnEndCellCore(Importer.Position.ParagraphFormattingInfo.NestingLevel);
		}
		public override void OnStartNestedTableProperties() {
		}
		public override void OnTableRowDefaults() {
			RtfTable currentTable = Reader.TableController.CurrentTable;
			if (!Reader.IsNestedTableProperetiesReading && currentTable != null && currentTable.NestingLevel > 1)
				TableController.ChangeTable(1);
		}
	}
	#endregion
	#region RtfTableController
	public class RtfTableController {
		readonly RtfTableReader reader;
		RtfTable currentTable;
		RtfTableRowController rowController;
		public RtfTableController(RtfTableReader reader) {
			this.reader = reader;
			this.rowController = CreateRowController();
		}
		public RtfTableReader Reader { get { return reader; } }
		public RtfTable CurrentTable { get { return currentTable; } }
		public RtfTableRowController RowController { get { return rowController; } }
		protected internal virtual RtfTableRowController CreateRowController() {
			return new RtfTableRowController(this);
		}
		public virtual void ChangeTable(int nestingLevel) {
			int depth = nestingLevel - CurrentTable.NestingLevel;
			if (depth > 0)
				CreateNestedTable(depth);
			else if (depth < 0)
				PopParentTable(depth);
			else {
				FinishTable();
				CreateCurrentTable();
			}
		}
		public virtual void PopParentTable(int depth) {
			int count = Math.Abs(depth);
			RtfTableState state = null;
			for (int i = 0; i < count && Reader.TableStack.Count > 0; i++)
				state = Reader.TableStack.Pop();
			if (state == null)
				return;
			currentTable = state.Table;
			Reader.RestoreProperties(state);
			RowController.AssignLastRowAsCurrent();
		}
		protected internal virtual void CreateNestedTable(int depth) {
			for (int i = 0; i < depth; i++) {
				FinishTable();
				Reader.TableStack.Push(new RtfTableState(currentTable, Reader));
				CreateCurrentTable();
			}
		}
		protected internal virtual void CreateCurrentTable() {
			this.currentTable = new RtfTable(reader.Importer);
			RtfTableCell currentCell = RowController.CellController.CurrentCell;
			this.currentTable.ParentCell = currentCell;
			if (currentCell != null) {
				RtfTableCollection tables;
				if (!Reader.ParentCellMap.TryGetValue(currentCell, out tables)) {
					tables = new RtfTableCollection();
					Reader.ParentCellMap.Add(currentCell, tables);
				}
				tables.Add(currentTable);
			}
			Reader.Tables.Add(currentTable);
			RowController.StartNewRow();
		}
		public virtual void FinishTable() {
			RowController.CellController.FinishCell();
			RowController.FinishRowCore();
		}
		public virtual void Reset() {
			currentTable = null;
			RowController.Reset();
		}
	}
	#endregion
	#region RtfTableRowController
	public class RtfTableRowController {
		readonly RtfTableController tableController;
		RtfTableRow currentRow;
		RtfTableCellController cellController;
		public RtfTableRowController(RtfTableController tableController) {
			this.tableController = tableController;
			this.cellController = CreateCellController();
		}
		public RtfTableRow CurrentRow { get { return currentRow; } }
		public RtfTableCellController CellController { get { return cellController; } }
		protected internal RtfTableController TableController { get { return tableController; } }
		protected internal virtual RtfTableCellController CreateCellController() {
			return new RtfTableCellController(this);
		}
		public virtual void StartNewRow() {
			RtfTable table = tableController.CurrentTable;
			Debug.Assert(table != null);
			currentRow = new RtfTableRow(table, tableController.Reader.Importer);
			CellController.StartNewCell();
		}
		public virtual bool IsCurrentRowNotComplete() {
			return !TableController.CurrentTable.Rows.Contains(CurrentRow) && (CurrentRow.Cells.Count > 0 || CellController.IsCurrentCellNotComplete());
		}
		public virtual bool IsCurrentRowValid() {
			return TableController.CurrentTable.Rows.Contains(CurrentRow) && CurrentRow.Cells.Count > 0;
		}
		public virtual void AssignLastRowAsCurrent() {
			Debug.Assert(TableController.CurrentTable.Rows.Count > 0);
			currentRow = TableController.CurrentTable.Rows.Last;
			CellController.AssignLastCellAsCurrent();
		}
		public virtual void FinishRow() {
			FinishRowCore();
			AssignRowProperties();
		}
		protected internal virtual void FinishRowCore() {
			Debug.Assert(Object.ReferenceEquals(currentRow.Table, TableController.CurrentTable));
			RtfTableRowCollection rows = TableController.CurrentTable.Rows;
			if (rows.Count == 0 || rows.Last != currentRow) {
				currentRow.Index = rows.Count;
				Debug.Assert(Object.ReferenceEquals(currentRow.Table, TableController.CurrentTable));
				Debug.Assert(!rows.Contains(currentRow));
				rows.Add(currentRow);
			}
		}
		protected internal virtual void AssignRowProperties() {
			RtfTableProperties tableProperties = TableController.CurrentTable.Properties;
			if (!tableProperties.IsChanged()) {
				tableProperties.CopyFrom(TableController.Reader.TableProperties);
				tableProperties.FloatingPosition.CopyFrom(TableController.Reader.TableProperties.FloatingPosition);
			}
			tableProperties.TableStyleIndex = TableController.Reader.TableProperties.TableStyleIndex;
			currentRow.Properties.CopyFrom(TableController.Reader.RowProperties);
			currentRow.Properties.FloatingPosition.CopyFrom(TableController.Reader.RowProperties.FloatingPosition);
			currentRow.Left = TableController.Reader.RowProperties.Left;
			AssignCellProperties();
		}
		protected internal virtual void AssignCellProperties() {
			RtfTableReader reader = TableController.Reader;
			int cellPropertiesCount = reader.CellPropertiesCollection.Count;
			if (cellPropertiesCount == 0)
				RtfImporter.ThrowInvalidRtfFile();
			RtfTableCellCollection cells = currentRow.Cells;
			while (cells.Count > cellPropertiesCount && cells.Count > 1) {
				cells[1].StartParagraphIndex = cells[0].StartParagraphIndex;
				cells.RemoveAt(0);
			}
			int cellCount = cells.Count;
			for (int i = 0; i < cellCount; i++) {
				RtfTableCell cell = currentRow.Cells[i];
				RtfTableCellProperties properties = reader.CellPropertiesCollection[i];
				cell.Right = properties.Right;
				cell.Properties.HorizontalMerging = properties.HorizontalMerging;
				cell.Properties.CopyFrom(properties);
			}
		}
		public virtual void Reset() {
			currentRow = null;
			CellController.Reset();
		}
	}
	#endregion
	#region RtfTableCellController
	public class RtfTableCellController {
		readonly RtfTableRowController rowController;
		RtfTableCell currentCell;
		public RtfTableCellController(RtfTableRowController rowController) {
			this.rowController = rowController;
		}
		public RtfTableCell CurrentCell { get { return currentCell; } }
		protected internal RtfTableRowController RowController { get { return rowController; } }
		public virtual void StartNewCell() {
			RtfTableRow row = RowController.CurrentRow;
			Debug.Assert(row != null);
			currentCell = new RtfTableCell(row, RowController.TableController.Reader.Importer);
		}
		public virtual bool IsCurrentCellNotComplete() {
			return !RowController.CurrentRow.Cells.Contains(CurrentCell) && !CurrentCell.IsEmpty;
		}
		public virtual void AssignLastCellAsCurrent() {
			Debug.Assert(RowController.CurrentRow.Cells.Count > 0);
			currentCell = RowController.CurrentRow.Cells.Last;
		}
		public virtual void FinishCell() {
			Debug.Assert(Object.ReferenceEquals(currentCell.Row, RowController.CurrentRow));
			RtfTableCellCollection cells = RowController.CurrentRow.Cells;
			if (cells.Count == 0 || cells.Last != currentCell) {
				currentCell.Index = cells.Count;
				Debug.Assert(Object.ReferenceEquals(currentCell.Row, RowController.CurrentRow));
				Debug.Assert(!cells.Contains(currentCell));
				cells.Add(currentCell);
			}
		}
		public virtual void SetParagraphIndex(ParagraphIndex paragraphIndex) {
			Debug.Assert(paragraphIndex >= ParagraphIndex.Zero);
			SetParagraphIndexCore(CurrentCell, paragraphIndex);
			SetParagraphIndexesToParentCell(paragraphIndex);
		}
		protected internal virtual void SetParagraphIndexesToParentCell(ParagraphIndex paragraphIndex) {
			RtfTableCell parentCell = RowController.TableController.CurrentTable.ParentCell;
			while (parentCell != null) {
				SetParagraphIndexCore(parentCell, paragraphIndex);
				parentCell = parentCell.Row.Table.ParentCell;
			}
		}
		protected internal virtual void SetParagraphIndexCore(RtfTableCell cell, ParagraphIndex paragraphIndex) {
			if (cell.StartParagraphIndex < ParagraphIndex.Zero)
				cell.StartParagraphIndex = paragraphIndex;
			cell.EndParagraphIndex = paragraphIndex;
		}
		public virtual void Reset() {
			currentCell = null;
		}
	}
	#endregion
	#region Old Reader
	#endregion
}
