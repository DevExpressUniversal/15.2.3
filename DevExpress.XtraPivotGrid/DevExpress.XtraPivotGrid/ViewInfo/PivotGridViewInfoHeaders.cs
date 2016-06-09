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
using DevExpress.Data.PivotGrid;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.Utils.Menu;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Drawing;
using DevExpress.XtraPivotGrid.Customization;
using DevExpress.XtraPivotGrid.Data;
using DevExpress.XtraPivotGrid.FilterDropDown;
using DevExpress.XtraPivotGrid.FilterPopup.SummaryFilter;
using DevExpress.XtraPivotGrid.Localization;
namespace DevExpress.XtraPivotGrid.ViewInfo {
	public class PivotHeadersViewInfo : PivotHeadersViewInfoBase {
		public PivotHeadersViewInfo(PivotGridViewInfo viewInfo, PivotArea area)
			: base(viewInfo, area) { }
		public new PivotGridViewInfo ViewInfo { get { return (PivotGridViewInfo)base.ViewInfo; } }
		public new PivotGridViewInfoData Data { get { return ViewInfo.Data; } }
		protected override Control GetControlOwner() {
			return Data.ControlOwner;
		}
		protected override IDXMenuManager GetMenuManager() {
			return Data.MenuManager;
		}
		protected override PivotGridMenuEventArgsBase CreateMenuEventArgs() {
#pragma warning disable 612 // Obsolete
#pragma warning disable 618 // Obsolete
			return Data.EventArgsHelper.CreateMenuEventArgs(menu, MenuType, (PivotGridField)Data.GetField(MenuField), MenuArea, menuLocation);
#pragma warning restore 618 // Obsolete
#pragma warning restore 612 // Obsolete
		}
		protected override PivotGridMenuItemClickEventArgsBase CreateMenuItemClickEventArgs(DXMenuItem menuItem) {
			return Data.EventArgsHelper.CreateGridMenuItemClickEventArgs(this.menu, MenuType, (PivotGridField)Data.GetField(MenuField), MenuArea, menuLocation, menuItem);
		}
		public new PivotHeaderViewInfo this[int index] { get { return (PivotHeaderViewInfo)base[index]; } }
		protected override PivotHeaderViewInfoBase CreateHeaderViewInfo(PivotFieldItem field) {
			return new PivotHeaderViewInfo(ViewInfo, field as PivotFieldItem);
		}
		public override ToolTipControlInfo GetToolTipObjectInfo(Point pt) {
			if(!Data.OptionsHint.ShowHeaderHints) return null;
			if (ChildCount == 0) {
				return GetHeaderAreaToolTip();
			}
			for(int i = 0; i < ChildCount; i++) {
				if(this[i].PaintBounds.Contains(pt)) {
					return this[i].GetToolTipObjectInfo(pt);
				}
			}
			return null;
		}
		ToolTipControlInfo GetHeaderAreaToolTip() {
			Rectangle textRectangle = this.GetTextRectangle();
			Graphics graphics = GraphicsInfo.Default.AddGraphics(null);
			try {
				Size size = graphics.MeasureString(this.CustomizeText, this.Appearance.Font, textRectangle.Width - 1, StringFormat.GenericTypographic).ToSize();
				bool isFit = textRectangle.Contains(new Rectangle(textRectangle.Location, size));
				return !isFit ? new ToolTipControlInfo(this.Area, this.CustomizeText) : null;
			}
			finally {
				GraphicsInfo.Default.ReleaseGraphics();
			}
		}
		public override void InvalidateHighLight() {
			if(Data.ControlOwner == null) return;
			Graphics graphics = GraphicsInfo.Default.AddGraphics(null);
			try {
				this.Paint(Data.ControlOwner, new PaintEventArgs(graphics, ControlBounds));
			} finally {
				GraphicsInfo.Default.ReleaseGraphics();
			}
		}
		public void CloseFilterPopups() {
			for(int i = 0; i < ChildCount; i++)
				this[i].CloseFilterPopup();
		}
		protected override BaseViewInfo MouseDownCore(MouseEventArgs e) {
			if(e.Button == MouseButtons.Right && Data.OptionsMenu.EnableHeaderAreaMenu) {
				ShowPopupMenu(e);
			}
			return null;
		}
		protected override PivotGridMenuType MenuType { get { return PivotGridMenuType.HeaderArea; } }
		protected override PivotArea MenuArea { get { return Area; } }
		protected override void CreatePopupMenuItems(DXPopupMenu menu) {
			AddPopupMenuRefresh();
			AddPopupMenuFieldCustomization();
			AddPopupMenuPrefilter();
		}		
		protected override Rectangle GetDrawRectangle() {
			Rectangle drawBounds = PaintBounds;
			if(drawBounds.Right > ViewInfo.PivotScrollableRectangle.Right)
				drawBounds.Width = ViewInfo.PivotScrollableRectangle.Right - drawBounds.X;
			return drawBounds;
		}
	}
	public class PivotHeadersViewInfoBase : PivotViewInfo, IPivotCustomDrawAppearanceOwner {
		PivotArea area;
		AppearanceObject appearance;
		public PivotHeadersViewInfoBase(PivotGridViewInfoBase viewInfo, PivotArea area)
			: base(viewInfo) {
			this.area = area;
			this.appearance = new AppearanceObject();
			AppearanceHelper.Combine(this.appearance, new AppearanceObject[] {DefaultAppearance, Data.PaintAppearance.HeaderArea});
		}
		public PivotArea Area { get { return area; } }
		public new PivotHeaderViewInfoBase this[int index] { get { return (PivotHeaderViewInfoBase)base[index]; } }
		public PivotHeaderViewInfoBase this[PivotFieldItemBase field] {
			get {
				if(field == null)
					return null;
				for(int i = 0; i < ChildCount; i++) {
					if(field.Equals(this[i].Field))
						return this[i];
				}
				return null;
			}
		}
		public override PivotFieldItem GetFieldAt(Point pt) {
			if(!PaintBounds.Contains(pt)) return null;
			for(int i = 0; i < ChildCount; i ++) {
				if(this[i].PaintBounds.Contains(pt))
					return this[i].Field;
			}
			return null;
		}
		public override bool AcceptDragDrop { get { return true; } }
		public override PivotGridHitInfo CalcHitInfo(Point hitPoint) {
			PivotHeaderViewInfoBase viewInfo = GetViewInfoAtPoint(hitPoint, false) as PivotHeaderViewInfoBase;
			return viewInfo != null ? viewInfo.CalcHitInfo(hitPoint) : new PivotGridHitInfo(this, null, PivotGridHeaderHitTest.None,  hitPoint);
		}
		public override Rectangle GetDragDrawRectangle(PivotFieldItem field, Point pt) { 
			if(!PaintBounds.Contains(pt)) return Rectangle.Empty;
			bool isBefore;
			PivotHeaderViewInfoBase header = GetDragHeaderInfo(pt, out isBefore);
			if(header != null) {
				if(CheckDragHeader(header.Field, field, isBefore))
					return Rectangle.Empty;
				return GetDragDrawRectangle(header, isBefore);
			}
			Rectangle bounds = PaintBounds;
			bounds.Inflate(0, -HeaderHeightOffset);
			bounds.X += HeaderWidthOffset;
			bounds.Width = FieldMeasures.DefaultFieldWidth / 2;
			return bounds;
		}
		public override int GetNewFieldPosition(PivotFieldItem field, Point pt, out PivotArea area) {
			area = Area;
			if(!PaintBounds.Contains(pt)) return -1;
			bool isBefore;
			PivotHeaderViewInfoBase header = GetDragHeaderInfo(pt, out isBefore);
			if(header == null) return 0;
			if(CheckDragHeader(header.Field, field, isBefore)) return -1;
			int oldAreaIndexWithData = 0, targetAreaIndexWithData = 0;
			GetFieldIndexes(field, header.Field, out oldAreaIndexWithData, out targetAreaIndexWithData);
			if(IsRightToLeft)
				isBefore = !isBefore;
			targetAreaIndexWithData = isBefore ? targetAreaIndexWithData : targetAreaIndexWithData + 1;
			if(oldAreaIndexWithData == targetAreaIndexWithData && field.Area == Area)
				return -1;
			return targetAreaIndexWithData;
		}
		bool CheckDragHeader(PivotFieldItem headerField, PivotFieldItem field, bool isBefore) {
			if(field.Equals(headerField)) return true;
			int index = headerField.InnerGroupIndex;
			return (headerField.Group != null && headerField.Group.VisibleCount > 1)
				&& ((isBefore && index != 0) || (!isBefore && index < headerField.Group.VisibleCount - 1));
		}
		void GetFieldIndexes(PivotFieldItem field, PivotFieldItem tagetField, out int oldFullIndex, out int newFullIndex) {
			List<PivotFieldItemBase> fields = FieldItems.GetFieldItemsByArea(area, true);
			oldFullIndex = fields.IndexOf(field);
			fields.Remove(field);
			newFullIndex = fields.IndexOf(tagetField);
		}
		public void CorrectHeadersHeight() {
			if(Area != PivotArea.FilterArea) return;
			int headersWidth = Bounds.Width < ViewInfo.Bounds.Width ? Bounds.Width : ViewInfo.Bounds.Width;
			if(!ShouldPerformWordWrap(headersWidth)) 
				return;
			int right = HeaderWidthOffset;
			int top = HeaderHeightOffset;
			int headerCount = 0;
			int index = 0;
			while(index < ChildCount) {
				int width = GetFieldGroupWidth(index);
				if(headerCount > 0 && (right + width) >= headersWidth) {
					Height += FieldMeasures.DefaultHeaderHeight + HeaderHeightOffset;
					top += FieldMeasures.DefaultHeaderHeight + HeaderHeightOffset;
					right = HeaderWidthOffset;
					headerCount = 0;
				}
				SetFieldGroupLocation(index, new Point(right, top));
				right += GetFieldGroupWidth(index);
				index += GetFieldGroupCount(index);
				headerCount += GetFieldGroupCount(index);
			}
		}
		bool ShouldPerformWordWrap(int headersWidth) {
			return ChildCount >= 2 && 
				(this[ChildCount - 1].Bounds.Right >= headersWidth || this[ChildCount - 1].Bounds.Bottom >= Bounds.Height);
		}
		int GetFieldGroupCount(int index) {
			if(index >= ChildCount) return 0;
			return this[index].Field.Group != null ? this[index].Field.Group.VisibleCount : 1;
		}
		int GetFieldGroupWidth(int index) {
			int visibleCount = GetFieldGroupCount(index);
			int width = 0;
			for(int i = 0; i < visibleCount; i ++)
				width += this[index + i].Bounds.Width + HeaderWidthOffset;
			return width;
		}
		void SetFieldGroupLocation(int index, Point location) {
			int visibleCount = GetFieldGroupCount(index);
			for(int i = 0; i < visibleCount; i ++) {
				this[index + i].Location = location;
				location.X += this[index + i].Bounds.Width + HeaderWidthOffset;
			}
		}
		bool IsHeaderWidthCorrectionNeeded {
			get {
				return Area == PivotArea.DataArea || Area == PivotArea.ColumnArea ||
					(Area == PivotArea.RowArea && Data.OptionsView.RowTotalsLocation == PivotRowTotalsLocation.Tree);
			}
		}
		public void CorrectHeadersWidth() {
			if(!IsHeaderWidthCorrectionNeeded)
				return;
			if(ChildCount == 0) return;
			if(Area == PivotArea.ColumnArea || (Area == PivotArea.RowArea && Data.OptionsView.RowTotalsLocation == PivotRowTotalsLocation.Tree)) {
				SetChildsBestWidth();
			}
			if(this[ChildCount - 1].Bounds.Right >= Bounds.Width) {
				FitHeadersIntoParent();
				return;
			}
		}
		void FitHeadersIntoParent() {
			int averageWidth = (Bounds.Width - (ChildCount + 1) * HeaderWidthOffset) / ChildCount;
			int noNeedCorrectionWidth = 0;
			int noNeedCorrectionCount = 0;
			for(int i = 0; i < ChildCount; i++) {
				if(this[i].Bounds.Width < averageWidth) {
					noNeedCorrectionWidth += this[i].Bounds.Width;
					noNeedCorrectionCount++;
				}
			}
			averageWidth = (Bounds.Width - (ChildCount + 1) * HeaderWidthOffset - noNeedCorrectionWidth) / (ChildCount - noNeedCorrectionCount);
			for(int i = 0; i < ChildCount; i++) {
				if(this[i].Bounds.Width > averageWidth)
					this[i].Width = averageWidth;
			}
			UpdateChildXLocation();
		}
		void SetChildsBestWidth() {
			for(int i = 0; i < ChildCount; i++)
				this[i].SetBestWidth();
			UpdateChildXLocation();
		}
		public int GetBestWidth(PivotFieldItem field) {
			PivotHeaderViewInfoBase header = GetHeader(field);
			return header == null ? 0 : header.BestWidth;
		}
		public int GetWidthOffset(PivotFieldItem field) {
			PivotHeaderViewInfoBase header = GetHeader(field);
			return header == null ? 0 : header.WidthOffset;
		}
		public string CustomizeText {
			get { return PivotGridLocalizer.GetHeadersAreaText((int)Area); }
		}		
		protected int HeaderWidthOffset { get { return FieldMeasures.HeaderWidthOffset; } }
		protected int HeaderHeightOffset { get { return FieldMeasures.HeaderHeightOffset; } }
		protected int FirstLastHeaderWidthOffset { get { return ViewInfo.FirstLastHeaderWidthOffset; } }
		bool PointIsLessThenFarBorder(int x, Rectangle rect) { 
			return IsRightToLeft ? x > rect.Left : x < rect.Right;
		}
		PivotHeaderViewInfoBase GetDragHeaderInfo(Point pt, out bool isBefore) {
			isBefore = IsRightToLeft;
			if(ChildCount == 0 || (pt.Y <= PaintBounds.Top && pt.Y >= PaintBounds.Bottom)) return null;
			for(int i = 0; i < ChildCount; i++) {
				Rectangle bounds = this[i].PaintBounds;
				if(pt.Y >= bounds.Top && pt.Y <= bounds.Bottom) {
					if(PointIsLessThenFarBorder(pt.X, bounds)) {
						if(pt.X - bounds.Left < bounds.Right - pt.X)
							isBefore = true;
						else
							isBefore = false;
						return this[i];
					}
				}
			}
			return this[ChildCount - 1];
		}
		PivotHeaderViewInfoBase GetHeader(PivotFieldItem field) {
			PivotHeaderViewInfoBase header = field.AreaIndex < ChildCount ? this[field.AreaIndex] : null;
			if(header == null || !field.Equals(header.Field))
				header = this[(PivotFieldItemBase)field];
			return header;
		}
		Rectangle GetDragDrawRectangle(PivotHeaderViewInfoBase headerViewInfo, bool isBefore) {
			Rectangle bounds = headerViewInfo.PaintBounds;
			if(isBefore)
				return new Rectangle(bounds.Left - HeaderWidthOffset, bounds.Y, bounds.Width / 2, bounds.Height);
			else return new Rectangle(bounds.Left + bounds.Width / 2 + HeaderWidthOffset, bounds.Y, bounds.Width / 2, bounds.Height);
		}
		protected virtual PivotFieldItemBase[] GetPivotFields() {
			List<PivotFieldItemBase> baseFields = FieldItems.GetFieldItemsByArea(Area, true);
			List<PivotFieldItemBase> fields = new List<PivotFieldItemBase>();
			for(int i = 0; i < baseFields.Count; i++)
				fields.Add(baseFields[i]);
			return fields.ToArray();
		}
		protected override void OnBeforeCalculating() {
			PivotFieldItemBase[] fields = GetPivotFields();
			for(int i = 0; i < fields.Length; i++) {
				PivotHeaderViewInfoBase header = CreateHeaderViewInfo(fields[i] as PivotFieldItem);
				AddChild(header);
				header.Y = HeaderHeightOffset;
				header.Size = new Size(fields[i].Width - header.WidthOffset, FieldMeasures.GetHeaderHeight(fields[i]));
				header.UpdateCaption();
			}
		}
		protected virtual PivotHeaderViewInfoBase CreateHeaderViewInfo(PivotFieldItem field) {
			return new PivotHeaderViewInfoBase(ViewInfo, field);
		}
		protected override void OnAfterCalculated() {
			UpdateChildXLocation();
		}
		void UpdateChildXLocation() {
			int x = Area == PivotArea.ColumnArea ? 0 : HeaderWidthOffset;
			for(int i = 0; i < ChildCount; i++) {
				this[i].X = x;
				x += this[i].Bounds.Width + HeaderWidthOffset;
			}
		}
		public AppearanceObject Appearance { 
			get { return appearance; } 
			set {
				if(value == null) return;
				appearance = value;
			}
		}
		protected AppearanceObject DefaultAppearance {
			get {
				if(Area == PivotArea.ColumnArea)
					return Data.PaintAppearance.ColumnHeaderArea;
				if(Area == PivotArea.RowArea)
					return Data.PaintAppearance.RowHeaderArea;
				if(Area == PivotArea.DataArea)
					return Data.PaintAppearance.DataHeaderArea;
				return Data.PaintAppearance.FilterHeaderArea;
			}
		}
		protected bool IsNeedFillBackground {
			get {
				AppearanceObject areaAppearance = Data.Appearance.HeaderArea;
				AppearanceObject defaultAppearance = DefaultAppearance;
				return areaAppearance.BackColor != defaultAppearance.BackColor || 
					areaAppearance.BackColor2 != defaultAppearance.BackColor2 ||
					areaAppearance.GradientMode != defaultAppearance.GradientMode;
			}
		}		
		protected virtual Rectangle GetDrawRectangle() {
			Rectangle drawBounds = PaintBounds;
			return drawBounds;
		}
		protected virtual Rectangle GetTextRectangle() {
			Rectangle bounds = PaintBounds;
			bounds.Inflate(-HeaderWidthOffset - 1, 0);
			return bounds;
		}
		protected virtual void DrawSeparator(ViewInfoPaintArgs e) {
			if(Area != PivotArea.FilterArea || !Data.OptionsView.ShowFilterSeparatorBar) return;
			Rectangle lineBounds = GetDrawRectangle();
			lineBounds.Y = lineBounds.Bottom - 1;		
			lineBounds.Height = 1;
			lineBounds.Width = lineBounds.Right - lineBounds.X;
			Data.PaintAppearance.FilterSeparator.FillRectangle(e.GraphicsCache, lineBounds);
		}
		protected virtual void DrawGroupLines(ViewInfoPaintArgs e) {
			for(int i = 0; i < ChildCount - 1; i ++) {
				if(this[i].Field.IsNextVisibleFieldInSameGroup) {
					DrawGroupLine(e, this[i]);
				}
			}
		}
		protected virtual void DrawGroupLine(ViewInfoPaintArgs e, PivotHeaderViewInfoBase headerViewInfo) {
			int x = headerViewInfo.ControlBounds.Right;
			int y = headerViewInfo.ControlBounds.Top + headerViewInfo.Bounds.Height / 2;
			Data.PaintAppearance.HeaderGroupLine.FillRectangle(e.GraphicsCache, RightToLeftRect(new Rectangle(x, y, HeaderWidthOffset, 1)));
		}
		protected override void InternalPaint(ViewInfoPaintArgs e) {
			 MethodInvoker defaultDraw = () => {
				if(IsNeedFillBackground)
					Appearance.FillRectangle(e.GraphicsCache, GetDrawRectangle());
				if(ChildCount == 0) 
					DrawCustomizeText(e);
				DrawSeparator(e);
				DrawGroupLines(e);
			 };
			if(!ViewInfo.DrawHeaderArea(this, e, GetDrawRectangle(), defaultDraw))
				defaultDraw();
		}
		protected override void AfterPaint(ViewInfoPaintArgs e) {
			if(ViewInfo.HighLightedArea == this) {
				Color highLightColor = Color.FromArgb(75, SystemColors.Highlight);
				Brush brush = new SolidBrush(highLightColor);
				e.Graphics.FillRectangle(brush, GetDrawRectangle());
				brush.Dispose();
			}
		}
		protected virtual void DrawCustomizeText(ViewInfoPaintArgs e) {
			StringFormat format = new StringFormat();
			format.Alignment = IsRightToLeft ? StringAlignment.Far : StringAlignment.Near; 
			format.LineAlignment = StringAlignment.Center;
			Appearance.DrawString(e.GraphicsCache, CustomizeText, GetTextRectangle(), format);
		}
	}
	public class PivotHeaderViewInfo : PivotHeaderViewInfoBase, IPivotGridDropDownFilterEditOwner {
		bool hotTrackFilterButton;
		bool isFilterDown;
		Point mouseDownLocation;
		protected override bool IsFilterDown { get { return isFilterDown; } }
		public PivotHeaderViewInfo(PivotGridViewInfo viewInfo, PivotFieldItem field)
			: this(viewInfo, field, true) {
		}
		protected PivotHeaderViewInfo(PivotGridViewInfo viewInfo, PivotFieldItem field, bool initialize)
			: base(viewInfo, field, initialize) {
			this.hotTrackFilterButton = false;
			this.isFilterDown = false;
			ResetMouseDownLocation();
		}
		public new PivotHeadersViewInfo HeadersViewInfo { get { return (PivotHeadersViewInfo)Parent; } }
		public new PivotGridViewInfo ViewInfo { get { return (PivotGridViewInfo)base.ViewInfo; } }
		public new PivotGridViewInfoData Data { get { return ViewInfo.Data; } }
		protected virtual bool DeferUpdates { get { return false; } }
		protected override Control GetControlOwner() {
			return Data.ControlOwner;
		}
		protected override IDXMenuManager GetMenuManager() {
			return Data.MenuManager;
		}
#pragma warning disable 618 // Obsolete
#pragma warning disable 612 // Obsolete
		protected override PivotGridMenuEventArgsBase CreateMenuEventArgs() {
			return Data.EventArgsHelper.CreateMenuEventArgs(menu, MenuType, (PivotGridField)Data.GetField(MenuField), MenuArea, menuLocation);
		}
		protected virtual PivotGridMenuEventArgs CreateSummariesMenuEventArgs() {
			return Data.EventArgsHelper.CreateMenuEventArgs(this.menu, PivotGridMenuType.HeaderSummaries,
							(PivotGridField)Data.GetField(MenuField), MenuArea, this.menuLocation);
		}
#pragma warning restore 618 // Obsolete
#pragma warning restore 612 // Obsolete
		protected override PivotGridMenuItemClickEventArgsBase CreateMenuItemClickEventArgs(DXMenuItem menuItem) {
			return Data.EventArgsHelper.CreateGridMenuItemClickEventArgs(this.menu, MenuType, (PivotGridField)Data.GetField(MenuField), MenuArea, menuLocation, menuItem);
		}
		public bool IsDragging { get { return ViewInfo.IsDragging; } }
		protected override bool HotTrackFilterButton {
			get { return hotTrackFilterButton; }
			set {
				if(HotTrackFilterButton == value || FilterInfo == null) return;
				if(value && OriginalField.IsDesignTime)
					return;
				this.hotTrackFilterButton = value;
				Data.Invalidate(ControlBounds);
			}
		}
		protected internal PivotHeaderViewInfo DraggingHeader {
			get {
				if(OriginalField.Group == null || HeadersViewInfo == null) return this;
				if(OriginalField.Group[0] == OriginalField) return this;
				return HeadersViewInfo[OriginalField.Group.AreaIndex];
			}
		}
		protected override void MouseUpCore(MouseEventArgs e) {
			ResetMouseDownLocation();
			if(e.Button != MouseButtons.Left)
				return;
			if(OriginalField.IsDesignTime) {
				OriginalField.SelectedAtDesignTime = true;
				Data.Invalidate();
				return;
			}
			if(isFilterDown) {
				if(IsMouseOnFilterButton(new Point(e.X, e.Y)))
					ShowFilterPopup();
				else isFilterDown = false;
			}
			if (!isFilterDown && !IsDragging && IsClickAllowed(e)) {
				if(OriginalField.Area != PivotArea.DataArea && OriginalField.Area != PivotArea.FilterArea && CanSort)
					Data.ChangeFieldSortOrderAsync(OriginalField, false);
				else if(AllowRunTimeSummaryChange)
					ShowSummariesMenu();
			}
			Invalidate();
		}
		protected virtual bool IsClickAllowed(MouseEventArgs e) {
			return ViewInfo.GetFieldAt(e.Location) == Field;
		}
		protected virtual bool CanSort {
			get { return Field.CanSortHeader ; }
		}
		protected void ResetMouseDownLocation() {
			this.mouseDownLocation = Point.Empty;
		}
		PivotDropDownFilterEditBase dropDown;
		public PivotDropDownFilterEditBase FilterDropDown { get { return dropDown; } }
		public void ShowFilterPopup() {
			Data.UserAction = UserAction.FieldFilter;
			Rectangle bounds = ControlBounds;
			if(!FilterInfo.Bounds.IsEmpty && IsFilterSmartTagStyle) bounds = FilterInfo.Bounds;
			if(dropDown != null)
				dropDown.Close();
			dropDown = CreateDropDownFilterEdit(bounds);
			dropDown.Show();
			isFilterDown = true;
		}
		public void CloseFilterPopup() {
			if(dropDown != null)
				dropDown.Close();
			isFilterDown = false;
			Data.UserAction = UserAction.None;
		}
		protected virtual PivotDropDownFilterEditBase CreateDropDownFilterEdit(Rectangle bounds) {
			if(OriginalField.Area != PivotArea.DataArea)
				return CreateDropDownRegularFilterEdit(GetControlOwner(), Data, OriginalField, bounds, DeferUpdates);
			else
				return CreateDropDownSummaryFilterEdit(GetControlOwner(), Data, OriginalField, bounds);
		}
		protected virtual PivotGridDropDownFilterEdit CreateDropDownRegularFilterEdit(Control controlOwner, PivotGridViewInfoData data, PivotGridField originalField, Rectangle bounds, bool deferUpdates) {
			return new PivotGridDropDownFilterEdit(this, controlOwner, data, originalField, bounds, deferUpdates);
		}
		protected virtual PivotDropDownSummaryFilterEdit CreateDropDownSummaryFilterEdit(Control controlOwner, PivotGridViewInfoData data, PivotGridField originalField, Rectangle bounds) {
			return new PivotDropDownSummaryFilterEdit(this, controlOwner, data, originalField, bounds);
		}
#pragma warning disable 618 // Obsolete
#pragma warning disable 612 // Obsolete
		protected void ShowSummariesMenu() {
			if(Data.IsLockUpdate)
				return;
			this.menu = new DXPopupMenu();
			if(Parent != null)
				this.menuLocation = new Point(Parent.Bounds.Left + Bounds.Left, Parent.Bounds.Top + Bounds.Bottom + 1);
			else
				this.menuLocation = new Point(Bounds.Left, Bounds.Bottom + 1);
			CreateSummaryTypesMenuItems(this.menu);
			PivotGridMenuEventArgs menuEvent = CreateSummariesMenuEventArgs();
			if(!RaiseShowingMenu(menuEvent) && (menuEvent.Menu != null) && (menuEvent.Menu.Items.Count > 0)) {
				ShowMenuCore(menuEvent);
			}
			this.menu = null;
		}
#pragma warning restore 618 // Obsolete
#pragma warning restore 612 // Obsolete
		bool IsMouseOnFilterButton(Point pt) {
			return FilterInfo != null && FilterInfo.Bounds.Contains(pt);
		}
#if DEBUGTEST
		internal void AccessMouseMove(MouseEventArgs e) {
			MouseMoveCore(e);
		}
		internal void AccessCollapseButtonMouseDown() {
			Rectangle bounds = new Rectangle(0, 0, 10, 10);
			OpenCloseButtonInfo.Bounds = bounds;
			MouseEventArgs e = new MouseEventArgs(MouseButtons.Left, 1, bounds.X + bounds.Width / 2, bounds.Y + bounds.Height / 2, 0);
			MouseDownCore(e);
		}
#endif
		protected override void MouseMoveCore(MouseEventArgs e) {
			if(IsDragging) return;
			if(!this.mouseDownLocation.IsEmpty && CanDrag && !isFilterDown) {
				Size dragSize = System.Windows.Forms.SystemInformation.DragSize;
				if(Math.Abs(this.mouseDownLocation.X - e.X) >= dragSize.Width
					|| Math.Abs(this.mouseDownLocation.Y - e.Y) >= dragSize.Height) {
					PivotHeaderViewInfo dragHeader = DraggingHeader;
					ViewInfo.StartDragging(dragHeader.Field, delegate() {
						MouseEventArgs mouseEventArgs = new MouseEventArgs(MouseButtons.Left, 1, 0, 0, 0);
						dragHeader.MouseUp(mouseEventArgs);
					});
				}
			}
			HotTrackFilterButton = FilterInfo != null ? FilterInfo.Bounds.Contains(new Point(e.X, e.Y)) : false;
		}
		protected override void MouseEnterCore() {
			Invalidate();
		}
		protected override void MouseLeaveCore() {
			this.hotTrackFilterButton = false;
			Invalidate();
		}
		protected override BaseViewInfo MouseDownCore(MouseEventArgs e) {
			if(e.Button == MouseButtons.Right && Data.OptionsMenu.EnableHeaderMenu) {
				ResetMouseDownLocation();
				ShowPopupMenu(e);
			}
			if(e.Button != MouseButtons.Left) return null;
			if(OpenCloseButtonInfo != null && OpenCloseButtonInfo.Bounds.Contains(e.X, e.Y)) {
				Data.ChangeFieldExpandedInFieldsGroupAsync(OriginalField, !Field.ExpandedInFieldsGroup, false);
				return null;
			}
			this.mouseDownLocation = new Point(e.X, e.Y);
			if(!OriginalField.IsDesignTime && IsMouseOnFilterButton(new Point(e.X, e.Y)) && CanShowFilterPopup) {
				isFilterDown = true;
			}
			Invalidate();
			return this;
		}
		protected virtual bool CanShowFilterPopup { get { return true; } }
		public override PivotGridHitInfo CalcHitInfo(Point hitPoint) {
			PivotGridHeaderHitTest hitTest = PivotGridHeaderHitTest.None;
			if(IsMouseOnFilterButton(hitPoint))
				hitTest = PivotGridHeaderHitTest.Filter;
			return new PivotGridHitInfo(HeadersViewInfo, Field, hitTest, hitPoint);
		}	
		void IPivotGridDropDownFilterEditOwner.CloseFilter() {
			this.isFilterDown = false;
			Invalidate();
			Data.UserAction = UserAction.None;
		}
		CustomizationFormFields IPivotGridDropDownFilterEditOwner.CustomizationFormFields { get { return GetCustomizationFormFields(); } }
		protected virtual CustomizationFormFields GetCustomizationFormFields() {
			return null;
		}
		public override bool IsFilterSmartTagStyle { get { return Data.OptionsView.GetHeaderFilterButtonShowMode() == FilterButtonShowMode.SmartTag; } }
		protected override void OnMenuItemClick(DXMenuItem menuItem) {
			base.OnMenuItemClick(menuItem);
			switch((int)menuItem.Tag) {
				case PivotContextMenuIds.HideFieldMenuID:
					OriginalField.Visible = false;
					break;
				case PivotContextMenuIds.ShowHeaderExpressionMenuID:
					ShowExpressionEditor();
					break;
				case PivotContextMenuIds.MoveToBeginningMenuID:
					Data.SetFieldAreaPosition(OriginalField, OriginalField.Area, 0);
					break;
				case PivotContextMenuIds.MoveToEndMenuID:
					if(Field.AreaIndex > 0) {
						Data.SetFieldAreaPosition(OriginalField, OriginalField.Area, IsFieldAfterDataField() ? OriginalField.AreaIndex : OriginalField.AreaIndex - 1);
					}
					break;
				case PivotContextMenuIds.MoveRightMenuID:
					Data.SetFieldAreaPosition(OriginalField, OriginalField.Area, IsFieldAfterDataField() ? OriginalField.AreaIndex + 2 : OriginalField.AreaIndex + 1);
					break;
				case PivotContextMenuIds.MoveLeftMenuID:
					Data.SetFieldAreaPosition(OriginalField, OriginalField.Area, int.MaxValue);
					break;
				case PivotContextMenuIds.SortAscendingID:
					Data.SetFieldSortingAsync(OriginalField, PivotSortOrder.Ascending, PivotSortMode.DisplayText, null, true, false);
					break;
				case PivotContextMenuIds.SortDescendingID:
					Data.SetFieldSortingAsync(OriginalField, PivotSortOrder.Descending, PivotSortMode.DisplayText, null, true, false);
					break;
				case PivotContextMenuIds.ClearSortingID:
					Data.SetFieldSortingAsync(OriginalField, PivotSortOrder.Ascending, null, PivotSortMode.None, false, false);
					break;
			}
		}
		bool IsFieldAfterDataField() {
			return !Field.IsDataField && Field.Area == Data.DataField.Area && Data.DataField.Visible && Data.DataField.AreaIndex <= Field.AreaIndex;
		}
		protected void ShowExpressionEditor() {
			Data.PivotGrid.ShowUnboundExpressionEditor(OriginalField);		   
		}
		protected override void OnAfterCalculated() {
			if(Field.Area != PivotArea.RowArea) {
				SetBestWidth();
			}
		}
		protected override ViewInfoPaintArgs CreateViewInfoPaintArgs(GraphicsCache cache) {
			return new ViewInfoPaintArgs(Data.ControlOwner, new PaintEventArgs(cache.Graphics, Rectangle.Empty));
		}
		protected override void Invalidate(Rectangle bounds) {
			base.Invalidate(bounds);
			Data.Invalidate(bounds);
		}
		protected void CreateSummaryTypesMenuItems(DXPopupMenu menu) {
			foreach(PivotSummaryType type in Enum.GetValues(typeof(PivotSummaryType))) {
				DXMenuCheckItem item = new DXMenuCheckItem(PivotGridLocalizer.GetSummaryTypeText(type), 
					type == Field.SummaryType);
				item.Tag = type;
				item.CheckedChanged += new EventHandler(OnSummaryTypeMenu_Click);
				menu.Items.Add(item);
			}
		}
		void OnSummaryTypeMenu_Click(object sender, EventArgs e) {
			if(RaiseMenuClick((DXMenuItem)sender)) return;
			OriginalField.SummaryType = (PivotSummaryType)((DXMenuItem)sender).Tag;
		}
		protected override void CreatePopupMenuItems(DXPopupMenu menu) {
			if(Field.CanSortCore) {
				AddPopupMenuSortAscending();
				AddPopupMenuSortDescending();
				AddPopupMenuClearSorting();
			}
			AddPopupMenuRefresh();
			if(Field.IsOLAPSortModeNone && Field.CanSortCore) menu.Items[menu.Items.Count - 1].BeginGroup = true;
			AddPopupMenuShowExpression();
			menu.Items.Add(CreateMenuItem(GetLocalizedString(PivotGridStringId.PopupMenuHideField), PivotContextMenuIds.HideFieldMenuID, CanHide));
			menu.Items[menu.Items.Count - 1].BeginGroup = true;
			if(Field.Group == null) 
				AddPopupMenuOrderHeader();
			AddPopupMenuFieldCustomization();
			AddPopupMenuPrefilter();
		}
		protected virtual void AddPopupMenuOrderHeader() {
			int fieldCount = Data.GetFieldCountByArea(OriginalField.Area);
			if(Data.DataField.Visible && Data.DataField.Area == OriginalField.Area)
				fieldCount++;
			DXSubMenuItem subMenuItem = new DXSubMenuItem(GetLocalizedString(PivotGridStringId.PopupMenuFieldOrder));
			subMenuItem.Tag = PivotContextMenuIds.OrderMenuID;
			int correctedAreaIndex = Field.AreaIndex;
			if(IsFieldAfterDataField())
				correctedAreaIndex++;
			subMenuItem.Items.Add(CreateMenuItem(GetLocalizedString(PivotGridStringId.PopupMenuMovetoBeginning), PivotContextMenuIds.MoveToBeginningMenuID, correctedAreaIndex > 0 && OriginalField.CanChangeLocationTo(OriginalField.Area, 0)));
			subMenuItem.Items.Add(CreateMenuItem(GetLocalizedString(PivotGridStringId.PopupMenuMovetoLeft), PivotContextMenuIds.MoveToEndMenuID, correctedAreaIndex > 0 && OriginalField.CanChangeLocationTo(OriginalField.Area, correctedAreaIndex - 1)));
			subMenuItem.Items.Add(CreateMenuItem(GetLocalizedString(PivotGridStringId.PopupMenuMovetoRight), PivotContextMenuIds.MoveRightMenuID, correctedAreaIndex + 1 < fieldCount && OriginalField.CanChangeLocationTo(OriginalField.Area, correctedAreaIndex + 1)));
			subMenuItem.Items.Add(CreateMenuItem(GetLocalizedString(PivotGridStringId.PopupMenuMovetoEnd), PivotContextMenuIds.MoveLeftMenuID, correctedAreaIndex + 1 < fieldCount && OriginalField.CanChangeLocationTo(OriginalField.Area, fieldCount - 1)));
			menu.Items.Add(subMenuItem);
		}
		protected virtual void AddPopupMenuShowExpression() {
			if(!Field.CanShowUnboundExpressionMenu) return;
			menu.Items.Add(CreateMenuItem(GetLocalizedString(PivotGridStringId.PopupMenuShowExpression), PivotContextMenuIds.ShowHeaderExpressionMenuID, true));
		}
		protected void AddPopupMenuSortAscending() {
			if(!Field.IsOLAPSortModeNone) return;
			bool isChecked = OriginalField.IsOLAPSorted && (OriginalField.SortOrder == PivotSortOrder.Ascending);
			menu.Items.Add(CreateMenuCheckItem(GetLocalizedString(PivotGridStringId.PopupMenuSortAscending), isChecked, PivotContextMenuIds.SortAscendingID, 3));
		}
		protected void AddPopupMenuSortDescending() {
			if(!Field.IsOLAPSortModeNone) return;
			bool isChecked = OriginalField.IsOLAPSorted && (OriginalField.SortOrder == PivotSortOrder.Descending);
			menu.Items.Add(CreateMenuCheckItem(GetLocalizedString(PivotGridStringId.PopupMenuSortDescending), isChecked, PivotContextMenuIds.SortDescendingID, 4));
		}
		protected void AddPopupMenuClearSorting() {
			if(!OriginalField.IsOLAPSortModeNone) return;
			menu.Items.Add(CreateMenuItem(GetLocalizedString(PivotGridStringId.PopupMenuClearSorting), PivotContextMenuIds.ClearSortingID, OriginalField.IsOLAPSorted));
		}
	}
	public class PivotDragHeaderViewInfo : PivotHeaderViewInfoBase {
		public PivotDragHeaderViewInfo(PivotGridViewInfoBase viewInfo, PivotFieldItem field)
			: base(viewInfo, field, true) {
			UpdateCaption();
			EnsureIsCalculated();
		}
		public override bool ShowSortButton { get { return false; } }
		public override bool ShowFilterButton { get { return false; } }
		public override bool ShowCollapsedButton { get { return false; } }
		public new PivotHeadersViewInfoBase HeadersViewInfo { get { return ViewInfo.GetHeader(Field.Area); } }
		public override string Caption {
			get {
				return Field.HeaderDisplayText;
			}
		}
		public void PaintDragHeader(ViewInfoPaintArgs e, int locationX) {
			Rectangle drawBounds = new Rectangle(new Point(locationX, 0), Bounds.Size);
			if(HeadersViewInfo != null) {
				HeadersViewInfo.Appearance.FillRectangle(e.GraphicsCache, drawBounds);
			}
			InternalPaint(e, drawBounds, false, false);
		}
	}
	public class PivotHeaderViewInfoBase : PivotViewInfo, IPivotCustomDrawAppearanceOwner {
		const int CollapsedButtonOffset = 2;
		PivotFieldItem field;
		HeaderObjectInfoArgs infoArgs;
		SortedShapeObjectInfoArgs sortedInfo;
		GridFilterButtonInfoArgs filterInfo;		
		AppearanceObject appearance;		
		OpenCloseButtonInfoArgs openCloseButtonInfo;
		protected OpenCloseButtonInfoArgs OpenCloseButtonInfo { get { return openCloseButtonInfo; } }
		protected virtual bool IsFilterDown { get { return false; } }
		public PivotHeaderViewInfoBase(PivotGridViewInfoBase viewInfo, PivotFieldItem field)
			: this(viewInfo, field, true) {
		}
		protected PivotHeaderViewInfoBase(PivotGridViewInfoBase viewInfo, PivotFieldItem field, bool initialize)
			: base(viewInfo) {
			this.field = field;
			this.infoArgs = new HeaderObjectInfoArgs();
			this.appearance = new AppearanceObject();
			AppearanceHelper.Combine(this.appearance, GetCombinedAppearanceObjects());
			if(initialize)
				Initialize();
		}
		protected void Initialize() {
			InfoArgs.SetAppearance(Appearance);
			InfoArgs.Caption = Caption;
			InfoArgs.IsTopMost = true;
			InfoArgs.HeaderPosition = HeaderPositionKind.Special;
			CreateSort();
			CreateFilter();
			CreateHeaderImage();
			CreateOpenCloseButton();
		}
		protected virtual AppearanceObject[] GetCombinedAppearanceObjects() {
			return new AppearanceObject[] { Field.Appearance.Header, ViewInfo.PrintAndPaintAppearance.FieldHeader };
		}
		protected void CreateSort() {
			if(!ShowSortButton) return;
			this.sortedInfo = new SortedShapeObjectInfoArgs();
			this.sortedInfo.Ascending = Field.SortOrder == PivotSortOrder.Ascending;
			InfoArgs.InnerElements.Add(Data.ActiveLookAndFeel.Painter.SortedShape, this.sortedInfo);
		}
		protected void CreateFilter() {
			if(!ShowFilterButton) return;
			this.filterInfo = new GridFilterButtonInfoArgs();
			this.filterInfo.Filtered = ShowActiveFilterButton;
			if(IsFilterSmartTagStyle) {
				DrawElementInfo di = new DrawElementInfo(FilterButtonHelper.GetSmartPainter(Data.ActiveLookAndFeel), this.filterInfo, StringAlignment.Far);
				di.ElementInterval = 0;
				di.RequireTotalBounds = true;
				InfoArgs.InnerElements.Add(di);
			} else {
				InfoArgs.InnerElements.Add(FilterButtonHelper.GetPainter(Data.ActiveLookAndFeel), this.filterInfo);
			}
		}
		protected void CreateHeaderImage() {
			if(AddImage && Field.ImageIndex > -1 && Data.HeaderImages != null) {
				bool allowGlyphSkinning = Data.OptionsView.AllowGlyphSkinning;
				GlyphElementPainter painter = PivotViewInfo.CreateGlyphPainter(allowGlyphSkinning);
				GlyphElementInfoArgs info = PivotViewInfo.CreateGlyphInfoArgs(Data.HeaderImages, Field.ImageIndex, allowGlyphSkinning);
				InfoArgs.InnerElements.Add(new DrawElementInfo(painter, info, StringAlignment.Near));
			}
		}
		protected void CreateOpenCloseButton() {
			if(!AddCollapseButton) return;
			this.openCloseButtonInfo = ShowCollapsedButton ? new OpenCloseButtonInfoArgs(null) : null;
			if(this.openCloseButtonInfo != null) {
				this.openCloseButtonInfo.Opened = Field.ExpandedInFieldsGroup;
				InfoArgs.InnerElements.Add(new DrawElementInfo(Data.ActiveLookAndFeel.Painter.OpenCloseButton, this.openCloseButtonInfo, StringAlignment.Near));
			}
		}
		public PivotFieldItem Field { get { return (PivotFieldItem)ViewInfo.Data.GetFieldItem(field); } }
		protected internal PivotGridField OriginalField {
			get { return Data.GetField(Field) as PivotGridField; }
		}
		public HeaderObjectInfoArgs InfoArgs { get { return infoArgs; } }
		public virtual bool ShowSortButton { get { return Field.Visible && Field.ShowSortButton; } }
		public virtual bool ShowFilterButton { get { return Field.ShowFilterButton; } }
		public virtual bool ShowActiveFilterButton { get { return Field.ShowActiveFilterButton; } }
		public SortedShapeObjectInfoArgs SortedInfo { get { return sortedInfo; } }
		public GridFilterButtonInfoArgs FilterInfo { get { return filterInfo; } }
		public bool CanDrag { get { return Field.CanDrag; } }
		public bool CanHide { get { return Field.CanHide; } }
		public virtual bool ShowCollapsedButton {
			get {
				return Field.ShowCollapsedButton;
			}
		}
		protected bool AllowRunTimeSummaryChange {
			get {
				if(Field.Area != PivotArea.DataArea) return false;
				return Field.Options.AllowRunTimeSummaryChange;
			}
		}
		protected bool ShowSummaryTypeName { get { return Field.Options.ShowSummaryTypeName; } }
		protected virtual bool HotTrackFilterButton { get { return false; } set { ; } }
		public virtual bool IsFilterSmartTagStyle { get { return false; } }
		public int BestWidth {
			get {
				int bestWidth = 0;
				Graphics graphics =	GraphicsInfo.Default.AddGraphics(null);
				GraphicsCache graphicsCache = new GraphicsCache(graphics);
				try {
					bestWidth = GetMinBounds(graphicsCache).Width;
				}
				finally {
					graphicsCache.Dispose();
					GraphicsInfo.Default.ReleaseGraphics();
				}
				return bestWidth + WidthOffset;
			}
		}
		public void SetBestWidth() {
			Width = BestWidth;
		}
		protected internal int WidthOffset {
			get {
				if(Field.Area == PivotArea.ColumnArea) return 0;
				return Parent == null || this != Parent.FirstChild ? FieldMeasures.HeaderWidthOffset : 2 * FieldMeasures.HeaderWidthOffset;
			}
		}
		protected Rectangle GetMinBounds(GraphicsCache graphicsCache) {
			InfoArgs.Cache = graphicsCache;
			InfoArgs.Bounds = new Rectangle(0, 0, int.MaxValue, Bounds.Height);
			InfoArgs.CaptionRect = InfoArgs.Bounds;
			return GetHeaderPainter().CalcObjectMinBounds(InfoArgs);
		}
		public override ToolTipControlInfo GetToolTipObjectInfo(Point pt) {
			if(Field.ToolTips.HeaderText != string.Empty)
				return new ToolTipControlInfo(this, Field.ToolTips.HeaderText);
			GraphicsInfo ginfo = new GraphicsInfo();
			ginfo.AddGraphics(null);
			bool isFit = GetHeaderPainter().IsCaptionFit(ginfo.Cache, InfoArgs);
			ginfo.ReleaseGraphics();
			if(isFit) return null;
			return new ToolTipControlInfo(this, InfoArgs.Caption);
		}		
		public void UpdateCaption() {
			InfoArgs.Caption = Caption;
		}
		public PivotHeadersViewInfoBase HeadersViewInfo { get { return (PivotHeadersViewInfoBase)Parent; } }
		public virtual string Caption {
			get {
				bool isGroup = (Field.Group != null && HeadersViewInfo == null);
				return isGroup ? Field.Group.ToString() : Field.HeaderDisplayText;
			}
		}
		public bool IsSelected {
			get {
				return IsActive && !IsFilterDown;
			}		
		}
		protected virtual bool AddImage { get { return true; } }
		protected virtual bool AddCollapseButton { get { return true; } }
		protected override PivotFieldItem MenuField { get { return Field; } }				
		public AppearanceObject Appearance { 
			get { return appearance; } 
			set {
				if(value == null) return;
				appearance = value;
			}
		}		
		public void Paint(GraphicsCache cache, bool selected, bool hot) {
			InternalPaint(CreateViewInfoPaintArgs(cache), Bounds, selected, hot);
		}
		protected virtual ViewInfoPaintArgs CreateViewInfoPaintArgs(GraphicsCache cache) {
			return new ViewInfoPaintArgs(null, new PaintEventArgs(cache.Graphics, Rectangle.Empty));
		}
		protected override void InternalPaint(ViewInfoPaintArgs e) {
			if(PaintBounds.Width < FieldMeasures.HeaderWidthOffset * 2) return;
			InternalPaint(e, PaintBounds, IsSelected, IsHotTrack);
		}
		protected void InternalPaint(ViewInfoPaintArgs e, Rectangle paintBounds, bool selected, bool hot) {
			InitInfoArgs(e.GraphicsCache, paintBounds, selected, hot);
			GetHeaderPainter().CalcObjectBounds(InfoArgs);
			if(this.filterInfo != null) {
				this.filterInfo.SetAppearance(this.filterInfo.Filtered ? Data.PaintAppearance.HeaderFilterButtonActive : Data.PaintAppearance.HeaderFilterButton);
				this.filterInfo.State = IsFilterDown ? ObjectState.Pressed : HotTrackFilterButton ? ObjectState.Hot : ObjectState.Normal;
			}
			MethodInvoker defaultDraw = () => GetHeaderPainter().DrawObject(InfoArgs);
			if(!ViewInfo.DrawFieldHeader(this, e, GetHeaderPainter(), defaultDraw))
				defaultDraw();
		}
		bool IsDisabledState { get { return !IsEnabled && !Data.IsInProcessing; } }
		protected void InitInfoArgs(GraphicsCache graphicsCache, Rectangle paintBounds, bool selected, bool hot) {
			InfoArgs.Bounds = paintBounds;
			InfoArgs.CaptionRect = paintBounds;
			InfoArgs.Cache = graphicsCache;
			InfoArgs.DesignTimeSelected = Field.SelectedAtDesignTime;
			InfoArgs.RightToLeft = IsRightToLeft;
			if(IsDisabledState)
				InfoArgs.State = ObjectState.Disabled;
			else
				InfoArgs.State = selected ? ObjectState.Pressed : hot ? ObjectState.Hot : ObjectState.Normal;
		}
		protected HeaderObjectPainter GetHeaderPainter() {
			return Data.ActiveLookAndFeel.Painter.Header;
		}
		public override PivotGridHitInfo CalcHitInfo(Point hitPoint) {
			PivotGridHeaderHitTest hitTest = PivotGridHeaderHitTest.None;
			return new PivotGridHitInfo(HeadersViewInfo, Field, hitTest, hitPoint);
		}
#if DEBUGTEST
		public void MouseUpCoreNotOnFilterButtonTest() {
			MouseEventArgs e;
			if (HeadersViewInfo == null)
				e = new MouseEventArgs(MouseButtons.Left, 1, Bounds.X + 5, Bounds.Y + 5, 0);
			else
				e = new MouseEventArgs(MouseButtons.Left, 1, Bounds.X + 5 + HeadersViewInfo.Bounds.X, Bounds.Y + 5 + HeadersViewInfo.Bounds.Y, 0);
			MouseUpCore(e);
		}
		public Point HeaderLocation {
			get {
				return new Point(this.ControlBounds.X + this.ControlBounds.Width / 2, this.ControlBounds.Y + this.ControlBounds.Height / 2);
			}
		}
		public Point NotHeaderLocation {
			get {
				return new Point(this.ControlBounds.X + this.ControlBounds.Width + 1, this.ControlBounds.Y + this.ControlBounds.Height + 1);
			}
		}
#endif
	}
}
