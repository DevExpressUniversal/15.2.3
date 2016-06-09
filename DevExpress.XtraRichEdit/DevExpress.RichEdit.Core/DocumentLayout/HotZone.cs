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
using DevExpress.Utils;
using DevExpress.Utils.KeyboardHandler;
using DevExpress.Office.Utils;
using DevExpress.Office.Internal;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.XtraRichEdit.Model;
using DevExpress.Compatibility.System.Drawing;
using System.Drawing.Drawing2D;
using DevExpress.Compatibility.System.Drawing.Drawing2D;
using DevExpress.XtraRichEdit.Mouse;
using System.Diagnostics;
using Debug = System.Diagnostics.Debug;
namespace DevExpress.XtraRichEdit.Layout {
#region IHotZoneVisitor
	public interface IHotZoneVisitor { }
#endregion
#region IRectangularObjectHotZoneVisitor
	public interface IRectangularObjectHotZoneVisitor : IHotZoneVisitor {
		void Visit(RectangularObjectTopLeftHotZone hotZone);
		void Visit(RectangularObjectTopMiddleHotZone hotZone);
		void Visit(RectangularObjectTopRightHotZone hotZone);
		void Visit(RectangularObjectMiddleLeftHotZone hotZone);
		void Visit(RectangularObjectMiddleRightHotZone hotZone);
		void Visit(RectangularObjectBottomLeftHotZone hotZone);
		void Visit(RectangularObjectBottomMiddleHotZone hotZone);
		void Visit(RectangularObjectBottomRightHotZone hotZone);
		void Visit(RectangularObjectRotationHotZone hotZone);
	}
#endregion
#region IHotZone
	public interface IHotZone {
		bool HitTest(Point point, float dpi, float zoomFactor);
		void Accept(IHotZoneVisitor visitor);
	}
#endregion
#region HotZone (abstract class)
	public abstract class HotZone : IHotZone {
		const int hotZoneSize = 10;
		Rectangle bounds;
		Rectangle extendedBounds;
		Matrix hitTestTransform;
		IGestureStateIndicator gestureStateIndicator;
		public Rectangle Bounds { get { return bounds; } set { bounds = value; } }
		public Rectangle ExtendedBounds { get { return extendedBounds; } set { extendedBounds = value; } }
		public IGestureStateIndicator GestureStateIndicator { get { return gestureStateIndicator; } set { gestureStateIndicator = value; } }
		bool UseExtendedBounds { get { return GestureStateIndicator.GestureActivated; } }
		public abstract RichEditCursor Cursor { get; }
		public Matrix HitTestTransform { get { return hitTestTransform; } set { hitTestTransform = value; } }
		public Rectangle CalculateActualBounds(float dpiX, float zoomFactor) {
			int minHotZoneSize = Units.PixelsToDocuments(hotZoneSize, dpiX * zoomFactor);
			Rectangle result = (UseExtendedBounds)? ExtendedBounds : Bounds;
			if (result.Width < minHotZoneSize) {
				result.X += (result.Width - minHotZoneSize) / 2;
				result.Width = minHotZoneSize;
			}
			if (result.Height < minHotZoneSize) {
				result.Y += (result.Height - minHotZoneSize) / 2;
				result.Height = minHotZoneSize;
			}
			return result;
		}
		public virtual bool HitTest(Point point, float dpi, float zoomFactor) {
			Rectangle hotZoneBounds = CalculateActualBounds(dpi, zoomFactor);
			if (hitTestTransform != null)
				point = hitTestTransform.TransformPoint(point);
			return hotZoneBounds.Contains(point);
		}
		public abstract bool BeforeActivate(RichEditMouseHandler handler, RichEditHitTestResult result);
		public abstract void Activate(RichEditMouseHandler handler, RichEditHitTestResult result);
		void IHotZone.Accept(IHotZoneVisitor visitor) {
			AcceptCore(visitor);
		}
		protected abstract void AcceptCore(IHotZoneVisitor visitor);
	}
#endregion
#region RoundHotZone
	public interface IRoundHotZone : IHotZone {
		PointF Center { get; }
		float Radius { get; }
	}
	public abstract class RoundHotZone : HotZone, IRoundHotZone {
		PointF center;
		float radius;
		protected RoundHotZone(PointF center, float radius) {
			this.center = center;
			this.radius = radius;
			Bounds = new Rectangle((int)(center.X - radius), (int)(center.Y - radius), 2 * (int)Radius, 2 * (int)Radius);
		}
		public PointF Center { get { return center; } }
		public float Radius { get { return radius; } }
		public override RichEditCursor Cursor { get { return RichEditCursors.Cross; } } 
		public override void Activate(RichEditMouseHandler handler, RichEditHitTestResult result) {
		}
		public override bool BeforeActivate(RichEditMouseHandler handler, RichEditHitTestResult result) {
			return false;
		}
		public override bool HitTest(Point point, float dpi, float zoomFactor) {
			return false;
		}
	}
#endregion
#region HotZoneCollection
	public class HotZoneCollection : List<HotZone> {
	}
#endregion
#region RectangularObjectHotZone (abstract class)
	public abstract class RectangularObjectHotZone : HotZone {
#region Fields
		readonly Box box;
		readonly PieceTable pieceTable;
#endregion
		protected RectangularObjectHotZone(Box box, PieceTable pieceTable, IGestureStateIndicator gestureStateIndicator) {
			Guard.ArgumentNotNull(box, "box");
			this.box = box;
			this.pieceTable = pieceTable;
		}
#region Properties
		public Box Box { get { return box; } }
		public virtual bool CanKeepAspectRatio {
			get {
				bool lockAspectRatio = true;
				FloatingObjectBox floatingObjectBox = box as FloatingObjectBox;
				if(floatingObjectBox != null)
					lockAspectRatio = floatingObjectBox.GetFloatingObjectRun().FloatingObjectProperties.LockAspectRatio;
				else {
					InlinePictureBox inlinePictureBox = box as InlinePictureBox;
					if(inlinePictureBox != null)
						lockAspectRatio = ((InlinePictureRun)inlinePictureBox.GetRun(PieceTable)).PictureProperties.LockAspectRatio;
				}
				return lockAspectRatio ^ KeyboardHandler.IsShiftPressed;
			}
		}
		public PieceTable PieceTable { get { return pieceTable; } }
		public bool EnlargedHotZones { get { return GestureStateIndicator.GestureActivated; } }
		#endregion
		public override bool BeforeActivate(RichEditMouseHandler handler, RichEditHitTestResult result) {
			return !handler.DeactivateTextBoxPieceTableIfNeed(PieceTable, result);
		}
		public override void Activate(RichEditMouseHandler handler, RichEditHitTestResult result) {
			RichEditRectangularObjectResizeMouseHandlerState state = handler.CreateRectangularObjectResizeState(this, result);
			handler.SwitchStateCore(state, Point.Empty);
		}
		protected internal virtual Rectangle ForceKeepOriginalAspectRatio(Rectangle bounds, Size originalSize) {
			if (!CanKeepAspectRatio)
				return bounds;
			float aspectX = Math.Max(1, 100 * bounds.Width / (float)originalSize.Width);
			float aspectY = Math.Max(1, 100 * bounds.Height / (float)originalSize.Height);
			if (aspectX > aspectY)
				return ForceKeepOriginalAspectRatio(bounds, bounds.Width, (int)Math.Round(originalSize.Height * aspectX / 100.0f));
			else
				return ForceKeepOriginalAspectRatio(bounds, (int)Math.Round(originalSize.Width * aspectY / 100.0f), bounds.Height);
		}
		protected internal virtual Rectangle ForceKeepOriginalAspectRatio(Rectangle bounds, int desiredWidth, int desiredHeight) {
			return bounds;
		}
		public Rectangle CreateValidBoxBounds(Point point) {
			if (HitTestTransform != null)
				point = HitTestTransform.TransformPoint(point);
			return CreateValidBoxBoundsCore(point);
		}
		protected internal abstract Rectangle CreateValidBoxBoundsCore(Point point);
		public Point CalculateOffset(Point point) {
			if (HitTestTransform != null)
				point = HitTestTransform.TransformPoint(point);
			return CalculateOffsetCore(point);
		}
		protected internal abstract Point CalculateOffsetCore(Point point);
		protected RichEditCursor CalculateCursor(RichEditCursor defaultCursor) {
			if (HitTestTransform == null)
				return defaultCursor;
			float cursorAngle = CalculateCursorAngle(defaultCursor);
			if (cursorAngle < 0)
				return defaultCursor;
			float rotationAngle = -PieceTable.DocumentModel.GetBoxEffectiveRotationAngleInDegrees(Box);
			return CalculateCursorByAngle(rotationAngle + cursorAngle);
		}
		float CalculateCursorAngle(RichEditCursor defaultCursor) {
			if (defaultCursor == RichEditCursors.SizeWE)
				return 0;
			if (defaultCursor == RichEditCursors.SizeNS)
				return 90;
			if (defaultCursor == RichEditCursors.SizeNESW)
				return 45;
			if (defaultCursor == RichEditCursors.SizeNWSE)
				return 135;
			return -1;
		}
		RichEditCursor CalculateCursorByAngle(float angle) {
			angle %= 360f;
			if (angle >= 180)
				angle -= 180;
			if (angle <= -180)
				angle += 180;
			if (angle < 0)
				angle += 180;
			Debug.Assert(angle >= 0 && angle <= 180);
			if (angle < 22.5f || angle >= 157.5f) 
				return RichEditCursors.SizeWE;
			if (angle >= 22.5f && angle <= 67.5f) 
				return RichEditCursors.SizeNESW;
			if (angle > 67.5f && angle < 112.5f) 
				return RichEditCursors.SizeNS;
			return RichEditCursors.SizeNWSE;
		}
		protected override void AcceptCore(IHotZoneVisitor visitor) {
			Accept((IRectangularObjectHotZoneVisitor)visitor);
		}
		public abstract void Accept(IRectangularObjectHotZoneVisitor visitor);
	}
#endregion
#region RectangularObjectTopLeftHotZone
	public class RectangularObjectTopLeftHotZone : RectangularObjectHotZone {
		public RectangularObjectTopLeftHotZone(Box box, PieceTable pieceTable, IGestureStateIndicator gestureIndicator)
			: base(box, pieceTable, gestureIndicator) {
		}
		public override RichEditCursor Cursor { get { return CalculateCursor(RichEditCursors.SizeNWSE); } }
		protected internal override Rectangle CreateValidBoxBoundsCore(Point point) {
			Rectangle boxBounds = Box.ActualSizeBounds;
			return Rectangle.FromLTRB(
				Math.Min(point.X, boxBounds.Right - 1),
				Math.Min(point.Y, boxBounds.Bottom - 1),
				boxBounds.Right,
				boxBounds.Bottom);
		}
		protected internal override Point CalculateOffsetCore(Point point) {
			Rectangle boxBounds = Box.ActualSizeBounds;
			return new Point(boxBounds.Left - point.X, boxBounds.Top - point.Y);
		}
		protected internal override Rectangle ForceKeepOriginalAspectRatio(Rectangle bounds, int desiredWidth, int desiredHeight) {
			bounds.X = bounds.Right - desiredWidth;
			bounds.Y = bounds.Bottom - desiredHeight;
			bounds.Width = desiredWidth;
			bounds.Height = desiredHeight;
			return bounds;
		}
		public override void Accept(IRectangularObjectHotZoneVisitor visitor) {
			visitor.Visit(this);
		}
	}
#endregion
#region RectangularObjectBottomRightHotZone
	public class RectangularObjectBottomRightHotZone : RectangularObjectHotZone {
		public RectangularObjectBottomRightHotZone(Box box, PieceTable pieceTable, IGestureStateIndicator gestureIndicator)
			: base(box, pieceTable, gestureIndicator) {
		}
		public override RichEditCursor Cursor { get { return CalculateCursor(RichEditCursors.SizeNWSE); } }
		protected internal override Rectangle CreateValidBoxBoundsCore(Point point) {
			Rectangle boxBounds = Box.ActualSizeBounds;
			return Rectangle.FromLTRB(
				boxBounds.Left,
				boxBounds.Top,
				Math.Max(point.X, boxBounds.Left + 1),
				Math.Max(point.Y, boxBounds.Top + 1));
		}
		protected internal override Point CalculateOffsetCore(Point point) {
			Rectangle boxBounds = Box.ActualSizeBounds;
			return new Point(boxBounds.Right - point.X, boxBounds.Bottom - point.Y);
		}
		protected internal override Rectangle ForceKeepOriginalAspectRatio(Rectangle bounds, int desiredWidth, int desiredHeight) {
			bounds.Width = desiredWidth;
			bounds.Height = desiredHeight;
			return bounds;
		}
		public override void Accept(IRectangularObjectHotZoneVisitor visitor) {
			visitor.Visit(this);
		}
	}
#endregion
#region RectangularObjectTopRightHotZone
	public class RectangularObjectTopRightHotZone : RectangularObjectHotZone {
		public RectangularObjectTopRightHotZone(Box box, PieceTable pieceTable, IGestureStateIndicator gestureIndicator)
			: base(box, pieceTable, gestureIndicator) {
		}
		public override RichEditCursor Cursor { get { return CalculateCursor(RichEditCursors.SizeNESW); } }
		protected internal override Rectangle CreateValidBoxBoundsCore(Point point) {
			Rectangle boxBounds = Box.ActualSizeBounds;
			return Rectangle.FromLTRB(
				boxBounds.Left,
				Math.Min(point.Y, boxBounds.Bottom - 1),
				Math.Max(point.X, boxBounds.Left + 1),
				boxBounds.Bottom);
		}
		protected internal override Point CalculateOffsetCore(Point point) {
			Rectangle boxBounds = Box.ActualSizeBounds;
			return new Point(boxBounds.Right - point.X, boxBounds.Top - point.Y);
		}
		protected internal override Rectangle ForceKeepOriginalAspectRatio(Rectangle bounds, int desiredWidth, int desiredHeight) {
			bounds.Width = desiredWidth;
			bounds.Y = bounds.Bottom - desiredHeight;
			bounds.Height = desiredHeight;
			return bounds;
		}
		public override void Accept(IRectangularObjectHotZoneVisitor visitor) {
			visitor.Visit(this);
		}
	}
#endregion
#region RectangularObjectBottomLeftHotZone
	public class RectangularObjectBottomLeftHotZone : RectangularObjectHotZone {
		public RectangularObjectBottomLeftHotZone(Box box, PieceTable pieceTable, IGestureStateIndicator gestureIndicator)
			: base(box, pieceTable, gestureIndicator) {
		}
		public override RichEditCursor Cursor { get { return CalculateCursor(RichEditCursors.SizeNESW); } }
		protected internal override Rectangle CreateValidBoxBoundsCore(Point point) {
			Rectangle boxBounds = Box.ActualSizeBounds;
			return Rectangle.FromLTRB(
				Math.Min(point.X, boxBounds.Right - 1),
				boxBounds.Top,
				boxBounds.Right,
				Math.Max(point.Y, boxBounds.Top + 1));
		}
		protected internal override Point CalculateOffsetCore(Point point) {
			Rectangle boxBounds = Box.ActualSizeBounds;
			return new Point(boxBounds.Left - point.X, boxBounds.Bottom - point.Y);
		}
		protected internal override Rectangle ForceKeepOriginalAspectRatio(Rectangle bounds, int desiredWidth, int desiredHeight) {
			bounds.X = bounds.Right - desiredWidth;
			bounds.Width = desiredWidth;
			bounds.Height = desiredHeight;
			return bounds;
		}
		public override void Accept(IRectangularObjectHotZoneVisitor visitor) {
			visitor.Visit(this);
		}
	}
#endregion
#region RectangularObjectTopMiddleHotZone
	public class RectangularObjectTopMiddleHotZone : RectangularObjectHotZone {
		public RectangularObjectTopMiddleHotZone(Box box, PieceTable pieceTable, IGestureStateIndicator gestureIndicator)
			: base(box, pieceTable, gestureIndicator) {
		}
		public override RichEditCursor Cursor { get { return CalculateCursor(RichEditCursors.SizeNS); } }
		public override bool CanKeepAspectRatio { get { return false; } }
		protected internal override Rectangle CreateValidBoxBoundsCore(Point point) {
			Rectangle boxBounds = Box.ActualSizeBounds;
			return Rectangle.FromLTRB(
				boxBounds.Left,
				Math.Min(point.Y, boxBounds.Bottom - 1),
				boxBounds.Right,
				boxBounds.Bottom);
		}
		protected internal override Point CalculateOffsetCore(Point point) {
			Rectangle boxBounds = Box.ActualSizeBounds;
			return new Point(0, boxBounds.Top - point.Y);
		}
		public override void Accept(IRectangularObjectHotZoneVisitor visitor) {
			visitor.Visit(this);
		}
	}
#endregion
#region RectangularObjectBottomMiddleHotZone
	public class RectangularObjectBottomMiddleHotZone : RectangularObjectHotZone {
		public RectangularObjectBottomMiddleHotZone(Box box, PieceTable pieceTable, IGestureStateIndicator gestureIndicator)
			: base(box, pieceTable, gestureIndicator) {
		}
		public override RichEditCursor Cursor { get { return CalculateCursor(RichEditCursors.SizeNS); } }
		public override bool CanKeepAspectRatio { get { return false; } }
		protected internal override Rectangle CreateValidBoxBoundsCore(Point point) {
			Rectangle boxBounds = Box.ActualSizeBounds;
			return Rectangle.FromLTRB(
				boxBounds.Left,
				boxBounds.Top,
				boxBounds.Right,
				Math.Max(point.Y, boxBounds.Top + 1));
		}
		protected internal override Point CalculateOffsetCore(Point point) {
			Rectangle boxBounds = Box.ActualSizeBounds;
			return new Point(0, boxBounds.Bottom - point.Y);
		}
		public override void Accept(IRectangularObjectHotZoneVisitor visitor) {
			visitor.Visit(this);
		}
	}
#endregion
#region RectangularObjectMiddleLeftHotZone
	public class RectangularObjectMiddleLeftHotZone : RectangularObjectHotZone {
		public RectangularObjectMiddleLeftHotZone(Box box, PieceTable pieceTable, IGestureStateIndicator gestureIndicator)
			: base(box, pieceTable, gestureIndicator) {
		}
		public override RichEditCursor Cursor { get { return CalculateCursor(RichEditCursors.SizeWE); } }
		public override bool CanKeepAspectRatio { get { return false; } }
		protected internal override Rectangle CreateValidBoxBoundsCore(Point point) {
			Rectangle boxBounds = Box.ActualSizeBounds;
			return Rectangle.FromLTRB(
				Math.Min(point.X, boxBounds.Right - 1),
				boxBounds.Top,
				boxBounds.Right,
				boxBounds.Bottom);
		}
		protected internal override Point CalculateOffsetCore(Point point) {
			Rectangle boxBounds = Box.ActualSizeBounds;
			return new Point(boxBounds.Left - point.X, 0);
		}
		public override void Accept(IRectangularObjectHotZoneVisitor visitor) {
			visitor.Visit(this);
		}
	}
#endregion
#region RectangularObjectMiddleRightHotZone
	public class RectangularObjectMiddleRightHotZone : RectangularObjectHotZone {
		public RectangularObjectMiddleRightHotZone(Box box, PieceTable pieceTable, IGestureStateIndicator gestureIndicator)
			: base(box, pieceTable, gestureIndicator) {
		}
		public override RichEditCursor Cursor { get { return CalculateCursor(RichEditCursors.SizeWE); } }
		public override bool CanKeepAspectRatio { get { return false; } }
		protected internal override Rectangle CreateValidBoxBoundsCore(Point point) {
			Rectangle boxBounds = Box.ActualSizeBounds;
			return Rectangle.FromLTRB(
				boxBounds.Left,
				boxBounds.Top,
				Math.Max(point.X, boxBounds.Left + 1),
				boxBounds.Bottom);
		}
		protected internal override Point CalculateOffsetCore(Point point) {
			Rectangle boxBounds = Box.ActualSizeBounds;
			return new Point(boxBounds.Right - point.X, 0);
		}
		public override void Accept(IRectangularObjectHotZoneVisitor visitor) {
			visitor.Visit(this);
		}
	}
#endregion
#region RectangularObjectRotationHotZone
	public class RectangularObjectRotationHotZone : RectangularObjectHotZone {
		Point lineEnd;
		public RectangularObjectRotationHotZone(Box box, PieceTable pieceTable, IGestureStateIndicator gestureIndicator)
			: base(box, pieceTable, gestureIndicator) {
		}
		public override RichEditCursor Cursor { get { return CalculateCursor(RichEditCursors.BeginRotate); } }
		public Point LineEnd { get { return lineEnd; } set { lineEnd = value; } }
		public override void Activate(RichEditMouseHandler handler, RichEditHitTestResult result) {
			RichEditRectangularObjectRotateMouseHandlerState state = handler.CreateRectangularObjectRotateState(this, result);
			handler.SwitchStateCore(state, Point.Empty);
		}
		protected internal override Rectangle CreateValidBoxBoundsCore(Point point) {
			return Box.ActualSizeBounds;
		}
		protected internal override Point CalculateOffsetCore(Point point) {
			Point rotationPoint = RectangleUtils.CenterPoint(Bounds);
			return new Point(rotationPoint.X - point.X, rotationPoint.Y - point.Y);
		}
		protected internal override Rectangle ForceKeepOriginalAspectRatio(Rectangle bounds, int desiredWidth, int desiredHeight) {
			return bounds;
		}
		public override void Accept(IRectangularObjectHotZoneVisitor visitor) {
			visitor.Visit(this);
		}
	}
#endregion
}
