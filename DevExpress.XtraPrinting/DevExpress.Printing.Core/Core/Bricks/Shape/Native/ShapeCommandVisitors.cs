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

using DevExpress.XtraPrinting;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
namespace DevExpress.XtraPrinting.Shape.Native {
	public interface IShapeCommandVisitor {
		void VisitShapeLineCommand(ShapeLineCommand command);
		void VisitShapeBezierCommand(ShapeBezierCommand command);
		void VisitShapeDrawPathCommand(ShapeDrawPathCommand command);
		void VisitShapeFillPathCommand(ShapeFillPathCommand command);
	}
	public class ShapeCommandPainter : IShapeCommandVisitor {
		#region inner classes
		interface IPathAction {
			void Action(IGraphics gr, GraphicsPath path, Brush brush, Pen pen);
		}
		class DrawPathAction : IPathAction {
			public static readonly DrawPathAction Instance = new DrawPathAction();
			private DrawPathAction() {
			}
			void IPathAction.Action(IGraphics gr, GraphicsPath path, Brush brush, Pen pen) {
				gr.DrawPath(pen, path);
			}
		}
		class FillPathAction : IPathAction {
			public static readonly FillPathAction Instance = new FillPathAction();
			private FillPathAction() {
			}
			void IPathAction.Action(IGraphics gr, GraphicsPath path, Brush brush, Pen pen) {
				gr.FillPath(brush, path);
			}
		}
		#endregion
		IGraphics graphics;
		Pen pen;
		Brush brush;
		public ShapeCommandPainter(IGraphics graphics, Pen pen, Brush brush) {
			this.graphics = graphics;
			this.pen = pen;
			this.brush = brush;
		}
		void IShapeCommandVisitor.VisitShapeLineCommand(ShapeLineCommand command) {
			if(pen.Width > 0)
				graphics.DrawLine(pen, command.StartPoint, command.EndPoint);
		}
		void IShapeCommandVisitor.VisitShapeBezierCommand(ShapeBezierCommand command) {
		}
		void IShapeCommandVisitor.VisitShapeDrawPathCommand(ShapeDrawPathCommand command) {
			if(pen.Width > 0)
				DoActionOnPath(command, DrawPathAction.Instance);
		}
		void IShapeCommandVisitor.VisitShapeFillPathCommand(ShapeFillPathCommand command) {
			DoActionOnPath(command, FillPathAction.Instance);
		}
		void DoActionOnPath(ShapePathCommand command, IPathAction action) {
			using(GraphicsPathFiller filler = new GraphicsPathFiller()) {
				command.Commands.Iterate(filler);
				if(command.IsClosed)
					filler.Path.CloseFigure();
				action.Action(graphics, filler.Path, brush, pen);
			}
		}
	}
	public abstract class CompositeCommandsVisitor : IShapeCommandVisitor {
		public abstract void VisitShapeLineCommand(ShapeLineCommand command);
		public abstract void VisitShapeBezierCommand(ShapeBezierCommand command);
		void IShapeCommandVisitor.VisitShapeDrawPathCommand(ShapeDrawPathCommand command) {
			HandlePathCommand(command);
		}
		void IShapeCommandVisitor.VisitShapeFillPathCommand(ShapeFillPathCommand command) {
			HandlePathCommand(command);
		}
		void HandlePathCommand(ShapePathCommand command) {
			command.Commands.Iterate(this);
		}
	}
	public abstract class ShapeCommandsTransformer : CompositeCommandsVisitor {
		protected PointF center;
		protected ShapeCommandsTransformer(PointF center) {
			this.center = center;
		}
		public override void VisitShapeLineCommand(ShapeLineCommand command) {
			TransformPointsCommand(command);
		}
		public override void VisitShapeBezierCommand(ShapeBezierCommand command) {
			TransformPointsCommand(command);
		}
		void TransformPointsCommand(ShapePointsCommand command) {
			int count = command.Points.Length;
			PointF originalPoint;
			for(int i = 0; i < count; i++) {
				originalPoint = command.Points[i];
				TransformPoint(command, new SizeF(originalPoint.X - center.X, originalPoint.Y - center.Y), i);
			}
		}
		protected abstract void TransformPoint(ShapePointsCommand command, SizeF vector, int i);
	}
	public class ShapeCommandsScaler : ShapeCommandsTransformer {
		#region static
		static float ScaleValue(float value, float scaleFactor) {
			return value * scaleFactor;
		}
		#endregion
		float scaleFactorX;
		float scaleFactorY;
		public ShapeCommandsScaler(PointF scaleCenter, float scaleFactorX, float scaleFactorY)
			: base(scaleCenter) {
			this.scaleFactorX = scaleFactorX;
			this.scaleFactorY = scaleFactorY;
		}
		protected override void TransformPoint(ShapePointsCommand command, SizeF vector, int i) {
			command.Points[i] = new PointF(center.X + ScaleValue(vector.Width, scaleFactorX), center.Y + ScaleValue(vector.Height, scaleFactorY));
		}
	}
	public class ShapeCommandsRotator : ShapeCommandsTransformer {
		float sin;
		float cos;
		public ShapeCommandsRotator(PointF rotateCenter, int angle)
			: base(rotateCenter) {
			double angleInRad = (double)ShapeHelper.DegToRad(angle);
			sin = (float)Math.Sin(angleInRad);
			cos = (float)Math.Cos(angleInRad);
		}
		protected override void TransformPoint(ShapePointsCommand command, SizeF vector, int i) {
			SizeF rotatedVector = new SizeF(cos * vector.Width + sin * vector.Height, -sin * vector.Width + cos * vector.Height);
			command.Points[i] = new PointF(center.X + rotatedVector.Width, center.Y + rotatedVector.Height);
		}
	}
	public class ShapeCommandsOfsetter : ShapeCommandsTransformer {
		PointF offset;
		public ShapeCommandsOfsetter(PointF offset)
			: base(PointF.Empty) {
			this.offset = offset;
		}
		protected override void TransformPoint(ShapePointsCommand command, SizeF vector, int i) {
			PointF originalPoint = command.Points[i];
			command.Points[i] = new PointF(originalPoint.X + offset.X, originalPoint.Y + offset.Y);
		}
	}
	public class CriticalPointsCalculator : CompositeCommandsVisitor {
		float maxX = float.MinValue;
		float maxY = float.MinValue;
		float minX = float.MaxValue;
		float minY = float.MaxValue;
		public float MaxX {
			get { return maxX; }
		}
		public float MaxY {
			get { return maxY; }
		}
		public float MinX {
			get { return minX; }
		}
		public float MinY {
			get { return minY; }
		}
		public override void VisitShapeLineCommand(ShapeLineCommand command) {
			UpdateCriticalValues(command);
		}
		public override void VisitShapeBezierCommand(ShapeBezierCommand command) {
			UpdateCriticalValues(command);
		}
		void UpdateCriticalValues(ShapePointsCommand command) {
			foreach(PointF point in command.Points) {
				maxX = Math.Max(maxX, point.X);
				minX = Math.Min(minX, point.X);
				maxY = Math.Max(maxY, point.Y);
				minY = Math.Min(minY, point.Y);
			}
		}
	}
	public class GraphicsPathFiller : IShapeCommandVisitor, IDisposable {
		#region static
		static void InvalidOperation() {
			ExceptionHelper.ThrowInvalidOperationException();
		}
		#endregion
		GraphicsPath path = new GraphicsPath();
		public GraphicsPath Path {
			get { return path; }
		}
		public GraphicsPathFiller() {
		}
		void IShapeCommandVisitor.VisitShapeLineCommand(ShapeLineCommand command) {
			path.AddLine(command.StartPoint, command.EndPoint);
		}
		void IShapeCommandVisitor.VisitShapeBezierCommand(ShapeBezierCommand command) {
			path.AddBezier(command.StartPoint, command.StartControlPoint, command.EndControlPoint, command.EndPoint);
		}
		void IShapeCommandVisitor.VisitShapeDrawPathCommand(ShapeDrawPathCommand command) {
			InvalidOperation();
		}
		void IShapeCommandVisitor.VisitShapeFillPathCommand(ShapeFillPathCommand command) {
			InvalidOperation();
		}
		void IDisposable.Dispose() {
			if(path != null) {
				path.Dispose();
				path = null;
			}
		}
	}
}
