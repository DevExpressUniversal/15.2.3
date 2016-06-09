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
using DevExpress.Utils.DragDrop;
using DevExpress.XtraBars.Ribbon;
using System.Drawing;
using DevExpress.Utils.Drawing;
using DevExpress.LookAndFeel;
using DevExpress.Skins;
using DevExpress.Utils.Win;
using System.Windows.Forms;
using DevExpress.Utils;
using DevExpress.XtraTab;
using DevExpress.XtraTab.Drawing;
using DevExpress.XtraTab.ViewInfo;
using System.Collections.Generic;
using DevExpress.Utils.Mdi;
namespace DevExpress.XtraTabbedMdi {
	public delegate void CustomDocumentSelectorItemEventHandler(object sender, CustomDocumentSelectorItemEventArgs e);
	public class CustomDocumentSelectorItemEventArgs : EventArgs {
		DocumentSelectorItem itemCore;
		public CustomDocumentSelectorItemEventArgs(DocumentSelectorItem item) {
			itemCore = item;
		}
		public DocumentSelectorItem Item {
			get { return itemCore; }
		}
	}
	public delegate void CustomDocumentSelectorSettingsEventHandler(object sender, CustomDocumentSelectorSettingsEventArgs e);
	public class CustomDocumentSelectorSettingsEventArgs : EventArgs {
		DocumentSelector selectorCore;
		public CustomDocumentSelectorSettingsEventArgs(DocumentSelector selector) {
			selectorCore = selector;
		}
		public DocumentSelector Selector {
			get { return selectorCore; }
		}
	}
	public class DocumentSelector : TopFormBase {
		XtraTabbedMdiManager managerCore;
		GalleryControl galleryCore;
		DocumentSelectorViewInfo viewInfoCore;
		ObjectPainter BackgroundPainter;
		public XtraTabbedMdiManager Manager {
			get { return managerCore; }
		}
		protected internal GalleryControl Gallery {
			get { return galleryCore; }
		}
		protected internal GalleryItemGroup MDIChildrenGroup {
			get { return mdiChildrenGroup; }
		}
		protected internal ISkinProvider CurrentSkinProvider {
			get { return Manager.ViewInfo.TabControl.LookAndFeel; }
		}
		protected DocumentSelectorViewInfo ViewInfo {
			get { return viewInfoCore; }
		}
		XtraMdiTabPage startPageCore;
		public XtraMdiTabPage StartPage {
			get { return startPageCore; }
		}
		XtraMdiTabPage nextPageCore;
		public XtraMdiTabPage NextPage {
			get { return nextPageCore; }
			private set {
				if(nextPageCore == value) return;
				nextPageCore = value;
				OnNextPageChanged();
			}
		}
		public DocumentSelector(XtraTabbedMdiManager manager, XtraMdiTabPage startPage, XtraMdiTabPage nextPage) {
			DoubleBuffered = true;
			ShowPreview = true;
			KeyPreview = true;
			startPageCore = startPage;
			nextPageCore = nextPage;
			managerCore = manager;
			viewInfoCore = CreateViewInfo();
			galleryCore = CreateGallery();
			UpdatePainters();
			Gallery.BackColor = Color.Transparent;
			Gallery.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			Gallery.Size = ClientSize;
			Gallery.Location = new Point(-ClientSize.Width, -ClientSize.Height);
			Gallery.Parent = this;
			Initialize();
			OnNextPageChanged();
		}
		void Initialize() {
			GalleryMaxRows = 12;
			UpdateGallery(Manager.Pages);
			ShowHeader = true;
			HeaderHeight = 24;
			FooterHeight = 24;
			ShowFooter = CalcShowFooter();
			GalleryColumnWidth = CalcBestColumnWidth();
			Manager.RaiseDocumentSelectorSettings(this);
			Size = CalcSize(Manager.MdiParent.Size, Math.Min(Manager.Pages.Count, GalleryMaxRows), Gallery.Gallery.ColumnCount);
			Location = CalcLocation(Manager.MdiParent.Location, Manager.MdiParent.Size);
		}
		bool CalcShowFooter() {
			bool result = false;
			foreach(DocumentSelectorItem item in MDIChildrenGroup.Items) {
				if(!string.IsNullOrEmpty(item.FooterText))
					return true;
			}
			return result;
		}
		int CalcBestColumnWidth() {
			Gallery.Gallery.UpdateGallery();
			return Gallery.Gallery.ViewInfo.GetItemMaxSize().Width + 1;
		}
		Size CalcSize(Size mdiParentSize, int rowCount, int columnCount) {
			int galleryHeight = Gallery.Gallery.ViewInfo.CalcGalleryBestSize(rowCount, columnCount).Height + 3;
			return ViewInfo.CalcMinSize(galleryHeight);
		}
		Point CalcLocation(Point mdiParentLocation, Size mdiParentSize) {
			return new Point(
					Round(mdiParentLocation.X + (mdiParentSize.Width - Size.Width) * 0.5f),
					Round(mdiParentLocation.Y + (mdiParentSize.Height - Size.Height) * 0.5f)
				);
		}
		static int Round(float value) {
			return value > 0 ? (int)(value + 0.5f) : (int)(value - 0.5f);
		}
		protected override void Dispose(bool disposing) {
			if(disposing) {
				if(Gallery != null) {
					Gallery.Dispose();
					galleryCore = null;
				}
				if(ViewInfo != null)
					ViewInfo.Dispose();
			}
			base.Dispose(disposing);
		}
		protected override void OnLoad(EventArgs e) {
			base.OnLoad(e);
			SubscribeGalleryMouse();
			Gallery.Gallery.ItemClick += Gallery_ItemClick;
		}
		protected override void OnShown(EventArgs e) {
			base.OnShown(e);
			Manager.DoFocusWindow(Gallery);
			Gallery.KeyboardSelectedItem = SelectedItem;
		}
		protected override void OnClosing(System.ComponentModel.CancelEventArgs e) {
			base.OnClosing(e);
			Gallery.Gallery.ItemClick -= Gallery_ItemClick;
			UnSubscribeGalleryMouse();
		}
		int tabCounter = 0;
		protected override void OnKeyUp(KeyEventArgs e) {
			base.OnKeyUp(e);
			if(e.Control) {
				switch(e.KeyCode) {
					case Keys.Tab:
						tabCounter++;
						if(StartPage != null)
							SelectStart();
						else SelectNext(!e.Shift);
						break;
					case Keys.Escape:
						DialogResult = DialogResult.Cancel;
						break;
				}
			}
			else {
				if(e.KeyCode == Keys.ControlKey) {
					CheckNextPageBeforeClosing(e);
					DialogResult = DialogResult.OK;
				}
			}
		}
		void CheckNextPageBeforeClosing(KeyEventArgs e) {
			if(Gallery.KeyboardSelectedItem != null) {
				nextPageCore = ((DocumentSelectorItem)Gallery.KeyboardSelectedItem).Page;
			}
			else {
				if(tabCounter == 0) {
					if(StartPage != null)
						nextPageCore = StartPage;
					else {
						int index = mdiChildrenGroup.Items.IndexOf(SelectedItem);
						if(index != -1) {
							nextPageCore = GetNextItem(!e.Shift, index).Page;
						}
					}
				}
			}
		}
		void SubscribeGalleryMouse() {
			Gallery.Capture = true;
			Gallery.MouseDown += GalleryMouseDown;
		}
		void UnSubscribeGalleryMouse() {
			Gallery.MouseDown -= GalleryMouseDown;
			Gallery.Capture = false;
		}
		void GalleryMouseDown(object sender, MouseEventArgs e) {
			if(!Gallery.Bounds.Contains(e.Location))
				DialogResult = DialogResult.Cancel;
		}
		void Gallery_ItemClick(object sender, GalleryItemClickEventArgs e) {
			NextPage = ((DocumentSelectorItem)e.Item).Page;
			DialogResult = DialogResult.OK;
		}
		protected void SelectNext(bool forward) {
			int index = mdiChildrenGroup.Items.IndexOf(SelectedItem);
			if(index != -1) {
				SelectedItem = GetNextItem(forward, index);
				Gallery.KeyboardSelectedItem = SelectedItem;
				NextPage = SelectedItem.Page;
				SelectedItem.Checked = true;
			}
		}
		protected void SelectStart() {
			DocumentSelectorItem item = GetItem(StartPage);
			if(item != null) {
				nextPageCore = StartPage;
				SelectedItem = item;
				Gallery.KeyboardSelectedItem = item;
				item.Checked = true;
				startPageCore = null;
			}
		}
		DocumentSelectorItem GetNextItem(bool forward, int index) {
			int nextIndex = (index + (forward ? 1 : -1)) % mdiChildrenGroup.Items.Count;
			if(nextIndex < 0)
				nextIndex = mdiChildrenGroup.Items.Count + nextIndex;
			return mdiChildrenGroup.Items[nextIndex] as DocumentSelectorItem;
		}
		GalleryItemGroup mdiChildrenGroup;
		DocumentSelectorItem selectedItemCore;
		#region settings
		public int GalleryColumnWidth { get; set; }
		public int GalleryMaxRows { get; set; }
		public bool ShowHeader { get; set; }
		public bool ShowFooter { get; set; }
		public bool ShowPreview { get; set; }
		public int HeaderHeight { get; set; }
		public int FooterHeight { get; set; }
		#endregion settings
		protected void UpdateGallery(XtraMdiTabPageCollection pages) {
			Gallery.BeginInit();
			Gallery.Gallery.DistanceItemImageToText = 4;
			Gallery.Gallery.ShowGroupCaption = false;
			Gallery.Gallery.ShowItemText = true;
			Gallery.Gallery.ColumnCount = Manager.Pages.Count > GalleryMaxRows ? 2 : 1;
			Gallery.Gallery.ShowScrollBar = DevExpress.XtraBars.Ribbon.Gallery.ShowScrollBar.Auto;
			Gallery.Gallery.ItemCheckMode = DevExpress.XtraBars.Ribbon.Gallery.ItemCheckMode.SingleCheck;
			Gallery.Gallery.ItemImageLocation = DevExpress.Utils.Locations.Left;
			Gallery.Gallery.Appearance.ItemCaptionAppearance.Normal.TextOptions.VAlignment = VertAlignment.Center;
			Gallery.Gallery.Appearance.ItemCaptionAppearance.Normal.TextOptions.HAlignment = HorzAlignment.Near;
			Gallery.Gallery.Appearance.ItemCaptionAppearance.Normal.Options.UseTextOptions = true;
			Gallery.Gallery.Appearance.ItemCaptionAppearance.Hovered.TextOptions.VAlignment = VertAlignment.Center;
			Gallery.Gallery.Appearance.ItemCaptionAppearance.Hovered.TextOptions.HAlignment = HorzAlignment.Near;
			Gallery.Gallery.Appearance.ItemCaptionAppearance.Hovered.Options.UseTextOptions = true;
			Gallery.Gallery.Appearance.ItemCaptionAppearance.Pressed.TextOptions.VAlignment = VertAlignment.Center;
			Gallery.Gallery.Appearance.ItemCaptionAppearance.Pressed.TextOptions.HAlignment = HorzAlignment.Near;
			Gallery.Gallery.Appearance.ItemCaptionAppearance.Pressed.Options.UseTextOptions = true;
			Gallery.Gallery.UseMaxImageSize = true;
			Gallery.Gallery.FixedImageSize = false;
			mdiChildrenGroup = new GalleryItemGroup();
			mdiChildrenGroup.Caption = "MDI Children";
			IEnumerable<XtraMdiTabPage> orderedPages = GetTabOrderedPages(pages);
			foreach(XtraMdiTabPage page in orderedPages) {
				DocumentSelectorItem item = new DocumentSelectorItem(page,
						Gallery.Gallery.ViewInfo.PaintAppearance.ItemCaptionAppearance.Normal,
						Gallery.Gallery.ViewInfo.PaintAppearance.ItemDescriptionAppearance.Normal
					);
				Manager.RaiseCustomDocumentSelectorItem(item);
				mdiChildrenGroup.Items.Add(item);
			}
			Gallery.Gallery.Groups.Add(mdiChildrenGroup);
			Gallery.EndInit();
		}
		IEnumerable<XtraMdiTabPage> GetTabOrderedPages(XtraMdiTabPageCollection pages) {
			XtraMdiTabPage[] pageArray = new XtraMdiTabPage[pages.Count];
			for(int i = 0; i < pageArray.Length; i++) {
				pageArray[i] = pages[i];
			}
			XtraMdiTabPage[] orderedPages = new XtraMdiTabPage[pageArray.Length];
			int selectedIndex = Array.IndexOf(pageArray, Manager.SelectedPage);
			if(selectedIndex != -1) {
				int rightCount = pageArray.Length - selectedIndex;
				Array.Copy(pageArray, selectedIndex, orderedPages, 0, rightCount);
				if(selectedIndex > 0) {
					Array.Copy(pageArray, 0, orderedPages, rightCount, pageArray.Length - rightCount);
				}
				if(StartPage != null) {
					int startIndex = Array.IndexOf(orderedPages, StartPage);
					if(startIndex != -1 && orderedPages.Length > 2) {
						XtraMdiTabPage tmp = orderedPages[startIndex];
						Array.Copy(orderedPages, 1, orderedPages, 2, startIndex - 1);
						orderedPages[1] = tmp;
					}
				}
				return orderedPages;
			}
			return pageArray;
		}
		protected void OnNextPageChanged() {
			UpdateSelectedItem();
			if(NextPage != null) {
				ViewInfo.UpdatePreview(NextPage.MdiChild);
			}
		}
		protected DocumentSelectorItem GetItem(XtraMdiTabPage page) {
			foreach(DocumentSelectorItem item in mdiChildrenGroup.Items) {
				if(item.Page == page) return item;
			}
			return null;
		}
		protected void UpdateSelectedItem() {
			foreach(DocumentSelectorItem item in mdiChildrenGroup.Items) {
				bool selected = (item.Page == NextPage);
				item.Checked = selected;
				if(selected) {
					SelectedItem = item;
				}
			}
		}
		protected DocumentSelectorItem SelectedItem {
			get { return selectedItemCore; }
			private set {
				if(selectedItemCore == value) return;
				selectedItemCore = value;
				OnSelectedItemChanged();
			}
		}
		protected virtual void OnSelectedItemChanged() {
			Invalidate(ViewInfo.Header);
			Invalidate(ViewInfo.Preview);
			Invalidate(ViewInfo.Footer);
		}
		protected void UpdatePainters() {
			BackgroundPainter = SkinElementPainter.Default;
			if(!ViewInfo.UseSkin)
				BackgroundPainter = GetBackgroundPainter(Manager.ViewInfo.TabControl.LookAndFeel);
		}
		protected virtual ObjectPainter GetBackgroundPainter(UserLookAndFeel lookAndFeel) {
			return LookAndFeelPainterHelper.GetPainter(lookAndFeel, lookAndFeel.ActiveStyle).Header;
		}
		protected virtual DocumentSelectorViewInfo CreateViewInfo() {
			return new DocumentSelectorViewInfo(this);
		}
		protected GalleryControl CreateGallery() {
			return new GalleryControl();
		}
		protected override void OnResize(EventArgs e) {
			base.OnResize(e);
			ViewInfo.SetDirty();
		}
		protected override void OnPaint(System.Windows.Forms.PaintEventArgs e) {
			if(Disposing) return;
			base.OnPaint(e);
			using(GraphicsCache cache = new GraphicsCache(e.Graphics)) {
				if(UpdateBeforePaint(cache)) {
					DrawBackground(cache);
					DrawPreview(cache);
					DrawCaptions(cache);
				}
			}
		}
		bool UpdateBeforePaint(GraphicsCache cache) {
			if(!ViewInfo.IsReady) {
				ViewInfo.Calc(cache, ClientRectangle);
				EnsureGalleryLocation();
			}
			return ViewInfo.IsReady;
		}
		void EnsureGalleryLocation() {
			galleryCore.Bounds = ViewInfo.Gallery;
		}
		protected virtual void DrawBackground(GraphicsCache cache) {
			ObjectPainter.DrawObject(cache, BackgroundPainter, ViewInfo.Background);
		}
		protected virtual void DrawCaptions(GraphicsCache cache) {
			if(SelectedItem == null) return;
			SelectedItem.AppearanceHeader.DrawString(cache, SelectedItem.HeaderText, ViewInfo.Header);
			SelectedItem.AppearanceFooter.DrawString(cache, SelectedItem.FooterText, ViewInfo.Footer);
		}
		protected virtual void DrawPreview(GraphicsCache cache) {
			if(ViewInfo.PreviewImage != null)
				cache.Graphics.DrawImage(ViewInfo.PreviewImage, ViewInfo.Preview);
		}
		protected internal float PreviewAspect {
			get { return (float)Manager.MdiParent.Height / (float)Manager.MdiParent.Width; }
		}
	}
	public class DocumentSelectorViewInfo : IDisposable {
		DocumentSelector ownerCore;
		Rectangle boundsCore, galleryCore, previewCore, headerCore, footerCore;
		bool isReadyCore;
		ObjectInfoArgs backgroundCore;
		Skin skin;
		public void Dispose() {
			if(PreviewImage != null) {
				PreviewImage.Dispose();
				previewImageCore = null;
			}
		}
		public DocumentSelector Owner {
			get { return ownerCore; }
		}
		public bool IsReady {
			get { return isReadyCore; }
		}
		public Rectangle Bounds {
			get { return boundsCore; }
		}
		public Rectangle Gallery {
			get { return galleryCore; }
		}
		public Rectangle Header {
			get { return headerCore; }
		}
		public Rectangle Footer {
			get { return footerCore; }
		}
		public Rectangle Preview {
			get { return previewCore; }
		}
		public DocumentSelectorViewInfo(DocumentSelector owner) {
			ownerCore = owner;
			UseSkin = !string.IsNullOrEmpty(Owner.CurrentSkinProvider.SkinName);
			if(UseSkin)
				skin = BarSkins.GetSkin(Owner.CurrentSkinProvider);
		}
		public readonly bool UseSkin;
		public ObjectInfoArgs Background {
			get { return backgroundCore; }
		}
		public void SetDirty() {
			isReadyCore = false;
			backgroundCore = null;
			boundsCore = galleryCore = previewCore = headerCore = footerCore = Rectangle.Empty;
		}
		Image previewImageCore;
		public Image PreviewImage {
			get { return previewImageCore; }
		}
		public void UpdatePreview(Form form) {
			XtraBars.Docking2010.Ref.Dispose(ref previewImageCore);
			if(!Owner.ShowPreview)
				return;
			XtraTabbedMdiManager manager = Owner.Manager;
			Rectangle bounds = manager.ViewInfo.Bounds;
			previewImageCore = new Bitmap(bounds.Width, bounds.Height);
			using(Graphics graphics = Graphics.FromImage(PreviewImage)) {
				using(GraphicsCache cache = new GraphicsCache(graphics)) {
					DrawBackground(cache, bounds);
					DrawTab(cache, bounds, form);
					DrawContent(graphics, form);
				}
			}
		}
		ObjectPainter backGroundPainter;
		ObjectPainter GetBackgroundPainter() {
			UserLookAndFeel lf = Owner.Manager.ViewInfo.TabControl.LookAndFeel;
			BaseLookAndFeelPainters painters = DevExpress.LookAndFeel.LookAndFeelPainterHelper.GetPainter(lf, lf.ActiveStyle);
			return painters.Header;
		}
		protected virtual void DrawBackground(GraphicsCache cache, Rectangle bounds) {
			ObjectInfoArgs info = new HeaderObjectInfoArgs();
			info.Bounds = bounds;
			info.Cache = cache;
			if(backGroundPainter == null)
				backGroundPainter = GetBackgroundPainter();
			backGroundPainter.DrawObject(info);
		}
		protected void DrawTab(GraphicsCache cache, Rectangle bounds, Form target) {
			TabDrawArgs args = new TabDrawArgs(cache, Owner.Manager.ViewInfo, bounds);
			AllowDrawPage(false, Owner.Manager.ViewInfo.HeaderInfo.VisiblePages, target);
			Owner.Manager.Painter.Draw(args);
			AllowDrawPage(true, Owner.Manager.ViewInfo.HeaderInfo.VisiblePages, target);
		}
		protected void DrawContent(Graphics graphics, Form target) {
			using(Image formPreview = MDIChildPreview.Create(target, Color.Black)) {
				graphics.DrawImage(formPreview, Owner.Manager.ViewInfo.PageClientBounds);
			}
		}
		void AllowDrawPage(bool allow, PageViewInfoCollection pages, Form exclude) {
			foreach(BaseTabPageViewInfo pageInfo in pages) {
				if(((XtraMdiTabPage)pageInfo.Page).MdiChild != exclude)
					pageInfo.AllowDraw = allow;
			}
		}
		int clientMargin = 12;
		public void Calc(GraphicsCache cache, Rectangle bounds) {
			if(IsReady) return;
			boundsCore = bounds;
			backgroundCore = CalcBorderInfo(cache, bounds);
			Rectangle client = Rectangle.Inflate(bounds, -clientMargin, -clientMargin);
			int margin = CalcMargin(cache, client);
			int header = CalcHeaderHeight(cache, client);
			int footer = CalcFooterHeight(cache, client);
			headerCore = new Rectangle(client.X, client.Y, client.Width, header);
			footerCore = new Rectangle(client.X, client.Bottom - footer, client.Width, footer);
			int content = client.Height - header - margin - margin - footer;
			int previewWidth = Owner.ShowPreview ? (client.Width - Owner.GalleryColumnWidth * Owner.Gallery.Gallery.ColumnCount - clientMargin) : 0;
			int previewHeight = Owner.ShowPreview ? Math.Min(content, Round((float)previewWidth * Owner.PreviewAspect)) : 0;
			galleryCore = new Rectangle(client.X, client.Y + header + margin, client.Width - previewWidth - (Owner.ShowPreview ? clientMargin : 0), content);
			previewCore = new Rectangle(
				Gallery.Right + clientMargin,
				client.Y + header + margin + (content - previewHeight) / 2,
				previewWidth, previewHeight);
			isReadyCore = true;
		}
		static int Round(float value) {
			return value > 0 ? (int)(value + 0.5f) : (int)(value - 0.5f);
		}
		public Size CalcMinSize(int galleryHeight) {
			using(Bitmap bmp = new Bitmap(100, 100)) {
				using(Graphics g = Graphics.FromImage(bmp)) {
					using(GraphicsCache cache = new GraphicsCache(g)) {
						Rectangle client = new Rectangle(0, 0, 300, 300);
						int margin = CalcMargin(cache, client);
						int header = CalcHeaderHeight(cache, client);
						int footer = CalcFooterHeight(cache, client);
						int height = clientMargin + header + margin + galleryHeight + margin + footer + clientMargin;
						int width = clientMargin + Owner.GalleryColumnWidth * Owner.Gallery.Gallery.ColumnCount + margin +
							(Owner.ShowPreview ? Round((float)galleryHeight / Owner.PreviewAspect) : 0) + clientMargin;
						return new Size(width, height);
					}
				}
			}
		}
		protected virtual int CalcMargin(GraphicsCache cache, Rectangle client) {
			return 2;
		}
		protected virtual int CalcBestColumnWidth(GraphicsCache cache, XtraTabPageCollection pages) {
			return 150;
		}
		protected virtual int CalcHeaderHeight(GraphicsCache cache, Rectangle client) {
			return Owner.ShowHeader ? Math.Max(0, Owner.HeaderHeight) : 0;
		}
		protected virtual int CalcFooterHeight(GraphicsCache cache, Rectangle client) {
			return Owner.ShowFooter ? Math.Max(0, Owner.FooterHeight) : 0;
		}
		protected virtual ObjectInfoArgs CalcBorderInfo(GraphicsCache cache, Rectangle bounds) {
			ObjectInfoArgs info;
			if(UseSkin) {
				info = new SkinElementInfo(skin[BarSkins.SkinAlertWindow], bounds);
			}
			else {
				info = new HeaderObjectInfoArgs();
				info.Bounds = bounds;
			}
			return info;
		}
	}
	public class DocumentSelectorItem : GalleryItem {
		XtraMdiTabPage pageCore;
		string headerTextCore;
		string footerTextCore;
		AppearanceObject appearanceHeaderCore;
		AppearanceObject appearanceFooterCore;
		public XtraMdiTabPage Page {
			get { return pageCore; }
		}
		public AppearanceObject AppearanceHeader {
			get { return appearanceHeaderCore; }
		}
		public AppearanceObject AppearanceFooter {
			get { return appearanceFooterCore; }
		}
		public DocumentSelectorItem(XtraMdiTabPage page, AppearanceObject header, AppearanceObject footer) {
			pageCore = page;
			appearanceHeaderCore = new AppearanceObject();
			appearanceFooterCore = new AppearanceObject();
			AppearanceHelper.Combine(AppearanceHeader, new AppearanceObject[] { GetHeaderAppearance(Page), header });
			AppearanceHelper.Combine(AppearanceFooter, new AppearanceObject[] { GetFooterAppearance(Page), footer });
			AppearanceHeader.TextOptions.Trimming = Trimming.EllipsisPath;
			AppearanceFooter.TextOptions.Trimming = Trimming.EllipsisCharacter;
			HeaderText = Caption = GetCaption(Page);
			Image = Page.Image ?? XtraMdiTabPage.GetImage(Page.MdiChild);
		}
		protected virtual AppearanceObject GetHeaderAppearance(XtraMdiTabPage page) {
			return new AppearanceObject(AppearanceDefault.Window);
		}
		protected virtual AppearanceObject GetFooterAppearance(XtraMdiTabPage page) {
			return new AppearanceObject(AppearanceDefault.Control);
		}
		protected virtual string GetCaption(XtraMdiTabPage page) {
			return page.Text;
		}
		public string HeaderText {
			get { return headerTextCore; }
			set { headerTextCore = value; }
		}
		public string FooterText {
			get { return footerTextCore; }
			set { footerTextCore = value; }
		}
	}
}
