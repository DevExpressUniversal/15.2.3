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

using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;
#if !SL
using DevExpress.Xpf.Utils;
#else
using DevExpress.Xpf.Core.WPFCompatibility;
using PropertyMetadata = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyMetadata;
#endif
namespace DevExpress.Xpf.Core {
	public class LayoutTransformPanel : Panel {
		public static readonly DependencyProperty ClockwiseProperty =
			DependencyPropertyManager.Register("Clockwise", typeof(bool), typeof(LayoutTransformPanel),
				new PropertyMetadata(false, (d, e) => ((LayoutTransformPanel)d).OnClockwisePropertyChanged()));
		public static readonly DependencyProperty OrientationProperty =
			DependencyPropertyManager.Register("Orientation", typeof(Orientation), typeof(LayoutTransformPanel),
				new PropertyMetadata(Orientation.Vertical, (d, e) => ((LayoutTransformPanel)d).OnOrientationPropertyChanged()));
		public bool Clockwise {
			get { return (bool)GetValue(ClockwiseProperty); }
			set { SetValue(ClockwiseProperty, value); }
		}
		public Orientation Orientation {
			get { return (Orientation)GetValue(OrientationProperty); }
			set { SetValue(OrientationProperty, value); }
		}
		protected override Size MeasureOverride(Size availableSize) {
			if(Children.Count == 0)
				return Size.Empty;
			UIElement child = Children[0];
			child.Measure(GetCorrectSize(availableSize));
			return GetCorrectSize(child.DesiredSize);
		}
		protected override Size ArrangeOverride(Size finalSize) {
			if(Children.Count == 0)
				return Size.Empty;
			UIElement child = Children[0];
			Size arrangeSize = GetCorrectSize(finalSize);
			child.Arrange(new Rect(0, 0, arrangeSize.Width, arrangeSize.Height));
			RenderTransform = GetTransform(arrangeSize);
			return GetCorrectSize(arrangeSize);
		}
		Size GetCorrectSize(Size size) {
			return Orientation == Orientation.Vertical ?
				size : new Size(size.Height, size.Width);
		}
		Transform GetTransform(Size size) {
			return Orientation == Orientation.Horizontal ?
				GetTransformHorizontal(size) : GetTransformVertical(size);
		}
		Transform GetTransformHorizontal(Size size) {
			TransformGroup transform = new TransformGroup();
			if(Clockwise) {
				transform.Children.Add(new RotateTransform() { Angle = 90 });
				transform.Children.Add(new TranslateTransform() { X = size.Height });
			}
			else {
				transform.Children.Add(new RotateTransform() { Angle = -90 });
				transform.Children.Add(new TranslateTransform() { Y = size.Width });
			}
			return transform;
		}
		Transform GetTransformVertical(Size size) {
			return new RotateTransform();
		}
		void OnClockwisePropertyChanged() {
			InvalidateMeasure();
		}
		void OnOrientationPropertyChanged() {
			InvalidateMeasure();
		}
	}
}
