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
using DevExpress.Skins;
using DevExpress.Utils.Drawing;
using DevExpress.XtraEditors.ButtonPanel;
namespace DevExpress.XtraBars.Docking2010.Views.WindowsUI {
	public class WindowsUIViewPainter : BaseViewPainter {
		static WindowsUIViewPainter() {
			DefaultFont = SegoeUIFontsCache.GetSegoeUIFont();
			DefaultFont2 = SegoeUIFontsCache.GetSegoeUILightFont();
			DefaultFontSplashScreen = SegoeUIFontsCache.GetSegoeUILightFont(6f);
		}
		IDictionary<Type, ObjectPainter> elementPainters;
		IDictionary<Type, ObjectPainter> elementHeaderPainters;
		public WindowsUIViewPainter(WindowsUIView view)
			: base(view) {
			elementPainters = new Dictionary<Type, ObjectPainter>();
			elementHeaderPainters = new Dictionary<Type, ObjectPainter>();
			RegisterPainters();
		}
		public WindowsUIViewInfo Info {
			get { return View.ViewInfo as WindowsUIViewInfo; }
		}
		protected override void DrawCore(GraphicsCache bufferedCache, Rectangle clip) {
			DrawBackground(bufferedCache, clip);
			DrawContentContainer(bufferedCache);
		}
		protected virtual void DrawContentContainer(GraphicsCache bufferedCache) {
			if(Info.ContentInfo != null)
				Info.ContentInfo.Draw(bufferedCache);
		}
		protected virtual void RegisterPainters() {
			RegisterPainter<IPageInfo>(new PageInfoPainter());
			RegisterPainter<IFlyoutInfo>(new FlyoutInfoPainter());
			RegisterPainter<IDetailContainerInfo>(new DetailContainerInfoPainter());
			RegisterPainter<IOverviewContainerInfo>(new OverviewContainerInfoPainter());
			RegisterPainter<ITileContainerInfo>(new TileContainerInfoPainter());
			RegisterPainter<IPageGroupInfo>(new PageGroupInfoPainter());
			RegisterPainter<ITabbedGroupInfo>(new TabbedGroupInfoPainter());
			RegisterPainter<ISlideGroupInfo>(new SlideGroupInfoPainter());
			RegisterPainter<ISplitGroupInfo>(new SplitGroupInfoPainter());
			RegisterHeaderPainter<ISlideGroupInfo>(new SlideGroupHeaderInfoPainter());
			RegisterHeaderPainter<IPageGroupInfo>(new PageGroupHeaderInfoPainter());
			RegisterHeaderPainter<ITabbedGroupInfo>(new TabbedGroupHeaderInfoPainter());
			RegisterHeaderPainter<IPageInfo>(new ContentContainerHeaderInfoPainter());
			RegisterHeaderPainter<IFlyoutInfo>(new ContentContainerHeaderInfoPainter());
			RegisterHeaderPainter<ITileContainerInfo>(new ContentContainerHeaderInfoPainter());
			RegisterHeaderPainter<ISplitGroupInfo>(new ContentContainerHeaderInfoPainter());
			RegisterHeaderPainter<IDetailContainerInfo>(new ContentContainerHeaderInfoPainter());
			RegisterHeaderPainter<IOverviewContainerInfo>(new ContentContainerHeaderInfoPainter());
			RegisterPainter<INavigationActionsBarInfo>(new NavigationActionsBarInfoPainter());
			RegisterPainter<IContextActionsBarInfo>(new ContextActionsBarInfoPainter());
		}
		protected void RegisterPainter<T>(ObjectPainter painter)
			where T : IBaseElementInfo {
			elementPainters.Add(typeof(T), painter);
		}
		protected void RegisterHeaderPainter<T>(ObjectPainter painter)
			where T : IBaseElementInfo {
			elementHeaderPainters.Add(typeof(T), painter);
		}
		public virtual BaseButtonPainter GetHeaderButtonPainter() {
			return new BaseButtonPainter();
		}
		public virtual ObjectPainter GetActionBarButtonsPanelPainter() {
			return new ActionBarButtonsPanelPainter();
		}
		public virtual ObjectPainter GetPageGroupButtonsPanelPainter() {
			return new ButtonPanelOffice2000Painter();
		}
		public virtual ObjectPainter GetTabbedGroupButtonsPanelTabPainter() {
			return new TabbedGroupButtonsPanelTabPainter();
		}
		public virtual ObjectPainter GetTabbedGroupButtonsPanelTilePainter() {
			return new TabbedGroupButtonsPanelTilePainter();
		}
		public ObjectPainter GetPainter(IBaseElementInfo info) {
			return GetPainterCore(info.GetUIElementKey());
		}
		public ObjectPainter GetHeaderPainter(IBaseElementInfo info) {
			return GetHeaderPainterCore(info.GetUIElementKey());
		}
		protected ObjectPainter GetPainterCore(Type key) {
			ObjectPainter painter;
			return elementPainters.TryGetValue(key, out painter) ? painter : null;
		}
		protected ObjectPainter GetHeaderPainterCore(Type key) {
			ObjectPainter painter;
			return elementHeaderPainters.TryGetValue(key, out painter) ? painter : null;
		}
		public virtual ObjectPainter GetSplashScreenPainter() {
			return new Customization.SplashScreenPainter();
		}
		public virtual ObjectPainter GetTransitionScreenPainter() {
			return new Customization.TransitionScreenPainter();
		}
		public virtual ObjectPainter GetFlyoutPainter() {
			return new Customization.FlyoutPainter();
		}
		public static Font DefaultFont { get; set; }
		public static Font DefaultFont2 { get; set; }
		public static Font DefaultFontSplashScreen { get; set; }
	}
	public class WindowsUIViewSkinPainter : WindowsUIViewPainter {
		Skin skin;
		public WindowsUIViewSkinPainter(WindowsUIView view)
			: base(view) {
			skin = DockingSkins.GetSkin(View.ElementsLookAndFeel);
		}
		protected internal override Color GetBackColor(Color parentBackColor) {
			Color skinColor = skin.Properties.GetColor(DockingSkins.DocumentGroupBackColor);
			return skinColor.IsEmpty ? parentBackColor : skinColor;
		}
		protected internal override ObjectPainter GetDocumentSelectorHeaderPainter() {
			return new Customization.DocumentSelectorHeaderSkinPainter(View.ElementsLookAndFeel);
		}
		protected internal override ObjectPainter GetDocumentSelectorFooterPainter() {
			return new Customization.DocumentSelectorFooterSkinPainter(View.ElementsLookAndFeel);
		}
		protected internal override ObjectPainter GetDocumentSelectorItemsListPainter() {
			return new Customization.DocumentSelectorItemsListSkinPainter(View.ElementsLookAndFeel);
		}
		protected internal override ObjectPainter GetDocumentSelectorPreviewPainter() {
			return new Customization.DocumentSelectorPreviewSkinPainter(View.ElementsLookAndFeel);
		}
		protected internal override ObjectPainter GetDocumentSelectorBackgroundPainter() {
			return new Customization.DocumentSelectorBackgroundSkinPainter(View.ElementsLookAndFeel);
		}
		public override BaseButtonPainter GetHeaderButtonPainter() {
			return new HeaderButtonSkinPainter(View.ElementsLookAndFeel);
		}
		public override ObjectPainter GetActionBarButtonsPanelPainter() {
			return new ActionBarButtonsSkinPanelPainter(View.ElementsLookAndFeel);
		}
		public override ObjectPainter GetPageGroupButtonsPanelPainter() {
			return new PageGroupButtonsPanelSkinPainter(View.ElementsLookAndFeel);
		}
		public override ObjectPainter GetTabbedGroupButtonsPanelTabPainter() {
			return new TabbedGroupButtonsPanelTabSkinPainter(View.ElementsLookAndFeel);
		}
		public override ObjectPainter GetTabbedGroupButtonsPanelTilePainter() {
			return new TabbedGroupButtonsPanelTileSkinPainter(View.ElementsLookAndFeel);
		}
		public override ObjectPainter GetSplashScreenPainter() {
			return new Customization.SplashScreenSkinPainter(View.ElementsLookAndFeel);
		}
		public override ObjectPainter GetWaitScreenPainter() {
			return new Customization.WaitScreenSkinPainter(View.ElementsLookAndFeel);
		}
		public override ObjectPainter GetTransitionScreenPainter() {
			return new Customization.TransitionScreenSkinPainter(View.ElementsLookAndFeel);
		}
		public override ObjectPainter GetFlyoutPainter() {
			return new Customization.FlyoutSkinPainter(View.ElementsLookAndFeel);
		}
		protected override void RegisterPainters() {
			RegisterPainter<IPageInfo>(new PageInfoSkinPainter(View.ElementsLookAndFeel));
			RegisterPainter<IFlyoutInfo>(new FlyoutInfoSkinPainter(View.ElementsLookAndFeel));
			RegisterPainter<IDetailContainerInfo>(new DetailContainerInfoSkinPainter(View.ElementsLookAndFeel));
			RegisterPainter<IOverviewContainerInfo>(new OverviewContainerInfoSkinPainter(View.ElementsLookAndFeel));
			RegisterPainter<ITileContainerInfo>(new TileContainerInfoSkinPainter(View.ElementsLookAndFeel));
			RegisterPainter<IPageGroupInfo>(new PageGroupInfoSkinPainter(View.ElementsLookAndFeel));
			RegisterPainter<ITabbedGroupInfo>(new TabbedGroupInfoSkinPainter(View.ElementsLookAndFeel));
			RegisterPainter<ISlideGroupInfo>(new SlideGroupInfoSkinPainter(View.ElementsLookAndFeel));
			RegisterPainter<ISplitGroupInfo>(new SplitGroupInfoSkinPainter(View.ElementsLookAndFeel));
			RegisterHeaderPainter<ISlideGroupInfo>(new SlideGroupHeaderInfoSkinPainter(View.ElementsLookAndFeel));
			RegisterHeaderPainter<IPageGroupInfo>(new PageGroupHeaderInfoSkinPainter(View.ElementsLookAndFeel));
			RegisterHeaderPainter<ITabbedGroupInfo>(new TabbedGroupHeaderInfoSkinPainter(View.ElementsLookAndFeel));
			RegisterHeaderPainter<IPageInfo>(new ContentContainerHeaderInfoSkinPainter(View.ElementsLookAndFeel));
			RegisterHeaderPainter<IFlyoutInfo>(new ContentContainerHeaderInfoSkinPainter(View.ElementsLookAndFeel));
			RegisterHeaderPainter<ITileContainerInfo>(new ContentContainerHeaderInfoSkinPainter(View.ElementsLookAndFeel));
			RegisterHeaderPainter<ISplitGroupInfo>(new ContentContainerHeaderInfoSkinPainter(View.ElementsLookAndFeel));
			RegisterHeaderPainter<IDetailContainerInfo>(new ContentContainerHeaderInfoSkinPainter(View.ElementsLookAndFeel));
			RegisterHeaderPainter<IOverviewContainerInfo>(new ContentContainerHeaderInfoSkinPainter(View.ElementsLookAndFeel));
			RegisterPainter<INavigationActionsBarInfo>(new NavigationActionsBarInfoSkinPainter(View.ElementsLookAndFeel));
			RegisterPainter<IContextActionsBarInfo>(new ContextActionsBarInfoSkinPainter(View.ElementsLookAndFeel));
		}
	}
	class ContentContainerInfoArgs : ObjectInfoArgs {
		IContentContainerInfo infoCore;
		public ContentContainerInfoArgs(GraphicsCache cache, IContentContainerInfo info)
			: base(cache) {
			this.infoCore = info;
		}
		public IContentContainerInfo Info {
			get { return infoCore; }
		}
	}
	abstract class ContentContainerInfoPainter : ObjectPainter {
		ContentContainerHeaderInfoPainter headerPainterCore;
		protected ContentContainerInfoPainter() {
			headerPainterCore = CreateHeaderPainter();
		}
		public ContentContainerHeaderInfoPainter HeaderPainter {
			get { return headerPainterCore; }
		}
		public sealed override void DrawObject(ObjectInfoArgs e) {
			ContentContainerInfoArgs args = e as ContentContainerInfoArgs;
			DrawHeader(e.Cache, args.Info.HeaderInfo);
			DrawContent(e.Cache, args.Info);
		}
		public virtual int ButtonToTextInterval {
			get { return 5; }
		}
		public Size GetHeadersSizeByContentSize(Size headersSize) {
			return GetHeadersBoundsByContentRectangle(new Rectangle(Point.Empty, headersSize)).Size;
		}
		public virtual Rectangle GetHeadersBoundsByContentRectangle(Rectangle headers) {
			return headers;
		}
		protected virtual ContentContainerHeaderInfoPainter CreateHeaderPainter() {
			return new ContentContainerHeaderInfoPainter();
		}
		protected virtual void DrawHeader(GraphicsCache cache, IContentContainerHeaderInfo info) {
			ObjectPainter.DrawObject(cache, HeaderPainter, new ContentContainerHeaderInfoArgs(cache, info));
		}
		protected abstract void DrawContent(GraphicsCache cache, IContentContainerInfo info);
		public virtual System.Windows.Forms.Padding GetContentMargins() {
			return System.Windows.Forms.Padding.Empty;
		}
		public virtual int GetHeaderOffset() {
			return 0;
		}
	}
	static class SegoeUIFontsCache {
		static IDictionary<string, Font> cache;
		static SegoeUIFontsCache() {
			cache = new Dictionary<string, Font>();
		}
		public static Font GetSegoeUIFont(float sizeGrow = 0) {
			float defaultSize = DevExpress.Utils.AppearanceObject.DefaultFont.Size;
			return GetFont("Segoe UI", defaultSize + sizeGrow);
		}
		public static Font GetSegoeUILightFont(float sizeGrow = 0) {
			float defaultSize = DevExpress.Utils.AppearanceObject.DefaultFont.Size;
			return GetFont("Segoe UI Light", defaultSize + sizeGrow);
		}
		public static Font GetFont(string familyName, float size) {
			string key = familyName + "#" + size.ToString();
			Font result;
			if(!cache.TryGetValue(key, out result)) {
				try {
					var family = FindFontFamily(familyName);
					result = new Font(family ?? FontFamily.GenericSansSerif, size);
				}
				catch(ArgumentException) { result = DevExpress.Utils.AppearanceObject.DefaultFont; }
				cache.Add(key, result);
			}
			return result;
		}
		static FontFamily FindFontFamily(string familyName) {
			return Array.Find(FontFamily.Families, (f) => f.Name == familyName);
		}
	}
}
