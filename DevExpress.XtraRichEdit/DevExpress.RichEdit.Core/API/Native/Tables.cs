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
using System.Runtime.InteropServices;
using DevExpress.XtraRichEdit.Model;
using DevExpress.Utils;
using DevExpress.Office;
using DevExpress.Compatibility.System.Drawing;
#if !SL
using System.Drawing;
#else
using System.Windows.Media;
#endif
namespace DevExpress.XtraRichEdit.API.Native {
	#region TableCellVerticalAlignment
	[ComVisible(true)]
	public enum TableCellVerticalAlignment {
		Top = DevExpress.XtraRichEdit.Model.VerticalAlignment.Top,
		Center = DevExpress.XtraRichEdit.Model.VerticalAlignment.Center,
		Bottom = DevExpress.XtraRichEdit.Model.VerticalAlignment.Bottom
	}
	#endregion
	#region TableBorderLineStyle
	[ComVisible(true)]
	public enum TableBorderLineStyle {
		Nil = DevExpress.XtraRichEdit.Model.BorderLineStyle.Nil,
		None = DevExpress.XtraRichEdit.Model.BorderLineStyle.None,
		Single = DevExpress.XtraRichEdit.Model.BorderLineStyle.Single,
		Thick = DevExpress.XtraRichEdit.Model.BorderLineStyle.Thick,
		Double = DevExpress.XtraRichEdit.Model.BorderLineStyle.Double,
		Dotted = DevExpress.XtraRichEdit.Model.BorderLineStyle.Dotted,
		Dashed = DevExpress.XtraRichEdit.Model.BorderLineStyle.Dashed,
		DotDash = DevExpress.XtraRichEdit.Model.BorderLineStyle.DotDash,
		DotDotDash = DevExpress.XtraRichEdit.Model.BorderLineStyle.DotDotDash,
		Triple = DevExpress.XtraRichEdit.Model.BorderLineStyle.Triple,
		ThinThickSmallGap = DevExpress.XtraRichEdit.Model.BorderLineStyle.ThinThickSmallGap,
		ThickThinSmallGap = DevExpress.XtraRichEdit.Model.BorderLineStyle.ThickThinSmallGap,
		ThinThickThinSmallGap = DevExpress.XtraRichEdit.Model.BorderLineStyle.ThinThickThinSmallGap,
		ThinThickMediumGap = DevExpress.XtraRichEdit.Model.BorderLineStyle.ThinThickMediumGap,
		ThickThinMediumGap = DevExpress.XtraRichEdit.Model.BorderLineStyle.ThickThinMediumGap,
		ThinThickThinMediumGap = DevExpress.XtraRichEdit.Model.BorderLineStyle.ThinThickThinMediumGap,
		ThinThickLargeGap = DevExpress.XtraRichEdit.Model.BorderLineStyle.ThinThickLargeGap,
		ThickThinLargeGap = DevExpress.XtraRichEdit.Model.BorderLineStyle.ThickThinLargeGap,
		ThinThickThinLargeGap = DevExpress.XtraRichEdit.Model.BorderLineStyle.ThinThickThinLargeGap,
		Wave = DevExpress.XtraRichEdit.Model.BorderLineStyle.Wave,
		DoubleWave = DevExpress.XtraRichEdit.Model.BorderLineStyle.DoubleWave,
		DashSmallGap = DevExpress.XtraRichEdit.Model.BorderLineStyle.DashSmallGap,
		DashDotStroked = DevExpress.XtraRichEdit.Model.BorderLineStyle.DashDotStroked,
		ThreeDEmboss = DevExpress.XtraRichEdit.Model.BorderLineStyle.ThreeDEmboss,
		ThreeDEngrave = DevExpress.XtraRichEdit.Model.BorderLineStyle.ThreeDEngrave,
		Outset = DevExpress.XtraRichEdit.Model.BorderLineStyle.Outset,
		Inset = DevExpress.XtraRichEdit.Model.BorderLineStyle.Inset,
	}
	#endregion
	#region WidthType
	[ComVisible(true)]
	public enum WidthType {
		None = DevExpress.XtraRichEdit.Model.WidthUnitType.Nil,
		Auto = DevExpress.XtraRichEdit.Model.WidthUnitType.Auto,
		FiftiethsOfPercent = DevExpress.XtraRichEdit.Model.WidthUnitType.FiftiethsOfPercent,
		Fixed = DevExpress.XtraRichEdit.Model.WidthUnitType.ModelUnits
	}
	#endregion
	#region AutoFitBehaviorType
	[ComVisible(true)]
	public enum AutoFitBehaviorType {
		FixedColumnWidth = DevExpress.XtraRichEdit.Model.TableAutoFitBehaviorType.FixedColumnWidth,
		AutoFitToContents = DevExpress.XtraRichEdit.Model.TableAutoFitBehaviorType.AutoFitToContents,
		AutoFitToWindow = DevExpress.XtraRichEdit.Model.TableAutoFitBehaviorType.AutoFitToWindow
	}
	#endregion
	#region HeightType
	[ComVisible(true)]
	public enum HeightType {
		Auto = DevExpress.XtraRichEdit.Model.HeightUnitType.Auto,
		Exact = DevExpress.XtraRichEdit.Model.HeightUnitType.Exact,
		AtLeast = DevExpress.XtraRichEdit.Model.HeightUnitType.Minimum
	}
	#endregion
	#region TableRowAlignment
	[ComVisible(true)]
	public enum TableRowAlignment {
		Both = DevExpress.XtraRichEdit.Model.TableRowAlignment.Both,
		Center = DevExpress.XtraRichEdit.Model.TableRowAlignment.Center,
		Distribute = DevExpress.XtraRichEdit.Model.TableRowAlignment.Distribute,
		Left = DevExpress.XtraRichEdit.Model.TableRowAlignment.Left,
		NumTab = DevExpress.XtraRichEdit.Model.TableRowAlignment.NumTab, 
		Right = DevExpress.XtraRichEdit.Model.TableRowAlignment.Right
	}
	#endregion
	[ComVisible(true)]
	public enum TableLayoutType {
		Autofit = Model.TableLayoutType.Autofit,
		Fixed = Model.TableLayoutType.Fixed
	}
	[ComVisible(true)]
	public delegate void TableCellProcessorDelegate(TableCell cell, int rowIndex, int cellIndex);
	[ComVisible(true)]
	public delegate void TableRowProcessorDelegate(TableRow row, int rowIndex);
	#region TableCellBorder
	[ComVisible(true)]
	public interface TableCellBorder {
		Color LineColor { get; set; }
		float LineThickness { get; set; }
		TableBorderLineStyle LineStyle { get; set; }
	}
	#endregion
	#region TableCellBorders
	[ComVisible(true)]
	public interface TableCellBorders {
		TableCellBorder Left { get; }
		TableCellBorder Right { get; }
		TableCellBorder Top { get; }
		TableCellBorder Bottom { get; }
	}
	#endregion
	#region TableBorder
	[ComVisible(true)]
	public interface TableBorder {
		Color LineColor { get; set; }
		TableBorderLineStyle LineStyle { get; set; }
		float LineThickness { get; set; }
	}
	#endregion
	#region TableBorders
	[ComVisible(true)]
	public interface TableBorders {
		TableBorder Left { get; }
		TableBorder Right { get; }
		TableBorder Top { get; }
		TableBorder Bottom { get; }
		TableBorder InsideHorizontalBorder { get; }
		TableBorder InsideVerticalBorder { get; }
	}
	#endregion
	[ComVisible(true)]
	public interface TableCell {
		DocumentRange Range { get; }
		DocumentRange ContentRange { get; }
		TableRow Row { get; }
		Table Table { get; }
		int Index { get; }
		TableCell Previous { get; }
		TableCell Next { get; }
		int NestingLevel { get; }
		Color BackgroundColor { get; set; }
		TableCellVerticalAlignment VerticalAlignment { get; set; }
		bool WordWrap { get; set; }
		float TopPadding { get; set; }
		float BottomPadding { get; set; }
		float LeftPadding { get; set; }
		float RightPadding { get; set; }
		TableCellBorders Borders { get; }
		float PreferredWidth { get; set; }
		WidthType PreferredWidthType { get; set; }
		float Height { get; set; }
		HeightType HeightType { get; set; }
		TableCellStyle Style { get; set; }
		void Split(int rowsCount, int columnCount);
		void Delete();
	}
	[ComVisible(true)]
	public interface TableCellCollection : ISimpleCollection<TableCell> {
		TableCell First { get; }
		TableCell Last { get; }
		TableCell Append();
		TableCell InsertBefore(int columnIndex);
		TableCell InsertAfter(int columnIndex);
		void RemoveAt(int columnIndex);
	}
	[ComVisible(true)]
	public interface TableRow {
		DocumentRange Range { get; }
		TableCellCollection Cells { get; }
		TableCell this[int column] { get; }
		Table Table { get; }
		int Index { get; }
		void Delete();
		bool IsLast { get; }
		bool IsFirst { get; }
		TableRow Previous { get; }
		TableRow Next { get; }
		int NestingLevel { get; }
		float Height { get; set; }
		HeightType HeightType { get; set; }
		TableCell FirstCell { get; }
		TableCell LastCell { get; }
		TableRowAlignment TableRowAlignment { get; set; }
	}
	[ComVisible(true)]
	public interface TableRowCollection : ISimpleCollection<TableRow> {
		TableRow First { get; }
		TableRow Last { get; }
		TableRow InsertBefore(int rowIndex);
		TableRow InsertAfter(int rowIndex);
		TableRow Append();
		void RemoveAt(int rowIndex);
	}
	[ComVisible(true)]
	public interface Table {
		DocumentRange Range { get; }
		TableRowCollection Rows { get; }
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1023")]
		TableCell this[int row, int column] { get; }
		TableCell Cell(int row, int column);
		int NestingLevel { get; }
		TableCell ParentCell { get; }
		float PreferredWidth { get; set; }
		WidthType PreferredWidthType { get; set; }
		float TopPadding { get; set; }
		float BottomPadding { get; set; }
		float LeftPadding { get; set; }
		float RightPadding { get; set; }
		float TableCellSpacing { get; set; }
		float Indent { get; set; }
		TableLayoutType TableLayout { get; set; }
		TableRowAlignment TableAlignment { get; set; }
		Color TableBackgroundColor { get; set; }
		TableBorders Borders { get; }
		TableRow FirstRow { get; }
		TableRow LastRow { get; }
		void BeginUpdate();
		void EndUpdate();
		void Validate();
		void MergeCells(TableCell mergeFrom, TableCell mergeTo);
		void SetPreferredWidth(float Width, WidthType widthType);
		void ForEachCell(TableCellProcessorDelegate cellProcessor);
		void ForEachRow(TableRowProcessorDelegate cellProcessor);
		TableStyle Style { get; set; }
		TableLookTypes TableLook { get; set; }
		bool MakeWordCompatible();
		bool IsWordCompatible { get; }
		void Reset();
		void Reset(TablePropertiesMask mask);
	}
	#region ReadOnlyTableCollection
	[ComVisible(true)]
	public interface ReadOnlyTableCollection : ISimpleCollection<Table> {
		ReadOnlyTableCollection Get(DocumentRange range);
		TableCell GetTableCell(DocumentPosition pos);
	}
	#endregion
	[ComVisible(true)]
	public interface TableCollection : ReadOnlyTableCollection {
		Table First { get; }
		Table Last { get; }
		[Obsolete("This method has become obsolete. Use the 'Create' method instead.")]
		Table Add(DocumentPosition pos, int rowCount, int columnCount);
		[Obsolete("This method has become obsolete. Use the 'Create' method instead.")]
		Table Add(DocumentPosition pos, int rowCount, int columnCount, AutoFitBehaviorType autoFitBehavior);
		[Obsolete("This method has become obsolete. Use the 'Create' method instead.")]
		Table Add(DocumentPosition pos, int rowCount, int columnCount, AutoFitBehaviorType autoFitBehavior, int fixedColumnWidths);
		Table Create(DocumentPosition pos, int rowCount, int columnCount);
		Table Create(DocumentPosition pos, int rowCount, int columnCount, AutoFitBehaviorType autoFitBehavior);
		Table Create(DocumentPosition pos, int rowCount, int columnCount, AutoFitBehaviorType autoFitBehavior, int fixedColumnWidths);
		void RemoveTableAt(int tableIndex);
		void Remove(Table table);
		int IndexOf(Table table);
	}
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2217")]
	[Flags]
	public enum TablePropertiesMask {
		LeftPadding = 0x00000001,
		RightPadding = 0x00000002,
		TopPadding = 0x00000004,
		BottomPadding = 0x00000008,
		CellSpacing = 0x00000010,
		TableIndent = 0x00000020,
		TableLayout = 0x00000040,
		TableLook = 0x00000080,
		PreferredWidth = 0x00000100,
		TableStyleColBandSize = 0x00000200,
		TableStyleRowBandSize = 0x00000400,
		LeftBorder = 0x00002000,
		RightBorder = 0x00004000,
		TopBorder = 0x00008000,
		BottomBorder = 0x00010000,
		InsideHorizontalBorder = 0x00020000,
		InsideVerticalBorder = 0x00040000,
		BackgroundColor = 0x00080000,
		TableAlignment = 0x00100000,
		Borders = 0x0007E000,
		All = 0x7FFFFFFF
	}
	[ComVisible(true)]
	public interface TablePropertiesBase {
		float? TopPadding { get; set; }
		float? BottomPadding { get; set; }
		float? LeftPadding { get; set; }
		float? RightPadding { get; set; }
		float? TableCellSpacing { get; set; }
		Color? TableBackgroundColor { get; set; }
		TableBorders TableBorders { get; }
		TableRowAlignment? TableAlignment { get; set; }
		TableLayoutType? TableLayout { get; set;}
		void Reset();
		void Reset(TablePropertiesMask mask);
	}
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2217")]
	[Flags]
	public enum TableCellPropertiesMask {
		PreferredWidth = 0x00000001,
		NoWrap = 0x00000004,
		LeftPadding = 0x00000010,
		RightPadding = 0x00000020,
		TopPadding = 0x00000040,
		BottomPadding = 0x00000080,
		VerticalAlignment = 0x00000200,
		LeftBorder = 0x00001000,
		RightBorder = 0x00002000,
		TopBorder = 0x00004000,
		BottomBorder = 0x00008000,
		InsideHorizontalBorder = 0x00010000,
		InsideVerticalBorder = 0x000020000,
		TopLeftDiagonalBorder = 0x00040000,
		TopRightDiagonalBorder = 0x00080000,
		BackgroundColor = 0x00100000,
		All = 0x7FFFFFFF
	}
	[ComVisible(true)]
	public interface TableCellPropertiesBase {
		Color? CellBackgroundColor { get; set; }
		TableCellVerticalAlignment? VerticalAlignment { get; set; }
		bool? NoWrap { get; set; }
		float? CellTopPadding { get; set; }
		float? CellBottomPadding { get; set; }
		float? CellLeftPadding { get; set; }
		float? CellRightPadding { get; set; }
		TableCellBorders TableCellBorders { get; }
		void Reset();
		void Reset(TableCellPropertiesMask mask);
	}
}
namespace DevExpress.XtraRichEdit.API.Native.Implementation {
	using System.Collections;
	using DevExpress.XtraRichEdit.Utils;
	using ModelPieceTable = DevExpress.XtraRichEdit.Model.PieceTable;
	using ModelLogPosition = DevExpress.XtraRichEdit.Model.DocumentLogPosition;
	using ModelPosition = DevExpress.XtraRichEdit.Model.DocumentModelPosition;
	using ModelParagraph = DevExpress.XtraRichEdit.Model.Paragraph;
	using ModelParagraphIndex = DevExpress.XtraRichEdit.Model.ParagraphIndex;
	using ModelTable = DevExpress.XtraRichEdit.Model.Table;
	using ModelTableRow = DevExpress.XtraRichEdit.Model.TableRow;
	using ModelTableCell = DevExpress.XtraRichEdit.Model.TableCell;
	using ModelTableBorder = DevExpress.XtraRichEdit.Model.BorderBase;
	using ModelTableStructureBySelectionCalculator = DevExpress.XtraRichEdit.Tables.Native.TableStructureBySelectionCalculator;
	using ModelSelectedCellsCollection = DevExpress.XtraRichEdit.Tables.Native.SelectedCellsCollection;
	using ModelDocumentModel = DevExpress.XtraRichEdit.Model.DocumentModel;
	using DevExpress.XtraRichEdit.Localization;
	using DevExpress.Office.Utils;
	using Compatibility.System.Drawing;
	#region NativeTable
	public class NativeTable : Table, IBatchUpdateable, IBatchUpdateHandler {
		#region Fields
		readonly BatchUpdateHelper batchUpdateHelper;
		readonly NativeSubDocument document;
		readonly ModelTable table;
		readonly NativeTableRowCollection rows;
		NativeTableBorders borders;
		bool isValid;
		#endregion
		internal NativeTable(NativeSubDocument document, ModelTable table) {
			Guard.ArgumentNotNull(document, "document");
			Guard.ArgumentNotNull(table, "table");
			this.batchUpdateHelper = new BatchUpdateHelper(this);
			this.table = table;
			this.document = document;
			this.rows = new NativeTableRowCollection(this);
			this.isValid = true;
		}
		#region Properties
		public NativeSubDocument Document { get { return document; } }
		protected internal DevExpress.XtraRichEdit.Model.DocumentModel DocumentModel { get { return ModelTable.DocumentModel; } }
		public ModelTable ModelTable { get { return table; } }
		public bool IsValid { get { return isValid; } set { isValid = value; } }
		public TableRowCollection Rows { get { return rows; } }
		public TableCell this[int row, int column] { get { return Rows[row].Cells[column]; } }
		public DocumentRange Range {
			get {
				ModelParagraph startParagraph = Document.PieceTable.Paragraphs[table.StartParagraphIndex];
				ModelParagraph endParagraph = Document.PieceTable.Paragraphs[table.EndParagraphIndex];
				return Document.CreateRange(startParagraph.LogPosition, endParagraph.LogPosition + endParagraph.Length - startParagraph.LogPosition);
			}
		}
		public int NestingLevel { get { return ModelTable.NestedLevel; } }
		public TableCell ParentCell { get { return NestingLevel > 0 ? GetParentCell() : null; } }
		public TableBorders Borders {
			get {
				if (borders == null)
					borders = new NativeTableBorders(this);
				return borders;
			}
		}
		public TableRow FirstRow { get { return Rows.First; } }
		public TableRow LastRow { get { return Rows.Last; } }
		#region PreferredWidth
		public float PreferredWidth {
			get { return Document.GetWidthValue(ModelTable.PreferredWidth); }
			set { Document.SetWidthValue(ModelTable.TableProperties.PreferredWidth, value); }
		}
		#endregion
		#region PreferredWidthType
		public WidthType PreferredWidthType {
			get { return (WidthType)ModelTable.PreferredWidth.Type; }
			set { ModelTable.TableProperties.PreferredWidth.Type = (DevExpress.XtraRichEdit.Model.WidthUnitType)value; } }
		#endregion
		#region TopPadding
		public float TopPadding {
			get { return Document.GetWidthUnitFixedValue(ModelTable.TopMargin); }
			set { Document.SetWidthUnitFixedValue(ModelTable.TableProperties.CellMargins.Top, value); }
		}
		#endregion
		#region BottomPadding
		public float BottomPadding {
			get { return Document.GetWidthUnitFixedValue(ModelTable.BottomMargin); }
			set { Document.SetWidthUnitFixedValue(ModelTable.TableProperties.CellMargins.Bottom, value); }
		}
		#endregion
		#region LeftPadding
		public float LeftPadding {
			get { return Document.GetWidthUnitFixedValue(ModelTable.LeftMargin); }
			set { Document.SetWidthUnitFixedValue(ModelTable.TableProperties.CellMargins.Left, value); }
		}
		#endregion
		#region RightPadding
		public float RightPadding {
			get { return Document.GetWidthUnitFixedValue(ModelTable.RightMargin); }
			set { Document.SetWidthUnitFixedValue(ModelTable.TableProperties.CellMargins.Right, value); }
		}
		#endregion
		#region Spacing
		public float TableCellSpacing {
			get { return Document.GetWidthUnitFixedValue(ModelTable.CellSpacing); }
			set { Document.SetWidthUnitFixedValue(ModelTable.TableProperties.CellSpacing, value); }
		}
		#endregion
		#region Indent
		public float Indent {
			get { return Document.GetWidthUnitFixedValue(ModelTable.TableIndent); }
			set { Document.SetWidthUnitFixedValue(ModelTable.TableProperties.TableIndent, value); }
		}
		#endregion
		#region TableAlignment
		public TableRowAlignment TableAlignment {
			get { return (Native.TableRowAlignment)ModelTable.TableAlignment; }
			set { ModelTable.TableProperties.TableAlignment = (Model.TableRowAlignment)value; }
		}
		#endregion
		#region TableBackgroundColor
		public Color TableBackgroundColor {
			get { return (Color)ModelTable.BackgroundColor; }
			set { ModelTable.TableProperties.BackgroundColor = value; }
		}
		#endregion
		#region TableLayout
		public TableLayoutType TableLayout { 
			get { return (Native.TableLayoutType)ModelTable.TableLayout; }
			set { ModelTable.TableProperties.TableLayout = (Model.TableLayoutType)value; }
		}
		#endregion
		public TableLookTypes TableLook {
			get { return (Native.TableLookTypes)ModelTable.TableLook; }
			set { ModelTable.TableLook = (Model.TableLookTypes)value; }
		}
		public void Reset() {
			ModelTable.TableProperties.ResetAllUse();
		}
		public void Reset(TablePropertiesMask mask) {
			ModelTable.TableProperties.ResetUse((TablePropertiesOptions.Mask)mask);
		}
		#endregion
		#region IBatchUpdateable implementation
		public void BeginUpdate() {
			batchUpdateHelper.BeginUpdate();
		}
		public void EndUpdate() {
			batchUpdateHelper.EndUpdate();
		}
		public void CancelUpdate() {
			batchUpdateHelper.CancelUpdate();
		}
		BatchUpdateHelper IBatchUpdateable.BatchUpdateHelper { get { return batchUpdateHelper; } }
		public bool IsUpdateLocked { get { return batchUpdateHelper.IsUpdateLocked; } }
		#endregion
		#region IBatchUpdateHandler implementation
		void IBatchUpdateHandler.OnFirstBeginUpdate() {
			Document.DocumentModel.BeginUpdate();
		}
		void IBatchUpdateHandler.OnBeginUpdate() {
		}
		void IBatchUpdateHandler.OnEndUpdate() {
		}
		void IBatchUpdateHandler.OnLastEndUpdate() {
			OnLastEndUpdateCore();
		}
		void IBatchUpdateHandler.OnCancelUpdate() {
		}
		void IBatchUpdateHandler.OnLastCancelUpdate() {
			OnLastEndUpdateCore();
		}
		#endregion
		public TableCell Cell(int row, int column) {
			return Rows[row].Cells[column];
		}
		protected internal virtual void OnLastEndUpdateCore() {
			Validate();
			Document.DocumentModel.EndUpdate();
		}
		internal void CheckValid() {
			if (!isValid)
				RichEditExceptions.ThrowInvalidOperationException(XtraRichEditStringId.Msg_UseDeletedTableError);
		}
		public void Validate() {
			CheckValid();
		}
		public void MergeCells(TableCell mergeFrom, TableCell mergeTo) {
			NativeTableCell startCell = (NativeTableCell)mergeFrom;
			NativeTableCell endCell = (NativeTableCell)mergeTo;
			ModelSelectedCellsCollection cells = GetCellsForMerging(startCell, endCell);
			Document.DocumentModel.BeginUpdate();
			try {
				Document.PieceTable.MergeCells(cells);
			}
			finally {
				Document.DocumentModel.EndUpdate();
			}
		}
		ModelSelectedCellsCollection GetCellsForMerging(NativeTableCell from, NativeTableCell to) {
			DevExpress.XtraRichEdit.Tables.Native.TableStructureBySelectionCalculator selectionCalculator = new DevExpress.XtraRichEdit.Tables.Native.TableStructureBySelectionCalculator(Document.PieceTable);
			return selectionCalculator.Calculate(from.ModelCell, to.ModelCell);
		}
		TableCell GetParentCell() {
			ModelTableCell parentCell = ModelTable.ParentCell;
			int column = parentCell.IndexInRow;
			int rowIndexInTable = parentCell.Row.IndexInTable;
			int tablesCount = document.Tables.Count;
			Table findedParentTable = null;
			for (int i = 0; i < tablesCount; i++) {
				if (((NativeTable)document.Tables[i]).ModelTable == parentCell.Table) {
					findedParentTable = document.Tables[i];
					break;
				}
			}
			return (findedParentTable != null) ? findedParentTable[rowIndexInTable, column] : null;
		}
		public void SetPreferredWidth(float width, WidthType type) {
			PreferredWidthType = type;
			PreferredWidth = width;
		}
		public void ForEachRow(TableRowProcessorDelegate rowProcessor) {
			TableRowCollection rows = Rows;
			int rowCount = rows.Count;
			for (int i = 0; i < rowCount; i++)
				rowProcessor(rows[i], i);
		}
		public void ForEachCell(TableCellProcessorDelegate cellProcessor) {
			TableRowCollection rows = Rows;
			int rowCount = rows.Count;
			for (int rowIndex = 0; rowIndex < rowCount; rowIndex++) {
				TableCellCollection cells = rows[rowIndex].Cells;
				int cellCount = cells.Count; ;
				for (int cellIndex = 0; cellIndex < cellCount; cellIndex++)
					cellProcessor(cells[cellIndex], rowIndex, cellIndex);
			}
		}
		#region Style
		public TableStyle Style {
			get {
				CheckValid();
				NativeTableStyleCollection styles = (NativeTableStyleCollection)Document.MainDocument.TableStyles;
				return styles.GetStyle( ModelTable.TableStyle);
			}
			set {
				CheckValid();
				Model.TableStyle style = value != null ? ((NativeTableStyle)value).InnerStyle : null;
ModelTable.StyleIndex = (DocumentModel.TableStyles.IndexOf(style));
			}
		}
		#endregion
		public bool MakeWordCompatible() {
			return document.PieceTable.MakeTableWordCompatible(ModelTable);
		}
		public bool IsWordCompatible {
			get {
				TableRowCollection rows = Rows;
				int rowCount = rows.Count;
				for(int i = 0; i < rowCount; i++)
					if(rows[i].Cells.Count > PieceTable.wordMaxColCount)
						return false;
				return true;
			}
		}
	}
	#endregion
	#region NativeTableCollection
	public class NativeTableCollection : List<NativeTable>, TableCollection {
		readonly NativeSubDocument document;
		internal NativeTableCollection(NativeSubDocument document) {
			Guard.ArgumentNotNull(document, "document");
			this.document = document;
		}
		#region ISimpleCollection<Table> Members
		Table ISimpleCollection<Table>.this[int index] {
			get { return this[index]; }
		}
		#endregion
		#region IEnumerable<Table> Members
		IEnumerator<Table> IEnumerable<Table>.GetEnumerator() {
			return new EnumeratorAdapter<Table, NativeTable>(this.GetEnumerator());
		}
		#endregion
		ModelPieceTable PieceTable { get { return document.PieceTable; } }
		ModelDocumentModel DocumentModel { get { return PieceTable.DocumentModel; } }
		public Table First { get { return Count > 0 ? this[0] : null; } }
		public Table Last {
			get {
				int count = Count;
				return count > 0 ? this[count - 1] : null;
			}
		}
		[Obsolete("This method has become obsolete. Use the 'Create' method instead.")]
		public Table Add(DocumentPosition pos, int rowCount, int columnCount) {
			return document.InsertTable(pos, rowCount, columnCount);
		}
		[Obsolete("This method has become obsolete. Use the 'Create' method instead.")]
		public Table Add(DocumentPosition pos, int rowCount, int columnCount, AutoFitBehaviorType autoFitBehaviorType) {
			return document.InsertTable(pos, rowCount, columnCount, autoFitBehaviorType);
		}
		[Obsolete("This method has become obsolete. Use the 'Create' method instead.")]
		public Table Add(DocumentPosition pos, int rowCount, int columnCount, AutoFitBehaviorType autoFitBehaviorType, int fixedColumnWidths) {
			return document.InsertTable(pos, rowCount, columnCount, autoFitBehaviorType, fixedColumnWidths);
		}
		public Table Create(DocumentPosition pos, int rowCount, int columnCount) {
			return Create(pos, rowCount, columnCount, AutoFitBehaviorType.AutoFitToContents, Int32.MinValue);
		}
		public Table Create(DocumentPosition pos, int rowCount, int columnCount, AutoFitBehaviorType autoFitBehaviorType) {
			return Create(pos, rowCount, columnCount, autoFitBehaviorType, Int32.MinValue);
		}
		public Table Create(DocumentPosition pos, int rowCount, int columnCount, AutoFitBehaviorType autoFitBehaviorType, int fixedColumnWidths) {
			document.CheckValid();
			document.CheckDocumentPosition(pos);
			ModelLogPosition logPosition = document.NormalizeLogPosition(pos.LogPosition);
			ModelTable newTable;
			DocumentModel.BeginUpdate();
			try {
				newTable = PieceTable.InsertTable(logPosition, rowCount, columnCount, (TableAutoFitBehaviorType)autoFitBehaviorType, document.UnitsToModelUnits(fixedColumnWidths));
				int styleIndex = DocumentModel.TableStyles.GetStyleIndexByName(DevExpress.XtraRichEdit.Model.TableStyleCollection.TableSimpleStyleName);
				if (styleIndex >= 0)
					newTable.StyleIndex = styleIndex;
			}
			finally {
				DocumentModel.EndUpdate();
			}
			return this[newTable.Index];
		}
		public void RemoveTableAt(int tableIndex) {
			if (tableIndex < 0 || tableIndex >= Count)
				return;
			DocumentModel.BeginUpdate();
			try {
				PieceTable.DeleteTableWithContent(PieceTable.Tables[tableIndex]);
			}
			finally {
				DocumentModel.EndUpdate();
			}
		}
		public void Remove(Table table) {
			DocumentModel.BeginUpdate();
			try {
				int index = document.Tables.IndexOf(table);
				PieceTable.DeleteTableWithContent(PieceTable.Tables[index]);
			}
			finally {
				DocumentModel.EndUpdate();
			}
		}
		public int IndexOf(Table table) {
			DevExpress.XtraRichEdit.Model.DocumentLogPosition endLogPos = table.Rows.Last.Cells.Last.Range.End.LogPosition-1;
			ModelParagraph lastTableParagraph = PieceTable.FindParagraph(endLogPos);
			ModelTableCell cell = PieceTable.TableCellsManager.GetCellByNestingLevel(lastTableParagraph.Index, table.NestingLevel);
			return cell.Table.Index;
		}
		public TableCell GetTableCell(DocumentPosition pos) {
			Paragraph paragraph = document.Paragraphs.Get(pos);
			NativeParagraph nativeParagraph = (NativeParagraph)paragraph;
			if (nativeParagraph == null)
				return null;
			Model.TableCell cell = nativeParagraph.InnerParagraph.GetCell();
			return cell != null ? this[cell.Table.Index][cell.RowIndex, cell.IndexInRow] : null;
		}
		public ReadOnlyTableCollection Get(DocumentRange range) {
			document.CheckValid();
			document.CheckDocumentRange(range);
			DevExpress.XtraRichEdit.API.Native.Implementation.NativeSubDocument.ParagraphRange paragraphsRange = document.CalculateParagraphsRange(range);
			NativeDocumentRange nativeRange = (NativeDocumentRange)range;
			ModelParagraphIndex firstIndex =  nativeRange.NormalizedStart.Position.ParagraphIndex;
			ModelParagraphIndex lastIndex = nativeRange.NormalizedEnd.Position.ParagraphIndex;
			NativeTableCollection result = new NativeTableCollection(document);
			int count = PieceTable.Tables.Count;
			for (int i = 0; i < count; i++) {
				ModelParagraphIndex tableStartParagraphIndex = PieceTable.Tables[i].StartParagraphIndex;
				ModelParagraphIndex tableEndParagraphIndex = PieceTable.Tables[i].EndParagraphIndex;
				if ((tableStartParagraphIndex >= firstIndex) && (tableEndParagraphIndex <= lastIndex)) {
					NativeTable table = new NativeTable(document, PieceTable.Tables[i]);
					result.Add(table);
				}
			}
			return result;
		}
	}
	#endregion
	#region NativeTableRow
	public class NativeTableRow : TableRow {
		#region Fields
		readonly NativeTable table;
		readonly int rowIndex;
		readonly NativeTableCellCollection cells;
		bool isValid;
		#endregion
		internal NativeTableRow(NativeTable table, int rowIndex) {
			Guard.ArgumentNotNull(table, "table");
			Guard.ArgumentNonNegative(rowIndex, "rowIndex");
			this.table = table;
			this.rowIndex = rowIndex;
			this.cells = new NativeTableCellCollection(this);
			this.isValid = true;
		}
		#region Properties
		public ModelTable ModelTable {
			get {
				CheckValid();
				return table.ModelTable;
			}
		}
		public ModelTableRow ModelRow {
			get {
				CheckValid();
				return table.ModelTable.Rows[rowIndex];
			}
		}
		public bool IsValid { get { return isValid; } set { isValid = value; } }
		public NativeTable NativeTable { get { return table; } }
		public NativeSubDocument Document { get { return NativeTable.Document; } }
		public Table Table { get { return table; } }
		public TableCellCollection Cells { get { return cells; } }
		public TableCell this[int column] { get { return Cells[column]; } }
		public TableCell FirstCell { get { return Cells.First; } }
		public TableCell LastCell { get { return Cells.Last; } }
		public DocumentRange Range {
			get {
				ModelParagraph startParagraph = Document.PieceTable.Paragraphs[ModelRow.FirstCell.StartParagraphIndex];
				ModelParagraph endParagraph = Document.PieceTable.Paragraphs[ModelRow.LastCell.EndParagraphIndex];
				return Document.CreateRange(startParagraph.LogPosition, endParagraph.LogPosition + endParagraph.Length - startParagraph.LogPosition);
			}
		}
		public int NestingLevel { get { return Table.NestingLevel; } }
		public int Index { get { return rowIndex; } }
		public bool IsFirst { get { return rowIndex == 0; } }
		public bool IsLast { get { return rowIndex == NativeTable.ModelTable.Rows.Count - 1; } }
		public TableRow Next {
			get {
				CheckValid();
				if (rowIndex + 1 < NativeTable.ModelTable.Rows.Count)
					return Table.Rows[rowIndex + 1];
				else
					return null;
			}
		}
		public TableRow Previous {
			get {
				CheckValid();
				if (rowIndex > 0)
					return Table.Rows[rowIndex - 1];
				else
					return null;
			}
		}
		public float Height {
			get { return Document.ModelUnitsToUnits(ModelRow.Properties.Height.Value); }
			set { ModelRow.Properties.Height.Value = Document.UnitsToModelUnits(value); }
		}
		public HeightType HeightType {
			get { return (HeightType)ModelRow.Height.Type; }
			set { ModelRow.Properties.Height.Type = (DevExpress.XtraRichEdit.Model.HeightUnitType)value; }
		}
		public TableRowAlignment TableRowAlignment {
			get { return (Native.TableRowAlignment)ModelRow.TableRowAlignment; }
			set { ModelRow.Properties.TableRowAlignment = (Model.TableRowAlignment)value; } 
		}
		#endregion
		internal void CheckValid() {
			table.CheckValid();
			if (isValid)
				isValid = (rowIndex <= table.ModelTable.Rows.Count);
			if (!isValid)
				RichEditExceptions.ThrowInvalidOperationException(XtraRichEditStringId.Msg_UseDeletedTableRowError);
		}
		public void Delete() {
			Table.Rows.RemoveAt(rowIndex);
		}
	}
	#endregion
	#region NativeTableRowCollection
	public class NativeTableRowCollection : TableRowCollection {
		readonly Dictionary<int, NativeTableRow> cachedItems = new Dictionary<int, NativeTableRow>();
		readonly NativeTable table;
		internal NativeTableRowCollection(NativeTable table) {
			Guard.ArgumentNotNull(table, "table");
			this.table = table;
		}
		ModelPieceTable PieceTable { get { return table.Document.PieceTable; } }
		public int Count { get { return table.ModelTable.Rows.Count; } }
		public TableRow First { get { return Count > 0 ? GetItem(0) : null; } }
		public TableRow Last {
			get {
				int count = Count;
				return count > 0 ? GetItem(count - 1) : null;
			}
		}
		#region ISimpleCollection<TableRow> Members
		TableRow ISimpleCollection<TableRow>.this[int index] {
			get { return GetItem(index); }
		}
		#endregion
		#region IEnumerable<TableRow> Members
		IEnumerator<TableRow> IEnumerable<TableRow>.GetEnumerator() {
			for (int i = 0; i < Count; i++)
				yield return GetItem(i);
		}
		#endregion
		#region IEnumerable Members
		IEnumerator IEnumerable.GetEnumerator() {
			for (int i = 0; i < Count; i++)
				yield return GetItem(i);
		}
		#endregion
		#region ICollection Members
		void ICollection.CopyTo(Array array, int index) {
			List<TableRow> result = new List<TableRow>();
			int count = Count;
			for (int i = 0; i < count; i++)
				result.Add(GetItem(i));
			Array.Copy(result.ToArray(), 0, array, index, count);
		}
		bool ICollection.IsSynchronized {
			get {
				return false;
			}
		}
		object ICollection.SyncRoot {
			get {
				return this;
			}
		}
		#endregion
		TableRow GetItem(int index) {
			Guard.ArgumentNonNegative(index, "index");
			if (index >= Count) {
				if (cachedItems.ContainsKey(index))
					cachedItems.Remove(index);
				throw new ArgumentException("index");
			}
			NativeTableRow row;
			if (!cachedItems.TryGetValue(index, out row)) {
				row = new NativeTableRow(table, index);
				cachedItems.Add(index, row);
			}
			return row;
		}
		public TableRow Append() {
			int count = Count;
			if (count <= 0)
				return null;
			PieceTable.InsertTableRowBelow(table.ModelTable.Rows[count - 1], false);
			return Last;
		}
		public TableRow InsertBefore(int rowIndex) {
			if (rowIndex < 0 || rowIndex >= Count)
				throw new ArgumentException("rowIndex");
			PieceTable.InsertTableRowAbove(table.ModelTable.Rows[rowIndex], false);
			return GetItem(rowIndex);
		}
		public TableRow InsertAfter(int rowIndex) {
			if (rowIndex < 0 || rowIndex >= Count)
				throw new ArgumentException("rowIndex");
			PieceTable.InsertTableRowBelow(table.ModelTable.Rows[rowIndex], false);
			return GetItem(rowIndex + 1);
		}
		public void RemoveAt(int rowIndex) {
			if (rowIndex < 0 || rowIndex >= Count)
				return;
			PieceTable.DocumentModel.BeginUpdate();
			try {
				if (Count == 1) {
					PieceTable.DeleteTableWithContent(table.ModelTable);
				}else {
					PieceTable.DeleteTableRowWithContent(table.ModelTable.Rows[rowIndex]);
					table.ModelTable.NormalizeCellColumnSpans();
				}
			}
			finally {
				PieceTable.DocumentModel.EndUpdate();
			}
		}
	}
	#endregion
	#region NativeTableCell
	public class NativeTableCell : TableCell {
		#region Fields
		readonly NativeTableRow row;
		readonly int columnIndex;
		NativeTableCellBorders borders;
		bool isValid;
		#endregion
		internal NativeTableCell(NativeTableRow row, int columnIndex) {
			Guard.ArgumentNotNull(row, "row");
			Guard.ArgumentNonNegative(columnIndex, "columnIndex");
			this.row = row;
			this.columnIndex = columnIndex;
			this.isValid = true;
		}
		#region Properties
		public ModelTable ModelTable {
			get {
				CheckValid();
				return row.ModelTable;
			}
		}
		public ModelTableRow ModelRow {
			get {
				CheckValid();
				return row.ModelRow;
			}
		}
		public ModelTableCell ModelCell {
			get {
				CheckValid();
				return row.ModelRow.Cells[columnIndex];
			}
		}
		public bool IsValid { get { return isValid; } set { isValid = value; } }
		public Table Table { get { return row.Table; } }
		public TableRow Row { get { return row; } }
		public NativeTable NativeTable { get { return row.NativeTable; } }
		public NativeSubDocument Document { get { return NativeTable.Document; } }
		#region Range
		public DocumentRange Range {
			get {
				ModelParagraph startParagraph = Document.PieceTable.Paragraphs[ModelCell.StartParagraphIndex];
				ModelParagraph endParagraph = Document.PieceTable.Paragraphs[ModelCell.EndParagraphIndex];
				return Document.CreateRange(startParagraph.LogPosition, endParagraph.LogPosition + endParagraph.Length - startParagraph.LogPosition);
			}
		}
		public DocumentRange ContentRange {
			get {
				ModelParagraph startParagraph = Document.PieceTable.Paragraphs[ModelCell.StartParagraphIndex];
				ModelParagraph endParagraph = Document.PieceTable.Paragraphs[ModelCell.EndParagraphIndex];
				return Document.CreateRange(startParagraph.LogPosition, endParagraph.LogPosition + endParagraph.Length - startParagraph.LogPosition - 1);
			}
		}
		#endregion
		public int NestingLevel { get { return Table.NestingLevel; } }
		public int Index { get { return columnIndex; } }
		#region Next
		public TableCell Next {
			get {
				CheckValid();
				if (columnIndex + 1 < row.Cells.Count)
					return row.Cells[columnIndex + 1];
				else
					return null;
			}
		}
		#endregion
		#region Previous
		public TableCell Previous {
			get {
				CheckValid();
				if (columnIndex > 0)
					return row.Cells[columnIndex - 1];
				else
					return null;
			}
		}
		#endregion
		public Color BackgroundColor { get { return ModelCell.BackgroundColor; } set { ModelCell.Properties.BackgroundColor = value; } }
		public TableCellVerticalAlignment VerticalAlignment { get { return (TableCellVerticalAlignment)ModelCell.VerticalAlignment; } set { ModelCell.Properties.VerticalAlignment = (DevExpress.XtraRichEdit.Model.VerticalAlignment)value; } }
		public bool WordWrap { get { return !ModelCell.NoWrap; } set { ModelCell.Properties.NoWrap = !value; } }
		#region TopPadding
		public float TopPadding {
			get { return Document.GetWidthUnitFixedValue(ModelCell.GetActualTopMargin()); }
			set { Document.SetWidthUnitFixedValue(ModelCell.Properties.CellMargins.Top, value); }
		}
		#endregion
		#region BottomPadding
		public float BottomPadding {
			get { return Document.GetWidthUnitFixedValue(ModelCell.GetActualBottomMargin()); }
			set { Document.SetWidthUnitFixedValue(ModelCell.Properties.CellMargins.Bottom, value); }
		}
		#endregion
		#region LeftPadding
		public float LeftPadding {
			get { return Document.GetWidthUnitFixedValue(ModelCell.GetActualLeftMargin()); }
			set { Document.SetWidthUnitFixedValue(ModelCell.Properties.CellMargins.Left, value); }
		}
		#endregion
		#region RightPadding
		public float RightPadding {
			get { return Document.GetWidthUnitFixedValue(ModelCell.GetActualRightMargin()); }
			set { Document.SetWidthUnitFixedValue(ModelCell.Properties.CellMargins.Right, value); }
		}
		#endregion
		public float PreferredWidth {
			get { return Document.GetWidthValue(ModelCell.PreferredWidth); }
			set { Document.SetWidthValue(ModelCell.Properties.PreferredWidth, value); }
		}
		public WidthType PreferredWidthType { get { return (WidthType)ModelCell.PreferredWidth.Type; } set { ModelCell.Properties.PreferredWidth.Type = (DevExpress.XtraRichEdit.Model.WidthUnitType)value; } }
		public float Height { get { return row.Height; } set { row.Height = value; } }
		public HeightType HeightType { get { return row.HeightType; } set { row.HeightType = value; } }
		public TableCellBorders Borders {
			get {
				if (borders == null)
					borders = new NativeTableCellBorders(this);
				return borders;
			}
		}
		#endregion
		internal void CheckValid() {
			row.CheckValid();
			if (isValid)
				isValid = (columnIndex <= row.ModelRow.Cells.Count);
			if (!isValid)
				RichEditExceptions.ThrowInvalidOperationException(XtraRichEditStringId.Msg_UseDeletedTableCellError);
		}
		public void Split(int rowsCount, int columnCount) {
			ModelPieceTable pieceTable = row.ModelTable.PieceTable;
			DocumentModel documentModel = Document.DocumentModel;
			documentModel.BeginUpdate();
			try {
				pieceTable.SplitTableCellsHorizontally(row.ModelRow.Cells[columnIndex], columnCount, Document.DocumentServer.Owner);
				pieceTable.SplitTableCellsVertically(row.ModelRow.Cells[columnIndex], rowsCount, columnCount, false);
			}
			finally {
				documentModel.EndUpdate();
			}
		}
		public void Delete() {
			row.Cells.RemoveAt(columnIndex);
		}
		#region Style
		public virtual TableCellStyle Style {
			get {
				bool isTableCellStyleAvailable = this.Document.DocumentModel.ActivePieceTable.IsTableCellStyleAvailable;
				if (!isTableCellStyleAvailable)
					throw new NotSupportedException("This property may take effect only in a Snap application.");
				CheckValid();
				return Document.GetTableCellStyle(ModelCell);
			}
			set {
				bool isTableCellStyleAvailable = this.Document.DocumentModel.ActivePieceTable.IsTableCellStyleAvailable;
				if (!isTableCellStyleAvailable)
					throw new NotSupportedException("This property may take effect only in a Snap application.");
				CheckValid();
				Model.TableCellStyle style = value != null ? Document.GetInnerTableCellStyle(value) : null;
				ModelCell.StyleIndex = (ModelTable.DocumentModel.TableCellStyles.IndexOf(style));
			}
		}
		#endregion
	}
	#endregion
	#region NativeTableCellCollection
	public class NativeTableCellCollection : TableCellCollection {
		readonly Dictionary<int, NativeTableCell> cachedItems = new Dictionary<int, NativeTableCell>();
		readonly NativeTableRow row;
		internal NativeTableCellCollection(NativeTableRow row) {
			Guard.ArgumentNotNull(row, "row");
			this.row = row;
		}
		#region Properties
		NativeSubDocument Document { get { return row.NativeTable.Document; } }
		ModelPieceTable PieceTable { get { return Document.PieceTable; } }
		public int Count { get { return row.ModelRow.Cells.Count; } }
		public TableCell First { get { return Count > 0 ? GetItem(0) : null; } }
		public TableCell Last {
			get {
				int count = Count;
				return count > 0 ? GetItem(count - 1) : null;
			}
		}
		#endregion
		TableCell GetItem(int index) {
			Guard.ArgumentNonNegative(index, "index");
			if (index >= Count) {
				if (cachedItems.ContainsKey(index))
					cachedItems.Remove(index);
				throw new ArgumentException("index");
			}
			NativeTableCell cell;
			if (!cachedItems.TryGetValue(index, out cell)) {
				cell = new NativeTableCell(row, index);
				cachedItems.Add(index, cell);
			}
			return cell;
		}
		public TableCell Append() {
			if (Count <= 0)
				throw new ArgumentException("rowIndex");
			ModelTableCell cell = row.ModelRow.Cells.Last;
			PieceTable.InsertColumnToTheRight(cell, false);
			return Last;
		}
		public TableCell InsertBefore(int columnIndex) {
			if (columnIndex < 0 || columnIndex >= Count)
				throw new ArgumentException("rowIndex");
			ModelTableCell cell = row.ModelRow.Cells[columnIndex];
			PieceTable.InsertColumnToTheLeft(cell, false);
			return GetItem(columnIndex);
		}
		public TableCell InsertAfter(int columnIndex) {
			if (columnIndex < 0 || columnIndex >= Count)
				throw new ArgumentException("rowIndex");
			ModelTableCell cell = row.ModelRow.Cells[columnIndex];
			PieceTable.InsertColumnToTheRight(cell, false);
			return GetItem(columnIndex + 1);
		}
		public void RemoveAt(int columnIndex) {
			if (columnIndex < 0 || columnIndex >= Count)
				return;
			PieceTable.DeleteTableCellWithContent(row.ModelRow.Cells[columnIndex], Document.DocumentServer.Owner);
		}
		#region ISimpleCollection<TableCell> Members
		TableCell ISimpleCollection<TableCell>.this[int index] {
			get { return GetItem(index); }
		}
		#endregion
		#region IEnumerable<TableCell> Members
		IEnumerator<TableCell> IEnumerable<TableCell>.GetEnumerator() {
			for (int i = 0; i < Count; i++)
				yield return GetItem(i);
		}
		#endregion
		#region IEnumerable Members
		IEnumerator IEnumerable.GetEnumerator() {
			for (int i = 0; i < Count; i++)
				yield return GetItem(i);
		}
		#endregion
		#region ICollection Members
		void ICollection.CopyTo(Array array, int index) {
			List<TableCell> result = new List<TableCell>();
			int count = Count;
			for (int i = 0; i < count; i++)
				result.Add(GetItem(i));
			Array.Copy(result.ToArray(), 0, array, index, count);
		}
		bool ICollection.IsSynchronized {
			get {
				return false;
			}
		}
		object ICollection.SyncRoot {
			get {
				return this;
			}
		}
		#endregion
	}
	#endregion
	#region NativeTableCellBorders
	public class NativeTableCellBorders : TableCellBorders {
		#region Fields
		readonly NativeTableCell cell;
		NativeTableCellBorderBase left;
		NativeTableCellBorderBase right;
		NativeTableCellBorderBase top;
		NativeTableCellBorderBase bottom;
		#endregion
		internal NativeTableCellBorders(NativeTableCell cell) {
			Guard.ArgumentNotNull(cell, "cell");
			this.cell = cell;
		}
		#region Properties
		public TableCellBorder Left {
			get {
				if (left == null)
					left = new NativeTableCellLeftBorder(cell);
				return left;
			}
		}
		public TableCellBorder Right {
			get {
				if (right == null)
					right = new NativeTableCellRightBorder(cell);
				return right;
			}
		}
		public TableCellBorder Top {
			get {
				if (top == null)
					top = new NativeTableCellTopBorder(cell);
				return top;
			}
		}
		public TableCellBorder Bottom {
			get {
				if (bottom == null)
					bottom = new NativeTableCellBottomBorder(cell);
				return bottom;
			}
		}
		#endregion
	}
	#endregion
	#region NativeStyleTableCellBorders
	public class NativeStyleTableCellBorders : TableCellBorders {
		#region Fields
		readonly Model.TableCellBorders cellBorders;
		readonly NativeSubDocument document;
		NativeTableCellBorderBase left;
		NativeTableCellBorderBase right;
		NativeTableCellBorderBase top;
		NativeTableCellBorderBase bottom;
		#endregion
		internal NativeStyleTableCellBorders(NativeSubDocument document, Model.TableCellBorders cellBorders) {
			Guard.ArgumentNotNull(cellBorders, "cellBorders");
			this.cellBorders = cellBorders;
			this.document = document;
		}
		#region Properties
		public TableCellBorder Left {
			get {
				if (left == null)
					left = new NativeStyleTableCellLeftBorder(document.UnitConverter.Converter, cellBorders);
				return left;
			}
		}
		public TableCellBorder Right {
			get {
				if (right == null)
					right = new NativeStyleTableCellRightBorder(document.UnitConverter.Converter, cellBorders);
				return right;
			}
		}
		public TableCellBorder Top {
			get {
				if (top == null)
					top = new NativeStyleTableCellTopBorder(document.UnitConverter.Converter, cellBorders);
				return top;
			}
		}
		public TableCellBorder Bottom {
			get {
				if (bottom == null)
					bottom = new NativeStyleTableCellBottomBorder(document.UnitConverter.Converter, cellBorders);
				return bottom;
			}
		}
		#endregion
	}
	#endregion
	#region NativeTableCellBorder (abstract class)
	public abstract class NativeTableCellBorderBase : TableCellBorder , TableBorder {
		readonly DocumentModelUnitConverter converter;
		internal NativeTableCellBorderBase(DocumentModelUnitConverter converter) {
			Guard.ArgumentNotNull(converter, "converter");
			this.converter = converter;
		}
		DocumentModelUnitConverter Converter { get { return converter; } }
		public Color LineColor { 
			get { return GetActualBorder().Color; } 
			set {
				if (GetActualBorder().Color == value)
					return;
				BorderBase oldBorder = GetBorder();
				oldBorder.CopyFrom(GetActualBorder());
				oldBorder.Color = value;
			} 
		}
		public TableBorderLineStyle LineStyle { 
			get { return (TableBorderLineStyle)GetActualBorder().Style; }
			set {
				BorderLineStyle newValue = (DevExpress.XtraRichEdit.Model.BorderLineStyle)value;
				if (GetActualBorder().Style == newValue)
					return;
				BorderBase oldBorder = GetBorder();
				oldBorder.CopyFrom(GetActualBorder());
				oldBorder.Style = newValue;
			} 
		}
		public float LineThickness {
			get { return Converter.ModelUnitsToPointsF(GetActualBorder().Width); }
			set {
				int newValue = (int)Math.Round(Converter.PointsToModelUnitsF(value));
				if (GetActualBorder().Width == newValue)
					return;
				BorderBase oldBorder = GetBorder();
				oldBorder.CopyFrom(GetActualBorder());
				oldBorder.Width = newValue;
			}
		}
		protected internal abstract ModelTableBorder GetActualBorder();
		protected internal abstract ModelTableBorder GetBorder();
	}
	#endregion
	#region NativeTableCellBorder (absctract class)
	public abstract class NativeTableCellBorder : NativeTableCellBorderBase {
		readonly NativeTableCell cell;
		internal NativeTableCellBorder(NativeTableCell cell)
			: base(cell.Document.UnitConverter.Converter) {
			Guard.ArgumentNotNull(cell, "cell");
			this.cell = cell;
		}
		public NativeTableCell Cell { get { return cell; } }
	}
	#endregion
	#region NativeTableCellLeftBorder
	public class NativeTableCellLeftBorder : NativeTableCellBorder {
		internal NativeTableCellLeftBorder(NativeTableCell cell)
			: base(cell) {
		}
		protected internal override ModelTableBorder GetActualBorder() {
			return Cell.ModelCell.GetActualLeftCellBorder();
		}
		protected internal override ModelTableBorder GetBorder() {
			return Cell.ModelCell.Properties.Borders.LeftBorder;
		}
	}
	#endregion
	#region NativeTableCellRightBorder
	public class NativeTableCellRightBorder : NativeTableCellBorder {
		internal NativeTableCellRightBorder(NativeTableCell cell)
			: base(cell) {
		}
		protected internal override ModelTableBorder GetActualBorder() {
			return Cell.ModelCell.GetActualRightCellBorder();
		}
		protected internal override ModelTableBorder GetBorder() {
			return Cell.ModelCell.Properties.Borders.RightBorder;
		}
	}
	#endregion
	#region NativeTableCellTopBorder
	public class NativeTableCellTopBorder : NativeTableCellBorder {
		internal NativeTableCellTopBorder(NativeTableCell cell)
			: base(cell) {
		}
		protected internal override ModelTableBorder GetActualBorder() {
			return Cell.ModelCell.GetActualTopCellBorder();
		}
		protected internal override ModelTableBorder GetBorder() {
			return Cell.ModelCell.Properties.Borders.TopBorder;
		}
	}
	#endregion
	#region NativeTableCellBottomBorder
	public class NativeTableCellBottomBorder : NativeTableCellBorder {
		internal NativeTableCellBottomBorder(NativeTableCell cell)
			: base(cell) {
		}
		protected internal override ModelTableBorder GetActualBorder() {
			return Cell.ModelCell.GetActualBottomCellBorder();
		}
		protected internal override ModelTableBorder GetBorder() {
			return Cell.ModelCell.Properties.Borders.BottomBorder;
		}
	}
	#endregion
	#region NativeTableBorder (abstract class)
	public abstract class NativeTableBorderBase : NativeTableCellBorderBase {
		readonly TableProperties innerTableProperties;
		internal NativeTableBorderBase(DocumentModelUnitConverter converter, TableProperties modelTableProperties)
			: base(converter) {
			this.innerTableProperties = modelTableProperties;
		}
		public TableProperties InnerTableProperties {
			get { return innerTableProperties; }
		}
	}
	public abstract class NativeTableBorder : NativeTableBorderBase {
		readonly NativeTable table;
		internal NativeTableBorder (NativeTable table)
			:base(table.Document.UnitConverter.Converter, table.ModelTable.TableProperties){
				this.table = table;
		}
		public NativeTable Table { get { return table; } }
	}
	#endregion
	#region NativeTableLeftBorder
	public class NativeTableLeftBorder : NativeTableBorder {
		internal NativeTableLeftBorder(NativeTable table)
			: base(table) {
		}
		protected internal override ModelTableBorder GetActualBorder() {
			return Table.ModelTable.GetActualLeftBorder();
		}
		protected internal override ModelTableBorder GetBorder() {
			return Table.ModelTable.TableProperties.Borders.LeftBorder;
		}
	}
	#endregion
	#region NativeTableRightBorder
	public class NativeTableRightBorder : NativeTableBorder {
		internal NativeTableRightBorder(NativeTable table)
			: base(table) {
		}
		protected internal override ModelTableBorder GetActualBorder() {
			return Table.ModelTable.GetActualRightBorder();
		}
		protected internal override ModelTableBorder GetBorder() {
			return Table.ModelTable.TableProperties.Borders.RightBorder;
		}
	}
	#endregion
	#region NativeTableTopBorder
	public class NativeTableTopBorder : NativeTableBorder {
		internal NativeTableTopBorder(NativeTable table)
			: base(table) {
		}
		protected internal override ModelTableBorder GetActualBorder() {
			return Table.ModelTable.GetActualTopBorder();
		}
		protected internal override ModelTableBorder GetBorder() {
			return Table.ModelTable.TableProperties.Borders.TopBorder;
		}
	}
	#endregion
	#region NativeTableBottomBorder
	public class NativeTableBottomBorder : NativeTableBorder {
		internal NativeTableBottomBorder(NativeTable table)
			: base(table) {
		}
		protected internal override ModelTableBorder GetActualBorder() {
			return Table.ModelTable.GetActualBottomBorder();
		}
		protected internal override ModelTableBorder GetBorder() {
			return Table.ModelTable.TableProperties.Borders.BottomBorder;
		}
	}
	#endregion
	#region NativeStyleTableLeftBorder
	public class NativeStyleTableLeftBorder : NativeTableBorderBase {
		internal NativeStyleTableLeftBorder(DocumentModelUnitConverter converter, TableProperties modelTableProperties)
			: base(converter, modelTableProperties) {
		}
		protected internal override ModelTableBorder GetActualBorder() {
			return InnerTableProperties.Borders.LeftBorder;
		}
		protected internal override ModelTableBorder GetBorder() {
			return InnerTableProperties.Borders.LeftBorder;
		}
	}
	#endregion
	#region NativeStyleTableRightBorder
	public class NativeStyleTableRightBorder : NativeTableBorderBase {
		internal NativeStyleTableRightBorder(DocumentModelUnitConverter converter, TableProperties modelTableProperties)
			: base(converter, modelTableProperties) {
		}
		protected internal override ModelTableBorder GetActualBorder() {
			return InnerTableProperties.Borders.RightBorder;
		}
		protected internal override ModelTableBorder GetBorder() {
			return InnerTableProperties.Borders.RightBorder;
		}
	}
	#endregion
	#region NativeStyleTableTopBorder
	public class NativeStyleTableTopBorder : NativeTableBorderBase {
		internal NativeStyleTableTopBorder(DocumentModelUnitConverter converter, TableProperties modelTableProperties)
			: base(converter, modelTableProperties) {
		}
		protected internal override ModelTableBorder GetActualBorder() {
			return InnerTableProperties.Borders.TopBorder;
		}
		protected internal override ModelTableBorder GetBorder() {
			return InnerTableProperties.Borders.TopBorder;
		}
	}
	#endregion
	#region NativeStyleTableBottomBorder
	public class NativeStyleTableBottomBorder : NativeTableBorderBase {
		internal NativeStyleTableBottomBorder(DocumentModelUnitConverter converter, TableProperties modelTableProperties)
			: base(converter, modelTableProperties) {
		}
		protected internal override ModelTableBorder GetActualBorder() {
			return InnerTableProperties.Borders.BottomBorder;
		}
		protected internal override ModelTableBorder GetBorder() {
			return InnerTableProperties.Borders.BottomBorder;
		}
	}
	#endregion
	#region NativeTableInsideVerticalBorderBorder
	public class NativeTableInsideVerticalBorderBorder : NativeTableBorderBase {
		internal NativeTableInsideVerticalBorderBorder(DocumentModelUnitConverter converter, TableProperties modelTableProperties)
			: base(converter, modelTableProperties) {
		}
		protected internal override ModelTableBorder GetActualBorder() {
			return InnerTableProperties.Borders.InsideVerticalBorder;
		}
		protected internal override ModelTableBorder GetBorder() {
			return InnerTableProperties.Borders.InsideVerticalBorder;
		}
	}
	#endregion
	#region NativeTableInsideHorizontalBorder
	public class NativeTableInsideHorizontalBorder : NativeTableBorderBase {
		internal NativeTableInsideHorizontalBorder(DocumentModelUnitConverter converter, TableProperties modelTableProperties)
			: base(converter, modelTableProperties) {
		}
		protected internal override ModelTableBorder GetActualBorder() {
			return InnerTableProperties.Borders.InsideHorizontalBorder;
		}
		protected internal override ModelTableBorder GetBorder() {
			return InnerTableProperties.Borders.InsideHorizontalBorder;
		}
	}
	#endregion
	#region NativeStyleTableCellBorder (abstract class)
	public abstract class NativeStyleTableCellBorder : NativeTableCellBorderBase {
		readonly Model.TableCellBorders borders;
		internal NativeStyleTableCellBorder(DocumentModelUnitConverter converter, Model.TableCellBorders borders)
			: base(converter) {
			Guard.ArgumentNotNull(borders, "borders");
			this.borders = borders;
		}
		protected Model.TableCellBorders Borders { get { return borders; } }
	}
	#endregion
	#region NativeStyleTableCellTopBorder
	public class NativeStyleTableCellTopBorder : NativeStyleTableCellBorder {
		internal NativeStyleTableCellTopBorder(DocumentModelUnitConverter converter, Model.TableCellBorders borders)
			: base(converter, borders) {
		}
		protected internal override ModelTableBorder GetActualBorder() {
			return Borders.TopBorder;
		}
		protected internal override ModelTableBorder GetBorder() {
			return Borders.TopBorder;
		}
	}
	#endregion
	#region NativeStyleTableCellBottomBorder
	public class NativeStyleTableCellBottomBorder : NativeStyleTableCellBorder {
		internal NativeStyleTableCellBottomBorder(DocumentModelUnitConverter converter, Model.TableCellBorders borders)
			: base(converter, borders) {
		}
		protected internal override ModelTableBorder GetActualBorder() {
			return Borders.BottomBorder;
		}
		protected internal override ModelTableBorder GetBorder() {
			return Borders.BottomBorder;
		}
	}
	#endregion
	#region NativeStyleTableCellLeftBorder
	public class NativeStyleTableCellLeftBorder : NativeStyleTableCellBorder {
		internal NativeStyleTableCellLeftBorder(DocumentModelUnitConverter converter, Model.TableCellBorders borders)
			: base(converter, borders) {
		}
		protected internal override ModelTableBorder GetActualBorder() {
			return Borders.LeftBorder;
		}
		protected internal override ModelTableBorder GetBorder() {
			return Borders.LeftBorder;
		}
	}
	#endregion
	#region NativeStyleTableCellRightBorder
	public class NativeStyleTableCellRightBorder : NativeStyleTableCellBorder {
		internal NativeStyleTableCellRightBorder(DocumentModelUnitConverter converter, Model.TableCellBorders borders)
			: base(converter, borders) {
		}
		protected internal override ModelTableBorder GetActualBorder() {
			return Borders.RightBorder;
		}
		protected internal override ModelTableBorder GetBorder() {
			return Borders.RightBorder;
		}
	}
	#endregion
	#region NativeTableBorders
	public class NativeTableBorders : TableBorders {
		#region Fields
		readonly NativeTable table;
		NativeTableBorderBase left;
		NativeTableBorderBase right;
		NativeTableBorderBase top;
		NativeTableBorderBase bottom;
		NativeTableBorderBase insideHorizontalBorder;
		NativeTableBorderBase insideVerticalBorder;
		#endregion
		internal NativeTableBorders(NativeTable table) {
			Guard.ArgumentNotNull(table, "table");
			this.table = table;
		}
		#region Properties
		public TableBorder Left {
			get {
				if (left == null)
					left = new NativeTableLeftBorder(table);
				return left;
			}
		}
		public TableBorder Right {
			get {
				if (right == null)
					right = new NativeTableRightBorder(table);
				return right;
			}
		}
		public TableBorder Top {
			get {
				if (top == null)
					top = new NativeTableTopBorder(table);
				return top;
			}
		}
		public TableBorder Bottom {
			get {
				if (bottom == null)
					bottom = new NativeTableBottomBorder(table);
				return bottom;
			}
		}
		public TableBorder InsideHorizontalBorder {
			get {
				if (insideHorizontalBorder == null)
					insideHorizontalBorder = new NativeTableInsideHorizontalBorder(table.Document.UnitConverter.Converter, table.ModelTable.TableProperties);
				return insideHorizontalBorder;
			}
		}
		public TableBorder InsideVerticalBorder {
			get {
				if (insideVerticalBorder == null)
					insideVerticalBorder = new NativeTableInsideVerticalBorderBorder(table.Document.UnitConverter.Converter, table.ModelTable.TableProperties);
				return insideVerticalBorder;
			}
		}
		#endregion
	}
	#endregion
	#region NativeTableBorders
	public class NativeStyleTableBorders : TableBorders {
		#region Fields
		readonly Model.TableProperties tableProperties;
		readonly NativeDocument document;
		NativeTableBorderBase left;
		NativeTableBorderBase right;
		NativeTableBorderBase top;
		NativeTableBorderBase bottom;
		NativeTableBorderBase insideHorizontalBorder;
		NativeTableBorderBase insideVerticalBorder;
		#endregion
		internal NativeStyleTableBorders(NativeDocument document, Model.TableProperties tableProperties) {
			Guard.ArgumentNotNull(document, "document");
			Guard.ArgumentNotNull(tableProperties, "tableProperties");
			this.document  = document;
			this.tableProperties = tableProperties;
		}
		public NativeDocument Document { get { return document; } }
		public Model.TableProperties InnerTableProperties { get { return tableProperties; } }
		#region Properties
		public DocumentModelUnitConverter Converter { get { return document.UnitConverter.Converter; } }
		public TableBorder Left {
			get {
				if (left == null)
					left = new NativeStyleTableLeftBorder(Converter, InnerTableProperties);
				return left;
			}
		}
		public TableBorder Right {
			get {
				if (right == null)
					right = new NativeStyleTableRightBorder(Converter, InnerTableProperties);
				return right;
			}
		}
		public TableBorder Top {
			get {
				if (top == null)
					top = new NativeStyleTableTopBorder(Converter, InnerTableProperties);
				return top;
			}
		}
		public TableBorder Bottom {
			get {
				if (bottom == null)
					bottom = new NativeStyleTableBottomBorder(Converter, InnerTableProperties);
				return bottom;
			}
		}
		public TableBorder InsideHorizontalBorder {
			get {
				if (insideHorizontalBorder == null)
					insideHorizontalBorder = new NativeTableInsideHorizontalBorder(Converter, InnerTableProperties);
				return insideHorizontalBorder;
			}
		}
		public TableBorder InsideVerticalBorder {
			get {
				if (insideVerticalBorder == null)
					insideVerticalBorder = new NativeTableInsideVerticalBorderBorder(Converter, InnerTableProperties);
				return insideVerticalBorder;
			}
		}
		#endregion
	}
	#endregion
	#region NativeTablePropertiesBase (abstract class)
	public abstract class NativeTablePropertiesBase : TablePropertiesBase {
		#region Fields
		bool isValid = true;
		readonly NativeSubDocument document;
		PropertyAccessor<int?> topPadding;
		PropertyAccessor<int?> bottomPadding;
		PropertyAccessor<int?> leftPadding;
		PropertyAccessor<int?> rightPadding;
		PropertyAccessor<int?> tableCellSpacing;
		PropertyAccessor<Color?> tableBackgroundColor;
		PropertyAccessor<WidthType?> tableCellSpacingType;
		PropertyAccessor<TableRowAlignment?> tableAlignment;
		PropertyAccessor<TableLayoutType?> tableLayout;
		PropertyAccessor<bool?> resetAccessor;
		PropertyAccessor<TablePropertiesOptions.Mask?> resetMaskAccessor;
		NativeStyleTableBorders tableBorders;
		#endregion
		internal NativeTablePropertiesBase(NativeSubDocument document) {
			Guard.ArgumentNotNull(document, "document");
			this.document = document;
		}
		public bool IsValid { get { return isValid; } set { isValid = value; } }
		public NativeSubDocument Document { get { return document; } }
		#region TopPadding
		public float? TopPadding {
			get {
				CheckValid();
				return Document.ModelUnitsToUnits(topPadding.GetValue());
			}
			set {
				CheckValid();
				if (!value.HasValue)
					return;
				if (topPadding.SetValue(Document.UnitsToModelUnits(value.Value)))
					RaiseChanged();
			}
		}
		#endregion 
		#region BottomPadding
		public float? BottomPadding {
			get {
				CheckValid();
				return Document.ModelUnitsToUnits(bottomPadding.GetValue());
			}
			set {
				CheckValid();
				if (!value.HasValue)
					return;
				if (bottomPadding.SetValue(Document.UnitsToModelUnits(value.Value)))
					RaiseChanged();
			}
		}
		#endregion
		#region LeftPadding
		public float? LeftPadding {
			get {
				CheckValid();
				return Document.ModelUnitsToUnits(leftPadding.GetValue());
			}
			set {
				CheckValid();
				if (!value.HasValue)
					return;
				if (leftPadding.SetValue(Document.UnitsToModelUnits(value.Value)))
					RaiseChanged();
			}
		}
		#endregion
		#region RightPadding
		public float? RightPadding {
			get {
				CheckValid();
				return Document.ModelUnitsToUnits(rightPadding.GetValue());
			}
			set {
				CheckValid();
				if (!value.HasValue)
					return;
				if (rightPadding.SetValue(Document.UnitsToModelUnits(value.Value)))
					RaiseChanged();
			}
		} 
		#endregion
		#region TableCellSpacing
		public float? TableCellSpacing {
			get {
				CheckValid();
				return Document.ModelUnitsToUnits(tableCellSpacing.GetValue());
			}
			set {
				CheckValid();
				if (!value.HasValue)
					return;
				if (tableCellSpacing.SetValue(Document.UnitsToModelUnits(value.Value)))
					RaiseChanged();
			}
		}
		#endregion
		#region TableBackgroundColor
		public Color? TableBackgroundColor {
			get {
				CheckValid();
				return tableBackgroundColor.GetValue();
			}
			set {
				CheckValid();
				if (!value.HasValue)
					return;
				if (tableBackgroundColor.SetValue(value.Value))
					RaiseChanged();
			}
		}
		#endregion
		#region TableCellSpacingType
		public WidthType? CellSpacingType {
			get {
				CheckValid();
				return tableCellSpacingType.GetValue();
			}
			set {
				CheckValid();
				if (!value.HasValue)
					return;
				if (tableCellSpacingType.SetValue((WidthType)value.Value))
					RaiseChanged();
			}
		}
		#endregion
		#region TableAlignment
		public TableRowAlignment? TableAlignment {
			get {
				CheckValid();
				return tableAlignment.GetValue();
			}
			set {
				CheckValid();
				if (!value.HasValue)
					return;
				if (tableAlignment.SetValue(value.Value))
					RaiseChanged();
			}
		}
		#endregion
		#region TableLayout
		public TableLayoutType? TableLayout {
			get {
				CheckValid();
				return tableLayout.GetValue();
			}
			set {
				CheckValid();
				if (!value.HasValue)
					return;
				if (tableLayout.SetValue(value.Value))
					RaiseChanged();
			}
		}
		#endregion
		public TableBorders TableBorders { 
			get {
				if (tableBorders == null)
					tableBorders = CreateNativeStyleTableBorders();
				return tableBorders; 
			}
		}
		public virtual void Reset() {
			CheckValid();
			if (resetAccessor.SetValue(true))
				RaiseChanged();
		}
		public void Reset(TablePropertiesMask mask) {
			CheckValid();
			if (resetMaskAccessor.SetValue((TablePropertiesOptions.Mask)mask))
				RaiseChanged();
		}
		protected void CheckValid() {
			if (!isValid)
				RichEditExceptions.ThrowInvalidOperationException(XtraRichEditStringId.Msg_UseInvalidParagraphProperties);
		}
		protected internal virtual void CreateAccessors() {
			this.topPadding = CreateTopPaddingAccessor();
			this.bottomPadding = CreateBottomPaddingAccessor();
			this.leftPadding = CreateLeftPaddingAccessor();
			this.rightPadding = CreateRightPaddingAccessor();
			this.tableAlignment = CreateTableAlignmentAccessor();
			this.tableLayout = CreateTableLayoutAccessor();
			this.resetAccessor = CreateResetAccessor();
			this.resetMaskAccessor = CreateResetMaskAccessor();
			this.tableCellSpacing = CreateTableCellSpacingAccessor();
			this.tableCellSpacingType = CreateTableCellSpacingTypeAccessor();
			this.tableBackgroundColor = CreateTableBackgroundColorAccessor();
		}
		protected internal abstract void RaiseChanged();
		protected abstract PropertyAccessor<int?> CreateTopPaddingAccessor();
		protected abstract PropertyAccessor<int?> CreateBottomPaddingAccessor();
		protected abstract PropertyAccessor<int?> CreateLeftPaddingAccessor();
		protected abstract PropertyAccessor<int?> CreateRightPaddingAccessor();
		protected abstract PropertyAccessor<TableRowAlignment?> CreateTableAlignmentAccessor();
		protected abstract PropertyAccessor<TableLayoutType?> CreateTableLayoutAccessor();
		protected abstract PropertyAccessor<int?> CreateTableCellSpacingAccessor();
		protected abstract PropertyAccessor<WidthType?> CreateTableCellSpacingTypeAccessor();
		protected abstract PropertyAccessor<Color?> CreateTableBackgroundColorAccessor();
		protected abstract PropertyAccessor<bool?> CreateResetAccessor();
		protected abstract PropertyAccessor<TablePropertiesOptions.Mask?> CreateResetMaskAccessor();
		internal abstract NativeStyleTableBorders CreateNativeStyleTableBorders();
	}
	#endregion
	#region NativeStyleTableProperties
	public class NativeStyleTableProperties : NativeTablePropertiesBase {
		readonly Model.TableProperties innerTableProperties;
		internal NativeStyleTableProperties(NativeDocument document, Model.TableProperties modelTableProperties)
			: base(document) {
			Guard.ArgumentNotNull(modelTableProperties, "modelTableProperties");
			this.innerTableProperties = modelTableProperties;
			CreateAccessors();
		}
		protected internal virtual Model.TableProperties ModelTableProperties { get { return innerTableProperties; } }
		protected override PropertyAccessor<int?> CreateTopPaddingAccessor() {
			return new TopPaddingAccessor(ModelTableProperties);
		}
		protected override PropertyAccessor<int?> CreateBottomPaddingAccessor() {
			return new BottomPaddingAccessor(ModelTableProperties);
		}
		protected override PropertyAccessor<int?> CreateLeftPaddingAccessor() {
			return new LeftPaddingAccessor(ModelTableProperties);
		}
		protected override PropertyAccessor<int?> CreateRightPaddingAccessor() {
			return new RightPaddingAccessor(ModelTableProperties);
		}
		protected override PropertyAccessor<TableRowAlignment?> CreateTableAlignmentAccessor() {
			return new TableAlignmentAccessor(ModelTableProperties);
		}
		protected override PropertyAccessor<TableLayoutType?> CreateTableLayoutAccessor() {
			return new TableLayoutAccessor(ModelTableProperties);
		}
		protected override PropertyAccessor<int?> CreateTableCellSpacingAccessor() {
			return new TableCellSpacingAccessor(ModelTableProperties);
		}
		protected override PropertyAccessor<bool?> CreateResetAccessor() {
			return new ResetUseAccessor(ModelTableProperties);
		}
		protected override PropertyAccessor<TablePropertiesOptions.Mask?> CreateResetMaskAccessor() {
			return new ResetUseMaskAccessor(ModelTableProperties);
		}
		protected override PropertyAccessor<WidthType?> CreateTableCellSpacingTypeAccessor() {
			return new TableCellSpacingTypeAccessor(ModelTableProperties);
		}
		protected override PropertyAccessor<Color?> CreateTableBackgroundColorAccessor() {
			return new TableBackgroundColorAccessor(ModelTableProperties);
		}
		protected internal override void RaiseChanged() {
		}
		#region ModelTablePropertiesPropertyAccessor<T> (abstract class)
		abstract class ModelTablePropertiesPropertyAccessor<T> : PropertyAccessor<T> {
			readonly Model.TableProperties properties;
			public ModelTablePropertiesPropertyAccessor(Model.TableProperties properties) {
				this.properties = properties;
			}
			public Model.TableProperties Properties { get { return properties; } }
		}
		#endregion
		internal override NativeStyleTableBorders CreateNativeStyleTableBorders() {
			return new NativeStyleTableBorders(Document.MainDocument, ModelTableProperties);
		}
		class TopPaddingAccessor : ModelTablePropertiesPropertyAccessor<int?> {
			public TopPaddingAccessor(Model.TableProperties properties)
				: base(properties) {
			}
			public override int? GetValue() {
				if (!Properties.UseTopMargin)
					return null;
				return Properties.CellMargins.Top.Value;
			}
			public override bool SetValue(int? value) {
				if (!value.HasValue)
					return false;
				Properties.CellMargins.Top.Value = value.Value;
				Properties.CellMargins.Top.Type = WidthUnitType.ModelUnits;
				return true;
			}
		}
		class RightPaddingAccessor : ModelTablePropertiesPropertyAccessor<int?> {
			public RightPaddingAccessor(Model.TableProperties properties)
				: base(properties) {
			}
			public override int? GetValue() {
				if (!Properties.UseRightMargin)
					return null;
				return Properties.CellMargins.Right.Value;
			}
			public override bool SetValue(int? value) {
				if (!value.HasValue)
					return false;
				Properties.CellMargins.Right.Value = value.Value;
				Properties.CellMargins.Right.Type = WidthUnitType.ModelUnits;
				return true;
			}
		}
		class BottomPaddingAccessor : ModelTablePropertiesPropertyAccessor<int?> {
			public BottomPaddingAccessor(Model.TableProperties properties)
				: base(properties) {
			}
			public override int? GetValue() {
				if (!Properties.UseBottomMargin)
					return null;
				return Properties.CellMargins.Bottom.Value;
			}
			public override bool SetValue(int? value) {
				if (!value.HasValue)
					return false;
				Properties.CellMargins.Bottom.Value = value.Value;
				Properties.CellMargins.Bottom.Type = WidthUnitType.ModelUnits;
				return true;
			}
		}
		class LeftPaddingAccessor : ModelTablePropertiesPropertyAccessor<int?> {
			public LeftPaddingAccessor(Model.TableProperties properties)
				: base(properties) {
			}
			public override int? GetValue() {
				if (!Properties.UseLeftMargin)
					return null;
				return Properties.CellMargins.Left.Value;
			}
			public override bool SetValue(int? value) {
				if (!value.HasValue)
					return false;
				Properties.CellMargins.Left.Value = value.Value;
				Properties.CellMargins.Left.Type = WidthUnitType.ModelUnits;
				return true;
			}
		}
		class TableAlignmentAccessor : ModelTablePropertiesPropertyAccessor<TableRowAlignment?> {
			public TableAlignmentAccessor(Model.TableProperties properties)
				: base(properties) {
			}
			public override TableRowAlignment? GetValue() {
				if (!Properties.UseTableAlignment)
					return null;
				return (TableRowAlignment)Properties.TableAlignment;
			}
			public override bool SetValue(TableRowAlignment? value) {
				if (!value.HasValue)
					return false;
				Properties.TableAlignment = (Model.TableRowAlignment)value.Value;
				return true;
			}
		}
		class TableLayoutAccessor : ModelTablePropertiesPropertyAccessor<TableLayoutType?> {
			public TableLayoutAccessor(Model.TableProperties properties)
				: base(properties) {
			}
			public override TableLayoutType? GetValue() {
				if (!Properties.UseTableLayout)
					return null;
				return (TableLayoutType)Properties.TableLayout;
			}
			public override bool SetValue(TableLayoutType? value) {
				if (!value.HasValue)
					return false;
				Properties.TableLayout = (Model.TableLayoutType)value.Value;
				return true;
			}
		}
		class TableCellSpacingAccessor : ModelTablePropertiesPropertyAccessor<int?> {
			public TableCellSpacingAccessor(Model.TableProperties properties)
				: base(properties) {
			}
			public override int? GetValue() {
				if (!Properties.UseCellSpacing)
					return null;
				return Properties.CellSpacing.Value;
			}
			public override bool SetValue(int? value) {
				if (!value.HasValue)
					return false;
				Properties.CellSpacing.Value = value.Value;
				return true;
			}
		}
		class TableCellSpacingTypeAccessor : ModelTablePropertiesPropertyAccessor<WidthType?> {
			public TableCellSpacingTypeAccessor(Model.TableProperties properties)
				: base(properties) {
			}
			public override WidthType? GetValue() {
				if (!Properties.UseCellSpacing)
					return null;
				return (WidthType)Properties.CellSpacing.Type;
			}
			public override bool SetValue(WidthType? value) {
				if (!value.HasValue)
					return false;
				Properties.CellSpacing.Type = (Model.WidthUnitType)value.Value;
				return true;
			}
		}
		class TableBackgroundColorAccessor : ModelTablePropertiesPropertyAccessor<Color?> {
			public TableBackgroundColorAccessor(Model.TableProperties properties)
				: base(properties) {
			}
			public override Color? GetValue() {
				if (!Properties.UseBackgroundColor)
					return null;
				return Properties.BackgroundColor;
			}
			public override bool SetValue(Color? value) {
				if (!value.HasValue)
					return false;
				Properties.BackgroundColor = value.Value;
				return true;
			}
		}
		class ResetUseAccessor : ModelTablePropertiesPropertyAccessor<bool?> {
			public ResetUseAccessor(Model.TableProperties properties)
				: base(properties) {
			}
			public override bool? GetValue() {
				return Properties.Info.Value == TablePropertiesOptions.Mask.UseNone;
			}
			public override bool SetValue(bool? value) {
				Properties.ResetAllUse();
				return true;
			}
		}
		#region ResetUseMaskAccessor
		class ResetUseMaskAccessor : ModelTablePropertiesPropertyAccessor<TablePropertiesOptions.Mask?> {
			public ResetUseMaskAccessor(Model.TableProperties properties)
				: base(properties) {
			}
			public override TablePropertiesOptions.Mask? GetValue() {
				return Properties.Info.Value;
			}
			public override bool SetValue(TablePropertiesOptions.Mask? value) {
				if (!value.HasValue)
					return false;
				Properties.ResetUse(value.Value);
				return true;
			}
		}
		#endregion
	}
	#endregion
	#region NativeDefaultTableProperties
	public class NativeDefaultTableProperties : NativeStyleTableProperties {
		internal NativeDefaultTableProperties(NativeDocument document, Model.TableProperties properties)
			: base(document, properties) {
		}
		public override void Reset() {
			RichEditExceptions.ThrowInvalidOperationException(DevExpress.XtraRichEdit.Localization.XtraRichEditStringId.Msg_CantResetDefaultProperties);
		}
	}
	#endregion
	#region NativeTableCellPropertiesBase (abstract class)
	public abstract class NativeTableCellPropertiesBase : TableCellPropertiesBase {
		#region Fields
		bool isValid = true;
		readonly NativeSubDocument document;
		PropertyAccessor<int?> topPadding;
		PropertyAccessor<int?> bottomPadding;
		PropertyAccessor<int?> leftPadding;
		PropertyAccessor<int?> rightPadding;
		PropertyAccessor<Color?> backColor;
		PropertyAccessor<TableCellVerticalAlignment?> verticalAlignment;
		PropertyAccessor<bool?> noWrap;
		PropertyAccessor<bool?> resetAccessor;
		PropertyAccessor<TableCellPropertiesOptions.Mask?> resetMaskAccessor;
		TableCellBorders tableCellBorders = null;
		#endregion
		internal NativeTableCellPropertiesBase(NativeSubDocument document) {
			Guard.ArgumentNotNull(document, "document");
			this.document = document;
		}
		public bool IsValid { get { return isValid; } set { isValid = value; } }
		public NativeSubDocument Document { get { return document; } }
		#region CellTopPadding
		public float? CellTopPadding {
			get {
				CheckValid();
				return Document.ModelUnitsToUnits(topPadding.GetValue());
			}
			set {
				CheckValid();
				if (!value.HasValue)
					return;
				if (topPadding.SetValue(Document.UnitsToModelUnits(value.Value)))
					RaiseChanged();
			}
		}
		#endregion
		#region CellBottomPadding
		public float? CellBottomPadding {
			get {
				CheckValid();
				return Document.ModelUnitsToUnits(bottomPadding.GetValue());
			}
			set {
				CheckValid();
				if (!value.HasValue)
					return;
				if (bottomPadding.SetValue(Document.UnitsToModelUnits(value.Value)))
					RaiseChanged();
			}
		}
		#endregion
		#region CellLeftPadding
		public float? CellLeftPadding {
			get {
				CheckValid();
				return Document.ModelUnitsToUnits(leftPadding.GetValue());
			}
			set {
				CheckValid();
				if (!value.HasValue)
					return;
				if (leftPadding.SetValue(Document.UnitsToModelUnits(value.Value)))
					RaiseChanged();
			}
		}
		#endregion
		#region CellRightPadding
		public float? CellRightPadding {
			get {
				CheckValid();
				return Document.ModelUnitsToUnits(rightPadding.GetValue());
			}
			set {
				CheckValid();
				if (!value.HasValue)
					return;
				if (rightPadding.SetValue(Document.UnitsToModelUnits(value.Value)))
					RaiseChanged();
			}
		}
		#endregion
		#region BackColor
		public Color? CellBackgroundColor {
			get {
				CheckValid();
				return backColor.GetValue();
			}
			set {
				CheckValid();
				if (!value.HasValue)
					return;
				if (backColor.SetValue(value.Value))
					RaiseChanged();
			}
		}
		#endregion
		#region VerticalAlignment
		public TableCellVerticalAlignment? VerticalAlignment {
			get {
				CheckValid();
				return verticalAlignment.GetValue();
			}
			set {
				CheckValid();
				if (!value.HasValue)
					return;
				if (verticalAlignment.SetValue(value.Value))
					RaiseChanged();
			}
		}
		#endregion
		#region NoWrap
		public bool? NoWrap {
			get {
				CheckValid();
				return noWrap.GetValue();
			}
			set {
				CheckValid();
				if (!value.HasValue)
					return;
				if (noWrap.SetValue(value.Value))
					RaiseChanged();
			}
		}
		#endregion
		public TableCellBorders TableCellBorders {
			get {
				if (tableCellBorders == null)
					tableCellBorders = CreateTableCellBorders();
				return tableCellBorders;
			}
		}
		protected internal abstract TableCellBorders CreateTableCellBorders();
		public virtual void Reset() {
			CheckValid();
			if (resetAccessor.SetValue(true))
				RaiseChanged();
		}
		public void Reset(TableCellPropertiesMask mask) {
			CheckValid();
			if (resetMaskAccessor.SetValue((TableCellPropertiesOptions.Mask)mask))
				RaiseChanged();
		}
		protected void CheckValid() {
			if (!isValid)
				RichEditExceptions.ThrowInvalidOperationException(XtraRichEditStringId.Msg_UseInvalidParagraphProperties);
		}
		protected internal virtual void CreateAccessors() {
			this.topPadding = CreateTopPaddingAccessor();
			this.bottomPadding = CreateBottomPaddingAccessor();
			this.leftPadding = CreateLeftPaddingAccessor();
			this.rightPadding = CreateRightPaddingAccessor();
			this.backColor = CreateBackColorAccessor();
			this.verticalAlignment = CreateVerticalAlignmentAccessor();
			this.noWrap = CreateNoWrapAccessor();
			this.resetAccessor = CreateResetAccessor();
			this.resetMaskAccessor = CreateResetMaskAccessor();
		}
		protected internal abstract void RaiseChanged();
		protected abstract PropertyAccessor<int?> CreateTopPaddingAccessor();
		protected abstract PropertyAccessor<int?> CreateBottomPaddingAccessor();
		protected abstract PropertyAccessor<int?> CreateLeftPaddingAccessor();
		protected abstract PropertyAccessor<int?> CreateRightPaddingAccessor();
		protected abstract PropertyAccessor<Color?> CreateBackColorAccessor();
		protected abstract PropertyAccessor<TableCellVerticalAlignment?> CreateVerticalAlignmentAccessor();
		protected abstract PropertyAccessor<bool?> CreateNoWrapAccessor();
		protected abstract PropertyAccessor<bool?> CreateResetAccessor();
		protected abstract PropertyAccessor<TableCellPropertiesOptions.Mask?> CreateResetMaskAccessor();
	}
	#endregion
	#region NativeStyleTableCellProperties
	public class NativeStyleTableCellProperties : NativeTableCellPropertiesBase {
		readonly Model.TableCellProperties innerProperties;
		internal NativeStyleTableCellProperties(NativeDocument document, Model.TableCellProperties modelTableCellProperties)
			: base(document) {
			Guard.ArgumentNotNull(document, "document");
			Guard.ArgumentNotNull(modelTableCellProperties, "modelTableCellProperties");
			this.innerProperties = modelTableCellProperties;
			CreateAccessors();
		}
		protected internal virtual Model.TableCellProperties InnerProperties { get { return innerProperties; } }
		protected override PropertyAccessor<int?> CreateTopPaddingAccessor() {
			return new TopPaddingAccessor(InnerProperties);
		}
		protected override PropertyAccessor<int?> CreateBottomPaddingAccessor() {
			return new BottomPaddingAccessor(InnerProperties);
		}
		protected override PropertyAccessor<int?> CreateLeftPaddingAccessor() {
			return new LeftPaddingAccessor(InnerProperties);
		}
		protected override PropertyAccessor<int?> CreateRightPaddingAccessor() {
			return new RightPaddingAccessor(InnerProperties);
		}
		protected override PropertyAccessor<TableCellVerticalAlignment?> CreateVerticalAlignmentAccessor() {
			return new VerticalAlignmentAccessor(InnerProperties);
		}
		protected override PropertyAccessor<Color?> CreateBackColorAccessor() {
			return new BackColorAccessor(InnerProperties);
		}
		protected override PropertyAccessor<bool?> CreateNoWrapAccessor() {
			return new NoWrapAccessor(InnerProperties);
		}
		protected override PropertyAccessor<bool?> CreateResetAccessor() {
			return new ResetUseAccessor(InnerProperties);
		}
		protected override PropertyAccessor<TableCellPropertiesOptions.Mask?> CreateResetMaskAccessor() {
			return new ResetUseMaskAccessor(InnerProperties);
		}
		protected internal override TableCellBorders CreateTableCellBorders() {
			return new NativeStyleTableCellBorders(Document, InnerProperties.Borders);
		}
		protected internal override void RaiseChanged() {
		}
		#region ModelTableCellPropertiesPropertyAccessor<T> (abstract class)
		abstract class ModelTableCellPropertiesPropertyAccessor<T> : PropertyAccessor<T> {
			readonly Model.TableCellProperties properties;
			public ModelTableCellPropertiesPropertyAccessor(Model.TableCellProperties properties) {
				this.properties = properties;
			}
			public Model.TableCellProperties Properties { get { return properties; } }
		}
		#endregion
		#region TopPaddingAccessor
		class TopPaddingAccessor : ModelTableCellPropertiesPropertyAccessor<int?> {
			public TopPaddingAccessor(Model.TableCellProperties properties)
				: base(properties) {
			}
			public override int? GetValue() {
				if (!Properties.UseTopMargin)
					return null;
				return Properties.CellMargins.Top.Value;
			}
			public override bool SetValue(int? value) {
				if (!value.HasValue)
					return false;
				Properties.CellMargins.Top.Value = value.Value;
				Properties.CellMargins.Top.Type = WidthUnitType.ModelUnits;
				return true;
			}
		}
		#endregion
		#region RightPaddingAccessor
		class RightPaddingAccessor : ModelTableCellPropertiesPropertyAccessor<int?> {
			public RightPaddingAccessor(Model.TableCellProperties properties)
				: base(properties) {
			}
			public override int? GetValue() {
				if (!Properties.UseRightMargin)
					return null;
				return Properties.CellMargins.Right.Value;
			}
			public override bool SetValue(int? value) {
				if (!value.HasValue)
					return false;
				Properties.CellMargins.Right.Value = value.Value;
				Properties.CellMargins.Right.Type = WidthUnitType.ModelUnits;
				return true;
			}
		}
		#endregion
		#region BottomPaddingAccessor
		class BottomPaddingAccessor : ModelTableCellPropertiesPropertyAccessor<int?> {
			public BottomPaddingAccessor(Model.TableCellProperties properties)
				: base(properties) {
			}
			public override int? GetValue() {
				if (!Properties.UseBottomMargin)
					return null;
				return Properties.CellMargins.Bottom.Value;
			}
			public override bool SetValue(int? value) {
				if (!value.HasValue)
					return false;
				Properties.CellMargins.Bottom.Value = value.Value;
				Properties.CellMargins.Bottom.Type = WidthUnitType.ModelUnits;
				return true;
			}
		}
		#endregion
		#region LeftPaddingAccessor
		class LeftPaddingAccessor : ModelTableCellPropertiesPropertyAccessor<int?> {
			public LeftPaddingAccessor(Model.TableCellProperties properties)
				: base(properties) {
			}
			public override int? GetValue() {
				if (!Properties.UseLeftMargin)
					return null;
				return Properties.CellMargins.Left.Value;
			}
			public override bool SetValue(int? value) {
				if (!value.HasValue)
					return false;
				Properties.CellMargins.Left.Value = value.Value;
				Properties.CellMargins.Left.Type = WidthUnitType.ModelUnits;
				return true;
			}
		}
		#endregion
		#region VerticalAlignmentAccessor
		class VerticalAlignmentAccessor : ModelTableCellPropertiesPropertyAccessor<TableCellVerticalAlignment?> {
			public VerticalAlignmentAccessor(Model.TableCellProperties properties)
				: base(properties) {
			}
			public override TableCellVerticalAlignment? GetValue() {
				if (!Properties.UseVerticalAlignment)
					return null;
				return (TableCellVerticalAlignment)Properties.VerticalAlignment;
			}
			public override bool SetValue(TableCellVerticalAlignment? value) {
				if (!value.HasValue)
					return false;
				Properties.VerticalAlignment = (Model.VerticalAlignment)value.Value;
				return true;
			}
		}
		#endregion
		#region BackColorAccessor
		class BackColorAccessor : ModelTableCellPropertiesPropertyAccessor<Color?> {
			public BackColorAccessor(Model.TableCellProperties properties)
				: base(properties) {
			}
			public override Color? GetValue() {
				if (!Properties.UseBackgroundColor)
					return null;
				return Properties.BackgroundColor;
			}
			public override bool SetValue(Color? value) {
				if (!value.HasValue)
					return false;
				Properties.BackgroundColor = value.Value;
				return true;
			}
		}
		#endregion
		#region NoWrapAccessor
		class NoWrapAccessor : ModelTableCellPropertiesPropertyAccessor<bool?> {
			public NoWrapAccessor(Model.TableCellProperties properties)
				: base(properties) {
			}
			public override bool? GetValue() {
				if (!Properties.UseNoWrap)
					return null;
				return Properties.NoWrap;
			}
			public override bool SetValue(bool? value) {
				if (!value.HasValue)
					return false;
				Properties.NoWrap = value.Value;
				return true;
			}
		}
		#endregion
		#region ResetUseAccessor
		class ResetUseAccessor : ModelTableCellPropertiesPropertyAccessor<bool?> {
			public ResetUseAccessor(Model.TableCellProperties properties)
				: base(properties) {
			}
			public override bool? GetValue() {
				return Properties.Info.Value == TableCellPropertiesOptions.Mask.UseNone;
			}
			public override bool SetValue(bool? value) {
				Properties.ResetAllUse();
				return true;
			}
		}
		#endregion
		#region ResetUseMaskAccessor
		class ResetUseMaskAccessor : ModelTableCellPropertiesPropertyAccessor<TableCellPropertiesOptions.Mask?> {
			public ResetUseMaskAccessor(Model.TableCellProperties properties)
				: base(properties) {
			}
			public override TableCellPropertiesOptions.Mask? GetValue() {
				return Properties.Info.Value;
			}
			public override bool SetValue(TableCellPropertiesOptions.Mask? value) {
				if (!value.HasValue)
					return false;
				Properties.ResetUse(value.Value);
				return true;
			}
		}
		#endregion
	}
	#endregion
	#region NativeDefaultTableProperties
	public class NativeDefaultTableCellProperties : NativeStyleTableCellProperties {
		internal NativeDefaultTableCellProperties(NativeDocument document, Model.TableCellProperties properties)
			: base(document, properties) {
		}
		public override void Reset() {
			RichEditExceptions.ThrowInvalidOperationException(DevExpress.XtraRichEdit.Localization.XtraRichEditStringId.Msg_CantResetDefaultProperties);
		}
	}
	#endregion
}
