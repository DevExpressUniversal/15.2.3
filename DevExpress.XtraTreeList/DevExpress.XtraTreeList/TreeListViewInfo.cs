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
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraTreeList.Columns;
using DevExpress.XtraTreeList.Nodes;
using DevExpress.XtraTreeList.Nodes.Operations;
using DevExpress.XtraTreeList.Painter;
using DevExpress.XtraTreeList.StyleFormatConditions;
using DevExpress.XtraEditors;
using DevExpress.Utils.Drawing.Animation;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.Skins;
using DevExpress.LookAndFeel;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraTreeList.Internal;
using DevExpress.XtraTreeList.Localization;
using DevExpress.Utils.Text;
using DevExpress.XtraGrid;
using System.Collections.Generic;
using System.Linq;
using DevExpress.XtraEditors.Helpers;
namespace DevExpress.XtraTreeList.ViewInfo {
	[Flags]
	public enum TreeNodeCellState {
		Dirty = 0,
		Default	 = 0x001,
		Selected	= 0x002,
		Focused		= 0x004,
		FocusedCell	= 0x008,
		Even		= 0x010,
		Odd			= 0x020,
		TreeFocused = 0x100,
		FocusedAndTreeFocused = TreeFocused | Focused
	}
	public class TreeListCellId {
		TreeListNode node;
		TreeListColumn column;
		public TreeListCellId(TreeListNode node, TreeListColumn column) {
			this.node = node;
			this.column = column;
		}
		public TreeListNode Node { get { return node; } }
		public TreeListColumn Column { get { return column; } }
		public override bool Equals(object obj) {
			TreeListCellId id = obj as TreeListCellId;
			return node == id.node && column == id.column;
		}
		public override int GetHashCode() {
			int h1 = column.GetHashCode();
			int h2 = node.GetHashCode();
			return h1 ^ h2;
		}
	}
	public class ConditionInfo {
		Hashtable cellAppearances;
		TreeListViewInfo viewInfo;
		public ConditionInfo(TreeListViewInfo viewInfo) {
			this.viewInfo = viewInfo;
		}
		public Hashtable CellAppearances {
			get { return cellAppearances; }
			protected set { cellAppearances = value; }
		}
		protected TreeListColumnCollection Columns { get { return viewInfo.TreeList.Columns; } }
		public virtual void Reset() {
			CellAppearances = null;
		}
		public AppearanceObjectEx GetCellAppearance(TreeListColumn column) {
			if(CellAppearances == null || column == null) return null;
			return CellAppearances[column] as AppearanceObjectEx;
		}
		public void CheckCondition(StyleFormatCondition cond, TreeListNode node, TreeListColumn column) {
			object value = node.GetValue(column);
			if(cond.CheckValue(column, value, node))
				ApplyCondition(cond, cond.ApplyToRow ? null : column);
		}
		void ApplyCondition(StyleFormatCondition cond, TreeListColumn column) {
			AppearanceObjectEx appearance = cond.Appearance.Clone() as AppearanceObjectEx;
			if(CellAppearances == null)
				CellAppearances = new Hashtable();
			if(column == null) {
				for(int i = 0; i < Columns.Count; i++)
					ApplyColumnAppearance(Columns[i], appearance);
			}
			else {
				ApplyColumnAppearance(column, appearance);
			}
		}
		void ApplyColumnAppearance(TreeListColumn column, AppearanceObject appearance) {
			CellAppearances[column] = appearance; 
		}
	}
	public class FormatRuleInfo {
		public class ColumnFormatRuleInfo {
			public TreeListColumn Column { get; set; }
			public TreeListFormatRule Format { get; set; }
		}
		public FormatRuleInfo(TreeListViewInfo viewInfo) {
			ViewInfo = viewInfo;
		}
		protected Dictionary<TreeListColumn, AppearanceObjectEx> CellAppearances { get; private set; }
		protected List<TreeListColumn> FinalizedColumns { get; private set; }
		protected List<ColumnFormatRuleInfo> DrawFormatRules { get; private set; }
		protected List<ColumnFormatRuleInfo> DrawContextImageRules { get; private set; }
		protected TreeListViewInfo ViewInfo { get; private set; }
		protected TreeList TreeList { get { return ViewInfo.TreeList; } }
		protected TreeListColumnCollection Columns { get { return TreeList.Columns; } }
		public bool HasDrawFormatRules { get { return DrawFormatRules != null && DrawFormatRules.Count > 0; } }
		public bool HasDrawContextImageRules { get { return DrawContextImageRules != null && DrawContextImageRules.Count > 0; } }
		public void Reset() {
			CellAppearances = null;
			DrawFormatRules = null;
			DrawContextImageRules = null;
			FinalizedColumns = null;
		}
		public virtual AppearanceObjectEx GetCellAppearance(TreeListColumn column) {
			if(CellAppearances == null || column == null) return null;
			AppearanceObjectEx appearance;
			if(!CellAppearances.TryGetValue(column, out appearance)) return null;
			return appearance;
		}
		public virtual void CheckRule(TreeListFormatRule format, TreeListNode node, TreeListColumn column, object value) {
			if(!format.IsValid || IsFinalizedColumn(column)) return;
			format.ValueProvider.Set(node, value);
			if(format.IsFit(format.ValueProvider)) {
				if(format.ApplyToRow)
					ApplyRuleToRow(format, value);
				else
					ApplyRuleToCell(format, column, value);
			}
		}
		public virtual bool ApplyDrawFormat(GraphicsCache cache, TreeListColumn column, TreeListNode node,  Rectangle bounds, BaseEditViewInfo viewInfo, DrawAppearanceMethod originalContentPainter, AppearanceObject originalContentAppearance) {
			if(!HasDrawFormatRules) return false;
			bool disableDrawContent = false;
			foreach(ColumnFormatRuleInfo columnInfo in DrawFormatRules) {
				if(columnInfo.Column != column) continue;
				IFormatRuleDraw drawFormatRule = columnInfo.Format.Rule as IFormatRuleDraw;
				if(columnInfo.Format.Rule is IFormatRuleContextImage) 
					if(FormatRuleDrawArgs.IsSupportContextImage(viewInfo)) continue;
				columnInfo.Format.ValueProvider.Set(node, node.GetValue(column));
				drawFormatRule.DrawOverlay(new FormatRuleDrawArgs(cache, bounds, columnInfo.Format.ValueProvider) { OriginalContentAppearance = originalContentAppearance, OriginalContentPainter = originalContentPainter });
				disableDrawContent |= !drawFormatRule.AllowDrawValue;
			}
			return disableDrawContent;
		}
		public virtual bool ApplyContextImage(GraphicsCache cache, TreeListColumn column, TreeListNode node, Rectangle bounds, BaseEditViewInfo viewInfo) {
			IFormatRuleSupportContextImage supportContextImage = viewInfo as IFormatRuleSupportContextImage;
			if(supportContextImage == null) return false;
			if(TreeList.FormatRules.Count > 0 || TreeList.FormatRules.Changed) 
				supportContextImage.SetContextImage(null);
			if(!HasDrawContextImageRules) return false;
			foreach(ColumnFormatRuleInfo columnInfo in DrawContextImageRules) {
				if(columnInfo.Column != column) continue;
				var contextImage = columnInfo.Format.Rule as IFormatRuleContextImage;
				columnInfo.Format.ValueProvider.Set(node, node.GetValue(column));
				var image = contextImage.GetContextImage(new FormatRuleDrawArgs(cache, bounds, columnInfo.Format.ValueProvider));
				if(image != null) 
					supportContextImage.SetContextImage(image);
			}
			return true;
		}
		protected virtual void ApplyRuleToRow(TreeListFormatRule rule, object value) {
			for(int i = 0; i < Columns.Count; i++)
				ApplyRuleToCell(rule, Columns[i], value);
		}
		protected virtual void ApplyRuleToCell(TreeListFormatRule format, TreeListColumn column, object value) {
			IFormatRuleDraw drawRule = format.Rule as IFormatRuleDraw;
			if(drawRule != null) {
				EnsureDrawFormatRules();
				DrawFormatRules.Add(new ColumnFormatRuleInfo() { Column = column, Format = format });  
			}
			IFormatRuleContextImage contextImageRule = format.Rule as IFormatRuleContextImage;
			if(contextImageRule != null) {
				EnsureDrawContextImageRules();
				DrawContextImageRules.Add(new ColumnFormatRuleInfo() { Column = column, Format = format });
			}
			IFormatRuleAppearance appearanceRule = format.Rule as IFormatRuleAppearance;
			if(appearanceRule != null) {
				EnsureCellApperances();
				AppearanceObjectEx currentApperance, appearance = appearanceRule.QueryAppearance(new FormatRuleAppearanceArgs(format.ValueProvider, value));
				if(CellAppearances.TryGetValue(column, out currentApperance)) 
					currentApperance.Combine(appearance);
				else 
					CellAppearances[column] = appearance.Clone() as AppearanceObjectEx;
			}
			if(format.StopIfTrue) {
				EnsureFinalizedColumns();
				FinalizedColumns.Add(column);
			}
		}
		protected bool IsFinalizedColumn(TreeListColumn column) {
			if(FinalizedColumns == null) return false;
			return FinalizedColumns.Contains(column);
		}
		protected void EnsureCellApperances() {
			if(CellAppearances == null)
				CellAppearances = new Dictionary<TreeListColumn, AppearanceObjectEx>();
		}
		protected void EnsureDrawFormatRules() {
			if(DrawFormatRules == null)
				DrawFormatRules = new List<ColumnFormatRuleInfo>();
		}
		protected void EnsureDrawContextImageRules() {
			if(DrawContextImageRules == null)
				DrawContextImageRules = new List<ColumnFormatRuleInfo>();
		}
		protected void EnsureFinalizedColumns() {
			if(FinalizedColumns == null)
				FinalizedColumns = new List<TreeListColumn>();
		}
	}
	public class TreeListViewInfo : IDisposable {
		ColumnsInfo columnsInfo;
		BandsInfo bandsInfo;
		RowsInfo rowsInfo;
		SummaryFooterInfo summaryFooterInfo;
		TreeList treeList;
		internal DragScrollInfo dragInfo;
		ViewRects viewRects;
		TreeListAppearanceCollection paintAppearance;
		ResourceInfo rc;
		GraphicsInfo ginfo;
		bool isValid, paintAppearanceDirty = true;
		Region emptyAreaRegion;
		protected TreeListColumn fFixedLeftColumn, fFixedRightColumn;
		protected TreeListBand fFixedLeftBand, fFixedRightBand;
		BandInfo exFixedRightBandInfo, exFixedLeftBandInfo;
		internal Rectangle ColumnDragFrameRect = Rectangle.Empty;
		RowInfo autoFilterRowInfo;
		TreeListFilterPanelInfoArgs filterPanel;
		internal const int RowIndicatorEdgeHeight = 3;
		public static int DefaultMouseWheelScrollPixels = 300;
		TreeListPixelScrollingInfo pixelScrollingInfo;
		int bandPanelRowCount = 1, columnPanelRowCount = 1, rowLineCount = 1;
		public TreeListViewInfo(TreeList treeList) {
			this.ginfo = new GraphicsInfo();
			this.treeList = treeList;
			this.pixelScrollingInfo = new TreeListPixelScrollingInfo(TreeList);
			this.paintAppearance = new TreeListAppearanceCollection(TreeList);
			this.filterPanel = CreateFilterPanelInfo();
			this.viewRects = CreateViewRects();
			this.rc = CreateResourceCache();
			this.summaryFooterInfo = CreateSummaryFooterInfo();
			this.dragInfo = null;
			this.isValid = false;
			this.emptyAreaRegion = null;
			this.fFixedLeftColumn = this.fFixedRightColumn = null;
			this.fFixedLeftBand = this.fFixedRightBand = null;
			Clear();
		}
		protected internal bool IsRightToLeft { get { return treeList != null && treeList.IsRightToLeft; } }
		internal int Direction { get { return IsRightToLeft ? -1 : 1; } }
		protected internal TreeListPixelScrollingInfo PixelScrollingInfo { get { return pixelScrollingInfo; } }
		protected virtual TreeListFilterPanelInfoArgs CreateFilterPanelInfo() {
			return new TreeListFilterPanelInfoArgs();
		}		
		protected internal virtual void UpdateFilterPanelInfo() {
			FilterPanel.CustomizeText = TreeListLocalizer.Active.GetLocalizedString(TreeListStringId.FilterPanelCustomizeButton);
			FilterPanel.RightToLeft = IsRightToLeft;
			FilterPanel.DisplayText = TreeList.FilterPanelText;
			FilterPanel.FilterActive = TreeList.ActiveFilterEnabled;
			FilterPanel.AllowMRU = TreeList.GetAllowMRUFilterList();			
			FilterPanel.SetAppearance(PaintAppearance.FilterPanel);
			FilterPanel.ShowCustomizeButton = TreeList.GetAllowFilterEditor();
			FilterPanel.ShowCloseButton = TreeList.OptionsView.ShowFilterPanelMode != ShowFilterPanelMode.ShowAlways;
		}
		protected virtual void CalcFilterPanelInfo() {
			if(FilterPanel.Bounds.IsEmpty) return;
			UpdateFilterPanelInfo();
			FilterPanel.Graphics = GInfo.AddGraphics(null);
			try {
				FilterPanelPainter.CalcObjectBounds(FilterPanel);
			}
			finally {
				FilterPanel.Graphics = null;
				GInfo.ReleaseGraphics();
			}
		}
		protected internal virtual int GetFilterPanelHeight() {
			UpdateFilterPanelInfo();
			int height = 0;
			GInfo.AddGraphics(null);
			try {
				height = ObjectPainter.CalcObjectMinBounds(GInfo.Graphics, FilterPanelPainter, FilterPanel).Height;
			}
			finally {
				GInfo.ReleaseGraphics();
			}
			return height;
		}
		public virtual void Clear() {
			ViewRects.Clear();
			this.filterPanel.Bounds = Rectangle.Empty; 
			this.autoFilterRowInfo = null;
			this.columnsInfo = new ColumnsInfo();
			this.bandsInfo = new BandsInfo();
			this.rowsInfo = new RowsInfo();
			DestroyEmptyAreaRegion();
		}
		void DestroyEmptyAreaRegion() {
			if(EmptyAreaRegion != null) {
				EmptyAreaRegion.Dispose();
				emptyAreaRegion = null;
			}
		}
		public void Dispose() {
			RC.Dispose();
			DestroyEmptyAreaRegion();
		}
		public GraphicsInfo GInfo { get { return ginfo; } }
		public TreeList TreeList { get { return treeList; } }
		public RowInfo AutoFilterRowInfo { get { return autoFilterRowInfo; } }
		public TreeListFilterPanelInfoArgs FilterPanel { get { return filterPanel; } }
		public ObjectPainter FilterPanelPainter { get { return ElementPainters.FilterPanel; } }
		public void SetAppearanceDirty() { this.paintAppearanceDirty = true; }
		protected bool IsAutoFilterRow(RowInfo ri) { return AutoFilterRowInfo != null && ri == AutoFilterRowInfo; }
		protected virtual void UpdatePaintAppearance() {						
			paintAppearance.Combine(TreeList.Appearance, ElementPainters.GetAppearanceDefault());
			if(paintAppearance.GroupFooter.HAlignment == HorzAlignment.Default)
				paintAppearance.GroupFooter.TextOptions.HAlignment = HorzAlignment.Far;
			if(paintAppearance.FooterPanel.HAlignment == HorzAlignment.Default)
				paintAppearance.FooterPanel.TextOptions.HAlignment = HorzAlignment.Far;
			this.paintAppearance.UpdateRightToLeft(IsRightToLeft);
			this.paintAppearanceDirty = false;
			UpdateAlphaBlending();
		}
		protected virtual void UpdateAlphaBlending() {
			if(!TreeList.CanUpdatePaintAppearanceBlending) return;
			foreach(DictionaryEntry entry in TreeList.Blending.AlphaStyles) {
				AppearanceObject app = PaintAppearance.GetAppearance(entry.Key.ToString());
				if(app == null || app.BackColor.IsEmpty) continue;
				int level = (int)entry.Value;
				if(level >= 255) continue;
				app.BackColor = Color.FromArgb(level, app.BackColor);
				if(app.BackColor2 != Color.Empty) app.BackColor2 = Color.FromArgb(level, app.BackColor2);
			}
		}
		public TreeListAppearanceCollection PaintAppearance {
			get {
				if(this.paintAppearanceDirty) UpdatePaintAppearance();
				return paintAppearance;
			}
		}
		void FixedDivHitTest(TreeListHitTest ht) {
			Rectangle r = Rectangle.Empty; 
			if(HasFixedLeft) {
				r = ViewRects.FixedLeft;
				if(!IsRightToLeft) r.X = r.Right - TreeList.FixedLineWidth;				
				r.Width = TreeList.FixedLineWidth;
				if(r.Contains(ht.MousePoint))
					ht.HitInfoType = HitInfoType.FixedLeftDiv;
			}
			if(HasFixedRight) {
				r = ViewRects.FixedRight;
				if(IsRightToLeft) r.X = r.Right - TreeList.FixedLineWidth;				
				r.Width = TreeList.FixedLineWidth;				
				if(r.Contains(ht.MousePoint))
					ht.HitInfoType = HitInfoType.FixedRightDiv;
			}			
		}
		public virtual TreeListHitTest GetHitTest(Point pt) {
			TreeListHitTest ht = new TreeListHitTest();
			ht.MousePoint = pt;
			FixedDivHitTest(ht);
			if(ht.HitInfoType != HitInfoType.None) return ht;
			if(ViewRects.Caption.Contains(pt)) {
				ht.HitInfoType = HitInfoType.Caption;
				return ht;
			}
			if(ViewRects.BandPanel.Contains(pt)) {
				BandInfo bi = CalcBandHitInfo(pt);
				ht.HitInfoType = HitInfoType.BandPanel;
				ht.BandInfo = bi;
				if(ht.BandInfo != null) {
					ht.MouseDest = ht.BandInfo.Bounds;
					if(bi.Type == ColumnInfo.ColumnInfoType.ColumnButton) {
						ht.HitInfoType = HitInfoType.BandButton;
						return ht;
					}
					if(bi.Type == ColumnInfo.ColumnInfoType.BehindColumn) {
						ht.HitInfoType = HitInfoType.BehindColumn;
						return ht;
					}
					bool leftBandEdge = (pt.X >= bi.Bounds.Left) && (pt.X < (bi.Bounds.Left + ControlUtils.ColumnResizeEdgeSize / 2)),
						 rightBandEdge = (pt.X >= (bi.Bounds.Right - ControlUtils.ColumnResizeEdgeSize / 2)) && (pt.X < bi.Bounds.Right);
					if(IsRightToLeft) {
						bool tempBandEdge = leftBandEdge;
						leftBandEdge = rightBandEdge;
						rightBandEdge = tempBandEdge;
					}
					if(leftBandEdge) {
						if(bi.Band != null && (bi.Band.OwnedCollection.FirstVisibleBand == bi.Band || bi.Band.Fixed == FixedStyle.Right))
							leftBandEdge = false;
					}
					if(leftBandEdge || rightBandEdge) {
						ht.HitInfoType = HitInfoType.BandEdge;
						if(leftBandEdge && bi.Band != null) {
							TreeListBand band = bi.Band.OwnedCollection.GetVisibleBand(TreeList.GetBandVisibleIndex(bi.Band) - 1);
							if(band != null) {
								ht.BandInfo = TreeList.ViewInfo.BandsInfo.FindBand(band);
								ht.MouseDest = ht.BandInfo.Bounds;
							}
						}
						return ht;
					}
					if(bi.Type == ColumnInfo.ColumnInfoType.Column) {
						ht.HitInfoType = HitInfoType.Band;
						return ht;
					}
				}
				return ht;
			}
			if(ViewRects.ColumnPanel.Contains(pt)) {
				ColumnInfo ci = GetColumnInfoByPoint(pt, 0, ColumnsInfo);
				ht.ColumnInfo = ci;
				if(ht.ColumnInfo != null) {
					ht.MouseDest = ht.ColumnInfo.Bounds;
					if(ci.Type == ColumnInfo.ColumnInfoType.ColumnButton) {
						ht.HitInfoType = HitInfoType.ColumnButton;
						return ht;
					}
					if(ci.Type == ColumnInfo.ColumnInfoType.BehindColumn) {
						ht.HitInfoType = HitInfoType.BehindColumn;
						return ht;
					}
					ht.HitInfoType = HitInfoType.Column;
					int lastIndex = TreeList.IsAutoWidth ? 1 : 0;
					int resizeEdge = (IsRightToLeft ? ci.Bounds.Left : ci.Bounds.Right) - Direction * (ControlUtils.ColumnResizeEdgeSize + ControlUtils.ColumnResizeEdgeSize / 2);
					bool canResize = IsRightToLeft ? resizeEdge >= pt.X : resizeEdge <= pt.X;
					if(canResize && TreeList.VisibleColumns.IndexOf(ci.Column) < TreeList.VisibleColumns.Count - lastIndex) {
						ht.HitInfoType = HitInfoType.ColumnEdge;
						ht.MousePoint.X = IsRightToLeft ? ci.Bounds.Left + 1 : ci.Bounds.Right - 1;
						return ht;
					}
					if(IsInFilterButton(ci, pt)) {
						ht.HitInfoType = HitInfoType.ColumnFilterButton;
						return ht;
					}
				}
				return ht;
			}
			if(ViewRects.EmptyBehindColumn.Contains(pt) ||
				ViewRects.EmptyRows.Contains(pt)) {
				ht.HitInfoType = HitInfoType.Empty;
				ht.MouseDest = ViewRects.EmptyRows;
				return ht;
			}
			if(ViewRects.AutoFilterRow.Contains(pt)) {
				RowInfo ri = GetRowInfoByPoint(pt);
				ht.HitInfoType = HitInfoType.AutoFilterRow;
				ht.RowInfo = ri;
				ht.MouseDest = ri.Bounds;
				if(ri.IndicatorInfo.Bounds.Contains(pt)) {
					ht.HitInfoType = HitInfoType.RowIndicator;
					ht.MouseDest = ri.IndicatorInfo.Bounds;
					if(ri.IndicatorInfo.Bounds.Bottom - RowIndicatorEdgeHeight <= pt.Y) {
						ht.HitInfoType = HitInfoType.RowIndicatorEdge;
					}
					return ht;
				}
				CellInfo cell = GetRowCellInfoByPoint(ri, pt);
				if(cell != null) {
					ht.HitInfoType = HitInfoType.Cell;
					ht.CellInfo = cell;
					ht.ColumnInfo = cell.ColumnInfo;
					ht.MouseDest = cell.Bounds;
					return ht;
				}
			} 
			if(ViewRects.Rows.Contains(pt)) {
				RowInfo ri = GetRowInfoByPoint(pt);
				if(ri != null) {
					ht.HitInfoType = HitInfoType.Row;
					ht.RowInfo = ri;
					ht.MouseDest = ri.Bounds;
					if(!ViewRects.FixedRight.IsEmpty && ViewRects.FixedRight.Contains(pt)) {
						CellInfo fixedColumnCell = GetRowCellInfoByPoint(ri, pt);
						if(fixedColumnCell != null) {
							ht.HitInfoType = HitInfoType.Cell;
							ht.CellInfo = fixedColumnCell;
							ht.ColumnInfo = fixedColumnCell.ColumnInfo;
							ht.MouseDest = fixedColumnCell.Bounds;
							return ht;
						}
					}
					if(ri.IndicatorInfo.Bounds.Contains(pt)) {
						ht.HitInfoType = HitInfoType.RowIndicator;
						ht.MouseDest = ri.IndicatorInfo.Bounds;
						if(ri.IndicatorInfo.Bounds.Bottom - RowIndicatorEdgeHeight <= pt.Y) {
							ht.HitInfoType = HitInfoType.RowIndicatorEdge;
						}
						return ht;
					}
					if(ri.ButtonBounds.Contains(pt)) {
						ht.HitInfoType = HitInfoType.Button;
						ht.MouseDest = ri.ButtonBounds;
						return ht;
					}
					if(ri.IndentInfo.Bounds.Contains(pt)) {
						ht.MouseDest = ri.IndentInfo.Bounds; 
						if(TreeList.OptionsView.ShowIndentAsRowStyle) 
							ht.HitInfoType = HitInfoType.RowIndent;
						else 
							ht.HitInfoType = HitInfoType.Empty;
						return ht;
					}
					if(ri.PreviewBounds.Contains(pt)) {
						ht.HitInfoType = HitInfoType.RowPreview;
						ht.MouseDest = ri.PreviewBounds;
						return ht;
					}
					if(ri.SelectImageBounds.Contains(pt)) {
						ht.HitInfoType = HitInfoType.SelectImage;
						ht.MouseDest = ri.SelectImageBounds;
						return ht;
					}
					if(ri.StateImageBounds.Contains(pt)) {
						ht.HitInfoType = HitInfoType.StateImage;
						ht.MouseDest = ri.StateImageBounds;
						return ht;
					}
					if(ri.CheckBounds.Contains(pt)) {
						ht.HitInfoType = HitInfoType.NodeCheckBox;
						ht.MouseDest = ri.CheckBounds;
						return ht;
					}
					CellInfo cell = GetRowCellInfoByPoint(ri, pt);
					if(cell != null) {
						ht.HitInfoType = HitInfoType.Cell;
						ht.CellInfo = cell;
						ht.ColumnInfo = cell.ColumnInfo;
						ht.MouseDest = cell.Bounds;
						return ht;
					}
				}
				else {
					RowFooterInfo fi = GetRowFooterInfoByPoint(pt);
					if (fi == null) return ht; 
					ht.HitInfoType = HitInfoType.RowFooter;
					ht.RowFooterInfo = fi;
					ht.MouseDest = fi.Bounds;
					if(fi.IndentInfo.Bounds.Contains(pt)) {
						ht.MouseDest = fi.IndentInfo.Bounds;
						if(!TreeList.OptionsView.ShowIndentAsRowStyle) 
							ht.HitInfoType = HitInfoType.Empty; 
						return ht;
					}
					FooterItem fItem = GetFooterItemByPoint(pt, fi);
					if(fItem != null) {
						ht.MouseDest = fItem.ItemBounds;
						ht.FooterItem = fItem;
						return ht;
					}
				}
				return ht;
			}
			if(ViewRects.Footer.Contains(pt)) {
				ht.HitInfoType = HitInfoType.SummaryFooter;
				ht.MouseDest = ViewRects.Footer;
				FooterItem fItem = GetFooterItemByPoint(pt, SummaryFooterInfo);
				if(fItem != null) {
					ht.MouseDest = fItem.ItemBounds;
					ht.FooterItem = fItem;
					return ht;
				}
				return ht;
			}
			if(FilterPanel.Bounds.Contains(pt)) {
				ht.HitInfoType = HitInfoType.FilterPanel;
				ht.MouseDest = FilterPanel.Bounds;
				if(FilterPanel.TextBounds.Contains(pt)) {
					ht.HitInfoType = HitInfoType.FilterPanelText;
					ht.MouseDest = FilterPanel.TextBounds;
					return ht;
				}
				if(FilterPanel.CloseButtonInfo.Bounds.Contains(pt)) {
					ht.HitInfoType = HitInfoType.FilterPanelCloseButton;
					ht.MouseDest = FilterPanel.CloseButtonInfo.Bounds;
					return ht;
				}
				if(FilterPanel.CustomizeButtonInfo.Bounds.Contains(pt)) {
					ht.HitInfoType = HitInfoType.FilterPanelCustomizeButton;
					ht.MouseDest = FilterPanel.CustomizeButtonInfo.Bounds;
					return ht;
				}
				if(FilterPanel.ActiveButtonInfo.Bounds.Contains(pt)) {
					ht.HitInfoType = HitInfoType.FilterPanelActiveButton;
					ht.MouseDest = FilterPanel.ActiveButtonInfo.Bounds;
					return ht;
				}
				if(FilterPanel.MRUButtonInfo.Bounds.Contains(pt)) {
					ht.HitInfoType = HitInfoType.FilterPanelMRUButton;
					ht.MouseDest = FilterPanel.MRUButtonInfo.Bounds;
					return ht;
				}
			}
			return ht;
		}
		bool IsInFilterButton(ColumnInfo ci, Point pt) {
			foreach(DrawElementInfo info in ci.InnerElements) {
				if(!info.Visible) continue;
				if(!(info.ElementInfo is TreeListFilterButtonInfoArgs)) continue;
				if(info.ElementInfo.Bounds.Contains(pt)) 
					return true;
			}
			return false;
		}
		protected virtual CellInfo GetRowCellInfoByPoint(RowInfo ri, Point pt) {
			if(HasFixedLeft || HasFixedRight) {
				CellInfo ci = GetRowCellInfoByPoint(ri, pt, true);
				if(ci != null) return ci;
			}
			return GetRowCellInfoByPoint(ri, pt, false);
		}
		protected virtual CellInfo GetRowCellInfoByPoint(RowInfo ri, Point pt,bool onlyFixedColumns) {
			foreach(CellInfo ci in ri.Cells) {
				if(onlyFixedColumns && !IsFixedLeftPaint(ci.ColumnInfo) && !IsFixedRightPaint(ci.ColumnInfo)) continue;
				if(ci.Bounds.Contains(pt)) return ci;
			}
			return null;
		}
		protected virtual FooterItem GetFooterItemByPoint(Point pt, SummaryFooterInfo fi) {
			if(HasFixedLeft || HasFixedRight) {
				FooterItem fci = GetFooterItemByPoint(pt, true, fi);
				if(fci != null) return fci;
			}
			return GetFooterItemByPoint(pt, false, fi);
		}
		protected virtual FooterItem GetFooterItemByPoint(Point pt, bool onlyFixedColumns, SummaryFooterInfo footerInfo) {
			foreach(FooterItem fi in footerInfo.FooterItems) {
				if(onlyFixedColumns && !IsFixedLeftPaint(ColumnsInfo[fi.Column]) && !IsFixedRightPaint(ColumnsInfo[fi.Column])) continue;
				if(fi.ItemBounds.Contains(pt)) return fi;
			}
			return null;
		}
		public virtual void CalcViewInfo() {
			Clear();
			UpdateFixedColumnInfo();
			GInfo.AddGraphics(null);
			try {
				if(SummaryFooterInfo.NeedsRecalcAll)
					TreeList.FormatRules.UpdateStateValues();
				CalcColumnPanelRowCount();
				CreateResources();
				CalcViewRects();
				CalcColumnsInfo();
				CalcBandPanelRowCount();
				CalcBandsInfo();
				UpdateViewRects(); 
				UpdateFixedRects();
				CalcFilterPanelInfo();
				CalcRowsInfo();
				CalcSummaryFooterInfo();
				CalcEmptyAreaRegion();
				IsValid = true;
			} 
			finally {
				GInfo.ReleaseGraphics();
			}
		}
		protected virtual void UpdateViewRects() {
			CalcViewRects();
			ColumnsInfo.Columns.Clear();
			BandsInfo.Clear();
			CalcColumnsInfo();
			CalcBandsInfo();
		}
		public virtual void CreateResources() {
			if(!RC.NeedsRestore) return;
			RC.SetLevelWidth(TreeList.TreeLevelWidth);
			LineBrushChanged(TreeList.TreeLineStyle, PaintAppearance.TreeLine);
			RC.StateImageSize = GetImageSize(TreeList.StateImageList);
			RC.SelectImageSize = GetImageSize(TreeList.SelectImageList);
			RC.ColumnImageSize = GetImageSize(TreeList.ColumnsImageList);
			if(RC.StateImageSize.Width + RC.SelectImageSize.Width > 0) {
				const int firstCellLeftVertLineIndent = 1;
				if(RC.SelectImageSize != Size.Empty) RC.SelectImageSize.Width += firstCellLeftVertLineIndent;
				else RC.StateImageSize.Width += firstCellLeftVertLineIndent;
			}
			RC.CheckBoxSize = GetCheckBoxSize();
			if(RC.CheckBoxSize != Size.Empty)
				RC.CheckBoxSize.Width++ ; 
			CalcLayoutHeights();
			RC.NeedsRestore = false;
		}
		Size GetImageSize(object imageList) { return ImageCollection.GetImageListSize(imageList); }
		protected virtual ResourceInfo CreateResourceCache() { return new ResourceInfo(this); }
		protected virtual ViewRects CreateViewRects() { return new ViewRects(this); }
		protected virtual SummaryFooterInfo CreateSummaryFooterInfo() { return new SummaryFooterInfo(); }
		protected virtual void CalcViewRects() {
			CalcColumnTotalWidth();
			ViewRects.Window = CalcWindowRect(TreeList.ClientRectangle);
			ViewRects.ScrollArea = CalcScrollRect(ViewRects.Window);
			if(TreeList.OptionsView.ShowCaption) { 
				ViewRects.Caption = ViewRects.ScrollArea;
				ViewRects.Caption.Height = CalcCaptionHeight(ViewRects.Caption);
				ViewRects.ScrollArea.Y = ViewRects.Caption.Bottom;
				ViewRects.ScrollArea.Height -= ViewRects.Caption.Height;
			}
			if(ShowFilterPanel) {
				Rectangle panelRect = ViewRects.ScrollArea;
				panelRect.Height = GetFilterPanelHeight();
				panelRect.Y = ViewRects.ScrollArea.Bottom - panelRect.Height;
				ViewRects.ScrollArea.Height -= panelRect.Height;
				FilterPanel.Bounds = panelRect;
			}
			ViewRects.FindPanel = CalcFindPanelRect(ViewRects.ScrollArea);
			if(!ViewRects.FindPanel.IsEmpty) {
				ViewRects.ScrollArea.Y = ViewRects.FindPanel.Bottom;
				ViewRects.ScrollArea.Height -= ViewRects.FindPanel.Height;
			}
			ViewRects.Client = CalcClientRect(ViewRects.ScrollArea);
			ViewRects.BandPanel = ViewRects.Client;
			ViewRects.BandPanel.Height = BandPanelRowHeight * BandPanelRowCount;
			ViewRects.ScrollArea.Height -= ViewRects.BandPanel.Height;
			ViewRects.ColumnPanel = ViewRects.Client;
			if(TreeList.ActualShowBands)
				ViewRects.ColumnPanel.Y = ViewRects.BandPanel.Bottom;
			ViewRects.ColumnPanel.Height = ColumnPanelHeight * ColumnPanelRowCount;
			ViewRects.Footer = ViewRects.ScrollArea;
			ViewRects.Footer.Y = ViewRects.Client.Bottom - FooterPanelHeight;
			ViewRects.Footer.Height = FooterPanelHeight;
			ViewRects.ClipRectangle = CalcClipRect(ViewRects.Client);
		}
		public virtual int GetColumnLeftCoord(TreeListColumn column) {
			if(column.VisibleIndex < 0) return 0;
			int res = 0;
			if(column.ParentBand != null) {
				if(!column.ParentBand.Visible) return 0;
				TreeListBand rootBand = column.ParentBand.RootBand;
				if(!rootBand.Visible) return 0;
				foreach(TreeListBand band in TreeList.Bands) {
					if(!band.Visible) continue;
					if(band == rootBand) {
						if(column.ParentBand == rootBand) 
							break;
						TreeListBand bnd = column.ParentBand;
						while(bnd.ParentBand != null) {
							int index = bnd.Index;
							for(int n = 0; n < index; n++) {
								TreeListBand child = bnd.ParentBand.Bands[n];
								if(TreeList.GetBandVisibleIndex(child) < 0) continue;
								res += child.VisibleWidth;
							}
							bnd = bnd.ParentBand;
						}
						break;
					}
					res += band.VisibleWidth;
				}
				foreach(TreeListColumn col in column.ParentBand.Columns) {
					if(col == column) break;
					if(col.VisibleIndex < 0) continue;
					res += col.VisibleWidth;
				}
				return res;
			}
			else {
				foreach(TreeListColumn col in TreeList.VisibleColumns) {
					if(col == column) return res;
					res += col.VisibleWidth;
				}
				return res;
			}
		}
		public virtual TreeListColumn FixedLeftColumn { get { return fFixedLeftColumn; } }
		public virtual TreeListColumn FixedRightColumn { get { return fFixedRightColumn; } }
		public virtual TreeListBand FixedLeftBand { get { return fFixedLeftBand; } }
		public virtual TreeListBand FixedRightBand { get { return fFixedRightBand; } }
		public virtual bool HasFixedLeft { get { return FixedLeftColumn != null || HasFixedLeftBand; } }
		public virtual bool HasFixedRight { get { return FixedRightColumn != null || HasFixedRightBand; } }
		public virtual bool HasFixedLeftBand { get { return FixedLeftBand != null; } }
		public virtual bool HasFixedRightBand { get { return FixedRightBand != null; } }
		public virtual bool HasFixedColumns {
			get { return FixedRightColumn != null || FixedLeftColumn != null || HasFixedBands; }
		}
		public virtual bool HasFixedBands {
			get { return FixedRightBand != null || FixedLeftBand != null; }
		}
		public virtual bool IsFixedRight(ColumnInfo ci) {
			if(ci == null) return false;
			if(fFixedRightBand != null && ci.ParentBandInfo != null)
				if(IsFixedColumnMultiRow(ci, exFixedRightBandInfo, ColumnInfo.ColumnPosition.Left)) 
					return true;
			return ci.Column != null && ci.Column == FixedRightColumn;
		}			
		bool IsFixedColumnMultiRow(ColumnInfo ci, BandInfo fixedBandInfo, ColumnInfo.ColumnPosition position) {
			if(fixedBandInfo == null) return false;
			if(ci.ParentBandInfo == null) return false;
			if(ci.ParentBandInfo != fixedBandInfo) return false;
			if(fixedBandInfo == null || fixedBandInfo.Columns == null) return false;
			if(fixedBandInfo.Columns.IndexOf(ci) < 0) return false;
			if(ci.Type == ColumnInfo.ColumnInfoType.EmptyColumn) return true;
			return ci.Position == position || ci.Position == ColumnInfo.ColumnPosition.Single;
		}
		public virtual bool IsFixedLeft(ColumnInfo ci) {
			if(ci == null) return false;
			if(fFixedLeftBand != null)
				if(IsFixedColumnMultiRow(ci, exFixedLeftBandInfo, ColumnInfo.ColumnPosition.Right)) 
					return true;
			return ci.Column != null && ci.Column == FixedLeftColumn;
		}
		public virtual bool IsFixedLeftPaint(ColumnInfo ci) {
			if(ci != null) {
				if(ci.ParentBandInfo != null) 
					return ci.ParentBandInfo.Fixed == FixedStyle.Left;
				return ci.Column != null && ci.Column.Fixed == FixedStyle.Left;
			}
			return false;
		}
		public virtual bool IsFixedRightPaint(ColumnInfo ci) {
			if(ci != null) {
				if(ci.ParentBandInfo != null) 
					return ci.ParentBandInfo.Fixed == FixedStyle.Right;
				return ci.Column != null && ci.Column.Fixed == FixedStyle.Right;
			}
			return false;
		}
		public virtual bool IsFixed(ColumnInfo ci) {
			if(ci != null) {
				if(ci.ParentBandInfo != null)
					return ci.ParentBandInfo.Fixed != FixedStyle.None;
				return ci.Column != null && ci.Column.Fixed != FixedStyle.None;
			}
			return false;
		}
		protected internal virtual TreeListColumn FindFixedLeftColumn() {
			if(FixedLeftColumn == null || ColumnsInfo[FixedLeftColumn] != null) return FixedLeftColumn;
			int index = TreeList.VisibleColumns.IndexOf(FixedLeftColumn);
			if(index < 1) return FixedLeftColumn;
			while(index > 0) {
				TreeListColumn column = TreeList.VisibleColumns[--index];
				if(ColumnsInfo[column] != null) return column;
			}
			return FixedLeftColumn;
		}
		Rectangle CalcFixedLeft(IHeaderObjectInfo info){		  
			Rectangle r = ViewRects.Client;			
			if(IsRightToLeft) {
				r.X = info.Bounds.Left - TreeList.FixedLineWidth;
				r.Width = ViewRects.Client.Right - r.X - ViewRects.IndicatorWidth;
			}
			else {
				r.X += ViewRects.IndicatorWidth;
				r.Width = info.Bounds.Right + TreeList.FixedLineWidth - r.Left;
			}
			if(r.Width < 0) r.Width = 0;
			return r;
		}
		Rectangle CalcFixedRight(IHeaderObjectInfo info) {		   
			Rectangle r = ViewRects.Client;
			if(IsRightToLeft) {
				r.Width = (info.Bounds.Right + TreeList.FixedLineWidth) - r.Left;
				if(ViewRects.FixedLeft.IntersectsWith(r))
					r.Width = ViewRects.FixedLeft.Left - r.Left + TreeList.FixedLineWidth;
			}
			else {
				r.X = info.Bounds.Left - TreeList.FixedLineWidth;
				if(ViewRects.FixedLeft.Contains(r.Location))
					r.X = ViewRects.FixedLeft.Right - TreeList.FixedLineWidth;
				r.Width = ViewRects.Client.Right - r.X;
			}
			if(r.Width < 0) r.Width = 0;
			return r;
		}
		Rectangle CalcFixedBounds(IHeaderObjectInfo info, bool isLeft) {
			if(info == null) return Rectangle.Empty;			
			if(isLeft) return CalcFixedLeft(info);
			return CalcFixedRight(info);
		}
		protected virtual void UpdateFixedRects() {
			ViewRects.FixedLeft = ViewRects.FixedRight = Rectangle.Empty;
			Rectangle r = Rectangle.Empty;
			BandInfo bi;
			if(FixedLeftBand != null) {
				bi = BandsInfo[FixedLeftBand];
				ViewRects.FixedLeft = CalcFixedBounds(bi, true);
			}
			if(FixedRightBand != null) {
				bi = BandsInfo[FixedRightBand];
				ViewRects.FixedRight = CalcFixedBounds(bi, false);
			}
			if(TreeList.HasBands) return;
			ColumnInfo ci;
			if(FixedLeftColumn != null) {
				ci = ColumnsInfo[FindFixedLeftColumn()];
				ViewRects.FixedLeft = CalcFixedBounds(ci, true);
			}
			if(FixedRightColumn != null) {
				ci = ColumnsInfo[FixedRightColumn];
				ViewRects.FixedRight = CalcFixedBounds(ci, false);
			}
		}
		public virtual void UpdateFixedColumnInfo() {
			this.fFixedLeftColumn = this.fFixedRightColumn = null;
			this.fFixedLeftBand = this.fFixedRightBand = null;
			this.exFixedLeftBandInfo = this.exFixedRightBandInfo = null;
			foreach(TreeListBand band in TreeList.Bands) {
				if(!band.Visible) continue;
				if(band.Fixed == FixedStyle.Left) {
					this.fFixedLeftBand = band;
				}
				if(this.fFixedRightBand == null && band.Fixed == FixedStyle.Right) {
					this.fFixedRightBand = band;
				}
			}
			if(treeList.HasBands) return;
			foreach(TreeListColumn column in TreeList.VisibleColumns) {
				if(column.Fixed == FixedStyle.Left) {
					this.fFixedLeftColumn = column;
				}
				if(this.fFixedRightColumn == null && column.Fixed == FixedStyle.Right) {
					this.fFixedRightColumn = column;
				}
			}
		}
		public virtual Rectangle UpdateFixedRange(Rectangle rect, ColumnInfo ci) {
			if(ci != null && IsFixedLeftPaint(ci)) return rect;
			return UpdateFixedRange(rect, (ci == null || !IsFixedRightPaint(ci)));
		}
		public virtual Rectangle UpdateFixedRange(Rectangle rect, BandInfo bi) {
			if(bi != null && bi.Fixed == FixedStyle.Left) return rect;
			return UpdateFixedRange(rect, (bi == null || bi.Fixed != FixedStyle.Right));
		}
		public Rectangle UpdateFixedRange(Rectangle rect) { return UpdateFixedRange(rect, true); }  
		Rectangle CalcFixedLeftRange(Rectangle lFixed, Rectangle rect) {
			Rectangle res = rect;
			if(rect.X < lFixed.Right) {
				res.X = lFixed.Right;
				res.Width = Math.Max(rect.Right - res.X, 0);
			}
			return res;
		}
		Rectangle CalcFixedRightRange(Rectangle rFixed, Rectangle rect) {
			Rectangle res = rect;
			if(rect.Left > rFixed.Left) {
				res.Width = 0;
				return res;
			}
			if(rect.Right > rFixed.Left) {
				res.Width = Math.Max(rFixed.Left - rect.X, 0);
			}
			return res;
		}
		Rectangle CalcFixedRange(Rectangle hFixed, Rectangle rect, bool isLeft) {
			isLeft = IsRightToLeft ? !isLeft : isLeft;
			if(isLeft) return CalcFixedLeftRange(hFixed, rect);
			return CalcFixedRightRange(hFixed, rect);
		}
		protected virtual Rectangle UpdateFixedRange(Rectangle rect, bool checkRight) {			
			Rectangle l = ViewRects.FixedLeft, 
				r = ViewRects.FixedRight, res = rect;
			if(l.IsEmpty && r.IsEmpty) return rect;			
			if(!l.IsEmpty) {
				res = CalcFixedRange(l, res, true);
				if(res.Width == 0) return res;
			}			
			if(!r.IsEmpty && checkRight) {
				res = CalcFixedRange(r, res, false);
				if(res.Width == 0) return res;
			}
			if(res.Right > ViewRects.ActualColumnPanel.Right) 
				res.Width = Math.Max(0, ViewRects.ActualColumnPanel.Right - res.Left);
			return res;
		}
		protected virtual TreeNodeCellState CalcRowState(RowInfo ri) {
			TreeNodeCellState state = TreeNodeCellState.Default;
			if(TreeList.HasFocus) state |= TreeNodeCellState.TreeFocused;
			if(TreeList.FocusedNode == ri.Node) state |= TreeNodeCellState.Focused;
			if(TreeList.Selection.Contains(ri.Node) && !TreeList.IsCellSelect) state |= TreeNodeCellState.Selected;
			if(!IsAutoFilterRow(ri))
				state |= (IsEven(ri.VisibleIndex) ? TreeNodeCellState.Even : TreeNodeCellState.Odd);
			else {
				state |= TreeNodeCellState.Even | TreeNodeCellState.Odd;
			}
			return state;
		}
		protected virtual TreeNodeCellState CalcRowCellState(CellInfo cell) {
			TreeNodeCellState state = cell.RowInfo.RowState;
			if(cell.Column != null && cell.Column == TreeList.FocusedColumn && cell.RowInfo.Node == TreeList.FocusedNode)
				state |= TreeNodeCellState.FocusedCell;
			if(TreeList.IsCellSelect && TreeList.IsCellSelected(cell.RowInfo.Node, cell.Column)) state |= TreeNodeCellState.Selected;
			return state;
		}
		bool IsEven(int value) { return (value % 2 != 0); }
		internal AppearanceObject[] GetRowMaxHeightAppearances(TreeNodeCellState oddeven) {
			return GetRowMixAppearances(TreeNodeCellState.Focused | TreeNodeCellState.Selected | oddeven,
				null, PaintAppearance.FocusedCell, null);
		}
		protected internal virtual AppearanceObject[] GetRowMixAppearances(TreeNodeCellState state, AppearanceObjectEx column, AppearanceObject focusedCell, AppearanceObjectEx conditionAppearance) {
			return GetRowMixAppearancesCore(state, column, focusedCell, PaintAppearance.FocusedRow, PaintAppearance.SelectedRow, conditionAppearance);
		}
		protected internal virtual bool IsTransparentFocusedStyle { get { return PaintAppearance.FocusedRow.BackColor.A != 255; } }
		protected internal virtual bool IsTransparentSelectedStyle { get { return PaintAppearance.SelectedRow.BackColor.A != 255 || IsTransparentFocusedStyle; } }
		protected internal virtual bool IsTransparentHideSelectionStyle { get { return PaintAppearance.HideSelectionRow.BackColor.A != 255; } }
		protected virtual AppearanceObject[] GetRowMixAppearancesCore(TreeNodeCellState state, AppearanceObjectEx column, AppearanceObject focusedCell, AppearanceObject focused, AppearanceObject selected, AppearanceObjectEx conditionAppearance) {
			AppearanceObject even = PaintAppearance.EvenRow, odd = PaintAppearance.OddRow,
				row = PaintAppearance.Row;
			AppearanceObjectEx columnHigh, columnLow;
			columnHigh = columnLow = null;
			if(!TreeList.OptionsSelection.EnableAppearanceFocusedRow) focused = null;
			if(!TreeList.OptionsSelection.EnableAppearanceFocusedCell || TreeList.IsLookUpMode) focusedCell = null;
			if((state & TreeNodeCellState.Even) != 0) {
				odd = null;
				if(!IsEnableAppearanceEvenRow)
					even = null;
			}
			if((state & TreeNodeCellState.Odd) != 0) {
				even = null;
				if(!IsEnableAppearanceOddRow)
					odd = null;
			}
			if(column != null) {
				if(column.Options.HighPriority)
					columnHigh = column;
				else
					columnLow = column;
			}
			if((state & TreeNodeCellState.TreeFocused) == 0) selected = PaintAppearance.HideSelectionRow;
			if((state & TreeNodeCellState.Selected) == 0) selected = null;
			if((state & TreeNodeCellState.Focused) != 0) {
				if(TreeList.IsCellSelect && (state & TreeNodeCellState.Selected) != 0) {
					focused = focusedCell = null;
				}
				else {
					selected = null;
				}
				if((state & TreeNodeCellState.TreeFocused) == 0) {
					if(TreeList.HasFocus)
						focused = null;
					else {
						if(focused != null) focused = PaintAppearance.HideSelectionRow;
					}
				}
				if(TreeList.OptionsSelection.InvertSelection) {
					focused = selected = null;
				}
				if(TreeList.OptionsSelection.MultiSelect && (state & TreeNodeCellState.Selected) == 0) focused = null;
			} 
			else {
				focused = null;
			}
			if(IsTransparentFocusedStyle) {
				if(!(focused == PaintAppearance.HideSelectionRow && !IsTransparentHideSelectionStyle)) focused = null;
			}
			if(IsTransparentSelectedStyle) {
				if(!(selected == PaintAppearance.HideSelectionRow && !IsTransparentHideSelectionStyle)) selected = null;
			}
			if (conditionAppearance != null){
				if (conditionAppearance.Options.HighPriority)
					return new AppearanceObject[] { columnHigh, conditionAppearance, focusedCell, focused, columnLow, selected, even, odd, row};
				else return new AppearanceObject[] { columnHigh, focusedCell, focused,conditionAppearance, columnLow, selected, even, odd, row};	
			}
			return new AppearanceObject[] { columnHigh, focusedCell, focused, columnLow, selected, even, odd, row};
		}
		protected internal virtual AppearanceObject GetRowTransparentAppearance(RowInfo ri) {
			AppearanceObject res = PaintAppearance.FocusedRow;
			if(TreeList.OptionsSelection.MultiSelect) {
				if(!IsTransparentSelectedStyle) return null;
				if((ri.RowState & TreeNodeCellState.Focused) == 0) res = PaintAppearance.SelectedRow;
				if((ri.RowState & TreeNodeCellState.TreeFocused) == 0) res = PaintAppearance.HideSelectionRow;
			}
			else {
				if((ri.RowState & TreeNodeCellState.TreeFocused) == 0) res = PaintAppearance.HideSelectionRow;
			}
			if(res != null && res.BackColor.A == 255) return null;
			return res;
		}
		protected internal virtual bool IsFirstBand(TreeListBand band) {
			if(band == null) return false;
			while(band != null) {
				if(TreeList.GetBandVisibleIndex(band) != 0) return false;
				band = band.ParentBand;
			}
			return true;
		}
		protected virtual bool IsLastBand(TreeListBand band) {
			while(band != null) {
				if(TreeList.GetBandVisibleIndex(band) != band.OwnedCollection.Count - 1) return false;
				band = band.ParentBand;
			}
			return true;
		}
		public virtual void CalcBandColumnsInfo(BandInfo bi) {
			int bandEdge = IsRightToLeft ? bi.Bounds.Right : bi.Bounds.Left,
				top = ViewRects.ColumnPanel.Top;
			int childCount = 0;
			if(!bi.HasChildren && bi.Band != null && bi.Band.Columns.Count > 0) {
				int cIndex = 0;
				for(int n = 0; n < bi.Band.Columns.Count; n++) {
					TreeListColumn column = bi.Band.Columns[n];
					if(column.VisibleIndex < 0) continue;
					ColumnInfo ci = CreateColumnInfo(column);
					Rectangle bounds = GetHeaderBounds(bandEdge, top, column.VisibleWidth, ColumnPanelHeight);
					UpdateColumnPosition(ci, n, bi.Band.Columns.Count);
					bandEdge += CalcBandColumnInfo(ci, bi, bounds, ref cIndex);
					childCount++;
				}
			}
			if(childCount > 0) return;
			CalcBandEmptyColumnInfo(bi, GetHeaderBounds(bandEdge, top, bi.Bounds.Width, ColumnPanelHeight));
		}
		public virtual void CalcBandsColumnInfoMultiRow(BandInfo bi) {
			int bandEdge = IsRightToLeft ? bi.Bounds.Right : bi.Bounds.Left, top = ViewRects.ColumnPanel.Top;
			int childCount = 0;
			if(!bi.HasChildren && bi.Band != null && bi.Band.Columns.Count > 0) {
				TreeListBandRowCollection rows = TreeList.GetBandRows(bi.Band);
				int currentRowCount = 0;
				for(int i = 0; i < rows.Count; i++) {
					TreeListBandRow row = rows[i];
					if(row.Columns.Count == 0) continue;
					int rowEdge = bandEdge;
					int cIndex = 0;
					int maxRowCount = 0;
					bool isLastRow = (i == rows.Count - 1);
					for(int n = 0; n < row.Columns.Count; n++) {
						TreeListColumn column = row.Columns[n];
						Rectangle emptyBounds = Rectangle.Empty;
						int rowCount = 0;
						if(column.VisibleIndex < 0) continue;
						if(column.AutoFill) {
							if(isLastRow)
								rowCount = ColumnPanelRowCount - currentRowCount;
							else
								rowCount = row.TotalRowCount;
						}
						else {
							rowCount = column.RowCount;
							if(isLastRow && currentRowCount + rowCount < ColumnPanelRowCount)
								emptyBounds = GetHeaderBounds(rowEdge, top, column.VisibleWidth, ColumnPanelHeight * (ColumnPanelRowCount - rowCount - currentRowCount));
						}
						maxRowCount = Math.Max(maxRowCount, rowCount);
						ColumnInfo ci = CreateColumnInfo(column);
						UpdateColumnPosition(ci, n, row.Columns.Count);
						Rectangle bounds = GetHeaderBounds(rowEdge, top, column.VisibleWidth, rowCount * ColumnPanelHeight);
						rowEdge += CalcBandColumnInfo(ci, bi, bounds, ref cIndex, currentRowCount, rowCount);
						childCount++;
						if(!emptyBounds.IsEmpty) {
							UpdateColumnPosition(ci, n, row.Columns.Count);
							bandEdge += CalcBandEmptyColumnInfo(bi, emptyBounds, rowCount - currentRowCount, ColumnPanelRowCount - rowCount - currentRowCount);
							childCount++;
						}
					}
					currentRowCount += maxRowCount;
					top += ColumnPanelHeight * row.TotalRowCount;
				}
			}
			if(childCount > 0) return;			
			CalcBandEmptyColumnInfo(bi, GetHeaderBounds(bandEdge, top, bi.Bounds.Width, ColumnPanelHeight * ColumnPanelRowCount), 0, ColumnPanelRowCount);
		}
		void RegisterColumnInfo(ColumnInfo ci, BandInfo bi = null) {
			if(ci.Bounds.Width > 0) {
				if(bi != null) bi.Columns.Columns.Add(ci);
				ColumnsInfo.Columns.Add(ci);
				if(ci.Type == ColumnInfo.ColumnInfoType.Column) {
					ColumnWidthInfo cw = new ColumnWidthInfo(ci.Column);
					cw.Width -= RC.vlw + 2 * CellTextIndent;
					ColumnsInfo.CellWidthes.Add(cw);
				}
			}
		}
		int CalcBandColumnInfo(ColumnInfo ci, BandInfo bi, Rectangle bounds, ref int cIndex, int startRow = 0, int rowCount = 1) {
			if(IsFirstBand(bi.Band))
				ci.CellIndex = cIndex++;
			ci.ParentBandInfo = bi;
			ci.IsTopMost = false;
			ci.RowCount = rowCount;
			ci.StartRow = startRow;
			int result = CalcColumnInfo(ci, bounds, false);
			RegisterColumnInfo(ci, bi);
			return result;
		}
		int CalcBandEmptyColumnInfo(BandInfo bi, Rectangle bounds, int startRow = 0, int rowCount = 1) {
			ColumnInfo ci = CreateColumnInfo(null);
			ci.Type = ColumnInfo.ColumnInfoType.EmptyColumn;
			int cIndex = 0;
			return CalcBandColumnInfo(ci, bi, bounds, ref cIndex, startRow, rowCount);
		}
		void UpdateColumnPosition(ColumnInfo ci, int index, int count) {
			if(count == 1)
				ci.Position = ColumnInfo.ColumnPosition.Single;
			else if(index == count - 1)
				ci.Position = ColumnInfo.ColumnPosition.Right;
			else if(index == 0)
				ci.Position = ColumnInfo.ColumnPosition.Left;
		}	   
		protected int GetLastColumnWidth(int edge, Rectangle actualBounds) {
			int width = 0;
			if(IsRightToLeft) {
				if(edge < actualBounds.Left) return -1;
				width = edge - actualBounds.Left;
			}
			else {
				if(edge >= actualBounds.Right) return -1;
				width = actualBounds.Right - edge;
			}
			if(width == 1) width = 0;
			return width;
		}
		protected void CheckOffsetFixedRightHeader(ref int edge, IHeaderObject header, IHeaderObject fixedHeader, Rectangle bounds) {
			if(header == null || header != fixedHeader) return;
			int columnPanelEdge = IsRightToLeft ? bounds.Left : bounds.Right;
			edge += Direction * TreeList.FixedLineWidth;
			int w = Direction * CalcRestHeaderWidth(header);
			if(Direction * (edge + w) > Direction * columnPanelEdge)
				edge = columnPanelEdge - w;
		}
		protected void CheckOffsetFixedLeftHeader(ref int edge, IHeaderObject header, IHeaderObject fixedHeader) {
			if(header == null || header != fixedHeader) return;
			edge += Direction * TreeList.FixedLineWidth;
		}
		Rectangle GetHeaderBounds(int edge, int top, int width, int height) {
			int x = edge - (IsRightToLeft ? width : 0);
			return new Rectangle(x, top, width, height);
		}
		public virtual void CalcColumnsInfo() {
			if(TreeList.HasBands || TreeList.ActualShowBands) return;
			ColumnsInfo.CellWidthes.Clear();
			int columnEdge = IsRightToLeft ? viewRects.Client.Right : ViewRects.Client.Left;
			int top = ViewRects.ColumnPanel.Top;  
			bool leftCoordSubstracted = false;
			if(TreeList.OptionsView.ShowIndicator) 
				columnEdge += CalcHeaderButton(CreateColumnInfo(null), columnEdge, top, ColumnPanelHeight);
			CheckLeftCoord(ref columnEdge, ref leftCoordSubstracted, FixedLeftColumn == null);			
			for(int i = 0; i < TreeList.VisibleColumns.Count + 1; i++) {
				ColumnInfo ci = CreateColumnInfo(TreeList.VisibleColumns[i]);
				ci.CellIndex = i;
				TreeListColumn column = i > -1 ? TreeList.VisibleColumns[i] : null;
				int width = 0;
				if(i == TreeList.VisibleColumns.Count) { 
					width = GetLastColumnWidth(columnEdge, ViewRects.ActualColumnPanel);
					if(width == -1) break;
					ci.Type = ColumnInfo.ColumnInfoType.BehindColumn;
				}
				CheckLeftCoord(ref columnEdge, ref leftCoordSubstracted, column != null && column.Fixed != FixedStyle.Left);
				CheckOffsetFixedRightHeader(ref columnEdge, column, FixedRightColumn, ViewRects.ColumnPanel);
				int actualWidth = ci.Column == null ? width : ci.Column.VisibleWidth;
				Rectangle bounds = GetHeaderBounds(columnEdge, top, actualWidth, ColumnPanelHeight);
				columnEdge += CalcColumnInfo(ci, bounds, false);
				CheckOffsetFixedLeftHeader(ref columnEdge, column, FixedLeftColumn);
				CalcColumnActualBounds(ci);
				RegisterColumnInfo(ci);				
			}
			UpdateHeaderPositions();
		}
		void CalcFixedColumnActualBounds(ColumnInfo ci) {
			if(ci == null || ci.Column == null) return;
			ci.ActualBounds = GetFixedHeaderActualBounds(ci);
		}
		protected virtual void CalcColumnActualBounds(ColumnInfo ci) {
			if(ci == null || ci.Column == null) return;
			if(ci.Column == FixedLeftColumn)
				CalcFixedColumnActualBounds(ci);
			if(ci.Column == FixedRightColumn)
				CalcFixedColumnActualBounds(ColumnsInfo.LastColumnInfo);
		}
		int CalcRestHeaderWidth(IHeaderObject header){
			if(header == null) return 0;
			if(header is TreeListBand) return CalcRestBandsWidth(header as TreeListBand);
			return CalcRestColumnsWidth(header as TreeListColumn);
		}
		protected virtual int CalcRestColumnsWidth(TreeListColumn column) {
			if(column == null) return 0;
			int res = 0;
			for(int n = 0; n < TreeList.VisibleColumns.Count; n++) {
				TreeListColumn col = TreeList.VisibleColumns[n];
				if(col == column || column == null) {
					res += col.VisibleWidth;
					column = null;
				}
			}
			return res;
		}
		internal AppearanceObject ClonePaintAppearance(AppearanceObject appearance) { return (AppearanceObject)appearance.Clone(); }	   
		public virtual int CalcColumnInfo(ColumnInfo ci, Rectangle bounds, bool customization) {
			AppearanceObject app = new AppearanceObject();
			AppearanceHelper.Combine(app, new AppearanceObject[] { (ci.Column == null || customization) ? null : ci.Column.AppearanceHeader, this.PaintAppearance.HeaderPanel });
			ci.CustomizationForm = customization;
			ci.SetAppearance(app);
			if(ci.Column != null) {
				if(ci.Column == TreeList.PressedColumn)
					ci.Pressed = true;
			}
			if(ci.Column != null) {
				ci.DefaultCellTextAlignment = ci.Column.DefaultCellTextAlignment;
			}
			ci.Bounds = bounds;
			if(ci.Bounds.Width > 0) {
				if(ci.Bounds.Right < ViewRects.ScrollArea.Left || ci.Bounds.Left > ViewRects.ScrollArea.Right) {
					ci.Bounds = new Rectangle(bounds.Location, new Size(0, bounds.Height));
					return Direction * bounds.Width;
				}
			}
			UpdateGlyphInfo(ci);
			ObjectPainter.CalcObjectBounds(GInfo.Graphics, TreeList.ElementsLookAndFeel.Painter.Header, ci);
			return Direction * ci.Bounds.Width;
		}
		protected virtual void UpdateHeaderPositions() {
			if(TreeList.AllowBandColumnsMultiRow) {
				for(int i = 0; i < ColumnsInfo.Columns.Count; i++) {
					ColumnInfo ci = (ColumnInfo)ColumnsInfo.Columns[i];
					if(ci.Type == ColumnInfo.ColumnInfoType.ColumnButton) {
						ci.HeaderPosition = HeaderPositionKind.Left;
						break;
					}
					if(ci.ParentBandInfo != null && ci.ParentBandInfo.Band != null && IsFirstBand(ci.ParentBandInfo.Band) && (ci.Position == ColumnInfo.ColumnPosition.Left || ci.Position == ColumnInfo.ColumnPosition.Single))
						ci.HeaderPosition = HeaderPositionKind.Left;
				}
				for(int i = ColumnsInfo.Columns.Count - 1; i >= 0; i--) {
					ColumnInfo ci = (ColumnInfo)ColumnsInfo.Columns[i];
					if(ci.Type == ColumnInfo.ColumnInfoType.BehindColumn) {
						ci.HeaderPosition = HeaderPositionKind.Right;
						break;
					}
					if(ci.ParentBandInfo != null && ci.ParentBandInfo.Band != null && IsLastBand(ci.ParentBandInfo.Band) && (ci.Position == ColumnInfo.ColumnPosition.Right || ci.Position == ColumnInfo.ColumnPosition.Single))
						ci.HeaderPosition = HeaderPositionKind.Right;
				}
			}
			else {
				for(int i = 0; i < ColumnsInfo.Columns.Count; i++) {
					ColumnInfo ci = (ColumnInfo)ColumnsInfo.Columns[i];
					if(ColumnsInfo.Columns.Count == 1) ci.HeaderPosition = HeaderPositionKind.Left;
					else if(i == 0) ci.HeaderPosition = HeaderPositionKind.Left;
					else if(i == ColumnsInfo.Columns.Count - 1) ci.HeaderPosition = HeaderPositionKind.Right;
					else ci.HeaderPosition = HeaderPositionKind.Center;
				}
			}
			if(BandsInfo.Count > 0) {
				BandInfo firstBandInfo = BandsInfo[0];
				while(firstBandInfo != null) {
					firstBandInfo.HeaderPosition = HeaderPositionKind.Left;
					if(firstBandInfo.Type == ColumnInfo.ColumnInfoType.ColumnButton) break;
					firstBandInfo = firstBandInfo.HasChildren ? firstBandInfo.Bands[0] : null;
				}
			}
			if(BandsInfo.Count > 1) {
				BandInfo lastBandInfo = BandsInfo[BandsInfo.Count - 1];
				while(lastBandInfo != null) {
					lastBandInfo.HeaderPosition = HeaderPositionKind.Right;
					if(lastBandInfo.Type == ColumnInfo.ColumnInfoType.BehindColumn) break;
					lastBandInfo = lastBandInfo.HasChildren ? lastBandInfo.Bands[lastBandInfo.Bands.Count - 1] : null;
				}
			}
		}
		public virtual void UpdateGlyphInfo(ColumnInfo ci) {
			ci.InnerElements.Clear();
			UpdateSortInfo(ci);
			UpdateFilterInfo(ci);
			UpdateImageInfo(ci);
		}
		protected virtual void UpdateFilterInfo(ColumnInfo ci) { 
			if(ci.Column == null) return;
			if(!CanShowFilterButton(ci)) return;
			TreeListFilterButtonInfoArgs args = new TreeListFilterButtonInfoArgs();
			args.Filtered = !ci.Column.FilterInfo.IsEmpty;
			if(TreeList.GetHeaderFilterButtonShowMode() == FilterButtonShowMode.SmartTag) {
				DrawElementInfo di = new DrawElementInfo(ElementPainters.SmartFilterButton, args, StringAlignment.Far);
				di.ElementInterval = 0;
				di.Visible = args.Filtered || ci.Pressed;
				di.RequireTotalBounds = true;
				ci.InnerElements.Add(di);
			}
			else {
				DrawElementInfo di = new DrawElementInfo(ElementPainters.FilterButton, args);
				di.Visible = args.Filtered || ci.Pressed;
				ci.InnerElements.Add(di);
			}
		}
		protected internal Rectangle GetFilterButtonBounds(TreeListColumn column) {
			ColumnInfo ci = ColumnsInfo[column];
			Rectangle rect = Rectangle.Empty;
			if(ci != null) {
				rect = ci.Bounds;
				if(TreeList.GetHeaderFilterButtonShowMode() == FilterButtonShowMode.SmartTag) {
					DrawElementInfo info = ci.InnerElements.Find(typeof(TreeListFilterButtonInfoArgs));
					if(info != null && !info.ElementInfo.Bounds.IsEmpty)
						rect = info.ElementInfo.Bounds;
				}
			}
			return rect;
		}
		protected virtual bool CanShowFilterButton(ColumnInfo ci) {
			return TreeList.OptionsBehavior.EnableFiltering && ci.Column.OptionsFilter.AllowFilter && ci.Column.Visible;
		}
		protected virtual void UpdateSortInfo(ColumnInfo ci) {
			if(ci.Column != null && ci.Column.SortOrder != SortOrder.None) {
				SortedShapeObjectInfoArgs sortArgs = new SortedShapeObjectInfoArgs();
				sortArgs.Ascending = (ci.Column.SortOrder == SortOrder.Ascending);
				ci.InnerElements.Add(new DrawElementInfo(ElementPainters.SortedShapePainter, sortArgs));
			}
		}
		protected virtual void UpdateImageInfo(ColumnInfo ci) {
			if(ci.Column != null && IsValidImageIndex(ci.Column.ImageIndex, ci.Column.Images)) {
				if(TreeList.OptionsView.AllowGlyphSkinning)
					ci.InnerElements.Add(new DrawElementInfo(new SkinnedGlyphElementPainter(), new SkinnedGlyphElementInfoArgs(ci.Column.Images, ci.Column.ImageIndex, null), ci.Column.ImageAlignment));
				else
					ci.InnerElements.Add(new DrawElementInfo(new GlyphElementPainter(), new GlyphElementInfoArgs(ci.Column.Images, ci.Column.ImageIndex, null), ci.Column.ImageAlignment));
			}
		}
		public virtual ColumnInfo CloneDragInfo(ColumnInfo ci) {
			ColumnInfo dragInfo = CreateColumnInfo(ci.Column);
			dragInfo.MouseOver = true;
			dragInfo.Bounds = new Rectangle(Point.Empty, ci.Bounds.Size);
			UpdateGlyphInfo(dragInfo);
			dragInfo.Appearance.Assign(ci.Appearance);
			dragInfo.HeaderPosition = HeaderPositionKind.Special;
			ObjectPainter.CalcObjectBounds(GInfo.Graphics, TreeList.ElementsLookAndFeel.Painter.Header, dragInfo);
			return dragInfo;
		}
		protected virtual void CalcAutoFilterRowInfo() {
			if(!TreeList.OptionsView.ShowAutoFilterRow) return;
			this.autoFilterRowInfo = CreateRowInfo(TreeList.AutoFilterNode);
			AutoFilterRowInfo.Level = 0;
			AutoFilterRowInfo.VisibleIndex = TreeList.AutoFilterNodeId;
			ArrayList viewInfoList = new ArrayList();
			AutoFilterRowInfo.NodeHeight = CalcNodeHeight(AutoFilterRowInfo, out viewInfoList);
			CalcAutoFilterRowBounds(AutoFilterRowInfo.NodeHeight * RowLineCount);
			CalcRowCellsInfo(AutoFilterRowInfo, viewInfoList);
			CalcRowIndentInfo(AutoFilterRowInfo);
		}
		protected virtual void CalcAutoFilterRowBounds(int height) {
			if(AutoFilterRowInfo == null) return;
			ViewRects.AutoFilterRow = ViewRects.TotalRows;
			ViewRects.AutoFilterRow.Height = height;
			ViewRects.Rows.Y += height;
			ViewRects.Rows.Height -= height;
			int separatorHeight = GetSpecialRowSeparatorHeight();
			if(separatorHeight > 0) {
				ViewRects.TopRowSeparator = ViewRects.AutoFilterRow;
				ViewRects.TopRowSeparator.Height = separatorHeight;
				ViewRects.TopRowSeparator.Y = ViewRects.AutoFilterRow.Bottom + 1;
				ViewRects.Rows.Y += separatorHeight + 1;
				ViewRects.Rows.Height -= separatorHeight + 1;
			}
			Point location = new Point(ViewRects.Rows.X, ViewRects.AutoFilterRow.Y);
			AutoFilterRowInfo.Bounds = new Rectangle(location, new Size(ViewRects.ColumnTotalWidth + (HasFixedLeft ? TreeList.FixedLineWidth : 0) + (HasFixedRight ? TreeList.FixedLineWidth : 0), height));
			AutoFilterRowInfo.DataBounds = GetRowDataBounds(AutoFilterRowInfo);
		}
		public virtual void CalcEmptyBehindColumn() {
			if(BehindColumnIsVisible) {
				ColumnInfo behindci = ColumnsInfo.Columns[ColumnsInfo.Columns.Count - 1] as ColumnInfo;
				CalcEmptyBehindColumn(behindci);
			}
		}
		void CalcEmptyBehindColumn(ColumnInfo behindci) {
			int x = IsRightToLeft ? ViewRects.Client.Left : behindci.Bounds.Left;
			int width = (IsRightToLeft ? behindci.Bounds.Right : ViewRects.Client.Right) - x;
			ViewRects.EmptyBehindColumn = new Rectangle(x, behindci.Bounds.Bottom, width, ViewRects.Rows.Height + ViewRects.Footer.Height);
		}
		public virtual void CalcRowsInfo() {
			ViewRects.Rows = ViewRects.Client;
			ViewRects.Rows.Y = ViewRects.ColumnPanel.Bottom;
			ViewRects.Rows.Height = ViewRects.Footer.Top - ViewRects.ColumnPanel.Bottom;
			ViewRects.TotalRows = ViewRects.Rows;
			CalcBeyondScrollSquare();
			CalcEmptyBehindColumn();
			if(BehindColumnIsVisible) {
				if(IsRightToLeft)
					ViewRects.Rows.X = ViewRects.EmptyBehindColumn.Right;
				ViewRects.Rows.Width -= ViewRects.EmptyBehindColumn.Width;
				if(TreeList.VisibleColumns.Count == 0 && !TreeList.OptionsView.ShowIndicator)
					ViewRects.Footer.Width = 0;
			}
			CalcAutoFilterRowInfo();
			Rectangle rect = BriefCalcRowsInfo();
			if(rect.Bottom < ViewRects.Rows.Bottom) {
				ViewRects.EmptyRows = new Rectangle(ViewRects.Rows.Left, rect.Bottom,
					ViewRects.Rows.Width, ViewRects.Rows.Bottom - rect.Bottom);
				ViewRects.Rows.Height -= ViewRects.EmptyRows.Height;
			}
			CheckIncreaseVisibleRows(rect.Width);
		}
		public virtual Rectangle BriefCalcRowsInfo() {
			Rectangle rect = ViewRects.Rows;
			if(TreeList != null && TreeList.Nodes.Count > 0) {
				rect.Height = TotalRowHeight;
				if(TreeList.IsPixelScrolling) 
					rect.Y -= TreeList.TopVisibleNodePixel - CalcPixelPositionByVisibleIndex(TreeList.TopVisibleNodeIndex);
				CalcRowGroupInfoArgs rowArgs = new CalcRowGroupInfoArgs(rect);
				ViewRects.RowsTotalHeight = 0;
				CalcRowGroupInfo(TreeList.Nodes, rowArgs);
				rect = rowArgs.Bounds;
				rect.Y -= rect.Height;
			}
			else rect.Height = 0;
			return rect;
		}
		protected internal virtual void UpdateRowsInfo(bool refreshCellState = false) {
			if(!IsValid || TreeList.Nodes.Count == 0) return;
			if(AutoFilterRowInfo != null)
				UpdateRowInfo(AutoFilterRowInfo);
			foreach(RowInfo ri in RowsInfo.Rows) {
				TreeNodeCellState oldRowState = ri.RowState;
				TreeNodeCellState newRowState = CalcRowState(ri);
				if(oldRowState != newRowState || refreshCellState)
					UpdateRowInfo(ri, false, refreshCellState);
			}
		}
		protected internal Rectangle GetEditorBounds(CellInfo cell) {
			Rectangle r = cell.CellValueRect;
			Rectangle bounds = UpdateFixedRange(r, cell.ColumnInfo);
			if(bounds.Right > ViewRects.TotalRows.Right) {
				bounds.Width = ViewRects.TotalRows.Right - bounds.Left;
			}
			if(bounds.Bottom > ViewRects.TotalRows.Bottom) {
				bounds.Height = ViewRects.TotalRows.Bottom - bounds.Top;
			}
			if(bounds.Width < 1 || bounds.Height < 1) return Rectangle.Empty;
			return bounds;
		}
		protected internal void UpdateRowInfo(RowInfo ri, bool recreateEditViewInfo = false, bool refreshCellState = false) {
			if(ri == null)
				return;
			ri.RowState = CalcRowState(ri);
			UpdateRowPaintAppearance(ri);
			for(int i = 0; i < ri.Cells.Count; i++) {
				CellInfo cell = ri.Cells[i] as CellInfo;
				UpdateCellInfo(cell, ri.Node, recreateEditViewInfo, refreshCellState);
			}
			CalcSelectImage(ri);
			CalcStateImage(ri);
			ri.IndicatorInfo.ImageIndex = GetRowIndicatorImageIndex(ri);
		}
		protected internal void UpdateCellInfo(CellInfo cell, TreeListNode node, bool recreateEditViewInfo = false, bool refreshCellState = false) {
			if(cell == null)
				return;
			if(recreateEditViewInfo) {
				Rectangle bounds = cell.EditorViewInfo != null ? cell.EditorViewInfo.Bounds : Rectangle.Empty;
				cell.CreateViewInfo();
				cell.EditorViewInfo.Bounds = bounds;
			}
			cell.State = CalcRowCellState(cell);
			UpdateRowCellPaintAppearance(cell);
			UpdateEditorInfo(cell);
			if(!refreshCellState)
				UpdateCell(cell, cell.Column, node);
		}
		protected internal virtual RowInfo CreateRowInfo(TreeListNode node) {
			RowInfo info = new RowInfo(this, node);
			info.RightToLeft = IsRightToLeft;
			return info;
		}
		protected virtual void CalcRowGroupInfo(TreeListNodes nodes, CalcRowGroupInfoArgs rowArgs) {
			rowArgs.Index = TreeList.TopVisibleNodeIndex;
			TreeListNode node = TreeList.TopVisibleNode;
			if(!TreeListFilterHelper.IsNodeVisible(node)) {
				node = TreeListNodesIterator.GetNextVisible(node);
			}
			while(node != null && rowArgs.Bounds.Top < ViewRects.Rows.Bottom) {
				ArrayList viewInfoList = null;
				RowInfo ri = CreateRowInfo(node);
				ri.VisibleIndex = rowArgs.Index;
				ri.Level = TreeListFilterHelper.CalcVisibleNodeLevel(ri.Node);
				ri.Expanded = ri.Node.Expanded;
				UpdateRowCondition(ri);
				ri.NodeHeight = CalcNodeHeight(ri, out viewInfoList);
				rowArgs.InflateBoundsHeight(ri.NodeHeight - RowHeight);
				ri.Bounds = rowArgs.Bounds;
				CalcRowInfo(ri, viewInfoList);
				RowsInfo.Rows.Add(ri);
				rowArgs.VertOffsetBounds(ri.Bounds.Height);
				CalcRowFooterInfo(node, rowArgs);
				rowArgs.IncIndex();
				node = TreeListNodesIterator.GetNextVisible(node);
			}
			RemoveInvisibleAnimatedItems();
		}
		internal int GetDataBoundsLeftLocation(TreeListNode node) {
			if(TreeListAutoFilterNode.IsAutoFilterNode(node) || TreeList.HasAllColumnsFixedRight)
				return 0;
			return GetViewLevel(TreeListFilterHelper.CalcVisibleNodeLevel(node)) * RC.LevelWidth + RC.SelectImageSize.Width + RC.StateImageSize.Width + RC.CheckBoxSize.Width;
		}
		protected virtual void RemoveAnimatedItem(EditorAnimationInfo info) {
			XtraAnimator.RemoveObject(TreeList, info.Link);
		}
		protected virtual void RemoveInvisibleAnimatedItems() {
			for(int i = 0; i < XtraAnimator.Current.Animations.Count; i++) {
				EditorAnimationInfo info = XtraAnimator.Current.Animations[i] as EditorAnimationInfo;
				if(info == null || info.AnimatedObject != TreeList) continue;
				if(!HasAnimatedItem(info.Link as TreeListCellId))
					RemoveAnimatedItem(info);
			}
		}
		protected virtual bool HasAnimatedItem(TreeListCellId id) {
			RowInfo ri = RowsInfo[id.Node];
			if(ri == null) return false;
			return ri[id.Column] != null;
		}
		void CalcRowFooterInfo(TreeListNode node, CalcRowGroupInfoArgs rowArgs) {
			if(node == node.owner.LastNodeEx) {
				if(!(TreeListFilterHelper.HasVisibleChildren(node) && node.Expanded)) {
					bool even = IsEven(rowArgs.Index);
					CalcRowFooterInfo(RowsInfo.Rows.Count - 1, node, rowArgs, even);
					TreeListNode parent = node.ParentNode;
					while(parent != null && parent == parent.owner.LastNodeEx) {
						even = !even;
						CalcRowFooterInfo(RowsInfo.Rows.Count - 1, parent, rowArgs, even);
						node = parent;
						parent = parent.ParentNode;
					}
				}
			}
		}	  
		Rectangle GetRowFooterIndentBounds(Rectangle footerBounds) {
			Rectangle bounds = Rectangle.Empty;
			if(IsRightToLeft) {
				if(footerBounds.Right >= ViewRects.Rows.Right + ViewRects.IndicatorWidth) return bounds;
				bounds.X = ViewRects.Rows.Right - ViewRects.IndicatorWidth + (!IsFixedIndent ? TreeList.LeftCoord : 0);
				bounds.Width = ViewRects.Rows.Right - ViewRects.IndicatorWidth - footerBounds.Right;
			}
			else {
				if(footerBounds.Left <= ViewRects.Rows.Left + ViewRects.IndicatorWidth) return bounds;
				bounds.X = ViewRects.Rows.Left + ViewRects.IndicatorWidth -  (!IsFixedIndent ? TreeList.LeftCoord : 0);
				bounds.Width = footerBounds.Left - (ViewRects.Rows.Left + ViewRects.IndicatorWidth );
			}
			bounds.Y = footerBounds.Top;
			bounds.Height = footerBounds.Height;
			return bounds;
		}
		protected virtual void CalcRowFooterInfo(int rowIndex, TreeListNode node, CalcRowGroupInfoArgs rowArgs, bool even) {
			if(!ShowGroupFooter) return;
			if(rowIndex < 0) return;
			RowInfo riLastChild = (RowInfo)RowsInfo.Rows[rowIndex];
			int levDif = (riLastChild.Node.Level - node.Level) * RC.LevelWidth;
			RowFooterInfo fi = new RowFooterInfo(node, riLastChild);
			fi.RightToLeft = IsRightToLeft;
			fi.SetAppearance(ClonePaintAppearance(PaintAppearance.GroupFooter));
			fi.Level = node.Level;
			AppearanceHelper.Combine(fi.IndentInfo.Appearance, new AppearanceObject[] { even ? EvenAppearance : OddAppearance, PaintAppearance.Row});
			fi.IndentInfo.TreeLineBrush = CalcTreeLineBrush(fi);
			fi.IndentInfo.LevelWidth = RC.LevelWidth;
			bool last = node.ParentNode != null && node.ParentNode != node.ParentNode.owner.LastNodeEx;
			fi.Bounds = new Rectangle(ViewRects.Rows.Left, rowArgs.Bounds.Top, ViewRects.Rows.Width, RC.GroupFooterHeight + (last ? 1 : 0));
			fi.FooterBounds = new Rectangle(riLastChild.DataBounds.Left - (IsRightToLeft ? 0 : levDif), rowArgs.Bounds.Top,
				riLastChild.DataBounds.Width + levDif, fi.Bounds.Height - (last ? 1 : 0));
			fi.ClientBounds = GetFooterClientBounds(fi.FooterBounds);
			if(ViewRects.IndicatorWidth != 0)
				UpdateIndicatorInfo(fi.IndicatorInfo, GetIndicatorBounds(fi.Bounds));
			fi.IndentInfo.Bounds = GetRowFooterIndentBounds(fi.FooterBounds);
			for(int i = 0; i < riLastChild.Cells.Count; i++) {
				CellInfo cell = riLastChild.Cells[i] as CellInfo;
				TreeListColumn col = cell.Column;
				if(col == null) continue;
				int delta = ((i == 0 || cell.ColumnInfo.IsFirst) ? levDif : 0);
				int decWidth = ((i == 0 || cell.ColumnInfo.IsFirst) ? col.VisibleWidth - cell.Bounds.Width - RC.vlw : 0);
				Rectangle r = new Rectangle(cell.Bounds.Left - (IsRightToLeft ? 0 : delta), fi.Bounds.Top, cell.ColumnInfo.Bounds.Width - decWidth + delta, RC.GroupFooterCellHeight * cell.ColumnInfo.RowCount);
				Rectangle itemBounds = ElementPainters.FooterPanelPainter.CalcCellBounds(fi.ClientBounds, r, RC.GroupFooterCellHeight, cell.ColumnInfo.StartRow, cell.ColumnInfo.RowCount);
				AddFooterItem(fi, node.owner, col, itemBounds, col.RowFooterSummary, col.RowFooterSummaryStrFormat, false, false);
			}
			FillRowFooterIndentInfo(fi, levDif / RC.LevelWidth);
			RowsInfo.RowFooters.Add(fi);
			riLastChild.LastFooter = fi;
			rowArgs.VertOffsetBounds(fi.Bounds.Height);
			fi.NeedsRecalcAll = false;
		}
		Brush CreateRowFooterIndentBrush(TreeNodeCellState state) {
			if(RC.GroupFooterHeight == 0) return null;
			Rectangle bounds = new Rectangle(0, 0, ViewRects.ColumnTotalWidth, RC.GroupFooterHeight);
			using(AppearanceObject app = RC.CreateAppearance(GetRowMixAppearancesCore(state, null, null, null, null,null))) {
				if(app.BackColor == Color.Empty || app.BackColor2 == Color.Empty) return new SolidBrush(app.BackColor == Color.Empty ? app.BackColor2 : app.BackColor);
				return new LinearGradientBrush(bounds, app.BackColor, app.BackColor2, app.GradientMode);
			}
		}
		protected virtual void CalcEmptyAreaRegion() {
			if(EmptyArea.IsEmpty) return;
			this.emptyAreaRegion = new Region(EmptyArea);
			if(ViewRects.IndicatorBounds.Width > 0) {
				EmptyAreaRegion.Exclude(ViewRects.IndicatorBounds);
			}
			foreach(RowInfo ri in RowsInfo.Rows) {
				EmptyAreaRegion.Exclude(ri.GetExcludeEmptyRect());
			}
			foreach(RowFooterInfo fi in RowsInfo.RowFooters) {
				EmptyAreaRegion.Exclude(fi.GetExcludeEmptyRect(TreeList.OptionsView.ShowIndentAsRowStyle));
			}
		}
		AppearanceObjectEx CreateAppearance(StyleFormatCondition cond) {	
			AppearanceObjectEx app = new AppearanceObjectEx();
			app.Assign(cond.Appearance);
			return app;
		}
		protected internal virtual bool CalcFormatRulesCore(RowInfo ri) {
			if(!TreeList.FormatRules.HasValidRules) return false;
			ri.FormatRuleInfo.Reset();
			for(int n = 0; n < TreeList.FormatRules.Count; n++) {
				TreeListFormatRule rule = TreeList.FormatRules[n];
				if(!rule.IsValid) continue;
				if(rule.ActualColumn != null) {
					if(!rule.ApplyToRow && !rule.ActualColumn.Visible) continue;
					ri.FormatRuleInfo.CheckRule(rule,  ri.Node, rule.ActualColumn, ri.Node.GetValue(rule.Column));
					continue;
				}
				for(int c = 0; c < TreeList.Columns.Count; c++) {
					TreeListColumn column = TreeList.Columns[c];
					if(!rule.ApplyToRow && !column.Visible) continue;
					if(rule.ActualColumn != null && rule.ActualColumn != column) continue;
					ri.FormatRuleInfo.CheckRule(rule, ri.Node, rule.ActualColumn, ri.Node.GetValue(rule.Column));
				}
			}
			return true;
		}
		protected internal virtual bool CalcConditionsCore(RowInfo ri) {
			if(treeList.FormatConditions.Count == 0) 
				return false;
			ri.ConditionInfo.Reset();
			for(int i = 0; i < treeList.FormatConditions.Count; i++) {
				StyleFormatCondition condition = treeList.FormatConditions[i];
				if(!condition.IsValid) continue;
				if(condition.Column != null) {
					if(!condition.Column.Visible && !condition.ApplyToRow) continue;
					ri.ConditionInfo.CheckCondition(condition, ri.Node, condition.Column);
					continue;
				}
				for(int k = 0; k < treeList.Columns.Count; k++) {
					TreeListColumn column = treeList.Columns[k];
					if(!condition.ApplyToRow && !column.Visible) continue;
					ri.ConditionInfo.CheckCondition(condition, ri.Node, column);
					if(condition.ApplyToRow && condition.Condition == FormatConditionEnum.Expression) break;
				}
			}
			return true;
		}		
		public virtual bool UpdateRowCondition(RowInfo ri) {
			if(ri.VisibleIndex < 0)
				return false;
			bool result = CalcFormatRulesCore(ri);
			return result || CalcConditionsCore(ri);
		}
		public void UpdateBeforePaint(RowInfo ri) {
			UpdateBeforePaint(ri, false);
		}
		public virtual void UpdateBeforePaint(RowInfo ri, bool always) {
			TreeNodeCellState rowState = CalcRowState(ri);
			if(rowState != ri.RowState || always) {
				ri.RowState = rowState;
				ri.IndicatorInfo.ImageIndex = GetRowIndicatorImageIndex(ri);
				UpdateRowPaintAppearance(ri);
				foreach(CellInfo cell in ri.Cells) {
					cell.State = CalcRowCellState(cell);
					UpdateRowCellPaintAppearance(cell);
				}
			}
		}
		public virtual void UpdateRowPaintAppearance(RowInfo ri) {
			AppearanceHelper.Combine(ri.PaintAppearance, GetRowMixAppearances(ri.RowState, null, null ,null));
			ri.IndentInfo.SetAppearance(ClonePaintAppearance(TreeList.OptionsView.ShowIndentAsRowStyle ? ri.PaintAppearance : PaintAppearance.Empty));
		}
		public virtual void UpdateRowCellPaintAppearance(CellInfo cell) {
			AppearanceObject focusedCell = null;
			AppearanceObjectEx cellApp = null, conditionAppearance = null;
			if(cell.Column != null) {
				cellApp = cell.Column.AppearanceCell;
				if(cell.Focused) {
					if(TreeList.OptionsSelection.InvertSelection) 
						focusedCell = TreeList.HasFocus ? PaintAppearance.FocusedRow : PaintAppearance.HideSelectionRow;
					else
						focusedCell = PaintAppearance.FocusedCell;
				}
			}
			conditionAppearance = cell.RowInfo.GetCellConditionAppearance(cell.Column);
			if(cell.Item != null && !cell.Item.AllowFocusedAppearance)
				cell.State &= ~(TreeNodeCellState.Selected | TreeNodeCellState.Focused);
			AppearanceHelper.Combine(cell.PaintAppearance, GetRowMixAppearances(cell.State, cellApp, focusedCell, conditionAppearance));
			cell.PaintAppearance = TreeList.InternalGetCustomNodeCellStyle(cell.Column, cell.RowInfo.Node, cell.PaintAppearance);
			cell.CheckHAlignment();
		}
		protected virtual void CalcRowInfo(RowInfo ri, ArrayList viewInfoList) {
			ri.RowState = CalcRowState(ri);
			string error;
			DevExpress.XtraEditors.DXErrorProvider.ErrorType errorType;
			TreeList.GetColumnError(null, ri.Node, out error, out errorType);
			ri.ErrorText = error;
			if(ri.ErrorText != null && ri.ErrorText.Length > 0) {
				ri.ErrorIcon = DevExpress.XtraEditors.DXErrorProvider.DXErrorProvider.GetErrorIconInternal(errorType);
			}
			ri.IndicatorInfo.ImageIndex = GetRowIndicatorImageIndex(ri);
			UpdateRowPaintAppearance(ri);
			ri.IndentAsRowStyle = TreeList.OptionsView.ShowIndentAsRowStyle;
			int width = BandsInfo.Count > 0 ? ViewRects.BandTotalWidth : ViewRects.ColumnTotalWidth + (HasFixedLeft ? TreeList.FixedLineWidth : 0) + (HasFixedRight ? TreeList.FixedLineWidth : 0);
			int x = IsRightToLeft ? ri.Bounds.Right - width : ri.Bounds.X;
			ri.Bounds = new Rectangle(x, ri.Bounds.Y, width, ri.NodeHeight * RowLineCount);
			ri.IndentInfo.ActualRowHeight = ri.Bounds.Height;
			ri.DataBounds = GetRowDataBounds(ri);
			CalcRowCellsInfo(ri, viewInfoList);
			CalcRowPreviewInfo(ri);
			CalcRowIndentInfo(ri);
			ViewRects.RowsTotalHeight += ri.Bounds.Height;
		}
		protected virtual int CalcNodeHeight(RowInfo ri, out ArrayList viewInfoList) {
			ArrayList cellWidthes = new ArrayList();
			if(TreeList.ActualAutoNodeHeight) {
				cellWidthes = TreeList.CreateColumnWidthesList(0);
				if(cellWidthes.Count > 0) {
					ColumnWidthInfo wi = (ColumnWidthInfo)cellWidthes[0];
					wi.Width = wi.Column.VisibleWidth;
				}
			}
			int nh = TreeList.InternalCalcNodeHeight(ri, RowHeight, cellWidthes, !TreeList.IsIniting, out viewInfoList, IsEven(ri.VisibleIndex), true);
			if(nh == -1) nh = RowHeight;
			return nh;
		}
		protected virtual Brush CalcTreeLineBrush(StyleObjectInfoArgs indentItem) {
			return RC.TreeLineBrush;
		}
		protected internal virtual int CheckIncreaseVisibleRows(int rowWidth) {
			if(!(ViewRects.EmptyRows.Height > 0 && TreeList.TopVisibleNodeIndex > 0 &&
				TreeList.TopVisibleNodeIndex + VisibleRowCount == TreeList.RowCount)) return 0;
			int erHeight = ViewRects.EmptyRows.Height;
			int increaseCount = 0;
			TreeListNode topNode = TreeListNodesIterator.GetPrevVisible(TreeList.TopVisibleNode);
			int totalRowHeight = 0;
			while(topNode != null) {
				totalRowHeight += GetTotalNodeHeight(topNode);
				if(erHeight - totalRowHeight >= 0) {
					increaseCount++;
					erHeight -= totalRowHeight;
				}
				else break;
				totalRowHeight = 0;
				topNode = TreeListNodesIterator.GetPrevVisible(topNode);
			}
			RowsInfo.IncreaseVisibleRows = increaseCount;
			return erHeight;
		}
		internal static string FormatValue(object val, TreeListColumn col, string strFmt) {
			string text = string.Empty;
			if(val != null) {
				if(strFmt != null && strFmt != string.Empty) {
					if(col.Format != null) text = string.Format(col.Format.Format, strFmt, val); 
					else text = string.Format(strFmt, val);
				}
				else text = val.ToString();
			}
			return text;
		}
		protected virtual void FillRowFooterIndentInfo(RowFooterInfo fi, int ld) {
			for(int i = 0; i < fi.RowAbove.IndentInfo.IndentItems.Count - ld - 1; i++)
				fi.IndentInfo.IndentItems.Add(fi.RowAbove.IndentInfo.IndentItems[i]);
		}
		public virtual void CalcSummaryFooterInfo() {
			if(!SummaryFooterInfo.NeedsRecalcAll && !SummaryFooterInfo.NeedsRecalcRects || TreeList.IsLockUpdate) return;
			SummaryFooterInfo.SetAppearance(ClonePaintAppearance(PaintAppearance.FooterPanel));
			SummaryFooterInfo.Bounds = ViewRects.Footer;
			SummaryFooterInfo.ClientBounds = GetFooterClientBounds(SummaryFooterInfo.Bounds);
			if(SummaryFooterInfo.NeedsRecalcAll)
				SummaryFooterInfo.FooterItems.Clear();
			CalcSummaryFooterItems(SummaryFooterInfo.NeedsRecalcAll);	   
			SummaryFooterInfo.NeedsRecalcAll = false;
			SummaryFooterInfo.NeedsRecalcRects = false;
		}
		protected virtual void CalcSummaryFooterItems(bool calcValues) {
			int index = 0;
			foreach(ColumnInfo ci in ColumnsInfo.Columns) {
				if(ci.Type != ColumnInfo.ColumnInfoType.Column) continue;
				Rectangle r = new Rectangle(ci.Bounds.Left, SummaryFooterInfo.Bounds.Top, ci.Bounds.Width, SummaryFooterInfo.CellHeight * ci.RowCount);
				Rectangle itemBounds = this.ElementPainters.FooterPanelPainter.CalcCellBounds(SummaryFooterInfo.ClientBounds, r, SummaryFooterInfo.CellHeight, ci.StartRow, ci.RowCount);
				if(calcValues) {
					AddFooterItem(SummaryFooterInfo, TreeList.Nodes, ci.Column, itemBounds, ci.Column.SummaryFooter, ci.Column.SummaryFooterStrFormat, ci.Column.AllNodesSummary, true);
				}
				else {
					FooterItem fi = (FooterItem)SummaryFooterInfo.FooterItems[index];
					fi.ItemBounds = itemBounds;
				}
				index++;
			}
		}
		protected virtual void AddFooterItem(SummaryFooterInfo fi, TreeListNodes nodes, TreeListColumn column, Rectangle itemBounds, SummaryItemType itemType, string strFormat, bool recursively, bool isSummaryFooter) {
			string text = string.Empty;
			if(itemType != SummaryItemType.None) {
				object val = TreeList.GetSummaryValueCore(nodes, column, itemType, recursively, isSummaryFooter);
				text = FormatValue(val, column, strFormat);
			}
			fi.FooterItems.Add(new FooterItem(itemBounds, text, column, itemType));
		}
		protected virtual Rectangle GetFooterClientBounds(Rectangle bounds) {
			return ElementPainters.FooterPanelPainter.GetObjectClientRectangle(new FooterPanelInfoArgs(null, PaintAppearance.GroupFooter,
				bounds, ColumnPanelRowCount, RC.GroupFooterCellHeight));
		}
		bool IsFirstRowInList(RowInfo ri) { return (RowsInfo.Rows.Count == 0); }
		protected virtual void UpdateIndicatorInfo(IndicatorInfo indicatorInfo, Rectangle bounds) {
			indicatorInfo.Bounds = bounds;
			indicatorInfo.Appearance.Assign(PaintAppearance.HeaderPanel);
		}
		Rectangle GetIndicatorBounds(Rectangle rowBounds) {
			int x = IsRightToLeft ? rowBounds.Right - ViewRects.IndicatorWidth : rowBounds.X;
			return new Rectangle(x, rowBounds.Top, ViewRects.IndicatorWidth, rowBounds.Height);
		}
		Rectangle GetRowIndentBounds(RowInfo ri) {
			int width = GetViewLevel(ri.Level) * RC.LevelWidth;
			int leftEdge = IsRightToLeft ? ri.Bounds.Right - width : ri.Bounds.Left;			
			int x = leftEdge + Direction * (ri.IndicatorInfo.Bounds.Width - (!IsFixedIndent ? TreeList.LeftCoord : 0));
			return new Rectangle(x, ri.Bounds.Top, width, ri.Bounds.Height);
		}
		Rectangle GetRowButtonBounds(RowInfo ri) {
			int h = TreeList.OptionsView.ExpandButtonCentered ? ri.Bounds.Height / 2 : ri.IndentInfo.ActualRowHeight / 2;
			int leftEdge = IsRightToLeft ? ri.IndentInfo.Bounds.Right - RC.PlusMinusButtonSize.Width : ri.IndentInfo.Bounds.Left;
			int x = leftEdge + Direction * ((ri.Level - RootLevel) * RC.LevelWidth + RC.LevelWidth / 2 - RC.PlusMinusButtonSize.Width / 2);
			return new Rectangle(x, ri.IndentInfo.Bounds.Top + h - RC.PlusMinusButtonSize.Height / 2, RC.PlusMinusButtonSize.Width, RC.PlusMinusButtonSize.Height);
		}
		protected virtual void CalcRowIndentInfo(RowInfo ri) {			
			if(TreeList.OptionsView.ShowIndicator) {
				UpdateIndicatorInfo(ri.IndicatorInfo, GetIndicatorBounds(ri.Bounds));
				ri.IndicatorInfo.ImageIndex = GetRowIndicatorImageIndex(ri);
				if(!TreeList.OptionsView.ShowColumns && IsFirstRowInList(ri))
					ri.TopMostIndicator = true;
			}
			if(TreeList.HasAllColumnsFixedRight)
				return;
			ri.IndentInfo.Bounds = GetRowIndentBounds(ri);
			if(ri.Node.HasChildren && TreeList.OptionsView.ShowButtons && (ri.Level - RootLevel) > -1)
				if(ri.Node.Nodes.Count > 0 && TreeListFilterHelper.HasVisibleChildren(ri.Node, treeList.GetActualTreeListFilterMode() == FilterMode.Smart) || ri.Node.Nodes.Count == 0) {
					int h = TreeList.OptionsView.ExpandButtonCentered ? ri.Bounds.Height / 2 : ri.IndentInfo.ActualRowHeight / 2;
					ri.ButtonBounds = GetRowButtonBounds(ri);
				}
			if(IsAutoFilterRow(ri))
				return;
			int edge = IsRightToLeft ? ri.IndentInfo.Bounds.Left - RC.CheckBoxSize.Width : ri.IndentInfo.Bounds.Right;
			int height = TreeList.OptionsView.ExpandButtonCentered ? ri.Bounds.Height : ri.IndentInfo.ActualRowHeight;
			if(!RC.CheckBoxSize.IsEmpty)
				ri.CheckBounds = new Rectangle(edge, ri.IndentInfo.Bounds.Top, RC.CheckBoxSize.Width, height);
			edge = IsRightToLeft ? edge - RC.SelectImageSize.Width : edge + RC.CheckBoxSize.Width;
			ri.SelectImageBounds = new Rectangle(edge, ri.IndentInfo.Bounds.Top,
				RC.SelectImageSize.Width, height);
			CalcSelectImage(ri);
			ri.StateImageBounds = new Rectangle(edge, ri.SelectImageBounds.Top,
				RC.StateImageSize.Width, height);
			CalcStateImage(ri);
			ri.IndentInfo.LevelWidth = RC.LevelWidth;
			ri.IndentInfo.TreeLineBrush = CalcTreeLineBrush(ri);
			CalcRowIndentItems(ri);
		}
		Size GetCheckBoxSize() { 
			if(!TreeList.OptionsView.ShowCheckBoxes) return Size.Empty;
			CheckObjectInfoArgs chArgs = new CheckObjectInfoArgs(null);
			chArgs.GlyphAlignment = HorzAlignment.Center;
			return ElementPainters.CheckPainter.CalcObjectMinBounds(chArgs).Size;
		}
		ColumnInfo GetNextColumn(ColumnInfo ci) {
			int index = ColumnsInfo.Columns.IndexOf(ci);
			return ((index + 1) >= ColumnsInfo.Columns.Count) ? null : (ColumnInfo)ColumnsInfo.Columns[index + 1];
		}
		protected bool IsFixedIndent {
			get {
				foreach(ColumnInfo ci in ColumnsInfo.Columns) {
					if(ci.Type == ColumnInfo.ColumnInfoType.EmptyColumn && IsFixed(ci)) {
						if(ci.ParentBandInfo != null && ci.ParentBandInfo.Band != null) {
							TreeListBand band = ci.ParentBandInfo.Band;
							return band.RootBand.OwnedCollection.FirstVisibleBand == band.RootBand;
						}  
					}
					if(ci.Column == null) continue;
					if(ci.Column.VisibleIndex == 0 && IsFixed(ci)) return true;
				}
				return false;
			}
		}
		protected virtual void UpdateAnimatedItem(EditorAnimationInfo info, BaseEditViewInfo vi) {
			info.ViewInfo = vi;
		}
		protected virtual void AddAnimatedItem(TreeListNode node, TreeListColumn column, BaseEditViewInfo vi) {
			TreeListCellId id = new TreeListCellId(node, column);
			XtraAnimator.Current.AddEditorAnimation(id, vi, TreeList, vi as IAnimatedItem, AnimationInvoker);
		}
		CustomAnimationInvoker animationInvoker;
		internal CustomAnimationInvoker AnimationInvoker {
			get {
				if(animationInvoker == null) animationInvoker = new CustomAnimationInvoker(OnAnimation);
				return animationInvoker;
			}
		}
		bool paintAnimatedItems = true;
		internal bool PaintAnimatedItems { get { return paintAnimatedItems; } set { paintAnimatedItems = value; } }
		protected virtual void OnAnimation(BaseAnimationInfo info) {
			EditorAnimationInfo editorInfo = info as EditorAnimationInfo;
			if(editorInfo == null) return;
			IAnimatedItem item = editorInfo.ViewInfo as IAnimatedItem;
			if(item == null) return;
			if(ShouldStopAnimation(editorInfo)) {
				RemoveAnimatedItem(editorInfo);
				info.CurrentFrame = 0;
				item.UpdateAnimation(info);
				TreeList.Invalidate(item.AnimationBounds);
				return;
			}
			if(ShouldAnimate(editorInfo)) {
				item.UpdateAnimation(editorInfo);
				TreeList.Invalidate(item.AnimationBounds);
			}
		}
		protected virtual bool ShouldAnimate(EditorAnimationInfo info) {
			BaseEditViewInfo vi = info.ViewInfo as BaseEditViewInfo;
			return vi == null || vi.OwnerEdit == null;
		}
		protected internal virtual bool ShouldStopAnimation(EditorAnimationInfo info) {
			TreeListAnimationType animationType = TreeList.GetActualAnimationType();
			if(animationType == TreeListAnimationType.NeverAnimate) return true;
			BaseEditViewInfo vi = info.ViewInfo as BaseEditViewInfo;
			if(vi == null) return false;
			TreeListCellId id = info.Link as TreeListCellId;
			if(id == null) return false;
			if(id.Node != TreeList.FocusedNode && animationType == TreeListAnimationType.AnimateFocusedNode) return true;
			return false;
		}
		protected virtual bool ShouldAddAnimatedItem(TreeListNode node, TreeListColumn column, BaseEditViewInfo info) {
			if(!(info is IAnimatedItem)) return false;
			TreeListAnimationType animationType = TreeList.GetActualAnimationType();
			if(animationType == TreeListAnimationType.NeverAnimate) return false;
			if(animationType == TreeListAnimationType.AnimateFocusedNode && node != TreeList.FocusedNode) return false;
			TreeListCellId id = new TreeListCellId(node, column);
			return XtraAnimator.Current.Get(TreeList, id) == null;
		}
		protected virtual EditorAnimationInfo GetAnimation(TreeListNode node, TreeListColumn column, BaseEditViewInfo vi) {
			if(!(vi is IAnimatedItem)) return null;
			TreeListCellId id = new TreeListCellId(node, column);
			return XtraAnimator.Current.Get(TreeList, id) as EditorAnimationInfo; 
		}
		protected virtual Rectangle GetRowDataBounds(RowInfo ri) {
			Point pt = GetDataBoundsLocation(ri.Node, ri.Bounds.Top);
			Rectangle bounds = new Rectangle(0, pt.Y, 0, ri.Bounds.Height);
			if(IsRightToLeft) {
				bounds.X = ri.Bounds.X;
				bounds.Width = pt.X - bounds.X;
				if(!ViewRects.FixedRight.IsEmpty) {
					if(bounds.Right < ViewRects.FixedRight.Right)
						bounds.Width = ViewRects.FixedRight.Width;
				}
			}
			else {
				bounds.X = pt.X;
				bounds.Width = ri.Bounds.Right - bounds.X - (TreeList.HasAllColumnsFixedRight ? 0 : TreeList.LeftCoord);
				if(!ViewRects.FixedRight.IsEmpty) {
					if(bounds.Left > ViewRects.FixedRight.Left)
						bounds.X = ViewRects.FixedRight.Left;
				}
			}
			return bounds;
		}
		protected virtual Rectangle GetRowCellBounds(RowInfo ri, ColumnInfo ci, bool shouldHideLeftVerticalLine) {
			int x = ci.Bounds.X + CellTextIndent;
			if(IsRightToLeft && !IsFixedLeft(ci)) x += RC.vlw;
			int y = ri.DataBounds.Top + ri.NodeHeight * ci.StartRow + (ci.StartRow > 0 ? CellTextIndent : 0) + CellTextIndent;
			int width = ci.Bounds.Width - 2 * CellTextIndent + (!IsFixedLeft(ci) ? -RC.vlw : 0);
			int height = ri.NodeHeight * ci.RowCount - 2 * CellTextIndent - (ci.StartRow > 0 ? CellTextIndent : 0);
			Rectangle cellRect = new Rectangle(x, y, width, height);
			if(ci.IsFirst) {
				int indent = (IsRightToLeft ? ci.Bounds.Right - ri.DataBounds.Right : ri.DataBounds.Left - ci.Bounds.Left) + (shouldHideLeftVerticalLine ? 0 : RC.vlw);
				cellRect.X += IsRightToLeft ? 0 : indent;
				cellRect.Width -= indent;
			}
			return cellRect;
		}
		protected virtual void CalcRowCellsInfo(RowInfo ri, ArrayList viewInfoList) {			
			Rectangle cellRect = Rectangle.Empty;
			bool shouldHideLeftVerticalLine = (!TreeList.OptionsView.ShowRoot && ri.Node.Level == 0 && RC.SelectImageSize.Width == 0 && RC.StateImageSize.Width == 0 && RC.CheckBoxSize.Width == 0) || IsAutoFilterRow(ri);
			foreach(ColumnInfo ci in ColumnsInfo.Columns) {
				if(ci.Type != ColumnInfo.ColumnInfoType.Column && ci.Type != ColumnInfo.ColumnInfoType.EmptyColumn) continue;
				cellRect = GetRowCellBounds(ri, ci, shouldHideLeftVerticalLine);
				ColumnInfo nextColumn = GetNextColumn(ci);
				if(nextColumn != null && FixedRightColumn == nextColumn.Column) {
					if(!IsFixed(ci) && ci.Bounds.Right <= (nextColumn.Bounds.Left - TreeList.FixedLineWidth)) cellRect.Width += RC.vlw;
				}
				if(ci.Bounds.Width == 0) continue;
				if(ci.Type == ColumnInfo.ColumnInfoType.EmptyColumn) {
					EmptyCellInfo cell = new EmptyCellInfo(ci, ri);
					cell.SetBounds(cellRect);
					ri.Cells.Add(cell);
					UpdateRowCellLines(ri, ci, nextColumn, cell);
				}
				else {
					CellInfo cell = GetCachedCellInfo(ci.Column, viewInfoList);
					if(cell == null)
						cell = CreateCellInfo(ci, ri);
					cell.ColumnInfo = ci;
					cell.State = CalcRowCellState(cell);
					UpdateRowCellPaintAppearance(cell);
					if(cell.EditorViewInfo != null) {
						cell.EditorViewInfo.Bounds = cellRect;
						cell.EditorViewInfo.PaintAppearance = cell.PaintAppearance;
						cell.EditorViewInfo.AllowTextToolTip = TreeList.OptionsBehavior.ShowToolTips;
						UpdateEditorInfo(cell);
						UpdateCell(cell, ci.Column, ri.Node);
					}
					UpdateAnimation(ri.Node, ci.Column, cell);
					ri.Cells.Add(cell);
					UpdateRowCellLines(ri, ci, nextColumn, cell);
				}
			}
			UpdateRowCellFloatLines(ri, shouldHideLeftVerticalLine);
			ri.SetHeight(ri.Bounds.Height + RC.hlw);
		}
		Rectangle GetRTLFloatLineBounds(RowInfo ri, RowInfo riPrev, bool vert) {			
			if(vert) return new Rectangle(ri.DataBounds.Right - RC.vlw, ri.DataBounds.Top, RC.vlw, ri.DataBounds.Height + RC.hlw);
			int y = 0, width = 0;
			if(ShowGroupFooter && riPrev.LastFooter != null) {
				width = ri.DataBounds.Width;
				y = riPrev.LastFooter.IndentInfo.Bounds.Bottom;
			}
			else {
				y = TreeList.OptionsView.ShowPreview ? ri.Bounds.Top - RC.hlw : riPrev.Bounds.Bottom - 1;
				width = ri.DataBounds.Right - riPrev.DataBounds.Right;
			}
			return new Rectangle(ri.DataBounds.Right - width, y, width, RC.hlw);
		}
		Rectangle GetFloatLineBounds(RowInfo ri, RowInfo riPrev, bool vert) {
			if(IsRightToLeft) return GetRTLFloatLineBounds(ri, riPrev, vert);
			int x = ri.DataBounds.Left;
			if(vert) return new Rectangle(x, ri.DataBounds.Top, RC.vlw, ri.DataBounds.Height + RC.hlw);
			if(ShowGroupFooter && riPrev.LastFooter != null)
				return new Rectangle(x, riPrev.LastFooter.IndentInfo.Bounds.Bottom, ri.DataBounds.Width, RC.hlw);
			int y = TreeList.OptionsView.ShowPreview ? ri.Bounds.Top - RC.hlw : riPrev.Bounds.Bottom - 1;
			return new Rectangle(x, y, riPrev.DataBounds.Left - ri.DataBounds.Left, RC.hlw);
		}
		void UpdateRowCellFloatLines(RowInfo ri, bool shouldHideLeftVerticalLine) {
			if(RC.vlw != 0 && !shouldHideLeftVerticalLine)
				ri.FloatLines.Add(new LineInfo(GetFloatLineBounds(ri, null, true), PaintAppearance.VertLine));
			int visIndex = ri.VisibleIndex - TreeList.TopVisibleNodeIndex;
			if((RC.hlw != 0) && (visIndex > 0)) {
				RowInfo riPrev = RowsInfo.Rows[visIndex - 1] as RowInfo;
				bool allowHorzLine = (IsRightToLeft ? ri.DataBounds.Right > riPrev.DataBounds.Right : ri.DataBounds.Left < riPrev.DataBounds.Left) && TreeList.VisibleColumns.Count > 0;
				if(allowHorzLine) {
					if(ShowGroupFooter && riPrev.LastFooter != null)
						riPrev.LastFooter.FloatLines.Add(new LineInfo(GetFloatLineBounds(ri, riPrev, false), PaintAppearance.HorzLine));
					else if(TreeList.OptionsView.ShowPreview)
						ri.FloatLines.Add(new LineInfo(GetFloatLineBounds(ri, riPrev, false), PaintAppearance.HorzLine));
					else
						riPrev.FloatLines.Add(new LineInfo(GetFloatLineBounds(ri, riPrev, false), PaintAppearance.HorzLine));
				}
			}
		}
		Rectangle CalcRowCellLine(CellInfo cell, int width, bool isHorz, bool isRight) {
			if(isHorz) return new Rectangle(cell.Bounds.Left, cell.Bounds.Bottom, cell.Bounds.Width, RC.hlw);
			int x = cell.Bounds.X - width;			
			if(IsRightToLeft && isRight || (!IsRightToLeft && !isRight))
				x = cell.Bounds.Right;					 
			return new Rectangle(x, cell.Bounds.Top, width, cell.Bounds.Height + RC.hlw);
		}
		void UpdateRowCellLine(RowInfo ri, ColumnInfo ci, Rectangle boundsLine, AppearanceObject appearance) {			
			Rectangle line = UpdateFixedRange(boundsLine, ci);
			if(line.Width > 0) ri.Lines.Add(new LineInfo(line, appearance));
		}
		void UpdateRowCellLines(RowInfo ri, ColumnInfo ci, ColumnInfo nextColumn, CellInfo cell) {
			if(IsFixedRight(ci)) {
				Rectangle vLine = CalcRowCellLine(cell, TreeList.FixedLineWidth, false, true);
				UpdateRowCellLine(ri, ci, vLine, PaintAppearance.FixedLine);
			}
			if(IsFixedLeft(ci)) {
				Rectangle vLine = CalcRowCellLine(cell, TreeList.FixedLineWidth, false, false);
				UpdateRowCellLine(ri, ci, vLine, PaintAppearance.FixedLine);
			}				
			else if(RC.vlw != 0 && (nextColumn == null || nextColumn.Column == null || nextColumn.Column != FixedRightColumn)) {
				Rectangle vLine = CalcRowCellLine(cell, RC.vlw, false, false);
				UpdateRowCellLine(ri, ci, vLine, PaintAppearance.VertLine);
			}
			if(RC.hlw != 0) {
				Rectangle hLine = CalcRowCellLine(cell, RC.vlw, true, false);
				UpdateRowCellLine(ri, ci, hLine, PaintAppearance.HorzLine);
			}
		}
		internal void UpdateAnimation(TreeListNode node, TreeListColumn column, CellInfo cell) {
			EditorAnimationInfo info = GetAnimation(node, column, cell.EditorViewInfo);
			if(info != null)
				UpdateAnimatedItem(info, cell.EditorViewInfo);
			else if(ShouldAddAnimatedItem(node, column, cell.EditorViewInfo))
				AddAnimatedItem(node, column, cell.EditorViewInfo);
		}
		protected CellInfo GetCachedCellInfo(TreeListColumn column, ArrayList viewInfoList) {
			foreach(CellInfo cell in viewInfoList) {
				if(cell.Column == column)
					return cell;
			}
			return null;
		}
		protected virtual Point GetDataBoundsLocation(TreeListNode node, int top) {
			int indent = !HasFixedLeft && !TreeList.HasAllColumnsFixedRight ? TreeList.LeftCoord : 0;
			Point pt = new Point((IsRightToLeft ? ViewRects.Client.Right : ViewRects.Client.Left) + Direction * (GetDataBoundsLeftLocation(node) - indent + ViewRects.IndicatorWidth), top);
			return pt;
		}
		protected internal virtual int GetViewLevel(int level) {
			return (level + 1 - RootLevel);
		}
		protected virtual void CalcRowPreviewInfo(RowInfo ri) {
			if(!TreeList.OptionsView.ShowPreview) return;
			ri.PreviewText = TreeList.InternalGetPreviewText(ri.Node);
			if(ri.PreviewText == string.Empty) return;
			ri.PreviewBounds = new Rectangle(ri.DataBounds.Left, ri.DataBounds.Bottom + RC.hlw,
				ri.DataBounds.Width - RC.vlw, GetPreviewRowHeight(ri.PreviewText, ri.DataBounds.Width - RC.vlw - 2 * PreviewTextIndent, PaintAppearance.Preview, ri.Node));
			if(RC.hlw != 0)
				ri.Lines.Add(new LineInfo(ri.PreviewBounds.Left + RC.vlw, ri.PreviewBounds.Bottom, ri.PreviewBounds.Width - 2 * RC.vlw, RC.hlw, PaintAppearance.HorzLine));
			if(RC.vlw != 0) {
				ri.Lines.Add(new LineInfo(ri.PreviewBounds.Left, ri.PreviewBounds.Top, RC.vlw, ri.PreviewBounds.Height + RC.hlw, PaintAppearance.VertLine));
				ri.Lines.Add(new LineInfo(ri.PreviewBounds.Right, ri.PreviewBounds.Top, RC.vlw, ri.PreviewBounds.Height + RC.hlw, PaintAppearance.VertLine));
			}
			ri.SetHeight(ri.Bounds.Height + ri.PreviewBounds.Height + RC.hlw);
		}
		public int GetTotalNodeHeight(TreeListNode node) {
			ArrayList viList;
			RowInfo ri = CreateRowInfo(node);
			CalcConditionsCore(ri);
			CalcFormatRulesCore(ri);
			int previewWidth = ViewRects.Rows.Right - GetDataBoundsLocation(node, 0).X - RC.vlw - 2 * PreviewTextIndent;
			int previewHeight = GetPreviewRowHeight(TreeList.InternalGetPreviewText(node), previewWidth, PaintAppearance.Preview, node);
			return CalcNodeHeight(ri, out viList) * RowLineCount + HorzLineWidth + previewHeight + ((TreeList.OptionsView.ShowPreview && previewHeight > 0) ? HorzLineWidth : 0);
		}
		public int CalcVisibleNodeCount(TreeListNode topNode) {
			if(!IsValid && !TreeList.IsLockUpdate)
				CalcViewInfo();
			return CalcVisibleNodeCount(topNode, ViewRects.Rows.Height + ViewRects.EmptyRows.Height);
		}
		public int CalcVisibleNodeCount(TreeListNode topNode, int height) {
			int count = 0;
			TreeListNode node = topNode;
			while(height > 0 && node != null) {
				height -= GetTotalNodeHeight(node);
				if(height >= 0) {
					count++;
				}
				if(ShowGroupFooter && node.owner.IndexOf(node) == node.owner.Count - 1 && !node.Expanded)
					height -= RC.GroupFooterHeight;
				node = TreeListNodesIterator.GetNextVisible(node);
			}
			return count;
		}
		protected virtual void CalcSelectImage(RowInfo ri) {
			if(!CheckVisibleImage(ri, ri.SelectImageBounds)) return; 
			if(TreeList.SelectImageList != null) {
				int index = TreeList.InternalGetSelectImage(ri.Node, ri.VisibleIndex == TreeList.FocusedRowIndex);
				if(IsValidImageIndex(index, TreeList.SelectImageList)) {
					ri.SelectImageIndex = index;
					ri.SelectImageLocation = new Point(ri.SelectImageBounds.Left,
						ri.SelectImageBounds.Top + (ri.SelectImageBounds.Height - RC.SelectImageSize.Height) / 2);
				}
				else {
					ri.SelectImageIndex = -1;
				}
			}
		}
		protected virtual bool CheckVisibleImage(RowInfo ri, Rectangle imageBounds) {
			if(ri.IndicatorInfo.Bounds.IsEmpty) return true;
			if(imageBounds.Width <= 0) return false;
			if(IsRightToLeft && imageBounds.Left > ri.IndicatorInfo.Bounds.Left) return false;
			if(!IsRightToLeft && imageBounds.Right < ri.IndicatorInfo.Bounds.Right) return false;
			return true;
		}
		protected virtual void CalcStateImage(RowInfo ri) {
			ri.StateImageBounds.X = IsRightToLeft ? ri.SelectImageBounds.Left - RC.StateImageSize.Width : ri.SelectImageBounds.Right;
			if(!CheckVisibleImage(ri, ri.StateImageBounds)) return; 
			if(TreeList.StateImageList != null) {
				int index = TreeList.InternalGetStateImage(ri.Node);
				if(IsValidImageIndex(index, TreeList.StateImageList)) {
					ri.StateImageIndex = index;
					ri.StateImageLocation = new Point(ri.StateImageBounds.Left,
						ri.StateImageBounds.Top + (ri.StateImageBounds.Height - RC.StateImageSize.Height) / 2);
				}
				else {
					ri.StateImageIndex = -1;
				}
			}
		}
		bool IsValidImageIndex(int imageIndex, object imageList) { return ImageCollection.IsImageListImageExists(imageList, imageIndex); }
		protected virtual void CalcRowIndentItems(RowInfo ri) {
			if(ri.IndentInfo.Bounds.Right < ri.IndicatorInfo.Bounds.Right) return; 
			if(TreeList.VisibleColumns.Count == 0) return;
			CalcNodeIndents(ri.Node, TreeList.Nodes, ri.IndentInfo.IndentItems, ri.Level, RootLevel);
		}
		internal static void CalcNodeIndents(TreeListNode node, TreeListNodes rtNodes, ArrayList indents, int level, int rootLevel) {
			RowIndentItem indent;
			TreeListNode parent = TreeListFilterHelper.GetVisibleParent(node);
			for(int i = level; i >= rootLevel; i--) {
				if(i == level) {
					if(level == 0 && TreeListFilterHelper.IsFirstVisible(node) && TreeListFilterHelper.IsLastVisible(node)) {
						if(parent == null)
							indent = RowIndentItem.None;
						else
							indent = RowIndentItem.LastRoot;
					}
					else if(level == 0 && TreeListFilterHelper.IsFirstVisible(node)) {
						indent = RowIndentItem.FirstRoot;
					}
					else if(TreeListFilterHelper.IsLastVisible(node)) {
						indent = RowIndentItem.LastChild;
					}
					else {
						indent = RowIndentItem.NextChild;
					}
				}
				else {
					indent = TreeListFilterHelper.IsLastVisible(parent) ? RowIndentItem.None : RowIndentItem.Root;
					parent = TreeListFilterHelper.GetVisibleParent(parent);
				}
				indents.Add(indent);
			}
			indents.Reverse();
		}
		protected virtual Rectangle CalcClientRect(Rectangle scrollRect) {
			Rectangle clRect = scrollRect;
			if(TreeList.IsNeededHScrollBar && !TreeList.ScrollInfo.IsOverlapScrollbar)
				clRect.Height = Math.Max(0, clRect.Height - TreeList.ScrollInfo.HScrollHeight);
			if(TreeList.IsNeededVScrollBar && !TreeList.ScrollInfo.IsOverlapScrollbar) {
				clRect.Width = Math.Max(0, clRect.Width - TreeList.ScrollInfo.VScrollWidth);
				if(IsRightToLeft)
					clRect.X = scrollRect.Right - clRect.Width;
			}
			return clRect;
		}
		protected virtual Rectangle CalcFindPanelRect(Rectangle client) {
			Rectangle rect = Rectangle.Empty;
			if(TreeList.FindPanelVisible && TreeList.FindPanel != null) {
				rect = client;
				rect.Height = TreeList.FindPanel.Height;
				TreeList.FindPanel.Bounds = rect;
				TreeList.FindPanel.Visible = true;
			}
			return rect; 
		}
		protected virtual Rectangle CalcClipRect(Rectangle client) {
			Rectangle clip = client;
			if(TreeList.Nodes.Count > 0) {
				if(!IsRightToLeft)
					clip.X += ViewRects.IndicatorWidth;
				clip.Width -= ViewRects.IndicatorWidth;
			}
			clip.Height -= ViewRects.Footer.Height;
			return clip;
		}
		protected virtual int CalcMinHeight(AppearanceObject appearance) {
			if(appearance == null) return 0;
			return appearance.CalcDefaultTextSize(GInfo.Graphics).Height;
		}
		protected virtual void CalcLayoutHeights() {
			GInfo.AddGraphics(null);
			try {
				CalcRowHeight();
				CalcPlusMinusButtonSize();
				CalcPreviewRowHeight();
				RC.SetGroupFooterCellHeight(CalcFooterCellHeight(PaintAppearance.GroupFooter));
				SummaryFooterInfo.CellHeight = CalcFooterCellHeight(PaintAppearance.FooterPanel);
				CalcColumnPanelHeight();
				CalcBandPanelRowHeight();
				CalcFooterPanelHeight();
				CalcGroupFooterHeight();
				CalcIndicatorMinWidth();
				RC.hlw = HorzLineWidth;
				RC.vlw = VertLineWidth;
			} finally {
				GInfo.ReleaseGraphics();
			}
		}
		int CalcFooterCellHeight(AppearanceObject appearance) {
			int height = 0;
			GInfo.AddGraphics(null);
			try {
				FooterCellInfoArgs fcArgs = new FooterCellInfoArgs();
				fcArgs.SetAppearance(ClonePaintAppearance(appearance));
				height = ObjectPainter.CalcObjectMinBounds(GInfo.Graphics, ElementPainters.FooterCellPainter, fcArgs).Height;
			} finally {
				GInfo.ReleaseGraphics();
			}
			return height;
		}
		protected virtual void CalcPlusMinusButtonSize() {
			RC.PlusMinusButtonSize = ElementPainters.OpenCloseButtonPainter.CalcObjectMinBounds(new OpenCloseButtonInfoArgs(null, Rectangle.Empty, false, null, ObjectState.Normal)).Size;
			if(RowHeight - 2 > 0) RC.PlusMinusButtonSize.Height = Math.Min(RC.PlusMinusButtonSize.Height, RowHeight - 2);
		}
		protected virtual void CalcRowHeight() {
			AppearanceObject[] apps = GetRowMaxHeightAppearances(TreeNodeCellState.Dirty);
			int maxH = new TreeListEditorMinHeightCalculator(TreeList).CalcMinEditorHeight(GInfo, apps);
			maxH = (maxH % 2 == 0 ? maxH : maxH - 1);
			RC.SetMinRowHeight(maxH + 2 * CellTextIndent);
			if(!TreeList.OptionsView.ShowPreview) {
				if(RC.SelectImageSize.Height + 2 > RC.MinRowHeight) RC.SetMinRowHeight(RC.SelectImageSize.Height + 2);
				if(RC.StateImageSize.Height + 2 > RC.MinRowHeight) RC.SetMinRowHeight(RC.StateImageSize.Height + 2);
			}
			else {
				int oldHeight = RC.RowHeight;
				RC.SetRowHeight(RC.MinRowHeight);
				if(RC.SelectImageSize.Height + 2 > TotalRowHeight) RC.SetMinRowHeight(RC.SelectImageSize.Height + 2 - PreviewRowHeight);
				if(RC.StateImageSize.Height + 2 > TotalRowHeight) RC.SetMinRowHeight(RC.StateImageSize.Height + 2 - PreviewRowHeight);
				RC.SetRowHeight(oldHeight);
			}
			int rowHeight = TreeList.RowHeight; 
			if(TreeList.Handler.State != TreeListState.NodeSizing)
				rowHeight =  ScaleVertical(rowHeight);
			RC.SetRowHeight(TreeList.RowHeight == -1 ? RC.MinRowHeight : rowHeight);
		}
		protected virtual void CalcPreviewRowHeight() {
			RC.SetPreviewRowHeight(0);
			if(!TreeList.OptionsView.ShowPreview) return;
			if(!TreeList.PreviewColumnExists) return;
			RC.SetPreviewRowHeight(GetSimplePreviewRowHeight(PaintAppearance.Preview));
		}
		protected internal virtual void CalcColumnPanelHeight() {
			RC.SetColumnPanelHeight(0);
			if(TreeList.OptionsView.ShowColumns) {
				RC.SetColumnPanelHeight(GetColumnPanelHeight());
			}
		}
		protected virtual int CalcCaptionHeight(Rectangle client) {
			Graphics g = GInfo.AddGraphics(null);
			int height = 10;
			try {
				StyleObjectInfoArgs info = new StyleObjectInfoArgs(GInfo.Cache, client, PaintAppearance.Caption);
				height = ElementPainters.CaptionPainter.CalcObjectMinBounds(info).Height;
			}
			finally {
				GInfo.ReleaseGraphics();
			}
			return Math.Max(height, TreeList.CaptionHeight);
		}
		public virtual int GetColumnPanelHeight() {
			int cph = GetMinColumnPanelHeight();
			if(TreeList.ColumnPanelRowHeight != -1)
				cph = Math.Max(ScaleVertical(TreeList.ColumnPanelRowHeight), cph);
			return cph;
		}
		protected internal virtual void CalcBandPanelRowHeight() {
			RC.SetBandPanelRowHeight(0);
			if(TreeList.ActualShowBands)
				RC.SetBandPanelRowHeight(GetBandPanelRowHeight());
		}
		protected virtual void CalcBandPanelRowCount() {
			this.bandPanelRowCount = GetBandPanelRowCount();
		}
		protected virtual void CalcColumnPanelRowCount() {
			this.columnPanelRowCount = this.rowLineCount = GetColumnPanelRowCount();
		}
		public virtual int GetBandPanelRowHeight() {
			int bph = GetMinBandPanelRowHeight();
			if(TreeList.BandPanelRowHeight != -1)
				bph = Math.Max(ScaleVertical(TreeList.BandPanelRowHeight), bph);
			return bph;
		}
		protected internal virtual ColumnInfo CreateColumnInfo(TreeListColumn column) {
			ColumnInfo ci = new ColumnInfo(column);
			ci.RightToLeft = IsRightToLeft;
			ci.AutoHeight = TreeList.IsColumnHeaderAutoHeight;
			return ci;
		}
		public virtual int GetMinBandPanelRowHeight() {
			int height = 0;
			GInfo.AddGraphics(null);
			try {
				BandInfo bi = CreateBandInfo(null);
				bi.Caption = "Wg";
				if(IsValidImageIndex(0, TreeList.ColumnsImageList)) {
					if(TreeList.OptionsView.AllowGlyphSkinning)
						bi.InnerElements.Add(new DrawElementInfo(new SkinnedGlyphElementPainter(), new SkinnedGlyphElementInfoArgs(TreeList.ColumnsImageList, 0, null), StringAlignment.Near));
					else
						bi.InnerElements.Add(new DrawElementInfo(new GlyphElementPainter(), new GlyphElementInfoArgs(TreeList.ColumnsImageList, 0, null), StringAlignment.Near));
				}
				bi.SetAppearance(PaintAppearance.BandPanel);
				height = ObjectPainter.CalcObjectMinBounds(GInfo.Graphics, ElementPainters.BandPainter, bi).Height;
				if(TreeList.OptionsView.AllowHtmlDrawHeaders) {
					bi.UseHtmlTextDraw = true;
					bi.HtmlContext = TreeList;
					for(int n = 0; n < TreeList.Bands.Count; n++) {
						TreeListBand band = TreeList.Bands[n];
						if(!band.Visible) continue;
						bi.Caption = band.Caption;
						AppearanceObject app = new AppearanceObject();
						AppearanceHelper.Combine(app, new AppearanceObject[] { band.AppearanceHeader, PaintAppearance.BandPanel });
						bi.SetAppearance(app);
						height = Math.Max(height, ObjectPainter.CalcObjectMinBounds(GInfo.Graphics, ElementPainters.BandPainter, bi).Height);
					}
				}
			}
			finally {
				GInfo.ReleaseGraphics();
			}
			return height;
		}
		public virtual int GetMinColumnPanelHeight() {
			int height = 0;
			GInfo.AddGraphics(null);
			try {
				ColumnInfo ci = CreateColumnInfo(null);
				ci.Caption = "Wg";
				ci.SetAppearance(PaintAppearance.HeaderPanel);
				SortedShapeObjectInfoArgs sortArgs = new SortedShapeObjectInfoArgs();
				sortArgs.Ascending = true;
				ci.InnerElements.Add(new DrawElementInfo(ElementPainters.SortedShapePainter, sortArgs));
				if(IsValidImageIndex(0, TreeList.ColumnsImageList)) {
					if(TreeList.OptionsView.AllowGlyphSkinning)
						ci.InnerElements.Add(new DrawElementInfo(new SkinnedGlyphElementPainter(), new SkinnedGlyphElementInfoArgs(TreeList.ColumnsImageList, 0, null), StringAlignment.Near));
					else
						ci.InnerElements.Add(new DrawElementInfo(new GlyphElementPainter(), new GlyphElementInfoArgs(TreeList.ColumnsImageList, 0, null), StringAlignment.Near));
				}
				if(TreeList.GetHeaderFilterButtonShowMode() == FilterButtonShowMode.SmartTag) 
					ci.InnerElements.Add(new DrawElementInfo(ElementPainters.SmartFilterButton, new TreeListFilterButtonInfoArgs()));
				else
					ci.InnerElements.Add(new DrawElementInfo(ElementPainters.FilterButton, new TreeListFilterButtonInfoArgs()));
				height = ObjectPainter.CalcObjectMinBounds(GInfo.Graphics, ElementPainters.HeaderPainter, ci).Height;
				if(TreeList.OptionsView.AllowHtmlDrawHeaders || TreeList.IsColumnHeaderAutoHeight) {
					if(TreeList.OptionsView.AllowHtmlDrawHeaders) {
						ci.UseHtmlTextDraw = true;
						ci.HtmlContext = TreeList;
					}
					for(int n = 0; n < TreeList.VisibleColumns.Count; n++) {
						TreeListColumn column = TreeList.VisibleColumns[n];
						AppearanceObject app = new AppearanceObject();
						AppearanceHelper.Combine(app, new AppearanceObject[] { column.AppearanceHeader, PaintAppearance.HeaderPanel });
						ci.SetAppearance(app);
						ci.Caption = column.GetCaption();
						if(TreeList.IsColumnHeaderAutoHeight) {
							ci.CaptionRect = Rectangle.Empty;
							ci.Bounds = new Rectangle(0, 0, column.VisibleWidth, 0);
							ci.SetColumn(column);
							UpdateGlyphInfo(ci);
							ci.UpdateCaption();
						}
						height = Math.Max(height, ObjectPainter.CalcObjectMinBounds(GInfo.Graphics, ElementPainters.HeaderPainter, ci).Height);
					}
				}
			} finally {
				GInfo.ReleaseGraphics();
			}
			return height;
		}
		public virtual void CalcIndicatorWidth() {
			if(RC.NeedsRestore) {
				GInfo.AddGraphics(null);
				try {
					CalcIndicatorMinWidth();
				}
				finally {
					GInfo.ReleaseGraphics();
				}
			}
			ViewRects.IndicatorWidth = (TreeList.OptionsView.ShowIndicator ? Math.Max(RC.IndicatorWidth, ScaleHorizontal(TreeList.IndicatorWidth)) : 0);
		}
		public virtual void CalcColumnTotalWidth() {
			CalcIndicatorWidth();
			ViewRects.ColumnTotalWidth = ViewRects.BandTotalWidth = ViewRects.IndicatorWidth;
			if(TreeList.Bands.Count == 0) {
				foreach(TreeListColumn c in TreeList.VisibleColumns)
					ViewRects.ColumnTotalWidth += c.VisibleWidth;
			}
			else {
				foreach(TreeListBand band in TreeList.Bands) {
					if(band.Visible)
						ViewRects.ColumnTotalWidth += band.VisibleWidth;
				}
				ViewRects.BandTotalWidth = ViewRects.ColumnTotalWidth;
			}
		}
		public virtual void CalcMaxIndents() {
			ViewRects.MaxIndents = 0;
			if(TreeList.Nodes.Count > 0) { 
				ViewRects.MaxIndents = GetMaxIndent(TreeList.Nodes, 0);
				if(TreeList.OptionsView.ShowRoot) {
					ViewRects.MaxIndents++;
				}
			}
		}
		protected virtual int GetMaxIndent(TreeListNodes nodes, int countUpNodes) {
			int max = 0, i = 0;
			foreach(TreeListNode child in nodes) {
				if(child.HasChildren) {
					if(TreeList.BestFitVisibleOnly && !child.Expanded) continue; 
					i++;
					max = Math.Max(max, GetMaxIndent(child.Nodes, i));
					i = 0;
				}
			}
			return countUpNodes + max;
		}
		protected virtual int GetSimplePreviewRowHeight(AppearanceObject appearance) {
			return TreeList.PreviewLineCount * CalcMinHeight(appearance) + RC.hlw;
		}
		public virtual int GetPreviewRowHeight(string previewText, int previewWidth, AppearanceObject appearance, TreeListNode node) {
			if(!TreeList.OptionsView.ShowPreview || previewText == string.Empty) return 0;
			GInfo.AddGraphics(null);
			try {
				int height = TreeList.RaiseMeasurePreviewHeight(node);
				if(height > -1) return height;
				if(!TreeList.OptionsView.AutoCalcPreviewLineCount)
					return GetSimplePreviewRowHeight(appearance);
				return Convert.ToInt32(appearance.CalcTextSize(GInfo.Graphics, appearance.GetStringFormat(GetPreviewTextOptions()),
					previewText, previewWidth).Height) + RC.hlw;
			} 
			finally {
				GInfo.ReleaseGraphics();
			}
		}
		static TextOptions defaultTextOptionsPreview = new TextOptions(HorzAlignment.Near, VertAlignment.Top, WordWrap.Wrap, DevExpress.Utils.Trimming.EllipsisCharacter);
		protected internal static TextOptions GetPreviewTextOptions() {
			return defaultTextOptionsPreview;
		}
		public virtual void CalcFooterPanelHeight() {
			RC.SetFooterPanelHeight(0);
			if(TreeList.OptionsView.ShowSummaryFooter) 
				RC.SetFooterPanelHeight(GetFooterPanelHeight(SummaryFooterInfo.CellHeight));
			treeList.ScrollInfo.FooterHeight = FooterPanelHeight;
		}
		public virtual void CalcGroupFooterHeight() {
			RC.SetGroupFooterHeight(0);
			if(ShowGroupFooter)
				RC.SetGroupFooterHeight(GetFooterPanelHeight(RC.GroupFooterCellHeight));
		}
		public virtual void CalcIndicatorMinWidth() {
			RC.SetIndicatorWidth(0);
			if(TreeList.OptionsView.ShowIndicator) {
				IndicatorObjectInfoArgs e = new IndicatorObjectInfoArgs(ElementPainters.IndicatorImageCollection);
				e.Graphics = GInfo.Graphics;
				e.SetAppearance(ClonePaintAppearance(PaintAppearance.HeaderPanel));
				e.Bounds = new Rectangle(Point.Empty, ElementPainters.IndicatorImageSize);
				RC.SetIndicatorWidth(ElementPainters.IndicatorPainter.CalcObjectMinBounds(e).Width);
			}
		}
		public virtual int GetFooterPanelHeight(int cellHeight) {
			return GetFooterPanelHeightCore(cellHeight, ColumnPanelRowCount);
		}
		protected internal virtual int GetFooterPanelHeightCore(int cellHeight, int rowCount) {
			FooterPanelInfoArgs fpArgs = new FooterPanelInfoArgs(null, PaintAppearance.GroupFooter, new Rectangle(0, 0, 10, cellHeight), rowCount, cellHeight);
			return Math.Max(ElementPainters.FooterPanelPainter.CalcObjectMinBounds(fpArgs).Height, ScaleVertical(TreeList.FooterPanelHeight));
		}
		public virtual Rectangle CalcScrollRect(Rectangle windowRect) {
			return ElementPainters.BorderPainter.GetObjectClientRectangle(ElementPainters.GetBorderPainterInfoArgs(null, windowRect, null));
		}
		protected virtual Rectangle CalcWindowRect(Rectangle clientRect) {
			return clientRect;
		}
		internal void CalcBeyondScrollSquare() {
			ViewRects.BeyondScrollSquare = Rectangle.Empty;
			if(TreeList == null) return;
			if(!TreeList.ScrollInfo.IsOverlapScrollbar && TreeList.ScrollInfo.HScrollVisible && TreeList.ScrollInfo.VScrollVisible)
				ViewRects.BeyondScrollSquare = new Rectangle(TreeList.ScrollInfo.VScroll.Left,
					TreeList.ScrollInfo.HScroll.Top, TreeList.ScrollInfo.VScroll.Width,
					TreeList.ScrollInfo.HScroll.Height);
		}
		public virtual ColumnInfo GetColumnInfoByPoint(Point pt, int increasedHeight, ColumnsInfo info) {
			if(HasFixedLeft || HasFixedRight) {
				ColumnInfo fci = GetColumnInfoByPoint(pt, increasedHeight, true, info);
				if(fci != null) return fci;
			}
			return GetColumnInfoByPoint(pt, increasedHeight, false, info);
		}
		protected ColumnInfo GetColumnInfoByPoint(Point pt, int increasedHeight, bool onlyFixedColumns, ColumnsInfo info) {
			foreach(ColumnInfo ci in info.Columns) {
				Rectangle bounds = ci.Bounds;
				bounds.Height += increasedHeight;
				if(onlyFixedColumns && !IsFixedLeftPaint(ci) && !IsFixedRightPaint(ci)) continue;
				if(bounds.Contains(pt)) return ci;
			}
			return null;
		}
		public virtual RowInfo GetRowInfoByPoint(Point pt) {
			if(AutoFilterRowInfo != null && AutoFilterRowInfo.Bounds.Contains(pt))
				return AutoFilterRowInfo;
			foreach(RowInfo ri in RowsInfo.Rows) {
				if(ri.Bounds.Contains(pt)) 
					return ri;
			}
			return null;
		}
		public virtual RowFooterInfo GetRowFooterInfoByPoint(Point pt) {
			foreach(RowFooterInfo fi in RowsInfo.RowFooters) {
				if(fi.Bounds.Contains(pt)) 
					return fi;
			}
			return null;
		}
		public virtual void ChangeFocusedRow(RowInfo riNew, RowInfo riOld) {
			UpdateRowViewInfo(riNew);
			UpdateRowViewInfo(riOld);
		}
		private void CalcCellViewInfo(RowInfo ri, CellInfo cell, int index) {
			if(EmptyCellInfo.IsEmptyCell(cell)) return;
			Rectangle bounds = (index == 0 ? CorrectImagesInfo(ri, cell) : cell.CellValueRect);
			UpdateCellInfo(cell, ri.Node, true);
			UpdateAnimation(ri.Node, cell.Column, cell);
		}
		public AppearanceObject GetWidestRowAppearance() {
			AppearanceObject result = null;
			int lastWidth = 0;
			GInfo.AddGraphics(null);
			try {
				AppearanceObject[] apps = new AppearanceObject[] { 
																	 PaintAppearance.Row, PaintAppearance.FocusedCell, PaintAppearance.FocusedRow, PaintAppearance.SelectedRow,
																	 PaintAppearance.EvenRow, PaintAppearance.OddRow};
				for(int n = 0; n < apps.Length; n++) {
					int w = Convert.ToInt32(GInfo.Graphics.MeasureString("Wg", apps[n].Font, 100).Width);
					if(w > lastWidth || result == null)
						result = apps[n];
					lastWidth = w;
				}
			} finally {
				GInfo.ReleaseGraphics();
			}
			return result;
		}
		protected virtual void UpdateEditorInfo(CellInfo cell) {
			if(cell.EditorViewInfo == null) return;
			cell.EditorViewInfo.DetailLevel = DetailLevel.Minimum;
			ShowButtonModeEnum showMode = TreeList.GetColumnShowButtonMode(cell.Column);
			if(showMode == ShowButtonModeEnum.ShowAlways ||	
				(showMode == ShowButtonModeEnum.ShowForFocusedRow && cell.RowInfo.VisibleIndex == TreeList.FocusedRowIndex) ||
				(showMode == ShowButtonModeEnum.ShowForFocusedCell && cell.RowInfo.VisibleIndex == TreeList.FocusedRowIndex && cell.Column.VisibleIndex == TreeList.FocusedCellIndex)) {
				if(TreeList.IsLookUpMode && !IsAutoFilterRow(cell.RowInfo) && !TreeList.Editable)
					cell.EditorViewInfo.DetailLevel = DetailLevel.Minimum;
				else
					cell.EditorViewInfo.DetailLevel = DetailLevel.Full;
			}
			if(cell.Item.LookAndFeel.UseDefaultLookAndFeel) 
				cell.EditorViewInfo.LookAndFeel = TreeList.ElementsLookAndFeel;
			if(IsAutoFilterRow(cell.RowInfo)) return;
			cell.RowInfo.FormatRuleInfo.ApplyContextImage(GInfo.Cache, cell.Column, cell.RowInfo.Node, cell.EditorViewInfo.Bounds, cell.EditorViewInfo);
			string error;
			DevExpress.XtraEditors.DXErrorProvider.ErrorType errorType;
			TreeList.GetColumnError(cell.Column, cell.RowInfo.Node, out error, out errorType);
			cell.EditorViewInfo.ErrorIconText = error;
			cell.EditorViewInfo.ShowErrorIcon = (cell.EditorViewInfo.ErrorIconText != null && cell.EditorViewInfo.ErrorIconText.Length > 0);
			if(cell.EditorViewInfo.ShowErrorIcon)
				cell.EditorViewInfo.ErrorIcon = DevExpress.XtraEditors.DXErrorProvider.DXErrorProvider.GetErrorIconInternal(errorType);
		}
		protected virtual Rectangle CorrectImagesInfo(RowInfo ri, CellInfo cell) {
			int sumw = ri.SelectImageBounds.Width + ri.StateImageBounds.Width;
			CalcSelectImage(ri);
			CalcStateImage(ri);
			int dx = (ri.SelectImageBounds.Width + ri.StateImageBounds.Width) - sumw;
			ri.DataBounds.Offset(dx, 0);
			ri.DataBounds.Width -= dx;
			Rectangle bounds = new Rectangle(cell.EditorViewInfo.Bounds.Left + dx, cell.EditorViewInfo.Bounds.Top, cell.EditorViewInfo.Bounds.Width - dx, cell.EditorViewInfo.Bounds.Height);
			if(TreeList.OptionsView.ShowHorzLines && ri.FloatLines.Count > 0 && !IsRightToLeft) {
				LineInfo li = ri.FloatLines[0] as LineInfo;
				li.Rect.X = bounds.X - CellTextIndent - 1;
			}
			return bounds;
		}
		public virtual void ChangeFocusedCell(RowInfo ri, int newIndex, int oldIndex) {
			UpdateRowViewInfo(ri);
		}
		public virtual void UpdateRowViewInfo(RowInfo ri) {
			if(ri == null) return;
			ri.SetDirty();
			for(int i = 0; i < ri.Cells.Count; i++) {
				CellInfo cell = ri.Cells[i] as CellInfo;
				CalcCellViewInfo(ri, cell, i);
			}
			ri.IndicatorInfo.ImageIndex = GetRowIndicatorImageIndex(ri);
		}
		protected internal virtual int GetRowIndicatorImageIndex(RowInfo ri) {
			if(IsAutoFilterRow(ri)) return TreeListPainter.AutoFilterRowIndicatorImageIndex;
			if((ri.RowState & TreeNodeCellState.Focused) != 0)
				return GetFocusedNodeIndicatorImageIndex(ri);
			return (IsTextEmpty(ri.ErrorText) ? -1 : TreeListPainter.ErrorInNodeIndicatorImageIndex);
		}
		public virtual Rectangle SetFocusedRowActive(int AbsIndex, bool hasFocus) {
			int index = AbsIndex - TreeList.TopVisibleNodeIndex;
			if(index < 0 || index >= RowsInfo.Rows.Count) return Rectangle.Empty;
			RowInfo ri = RowsInfo.Rows[index] as RowInfo;
			UpdateRowViewInfo(ri);
			return ri.Bounds;
		}
		public virtual RowInfo GetDragNodeRowInfo(RowInfo dragRow, Rectangle bounds) {
			TreeListNode node = dragRow.Node;
			if(node == null) return null;
			RowInfo ri = CreateRowInfo(node);
			ri.RowState = TreeNodeCellState.Dirty;
			UpdateRowPaintAppearance(ri);
			int edgeX = IsRightToLeft ? bounds.Right - dragRow.SelectImageBounds.Right : (dragRow.SelectImageBounds.X - bounds.X);
			int edgeY = bounds.Y;
			ri.Bounds = bounds;
			Func<Rectangle, Rectangle> GetObjectBounds = (oldBounds) =>
			{
				Point location = new Point(oldBounds.X - Direction * edgeX, edgeY);
				return new Rectangle(location, oldBounds.Size);
			};
			ri.DataBounds = GetObjectBounds(dragRow.DataBounds);
			ri.SelectImageBounds = GetObjectBounds(dragRow.SelectImageBounds);
			CalcSelectImage(ri);
			ri.StateImageBounds = GetObjectBounds(dragRow.StateImageBounds);
			CalcStateImage(ri);
			int cellEdgeX = ri.Bounds.Width - (dragRow.DataBounds.Width + dragRow.SelectImageBounds.Width + dragRow.StateImageBounds.Width);
			if(dragRow.Cells != null) {
				foreach(CellInfo dragCell in dragRow.Cells) {
					CellInfo cell = CreateCellInfo(dragCell.ColumnInfo, ri);
					Rectangle cellBounds = dragCell.Bounds;
					cellBounds.Y = ri.DataBounds.Y;
					if(dragCell == dragRow.Cells[0]) {
						cellBounds.X -= Direction * edgeX;
						cellBounds.Width = cellBounds.Width + cellEdgeX;
					}
					else
						cellBounds.X -= Direction * (edgeX - cellEdgeX);
					FillCellViewInfo(cell, dragCell.Column, ri.Node, cellBounds, ri.PaintAppearance, true);
					UpdateRowCellLines(ri, cell.ColumnInfo, GetNextColumn(cell.ColumnInfo), cell);
					ri.Cells.Add(cell);
				}
			}
			UpdateRowCellFloatLines(ri, false);
			return ri;
		}
		internal void FillCellViewInfo(CellInfo cell, TreeListColumn col, TreeListNode node, Rectangle br, AppearanceObject apperance, bool canUseCustomStyle) {
			if(cell.EditorViewInfo != null) {
				cell.EditorViewInfo.Bounds = Rectangle.Inflate(br, -CellInfo.CellTextIndent, -CellInfo.CellTextIndent);
				cell.EditorViewInfo.State = ObjectState.Disabled;
				if(canUseCustomStyle)
					apperance = TreeList.InternalGetCustomNodeCellStyle(col, node, apperance);
				AppearanceHelper.Combine(cell.PaintAppearance, new AppearanceObject[] { apperance });
				cell.EditorViewInfo.DetailLevel = DetailLevel.Minimum;
				UpdateCell(cell, col, node);
			}
		}
		protected internal virtual void UpdateCell(CellInfo cell, TreeListColumn col, TreeListNode node) {
			if(!col.Format.IsEmpty)
				cell.EditorViewInfo.Format = col.Format;
			cell.EditorViewInfo.EditValue = col.TreeList.InternalGetNodeValue(node, col);
			cell.EditorViewInfo.DefaultBorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			cell.EditorViewInfo.AllowDrawFocusRect = false;
			cell.EditorViewInfo.FillBackground = false;
			try {
				GInfo.AddGraphics(null);
				cell.CalcViewInfo(GInfo.Graphics);
			}
			finally {
				GInfo.ReleaseGraphics();
			}
		}
		public virtual Rectangle PressColumnButton(bool pressed) {
			if(ColumnsInfo.Columns.Count > 0) {
				ColumnInfo ci = (ColumnInfo) ColumnsInfo.Columns[0];
				if(ci.Type != ColumnInfo.ColumnInfoType.ColumnButton) return Rectangle.Empty;
				ci.SetAppearance(ClonePaintAppearance(PaintAppearance.HeaderPanel));
				ci.Pressed = pressed;
				return ci.Bounds;
			}
			return Rectangle.Empty;
		}
		public virtual Rectangle PressBandButton(bool pressed) {
			if(BandsInfo.Count == 0) return Rectangle.Empty;
			BandInfo bi = BandsInfo[0];
			if(bi.Type != ColumnInfo.ColumnInfoType.ColumnButton) return Rectangle.Empty;
			bi.SetAppearance(ClonePaintAppearance(PaintAppearance.BandPanel));
			bi.Pressed = pressed;
			return bi.Bounds;
		}
		public static bool IsSamePaintBackground(AppearanceObject value1, AppearanceObject value2) {
			return value1 == value2 || (value1.GetBackColor() == value2.GetBackColor() && 
				value1.GetBackColor2() == value2.GetBackColor2() && value1.GetGradientMode() == value2.GetGradientMode());
		}
		public static Rectangle GetExcludeEmptyRect(Rectangle bounds, Rectangle dataBounds, bool indentAsRowStyle) {
			Rectangle result = bounds;
			if(!indentAsRowStyle && dataBounds.Left > result.Left) {
				int cx = dataBounds.Left - result.Left;
				result.X += cx;
				result.Width -= cx;
			}
			if(result.Right > dataBounds.Right) {
				result.Width -= result.Right - dataBounds.Right;
			}
			return result;
		}
		public virtual CellInfo CreateCellInfo(ColumnInfo ci, RowInfo ri) {
			return new CellInfo(ci, ri); 
		}
		public virtual int RowHeight { get { return RC.RowHeight; } }
		public Size BorderSize {
			get {
				Rectangle client = new Rectangle(0, 0, 20, 20);
				Rectangle bounds = ElementPainters.BorderPainter.CalcBoundsByClientRectangle(new ObjectInfoArgs(null, client, ObjectState.Normal));
				return new Size(bounds.Width - client.Width, bounds.Height - client.Height); }
		}
		protected virtual int PreviewRowHeight { get { return RC.PreviewRowHeight; } }
		public virtual int ColumnPanelHeight { get { return RC.ColumnPanelHeight; } }
		public virtual int ColumnPanelRowCount { get { return columnPanelRowCount; } }
		public virtual int RowLineCount { get { return rowLineCount; } } 
		public virtual int BandPanelRowHeight { get { return RC.BandPanelRowHeight; } }
		public virtual int BandPanelRowCount { get { return bandPanelRowCount; } }
		public virtual int TotalRowHeight { 
			get { return RowHeight * RowLineCount + RC.hlw + 
					  (TreeList.OptionsView.ShowPreview && TreeList.PreviewColumnExists ?
					  PreviewRowHeight + RC.hlw : 0); 
			} 
		}
		public virtual int FooterPanelHeight { get { return RC.FooterPanelHeight; } }
		public virtual int FirstColumnIndent {
			get {
				if(TreeList.BestFitVisibleOnly) CalcMaxIndents(); 
				return ViewRects.MaxIndents * RC.LevelWidth +
					RC.SelectImageSize.Width + RC.StateImageSize.Width + RC.CheckBoxSize.Width;
			}
		}
		public virtual int VisibleRowCount {
			get {
				int count = RowsInfo.Rows.Count;
				if(count > 0) {
					RowInfo riFirst = (RowInfo) RowsInfo.Rows[0];
					RowInfo riLast = (RowInfo) RowsInfo.Rows[count - 1];
					if(riFirst.Bounds.Top < ViewRects.Rows.Top) count--;
					if(riLast.Bounds.Bottom > ViewRects.Rows.Bottom) count--;
				}
				return count;
			}
		}
		public Rectangle EmptyArea {
			get {
				Rectangle result = ViewRects.EmptyUnionArea;
				if(!TreeList.OptionsView.ShowIndentAsRowStyle && result.IsEmpty && ViewRects.MaxIndents > 0) {
					result = ViewRects.Rows;
				}
				return result;
			}
		}
		public bool IsValid { get { return isValid; } set { isValid = value; } }
		protected virtual int PreviewTextIndent { get { return 2; } }
		protected virtual int CellTextIndent { get { return CellInfo.CellTextIndent; } }
		protected virtual bool BehindColumnIsVisible {
			get {
				if(ColumnsInfo.Columns.Count == 0) return false;
				ColumnInfo ci = ColumnsInfo.Columns[ColumnsInfo.Columns.Count - 1] as ColumnInfo;
				if(ci.Type == ColumnInfo.ColumnInfoType.BehindColumn) {
					if(IsRightToLeft) return ci.Bounds.Right > ViewRects.Client.Left;
					return ci.Bounds.Left < ViewRects.Client.Right;
				}
				return false;
			}
		}
		bool IsTextEmpty(string text) { return (text == null || text == string.Empty); }
		protected virtual int GetFocusedNodeIndicatorImageIndex(RowInfo ri) {
			if(!IsTextEmpty(ri.ErrorText)) return TreeListPainter.ErrorInFocusedNodeIndicatorImageIndex;
			if(TreeList.IsFocusedNodeDataModified) return TreeListPainter.EditorIsModifiedIndicatorImageIndex;
			if(TreeList.ActiveEditor != null) {
				return (TreeList.ActiveEditor.IsModified ? TreeListPainter.EditorIsModifiedIndicatorImageIndex : TreeListPainter.EditorIsActiveIndicatorImageIndex);
			}
			return TreeListPainter.EditorIsInactiveIndicatorImageIndex;
		}
		protected virtual int RootLevel {
			get { return (TreeList.OptionsView.ShowRoot ? 0 : 1); }
		}
		protected internal ElementPainters ElementPainters { get { return TreeList.Painter.ElementPainters; } }
		public virtual void LineBrushChanged(LineStyle style, AppearanceObject appearance) {
			RC.CreateLineBrush(appearance, style);
		}
		protected internal virtual int GetSpecialRowSeparatorHeight() {
			return ElementPainters.SpecialRowSeparatorPainter.CalcObjectMinBounds(new ObjectInfoArgs(GInfo.Cache)).Height;
		}
		int HorzLineWidth { get { return (TreeList.OptionsView.ShowHorzLines ? 1 : 0); } }
		int VertLineWidth { get { return (TreeList.OptionsView.ShowVertLines ? 1 : 0); } }
		bool IsEnableAppearanceEvenRow { get { return TreeList.OptionsView.EnableAppearanceEvenRow; } }
		bool IsEnableAppearanceOddRow { get { return TreeList.OptionsView.EnableAppearanceOddRow; } }
		bool ShowGroupFooter { get { return TreeList.OptionsView.ShowRowFooterSummary; } }
		protected internal Region EmptyAreaRegion { get { return emptyAreaRegion; } }
		public virtual bool ShowFilterPanel {
			get {
				if(TreeList.OptionsView.ShowFilterPanelMode == ShowFilterPanelMode.Never) return false;
				if(TreeList.OptionsView.ShowFilterPanelMode == ShowFilterPanelMode.ShowAlways) return true;
				return !ReferenceEquals(TreeList.ActiveFilterCriteria, null);
			}
		}
		public ColumnsInfo ColumnsInfo { get { return columnsInfo; } }
		public BandsInfo BandsInfo { get { return bandsInfo; } }
		public RowsInfo RowsInfo { get { return rowsInfo; } }
		public SummaryFooterInfo SummaryFooterInfo { get { return summaryFooterInfo; } }
		public ViewRects ViewRects { get { return viewRects; } }
		public ResourceInfo RC { get { return rc; } }
		public AppearanceObject EvenAppearance { get { return (IsEnableAppearanceEvenRow ? PaintAppearance.EvenRow : null); } }
		public AppearanceObject OddAppearance { get { return (IsEnableAppearanceOddRow ? PaintAppearance.OddRow : null); } }
		internal bool IsRowVisible(RowInfo ri) {
			if(ri == null || ri.Bounds.Height < 1) return false;
			if(ri.Bounds.Bottom > treeList.ViewInfo.ViewRects.Rows.Bottom || ri.Bounds.Top < treeList.ViewInfo.ViewRects.Rows.Top)
				return false;
			return true;
		}
		internal bool IsRowVisible(int visibleIndex) {
			return IsRowVisible(RowsInfo[TreeList.GetNodeByVisibleIndex(visibleIndex)]);
		}
		#region Pixel Scrolling
		protected internal virtual int CalcVisibleIndexByPixelPosition(int value) {
			return TreeList.GetVisibleIndexByNode(pixelScrollingInfo.GetNodeByPixelPosition(value));
		}
		protected internal virtual int CalcPixelPositionByVisibleIndex(int value) {
			return pixelScrollingInfo.GetPixelPositionByNode(TreeList.GetNodeByVisibleIndex(value));
		}
		protected internal int CalcTotalScrollableRowsHeight() {
			return pixelScrollingInfo.GetTotalScrollableRowsHeight();
		}
		#endregion
		#region scaling
		public virtual int ScaleVertical(int height) {
			return RectangleHelper.ScaleVertical(height, TreeList.ScaleFactor.Height);
		}
		public virtual int ScaleHorizontal(int width) {
			return RectangleHelper.ScaleHorizontal(width, TreeList.ScaleFactor.Width);
		}
		#endregion
		#region Bands
		protected virtual BandInfo CreateBandInfo(TreeListBand band) {
			BandInfo bi = new BandInfo(band);
			bi.RightToLeft = IsRightToLeft;
			return bi;
		}
		void CheckLeftCoord(ref int edge, ref bool leftCoordSubstracted, bool condition) {
			if(!condition || leftCoordSubstracted) return;
			edge -= Direction * TreeList.LeftCoord;
			leftCoordSubstracted = true;
		}
		int CalcHeaderButton(IHeaderObjectInfo hi, int edge, int top, int height) {
			hi.Type = ColumnInfo.ColumnInfoType.ColumnButton;
			Rectangle bounds = GetHeaderBounds(edge, top, ViewRects.IndicatorWidth, height);
			if(hi is BandInfo) {
				BandInfo bi = hi as BandInfo;
				CalcBandInfo(bi, bounds, 0, true);
				BandsInfo.Add(bi);
			}
			if(hi is ColumnInfo) {
				ColumnInfo ci = hi as ColumnInfo;
				CalcColumnInfo(ci, bounds, false);
				ColumnsInfo.Columns.Add(ci);
			}
			return Direction * hi.Bounds.Width;
		}
		protected internal virtual void CalcBandsInfo() {
			if(!TreeList.HasBands && !TreeList.ActualShowBands) return;
			int bandEdge = IsRightToLeft ? ViewRects.BandPanel.Right : ViewRects.BandPanel.Left,
				top = ViewRects.BandPanel.Top;
			bool leftCoordSubstracted = false;
			if(TreeList.OptionsView.ShowIndicator)
				bandEdge += CalcHeaderButton(CreateBandInfo(null), bandEdge, top, BandPanelRowHeight * BandPanelRowCount);
			CheckLeftCoord(ref bandEdge, ref leftCoordSubstracted, FixedLeftBand == null);
			for(int i = 0; i < TreeList.Bands.Count + 1; i++) {
				TreeListBand band = null;
				int width = 0;
				band = (i >= 0 && i < TreeList.Bands.Count ? TreeList.Bands[i] : null);
				BandInfo bi = CreateBandInfo(band);
				int rowCount = band == null ? 1 : band.RowCount;
				if(i == TreeList.Bands.Count) {					
					width = GetLastColumnWidth(bandEdge, ViewRects.ActualBandPanel);
					if(width == -1) break;
					bi.Type = ColumnInfo.ColumnInfoType.BehindColumn;
					rowCount = BandPanelRowCount;
				}
				if(band != null && !band.Visible) continue;
				CheckLeftCoord(ref bandEdge, ref leftCoordSubstracted, band != null && band.Fixed != FixedStyle.Left);				
				CheckOffsetFixedRightHeader(ref bandEdge, band, FixedRightBand, ViewRects.BandPanel);				
				int actualWidth = band == null ? width : band.VisibleWidth;
				Rectangle bounds = GetHeaderBounds(bandEdge, top, actualWidth, BandPanelRowHeight * rowCount);
				bandEdge += Direction * CalcBandInfo(bi, bounds, 0, true);
				CheckOffsetFixedLeftHeader(ref bandEdge, band, FixedLeftBand);
				CalcBandActualBounds(bi);
				if(bi.Bounds.Width > 0)
					BandsInfo.Add(bi);
			}
			UpdateExFixedBandInfo();
			UpdateHeaderPositions();
		}
		void UpdateExFixedBandInfo() {
			exFixedRightBandInfo = GetExFixedBandInfo(FixedRightBand, true);
			exFixedLeftBandInfo = GetExFixedBandInfo(FixedLeftBand);			
		}
		BandInfo GetExFixedBandInfo(TreeListBand fixedBand, bool isRight = false) {
			if(fixedBand == null) return null;
			BandInfo bi = BandsInfo.FindBand(fixedBand);
			return GetExFixedBandInfoCore(bi, isRight);
		}
		BandInfo GetExFixedBandInfoCore(BandInfo bi, bool isRight = false) {
			if(bi == null) return null;			
			if(bi.HasChildren) {
				if(isRight)
					return GetExFixedBandInfoCore(bi.Bands.FirstBandInfo, isRight);
				return GetExFixedBandInfoCore(bi.Bands.LastBandInfo, isRight);
			}
			return bi;
		}
		void CalcFixedBandActualBounds(BandInfo bi) {
			if(bi == null || bi.Band == null) return;
			bi.ActualBounds = GetFixedHeaderActualBounds(bi);
			if(bi.HasChildren)
				CalcFixedBandActualBounds(bi.Bands.LastBandInfo);
			else
				CalcMultiRowsActualBounds(bi);
		}
		protected virtual void CalcBandActualBounds(BandInfo bi) {
			if(bi == null || bi.Band == null) return;
			if(bi.Band == FixedRightBand)
				CalcFixedBandActualBounds(BandsInfo.LastBandInfo);
			if(bi.Band == FixedLeftBand)
				CalcFixedBandActualBounds(bi);
		}
		void CalcMultiRowsActualBounds(BandInfo bi) {
			if(bi == null || bi.Columns == null) return;
			foreach(ColumnInfo ci in bi.Columns.Columns) {
				if(IsFixedColumnMultiRow(ci, bi, ColumnInfo.ColumnPosition.Right))
					ci.ActualBounds = GetFixedHeaderActualBounds(ci);
			}
		}
		Rectangle GetFixedHeaderActualBounds(IHeaderObjectInfo info) {
			if(info.Bounds.Width <= 0) return Rectangle.Empty;
			return new Rectangle(info.Bounds.X - (IsRightToLeft ? 1 : 0), info.Bounds.Y, info.Bounds.Width + 1, info.Bounds.Height);
		}		
		protected internal virtual int CalcBandInfo(BandInfo bi, Rectangle bounds, int rowCount, bool isRoot) {			
			bi.SetAppearance(PaintAppearance.BandPanel);
			if(bi.Band != null) {
				bi.Pressed = (bi.Band == TreeList.PressedBand);
				bi.Fixed = bi.Band.RootBand.Fixed;
				AppearanceObject app = new AppearanceObject();
				AppearanceHelper.Combine(app, new AppearanceObject[] { bi.Band.AppearanceHeader, PaintAppearance.BandPanel });
				bi.SetAppearance(app);
			}   
			bi.IsTopMost = isRoot;
			bi.Bounds = bounds;
			if(bi.Type != ColumnInfo.ColumnInfoType.Column) {
				ColumnInfo ci = CreateColumnInfo(null);
				ci.Type = bi.Type;
				CalcColumnInfo(ci, new Rectangle(bounds.X, ViewRects.ColumnPanel.Top, bounds.Width, ColumnPanelHeight * ColumnPanelRowCount), false);
				ci.IsTopMost = false;
				RegisterColumnInfo(ci);
				return bi.Bounds.Width;
			}
			if(bi.Band == null) {
				ElementPainters.BandPainter.CalcObjectBounds(bi);
				return bi.Bounds.Width;
			}
			if(bounds.Width > 0) {
				if(bi.Bounds.Right < ViewRects.ActualBandPanel.Left ||
					bi.Bounds.Left > ViewRects.ActualBandPanel.Right) {
					bi.Bounds = new Rectangle(bounds.Location, new Size(0, bounds.Height));
					return bounds.Width;
				}
			}
			bool hasChildren = bi.Band.HasVisibleChildren;
			if(!bi.Band.HasVisibleChildren && bi.Band.AutoFill) {
				int rc = bi.Band.RowCount + rowCount;
				if(rc < BandPanelRowCount) {
					rc = BandPanelRowCount - rc;
					bounds.Height = BandPanelRowHeight * (bi.Band.RowCount + rc);
					bi.Bounds = bounds;
				}
			}
			ElementPainters.BandPainter.CalcObjectBounds(bi);
			if(hasChildren) {
				CalcChildrenBandInfo(bi, new Point(IsRightToLeft ? bi.Bounds.Right : bi.Bounds.Left, bi.Bounds.Bottom), rowCount + bi.Band.RowCount);
			}
			else {
				if(TreeList.AllowBandColumnsMultiRow)
					CalcBandsColumnInfoMultiRow(bi);
				else
					CalcBandColumnsInfo(bi);
			}
			return bi.Bounds.Width;
		}
		protected virtual void CalcChildrenBandInfo(BandInfo parent, Point leftTop, int rows) {
			int leftEdge = leftTop.X, top = leftTop.Y;
			for(int i = 0; i < parent.Band.Bands.Count; i++) {
				TreeListBand band = parent.Band.Bands[i];
				if(!band.Visible) continue;
				BandInfo bi = CreateBandInfo(band);
				Rectangle bounds = GetHeaderBounds(leftEdge, top, band.VisibleWidth, BandPanelRowHeight * band.RowCount);
				leftEdge += Direction * CalcBandInfo(bi, bounds, rows, false);
				if(bi.Bounds.Width > 0) parent.Bands.Add(bi);
			}
		}
		protected virtual int CalcRestBandsWidth(TreeListBand band) {
			if(band == null) return 0;
			int res = 0;
			TreeListBandCollection bands = TreeList.Bands;
			if(band.ParentBand != null) {
				bands = band.ParentBand.Bands;
				if(bands == null) return 0;
			}
			for(int n = 0; n < bands.Count; n++) {
				TreeListBand currentBand = bands[n];
				if(currentBand == band || band == null) {
					if(currentBand.Visible)
						res += currentBand.VisibleWidth;
					band = null;
				}
			}
			return res;
		}
		public virtual int GetBandPanelRowCount() {
			int res = 1;
			for(int n = 0; n < TreeList.Bands.Count; n++) {
				TreeListBand band = TreeList.Bands[n];
				if(!band.Visible) continue;
				res = Math.Max(res, GetBandPanelRowCountCore(band));
			}
			return res;
		}
		public virtual int GetColumnPanelRowCount() {
			if(!TreeList.AllowBandColumnsMultiRow) return 1;
			int res = 0;
			Dictionary<int, int> rows = new Dictionary<int, int>();
			for(int n = 0; n < TreeList.Columns.Count; n++) {
				TreeListColumn column = TreeList.Columns[n];
				if(!column.Visible || column.ParentBand == null || !column.ParentBand.ActualVisible) continue;
				int rowIndex = column.RowIndex;
				if(!rows.ContainsKey(rowIndex))
					rows[rowIndex] = column.RowCount;
				else
					rows[rowIndex] = Math.Max(rows[rowIndex], column.RowCount);
			}
			foreach(KeyValuePair<int, int> pair in rows)
				res += pair.Value;
			return Math.Max(1, res);
		}
		int GetBandPanelRowCountCore(TreeListBand band) {
			if(!band.Visible) return 0;
			int count = band.RowCount;
			int res = 0;
			for(int n = 0; n < band.Bands.Count; n++) {
				TreeListBand childBand = band.Bands[n];
				res = Math.Max(GetBandPanelRowCountCore(childBand), res);
			}
			return count + res;
		}
		protected virtual BandInfo CalcBandHitInfo(BandsInfo bands, Point pt, bool onlyFixed) {
			int bandDelta = TreeList.ActualShowBands ? 0 : -ViewRects.BandPanel.Height;
			foreach(BandInfo bi in bands) {
				if(onlyFixed && bi.Fixed == FixedStyle.None) continue;
				if(!bi.Bounds.IsEmpty && pt.X >= bi.Bounds.Left && pt.X < bi.Bounds.Right) {
					Rectangle bandBounds = bi.Bounds;
					bandBounds.Offset(0, bandDelta);
					if(bandBounds.Contains(pt)) return bi;
					if(bi.Band != null && bi.HasChildren && pt.Y > (bi.Bounds.Bottom + bandDelta)) {
						return CalcBandHitInfo(bi.Bands, pt, onlyFixed);
					}
					return bi;
				}
			}
			return null;
		}
		public virtual BandInfo CalcBandHitInfo(Point pt) {
			if(HasFixedLeft || HasFixedRight) {
				BandInfo bi = CalcBandHitInfo(BandsInfo, pt, true);
				if(bi != null) return bi;
			}
			return CalcBandHitInfo(BandsInfo, pt, false);
		}
		#endregion
		public virtual RowInfo GetNearestRow(Point pt) {
			RowInfo res = null;
			if(!IsValid) return res;
			int minDelta = 10000;
			foreach(RowInfo row in RowsInfo.Rows) {
				if(row.Bounds.Contains(pt)) return row;
				int delta = Math.Abs(pt.Y - row.Bounds.Top);
				if(delta < minDelta) {
					res = row;
					minDelta = delta;
				}
			}
			return res;
		}
		public TreeListColumn GetNearestColumn(Point pt) {
			if(!IsValid) return null;
			TreeListColumn res = null;
			int minDelta = 10000;
			res = GetNearestColumnCore(pt, ref minDelta, true);
			if(res != null) return res;
			return GetNearestColumnCore(pt, ref minDelta, false);
		}
		TreeListColumn GetNearestColumnCore(Point pt, ref int minDelta, bool onlyFixedRight) {
			TreeListColumn res = null;
			foreach(ColumnInfo info in ColumnsInfo.Columns) {
				if(info.Column == null) continue;
				if(onlyFixedRight && !IsFixedRightPaint(info)) continue;
				if(info.Bounds.Contains(pt)) return info.Column;
				int x = pt.X, left = info.Bounds.X, right = info.Bounds.Right;
				if(right < left) {
					int temp = left; left = right; right = temp;
				}
				if(x >= left && x < right) return info.Column;
				int delta = Math.Abs(pt.X - info.Bounds.X);
				if(delta < minDelta && !onlyFixedRight) {
					res = info.Column;
					minDelta = delta;
				}
			}
			return res;
		}
	}
	public class ResourceInfo : IDisposable {
		public bool NeedsRestore;
		Brush treeLineBrush;
		public Size StateImageSize, SelectImageSize, ColumnImageSize, PlusMinusButtonSize, CheckBoxSize;
		int minRowHeight, rowHeight, previewRowHeight, columnPanelHeight, bandPanelRowHeight, indicatorWidth,
			footerPanelHeight, groupFooterHeight, groupFooterCellHeight, levelWidth;
		internal int hlw, vlw; 
		TreeListViewInfo viewInfo;
		public ResourceInfo(TreeListViewInfo viewInfo) {
			this.viewInfo = viewInfo;
			NeedsRestore = true;
			treeLineBrush = null;
			StateImageSize = Size.Empty;
			SelectImageSize = Size.Empty;
			ColumnImageSize = Size.Empty;
			PlusMinusButtonSize = Size.Empty;
			CheckBoxSize = Size.Empty;
			minRowHeight = rowHeight = previewRowHeight = columnPanelHeight = bandPanelRowHeight =
				footerPanelHeight = groupFooterHeight =  
				groupFooterCellHeight = indicatorWidth = levelWidth = 0;
			hlw = vlw = 0;
		}
		public virtual void Dispose() {
			DestroyLineBrush();
		}
		internal AppearanceObject CreateAppearance(AppearanceObject[] combine) {
			AppearanceObject app = new AppearanceObject();
			AppearanceHelper.Combine(app, combine);
			return app;
		}
		public virtual void CreateLineBrush(AppearanceObject vs, LineStyle style) {
			DestroyLineBrush();
			this.treeLineBrush = GetTreeLineBrush(vs, style);
		}
		protected virtual bool ShowTreeLine {
			get {
				if(viewInfo.TreeList.ElementsLookAndFeel.ActiveStyle == ActiveLookAndFeelStyle.Skin) {
					Skin skin = GridSkins.GetSkin(viewInfo.TreeList.ElementsLookAndFeel);
					object value = skin.Properties[GridSkins.OptShowTreeLine];
					if(value != null)
						return (bool)value;
				}
				return true;
			}
		}
		protected internal virtual Brush GetTreeLineBrush(AppearanceObject foreAppearance, LineStyle lineStyle) {
			if(!ShowTreeLine) return null;
			if(lineStyle == LineStyle.Solid) return new SolidBrush(foreAppearance.BackColor);
			if(lineStyle == LineStyle.None) return new SolidBrush(Color.Empty);
			return new HatchBrush((HatchStyle)lineStyle, foreAppearance.BackColor, Color.Empty);
		}
		protected virtual void DestroyLineBrush() {
			if(TreeLineBrush == null) return;
			this.treeLineBrush.Dispose();
			this.treeLineBrush = null;
		}
		protected internal void SetMinRowHeight(int value) { this.minRowHeight = value; }
		protected internal void SetRowHeight(int value) { this.rowHeight = value; }
		protected internal void SetPreviewRowHeight(int value) { this.previewRowHeight = value; }
		protected internal void SetColumnPanelHeight(int value) { this.columnPanelHeight = value; }
		protected internal void SetBandPanelRowHeight(int value) { this.bandPanelRowHeight = value; }
		protected internal void SetIndicatorWidth(int value) { this.indicatorWidth = value; }
		protected internal void SetFooterPanelHeight(int value) { this.footerPanelHeight = value; }
		protected internal void SetGroupFooterHeight(int value) { this.groupFooterHeight = value; }
		protected internal void SetGroupFooterCellHeight(int value) { this.groupFooterCellHeight = value; }
		protected internal void SetLevelWidth(int value) { this.levelWidth = value; }
		public int MinRowHeight { get { return minRowHeight; } }
		public int  RowHeight { get { return rowHeight; } }
		public int  PreviewRowHeight { get { return previewRowHeight; } }
		public int ColumnPanelHeight { get { return columnPanelHeight; } }
		public int BandPanelRowHeight { get { return bandPanelRowHeight; } }
		public int  IndicatorWidth { get { return indicatorWidth; } }
		public int FooterPanelHeight { get { return footerPanelHeight; } }
		public int GroupFooterHeight { get { return groupFooterHeight; } }
		public int GroupFooterCellHeight { get { return groupFooterCellHeight; } }
		public int LevelWidth { get { return levelWidth; } }
		public Brush TreeLineBrush { get { return treeLineBrush; } }
	}
	public class ViewRects {
		TreeListViewInfo viewInfo;
		public Rectangle ClipRectangle;
		public Rectangle Window;
		public Rectangle Client;
		public Rectangle ScrollArea;
		public Rectangle ColumnPanel;
		public Rectangle BandPanel;
		public Rectangle ActualBandPanel { get { return CalcActualBounds(BandPanel); } }
		public Rectangle ActualColumnPanel { get { return CalcActualBounds(ColumnPanel); } }
		public Rectangle Rows;
		public Rectangle TotalRows;
		public Rectangle AutoFilterRow;
		public Rectangle Footer;
		public Rectangle FixedLeft;
		public Rectangle FixedRight;
		public Rectangle TopRowSeparator;
		public Rectangle FindPanel;
		public Rectangle Caption;
		public Rectangle EmptyRows, EmptyBehindColumn, BeyondScrollSquare;
		public int ColumnTotalWidth;
		public int BandTotalWidth;
		public int IndicatorWidth;
		public int RowsTotalHeight;
		public int MaxIndents;
		public Size SortShapeSize;
		public ViewRects(TreeListViewInfo viewInfo) {
			this.viewInfo = viewInfo;
			SortShapeSize = new Size(14, 11);
			MaxIndents = 0;
			Clear();
		}
		public virtual void Clear() {
			Client = Rectangle.Empty;
			ClipRectangle = Window = EmptyRows = ColumnPanel = BandPanel = Rows = Footer =
				EmptyBehindColumn = BeyondScrollSquare = FixedLeft = FixedRight = ScrollArea = AutoFilterRow = TopRowSeparator = Caption = Client;
			ColumnTotalWidth = 0;
			IndicatorWidth = 0;
			RowsTotalHeight = 0;
		}
		public bool IsColumnRectVisible(Rectangle bounds) {
			Rectangle columnsPanel = new Rectangle(ColumnPanel.Left + IndicatorWidth, ColumnPanel.Top, ColumnPanel.Width - IndicatorWidth, ColumnPanel.Height);
			return columnsPanel.Contains(bounds);
		}
		public Rectangle EmptyUnionArea {
			get {
				if(!IsEmptyVisible) return Rectangle.Empty;
				Rectangle result = Rows;
				if(EmptyRows.Height != 0) result = Rectangle.Union(result, EmptyRows);
				if(EmptyBehindColumn.Width != 0) result = Rectangle.Union(result, EmptyBehindColumn);
				return result;
			}
		}
		public int ColumnPanelWidth {
			get {
				if(ColumnPanel.Width > 0) return ColumnPanel.Width - IndicatorWidth;
				int res = 0;
				foreach(TreeListColumn col in TreeList.VisibleColumns) {
					res += col.VisibleWidth;
				}
				return res - IndicatorWidth;
			}
		}
		public int BandPanelWidth {
			get {
				if(BandPanel.Width > 0) return BandPanel.Width - IndicatorWidth;
				int res = 0;
				foreach(TreeListBand band in TreeList.Bands) {
					if(band.Visible) res += band.VisibleWidth;
				}
				return res;
			}
		}
		Rectangle CalcActualBounds(Rectangle bounds) {
			Rectangle res = bounds;
			if(viewInfo.IsRightToLeft)
				res.X = ScrollArea.X;
			res.Width = ScrollArea.Right - res.X;
			return res;
		}
		bool IsEmptyVisible {
			get {
				return (EmptyRows.Height != 0 || EmptyBehindColumn.Width != 0 ||
			  (!TreeList.OptionsView.ShowIndentAsRowStyle && (TreeList.StateImageList != null || TreeList.SelectImageList != null || TreeList.OptionsView.ShowCheckBoxes)));
			}
		}
		protected TreeListViewInfo ViewInfo { get { return viewInfo; } }
		protected TreeList TreeList { get { return ViewInfo.TreeList; } }
		public Rectangle IndicatorBounds {
			get {
				return new Rectangle(Rows.Location, new Size(IndicatorWidth, Rows.Height));
			}
		}
	}	
	public class BandsInfo : CollectionBase {
		public virtual void Add(BandInfo ci) {
			List.Add(ci);
		}
		public BandInfo this[TreeListBand band] {
			get {
				if(band == null) return null;
				for(int n = 0; n < Count; n++) {
					BandInfo bi = this[n];
					if(bi.Band == band) return bi;
				}
				return null;
			}
		}
		public BandInfo this[int index] { get { return List[index] as BandInfo; } }	   
		public BandInfo FindBand(TreeListBand band) {
			if(band == null) return null;
			BandInfo res = this[band];
			if(res != null) return res;
			foreach(BandInfo info in this) {
				if(info.HasChildren) {
					res = info.Bands.FindBand(band);
					if(res != null) return res;
				}
			}
			return null;
		}
		public BandInfo LastBandInfo {
			get {				
				for(int n = Count - 1; n > -1; n--) {
					BandInfo bi = this[n];
					if(bi.Type == ColumnInfo.ColumnInfoType.Column)
						return bi;
				}
				return null;
			}
		}
		public BandInfo FirstBandInfo {
			get {
				for(int n = 0; n < Count; n++) {
					BandInfo bi = this[n];
					if(bi.Type == ColumnInfo.ColumnInfoType.Column)
						return bi;
				}
				return null;
			}
		}
	}
	public class ColumnsInfo {
		ArrayList columns;
		ArrayList cellWidthes;
		public ColumnsInfo() {
			this.columns = new ArrayList();
			this.cellWidthes = new ArrayList();
		}
		public int ColumnsCount {
			get {
				int count = 0;
				foreach(ColumnInfo ci in Columns) {
					if(ci.Type == ColumnInfo.ColumnInfoType.Column)
						count++;
				}
				return count;
			}
		}
		public ColumnInfo this[TreeListColumn col] {
			get {
				if(col == null) return null;
				foreach(ColumnInfo ci in Columns) {
					if(ci.Column == col) return ci;
				}
				return null;
			}
		}
		public ColumnInfo this[int index] {
			get {
				if(index < 0 || index >= Columns.Count) return null;
				return (ColumnInfo)Columns[index]; 
			}
		}
		public int IndexOf(ColumnInfo ci) { return Columns.IndexOf(ci); }
		public ColumnInfo LastColumnInfo {
			get {
				for(int n = Columns.Count - 1; n > -1; n--) {
					ColumnInfo ci = (ColumnInfo)Columns[n];
					if(ci.Type == ColumnInfo.ColumnInfoType.Column)
						return ci;
				}
				return null;
			}
		}
		public ColumnInfo FirstColumnInfo {
			get {
				for(int n = 0; n < Columns.Count; n++) {
					ColumnInfo ci = (ColumnInfo)Columns[n];
					if(ci.Type == ColumnInfo.ColumnInfoType.Column)
						return ci;
				}
				return null;
			}
		}
		public ArrayList Columns { get { return columns; } }
		public ArrayList CellWidthes { get { return cellWidthes; } }
	}
	public interface IHeaderObjectInfo {		
		Rectangle ActualBounds { get; set; }
		Rectangle Bounds { get; }
		ColumnInfo.ColumnInfoType Type { get; set; }	 
	}
	public class ColumnInfo : HeaderObjectInfoArgs, IHeaderObjectInfo {
		public enum ColumnInfoType { Column, ColumnButton, BehindColumn, EmptyColumn }
		public enum ColumnPosition { Left, Center, Right, Single }
		TreeListColumn column;
		ColumnInfoType type;
		HorzAlignment defaultCellTextAlignment;
		bool customizationForm;
		int cellIndex = -1;
		public ColumnInfo(TreeListColumn column) {
			this.column = column;
			this.type = ColumnInfoType.Column;
			this.defaultCellTextAlignment = HorzAlignment.Near;
			RowCount = 1;
			StartRow = 0;
			Position = ColumnPosition.Center;
			UpdateCaption();
		}
		protected internal void UpdateCaption() {
			UpdateHtmlDrawInfo();
			if(Column == null) {
				Caption = string.Empty;
				return;
			}
			if(CustomizationForm)
				Caption = Column.GetCustomizationCaption();
			else
				Caption = Column.GetCaption();
		}
		public bool CustomizationForm {
			get { return customizationForm; }
			set {
				customizationForm = value;
				UpdateCaption();
			}
		}
		public bool Pressed {
			get { return IsSetState(ObjectState.Pressed); }
			set {
				if(Pressed == value) return;
				SetState(ObjectState.Pressed, value);
			}
		}
		public bool MouseOver {
			get { return IsSetState(ObjectState.Hot); }
			set {
				if(MouseOver == value) return;
				SetState(ObjectState.Hot, value);
			}
		}
		private bool IsSetState(ObjectState objectState) { return (State & objectState) != 0; }
		private void SetState(ObjectState objectState, bool check) {
			if(check) State |= objectState;
			else State &= ~objectState;
		}
		public TreeListColumn Column { get { return column; } }
		public TreeList TreeList { get { return Column.TreeList; } }
		public Rectangle SortShapeRect {
			get {
				if(InnerElements.Count == 0) return Rectangle.Empty;
				foreach(DrawElementInfo info in InnerElements) {
					SortedShapeObjectInfoArgs args = info.ElementInfo as SortedShapeObjectInfoArgs;
					if(args != null)
						return args.Bounds; 
				}
				return Rectangle.Empty;
			}
		}
		public string GetTextCaption() {
			if(Column == null) return string.Empty;
			return Column.GetTextCaption();
		}
		void UpdateHtmlDrawInfo() {
			if(Column == null || TreeList == null) return;
			UseHtmlTextDraw = TreeList.OptionsView.AllowHtmlDrawHeaders;
			if(UseHtmlTextDraw)
				HtmlContext = TreeList;
		}
		protected internal bool SetFilterButtonState(ObjectState state) {
			if(FilterButtonInfo == null) return false;
			ObjectState oldState = FilterButtonInfo.State;
			FilterButtonInfo.State = state;
			return oldState != state; 
		}
		protected internal bool SetFilterButtonVisible(bool value) {
			if(FilterButton == null) return false;
			bool oldVisible = FilterButton.Visible;
			FilterButton.Visible = value;
			bool changed = oldVisible != FilterButton.Visible;
			if(changed && TreeList != null)
				TreeList.ElementsLookAndFeel.Painter.Header.CalcObjectBounds(this);
			return changed;
		}
		protected TreeListFilterButtonInfoArgs FilterButtonInfo {
			get { return FilterButton == null ? null : (TreeListFilterButtonInfoArgs)FilterButton.ElementInfo; }
		}
		protected DrawElementInfo FilterButton {
			get {
				if(InnerElements.Count == 0) return null;
				return InnerElements.Find(typeof(TreeListFilterButtonInfoArgs));
			}
		}
		internal void SetColumn(TreeListColumn column) {
			this.column = column;
		}
		protected internal int StartRow { get; set; }
		protected internal int RowCount { get; set; }
		public ColumnInfoType Type { get { return type; } set { type = value; } }
		public HorzAlignment DefaultCellTextAlignment { get { return defaultCellTextAlignment; } set { defaultCellTextAlignment = value; } }
		public Rectangle ActualBounds { get; set; }
		public int CellIndex { get { return cellIndex; } set { cellIndex = value; } }
		public bool IsFirst {  get { return CellIndex == 0; } }
		public BandInfo ParentBandInfo { get; set; }
		public ColumnPosition Position { get; set; }
	}
	public class BandInfo : HeaderObjectInfoArgs, IHeaderObjectInfo {
		bool customizationForm;
		public BandInfo(TreeListBand band) {
			Band = band;
			Columns = new ColumnsInfo();
			Bands = new BandsInfo();
			Type = ColumnInfo.ColumnInfoType.Column;
			Fixed = FixedStyle.None;
			UpdateCaption();
		}
		public TreeListBand Band { get; private set; }
		public ColumnInfo.ColumnInfoType Type { get; set; }
		public ColumnsInfo Columns { get; private set; }
		public BandsInfo Bands { get; private set; }
		public FixedStyle Fixed { get; set; }
		public bool CustomizationForm {
			get { return customizationForm; }
			set {
				customizationForm = value;
				UpdateCaption();
			}
		}
		protected TreeList TreeList { get { return Band.TreeList; } }
		protected void UpdateCaption() {
			UpdateHtmlDrawInfo();
			if(Band == null) {
				Caption = string.Empty;
				return;
			}
			if(CustomizationForm)
				Caption = Band.GetCustomizationCaption();
			else
				Caption = Band.Caption;
		}
		void UpdateHtmlDrawInfo() {
			if(Band == null || TreeList == null) return;
			UseHtmlTextDraw = TreeList.OptionsView.AllowHtmlDrawHeaders;
			if(UseHtmlTextDraw)
				HtmlContext = TreeList;
		}
		public bool MouseOver {
			get { return IsSetState(ObjectState.Hot); }
			set {
				if(MouseOver == value) return;
				SetState(ObjectState.Hot, value);
			}
		}
		public bool Pressed {
			get { return IsSetState(ObjectState.Pressed); }
			set {
				if(Pressed == value) return;
				SetState(ObjectState.Pressed, value);
			}
		}
		bool IsSetState(ObjectState objectState) { return (State & objectState) != 0; }
		void SetState(ObjectState objectState, bool check) {
			if(check) State |= objectState;
			else State &= ~objectState;
		}
		public bool HasChildren { get { return Bands.Count > 0; } }
		public Rectangle ActualBounds { get; set; }
	}
	public class RowsInfo {
		int increaseVisibleRows;
		ArrayList rows;
		ArrayList rowFooters;
		public RowsInfo() {
			this.rows = new ArrayList();
			this.rowFooters = new ArrayList();
			this.increaseVisibleRows = 0;
		}
		public int SelectedCount {
			get {
				int count = 0;
				for(int i = 0; i < Rows.Count; i++) {
					RowInfo ri = (RowInfo)Rows[i];
					if(ri.Selected) count++;
				}
				return count;
			}
		}
		public RowInfo this[TreeListNode node] {
			get {
				if(node == null) return null;
				foreach(RowInfo ri in Rows) {
					if(ri.Node == node) return ri;
				}
				return null;
			}
		}
		public RowInfo this[int index] {
			get {
				if(index < 0 || index > Rows.Count - 1) return null;
				return Rows[index] as RowInfo;
			}
		}
		public void Clear() {
			Rows.Clear();
			RowFooters.Clear();
		}
		public int GetInvisibleRowFooterLinesCount(RowInfo rowAbove, int bottomY) {
			int count = 0;
			foreach(RowFooterInfo fi in RowFooters) {
				if(fi.RowAbove == rowAbove && fi.Bounds.Bottom > bottomY)
					count++;
			}
			return count;
		}
		public ArrayList Rows { get { return rows; } }
		public ArrayList RowFooters { get { return rowFooters; } }
		public int IncreaseVisibleRows { get { return increaseVisibleRows; } set { increaseVisibleRows = value; } }
	}
	public class RowInfo : StyleObjectInfoArgs {
		public Rectangle ButtonBounds,
			DataBounds,
			StateImageBounds,
			SelectImageBounds,
			PreviewBounds,
			CheckBounds;
		public Point StateImageLocation, SelectImageLocation;
		TreeListNode node;
		TreeNodeCellState rowState;
		ArrayList cells, lines, floatLines;
		bool fillImagesBackground, expanded, indentAsRowStyle, topMostIndicator;
		int level, indent, previewIndent; 
		int visibleIndex, stateImageIndex, selectImageIndex;
		IndentInfo indentInfo;
		RowFooterInfo lastFooter;
		RowIndicatorInfo indicatorInfo;
		int nodeHeight;
		string previewText, errorText;
		Image errorIcon;
		TreeListViewInfo viewInfo;
		ConditionInfo conditionInfo;
		FormatRuleInfo formatRuleInfo;
		public RowInfo(TreeListViewInfo viewInfo, TreeListNode node) {
			this.viewInfo = viewInfo;
			this.conditionInfo = new ConditionInfo(viewInfo);
			this.formatRuleInfo = new FormatRuleInfo(viewInfo);
			this.node = node;
			SetAppearance(new AppearanceObject());
			this.rowState = TreeNodeCellState.Dirty;
			this.cells = new ArrayList();
			this.lines = new ArrayList();
			this.floatLines = new ArrayList();
			this.indentInfo = new IndentInfo();
			this.fillImagesBackground = this.expanded = this.indentAsRowStyle = 
				this.topMostIndicator = false;
			this.lastFooter = null;
			this.level = this.previewIndent = this.indent = 0;
			this.visibleIndex = this.stateImageIndex = this.selectImageIndex = -1;
			this.nodeHeight = -1;
			this.previewText = this.errorText = string.Empty;
			this.indicatorInfo = new RowIndicatorInfo();
			PreviewBounds = DataBounds = ButtonBounds = 
				StateImageBounds = SelectImageBounds = CheckBounds = Rectangle.Empty;
			StateImageLocation = SelectImageLocation = Point.Empty;
		}
		public bool IsSamePaintBackground(AppearanceObject value) {
			return TreeListViewInfo.IsSamePaintBackground(PaintAppearance, value) && (PaintAppearance.GetImage() == value.GetImage());
		}
		public Rectangle GetFillBackgroundBounds(bool canFillWithIndent) {
			Rectangle result = DataBounds;
			if(IndentAsRowStyle && canFillWithIndent) {
				int cx = DataBounds.Left - Bounds.Left;
				result.Width += cx;
				result.X -= cx;
			}
			result.Height = Bounds.Height;
			return result;
		}
		protected internal ConditionInfo ConditionInfo { get { return conditionInfo; } }
		protected internal FormatRuleInfo FormatRuleInfo { get { return formatRuleInfo; } }
		public AppearanceObject AppearancePreview { get { return viewInfo.PaintAppearance.Preview; } }
		public AppearanceObject AppearanceButton { get { return viewInfo.PaintAppearance.GroupButton; } }
		public AppearanceObject PaintAppearance { get { return Appearance; } }
		public void SetDirty() {
			this.rowState = TreeNodeCellState.Dirty;
			foreach(CellInfo cell in Cells) {
				cell.SetDirty();
			}
		}
		public bool FillImagesBackground { get { return fillImagesBackground; } set { fillImagesBackground = value; } }
		public bool Expanded { get { return expanded; } set { expanded = value; } }
		public bool IndentAsRowStyle { get { return indentAsRowStyle; } set { indentAsRowStyle = value; } }
		public bool TopMostIndicator { get { return topMostIndicator; } set { topMostIndicator = value; } }
		public int Level { get { return level; } set { level = value; } }
		public int Indent { get { return indent; } set { indent = value; } }
		public int PreviewIndent { get { return previewIndent; } set { previewIndent = value; } }
		public int VisibleIndex { get { return visibleIndex; } set { visibleIndex = value; } }
		public int StateImageIndex { get { return stateImageIndex; } set { stateImageIndex = value; } }
		public int SelectImageIndex { get { return selectImageIndex; } set { selectImageIndex = value; } }
		public int NodeHeight { get { return nodeHeight; } set { nodeHeight = value; } }
		public string PreviewText { get { return previewText; } set { previewText = value; } }
		public string ErrorText { get { return errorText; } set { errorText = value; } }
		internal Image ErrorIcon { get { return errorIcon; } set { errorIcon = value; } }
		public TreeNodeCellState RowState { get { return rowState; } set { rowState = value; } }
		public RowFooterInfo LastFooter { get { return lastFooter; } set { lastFooter = value; } }
		public RowIndicatorInfo IndicatorInfo { get { return indicatorInfo; } }
		public ArrayList Cells { get  { return cells; } }
		public ArrayList Lines { get  { return lines; } }
		public ArrayList FloatLines { get  { return floatLines; } }
		public IndentInfo IndentInfo { get { return indentInfo; } }
		public TreeListNode Node { get { return node; } }
		public bool Selected { get { return (RowState & TreeNodeCellState.Selected) != 0; } }
		public CellInfo this[TreeListColumn col] {
			get {
				foreach(CellInfo cell in Cells) {
					if(cell.Column == col)
						return cell;
				}
				return null;
			}
		}
		public void AddLineInfo(int x, int y, int width, int height, AppearanceObject appearance) {
			Lines.Add(new LineInfo(x, y, width, height, appearance));
		}
		public void AddLineInfo(Rectangle r, AppearanceObject appearance) {
			Lines.Add(new LineInfo(r, appearance));
		}
		public void SetHeight(int h) {
			Bounds = new Rectangle(Bounds.Location, new Size(Bounds.Width, h));
		}
		public Rectangle GetExcludeEmptyRect() {
			if(Bounds == IndicatorInfo.Bounds) return Bounds;
			return TreeListViewInfo.GetExcludeEmptyRect(Bounds, DataBounds, IndentAsRowStyle);
		}
		public AppearanceObjectEx GetCellConditionAppearance(TreeListColumn column) {
			AppearanceObjectEx appearance = FormatRuleInfo.GetCellAppearance(column);
			if(appearance != null) 
				return appearance;
			return ConditionInfo.GetCellAppearance(column);
		}
	}
	public class EmptyCellInfo : CellInfo {
		Rectangle bounds;
		public EmptyCellInfo(ColumnInfo columnInfo, RowInfo rowInfo)
			: base(columnInfo, rowInfo) {
		}
		public override object Value { get { return null; } }
		public override Rectangle Bounds {
			get { return Rectangle.Inflate(CellValueRect, CellTextIndent, CellTextIndent); }
		}
		public override Rectangle CellValueRect {
			get { return bounds; }
		}
		public void SetBounds(Rectangle rect) {
			this.bounds = rect;
		}
		public static bool IsEmptyCell(CellInfo cell) {
			return cell is EmptyCellInfo;
		}
	}
	public class CellInfo {
		ColumnInfo columnInfo;
		RowInfo rowInfo;
		TreeNodeCellState state = TreeNodeCellState.Dirty;
		AppearanceObject paintAppearance;
		DevExpress.XtraEditors.ViewInfo.BaseEditViewInfo editorViewInfo;
		CellInfo(ColumnInfo columnInfo, RowInfo rowInfo) {
			this.columnInfo = columnInfo;
			this.rowInfo = rowInfo;
			this.paintAppearance = new AppearanceObject();
		}
		public CellInfo(ColumnInfo columnInfo, RowInfo rowInfo, BaseEditViewInfo editorViewInfo = null)
			: this(columnInfo, rowInfo) {
			this.editorViewInfo = editorViewInfo;
			if(this.editorViewInfo == null)
				CreateViewInfo();
		}
		public AppearanceObject PaintAppearance { get { return paintAppearance; } set { paintAppearance = value; } }
		public bool Focused { get { return (State & TreeNodeCellState.FocusedCell) != 0; } }
		public void SetDirty() {
			State = TreeNodeCellState.Dirty;
		}
		public TreeNodeCellState State {
			get { return state; }
			set { state = value; }
		}
		public virtual object Value {
			get {
				if(RowInfo != null && Column != null)
					return RowInfo.Node[Column];
				if(EditorViewInfo != null)
					return EditorViewInfo.EditValue;
				return null;
			}
		}
		public virtual void CreateViewInfo() { this.editorViewInfo = GetEditorViewInfoCore(Column, RowInfo.Node); }
		public static DevExpress.XtraEditors.ViewInfo.BaseEditViewInfo GetEditorViewInfo(TreeListColumn column, TreeListNode node) {
			if(column == null) return null;
			DevExpress.XtraEditors.ViewInfo.BaseEditViewInfo tempEditorViewInfo = GetEditorViewInfoCore(column, node);
			if(tempEditorViewInfo == null) return null;
			if(!column.Format.IsEmpty)
				tempEditorViewInfo.Format = column.Format;
			tempEditorViewInfo.EditValue = column.TreeList.InternalGetNodeValue(node, column);
			return tempEditorViewInfo;
		}
		static DevExpress.XtraEditors.ViewInfo.BaseEditViewInfo GetEditorViewInfoCore(TreeListColumn column, TreeListNode node) {
			if(column == null) return null;
			DevExpress.XtraEditors.Repository.RepositoryItem item = column.TreeList.InternalGetCustomNodeCellEdit(column, node);
			DevExpress.XtraEditors.ViewInfo.BaseEditViewInfo tempEditorViewInfo = item.CreateViewInfo();
			tempEditorViewInfo.InplaceType = InplaceType.Grid;
			tempEditorViewInfo.RightToLeft = column.TreeList.IsRightToLeft;
			return tempEditorViewInfo;
		}
		public void CalcViewInfo(Graphics g) { CalcViewInfo(g, Point.Empty, CellValueRect); }
		public void CalcViewInfo(Graphics g, Point pt) { CalcViewInfo(g, pt, CellValueRect); }
		public virtual void CalcViewInfo(Graphics g, Point pt, Rectangle bounds) {
			if(EditorViewInfo == null) return;
			EditorViewInfo.PaintAppearance = PaintAppearance;
			EditorViewInfo.CalcViewInfo(g, MouseButtons.None, pt, bounds);
		}
		public void CheckHAlignment() {
			if(PaintAppearance.HAlignment == HorzAlignment.Default && Column != null)
				PaintAppearance.TextOptions.HAlignment = Column.DefaultCellTextAlignment;
		}
		public virtual Rectangle Bounds { get { return Rectangle.Inflate(EditorViewInfo.Bounds, CellTextIndent, CellTextIndent); } }
		public virtual Rectangle CellValueRect { get { return EditorViewInfo.Bounds; } }
		public virtual XtraEditors.Repository.RepositoryItem Item { get { return EditorViewInfo != null ? EditorViewInfo.Item : null; } }
		public virtual DevExpress.XtraEditors.ViewInfo.BaseEditViewInfo EditorViewInfo { get { return editorViewInfo; } }
		public ColumnInfo ColumnInfo { get { return columnInfo; } internal set { columnInfo = value; } }
		public TreeListColumn Column { get { return ColumnInfo == null ? null : ColumnInfo.Column; } }
		public RowInfo RowInfo { get { return rowInfo; } }
		static public int CellTextIndent { get { return 1; } }
	}
	public class IndicatorInfo {
		AppearanceObject appearance;
		Rectangle bounds;
		public IndicatorInfo() {
			this.appearance = new AppearanceObject();
			this.bounds = Rectangle.Empty;
		}
		public AppearanceObject Appearance { get { return appearance; } }
		public Rectangle Bounds { get { return bounds; } set { bounds = value; } }
	}
	public class RowIndicatorInfo : IndicatorInfo {
		int imageIndex;
		public RowIndicatorInfo() {
			this.imageIndex = -1;
		}
		public int ImageIndex { get { return imageIndex; } set { imageIndex = value; } }
	}
	public class SummaryFooterInfo : FooterPanelInfoArgs {
		Rectangle clientBounds;
		bool needsRecalcAll, needsRecalcRects;
		ArrayList footerItems;
		public SummaryFooterInfo() : base(null, 1, 0) {
			this.clientBounds = Rectangle.Empty;
			this.footerItems = new ArrayList();
			this.needsRecalcAll = this.needsRecalcRects = true;
		}
		public FooterItem this[TreeListColumn column] {
			get {
				foreach(FooterItem fi in FooterItems) {
					if(fi.Column == column)
						return fi;
				}
				return null; 
			}
		}
		public bool NeedsRecalcAll { get { return needsRecalcAll; } set { needsRecalcAll = value; } }
		public bool NeedsRecalcRects { get { return needsRecalcRects; } set { needsRecalcRects = value; } }
		public ArrayList FooterItems { get { return footerItems; } }
		public Rectangle ClientBounds { get { return clientBounds; } set { clientBounds = value; } }
	}
	public class RowFooterInfo : SummaryFooterInfo {
		IndentInfo indentInfo;
		Rectangle footerBounds;
		TreeListNode node;
		RowInfo rowAbove;
		ArrayList floatLines;
		int level;
		IndicatorInfo indicatorInfo;
		public RowFooterInfo(TreeListNode node, RowInfo rowAbove) {
			this.indentInfo = new IndentInfo();
			this.footerBounds = Rectangle.Empty;
			this.node = node;
			this.rowAbove = rowAbove;
			this.level = 0;
			this.floatLines = new ArrayList();
			this.indicatorInfo = new IndicatorInfo();
		}
		public Rectangle GetExcludeEmptyRect(bool indentAsRowStyle) {
			return TreeListViewInfo.GetExcludeEmptyRect(Bounds, ClientBounds, indentAsRowStyle);
		}
		public bool CanFillIndent { get { return RowAbove.Node.TreeList.OptionsView.ShowIndentAsRowStyle; } }
		public IndentInfo IndentInfo { get { return indentInfo; } }
		public Rectangle FooterBounds { get { return footerBounds; } set { footerBounds = value; } }
		public TreeListNode Node { get { return node; } }
		public RowInfo RowAbove { get { return rowAbove; } }
		public int Level { get { return level; } set { level = value; } }
		public ArrayList FloatLines { get { return floatLines; } }
		public IndicatorInfo IndicatorInfo { get { return indicatorInfo; } }
		public Rectangle IndentBounds {
			get {
				Rectangle result = IndentInfo.Bounds;
				if(RowAbove.Node.TreeList.OptionsView.ShowHorzLines)
					result.Height++;
				return result;
			}
		}
	}
	public class IndentInfo : StyleObjectInfoArgs {
		public Brush TreeLineBrush;
		public ArrayList IndentItems;
		int levelWidth;
		public IndentInfo() {
			this.levelWidth = 0;
			TreeLineBrush = null;
			IndentItems = new ArrayList();
		}
		public int LevelWidth { get { return levelWidth; } set { levelWidth = value; } }
		internal int ActualRowHeight;
	}
	public class TreeListCellToolTipInfo {
		TreeListNode node;
		TreeListColumn column;
		object cellObject;
		public TreeListCellToolTipInfo(TreeListNode node, TreeListColumn column, object cellObject) {
			this.node = node;
			this.column = column;
			this.cellObject = cellObject;
		}
		public TreeListNode Node { get { return node; } }
		public TreeListColumn Column { get { return column; } }
		public object CellObject { get { return cellObject; } }
		public override bool Equals(object obj) {
			TreeListCellToolTipInfo info = obj as TreeListCellToolTipInfo;
			if(info == null) return false;
			return info.Node == this.Node && info.Column == this.Column &&
				Object.Equals(info.CellObject, this.CellObject);
		}
		public override int GetHashCode() {
			return string.Format("{0},{1},{2}", Node == null ? 0 : Node.GetHashCode(), this.Column == null ? 0 : this.Column.GetHashCode(), this.CellObject == null ? 0 : this.CellObject.GetHashCode()).GetHashCode();
		}
	}
	class TreeListEditorMinHeightCalculator {
		TreeList treeList;
		public TreeListEditorMinHeightCalculator(TreeList treeList) {
			this.treeList = treeList;
		}
		protected virtual ArrayList GetAvailableEditViewInfos() {
			Hashtable hash = new Hashtable();
			treeList.ContainerHelper.DefaultRepository.GetRepositoryItem(typeof(string));
			treeList.ContainerHelper.DefaultRepository.GetRepositoryItem(typeof(bool));
			treeList.ContainerHelper.DefaultRepository.GetRepositoryItem(typeof(DateTime));
			FillRepositoryHashtable(treeList.ContainerHelper.DefaultRepository.Items, hash);
			FillRepositoryHashtable(treeList.ContainerHelper.InternalRepository.Items, hash);
			if(treeList.ExternalRepository != null)
				FillRepositoryHashtable(treeList.ExternalRepository.Items, hash);
			return new ArrayList(hash.Values);
		}
		void FillRepositoryHashtable(RepositoryItemCollection collection, Hashtable hash) {
			if(collection == null) return;
			foreach(RepositoryItem item in collection) {
				if(hash.Contains(item)) continue;
				hash.Add(item, CreateEditViewInfo(item));
			}
		}
		BaseEditViewInfo CreateEditViewInfo(RepositoryItem item) {
			DevExpress.XtraEditors.ViewInfo.BaseEditViewInfo viewInfo = item.CreateViewInfo();
			UpdateEditViewInfo(viewInfo);
			return viewInfo;
		}
		protected virtual void UpdateEditViewInfo(DevExpress.XtraEditors.ViewInfo.BaseEditViewInfo vi) {
			vi.DefaultBorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			vi.AllowDrawFocusRect = false;
			vi.FillBackground = vi.Item.AllowFocusedAppearance ? false : true;
			vi.DetailLevel = DevExpress.XtraEditors.Controls.DetailLevel.Minimum;
			vi.InplaceType = InplaceType.Grid;
			vi.RightToLeft = treeList.IsRightToLeft;
			if(vi.LookAndFeel != treeList.ElementsLookAndFeel && treeList.LookAndFeel.UseDefaultLookAndFeel) 
				vi.LookAndFeel = treeList.ElementsLookAndFeel;
		}
		public virtual int CalcMinEditorHeight(GraphicsInfo gInfo, AppearanceObject[] styles) {
			Graphics g = gInfo.AddGraphics(null);
			int maxH = CalcDefaultHeight(g);
			try {
				ArrayList infos = GetAvailableEditViewInfos();
				foreach(BaseEditViewInfo editInfo in infos) {
					for(int n = 0; n < styles.Length; n++) {
						if(styles[n] == null) continue;
						editInfo.PaintAppearance = (AppearanceObject)((AppearanceObject)styles[n]).Clone();
						int minHeight = editInfo.CalcMinHeight(g);
						maxH = Math.Max(minHeight, maxH);
					}
				}
			}
			finally {
				gInfo.ReleaseGraphics();
			}
			return maxH;
		}
		int CalcDefaultHeight(Graphics g) {
			return new AppearanceObject().CalcDefaultTextSize(g).Height;
		}
	}
	public class TreeListPixelScrollingInfo {
		Dictionary<TreeListNode, int> nodesHeightCache = new Dictionary<TreeListNode, int>();
		int totalScrollableRowsHeight = -1;
		bool isValid = false;
		public TreeListPixelScrollingInfo(TreeList treeList) {
			TreeList = treeList;
		}
		protected TreeList TreeList { get; private set; }
		protected TreeListViewInfo ViewInfo { get { return TreeList.ViewInfo; } }
		public void Invalidate() {
			this.isValid = false;
			this.totalScrollableRowsHeight = -1;
			this.nodesHeightCache.Clear();
		}
		protected void ValidateCache() {
			if(isValid || !TreeList.IsPixelScrolling) return;
			int result = 0;
			IterateNodes(TreeList.Nodes, (currentNode) =>
			{
				int rowHeight = GetNodeHeight(currentNode);
				nodesHeightCache[currentNode] = result;
				result += rowHeight;
			});
			isValid = true;
		}
		public virtual TreeListNode GetNodeByPixelPosition(int pixelPostion) {
			ValidateCache();
			if(nodesHeightCache.Count == 0)
				return null;
			int index = nodesHeightCache.Values.ToList<int>().BinarySearch(pixelPostion);
			if(index < 0)
				index = ~index - 1;
			if(index < 0)
				index = 0;
			return nodesHeightCache.Keys.ToList<TreeListNode>()[index];
		}
		protected internal int GetNodeHeight(TreeListNode node) {
			return ViewInfo.GetTotalNodeHeight(node) + GetNodeFooterHeight(node);
		}
		int GetNodeFooterHeight(TreeListNode node) {
			if(node == null || !TreeList.OptionsView.ShowRowFooterSummary) return 0;
			int footerCount = 0;
			if(node == node.owner.LastNodeEx) {
				if(!(TreeListFilterHelper.HasVisibleChildren(node) && node.Expanded)) {
					footerCount++;
					TreeListNode parent = node.ParentNode;
					while(parent != null && parent == parent.owner.LastNodeEx) {
						footerCount++;
						parent = parent.ParentNode;
					}
				}
			}
			return ViewInfo.RC.GroupFooterHeight * footerCount;
		}
		public virtual int GetPixelPositionByNode(TreeListNode node) {
			if(node == null || TreeListAutoFilterNode.IsAutoFilterNode(node)) return 0;
			if(!nodesHeightCache.ContainsKey(node) && isValid)
				Invalidate();
			ValidateCache();
			return nodesHeightCache[node];
		}
		protected internal int GetTotalScrollableRowsHeight() {
			if(totalScrollableRowsHeight == -1) {
				int rowCount = TreeList.RowCount;
				if(rowCount > 0) {
					TreeListNode node = TreeList.GetNodeByVisibleIndex(rowCount - 1);
					if(node != null)
						totalScrollableRowsHeight = GetPixelPositionByNode(node) + GetNodeHeight(node);
				}
			}
			return totalScrollableRowsHeight < 0 ? 0 : totalScrollableRowsHeight;
		}
		void IterateNodes(TreeListNodes nodes, Action<TreeListNode> action) {
			for(int i = 0; i < nodes.Count; i++) {
				TreeListNode node = nodes[i];
				if(TreeListFilterHelper.IsNodeVisible(node)) 
					action(node);
				if(node.HasChildren && node.Expanded)
					IterateNodes(node.Nodes, action);
			}
		}
	}
}
