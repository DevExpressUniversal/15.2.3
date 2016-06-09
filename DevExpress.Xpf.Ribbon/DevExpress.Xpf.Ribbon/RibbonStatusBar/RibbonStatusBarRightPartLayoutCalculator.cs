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

using DevExpress.Xpf.Bars;
using System.Windows;
namespace DevExpress.Xpf.Ribbon {
	public class RibbonStatusBarRightPartLayoutCalculator : RibbonLayoutCalculatorBase {
		public RibbonStatusBarRightPartLayoutCalculator(RibbonItemsPanelBase panel) : base(panel) { }
		public override System.Windows.Size MeasurePanel(Size availableSize) {
			return CalcBestFitSize(availableSize);
		}
		public override Size ArrangePanel(Size finalSize) {
			double x = finalSize.Width;
			for(int i = Panel.Children.Count - 1; i >= 0; i--) {
				UIElement child = Panel.Children[i];
				child.Arrange(new Rect(x - child.DesiredSize.Width, 0, child.DesiredSize.Width, finalSize.Height));				
				if(x - child.DesiredSize.Width < 0) {
					child.Opacity = 0;
					child.IsHitTestVisible = false;
				} else {
					child.Opacity = 1;
					child.IsHitTestVisible = true;
				}
				x -= child.DesiredSize.Width;
			}
			return finalSize;
		}
		protected virtual Size CalcBestFitSize(Size availableSize) {
			double width = 0;
			double height = 0;
			for(int i = Panel.Children.Count - 1; i >= 0; i--) {
				UIElement child = Panel.Children[i];
				child.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
				if(child.DesiredSize.Height > height) height = child.DesiredSize.Height;
				if(!double.IsInfinity(availableSize.Width) && !double.IsNaN(availableSize.Width) && (width + child.DesiredSize.Width > availableSize.Width)) break;
				width += child.DesiredSize.Width;
			}
			if(double.IsInfinity(availableSize.Height) || double.IsNaN(availableSize.Height)) return new Size(width, height);
			if(availableSize.Height < height) height = availableSize.Height;
			return new Size(width, height);
		}
	}
}
