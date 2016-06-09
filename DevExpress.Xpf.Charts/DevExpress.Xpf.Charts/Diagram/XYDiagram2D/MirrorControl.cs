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
using DevExpress.Xpf.Utils;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Charts.Native;
namespace DevExpress.Xpf.Charts {
	[NonCategorized]
	public class ChartMirrorControl : ChartElementBase {
		public static readonly DependencyProperty SeriesProperty = DependencyPropertyManager.Register("Series", typeof(UIElement), typeof(ChartMirrorControl), new PropertyMetadata(SeriesChanged));
		public static readonly DependencyProperty MirrorBrushProperty = DependencyPropertyManager.Register("MirrorBrush", typeof(Brush), typeof(ChartMirrorControl));
		public static readonly DependencyProperty MirrorOpacityBrushProperty = DependencyPropertyManager.Register("MirrorOpacityBrush", typeof(Brush), typeof(ChartMirrorControl));
		public static readonly DependencyProperty IsPseudo3DProperty = DependencyPropertyManager.Register("IsPseudo3D", typeof(bool), typeof(ChartMirrorControl));
		static void SeriesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			ChartMirrorControl mirrorControl = d as ChartMirrorControl;
			if (mirrorControl != null)
				mirrorControl.InvalidateArrange();
		}
		public UIElement Series {
			get { return (UIElement)GetValue(SeriesProperty); }
			set { SetValue(SeriesProperty, value); }
		}
		public Brush MirrorBrush {
			get { return (Brush)GetValue(MirrorBrushProperty); }
			set { SetValue(MirrorBrushProperty, value); }
		}
		public Brush MirrorOpacityBrush {
			get { return (Brush)GetValue(MirrorOpacityBrushProperty); }
			set { SetValue(MirrorOpacityBrushProperty, value); }
		}
		public bool IsPseudo3D {
			get { return (bool)GetValue(IsPseudo3DProperty); }
			set { SetValue(IsPseudo3DProperty, value); }
		}
		Pane Pane { get { return DataContext as Pane; } }
		public ChartMirrorControl() {
			DefaultStyleKey = typeof(ChartMirrorControl);
		}
		Brush GetMirrorBrush(UIElement element) {
			if (element != null && element.RenderSize.Width > 0 && element.RenderSize.Height > 0) {
				TileBrush brush = RenderHelper.CreateMirrorBrush(element, IsPseudo3D, Pane);
				brush.Stretch = Stretch.None;
				brush.AlignmentX = AlignmentX.Left;
				brush.AlignmentY = AlignmentY.Bottom;
				return brush;
			}
			return null;
		}
		protected override Size MeasureOverride(Size availableSize) {
			Pane pane = Pane;
			if (pane == null)
				return base.MeasureOverride(availableSize);
			Size constraint = new Size(availableSize.Width, pane.MirrorHeight);
			base.MeasureOverride(constraint);
			return constraint;
		}
		protected override Size ArrangeOverride(Size finalSize) {
			Pane pane = Pane;
			if (pane != null && pane.MirrorHeight > 0 && finalSize.Width > 0 && 
				Series != null &&		  Series.RenderSize.Height > 0 && Series.RenderSize.Width > 0) {
				MirrorBrush = GetMirrorBrush(Series);
				MirrorOpacityBrush = RenderHelper.CreateOpacityBrush(pane, IsPseudo3D);
			}
			return base.ArrangeOverride(finalSize);
		}
	}
}
