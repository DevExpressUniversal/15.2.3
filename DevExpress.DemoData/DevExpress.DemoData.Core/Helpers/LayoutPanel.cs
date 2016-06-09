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
namespace DevExpress.DemoData.Helpers {
	public class RectClass {
		public Point Location { get; set; }
		public Size Size { get; set; }
		public Rect Rect { get { return new Rect(Location, Size); } }
		public double Width { get { return Size.Width; } }
		public double Height { get { return Size.Height; } }
		public double Left { get { return Location.X; } }
		public double Top { get { return Location.Y; } }
		public double Right { get { return Location.X + Size.Width; } }
		public double Bottom { get { return Location.Y + Size.Height; } }
	}
	public class LayoutPanel : Panel {
		#region Dependency Properties
		public static readonly DependencyProperty ExtraMarginProperty;
		static LayoutPanel() {
			Type ownerType = typeof(LayoutPanel);
			ExtraMarginProperty = DependencyProperty.RegisterAttached("ExtraMargin", typeof(Thickness), ownerType, new PropertyMetadata(new Thickness()));
		}
		#endregion
		public static Thickness GetExtraMargin(UIElement child) { return (Thickness)child.GetValue(ExtraMarginProperty); }
		public static void SetExtraMargin(UIElement child, Thickness v) { child.SetValue(ExtraMarginProperty, v); }
		protected void SetOpacity(UIElement child, double opacity, bool isHitTestVisible) {
			if(child == null) return;
			child.Opacity = opacity;
			child.IsHitTestVisible = isHitTestVisible;
		}
		protected void SetScale(UIElement child, double x, double y, double originX, double originY) {
			if(child == null) return;
			ScaleTransform transform = child.RenderTransform as ScaleTransform;
			if(transform == null) {
				transform = new ScaleTransform();
				child.RenderTransform = transform;
				child.RenderTransformOrigin = new Point(originX, originY);
			}
			transform.ScaleX = x;
			transform.ScaleY = y;
		}
		protected void SetTranslate(UIElement child, double x, double y) {
			if(child == null) return;
			TranslateTransform transform = child.RenderTransform as TranslateTransform;
			if(transform == null) {
				transform = new TranslateTransform();
				child.RenderTransform = transform;
			}
			transform.X = x;
			transform.Y = y;
		}
		protected void SetClip(UIElement child, Geometry clip) {
			if(child == null) return;
			child.Clip = clip;
		}
		protected Size GetSize(double width, double height) {
			return new Size(width < 0.0 ? 0.0 : width, height < 0.0 ? 0.0 : height);
		}
		protected void MeasureChild(RectClass rect, UIElement child, bool preserveWidth, bool preserveHeight) {
			MeasureChild(rect, child, preserveWidth, preserveHeight, 1.0);
		}
		protected void MeasureChild(RectClass rect, UIElement child, bool preserveWidth, bool preserveHeight, double extraMarginFactor) {
			Size childSize = new Size(preserveWidth ? rect.Width : double.PositiveInfinity, preserveHeight ? rect.Height : double.PositiveInfinity);
			if(child != null) {
				Thickness extraMargin = GetExtraMargin(child, extraMarginFactor);
				double extraMarginWidth = extraMargin.Left + extraMargin.Right;
				double extraMarginHeight = extraMargin.Top + extraMargin.Bottom;
				child.Measure(GetSize(childSize.Width - extraMarginWidth, childSize.Height - extraMarginHeight));
				childSize = GetSize(child.DesiredSize.Width + extraMarginWidth, child.DesiredSize.Height + extraMarginHeight);
				childSize = new Size(childSize.Width > rect.Width ? rect.Width : childSize.Width, childSize.Height > rect.Height ? rect.Height : childSize.Height);
			}
			double childWidth = double.IsInfinity(childSize.Width) ? 0.0 : childSize.Width;
			double childHeight = double.IsInfinity(childSize.Height) ? 0.0 : childSize.Height;
			rect.Size = new Size(preserveWidth && !double.IsInfinity(rect.Width) ? rect.Width : childWidth, preserveHeight && !double.IsInfinity(rect.Height) ? rect.Height : childHeight);
		}
		protected void ArrangeChild(RectClass rect, UIElement child) {
			ArrangeChild(rect, child, 1.0);
		}
		protected void ArrangeChild(RectClass rect, UIElement child, double extraMarginFactor) {
			if(child == null) return;
			Thickness extraMargin = GetExtraMargin(child, extraMarginFactor);
			double extraMarginWidth = extraMargin.Left + extraMargin.Right;
			double extraMarginHeight = extraMargin.Top + extraMargin.Bottom;
			child.Arrange(CorrectRect(new Rect(new Point(rect.Left + extraMargin.Left, rect.Top + extraMargin.Top), GetSize(rect.Width - extraMarginWidth, rect.Height - extraMarginHeight))));
		}
		Thickness GetExtraMargin(UIElement child, double factor) {
			Thickness extraMargin = GetExtraMargin(child);
			double extraMarginLeft = Math.Floor(factor * extraMargin.Left);
			double extraMarginTop = Math.Floor(factor * extraMargin.Top);
			double extraMarginRight = Math.Floor(factor * extraMargin.Right);
			double extraMarginBottom = Math.Floor(factor * extraMargin.Bottom);
			return new Thickness(extraMarginLeft, extraMarginTop, extraMarginRight, extraMarginBottom);
		}
		protected void UpdateChild(object oldValue, object newValue, int zIndex) {
			UIElement oldChild = (UIElement)oldValue;
			UIElement newChild = (UIElement)newValue;
			if(oldValue != null)
				Children.Remove(oldChild);
			if(newValue != null) {
				Canvas.SetZIndex(newChild, zIndex);
				Children.Add(newChild);
			}
		}
		protected Rect CorrectRect(Rect rect) {
			return new Rect(new Point(double.IsNaN(rect.Left) || double.IsInfinity(rect.Left) ? 0.0 : rect.Left, double.IsNaN(rect.Top) || double.IsInfinity(rect.Top) ? 0.0 : rect.Top), CorrectSize(new Size(rect.Width, rect.Height)));
		}
		protected Size CorrectSize(Size initialSize) {
			return double.IsInfinity(initialSize.Width) || double.IsNaN(initialSize.Width) || double.IsInfinity(initialSize.Height) || double.IsNaN(initialSize.Height) ? new Size(0.0, 0.0) : initialSize;
		}
	}
}
