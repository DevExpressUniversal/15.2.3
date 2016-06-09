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

using DevExpress.LookAndFeel;
using DevExpress.Skins;
using DevExpress.Utils;
using DevExpress.Utils.Design;
using DevExpress.Utils.Drawing;
using DevExpress.Utils.Drawing.Animation;
using DevExpress.XtraBars.Ribbon;
using DevExpress.XtraBars.Ribbon.Drawing;
using DevExpress.XtraBars.Ribbon.Handler;
using DevExpress.XtraBars.Ribbon.ViewInfo;
using DevExpress.XtraEditors;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Drawing.Design;
using System.Linq;
using System.Text;
using System.Windows.Forms;
namespace DevExpress.XtraBars.Ribbon {
	[ToolboxItem(false), DesignTimeVisible(false)]
	public abstract class RecentItemBase : Component {
		private static readonly object itemClick = new object();
		private static readonly object itemPressed = new object();
		string name;
		bool visible;
		Padding padding;
		Padding margin;
		RecentItemViewInfoBase viewInfo;
		RecentItemPainterBase painter;
		RecentItemHandlerBase handler;
		RecentPanelBase ownerPanel;
		SuperToolTip superTip;
		public RecentItemBase()
			: this(null) {
		}
		public RecentItemBase(RecentPanelBase recentControlPanel) {
			this.visible = true;
			this.padding = Padding.Empty;
			this.margin = Padding.Empty;
		}
		public void SetPanel(RecentPanelBase panel) {
			this.ownerPanel = panel;
			OnOwnerPanelChanged();
		}
		protected virtual void OnOwnerPanelChanged() { }
		protected internal virtual void OnRecentControlChanged(RecentItemControl rc) { }
		public RecentPanelBase OwnerPanel { get { return ownerPanel; } }
		public RecentItemControl RecentControl { get { return OwnerPanel != null ? OwnerPanel.RecentControl : null; } }
		bool ShouldSerializePadding() { return Padding != Padding.Empty; }
		void ResetPadding() { Padding = Padding.Empty; }
		[RefreshProperties(System.ComponentModel.RefreshProperties.All)]
		public Padding Padding {
			get { return padding; }
			set {
				if(Padding == value)
					return;
				padding = value;
				OnItemChanged();
			}
		}
		bool ShouldSerializeMargin() { return Margin != Padding.Empty; }
		void ResetMargin() { Margin = Padding.Empty; }
		[RefreshProperties(System.ComponentModel.RefreshProperties.All)]
		public Padding Margin {
			get { return margin; }
			set {
				if(Margin == value)
					return;
				margin = value;
				OnItemChanged();
			}
		}
		[DefaultValue(true)]
		public bool Visible {
			get { return visible; }
			set {
				if(Visible == value) return;
				visible = value;
				OnItemChanged();
			}
		}
		public string Name {
			get {
				if(Site == null)
					return name;
				return Site.Name;
			}
			set {
				if(value == null)
					value = string.Empty;
				name = value;
			}
		}
		[
		Editor("DevExpress.XtraEditors.Design.ToolTipContainerUITypeEditor, " + AssemblyInfo.SRAssemblyEditorsDesign, typeof(System.Drawing.Design.UITypeEditor)), Category("Appearance"),
		Localizable(true), SmartTagProperty("Super Tip", "Appearance")
		]
		public virtual SuperToolTip SuperTip {
			get { return superTip; }
			set { superTip = value; }
		}
		protected internal bool IsVisible { get { return Visible || DesignMode; } }
		protected internal virtual void OnItemChanged() {
			if(RecentControl != null) {
				RecentControl.GetViewInfo().ResetSplitterPos();
				RecentControl.Refresh();
			}
		}
		protected internal RecentItemViewInfoBase ViewInfo {
			get {
				if(viewInfo == null)
					viewInfo = CreateItemViewInfo();
				return viewInfo;
			}
		}
		protected virtual RecentItemViewInfoBase CreateItemViewInfo() {
			return null;
		}
		protected internal RecentItemPainterBase Painter {
			get {
				if(painter == null)
					painter = CreateItemPainter();
				return painter;
			}
		}
		protected virtual RecentItemPainterBase CreateItemPainter() {
			return null;
		}
		protected internal RecentItemHandlerBase Handler {
			get {
				if(handler == null)
					handler = CreateItemHandler();
				return handler;
			}
		}
		protected virtual RecentItemHandlerBase CreateItemHandler() {
			return null;
		}
		#region Events
		public event RecentItemEventHandler ItemClick {
			add { Events.AddHandler(itemClick, value); }
			remove { Events.RemoveHandler(itemClick, value); }
		}
		public event RecentItemEventHandler ItemPressed {
			add { Events.AddHandler(itemPressed, value); }
			remove { Events.RemoveHandler(itemPressed, value); }
		}
		protected internal void RaiseItemClick() {
			RecentItemEventHandler handler = Events[itemClick] as RecentItemEventHandler;
			if(handler != null)
				handler(this, new RecentItemEventArgs(this));
		}
		protected internal void RaiseItemPressed() {
			RecentItemEventHandler handler = Events[itemPressed] as RecentItemEventHandler;
			if(handler != null)
				handler(this, new RecentItemEventArgs(this));
		}
		#endregion
	}
	[
	SmartTagSupport(typeof(RecentControlTextGlyphItemDesignTimeBoundsProvider),
	SmartTagSupportAttribute.SmartTagCreationMode.Auto)]
	public class RecentTextGlyphItemBase : RecentItemBase {
		const int DefaultGlyphToCaptionIndent = 7;
		BaseAppearanceCollection appearances;
		string caption;
		Image glyph, glyphDisabled, glyphHover, glyphPressed;
		int glyphToCaptionIndent;
		public RecentTextGlyphItemBase() : base() {
			this.glyphToCaptionIndent = DefaultGlyphToCaptionIndent;
			this.appearances = CreateAppearanceCollection();
			this.appearances.Changed += new EventHandler(OnAppearanceChanged);
		}
		protected virtual BaseAppearanceCollection CreateAppearanceCollection() {
			return new BaseRecentItemAppearanceCollection();
		}
		[
		DXCategory(CategoryName.Appearance), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual BaseAppearanceCollection Appearances { get { return appearances; } }
		protected virtual void OnAppearanceChanged(object sender, EventArgs e) {
			ViewInfo.SetAppearanceDirty();
			OnItemChanged();
		}
		[RefreshProperties(System.ComponentModel.RefreshProperties.All),
		SmartTagProperty("Caption", "Appearance", 0, SmartTagActionType.RefreshAfterExecute)]
		public string Caption {
			get { return caption; }
			set {
				if(Caption == value) return;
				caption = value;
				OnItemCaptionChanged();
				OnItemChanged();
			}
		}
		protected virtual void OnItemCaptionChanged() { }
		[ DefaultValue(null), Category("Appearance"), SmartTagProperty("Glyph", "Image", 0, SmartTagActionType.RefreshAfterExecute), Editor("DevExpress.Utils.Design.DXImageEditor, " + AssemblyInfo.SRAssemblyDesign, typeof(UITypeEditor)),
		RefreshProperties(System.ComponentModel.RefreshProperties.All)]
		public virtual Image Glyph {
			get { return glyph; }
			set {
				if(value == Glyph) return;
				glyph = value;
				OnItemGlyphChanged();
				OnItemChanged();
			}
		}
		protected virtual void OnItemGlyphChanged() { }
		[ DefaultValue(null), Category("Appearance"), Editor("DevExpress.Utils.Design.DXImageEditor, " + AssemblyInfo.SRAssemblyDesign, typeof(UITypeEditor))]
		public virtual Image GlyphHover {
			get { return glyphHover; }
			set {
				if(GlyphHover == value) return;
				glyphHover = value;
				OnItemChanged();
			}
		}
		[ DefaultValue(null), Category("Appearance"), Editor("DevExpress.Utils.Design.DXImageEditor, " + AssemblyInfo.SRAssemblyDesign, typeof(UITypeEditor))]
		public virtual Image GlyphPressed {
			get { return glyphPressed; }
			set {
				if(GlyphPressed == value) return;
				glyphPressed = value;
				OnItemChanged();
			}
		}
		[ DefaultValue(null), Category("Appearance"), Editor("DevExpress.Utils.Design.DXImageEditor, " + AssemblyInfo.SRAssemblyDesign, typeof(UITypeEditor))]
		public virtual Image GlyphDisabled {
			get { return glyphDisabled; }
			set {
				if(GlyphDisabled == value) return;
				glyphDisabled = value;
				OnItemChanged();
			}
		}
		[DefaultValue(DefaultGlyphToCaptionIndent), RefreshProperties(System.ComponentModel.RefreshProperties.All)]
		public int GlyphToCaptionIndent {
			get { return glyphToCaptionIndent; }
			set {
				if(GlyphToCaptionIndent == value) return;
				glyphToCaptionIndent = value;
				OnItemChanged();
			}
		}
		protected override RecentItemViewInfoBase CreateItemViewInfo() {
			return new RecentTextGlyphItemViewInfoBase(this);
		}
		protected override RecentItemPainterBase CreateItemPainter() {
			return new RecentTextGlyphItemPainterBase();
		}
		protected override void Dispose(bool disposing) {
			if(disposing)
				appearances.Changed -= new EventHandler(OnAppearanceChanged);
			base.Dispose(disposing);
		}
	}
	public class RecentItemCollection : CollectionBase {
		RecentPanelBase panel;
		int lockUpdate;
		public RecentItemCollection(RecentPanelBase panel) {
			this.panel = panel;
			this.lockUpdate = 0;
		}
		public RecentItemControl Control { get { return panel.RecentControl; } }
		public void Add(RecentItemBase item) { List.Add(item); }
		public virtual void AddRange(IEnumerable items) {
			BeginUpdate();
			try {
				foreach(RecentItemBase item in items) Add(item);
			}
			finally {
				EndUpdate();
			}
		}
		public void Insert(int index, RecentItemBase item) { List.Insert(index, item); }
		public void Remove(RecentItemBase item) { 
			List.Remove(item);
		}
		public int IndexOf(RecentItemBase item) { return List.IndexOf(item); }
		public bool Contains(RecentItemBase item) { return List.Contains(item); }
		public RecentItemBase this[int index] { get { return (RecentItemBase)List[index]; } set { List[index] = value; } }
		public virtual void BeginUpdate() { lockUpdate++; }
		public virtual void EndUpdate() {
			if(--lockUpdate == 0) {
				RaiseListChanged(new ListChangedEventArgs(ListChangedType.Reset, -1));
			}
		}
		protected virtual void RaiseListChanged(ListChangedEventArgs e) {
			if(lockUpdate != 0) return;
			if(ListChanged != null) ListChanged(this, e);
		}
		public event ListChangedEventHandler ListChanged;
		protected override void OnInsertComplete(int position, object value) {
			base.OnInsertComplete(position, value);
			((RecentItemBase)value).SetPanel(panel);
			RaiseListChanged(new ListChangedEventArgs(ListChangedType.ItemAdded, position));
		}
		protected override void OnRemoveComplete(int position, object value) {
			base.OnRemoveComplete(position, value);
			if(panel.RecentControl != null)
				if(panel.RecentControl.SelectedTab == value)
					panel.RecentControl.UpdateSelectedTabToDefault();
			((RecentItemBase)value).SetPanel(null);
			RaiseListChanged(new ListChangedEventArgs(ListChangedType.ItemDeleted, position));
		}
		protected override void OnClearComplete() {
			base.OnClearComplete();
			RaiseListChanged(new ListChangedEventArgs(ListChangedType.Reset, -1));
		}
		protected override void OnSetComplete(int index, object oldValue, object newValue) {
			base.OnSetComplete(index, oldValue, newValue);
			RaiseListChanged(new ListChangedEventArgs(ListChangedType.ItemChanged, index));
		}
	}
	[ToolboxItem(false), DesignTimeVisible(false),
	SmartTagSupport(typeof(RecentControlPanelDesignTimeBoundsProvider), 
	SmartTagSupportAttribute.SmartTagCreationMode.Auto),
	SmartTagAction(typeof(RecentControlPanelDesignTimeActionsProvider), "AddLabelItem", "Add LabelItem"),
	SmartTagAction(typeof(RecentControlPanelDesignTimeActionsProvider), "AddButtonItem", "Add ButtonItem"),
	SmartTagAction(typeof(RecentControlPanelDesignTimeActionsProvider), "AddSeparatorItem", "Add SeparatorItem"),
	SmartTagAction(typeof(RecentControlPanelDesignTimeActionsProvider), "AddRecentItem", "Add PinItem"),
	SmartTagAction(typeof(RecentControlPanelDesignTimeActionsProvider), "AddTabItem", "Add TabItem"),
	SmartTagAction(typeof(RecentControlPanelDesignTimeActionsProvider), "AddContainerItem", "Add ContainerItem"),
	SmartTagAction(typeof(RecentControlPanelDesignTimeActionsProvider), "AddHyperLinkItem", "Add HyperLinkItem"),
	]
	public abstract class RecentPanelBase : Component {
		static readonly Padding DefaultPanelPadding = new Padding(20);
		const int DefaultGlyphToCaptionIndent = 11;
		const int DefaultIndentBetweenItems = 5;
		const int DefaultCaptionToContentIndent = 3;
		RecentItemCollection items;
		RecentItemControl recentControl;
		RecentPanelViewInfoBase viewInfo;
		RecentPanelPainterBase painter;
		string caption;
		bool showCaption;
		Padding panelPadding;
		AppearanceObject appearance;
		int indentBetweenItems;
		Image glyph;
		int glyphToCaptionIndent;
		int captionToContentIndent;
		bool movePinnedItemsUp;
		public RecentPanelBase() {
			this.caption = string.Empty;
			this.showCaption = true;
			this.items = new RecentItemCollection(this);
			this.panelPadding = DefaultPanelPadding;
			this.appearance = CreateAppearance();
			this.items.ListChanged += OnItemsListChanged;
			this.indentBetweenItems = DefaultIndentBetweenItems;
			this.glyphToCaptionIndent = DefaultGlyphToCaptionIndent;
			this.captionToContentIndent = DefaultCaptionToContentIndent;
			this.movePinnedItemsUp = true;
		}
		public void SetOwnerControl(RecentItemControl rc) {
			this.recentControl = rc;
			OnRecentControlChanged(this.recentControl);
		}
		void OnRecentControlChanged(RecentItemControl rc) {
			foreach(RecentItemBase item in Items) {
				item.OnRecentControlChanged(rc);
			}
		}
		void UpdateItemsSizes() {
			foreach(RecentItemBase item in Items) {
				RecentButtonItem buttonItem = item as RecentButtonItem;
				if(buttonItem != null)
					buttonItem.UpdateSize();
			 }
		}
		void OnItemsListChanged(object sender, ListChangedEventArgs e) {
			OnPanelChanged();
		}
		protected virtual AppearanceObject CreateAppearance() {
			AppearanceObject res = new AppearanceObject();
			res.Changed += new EventHandler(OnAppearanceChanged);
			return res;
		}
		protected virtual void OnAppearanceChanged(object sender, EventArgs e) {
			ViewInfo.SetAppearanceDirty();
			OnPanelChanged();
		}
		internal void ResetAppearance() { Appearance.Reset(); }
		internal bool ShouldSerializeAppearance() { return Appearance != null && Appearance.Options != AppearanceOptions.Empty; }
		[ Category("Appearance"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual AppearanceObject Appearance {
			get { return appearance; }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public RecentItemCollection Items { get { return items; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public RecentItemControl RecentControl { get { return recentControl; } }
		[DefaultValue(DefaultGlyphToCaptionIndent)]
		public int GlyphToCaptionIndent {
			get { return glyphToCaptionIndent; }
			set {
				if(GlyphToCaptionIndent == value) return;
				glyphToCaptionIndent = value;
				OnPanelChanged();
			}
		}
		[ DefaultValue(null), Category("Appearance"), SmartTagProperty("Glyph", "Image", 0, SmartTagActionType.RefreshAfterExecute), Editor("DevExpress.Utils.Design.DXImageEditor, " + AssemblyInfo.SRAssemblyDesign, typeof(UITypeEditor))]
		public Image Glyph {
			get { return glyph; }
			set {
				if(value == Glyph) return;
				glyph = value;
				OnPanelChanged();
			}
		}
		[DefaultValue("")]
		public string Caption {
			get { return caption; }
			set {
				if(Caption == value) return;
				caption = value;
				OnPanelChanged();
			}
		}
		[DefaultValue(true)]
		public bool ShowCaption {
			get { return showCaption; }
			set {
				if(ShowCaption == value) return;
				showCaption = value;
				OnPanelChanged();
			}
		}
		[DefaultValue(DefaultIndentBetweenItems)]
		public int IndentBetweenItems {
			get { return indentBetweenItems; }
			set {
				if(IndentBetweenItems == value) return;
				indentBetweenItems = value;
				OnPanelChanged();
			}
		}
		[DefaultValue(DefaultCaptionToContentIndent)]
		public int CaptionToContentIndent {
			get { return captionToContentIndent; }
			set {
				if(CaptionToContentIndent == value) return;
				captionToContentIndent = value;
				OnPanelChanged();
			}
		}
		[DefaultValue(true)]
		public bool MovePinnedItemsUp {
			get { return movePinnedItemsUp; }
			set {
				if(MovePinnedItemsUp == value) return;
				movePinnedItemsUp = value;
			}
		}
		bool ShouldSerializePanelPadding() { return PanelPadding != DefaultPanelPadding; }
		void ResetPanelPadding() { PanelPadding = DefaultPanelPadding; }
		public Padding PanelPadding {
			get { return panelPadding; }
			set {
				if(PanelPadding == value)
					return;
				panelPadding = value;
				OnPanelChanged();
			}
		}
		protected internal bool IsViewPanel {
			get { 
				if(RecentControl == null) return false;
				return RecentControl.ContentPanel == this;
			}
		}
		protected internal virtual void OnPanelChanged() {
			if(RecentControl != null)
				RecentControl.Refresh();
		}
		protected internal RecentPanelViewInfoBase ViewInfo {
			get {
				if(viewInfo == null)
					viewInfo = CreatePanelViewInfo();
				return viewInfo;
			}
		}
		protected virtual RecentPanelViewInfoBase CreatePanelViewInfo() {
			return null;
		}
		protected internal RecentPanelPainterBase Painter {
			get {
				if(painter == null)
					painter = CreatePanelPainter();
				return painter;
			}
		}
		protected virtual RecentPanelPainterBase CreatePanelPainter() {
			return new RecentPanelPainterBase();
		}
		protected override void Dispose(bool disposing) {
			if(disposing) {
				items.ListChanged -= OnItemsListChanged;
				ClearItems();
			}
			base.Dispose(disposing);
		}
		void ClearItems() {
			RecentItemBase[] itemsArray = new RecentItemBase[Items.Count];
			for(int i = 0; i < Items.Count; i++) {
				itemsArray[i] = Items[i];
			}
			for(int i = 0; i < itemsArray.Length; i++) {
				itemsArray[i].Dispose();
			}
		}
		internal void UpdateCaption(Image glyph, string caption) {
			this.glyph = glyph;
			this.caption = caption;
		}
		internal void HidePanel() {
			foreach(RecentItemBase item in Items) {
				RecentControlContainerItem containerItem = item as RecentControlContainerItem;
				if(containerItem != null)
					containerItem.ControlContainer.Visible = false;
			}
		}
		internal void ShowPanel() {
			foreach(RecentItemBase item in Items) {
				RecentControlContainerItem containerItem = item as RecentControlContainerItem;
				if(containerItem != null)
					containerItem.ControlContainer.Visible = true;
			}
		}
	}
}
namespace DevExpress.XtraBars.Ribbon.ViewInfo{
	public class RecentTextGlyphItemViewInfoBase : RecentItemViewInfoBase {
		BaseAppearanceCollection paintAppearance;
		bool paintAppearanceDirty;
		public RecentTextGlyphItemViewInfoBase(RecentTextGlyphItemBase item)
			: base(item) {
			this.paintAppearance = CreatePaintAppearances();
			this.paintAppearanceDirty = true;
		}
		public new RecentTextGlyphItemBase Item { get { return base.Item as RecentTextGlyphItemBase; } }
		public Size CaptionTextSize { get; private set; }
		public Size GlyphSize { get; private set; }
		public Rectangle CaptionTextBounds { get; private set; }
		public Rectangle GlyphBounds { get; private set; }
		public virtual bool IsHovered { get { return Item.OwnerPanel.ViewInfo.HotItem == this; } }
		public virtual bool IsPressed { get { return Item.OwnerPanel.ViewInfo.PressedItem == this; } }
		public virtual bool IsSelected { get { return Item.OwnerPanel.ViewInfo.SelectedItem == this; } }
		protected override Size CalcBestSizeCore(int width) {
			CalcTextSizes(width);
			GlyphSize = CalcGlyphSize();
			return new Size(CaptionTextSize.Width + Item.GlyphToCaptionIndent + GlyphSize.Width, Math.Max(CaptionTextSize.Height, GlyphSize.Height));
		}
		protected virtual void CalcTextSizes(int width) {
			CaptionTextSize = CalcCaptionTextSizeCore(width);
		}
		protected Size CalcCaptionTextSizeCore(int width) {
			Size res = Size.Empty;
			GraphicsInfo gInfo = new GraphicsInfo();
			gInfo.AddGraphics(null);
			try {
				res = GetPaintAppearance("ItemNormal").CalcTextSizeInt(gInfo.Graphics, Item.Caption, width);
			}
			finally {
				gInfo.ReleaseGraphics();
			}
			return res;
		}
		protected virtual Size CalcGlyphSize() {
			if(Item.Glyph != null)
				return Item.Glyph.Size;
			return Size.Empty;
		}
		protected override Padding GetSkinItemPadding() {
			return new Padding(5);
		}
		protected override void ClearBounds() {
			GlyphBounds = Rectangle.Empty;
			CaptionTextBounds = Rectangle.Empty;
		}
		public override void CalcViewInfo(Rectangle bounds) {
			base.CalcViewInfo(bounds);
			if(ContentBounds.Width <= 0) return;
			GlyphBounds = CalcGlyphBounds();
			CaptionTextBounds = CalcCaptionTextBounds();
		}
		protected virtual Rectangle CalcGlyphBounds() {
			if(Item.Glyph == null) return Rectangle.Empty;
			Rectangle rect = Rectangle.Empty;
			rect.X = ContentBounds.X;
			rect.Size = new Size(Math.Min(GlyphSize.Width, ContentBounds.Right - rect.X), GlyphSize.Height);
			rect.Y = ContentBounds.Y;
			if(CaptionTextSize.Height > GlyphSize.Height)
				rect.Y += (CaptionTextSize.Height - GlyphSize.Height) / 2;
			return rect;
		}
		protected virtual Rectangle CalcCaptionTextBounds() {
			Rectangle rect = new Rectangle();
			rect.X = GlyphBounds == Rectangle.Empty ? ContentBounds.X : GlyphBounds.Right + Item.GlyphToCaptionIndent;
			rect.Width = ContentBounds.Right - rect.X;
			rect.Height = ContentBounds.Height;
			rect.Y = ContentBounds.Y;
			return rect;
		}
		protected virtual ObjectState CalcItemState() {
			if(IsPressed) return ObjectState.Pressed;
			if(IsSelected) return ObjectState.Selected;
			if(IsHovered) return ObjectState.Hot;
			return ObjectState.Normal;
		}
		public override SkinElementInfo GetItemInfo() {
			SkinElement element = RibbonSkins.GetSkin(Item.RecentControl.LookAndFeel.ActiveLookAndFeel)[RibbonSkins.SkinGalleryButton];
			if(element == null)
				element = RibbonSkins.GetSkin(DevExpress.XtraEditors.Controls.DefaultSkinProvider.Default)[RibbonSkins.SkinGalleryButton];
			SkinElementInfo info = new SkinElementInfo(element, Bounds);
			info.ImageIndex = -1;
			info.State = CalcItemState();
			return info;
		}
		protected internal override int CalcMinWidth() {
			int min = base.CalcMinWidth();
			min += GlyphSize.Width;
			if(Item.Glyph != null) min += Item.GlyphToCaptionIndent;
			min += CalcTextMinWidth(GetPaintAppearance("ItemNormal"));
			return min;
		}
		int CalcTextMinWidth(AppearanceObject app) {
			int width = 0;
			GraphicsInfo gInfo = Item.RecentControl.GetViewInfo().GInfo;
			if(gInfo.Graphics == null) {
				gInfo.AddGraphics(null);
				try {
					width = app.CalcTextSizeInt(gInfo.Graphics, "Wg", 0).Width;
				}
				catch {
					gInfo.ReleaseGraphics();
				}
			}
			else width = app.CalcTextSizeInt(Item.RecentControl.GetViewInfo().GInfo.Graphics, "Wg", 0).Width;
			return width;
		}
		protected virtual BaseAppearanceCollection CreatePaintAppearances() {
			return new BaseRecentItemAppearanceCollection();
		}
		protected internal override void SetAppearanceDirty() {
			this.paintAppearanceDirty = true;
		}
		public BaseAppearanceCollection PaintAppearance {
			get {
				if(this.paintAppearanceDirty) {
					UpdatePaintAppearance();
				}
				return paintAppearance;
			}
		}
		void UpdatePaintAppearance() {
			this.paintAppearance.Combine(ItemAppearanceCollection, GetAppearanceDefaultInfo(), true);
					this.paintAppearanceDirty = false;
		}
		protected virtual void UpdateDefaults(AppearanceObject app){
			app.TextOptions.Trimming = Trimming.EllipsisCharacter;
		}
		protected virtual BaseRecentItemAppearanceCollection ItemAppearanceCollection { get { return Item.Appearances as BaseRecentItemAppearanceCollection; } }
		protected virtual BaseRecentItemAppearanceCollection ControlAppearances { get { return null; } }
		protected AppearanceObject GetPaintAppearance(string name) {
			AppearanceObject app = new AppearanceObject();
			AppearanceHelper.Combine(app, new AppearanceObject[] { PaintAppearance.GetAppearance(name), ControlAppearances.GetAppearance(name) });
			UpdateDefaults(app);
			return app;
		}
		protected internal AppearanceObject GetPaintAppearanceByState() {
			if(IsPressed) return GetPaintAppearance("ItemPressed");
			if(IsHovered) return GetPaintAppearance("ItemHovered");
			return GetPaintAppearance("ItemNormal");
		}
		protected virtual AppearanceDefaultInfo[] GetAppearanceDefaultInfo() {
			AppearanceDefault appearance = new AppearanceDefault();
			ApplyBaseForeColor(appearance);
			appearance.Font = new Font("SegoeUI", 11.0f, FontStyle.Regular);
			return new AppearanceDefaultInfo[]{
				new AppearanceDefaultInfo("ItemNormal", appearance),
				new AppearanceDefaultInfo("ItemHovered", appearance),
				new AppearanceDefaultInfo("ItemPressed", appearance)
			};
		}
		protected void ApplyBaseForeColor(AppearanceDefault app) {
			SkinElement elem = Item.RecentControl.GetViewInfo().GetRibbonSkinElement(RibbonSkins.SkinPopupGalleryItemCaption);
			if(elem != null) {
				elem.ApplyForeColorAndFont(app);
			}
		}
	}
	public abstract class RecentPanelViewInfoBase {
		RecentPanelBase panel;
		Rectangle bounds;
		Rectangle contentBounds;
		AppearanceObject paintAppearance;
		RecentItemViewInfoBase hotItem, selectedItem, pressedItem;
		bool showScrollBar;
		public RecentPanelViewInfoBase(RecentPanelBase panel) {
			this.panel = panel;
			this.showScrollBar = false;
		}
		bool ShowScrollBar { get { return Panel.IsViewPanel; } }
		protected internal RecentItemViewInfoBase HotItem {
			get { return hotItem; }
			set {
				if(HotItem == value) return;
				RecentItemViewInfoBase prev = HotItem;
				hotItem = value;
				OnHotItemChanged(prev);
			}
		}
		protected internal RecentItemViewInfoBase PressedItem {
			get { return pressedItem; }
			set {
				if(PressedItem == value)
					return;
				RecentItemViewInfoBase prev = PressedItem;
				pressedItem = value;
				OnHotItemChanged(prev);
			}
		}
		protected internal virtual RecentItemViewInfoBase SelectedItem {
			get { return selectedItem; }
			set {
				if(SelectedItem == value)
					return;
				RecentItemViewInfoBase prev = SelectedItem;
				selectedItem = value;
				OnSelectedItemChanged(prev);
			}
		}
		void OnHotItemChanged(RecentItemViewInfoBase prev) {
			if(prev != null) {
				prev.SetAppearanceDirty();
				Invalidate(prev);
			}
			if(HotItem != null) {
				HotItem.SetAppearanceDirty();
				Invalidate(HotItem);
			}
		}
		private void Invalidate(RecentItemViewInfoBase item) {
			if(item == null)
				return;
			Panel.RecentControl.Invalidate(item.Bounds);
		}
		void OnPressedItemChanged(RecentItemViewInfoBase prev) {
			if(prev != null) {
				prev.SetAppearanceDirty();
				Invalidate(prev);
			}
			if(PressedItem != null) {
				PressedItem.SetAppearanceDirty();
				Invalidate(HotItem);
			}
		}
		void OnSelectedItemChanged(RecentItemViewInfoBase prev) {
			RecentTabItem tabItem = SelectedItem == null ? null : SelectedItem.Item as RecentTabItem;
			if(tabItem != null)
				if(Panel.RecentControl.SelectedTab != tabItem)
					Panel.RecentControl.SelectedTab = tabItem;
			if(prev != null) {
				prev.SetAppearanceDirty();
				Invalidate(prev);
			}
			if(SelectedItem != null) {
				SelectedItem.SetAppearanceDirty();
				Invalidate(SelectedItem);
			}
		}
		public RecentPanelBase Panel { get { return panel; } }
		public Rectangle Bounds { get { return bounds; } }
		public Rectangle ContentBounds { get { return contentBounds; } }
		public Rectangle PanelContentRect { get; private set; }
		public Rectangle CaptionBounds { get; private set; }
		public Rectangle CaptionAreaBounds { get; private set; }
		public Rectangle GlyphBounds { get; private set; }
		public Size GlyphSize { get; private set; }
		public Size CaptionSize { get; private set; }
		public int MinWidth { get; private set; }
		protected internal Rectangle DropBounds {
			get { return PanelContentRect; }
		}
		public bool IsAnimationActive {
			get { return Panel.RecentControl.ContentPanel == Panel ? Panel.RecentControl.GetViewInfo().IsAnimation : false; }
		}
		protected virtual Padding Padding {
			get {
				if(Panel.PanelPadding != Padding.Empty) return Panel.PanelPadding;
				return GetSkinPanelPadding();
			}
		}
		protected virtual Padding GetSkinPanelPadding() {
			return new Padding(42, 1, 0, 0);
		}
		public int ItemsIndent { get { return Panel.IndentBetweenItems; } }
		AppearanceDefault defaultAppearance = null;
		public virtual AppearanceDefault DefaultAppearance {
			get {
				if(defaultAppearance == null)
					defaultAppearance = CreateDefaultAppearance();
				return defaultAppearance;
			}
		}
		protected AppearanceDefault CreateDefaultAppearance() {
			AppearanceDefault appearance = new AppearanceDefault();
			SkinElement element = Panel.RecentControl.MainPanel == Panel ? Panel.RecentControl.GetViewInfo().GetRibbonSkinElement(RibbonSkins.SkinPopupGalleryGroupCaption) :
				Panel.RecentControl.GetViewInfo().GetRibbonSkinElement(RibbonSkins.SkinTabHeaderPage);
			if(element != null) {
				element.ApplyForeColorAndFont(appearance);
			}
			appearance.Font = new Font("Segoe UI Light", 24F);
			if(Panel.RecentControl.MainPanel != Panel) appearance.ForeColor = RibbonSkins.GetSkin(Panel.RecentControl.GetViewInfo().LookAndFeel.ActiveLookAndFeel)[RibbonSkins.SkinTabHeaderPage].GetForeColor(ObjectState.Selected);
			appearance.FontStyleDelta = FontStyle.Regular;
			return appearance;
		}
		public virtual AppearanceObject PaintAppearance {
			get {
				if(paintAppearance == null) {
					paintAppearance = CreatePaintAppearance();
					UpdatePaintAppearance();
				}
				return paintAppearance;
			}
		}
		protected virtual AppearanceObject CreatePaintAppearance() {
			return new AppearanceObject(DefaultAppearance);
		}
		void OnPaintAppearanceChanged() {
			Panel.OnPanelChanged();
		}
		public void UpdatePaintAppearance() {
			AppearanceHelper.Combine(PaintAppearance, new AppearanceObject[] { Panel.Appearance, (Panel.RecentControl.Appearances as RecentAppearanceCollection).PanelCaption}, DefaultAppearance);
		}
		public virtual Size CalcBestSize() {
			return Size.Empty;
		}
		public virtual void CalcViewInfo(Rectangle bounds) {
			this.bounds = bounds;
			PanelContentRect = CalcPanelContentRect(bounds);
			GlyphSize = CalcGlyphSize();
			CaptionSize = CalcCaptionSize();
			CaptionAreaBounds = CalcCaptionAreaBounds();
			GlyphBounds = CalcGlyphBounds();
			CaptionBounds = CalcCaptionBounds();
			CalcContentBounds();
		}
		Size CalcCaptionSize() {
			if(Panel.Caption == string.Empty) return Size.Empty;
			return PaintAppearance.CalcTextSizeInt(Panel.RecentControl.GetViewInfo().GInfo.Graphics, Panel.Caption, 0);
		}
		Size CalcGlyphSize() {
			if(Panel.Glyph != null)
				return Panel.Glyph.Size;
			return Size.Empty;
		}
		Rectangle CalcCaptionAreaBounds() {
			Rectangle rect = Rectangle.Empty;
			rect.X = PanelContentRect.X;
			rect.Y = PanelContentRect.Y - Panel.RecentControl.GetViewInfo().VScrollOffset;
			rect.Width = PanelContentRect.Width;
			rect.Height = Math.Max(GlyphSize.Height, CaptionSize.Height);
			return rect;
		}
		Rectangle CalcPanelContentRect(Rectangle bounds) {
			bounds.Width -= Padding.Horizontal;
			bounds.Height -= Padding.Vertical;
			bounds.X += Padding.Left;
			bounds.Y += Padding.Top;
			if(!IsAnimationActive) return bounds;
			RecentTransitionAnimationInfo info = (RecentTransitionAnimationInfo)XtraAnimator.Current.Get(Panel.RecentControl, Panel.RecentControl.AnimationId);
			if(info == null) return bounds;
			int left = info.CurrentPos;
			return new Rectangle(left + Padding.Left, bounds.Y, bounds.Width, bounds.Height);
		}
		protected internal virtual int CalcMinWidth() {
			int min = 0;
			foreach(RecentItemBase item in Panel.Items) {
				min = Math.Max(item.ViewInfo.CalcMinWidth() + item.Margin.Horizontal, min);
			}
			return Padding.Horizontal + min;
		}
		protected virtual Rectangle CalcGlyphBounds() {
			if(!Panel.ShowCaption) return Rectangle.Empty;
			Rectangle rect = Rectangle.Empty;
			rect.Size = GlyphSize;
			rect.X = CaptionAreaBounds.X;
			rect.Y = CaptionAreaBounds.Y + (CaptionAreaBounds.Height - GlyphSize.Height) / 2;
			return rect;
		}
		protected virtual void CalcContentBounds() {
			Rectangle rect = Rectangle.Empty;
			rect.X = PanelContentRect.X;
			rect.Width = PanelContentRect.Width;
			rect.Height = PanelContentRect.Height - CaptionAreaBounds.Height;
			rect.Y = PanelContentRect.Y;
			if(CaptionAreaBounds.Height != 0) {
				rect.Height -= Panel.CaptionToContentIndent;
				rect.Y += Panel.CaptionToContentIndent;
			}
			if(Panel.ShowCaption)
				rect.Y += CaptionAreaBounds.Height;
			this.contentBounds = rect;
		}
		Rectangle CalcCaptionBounds() {
			if(!Panel.ShowCaption) return Rectangle.Empty;
			Rectangle rect = Rectangle.Empty;
			rect.X = GlyphBounds == Rectangle.Empty ? CaptionAreaBounds.X : GlyphBounds.Right + Panel.GlyphToCaptionIndent;
			rect.Y = CaptionAreaBounds.Y + (CaptionAreaBounds.Height - CaptionSize.Height) / 2;
			rect.Size = CaptionSize;
			return rect;
		}
		public bool ShowCaption { get { return Panel.ShowCaption; } }
		internal RecentControlHitInfo CalcHitInfo(RecentControlHitInfo hitInfo) {
			foreach(RecentItemBase item in Panel.Items) {
				if(!item.Visible) continue;
				if(hitInfo.ContainsSet(item.ViewInfo.Bounds, RecentControlHitTest.Item)) {
					hitInfo.Item = item;
					return item.ViewInfo.CalcHitInfo(hitInfo);
				}
			}
			return hitInfo;
		}
		protected internal virtual RecentItemBase GetItemByLocation(Point p) {
			foreach(RecentItemBase item in Panel.Items) {
				if(item.ViewInfo.Bounds.Contains(p)) return item;
			}
			return null;
		}
		internal void InvalidateControlContainers() {
			foreach(RecentItemBase item in Panel.Items) {
				RecentControlContainerItem containerItem = item as RecentControlContainerItem;
				if(containerItem != null) {
					if(containerItem.ControlContainer != null && containerItem.ControlContainer.Visible)
						containerItem.ControlContainer.Invalidate();
				}
			}
		}
		internal void SetAppearanceDirty() {
			this.paintAppearance = null;
			this.defaultAppearance = null;
			foreach(RecentItemBase item in Panel.Items)
				item.ViewInfo.SetAppearanceDirty();
		}
	}
	public abstract class RecentItemViewInfoBase {
		RecentItemBase item;
		Rectangle bounds;
		Rectangle contentBounds;
		public RecentItemViewInfoBase(RecentItemBase item) {
			this.item = item;
		}
		public RecentItemBase Item { get { return item; } }
		public Rectangle Bounds { get { return bounds; } }
		public Rectangle ContentBounds { get { return this.contentBounds; } }
		public Size BestSize { get; set; }
		protected virtual Padding ControlItemPadding { get { return Padding.Empty; } }
		protected virtual Padding ItemPadding { get { return Item.Padding; } }
		protected Padding Padding {
			get {
				if(ItemPadding != Padding.Empty) return ItemPadding;
				if(ControlItemPadding != Padding.Empty) return ControlItemPadding;
				return GetSkinItemPadding();
			}
		}
		protected internal virtual void SetAppearanceDirty() { }
		protected virtual Padding GetSkinItemPadding() {
			return new Padding(0);
		}
		public virtual Size CalcBestSize(int width) {
			Size size = CalcBestSizeCore(width);
			SkinElementInfo info = GetItemInfo();
			BestSize = ApplyPaddings(SizeFromContentSize(size));
			return BestSize;
		}
		protected virtual Size CalcBestSizeCore(int width) {
			return Size.Empty;
		}
		protected virtual Size SizeFromContentSize(Size size) {
			SkinElementInfo info = GetItemInfo();
			if(info == null) return size;
			return ObjectPainter.CalcBoundsByClientRectangle(Item.RecentControl.GetViewInfo().GInfo.Graphics, SkinElementPainter.Default, info, new Rectangle(Point.Empty, size)).Size;
		}
		public virtual SkinElementInfo GetItemInfo() {
			return null;
		}
		protected virtual Size ApplyPaddings(Size size) {
			return new Size(size.Width + Padding.Horizontal, size.Height + Padding.Vertical);
		}
		public virtual void CalcViewInfo(Rectangle bounds) {
			this.bounds = bounds;
			ClearBounds();
			this.contentBounds = CalcContentBounds();
		}
		protected virtual void ClearBounds() {
			this.contentBounds = Rectangle.Empty;
		}
		protected virtual Rectangle CalcContentBounds() {
			Rectangle rect = CalcSkinContentBounds();
			rect.X = rect.X + Padding.Left;
			rect.Y = rect.Y + Padding.Top;
			rect.Width = rect.Width - Padding.Horizontal;
			rect.Height = rect.Height - Padding.Vertical;
			return rect;
		}
		protected virtual Rectangle CalcSkinContentBounds() {
			SkinElementInfo info = GetItemInfo();
			if(info == null) return Bounds;
			return ObjectPainter.GetObjectClientRectangle(Item.RecentControl.GetViewInfo().GInfo.Graphics, SkinElementPainter.Default, info);
		}
		public RecentControlViewInfo RecentViewInfo { get { return Item.RecentControl.GetViewInfo(); } }
		protected internal virtual RecentControlHitInfo CalcHitInfo(RecentControlHitInfo hitInfo) {
			return hitInfo;
		}
		protected internal virtual int CalcMinWidth() {
			return Padding.Horizontal;
		}
	}
}
namespace DevExpress.XtraBars.Ribbon.Drawing{
	public abstract class RecentItemPainterBase {
		public virtual void Draw(GraphicsCache cache, RecentItemViewInfoBase itemBaseInfo) {
			DrawBackground(cache, itemBaseInfo);
			DrawItemCore(cache, itemBaseInfo);
			DrawItemSelection(cache, itemBaseInfo);
		}
		protected virtual void DrawItemCore(GraphicsCache cache, RecentItemViewInfoBase itemBaseInfo) { }
		protected virtual void DrawBackground(GraphicsCache cache, RecentItemViewInfoBase itemBaseInfo) {
			ObjectPainter.DrawObject(cache, SkinElementPainter.Default, itemBaseInfo.GetItemInfo());
		}
		protected virtual void DrawItemSelection(GraphicsCache cache, RecentItemViewInfoBase itemInfo) {
			if(itemInfo.RecentViewInfo.DesignTimeManager.IsComponentSelected(itemInfo.Item))
				itemInfo.RecentViewInfo.DesignTimeManager.DrawSelection(cache, itemInfo.Bounds, Color.Magenta);
		}
	}
	public class RecentTextGlyphItemPainterBase : RecentItemPainterBase {
		protected override void DrawItemCore(GraphicsCache cache, RecentItemViewInfoBase itemBaseInfo) {
			RecentTextGlyphItemViewInfoBase textInfo = itemBaseInfo as RecentTextGlyphItemViewInfoBase;
			base.DrawItemCore(cache, itemBaseInfo);
			DrawGlyph(cache, textInfo);
			DrawText(cache, textInfo);
		}
		protected virtual void DrawText(GraphicsCache cache, RecentTextGlyphItemViewInfoBase labelInfo) {
			labelInfo.GetPaintAppearanceByState().DrawString(cache, labelInfo.Item.Caption, labelInfo.CaptionTextBounds);
		}
		void DrawGlyph(GraphicsCache cache, RecentTextGlyphItemViewInfoBase labelInfo) {
			if(labelInfo.Item.Glyph != null)
				cache.Graphics.DrawImage(labelInfo.Item.Glyph, labelInfo.GlyphBounds.Location);
		}
	}
}
namespace DevExpress.XtraBars.Ribbon.Handler {
	public abstract class RecentItemHandlerBase {
		RecentItemBase item;
		public RecentItemHandlerBase(RecentItemBase item) {
			this.item = item;
		}
		public RecentItemBase Item { get { return item; } }
		public virtual bool OnMouseMove(MouseEventArgs e) {
			return false;
		}
		public virtual bool OnMouseDown(MouseEventArgs e) {
			return false;
		}
		public virtual bool OnMouseUp(MouseEventArgs e) {
			return false;
		}
		public virtual bool OnMouseClick(MouseEventArgs e) {
			return false;
		}
	}
}
