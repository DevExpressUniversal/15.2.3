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

using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraEditors.ViewInfo;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors.Controls;
using DevExpress.Utils.Text;
using DevExpress.Skins;
using DevExpress.LookAndFeel;
using DevExpress.XtraEditors.Drawing;
using DevExpress.XtraPrinting;
using DevExpress.XtraEditors.ToolboxIcons;
namespace DevExpress.XtraEditors.Repository {
	public enum RatingHitTest { None, Rating, Text }
	public enum RatingTextLocation { Default, None, Top, Left, Bottom, Right }
	public delegate void ItemEventHandler(object sender, ItemEventArgs e);
	public class ItemEventArgs : EventArgs {
		int index;
		public ItemEventArgs(int index) {
			this.index = index;
		}
		public int Index { get { return index; } }
	}
	public delegate void RatingToolTipEventHandler(object sender, RatingToolTipEventArgs e);
	public class RatingToolTipEventArgs : EventArgs {
		decimal value;
		RatingHitTest hitTest;
		string text;
		public RatingToolTipEventArgs(RatingHitTest hitTest, decimal value) {
			this.value = value;
			this.hitTest = hitTest;
			this.text = String.Empty;
		}
		public decimal Value { get { return value; } }
		public RatingHitTest HitTest { get { return hitTest; } }
		public string Text { 
			get { return text; }
			set {
				if(text == value) return;
				text = value;
			}
		}
	}
	public class RepositoryItemRatingControl : RepositoryItem {
		int itemCount;
		Image glyph;
		Image checkedGlyph;
		RatingTextLocation textLocation;
		ItemLocation ratingLocation;
		Padding padding;
		int textToRatingIndent;
		bool autoSize;
		bool showText;
		Orientation ratingOrientation;
		Image hoverGlyph;
		RatingItemFillPrecision fillPrecision;
		bool isDirectionReversed;
		private static readonly object itemClick = new object();
		private static readonly object itemMouseOver = new object();
		private static readonly object itemMouseOut = new object();
		private static readonly object beforeShowToolTip = new object();
		static RepositoryItemRatingControl() {			
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never), Browsable(false), Obsolete(ObsoleteText.SRObsoletePropertiesText)]
		public new RepositoryItemRatingControl Properties { get { return this; } }
		public RepositoryItemRatingControl() {
			this.textLocation = Repository.RatingTextLocation.Default;
			this.autoSize = true;
			this.textToRatingIndent = 3;
			this.showText = false;
			this.fillPrecision = RatingItemFillPrecision.Full;
			this.ratingOrientation = Orientation.Horizontal;
			this.glyph = null;
			this.checkedGlyph = null;
			this.hoverGlyph = null;
			this.itemCount = 5;
			BorderStyle = BorderStyles.NoBorder;
			this.isDirectionReversed = false;
			this.ratingLocation = ItemLocation.Default;
		}
		[Browsable(false)]
		public override string EditorTypeName { get { return "RatingControl"; } }
		[DefaultValue(5)]
		public int ItemCount {
			get { return itemCount; }		   
			set {
				if(itemCount == value || itemCount <= 0) return;			   
				itemCount = value;
				OnPropertiesChanged();
			}
		}
		[DefaultValue(null)]
		public Image Glyph {
			get { return glyph; }
			set {
				if(glyph == value) return;
				glyph = value;				
				OnPropertiesChanged();
			}
		}
		[DefaultValue(null)]
		public Image CheckedGlyph {
			get { return checkedGlyph; }
			set {
				if(checkedGlyph == value) return;
				checkedGlyph = value;			  
				OnPropertiesChanged();
			}
		}
		[DefaultValue(HorzAlignment.Near), Obsolete("This property is obsolete. Use TextLocation property"), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public virtual HorzAlignment RatingAlignment {
			get { return GetRatingAlignment(); }
			set { TextLocation = GetTextLocation(value); }
		}
		RatingTextLocation GetTextLocation(HorzAlignment ratingAlignment) {
			if(ratingAlignment == HorzAlignment.Far) return OwnerEdit != null && OwnerEdit.IsRightToLeft ? RatingTextLocation.Right : RatingTextLocation.Left;
			if(ratingAlignment == HorzAlignment.Near) return OwnerEdit != null && OwnerEdit.IsRightToLeft ? RatingTextLocation.Left : RatingTextLocation.Right;
			if(ratingAlignment == HorzAlignment.Center) return RatingTextLocation.None;
			return Repository.RatingTextLocation.Default;
		}
		HorzAlignment GetRatingAlignment() {
			if(TextLocation == Repository.RatingTextLocation.Right) return OwnerEdit != null && OwnerEdit.IsRightToLeft ? HorzAlignment.Far : HorzAlignment.Near;
			if(TextLocation == Repository.RatingTextLocation.Left) return OwnerEdit != null && OwnerEdit.IsRightToLeft ? HorzAlignment.Near : HorzAlignment.Far;
			if(TextLocation == Repository.RatingTextLocation.None) return HorzAlignment.Center;
			return HorzAlignment.Default;
		}
		[DefaultValue(RatingTextLocation.Default)]
		public virtual RatingTextLocation TextLocation {
			get { return textLocation; }
			set {
				if(textLocation == value) return;
				textLocation = value;
				OnPropertiesChanged();
			}
		}
		[DefaultValue(ItemLocation.Default)]
		public virtual ItemLocation RatingLocation {
			get { return ratingLocation; }
			set {
				if(ratingLocation == value) return;
				ratingLocation = value;
				OnPropertiesChanged();
			}
		}
		public Padding Padding {
			get { return padding; }
			set {
				if(padding == value) return;
				padding = value;				
				OnPropertiesChanged();
			}
		}
		[DefaultValue(3)]
		public int TextToRatingIndent {
			get { return textToRatingIndent; }
			set {
				if(textToRatingIndent == value) return;
				textToRatingIndent = value;
				OnPropertiesChanged();
			}
		}
		[Browsable(false)]
		public override bool AutoHeight {
			get {
				return base.AutoHeight;
			}
			set {
				base.AutoHeight = value;
			}
		}
		public override DefaultBoolean AllowHtmlDraw {
			get {
				return base.AllowHtmlDraw;
			}
			set {
				if(value == DefaultBoolean.True && (base.AllowHtmlDraw != value))
					recalcHtml = true;
				base.AllowHtmlDraw = value;
			}
		}
		internal bool recalcHtml = false;
		[DefaultValue(true)]
		public bool AutoSize {
			get { return autoSize; }
			set {
				if(autoSize == value) return;				
				autoSize = value;
				AutoHeight = value;
				if(AutoSize)
					recalcHtml = true;				
				OnPropertiesChanged();
			}
		}
		[DefaultValue(false)]
		public bool ShowText {
			get { return showText; }
			set {
				if(showText == value) return;
				showText = value;
				OnPropertiesChanged();
			}
		}
		[DefaultValue(Orientation.Horizontal)]
		public Orientation RatingOrientation {
			get { return ratingOrientation; }
			set {
				if(ratingOrientation == value) return;
				ratingOrientation = value;
				OnPropertiesChanged();
			}
		}
		[DefaultValue(null)]
		public Image HoverGlyph {
			get { return hoverGlyph; }
			set {
				if(hoverGlyph == value) return;
				hoverGlyph = value;
				OnPropertiesChanged();
			}
		}
		[DefaultValue(RatingItemFillPrecision.Full)]
		public RatingItemFillPrecision FillPrecision {
			get { return fillPrecision; }
			set {
				if(fillPrecision == value) return;
				fillPrecision = value;
				OnPropertiesChanged();
			}
		}
		[DefaultValue(BorderStyles.NoBorder)]
		public override BorderStyles BorderStyle {
			get {
				return base.BorderStyle;
			}
			set {
				base.BorderStyle = value;
			}
		}	
		[DefaultValue(false)]
		public bool IsDirectionReversed {
			get { return isDirectionReversed; }
			set {
				if(isDirectionReversed == value) return;
				isDirectionReversed = value;
				OnPropertiesChanged();
			}
		}		
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemRatingControlItemClick"),
#endif
 DXCategory(CategoryName.Events)]
		public event ItemEventHandler ItemClick {
			add { this.Events.AddHandler(itemClick, value); }
			remove { this.Events.RemoveHandler(itemClick, value); }
		}
		protected internal virtual void RaiseItemClick(ItemEventArgs e) {
			if(IsLockEvents)
				return;
			ItemEventHandler handler = (ItemEventHandler)this.Events[itemClick];
			if(handler != null) handler(GetEventSender(), e);
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemRatingControlItemMouseOver"),
#endif
 DXCategory(CategoryName.Events)]
		public event ItemEventHandler ItemMouseOver {
			add { this.Events.AddHandler(itemMouseOver, value); }
			remove { this.Events.RemoveHandler(itemMouseOver, value); }
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemRatingControlBeforeShowToolTip"),
#endif
 DXCategory(CategoryName.Events)]
		public event RatingToolTipEventHandler BeforeShowToolTip {
			add { this.Events.AddHandler(beforeShowToolTip, value); }
			remove { this.Events.RemoveHandler(beforeShowToolTip, value); }
		}
		protected internal virtual void RaiseItemMouseOver(ItemEventArgs e) {
			if(IsLockEvents)
				return;
			ItemEventHandler handler = (ItemEventHandler)this.Events[itemMouseOver];
			if(handler != null) handler(GetEventSender(), e);
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemRatingControlItemMouseOut"),
#endif
 DXCategory(CategoryName.Events)]
		public event ItemEventHandler ItemMouseOut {
			add { this.Events.AddHandler(itemMouseOut, value); }
			remove { this.Events.RemoveHandler(itemMouseOut, value); }
		}
		protected internal virtual void RaiseItemMouseOut(ItemEventArgs e) {
			if(IsLockEvents)
				return;
			ItemEventHandler handler = (ItemEventHandler)this.Events[itemMouseOut];
			if(handler != null) handler(GetEventSender(), e);
		}
		protected internal virtual void RaiseBeforeShowToolTip(RatingToolTipEventArgs e) {
			if(IsLockEvents)
				return;
			RatingToolTipEventHandler handler = (RatingToolTipEventHandler)this.Events[beforeShowToolTip];
			if(handler != null) handler(GetEventSender(), e);
		}	   
		public override void Assign(RepositoryItem item) {
			RepositoryItemRatingControl source = item as RepositoryItemRatingControl;
			BeginUpdate();
			try {
				base.Assign(item);
				if(source == null) return;
				this.itemCount = source.ItemCount;
				this.glyph = source.Glyph;
				this.checkedGlyph = source.CheckedGlyph;				
				this.textLocation = source.TextLocation;
				this.ratingLocation = source.RatingLocation;
				this.padding = source.Padding;
				this.textToRatingIndent = source.TextToRatingIndent;
				this.autoSize = source.AutoSize;
				this.showText = source.ShowText;
				this.ratingOrientation = source.RatingOrientation;
				this.hoverGlyph = source.HoverGlyph;
				this.fillPrecision = source.FillPrecision;
				this.isDirectionReversed = source.IsDirectionReversed;
			}
			finally {
				EndUpdate();
			}
			Events.AddHandler(itemClick, source.Events[itemClick]);
			Events.AddHandler(itemMouseOver, source.Events[itemMouseOver]);
			Events.AddHandler(itemMouseOut, source.Events[itemMouseOut]);
			Events.AddHandler(beforeShowToolTip, source.Events[beforeShowToolTip]);
		}		
		protected internal Decimal GetRating(object val) {
			Decimal rating = ConvertObjectToDecimal(val);
			return AdjustRating(rating);		  
		}
		protected Decimal ConvertObjectToDecimal(object val) {
			if(val is Decimal)
				return (Decimal)val;
			if(val == null || val.ToString().Length == 0)
				return 0;
			decimal res;
			if(decimal.TryParse(val.ToString(), out res))
				return res;
			return 0;					   
		}
		protected internal virtual decimal AdjustRating(Decimal value){
			if(value < 0)
				return 0;
			if(value > Convert.ToDecimal(ItemCount))
				return Convert.ToDecimal(ItemCount);
			if(FillPrecision == RatingItemFillPrecision.Full)
				return Decimal.Floor(value);
			if(FillPrecision == RatingItemFillPrecision.Half) {
				decimal fraction = value - Decimal.Floor(value);
				if(fraction < 0.5m)
					return Decimal.Floor(value);
				else
					return Decimal.Floor(value) + 0.5m;
			}
			return value;
		}
		public override XtraPrinting.IVisualBrick GetBrick(PrintCellHelperInfo info) {
			RatingControlViewInfo viewInfo = PreparePrintViewInfo(info, true) as RatingControlViewInfo;
			viewInfo.EditValue = info.EditValue;
			RatingControlPainter painter = CreatePainter() as RatingControlPainter;
			Bitmap image = new Bitmap(viewInfo.Bounds.Size.Width, viewInfo.Bounds.Size.Height);
			using(Graphics g = Graphics.FromImage(image)) {
				using(GraphicsCache cache = new GraphicsCache(g)) {
					ControlGraphicsInfoArgs painterInfo = new ControlGraphicsInfoArgs(viewInfo, cache, viewInfo.Bounds);
					painter.Draw(painterInfo);
				}
			}
			IImageBrick imageBrick = CreateImageBrick(info, CreateBrickStyle(info, "image"));
			imageBrick.SizeMode = ImageSizeMode.Squeeze;
			imageBrick.Image = image;
			return imageBrick;
		}
	}
}
namespace DevExpress.XtraEditors {
	[DXToolboxItem(DXToolboxItemKind.Free)]
	[Designer("DevExpress.XtraEditors.Design.RatingControlDesigner, " + AssemblyInfo.SRAssemblyEditorsDesign)]
	[ToolboxTabName(AssemblyInfo.DXTabCommon)]
	[Description("Allows rating content by selecting the precise score in just one click.")]
	[ToolboxBitmap(typeof(ToolboxIconsRootNS), "RatingControl")]
	public class RatingControl : BaseEdit {
		string text;
		public RatingControl() {
			Text = String.Empty;
			base.AutoSize = true;	  
		}
		[DefaultValue(true)]
		public override bool AutoSize {
			get {
				return base.AutoSize;
			}
			set {
				base.AutoSize = value;
			}
		}
		public override string EditorTypeName { get { return "RatingControl"; } }
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new RepositoryItemRatingControl Properties {
			get { return base.Properties as RepositoryItemRatingControl; }
		}
		[RefreshProperties(RefreshProperties.All)]
		public Decimal Rating {
			get { return Properties.GetRating(EditValue); }
			set {
				decimal newValue = Properties.AdjustRating(value);
				if(Rating == newValue) return;
				EditValue = newValue;
				OnPropertiesChanged();
			}
		}
		protected internal new RatingControlViewInfo ViewInfo { get { return base.ViewInfo as RatingControlViewInfo; } }
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RatingControlItemClick"),
#endif
 DXCategory(CategoryName.Events)]
		public event ItemEventHandler ItemClick {
			add { Properties.ItemClick += value; }
			remove { Properties.ItemClick -= value; }
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RatingControlItemMouseOver"),
#endif
 DXCategory(CategoryName.Events)]
		public event ItemEventHandler ItemMouseOver {
			add { Properties.ItemMouseOver += value; }
			remove { Properties.ItemMouseOver -= value; }
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RatingControlItemMouseOut"),
#endif
 DXCategory(CategoryName.Events)]
		public event ItemEventHandler ItemMouseOut {
			add { Properties.ItemMouseOut += value; }
			remove { Properties.ItemMouseOut -= value; }
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RatingControlBeforeShowToolTip"),
#endif
 DXCategory(CategoryName.Events)]
		public event RatingToolTipEventHandler BeforeShowToolTip {
			add { Properties.BeforeShowToolTip += value; }
			remove { Properties.BeforeShowToolTip -= value; }
		}
		protected override void SetBoundsCore(int x, int y, int width, int height, BoundsSpecified specified) {
			if(InplaceType == InplaceType.Standalone && Properties.AutoSize)
				width = CalcBestSize().Width;		  
			base.SetBoundsCore(x, y, width, height, specified);		
		}
		protected virtual void SetAutoWidth() {
			int bestWidth = CalcBestSize().Width;
			if(Width != bestWidth)
				Width = bestWidth;
		}	   
		protected internal override void OnPropertiesChanged() {
			if(InplaceType == InplaceType.Standalone && Properties.AutoSize)
				SetAutoWidth();		   
			base.OnPropertiesChanged();								   
		}	  
		protected override void OnMouseMove(MouseEventArgs e) {
			RatingControlViewInfo viewInfo = ViewInfo as RatingControlViewInfo;
			decimal ratingRectIndex = viewInfo.GetRatingRectValue(e.Location);
			if(ratingRectIndex != viewInfo.CurrentRatingRectIndex) {
				viewInfo.CurrentRatingRectIndex = ratingRectIndex;
				if(viewInfo.DisabledHotTrack)
					viewInfo.DisabledHotTrack = false;
			}			
			base.OnMouseMove(e);
		}	  
		protected override void OnMouseUp(MouseEventArgs e) {
			base.OnMouseUp(e);
			if(e.Button != System.Windows.Forms.MouseButtons.Left) return;
			RatingControlViewInfo viewInfo = ViewInfo as RatingControlViewInfo;   
			if(!Properties.ReadOnly) {
				if(!UpdateRating(e.Location)) return;
				viewInfo.DisabledHotTrack = true;
				Invalidate();
			}			
			int index = viewInfo.GetRatingItemIndex(e.Location);
			if(index == -1) return;
			Properties.RaiseItemClick(new ItemEventArgs(index));
		}
		protected virtual bool UpdateRating(Point point) {			
			RatingControlViewInfo viewInfo = ViewInfo as RatingControlViewInfo;
			decimal ratingRectValue = viewInfo.GetRatingRectValue(point);
			if(ratingRectValue == -2) return false;
			decimal ratingBefore = Rating;
			CalcRating(ratingRectValue);
			return !(ratingBefore == Rating);
		}
		protected virtual void CalcRating(decimal ratingRectValue) {		   
				if(ratingRectValue < Properties.ItemCount - 1) {
					if(Rating == ratingRectValue + 1)
						Rating = 0;
					else
						Rating = ratingRectValue + 1;
				}
				else {
					if(Rating == Properties.ItemCount)
						Rating = 0;
					else
						Rating = ratingRectValue + 1;
				}		   
		}	   
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		public override string Text {
			get { return text; }
			set {
				if(text == value) return;
				text = value;
				((BaseEditViewInfo)ViewInfo).SetDisplayText(text);
				if(Properties.AllowHtmlDraw == DefaultBoolean.True)
					Properties.recalcHtml = true;
				OnPropertiesChanged();
			}
		}		
	}
}
namespace DevExpress.XtraEditors.ViewInfo {
	 public class RatingHitInfo : EditHitInfo {
		RatingHitTest ratingHitTest; 
		object ratingHitValue;		
		public RatingHitInfo(Point pt) : base(pt) { }
		public RatingHitTest RatingHitTest { get { return ratingHitTest; } }
		public object RatingHitValue { get { return ratingHitValue; } }
		protected virtual internal void SetRatingHitTest(RatingHitTest hitTest) { this.ratingHitTest = hitTest; }
		protected virtual internal void SetRatingHitValue(object hitValue) { this.ratingHitValue = hitValue; }
		public override void Assign(EditHitInfo hitInfo) {
			base.Assign(hitInfo);
			RatingHitInfo ratingInfo = hitInfo as RatingHitInfo;
			if(ratingInfo == null) return;
			this.ratingHitTest = ratingInfo.RatingHitTest;
			this.ratingHitValue = ratingInfo.RatingHitValue;
		}
	}
	public class RatingControlViewInfo : BaseEditViewInfo, IHeightAdaptable {
		List<Rectangle> ratingRects;
		Items2Panel panel;
		Rectangle ratingBounds;
		Rectangle textBounds;
		decimal prevRatingIndex = -2;
		decimal curRatingIndex = -2;
		int itemOverIndex = -1;
		internal int itemIndent = 2;
		RatingControlHelper helper;
		public RatingControlViewInfo(RepositoryItem item)
			: base(item) {
		}
		protected internal new RatingControl OwnerControl { get { return base.OwnerControl as RatingControl; } }
		public new RepositoryItemRatingControl Item { get { return base.Item as RepositoryItemRatingControl; } }
		public int ItemCount { get { return Item.ItemCount; } }
		protected internal Image Glyph { get; set; }
		protected internal Image HoverGlyph { get; set; }
		protected internal Image CheckedGlyph { get; set; }
		protected internal RatingControlHelper Helper {
			get {
				if(helper == null)
					helper = CreateRatingControlHelper();
				return helper;
			}
		}
		protected virtual RatingControlHelper CreateRatingControlHelper() {
			return new RatingControlHelper(Item.RatingOrientation, OwnerControl != null ? OwnerControl.IsRightToLeft : false, GetGlyphSize(), Item.IsDirectionReversed);
		}
		protected virtual void UpdateGlyphs() {
			SkinElement elem = GetSkinGlyph();
			if(elem == null || elem.Image == null)
				return;
			Glyph = Item.Glyph != null ? Item.Glyph : elem.Image.GetImages().Images[0];
			HoverGlyph = Item.HoverGlyph != null ? Item.HoverGlyph : elem.Image.GetImages().Images[1];
			CheckedGlyph = Item.CheckedGlyph != null ? Item.CheckedGlyph : elem.Image.GetImages().Images[2];
		}
		public virtual List<Rectangle> RatingRects {
			get {
				if(ratingRects == null)
					ratingRects = new List<Rectangle>();
				return ratingRects;
			}
			private set {
				ratingRects = value;
			}
		}
		public virtual Items2Panel Panel {
			get {
				if(panel == null) {
					panel = new Items2Panel();
					panel.Content1VerticalAlignment = ItemVerticalAlignment.Center;
					panel.Content2VerticalAlignment = ItemVerticalAlignment.Center;
				}
				return panel;
			}
		}
		protected internal bool IsRightToLeft {
			get {
				if(OwnerEdit == null) return RightToLeft;
				return OwnerEdit.IsRightToLeft;
			}
		}
		internal Color CustomSkinElementColor { get; set; }
		protected virtual SkinElement GetSkinGlyph() {
			Skin skin = EditorsSkins.GetSkin(LookAndFeel.ActiveLookAndFeel);
			SkinElement elem = skin[EditorsSkins.SkinRatingIndicator];
			if(elem != null && (!elem.IsCustomElement || CustomSkinElementColor == skin.Properties.GetColor(EditorsSkins.SkinRatingIndicatorColor)))
				return elem;
			if(elem != null)
				skin.RemoveElement(elem);
			SkinElement src = EditorsSkins.GetSkin(DefaultProvider)[EditorsSkins.SkinRatingIndicator];
			if(src == null)
				return null;
			elem = src.Copy(skin, EditorsSkins.SkinRatingIndicator);
			elem.IsCustomElement = true;
			skin.AddElement(elem);
			CustomSkinElementColor = skin.Properties.GetColor(EditorsSkins.SkinRatingIndicatorColor);
			elem.Info.Image.Image = SkinImageColorizer.GetColoredImage(src.Image.ImageCore, CustomSkinElementColor, SkinColorizationMode.Hue);
			return elem;
		}
		public Size RatingSize { get; private set; }
		public virtual Rectangle RatingBounds {
			get { return ratingBounds; }
			private set {
				if(ratingBounds == value) return;
				ratingBounds = value;
			}
		}
		public virtual Rectangle TextBounds {
			get { return textBounds; }
			private set {
				if(textBounds == value) return;
				textBounds = value;
			}
		}
		public bool DisabledHotTrack { get; protected internal set; }
		public decimal CurrentRatingRectIndex { get; protected internal set; }
		protected override void OnEditValueChanged() { }			  
		public virtual Size GetGlyphSize() {
			if(Glyph == null)
				return Size.Empty;
			else
				return Glyph.Size;
		}
		protected virtual void CalcRatingRects() {
			List<Rectangle> ratingRects = new List<Rectangle>();
			Size glyphSize = GetGlyphSize();
			Point location = RatingBounds.Location;
			bool flag = false;
			for(int i = 0; i < ItemCount; i++) {
				Rectangle ratingRect;
				if(flag) {
					ratingRect = Rectangle.Empty;
					continue;
				}
				Point ratingRectLoc;
				if(i == 0) 
					ratingRectLoc = (Item.RatingOrientation == Orientation.Horizontal) ? new Point(location.X + itemIndent, location.Y) : new Point(location.X, location.Y + itemIndent);			  
				else
					ratingRectLoc = (Item.RatingOrientation == Orientation.Horizontal) ? new Point(ratingRects[i - 1].Right + itemIndent, location.Y) : new Point(location.X, ratingRects[i - 1].Bottom + itemIndent);
				ratingRect = new Rectangle(ratingRectLoc, glyphSize);
				if(ratingRect.Right > RatingBounds.Right) {
					ratingRect.Width = RatingBounds.Right - ratingRect.Left;
					if(Item.RatingOrientation == Orientation.Horizontal)
						flag = true;				  
				}
				ratingRects.Add(ratingRect);
			}
			RatingRects = ratingRects;
		}	   
		protected virtual Size CalcRatingSize() {
			Size size = new Size();
			if(Item.RatingOrientation == Orientation.Horizontal) {
				size.Width = ItemCount * GetGlyphSize().Width + (ItemCount + 1) * itemIndent;
				size.Height = GetGlyphSize().Height;
			}
			else {
				size.Width = GetGlyphSize().Width;
				size.Height = ItemCount * GetGlyphSize().Height + (ItemCount + 1) * itemIndent;
			}		   
			return size;
		}
		protected override Size CalcContentSize(Graphics g) {
			RatingSize = CalcRatingSize();
			UpdatePanelProperties();
			Size contentSize;
			if(Item.ShowText && Item.TextLocation != RatingTextLocation.None)
				contentSize = this.Panel.CalcBestSize(RatingSize, TextSize);
			else
				contentSize = this.Panel.CalcBestSize(RatingSize, Size.Empty);
			return contentSize;
		}
		protected virtual void UpdatePanelProperties() {
			if(Item.TextLocation == RatingTextLocation.Default)
				Panel.Content1Location = OwnerControl != null && OwnerControl.IsRightToLeft ? ItemLocation.Right : ItemLocation.Left;
			if(Item.TextLocation == RatingTextLocation.Left) Panel.Content1Location = ItemLocation.Right;
			if(Item.TextLocation == RatingTextLocation.Right) Panel.Content1Location = ItemLocation.Left;
			if(Item.TextLocation == RatingTextLocation.Bottom) Panel.Content1Location = ItemLocation.Top;
			if(Item.TextLocation == RatingTextLocation.Top) Panel.Content1Location = ItemLocation.Bottom;
			if(Item.TextLocation == RatingTextLocation.None) Panel.Content1Location = Item.RatingLocation;
			if(!Item.AutoSize) {
				if(Item.ShowText) {
					#region if showText
					if((Item.RatingLocation == ItemLocation.Bottom || Item.RatingLocation == ItemLocation.Top || Item.RatingLocation == ItemLocation.Default) 
						&& (Panel.Content1Location == ItemLocation.Top || Panel.Content1Location == ItemLocation.Bottom)) {
						Panel.Content1HorizontalAlignment = ItemHorizontalAlignment.Center;
						Panel.Content2HorizontalAlignment = ItemHorizontalAlignment.Center;
					}
					if(Item.RatingLocation == ItemLocation.Bottom) {
						if(Panel.Content1Location == ItemLocation.Right || Panel.Content1Location == ItemLocation.Left) {
							Panel.StretchContent1 = true;
							Panel.StretchContent2 = true;
							Panel.Content1VerticalAlignment = ItemVerticalAlignment.Bottom;
							Panel.Content2VerticalAlignment = ItemVerticalAlignment.Bottom;
						}
						switch (Panel.Content1Location){
							case ItemLocation.Top:
								Panel.StretchContent1 = true;
								Panel.Content1VerticalAlignment = ItemVerticalAlignment.Bottom;
								break;
							case ItemLocation.Bottom:
								Panel.StretchContent2 = true;
								Panel.Content2VerticalAlignment = ItemVerticalAlignment.Bottom;
								break;
							case ItemLocation.Right:
								Panel.Content2HorizontalAlignment = ItemHorizontalAlignment.Right;
								break;
							case ItemLocation.Left:
								Panel.Content1HorizontalAlignment = ItemHorizontalAlignment.Right;
								break;
						}
					}
					if(Item.RatingLocation == ItemLocation.Top) {
						if(Panel.Content1Location == ItemLocation.Right || Panel.Content1Location == ItemLocation.Left) {
							Panel.StretchContent1 = true;
							Panel.StretchContent2 = true;
							Panel.Content1VerticalAlignment = ItemVerticalAlignment.Top;
							Panel.Content2VerticalAlignment = ItemVerticalAlignment.Top;
						}
						switch(Panel.Content1Location) {
							case ItemLocation.Top:
								Panel.Content1VerticalAlignment = ItemVerticalAlignment.Top;
								break;
							case ItemLocation.Bottom:
								Panel.StretchContent1 = true;
								Panel.Content1VerticalAlignment = ItemVerticalAlignment.Top;
								break;
							case ItemLocation.Right:
								Panel.Content2HorizontalAlignment = ItemHorizontalAlignment.Right;
								break;
							case ItemLocation.Left:
								Panel.Content1HorizontalAlignment = ItemHorizontalAlignment.Right;
								break;
						}
					}
					if((Item.RatingLocation == ItemLocation.Right || Item.RatingLocation == ItemLocation.Left || Item.RatingLocation == ItemLocation.Default) 
						&& (Panel.Content1Location == ItemLocation.Left || Panel.Content1Location == ItemLocation.Right)){
						Panel.Content1VerticalAlignment = ItemVerticalAlignment.Center;
						Panel.Content2VerticalAlignment = ItemVerticalAlignment.Center;
					}
					if(Item.RatingLocation == ItemLocation.Right) {
						if(Panel.Content1Location == ItemLocation.Top || Panel.Content1Location == ItemLocation.Bottom) {
							Panel.StretchContent1 = true;
							Panel.StretchContent2 = true;
							Panel.Content1HorizontalAlignment = ItemHorizontalAlignment.Right;
							Panel.Content2HorizontalAlignment = ItemHorizontalAlignment.Right;
						}
						switch (Panel.Content1Location){
							case ItemLocation.Top:
								Panel.Content1VerticalAlignment = ItemVerticalAlignment.Bottom;
								Panel.Content2VerticalAlignment = ItemVerticalAlignment.Top;
								break;
							case ItemLocation.Bottom:
								Panel.Content1VerticalAlignment = ItemVerticalAlignment.Top;
								Panel.Content2VerticalAlignment = ItemVerticalAlignment.Bottom;
								break;
							case ItemLocation.Right:
								Panel.StretchContent2 = true;
								Panel.Content2HorizontalAlignment = ItemHorizontalAlignment.Right;
								break;
							case ItemLocation.Left:
								Panel.StretchContent1 = true;
								Panel.Content1HorizontalAlignment = ItemHorizontalAlignment.Right;
								break;
						}
					}
					if(Item.RatingLocation == ItemLocation.Left) {
						if(Panel.Content1Location == ItemLocation.Top || Panel.Content1Location == ItemLocation.Bottom) {
							Panel.StretchContent1 = true;
							Panel.StretchContent2 = true;
							Panel.Content1HorizontalAlignment = ItemHorizontalAlignment.Left;
							Panel.Content2HorizontalAlignment = ItemHorizontalAlignment.Left;
						}
						switch (Panel.Content1Location){
							case ItemLocation.Top: 
								Panel.Content1VerticalAlignment = ItemVerticalAlignment.Bottom;
								Panel.Content2VerticalAlignment = ItemVerticalAlignment.Top;
								break;
							case ItemLocation.Bottom:
								Panel.Content1VerticalAlignment = ItemVerticalAlignment.Top;
								Panel.Content2VerticalAlignment = ItemVerticalAlignment.Bottom;
								break;
							case ItemLocation.Right:
								Panel.StretchContent1 = true;
								Panel.Content1HorizontalAlignment = ItemHorizontalAlignment.Left;
								break;
							case ItemLocation.Left:
								Panel.StretchContent2 = true;
								Panel.Content2HorizontalAlignment = ItemHorizontalAlignment.Left;
								break;
						}
					}
					if(Item.RatingLocation == ItemLocation.Default) {
						Panel.StretchContent1 = true;
						Panel.StretchContent2 = true;
						switch (Panel.Content1Location){
							case ItemLocation.Top:
								Panel.Content1VerticalAlignment = ItemVerticalAlignment.Bottom;
								Panel.Content2VerticalAlignment = ItemVerticalAlignment.Top;
								break;
							case ItemLocation.Bottom:
								Panel.Content1VerticalAlignment = ItemVerticalAlignment.Top;
								Panel.Content2VerticalAlignment = ItemVerticalAlignment.Bottom;
								break;
							case ItemLocation.Left:
								Panel.Content1HorizontalAlignment = ItemHorizontalAlignment.Right;
								Panel.Content2HorizontalAlignment = ItemHorizontalAlignment.Left;
								break;
							case ItemLocation.Right:
								Panel.Content1HorizontalAlignment = ItemHorizontalAlignment.Left;
								Panel.Content2HorizontalAlignment = ItemHorizontalAlignment.Right;
								break;
						}
					}
					#endregion
				}
				if(!Item.ShowText || Item.TextLocation == RatingTextLocation.None) {
					Panel.Content1Location = Item.RatingLocation;
					if(Item.RatingLocation == ItemLocation.Bottom || Item.RatingLocation == ItemLocation.Top) {
						Panel.Content1HorizontalAlignment = ItemHorizontalAlignment.Center;
					}
					if(Item.RatingLocation == ItemLocation.Left || Item.RatingLocation == ItemLocation.Right) {
						Panel.Content1VerticalAlignment = ItemVerticalAlignment.Center;
					}
					if(Item.RatingLocation == ItemLocation.Top) Panel.Content1VerticalAlignment = ItemVerticalAlignment.Top;
					if(Item.RatingLocation == ItemLocation.Bottom) Panel.Content1VerticalAlignment = ItemVerticalAlignment.Bottom;
					if(Item.RatingLocation == ItemLocation.Left) Panel.Content1HorizontalAlignment = ItemHorizontalAlignment.Left;
					if(Item.RatingLocation == ItemLocation.Right) Panel.Content1HorizontalAlignment = ItemHorizontalAlignment.Right;
				}
				if(Item.RatingLocation == ItemLocation.Default && (Item.TextLocation == RatingTextLocation.None|| !Item.ShowText)) {
					Panel.StretchContent1 = false;
					Panel.Content1VerticalAlignment = ItemVerticalAlignment.Center;
					Panel.Content1HorizontalAlignment = ItemHorizontalAlignment.Center;
				}
			}
			else if(InplaceType != Controls.InplaceType.Standalone || Item.TextLocation == RatingTextLocation.None || Item.TextLocation == RatingTextLocation.Bottom || Item.TextLocation == RatingTextLocation.Top) {
				Panel.Content1HorizontalAlignment = ItemHorizontalAlignment.Center;
				Panel.Content2HorizontalAlignment = ItemHorizontalAlignment.Center;
			}
			Panel.HorizontalIndent = Item.TextToRatingIndent;
			Panel.HorizontalPadding = Item.Padding;
			if(!Item.ShowText)
				Panel.Content1Padding = Item.Padding;
			else
				if(Item.AutoSize)
					Panel.StretchContent2 = false;	  
		}
		protected override int MaxTextWidth {
			get {
				if(Item.AutoSize)
					return base.MaxTextWidth;
				else
					return GetMaxTextWidth();			   
			}
		}	   
		protected override Size CalcTextSizeCore(GraphicsCache cache, string text, int maxWidth) {
			if(String.IsNullOrEmpty(text) || maxWidth < 0 || (!Item.AutoSize && maxWidth == 0))
				return Size.Empty;
			if(Item.AutoSize)
				return base.CalcTextSizeCore(cache, text, maxWidth);
			else {
				Size size;
				if(AllowHtmlString) {
					size = CalcHtmlTextSize(cache, text, maxWidth);
				}
				else{
					size = PaintAppearance.CalcTextSize(cache.Graphics, PaintAppearance.GetTextOptions().GetStringFormat(PaintAppearance.GetTextOptions()), text, maxWidth, GetMaxTextHeight()).ToSize();					
				}
				if(size.Height % 2 != 0) size.Height++;
				return size;				
			}		   
		}
		protected virtual int GetMaxTextHeight() {
			int maxHeight = ContentRect.Height - Item.Padding.Vertical;
			return maxHeight;
		}
		protected virtual int GetMaxTextWidth() {
			int maxTextWidth = ContentRect.Width - RatingSize.Width - Item.Padding.Horizontal - Item.TextToRatingIndent;
			return maxTextWidth;
		}
		protected override void CalcConstants() {
			RatingSize = CalcRatingSize();					
		}
		protected override void CalcRects() {
			base.CalcRects();
			CalcTextSize(null);
			UpdatePanelProperties();
			if(InplaceType != Controls.InplaceType.Standalone || Item.TextLocation == RatingTextLocation.None || !Item.ShowText) {
				this.Panel.ArrangeItems(ContentRect, RatingSize, Size.Empty, ref ratingBounds, ref textBounds);
				AdjustRatingBounds();		  
			}
			else {
				this.Panel.ArrangeItems(ContentRect, RatingSize, TextSize, ref ratingBounds, ref textBounds);
			}		   
			if(AllowHtmlString) {
				using(Graphics g = OwnerControl.CreateGraphics()) {
					StringInfo = StringPainter.Default.Calculate(g, PaintAppearance, PaintAppearance.TextOptions, Color.Empty, DisplayText, TextBounds);						
				}
			}
			CalcRatingRects();
		}
		protected virtual void AdjustRatingBounds() {
			if(ratingBounds.Width > ContentRect.Width - Panel.Content1Padding.Horizontal) {
				ratingBounds.Width = ContentRect.Width - Panel.Content1Padding.Horizontal;
				ratingBounds.X = ContentRect.X + Panel.Content1Padding.Left;
			}
			if(ratingBounds.Height > ContentRect.Height - Panel.Content1Padding.Vertical) {
				ratingBounds.Height = ContentRect.Height - Panel.Content1Padding.Vertical;
				ratingBounds.Y = ContentRect.Y + Panel.Content1Padding.Top;
			}
		}		
		protected override void CalcTextSize(Graphics g, bool useDisplayText) {
			base.CalcTextSize(g, true);
		}
		protected internal virtual int GetRatingItemIndex(Point point) {
			for(int i = 0; i<RatingRects.Count; i++) {
				if(RatingRects[i].Contains(point))
					return i;
			}
			return -1;	   
	   }
		protected internal virtual Rectangle GetRatingIndentRect(Rectangle ratingRect) {
			Rectangle indentRect = Rectangle.Empty;
			Point loc = Point.Empty;
			if(Item.RatingOrientation == Orientation.Horizontal) {
				loc = (IsRightToLeft == Item.IsDirectionReversed) ? new Point(ratingRect.Right, ratingRect.Top) : new Point(ratingRect.Left - itemIndent, ratingRect.Top);
				indentRect = new Rectangle(loc, new Size(itemIndent, ratingRect.Height));
			}
			else {
				loc = (!Item.IsDirectionReversed) ? new Point(ratingRect.Left, ratingRect.Top - itemIndent) : new Point(ratingRect.Left, ratingRect.Bottom);
				indentRect = new Rectangle(loc, new Size(ratingRect.Width, itemIndent));
			}
			return indentRect;
		}
		protected virtual internal Rectangle GetZeroRatingIndentRect() {
			Rectangle rect = Rectangle.Empty;
			Point loc = Point.Empty;
			if(Item.RatingOrientation == Orientation.Horizontal) {
				loc = (IsRightToLeft == Item.IsDirectionReversed) ? new Point(RatingRects[0].X - itemIndent, RatingRects[0].Y) : new Point(RatingRects[RatingRects.Count - 1].Right, RatingRects[0].Y);
				rect = new Rectangle(loc, new Size(itemIndent, RatingRects[0].Height));
			}
			else {
				loc = (!Item.IsDirectionReversed) ? new Point(RatingRects[0].X, RatingRects[RatingRects.Count - 1].Bottom) : new Point(RatingRects[0].X, RatingRects[0].Top - itemIndent);
				rect = new Rectangle(loc, new Size(RatingRects[0].Width, itemIndent));
			}
			return rect;					   
		}
		protected internal virtual Decimal GetRatingRectValue(Point point) {			
			for(int i = 0; i < RatingRects.Count; i++) {
				if(i == 0 || i == RatingRects.Count - 1) {
					Rectangle ZeroRatingIndentRect = GetZeroRatingIndentRect();
					if(ZeroRatingIndentRect.Contains(point))
						return -1.0m;
				}			
				Rectangle IndentRect = GetRatingIndentRect(RatingRects[i]);
				if(!(RatingRects[i].Contains(point) || IndentRect.Contains(point))) continue;
				decimal value;
				if(Item.RatingOrientation == Orientation.Horizontal)
					value = IsRightToLeft != Item.IsDirectionReversed ? (Decimal)(RatingRects.Count - 1 - i) : (Decimal)i;
				else
					value = (!Item.IsDirectionReversed) ? (Decimal)(RatingRects.Count - 1 - i) : (Decimal)i;
				if(Item.FillPrecision == RatingItemFillPrecision.Half)
					value = Helper.GetRatingRectHalfValue(RatingRects[i], point, value);
				if(Item.FillPrecision ==RatingItemFillPrecision.Exact)
					value = Helper.GetRatingRectExactValue(RatingRects[i], point, value);
				return value;
			}
			return -2.0m;		   
		}
		protected override void Assign(BaseControlViewInfo info) {
			base.Assign(info);
			RatingControlViewInfo be = info as RatingControlViewInfo;
			if(be == null) return;
			Glyph = be.Glyph;
			HoverGlyph = be.HoverGlyph;
			CheckedGlyph = be.CheckedGlyph;			
		}
		public override void Offset(int x, int y) {
			base.Offset(x, y);
			CalcRects();		   
		}
		public int CalcHeight(GraphicsCache cache, int width) {
			int res = 0;
			res = CalcBestFit(cache.Graphics).Height;
			return res;
		}
		protected override AppearanceDefault CreateDefaultAppearance() {
			return new AppearanceDefault(GetSystemColor(SystemColors.WindowText), Color.Transparent, GetDefaultFont());
		}
		public override void CalcViewInfo(Graphics g) {
			UpdateGlyphs();
			UpdateHelper();
			base.CalcViewInfo(g);			
		}
		public override Size CalcBestFit(Graphics g) {
			UpdateGlyphs();
			UpdateHelper();
			return base.CalcBestFit(g);
		}
		protected void UpdateHelper() {
			Helper.GlyphSize = GetGlyphSize();
			Helper.RatingOrientation = Item.RatingOrientation;
			Helper.IsRightToLeft = IsRightToLeft;
			Helper.IsDirectionReversed = Item.IsDirectionReversed;
		}
		public override bool AllowHtmlString {
			get {
				return Item.GetAllowHtmlDraw();
			}
		}
		protected override Size CalcHtmlTextSize(GraphicsCache cache, string text, int maxWidth) {
			if(Item.AutoSize) {
				if(TextBounds == Rectangle.Empty || Item.recalcHtml) {
					StringInfo = StringPainter.Default.Calculate(cache.Graphics, PaintAppearance, text, maxWidth);
					if(Item.recalcHtml) Item.recalcHtml = false;
				}
			}
			else{
				StringInfo = StringPainter.Default.Calculate(cache.Graphics, PaintAppearance, text, maxWidth);
				if(StringInfo.Bounds.Height > GetMaxTextHeight()) {
					StringInfo = StringPainter.Default.Calculate(cache.Graphics, PaintAppearance, Color.Empty, text, new Rectangle(new Point(0, 0), new Size(StringInfo.Bounds.Width, GetMaxTextHeight())));
					return new Size(StringInfo.Bounds.Width, GetMaxTextHeight());
				}				
			}
			return StringInfo.Bounds.Size;
		}
		public override ToolTipControlInfo GetToolTipInfo(Point point) {
			ToolTipControlInfo info = base.GetToolTipInfo(point);
			RatingToolTipEventArgs e = null;
			if(TextBounds.Contains(point)) {
				e = new RatingToolTipEventArgs(RatingHitTest.Text, -1);
				if(info == null) info = new ToolTipControlInfo();
				e.Text = info.Text;
				Item.RaiseBeforeShowToolTip(e);
				info.Object = "Text";
				info.Text = (!String.IsNullOrEmpty(e.Text)) ? e.Text : DisplayText;													 
			}
			if(RatingBounds.Contains(point)) {
				decimal rating = GetRatingRectValue(point) + 1.0m;
				e = new RatingToolTipEventArgs(RatingHitTest.Rating, rating);
				if(info == null) info = new ToolTipControlInfo();
				e.Text = info.Text;
				Item.RaiseBeforeShowToolTip(e);
				info.Object = rating;
				info.Text = (!String.IsNullOrEmpty(e.Text)) ? e.Text : rating.ToString();														 
			}
			if(info != null && OwnerEdit != null && OwnerEdit.InplaceType == InplaceType.Standalone) {
				info.Title = OwnerEdit.ToolTipTitle;
				info.IconType = OwnerEdit.ToolTipIconType;
				info.AllowHtmlText = OwnerEdit.AllowHtmlTextInToolTip;
			}
			return info;		   
		}
		public override EditHitInfo CalcHitInfo(Point p) {
			RatingHitInfo hitInfo = new RatingHitInfo(p);		   
			hitInfo.SetHitPoint(p);
			if(RatingBounds.Contains(p)) {
				prevRatingIndex = curRatingIndex;
				hitInfo.SetRatingHitTest(RatingHitTest.Rating);
				hitInfo.SetHitTest(EditHitTest.Button);
				decimal value = GetRatingRectValue(p);
				hitInfo.SetRatingHitValue(value + 1.0m);
				hitInfo.SetHitObject(value);
				curRatingIndex = Convert.ToDecimal(hitInfo.HitObject);
				if(DisabledHotTrack) {
					prevRatingIndex = -2;
					curRatingIndex = -2;
					hitInfo.SetHitObject(-2);
				}
			}
			else {
				prevRatingIndex = -2;
				if(TextBounds.Contains(p)) {
					hitInfo.SetRatingHitTest(RatingHitTest.Text);
					hitInfo.SetRatingHitValue(DisplayText);
					hitInfo.SetHitTest(EditHitTest.None);
				}
				else {
					hitInfo.SetRatingHitTest(RatingHitTest.None);
					hitInfo.SetHitTest(EditHitTest.None);
				}
			}
			return hitInfo;
		}
		protected virtual void RaiseItemHoverEvents() {
			if(RatingBounds.Contains(MousePosition)) {
				for(int i = 0; i < RatingRects.Count; i++) {
					if(RatingRects[i].Contains(MousePosition) && itemOverIndex != i) {
						if(itemOverIndex != -1)
							Item.RaiseItemMouseOut(new ItemEventArgs(itemOverIndex));
						itemOverIndex = i;
						Item.RaiseItemMouseOver(new ItemEventArgs(itemOverIndex));
					}
				}
			}
			else {
				if(itemOverIndex != -1) {
					Item.RaiseItemMouseOut(new ItemEventArgs(itemOverIndex));
					itemOverIndex = -1;
				}
			}
		}
		protected override bool UpdateObjectState() {
			RaiseItemHoverEvents();
			ObjectState prevState = State;
			if(Item.ReadOnly) {
				State = ObjectState.Normal;
				return false;
			}
			State = CalcObjectState();
			if(curRatingIndex != prevRatingIndex) {
				if(!RatingBounds.Contains(MousePosition) && Bounds.Contains(MousePosition))
					curRatingIndex = -2;
				return true;
			}
			return prevState != State;					 
		}
		public override bool IsRequiredUpdateOnMouseMove {
			get {
				return (base.IsRequiredUpdateOnMouseMove || !Item.ReadOnly);
			}
		}
		protected internal virtual Image GetCurrentImage(RatingControlViewInfo viewInfo, int index, decimal ratingFloor, decimal hotFraction) {
			Image curGlyph = new Bitmap(viewInfo.GetGlyphSize().Width, viewInfo.GetGlyphSize().Height);
			if(index < ratingFloor)
				curGlyph = viewInfo.CheckedGlyph;
			if(index == ratingFloor) {
				if(hotFraction > 0)
					curGlyph = viewInfo.Glyph;
				else
					curGlyph = viewInfo.CheckedGlyph;
			}
			if(index > ratingFloor)
				curGlyph = viewInfo.Glyph;
			return curGlyph;
		}
		ISkinProvider defaultProvider = null;
		protected ISkinProvider DefaultProvider {
			get { 
				if(defaultProvider == null)
					defaultProvider = new DefaultSkinProvider();
				return defaultProvider;
			}
		}
	}
}
namespace DevExpress.XtraEditors.Drawing {
	public class RatingControlPainter : BaseEditPainter {
		public override void Draw(ControlGraphicsInfoArgs info) {		   
			base.Draw(info);			
		}
		protected override void DrawContent(ControlGraphicsInfoArgs info) {
			base.DrawContent(info);
			DrawRatingItems(info);
			DrawText(info);
		}	  
		protected virtual void DrawText(ControlGraphicsInfoArgs info) {
			RatingControlViewInfo viewInfo = info.ViewInfo as RatingControlViewInfo;
			if(String.IsNullOrEmpty(viewInfo.DisplayText) || viewInfo.TextBounds == Rectangle.Empty) return;				
			if(!viewInfo.AllowHtmlString)
				viewInfo.PaintAppearance.DrawString(info.Cache, viewInfo.DisplayText, viewInfo.TextBounds);
			else 
				StringPainter.Default.DrawString(info.Cache, viewInfo.StringInfo);			
		}
		protected virtual void DrawRatingItems(ControlGraphicsInfoArgs info) {
			RatingControlViewInfo viewInfo = info.ViewInfo as RatingControlViewInfo;
			decimal hotValue = -2;
			if(viewInfo.HotInfo.HitTest == EditHitTest.Button)
				hotValue = Convert.ToDecimal(viewInfo.HotInfo.HitObject);
			int index;
			for(int i = 0; i < viewInfo.RatingRects.Count; i++) {
				if(viewInfo.Item.RatingOrientation == Orientation.Vertical)
					index = (!viewInfo.Item.IsDirectionReversed) ? viewInfo.RatingRects.Count - 1 - i : i;
				else
					index = viewInfo.IsRightToLeft != viewInfo.Item.IsDirectionReversed ? viewInfo.RatingRects.Count - 1 - i : i;
				DrawItem(info, viewInfo.RatingRects[i], index, hotValue);
			}			  
		}	   
		protected virtual void DrawItemCore(ControlGraphicsInfoArgs info, decimal rating, Rectangle rect, int index, decimal hotValue) {			
			if(DrawHotRatingItem(info, rect, index, rating, hotValue))
				return;
			DrawNormalStateItem(info, index, rating, rect);			   
		}
		protected virtual Rectangle GetGlyphHotRect(RatingControlViewInfo viewInfo, Rectangle itemRect, Rectangle glyphRect, decimal hotFraction ) {
			Rectangle fullGlyphRect = new Rectangle(Point.Empty, viewInfo.GetGlyphSize());
			Rectangle fullGlyphHotRect = viewInfo.Helper.CalcFractionRect(fullGlyphRect, hotFraction);
			Rectangle itemHotRect = viewInfo.Helper.OffsetRect(fullGlyphHotRect, itemRect.Location);
			int hiddenWidth = (itemHotRect.Right > itemRect.Right) ? viewInfo.GetGlyphSize().Width - glyphRect.Width : 0;
			Rectangle glyphHotRect = new Rectangle(fullGlyphHotRect.Location, new Size(fullGlyphHotRect.Width - hiddenWidth, fullGlyphHotRect.Height)); 
			return glyphHotRect;		   
		}
		protected virtual bool DrawHotRatingItem(ControlGraphicsInfoArgs info, Rectangle itemRect, int index, decimal rating, decimal hotValue) {
			RatingControlViewInfo viewInfo = info.ViewInfo as RatingControlViewInfo;
			Rectangle glyphRect = new Rectangle(Point.Empty, new Size(itemRect.Width, viewInfo.GetGlyphSize().Height));
			int ratingFloor = Convert.ToInt32(Decimal.Floor(rating));
			decimal ratingFraction = rating - ratingFloor;
			int hotFloor = Convert.ToInt32(Decimal.Floor(hotValue));
			decimal hotFraction = hotValue - hotFloor;
			if(index <= hotFloor) {
				info.Cache.Paint.DrawImage(info.Graphics, viewInfo.HoverGlyph, itemRect, glyphRect, null);
				return true;
			}
			if(hotFraction > 0 && index == hotFloor + 1) {
				Rectangle glyphHotRect = GetGlyphHotRect(viewInfo, itemRect, glyphRect, hotFraction);
				Rectangle itemHotRect = viewInfo.Helper.OffsetRect(glyphHotRect, itemRect.Location);
				info.Cache.Paint.DrawImage(info.Graphics, viewInfo.HoverGlyph, itemHotRect, glyphHotRect, null);
				if(viewInfo.Item.FillPrecision == RatingItemFillPrecision.Half || (viewInfo.Item.FillPrecision == RatingItemFillPrecision.Exact && index != ratingFloor)) {
					Rectangle glyphNormalRect = viewInfo.Helper.CalcNotFractionRect(glyphRect, glyphHotRect);
					Rectangle itemNormalRect = viewInfo.Helper.OffsetRect(glyphNormalRect, itemRect.Location);
					Image curGlyph = viewInfo.GetCurrentImage(viewInfo, index, ratingFloor, hotFraction);
					info.Cache.Paint.DrawImage(info.Graphics, curGlyph, itemNormalRect, glyphNormalRect, null);
					return true;
				}
				if(viewInfo.Item.FillPrecision == RatingItemFillPrecision.Exact && index == ratingFloor) {
					DrawHotRatingItemWithExactValue(info, itemRect, glyphRect, ratingFraction, hotFraction, glyphHotRect);
					return true;
				}			 
			}
			return false;
		}		
		protected virtual void DrawHotRatingItemWithExactValue(ControlGraphicsInfoArgs info, Rectangle itemRect, Rectangle glyphRect, decimal ratingFraction, decimal hotFraction, Rectangle glyphHotRect) {
			RatingControlViewInfo viewInfo = info.ViewInfo as RatingControlViewInfo;
			Rectangle glyphEmptySection;
			Rectangle itemEmptySection;			
			if(ratingFraction > hotFraction) {   
				Rectangle fullGlyphRect = new Rectangle(Point.Empty, viewInfo.GetGlyphSize());
				Rectangle fullGlyphRatingRect = viewInfo.Helper.CalcFractionRect(fullGlyphRect, ratingFraction);
				if(fullGlyphRatingRect.Right > glyphRect.Right)
					glyphRect.Width -= fullGlyphRatingRect.Width - glyphRect.Width;
				List<Rectangle> sections = viewInfo.Helper.CalcGlyphRatingAndEmptySection(glyphRect, glyphHotRect, fullGlyphRatingRect);
				Rectangle imageRatingSection = sections[0];
				glyphEmptySection = sections[1];
				Rectangle itemRatingSection = viewInfo.Helper.OffsetRect(imageRatingSection, itemRect.Location);
				info.Cache.Paint.DrawImage(info.Graphics, viewInfo.CheckedGlyph, itemRatingSection, imageRatingSection, null);	 
			}
			else
				glyphEmptySection = viewInfo.Helper.CalcGlyphEmptySection(glyphRect, glyphHotRect);
			itemEmptySection = viewInfo.Helper.OffsetRect(glyphEmptySection, itemRect.Location);
			info.Cache.Paint.DrawImage(info.Graphics, viewInfo.Glyph, itemEmptySection, glyphEmptySection, null); 
		}		
		protected virtual void DrawNormalStateItem(ControlGraphicsInfoArgs info, int index, decimal rating, Rectangle itemRect) {
			RatingControlViewInfo viewInfo = info.ViewInfo as RatingControlViewInfo;
			Rectangle glyphRect = new Rectangle(Point.Empty, new Size(itemRect.Width, viewInfo.GetGlyphSize().Height));			
			int ratingFloor = Convert.ToInt32(Decimal.Floor(rating));
			decimal ratingFraction = rating - ratingFloor;
			if(rating == 0) {
				info.Cache.Paint.DrawImage(info.Graphics, viewInfo.Glyph, itemRect, glyphRect, null);
			}
			else {				
				if(index <= ratingFloor - 1) {
					info.Cache.Paint.DrawImage(info.Graphics, viewInfo.CheckedGlyph, itemRect, glyphRect, null);
					return;
				}
				if(ratingFraction > 0 && index == ratingFloor) {
					Rectangle glyphRatingRect = Rectangle.Empty;
					Rectangle glyphNormalRect = Rectangle.Empty;
					if(glyphRect.Width < viewInfo.GetGlyphSize().Width) {
						List<Rectangle> rects = viewInfo.Helper.CalcPartialRatingAndNormalRects(itemRect, glyphRect, ratingFraction);
						glyphRatingRect = rects[0];
						glyphNormalRect = rects[1];
					}
					else {
						glyphRatingRect = viewInfo.Helper.CalcFractionRect(glyphRect, ratingFraction);
						glyphNormalRect = viewInfo.Helper.CalcNotFractionRect(glyphRect, glyphRatingRect);
					}
					if(glyphRatingRect != Rectangle.Empty) {
						Rectangle itemRatingRect = viewInfo.Helper.OffsetRect(glyphRatingRect, itemRect.Location);
						info.Cache.Paint.DrawImage(info.Graphics, viewInfo.CheckedGlyph, itemRatingRect, glyphRatingRect, null);
					}
					if(glyphNormalRect != Rectangle.Empty) {
						Rectangle itemNormalRect = viewInfo.Helper.OffsetRect(glyphNormalRect, itemRect.Location);
						info.Cache.Paint.DrawImage(info.Graphics, viewInfo.Glyph, itemNormalRect, glyphNormalRect, null);
					}
					return;
				}
				info.Cache.Paint.DrawImage(info.Graphics, viewInfo.Glyph, itemRect, glyphRect, null);			  
			}
		}
		protected virtual void DrawItem(ControlGraphicsInfoArgs info, Rectangle rect, int index, decimal hotValue) {
			RatingControlViewInfo viewInfo = info.ViewInfo as RatingControlViewInfo;
			decimal rating = viewInfo.Item.GetRating(viewInfo.EditValue);			
			DrawItemCore(info, rating, rect, index, hotValue);			
		}   
	}
}
