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
using System.Linq;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Drawing;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraVerticalGrid.Events;
using DevExpress.XtraVerticalGrid.Painters;
using DevExpress.XtraVerticalGrid.Rows;
using DevExpress.XtraVerticalGrid.ViewInfo;
using DevExpress.XtraVerticalGrid.Utils;
using DevExpress.Utils;
using DevExpress.Utils.Serializing;
using DevExpress.Utils.Drawing;
using DevExpress.Utils.Design;
using System.Collections.Generic;
namespace DevExpress.XtraVerticalGrid {
	public enum PropertySort {
		NoSort,
		Alphabetical
	}
	public enum SummaryItemType { None, Sum, Min, Max, Count, Average, Custom }
	public enum LayoutViewStyle { BandsView, SingleRecordView, MultiRecordView }
	public enum TreeButtonType { TreeViewButton, ExplorerBarButton }
	public enum TreeButtonStyle { Default, TreeView, ExplorerBar }
	public enum HitInfoTypeEnum { None, Empty, Row, HeaderCell, MultiEditorCellSeparator, ExpandButton, HeaderCellImage, HeaderCellSortShape, HeaderCellFilterButton, ValueCell, Border, CustomizationForm,
		OutLeftSide, OutRightSide, OutTopSide, OutBottomSide, HeaderSeparator, RowEdge, RecordValueEdge, BandEdge,
		HorzScrollBar, VertScrollBar, GripPlace }
	public enum RowChangeTypeEnum { Add, Delete, Move, RowAssigned, Index, Expanded, Options, StyleName, Visible, Height, MaxCaptionLineCount,
		Tag, Value, FormatType, Format, Caption, CustomizationCaption, FieldName, SummaryFooter, SummaryFooterStrFormat,
		SeparatorString, SeparatorKind, ImageIndex, RowEdit, Width, CellWidth, FormatString, SortOrder, Name, ReadOnly, Enabled, TabStop,
		PropertiesAdded, PropertiesDeleted, PropertiesReplaced, PropertiesCleared, PropertiesAssigned,
		UserProperty1, UserProperty2, UserProperty3, UserProperty4, UserProperty5, UserProperty6, UserProperty7, UserProperty8, UserProperty9, UserProperty10, 
		UnboundType, Fixed, UnboundExpression, ShowUnboundExpressionMenu, ToolTip, AllowHtmlText
	}
	public enum PGridScrollVisibility { Never, Auto, Vertical } 
	public enum ScrollVisibility { Never, Auto, Both, Horizontal, Vertical } 
	public enum RowDragSource { Control, CustomizationForm }
	public enum ShowButtonModeEnum { ShowAlways, ShowForFocusedRow,	ShowForFocusedCell,	ShowForFocusedRecord, ShowOnlyInEditor }
	public enum VGridState { Regular, Editing, FocusedRowChanging, RowDragging, RowSizing, MultiEditorRowCellSizing, HeaderPanelSizing, RecordSizing, Disposed }
	public enum HScrollNeeding { Yes, No, CanBe }
	public enum VScrollNeeding { Yes, No, CanBe }
	public class GridRowReadOnlyCollection : ReadOnlyCollectionBase {
		public static GridRowReadOnlyCollection Empty;
		Dictionary<BaseRow, int> indexCache = new Dictionary<BaseRow,int>();
		static GridRowReadOnlyCollection() {
			Empty = new GridRowReadOnlyCollection();
		}
		public GridRowReadOnlyCollection() { }
		protected internal GridRowReadOnlyCollection(ICollection collection) {
			AddRange(collection);
		}
		public BaseRow this[int index] {
			get {
				if(index < 0 || index > Count - 1) return null;
				return InnerList[index] as BaseRow;
			}
		}
		public BaseRow this[string fieldName] {
			get {
				foreach(BaseRow row in InnerList) {
					if(row.Properties.FieldName == fieldName) return row;
				}
				return null;
			}
		}
		public virtual int IndexOf(BaseRow row) {
			if(row == null) return -1;
			UpdateIndexes();
			int index;
			if (!indexCache.TryGetValue(row, out index))
				return -1;
			return index;
		}
		public virtual bool Contains(BaseRow row) {
			UpdateIndexes();
			return IndexOf(row) != -1;
		} 
		protected internal virtual int Add(BaseRow row) { return InnerList.Add(row); }
		protected internal virtual void Clear() { InnerList.Clear(); }
		protected internal virtual void AddRange(ICollection collection) {
			InnerList.AddRange(collection);
		}
		internal void Sort(IComparer comparer) {
			InnerList.Sort(comparer);
		}
		void UpdateIndexes() {
			if (indexCache.Keys.Count == 0) {
				for (int i = 0; i < Count; i++) {
					indexCache[this[i]] = i;
				}
			}
		}
	}
	public class GridCell {
		public GridCell(BaseRow row, int cell, int record) {
			Row = row;
			Cell = cell;
			Record = record;
		}
		public BaseRow Row;
		public int Cell;
		public int Record;
		public override int GetHashCode() {
			return (Row != null ? Row.GetHashCode() : 0) ^ Cell.GetHashCode() ^ Record.GetHashCode();
		}
		public override bool Equals(object obj) {
			GridCell gridCell = obj as GridCell;
			if(gridCell == null)
				return false;
			return object.Equals(Row, gridCell.Row) && Cell == gridCell.Cell && Record == gridCell.Record;
		}
	}
	public class VGridHitInfo {
		public Point PtMouse;
		public HitInfoTypeEnum HitInfoType;
		public BaseRow Row;
		public int BandIndex, RecordIndex, CellIndex;
		public VGridHitInfo() {
			PtMouse = Point.Empty;
			HitInfoType = HitInfoTypeEnum.None;
			Row = null;
			BandIndex = RecordIndex = CellIndex = -1;
		}
	}
	public class DragInfo {
		private Timer scrollTimer;
		internal int cx, cy;
		internal DragInfo() {
			this.scrollTimer = new Timer();
			this.scrollTimer.Interval = 30;
			this.cx = this.cy = 0;
		}
		internal void Go() {
			if(!scrollTimer.Enabled) {
				cx = cy = 0;
				scrollTimer.Enabled = true;
			}
		}
		internal void Stop() {
			cx = cy = 0;
			scrollTimer.Enabled = false;
		}
		internal event EventHandler DragScroll {
			add { scrollTimer.Tick += new EventHandler(value); }
			remove { scrollTimer.Tick -= new EventHandler(value); } 
		}
	}
	internal class VGridScrollBehavior {
		private ScrollVisibility scrollVisibility;
		internal VGridScrollBehavior() {
			this.scrollVisibility = ScrollVisibility.Auto;
		}
		public ScrollVisibility ScrollVisibility {
			get { return scrollVisibility; }
			set { scrollVisibility = value; }
		}
		public HScrollNeeding HScrollNeeding {
			get {
				if(ScrollVisibility == ScrollVisibility.Never ||
					ScrollVisibility == ScrollVisibility.Vertical) return HScrollNeeding.No;
				if(ScrollVisibility == ScrollVisibility.Both) return HScrollNeeding.Yes;
				return HScrollNeeding.CanBe;
			}
		}
		public VScrollNeeding VScrollNeeding {
			get {
				if(ScrollVisibility == ScrollVisibility.Never ||
					ScrollVisibility == ScrollVisibility.Horizontal) return VScrollNeeding.No;
				if(ScrollVisibility == ScrollVisibility.Both) return VScrollNeeding.Yes;
				return VScrollNeeding.CanBe;
			}
		}
		public bool CanChangeCurrentRecord(LayoutViewStyle layoutStyle) {
			if(HScrollNeeding != HScrollNeeding.No) return true;
			return layoutStyle == LayoutViewStyle.MultiRecordView; 
		}
	}
	public class BandInfo {
		internal int bandIndex, rowsCount, bandHeight;
		internal BaseRow firstRow;
		internal BandInfo() : this(-1, 0, 0, null) {
		}
		internal BandInfo(int bandIndex, int rowsCount, int bandHeight, BaseRow firstRow) {
			this.bandIndex = bandIndex;
			this.rowsCount = rowsCount;
			this.bandHeight = bandHeight;
			this.firstRow = firstRow;
		}
	}
	internal class MouseHover {
		private VGridControlBase grid;
		private RowValueInfo rv;
		private EditHitTest objectType;
		private EditorButton button;
		internal MouseHover(VGridControlBase grid) {
			this.grid = grid;
			this.rv = null;
			this.objectType = EditHitTest.None;
			this.button = null;
		}
		internal void CheckMouseHotTrack(VGridHitTest ht) {
			CheckCellHotTrack(ht);
		}
		private void CheckCellHotTrack(VGridHitTest ht) {
			if(grid.ShowButtonMode == ShowButtonModeEnum.ShowOnlyInEditor) return;
			RowValueInfo newCell = null;
			EditorButton newButton = null;
			EditHitTest newObjectType = EditHitTest.None;
			Point newPt = new Point(-1, -1);
			if(ht != null && ht.HitInfoType == HitInfoTypeEnum.ValueCell && ht.ValueInfo.EditorViewInfo.Bounds.Contains(ht.PtMouse)) {
				newCell = ht.ValueInfo;
				newPt = ht.PtMouse;
				EditHitInfo editHI = newCell.EditorViewInfo.CalcHitInfo(newPt);
				newObjectType = editHI.HitTest;
				if(newObjectType != EditHitTest.Button) {
					newObjectType = EditHitTest.None;
					newButton = null;
				}
				else {
					EditorButtonObjectInfoArgs buttonInfo = editHI.HitObject as EditorButtonObjectInfoArgs;
					if(buttonInfo == null)
						newButton = editHI.HitObject as EditorButton;
					else
						newButton = buttonInfo.Button;
				}
			}
			if(rv != newCell) {
				UpdateCell(rv, newPt);
				UpdateCell(newCell, newPt);
			} else {
				if(objectType != newObjectType || button != newButton)
					UpdateCell(newCell, newPt);
			}
			rv = newCell;
			objectType = newObjectType;
			button = newButton;
		}
		void UpdateCell(RowValueInfo cell, Point pt) {
			if(cell == null || cell.EditorViewInfo == null || !cell.EditorViewInfo.IsRequiredUpdateOnMouseMove)
				return;
			cell.RecalcViewInfo(pt);
			grid.Invalidate(cell.EditorViewInfo.Bounds);
		}
	}
	public class PressInfo {
		BaseRow pressedRow;
		bool isEditing;
		public PressInfo() { Clear(); }
		public void Clear() {
			this.pressedRow = null;
			this.isEditing = false;
		}
		public BaseRow PressedRow { get { return pressedRow; } set { pressedRow = value; } }
		public bool IsEditing { get { return isEditing; } set { isEditing = value; } }
	}
	public class NavigationInfo {
		BaseRow row;
		int recordIndex, cellIndex;
		public NavigationInfo() : this(null, -1, -1) {}
		public NavigationInfo(BaseRow row, int recordIndex, int cellIndex) {
			this.row = row;
			this.recordIndex = recordIndex;
			this.cellIndex = cellIndex;
		}
		public BaseRow Row { get { return row; } set { row = value; } }
		public int RecordIndex { get { return recordIndex; } set { recordIndex = value; } }
		public int CellIndex { get { return cellIndex; } set { cellIndex = value; } }
	}
	public interface IScrollStyleChangeListener {
		void OnScrollStyleChanged(VGridScrollStylesController styleController);
	}
	public class VGridScrollStylesController {
		Color foreColor, backColor;
		DevExpress.XtraEditors.Controls.BorderStyles borderStyle;
		ArrayList listeners;
		readonly Color defBackColor = SystemColors.Control;
		readonly Color defForeColor = SystemColors.WindowFrame;
		const DevExpress.XtraEditors.Controls.BorderStyles defBorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.Flat;
		public VGridScrollStylesController() {
			this.backColor = defBackColor;
			this.foreColor = defForeColor;
			this.borderStyle = defBorderStyle;
			this.listeners = new ArrayList();
		}
		protected virtual bool ShouldSerializeBackColor() { return BackColor != defBackColor; }
		[XtraSerializableProperty()]
		public Color BackColor {
			get { return backColor; }
			set {
				if(BackColor != value) {
					backColor = value;
					Changed();
				}
			}
		}
		protected virtual bool ShouldSerializeForeColor() { return ForeColor != defForeColor; }
		[XtraSerializableProperty()]
		public Color ForeColor {
			get { return foreColor; }
			set {
				if(ForeColor != value) {
					foreColor = value;
					Changed();
				}
			}
		}
		protected virtual bool ShouldSerializeBorderStyle() { return BorderStyle != defBorderStyle; }
		[XtraSerializableProperty()]
		public DevExpress.XtraEditors.Controls.BorderStyles BorderStyle {
			get { return borderStyle; }
			set {
				if(BorderStyle != value) {
					borderStyle = value;
					Changed();
				}
			}
		}
		public virtual void AddListener(IScrollStyleChangeListener listener) {
			if(listener != null && listeners.IndexOf(listener) == -1) {
				listeners.Add(listener);
				listener.OnScrollStyleChanged(this);
			}
		}
		public virtual void RemoveListener(IScrollStyleChangeListener listener) {
			if(listener != null && listeners.IndexOf(listener) != -1)
				listeners.Remove(listener);
		}
		protected virtual void Changed() {
			for(int i = 0; i < listeners.Count; i++) {
				((IScrollStyleChangeListener)listeners[i]).OnScrollStyleChanged(this);
			}
		}
	}
}
namespace DevExpress.XtraVerticalGrid.Rows {
	public interface IRowChangeListener {
		void RowPropertiesChanged(RowProperties prop, RowChangeTypeEnum changeType);
		void RowPropertiesChanging(RowProperties prop, RowPropertiesChangingArgs args);
		BaseRow Row { get; }
		bool IsBlocked { get; set; }
	}
	public interface IRowViewScaler {
		int GetCellViewRectWidth(RowProperties props, int commonWidth, bool calcValue);
	}
	public class RowInfo {
		public BaseRow Row { get; set; }
		public string ParentRowName { get; set; }
	}
	internal class RowXtraDeserializer {
		static List<RowInfo> rows = null;
		static List<RowProperties> rowProperties = null;
		static internal void StartDeserializing(VGridRows gridRows) {
			VGridRowsIterator iterator = new VGridRowsIterator(gridRows.Grid, false);
			CreateItemsPoolOperation operation = new CreateItemsPoolOperation();
			iterator.DoLocalOperation(operation, gridRows);
			rowProperties = operation.RowProperties;
			rows = operation.Rows;
			rows.ForEach(info => info.Row.ChildRows.ClearRows(false));
		}
		static internal void EndDeserializing(VGridControlBase grid) {
			AddNewRows(grid);
			DisposeRows();
			DisposeRowProperties();
		}
		static void AddNewRows(VGridControlBase grid) {
			if (grid == null || !grid.OptionsLayout.Columns.AddNewColumns)
				return;
			while (rows.Count != 0) {
				RowInfo rowInfo = rows[0];
				rows.RemoveAt(0);
				BaseRow parentRow = grid.GetRowByName(rowInfo.ParentRowName);
				if (parentRow == null)
					grid.Rows.Add(rowInfo.Row);
				else
					parentRow.ChildRows.Insert(rowInfo.Row, rowInfo.Row.Index);
			}
		}
		static void DisposeRowProperties() {
			if (rowProperties == null)
				return;
			for (int i = 0; i < rowProperties.Count; i++) {
				RowProperties row = rowProperties[i];
				if (row.Row == null)
					(row as IDisposable).Dispose();
			}
			rowProperties.Clear();
			rowProperties = null;
		}
		private static void DisposeRows() {
			if (rows == null)
				return;
			for (int i = 0; i < rows.Count; i++) {
				BaseRow row = rows[i].Row;
				if (row.Rows == null)
					(row as IDisposable).Dispose();
			}
			rows.Clear();
			rows = null;
		}
		static internal void ClearRows(XtraItemEventArgs e) {
			VGridRows gridRows = e.Collection as VGridRows;
			if(gridRows != null)
				gridRows.Clear();
		}
		static internal object CreateRowsItem(XtraItemEventArgs e, VGridControlBase vGrid) {
			if (vGrid.OptionsLayout.Columns.RemoveOldColumns)
				return null;
			string typeID = e.Item.ChildProperties["XtraRowTypeID"].Value.ToString();
			return vGrid.CreateRow(Convert.ToInt32(typeID));
		}
		internal static RowProperties FindRowProperties(XtraItemEventArgs e) {
			if (rowProperties == null || e.Item.ChildProperties == null)
				return null;
			string itemCaption = e.Item.ChildProperties["Caption"].Value as string;
			string itemFieldName = e.Item.ChildProperties["FieldName"].Value as string;
			for(int i = 0; i < rowProperties.Count; i++) {
				RowProperties current = rowProperties[i];
				if (current.Caption == itemCaption && current.FieldName == itemFieldName) {
					rowProperties.RemoveAt(i);
					return current;
				}
			}
			return null;
		}
		static internal object FindRowsItem(XtraItemEventArgs e) {
			if(rows == null) return null;
			if(e.Item.ChildProperties == null) return null;
			string name = null;
			DevExpress.Utils.Serializing.Helpers.XtraPropertyInfo xp = e.Item.ChildProperties["Name"];
			if(xp != null && xp.Value != null)
				name = xp.Value.ToString();
			if(name == null || name == string.Empty) return null;
			for (int i = 0; i < rows.Count; i++) {
				BaseRow result = rows[i].Row;
				if (result.Name == name) {
					rows.RemoveAt(i);
					return result;
				}
			}
			return null;
		}
		static internal void SetItemIndex(XtraSetItemIndexEventArgs e) {
			VGridRows gridRows = e.Collection as VGridRows;
			if(gridRows == null) return;
			BaseRow row = e.Item.Value as BaseRow;
			if(row == null) return;
			gridRows.Add(row);
		}
		internal static void SetIndexPropertiesCollectionItem(XtraSetItemIndexEventArgs e) {
			MultiEditorRowPropertiesCollection collection = e.Collection as MultiEditorRowPropertiesCollection;
			if (collection == null)
				return;
			MultiEditorRowProperties item = e.Item.Value as MultiEditorRowProperties;
			if (item == null)
				return;
			collection.Add(item);
		}
	}
	internal class RowSizeInfo {
		internal int oldSize, oldSize2, cellIndex;
		internal float scaleCoeff;
		internal BaseRow row;
		internal RowSizeInfo() {
			Reset();
		}
		internal void Reset() {
			oldSize = oldSize2 = 0;
			scaleCoeff = 1.0f;
			cellIndex = -1;
			row = null;
		}
		internal MultiEditorRow editorRow { get { return (MultiEditorRow)row; } }
	}
	public class RowChangeArgs {
		object val, oldVal;
		RowChangeTypeEnum changeType;
		RowProperties prop;
		public RowChangeArgs(RowChangeTypeEnum changeType, object value, object oldValue) {
			this.val = value;
			this.oldVal = oldValue;
			this.changeType = changeType;
			this.prop = null;
		}
		public object Value { get { return val; } set { val = value; } }
		public object OldValue { get { return   oldVal; } }
		public RowChangeTypeEnum ChangeType { get { return  changeType; } }
		public RowProperties Prop { get { return  prop; } set { prop = value; } }
										 }
	public class RowPropertiesChangingArgs : RowChangeArgs {
		bool canChange;
		public RowPropertiesChangingArgs(RowChangeTypeEnum changeType, object _value, object _oldValue) :
			base(changeType, _value, _oldValue) {
			this.canChange = true;
		}
		public bool CanChange { get { return canChange; } set { canChange = value; } }
	}
	public class RowTypeIdConsts {
		public const int CategoryRowTypeId = 0;
		public const int EditorRowTypeId  =  1;
		public const int MultiEditorRowTypeId = 2;
	}
}
namespace DevExpress.XtraVerticalGrid.ViewInfo {
	public class GridStyles {
		public static readonly string[] DefaultNames  = new string[] { "RowHeaderPanel", "PressedRow", "HideSelectionRow", "Category", "HorzLine", "VertLine", 
			"RecordValue", "BandBorder", "FocusedRow", "FocusedRecord", "FocusedCell",
			"ExpandButton", "CategoryExpandButton", "Empty", "DisabledRecordValue", "DisabledRow",
			"ReadOnlyRecordValue", "ReadOnlyRow", "ModifiedRecordValue", "ModifiedRow", "FixedLine"
			 };
		public static string RowHeaderPanel { get { return DefaultNames[RowHeaderPanelId]; } }
		public static string PressedRow { get { return DefaultNames[PressedRowId]; } }
		public static string HideSelectionRow { get { return DefaultNames[HideSelectionRowId]; } }
		public static string Category { get { return DefaultNames[CategoryId]; } }
		public static string HorzLine { get { return DefaultNames[HorzLineId]; } }
		public static string VertLine { get { return DefaultNames[VertLineId]; } }
		public static string RecordValue { get { return DefaultNames[RecordValueId]; } }
		public static string BandBorder { get { return DefaultNames[BandBorderId]; } }
		public static string FocusedRow { get { return DefaultNames[FocusedRowId]; } }
		public static string FocusedRecord { get { return DefaultNames[FocusedRecordId]; } }
		public static string FocusedCell { get { return DefaultNames[FocusedCellId]; } }
		public static string ExpandButton { get { return DefaultNames[ExpandButtonId]; } }
		public static string CategoryExpandButton { get { return DefaultNames[CategoryExpandButtonId]; } }
		public static string Empty { get { return DefaultNames[EmptyId]; } }
		public static string DisabledRecordValue { get { return DefaultNames[DisabledRecordValueId]; } }
		public static string DisabledRow { get { return DefaultNames[DisabledRowId]; } }
		public static string ReadOnlyRecordValue { get { return DefaultNames[ReadOnlyRecordValueId]; } }
		public static string ReadOnlyRow { get { return DefaultNames[ReadOnlyRowId]; } }
		public static string ModifiedRecordValue { get { return DefaultNames[ModifiedRecordValueId]; } }
		public static string ModifiedRow { get { return DefaultNames[ModifiedRowId]; } }
		public static string FixedLine { get { return DefaultNames[FixedLineId]; } }
		public const int RowHeaderPanelId = 0,
			PressedRowId = 1,
			HideSelectionRowId = 2,
			CategoryId = 3,
			HorzLineId = 4,
			VertLineId = 5,
			RecordValueId = 6,
			BandBorderId = 7,
			FocusedRowId = 8,
			FocusedRecordId = 9,
			FocusedCellId = 10,
			ExpandButtonId = 11,
			CategoryExpandButtonId = 12,
			EmptyId = 13,
			DisabledRecordValueId = 14,
			DisabledRowId = 15,
			ReadOnlyRecordValueId = 16,
			ReadOnlyRowId = 17,
			ModifiedRecordValueId = 18,
			ModifiedRowId = 19,
			FixedLineId = 20;
		public static readonly int[] Styles  = new int[] { RowHeaderPanelId, PressedRowId, HideSelectionRowId, CategoryId, HorzLineId, VertLineId,
			RecordValueId, BandBorderId, FocusedRowId, FocusedRecordId, FocusedCellId,
			ExpandButtonId, CategoryExpandButtonId, EmptyId,
			DisabledRecordValueId, DisabledRowId, ReadOnlyRecordValueId, ReadOnlyRowId, ModifiedRecordValueId, ModifiedRowId,
			FixedLineId
															  };
	}
	public class LineInfo {
		public Rectangle Rect;
		public Brush Brush;
		public LineInfo(int x, int y, int width, int height, Brush brush) {
			Rect = new Rectangle(x, y, width, height);
			Brush = brush;
		}
		public void Draw(GraphicsCache cache) {
			cache.Graphics.FillRectangle(Brush, Rect);
		}
	}
	public class Lines : ArrayList {
		public bool lockAddLines;
		public Lines() { lockAddLines = false; }
		public new LineInfo this[int index] { get { return (LineInfo)base[index]; } }
		public void AddLine(LineInfo li) {
			Add(li);
		}
		public override int Add(object value) {
			if(!lockAddLines) return base.Add(value);
			return -1;
		}
	}
	public class Indents : ArrayList {
		public new IndentInfo this[int index] { get { return (IndentInfo)base[index]; } }
	}
	public class SeparatorInfo {
		int headerSeparatorWidth,
			valueSeparatorWidth;
		internal string SeparatorString;
		internal SeparatorKind SeparatorKind;
		internal StringFormat SeparatorFormat;
		internal MultiEditorRow Row;
		public SeparatorInfo(MultiEditorRow row) {
			Row = row;
			headerSeparatorWidth = 0;
			valueSeparatorWidth = 0;
			SeparatorString = Row.SeparatorString;
			SeparatorKind = Row.SeparatorKind;
			SeparatorFormat = new StringFormat();
			SeparatorFormat.Alignment = StringAlignment.Center;
			SeparatorFormat.LineAlignment = StringAlignment.Center;
		}
		internal int HeaderSeparatorWidth {
			get { return headerSeparatorWidth; }
			set { headerSeparatorWidth = value; }
		}
		internal int ValueSeparatorWidth {
			get { return valueSeparatorWidth; }
			set { valueSeparatorWidth = value; }
		}
	}
	public class VGridHitTest {
		internal Point PtMouse;
		internal HitInfoTypeEnum HitInfoType;
		BaseRowViewInfo rowInfo;
		internal BaseRowHeaderInfo CustomizationHeaderInfo;
		internal RowCaptionInfo CaptionInfo;
		internal RowValueInfo ValueInfo;
		internal VGridHitTest() {
			Clear();
		}
		public void Clear() {
			PtMouse = Point.Empty;
			HitInfoType = HitInfoTypeEnum.None;
			this.rowInfo = null;
			CustomizationHeaderInfo = null;
			CaptionInfo = null;
			ValueInfo = null;
		}
		internal BaseRowViewInfo RowInfo {
			get { return rowInfo; }
			set { rowInfo = value; }
		}
		internal VGridHitInfo ToHitInfo() {
			VGridHitInfo hi = new VGridHitInfo();
			hi.PtMouse = PtMouse;
			hi.HitInfoType = HitInfoType;
			if(RowInfo != null) { 
				hi.Row = RowInfo.Row;
				hi.BandIndex = RowInfo.BandIndex;
			}
			if(CustomizationHeaderInfo != null) {
				hi.Row = CustomizationHeaderInfo.Row;
			}
			if(CaptionInfo != null) hi.CellIndex = CaptionInfo.RowCellIndex;
			if(ValueInfo != null) {
				hi.RecordIndex = ValueInfo.RecordIndex;
				hi.CellIndex = ValueInfo.RowCellIndex;
			}
			return hi;
		}
		public BaseRow Row {
			get {
				if(RowInfo != null) return RowInfo.Row;
				if(ValueInfo != null) return ValueInfo.Row;
				if(CustomizationHeaderInfo != null) return CustomizationHeaderInfo.Row;
				return null;
			}
		}
	}
	public class RowValueCollection : CollectionBase {
		BaseRowViewInfo rowViewInfo;
		public RowValueCollection(BaseRowViewInfo rowViewInfo) {
			this.rowViewInfo = rowViewInfo;
		}
		public RowValueInfo this[int index] { get { return List[index] as RowValueInfo;} set { List[index] = value; } }
		public int Add(RowValueInfo rv) { return List.Add(rv); }
		public void Insert(int index, RowValueInfo value) { List.Insert(index, value); }
		public int IndexOf(RowValueInfo value) { return List.IndexOf(value); }
		protected override void OnInsertComplete(int index, object value) {
			SetOwner(value, rowViewInfo);
		}
		protected override void OnRemoveComplete(int index, object value) {
			SetOwner(value, null);
		}
		protected override void OnSetComplete(int index, object oldValue, object newValue) {
			SetOwner(oldValue, null);
			SetOwner(newValue, rowViewInfo);
		}
		protected override void OnClear() {
			foreach(RowValueInfo rv in this)
				SetOwner(rv, null);
		}
		private void SetOwner(object rowValue, BaseRowViewInfo owner) {
			(rowValue as RowValueInfo).RowViewInfo = owner;
		}
	}
	internal class IndentRectInfo {
		internal Size size;
		internal bool underline, isCategory;
		internal AppearanceObject style;
		internal IndentRectInfo(Size size, bool isCategory, bool underline, AppearanceObject style) {
			this.size = size;
			this.isCategory = isCategory;
			this.underline = underline;
			this.style = style;
		}
	}
	public class IndentInfo {
		Rectangle bounds;
		AppearanceObject style;
		int horzLinesSepPos;
		public IndentInfo(Rectangle bounds, int horzLinesSepPos, AppearanceObject style) {
			this.bounds = bounds;
			this.style = style;
			this.horzLinesSepPos = horzLinesSepPos;
		}
		public Rectangle Bounds { get { return bounds; } set { bounds = value; } }
		public int HorzLinesSepPos { get { return horzLinesSepPos; } }
		public AppearanceObject Style {
			get { return style; }
			set {
				if(value != null)
					style = value;
			}
		}
	}
	public abstract class PaintStyleCalcHelper {
		protected BaseViewInfo vi;
		protected VGridControlBase Grid { get { return vi.Grid; } }
		public PaintStyleCalcHelper(BaseViewInfo vi) { this.vi = vi; }
		public abstract Rectangle ChangeFocusRow(BaseRow newFocus, BaseRow oldFocus);
		public abstract Rectangle GetCategoryFocusRect(CategoryRowHeaderInfo rh);
		public abstract Rectangle GetComplexFillRect(BaseRowHeaderInfo rh);
		public abstract void AddHeaderIndentLines(BaseRowHeaderInfo rh,  IndentInfo val, bool toCategories, bool underline, bool addVertLine);
		public abstract void CalcPaintStyleLines(BaseRowViewInfo ri, BaseRow nextRow);
		public abstract void AddBoundHeaderLines(BaseRowHeaderInfo rh, BaseRow nextRow);
		public abstract AppearanceObject GetIndentStyle(BaseRowHeaderInfo rh, BaseRow parentIndentRow);
		public virtual int GetRowHeaderViewPoint(BaseRowHeaderInfo rh) { return rh.HeaderRect.Left; }
		public virtual int GetRowHeaderViewEndPoint(BaseRowHeaderInfo rh) { return rh.HeaderRect.Right; }
		public virtual int GetCategoryHeaderViewPoint(CategoryRowHeaderInfo rh) { return rh.HeaderRect.Left; }
		public virtual Brush GetUnderLineHorzBrush(BaseRowHeaderInfo rh, BaseRow nextRow) { return rh.GetHorzLineBrush(vi.RC); }
		public virtual Rectangle UpdateGridLinesBounds(Rectangle cellBounds, BaseRow nextRow) { return cellBounds; }
		public bool IsCategory(BaseRow row) {
			if(row == null) return false;
			return (row.XtraRowTypeID == RowTypeIdConsts.CategoryRowTypeId);
		}
	}
	public class DotNetStyleCalcHelper : PaintStyleCalcHelper {
		public DotNetStyleCalcHelper(BaseViewInfo vi) : base(vi) {}
		public override Rectangle ChangeFocusRow(BaseRow newFocus, BaseRow oldFocus) {
			Rectangle rUnion = Rectangle.Empty;
			BaseRowViewInfo ri = vi[newFocus];
			if(ri != null) {
				ri.Calc(vi.RestoreRowRect(ri.RowRect), vi, Grid.VisibleRows[ri.Row.VisibleIndex + 1]);
				rUnion = ri.RowRect;
			}
			ri = vi[oldFocus];
			if(ri != null) {
				ri.Calc(vi.RestoreRowRect(ri.RowRect), vi, Grid.VisibleRows[ri.Row.VisibleIndex + 1]);
				rUnion = Rectangle.Union(rUnion, ri.RowRect);
			}
			return rUnion;
		}
		public override Rectangle GetCategoryFocusRect(CategoryRowHeaderInfo rh) {
			if(Grid.FocusedRow != rh.Row) return Rectangle.Empty;
			if(rh.Row.Properties.Caption == string.Empty) return Rectangle.Empty;
			Rectangle textRect = ((RowCaptionInfo)rh.CaptionsInfo[0]).CaptionTextRect;
			Rectangle captRect = ((RowCaptionInfo)rh.CaptionsInfo[0]).CaptionRect;
			Rectangle rCell = new Rectangle(textRect.Left, captRect.Top, textRect.Width, captRect.Height);
			AppearanceObject captStyle = rh.Style;
			Size sz = BaseViewInfo.CalcTextSize(captStyle, vi.Graphics, rh.Row.Properties.Caption, textRect.Width);
			sz.Width = Math.Min(sz.Width + 1, rCell.Width - 2);
			sz.Height = Math.Min(sz.Height + 1, rCell.Height - 2);
			int x = rCell.Left;
			if(captStyle.TextOptions.HAlignment == HorzAlignment.Far) x = rCell.Right - sz.Width;
			if(captStyle.TextOptions.HAlignment == HorzAlignment.Center) x = RectUtils.GetLeftCentralPoint(sz.Width, rCell);
			int y = rCell.Top;
			if(captStyle.TextOptions.VAlignment == VertAlignment.Bottom) y = rCell.Bottom - sz.Height;
			if(captStyle.TextOptions.VAlignment == VertAlignment.Center || captStyle.TextOptions.VAlignment == VertAlignment.Default) y = RectUtils.GetTopCentralPoint(sz.Height, rCell);
			Rectangle rect = new Rectangle(new Point(x - 1, y + 1), sz);
			if(rh.Grid.ViewInfo.IsRightToLeft)
				rect = rh.Grid.ViewInfo.ConvertBoundsToRTL(rect, rCell);
			return rect;
		}
		public override Rectangle GetComplexFillRect(BaseRowHeaderInfo rh) {
			int left = rh.RowIndents[rh.RowIndents.Count - 1].Bounds.Left;
			return new Rectangle(left, rh.HeaderRect.Top, rh.HeaderRect.Right - left, rh.HeaderRect.Height);
		}
		public override void AddHeaderIndentLines(BaseRowHeaderInfo rh, IndentInfo val, bool toCategories, bool addHorzLine, bool addVertLine) {
			Rectangle indent = vi.IsRightToLeft ? vi.ConvertBoundsToRTL(val.Bounds, rh.HeaderRect) : val.Bounds;
			if(vi.RC.VertLineWidth != 0) {
				if(addVertLine){
					int x = vi.IsRightToLeft ? indent.Left + vi.RC.VertLineWidth : indent.Right - vi.RC.VertLineWidth;
					rh.LinesInfo.AddLine(new LineInfo(x, indent.Top, vi.RC.VertLineWidth, indent.Height + vi.RC.HorzLineWidth, vi.RC.CategoryHorzLineBrush));
				}
				else
					indent.Width += vi.RC.VertLineWidth;
			}
			if(vi.RC.HorzLineWidth != 0) {
				if(addHorzLine) {
					int width = indent.Width - (addVertLine ? vi.RC.VertLineWidth : 0);
					int x = vi.IsRightToLeft ? indent.Right - width : indent.Left;
					rh.LinesInfo.AddLine(new LineInfo(x, indent.Bottom, width, vi.RC.HorzLineWidth, toCategories ? vi.RC.CategoryHorzLineBrush : vi.RC.RowHorzLineBrush));
				}
				else
					indent.Height += vi.RC.HorzLineWidth;
			}
			val.Bounds = indent;
		}
		public override void CalcPaintStyleLines(BaseRowViewInfo ri, BaseRow nextRow) {}
		public override void AddBoundHeaderLines(BaseRowHeaderInfo rh, BaseRow nextRow) {
			bool isRightToLeft = rh.Grid.ViewInfo.IsRightToLeft;
			int x = isRightToLeft ? rh.IndentBounds.Left : rh.IndentBounds.Right;
			if(rh.Row.XtraRowTypeID == RowTypeIdConsts.CategoryRowTypeId) {
				int xx = ((CategoryRowHeaderInfo)rh).ValuesSeparatorPos;
				if(xx != BaseViewInfo.invalid_position && xx < x) 
					x = xx;
			}
			Rectangle r = new Rectangle(x, rh.IndentBounds.Top, rh.HeaderRect.Width - rh.IndentBounds.Width, rh.IndentBounds.Height);
			rh.AddBottomHorzLine(r, vi, GetUnderLineHorzBrush(rh, nextRow));
		}
		public override Brush GetUnderLineHorzBrush(BaseRowHeaderInfo rh, BaseRow nextRow) {
			Brush result = base.GetUnderLineHorzBrush(rh, nextRow);
			if(IsCategory(nextRow))
				result = vi.RC.CategoryHorzLineBrush;
			return result;
		}
		public override Rectangle UpdateGridLinesBounds(Rectangle cellBounds, BaseRow nextRow) {
			Rectangle result = base.UpdateGridLinesBounds(cellBounds, nextRow);
			if(IsCategory(nextRow))
				result = RectUtils.IncreaseFromLeft(result, vi.RC.VertLineWidth);
			return result;
		}
		public override AppearanceObject GetIndentStyle(BaseRowHeaderInfo rh, BaseRow parentIndentRow) {
			if(rh.Row == Grid.FocusedRow && parentIndentRow == rh.Row && rh.Row.XtraRowTypeID != RowTypeIdConsts.CategoryRowTypeId)
				return vi.FocusedRowStyle;
			return parentIndentRow.GetRowHeaderStyle(vi.PaintAppearance);
		}
		public override int GetRowHeaderViewPoint(BaseRowHeaderInfo rh) {
			int result = base.GetRowHeaderViewPoint(rh);
			if(rh.RowIndents.Count > 0 && !vi.IsRightToLeft)
				result = rh.RowIndents[rh.RowIndents.Count - 1].Bounds.Left;
			return result;
		}
		public override int GetRowHeaderViewEndPoint(BaseRowHeaderInfo rh) {
			int result = base.GetRowHeaderViewEndPoint(rh);
			if(rh.RowIndents.Count > 0 && vi.IsRightToLeft)
				result = rh.RowIndents[rh.RowIndents.Count - 1].Bounds.Right;
			return result;
		}
		public override int GetCategoryHeaderViewPoint(CategoryRowHeaderInfo rh) {
			return rh.IndentBounds.Right;
		}
	}
	public class Style3DCalcHelper : PaintStyleCalcHelper {
		public Style3DCalcHelper(BaseViewInfo vi) : base(vi) {}
		public override Rectangle ChangeFocusRow(BaseRow newFocus, BaseRow oldFocus) {
			vi.FocusLinesInfo.Clear();
			Rectangle rUnion = Rectangle.Empty;
			BaseRowViewInfo ri = vi[newFocus];
			if(ri != null) {
				ri.Calc(vi.RestoreRowRect(ri.RowRect), vi, Grid.VisibleRows[ri.Row.VisibleIndex + 1]);
				rUnion = ri.RowRect;
				ri = vi[Grid.VisibleRows[ri.Row.VisibleIndex - 1]];
				if(ri != null) {
					ri.Calc(vi.RestoreRowRect(ri.RowRect), vi, newFocus);
					rUnion = Rectangle.Union(rUnion, ri.RowRect);
				}
			}
			ri = vi[oldFocus];
			if(ri != null) {
				 ri.Calc(vi.RestoreRowRect(ri.RowRect), vi, Grid.VisibleRows[ri.Row.VisibleIndex + 1]);
				if(rUnion.IsEmpty) rUnion = ri.RowRect;
				else rUnion = Rectangle.Union(rUnion, ri.RowRect);
				ri = vi[Grid.VisibleRows[ri.Row.VisibleIndex - 1]];
				if(ri != null ) {
					ri.Calc(vi.RestoreRowRect(ri.RowRect), vi, oldFocus);
					rUnion = Rectangle.Union(rUnion, ri.RowRect);
				}
			}
			return Rectangle.Inflate(rUnion, Math.Max(1, vi.RC.HorzLineWidth), 2 * Math.Max(1, vi.RC.VertLineWidth));
		}
		public override Rectangle GetCategoryFocusRect(CategoryRowHeaderInfo rh) { return Rectangle.Empty; }
		public override Rectangle GetComplexFillRect(BaseRowHeaderInfo rh) {
			return rh.HeaderRect;
		}
		public override void AddHeaderIndentLines(BaseRowHeaderInfo rh, IndentInfo val, bool toCategories, bool underline, bool addVertLine) {}
		public override void CalcPaintStyleLines(BaseRowViewInfo ri, BaseRow nextRow) {
			if(Grid.FocusedRow != null) {
				const int lw = 1;
				Rectangle rowRect = ri.RowRect;
				if(nextRow == Grid.FocusedRow) {
					int focusedRowBandIndex = Grid.Scroller.GetBandIndexByRowIndex(nextRow.VisibleIndex);
					if(focusedRowBandIndex == ri.BandIndex) {
						vi.FocusLinesInfo.AddLine(new LineInfo(rowRect.Left, rowRect.Bottom - lw, rowRect.Width, lw, SystemBrushes.ControlDarkDark));
						vi.FocusLinesInfo.AddLine(new LineInfo(rowRect.Left + lw, rowRect.Bottom, rowRect.Width - lw, lw, SystemBrushes.ControlDark));
					}
				}
				if(ri.Row == Grid.FocusedRow) {
					vi.FocusLinesInfo.AddLine(new LineInfo(rowRect.Left, rowRect.Bottom, rowRect.Width, lw, SystemBrushes.ControlLightLight));
					vi.FocusLinesInfo.AddLine(new LineInfo(rowRect.Left, rowRect.Top - lw, lw, rowRect.Height + lw, SystemBrushes.ControlDarkDark));
					vi.FocusLinesInfo.AddLine(new LineInfo(rowRect.Left + lw, rowRect.Top, lw, rowRect.Height - lw, SystemBrushes.ControlDark));
				}
			}
		}
		public override void AddBoundHeaderLines(BaseRowHeaderInfo rh, BaseRow nextRow) {
			if(vi.RC.HorzLineWidth != 0)
				rh.LinesInfo.AddLine(new LineInfo(rh.HeaderRect.Left, rh.HeaderRect.Bottom, rh.HeaderRect.Width, vi.RC.HorzLineWidth, rh.GetHorzLineBrush(vi.RC)));
		}
		public override AppearanceObject GetIndentStyle(BaseRowHeaderInfo rh, BaseRow parentIndentRow) {
			return rh.Style;
		}
	}
}
namespace DevExpress.XtraVerticalGrid.Painters {
	public class PaintEventHelper {
		private VGridControlBase grid;
		internal PaintEventHelper(VGridControlBase grid) {
			this.grid = grid;
		}
		public void DrawRowHeaderCell(CustomDrawRowHeaderCellEventArgs e) {
			grid.RaiseCustomDrawRowHeaderCell(e);
		}
		public void DrawRowValueCell(CustomDrawRowValueCellEventArgs e) {
			grid.RaiseCustomDrawRowValueCell(e);
		}
		public void DrawRowHeaderIndent(CustomDrawRowHeaderIndentEventArgs e) {
			grid.RaiseCustomDrawRowHeaderIndent(e);
		}
		public void DrawSeparator(CustomDrawSeparatorEventArgs e) {
			grid.RaiseCustomDrawSeparator(e);
		}
		public void DrawTreeButton(CustomDrawTreeButtonEventArgs e) {
			grid.RaiseCustomDrawTreeButton(e);
		}
		public RowValueInfo DrawnCell { 
			get { return grid.drawnCell; } 
			set { grid.drawnCell = value; }
		}
		public object ImageList { get { return grid.ImageList; } }
		public LookAndFeel.UserLookAndFeel LookAndFeel { get { return grid.ElementsLookAndFeel; } }
		public BorderStyles BorderStyle { get { return grid.BorderStyle; } }
	}
	public class GridSavePaintArgs {
		Rectangle clipRect;
		VGridDrawInfo drawInfo;
		public GridSavePaintArgs() {
			this.clipRect = Rectangle.Empty;
			this.drawInfo = null;
		}
		public VGridDrawInfo DrawInfo { get { return drawInfo; } set { drawInfo = value; } }
		public Rectangle ClipRect { get { return clipRect; } set { clipRect = value; } }
	}
	public class VGridDrawInfo : GraphicsInfoArgs, IDisposable {
		BaseViewInfo viewInfo;
		Region nativeClip;
		public VGridDrawInfo(GraphicsCache graphicsCache, BaseViewInfo viewInfo, Rectangle bounds) : base(graphicsCache, bounds) {
			this.viewInfo = viewInfo;
			this.nativeClip = graphicsCache.Graphics.Clip;
		}
		public virtual void Dispose() { this.nativeClip = null; }
		public virtual BaseViewInfo ViewInfo { get { return viewInfo; } }
		public Region NativeClip { get { return nativeClip; } }
	}
	public class PaintStyleParams {
		int separatorWidth;
		Size treeButClientSize;
		public PaintStyleParams() {
			treeButClientSize = new Size(7, 7);
			separatorWidth = 1;
		}
		public int SeparatorWidth { get { return separatorWidth; } set { separatorWidth = value; } }
		public virtual Size TreeButClientSize { get { return treeButClientSize; } }
	}
}
