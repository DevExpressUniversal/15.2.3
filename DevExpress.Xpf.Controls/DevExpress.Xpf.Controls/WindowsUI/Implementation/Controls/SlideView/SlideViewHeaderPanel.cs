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
namespace DevExpress.Xpf.WindowsUI.Internal {
	public class SlideViewHeaderPanel : Panel {
#if SILVERLIGHT
		#region Clipping
		RectangleGeometry clipGeometry;
		public SlideViewHeaderPanel() {
			SizeChanged += SlideViewHeaderPanel_SizeChanged;
			clipGeometry = new RectangleGeometry();
			Clip = clipGeometry;
		}
		void SlideViewHeaderPanel_SizeChanged(object sender, SizeChangedEventArgs e) {
			clipGeometry.Rect = new Rect(new Point(), e.NewSize);
		}
		#endregion
#endif
		internal void SetRelativeLocation(double scrollOffset, double headerOffset) {
			if(Children.Count == 0) return;
			UIElement child = Children[0];
			if(scrollOffset >= headerOffset) {
				ArrangeX = 0;	
				InvalidateArrange();
				child.Opacity = 1.0;
				return;
			}
			double position = headerOffset - scrollOffset + Margin.Left;
			double maxPosition = ActualWidth - child.DesiredSize.Width;
			if(position > maxPosition)
				child.Opacity = Math.Max(0, 1.0 - (position - maxPosition) / child.DesiredSize.Width);
			else child.Opacity = 1.0;
			ArrangeX = position;
			InvalidateArrange();
		}
		double ArrangeX;
		protected override Size MeasureOverride(Size availableSize) {
			if(Children.Count == 0) return new Size();
			UIElement child = Children[0];
			child.Measure(availableSize);
			return child.DesiredSize;
		}
		protected override Size ArrangeOverride(Size finalSize) {
			foreach(FrameworkElement child in Children) {
				child.Arrange(new Rect(ArrangeX, 0, Math.Max(0, finalSize.Width - ArrangeX), finalSize.Height));
			}
#if SILVERLIGHT
			ArrangeX = 0;
#endif
			return finalSize;
		}
	}
}
