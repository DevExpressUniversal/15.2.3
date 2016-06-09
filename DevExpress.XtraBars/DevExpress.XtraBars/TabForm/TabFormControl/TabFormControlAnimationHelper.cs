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

using DevExpress.Utils.Drawing.Animation;
using System;
using System.Drawing;
namespace DevExpress.XtraBars {
	public class TabFormPageAnimationInfo : BaseAnimationInfo {
		public TabFormPageAnimationInfo(TabFormPageViewInfo pageInfo, int distance, int ms)
			: base(pageInfo, pageInfo, 10, (int)(TimeSpan.TicksPerMillisecond * ms / 10)) {
			PageInfo = pageInfo;
			SplineHelper = new SplineAnimationHelper();
			SplineHelper.Init(0, 1, 1.0f);
			StartX = CurrentX = PageInfo.CurrentBounds.X;
			EndX = StartX + distance;
			Owner.Handler.Animator.AnimationCount++;
		}
		public TabFormPageViewInfo PageInfo { get; private set; }
		public TabFormControl Owner { get { return AnimatedObject.OwnerControl as TabFormControl; } }
		public int StartX { get; private set; }
		public int CurrentX { get; private set; }
		public int EndX { get; private set; }
		public override void FrameStep() {
			float k = ((float)(CurrentFrame)) / FrameCount;
			FrameStepCore(k);
			Invalidate();
			if(IsFinalFrame) OnAnimationComplete();
		}
		protected virtual void Invalidate() {
			PageInfo.UpdateCurrentLeft(CurrentX);
			Owner.Invalidate();
		}
		protected SplineAnimationHelper SplineHelper { get; private set; }
		protected virtual void FrameStepCore(float k) {
			k = (float)SplineHelper.CalcSpline(k);
			if(IsFinalFrame) k = 1.0f;
			CurrentX = StartX + (int)(k * (EndX - StartX));
		}
		protected virtual void OnAnimationComplete() {
			Owner.Handler.Animator.AnimationCount--;
			Owner.Handler.Animator.RunAnimation();
		}
	}
	public class TabFormAnimator {
		TabFormControlBase owner;
		Rectangle animatedPageRect, currentPageRect;
		int animationCount;
		public TabFormAnimator(TabFormControlBase owner) {
			this.owner = owner;
			this.animationCount = 0;
			ResetAnimationInfo();
		}
		TabFormControlBase Owner { get { return owner; } }
		Rectangle AnimatedPageRect { get { return animatedPageRect; } }
		Rectangle CurrentPageRect { get { return currentPageRect; } }
		public void ResetAnimationInfo() {
			this.animatedPageRect = Rectangle.Empty;
			this.currentPageRect = Rectangle.Empty;
		}
		public void SetAnimationInfo(Rectangle movingPageBounds) {
			this.currentPageRect = movingPageBounds;
			if(AnimatedPageRect == Rectangle.Empty) {
				this.animatedPageRect = movingPageBounds;
			}
		}
		public void RunAnimation() {
			if(AnimatedPageRect.Equals(CurrentPageRect))
				return;
			TabFormPageViewInfo movingPageInfo = Owner.Handler.MovingPageInfo;
			if(movingPageInfo == null || Owner.IsInAnimation)
				return;
			foreach(TabFormPageViewInfo pageInfo in Owner.ViewInfo.PageInfos) {
				if(pageInfo.Equals(movingPageInfo)) continue;
				RunPageAnimation(movingPageInfo, pageInfo);
			}
			this.animatedPageRect = CurrentPageRect;
		}
		internal int AnimationCount {
			get { return animationCount; }
			set {
				animationCount = value;
				if(animationCount == 0)
					Owner.IsInAnimation = false;
				else Owner.IsInAnimation = true;
			}
		}
		public void RunPageAnimation(TabFormPageViewInfo movingPageInfo, TabFormPageViewInfo pageInfo) {
			if(ShouldMovePageRight(movingPageInfo, AnimatedPageRect, pageInfo)) {
				RunPageAnimationCore(pageInfo, movingPageInfo.CurrentBounds.Width + DistanceBetweenPages);
			}
			else if(ShouldMovePageLeft(movingPageInfo, AnimatedPageRect, pageInfo)) {
				RunPageAnimationCore(pageInfo, -(movingPageInfo.CurrentBounds.Width + DistanceBetweenPages));
			}
		}
		public void RunPageAnimationCore(TabFormPageViewInfo pageInfo, int distance) {
			if(Owner.AllowTabAnimation)
				XtraAnimator.Current.AddAnimation(new TabFormPageAnimationInfo(pageInfo, distance, 100));
			else {
				pageInfo.UpdateCurrentLeft(pageInfo.CurrentBounds.Left + distance);
			}
		}
		bool ShouldMovePageLeft(TabFormPageViewInfo movingPageInfo, Rectangle movingPagePrevBounds, TabFormPageViewInfo pageInfo) {
			if(!CanMovePage(pageInfo, true))
				return false;
			if(pageInfo.CurrentBounds.X == pageInfo.Bounds.X) {
				if(movingPageInfo.Bounds.X > pageInfo.Bounds.X) return false;
			}
			else if(movingPageInfo.Bounds.X < pageInfo.Bounds.X) return false;
			return PageMovedAcrossPage(movingPagePrevBounds, movingPageInfo.CurrentBounds, pageInfo.CurrentBounds, false);
		}
		bool ShouldMovePageRight(TabFormPageViewInfo movingPageInfo, Rectangle movingPagePrevBounds, TabFormPageViewInfo pageInfo) {
			if(!CanMovePage(pageInfo, false))
				return false;
			if(pageInfo.CurrentBounds.X == pageInfo.Bounds.X) {
				if(movingPageInfo.Bounds.X < pageInfo.Bounds.X) return false;
			}
			else if(movingPageInfo.Bounds.X > pageInfo.Bounds.X) return false;
			return PageMovedAcrossPage(movingPagePrevBounds, movingPageInfo.CurrentBounds, pageInfo.CurrentBounds, true);
		}
		bool CanMovePage(TabFormPageViewInfo pageInfo, bool left) {
			if(left) {
				if(pageInfo.CurrentBounds.X < pageInfo.Bounds.X) return false;
				if(pageInfo.CurrentBounds.X > pageInfo.Bounds.X) return true;
				if(Owner.IsRightToLeft)
					return Owner.ViewInfo.PageInfos.IndexOf(pageInfo) != Owner.Pages.Count - 1;
				return Owner.ViewInfo.PageInfos.IndexOf(pageInfo) != 0;
			}
			if(pageInfo.CurrentBounds.X > pageInfo.Bounds.X) return false;
			if(pageInfo.CurrentBounds.X < pageInfo.Bounds.X) return true;
			if(Owner.IsRightToLeft)
				return Owner.ViewInfo.PageInfos.IndexOf(pageInfo) != 0;
			return Owner.ViewInfo.PageInfos.IndexOf(pageInfo) != Owner.Pages.Count - 1;
		}
		bool PageMovedAcrossPage(Rectangle prevBounds, Rectangle currentBounds, Rectangle pageBounds, bool rightToLeft) {
			int minWidth = Math.Min(currentBounds.Width, pageBounds.Width);
			if(rightToLeft) {
				if(prevBounds.X < currentBounds.X)
					return false;
				return pageBounds.Left + minWidth / 2 > currentBounds.X;
			}
			if(prevBounds.X > currentBounds.X)
				return false;
			return pageBounds.Right - minWidth / 2 < currentBounds.Right;
		}
		int DistanceBetweenPages { get { return Owner.ViewInfo.GetDistanceBetweenTabs(); } }
	}
}
