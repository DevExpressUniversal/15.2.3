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

using System.ComponentModel;
using System.Windows;
using System.Windows.Media;
using DevExpress.Xpf.Charts.Native;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Utils;
namespace DevExpress.Xpf.Charts {
	public abstract class PredefinedBar2DModelControl : PredefinedModelControl, IFinishInvalidation {		
	}
	public class BorderlessGradientBar2DModelControl : PredefinedBar2DModelControl {
		public BorderlessGradientBar2DModelControl() {
			DefaultStyleKey = typeof(BorderlessGradientBar2DModelControl);
		}
	}
	public class BorderlessSimpleBar2DModelControl : PredefinedBar2DModelControl {
		public BorderlessSimpleBar2DModelControl() {
			DefaultStyleKey = typeof(BorderlessSimpleBar2DModelControl);
		}
	}
	public class GradientBar2DModelControl : PredefinedBar2DModelControl {
	   public GradientBar2DModelControl() {
			DefaultStyleKey = typeof(GradientBar2DModelControl);
		}
	}
	public class SimpleBar2DModelControl : PredefinedBar2DModelControl {
		public SimpleBar2DModelControl() {
			DefaultStyleKey = typeof(SimpleBar2DModelControl);
		}
	}
	public class TransparentBar2DModelControl : PredefinedBar2DModelControl {
		public TransparentBar2DModelControl() {
			DefaultStyleKey = typeof(TransparentBar2DModelControl);
		}
	}
	public class FlatBar2DModelControl : PredefinedBar2DModelControl {
	   public FlatBar2DModelControl() {
			DefaultStyleKey = typeof(FlatBar2DModelControl);
		}
	}
	public class FlatGlassBar2DModelControl : PredefinedBar2DModelControl {
		public FlatGlassBar2DModelControl() {
			DefaultStyleKey = typeof(FlatGlassBar2DModelControl);
		}
	}
	public class OutsetBar2DModelControl : PredefinedBar2DModelControl {
		public OutsetBar2DModelControl() {
			DefaultStyleKey = typeof(OutsetBar2DModelControl);
		}
	}
	public class SteelColumnBar2DModelControl : PredefinedBar2DModelControl {
		public static readonly DependencyProperty InnerTopBrushProperty = DependencyPropertyManager.Register("InnerTopBrush", 
			typeof(Brush), typeof(SteelColumnBar2DModelControl));
		public static readonly DependencyProperty InnerBottomBrushProperty = DependencyPropertyManager.Register("InnerBottomBrush", 
			typeof(Brush), typeof(SteelColumnBar2DModelControl));
		[
		Category(Categories.Brushes)
		]
		public Brush InnerTopBrush {
			get { return (Brush)GetValue(InnerTopBrushProperty); }
			set { SetValue(InnerTopBrushProperty, value); }
		}
		[
		Category(Categories.Brushes)
		]
		public Brush InnerBottomBrush {
			get { return (Brush)GetValue(InnerBottomBrushProperty); }
			set { SetValue(InnerBottomBrushProperty, value); }
		}
		static LinearGradientBrush CreateInnerBrushTemplate() {
			LinearGradientBrush brush = new LinearGradientBrush();
			brush.StartPoint = new Point(0.5, 0.0);
			brush.EndPoint = new Point(0.5, 1.0);
			brush.GradientStops.Add(new GradientStop() { Color = Color.FromArgb(0xFF, 0x63, 0x63, 0x63), Offset = 0.0 });
			brush.GradientStops.Add(new GradientStop() { Color = Color.FromArgb(0xFF, 0xAC, 0xAC, 0xAC), Offset = 1.0 });
			return brush;
		}
		const double coverHeight = 22;
		readonly LinearGradientBrush innerBrushTemplate = CreateInnerBrushTemplate();
		public SteelColumnBar2DModelControl() {
			DefaultStyleKey = typeof(SteelColumnBar2DModelControl);
		}
		void UpdateInnerBrushes(double height, Color color) {
			double yOffset = coverHeight < height ? coverHeight / height : 1.0;
			InnerTopBrush = ColorHelper.MixColors(ColorUtils.InterpolateVerticalGradientBrush(innerBrushTemplate, 0, yOffset), color);
			InnerBottomBrush = ColorHelper.MixColors(ColorUtils.InterpolateVerticalGradientBrush(innerBrushTemplate, yOffset, 1), color);
		}
		protected override void UpdateBrushes(Color color) {
			UpdateInnerBrushes(ActualHeight, color);
		}
		protected override Size MeasureOverride(Size availableSize) {
			UpdateInnerBrushes(availableSize.Height, PointColor);
			return base.MeasureOverride(availableSize);
		}
	}
	public class Quasi3DBar2DModelControl : PredefinedBar2DModelControl {
		public Quasi3DBar2DModelControl() {
			DefaultStyleKey = typeof(Quasi3DBar2DModelControl);
		}		
	}
	public class GlassCylinderBar2DModelControl : PredefinedBar2DModelControl {
		public static readonly DependencyProperty RightEdgeSideSurfaceBrushProperty = DependencyPropertyManager.Register("RightEdgeSideSurfaceBrush", 
			typeof(Brush), typeof(GlassCylinderBar2DModelControl));
		public static readonly DependencyProperty RightEdgeBaseBrushProperty = DependencyPropertyManager.Register("RightEdgeBaseBrush", 
			typeof(Brush), typeof(GlassCylinderBar2DModelControl));
		public static readonly DependencyProperty MiddleEdgeSideSurfaceBrushProperty = DependencyPropertyManager.Register("MiddleEdgeSideSurfaceBrush", 
			typeof(Brush), typeof(GlassCylinderBar2DModelControl));
		public static readonly DependencyProperty MiddleEdgeBaseBrushProperty = DependencyPropertyManager.Register("MiddleEdgeBaseBrush", 
			typeof(Brush), typeof(GlassCylinderBar2DModelControl));
		public static readonly DependencyProperty LeftEdgeSideSurfaceBrushProperty = DependencyPropertyManager.Register("LeftEdgeSideSurfaceBrush", 
			typeof(Brush), typeof(GlassCylinderBar2DModelControl));
		public static readonly DependencyProperty LeftEdgeBaseBrushProperty = DependencyPropertyManager.Register("LeftEdgeBaseBrush", 
			typeof(Brush), typeof(GlassCylinderBar2DModelControl));
		static LinearGradientBrush CreateRightEdgeBrushTemplate() {
			LinearGradientBrush brush = new LinearGradientBrush();
			brush.StartPoint = new Point(0.499988, 1.22697);
			brush.EndPoint = new Point(0.499988, -0.261557);
			brush.GradientStops.Add(new GradientStop() { Color = Color.FromArgb(0x00, 0x00, 0x00, 0x00), Offset = 0.0 });
			brush.GradientStops.Add(new GradientStop() { Color = Color.FromArgb(0x34, 0x22, 0x1E, 0x1F), Offset = 0.36264 });
			brush.GradientStops.Add(new GradientStop() { Color = Color.FromArgb(0x34, 0x22, 0x1E, 0x1F), Offset = 0.785721 });
			brush.GradientStops.Add(new GradientStop() { Color = Color.FromArgb(0x00, 0x00, 0x00, 0x00), Offset = 1.0 });
			return brush;
		}
		static LinearGradientBrush CreateMiddleEdgeBrushTemplate() {
			LinearGradientBrush brush = new LinearGradientBrush();
			brush.StartPoint = new Point(0.49998, 0.991814);
			brush.EndPoint = new Point(0.49998, 0.00388777);
			brush.GradientStops.Add(new GradientStop() { Color = Color.FromArgb(0x00, 0x00, 0x00, 0x00), Offset = 0.0 });
			brush.GradientStops.Add(new GradientStop() { Color = Color.FromArgb(0x1A, 0x22, 0x1E, 0x1F), Offset = 0.36264 });
			brush.GradientStops.Add(new GradientStop() { Color = Color.FromArgb(0x1A, 0x22, 0x1E, 0x1F), Offset = 0.785721 });
			brush.GradientStops.Add(new GradientStop() { Color = Color.FromArgb(0x00, 0x00, 0x00, 0x00), Offset = 1.0 });
			return brush;
		}
		static LinearGradientBrush CreateLeftEdgeBrushTemplate() {
			LinearGradientBrush brush = new LinearGradientBrush();
			brush.StartPoint = new Point(0.499992, 0.992369);
			brush.EndPoint = new Point(0.499992, -0.00205491);
			brush.GradientStops.Add(new GradientStop() { Color = Color.FromArgb(0x00, 0xFF, 0xFF, 0xFF), Offset = 0.0 });
			brush.GradientStops.Add(new GradientStop() { Color = Color.FromArgb(0x5A, 0xFF, 0xFF, 0xFF), Offset = 0.36264 });
			brush.GradientStops.Add(new GradientStop() { Color = Color.FromArgb(0x5A, 0xFF, 0xFF, 0xFF), Offset = 0.785721 });
			brush.GradientStops.Add(new GradientStop() { Color = Color.FromArgb(0x00, 0xFF, 0xFF, 0xFF), Offset = 1.0 });
			return brush;
		}
		readonly LinearGradientBrush rightEdgeBrushTemplate = CreateRightEdgeBrushTemplate();
		readonly LinearGradientBrush middleEdgeBrushTemplate = CreateMiddleEdgeBrushTemplate();
		readonly LinearGradientBrush leftEdgeBrushTemplate = CreateLeftEdgeBrushTemplate();
		[
		Category(Categories.Brushes)
		]
		public Brush RightEdgeSideSurfaceBrush {
			get { return (Brush)GetValue(RightEdgeSideSurfaceBrushProperty); }
			set { SetValue(RightEdgeSideSurfaceBrushProperty, value); }
		}
		[
		Category(Categories.Brushes)
		]
		public Brush RightEdgeBaseBrush {
			get { return (Brush)GetValue(RightEdgeBaseBrushProperty); }
			set { SetValue(RightEdgeBaseBrushProperty, value); }
		}
		[
		Category(Categories.Brushes)
		]
		public Brush MiddleEdgeSideSurfaceBrush {
			get { return (Brush)GetValue(MiddleEdgeSideSurfaceBrushProperty); }
			set { SetValue(MiddleEdgeSideSurfaceBrushProperty, value); }
		}
		[
		Category(Categories.Brushes)
		]
		public Brush MiddleEdgeBaseBrush {
			get { return (Brush)GetValue(MiddleEdgeBaseBrushProperty); }
			set { SetValue(MiddleEdgeBaseBrushProperty, value); }
		}
		[
		Category(Categories.Brushes)
		]
		public Brush LeftEdgeSideSurfaceBrush {
			get { return (Brush)GetValue(LeftEdgeSideSurfaceBrushProperty); }
			set { SetValue(LeftEdgeSideSurfaceBrushProperty, value); }
		}
		[
		Category(Categories.Brushes)
		]
		public Brush LeftEdgeBaseBrush {
			get { return (Brush)GetValue(LeftEdgeBaseBrushProperty); }
			set { SetValue(LeftEdgeBaseBrushProperty, value); }
		}
		public GlassCylinderBar2DModelControl() {
			DefaultStyleKey = typeof(GlassCylinderBar2DModelControl);
		}
		void UpdateEdgeBrushes(double sideSurfaceHeight, Color pointColor) {
			double yOffset = GlassCylinderBar2DModel.CoverHalfHeight < sideSurfaceHeight ? sideSurfaceHeight / (sideSurfaceHeight + GlassCylinderBar2DModel.CoverHalfHeight) : 1.0;
			RightEdgeSideSurfaceBrush = ColorHelper.MixColors(ColorUtils.InterpolateVerticalGradientBrush(rightEdgeBrushTemplate, 0, yOffset), pointColor);
			RightEdgeBaseBrush = ColorHelper.MixColors(ColorUtils.InterpolateVerticalGradientBrush(rightEdgeBrushTemplate, yOffset, 1), pointColor);
			MiddleEdgeSideSurfaceBrush = ColorHelper.MixColors(ColorUtils.InterpolateVerticalGradientBrush(middleEdgeBrushTemplate, 0, yOffset), pointColor);
			MiddleEdgeBaseBrush = ColorHelper.MixColors(ColorUtils.InterpolateVerticalGradientBrush(middleEdgeBrushTemplate, yOffset, 1), pointColor);
			LeftEdgeSideSurfaceBrush = ColorHelper.MixColors(ColorUtils.InterpolateVerticalGradientBrush(leftEdgeBrushTemplate, 0, yOffset), pointColor);
			LeftEdgeBaseBrush = ColorHelper.MixColors(ColorUtils.InterpolateVerticalGradientBrush(leftEdgeBrushTemplate, yOffset, 1), pointColor);
		}
		protected override void UpdateBrushes(Color pointColor) {
			UpdateEdgeBrushes(ActualHeight, pointColor);
		}		
		protected override Size MeasureOverride(Size constraint) {
			UpdateEdgeBrushes(constraint.Height, PointColor);
			return base.MeasureOverride(constraint);
		}
	}
}
