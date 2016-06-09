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
using DevExpress.Xpf.Core.Native;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
namespace DevExpress.Xpf.Ribbon {
	public class RibbonLayoutCalculatorBase {
		public RibbonLayoutCalculatorBase(RibbonItemsPanelBase panel) {
			Panel = panel;
		}
		public RibbonItemsPanelBase Panel { get; private set; }
		public virtual Size MeasurePanel(Size availableSize) {
			return new Size(0, 0);
		}
		public virtual Size ArrangePanel(Size finalSize) {
			return new Size(0, 0);
		}
		public void InvalidateMeasureItems(DependencyObject from, DependencyObject to) {
			DependencyObject node = from;
			UIElement elem;
			do {
				elem = node as UIElement;
				if(elem != null)
					elem.InvalidateMeasure();
				node = VisualTreeHelper.GetParent(node);
			}
			while(node != to);
			elem = to as UIElement;
			if(elem != null)
				elem.InvalidateMeasure();
		}
		protected BarItemLinkControlBase GetLinkControl(UIElement child) {
			return (child as BarItemLinkInfo).LinkControl;
		}
		protected BarItemLinkControlBase GetLinkControl(int index) {
			return GetLinkControl(Panel.Children[index]);
		}
	}
	public class RibbonItemsPanelBase : Panel {
		protected virtual RibbonLayoutCalculatorBase CreateLayoutCalculator() {
			return new RibbonLayoutCalculatorBase(this);
		}
		RibbonLayoutCalculatorBase layoutCalculator;
		protected internal RibbonLayoutCalculatorBase LayoutCalculator {
			get {
				if(layoutCalculator == null)
					layoutCalculator = CreateLayoutCalculator();
				return layoutCalculator;
			}
		}
		public RibbonControl Ribbon {
			get {
				RibbonControl res = LayoutHelper.FindParentObject<RibbonControl>(this);
				if(res == null) {
					RibbonSelectedPageControl spc = LayoutHelper.FindParentObject<RibbonSelectedPageControl>(this);
					if(spc != null)
						res = spc.Ribbon;
				}
				return res;
			}
		}
		public RibbonQuickAccessToolbar Toolbar {
			get {
				if(Ribbon != null)
					return Ribbon.Toolbar;
				RibbonQuickAccessToolbarControl control = LayoutHelper.FindParentObject<RibbonQuickAccessToolbarControl>(this);
				return control != null ? control.Toolbar : null;
			}
		}
		public RibbonQuickAccessToolbarControl ToolbarControl {
			get {
				LinksControl lc = LayoutHelper.FindParentObject<RibbonQuickAccessToolbarControl>(this);
				return lc as RibbonQuickAccessToolbarControl;
			}
		}
		public RibbonPageCategoryControl PageCategoryControl { get { return LayoutHelper.FindParentObject<RibbonPageCategoryControl>(this); } }
		public RibbonPageGroupControl PageGroupControl { get { return LayoutHelper.FindParentObject<RibbonPageGroupControl>(this); } }
		protected override Size MeasureOverride(Size availableSize) {
			int a = InternalChildren.Count;
			return LayoutCalculator.MeasurePanel(availableSize);
		}
		protected override Size ArrangeOverride(Size finalSize) {
			return LayoutCalculator.ArrangePanel(finalSize);
		}
		protected internal virtual BarItemLinkControlBase GetLinkControl(UIElement child) {
			return ((BarItemLinkInfo)child).Content as BarItemLinkControlBase;
		}
		protected internal virtual BarItemLinkControlBase GetLinkControl(int index) {
			return GetLinkControl(Children[index]);
		}
	}
}
