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
using System.Windows.Controls;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Printing.Native;
using System.Windows.Shapes;
using DevExpress.Xpf.Core.Native;
namespace DevExpress.Xpf.Printing {
	public class HighlightingService : IHighlightingService {
		#region Fields and Properties
		Border highlightRect;
		Border HighlightRect {
			get {
				if(highlightRect == null) {
					highlightRect = new Border();
					highlightRect.BorderThickness = new Thickness(5);
					highlightRect.BorderBrush = CreateHatchBrush();
				}
				return highlightRect;
			}
		}
		#endregion
		#region IHighlightService members
		public void ShowHighlighting(FrameworkElement parent, FrameworkElement target) {
			if(!(parent is Canvas))
				throw new ArgumentException("parent");
			new OnLoadedScheduler().Schedule(() => ShowHighlightingCore((Canvas)parent, target), target);
		}
		public void HideHighlighting() {
			HighlightRect.RemoveFromVisualTree();
		}
		#endregion
		#region Methods
		void ShowHighlightingCore(Canvas parentCanvas, FrameworkElement target) {
			const int highlightRectOffset = 6;
			if(target == null)
				throw new ArgumentNullException("target");
			if(parentCanvas == null)
				throw new ArgumentNullException("parentCanvas");
			if(!target.IsInVisualTree() || !parentCanvas.IsInVisualTree())
				return;
			Point point = LayoutHelper.GetRelativeElementRect(target, parentCanvas).Location();
			point.X -= highlightRectOffset;
			point.Y -= highlightRectOffset;
			Canvas.SetLeft(HighlightRect, point.X);
			Canvas.SetTop(HighlightRect, point.Y);
			Canvas.SetZIndex(HighlightRect, byte.MaxValue);
			HighlightRect.Width = target.Width + highlightRectOffset * 2;
			HighlightRect.Height = target.Height + highlightRectOffset * 2;
			HighlightRect.Clip = new RectangleGeometry() {
				Rect = new Rect(
					new Point(),
					new Size(
						parentCanvas.Width > point.X ? parentCanvas.Width - point.X : 0d,
						parentCanvas.Height > point.Y ? parentCanvas.Height - point.Y : 0d
					)
				)
			};
			HideHighlighting();
			parentCanvas.Children.Add(HighlightRect);
		}
		static Brush CreateHatchBrush() {
			LinearGradientBrush brush = new LinearGradientBrush();
			brush.MappingMode = BrushMappingMode.Absolute;
			brush.SpreadMethod = GradientSpreadMethod.Repeat;
			brush.StartPoint = new Point(0, 0);
			brush.EndPoint = new Point(3.0, 3.0);
			GradientStop gs = new GradientStop();
			gs.Color = Colors.Gray;
			brush.GradientStops.Add(gs);
			gs = new GradientStop();
			gs.Color = Colors.Gray;
			gs.Offset = 0.5;
			brush.GradientStops.Add(gs);
			gs = new GradientStop();
			gs.Color = Colors.Transparent;
			gs.Offset = 0.5;
			brush.GradientStops.Add(gs);
			gs = new GradientStop();
			gs.Color = Colors.Transparent;
			gs.Offset = 1;
			brush.GradientStops.Add(gs);
			return brush;
		}
		#endregion
	}
}
