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
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using DevExpress.Xpf.TreeMap.Native;
using DevExpress.Xpf.Core.Native;
namespace DevExpress.Xpf.TreeMap {
	public class TreeMapPanel : Panel {
		const double defaultWidth = 300.0;
		const double defaultHeight = 300.0;
		ITreeMapLayoutCalculator layoutCalculator;
		ITreeMapLayoutCalculator LayoutCalculator {
			get {
				if (layoutCalculator == null) {
					TreeMapItemsControl itemsControl = LayoutHelper.FindAmongParents<TreeMapItemsControl>(this, null);
					if (itemsControl != null)
						layoutCalculator = itemsControl.LayoutCalculator;
				}
				return layoutCalculator;
			}
		}
		protected override Size MeasureOverride(Size availableSize) {
			double constraintWidth = double.IsInfinity(availableSize.Width) ? defaultWidth : availableSize.Width;
			double constraintHeight = double.IsInfinity(availableSize.Height) ? defaultHeight : availableSize.Height;
			Size constraint = new Size(constraintWidth, constraintHeight);
			List<ITreeMapLayoutItem> items = new List<ITreeMapLayoutItem>();
			foreach (UIElement child in Children) {
				TreeMapItemPresentation presentation = child as TreeMapItemPresentation;
				if (presentation != null && presentation.LayoutItem != null)
					items.Add(presentation.LayoutItem);
			}
			if (LayoutCalculator != null)
				LayoutCalculator.CalculateLayout(items, constraint);
			foreach (UIElement child in Children) {
				Size elementSize = new Size(0, 0);
				TreeMapItemPresentation presentation = child as TreeMapItemPresentation;
				if (presentation != null && presentation.LayoutItem != null)
					elementSize = TreeMapPanelLayoutHelper.RoundChildLayout(presentation.LayoutItem.Layout, availableSize).Size;
				child.Measure(elementSize);
				if (presentation.TreeMapItem.Children.Count > 0)
					child.InvalidateParentsOfModifiedChildren();
			}
			return constraint;
		}
		protected override Size ArrangeOverride(Size finalSize) {
			foreach (UIElement child in Children) {
				Rect elementBounds = new Rect(0, 0, 0, 0);
				TreeMapItemPresentation presentation = child as TreeMapItemPresentation;
				if (presentation != null && presentation.LayoutItem != null && presentation.LayoutItem.Layout.Width <= finalSize.Width && presentation.LayoutItem.Layout.Height <= finalSize.Height)
					elementBounds = TreeMapPanelLayoutHelper.RoundChildLayout(presentation.LayoutItem.Layout, finalSize);
				child.Arrange(elementBounds);
			}
			return finalSize;
		}
	}
}
namespace DevExpress.Xpf.TreeMap.Native {
	public static class TreeMapPanelLayoutHelper {
		public static Rect RoundChildLayout(Rect childLayout, Size finalSize) {
			double scale = ScreenHelper.ScaleX;
			if (scale == 1.0)
				return RoundChildLayoutInternal(childLayout, finalSize);
			Rect scaledLayout = new Rect(childLayout.X * scale, childLayout.Y * scale, childLayout.Width * scale, childLayout.Height * scale);
			Size scaledSize = new Size(finalSize.Width * scale, finalSize.Height * scale);
			Rect scaledResult = RoundChildLayoutInternal(scaledLayout, scaledSize);
			return new Rect(scaledResult.X / scale, scaledResult.Y / scale, scaledResult.Width / scale, scaledResult.Height / scale);
		}
		static Rect RoundChildLayoutInternal(Rect childLayout, Size finalSize) {
			double x = Math.Floor(childLayout.X);
			double y = Math.Floor(childLayout.Y);
			double offsetX = childLayout.X + childLayout.Width - Math.Floor(childLayout.Width) - x;
			double offsetY = childLayout.Y + childLayout.Height - Math.Floor(childLayout.Height) - y;
			double width = Math.Round(offsetX, 3) >= 1 ? Math.Floor(childLayout.Width + 1) + 1 : Math.Floor(childLayout.Width) + 1;
			double height = Math.Round(offsetY, 3) >= 1 ? Math.Floor(childLayout.Height + 1) + 1 : Math.Floor(childLayout.Height) + 1;
			width = width + x > finalSize.Width ? width - 1 : width;
			height = height + y > finalSize.Height ? height - 1 : height;
			return new Rect(x, y, width, height);
		}
	}
}
