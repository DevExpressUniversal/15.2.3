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
using System.ComponentModel;
using System.Windows.Markup;
using System.Windows.Data;
using System.Windows.Controls;
using System.Windows.Media;
using DevExpress.Xpf.Core;
using System.Windows.Documents;
using DevExpress.Xpf.Core.Native;
using System.Windows.Input;
using DevExpress.Xpf.Utils;
using DevExpress.Mvvm.Native;
namespace DevExpress.Xpf.NavBar {
	[DefaultProperty("Content"), ContentProperty("Content")]
	public class NavBarItem : DXFrameworkContentElement {
		public static readonly DependencyProperty ContentProperty;
		public static readonly DependencyProperty IsVisibleProperty;
		public static readonly DependencyProperty TemplateProperty;
		public static readonly DependencyProperty IsSelectedProperty;
		public static readonly DependencyProperty ImageSourceProperty;
		internal static readonly DependencyPropertyKey GroupPropertyKey;
		public static readonly DependencyProperty GroupProperty;
		public static readonly DependencyProperty CommandProperty;
		public static readonly DependencyProperty CommandParameterProperty;
		public static readonly DependencyProperty CommandTargetProperty ;
		public static readonly DependencyProperty DisplayModeProperty;
		public static readonly DependencyProperty LayoutSettingsProperty;
		public static readonly DependencyProperty ImageSettingsProperty;
		public static readonly DependencyProperty FontSettingsProperty;
		public static readonly DependencyProperty ActualFontSettingsProperty;
		public static readonly DependencyProperty ActualDisplayModeProperty;
		public static readonly DependencyProperty ActualLayoutSettingsProperty;
		public static readonly DependencyProperty ActualImageSettingsProperty;
		public static readonly DependencyPropertyKey ActualDisplayModePropertyKey;
		public static readonly DependencyPropertyKey ActualLayoutSettingsPropertyKey;
		public static readonly DependencyPropertyKey ActualImageSettingsPropertyKey;
		protected static readonly DependencyPropertyKey ActualFontSettingsPropertyKey;		
		public static readonly DependencyProperty VisualStyleProperty;
		static readonly DependencyPropertyKey ActualVisualStylePropertyKey;
		public static readonly DependencyProperty ActualVisualStyleProperty;
		[IgnoreDependencyPropertiesConsistencyChecker]
		protected static readonly DependencyProperty SourceContentProperty;
		static NavBarItem() {
			SourceContentProperty = DependencyPropertyManager.Register("SourceContent", typeof(object), typeof(NavBarItem), new FrameworkPropertyMetadata(null, (d, e) => SetCurrentValueIfDefault(d, NavBarItem.ContentProperty, e.NewValue)));
			ContentProperty = DependencyPropertyManager.Register("Content", typeof(object), typeof(NavBarItem), new PropertyMetadata(null, (d,e)=>((NavBarItem)d).OnContentPropertyChanged(e)));
			IsVisibleProperty = DependencyPropertyManager.Register("IsVisible", typeof(bool), typeof(NavBarItem), new PropertyMetadata(true));
			TemplateProperty = DependencyPropertyManager.Register("Template", typeof(DataTemplate), typeof(NavBarItem), new PropertyMetadata(null));			
			IsSelectedProperty = DependencyPropertyManager.Register("IsSelected", typeof(bool), typeof(NavBarItem), new FrameworkPropertyMetadata(false, (d,e)=>((NavBarItem)d).If(x=>x.IsSelected).With(x=>x.Group).With(x=>x.NavBar).With(x=>x.SelectionStrategy).Do(x=>x.SelectItem(d))));
			ImageSourceProperty = DependencyPropertyManager.Register("ImageSource", typeof(ImageSource), typeof(NavBarItem), new PropertyMetadata(null));
			GroupPropertyKey = DependencyPropertyManager.RegisterReadOnly("Group", typeof(NavBarGroup), typeof(NavBarItem), new FrameworkPropertyMetadata(null, OnGroupChanged));
			GroupProperty = GroupPropertyKey.DependencyProperty;
			CommandProperty = DependencyPropertyManager.Register("Command", typeof(ICommand), typeof(NavBarItem), new FrameworkPropertyMetadata(null, OnCommandPropertyChanged));
			CommandParameterProperty = DependencyPropertyManager.Register("CommandParameter", typeof(object), typeof(NavBarItem), new FrameworkPropertyMetadata(null, (d, e) => ((NavBarItem)d).UpdateCanExecute()));
			CommandTargetProperty = DependencyPropertyManager.Register("CommandTarget", typeof(IInputElement), typeof(NavBarItem), new FrameworkPropertyMetadata(null, (d,e) => ((NavBarItem)d).UpdateCanExecute()));
			VisualStyleProperty = DependencyPropertyManager.Register("VisualStyle", typeof(Style), typeof(NavBarItem), new PropertyMetadata(null, OnVisualStyleChanged));
			ActualVisualStylePropertyKey = DependencyPropertyManager.RegisterReadOnly("ActualVisualStyle", typeof(Style), typeof(NavBarItem), new FrameworkPropertyMetadata(null, (d, e) => ((NavBarItem)d).OnActualVisualStyleChanged(d, e)));
			ActualVisualStyleProperty = ActualVisualStylePropertyKey.DependencyProperty;
			DisplayModeProperty = DependencyPropertyManager.Register("DisplayMode", typeof(DisplayMode), typeof(NavBarItem), new FrameworkPropertyMetadata(DisplayMode.Default, new PropertyChangedCallback(OnDisplayModePropertyChanged)));
			LayoutSettingsProperty = DependencyPropertyManager.Register("LayoutSettings", typeof(LayoutSettings), typeof(NavBarItem), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnLayoutSettingsPropertyChanged)));
			ImageSettingsProperty = DependencyPropertyManager.Register("ImageSettings", typeof(ImageSettings), typeof(NavBarItem), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnImageSettingsPropertyChanged)));
			ActualDisplayModePropertyKey = DependencyPropertyManager.RegisterReadOnly("ActualDisplayMode", typeof(DisplayMode), typeof(NavBarItem), new FrameworkPropertyMetadata(DisplayMode.Default));
			ActualLayoutSettingsPropertyKey = DependencyPropertyManager.RegisterReadOnly("ActualLayoutSettings", typeof(LayoutSettings), typeof(NavBarItem), new FrameworkPropertyMetadata(null));
			ActualImageSettingsPropertyKey = DependencyPropertyManager.RegisterReadOnly("ActualImageSettings", typeof(ImageSettings), typeof(NavBarItem), new FrameworkPropertyMetadata(null));
			FontSettingsProperty = DependencyPropertyManager.Register("FontSettings", typeof(FontSettings), typeof(NavBarItem), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnFontSettingsPropertyChanged)));
			ActualFontSettingsPropertyKey = DependencyPropertyManager.RegisterReadOnly("ActualFontSettings", typeof(FontSettings), typeof(NavBarItem), new FrameworkPropertyMetadata(null));
			ActualFontSettingsProperty = ActualFontSettingsPropertyKey.DependencyProperty;
			ActualDisplayModeProperty = ActualDisplayModePropertyKey.DependencyProperty;
			ActualLayoutSettingsProperty = ActualLayoutSettingsPropertyKey.DependencyProperty;
			ActualImageSettingsProperty = ActualImageSettingsPropertyKey.DependencyProperty;
		}
		object sourceObject;
		protected internal object SourceObject {
			get { return sourceObject; }
			set {
				sourceObject = value;
				if (System.Windows.DependencyPropertyHelper.GetValueSource(this, NavBarItem.ContentProperty).BaseValueSource == BaseValueSource.Default)
					this.SetBinding(NavBarItem.SourceContentProperty, new Binding());
			}
		}
		static void SetCurrentValueIfDefault(DependencyObject dObj, DependencyProperty dp, object value) {
			if (System.Windows.DependencyPropertyHelper.GetValueSource(dObj, dp).BaseValueSource == BaseValueSource.Default)
				dObj.SetCurrentValue(dp, value);
		}
		protected override System.Collections.IEnumerator LogicalChildren {
			get {
				if(Content != null)
					return new object[] { Content }.GetEnumerator();
				return base.LogicalChildren;
			}
		}
		public NavBarItem() {
		}
		private void OnContentPropertyChanged(DependencyPropertyChangedEventArgs e) {
			if(e.OldValue != null)
				RemoveLogicalChild(e.OldValue);
			if(e.NewValue != null)
				AddLogicalChild(e.NewValue);
		}
		static void OnGroupChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			NavBarItem item = (NavBarItem)d;
			item.UpdateActualVisualStyle();
			item.UpdateActualLayoutSettings();
			item.UpdateActualDisplayMode();
			item.UpdateActualImageSettings();
			item.UpdateActualFontSettings();
			NavBarGroup oldGroup = e.OldValue as NavBarGroup;
			NavBarGroup newGroup = e.NewValue as NavBarGroup;			
			if(oldGroup != null) {
				oldGroup.RemoveChild(item);
			}
			if(newGroup != null) {
				newGroup.AddChild(item);
				if (item.IsSelected)
					newGroup.NavBar.With(x => x.SelectionStrategy).Do(x => x.SelectItem(item));
			}
		}
		static void OnCommandPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((NavBarItem)d).OnCommandChanged((ICommand)e.OldValue, (ICommand)e.NewValue);
		}
		static void OnVisualStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((NavBarItem)d).UpdateActualVisualStyle();
		}
		protected static void OnDisplayModePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((NavBarItem)d).OnDisplayModeChanged((DisplayMode)e.OldValue);
		}
		protected static void OnLayoutSettingsPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((NavBarItem)d).OnLayoutSettingsChanged((LayoutSettings)e.OldValue);
		}
		protected static void OnImageSettingsPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((NavBarItem)d).OnImageSettingsChanged((ImageSettings)e.OldValue);
		}
		protected static void OnFontSettingsPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((NavBarItem)d).OnFontSettingsChanged((FontSettings)e.OldValue);
		}
		protected virtual void OnFontSettingsChanged(FontSettings oldValue) {
			UpdateActualFontSettings();
		}
		protected virtual void OnImageSettingsChanged(ImageSettings oldValue) {
			UpdateActualImageSettings();
		}
		protected virtual void OnLayoutSettingsChanged(LayoutSettings oldValue) {
			UpdateActualLayoutSettings();
		}
		protected virtual void OnDisplayModeChanged(DisplayMode oldValue) {
			UpdateActualDisplayMode();
		}
		[
#if !SL
	DevExpressXpfNavBarLocalizedDescription("NavBarItemContent"),
#endif
 TypeConverter(typeof(ObjectConverter)), Category(Categories.Data)]
		public object Content {
			get { return (object)GetValue(ContentProperty); }
			set { SetValue(ContentProperty, value); }
		}
		[
#if !SL
	DevExpressXpfNavBarLocalizedDescription("NavBarItemIsVisible"),
#endif
 Category(Categories.Appearance)]
		public bool IsVisible {
			get { return (bool)GetValue(IsVisibleProperty); }
			set { SetValue(IsVisibleProperty, value); }
		}
		[
#if !SL
	DevExpressXpfNavBarLocalizedDescription("NavBarItemTemplate"),
#endif
 Category(Categories.Templates)]
		public DataTemplate Template {
			get { return (DataTemplate)GetValue(TemplateProperty); }
			set { SetValue(TemplateProperty, value); }
		}
