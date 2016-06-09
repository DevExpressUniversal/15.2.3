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
using System.Drawing;
using System.Threading;
using System.Collections;
using DevExpress.XtraBars;
using DevExpress.XtraEditors;
using System.ComponentModel;
using System.Drawing.Imaging;
using System.Windows.Forms.Design;
using System.ComponentModel.Design;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraEditors.Drawing;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.Utils.Drawing.Animation;
using DevExpress.Skins;
using DevExpress.XtraBars.Helpers;
using DevExpress.XtraBars.Ribbon.Handler;
using DevExpress.XtraBars.Ribbon.Helpers;
using DevExpress.XtraBars.Ribbon.Internal;
using DevExpress.XtraBars.Ribbon;
using DevExpress.XtraBars.Ribbon.Drawing;
using DevExpress.Skins.XtraForm;
using DevExpress.Utils.Drawing.Helpers;
using DevExpress.Utils.Text;
using DevExpress.XtraBars.Docking2010;
using System.Collections.Generic;
using System.Linq;
using DevExpress.XtraBars.Utils;
namespace DevExpress.XtraBars.Ribbon.ViewInfo {
	public class RibbonCaptionViewInfo {
		const int minTextWidth = 40;
		RibbonViewInfo viewInfo;
		Rectangle bounds, contentBounds, clientBounds, textBounds;
		ObjectPainter captionPainter;
		public RibbonCaptionViewInfo(RibbonViewInfo viewInfo) {
			this.viewInfo = viewInfo;
			this.textBounds = this.clientBounds = this.contentBounds = this.bounds = Rectangle.Empty;
		}
		public RibbonViewInfo ViewInfo { get { return viewInfo; } }
		public RibbonControl Ribbon { get { return ViewInfo.Ribbon; } }
		public Rectangle Bounds { get { return bounds; } set { bounds = value; } }
		public Rectangle ContentBounds { get { return contentBounds; } set { contentBounds = value; } }
		public Rectangle ClientBounds { get { return clientBounds; } set { clientBounds = value; } }
		public Rectangle TextBounds { 
			get { return textBounds; } 
			set { 
				textBounds = value;
				OnTextBoundsChanged();
			} 
		}
		protected virtual void OnTextBoundsChanged() {
			if(Form != null && Form.FormPainter != null && Form.FormPainter.AllowHtmlText) {
				if(TextBounds.IsEmpty) {
					StringInfo = null;
				} else {
					StringInfo = StringPainter.Default.Calculate(GInfo.Graphics, ViewInfo.PaintAppearance.FormCaption, GetFormText(), TextBounds.Width);
					StringInfo.SetLocation(TextBounds.Location);
				}
			}
		}
		public RibbonForm Form { get { return ViewInfo.Form; } }
		public StringInfo StringInfo { get; set; }
		public ObjectPainter CaptionPainter {
			get {
				if(captionPainter == null) captionPainter = CreateCaptionPainter();
				return captionPainter;
			}
		}
		public virtual bool IsWindowActive {
			get { return FormPainter == null ? true : FormPainter.IsWindowActiveCore; }
		}
		protected virtual ObjectPainter CreateCaptionPainter() { return new RibbonCaptionPainter(); }
		internal string GetCaptionSkinName() {  
			if(Form != null && Form.IsWindowMinimized)
				return RibbonSkins.SkinFormCaptionMinimized;
			if(!ViewInfo.IsAllowDisplayRibbon || ViewInfo.GetRibbonStyle() == RibbonControlStyle.MacOffice) {
				SkinElement elem = RibbonSkins.GetSkin(ViewInfo.Provider)[RibbonSkins.SkinFormCaptionNoRibbon];
				if(elem != null)
					return RibbonSkins.SkinFormCaptionNoRibbon;
			}
			return RibbonSkins.SkinFormCaption;
		}
		public virtual SkinElementInfo GetCaptionInfo() {
			string skinName = GetCaptionSkinName();
			SkinElementInfo res = new SkinElementInfo(RibbonSkins.GetSkin(ViewInfo.Provider)[skinName], Bounds);
			res.CorrectImageFormRTL = ViewInfo.Form != null && ViewInfo.Form.RightToLeftLayout;
			res.ImageIndex = IsWindowActive ? 0 : 1;
			return res;
		}
		public virtual SkinElementInfo GetCaptionDrawInfo() {
			if(IsBorderedGlassForm) return new SkinElementInfo(null);
			SkinElementInfo info = GetCaptionInfo();
			Rectangle bounds = info.Bounds;
			bounds.X -= ViewInfo.GetFormFrameLeftWidth();
			bounds.Width += (ViewInfo.GetFormFrameLeftWidth() + ViewInfo.GetFormFrameRightWidth());
			info.Bounds = bounds;
			return info;
		}
		public GraphicsInfo GInfo { get { return ViewInfo.GInfo; } }
		protected virtual Rectangle CalcCaptionBounds() {
			Form form = ViewInfo.Form;
			if(form != null && form.WindowState == FormWindowState.Minimized) {
				int formWidth = form.Width;
				RibbonForm ribbonForm = form as RibbonForm;
				if(ribbonForm != null) {
					formWidth = ribbonForm.CalcMinimizedClientSize().Width;
				}
				return new Rectangle(0, 0, formWidth, form.Height);
			}
			return new Rectangle(0, ViewInfo.Bounds.Y, ViewInfo.Bounds.Width, CalcCaptionHeight());
		}
		public virtual void CalcCaption() {
			Bounds = CalcCaptionBounds();
			ClientBounds = CalcClientBounds();
			if(!RibbonForm.UseAdvancedDisplayRectangle) {
				if(CaptionYOffset > 0) {
					clientBounds.Y += CaptionYOffset;
					clientBounds.Height -= CaptionYOffset;
				}
			}
			ContentBounds = CalcContentBounds();
			CalcIcon();
			CalcButtons();
			CalcToolbar();
			TextBounds = CalcTextBounds();
		}
		protected virtual Rectangle CalcClientBounds() {
			if(Form != null && Form.IsGlassForm)
				return Bounds;
			return ObjectPainter.GetObjectClientRectangle(GInfo.Graphics, SkinElementPainter.Default, GetCaptionInfo());
		}
		protected internal virtual void RecalcTextBounds() {
			GInfo.AddGraphics(null);
			try {
				TextBounds = CalcTextBounds();
			}
			finally {
				GInfo.ReleaseGraphics();
			}
		}
		public virtual string GetFormText() {
			string p1 = GetTextChild();
			string p2 = GetTextOwner();
			if(p1 == string.Empty) return p2;
			if(p2 == string.Empty) return p1;
			return string.Format("{0}{1}{2}", p1, Delimiter, p2);
		}
		protected internal virtual string GetTextChild() {
			if(Ribbon.ApplicationDocumentCaption != string.Empty) {
				if(Ribbon.ApplicationDocumentCaption == " ") return string.Empty;
				return Ribbon.ApplicationDocumentCaption;
			}
			if(Form == null || !Form.IsMdiContainer || Form.ActiveMdiChild == null) return string.Empty;
			string res = Form.ActiveMdiChild.Text;
			return res ?? string.Empty;
		}
		protected internal virtual string GetTextOwner() {
			if(Ribbon.ApplicationCaption != string.Empty) {
				if(Ribbon.ApplicationCaption == " ") return string.Empty; 
				return Ribbon.ApplicationCaption;
			}
			string res = (Form == null || Form.FormPainter == null) ? string.Empty : Ribbon.OriginalFormText;
			return res ?? string.Empty;
		}
		protected internal virtual string Delimiter { get { return " - "; } }
		protected internal virtual Rectangle GetTextChildBounds(Graphics g) {
			int width = Math.Min(TextBounds.Width, ViewInfo.PaintAppearance.FormCaption.CalcTextSize(g, GetTextChild(), 0).ToSize().Width);
			int x = ViewInfo.IsRightToLeft ? TextBounds.Right - width : TextBounds.X;
			return new Rectangle(x, ViewInfo.Caption.TextBounds.Y, width, TextBounds.Height);
		}
		protected internal virtual Rectangle GetTextDelimiterBounds(Graphics g) {
			int width = (int)ViewInfo.PaintAppearance.FormCaption.CalcTextSize(g, Delimiter, 0).Width;
			Rectangle childRect = GetTextChildBounds(g);
			Rectangle bounds = new Rectangle(ViewInfo.IsRightToLeft ? childRect.Left - width : childRect.Right, childRect.Y, width, childRect.Height);
			if(!TextBounds.Contains(bounds)) return Rectangle.Empty;
			return bounds;
		}
		protected internal virtual Rectangle GetTextOwnerBounds(Graphics g) {
			if(!HasTextChild) return TextBounds;
			Rectangle delimiterRect = GetTextDelimiterBounds(g);
			if(delimiterRect.IsEmpty) return Rectangle.Empty;
			Rectangle childRect = GetTextChildBounds(g);
			int width = TextBounds.Width - childRect.Width - delimiterRect.Width;
			return new Rectangle(ViewInfo.IsRightToLeft ? delimiterRect.Left - width : delimiterRect.Right, childRect.Y, width, childRect.Height);
		}
		protected internal virtual bool HasTextChild { get { return !string.IsNullOrEmpty(GetTextChild()) && GetTextChild() != " "; } }
		protected internal virtual void UpdateToolbar() {
			if(ViewInfo.Header.PageCategories.Count == 0 || ViewInfo.Form == null) return;
			CalcToolbar();
		}
		protected internal virtual void UpdateCaptionTextBounds() {
			if(ViewInfo.Header.PageCategories.Count == 0 || ViewInfo.Form == null) return;
			TextBounds = CalcTextBounds();
		}
		protected virtual Rectangle CalcTextBounds() {
			if(!IsCaptionVisible) return Rectangle.Empty;
			Rectangle caption = Rectangle.Empty;
			Size captionSize = CalcCaptionSize();
			caption = GetTextBoundsCore(GetBestCaptionArea(Bounds, false), captionSize);
			if(!IsIntersectCaption(caption, CaptionRects)) return caption;
			caption = GetTextBoundsCore(GetBestCaptionArea(Bounds, true, CaptionRects), captionSize);
			if(caption.Width >= captionSize.Width) return caption;
			return GetTextBoundsCore(GetBestCaptionArea(Bounds, false, CaptionRects), captionSize);
		}
		protected virtual Rectangle GetTextBoundsCore(Rectangle dest, Size size) {
			if(ViewInfo.GetRibbonStyle() == RibbonControlStyle.OfficeUniversal && ViewInfo.Header.IsHeaderItemsInCaption)
				return dest;
			return RectangleHelper.GetCenterBounds(dest, size);
		}
		protected virtual bool IsCaptionVisible {
			get { return ContentBounds.Width >= minTextWidth; }
		}
		protected virtual Rectangle[] CaptionRects {
			get {
				Rectangle[] rects = new Rectangle[] { 
				ViewInfo.ApplicationButton.Bounds,
				GetFormIconArea(),
				GetCaptionToolbarArea(),
				GetCaptionButtonsArea(),
				GetCaptionCategoriesArea(),
				GetCaptionItemsArea()
				};
				return rects;
			}
		}
		protected virtual Rectangle GetCaptionItemsArea() {
			if(ViewInfo.GetRibbonStyle() == RibbonControlStyle.OfficeUniversal && ViewInfo.Header.IsHeaderItemsInCaption)
				return ViewInfo.Header.PageHeaderItemsBounds;
			return Rectangle.Empty;
		}
		protected internal virtual Rectangle GetCaptionButtonsArea() {
			if(!ViewInfo.Caption.FormButtonsBounds.IsEmpty) return ViewInfo.Caption.FormButtonsBounds;
			int width = ViewInfo.IsRightToLeft ? ContentBounds.X - Bounds.X : Bounds.Right - ContentBounds.Right;
			return new Rectangle(ViewInfo.IsRightToLeft ? Bounds.X : Bounds.Right - width, Bounds.Y, width, Bounds.Height);
		}
		protected virtual bool IsIntersectCaption(Rectangle captionTextRect, params Rectangle[] rects) {
			foreach(Rectangle rect in rects)
				if(captionTextRect.IntersectsWith(rect)) return true;	
			return false;
		}
		protected virtual Rectangle GetFormIconArea() {
			Rectangle iconArea;
			if(Ribbon.RibbonForm != null && Ribbon.RibbonForm.FormPainter !=null && !Ribbon.RibbonForm.FormPainter.GetIconBounds().IsEmpty){
				int formIconWidth = Ribbon.RibbonForm.FormPainter.GetIconBounds().Width;
				int formIconIndent = (Bounds.Height - formIconWidth) / 2;
				iconArea = new Rectangle(Bounds.X, Bounds.Y, formIconWidth + 2 * formIconIndent, Bounds.Height);
			}
			else 
				iconArea = new Rectangle(0, 0, (Bounds.Height - 16) / 2, Bounds.Height);
			return ViewInfo.IsRightToLeft ? BarUtilites.ConvertBoundsToRTL(iconArea, Bounds) : iconArea;
		}
		protected virtual Rectangle GetCaptionToolbarArea() {
			if(!ViewInfo.FormMinimized && !ViewInfo.Toolbar.Bounds.IsEmpty && ViewInfo.GetToolbarLocation() == RibbonQuickAccessToolbarLocation.Above)
				return ViewInfo.Toolbar.ContentBackgroundBounds;
			return Rectangle.Empty;
		}
		protected internal virtual Rectangle GetCaptionCategoriesArea() {
			if(!ViewInfo.ShouldDrawPageCategories()) return Rectangle.Empty;
			int width = 0;
			int x = ViewInfo.Header.PageCategories[0].Bounds.X;
			foreach(RibbonPageCategoryViewInfo category in ViewInfo.Header.PageCategories) {
				width += category.Bounds.Width;
				if(x > category.Bounds.X)
					x = category.Bounds.X;
			}
			return new Rectangle(x, Bounds.Y, width, Bounds.Height);
		}
		protected virtual Rectangle GetBestCaptionArea(Rectangle ownerRect, bool getFirstEmpty, params Rectangle[] rects) {
			int[][] buffer = BarUtilites.CreateZBuffer(ownerRect);
			foreach(Rectangle rect in rects) {
				if(rect.IsEmpty || !ownerRect.IntersectsWith(rect)) continue;
				for(int i = Math.Max(ownerRect.Left, rect.Left); i < Math.Min(ownerRect.Right, rect.Right); i++) { buffer[1][i]++; }
			}
			List<Rectangle> emptyAreas = BarUtilites.GetEmptyAreas(buffer, ownerRect.Width);
			if(emptyAreas.Count == 0) return ownerRect;
			Rectangle r = getFirstEmpty ? GetFirstEmptyArea(emptyAreas) : GetMaxEmptyArea(emptyAreas);
			return new Rectangle(r.X, ownerRect.Y, r.Width, ownerRect.Height);
		}
		protected virtual int MinEmptyAreaWidth { get { return 10; } }
		protected virtual Rectangle GetFirstEmptyArea(List<Rectangle> areas) {
			if(ViewInfo.IsRightToLeft)
				return areas.LastOrDefault(x => x.Width >= MinEmptyAreaWidth);
			return areas.FirstOrDefault(x => x.Width >= MinEmptyAreaWidth);
		}
		protected virtual Rectangle GetMaxEmptyArea(List<Rectangle> areas) { return areas.Where(x => x.Width == areas.Max(max => max.Width)).First(); }
		protected virtual Size CalcCaptionSize() {
			Size textSize = Size.Empty;
			string text = GetFormText();
			if(Form != null && Form.FormPainter != null && Form.FormPainter.AllowHtmlText) {
				StringInfo = StringPainter.Default.Calculate(GInfo.Graphics, ViewInfo.PaintAppearance.FormCaption, text, Bounds.Width);
				textSize = StringInfo.Bounds.Size;
			}
			else {
				textSize = ViewInfo.PaintAppearance.FormCaption.CalcTextSize(GInfo.Graphics, text, 0).ToSize();
			}
			textSize.Width += 1;
			if(IsGlassForm && ViewInfo.IsAllowDisplayRibbon) textSize.Width += 20;
			return textSize;
		}
		protected virtual Rectangle CalcContentBounds() {
			Rectangle res = ClientBounds;
			if(!ViewInfo.IsAllowDisplayRibbon) {
				if(Form != null && Form.IsGlassForm)
					return res;
				return ObjectPainter.GetObjectClientRectangle(ViewInfo.GInfo.Graphics, SkinElementPainter.Default, GetCaptionInfo());
			}
			if(Ribbon.GetRibbonStyle() != RibbonControlStyle.Office2007) {
				return res;
			}
			else if(!ViewInfo.ApplicationButtonSize.IsEmpty) {
				int width = ViewInfo.ApplicationButtonSize.Height + ViewInfo.ApplicationButtonLeftIndent + ViewInfo.ApplicationButtonRightIndent;
				if(!ViewInfo.IsRightToLeft)
					res.X += width;
				res.Width -= width;
				if(res.Width < 0) res.Width = 0;
			}
			return res;
		}
		protected virtual void CalcIcon() {
			if(FormPainter != null) {
				FormPainter.ResetIconBounds();
				if(!ViewInfo.IsAllowDisplayRibbon || Ribbon.GetRibbonStyle() != RibbonControlStyle.Office2007) ContentBounds = FormPainter.CalcIcon(ContentBounds);
			}
		}
		protected virtual void CalcButtons() {
			if(FormPainter != null) {
				ContentBounds = FormPainter.CalcButtons(ContentBounds);
			}
		}
		protected internal virtual void CalcToolbar() {
			if(!ViewInfo.IsAllowDisplayRibbon) return;
			if(ViewInfo.GetToolbarLocation() == RibbonQuickAccessToolbarLocation.Above && ViewInfo.GetRibbonStyle() != RibbonControlStyle.TabletOffice) {
				bool needDecrease = true;
				Rectangle rect = ContentBounds;
				if(Ribbon.ShowCategoryInCaption && ViewInfo.Header.PageCategories.Count > 0) {
					rect.Width = ViewInfo.Header.PageCategories[0].Bounds.X - rect.X;
					if(ContentBounds.Right - ViewInfo.Header.PageCategories[ViewInfo.Header.PageCategories.Count - 1].Bounds.Right > 50) needDecrease = false;
				}
				if(this.GetFormText() != string.Empty && needDecrease) rect.Width -= minTextWidth;
				ViewInfo.Toolbar.CalcViewInfo(CenterToolbarVertically(rect));
			}
		}
		protected Rectangle CenterToolbarVertically(Rectangle rect) {
			Rectangle toolbarRect = rect;
			toolbarRect.Height = ViewInfo.Toolbar.CalcMinHeight();
			toolbarRect.Y = rect.Y + (rect.Height - toolbarRect.Height) / 2;
			return toolbarRect;
		}
		protected internal bool IsGlassForm {
			get { return Form != null && Form.IsGlassForm; }
		}
		protected internal bool IsBorderedGlassForm {
			get { return IsGlassForm && Form.FormBorderStyle != FormBorderStyle.None; }
		}
		protected internal bool IsZoomedWindow {
			get {
				if(Form != null && Form.IsHandleCreated) {
					return NativeMethods.IsZoomed(Form.Handle);
				}
				return false;
			}
		}
		protected internal int CaptionYOffset {
			get {
				if(Form != null && Form.IsHandleCreated && IsZoomedWindow && Form.FormBorderStyle != FormBorderStyle.None) {
					if(NativeVista.IsVista) return 8;
					else return 2;
				}
				return 0;
			}
		}
		protected internal virtual bool AllowGlassCaption {
			get {
				object obj = RibbonSkins.GetSkin(ViewInfo.Provider).Properties[RibbonSkins.OptAllowGlassCaption];
				if(obj == null)
					return true;
				return (bool)obj;
			}
		}
		public virtual int CalcGlassFormTopMargin() {
			if(!AllowGlassCaption)
				return 0;
			int res = CalcCaptionHeight();
			if(RibbonForm.UseAdvancedDisplayRectangle) {
				if(Form != null && Form.WindowState == FormWindowState.Maximized)
					res += CaptionYOffset;
			}
			if(!Ribbon.IsOffice2010LikeStyle)
				return res;
			if(Ribbon.AllowGlassTabHeader && ViewInfo.IsAllowDisplayRibbon)
				res += ViewInfo.CalcTabHeaderHeight();
			return res; 
		}
		public virtual int CalcCaptionHeight() {
			int res = 0;
			GInfo.AddGraphics(null);
			try {
				if(Form != null && Form.IsMdiChild && Form.IsWindowMinimized) return Form.Height;
				res = Math.Max(ViewInfo.PaintAppearance.FormCaption.CalcDefaultTextSize(GInfo.Graphics).Height + 1, ViewInfo.Toolbar.CalcMinHeightCore(ViewInfo.Toolbar.GetToolbarInfoInCaption()));
				if(Form != null && Form.IsGlassForm) {
					res = SystemInformation.CaptionHeight;
				}
				else {
					res = SkinElementPainter.CalcBoundsByClientRectangle(GInfo.Graphics, SkinElementPainter.Default, GetCaptionInfo(), new Rectangle(0, 0, 10, res)).Height;
				}
				int appHeight = ViewInfo.ApplicationButtonSize.Height;
				if(appHeight > 0) res = Math.Max(appHeight / 2, res);
				if(IsBorderedGlassForm) res = Math.Max(res, SystemInformation.CaptionHeight + 8);
				if(!RibbonForm.UseAdvancedDisplayRectangle) {
					res += CaptionYOffset;
				}
			}
			finally {
				GInfo.ReleaseGraphics();
			}
			return res;
		}
		RibbonFormPainter formPainter;
		protected internal virtual RibbonFormPainter FormPainter {
			get {
				if(formPainter == null || formPainter.IsDestroyed) {
					if(Form != null) formPainter = Form.FormPainter;
				}
				return formPainter;
			}
		}
		protected internal virtual bool ProcessMessage(ref Message msg) {
			if(Bounds.IsEmpty || FormPainter == null) return false;
			switch(msg.Msg) {
				case WM_NCHITTEST :
					return WMNCHitTest(ref msg);
			}
			return false;
		}
		protected bool RectangleContainsX(Rectangle rect, Point pt) {
			return rect.X < pt.X && rect.Right > pt.X;
		}
		protected bool ShouldCheckByX(Rectangle rect, Point pt) { 
			return ViewInfo.FormMaximized && ViewInfo.GetToolbarLocation() == RibbonQuickAccessToolbarLocation.Above && pt.Y < rect.Y;
		}
		protected virtual bool ToolbarBoundsContains(Point pt) {
			if(ViewInfo.Toolbar.VisibleButtonCount == 0)
				return false;
			if(!ShouldCheckByX(ViewInfo.ToolbarBounds, pt))
				return ViewInfo.ToolbarBounds.Contains(pt);
			return RectangleContainsX(ViewInfo.ToolbarBounds, pt);
		}
		protected virtual bool ButtonsBoundsContains(Point pt) {
			if(!ShouldCheckByX(FormPainter.Buttons.ButtonsBounds, pt)) return FormPainter.Buttons.ButtonsBounds.Contains(pt);
			return RectangleContainsX(FormPainter.Buttons.ButtonsBounds, pt);
		}
		protected internal virtual bool CanGetFormButtonsBounds { get { return FormPainter != null; } }
		protected internal virtual Rectangle FormButtonsBounds {
			get {
				if(!CanGetFormButtonsBounds)
					return Rectangle.Empty;
				return FormPainter.Buttons.ButtonsBounds;
			}
		}
		protected bool WMNCHitTest(ref Message msg) {
			Point screen = FormPainter.PointToFormBounds(msg.LParam);
			Point p = Ribbon.PointToClient(WinAPIHelper.GetPoint(msg.LParam));
			if(Ribbon.GetRibbonStyle() != RibbonControlStyle.Office2007 && !IsInDesignRect(p)) {
				RibbonHitInfo hi = ViewInfo.CalcHitInfo(p);
				if(Ribbon.GetRibbonStyle() == RibbonControlStyle.Office2013 && hi.HitTest == RibbonHitTest.FullScreenModeBar) {
					msg.Result = new IntPtr(HT.HTTRANSPARENT);
					return true;
				}
				if(hi.HitTest == RibbonHitTest.HeaderPanel) {
					msg.Result = new IntPtr(HT.HTTRANSPARENT);
					return true;
				}
			}
			int res = HT.HTCLIENT;
			Rectangle client = Bounds;
			if(!client.Contains(p)) return false;
			res = FormPainter.WMNCHitTestResize(screen, HT.HTCLIENT);
			if(res != HT.HTCLIENT) {
				msg.Result = new IntPtr(HT.HTTRANSPARENT);
				return true;
			}
			if(FormPainter.IconBoundsCore.Contains(p)) {
				msg.Result = new IntPtr(HT.HTTRANSPARENT);
				return true;
			}
			if(ViewInfo.ApplicationButtonHitBounds.Contains(p)) return false;
			if(!Ribbon.IsMerged) {
				if(ToolbarBoundsContains(p)) return false;
				if(ViewInfo.GetRibbonStyle() == RibbonControlStyle.OfficeUniversal) {
					if(ViewInfo.Header.PageHeaderBounds.Contains(p) || 
						ViewInfo.Header.PageHeaderItemsBounds.Contains(p))
					return false;
				}
				if(PageCategoriesContains(p)) {
					if(Form != null && Form.ShouldDraggingByPageCategory()) {
						msg.Result = new IntPtr(HT.HTTRANSPARENT);
						return true;
					}
					return false;
				}
				if(ViewInfo.Toolbar.ContentBounds.Contains(p)) return false;
			}
			if(ButtonsBoundsContains(screen)) return false;
			msg.Result = new IntPtr(HT.HTTRANSPARENT);
			return true;
		}
		protected bool IsInDesignRect(Point p) {
			return Ribbon.IsDesignMode && ViewInfo.Header != null && ViewInfo.Header.DesignerRect.Contains(p);
		}
		protected internal virtual bool PageCategoriesContains(Point p) {
			if(ViewInfo.Header == null || ViewInfo.Header.PageCategories == null) return false;
			if(Ribbon != null && !Ribbon.ShowCategoryInCaption) return false;
			foreach(RibbonPageCategoryViewInfo category in ViewInfo.Header.PageCategories) {
				if(category.UpperBounds.Contains(p)) return true;
			}
			return false;
		}
		internal bool OnMouseMove(DXMouseEventArgs e) {
			if(Bounds.IsEmpty || FormPainter == null) return false;
			return FormPainter.OnMouseMove(e);
		}
		internal bool OnMouseDown(DXMouseEventArgs e) {
			if(Bounds.IsEmpty || FormPainter == null) return false;
			return FormPainter.OnMouseDown(e);
		}
		internal bool OnMouseUp(DXMouseEventArgs e) {
			if(Bounds.IsEmpty || FormPainter == null) return false;
			return FormPainter.OnMouseUp(e);
		}
		internal void OnMouseLeave(DXMouseEventArgs e) {
			if(Bounds.IsEmpty || FormPainter == null) return;
			FormPainter.OnMouseLeave();
		}
		internal const int WM_NCHITTEST = 0x0084, WM_SYSCOMMAND = 0x0112, SC_SIZE = 0xF000, SC_SIZE2 = 0xF008;
		internal struct HT {
			public const int HTERROR = -2;
			public const int HTTRANSPARENT = -1;
			public const int HTNOWHERE = 0, HTCLIENT = 1, HTCAPTION = 2, HTSYSMENU = 3,
				HTGROWBOX = 4, HTSIZE = HTGROWBOX, HTMENU = 5, HTHSCROLL = 6, HTVSCROLL = 7, HTMINBUTTON = 8, HTMAXBUTTON = 9,
				HTLEFT = 10, HTRIGHT = 11, HTTOP = 12, HTTOPLEFT = 13, HTTOPRIGHT = 14, HTBOTTOM = 15, HTBOTTOMLEFT = 16,
				HTBOTTOMRIGHT = 17, HTBORDER = 18, HTREDUCE = HTMINBUTTON, HTZOOM = HTMAXBUTTON, HTSIZEFIRST = HTLEFT,
				HTSIZELAST = HTBOTTOMRIGHT, HTOBJECT = 19, HTCLOSE = 20, HTHELP = 21;
		}
		protected internal virtual void CalcHitInfo(RibbonHitInfo res) {
			if(ViewInfo.GetRibbonStyle() == RibbonControlStyle.OfficeUniversal && ViewInfo.Header.IsHeaderItemsInCaption) {
				ViewInfo.Header.CalcHitInfo(res);
				return;
			}
			if(Form == null) return;
			foreach(FormCaptionButton button in FormPainter.Buttons) {
				if(button.Bounds.Contains(res.HitPoint)) {
					res.SetHitTest(FormCaptionButtonKind2HitTest(button.Kind));
					return;
				}
			}
		}
		protected RibbonHitTest FormCaptionButtonKind2HitTest(FormCaptionButtonKind kind) {
			switch(kind) { 
				case FormCaptionButtonKind.Minimize:
					return RibbonHitTest.FormMinimizeButton;
				case FormCaptionButtonKind.FullScreen:
					return RibbonHitTest.FormFullScreenButton;
				case FormCaptionButtonKind.Maximize:
					return RibbonHitTest.FormMaximizeButton;
				case FormCaptionButtonKind.Close:
					return RibbonHitTest.FormCloseButton;
				case FormCaptionButtonKind.Help:
					return RibbonHitTest.FormHelpButton;
			}
			return RibbonHitTest.FormCaption;
		}
	}
}
