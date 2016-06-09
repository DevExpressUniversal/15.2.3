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
using DevExpress.Xpf.Core.Native;
namespace DevExpress.Xpf.Charts{
	public class FibonacciLabelItemsControl : ChartItemsControl {
		protected override DependencyObject GetContainerForItemOverride() {
			base.GetContainerForItemOverride();
			return new IndicatorLabelPresentation();
		}
		protected override void PrepareContainerForItemOverride(DependencyObject element, object item) {
			base.PrepareContainerForItemOverride(element, item);
			IndicatorLabelPresentation presentation = element as IndicatorLabelPresentation;
			IndicatorLabelItem labelItem = item as IndicatorLabelItem;
			if (presentation != null && labelItem != null)
				presentation.LabelItem = labelItem;
		}
	}
	public class IndicatorLabelElementsPanel : Panel {
		Pane Pane {
			get {
				Pane parentItemsControl = LayoutHelper.FindParentObject<Pane>(this);
				return parentItemsControl == null ? null : parentItemsControl as Pane;
			}
		}
		protected override Size MeasureOverride(Size availableSize) {
			if (Pane != null)
				foreach (IndicatorLabelPresentation presentation in Children) 
					presentation.Measure(Pane.Diagram.Rotated ? new Size(availableSize.Height, availableSize.Width) : availableSize);
			return availableSize;
		}
		protected override Size ArrangeOverride(Size finalSize) {
			if (Pane != null)
				foreach (IndicatorLabelPresentation presentation in Children)
					if (presentation.LabelItem.Layout != null) {
						Point point = presentation.LabelItem.Layout.CenterLabelLocation;
						RotateTransform rotation = new RotateTransform() { Angle = presentation.LabelItem.Layout.ClockwiseAngleInDegees };
						presentation.RenderTransform = rotation;
						presentation.RenderTransformOrigin = presentation.LabelItem.Layout.RotationOrigin;
						Point arrangePoint = Pane.ViewportTransform.Transform(point);
						arrangePoint -= new Vector(presentation.DesiredSize.Width / 2, presentation.DesiredSize.Height / 2);
						presentation.Arrange(new Rect(arrangePoint, presentation.DesiredSize));
					}
			return finalSize;
		}
	}
}
