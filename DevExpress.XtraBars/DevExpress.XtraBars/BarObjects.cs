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
using System.Windows.Forms;
using System.ComponentModel;
using System.Collections;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.XtraBars;
using DevExpress.XtraBars.Painters;
using DevExpress.XtraBars.ViewInfo;
using DevExpress.XtraBars.Objects;
using DevExpress.XtraBars.Controls;
using DevExpress.Skins;
namespace DevExpress.XtraBars.Objects {
	public class ObjectElPainter {
		public static DevExpress.Utils.Paint.XPaint Painter = DevExpress.Utils.Paint.XPaint.CreateDefaultPainter();
		public virtual void Draw(GraphicsInfoArgs e, ObjectElViewInfo v) { }
	}
	public class ObjectElCalcSizeArgs {
		public Graphics Graphics;
		public ObjectElCalcSizeArgs(Graphics g) {
			this.Graphics = g;
		}
	}
	public class ObjectElViewInfo {
		ObjectEl element;
		bool ready;
		public ObjectElViewInfo(ObjectEl element) {
			this.ready = false;
			this.element = element;
		}
		public virtual void ClearReady() {
			this.ready = false;
		}
		public virtual void CalcViewInfo(Graphics g, Rectangle r) {
			this.ready = true;
		}
		public virtual Size CalcSize(ObjectElCalcSizeArgs e) {
			return Size.Empty;
		}
		public ObjectEl Element { get { return element; } }
		public bool IsReady { get { return ready; } }
	}
	public class ObjectEl : IDisposable {
		Control control;
		ArrayList children;
		public event EventHandler VisualChanged;
		Rectangle elementRectangle;
		int lockUpdate;
		ObjectElPainter painter;
		ObjectElViewInfo viewInfo;
		public ObjectEl() {
			this.children = null;
			this.control = null;
			this.elementRectangle = Rectangle.Empty;
			this.lockUpdate = 0;
			this.painter = CreatePainter();
			this.viewInfo = CreateViewInfo();
			if(HasChildren) CreateChildrenList();
		}
		public virtual void Dispose() {
		}
		public ObjectElViewInfo CreateViewInfo() {
			return CreateViewInfoCore();
		}
		protected virtual ObjectElViewInfo CreateViewInfoCore() {
			return new ObjectElViewInfo(this);
		}
		protected virtual ObjectElPainter CreatePainter() {
			return new ObjectElPainter();
		}
		public virtual void DoDraw(GraphicsInfoArgs e) {
			Painter.Draw(e, ViewInfo);
		}
		protected virtual ObjectElPainter Painter { get { return painter; } }
		protected virtual void UpdateChildrenControl() {
			if(Children == null) return;
			foreach(ObjectEl el in Children) el.Control = Control;
		}
		protected void CreateChildrenList() {
			if(Children != null) return;
			children = new ArrayList();
		}
		protected virtual ArrayList Children { get { return children; } }
		public virtual bool HasChildren { get { return false; } }
		public Control Control { 
			get { return control; } 
			set {
				if(Control == value) return;
				control = value;
				UpdateChildrenControl();
			}
		}
		public virtual ObjectElViewInfo ViewInfo { get { return viewInfo; } }
		public virtual Rectangle ElementRectangle { 
			get { return elementRectangle; } 
			set {
				if(ElementRectangle == value) return;
				elementRectangle = value;
				Changed();
			}
		}
		public virtual bool IsEmpty { get { return ElementRectangle.IsEmpty; } }
		public virtual void Invalidate() {
			if(Control == null || IsEmpty) return;
			Control.Invalidate(ElementRectangle);
		}
		protected virtual void UpdateViewInfo() {
			if(IsUpdating) return;
			if(ViewInfo == null) CreateViewInfo();
			if(ViewInfo == null) return;
			ViewInfo.CalcViewInfo(null, ElementRectangle);
			ViewInfoUpdated();
		}
		protected virtual void ViewInfoUpdated() {
		}
		protected virtual void Changed() {
			if(IsUpdating) return;
			UpdateViewInfo();
			if(VisualChanged != null) 
				VisualChanged(this, EventArgs.Empty);
		}
		public virtual void BeginUpdate() {
			lockUpdate ++;
		}
		public virtual void CancelUpdate() {
			--lockUpdate;
		}
		public virtual void EndUpdate() {
			if(--lockUpdate == 0) Changed();
		}
		public virtual bool IsUpdating { get { return lockUpdate != 0; } }
	}
	public class BorderEl : ObjectEl {
		[Flags]
			public enum BorderElSides { None = 0, Left = 1, Top = 2, Right = 4, Bottom = 8, All = Left | Top | Right | Bottom };
		BarItemBorderStyle border;
		Color borderColor;
		BorderElSides sides;
		public BorderEl() {
			sides = BorderElSides.All;
			border = BarItemBorderStyle.None;
			borderColor = SystemColors.WindowFrame;
			CreateViewInfo();
		}
		public BarItemBorderStyle Border {
			get { return border; }
			set {
				if(value == Border) return;
				border = value;
				Changed();
			}
		}
		public Color BorderColor {
			get { return borderColor; }
			set {
				if(BorderColor == value) return;
				borderColor = value;
				Changed();
			}
		}
		public BorderElSides Sides { 
			get { return sides; } 
			set {
				if(Sides == value) return;
				sides = value;
				Changed();
			}
		}
		public Rectangle InnerRectangle {
			get { return ((BorderElViewInfo)ViewInfo).InnerRectangle; } 
		}
		protected override ObjectElViewInfo CreateViewInfoCore() {
			return new BorderElViewInfo(this);
		}
		protected override ObjectElPainter CreatePainter() {
			return new BorderElPainter();
		}
	}
	public class BorderElViewInfo : ObjectElViewInfo {
		public Rectangle InnerRectangle;
		public BorderElViewInfo(BorderEl element) : base(element) {
			InnerRectangle = Rectangle.Empty;
		}
		public new BorderEl Element { get { return base.Element as BorderEl; } }
		public override void CalcViewInfo(Graphics g, Rectangle r) {
			if(r.IsEmpty) return;
			Rectangle indt = GetIndents();
			r.X += indt.X;
			r.Width -= (indt.X + indt.Width);
			r.Y += indt.Y;
			r.Height -= (indt.Y + indt.Height);
			InnerRectangle = r;
			base.CalcViewInfo(g, r);
		}
		public Rectangle GetIndents() {
			Rectangle r = Rectangle.Empty;
			BarItemBorderStyle elType = ((BorderEl)Element).Border;
			switch(elType) {
				case BarItemBorderStyle.None : break;
				case BarItemBorderStyle.Single : r = new Rectangle(1, 1, 1, 1); break;
				case BarItemBorderStyle.Lowered : r = new Rectangle(1, 1, 1, 1); break;
				case BarItemBorderStyle.Raised : r = new Rectangle(1, 1, 1, 1); break;
			}
			if((((BorderEl)Element).Sides & BorderEl.BorderElSides.Left) == 0) r.X = 0;
			if((((BorderEl)Element).Sides & BorderEl.BorderElSides.Top) == 0) r.Y = 0;
			if((((BorderEl)Element).Sides & BorderEl.BorderElSides.Right) == 0) r.Width = 0;
			if((((BorderEl)Element).Sides & BorderEl.BorderElSides.Bottom) == 0) r.Height = 0;
			return r;
		}
	}
	public class BorderElPainter : ObjectElPainter {
		public override void Draw(GraphicsInfoArgs e, ObjectElViewInfo v) {
			BorderElViewInfo vi = v as BorderElViewInfo;
			if(vi == null || vi.Element.ElementRectangle == vi.Element.InnerRectangle) return;
			Painter.DrawRectangle(e.Graphics, e.Cache.GetPen(vi.Element.BorderColor), vi.Element.ElementRectangle);
		}
	}
	public class TitleBarElCalcSizeArgs : ObjectElCalcSizeArgs {
		public Size InnerSize;
		public TitleBarElCalcSizeArgs(Graphics g, Size innerSize) : base(g) {
			this.InnerSize = innerSize;
		}
	}
	public class TitleBarControl : CustomBarControl {
		TitleBarEl titleBar;
		BarLinkState defaultLinkState;
		internal TitleBarControl(TitleBarEl titleBar, BarManager ABarManager, IList links) : base(ABarManager, null) {
			this.defaultLinkState = BarLinkState.Normal;
			this.linksSource = links;
			this.titleBar = titleBar;
		}
		protected override DevExpress.Accessibility.BaseAccessible CreateAccessibleInstance() {
			return null;
		}
		public BarLinkState DefaultLinkState { 
			get { return defaultLinkState; }
			set {
				defaultLinkState = value;
			}
		}
		public TitleBarEl TitleBar {
			get { return titleBar; }
		}
		protected internal override void UpdateVisibleLinks() {
			base.UpdateVisibleLinks();
			SetLinksOwner(this);
		}
	}
	public class TitleBarControlViewInfo : BarControlViewInfo {
		public TitleBarControlViewInfo(BarManager manager, BarDrawParameters parameters, CustomControl bar) : base(manager, parameters, bar) {
		}
		public new virtual TitleBarControl BarControl { get { return base.BarControl as TitleBarControl; } }
		public override bool DrawDragBorder {
			get { return false; }
		}
		public override int VertIndent { get { return 0; } }
		public override int HorzIndent { get { return 0; } }
		protected override void UpdateAppearance() {
			base.UpdateAppearance();
			Appearance.Normal.BackColor = BarControl.TitleBar.CurrentBackColor;
			Appearance.Normal.BackColor2 = Color.Empty; 
		}
		public override void CalcViewInfo(Graphics g, object sourceObject, Rectangle rect) {
			base.CalcViewInfo(g, sourceObject, rect);
			foreach(BarControlRowViewInfo row in Rows) {
				foreach(BarLinkViewInfo lv in row.Links) {
					if(lv.LinkState == BarLinkState.Normal && BarControl.DefaultLinkState != BarLinkState.Normal) {
						lv.LinkState = BarControl.DefaultLinkState;
					}
				}
			}
		}
		public override void OnLinkStateUpdated(BarLinkViewInfo lv) {
			if(lv.LinkState == BarLinkState.Normal && BarControl.DefaultLinkState != BarLinkState.Normal) {
				lv.LinkState = BarControl.DefaultLinkState;
			}
		}
	}
	public class TitleBarEl : ObjectEl {
		BarManager manager;
		TitleBarControl barControl;
		string caption;
		BorderEl border;
		Color selectedForeColor, selectedBackColor, foreColor, backColor, currentBackColor;
		Font font;
		StringFormat titleFormat;
		bool selected, showCloseButton, showMenuButton;
		BarButtonItem closeItem, customizeItem;
		PopupControl popupMenu = null;
		public event EventHandler CloseItemClick;
		public TitleBarEl(BarManager manager) {
			this.barControl = null;
			this.currentBackColor = SystemColors.Control;
			this.showCloseButton = true;
			this.showMenuButton = true; 
			this.manager = manager;
			this.titleFormat = new StringFormat(StringFormatFlags.NoWrap);
			this.titleFormat.Trimming = StringTrimming.EllipsisCharacter;
			this.titleFormat.LineAlignment = StringAlignment.Center;
			this.font = Control.DefaultFont;
			this.selected = false;
			this.foreColor = SystemColors.ControlText;
			this.backColor = SystemColors.Control;
			this.selectedForeColor = SystemColors.ActiveCaptionText;
			this.selectedBackColor = SystemColors.InactiveCaption;
			Manager.StartCustomization += new EventHandler(ManagerCustomizationChanged);
			Manager.EndCustomization += new EventHandler(ManagerCustomizationChanged);
			this.border = CreateBorder(); 
			this.border.VisualChanged += new EventHandler(ChildChanged);
			CreateLinks();
			CreateViewInfo();
			Children.Add(border);
			UpdateBrush();
		}
		protected internal virtual bool IsRightToLeft { get { return manager != null && manager.IsRightToLeft; } }
		protected virtual BorderEl CreateBorder() {
			BorderEl border = new BorderEl();
			border.Border = BarItemBorderStyle.Single;
			border.BorderColor = SystemColors.ControlDark;
			return border;
		}
		public override void Dispose() {
			if(closeItem != null)
				closeItem.ItemClick -= new ItemClickEventHandler(OnCloseItemClick);
			Manager.StartCustomization -= new EventHandler(ManagerCustomizationChanged);
			Manager.EndCustomization -= new EventHandler(ManagerCustomizationChanged);
			DestroyGlyphs();
			if(BarControl != null)
				BarControl.Dispose();
			barControl = null;
			base.Dispose();
		}
		void ManagerCustomizationChanged(object sender, EventArgs e) {
			if(BarControl != null) BarControl.Enabled = !Manager.IsCustomizing;
		}
		public PopupControl Menu { 
			get { return popupMenu; }
			set { 
				if(Menu == value) return;
				popupMenu = value; 
				if(ShowCloseButton || ShowMenuButton) RecreateLinks();
			}
		}
		public bool CanShowMenuButton {
			get { return ShowMenuButton && Menu != null; }
		}
		public bool ShowCloseButton {
			get { return showCloseButton; }
			set {
				if(ShowCloseButton == value) return;
				showCloseButton = value;
				RecreateLinks();
			}
		}
		public virtual BarButtonItem CloseItem { get { return closeItem; } }
		public bool ShowMenuButton {
			get { return showMenuButton; }
			set {
				if(ShowMenuButton == value) return;
				showMenuButton = value;
				RecreateLinks();
			}
		}
		public BarManager Manager { get { return manager; } }
		internal TitleBarControl BarControl { get { return barControl; } }
		void DestroyGlyphs() {
			if(closeItem == null || closeItem.Glyph == null) return;
			closeItem.Glyph.Dispose();
			closeItem.Glyph = null;
			if(customizeItem.Glyph != null) customizeItem.Glyph.Dispose();
			customizeItem.Glyph = null;
		}
		void UpdateGlyphs() {
			DestroyGlyphs();
			closeItem.Glyph = GetImage(closeImage, Selected);
			customizeItem.Glyph = GetImage(menuImage, Selected);
		}
		void OnCloseItemClick(object sender, ItemClickEventArgs e) {
			if(CloseItemClick != null)
				CloseItemClick(this, EventArgs.Empty);
		}
		public void RecreateLinks() {
			CreateLinks();
			Changed();
		}
		ArrayList links = new ArrayList();
		void CreateLinks() {
			links.Clear();
			if(barControl == null) {
				barControl = new TitleBarControl(this, Manager, links);
				barControl.Init();
			}  else {
				if(closeItem != null)
					closeItem.ItemClick -= new ItemClickEventHandler(OnCloseItemClick);
				closeItem.Dispose();
				customizeItem.Dispose();
			}
			ManagerCustomizationChanged(Manager, EventArgs.Empty);
			closeItem = new BarButtonItem(null, true);
			closeItem.Caption = "Close";
			closeItem.PaintStyle = BarItemPaintStyle.CaptionInMenu;
			closeItem.ItemClick += new ItemClickEventHandler(OnCloseItemClick);
			customizeItem = new BarButtonItem(null, true);
			customizeItem.Caption = "Menu";
			customizeItem.AllowDrawArrowInMenu = false;
			customizeItem.ButtonStyle = BarButtonStyle.DropDown;
			customizeItem.DropDownControl = Menu;
			customizeItem.ActAsDropDown = true;
			customizeItem.PaintStyle = BarItemPaintStyle.CaptionInMenu;
			UpdateGlyphs();
			customizeItem.Manager = Manager;
			closeItem.Manager = Manager;
			if(CanShowMenuButton) links.Add(customizeItem.CreateLink(null, null));
			if(ShowCloseButton) links.Add(closeItem.CreateLink(null, null));
			BarControl.Visible = false;
			BarControl.ClearHash();
			BarControl.UpdateVisibleLinks();
			BarControl.ViewInfo.ClearReady();
		}
		internal const int closeImage = 0, menuImage = 1, imageWidth = 10, imageHeight = 9;
		protected virtual Size ButtonImageSize { get { return new Size(11, 13); } }
		protected virtual Bitmap GetImage(int index, bool inverted) {
			Bitmap res = new Bitmap(ButtonImageSize.Width, ButtonImageSize.Height);
			Bitmap total = Manager.GetController().GetEmbeddedBitmap("BarWindowButtons");
			total.MakeTransparent(Color.White);
			Graphics g = Graphics.FromImage(res);
			Rectangle dest = new Rectangle(1 + (ButtonImageSize.Width - imageWidth) / 2, 1 + (ButtonImageSize.Height - imageHeight) / 2, imageWidth, imageHeight);
			Rectangle src = new Rectangle(index * imageWidth, 0, imageWidth, imageHeight);
			g.DrawImage(total, dest, src.X, src.Y, src.Width, src.Height, GraphicsUnit.Pixel, null);
			g.Dispose();
			total.Dispose();
			return res;
		}
		protected override void ViewInfoUpdated() {
			if(BarControl.Parent != Control)
				BarControl.Parent = Control;
			BarControl.Bounds = ((TitleBarElViewInfo)ViewInfo).ButtonsRectangle;
			BarControl.LayoutChanged();
			BarControl.Visible = true;
		}
		public override bool HasChildren { get { return true; } }
		public int CalcHeight(Graphics g, int innerHeight) {
			return ViewInfo.CalcSize(new TitleBarElCalcSizeArgs(g, new Size(0, innerHeight))).Height;
		}
		public StringFormat TitleFormat {
			get { return titleFormat; }
			set {
				if(TitleFormat.Equals(value)) return;
				titleFormat = value.Clone() as StringFormat;
				titleFormat.LineAlignment = StringAlignment.Center;
				titleFormat.Trimming = StringTrimming.EllipsisCharacter;
				titleFormat.FormatFlags = StringFormatFlags.NoWrap | StringFormatFlags.LineLimit;
				Changed();
			}
		}
		public Font Font { 
			get { return font; }
			set {
				if(Font.Equals(value)) return;
				font = value;
				Changed();
			}
		}
		protected virtual void UpdateBrush() {
			this.currentBackColor = Selected ? SelectedBackColor : BackColor;
		}
		public Color CurrentBackColor { 
			get { return currentBackColor; }
			set {
				if(CurrentBackColor == value) return;
				currentBackColor = value;
			}
		}
		public Color SelectedForeColor {
			get { return selectedForeColor; } 
			set {
				if(SelectedForeColor == value) return;
				selectedForeColor = value;
				Changed();
			}
		}
		public string Caption {
			get { return caption; }
			set {
				if(Caption == value) return;
				caption = value;
				if(IsUpdating) return;
				Invalidate();
			}
		}
		public bool Selected { 
			get { return selected; }
			set {
				if(Selected == value) return;
				selected = value;
				UpdateBrush();
				UpdateGlyphs();
				Invalidate();
				BarControl.Invalidate();
			}
		}
		public Color SelectedBackColor {
			get { return selectedBackColor; } 
			set {
				if(SelectedBackColor == value) return;
				selectedBackColor = value;
				Changed();
			}
		}
		public Color BackColor {
			get { return backColor; } 
			set {
				if(BackColor == value) return;
				backColor = value;
				Changed();
			}
		}
		public Color ForeColor {
			get { return foreColor; } 
			set {
				if(ForeColor == value) return;
				foreColor = value;
				Changed();
			}
		}
		public BorderEl Border {
			get { return border; } 
		}
		protected virtual void ChildChanged(object sender, EventArgs e) {
			Changed();
		}
		protected override ObjectElViewInfo CreateViewInfoCore() {
			return new TitleBarElViewInfo(this);
		}
		protected override ObjectElPainter CreatePainter() {
			return new TitleBarElPainter();
		}
	}
	public class TitleBarElViewInfo : ObjectElViewInfo {
		public new TitleBarEl Element { get { return base.Element as TitleBarEl; } }
		public Rectangle TitleRectangle, CaptionRectangle, ButtonsRectangle, ContentRectangle;
		public TitleBarElViewInfo(TitleBarEl element) : base(element) {
			ButtonsRectangle = TitleRectangle = CaptionRectangle = Rectangle.Empty;
		}
		protected virtual Rectangle TitleBoundsFromContentBounds(Rectangle innerBounds) {
			innerBounds.Inflate(4, 2);
			return innerBounds;
		}
		protected virtual Rectangle ContentBoundsFromTitleBounds(Rectangle titleBounds) {
			titleBounds.Inflate(-4, -2);
			return titleBounds;
		}
		public override void CalcViewInfo(Graphics g, Rectangle source) {
			if(source.IsEmpty) return;
			Element.BeginUpdate();
			try {
				Element.Border.ElementRectangle = source;
				TitleRectangle = Element.Border.InnerRectangle;
				ContentRectangle = ContentBoundsFromTitleBounds(TitleRectangle);
				CaptionRectangle = ContentRectangle;
				ButtonsRectangle = ContentRectangle;
				if(Element.BarControl.VisibleLinks.Count < 1) ButtonsRectangle.Width = 0;
				else
					ButtonsRectangle.Width = Element.BarControl.CalcSize(TitleRectangle.Width - 2).Width;
				ButtonsRectangle.X = ContentRectangle.Right - ButtonsRectangle.Width;
				CaptionRectangle.Width = ButtonsRectangle.X - CaptionRectangle.X;
				if(CaptionRectangle.Width < 1) CaptionRectangle = Rectangle.Empty;
				if(Element.IsRightToLeft)
					RotateRects(source);
			}
			finally {
				Element.CancelUpdate();
			}
			base.CalcViewInfo(g, source);
		}
		private void RotateRects(Rectangle source) {
			ContentRectangle.X = source.Right - (ContentRectangle.X + ContentRectangle.Width);
			CaptionRectangle.X = source.Right - (CaptionRectangle.X + CaptionRectangle.Width);
			ButtonsRectangle.X = source.Right - (ButtonsRectangle.X + ButtonsRectangle.Width);
		}
		protected virtual int CalcButtonsHeight() {
			if(Element.BarControl.VisibleLinks.Count > 0)
				return Element.BarControl.CalcSize(1000).Height;
			return 0;
		}
		public override Size CalcSize(ObjectElCalcSizeArgs e) {
			TitleBarElCalcSizeArgs ee = e as TitleBarElCalcSizeArgs;
			int captionHeight = 0;
			Graphics g = e.Graphics;
			Graphics oldG = g;
			if(g == null) g = Graphics.FromHwnd(IntPtr.Zero);
			try {
				captionHeight = CalcButtonsHeight();
				captionHeight = Math.Max((int)(ObjectElPainter.Painter.CalcTextSize(g, "Wg", Element.Font, Element.TitleFormat, 0).Height) + 3, captionHeight);
				captionHeight = Math.Max(ee.InnerSize.Height, captionHeight);
				captionHeight += ((BorderElViewInfo)Element.Border.ViewInfo).GetIndents().Bottom;
				captionHeight = TitleBoundsFromContentBounds(new Rectangle(0,0,0,captionHeight)).Height;
			}
			finally {
				if(oldG == null) g.Dispose();
			}
			return new Size(0, captionHeight);
		}
	}
	public class TitleBarElPainter : ObjectElPainter {
		public override void Draw(GraphicsInfoArgs e, ObjectElViewInfo v) { 
			TitleBarElViewInfo vi = v as TitleBarElViewInfo;
			if(vi == null) return;
			vi.Element.Border.DoDraw(e);
			e.Graphics.ExcludeClip(vi.ButtonsRectangle);
			Painter.FillRectangle(e.Graphics, e.Cache.GetSolidBrush(vi.Element.Selected ? vi.Element.SelectedBackColor : vi.Element.BackColor), vi.TitleRectangle);
			if(!vi.CaptionRectangle.IsEmpty) {
				Painter.DrawString(e.Cache, vi.Element.Caption, vi.Element.Font, e.Cache.GetSolidBrush(vi.Element.Selected ? vi.Element.SelectedForeColor : vi.Element.ForeColor), vi.CaptionRectangle, vi.Element.TitleFormat);
			}
		}
	}
	public class FloatingBarTitleBarEl : TitleBarEl {
		protected override Size ButtonImageSize { get { return new Size(13, 15); } }
		public FloatingBarTitleBarEl(BarManager manager) : base(manager) {
			Border.Border = BarItemBorderStyle.None;
			Font = new Font(Control.DefaultFont, FontStyle.Bold);
			SelectedForeColor = ForeColor = ControlPaint.Light(SystemColors.ControlText);
			SelectedBackColor = BackColor = SystemColors.ControlDark;
		}
		protected override ObjectElViewInfo CreateViewInfoCore() {
			return new FloatingBarTitleBarElViewInfo(this);
		}
	}
	public class FloatingBarTitleBarElViewInfo : TitleBarElViewInfo {
		public FloatingBarTitleBarElViewInfo(TitleBarEl element) : base(element) {
		}
		protected virtual SkinPaddingEdges ContentMargin { 
			get { 
				SkinPaddingEdges res = new SkinPaddingEdges(4, 1, 1, 1);
				res.Left = 4;
				return res;
			} 
		}
		protected override Rectangle ContentBoundsFromTitleBounds(Rectangle titleBounds) {
			return ContentMargin.Deflate(titleBounds);   
		}
		protected override Rectangle TitleBoundsFromContentBounds(Rectangle innerBounds) {
			return ContentMargin.Inflate(innerBounds);
		}
	}
	public class SkinFloatingBarTitleBarElViewInfo : FloatingBarTitleBarElViewInfo {
		public SkinFloatingBarTitleBarElViewInfo(TitleBarEl element) : base(element) { 
		}
		protected override SkinPaddingEdges ContentMargin {
			get {
				object left = BarSkins.GetSkin(Element.Manager.GetController().LookAndFeel)[BarSkins.SkinFloatingBarTitle].Properties[BarSkins.OptFloatingBarTitleLeftIndent];
				object right = BarSkins.GetSkin(Element.Manager.GetController().LookAndFeel)[BarSkins.SkinFloatingBarTitle].Properties[BarSkins.OptFloatingBarTitleRightIndent];
				object top = BarSkins.GetSkin(Element.Manager.GetController().LookAndFeel)[BarSkins.SkinFloatingBarTitle].Properties[BarSkins.OptFloatingBarTitleTopIndent];
				object bottom = BarSkins.GetSkin(Element.Manager.GetController().LookAndFeel)[BarSkins.SkinFloatingBarTitle].Properties[BarSkins.OptFloatingBarTitleBottomIndent];
				if(left == null || right == null || top == null || bottom == null)
					return base.ContentMargin;
				SkinPaddingEdges res = new SkinPaddingEdges((int)left, (int)top, (int)right, (int)bottom);
				res.Left = (int)left;
				return res;
			}
		}
	}
}
