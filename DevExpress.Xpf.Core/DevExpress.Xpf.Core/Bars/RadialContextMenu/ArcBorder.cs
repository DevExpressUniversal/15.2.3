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
using System.Windows.Media;
using System.Windows.Shapes;
using DevExpress.Xpf.Core.Native;
namespace DevExpress.Xpf.Bars {
	public class ArcBorder : Shape {
		#region static
		public static readonly DependencyProperty ClipToSectorBoundsProperty =
			DependencyProperty.RegisterAttached("ClipToSectorBounds", typeof(bool), typeof(ArcBorder), new PropertyMetadata(false, OnClipToSectorBoundsPropertyChanged));
		public static readonly DependencyProperty ThicknessProperty =
			DependencyProperty.Register("Thickness", typeof(double), typeof(ArcBorder), new PropertyMetadata(double.NaN));
		public static readonly DependencyProperty TopPaddingProperty =
			DependencyProperty.Register("TopPadding", typeof(double), typeof(ArcBorder), new FrameworkPropertyMetadata(0d, FrameworkPropertyMetadataOptions.AffectsMeasure));
		public static readonly DependencyProperty GapProperty =
			DependencyProperty.Register("Gap", typeof(double), typeof(ArcBorder), new FrameworkPropertyMetadata(0d, FrameworkPropertyMetadataOptions.AffectsMeasure));
		public static readonly DependencyProperty InnerRadiusProperty =
			DependencyProperty.Register("InnerRadius", typeof(double), typeof(ArcBorder), new FrameworkPropertyMetadata(0d, FrameworkPropertyMetadataOptions.AffectsMeasure));
		public static bool GetClipToSectorBounds(DependencyObject obj) { return (bool)obj.GetValue(ClipToBoundsProperty); }
		public static void SetClipToSectorBounds(DependencyObject obj, bool value) { obj.SetValue(ClipToBoundsProperty, value); }
		private static void OnClipToSectorBoundsPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			if(d is FrameworkElement) {
				((FrameworkElement)d).SizeChanged += RadialMenuItemsPanel_ClipToBoundsElementSizeChanged;
				UpdateClipRegion(d as FrameworkElement);
			}
		}
		private static void UpdateClipRegion(FrameworkElement frameworkElement) {
			if(!frameworkElement.IsArrangeValid)
				return;
			frameworkElement.ClipToBounds = true;
			Point rightArcPoint = CalcRightArcPoint(frameworkElement.ActualHeight, 0, 0, frameworkElement.ActualWidth / 2);
			frameworkElement.Clip = new PathGeometry(new PathFigure[] {
				new PathFigure(new Point(frameworkElement.ActualWidth / 2 , frameworkElement.ActualHeight), new PathSegment[] {					
					new LineSegment(new Point(frameworkElement.ActualWidth / 2 - rightArcPoint.X, rightArcPoint.Y), true),
					new ArcSegment(new Point(frameworkElement.ActualWidth / 2 + rightArcPoint.X, rightArcPoint.Y), new Size(frameworkElement.ActualHeight, frameworkElement.ActualHeight), 0, false, SweepDirection.Clockwise, true)
				}, true)});
		}
		static void RadialMenuItemsPanel_ClipToBoundsElementSizeChanged(object sender, SizeChangedEventArgs e) {
			UpdateClipRegion(sender as FrameworkElement);
		}
		public static Point CalcRightArcPoint(double r, double topPadding, double rightPadding, double w) {
			if(r == 0) {
				return new Point(0, 0);
			}
			if(w == 0) {
				return new Point(0, Math.Min(r, topPadding));
			}
			double a_x = Math.Min(r, w);
			double a_y = Math.Sqrt(r * r - a_x * a_x);
			double r_min = rightPadding * r / a_x;
			double r1 = Math.Max(r_min, r - topPadding);
			if(r1 == 0) {
				return new Point(0, r);
			}
			double b_x = r1 * a_x / r;
			double b_y = r1 * a_y / r;
			double c_y = (rightPadding * b_x + b_y * Math.Sqrt(r1 * r1 - rightPadding * rightPadding)) / r1;
			double c_x = Math.Sqrt(r1 * r1 - c_y * c_y);
			return new Point(c_x, r - c_y);
		}
		#endregion
		#region dep props
		public double Thickness {
			get { return (double)GetValue(ThicknessProperty); }
			set { SetValue(ThicknessProperty, value); }
		}
		public double TopPadding {
			get { return (double)GetValue(TopPaddingProperty); }
			set { SetValue(TopPaddingProperty, value); }
		}
		public double Gap {
			get { return (double)GetValue(GapProperty); }
			set { SetValue(GapProperty, value); }
		}
		public double InnerRadius {
			get { return (double)GetValue(InnerRadiusProperty); }
			set { SetValue(InnerRadiusProperty, value); }
		}
		#endregion
		protected override Geometry DefiningGeometry {
			get {
				double r1 = Math.Max(InnerRadius, ActualHeight - TopPadding);
				double r2 = 0;
				if(Thickness.IsNotNumber())
					r2 = Math.Min(r1, InnerRadius);
				else
					r2 = Math.Max(InnerRadius, r1 - Thickness);
				double centerX = ActualWidth / 2;
				Point rightBottomPoint = CalcRightArcPoint(ActualHeight, ActualHeight - r2, Gap, centerX);
				Point rightTopPoint = CalcRightArcPoint(ActualHeight, ActualHeight - r1, Gap, centerX);
				return new PathGeometry(new PathFigure[] {
					new PathFigure(new Point(centerX - rightBottomPoint.X, rightBottomPoint.Y), new PathSegment[] {
						new LineSegment(new Point(centerX - rightTopPoint.X, rightTopPoint.Y), true),
						new ArcSegment(new Point(centerX+rightTopPoint.X, rightTopPoint.Y), new Size(r1,r1), 0, false, SweepDirection.Clockwise, true),
						new LineSegment(new Point(centerX + rightBottomPoint.X, rightBottomPoint.Y), true),
						new ArcSegment(new Point(centerX-rightBottomPoint.X, rightBottomPoint.Y), new Size(r2, r2), 0, false, SweepDirection.Counterclockwise, true)
					}, true)});
			}
		}
		protected override Size MeasureOverride(Size constraint) {
			base.MeasureOverride(constraint);
			return new Size(0, 0);
		}
	}
}
