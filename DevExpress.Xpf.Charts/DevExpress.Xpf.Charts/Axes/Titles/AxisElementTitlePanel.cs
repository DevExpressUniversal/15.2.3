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
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Utils;
namespace DevExpress.Xpf.Charts {
	[NonCategorized]
	public class AxisElementTitlePanel : Panel {
		public static readonly DependencyProperty TitleProperty = DependencyPropertyManager.Register("Title", 
			typeof(AxisElementTitleBase), typeof(AxisElementTitlePanel), new PropertyMetadata(null, TitleChanged));
		static void TitleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			AxisElementTitlePanel panel = d as AxisElementTitlePanel;
			if (panel != null)
				panel.InvalidateMeasure();
		}
		public AxisElementTitleBase Title {
			get { return (AxisElementTitleBase)GetValue(TitleProperty); }
			set { SetValue(TitleProperty, value); }
		}
		protected override Size MeasureOverride(Size availableSize) {
			AxisElementTitleBase title = Title;
			if (title == null)
				return base.MeasureOverride(availableSize);
			double width = 0;
			double height = 0;
			if (title.ShouldRotate) {
				availableSize = new Size(availableSize.Height, availableSize.Width);
				foreach (UIElement element in Children) {
					element.Measure(availableSize);
					Size desiredSize = element.DesiredSize;
					width = Math.Max(desiredSize.Height, width);
					height = Math.Max(desiredSize.Width, height);
				}
			}
			else
				foreach (UIElement element in Children) {
					element.Measure(availableSize);
					Size desiredSize = element.DesiredSize;
					width = Math.Max(desiredSize.Width, width);
					height = Math.Max(desiredSize.Height, height);
				}
			return new Size(width, height);
		}
		protected override Size ArrangeOverride(Size arrangeBounds) {
			AxisElementTitleBase title = Title;
			if (title == null)
				return base.ArrangeOverride(arrangeBounds);
			if (title.ShouldRotate) {
				double width = arrangeBounds.Width;
				double height = arrangeBounds.Height;
				RenderTransform = new RotateTransform { Angle = title.RotateAngle };
				RenderTransformOrigin = new Point(0.5, 0.5);
				foreach (UIElement element in Children) {
					Size elementSize = element.DesiredSize;
					double elementWidth = elementSize.Width;
					double elementHeight = elementSize.Height;
					element.Arrange(new Rect((width - elementWidth) / 2.0, (height - elementHeight) / 2.0 - 1, elementWidth, elementHeight));
				}
			}
			else {
				RenderTransform = new MatrixTransform() { Matrix = Matrix.Identity };
				foreach (UIElement element in Children) {
					Size elementSize = element.DesiredSize;
					element.Arrange(new Rect(0, 0, elementSize.Width, elementSize.Height));
				}
			}
			return arrangeBounds;
		}
	}
}
