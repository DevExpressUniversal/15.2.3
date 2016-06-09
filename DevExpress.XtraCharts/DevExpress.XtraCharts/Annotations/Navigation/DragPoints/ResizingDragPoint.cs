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

using DevExpress.XtraCharts.Localization;
namespace DevExpress.XtraCharts.Native {
	public enum ResizeDirection {
		Left,
		LeftTop,
		Top,
		RightTop,
		Right,
		RightBottom,
		Bottom,
		LeftBottom
	}
	public abstract class ResizingDragPoint : AnnotationDragPoint {
		protected abstract ResizeDirection ResizeDirection { get; }
		protected override string DesignerHint { get { return ChartLocalizer.GetString(ChartStringId.MsgAnnotationResizingToolTip); } }
		public ResizingDragPoint(Annotation annotation, DiagramPoint point) : base(annotation, point) {
		}
		protected double GetCorrectedAngle() {
			if (Annotation.Angle > 0)
				return Annotation.Angle;
			return 360 + Annotation.Angle;
		}
		public override AnnotationOperation CreateOperation() {
			if (Annotation.ShapePosition is RelativePosition)
				return new RelativePositionAllResizing(Annotation, ResizeDirection);
			return new FreePositionAllResizing(Annotation, ResizeDirection);
		}
	}
	public class LeftTopDragPoint : ResizingDragPoint {
		protected override CursorType CanDragCursorType {
			get {
				double angle = GetCorrectedAngle();
				if ((angle > 22.5 && angle < 67.5) || (angle > 202.5 && angle < 247.5))
					return CursorType.SizeNS;
				if ((angle > 67.5 && angle < 112.5) || (angle > 247.5 && angle < 292.5))
					return CursorType.SizeNESW;
				if ((angle > 112.5 && angle < 157.5) || (angle > 292.5 && angle < 337.5))
					return CursorType.SizeWE;
				return CursorType.SizeNWSE;
			}
		}
		protected override ResizeDirection ResizeDirection { get { return ResizeDirection.LeftTop; } }
		public LeftTopDragPoint(Annotation annotation, DiagramPoint point)
			: base(annotation, point) {
		}
	}
	public class RightTopDragPoint : ResizingDragPoint {
		protected override CursorType CanDragCursorType {
			get {
				double angle = GetCorrectedAngle();
				if ((angle > 22.5 && angle < 67.5) || (angle > 202.5 && angle < 247.5))
					return CursorType.SizeWE;
				if ((angle > 67.5 && angle < 112.5) || (angle > 247.5 && angle < 292.5))
					return CursorType.SizeNWSE;
				if ((angle > 112.5 && angle < 157.5) || (angle > 292.5 && angle < 337.5))
					return CursorType.SizeNS;
				return CursorType.SizeNESW;
			}
		}
		protected override ResizeDirection ResizeDirection { get { return ResizeDirection.RightTop; } }
		public RightTopDragPoint(Annotation annotation, DiagramPoint point)
			: base(annotation, point) {
		}
	}
	public class RightBottomDragPoint : ResizingDragPoint {
		protected override CursorType CanDragCursorType {
			get {
				double angle = GetCorrectedAngle();
				if ((angle > 22.5 && angle < 67.5) || (angle > 202.5 && angle < 247.5))
					return CursorType.SizeNS;
				if ((angle > 67.5 && angle < 112.5) || (angle > 247.5 && angle < 292.5))
					return CursorType.SizeNESW;
				if ((angle > 112.5 && angle < 157.5) || (angle > 292.5 && angle < 337.5))
					return CursorType.SizeWE;
				return CursorType.SizeNWSE;
			}
		}
		protected override ResizeDirection ResizeDirection { get { return ResizeDirection.RightBottom; } }
		public RightBottomDragPoint(Annotation annotation, DiagramPoint point)
			: base(annotation, point) {
		}
	}
	public class LeftBottomDragPoint : ResizingDragPoint {
		protected override CursorType CanDragCursorType {
			get {
				double angle = GetCorrectedAngle();
				if ((angle > 22.5 && angle < 67.5) || (angle > 202.5 && angle < 247.5))
					return CursorType.SizeWE;
				if ((angle > 67.5 && angle < 112.5) || (angle > 247.5 && angle < 292.5))
					return CursorType.SizeNWSE;
				if ((angle > 112.5 && angle < 157.5) || (angle > 292.5 && angle < 337.5))
					return CursorType.SizeNS;
				return CursorType.SizeNESW;
			}
		}
		protected override ResizeDirection ResizeDirection { get { return ResizeDirection.LeftBottom; } }
		public LeftBottomDragPoint(Annotation annotation, DiagramPoint point)
			: base(annotation, point) {
		}
	}
	public abstract class SideDragPoint : ResizingDragPoint {
		readonly DiagramPoint point1;
		readonly DiagramPoint point2;
		protected DiagramPoint Point1 { get { return point1; } }
		protected DiagramPoint Point2 { get { return point2; } }
		public SideDragPoint(Annotation annotation, DiagramPoint point1, DiagramPoint point2)
			: base(annotation, new DiagramPoint((point1.X + point2.X) / 2, (point1.Y + point2.Y) / 2)) {
			this.point1 = point1;
			this.point2 = point2;
		}
	}
	public class LeftDragPoint : SideDragPoint {
		protected override CursorType CanDragCursorType {
			get {
				double angle = GetCorrectedAngle();
				if ((angle > 22.5 && angle < 67.5) || (angle > 202.5 && angle < 247.5))
					return CursorType.SizeNWSE;
				if ((angle > 67.5 && angle < 112.5) || (angle > 247.5 && angle < 292.5))
					return CursorType.SizeNS;
				if ((angle > 112.5 && angle < 157.5) || (angle > 292.5 && angle < 337.5))
					return CursorType.SizeNESW;
				return CursorType.SizeWE;
			}
		}
		protected override ResizeDirection ResizeDirection { get { return ResizeDirection.Left; } }
		public LeftDragPoint(Annotation annotation, DiagramPoint point1, DiagramPoint point2)
			: base(annotation, point1, point2) {
		}
		public override AnnotationOperation CreateOperation() {
			if (Annotation.ShapePosition is RelativePosition)
				return new RelativePositionHorizontalResizing(Annotation, ResizeDirection);
			return new FreePositionHorizontalResizing(Annotation, ResizeDirection);
		}
	}
	public class TopDragPoint : SideDragPoint {
		protected override CursorType CanDragCursorType {
			get {
				double angle = GetCorrectedAngle();
				if ((angle > 22.5 && angle < 67.5) || (angle > 202.5 && angle < 247.5))
					return CursorType.SizeNESW;
				if ((angle > 67.5 && angle < 112.5) || (angle > 247.5 && angle < 292.5))
					return CursorType.SizeWE;
				if ((angle > 112.5 && angle < 157.5) || (angle > 292.5 && angle < 337.5))
					return CursorType.SizeNWSE;
				return CursorType.SizeNS;
			}
		}
		protected override ResizeDirection ResizeDirection { get { return ResizeDirection.Top; } }
		public TopDragPoint(Annotation annotation, DiagramPoint point1, DiagramPoint point2)
			: base(annotation, point1, point2) {
		}
		public override AnnotationOperation CreateOperation() {
			if (Annotation.ShapePosition is RelativePosition)
				return new RelativePositionVerticalResizing(Annotation, ResizeDirection);
			return new FreePositionVerticalResizing(Annotation, ResizeDirection);
		}
	}
	public class RightDragPoint : SideDragPoint {
		protected override CursorType CanDragCursorType {
			get {
				double angle = GetCorrectedAngle();
				if ((angle > 22.5 && angle < 67.5) || (angle > 202.5 && angle < 247.5))
					return CursorType.SizeNWSE;
				if ((angle > 67.5 && angle < 112.5) || (angle > 247.5 && angle < 292.5))
					return CursorType.SizeNS;
				if ((angle > 112.5 && angle < 157.5) || (angle > 292.5 && angle < 337.5))
					return CursorType.SizeNESW;
				return CursorType.SizeWE;
			}
		}
		protected override ResizeDirection ResizeDirection { get { return ResizeDirection.Right; } }
		public RightDragPoint(Annotation annotation, DiagramPoint point1, DiagramPoint point2)
			: base(annotation, point1, point2) {
		}
		public override AnnotationOperation CreateOperation() {
			if (Annotation.ShapePosition is RelativePosition)
				return new RelativePositionHorizontalResizing(Annotation, ResizeDirection);
			return new FreePositionHorizontalResizing(Annotation, ResizeDirection);
		}
	}
	public class BottomDragPoint : SideDragPoint {
		protected override CursorType CanDragCursorType {
			get {
				double angle = GetCorrectedAngle();
				if ((angle > 22.5 && angle < 67.5) || (angle > 202.5 && angle < 247.5))
					return CursorType.SizeNESW;
				if ((angle > 67.5 && angle < 112.5) || (angle > 247.5 && angle < 292.5))
					return CursorType.SizeWE;
				if ((angle > 112.5 && angle < 157.5) || (angle > 292.5 && angle < 337.5))
					return CursorType.SizeNWSE;
				return CursorType.SizeNS;
			}
		}
		protected override ResizeDirection ResizeDirection { get { return ResizeDirection.Bottom; } }
		public BottomDragPoint(Annotation annotation, DiagramPoint point1, DiagramPoint point2)
			: base(annotation, point1, point2) {
		}
		public override AnnotationOperation CreateOperation() {
			if (Annotation.ShapePosition is RelativePosition)
				return new RelativePositionVerticalResizing(Annotation, ResizeDirection);
			return new FreePositionVerticalResizing(Annotation, ResizeDirection);
		}
	}
}
