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
using DevExpress.XtraBars.Forms;
using System.Collections;
namespace DevExpress.XtraBars.Controls {
	public class Animator {
		static bool allowFadeAnimation = true;
		public static bool AllowFadeAnimation { 
			get { return allowFadeAnimation; } 
			set { allowFadeAnimation = value; } 
		}
		Form form;
		Control childControl;
		Size originalSize;
		SizeF deltaSize, currentSize;
		PointF childPosition;
		Point originalChildPosition;
		DockStyle childDockStyle;
		AnimationType animation, systemAnimation;
		AnimationDirection direction;
		int totalTime;
		int iterationTimeOut;
		int iterationCount = 50;
		bool allowSystemAnimation, requireFormPaint;
		public Animator(Control mainControl, Form form, Control childControl) {
			this.form = form;
			this.childControl = childControl;
			this.originalSize = Form.ClientSize;
			this.childDockStyle = ChildControl.Dock;
			this.originalChildPosition = ChildControl.Location;
			this.systemAnimation = this.animation = AnimationType.None;
			this.direction = AnimationDirection.None;
			this.totalTime = 150;
			CheckSystemAnimation();
			if(mainControl != null) mainControl.Update();
		}
		public Point ChildPosition { get { return PointF2Point(childPosition); } }
		public bool RequireFormPaint { get { return requireFormPaint; } }
		protected virtual void CheckSystemAnimation() {
			int val = 0;
			BarNativeMethods.SystemParametersInfo(SPI_GETMENUANIMATION, 0, ref val, 0);
			this.systemAnimation = (val != 0 ? AnimationType.Fade : AnimationType.None);
			if(SystemAnimation == AnimationType.None) return;
			this.allowSystemAnimation = (val != 0);
			BarNativeMethods.SystemParametersInfo(SPI_GETMENUFADE, 0, ref val, 0);
			if (val == 0) 
				this.systemAnimation = AnimationType.Unfold;
		}
		const uint SPI_GETMENUFADE = 0x1012, SPI_GETMENUANIMATION = 0x1002;
		public virtual void Animate(AnimationType type) {
			this.animation = type;
			this.direction = AnimationDirection.None;
			AnimateCore();
		}
		protected virtual void AnimateCore() {
			ChildControl.Dock = DockStyle.None;
			try {
				DoAnimate();
			}
			finally {
				ChildControl.Dock = childDockStyle;
				ChildControl.Location = originalChildPosition;
			}
		}
		protected virtual void DoAnimate() {
			AnimationType anim = Animation;
			if(anim == AnimationType.System) anim = SystemAnimation;
			if(anim == AnimationType.Random) {
				anim = (AnimationType)(new Random().Next(2, Enum.GetValues(typeof(AnimationType)).Length - 1));
			}
			if(anim == AnimationType.Fade) {
				iterationCount = 255;
				totalTime += 100; 
			}
			iterationTimeOut = (totalTime * 1000) / iterationCount;
			switch(anim) {
				case AnimationType.Unfold :
					this.direction = AnimationDirection.HorzInc | AnimationDirection.VertInc;
					DoSlideAnimation();
					break;
				case AnimationType.Slide : 
					this.direction = AnimationDirection.VertInc;
					DoSlideAnimation(); 
					break;
				case AnimationType.Fade : 
					DoFadeAnimation(); 
					break;
			}
			Form.Visible = true;
		}
		protected void UpdateSlideDeltaSize() {
			if(this.iterationCount < 1) this.iterationCount = 1;
			deltaSize = SizeF.Empty;
			deltaSize.Width = (float)originalSize.Width / (float)iterationCount;
			deltaSize.Height = (float)originalSize.Height / (float)iterationCount;
			if((Direction & AnimationDirection.Horz) == 0) {
				deltaSize.Width = 0;
			}
			if((Direction & AnimationDirection.Vert) == 0) {
				deltaSize.Height = 0;
			}
		}
		protected virtual void DoSlideAnimation() {
			if(ChildControl != null) ChildControl.Visible = false;
			this.requireFormPaint = true;
			try {
				DoSlideAnimationCore();
			}
			finally {
				this.requireFormPaint = false;
				if(ChildControl != null) ChildControl.Visible = true;
			}
		}
		protected virtual void DoSlideAnimationCore() {
			Size minSize = Form.MinimumSize;
			if(!(Form is ControlForm)) {
				Form.MinimumSize = Size.Empty;
				Form.MinimumSize = new Size(2, 2);
			}
			currentSize = SizeF.Empty;
			childPosition = originalChildPosition;
			childPosition.X = childPosition.X - ChildControl.Size.Width;
			childPosition.Y = childPosition.Y - ChildControl.Size.Height;
			if((Direction & AnimationDirection.Horz) == 0) {
				currentSize.Width = OriginalSize.Width;
				childPosition.X = originalChildPosition.X;
			}
			if((Direction & AnimationDirection.Vert) == 0) {
				currentSize.Height = OriginalSize.Height;
				childPosition.Y = originalChildPosition.Y;
			}
			UpdateSlideDeltaSize();
			Form.ClientSize = currentSize.ToSize();
			ChildControl.Location = ChildPosition;
			Form.Visible = true;
			int startIterationCount = iterationCount;
			for(int n = 0; n < iterationCount; n++) {
				currentSize.Width += deltaSize.Width;
				currentSize.Height += deltaSize.Height;
				childPosition.X = Math.Min(originalChildPosition.X, childPosition.X + deltaSize.Width);
				childPosition.Y = Math.Min(originalChildPosition.Y, childPosition.Y + deltaSize.Height);
				currentSize.Width = Math.Min(OriginalSize.Width, currentSize.Width);
				currentSize.Height = Math.Min(OriginalSize.Height, currentSize.Height);
				long ticks = DateTime.Now.Ticks;
				Form.ClientSize = currentSize.ToSize();
				if(Form.ClientSize.Width > 0 && Form.ClientSize.Height > 0)
					Form.Update();
				ticks = DateTime.Now.Ticks - ticks;
				if(ticks < iterationTimeOut)
					Thread.Sleep(new TimeSpan(iterationTimeOut - ticks)); 
				else {
					if(n < startIterationCount - 1) {
						iterationCount = Math.Max(iterationCount - (Math.Max(1, (int)((ticks - iterationTimeOut) / 50000))), 1);
						UpdateSlideDeltaSize();
					}
				}
			}
			Form.ClientSize = OriginalSize;
			ChildControl.Location = originalChildPosition;
			if(!(Form is ControlForm)) Form.MinimumSize = minSize;
		}
		Point PointF2Point(PointF fl) { return new Point((int)fl.X, (int)fl.Y); }
		protected virtual Size OriginalSize { get { return originalSize; } }
		protected virtual void DoFadeAnimation() {
			if(!AllowFadeAnimation) {
				Form.Visible = true;
				return;
			}
			double prevOpacity = Form.Opacity, inter;
			bool prevAllowTransparency = Form.AllowTransparency;
			Form.AllowTransparency = true;
			Form.Opacity = 0;
			Form.Visible = true;
			Form.Update();
			try {
				int startIterationCount = iterationCount;
				for(int n = 0; n < iterationCount; n++) {
					inter = prevOpacity / ((double)iterationCount);
					long ticks = DateTime.Now.Ticks;
					Form.Opacity = Math.Min(1, (1f / iterationCount) * n);
					ticks = DateTime.Now.Ticks - ticks;
					if (ticks < iterationTimeOut) {
						Thread.Sleep(new TimeSpan(iterationTimeOut - ticks));
					}
					else {
						if(n < startIterationCount - 1) {
							if(ticks / (float)iterationTimeOut < 1.5f) continue; 
							iterationCount = Math.Max(iterationCount - (Math.Max(1, (int)((ticks - iterationTimeOut) / 7000))), 1);
						}
					}
				}
			}
			finally {
				Form.AllowTransparency = prevAllowTransparency;
				Form.Opacity = prevOpacity;
			}
		}
		public AnimationType Animation { get { return animation; } }
		public AnimationType SystemAnimation { get { return systemAnimation; } }
		public AnimationDirection Direction { get { return direction; } }
		public int AnimationTime { get { return totalTime; } }
		public Form Form { get { return form; } }
		public Control ChildControl { get { return childControl; } }
	}
	[Flags]
	public enum AnimationDirection { None = 0, HorzInc = 1, VertInc = 2, Horz = 1, Vert = 2 };
}
