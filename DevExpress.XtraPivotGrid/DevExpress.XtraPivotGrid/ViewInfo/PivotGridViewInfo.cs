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
using System.Windows.Forms;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.Utils.Gesture;
using DevExpress.Utils.Menu;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Drawing;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraPivotGrid.Data;
using DevExpress.XtraPivotGrid.Localization;
using DevExpress.XtraPivotGrid.Printing;
using DevExpress.Skins;
namespace DevExpress.XtraPivotGrid.ViewInfo {
	internal class PivotCustomDrawAppearanceOwner : IPivotCustomDrawAppearanceOwner {
		AppearanceObject appearance;
		public PivotCustomDrawAppearanceOwner(AppearanceObject originalAppearance) {
			this.appearance = new AppearanceObject();
			AppearanceHelper.Combine(this.appearance, new AppearanceObject[] { originalAppearance });
		}
		public AppearanceObject Appearance {
			get { return appearance; }
			set {
				if(value == null)
					return;
				appearance = value;
			}
		}
	}
	public class FieldMeasures : CellSizeProvider {
		PivotGridAppearancesBase appearances;
		public new PivotGridViewInfoData Data { get { return (PivotGridViewInfoData)base.Data; } }
		public override int LeftCellPadding { get { return GetContentMargins().Left; } }
		public override int RightCellPadding { get { return GetContentMargins().Right; } }
		public override int BottomCellPadding { get { return GetContentMargins().Bottom; } }
		public override int TopCellPadding { get { return GetContentMargins().Top; } }
		public FieldMeasures(PivotGridViewInfoData data, PivotGridAppearancesBase appearances)
			: base(data, data.VisualItems) {
			Appearances = appearances;
		}
		protected PivotGridAppearancesBase Appearances {
			get { return appearances; }
			private set {
				if(value == null)
					return;
				appearances = value;
				appearances.Changed += delegate(object sender, EventArgs e) { Clear(); };
			}
		}
		protected override int GetColumnFieldWidth(PivotFieldValueItem item) {
			return Data.GetCustomColumnWidth(item, base.GetColumnFieldWidth(item));
		}
		protected override int GetRowFieldHeight(PivotFieldValueItem item) {
			return Data.GetCustomRowHeight(item, base.GetRowFieldHeight(item));
		}
		protected override int GetFieldValueHeight(int lineCount, PivotGridValueType valueType, PivotFieldItemBase field) {
			AppearanceObject appearance = Appearances.GetActualFieldValueAppearance(valueType, (PivotFieldItem)field);
			return CalculateHeaderHeightCore(false, lineCount, appearance);
		}
		protected override int CalculateHeaderHeight(PivotFieldItemBase field) {
			PivotFieldItem fd = field as PivotFieldItem;
			AppearanceObject headerAppearance;
			if(fd != null)
				headerAppearance = new AppearanceObject(fd.Appearance.Header, Appearances.FieldHeader);
			else
				headerAppearance = Appearances.FieldHeader;
			return CalculateHeaderHeightCore(true, 1, headerAppearance);
		}
		protected int CalculateHeaderHeightCore(bool isHeader, int lineCount, AppearanceObject headerAppearance) {
			HeaderObjectInfoArgs headerInfo = new HeaderObjectInfoArgs();
			bool allowGlyphSkinning = Data.OptionsView.AllowGlyphSkinning;
			headerInfo.SetAppearance(headerAppearance);
			if(isHeader) {
				headerInfo.InnerElements.Add(Data.ActiveLookAndFeel.Painter.SortedShape, new SortedShapeObjectInfoArgs());
				headerInfo.InnerElements.Add(FilterButtonHelper.GetPainter(Data.ActiveLookAndFeel), new GridFilterButtonInfoArgs());
				if(Data.HeaderImages != null) {
					headerInfo.InnerElements.Add(PivotViewInfo.CreateGlyphPainter(allowGlyphSkinning), PivotViewInfo.CreateGlyphInfoArgs(Data.HeaderImages, 0, allowGlyphSkinning));
				}
			} else {
				if(Data.ValueImages != null) {
					headerInfo.InnerElements.Add(PivotViewInfo.CreateGlyphPainter(allowGlyphSkinning), PivotViewInfo.CreateGlyphInfoArgs(Data.ValueImages, 0, allowGlyphSkinning));
				}
			}
			headerInfo.Caption = GetMeasureCaption(lineCount);
			headerInfo.Bounds = new Rectangle(0, 0, int.MaxValue, int.MaxValue);
			Graphics graphics = GraphicsInfo.Default.AddGraphics(null);
			GraphicsCache graphicsCache = new GraphicsCache(graphics);
			int height = 0;
			try {
				headerInfo.Cache = graphicsCache;
				Rectangle minBounds = Data.ActiveLookAndFeel.Painter.Header.CalcObjectMinBounds(headerInfo);
				if(Data.ActiveLookAndFeel.GetTouchUI()) {
					headerInfo.Bounds = minBounds;
					height = Data.ActiveLookAndFeel.Painter.Header.CalcBoundsByClientRectangle(headerInfo).Height;
				} else {
					height = minBounds.Height;
				}
				if(!isHeader) {
					float cellHeight = Math.Max(GetAppearancesCellHeight(Appearances.Cell, graphics, headerInfo.Caption),
						GetAppearancesCellHeight(Appearances.TotalCell, graphics, headerInfo.Caption));
					foreach(PivotGridField field in Data.GetFieldsByArea(PivotArea.DataArea, false)) {
						if(field.FieldEdit == null)
							continue;
						cellHeight = Math.Max(cellHeight, GetEditCellHeight(Data.GetFieldItem(field), headerInfo.Caption, graphics, Appearances.Cell));
						cellHeight = Math.Max(cellHeight, GetEditCellHeight(Data.GetFieldItem(field), headerInfo.Caption, graphics, Appearances.TotalCell));
					}
					if(cellHeight > height)
						height = (int)(cellHeight + 0.5);
				}
			} finally {
				graphicsCache.Dispose();
				GraphicsInfo.Default.ReleaseGraphics();
			}
			return height;
		}
		float GetAppearancesCellHeight(AppearanceObject appearance, Graphics graphics, string caption) {
			return appearance.CalcTextSize(graphics, caption, Int32.MaxValue).Height;
		}
		public override int GetFieldValueSeparator(PivotFieldValueItem fieldValueItem) {
			return Data.GetFieldValueSeparator(fieldValueItem);
		}
		protected override int CalculateHeaderWidth(PivotFieldItemBase field) {
			PivotFieldItem fieldItem = (PivotFieldItem)field;
			switch(fieldItem.Area) {
				case PivotArea.ColumnArea:
					return Data.ViewInfo.ColumnHeaders.GetBestWidth(fieldItem);
				case PivotArea.DataArea:
					return Data.ViewInfo.DataHeaders.GetBestWidth(fieldItem);
				case PivotArea.FilterArea:
					return Data.ViewInfo.FilterHeaders.GetBestWidth(fieldItem);
				case PivotArea.RowArea:
					return Data.ViewInfo.RowHeaders.GetBestWidth(fieldItem);
				default:
					throw new ArgumentException("Unsupported PivotArea");
			}
		}
		protected override int CalculateHeaderWidthOffset(PivotFieldItemBase field) {
			PivotFieldItem fieldItem = (PivotFieldItem)field;
			switch(fieldItem.Area) {
				case PivotArea.ColumnArea:
					return Data.ViewInfo.ColumnHeaders.GetWidthOffset(fieldItem);
				case PivotArea.DataArea:
					return Data.ViewInfo.DataHeaders.GetWidthOffset(fieldItem);
				case PivotArea.FilterArea:
					return Data.ViewInfo.FilterHeaders.GetWidthOffset(fieldItem);
				case PivotArea.RowArea:
					return Data.ViewInfo.RowHeaders.GetWidthOffset(fieldItem);
				default:
					throw new ArgumentException("Unsupported PivotArea");
			}
		}
		public int GetLevelLength(bool width, int level) {
			if(width)
				return GetWidthDifference(true, level, level + 1);
			else
				return GetHeightDifference(false, level, level + 1);
		}
		public int GetEditCellWidth(PivotFieldItem field, string maxText, Graphics graphics, AppearanceObject appearance) {
			return GetEditCellSize(field, maxText, graphics, appearance).Width;
		}
		public int GetEditCellHeight(PivotFieldItem field, string maxText, Graphics graphics, AppearanceObject appearance) {
			return GetEditCellSize(field, maxText, graphics, appearance).Height;
		}
		Size GetEditCellSize(PivotFieldItem field, string maxText, Graphics graphics, AppearanceObject appearance) {
			BaseEditViewInfo viewInfo = Data.viewInfo.GetEditViewInfo(field.FieldEdit);
			viewInfo.Item.BeginUpdate();
			AppearanceObject saved = viewInfo.PaintAppearance;
			viewInfo.PaintAppearance = appearance ?? Appearances.Cell;
			viewInfo.Item.CancelUpdate();
			viewInfo.DetailLevel = DetailLevel.Full;
			viewInfo.SetDisplayText(maxText);
			Size result = viewInfo.CalcBestFit(graphics);
			viewInfo.PaintAppearance = saved;
			return result;
		}
		Padding GetContentMargins() {
			return GridSkins.GetSkin(Data.ActiveLookAndFeel)[GridSkins.SkinHeader].ContentMargins.ToPadding();
		}
	}
	public class PivotGridViewInfo : PivotGridViewInfoBase {
		IViewInfoControl control;
		PivotFieldItem resizingField;
		int initResizingX;
		int drawResizingLineX;
		PivotGridViewInfoState state;
		PivotGridGestureScroller gestureScroller;
		PivotGridDragManager dragManager;
		FieldMeasures fieldMeasures;
		Dictionary<RepositoryItem, BaseEditViewInfo> editViewInfoCache;
		public static readonly GestureAllowArgs[] AllowedCellAreaGestures = new GestureAllowArgs[] { GestureAllowArgs.Pan };
		public PivotGridViewInfo(PivotGridViewInfoData data)
			: base(data) {
			this.control = data;
			this.resizingField = null;
			this.initResizingX = 0;
			this.drawResizingLineX = -1;
			this.gestureScroller = new PivotGridGestureScroller(this);
			this.dragManager = null;
			this.fieldMeasures = CreateFieldMeasures();
			BestFitter.SetSizeProvider(this.fieldMeasures);
		}
		public new PivotViewInfo this[int index] { get { return (PivotViewInfo)base[index]; } }
		public new PivotCellsViewInfo CellsArea { get { return (PivotCellsViewInfo)base.CellsArea; } }
		public new PivotFieldsAreaViewInfo ColumnAreaFields { get { return (PivotFieldsAreaViewInfo)base.ColumnAreaFields; } }
		public new PivotFieldsAreaViewInfo RowAreaFields { get { return (PivotFieldsAreaViewInfo)base.RowAreaFields; } }
		public bool IsShiftDown { get { return VisualItems.IsShiftDown; } }
		public Dictionary<RepositoryItem, BaseEditViewInfo> EditViewInfoCache {
			get {
				if(editViewInfoCache == null)
					editViewInfoCache = new Dictionary<RepositoryItem, BaseEditViewInfo>();
				return editViewInfoCache;
			}
		}
		public override bool IsPrefilterPanelVisible {
			get { return Data.OptionsCustomization.AllowPrefilter && !ReferenceEquals(Data.Prefilter.Criteria, null) && Data.IsCapabilitySupported(PivotDataSourceCaps.Prefilter); }
		}
		ILoadingPanelOwner LoadingPanelOwner {
			get { return Data as ILoadingPanelOwner; }
		}
		public override bool IsLoadingPanelVisible {
			get { return LoadingPanelOwner.IsMainLoadingPanelVisible; }
		}
		public override bool IsHorzScrollControl { get { return Data.OptionsBehavior.HorizontalScrolling == PivotGridScrolling.Control; } }
		public override bool CustomDrawFieldValue(ViewInfoPaintArgs e, PivotFieldsAreaCellViewInfoBase fieldCellViewInfo, PivotHeaderObjectInfoArgs info,
			PivotHeaderObjectPainter painter, MethodInvoker defaultDraw) {
			return Data.CustomDrawFieldValue(e, (PivotFieldsAreaCellViewInfo)fieldCellViewInfo, info, painter, defaultDraw);
		}
		public override bool DrawFieldHeader(PivotHeaderViewInfoBase headerViewInfo, ViewInfoPaintArgs e, HeaderObjectPainter painter, MethodInvoker defaultDraw) {
			return Data.CustomDrawFieldHeader(headerViewInfo, e, painter, defaultDraw);
		}
		public override bool DrawHeaderArea(PivotHeadersViewInfoBase headersViewInfo, ViewInfoPaintArgs e, Rectangle bounds, MethodInvoker defaultDraw) {
			return Data.CustomDrawHeaderArea(headersViewInfo, e, bounds, defaultDraw);
		}
		public override bool ScrollBarOverlap { get { return control.ScrollBarOverlap; } }
		protected override Rectangle ClientRectangle { get { return Control != null ? Control.ClientRectangle : Rectangle.Empty; } }
		public override void ClientSizeChanged() {
			base.ClientSizeChanged();
		}
		protected override void Invalidate(Rectangle bounds) {
			base.Invalidate(bounds);
			if(Control != null)
				Control.Invalidate(bounds);
		}
		protected IViewInfoControl Control { get { return control; } }
		public override Point LeftTopCoord {
			get {
				return base.LeftTopCoord;
			}
			set {
				base.LeftTopCoord = value;
				if(Control != null)
					Control.InvalidateScrollBars();
			}
		}
		public override PivotGridViewInfoState State { get { return state; } }
		protected internal override int FilterHeadersHeight {
			get {
				int height = base.FilterHeadersHeight;
				if(Data.OptionsView.ShowFilterSeparatorBar) {
					height += 2 * Data.OptionsView.FilterSeparatorBarPadding + 1;
				}
				return height;
			}
		}
		#region Field Measures
		public override FieldMeasures FieldMeasures { get { return fieldMeasures; } }
		protected virtual FieldMeasures CreateFieldMeasures() {
			return new FieldMeasures(Data, PrintAndPaintAppearance);
		}
		protected override void OnVisualItemsCleared(object sender, EventArgs e) {
			base.OnVisualItemsCleared(sender, e);
			this.fieldMeasures.Clear();
		}
		#endregion
		protected override void OnAfterCalculated() {
			base.OnAfterCalculated();
			VisualItems.CorrectSelection();
		}
		public PivotFieldItem GetFieldAt(Point pt) {
			EnsureIsCalculated();
			for(int i = 0; i < ChildCount; i++) {
				PivotFieldItem field = this[i].GetFieldAt(pt);
				if(field != null)
					return field;
			}
			return null;
		}
		public bool IsDragging { get { return this.dragManager != null; } }
		public void StartDragging(PivotFieldItem field, DragCompletedCallback callback) {
			if(Data.IsLockUpdate)
				return;
			Data.UserAction = UserAction.FieldDrag;
			dragManager = Data.CreateDragManager(field, callback);
			dragManager.DoDragDrop();
		}
		public void DisposeDragManager() {
			StopDragging();
		}
		public void StopDragging() {
			if(dragManager == null)
				return;
			dragManager.Dispose();
			dragManager = null;
			Data.UserAction = UserAction.None;
		}
		public override PivotFieldItem GetSizingField(int x, int y) {
			if(IsLocked)
				return null;
			if(!IsReady)
				EnsureIsCalculated();
			if(IsPrefilterPanelVisible && PrefilterPanel.PaintBounds.Contains(x, y))
				return null;
			if(CellsArea.PaintBounds.Contains(new Point(x, y))) {
				if(RowAreaFields.PaintBounds.Contains(new Point(x - FrameBorderWidth, y)))
					x -= FrameBorderWidth;
			}
			if(RowAreaFields.PaintBounds.Contains(new Point(x, y)))
				return RowAreaFields.GetSizingField(new Point(x, y));
			if(ColumnAreaFields.PaintBounds.Contains(new Point(x, y)))
				return ColumnAreaFields.GetSizingField(new Point(x, y));
			return null;
		}
		bool CanResizeField(int resizingDelta) {
			return this.resizingField.Width + resizingDelta >= this.resizingField.MinWidth;
		}
		int GetResizeDelta(int newX) {
			return IsRightToLeft ? this.initResizingX - newX : newX - this.initResizingX; 
		}
		protected override PivotHeadersViewInfoBase CreateHeadersViewInfo(int i) {
			return new PivotHeadersViewInfo(this, (PivotArea)i);
		}
		public override void KeyDown(KeyEventArgs e) {
			base.KeyDown(e);
			KeyDown(e.KeyCode, e.Control, e.Shift);
		}
		public void KeyDown(Keys keyCode, bool control, bool shift) {
			Point newFocusedCell = VisualItems.GetKeyDownNextFocusedCell();
			switch(keyCode) {
				case Keys.Escape:
					if(CanColumnResizing)
						StopResizing();
					break;
				case Keys.PageDown: {
						int visibleRowCount = Scroller.VisibleRowCount;
						int newY = newFocusedCell.Y < LeftTopCoord.Y + visibleRowCount - 1 ? LeftTopCoord.Y + visibleRowCount - 1 : newFocusedCell.Y + visibleRowCount;
						newFocusedCell = new Point(newFocusedCell.X, newY < CellsArea.RowCount ? newY : CellsArea.RowCount - 1);
						break;
					}
				case Keys.PageUp: {
						int newY = newFocusedCell.Y > LeftTopCoord.Y ? LeftTopCoord.Y : newFocusedCell.Y - Scroller.VisibleRowCount;
						newFocusedCell = new Point(newFocusedCell.X, newY >= 0 ? newY : 0);
						break;
					}
				case Keys.Insert:
				case Keys.C:
					if(control)
						VisualItems.CopySelectionToClipboard();
					break;
			}
			int virtualKeyCode = (int)keyCode;
			if(IsRightToLeft) {
				if(keyCode == Keys.Left)
					virtualKeyCode = (int)Keys.Right;
				else if(keyCode == Keys.Right)
					virtualKeyCode = (int)Keys.Left;
			}
			VisualItems.OnKeyDown(virtualKeyCode, control, shift);
		}
		protected PivotFieldItem ResizingField { get { return resizingField; } }
		protected bool CanColumnResizing { get { return !IsLocked && Data.OptionsCustomization.AllowResizing && (resizingField != null); } }
		protected bool IsLocked { get { return Data.IsLockUpdate; } }
		public override void MouseDown(MouseEventArgs e) {
			if(e.Button == MouseButtons.Left) {
				this.resizingField = GetSizingField(e.X, e.Y);
				if(CanColumnResizing) {
					Data.UserAction = UserAction.FieldResize;
					this.initResizingX = e.X;
					this.state = PivotGridViewInfoState.FieldResizing;
					DrawSizingLine(e.X, true);
					return;
				}
				StartColumnRowSelection(e);
			}
			if(Data.ControlOwner != null && e.Button == MouseButtons.Middle && e.Clicks == 1) {
				if(this.CellsArea.PaintBounds.Contains(e.X, e.Y)) {
					Data.ControlOwner.Focus();
					this.gestureScroller.Start(Data.ControlOwner);
				}
			}
			base.MouseDown(e);
		}
		protected override BaseViewInfo MouseDownCore(MouseEventArgs e) {
			if((e.Button & MouseButtons.Right) != 0 && Data.OptionsMenu.EnableHeaderAreaMenu &&
					Data.OptionsView.ShowRowHeaders == false && Data.OptionsView.ShowDataHeaders == false &&
					e.Y > ColumnAreaFields.Bounds.Top && e.Y < ColumnAreaFields.Bounds.Bottom &&
					e.X > RowAreaFields.Bounds.Left && e.X < RowAreaFields.Bounds.Right) {
				new PivotHeadersViewInfo(this, PivotArea.DataArea).ShowPopupMenu(e);
			}
			return base.MouseDownCore(e);
		}
		public override void MouseMove(MouseEventArgs e) {
			if(!IsReady)
				return;
			if(CanColumnResizing) {
				if(CanResizeField(GetResizeDelta(e.X)))
					DrawSizingLine(e.X, true);
				return;
			}
			if(Data.OptionsCustomization.AllowResizing && GetSizingField(e.X, e.Y) != null)
				this.state = PivotGridViewInfoState.FieldResizing;
			else
				this.state = PivotGridViewInfoState.Normal;
			PerformColumnRowSelection(e);
			base.MouseMove(e);
		}
		public override void MouseUp(MouseEventArgs e) {
			if(CanColumnResizing) {
				int newX = CorrectXToRightActualBoundsCorner(e.X);
				int resizeDelta = GetResizeDelta(newX);
				if(CanResizeField(resizeDelta)) {
					new PivotGridFieldUISetWidthAction(this.resizingField, Data).SetWidth(this.resizingField.Width + resizeDelta);
				} else
					new PivotGridFieldUISetWidthAction(this.resizingField, Data).SetWidth(this.resizingField.MinWidth);
				StopResizing();
				return;
			}
			StopColumnRowSelection();
			base.MouseUp(e);
		}
		public override void DoubleClick(MouseEventArgs e) {
			if(CanColumnResizing) {
				BestFit(this.resizingField);
			} else
				base.DoubleClick(e);
		}
		public void OnGestureBegin(GestureArgs info) {
			this.gestureScroller.ResetOffsetAccumulator();
		}
		public void OnGesturePan(GestureArgs info, Point delta, ref Point overPan) {
			this.gestureScroller.GestureScroll(delta, ref overPan);
		}
		public override void OnGestureTwoFingerSelection(Point start, Point end) {
			base.OnGestureTwoFingerSelection(start, end);
		}
		void PerformColumnRowSelection(MouseEventArgs e) {
			if(VisualItemsInternal.IsValueMouseDown) {
				Point p = e.Location;
				if(VisualItems.ValueSelectionArea == PivotArea.RowArea) {
					p.X = Math.Max(p.X, RowAreaFields.PaintBounds.Left + 1);
					p.X = Math.Min(p.X, RowAreaFields.PaintBounds.Right + 1);
					p.Y = Math.Max(p.Y, RowAreaFields.PaintBounds.Top + 1);
					p.Y = Math.Min(p.Y, RowAreaFields.PaintBounds.Bottom - 1);
				} else {
					p.X = Math.Max(p.X, ColumnAreaFields.PaintBounds.Left + 1);
					p.X = Math.Min(p.X, ColumnAreaFields.PaintBounds.Right - 1);
					p.Y = Math.Max(p.Y, ColumnAreaFields.PaintBounds.Top + 1);
					p.Y = Math.Min(p.Y, ColumnAreaFields.PaintBounds.Bottom - 1);
				}
				PivotFieldsAreaCellViewInfo activeViewInfo = GetViewInfoAtPoint(p) as PivotFieldsAreaCellViewInfo;
				if(activeViewInfo != null && !activeViewInfo.IsMouseOnCollapseButton(e.Location))
					VisualItems.PerformColumnRowSelection(activeViewInfo.Item);
			}
		}
		void StartColumnRowSelection(MouseEventArgs e) {
			PivotFieldsAreaCellViewInfo viewInfo = GetViewInfoAtPoint(e.X, e.Y) as PivotFieldsAreaCellViewInfo;
			if(e.Button == MouseButtons.Left && viewInfo != null &&
					(!IsPrefilterPanelVisible || !PrefilterPanel.PaintBounds.Contains(e.Location)) &&
				!viewInfo.IsMouseOnCollapseButton(e.Location)
				) {
				VisualItems.StartColumnRowSelection(viewInfo.Item, CellsArea.GetCellCoordAt(e.Location));
			}
		}
		void StopColumnRowSelection() {
			VisualItems.StopColumnRowSelection();
		}
		public bool AcceptDragDrop(Point pt) {
			if(Control.ControlOwner != null && !Control.ControlOwner.Visible)
				return false;
			EnsureIsCalculated();
			if(Control.ControlOwner != null)
				pt = Control.ControlOwner.PointToClient(pt);
			for(int i = 0; i < ChildCount; i++) {
				if(Rectangle.Intersect(this[i].PaintBounds, this.PaintBounds).Contains(pt))
					return this[i].AcceptDragDrop;
			}
			return false;
		}
		public Rectangle GetDragDrawRectangle(PivotFieldItem field, Point pt) {
			EnsureIsCalculated();
			if(Control.ControlOwner != null)
				pt = Control.ControlOwner.PointToClient(pt);
			for(int i = 0; i < ChildCount; i++) {
				if(!this[i].PaintBounds.Contains(pt))
					continue;
				return this[i].GetDragDrawRectangle(field, pt);
			}
			return Rectangle.Empty;
		}
		public void HighLightArea(Point pt) {
			EnsureIsCalculated();
			if(Control.ControlOwner != null)
				pt = Control.ControlOwner.PointToClient(pt);
			for(int i = 0; i < ChildCount; i++) {
				if(this[i].PaintBounds.Contains(pt)) {
					HighLightedArea = this[i];
					return;
				}
			}
			HighLightedArea = null;
		}
		public int GetNewFieldPosition(PivotFieldItem field, Point pt, out PivotArea area) {
			area = PivotArea.FilterArea;
			if(Control.ControlOwner != null)
				pt = Control.ControlOwner.PointToClient(pt);
			for(int i = 0; i < ChildCount; i++) {
				if(!this[i].PaintBounds.Contains(pt))
					continue;
				int newAreaIndex = this[i].GetNewFieldPosition(field, pt, out area);
				if(newAreaIndex > -1) {
					return newAreaIndex;
				}
			}
			return -1;
		}
		void DrawSizingLine(int x, bool show) {
			EnsureIsCalculated();
			Rectangle bounds = GetSizingLineBounds(x);
			DrawSizingLine(this.drawResizingLineX, bounds.Top, bounds.Bottom);
			this.drawResizingLineX = show ? bounds.Left : -1;
			DrawSizingLine(this.drawResizingLineX, bounds.Top, bounds.Bottom);
		}
		int CorrectXToRightActualBoundsCorner(int x) {
			if(x > PaintBoundsWithScroll.Right - FrameBorderWidth)
				return PaintBoundsWithScroll.Right - FrameBorderWidth;
			else if(x < PaintBoundsWithScroll.Left + FrameBorderWidth)
				return PaintBoundsWithScroll.Left + FrameBorderWidth;
			else
				return x;
		}
		internal Rectangle GetSizingLineBounds(int x) {
			x = CorrectXToRightActualBoundsCorner(x);
			int top = RowAreaFields.PaintBounds.Top;
			if(this.resizingField != null && this.resizingField.Area != PivotArea.RowArea)
				top -= FieldMeasures.DefaultHeaderHeight;
			int bottom = PivotScrollableRectangle.Bottom;
			if(IsHScrollBarVisible)
				bottom -= ScrollBarSize.Height;
			return new Rectangle(x, top, 2, bottom - top);
		}
		void DrawSizingLine(int x, int top, int bottom) {
			if(x >= 0 && Control.ControlOwner != null) {
				Point start = new Point(x, top);
				Point end = new Point(x, bottom);
				SplitterLineHelper.Default.DrawReversibleLine(Control.ControlOwner.Handle, start, end);
			}
		}
		void StopResizing() {
			StopResizing(true);
		}
		void StopResizing(bool refreshCustomizationForm) {
			DrawSizingLine(-1, false);
			this.initResizingX = 0;
			this.drawResizingLineX = -1;
			if(this.resizingField != null)
				Data.UserAction = UserAction.None;
			this.resizingField = null;
			this.state = PivotGridViewInfoState.Normal;
			if(refreshCustomizationForm && Data.CustomizationForm != null)
				Data.CustomizationForm.Refresh();
		}
		void CloseFilterPopups() {
			((PivotHeadersViewInfo)ColumnHeaders).CloseFilterPopups();
			((PivotHeadersViewInfo)RowHeaders).CloseFilterPopups();
			((PivotHeadersViewInfo)FilterHeaders).CloseFilterPopups();
			((PivotHeadersViewInfo)DataHeaders).CloseFilterPopups();
		}
		internal void ClearBeforeAsyncOperation() {
			StopResizing(false);
			if(!Data.OptionsBehavior.UseAsyncMode)
				CloseFilterPopups();
		}
		protected override void InternalClear() {
			base.InternalClear();
			this.state = PivotGridViewInfoState.Normal;
			if(this.editViewInfoCache != null)
				this.editViewInfoCache.Clear();
			if(FieldMeasures != null)
				FieldMeasures.Clear();
		}
		protected override bool CustomDrawEmptyArea(IPivotCustomDrawAppearanceOwner appearanceOwner, ViewInfoPaintArgs e, Rectangle bounds, MethodInvoker defaultDraw) {
			return Data.CustomDrawEmptyArea(appearanceOwner, e, bounds, defaultDraw);
		}
		protected override PivotCellsViewInfoBase CreateCellsViewInfo() {
			return new PivotCellsViewInfo(this);
		}
		protected override PivotFieldsAreaViewInfoBase CreateFieldsAreawViewInfo(bool isColumn) {
			return new PivotFieldsAreaViewInfo(this, isColumn);
		}
		protected override PivotPrefilterPanelViewInfoBase CreatePrefilterPanelViewInfo() {
			return new PivotPrefilterPanelViewInfo(this);
		}
		public virtual ToolTipControlInfo GetToolTipObjectInfo(Point pt) {
			for(int i = 0; i < ChildCount; i++)
				if(this[i].PaintBounds.Contains(pt))
					return this[i].GetToolTipObjectInfo(pt);
			return null;
		}
		public PivotGridHitInfo CalcHitInfo(Point hitPoint) {
			EnsureIsCalculated();
			if(!PaintBounds.Contains(hitPoint))
				return new PivotGridHitInfo(hitPoint);
			for(int i = 0; i < ChildCount; i++) {
				if(this[i].PaintBounds.Contains(hitPoint))
					return this[i].CalcHitInfo(hitPoint);
			}
			return new PivotGridHitInfo(hitPoint);
		}
		protected override void OnAfterChildCalculated(BaseViewInfo viewInfo) {
			base.OnAfterChildCalculated(viewInfo);
			if(Control != null)
				Control.InvalidateScrollBars();
		}
		public BaseEditViewInfo GetEditViewInfo(RepositoryItem repositoryItem) {
			if(!EditViewInfoCache.ContainsKey(repositoryItem)) {
				EditViewInfoCache.Add(repositoryItem, CreateEditViewInfo(repositoryItem));
			}
			return EditViewInfoCache[repositoryItem];
		}
		public DetailLevel GetEditDetailLevel(PivotCellViewInfo cellViewInfo) {
			PivotFieldItem dataField = cellViewInfo.DataField;
			bool focusedCell = VisualItems.IsCellFocused(cellViewInfo);
			PivotShowButtonModeEnum mode = dataField.Options.ShowButtonMode;
			if(mode == PivotShowButtonModeEnum.Default)
				mode = Data.OptionsView.ShowButtonMode;
			if(mode == PivotShowButtonModeEnum.Default)
				mode = PivotShowButtonModeEnum.ShowForFocusedCell;
			bool showButtons = (mode == PivotShowButtonModeEnum.ShowAlways) ||
				(focusedCell && mode == PivotShowButtonModeEnum.ShowForFocusedCell);
			return showButtons ? DetailLevel.Full : DetailLevel.Minimum;
		}
		protected BaseEditViewInfo CreateEditViewInfo(RepositoryItem repositoryItem) {
			BaseEditViewInfo res = repositoryItem.CreateViewInfo();
			res.InplaceType = InplaceType.Grid;
			return res;
		}
	}
	public abstract class PivotGridViewInfoBase : BaseViewInfo {
		public const int FieldResizingOffset = 2;
		const int firstLastHeaderWidthOffset = 1;
		PivotGridViewInfoData data;
		PivotFieldsAreaViewInfoBase columnAreaFields;
		PivotFieldsAreaViewInfoBase rowAreaFields;
		PivotCellsViewInfoBase cellsArea;
		PivotPrefilterPanelViewInfoBase prefilterPanel;
		LoadingPanel loadingPanel;
		LoadingAnimator loadingAnimator;
		Size scrollableSize;
		PivotHeadersViewInfoBase[] headers;
		Point leftTopCoord;
		PivotViewInfo highLightedArea;
		PivotGridBestFitter bestFitter;
		PivotGridScrollBarsViewInfo scrollbars;
		PivotGridScroller scroller;
		public PivotFieldItemCollection FieldItems {
			get { return data.FieldItems; }
		}
		internal PivotGridBestFitter BestFitter { get { return bestFitter; } }
		internal PivotGridScroller Scroller { get { return scroller; } }
		public PivotGridViewInfoBase(PivotGridViewInfoData data)
			: base() {
			this.leftTopCoord = Point.Empty;
			this.data = data;
			this.rowAreaFields = null;
			this.columnAreaFields = null;
			this.cellsArea = null;
			this.highLightedArea = null;
			this.headers = new PivotHeadersViewInfoBase[Enum.GetValues(typeof(PivotArea)).Length];
			this.scrollbars = new PivotGridScrollBarsViewInfo(this);
			InternalClear();
			VisualItems.Cleared += OnVisualItemsCleared;
			VisualItems.AfterCalculating += OnVisualItemsAfterCalculating;
			Data.FieldSizeChanged += OnFieldSizeChanged;
			bestFitter = new PivotGridBestFitter(this);
			scroller = PivotGridScroller.CreateInstance(this);
		}
		public override void Dispose() {
			base.Dispose();
			if(VisualItems != null) {
				VisualItems.Cleared -= OnVisualItemsCleared;
				VisualItems.AfterCalculating -= OnVisualItemsAfterCalculating;
			}
			if(Data != null)
				Data.FieldSizeChanged -= OnFieldSizeChanged;
			if(loadingPanel != null) {
				LoadingPanel.Dispose();
				loadingPanel = null;
			}
			if(loadingAnimator != null) {
				LoadingAnimator.Dispose();
				loadingAnimator = null;
			}
		}
		public PivotGridViewInfoData Data { get { return data; } }
		public PivotVisualItems VisualItems { get { return Data != null ? Data.VisualItems : null; } }
		public PivotVisualItems VisualItemsInternal { get { return Data != null ? Data.VisualItemsInternal : null; } }
		public new PivotViewInfo this[int index] { get { return (PivotViewInfo)base[index]; } }
		public PivotFieldsAreaViewInfoBase ColumnAreaFields { get { return columnAreaFields; } }
		public PivotFieldsAreaViewInfoBase RowAreaFields { get { return rowAreaFields; } }
		public PivotCellsViewInfoBase CellsArea { get { return cellsArea; } }
		public PivotPrefilterPanelViewInfoBase PrefilterPanel { get { return prefilterPanel; } }
		public PivotHeadersViewInfoBase GetHeader(PivotArea area) { return headers[(int)area]; }
		public int HeaderCount { get { return this.headers.Length; } }
		public virtual bool CanShowHeader(PivotArea area) {
			return Data.OptionsView.GetShowHeaders(area);
		}
		public virtual bool IsPrefilterPanelVisible { get { return false; } }
		public virtual bool IsLoadingPanelVisible { get { return false; } }
		public virtual bool IsEnabled { get { return Data.IsEnabled; } }
		public virtual bool UseDisabledStatePainter { get { return Data.UseDisabledStatePainter; } }
		protected void CreateHeaders() {
			for(int i = 0; i < HeaderCount; i++)
				headers[i] = CreateHeadersViewInfo(i);
		}
		protected virtual PivotHeadersViewInfoBase CreateHeadersViewInfo(int i) {
			return new PivotHeadersViewInfoBase(this, (PivotArea)i);
		}
		public PivotHeadersViewInfoBase ColumnHeaders { get { return GetHeader(PivotArea.ColumnArea); } }
		public PivotHeadersViewInfoBase RowHeaders { get { return GetHeader(PivotArea.RowArea); } }
		public PivotHeadersViewInfoBase FilterHeaders { get { return GetHeader(PivotArea.FilterArea); } }
		public PivotHeadersViewInfoBase DataHeaders { get { return GetHeader(PivotArea.DataArea); } }
		public virtual int FirstLastHeaderWidthOffset { get { return firstLastHeaderWidthOffset; } }
		public override Rectangle ControlBounds { get { return Data.ClientRectangleAccordingBounds; } }
		public virtual Point LeftTopCoord {
			get { return leftTopCoord; }
			set {
				if(LeftTopCoord == value)
					return;
				EnsureIsCalculated();
				SetLeftTopCoordCore(value);
			}
		}
		public Point MaximumLeftTopCoord {
			get {
				EnsureIsCalculated();
				return scroller.MaximumLeftTopCoord;
			}
		}
		public virtual bool IsHorzScrollControl { get { return false; } }
		Rectangle? scrollableRectangle;
		public Rectangle PivotScrollableRectangle {
			get {
				if(!scrollableRectangle.HasValue) {
					Rectangle bounds = Data.ClientRectangleAccordingBounds;
					if(IsPrefilterPanelVisible)
						bounds.Height -= PivotPrefilterPanelViewInfoBase.CalcHeight(Data);
					scrollableRectangle = bounds;
				}
				return scrollableRectangle.Value;
			}
		}
		protected virtual Rectangle ClientRectangle { get { return Rectangle.Empty; } }
		public virtual bool ScrollBarOverlap { get { return false; } }
		protected PivotGridScrollBarsViewInfo ScrollBars { get { return scrollbars; } }
		#region ScrollBars
		public override Rectangle PaintBoundsWithScroll {
			get {
				Rectangle bounds = base.PaintBoundsWithScroll;
				if(!ScrollBarOverlap && IsVScrollBarVisible && bounds.Right > RowAreaFields.ControlBounds.Right + ScrollBarSize.Width)
					bounds.Width -= ScrollBarSize.Width;
				return RightToLeftRect(bounds);
			}
		}
		public Size ScrollBarSize { get { return PivotGridScrollBarsViewInfo.ScrollBarSize; } }
		public Rectangle ScrollableBounds {
			get {
				EnsureIsCalculated();
				return scroller.ScrollableBounds;
			}
		}
		public bool IsHScrollBarVisible {
			get {
				EnsureIsCalculated();
				return scrollbars.HScrollVisible;
			}
		}
		public bool IsVScrollBarVisible {
			get {
				EnsureIsCalculated();
				return scrollbars.VScrollVisible;
			}
		}
		public ScrollArgs HScrollBarInfo {
			get {
				EnsureIsCalculated();
				return scrollbars.HScrollBarInfo;
			}
		}
		public ScrollArgs VScrollBarInfo {
			get {
				EnsureIsCalculated();
				return scrollbars.VScrollBarInfo;
			}
		}
		#endregion
		public virtual void ClientSizeChanged() {
			scrollableRectangle = null;
			Bounds = ClientRectangle;
			ClearOnClientSizeChanged();
			EnsureIsCalculated();
			scroller.Recalculate();
			CellsArea.CalculateCellsViewInfo();
			Point maxLeftTopCoord = scroller.MaximumLeftTopCoord;
			if(maxLeftTopCoord.X < LeftTopCoord.X || maxLeftTopCoord.Y < LeftTopCoord.Y) {
				Point newLeftTopCoord = new Point {
					X = Math.Min(maxLeftTopCoord.X, LeftTopCoord.X),
					Y = Math.Min(maxLeftTopCoord.Y, LeftTopCoord.Y)
				};
				SetLeftTopCoordCore(newLeftTopCoord);
			}
			if(FilterHeaders != null)
				FilterHeaders.ResetPaintBounds();
			if(PrefilterPanel != null)
				PrefilterPanel.ResetPaintBounds();
		}
		protected void ClearOnClientSizeChanged() {
			if(!IsReady)
				return;
			SetFilterAndColumnHeadersWidth();
			if(CanShowHeader(PivotArea.ColumnArea)) {
				ColumnHeaders.CorrectHeadersWidth();
			}
			if(CanShowHeader(PivotArea.FilterArea)) {
				int oldHeight = FilterHeaders.Bounds.Height;
				FilterHeaders.Height = FilterHeadersHeight;
				FilterHeaders.CorrectHeadersHeight();
				if(FilterHeaders.Bounds.Height != oldHeight)
					Clear();
			}
		}
		protected LoadingPanel LoadingPanel {
			get {
				if(loadingPanel == null) {
					loadingPanel = new LoadingPanel(Data.ControlOwner);
					loadingPanel.LookAndFeel = Data.ActiveLookAndFeel;
				}
				return loadingPanel;
			}
		}
		protected LoadingAnimator LoadingAnimator {
			get {
				if(loadingAnimator == null) {
					loadingAnimator = new LoadingAnimator(Data.ControlOwner, LoadingAnimator.LoadingImage);
				}
				return loadingAnimator;
			}
		}
		public void BestFit() {
			bestFitter.BestFit();
		}
		public void BestFitColumnArea() {
			bestFitter.BestFitColumnArea();
		}
		public void BestFitRowArea() {
			bestFitter.BestFitRowArea();
		}
		public void BestFitDataHeaders(bool considerRowArea) {
			bestFitter.BestFitDataHeaders(considerRowArea);
		}
		public void BestFit(PivotFieldItem field) {
			bestFitter.BestFit(field);
		}
		protected virtual void BestFitRowField(PivotFieldItem field) {
			bestFitter.BestFitRowField(field);
		}
		protected virtual void BestFitDataField(PivotFieldItem field) {
			bestFitter.BestFitDataField(field);
		}
		protected virtual void BestFitColumnField(PivotFieldItem field) {
			bestFitter.BestFitColumnField(field);
		}
		protected override void InternalClear() {
			base.InternalClear();
			this.scrollableRectangle = null;
			this.scrollableSize = Size.Empty;
		}
		public PivotViewInfo HighLightedArea {
			get { return highLightedArea; }
			set {
				if(HighLightedArea == value)
					return;
				PivotViewInfo oldHighLightedArea = HighLightedArea;
				this.highLightedArea = value;
				if(oldHighLightedArea != null)
					oldHighLightedArea.InvalidateHighLight();
				if(HighLightedArea != null)
					HighLightedArea.InvalidateHighLight();
			}
		}
		public virtual bool AllowExpand { get { return true; } }
		public virtual bool CustomDrawFieldValue(ViewInfoPaintArgs e, PivotFieldsAreaCellViewInfoBase fieldCellViewInfo,
			PivotHeaderObjectInfoArgs info, PivotHeaderObjectPainter painter, MethodInvoker defaultDraw) {
			return true;
		}
		public virtual bool DrawFieldHeader(PivotHeaderViewInfoBase headerViewInfo, ViewInfoPaintArgs e, HeaderObjectPainter painter, MethodInvoker defaultDraw) {
			return true;
		}
		public virtual bool DrawHeaderArea(PivotHeadersViewInfoBase headersViewInfo, ViewInfoPaintArgs e, Rectangle bounds, MethodInvoker defaultDraw) {
			return true;
		}
		protected override bool CheckControlBounds { get { return false; } }
		protected override void InternalPaint(ViewInfoPaintArgs e) {
			AdjustPrefilterBounds();
			AdjustLoadingPanelBounds();
			DrawHeadersAndFields(e);
			DrawHorzEmptySpace(e);
			DrawVertEmptySpace(e);
		}
		protected override void AfterPaint(ViewInfoPaintArgs e) {
			base.AfterPaint(e);
			ScrollBars.PaintEmptyScrollFieldValueCell(e);
			PaintBorder(e);
			PaintDisabled(e);
			PaintLoadingPanel(e);
		}
		protected void PaintBorder(ViewInfoPaintArgs e) {
			BorderObjectInfoArgs borderArgs = new BorderObjectInfoArgs(e.GraphicsCache, Data.PaintAppearance.Empty, ClientRectangle); 
			BorderPainter borderPainter = BorderHelper.GetGridPainter(Data.BorderStyle, Data.ActiveLookAndFeel); 
			borderPainter.CalcObjectBounds(borderArgs);
			borderPainter.DrawObject(borderArgs);
		}
		private void PaintDisabled(ViewInfoPaintArgs e) {
			if(!IsEnabled && UseDisabledStatePainter)
				BackgroundPaintHelper.PaintDisabledControl(Data.ActiveLookAndFeel, e.GraphicsCache, e.ClientRectangle);
		}
		void PaintLoadingPanel(ViewInfoPaintArgs e) {
			if(IsLoadingPanelVisible) {
				LoadingPanel.Bounds = Bounds;
				LoadingPanel.Draw(e.GraphicsCache);
			} else {
				LoadingAnimator.StopAnimation();
				LoadingPanel.Stop();
			}
		}
		public int FrameBorderWidth { get { return System.Windows.Forms.SystemInformation.FrameBorderSize.Width / 2; } }
		void DrawHorzEmptySpace(ViewInfoPaintArgs e) {
			if(RowAreaFields.ControlBounds.Bottom >= Bounds.Bottom)
				return;
			Rectangle bounds = new Rectangle(Bounds.X, RowAreaFields.ControlBounds.Bottom, Bounds.Width, Bounds.Bottom - RowAreaFields.ControlBounds.Bottom);
			DrawEmptySpace(e, bounds);
		}
		void DrawVertEmptySpace(ViewInfoPaintArgs e) {
			if(ColumnAreaFields.ControlBounds.Right >= Bounds.Right)
				return;
			int x = ColumnAreaFields.ControlBounds.Right;
			int width = Bounds.Right - ColumnAreaFields.ControlBounds.Right;
			Rectangle bounds = RightToLeftRect(new Rectangle(x, ColumnAreaFields.ControlBounds.Y, width, RowAreaFields.ControlBounds.Bottom));
			DrawEmptySpace(e, bounds);
		}
		public override bool IsRightToLeft { get { return Data.IsRightToLeft; } }
		protected void DrawEmptySpace(ViewInfoPaintArgs e, Rectangle bounds) {
			PivotCustomDrawAppearanceOwner appearanceOnwer = new PivotCustomDrawAppearanceOwner(Data.PaintAppearance.Empty);
			MethodInvoker defaultDraw = () => appearanceOnwer.Appearance.FillRectangle(e.GraphicsCache, bounds);
			if(!CustomDrawEmptyArea(appearanceOnwer, e, bounds, defaultDraw))
				defaultDraw();
		}
		protected virtual bool CustomDrawEmptyArea(IPivotCustomDrawAppearanceOwner appearanceOwner, ViewInfoPaintArgs e, Rectangle bounds, MethodInvoker defaultDraw) {
			return false;
		}
		void DrawHeadersAndFields(ViewInfoPaintArgs e) {
			Rectangle bounds;
			if(CanShowHeader(PivotArea.FilterArea))
				bounds = new Rectangle(FilterHeaders.ControlBounds.X, FilterHeaders.ControlBounds.Y, FilterHeaders.ControlBounds.Width, ColumnAreaFields.ControlBounds.Bottom - FilterHeaders.ControlBounds.Y);
			else
				bounds = new Rectangle(PivotScrollableRectangle.Left,
					CanShowHeader(PivotArea.DataArea) ? DataHeaders.ControlBounds.Y : CanShowHeader(PivotArea.ColumnArea) ? ColumnHeaders.ControlBounds.Y : PivotScrollableRectangle.Top,
						FilterHeaders.ControlBounds.Width,
							ColumnAreaFields.ControlBounds.Bottom - FilterHeaders.ControlBounds.Y);
			if(bounds.Width > PivotScrollableRectangle.Width)
				bounds.Width = PivotScrollableRectangle.Width;
			FillHeadersAndFields(e, bounds);
		}
		protected virtual void FillHeadersAndFields(ViewInfoPaintArgs e, Rectangle bounds) {
			StyleObjectInfoArgs infoArgs = new StyleObjectInfoArgs(e.GraphicsCache, bounds, Data.PaintAppearance.HeaderArea) {
				RightToLeft = IsRightToLeft
			};
			ObjectPainter.DrawObject(e.GraphicsCache, Data.ActiveLookAndFeel.Painter.GroupPanel, infoArgs);
		}
		protected internal virtual int FilterHeadersHeight {
			get {
				return FieldMeasures.GetHeaderAreaHeight(PivotArea.FilterArea);
			}
		}
		protected override void OnBeforeCalculating() {
			Bounds = ClientRectangle;
			Rectangle scrollableRectangle = PivotScrollableRectangle;
			CreateHeaders();
			this.rowAreaFields = CreateFieldsAreawViewInfo(false);
			this.columnAreaFields = CreateFieldsAreawViewInfo(true);
			this.cellsArea = CreateCellsViewInfo();
			if(IsPrefilterPanelVisible)
				this.prefilterPanel = CreatePrefilterPanelViewInfo();
			FilterHeaders.Y = scrollableRectangle.Top;
			RowAreaFields.X = FilterHeaders.X = DataHeaders.X = RowHeaders.X = scrollableRectangle.Left;
			AdjustPrefilterBounds();
			AdjustLoadingPanelBounds();
			AddChild(CellsArea);
			AddChild(ColumnAreaFields);
			AddChild(RowAreaFields);
			if(CanShowHeader(PivotArea.RowArea)) {
				AddChild(RowHeaders);
			}
			if(CanShowHeader(PivotArea.ColumnArea)) {
				AddChild(ColumnHeaders);
			}
			if(CanShowHeader(PivotArea.FilterArea)) {
				AddChild(FilterHeaders);
			}
			if(CanShowHeader(PivotArea.DataArea)) {
				AddChild(DataHeaders);
			}
			if(IsPrefilterPanelVisible)
				AddChild(PrefilterPanel);
			base.OnBeforeCalculating();
		}
		protected virtual void AdjustPrefilterBounds() {
			if(!IsPrefilterPanelVisible)
				return;
			PrefilterPanel.Bounds = PrefilterPanel.CalculateBounds(PivotScrollableRectangle);
		}
		protected void AdjustLoadingPanelBounds() {
			Rectangle bounds = new Rectangle();
			bounds.X = PivotScrollableRectangle.Width / 2 + PivotScrollableRectangle.X - 80 / 2;
			bounds.Y = PivotScrollableRectangle.Height / 2 + PivotScrollableRectangle.Y - 40 / 2;
			bounds.Width = 80;
			bounds.Height = 40;
			LoadingPanel.Bounds = bounds;
		}
		protected abstract PivotCellsViewInfoBase CreateCellsViewInfo();
		protected virtual PivotFieldsAreaViewInfoBase CreateFieldsAreawViewInfo(bool isColumn) {
			return new PivotFieldsAreaViewInfoBase(this, isColumn);
		}
		protected virtual PivotPrefilterPanelViewInfoBase CreatePrefilterPanelViewInfo() {
			return new PivotPrefilterPanelViewInfoBase(this);
		}
		protected override void OnAfterChildrenCalculated() {
			CellsArea.X = ColumnAreaFields.X = ColumnHeaders.X = RowAreaFields.Bounds.Right;
			SetFilterAndColumnHeadersWidth();
			RowHeaders.Width = DataHeaders.Width = RowAreaFields.Bounds.Width;
			FilterHeaders.Height = FilterHeadersHeight;
			FilterHeaders.CorrectHeadersHeight();
			RowHeaders.Height = FieldMeasures.GetHeaderAreaHeight(PivotArea.RowArea);
			ColumnHeaders.Height = FieldMeasures.GetHeaderAreaHeight(PivotArea.ColumnArea);
			DataHeaders.Height = FieldMeasures.GetHeaderAreaHeight(PivotArea.DataArea);
			for(int i = 0; i < HeaderCount; i++) {
				if(!CanShowHeader((PivotArea)i))
					GetHeader((PivotArea)i).Height = 0;
			}
			StretchRowHeadersHeight();
			DataHeaders.Y = ColumnHeaders.Y = FilterHeaders.ControlBounds.Bottom;
			if(DataHeaders.Bounds.Height + RowHeaders.Bounds.Height > ColumnHeaders.Bounds.Height + ColumnAreaFields.Bounds.Height)
				ColumnHeaders.Height = DataHeaders.Bounds.Height + RowHeaders.Bounds.Height - ColumnAreaFields.Bounds.Height;
			ColumnAreaFields.Y = ColumnHeaders.ControlBounds.Bottom;
			CellsArea.Y = RowAreaFields.Y = RowAreaFieldsAndCellAreaTop;
			CellsArea.Width = ColumnAreaFields.Bounds.Width;
			CellsArea.Height = RowAreaFields.Bounds.Height;
			RowHeaders.Y = ColumnAreaFields.Bounds.Bottom - RowHeaders.Bounds.Height;
			DataHeaders.CorrectHeadersWidth();
			RowHeaders.CorrectHeadersWidth();
			ColumnHeaders.CorrectHeadersWidth();
			SetLeftTopCoordCore(LeftTopCoord, false);
		}
		protected override void OnAfterCalculated() {
			base.OnAfterCalculated();
			scroller.Recalculate();
			CellsArea.CalculateCellsViewInfo();
		}
		protected virtual void OnVisualItemsCleared(object sender, EventArgs e) {
			Clear();
		}
		protected virtual void OnVisualItemsAfterCalculating(object sender, EventArgs e) {
			if(IsReady)
				Clear();
		}
		protected virtual void OnFieldSizeChanged(object sender, FieldSizeChangedEventArgs e) {
			Clear();
		}
		void StretchRowHeadersHeight() {
			if(RowHeaders.Bounds.Height >= ColumnAreaFields.Bounds.Height)
				return;
			int prevRowHeadersHeight = RowHeaders.Bounds.Height;
			RowHeaders.Height = ColumnAreaFields.Bounds.Height;
			int offset = RowHeaders.Bounds.Height - prevRowHeadersHeight;
			for(int i = 0; i < RowHeaders.ChildCount; i++)
				RowHeaders[i].Y += offset;
		}
		protected virtual int RowAreaFieldsAndCellAreaTop { get { return ColumnAreaFields.ControlBounds.Bottom; } }
		public void SetFilterAndColumnHeadersWidth() {
			if(IsHorzScrollControl)
				ColumnHeaders.Width = Math.Max(ColumnAreaFields.Bounds.Width, Bounds.Right - RowAreaFields.Bounds.Right);
			else
				ColumnHeaders.Width = ControlBounds.Width - ColumnHeaders.Bounds.Left;
			FilterHeaders.Width = ControlBounds.Width;
			FilterHeaders.X = -BoundsOffset.Width + ControlBounds.X;
		}
		protected void SetLeftTopCoordCore(Point value) {
			SetLeftTopCoordCore(value, true);
		}
		protected void SetLeftTopCoordCore(Point value, bool invalidate) {
			Point oldLeftTopCoord = LeftTopCoord;
			this.leftTopCoord = scroller.CorrectLeftTopCoord(value);
			scroller.SetLeftTopCoordOffset();
			if(!oldLeftTopCoord.Equals(LeftTopCoord)) {
				if(oldLeftTopCoord.X != LeftTopCoord.X) {
					ColumnAreaFields.ResetPaintBounds();
					if(IsHorzScrollControl) {
						RowAreaFields.ResetPaintBounds();
						CellsArea.ResetPaintBounds();
						RowHeaders.ResetPaintBounds();
						ColumnHeaders.ResetPaintBounds();
						DataHeaders.ResetPaintBounds();
					}
				}
				if(oldLeftTopCoord.Y != LeftTopCoord.Y)
					RowAreaFields.ResetPaintBounds();
				CellsArea.CalculateCellsViewInfo();
				if(invalidate) {
					Rectangle bounds = Bounds;
					bounds.Y -= ColumnAreaFields.Bounds.Height;
					bounds.Height += ColumnAreaFields.Bounds.Height;
					bounds.X -= RowAreaFields.Bounds.Width;
					bounds.Width += RowAreaFields.Bounds.Width;
					Invalidate(bounds);
				}
				Data.OnLeftTopCellChanged(oldLeftTopCoord, LeftTopCoord);
			}
		}
		public virtual PivotGridAppearancesBase PrintAndPaintAppearance { get { return Data.PaintAppearance; } }
		public PivotFieldsAreaViewInfoBase GetFieldsArea(bool isColumn) {
			return isColumn ? ColumnAreaFields : RowAreaFields;
		}
		#region dumps
		public virtual PivotFieldItem GetSizingField(int x, int y) { return null; }
		public virtual PivotGridViewInfoState State { get { return PivotGridViewInfoState.Normal; } }
		#endregion
	}
	public class PivotViewInfo : BaseViewInfo {
		internal static GlyphElementPainter CreateGlyphPainter(bool allowGlyphSkinning) {
			if(allowGlyphSkinning) {
				return new SkinnedGlyphElementPainter();
			} else {
				return new GlyphElementPainter();
			}
		}
		internal static GlyphElementInfoArgs CreateGlyphInfoArgs(object imageList, int imageIndex, bool allowGlyphSkinning) {
			if(allowGlyphSkinning) {
				return new SkinnedGlyphElementInfoArgs(imageList, imageIndex, null);
			} else {
				return new GlyphElementInfoArgs(imageList, imageIndex, null);
			}
		}
		PivotGridViewInfoBase viewInfo;
		public PivotViewInfo(PivotGridViewInfoBase viewInfo) {
			this.viewInfo = viewInfo;
		}
		public PivotGridViewInfoBase ViewInfo { get { return viewInfo; } }
		public PivotGridViewInfoData Data { get { return viewInfo.Data; } }
		public PivotFieldItemCollection FieldItems { get { return Data.FieldItems; } }
		public PivotVisualItems VisualItems { get { return Data.VisualItems; } }
		protected virtual Control GetControlOwner() { return null; }
		protected virtual IDXMenuManager GetMenuManager() { return null; }
		protected bool IsEnabled { get { return ViewInfo.IsEnabled; } }
		public override bool IsRightToLeft { get { return Data.IsRightToLeft; } }
		public virtual PivotFieldItem GetFieldAt(Point pt) { return null; }
		public virtual bool AcceptDragDrop { get { return false; } }
		public virtual Rectangle GetDragDrawRectangle(PivotFieldItem field, Point pt) { return Rectangle.Empty; }
		public virtual int GetNewFieldPosition(PivotFieldItem field, Point pt, out PivotArea area) {
			area = PivotArea.FilterArea;
			return -1;
		}
		public virtual void InvalidateHighLight() { }
		protected string GetLocalizedString(PivotGridStringId stringId) {
			return PivotGridLocalizer.GetString(stringId);
		}
		#region Menu
		[ThreadStatic]
		static ImageCollection menuImages = null;
		protected static ImageCollection MenuImages {
			get {
				if(menuImages == null)
					menuImages = DevExpress.Utils.Controls.ImageHelper.CreateImageCollectionFromResources("DevExpress.XtraPivotGrid.Images.popupmenuicons.png", typeof(PivotViewInfo).Assembly, new Size(16, 16));
				return menuImages;
			}
		}
		protected Point menuLocation = Point.Empty;
		protected DXPopupMenu menu = null;
		protected internal void ShowPopupMenu(MouseEventArgs e) {
			if(Data.IsDesignMode || Data.IsLockUpdate)
				return;
			this.menu = new DXPopupMenu();
			this.menu.CloseUp += menu_CloseUp;
			menuLocation = new Point(e.X, e.Y);
			CreatePopupMenuItems(this.menu);
			PivotGridMenuEventArgsBase menuEvent = CreateMenuEventArgs();
			if(!RaiseShowingMenu(menuEvent) && (menuEvent.Menu != null) && (menuEvent.Menu.Items.Count > 0)) {
				ShowMenuCore(menuEvent);
			}
			this.menu = null;
		}
		void menu_CloseUp(object sender, EventArgs e) {
			DXPopupMenu dxMenu = (sender as DXPopupMenu) ?? this.menu;
			if(dxMenu != null)
				dxMenu.CloseUp -= menu_CloseUp;
			Data.UserAction = UserAction.None;
		}
		protected void ShowMenuCore(PivotGridMenuEventArgsBase menuEvent) {
			Data.UserAction = UserAction.MenuOpen;
			MenuManagerHelper.ShowMenu(menuEvent.Menu, ((PivotGridViewInfoData)Data).ActiveLookAndFeel, GetMenuManager(), GetControlOwner(), menuLocation);
		}
		protected virtual PivotGridMenuType MenuType { get { return PivotGridMenuType.Header; } }
		protected virtual PivotFieldItem MenuField { get { return null; } }
		protected virtual PivotArea MenuArea { get { return MenuField != null ? MenuField.Area : PivotArea.DataArea; } }
		protected virtual void CreatePopupMenuItems(DXPopupMenu menu) { }
#if DEBUGTEST
		internal DXPopupMenu CreatePopupMenuItemsTest() {
			this.menu = new DXPopupMenu();
			CreatePopupMenuItems(menu);
			return menu;
		}
#endif
		protected virtual void OnMenuItemClick(DXMenuItem menuItem) {
			switch((int)menuItem.Tag) {
				case PivotContextMenuIds.ReloadDataMenuID:
					Data.ReloadDataAsync(false);
					break;
				case PivotContextMenuIds.ShowHideFieldListMenuID:
					Data.ChangeFieldsCustomizationVisible();
					break;
				case PivotContextMenuIds.ShowPrefilterMenuID:
					Data.ChangePrefilterVisible();
					break;
			}
		}
		protected void AddPopupMenuRefresh() {
			menu.Items.Add(CreateMenuItem(GetLocalizedString(PivotGridStringId.PopupMenuRefreshData), PivotContextMenuIds.ReloadDataMenuID, 0));
		}
		protected void AddPopupMenuFieldCustomization() {
			if(!Data.OptionsCustomization.AllowCustomizationForm)
				return;
			menu.Items.Add(CreateMenuItem(GetLocalizedString(Data.IsFieldCustomizationShowing ?
												PivotGridStringId.PopupMenuHideFieldList :
												PivotGridStringId.PopupMenuShowFieldList),
											PivotContextMenuIds.ShowHideFieldListMenuID, 1));
			SetBeginGrouptoLastMenuItem();
		}
		protected void AddPopupMenuPrefilter() {
			if(!Data.OptionsCustomization.AllowPrefilter ||
				!Data.IsCapabilitySupported(PivotDataSourceCaps.Prefilter))
				return;
			menu.Items.Add(CreateMenuItem(GetLocalizedString(Data.IsPrefilterFormShowing ?
											   PivotGridStringId.PopupMenuHidePrefilter :
											   PivotGridStringId.PopupMenuShowPrefilter),
										  PivotContextMenuIds.ShowPrefilterMenuID, 2));
		}
		protected void SetBeginGrouptoLastMenuItem() {
			if(menu.Items.Count == 1)
				return;
			menu.Items[menu.Items.Count - 1].BeginGroup = true;
		}
		protected DXMenuItem CreateMenuItem(string caption, object tag) {
			return CreateMenuItem(caption, tag, true);
		}
		protected DXMenuItem CreateMenuItem(string caption, object tag, bool enabled) {
			return CreateMenuItem(caption, tag, enabled, -1, false);
		}
		protected DXMenuItem CreateMenuItem(string caption, object tag, int imageIndex) {
			return CreateMenuItem(caption, tag, true, imageIndex, false);
		}
		protected DXMenuItem CreateMenuItem(string caption, object tag, bool enabled, int imageIndex, bool beginGroup) {
			DXMenuItem item = new DXMenuItem(caption, new EventHandler(OnMenu_Click));
			item.Tag = tag;
			item.Enabled = enabled;
			item.BeginGroup = beginGroup;
			if(imageIndex >= 0) {
				item.Image = MenuImages.Images[imageIndex];
			}
			return item;
		}
		protected DXMenuCheckItem CreateMenuCheckItem(string caption, bool check, object tag, bool beginGroup) {
			DXMenuCheckItem item = new DXMenuCheckItem(caption, check);
			item.CheckedChanged += new EventHandler(OnMenu_Click);
			item.Tag = tag;
			item.BeginGroup = beginGroup;
			return item;
		}
		protected DXMenuCheckItem CreateMenuCheckItem(string caption, bool check, object tag, int imageIndex) {
			DXMenuCheckItem item = CreateMenuCheckItem(caption, check, tag, false);
			item.Image = MenuImages.Images[imageIndex];
			return item;
		}
		protected bool RaiseShowingMenu(PivotGridMenuEventArgsBase e) {
			Data.OnPopupMenuShowing(e);
			menuLocation = e.Point;
			return !e.Allow;
		}
		protected bool RaiseMenuClick(DXMenuItem menuItem) {
			PivotGridMenuItemClickEventArgsBase e = CreateMenuItemClickEventArgs(menuItem);
			Data.OnPopupMenuItemClick(e);
			return !e.Allow;
		}
		void OnMenu_Click(object sender, EventArgs e) {
			if(RaiseMenuClick((DXMenuItem)sender))
				return;
			OnMenuItemClick((DXMenuItem)sender);
		}
		protected virtual PivotGridMenuEventArgsBase CreateMenuEventArgs() {
			return Data.EventArgsHelper.CreateMenuEventArgsBase(menu, MenuType, Data.GetField(MenuField), MenuArea, menuLocation);
		}
		protected virtual PivotGridMenuItemClickEventArgsBase CreateMenuItemClickEventArgs(DXMenuItem menuItem) {
			return Data.EventArgsHelper.CreateMenuItemClickEventArgsBase(menu, MenuType, Data.GetField(MenuField), MenuArea, menuLocation, menuItem);
		}
		#endregion
		public virtual PivotGridHitInfo CalcHitInfo(Point hitPoint) { return new PivotGridHitInfo(hitPoint); }
		public virtual ToolTipControlInfo GetToolTipObjectInfo(Point pt) { return null; }
#if DEBUGTEST
		public void OnMenuItemClickAccess(DXMenuItem menuItem) {
			OnMenuItemClick(menuItem);
		}
#endif
	}
	class PivotGridBestFitter : PivotGridBestFitterBase {
		PivotGridViewInfoBase viewInfo;
		protected override ICellsBestFitProvider CellsArea { get { return viewInfo.CellsArea; } }
		protected override IFieldValueAreaBestFitProvider RowAreaFields { get { return viewInfo.RowAreaFields; } }
		protected override IFieldValueAreaBestFitProvider ColumnAreaFields { get { return viewInfo.ColumnAreaFields; } }
		PivotFieldItemCollection FieldItems {
			get { return viewInfo.FieldItems; }
		}
		public PivotGridBestFitter(PivotGridViewInfoBase viewInfo)
			: base(viewInfo.Data) {
			this.viewInfo = viewInfo;
		}
		public override void BeginBestFit() {
			viewInfo.EnsureIsCalculated();
			base.BeginBestFit();
		}
		protected override PivotGridCellItem CreatCellItem(int rowIndex, int columnIndex) {
			return viewInfo.CellsArea.CreateCellViewInfo(columnIndex, rowIndex);
		}
	}
}
