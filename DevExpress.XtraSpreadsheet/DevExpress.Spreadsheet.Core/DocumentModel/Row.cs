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
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using DevExpress.Utils;
using DevExpress.Office.Utils;
using DevExpress.XtraSpreadsheet.Utils;
using DevExpress.Office;
using DevExpress.Office.History;
using DevExpress.XtraSpreadsheet.Model.History;
using System.Runtime.InteropServices;
using DevExpress.XtraSpreadsheet.Localization;
using DevExpress.Office.Drawing;
using System.Threading;
using DevExpress.Spreadsheet;
using DevExpress.Export.Xl;
using DevExpress.Compatibility.System.Drawing;
#if SL
using System.Windows.Media;
#endif
namespace DevExpress.XtraSpreadsheet.Model {
	#region IRowBase
#if BTREE
	public interface IIndexedObject {
		long Index { get; }
	}
	public interface IRowBase : IIndexedObject {
		IWorksheet Sheet { get; }
		ICellCollectionBase Cells { get; }
		void OffsetRowIndex(int offset);
		void CheckIntegrity(CheckIntegrityFlags flags);
		bool IsVisible { get; }
		bool HasVisibleFill { get; }
		bool HasVisibleBorder { get; }
	}
#else
	public interface IRowBase {
		int Index { get; }
		IWorksheet Sheet { get; }
		ICellCollectionBase Cells { get; }
		void OffsetRowIndex(int offset);
		void CheckIntegrity(CheckIntegrityFlags flags);
		bool IsVisible { get; }
		bool HasVisibleFill { get; }
		bool HasVisibleBorder { get; }
	}
#endif
	#endregion
	#region IRow
	public interface IRow : IRowBase, ICellContainer, ICellFormat, IFormatBaseBatchUpdateable, IActualApplyInfo {
		new Worksheet Sheet { get; }
		int CellsCount { get; }
		int FirstColumnIndex { get; }
		int LastColumnIndex { get; }
		int FormatIndex { get; }
		ICell this[int index] { get; }
		float Height { get; set; }
		int OutlineLevel { get; set; }
		bool IsHidden { get; set; }
		bool IsCollapsed { get; set; }
		bool IsCustomHeight { get; set; }
		bool ApplyStyle { get; }
		bool IsThickTopBorder { get; set; }
		bool IsThickBottomBorder { get; set; }
		int CachedRowHeight { get; set; }
		void SetCachedRowHeight(int value);
		bool HasValidCachedRowHeight { get; }
		void ResetCachedTransactionVersions();
		bool HasSameFormatting(Row row);
		void OffsetRowIndexCore(int offset);
		CellIntervalRange GetCellIntervalRange();
		void CheckCellIntegrity(ICell cell, CheckIntegrityFlags flags);
		void SetCustomHeight(float valueInModelUnits);
		new ICellCollection Cells { get; }
	}
	#endregion
	#region Row
	public class Row : MultiIndexObject<Row, DocumentModelChangeActions>, IRow {
		readonly static RowCellFormatIndexAccessor cellFormatIndexAccessor = new RowCellFormatIndexAccessor();
		readonly static RowHeightIndexAccessor heightIndexAccessor = new RowHeightIndexAccessor();
		readonly static RowInfoIndexAccessor infoIndexAccessor = new RowInfoIndexAccessor();
		readonly static IIndexAccessorBase<Row, DocumentModelChangeActions>[] indexAccessors = new IIndexAccessorBase<Row, DocumentModelChangeActions>[] {
			cellFormatIndexAccessor,
			heightIndexAccessor,
			infoIndexAccessor,
		};
		public static RowCellFormatIndexAccessor CellFormatIndexAccessor { get { return cellFormatIndexAccessor; } }
		public static RowHeightIndexAccessor HeightIndexAccessor { get { return heightIndexAccessor; } }
		public static RowInfoIndexAccessor InfoIndexAccessor { get { return infoIndexAccessor; } }
		#region Fields
		public const int MaxHeightInTwips = 8192; 
		int formatIndex;
		int heightIndex;
		int infoIndex;
		readonly Worksheet sheet;
		int index;  
		ICellCollection cells;
		const uint cachedRowHeightValidMask = 0x80000000;
		uint cachedRowHeightPackedValue; 
		#endregion
		public Row(int index, Worksheet sheet) {
			Guard.ArgumentNotNull(sheet, "sheet");
			this.index = index;
			this.sheet = sheet;
			this.cells = null;
			this.formatIndex = sheet.Workbook.StyleSheet.DefaultCellFormatIndex;
		}
		#region Properties
		public virtual int CellsCount { get { return (this.Cells != null) ? this.Cells.Count : 0; } }
		public int FirstColumnIndex { get { return (CellsCount > 0) ? Cells.First.ColumnIndex : -1; } }
		public int LastColumnIndex { get { return (CellsCount > 0) ? Cells.Last.ColumnIndex : -1; } }
		public virtual int FormatIndex { get { return formatIndex; } }
		public virtual int HeightIndex { get { return heightIndex; } }
		public virtual int InfoIndex { get { return infoIndex; } }
		public virtual ICell this[int index] { get { return Cells[index]; } }
		public virtual Worksheet Sheet { get { return sheet; } }
		public virtual int Index { get { return index; } }
		internal new RowBatchUpdateHelper BatchUpdateHelper { get { return (RowBatchUpdateHelper)base.BatchUpdateHelper; } }
		public new DocumentModel DocumentModel { get { return Sheet.Workbook; } }
		internal virtual CellFormat FormatInfo { get { return BatchUpdateHelper != null ? (CellFormat)BatchUpdateHelper.CellFormat : FormatInfoCore; } }
		CellFormat FormatInfoCore { get { return (CellFormat)DocumentModel.Cache.CellFormatCache[FormatIndex]; } }
		internal virtual RowHeightInfo HeightInfo { get { return BatchUpdateHelper != null ? BatchUpdateHelper.HeightInfo : HeightInfoCore; } }
		RowHeightInfo HeightInfoCore { get { return heightIndexAccessor.GetInfo(this); } }
		internal virtual RowInfo Info { get { return BatchUpdateHelper != null ? BatchUpdateHelper.Info : InfoCore; } }
		RowInfo InfoCore { get { return infoIndexAccessor.GetInfo(this); } }
		#region Height
		public float Height {
			get { return HeightInfo.FloatValue; }
			set {
				if (HeightInfo.FloatValue == value)
					return;
				if (value < 0 || value > DocumentModel.UnitConverter.TwipsToModelUnits(MaxHeightInTwips))
					throw new ArgumentException(XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Msg_ErrorIncorrectRowHeight));
				SetPropertyValueForStruct(heightIndexAccessor, SetHeightCore, value);
			}
		}
		DocumentModelChangeActions SetHeightCore(ref RowHeightInfo info, float value) {
			info.FloatValue = value;
			sheet.WebRanges.ChangeRange(GetCellIntervalRange());
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region OutlineLevel
		public int OutlineLevel {
			get { return Info.OutlineLevel; }
			set {
				if (Info.OutlineLevel == value)
					return;
				SetPropertyValueForStruct(infoIndexAccessor, SetOutlineLevelCore, value);
			}
		}
		DocumentModelChangeActions SetOutlineLevelCore(ref RowInfo info, int value) {
			info.OutlineLevel = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region IsHidden
		public bool IsHidden {
			get { return Info.IsHidden; }
			set {
				if (Info.IsHidden == value)
					return;
				BeginUpdate();
				SetPropertyValueForStruct(infoIndexAccessor, SetIsHiddenCore, value);
				if (Sheet.DrawingObjects.Count > 0 || Sheet.VmlDrawing.Shapes.Count > 0) {
					InvalidateAnchorDatasByRowHistoryItem historyItem = new InvalidateAnchorDatasByRowHistoryItem(Sheet, Index);
					DocumentModel.History.Add(historyItem);
					historyItem.Execute();
				}
				EndUpdate();
			}
		}
		DocumentModelChangeActions SetIsHiddenCore(ref RowInfo info, bool value) {
			info.IsHidden = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region IsCollapsed
		public bool IsCollapsed {
			get { return Info.IsCollapsed; }
			set {
				if (Info.IsCollapsed == value)
					return;
				SetPropertyValueForStruct(infoIndexAccessor, SetIsCollapsedCore, value);
			}
		}
		DocumentModelChangeActions SetIsCollapsedCore(ref RowInfo info, bool value) {
			info.IsCollapsed = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region IsCustomHeight
		public bool IsCustomHeight {
			get { return Info.IsCustomHeight; }
			set {
				if (Info.IsCustomHeight == value)
					return;
				SetPropertyValueForStruct(infoIndexAccessor, SetIsCustomHeightCore, value);
			}
		}
		DocumentModelChangeActions SetIsCustomHeightCore(ref RowInfo info, bool value) {
			info.IsCustomHeight = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region ApplyStyle
		public bool ApplyStyle {
			get { return formatIndex != sheet.Workbook.StyleSheet.DefaultCellFormatIndex; ; }
		}
		#endregion
		#region IsThickTopBorder
		public bool IsThickTopBorder {
			get { return Info.IsThickTopBorder; }
			set {
				if (Info.IsThickTopBorder == value)
					return;
				SetPropertyValueForStruct(infoIndexAccessor, SetIsThickTopBorderCore, value);
			}
		}
		DocumentModelChangeActions SetIsThickTopBorderCore(ref RowInfo info, bool value) {
			info.IsThickTopBorder = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region IsThickBottomBorder
		public bool IsThickBottomBorder {
			get { return Info.IsThickBottomBorder; }
			set {
				if (Info.IsThickBottomBorder == value)
					return;
				SetPropertyValueForStruct(infoIndexAccessor, SetIsThickBottomBorderCore, value);
			}
		}
		DocumentModelChangeActions SetIsThickBottomBorderCore(ref RowInfo info, bool value) {
			info.IsThickBottomBorder = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		public bool IsVisible { get { return !(IsHidden || Height == 0 && IsCustomHeight); } }
		public bool HasVisibleFill {
			get {
				if (!IsVisible)
					return false;
				if (ActualFill.FillType == ModelFillType.Gradient)
					return true;
				return (ActualFill.PatternType == XlPatternType.Solid && !DXColor.IsTransparentOrEmpty(Cell.GetBackgroundColor(ActualFill))) || (ActualFill.PatternType != XlPatternType.None && ActualFill.PatternType != XlPatternType.Solid);
			}
		}
		public bool HasVisibleBorder {
			get {
				if (!IsVisible)
					return false;
				return !Cell.IsBorderEmpty(ActualBorder);
			}
		}
		public int CachedRowHeight {
			get {
				return (int)(cachedRowHeightPackedValue & (~(Cell.transactionVersionMask | cachedRowHeightValidMask)));
			}
			set {
				cachedRowHeightPackedValue &= Cell.transactionVersionMask;
				cachedRowHeightPackedValue |= (uint)(value & ~Cell.transactionVersionMask);
			}
		}
		int TransactionVersion {
			get {
				return (int)((cachedRowHeightPackedValue & Cell.transactionVersionMask) >> Cell.transactionMaskOffset);
			}
			set {
				int delta = value - ((int)Cell.transactionVersionMask >> Cell.transactionMaskOffset);
				if (delta > 0)
					value = delta;
				cachedRowHeightPackedValue &= ~Cell.transactionVersionMask;
				cachedRowHeightPackedValue |= ((uint)value << Cell.transactionMaskOffset) & Cell.transactionVersionMask;
			}
		}
		public void SetCachedRowHeight(int value) {
			this.CachedRowHeight = value;
			this.TransactionVersion = sheet.Workbook.TransactionVersion;
			this.cachedRowHeightPackedValue |= cachedRowHeightValidMask;
		}
		public bool HasValidCachedRowHeight { get { return (cachedRowHeightPackedValue & cachedRowHeightValidMask) != 0 && TransactionVersion == sheet.Workbook.TransactionVersion; } }
		public void ResetCachedTransactionVersions() {
			cachedRowHeightPackedValue = 0;
		}
		#endregion
		#region MultiIndexObject
		protected override IDocumentModel GetDocumentModel() {
			return sheet.Workbook;
		}
		public override Row GetOwner() {
			return this;
		}
		public override DocumentModelChangeActions GetBatchUpdateChangeActions() {
			return DocumentModelChangeActions.None;
		}
		protected override void ApplyChanges(DocumentModelChangeActions actions) {
		}
		protected override MultiIndexBatchUpdateHelper CreateBatchUpdateHelper() {
			return new RowBatchUpdateHelper(this);
		}
		protected override MultiIndexBatchUpdateHelper CreateBatchInitHelper() {
			return new RowBatchInitHelper(this);
		}
		protected override IIndexAccessorBase<Row, DocumentModelChangeActions>[] IndexAccessors { get { return indexAccessors; } }
		internal void AssignCellFormatIndex(int value) {
			this.formatIndex = value;
		}
		internal void AssignHeightIndex(int value) {
			this.heightIndex = value;
		}
		internal void AssignInfoIndex(int value) {
			this.infoIndex = value;
		}
		#endregion
		#region ICellContainer Members
		public virtual ICellCollection Cells {
			get {
				if (this.cells == null)
					this.cells = new CellCollection(this);
				return this.cells;
			}
		}
		public CellPosition GetCellPosition(int index) {
			return new CellPosition(index, Index);
		}
		#endregion
		protected internal void RemoveStyle() {
			bool applyNumberFormat = ApplyNumberFormat;
			bool applyFont = ApplyFont;
			bool applyFill = ApplyFill;
			bool applyBorder = ApplyBorder;
			bool applyAlignment = ApplyAlignment;
			bool applyProtection = ApplyProtection;
			Style = DocumentModel.StyleSheet.CellStyles[0];
			ApplyNumberFormat = applyNumberFormat;
			ApplyFont = applyFont;
			ApplyFill = applyFill;
			ApplyBorder = applyBorder;
			ApplyAlignment = applyAlignment;
			ApplyProtection = applyProtection;
		}
		public override bool Equals(object obj) {
			Row other = obj as Row;
			if (other == null)
				return false;
			return IsEqual(other);
		}
		bool IsEqual(Row other) {
			return
				this.Index == other.Index &&
				this.Sheet == other.Sheet;
		}
		public bool HasSameFormatting(Row row) {
			return base.Equals(row); 
		}
		public override int GetHashCode() {
			return base.GetHashCode() ^ Index ^ Sheet.GetHashCode();
		}
		#region IRowBase Members
		IWorksheet IRowBase.Sheet { get { return Sheet; } }
		ICellCollectionBase IRowBase.Cells { get { return Cells; } }
		#endregion
		public void OffsetRowIndex(int offset) {
			OffsetRowIndexCore(offset);
			Cells.OffsetRowIndex(offset);
		}
		public void OffsetRowIndexCore(int offset) {
			index += offset;
		}
		#region ICellFormat Members
		public IRunFontInfo Font { get { return this; } }
		public IActualRunFontInfo ActualFont { get { return this; } }
		public IFillInfo Fill { get { return this; } }
		public IActualFillInfo ActualFill { get { return this; } }
		public ICellAlignmentInfo Alignment { get { return this; } }
		public IActualCellAlignmentInfo ActualAlignment { get { return this; } }
		public IBorderInfo Border { get { return this; } }
		public IActualBorderInfo ActualBorder { get { return this; } }
		public ICellProtectionInfo Protection { get { return this; } }
		public IActualCellProtectionInfo ActualProtection { get { return this; } }
		#region IRunFontInfo Members
		#region IRunFontInfo.Name
		string IRunFontInfo.Name {
			get { return FormatInfo.Font.Name; }
			set {
				if (FormatInfo.Font.Name == value)
					return;
				SetPropertyValue(cellFormatIndexAccessor, SetFontName, value);
			}
		}
		DocumentModelChangeActions SetFontName(FormatBase info, string value) {
			info.Font.Name = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region IRunFontInfo.Color
		Color IRunFontInfo.Color {
			get { return FormatInfo.Font.Color; }
			set {
				if (FormatInfo.Font.Color == value)
					return;
				SetPropertyValue(cellFormatIndexAccessor, SetFontColor, value);
			}
		}
		DocumentModelChangeActions SetFontColor(FormatBase info, Color value) {
			info.Font.Color = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region IRunFontInfo.Bold
		bool IRunFontInfo.Bold {
			get { return FormatInfo.Font.Bold; }
			set {
				if (FormatInfo.Font.Bold == value)
					return;
				SetPropertyValue(cellFormatIndexAccessor, SetFontBold, value);
			}
		}
		DocumentModelChangeActions SetFontBold(FormatBase info, bool value) {
			info.Font.Bold = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region IRunFontInfo.Condense
		bool IRunFontInfo.Condense {
			get { return FormatInfo.Font.Condense; }
			set {
				if (FormatInfo.Font.Condense == value)
					return;
				SetPropertyValue(cellFormatIndexAccessor, SetFontCondense, value);
			}
		}
		DocumentModelChangeActions SetFontCondense(FormatBase info, bool value) {
			info.Font.Condense = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region IRunFontInfo.Extend
		bool IRunFontInfo.Extend {
			get { return FormatInfo.Font.Extend; }
			set {
				if (FormatInfo.Font.Extend == value)
					return;
				SetPropertyValue(cellFormatIndexAccessor, SetFontExtend, value);
			}
		}
		DocumentModelChangeActions SetFontExtend(FormatBase info, bool value) {
			info.Font.Extend = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region IRunFontInfo.Italic
		bool IRunFontInfo.Italic {
			get { return FormatInfo.Font.Italic; }
			set {
				if (FormatInfo.Font.Italic == value)
					return;
				SetPropertyValue(cellFormatIndexAccessor, SetFontItalic, value);
			}
		}
		DocumentModelChangeActions SetFontItalic(FormatBase info, bool value) {
			info.Font.Italic = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region IRunFontInfo.Outline
		bool IRunFontInfo.Outline {
			get { return FormatInfo.Font.Outline; }
			set {
				if (FormatInfo.Font.Outline == value)
					return;
				SetPropertyValue(cellFormatIndexAccessor, SetFontOutline, value);
			}
		}
		DocumentModelChangeActions SetFontOutline(FormatBase info, bool value) {
			info.Font.Outline = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region IRunFontInfo.Shadow
		bool IRunFontInfo.Shadow {
			get { return FormatInfo.Font.Shadow; }
			set {
				if (FormatInfo.Font.Shadow == value)
					return;
				SetPropertyValue(cellFormatIndexAccessor, SetFontShadow, value);
			}
		}
		DocumentModelChangeActions SetFontShadow(FormatBase info, bool value) {
			info.Font.Shadow = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region IRunFontInfo.StrikeThrough
		bool IRunFontInfo.StrikeThrough {
			get { return FormatInfo.Font.StrikeThrough; }
			set {
				if (FormatInfo.Font.StrikeThrough == value)
					return;
				SetPropertyValue(cellFormatIndexAccessor, SetFontStrikeThrough, value);
			}
		}
		DocumentModelChangeActions SetFontStrikeThrough(FormatBase info, bool value) {
			info.Font.StrikeThrough = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region IRunFontInfo.Charset
		int IRunFontInfo.Charset {
			get { return FormatInfo.Font.Charset; }
			set {
				if (FormatInfo.Font.Charset == value)
					return;
				SetPropertyValue(cellFormatIndexAccessor, SetFontCharset, value);
			}
		}
		DocumentModelChangeActions SetFontCharset(FormatBase info, int value) {
			info.Font.Charset = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region IRunFontInfo.FontFamily
		int IRunFontInfo.FontFamily {
			get { return FormatInfo.Font.FontFamily; }
			set {
				if (FormatInfo.Font.FontFamily == value)
					return;
				SetPropertyValue(cellFormatIndexAccessor, SetFontFamily, value);
			}
		}
		DocumentModelChangeActions SetFontFamily(FormatBase info, int value) {
			info.Font.FontFamily = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region IRunFontInfo.Size
		double IRunFontInfo.Size {
			get { return FormatInfo.Font.Size; }
			set {
				if (FormatInfo.Font.Size == value)
					return;
				SetPropertyValue(cellFormatIndexAccessor, SetFontSize, value);
			}
		}
		DocumentModelChangeActions SetFontSize(FormatBase info, double value) {
			info.Font.Size = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region IRunFontInfo.SchemeStyle
		XlFontSchemeStyles IRunFontInfo.SchemeStyle {
			get { return FormatInfo.Font.SchemeStyle; }
			set {
				if (FormatInfo.Font.SchemeStyle == value)
					return;
				SetPropertyValue(cellFormatIndexAccessor, SetFontSchemeStyle, value);
			}
		}
		DocumentModelChangeActions SetFontSchemeStyle(FormatBase info, XlFontSchemeStyles value) {
			info.Font.SchemeStyle = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region IRunFontInfo.Script
		XlScriptType IRunFontInfo.Script {
			get { return FormatInfo.Font.Script; }
			set {
				if (FormatInfo.Font.Script == value)
					return;
				SetPropertyValue(cellFormatIndexAccessor, SetFontScript, value);
			}
		}
		DocumentModelChangeActions SetFontScript(FormatBase info, XlScriptType value) {
			info.Font.Script = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region IRunFontInfo.Underline
		XlUnderlineType IRunFontInfo.Underline {
			get { return FormatInfo.Font.Underline; }
			set {
				if (FormatInfo.Font.Underline == value)
					return;
				SetPropertyValue(cellFormatIndexAccessor, SetFontUnderline, value);
			}
		}
		DocumentModelChangeActions SetFontUnderline(FormatBase info, XlUnderlineType value) {
			info.Font.Underline = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#endregion
		#region IActualRunFontInfo Members
		string IActualRunFontInfo.Name { get { return FormatInfo.ActualFont.Name; } }
		Color IActualRunFontInfo.Color { get { return FormatInfo.ActualFont.Color; } }
		bool IActualRunFontInfo.Bold { get { return FormatInfo.ActualFont.Bold; } }
		bool IActualRunFontInfo.Condense { get { return FormatInfo.ActualFont.Condense; } }
		bool IActualRunFontInfo.Extend { get { return FormatInfo.ActualFont.Extend; } }
		bool IActualRunFontInfo.Italic { get { return FormatInfo.ActualFont.Italic; } }
		bool IActualRunFontInfo.Outline { get { return FormatInfo.ActualFont.Outline; } }
		bool IActualRunFontInfo.Shadow { get { return FormatInfo.ActualFont.Shadow; } }
		bool IActualRunFontInfo.StrikeThrough { get { return FormatInfo.ActualFont.StrikeThrough; } }
		int IActualRunFontInfo.Charset { get { return FormatInfo.ActualFont.Charset; } }
		int IActualRunFontInfo.FontFamily { get { return FormatInfo.ActualFont.FontFamily; } }
		double IActualRunFontInfo.Size { get { return FormatInfo.ActualFont.Size; } }
		XlFontSchemeStyles IActualRunFontInfo.SchemeStyle { get { return FormatInfo.ActualFont.SchemeStyle; } }
		XlScriptType IActualRunFontInfo.Script { get { return FormatInfo.ActualFont.Script; } }
		XlUnderlineType IActualRunFontInfo.Underline { get { return FormatInfo.ActualFont.Underline; } }
		int IActualRunFontInfo.ColorIndex { get { return FormatInfo.ActualFont.ColorIndex; } }
		FontInfo IActualRunFontInfo.GetFontInfo() {
			return FormatInfo.ActualFont.GetFontInfo();
		}
		#endregion
		#region IFillInfo Members
		#region IFillInfo.Clear
		void IFillInfo.Clear() {
			if (FormatInfo.ApplyFill)
				ClearFillCore();
		}
		void ClearFillCore() {
			DocumentModel.BeginUpdate();
			try {
				FormatBase info = GetInfoForModification(cellFormatIndexAccessor);
				info.Fill.Clear();
				ReplaceInfo(cellFormatIndexAccessor, info, DocumentModelChangeActions.None);
			}
			finally {
				DocumentModel.EndUpdate();
			}
		}
		#endregion
		#region IFillInfo.PatternType
		XlPatternType IFillInfo.PatternType {
			get { return FormatInfo.Fill.PatternType; }
			set {
				if (FormatInfo.Fill.PatternType == value)
					return;
				SetPropertyValue(cellFormatIndexAccessor, SetFillPatternType, value);
			}
		}
		DocumentModelChangeActions SetFillPatternType(FormatBase info, XlPatternType value) {
			info.Fill.PatternType = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region IFillInfo.ForeColor
		Color IFillInfo.ForeColor {
			get { return FormatInfo.Fill.ForeColor; }
			set {
				if (FormatInfo.Fill.ForeColor == value)
					return;
				SetPropertyValue(cellFormatIndexAccessor, SetFillForeColor, value);
			}
		}
		DocumentModelChangeActions SetFillForeColor(FormatBase info, Color value) {
			info.Fill.ForeColor = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region IFillInfo.BackColor
		Color IFillInfo.BackColor {
			get { return FormatInfo.Fill.BackColor; }
			set {
				if (FormatInfo.Fill.BackColor == value)
					return;
				SetPropertyValue(cellFormatIndexAccessor, SetFillBackColor, value);
			}
		}
		DocumentModelChangeActions SetFillBackColor(FormatBase info, Color value) {
			info.Fill.BackColor = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region IFillInfo.GradientFill
		IGradientFillInfo IFillInfo.GradientFill { get { return this; } }
		#endregion
		#region IGradientFillInfo Members
		#region IGradientFillInfo.Convergence
		IConvergenceInfo IGradientFillInfo.Convergence { get { return this; } }
		#endregion
		#region IGradientFillInfo.GradientStops
		IGradientStopCollection IGradientFillInfo.GradientStops { get { return FormatInfo.Fill.GradientFill.GradientStops; } }
		#endregion
		#region IGradientFillInfo.Type
		ModelGradientFillType IGradientFillInfo.Type {
			get { return FormatInfo.Fill.GradientFill.Type; }
			set {
				if (FormatInfo.Fill.GradientFill.Type == value)
					return;
				SetPropertyValue(cellFormatIndexAccessor, SetGradientFillInfoType, value);
			}
		}
		DocumentModelChangeActions SetGradientFillInfoType(FormatBase info, ModelGradientFillType value) {
			info.Fill.GradientFill.Type = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region IGradientFillInfo.Degree
		double IGradientFillInfo.Degree {
			get { return FormatInfo.Fill.GradientFill.Degree; }
			set {
				if (FormatInfo.Fill.GradientFill.Degree == value)
					return;
				SetPropertyValue(cellFormatIndexAccessor, SetGradientFillInfoDegree, value);
			}
		}
		protected DocumentModelChangeActions SetGradientFillInfoDegree(FormatBase info, double value) {
			info.Fill.GradientFill.Degree = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region IConvergenceInfo Members
		#region IConvergenceInfo.Left
		float IConvergenceInfo.Left {
			get { return FormatInfo.Fill.GradientFill.Convergence.Left; }
			set {
				if (FormatInfo.Fill.GradientFill.Convergence.Left == value)
					return;
				SetPropertyValue(cellFormatIndexAccessor, SetGradientFillInfoLeft, value);
			}
		}
		DocumentModelChangeActions SetGradientFillInfoLeft(FormatBase info, float value) {
			info.Fill.GradientFill.Convergence.Left = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region IConvergenceInfo.Right
		float IConvergenceInfo.Right {
			get { return FormatInfo.Fill.GradientFill.Convergence.Right; }
			set {
				if (FormatInfo.Fill.GradientFill.Convergence.Right == value)
					return;
				SetPropertyValue(cellFormatIndexAccessor, SetGradientFillInfoRight, value);
			}
		}
		DocumentModelChangeActions SetGradientFillInfoRight(FormatBase info, float value) {
			info.Fill.GradientFill.Convergence.Right = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region IConvergenceInfo.Top
		float IConvergenceInfo.Top {
			get { return FormatInfo.Fill.GradientFill.Convergence.Top; }
			set {
				if (FormatInfo.Fill.GradientFill.Convergence.Top == value)
					return;
				SetPropertyValue(cellFormatIndexAccessor, SetGradientFillInfoTop, value);
			}
		}
		DocumentModelChangeActions SetGradientFillInfoTop(FormatBase info, float value) {
			info.Fill.GradientFill.Convergence.Top = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region IConvergenceInfo.Bottom
		float IConvergenceInfo.Bottom {
			get { return FormatInfo.Fill.GradientFill.Convergence.Bottom; }
			set {
				if (FormatInfo.Fill.GradientFill.Convergence.Bottom == value)
					return;
				SetPropertyValue(cellFormatIndexAccessor, SetGradientFillInfoBottom, value);
			}
		}
		DocumentModelChangeActions SetGradientFillInfoBottom(FormatBase info, float value) {
			info.Fill.GradientFill.Convergence.Bottom = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#endregion
		#endregion
		#region IFillInfo.FillType
		ModelFillType IFillInfo.FillType {
			get { return FormatInfo.Fill.FillType; }
			set {
				if (FormatInfo.Fill.FillType == value)
					return;
				SetPropertyValue(cellFormatIndexAccessor, SetModelFillType, value);
			}
		}
		DocumentModelChangeActions SetModelFillType(FormatBase info, ModelFillType value) {
			info.Fill.FillType = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#endregion
		#region IActualFillInfo Members
		XlPatternType IActualFillInfo.PatternType { get { return FormatInfo.ActualFill.PatternType; } }
		Color IActualFillInfo.ForeColor { get { return FormatInfo.ActualFill.ForeColor; } }
		Color IActualFillInfo.BackColor { get { return FormatInfo.ActualFill.BackColor; } }
		int IActualFillInfo.ForeColorIndex { get { return FormatInfo.ActualFill.ForeColorIndex; } }
		int IActualFillInfo.BackColorIndex { get { return FormatInfo.ActualFill.BackColorIndex; } }
		bool IActualFillInfo.ApplyPatternType { get { return true; } }
		bool IActualFillInfo.ApplyForeColor { get { return true; } }
		bool IActualFillInfo.ApplyBackColor { get { return true; } }
		bool IActualFillInfo.IsDifferential { get { return false; } }
		IActualGradientFillInfo IActualFillInfo.GradientFill { get { return this; } }
		IActualConvergenceInfo IActualGradientFillInfo.Convergence { get { return this; } }
		ModelFillType IActualFillInfo.FillType { get { return FormatInfo.ActualFill.FillType; } }
		ModelGradientFillType IActualGradientFillInfo.Type { get { return FormatInfo.ActualFill.GradientFill.Type; } }
		double IActualGradientFillInfo.Degree { get { return FormatInfo.ActualFill.GradientFill.Degree; } }
		IActualGradientStopCollection IActualGradientFillInfo.GradientStops { get { return FormatInfo.ActualFill.GradientFill.GradientStops; } }
		float IActualConvergenceInfo.Left { get { return FormatInfo.ActualFill.GradientFill.Convergence.Left; } }
		float IActualConvergenceInfo.Right { get { return FormatInfo.ActualFill.GradientFill.Convergence.Right; } }
		float IActualConvergenceInfo.Top { get { return FormatInfo.ActualFill.GradientFill.Convergence.Top; } }
		float IActualConvergenceInfo.Bottom { get { return FormatInfo.ActualFill.GradientFill.Convergence.Bottom; } }
		#endregion
		#region ICellAlignmentInfo Members
		#region ICellAlignmentInfo.WrapText
		bool ICellAlignmentInfo.WrapText {
			get { return FormatInfo.Alignment.WrapText; }
			set {
				if (FormatInfo.Alignment.WrapText == value)
					return;
				SetPropertyValue(cellFormatIndexAccessor, SetAlignmentWrapText, value);
			}
		}
		DocumentModelChangeActions SetAlignmentWrapText(FormatBase info, bool value) {
			info.Alignment.WrapText = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region ICellAlignmentInfo.JustifyLastLine
		bool ICellAlignmentInfo.JustifyLastLine {
			get { return FormatInfo.Alignment.JustifyLastLine; }
			set {
				if (FormatInfo.Alignment.JustifyLastLine == value)
					return;
				SetPropertyValue(cellFormatIndexAccessor, SetAlignmentJustifyLastLine, value);
			}
		}
		DocumentModelChangeActions SetAlignmentJustifyLastLine(FormatBase info, bool value) {
			info.Alignment.JustifyLastLine = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region ICellAlignmentInfo.ShrinkToFit
		bool ICellAlignmentInfo.ShrinkToFit {
			get { return FormatInfo.Alignment.ShrinkToFit; }
			set {
				if (FormatInfo.Alignment.ShrinkToFit == value)
					return;
				SetPropertyValue(cellFormatIndexAccessor, SetAlignmentShrinkToFit, value);
			}
		}
		DocumentModelChangeActions SetAlignmentShrinkToFit(FormatBase info, bool value) {
			info.Alignment.ShrinkToFit = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region ICellAlignmentInfo.TextRotation
		int ICellAlignmentInfo.TextRotation {
			get { return FormatInfo.Alignment.TextRotation; }
			set {
				if (FormatInfo.Alignment.TextRotation == value)
					return;
				SetPropertyValue(cellFormatIndexAccessor, SetAlignmentTextRotation, value);
			}
		}
		DocumentModelChangeActions SetAlignmentTextRotation(FormatBase info, int value) {
			info.Alignment.TextRotation = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region ICellAlignmentInfo.Indent
		byte ICellAlignmentInfo.Indent {
			get { return FormatInfo.Alignment.Indent; }
			set {
				if (FormatInfo.Alignment.Indent == value)
					return;
				SetPropertyValue(cellFormatIndexAccessor, SetAlignmentIndent, value);
			}
		}
		DocumentModelChangeActions SetAlignmentIndent(FormatBase info, byte value) {
			info.Alignment.Indent = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region ICellAlignmentInfo.RelativeIndent
		int ICellAlignmentInfo.RelativeIndent {
			get { return FormatInfo.Alignment.RelativeIndent; }
			set {
				if (FormatInfo.Alignment.RelativeIndent == value)
					return;
				SetPropertyValue(cellFormatIndexAccessor, SetAlignmentRelativeIndent, value);
			}
		}
		DocumentModelChangeActions SetAlignmentRelativeIndent(FormatBase info, int value) {
			info.Alignment.RelativeIndent = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region ICellAlignmentInfo.Horizontal
		XlHorizontalAlignment ICellAlignmentInfo.Horizontal {
			get { return FormatInfo.Alignment.Horizontal; }
			set {
				if (FormatInfo.Alignment.Horizontal == value)
					return;
				SetPropertyValue(cellFormatIndexAccessor, SetAlignmentHorizontal, value);
			}
		}
		DocumentModelChangeActions SetAlignmentHorizontal(FormatBase info, XlHorizontalAlignment value) {
			info.Alignment.Horizontal = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region ICellAlignmentInfo.Vertical
		XlVerticalAlignment ICellAlignmentInfo.Vertical {
			get { return FormatInfo.Alignment.Vertical; }
			set {
				if (FormatInfo.Alignment.Vertical == value)
					return;
				SetPropertyValue(cellFormatIndexAccessor, SetAlignmentVertical, value);
			}
		}
		DocumentModelChangeActions SetAlignmentVertical(FormatBase info, XlVerticalAlignment value) {
			info.Alignment.Vertical = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region ICellAlignmentInfo.XlReadingOrder
		XlReadingOrder ICellAlignmentInfo.ReadingOrder {
			get { return FormatInfo.Alignment.ReadingOrder; }
			set {
				if (FormatInfo.Alignment.ReadingOrder == value)
					return;
				SetPropertyValue(cellFormatIndexAccessor, SetAlignmentReadingOrder, value);
			}
		}
		DocumentModelChangeActions SetAlignmentReadingOrder(FormatBase info, XlReadingOrder value) {
			info.Alignment.ReadingOrder = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#endregion
		#region IActualCellAlignmentInfo Members
		bool IActualCellAlignmentInfo.WrapText { get { return FormatInfo.ActualAlignment.WrapText; } }
		bool IActualCellAlignmentInfo.JustifyLastLine { get { return FormatInfo.ActualAlignment.JustifyLastLine; } }
		bool IActualCellAlignmentInfo.ShrinkToFit { get { return FormatInfo.ActualAlignment.ShrinkToFit; } }
		int IActualCellAlignmentInfo.TextRotation { get { return FormatInfo.ActualAlignment.TextRotation; } }
		byte IActualCellAlignmentInfo.Indent { get { return FormatInfo.ActualAlignment.Indent; } }
		int IActualCellAlignmentInfo.RelativeIndent { get { return FormatInfo.ActualAlignment.RelativeIndent; } }
		XlHorizontalAlignment IActualCellAlignmentInfo.Horizontal { get { return FormatInfo.ActualAlignment.Horizontal; } }
		XlVerticalAlignment IActualCellAlignmentInfo.Vertical { get { return FormatInfo.ActualAlignment.Vertical; } }
		XlReadingOrder IActualCellAlignmentInfo.ReadingOrder { get { return FormatInfo.ActualAlignment.ReadingOrder; } }
		#endregion
		#region IBorderInfo Members
		#region IBorderInfo.LeftLineStyle
		XlBorderLineStyle IBorderInfo.LeftLineStyle {
			get { return FormatInfo.Border.LeftLineStyle; }
			set {
				if (FormatInfo.Border.LeftLineStyle == value)
					return;
				SetPropertyValue(cellFormatIndexAccessor, SetBorderLeftLineStyle, value);
			}
		}
		DocumentModelChangeActions SetBorderLeftLineStyle(FormatBase info, XlBorderLineStyle value) {
			info.Border.LeftLineStyle = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region IBorderInfo.LeftColor
		Color IBorderInfo.LeftColor {
			get { return FormatInfo.Border.LeftColor; }
			set {
				if (FormatInfo.Border.LeftColor == value)
					return;
				SetPropertyValue(cellFormatIndexAccessor, SetBorderLeftColor, value);
			}
		}
		DocumentModelChangeActions SetBorderLeftColor(FormatBase info, Color value) {
			info.Border.LeftColor = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region IBorderInfo.LeftColorIndex
		int IBorderInfo.LeftColorIndex {
			get { return FormatInfo.Border.LeftColorIndex; }
			set {
				if (FormatInfo.Border.LeftColorIndex == value)
					return;
				SetPropertyValue(cellFormatIndexAccessor, SetBorderLeftColorIndex, value);
			}
		}
		DocumentModelChangeActions SetBorderLeftColorIndex(FormatBase info, int value) {
			info.Border.LeftColorIndex = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region IBorderInfo.RightLineStyle
		XlBorderLineStyle IBorderInfo.RightLineStyle {
			get { return FormatInfo.Border.RightLineStyle; }
			set {
				if (FormatInfo.Border.RightLineStyle == value)
					return;
				SetPropertyValue(cellFormatIndexAccessor, SetBorderRightLineStyle, value);
			}
		}
		DocumentModelChangeActions SetBorderRightLineStyle(FormatBase info, XlBorderLineStyle value) {
			info.Border.RightLineStyle = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region IBorderInfo.RightColor
		Color IBorderInfo.RightColor {
			get { return FormatInfo.Border.RightColor; }
			set {
				if (FormatInfo.Border.RightColor == value)
					return;
				SetPropertyValue(cellFormatIndexAccessor, SetBorderRightColor, value);
			}
		}
		DocumentModelChangeActions SetBorderRightColor(FormatBase info, Color value) {
			info.Border.RightColor = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region IBorderInfo.RightColorIndex
		int IBorderInfo.RightColorIndex {
			get { return FormatInfo.Border.RightColorIndex; }
			set {
				if (FormatInfo.Border.RightColorIndex == value)
					return;
				SetPropertyValue(cellFormatIndexAccessor, SetBorderRightColorIndex, value);
			}
		}
		DocumentModelChangeActions SetBorderRightColorIndex(FormatBase info, int value) {
			info.Border.RightColorIndex = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region IBorderInfo.TopLineStyle
		XlBorderLineStyle IBorderInfo.TopLineStyle {
			get { return FormatInfo.Border.TopLineStyle; }
			set {
				if (FormatInfo.Border.TopLineStyle == value)
					return;
				SetPropertyValue(cellFormatIndexAccessor, SetBorderTopLineStyle, value);
			}
		}
		DocumentModelChangeActions SetBorderTopLineStyle(FormatBase info, XlBorderLineStyle value) {
			info.Border.TopLineStyle = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region IBorderInfo.TopColor
		Color IBorderInfo.TopColor {
			get { return FormatInfo.Border.TopColor; }
			set {
				if (FormatInfo.Border.TopColor == value)
					return;
				SetPropertyValue(cellFormatIndexAccessor, SetBorderTopColor, value);
			}
		}
		DocumentModelChangeActions SetBorderTopColor(FormatBase info, Color value) {
			info.Border.TopColor = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region IBorderInfo.TopColorIndex
		int IBorderInfo.TopColorIndex {
			get { return FormatInfo.Border.TopColorIndex; }
			set {
				if (FormatInfo.Border.TopColorIndex == value)
					return;
				SetPropertyValue(cellFormatIndexAccessor, SetBorderTopColorIndex, value);
			}
		}
		DocumentModelChangeActions SetBorderTopColorIndex(FormatBase info, int value) {
			info.Border.TopColorIndex = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region IBorderInfo.BottomLineStyle
		XlBorderLineStyle IBorderInfo.BottomLineStyle {
			get { return FormatInfo.Border.BottomLineStyle; }
			set {
				if (FormatInfo.Border.BottomLineStyle == value)
					return;
				SetPropertyValue(cellFormatIndexAccessor, SetBorderBottomLineStyle, value);
			}
		}
		DocumentModelChangeActions SetBorderBottomLineStyle(FormatBase info, XlBorderLineStyle value) {
			info.Border.BottomLineStyle = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region IBorderInfo.BottomColor
		Color IBorderInfo.BottomColor {
			get { return FormatInfo.Border.BottomColor; }
			set {
				if (FormatInfo.Border.BottomColor == value)
					return;
				SetPropertyValue(cellFormatIndexAccessor, SetBorderBottomColor, value);
			}
		}
		DocumentModelChangeActions SetBorderBottomColor(FormatBase info, Color value) {
			info.Border.BottomColor = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region IBorderInfo.BottomColorIndex
		int IBorderInfo.BottomColorIndex {
			get { return FormatInfo.Border.BottomColorIndex; }
			set {
				if (FormatInfo.Border.BottomColorIndex == value)
					return;
				SetPropertyValue(cellFormatIndexAccessor, SetBorderBottomColorIndex, value);
			}
		}
		DocumentModelChangeActions SetBorderBottomColorIndex(FormatBase info, int value) {
			info.Border.BottomColorIndex = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region IBorderInfo.HorizontalLineStyle
		XlBorderLineStyle IBorderInfo.HorizontalLineStyle {
			get { return FormatInfo.Border.HorizontalLineStyle; }
			set {
				if (FormatInfo.Border.HorizontalLineStyle == value)
					return;
				SetPropertyValue(cellFormatIndexAccessor, SetBorderHorizontalLineStyle, value);
			}
		}
		DocumentModelChangeActions SetBorderHorizontalLineStyle(FormatBase info, XlBorderLineStyle value) {
			info.Border.HorizontalLineStyle = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region IBorderInfo.HorizontalColor
		Color IBorderInfo.HorizontalColor {
			get { return FormatInfo.Border.HorizontalColor; }
			set {
				if (FormatInfo.Border.HorizontalColor == value)
					return;
				SetPropertyValue(cellFormatIndexAccessor, SetBorderHorizontalColor, value);
			}
		}
		DocumentModelChangeActions SetBorderHorizontalColor(FormatBase info, Color value) {
			info.Border.HorizontalColor = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region IBorderInfo.HorizontalColorIndex
		int IBorderInfo.HorizontalColorIndex {
			get { return FormatInfo.Border.HorizontalColorIndex; }
			set {
				if (FormatInfo.Border.HorizontalColorIndex == value)
					return;
				SetPropertyValue(cellFormatIndexAccessor, SetBorderHorizontalColorIndex, value);
			}
		}
		DocumentModelChangeActions SetBorderHorizontalColorIndex(FormatBase info, int value) {
			info.Border.HorizontalColorIndex = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region IBorderInfo.VerticalLineStyle
		XlBorderLineStyle IBorderInfo.VerticalLineStyle {
			get { return FormatInfo.Border.VerticalLineStyle; }
			set {
				if (FormatInfo.Border.VerticalLineStyle == value)
					return;
				SetPropertyValue(cellFormatIndexAccessor, SetBorderVerticalLineStyle, value);
			}
		}
		DocumentModelChangeActions SetBorderVerticalLineStyle(FormatBase info, XlBorderLineStyle value) {
			info.Border.VerticalLineStyle = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region IBorderInfo.VerticalColor
		Color IBorderInfo.VerticalColor {
			get { return FormatInfo.Border.VerticalColor; }
			set {
				if (FormatInfo.Border.VerticalColor == value)
					return;
				SetPropertyValue(cellFormatIndexAccessor, SetBorderVerticalColor, value);
			}
		}
		DocumentModelChangeActions SetBorderVerticalColor(FormatBase info, Color value) {
			info.Border.VerticalColor = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region IBorderInfo.VerticalColorIndex
		int IBorderInfo.VerticalColorIndex {
			get { return FormatInfo.Border.VerticalColorIndex; }
			set {
				if (FormatInfo.Border.VerticalColorIndex == value)
					return;
				SetPropertyValue(cellFormatIndexAccessor, SetBorderVerticalColorIndex, value);
			}
		}
		DocumentModelChangeActions SetBorderVerticalColorIndex(FormatBase info, int value) {
			info.Border.VerticalColorIndex = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region IBorderInfo.DiagonalUpLineStyle
		XlBorderLineStyle IBorderInfo.DiagonalUpLineStyle {
			get { return FormatInfo.Border.DiagonalUpLineStyle; }
			set {
				if (FormatInfo.Border.DiagonalUpLineStyle == value)
					return;
				SetPropertyValue(cellFormatIndexAccessor, SetBorderDiagonalUpLineStyle, value);
			}
		}
		DocumentModelChangeActions SetBorderDiagonalUpLineStyle(FormatBase info, XlBorderLineStyle value) {
			info.Border.DiagonalUpLineStyle = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region IBorderInfo.DiagonalDownLineStyle
		XlBorderLineStyle IBorderInfo.DiagonalDownLineStyle {
			get { return FormatInfo.Border.DiagonalDownLineStyle; }
			set {
				if (FormatInfo.Border.DiagonalDownLineStyle == value)
					return;
				SetPropertyValue(cellFormatIndexAccessor, SetBorderDiagonalDownLineStyle, value);
			}
		}
		DocumentModelChangeActions SetBorderDiagonalDownLineStyle(FormatBase info, XlBorderLineStyle value) {
			info.Border.DiagonalDownLineStyle = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region IBorderInfo.DiagonalColor
		Color IBorderInfo.DiagonalColor {
			get { return FormatInfo.Border.DiagonalColor; }
			set {
				if (FormatInfo.Border.DiagonalColor == value)
					return;
				SetPropertyValue(cellFormatIndexAccessor, SetBorderDiagonalColor, value);
			}
		}
		DocumentModelChangeActions SetBorderDiagonalColor(FormatBase info, Color value) {
			info.Border.DiagonalColor = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region IBorderInfo.DiagonalColorIndex
		int IBorderInfo.DiagonalColorIndex {
			get { return FormatInfo.Border.DiagonalColorIndex; }
			set {
				if (FormatInfo.Border.DiagonalColorIndex == value)
					return;
				SetPropertyValue(cellFormatIndexAccessor, SetBorderDiagonalColorIndex, value);
			}
		}
		DocumentModelChangeActions SetBorderDiagonalColorIndex(FormatBase info, int value) {
			info.Border.DiagonalColorIndex = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region IBorderInfo.Outline
		bool IBorderInfo.Outline {
			get { return FormatInfo.Border.Outline; }
			set {
				if (FormatInfo.Border.Outline == value)
					return;
				SetPropertyValue(cellFormatIndexAccessor, SetBorderOutline, value);
			}
		}
		DocumentModelChangeActions SetBorderOutline(FormatBase info, bool value) {
			info.Border.Outline = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#endregion
		#region IActualBorderInfo Members
		XlBorderLineStyle IActualBorderInfo.LeftLineStyle { get { return FormatInfo.ActualBorder.LeftLineStyle; } }
		XlBorderLineStyle IActualBorderInfo.RightLineStyle { get { return FormatInfo.ActualBorder.RightLineStyle; } }
		XlBorderLineStyle IActualBorderInfo.TopLineStyle { get { return FormatInfo.ActualBorder.TopLineStyle; } }
		XlBorderLineStyle IActualBorderInfo.BottomLineStyle { get { return FormatInfo.ActualBorder.BottomLineStyle; } }
		XlBorderLineStyle IActualBorderInfo.HorizontalLineStyle { get { return FormatInfo.ActualBorder.HorizontalLineStyle; } }
		XlBorderLineStyle IActualBorderInfo.VerticalLineStyle { get { return FormatInfo.ActualBorder.VerticalLineStyle; } }
		XlBorderLineStyle IActualBorderInfo.DiagonalUpLineStyle { get { return FormatInfo.ActualBorder.DiagonalUpLineStyle; } }
		XlBorderLineStyle IActualBorderInfo.DiagonalDownLineStyle { get { return FormatInfo.ActualBorder.DiagonalDownLineStyle; } }
		Color IActualBorderInfo.LeftColor { get { return FormatInfo.ActualBorder.LeftColor; } }
		Color IActualBorderInfo.RightColor { get { return FormatInfo.ActualBorder.RightColor; } }
		Color IActualBorderInfo.TopColor { get { return FormatInfo.ActualBorder.TopColor; } }
		Color IActualBorderInfo.BottomColor { get { return FormatInfo.ActualBorder.BottomColor; } }
		Color IActualBorderInfo.HorizontalColor { get { return FormatInfo.ActualBorder.HorizontalColor; } }
		Color IActualBorderInfo.VerticalColor { get { return FormatInfo.ActualBorder.VerticalColor; } }
		Color IActualBorderInfo.DiagonalColor { get { return FormatInfo.ActualBorder.DiagonalColor; } }
		int IActualBorderInfo.LeftColorIndex { get { return FormatInfo.ActualBorder.LeftColorIndex; } }
		int IActualBorderInfo.RightColorIndex { get { return FormatInfo.ActualBorder.RightColorIndex; } }
		int IActualBorderInfo.TopColorIndex { get { return FormatInfo.ActualBorder.TopColorIndex; } }
		int IActualBorderInfo.BottomColorIndex { get { return FormatInfo.ActualBorder.BottomColorIndex; } }
		int IActualBorderInfo.HorizontalColorIndex { get { return FormatInfo.ActualBorder.HorizontalColorIndex; } }
		int IActualBorderInfo.VerticalColorIndex { get { return FormatInfo.ActualBorder.VerticalColorIndex; } }
		int IActualBorderInfo.DiagonalColorIndex { get { return FormatInfo.ActualBorder.DiagonalColorIndex; } }
		bool IActualBorderInfo.Outline { get { return FormatInfo.ActualBorder.Outline; } }
		#endregion
		#region ICellProtectionInfo Members
		#region ICellProtectionInfo.Locked
		bool ICellProtectionInfo.Locked {
			get { return FormatInfo.Protection.Locked; }
			set {
				if (FormatInfo.Protection.Locked == value)
					return;
				SetPropertyValue(cellFormatIndexAccessor, SetProtectionLocked, value);
			}
		}
		DocumentModelChangeActions SetProtectionLocked(FormatBase info, bool value) {
			info.Protection.Locked = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region ICellProtectionInfo.Hidden
		bool ICellProtectionInfo.Hidden {
			get { return FormatInfo.Protection.Hidden; }
			set {
				if (FormatInfo.Protection.Hidden == value)
					return;
				SetPropertyValue(cellFormatIndexAccessor, SetProtectionHidden, value);
			}
		}
		DocumentModelChangeActions SetProtectionHidden(FormatBase info, bool value) {
			info.Protection.Hidden = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#endregion
		#region IActualCellProtectionInfo Members
		bool IActualCellProtectionInfo.Locked { get { return FormatInfo.ActualProtection.Locked; } }
		bool IActualCellProtectionInfo.Hidden { get { return FormatInfo.ActualProtection.Hidden; } }
		#endregion
		#region FormatString members
		public string FormatString {
			get { return FormatInfo.FormatString; }
			set {
				if (FormatInfo.FormatString == value)
					return;
				SetPropertyValue(cellFormatIndexAccessor, SetNumberFormat, value);
			}
		}
		DocumentModelChangeActions SetNumberFormat(FormatBase info, string value) {
			info.FormatString = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region ActualFormatString members
		public string ActualFormatString { get { return FormatInfo.ActualFormatString; } }
		#endregion
		#region ActualFormatIndex members
		public int ActualFormatIndex { get { return FormatInfo.ActualFormatIndex; } }
		#endregion
		#region IActualApplyInfo Members
		public IActualApplyInfo ActualApplyInfo { get { return this; } }  
		bool IActualApplyInfo.ApplyFont { get { return GetApplyValue(FormatInfo.ActualApplyInfo.ApplyFont); } }
		bool IActualApplyInfo.ApplyFill { get { return GetApplyValue(FormatInfo.ActualApplyInfo.ApplyFill); } }
		bool IActualApplyInfo.ApplyBorder { get { return GetApplyValue(FormatInfo.ActualApplyInfo.ApplyBorder); } }
		bool IActualApplyInfo.ApplyAlignment { get { return GetApplyValue(FormatInfo.ActualApplyInfo.ApplyAlignment); } }
		bool IActualApplyInfo.ApplyProtection { get { return GetApplyValue(FormatInfo.ActualApplyInfo.ApplyProtection); } }
		bool IActualApplyInfo.ApplyNumberFormat { get { return GetApplyValue(FormatInfo.ActualApplyInfo.ApplyNumberFormat); } }
		bool GetApplyValue(bool apply) {
			return apply || ApplyStyle;
		}
		#endregion
		#endregion
		#region CellStyle members
		public CellStyleBase Style { get { return FormatInfo.Style; } set { SetPropertyValue(cellFormatIndexAccessor, SetStyleIndex, value); } }
		DocumentModelChangeActions SetStyleIndex(FormatBase info, CellStyleBase value) {
			((CellFormat)info).ApplyStyle(value, Sheet.IsProtected);
			return DocumentModelChangeActions.None;
		}
		#endregion
		public override string ToString() {
			return String.Format("Range=[{2}:{2}], Index=[{0}] sheet='{1}'", Index, Sheet.Name, Index + 1, Index + 1);
		}
		public CellIntervalRange GetCellIntervalRange() {
			return CellIntervalRange.CreateRowInterval(Sheet, Index, PositionType.Absolute, Index, PositionType.Absolute);
		}
		#region Check Integrity
		public void CheckIntegrity(CheckIntegrityFlags flags) {
			if (sheet == null)
				IntegrityChecks.Fail("RowBase: sheet should not be null");
			if (index < 0)
				IntegrityChecks.Fail(String.Format("RowBase: invalid index={0}", index));
			if (Sheet == null)
				IntegrityChecks.Fail("Row: Sheet should not be null");
			if ((flags & CheckIntegrityFlags.SkipTimeConsumingChecks) == 0) {
				if (!sheet.Rows.Contains(this))
					IntegrityChecks.Fail("Row: Sheet.Rows.Contains(this) == false");
			}
			if (cells != null) {
				for (int i = 0; i < Cells.InnerList.Count; i++) {
					CheckCellIntegrity(Cells.InnerList[i], flags);
				}
			}
		}
		public void CheckCellIntegrity(ICell cell, CheckIntegrityFlags flags) {
			if (cell.Key.RowIndex != this.Index)
				IntegrityChecks.Fail(String.Format("Row: inconsistent cell in row: rowIndex={0}, cell.Key={1}", this.Index, cell.Key));
			cell.CheckIntegrity(flags);
		}
		#endregion
		public void SetCustomHeight(float valueInModelUnits) {
			if (valueInModelUnits == 0) {
				if (!IsHidden)
					Sheet.HideRows(Index, Index);
				return;
			}
			DocumentModel.BeginUpdate();
			this.BeginUpdate();
			try {
				if (IsHidden)
					Sheet.UnhideRows(Index, Index);
				this.IsCustomHeight = true;
				this.Height = valueInModelUnits;
				InvalidateAnchorDatasByRowHistoryItem historyItem = new InvalidateAnchorDatasByRowHistoryItem(Sheet, Index);
				DocumentModel.History.Add(historyItem);
				historyItem.Execute();
			}
			finally {
				this.EndUpdate();
				DocumentModel.EndUpdate();
			}
		}
		#region Applies.ApplyNumberFormat
		public bool ApplyNumberFormat {
			get { return FormatInfo.ApplyNumberFormat; }
			set {
				if (FormatInfo.ApplyNumberFormat == value)
					return;
				SetPropertyValue(cellFormatIndexAccessor, SetApplyNumberFormatCore, value);
			}
		}
		DocumentModelChangeActions SetApplyNumberFormatCore(FormatBase info, bool value) {
			((CellFormatBase)info).ApplyNumberFormat = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region Applies.ApplyFont
		public bool ApplyFont {
			get { return FormatInfo.ApplyFont; }
			set {
				if (FormatInfo.ApplyFont == value)
					return;
				SetPropertyValue(cellFormatIndexAccessor, SetApplyFontCore, value);
			}
		}
		DocumentModelChangeActions SetApplyFontCore(FormatBase info, bool value) {
			((CellFormatBase)info).ApplyFont = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region Applies.ApplyFill
		public bool ApplyFill {
			get { return FormatInfo.ApplyFill; }
			set {
				if (FormatInfo.ApplyFill == value)
					return;
				SetPropertyValue(cellFormatIndexAccessor, SetApplyFillCore, value);
			}
		}
		DocumentModelChangeActions SetApplyFillCore(FormatBase info, bool value) {
			((CellFormatBase)info).ApplyFill = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region Applies.ApplyBorder
		public bool ApplyBorder {
			get { return FormatInfo.ApplyBorder; }
			set {
				if (FormatInfo.ApplyBorder == value)
					return;
				SetPropertyValue(cellFormatIndexAccessor, SetApplyBorderCore, value);
			}
		}
		DocumentModelChangeActions SetApplyBorderCore(FormatBase info, bool value) {
			((CellFormatBase)info).ApplyBorder = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region Applies.ApplyAlignment
		public bool ApplyAlignment {
			get { return FormatInfo.ApplyAlignment; }
			set {
				if (FormatInfo.ApplyAlignment == value)
					return;
				SetPropertyValue(cellFormatIndexAccessor, SetApplyAlignmentCore, value);
			}
		}
		DocumentModelChangeActions SetApplyAlignmentCore(FormatBase info, bool value) {
			((CellFormatBase)info).ApplyAlignment = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region Applies.ApplyProtection
		public bool ApplyProtection {
			get { return FormatInfo.ApplyProtection; }
			set {
				if (FormatInfo.ApplyProtection == value)
					return;
				SetPropertyValue(cellFormatIndexAccessor, SetApplyProtectionCore, value);
			}
		}
		DocumentModelChangeActions SetApplyProtectionCore(FormatBase info, bool value) {
			((CellFormatBase)info).ApplyProtection = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
#if BTREE
		#region IIndexedObject Members
		long IIndexedObject.Index {			get { return Index; }		}
		#endregion
#endif
		public void CopyFrom(Row other, int defaultOtherFormatIndex) {
			if (Object.ReferenceEquals(this.DocumentModel, other.DocumentModel)) {
				base.CopyFrom(other);
				return;
			}
			try {
				System.Diagnostics.Debug.Assert(!this.IsUpdateLocked);
				this.BeginUpdate();
				int count = IndexAccessors.Length;
				System.Diagnostics.Debug.Assert(3 == count);
				if (other.FormatIndex != defaultOtherFormatIndex)
					CellFormatIndexAccessor.CopyDeferredInfo(this, other);
				HeightIndexAccessor.CopyDeferredInfo(this, other);
				InfoIndexAccessor.CopyDeferredInfo(this, other);
			}
			finally {
				this.EndUpdate();
			}
		}
	}
	#endregion
	public enum RemoveRowMode {
		Default,
		MoveRowsUp
	}
	#region IRowCollectionBase
	public interface IRowCollectionBase : IEnumerable {
		IEnumerable GetExistingRows(int topRow, int bottomRow, bool reverseOrder);
		IEnumerator GetExistingRowsEnumerator(int topRow, int bottomRow);
		IEnumerator GetExistingRowsEnumerator(int topRow, int bottomRow, bool reverseOrder);
		IEnumerable GetExistingVisibleRows(int topRow, int bottomRow, bool reverseOrder);
		IRowBase TryGetRow(int index);
		int TryGetRowIndex(int modelIndex);
		int LastRowIndex { get; }
	}
	#endregion
	#region RowCollectionBase (abstract class)
	public abstract class RowCollectionBase<T> : IEnumerable<T> where T : class, IRowBase {
		#region Fields
		readonly List<T> innerList = new List<T>();
		#endregion
		protected RowCollectionBase() {
		}
		#region Properties
		public T this[int index] {
			get {
				if (!IndicesChecker.CheckIsRowIndexValid(index))
					Exceptions.ThrowArgumentException("index", index);
				return GetRow(index);
			}
		}
		public virtual IList<T> InnerList { get { return innerList; } }
		public T First { get { return Count > 0 ? InnerList[0] : null; } }
		public T Last { get { return Count > 0 ? InnerList[Count - 1] : null; } }
		public int Count { get { return InnerList.Count; } }
		public int LastRowIndex { get { return (int)Last.Index; } }
		#endregion
		public virtual T GetRow(int index) {
			int position = TryGetRowIndex(index);
			if (position < 0) {
				position = ~position;
				T row = CreateRow(index);
				Insert(position, row);
				return row;
			}
			else
				return InnerList[position];
		}
		public virtual T TryGetRow(int index) {
			int position = TryGetRowIndex(index);
			if (position < 0)
				return null;
			return InnerList[position];
		}
		public virtual int TryGetRowIndex(int modelIndex) {
			return TryGetRowIndexCore(modelIndex);
		}
		protected int TryGetRowIndexCore(int modelIndex) {
			return Algorithms.BinarySearch<T>(InnerList, new RowComparable<T>(modelIndex));
		}
		public bool Contains(int index) {
			int position = TryGetRowIndex(index);
			return position >= 0;
		}
		public bool Contains(T item) {
			return InnerList.Contains(item);
		}
		public IEnumerable<T> GetExistingRows() {
			return new Enumerable<T>(GetExistingRowsEnumerator(0, IndicesChecker.MaxRowCount - 1, false));
		}
		public IEnumerable<T> GetExistingRows(int topRow, int bottomRow, bool reverseOrder) {
			if (!reverseOrder)
				return new Enumerable<T>(GetExistingRowsEnumerator(topRow, bottomRow));
			return new Enumerable<T>(GetExistingRowsEnumerator(topRow, bottomRow, reverseOrder));
		}
		public IEnumerator<T> GetExistingRowsEnumerator(int topRow, int bottomRow) {
			return new ExistingRowsEnumeratorFast<T>(InnerList, topRow, bottomRow, null);
		}
		public IOrderedItemsRangeEnumerator<T> GetExistingRowsEnumerator(int topRow, int bottomRow, bool reverseOrder) {
			return new ExistingRowsEnumerator<T>(InnerList, topRow, bottomRow, reverseOrder, null);
		}
		public IOrderedItemsRangeEnumerator<T> GetExistingRowsEnumerator(int topRow, int bottomRow, bool reverseOrder, Predicate<T> filter) {
			return new ExistingRowsEnumerator<T>(InnerList, topRow, bottomRow, reverseOrder, filter);
		}
		public IEnumerable<T> GetExistingVisibleRows(int topRow, int bottomRow, bool reverseOrder) {
			return new Enumerable<T>(GetExistingVisibleRowsEnumerator(topRow, bottomRow, reverseOrder));
		}
		public IOrderedEnumerator<T> GetExistingNotVisibleRowsEnumerator(int topRow, int bottomRow, bool reverseOrder) {
			return new ExistingNotVisibleRowsEnumerator<T>(InnerList, topRow, bottomRow, reverseOrder, null);
		}
		public IOrderedEnumerator<T> GetExistingVisibleRowsEnumerator(int topRow, int bottomRow, bool reverseOrder) {
			return new ExistingVisibleRowsEnumerator<T>(InnerList, topRow, bottomRow, reverseOrder, null);
		}
		protected abstract bool CanRemove(int rowIndex);
		#region RemoveRange
		public virtual void RemoveRange(int from, int count) {
			if (InnerList.Count == 0)
				return;
			int positionFrom = TryGetRowIndex(from);
			int positionTo = TryGetRowIndex(from + count - 1);
			bool removeExistingRows = false;
			if (positionFrom < 0 && positionTo < 0) {
				positionFrom = ~positionFrom;
				if (positionFrom == InnerList.Count)
					return;
				positionTo = ~positionTo;
				if (positionTo != positionFrom) {
					positionTo--;
					removeExistingRows = true;
				}
			}
			else {
				if (positionFrom < 0)
					positionFrom = ~positionFrom;
				if (positionTo < 0)
					positionTo = ~positionTo - 1;
				removeExistingRows = true;
			}
			RemoveRangeCore(positionFrom, positionTo - positionFrom + 1, count, removeExistingRows);
		}
		public virtual void RemoveRangeCore(int positionFrom, int innerCount, int count, bool removeExistingRows) {
			RemoveRangeInner(positionFrom, innerCount, count, removeExistingRows);
		}
		public virtual void RemoveRangeInner(int index, int innerCount, int count, bool removeExistingRows) {
			if (removeExistingRows)
				InnerList.RemoveRange(index, innerCount);
			OffsetCellRowIndices(index, -count);
		}
		public virtual void RemoveCore(int index) {
			InnerList.RemoveAt(index);
		}
		public void RemoveRangeCore(int index, int count) {
			OffsetCellRowIndices(index, -count);
		}
		#endregion
		#region Insert
		public virtual void Insert(int index, T item) {
			InsertCore(index, item);
		}
		public virtual void InsertCore(int index, T item) {
			InnerList.Insert(index, item);
		}
		public virtual void InsertRangeFromHistory(int index, IList<T> items, int offsetCount) {
			OffsetCellRowIndices(index, offsetCount);
			if (items != null)
				InnerList.InsertRange(index, items);
		}
		#endregion
		public abstract T CreateRow(int index);
		protected abstract void OffsetCellRowIndices(int rowIndex, int offset);
		#region IEnumerable<T> Members
		IEnumerator<T> IEnumerable<T>.GetEnumerator() {
			return InnerList.GetEnumerator();
		}
		#endregion
		#region IEnumerable Members
		IEnumerator IEnumerable.GetEnumerator() {
			return InnerList.GetEnumerator();
		}
		#endregion
		public void ForEach(Action<T> action) {
			InnerList.ForEach(action);
		}
		public List<T> GetInnerRange(int position, int innerCount) {
			List<T> result = new List<T>(innerCount);
			for (int i = position; i < position + innerCount; i++)
				result.Add(InnerList[i]);
			return result;
		}
		public virtual void Clear() {
			InnerList.Clear();
		}
	}
	#endregion
	#region CachedRowCollectionBase (abstract class)
	public abstract class CachedRowCollectionBase<T> : RowCollectionBase<T> where T : class, IRowBase {
		#region Fields
		readonly IWorksheet sheet;
		int cachedModelIndex;
		int cachedRowIndex;
		#endregion
		protected CachedRowCollectionBase(IWorksheet sheet)
			: base() {
			Guard.ArgumentNotNull(sheet, "sheet");
			this.sheet = sheet;
			ResetTryGetRowIndexCache();
		}
		#region Properties
		public IWorksheet Sheet { get { return sheet; } }
		#endregion
		public override T GetRow(int index) {
			int position = TryGetRowIndex(index);
			if (position < 0) {
				position = ~position;
				T row = CreateRow(index);
				Insert(position, row);
				cachedRowIndex = position; 
				cachedModelIndex = index;
				return row;
			}
			else
				return InnerList[position];
		}
		public override int TryGetRowIndex(int modelIndex) {
			int workBookContentVersion = sheet.Workbook.ContentVersion;
			if (cachedModelIndex == modelIndex)
				return cachedRowIndex;
			cachedRowIndex++;
			if (cachedRowIndex >= 0 && cachedModelIndex + 1 == modelIndex) {
				if (cachedRowIndex < this.Count) {
					T row = this.InnerList[cachedRowIndex];
					if (row.Index == modelIndex) {
						cachedModelIndex = modelIndex;
						return cachedRowIndex;
					}
				}
			}
			cachedModelIndex = modelIndex;
			cachedRowIndex = TryGetRowIndexCore(modelIndex);
			return cachedRowIndex;
		}
		protected virtual void ResetTryGetRowIndexCache() {
			cachedModelIndex = -1;
		}
		#region RemoveRange
		public override void RemoveRange(int from, int count) {
			if (InnerList.Count == 0)
				return;
			int positionFrom = TryGetRowIndex(from);
			int positionTo = TryGetRowIndex(from + count - 1);
			bool removeRange = false;
			if (positionFrom < 0 && positionTo < 0) {
				positionFrom = ~positionFrom;
				if (positionFrom == InnerList.Count)
					return;
				positionTo = ~positionTo;
				if (positionTo != positionFrom) {
					positionTo--;
					ResetTryGetRowIndexCache();
					removeRange = true;
				}
			}
			else {
				if (positionFrom < 0)
					positionFrom = ~positionFrom;
				if (positionTo < 0)
					positionTo = ~positionTo - 1;
				ResetTryGetRowIndexCache();
				removeRange = true;
			}
			RemoveRangeCore(positionFrom, positionTo - positionFrom + 1, count, removeRange);
		}
		public override void RemoveRangeInner(int index, int innerCount, int count, bool removeRange) {
			ResetTryGetRowIndexCache();
			base.RemoveRangeInner(index, innerCount, count, removeRange);
		}
		public override void RemoveCore(int index) {
			ResetTryGetRowIndexCache();
			base.RemoveCore(index);
		}
		#endregion
		#region Insert
		public override void InsertCore(int index, T item) {
			ResetTryGetRowIndexCache();
			base.InsertCore(index, item);
		}
		public override void InsertRangeFromHistory(int index, IList<T> items, int offsetCount) {
			ResetTryGetRowIndexCache();
			base.InsertRangeFromHistory(index, items, offsetCount);
		}
		#endregion
		public override void Clear() {
			ResetTryGetRowIndexCache();
			base.Clear();
		}
		public void SetCachedData(int cachedRowIndex, int cachedRowCollectionIndex) {
			this.cachedModelIndex = cachedRowIndex;
			this.cachedRowIndex = cachedRowCollectionIndex;
		}
	}
	#endregion
	#region IRowCollectionGeneric
	public interface IRowCollectionGeneric<T> : IRowCollectionBase, IEnumerable<T> where T : IRowBase {
		bool Contains(int index);
		bool Contains(T item);
		T this[int index] { get; }
		IList<T> InnerList { get; }
		T First { get; }
		T Last { get; }
		int Count { get; }
		void ForEach(Action<T> action);
		T GetRow(int index);
		new T TryGetRow(int index);
		T CreateRow(int index);
		void SetCachedData(int cachedRowIndex, int cachedRowCollectionIndex);
		IEnumerable<T> GetExistingRows();
		new IEnumerable<T> GetExistingRows(int topRow, int bottomRow, bool reverseOrder);
		new IEnumerator<T> GetExistingRowsEnumerator(int topRow, int bottomRow);
		new IOrderedItemsRangeEnumerator<T> GetExistingRowsEnumerator(int topRow, int bottomRow, bool reverseOrder);
		IOrderedItemsRangeEnumerator<T> GetExistingRowsEnumerator(int topRow, int bottomRow, bool reverseOrder, Predicate<T> filter);
		new IEnumerable<T> GetExistingVisibleRows(int topRow, int bottomRow, bool reverseOrder);
		IOrderedEnumerator<T> GetExistingVisibleRowsEnumerator(int topRow, int bottomRow, bool reverseOrder);
		IOrderedEnumerator<T> GetExistingNotVisibleRowsEnumerator(int topRow, int bottomRow, bool reverseOrder);
		void RemoveRange(int from, int count);
		void RemoveRangeCore(int positionFrom, int innerCount, int count, bool removeRange);
		void RemoveRangeInner(int index, int innerCount, int count, bool removeRange);
		void RemoveCore(int index);
		void RemoveRangeCore(int index, int count);
		void InsertCore(int index, T item);
		void InsertRangeFromHistory(int index, IList<T> items, int offsetCount);
		List<T> GetInnerRange(int position, int innerCount);
	}
	#endregion
	#region IRowCollection
	public interface IRowCollection : IRowCollectionGeneric<Row> {
		Worksheet Sheet { get; }
		void Insert(int index, Row item);
		void Clear();
		IOrderedEnumerator<Row> GetAllRowsEnumerator(int topRow, int bottomRow, bool reverseOrder);
		Row GetRowForReading(int rowIndex);
		List<Row> GetRowsForReading(int topIndex, int bottomIndex);
		void InsertRowsShiftDownCore(int position, int count);
		void InsertRowsShiftDown(int rowIndex, int count);
		void CheckForEmptyRows(int startIndex, int endIndex);
	}
	#endregion
	#region RowCollection
	public class RowCollection : CachedRowCollectionBase<Row>, IRowCollection {
		public RowCollection(Worksheet sheet)
			: base(sheet) {
		}
		#region Properties
		public new Worksheet Sheet { get { return (Worksheet)base.Sheet; } }
		#endregion
		public override Row CreateRow(int index) {
			return Sheet.CreateRowCore(index);
		}
		public void InsertRowsShiftDown(int rowIndex, int count) {
			int position = TryGetRowIndexCore(rowIndex);
			if (position < 0)
				position = ~position;
			if (position >= Count)
				return;
			DocumentHistory history = Sheet.Workbook.History;
			SpreadsheetRowsMovedHistoryItem historyItem = new SpreadsheetRowsMovedHistoryItem(Sheet, position, count);
			history.Add(historyItem);
			historyItem.Execute();
		}
		public void InsertRowsShiftDownCore(int position, int count) {
			for (int i = Count - 1; i >= position; i--)
				InnerList[i].OffsetRowIndexCore(count);
			ResetTryGetRowIndexCache();
		}
		protected override bool CanRemove(int position) {
			if (position < 0)
				return true;
			Row row = InnerList[position];
			foreach (ICell cell in row.Cells) {
				FormulaBase formula = cell.GetFormula();
				if ((formula is ArrayFormula) || (formula is ArrayFormulaPart))
					return false;
			}
			return true;
		}
		public override void Insert(int index, Row item) {
			DocumentHistory history = Sheet.Workbook.History;
			SpreadsheetRowInsertedHistoryItem historyItem = new SpreadsheetRowInsertedHistoryItem(Sheet, item, index);
			history.Add(historyItem);
			historyItem.Execute();
		}
		public override void RemoveRangeCore(int positionFrom, int innerCount, int count, bool removeExistingRows) {
			DocumentHistory history = Sheet.Workbook.History;
			SpreadsheetRowRangeRemovedHistoryItem historyItem = new SpreadsheetRowRangeRemovedHistoryItem(Sheet, positionFrom, innerCount, count, removeExistingRows);
			history.Add(historyItem);
			historyItem.Execute();
		}
		#region IRowCollection Members
		IEnumerable IRowCollectionBase.GetExistingRows(int topRow, int bottomRow, bool reverseOrder) {
			return GetExistingRows(topRow, bottomRow, reverseOrder);
		}
		IEnumerable IRowCollectionBase.GetExistingVisibleRows(int topRow, int bottomRow, bool reverseOrder) {
			return GetExistingVisibleRows(topRow, bottomRow, reverseOrder);
		}
		IRowBase IRowCollectionBase.TryGetRow(int index) {
			return this.TryGetRow(index);
		}
		IEnumerator IRowCollectionBase.GetExistingRowsEnumerator(int topRow, int bottomRow) {
			return GetExistingRowsEnumerator(topRow, bottomRow);
		}
		IEnumerator IRowCollectionBase.GetExistingRowsEnumerator(int topRow, int bottomRow, bool reverseOrder) {
			return GetExistingRowsEnumerator(topRow, bottomRow, reverseOrder);
		}
		#endregion
		protected override void OffsetCellRowIndices(int startRowIndex, int offset) {
			if (InnerList.Count > 0) {
				ResetTryGetRowIndexCache();
				for (int i = startRowIndex; i < InnerList.Count; i++)
					InnerList[i].OffsetRowIndexCore(offset);
			}
		}
		public override void Clear() {
			Sheet.Workbook.InternalAPI.OnBeforeRowsCleared(Sheet);
			base.Clear();
		}
		public IOrderedEnumerator<Row> GetAllRowsEnumerator(int topRow, int bottomRow, bool reverseOrder) {
			return new AllRowsEnumerator(Sheet, topRow, bottomRow, reverseOrder, null);
		}
		#region CheckForEmptyRows
		public void CheckForEmptyRows(int startIndex, int endIndex) {
			if (!IndicesChecker.CheckIsRowIndexValid(startIndex))
				Exceptions.ThrowArgumentException("startIndex", startIndex);
			if (!IndicesChecker.CheckIsRowIndexValid(endIndex))
				Exceptions.ThrowArgumentException("endIndex", endIndex);
			if (startIndex > endIndex)
				Exceptions.ThrowArgumentException("Start index is greater than end index", startIndex);
			Sheet.Workbook.BeginUpdate();
			try {
				for (int i = startIndex; i <= endIndex; i++)
					CheckForEmptyRow(i);
			}
			finally {
				Sheet.Workbook.EndUpdate();
			}
		}
		void CheckForEmptyRow(int rowIndex) {
			int position = TryGetRowIndexCore(rowIndex);
			if (position < 0)
				return;
			Row row = InnerList[position];
			ICellCollection cells = row.Cells;
			Row defaultRow = new Row(row.Index, Sheet);
			if (row.HasSameFormatting(defaultRow) && (cells == null || cells.Count == 0))
				DeleteRow(row, position);
		}
		void DeleteRow(Row row, int position) {
			DocumentHistory history = Sheet.Workbook.History;
			SpreadsheetDeleteRowHistoryItem historyItem = new SpreadsheetDeleteRowHistoryItem(row, position);
			history.Add(historyItem);
			historyItem.Execute();
		}
		#endregion
		public Row GetRowForReading(int rowIndex) {
			if (!IndicesChecker.CheckIsRowIndexValid(rowIndex))
				Exceptions.ThrowArgumentException("rowIndex", rowIndex);
			Row result = TryGetRow(rowIndex);
			if (result == null)
				return new Row(rowIndex, Sheet);
			else
				return result;
		}
		public List<Row> GetRowsForReading(int topIndex, int bottomIndex) {
			List<Row> result = new List<Row>();
			for (int i = topIndex; i <= bottomIndex; i++)
				result.Add(GetRowForReading(i));
			return result;
		}
		protected override void ResetTryGetRowIndexCache() {
			base.ResetTryGetRowIndexCache();
			Sheet.ResetCachedData();
		}
	}
	#endregion
	#region RowCellFormatIndexAccessor
	public class RowCellFormatIndexAccessor : IIndexAccessor<Row, FormatBase, DocumentModelChangeActions> {
		#region IIndexAccessor Members
		public int GetIndex(Row owner) {
			return owner.FormatIndex;
		}
		public int GetDeferredInfoIndex(Row owner) {
			return GetInfoIndex(owner, GetDeferredInfo(owner.BatchUpdateHelper));
		}
		public void SetIndex(Row owner, int value) {
			owner.AssignCellFormatIndex(value);
		}
		public int GetInfoIndex(Row owner, FormatBase value) {
			return GetInfoCache(owner).GetItemIndex(value);
		}
		public FormatBase GetInfo(Row owner) {
			return GetInfoCache(owner)[GetIndex(owner)];
		}
		public bool IsIndexValid(Row owner, int index) {
			return index < GetInfoCache(owner).Count;
		}
		UniqueItemsCache<FormatBase> GetInfoCache(Row owner) {
			return owner.DocumentModel.Cache.CellFormatCache;
		}
		public IndexChangedHistoryItemCore<DocumentModelChangeActions> CreateHistoryItem(Row owner) {
			return new RowCellFormatIndexChangeHistoryItem(owner);
		}
		public FormatBase GetDeferredInfo(MultiIndexBatchUpdateHelper helper) {
			return ((RowBatchUpdateHelper)helper).CellFormat;
		}
		public void SetDeferredInfo(MultiIndexBatchUpdateHelper helper, FormatBase info) {
			RowBatchUpdateHelper rowBatchUpdateHelper = helper as RowBatchUpdateHelper;
			if (rowBatchUpdateHelper.CellFormat == null)
				rowBatchUpdateHelper.CellFormat = info.Clone();
			else
				rowBatchUpdateHelper.CellFormat.CopyFromDeferred(info);
		}
		public void InitializeDeferredInfo(Row owner) {
			this.SetDeferredInfo(owner.BatchUpdateHelper, GetInfo(owner));
		}
		public void CopyDeferredInfo(Row owner, Row from) {
			SetDeferredInfo(owner.BatchUpdateHelper, GetInfo(from));
		}
		public bool ApplyDeferredChanges(Row owner) {
			return owner.ReplaceInfo(this, GetDeferredInfo(owner.BatchUpdateHelper), owner.GetBatchUpdateChangeActions());
		}
		#endregion
	}
	#endregion
	#region RowHeightIndexAccessor
	public class RowHeightIndexAccessor : IIndexAccessor<Row, RowHeightInfo, DocumentModelChangeActions> {
		#region IIndexAccessor<Row, RowHeightInfo> Members
		public int GetIndex(Row owner) {
			return owner.HeightIndex;
		}
		public int GetDeferredInfoIndex(Row owner) {
			return GetInfoIndex(owner, GetDeferredInfo(owner.BatchUpdateHelper));
		}
		public void SetIndex(Row owner, int value) {
			owner.AssignHeightIndex(value);
		}
		public int GetInfoIndex(Row owner, RowHeightInfo value) {
			return value.IntValue;
		}
		public RowHeightInfo GetInfo(Row owner) {
			RowHeightInfo info = new RowHeightInfo();
			info.IntValue = owner.HeightIndex;
			return info;
		}
		public bool IsIndexValid(Row owner, int index) {
			return true;
		}
		public IndexChangedHistoryItemCore<DocumentModelChangeActions> CreateHistoryItem(Row owner) {
			return new RowHeightIndexChangeHistoryItem(owner);
		}
		public RowHeightInfo GetDeferredInfo(MultiIndexBatchUpdateHelper helper) {
			return ((RowBatchUpdateHelper)helper).HeightInfo;
		}
		public void SetDeferredInfo(MultiIndexBatchUpdateHelper helper, RowHeightInfo info) {
			((RowBatchUpdateHelper)helper).HeightInfo = info.Clone();
		}
		public void InitializeDeferredInfo(Row owner) {
			this.SetDeferredInfo(owner.BatchUpdateHelper, GetInfo(owner));
		}
		public void CopyDeferredInfo(Row owner, Row from) {
			SetDeferredInfo(owner.BatchUpdateHelper, GetInfo(from));
		}
		public bool ApplyDeferredChanges(Row owner) {
			return owner.ReplaceInfoForFlags(this, GetDeferredInfo(owner.BatchUpdateHelper), owner.GetBatchUpdateChangeActions());
		}
		#endregion
	}
	#endregion
	#region RowInfoIndexAccessor
	public class RowInfoIndexAccessor : IIndexAccessor<Row, RowInfo, DocumentModelChangeActions> {
		#region IIndexAccessor<Row, RowInfo> Members
		public int GetIndex(Row owner) {
			return owner.InfoIndex;
		}
		public int GetDeferredInfoIndex(Row owner) {
			return GetInfoIndex(owner, GetDeferredInfo(owner.BatchUpdateHelper));
		}
		public void SetIndex(Row owner, int value) {
			owner.AssignInfoIndex(value);
		}
		public int GetInfoIndex(Row owner, RowInfo value) {
			return value.PackedValues;
		}
		public RowInfo GetInfo(Row owner) {
			RowInfo info = new RowInfo();
			info.PackedValues = owner.InfoIndex;
			return info;
		}
		public bool IsIndexValid(Row owner, int index) {
			return true;
		}
		public IndexChangedHistoryItemCore<DocumentModelChangeActions> CreateHistoryItem(Row owner) {
			return new RowInfoIndexChangeHistoryItem(owner);
		}
		public RowInfo GetDeferredInfo(MultiIndexBatchUpdateHelper helper) {
			return ((RowBatchUpdateHelper)helper).Info;
		}
		public void SetDeferredInfo(MultiIndexBatchUpdateHelper helper, RowInfo info) {
			((RowBatchUpdateHelper)helper).Info = info.Clone();
		}
		public void InitializeDeferredInfo(Row owner) {
			this.SetDeferredInfo(owner.BatchUpdateHelper, GetInfo(owner));
		}
		public void CopyDeferredInfo(Row owner, Row from) {
			SetDeferredInfo(owner.BatchUpdateHelper, GetInfo(from));
		}
		public bool ApplyDeferredChanges(Row owner) {
			return owner.ReplaceInfoForFlags(this, GetDeferredInfo(owner.BatchUpdateHelper), owner.GetBatchUpdateChangeActions());
		}
		#endregion
	}
	#endregion
	#region ExistingRowsEnumerator
	public class ExistingRowsEnumerator<T> : SparseOrderedItemsRangeEnumerator<T> where T : IRowBase {
		public ExistingRowsEnumerator(IList<T> rows, int nearItemIndex, int farItemIndex, bool reverseOrder, Predicate<T> filter)
			: base(rows, nearItemIndex, farItemIndex, reverseOrder, filter) {
		}
		protected internal override IComparable<T> CreateComparable(int itemIndex) {
			return new RowComparable<T>(itemIndex);
		}
		protected internal override int GetCurrentValueOrder(int itemIndex) {
			return (int)GetItem(itemIndex).Index;
		}
	}
	#endregion
	#region ExistingRowsEnumeratorFast
	public class ExistingRowsEnumeratorFast<T> : IEnumerator<T> where T : IRowBase {
		readonly IList<T> rows;
		readonly int nearItemIndex;
		readonly int farItemIndex;
		readonly Predicate<T> filter;
		int currentIndex;
		T currentItem;
		public ExistingRowsEnumeratorFast(IList<T> rows, int nearItemIndex, int farItemIndex, Predicate<T> filter) {
			this.rows = rows;
			this.nearItemIndex = nearItemIndex;
			this.farItemIndex = farItemIndex;
			this.currentIndex = -1;
			this.filter = filter;
		}
		protected internal IComparable<T> CreateComparable(int itemIndex) {
			return new RowComparable<T>(itemIndex);
		}
		protected int CalculateActualFirstIndex() {
			int actualFirstIndex = Algorithms.BinarySearch(rows, CreateComparable(nearItemIndex));
			if (actualFirstIndex < 0)
				actualFirstIndex = ~actualFirstIndex;
			return actualFirstIndex;
		}
		#region IEnumerator<T> Members
		public T Current { get { return currentItem; } }
		#endregion
		#region IDisposable Members
		public void Dispose() {
			currentIndex = -1;
			currentItem = default(T);
		}
		#endregion
		#region IEnumerator Members
		object IEnumerator.Current { get { return Current; } }
		public bool MoveNext() {
			if (currentIndex < 0)
				currentIndex = CalculateActualFirstIndex();
			else
				currentIndex++;
			for (; ; ) {
				if (currentIndex >= rows.Count)
					return false;
				currentItem = rows[currentIndex];
				if (filter == null || filter(currentItem))
					break;
				currentIndex++;
			}
			return currentItem.Index <= farItemIndex;
		}
		public void Reset() {
			currentIndex = -1;
			currentItem = default(T);
		}
		#endregion
	}
	#endregion
	#region ContinuousRowsEnumerator
	public class ContinuousRowsEnumerator : OrderedItemsRangeEnumerator<Row> {
		readonly Worksheet sheet;
		public ContinuousRowsEnumerator(Worksheet sheet, int nearItemIndex, int farItemIndex, bool reverseOrder, Predicate<Row> filter)
			: base(null, nearItemIndex, farItemIndex, reverseOrder, filter) {
			this.sheet = sheet;
		}
		public Worksheet Sheet { get { return sheet; } }
		protected override Row GetItemCore(int index) {
			return CreateFakeRow(index);
		}
		protected internal override int GetCurrentValueOrder(int itemIndex) {
			return itemIndex;
		}
		protected virtual Row CreateFakeRow(int index) {
			return new Row(index, sheet);
		}
		public override void OnObjectInserted(int insertedItemValueOrder) {
		}
	}
	#endregion
	#region AllRowsEnumeratorFORREADONLY
	public class AllRowsEnumerator : ContinuousRowsEnumerator {
		public AllRowsEnumerator(Worksheet sheet, int nearItemIndex, int farItemIndex, bool reverseOrder, Predicate<Row> filter)
			: base(sheet, nearItemIndex, farItemIndex, reverseOrder, filter) {
		}
		protected override Row GetItemCore(int index) {
			return Sheet.Rows[index];
		}
		protected internal override int GetCurrentValueOrder(int itemIndex) {
			return itemIndex;
		}
	}
	#endregion
	#region ContinuousRowsEnumeratorFakeRowsAsNull
	public class ContinuousRowsEnumeratorFakeRowsAsNull : ContinuousRowsEnumerator {
		public ContinuousRowsEnumeratorFakeRowsAsNull(Worksheet worksheet, int nearItemIndex, int farItemIndex, bool reverseOrder, Predicate<Row> filter)
			: base(null, nearItemIndex, farItemIndex, reverseOrder, filter) {
		}
		protected override Row CreateFakeRow(int index) {
			return null;
		}
	}
	#endregion
	#region ContinuosFakeRowsEnumerator
	public class ContinuousFakeRowsEnumerator : ContinuousRowsEnumerator {
		public ContinuousFakeRowsEnumerator(Worksheet sheet, int nearItemIndex, int farItemIndex, bool reverseOrder, Predicate<Row> filter)
			: base(sheet, nearItemIndex, farItemIndex, reverseOrder, filter) {
		}
		protected override bool IsValidItem(int index) {
			return !Sheet.Rows.Contains(index);
		}
	}
	#endregion
	#region RowLayoutVisibleCellsEnumerator
	public class RowLayoutVisibleCellsEnumerator : IOrderedEnumerator<ICell> {
		readonly Row row;
		readonly int firstColumnIndex;
		readonly int lastColumnIndex;
		int columnIndex;
		ICell current;
		public RowLayoutVisibleCellsEnumerator(Row row, int firstColumnIndex, int lastColumnIndex) {
			this.row = row;
			this.firstColumnIndex = firstColumnIndex;
			this.lastColumnIndex = lastColumnIndex;
			this.columnIndex = firstColumnIndex - 1;
		}
		#region IOrderedEnumerator<ICell> Members
		public int CurrentValueOrder { get { return current.ColumnIndex; } }
		public bool IsReverseOrder { get { return false; } }
		#endregion
		#region IEnumerator<ICell> Members
		public ICell Current { get { return current; } }
		#endregion
		#region IDisposable Members
		public void Dispose() {
		}
		#endregion
		#region IEnumerator Members
		object IEnumerator.Current { get { return current; } }
		public bool MoveNext() {
			columnIndex++;
			if (columnIndex > lastColumnIndex)
				return false;
			this.current = new FakeCellWithRowFormatting(columnIndex, row);
			return true;
		}
		public void Reset() {
			this.columnIndex = firstColumnIndex - 1;
			this.current = null;
		}
		#endregion
		#region IShiftableEnumerator
		void IShiftableEnumerator.OnObjectInserted(int insertedItemValueOrder) {
		}
		void IShiftableEnumerator.OnObjectDeleted(int deletedItemValueOrder) {
		}
		#endregion
	}
	#endregion
	#region ExistingVisibleRowsEnumerator
	public class ExistingVisibleRowsEnumerator<T> : ExistingRowsEnumerator<T> where T : IRowBase {
		public ExistingVisibleRowsEnumerator(IList<T> rows, int nearItemIndex, int farItemIndex, bool reverseOrder, Predicate<T> filter)
			: base(rows, nearItemIndex, farItemIndex, reverseOrder, filter) {
		}
		protected override bool IsValidItem(int index) {
			return Items[index].IsVisible;
		}
	}
	#endregion
	#region ExistingNotVisibleRowsEnumerator
	public class ExistingNotVisibleRowsEnumerator<T> : ExistingRowsEnumerator<T> where T : IRowBase {
		public ExistingNotVisibleRowsEnumerator(IList<T> rows, int nearItemIndex, int farItemIndex, bool reverseOrder, Predicate<T> filter)
			: base(rows, nearItemIndex, farItemIndex, reverseOrder, filter) {
		}
		protected override bool IsValidItem(int index) {
			return !Items[index].IsVisible;
		}
	}
	#endregion
	#region RowComparable
#if BTREE
	internal class RowComparable<T> : IComparable<T> where T : IIndexedObject {
		readonly long index;
		public RowComparable(long index) {
			this.index = index;
		}
		public int CompareTo(T other) {
			return other.Index.CompareTo(index);
		}
	}
#else
	internal class RowComparable<T> : IComparable<T> where T : IRowBase {
		readonly int index;
		public RowComparable(int index) {
			this.index = index;
		}
		public int CompareTo(T other) {
			return other.Index - index;
		}
	}
#endif
	#endregion
	#region RowBatchUpdateHelper
	public class RowBatchUpdateHelper : MultiIndexBatchUpdateHelper {
		FormatBase cellFormat;
		RowHeightInfo heightInfo;
		RowInfo info;
		public RowBatchUpdateHelper(IBatchUpdateHandler handler)
			: base(handler) {
		}
		public FormatBase CellFormat {
			get { return cellFormat; }
			set { cellFormat = value; }
		}
		public RowHeightInfo HeightInfo { get { return heightInfo; } set { heightInfo = value; } }
		public RowInfo Info { get { return info; } set { info = value; } }
		public override void BeginUpdateDeferredChanges() {
			cellFormat.BeginUpdate();
		}
		public override void CancelUpdateDeferredChanges() {
			cellFormat.CancelUpdate();
		}
		public override void EndUpdateDeferredChanges() {
			cellFormat.EndUpdate();
		}
	}
	public class RowBatchInitHelper : FormatBaseBatchUpdateHelper {
		public RowBatchInitHelper(IBatchInitHandler handler)
			: base(new BatchInitAdapter(handler)) {
		}
		public IBatchInitHandler BatchInitHandler { get { return ((BatchInitAdapter)BatchUpdateHandler).BatchInitHandler; } }
	}
	#endregion
	#region RowHeightInfo
	[StructLayout(LayoutKind.Explicit)]
	public struct RowHeightInfo : ICloneable<RowHeightInfo>, ISupportsSizeOf {
		[FieldOffset(0)]
		float floatValue;
		[FieldOffset(0)]
		int intValue;
		public float FloatValue { get { return floatValue; } set { floatValue = value; } }
		public int IntValue { get { return intValue; } set { intValue = value; } }
		#region ISupportsSizeOf Members
		public int SizeOf() {
			return DXMarshal.SizeOf(typeof(RowHeightInfo));
		}
		#endregion
		#region ICloneable<RowHeightInfo> Members
		public RowHeightInfo Clone() {
			return this;
		}
		#endregion
	}
	#endregion
	#region RowInfo
	public struct RowInfo : ICloneable<RowInfo>, ISupportsSizeOf {
		#region Fields
		const int MaskOutlineLevel = 0x0000007;
		const int MaskIsHidden = 0x00000008;
		const int MaskIsCollapsed = 0x00000010;
		const int MaskIsCustomHeight = 0x00000020;
		const int MaskIsThickTopBorder = 0x00000040;
		const int MaskIsThickBottomBorder = 0x00000080;
		public const int DefaultValue = 0x0;
		int packedValues;
		#endregion
		#region Properties
		public int PackedValues { get { return packedValues; } set { this.packedValues = value; } }
		public int OutlineLevel {
			get {
				return (packedValues & MaskOutlineLevel);
			}
			set {
				packedValues &= ~MaskOutlineLevel;
				packedValues |= (value & MaskOutlineLevel);
			}
		}
		public bool IsHidden { get { return GetBooleanValue(MaskIsHidden); } set { SetBooleanValue(MaskIsHidden, value); } }
		public bool IsCollapsed { get { return GetBooleanValue(MaskIsCollapsed); } set { SetBooleanValue(MaskIsCollapsed, value); } }
		public bool IsCustomHeight { get { return GetBooleanValue(MaskIsCustomHeight); } set { SetBooleanValue(MaskIsCustomHeight, value); } }
		public bool IsThickTopBorder { get { return GetBooleanValue(MaskIsThickTopBorder); } set { SetBooleanValue(MaskIsThickTopBorder, value); } }
		public bool IsThickBottomBorder { get { return GetBooleanValue(MaskIsThickBottomBorder); } set { SetBooleanValue(MaskIsThickBottomBorder, value); } }
		#endregion
		#region GetBooleanValue/SetBooleanValue helpers
		void SetBooleanValue(int mask, bool bitVal) {
			if (bitVal)
				packedValues |= mask;
			else
				packedValues &= ~mask;
		}
		bool GetBooleanValue(int mask) {
			return (packedValues & mask) != 0;
		}
		#endregion
		#region ISupportsSizeOf Members
		public int SizeOf() {
			return DXMarshal.SizeOf(typeof(RowInfo));
		}
		#endregion
		#region ICloneable<RowInfo> Members
		public RowInfo Clone() {
			return this;
		}
		#endregion
	}
	#endregion
#if BTREE
	#region BalancedBPlusTree
	public interface IBalancedBPlusTreeNode<T> : IEnumerable<T> where T : IIndexedObject {
		long Index { get; }
		bool Add(T item);
		T this[int index] { get; }
		int ItemCount { get; }
		IBalancedBPlusTreeNode<T> Split();
		T First { get; }
		T Last { get; }
		T GetOrCreate(long index, Func<long, T> createItemAction, BalancedBPlusTreeContext<T> context);
		bool IsLeaf { get; }
		IBalancedBPlusTreeNode<T> GetByPosition(int index);
	}
	public class BalancedBPlusTreeContext<T> where T : IIndexedObject {
		public BalancedBPlusTreeContext(BalancedBPlusTreeLeaf<T> lastLeaf) {
			this.LastLeaf = lastLeaf;
			this.LastAvailableIndex = Int64.MaxValue;
		}
		public BalancedBPlusTreeLeaf<T> LastLeaf { get; set; }
		public long LastAvailableIndex { get; set; }
	}
	public class BalancedBPlusTree<T> : IEnumerable<T> where T : IIndexedObject {
		IBalancedBPlusTreeNode<T> header;
		int count;
		readonly BalancedBPlusTreeContext<T> context;
		public BalancedBPlusTree() {
			BalancedBPlusTreeLeaf<T> headerLeaf = new BalancedBPlusTreeLeaf<T>(0);
			this.header = headerLeaf;
			context = new BalancedBPlusTreeContext<T>(headerLeaf);
		}
		public void Add(T item) {
			if (!this.header.Add(item)) {
				SplitHeader();
				this.header.Add(item);
			}
			count++;
		}
		void SplitHeader() {
			IBalancedBPlusTreeNode<T> splittedItem = header.Split();
			List<IBalancedBPlusTreeNode<T>> items = new List<IBalancedBPlusTreeNode<T>>();
			items.Add(header);
			items.Add(splittedItem);
			BalancedBPlusTreeNode<T> newHeader = new BalancedBPlusTreeNode<T>(items);
			this.header = newHeader;
		}
		public T First { get { return header.First; } }
		public T Last { get { return header.Last; } }
		public int Count { get { return count; } }
		public T this[int index] { get { return header[index]; } }
		internal T GetOrCreate(long index, Func<long, T> createItemAction) {
			T result = TryToGetOrCreateUsingContext(index, createItemAction);
			if (result != null)
				return result;
			result = header.GetOrCreate(index, createItemAction, context);
			if (result == null) {
				SplitHeader();
				result = this.header.GetOrCreate(index, createItemAction, context);
			}
			return result;
		}
		T TryToGetOrCreateUsingContext(long index, Func<long, T> createItemAction) {
			if (index < context.LastLeaf.Index || index > context.LastAvailableIndex)
				return default(T);
			return context.LastLeaf.GetOrCreate(index, createItemAction, context);
		}
		#region IEnumerable<T> Members
		public IEnumerator<T> GetEnumerator() {
			return header.GetEnumerator();
		}
		IEnumerator IEnumerable.GetEnumerator() {
			return (IEnumerator<T>)this.GetEnumerator();
		}
		#endregion
	}
	public class BalancedBPlusTreeNode<T> : IBalancedBPlusTreeNode<T> where T : IIndexedObject {
		const int MaxItemsCount = 4096;
		readonly long index;
		readonly List<IBalancedBPlusTreeNode<T>> items;
		int lastCollectionIndex;
		long lastIndex;
		public BalancedBPlusTreeNode(long index) {
			this.index = index;
			this.items = new List<IBalancedBPlusTreeNode<T>>(MaxItemsCount);
			lastCollectionIndex = -1;
			lastIndex = Int64.MaxValue;
		}
		public BalancedBPlusTreeNode(List<IBalancedBPlusTreeNode<T>> items) {
			Debug.Assert(items.Count > 0);
			this.index = items[0].Index;
			this.items = items;
			lastCollectionIndex = -1;
			lastIndex = Int64.MaxValue;
		}
		public T First {
			get {
				return items[0].First;
			}
		}
		public T Last {
			get {
				return items[items.Count - 1].Last;
				;
			}
		}
		#region IBalancedBPlusTreeNode<T> Members
		public long Index { get { return index; } }
		public T this[int index] { get { return GetNode(index)[index]; } }
		public bool IsLeaf { get { return false; } }
		public int ItemCount { get { return items.Count; } }
		public bool Add(T item) {
			if (items.Count >= MaxItemsCount)
				return false;
			long itemIndex = GetItemIndex(item);
			int index = SearchNode(itemIndex);
			IBalancedBPlusTreeNode<T> node = items[index];
			lastIndex = node.Index;
			if (!node.Add(item)) {
				SplitChildNode(index, node);
				Add(item);
			}
			return true;
		}
		public IBalancedBPlusTreeNode<T> Split() {
			int halfPosition = items.Count / 2;
			int halfCount = items.Count - halfPosition;
			List<IBalancedBPlusTreeNode<T>> newItems = items.GetRange(halfPosition, halfCount);
			BalancedBPlusTreeNode<T> newNode = new BalancedBPlusTreeNode<T>(newItems);
			items.RemoveRange(halfPosition, halfCount);
			lastCollectionIndex = -1;
			lastIndex = Int64.MaxValue;
			return newNode;
		}
		public T GetOrCreate(long index, Func<long, T> createItemAction, BalancedBPlusTreeContext<T> context) {
			int nodeIndex = SearchNode(index);
			IBalancedBPlusTreeNode<T> node = items[nodeIndex];
			context.LastAvailableIndex = nodeIndex >= items.Count - 1 ? Int64.MaxValue : items[nodeIndex + 1].Index - 1;
			T result = node.GetOrCreate(index, createItemAction, context);
			lastIndex = node.Index;
			if (result == null) {
				SplitChildNode(nodeIndex, node);
				result = GetOrCreate(index, createItemAction, context);
			}
			return result;
		}
		#endregion
		void SplitChildNode(int index, IBalancedBPlusTreeNode<T> node) {
			IBalancedBPlusTreeNode<T> newNode = node.Split();
			items.Insert(index + 1, newNode);
			lastCollectionIndex = -1;
			lastIndex = -1;
		}
		long GetItemIndex(T item) {
			return item.Index;
		}
		int SearchNode(long index) {
			if (lastCollectionIndex > 0) {
				if (lastIndex < index) {
					if (items.Count == lastCollectionIndex + 1) 
						return lastCollectionIndex;
					if (items[lastCollectionIndex + 1].Index > index)
						return lastCollectionIndex;
				}
			}
			int result = Algorithms.BinarySearch<IBalancedBPlusTreeNode<T>>(items, new BalancedBPlusTreeNodeComparable<T>(index));
			if (result < 0) {
				result = ~result;
				if (result >= items.Count)
					result--;
			}
			lastIndex = index;
			lastCollectionIndex = result;
			lastIndex = -1;
			return result;
		}
		IBalancedBPlusTreeNode<T> GetNode(long index) {
			return items[this.SearchNode(index)];
		}
		public IBalancedBPlusTreeNode<T> GetByPosition(int index) {
			return items[index];
		}
		#region IEnumerable<T> Members
		public IEnumerator<T> GetEnumerator() {
			return new BPlusTreeEnumerator<T>(this);
		}
		IEnumerator IEnumerable.GetEnumerator() {
			return (IEnumerator<T>)this.GetEnumerator();
		}
		#endregion
	}
	public class BalancedBPlusTreeLeaf<T> : IBalancedBPlusTreeNode<T> where T : IIndexedObject {
		const int MaxItemsCount = 65535;
		readonly long index;
		readonly List<T> items;
		long lastIndex;
		int lastCollectionIndex;
		T lastItem;
		public BalancedBPlusTreeLeaf(long index) {
			this.index = index;
			this.items = new List<T>(MaxItemsCount);
			lastIndex = Int64.MaxValue;
		}
		public BalancedBPlusTreeLeaf(List<T> items) {
			Debug.Assert(items.Count > 0);
			this.items = items;
			this.index = GetItemIndex(items[0]);
			lastIndex = Int64.MaxValue;
		}
		#region IBalancedBPlusTreeNode<T> Members
		public List<T> Items { get { return items; } }
		public long Index { get { return index; } }
		public T First { get { return Items[0]; } }
		public T Last { get { return Items[Items.Count - 1]; } }
		public T this[int index] { get { return GetItem(index); } }
		public bool IsLeaf { get { return true; } }
		public int ItemCount { get { return Items.Count; } }
		bool IsFull() {
			return Items.Count >= MaxItemsCount;
		}
		public bool Add(T item) {
			if (!IsFull())
				return false;
			long itemIndex = GetItemIndex(item);
			int index = SearchNode(itemIndex);
			if (index > 0)
				throw new InvalidOperationException("An item already exists");
			index = ~index;
			Items.Insert(index, item);
			return true;
		}
		int SearchNode(long index) {
			return Algorithms.BinarySearch<T>(Items, new RowComparable<T>(index));
		}
		T GetItem(long index) {
			int itemIndex = SearchNode(index);
			if (itemIndex >= 0)
				return Items[itemIndex];
			return default(T);
		}
		long GetItemIndex(T item) {
			return item.Index;
		}
		public IBalancedBPlusTreeNode<T> Split() {
			int halfPosition = Items.Count / 2;
			int halfCount = Items.Count - halfPosition;
			List<T> newItems = Items.GetRange(halfPosition, halfCount);
			BalancedBPlusTreeLeaf<T> newNode = new BalancedBPlusTreeLeaf<T>(newItems);
			Items.RemoveRange(halfPosition, halfCount);
			lastIndex = Int64.MaxValue;
			return newNode;
		}
		public T GetOrCreate(long index, Func<long, T> createItemAction, BalancedBPlusTreeContext<T> context) {
			context.LastLeaf = this;
			if (index == lastIndex)
				return lastItem;
			int itemIndex;
			if (index > lastIndex) {
				if (lastCollectionIndex + 1 < items.Count) {
					lastItem = items[lastCollectionIndex];
					if (lastItem.Index == index) {
						lastCollectionIndex++;
					}
					else {
						itemIndex = SearchNode(index);
						if (itemIndex >= 0)
							lastItem = Items[itemIndex];
						else {
							if (IsFull()) 
								return default(T);
							itemIndex = ~itemIndex;
							lastItem = createItemAction(index);
							Items.Insert(itemIndex, lastItem);
						}
						lastCollectionIndex = itemIndex;
					}
				}
				else {
					if (IsFull()) 
						return default(T);
					lastItem = createItemAction(index);
					Items.Add(lastItem);
					lastCollectionIndex = items.Count - 1;
				}
				lastIndex = index;
				return lastItem;
			}
			itemIndex = SearchNode(index);
			if (itemIndex >= 0)
				lastItem = Items[itemIndex];
			else {
				if (IsFull()) 
					return default(T);
				itemIndex = ~itemIndex;
				lastItem = createItemAction(index);
				Items.Insert(itemIndex, lastItem);
			}
			lastIndex = index;
			lastCollectionIndex = itemIndex;
			return lastItem;
		}
		public IBalancedBPlusTreeNode<T> GetByPosition(int index) {
			throw new InvalidOperationException();
		}
		#endregion
		#region IEnumerable<T> Members
		public IEnumerator<T> GetEnumerator() {
			return Items.GetEnumerator();
		}
		IEnumerator IEnumerable.GetEnumerator() {
			return (IEnumerator<T>)this.GetEnumerator();
		}
		#endregion
	}
	#region BalancedBPlusTreeNodeComparable
	public class BalancedBPlusTreeNodeComparable<T> : IComparable<IBalancedBPlusTreeNode<T>> where T : IIndexedObject {
		readonly long index;
		public BalancedBPlusTreeNodeComparable(long index) {
			this.index = index;
		}
		public System.Int32 CompareTo(IBalancedBPlusTreeNode<T> other) {
			return other.Index.CompareTo(index);
		}
	}
	#endregion
	public class BalancedBPlusTreeNodePosition<T> where T : IIndexedObject {
		int index;
		IBalancedBPlusTreeNode<T> node;
		public BalancedBPlusTreeNodePosition(int index, IBalancedBPlusTreeNode<T> node) {
			this.index = index;
			this.node = node;
		}
		public int Index { get { return index; } set { index = value; } }
		public IBalancedBPlusTreeNode<T> Node { get { return node; } set { node = value; } }
	}
	public class BPlusTreeEnumerator<T> : IEnumerator<T> where T : IIndexedObject {
		Stack<BalancedBPlusTreeNodePosition<T>> stack;
		IEnumerator<T> currentEnumerator;
		public BPlusTreeEnumerator(BalancedBPlusTreeNode<T> node) {
			stack = new Stack<BalancedBPlusTreeNodePosition<T>>();
			DrillDown(node);
		}
		#region IEnumerator<T> Members
		public T Current { get { return currentEnumerator.Current; } }
		#endregion
		#region IEnumerator Members
		object IEnumerator.Current { get { return ((IEnumerator<T>)this).Current; } }
		public bool MoveNext() {
			if (!currentEnumerator.MoveNext()) {
				if (!FindNextLeaf())
					return false;
				currentEnumerator.MoveNext();
			}
			return true;
		}
		public void Reset() {
			stack = new Stack<BalancedBPlusTreeNodePosition<T>>();
			currentEnumerator = null;
		}
		bool FindNextLeaf() {
			BalancedBPlusTreeNodePosition<T> currentNodePosition = stack.Peek();
			while (stack.Count > 0 && currentNodePosition.Index >= currentNodePosition.Node.ItemCount - 1) {
				currentNodePosition = stack.Pop();
			}
			if (stack.Count <= 0)
				return false;
			currentNodePosition.Index++;
			DrillDown(currentNodePosition.Node.GetByPosition(currentNodePosition.Index));
			return true;
		}
		void DrillDown(IBalancedBPlusTreeNode<T> node) {
			while (!node.IsLeaf) {
				stack.Push(new BalancedBPlusTreeNodePosition<T>(0, node));
				node = node.GetByPosition(0);
			}
			currentEnumerator = node.GetEnumerator();
		}
		#endregion
		#region IDisposable Members
		public void Dispose() {
			stack = null;
			currentEnumerator = null;
		}
		#endregion
	}
	#endregion
	#region RowCollectionBtree
	public class RowCollectionBtree : IRowCollection {
		Worksheet sheet;
		BalancedBPlusTree<Row> tree;
		public RowCollectionBtree(Worksheet sheet) {
			this.sheet = sheet;
			this.tree = new BalancedBPlusTree<Row>();
		}
		#region Properties
		public Worksheet Sheet { get { return sheet; } }
		#endregion
		public void InsertRowsShiftDown(int rowIndex, int count) {
			throw new NotImplementedException();
		}
		public void InsertRowsShiftDownCore(int position, int count) {
			throw new NotImplementedException();
		}
		protected bool CanRemove(int position) {
			throw new NotImplementedException();
		}
		public void Insert(int index, Row item) {
			throw new InvalidOperationException();
		}
		public void RemoveRangeCore(int positionFrom, int innerCount, int count, bool removeExistingRows) {
			throw new NotImplementedException();
		}
		#region IRowCollection Members
		IEnumerable IRowCollectionBase.GetExistingRows(int topRow, int bottomRow, bool reverseOrder) {
			return GetExistingRows(topRow, bottomRow, reverseOrder);
		}
		IEnumerable IRowCollectionBase.GetExistingVisibleRows(int topRow, int bottomRow, bool reverseOrder) {
			return GetExistingVisibleRows(topRow, bottomRow, reverseOrder);
		}
		IRowBase IRowCollectionBase.TryGetRow(int index) {
			return this.TryGetRow(index);
		}
		IEnumerator IRowCollectionBase.GetExistingRowsEnumerator(int topRow, int bottomRow) {
			return GetExistingRowsEnumerator(topRow, bottomRow);
		}
		IEnumerator IRowCollectionBase.GetExistingRowsEnumerator(int topRow, int bottomRow, bool reverseOrder) {
			return GetExistingRowsEnumerator(topRow, bottomRow, reverseOrder);
		}
		#endregion
		protected void OffsetCellRowIndices(int startRowIndex, int offset) {
			throw new NotImplementedException();
		}
		public void Clear() {
			throw new NotImplementedException();
		}
		public IOrderedEnumerator<Row> GetAllRowsEnumerator(int topRow, int bottomRow, bool reverseOrder) {
			return new AllRowsEnumerator(Sheet, topRow, bottomRow, reverseOrder, null);
		}
		#region CheckForEmptyRows
		public void CheckForEmptyRows(int startIndex, int endIndex) {
			throw new NotImplementedException();
		}
		void CheckForEmptyRow(int rowIndex) {
			throw new NotImplementedException();
		}
		void DeleteRow(Row row, int position) {
			throw new NotImplementedException();
		}
		#endregion
		public Row GetRowForReading(int rowIndex) {
			if (!IndicesChecker.CheckIsRowIndexValid(rowIndex))
				Exceptions.ThrowArgumentException("rowIndex", rowIndex);
			Row result = TryGetRow(rowIndex);
			if (result == null)
				return new Row(rowIndex, Sheet);
			else
				return result;
		}
		public List<Row> GetRowsForReading(int topIndex, int bottomIndex) {
			List<Row> result = new List<Row>();
			for (int i = topIndex; i <= bottomIndex; i++)
				result.Add(GetRowForReading(i));
			return result;
		}
		protected void ResetTryGetRowIndexCache() {
		}
		#region IRowCollectionGeneric<Row> Members
		public bool Contains(int index) {
			throw new NotImplementedException();
		}
		public bool Contains(Row item) {
			throw new NotImplementedException();
		}
		public Row this[int index] { get { return GetRow(index); } }
		public IList<Row> InnerList {
			get { throw new NotImplementedException(); }
		}
		public Row First { get { return tree.First; } }
		public Row Last { get { return tree.Last; } }
		public int Count { get { return tree.Count; } }
		public void ForEach(Action<Row> action) {
			throw new NotImplementedException();
		}
		public Row GetRow(int index) {
			return tree.GetOrCreate(index, CreateRow);
		}
		public Row TryGetRow(int index) {
			return tree[index];
		}
		public Row CreateRow(int index) {
			return Sheet.CreateRowCore(index);
		}
		public Row CreateRow(long index) {
			return Sheet.CreateRowCore((int)index);
		}
		public void SetCachedData(int cachedRowIndex, int cachedRowCollectionIndex) {
			throw new NotImplementedException();
		}
		public IEnumerable<Row> GetExistingRows() {
			throw new NotImplementedException();
		}
		public IEnumerable<Row> GetExistingRows(int topRow, int bottomRow, bool reverseOrder) {
			throw new NotImplementedException();
		}
		public IEnumerator<Row> GetExistingRowsEnumerator(int topRow, int bottomRow) {
			throw new NotImplementedException();
		}
		public IOrderedItemsRangeEnumerator<Row> GetExistingRowsEnumerator(int topRow, int bottomRow, bool reverseOrder) {
			throw new NotImplementedException();
		}
		public IOrderedItemsRangeEnumerator<Row> GetExistingRowsEnumerator(int topRow, int bottomRow, bool reverseOrder, Predicate<Row> filter) {
			throw new NotImplementedException();
		}
		public IEnumerable<Row> GetExistingVisibleRows(int topRow, int bottomRow, bool reverseOrder) {
			throw new NotImplementedException();
		}
		public IOrderedEnumerator<Row> GetExistingVisibleRowsEnumerator(int topRow, int bottomRow, bool reverseOrder) {
			throw new NotImplementedException();
		}
		public IOrderedEnumerator<Row> GetExistingNotVisibleRowsEnumerator(int topRow, int bottomRow, bool reverseOrder) {
			throw new NotImplementedException();
		}
		#region Remove
		public void RemoveRange(int from, int count) {
			throw new NotImplementedException();
		}
		public void RemoveRangeInner(int index, int innerCount, int count, bool removeRange) {
			throw new NotImplementedException();
		}
		public void RemoveCore(int index) {
			throw new NotImplementedException();
		}
		public void RemoveRangeCore(int index, int count) {
			throw new NotImplementedException();
		}
		#endregion
		public void InsertCore(int index, Row item) {
			throw new InvalidOperationException();
		}
		public void InsertRangeFromHistory(int index, IList<Row> items, int offsetCount) {
			throw new NotImplementedException();
		}
		public List<Row> GetInnerRange(int position, int innerCount) {
			throw new NotImplementedException();
		}
		#endregion
		#region IRowCollectionBase Members
		public int TryGetRowIndex(int modelIndex) {
			throw new NotImplementedException();
		}
		public int LastRowIndex { get { return Last.Index; } }
		#endregion
		#region IEnumerable Members
		public IEnumerator GetEnumerator() {
			return tree.GetEnumerator();
		}
		#endregion
		#region IEnumerable<Row> Members
		IEnumerator<Row> IEnumerable<Row>.GetEnumerator() {
			return tree.GetEnumerator();
		}
		#endregion
	}
	#endregion
	#region CellTree
	public class CellTree : BalancedBPlusTree<ICell> {
	}
	#endregion
#endif
}
