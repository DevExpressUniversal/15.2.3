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
using System.Drawing.Imaging;
using System.Windows.Forms;
using DevExpress.LookAndFeel;
using DevExpress.Skins;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.Utils.Drawing.Animation;
namespace DevExpress.XtraEditors.Drawing {
	public class LoadingPanel : IDisposable {
		Control owner;
		Rectangle bounds, progressBounds;
		bool isActive;
		UserLookAndFeel lookAndFeel;
		LoadingAnimator animator;
		public LoadingPanel(Control owner) {
			this.lookAndFeel = UserLookAndFeel.Default;
			this.owner = owner;
			this.progressBounds = this.bounds = Rectangle.Empty;
			this.isActive = false;
		}
		public virtual void Dispose() {
			DestroyAnimator();
			LookAndFeel = UserLookAndFeel.Default;
		}
		protected void DestroyAnimator() {
			Stop();
			if(animator != null) animator.Dispose();
			this.animator = null;
		}
		public bool IsOnlyLoadingPanelDraw(GraphicsCache cache) {
			if(Bounds.IsEmpty || !IsActive) return false;
			if(cache.PaintArgs == null) return false;
			if(cache.PaintArgs.ClipRectangle == Animator.InvalidateBounds || Animator.IsOnlyAnimatorDraw(cache)) return true;
			return false;
		}
		protected LoadingAnimator Animator {
			get {
				if(animator == null) animator = new LoadingAnimator(owner,  GetImage());
				return animator;
			}
		}
		protected virtual Image GetImage() {
			SkinElement element = CommonSkins.GetSkin(LookAndFeel)[CommonSkins.SkinLoadingBig];
			if(element != null && element.Image != null && element.Image.Image != null) return element.Image.Image;
			return LoadingAnimator.LoadingImageBig;
		}
		public UserLookAndFeel LookAndFeel { 
			get { return lookAndFeel; } 
			set {
				if(LookAndFeel == value) return;
				lookAndFeel.StyleChanged -= new EventHandler(lookAndFeel_StyleChanged);
				lookAndFeel = value;
				if(LookAndFeel != UserLookAndFeel.Default)
					lookAndFeel.StyleChanged += new EventHandler(lookAndFeel_StyleChanged);
			} 
		}
		void lookAndFeel_StyleChanged(object sender, EventArgs e) {
			DestroyAnimator();
		}
		public Rectangle Bounds { get { return bounds; } set { bounds = value; } }
		protected Rectangle ProgressBounds { get { return progressBounds; } }
		public bool IsActive {
			get { return isActive; }
		}
		public void Draw(GraphicsCache cache) {
			DrawCore(cache);
			if(!isActive) Start();
			if(ProgressBounds.IsEmpty) return;
			Animator.DrawAnimatedItem(cache, ProgressBounds);
		}
		Size panelSize = new Size(140, 80);
		protected Size CalcPanelSize() { return panelSize; }
		protected Rectangle CalcDrawBounds(GraphicsCache cache) {
			Rectangle rect = Bounds;
			Size min = CalcPanelSize();
			rect.Y += (rect.Height - min.Height) / 2;
			rect.X += (rect.Width - min.Width) / 2;
			if(min.Height > rect.Height || min.Width > rect.Width) return Rectangle.Empty; ;
			rect.Size = min;
			return rect;
		}
		void DrawCore(GraphicsCache cache) {
			Rectangle progress = this.progressBounds = Rectangle.Empty;
			Rectangle drawBounds = Animator.InvalidateBounds = CalcDrawBounds(cache);
			if(drawBounds.IsEmpty) return;
			if(LookAndFeel.ActiveStyle != ActiveLookAndFeelStyle.Skin) {
				progress = DrawNonSkinBackground(cache, drawBounds);
			}
			else {
				progress = DrawSkinBackground(cache, drawBounds);
			}
			this.progressBounds = Rectangle.Inflate(progress, -3, -3);
		}
		Rectangle DrawNonSkinBackground(GraphicsCache cache, Rectangle rect) {
			StyleObjectInfoArgs einfo = new StyleObjectInfoArgs(cache, rect, null);
			ObjectPainter op = LookAndFeel.Painter.Button;
			op.CalcObjectBounds(einfo);
			op.DrawObject(einfo);
			return op.GetObjectClientRectangle(einfo);
		}
		Rectangle DrawSkinBackground(GraphicsCache cache, Rectangle rect) {
			return DrawSkinBackgroundCore(cache, rect, BarSkins.SkinAlertWindow);
		}
		protected Rectangle DrawSkinBackgroundCore(GraphicsCache cache, Rectangle rect, string skinElement) {
			SkinElement element = BarSkins.GetSkin(LookAndFeel)[skinElement];
			SkinElementInfo einfo = new SkinElementInfo(element);
			einfo.Cache = cache;
			einfo.Bounds = rect;
			painter.CalcObjectBounds(einfo);
			painter.DrawObject(einfo);
			return painter.GetObjectClientRectangle(einfo);
		}
		protected Rectangle GetTopElementRectangle(Rectangle rect) {
			return new Rectangle(rect.X, rect.Y - 3, rect.Width, 10);
		}
		void Start() {
			this.isActive = true;
			Animator.Bounds = ProgressBounds;
			Animator.StartAnimation();
		}
		public void Stop() {
			if(!isActive) return;
			this.isActive = false;
			if(this.animator != null) {
				this.animator.StopAnimation();
				owner.Invalidate(animator.InvalidateBounds);
			}
		}
		EmptySkinElementPainter painter = new EmptySkinElementPainter();
		class EmptySkinElementPainter : SkinElementPainter {
			protected override void DrawSkinForeground(SkinElementInfo ee) { }
		}
	}
	public class LoadingAnimator : DevExpress.Utils.Animation.BaseLoadingAnimator {
		Control owner;
		Rectangle invalidateBounds; 
		public static Image LoadingImage { get { return LoadingImages.Image; } }
		public static Image LoadingImageBig { get { return LoadingImages.ImageBig; } }
		public static Image LoadingImageLine { get { return LoadingImages.ImageLine; } }
		public LoadingAnimator(Control owner) : this(owner, LoadingImage) { }
		public LoadingAnimator(Control owner, Image image) : base(image) {
			this.owner = owner;
			this.invalidateBounds = base.BoundsCore = Rectangle.Empty;			
		}
		public Rectangle InvalidateBounds { get { return invalidateBounds; } set { invalidateBounds = value; } }
		public Rectangle Bounds { get { return base.BoundsCore; } set { base.BoundsCore = value; } }
		protected override Control Owner { get { return owner; } }
		protected override void OnStop() {
			base.OnStop();
			Invalidate();
		}
		protected override void OnImageAnimation(BaseAnimationInfo animInfo) {			
			EditorAnimationInfo info = animInfo as EditorAnimationInfo;
			if(!AnimationInProgress) return;
			if(!info.IsFinalFrame) {
				ImageHelper.Image.SelectActiveFrame(FrameDimension.Time, info.CurrentFrame);
				Invalidate();
			}
			else RestartAnimation();
		}
		protected virtual void Invalidate() {
			Owner.Invalidate(InvalidateBounds.IsEmpty ? Bounds : InvalidateBounds);
		}
		public bool IsOnlyAnimatorDraw(GraphicsCache cache) {
			if(Bounds.IsEmpty || !AnimationInProgress) return false;
			if(cache.PaintArgs == null) return false;
			if(cache.PaintArgs.ClipRectangle == Bounds) return true;
			return false;
		}
	}
	public class LoadingAnimationInfo : IDisposable {
		internal DateTime showTime = DateTime.Now, hideTime = DateTime.Now;
		bool visible = false, shown = false;
		int waitAnimationShowDelay = 100, waitAnimationHideDelay = 500;
		public virtual void Dispose() {
		}
		public int WaitAnimationHideDelay {
			get { return waitAnimationHideDelay; }
			set {
				waitAnimationHideDelay = Math.Max(10, value);
			}
		}
		public int WaitAnimationShowDelay {
			get { return waitAnimationShowDelay; }
			set {
				waitAnimationShowDelay = Math.Max(10, value);
			}
		}
		public void HideImmediate() {
			this.visible = false;
			this.shown = false;
			this.hideTime = this.showTime = CurrentTime;
		}
		public bool IsShouldShow {
			get {
				if(!Visible) {
					if(shown && CurrentTime.Subtract(hideTime).TotalMilliseconds < WaitAnimationHideDelay) return true;
					return false;
				}
				if(shown || CurrentTime.Subtract(showTime).TotalMilliseconds > WaitAnimationShowDelay) {
					shown = true;
					return true;
				}
				return false;
			}
		}
		public void CheckVisible(bool loadingInProgress) {
			if(visible != loadingInProgress) {
				OnVisibleChanging(loadingInProgress);
			}
		}
		protected void OnVisibleChanging(bool newVisible) {
			bool shouldShow = IsShouldShow;
			this.visible = newVisible;
			if(newVisible) {
				if(!shouldShow) this.shown = false;
				this.showTime = CurrentTime;
			}
			else {
				this.hideTime = CurrentTime;
				if(!IsShouldShow) this.shown = false;
			}
		}
		public bool Visible { get { return visible; } }
		public DateTime ShowTime { get { return showTime; } }
		public DateTime HideTime { get { return hideTime; } }
		protected internal virtual DateTime CurrentTime { get { return DateTime.Now; } }
	}
}
namespace DevExpress.XtraEditors {
	public enum WaitAnimationOptions { Default, Indicator, Panel };
}
