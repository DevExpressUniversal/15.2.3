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

extern alias Platform;
using System.Windows.Controls;
using Platform::DevExpress.Xpf.Core.Native;
using Microsoft.Windows.Design.Model;
using Microsoft.Windows.Design.Interaction;
using System.Windows;
using System.Linq;
using System.Windows.Media;
using Platform::DevExpress.Xpf.Bars;
using System;
using Microsoft.Windows.Design;
using DevExpress.Xpf.Core.Design.SmartTags;
using DevExpress.Mvvm.Native;
#if SL
using FrameworkElement = Platform::System.Windows.FrameworkElement;
using SizeChangedEventArgs = Platform::System.Windows.SizeChangedEventArgs;
using DependencyObject = Platform::System.Windows.DependencyObject;
using VisualTreeHelper = Platform::System.Windows.Media.VisualTreeHelper;
#endif
namespace DevExpress.Xpf.Core.Design {
	public abstract class SelectionBorder : Border {
		public ModelItem PrimarySelection {
			get { return _modelItem; }
			set {
				if(_modelItem == value) return;
				ModelItem oldModelItem = _modelItem;
				_modelItem = value;
				OnPrimarySelectionChanged(oldModelItem);
			}
		}
		public FrameworkElement SelectedElement {
			get { return _selectedElement; }
			set {
				if(_selectedElement == value) return;
				FrameworkElement oldElement = _selectedElement;
				_selectedElement = value;
				OnSelectionElementChanged(oldElement);
			}
		}
		protected DesignerView DesignerView {
			get { return _designerView; }
			set {
				if(_designerView == value) return;
				UnsubscribeDesignerViewEvents();
				_designerView = value;
				SubscribeDesignerViewEvents();
			}
		}
		protected ViewItem ViewItem {
			get { return _viewItem; }
			set {
				if(_viewItem == value) return;
				_viewItem = value;
				OnViewItemChanged();
			}
		}
		protected ModelItem Root {
			get { return _root; }
			set {
				if(_root == value) return;
				_root = value;
				OnRootChanged();
			}
		}
		public SelectionBorder() {
			Initialize();
		}
		public SelectionBorder(ModelItem root) {
			Root = root;
			Initialize();
		}
		protected virtual FrameworkElement GetSelectedElement() {
			FrameworkElement elem = null;
			if(PrimarySelection != null && PrimarySelection.View != null)
				elem = PrimarySelection.View.PlatformObject as FrameworkElement;
			return elem;
		}
		protected virtual void Initialize() {
			Visibility = Visibility.Collapsed;
			Background = new SolidColorBrush(Color.FromRgb(255, 0, 0)) { Opacity = 0.25 };
			BorderThickness = new Thickness(1);
			BorderBrush = new SolidColorBrush(Color.FromRgb(192, 0, 0)) { Opacity = 0.35 };
			IsHitTestVisible = false;
		}
		protected virtual void OnPrimarySelectionChanged(ModelItem oldSelection) {
			Root = PrimarySelection == null ? null : PrimarySelection.Root;
			UpdateSelectedElement();
		}
		protected void UpdateSelectedElement() {
			SelectedElement = PrimarySelection != null ? GetSelectedElement() : null;
		}
		protected virtual void OnSelectionElementChanged(FrameworkElement oldSelectionElement) {
			Visibility = Visibility.Collapsed;
			if(oldSelectionElement != null) UnsubscribeEvents(oldSelectionElement);
			if(SelectedElement != null) {
				SubscribeEvents(SelectedElement);
				UpdateView();
				UpdateBounds();
			}
		}
		protected virtual void OnSelectionElementSizeChanged(object sender, SizeChangedEventArgs e) {
			UpdateBounds();
		}
		protected virtual void OnSelectedElementLayoutUpdated(object sender, System.EventArgs e) {
			UpdateBounds();
		}
		protected virtual void OnDesignerViewZoomLevelChanged(object sender, System.EventArgs e) {
			UpdateBounds();
		}
		protected virtual void OnRootChanged() {
			DesignerView = _root == null ? null : DesignerView.FromContext(_root.Context);
		}
		protected virtual void OnViewItemChanged() {
			if(SelectedElement != null)
				UpdateBounds();
		}
		protected void UpdateView() {
			ViewItem = Root == null ? null : ViewItemHelper.GetViewItem(Root.View, SelectedElement);
		}
		protected void UpdateBounds() {
			if(SelectedElement == null) return;
			UpdateVisibility();
			UpdatePostion();
			UpdateSize();
		}
		void SubscribeEvents(FrameworkElement control) {
			control.LayoutUpdated += OnSelectedElementLayoutUpdated;
			control.SizeChanged += OnSelectionElementSizeChanged;
		}
		void UnsubscribeEvents(FrameworkElement control) {
			control.LayoutUpdated -= OnSelectedElementLayoutUpdated;
			control.SizeChanged -= OnSelectionElementSizeChanged;
		}
		void SubscribeDesignerViewEvents() {
			if(DesignerView == null) return;
			DesignerView.ZoomLevelChanged += OnDesignerViewZoomLevelChanged;
			DesignerView.LayoutUpdated += OnDesignerViewLayoutUpdated;
		}
		void OnDesignerViewLayoutUpdated(object sender, EventArgs e) {
			if(ViewItem == null) UpdateView();
			if(SelectedElement == null || !SelectedElement.IsInVisualTree()) {
				UpdateSelectedElement();
			}
		}
		void UnsubscribeDesignerViewEvents() {
			if(DesignerView == null) return;
			DesignerView.ZoomLevelChanged -= OnDesignerViewZoomLevelChanged;
			DesignerView.LayoutUpdated -= OnDesignerViewLayoutUpdated;
		}
		protected virtual void UpdatePostion() {
			if(ViewItem == null)
				return;
			Point position = GetPosition();
			Canvas.SetLeft(this, position.X);
			Canvas.SetTop(this, position.Y);
		}
		protected virtual Point GetPosition() {
			return TranslatePoint(new Point());
		}
		protected virtual void UpdateVisibility() {
			Visibility = ViewItem != null && ViewItem.IsVisible && !IsHidden() ? Visibility.Visible : Visibility.Collapsed;
		}
		protected virtual bool IsHidden() {
			if(SelectedElement == null) return true;
#if !SL
			if(!SelectedElement.IsVisible || !SelectedElement.IsInVisualTree()) return true;
#endif
			DependencyObject parent = SelectedElement;
			while(parent != null) {
				var uiParent = parent as FrameworkElement;
				if(uiParent != null) {
					if (uiParent.Opacity == 0d) return true;
				}
				parent = VisualTreeHelper.GetParent(parent);
			}
			return false;
		}
		protected void UpdateSize() {
			Size newSize = GetSize();
			Width = newSize.Width;
			Height = newSize.Height;
		}
		protected virtual Size GetSize() {
			if (DesignerView == null || ViewItem == null)
				return new Size();
			Size newSize = ViewItem.RenderSize;
			newSize = CorrectSize(newSize);
			newSize.Width *= DesignerView.ZoomLevel;
			newSize.Height *= DesignerView.ZoomLevel;
			return newSize;
		}
		protected Size CorrectSize(Size newSize) {
#if SILVERLIGHT
			double x, y;
			ResolutionHelper.GetTransformCoefficient(DesignerView, out x, out y);
			newSize.Width /= x; newSize.Height /= y;
#endif
			return newSize;
		}
		protected Point TranslatePoint(Point p) {
			ModelItem parent = GetParent();
			if(parent == null || parent.View == null) {
				return new Point();
			}
			Point point = ViewItem.TransformToView(parent.View).Transform(p);
			Size size = CorrectSize(new Size(Math.Max(0, point.X), Math.Max(0, point.Y)));
			point.X = size.Width;
			point.Y = size.Height;
			point.X *= DesignerView.ZoomLevel;
			point.Y *= DesignerView.ZoomLevel;
			return point;
		}
		protected virtual ModelItem GetParent() {
			ModelItem parent = AttributeHelper.GetAttributes<DesignTimeParentAttribute>(PrimarySelection.ItemType).
				Select(attribute => BarManagerDesignTimeHelper.FindParentByType(attribute.ParentType, PrimarySelection)).
				FirstOrDefault(item => item != null);
			return parent;
		}
		DesignerView _designerView;
		ModelItem _modelItem;
		FrameworkElement _selectedElement;
		ViewItem _viewItem;
		ModelItem _root;
	}
}
