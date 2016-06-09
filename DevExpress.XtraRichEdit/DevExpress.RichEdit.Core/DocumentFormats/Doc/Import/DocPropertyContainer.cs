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
using DevExpress.XtraRichEdit.Model;
using DevExpress.Office;
using DevExpress.Utils;
using DevExpress.Compatibility.System.Drawing;
#if !SL
using System.Drawing;
#else
using System.Windows.Media;
#endif
namespace DevExpress.XtraRichEdit.Import.Doc {
	#region DocPropertyContainer
	public class DocPropertyContainer {
		protected internal DocPropertyContainer(DocCommandFactory factory, DocumentModelUnitConverter unitConverter) {
			Guard.ArgumentNotNull(factory, "factory");
			Factory = factory;
			UnitConverter = unitConverter;
		}
		#region Properties
		protected internal DocCommandFactory Factory { get; private set; }
		public DocumentModelUnitConverter UnitConverter { get; private set; }
		public bool IsDeleted { get; set; }
		public short FontFamilyNameIndex { get; set; }
		public int DataStreamOffset { get; set; }
		public CharacterInfo CharacterInfo { get; protected internal set; }
		public ParagraphInfo ParagraphInfo { get; protected internal set; }
		public FrameInfo FrameInfo { get; protected internal set; }
		public SectionInfo SectionInfo { get; protected internal set; }
		public TableInfo TableInfo { get; protected internal set; }
		public TableRowInfo TableRowInfo { get; protected internal set; }
		public TableCellInfo TableCellInfo { get; protected internal set; }
		public FieldProperties FieldProperties { get; set; }
		#endregion
		public void Update(ChangeActionTypes changeType) {
			Factory.UpdatePropertyContainer(this, changeType);
		}
	}
	#endregion
	#region SectionInfo
	public class SectionInfo {
		#region static
		public static SectionInfo CreateDefault(DocumentModel documentModel) {
			SectionInfo result = new SectionInfo();
			DocumentCache cache = documentModel.Cache;
			result.marginsInfo = cache.MarginsInfoCache.DefaultItem.Clone();
			result.columnsInfo = cache.ColumnsInfoCache.DefaultItem.Clone();
			result.pageInfo = cache.PageInfoCache.DefaultItem.Clone();
			result.generalSectionInfo = cache.GeneralSectionInfoCache.DefaultItem.Clone();
			result.generalSectionInfo.VerticalTextAlignment = VerticalAlignment.Top;
			result.pageNumberingInfo = cache.PageNumberingInfoCache.DefaultItem.Clone();
			result.lineNumberingInfo = cache.LineNumberingInfoCache.DefaultItem.Clone();
			result.footNote = new SectionFootNote(documentModel);
			result.footNote.CopyFrom(cache.FootNoteInfoCache[FootNoteInfoCache.DefaultFootNoteItemIndex]);
			result.endNote = new SectionFootNote(documentModel);
			result.endNote.CopyFrom(cache.FootNoteInfoCache[FootNoteInfoCache.DefaultEndNoteItemIndex]);
			return result;
		}
		#endregion
		#region Fields
		ColumnsInfo columnsInfo;
		MarginsInfo marginsInfo;
		PageInfo pageInfo;
		GeneralSectionInfo generalSectionInfo;
		PageNumberingInfo pageNumberingInfo;
		LineNumberingInfo lineNumberingInfo;
		SectionFootNote footNote;
		SectionFootNote endNote;
		#endregion
		protected SectionInfo() {
		}
		#region Properties
		public MarginsInfo SectionMargins { get { return marginsInfo; } }
		public ColumnsInfo SectionColumns { get { return columnsInfo; } }
		public PageInfo SectionPage { get { return pageInfo; } }
		public GeneralSectionInfo SectionGeneralSettings { get { return generalSectionInfo; } }
		public PageNumberingInfo SectionPageNumbering { get { return pageNumberingInfo; } }
		public LineNumberingInfo SectionLineNumbering { get { return lineNumberingInfo; } }
		public SectionFootNote FootNote { get { return footNote; } }
		public SectionFootNote EndNote { get { return endNote; } }
		#endregion
	}
	#endregion
	#region ParagraphInfo
	public class ParagraphInfo {
		#region static
		public static ParagraphInfo CreateDefault(DocumentModel documentModel) {
			ParagraphInfo result = new ParagraphInfo();
			result.formattingInfo = documentModel.Cache.ParagraphFormattingInfoCache.DefaultItem.Clone();
			result.formattingOptions = documentModel.Cache.ParagraphFormattingOptionsCache.DefaultItem.Clone();
			result.tabInfo = documentModel.Cache.TabFormattingInfoCache.DefaultItem.Clone();
			result.paragraphStyleIndex = (short)documentModel.ParagraphStyles.DefaultItemIndex;
			return result;
		}
		#endregion
		#region Fields
		ParagraphFormattingInfo formattingInfo;
		ParagraphFormattingOptions formattingOptions;
		TabFormattingInfo tabInfo;
		int paragraphStyleIndex;
		byte listLevel;
		int listInfoIndex;
		bool inTable;
		bool innerTableCell;
		bool nestedTableTrailer;
		bool tableTrailer;
		int tableDepth;
		#endregion
		protected ParagraphInfo() {
			this.listInfoIndex = -1;
		}
		#region Properties
		public ParagraphFormattingInfo FormattingInfo {
			get { return this.formattingInfo; }
			protected internal set { this.formattingInfo = value; }
		}
		public ParagraphFormattingOptions FormattingOptions {
			get { return this.formattingOptions; }
			protected internal set { this.formattingOptions = value; }
		}
		public TabFormattingInfo Tabs { get { return this.tabInfo; } }
		public int ParagraphStyleIndex { get { return this.paragraphStyleIndex; } set { this.paragraphStyleIndex = value; } }
		public byte ListLevel { get { return this.listLevel; } set { this.listLevel = value; } }
		public int ListInfoIndex { get { return this.listInfoIndex; } set { this.listInfoIndex = value; } }
		public bool InnerTableCell { get { return this.innerTableCell; } set { this.innerTableCell = value; } }
		public bool NestedTableTrailer { get { return this.nestedTableTrailer; } set { this.nestedTableTrailer = value; } }
		public bool InTable { get { return this.inTable; } set { this.inTable = value; } }
		public bool TableTrailer { get { return this.tableTrailer; } set { this.tableTrailer = value; } }
		public int TableDepth {
			get {
				if (this.inTable)
					return Math.Max(1, this.tableDepth);
				return this.tableDepth; 
			}
			set { this.tableDepth = value; }
		}
		#endregion
	}
	#endregion
	#region FrameInfo
	public class FrameInfo {
		#region static
		public static FrameInfo CreateDefault(DocumentModel documentModel) {
			FrameInfo result = new FrameInfo();
			result.formattingInfo = documentModel.Cache.ParagraphFrameFormattingInfoCache.DefaultItem.Clone();
			result.formattingOptions = documentModel.Cache.ParagraphFrameFormattingOptionsCache.DefaultItem.Clone();
			return result;
		}
		#endregion
		#region Fields
		ParagraphFrameFormattingInfo formattingInfo;
		ParagraphFrameFormattingOptions formattingOptions;
		#endregion
		protected FrameInfo() {
		}
		#region Properties
		public ParagraphFrameFormattingInfo FormattingInfo {
			get { return this.formattingInfo; }
			protected internal set { this.formattingInfo = value; }
		}
		public ParagraphFrameFormattingOptions FormattingOptions {
			get { return this.formattingOptions; }
			protected internal set { this.formattingOptions = value; }
		}
		#endregion
	}
	#endregion
	#region CharacterInfo
	public class CharacterInfo {
		#region static
		public static CharacterInfo CreateDefault(DocumentModel documentModel) {
			CharacterInfo result = new CharacterInfo();
			result.formattingInfo = DocCharacterFormattingInfo.CreateDefault();
			result.formattingOptions = documentModel.Cache.CharacterFormattingCache[CharacterFormattingInfoCache.DefaultItemIndex].Options.Clone();
			result.pictureBulletInformation = new DocPictureBulletInformation();
			return result;
		}
		#endregion
		#region Fields
		DocCharacterFormattingInfo formattingInfo;
		CharacterFormattingOptions formattingOptions;
		DocPictureBulletInformation pictureBulletInformation;
		bool special;
		bool binaryData;
		bool embeddedObject;
		bool ole2Object;
		char symbol;
		short specialCharactersFontFamilyNameIndex;
		#endregion
		protected CharacterInfo() {
		}
		#region Properties
		public DocCharacterFormattingInfo FormattingInfo {
			get { return this.formattingInfo; }
			protected internal set { this.formattingInfo = value; }
		}
		public CharacterFormattingOptions FormattingOptions {
			get { return this.formattingOptions; }
			protected internal set { this.formattingOptions = value; }
		}
		public short SpecialCharactersFontFamilyNameIndex {
			get { return this.specialCharactersFontFamilyNameIndex; }
			set { this.specialCharactersFontFamilyNameIndex = value; }
		}
		public bool Special { 
			get { return this.special; }
			set { this.special = value; }
		}
		public bool BinaryData {
			get { return this.binaryData; }
			set { this.binaryData = value; }
		}
		public bool EmbeddedObject {
			get { return this.embeddedObject; }
			protected internal set { this.embeddedObject = value; }
		}
		public bool Ole2Object {
			get { return this.ole2Object; }
			protected internal set { this.ole2Object = value; }
		}
		public char Symbol {
			get { return this.symbol; }
			set { this.symbol = value; }
		}
		public DocPictureBulletInformation PictureBulletInformation { get { return this.pictureBulletInformation; } }
		#endregion
	}
	#endregion
	#region TableInfo
	public class TableInfo {
		#region static
		public static TableInfo CreateDefault(DocumentModel documentModel) {
			TableInfo result = new TableInfo();
			result.tableProperties = new TableProperties(documentModel.MainPieceTable);
			result.tableProperties.SetIndexInitial(TablePropertiesOptionsCache.EmptyTableFormattingOptionsItem);
			result.TableProperties.TableLayout = TableLayoutType.Fixed; 
			result.tableDefinition = new TableDefinitionOperand();
			return result;
		}
		#endregion
		#region Fields
		int tableStyleIndex = -1;
		TableDefinitionOperand tableDefinition;
		TableProperties tableProperties;
		#endregion
		protected TableInfo() {
		}
		#region Properties
		public int TableStyleIndex { get { return this.tableStyleIndex; } set { this.tableStyleIndex = value; } }
		public TableProperties TableProperties { get { return this.tableProperties; } }
		public TableDefinitionOperand TableDefinition { get { return this.tableDefinition; } set { this.tableDefinition = value; } }
		#endregion
	}
	#endregion
	#region TableRowInfo
	public class TableRowInfo {
		#region static
		public static TableRowInfo CreateDefault(DocumentModel documentModel) {
			TableRowInfo result = new TableRowInfo();
			result.tableRowProperties = new TableRowProperties(documentModel.MainPieceTable);
			result.tableRowProperties.SetIndexInitial(TableRowPropertiesOptionsCache.EmptyRowPropertiesOptionsItem);
			return result;
		}
		#endregion
		#region Fields
		List<HorizontalMergeInfo> horizontalMerging;
		List<VerticalMergeInfo> verticalMerging;
		List<TableCellWidthOperand> preferredCellWidths;
		List<ColumnWidthOperand> columnWidthActions;
		List<InsertOperand> insertActions;
		List<CellSpacingOperand> cellMarginActions;
		List<TableBordersOverrideOperand> overrideCellBordersActions;
		List<CellRangeVerticalAlignmentOperand> cellRangeVerticalAlignmentActions;
		List<CellHideMarkOperand> cellHideMarkActions;
		List<DocTableBorderColorReference> topBorders;
		List<DocTableBorderColorReference> leftBorders;
		List<DocTableBorderColorReference> rightBorders;
		List<DocTableBorderColorReference> bottomBorders;
		List<Color> cellShading1;
		List<Color> cellShading2;
		List<Color> cellShading3;
		List<Color> defaultCellsShading;
		TableRowProperties tableRowProperties;
		TLP tableAutoformatLookSpecifier;
		#endregion
		protected TableRowInfo() {
			this.horizontalMerging = new List<HorizontalMergeInfo>();
			this.verticalMerging = new List<VerticalMergeInfo>();
			this.preferredCellWidths = new List<TableCellWidthOperand>();
			this.columnWidthActions = new List<ColumnWidthOperand>();
			this.insertActions = new List<InsertOperand>();
			this.cellMarginActions = new List<CellSpacingOperand>();
			this.overrideCellBordersActions = new List<TableBordersOverrideOperand>();
			this.cellRangeVerticalAlignmentActions = new List<CellRangeVerticalAlignmentOperand>();
			this.cellHideMarkActions = new List<CellHideMarkOperand>();
			this.topBorders = new List<DocTableBorderColorReference>();
			this.leftBorders = new List<DocTableBorderColorReference>();
			this.rightBorders = new List<DocTableBorderColorReference>();
			this.bottomBorders = new List<DocTableBorderColorReference>();
			this.cellShading1 = new List<Color>();
			this.cellShading2 = new List<Color>();
			this.cellShading3 = new List<Color>();
			this.defaultCellsShading = new List<Color>();			
		}
		#region Properties
		public List<HorizontalMergeInfo> HorizontalMerging { get { return horizontalMerging; } }
		public List<VerticalMergeInfo> VerticalMerging { get { return verticalMerging; } }
		public List<TableCellWidthOperand> PreferredCellWidths { get { return preferredCellWidths; } }
		public List<ColumnWidthOperand> ColumnWidthActions { get { return columnWidthActions; } }
		public List<InsertOperand> InsertActions { get { return insertActions; } }
		public List<CellSpacingOperand> CellMarginsActions { get { return cellMarginActions; } }
		public List<TableBordersOverrideOperand> OverrideCellBordersActions { get { return overrideCellBordersActions; } }
		public List<CellRangeVerticalAlignmentOperand> CellRangeVerticalAlignmentActions { get { return cellRangeVerticalAlignmentActions; } }
		public List<CellHideMarkOperand> CellHideMarkActions { get { return cellHideMarkActions; } }
		public TableRowProperties TableRowProperties { get { return tableRowProperties; } }
		public List<DocTableBorderColorReference> TopBorders { get { return topBorders; } }
		public List<DocTableBorderColorReference> LeftBorders { get { return leftBorders; } }
		public List<DocTableBorderColorReference> RightBorders { get { return rightBorders; } }
		public List<DocTableBorderColorReference> BottomBorders { get { return bottomBorders; } }
		public List<Color> CellShading1 { get { return cellShading1; } }
		public List<Color> CellShading2 { get { return cellShading2; } }
		public List<Color> CellShading3 { get { return cellShading3; } }
		public List<Color> DefaultCellsShading { get { return defaultCellsShading; } }
		public bool WidthBeforeSetted { get; set; }
		public TLP TableAutoformatLookSpecifier { get { return tableAutoformatLookSpecifier; } protected internal set { tableAutoformatLookSpecifier = value; } }
		#endregion
	}
	#endregion
	#region TableCellInfo
	public class TableCellInfo {
		#region static
		public static TableCellInfo CreateDefault(DocumentModel documentModel) {
			TableCellInfo result = new TableCellInfo();
			result.tableCellProperties = new TableCellProperties(documentModel.MainPieceTable, documentModel);
			result.tableCellProperties.SetIndexInitial(TableCellPropertiesOptionsCache.EmptyCellPropertiesOptionsItem);
			return result;
		}
		#endregion
		#region Fields
		public const int MaxCellCount = 64;
		TableCellProperties tableCellProperties;
		List<Color> cellColors;
		#endregion
		public TableCellInfo() {
			this.cellColors = new List<Color>(MaxCellCount);
		}
		#region Properties
		public TableCellProperties TableCellProperties { get { return this.tableCellProperties; } }
		public List<Color> CellColors { get { return this.cellColors; } }
		#endregion
	}
	#endregion
	#region VerticalMergeInfo
	public class VerticalMergeInfo {
		#region Fields
		byte cellIndex;
		MergingState mergingState;
		#endregion
		public VerticalMergeInfo(byte cellIndex, MergingState mergingState) {
			this.cellIndex = cellIndex;
			this.mergingState = mergingState;
		}
		#region Properties
		public byte CellIndex {
			get { return this.cellIndex; }
			set { this.cellIndex = value; }
		}
		public MergingState MergingState {
			get { return this.mergingState; }
			set { this.mergingState = value; }
		}
		#endregion
	}
	#endregion
	#region HorizontalMergeInfo
	public class HorizontalMergeInfo {
		 #region Fields
		byte firstCellIndex;
		byte lastCellIndex;
		bool split;
		#endregion
		public HorizontalMergeInfo(byte firstCellIndex, byte lastCellIndex, bool split) {
			this.firstCellIndex = firstCellIndex;
			this.lastCellIndex = lastCellIndex;
			this.split = split;
		}
		#region Properties
		public byte FirstCellIndex { get { return this.firstCellIndex; } }
		public byte LastCellIndex { get { return this.lastCellIndex; } }
		#endregion
	}
	#endregion
}
