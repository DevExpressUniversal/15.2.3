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
namespace DevExpress.Xpf.Charts.Native {
	public static class RenderHelper {
		public static TileBrush CreateMirrorBrush(UIElement element, bool isPseudo3D, Pane pane) {
			VisualBrush brush = new VisualBrush(element);
			brush.ViewboxUnits = BrushMappingMode.Absolute;
			double top = isPseudo3D ? pane.Viewport.Top : 0;
			brush.Viewbox = new Rect(0, top, element.DesiredSize.Width, element.DesiredSize.Height);
			brush.RelativeTransform = new ScaleTransform() { CenterX = 0.5, CenterY = 0.5, ScaleX = 1, ScaleY = -1 };
			return brush;
		}
		public static LinearGradientBrush CreateOpacityBrush(Pane pane, bool isPseudo3D) {
			LinearGradientBrush opacityBrush = new LinearGradientBrush();
			opacityBrush.StartPoint = new Point(0, 0);
			opacityBrush.EndPoint = new Point(0, 1);
			opacityBrush.GradientStops.Add(new GradientStop() { Color = Color.FromArgb(100, 0, 0, 0), Offset = 0 });
			opacityBrush.GradientStops.Add(new GradientStop() { Color = Color.FromArgb(0, 0, 0, 0), Offset = 1 });
			return opacityBrush;
		}
		public static Brush CreateBrush(ContentPresenter presenter, ISupportFlowDirection supportFlowDirection) {
			return CreateTransformedBrush(presenter, supportFlowDirection, null);
		}
		public static Brush CreateMirrorBrush(DrawingVisual seriesVisual) {
			return new VisualBrush(seriesVisual);
		}
		public static Brush CreateTransformedBrush(ContentPresenter presenter, ISupportFlowDirection supportFlowDirection, Transform transform) {
			Brush presenterBrush = new VisualBrush(presenter);
			TransformGroup presenterBrushTransform = new TransformGroup();
			if (transform != null && !transform.IsIdentity())
				presenterBrushTransform.Children.Add(transform);
			if (supportFlowDirection != null) {
				Transform directionTransform = supportFlowDirection.CreateDirectionTransform();
				if (directionTransform != null && !directionTransform.IsIdentity())
					presenterBrushTransform.Children.Add(directionTransform);
			}
			presenterBrush.RelativeTransform = presenterBrushTransform;
			return presenterBrush;
		}
	}
	public static class HitTestingHelper {
		public static HitTestFilterBehavior HitFilter(DependencyObject obj) {
			UIElement element = obj as UIElement;
			if (element != null && !element.IsHitTestVisible)
				return HitTestFilterBehavior.ContinueSkipSelf;
			return HitTestFilterBehavior.Continue;
		}
		public static IHitTestableElement GetParentHitTestableElement(DependencyObject obj) {
			DependencyObject parent = obj;
			while (parent != null && !(parent is ChartControl)) {
				IHitTestableElement hitTestableElement = parent as IHitTestableElement;
				if (hitTestableElement != null)
					return hitTestableElement;
				parent = VisualTreeHelper.GetParent(parent);
			}
			return null;
		}
	}
}
