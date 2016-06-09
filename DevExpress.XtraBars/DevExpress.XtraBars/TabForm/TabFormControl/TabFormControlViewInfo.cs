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

using DevExpress.Skins;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.Utils.Drawing.Animation;
using DevExpress.XtraBars.Controls;
using DevExpress.XtraBars.ViewInfo;
using DevExpress.XtraEditors;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
namespace DevExpress.XtraBars {
	public class TabFormControlViewInfo : TabFormControlViewInfoBase {
		public TabFormControlViewInfo(BarManager manager, BarDrawParameters param, CustomControl owner)
			: base(manager, param, owner) {
		}
		public new TabFormControl Owner { get { return (TabFormControl)base.Owner; } }
		protected override Rectangle CalcClientBounds() {
			if(Owner.GetFormPainter() != null && Owner.TabForm.IsMaximized) {
				return new Rectangle(Bounds.X, Bounds.Y + ZoomedInvisibleHeight, CalcClientWidth(), Bounds.Height - ZoomedInvisibleHeight);
			}
			return base.CalcClientBounds();
		}
		public override int GetCaptionHeight() {
			if(Owner.TabForm != null) {
				return PagesBottom;
			}
			return base.GetCaptionHeight();
		}
		public override int GetTopPanelHeight() {
			if(Owner.ShowTabsInTitleBar == ShowTabsInTitleBar.True) return 0;
			if(Owner.GetFormPainter() != null) {
				return Owner.GetFormPainter().GetXtraFormCaptionHeight();
			}
			return base.GetTopPanelHeight();
		}
		protected internal override int GetPageTop() {
			if(Owner.GetFormPainter() == null)
				return base.GetPageTop();
			int pageTop;
			if(Owner.ShowTabsInTitleBar == ShowTabsInTitleBar.True)
				pageTop = Owner.TitleTabVerticalOffset;
			else pageTop = GetCaptionToPageDistance() + Owner.GetFormPainter().GetXtraFormCaptionHeight();
			if(Owner.TabForm != null && Owner.TabForm.IsMaximized)
				return pageTop + ZoomedInvisibleHeight;
			return pageTop;
		}
		int GetCaptionToPageDistance() {
			SkinElement elem = GetPageSkinElement();
			if(elem == null) return 0;
			return GetPageSkinElement().Properties.GetInteger("PaddingTop", 0);
		}
		public override TabFormControlHitInfo CalcHitInfo(Point p) {
			TabFormControlHitInfo hitInfo = base.CalcHitInfo(p);
			if(hitInfo.HitTest == TabFormControlHitTest.None && Owner.TabForm != null) {
				if(Owner.TabForm.GetCaptionClientBounds().Contains(p)) {
					hitInfo.HitTest = TabFormControlHitTest.Caption;
					return hitInfo;
				}
			}
			return hitInfo;
		}
		int ZoomedInvisibleHeight {
			get {
				if(Owner.GetFormPainter() == null)
					return 0;
				return Owner.GetFormPainter().ZoomedInvisibleHeight;
			}
		}
		protected internal override bool IsRightToLeft {
			get { return base.IsRightToLeft && WindowsFormsSettings.RightToLeftLayout == DefaultBoolean.True; }
		}
	}
	public class TabFormControlViewInfoBase : CustomControlViewInfo, ISupportXtraAnimation {
		TabFormControlBase owner;
		TabFormLinkInfoProvider linkInfoProvider;
		TabFormLinkCalculator linkCalculator;
		TabFormPageCalculator pageCalculator;
		TabFormControlDefaultAppearances defaultAppearances;
		List<TabFormPageViewInfo> pageInfos;
		TabFormPageViewInfo addPageInfo;
		TabFormPage addPage;
		Rectangle clientRect;
		public TabFormControlViewInfoBase(BarManager manager, BarDrawParameters param, CustomControl owner)
			: base(manager, param, owner) {
				this.owner = (TabFormControlBase)owner;
				this.linkInfoProvider = new TabFormLinkInfoProvider();
				this.linkCalculator = new TabFormLinkCalculator(this);
				this.pageCalculator = new TabFormPageCalculator(this);
				this.defaultAppearances = new TabFormControlDefaultAppearances(this);
				this.pageInfos = new List<TabFormPageViewInfo>();
				this.addPage = CreateAddPage();
				this.addPageInfo = new TabFormPageViewInfo(AddPage);
				this.clientRect = Rectangle.Empty;
		}
		public TabFormControlBase Owner { get { return owner; } }
		public Rectangle ClientRect { get { return clientRect; } }
		public TabFormLinkCalculator LinkCalculator { get { return linkCalculator; } }
		public TabFormPageCalculator PageCalculator { get { return pageCalculator; } }
		public List<TabFormPageViewInfo> PageInfos { get { return pageInfos; } }
		public TabFormPageViewInfo AddPageInfo { get { return addPageInfo; } }
		static readonly Size DefaultImageSize = new Size(16, 16);
		internal Size ImageSize { get { return DefaultImageSize; } }
		protected virtual void CreatePageInfos() {
			if(!this.shouldCreatePageInfos)
				return;
			this.pageInfos = new List<TabFormPageViewInfo>();
			foreach(TabFormPage page in Owner.Pages) {
				if(page.GetVisible()) {
					this.pageInfos.Add(new TabFormPageViewInfo(page));
				}
			}
			this.addPageInfo = new TabFormPageViewInfo(AddPage);
			this.shouldCreatePageInfos = false;
		}
		protected internal bool ShouldShowAddPage() {
			if(Owner.IsDesignMode) return true;
			return Owner.ShowAddPageButton;
		}
		bool shouldCreatePageInfos = true;
		public void ClearPageInfos() {
			this.shouldCreatePageInfos = true;
			this.pageInfos = null;
			this.addPageInfo = null;
			this.closeButton = null;
		}
		Image closeButton;
		internal Image CloseButton {
			get {
				if(closeButton == null)
					closeButton = LoadCloseButton("TabForm.CloseButton.png");
				return closeButton;
			}
		}
		internal Size GetCloseButtonSize() {
			return new Size(CloseButton.Width, CloseButton.Height / 4);
		}
		Image LoadCloseButton(string name) {
			Image res = ResourceImageHelper.CreateImageFromResources("DevExpress.XtraBars." + name, typeof(TabFormControlViewInfoBase).Assembly);
			Bitmap bmp = res as Bitmap;
			if(bmp != null) bmp.MakeTransparent(Color.Magenta);
			return res;
		}
		protected internal Point GetCloseButtonOffset() {
			SkinElement elem = GetPageSkinElement();
			if(string.Equals(elem.ElementName, TabSkins.SkinTabHeader))
				return new Point(10, 0);
			return new Point(elem.Properties.GetInteger("CloseButtonLeftOffset", 0), elem.Properties.GetInteger("CloseButtonTopOffset", 0));
		}
		protected internal Color GetForeColorByState(ObjectState state) {
			Color defColor;
			if(!HasFormSkinElement) {
				Skin skin = TabSkins.GetSkin(Owner.LookAndFeel.ActiveLookAndFeel);
				defColor = skin.Colors.GetColor("TabHeaderTextColor");
				if(state == ObjectState.Hot) return skin.Colors.GetColor("TabHeaderTextColorHot", defColor);
				if(state == ObjectState.Pressed) return skin.Colors.GetColor("TabHeaderTextColorActive", defColor);
				if(state == ObjectState.Disabled) return skin.Colors.GetColor("TabHeaderTextColorDisabled", defColor);
				return defColor;
			}
			SkinElement elem = GetPageSkinElement();
			defColor = elem.Color.GetForeColor();
			if(state == ObjectState.Hot) return elem.Properties.GetColor("ForeColorHot", defColor);
			if(state == ObjectState.Pressed) return elem.Properties.GetColor("ForeColorPressed", defColor);
			if(state == ObjectState.Disabled) return elem.Properties.GetColor("ForeColorDisabled", defColor);
			return defColor;
		}
		public TabFormPageViewInfo GetPageInfo(TabFormPage page) {
			foreach(TabFormPageViewInfo pageInfo in PageInfos) {
				if(pageInfo.Page.Equals(page)) return pageInfo;
			}
			return null;
		}
		TabFormPage CreateAddPage() {
			TabFormPage page = new TabFormPage();
			page.Text = "+";
			return page;
		}
		public override void CalcViewInfo(Graphics g, object sourceObject, Rectangle r) {
			base.CalcViewInfo(g, sourceObject, r);
			GInfo.AddGraphics(null);
			try {
				this.fBounds = new Rectangle(Point.Empty, Owner.Size);
				this.clientRect = CalcClientBounds();
				UpdateLinkInfoProvider();
				CalcLinks();
				CreatePageInfos();
				PageCalculator.Calculate();
				CheckRects();
			}
			finally { GInfo.ReleaseGraphics(); }
		}
		static BarLinkParts[] barLinkPartsValues = (BarLinkParts[])Enum.GetValues(typeof(BarLinkParts));
		void CheckRects() {
			if(!IsRightToLeft) return;
			LinkInfoProvider.ForEachLinkInfo(link => {
				foreach(BarLinkParts part in barLinkPartsValues) {
					link.Rects[part] = ReverseRect(link.Rects[part]);
				}
				BarEditLinkViewInfo editor = link as BarEditLinkViewInfo;
				if(editor != null) {
					editor.EditRectangle = ReverseRect(editor.EditRectangle);
					editor.UpdateEditBoundsCore();
				}
			});
			foreach(TabFormPageViewInfo page in PageInfos) {
				page.Bounds = ReverseRect(page.Bounds);
				page.ReverseRects();
			}
			AddPageInfo.Bounds = ReverseRect(AddPageInfo.Bounds);
			AddPageInfo.ReverseRects();
		}
		Rectangle ReverseRect(Rectangle rect) {
			Rectangle res = rect;
			res.X = Bounds.Right - rect.Right;
			return res;
		}
		public virtual void CalcLinks() {
			if(Owner.ShowTabsInTitleBar == ShowTabsInTitleBar.False)
				LinkCalculator.Calculate(LinkInfoProvider.TitleItemLinks);
			LinkCalculator.Calculate(LinkInfoProvider.TabLeftItemLinks);
			LinkCalculator.Calculate(LinkInfoProvider.TabRightItemLinks);
		}
		protected virtual Rectangle CalcClientBounds() {
			if(Owner.ShowTabsInTitleBar == ShowTabsInTitleBar.True)
				return new Rectangle(Bounds.X, Bounds.Y, CalcClientWidth(), Bounds.Height);
			return Bounds;
		}
		protected int CalcClientWidth() {
			if(Owner.ShowTabsInTitleBar == ShowTabsInTitleBar.True)
				return linkCalculator.GetTopPanelRight() - LinkCalculator.GetTopPanelLeft() - 50;
			return Bounds.Width;
		}
		internal TabFormPage AddPage { get { return addPage; } }
		public SkinElement GetPageSkinElement() {
			SkinElement elem = FormSkins.GetSkin(Owner.LookAndFeel.ActiveLookAndFeel)[FormSkins.SkinTabFormPage];
			if(elem != null) return elem;
			return TabSkins.GetSkin(Owner.LookAndFeel.ActiveLookAndFeel)[TabSkins.SkinTabHeader];
		}
		public SkinElement GetAddPageSkinElement() {
			SkinElement elem = FormSkins.GetSkin(Owner.LookAndFeel.ActiveLookAndFeel)[FormSkins.SkinTabFormAddPage];
			if(elem != null) return elem;
			elem = EditorsSkins.GetSkin(Owner.LookAndFeel.ActiveLookAndFeel)[EditorsSkins.SkinNavigatorButton];
			if(elem != null) return elem;
			return GetPageSkinElement();
		}
		protected void UpdateLinkInfoProvider() {
			LinkInfoProvider.Reset();
			FillLinkInfoProvider();
		}
		protected virtual void FillLinkInfoProvider() {
			FillLinkInfoProvider(Owner.LinkProvider.TitleItemLinks, LinkInfoProvider.TitleItemLinks);
			FillLinkInfoProvider(Owner.LinkProvider.TabLeftItemLinks, LinkInfoProvider.TabLeftItemLinks);
			FillLinkInfoProvider(Owner.LinkProvider.TabRightItemLinks, LinkInfoProvider.TabRightItemLinks);
		}
		protected void FillLinkInfoProvider(TabFormLinkCollection links, List<BarLinkViewInfo> infos) {
			for(int i = 0; i < links.Count; i++) {
				if(links[i].Visible)
					CreateLinkViewInfo(links[i], infos);
			}
			if(Owner.IsDesignMode){
				CreateAddLinkInfo(links, infos);
			}
		}
		public new BarManager Manager { get { return Owner.Manager; } }
		void CreateAddLinkInfo(TabFormLinkCollection links, List<BarLinkViewInfo> infos) {
			BarItemLink addLink = Manager.InternalItems.DesignTimeItem.CreateLink(null, links);
			CreateLinkViewInfo(addLink, infos);
		}
		protected void CreateLinkViewInfo(BarItemLink link, List<BarLinkViewInfo> infos) {
			link.SetLinkViewInfo(link.CreateViewInfo());
			infos.Add(link.LinkViewInfo);
		}
		public TabFormLinkInfoProvider LinkInfoProvider {
			get { return linkInfoProvider; }
		}
		public bool CanAnimate {
			get { return true; }
		}
		public System.Windows.Forms.Control OwnerControl {
			get { return this.owner; }
		}
		public virtual TabFormControlHitInfo CalcHitInfo(Point p) {
			TabFormControlHitInfo hitInfo = new TabFormControlHitInfo(p);
			BarItemLink link = LinkInfoProvider.GetItemLinkByPoint(p);
			if(link != null) {
				hitInfo.HitTest = TabFormControlHitTest.Link;
				return hitInfo;
			}
			foreach(TabFormPageViewInfo pageInfo in PageInfos) {
				if(!pageInfo.Page.GetEnabled()) continue;
				if(pageInfo.Bounds.Contains(p)) {
					hitInfo.HitTest = TabFormControlHitTest.Page;
					return hitInfo;
				}
			}
			if(AddPageInfo != null && AddPageInfo.Bounds.Contains(p)) {
				hitInfo.HitTest = TabFormControlHitTest.AddPage;
				return hitInfo;
			}
			return hitInfo;
		}
		public virtual int GetCaptionHeight() {
			return 0;
		}
		public virtual int GetTopPanelHeight() {
			return 27;
		}
		public virtual int GetDistanceBetweenTabs() {
			SkinElement elem = GetPageSkinElement();
			if(elem == null)
				return 0;
			return elem.Properties.GetInteger("DistanceBetweenPages", 0);
		}
		public virtual int GetDistanceToAddTab() {
			SkinElement elem = GetAddPageSkinElement();
			if(elem == null)
				return 0;
			return elem.Properties.GetInteger("DistanceBetweenPages", 0);
		}
		public virtual int GetImageToTextIndent() {
			return 4;
		}
		static ObjectState[] objectStateValues = (ObjectState[])Enum.GetValues(typeof(ObjectState));
		protected internal int CalcBestPageHeight() {
			SkinElement elem = GetPageSkinElement();
			if(elem == null) return 0;
			int textHeight = 0;
			foreach(ObjectState state in objectStateValues) {
				textHeight = Math.Max(CreatePagePaintAppearance(state).CalcDefaultTextSize().Height, textHeight);
			}
			int maxContentHeight = textHeight + GetPageMargins(elem).Height;
			elem = GetAddPageSkinElement();
			if(elem == null) return maxContentHeight;
			int addPageHeight = CalcAddPageHeight(elem);
			return Math.Max(maxContentHeight, addPageHeight);
		}
		protected internal SkinPaddingEdges GetPageMargins(SkinElement elem) {
			if(elem == null) return new SkinPaddingEdges();
			if(string.Equals(elem.ElementName, TabSkins.SkinTabHeader)) {
				SkinPaddingEdges margins = elem.ContentMargins.Clone() as SkinPaddingEdges;
				margins.Bottom += TabSkins.GetSkin(Owner.LookAndFeel.ActiveLookAndFeel).Properties.GetInteger("SelectedHeaderDownGrow", 0);
				return margins;
			}
			return elem.ContentMargins;
		}
		protected internal bool HasFormSkinElement {
			get { return FormSkins.GetSkin(Owner.LookAndFeel.ActiveLookAndFeel)[FormSkins.SkinTabFormPage] != null; }
		}
		protected internal virtual int CalcAddPageHeight(SkinElement elem) {
			int height = CalcAddPageContentSize(elem).Height + elem.ContentMargins.Height;
			int addPageHeight = Math.Max(height, elem.Size.MinSize.Height);
			return addPageHeight;
		}
		protected internal virtual int CalcAddPageWidth() {
			SkinElement elem = GetAddPageSkinElement();
			Size size = CalcAddPageContentSize(elem);
			return Math.Max(size.Width + elem.ContentMargins.Width, elem.Size.MinSize.Width);
		}
		protected internal Size CalcAddPageContentSize(SkinElement elem) {
			if(elem == null) return Size.Empty;
			if(string.Equals(elem.ElementName, EditorsSkins.SkinNavigatorButton)) {
				return EditorsSkins.GetSkin(Owner.LookAndFeel.ActiveLookAndFeel)[EditorsSkins.SkinNavigator].Image.GetImages().ImageSize;
			}
			if(elem.Glyph == null || elem.Glyph.Image == null)
				return Size.Empty;
			return elem.Glyph.GetImages().ImageSize;
		}
		public virtual int CalcControlBestHeight() {
			return PagesBottom;
		}
		protected internal int PagesBottom {
			get { return GetPageTop() + CalcBestPageHeight(); }
		}
		protected internal virtual int GetPageTop() {
			return GetTopPanelHeight();
		}
		public virtual void UpdatePagePaintAppearance(TabFormPageViewInfo pageInfo) {
			ObjectState state = GetPageState(pageInfo);
			pageInfo.PaintAppearance = CreatePagePaintAppearance(state);
		}
		protected internal AppearanceObject CreatePagePaintAppearance(ObjectState state) {
			AppearanceObject pageApp = GetPageAppearance(state);
			AppearanceDefault defPageApp = DefaultAppearances.GetAppearance(state);
			AppearanceObject obj = new AppearanceObject(pageApp, defPageApp);
			if(!pageApp.Options.UseFont) obj.FontStyleDelta = defPageApp.FontStyleDelta;
			if(obj.TextOptions.Trimming == Trimming.Default) obj.TextOptions.Trimming = Trimming.EllipsisCharacter;
			obj.TextOptions.RightToLeft = IsRightToLeft;
			return obj;
		}
		protected ObjectState GetPageState(TabFormPageViewInfo pageInfo) {
			if(!pageInfo.Page.GetEnabled()) return ObjectState.Disabled;
			if(Owner.SelectedPage == pageInfo.Page) return ObjectState.Pressed;
			if(Owner.Handler.HotPage == pageInfo) return ObjectState.Hot;
			return ObjectState.Normal;
		}
		internal TabFormControlDefaultAppearances DefaultAppearances { get { return defaultAppearances; } }
		protected AppearanceObject GetPageAppearance(ObjectState state) {
			if(state == ObjectState.Disabled) {
				return Owner.Appearance.Page.Disabled;
			}
			if(state == ObjectState.Hot) {
				return Owner.Appearance.Page.Hovered;
			}
			if(state == ObjectState.Pressed) {
				return Owner.Appearance.Page.Pressed;
			}
			return Owner.Appearance.Page.Normal;
		}
		protected override void UpdateAppearance() {
			StateAppearances skinAppearance = Manager.GetController().PaintStyle.DrawParameters.Colors.StateAppearance(BarAppearance.Bar);
			Appearance.Combine(new StateAppearances[] { Manager.GetController().AppearancesBar.MainMenuAppearance, skinAppearance });
			Appearance.UpdateRightToLeft(IsRightToLeft);
		}
		protected internal override bool IsRightToLeft {
			get { return Owner.IsRightToLeft; }
		}
	}
	public enum TabFormControlHitTest { None, Link, Page, AddPage, Caption }
	public class TabFormControlHitInfo {
		Point hitPoint;
		TabFormControlHitTest hitTest;
		public TabFormControlHitInfo(Point pt) {
			this.hitPoint = pt;
			this.hitTest = TabFormControlHitTest.None;
		}
		public Point HitPoint { get { return hitPoint; } }
		public TabFormControlHitTest HitTest { get { return hitTest; } set { hitTest = value; } }
	}
}
