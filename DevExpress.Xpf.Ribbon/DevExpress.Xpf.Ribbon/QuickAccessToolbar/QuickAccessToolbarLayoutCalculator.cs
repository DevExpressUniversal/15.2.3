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
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using DevExpress.Xpf.Bars;
namespace DevExpress.Xpf.Ribbon {
	public class RibbonQuickAccessToolbarLayoutCalculator : RibbonLayoutCalculatorBase {
		public RibbonQuickAccessToolbarLayoutCalculator(RibbonItemsPanelBase panel) : base(panel) { }
		int FirstVisibleItemIndex {
			get { return Panel.ToolbarControl.FirstVisibleItemIndex; }
		}
		int VisibleItemsCount {
			get { return Panel.ToolbarControl.VisibleItemsCount; }
			set { Panel.ToolbarControl.VisibleItemsCount = value; }
		}
		UIElementCollection Children { get { return Panel.Children; } }
		public override Size MeasurePanel(Size availableSize) {
			Size bestSize = new Size(0, 0);
			Size realSize = new Size(0, 0);
			int index = 0;
			VisibleItemsCount = 0;
			bool allowRealSizeCalculation = true;
			foreach(UIElement child in Children) {
				child.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
				bestSize.Width += child.DesiredSize.Width;
				bestSize.Height = Math.Max(child.DesiredSize.Height, bestSize.Height);
				if(index < FirstVisibleItemIndex) {
					child.Opacity = 0;
					child.IsHitTestVisible = false;
				}
				else {
					if(allowRealSizeCalculation && (!double.IsPositiveInfinity(availableSize.Width) && realSize.Width + child.DesiredSize.Width < availableSize.Width || double.IsPositiveInfinity(availableSize.Width))) {
						realSize.Width += child.DesiredSize.Width;
						realSize.Height = Math.Max(child.DesiredSize.Height, realSize.Height);
						VisibleItemsCount++;
						child.Opacity = 1;
						child.IsHitTestVisible = true;
					}
					else {
						allowRealSizeCalculation = false;
						child.Opacity = 0;
						child.IsHitTestVisible = false;
					}
				}
				index++;
			}
			Panel.ToolbarControl.BestSize = bestSize;
			Panel.ToolbarControl.UpdateButtonsVisibility();
			return realSize;
		}
		public override Size ArrangePanel(Size finalSize) {
			Rect finalRect = new Rect(0, 0, 0, finalSize.Height);
			for(int i = FirstVisibleItemIndex; i < Children.Count; i++) {
				UIElement child = Children[i];
				if(finalRect.X + child.DesiredSize.Width - finalSize.Width > 0.1) {
					break;
				}
				finalRect.Width = child.DesiredSize.Width;
				child.Arrange(finalRect);
				finalRect.X += finalRect.Width;
			}
			return finalSize;
		}
	}
}
