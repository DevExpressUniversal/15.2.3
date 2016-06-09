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
using System.Drawing.Drawing2D;
using DevExpress.XtraGauges.Core.Base;
using DevExpress.XtraGauges.Core.Drawing;
using System.IO;
using System.Collections.Generic;
using System.Collections;
using DevExpress.Compatibility.System.Drawing;
using DevExpress.Compatibility.System.Drawing.Drawing2D;
namespace DevExpress.XtraGauges.Core.Primitive {
	public sealed class RenderingContext {
		public static IRenderingContext FromGraphics(Graphics g) {
			return new RenderingContextFromGraphics(g);
		}
		public static IRenderingContext FromStream(Stream s) {
			return new RenderingContextFromStream(s);
		}
		struct RenderingContextFromStream : IRenderingContext {
			Matrix matrixCore;
			Stream streamCore;
			public RenderingContextFromStream(Stream s) {
				this.matrixCore = new Matrix();
				this.streamCore = s;
			}
			public void Dispose() {
				if(matrixCore!=null) {
					matrixCore.Dispose();
					matrixCore = null;
				}
				streamCore = null;
			}
			Matrix IRenderingContext.Transform {
				get { return matrixCore; }
				set { matrixCore = value; }
			}
			Stream IRenderingContext.Stream {
				get { return streamCore; }
			}
			Graphics IRenderingContext.Graphics {
				get { return null; }
			}
		}
		struct RenderingContextFromGraphics : IRenderingContext {
			Graphics graphicsCore;
			public RenderingContextFromGraphics(Graphics g) {
				graphicsCore = g;
			}
			public void Dispose() {
				graphicsCore = null;
			}
			Matrix IRenderingContext.Transform {
				get { return graphicsCore.Transform; }
				set { graphicsCore.Transform = value; }
			}
			Graphics IRenderingContext.Graphics { 
				get { return graphicsCore; } 
			}
			Stream IRenderingContext.Stream {
				get { return null; }
			}
		}
	}
	public sealed class SafeGraphicsContext : IDisposable {
		GraphicsState savedState;
		IRenderingContext context;
		public SafeGraphicsContext(Graphics g) {
			savedState = g.Save();
			this.context = RenderingContext.FromGraphics(g);
		}
		public void Dispose() {
			context.Graphics.Restore(savedState);
			savedState = null;
		}
		public IRenderingContext Context {
			get { return context; }
		}
	}
	public sealed class SmartContext : IDisposable {
		GraphicsState savedState;
		IRenderingContext context;
		Matrix savedMatrix;
		public SmartContext(IRenderingContext context, IRenderableElement element) {
			this.context = context;
			if(Context.Graphics != null) savedState = context.Graphics.Save();
			if(Context.Stream != null) savedMatrix = context.Transform;
			Context.Transform = CalcWorkMatrix(Context.Transform, element);
			if(!element.ViewInfo.IsReady) element.CalcViewInfo(Context.Transform.Clone());
		}
		public void Dispose() {
			if(Context.Graphics != null) Context.Graphics.Restore(savedState);
			if(Context.Stream != null) Context.Transform = savedMatrix;
			context = null;
			savedState = null;
		}
		IRenderingContext Context {
			get { return context; }
		}
		Matrix CalcWorkMatrix(Matrix contextMatrix, IRenderableElement element) {
			Matrix workMatrix = (Matrix)element.TryGetValue(CacheKeys.TransformationMatrix);
			if(workMatrix!=null) return workMatrix.Clone();
			workMatrix = contextMatrix.Clone();
			if(!element.Transform.IsIdentity) workMatrix.Multiply(element.Transform, MatrixOrder.Prepend);
			element.CacheValue(CacheKeys.TransformationMatrix, workMatrix);
			return workMatrix;
		}
	}
	public class BasePrimitiveGeometry : BaseObject, ISupportGeometry {
		Matrix matrixCore;
		PointF locationCore = PointF.Empty;
		float angleCore = 0.0f;
		FactorF2D scaleFactorCore;
		int lockTransformCounterCore =0;
		TransformChangedHandler ownerOnTransformChanded;
		readonly FactorF2D IdentityScaleFactor = new FactorF2D(1.0f, 1.0f);
		public BasePrimitiveGeometry(TransformChangedHandler transformChanded) :base(){
			this.ownerOnTransformChanded = transformChanded;
		}
		protected override void OnCreate() {
			this.matrixCore = new Matrix();
			this.scaleFactorCore = IdentityScaleFactor;
		}
		protected override void OnDispose() {
			if(matrixCore!=null) {
				matrixCore.Dispose();
				matrixCore = null;
			}
			if(ownerOnTransformChanded!=null) {
				ownerOnTransformChanded = null;
			}
		}
		public Matrix Transform {
			get { return matrixCore; }
			set {
				if(Transform == value) return;
				SetMatrixCore(value);
			}
		}
		protected void SetMatrixCore(Matrix value) {
			this.matrixCore = value;
			if(!IsTransformLocked) RaiseTransformChanged();
		}
		public PointF Location {
			get { return locationCore; }
			set {
				if(locationCore==value) return;
				locationCore = value;
				OnTransformChanged();
			}
		}
		public float Angle {
			get { return angleCore; }
			set {
				if(angleCore==value) return;
				angleCore = value;
				OnTransformChanged();
			}
		}
		public FactorF2D ScaleFactor {
			get { return scaleFactorCore; }
			set {
				if(scaleFactorCore==value) return;
				scaleFactorCore = value;
				OnTransformChanged();
			}
		}
		public void ResetTransform() {
			BeginTransform();
			ScaleFactor = IdentityScaleFactor;
			Location = PointF.Empty;
			Angle = 0.0f;
			EndTransform();
		}
		protected virtual void OnTransformChanged() {
			if(IsTransformLocked || IsDisposing) return;
			SetMatrixCore(CalcLocalTransform());
		}
		protected void RaiseTransformChanged() {
			if(ownerOnTransformChanded!=null) ownerOnTransformChanded();
		}
		protected Matrix CalcLocalTransform() {
			double a	= Math.PI/180 * Angle;
			return MathHelper.CalcTransform(Location, ScaleFactor, (float)Math.Sin(a), (float)Math.Cos(a));
		}
		public bool IsTransformLocked {
			get { return lockTransformCounterCore>0; }
		}
		public void BeginTransform() {
			lockTransformCounterCore++;
		}
		public void EndTransform() {
			if(--lockTransformCounterCore == 0) OnTransformChanged();
		}
		public void CancelTransform() {
			lockTransformCounterCore--;
		}
	}
	public class BasePrimitiveHandler : BaseObject, ISupportInteraction {
		IRenderableElement ownerCore;
		public BasePrimitiveHandler(IRenderableElement owner) {
			this.ownerCore = owner;
		}
		protected override void OnCreate() {}
		protected override void OnDispose() {
			this.ownerCore = null;
		}
		protected IRenderableElement Owner {
			get { return ownerCore; }
		}
		public void ProcessMouseDown(MouseEventArgsInfo ea) { }
		public void ProcessMouseUp(MouseEventArgsInfo ea) { }
		public void ProcessMouseMove(MouseEventArgsInfo ea) { }
		public BasePrimitiveHitInfo CalcHitInfo(Point pt) {
			return CalcHitInfoCore(pt);
		}
		protected virtual BasePrimitiveHitInfo CalcHitInfoCore(Point pt) {
			BasePrimitiveHitInfo hitInfo = null;
			Owner.Accept(
				delegate(IElement<IRenderableElement> e) {
					if(e.Self.HitTestEnabled && e.Self.ViewInfo.HitTest(pt)) hitInfo = new BasePrimitiveHitInfo(e.Self, pt);
				}
			);
			return (hitInfo==null) ? BasePrimitiveHitInfo.Empty : hitInfo;
		}
	}
	public sealed class CollectionHelper {
		public static string[] GetNames<T>(DevExpress.XtraGauges.Core.Base.IReadOnlyCollection<T> collection) where T : class, INamed {
			string[] existingNames = new string[collection.Count];
			int i = 0;
			foreach(INamed obj in collection) {
				existingNames[i++] = obj.Name;
			}
			return existingNames;
		}
		public static bool NamesContains(string name, string[] names) {
			for(int i = 0; i < names.Length; i++) {
				if(names[i] == name) return true;
			}
			return false;
		}
	}
	public sealed class MathHelper {
		public static PointF GetCenterOfRect(RectangleF rect) {
			return new PointF(rect.Left + rect.Width / 2, rect.Top + rect.Height / 2);
		}
		public static PointF GetRadiusVector(float rx, float ry, float alpha) {
			return new PointF(rx * (float)Math.Cos(alpha * Math.PI / 180d), ry * (float)Math.Sin(alpha * Math.PI / 180d));
		}
		public static double CalcRadiusVectorAngle(PointF origin, PointF orientation) {
			double x = orientation.X - origin.X;
			double y = orientation.Y - origin.Y;
			double len = Math.Sqrt(x * x + y * y);
			return (y > 0) ? Math.Acos(x / len) : Math.PI * 2d - Math.Acos(x / len);
		}
		public static double CalcEllipticalRadiusVectorAngle(PointF2D origin, PointF2D orientation, float rx, float ry) {
			PointF circlePoint = new PointF(orientation.X, origin.Y + (orientation.Y - origin.Y) * rx / ry);
			return CalcRadiusVectorAngle(origin, circlePoint);
		}
		public static Matrix CalcMorphTransform(PointF shapePt1, PointF shapePt2, PointF2D targetPt1, PointF2D targetPt2) {
			double sourceX = shapePt2.X - shapePt1.X;
			double sourceY = shapePt2.Y - shapePt1.Y;
			double targetX = targetPt2.X - targetPt1.X;
			double targetY = targetPt2.Y - targetPt1.Y;
			double sourceLen = Math.Sqrt(sourceX * sourceX + sourceY * sourceY);
			double targetLen = Math.Sqrt(targetX * targetX + targetY * targetY);
			double sign = (sourceX * targetY - targetX * sourceY);
			double cosa = (targetX * sourceX + targetY * sourceY) / (sourceLen * targetLen);
			double sina = (sign < 0) ? -Math.Sqrt(1 - cosa * cosa) : Math.Sqrt(1 - cosa * cosa);
			float s = (float)(targetLen / sourceLen);
			Matrix temp = CalcTransform(PointF2D.Empty, new FactorF2D(s, s), (float)sina, (float)cosa);
			PointF[] pts = new PointF[] { shapePt1 };
			temp.TransformPoints(pts);
			temp.Dispose();
			return CalcTransform(targetPt1 - new SizeF(pts[0].X, pts[0].Y), new SizeF(s, s), (float)sina, (float)cosa);
		}
		public static double CalcVectorLength(PointF start, PointF end) {
			double x = end.X - start.X;
			double y = end.Y - start.Y;
			return Math.Sqrt(x * x + y * y);
		}
		public static double CalcVectorLength(PointF2D start, PointF2D end) {
			double x = end.X - start.X;
			double y = end.Y - start.Y;
			double result = Math.Sqrt(x * x + y * y);
			return result == 0 ? 0.0001 : result;
		}
		public static double CalcVectorLength(PointF v) {
			return Math.Sqrt(v.X * v.X + v.Y * v.Y);
		}
		public static Matrix CalcTransform(PointF2D origin, PointF2D orientation, FactorF2D scale) {
			double x = orientation.X - origin.X;
			double y = orientation.Y - origin.Y;
			double len = Math.Sqrt(x * x + y * y);
			return CalcTransform(origin, scale, (float)(y / len), (float)(x / len));
		}
		public static Matrix CalcTransform(PointF2D origin, FactorF2D scale, float sina, float cosa) {
			Matrix transform = new Matrix(scale.XFactor, 0.0f, 0.0f, scale.YFactor, 0.0f, 0.0f);
			if(cosa != 1 || sina != 0) {
				Matrix rotation = new Matrix(cosa, sina, -sina, cosa, 0.0f, 0.0f);
				transform.Multiply(rotation, MatrixOrder.Append);
				rotation.Dispose();
			}
			if(!origin.IsEmpty) {
				Matrix translation = new Matrix(1.0f, 0.0f, 0.0f, 1.0f, origin.X, origin.Y);
				transform.Multiply(translation, MatrixOrder.Append);
				translation.Dispose();
			}
			return transform;
		}
		public static PointF[] PointsFromRectangle(RectangleF rect) {
			return new PointF[] { 
				rect.Location, 
				rect.Location + new SizeF(rect.Size.Width, 0), 
				rect.Location + rect.Size,
				rect.Location + new SizeF(0, rect.Size.Height) 
			};
		}
		public static RectangleF CalcRelativeBoundBox(RectangleF rect, Matrix local) {
			return CalcRelativeBoundBox(PointsFromRectangle(rect), local);
		}
		public static RectangleF CalcBoundBox(IRenderableElement e) {
			List<PointF> list = new List<PointF>(128);
			foreach(BaseShape shape in e.Shapes)
				list.AddRange(ShapeHelper.GetPoints(shape));
			return CalcBoundBox(list.ToArray());
		}
		public static RectangleF CalcBoundBox(PointF[] points) {
			if(points.Length == 0) return RectangleF.Empty;
			float left = points[0].X;
			float top = points[0].Y;
			float right = points[0].X;
			float bottom = points[0].Y;
			for(int i = 0; i < points.Length; i++) {
				PointF p = points[i];
				left = Math.Min(left, p.X);
				top = Math.Min(top, p.Y);
				right = Math.Max(right, p.X);
				bottom = Math.Max(bottom, p.Y);
			}
			return new RectangleF(left, top, right - left, bottom - top);
		}
		public static RectangleF CalcRelativeBoundBox(PointF[] points, Matrix local) {
			if(local != null) local.TransformPoints(points);
			return CalcBoundBox(points);
		}
		public static PointF PointToModelPoint(IElement<IRenderableElement> e, PointF clientPoint) {
			Stack<Matrix> transformationStack = new Stack<Matrix>();
			Matrix resultTransform = new Matrix();
			IElement<IRenderableElement> element = e;
			while(element != null) {
				if(!element.Self.Transform.IsIdentity) transformationStack.Push(element.Self.Transform.Clone());
				element = element.Parent;
			}
			while(transformationStack.Count > 0) {
				resultTransform.Multiply(transformationStack.Pop());
			}
			resultTransform.Invert();
			PointF[] pts = new PointF[] { clientPoint };
			resultTransform.TransformPoints(pts);
			resultTransform.Dispose();
			return pts[0];
		}
		public static PointF[] ModelPointsToPoints(IElement<IRenderableElement> e, PointF[] clientPoints) {
			Stack<Matrix> transformationStack = new Stack<Matrix>();
			Matrix resultTransform = new Matrix();
			IElement<IRenderableElement> element = e;
			while(element != null) {
				if(!element.Self.Transform.IsIdentity) transformationStack.Push(element.Self.Transform.Clone());
				element = element.Parent;
			}
			while(transformationStack.Count > 0) {
				Matrix m = transformationStack.Pop();
				resultTransform.Multiply(m);
			}
			PointF[] pts = new PointF[clientPoints.Length];
			clientPoints.CopyTo(pts, 0);
			resultTransform.TransformPoints(pts);
			resultTransform.Dispose();
			return pts;
		}
	}
	public sealed class ShapeHelper {
		public static RectangleF GetShapeBounds(BaseShape shape, bool forceFlatten) {
			RectangleF res = RectangleF.Empty;
			shape.Accept(
				delegate(BaseShape s) {
					s.BeginUpdate();
					GraphicsPath path = s.Path;
					if(!BaseShape.IsEmptyPath(path) && path.PointCount > 0) {
						if(forceFlatten || (s is PathShape)) {
							path = path.Clone() as GraphicsPath;
							path.Flatten(null, 0.15f);
							RectangleF newRec = path.GetBounds();
							res = res.IsEmpty ? newRec : RectangleF.Union(res, newRec);
							path.Dispose();
						}
						else {
							RectangleF pathBounds = path.GetBounds();
							if(!pathBounds.IsEmpty)
								res = res.IsEmpty ? pathBounds : RectangleF.Union(res, pathBounds);
						}
					}
					s.CancelUpdate();
				}
			);
			return res;
		}
		public static RectangleF GetShapeBounds(BaseShape shape) {
			return GetShapeBounds(shape, false);
		}
		public static IEnumerable<PointF> GetPoints(BaseShape shape) {
			List<PointF> points = new List<PointF>();
			shape.Accept(
				delegate(BaseShape s) {
					if(s is ComplexShape || s.IsEmpty) return;
					s.BeginUpdate();
					if(s is PathShape) {
						GraphicsPath path = s.Path;
						if(path.PointCount > 0) {
							path = s.Path.Clone() as GraphicsPath;
							path.Flatten(null, 0.15f);
							points.AddRange(path.PathPoints);
							path.Dispose();
						}
					}
					else {
						if(s is PolygonShape)
							points.AddRange(((PolygonShape)s).Points);
						else {
							RectangleF pathBounds = s.BoundingBox;
							if(!pathBounds.IsEmpty) points.AddRange(MathHelper.PointsFromRectangle(pathBounds));
						}
					}
					s.CancelUpdate();
				}
			);
			return points;
		}
		public static PointF[] GetPoints(BaseShape shape,bool withTransform) {
			int n = shape.ShapePoints.Count;
			PointF[] points = new PointF[n];
			for(int i =0; i<n; i++) points[i] = shape.ShapePoints[i].Point;
			if(!withTransform) return points;
			if(!shape.Transform.IsIdentity) shape.Transform.TransformPoints(points);
			return points;
		}
		public static byte[] GetPointTypes(BaseShape shape) {
			int n = shape.ShapePoints.Count;
			byte[] types = new byte[n];
			for(int i =0; i<n; i++) types[i] = (byte)shape.ShapePoints[i].PointType;
			return types;
		}
	}
}
