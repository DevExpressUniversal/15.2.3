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
using System.Windows.Input;
using System.Windows.Media;
using DevExpress.Xpf.Utils.Themes;
using System.Windows.Threading;
using System.Windows.Data;
using DevExpress.Xpf.Bars;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Bars.Helpers;
using DevExpress.Xpf.Utils;
using DevExpress.Mvvm.Native;
namespace DevExpress.Xpf.Ribbon {
	public class RibbonCaptionControl : Control {
		#region static
		public static readonly DependencyProperty IsPressedProperty;
		public static readonly DependencyProperty IsSelectedProperty;
		public static readonly DependencyProperty MaxDesiredSizeProperty;
		static RibbonCaptionControl() {
			IsSelectedProperty = DependencyPropertyManager.Register("IsSelected", typeof(bool), typeof(RibbonCaptionControl), new FrameworkPropertyMetadata(false, new PropertyChangedCallback(OnIsSelectedPropertyChanged)));
			IsPressedProperty = DependencyPropertyManager.Register("IsPressed", typeof(bool), typeof(RibbonCaptionControl), new UIPropertyMetadata(false));			
			MaxDesiredSizeProperty = DependencyPropertyManager.Register("MaxDesiredSize", typeof(Size), typeof(RibbonCaptionControl), new FrameworkPropertyMetadata(default(Size)));
		}		
		static protected void OnIsSelectedPropertyChanged(DependencyObject o, DependencyPropertyChangedEventArgs e) {
			((RibbonCaptionControl)o).OnIsSelectedChanged((bool)e.OldValue);
		}
		#endregion
		#region dep props
		public bool IsPressed {
			get { return (bool)GetValue(IsPressedProperty); }
			set { SetValue(IsPressedProperty, value); }
		}
		public bool IsSelected {
			get { return (bool)GetValue(IsSelectedProperty); }
			set { SetValue(IsSelectedProperty, value); }
		}
		#endregion
		#region props
		#endregion
		public RibbonCaptionControl() {
			Loaded += new RoutedEventHandler(OnLoaded);
		}
		protected virtual void OnLoaded(object sender, RoutedEventArgs e) {
			if(BlendHelper.IsInBlendTemplateEditor(this)) PrepareControlForBlendEditor();		   
		}
		protected override void OnMouseDown(MouseButtonEventArgs e) {
			base.OnMouseDown(e);
			IsPressed = true;
		}
		protected override void OnMouseUp(MouseButtonEventArgs e) {
			base.OnMouseUp(e);
			IsPressed = false;
		}
		protected override void OnMouseLeave(MouseEventArgs e) {
			base.OnMouseLeave(e);
			IsPressed = false;
		}
		protected virtual void PrepareControlForBlendEditor() { }
		protected internal virtual RibbonControl Ribbon { get { return LayoutHelper.FindParentObject<RibbonControl>(this); } }
		protected internal ContentViewport ContentViewport { get; private set; }	   
		public double MaxEnlargeValue { get { return double.PositiveInfinity; } }
		public double MaxReduceIndentValue {
			get {
				if(BestIndent > GetTotalReduceValue()) return GetTotalReduceValue();
				return BestIndent;				
			}
		}
		public double MaxReduceWidthValue {
			get { return Math.Max(GetTotalReduceValue() - BestIndent, 0); }
		}
		public Size MaxDesiredSize {
			get { return (Size)GetValue(MaxDesiredSizeProperty); }
			set { SetValue(MaxDesiredSizeProperty, value); }
		}
		public double BestIndent {
			get { return Ribbon == null ? 0 : Ribbon.MaxPageCaptionTextIndent; }
		}
		public double BestWidth {
			get { return DevExpress.Xpf.Core.LayoutDoubleHelper.CeilScaledValue(Math.Max(GetTotalWidth(), MinWidth)); }
		}
		public double BestDesiredWidth {
			get { return DesiredSize.Width - ActualWidth + Math.Max(GetTotalWidth(), MinWidth) - Margin.Right; }
		}
		double GetTotalContentWidth() {
			return ContentViewport == null ? BestIndent : ContentViewport.ContentSize.Width + BestIndent;
		}
		double GetTotalWidth() {
			if(ContentViewport == null)
				return ActualWidth + GetTotalContentWidth();
			else
				return ActualWidth - ContentViewport.ActualWidth + GetTotalContentWidth();
		}
		double GetTotalReduceValue() {
			return Math.Max(GetTotalWidth() - MinWidth, 0);
		}
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			ContentViewport = (ContentViewport)GetTemplateChild("PART_Content");
		}
		protected virtual void OnIsSelectedChanged(bool oldValue) { }
		protected override Size MeasureOverride(Size constraint) {
			MaxDesiredSize = base.MeasureOverride(SizeHelper.Infinite);
			return base.MeasureOverride(constraint);
		}
	}
	public class RibbonPageCaptionControl : RibbonCaptionControl {
		#region static
		public static readonly DependencyProperty PageProperty;
		public static readonly DependencyProperty SelectedTextStyleProperty;
		public static readonly DependencyProperty NormalTextStyleProperty;
		public static readonly DependencyProperty HoverTextStyleProperty;
		public static readonly DependencyProperty ActualTextStyleProperty;
		protected static readonly DependencyPropertyKey ActualTextStylePropertyKey;
		public static readonly DependencyProperty IsDefaultCategoryProperty;
		public static readonly DependencyProperty NormalContentContainerStyleProperty;
		public static readonly DependencyProperty HighlightedContentContainerStyleProperty;
		public static readonly DependencyProperty ActualContentContainerStyleProperty;
		protected static readonly DependencyPropertyKey ActualContentContainerStylePropertyKey;
		public static readonly DependencyProperty ColorProperty;
		public static readonly DependencyProperty NormalTextStyleInMinimizedRibbonProperty;
		public static readonly DependencyProperty SelectedTextStyleInMinimizedRibbonProperty;
		public static readonly DependencyProperty HoverTextStyleInMinimizedRibbonProperty;
		public static readonly DependencyProperty IsRibbonMinimizedProperty;
		public static readonly DependencyProperty ActualBorderTemplateProperty;
		protected static readonly DependencyPropertyKey ActualBorderTemplatePropertyKey;
		public static readonly DependencyProperty BorderTemplateProperty;
		public static readonly DependencyProperty HighlightedBorderTemplateProperty;
		public static readonly DependencyProperty BorderTemplateInMinimizedRibbonProperty;
		public static readonly DependencyProperty HighlightedBorderTemplateInMinimizedRibbonProperty;
		static RibbonPageCaptionControl() {
			DefaultStyleKeyProperty.OverrideMetadata(typeof(RibbonPageCaptionControl), new FrameworkPropertyMetadata(typeof(RibbonPageCaptionControl)));
			PageProperty = DependencyPropertyManager.Register("Page", typeof(RibbonPage), typeof(RibbonPageCaptionControl), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnPagePropertyChanged)));
			SelectedTextStyleProperty = DependencyPropertyManager.Register("SelectedTextStyle", typeof(Style), typeof(RibbonPageCaptionControl), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnSelectedTextStylePropertyChanged)));
			NormalTextStyleProperty = DependencyPropertyManager.Register("NormalTextStyle", typeof(Style), typeof(RibbonPageCaptionControl), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnNormalTextStylePropertyChanged)));
			HoverTextStyleProperty = DependencyPropertyManager.Register("HoverTextStyle", typeof(Style), typeof(RibbonPageCaptionControl), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnHoverTextStylePropertyChanged)));
			ActualTextStylePropertyKey = DependencyPropertyManager.RegisterReadOnly("ActualTextStyle", typeof(Style), typeof(RibbonPageCaptionControl), new FrameworkPropertyMetadata(null));
			ActualTextStyleProperty = ActualTextStylePropertyKey.DependencyProperty;
			IsDefaultCategoryProperty = DependencyPropertyManager.Register("IsDefaultCategory", typeof(bool), typeof(RibbonPageCaptionControl), new FrameworkPropertyMetadata(false, new PropertyChangedCallback(OnIsDefaultCategoryPropertyChanged)));
			NormalContentContainerStyleProperty = DependencyPropertyManager.Register("NormalContentContainerStyle", typeof(Style), typeof(RibbonPageCaptionControl), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnNormalContentContainerStylePropertyChanged)));
			HighlightedContentContainerStyleProperty = DependencyPropertyManager.Register("HighlightedContentContainerStyle", typeof(Style), typeof(RibbonPageCaptionControl), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnHighlightedContentContainerStylePropertyChanged)));
			ActualContentContainerStylePropertyKey = DependencyPropertyManager.RegisterReadOnly("ActualContentContainerStyle", typeof(Style), typeof(RibbonPageCaptionControl), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnHighlightedContentContainerStylePropertyChanged)));
			ActualContentContainerStyleProperty = ActualContentContainerStylePropertyKey.DependencyProperty;
			ColorProperty = DependencyPropertyManager.Register("Color", typeof(Color), typeof(RibbonPageCaptionControl), new FrameworkPropertyMetadata(Colors.Transparent));
			SelectedTextStyleInMinimizedRibbonProperty = DependencyPropertyManager.Register("SelectedTextStyleInMinimizedRibbon", typeof(Style), typeof(RibbonPageCaptionControl), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnSelectedTextStyleInMinimizedRibbonPropertyChanged)));
			NormalTextStyleInMinimizedRibbonProperty = DependencyPropertyManager.Register("NormalTextStyleInMinimizedRibbon", typeof(Style), typeof(RibbonPageCaptionControl), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnNormalTextStyleInMinimizedRibbonPropertyChanged)));
			HoverTextStyleInMinimizedRibbonProperty = DependencyPropertyManager.Register("HoverTextStyleInMinimizedRibbon", typeof(Style), typeof(RibbonPageCaptionControl), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnHoverTextStyleInMinimizedRibbonPropertyChanged)));
			IsRibbonMinimizedProperty = DependencyPropertyManager.Register("IsRibbonMinimized", typeof(bool), typeof(RibbonPageCaptionControl), new FrameworkPropertyMetadata(false, new PropertyChangedCallback(OnIsRibbonMinimizedPropertyChanged)));
			ActualBorderTemplatePropertyKey = DependencyPropertyManager.RegisterReadOnly("ActualBorderTemplate", typeof(ControlTemplate), typeof(RibbonPageCaptionControl), new FrameworkPropertyMetadata(null));
			ActualBorderTemplateProperty = ActualBorderTemplatePropertyKey.DependencyProperty;
			BorderTemplateProperty = DependencyPropertyManager.Register("BorderTemplate", typeof(ControlTemplate), typeof(RibbonPageCaptionControl), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnBorderTemplatePropertyChanged)));
			HighlightedBorderTemplateProperty = DependencyPropertyManager.Register("HighlightedBorderTemplate", typeof(ControlTemplate), typeof(RibbonPageCaptionControl), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnHighlightedBorderTemplatePropertyChanged)));
			BorderTemplateInMinimizedRibbonProperty = DependencyPropertyManager.Register("BorderTemplateInMinimizedRibbon", typeof(ControlTemplate), typeof(RibbonPageCaptionControl), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnBorderTemplateInMinimizedRibbonPropertyChanged)));
			HighlightedBorderTemplateInMinimizedRibbonProperty = DependencyPropertyManager.Register("HighlightedBorderTemplateInMinimizedRibbon", typeof(ControlTemplate), typeof(RibbonPageCaptionControl), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnHighlightedBorderTemplateInMinimizedRibbonPropertyChanged)));
		}
		static protected void OnSelectedTextStylePropertyChanged(DependencyObject o, DependencyPropertyChangedEventArgs e) {
			((RibbonPageCaptionControl)o).OnSelectedTextStyleChanged(e.OldValue as Style);
		}
		static protected void OnNormalTextStylePropertyChanged(DependencyObject o, DependencyPropertyChangedEventArgs e) {
			((RibbonPageCaptionControl)o).OnNormalTextStyleChanged(e.OldValue as Style);
		}
		static protected void OnIsDefaultCategoryPropertyChanged(DependencyObject o, DependencyPropertyChangedEventArgs e) {
			((RibbonPageCaptionControl)o).OnIsDefaultCategoryChanged((bool)e.OldValue);
		}
		static protected void OnNormalContentContainerStylePropertyChanged(DependencyObject o, DependencyPropertyChangedEventArgs e) {
			((RibbonPageCaptionControl)o).OnNormalContentContainerStyleChanged(e.OldValue as Style);
		}
		static protected void OnHighlightedContentContainerStylePropertyChanged(DependencyObject o, DependencyPropertyChangedEventArgs e) {
			((RibbonPageCaptionControl)o).OnHighlightedContentContainerStyleChanged(e.OldValue as Style);
		}
		static protected void OnPagePropertyChanged(DependencyObject o, DependencyPropertyChangedEventArgs e) {
			((RibbonPageCaptionControl)o).OnPageChanged(e.OldValue as RibbonPage);
		}
		protected static void OnNormalTextStyleInMinimizedRibbonPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((RibbonPageCaptionControl)d).OnNormalTextStyleInMinimizedRibbonChanged((Style)e.OldValue);
		}		
		protected static void OnHoverTextStylePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((RibbonPageCaptionControl)d).OnHoverTextStyleChanged((Style)e.OldValue);
		}
		protected static void OnSelectedTextStyleInMinimizedRibbonPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((RibbonPageCaptionControl)d).OnSelectedTextStyleInMinimizedRibbonChanged((Style)e.OldValue);
		}		
		protected static void OnHoverTextStyleInMinimizedRibbonPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((RibbonPageCaptionControl)d).OnHoverTextStyleInMinimizedRibbonChanged((Style)e.OldValue);
		}
		protected static void OnIsRibbonMinimizedPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((RibbonPageCaptionControl)d).OnIsRibbonMinimizedChanged((bool)e.OldValue);
		}
		protected static void OnBorderTemplatePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((RibbonPageCaptionControl)d).OnBorderTemplateChanged((ControlTemplate)e.OldValue);
		}
		protected static void OnHighlightedBorderTemplatePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((RibbonPageCaptionControl)d).OnHighlightedBorderTemplateChanged((ControlTemplate)e.OldValue);
		}
		protected static void OnBorderTemplateInMinimizedRibbonPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((RibbonPageCaptionControl)d).OnBorderTemplateInMinimizedRibbonChanged((ControlTemplate)e.OldValue);
		}
		protected static void OnHighlightedBorderTemplateInMinimizedRibbonPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((RibbonPageCaptionControl)d).OnHighlightedBorderTemplateInMinimizedRibbonChanged((ControlTemplate)e.OldValue);
		}
		#endregion
		#region dep props
		public RibbonPage Page {
			get { return (RibbonPage)GetValue(PageProperty); }
			set { SetValue(PageProperty, value); }
		}
		public Style SelectedTextStyle {
			get { return (Style)GetValue(SelectedTextStyleProperty); }
			set { SetValue(SelectedTextStyleProperty, value); }
		}
		public Style NormalTextStyle {
			get { return (Style)GetValue(NormalTextStyleProperty); }
			set { SetValue(NormalTextStyleProperty, value); }
		}
		public Style HoverTextStyle {
			get { return (Style)GetValue(HoverTextStyleProperty); }
			set { SetValue(HoverTextStyleProperty, value); }
		}
		public Style ActualTextStyle {
			get { return (Style)GetValue(ActualTextStyleProperty); }
			protected set { this.SetValue(ActualTextStylePropertyKey, value); }
		}
		public ControlTemplate ActualBorderTemplate {
			get { return (ControlTemplate)GetValue(ActualBorderTemplateProperty); }
			protected set { this.SetValue(ActualBorderTemplatePropertyKey, value); }
		}
		public bool IsDefaultCategory {
			get { return (bool)GetValue(IsDefaultCategoryProperty); }
			set { SetValue(IsDefaultCategoryProperty, value); }
		}
		public Style NormalContentContainerStyle {
			get { return (Style)GetValue(NormalContentContainerStyleProperty); }
			set { SetValue(NormalContentContainerStyleProperty, value); }
		}
		public Style HighlightedContentContainerStyle {
			get { return (Style)GetValue(HighlightedContentContainerStyleProperty); }
			set { SetValue(HighlightedContentContainerStyleProperty, value); }
		}
		public Style ActualContentContainerStyle {
			get { return (Style)GetValue(ActualContentContainerStyleProperty); }
			protected set { this.SetValue(ActualContentContainerStylePropertyKey, value); }
		}
		public Color Color {
			get { return (Color)GetValue(ColorProperty); }
			set { SetValue(ColorProperty, value); }
		}
		public Style NormalTextStyleInMinimizedRibbon {
			get { return (Style)GetValue(NormalTextStyleInMinimizedRibbonProperty); }
			set { SetValue(NormalTextStyleInMinimizedRibbonProperty, value); }
		}
		public Style HoverTextStyleInMinimizedRibbon {
			get { return (Style)GetValue(HoverTextStyleInMinimizedRibbonProperty); }
			set { SetValue(HoverTextStyleInMinimizedRibbonProperty, value); }
		}
		public Style SelectedTextStyleInMinimizedRibbon {
			get { return (Style)GetValue(SelectedTextStyleInMinimizedRibbonProperty); }
			set { SetValue(SelectedTextStyleInMinimizedRibbonProperty, value); }
		}
		public bool IsRibbonMinimized {
			get { return (bool)GetValue(IsRibbonMinimizedProperty); }
			set { SetValue(IsRibbonMinimizedProperty, value); }
		}
		public ControlTemplate BorderTemplate {
			get { return (ControlTemplate)GetValue(BorderTemplateProperty); }
			set { SetValue(BorderTemplateProperty, value); }
		}
		public ControlTemplate HighlightedBorderTemplate {
			get { return (ControlTemplate)GetValue(HighlightedBorderTemplateProperty); }
			set { SetValue(HighlightedBorderTemplateProperty, value); }
		}
		public ControlTemplate BorderTemplateInMinimizedRibbon {
			get { return (ControlTemplate)GetValue(BorderTemplateInMinimizedRibbonProperty); }
			set { SetValue(BorderTemplateInMinimizedRibbonProperty, value); }
		}
		public ControlTemplate HighlightedBorderTemplateInMinimizedRibbon {
			get { return (ControlTemplate)GetValue(HighlightedBorderTemplateInMinimizedRibbonProperty); }
			set { SetValue(HighlightedBorderTemplateInMinimizedRibbonProperty, value); }
		}
		#endregion
		#region props  
		private RibbonPageHeaderControl pageHeaderControlCore = null;
		public RibbonPageHeaderControl PageHeaderControl {
			get { return pageHeaderControlCore; }
			set {
				if(pageHeaderControlCore == value)
					return;
				RibbonPageHeaderControl oldValue = pageHeaderControlCore;
				pageHeaderControlCore = value;
				OnPageHeaderControlChanged(oldValue);
			}
		}
		protected virtual void OnPageHeaderControlChanged(RibbonPageHeaderControl oldValue) {
			ClearValue(IsRibbonMinimizedProperty);
			if(PageHeaderControl != null)
				SetBinding(IsRibbonMinimizedProperty, new Binding("IsRibbonMinimized") { Source = PageHeaderControl });
		}
		#endregion
		public RibbonPageCaptionControl() {
			CreateBindings();
		}
		protected virtual void OnSelectedTextStyleChanged(Style oldValue) {
			UpdateActualTextStyle();
		}
		protected virtual void OnNormalTextStyleChanged(Style oldValue) {
			UpdateActualTextStyle();
		}
		protected virtual void OnIsDefaultCategoryChanged(bool oldValue) {
			UpdateActualContentContainerStyle();
			UpdateActualBorderTemplate();
		}
		protected virtual void OnNormalContentContainerStyleChanged(Style oldValue) {
			UpdateActualContentContainerStyle();
		}
		protected virtual void OnHighlightedContentContainerStyleChanged(Style oldValue) {
			UpdateActualContentContainerStyle();
		}
		protected virtual void OnNormalTextStyleInMinimizedRibbonChanged(Style oldValue) {
			UpdateActualTextStyle();
		}
		protected virtual void OnHoverTextStyleChanged(Style oldValue) {
			UpdateActualTextStyle();
		}
		protected virtual void OnSelectedTextStyleInMinimizedRibbonChanged(Style oldValue) {
			UpdateActualTextStyle();
		}
		protected virtual void OnHoverTextStyleInMinimizedRibbonChanged(Style oldValue) {
			UpdateActualTextStyle();
		}
		protected virtual void OnIsRibbonMinimizedChanged(bool oldValue) {
			UpdateActualTextStyle();
			UpdateActualBorderTemplate();
		}
		protected virtual void OnPageChanged(RibbonPage oldValue) {
		}
		protected virtual void OnBorderTemplateChanged(ControlTemplate oldValue) {
			UpdateActualBorderTemplate();
		}
		protected virtual void OnHighlightedBorderTemplateChanged(ControlTemplate oldValue) {
			UpdateActualBorderTemplate();
		}
		protected virtual void OnBorderTemplateInMinimizedRibbonChanged(ControlTemplate oldValue) {
			UpdateActualBorderTemplate();
		}
		protected virtual void OnHighlightedBorderTemplateInMinimizedRibbonChanged(ControlTemplate oldValue) {
			UpdateActualBorderTemplate();
		}
		protected virtual void UpdateActualBorderTemplate() {
			if(IsRibbonMinimized) {
				ActualBorderTemplate = IsDefaultCategory ? BorderTemplateInMinimizedRibbon : HighlightedBorderTemplateInMinimizedRibbon;
			} else {
				ActualBorderTemplate = IsDefaultCategory ? BorderTemplate : HighlightedBorderTemplate;
			}
		}
		protected internal override RibbonControl Ribbon {
			get { return PageHeaderControl == null ? null : PageHeaderControl.Ribbon; }
		}
		protected virtual void CreateBindings() {
			CreateBinding("Page.PageCategory.IsDefault", IsDefaultCategoryProperty);
		}
		private void CreateBinding(string path, DependencyProperty property) {
			Binding bnd = new Binding(path);
			bnd.Source = this;
			this.SetBinding(property, bnd);
		}
		protected virtual void UpdateActualTextStyle() {
			if (IsSelected) {
				ActualTextStyle = IsRibbonMinimized ? SelectedTextStyleInMinimizedRibbon : SelectedTextStyle;
			} else if (IsMouseOver) {
				ActualTextStyle = IsRibbonMinimized ? HoverTextStyleInMinimizedRibbon : HoverTextStyle;
			} else
				ActualTextStyle = IsRibbonMinimized ? NormalTextStyleInMinimizedRibbon : NormalTextStyle;
		}
		protected virtual void UpdateActualContentContainerStyle() {
			ActualContentContainerStyle = IsDefaultCategory ? NormalContentContainerStyle : HighlightedContentContainerStyle;
		}
		protected ContentControl UserContent {
			get { return userContent; }
			private set {
				if (userContent == value)
					return;
				var oldValue = userContent;
				userContent = value;
				OnUserContentChanged(oldValue);
			}
		}
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			UpdateActualTextStyle();
			UpdateActualContentContainerStyle();
			UserContent = GetTemplateChild("PART_UserContent") as ContentControl;
		}
		protected override void OnIsSelectedChanged(bool oldValue) {
			base.OnIsSelectedChanged(oldValue);
			UpdateActualTextStyle();
		}
		protected override void OnMouseEnter(MouseEventArgs e) {
			base.OnMouseEnter(e);
			UpdateActualTextStyle();
		}
		protected override void OnMouseLeave(MouseEventArgs e) {
			base.OnMouseLeave(e);
			UpdateActualTextStyle();
		}
		protected virtual void OnUserContentChanged(ContentControl oldValue) {
			if (oldValue!=null)
				oldValue.SizeChanged += OnUserContentSizeChanged;
			if (UserContent != null)
				UserContent.SizeChanged += OnUserContentSizeChanged;
		}
		void OnUserContentSizeChanged(object sender, SizeChangedEventArgs e) {
			if (Ribbon == null)
				return;
			var parents = LayoutHelper.GetRootPath(Ribbon.CategoriesPane, this).OfType<UIElement>();
			foreach (var item in parents) {
				item.InvalidateMeasure();
			}
		}
		ContentControl userContent;
	}
}
