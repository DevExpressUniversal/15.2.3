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
using System.Drawing;
using DevExpress.LookAndFeel;
using DevExpress.Skins;
using DevExpress.Utils;
using DevExpress.Utils.Animation;
using DevExpress.Utils.Drawing;
using DevExpress.Utils.Internal;
using DevExpress.XtraBars.Docking2010.Views.WindowsUI;
namespace DevExpress.XtraBars.Docking2010.Customization {
	public class SplashScreenAdornerInfoArgs : AsyncAdornerElementInfoArgs {
		SplashScreenAdornerInfoArgs(WindowsUIView owner)
			: base(owner) {
		}
		protected sealed override ObjectPainter GetPainter() {
			return ((WindowsUIViewPainter)Owner.Painter).GetSplashScreenPainter();
		}
		public new WindowsUIView Owner {
			get { return base.Owner as WindowsUIView; }
		}
		ILoadingAnimator loadingAnimatorCore;
		public ILoadingAnimator LoadingAnimator {
			get { return loadingAnimatorCore; }
		}
		IntPtr adornerHandle;		
		protected sealed override void BeginAsync(IntPtr adornerHandle) {
			this.adornerHandle = adornerHandle;
			loadingAnimatorCore = new RingAnimator();
			LoadingAnimator.Invalidate += LoadingAnimator_Invalidate;
		}
		protected sealed override void EndAsync() {
			DestroyCore();
		}
		void LoadingAnimator_Invalidate(object sender, EventArgs e) {
			if(WindowsUIView.IsAboutOpen())
				LayeredWindowMessanger.PostClose(adornerHandle);
			else
				LayeredWindowMessanger.PostInvalidate(adornerHandle);
		}
		protected sealed override void Destroy() {
			LayeredWindowMessanger.PostClose(adornerHandle);
			DestroyCore();
		}
		protected virtual void DestroyCore() {
			if(LoadingAnimator == null) return;
			LoadingAnimator.Invalidate -= LoadingAnimator_Invalidate;
			Ref.Dispose(ref loadingAnimatorCore);
		}
		public static SplashScreenAdornerInfoArgs EnsureInfoArgs(ref AsyncAdornerElementInfo target, WindowsUIView owner) {
			SplashScreenAdornerInfoArgs args;
			if(target == null) {
				args = new SplashScreenAdornerInfoArgs(owner);
				target = new AsyncAdornerElementInfo(new AsyncAdornerOpaquePainter(), args);
			}
			else args = target.InfoArgs as SplashScreenAdornerInfoArgs;
			IntPtr handle = owner.Manager.GetOwnerControlHandle();
			args.Bounds = System.Windows.Forms.Screen.FromHandle(handle).Bounds;
			args.SetDirty();
			return args;
		}
	}
	public class SplashScreenPainter : ObjectPainter {
		static Font DefaultCaptionFont = SegoeUIFontsCache.GetSegoeUILightFont(25f);
		static Font DefaultLoadingDescriptionFont = SegoeUIFontsCache.GetSegoeUILightFont(8f);
		AppearanceDefault defaultAppearance;
		public sealed override AppearanceDefault DefaultAppearance {
			get {
				if(defaultAppearance == null)
					defaultAppearance = CreateDefaultAppearance();
				return defaultAppearance;
			}
		}
		AppearanceDefault defaultAppearanceCaption;
		public AppearanceDefault DefaultAppearanceCaption {
			get {
				if(defaultAppearanceCaption == null)
					defaultAppearanceCaption = CreateDefaultAppearanceCaption();
				return defaultAppearanceCaption;
			}
		}
		AppearanceDefault defaultAppearanceLoadingDescription;
		public AppearanceDefault DefaultAppearanceLoadingDescription {
			get {
				if(defaultAppearanceLoadingDescription == null)
					defaultAppearanceLoadingDescription = CreateDefaultAppearanceLoadingDescription();
				return defaultAppearanceLoadingDescription;
			}
		}
		protected virtual AppearanceDefault CreateDefaultAppearanceCaption() {
			return new AppearanceDefault(SystemColors.ControlText, Color.Empty, DefaultCaptionFont);
		}
		protected virtual AppearanceDefault CreateDefaultAppearanceLoadingDescription() {
			return new AppearanceDefault(SystemColors.ControlText, Color.Empty, DefaultLoadingDescriptionFont);
		}
		protected virtual AppearanceDefault CreateDefaultAppearance() {
			return new AppearanceDefault(Color.Empty, Color.Transparent);
		}
		public sealed override void DrawObject(ObjectInfoArgs e) {
			SplashScreenAdornerInfoArgs ea = e as SplashScreenAdornerInfoArgs;
			var args = Calc(e.Cache, ea);
			DrawBackground(e.Cache, args);
			BufferedDraw(e.Cache, args.Content, DrawContent, args);
			BufferedDraw(e.Cache, args.LoadingElements, DrawLoadingElements, args);
			ea.LoadingAnimator.DrawAnimatedItem(e.Cache, args.LoadingIndicatorBounds, args.PaintAppearanceLoadingDescription.ForeColor);
		}
		protected virtual void DrawBackground(GraphicsCache cache, SplashScreenInfoArgs args) {
			args.PaintAppearance.DrawBackground(cache, args.Bounds);
		}
		protected virtual void DrawContent(GraphicsCache bufferedCache, SplashScreenInfoArgs args) {
			if(!args.ImageBounds.Size.IsEmpty) {
				Image image = args.UseDefaultImage ? DevExpress.Utils.Helpers.ColoredImageHelper.GetColoredImage(args.Image, args.PaintAppearanceCaption.ForeColor) : args.Image;
				bufferedCache.Graphics.DrawImage(image, args.ImageBounds);
			}
			if(!args.CaptionBounds.Size.IsEmpty)
				args.PaintAppearanceCaption.DrawString(bufferedCache, args.Caption, args.CaptionBounds);
		}
		protected virtual void DrawLoadingElements(GraphicsCache bufferedCache, SplashScreenInfoArgs args) {
			if(!args.LoadingDescriptionBounds.Size.IsEmpty)
				args.PaintAppearanceLoadingDescription.DrawString(bufferedCache, args.LoadingDescription, args.LoadingDescriptionBounds);
		}
		delegate void BufferedDrawAction(GraphicsCache bufferedCache, SplashScreenInfoArgs args);
		void BufferedDraw(GraphicsCache cache, Rectangle bounds, BufferedDrawAction drawAction, SplashScreenInfoArgs args) {
			Rectangle rect = new Rectangle(Point.Empty, bounds.Size);
			if(rect.Height == 0 || rect.Width == 0) return;
			using(Bitmap img = new Bitmap(rect.Width, rect.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb)) {
				using(Graphics g = Graphics.FromImage(img)) {
					using(XtraBufferedGraphics bg = XtraBufferedGraphicsManager.Current.Allocate(g, rect)) {
						bg.Graphics.TranslateTransform(-bounds.X, -bounds.Y);
						using(GraphicsCache bufferedCache = new GraphicsCache(bg.Graphics)) {
							args.PaintAppearance.DrawBackground(bufferedCache, bounds);
							drawAction(bufferedCache, args);
						}
						bg.Render();
					}
				}
				cache.Graphics.DrawImageUnscaled(img, bounds.Location);
			}
		}
		protected virtual SplashScreenInfoArgs Calc(GraphicsCache cache, SplashScreenAdornerInfoArgs e) {
			SplashScreenInfoArgs args = new SplashScreenInfoArgs();
			args.Bounds = e.Bounds;
			ISplashScreenProperties properties = e.Owner.SplashScreenProperties;
			args.UseDefaultImage = properties.Image == null;
			args.Image = args.UseDefaultImage ? Resources.CommonResourceLoader.GetImage("SplashScreenLogo") : properties.Image;
			args.Caption = (properties.Caption ?? e.Owner.Caption) ?? DocumentManagerLocalizer.GetString(DocumentManagerStringId.SplashScreenCaption);
			args.LoadingDescription = properties.LoadingDescription ?? DocumentManagerLocalizer.GetString(DocumentManagerStringId.LoadingIndicatorDescription);
			args.PaintAppearance = new FrozenAppearance();
			args.PaintAppearanceCaption = new FrozenAppearance();
			args.PaintAppearanceLoadingDescription = new FrozenAppearance();
			var main = e.Owner.AppearanceSplashScreen;
			AppearanceHelper.Combine(args.PaintAppearance, new AppearanceObject[] { main }, DefaultAppearance);
			AppearanceHelper.Combine(args.PaintAppearanceCaption, new AppearanceObject[] { properties.AppearanceCaption, main }, DefaultAppearanceCaption);
			AppearanceHelper.Combine(args.PaintAppearanceLoadingDescription, new AppearanceObject[] { properties.AppearanceLoadingDescription, main }, DefaultAppearanceLoadingDescription);
			Size imageSize = properties.ShowImage ?
				args.Image.Size : Size.Empty;
			Size captionSize = properties.ShowCaption ?
				Size.Round(args.PaintAppearanceCaption.CalcTextSize(cache, args.Caption, 0)) : Size.Empty;
			Size descriptionSize = properties.ShowLoadingDescription ?
				Size.Round(args.PaintAppearanceLoadingDescription.CalcTextSize(cache, args.LoadingDescription, 0)) : Size.Empty;
			Size loadingIndicatorSize = properties.ShowLoadingDescription ?
				e.LoadingAnimator.GetMinSize() : Size.Empty;
			ImageLocation imgLocation = properties.ImageLocation;
			bool horz = (imgLocation == ImageLocation.BeforeText) || (imgLocation == ImageLocation.AfterText);
			int w = horz ?
				imageSize.Width + captionSize.Width + properties.ImageToCaptionDistance :
				Math.Max(imageSize.Width, captionSize.Width);
			int h = !horz ?
				imageSize.Height + captionSize.Height + properties.ImageToCaptionDistance :
				Math.Max(imageSize.Height, captionSize.Height);
			args.Content = PlacementHelper.Arrange(new Size(w, h), e.Bounds, ContentAlignment.MiddleCenter);
			args.Content = new Rectangle(new Point(args.Content.X, args.Content.Y - 60), args.Content.Size);
			ContentAlignment imageAlignment = horz ?
				(imgLocation == ImageLocation.AfterText ? ContentAlignment.MiddleRight : ContentAlignment.MiddleLeft) :
				(imgLocation == ImageLocation.BelowText ? ContentAlignment.BottomCenter : ContentAlignment.TopCenter);
			args.ImageBounds = PlacementHelper.Arrange(imageSize, args.Content, imageAlignment);
			ContentAlignment textAlignment = horz ?
				(imgLocation == ImageLocation.AfterText ? ContentAlignment.MiddleLeft : ContentAlignment.MiddleRight) :
				(imgLocation == ImageLocation.BelowText ? ContentAlignment.TopCenter : ContentAlignment.BottomCenter);
			args.CaptionBounds = PlacementHelper.Arrange(captionSize, args.Content, textAlignment);
			int interval = (descriptionSize.Width == 0) || (loadingIndicatorSize.Width == 0) ? 0 : 20;
			Size footerSize = new Size(
				descriptionSize.Width + loadingIndicatorSize.Width + interval,
				Math.Max(descriptionSize.Height, loadingIndicatorSize.Height));
			int top = args.Content.Bottom + properties.CaptionToLoadingElementsDistance.GetValueOrDefault();
			args.LoadingElements = PlacementHelper.Arrange(footerSize,
				new Rectangle(args.Bounds.Left, top, args.Bounds.Width, args.Bounds.Bottom - top),
				properties.CaptionToLoadingElementsDistance.HasValue ? ContentAlignment.TopCenter : ContentAlignment.MiddleCenter);
			args.LoadingElements = new Rectangle(new Point(args.LoadingElements.X, args.LoadingElements.Y + 80), args.LoadingElements.Size);
			args.LoadingIndicatorBounds = PlacementHelper.Arrange(loadingIndicatorSize, args.LoadingElements, ContentAlignment.MiddleLeft);
			args.LoadingDescriptionBounds = PlacementHelper.Arrange(descriptionSize, args.LoadingElements, ContentAlignment.MiddleRight);
			return args;
		}
		protected class SplashScreenInfoArgs {
			public Image Image { get; set; }
			public string Caption { get; set; }
			public string LoadingDescription { get; set; }
			public AppearanceObject PaintAppearance { get; set; }
			public AppearanceObject PaintAppearanceCaption { get; set; }
			public AppearanceObject PaintAppearanceLoadingDescription { get; set; }
			public Rectangle Bounds { get; set; }
			public Rectangle Content { get; set; }
			public Rectangle ImageBounds { get; set; }
			public Rectangle CaptionBounds { get; set; }
			public Rectangle LoadingElements { get; set; }
			public Rectangle LoadingDescriptionBounds { get; set; }
			public Rectangle LoadingIndicatorBounds { get; set; }
			public bool UseDefaultImage { get; set; }
		}
	}
	public class SplashScreenSkinPainter : SplashScreenPainter {
		ISkinProvider provider;
		public SplashScreenSkinPainter(ISkinProvider provider) {
			this.provider = provider;
		}
		protected override AppearanceDefault CreateDefaultAppearanceCaption() {
			AppearanceDefault appearance = base.CreateDefaultAppearanceCaption();
			Color backgroudColor = LookAndFeelHelper.GetSystemColorEx(provider, SystemColors.Control);
			Color foreColor = LookAndFeelHelper.GetSystemColorEx(provider, SystemColors.ControlText);
			appearance.ForeColor = foreColor;
			appearance.BackColor = backgroudColor;
			return appearance;
		}
		protected override AppearanceDefault CreateDefaultAppearanceLoadingDescription() {
			AppearanceDefault appearance = base.CreateDefaultAppearanceLoadingDescription();
			Color backgroudColor = LookAndFeelHelper.GetSystemColorEx(provider, SystemColors.Control);
			Color foreColor = LookAndFeelHelper.GetSystemColorEx(provider, SystemColors.ControlText);
			appearance.ForeColor = foreColor;
			appearance.BackColor = backgroudColor;
			return appearance;
		}
		protected override AppearanceDefault CreateDefaultAppearance() {
			AppearanceDefault appearance = base.CreateDefaultAppearance();
			Color backgroudColor = LookAndFeelHelper.GetSystemColorEx(provider, SystemColors.Control);
			Color foreColor = LookAndFeelHelper.GetSystemColorEx(provider, SystemColors.ControlText);
			appearance.ForeColor = foreColor;
			appearance.BackColor = backgroudColor;
			return appearance;
		}
		protected virtual Skin GetSkin() {
			return MetroUISkins.GetSkin(provider);
		}
		protected virtual SkinElement GetActionsBar() {
			return GetSkin()[MetroUISkins.SkinActionsBar];
		}
	}
}
