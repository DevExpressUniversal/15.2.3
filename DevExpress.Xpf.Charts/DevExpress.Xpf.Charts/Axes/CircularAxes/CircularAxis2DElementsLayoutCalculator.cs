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
using System;
using System.Collections.Generic;
using DevExpress.Charts.Native;
using DevExpress.Xpf.Core.Native;
namespace DevExpress.Xpf.Charts.Native {
	public abstract class CircularAxis2DElementsLayoutCalculator {
		public static void CalculateLayout(Axis2DItem axisItem, Rect viewport, Rect diagramBounds, CircularDiagramRotationDirection direction, double startAngle, ref Thickness offsets) {
			CircularAxis2DElementsLayoutCalculator calculator;
			if (axisItem.Axis is CircularAxisY2D)
				calculator = new VerticalAxis2DElementsLayoutCalculator(axisItem, viewport, direction, startAngle, diagramBounds);
			else {
				calculator = new RoundAxis2DElementsLayoutCalculator(axisItem, viewport, direction, startAngle, diagramBounds);
			}
			calculator.CreateLayout();
		}
		protected readonly Axis2DItem axisItem;
		protected readonly Rect viewport;
		protected readonly Rect diagramBounds;
		protected readonly CircularDiagramRotationDirection rotationDirection;
		protected readonly double startAngle;
		protected IAxisMapping axisMapping;
		protected CircularAxis2DElementsLayoutCalculator(Axis2DItem axisItem, Rect viewport, Rect diagramBounds, CircularDiagramRotationDirection direction, double startAngle) {
			this.axisItem = axisItem;
			this.viewport = viewport;
			this.diagramBounds = diagramBounds;
			this.rotationDirection = direction;
			this.startAngle = startAngle;
		}
		void CreateLayout() {
			AxisBase axis = axisItem.Axis as AxisBase;
			Axis2DElementLayout axisLayout = new Axis2DElementLayout(CalculateAxisItemRect());
			IList<AxisLabelItem> items = axisItem.LabelItems;
			Rect labelRect;
			bool axisVisible = true;
			if (axis is CircularAxisY2D)
				if (((CircularAxisY2D)axis).Visible == false)
					axisVisible = false;
			if (axisVisible) {
				if (items == null || items.Count == 0)
					labelRect = RectExtensions.Zero;
				else if (axis.ActualLabel.Visible && items != null) {
					List<IAxisLabelLayout> layoutItems = new List<IAxisLabelLayout>();
					foreach (AxisLabelItem labelItem in items) {
						IAxisLabelLayout labelLayout = new CircularAxis2DLabelItemLayout(CalculateLabelItemRect(labelItem), labelItem);
						labelLayout.Visible = !labelItem.IsOutOfRange;
						layoutItems.Add(labelLayout);
					}
					AxisLabelsResolveOverlappingHelper.Process(layoutItems, axis, AxisAlignment.Near, true);
					for (int i = 0; i < layoutItems.Count; i++) {
						items[i].Visible = layoutItems[i].Visible;
						items[i].Layout = items[i].Visible ?  layoutItems[i] as Axis2DLabelItemLayout : null;
					}
					labelRect = Rect.Empty;
					foreach (AxisLabelItem labelItem in items)
						if (labelItem.Visible) {
							Rect labelItemRect = labelItem.Layout.Bounds;
							if (labelRect.IsEmpty)
								labelRect = labelItemRect;
							else
								labelRect.Union(labelItemRect);
						}
				}
				else {
					foreach (AxisLabelItem labelItem in items)
						labelItem.Layout = null;
					labelRect = RectExtensions.Zero;
				}
			}
			else {
				axisLayout = null;
				if (items != null)
					foreach (AxisLabelItem labelItem in items)
						labelItem.Layout = null;
				labelRect = RectExtensions.Zero;
			}
			axisItem.Layout = axisLayout;
			axisItem.LabelRect = labelRect;
		}
		protected abstract Rect CalculateLabelItemRect(AxisLabelItem labelItem);
		protected abstract Rect CalculateAxisItemRect();
	}
	public class VerticalAxis2DElementsLayoutCalculator : CircularAxis2DElementsLayoutCalculator {
		Rect axisItemRect;
		internal VerticalAxis2DElementsLayoutCalculator(Axis2DItem axisItem, Rect viewport, CircularDiagramRotationDirection direction, double startAngle, Rect diagramBounds)
			: base(axisItem, viewport, diagramBounds, direction, startAngle) {
			this.axisMapping = axisItem.Axis.CreateMapping(viewport);
		}
		protected override Rect CalculateAxisItemRect() {
			double offset = ((CircularAxisY2D)axisItem.Axis).TickmarksCrossAxis == true ? axisItem.TotalThickness / 2.0 : axisItem.Thickness / 2.0 + axisItem.MaxTickmarksLength;
			this.axisItemRect = new Rect(new Point(0.5 * (viewport.Left + viewport.Right) - offset, viewport.Top), new Size(axisItem.TotalThickness, 0.5 * viewport.Height));
			return axisItemRect;
		}
		protected override Rect CalculateLabelItemRect(AxisLabelItem labelItem) {
			Size labelSize = labelItem.Size;
			double normalizedStartAngle = GetNormalizedStartAngleInRadian();
			Vector directingVector = new Vector(Math.Cos(normalizedStartAngle), -Math.Sin(normalizedStartAngle));
			double lengthAlongAxis = axisMapping.GetAxisValue(labelItem.Value);
			Point center = viewport.CalcCenter();
			Point labelPosition = Vector.Add(directingVector * lengthAlongAxis, center);
			labelPosition.X -= labelSize.Width/2.0;
			labelPosition.Y -= labelSize.Height / 2.0;
			double labelDiagonal = Math.Sqrt(labelSize.Height*labelSize.Height + labelSize.Width*labelSize.Width);
			Vector tickmarkDirection = new Vector(directingVector.Y, -directingVector.X);
			tickmarkDirection.Normalize();
			labelPosition = Vector.Add(tickmarkDirection * (axisItem.MaxTickmarksLength + labelDiagonal/2.0), labelPosition);
			return new Rect(labelPosition, labelSize);
		}
		double GetNormalizedStartAngleInRadian() {
			double normalizedStartAngle = MathUtils.Degree2Radian(startAngle);
			if (rotationDirection == CircularDiagramRotationDirection.Clockwise)
				normalizedStartAngle = -normalizedStartAngle;
			normalizedStartAngle += Math.PI / 2.0;
			return normalizedStartAngle;
		}
	}
	public class RoundAxis2DElementsLayoutCalculator : CircularAxis2DElementsLayoutCalculator {
		CircularAxisMapping AxisMapping { get { return axisMapping as CircularAxisMapping; } }
		internal RoundAxis2DElementsLayoutCalculator(Axis2DItem axisItem, Rect viewport, CircularDiagramRotationDirection direction, double startAngle, Rect diagramBounds)
			: base(axisItem, viewport, diagramBounds, direction, startAngle) {
			this.axisMapping = axisItem.Axis.CreateMapping(viewport);
		}
		protected override Rect CalculateLabelItemRect(AxisLabelItem labelItem) {
			if (AxisMapping.IsLastValue(labelItem.Value)) {
				labelItem.Layout = null;
				return RectExtensions.Zero;
			}
			double normalAngleRadian = AxisMapping.GetNormalizedAngleRadian(labelItem.Value);
			double directingVectorXCoord = Math.Cos(normalAngleRadian);
			double directionVectorYCoord = -Math.Sin(normalAngleRadian);
			Point viewPortCenter = viewport.CalcCenter();
			Point labelLocation = AxisMapping.GetPointOnCircularAxis(labelItem.Value, viewPortCenter);
			Size labelSize = labelItem.Size;
			if (Math.Round(directingVectorXCoord, 12) == 0)
				labelLocation.X -= labelSize.Width / 2.0;
			else if (directingVectorXCoord < 0)
				labelLocation.X -= labelSize.Width;
			if (Math.Round(directionVectorYCoord, 12) == 0)
				labelLocation.Y -= labelSize.Height / 2.0;
			else if (directionVectorYCoord < 0)
				labelLocation.Y -= labelSize.Height;
			return new Rect(labelLocation, labelSize);
		}
		protected override Rect CalculateAxisItemRect() {
			return viewport;
		}
	}
}
