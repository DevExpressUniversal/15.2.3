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
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DevExpress.LookAndFeel;
using DevExpress.Utils.Base;
using DevExpress.Utils.Drawing;
using DevExpress.Utils.Drawing.Animation;
using DevExpress.Utils.Drawing.Helpers;
using DevExpress.Utils.VisualEffects;
namespace DevExpress.Utils.Animation {
	public abstract class DotAnimator : LoadingAnimator, ISupportXtraAnimation {
		List<BaseProgressDotInfoArgs> progressDotListCore;
		Color colorCore;
		Image dotImageCore;
		static readonly object invalidate = new object();
		double progressCore;
		int frameIntervalCore;
		int frameCountCore;
		double accelerationCore;
		double initialSpeedCore;
		int animationElementNumberCore;
		BaseProgressDotPainter painterCore;
		public List<BaseProgressDotInfoArgs> ProgressDotList { get { return progressDotListCore; }
			set { progressDotListCore = value; }
		}
		public double Acceleration {
			get { return accelerationCore; }
			set { accelerationCore = value; }
		}
		public double InitialSpeed {
			get { return initialSpeedCore; }
			set { initialSpeedCore = value; }
		}
		public int AnimationElementNumber {
			get { return animationElementNumberCore; }
			set { animationElementNumberCore = value; }
		}
		public virtual Color Color {
			get { return colorCore; }
			set { colorCore = value; }
		}
		public AnimationType AnimationType {
			get { return Drawing.Animation.AnimationType.Cycle; }
		}
		public Image DotImage {
			get { return dotImageCore; }
			set {
				dotImageCore = value;
			}
		}
		public double Progress { 
			get { return progressCore; }
			set { progressCore = value; }
		}
		public BaseProgressDotPainter Painter {
			get { return painterCore; }
		}
		Rectangle boundsCore;
		public Rectangle Bounds {
			get { return boundsCore; }
			set { boundsCore = value; }
		}
		public DotAnimator()
			: base(null) {
			painterCore = CreatePainter();
			accelerationCore = 7.0;
			initialSpeedCore = 9.0;
			colorCore = Color.Empty;
			frameIntervalCore = 1000;
			frameCountCore = 38000;
			dotImageCore = null;
			animationElementNumberCore = 5;
		}
		protected virtual BaseProgressDotPainter CreatePainter() {
			return new BaseProgressDotPainter();
		}
		public override void StartAnimation() {
			IAnimatedItem animItem = this;
			progressCore = 0.0;
			InitProgressDotList(Bounds);
			CustomAnimationInfo info = new CustomAnimationInfo(this, this, (int)AnimationInterval, (int)FramesCount, AnimationType, OnImageAnimation);
			XtraAnimator.Current.AddAnimation(info);
			OnStart();
		}
		public override void StopAnimation() {
			base.StopAnimation();
			boundsCore = Rectangle.Empty;
		}
		protected override void OnImageAnimation(BaseAnimationInfo info) { }
		protected internal virtual void InitProgressDotList(Rectangle bounds) {
			progressDotListCore = new List<BaseProgressDotInfoArgs>();
			for(int i = 0; i < 5; i++) {
				BaseProgressDotInfoArgs progressDot = new BaseProgressDotInfoArgs(Acceleration, InitialSpeed, DotImage);
				progressDot.CalcPeriod(bounds);
				progressDotListCore.Add(progressDot);
			}
		}
		public override void DrawAnimatedItem(GraphicsCache cache, Rectangle bounds) {
			if(Color == Color.Empty)
				DrawAnimatedItem(cache, bounds, Color.Black);
			else
				DrawAnimatedItem(cache, bounds, Color);
		}
		public virtual void DrawAnimatedItem(GraphicsCache cache, Rectangle bounds, Color color) {
			if(!AnimationInProgress) {
				StartAnimation();
				SetAnimationInProgress(true);
			}
			else {
				GraphicsState savedState = cache.Graphics.Save();
				try {
					cache.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
					if(progressDotListCore != null)
						foreach(BaseProgressDotInfoArgs progressDot in progressDotListCore) {
							progressDot.Color = color;
							ObjectPainter.DrawObject(cache, Painter, progressDot);
						}
				}
				finally {
					cache.Graphics.Restore(savedState);
				}
			}
		}
		public virtual Size GetMinSize() {
			return new Size(40, 40);
		}
		protected override void OnStop() {
			progressDotListCore.Clear();
			base.OnStop();
		}
		#region IAnimatedItem Members
		public Rectangle AnimationBounds {
			get { return Bounds; }
		}
		public int GetAnimationInterval(int frameIndex) {
			return 0;
		}
		public int[] AnimationIntervals {
			get { return null; }
		}
		public int AnimationInterval {
			get { return frameIntervalCore; }
			set { frameIntervalCore = value; }
		}
		public override int FramesCount {
			get { return frameCountCore; }
			set { frameCountCore = value; }
		}
		public bool IsAnimated {
			get { return true; }
		}
		public void UpdateAnimation(BaseAnimationInfo info) { }
		#endregion
		#region ISupportXtraAnimation Members
		public Control OwnerControl {
			get { return null; }
		}
		public bool CanAnimate {
			get { return true; }
		}
		#endregion
		protected virtual Rectangle GetBounds() {
			return Rectangle.Empty;
		}
	}
	public class BaseProgressDotInfoArgs : ObjectInfoArgs {
		internal double Acceleration { get; set; }
		internal double AccelerationTime { get; set; }
		internal bool AllowDraw { get; set; }
		internal Color Color { get; set; }
		internal double InitialSpeed { get; set; }
		internal Image Image { get; set; }
		const int scalingDistance = 100;
		protected int ScalingDistance { get { return scalingDistance; } }
		internal double Period { get; set; }
		public BaseProgressDotInfoArgs(double acceleration, double initialSpeed, Image image) {
			Acceleration = acceleration;
			InitialSpeed = initialSpeed;
			Color = Color.White;
			Image = image;
		}
		public virtual void Calc(Rectangle bounds, double frameCount) { }
		public virtual void CalcPeriod(Rectangle bounds) { }
	}
	public class DotAnimatorPainter : WaitingIndicatorPainter {
		DotAnimator animatorCore;
		public DotAnimatorPainter(DotAnimator animator) {
			animatorCore = animator;
		}
		protected DotAnimator Animator { get { return animatorCore; } }
		protected override AppearanceDefault CreateDefaultAppearance() {
			return new AppearanceDefault();
		}
		public override void DrawObject(ObjectInfoArgs e) {
			e.Cache.Graphics.Clear(Color.Empty);
			animatorCore.DrawAnimatedItem(e.Cache, Animator.Bounds);
		}
	}
	public class BaseProgressDotPainter : ObjectPainter {
		public override void DrawObject(ObjectInfoArgs e) {
			base.DrawObject(e);
			DrawProgressDot(e.Cache, e as BaseProgressDotInfoArgs);
		}
		public virtual void DrawProgressDot(GraphicsCache cache, BaseProgressDotInfoArgs info) { }
	}
}
