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
using DevExpress.Utils.Drawing;
using DevExpress.Utils.Drawing.Animation;
using DevExpress.Utils.Paint;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
namespace DevExpress.Utils {
	public class LoadingImageAnimationInfo : CustomAnimationInfo {
		AsyncImageLoader loader;
		public LoadingImageAnimationInfo(ISupportXtraAnimation obj, object animObj, int[] deltaTick, int frameCount)
			: base(obj, animObj, deltaTick, frameCount, null) {
			this.AnimationType = AnimationType.Cycle;
			this.loader = obj as AsyncImageLoader;
		}
		public override void FrameStep() {
			base.FrameStep();
			Invalidate();
		}
		protected void Invalidate() {
			loader.ForceRefreshAnimatedItems(AnimationId);
		}
	}
	public abstract class BaseImageLoader : IDisposable {
		public BaseImageLoader() {
		}
		public virtual void Clear() { }
		public abstract Image LoadImage(ImageLoadInfo info);
		protected virtual Image GetImageCore(int rowHandle) {
			return null;
		}
		public virtual void Dispose() { }
	}
	public enum ImageContentAnimationType { Expand, SegmentedFade, Slide, Push, None }
	public interface IAsyncImageLoaderClient {
		void AddAnimation(ImageLoadInfo info);
		void ForceItemRefresh(ImageLoadInfo info);
		ThumbnailImageEventArgs RaiseGetThumbnailImage(ThumbnailImageEventArgs e);
		Image RaiseGetLoadingImage(GetLoadingImageEventArgs e);
	}
	public interface IAsyncImageItemViewInfo {
		ImageLoadInfo ImageInfo { get; set; }
		Rectangle ImageContentBounds { get; }
	}
	public class AsyncImageLoader : BaseImageLoader, ISupportXtraAnimation {
		public AsyncImageLoader(IAsyncImageLoaderClient viewInfo)
			: base() {
			this.imagesToBeLoad = null;
			ViewInfo = viewInfo;
		}
		protected internal IAsyncImageLoaderClient ViewInfo { get; set; }
		protected virtual bool ShouldCacheThumbnails() {
			return false;
		}
		protected virtual bool ShouldLoadThumbnailImagesFromDataSource() {
			return false;
		}
		public virtual bool IsRowLoaded(int rowHandle) {
			return true;
		}
		public override Image LoadImage(ImageLoadInfo info) {
			info.LoadingStarted = false;
			if(info.ThumbImage != null || info.IsLoaded) return info.ThumbImage;
			if(!info.IsInAnimation) {
				if(ShouldCacheThumbnails()) {
					info.ThumbImage = GetCachedThumbnailImage(info.ImageInfoKey);
					if(info.ThumbImage != null) {
						RemoveImageFromAnimation(info);
						return info.ThumbImage;
					}
				}
				if(ShouldLoadThumbnailImagesFromDataSource()) {
					Image img = GetImageFromDataSource(info);
					if(img != null) { 
						lock(img)
							info.ThumbImage = img;
					}
					else 
						info.ThumbImage = img;
					if(info.ThumbImage != null) {
						RemoveImageFromAnimation(info);
						return info.ThumbImage;
					}
				}
			}
			if(!disposing) {
				info.IsInAnimation = true;
				AddInfoToLoadingList(info);
			}
			info.LoadingStarted = true;
			Image loadingImage = GetLoadingImage(info);
			CheckAnimationObject(info, loadingImage);
			return loadingImage;
		}
		private void CheckAnimationObject(ImageLoadInfo info, Image loadingImage) {
			if(info.AnimationObject != null) return;
			object animObj;
			if(LoadingImageCollection.TryGetValue(loadingImage, out animObj)) {
				info.AnimationObject = animObj;
				return;
			}
			info.AnimationObject = new object();
			AnimatedImageHelper helper = new AnimatedImageHelper(loadingImage);
			XtraAnimator.Current.AddAnimation(new LoadingImageAnimationInfo(this, info.AnimationObject, helper.AnimationIntervals, helper.FramesCount));
			LoadingImageCollection.Add(loadingImage, info.AnimationObject);
		}
		Dictionary<Image, object> LoadingImageCollection = new Dictionary<Image, object>();
		protected void RemoveImageFromAnimation(ImageLoadInfo info) {
			info.IsLoaded = true;
			info.IsInAnimation = false;
		}
		protected Image GetImageFromDataSource(ImageLoadInfo info) {
			return GetImageCore(info.DataSourceIndex);
		}
		protected internal void AddInfoToLoadingList(ImageLoadInfo info) {
			if(IsContains(info.ImageInfoKey)) return;
			ImagesToBeLoad.Add(info);
			StartLoadingThread();
		}
		protected void StartLoadingThread() {
			if(IsSuspended)
				return;
			if(ImagesToBeLoad.Count != 0 && !LoadingThread.IsBusy) LoadingThread.RunWorkerAsync();
		}
		protected bool IsContains(ImageInfoKeyBase key) {
			lock(locker) {
				for(int i = 0; i < ImagesToBeLoad.Count; i++)
					if(ImagesToBeLoad[i].ImageInfoKey == key) return true;
			}
			return false;
		}
		bool disposing = false;
		public void Suspend() {
			IsSuspended = true;
		}
		public void Resume() { 
			IsSuspended = false;
			StartLoadingThread();
		}
		bool IsSuspended { get; set; }
		void LoadingThread_DoWork(object sender, DoWorkEventArgs e) {
			while(true) {
				if(IsSuspended)
					continue;
				ImageLoadInfo info;
				lock(locker) {
					if(ImagesToBeLoad.Count == 0 || disposing) break;
					info = ImagesToBeLoad[0];
				}
				if(info.ThumbImage == null) {
					Image img;
					bool isRowLoaded = IsRowLoaded(info.RowHandle);
					bool shouldStartAnimation = ShouldStartAnimation(info, out img);
					if(img != null) {
						lock(img)
							info.ThumbImage = img;
					}
					else {
						info.ThumbImage = img;
						if(!isRowLoaded) continue;
					}
					info.IsLoaded = true;
					if(shouldStartAnimation) {
						info.IsInAnimation = true;
						info.LoadingStarted = false;
						ViewInfo.AddAnimation(info);
					}
					else {
						info.IsInAnimation = false;
						ViewInfo.ForceItemRefresh(info);
					}
				}
				lock(locker) { ImagesToBeLoad.Remove(info); }
			}
		}
		protected virtual ThumbnailImageEventArgs RaiseGetThumbnailImage(ImageLoadInfo info) {
			return ViewInfo.RaiseGetThumbnailImage(new ThumbnailImageEventArgs(info.DataSourceIndex, this, info));
		}
		protected virtual Image RaiseGetLoadingImage(ImageLoadInfo info) {
			return ViewInfo.RaiseGetLoadingImage(new GetLoadingImageEventArgs(info.DataSourceIndex));
		}
		protected virtual Image RaiseGetLoadingImage(string fieldName, int dataSourceIndex) {
			return ViewInfo.RaiseGetLoadingImage(new GetLoadingImageEventArgs(dataSourceIndex));
		}
		protected bool ShouldStartAnimation(ImageLoadInfo info, out Image thumbnailImage) {
			ThumbnailImageEventArgs e = RaiseGetThumbnailImage(info);
			thumbnailImage = e.ThumbnailImage;
			if(thumbnailImage != null) {
				if(ShouldCacheThumbnails()) CacheImage(info.ImageInfoKey, thumbnailImage);
				return true;
			}
			info.Image = GetImageCore(info.RowHandle);
			if(info.Image == null) {
				return false;
			}
			thumbnailImage = CreateThumbCore(info.Image, info.DesiredThumbnailSize);
			if(ShouldCacheThumbnails()) CacheImage(info.ImageInfoKey, thumbnailImage);
			info.Image = null;
			return true;
		}
		protected void CacheImage(ImageInfoKeyBase key, Image thumbnailImage) {
			if(ThumbnailImages.ContainsKey(key)) return;
			ThumbnailImages.Add(key, thumbnailImage);
		}
		protected internal void ResetImageCache() {
			ThumbnailImages.Clear();
		}
		protected internal Image CreateThumbCore(Image img, Size size) {
			try {
				Rectangle rect = ImageLayoutHelper.GetImageBounds(new Rectangle(Point.Empty, size), img.Size, ImageLayoutMode.ZoomInside);
				if(rect.Width == 0 || rect.Height == 0) return null;
				rect = new Rectangle(Point.Empty, rect.Size);
				Image res = new Bitmap(rect.Width, rect.Height);
				using(Graphics g = Graphics.FromImage(res)) {
					g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;
					g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
					g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
					g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
					g.Clear(Color.Transparent);
					g.DrawImage(img, rect, new Rectangle(0, 0, img.Width, img.Height), GraphicsUnit.Pixel);
					return res;
				}
			}
			catch {
				return null;
			}
		}
		protected Image GetCachedThumbnailImage(ImageInfoKeyBase key) {
			Image img;
			if(!ThumbnailImages.TryGetValue(key, out img)) return null;
			return img;
		}
		Dictionary<ImageInfoKeyBase, Image> thumbnailImages;
		Dictionary<ImageInfoKeyBase, Image> ThumbnailImages {
			get {
				if(thumbnailImages == null)
					thumbnailImages = new Dictionary<ImageInfoKeyBase, Image>();
				return thumbnailImages;
			}
		}
		List<ImageLoadInfo> imagesToBeLoad;
		public List<ImageLoadInfo> ImagesToBeLoad {
			get {
				if(imagesToBeLoad == null)
					imagesToBeLoad = new List<ImageLoadInfo>();
				return imagesToBeLoad;
			}
		}
		BackgroundWorker loadingThread;
		public BackgroundWorker LoadingThread {
			get {
				if(loadingThread == null) {
					loadingThread = new BackgroundWorker();
					loadingThread.DoWork += LoadingThread_DoWork;
					loadingThread.RunWorkerCompleted += LoadingThread_RunWorkerCompleted;
				}
				return loadingThread;
			}
		}
		protected void LoadingThread_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e) {
			if(!IsSuspended)
				StartLoadingThread();
		}
		public Image GetLoadingImage(ImageLoadInfo info) {
			if(info.LoadingImage != null) return info.LoadingImage;
			Image img = RaiseGetLoadingImage(info);
			if(img != null) info.LoadingImage = img;
			else info.LoadingImage = CommonSkins.GetSkin(UserLookAndFeel.Default.ActiveLookAndFeel)[CommonSkins.SkinLoadingBig].Image.Image;
			return info.LoadingImage;
		}
		public Image GetLoadingImage(int rowHandle, string fieldName) {
			Image img = RaiseGetLoadingImage(fieldName, rowHandle);
			if(img != null) return img;
			return CommonSkins.GetSkin(UserLookAndFeel.Default.ActiveLookAndFeel)[CommonSkins.SkinLoadingBig].Image.Image;
		}
		public void ForceRefreshAnimatedItems(object animationId) {
			lock(locker) {
				for(int i = 0; i < ImagesToBeLoad.Count; i++) {
					if(ImagesToBeLoad[i].AnimationObject == animationId)
						ViewInfo.ForceItemRefresh(ImagesToBeLoad[i]);
				}
			}
		}
		object locker = new object();
		public void BreakLoading() {
			this.disposing = true;
			try {
				lock(locker) { ImagesToBeLoad.Clear(); }
			}
			finally { this.disposing = false; }
		}
		public override void Clear() {
			this.disposing = true;
			try {
				LoadingThread.Dispose();
				lock(locker) { ImagesToBeLoad.Clear(); }
				ThumbnailImages.Clear();
			}
			finally { this.disposing = false; }
		}
		public override void Dispose() {
			Clear();
			XtraAnimator.Current.Animations.Remove(this);
			this.imagesToBeLoad = null;
		}
		public System.Windows.Forms.Control OwnerControl { get { return null; } }
		public bool CanAnimate { get { return true; } }
	}
	public class SyncImageLoader : BaseImageLoader {
		public SyncImageLoader(IAsyncImageLoaderClient viewInfo) : base() {
			ViewInfo = viewInfo;
		}
		protected internal IAsyncImageLoaderClient ViewInfo { get; set; }
		public override Image LoadImage(ImageLoadInfo info) {
			return GetImageCore(info.RowHandle);
		}
	}
	public class ImageShowingBoundsAnimationInfo : BaseAnimationInfo {
		public ImageShowingBoundsAnimationInfo(ISupportXtraAnimation anim, object animationId, RenderImageViewInfo imageInfo, int ms)
			: base(anim, animationId, 10, (int)(TimeSpan.TicksPerMillisecond * ms / 10)) {
			Item = imageInfo;
			EndBounds = imageInfo.Bounds;
			StartBounds = RectangleFToRectangle(Item.RenderImageBounds);
		}
		Rectangle RectangleFToRectangle(RectangleF input) {
			return new Rectangle((int)input.X, (int)input.Y, (int)input.Width, (int)input.Height);
		}
		public RenderImageViewInfo Item { get; private set; }
		public Rectangle EndBounds { get; private set; }
		public Rectangle StartBounds { get; private set; }
		public override void FrameStep() {
			FrameStepCore();
			Invalidate();
			if(IsFinalFrame)
				OnAnimationComplete();
		}
		protected virtual void Invalidate() {
		}
		protected virtual void FrameStepCore() {
			float k = ((float)(CurrentFrame)) / FrameCount;
			if(IsFinalFrame) k = 1.0f;
			Rectangle rect = Rectangle.Empty;
			rect.X = StartBounds.X + (int)(k * (EndBounds.X - StartBounds.X));
			rect.Y = StartBounds.Y + (int)(k * (EndBounds.Y - StartBounds.Y));
			rect.Width = StartBounds.Width + (int)(k * (EndBounds.Width - StartBounds.Width));
			rect.Height = StartBounds.Height + (int)(k * (EndBounds.Height - StartBounds.Height));
			Item.RenderImageBounds = rect;
		}
		protected virtual void OnAnimationComplete() {
		}
	}
	public class ImageShowingBoundsSplineAnimationInfo : ImageShowingBoundsAnimationInfo {
		public ImageShowingBoundsSplineAnimationInfo(ISupportXtraAnimation anim, object animationId, RenderImageViewInfo imageInfo, int ms)
			: base(anim, animationId, imageInfo, ms) {
			SplineHelperX = new SplineAnimationHelper();
			SplineHelperY = new SplineAnimationHelper();
			SplineHelperWidth = new SplineAnimationHelper();
			SplineHelperHeight = new SplineAnimationHelper();
			SplineHelperX.Init(StartBounds.X, EndBounds.X, 1.0f);
			SplineHelperY.Init(StartBounds.Y, EndBounds.Y, 1.0f);
			SplineHelperWidth.Init(StartBounds.Width, EndBounds.Width, 1.0f);
			SplineHelperHeight.Init(StartBounds.Height, EndBounds.Height, 1.0f);
		}
		protected SplineAnimationHelper SplineHelperX { get; private set; }
		protected SplineAnimationHelper SplineHelperY { get; private set; }
		protected SplineAnimationHelper SplineHelperWidth { get; private set; }
		protected SplineAnimationHelper SplineHelperHeight { get; private set; }
		protected override void FrameStepCore() {
			float k = ((float)(CurrentFrame)) / FrameCount;
			if(IsFinalFrame) k = 1.0f;
			Rectangle rect = new Rectangle();
			rect.X = (int)SplineHelperX.CalcSpline(k);
			rect.Y = (int)SplineHelperY.CalcSpline(k);
			rect.Width = (int)SplineHelperWidth.CalcSpline(k);
			rect.Height = (int)SplineHelperHeight.CalcSpline(k);
			Item.RenderImageBounds = rect;
		}
	}
	public class ImageShowingAnimationInfo : ImageShowingBoundsSplineAnimationInfo {
		public ImageShowingAnimationInfo(ISupportXtraAnimation anim, object animationId, RenderImageViewInfo imageInfo, int ms, int delay)
			: base(anim, animationId, imageInfo, ms) {
			Item.Opacity = 0.0f;
			Item.SplineCoefficient = 0.0f;
			AnimationHelper = new SplineAnimationHelper();
			AnimationHelper.Init(0, 1, 1.0f);
			FirstAnimationFrame = (ms == 0) ? FrameCount : delay * FrameCount / ms;
			if(Item.LoadInfo.AnimationType == ImageContentAnimationType.SegmentedFade)
				Item.LoadInfo.GenerateRandomSegments();
			forcedLastFrame = false;
		}
		protected SplineAnimationHelper AnimationHelper { get; private set; }
		protected int FirstAnimationFrame { get; set; }
		bool forcedLastFrame;
		protected override void FrameStepCore() {
			float k = ((float)(CurrentFrame - FirstAnimationFrame)) / (FrameCount - FirstAnimationFrame);
			if(k < 0) return;
			if(k > 0) {
				Item.LoadInfo.IsLoaded = true;
				if(!forcedLastFrame && Item.LoadInfo.AnimationType == ImageContentAnimationType.None) {
					forcedLastFrame = true;
					ForceLastFrameStep();
					return;
				}
			}
			if(IsFinalFrame) k = 1.0f;
			Item.Opacity = k;
			Item.SplineCoefficient = (float)AnimationHelper.CalcSpline(k);
			RectangleF rect = new Rectangle();
			if(Item.LoadInfo.AnimationType == ImageContentAnimationType.Slide) {
				Rectangle startBounds = EndBounds;
				startBounds.X += ImageShowingAnimationHelper.OutsideWidth * Item.LoadInfo.AnimatedRegion.Width / Item.LoadInfo.ImageMaxSize.Width;
				startBounds.Y += ImageShowingAnimationHelper.OutsideHeight * Item.LoadInfo.AnimatedRegion.Height / Item.LoadInfo.ImageMaxSize.Height;
				k = (float)AnimationHelper.CalcSpline(k);
				double dx = k * (EndBounds.X - startBounds.X);
				double ky = (EndBounds.Y - startBounds.Y) / Math.Sqrt(Math.Abs(EndBounds.X - startBounds.X));
				rect.X = (float)(startBounds.X + dx);
				rect.Y = (float)(startBounds.Y + ky * Math.Sqrt(Math.Abs(dx)));
				rect.Width = startBounds.Width + (float)((EndBounds.Width - startBounds.Width) * k);
				rect.Height = startBounds.Height + (float)((EndBounds.Height - startBounds.Height) * k);
			}
			else if(Item.LoadInfo.AnimationType == ImageContentAnimationType.Push) {
				Item.SplineCoefficient = Math.Max(0f, (Item.SplineCoefficient - 0.5f)) * 2;
				k = (float)AnimationHelper.CalcSpline(k);
				rect = CalcPushAnimationRectangle(rect, k);
			}
			else {
				rect.X = (float)SplineHelperX.CalcSpline(k);
				rect.Width = (float)SplineHelperWidth.CalcSpline(k);
				rect.Height = (float)SplineHelperHeight.CalcSpline(k);
				rect.Y = (float)SplineHelperY.CalcSpline(k);
			}
			Item.RenderImageBounds = rect;
		}
		protected virtual RectangleF CalcPushAnimationRectangle(RectangleF rect, float spline) {
			rect.X = EndBounds.X;
			if(spline < 0.9) {
				rect.Y = (-ImageShowingAnimationHelper.EasyInAnimationOdds - ImageShowingAnimationHelper.EasyOutAnimationOdds - EndBounds.Height) / 0.9f * spline + EndBounds.Height + 10;
			}
			else rect.Y = Math.Min(10 * ImageShowingAnimationHelper.EasyOutAnimationOdds * spline - 10 * ImageShowingAnimationHelper.EasyOutAnimationOdds, 0);
			rect.Width = EndBounds.Width;
			rect.Height = EndBounds.Height;
			return rect;
		}
		protected override void OnAnimationComplete() {
			Item.SplineCoefficient = 1.0f;
			Item.LoadInfo.IsAnimationEnd = true;
			base.OnAnimationComplete();
		}
	}
	public static class ImageShowingAnimationHelper {
		const int outsideHeight = 8;
		const int outsideWidth = 25;
		const float minSizeXOdds = 0.7f;
		const float minSizeYOdds = 0.7f;
		const int easyInAnimationOdds = 10;
		const int easyOutAnimationOdds = 10;
		public static int OutsideHeight { get { return outsideHeight; } }
		public static int OutsideWidth { get { return outsideWidth; } }
		public static float MinSizeXOdds { get { return minSizeXOdds; } }
		public static float MinSizeYOdds { get { return minSizeYOdds; } }
		public static int EasyInAnimationOdds { get { return easyInAnimationOdds; } }
		public static int EasyOutAnimationOdds { get { return easyInAnimationOdds; } }
	}
	public class ImageLoadInfo {
		Image thumbImage, loadingImage;
		int dataSourceIndex, rowHandle;
		Size desiredThumbnailSize;
		ImageLayoutMode imageLayoutMode;
		ImageInfoKeyBase imageInfoKey;
		Color backColor;
		public ImageLoadInfo(int dataSourceIndex, int rowHandle, ImageContentAnimationType animationType, ImageLayoutMode mode, Size maxSize, Size desiredSize) {
			IsLoaded = IsInAnimation = IsAnimationEnd = LoadingStarted = false;
			this.thumbImage = null;
			this.dataSourceIndex = dataSourceIndex;
			this.rowHandle = rowHandle;
			this.imageLayoutMode = mode;
			this.desiredThumbnailSize = desiredSize;
			this.backColor = Color.Empty;
			AnimationType = animationType;
			ImageMaxSize = maxSize;
		}
		public bool LoadingStarted { get; set; }
		public object AnimationObject { get; set; }
		public Image Image { get; set; }
		public Size ThumbSize { get; set; }
		public Image ThumbImage {
			get { return thumbImage; }
			set {
				thumbImage = value;
				InitRenderImageInfo();
			}
		}
		public void InitRenderImageInfo() {
			if(ThumbImage == null)
				return;
			lock(ThumbImage) {
				ThumbSize = ThumbImage.Size;
			}
			Size animatedSize = ImageAnimationHelper.CalcAnimatedRegion(ThumbSize, ImageLayoutMode, ImageMaxSize);
			RenderImageInfo = new RenderImageViewInfo(animatedSize, this);
		}
		public ImageInfoKeyBase ImageInfoKey {
			get {
				if(imageInfoKey == null) {
					imageInfoKey = CreateImageInfoKey();
				}
				return imageInfoKey;
			}
		}
		public virtual ImageInfoKeyBase CreateImageInfoKey() {
			return new ImageInfoKeyBase() { RowHandle = DataSourceIndex };
		}
		public Image LoadingImage { get { return loadingImage; } set { loadingImage = value; } }
		public ImageContentAnimationType AnimationType { get; set; }
		public ImageLayoutMode ImageLayoutMode { get { return imageLayoutMode; } set { imageLayoutMode = value; } }
		public Size DesiredThumbnailSize { get { return desiredThumbnailSize; } set { desiredThumbnailSize = value; } }
		public object InfoId = new object();
		public Size ImageMaxSize { get; set; }
		public int DataSourceIndex { get { return dataSourceIndex; } }
		public int RowHandle { get { return rowHandle; } }
		public bool IsLoaded { get; set; }
		public bool IsInAnimation { get; set; }
		public bool IsAnimationEnd { get; set; }
		public RenderImageViewInfo RenderImageInfo { get; set; }
		public Image GetThumbImage() {
			return ThumbImage;
		}
		public List<Point> RandomSegments { get; set; }
		public virtual void GenerateRandomSegments() {
			List<Point> list = new List<Point>();
			RandomSegments = new List<Point>(SegmentColumn * SegmentRow);
			for(int i = 0; i < SegmentColumn; i++) {
				for(int j = 0; j < SegmentRow; j++) {
					list.Add(new Point(i, j));
				}
			}
			Random r = new Random();
			while(list.Count > 0) {
				int t = r.Next(list.Count);
				RandomSegments.Add(list[t]);
				list.RemoveAt(t);
			}
		}
		const int segmentCountInRow = 5;
		const int segmentCountInColumn = 5;
		public Size ImageSize {
			get {
				if(ThumbImage == null) return Size.Empty;
				float k = CalcBoundsMultiplier();
				return new Size((int)(AnimatedRegion.Width * k), (int)(AnimatedRegion.Height * k));
			}
		}
		public RectangleF RenderImageBounds {
			get {
				if(RenderImageInfo == null) return Rectangle.Empty;
				RectangleF rect = RenderImageInfo.RenderImageBounds;
				float k = CalcBoundsMultiplier();
				return new RectangleF(rect.X * k, rect.Y * k, rect.Width * k, rect.Height * k);
			}
		}
		public Size AnimatedRegion {
			get {
				if(ThumbImage == null) return Size.Empty;
				return ImageAnimationHelper.CalcAnimatedRegion(ThumbSize, ImageLayoutMode, ImageMaxSize);
			}
		}
		protected internal float CalcBoundsMultiplier() {
			float k = Math.Max((float)ThumbSize.Width / ImageMaxSize.Width, (float)ThumbSize.Height / ImageMaxSize.Height);
			return 1 / k;
		}
		public SizeF SegmentSize { get { return new SizeF((float)AnimatedRegion.Width / segmentCountInRow, (float)AnimatedRegion.Height / segmentCountInColumn); } }
		public int SegmentRow { get { return segmentCountInRow; } }
		public int SegmentColumn { get { return segmentCountInColumn; } }
		public Color BackColor { get { return backColor; } set { backColor = value; } }
		public bool IsLoadingImage { get { return (!IsLoaded || ImageSize == Size.Empty) && LoadingStarted; } }
	}
	public class RenderImageViewInfo {
		public RenderImageViewInfo(Size imageSize, ImageLoadInfo info) {
			LoadInfo = info;
			Bounds = new Rectangle(Point.Empty, imageSize);
			RenderImageBounds = CalcStartRenderImageBounds(imageSize);
			SplineOpacityHelper = new SplineAnimationHelper();
			SplineOpacityHelper.Init(0, 1, 1.0f);
		}
		public Size AnimatedSize { get; set; }
		protected RectangleF CalcStartRenderImageBounds(Size imageSize) {
			Size minSize = new Size((int)(imageSize.Width * ImageShowingAnimationHelper.MinSizeXOdds), (int)(imageSize.Height * ImageShowingAnimationHelper.MinSizeYOdds));
			return new Rectangle((imageSize.Width - minSize.Width) / 2, (imageSize.Height - minSize.Height) / 2, minSize.Width, minSize.Height);
		}
		protected internal SplineAnimationHelper SplineOpacityHelper { get; set; }
		public ImageLoadInfo LoadInfo { get; set; }
		public Rectangle Bounds { get; set; }
		[Obsolete("Use SplineCoefficient"), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public float SplineKoefficient { get { return SplineCoefficient; } set { SplineCoefficient = value; } }
		public float SplineCoefficient { get; set; }
		public RectangleF RenderImageBounds { get; set; }
		public float Opacity { get; set; }
	}
	public static class ImageAnimationHelper {
		public static Size CalcAnimatedRegion(Size size, ImageLayoutMode mode, Size maxSize) {
			if(mode == ImageLayoutMode.Stretch) return maxSize;
			float kx = ((float)size.Width) / maxSize.Width;
			float ky = ((float)size.Height) / maxSize.Height;
			float k = Math.Max(kx, ky);
			if(mode == ImageLayoutMode.Squeeze) {
				if(kx < 1 && ky < 1) return size;
				return new Size((int)(size.Width / k), (int)(size.Height / k));
			}
			if(mode == ImageLayoutMode.Default || mode == ImageLayoutMode.ZoomInside)
				return new Size((int)(size.Width / k), (int)(size.Height / k));
			if(mode == ImageLayoutMode.StretchHorizontal)
				return new Size((int)(size.Width / kx), size.Height);
			if(mode == ImageLayoutMode.StretchVertical)
				return new Size(size.Width, (int)(size.Height / ky));
			if(mode == ImageLayoutMode.ZoomOutside) {
				float kMin = Math.Min(kx, ky);
				Size minSize = new Size((int)(size.Width / kMin), (int)(size.Height / kMin));
				return new Size(Math.Min(minSize.Width, maxSize.Width), Math.Min(minSize.Height, maxSize.Height));
			}
			return new Size(Math.Min(size.Width, maxSize.Width), Math.Min(size.Height, maxSize.Height));
		}
		public static Rectangle CalcImageVisibleRegion(Size contentSize, Size imageSize, ImageLayoutMode mode) {
			Rectangle rect = new Rectangle();
			if(mode == ImageLayoutMode.Default || mode == ImageLayoutMode.ZoomInside || mode == ImageLayoutMode.Squeeze ||
				mode == ImageLayoutMode.Stretch || mode == ImageLayoutMode.StretchHorizontal || mode == ImageLayoutMode.StretchVertical) {
				return new Rectangle(0, 0, imageSize.Width, imageSize.Height);
			}
			if(mode == ImageLayoutMode.ZoomOutside) {
				float kx = ((float)imageSize.Width) / contentSize.Width;
				float ky = ((float)imageSize.Height) / contentSize.Height;
				float k = Math.Min(kx, ky);
				float imageDisplayWidth = imageSize.Width / k;
				float imageDisplayHeight = imageSize.Height / k;
				float x = (imageDisplayWidth - contentSize.Width) / 2.0f;
				float y = (imageDisplayHeight - contentSize.Height) / 2.0f;
				float imageX = x * k;
				float imageY = y * k;
				float imageWidth = Math.Min(imageDisplayWidth, contentSize.Width) * k;
				float imageHeight = Math.Min(imageDisplayHeight, contentSize.Height) * k;
				return new Rectangle((int)imageX, (int)imageY, (int)imageWidth, (int)imageHeight);
			}
			if(mode == ImageLayoutMode.BottomCenter || mode == ImageLayoutMode.BottomLeft || mode == ImageLayoutMode.BottomRight) {
				rect.Y = imageSize.Height - contentSize.Height;
				rect.Height = contentSize.Height;
			}
			if(mode == ImageLayoutMode.BottomRight || mode == ImageLayoutMode.MiddleRight || mode == ImageLayoutMode.TopRight) {
				rect.X = imageSize.Width - contentSize.Width;
				rect.Width = contentSize.Width;
			}
			if(mode == ImageLayoutMode.TopLeft || mode == ImageLayoutMode.TopCenter || mode == ImageLayoutMode.TopRight) {
				rect.Y = 0;
				rect.Height = contentSize.Height;
			}
			if(mode == ImageLayoutMode.BottomLeft || mode == ImageLayoutMode.MiddleLeft || mode == ImageLayoutMode.TopLeft) {
				rect.X = 0;
				rect.Width = contentSize.Width;
			}
			if(mode == ImageLayoutMode.TopCenter || mode == ImageLayoutMode.BottomCenter || mode == ImageLayoutMode.MiddleCenter) {
				rect.X = (imageSize.Width - contentSize.Width) / 2;
				rect.Width = contentSize.Width;
			}
			if(mode == ImageLayoutMode.MiddleCenter || mode == ImageLayoutMode.MiddleLeft || mode == ImageLayoutMode.MiddleRight) {
				rect.Y = (imageSize.Height - contentSize.Height) / 2;
				rect.Height = contentSize.Height;
			}
			return rect;
		}
		public static Rectangle CalcLoadingImageRectangle(Rectangle contentRect, Size imageSize) {
			float k = Math.Max(imageSize.Width / contentRect.Width, imageSize.Height / contentRect.Height);
			if(k > 1) imageSize = new Size((int)(imageSize.Width / k), (int)(imageSize.Height / k));
			int x = contentRect.X + (contentRect.Width - imageSize.Width) / 2;
			int y = contentRect.Y + (contentRect.Height - imageSize.Height) / 2;
			return new Rectangle(new Point(x, y), imageSize);
		}
	}
	public static class ImageLoaderPaintHelper {
		static ImageAttributes GetImageAttributes(bool enabled, float opacity){
			if(enabled) {
				ImageAttributes attr = new ImageAttributes();
				attr.SetColorMatrix(GetOpacityMatrix(opacity));
				return attr;
			}
			return XPaint.GetDisabledImageAttrWithOpacity(opacity);
		}
		public static void DrawRenderImage(Graphics g, IAsyncImageItemViewInfo itemInfo, Color backColor, bool enabled) {
			DrawRenderImage(g, itemInfo.ImageInfo, itemInfo.ImageContentBounds, backColor, enabled);
		}
		public static void DrawRenderImage(Graphics g, ImageLoadInfo info, RectangleF contentBounds, Color backColor, bool enabled) {
			float opacity = info.IsInAnimation ? info.RenderImageInfo.SplineCoefficient : 1;
			ImageAttributes attr = GetImageAttributes(enabled, opacity);
			Rectangle srcRect = ImageAnimationHelper.CalcImageVisibleRegion(info.RenderImageInfo.Bounds.Size, info.ThumbSize, info.ImageLayoutMode);
			PointF[] destRect = CreateRectangle(info.RenderImageInfo.Bounds, contentBounds);
			if(!info.IsInAnimation) {
				g.DrawImage(info.ThumbImage, destRect, srcRect, GraphicsUnit.Pixel, attr);
				return;
			}
			if(info.AnimationType == ImageContentAnimationType.SegmentedFade) {
				DrawSegmentedImage(g, destRect, srcRect, attr, info, backColor);
			}
			else {
				DrawRenderImageCore(g, contentBounds, srcRect, attr, info);
			}
		}
		static Random r = new Random();
		static void DrawRenderImageCore(Graphics g, RectangleF content, Rectangle srcRect, ImageAttributes attr, ImageLoadInfo info) {
			if(info.ThumbImage == null) return;
			PointF[] destRect = CreateRectangle(info.RenderImageInfo.RenderImageBounds, content);
			g.DrawImage(info.ThumbImage, destRect, srcRect, GraphicsUnit.Pixel, attr);
		}
		static PointF[] CreateRectangle(RectangleF rect, RectangleF content) {
			PointF[] points = new PointF[3];
			points[0] = new PointF(content.X + rect.X, content.Y + rect.Y);
			points[1] = new PointF(content.X + rect.X + rect.Width, content.Y + rect.Y);
			points[2] = new PointF(content.X + rect.X, content.Y + rect.Y + rect.Height);
			return points;
		}
		static void DrawSegmentedImage(Graphics g, PointF[] content, Rectangle srcRect, ImageAttributes attr, ImageLoadInfo info, Color backColor) {
			if(info.RandomSegments == null || info.ThumbImage == null) return;
			Size destSize = info.ImageSize;
			g.DrawImage(info.ThumbImage, content, srcRect, GraphicsUnit.Pixel, attr);
			for(int i = 0; i < info.RandomSegments.Count; i++) {
				DrawSegment(g, content, attr, info.RandomSegments[i].X, info.RandomSegments[i].Y, i, info, backColor);
			}
		}
		const float maxOpacityOdds = 0.5f;
		static void DrawSegment(Graphics g, PointF[] content, ImageAttributes attr, int x, int y, int index, ImageLoadInfo info, Color backColor) {
			float startOpacityOdds = (float)index / (info.SegmentColumn * info.SegmentRow);
			float opacity = Math.Min(1.0f, Math.Max(0, info.RenderImageInfo.Opacity * (1 + maxOpacityOdds) - startOpacityOdds * maxOpacityOdds));
			float splineOpacity = 1.0f - (float)info.RenderImageInfo.SplineOpacityHelper.CalcSpline(opacity);
			if(splineOpacity == 0.0f) return;
			attr.SetColorMatrix(GetOpacityMatrix(splineOpacity));
			RectangleF rect = new RectangleF(content[0].X + x * info.SegmentSize.Width, content[0].Y + y * info.SegmentSize.Height, info.SegmentSize.Width, info.SegmentSize.Height);
			Brush brush = new SolidBrush(Color.FromArgb((int)(splineOpacity * 255), backColor));
			g.FillRectangle(brush, rect.X, rect.Y, info.SegmentSize.Width, info.SegmentSize.Height);
		}
		static ColorMatrix opacityMatrix;
		static ColorMatrix GetOpacityMatrix(float opacity) {
			if(opacityMatrix == null)
				opacityMatrix = new ColorMatrix();
			opacityMatrix.Matrix33 = opacity;
			return opacityMatrix;
		}
	}
	public delegate void ThumbnailImageEventHandler(object sender, ThumbnailImageEventArgs e);
	public class ThumbnailImageEventArgs : EventArgs {
		int dataSourceIndex;
		Image thumbnailImage;
		AsyncImageLoader loader;
		ImageLoadInfo info;
		public ThumbnailImageEventArgs(int dataSourceIndex, AsyncImageLoader loader, ImageLoadInfo info) {
			this.dataSourceIndex = dataSourceIndex;
			this.thumbnailImage = null;
			this.loader = loader;
			this.info = info;
		}
		public int DataSourceIndex { get { return dataSourceIndex; } }
		public Image ThumbnailImage { get { return thumbnailImage; } set { thumbnailImage = value; } }
		public Size DesiredThumbnailSize { get { return info.DesiredThumbnailSize; } }
		public Image CreateThumbnailImage(Image img) {
			return loader.CreateThumbCore(img, info.DesiredThumbnailSize);
		}
		public Image CreateThumbnailImage(Image img, Size size) {
			return loader.CreateThumbCore(img, size);
		}
		public void ResetImageCache() { loader.ResetImageCache(); }
	}
	public delegate void GetLoadingImageEventHandler(object sender, GetLoadingImageEventArgs e);
	public class GetLoadingImageEventArgs : EventArgs {
		Image loadingImage;
		int dataSourceIndex;
		public GetLoadingImageEventArgs(int dataSourceIndex) {
			this.loadingImage = null;
			this.dataSourceIndex = dataSourceIndex;
		}
		public int DataSourceIndex { get { return dataSourceIndex; } }
		public Image LoadingImage { get { return loadingImage; } set { loadingImage = value; } }
	}
	public class ImageInfoKeyBase {
		public int RowHandle { get; set; }
		public override bool Equals(object obj) {
			ImageInfoKeyBase key = (ImageInfoKeyBase)obj;
			if(key == null) return false;
			return key.RowHandle == RowHandle;
		}
		public override int GetHashCode() {
			return RowHandle;
		}
	}
}
