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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using DevExpress.Xpf.Docking.Base;
using DevExpress.Xpf.Layout.Core;
namespace DevExpress.Xpf.Docking.VisualElements {
	[TemplatePart(Name = "PART_Header", Type = typeof(DockPaneHeaderPresenter))]
	[TemplatePart(Name = "PART_Content", Type = typeof(DockPaneContentPresenter))]
	public class DockPane : psvHeaderedContentControl, Bars.IMDIChildHost {
		#region static
		public static readonly DependencyProperty HitTestTypeProperty;
		public static readonly DependencyProperty ControlHostTemplateProperty;
		public static readonly DependencyProperty LayoutHostTemplateProperty;
		public static readonly DependencyProperty DataHostTemplateProperty;
		public static readonly DependencyProperty CaptionActiveBackgroundProperty;
		public static readonly DependencyProperty CaptionNormalBackgroundProperty;
		public static readonly DependencyProperty CaptionActiveForegroundProperty;
		public static readonly DependencyProperty CaptionNormalForegroundProperty;
		public static readonly DependencyProperty ActualCaptionBackgroundProperty;
		public static readonly DependencyProperty ActualCaptionForegroundProperty;
		public static readonly DependencyProperty CaptionCornerRadiusProperty;
		public static readonly DependencyProperty FloatingCaptionCornerRadiusProperty;
		public static readonly DependencyProperty ActualCaptionCornerRadiusProperty;
		public static readonly DependencyProperty BorderMarginProperty;
		public static readonly DependencyProperty BorderPaddingProperty;
		public static readonly DependencyProperty BarContainerMarginProperty;
		public static readonly DependencyProperty ContentMarginProperty;
		public static readonly DependencyProperty ActualBorderThicknessProperty;
		public static readonly DependencyProperty ActualBorderMarginProperty;
		public static readonly DependencyProperty ActualBorderPaddingProperty;
		static DockPane() {
			var dProp = new DependencyPropertyRegistrator<DockPane>();
			dProp.OverrideDefaultStyleKey(DefaultStyleKeyProperty);
			dProp.Register("ControlHostTemplate", ref ControlHostTemplateProperty, (DataTemplate)null);
			dProp.Register("LayoutHostTemplate", ref LayoutHostTemplateProperty, (DataTemplate)null);
			dProp.Register("DataHostTemplate", ref DataHostTemplateProperty, (DataTemplate)null);
			dProp.Register("CaptionActiveBackground", ref CaptionActiveBackgroundProperty, (Brush)null);
			dProp.Register("CaptionNormalBackground", ref CaptionNormalBackgroundProperty, (Brush)null);
			dProp.Register("CaptionActiveForeground", ref CaptionActiveForegroundProperty, (Brush)null);
			dProp.Register("CaptionNormalForeground", ref CaptionNormalForegroundProperty, (Brush)null);
			dProp.Register("ActualCaptionBackground", ref ActualCaptionBackgroundProperty, (Brush)null);
			dProp.Register("ActualCaptionForeground", ref ActualCaptionForegroundProperty, (Brush)null);
			dProp.Register("CaptionCornerRadius", ref CaptionCornerRadiusProperty, new CornerRadius(0));
			dProp.Register("FloatingCaptionCornerRadius", ref FloatingCaptionCornerRadiusProperty, new CornerRadius(0));
			dProp.Register("ActualCaptionCornerRadius", ref ActualCaptionCornerRadiusProperty, new CornerRadius(0));
			dProp.Register("BorderMargin", ref BorderMarginProperty, new Thickness(0));
			dProp.Register("BorderPadding", ref BorderPaddingProperty, new Thickness(0));
			dProp.Register("ActualBorderThickness", ref ActualBorderThicknessProperty, new Thickness(0));
			dProp.Register("ActualBorderMargin", ref ActualBorderMarginProperty, new Thickness(0));
			dProp.Register("ActualBorderPadding", ref ActualBorderPaddingProperty, new Thickness(0));
			dProp.Register("BarContainerMargin", ref BarContainerMarginProperty, new Thickness(0));
			dProp.Register("ContentMargin", ref ContentMarginProperty, new Thickness(0));
			dProp.RegisterAttachedInherited("HitTestType", ref HitTestTypeProperty, HitTestType.Undefined);
		}
		public static HitTestType GetHitTestType(DependencyObject obj) {
			return (HitTestType)obj.GetValue(HitTestTypeProperty);
		}
		public static void SetHitTestType(DependencyObject obj, HitTestType value) {
			obj.SetValue(HitTestTypeProperty, value);
		}
		#endregion static
		public DockPane() {
		}
		protected override void OnDispose() {
			if(PartContent != null) {
				PartContent.Dispose();
				PartContent = null;
			}
			if(PartHeader != null) {
				PartHeader.Dispose();
				PartHeader = null;
			}
			ClearValue(ControlHostTemplateProperty);
			ClearValue(LayoutHostTemplateProperty);
			ClearValue(DataHostTemplateProperty);
			isChildMenuVisibleChangedHandlers.Clear();
			base.OnDispose();
		}
		protected override void OnActualSizeChanged(Size value) {
			base.OnActualSizeChanged(value);
			if(IsDisposing) return;
			if(LayoutItem != null) LayoutItem.LayoutSize = value;
		}
		public DataTemplate ControlHostTemplate {
			get { return (DataTemplate)GetValue(ControlHostTemplateProperty); }
			set { SetValue(ControlHostTemplateProperty, value); }
		}
		public DataTemplate LayoutHostTemplate {
			get { return (DataTemplate)GetValue(LayoutHostTemplateProperty); }
			set { SetValue(LayoutHostTemplateProperty, value); }
		}
		public DataTemplate DataHostTemplate {
			get { return (DataTemplate)GetValue(DataHostTemplateProperty); }
			set { SetValue(DataHostTemplateProperty, value); }
		}
		public Brush CaptionActiveBackground {
			get { return (Brush)GetValue(CaptionActiveBackgroundProperty); }
			set { SetValue(CaptionActiveBackgroundProperty, value); }
		}
		public Brush CaptionNormalBackground {
			get { return (Brush)GetValue(CaptionNormalBackgroundProperty); }
			set { SetValue(CaptionNormalBackgroundProperty, value); }
		}
		public Brush CaptionActiveForeground {
			get { return (Brush)GetValue(CaptionActiveForegroundProperty); }
			set { SetValue(CaptionActiveForegroundProperty, value); }
		}
		public Brush CaptionNormalForeground {
			get { return (Brush)GetValue(CaptionNormalForegroundProperty); }
			set { SetValue(CaptionNormalForegroundProperty, value); }
		}
		public Brush ActualCaptionForeground {
			get { return (Brush)GetValue(ActualCaptionForegroundProperty); }
			set { SetValue(ActualCaptionForegroundProperty, value); }
		}
		public Brush ActualCaptionBackground {
			get { return (Brush)GetValue(ActualCaptionBackgroundProperty); }
			set { SetValue(ActualCaptionBackgroundProperty, value); }
		}
		public CornerRadius CaptionCornerRadius {
			get { return (CornerRadius)GetValue(CaptionCornerRadiusProperty); }
			set { SetValue(CaptionCornerRadiusProperty, value); }
		}
		public CornerRadius FloatingCaptionCornerRadius {
			get { return (CornerRadius)GetValue(FloatingCaptionCornerRadiusProperty); }
			set { SetValue(FloatingCaptionCornerRadiusProperty, value); }
		}
		public CornerRadius ActualCaptionCornerRadius {
			get { return (CornerRadius)GetValue(ActualCaptionCornerRadiusProperty); }
			set { SetValue(ActualCaptionCornerRadiusProperty, value); }
		}
		public Thickness BorderMargin {
			get { return (Thickness)GetValue(BorderMarginProperty); }
			set { SetValue(BorderMarginProperty, value); }
		}
		public Thickness BorderPadding {
			get { return (Thickness)GetValue(BorderPaddingProperty); }
			set { SetValue(BorderPaddingProperty, value); }
		}
		public Thickness ActualBorderThickness {
			get { return (Thickness)GetValue(ActualBorderThicknessProperty); }
			set { SetValue(ActualBorderThicknessProperty, value); }
		}
		public Thickness ActualBorderMargin {
			get { return (Thickness)GetValue(ActualBorderMarginProperty); }
			set { SetValue(ActualBorderMarginProperty, value); }
		}
		public Thickness ActualBorderPadding {
			get { return (Thickness)GetValue(ActualBorderPaddingProperty); }
			set { SetValue(ActualBorderPaddingProperty, value); }
		}
		public Thickness BarContainerMargin {
			get { return (Thickness)GetValue(BarContainerMarginProperty); }
			set { SetValue(BarContainerMarginProperty, value); }
		}
		public Thickness ContentMargin {
			get { return (Thickness)GetValue(ContentMarginProperty); }
			set { SetValue(ContentMarginProperty, value); }
		}
		public DockPaneHeaderPresenter PartHeader { get; private set; }
		public DockPaneContentPresenter PartContent { get; private set; }
		LayoutPanel LayoutPanel { get { return LayoutItem as LayoutPanel; } }
		bool IsTemplateApplied;
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			IsTemplateApplied = true;
			if(PartHeader != null && !LayoutItemsHelper.IsTemplateChild(PartHeader, this))
				PartHeader.Dispose();
			PartHeader = GetTemplateChild("PART_Header") as DockPaneHeaderPresenter;
			if(PartHeader != null) {
				PartHeader.EnsureOwner(this);
				BindingHelper.SetBinding(PartHeader, DockPaneHeaderPresenter.IsCaptionVisibleProperty, LayoutItem, "IsCaptionVisible");
				BindingHelper.SetBinding(PartHeader, DockPaneHeaderPresenter.BackgroundProperty, this, "ActualCaptionBackground");
				BindingHelper.SetBinding(PartHeader, DockPaneHeaderPresenter.ForegroundProperty, this, "ActualCaptionForeground");
			}
			if(PartContent != null && !LayoutItemsHelper.IsTemplateChild(PartContent, this))
				PartContent.Dispose();
			PartContent = GetTemplateChild("PART_Content") as DockPaneContentPresenter;
			if(PartContent != null) {
				PartContent.EnsureOwner(this);
				BindingHelper.SetBinding(PartContent, DockPaneContentPresenter.IsControlItemsHostProperty, LayoutItem, "IsControlItemsHost");
				BindingHelper.SetBinding(PartContent, DockPaneContentPresenter.IsDataBoundProperty, LayoutItem, "IsDataBound");
			}
			LayoutPanel panel = LayoutItem as LayoutPanel;
			if(panel != null) {
				UpdateBrushes(panel);
				UpdateGeometry(panel);
			}
		}
		protected override void Unsubscribe(BaseLayoutItem item) {
			base.Unsubscribe(item);
			if(item != null) {
				item.VisualChanged -= OnItemVisualChanged;
				item.GeometryChanged -= OnItemGeometryChanged;
			}
		}
		protected override void Subscribe(BaseLayoutItem item) {
			base.Subscribe(item);
			if(item != null) {
				item.VisualChanged += OnItemVisualChanged;
				item.GeometryChanged += OnItemGeometryChanged;
			}
		}
		void OnItemVisualChanged(object sender, System.EventArgs e) {
			LayoutPanel panel = sender as LayoutPanel;
			if(panel != null)
				UpdateBrushes(panel);
		}
		void OnItemGeometryChanged(object sender, System.EventArgs e) {
			LayoutPanel panel = sender as LayoutPanel;
			if(panel != null)
				UpdateGeometry(panel);
		}
		protected override void OnLayoutItemChanged(BaseLayoutItem item, BaseLayoutItem oldItem) {
			base.OnLayoutItemChanged(item, oldItem);
			if(!IsTemplateApplied) return;
			LayoutPanel panel = item as LayoutPanel;
			if(panel != null) {
				if(PartHeader != null)
					BindingHelper.SetBinding(PartHeader, DockPaneHeaderPresenter.IsCaptionVisibleProperty, LayoutItem, "IsCaptionVisible");
				if(PartContent != null) {
					BindingHelper.SetBinding(PartContent, DockPaneContentPresenter.IsControlItemsHostProperty, LayoutItem, "IsControlItemsHost");
					BindingHelper.SetBinding(PartContent, DockPaneContentPresenter.IsDataBoundProperty, LayoutItem, "IsDataBound");
				}
				UpdateBrushes(panel);
				UpdateGeometry(panel);
			}
		}
		bool HasBorderCore() {
			return LayoutPanel != null ? LayoutPanel.HasBorder && !LayoutPanel.IsDockedAsDocument : false;
		}
		protected void UpdateGeometry(LayoutPanel panel) {
			UpdateActualBorderThickness(panel);
			UpdateActualBorderMargin(panel);
			UpdateActualBorderPadding(panel);
			UpdateCaptionCornerRadius(panel);
			UpdatePartContentMargin(panel);
			InvalidateMeasure();
		}
		protected void UpdateBrushes(LayoutPanel panel) {
			UpdateActualCaptionBackground(panel);
			UpdateActualCaptionForeground(panel);
		}
		void UpdateActualCaptionBackground(LayoutPanel panel) {
			bool isActive = panel.IsActive;
			Brush captionBackground = isActive ? CaptionActiveBackground : CaptionNormalBackground;
			if(panel.ActualAppearanceObject != null && panel.ActualAppearanceObject.Background != null)
				captionBackground = panel.ActualAppearanceObject.Background;
			ActualCaptionBackground = captionBackground;
		}
		void UpdateActualCaptionForeground(LayoutPanel panel) {
			bool isActive = panel.IsActive;
			Brush captionForeground = isActive ? CaptionActiveForeground : CaptionNormalForeground;
			if(panel.ActualAppearanceObject != null && panel.ActualAppearanceObject.Foreground != null)
				captionForeground = panel.ActualAppearanceObject.Foreground;
			ActualCaptionForeground = captionForeground;
		}
		void UpdateActualBorderThickness(LayoutPanel panel) {
			Thickness borderThickness = BorderThickness;
			if(panel.IsTabPage) {
				borderThickness = new Thickness(borderThickness.Left, borderThickness.Top, borderThickness.Right, 0);
			}
			if(!HasBorderCore()) {
				borderThickness = new Thickness(0);
			}
			ActualBorderThickness = borderThickness;
		}
		void UpdateActualBorderMargin(LayoutPanel panel) {
			Thickness borderMargin = BorderMargin;
			if(panel.IsTabPage && !panel.IsDockedAsDocument) {
				borderMargin = new Thickness(borderMargin.Left, borderMargin.Top, borderMargin.Right, 0);
			}
			else borderMargin = new Thickness(HasBorderCore() ? borderMargin.Left : 0);
			ActualBorderMargin = borderMargin;
		}
		void UpdateActualBorderPadding(LayoutPanel panel) {
			Thickness borderPadding = BorderPadding;
			if(panel.IsTabPage && !panel.IsDockedAsDocument) {
				borderPadding = new Thickness(0, 0, 0, borderPadding.Bottom);
			}
			else borderPadding = new Thickness(0);
			ActualBorderPadding = borderPadding;
		}
		void UpdatePartContentMargin(LayoutPanel panel) {
			if(PartContent == null) return;
			bool hasBorder = HasBorderCore();
			PartContent.BarContainerMargin = hasBorder ? BarContainerMargin : new Thickness(0);
			PartContent.ContentMargin = hasBorder ? ContentMargin : new Thickness(0);
		}
		void UpdateCaptionCornerRadius(LayoutPanel panel) {
			CornerRadius cornerRadius = panel.IsFloatingRootItem ? FloatingCaptionCornerRadius : CaptionCornerRadius;
			if(!panel.ShowCaption) {
				cornerRadius = new CornerRadius(0, 0, cornerRadius.BottomRight, cornerRadius.BottomLeft);
			}
			if(!HasBorderCore()) cornerRadius = new CornerRadius();
			ActualCaptionCornerRadius = cornerRadius;
		}
		#region IMDIChildHost Members
		bool _IsChildMenuVisibleCore = true;
		protected bool IsChildMenuVisibleCore {
			get { return _IsChildMenuVisibleCore; }
			set {
				if(_IsChildMenuVisibleCore == value) return;
				_IsChildMenuVisibleCore = value;
				NotifyListeners();
			}
		}
		protected virtual void NotifyListeners() {
			foreach(EventHandler handler in isChildMenuVisibleChangedHandlers) {
				if(handler != null) handler(this, EventArgs.Empty);
			}
		}
		protected DevExpress.Xpf.Bars.Native.WeakList<EventHandler> isChildMenuVisibleChangedHandlers = new DevExpress.Xpf.Bars.Native.WeakList<EventHandler>();
		bool Bars.IMDIChildHost.IsChildMenuVisible {
			get { return _IsChildMenuVisibleCore; }
		}
		event EventHandler Bars.IMDIChildHost.IsChildMenuVisibleChanged {
			add { isChildMenuVisibleChangedHandlers.Add(value); }
			remove { isChildMenuVisibleChangedHandlers.Remove(value); }
		}
		#endregion
	}
	[TemplatePart(Name = "PART_Caption", Type = typeof(CaptionControl))]
	[TemplatePart(Name = "PART_CaptionBackground", Type = typeof(Border))]
	[TemplatePart(Name = "PART_ControlBox", Type = typeof(BaseControlBoxControl))]
	public class DockPaneHeaderPresenter :
		BasePanePresenter<DockPane, LayoutPanel> {
		#region static
		public static readonly DependencyProperty IsCaptionVisibleProperty;
		public static readonly DependencyProperty BackgroundProperty;
		public static readonly DependencyProperty ForegroundProperty;
		static DockPaneHeaderPresenter() {
			var dProp = new DependencyPropertyRegistrator<DockPaneHeaderPresenter>();
			dProp.Register("IsCaptionVisible", ref IsCaptionVisibleProperty, true,
				(dObj, e) => ((DockPaneHeaderPresenter)dObj).OnIsCaptionVisibleChanged((bool)e.NewValue));
			dProp.Register("Background", ref BackgroundProperty, (Brush)null);
			dProp.Register("Foreground", ref ForegroundProperty, (Brush)null);
		}
		#endregion static
		class DefaultHeaderTemplateSelector : DefaultItemTemplateSelectorWrapper.DefaultItemTemplateSelector {
			public override DataTemplate SelectTemplate(object item, DependencyObject container) {
				DockPaneHeaderPresenter presenter = container as DockPaneHeaderPresenter;
				if(presenter != null && presenter.Owner != null) {
					return presenter.Owner.HeaderTemplate;
				}
				return null;
			}
		}
		DataTemplateSelector defaultCaptionTemplateSelector;
		DataTemplateSelector _DefaultCaptionTemplateSelector {
			get {
				if(defaultCaptionTemplateSelector == null)
					defaultCaptionTemplateSelector = new DefaultHeaderTemplateSelector();
				return defaultCaptionTemplateSelector;
			}
		}
		protected override void OnDispose() {
			BindingHelper.ClearBinding(this, IsCaptionVisibleProperty);
			BindingHelper.ClearBinding(this, BackgroundProperty);
			BindingHelper.ClearBinding(this, ForegroundProperty);
			if(PartCaptionPresenter != null) {
				PartCaptionPresenter.Dispose();
				PartCaptionPresenter = null;
			}
			if(PartControlBox != null) {
				PartControlBox.Dispose();
				PartControlBox = null;
			}
			base.OnDispose();
		}
		public bool IsCaptionVisible {
			get { return (bool)GetValue(IsCaptionVisibleProperty); }
			set { SetValue(IsCaptionVisibleProperty, value); }
		}
		public Brush Background {
			get { return (Brush)GetValue(BackgroundProperty); }
			set { SetValue(BackgroundProperty, value); }
		}
		public Brush Foreground {
			get { return (Brush)GetValue(ForegroundProperty); }
			set { SetValue(ForegroundProperty, value); }
		}
		protected virtual void OnIsCaptionVisibleChanged(bool visible) {
			Visibility = VisibilityHelper.Convert(visible);
		}
		protected override bool CanSelectTemplate(LayoutPanel panel) {
			return _DefaultCaptionTemplateSelector != null;
		}
		protected override DataTemplate SelectTemplateCore(LayoutPanel panel) {
			return _DefaultCaptionTemplateSelector.SelectTemplate(panel, this);
		}
		protected TemplatedCaptionControl PartCaptionPresenter { get; private set; }
		protected Border PartCaptionBackground { get; private set; }
		protected BaseControlBoxControl PartControlBox { get; private set; }
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			PartCaptionPresenter = GetTemplateChild("PART_CaptionControlPresenter") as TemplatedCaptionControl;
			if(PartCaptionPresenter != null)
				this.Forward(PartCaptionPresenter, TemplatedCaptionControl.ForegroundProperty, "Foreground");
			PartCaptionBackground = GetTemplateChild("PART_CaptionBackground") as Border;
			if(PartCaptionBackground != null) {
				this.Forward(PartCaptionBackground, Border.BackgroundProperty, "Background");
			}
			if(PartControlBox != null && !LayoutItemsHelper.IsTemplateChild(PartControlBox, this))
				PartControlBox.Dispose();
			PartControlBox = GetTemplateChild("PART_ControlBox") as BaseControlBoxControl;
		}
		protected override LayoutPanel ConvertToLogicalItem(object content) {
			return LayoutItemData.ConvertToBaseLayoutItem(content) as LayoutPanel ?? base.ConvertToLogicalItem(content);
		}
	}
	public class DockPaneContentPresenter : DockItemContentPresenter<DockPane, LayoutPanel> {
		#region static
		public static readonly DependencyProperty BarContainerMarginProperty;
		public static readonly DependencyProperty ContentMarginProperty;
		static DockPaneContentPresenter() {
			var dProp = new DependencyPropertyRegistrator<DockPaneContentPresenter>();
			dProp.Register("BarContainerMargin", ref BarContainerMarginProperty, new Thickness(0));
			dProp.Register("ContentMargin", ref ContentMarginProperty, new Thickness(0));
		}
		#endregion static
		class DefaultContentTemplateSelector : DataTemplateSelector {
			public override DataTemplate SelectTemplate(object item, DependencyObject container) {
				DockPaneContentPresenter presenter = container as DockPaneContentPresenter;
				LayoutPanel panel = item as LayoutPanel;
				if(panel != null && presenter != null && presenter.Owner != null) {
					return panel.IsControlItemsHost ?
						presenter.Owner.LayoutHostTemplate :
						panel.IsDataBound ? presenter.Owner.DataHostTemplate : presenter.Owner.ControlHostTemplate;
				}
				return null;
			}
		}
		DataTemplateSelector defaultContentTemplateSelector;
		DataTemplateSelector _DefaultContentTemplateSelector {
			get {
				if(defaultContentTemplateSelector == null)
					defaultContentTemplateSelector = new DefaultContentTemplateSelector();
				return defaultContentTemplateSelector;
			}
		}
		protected override void OnDispose() {
			if(PartBarContainer != null) {
				PartBarContainer.Dispose();
				PartBarContainer = null;
			}
			if(PartControl != null) {
				PartControl.Dispose();
				PartControl = null;
			}
			if(PartLayout != null) {
				PartLayout.Dispose();
				PartLayout = null;
			}
			if(PartContent != null) {
				PartContent.Dispose();
				PartContent = null;
			}
			base.OnDispose();
		}
		public Thickness BarContainerMargin {
			get { return (Thickness)GetValue(BarContainerMarginProperty); }
			set { SetValue(BarContainerMarginProperty, value); }
		}
		public Thickness ContentMargin {
			get { return (Thickness)GetValue(ContentMarginProperty); }
			set { SetValue(ContentMarginProperty, value); }
		}
		public bool IsControlItemsHost {
			get { return (bool)GetValue(IsControlItemsHostProperty); }
			set { SetValue(IsControlItemsHostProperty, value); }
		}
		protected override bool CanSelectTemplate(LayoutPanel panel) {
			return _DefaultContentTemplateSelector != null;
		}
		protected override DataTemplate SelectTemplateCore(LayoutPanel panel) {
			return _DefaultContentTemplateSelector.SelectTemplate(panel, this);
		}
		public DockBarContainerControl PartBarContainer { get; private set; }
		public psvContentPresenter PartLayout { get; private set; }
		public UIElementPresenter PartControl { get; private set; }
		public psvContentPresenter PartContent { get; private set; }
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			if(PartBarContainer != null && !LayoutItemsHelper.IsTemplateChild(PartBarContainer, this))
				PartBarContainer.Dispose();
			PartBarContainer = LayoutItemsHelper.GetTemplateChild<DockBarContainerControl>(this);
			if(PartBarContainer != null) {
				PartBarContainer.Margin = BarContainerMargin;
			}
			if(PartControl != null && !LayoutItemsHelper.IsTemplateChild(PartControl, this))
				PartControl.Dispose();
			PartControl = LayoutItemsHelper.GetTemplateChild<UIElementPresenter>(this);
			if(PartControl != null) {
				PartControl.Margin = ContentMargin;
			}
			if(PartLayout != null && !LayoutItemsHelper.IsTemplateChild(PartLayout, this))
				PartLayout.Dispose();
			ScrollViewer scrollViewer = LayoutItemsHelper.GetTemplateChild<ScrollViewer>(this);
			if(scrollViewer != null) {
				PartLayout = scrollViewer.Content as psvContentPresenter;
			}
			if(PartContent != null && !LayoutItemsHelper.IsTemplateChild(PartContent, this))
				PartContent.Dispose();
			PartContent = LayoutItemsHelper.GetTemplateChild<psvContentPresenter>(this, false);
		}
	}
}
