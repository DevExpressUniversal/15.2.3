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
using System.ComponentModel;
using System.Drawing;
using DevExpress.Data.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.Utils.Drawing.Animation;
namespace DevExpress.Utils.Animation {
	public interface IAnimator : IDisposable {
		bool AnimationInProgress { get; }
		void DrawAnimatedItem(DevExpress.Utils.Drawing.GraphicsCache cache, Rectangle bounds);
		event EventHandler Invalidate;
	}
	public interface ITransitionAnimator : IAnimator, ICloneable {
		void Restart(Image from, Image to, IAnimationParameters parameters);
		event EventHandler Complete;
		string Title { get; }
		void StartAnimation();
		void StopAnimation();
		IEasingFunction EasingFunction { get; set; }
		EasingMode EasingMode { get; set; }
		Image From { get; }
		Image To { get; }
		IAnimationParameters Parameters { get; }
		void SetSkinProvider(Skins.ISkinProvider provider);		
	}
	public abstract class BaseLoadingAnimator : IAnimatedItem, ISupportXtraAnimation, IDisposable {
		Image imageCore;
		bool isDisposing;
		bool disposeImage;
		AnimatedImageHelper imageHelper;
		bool animationInProgressCore;
		Rectangle bounds;
		public BaseLoadingAnimator(Image image) {
			this.imageCore = image;
		}
		public BaseLoadingAnimator(Image image, bool disposeImage)
			: this(image) {
				this.disposeImage = disposeImage;
		}
		protected Rectangle BoundsCore {
			get { return bounds; }
			set { bounds = value; }
		}
		protected Image Image { get { return imageCore; } }
		public virtual void Dispose() {
			if(!isDisposing) {
				isDisposing = true;				
				OnDispose();
				if(disposeImage) Ref.Dispose(ref this.imageCore);
			}
			GC.SuppressFinalize(this);
		}
		protected virtual void OnDispose() {
			StopAnimation();
			if(imageHelper != null)
				imageHelper.Image = null;
			this.imageHelper = null;
		}
		protected virtual void OnStart() { }
		protected virtual void OnStop() { }
		protected virtual System.Windows.Forms.Control Owner { get { return null; } }
		#region IAnimatedItem Members
		Rectangle IAnimatedItem.AnimationBounds {
			get { return BoundsCore; }
		}
		int IAnimatedItem.AnimationInterval {
			get { return GetAnimationInterval(); }
		}
		protected virtual int GetAnimationInterval() { return ImageHelper.AnimationInterval; }
		int[] IAnimatedItem.AnimationIntervals {
			get { return ImageHelper.AnimationIntervals; }
		}
		AnimationType IAnimatedItem.AnimationType {
			get { return ImageHelper.AnimationType; }
		}
		public virtual int FramesCount {
			get { return ImageHelper.FramesCount; }
			set { }
		}
		int IAnimatedItem.GetAnimationInterval(int frameIndex) {
			return ImageHelper.GetAnimationInterval(frameIndex);
		}
		bool IAnimatedItem.IsAnimated {
			get { return ImageHelper.IsAnimated; }
		}
		void IAnimatedItem.OnStart() { }
		void IAnimatedItem.OnStop() { }
		object IAnimatedItem.Owner {
			get { return this; }
		}
		void IAnimatedItem.UpdateAnimation(BaseAnimationInfo info) {
			ImageHelper.UpdateAnimation(info);
		}
		#endregion
		protected AnimatedImageHelper ImageHelper {
			get {
				if(imageHelper == null && !isDisposing)
					imageHelper = CreateAnimatedImageHelper(imageCore);
				return imageHelper;
			}
		}
		protected virtual AnimatedImageHelper CreateAnimatedImageHelper(Image image) {
			return new AnimatedImageHelper(imageCore);
		}
		#region ISupportXtraAnimation Members
		bool ISupportXtraAnimation.CanAnimate {
			get { return AnimationInProgress && ImageHelper.FramesCount > 1; }
		}
		System.Windows.Forms.Control ISupportXtraAnimation.OwnerControl {
			get { return Owner; }
		} 
		#endregion
		public virtual void DrawAnimatedItem(GraphicsCache cache, Rectangle bounds) {
			if(!animationInProgressCore) {
				this.animationInProgressCore = true;
				StartAnimation();
			}
			Image image = ImageHelper.Image;
			Size imageSize = image.Size;
			if(bounds.Width < imageSize.Width || bounds.Height < imageSize.Height) {
				Size newSize = imageSize;
				double p;
				if(imageSize.Width > imageSize.Height)
					p = (double)imageSize.Width / (double)imageSize.Height;
				else
					p = (double)imageSize.Height / (double)imageSize.Width;
				newSize.Height = (int)((double)bounds.Width / p);
				newSize.Width = (int)((double)bounds.Height / p);
				imageSize = newSize;
			}
			if(bounds.Width > imageSize.Width) {
				bounds.X += (bounds.Width - imageSize.Width) / 2;
				bounds.Width = imageSize.Width;
			}
			if(bounds.Height > imageSize.Height) {
				bounds.Y += (bounds.Height - imageSize.Height) / 2;
				bounds.Height = imageSize.Height;
			}
			cache.Paint.DrawImage(cache.Graphics, image, bounds);
			BoundsCore = bounds;
		}
		public bool AnimationInProgress {
			get { return animationInProgressCore; }
		}
		protected void SetAnimationInProgress(bool value) {
			animationInProgressCore = value;
		} 
		public virtual void StopAnimation() {
			if(!AnimationInProgress) return;
			this.animationInProgressCore = false;
			XtraAnimator.RemoveObject(this);
			OnStop();
		}
		public virtual void StartAnimation() {
			IAnimatedItem animItem = this;
			if(animItem.FramesCount < 2) return;
			this.animationInProgressCore = true;			
			XtraAnimator.Current.AddEditorAnimation(null, this, animItem, new CustomAnimationInvoker(OnImageAnimation));
			OnStart();
		}
		public void RestartAnimation() {
			StopAnimation();
			StartAnimation();
		}
		protected abstract void OnImageAnimation(BaseAnimationInfo info);
	}
	public abstract class BaseTransition : ITransitionAnimator, ISupportXtraAnimation {
		Image imageFromCore;
		Image imageToCore;
		IAnimationParameters animationParametersCore;
		Action restartRequest;
		bool isDisposing;
		bool animationInProgressCore;
		double progress;
		IEasingFunction easingFunctionCore;
		IEasingFunction defaultEasingFunction;
		EasingMode easyModeCore;
		static object syncObj = new object();
		DevExpress.Skins.ISkinProvider provider;
		public BaseTransition() {
			easyModeCore = DefaultEasingMode;
		}
		public BaseTransition(Image from, Image to, IAnimationParameters parameters)
			: this() {			
			this.animationParametersCore = parameters;
			this.imageFromCore = from;
			this.imageToCore = to;
		}
		protected virtual Color GetBackground(Color defColor) {
			if(Provider == null || defColor != Color.Empty) return defColor;
			Skins.Skin skin = Skins.CommonSkins.GetSkin(Provider);
			return skin.Colors.GetColor(Skins.CommonColors.Control);
		}
		[Browsable(false)]
		public Image From { get { return imageFromCore; } }
		[Browsable(false)]
		public Image To { get { return imageToCore; } }
		[
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public IAnimationParameters Parameters {
			get {
				if(animationParametersCore == null)
					animationParametersCore = CreateAnimationParameters();
				return animationParametersCore;
			}
		}
		protected DevExpress.Skins.ISkinProvider Provider { get { return provider; } }
		public void Dispose() {
			if(!isDisposing) {
				isDisposing = true;
				OnDispose();
			}
			GC.SuppressFinalize(this);
		}
		protected virtual IEasingFunction DefaultEasingFunction {
			get {
				if(defaultEasingFunction == null)
					defaultEasingFunction = new ExponentialEase();
				return defaultEasingFunction;
			}
		}
		protected virtual void OnDispose() {
			StopAnimation();
			Ref.Dispose(ref imageFromCore);
			Ref.Dispose(ref imageToCore);
			animationParametersCore = null;
			defaultEasingFunction = null;
			easingFunctionCore = null;
			Invalidate = null;
			Complete = null;
		}
		protected virtual IAnimationParameters CreateAnimationParameters() { return new AnimationParameters(); }
		protected abstract void OnAnimationCore(BaseAnimationInfo info, double current);
		protected abstract void DrawCore(GraphicsCache cache, Rectangle bounds);
		protected void RaiseInvalidate() {
			if(Invalidate != null)
				Invalidate(this, EventArgs.Empty);
		}
		protected void RaiseComplete() {
			if(Complete != null)
				Complete(this, EventArgs.Empty);
		}
		protected virtual void RestartCore(Image from, Image to, IAnimationParameters parameters) {
			if(imageFromCore != from && imageFromCore != to)
				Ref.Dispose(ref imageFromCore);
			if(imageToCore != from && imageToCore != to)
				Ref.Dispose(ref imageToCore);
			this.animationParametersCore = parameters;
			this.imageFromCore = from;
			this.imageToCore = to;
		}
		protected double CalcEasingFunction() {
			if(easingFunctionCore == null && DefaultEasingFunction == null) return progress;
			return EaseHelper.Ease(easyModeCore, easingFunctionCore ?? DefaultEasingFunction, progress);
		}
		void OnAnimation(BaseAnimationInfo info) {
			if(!AnimationInProgress || isDisposing) return;
			progress += ((double)(info.CurrentFrame - (info.PrevFrame < 0 ? 0 : info.PrevFrame)) / (double)info.FrameCount);
			OnAnimationCore(info, CalcEasingFunction());
			RaiseInvalidate();
			if(info.IsFinalFrame) {
				animationInProgressCore = false;
				RaiseComplete();
				lock(syncObj) {
					if(restartRequest != null) {
						restartRequest();
						StartAnimation();
						restartRequest = null;
					}
				}
				return;
			}
		}
		#region IAnimator Members
		void IAnimator.DrawAnimatedItem(GraphicsCache cache, Rectangle bounds) {
			if(From == null || To == null) return;
			if(!animationInProgressCore)
				StartAnimation();
			DrawCore(cache, bounds);
		}
		public event EventHandler Invalidate;
		[Browsable(false)]
		public bool AnimationInProgress { get { return animationInProgressCore; } }
		#endregion
		#region ISupportXtraAnimation Members
		bool ISupportXtraAnimation.CanAnimate { get { return true; } }
		System.Windows.Forms.Control ISupportXtraAnimation.OwnerControl { get { return null; } }
		#endregion
		#region ITransitionAnimator Members
		IEasingFunction ITransitionAnimator.EasingFunction {
			get { return easingFunctionCore; }
			set { easingFunctionCore = value; }
		}
		EasingMode ITransitionAnimator.EasingMode {
			get { return easyModeCore; }
			set { easyModeCore = value; }
		}
		protected virtual EasingMode DefaultEasingMode { get { return EasingMode.EaseOut; } }
		public void Restart(Image from, Image to, IAnimationParameters parameters) {			
			RestartCore(from, to, parameters);			
		}
		public void StartAnimation() {
			progress = 0.0;
			animationInProgressCore = true;
			XtraAnimator.Current.AddObject(this, this, (int)Parameters.FrameInterval, (int)Parameters.FrameCount, OnAnimation);
			OnStartAnimation();
		}
		protected virtual void OnStartAnimation() { }
		protected virtual void OnStopAnimation() { }
		public void StopAnimation() {
			progress = 0.0;
			animationInProgressCore = false;
			XtraAnimator.RemoveObject(this);
			OnStopAnimation();
		}
		public event EventHandler Complete;
		string ITransitionAnimator.Title {
			get { return GetTitle(); }
		}
		protected virtual string GetTitle() { return string.Empty; }
		void ITransitionAnimator.SetSkinProvider(Skins.ISkinProvider provider) {
			this.provider = provider;
		}
		#endregion
		const byte AC_SRC_OVER = 0x00;
		protected void DrawImage_AlphaBlend(IntPtr hDC, Image image, Rectangle dest, Rectangle source, byte opacity) {
			IntPtr srcDC = DevExpress.Utils.Drawing.Helpers.NativeMethods.CreateCompatibleDC(hDC);
			IntPtr tmp = IntPtr.Zero;
			IntPtr hBitmap = ((Bitmap)image).GetHbitmap();							
			try {
				tmp = DevExpress.Utils.Drawing.Helpers.NativeMethods.SelectObject(srcDC, hBitmap);
				DevExpress.Utils.Drawing.Helpers.NativeMethods.AlphaBlend(
					hDC, dest.X, dest.Y, dest.Width, dest.Height,
					srcDC, source.X, source.Y, source.Width, source.Height,
					new DevExpress.Utils.Drawing.Helpers.NativeMethods.BLENDFUNCTION() { SourceConstantAlpha = opacity, BlendOp = AC_SRC_OVER });
			}
			finally {
				DevExpress.Utils.Drawing.Helpers.NativeMethods.SelectObject(srcDC, tmp);
				DevExpress.Utils.Drawing.Helpers.NativeMethods.DeleteObject(hBitmap);
				DevExpress.Utils.Drawing.Helpers.NativeMethods.DeleteDC(srcDC);
			}
		}
		const int SRCCOPY = 0xCC0020;
		protected void DrawImage_BitBlt(IntPtr hDC, Image image, Rectangle dest, Rectangle source) {
			IntPtr srcDC = DevExpress.Utils.Drawing.Helpers.NativeMethods.CreateCompatibleDC(hDC);
			IntPtr tmp = IntPtr.Zero;
			IntPtr hBitmap = ((Bitmap)image).GetHbitmap();
			try {
				tmp = DevExpress.Utils.Drawing.Helpers.NativeMethods.SelectObject(srcDC, hBitmap);
				DevExpress.Utils.Drawing.Helpers.NativeMethods.BitBlt(
					hDC, dest.X, dest.Y, dest.Width, dest.Height,
					srcDC, source.X, source.Y, SRCCOPY);
			}
			finally {
				DevExpress.Utils.Drawing.Helpers.NativeMethods.SelectObject(srcDC, tmp);
				DevExpress.Utils.Drawing.Helpers.NativeMethods.DeleteObject(hBitmap);
				DevExpress.Utils.Drawing.Helpers.NativeMethods.DeleteDC(srcDC);
			}
		}
		object ICloneable.Clone() {
			return this.MemberwiseClone();
		}
	}
}
