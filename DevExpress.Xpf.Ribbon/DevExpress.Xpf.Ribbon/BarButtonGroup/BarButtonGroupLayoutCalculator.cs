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
using System.Windows;
using DevExpress.Xpf.Bars;
using DevExpress.Xpf.Core.Native;
namespace DevExpress.Xpf.Ribbon {
	public class BarButtonGroupLayoutCalculator : RibbonLayoutCalculatorBase {
		public BarButtonGroupLayoutCalculator(RibbonItemsPanelBase panel) : base(panel) { }
		BarItemLinkControlBase GetFirstVisibleLinkControl() {
			foreach(UIElement child in Panel.Children) {
				if(GetLinkControl(child).Visibility == Visibility.Visible)
					return GetLinkControl(child);
			}
			return null;
		}
		BarItemLinkControlBase GetLastVisibleLinkControl() {
			for(int i = Panel.Children.Count - 1; i >= 0; i--) {
				if(GetLinkControl(i).Visibility == Visibility.Visible)
					return GetLinkControl(i);
			}
			return null;
		}
		void SetCenterItemsPosition(BarItemLinkControlBase leftLink, BarItemLinkControlBase rightLink) {
			foreach(UIElement child in Panel.Children) {
				if(GetLinkControl(child) == leftLink || GetLinkControl(child) == rightLink)
					continue;
				ItemPositionTypeProvider.SetHorizontalItemPosition(GetLinkControl(child), HorizontalItemPositionType.Center);
			}
		}
		protected virtual void UpdateItemsPosition() {
			if(Panel.Children.Count == 1) {
				ItemPositionTypeProvider.SetHorizontalItemPosition(GetLinkControl(0), HorizontalItemPositionType.Single);
				return;
			}
			BarItemLinkControlBase leftLink = GetFirstVisibleLinkControl();
			BarItemLinkControlBase rightLink = GetLastVisibleLinkControl();
			if(leftLink != null) ItemPositionTypeProvider.SetHorizontalItemPosition(leftLink, HorizontalItemPositionType.Left);
			if(rightLink != null) ItemPositionTypeProvider.SetHorizontalItemPosition(rightLink, HorizontalItemPositionType.Right);
			if(leftLink != null && rightLink != null) SetCenterItemsPosition(leftLink, rightLink);
		}
		public override Size MeasurePanel(Size availableSize) {
			UpdateItemsPosition();
			Size sz = new Size(0, 0);
			foreach(UIElement child in Panel.Children) { 
				child.Measure(new Size(double.PositiveInfinity, availableSize.Height));
				sz.Height = Math.Max(sz.Height, child.DesiredSize.Height);
				sz.Width += child.DesiredSize.Width;
			}
			return sz;
		}
		protected virtual double GetItemMaxHeight() { 
			double res = 0.0;
			foreach(UIElement child in Panel.Children) {
				res = Math.Max(res, child.DesiredSize.Height);
			}
			return res;
		}
		public override Size ArrangePanel(Size finalSize) {
			Rect r = new Rect(0,0,0,0);
			double height = GetItemMaxHeight();
			foreach(UIElement child in Panel.Children) {
				r.Width = child.DesiredSize.Width;
				r.Height = height;
				child.Arrange(r);
				r.X += child.DesiredSize.Width;
			}
			return new Size(r.X, height);
		}
	}
}
