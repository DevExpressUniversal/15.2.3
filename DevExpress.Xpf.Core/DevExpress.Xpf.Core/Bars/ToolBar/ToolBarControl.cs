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
using DevExpress.Mvvm.UI;
using DevExpress.Utils;
using DevExpress.Xpf.Bars.Native;
using DevExpress.Xpf.Bars.Themes;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Markup;
namespace DevExpress.Xpf.Bars {
	[ContentProperty("Items")]
	public abstract class ToolBarControlBase : Control, IBar, IBarLayoutTableInfo {
		const string BarControlName = "PART_BarControl";
		public static readonly DependencyProperty ItemTemplateSelectorProperty;
		public static readonly DependencyProperty ItemTemplateProperty;
		public static readonly DependencyProperty ItemStyleProperty;
		public static readonly DependencyProperty ItemsSourceProperty;
		protected static readonly DependencyPropertyKey ItemsPropertyKey;
		public static readonly DependencyProperty ItemsProperty;		
		public static readonly DependencyProperty BarItemDisplayModeProperty;
		public static readonly DependencyProperty AllowCustomizationMenuProperty;
		public static readonly DependencyProperty AllowHideProperty;		
		public static readonly DependencyProperty AllowRenameProperty;
		public static readonly DependencyProperty BarItemHorizontalIndentProperty;
		public static readonly DependencyProperty BarItemsAlignmentProperty;
		public static readonly DependencyProperty BarItemVerticalIndentProperty;
		public static readonly DependencyProperty CaptionProperty;
		public static readonly DependencyProperty GlyphSizeProperty;		
		public static readonly DependencyProperty IsMultiLineProperty;		
		public static readonly DependencyProperty BackgroundTemplateProperty;
		public static readonly DependencyProperty ShowBackgroundProperty;
		protected static readonly DependencyPropertyKey IsStandalonePropertyKey;
		public static readonly DependencyProperty IsStandaloneProperty;
		public static readonly DependencyProperty HideWhenEmptyProperty;
		public static readonly DependencyProperty BorderTemplateProperty;
		public ControlTemplate BorderTemplate {
			get { return (ControlTemplate)GetValue(BorderTemplateProperty); }
			set { SetValue(BorderTemplateProperty, value); }
		}
		public bool HideWhenEmpty {
			get { return (bool)GetValue(HideWhenEmptyProperty); }
			set { SetValue(HideWhenEmptyProperty, value); }
		}	   
		public BarItemDisplayMode BarItemDisplayMode {
			get { return (BarItemDisplayMode)GetValue(BarItemDisplayModeProperty); }
			set { SetValue(BarItemDisplayModeProperty, value); }
		}
		public bool AllowCustomizationMenu {
			get { return (bool)GetValue(AllowCustomizationMenuProperty); }
			set { SetValue(AllowCustomizationMenuProperty, value); }
		}
		public bool AllowHide {
			get { return (bool)GetValue(AllowHideProperty); }
			set { SetValue(AllowHideProperty, value); }
		}
		public bool AllowRename {
			get { return (bool)GetValue(AllowRenameProperty); }
			set { SetValue(AllowRenameProperty, value); }
		}
		public double BarItemHorizontalIndent {
			get { return (double)GetValue(BarItemHorizontalIndentProperty); }
			set { SetValue(BarItemHorizontalIndentProperty, value); }
		}
		public BarItemAlignment BarItemsAlignment {
			get { return (BarItemAlignment)GetValue(BarItemsAlignmentProperty); }
			set { SetValue(BarItemsAlignmentProperty, value); }
		}
		public double BarItemVerticalIndent {
			get { return (double)GetValue(BarItemVerticalIndentProperty); }
			set { SetValue(BarItemVerticalIndentProperty, value); }
		}
		public string Caption {
			get { return (string)GetValue(CaptionProperty); }
			set { SetValue(CaptionProperty, value); }
		}
		public GlyphSize GlyphSize {
			get { return (GlyphSize)GetValue(GlyphSizeProperty); }
			set { SetValue(GlyphSizeProperty, value); }
		}
		public bool IsMultiLine {
			get { return (bool)GetValue(IsMultiLineProperty); }
			set { SetValue(IsMultiLineProperty, value); }
		}						
		public bool ShowBackground {
			get { return (bool)GetValue(ShowBackgroundProperty); }
			set { SetValue(ShowBackgroundProperty, value); }
		}
		public ControlTemplate BackgroundTemplate {
			get { return (ControlTemplate)GetValue(BackgroundTemplateProperty); }
			set { SetValue(BackgroundTemplateProperty, value); }
		}		
		public bool IsStandalone {
			get { return (bool)GetValue(IsStandaloneProperty); }
			protected internal set { this.SetValue(IsStandalonePropertyKey, value); }
		}
		static ToolBarControlBase() {
			DefaultStyleKeyProperty.OverrideMetadata(typeof(ToolBarControlBase), new FrameworkPropertyMetadata(typeof(ToolBarControlBase)));
			BorderTemplateProperty = DependencyPropertyManager.Register("BorderTemplate", typeof(ControlTemplate), typeof(ToolBarControlBase), new FrameworkPropertyMetadata(null));
			BarItemDisplayModeProperty = DependencyPropertyManager.Register("BarItemDisplayMode", typeof(BarItemDisplayMode), typeof(ToolBarControlBase), new FrameworkPropertyMetadata(BarItemDisplayMode.Default));			
			IsStandalonePropertyKey = DependencyPropertyManager.RegisterReadOnly("IsStandalone", typeof(bool), typeof(ToolBarControlBase), new FrameworkPropertyMetadata(true, (d, e) => ((ToolBarControlBase)d).OnIsStandaloneChanged((bool)e.OldValue)));
			HideWhenEmptyProperty = DependencyPropertyManager.Register("HideWhenEmpty", typeof(bool), typeof(ToolBarControlBase), new FrameworkPropertyMetadata(false));
			AllowCustomizationMenuProperty = DependencyPropertyManager.Register("AllowCustomizationMenu", typeof(bool), typeof(ToolBarControlBase), new FrameworkPropertyMetadata(true));
			AllowHideProperty = DependencyPropertyManager.Register("AllowHide", typeof(bool), typeof(ToolBarControlBase), new FrameworkPropertyMetadata(true));			
			AllowRenameProperty = DependencyPropertyManager.Register("AllowRename", typeof(bool), typeof(ToolBarControlBase), new FrameworkPropertyMetadata(true));
			BarItemHorizontalIndentProperty = DependencyPropertyManager.Register("BarItemHorizontalIndent", typeof(double), typeof(ToolBarControlBase), new FrameworkPropertyMetadata(0d));
			BarItemsAlignmentProperty = DependencyPropertyManager.Register("BarItemsAlignment", typeof(BarItemAlignment), typeof(ToolBarControlBase), new FrameworkPropertyMetadata(BarItemAlignment.Default));
			BarItemVerticalIndentProperty = DependencyPropertyManager.Register("BarItemVerticalIndent", typeof(double), typeof(ToolBarControlBase), new FrameworkPropertyMetadata(0d));
			CaptionProperty = DependencyPropertyManager.Register("Caption", typeof(string), typeof(ToolBarControlBase), new FrameworkPropertyMetadata(String.Empty));
			GlyphSizeProperty = DependencyPropertyManager.Register("GlyphSize", typeof(GlyphSize), typeof(ToolBarControlBase), new FrameworkPropertyMetadata(GlyphSize.Default));			
			IsMultiLineProperty = DependencyPropertyManager.Register("IsMultiLine", typeof(bool), typeof(ToolBarControlBase), new FrameworkPropertyMetadata(false));						
			BackgroundTemplateProperty = DependencyPropertyManager.Register("BackgroundTemplate", typeof(ControlTemplate), typeof(ToolBarControlBase), new FrameworkPropertyMetadata(null, OnBackgroundTemplatePropertyChanged));
			ShowBackgroundProperty = DependencyPropertyManager.Register("ShowBackground", typeof(bool), typeof(ToolBarControlBase), new FrameworkPropertyMetadata(true, (d, e) => ((ToolBarControlBase)d).OnShowBackgroundChanged((bool)e.OldValue)));
			IsStandaloneProperty = IsStandalonePropertyKey.DependencyProperty;			
			ItemTemplateSelectorProperty = DependencyPropertyManager.Register("ItemTemplateSelector", typeof(DataTemplateSelector), typeof(ToolBarControlBase), new FrameworkPropertyMetadata(null, new PropertyChangedCallback((d, e) => ((ToolBarControlBase)d).OnItemTemplateSelectorChanged((DataTemplateSelector)e.OldValue))));
			ItemTemplateProperty = DependencyPropertyManager.Register("ItemTemplate", typeof(DataTemplate), typeof(ToolBarControlBase), new FrameworkPropertyMetadata(null, new PropertyChangedCallback((d, e) => ((ToolBarControlBase)d).OnItemTemplateChanged((DataTemplate)e.OldValue))));
			ItemStyleProperty = DependencyPropertyManager.Register("ItemStyle", typeof(Style), typeof(ToolBarControlBase), new FrameworkPropertyMetadata(null, new PropertyChangedCallback((d, e) => ((ToolBarControlBase)d).OnItemStyleChanged((Style)e.OldValue))));
			ItemsSourceProperty = DependencyPropertyManager.Register("ItemsSource", typeof(object), typeof(ToolBarControlBase), new FrameworkPropertyMetadata(null, new PropertyChangedCallback((d, e) => ((ToolBarControlBase)d).OnItemsSourceChanged((object)e.OldValue))));
			ItemsPropertyKey = DependencyPropertyManager.RegisterReadOnly("Items", typeof(ObservableCollection<IBarItem>), typeof(ToolBarControlBase), new FrameworkPropertyMetadata(null));
			HorizontalAlignmentProperty.OverrideMetadata(typeof(ToolBarControlBase), new FrameworkPropertyMetadata(HorizontalAlignment.Stretch, null, new CoerceValueCallback((d, e) => ((ToolBarControlBase)d).CoerceHorizontalAlignment((HorizontalAlignment)e))));
			VerticalAlignmentProperty.OverrideMetadata(typeof(ToolBarControlBase), new FrameworkPropertyMetadata(VerticalAlignment.Stretch, null, new CoerceValueCallback((d, e) => ((ToolBarControlBase)d).CoerceVerticalAlignment((VerticalAlignment)e))));
			VisibilityProperty.OverrideMetadata(typeof(ToolBarControlBase), new FrameworkPropertyMetadata(Visibility.Visible, null, new CoerceValueCallback((d, e) => ((ToolBarControlBase)d).CoerceVisibility((Visibility)e))));
			ItemsProperty = ItemsPropertyKey.DependencyProperty;			
		}
		static void OnBackgroundTemplatePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((ToolBarControlBase)d).OnBackgroundTemplateChanged(e.OldValue);
		}
		void OnBackgroundTemplateChanged(object oldValue) {
		}
		object CreateEmptyBackgroundTemplate() {
			const string xaml = "<ControlTemplate xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\" xmlns:x=\"http://schemas.microsoft.com/winfx/2006/xaml\" TargetType=\"{x:Type ContentControl}\"><ContentPresenter/></ControlTemplate>";
			var controlTemplate = XamlReader.Parse(xaml);
			return controlTemplate;
		}		
		Bar bar;
		Visibility? forcedVisibility;
		protected internal Bar Bar { get { return bar; } }		
		protected internal Visibility? ForcedVisibility {
			get { return forcedVisibility; }
			set {
				if (value == forcedVisibility) return;
				forcedVisibility = value;
				CoerceValue(VisibilityProperty);
			}
		}
		protected virtual Visibility CoerceVisibility(Visibility e) {
			return ForcedVisibility ?? e;
		}
		public ToolBarControlBase() {
			this.bar = CreateBar();
			Bar.ToolBar = this;
			((IBar)this).ShowInOriginContainer = true;
			CreateBindings();
			this.SetValue(ItemsPropertyKey, new ObservableCollection<IBarItem>());
			Items.CollectionChanged += OnItemsCollectionChanged;
			AddLogicalChild(bar);
		}
		List<Tuple<DependencyProperty, DependencyProperty, IValueConverter, BindingMode>> props;
		void CreateBindings() {
			props = new List<Tuple<DependencyProperty, DependencyProperty, IValueConverter, BindingMode>>() {				
				Pair(AllowCustomizationMenuProperty, Bar.AllowCustomizationMenuProperty	 ),
				Pair(AllowHideProperty,			  Bar.AllowHideProperty				  , converter: new DefaultBooleanToBooleanConverter()),				
				Pair(AllowRenameProperty,			Bar.AllowRenameProperty				),
				Pair(BarItemHorizontalIndentProperty,Bar.BarItemHorzIndentProperty		  ),
				Pair(BarItemsAlignmentProperty,	  Bar.BarItemsAlignmentProperty		  ),
				Pair(BarItemVerticalIndentProperty,  Bar.BarItemVertIndentProperty		  ),
				Pair(CaptionProperty,				Bar.CaptionProperty					),
				Pair(GlyphSizeProperty,			  Bar.GlyphSizeProperty				  ),				
				Pair(IsMultiLineProperty,			Bar.IsMultiLineProperty				),								
				Pair(BarItemDisplayModeProperty,	 Bar.BarItemDisplayModeProperty		 ),
				Pair(MergingProperties.NameProperty, MergingProperties.NameProperty		 ),
				Pair(HideWhenEmptyProperty,		  Bar.HideWhenEmptyProperty			  ),
			};
			CreateCustomBindings();
			foreach (var pair in props) {
				BindingOperations.SetBinding(bar, pair.Item2, new Binding() { Source = this, Converter = pair.Item3, Path = new PropertyPath(pair.Item1), Mode = pair.Item4 });
			}
			props = null;
		}
		protected virtual object CoerceHorizontalAlignment(HorizontalAlignment e) { return IsStandalone ? e : HorizontalAlignment.Stretch; }
		protected virtual object CoerceVerticalAlignment(VerticalAlignment e) { return IsStandalone ? e : VerticalAlignment.Stretch; }
		protected abstract void CreateCustomBindings();
		protected void AddCustomBinding(DependencyProperty first, DependencyProperty second, IValueConverter converter = null, BindingMode mode = BindingMode.TwoWay) {
			if (props == null)
				throw new InvalidOperationException();
			props.Add(Pair(first, second, converter, mode));
		}
		Tuple<DependencyProperty, DependencyProperty, IValueConverter, BindingMode> Pair(DependencyProperty first, DependencyProperty second, IValueConverter converter = null, BindingMode mode = BindingMode.TwoWay) {
			return new Tuple<DependencyProperty, DependencyProperty, IValueConverter, BindingMode>(first, second, converter, mode);
		}
		public ObservableCollection<IBarItem> Items {
			get { return (ObservableCollection<IBarItem>)GetValue(ItemsProperty); }
		}
		public object ItemsSource {
			get { return (object)GetValue(ItemsSourceProperty); }
			set { SetValue(ItemsSourceProperty, value); }
		}
		public Style ItemStyle {
			get { return (Style)GetValue(ItemStyleProperty); }
			set { SetValue(ItemStyleProperty, value); }
		}
		public DataTemplate ItemTemplate {
			get { return (DataTemplate)GetValue(ItemTemplateProperty); }
			set { SetValue(ItemTemplateProperty, value); }
		}
		public DataTemplateSelector ItemTemplateSelector {
			get { return (DataTemplateSelector)GetValue(ItemTemplateSelectorProperty); }
			set { SetValue(ItemTemplateSelectorProperty, value); }
		}		
		protected internal BarControl BarControl { get; private set; }		
		protected virtual Bar CreateBar() { return new Bar() { }; }
		protected override IEnumerator LogicalChildren {
			get { return new MergedEnumerator(base.LogicalChildren, new SingleLogicalChildEnumerator(bar)); }
		}
		BarDockInfo IBar.DockInfo { get { return bar.DockInfo; } }
		BarContainerControl IBar.OriginContainer {
			get { return ((IBar)bar).OriginContainer; }
			set {
				((IBar)bar).OriginContainer = value;
				IsStandalone = value == null;
			}
		}
		bool IBar.ShowInOriginContainer { get; set; }
		bool IBar.CanBind(BarContainerControl container, object binderKey) { return false; }
		int IBar.Index { get; set; }
		public void Merge(IBar bar) {
			Bar _bar = bar as Bar ?? (bar as ToolBarControlBase).With(x => x.Bar);
			if (_bar == null)
				return;
			Bar.Merge(_bar);
		}
		public void Unmerge(IBar bar) {
			Bar _bar = bar as Bar ?? (bar as ToolBarControlBase).With(x => x.Bar);
			if (_bar == null)
				return;
			Bar.UnMerge(_bar);
		}
		public void Unmerge() {
			Bar.UnMerge();
		}
		protected override void OnInitialized(EventArgs e) {
			base.OnInitialized(e);
			BarNameScope.EnsureRegistrator(this);
		}
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			DetachBarControl(BarControl);
			BarControl = (BarControl)GetTemplateChild(BarControlName);
			AttachBarControl(BarControl);
		}
		#region Items
		protected virtual void OnItemsCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e) {
			SyncCollectionHelper.SyncCollection(e, bar.Items, Items, x => x);
			bar.Remerge();
		}
		protected virtual void OnItemsSourceChanged(object oldValue) { CheckItemsSource(); bar.ItemLinksSource = ItemsSource; }
		protected virtual void OnItemStyleChanged(Style oldValue) { CheckItemsSource(); bar.ItemStyle = ItemStyle; }
		protected virtual void OnItemTemplateChanged(DataTemplate oldValue) { CheckItemsSource(); bar.ItemTemplate = ItemTemplate; }
		protected virtual void OnItemTemplateSelectorChanged(DataTemplateSelector oldValue) { CheckItemsSource(); bar.ItemTemplateSelector = ItemTemplateSelector; }
		protected virtual void CheckItemsSource() {
			if (ItemsSource != null && Items.Count > 0)
				throw new InvalidOperationException("Cannot Use ItemsSource when Items collection is not empty");
		}
		#endregion                
		protected virtual void OnIsStandaloneChanged(bool oldValue) {
			CoerceValue(HorizontalAlignmentProperty);
			CoerceValue(VerticalAlignmentProperty);
		}
		protected virtual void OnShowBackgroundChanged(bool oldValue) { }		
		protected virtual void AttachBarControl(BarControl value) {
			if (value == null)
				return;
			value.LinksHolder = bar;
			value.ContainerType = GetContainerType();
			((IBarLayoutTableInfo)value).LayoutPropertyChanged += RaiseLayoutPropertyChanged;
		}
		protected virtual void DetachBarControl(BarControl value) {
			if (value == null)
				return;
			value.Resources.Clear();
			value.LinksHolder = null;
			((IBarLayoutTableInfo)value).LayoutPropertyChanged -= RaiseLayoutPropertyChanged;
		}		
		protected virtual LinkContainerType GetContainerType() {
			return LinkContainerType.Bar;
		}
		protected internal void RaiseLayoutPropertyChanged(object sender, EventArgs args) {
			if (layoutPropertyChanged != null)
				layoutPropertyChanged(this, EventArgs.Empty);
		}
		Bar IBarLayoutTableInfo.Bar { get { return Bar; } }
		void IBarLayoutTableInfo.InvalidateMeasure() { InvalidateMeasure(); }
		void IBarLayoutTableInfo.Measure(Size constraint) { Measure(constraint); }
		void IBarLayoutTableInfo.Arrange(Rect finalRect) { Arrange(finalRect); }
		bool IBarLayoutTableInfo.CanDock(Dock dock) { return BarControl.Return(x => ((IBarLayoutTableInfo)x).CanDock(dock), () => true); }
		bool IBarLayoutTableInfo.UseWholeRow { get { return GetUseWholeRow(); } }
		bool IBarLayoutTableInfo.CanReduce { get { return BarControl.Return(x => ((IBarLayoutTableInfo)x).CanReduce, () => false); } }
		protected abstract bool GetUseWholeRow();
		int IBarLayoutTableInfo.Row { get { return bar.DockInfo.Row; } set { bar.DockInfo.Row = value; } }
		int IBarLayoutTableInfo.Column { get { return bar.DockInfo.Column; } set { bar.DockInfo.Column = value; } }
		int IBarLayoutTableInfo.CollectionIndex { get { return GetCollectionIndex(); } }		
		double IBarLayoutTableInfo.Offset { get { return bar.DockInfo.Offset; } set{ bar.DockInfo.Offset = value; } }
		Size IBarLayoutTableInfo.DesiredSize { get { return DesiredSize; } }
		Size IBarLayoutTableInfo.RenderSize { get { return RenderSize; } }						
		bool IBarLayoutTableInfo.MakeFloating() {
			return (BarControl as IBarLayoutTableInfo).Return(x => x.MakeFloating(), () => false);
		}
		EventHandler layoutPropertyChanged;
		event EventHandler IBarLayoutTableInfo.LayoutPropertyChanged {
			add { layoutPropertyChanged += value; }
			remove { layoutPropertyChanged -= value; }
		}		
		int GetCollectionIndex() {
			if (bar.IsMainMenu)
				return Int32.MinValue;
			if (bar.IsStatusBar)
				return Int32.MaxValue;
			return ((IBar)this).Index;
		}
	}
	[DXToolboxBrowsableAttribute]
	[ToolboxTabName(AssemblyInfo.DXTabWpfNavigation)]
	public class ToolBarControl : ToolBarControlBase {
		public static readonly DependencyProperty UseWholeRowProperty;
		public static readonly DependencyProperty ShowDragWidgetProperty;
		public static readonly DependencyProperty RotateWhenVerticalProperty;
		public static readonly DependencyProperty OrientationProperty;
		public static readonly DependencyProperty AllowCollapseProperty;
		public static readonly DependencyProperty IsCollapsedProperty;
		public static readonly DependencyProperty AllowQuickCustomizationProperty;
		[Browsable(false)]
		protected static readonly DependencyPropertyKey ActualShowDragWidgetPropertyKey;
		[Browsable(false)]
		public static readonly DependencyProperty ActualShowDragWidgetProperty;
		static ToolBarControl() {
			OrientationProperty = DependencyPropertyManager.Register("Orientation", typeof(Orientation), typeof(ToolBarControl), new FrameworkPropertyMetadata(Orientation.Horizontal, new PropertyChangedCallback((d, e) => ((ToolBarControl)d).OnOrientationChanged((Orientation)e.OldValue))));
			AllowCollapseProperty = DependencyPropertyManager.Register("AllowCollapse", typeof(bool), typeof(ToolBarControl), new FrameworkPropertyMetadata(false, new PropertyChangedCallback(OnAllowCollapseChanged)));
			IsCollapsedProperty = DependencyPropertyManager.Register("IsCollapsed", typeof(bool), typeof(ToolBarControl), new FrameworkPropertyMetadata(false));
			RotateWhenVerticalProperty = DependencyPropertyManager.Register("RotateWhenVertical", typeof(bool), typeof(ToolBarControl), new FrameworkPropertyMetadata(true));
			ShowDragWidgetProperty = DependencyPropertyManager.Register("ShowDragWidget", typeof(bool), typeof(ToolBarControl), new FrameworkPropertyMetadata(true, (d, e) => ((ToolBarControl)d).OnShowDragWidgetChanged((bool)e.OldValue)));
			UseWholeRowProperty = DependencyPropertyManager.Register("UseWholeRow", typeof(bool), typeof(ToolBarControl), new FrameworkPropertyMetadata(false, new PropertyChangedCallback((d, e) => ((ToolBarControl)d).OnUseWholeRowChanged((bool)e.OldValue))));
			AllowQuickCustomizationProperty = DependencyPropertyManager.Register("AllowQuickCustomization", typeof(bool), typeof(ToolBarControl), new FrameworkPropertyMetadata(true));
			ActualShowDragWidgetPropertyKey = DependencyPropertyManager.RegisterReadOnly("ActualShowDragWidget", typeof(bool), typeof(ToolBarControl), new FrameworkPropertyMetadata(false));
			ActualShowDragWidgetProperty = ActualShowDragWidgetPropertyKey.DependencyProperty;
		}
		public bool AllowQuickCustomization {
			get { return (bool)GetValue(AllowQuickCustomizationProperty); }
			set { SetValue(AllowQuickCustomizationProperty, value); }
		}
		public bool AllowCollapse {
			get { return (bool)GetValue(AllowCollapseProperty); }
			set { SetValue(AllowCollapseProperty, value); }
		}
		public bool IsCollapsed {
			get { return (bool)GetValue(IsCollapsedProperty); }
			set { SetValue(IsCollapsedProperty, value); }
		}
		public bool RotateWhenVertical {
			get { return (bool)GetValue(RotateWhenVerticalProperty); }
			set { SetValue(RotateWhenVerticalProperty, value); }
		}
		public bool ShowDragWidget {
			get { return (bool)GetValue(ShowDragWidgetProperty); }
			set { SetValue(ShowDragWidgetProperty, value); }
		}
		public bool UseWholeRow {
			get { return (bool)GetValue(UseWholeRowProperty); }
			set { SetValue(UseWholeRowProperty, value); }
		}
		public Orientation Orientation {
			get { return (Orientation)GetValue(OrientationProperty); }
			set { SetValue(OrientationProperty, value); }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool ActualShowDragWidget {
			get { return (bool)GetValue(ActualShowDragWidgetProperty); }
			protected internal set { this.SetValue(ActualShowDragWidgetPropertyKey, value); }
		}
		static void OnAllowCollapseChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) { }
		protected override void OnIsStandaloneChanged(bool oldValue) {
			base.OnIsStandaloneChanged(oldValue);
			UpdateActualShowDragWidget();
		}
		protected virtual void OnShowDragWidgetChanged(bool oldValue) { UpdateActualShowDragWidget(); }
		protected virtual void UpdateActualShowDragWidget() { ActualShowDragWidget = !IsStandalone && ShowDragWidget; }
		protected virtual void OnUseWholeRowChanged(bool oldValue) { Bar.UseWholeRow = UseWholeRow ? DevExpress.Utils.DefaultBoolean.True : DevExpress.Utils.DefaultBoolean.False; }		
		protected virtual void OnOrientationChanged(Orientation oldValue) { UpdateBarControlOrientation(); }
		protected virtual void UpdateBarControlOrientation() {
			if (BarControl == null)
				return;
			BarControl.ContainerOrientation = Orientation;
		}
		protected override void AttachBarControl(BarControl value) {
			base.AttachBarControl(value);
			UpdateBarControlOrientation();
		}
		protected override void CreateCustomBindings() {
			AddCustomBinding(RotateWhenVerticalProperty, Bar.RotateWhenVerticalProperty);
			AddCustomBinding(IsCollapsedProperty, Bar.IsCollapsedProperty);
			AddCustomBinding(AllowCollapseProperty, Bar.AllowCollapseProperty);
			AddCustomBinding(ActualShowDragWidgetProperty, Bar.ShowDragWidgetProperty, mode: BindingMode.OneWay);
			AddCustomBinding(AllowQuickCustomizationProperty, Bar.AllowQuickCustomizationProperty, converter: new DefaultBooleanToBooleanConverter());
		}
		protected override bool GetUseWholeRow() { return UseWholeRow; }
	}
	[DXToolboxBrowsableAttribute]
	[ToolboxTabName(AssemblyInfo.DXTabWpfNavigation)]
	public class MainMenuControl : ToolBarControlBase {
		static MainMenuControl() {
			AllowHideProperty.OverrideMetadata(typeof(MainMenuControl), new FrameworkPropertyMetadata(false));
		}
		protected override Bar CreateBar() {
			return base.CreateBar().Do(x => x.IsMainMenu = true).Do(x=>x.AllowQuickCustomization = DefaultBoolean.False);
		}
		protected override LinkContainerType GetContainerType() {
			return LinkContainerType.MainMenu;
		}
		protected override void CreateCustomBindings() { }
		protected override bool GetUseWholeRow() { return true; }
	}
	[DXToolboxBrowsableAttribute]
	[ToolboxTabName(AssemblyInfo.DXTabWpfNavigation)]
	public class StatusBarControl : ToolBarControlBase {
		public static readonly DependencyProperty ShowSizeGripProperty;
		public bool ShowSizeGrip {
			get { return (bool)GetValue(ShowSizeGripProperty); }
			set { SetValue(ShowSizeGripProperty, value); }
		}
		static StatusBarControl() {
			ShowSizeGripProperty = DependencyPropertyManager.Register("ShowSizeGrip", typeof(bool), typeof(StatusBarControl), new FrameworkPropertyMetadata(false));
			AllowHideProperty.OverrideMetadata(typeof(StatusBarControl), new FrameworkPropertyMetadata(false));
		}
		protected override void CreateCustomBindings() {
			AddCustomBinding(ShowSizeGripProperty, Bar.ShowSizeGripProperty);
		}
		protected override Bar CreateBar() { return base.CreateBar().Do(x => x.IsStatusBar = true).Do(x => x.AllowQuickCustomization = DefaultBoolean.False); }
		protected override LinkContainerType GetContainerType() { return LinkContainerType.StatusBar; }
		protected override bool GetUseWholeRow() { return true; }
	}
}
