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
using System.Windows.Input;
using System.Windows.Media;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
namespace DevExpress.Xpf.Docking.VisualElements {
	[TemplatePart(Name = "PART_Image", Type = typeof(Image))]
	[TemplatePart(Name = "PART_Text", Type = typeof(FrameworkElement))]
	public class CaptionControl : AppearanceControl {
		#region static
		public static readonly DependencyProperty TargetProperty;
		public static readonly DependencyProperty TextWrappingProperty;
		public static readonly DependencyProperty CaptionTextProperty;
		public static readonly DependencyProperty AppearanceProperty;
		static readonly DependencyPropertyKey ActualAppearancePropertyKey;
		public static readonly DependencyProperty ActualAppearanceProperty;
		public static readonly DependencyProperty AlternateForegroundProperty;
		static CaptionControl() {
			Type ownerType = typeof(CaptionControl);
			var dProp = new DependencyPropertyRegistrator<CaptionControl>();
			EventManager.RegisterClassHandler(ownerType, AccessKeyManager.AccessKeyPressedEvent, new AccessKeyPressedEventHandler(OnAccessKeyPressed));
			ToolTipProperty.OverrideMetadata(ownerType, new FrameworkPropertyMetadata(null, 
				(dObj, e) => ((CaptionControl)dObj).OnToolTipChanged((object)e.NewValue),
				(d, v) => ((CaptionControl)d).CoerceToolTip(v)));
			dProp.OverrideDefaultStyleKey(DefaultStyleKeyProperty);
			dProp.Register("Target", ref TargetProperty, (UIElement)null);
			dProp.Register("TextWrapping", ref TextWrappingProperty, TextWrapping.NoWrap, null,
				(dObj, value) => ((CaptionControl)dObj).CoerceTextWrapping((TextWrapping)value));
			dProp.Register("CaptionText", ref CaptionTextProperty, (string)null,
				(dObj, e) => ((CaptionControl)dObj).OnCaptionTextChanged((string)e.NewValue));
			dProp.Register("Appearance", ref AppearanceProperty, (AppearanceObject)null,
				(dObj, e) => ((CaptionControl)dObj).OnAppearanceChanged((AppearanceObject)e.NewValue));
			dProp.RegisterReadonly("ActualAppearance", ref ActualAppearancePropertyKey, ref ActualAppearanceProperty, (AppearanceObject)null);
			dProp.Register("AlternateForeground", ref AlternateForegroundProperty, (Brush)null);
		}
		static void OnAccessKeyPressed(object sender, AccessKeyPressedEventArgs e) {
			CaptionControl captionControl = sender as CaptionControl;
			if((!e.Handled && (e.Scope == null)) && ((e.Target == null) || (e.Target == captionControl))) {
				e.Target = captionControl.Target;
			}
		}
		#endregion static
		public CaptionControl() {
			EnsureAppearance();
			DataContextChanged += OnDataContextChanged;
		}
		void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e) {
			Item = DataContext as BaseLayoutItem;
		}
		void EnsureAppearance() {
			if(ActualAppearance == null) {
				ActualAppearance = new AppearanceObject()
				{
					Background = Background,
					Foreground = Foreground,
					FontWeight = FontWeight,
					FontFamily = FontFamily,
					FontSize = FontSize,
					FontStretch = FontStretch,
					FontStyle = FontStyle
				};
			}
		}
		protected override void OnDispose() {
			Unsubscribe();
			ClearLayoutItemBindings();
			base.OnDispose();
		}
		public Image PartImage { get; private set; }
		public FrameworkElement PartText { get; private set; }
		public ColumnDefinition PartSpace { get; private set; }
		BaseLayoutItem _Item;
		public BaseLayoutItem Item {
			get { return _Item; }
			set {
				if(_Item == value) return;
				_Item = value;
				OnItemChanged();
			}
		}
		private void OnItemChanged() {
			if(Item is LayoutControlItem) {
				Target = ((LayoutControlItem)Item).Control;
				Subscribe();
			}
			SetLayoutItemBindings();
			UpdateAppearance();
		}		
		DevExpress.Xpf.Bars.SplitLayoutPanel PartSplitPanel;
		public string CaptionText {
			get { return (string)GetValue(CaptionTextProperty); }
			set { SetValue(CaptionTextProperty, value); }
		}
		public UIElement Target {
			get { return (UIElement)base.GetValue(TargetProperty); }
			set { base.SetValue(TargetProperty, value); }
		}
		public TextWrapping TextWrapping {
			get { return (TextWrapping)GetValue(TextWrappingProperty); }
			set { SetValue(TextWrappingProperty, value); }
		}
		public AppearanceObject Appearance {
			get { return (AppearanceObject)GetValue(AppearanceProperty); }
			set { SetValue(AppearanceProperty, value); }
		}
		public AppearanceObject ActualAppearance {
			get { return (AppearanceObject)GetValue(ActualAppearanceProperty); }
			private set { SetValue(ActualAppearancePropertyKey, value); }
		}
		AppearanceObject ItemAppearance {
			get { return Item != null ? Item.ActualAppearanceObject : null; }
		}
		public Brush AlternateForeground {
			get { return (Brush)GetValue(AlternateForegroundProperty); }
			set { SetValue(AlternateForegroundProperty, value); }
		}
		protected void Unsubscribe() {
			LayoutUpdated -= OnLayoutUpdated;
		}
		protected void Subscribe() {
			Unsubscribe();
			LayoutUpdated += OnLayoutUpdated;
		}
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			PartImage = GetTemplateChild("PART_Image") as Image;
			PartText = GetTemplateChild("PART_Text") as FrameworkElement;
			PartSpace = GetTemplateChild("PART_Space") as ColumnDefinition;
			PartSplitPanel = LayoutItemsHelper.GetTemplateChild<Bars.SplitLayoutPanel>(this);
			Item = DataContext as BaseLayoutItem;
		}
		protected override Size MeasureOverride(Size constraint) {
			Size baseSize = base.MeasureOverride(constraint);
			string text = CaptionText;
			if(PartText != null && !string.IsNullOrEmpty(text)) {
				FormattedText fText = GetFormattedText(PartText, text);
				double fWidth = fText.Width;
				double dw = Layout.Core.MathHelper.Round(fWidth + 1.0) - fWidth;
				double w = PartText.DesiredSize.Width;
				bool shouldRemeasure = false;
				if(fWidth - w > 0.0 && fWidth - w < 1.0) {
					baseSize = new Size(baseSize.Width + dw, baseSize.Height);
					shouldRemeasure = true;
				}
				double cw = constraint.Width;
				if(fWidth - cw > 0.0 && fWidth - cw < 1.0) {
					baseSize = new Size(cw + dw, baseSize.Height);
					shouldRemeasure = true;
				}
				if(shouldRemeasure)
					base.MeasureOverride(new Size(cw + dw, constraint.Height));
			}
			return baseSize;
		}
		protected override Size ArrangeOverride(Size arrangeBounds) {
			UpdateDesiredCaptionWidth(DesiredSize.Width);
			return base.ArrangeOverride(arrangeBounds);
		}
		protected override void OnVisualChanged() {
			base.OnVisualChanged();
			UpdateAppearance();
		}
		protected virtual void SetLayoutItemBindings() {
			SetupToolTipService();
			BindingHelper.SetBinding(this, ToolTipProperty, Item, BaseLayoutItem.ToolTipProperty);
			BindingHelper.SetBinding(this, TextWrappingProperty, Item, BaseLayoutItem.TextWrappingProperty);
			BindingHelper.SetBinding(this, CaptionTextProperty, Item, BaseLayoutItem.ActualCaptionProperty);
			BindingHelper.SetBinding(this, AppearanceProperty, Item, BaseLayoutItem.ActualAppearanceObjectProperty);
		}
		protected virtual void ClearLayoutItemBindings() {
			BindingHelper.ClearBinding(this, ToolTipProperty);
			BindingHelper.ClearBinding(this, TextWrappingProperty);
			BindingHelper.ClearBinding(this, CaptionTextProperty);
			BindingHelper.ClearBinding(this, AppearanceProperty);
			ResetToolTipService();
		}
		void ResetToolTipService() {
			BindingHelper.ClearBinding(this, ToolTipService.PlacementProperty);
			BindingHelper.ClearBinding(this, ToolTipService.BetweenShowDelayProperty);
			BindingHelper.ClearBinding(this, ToolTipService.HasDropShadowProperty);
			BindingHelper.ClearBinding(this, ToolTipService.HorizontalOffsetProperty);
			BindingHelper.ClearBinding(this, ToolTipService.InitialShowDelayProperty);
			BindingHelper.ClearBinding(this, ToolTipService.IsEnabledProperty);
			BindingHelper.ClearBinding(this, ToolTipService.PlacementRectangleProperty);
			BindingHelper.ClearBinding(this, ToolTipService.ShowDurationProperty);
			BindingHelper.ClearBinding(this, ToolTipService.ShowOnDisabledProperty);
			BindingHelper.ClearBinding(this, ToolTipService.VerticalOffsetProperty);
		}
		void SetupToolTipService() {
			BindingHelper.SetBinding(this, ToolTipService.PlacementProperty, Item, ToolTipService.PlacementProperty);
			BindingHelper.SetBinding(this, ToolTipService.BetweenShowDelayProperty, Item, ToolTipService.BetweenShowDelayProperty);
			BindingHelper.SetBinding(this, ToolTipService.HasDropShadowProperty, Item, ToolTipService.HasDropShadowProperty);
			BindingHelper.SetBinding(this, ToolTipService.HorizontalOffsetProperty, Item, ToolTipService.HorizontalOffsetProperty);
			BindingHelper.SetBinding(this, ToolTipService.InitialShowDelayProperty, Item, ToolTipService.InitialShowDelayProperty);
			BindingHelper.SetBinding(this, ToolTipService.IsEnabledProperty, Item, ToolTipService.IsEnabledProperty);
			BindingHelper.SetBinding(this, ToolTipService.PlacementRectangleProperty, Item, ToolTipService.PlacementRectangleProperty);
			BindingHelper.SetBinding(this, ToolTipService.ShowDurationProperty, Item, ToolTipService.ShowDurationProperty);
			BindingHelper.SetBinding(this, ToolTipService.ShowOnDisabledProperty, Item, ToolTipService.ShowOnDisabledProperty);
			BindingHelper.SetBinding(this, ToolTipService.VerticalOffsetProperty, Item, ToolTipService.VerticalOffsetProperty);
		}
		void OnLayoutUpdated(object sender, EventArgs e) {
			UpdateDesiredCaptionWidth(DesiredSize.Width);
		}
		protected override void OnActualSizeChanged(Size value) {
			base.OnActualSizeChanged(value);
			CoerceValue(ToolTipProperty);
		}
		bool CanSetDesiredCaptionWidth() {
			if(Item.CaptionLocation == CaptionLocation.Bottom || Item.CaptionLocation == CaptionLocation.Top || Item.IsLogicalTreeLocked) return false;
			return VisibilityHelper.GetIsVisible(this);
		}
		void UpdateDesiredCaptionWidth(double desiredWidth) {
			LayoutControlItem controlItem = Item as LayoutControlItem;
			if(controlItem != null && !controlItem.HasDesiredCaptionWidth && CanSetDesiredCaptionWidth()) {
				controlItem.DesiredCaptionWidth = controlItem.HasVisibleCaption ? desiredWidth : 0;
				InvalidateMeasure();
			}
		}
		protected virtual object CoerceTextWrapping(TextWrapping wrapping) {
			if(Item != null && (Item is LayoutControlItem || Item is FixedItem))
				return wrapping;
			return TextWrapping.NoWrap;
		}
		protected virtual void OnToolTipChanged(object newValue) {
		}
		protected virtual object CoerceToolTip(object tooltip) {
			if(tooltip != null) return tooltip;
			if(IsTextTrimmed(PartText, CaptionText))
				return CaptionText;
			return null;
		}
		bool IsTextTrimmed(FrameworkElement textBlock, string text) {
			if(textBlock == null || string.IsNullOrEmpty(text)) return false;
			FormattedText formattedText = GetFormattedText(textBlock, text);
			return formattedText.Width - textBlock.DesiredSize.Width > 1.0;
		}
		protected FormattedText formattedText;
		FormattedText GetFormattedText(FrameworkElement textBlock, string text) {
			if(formattedText == null || formattedText.Text != text) {
				Typeface typeface = new Typeface(
					TextBlock.GetFontFamily(textBlock),
					TextBlock.GetFontStyle(textBlock),
					TextBlock.GetFontWeight(textBlock),
					TextBlock.GetFontStretch(textBlock)
				);
				formattedText = new FormattedText(text,
					System.Globalization.CultureInfo.CurrentCulture,
					TextBlock.GetFlowDirection(textBlock),
					typeface,
					TextBlock.GetFontSize(textBlock),
					TextBlock.GetForeground(textBlock)
				);
			}
			return formattedText;
		}
		protected virtual void OnCaptionTextChanged(string captionText) {
			CoerceValue(ToolTipProperty);
			BaseHeadersPanel.Invalidate(this);
		}
		object TryGetValue(DependencyProperty property, object value) {
			return value ?? GetValue(property);
		}
		object TryGetValue(DependencyProperty property, double value) {
			return !double.IsNaN(value) ? value : GetValue(property);
		}
		GroupBorderStyle GetParentStyle(BaseLayoutItem item) {
			if(item == null) return GroupBorderStyle.NoBorder;
			LayoutGroup group = item as LayoutGroup;
			return group != null && (group.GroupBorderStyle == GroupBorderStyle.Tabbed || group.GroupBorderStyle == GroupBorderStyle.GroupBox) ?
				group.GroupBorderStyle : GetParentStyle(item.Parent);
		}
		bool CanUseAlternateForeground() {
			if(Item is LayoutControlItem && Item.IsPropertyAssigned(BaseLayoutItem.ForegroundProperty)) return false;
			return AlternateForeground != null && GetParentStyle(Item) == GroupBorderStyle.Tabbed;
		}
		bool CanUseDefaultBackground() {
			LayoutGroup group = Item as LayoutGroup;
			if(group != null) return group.GroupBorderStyle == GroupBorderStyle.GroupBox;
			return Item is LayoutPanel;
		}
		protected void UpdateAppearance() {
			if (ItemAppearance!= null) {
				Brush defaultBackground = GetDefaultBackground();
				bool canUseDefaultBackground = CanUseDefaultBackground();
				ActualAppearance.Background = (!canUseDefaultBackground && ItemAppearance.Background != null) ? ItemAppearance.Background : defaultBackground;
				ActualAppearance.Foreground =
					(Brush)TryGetValue(CanUseAlternateForeground() ? AlternateForegroundProperty : ForegroundProperty, ItemAppearance.Foreground);
				ActualAppearance.FontWeight = (FontWeight)TryGetValue(FontWeightProperty, ItemAppearance.FontWeight);
				ActualAppearance.FontFamily = (FontFamily)TryGetValue(FontFamilyProperty, ItemAppearance.FontFamily);
				ActualAppearance.FontSize = (double)TryGetValue(FontSizeProperty, ItemAppearance.FontSize);
				ActualAppearance.FontStretch = (FontStretch)TryGetValue(FontStretchProperty, ItemAppearance.FontStretch);
				ActualAppearance.FontStyle = (FontStyle)TryGetValue(FontStyleProperty, ItemAppearance.FontStyle);
			}
		}
		Brush GetDefaultBackground() {
			object value = BackgroundProperty.GetDefaultValue(typeof(CaptionControl));
			Brush defaultBackground = value is Brush ? (Brush)value : null;
			return defaultBackground;
		}
		protected virtual void OnAppearanceChanged(AppearanceObject newValue) {
			UpdateAppearance();
			formattedText = null;
			InvalidateMeasure();
		}
		#region UIAutomation
		protected override System.Windows.Automation.Peers.AutomationPeer OnCreateAutomationPeer() {
			return new DevExpress.Xpf.Docking.UIAutomation.CaptionControlAutomationPeer(this);
		}
		#endregion
	}
	public class TabCaptionControl : CaptionControl {
		#region static
		public static readonly DependencyProperty CaptionImageProperty;
		public static readonly DependencyProperty HasCaptionImageProperty;
		static readonly DependencyPropertyKey HasCaptionImagePropertyKey;
		[IgnoreDependencyPropertiesConsistencyChecker]
		static readonly DependencyProperty CaptionImageInternalProperty;
		[IgnoreDependencyPropertiesConsistencyChecker]
		static readonly DependencyProperty ShowCaptionImageProperty;
		static TabCaptionControl() {
			var dProp = new DependencyPropertyRegistrator<TabCaptionControl>();
			dProp.OverrideDefaultStyleKey(DefaultStyleKeyProperty);
			dProp.Register("CaptionImage", ref CaptionImageProperty, (ImageSource)null,
				(dObj, ea) => ((TabCaptionControl)dObj).OnCaptionImageChanged((ImageSource)ea.NewValue));
			dProp.RegisterReadonly("HasCaptionImage", ref HasCaptionImagePropertyKey, ref HasCaptionImageProperty, false);
			dProp.Register("CaptionImageInternal", ref CaptionImageInternalProperty, (ImageSource)null,
				(dObj, ea) => ((TabCaptionControl)dObj).OnCaptionImageInternalChanged((ImageSource)ea.NewValue));
			dProp.Register("ShowCaptionImage", ref ShowCaptionImageProperty, false,
				(dObj, ea) => ((TabCaptionControl)dObj).OnShowCaptionImageChanged((bool)ea.NewValue));
		}
		#endregion
		public TabCaptionControl() {
		}
		protected virtual void OnCaptionImageInternalChanged(ImageSource value) {
			CaptionImage = GetCaptionImage(CaptionImageInternal);
		}
		void UpdateHasCaptionImageProperty() {
			SetValue(HasCaptionImagePropertyKey, CaptionImage != null && ShowCaptionImage);
		}
		protected virtual void OnCaptionImageChanged(ImageSource value) {
			UpdateHasCaptionImageProperty();
		}
		protected virtual void OnShowCaptionImageChanged(bool newValue) {
			UpdateHasCaptionImageProperty();
		}
		protected virtual ImageSource GetCaptionImage(ImageSource value) {
			if(value == null && string.IsNullOrEmpty(CaptionText) && Manager != null)
				return Item != null && Item.IsAutoHidden ? Manager.DefaultAutoHidePanelCaptionImage : Manager.DefaultTabPageCaptionImage;
			return value;
		}
		protected override void OnCaptionTextChanged(string captionText) {
			base.OnCaptionTextChanged(captionText);
			CaptionImage = GetCaptionImage(CaptionImageInternal);
		}
		protected override void SetLayoutItemBindings() {
			base.SetLayoutItemBindings();
			BindingHelper.SetBinding(this, CaptionImageInternalProperty, Item, BaseLayoutItem.CaptionImageProperty);
			BindingHelper.SetBinding(this, CaptionTextProperty, Item, BaseLayoutItem.ActualTabCaptionProperty);
			BindingHelper.SetBinding(this, ShowCaptionImageProperty, Item, BaseLayoutItem.ShowCaptionImageProperty);
		}
		protected override void ClearLayoutItemBindings() {
			BindingHelper.ClearBinding(this, CaptionImageInternalProperty);
			BindingHelper.ClearBinding(this, CaptionTextProperty);
			BindingHelper.ClearBinding(this, ShowCaptionImageProperty);
			base.ClearLayoutItemBindings();
		}
		protected override void OnVisualChanged() {
			base.OnVisualChanged();
			formattedText = null;
		}
		public bool HasCaptionImage {
			get { return (bool)GetValue(HasCaptionImageProperty); }
		}
		bool ShowCaptionImage {
			get { return (bool)GetValue(ShowCaptionImageProperty); }
		}
		public ImageSource CaptionImage {
			get { return (ImageSource)GetValue(CaptionImageProperty); }
			set { SetValue(CaptionImageProperty, value); }
		}
		ImageSource CaptionImageInternal {
			get { return (ImageSource)GetValue(CaptionImageInternalProperty); }
		}
		public DockLayoutManager Manager {
			get { return DockLayoutManager.GetDockLayoutManager(this); }
		}
	}
	public class TemplatedCaptionControl : psvControl, IDisposable {
		#region static
		public static readonly DependencyProperty CaptionControlTemplateProperty;
		public static readonly DependencyProperty ContentPresenterTemplateProperty;
		public static readonly DependencyProperty LayoutItemProperty;
		public static readonly DependencyProperty CaptionMarginProperty;
		[IgnoreDependencyPropertiesConsistencyChecker]
		static readonly DependencyProperty ActualCaptionProperty;
		[IgnoreDependencyPropertiesConsistencyChecker]
		static readonly DependencyProperty ActualDataContextProperty;
		[IgnoreDependencyPropertiesConsistencyChecker]
		static readonly DependencyProperty ActualCaptionTemplateProperty;
		[IgnoreDependencyPropertiesConsistencyChecker]
		public static readonly DependencyProperty ActualContentProperty;
		static TemplatedCaptionControl() {
			var dProp = new DependencyPropertyRegistrator<TemplatedCaptionControl>();
			dProp.OverrideDefaultStyleKey(DefaultStyleKeyProperty);
			dProp.Register("CaptionControlTemplate", ref CaptionControlTemplateProperty, (ControlTemplate)null);
			dProp.Register("ContentPresenterTemplate", ref ContentPresenterTemplateProperty, (ControlTemplate)null);
			dProp.Register("LayoutItem", ref LayoutItemProperty, (BaseLayoutItem)null,
				(dObj, ea) => ((TemplatedCaptionControl)dObj).OnLayoutItemChanged((BaseLayoutItem)ea.OldValue, (BaseLayoutItem)ea.NewValue));
			dProp.Register("CaptionMargin", ref CaptionMarginProperty, (Thickness)new Thickness());
			dProp.Register("ActualCaption", ref ActualCaptionProperty, (object)null,
				(dObj, e) => ((TemplatedCaptionControl)dObj).OnItemPropertyChanged());
			dProp.Register("ActualDataContext", ref ActualDataContextProperty, (object)null,
				(dObj, e) => ((TemplatedCaptionControl)dObj).OnItemPropertyChanged());
			dProp.Register("ActualCaptionTemplate", ref ActualCaptionTemplateProperty, (DataTemplate)null,
				(dObj, e) => ((TemplatedCaptionControl)dObj).OnItemPropertyChanged());
			dProp.Register("ActualContent", ref ActualContentProperty, (object)null);
		}
		#endregion
		public TemplatedCaptionControl() {
		}
		protected override void OnDispose() {
			if(PartCaption != null) {
				PartCaption.Dispose();
				PartCaption = null;
			}
			ClearValue(CaptionControlTemplateProperty);
			ClearValue(ContentPresenterTemplateProperty);
			ClearValue(LayoutItemProperty);
			base.OnDispose();
		}
		public CaptionControl PartCaption { get; private set; }
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			PartCaption = LayoutItemsHelper.GetTemplateChild<CaptionControl>(this);
		}
		protected override void OnInitialized() {
			base.OnInitialized();
			SelectTemplate();
		}
		protected bool HasCaptionTemplate(BaseLayoutItem item) {
			return LayoutItem.CaptionTemplate != null || LayoutItem.CaptionTemplateSelector != null;
		}
		protected virtual void SelectTemplate() {
			if(LayoutItem == null) return;
			bool useCaptionTemplate = (LayoutItem.Caption == null || LayoutItem.Caption is string || LayoutItem.Caption.GetType().IsPrimitive) && !HasCaptionTemplate(LayoutItem);
			Template = useCaptionTemplate ? CaptionControlTemplate : ContentPresenterTemplate;
			if(!useCaptionTemplate)
				ActualContent = LayoutItem.Caption ?? LayoutItem.DataContext;
		}
		protected virtual void OnLayoutItemChanged(BaseLayoutItem oldValue, BaseLayoutItem newValue) {
			if(newValue != null) {
				BindingHelper.SetBinding(this, ActualCaptionProperty, newValue, "Caption");
				BindingHelper.SetBinding(this, ActualDataContextProperty, newValue, "DataContext");
				BindingHelper.SetBinding(this, ActualCaptionTemplateProperty, newValue, "CaptionTemplate");
				BindingHelper.SetBinding(this, HorizontalContentAlignmentProperty, newValue, BaseLayoutItem.CaptionHorizontalAlignmentProperty);
				BindingHelper.SetBinding(this, VerticalContentAlignmentProperty, newValue, BaseLayoutItem.CaptionVerticalAlignmentProperty);
				BindingHelper.SetBinding(this, HorizontalAlignmentProperty, newValue, BaseLayoutItem.CaptionHorizontalAlignmentProperty);
				BindingHelper.SetBinding(this, VerticalAlignmentProperty, newValue, BaseLayoutItem.CaptionVerticalAlignmentProperty);
			}
			else {
				BindingHelper.ClearBinding(this, ActualCaptionProperty);
				BindingHelper.ClearBinding(this, ActualDataContextProperty);
				BindingHelper.ClearBinding(this, ActualCaptionTemplateProperty);
				BindingHelper.ClearBinding(this, HorizontalContentAlignmentProperty);
				BindingHelper.ClearBinding(this, VerticalContentAlignmentProperty);
				BindingHelper.ClearBinding(this, HorizontalAlignmentProperty);
				BindingHelper.ClearBinding(this, VerticalAlignmentProperty);
			}
			OnItemPropertyChanged();
		}
		public void OnItemPropertyChanged() {
			SelectTemplate();
		}
		public ControlTemplate CaptionControlTemplate {
			get { return (ControlTemplate)GetValue(CaptionControlTemplateProperty); }
			set { SetValue(CaptionControlTemplateProperty, value); }
		}
		public ControlTemplate ContentPresenterTemplate {
			get { return (ControlTemplate)GetValue(ContentPresenterTemplateProperty); }
			set { SetValue(ContentPresenterTemplateProperty, value); }
		}
		public BaseLayoutItem LayoutItem {
			get { return (BaseLayoutItem)GetValue(LayoutItemProperty); }
			set { SetValue(LayoutItemProperty, value); }
		}
		public Thickness CaptionMargin {
			get { return (Thickness)GetValue(CaptionMarginProperty); }
			set { SetValue(CaptionMarginProperty, value); }
		}
		public object ActualContent {
			get { return (object)GetValue(ActualContentProperty); }
			set { SetValue(ActualContentProperty, value); }
		}
	}
	public class TemplatedTabCaptionControl : TemplatedCaptionControl {
		[IgnoreDependencyPropertiesConsistencyChecker]
		static readonly DependencyProperty ActualTabCaptionProperty;
		static TemplatedTabCaptionControl() {
			var dProp = new DependencyPropertyRegistrator<TemplatedTabCaptionControl>();
			dProp.OverrideDefaultStyleKey(DefaultStyleKeyProperty);
			dProp.Register("ActualTabCaption", ref ActualTabCaptionProperty, (object)null,
				(dObj, e) => ((TemplatedCaptionControl)dObj).OnItemPropertyChanged());
		}
		public TemplatedTabCaptionControl() {
		}
		protected override void SelectTemplate() {
			if(LayoutItem == null) return;
			bool useCaptionTemplate = (LayoutItem.TabCaption == null || LayoutItem.TabCaption is string || LayoutItem.TabCaption.GetType().IsPrimitive) && !HasCaptionTemplate(LayoutItem);
			Template = useCaptionTemplate ? CaptionControlTemplate : ContentPresenterTemplate;
			if(!useCaptionTemplate) 
				ActualContent = LayoutItem.TabCaption ?? LayoutItem.DataContext;
		}
		protected override void OnLayoutItemChanged(BaseLayoutItem oldValue, BaseLayoutItem newValue) {
			base.OnLayoutItemChanged(oldValue, newValue);
			if(newValue != null) {
				BindingHelper.SetBinding(this, ActualTabCaptionProperty, newValue, "TabCaption");
			}
			else BindingHelper.ClearBinding(this, ActualTabCaptionProperty);
		}
	}
}
