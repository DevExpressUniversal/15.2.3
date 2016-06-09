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
using System.Windows.Media;
using System.Windows.Controls;
using DevExpress.Xpf.Charts.Native;
namespace DevExpress.Xpf.Charts.Native {
	public interface IAxis2DElement {
		AxisBase Axis { get; }
		bool Visible { get; }
	}
}
namespace DevExpress.Xpf.Charts {
	public class Axis2DElementsPanel : Panel {
		protected override Size MeasureOverride(Size availableSize) {
			foreach (UIElement element in Children) {
				Size size;
				IAxis2DElement axisElement = element as IAxis2DElement;
				if (axisElement != null && axisElement.Visible) {
					AxisBase axis = axisElement.Axis;
					size = (axis != null && axis.IsVertical) ? new Size(availableSize.Height, availableSize.Width) : availableSize;
				}
				else
					size = new Size(0, 0);
				element.Measure(size);
			}
			return availableSize;
		}
		protected override Size ArrangeOverride(Size finalSize) {
			foreach (UIElement element in Children) {
				IAxis2DElement axisElement = element as IAxis2DElement;
				bool isVertical;
				if (element != null) {
					AxisBase axis = axisElement.Axis;
					isVertical = axis != null && axis.IsVertical;
				}
				else
					isVertical = false;
				if (isVertical) {
					element.RenderTransform = new RotateTransform() { Angle = -90 };
					element.Arrange(new Rect(new Point(0, finalSize.Height), element.DesiredSize));
				}
				else {
					element.RenderTransform = new MatrixTransform() { Matrix = Matrix.Identity };
					element.Arrange(new Rect(new Point(0, 0), element.DesiredSize));
				}
			}
			return finalSize;
		}
	}
}
