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
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.Skins;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Drawing;
using DevExpress.XtraGrid;
using DevExpress.XtraTreeList.Columns;
using DevExpress.XtraTreeList.Handler;
using DevExpress.XtraTreeList.Nodes;
using DevExpress.XtraTreeList.Nodes.Operations;
using DevExpress.XtraTreeList.Painter;
using DevExpress.XtraTreeList.ViewInfo;
using System.Drawing.Imaging;
namespace DevExpress.XtraTreeList {
	public enum SummaryItemType { Sum, Min, Max, Count, Average, Custom, None }
	public enum ShowButtonModeEnum { Default, ShowAlways, ShowForFocusedRow, ShowForFocusedCell, ShowOnlyInEditor }
	public enum ScrollVisibility { Never, Always, Auto }
	public enum ViewBorderStyle { None, Single, Border3D, Flat, ThinFlat }
	public enum LineStyle { None = 1, Percent50 = 12, Dark = 20, Light = 18, Wide = 23, Large = 50, Solid = 0 }
	public enum HitInfoType { None, Empty, ColumnButton, BehindColumn, Column, ColumnEdge, RowIndicator, RowIndicatorEdge, RowIndent, Row, RowPreview, RowFooter, Cell, Button, StateImage, SelectImage, SummaryFooter, CustomizationForm, VScrollBar, HScrollBar, FixedLeftDiv, FixedRightDiv, NodeCheckBox, AutoFilterRow, FilterPanel, FilterPanelCloseButton, FilterPanelActiveButton, FilterPanelText, FilterPanelMRUButton, FilterPanelCustomizeButton, ColumnFilterButton, Band, BandPanel, BandButton, BandEdge, Caption }
	public enum NodeChangeTypeEnum { Expanded, HasChildren, ImageIndex, SelectImageIndex, StateImageIndex, Tag, Add, Remove, CheckedState, User1, User2, User3 }
	public enum TreeListState { Regular, ColumnDragging, ColumnSizing, ColumnPressed, ColumnButtonPressed, Editing, OuterDragging, NodePressed, NodeDragging, NodeSizing, Design, MultiSelection, IncrementalSearch, BandPressed, BandDragging, BandSizing, BandButtonPressed, CellSelection }
	public enum TreeListDragNodesMode { Default, Standard, Advanced }
	public enum DropNodesMode { Default, Standard, Advanced }
	public enum DragNodesMode { None, Single, Multiple }
	public enum ShowFilterPanelMode { Default, ShowAlways, Never }
	public enum FilterPopupMode { Default, List, CheckedList, Date }
	public enum FilterMode { Default, Standard, Smart, Extended }
	public enum FindMode { Default, Always, FindClick }
	public enum TreeListAnimationType { Default, AnimateAllContent, AnimateFocusedNode, NeverAnimate }
	public enum DrawFocusRectStyle { None, RowFocus, CellFocus, RowFullFocus };
	public enum TreeListMultiSelectMode { RowSelect, CellSelect }
	internal enum RowIndentItem { None, Root, FirstRoot, LastRoot, Parent, Single, NextChild, LastChild }
	internal enum PrintSideEdges { All, WithoutRight, WithoutLeft, TopBottom }
	internal enum ServiceColumnEnum { KeyFieldName, ParentFieldName, ImageIndexFieldName }
	public interface IHeaderObject {
		int Width { get; }
		int MinWidth { get; }
		int VisibleWidth { get; }
		bool Visible { get; }
		FixedStyle Fixed { get; }
		bool FixedWidth { get; }
		IList Children { get; }
		IList Columns { get; }
		IHeaderObject Parent { get; }
		void SetWidth(int width, bool onlyVisibleWidth);
	}
	public interface IBandRow {
		ICollection Columns { get; }
	}
	public interface IHeaderObjectCollection<T> : IEnumerable<T> where T : IHeaderObject {
		void Synchronize(IEnumerable<T> sourceCollection);
	}
	public class TreeListCell {
		public TreeListCell(TreeListNode node, TreeListColumn column) {
			Node = node;
			Column = column;
		}
		public TreeListNode Node { get; private set; }
		public TreeListColumn Column { get; private set; }
		public bool Equals(TreeListCell cell) {
			return cell.Node == Node && cell.Column == Column;
		}
	}
	public class TreeListHitTest {
		public HitInfoType HitInfoType;
		public ColumnInfo ColumnInfo;
		public BandInfo BandInfo;
		public RowInfo RowInfo;
		public CellInfo CellInfo;
		public RowFooterInfo RowFooterInfo;
		public FooterItem FooterItem;
		public Rectangle MouseDest;
		public Point MousePoint;
		public TreeListHitTest() {
			Clear();
		}
		public void Clear() {
			HitInfoType = HitInfoType.None;
			ColumnInfo = null;
			BandInfo = null;
			RowInfo = null;
			CellInfo = null;
			RowFooterInfo = null;
			FooterItem = null;
			MouseDest = Rectangle.Empty;
			MousePoint = Point.Empty;
		}
		public TreeListNode Node { get { return RowInfo == null ? null : RowInfo.Node; } }
		public TreeListColumn Column { get { return ColumnInfo == null ? null : ColumnInfo.Column; } }
		public TreeListBand Band { get { return BandInfo == null ? null : BandInfo.Band; } }
		protected internal bool InRow {
			get {
				return HitInfoType == HitInfoType.Row || HitInfoType == HitInfoType.RowIndent || HitInfoType == HitInfoType.RowIndicator || HitInfoType == HitInfoType.RowPreview ||
					HitInfoType == HitInfoType.Cell || HitInfoType == HitInfoType.SelectImage || HitInfoType == HitInfoType.StateImage;
			}
		}
		public bool IsEqual(TreeListHitTest ht) {
			if(this.HitInfoType != ht.HitInfoType) return false;
			return this.MouseDest.Equals(ht.MouseDest);
		}
	}
	public class NodesComparer : IComparer {
		ICollection sortColumns;
		TreeList treeList;
		public NodesComparer(TreeList treeList, ICollection sortColumns) {
			this.sortColumns = sortColumns;
			this.treeList = treeList;
		}
		internal static int InternalCompare(object x, object y) {
			if(x == y) return 0;
			if(x == null || x == DBNull.Value) return -1;
			if(y == null || y == DBNull.Value) return 1;
			IComparable xComp = (x is IComparable) ? (IComparable)x : (IComparable)x.ToString();
			IComparable yComp = (y is IComparable) ? (IComparable)y : (IComparable)y.ToString();
			int res = 0;
			try {
				res = xComp.CompareTo(yComp);
			}
			catch { }
			return res;
		}
		int IComparer.Compare(object x, object y) {
			int res = 0;
			TreeListNode node1 = x as TreeListNode;
			TreeListNode node2 = y as TreeListNode;
			if(node1 == node2) return res;
			foreach(TreeListColumn columnID in sortColumns) {
				ColumnSortMode sortMode = columnID.GetActualSortMode();
				object v1 = GetCompareValue(node1, columnID, sortMode);
				object v2 = GetCompareValue(node2, columnID, sortMode);
				res = InternalCompare(v1, v2);
				if(sortMode == ColumnSortMode.Custom)
					res = treeList.InternalGetCompareResult(new CompareNodeValuesEventArgs(node1, node2, v1, v2, columnID, columnID.SortOrder, res));
				if(res == 0) continue;
				if(columnID.SortOrder == SortOrder.Ascending)
					return res;
				res = (res > 0 ? -1 : 1);
				return res;
			}
			if(res == 0 && treeList.EnableEnhancedSorting)
				res = Comparer<int>.Default.Compare(node1.Id, node2.Id);
			return res;
		}
		object GetCompareValue(TreeListNode node, TreeListColumn column, ColumnSortMode sortMode) {
			if(sortMode == ColumnSortMode.DisplayText)
				return node.GetDisplayText(column);
			return node.GetValue(column);
		}
		int GetNodeIndex(TreeListNode node) {
			return node.owner.IndexOf(node);
		}
	}
	public class VisibleColumnIndexComparer : IComparer {
		public VisibleColumnIndexComparer() { }
		int IComparer.Compare(object a, object b) {
			TreeListColumn c1 = a as TreeListColumn, c2 = b as TreeListColumn;
			if(c1 == c2) return 0;
			if(c1 == null) return -1;
			if(c2 == null) return 1;
			if(c1.Fixed != c2.Fixed) {
				if(c1.Fixed == FixedStyle.Left) return -1;
				if(c2.Fixed == FixedStyle.Left) return 1;
				if(c1.Fixed == FixedStyle.Right) return 1;
				if(c2.Fixed == FixedStyle.Right) return -1;
			}
			int res = Comparer.Default.Compare(c1.VisibleIndex, c2.VisibleIndex);
			if(res == 0) {
				res = Comparer.Default.Compare(c1.AbsoluteIndex, c2.AbsoluteIndex);
			}
			return res;
		}
	}
	public class BandIndexComparer : IComparer<TreeListBand> {
		int IComparer<TreeListBand>.Compare(TreeListBand band1, TreeListBand band2) {
			if(band1 == band2) return 0;
			int res = Comparer.Default.Compare(band1.RootBand.Index, band2.RootBand.Index);
			if(res != 0) return res;
			if(band1.ParentBand == band2.ParentBand) return Comparer.Default.Compare(band1.Index, band2.Index);
			res = CompareCore(band1, band2);
			if(res != 0) return res;
			return Comparer.Default.Compare(band1.Index, band2.Index);
		}
		int CompareCore(TreeListBand band1, TreeListBand band2) {
			while(band1.ParentBand != null) {
				TreeListBand band2_tmp = band2;
				while(band2_tmp != null) {
					if(band2_tmp.ParentBand == band1.ParentBand)
						return Comparer.Default.Compare(band1.Index, band2_tmp.Index);
					band2_tmp = band2_tmp.ParentBand;
				}
				band1 = band1.ParentBand;
			}
			return 0;
		}
	}
	public class BandVisibleIndexComparer : IComparer<TreeListBand> {
		int IComparer<TreeListBand>.Compare(TreeListBand b1, TreeListBand b2) {
			if(b1 == b2) return 0;
			if(b1 == null) return -1;
			if(b2 == null) return 1;
			if(b1.Fixed != b2.Fixed) {
				if(b1.Fixed == FixedStyle.Left) return -1;
				if(b2.Fixed == FixedStyle.Left) return 1;
				if(b1.Fixed == FixedStyle.Right) return 1;
				if(b2.Fixed == FixedStyle.Right) return -1;
			}
			return Comparer.Default.Compare(b1.Index, b2.Index);
		}
	}
	public class BandColumnRowIndexComparer : IComparer {
		int IComparer.Compare(object a, object b) {
			TreeListColumn c1 = (TreeListColumn)a, c2 = (TreeListColumn)b;
			if(c1 == c2) return 0;
			if(c1.RowIndex == c2.RowIndex)
				return Comparer.Default.Compare(c1.ParentBand.Columns.IndexOf(c1), c2.ParentBand.Columns.IndexOf(c2));
			return Comparer.Default.Compare(c1.RowIndex, c2.RowIndex);
		}
	}
	public class ColumnSortIndexComparer : IComparer {
		public ColumnSortIndexComparer() { }
		int IComparer.Compare(object a, object b) {
			TreeListColumn c1 = (TreeListColumn)a;
			TreeListColumn c2 = (TreeListColumn)b;
			int res = Comparer.Default.Compare(c1.SortIndex, c2.SortIndex);
			if(res == 0) {
				res = Comparer.Default.Compare(c1.AbsoluteIndex, c2.AbsoluteIndex);
			}
			return res;
		}
	}
	public class FooterItem {
		public Rectangle ItemBounds;
		public string ItemText;
		public TreeListColumn Column;
		public SummaryItemType ItemType;
		public FooterItem() : this(Rectangle.Empty, string.Empty, null, SummaryItemType.None) { }
		public FooterItem(Rectangle iR, string iT, TreeListColumn column, SummaryItemType itp) {
			ItemBounds = iR;
			ItemText = iT;
			Column = column;
			ItemType = itp;
		}
		public void OffsetContent(int cx, int cy) {
			ItemBounds.Offset(cx, cy);
		}
	}
	public class LineInfo {
		public Rectangle Rect;
		public AppearanceObject Appearance;
		public LineInfo(Rectangle r, AppearanceObject appearance) {
			Rect = r;
			this.Appearance = appearance;
		}
		public LineInfo(int x, int y, int width, int height, AppearanceObject appearance) {
			Rect = new Rectangle(x, y, width, height);
			this.Appearance = appearance;
		}
		public void OffsetContent(int cx, int cy) {
			Rect.Offset(cx, cy);
		}
	}
	public class TreeListHitInfo {
		TreeListColumn column;
		TreeListBand band;
		TreeListNode node;
		HitInfoType hitInfoType;
		Point mousePoint;
		Rectangle bounds;
		TreeListHitTest hitTest;
		public TreeListHitInfo() {
			Clear();
		}
		internal TreeListHitInfo(TreeListHitTest ht) {
			Clear();
			hitTest = ht;
			Parse(HitTest);
		}
		internal void Clear() {
			hitTest = null;
			column = null;
			band = null;
			node = null;
			hitInfoType = HitInfoType.None;
			mousePoint = Point.Empty;
			bounds = Rectangle.Empty;
		}
		internal void Parse(TreeListHitTest ht) {
			mousePoint = ht.MousePoint;
			hitInfoType = ht.HitInfoType;
			bounds = ht.MouseDest;
			switch(ht.HitInfoType) {
				case HitInfoType.Column:
				case HitInfoType.ColumnEdge:
				case HitInfoType.ColumnFilterButton:
					column = ht.ColumnInfo.Column;
					break;
				case HitInfoType.CustomizationForm:
					if(ht.ColumnInfo != null)
						column = ht.ColumnInfo.Column;
					else if(ht.Band != null)
						band = ht.Band;
					break;
				case XtraTreeList.HitInfoType.Band:
				case XtraTreeList.HitInfoType.BandEdge:
					band = ht.Band;
					break;
				case HitInfoType.Row:
				case HitInfoType.RowIndicator:
				case HitInfoType.RowIndicatorEdge:
				case HitInfoType.Button:
				case HitInfoType.RowPreview:
				case HitInfoType.RowIndent:
				case HitInfoType.SelectImage:
				case HitInfoType.StateImage:
				case HitInfoType.NodeCheckBox:
					node = ht.RowInfo.Node;
					break;
				case HitInfoType.AutoFilterRow:
					node = ht.RowInfo.Node;
					break;
				case HitInfoType.RowFooter:
					node = ht.RowFooterInfo.Node;
					if(ht.FooterItem != null)
						column = ht.FooterItem.Column;
					break;
				case HitInfoType.Cell:
					node = ht.RowInfo.Node;
					column = ht.ColumnInfo.Column;
					break;
				case HitInfoType.Empty:
				case HitInfoType.ColumnButton:
					break;
				case HitInfoType.SummaryFooter:
					if(ht.FooterItem != null)
						column = ht.FooterItem.Column;
					break;
			}
		}
		public TreeListColumn Column { get { return column; } }
		public TreeListBand Band { get { return band; } }
		public TreeListNode Node { get { return node; } }
		public HitInfoType HitInfoType { get { return hitInfoType; } }
		public Point MousePoint { get { return mousePoint; } }
		public Rectangle Bounds { get { return bounds; } }
		[Browsable(false)]
		public TreeListHitTest HitTest { get { return hitTest; } }
	}
	public class AppearanceName {
		public static readonly string[] DefaultStyleNames =
			new string[] {
							 "HeaderPanel", "FooterPanel", "Row", 
							 "EvenRow", "OddRow", "HorzLine", "VertLine",
							 "Preview", "FocusedRow", "FocusedCell",
							 "GroupButton", "TreeLine", "GroupFooter", "Empty",
							 "SelectedRow", "HideSelectionRow", "FixedLine", "CustomizationFormHint",
							 "FilterPanel", "BandPanel", "HeaderPanelBackground", "Caption"
						 };
		public const int HeaderPanelId = 0, FooterPanelId = 1, RowId = 2,
			EvenRowId = 3, OddRowId = 4, HorzLineId = 5, VertLineId = 6,
			PreviewId = 7, FocusedRowId = 8, FocusedCellId = 9,
			GroupButtonId = 10, TreeLineId = 11, GroupFooterId = 12, EmptyId = 13,
			SelectedRowId = 14, HideSelectionRowId = 15, FixedLineId = 16, CustomizationFormHintId = 17, FilterPanelId = 18, BandPanelId = 19, HeaderPanelBackgroundId = 20, CaptionId = 21;
		public static string HeaderPanel { get { return DefaultStyleNames[HeaderPanelId]; } }
		public static string HeaderPanelBackground { get { return DefaultStyleNames[HeaderPanelBackgroundId]; } }
		public static string BandPanel { get { return DefaultStyleNames[BandPanelId]; } }
		public static string FooterPanel { get { return DefaultStyleNames[FooterPanelId]; } }
		public static string Row { get { return DefaultStyleNames[RowId]; } }
		public static string EvenRow { get { return DefaultStyleNames[EvenRowId]; } }
		public static string OddRow { get { return DefaultStyleNames[OddRowId]; } }
		public static string HorzLine { get { return DefaultStyleNames[HorzLineId]; } }
		public static string VertLine { get { return DefaultStyleNames[VertLineId]; } }
		public static string Preview { get { return DefaultStyleNames[PreviewId]; } }
		public static string FocusedRow { get { return DefaultStyleNames[FocusedRowId]; } }
		public static string FocusedCell { get { return DefaultStyleNames[FocusedCellId]; } }
		public static string GroupButton { get { return DefaultStyleNames[GroupButtonId]; } }
		public static string TreeLine { get { return DefaultStyleNames[TreeLineId]; } }
		public static string GroupFooter { get { return DefaultStyleNames[GroupFooterId]; } }
		public static string Empty { get { return DefaultStyleNames[EmptyId]; } }
		public static string SelectedRow { get { return DefaultStyleNames[SelectedRowId]; } }
		public static string HideSelectionRow { get { return DefaultStyleNames[HideSelectionRowId]; } }
		public static string FixedLine { get { return DefaultStyleNames[FixedLineId]; } }
		public static string CustomizationFormHint { get { return DefaultStyleNames[CustomizationFormHintId]; } }
		public static string FilterPanel { get { return DefaultStyleNames[FilterPanelId]; } }
		public static string Caption { get { return DefaultStyleNames[CaptionId]; } }
	}
	public class DrawDragPreviewEventArgs : EventArgs {
		GraphicsCache cacheCore;
		RowInfo[] rowsInfoCore;
		Rectangle boundsCore;
		internal DrawDragPreviewEventArgs(GraphicsCache cache, RowInfo[] rowsInfo, Rectangle bounds) {
			cacheCore = cache;
			rowsInfoCore = rowsInfo;
			boundsCore = bounds;
		}
		public GraphicsCache Cache { get { return cacheCore; } }
		public RowInfo[] RowsInfo { get { return rowsInfoCore; } }
		public Rectangle Bounds { get { return boundsCore; } }
	}
	internal class DrawDragColumnEventArgs : EventArgs {
		Graphics graphics;
		ColumnInfo columnInfo;
		internal DrawDragColumnEventArgs(Graphics g, ColumnInfo ci) {
			graphics = g;
			columnInfo = ci;
		}
		internal Graphics Graphics { get { return graphics; } }
		internal ColumnInfo ColumnInfo { get { return columnInfo; } }
	}
	public enum DragInsertPosition { None, AsChild, Before, After }
	public class DragScrollInfo : IDisposable {
		bool customDragDrop;
		RowInfo rowInfo;
		DragDropEffects lastEffect;
		internal bool scrollLock;
		internal DragInsertPosition dragInsertPosition;
		internal bool copy;
		internal System.Timers.Timer scrollTimer;
		internal System.Windows.Forms.Timer expandTimer;
		internal Point mouse;
		internal int imageIndex;
		internal bool CanInsert {
			get { return dragInsertPosition != DragInsertPosition.None && dragInsertPosition != DragInsertPosition.AsChild; }
		}
		public DragScrollInfo() {
			this.scrollLock = false;
			this.scrollTimer = new System.Timers.Timer();
			this.expandTimer = new System.Windows.Forms.Timer();
			this.expandTimer.Interval = 1000;
			Reset();
		}
		public void Reset() {
			Stop();
			expandTimer.Enabled = false;
			rowInfo = null;
			dragInsertPosition = DragInsertPosition.None;
			copy = false;
			imageIndex = -1;
			customDragDrop = false;
			mouse = Point.Empty;
			lastEffect = DragDropEffects.None;
		}
		public void Dispose() {
			scrollTimer.Dispose();
			expandTimer.Dispose();
		}
		public void Go(int interval) {
			if(!scrollTimer.Enabled) {
				scrollTimer.Interval = interval;
				scrollTimer.Enabled = true;
			}
		}
		public void Stop() {
			if(scrollTimer.Enabled) {
				scrollTimer.Enabled = false;
			}
		}
		public void CheckCustomDragDrop(DragDropEffects effect) {
			if(!CustomDragDrop) {
				customDragDrop = (LastEffect != effect);
			}
		}
		public bool CustomDragDrop { get { return customDragDrop; } }
		public DragInsertPosition DragInsertPosition { get { return dragInsertPosition; } }
		public DragDropEffects LastEffect {
			get { return lastEffect; }
			set {
				if(!CustomDragDrop)
					lastEffect = value;
			}
		}
		public RowInfo RowInfo {
			get { return rowInfo; }
			set {
				if(RowInfo == value) return;
				rowInfo = value;
				expandTimer.Enabled = false;
				expandTimer.Enabled = (RowInfo != null);
			}
		}
	}
	internal class ColumnWidthInfo {
		internal TreeListColumn Column;
		internal int Width;
		internal ColumnWidthInfo(TreeListColumn col) {
			this.Column = col;
			this.Width = col.VisibleWidth;
		}
		internal ColumnWidthInfo() {
			this.Column = null;
			this.Width = 0;
		}
	}
	internal class NodeData {
		internal bool expanded;
		internal object data;
		internal bool visible;
		internal CheckState checkState;
		internal NodeData(bool expanded,  object data, bool visible, CheckState checkState) {
			this.expanded = expanded;
			this.data = data;
			this.visible = visible;
			this.checkState = checkState;
		}
	}
	internal class MouseHover {
		private CellInfo cellInfo;
		private ColumnInfo columnInfo;
		private BandInfo bandInfo;
		private EditorButton button;
		private TreeList treeList;
		private TreeListHitTest filterPanelHitInfo;
		private ColumnInfo filterButtonHitInfo;
		private StateData data;
		private RowInfo oldRowInfo;
		internal MouseHover(StateData data) {
			this.data = data;
			this.cellInfo = null;
			this.columnInfo = null;
			this.bandInfo = null;
			this.button = null;
			this.treeList = data.TreeList;
			this.filterPanelHitInfo = null;
			this.oldRowInfo = null;
		}
		internal void CheckMouseHotTrack(TreeListHitTest ht) {
			CheckColumnHotTrack(ht);
			CheckBandHotTrack(ht);
			CheckCellHotTrack(ht);
			CheckFilterPanelHotTrack(ht);
			if(treeList.IsLookUpMode && treeList.LookUpOwner.AllowSelectOnHover)
				CheckRowHotTrack(ht);
		}
		internal void CheckFilterPanelHotTrack(TreeListHitTest ht) {
			TreeListHitTest oldHitInfo = filterPanelHitInfo;
			filterPanelHitInfo = null;
			ObjectState state = ObjectState.Normal;
			if(ht != null && IsInFilterPanel(ht.HitInfoType)) {
				filterPanelHitInfo = ht;
				state |= ObjectState.Hot;
			}
			if(data.DownHitTest != null && IsInFilterPanel(data.DownHitTest.HitInfoType)) {
				filterPanelHitInfo = data.DownHitTest;
				state |= ObjectState.Pressed;
			}
			if(!AreEqual(filterPanelHitInfo, oldHitInfo, (state & ObjectState.Pressed) != 0)) {
				UpdateFilterPanel(filterPanelHitInfo, state);
				treeList.InvalidateFilterPanel();
			}
		}
		bool AreEqual(TreeListHitTest ht1, TreeListHitTest ht2, bool down) {
			if(ht1 == ht2) return true;
			if(ht1 != null && ht2 != null)
				return !down && ht1.IsEqual(ht2);
			return false;
		}
		bool IsInFilterPanel(HitInfoType ht) {
			return ht == HitInfoType.FilterPanelActiveButton
				|| ht == HitInfoType.FilterPanelCloseButton || ht == HitInfoType.FilterPanelCustomizeButton
				|| ht == HitInfoType.FilterPanelMRUButton || ht == HitInfoType.FilterPanelText;
		}
		void UpdateFilterPanel(TreeListHitTest ht, ObjectState state) {
			TreeListFilterPanelInfoArgs filterPanel = treeList.ViewInfo.FilterPanel;
			if(treeList.MRUFilterPopup != null) {
				filterPanel.TextState = ObjectState.Pressed;
				filterPanel.MRUButtonInfo.State = ObjectState.Pressed;
			}
			else {
				filterPanel.TextState = CalcObjectState(ht, HitInfoType.FilterPanelText, state);
				filterPanel.MRUButtonInfo.State = CalcObjectState(ht, HitInfoType.FilterPanelMRUButton, state);
			}
			filterPanel.CustomizeButtonInfo.State = CalcObjectState(ht, HitInfoType.FilterPanelCustomizeButton, state);
			filterPanel.CloseButtonInfo.State = CalcObjectState(ht, HitInfoType.FilterPanelCloseButton, state);
			filterPanel.ActiveButtonInfo.State = CalcObjectState(ht, HitInfoType.FilterPanelActiveButton, state);
		}
		ObjectState CalcObjectState(TreeListHitTest ht, HitInfoType hitType, ObjectState state) {
			ObjectState defaultState = ObjectState.Normal;
			if(ht != null && ht.HitInfoType == hitType) return state;
			return defaultState;
		}
		private void CheckCellHotTrack(TreeListHitTest ht) {
			CellInfo newCell = null;
			EditorButton newButton = null;
			Point newPt = new Point(-1, -1);
			if(IsEditorCellTest(ht)) {
				if(ht.Column != null && treeList.GetColumnShowButtonMode(ht.Column) != ShowButtonModeEnum.ShowOnlyInEditor) {
					newCell = ht.CellInfo;
					newPt = ht.MousePoint;
					EditorButtonObjectInfoArgs args = newCell.EditorViewInfo.CalcHitInfo(newPt).HitObject as EditorButtonObjectInfoArgs;
					if(args != null) newButton = args.Button;
				}
			}
			if(cellInfo != newCell) {
				UpdateCell(cellInfo, newPt);
				UpdateCell(newCell, newPt);
			}
			else if(button != newButton)
				UpdateCell(newCell, newPt);
			cellInfo = newCell;
			button = newButton;
		}
		private bool IsEditorCellTest(TreeListHitTest ht) {
			return (ht != null && ht.HitInfoType == HitInfoType.Cell && ht.CellInfo.CellValueRect.Contains(ht.MousePoint));
		}
		private void UpdateCell(CellInfo cell, Point pt) {
			if(cell != null && cell.EditorViewInfo != null && cell.EditorViewInfo.IsRequiredUpdateOnMouseMove) {
				cell.CalcViewInfo(null, pt);
				treeList.ViewInfo.PaintAnimatedItems = false;
				treeList.Invalidate(cell.Bounds);
			}
		}
		private void CheckColumnHotTrack(TreeListHitTest ht) {
			ColumnInfo newCol = null;
			if(ht != null && (ht.HitInfoType == HitInfoType.Column || ht.HitInfoType == HitInfoType.ColumnFilterButton))
				if(ht.ColumnInfo.Column != null)
					newCol = ht.ColumnInfo;
			if(columnInfo != newCol) {
				UpdateColumn(columnInfo, false);
				UpdateColumn(newCol, true);
				columnInfo = newCol;
			}
			else {
				if(data.TreeList.FilterPopup == null)
					UpdateFilterButton(ht);
			}
		}
		private void CheckBandHotTrack(TreeListHitTest ht) {
			BandInfo newBandInfo = null;
			if(ht != null && (ht.HitInfoType == HitInfoType.Band))
				if(ht.Band != null)
					newBandInfo = ht.BandInfo;
			if(bandInfo != newBandInfo) {
				UpdateBand(bandInfo, false);
				UpdateBand(newBandInfo, true);
				bandInfo = newBandInfo;
			}
		}
		private void UpdateFilterButton(TreeListHitTest ht) {
			ObjectState state = ObjectState.Normal;
			if(ht != null && ht.HitInfoType == HitInfoType.ColumnFilterButton) {
				filterButtonHitInfo = ht.ColumnInfo;
				state |= ObjectState.Hot;
			}
			if(data.DownHitTest != null && data.DownHitTest.HitInfoType == HitInfoType.ColumnFilterButton) {
				filterButtonHitInfo = data.DownHitTest.ColumnInfo;
				state |= ObjectState.Pressed;
			}
			if(data.TreeList.FilterPopup != null)
				state |= ObjectState.Pressed;
			if(filterButtonHitInfo != null) {
				if(UpdateFilterButtonState(filterButtonHitInfo, state)) {
					treeList.ViewInfo.PaintAnimatedItems = false;
					treeList.Invalidate(filterButtonHitInfo.Bounds);
				}
			}
			if(state == ObjectState.Normal)
				filterButtonHitInfo = null;
		}
		private bool UpdateFilterButtonState(ColumnInfo col, ObjectState state) {
			if(col == null) return false;
			bool changed = false;
			if(col.SetFilterButtonState(state))
				changed = true;
			if(col.SetFilterButtonVisible(col.State != ObjectState.Normal || !col.Column.FilterInfo.IsEmpty || (state & ObjectState.Pressed) != 0))
				changed = true;
			return changed;
		}
		private void UpdateColumn(ColumnInfo col, bool mouseOver) {
			if(col != null) {
				col.MouseOver = mouseOver;
				if(filterButtonHitInfo == null || filterButtonHitInfo != col)
					UpdateFilterButtonState(col, ObjectState.Normal);
				treeList.ViewInfo.PaintAnimatedItems = false;
				treeList.Invalidate(col.Bounds);
			}
		}
		private void UpdateBand(BandInfo bi, bool mouseOver) {
			if(bi != null) {
				bi.MouseOver = mouseOver;
				treeList.ViewInfo.PaintAnimatedItems = false;
				treeList.Invalidate(bi.Bounds);
			}
		}
		void CheckRowHotTrack(TreeListHitTest ht) {
			if(ht != null && ht.RowInfo != null && ht.RowInfo != oldRowInfo) {
				if(treeList.ViewInfo.IsRowVisible(ht.RowInfo)) {
					treeList.isHotTrackMode = true;
					try {
						treeList.FocusedNode = ht.RowInfo.Node;
					}
					finally {
						treeList.isHotTrackMode = false;
					}
					oldRowInfo = ht.RowInfo;
				}
			}
		}
	}
	public class NodesIdInfo {
		int startId;
		int length;
		NodesIdInfo next;
		public NodesIdInfo() {
			this.startId = 0;
			this.length = 0;
			this.next = null;
		}
		public NodesIdInfo(int startId, NodesIdInfo next) {
			this.startId = startId;
			this.length = 1;
			this.next = next;
		}
		public bool Contains(int id) {
			return InInterval(id, StartId, EndId);
		}
		bool InInterval(int value, int min, int max) {
			return (value >= min && value <= max);
		}
		public void Add(int id) {
			if(id == StartId - 1) {
				this.startId = id;
			}
			this.length++;
		}
		public NodesIdInfo FindNearestInfo(int id) {
			if(id < StartId) return null;
			NodesIdInfo prev = this;
			NodesIdInfo following = Next;
			while(following != null) {
				if(InInterval(id, prev.EndId, following.StartId)) break;
				prev = following;
				following = following.Next;
			}
			return prev;
		}
		public int StartId { get { return startId; } }
		public int EndId { get { return (StartId + Length - 1); } }
		public int Length { get { return length; } set { length = value; } }
		public NodesIdInfo Next { get { return next; } set { next = value; } }
	}
	internal class NodesIdInfoManager {
		NodesIdInfo head;
		public NodesIdInfoManager() {
			this.head = null;
		}
		public void Add(int id) {
			if(Head == null) {
				this.head = new NodesIdInfo(id, null);
				return;
			}
			NodesIdInfo nearest = Head.FindNearestInfo(id);
			if(nearest == null) { 
				this.head = new NodesIdInfo(id, Head);
				CheckUnion(Head, Head.Next);
				return;
			}
			else if(nearest.Contains(id)) return;
			if(nearest.Next == null) { 
				if(nearest.EndId + 1 == id) nearest.Add(id);
				else nearest.Next = new NodesIdInfo(id, null);
				return;
			}
			NodesIdInfo between = new NodesIdInfo(id, nearest.Next);
			nearest.Next = between;
			if(nearest.EndId + 1 == between.StartId) {
				Union(nearest, between);
				between = nearest;
			}
			CheckUnion(between, between.Next);
		}
		void CheckUnion(NodesIdInfo prev, NodesIdInfo next) {
			if(prev.EndId + 1 == next.StartId) {
				Union(prev, next);
			}
		}
		void Union(NodesIdInfo prev, NodesIdInfo next) {
			prev.Length += next.Length;
			prev.Next = next.Next;
		}
		public void BuildReversibleList() {
			if(Head == null) return;
			NodesIdInfo newHead = Head;
			NodesIdInfo newPrev = Head;
			NodesIdInfo current = Head;
			NodesIdInfo next = Head.Next;
			while(next != null) {
				newPrev = newHead;
				newHead = next;
				current = next;
				next = next.Next;
				newHead.Next = newPrev;
			}
			Head.Next = null;
			this.head = newHead;
		}
		public int GetSumLength(int id) {
			int result = 0;
			NodesIdInfo current = Head;
			while(current != null) {
				if(id < current.StartId) break;
				result += current.Length;
				current = current.Next;
			}
			return result;
		}
		public NodesIdInfo Head { get { return head; } }
	}
	public class CloneInfo {
		int rowId, count;
		public CloneInfo(int rowId) {
			this.rowId = rowId;
			this.count = 2;
		}
		public int RowId { get { return rowId; } set { rowId = value; } }
		public int Count { get { return count; } set { count = value; } }
	}
	public class CloneInfoCollection : CollectionBase {
		public CloneInfo this[int index] { get { return (CloneInfo)InnerList[index]; } }
		public CloneInfo GetInfoByRowId(int rowId) {
			for(int i = 0; i < Count; i++) {
				CloneInfo result = this[i];
				if(result.RowId == rowId)
					return result;
			}
			return null;
		}
		public void RegisterClone(int rowId) {
			CloneInfo info = GetInfoByRowId(rowId);
			if(info != null) info.Count++;
			else InnerList.Add(new CloneInfo(rowId));
		}
		public void SynchronizeID(TreeListOperationSynchronizeID op) {
			for(int i = 0; i < Count; i++) {
				CloneInfo info = this[i];
				info.RowId = op.GetNewId(info.RowId);
			}
		}
		public void OnClearNodes(TreeListNodes nodes) {
			if(nodes == nodes.TreeList.Nodes) {
				InnerList.Clear();
				return;
			}
			for(int i = Count - 1; i > -1; i--) {
				CloneInfo info = this[i];
				TreeListOperationAddNodeById op = new TreeListOperationAddNodeById(info.RowId);
				nodes.TreeList.NodesIterator.DoLocalOperation(op, nodes);
				info.RowId -= op.Nodes.Count;
				if(info.RowId < 2) InnerList.RemoveAt(i);
			}
		}
	}
#if DEBUG
	sealed internal class TLDebug {
		static readonly TLDebug instance = new TLDebug();
		private TLDebug() { }
		public static TLDebug Instance { get { return instance; } }
		public void Out(params object[] arg) {
			string outStr = string.Empty;
			for(int i = 0; i < arg.Length; i++) {
				outStr += string.Format("{0}", arg[i]);
				if(i < arg.Length - 1) outStr += " ";
			}
			System.Diagnostics.Debug.WriteLine(outStr);
		}
	}
#endif
	public interface IDragNodesProvider : IDisposable {
		TreeListNode PressedNode { get; }
		IEnumerable<TreeListNode> DragNodes { get; }
		TreeList TreeList { get; }
		int Count { get; }
	}
	public interface IDragRowsInfoProvider {
		RowInfo PressedRowInfo { get; }
		IEnumerable<RowInfo> DragRowsInfo { get; }
		Bitmap GetDragPreview();
	}
	class DragNodesProvider : IDragNodesProvider, IDragRowsInfoProvider, IDisposable {
		TreeList treeListCore;
		DragObjectCollection<RowInfo> dragRowsInfoCore;
		DragObjectCollection<TreeListNode> dragNodesCore;
		RowInfo pressedRowInfoCore;
		int rowsPreviewMaxCount = 4;
		Size borderSize = new Size(2, 2);
		Color borderColor = Color.FromArgb(125, Color.Black);
		public DragNodesProvider(TreeList treeList, ICollection<TreeListNode> dragNodes) {
			treeListCore = treeList;
			dragNodesCore = new DragObjectCollection<TreeListNode>(dragNodes);
			Initialize();
		}
		protected void Initialize() {
			if(treeListCore == null) return;
			dragRowsInfoCore = new DragObjectCollection<RowInfo>();
			foreach(RowInfo row in treeListCore.ViewInfo.RowsInfo.Rows) {
				if(dragNodesCore.Contains(row.Node))
					dragRowsInfoCore.Add(row);
				if(row.Node == treeListCore.PressedNode)
					pressedRowInfoCore = row;
			}
		}
		int RowsPreviewCount { get { return Math.Min(dragRowsInfoCore.Count, rowsPreviewMaxCount); } }
		Rectangle CalcDragImageBounds() {
			int width = 0, height = 0;
			for(int i = 0; i < RowsPreviewCount; i++) {
				width = Math.Max(width, dragRowsInfoCore[i].DataBounds.Width + dragRowsInfoCore[i].SelectImageBounds.Width + dragRowsInfoCore[i].StateImageBounds.Width);
				height += dragRowsInfoCore[i].Bounds.Height;
			}
			if(!treeListCore.ViewInfo.BorderSize.IsEmpty) {
				height += 2 * borderSize.Height;
				width += 2 * borderSize.Width;
			}
			return new Rectangle(0, 0, width, height);
		}
		Rectangle GetClientBounds(Rectangle bounds) {
			return Rectangle.Inflate(bounds, -borderSize.Width, -borderSize.Height);
		}
		Rectangle GetBounds(Rectangle bounds) {
			return Rectangle.Inflate(bounds, borderSize.Width, borderSize.Height);
		}
		RowInfo[] CalcRowsInfoPreview(Rectangle bounds) {
			List<RowInfo> rows = new List<RowInfo>();
			Rectangle rowBounds = GetClientBounds(bounds);
			rowBounds.Height = 0;
			for(int i = 0; i < RowsPreviewCount; i++) {
				rowBounds.Height = dragRowsInfoCore[i].Bounds.Height;
				RowInfo row = treeListCore.ViewInfo.GetDragNodeRowInfo(dragRowsInfoCore[i], rowBounds);
				rows.Add(row);
				rowBounds.Y += row.Bounds.Height;
			}
			return rows.ToArray();
		}
		[System.Security.SecuritySafeCritical]
		Bitmap ApplyMask(Bitmap image, RowInfo row) {
			if(row == null) return image;
			using(Bitmap mask = ResourceImageHelper.CreateBitmapFromResources("DevExpress.XtraTreeList.Images.dragMask.png", typeof(DragNodesProvider).Assembly)) {
				Rectangle bounds = GetBounds(row.Bounds);
				using(Bitmap realMask = new Bitmap(bounds.Width, bounds.Height)) {
					using(Graphics g = Graphics.FromImage(realMask))
						DevExpress.Utils.Helpers.PaintHelper.DrawImage(g, mask, new Rectangle(Point.Empty, realMask.Size), new Padding(2));
					BitmapData maskData = realMask.LockBits(new Rectangle(Point.Empty, realMask.Size), ImageLockMode.ReadWrite, realMask.PixelFormat);
					int maskDataSize = maskData.Stride * maskData.Height;
					byte[] maskBytes = new byte[maskDataSize];
					System.Runtime.InteropServices.Marshal.Copy(maskData.Scan0, maskBytes, 0, maskDataSize);
					BitmapData imageData = image.LockBits(bounds, ImageLockMode.ReadWrite, image.PixelFormat);
					int imageDataSize = imageData.Stride * imageData.Height;
					byte[] imageBytes = new byte[imageDataSize];
					System.Runtime.InteropServices.Marshal.Copy(imageData.Scan0, imageBytes, 0, imageDataSize);
					for(int i = 0; i < imageDataSize; i++) {
						if((i + 1) % 4 == 0)
							if(imageBytes[i] > maskBytes[i])
								imageBytes[i] = maskBytes[i];
					}
					realMask.UnlockBits(maskData);
					System.Runtime.InteropServices.Marshal.Copy(imageBytes, 0, imageData.Scan0, imageDataSize);
					image.UnlockBits(imageData);
				}
			}
			return image;
		}
		Bitmap GetDragPreview() {
			Rectangle bounds = CalcDragImageBounds();
			RowInfo[] rows = CalcRowsInfoPreview(bounds);
			if(bounds.Height == 0 || bounds.Width == 0) return null;
			Bitmap bmp = new Bitmap(bounds.Width, bounds.Height);
			using(Graphics g = Graphics.FromImage(bmp)) {
				Rectangle clientBounds = GetClientBounds(bounds);
				using(XtraBufferedGraphics bg = XtraBufferedGraphicsManager.Current.Allocate(g, clientBounds)) {
					using(GraphicsCache cache = new GraphicsCache(bg.Graphics))
						treeListCore.Painter.DrawDragPreview(treeListCore.ViewInfo, new DrawDragPreviewEventArgs(cache, rows, clientBounds));
					bg.Render();
				}
				using(SolidBrush brush = new SolidBrush(borderColor)) {
					using(Pen pen = new Pen(brush, 4))
						g.DrawRectangle(pen, bounds);
				}
			}
			return ApplyMask(bmp, dragRowsInfoCore.Count > 4 ? rows[3] : null);
		}
		#region IDragRowsInfoProvider Members
		RowInfo IDragRowsInfoProvider.PressedRowInfo { get { return pressedRowInfoCore; } }
		IEnumerable<RowInfo> IDragRowsInfoProvider.DragRowsInfo { get { return dragRowsInfoCore; } }
		Bitmap IDragRowsInfoProvider.GetDragPreview() { return GetDragPreview(); }
		TreeListNode IDragNodesProvider.PressedNode {
			get {
				if(pressedRowInfoCore == null) return null;
				return pressedRowInfoCore.Node;
			}
		}
		#endregion
		#region IDisposable Members
		bool isDisposing;
		void IDisposable.Dispose() {
			if(!isDisposing) {
				isDisposing = true;
				OnDispose();
			}
		}
		protected void OnDispose() {
			pressedRowInfoCore = null;
			dragRowsInfoCore.Clear();
			dragNodesCore.Clear();
			treeListCore = null;
		}
		#endregion
		#region IDragNodesProvider Members
		IEnumerable<TreeListNode> IDragNodesProvider.DragNodes { get { return dragNodesCore; } }
		TreeList IDragNodesProvider.TreeList { get { return treeListCore; } }
		int IDragNodesProvider.Count { get { return dragNodesCore.Count; } }
		#endregion
	}
	class DragObjectCollection<T> : IEnumerable<T>, ICollection<T> {
		ArrayList innerList;
		public DragObjectCollection() {
			innerList = new ArrayList();
		}
		public DragObjectCollection(ICollection<T> collection)
			: this() {
			AddRange(collection);
		}
		#region IEnumerable<T> Members
		public IEnumerator<T> GetEnumerator() {
			foreach(var obj in innerList)
				yield return (T)obj;
		}
		internal void Add(T value) { innerList.Add(value); }
		internal void AddRange(ICollection<T> collection) {
			if(collection == null) return;
			foreach(T val in collection)
				Add(val);
		}
		internal void Clear() { innerList.Clear(); }
		#endregion
		#region IEnumerable Members
		IEnumerator IEnumerable.GetEnumerator() { return innerList.GetEnumerator(); }
		#endregion
		#region ICollection<T> Members
		void ICollection<T>.Add(T item) { }
		void ICollection<T>.Clear() { }
		public bool Contains(T item) { return innerList.Contains(item); }
		public void CopyTo(T[] array, int arrayIndex) { innerList.CopyTo(array, arrayIndex); }
		public int Count { get { return innerList.Count; } }
		public T this[int index] {
			get {
				if(index < 0 || index >= Count) return default(T);
				return (T)innerList[index];
			}
		}
		bool ICollection<T>.IsReadOnly { get { return true; } }
		bool ICollection<T>.Remove(T item) { return false; }
		#endregion
	}
}
namespace DevExpress.XtraTreeList.ViewInfo {
	using DevExpress.LookAndFeel.Helpers;
	public class CalcRowGroupInfoArgs {
		int index;
		Rectangle bounds;
		public CalcRowGroupInfoArgs(Rectangle bounds) {
			this.index = 0;
			this.bounds = bounds;
		}
		public void IncIndex() {
			Index++;
		}
		public void VertOffsetBounds(int offset) {
			bounds.Y += offset;
		}
		public void InflateBoundsHeight(int dy) {
			bounds.Height += dy;
		}
		public int Index { get { return index; } set { index = value; } }
		public Rectangle Bounds { get { return bounds; } }
	}
	[ToolboxItem(false)]
	public class VisibleColumnsList : ReadOnlyCollectionBase, IEnumerable<TreeListColumn> {
		public TreeListColumn this[int index] {
			get {
				if(index < Count && index > -1) return (TreeListColumn)InnerList[index];
				return null;
			}
		}
		public TreeListColumn this[string columnName] {
			get {
				foreach(TreeListColumn col in this) {
					if(col.FieldName == columnName) return col;
				}
				return null;
			}
		}
		public int IndexOf(TreeListColumn column) { return InnerList.IndexOf(column); }
		public bool Contains(TreeListColumn column) { return InnerList.Contains(column); }
		protected internal void Clear() { InnerList.Clear(); }
		protected internal void AddRange(ICollection col) { InnerList.AddRange(col); }
		protected internal void Add(TreeListColumn col) { InnerList.Add(col); }
		protected internal void Remove(TreeListColumn col) { InnerList.Remove(col); }
		protected internal void RemoveAt(int index) { InnerList.RemoveAt(index); }
		protected internal void Insert(int index, TreeListColumn col) { InnerList.Insert(index, col); }
		protected internal void Sort(IComparer comparer) { InnerList.Sort(comparer); }
		protected internal void SetColumnIndex(int newIndex, TreeListColumn column) {
			int prevIndex = IndexOf(column);
			if(newIndex < 0) newIndex = -1;
			if(newIndex > Count) newIndex = Count;
			if(prevIndex == newIndex) return;
			if(newIndex == -1) {
				InnerList.Remove(column);
			}
			else {
				if(prevIndex == -1) {
					InnerList.Insert(newIndex, column);
				}
				else {
					InnerList.Remove(column);
					if(newIndex > prevIndex) newIndex--;
					InnerList.Insert(newIndex, column);
				}
			}
		}
		protected internal bool Show(TreeListColumn column) { return Show(column, Count); }
		protected internal bool Show(TreeListColumn column, int index) {
			if(index < 0) index = 0;
			if(index >= Count) index = Count;
			int currentIndex = IndexOf(column);
			if(currentIndex == index) {
				column.SetVisibleCore(true, index);
				return false;
			}
			if(currentIndex < 0) {
				Insert(index, column);
			}
			else {
				RemoveAt(currentIndex);
				if(index > currentIndex) index--;
				Insert(index, column);
			}
			column.SetVisibleCore(true, index);
			UpdateIndexes();
			return true;
		}
		protected internal bool Hide(TreeListColumn column) {
			int index = IndexOf(column);
			if(index < 0) {
				column.SetVisibleCore(false, -1);
				return false;
			}
			RemoveAt(index);
			column.SetVisibleCore(false, -1);
			UpdateIndexes();
			return true;
		}
		protected internal void UpdateIndexes() {
			for(int n = 0; n < Count; n++) {
				this[n].SetVisibleCore(true, n);
			}
		}
		IEnumerator<TreeListColumn> GetEnumeratorCore() {
			foreach(TreeListColumn column in InnerList)
				yield return column;
		}
		IEnumerator<TreeListColumn> IEnumerable<TreeListColumn>.GetEnumerator() {
			return GetEnumeratorCore();
		}
		IEnumerator IEnumerable.GetEnumerator() {
			return GetEnumeratorCore();
		}
	}
	public class TreeListEmbeddedLookAndFeel : EmbeddedLookAndFeel {
		public TreeListEmbeddedLookAndFeel(TreeList treeList) {
			TreeList = treeList;
		}
		protected TreeList TreeList { get; private set; }
		protected ISkinProviderEx SkinProviderEx { get { return TreeList.LookAndFeel as ISkinProviderEx; } }
		public override float GetTouchScaleFactor() {
			return SkinProviderEx.GetTouchScaleFactor();
		}
		public override Color GetMaskColor() {
			return SkinProviderEx.GetMaskColor();
		}
		public override Color GetMaskColor2() {
			return SkinProviderEx.GetMaskColor2();
		}
	}	
}
