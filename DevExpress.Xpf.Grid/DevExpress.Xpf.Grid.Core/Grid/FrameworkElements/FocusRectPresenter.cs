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
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
using System.Windows.Media;
using System.Windows.Data;
using DevExpress.Xpf.Grid.Hierarchy;
namespace DevExpress.Xpf.Grid {
	public class FocusRectPresenter : ContentControl {
		public static readonly DependencyProperty ViewProperty = DependencyProperty.Register("View", typeof(DataViewBase), typeof(FocusRectPresenter), new PropertyMetadata(null, (d, e) => ((FocusRectPresenter)d).OnViewChanged()));
		public static readonly DependencyProperty SizeExpansionProperty = DependencyProperty.Register("SizeExpansion", typeof(int), typeof(FocusRectPresenter), new PropertyMetadata(0));
		public static readonly DependencyProperty ChildTemplateProperty = DependencyProperty.Register("ChildTemplate", typeof(ControlTemplate), typeof(FocusRectPresenter), new PropertyMetadata(null, (d, e) => ((FocusRectPresenter)d).OnChildTemplateChanged()));
		public static readonly DependencyProperty IsHorizontalScrollHostProperty = DependencyProperty.RegisterAttached("IsHorizontalScrollHost", typeof(bool), typeof(FocusRectPresenter), new PropertyMetadata(false));
		public static readonly DependencyProperty IsVerticalScrollHostProperty = DependencyProperty.RegisterAttached("IsVerticalScrollHost", typeof(bool), typeof(FocusRectPresenter), new PropertyMetadata(false));
		public static void SetIsHorizontalScrollHost(DependencyObject element, bool value) {
			element.SetValue(IsHorizontalScrollHostProperty, value);
		}
		public static bool GetIsHorizontalScrollHost(DependencyObject element) {
			return (bool)element.GetValue(IsHorizontalScrollHostProperty);
		}
		public static void SetIsVerticalScrollHost(DependencyObject element, bool value) {
			element.SetValue(IsVerticalScrollHostProperty, value);
		}
		public static bool GetIsVerticalScrollHost(DependencyObject element) {
			return (bool)element.GetValue(IsVerticalScrollHostProperty);
		}
		public DataViewBase View {
			get { return (DataViewBase)GetValue(ViewProperty); }
			set { SetValue(ViewProperty, value); }
		}
		public int SizeExpansion {
			get { return (int)GetValue(SizeExpansionProperty); }
			set { SetValue(SizeExpansionProperty, value); }
		}
		public ControlTemplate ChildTemplate {
			get { return (ControlTemplate)GetValue(ChildTemplateProperty); }
			set { SetValue(ChildTemplateProperty, value); }
		}
		void OnViewChanged() {
			if(View != null)
				View.FocusRectPresenter = this;
		}
		void OnChildTemplateChanged() {
#if !SL
			((Control)Content).Template = ChildTemplate;
#else
			((SLControl)Content).Template = ChildTemplate;
#endif
		}
		FrameworkElement owner;
		internal FrameworkElement Owner {
			get {
				return owner;
			}
			set {
				if(owner == value)
					return;
				owner = value;
				if(owner != null) {
					HorizontalClipParent = FindScrollHost(owner, IsHorizontalScrollHostProperty);
					VerticalClipParent = FindScrollHost(owner, IsVerticalScrollHostProperty);
				} else {
					HorizontalClipParent = null;
					VerticalClipParent = null;
				}
			}
		}
		internal static FrameworkElement FindScrollHost(DependencyObject element, DependencyProperty property) {
			while(element != null) {
				if((bool)element.GetValue(property) == true)
					return element as FrameworkElement;
				element = VisualTreeHelper.GetParent(element);
			}
			return null;
		}
		FrameworkElement HorizontalClipParent;
		FrameworkElement VerticalClipParent;
		Size size = Size.Empty;
		double horizontalChildOffset = 0d;
		double verticalChildOffset = 0d;
		Rect oldOwnerRect;
		double oldHorizontalClipParentWidth = 0d;
		double oldVerticalClipParentHeight = 0d;
		public FocusRectPresenter() {
#if !SL
			Content = new Control();
#else
			Content = new SLControl();
#endif
			Visibility = Visibility.Collapsed;
		}
		Rect GetСoerceRect() {
			var iChrome = Owner as IChrome;
			return iChrome != null && iChrome.Root != null ? iChrome.Root.RenderRect : new Rect(new Point(), Owner.RenderSize);
		}
		internal void UpdateRendering(double leftIndent) {
			Rect ownerRect = LayoutHelper.GetRelativeElementRect(Owner, this.GetTemplatedParent() as UIElement);
			Rect coerceRect = GetСoerceRect();
			ownerRect.X += coerceRect.Left;
			ownerRect.Y += coerceRect.Top;
			ownerRect.Width -= Owner.ActualWidth - coerceRect.Width;
			ownerRect.Height -= Owner.ActualHeight - coerceRect.Height;
			ownerRect.X += leftIndent;
			if(ownerRect.Width >= leftIndent)
				ownerRect.Width -= leftIndent;
			ownerRect.X -= SizeExpansion;
			ownerRect.Y -= SizeExpansion;
			ownerRect.Height += (SizeExpansion * 2);
			ownerRect.Width += (SizeExpansion * 2);
			double horizontalClipParentWidth = HorizontalClipParent != null ? HorizontalClipParent.ActualWidth : 0;
			double verticalClipParentHeight = VerticalClipParent != null ? VerticalClipParent.ActualHeight : 0;
			if((ownerRect == oldOwnerRect) &&
			   (oldHorizontalClipParentWidth == horizontalClipParentWidth) &&
			   (oldVerticalClipParentHeight == verticalClipParentHeight))
				return;
			oldHorizontalClipParentWidth = horizontalClipParentWidth;
			oldVerticalClipParentHeight = verticalClipParentHeight;
			oldOwnerRect = ownerRect;
			Clip = GetClip(ownerRect);
			Margin = new Thickness(ownerRect.Left, ownerRect.Top, 0, 0);
			InvalidateMeasure();
		}
		RectangleGeometry GetClip(Rect ownerRect) {
			Point location = new Point();
			Rect vClipParentRect = VerticalClipParent != null ? LayoutHelper.GetRelativeElementRect(VerticalClipParent, this.GetTemplatedParent() as UIElement) : ownerRect;
			double vOffset = vClipParentRect.Top + GetFixedElementsHeight();
			if(ownerRect.Top < vOffset && ownerRect.Bottom >= vOffset)
				location.Y = vOffset - ownerRect.Top;
			Rect hClipParentRect = HorizontalClipParent != null ? LayoutHelper.GetRelativeElementRect(HorizontalClipParent, this.GetTemplatedParent() as UIElement) : ownerRect;
			if(ownerRect.Left < hClipParentRect.Left)
				location.X = hClipParentRect.Left - ownerRect.Left;
			double width = ownerRect.Size.Width - location.X;
			double height = ownerRect.Size.Height - location.Y;
			if(vClipParentRect.Bottom < ownerRect.Bottom)
				height += vClipParentRect.Bottom - ownerRect.Bottom;
			if(hClipParentRect.Right < ownerRect.Right)
				width += vClipParentRect.Right - ownerRect.Right;
			size = new Size(Math.Max(0, width), Math.Max(0, height));
			return new RectangleGeometry() { Rect = new Rect(location, size) };
		}
		double GetFixedElementsHeight() {
			double height = 0;
			HierarchyPanel panel = View.DataPresenter.Content as HierarchyPanel;
			if(panel != null)
				foreach(FrameworkElement element in panel.FixedElements) {
					height += element.ActualHeight;
				}
			return height;
		}
		protected override Size MeasureOverride(Size availableSize) {
			base.MeasureOverride(new Size(oldOwnerRect.Width, oldOwnerRect.Height));
			return size;
		}
		protected override Size ArrangeOverride(Size finalSize) {
			(Content as UIElement).Arrange(new Rect(horizontalChildOffset < 0 ? horizontalChildOffset : 0, verticalChildOffset < 0 ? verticalChildOffset : 0, oldOwnerRect.Width, oldOwnerRect.Height));
			return finalSize;
		}
	}
}
