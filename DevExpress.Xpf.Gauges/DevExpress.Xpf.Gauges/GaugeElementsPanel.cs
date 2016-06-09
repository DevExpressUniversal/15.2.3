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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using DevExpress.Xpf.Gauges.Native;
using DevExpress.Xpf.Core.Native;
namespace DevExpress.Xpf.Gauges {
	public class GaugeElementsPanel : Panel {
		const double defaultWidth = 300.0;
		const double defaultHeight = 300.0;
		protected override Size MeasureOverride(Size availableSize) {
			double constraintWidth = double.IsInfinity(availableSize.Width) ? defaultWidth : availableSize.Width;
			double constraintHeight = double.IsInfinity(availableSize.Height) ? defaultHeight : availableSize.Height;
			Size constraint = new Size(constraintWidth, constraintHeight);
			foreach (UIElement child in Children)
				child.Measure(constraint);
			return constraint;
		}
		protected override Size ArrangeOverride(Size finalSize) {
			foreach (UIElement child in Children) {
				IGaugeLayoutElement element = child as IGaugeLayoutElement;
				if (element != null && element.Layout != null) {
					child.RenderTransform = element.Layout.RenderTransform;
					child.RenderTransformOrigin = element.RenderTransformOrigin;
					child.Clip = element.Layout.ClipGeometry;
					Size childSize = new Size(element.Layout.Width.HasValue ? element.Layout.Width.Value : child.DesiredSize.Width,
						element.Layout.Height.HasValue ? element.Layout.Height.Value : child.DesiredSize.Height);
					Point location = MathUtils.CorrectLocationByTransformOrigin(element.Layout.AnchorPoint, element.RenderTransformOrigin, childSize);
					location.X += element.Offset.X;
					location.Y += element.Offset.Y;
					child.Arrange(new Rect(location, childSize));
				}
				else
					child.Arrange(new Rect(0, 0, 0, 0));
			}
			return finalSize;
		}
	}
	public class DigitalGaugeElementsPanel : Panel {
		const double defaultWidth = 300.0;
		const double defaultHeight = 300.0;
		DigitalGaugeModel Model { get { return DataContext is DigitalGaugeControl ? ((DigitalGaugeControl)DataContext).ActualModel : null; } }
		protected override Size MeasureOverride(Size availableSize) {
			double constraintWidth = double.IsInfinity(availableSize.Width) ? defaultWidth : availableSize.Width;
			double constraintHeight = double.IsInfinity(availableSize.Height) ? defaultHeight : availableSize.Height;
			Size constraint = new Size(constraintWidth, constraintHeight);
			double width = 0, height = 0;
			foreach (UIElement child in Children) {
				child.Measure(constraint);
				IGaugeLayoutElement element = child as IGaugeLayoutElement;
				if (element != null && element.InfluenceOnGaugeSize) {
					height = Math.Max(height, child.DesiredSize.Height);
					width += child.DesiredSize.Width;
				}
			}
			width += Model != null ? Model.InnerPadding.Left + Model.InnerPadding.Right : 0;
			height += Model != null ? Model.InnerPadding.Top + Model.InnerPadding.Bottom : 0;
			return new Size(Math.Min(width, constraintWidth), Math.Min(height, constraintHeight));
		}
		protected override Size ArrangeOverride(Size finalSize) {
			Rect rect = new Rect(0, 0, finalSize.Width, finalSize.Height);
			foreach (UIElement child in Children)
				child.Arrange(rect);
			return finalSize;
		}
	}
}
