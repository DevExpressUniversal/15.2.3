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
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using DevExpress.Charts.Native;
namespace DevExpress.Xpf.Charts.Native {
	public abstract class Diagram3DDomain {
		static Matrix3D CreateOrthoMatrix(Point3D p1, Point3D p2) {
			Matrix3D pmatrix = new Matrix3D();
			double width = p2.X - p1.X;
			double height = p2.Y - p1.Y;
			double depth = p2.Z - p1.Z;
			pmatrix.M11 = 2.0 / width;
			pmatrix.M22 = 2.0 / height;
			pmatrix.M33 = -1.0 / depth;
			pmatrix.OffsetZ = -(p1.Z) / depth;
			pmatrix.M44 = 1.0;
			return pmatrix;
		}
		static Matrix3D CreateProjectMatrix(Point3D p1, Point3D p2) {
			Matrix3D pmatrix = new Matrix3D();
			double width = p2.X - p1.X;
			double height = p2.Y - p1.Y;
			double depth = p2.Z - p1.Z;
			double doubleDistance = 2.0 * p1.Z;
			pmatrix.M11 = doubleDistance / width;
			pmatrix.M22 = doubleDistance / height;
			pmatrix.M33 = -p2.Z / depth;
			pmatrix.M34 = -1.0;
			pmatrix.OffsetZ = -p1.Z * p2.Z / depth;
			pmatrix.M44 = 0.0;
			return pmatrix;
		}
		public static Matrix3D PerformRotation(Matrix3D rotationMatrix, double angleX, double angleY) {
			Quaternion qX = new Quaternion(new Vector3D(0, 1, 0), angleX);
			Quaternion qY = new Quaternion(new Vector3D(1, 0, 0), angleY);
			rotationMatrix.Rotate(qY);
			rotationMatrix.Rotate(qX);
			return rotationMatrix;
		}
		public static Matrix3D PerformRotation(Matrix3D rotationMatrix, double angleZ) {
			Quaternion qZ = new Quaternion(new Vector3D(0, 0, 1), angleZ);
			rotationMatrix.Rotate(qZ);
			return rotationMatrix;
		}
		readonly Diagram3D diagram;
		readonly Rect viewport;
		readonly IList<SeriesData> seriesDataList;
		readonly Diagram3DBox diagramBox;
		double viewRadius;
		Matrix3D withoutZoomAndScrollMatrix;
		Matrix3D transformMatrix;
		Matrix3D backwardTransformMatrix;
		Transform3D rotationOnlyTransform;
		Transform3D backwardRotationOnlyTransform;
		Transform3DGroup modelTransform;
		Transform3D backwardModelTransform;
		MatrixCamera camera;
		Point3D cameraPosition;
		Point3D relativeCameraPosition;
		protected Diagram3D Diagram { get { return diagram; } }
		protected Diagram3DBox DiagramBox { get { return diagramBox; } }
		public IList<SeriesData> SeriesDataList { get { return seriesDataList; } }
		public double Width { get { return diagramBox.Width; } }
		public double Height { get { return diagramBox.Height; } }
		public double Depth { get { return diagramBox.Depth; } }
		public Transform3D RotationOnlyTransform { get { return rotationOnlyTransform; } }
		public Transform3D BackwardRotationOnlyTransform { get { return backwardRotationOnlyTransform; } }
		public Transform3D BackwardModelTransform { get { return backwardModelTransform; } }
		public Transform3DGroup ModelTransform { get { return modelTransform; } }
		public Point3D RelativeCameraPosition { get { return relativeCameraPosition; } }
		public Point3D CameraPosition { get { return cameraPosition; } }
		public Rect Viewport { get { return viewport; } }
		public double ViewRadius { get { return viewRadius; } }
		public Diagram3DDomain(Diagram3D diagram, Rect viewport, IList<SeriesData> seriesDataList, Diagram3DBox diagramBox) {
			this.diagram = diagram;
			this.viewport = viewport;
			this.seriesDataList = seriesDataList;
			this.diagramBox = diagramBox;
			viewRadius = diagramBox.CalcViewRadius();
			modelTransform = new Transform3DGroup();
			modelTransform.Children.Add(diagramBox.CreateModelTransform());
			modelTransform.Children.Add(Diagram.ActualContentTransform);
			rotationOnlyTransform = MathUtils.CalcRotationTransform(modelTransform.Value);
			Matrix3D matrix = rotationOnlyTransform.Value;
			matrix.Invert();
			backwardRotationOnlyTransform = new MatrixTransform3D(matrix);
			camera = CreateCamera(true);
			transformMatrix = modelTransform.Value * camera.Transform.Value * camera.ViewMatrix * camera.ProjectionMatrix * CreateViewportMatrix();
			MatrixCamera originCamera = CreateCamera(false);
			withoutZoomAndScrollMatrix = modelTransform.Value * originCamera.Transform.Value * originCamera.ViewMatrix * originCamera.ProjectionMatrix * CreateViewportMatrix();
			backwardTransformMatrix = transformMatrix;
			backwardTransformMatrix.Invert();
			matrix = ModelTransform.Value;
			matrix.Invert();
			backwardModelTransform = new MatrixTransform3D(matrix);
			relativeCameraPosition = backwardModelTransform.Transform(CameraPosition);
		}
		Model3D CreateLighting() {
			Model3DGroup lightingModel = new Model3DGroup();
			foreach(Light templateLight in Diagram.ActualLighting) {
				Light light = templateLight.Clone();
				light.Transform = templateLight.Transform;
				lightingModel.Children.Add(light);
			}
			lightingModel.Transform = new ScaleTransform3D(Width, Height, Depth);
			return lightingModel;
		}
		Point3D CalcCameraPosAndBoundsAccordingZoomingAndScrolling(double distance, double viewOffsetX, double viewOffsetY, bool useCustomTransform, out Point3D p1, out Point3D p2) {
			p1 = new Point3D(viewOffsetX, viewOffsetY, distance);
			p2 = new Point3D(-viewOffsetX, -viewOffsetY, distance + 2.0 * viewRadius);
			double width = p2.X - p1.X;
			double height = p2.Y - p1.Y;
			double cx = useCustomTransform ? (p1.X + p2.X) / 2.0 - width * Diagram.HorizontalScrollPercent / 100.0 : (p1.X + p2.X) / 2.0;
			double cy = useCustomTransform ? (p1.Y + p2.Y) / 2.0 - height * Diagram.VerticalScrollPercent / 100.0 : (p1.Y + p2.Y) / 2.0;
			double dx = useCustomTransform ? width / Diagram.ZoomPercent * 50.0 : 0.5 * width;
			double dy = useCustomTransform ? height / Diagram.ZoomPercent * 50.0 : 0.5 * height;
			p1.X = cx - dx;
			p1.Y = cy - dy;
			p2.X = cx + dx;
			p2.Y = cy + dy;
			return new Point3D(cx, cy, distance + viewRadius);
		}
		Matrix3D CreateViewportMatrix() {
			double num2 = viewport.Width / 2;
			double num = viewport.Height / 2;
			double offsetX = viewport.X + num2;
			return new Matrix3D(num2, 0, 0, 0, 0, -num, 0, 0, 0, 0, 1, 0, offsetX, viewport.Y + num, 0, 1);
		}
		MatrixCamera CreateCamera(bool useCustomTransform) {
			double zoomFactor, viewOffsetX, viewOffsetY;
			double radianAngle = Diagram.PerspectiveAngle / 360.0 * Math.PI;
			if(Diagram.PerspectiveAngle > 0) {
				zoomFactor = 2.0 * viewRadius * (1.0 - Math.Sin(radianAngle)) / Math.Cos(radianAngle);
				viewOffsetX = -zoomFactor / 2.0;
				viewOffsetY = -zoomFactor / 2.0;
			}
			else {
				zoomFactor = viewRadius * 2.0;
				viewOffsetX = -viewRadius;
				viewOffsetY = -viewRadius;
			}
			if (viewport.Width > viewport.Height) {
				zoomFactor /= viewport.Height;
				viewOffsetX -= (viewport.Width - viewport.Height) * zoomFactor * 0.5;
			}
			else {
				zoomFactor /= viewport.Width;
				viewOffsetY -= (viewport.Height - viewport.Width) * zoomFactor * 0.5;
			}
			double distance = viewRadius;
			if(Diagram.PerspectiveAngle > 0)
				distance = 0.5 * Math.Min(viewport.Width, viewport.Height) * zoomFactor / Math.Tan(radianAngle);
			Point3D p1, p2;
			cameraPosition = CalcCameraPosAndBoundsAccordingZoomingAndScrolling(distance, viewOffsetX, viewOffsetY, useCustomTransform, out p1, out p2);
			Matrix3D vmatrix = new TranslateTransform3D(-cameraPosition.X, -cameraPosition.Y, -cameraPosition.Z).Value;
			Matrix3D pmatrix = Diagram.PerspectiveAngle > 0 ? CreateProjectMatrix(p1, p2) : CreateOrthoMatrix(p1, p2);
			return new MatrixCamera(vmatrix, pmatrix);
		}
		public virtual void ValidateSeriesPointsCache() {
			foreach(SeriesData seriesData in seriesDataList)
				seriesData.ValidateSeriesPointsCache(this, Diagram.ViewController.GetRefinedSeries(seriesData.Series));
		}
		public void Render(VisualContainer visualContainer) {
			Viewport3DVisual visual3D = new Viewport3DVisual();
			DrawingVisual visual2D = new DrawingVisual();
			ModelVisual3D modelVisual = new ModelVisual3D();
			Model3DGroup modelHolder = new Model3DGroup();
			modelHolder.Transform = ModelTransform;
			Render3DContent(modelHolder);
			Model3DGroup group = new Model3DGroup();
			group.Children.Add(CreateLighting());
			group.Children.Add(modelHolder);
			modelVisual.Content = group;
			ChartControl.SetDiagram3DHitTestInfo(group, diagram);
			visual3D.Camera = camera;
			visual3D.Children.Add(modelVisual);
			visual3D.Viewport = viewport;
			visual3D.Clip = new RectangleGeometry(viewport);
			visual2D.Clip = new RectangleGeometry(viewport);
			Render2DContent(visual2D);
			visualContainer.AddVisual(visual3D);
			visualContainer.AddVisual(visual2D);
		}
		public Point Project(Point3D point) {
			Point3D transPoint = transformMatrix.Transform(point);
			return new Point(transPoint.X, transPoint.Y);
		}
		public Point ProjectWithoutZoomAndScroll(Point3D point) {
			Point3D transPoint = withoutZoomAndScrollMatrix.Transform(point);
			return new Point(transPoint.X, transPoint.Y);
		}
		public Point3D Project3D(Point3D point) {
			return transformMatrix.Transform(point);
		}
		public Point3D RestoredFromProject3D(Point3D point) {
			return backwardTransformMatrix.Transform(point);
		}
		public ScaleTransform3D CreateScaleTransform(Point3D connectionPoint) {
			Matrix3D rotationMatrix = BackwardRotationOnlyTransform.Value;
			rotationMatrix.Translate((Vector3D)connectionPoint);
			Point3D p0 = new Point3D(0, 0, 0);
			Point3D p1 = new Point3D(1, 1, 0);
			Point3D[] points = new Point3D[] { p0, p1 };
			rotationMatrix.Transform(points);
			Point pt0 = Project(points[0]);
			Point pt1 = Project(points[1]);
			return new ScaleTransform3D(Math.Abs(p0.X - p1.X) / Math.Abs(pt0.X - pt1.X), Math.Abs(p0.Y - p1.Y) / Math.Abs(pt0.Y - pt1.Y), 1);
		}
		public virtual void OnModelAdd(object modelHolder, params Model3D[] models) {
		}
		protected abstract void Render3DContent(Model3DGroup modelHolder);
		protected abstract void Render2DContent(DrawingVisual visualHolder);
	}
	public abstract class Diagram3DBox {
		double width;
		double height;
		double depth;
		public double Width { get { return width; } protected set { width = value; } }
		public double Height { get { return height; } protected set { height = value; } }
		public double Depth { get { return depth; } protected set { depth = value; } }
		public virtual double CalcViewRadius() {
			return Math.Sqrt(width * width + height * height + depth * depth) * 0.5;
		}
		public abstract Transform3D CreateModelTransform();
	}
}
