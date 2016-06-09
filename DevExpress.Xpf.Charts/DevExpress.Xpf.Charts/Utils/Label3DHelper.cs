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

using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Windows.Media.Imaging;
using DevExpress.Charts.Native;
namespace DevExpress.Xpf.Charts.Native {
	public sealed class Label3DHelper {
		static Point3D CalcBorderLocation(ContentPresenter presenter, Vector3D direction, Rect rect) {
			try {
				CustomShape shape = new CustomShape(presenter);
				shape.Clear();
				GRealPoint2D? intersectionPoint = IntersectionUtils.CalcLineWithShapeIntersection(new GRealPoint2D(-direction.X, -direction.Y), new GRealPoint2D(), rect, shape);
				return intersectionPoint.HasValue ? new Point3D(rect.Location.X - intersectionPoint.Value.X, rect.Location.Y - intersectionPoint.Value.Y, 0) :
													new Point3D(rect.Location.X, rect.Location.Y, 0);
			}
			catch {
				return CalcRectangleLocation(direction, rect);
			}
		}
		public static Point3D CalcRectangleLocation(Vector3D direction, Rect rect) {
			Point3D location = new Point3D(rect.Left, rect.Top, 0);
			if (direction.Length == 0)
				return location;
			IntersectionInfo intersection = GeometricUtils.CalcLineSegmentWithRectIntersection(new GRealPoint2D(-direction.X, -direction.Y), new GRealPoint2D(),
				new GRealPoint2D(rect.BottomLeft.X, rect.BottomLeft.Y), new GRealPoint2D(rect.TopRight.X, rect.TopRight.Y));
			if (intersection.HasIntersection)
				location.Offset(-intersection.IntersectionPoint.X, -intersection.IntersectionPoint.Y, 0);
			return location;
		}
		public static Model3D CreateModel(ContentPresenter presenter, ISupportFlowDirection supportFlowDirection) {
			if (presenter == null)
				return null;
			ZPlaneRectangle planeRect = new ZPlaneRectangle(new Point3D(), presenter.DesiredSize.Width, presenter.DesiredSize.Height);
			MaterialGroup material = new MaterialGroup();
			Brush brush = RenderHelper.CreateBrush(presenter, supportFlowDirection);
			RenderOptions.SetCachingHint(brush, CachingHint.Cache);
			DiffuseMaterial diffuseMaterial = new DiffuseMaterial(brush);
			diffuseMaterial.AmbientColor = Colors.Black;
			diffuseMaterial.Color = Colors.Black;
			material.Children.Add(diffuseMaterial);
			material.Children.Add(new EmissiveMaterial(brush));
			planeRect.Material = material;
			return planeRect.GetModel();
		}
		public static LabelModelContainer CreateModelContainer(Diagram3DDomain domain, Model3D model, Point3D center, Matrix3D transform) {
			Model3DGroup modelHolder = new Model3DGroup();
			transform.Append(domain.CreateScaleTransform(center).Value);
			transform.Append(domain.BackwardRotationOnlyTransform.Value);
			transform.Translate(new Vector3D(center.X, center.Y, center.Z));
			modelHolder.Transform = new MatrixTransform3D(transform);
			modelHolder.Children.Add(model);
			return new LabelModelContainer(modelHolder, MathUtils.CalcDistance(domain.CameraPosition, domain.ModelTransform.Transform(center)));
		}
		public static LabelModelContainer CreateLabelModelContainer(Diagram3DDomain domain, ContentPresenter presenter, Model3D model, Point3D center, Vector3D direction, LabelRenderMode renderMode) {
			return CreateLabelModelContainer(domain, presenter, model, center, direction, renderMode, 0.0, new Point());
		}
		public static LabelModelContainer CreateLabelModelContainer(Diagram3DDomain domain, ContentPresenter presenter, Model3D model, Point3D center, Vector3D direction, LabelRenderMode renderMode, double angle, Point offset) {
			direction = domain.RotationOnlyTransform.Transform(direction * presenter.DesiredSize.Width * presenter.DesiredSize.Height);
			Rect rect = new Rect(new Point(-presenter.DesiredSize.Width / 2, -presenter.DesiredSize.Height / 2), new Size(presenter.DesiredSize.Width, presenter.DesiredSize.Height));
			Point3D location;
			if (direction.Length == 0)
				location = new Point3D(rect.Location.X, rect.Location.Y, 0);
			else if (renderMode == LabelRenderMode.Rectangle)
				location = CalcRectangleLocation(direction, rect);
			else {
				Border border = VisualTreeHelper.GetChild(presenter, 0) as Border;
				if (border != null && GraphicsUtils.IsSimpleBorder(border)) {
					IntersectionInfo intersection = GeometricUtils.CalcLineSegmentWithRoundedRectIntersection(new GRealPoint2D(-direction.X, -direction.Y), new GRealPoint2D(),
						new GRealPoint2D(rect.Left, rect.Top), new GRealPoint2D(rect.Right, rect.Bottom),
						border.CornerRadius.BottomLeft, border.CornerRadius.TopLeft, border.CornerRadius.TopRight, border.CornerRadius.BottomRight);
					location = intersection.HasIntersection ? 
						new Point3D(rect.Location.X - intersection.IntersectionPoint.X, rect.Location.Y - intersection.IntersectionPoint.Y, 0) :
						new Point3D(rect.Location.X, rect.Location.Y, 0);
				}
				location = CalcBorderLocation(presenter, direction, rect);
			}
			Transform3DGroup transformGroup = new Transform3DGroup();
			transformGroup.Children.Add(new RotateTransform3D(new AxisAngleRotation3D() { Axis = new Vector3D(0, 0, 1), Angle = 360 - angle }, presenter.DesiredSize.Width / 2, presenter.DesiredSize.Height / 2, 0.0));
			transformGroup.Children.Add(new TranslateTransform3D(location.X + offset.X, location.Y + offset.Y, location.Z));
			return CreateModelContainer(domain, model, center, transformGroup.Value);
		}
	}
	public sealed class LabelModelContainer {
		public static int CompareByDistance(LabelModelContainer x, LabelModelContainer y) {
			if (x == null)
				return y == null ? 0 : -1;
			else
				return y == null ? 1 : y.distance.CompareTo(x.distance);
		}
		readonly Model3D model;
		readonly double distance;
		Model3D connectorModel;
		public Model3D Model { get { return model; } }
		public Model3D ConnectorModel { 
			get { return connectorModel; } 
			set { connectorModel = value; } 
		}
		public LabelModelContainer(Model3D model, double distance) {
			this.model = model;
			this.distance = distance;
		}
	}
}
