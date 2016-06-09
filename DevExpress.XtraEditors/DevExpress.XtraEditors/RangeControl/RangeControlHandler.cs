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
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DevExpress.Utils.Gesture;
using System.Drawing;
namespace DevExpress.XtraEditors {
	public class RangeControlHandler : IGestureClient {
		static Point InvalidPoint = new Point(-10000, -10000);
		public RangeControlHandler(RangeControl rangeControl) {
			RangeControl = rangeControl;
		}
		protected int Delta { get; set; }
		public RangeControl RangeControl { get; private set; }
		RangeControlHitInfo DownInfo { get; set; }
		bool FirstMouseMove { get; set; }
		public virtual void OnMouseDown(MouseEventArgs e) {
			if(e.Button == MouseButtons.Left) {
				RangeBoxMoved = false;
				RangeControl.Focus();
				ViewInfo.UpdatePressedInfo(e.Location);
				DownInfo = ViewInfo.PressedInfo;
				Delta = CalcPointDelta(e.Location);
				FirstMouseMove = true;
				if(ViewInfo.PressedInfo.HitTest == RangeControlHitTest.ViewPort || ViewInfo.PressedInfo.HitTest == RangeControlHitTest.RangeBox && !RangeControl.AllowPanMode) {
					ViewInfo.UpdateSelection(RangeControl.RangeViewInfo.Point2Value(DownPoint), RangeControl.RangeViewInfo.Point2Value(DownPoint));
				} else if(ViewInfo.PressedInfo.HitTest == RangeControlHitTest.ScrollBarArea) {
					RangeControl.UpdateViewPortPosition(e.Location, true);
				}
			}
			if(e.Button == MouseButtons.Right) {
				ViewInfo.UpdatePressedInfo(e.Location);
				if(ViewInfo.PressedInfo.HitTest != RangeControlHitTest.Client)
					ViewInfo.PressedInfo.HitTest = RangeControlHitTest.None;
				DownInfo = ViewInfo.PressedInfo;
			}
		}
		int CalcPointDelta(Rectangle rect, Point point) {
			return CalcPointDelta(rect, point, false);
		}
		int CalcPointDelta(Rectangle rect, Point point, bool useRightSide) {
			if(!useRightSide) {
				if(RangeControl.Orientation == Orientation.Horizontal) {
					if(RangeControl.IsRightToLeft) return  point.X - rect.Right;
					return point.X - rect.X;
				}
				return point.Y - rect.Y;
			} else {
				if(RangeControl.Orientation == Orientation.Horizontal) {
					if(RangeControl.IsRightToLeft) return point.X - rect.X;
					return point.X - rect.Right;
				}
				return point.Y - rect.Bottom;
			}
		}
		protected virtual int CalcPointDelta(Point point) {
			if(ViewInfo.PressedInfo.HitTest == RangeControlHitTest.LeftScaleThumb)
				return CalcPointDelta(ViewInfo.ScrollBarThumbBounds, point);
			else if(ViewInfo.PressedInfo.HitTest == RangeControlHitTest.RightScaleThumb)
				return CalcPointDelta(ViewInfo.ScrollBarThumbBounds, point, true);
			else if(ViewInfo.PressedInfo.HitTest == RangeControlHitTest.RangeIndicator)
				return CalcPointDelta(ViewInfo.RangeIndicatorBounds, point);
			else if(ViewInfo.PressedInfo.HitTest == RangeControlHitTest.ScrollBarThumb)
				return CalcPointDelta(ViewInfo.ScrollBarThumbBounds, point);
			else if(ViewInfo.PressedInfo.HitTest == RangeControlHitTest.MaxRangeThumb) {
				return CalcPointDelta(ViewInfo.RangeBounds, point, true);
			} else if(ViewInfo.PressedInfo.HitTest == RangeControlHitTest.MinRangeThumb)
				return CalcPointDelta(ViewInfo.RangeBounds, point);
			return CalcPointDelta(ViewInfo.RangeBounds, point);
		}
		protected Point DownPoint { get { return DownInfo.HitPoint; } }
		protected RangeControlViewInfo ViewInfo { get { return RangeControl.RangeViewInfo; } }
		protected virtual Point CorrectPoint(Point loc) {
			if(RangeControl.Orientation == Orientation.Horizontal)
				return new Point(loc.X - Delta, loc.Y);
			return new Point(loc.X, loc.Y - Delta);
		}
		bool RangeBoxMoved { get; set; }
		bool NeedResolveScaleCollision {
			get {
				return ViewInfo.GetRectWidth(ViewInfo.ScrollBarThumbBounds) < ViewInfo.GetRectWidth(ViewInfo.RightScaleThumbBounds) &&
					ViewInfo.PressedInfo.HitTest == RangeControlHitTest.RightScaleThumb;
			}
		}
		bool NeedResolveRangeThumbCollision {
			get {
				return ViewInfo.GetRectWidth(ViewInfo.RangeBounds) == 0 && ViewInfo.PressedInfo.HitTest == RangeControlHitTest.MaxRangeThumb;
			}
		}
		void MakeFirstMouseMoveWhenCollision(MouseEventArgs e) {
			if(!FirstMouseMove)
				return;
			if(!NeedResolveScaleCollision && !NeedResolveRangeThumbCollision) {
				FirstMouseMove = false;
				return;
			}
			Point delta = new Point(e.Location.X - DownPoint.X, e.Location.Y - DownPoint.Y);
			if((RangeControl.Orientation == Orientation.Horizontal && delta.X < 0) ||
				(RangeControl.Orientation == Orientation.Vertical && delta.Y < 0)) {
				if(delta.X < 0) {
					if(NeedResolveScaleCollision) ViewInfo.PressedInfo.HitTest = RangeControlHitTest.LeftScaleThumb;
					if(NeedResolveRangeThumbCollision) ViewInfo.PressedInfo.HitTest = RangeControlHitTest.MinRangeThumb;
				}
				else {
					if(NeedResolveScaleCollision) ViewInfo.PressedInfo.HitTest = RangeControlHitTest.RightScaleThumb;
					if(NeedResolveRangeThumbCollision) ViewInfo.PressedInfo.HitTest = RangeControlHitTest.MaxRangeThumb;
				}
			}
			else if(delta.X == 0 && delta.Y == 0) {
				return;
			}
			FirstMouseMove = false;
			DownInfo = ViewInfo.PressedInfo;
			Delta = CalcPointDelta(e.Location);
		}
		public virtual void OnMouseMove(MouseEventArgs e) {
			if(e.Button == MouseButtons.Left) {
				if(FirstMouseMove) {
					MakeFirstMouseMoveWhenCollision(e);
				}
				if(ViewInfo.PressedInfo.HitTest == RangeControlHitTest.LeftScaleThumb)
					RangeControl.UpdateViewPortStart(CorrectPoint(e.Location));
				else if(ViewInfo.PressedInfo.HitTest == RangeControlHitTest.RightScaleThumb)
					RangeControl.UpdateViewPortEnd(CorrectPoint(e.Location));
				else if(ViewInfo.PressedInfo.HitTest == RangeControlHitTest.MinRangeThumb)
					RangeControl.UpdateRangeMinimum(CorrectPoint(e.Location), true);
				else if(ViewInfo.PressedInfo.HitTest == RangeControlHitTest.MaxRangeThumb)
					RangeControl.UpdateRangeMaximum(CorrectPoint(e.Location), true);
				else if(ViewInfo.PressedInfo.HitTest == RangeControlHitTest.RangeBox) {
					if(!RangeControl.AllowPanMode)
						ViewInfo.UpdateSelection(ViewInfo.SelectionStart, ViewInfo.Point2Value(e.Location));
					else if(e.Location != DownPoint) {
						RangeControl.UpdateRangePositon(CorrectPoint(e.Location), false, true);
						RangeBoxMoved = true;
					}
				} else if(ViewInfo.PressedInfo.HitTest == RangeControlHitTest.RangeIndicator)
					RangeControl.UpdateRangePositon(CorrectPoint(e.Location), true, true);
				else if(ViewInfo.PressedInfo.HitTest == RangeControlHitTest.ScrollBarThumb)
					RangeControl.UpdateViewPortPosition(CorrectPoint(e.Location));
				else if(ViewInfo.PressedInfo.HitTest == RangeControlHitTest.ViewPort)
					ViewInfo.UpdateSelection(ViewInfo.SelectionStart, ViewInfo.Point2Value(e.Location));
			}
			ViewInfo.UpdateHotInfo(e.Location);
			UpdateCursors();
		}
		protected Cursor PrevCursor { get; set; }
		protected virtual void UpdateCursors() {
			if(ViewInfo.HotInfo.HitTest == RangeControlHitTest.LeftScaleThumb ||
				ViewInfo.HotInfo.HitTest == RangeControlHitTest.RightScaleThumb ||
				ViewInfo.HotInfo.HitTest == RangeControlHitTest.MinRangeThumb ||
				ViewInfo.HotInfo.HitTest == RangeControlHitTest.MaxRangeThumb) {
				if(PrevCursor == null)
					PrevCursor = RangeControl.Cursor;
				RangeControl.Cursor = ViewInfo.IsHorizontal ? Cursors.SizeWE : Cursors.SizeNS;
			} else {
				RangeControl.Cursor = PrevCursor;
				PrevCursor = null;
			}
		}
		bool IsClientClick {
			get {
				if(RangeControl.Client == null)
					return false;
				return ViewInfo.PressedInfo.HitTest == DownInfo.HitTest && 
					ViewInfo.PressedInfo.ClientHitTest == DownInfo.ClientHitTest && 
					ViewInfo.PressedInfo.HitObject == DownInfo.HitObject;
			}
		}
		public virtual void OnMouseUp(MouseEventArgs e) {
			bool prev = RangeControl.AnimateOnDataChange;
			RangeControl.AnimateOnDataChange = false;
			try{
				if(ViewInfo.PressedInfo.HitTest == RangeControlHitTest.Client) {
					if(IsClientClick) {
						RangeControl.Client.OnClick(ViewInfo.PressedInfo);
					}
				} 
				else if(ViewInfo.PressedInfo.HitTest == RangeControlHitTest.ViewPort && RangeControl.AllowSelection) {
					ViewInfo.SelectionEnd = ViewInfo.Point2Value(e.Location);
					RangeControl.SelectRange(ViewInfo.SelectionStart, ViewInfo.SelectionEnd);
				} else if(ViewInfo.PressedInfo.HitTest == RangeControlHitTest.MinRangeThumb)
					RangeControl.UpdateRangeMinimum(CorrectPoint(e.Location), false);
				else if(ViewInfo.PressedInfo.HitTest == RangeControlHitTest.MaxRangeThumb)
					RangeControl.UpdateRangeMaximum(CorrectPoint(e.Location), false);
				else if(ViewInfo.PressedInfo.HitTest == RangeControlHitTest.RangeBox) {
					if(RangeBoxMoved)
						RangeControl.UpdateRangePositon(CorrectPoint(e.Location), false, false);
					else if(e.Location != DownPoint)
						RangeControl.SelectRange(ViewInfo.SelectionStart, ViewInfo.Point2Value(e.Location));
					else RangeControl.SelectRange(ViewInfo.Point2Value(e.Location), ViewInfo.Point2Value(e.Location));
				}
				else if(ViewInfo.PressedInfo.HitTest == RangeControlHitTest.RangeIndicator)
					RangeControl.UpdateRangePositon(CorrectPoint(e.Location), true, false);
				ViewInfo.ResetSelection();
				ViewInfo.PressedInfo = RangeControlHitInfo.Empty;
				ViewInfo.UpdateHotInfo(e.Location);
			}
			finally { 
				RangeControl.AnimateOnDataChange = prev;
			}
		}
		public virtual void OnMouseWheel(MouseEventArgs e) {
			int count = -e.Delta / 120;
			double delta = count * RangeControl.VisibleRangeWidth / 10.0;
			RangeControl.VisibleRangeStartPosition += delta;
		}
		public virtual void OnMouseEnter(EventArgs e) { }
		public virtual void OnMouseLeave(EventArgs e) {
			ViewInfo.UpdateHotInfo(InvalidPoint);
		}
		public virtual void OnKeyDown(KeyEventArgs e) { 
		}
		public virtual void OnKeyUp(KeyEventArgs e) { }
		public virtual bool ProcessCmdKey(ref Message msg, Keys keyData) {
			switch(keyData) { 
				case Keys.Left:
				case Keys.Up:
					RangeControl.UpdateViewPortPosition(RangeControl.VisibleRangeStartPosition - RangeControl.VisibleRangeWidth / 10, true);
					return true;
				case Keys.Right:
				case Keys.Down:
					RangeControl.UpdateViewPortPosition(RangeControl.VisibleRangeStartPosition + RangeControl.VisibleRangeWidth / 10, true);
					return true;
				case Keys.PageUp:
					RangeControl.UpdateViewPortPosition(RangeControl.VisibleRangeStartPosition - RangeControl.VisibleRangeWidth, true);
					return true;
				case Keys.PageDown:
					RangeControl.UpdateViewPortPosition(RangeControl.VisibleRangeStartPosition + RangeControl.VisibleRangeWidth, true);
					return true;
				case Keys.Home:
					RangeControl.UpdateViewPortPosition(0.0, true);
					return true;
				case Keys.End:
					RangeControl.UpdateViewPortPosition(1.0, true);
					return true;
			}
			return false;
		}
		#region touch gestures
		GestureHelper gestureHelper;
		public GestureHelper GestureHelper {
			get { 
				if(gestureHelper == null)
					gestureHelper = new GestureHelper(this);
				return gestureHelper;
			}
		}
		#endregion
		#region IGestureClient Members
		protected Point TouchPoint { get; set; }
		GestureAllowArgs[] IGestureClient.CheckAllowGestures(Point point) {
			if(!RangeControl.IsClientValid)
				return new GestureAllowArgs[] { };
			TouchZoom = false;		   
			if(CalcTouchHitTest(point) == TouchHitTest.Content) {
				TouchPoint = point;
				StartTouchSelectionTimer();
			}
			int blockId = ViewInfo.RangeClientBounds.Contains(point) ? 0 : GestureHelper.GC_PAN_WITH_INERTIA;
			GestureAllowArgs p = new GestureAllowArgs() { GID = GID.PAN, AllowID = GestureHelper.GC_PAN | GestureHelper.GC_PAN_WITH_SINGLE_FINGER_VERTICALLY | GestureHelper.GC_PAN_WITH_SINGLE_FINGER_HORIZONTALLY, BlockID = blockId };
			return new GestureAllowArgs[] { p, GestureAllowArgs.Zoom };
		}
		IntPtr IGestureClient.Handle {
			get {
				if(RangeControl.IsHandleCreated)
					return RangeControl.Handle;
				return IntPtr.Zero;
			}
		}
		Timer TouchSelectionTimer { get; set; }
		protected virtual void StartTouchSelectionTimer() {
			MakeTouchSelection = false;
			if(TouchSelectionTimer == null) {
				TouchSelectionTimer = new Timer();
				TouchSelectionTimer.Interval = 500;
				TouchSelectionTimer.Tick += OnTouchSelectionTimerTick;
			}
			TouchSelectionTimer.Start();
		}
		protected virtual void StopTouchSelectionTimer() {
			if(TouchSelectionTimer != null)
				TouchSelectionTimer.Stop();
		}
		protected bool MakeTouchSelection { get; set; }
		void OnTouchSelectionTimerTick(object sender, EventArgs e) {
			TouchSelectionTimer.Stop();
			if(TouchZoom)
				return;
			MakeTouchSelection = true;
			ViewInfo.UpdateSelection(RangeControl.RangeViewInfo.Point2Value(TouchPoint), RangeControl.RangeViewInfo.Point2Value(TouchPoint));
		}
		TouchHitTest TouchHitTest { get; set; }
		protected virtual TouchHitTest CalcTouchHitTest(Point pt) {
			if(ViewInfo.MinThumbTouchBounds.Contains(pt))
				return TouchHitTest.MinThumb;
			else if(ViewInfo.MaxThumbTouchBounds.Contains(pt))
				return TouchHitTest.MaxThumb;
			else if(ViewInfo.RangeBounds.Contains(pt))
				return TouchHitTest.RangeBox;
			else if(ViewInfo.RangeClientBounds.Contains(pt))
				return TouchHitTest.Content;
			return TouchHitTest.None;
		}
		void IGestureClient.OnEnd(GestureArgs info) { }
		void IGestureClient.OnBegin(GestureArgs info) {
			StopTouchSelectionTimer();
			TouchHitTest = CalcTouchHitTest(info.Start.Point);
			if(TouchHitTest == TouchHitTest.RangeBox) {
				Delta = CalcPointDelta(info.Start.Point);
			}
			else if(TouchHitTest == TouchHitTest.Content) {
				if(MakeTouchSelection)
					ViewInfo.UpdateSelection(RangeControl.RangeViewInfo.Point2Value(info.Start.Point), RangeControl.RangeViewInfo.Point2Value(info.Start.Point));
			}
		}
		void IGestureClient.OnPan(GestureArgs info, System.Drawing.Point delta, ref System.Drawing.Point overPan) {
			if(TouchZoom)
				return;
			bool correctValue = info.IsEnd;
			if((TouchHitTest == TouchHitTest.MinThumb || TouchHitTest == TouchHitTest.MaxThumb) && (ViewInfo.MinThumbTouchBounds.Contains(info.Current.Point) && ViewInfo.MaxThumbTouchBounds.Contains(info.Current.Point))) {
				correctValue = true;
				if(ViewInfo.IsHorizontal) {
					if(delta.X < 0)
						TouchHitTest = TouchHitTest.MinThumb;
					else
						TouchHitTest = TouchHitTest.MaxThumb;
				}
				else {
					if(delta.Y < 0)
						TouchHitTest = TouchHitTest.MinThumb;
					else
						TouchHitTest = TouchHitTest.MaxThumb;
				}
			} 
			if(TouchHitTest == TouchHitTest.MinThumb) {
				RangeControl.UpdateRangeMinimum(info.Current.Point, !correctValue);
			}
			else if(TouchHitTest == TouchHitTest.MaxThumb) {
				RangeControl.UpdateRangeMaximum(info.Current.Point, !correctValue);
			}
			else if(TouchHitTest == TouchHitTest.RangeBox) {
			   RangeControl.UpdateRangePositon(CorrectPoint(info.Current.Point), false, !info.IsEnd);
			}
			else if(TouchHitTest == TouchHitTest.Content) {
				if(MakeTouchSelection) {
					if(info.IsEnd) {
						RangeControl.SelectRange(ViewInfo.SelectionStart, ViewInfo.Point2Value(info.Current.Point));
						ViewInfo.ResetSelection();
					}
					else
						ViewInfo.UpdateSelection(ViewInfo.SelectionStart, ViewInfo.Point2Value(info.Current.Point));
				}
				else {
					RangeControl.VisibleRangeStartPosition -= ViewInfo.Pixel2Delta(delta.X);
				}
			}
		}
		void IGestureClient.OnPressAndTap(GestureArgs info) {
		}
		void IGestureClient.OnRotate(GestureArgs info, System.Drawing.Point center, double degreeDelta) {
		}
		void IGestureClient.OnTwoFingerTap(GestureArgs info) {
		}
		protected bool TouchZoom { get; set; }
		void IGestureClient.OnZoom(GestureArgs info, System.Drawing.Point center, double zoomDelta) {
			TouchZoom = true;
			double centerValue = ViewInfo.Point2Value(center);
			double relation = (centerValue - RangeControl.VisibleRangeStartPosition) / RangeControl.VisibleRangeWidth;
			RangeControl.VisibleRangeScaleFactor *= zoomDelta;
			double startPosition = centerValue - relation * RangeControl.VisibleRangeWidth;
			RangeControl.VisibleRangeStartPosition = Math.Min(startPosition, 1.0 - RangeControl.VisibleRangeWidth);
		}
		IntPtr IGestureClient.OverPanWindowHandle {
			get {
				Form form = RangeControl.FindForm();
				return form != null && form.IsHandleCreated? form.Handle: IntPtr.Zero; 
			}
		}
		System.Drawing.Point IGestureClient.PointToClient(System.Drawing.Point p) {
			return RangeControl.IsHandleCreated ? RangeControl.PointToClient(p) : p;
		}
		#endregion
	}
	public enum TouchHitTest { None, MinThumb, MaxThumb, Content, RangeBox }
}
