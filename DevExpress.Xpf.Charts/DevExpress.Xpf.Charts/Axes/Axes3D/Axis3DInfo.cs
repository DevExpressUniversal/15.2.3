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
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using DevExpress.Utils;
using DevExpress.Charts.Native;
using DevExpress.Xpf.Core;
namespace DevExpress.Xpf.Charts.Native {
	public class Axis3DInfo {
		const int elementIndent = 3;
		readonly XYDiagram3DDomain domain;
		readonly Axis3D axis;
		readonly GridAndTextDataEx gridAndTextData;
		readonly AxisCache cache;
		readonly Axis3DMapping axisMapping;
		readonly BoxPlane planes;
		readonly double xOffset;
		readonly double yOffset;
		readonly double zOffset;
		readonly Vector direction;
		public Axis3DMapping AxisMapping { get { return axisMapping; } }
		double PlaneDepthFixed { get { return domain.Diagram.PlaneDepthFixed; } }
		public Axis3DInfo(XYDiagram3DDomain domain, Axis3D axis, GridAndTextDataEx gridAndTextData, AxisCache cache, Rect bounds) {
			this.domain = domain;
			this.axis = axis;
			this.gridAndTextData = gridAndTextData;
			this.cache = cache;
			axisMapping = new Axis3DMapping(axis, bounds);
			planes = domain.CalcVisiblePlanes();
			if (axis.IsVertical) {
				xOffset = -PlaneDepthFixed;
				if ((planes & BoxPlane.Right) == BoxPlane.Right)
					xOffset += domain.Width;
			}
			else
				yOffset = -PlaneDepthFixed;
			if ((planes & BoxPlane.Back) != 0)
				zOffset = domain.Depth - 2.0 * PlaneDepthFixed;
			Point3D far = CalcAxisPoint(Double.PositiveInfinity);
			Point3D near = CalcAxisPoint(Double.NegativeInfinity);
			Point projectedFar = domain.Project(far);
			Point projectedNear = domain.Project(near);
			Point directionStart = domain.Project(new Point3D((far.X + near.X) / 2, (far.Y + near.Y) / 2, ((planes & BoxPlane.Fore) != 0) ? domain.Depth - 2.0 * PlaneDepthFixed : 0));
			GRealPoint2D perpendicularPoint = new GRealPoint2D(directionStart.X - (projectedFar.Y - projectedNear.Y), directionStart.Y + (projectedFar.X - projectedNear.X));
			GRealPoint2D? directionEnd = GeometricUtils.CalcLinesIntersection(new GRealPoint2D(projectedNear.X, projectedNear.Y), new GRealPoint2D(projectedFar.X, projectedFar.Y), new GRealPoint2D(directionStart.X, directionStart.Y), perpendicularPoint, false);
			direction = directionEnd == null ? new Vector() : new Vector(directionEnd.Value.X - directionStart.X, directionEnd.Value.Y - directionStart.Y);
			if (ComparingUtils.CompareDoubles(direction.LengthSquared, 0, 0.001) == 0)
				if (axis.IsVertical) {
					double offset = domain.Width + 2.0 * PlaneDepthFixed;
					if ((planes & BoxPlane.Right) == BoxPlane.Right)
						offset = -offset;
					far.X += offset;
					near.X += offset;
					direction = (MathUtils.CalcCenter(projectedFar, projectedNear) - MathUtils.CalcCenter(domain.Project(far), domain.Project(near))).X < 0 ? new Vector(-1, 0) : new Vector(1, 0);
				}
				else
					direction = new Vector(0, 1);
			else
				direction.Normalize();
			if (!axis.IsVertical) {
				double value = (((IAxisData)axis).VisualRange.Max + ((IAxisData)axis).VisualRange.Min) / 2;
				direction = domain.Project(CalcCenter(value, direction * elementIndent, 1.5, new Vector(), new Size())) - domain.Project(CalcAxisPoint(value));			
				direction.Normalize();
			}
		}
		GeometryGroup CreateGridLinesGeometry(List<double> values, LineStyle lineStyle, double size, bool applyTransform) {
			bool shouldShiftPosition = lineStyle.Thickness % 2 == 1;
			GeometryGroup group = new GeometryGroup();
			foreach (double val in values) {
				double position = axisMapping.GetRoundedClampedAxisValue(val) + 1;
				if (shouldShiftPosition)
					position -= 0.5;
				group.Children.Add(new LineGeometry() { StartPoint = new Point(position, 0), EndPoint = new Point(position, size) });
			}
			if (applyTransform)
				group.Transform = axisMapping.Transform;
			return group;
		}
		Point3D CalcAxisPoint(double value) {
			Point point = axis.IsVertical ? domain.GetClippedDiagramPoint(Double.NegativeInfinity, value) : domain.GetClippedDiagramPoint(value, Double.NegativeInfinity);
			return new Point3D(point.X + xOffset, point.Y + yOffset, zOffset);
		}
		Point3D CalcCenter(double value, Vector offset) {
			Point3D projected = domain.Project3D(CalcAxisPoint(value));
			projected.Offset(offset.X, offset.Y, 0);
			return domain.RestoredFromProject3D(projected);
		}
		Point3D CalcCenter(double value, Vector offset, double labelTopLimitOffset, Vector angleDirection, Size labelSize) {
			Point3D center = CalcCenter(value, offset);
			if (axis.IsVertical)
				return center;
			Point3D projectedPoint = domain.Project3D(center);
			projectedPoint.Offset(angleDirection.X, angleDirection.Y, 0);
			Point3D location = Label3DHelper.CalcRectangleLocation(domain.RotationOnlyTransform.Transform(domain.RestoredFromProject3D(projectedPoint) - center), 
				new Rect(new Point(-labelSize.Width / 2, -labelSize.Height / 2), labelSize));
			Matrix3D transform = new TranslateTransform3D(location.X, location.Y, location.Z).Value;
			transform.Append(domain.CreateScaleTransform(center).Value);
			transform.Append(domain.BackwardRotationOnlyTransform.Value);
			transform.Translate(new Vector3D(center.X, center.Y, center.Z));
			Model3D model = new ZPlaneRectangle(new Point3D(), labelSize.Width, labelSize.Height).GetModel();
			model.Transform = new MatrixTransform3D(transform);
			double shift = model.Bounds.Y + model.Bounds.SizeY + labelTopLimitOffset - CalcAxisPoint(value).Y;
			if (shift <= 0)
				return center;
			center.Y -= shift;
			Point3D point = domain.ModelTransform.Transform(center);
			double distanceFactor = (domain.Width * domain.Width + domain.Height * domain.Height + domain.Depth * domain.Depth) / (2 * (point.X * point.X + point.Y * point.Y + point.Z * point.Z));
			if (distanceFactor >= 1.0) 
				return center;
			point.X *= distanceFactor;
			point.Y *= distanceFactor;
			point.Z *= distanceFactor;
			return domain.BackwardModelTransform.Transform(point);
		}
		Point GetRotationOffset(Size presenterSize) {
			double normalizedAngle = MathUtils.CancelAngle(360 - axis.ActualLabel.Angle);
			double radianAngle = MathUtils.Degree2Radian(normalizedAngle);
			GRealPoint2D calculatedOffset = new GRealPoint2D();
			GRealPoint2D leftTopPoint = new GRealPoint2D();
			GRealRect2D bounds;
			if (!axis.IsVertical && (direction.Y >= 0.0)) {
				bounds = new GRealRect2D(presenterSize.Width / 2, presenterSize.Height, presenterSize.Width, presenterSize.Height);
				calculatedOffset = AxisLabelRotationHelper.CalculateOffset(AxisLabelRotationHelper.CalculateRotationForTopNearPosition(normalizedAngle), bounds, radianAngle);
				leftTopPoint = AxisLabelRotationHelper.CalculateLeftTopPointForTopPosition(bounds, normalizedAngle, radianAngle, false);
			}
			if (!axis.IsVertical && (direction.Y < 0.0)) {
				bounds = new GRealRect2D(presenterSize.Width / 2, 0, presenterSize.Width, presenterSize.Height);
				calculatedOffset = AxisLabelRotationHelper.CalculateOffset(AxisLabelRotationHelper.CalculateRotationForBottomNearPosition(normalizedAngle), bounds, radianAngle);
				leftTopPoint = AxisLabelRotationHelper.CalculateLeftTopPointForBottomPosition(bounds, normalizedAngle, radianAngle, false);
			}
			if (axis.IsVertical && ((planes & BoxPlane.Left) != 0)) {
				bounds = new GRealRect2D(presenterSize.Width, presenterSize.Height / 2, presenterSize.Width, presenterSize.Height);
				calculatedOffset = AxisLabelRotationHelper.CalculateOffset(AxisLabelRotationHelper.CalculateRotationForLeftNearPosition(normalizedAngle), bounds, radianAngle);
				leftTopPoint = AxisLabelRotationHelper.CalculateLeftTopPointForLeftPosition(bounds, normalizedAngle, radianAngle, false);
			}
			if (axis.IsVertical && ((planes & BoxPlane.Right) != 0)) {
				bounds = new GRealRect2D(0, presenterSize.Height / 2, presenterSize.Width, presenterSize.Height);
				calculatedOffset = AxisLabelRotationHelper.CalculateOffset(AxisLabelRotationHelper.CalculateRotationForRightNearPosition(normalizedAngle), bounds, radianAngle);
				leftTopPoint = AxisLabelRotationHelper.CalculateLeftTopPointForRightPosition(bounds, normalizedAngle, radianAngle, false);
			}
			return new Point(leftTopPoint.X + calculatedOffset.X, leftTopPoint.Y + calculatedOffset.Y);
		}
		Vector CreateLabels(IList<LabelModelContainer> textElements, IList<AxisTextItem> items, double initialStaggeredSize) {
			Vector offset = direction * elementIndent + new Vector(0, initialStaggeredSize);
			double labelTopLimitOffset = XYDiagram3DBox.PredefinedWidth * domain.Diagram.HeightToWidthRatio * 0.002;
			Vector calculatedMaxStaggeredOffset = new Vector();
			Model3DGroup axislabelsGroup = new Model3DGroup();
			foreach (AxisTextItem item in items) {
				ContentPresenter presenter = cache.GetLabelPresenter(axis, item);
				Vector calculatedStaggeredOffset = new Vector(direction.X * presenter.DesiredSize.Width, direction.Y * presenter.DesiredSize.Height);
				if (calculatedStaggeredOffset.LengthSquared > calculatedMaxStaggeredOffset.LengthSquared)
					calculatedMaxStaggeredOffset = calculatedStaggeredOffset;
				Point3D centerPoint = CalcCenter(item.Value, offset, labelTopLimitOffset, calculatedStaggeredOffset, presenter.DesiredSize);
				Point3D projectedPoint = domain.Project3D(centerPoint);
				projectedPoint.Offset(calculatedStaggeredOffset.X, calculatedStaggeredOffset.Y, 0);
				Point3D shifted = domain.RestoredFromProject3D(projectedPoint);
				Vector3D labelOffset = shifted - centerPoint;
				Point rotationOffset = GetRotationOffset(presenter.DesiredSize);
				LabelModelContainer modelContainer = Label3DHelper.CreateLabelModelContainer(domain,
					presenter, cache.GetModel(axis.ActualLabel, presenter), centerPoint, labelOffset, LabelRenderMode.Rectangle, axis.ActualLabel.Angle, rotationOffset);
				axislabelsGroup.Children.Add(modelContainer.Model);
				textElements.Add(modelContainer);
			}
			if (((IInteractiveElement)axis).IsSelected && axislabelsGroup.Children.Count > 0) {
				Model3D boundingBox = CreateBoundingBox(axislabelsGroup.Bounds);
				axislabelsGroup.Children.Add(boundingBox);
				textElements.Add(new LabelModelContainer(boundingBox, 0));
			}
			ChartControl.SetAxis3DHitTestInfo(axislabelsGroup, axis);
			return calculatedMaxStaggeredOffset;
		}
		Model3D CreateBoundingBox(Rect3D bounds) {
			MeshGeometry3D mesh = new MeshGeometry3D();
			mesh.Positions = new Point3DCollection() {
				new Point3D(bounds.X, bounds.Y, bounds.Z),
				new Point3D(bounds.X + bounds.SizeX, bounds.Y, bounds.Z),
				new Point3D(bounds.X, bounds.Y + bounds.SizeY, bounds.Z),
				new Point3D(bounds.X + bounds.SizeX, bounds.Y + bounds.SizeY, bounds.Z),
				new Point3D(bounds.X, bounds.Y, bounds.Z + bounds.SizeZ),
				new Point3D(bounds.X + bounds.SizeX, bounds.Y, bounds.Z + bounds.SizeZ),
				new Point3D(bounds.X, bounds.Y + bounds.SizeY, bounds.Z + bounds.SizeZ),
				new Point3D(bounds.X + bounds.SizeX, bounds.Y + bounds.SizeY, bounds.Z + bounds.SizeZ)
			};
			mesh.TriangleIndices = new Int32Collection() { 2, 3, 1, 2, 1, 0, 7, 1, 3, 7, 5, 1, 6, 5, 7, 6, 4, 5, 6, 2, 0, 6, 0, 4, 2, 7, 3, 2, 6, 7, 0, 1, 5, 0, 5, 4 };
			return new GeometryModel3D(mesh, new DiffuseMaterial(new SolidColorBrush(VisualSelectionHelper.SelectionColor3D)));
		}
		public void RenderInterlaced(DrawingContext context, double size, bool applyTransform) {
			if (axis.Interlaced) {
				GeometryGroup interlacedGeometry = new GeometryGroup();
				IList<InterlacedData> interlacedData = gridAndTextData.GridData.InterlacedData;
				if (interlacedData.Count > 0) {
					foreach (InterlacedData data in interlacedData) {
						Rect rect = new Rect(new Point(axisMapping.GetRoundedClampedAxisValue(data.Near), 0), new Point(axisMapping.GetRoundedClampedAxisValue(data.Far) + 1, size));
						interlacedGeometry.Children.Add(new RectangleGeometry() { Rect =  rect });
					}
					if (applyTransform)
						interlacedGeometry.Transform = axisMapping.Transform;
				}
				context.DrawGeometry(axis.InterlacedBrush, null, interlacedGeometry);
			}
		}
		public void RenderGridLines(DrawingContext context, double size, bool applyTransform) {
			if (axis.GridLinesVisible) {
				Geometry gridLinesGeometry = CreateGridLinesGeometry(gridAndTextData.GridData.Items.VisibleValues, axis.ActualGridLinesLineStyle, size, applyTransform);
				if (gridLinesGeometry != null)
					context.DrawGeometry(null, axis.ActualGridLinesLineStyle.CreatePen(axis.GridLinesBrush), gridLinesGeometry);
			}
			if (axis.GridLinesMinorVisible) {
				Geometry minorGridLinesGeometry = CreateGridLinesGeometry(gridAndTextData.GridData.MinorValues, axis.ActualGridLinesMinorLineStyle, size, applyTransform);
				if (minorGridLinesGeometry != null)
					context.DrawGeometry(null, axis.ActualGridLinesMinorLineStyle.CreatePen(axis.GridLinesMinorBrush), minorGridLinesGeometry);
			}
		}
		public void CreateLabelsAndTitle(List<LabelModelContainer> textElements) {
			Vector offset = new Vector();
			if (axis.ActualLabel.Visible) {
				offset = CreateLabels(textElements, gridAndTextData.TextData.PrimaryItems, 0);
				offset.Y += CreateLabels(textElements, gridAndTextData.TextData.StaggeredItems, offset.Y).Y;
			}
			if (axis.IsTitleVisible) {
				ContentPresenter presenter = cache.GetTitlePresenter(axis);
				Point projectedFar = domain.Project(CalcAxisPoint(Double.PositiveInfinity));
				Point projectedNear = domain.Project(CalcAxisPoint(Double.NegativeInfinity));
				double projectedAxisLength = MathUtils.CalcDistance(projectedFar, projectedNear);
				double correction = projectedAxisLength < 1 ? 0 : ((IAxisData)axis).VisualRange.Delta * presenter.DesiredSize.Width / (projectedAxisLength * 2);
				double halfHeight = presenter.DesiredSize.Height / 2;
				double value;
				switch (axis.Title.Alignment) {
					case TitleAlignment.Near:
						value = ((IAxisData)axis).VisualRange.Min + correction;
						break;
					case TitleAlignment.Center:
						value = (((IAxisData)axis).VisualRange.Max + ((IAxisData)axis).VisualRange.Min) / 2;
						break;
					case TitleAlignment.Far:
						value = ((IAxisData)axis).VisualRange.Max - correction;
						break;
					default:
						ChartDebug.Fail("Unkown AxisTitle alignment.");
						goto case TitleAlignment.Center;
				}
				Point3D centerPoint = CalcCenter(value, offset + direction * (halfHeight + elementIndent));
				Matrix3D transformMatrix = new TranslateTransform3D(-presenter.DesiredSize.Width / 2, -halfHeight, 0).Value;
				double titleAngle = MathUtils.Radian2Degree(GeometricUtils.NormalizeRadian(Math.Atan2(projectedFar.Y - projectedNear.Y, projectedFar.X - projectedNear.X)));
				if (titleAngle > 90 && titleAngle < 270)
					titleAngle -= 180;
				transformMatrix.Append(new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(0, 0, 1), -titleAngle)).Value);
				textElements.Add(Label3DHelper.CreateModelContainer(domain, cache.GetModel(axis.Title, presenter), centerPoint, transformMatrix));
			}
		}
	}
}
