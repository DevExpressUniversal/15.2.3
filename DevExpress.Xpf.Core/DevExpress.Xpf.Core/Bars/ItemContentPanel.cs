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
using DevExpress.Xpf.Utils;
namespace DevExpress.Xpf.Bars {
	public enum ItemContentPanelAlignment { Near, Far}
	public class ItemContentPanel : Panel {
		#region static
		public static readonly DependencyProperty AlignmentProperty;
		static ItemContentPanel() {
			AlignmentProperty = DependencyPropertyManager.RegisterAttached("Alignment", typeof(ItemContentPanelAlignment), typeof(ItemContentPanel), new FrameworkPropertyMetadata(ItemContentPanelAlignment.Near));
		}
		public static void SetAlignment(DependencyObject d, ItemContentPanelAlignment value) { d.SetValue(AlignmentProperty, value); }
		public static ItemContentPanelAlignment GetAlignment(DependencyObject d) { return (ItemContentPanelAlignment)d.GetValue(AlignmentProperty); }
		#endregion
		protected override System.Windows.Size MeasureOverride(System.Windows.Size availableSize) {
			Size total = new Size();
			Size avail = availableSize;
			foreach(UIElement elem in Children) {
				elem.Measure(avail);
				total.Width += elem.DesiredSize.Width;
				total.Height = Math.Max(elem.DesiredSize.Height, total.Height);
				avail.Width -= elem.DesiredSize.Width;
			}
			return total;
		}
		protected override Size ArrangeOverride(Size finalSize) {
			Rect leftRect = new Rect(new Point(0,0), finalSize);
			foreach(UIElement elem in Children) {
				ItemContentPanelAlignment align = ItemContentPanel.GetAlignment(elem);
				if(align == ItemContentPanelAlignment.Near) {
					ArrangeItem(elem, leftRect);
					leftRect.X += elem.DesiredSize.Width;
					leftRect.Width = Math.Max(0, leftRect.Width - elem.DesiredSize.Width);
				}
			}
			Rect rightRect = new Rect(new Point(finalSize.Width, 0), new Size(Math.Max(0, finalSize.Width - leftRect.X), finalSize.Height));
			for(int i = Children.Count - 1; i >= 0; i--) {
				UIElement elem = Children[i];
				ItemContentPanelAlignment align = ItemContentPanel.GetAlignment(elem);
				if(align == ItemContentPanelAlignment.Far) {
					rightRect.X -= elem.DesiredSize.Width;
					ArrangeItem(elem, rightRect);
					rightRect.Width = Math.Max(0, rightRect.Width - elem.DesiredSize.Width);
				}
			}
				return finalSize;
		}
		protected virtual void ArrangeItem(UIElement item, Rect availRect) {
			VerticalAlignment align = (VerticalAlignment)item.GetValue(FrameworkElement.VerticalAlignmentProperty);
			if(align == VerticalAlignment.Top) { 
				item.Arrange(new Rect(new Point(availRect.Left, availRect.Top), item.DesiredSize));
			}
			else if(align == VerticalAlignment.Stretch) {
				item.Arrange(new Rect(availRect.Left, availRect.Top, item.DesiredSize.Width, availRect.Height));
			}
			else if(align == VerticalAlignment.Center) {
				item.Arrange(new Rect(availRect.Left, availRect.Top + (availRect.Height - item.DesiredSize.Height) / 2, item.DesiredSize.Width, item.DesiredSize.Height));
			}
			else {
				item.Arrange(new Rect(availRect.Left, availRect.Bottom - item.DesiredSize.Height, item.DesiredSize.Width, item.DesiredSize.Height));
			}
		}
	}
}
