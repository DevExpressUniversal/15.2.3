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
namespace DevExpress.Xpf.Core.Native {
	public delegate bool SkipLayout(UIElement elem);
	public delegate bool IsLastChild(UIElement elem);
	public static class DockPanelLayoutHelper {
		public static Size MeasureDockPanelLayout(FrameworkElement parent, Size constraint) {
			return MeasureDockPanelLayout(parent, constraint, null);
		}
		public static Size MeasureDockPanelLayout(FrameworkElement parent, Size constraint, SkipLayout skipLayout) {
			double maxWidth = 0.0;
			double MaxHeight = 0.0;
			double totalWidth = 0.0;
			double totalHeight = 0.0;
			int count = VisualTreeHelper.GetChildrenCount(parent);
			for(int i = 0; i < count; i++) {
				UIElement element = VisualTreeHelper.GetChild(parent, i) as UIElement;
				if(element == null || (skipLayout != null && skipLayout(element))) continue;
				Size availableSize = new Size(Math.Max(0, constraint.Width - totalWidth), Math.Max(0, constraint.Height - totalHeight));
				element.Measure(availableSize);
				Size desiredSize = element.DesiredSize;
				switch(DockPanel.GetDock(element)) {
					case Dock.Left:
					case Dock.Right:
						MaxHeight = Math.Max(MaxHeight, totalHeight + desiredSize.Height);
						totalWidth += desiredSize.Width;
						break;
					case Dock.Top:
					case Dock.Bottom:
						maxWidth = Math.Max(maxWidth, totalWidth + desiredSize.Width);
						totalHeight += desiredSize.Height;
						break;
				}
			}
			maxWidth = Math.Max(maxWidth, totalWidth);
			return new Size(maxWidth, Math.Max(MaxHeight, totalHeight));
		}
		public static Size ArrangeDockPanelLayout(FrameworkElement parent, Size arrangeSize, bool lastChildFill) {
			return ArrangeDockPanelLayout(parent, arrangeSize, lastChildFill, null, null);
		}
		public static Size ArrangeDockPanelLayout(FrameworkElement parent, Size arrangeSize, bool lastChildFill, SkipLayout skipLayout, IsLastChild isLastChild) {
			double x = 0;
			double y = 0;
			double totalWidth = 0;
			double totalHeight = 0;
			int count = VisualTreeHelper.GetChildrenCount(parent);
			int lastDockableChildIndex = count - (lastChildFill ? 1 : 0);
			for(int i = 0; i < count; i++) {
				UIElement element = VisualTreeHelper.GetChild(parent, i) as UIElement;
				if(element == null) continue;
				if(skipLayout != null && skipLayout(element)) {
					element.Arrange(new Rect(0, 0, 50, 50));
					continue;
				}
				Size desiredSize = element.DesiredSize;
				Rect finalRect = new Rect(x, y, Math.Max(0, arrangeSize.Width - (x + totalWidth)), Math.Max(0, arrangeSize.Height - (y + totalHeight)));
				bool lastChild = isLastChild != null ? isLastChild(element) : i == lastDockableChildIndex;
				if(!lastChild) {
					switch(DockPanel.GetDock(element)) {
						case Dock.Left:
							x += desiredSize.Width;
							finalRect.Width = desiredSize.Width;
							break;
						case Dock.Top:
							y += desiredSize.Height;
							finalRect.Height = desiredSize.Height;
							break;
						case Dock.Right:
							totalWidth += desiredSize.Width;
							finalRect.X = Math.Max((double)0.0, (double)(arrangeSize.Width - totalWidth));
							finalRect.Width = desiredSize.Width;
							break;
						case Dock.Bottom:
							totalHeight += desiredSize.Height;
							finalRect.Y = Math.Max((double)0.0, (double)(arrangeSize.Height - totalHeight));
							finalRect.Height = desiredSize.Height;
							break;
					}
				}
				element.Arrange(finalRect);
			}
			return arrangeSize;
		}
	}
}
