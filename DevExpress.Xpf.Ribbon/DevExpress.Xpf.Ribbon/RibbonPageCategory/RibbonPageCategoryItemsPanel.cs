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

using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Core.Native;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
namespace DevExpress.Xpf.Ribbon {
	public class RibbonPageCategoryItemsPanel : RibbonOrderedOnMergeItemsPanel {
		protected RibbonPageCategoryControl Owner { get { return (RibbonPageCategoryControl)ItemsControl.GetItemsOwner(this); } }
		protected override bool IsOrdered {
			get { return true; }
		}
		protected override int GetChildOrder(UIElement child) {
			RibbonPageHeaderControl hdr = child as RibbonPageHeaderControl;
			if (hdr == null || hdr.Ribbon == null || hdr.Page == null)
				return 0;
			if (hdr.Ribbon.IsMerged) {
				return hdr.Page.ActualMergeOrder;
			} else {
				return hdr.Page.Index;
			}
		}
		protected override Size MeasureOverride(Size availableSize) {
			double minTotalWidth = 0d;
			foreach (RibbonPageHeaderControl child in InternalChildren) {
				child.Measure(availableSize);
				child.Measure(new Size(child.BestDesiredSize.Width, availableSize.Height));
				if(child.Visibility == Visibility.Visible)
					minTotalWidth += child.MinDesiredWidth;
			}
			if (Math.Round(availableSize.Width, 2) <= Math.Round(minTotalWidth, 2)) {
				foreach (RibbonPageHeaderControl child in InternalChildren)
					child.Measure(new Size(child.MinDesiredWidth, double.PositiveInfinity));
			} else
				Reduce(Children.OfType<RibbonPageHeaderControl>(), availableSize.Width);
			Size result = new Size();
			foreach (RibbonPageHeaderControl child in InternalChildren) {
				result.Height = Math.Max(result.Height, child.DesiredSize.Height);
				result.Width += child.DesiredSize.Width;
			}
			return result;
		}			
		protected override Size ArrangeOrderedChildren(RibbonOrderedItemInfo[] items, Size finalSize) {
			Rect arrangeRect = new Rect(new Size(0, finalSize.Height));
			double desiredWidth = items.Sum(item => item.Item.DesiredSize.Width);
			double coef = finalSize.Width / desiredWidth;
			if (double.IsInfinity(coef) || double.IsNaN(coef))
				coef = 1d;
			foreach (RibbonOrderedItemInfo info in items) {
				UIElement child = info.Item as UIElement;
				arrangeRect.Width = child.DesiredSize.Width * coef;
				child.Arrange(arrangeRect);
				arrangeRect.X += arrangeRect.Width;
			}
			return finalSize;
		}
		void Reduce(IEnumerable<RibbonPageHeaderControl> children, double availableWidth) {
			double totalWidth = Children.OfType<RibbonPageHeaderControl>().Sum(child => child.DesiredSize.Width);
			if (Math.Round(availableWidth, 2) >= Math.Round(totalWidth, 2))
				return;
			double coef = CalcReduceCoef(children, availableWidth, totalWidth);
			var toReduce = children.ToList();
			double newWidth = 0d;
			children = children.OrderBy(child => child.DesiredSize.Width);
			foreach (RibbonPageHeaderControl child in children) {
				newWidth = Math.Floor(child.DesiredSize.Width * coef);
				if (newWidth <= child.MinDesiredWidth) {
					newWidth = child.MinDesiredWidth;
					toReduce.Remove(child);
				}
				totalWidth -= child.DesiredSize.Width;
				child.Measure(new Size(newWidth, double.PositiveInfinity));
				totalWidth += child.DesiredSize.Width;
				if (Math.Round(totalWidth, 2) <= Math.Round(availableWidth, 2))
					return;
			}
			Reduce(toReduce, availableWidth);
		}
		double CalcReduceCoef(IEnumerable<RibbonPageHeaderControl> children, double available, double totalWidth) {
			var minSizeChildren = InternalChildren.OfType<RibbonPageHeaderControl>().Except(children);
			double minSizeTotalWdith = minSizeChildren.Sum(child => child.DesiredSize.Width);
			return (available - minSizeTotalWdith) / (totalWidth - minSizeTotalWdith);
		}
	}
	public class OfficeSlimRibbonPageCategoryItemsPanel : RibbonPageCategoryItemsPanel {
		protected RibbonPageHeaderControl ActivePage { get { return Owner.ActivePage; } }
		protected override Size MeasureOverride(Size availableSize) {
			var children = InternalChildren.OfType<RibbonPageHeaderControl>();
			foreach (RibbonPageHeaderControl child in children) {
				child.ClearValue(RibbonControlLayoutHelper.IsItemCollapsedProperty);
				child.Measure(availableSize);
				child.Measure(child.BestDesiredSize);
			}
			bool collapsePage = false;
			Size desiredSize = ActivePage.Return(page => page.DesiredSize, () => new Size());
			double available = Math.Round(availableSize.Width, 2);
			foreach (var child in children.Where(page => page != ActivePage)) {
				collapsePage = Math.Round(child.DesiredSize.Width + desiredSize.Width, 2) > available;
				RibbonControlLayoutHelper.SetIsItemCollapsed(child, collapsePage);
				desiredSize.Width += child.DesiredSize.Width;
			}
			return desiredSize;
		}
	}
}