#if !SL
	[DevExpressXpfNavBarLocalizedDescription("NavBarItemIsSelected")]
#endif
		public bool IsSelected {
			get { return (bool)GetValue(IsSelectedProperty); }
			set { SetValue(IsSelectedProperty, value); }
		}
		[EditorBrowsable(EditorBrowsableState.Always), Browsable(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		public new bool IsEnabled {
			get { return base.IsEnabled; }
			set { base.IsEnabled = value; }
		}
		[
#if !SL
	DevExpressXpfNavBarLocalizedDescription("NavBarItemImageSource"),
#endif
 Category(Categories.Data)]
		public ImageSource ImageSource {
			get { return (ImageSource)GetValue(ImageSourceProperty); }
			set { SetValue(ImageSourceProperty, value); }
		}
#if !SL
	[DevExpressXpfNavBarLocalizedDescription("NavBarItemGroup")]
#endif
		public NavBarGroup Group {
			get { return (NavBarGroup)GetValue(GroupProperty); }
			internal set { this.SetValue(GroupPropertyKey, value); }
		}
		[
#if !SL
	DevExpressXpfNavBarLocalizedDescription("NavBarItemVisualStyle"),
#endif
 Category(Categories.Appearance)]
		public Style VisualStyle {
			get { return (Style)GetValue(VisualStyleProperty); }
			set { SetValue(VisualStyleProperty, value); }
		}
#if !SL
	[DevExpressXpfNavBarLocalizedDescription("NavBarItemActualVisualStyle")]
#endif
		public Style ActualVisualStyle {
			get { return (Style)GetValue(ActualVisualStyleProperty); }
			private set { this.SetValue(ActualVisualStylePropertyKey, value); }
		}
		[
#if !SL
	DevExpressXpfNavBarLocalizedDescription("NavBarItemCommand"),
#endif
		Bindable(true), Localizability(LocalizationCategory.NeverLocalize)]
		public ICommand Command {
			get { return (ICommand)GetValue(CommandProperty); }
			set { SetValue(CommandProperty, value); }
		}
		[
#if !SL
	DevExpressXpfNavBarLocalizedDescription("NavBarItemCommandParameter"),
#endif
		Bindable(true), Localizability(LocalizationCategory.NeverLocalize)]
		public object CommandParameter {
			get { return (object)GetValue(CommandParameterProperty); }
			set { SetValue(CommandParameterProperty, value); }
		}
		[
#if !SL
	DevExpressXpfNavBarLocalizedDescription("NavBarItemCommandTarget"),
#endif
		Bindable(true)]
		public IInputElement CommandTarget {
			get { return (IInputElement)GetValue(CommandTargetProperty); }
			set { SetValue(CommandTargetProperty, value); }
		}
		public FontSettings FontSettings {
			get { return (FontSettings)GetValue(FontSettingsProperty); }
			set { SetValue(FontSettingsProperty, value); }
		}
		public ImageSettings ImageSettings {
			get { return (ImageSettings)GetValue(ImageSettingsProperty); }
			set { SetValue(ImageSettingsProperty, value); }
		}
		public LayoutSettings LayoutSettings {
			get { return (LayoutSettings)GetValue(LayoutSettingsProperty); }
			set { SetValue(LayoutSettingsProperty, value); }
		}
		public DisplayMode DisplayMode {
			get { return (DisplayMode)GetValue(DisplayModeProperty); }
			set { SetValue(DisplayModeProperty, value); }
		}
		public ImageSettings ActualImageSettings {
			get { return (ImageSettings)GetValue(ActualImageSettingsProperty); }
			protected internal set { this.SetValue(ActualImageSettingsPropertyKey, value); }
		}
		public LayoutSettings ActualLayoutSettings {
			get { return (LayoutSettings)GetValue(ActualLayoutSettingsProperty); }
			protected internal set { this.SetValue(ActualLayoutSettingsPropertyKey, value); }
		}
		public DisplayMode ActualDisplayMode {
			get { return (DisplayMode)GetValue(ActualDisplayModeProperty); }
			protected internal set { this.SetValue(ActualDisplayModePropertyKey, value); }
		}
		public FontSettings ActualFontSettings {
			get { return (FontSettings)GetValue(ActualFontSettingsProperty); }
			protected internal set { this.SetValue(ActualFontSettingsPropertyKey, value); }
		}
		public event EventHandler Click;
		public event EventHandler Select;		
		EventHandler onCanExecuteChangedEventHandler;				
		protected virtual void OnCommandChanged(ICommand oldCommand, ICommand newCommand) {
			if(oldCommand != null) {				
				oldCommand.CanExecuteChanged -= onCanExecuteChangedEventHandler;
				onCanExecuteChangedEventHandler = null;				
			}
			if(newCommand != null) {
				onCanExecuteChangedEventHandler = OnCanExecuteChanged;
				newCommand.CanExecuteChanged += onCanExecuteChangedEventHandler;			 
			}
			UpdateCanExecute();			
		}
		void OnCanExecuteChanged(object sender, EventArgs e) {
			UpdateCanExecute();
		}
		void UpdateCanExecute() {
			if(Command == null) return;
			IInputElement target = CommandTarget ?? (Group == null ? null : Group.NavBar);
			IsEnabled = Command is RoutedCommand ? ((RoutedCommand)Command).CanExecute(CommandParameter, target) : Command.CanExecute(CommandParameter);
		}
		void OnActualVisualStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
		}
		internal void UpdateActualVisualStyle() {
			if(VisualStyle != null) {
				ActualVisualStyle = VisualStyle;
				return;
			}
			if(Group != null && Group.ItemVisualStyle != null) {
				ActualVisualStyle = Group.ItemVisualStyle;
				return;
			}
			if(Group != null && Group.NavBar != null && Group.NavBar.View != null) {
				NavigationPaneView navPaneView = Group.NavBar.View as NavigationPaneView;
				if(navPaneView != null && navPaneView.IsPopupOpened) {
					ActualVisualStyle = navPaneView.ItemVisualStyleInPopup;
				} else ActualVisualStyle = Group.NavBar.View.ItemVisualStyle;
				return;
			}
			ActualVisualStyle = VisualStyle;
		}
		protected internal virtual void UpdateActualDisplayMode() {
			if(DisplayMode != DisplayMode.Default) {
				ActualDisplayMode = DisplayMode;
				return;
			}
			if(Group == null) {
				ActualDisplayMode = DisplayMode.ImageAndText;
				return;
			}
			if(Group.ItemDisplayMode != DisplayMode.Default) {
				ActualDisplayMode = Group.ItemDisplayMode;
				return;
			}
			if(Group.NavBar == null || Group.NavBar.View == null || Group.NavBar.View.ItemDisplayMode == NavBar.DisplayMode.Default) {
				ActualDisplayMode = DisplayMode.ImageAndText;
				return;
			}
			ActualDisplayMode = Group.NavBar.View.ItemDisplayMode;
		}
		protected internal virtual void UpdateActualLayoutSettings() {
			if(LayoutSettings != null) {
				ActualLayoutSettings = LayoutSettings;
				return;
			}
			if(Group == null) {
				ActualLayoutSettings = LayoutSettings.Default;
				return;
			}
			if(Group.ItemLayoutSettings != null) {
				ActualLayoutSettings = Group.ItemLayoutSettings;
				return;
			}
			if(Group.NavBar == null || Group.NavBar.View == null || Group.NavBar.View.ItemLayoutSettings == null) {
				ActualLayoutSettings = LayoutSettings.Default;
				return;
			}
			ActualLayoutSettings = Group.NavBar.View.ItemLayoutSettings;
		}
		protected internal virtual void UpdateActualImageSettings() {
			if(ImageSettings != null) {
				ActualImageSettings = ImageSettings;
				return;
			}
			if(Group == null) {
				ActualImageSettings = ImageSettings.ItemDefault;
				return;
			}
			if(Group.ItemImageSettings != null) {
				ActualImageSettings = Group.ItemImageSettings;
				return;
			}
			if(Group.NavBar == null || Group.NavBar.View == null || Group.NavBar.View.ItemImageSettings == null) {
				ActualImageSettings = ImageSettings.ItemDefault;
				return;
			}
			ActualImageSettings = Group.NavBar.View.ItemImageSettings;
		}
		protected internal virtual void UpdateActualFontSettings() {
			if(FontSettings != null) {
				ActualFontSettings = FontSettings;
				return;
			}
			if(Group == null) {
				ActualFontSettings = FontSettings.Default;
				return;
			}
			if(Group.ItemFontSettings != null) {
				ActualFontSettings = Group.ItemFontSettings;
				return;
			}
			if(Group.NavBar == null || Group.NavBar.View == null || Group.NavBar.View.ItemFontSettings == null) {
				ActualFontSettings = FontSettings.Default;
				return;
			}
			ActualFontSettings = Group.NavBar.View.ItemFontSettings;
		}
		internal void RaiseClickEvent() {
			if(Group != null && Group.NavBar != null && Group.NavBar.AllowSelectItem &&
				(this.IsEnabled ? true : Group.NavBar.AllowSelectDisabledItem)) {
				Group.SelectedItem = this;
			}
			if(IsEnabled) {
				if(Click != null)
					Click(this, new EventArgs());
				ExecuteCommand();
			}
		}
		internal void RaiseSelectEvent() {			
			if(Select != null)
				Select(this, new EventArgs());
		}
		private void ExecuteCommand() {
			if(Command != null) {
				RoutedCommand routedCommand = Command as RoutedCommand;
				if(routedCommand != null)
					routedCommand.Execute(CommandParameter, CommandTarget);
				else Command.Execute(CommandParameter);
			}
		}
	}
	public partial class ImageAndTextContentPresenter : XPFContentPresenter {
		public static readonly DependencyProperty OrientationProperty;
		public static readonly DependencyProperty ContextElementProperty;		
		static ImageAndTextContentPresenter() {
			OrientationProperty = DependencyPropertyManager.Register("Orientation", typeof(Orientation), typeof(ImageAndTextContentPresenter),
				new FrameworkPropertyMetadata(Orientation.Horizontal, FrameworkPropertyMetadataOptions.AffectsMeasure));
			ContextElementProperty = DependencyPropertyManager.Register("ContextElement", typeof(object), typeof(ImageAndTextContentPresenter), new FrameworkPropertyMetadata(null));
		}
		public ImageAndTextContentPresenter() {
		}
		public Orientation Orientation {
			get { return (Orientation)GetValue(OrientationProperty); }
			set { SetValue(OrientationProperty, value); }
		}
		public object ContextElement {
			get { return (object)GetValue(ContextElementProperty); }
			set { SetValue(ContextElementProperty, value); }
		}
		public double ImageFallbackSize { get; set; }		
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			SetLayoutSettingsBinding();
			SetImageSettingsBinding();
		}				
		protected virtual void UpdateTransform(Size size) {
			UIElement child = GetChild();
			if (child == null)
				return;
			if (Orientation == Orientation.Horizontal) {
				RenderTransform = new RotateTransform();
			} else {
				var transform = new TransformGroup();
				transform.Children.Add(new RotateTransform() { Angle = -90 });
				transform.Children.Add(new TranslateTransform() { Y = size.Width });
				RenderTransform = transform;
			}
			if(child is ImageAndTextDecorator)
				(child as ImageAndTextDecorator).UseAliasedEdgeMode = Orientation == Orientation.Vertical;
			RenderOptions.SetBitmapScalingMode(child, Orientation == Orientation.Vertical ? BitmapScalingMode.NearestNeighbor : BitmapScalingMode.Unspecified);
		}
		protected override Size MeasureOverride(Size availableSize) {
			var child = GetChild();
			if (child == null) 
				return Size.Empty;
			child.Measure(GetCorrectSize(availableSize));
			return GetCorrectSize(child.DesiredSize);
		}
		protected override Size ArrangeOverride(Size finalSize) {
			var child = GetChild();
			if (child == null)
				return Size.Empty;
			Size arrangeSize = GetCorrectSize(finalSize);
			child.Arrange(new Rect(0, 0, arrangeSize.Width, arrangeSize.Height));
			UpdateTransform(arrangeSize);
			return GetCorrectSize(arrangeSize);
		}
		Size GetCorrectSize(Size size) {
			return Orientation == Orientation.Horizontal ?
				size : new Size(size.Height, size.Width);
		}
		UIElement GetChild() {
			if (VisualTreeHelper.GetChildrenCount(this) == 0)
				return null;
			return VisualTreeHelper.GetChild(this, 0) as UIElement;
		}		
	}
	public partial class ImageAndTextDecorator : Control {
		#region Dependency properties
		public static readonly DependencyProperty DisplayModeProperty =
			DependencyPropertyManager.Register("DisplayMode", typeof(DisplayMode), typeof(ImageAndTextDecorator), new PropertyMetadata((d, e) => ((ImageAndTextDecorator)d).OnDisplayModeChanged()));
		public static readonly DependencyProperty ImageDockingProperty =
			DependencyPropertyManager.Register("ImageDocking", typeof(Dock), typeof(ImageAndTextDecorator), new PropertyMetadata(Dock.Left, (d, e) => ((ImageAndTextDecorator)d).OnImageDockingChanged()));
		public static readonly DependencyProperty ContentSourceProperty =
			DependencyPropertyManager.Register("ContentSource", typeof(object), typeof(ImageAndTextDecorator), new PropertyMetadata(null));
		public static readonly DependencyProperty ContentStyleProperty =
			DependencyPropertyManager.Register("ContentStyle", typeof(Style), typeof(ImageAndTextDecorator), new PropertyMetadata(null));
		public static readonly DependencyProperty ImageSourceProperty =
			DependencyPropertyManager.Register("ImageSource", typeof(ImageSource), typeof(ImageAndTextDecorator), new PropertyMetadata(null));
		public static readonly DependencyProperty ImageStyleProperty =
			DependencyPropertyManager.Register("ImageStyle", typeof(Style), typeof(ImageAndTextDecorator), new PropertyMetadata(null));
		public static readonly DependencyProperty ImageFallbackSizeProperty =
			DependencyPropertyManager.Register("ImageFallbackSize", typeof(double), typeof(ImageAndTextDecorator), new PropertyMetadata(0.0));
		public static readonly DependencyProperty DisplayModeTextMarginProperty =
			DependencyPropertyManager.Register("DisplayModeTextMargin", typeof(Thickness), typeof(ImageAndTextDecorator), new FrameworkPropertyMetadata(new Thickness()));
		public static readonly DependencyProperty DisplayModeImageMarginProperty =
			DependencyPropertyManager.Register("DisplayModeImageMargin", typeof(Thickness), typeof(ImageAndTextDecorator), new FrameworkPropertyMetadata(new Thickness()));
		public static readonly DependencyProperty DockingTopContentMarginProperty =
			DependencyPropertyManager.Register("DockingTopContentMargin", typeof(Thickness), typeof(ImageAndTextDecorator), new FrameworkPropertyMetadata(new Thickness()));
		public static readonly DependencyProperty DockingTopImageMarginProperty =
			DependencyPropertyManager.Register("DockingTopImageMargin", typeof(Thickness), typeof(ImageAndTextDecorator), new FrameworkPropertyMetadata(new Thickness()));
		public static readonly DependencyProperty DockingBottomContentMarginProperty =
			DependencyPropertyManager.Register("DockingBottomContentMargin", typeof(Thickness), typeof(ImageAndTextDecorator), new FrameworkPropertyMetadata(new Thickness()));
		public static readonly DependencyProperty DockingBottomImageMarginProperty =
			DependencyPropertyManager.Register("DockingBottomImageMargin", typeof(Thickness), typeof(ImageAndTextDecorator), new FrameworkPropertyMetadata(new Thickness()));
		public static readonly DependencyProperty DockingRightContentMarginProperty =
			DependencyPropertyManager.Register("DockingRightContentMargin", typeof(Thickness), typeof(ImageAndTextDecorator), new FrameworkPropertyMetadata(new Thickness()));
		public static readonly DependencyProperty DockingRightImageMarginProperty =
			DependencyPropertyManager.Register("DockingRightImageMargin", typeof(Thickness), typeof(ImageAndTextDecorator), new FrameworkPropertyMetadata(new Thickness()));
		public static readonly DependencyProperty DockingLeftContentMarginProperty =
			DependencyPropertyManager.Register("DockingLeftContentMargin", typeof(Thickness), typeof(ImageAndTextDecorator), new FrameworkPropertyMetadata(new Thickness()));
		public static readonly DependencyProperty DockingLeftImageMarginProperty =
			DependencyPropertyManager.Register("DockingLeftImageMargin", typeof(Thickness), typeof(ImageAndTextDecorator), new FrameworkPropertyMetadata(new Thickness()));
		#endregion
		public ImageAndTextDecorator() {
			this.SetDefaultStyleKey(typeof(ImageAndTextDecorator));			
		}
		#region Properties
		public Grid Grid { get; set; }
		public Thickness DockingLeftImageMargin {
			get { return (Thickness)GetValue(DockingLeftImageMarginProperty); }
			set { SetValue(DockingLeftImageMarginProperty, value); }
		}
		public Thickness DockingLeftContentMargin {
			get { return (Thickness)GetValue(DockingLeftContentMarginProperty); }
			set { SetValue(DockingLeftContentMarginProperty, value); }
		}
		public Thickness DockingRightImageMargin {
			get { return (Thickness)GetValue(DockingRightImageMarginProperty); }
			set { SetValue(DockingRightImageMarginProperty, value); }
		}
		public Thickness DockingRightContentMargin {
			get { return (Thickness)GetValue(DockingRightContentMarginProperty); }
			set { SetValue(DockingRightContentMarginProperty, value); }
		}
		public Thickness DockingBottomImageMargin {
			get { return (Thickness)GetValue(DockingBottomImageMarginProperty); }
			set { SetValue(DockingBottomImageMarginProperty, value); }
		}
		public Thickness DockingBottomContentMargin {
			get { return (Thickness)GetValue(DockingBottomContentMarginProperty); }
			set { SetValue(DockingBottomContentMarginProperty, value); }
		}
		public Thickness DockingTopImageMargin {
			get { return (Thickness)GetValue(DockingTopImageMarginProperty); }
			set { SetValue(DockingTopImageMarginProperty, value); }
		}
		public Thickness DockingTopContentMargin {
			get { return (Thickness)GetValue(DockingTopContentMarginProperty); }
			set { SetValue(DockingTopContentMarginProperty, value); }
		}
		public Thickness DisplayModeImageMargin {
			get { return (Thickness)GetValue(DisplayModeImageMarginProperty); }
			set { SetValue(DisplayModeImageMarginProperty, value); }
		}
		public Thickness DisplayModeTextMargin {
			get { return (Thickness)GetValue(DisplayModeTextMarginProperty); }
			set { SetValue(DisplayModeTextMarginProperty, value); }
		}	  
		public object ContentSource {
			get { return (object)GetValue(ContentSourceProperty); }
			set { SetValue(ContentSourceProperty, value); }
		}
		public Style ContentStyle {
			get { return (Style)GetValue(ContentStyleProperty); }
			set { SetValue(ContentStyleProperty, value); }
		}
		public DisplayMode DisplayMode {
			get { return (DisplayMode)GetValue(DisplayModeProperty); }
			set { SetValue(DisplayModeProperty, value); }
		}
		public Dock ImageDocking {
			get { return (Dock)GetValue(ImageDockingProperty); }
			set { SetValue(ImageDockingProperty, value); }
		}
		public double ImageFallbackSize {
			get { return (double)GetValue(ImageFallbackSizeProperty); }
			set { SetValue(ImageFallbackSizeProperty, value); }
		}
		public ImageSource ImageSource {
			get { return (ImageSource)GetValue(ImageSourceProperty); }
			set { SetValue(ImageSourceProperty, value); }
		}
		public Style ImageStyle {
			get { return (Style)GetValue(ImageStyleProperty); }
			set { SetValue(ImageStyleProperty, value); }
		}
		bool useAliasedEdgeMode;
		internal bool UseAliasedEdgeMode {
			get { return useAliasedEdgeMode; }
			set {
				useAliasedEdgeMode = value;
				UpdateImageTransform();
			}
		}
		internal Image Image { get; set; }
		internal FrameworkElement Content { get; set; }
		#endregion
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			SetPropertiesBinding();
			Initialize();
			UpdateContentByDisplayMode();
			UpdateImageTransform();
		}
		void UpdateImageTransform() {
			if(Image == null || Image.Visibility == Visibility.Collapsed)
				return;
			Image.SnapsToDevicePixels = !UseAliasedEdgeMode;
			RenderOptions.SetEdgeMode(Image, UseAliasedEdgeMode ? EdgeMode.Aliased : EdgeMode.Unspecified);
		}
		protected internal virtual void Initialize() {
			Grid = this.GetTemplateChild("grid") as Grid;
			Image = this.GetTemplateChild("PART_Image") as Image;
			Content = this.GetTemplateChild("PART_Content") as FrameworkElement;
			SetBindings();
		}
		protected internal virtual void UpdateContentByDisplayMode() {
			switch (DisplayMode) {
				case DisplayMode.ImageAndText:
					if (Image.Source != null) {
						Image.Visibility = Visibility.Visible;
					}
					Content.Visibility = Visibility.Visible;
					break;
				case DisplayMode.Image:
					if (Image.Source != null) {
						Image.Visibility = Visibility.Visible;
						Grid.SetColumnSpan(Image, 2);
					}
					Content.Visibility = Visibility.Collapsed;
					Image.Margin = DisplayModeImageMargin;
					return;
				case DisplayMode.Text:
					Content.Visibility = Visibility.Visible;
					Image.Visibility = Visibility.Collapsed;
					Content.Margin = DisplayModeTextMargin;
					Grid.SetColumnSpan(Content, 2);
					return;
				default:
					break;
			}
			UpdateImageDocking();
		}
		protected internal virtual void UpdateImageDocking() {
			if (Grid == null)
				return;
			Grid.ColumnDefinitions[0].Width = new GridLength(1, GridUnitType.Auto);
			Grid.ColumnDefinitions[1].Width = new GridLength(1, GridUnitType.Star);
			ClearValue();
			switch (ImageDocking) {
				case Dock.Bottom:
					if (Image != null) {
						Grid.SetRow(Image, 1);
						Grid.SetColumnSpan(Image, 2);
						Image.Margin = DockingBottomImageMargin;
					}
					if (Content != null) {
						Grid.SetColumn(Content, 0);
						Grid.SetRow(Content, 0);
						Grid.SetColumnSpan(Content, 2);
						Content.Margin = DockingBottomContentMargin;
					}
					break;
				case Dock.Left:
					if (Image != null) {
						Grid.SetColumn(Image, 0);
						Image.Margin = DockingLeftImageMargin;
					}
					if (Content != null) {
						Grid.SetColumn(Content, 1);
						Content.Margin = DockingLeftContentMargin;
					}
					Grid.ColumnDefinitions[0].Width = new GridLength(1, GridUnitType.Auto);
					Grid.ColumnDefinitions[1].Width = new GridLength(1, GridUnitType.Star);
					break;
				case Dock.Right:
					Grid.ColumnDefinitions[0].Width = new GridLength(1, GridUnitType.Star);
					Grid.ColumnDefinitions[1].Width = new GridLength(1, GridUnitType.Auto);
					if (Image != null) {
						Grid.SetColumn(Image, 1);
						Image.Margin = DockingRightImageMargin;
					}
					if (Content != null) {
						Grid.SetColumn(Content, 0);
						Content.Margin = DockingRightContentMargin;
					}
					break;
				case Dock.Top:
					if (Image != null) {
						Grid.SetRow(Image, 0);
						Grid.SetColumn(Image, 0);
						Grid.SetColumnSpan(Image, 2);
						Image.Margin = DockingTopImageMargin;
					}
					if (Content != null) {
						Grid.SetColumn(Content, 0);
						Grid.SetRow(Content, 1);
						Grid.SetColumnSpan(Content, 2);
						Content.Margin = DockingTopContentMargin;
					}
					break;
			}
		}
		void ClearValue() {
			if (Image != null) {
				Image.ClearValue(Grid.RowProperty);
				Image.ClearValue(Grid.RowSpanProperty);
				Image.ClearValue(Grid.ColumnProperty);
				Image.ClearValue(Grid.ColumnSpanProperty);
			}
			if (Content != null) {
				Content.ClearValue(Grid.RowProperty);
				Content.ClearValue(Grid.RowSpanProperty);
				Content.ClearValue(Grid.ColumnProperty);
				Content.ClearValue(Grid.ColumnSpanProperty);
			}
		}
		void OnDisplayModeChanged() {
			UpdateContentByDisplayMode();
		}
		void OnImageDockingChanged() {
			UpdateImageDocking();
		}
		void SetPropertiesBinding() {
			Image image = this.GetElementByName("PART_Image") as Image;
			Control content = this.GetElementByName("PART_Content") as Control;
			SettingsProvider.SetPropertiesBinding(image, ImageFallbackSize, content, this.GetTemplatedParent());
		}
	}
	public class NavPaneImageAndTextDecorator : ImageAndTextDecorator {
		public static readonly DependencyProperty ContentOpacityProperty;
		static NavPaneImageAndTextDecorator() {
			ContentOpacityProperty = DependencyPropertyManager.Register("ContentOpacity", typeof(double), typeof(NavPaneImageAndTextDecorator), new PropertyMetadata((d, e) => ((NavPaneImageAndTextDecorator)d).OnContentOpacityChanged()));
		}
		public NavPaneImageAndTextDecorator() {
			Loaded += OnLoaded;
		}
		public Thickness GroupButtonCollapsedImageMargin { get; set; }
		public double ContentOpacity {
			get { return (double)GetValue(ContentOpacityProperty); }
			set { SetValue(ContentOpacityProperty, value); }
		}
		protected internal override void Initialize() {
			base.Initialize();
			ContentControl cc = this.GetElementByName("PART_Content") as ContentControl;
			if (cc == null)
				return;
			SetBinding(ContentOpacityProperty, new Binding("Opacity") { Source = cc });
			cc.SetBinding(ContentControl.OpacityProperty, new Binding("(0)") { Path = new PropertyPath(NavBarAnimationOptions.AnimationProgressProperty) });
		}
		protected internal override void UpdateContentByDisplayMode() {
			if (ContentOpacity != 0)
				base.UpdateContentByDisplayMode();
		}
		protected internal override void UpdateImageDocking() {
			if (ContentOpacity != 0)
				base.UpdateImageDocking();
		}
		void OnLoaded(object sender, RoutedEventArgs e) {
			UpdateContentLayout();
		}
		void OnContentOpacityChanged() {
			UpdateContentLayout();
		}
		void UpdateContentLayout() {
			if (ContentOpacity == 0) {
				if (Image == null)
					return;
				Grid.SetColumnSpan(Image, 2);
				Grid.SetColumn(Image, 0);
				Image.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
				Image.Margin = GroupButtonCollapsedImageMargin;
			} else
				UpdateContentByDisplayMode();
		}
	}
	public class ImageSourceToVisibilityConverter :IValueConverter {
		#region IValueConverter Members
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			if (parameter != null && (DisplayMode)parameter == DisplayMode.Text)
				return Visibility.Collapsed;
			return value == null ? Visibility.Collapsed : Visibility.Visible;
		}
		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			throw new NotImplementedException();
		}
		#endregion
	}
}
