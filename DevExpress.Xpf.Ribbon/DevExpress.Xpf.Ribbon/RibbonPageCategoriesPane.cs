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
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
namespace DevExpress.Xpf.Ribbon {
	public class RibbonPageCategoriesPane : ItemsControl {
		static RibbonPageCategoriesPane() {
			DefaultStyleKeyProperty.OverrideMetadata(typeof(RibbonPageCategoriesPane), new FrameworkPropertyMetadata(typeof(RibbonPageCategoriesPane)));
		}
		public RibbonPageCategoriesPane() {
			LayoutUpdated += OnLayoutUpdated;
			SizeChanged += OnSizeChanged;
		}
		public RibbonControl Ribbon {
			get {
				var ribbon = (TemplatedParent as RibbonControl);
				if(ribbon == null)
					ribbon = RibbonControl.GetRibbon(this);
				return ribbon;
			}
		}
		public ButtonBase LeftButton {
			get { return leftButton; }
			private set {
				if(leftButton == value)
					return;
				var oldValue = leftButton;
				leftButton = value;
				OnLeftButtonChanged(oldValue);
			}
		}
		public ButtonBase RightButton {
			get { return rightButton; }
			private set {
				if(rightButton == value)
					return;
				var oldValue = rightButton;
				rightButton = value;
				OnRightButtonChanged(oldValue);
			}
		}
		public ScrollViewer ScrollHost { get { return scrollHost; }
			private set { 
				if (scrollHost == value)
					return;
				var oldValue = scrollHost;
				scrollHost = value;
				OnSrollHostChanged(oldValue);
			}
		}
		public RibbonItemsPanel RibbonItemsPanel {
			get {
				if(ItemsPresenter == null || VisualTreeHelper.GetChildrenCount(ItemsPresenter) == 0)
					return null;
				return VisualTreeHelper.GetChild(ItemsPresenter, 0) as RibbonItemsPanel;
			}
		}
		ItemsPresenter ItemsPresenter { get; set; }
		RibbonPageCategoryHeaderControl HeaderControl { get; set; }
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			ItemsPresenter = GetTemplateChild("PART_ItemsPresenter") as ItemsPresenter;
			LeftButton = GetTemplateChild("PART_LeftRepeatButton") as ButtonBase;
			RightButton = GetTemplateChild("PART_RightRepeatButton") as ButtonBase;
			ScrollHost = GetTemplateChild("PART_ScrollViewer") as ScrollViewer;
			HeaderControl = GetTemplateChild("PART_OriginHeaderControl") as RibbonPageCategoryHeaderControl;
			if(HeaderControl != null)
				BindingOperations.ClearBinding(HeaderControl, RibbonPageCategoryHeaderControl.IsAeroModeProperty);
		}
		public double GetContextualCategoriesLeftOffset() {
			if (ItemContainerGenerator.Status != GeneratorStatus.ContainersGenerated)
				return 0d;
			var category = Items.Cast<RibbonPageCategoryBase>().Where(cat => !cat.IsDefault && cat.IsVisible).OrderBy(cat => GetChildOrder(cat)).FirstOrDefault();
			if (category == null)
				return 0d;
			var categoryControl = (FrameworkElement)ItemContainerGenerator.ContainerFromItem(category);
			return categoryControl.TranslatePoint(new Point(), this).X;
		}
		public double GetHeadersWidth() {
			if (ItemContainerGenerator.Status != GeneratorStatus.ContainersGenerated)
				return 0d;
			var categories = Items.Cast<RibbonPageCategoryBase>().Where(cat => !cat.IsDefault).Select(cat => (RibbonPageCategoryControl)ItemContainerGenerator.ContainerFromItem(cat));
			return categories.Sum(cat => cat.GetHeaderWidth());
		}
		protected override void ClearContainerForItemOverride(DependencyObject element, object item) {
			RibbonPageCategoryControl catControl = element as RibbonPageCategoryControl;
			catControl.PageCategory = null;
			catControl.Ribbon = null;
			base.ClearContainerForItemOverride(element, item);
		}
		protected override DependencyObject GetContainerForItemOverride() {
			return new RibbonPageCategoryControl();
		}
		protected override bool IsItemItsOwnContainerOverride(object item) {
			return item is RibbonPageCategoryControl;
		}
		protected override void PrepareContainerForItemOverride(DependencyObject element, object item) {
			base.PrepareContainerForItemOverride(element, item);
			RibbonPageCategoryControl catControl = element as RibbonPageCategoryControl;
			RibbonPageCategoryBase cat = null;
			if(item is RibbonPageCategoryBase)
				cat = (RibbonPageCategoryBase)item;
			else
				throw new ArgumentException("Only RibbonPageCategory or DefaultRibbonPageCategory can be added to RibbonControl.");
			catControl.PageCategory = cat;
			catControl.Ribbon = Ribbon;
		}
		protected virtual void OnLayoutUpdated(object sender, EventArgs e) {
			UpdateScrollButtonsVisibility();
		}
		protected virtual void OnLeftButtonChanged(ButtonBase oldValue) {
			if(oldValue != null)
				oldValue.Click -= OnLeftButtonClick;
			if(LeftButton != null)
				LeftButton.Click += OnLeftButtonClick;
		}
		protected virtual void OnRightButtonChanged(ButtonBase oldValue) {
			if(oldValue != null)
				oldValue.Click -= OnRightButtonClick;
			if(RightButton != null)
				RightButton.Click += OnRightButtonClick;
		}
		protected virtual void OnSizeChanged(object sender, SizeChangedEventArgs e) {
			if (RibbonItemsPanel != null) {
				RibbonItemsPanel.CheckOffset();
			}
		}
		protected virtual void OnSrollHostChanged(ScrollViewer oldValue) { }
		protected virtual void OnLeftButtonClick(object sender, RoutedEventArgs e) {
			if(RibbonItemsPanel != null) {
				RibbonItemsPanel.IncreaseOffset();
			}
		}
		protected virtual void OnRightButtonClick(object sender, RoutedEventArgs e) {
			if(RibbonItemsPanel != null) {
				RibbonItemsPanel.DecreaseOffset();
			}
		}
		protected override void OnMouseDown(MouseButtonEventArgs e) {
			base.OnMouseDown(e);
			if (Ribbon == null || !Ribbon.CanDragRibbonWindow())
				return;
			UIElement source = e.OriginalSource as UIElement;
			var category = source.With(s => LayoutHelper.FindParentObject<RibbonPageCategoryControl>(source));
			if (category != null)
				return;
			Ribbon.WindowHelper.DragOrMaximizeWindow(this, e);
		}
		protected virtual void UpdateScrollButtonsVisibility() {
			if (RibbonItemsPanel == null || LeftButton == null || RightButton == null)
				return;
			LeftButton.Visibility = RibbonItemsPanel.Offset < 0d ? Visibility.Visible : Visibility.Hidden;
			RightButton.Visibility = RibbonItemsPanel.Offset <= ScrollHost.ViewportWidth - RibbonItemsPanel.RenderSize.Width ?
													  Visibility.Hidden : Visibility.Visible;
		}
		int GetChildOrder(RibbonPageCategoryBase category) {
			if (Ribbon == null || !Ribbon.IsMerged)
				return 0;
			return category.ActualMergeOrder;
		}
		ButtonBase leftButton, rightButton;
		ScrollViewer scrollHost;
	}
}
