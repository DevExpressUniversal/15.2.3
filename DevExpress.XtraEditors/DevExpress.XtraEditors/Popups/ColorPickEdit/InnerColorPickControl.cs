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
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Windows.Forms;
using DevExpress.Skins;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Drawing;
using DevExpress.XtraEditors.ViewInfo;
namespace DevExpress.XtraEditors {
	[ToolboxItem(false)]
	public class InnerColorPickControl : InnerColorPickControlBase {
		static object moreButtonClick = new object();
		static object automaticButtonClick = new object();
		static object selectedColorChanging = new object();
		const int DefaultButtonHeight = 28;
		const int DefaultFirstRowGap = 4;
		const int DefaultItemHGap = 9;
		const int DefaultItemVGap = 0;
		const int DefaultMaxRowItemCount = -1;
		static readonly Color DefaultItemBorderColor = Color.FromArgb(128, 128, 128);
		static readonly Color DefaultAutomaticColor = Color.Black;
		static readonly Color DefaultAutomaticBorderColor = Color.Empty;
		static readonly Size DefaultItemSize = new Size(17, 16);
		static readonly Padding DefaultGroupPadding = new Padding(4);
		int buttonHeight;
		string automaticButtonCaption;
		string moreButtonCaption;
		ColorItemCollection themeColors;
		ColorItemCollection standardColors;
		ColorItemCollection recentColors;
		Padding groupPadding;
		int firstRowGap, itemHGap, itemVGap;
		int maxRowItemCount;
		Size itemSize;
		string themeGroupCaption;
		string standardGroupCaption;
		bool showAutomaticButton;
		bool showThemePalette;
		bool showMoreColors;
		Color itemBorderColor;
		Color automaticBorderColor;
		ColorItem automaticColorItem;
		ColorTooltipFormat colorTooltipFormat;
		AppearanceObject appearanceGroupCaption;
		public InnerColorPickControl() {
			this.buttonHeight = DefaultButtonHeight;
			this.automaticButtonCaption = string.Empty;
			this.moreButtonCaption = string.Empty;
			this.themeColors = new ColorItemCollection();
			this.themeColors.ListChanged += OnThemeColorListChanged;
			this.standardColors = new ColorItemCollection();
			this.standardColors.ListChanged += OnStandardColorListChanged;
			this.recentColors = new ColorItemCollection();
			this.recentColors.ListChanged += OnRecentColorListChanged;
			this.groupPadding = DefaultGroupPadding;
			this.firstRowGap = DefaultFirstRowGap;
			this.itemSize = DefaultItemSize;
			this.itemHGap = DefaultItemHGap;
			this.itemVGap = DefaultItemVGap;
			this.themeGroupCaption = string.Empty;
			this.standardGroupCaption = string.Empty;
			this.itemBorderColor = DefaultItemBorderColor;
			this.automaticBorderColor = DefaultAutomaticBorderColor;
			this.appearanceGroupCaption = CreateAppearance();
			this.colorTooltipFormat = ColorTooltipFormat.Argb;
			this.showAutomaticButton = this.showThemePalette = this.showMoreColors = true;
			this.maxRowItemCount = DefaultMaxRowItemCount;
			this.automaticColorItem = CreateAutomaticColorItemInstance(DefaultAutomaticColor);
			SetStyle(ControlStyles.SupportsTransparentBackColor | ControlStyles.OptimizedDoubleBuffer, true);
		}
		protected override BaseStyleControlViewInfo CreateViewInfo() {
			return new InnerColorPickControlViewInfo(this);
		}
		protected override BaseControlPainter CreatePainter() {
			return new InnerColorPickControlPainter();
		}
		InnerColorPickControlHandler handler;
		protected InnerColorPickControlHandler Handler {
			get {
				if(this.handler == null) {
					this.handler = CreateHandler();
				}
				return this.handler;
			}
		}
		protected virtual InnerColorPickControlHandler CreateHandler() {
			return new InnerColorPickControlHandler(this);
		}
		protected override bool IsInputKey(Keys keyData) {
			if(keyData == Keys.Left || keyData == Keys.Right || keyData == Keys.Up || keyData == Keys.Down) return true;
			return base.IsInputKey(keyData);
		}
		protected override Padding DefaultPadding {
			get { return new Padding(2); }
		}
		[DefaultValue(DefaultButtonHeight)]
		public int ButtonHeight {
			get { return buttonHeight; }
			set {
				if(ButtonHeight == value)
					return;
				buttonHeight = value;
				OnPropertiesChanged();
			}
		}
		[DefaultValue("")]
		public string AutomaticButtonCaption {
			get { return automaticButtonCaption; }
			set {
				if(AutomaticButtonCaption == value)
					return;
				automaticButtonCaption = value;
				OnPropertiesChanged();
			}
		}
		[DefaultValue("")]
		public string MoreButtonCaption {
			get { return moreButtonCaption; }
			set {
				if(MoreButtonCaption == value)
					return;
				moreButtonCaption = value;
				OnPropertiesChanged();
			}
		}
		[DefaultValue("")]
		public string ThemeGroupCaption {
			get { return themeGroupCaption; }
			set {
				if(ThemeGroupCaption == value)
					return;
				themeGroupCaption = value;
				OnPropertiesChanged();
			}
		}
		[DefaultValue("")]
		public string StandardGroupCaption {
			get { return standardGroupCaption; }
			set {
				if(StandardGroupCaption == value)
					return;
				standardGroupCaption = value;
				OnPropertiesChanged();
			}
		}
		public Padding GroupPadding {
			get { return groupPadding; }
			set {
				if(GroupPadding == value)
					return;
				groupPadding = value;
				OnPropertiesChanged();
			}
		}
		bool ShouldSerializeGroupPadding() { return GroupPadding != DefaultGroupPadding; }
		void ResetGroupPadding() { GroupPadding = DefaultGroupPadding; }
		[DefaultValue(DefaultFirstRowGap)]
		public int FirstRowGap {
			get { return firstRowGap; }
			set {
				if(FirstRowGap == value)
					return;
				firstRowGap = value;
				OnPropertiesChanged();
			}
		}
		public Size ItemSize {
			get { return itemSize; }
			set {
				if(ItemSize == value)
					return;
				itemSize = value;
				OnPropertiesChanged();
			}
		}
		bool ShouldSerializeItemSize() { return ItemSize != DefaultItemSize; }
		void ResetItemSize() { ItemSize = DefaultItemSize; }
		[DefaultValue(true)]
		public bool ShowAutomaticButton {
			get { return showAutomaticButton; }
			set {
				if(ShowAutomaticButton == value)
					return;
				showAutomaticButton = value;
				OnPropertiesChanged();
			}
		}
		[DefaultValue(true)]
		public bool ShowThemePalette {
			get { return showThemePalette; }
			set {
				if(ShowThemePalette == value)
					return;
				showThemePalette = value;
				OnPropertiesChanged();
			}
		}
		[DefaultValue(true)]
		public bool ShowMoreColors {
			get { return showMoreColors; }
			set {
				if(ShowMoreColors == value)
					return;
				showMoreColors = value;
				OnPropertiesChanged();
			}
		}
		[DefaultValue(DefaultItemHGap)]
		public int ItemHGap {
			get { return itemHGap; }
			set {
				if(ItemHGap == value)
					return;
				itemHGap = value;
				OnPropertiesChanged();
			}
		}
		[DefaultValue(DefaultItemVGap)]
		public int ItemVGap {
			get { return itemVGap; }
			set {
				if(value < 0) value = 0;
				if(ItemVGap == value)
					return;
				itemVGap = value;
				OnPropertiesChanged();
			}
		}
		public Color ItemBorderColor {
			get { return itemBorderColor; }
			set {
				if(ItemBorderColor == value)
					return;
				itemBorderColor = value;
				OnPropertiesChanged();
			}
		}
		bool ShouldSerializeItemBorderColor() { return ItemBorderColor != DefaultItemBorderColor; }
		void ResetItemBorderColor() { ItemBorderColor = DefaultItemBorderColor; }
		public Color AutomaticColor {
			get { return AutomaticColorItem != null ? AutomaticColorItem.Color : DefaultAutomaticColor; }
			set {
				if(AutomaticColor == value)
					return;
				AutomaticColorItem = CreateAutomaticColorItemInstance(value);
			}
		}
		protected virtual ColorItem CreateAutomaticColorItemInstance(Color color) {
			return new AutoColorItem(color);
		}
		bool ShouldSerializeAutomaticColor() { return AutomaticColor != DefaultAutomaticColor; }
		void ResetAutomaticColor() { AutomaticColor = DefaultAutomaticColor; }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public ColorItem AutomaticColorItem {
			get { return automaticColorItem; }
			set {
				if(AutomaticColorItem == value)
					return;
				automaticColorItem = value;
				OnPropertiesChanged();
			}
		}
		public Color AutomaticBorderColor {
			get { return automaticBorderColor; }
			set {
				if(AutomaticBorderColor == value)
					return;
				automaticBorderColor = value;
				OnPropertiesChanged();
			}
		}
		bool ShouldSerializeAutomaticBorderColor() { return AutomaticBorderColor != DefaultAutomaticBorderColor; }
		void ResetAutomaticBorderColor() { AutomaticBorderColor = DefaultAutomaticBorderColor; }
		[DefaultValue(DefaultMaxRowItemCount)]
		public int MaxRowItemCount {
			get { return maxRowItemCount; }
			set {
				if(MaxRowItemCount == value)
					return;
				maxRowItemCount = value;
				OnPropertiesChanged();
			}
		}
		protected internal virtual void DoSetSelectedColor(ColorItemInfo item) {
			SelectedColorItem = item.ColorItem;
		}
		protected virtual void SetSelectedColor(Color color) {
			SelectedColor = color;
		}
		[DefaultValue(ColorTooltipFormat.Argb)]
		public ColorTooltipFormat ColorTooltipFormat {
			get { return colorTooltipFormat; }
			set {
				if(ColorTooltipFormat == value)
					return;
				colorTooltipFormat = value;
				OnPropertiesChanged();
			}
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual AppearanceObject AppearanceGroupCaption { get { return appearanceGroupCaption; } }
		bool ShouldSerializeAppearanceGroupCaption() {
			return AppearanceGroupCaption.ShouldSerialize();
		}
		public void ApplyAutomaticColor() {
			SetAutomaticColor();
		}
		protected virtual void SetAutomaticColor() {
			SelectedColorItem = AutomaticColorItem;
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ColorItemCollection ThemeColors {
			get { return themeColors; }
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ColorItemCollection StandardColors {
			get { return standardColors; }
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ColorItemCollection RecentColors {
			get { return recentColors; }
		}
		protected int CalcSizeableHeight() {
			int oneItemSize = ItemSize.Width + ItemHGap;
			if(oneItemSize <= 0) return 0;
			int itemsInRow = Width / oneItemSize;
			return CalcBestHeight(itemsInRow);
		}
		protected override Size CalcSizeableMaxSize() {
			return new Size(0, AutoSizeInLayoutControl ? CalcSizeableHeight() : 0);
		}
		protected override Size CalcSizeableMinSize() {
			return new Size(120, CalcSizeableHeight());
		}
		protected override void OnLayout(LayoutEventArgs levent) {
			base.OnLayout(levent);
			RaiseSizeableChanged();
		}
		public int CalcBestHeight(int itemsInRow) {
			return ViewInfo.CalcBestHeight(itemsInRow);
		}
		protected internal new InnerColorPickControlViewInfo ViewInfo { get { return base.ViewInfo as InnerColorPickControlViewInfo; } }
		protected override void OnMouseDown(MouseEventArgs e) {
			Handler.OnMouseDown(e);
			base.OnMouseDown(e);
		}
		protected override void OnMouseUp(MouseEventArgs e) {
			Handler.OnMouseUp(e);
			base.OnMouseUp(e);
		}
		protected override void OnMouseMove(MouseEventArgs e) {
			Handler.OnMouseMove(e);
			base.OnMouseMove(e);
		}
		protected override void OnMouseEnter(EventArgs e) {
			Handler.OnMouseEnter(e);
			base.OnMouseEnter(e);
		}
		protected override void OnMouseLeave(EventArgs e) {
			Handler.OnMouseLeave(e);
			base.OnMouseLeave(e);
		}
		protected override void OnKeyDown(KeyEventArgs e) {
			Handler.OnKeyDown(e);
			base.OnKeyDown(e);
		}
		public event EventHandler MoreButtonClick {
			add { Events.AddHandler(moreButtonClick, value); }
			remove { Events.RemoveHandler(moreButtonClick, value); }
		}
		protected internal virtual void OnMoreButtonClick(EventArgs e) {
			EventHandler handler = (EventHandler)Events[moreButtonClick];
			if(handler != null) handler(this, e);
		}
		public event EventHandler AutomaticButtonClick {
			add { Events.AddHandler(automaticButtonClick, value); }
			remove { Events.RemoveHandler(automaticButtonClick, value); }
		}
		protected internal virtual void OnAutomaticButtonClick(EventArgs e) {
			EventHandler handler = (EventHandler)Events[automaticButtonClick];
			if(handler != null) handler(this, e);
		}
		public event InnerColorPickControlSelectedColorChangingEventHandler SelectedColorChanging {
			add { Events.AddHandler(selectedColorChanging, value); }
			remove { Events.RemoveHandler(selectedColorChanging, value); }
		}
		protected internal virtual bool OnSelectedColorChanging(InnerColorPickControlSelectedColorChangingEventArgs e) {
			InnerColorPickControlSelectedColorChangingEventHandler handler = (InnerColorPickControlSelectedColorChangingEventHandler)Events[selectedColorChanging];
			if(handler != null) handler(this, e);
			return e.Cancel;
		}
		protected override void OnPaddingChanged(EventArgs e) {
			base.OnPaddingChanged(e);
			OnPropertiesChanged();
		}
		void OnThemeColorListChanged(object sender, ListChangedEventArgs e) {
			OnThemeColorListChanged(e);
		}
		void OnStandardColorListChanged(object sender, ListChangedEventArgs e) {
			OnStandardColorListChanged(e);
		}
		void OnRecentColorListChanged(object sender, ListChangedEventArgs e) {
			OnRecentColorListChanged(e);
		}
		protected virtual void OnThemeColorListChanged(ListChangedEventArgs e) {
			OnPropertiesChanged();
		}
		protected virtual void OnStandardColorListChanged(ListChangedEventArgs e) {
			OnPropertiesChanged();
		}
		protected virtual void OnRecentColorListChanged(ListChangedEventArgs e) {
			OnPropertiesChanged();
		}
		public override bool ContainsColor(Color color) {
			return StandardColors.ContainsColor(color, false) || RecentColors.ContainsColor(color, false) || (ShowThemePalette && ThemeColors.ContainsColor(color, false)) || RecentColors.ContainsColor(color, false);
		}
		public override ColorItem GetColorItemByColor(Color color) {
			if(AutomaticColorItem != null && AutomaticColorItem.Color.Equals(color)) {
				return AutomaticColorItem;
			}
			if(StandardColors.ContainsColor(color, false)) {
				return StandardColors.GetItem(color, false);
			}
			if(RecentColors.ContainsColor(color, false)) {
				return RecentColors.GetItem(color, false);
			}
			if(ShowThemePalette && ThemeColors.ContainsColor(color, false)) {
				return ThemeColors.GetItem(color, false);
			}
			if(RecentColors.ContainsColor(color, false)) {
				return RecentColors.GetItem(color, false);
			}
			return null;
		}
		#region Tooltip
		protected override ToolTipControlInfo GetToolTipInfo(Point point) {
			InnerColorPickControlHitInfo hitInfo = GetViewInfo().CalcHitInfo(point);
			if(hitInfo.HitTest == InnerColorPickControlHitTest.ColorItem) {
				return ColorItemUtils.GetTooltipInfo(hitInfo.ColorItem, ColorTooltipFormat);
			}
			return base.GetToolTipInfo(point);
		}
		#endregion
		public new InnerColorPickControlViewInfo GetViewInfo() { return ViewInfo as InnerColorPickControlViewInfo; }
		protected override void Dispose(bool disposing) {
			if(disposing) {
				DestroyAppearance(AppearanceGroupCaption);
				this.themeColors.ListChanged -= OnThemeColorListChanged;
				this.standardColors.ListChanged -= OnStandardColorListChanged;
				this.recentColors.ListChanged -= OnRecentColorListChanged;
				if(this.handler != null) {
					this.handler.Dispose();
				}
				this.handler = null;
			}
			base.Dispose(disposing);
		}
	}
	public class ColorItemInfo {
		ColorItem colorItem;
		Rectangle bounds;
		int row;
		int line;
		ObjectState state;
		AppearanceObject paintAppearance;
		public ColorItemInfo(ColorItem colorItem) {
			this.row = this.line = -1;
			this.bounds = Rectangle.Empty;
			this.colorItem = colorItem;
			this.state = ObjectState.Normal;
			this.paintAppearance = new AppearanceObject();
		}
		public void SetBounds(Rectangle bounds) {
			this.bounds = bounds;
		}
		public void SetRow(int row) {
			this.row = row;
		}
		public void SetLine(int line) {
			this.line = line;
		}
		public void SetState(ObjectState state) {
			this.state = state;
		}
		public virtual bool IsMatch(Color other) {
			return other.ToArgb() == Color.ToArgb();
		}
		public string GetHint(ColorTooltipFormat format) {
			if(format == ColorTooltipFormat.Argb) return string.Format("{0}; {1}; {2}", Color.R.ToString(), Color.G.ToString(), Color.B.ToString());
			return string.Format("#{0}{1}{2}", Color.R.ToString("X02"), Color.G.ToString("X02"), Color.B.ToString("X02"));
		}
		public int Row { get { return row; } }
		public int Line { get { return line; } }
		public Rectangle Bounds { get { return bounds; } }
		public Color Color { get { return colorItem.Color; } }
		public ColorItem ColorItem { get { return colorItem; } }
		public ObjectState State { get { return state; } }
		public AppearanceObject PaintAppearance { get { return paintAppearance; } }
	}
	public enum InnerColorPickControlRect {
		AutomaticButtonBounds,
		MoreButtonBounds,
		ThemeGroupCaptionBounds,
		ThemeGroupContentBounds,
		StandardGroupCaptionBounds,
		StandardGroupContentBounds,
		AutomaticButtonCaptionBounds,
		MoreButtonCaptionBounds,
		ThemeGroupCaptionTextBounds,
		StandardGroupCaptionTextBounds,
		AutomaticColorItemBounds,
		MoreButtonGlyphBounds,
		RecentGroupCaptionBounds,
		RecentGroupCaptionTextBounds,
		RecentGroupContentBounds
	}
	public class InnerColorPickControlRects {
		Dictionary<InnerColorPickControlRect, Rectangle> rects;
		public InnerColorPickControlRects() {
			this.rects = new Dictionary<InnerColorPickControlRect, Rectangle>();
		}
		public Rectangle this[InnerColorPickControlRect rect] {
			get {
				if(!this.rects.ContainsKey(rect)) return Rectangle.Empty;
				return this.rects[rect];
			}
			set { this.rects[rect] = value; }
		}
	}
	public enum InnerColorPickControlHitTest {
		None, AutomaticButton, MoreButton, ColorItem,
	}
	public class InnerColorPickControlHitInfo {
		Point hitPoint;
		object hitObject;
		InnerColorPickControlHitTest hitTest;
		public InnerColorPickControlHitInfo() : this(Point.Empty) { }
		public InnerColorPickControlHitInfo(Point hitPoint) {
			this.hitPoint = hitPoint;
			this.hitObject = null;
			this.hitTest = InnerColorPickControlHitTest.None;
		}
		public void SetHitTest(InnerColorPickControlHitTest hitTest) {
			this.hitTest = hitTest;
		}
		public void SetHitObject(object hitObject) {
			this.hitObject = hitObject;
		}
		public void Reset() {
			this.hitPoint = Point.Empty;
			this.hitObject = null;
		}
		public static readonly InnerColorPickControlHitInfo Empty = new InnerColorPickControlHitInfo();
		public bool IsEmpty {
			get { return Equals(Empty); }
		}
		public override bool Equals(object obj) {
			InnerColorPickControlHitInfo other = obj as InnerColorPickControlHitInfo;
			if(other == null) return false;
			return other.HitTest == HitTest && other.hitObject == hitObject;
		}
		public override int GetHashCode() {
			return base.GetHashCode();
		}
		public ColorItemInfo ColorItem { get { return (ColorItemInfo)hitObject; } }
		public Point HitPoint { get { return hitPoint; } }
		public InnerColorPickControlHitTest HitTest { get { return hitTest; } }
	}
	public class InnerColorPickControlHotObject {
		InnerColorPickControlHitInfo hotObject;
		public InnerColorPickControlHotObject() {
			this.hotObject = InnerColorPickControlHitInfo.Empty;
		}
		public void SetHotObject(InnerColorPickControlHitInfo newObj) {
			if(newObj.Equals(this.hotObject)) return;
			InnerColorPickControlHitInfo prev = this.hotObject;
			this.hotObject = newObj;
			OnHotObjectChanged(new InnerColorPickControlHotObjectChangedEventArgs(prev, newObj));
		}
		public void Reset() {
			if(this.hotObject.IsEmpty) return;
			InnerColorPickControlHitInfo prev = this.hotObject;
			this.hotObject = InnerColorPickControlHitInfo.Empty;
			OnHotObjectChanged(new InnerColorPickControlHotObjectChangedEventArgs(prev, this.hotObject));
		}
		protected virtual void OnHotObjectChanged(InnerColorPickControlHotObjectChangedEventArgs e) {
			if(HotObjectChanged != null) HotObjectChanged(this, e);
		}
		public event InnerColorPickControlHotObjectChangedHandler HotObjectChanged;
	}
	public delegate void InnerColorPickControlHotObjectChangedHandler(object sender, InnerColorPickControlHotObjectChangedEventArgs e);
	public class InnerColorPickControlHotObjectChangedEventArgs : EventArgs {
		InnerColorPickControlHitInfo prev;
		InnerColorPickControlHitInfo next;
		public InnerColorPickControlHotObjectChangedEventArgs(InnerColorPickControlHitInfo prev, InnerColorPickControlHitInfo next) {
			this.prev = prev;
			this.next = next;
		}
		public InnerColorPickControlHitInfo Prev { get { return prev; } }
		public InnerColorPickControlHitInfo Next { get { return next; } }
	}
	public class InnerColorPickControlViewInfo : BaseStyleControlViewInfo {
		InnerColorPickControlRects rects;
		Hashtable themeItems, standardItems, recentItems;
		AppearanceObject appearanceGroupCaption;
		AppearanceObject paintAppearanceGroupCaption;
		int groupTextHeight;
		Image moreButtonGlyph;
		ObjectState automaticButtonState, moreButtonState;
		InnerColorPickControlHotObject hotObject;
		public InnerColorPickControlViewInfo(InnerColorPickControl owner) : base(owner) {
			this.groupTextHeight = 0;
			this.paintAppearanceGroupCaption = CreatePaintAppearance();
			this.themeItems = new Hashtable();
			this.standardItems = new Hashtable();
			this.recentItems = new Hashtable();
			this.rects = new InnerColorPickControlRects();
			this.moreButtonGlyph = LoadMoreButtonGlyph();
			this.hotObject = new InnerColorPickControlHotObject();
			this.hotObject.HotObjectChanged += OnHotObjectChanged;
		}
		AppearanceDefault defaultAppearanceGroupCaption = null;
		public virtual AppearanceDefault DefaultAppearanceGroupCaption {
			get {
				if(defaultAppearanceGroupCaption == null) defaultAppearanceGroupCaption = CreateDefaultAppearanceGroupCaption();
				return defaultAppearanceGroupCaption;
			}
		}
		protected virtual AppearanceDefault CreateDefaultAppearanceGroupCaption() {
			AppearanceDefault appearance = new AppearanceDefault(GetSystemColor(SystemColors.WindowText), GetSystemColor(SystemColors.Window), GetDefaultFont());
			SkinElement element = GetRibbonSkinElement(RibbonSkins.SkinPopupGalleryGroupCaption);
			if(element != null) {
				element.ApplyForeColorAndFont(appearance);
			}
			return appearance;
		}
		public virtual AppearanceObject PaintAppearanceGroupCaption {
			get { return paintAppearanceGroupCaption; }
			set {
				paintAppearanceGroupCaption = value;
				OnPaintAppearanceChanged();
			}
		}
		public virtual AppearanceObject AppearanceGroupCaption {
			get { return appearanceGroupCaption; }
			set {
				if(value == null) return;
				appearanceGroupCaption = value;
			}
		}
		public override void UpdatePaintAppearance() {
			base.UpdatePaintAppearance();
			AppearanceHelper.Combine(PaintAppearanceGroupCaption, new AppearanceObject[] { AppearanceGroupCaption, Appearance, StyleController != null ? StyleController.Appearance : null }, DefaultAppearanceGroupCaption);
			this.paintAppearanceGroupCaption.TextOptions.RightToLeft = RightToLeft;
		}
		protected override void UpdateFromOwner() {
			AppearanceGroupCaption = OwnerControl.AppearanceGroupCaption;
			base.UpdateFromOwner();
		}
		protected internal override void ResetAppearanceDefault() {
			this.defaultAppearanceGroupCaption = null;
			base.ResetAppearanceDefault();
		}
		protected internal void SetAutomaticButtonState(ObjectState state) {
			this.automaticButtonState = state;
		}
		protected internal void ResetStates(bool skipSelected = true) {
			SetAutomaticButtonState(ObjectState.Normal);
			SetMoreButtonState(ObjectState.Normal);
			foreach(ColorItemInfo colorItem in GetAllItems()) {
				if(colorItem.State == ObjectState.Selected && skipSelected) continue;
				colorItem.SetState(ObjectState.Normal);
			}
		}
		protected internal void SetMoreButtonState(ObjectState state) {
			this.moreButtonState = state;
		}
		#region Best Height
		public virtual int CalcBestHeight(int itemsInRow) {
			int bestHeight = 0;
			bool graphicsAdded = false;
			if(GInfo.Graphics == null) {
				GInfo.AddGraphics(null);
				graphicsAdded = true;
			}
			try {
				UpdateTextSizes();
				bestHeight = CalcBestHeightCore(itemsInRow);
			}
			finally {
				if(graphicsAdded) GInfo.ReleaseGraphics();
			}
			return bestHeight;
		}
		static readonly int BorderSize = 1;
		protected virtual int CalcBestHeightCore(int itemsInRow) {
			int height = OwnerControl.Padding.Vertical + CalcAutomaticButtonBestHeight() + CalcMoreColorButtonBestHeight() + CalcThemeGroupBestHeight(itemsInRow) + CalcStandardGroupBestHeight(itemsInRow) + CalcRecentGroupBestHeight(itemsInRow);
			if(OwnerControl.BorderStyle != BorderStyles.NoBorder) {
				height += BorderSize * 2;
			}
			if(!OwnerControl.ShowAutomaticButton) {
				height -= (OwnerControl.GroupPadding.Top - BorderSize * 2 - 1);
			}
			return Math.Max(0, height);
		}
		protected virtual int CalcAutomaticButtonBestHeight() {
			if(!OwnerControl.ShowAutomaticButton) return 0;
			return CalcButtonHeight() + 1;
		}
		protected virtual int CalcMoreColorButtonBestHeight() {
			return CalcButtonHeight() + 1;
		}
		protected virtual int CalcThemeGroupBestHeight(int itemsInRow) {
			if(!OwnerControl.ShowThemePalette) return 0;
			return CalcBestGroupHeightCore(itemsInRow, OwnerControl.ThemeColors.Count);
		}
		protected virtual int CalcStandardGroupBestHeight(int itemsInRow) {
			return CalcBestGroupHeightCore(itemsInRow, OwnerControl.StandardColors.Count);
		}
		protected virtual int CalcRecentGroupBestHeight(int itemsInRow) {
			if(!ShowRecentPalette) return 0;
			return CalcBestGroupHeightCore(itemsInRow, OwnerControl.RecentColors.Count);
		}
		protected int CalcBestGroupHeightCore(int itemsInRow, int colorCount) {
			if(itemsInRow == 0) return 0;
			int rowCount = colorCount / itemsInRow + ((colorCount % itemsInRow) > 0 ? 1 : 0);
			return OwnerControl.GroupPadding.Vertical + CalcGroupCaptionHeight() + CalcGroupContentHeightCore(rowCount);
		}
		#endregion
		protected override void CalcConstants() {
			base.CalcConstants();
			UpdateTextSizes();
		}
		protected void UpdateTextSizes() {
			this.groupTextHeight = CalcGroupTextHeight();
		}
		protected virtual int CalcGroupTextHeight() {
			return PaintAppearanceGroupCaption.CalcTextSizeInt(GInfo.Graphics, "Wg", 0).Height;
		}
		protected virtual Image LoadMoreButtonGlyph() {
			return ResourceImageHelper.CreateImageFromResources("DevExpress.XtraEditors.Popups.ColorPickEdit.palette.png", typeof(ColorPickEdit).Assembly);
		}
		protected override void CalcContentRect(Rectangle bounds) {
			base.CalcContentRect(bounds);
			AdjustContent(bounds);
			Rects[InnerColorPickControlRect.AutomaticButtonBounds] = CalcAutomaticButtonBounds();
			Rects[InnerColorPickControlRect.ThemeGroupCaptionBounds] = CalcThemeGroupCaptionBounds();
			Rects[InnerColorPickControlRect.ThemeGroupContentBounds] = CalcThemeGroupContentBounds();
			Rects[InnerColorPickControlRect.StandardGroupCaptionBounds] = CalcStandardGroupCaptionBounds();
			Rects[InnerColorPickControlRect.StandardGroupContentBounds] = CalcStandardGroupContentBounds();
			Rects[InnerColorPickControlRect.RecentGroupCaptionBounds] = CalcRecentGroupCaptionBounds();
			Rects[InnerColorPickControlRect.RecentGroupCaptionTextBounds] = CalcRecentGroupCaptionTextBounds();
			Rects[InnerColorPickControlRect.RecentGroupContentBounds] = CalcRecentGroupContentBounds();
			Rects[InnerColorPickControlRect.MoreButtonBounds] = CalcMoreButtonBounds();
			CalcThemeGroupItems();
			CalcStandardGroupItems();
			CalcRecentGroupItems();
			Rects[InnerColorPickControlRect.AutomaticColorItemBounds] = CalcAutomaticColorItemBounds();
			Rects[InnerColorPickControlRect.AutomaticButtonCaptionBounds] = CalcAutomaticButtonCaptionBounds();
			Rects[InnerColorPickControlRect.MoreButtonGlyphBounds] = CalcMoreButtonGlyphBounds();
			Rects[InnerColorPickControlRect.MoreButtonCaptionBounds] = CalcMoreButtonCaptionBounds();
			Rects[InnerColorPickControlRect.ThemeGroupCaptionTextBounds] = CalcThemeGroupCaptionTextBounds();
			Rects[InnerColorPickControlRect.StandardGroupCaptionTextBounds] = CalcStandardGroupCaptionTextBounds();
		}
		protected virtual void AdjustContent(Rectangle bounds) {
			Padding padding = OwnerControl.Padding;
			this.fContentRect.X += padding.Left;
			this.fContentRect.Width -= (padding.Left + padding.Right);
			this.fContentRect.Y += padding.Top;
			this.fContentRect.Height -= (padding.Top + padding.Bottom);
		}
		public bool IsHotObjectInitialized() {
			if(AutomaticButtonState != ObjectState.Normal || MoreButtonState != ObjectState.Normal) return true;
			foreach(ColorItemInfo colorItem in GetAllItems()) {
				if(colorItem.State == ObjectState.Hot) return true;
			}
			return false;
		}
		protected internal void CheckHotObject() { CheckHotObject(GetMousePoint()); }
		protected internal void CheckHotObject(Point pt) {
			InnerColorPickControlHitInfo hi = CalcHitInfo(pt);
			this.hotObject.SetHotObject(hi);
		}
		protected internal void UpdateState(Point pt) {
			InnerColorPickControlHitInfo hi = CalcHitInfo(pt);
			if(hi.HitTest == InnerColorPickControlHitTest.ColorItem) {
				UpdateState(hi.ColorItem);
			}
			else {
				UpdateButtonState();
			}
			OwnerControl.Invalidate();
		}
		protected internal void ResetHotObject() {
			this.hotObject.Reset();
		}
		protected virtual void UpdateState(ColorItemInfo itemInfo) {
			UpdateButtonState();
			UpdateColorItemState(itemInfo);
			UpdateColorItemAppearance(itemInfo);
		}
		protected virtual void UpdateButtonState() {
			this.automaticButtonState = CalcAutomaticButtonStateCore();
			this.moreButtonState = CalcMoreColorsButtonStateCore();
		}
		protected virtual ObjectState CalcAutomaticButtonStateCore() {
			ObjectState state = CalcButtonStateCore(Rects[InnerColorPickControlRect.AutomaticButtonBounds]);
			if(IsAutoColorChoosen) {
				state |= ObjectState.Selected;
			}
			return state;
		}
		protected virtual ObjectState CalcMoreColorsButtonStateCore() {
			return CalcButtonStateCore(Rects[InnerColorPickControlRect.MoreButtonBounds]);
		}
		protected virtual ObjectState CalcButtonStateCore(Rectangle bounds) {
			if(!OwnerControl.Enabled) return ObjectState.Disabled;
			Point pt = GetMousePoint();
			ObjectState state = ObjectState.Normal;
			if(bounds.Contains(pt)) {
				state = ObjectState.Hot;
				if((Control.MouseButtons & MouseButtons.Left) != 0) state |= ObjectState.Pressed;
			}
			return state;
		}
		protected Point GetMousePoint() { return OwnerControl.PointToClient(Control.MousePosition); }
		protected virtual void UpdateColorItemState(ColorItemInfo itemInfo) {
			foreach(ColorItemInfo item in GetAllItems()) {
				if(item.State == ObjectState.Hot) item.SetState(ObjectState.Normal);
			}
			if(!OwnerControl.Enabled) {
				itemInfo.SetState(ObjectState.Disabled);
				return;
			}
			if(OwnerControl.IsDesignMode) return;
			ObjectState state = ObjectState.Normal;
			if(!IsAutoColorChoosen && OwnerControl.SelectedColorItem != null && OwnerControl.SelectedColorItem.Equals(itemInfo.ColorItem)) {
				state = ObjectState.Selected;
			}
			Point pt = OwnerControl.PointToClient(Control.MousePosition);
			if(itemInfo.Bounds.Contains(pt)) {
				state |= ObjectState.Hot;
			}
			itemInfo.SetState(state);
		}
		public bool IsAutoColorChoosen { get { return OwnerControl.SelectedColorItem != null && OwnerControl.SelectedColorItem.Equals(OwnerControl.AutomaticColorItem); } }
		protected virtual void UpdateColorItemAppearance(ColorItemInfo itemInfo) {
			itemInfo.PaintAppearance.Assign(PaintAppearance);
		}
		public virtual InnerColorPickControlHitInfo CalcHitInfo(Point pt) {
			InnerColorPickControlHitInfo hitInfo = CreateHitInfo(pt);
			if(Rects[InnerColorPickControlRect.AutomaticButtonBounds].Contains(pt)) {
				hitInfo.SetHitTest(InnerColorPickControlHitTest.AutomaticButton);
			}
			else if(Rects[InnerColorPickControlRect.MoreButtonBounds].Contains(pt)) {
				hitInfo.SetHitTest(InnerColorPickControlHitTest.MoreButton);
			}
			else {
				ColorItemInfo item = GetItemByPoint(pt);
				if(item != null) {
					hitInfo.SetHitTest(InnerColorPickControlHitTest.ColorItem);
					hitInfo.SetHitObject(item);
				}
			}
			return hitInfo;
		}
		protected virtual InnerColorPickControlHitInfo CreateHitInfo(Point pt) {
			return new InnerColorPickControlHitInfo(pt);
		}
		protected ColorItemInfo GetItemByPoint(Point pt) {
			foreach(ColorItemInfo colorItem in GetAllItems()) {
				if(colorItem.Bounds.Contains(pt)) return colorItem;
			}
			return null;
		}
		public ColorItemInfo GetHotColorItem() {
			foreach(ColorItemInfo colorItem in GetAllItems()) {
				if(colorItem.State.HasFlag(ObjectState.Hot)) return colorItem;
			}
			return null;
		}
		public IEnumerable<ColorItemInfo> GetAllItems() {
			foreach(ColorItemInfo item in ThemeItems) {
				yield return item;
			}
			foreach(ColorItemInfo item in StandardItems) {
				yield return item;
			}
			foreach(ColorItemInfo item in RecentItems) {
				yield return item;
			}
		}
		void OnHotObjectChanged(object sender, InnerColorPickControlHotObjectChangedEventArgs e) {
			OnHotObjectChanged(e);
		}
		protected virtual void OnHotObjectChanged(InnerColorPickControlHotObjectChangedEventArgs e) {
			if(e.Prev.HitTest == InnerColorPickControlHitTest.ColorItem) {
				UpdateState(e.Prev.ColorItem);
			}
			if(e.Next.HitTest == InnerColorPickControlHitTest.ColorItem) {
				UpdateState(e.Next.ColorItem);
			}
			UpdateButtonState();
			OwnerControl.Invalidate();
		}
		protected virtual Rectangle CalcAutomaticButtonBounds() {
			if(!OwnerControl.ShowAutomaticButton) return Rectangle.Empty;
			return new Rectangle(ContentRect.X, ContentRect.Y, ContentRect.Width, CalcButtonHeight());
		}
		protected int CalcButtonHeight() {
			int minHeight = ObjectPainter.CalcObjectMinBounds(GInfo.Graphics, SkinElementPainter.Default, GetButtonElementInfo(ObjectState.Normal, Rectangle.Empty)).Height;
			return Math.Max(minHeight, OwnerControl.ButtonHeight);
		}
		protected virtual Rectangle CalcThemeGroupCaptionBounds() {
			if(!OwnerControl.ShowThemePalette) return Rectangle.Empty;
			Rectangle rect = new Rectangle(ContentRect.X, 0, ContentRect.Width, CalcGroupCaptionHeight());
			rect.Y = Rects[InnerColorPickControlRect.AutomaticButtonBounds].Bottom + OwnerControl.GroupPadding.Top;
			return rect;
		}
		protected virtual Rectangle CalcThemeGroupContentBounds() {
			if(!OwnerControl.ShowThemePalette) return Rectangle.Empty;
			return CalcGroupContentBounds(OwnerControl.ThemeColors.Count, Rects[InnerColorPickControlRect.ThemeGroupCaptionBounds].Bottom);
		}
		protected virtual Rectangle CalcStandardGroupCaptionBounds() {
			return new Rectangle(ContentRect.X, CalcStandardGroupTopPoint(), ContentRect.Width, CalcGroupCaptionHeight());
		}
		protected virtual int CalcGroupCaptionHeight() {
			return ObjectPainter.CalcObjectMinBounds(GInfo.Graphics, SkinElementPainter.Default, GetGroupCaptionElementInfo(Rectangle.Empty)).Height + GroupTextHeight;
		}
		protected virtual int CalcStandardGroupTopPoint() {
			if(OwnerControl.ShowThemePalette) {
				if(Rects[InnerColorPickControlRect.ThemeGroupContentBounds].Height == 0) {
					return Rects[InnerColorPickControlRect.ThemeGroupCaptionBounds].Bottom;
				}
				return Rects[InnerColorPickControlRect.ThemeGroupContentBounds].Bottom + OwnerControl.GroupPadding.Bottom;
			}
			else {
				if(OwnerControl.ShowAutomaticButton) return Rects[InnerColorPickControlRect.AutomaticButtonBounds].Bottom + OwnerControl.GroupPadding.Top;
				return ContentRect.Top;
			}
		}
		protected virtual Rectangle CalcStandardGroupContentBounds() {
			return CalcGroupContentBounds(OwnerControl.StandardColors.Count, Rects[InnerColorPickControlRect.StandardGroupCaptionBounds].Bottom);
		}
		protected virtual Rectangle CalcRecentGroupContentBounds() {
			if(!ShowRecentPalette) return Rectangle.Empty;
			return CalcGroupContentBounds(OwnerControl.RecentColors.Count, Rects[InnerColorPickControlRect.RecentGroupCaptionBounds].Bottom);
		}
		protected virtual Rectangle CalcGroupContentBounds(int itemCount, int top) {
			int height = CalcGroupContentHeight(itemCount);
			if(height == 0) return Rectangle.Empty;
			return new Rectangle(GroupLeftPt, top + OwnerControl.GroupPadding.Top, GroupWidth, height);
		}
		protected virtual int CalcGroupContentHeight(int itemCount) {
			return CalcGroupContentHeightCore(CalcRowCount(itemCount));
		}
		protected int CalcGroupContentHeightCore(int rowCount) {
			return rowCount * OwnerControl.ItemSize.Height + ((rowCount > 1) ? (OwnerControl.FirstRowGap + OwnerControl.ItemVGap * (rowCount - 1)) : 0) - (rowCount - 1);
		}
		protected virtual Rectangle CalcRecentGroupCaptionBounds() {
			if(!ShowRecentPalette) return Rectangle.Empty;
			return new Rectangle(ContentRect.X, CalcRecentGroupTopPoint(), ContentRect.Width, CalcGroupCaptionHeight());
		}
		protected virtual int CalcRecentGroupTopPoint() {
			if(Rects[InnerColorPickControlRect.StandardGroupContentBounds].Height == 0) {
				return Rects[InnerColorPickControlRect.StandardGroupCaptionBounds].Bottom;
			}
			return Rects[InnerColorPickControlRect.StandardGroupContentBounds].Bottom + OwnerControl.GroupPadding.Bottom;
		}
		public virtual bool ShowRecentPalette { get { return OwnerControl.RecentColors.Count > 0; } }
		protected virtual Rectangle CalcMoreButtonBounds() {
			return new Rectangle(ContentRect.X, CalcMoreButtonTopPoint(), ContentRect.Width, CalcButtonHeight());
		}
		protected virtual int CalcMoreButtonTopPoint() {
			if(ShowRecentPalette) {
				return Rects[InnerColorPickControlRect.RecentGroupContentBounds].Bottom + OwnerControl.GroupPadding.Bottom;
			}
			if(Rects[InnerColorPickControlRect.StandardGroupContentBounds].Height == 0) {
				return Rects[InnerColorPickControlRect.StandardGroupCaptionBounds].Bottom;
			}
			return Rects[InnerColorPickControlRect.StandardGroupContentBounds].Bottom + OwnerControl.GroupPadding.Bottom;
		}
		protected virtual Rectangle CalcAutomaticColorItemBounds() {
			if(!OwnerControl.ShowAutomaticButton) return Rectangle.Empty;
			Rectangle itemBounds = Rects[InnerColorPickControlRect.AutomaticButtonBounds];
			int width = itemBounds.Height / 2;
			return new Rectangle(RightToLeft ? GroupRightPt - width : GroupLeftPt, itemBounds.Y + itemBounds.Height / 2 - width / 2, width, width);
		}
		protected virtual Rectangle CalcAutomaticButtonCaptionBounds() {
			if(!OwnerControl.ShowAutomaticButton) return Rectangle.Empty;
			Rectangle itemBounds = Rects[InnerColorPickControlRect.AutomaticButtonBounds];
			Rectangle colorItemBounds = Rects[InnerColorPickControlRect.AutomaticColorItemBounds];
			if(!RightToLeft) {
				int left = colorItemBounds.Right + colorItemBounds.Width / 2;
				return new Rectangle(left, itemBounds.Y, itemBounds.Right - left, itemBounds.Height);
			}
			int right = colorItemBounds.Left - colorItemBounds.Width / 2;
			return new Rectangle(ContentRect.X, itemBounds.Y, right - ContentRect.X, itemBounds.Height);
		}
		protected virtual Rectangle CalcMoreButtonGlyphBounds() {
			Size glyphSize = MoreButtonGlyph.Size;
			Rectangle itemBounds = Rects[InnerColorPickControlRect.MoreButtonBounds];
			return new Rectangle(RightToLeft ? GroupRightPt - glyphSize.Width : GroupLeftPt, itemBounds.Y + (itemBounds.Height - glyphSize.Height) / 2, glyphSize.Width, glyphSize.Height);
		}
		protected virtual Rectangle CalcMoreButtonCaptionBounds() {
			Rectangle itemBounds = Rects[InnerColorPickControlRect.MoreButtonBounds];
			Rectangle glyphBounds = Rects[InnerColorPickControlRect.MoreButtonGlyphBounds];
			if(!RightToLeft) {
				int left = glyphBounds.Right + glyphBounds.Width / 2;
				return new Rectangle(left, itemBounds.Y, itemBounds.Right - left, itemBounds.Height);
			}
			int right = glyphBounds.Left - glyphBounds.Width / 2;
			return new Rectangle(ContentRect.X, itemBounds.Y, right - ContentRect.X, itemBounds.Height);
		}
		protected int GroupLeftPt {
			get { return ContentRect.X + OwnerControl.GroupPadding.Left; }
		}
		protected int GroupRightPt {
			get { return ContentRect.Right - OwnerControl.GroupPadding.Right - 1; }
		}
		protected int GroupWidth {
			get { return Math.Max(0, ContentRect.Width - OwnerControl.GroupPadding.Horizontal); }
		}
		protected virtual void CalcThemeGroupItems() {
			this.themeItems.Clear();
			if(OwnerControl.ShowThemePalette) {
				CalcGroupItems(OwnerControl.ThemeColors, Rects[InnerColorPickControlRect.ThemeGroupContentBounds].Location, this.themeItems, 0);
			}
		}
		protected virtual void CalcStandardGroupItems() {
			this.standardItems.Clear();
			CalcGroupItems(OwnerControl.StandardColors, Rects[InnerColorPickControlRect.StandardGroupContentBounds].Location, this.standardItems, GetStandardGroupStartLine());
		}
		protected int GetStandardGroupStartLine() {
			return ShowThemePalette ? CalcRowCount(OwnerControl.ThemeColors.Count) : 0;
		}
		protected virtual void CalcRecentGroupItems() {
			this.recentItems.Clear();
			if(ShowRecentPalette) {
				CalcGroupItems(OwnerControl.RecentColors, Rects[InnerColorPickControlRect.RecentGroupContentBounds].Location, this.recentItems, GetRecentGroupStartLine());
			}
		}
		protected int GetRecentGroupStartLine() {
			return GetStandardGroupStartLine() + CalcRowCount(OwnerControl.StandardColors.Count);
		}
		protected void CalcGroupItems(ColorItemCollection items, Point topPt, Hashtable itemStore, int groupLine) {
			if(items.Count == 0) return;
			Point pt = topPt;
			int itemsInRow = CalcItemsInRow();
			for(int i = 0; i < items.Count; i++) {
				ColorItem item = items[i];
				ColorItemInfo itemInfo = GetItem(item, pt, i);
				itemInfo.SetLine(itemInfo.Row + groupLine);
				itemStore.Add(item, itemInfo);
				UpdateState(itemInfo);
				pt.Y = itemInfo.Bounds.Y;
				pt.X = itemInfo.Bounds.Right + OwnerControl.ItemHGap;
			}
		}
		protected int GetRowByPos(int pos) {
			if(CalcItemsInRow() == 0) return 0;
			return 1 + pos / CalcItemsInRow();
		}
		protected ColorItemInfo GetItem(ColorItem item, Point pt, int pos) {
			ColorItemInfo itemInfo = new ColorItemInfo(item);
			itemInfo.SetRow(GetRowByPos(pos));
			Rectangle rect = new Rectangle(pt, OwnerControl.ItemSize);
			if(pos != 0 && IsLeadingItem(pos)) {
				rect.X = GroupLeftPt;
				rect.Y = rect.Bottom - 1 + OwnerControl.ItemVGap;
				if(itemInfo.Row == 2) rect.Y += OwnerControl.FirstRowGap;
			}
			itemInfo.SetBounds(rect);
			return itemInfo;
		}
		protected int CalcRowCount(int itemCount) {
			if(itemCount == 0) return 0;
			int itemsInRow = CalcItemsInRow();
			if(itemsInRow == 0) return 0;
			return (int)(itemCount / itemsInRow) + ((itemCount % itemsInRow != 0) ? 1 : 0);
		}
		protected bool IsLeadingItem(int pos) {
			if(CalcItemsInRow() == 0) return false;
			return (pos % CalcItemsInRow()) == 0;
		}
		protected int CalcItemsInRow() {
			int itemsInRow = CalcItemsInRowCore();
			if(OwnerControl.MaxRowItemCount > 0) {
				itemsInRow = Math.Min(itemsInRow, OwnerControl.MaxRowItemCount);
			}
			return itemsInRow;
		}
		protected int CalcItemsInRowCore() {
			int itemTotalWidth = OwnerControl.ItemSize.Width + OwnerControl.ItemHGap;
			int itemCount = (int)(GroupWidth / itemTotalWidth);
			if(GroupWidth % itemTotalWidth >= OwnerControl.ItemSize.Width) {
				return itemCount + 1;
			}
			return itemCount;
		}
		protected virtual Rectangle CalcThemeGroupCaptionTextBounds() {
			if(!OwnerControl.ShowThemePalette) return Rectangle.Empty;
			Rectangle captionBounds = Rects[InnerColorPickControlRect.ThemeGroupCaptionBounds];
			return new Rectangle(GroupLeftPt, captionBounds.Y, captionBounds.Right - GroupLeftPt, captionBounds.Height);
		}
		protected virtual Rectangle CalcStandardGroupCaptionTextBounds() {
			Rectangle captionBounds = Rects[InnerColorPickControlRect.StandardGroupCaptionBounds];
			return new Rectangle(GroupLeftPt, captionBounds.Y, captionBounds.Right - GroupLeftPt, captionBounds.Height);
		}
		protected virtual Rectangle CalcRecentGroupCaptionTextBounds() {
			if(!ShowRecentPalette) return Rectangle.Empty;
			Rectangle captionBounds = Rects[InnerColorPickControlRect.RecentGroupCaptionBounds];
			return new Rectangle(GroupLeftPt, captionBounds.Y, captionBounds.Right - GroupLeftPt, captionBounds.Height);
		}
		protected internal virtual SkinElementInfo GetButtonElementInfo(ObjectState state, Rectangle bounds) {
			SkinElementInfo elementInfo = new SkinElementInfo(GetRibbonSkinElement(RibbonSkins.SkinGalleryButton), bounds);
			elementInfo.ImageIndex = GetButtonElementImageIndex(state);
			return elementInfo;
		}
		protected int GetButtonElementImageIndex(ObjectState state) {
			if(state == ObjectState.Normal || state == ObjectState.Selected) return 0;
			if((state & ObjectState.Pressed) != 0) return 2;
			return 1;
		}
		protected internal virtual SkinElementInfo GetGroupCaptionElementInfo(Rectangle bounds) {
			return new SkinElementInfo(GetRibbonSkinElement(RibbonSkins.SkinPopupGalleryGroupCaption), bounds);
		}
		protected internal virtual SkinElementInfo GetGroupCaptionBackgroundElementInfo(Rectangle bounds) {
			return new SkinElementInfo(GetRibbonSkinElement(RibbonSkins.SkinPopupGalleryBackground), bounds);
		}
		protected SkinElement GetRibbonSkinElement(string elementName) {
			return RibbonSkins.GetSkin(LookAndFeel)[elementName];
		}
		public virtual Color HotItemBorderColor { get { return Color.FromArgb(242, 149, 54); } }
		public virtual Color HotItemInnerBorderColor { get { return Color.FromArgb(255, 227, 149); } }
		public virtual Color SelectedItemBorderColor { get { return Color.FromArgb(239, 72, 16); } }
		public virtual Color SelectedItemInnerBorderColor { get { return Color.FromArgb(255, 226, 148); } }
		public override void Dispose() {
			AppearanceGroupCaption.Dispose();
			base.Dispose();
		}
		public InnerColorPickControlRects Rects { get { return rects; } }
		public int GroupTextHeight { get { return groupTextHeight; } }
		public Color ItemBorderColor { get { return OwnerControl.ItemBorderColor; } }
		public IEnumerable<ColorItemInfo> ThemeItems {
			get { return GetOrderedItems(this.themeItems.Values); }
		}
		public IEnumerable<ColorItemInfo> StandardItems {
			get { return GetOrderedItems(this.standardItems.Values); }
		}
		public IEnumerable<ColorItemInfo> RecentItems {
			get { return GetOrderedItems(this.recentItems.Values); }
		}
		protected IEnumerable<ColorItemInfo> GetOrderedItems(ICollection col) {
			foreach(ColorItemInfo item in col) {
				if(item.State == ObjectState.Normal) yield return item;
			}
			foreach(ColorItemInfo item in col) {
				if(item.State != ObjectState.Normal) yield return item;
			}
		}
		public string GetThemePaletteCaption() {
			if(string.IsNullOrEmpty(OwnerControl.ThemeGroupCaption)) {
				return Localizer.Active.GetLocalizedString(StringId.ColorPickPopupThemeColorsGroupCaption);
			}
			return OwnerControl.ThemeGroupCaption;
		}
		public string GetStandardPaletteCaption() {
			if(string.IsNullOrEmpty(OwnerControl.StandardGroupCaption)) {
				return Localizer.Active.GetLocalizedString(StringId.ColorPickPopupStandardColorsGroupCaption);
			}
			return OwnerControl.StandardGroupCaption;
		}
		public string GetRecentPaletteCaption() {
			return Localizer.Active.GetLocalizedString(StringId.ColorPickPopupRecentColorsGroupCaption);
		}
		public string GetAutomaticColorButtonCaption() {
			if(string.IsNullOrEmpty(OwnerControl.AutomaticButtonCaption)) {
				return Localizer.Active.GetLocalizedString(StringId.ColorPickPopupAutomaticItemCaption);
			}
			return OwnerControl.AutomaticButtonCaption;
		}
		public string GetMoreColorButtonCaption() {
			if(string.IsNullOrEmpty(OwnerControl.MoreButtonCaption)) {
				return Localizer.Active.GetLocalizedString(StringId.ColorPickPopupMoreColorsItemCaption);
			}
			return OwnerControl.MoreButtonCaption;
		}
		public Color AutomatiColor { get { return OwnerControl.AutomaticColor; } }
		public Color AutomaticBorderColor { get { return OwnerControl.AutomaticBorderColor; } }
		public Image MoreButtonGlyph { get { return moreButtonGlyph; } }
		public ObjectState AutomaticButtonState { get { return automaticButtonState; } }
		public ObjectState MoreButtonState { get { return moreButtonState; } }
		public bool ShowAutomaticButton { get { return OwnerControl.ShowAutomaticButton; } }
		public bool ShowThemePalette { get { return OwnerControl.ShowThemePalette; } }
		public bool ShowMoreColors { get { return OwnerControl.ShowMoreColors; } }
		new public virtual InnerColorPickControl OwnerControl { get { return base.OwnerControl as InnerColorPickControl; } }
	}
	public class InnerColorPickControlHandler : IDisposable {
		InnerColorPickControl owner;
		InnerColorPickControlKeyboardHandler keyboardHandler;
		public InnerColorPickControlHandler(InnerColorPickControl owner) {
			this.owner = owner;
			this.keyboardHandler = CreateKeyboardHandler(owner);
		}
		protected virtual InnerColorPickControlKeyboardHandler CreateKeyboardHandler(InnerColorPickControl owner) {
			return new InnerColorPickControlKeyboardHandler(owner);
		}
		public virtual void OnMouseDown(MouseEventArgs e) {
			Owner.GetViewInfo().UpdateState(e.Location);
		}
		public virtual void OnMouseUp(MouseEventArgs e) {
			Owner.GetViewInfo().UpdateState(e.Location);
			CheckItemClick(e.Location);
		}
		public virtual void OnMouseMove(MouseEventArgs e) {
			Owner.GetViewInfo().CheckHotObject(e.Location);
		}
		public virtual void OnMouseEnter(EventArgs e) {
			Owner.GetViewInfo().CheckHotObject();
		}
		public virtual void OnMouseLeave(EventArgs e) {
			Owner.GetViewInfo().ResetHotObject();
		}
		protected InnerColorPickControlKeyboardHandler KeyboardHandler { get { return keyboardHandler; } }
		public virtual void OnKeyDown(KeyEventArgs e) {
			KeyboardHandler.OnKeyDown(e);
			if(IsPostKey(e)) DoPostValue();
		}
		protected virtual bool IsPostKey(KeyEventArgs e) {
			return e.KeyCode == Keys.Enter || e.KeyCode == Keys.Space;
		}
		protected virtual void CheckItemClick(Point pt) {
			InnerColorPickControlHitInfo hitInfo = Owner.GetViewInfo().CalcHitInfo(pt);
			DoPostValue(hitInfo);
		}
		protected virtual void DoPostValue() {
			if(Owner.ViewInfo.AutomaticButtonState.HasFlag(ObjectState.Hot)) {
				Owner.ApplyAutomaticColor();
			}
			else if(Owner.ViewInfo.MoreButtonState.HasFlag(ObjectState.Hot)) {
				Owner.OnMoreButtonClick(EventArgs.Empty);
			}
			else if(Owner.ViewInfo.AutomaticButtonState.HasFlag(ObjectState.Hot)) {
				Owner.OnAutomaticButtonClick(EventArgs.Empty);
			}
			ColorItemInfo colorItem = Owner.ViewInfo.GetHotColorItem();
			if(colorItem != null) {
				if(!RaiseColorChanging(colorItem)) Owner.DoSetSelectedColor(colorItem);
			}
		}
		protected virtual void DoPostValue(InnerColorPickControlHitInfo hitInfo) {
			if(hitInfo.HitTest == InnerColorPickControlHitTest.MoreButton) {
				Owner.OnMoreButtonClick(EventArgs.Empty);
			}
			if(hitInfo.HitTest == InnerColorPickControlHitTest.ColorItem) {
				if(!RaiseColorChanging(hitInfo.ColorItem)) Owner.DoSetSelectedColor(hitInfo.ColorItem);
			}
			if(hitInfo.HitTest == InnerColorPickControlHitTest.AutomaticButton) {
				Owner.ApplyAutomaticColor();
				Owner.OnAutomaticButtonClick(EventArgs.Empty);
			}
		}
		protected bool RaiseColorChanging(ColorItemInfo itemInfo) {
			InnerColorPickControlSelectedColorChangingEventArgs e = new InnerColorPickControlSelectedColorChangingEventArgs(Owner.SelectedColorItem, itemInfo.ColorItem);
			return Owner.OnSelectedColorChanging(e);
		}
		public InnerColorPickControl Owner { get { return owner; } }
		public void Dispose() {
			Dispose(true);
		}
		protected virtual void Dispose(bool disposing) {
		}
	}
	public class InnerColorPickControlKeyboardHandler {
		InnerColorPickControl owner;
		InnerColorPickControlColorItemNavigationHelper navHelper;
		public InnerColorPickControlKeyboardHandler(InnerColorPickControl owner) {
			this.owner = owner;
			this.navHelper = new InnerColorPickControlColorItemNavigationHelper(Owner);
		}
		public virtual void OnKeyDown(KeyEventArgs e) {
			switch(e.KeyCode) {
				case Keys.Up: DoMoveUp(); break;
				case Keys.Down: DoMoveDown(); break;
				case Keys.Left: DoMoveLeft(); break;
				case Keys.Right: DoMoveRight(); break;
				case Keys.Home: DoMoveHome(); break;
				case Keys.End: DoMoveEnd(); break;
				case Keys.PageUp: DoMovePageUp(); break;
				case Keys.PageDown: DoMovePageDown(); break;
			}
		}
		public virtual void DoMoveUp() {
			if(!IsHotObjectInitialized || Owner.ViewInfo.AutomaticButtonState == ObjectState.Hot) {
				DoSetMoreColorsButton();
			}
			else if(Owner.ViewInfo.MoreButtonState == ObjectState.Hot) {
				ColorItemInfo itemInfo = this.navHelper.GetBottomItem();
				if(itemInfo != null) DoSetColorItem(itemInfo);
			}
			else {
				if(this.navHelper.HotItemInTopRow()) {
					DoSetAutomaticButton();
				}
				else {
					ColorItemInfo itemInfo = this.navHelper.MoveUp();
					if(itemInfo != null) DoSetColorItem(itemInfo);
				}
			}
		}
		public virtual void DoMoveDown() {
			if(!IsHotObjectInitialized || Owner.ViewInfo.MoreButtonState == ObjectState.Hot) {
				DoSetAutomaticButton();
			}
			else if(Owner.ViewInfo.AutomaticButtonState == ObjectState.Hot) {
				ColorItemInfo itemInfo = this.navHelper.TopItem();
				if(itemInfo != null) DoSetColorItem(itemInfo);
			}
			else {
				if(this.navHelper.HotItemInLastRow()) {
					DoSetMoreColorsButton();
				}
				else {
					ColorItemInfo itemInfo = this.navHelper.MoveDown();
					if(itemInfo != null) DoSetColorItem(itemInfo);
				}
			}
		}
		public virtual void DoMoveLeft() {
			if(!IsHotObjectInitialized) {
				DoSetMoreColorsButton();
			}
			else if(this.navHelper.HasSelectedItem()) {
				ColorItemInfo itemInfo = this.navHelper.MoveLeft();
				DoSetColorItem(itemInfo);
			}
		}
		public virtual void DoMoveRight() {
			if(!IsHotObjectInitialized) {
				DoSetAutomaticButton();
			}
			else if(this.navHelper.HasSelectedItem()) {
				ColorItemInfo itemInfo = this.navHelper.MoveRight();
				DoSetColorItem(itemInfo);
			}
		}
		public virtual void DoMoveHome() { DoSetAutomaticButton(); }
		public virtual void DoMoveEnd() { DoSetMoreColorsButton(); }
		public void DoMovePageUp() { DoSetAutomaticButton(); }
		public void DoMovePageDown() { DoSetMoreColorsButton(); }
		protected virtual void DoSetAutomaticButton() {
			Owner.ViewInfo.ResetStates();
			Owner.ViewInfo.SetAutomaticButtonState(ObjectState.Hot);
			Owner.Invalidate();
		}
		protected virtual void DoSetMoreColorsButton() {
			Owner.ViewInfo.ResetStates();
			Owner.ViewInfo.SetMoreButtonState(ObjectState.Hot);
			Owner.Invalidate();
		}
		protected virtual void DoSetColorItem(ColorItemInfo colorItem) {
			Owner.ViewInfo.ResetStates();
			colorItem.SetState(ObjectState.Hot);
			Owner.Invalidate();
		}
		protected bool IsHotObjectInitialized {
			get { return Owner.ViewInfo.IsHotObjectInitialized(); }
		}
		public InnerColorPickControl Owner { get { return owner; } }
	}
	public class InnerColorPickControlColorItemNavigationHelper {
		InnerColorPickControl owner;
		public InnerColorPickControlColorItemNavigationHelper(InnerColorPickControl owner) {
			this.owner = owner;
		}
		public ColorItemInfo TopItem() {
			ReadOnlyCollection<ColorItemInfo> col = GetRowItems(1);
			return col.Count != 0 ? col[col.Count / 2] : null;
		}
		public ColorItemInfo GetBottomItem() {
			ReadOnlyCollection<ColorItemInfo> col = GetBottomRowItems();
			return col.Count != 0 ? col[col.Count - 1] : null;
		}
		public ColorItemInfo MoveUp() {
			ColorItemInfo colorItem = Owner.ViewInfo.GetHotColorItem();
			if(colorItem == null) return null;
			foreach(ColorItemInfo item in Owner.ViewInfo.GetAllItems()) {
				if(item.Line == colorItem.Line - 1 && item.Bounds.X == colorItem.Bounds.X) return item;
			}
			return null;
		}
		public ColorItemInfo MoveDown() {
			ColorItemInfo colorItem = Owner.ViewInfo.GetHotColorItem();
			if(colorItem == null) return null;
			foreach(ColorItemInfo item in Owner.ViewInfo.GetAllItems()) {
				if(item.Line == colorItem.Line + 1 && item.Bounds.X == colorItem.Bounds.X) return item;
			}
			return null;
		}
		public bool HasSelectedItem() {
			return Owner.ViewInfo.GetHotColorItem() != null;
		}
		public bool HotItemInTopRow() {
			ColorItemInfo colorItem = Owner.ViewInfo.GetHotColorItem();
			return colorItem != null ? colorItem.Line == 1 : false;
		}
		public bool HotItemInLastRow() {
			ColorItemInfo colorItem = Owner.ViewInfo.GetHotColorItem();
			return colorItem != null ? colorItem.Line == GetBottomLine() : false;
		}
		public ColorItemInfo MoveLeft() {
			ColorItemInfo colorItem = Owner.ViewInfo.GetHotColorItem();
			if(colorItem == null) return null;
			ReadOnlyCollection<ColorItemInfo> col = GetRowItems(colorItem.Line);
			int pos = col.IndexOf(colorItem);
			return pos > 0 ? col[pos - 1] : col[col.Count - 1];
		}
		public ColorItemInfo MoveRight() {
			ColorItemInfo colorItem = Owner.ViewInfo.GetHotColorItem();
			if(colorItem == null) return null;
			ReadOnlyCollection<ColorItemInfo> col = GetRowItems(colorItem.Line);
			int pos = col.IndexOf(colorItem);
			return pos == col.Count - 1 ? col[0] : col[pos + 1];
		}
		protected int GetBottomLine() {
			int line = -1;
			foreach(ColorItemInfo colorItem in Owner.ViewInfo.GetAllItems()) {
				if(colorItem.Line > line) line = colorItem.Line;
			}
			return line;
		}
		protected ReadOnlyCollection<ColorItemInfo> GetBottomRowItems() {
			int line = GetBottomLine();
			if(line < 0) return null;
			return GetRowItems(line);
		}
		protected ReadOnlyCollection<ColorItemInfo> GetRowItems(int line) {
			List<ColorItemInfo> list = new List<ColorItemInfo>();
			foreach(ColorItemInfo colorItem in Owner.ViewInfo.GetAllItems()) {
				if(colorItem.Line == line) list.Add(colorItem);
			}
			list.Sort((x, y) => x.Bounds.X.CompareTo(y.Bounds.X));
			return new ReadOnlyCollection<ColorItemInfo>(list);
		}
		public InnerColorPickControl Owner { get { return owner; } }
	}
	public class InnerColorPickControlPainter : BaseControlPainter {
		protected override void DrawContent(ControlGraphicsInfoArgs info) {
			InnerColorPickControlViewInfo viewInfo = (InnerColorPickControlViewInfo)info.ViewInfo;
			base.DrawContent(info);
			if(viewInfo.ShowAutomaticButton) {
				DoDrawAutomaticButton(info);
			}
			if(viewInfo.ShowThemePalette) {
				DoDrawThemeGroup(info);
			}
			DoDrawStandardGroup(info);
			if(viewInfo.ShowRecentPalette) {
				DoDrawRecentGroup(info);
			}
			if(viewInfo.ShowMoreColors) {
				DoDrawMoreButton(info);
			}
		}
		protected virtual void DoDrawAutomaticButton(ControlGraphicsInfoArgs info) {
			InnerColorPickControlViewInfo viewInfo = (InnerColorPickControlViewInfo)info.ViewInfo;
			ObjectPainter.DrawObject(info.Cache, SkinElementPainter.Default, viewInfo.GetButtonElementInfo(viewInfo.AutomaticButtonState, viewInfo.Rects[InnerColorPickControlRect.AutomaticButtonBounds]));
			info.Cache.FillRectangle(info.Cache.GetSolidBrush(viewInfo.AutomatiColor), viewInfo.Rects[InnerColorPickControlRect.AutomaticColorItemBounds]);
			if(viewInfo.AutomaticButtonState.HasFlag(ObjectState.Selected)) {
				DoDrawSelectedItemBorder(info, viewInfo.Rects[InnerColorPickControlRect.AutomaticColorItemBounds]);
			}
			else {
				if(viewInfo.AutomaticBorderColor != Color.Empty) {
					info.Cache.DrawRectangle(info.Cache.GetPen(viewInfo.AutomaticBorderColor), viewInfo.Rects[InnerColorPickControlRect.AutomaticColorItemBounds]);
				}
			}
			viewInfo.PaintAppearance.DrawString(info.Cache, viewInfo.GetAutomaticColorButtonCaption(), viewInfo.Rects[InnerColorPickControlRect.AutomaticButtonCaptionBounds]);
		}
		protected virtual void DoDrawMoreButton(ControlGraphicsInfoArgs info) {
			InnerColorPickControlViewInfo viewInfo = (InnerColorPickControlViewInfo)info.ViewInfo;
			ObjectPainter.DrawObject(info.Cache, SkinElementPainter.Default, viewInfo.GetButtonElementInfo(viewInfo.MoreButtonState, viewInfo.Rects[InnerColorPickControlRect.MoreButtonBounds]));
			info.Cache.Graphics.DrawImage(viewInfo.MoreButtonGlyph, viewInfo.Rects[InnerColorPickControlRect.MoreButtonGlyphBounds]);
			viewInfo.PaintAppearance.DrawString(info.Cache, viewInfo.GetMoreColorButtonCaption(), viewInfo.Rects[InnerColorPickControlRect.MoreButtonCaptionBounds]);
		}
		protected virtual void DoDrawThemeGroup(ControlGraphicsInfoArgs info) {
			InnerColorPickControlViewInfo viewInfo = (InnerColorPickControlViewInfo)info.ViewInfo;
			DoDrawThemeGroupCaption(info);
			DoDrawThemeGroupContent(info);
		}
		protected virtual void DoDrawThemeGroupCaption(ControlGraphicsInfoArgs info) {
			InnerColorPickControlViewInfo viewInfo = (InnerColorPickControlViewInfo)info.ViewInfo;
			DoDrawGroupCaption(info, viewInfo.GetThemePaletteCaption(), viewInfo.Rects[InnerColorPickControlRect.ThemeGroupCaptionTextBounds], viewInfo.Rects[InnerColorPickControlRect.ThemeGroupCaptionBounds]);
		}
		protected virtual void DoDrawThemeGroupContent(ControlGraphicsInfoArgs info) {
			InnerColorPickControlViewInfo viewInfo = (InnerColorPickControlViewInfo)info.ViewInfo;
			foreach(ColorItemInfo itemInfo in viewInfo.ThemeItems) {
				DoDrawColorItem(info, itemInfo);
			}
		}
		protected virtual void DoDrawColorItem(ControlGraphicsInfoArgs info, ColorItemInfo itemInfo) {
			InnerColorPickControlViewInfo viewInfo = (InnerColorPickControlViewInfo)info.ViewInfo;
			info.Cache.FillRectangle(info.Cache.GetSolidBrush(itemInfo.Color), itemInfo.Bounds);
			if(itemInfo.State == ObjectState.Normal) {
				info.Cache.DrawRectangle(info.Cache.GetPen(viewInfo.ItemBorderColor), itemInfo.Bounds);
			}
			else if((itemInfo.State & ObjectState.Hot) != 0) {
				DoDrawHotItemBorder(info, itemInfo.Bounds);
			}
			else {
				DoDrawSelectedItemBorder(info, itemInfo.Bounds);
			}
		}
		protected virtual void DoDrawHotItemBorder(ControlGraphicsInfoArgs info, Rectangle itemBounds) {
			InnerColorPickControlViewInfo viewInfo = (InnerColorPickControlViewInfo)info.ViewInfo;
			info.Cache.DrawRectangle(info.Cache.GetPen(viewInfo.HotItemBorderColor), itemBounds);
			info.Cache.DrawRectangle(info.Cache.GetPen(viewInfo.HotItemInnerBorderColor), Rectangle.Inflate(itemBounds, -1, -1));
		}
		protected virtual void DoDrawSelectedItemBorder(ControlGraphicsInfoArgs info, Rectangle itemBounds) {
			InnerColorPickControlViewInfo viewInfo = (InnerColorPickControlViewInfo)info.ViewInfo;
			info.Cache.DrawRectangle(info.Cache.GetPen(viewInfo.SelectedItemBorderColor), itemBounds);
			info.Cache.DrawRectangle(info.Cache.GetPen(viewInfo.SelectedItemInnerBorderColor), Rectangle.Inflate(itemBounds, -1, -1));
		}
		protected virtual void DoDrawStandardGroup(ControlGraphicsInfoArgs info) {
			DoDrawStandardGroupCaption(info);
			DoDrawStandardGroupContent(info);
		}
		protected virtual void DoDrawStandardGroupCaption(ControlGraphicsInfoArgs info) {
			InnerColorPickControlViewInfo viewInfo = (InnerColorPickControlViewInfo)info.ViewInfo;
			DoDrawGroupCaption(info, viewInfo.GetStandardPaletteCaption(), viewInfo.Rects[InnerColorPickControlRect.StandardGroupCaptionTextBounds], viewInfo.Rects[InnerColorPickControlRect.StandardGroupCaptionBounds]);
		}
		protected virtual void DoDrawStandardGroupContent(ControlGraphicsInfoArgs info) {
			InnerColorPickControlViewInfo viewInfo = (InnerColorPickControlViewInfo)info.ViewInfo;
			foreach(ColorItemInfo itemInfo in viewInfo.StandardItems) {
				DoDrawColorItem(info, itemInfo);
			}
		}
		protected virtual void DoDrawRecentGroup(ControlGraphicsInfoArgs info) {
			DoDrawRecentGroupCaption(info);
			DoDrawRecentGroupContent(info);
		}
		protected virtual void DoDrawRecentGroupCaption(ControlGraphicsInfoArgs info) {
			InnerColorPickControlViewInfo viewInfo = (InnerColorPickControlViewInfo)info.ViewInfo;
			DoDrawGroupCaption(info, viewInfo.GetRecentPaletteCaption(), viewInfo.Rects[InnerColorPickControlRect.RecentGroupCaptionTextBounds], viewInfo.Rects[InnerColorPickControlRect.RecentGroupCaptionBounds]);
		}
		protected virtual void DoDrawGroupCaption(ControlGraphicsInfoArgs info, string caption, Rectangle textBounds, Rectangle bounds) {
			InnerColorPickControlViewInfo viewInfo = (InnerColorPickControlViewInfo)info.ViewInfo;
			ObjectPainter.DrawObject(info.Cache, SkinElementPainter.Default, viewInfo.GetGroupCaptionBackgroundElementInfo(bounds));
			ObjectPainter.DrawObject(info.Cache, SkinElementPainter.Default, viewInfo.GetGroupCaptionElementInfo(bounds));
			viewInfo.PaintAppearanceGroupCaption.DrawString(info.Cache, caption, textBounds);
		}
		protected virtual void DoDrawRecentGroupContent(ControlGraphicsInfoArgs info) {
			InnerColorPickControlViewInfo viewInfo = (InnerColorPickControlViewInfo)info.ViewInfo;
			foreach(ColorItemInfo itemInfo in viewInfo.RecentItems) {
				DoDrawColorItem(info, itemInfo);
			}
		}
	}
}
