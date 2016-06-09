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
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
namespace DevExpress.XtraBars.Navigation {
	public class AccordionControlAnimationInfo : BaseAnimationInfo {
		public AccordionControlAnimationInfo(ISupportXtraAnimation anim, object animationId, AccordionElementBaseViewInfo collapsingItem, bool expanding, int ms)
			: base(anim, animationId, 10, (int)(TimeSpan.TicksPerMillisecond * ms / 10)) {
				Item = anim as AccordionElementBaseViewInfo;
				Item.IsInAnimation = true;
				Item.ControlInfo.IsInAnimation = true;
				CollapsingItem = collapsingItem;
				int addedAnimationHeight = 0;
				if(Item is AccordionGroupViewInfo) {
					AccordionGroupViewInfo groupInfo = Item as AccordionGroupViewInfo;
					EndHeight = expanding ? groupInfo.CalcGroupExpandedHeight() : 0;
					StartHeight = expanding ? 0 : groupInfo.CalcGroupExpandedHeight();
					addedAnimationHeight = groupInfo.CalcGroupExpandedHeight();
				}
				else {
					EndHeight = expanding ? Item.ContentContainerHeight : 0;
					StartHeight = expanding ? 0 : Item.ContentContainerHeight;
					addedAnimationHeight = Item.ContentContainerHeight;
				}
				if(CollapsingItem != null) {
					AccordionGroupViewInfo collapsingGroup = CollapsingItem as AccordionGroupViewInfo;
					CollapsingItemEndHeight = 0;
					CollapsingItemStartHeight = (collapsingGroup == null ? CollapsingItem.ContentContainerHeight : collapsingGroup.CalcGroupExpandedHeight());
					CollapsingItem.InnerContentHeight = CollapsingItemCurrentHeight = CollapsingItemStartHeight;
					addedAnimationHeight = Math.Max(addedAnimationHeight, CollapsingItemStartHeight);
				}
				Item.ControlInfo.AddedAnimationHeight = addedAnimationHeight;
				Item.InnerContentHeight = CurrentHeight = StartHeight;
				Item.OnStartAnimation();
		}
		public AccordionElementBaseViewInfo Item { get; private set; }
		public AccordionElementBaseViewInfo CollapsingItem { get; private set; }
		public int StartHeight { get; private set; }
		public int CurrentHeight { get; private set; }
		public int EndHeight { get; private set; }
		public int CollapsingItemStartHeight { get; private set; }
		public int CollapsingItemCurrentHeight { get; private set; }
		public int CollapsingItemEndHeight { get; private set; }
		public override void FrameStep() {
			float k = ((float)(CurrentFrame)) / FrameCount;
			FrameStepCore(k);
			Invalidate();
			if(IsFinalFrame)
				OnAnimationComplete();
		}
		protected virtual void Invalidate() {
			if(CollapsingItem != null) {
				if(CollapsingItem.HeaderBounds.Y < Item.HeaderBounds.Y) {
					CollapsingItem.OnAnimation(new SlideAnimationInfo(CollapsingItemCurrentHeight, 0, false));
					Item.OnAnimation(new SlideAnimationInfo(CurrentHeight, CollapsingItemCurrentHeight, true));
					return;
				}
				else {
					Item.OnAnimation(new SlideAnimationInfo(CurrentHeight, CollapsingItemCurrentHeight, true));
					CollapsingItem.OnAnimation(new SlideAnimationInfo(CollapsingItemCurrentHeight, 0, false));
					return;
				}
			}
			Item.OnAnimation(new SlideAnimationInfo(CurrentHeight, 0, true));
		}
		protected virtual void FrameStepCore(float k) {
			if(IsFinalFrame) k = 1.0f;
			CurrentHeight = StartHeight + (int)(k * (EndHeight - StartHeight));
			if(CollapsingItem != null) CollapsingItemCurrentHeight = CollapsingItemStartHeight + (int)(k * (CollapsingItemEndHeight - CollapsingItemStartHeight));
		}
		protected virtual void OnAnimationComplete() {
			Item.ControlInfo.AddedAnimationHeight = 0;
			if(CollapsingItem != null) OnAnimationCompleteCore(CollapsingItem, false);
			OnAnimationCompleteCore(Item, true);
		}
		private void OnAnimationCompleteCore(AccordionElementBaseViewInfo item, bool shouldRefresh) {
			item.IsInAnimation = false;
			item.ControlInfo.IsInAnimation = false;
			item.OnAnimationComplete(shouldRefresh);
		}
	}
	public class SlideAnimationInfo {
		int contentHeight, collapsingItemHeight;
		bool shouldUpdateAnimationInc;
		public SlideAnimationInfo(int contentHeight, int collapsingItemHeight, bool shouldUpdateAnimationInc) {
			this.contentHeight = contentHeight;
			this.collapsingItemHeight = collapsingItemHeight;
			this.shouldUpdateAnimationInc = shouldUpdateAnimationInc;
		}
		public int ContentHeight { get { return contentHeight; } }
		public int CollapsingItemHeight { get { return collapsingItemHeight; } }
		public bool ShouldUpdateAnimationInc { get { return shouldUpdateAnimationInc; } }
	}
	public class AccordionControlSplineAnimationInfo : AccordionControlAnimationInfo {
	   public AccordionControlSplineAnimationInfo(ISupportXtraAnimation anim, object animationId, AccordionElementBaseViewInfo collapsingItem, bool expanding, int ms)
			: base(anim, animationId, collapsingItem, expanding, ms) {
			SplineHelper = new SplineAnimationHelper();
			SplineHelper.Init(0, 1, 1.0f);
		}
		protected SplineAnimationHelper SplineHelper { get; private set; }
		protected override void FrameStepCore(float k) {
			k = (float)SplineHelper.CalcSpline(k);
			base.FrameStepCore(k);
		}
	}
	public class InertiaAnimationInfo : BaseAnimationInfo {
		public InertiaAnimationInfo(ISupportXtraAnimation obj, object animObj, int deltaTick, int frameCount)
			: base(obj, animObj, deltaTick, frameCount, DevExpress.Utils.Drawing.Animation.AnimationType.Cycle) {
			ControlInfo = obj as AccordionControlViewInfo;
			this.sw = new Stopwatch();
			sw.Start();
		}
		Stopwatch sw;
		AccordionControlViewInfo ControlInfo { get; set; }
		static double ticksPerFrame = TimeSpan.TicksPerMillisecond * 20;
		static double decelerationCoefficient = 0.4;
		public override void FrameStep() {
			sw.Stop();
			double time = sw.ElapsedTicks / ticksPerFrame;
			ControlInfo.GestureInertia *= Math.Pow(decelerationCoefficient, time);
			ControlInfo.UpdateContentTop(ControlInfo.ContentTopIndent + (int)ControlInfo.GestureInertia);
			if(Math.Abs(ControlInfo.GestureInertia) > 5) sw.Restart();
			else XtraAnimator.Current.Animations.Remove(this);
		}
	}
	public class AccordionBitmapAnimationInfo : BitmapAnimationInfo {
		public AccordionBitmapAnimationInfo(ISupportXtraAnimation obj, object animId, int length, Rectangle bounds, Bitmap bitmap, BitmapAnimationImageCallback callBack)
			: base(obj, animId, bitmap, callBack, bounds, length) {
			AccordionInfo = obj as AccordionControlViewInfo;
		}
		AccordionControlViewInfo AccordionInfo { get; set; }
		public override void FrameStep() {
			if(AccordionInfo.IsInAnimation) {
				return;
			}
			base.FrameStep();
		}
	}
	public class AccordionControlExpandAnimationInfo : BaseAnimationInfo {
		bool isExpanding;
		public AccordionControlExpandAnimationInfo(ISupportXtraAnimation anim, object animationId, bool expanding, int ms)
			: base(anim, animationId, 10, (int)(TimeSpan.TicksPerMillisecond * ms / 10)) {
			this.isExpanding = expanding;
			AccordionControl = (AccordionControl)anim.OwnerControl;
			StartWidth = CurrentWidth = AccordionControl.Width;
			if(!expanding) {
				EndWidth = AccordionControl.GetMinimizedWidth();
			}
			else EndWidth = AccordionControl.ControlInfo.GetExpandedWidth();
			AccordionControl.ControlInfo.IsInAnimation = true;
			AccordionControl.ControlInfo.IsInMinimizeAnimation = true;
			AccordionControl.ControlInfo.Helper.HideControls();
		}
		bool IsExpanding { get { return isExpanding; } }
		public AccordionControl AccordionControl { get; private set; }
		public int StartWidth { get; private set; }
		public int CurrentWidth { get; private set; }
		public int EndWidth { get; private set; }
		public override void FrameStep() {
			float k = ((float)(CurrentFrame)) / FrameCount;
			FrameStepCore(k);
			Invalidate();
			if(IsFinalFrame) OnAnimationComplete();
		}
		protected virtual void Invalidate() {
			AccordionControl.Width = CurrentWidth;
		}
		protected virtual void FrameStepCore(float k) {
			if(IsFinalFrame) k = 1.0f;
			CurrentWidth = StartWidth + (int)(k * (EndWidth - StartWidth));
		}
		protected virtual void OnAnimationComplete() {
			if(AccordionControl.OptionsMinimizing.State == AccordionControlState.Normal)
				AccordionControl.OptionsMinimizing.State = AccordionControlState.Minimized;
			else AccordionControl.OptionsMinimizing.State = AccordionControlState.Normal;
			AccordionControl.ControlInfo.IsInAnimation = false;
			AccordionControl.ControlInfo.IsInMinimizeAnimation = false;
			AccordionControl.ControlInfo.Helper.CheckControlsVisibility(AccordionControl.Elements);
			AccordionControl.ControlInfo.ClearPaintAppearance();
			AccordionControl.ForceLayoutChanged();
		}
	}
}
