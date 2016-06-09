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
using System.Windows.Media.Animation;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Controls;
using System.Collections;
using DevExpress.Xpf.Core.Native;
using DevExpress.Utils;
#if SL
using DevExpress.Xpf.Core.WPFCompatibility;
#endif
namespace DevExpress.Xpf.Core {	
	public abstract class OrderPanelBase : Panel {
		class VisibleIndexComparer<T> : IComparer<T> {
			readonly Func<T, int> getVisibleIndexFunc;
			public VisibleIndexComparer(Func<T, int> getVisibleIndexFunc) {
				this.getVisibleIndexFunc = getVisibleIndexFunc;
			}
			#region IComparer<T> Members
			public int Compare(T x, T y) {
				int visibleIndex1 = getVisibleIndexFunc(x);
				int visibleIndex2 = getVisibleIndexFunc(y);
				if(IsInvisibleIndex(visibleIndex1) || IsInvisibleIndex(visibleIndex2))
					throw new NotImplementedException();
				return visibleIndex1 - visibleIndex2;
			}
			#endregion
		}
		#region static
		public const int InvisibleIndex = -1;
		public const Orientation DefaultOrientation = Orientation.Horizontal;
		public static readonly DependencyProperty OrientationProperty;
		public static readonly DependencyProperty VisibleIndexProperty;
		public static readonly DependencyProperty ArrangeAccordingToVisibleIndexProperty;
		public static readonly DependencyProperty PanelProperty;
		static OrderPanelBase() {
#if !SL
			OrientationProperty = DependencyProperty.Register("Orientation", typeof(Orientation), typeof(OrderPanelBase), new FrameworkPropertyMetadata(DefaultOrientation, FrameworkPropertyMetadataOptions.AffectsMeasure, null, null));
#else
			OrientationProperty = DependencyProperty.Register("Orientation", typeof(Orientation), typeof(OrderPanelBase), new PropertyMetadata(DefaultOrientation));
#endif
			VisibleIndexProperty = DependencyProperty.RegisterAttached("VisibleIndex", typeof(int), typeof(OrderPanelBase), 
#if !SL
				new FrameworkPropertyMetadata(InvisibleIndex, FrameworkPropertyMetadataOptions.AffectsParentMeasure)
#else
				new PropertyMetadata(InvisibleIndex)
#endif
);
			ArrangeAccordingToVisibleIndexProperty = DependencyProperty.Register("ArrangeAccordingToVisibleIndex", typeof(bool), typeof(OrderPanelBase), new PropertyMetadata(false));
			PanelProperty = DependencyProperty.RegisterAttached("Panel", typeof(Panel), typeof(OrderPanelBase), new PropertyMetadata(null));
		}
		public static void SetVisibleIndex(DependencyObject element, int index) {
			if(element == null)
				throw new ArgumentNullException("element");
			IOrderPanelElement orderPanelElement = element as IOrderPanelElement;
			if(orderPanelElement != null) {
				SetOrderPanelElementVisibleIndex(orderPanelElement, index);
			} else {
				element.SetValue(VisibleIndexProperty, index);
			}
		}
		static void SetOrderPanelElementVisibleIndex(IOrderPanelElement element, int index) {
			int oldVisibleIndex = element.VisibleIndex;
			element.VisibleIndex = index;
			if(oldVisibleIndex == index)
				return;
			FrameworkElement parent = LayoutHelper.GetParent((DependencyObject)element) as FrameworkElement;
			if(parent != null)
				parent.InvalidateMeasure();
		}
		public static int GetVisibleIndex(DependencyObject element) {
			if(element == null)
				throw new ArgumentNullException("element");
			IOrderPanelElement orderPanelElement = element as IOrderPanelElement;
			if(orderPanelElement != null)
				return orderPanelElement.VisibleIndex;
			return (int)element.GetValue(VisibleIndexProperty);
		}
		public static void SetPanel(DependencyObject element, Panel panel) {
			if(element == null)
				throw new ArgumentNullException("element");
			element.SetValue(PanelProperty, panel);
		}
		public static Panel GetPanel(DependencyObject element) {
			if(element == null)
				throw new ArgumentNullException("element");
			return (Panel)element.GetValue(PanelProperty);
		}
		public bool ArrangeAccordingToVisibleIndex {
			get { return (bool)GetValue(ArrangeAccordingToVisibleIndexProperty); }
			set { SetValue(ArrangeAccordingToVisibleIndexProperty, value); }
		}
		public static IList<T> GetSortedElements<T>(ICollection elements, bool arrangeAccordingToVisibleIndex, Func<T, int> getVisibleIndexFunc) {
			List<T> sortedChildren = new List<T>();
			foreach(T item in elements) {
				if(!arrangeAccordingToVisibleIndex || !IsInvisibleIndex(getVisibleIndexFunc(item)))
					sortedChildren.Add(item);
			}
			if(arrangeAccordingToVisibleIndex) sortedChildren.Sort(new VisibleIndexComparer<T>(getVisibleIndexFunc));
			return sortedChildren;
		}
		public static IList<UIElement> GetSortedElements(ICollection uiElements, bool arrangeAccordingToVisibleIndex) {
			return GetSortedElements<UIElement>(uiElements, arrangeAccordingToVisibleIndex, item => GetVisibleIndex(item));
		}
		public IList<UIElement> GetSortedChildren(){
			return GetSortedElements(Children, ArrangeAccordingToVisibleIndex);
		}
		public static bool IsInvisibleIndex(int index) {
			return index == InvisibleIndex;
		}
		#endregion
		public Orientation Orientation {
			get { return (Orientation)GetValue(OrientationProperty); }
			set { SetValue(OrientationProperty, value); }
		}
		protected SizeHelperBase SizeHelper { get { return SizeHelperBase.GetDefineSizeHelper(Orientation); } }
		protected sealed override Size MeasureOverride(Size availableSize) {
			UpdateChildrenVisibility(Children, ArrangeAccordingToVisibleIndex);
			return MeasureSortedChildrenOverride(availableSize, GetSortedElements(Children, ArrangeAccordingToVisibleIndex));
		}
		static void UpdateChildrenVisibility(ICollection children, bool arrangeAccordingToVisibleIndex) {
			foreach(UIElement child in children)
				UpdateChildVisibility(child, arrangeAccordingToVisibleIndex, GetVisibleIndex(child));
		}
		public static void UpdateChildVisibility(UIElement child, bool arrangeAccordingToVisibleIndex, int visibleIndex) {
			Visibility previous = child.Visibility;
			Visibility current = visibleIndex >= 0 || !arrangeAccordingToVisibleIndex ? Visibility.Visible : Visibility.Collapsed;
			child.Visibility = current;
			if(previous == Visibility.Collapsed && current == Visibility.Visible)
				child.InvalidateMeasure();
		}
		protected sealed override Size ArrangeOverride(Size finalSize) {
			return ArrangeSortedChildrenOverride(finalSize, GetSortedElements(Children, ArrangeAccordingToVisibleIndex));
		}
		protected abstract Size MeasureSortedChildrenOverride(Size availableSize, IList<UIElement> sortedChildren);
		protected abstract Size ArrangeSortedChildrenOverride(Size finalSize, IList<UIElement> sortedChildren);
#if !SL
		protected override void OnIsItemsHostChanged(bool oldIsItemsHost, bool newIsItemsHost) {
			base.OnIsItemsHostChanged(oldIsItemsHost, newIsItemsHost);
			var itemsOwner = ItemsControl.GetItemsOwner(this);
			if(itemsOwner == null)
				return;
			SetPanel(itemsOwner, this);			
		}
#endif
	}
	public class DXListBox : ListBox {
		public EventHandler ItemLeftButtonDoubleClick;
		public DXListBox() { }
		protected override DependencyObject GetContainerForItemOverride() {
			DependencyObject d = base.GetContainerForItemOverride();
			if(d is Control) {
				MouseHelper.SubscribeLeftButtonDoubleClick((Control)d, delegate(object sender, EventArgs e) {
					if(ItemLeftButtonDoubleClick != null)
						ItemLeftButtonDoubleClick(this, EventArgs.Empty);
				});
			}
			return d;
		}
	}
}
