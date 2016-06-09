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
using System.Collections.Generic;
using System.ComponentModel;
namespace DevExpress.XtraScheduler.Native {
	#region AnimationEffect (abstract class)
	public abstract class AnimationEffect {
		public abstract void OnPaint(Graphics gr, float t);
	}
	#endregion
	#region SchedulerAnimationEffect (abstract class)
	public abstract class SchedulerAnimationEffect : AnimationEffect {
		readonly SchedulerControl control;
		readonly SchedulerAnimationInfo prevAnimationInfo;
		readonly SchedulerAnimationInfo currentAnimationInfo;
		protected SchedulerAnimationEffect(SchedulerControl control, SchedulerAnimationInfo prevAnimationInfo, SchedulerAnimationInfo currentAnimationInfo) {
			this.control = control;
			this.prevAnimationInfo = prevAnimationInfo;
			this.currentAnimationInfo = currentAnimationInfo;
		}
		public SchedulerAnimationInfo PrevAnimationInfo { get { return prevAnimationInfo; } }
		public SchedulerAnimationInfo CurrentAnimationInfo { get { return currentAnimationInfo; } }
		public SchedulerControl Control { get { return control; } }
	}
	#endregion
	#region HorizontalScrollEffect
	public class HorizontalScrollEffect : SlottedHorizontalScrollEffect {
		public HorizontalScrollEffect(SchedulerControl control, SchedulerAnimationInfo prevAnimationInfo, SchedulerAnimationInfo currentAnimationInfo)
			: base(control, prevAnimationInfo, currentAnimationInfo) {
			SlotsBounds.Add(Rectangle.Empty);
		}
		#region SlotsBounds
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new List<Rectangle> SlotsBounds { get { return base.SlotsBounds; } }
		#endregion
		#region ScrollBounds
		public Rectangle ScrollBounds {
			get {
				return SlotsBounds[0];
			}
			set {
				SlotsBounds.Clear();
				SlotsBounds.Add(value);
			}
		}
		#endregion
	}
	#endregion
	#region SlottedHorizontalScrollEffect
	public class SlottedHorizontalScrollEffect : SchedulerAnimationEffect {
		bool paintBackground = true;
		bool fadeNonOverlapedArea;
		List<Rectangle> slots;
		int scrollDelta;
		public SlottedHorizontalScrollEffect(SchedulerControl control, SchedulerAnimationInfo prevAnimationInfo, SchedulerAnimationInfo currentAnimationInfo)
			: base(control, prevAnimationInfo, currentAnimationInfo) {
			this.slots = new List<Rectangle>();
		}
		public bool PaintBackground { get { return paintBackground; } set { paintBackground = value; } }
		public List<Rectangle> SlotsBounds { get { return slots; } }
		public int ScrollDelta { get { return scrollDelta; } set { scrollDelta = value; } }
		bool RightToLeft { get { return scrollDelta < 0; } }
		public bool FadeOverlappedArea { get { return fadeNonOverlapedArea; } set { fadeNonOverlapedArea = value; } }
		public override void OnPaint(Graphics gr, float t) {
			int absDelta = Math.Abs(ScrollDelta);
			float accelerationX = -absDelta;
			float velocityX = absDelta - accelerationX;
			int shiftX = (int)Math.Round((velocityX * t + accelerationX * t * t));
			if (PaintBackground)
				gr.DrawImage(PrevAnimationInfo.Bitmap, Point.Empty);
			int count = SlotsBounds.Count;
			for (int i = 0; i < count; i++)
				DrawSlot(SlotsBounds[i], gr, t, absDelta, shiftX);
		}
		void DrawSlot(Rectangle slotBounds, Graphics gr, float t, int absDelta, int shiftX) {
			if (RightToLeft) {
				Rectangle prevViewBounds = slotBounds;
				prevViewBounds.Width -= shiftX;
				prevViewBounds.X += shiftX;
				gr.DrawImage(PrevAnimationInfo.Bitmap, slotBounds.Left, slotBounds.Top, prevViewBounds, GraphicsUnit.Pixel);
				Rectangle newViewBounds = slotBounds;
				newViewBounds.Width = shiftX;
				newViewBounds.X = slotBounds.Right - absDelta;
				gr.DrawImage(CurrentAnimationInfo.Bitmap, slotBounds.Right - shiftX, slotBounds.Top, newViewBounds, GraphicsUnit.Pixel);
				if (FadeOverlappedArea) {
					Rectangle overlappingAreaBounds = CalculateRightOverlappingAreaBounds(slotBounds, absDelta);
					AnimationDrawHelper.DrawImageWithTransparency(gr, CurrentAnimationInfo.Bitmap, t, slotBounds.Left + absDelta - shiftX, slotBounds.Top, overlappingAreaBounds);
				}
			}
			else {
				Rectangle prevViewBounds = slotBounds;
				prevViewBounds.Width -= shiftX;
				gr.DrawImage(PrevAnimationInfo.Bitmap, slotBounds.Left + shiftX, slotBounds.Top, prevViewBounds, GraphicsUnit.Pixel);
				Rectangle newViewBounds = slotBounds;
				newViewBounds.Width = shiftX;
				newViewBounds.X = slotBounds.Left + absDelta - shiftX;
				gr.DrawImage(CurrentAnimationInfo.Bitmap, slotBounds.Left, slotBounds.Top, newViewBounds, GraphicsUnit.Pixel);
				if (FadeOverlappedArea) {
					Rectangle nonOverlappingAreaBounds = CalculateOverlappingAreaBounds(slotBounds, absDelta);
					AnimationDrawHelper.DrawImageWithTransparency(gr, CurrentAnimationInfo.Bitmap, t, slotBounds.Left + shiftX, slotBounds.Top, nonOverlappingAreaBounds);
				}
			}
		}
		Rectangle CalculateOverlappingAreaBounds(Rectangle slotBounds, int leftOffset) {
			Rectangle nonOverlappingAreaBounds = slotBounds;
			nonOverlappingAreaBounds.Width = slotBounds.Width - leftOffset;
			nonOverlappingAreaBounds.X = slotBounds.Left + leftOffset;
			return nonOverlappingAreaBounds;
		}
		Rectangle CalculateRightOverlappingAreaBounds(Rectangle slotBounds, int absDelta) {
			Rectangle nonOverlappingAreaBounds = slotBounds;
			nonOverlappingAreaBounds.Width = slotBounds.Width - absDelta;
			nonOverlappingAreaBounds.X = slotBounds.Left;
			return nonOverlappingAreaBounds;
		}
	}
	#endregion
	#region VerticalScrollEffect
	public class VerticalScrollEffect : SchedulerAnimationEffect {
		bool paintBackground = true;
		Rectangle scrollBounds;
		int scrollDelta;
		public VerticalScrollEffect(SchedulerControl control, SchedulerAnimationInfo prevAnimationInfo, SchedulerAnimationInfo currentAnimationInfo)
			: base(control, prevAnimationInfo, currentAnimationInfo) {
		}
		public bool PaintBackground { get { return paintBackground; } set { paintBackground = value; } }
		public Rectangle ScrollBounds { get { return scrollBounds; } set { scrollBounds = value; } }
		public int ScrollDelta { get { return scrollDelta; } set { scrollDelta = value; } }
		bool RightToLeft { get { return scrollDelta < 0; } }
		public override void OnPaint(Graphics gr, float t) {
			int absDelta = Math.Abs(ScrollDelta);
			float accelerationY = -absDelta;
			float velocityY = absDelta - accelerationY;
			int shiftY = (int)Math.Round((velocityY * t + accelerationY * t * t));
			if (PaintBackground)
				gr.DrawImage(PrevAnimationInfo.Bitmap, Point.Empty);
			if (RightToLeft) {
				Rectangle prevViewBounds = scrollBounds;
				prevViewBounds.Height -= shiftY;
				prevViewBounds.Y += shiftY;
				gr.DrawImage(PrevAnimationInfo.Bitmap, scrollBounds.Left, scrollBounds.Top, prevViewBounds, GraphicsUnit.Pixel);
				Rectangle newViewBounds = scrollBounds;
				newViewBounds.Height = shiftY;
				newViewBounds.Y = scrollBounds.Bottom - absDelta;
				gr.DrawImage(CurrentAnimationInfo.Bitmap, scrollBounds.Left, scrollBounds.Bottom - shiftY, newViewBounds, GraphicsUnit.Pixel);
			}
			else {
				Rectangle prevViewBounds = scrollBounds;
				prevViewBounds.Height -= shiftY;
				gr.DrawImage(PrevAnimationInfo.Bitmap, scrollBounds.Left, scrollBounds.Top + shiftY, prevViewBounds, GraphicsUnit.Pixel);
				Rectangle newViewBounds = scrollBounds;
				newViewBounds.Height = shiftY;
				newViewBounds.Y = scrollBounds.Top + absDelta - shiftY;
				gr.DrawImage(CurrentAnimationInfo.Bitmap, scrollBounds.Left, scrollBounds.Top, newViewBounds, GraphicsUnit.Pixel);
			}
		}
	}
	#endregion
	#region FadeEffect
	public class FadeEffect : SchedulerAnimationEffect {
		public FadeEffect(SchedulerControl control, SchedulerAnimationInfo prevAnimationInfo, SchedulerAnimationInfo currentAnimationInfo)
			: base(control, prevAnimationInfo, currentAnimationInfo) {
		}
		Rectangle FadeBounds { get { return new Rectangle(Point.Empty, Control.Size); } }
		public override void OnPaint(Graphics gr, float t) {
			AnimationDrawHelper.DrawImageWithTransparency(gr, PrevAnimationInfo.Bitmap, 1.0f - t, FadeBounds.Left, FadeBounds.Top, FadeBounds);
			AnimationDrawHelper.DrawImageWithTransparency(gr, CurrentAnimationInfo.Bitmap, t, FadeBounds.Left, FadeBounds.Top, FadeBounds);
		}
	}
	#endregion
}
