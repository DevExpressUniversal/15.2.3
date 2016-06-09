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
using DevExpress.Xpf.Gauges.Native;
namespace DevExpress.Xpf.Gauges {
	public class SymbolPanel : Panel {
		const double defaultWidth = 300.0;
		const double defaultHeight = 300.0;
		protected override Size MeasureOverride(Size availableSize) {
			double constraintWidth = double.IsInfinity(availableSize.Width) ? defaultWidth : availableSize.Width;
			double constraintHeight = double.IsInfinity(availableSize.Height) ? defaultHeight : availableSize.Height;
			Size constraint = new Size(constraintWidth, constraintHeight);
			Size size = new Size(0, 0);
			Visibility visibility = Visibility.Collapsed;
			foreach (UIElement child in Children) {
				FrameworkElement childElement = child as FrameworkElement;
				if (childElement != null) {
					ElementInfoBase ElementInfo = childElement.DataContext as ElementInfoBase;
					if (ElementInfo != null) {
						ElementInfo.CreateLayout(constraint);
						if (ElementInfo.Layout != null) {
							size = new Size(ElementInfo.Layout.Width.HasValue ? ElementInfo.Layout.Width.Value : constraint.Width,
											ElementInfo.Layout.Height.HasValue ? ElementInfo.Layout.Height.Value : constraint.Height);
							visibility = Visibility.Visible;
						}
					}
				}
				child.Visibility = visibility;
				child.Measure(size);
			}
			return constraint;
		}
		protected override Size ArrangeOverride(Size finalSize) {
			foreach (UIElement child in Children) {
				Rect childRect = new Rect(0, 0, 0, 0);
				FrameworkElement childElement = child as FrameworkElement;
				if (childElement != null) {
					ElementInfoBase ElementInfo = childElement.DataContext as ElementInfoBase;
					if (ElementInfo != null) {
						ElementInfo.CompleteLayout();
						if (ElementInfo.Layout != null) {
							Size size = new Size(ElementInfo.Layout.Width.HasValue ? ElementInfo.Layout.Width.Value : child.DesiredSize.Width,
												 ElementInfo.Layout.Height.HasValue ? ElementInfo.Layout.Height.Value : child.DesiredSize.Height);
							Point transformOrigin = ElementInfo.PresentationControl != null ? ElementInfo.PresentationControl.GetRenderTransformOrigin() : new Point(0, 0);
							Point location = MathUtils.CorrectLocationByTransformOrigin(ElementInfo.Layout.AnchorPoint, transformOrigin, size);
							childRect = new Rect(location, size);
							child.RenderTransformOrigin = transformOrigin;
							child.RenderTransform = ElementInfo.Layout.RenderTransform;
							child.Clip = ElementInfo.Layout.ClipGeometry;
						}
					}
				}
				child.Arrange(childRect);
			}
			return finalSize;
		}
	}
}
