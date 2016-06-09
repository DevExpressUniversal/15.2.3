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
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using DevExpress.Skins;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.Utils.Drawing.Animation;
using DevExpress.XtraEditors.Drawing;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.Utils.Design;
using DevExpress.XtraEditors.ToolboxIcons;
namespace DevExpress.XtraEditors.Controls {
	#region EventArgs & Delegates
	public class ImageSliderCurrentImageIndexChangedEventArgs : EventArgs {
		int prevIndex, currentIndex;
		public ImageSliderCurrentImageIndexChangedEventArgs(int prevIndex, int currentIndex) {
			this.prevIndex = prevIndex;
			this.currentIndex = currentIndex;
		}
		public int PrevIndex { get { return prevIndex; } }
		public int CurrentIndex { get { return currentIndex; } }
	}
	public delegate void ImageSliderCurrentImageIndexChangedEventHandler(object sender, ImageSliderCurrentImageIndexChangedEventArgs e);
	#endregion
	public class BaseSliderPainter : BaseControlPainter {
		protected override void DrawContent(ControlGraphicsInfoArgs info) {
			base.DrawContent(info);
			DrawItems(info);
			DrawArrows(info);
		}
		protected virtual void DrawArrows(ControlGraphicsInfoArgs info) {
			BaseSliderViewInfo sliderInfo = (BaseSliderViewInfo)info.ViewInfo;
			SkinElementInfo left = sliderInfo.GetLeftArrowInfo();
			ColorMatrix m = new ColorMatrix();
			m.Matrix33 = sliderInfo.LeftArrowCurrentOpacity;
			left.Attributes = new ImageAttributes();
			left.Attributes.SetColorMatrix(m);
			ObjectPainter.DrawObject(info.Cache, SkinElementPainter.Default, left);
			SkinElementInfo right = sliderInfo.GetRightArrowInfo();
			m.Matrix33 = sliderInfo.RightArrowCurrentOpacity;
			right.Attributes = new ImageAttributes();
			right.Attributes.SetColorMatrix(m);
			ObjectPainter.DrawObject(info.Cache, SkinElementPainter.Default, right);
		}
		protected virtual void DrawItems(ControlGraphicsInfoArgs info) {
		}
	}
	public class ImageSliderPainter : BaseSliderPainter {
		protected override void DrawItems(ControlGraphicsInfoArgs info) {
			base.DrawItems(info);
			ImageSliderViewInfo viewInfo = (ImageSliderViewInfo)info.ViewInfo;
			if(viewInfo.ShouldDrawCurrentImage) {
				if(viewInfo.ShouldUseDisabledPainter)				
					info.Cache.Paint.DrawImage(info.Graphics, viewInfo.CurrentImage, viewInfo.CurrentImageBounds, new Rectangle(Point.Empty, viewInfo.CurrentImage.Size), false);
				else
					info.Graphics.DrawImage(viewInfo.CurrentImage, viewInfo.CurrentImageBounds);	  
			}
			if(viewInfo.ShouldDrawNextImage) {
				info.Graphics.DrawImage(viewInfo.ImageSlider.NextImage, viewInfo.NextImageBounds);
			}
			DrawContextButtons(info);
		}
		protected virtual void DrawContextButtons(ControlGraphicsInfoArgs info) {
			ImageSliderViewInfo vi = (ImageSliderViewInfo)info.ViewInfo;
			new ContextItemCollectionPainter().Draw(new ContextItemCollectionInfoArgs(vi.ContextButtonsViewInfo, info.Cache, info.Bounds));
		}
	}
	public class BaseSliderViewInfo : BaseStyleControlViewInfo {
		public BaseSliderViewInfo(SliderBase slider) : base(slider) {
			LeftArrowState = ObjectState.Normal;
			RightArrowState = ObjectState.Normal;
		}
		public ObjectState LeftArrowState {
			get;
			protected internal set;
		}
		public ObjectState RightArrowState {
			get;
			protected internal set;
		}
		protected virtual ObjectState GetLeftButtonState(Point point, MouseButtons buttons) {
			if(!Slider.HasPrevItem)
				return ObjectState.Disabled;
			return GetButtonState(point, LeftArrowOpacity, LeftArrowBounds, buttons);
		}
		protected virtual ObjectState GetRightButtonState(Point point, MouseButtons buttons) {
			if(!Slider.HasNextItem)
				return ObjectState.Disabled;
			return GetButtonState(point, RightArrowOpacity, RightArrowBounds, buttons);
		}
		protected virtual ObjectState GetButtonState(Point point, float opacity, Rectangle bounds, MouseButtons buttons) {
			return opacity > 0.0f && bounds.Contains(point) && buttons == MouseButtons.Left ? ObjectState.Pressed : ObjectState.Normal;
		}
		protected internal virtual void UpdateButtonsState(Point point, MouseButtons buttons) {
			LeftArrowState = GetLeftButtonState(point, buttons);
			RightArrowState = GetRightButtonState(point, buttons);
		}
		public Rectangle NextItemStartBounds {
			get {
				return GetNextItemBoundsCore();
			}
		}
		protected virtual Rectangle GetNextItemBoundsCore() {
			Rectangle rect = Bounds;
			rect.X += rect.Width;
			rect.Offset(OffsetPoint.X, 0);
			return rect;
		}
		public Rectangle NextItemStartBoundsWithoutOffset {
			get {
				Rectangle rect = Bounds;
				rect.X += rect.Width;
				return rect;
			}
		}
		public Rectangle NextItemEndBounds {
			get {
				return Bounds;
			}
		}
		public Rectangle CurrentItemEndBounds {
			get {
				return GetCurrentItemBoundsCore();
			}
		}
		protected virtual Rectangle GetCurrentItemBoundsCore() {
			Rectangle rect = Bounds;
			rect.X -= rect.Width;
			rect.Offset(OffsetPoint.X, 0);
			return rect;
		}
		[Obsolete("Use CurrentItemEndBoundsWithoutOffset"), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public Rectangle CurrentItemEndBoundsWitoutOffset {
			get {
				return CurrentItemEndBoundsWithoutOffset;
			}
		}
		public Rectangle CurrentItemEndBoundsWithoutOffset {
			get {
				return CalcCurrentItemEndBoundsWithoutOffset();
			}
		}
		Rectangle CalcCurrentItemEndBoundsWithoutOffset() {
			Rectangle rect = Bounds;
			rect.X -= rect.Width;
			return rect;
		}
		public Rectangle CurrentItemStartBounds {
			get { return GetCurrentItemStartBoundsCore(); }
		}
		protected virtual Rectangle GetCurrentItemStartBoundsCore() {
			Rectangle rect = Bounds;
			rect.Offset(OffsetPoint.X, 0);
			return rect;
		}
		public Rectangle CurrentItemStartBoundsWithoutOffset {
			get { return Bounds; }
		}
		protected internal SliderBase Slider { get { return (SliderBase)OwnerControl; } }
		protected internal SliderAnimationInfo GetAnimationInfo() { 
				return XtraAnimator.Current.Get(Slider, Slider.GetAnimationId()) as SliderAnimationInfo;
		}
		public Rectangle CurrentItemBounds {
			get {
				SliderAnimationInfo info = GetAnimationInfo();
				if(info == null || info.IsFinalFrame) {
					return CurrentItemStartBounds;
				}
				return info.Current;
			}
		}
		public Rectangle NextItemBounds {
			get {
				SliderAnimationInfo info = GetAnimationInfo();
				if(info == null || info.IsFinalFrame) {
					if(OffsetPoint.X > 0)
						return CurrentItemEndBounds;
					return NextItemStartBounds;
				}
				return info.Next;
			}
		}
		public Rectangle LeftArrowBounds { get; private set; }
		public Rectangle RightArrowBounds { get; private set; }
		protected internal SkinElementInfo GetLeftArrowInfo() {
			SkinElement elem = EditorsSkins.GetSkin(DefaultSkinProvider.Default)[EditorsSkins.SkinSliderLeftArrow];
			if(elem == null)
				elem = CommonSkins.GetSkin(LookAndFeel)[CommonSkins.SkinButton];
			SkinElementInfo res = new SkinElementInfo(elem, LeftArrowBounds);
			res.ImageIndex = -1;
			res.State = LeftArrowState;
			return res;
		}
		protected internal SkinElementInfo GetRightArrowInfo() {
			SkinElement elem = EditorsSkins.GetSkin(DefaultSkinProvider.Default)[EditorsSkins.SkinSliderRightArrow];
			if(elem == null)
				elem = CommonSkins.GetSkin(LookAndFeel)[CommonSkins.SkinButton];
			SkinElementInfo res = new SkinElementInfo(elem, RightArrowBounds);
			res.ImageIndex = -1;
			res.State = RightArrowState;
			return res;
		}
		protected internal Size GetLeftArrowSize(Graphics g) {
			return ObjectPainter.CalcObjectMinBounds(g, SkinElementPainter.Default, GetLeftArrowInfo()).Size;
		}
		protected internal Size GetRightArrowSize(Graphics g) {
			return ObjectPainter.CalcObjectMinBounds(g, SkinElementPainter.Default, GetRightArrowInfo()).Size;
		}
		int ArrowIndent { get { return 10; } }
		protected virtual void CalcArrowBounds(Graphics g) {
			Size ls = GetLeftArrowSize(g);
			Size rs = GetRightArrowSize(g);
			LeftArrowBounds = new Rectangle(ContentRect.X + ArrowIndent, ContentRect.Y + (ContentRect.Height - ls.Height) / 2, ls.Width, ls.Height);
			RightArrowBounds = new Rectangle(ContentRect.Right - rs.Width - ArrowIndent, ContentRect.Y + (ContentRect.Height - rs.Height) / 2, rs.Width, rs.Height);
		}
		protected Point LeftArrowCenter {
			get {
				return new Point(LeftArrowBounds.X + LeftArrowBounds.Width / 2, LeftArrowBounds.Y + LeftArrowBounds.Height / 2);
			}
		}
		protected Point RightArrowCenter {
			get {
				return new Point(RightArrowBounds.X + RightArrowBounds.Width / 2, RightArrowBounds.Y + RightArrowBounds.Height / 2);
			}
		}
		public override void CalcViewInfo(Graphics g) {
			base.CalcViewInfo(g);
			CalcArrowBounds(g);
		}
		[Obsolete("Use LeftArrowOpacity"), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public float LeftArrowOpactiy {
			get {
				return LeftArrowOpacity;
			}
		}
		public float LeftArrowOpacity {
			get {
				return CalcArrowOpacity();
			}
		}
		[Obsolete("Use RightArrowOpacity"), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public float RightArrowOpactiy {
			get {
				return RightArrowOpacity;
			}
		}
		public float RightArrowOpacity {
			get {
				return CalcArrowOpacity();
			}
		}
		public float LeftArrowCurrentOpacity {
			get {
				return CalcCurrentArrowOpacity();
			}
		}
		public float RightArrowCurrentOpacity {
			get {
				return CalcCurrentArrowOpacity();;
			}
		}
		#region Arrow Animation
		protected virtual void StartAnimation(float start, float end) {
			XtraAnimator.Current.AddAnimation(new FloatAnimationInfo(XtraAnimationObject, AnimationId, Slider.ScrollButtonFadeAnimationTime, start, end));
		}
		protected virtual void StopAnimation() {
			XtraAnimator.RemoveObject(XtraAnimationObject, AnimationId);
		}
		protected virtual float CalcArrowOpacity() {
			float opacity = CalcArrowOpacityCore();
			if(!ShouldStartAnimation(opacity)) return opacity;
			StopAnimation();
			StartAnimation(currentOpacity, opacity);
			prevOpacity = opacity;
			return opacity;
		}
		float prevOpacity = 0;
		protected bool ShouldStartAnimation(float opacity) {
			return Math.Abs(opacity - prevOpacity) > 0.01; 
		}
		protected virtual float CalcArrowOpacityCore() {
			if(IsDesignMode)
				return 0f;
			Point pt = Slider.PointToClient(Control.MousePosition);
			return Bounds.Contains(pt) ? 1f : 0f;
		}
		float currentOpacity = 0;
		protected virtual float CalcCurrentArrowOpacity() {
			float opacity = CalcArrowOpacity();
			FloatAnimationInfo info = XtraAnimator.Current.Get(XtraAnimationObject, AnimationId) as FloatAnimationInfo;
			if(info == null) return opacity;
			if(!info.IsStarted) return this.currentOpacity;
			this.currentOpacity = info.Value;
			return this.currentOpacity;
		}
		object animationId = null;
		protected object AnimationId {
			get {
				if(this.animationId == null) this.animationId = new object();
				return animationId;
			}
		}
		protected internal ISupportXtraAnimation XtraAnimationObject { get { return OwnerControl as ISupportXtraAnimation; } }
		#endregion
		public Point OffsetPoint { get; private set; }
		protected bool IsDesignMode { get { return OwnerControl.IsDesignMode; } }
		protected internal virtual void Offset(Point point) {
			OffsetPoint = point;
		}
		protected internal virtual ImageSliderHitInfo CalcHitInfo(Point hitPoint) {
			return new ImageSliderHitInfo() { HitPoint = hitPoint };
		}
	}
	public class ImageSliderViewInfo : BaseSliderViewInfo, IAnimatedItem, ISupportXtraAnimation{
		object imageAnimationID;
		public ImageSliderViewInfo(ImageSlider slider) : base(slider) { 
			this.imageAnimationID = new object();
		}
		public ImageSlider ImageSlider { get { return (ImageSlider)Slider; } }
		public bool ShouldDrawCurrentImage {
			get { return !CurrentItemBounds.IsEmpty && ImageSlider.CurrentImage != null; }
		}
		public bool ShouldDrawNextImage {
			get { return !NextItemBounds.IsEmpty && ImageSlider.NextImage != null; }
		}
		public bool ShouldUseDisabledPainter {
			get { return !ImageSlider.Enabled && ImageSlider.UseDisabledStatePainter; } }
		public Image CurrentImage { get { return ImageSlider.CurrentImage; } }
		public Image NextImage { get { return ImageSlider.NextImage; } }
		public Rectangle CurrentImageBounds {
			get { 
				if(CurrentImage == null)
					return CurrentItemBounds;
				return ImageLayoutHelper.GetImageBounds(CurrentItemBounds, CurrentImage.Size, ImageSlider.LayoutMode); 
			}
		}
		public Rectangle NextImageBounds {
			get {
				if(NextImage == null)
					return NextItemBounds;
				return ImageLayoutHelper.GetImageBounds(NextItemBounds, NextImage.Size, ImageSlider.LayoutMode);
			}
		}
		protected override Rectangle GetNextItemBoundsCore() {
			return base.GetNextItemBoundsCore();
		}
		protected override Rectangle GetCurrentItemBoundsCore() {
			return base.GetCurrentItemBoundsCore();
		}
		public override void CalcViewInfo(Graphics g) {
			base.CalcViewInfo(g);
			CalcContextButtonsViewInfo(g);
		}
		ISupportContextItems ContextItemsOwner { get { return (ISupportContextItems)ImageSlider; } }
		protected virtual void CalcContextButtonsViewInfo(Graphics g) {
			if(ContextButtonsViewInfo == null)
				ContextButtonsViewInfo = new ContextItemCollectionViewInfo(ContextItemsOwner.ContextItems, ContextItemsOwner.Options, ContextItemsOwner);
			ContextButtonsViewInfo.CalcItems();
		}
		protected internal ContextItemCollectionViewInfo ContextButtonsViewInfo { get; set; }
		protected internal override ImageSliderHitInfo CalcHitInfo(Point hitPoint) {
			ImageSliderHitInfo hitInfo = new ImageSliderHitInfo();
			hitInfo.HitPoint = hitPoint;
			if(hitInfo.Check(LeftArrowBounds, ImageSliderHitTest.LeftButton)) {
				return hitInfo;
			}
			if(hitInfo.Check(RightArrowBounds, ImageSliderHitTest.RightButton)) {
				return hitInfo;
			}
			if(hitInfo.Check(CurrentImageBounds, ImageSliderHitTest.Image)) {
				return hitInfo;
			}
			return hitInfo;
		}
		AnimatedImageHelper imageHelper;
		protected AnimatedImageHelper ImageHelper {
			get {
				if(imageHelper == null)
					imageHelper = new AnimatedImageHelper(CurrentImage);
				return imageHelper;
			}
		} 
		public virtual void OnCurrentImageChanged() {
			ImageHelper.Image = CurrentImage;
			StopImageAnimation();
			StartImageAnimation();
		}
		public virtual void StopImageAnimation() {
			XtraAnimator.RemoveObject(this, this.imageAnimationID);		  
		}
		public virtual void StartImageAnimation() {
			IAnimatedItem animItem = this;
			if(IsDesignMode || animItem.FramesCount < 2) return;
			XtraAnimator.Current.AddEditorAnimation(this.imageAnimationID, this, animItem, new CustomAnimationInvoker(OnImageAnimation));
		}
		protected virtual void OnImageAnimation(BaseAnimationInfo animInfo) {
			IAnimatedItem animItem = this;
			EditorAnimationInfo info = animInfo as EditorAnimationInfo;
			if(CurrentImage == null || info == null) return;
			CurrentImage.SelectActiveFrame(FrameDimension.Time, info.CurrentFrame);
			ImageSlider.Invalidate(animItem.AnimationBounds);
		}
		#region IAnimatedItem Members
		Rectangle IAnimatedItem.AnimationBounds { get { return CurrentItemBounds; } }
		int IAnimatedItem.AnimationInterval { get {return ImageHelper.AnimationInterval; }}
		int[] IAnimatedItem.AnimationIntervals { get { return ImageHelper.AnimationIntervals; } }
		AnimationType IAnimatedItem.AnimationType { get { return ImageHelper.AnimationType; } }
		int IAnimatedItem.FramesCount { get { return ImageHelper.FramesCount; } }
		int IAnimatedItem.GetAnimationInterval(int frameIndex)
		{
			return ImageHelper.GetAnimationInterval(frameIndex);
		}
		bool IAnimatedItem.IsAnimated { get { return ImageHelper.IsAnimated; } }
		void IAnimatedItem.OnStart() { }
		void IAnimatedItem.OnStop() { }
		object IAnimatedItem.Owner { get { return this; }}
		void IAnimatedItem.UpdateAnimation(BaseAnimationInfo info)
		{
			ImageHelper.UpdateAnimation(info);
		}
		#endregion
		#region ISupportXtarAnimation Members
		bool ISupportXtraAnimation.CanAnimate 
		{
			get { return ((IAnimatedItem)this).FramesCount > 1; }
		}
		Control ISupportXtraAnimation.OwnerControl
		{
			get { return ImageSlider; }
		}	 
		#endregion
	}
	public class SliderAnimationInfo : BaseAnimationInfo {
		public SliderAnimationInfo(SliderBase slider, int animationId, Rectangle currentEnd, Rectangle currentStart, Rectangle nextStart, Rectangle nextEnd, int deltaTick, int frameCount) : 
		base((ISupportXtraAnimation)slider, animationId, deltaTick, frameCount) {
			CurrentStart = currentStart;
			NextStart = nextStart;
			NextEnd = nextEnd;
			CurrentEnd = currentEnd;
			CurrentXHelper = new SplineAnimationHelper();
			CurrentXHelper.Init(CurrentStart.X, CurrentEnd.X, 1);
			CurrentYHelper = new SplineAnimationHelper();
			CurrentYHelper.Init(CurrentStart.Y, CurrentEnd.Y, 1);
			CurrentWidthHelper = new SplineAnimationHelper();
			CurrentWidthHelper.Init(CurrentStart.Width, CurrentEnd.Width, 1);
			CurrentHeightHelper = new SplineAnimationHelper();
			CurrentHeightHelper.Init(CurrentStart.Height, CurrentEnd.Height, 1);
			NextXHelper = new SplineAnimationHelper();
			NextXHelper.Init(NextStart.X, NextEnd.X, 1);
			NextYHelper = new SplineAnimationHelper();
			NextYHelper.Init(NextStart.Y, NextEnd.Y, 1);
			NextWidthHelper = new SplineAnimationHelper();
			NextWidthHelper.Init(NextStart.Width, NextEnd.Width, 1);
			NextHeightHelper = new SplineAnimationHelper();
			NextHeightHelper.Init(NextStart.Height, NextEnd.Height, 1);
			Current = CurrentStart;
			Next = NextStart;
		}
		public Rectangle Current { get; private set; }
		public Rectangle Next { get; private set; }
		protected Rectangle CurrentStart { get; private set; }
		protected Rectangle NextStart { get; private set; }
		protected Rectangle NextEnd { get; private set; }
		protected Rectangle CurrentEnd { get; private set; }
		public SliderBase Slider { get { return (SliderBase)AnimatedObject; } }
		public override void FrameStep() {
			float dt = (float)Math.Min(1.0, ((float)(CurrentTick - BeginTick)) / (DeltaTick * FrameCount));
			Current = new Rectangle(
				(int)CurrentXHelper.CalcSpline(dt),
				(int)CurrentYHelper.CalcSpline(dt),
				(int)CurrentWidthHelper.CalcSpline(dt),
				(int)CurrentHeightHelper.CalcSpline(dt));
			Next = new Rectangle(
				(int)NextXHelper.CalcSpline(dt),
				(int)NextYHelper.CalcSpline(dt),
				(int)NextWidthHelper.CalcSpline(dt),
				(int)NextHeightHelper.CalcSpline(dt));
			if(IsFinalFrame) {
				Slider.SliderViewInfo.Offset(Point.Empty);
				Slider.OnAnimationEnd();
			}
			Slider.Invalidate();
			Slider.Update();
		}
		protected SplineAnimationHelper CurrentXHelper { get; private set; }
		protected SplineAnimationHelper CurrentYHelper { get; private set; }
		protected SplineAnimationHelper CurrentWidthHelper { get; private set; }
		protected SplineAnimationHelper CurrentHeightHelper { get; private set; }
		protected SplineAnimationHelper NextXHelper { get; private set; }
		protected SplineAnimationHelper NextYHelper { get; private set; }
		protected SplineAnimationHelper NextWidthHelper { get; private set; }
		protected SplineAnimationHelper NextHeightHelper { get; private set; }
	}
	public interface IImageSlider {
		void OnCollectionChanged();		
		void ForceStopAnimation();		
	}
	[ToolboxItem(false)]
	public class SliderBase : BaseStyleControl, ISupportXtraAnimation, IImageSlider {
		static Point InvalidPoint = new Point(-10000, -10000);
		public SliderBase() {
			AnimationTime = 700;
			DownPoint = InvalidPoint;
		}
		protected internal BaseSliderViewInfo SliderViewInfo { get { return (BaseSliderViewInfo)ViewInfo; } }
		protected override BaseControlPainter CreatePainter() {
			return new BaseSliderPainter();
		}
		protected override BaseStyleControlViewInfo CreateViewInfo() {
			return new BaseSliderViewInfo(this);
		}
		public void SlideNext() {
			if(!AllowSlide || !PrepareNewItem(true))
				return;
			OnSlideNext();
			RunForwardAnimation();
		}
		protected virtual void RunForwardAnimation() {
			Rectangle currentEnd = GetCurrentItemEndBoundsWithoutOffset();
			Rectangle currentStart = GetCurrentItemStartBounds();
			Rectangle nextStart = GetNextItemStartBounds();
			Rectangle nextEnd = GetNextItemEndBounds();
			RunAnimation(currentEnd, currentStart, nextStart, nextEnd);
		}
		protected virtual Rectangle GetCurrentItemEndBoundsWithoutOffset() {
			return SliderViewInfo.CurrentItemEndBoundsWithoutOffset;
		}
		protected virtual Rectangle GetNextItemEndBounds() {
			return SliderViewInfo.NextItemEndBounds;
		}
		protected virtual void OnSlideNext() {
		}
		bool AllowSlide {
			get { return SliderViewInfo.GetAnimationInfo() == null; }
		}
		public void SlidePrev() {
			if(!AllowSlide || !PrepareNewItem(false))
				return;
			OnSlidePrev();
			RunBackwardAnimation();
		}
		protected virtual void RunBackwardAnimation() {
			Rectangle currentStart = GetCurrentItemStartBounds();
			Rectangle nextStart = GetCurrentItemEndBounds();
			Rectangle nextEnd = GetCurrentItemStartBoundsWithoutOffset();
			Rectangle currentEnd = GetNextItemStartBoundsWithoutOffset();
			RunAnimation(currentEnd, currentStart, nextStart, nextEnd);
		}
		protected virtual Rectangle GetCurrentItemStartBoundsWithoutOffset() {
			return SliderViewInfo.CurrentItemStartBoundsWithoutOffset;
		}
		protected virtual Rectangle GetNextItemStartBoundsWithoutOffset() {
			return SliderViewInfo.NextItemStartBoundsWithoutOffset;
		}
		protected virtual void OnSlidePrev() {
		}
		public void SlideFirst() {
			if(!AllowSlide || !PrepareEdgeItem(false))
				return;
			OnSlideFirst();
			RunBackwardAnimation();
		}
		protected virtual void OnSlideFirst() {
		}
		public void SlideLast() {
			if(!AllowSlide || !PrepareEdgeItem(true))
				return;
			OnSlideLast();
			RunForwardAnimation();
		}
		protected virtual void OnSlideLast() {
		}
		protected internal virtual bool HasPrevItem { get { return false; } }
		protected internal virtual bool HasNextItem { get { return false; } }
		protected internal virtual bool PrepareNewItem(bool next) {
			return false;
		}
		protected internal virtual bool PrepareNewItem(int prevIndex, int nextIndex) {
			return false;
		}
		protected internal virtual bool PrepareEdgeItem(bool next) {
			return false;
		}
		protected virtual Rectangle GetCurrentItemStartBounds() {
			return SliderViewInfo.CurrentItemStartBounds;
		}
		protected virtual Rectangle GetNextItemStartBounds() {
			return SliderViewInfo.NextItemStartBounds;
		}
		protected virtual Rectangle GetCurrentItemEndBounds() {
			return SliderViewInfo.CurrentItemEndBounds;
		}
		protected virtual Rectangle GetCurrentItemBounds() {
			return SliderViewInfo.CurrentItemBounds;
		}
		protected virtual Rectangle GetNextItemBounds() {
			return SliderViewInfo.NextItemBounds;
		}
		protected internal virtual int GetAnimationId() { return 1; }
		int animationTime;
		[DefaultValue(700), SmartTagProperty("Animation Time", "")]
		public int AnimationTime {
			get { return animationTime; }
			set { 
				if(animationTime == value) return;
				if(value < 0) throw new ArgumentException();
				if(value == 0) value = 1;
				animationTime = value;
			}
		}
		[DefaultValue(TimeSpan.TicksPerMillisecond * 700), Browsable(false)]
		public long Ticks {
			get { return AnimationTime * TimeSpan.TicksPerMillisecond; }
		}
		const int defaultScrollBtnFadeAnimationTime = 600;
		int scrollBtnFadeAnimationTime = defaultScrollBtnFadeAnimationTime;
		[DefaultValue(defaultScrollBtnFadeAnimationTime)]
		public int ScrollButtonFadeAnimationTime {
			get { return scrollBtnFadeAnimationTime; }
			set {
				if(scrollBtnFadeAnimationTime == value)
					return;
				if(value < 0)
					throw new ArgumentException();
				if(value == 0) value = 1;
				scrollBtnFadeAnimationTime = value;
			}
		}
		protected virtual void RunAnimation(Rectangle currentEnd, Rectangle currentStart, Rectangle nextStart, Rectangle nextEnd) {
			XtraAnimator.Current.AddAnimation(new SliderAnimationInfo(this, GetAnimationId(), currentEnd, currentStart, nextStart, nextEnd, (int)(Ticks / 1000), 1000));
		}	   
		#region ISupportXtraAnimation Members
		bool ISupportXtraAnimation.CanAnimate {
			get { return true; }
		}
		Control ISupportXtraAnimation.OwnerControl {
			get { return this; }
		}
		#endregion
		void InvalidateArrows() {
			Invalidate();
		}
		protected override void OnMouseMove(MouseEventArgs e) {
			base.OnMouseMove(e);
			SliderViewInfo.UpdateButtonsState(e.Location, e.Button);
			if(DownPoint == InvalidPoint || SliderViewInfo.GetAnimationInfo() != null)
				return;
			UpdateCurrentItemOffset(e.Location);
			if(DownPoint == InvalidPoint)
				return;
			if(ShouldSlidePrev(e.Location)) {
				DownPoint = InvalidPoint;
				SlidePrev();
			} else if(ShouldSlideNext(e.Location)) {
				DownPoint = InvalidPoint;
				SlideNext();
			}
		}
		private bool ShouldSlideNext(Point point) {
			return - point.X + DownPoint.X > ClientRectangle.Width / 2;
		}
		private bool ShouldSlidePrev(Point point) {
			return point.X - DownPoint.X > ClientRectangle.Width / 2;
		}
		private void UpdateCurrentItemOffset(Point point) {
			SliderViewInfo.Offset(new Point(point.X - DownPoint.X, point.Y - DownPoint.Y));
			bool hasNextItem = false;
			if(SliderViewInfo.OffsetPoint.X < 0) {
				hasNextItem = PrepareNewItem(true);
			} else if(SliderViewInfo.OffsetPoint.X > 0){
				hasNextItem = PrepareNewItem(false);
			}
			if(!hasNextItem) {
				SliderViewInfo.Offset(Point.Empty);
				DownPoint = InvalidPoint;
			}
			Invalidate();
			Update();
		}
		protected override void OnMouseLeave(EventArgs e) {
			base.OnMouseLeave(e);
			InvalidateArrows();
		}
		protected override void OnMouseEnter(EventArgs e) {
			base.OnMouseEnter(e);
			InvalidateArrows();
		}
		protected override void OnMouseUp(MouseEventArgs e) {
			base.OnMouseUp(e);
			DownPoint = InvalidPoint;
			SliderViewInfo.UpdateButtonsState(e.Location, e.Button);
			if(SliderViewInfo.OffsetPoint != Point.Empty) {
				ResetCurrentItemPoisiton();
			}
			Invalidate();
		}
		protected virtual void ResetCurrentItemPoisiton() {
			SliderViewInfo.Offset(Point.Empty);
			Invalidate();
			Update();
		}
		Point DownPoint { get; set; }
		protected override void OnMouseDown(MouseEventArgs e) {
			base.OnMouseDown(e);
			if(SliderViewInfo.GetAnimationInfo() == null) {
				SliderViewInfo.UpdateButtonsState(e.Location, e.Button);
				if(SliderViewInfo.LeftArrowBounds.Contains(e.Location))
					SlidePrev();
				else if(SliderViewInfo.RightArrowBounds.Contains(e.Location))
					SlideNext();
			}
			DownPoint = e.Location;
		}
		protected internal virtual void OnAnimationEnd() {
		}
		protected internal virtual void OnCollectionChanged() {
		}
		void IImageSlider.OnCollectionChanged() {
			OnCollectionChanged();
		}
		protected internal virtual void ForceStopAnimation() {
		}
		void IImageSlider.ForceStopAnimation() {
			ForceStopAnimation();
		}
	}
	[Editor("DevExpress.XtraEditors.Design.ImageSliderImagesEditor, " + AssemblyInfo.SRAssemblyEditorsDesign, typeof(System.Drawing.Design.UITypeEditor))]
	public class SliderImageCollection : CollectionBase {
		IImageSlider slider;
		public SliderImageCollection(IImageSlider slider) {
			this.slider = slider;
		}
		public int Add(Image image) { return List.Add(image); }
		public void Insert(int index, Image image) { List.Insert(index, image); }
		public void Remove(Image image) { List.Remove(image); }
		public bool Contains(Image image) { return List.Contains(image); }
		public int IndexOf(Image image) { return List.IndexOf(image); }
		public Image this[int index] { get { return (Image)List[index]; } set { List[index] = value; } }
		protected override void OnInsertComplete(int index, object value) {
			base.OnInsertComplete(index, value);
			ImageSlider imSlider = (ImageSlider)slider;
			if(!imSlider.IsLoading) {				
				Slider.OnCollectionChanged();
			}
		}
		protected override void OnRemoveComplete(int index, object value) {
			base.OnRemoveComplete(index, value);
			Slider.OnCollectionChanged();
		}
		protected override void OnSetComplete(int index, object oldValue, object newValue) {
			base.OnSetComplete(index, oldValue, newValue);
			Slider.OnCollectionChanged();
		}				   
		protected override void OnClearComplete() {
			Slider.ForceStopAnimation();
			base.OnClearComplete();
			Slider.OnCollectionChanged();
		}
		public IImageSlider Slider { get { return slider;  }}
	}
	[
	Designer("DevExpress.XtraEditors.Design.ImageSliderDesigner, " + AssemblyInfo.SRAssemblyEditorsDesign),
	DXToolboxItem(DXToolboxItemKind.Regular), 
	ToolboxTabName(AssemblyInfo.DXTabCommon), 
	ToolboxBitmap(typeof(ToolboxIconsRootNS), "ImageSlider"),
	SmartTagFilter(typeof(ImageSliderFilter)),
	SmartTagAction(typeof(ImageSliderActions), "Images", "Images", SmartTagActionType.CloseAfterExecute),
	Description("An image-browsing control that slides through its images using navigation buttons displayed on hover.")
	]
	public class ImageSlider : SliderBase, ISupportXtraAnimation, ISupportInitialize, ISupportContextItems, IContextItemCollectionOwner, IContextItemCollectionOptionsOwner {
		static readonly object currentImageIndexChanged = new object();
		private static readonly object contextButtonClick = new object();
		int currentImageIndex;   
		bool isLoading;
		bool suppressUpdatingImageIndex;	  
		Image currentImage;
		bool useDisabledStatePainter;
		public ImageSlider() {
			this.currentImageIndex = -1;
			this.useDisabledStatePainter = true;
		}
		public override bool IsLoading {
			get {
				return isLoading;
			}
		}	   
		SliderImageCollection images;
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public SliderImageCollection Images {
			get {
				if(images == null)
					images = new SliderImageCollection(this);			
				return images;
			}
		}
		object imageList;	   
		[DXCategory(CategoryName.Appearance), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("ImageSliderImageList"),
#endif
 DefaultValue(null), 
		TypeConverter(typeof(DevExpress.Utils.Design.ImageCollectionImagesConverter))]
		public object ImageList {
			get { return imageList; }
			set {
				if(imageList == value) return;
				imageList = value;
				ImageCollection imageColl = imageList as ImageCollection;
				if(imageColl != null)
					imageColl.Changed += OnImageCollectionChanged;
				OnCollectionChanged();			   
			}
		}
		private void OnImageCollectionChanged(object sender, EventArgs e) {
			OnCollectionChanged();
		}	  
		protected virtual int GetImagesCount() {
			if(imageList == null)
				return Images.Count;
			ImageCollection imageColl = imageList as ImageCollection;
			if(imageColl != null)
				return imageColl.Images.Count;
			System.Windows.Forms.ImageList imList = imageList as System.Windows.Forms.ImageList;
			if(imList != null)
				return imList.Images.Count;
			return 0;			 
		}
		protected virtual Image GetImageByIndex(int index) {
			if(index == -1)
				return null;
			if(imageList == null)
				return Images[index];
			ImageCollection imageColl = imageList as ImageCollection;
			if(imageColl != null)
				return imageColl.Images[index];
			System.Windows.Forms.ImageList imList = imageList as System.Windows.Forms.ImageList;
			if(imList != null) {
				if(index < imList.Images.Count)
					return imList.Images[index];
			}			 
			return null;
		}
		public override void Refresh() {
			base.Refresh();
			OnCollectionChanged();
		}		   
		protected internal override bool HasPrevItem {
			get {
				if(!VirtualMode) {
					if(!AllowLooping) return CurrentImageIndex > 0;
					return GetImagesCount() != 0;
				}
				return RaiseCanGetImageEvent(false);
			}
		}
		protected internal override bool HasNextItem {
			get {
				if(!VirtualMode) {
					if(!AllowLooping) return CurrentImageIndex < GetImagesCount() - 1;
					return GetImagesCount() != 0;
				}
				return RaiseCanGetImageEvent(true);
			}
		}
		protected override void OnSlideNext() {
			if(!HasNextItem) return;
			StopImageAnimation();		   
			if(!VirtualMode) {
				if(AllowLooping && CurrentImageIndex == GetImagesCount() - 1) {
				   this.suppressUpdatingImageIndex = true;
				   try {
					   CurrentImageIndex = 0;
				   }
				   finally {
					   this.suppressUpdatingImageIndex = false;
				   }					
				   return;
				}
				this.suppressUpdatingImageIndex = true;
				try {
					CurrentImageIndex++;
				}
				finally {
					this.suppressUpdatingImageIndex = false;
				}						  
			}
			else RaiseGetImageEvent(true, false, false, true);
		}
		protected override void OnSlidePrev() {
			if(!HasPrevItem) return;
			StopImageAnimation();		   
			if(!VirtualMode){
				if(AllowLooping && CurrentImageIndex == 0) {
				   this.suppressUpdatingImageIndex = true;
				   try {
					   CurrentImageIndex = GetImagesCount() - 1;
				   }
				   finally {
					   this.suppressUpdatingImageIndex = false;
				   }		   
				   return;
				}
				this.suppressUpdatingImageIndex = true;
				try {
					CurrentImageIndex--;
				}
				finally {
					this.suppressUpdatingImageIndex = false;
				}			   
			}
			else RaiseGetImageEvent(false, true, false, true);
		}
		protected override void OnSlideFirst() {
			base.OnSlideFirst();
			if(!HasPrevItem) return;
			if(Images == null) return;
			StopImageAnimation();  
			this.suppressUpdatingImageIndex = true;
			try {
				CurrentImageIndex = 0;
			}
			finally {
				this.suppressUpdatingImageIndex = false;
			}		   
		}
		protected override void OnSlideLast() {
			base.OnSlideLast();
			if(!HasNextItem) return;
			if(Images == null) return;
			StopImageAnimation();  
			this.suppressUpdatingImageIndex = true;
			try {
				CurrentImageIndex = GetImagesCount() - 1;
			}
			finally {
				this.suppressUpdatingImageIndex = false;
			}		   
		}
		protected ImageSliderViewInfo ImageSliderViewInfo { get { return (ImageSliderViewInfo)ViewInfo; } }
		protected override BaseStyleControlViewInfo CreateViewInfo() {
			return new ImageSliderViewInfo(this);
		}
		protected override BaseControlPainter CreatePainter() {
			return new ImageSliderPainter();
		}  
		public int CurrentImageIndex {
			get { return currentImageIndex; }
			set {
				if (!IsLoading)
					value = ConstrainImageIndex(value);
				if(currentImageIndex == value) return;
				ImageSliderCurrentImageIndexChangedEventArgs arg = new ImageSliderCurrentImageIndexChangedEventArgs(currentImageIndex, value);
				currentImageIndex = value;						
				RaiseCurrentImageIndexChanged(arg);
				if (!this.suppressUpdatingImageIndex && !IsLoading)				   
					 OnCollectionChanged();			  
			}
		}
		private int ConstrainImageIndex(int value) {
			if(value == -1)
			   return value;
			return Math.Min(Math.Max(0, value), GetImagesCount() - 1);
		}	   
		public void SetCurrentImageIndex(int imageIndex, bool animated) {
			if(imageIndex == CurrentImageIndex)
				return;
			if(VirtualMode) {
				throw new InvalidOperationException("SetCurrentImageIndex method call is disabled in virtual mode.");
			}
			if(imageIndex < 0 || imageIndex >= GetImagesCount()) {
				throw new IndexOutOfRangeException("Index was outside the bounds of the Images collection.");
			}
			if(animated)
				ChangeImageWithAnimation(imageIndex);
			else
				ChangeImageWithoutAnimation(imageIndex);					  
		}
		protected virtual void ChangeImageWithoutAnimation(int imageIndex) {
			CurrentImageIndex = imageIndex;		   
			Update();
		}
		protected virtual void ChangeImageWithAnimation(int imageIndex) {
			PrepareNewItem(CurrentImageIndex, imageIndex);
			if(imageIndex > CurrentImageIndex)
				RunForwardAnimation();
			else
				RunBackwardAnimation();
			this.suppressUpdatingImageIndex = true;
			try {
				CurrentImageIndex = imageIndex;
			}
			finally {
				this.suppressUpdatingImageIndex = false;
			}			
		}
		public void SetCurrentImageIndex(int imageIndex) {
			SetCurrentImageIndex(imageIndex, true);	  
		}
		[Obsolete("Use the CurrentImageIndex property instead.")]
		public int GetCurrentImageIndex() {
			return CurrentImageIndex;
		}
		public bool UseDisabledStatePainter {
			get { return useDisabledStatePainter; }
			set {
				if(useDisabledStatePainter == value) return;
				useDisabledStatePainter = value;
				OnUseDisabledStatePainterChanged();
			}
		}
		private void OnUseDisabledStatePainterChanged() {
			if(IsDesignMode || IsLoading || Enabled) return;
			Invalidate();
		}
		protected override void OnEnabledChanged(EventArgs e) {
			base.OnEnabledChanged(e);
			if(!Enabled)
				StopImageAnimation();
			else
				StartImageAnimation();
		}
		[Browsable(false)]		   
		public Image CurrentImage {
			get { return currentImage; }
			private set {
				if(currentImage == value) return;
				currentImage = value;	
				if (!Enabled) return;
				ImageSliderViewInfo.OnCurrentImageChanged();			   
			}
		}	  
		public void StartImageAnimation() { 
			ImageSliderViewInfo.StartImageAnimation(); 
		}
		public void StopImageAnimation() {
			ImageSliderViewInfo.StopImageAnimation();
		}
		[Browsable(false)]
		public Image NextImage { get; private set; }
		protected internal override bool PrepareNewItem(bool next) {
			if(!VirtualMode) return PrepareNewItemStandardMode(next);
			return PrepareNewItemVirtualMode(next);
		}
		protected virtual bool PrepareNewItemVirtualMode(bool next){
			if(!RaiseCanGetImageEvent(next)) return false;
			if(next) NextImage = RaiseGetImageEvent(true, false, false, false);
			else NextImage = RaiseGetImageEvent(false, true, false, false);
			return true;
		}
		protected internal override bool PrepareNewItem(int prevIndex, int nextIndex) {
			CurrentImage = GetImageByIndex(prevIndex);
			NextImage = GetImageByIndex(nextIndex);
			return true;
		}
		protected virtual bool PrepareNewItemStandardMode(bool next) {
			if((next && CurrentImageIndex < GetImagesCount() - 1) || (!next && CurrentImageIndex > 0)) {
				CurrentImage = GetImageByIndex(CurrentImageIndex);
				NextImage = GetImageByIndex(next ? CurrentImageIndex + 1 : CurrentImageIndex - 1);
				return true;
			}
			if(AllowLooping && GetImagesCount() != 0) {
				if(next && CurrentImageIndex == GetImagesCount() - 1) {
					CurrentImage = GetImageByIndex(GetImagesCount() - 1);
					NextImage = GetImageByIndex(0);
				}
				else {
					CurrentImage = GetImageByIndex(0);
					NextImage = GetImageByIndex(GetImagesCount() - 1);
				}
				return true;
			}
			NextImage = null;
			return false;
		}
		protected internal override bool PrepareEdgeItem(bool next) {
			if(!VirtualMode) {
				CurrentImage = GetImageByIndex(CurrentImageIndex);
				NextImage = GetImageByIndex(next ? GetImagesCount() - 1 : 0);
				return true;
			}
			return PrepareVirtualEdgeItem(next);
		}
		protected virtual bool PrepareVirtualEdgeItem(bool next) {
			if(!RaiseCanGetImageEvent(next)) return false;
			if(next) NextImage = RaiseGetEdgeImageEvent(false, true);
			else NextImage = RaiseGetEdgeImageEvent(true, false);
			return true;
		}   
		protected internal override void OnCollectionChanged() {
			if(VirtualMode) return;		   
			if(CurrentImageIndex >= GetImagesCount()) {
				CurrentImageIndex = GetImagesCount() - 1;
				CurrentImage = CurrentImageIndex == -1 ? null : GetImageByIndex(CurrentImageIndex);
				NextImage = null;
			}
			else {				
				if(CurrentImageIndex < 0 && GetImagesCount() > 0)					
					CurrentImageIndex = 0;		   
				if(CurrentImageIndex < 0)
					return;
				CurrentImage = GetImageByIndex(CurrentImageIndex);
			}
			Invalidate();
		}
		bool mode;
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("ImageSliderVirtualMode"),
#endif
 DefaultValue(false)]
		public bool VirtualMode {
			get { return mode; }
			set { 
				if(mode == value) return;
				if(value) 
					CurrentImage = RaiseGetImageEvent(false, false, true, false);
				else {
					if(GetImagesCount() == 0) CurrentImage = null;
					else CurrentImage = GetImageByIndex(CurrentImageIndex);
				}
				mode = value;
				Invalidate();
			}
		}
		[ DefaultValue(false)]
		public bool AllowLooping {
			get;
			set;
		}
		ImageLayoutMode layoutMode = ImageLayoutMode.Stretch;
		[SmartTagProperty("Layout Mode", "")]
		public ImageLayoutMode LayoutMode {
			get { return layoutMode; }
			set {
				if(LayoutMode == value)
					return;
				layoutMode = value;
				OnLayoutModeChanged();
			}
		}
		protected virtual void OnLayoutModeChanged() {
			Invalidate();
		}
		protected internal override void OnAnimationEnd() {
			base.OnAnimationEnd();
			if(!VirtualMode) {
				NextImage = null;
				CurrentImage = GetImageByIndex(CurrentImageIndex);
			}
			else CurrentImage = NextImage;
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("ImageSliderCurrentImageIndexChanged"),
#endif
 DXCategory(CategoryName.PropertyChanged)]
		public event ImageSliderCurrentImageIndexChangedEventHandler CurrentImageIndexChanged {
			add { Events.AddHandler(currentImageIndexChanged, value); }
			remove { Events.RemoveHandler(currentImageIndexChanged, value); }
		}
		protected virtual void RaiseCurrentImageIndexChanged(ImageSliderCurrentImageIndexChangedEventArgs e) {
			ImageSliderCurrentImageIndexChangedEventHandler handler = (ImageSliderCurrentImageIndexChangedEventHandler)Events[currentImageIndexChanged];
			if(handler != null) handler(this, e);
		}
		#region Virtual Mode
		protected virtual Image RaiseGetEdgeImageEvent(bool isFirst, bool isLast) {
			if(DesignMode || CurrentImage == null) return null;
			GetImageEventArgs arg = new GetImageEventArgs(isFirst, isLast);
			OnGetImage(arg);
			return arg.Image;
		}
		protected virtual Image RaiseGetImageEvent(bool isNext, bool isPrev, bool isStartUp, bool currentImageUpdated) {
			if(DesignMode || CurrentImage == null) return null;
			GetImageEventArgs arg = new GetImageEventArgs(isNext, isPrev, isStartUp, currentImageUpdated);
			OnGetImage(arg);
			return arg.Image;
		}
		protected virtual bool RaiseCanGetImageEvent(bool isNext) {
			if(DesignMode || CurrentImage == null) return false;
			CanGetNextPrevImageEventArgs arg = new CanGetNextPrevImageEventArgs(isNext);
			OnCanGetNextPrevImage(arg);
			return arg.CanGetImage;
		}
		public event EventHandler<GetImageEventArgs> GetImage;
		protected virtual void OnGetImage(GetImageEventArgs e) {
			if(GetImage != null)
				GetImage(this, e);
		}
		public event EventHandler<CanGetNextPrevImageEventArgs> CanGetNextPrevImage;
		protected virtual void OnCanGetNextPrevImage(CanGetNextPrevImageEventArgs e) {
			if(CanGetNextPrevImage != null)
				CanGetNextPrevImage(this, e);
		}
		#endregion
		#region ISupportXtraAnimation Members
		bool ISupportXtraAnimation.CanAnimate {
			get {  return true; }
		}
		Control ISupportXtraAnimation.OwnerControl {
			get { return this; }
		}
		#endregion
		protected virtual void RequestCurrentImageCore() {
			if(!VirtualMode) return;
			GetImageEventArgs arg = new GetImageEventArgs(false, false, true, false);
			OnGetImage(arg);
			CurrentImage = arg.Image;
		}
		public void UpdateImage() {
			RequestCurrentImageCore();
			Invalidate();
		}
		#region ISupportInitialize Members
		void ISupportInitialize.BeginInit() {
			this.isLoading = true;
		}
		void ISupportInitialize.EndInit() {		
			this.isLoading = false;
			UpdateImage();
			OnCollectionChanged();	   
		}
		#endregion
		protected internal override void ForceStopAnimation() {
			if(XtraAnimator.Current.Get(SliderViewInfo.XtraAnimationObject, GetAnimationId()) != null)
				XtraAnimator.Current.Animations.Remove(SliderViewInfo.XtraAnimationObject, GetAnimationId());
		}
		public virtual ImageSliderHitInfo CalcHitInfo(Point hitPoint) { 
			return SliderViewInfo.CalcHitInfo(hitPoint); 
		}
		protected override ToolTipControlInfo GetToolTipInfo(Point point) {
			ToolTipControlInfo info = base.GetToolTipInfo(point);
			ToolTipControlInfo contextBtnInfo = ((ImageSliderViewInfo)ViewInfo).ContextButtonsViewInfo.GetToolTipInfo(point);
			return contextBtnInfo == null ? info : contextBtnInfo;
		}
		ContextItemCollection contextButtons;
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ContextItemCollection ContextButtons {
			get {
				if(contextButtons == null) {
					contextButtons = CreateContextButtons();
					contextButtons.Options = ContextButtonOptions;
				}
				return contextButtons;
			}
		}
		protected virtual ContextItemCollection CreateContextButtons() {
			return new ContextItemCollection(this);
		}
		ContextItemCollectionOptions contextButtonOptions;
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content), TypeConverter(typeof(ExpandableObjectConverter))]
		public ContextItemCollectionOptions ContextButtonOptions {
			get {
				if(contextButtonOptions == null) {
					contextButtonOptions = CreateContextButtonOptions();
				}
				return contextButtonOptions;
			}
		}
		public event ContextItemClickEventHandler ContextButtonClick {
			add { Events.AddHandler(contextButtonClick, value); }
			remove { Events.RemoveHandler(contextButtonClick, value); }
		}
		protected virtual ContextItemCollectionOptions CreateContextButtonOptions() {
			return new ContextItemCollectionOptions(this);
		}
		Rectangle ISupportContextItems.DisplayBounds {
			get { return ViewInfo.ContentRect; }
		}
		Rectangle ISupportContextItems.DrawBounds {
			get { return ViewInfo.ContentRect; }
		}
		Rectangle ISupportContextItems.ActivationBounds {
			get { return ViewInfo.ContentRect; }
		}
		ContextItemCollection ISupportContextItems.ContextItems {
			get { return ContextButtons; }
		}
		Control ISupportContextItems.Control {
			get { return this; }
		}
		bool ISupportContextItems.DesignMode { get { return DesignMode; } }
		bool ISupportContextItems.ShowOutsideDisplayBounds { get { return false; } }
		bool ISupportContextItems.CloneItems { get { return false; } }
		void ISupportContextItems.RaiseCustomizeContextItem(ContextItem item) { }
		void ISupportContextItems.RaiseContextItemClick(ContextItemClickEventArgs e) {
			ContextItemClickEventHandler handler = Events[contextButtonClick] as ContextItemClickEventHandler;
			if(handler != null)
				handler(this, e);
		}
		void ISupportContextItems.RaiseCustomContextButtonToolTip(ContextButtonToolTipEventArgs e) { 
		}
		ItemHorizontalAlignment ISupportContextItems.GetCaptionHorizontalAlignment(ContextButton btn) {
			return ItemHorizontalAlignment.Left;
		}
		ItemVerticalAlignment ISupportContextItems.GetCaptionVerticalAlignment(ContextButton btn) {
			return ItemVerticalAlignment.Center;
		}
		ItemHorizontalAlignment ISupportContextItems.GetGlyphHorizontalAlignment(ContextButton btn) {
			return ItemHorizontalAlignment.Left;
		}
		ItemLocation ISupportContextItems.GetGlyphLocation(ContextButton btn) {
			return ItemLocation.Left;
		}
		int ISupportContextItems.GetGlyphToCaptionIndent(ContextButton btn) {
			return 2;
		}
		ItemVerticalAlignment ISupportContextItems.GetGlyphVerticalAlignment(ContextButton btn) {
			return ItemVerticalAlignment.Center;
		}
		LookAndFeel.UserLookAndFeel ISupportContextItems.LookAndFeel {
			get { return LookAndFeel.ActiveLookAndFeel; }
		}
		ContextItemCollectionOptions ISupportContextItems.Options {
			get { return ContextButtonOptions; }
		}
		void ISupportContextItems.Redraw() {
			Invalidate();
		}
		void ISupportContextItems.Redraw(Rectangle rect) {
			Invalidate(rect);
		}
		void ISupportContextItems.Update() {
			Update();
		}
		void IContextItemCollectionOwner.OnCollectionChanged() {
			LayoutChanged();
		}
		bool IContextItemCollectionOwner.IsDesignMode { get { return IsDesignMode; } }
		bool IContextItemCollectionOwner.IsRightToLeft {
			get { return IsRightToLeft; }
		}
		void IContextItemCollectionOwner.OnItemChanged(ContextItem item, string propertyName, object oldValue, object newValue) {
			if(propertyName == "Visibility") {
				Invalidate();
				Update();
				return;
			}
			LayoutChanged();
		}
		void IContextItemCollectionOptionsOwner.OnOptionsChanged(string propertyName, object oldValue, object newValue) {
			LayoutChanged();
		}
		ContextAnimationType IContextItemCollectionOptionsOwner.AnimationType { get { return ContextAnimationType.OpacityAnimation; } }
		ContextItemCollectionHandler contextButtonsHandler;
		protected ContextItemCollectionHandler ContextButtonsHandler {
			get {
				if(contextButtonsHandler == null)
					contextButtonsHandler = CreateContextButtonsHandler();
				return contextButtonsHandler;
			}
		}
		protected virtual ContextItemCollectionHandler CreateContextButtonsHandler() {
			return new ContextItemCollectionHandler();
		}
		protected override void OnMouseEnter(EventArgs e) {
			base.OnMouseEnter(e);
			ContextButtonsHandler.ViewInfo = ((ImageSliderViewInfo)ViewInfo).ContextButtonsViewInfo;
			ContextButtonsHandler.OnMouseEnter(e);
		}
		protected override void OnMouseLeave(EventArgs e) {
			base.OnMouseLeave(e);
			ContextButtonsHandler.ViewInfo = ((ImageSliderViewInfo)ViewInfo).ContextButtonsViewInfo;
			ContextButtonsHandler.OnMouseLeave(e);
		}
		protected override void OnMouseMove(MouseEventArgs e) {
			base.OnMouseMove(e);
			ContextButtonsHandler.ViewInfo = ((ImageSliderViewInfo)ViewInfo).ContextButtonsViewInfo;
			ContextButtonsHandler.OnMouseMove(e);
		}
		protected override void OnMouseUp(MouseEventArgs e) {
			base.OnMouseUp(e);
			if(ContextButtonsHandler.OnMouseUp(e))
				return;
		}
		protected override void OnMouseDown(MouseEventArgs e) {
			ContextButtonsHandler.ViewInfo = ((ImageSliderViewInfo)ViewInfo).ContextButtonsViewInfo;
			if(ContextButtonsHandler.OnMouseDown(e))
				return;
			base.OnMouseDown(e);
		}
	}
	public enum ImageSliderHitTest { None, LeftButton, RightButton, Image }
	public class ImageSliderHitInfo {
		Point hitPoint;
		ImageSliderHitTest hitTest;
		public ImageSliderHitInfo () {
			this.hitPoint = Point.Empty;
			this.hitTest = ImageSliderHitTest.None;
		}
		public bool Check(Rectangle bounds, ImageSliderHitTest hitTest) {
			if(bounds.Contains(HitPoint)) {
				this.hitTest = hitTest;
				return true;
			}
			return false;
		}
		public Point HitPoint { get { return hitPoint; } set { hitPoint = value; } }
		public ImageSliderHitTest HitTest { get { return hitTest; } }
	}
	public class GetImageEventArgs : EventArgs {
		public GetImageEventArgs(bool isNext, bool isPrev, bool isStartUp, bool currentImageUpdated) {
			Image = null;
			IsNext = isNext;
			IsPrev = isPrev;
			IsStartUp = isStartUp;
			CurrentImageUpdated = currentImageUpdated;
		}
		public GetImageEventArgs(bool isFirst, bool isLast)
			: this(false, false, false, false) {
			IsFirst = isFirst;
			IsLast = isLast;
		}
		public Image Image { get; set; }
		public bool IsNext { get; set; }
		public bool IsPrev { get; set; }
		public bool IsFirst { get; set; }
		public bool IsLast { get; set; }
		public bool IsStartUp { get; set; }
		public bool CurrentImageUpdated { get; set; }
	}
	public class CanGetNextPrevImageEventArgs : EventArgs {
		public CanGetNextPrevImageEventArgs(bool isNext) {
			CanGetImage = false;		  
			IsNext = isNext;
		}
		public bool CanGetImage { get; set; }
		public bool IsNext { get; set; }
	}  
}
