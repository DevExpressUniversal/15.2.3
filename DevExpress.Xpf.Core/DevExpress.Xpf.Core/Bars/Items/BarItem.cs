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
using System.Windows.Markup;
using System.Windows;
using System.Windows.Media;
using System.Windows.Input;
using System.ComponentModel;
using DevExpress.Utils.Serializing;
using System.Windows.Controls;
using System.Collections;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Bars.Native;
using System.Windows.Data;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using DevExpress.Xpf.Utils;
using System.Windows.Threading;
using DevExpress.Utils;
using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Bars.Converters;
namespace DevExpress.Xpf.Bars {
	public class BarItemNamePropertyChangedEventArgs : EventArgs {
		public string NewValue { get; protected set; }
		public string OldValue { get; protected set; }
		public BarItemNamePropertyChangedEventArgs(string newValue, string oldValue) {
			NewValue = newValue;
			OldValue = oldValue;
		}
	}	
	[RuntimeNameProperty("Name")]
	public abstract class BarItem : FrameworkContentElement, IBarItem, ICommandSource, IBarManagerControllerAction, INotifyPropertyChanged, IMultipleElementRegistratorSupport
	{
		#region static
		public static readonly RoutedEvent BeforeCommandExecuteClickEvent;
		public static readonly RoutedEvent ItemClickEvent;
		public static readonly RoutedEvent ItemDoubleClickEvent;		
		public static readonly DependencyProperty GlyphSizeProperty;
		public static readonly DependencyProperty CategoryNameProperty;
		public static readonly DependencyProperty CategoryProperty;
		public static readonly DependencyProperty ContentProperty;
		public static readonly DependencyProperty ContentTemplateProperty;
		public static readonly DependencyProperty ContentTemplateSelectorProperty;
		public static readonly DependencyProperty SuperTipProperty;
		public static readonly DependencyProperty GlyphProperty;
		public static readonly DependencyProperty GlyphAlignmentProperty;
		public static readonly DependencyProperty LargeGlyphProperty;
		public static readonly DependencyProperty CommandParameterProperty;
		public static readonly DependencyProperty CommandProperty;
		public static readonly DependencyProperty CommandTargetProperty;
		public static readonly DependencyProperty KeyGestureProperty;
		public static readonly DependencyProperty HintProperty;
		public static readonly DependencyProperty IsVisibleProperty;
		public static readonly DependencyProperty DescriptionProperty;
		static readonly DependencyPropertyKey ActualCustomizationContentPropertyKey;
		public static readonly DependencyProperty ActualCustomizationContentProperty;
		public static readonly DependencyProperty CustomizationContentProperty;
		static readonly DependencyPropertyKey ActualCustomizationGlyphPropertyKey;
		public static readonly DependencyProperty ActualCustomizationGlyphProperty;
		public static readonly DependencyProperty CustomizationGlyphProperty;
		public static readonly DependencyProperty ShowKeyGestureProperty;		
		public static readonly DependencyProperty AlignmentProperty;
		public static readonly DependencyProperty ShowScreenTipProperty;
		public static readonly DependencyProperty BarItemDisplayModeProperty;
		public static readonly DependencyProperty RibbonStyleProperty;
		public static readonly DependencyProperty GlyphTemplateProperty;
		public static readonly DependencyProperty AllowGlyphThemingProperty;
		public static readonly DependencyProperty MergeTypeProperty;
		public static readonly DependencyProperty MergeOrderProperty;
		public static readonly DependencyProperty SectorIndexProperty;
		public static readonly DependencyProperty UseAsBarItemGlyphProperty;
		public static readonly DependencyProperty KeyTipProperty;
		static BarItem() {
#if DEBUGTEST
			ShouldCoerceGlyph = true;
#endif
			KeyTipProperty = DependencyPropertyManager.Register("KeyTip", typeof(string), typeof(BarItem), new FrameworkPropertyMetadata(null));
			NameProperty.OverrideMetadata(typeof(BarItem), new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.None, new PropertyChangedCallback(OnNamePropertyChanged)));
			ShowKeyGestureProperty = DependencyPropertyManager.Register("ShowKeyGesture", typeof(bool), typeof(BarItem), new FrameworkPropertyMetadata(true, (d, e) => ((BarItem)d).ExecuteActionOnLinkControls(x => x.UpdateShowKeyGesture())));
			ItemDoubleClickEvent = EventManager.RegisterRoutedEvent("ItemDoubleClick", RoutingStrategy.Direct, typeof(ItemClickEventHandler), typeof(BarItem));
			ItemClickEvent = EventManager.RegisterRoutedEvent("ItemClick", RoutingStrategy.Direct, typeof(ItemClickEventHandler), typeof(BarItem));
			BeforeCommandExecuteClickEvent = EventManager.RegisterRoutedEvent("BeforeCommandExecute", RoutingStrategy.Direct, typeof(ItemClickEventHandler), typeof(BarItem));
			IsVisibleProperty = DependencyPropertyManager.Register("IsVisible", typeof(bool), typeof(BarItem), new FrameworkPropertyMetadata(true, new PropertyChangedCallback(OnIsVisiblePropertyChanged)));
			ToolTipProperty.OverrideMetadata(typeof(BarItem), new FrameworkPropertyMetadata(new PropertyChangedCallback((d, e) => ((BarItem)d).OnToolTipChanged(e.OldValue, e.NewValue))));
			HintProperty = DependencyPropertyManager.Register("Hint", typeof(object), typeof(BarItem), new FrameworkPropertyMetadata(null, (d, e) => ((BarItem)d).UpdateProperties()));
			DescriptionProperty = DependencyPropertyManager.Register("Description", typeof(string), typeof(BarItem), new FrameworkPropertyMetadata("", new PropertyChangedCallback(OnDescriptionPropertyChanged)));
			CommandProperty = DependencyPropertyManager.Register("Command", typeof(ICommand), typeof(BarItem), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnCommandChanged)));
			CommandParameterProperty = DependencyPropertyManager.Register("CommandParameter", typeof(object), typeof(BarItem), new FrameworkPropertyMetadata(null, (d, e) => ((BarItem)d).UpdateCanExecute()));
			CommandTargetProperty = DependencyPropertyManager.Register("CommandTarget", typeof(IInputElement), typeof(BarItem), new FrameworkPropertyMetadata(null, (d, e) => ((BarItem)d).UpdateCanExecute()));
			ContentProperty = DependencyPropertyManager.Register("Content", typeof(object), typeof(BarItem), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnContentPropertyChanged)));
			ContentTemplateProperty = DependencyPropertyManager.Register("ContentTemplate", typeof(DataTemplate), typeof(BarItem), new FrameworkPropertyMetadata(null, (d, e) => ((BarItem)d).OnContentTemplateChanged(e)));
			ContentTemplateSelectorProperty = DependencyPropertyManager.Register("ContentTemplateSelector", typeof(DataTemplateSelector), typeof(BarItem), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnContentTemplateSelectorPropertyChanged)));
			SuperTipProperty = DependencyPropertyManager.Register("SuperTip", typeof(SuperTip), typeof(BarItem), new FrameworkPropertyMetadata(null, (d, e) => ((BarItem)d).OnSuperTipChanged((SuperTip)e.OldValue)));
			GlyphProperty = DependencyPropertyManager.Register("Glyph", typeof(ImageSource), typeof(BarItem), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnGlyphPropertyChanged), (d,e)=>((BarItem)d).CoerceGlyph(e)));
			GlyphAlignmentProperty = DependencyPropertyManager.Register("GlyphAlignment", typeof(Dock), typeof(BarItem), new FrameworkPropertyMetadata(Dock.Left, (d, e)=>((BarItem)d).ExecuteActionOnLinkControls(lc => lc.OnSourceGlyphAlignmentChanged())));
			GlyphSizeProperty = DependencyPropertyManager.Register("GlyphSize", typeof(GlyphSize), typeof(BarItem), new FrameworkPropertyMetadata(GlyphSize.Default, new PropertyChangedCallback(OnGlyphSizePropertyChanged)));
			LargeGlyphProperty = DependencyPropertyManager.Register("LargeGlyph", typeof(ImageSource), typeof(BarItem), new FrameworkPropertyMetadata(null,  new PropertyChangedCallback(OnLargeGlyphPropertyChanged), (d, e) => ((BarItem)d).CoerceGlyph(e)));
			CategoryNameProperty = DependencyPropertyManager.Register("CategoryName", typeof(string), typeof(BarItem), new FrameworkPropertyMetadata("Unassigned Items", FrameworkPropertyMetadataOptions.None, new PropertyChangedCallback(OnCategoryNamePropertyChanged)));
			CategoryProperty = DependencyPropertyManager.Register("Category", typeof(BarManagerCategory), typeof(BarItem), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.None, new PropertyChangedCallback(OnCategoryPropertyChanged)));
			KeyGestureProperty = DependencyPropertyManager.Register("KeyGesture", typeof(KeyGesture), typeof(BarItem), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.None, new PropertyChangedCallback(OnKeyGestureChanged)));
			ActualCustomizationContentPropertyKey = DependencyPropertyManager.RegisterReadOnly("ActualCustomizationContent", typeof(object), typeof(BarItem), new FrameworkPropertyMetadata(null));
			CustomizationContentProperty = DependencyPropertyManager.Register("CustomizationContent", typeof(object), typeof(BarItem), new FrameworkPropertyMetadata(null, (d, e) => ((BarItem)d).OnCustomizationContentChanged()));
			ActualCustomizationGlyphPropertyKey = DependencyPropertyManager.RegisterReadOnly("ActualCustomizationGlyph", typeof(ImageSource), typeof(BarItem), new FrameworkPropertyMetadata(null));
			ActualCustomizationGlyphProperty = ActualCustomizationGlyphPropertyKey.DependencyProperty;
			CustomizationGlyphProperty = DependencyPropertyManager.Register("CustomizationGlyph", typeof(ImageSource), typeof(BarItem), new FrameworkPropertyMetadata(null, (d, e) => ((BarItem)d).OnCustomizationGlyphChanged()));
			AlignmentProperty = DependencyPropertyManager.Register("Alignment", typeof(BarItemAlignment), typeof(BarItem), new PropertyMetadata(BarItemAlignment.Default, new PropertyChangedCallback(OnAlignmentPropertyChanged)));
			ShowScreenTipProperty = DependencyPropertyManager.Register("ShowScreenTip", typeof(bool), typeof(BarItem), new PropertyMetadata(true, (d, e) => ((BarItem)d).UpdateProperties()));
			BarItemDisplayModeProperty = DependencyPropertyManager.Register("BarItemDisplayMode", typeof(BarItemDisplayMode), typeof(BarItem), new FrameworkPropertyMetadata(BarItemDisplayMode.Default, new PropertyChangedCallback(OnBarItemDisplayModePropertyChanged)));
			RibbonStyleProperty = DependencyPropertyManager.Register("RibbonStyle", typeof(RibbonItemStyles), typeof(BarItem), new FrameworkPropertyMetadata(RibbonItemStyles.Default, new PropertyChangedCallback(OnRibbonStylePropertyChanged)));
			ActualCustomizationContentProperty = ActualCustomizationContentPropertyKey.DependencyProperty;
			GlyphTemplateProperty = DependencyPropertyManager.Register("GlyphTemplate", typeof(DataTemplate), typeof(BarItem), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnGlyphTemplatePropertyChanged)));
			UseAsBarItemGlyphProperty = DependencyPropertyManager.RegisterAttached("UseAsBarItemGlyph", typeof(bool), typeof(BarItem), new FrameworkPropertyMetadata(false, new PropertyChangedCallback(OnUseAsBarItemGlyphPropertyChanged)));
			AllowGlyphThemingProperty = DependencyPropertyManager.Register("AllowGlyphTheming", typeof(bool?), typeof(BarItem), new FrameworkPropertyMetadata(null, (d, e) => ((BarItem)d).OnAllowGlyphThemingChanged((bool?)e.OldValue, (bool?)e.NewValue)));
			MergeTypeProperty = DependencyPropertyManager.Register("MergeType", typeof(BarItemMergeType), typeof(BarItem), new FrameworkPropertyMetadata(BarItemMergeType.Add));
			MergeOrderProperty = DependencyPropertyManager.Register("MergeOrder", typeof(int), typeof(BarItem), new FrameworkPropertyMetadata(-1));
			SectorIndexProperty = DependencyPropertyManager.Register("SectorIndex", typeof(int), typeof(BarItem), new FrameworkPropertyMetadata(-1, (d,e)=>((BarItem)d).OnSectorIndexChanged()));
			BarManager.ToolbarGlyphSizeProperty.OverrideMetadata(typeof(BarItem), new FrameworkPropertyMetadata(GlyphSize.Default, FrameworkPropertyMetadataOptions.Inherits, (o, e) => ((BarItem)o).OnToolbarGlyphSizeChanged((GlyphSize)e.OldValue, (GlyphSize)e.NewValue)));
		}
		protected static void OnNamePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((BarItem)d).OnNameChanged(e.NewValue as string, e.OldValue as string);
		}
		protected static void OnContentTemplateSelectorPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((BarItem)d).OnContentTemplateSelectorChanged(e.OldValue as DataTemplateSelector);
		}
		protected static void OnGlyphSizePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((BarItem)d).OnGlyphSizeChanged(e);
		}
		protected static void OnDescriptionPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e) {
			((BarItem)obj).OnDescriptionChanged(e.OldValue);
		}
		protected static void OnIsVisiblePropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e) {
			((BarItem)obj).OnIsVisibleChanged(e);
		}
		protected static void OnKeyGestureChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e) {
			((BarItem)obj).OnKeyGestureChanged(e);
		}
		protected static void OnCategoryNamePropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e) {
			((BarItem)obj).OnCategoryNameChanged(e);
		}
		protected static void OnCategoryPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e) {
			((BarItem)obj).OnCategoryChanged(e);
		}
		protected static void OnGlyphPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e) {
			((BarItem)obj).glyphInternal = e.NewValue as ImageSource;
			((BarItem)obj).OnGlyphChanged(e.OldValue as ImageSource);
		}
		protected static void OnLargeGlyphPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e) {
			((BarItem)obj).OnLargeGlyphChanged(e.OldValue as ImageSource);
		}
		protected static void OnContentPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e) {
			((BarItem)obj).OnContentChanged(e.OldValue);
		}
		static void OnCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((BarItem)d).OnCommandChanged((ICommand)e.OldValue, (ICommand)e.NewValue);
		}
		protected static void OnBarItemDisplayModePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((BarItem)d).OnBarItemDisplayModeChanged(e);
		}
		protected static void OnRibbonStylePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((BarItem)d).OnRibbonStyleChanged((RibbonItemStyles)e.OldValue);
		}
		protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e) {
			this.SafeOnPropertyChanged(e);
		}
		internal static bool IsContainsItself(ILinksHolder holder, ILinksHolder child) {
			if(holder == null || child == null)
				return false;
			if(holder == child)
				return true;
			foreach(BarItemLinkBase linkBase in holder.ActualLinks) {
				BarItemLink link = linkBase as BarItemLink;
				if(link == null)
					continue;
				ILinksHolder h = link.Item as ILinksHolder;
				if(h == null)
					continue;
				if(IsContainsItself(h, child))
					return true;
			}
			return false;
		}
		protected internal virtual BarItemLinkControlBase CreateBarItemLinkControl(BarItemLinkBase link) {
			return BarItemLinkControlCreator.Default.Create(GetLinkType(), link);
		}
		protected static void OnAlignmentPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((BarItem)d).OnAlignmentChanged(e);
		}
		protected static void OnGlyphTemplatePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((BarItem)d).OnGlyphTemplateChanged((DataTemplate)e.OldValue);
		}
		protected static void OnUseAsBarItemGlyphPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			if(!(d is Image))
				return;
			ImageUpdater im = new ImageUpdater(d as Image);
		}
		public static bool GetUseAsBarItemGlyph(DependencyObject obj) {
			return (bool)obj.GetValue(UseAsBarItemGlyphProperty);
		}
		public static void SetUseAsBarItemGlyph(DependencyObject obj, bool value) {
			obj.SetValue(UseAsBarItemGlyphProperty, value);
		}
		#endregion
		bool canExecute = true;
		private WeakReference styleWR = new WeakReference(null);
		private WeakReference templateWR = new WeakReference(null);
		DataTemplate HolderItemTemplate {
			get { return (DataTemplate)templateWR.Target; }
			set { templateWR = new WeakReference(value); }
		}
		Style HolderItemStyle {
			get { return (Style)styleWR.Target; }
			set { styleWR = new WeakReference(value); }
		}
		internal bool CompareWithItemCreatedFromSource(BarItem second) {
			if (second == null)
				return false;
			return DataContext!=null && Object.ReferenceEquals(DependencyObjectExtensions.GetDataContext(this), DependencyObjectExtensions.GetDataContext(second)) 
				&& ((HolderItemStyle==null && second.HolderItemStyle==null) || Object.Equals(HolderItemStyle, second.HolderItemStyle))
				&& ((HolderItemTemplate == null && second.HolderItemTemplate == null) || Object.Equals(HolderItemTemplate, second.HolderItemTemplate));
		}
		public void SetItemSourceData(Style itemStyle, DataTemplate itemTemplate) {
			HolderItemStyle = itemStyle;
			HolderItemTemplate = itemTemplate;
		}
		protected override bool ShouldSerializeProperty(DependencyProperty dp) {
			if(dp.Name == "Manager") return false;			
			return base.ShouldSerializeProperty(dp);
		}
		protected BarItem() {
			onCanExecuteChangedWeakEventHandler = new CanExecuteCommandWeakEventHandler(this,
					(item, sender, e) => {
						BarItem barItem = item as BarItem;
						barItem.OnCanExecuteChanged(sender, e);
					});
			Loaded -= OnLoaded;
			Loaded += OnLoaded;
			Unloaded -= OnUnloaded;
			Unloaded += OnUnloaded;
			IsEnabledChanged += new DependencyPropertyChangedEventHandler(OnIsEnabledChanged);
			DataContextChanged += OnDataContextChanged;
			NSName = this;
		}
		internal bool SkipAddToBarManagerLogicalTree { get; set; }		
		protected virtual void OnLoaded(object sender, System.Windows.RoutedEventArgs e) {			
			FrameworkElementHelper.SetIsLoaded(this, true);			
		}
		protected virtual void OnUnloaded(object sender, System.Windows.RoutedEventArgs e) {
			FrameworkElementHelper.SetIsLoaded(this, false);			
		}		
		[
#if !SL
	DevExpressXpfCoreLocalizedDescription("BarItemContent"),
#endif
		TypeConverter(typeof(ObjectConverter))]
		public object Content {
			get { return GetValue(ContentProperty); }
			set { SetValue(ContentProperty, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("BarItemContentTemplate")]
#endif
		public DataTemplate ContentTemplate {
			get { return (DataTemplate)GetValue(ContentTemplateProperty); }
			set { SetValue(ContentTemplateProperty, value); }
		}
		public DataTemplate GlyphTemplate {
			get { return (DataTemplate)GetValue(GlyphTemplateProperty); }
			set { SetValue(GlyphTemplateProperty, value); }
		}		
#if !SL
	[DevExpressXpfCoreLocalizedDescription("BarItemContentTemplateSelector")]
#endif
		public DataTemplateSelector ContentTemplateSelector {
			get { return (DataTemplateSelector)GetValue(ContentTemplateSelectorProperty); }
			set { SetValue(ContentTemplateSelectorProperty, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("BarItemSuperTip")]
#endif
		public SuperTip SuperTip {
			get { return (SuperTip)GetValue(SuperTipProperty); }
			set { SetValue(SuperTipProperty, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("BarItemShowScreenTip")]
#endif
		public bool ShowScreenTip {
			get { return (bool)GetValue(ShowScreenTipProperty); }
			set { SetValue(ShowScreenTipProperty, value); }
		}
		[
#if !SL
	DevExpressXpfCoreLocalizedDescription("BarItemBarItemName"),
#endif
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public string BarItemName {
			get { return Name; }
			set { 
				if(value != null)
					Name = value; 
			}
		}
		[
#if !SL
	DevExpressXpfCoreLocalizedDescription("BarItemCustomizationContent"),
#endif
		TypeConverter(typeof(ObjectConverter))]
		public object CustomizationContent {
			get { return GetValue(CustomizationContentProperty); }
			set { this.SetValue(CustomizationContentProperty, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("BarItemCustomizationGlyph")]
#endif
		public ImageSource CustomizationGlyph {
			get { return (ImageSource)GetValue(CustomizationGlyphProperty); }
			set { this.SetValue(CustomizationGlyphProperty, value); }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public ImageSource ActualCustomizationGlyph {
			get { return (ImageSource)GetValue(ActualCustomizationGlyphProperty); }
			private set { this.SetValue(ActualCustomizationGlyphPropertyKey, value); }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public object ActualCustomizationContent {
			get { return GetValue(ActualCustomizationContentProperty); }
			private set { this.SetValue(ActualCustomizationContentPropertyKey, value); }
		}
		public bool ShowKeyGesture {
			get { return (bool)GetValue(ShowKeyGestureProperty); }
			set { SetValue(ShowKeyGestureProperty, value); }
		}		
#if !SL
	[DevExpressXpfCoreLocalizedDescription("BarItemGlyphSize")]
#endif
		public GlyphSize GlyphSize {
			get { return (GlyphSize)GetValue(GlyphSizeProperty); }
			set { SetValue(GlyphSizeProperty, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("BarItemGlyphAlignment")]
#endif
		public Dock GlyphAlignment {
			get { return (Dock)GetValue(GlyphAlignmentProperty); }
			set { SetValue(GlyphAlignmentProperty, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("BarItemGlyph")]
#endif
		public ImageSource Glyph {
			get { return glyphInternal; }
			set { SetValue(GlyphProperty, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("BarItemLargeGlyph")]
#endif
		public ImageSource LargeGlyph {
			get { return (ImageSource)GetValue(LargeGlyphProperty); }
			set { SetValue(LargeGlyphProperty, value); }
		}
		[
#if !SL
	DevExpressXpfCoreLocalizedDescription("BarItemCategoryName"),
#endif
		DesignerSerializationOptions(DesignerSerializationOptions.SerializeAsAttribute)]
		public string CategoryName {
			get { return (string)GetValue(CategoryNameProperty); }
			set { SetValue(CategoryNameProperty, value); }
		}
		[
#if !SL
	DevExpressXpfCoreLocalizedDescription("BarItemCategory"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		DesignerSerializationOptions(DesignerSerializationOptions.SerializeAsAttribute)]
		public BarManagerCategory Category {
			get { return (BarManagerCategory)GetValue(CategoryProperty); }
			set { SetValue(CategoryProperty, value); }
		}
		[
#if !SL
	DevExpressXpfCoreLocalizedDescription("BarItemHint"),
#endif
		DesignerSerializationOptions(DesignerSerializationOptions.SerializeAsAttribute),
		TypeConverter(typeof(ObjectConverter))]
		public object Hint {
			get { return GetValue(HintProperty); }
			set { SetValue(HintProperty, value); }
		}
		public string KeyTip {
			get { return (string)GetValue(KeyTipProperty); }
			set { SetValue(KeyTipProperty, value); }
		}
		[
#if !SL
	DevExpressXpfCoreLocalizedDescription("BarItemDescription"),
#endif
		DesignerSerializationOptions(DesignerSerializationOptions.SerializeAsAttribute),
		TypeConverter(typeof(ObjectConverter))]
		public string Description {
			get { return (string)GetValue(DescriptionProperty); }
			set { SetValue(DescriptionProperty, value); }
		}
		[
#if !SL
	DevExpressXpfCoreLocalizedDescription("BarItemCommand"),
#endif
		Bindable(true), Localizability(LocalizationCategory.NeverLocalize)]
		public ICommand Command {
			get { return (ICommand)GetValue(CommandProperty); }
			set { SetValue(CommandProperty, value); }
		}		
		[
#if !SL
	DevExpressXpfCoreLocalizedDescription("BarItemCommandParameter"),
#endif
		Bindable(true), Localizability(LocalizationCategory.NeverLocalize)]
		public object CommandParameter {
			get { return GetValue(CommandParameterProperty); }
			set { SetValue(CommandParameterProperty, value); }
		}		
		[
#if !SL
	DevExpressXpfCoreLocalizedDescription("BarItemCommandTarget"),
#endif
		Bindable(true)]
		public IInputElement CommandTarget {
			get { return (IInputElement)GetValue(CommandTargetProperty); }
			set { SetValue(CommandTargetProperty, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("BarItemIsVisible")]
#endif
		public bool IsVisible {
			get { return (bool)GetValue(IsVisibleProperty); }
			set { SetValue(IsVisibleProperty, value); }
		}		
#if !SL
	[DevExpressXpfCoreLocalizedDescription("BarItemAlignment")]
#endif
		public BarItemAlignment Alignment {
			get { return (BarItemAlignment)GetValue(AlignmentProperty); }
			set { SetValue(AlignmentProperty, value); }
		}
		bool isPrivate;
		[
#if !SL
	DevExpressXpfCoreLocalizedDescription("BarItemIsPrivate"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
		public bool IsPrivate {
			get { return isPrivate; }
			set {
				if(IsPrivate == value) return;
				isPrivate = value;
				UpdateProperties();
			}
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("BarItemKeyGesture")]
#endif
		public KeyGesture KeyGesture {
			get { return (KeyGesture)GetValue(KeyGestureProperty); }
			set { SetValue(KeyGestureProperty, value); }
		}
		[
#if !SL
	DevExpressXpfCoreLocalizedDescription("BarItemItemTypeName"),
#endif
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public virtual string ItemTypeName {
			get { return GetType().FullName; }
			set { }
		}
		ReadOnlyLinkCollection links;
		[
#if !SL
	DevExpressXpfCoreLocalizedDescription("BarItemLinks"),
#endif
 Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public ReadOnlyLinkCollection Links {
			get {
				if(links == null) {
					links = new ReadOnlyLinkCollection();
					links.CollectionChanged += OnLinksCollectionChanged;
				}
				return links;
			}
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("BarItemBarItemDisplayMode")]
#endif
		public BarItemDisplayMode BarItemDisplayMode {
			get { return (BarItemDisplayMode)GetValue(BarItemDisplayModeProperty); }
			set { SetValue(BarItemDisplayModeProperty, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("BarItemRibbonStyle")]
#endif
		public RibbonItemStyles RibbonStyle {
			get { return (RibbonItemStyles)GetValue(RibbonStyleProperty); }
			set { SetValue(RibbonStyleProperty, value); }
		}
		public bool? AllowGlyphTheming {
			get { return (bool?)GetValue(AllowGlyphThemingProperty); }
			set { SetValue(AllowGlyphThemingProperty, value); }
		}
		public BarItemMergeType MergeType {
			get { return (BarItemMergeType)GetValue(MergeTypeProperty); }
			set { SetValue(MergeTypeProperty, value); }
		}
		public int MergeOrder {
			get { return (int)GetValue(MergeOrderProperty); }
			set { SetValue(MergeOrderProperty, value); }
		}
		[TypeConverter(typeof(DefaultValueInt32Converter))]
		public int SectorIndex {
			get { return (int)GetValue(SectorIndexProperty); }
			set { SetValue(SectorIndexProperty, value); }
		}
		public event ItemClickEventHandler ItemClick {
			add { this.AddHandler(ItemClickEvent, value); }
			remove { this.RemoveHandler(ItemClickEvent, value); }
		}
		DevExpress.Xpf.Bars.Native.WeakList<ItemClickEventHandler> handlersWeakItemClick = new Bars.Native.WeakList<ItemClickEventHandler>();
		internal event ItemClickEventHandler WeakItemClick {
			add { handlersWeakItemClick.Add(value); }
			remove { handlersWeakItemClick.Remove(value); }
		}
		void RaiseWeakItemClick(ItemClickEventArgs args) {
			foreach (ItemClickEventHandler e in handlersWeakItemClick)
				e(this, args);
		}
		public event ItemClickEventHandler ItemDoubleClick {
			add { this.AddHandler(ItemDoubleClickEvent, value); }
			remove { this.RemoveHandler(ItemDoubleClickEvent, value); }
		}
		public event ItemClickEventHandler BeforeCommandExecute {
			add { this.AddHandler(BeforeCommandExecuteClickEvent, value); }
			remove { this.RemoveHandler(BeforeCommandExecuteClickEvent, value); }
		}		
		protected internal virtual void RaiseItemClick(BarItemLink link) {
			var args = new ItemClickEventArgs(this, link) { RoutedEvent = ItemClickEvent };
			this.RaiseEvent(args);
			RaiseWeakItemClick(args);
		}
		protected internal virtual void RaiseItemDoubleClick(BarItemLink link) {
			this.RaiseEvent(new ItemClickEventArgs(this, link) { RoutedEvent = ItemDoubleClickEvent });
		}
		protected internal virtual bool RaiseBeforeCommandExecute(BarItemLink link) {
			var args = new ItemClickEventArgs(this, link) { RoutedEvent = BeforeCommandExecuteClickEvent };
			this.RaiseEvent(args);
			return args.Handled;
		}
		protected virtual void OnDescriptionChanged(object oldValue) {
			UpdateProperties();
			ExecuteActionOnLinkControls(lc => lc.OnSourceDescriptionChanged());
		}
		protected virtual void OnIsVisibleChanged(DependencyPropertyChangedEventArgs e) {
			ExecuteActionOnLinks((l) => l.UpdateActualIsVisible());
			ExecuteActionOnBaseLinkControls((lc) => lc.UpdateVisibility());
		}
		protected override System.Collections.IEnumerator LogicalChildren {
			get {
				return new SingleLogicalChildEnumerator(Content);
			}
		}
		internal ImageSource GetGlyph() {
			if(Glyph != null)
				return Glyph;
			return BarItemDefaultProperties.GetGlyph(this);
		}
		internal ImageSource GetLargeGlyph() {
			if(LargeGlyph != null)
				return LargeGlyph;
			return BarItemDefaultProperties.GetLargeGlyph(this);
		}
		internal object GetContent() {
			if(Content != null)
				return Content;
			return BarItemDefaultProperties.GetContent(this);
		}
		internal object GetCustomizationContent() {
			if(CustomizationContent != null)
				return CustomizationContent;
			return BarItemDefaultProperties.GetCustomizationContent(this);
		}
		internal ImageSource GetCustomizationGlyph() {
			if(CustomizationGlyph != null)
				return CustomizationGlyph;
			return BarItemDefaultProperties.GetCustomizationGlyph(this);
		}
		internal DataTemplate GetContentTemplate() {
			if(ContentTemplate != null)
				return ContentTemplate;
			return BarItemDefaultProperties.GetContentTemplate(this);
		}
		internal string GetDescription() {
			if(!string.IsNullOrEmpty(Description))
				return Description;
			return BarItemDefaultProperties.GetDescription(this);
		}
		internal SuperTip GetSuperTip() {
			if(SuperTip != null)
				return SuperTip;
			return BarItemDefaultProperties.GetSuperTip(this);
		}
		internal object GetHint() {
			return Hint ?? ToolTip ?? BarItemDefaultProperties.GetHint(this);
		}
		protected virtual void OnBarItemDisplayModeChanged(DependencyPropertyChangedEventArgs e) {			
			ExecuteActionOnLinkControls((lc) => lc.OnSourceBarItemDisplayModeChanged());
		}
		protected virtual void OnRibbonStyleChanged(RibbonItemStyles oldValue) {
			ExecuteActionOnLinkControls((lc) => lc.InitializeRibbonStyle());
		}
		protected virtual void OnContentTemplateChanged(DependencyPropertyChangedEventArgs e) {
			ExecuteActionOnLinks((l) => { if(l is BarItemLink) ((BarItemLink)l).UpdateActualContentTemplate(); });
			ExecuteActionOnLinkControls(lc => lc.OnSourceContentTemplateChanged());
		}		
		protected virtual void OnIsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e) {
			UpdateLinkByItemIsEnabled();
		}
		ImageSource glyphInternal = null;
#if DEBUGTEST
		public static bool ShouldCoerceGlyph { get; set; }
#endif
		protected virtual object CoerceGlyph(object e) {
#if DEBUGTEST
			if (!ShouldCoerceGlyph) return e;
#endif
			ImageSource glyph = e as ImageSource;
			if (glyph == null || glyph.IsFrozen) return e;
			var bSource = glyph as System.Windows.Media.Imaging.BitmapImage;
			if (bSource != null) {
				IUriContext iUriContext = glyph as IUriContext;
				if (iUriContext != null && bSource.UriSource != null && !bSource.UriSource.IsAbsoluteUri && iUriContext.BaseUri==null) {
					iUriContext.BaseUri = System.Windows.Navigation.BaseUriHelper.GetBaseUri(this);
				}
			}
			if (glyph.CanFreeze)
				glyph = glyph.GetAsFrozen() as ImageSource;
			return glyph;
		}		
		protected virtual void OnGlyphChanged(ImageSource oldValue) {
			UpdateActualCustomizationGlyph();
			ExecuteActionOnLinkControls((lc) => lc.OnSourceGlyphChanged());
		}
		protected virtual void OnLargeGlyphChanged(ImageSource oldValue) {
			UpdateActualCustomizationLargeGlyph();
			ExecuteActionOnLinkControls((lc) => lc.OnSourceLargeGlyphChanged());
		}
		protected virtual void OnContentChanged(object oldValue) {			
			if(oldValue != null) RemoveLogicalChild(oldValue as DependencyObject);
			if(Content != null) AddLogicalChild(Content as DependencyObject);
			OnSourceContentChanged();
		}
		protected virtual void OnToolbarGlyphSizeChanged(GlyphSize oldValue, GlyphSize newValue) { ExecuteActionOnLinkControls(x => x.UpdateActualGlyph()); }
		protected virtual internal void OnSourceContentChanged() {
			foreach(BarItemLinkBase link in Links)
				link.OnItemContentChanged();
			UpdateActualCustomizationContent();
			ExecuteActionOnLinkControls((lc) => lc.OnSourceContentChanged());
		}
		protected virtual void OnNameChanged(string newValue, string oldValue) {
			UpdateActualCustomizationContent();
			ExecuteActionOnLinks(li => ((BarItemLink)li).EnsureBarItemName());
			NSName = String.IsNullOrEmpty(newValue) ? (object)this : newValue;
			BarNameScope.GetService<IElementRegistratorService>(this).NameChanged(this, typeof(IFrameworkInputElement), oldValue, newValue);
		}				
		protected internal void ExecuteActionOnLinkControls<T> (LinkControlAction<T> action) where T : BarItemLinkControlBase {
			ExecuteActionOnLinks((l) => l.ExecuteActionOnLinkControls<T>(action));
		}
		protected internal void ExecuteActionOnLinkControls(LinkControlAction<BarItemLinkControl> action) {
			ExecuteActionOnLinks((l) => l.ExecuteActionOnLinkControls(action));
		}
		protected internal void ExecuteActionOnBaseLinkControls(LinkControlAction<BarItemLinkControlBase> action) {
			ExecuteActionOnLinks((l) => l.ExecuteActionOnBaseLinkControls(action));
		}
		protected internal void ExecuteActionOnLinks<T>(LinkAction<T> action) where T:BarItemLinkBase {
			foreach(T link in Links.ToList().OfType<T>()) {
				action(link);
			}
		}
		protected internal void ExecuteActionOnLinks(LinkAction<BarItemLink> action) {
			ExecuteActionOnLinks<BarItemLink>(action);
		}
		protected internal virtual void UpdateActualCustomizationContent() {
			string stringCustomizationContent = GetCustomizationContent() == null ? "" : GetCustomizationContent().ToString();
			string stringContent = GetContent() == null ? "" : GetContent().ToString();
			if(!string.IsNullOrEmpty(stringCustomizationContent))
				ActualCustomizationContent = stringCustomizationContent;
			else if(!string.IsNullOrEmpty(stringContent))
				ActualCustomizationContent = stringContent;
			else
				ActualCustomizationContent = "(" + Name + ")";
		}
		protected internal virtual void UpdateActualCustomizationGlyph() {
			ImageSource customizationGlyph = GetCustomizationGlyph();
			ImageSource glyph = GetGlyph();
			if(customizationGlyph != null) ActualCustomizationGlyph = customizationGlyph;
			else ActualCustomizationGlyph = glyph; 
		}
		protected internal virtual void UpdateActualCustomizationLargeGlyph() {
			ImageSource customizationGlyph = GetCustomizationGlyph();
			ImageSource glyph = GetLargeGlyph();
			if(customizationGlyph != null)
				ActualCustomizationGlyph = customizationGlyph;
			else
				ActualCustomizationGlyph = glyph;
		}
		protected virtual void OnCustomizationContentChanged() {
			UpdateActualCustomizationContent();
		}
		protected virtual void OnSectorIndexChanged() {
			ExecuteActionOnLinkControls((lc) => lc.OnSourceSectorIndexChanged());
		}
		protected virtual void OnCustomizationGlyphChanged() {
			UpdateActualCustomizationGlyph();
		}
		protected virtual void OnContentTemplateSelectorChanged(DataTemplateSelector oldValue) {
			ExecuteActionOnLinkControls((lc) => lc.OnSourceContentTemplateSelectorChanged());
		}
		protected virtual void OnGlyphSizeChanged(DependencyPropertyChangedEventArgs e) {
			ExecuteActionOnLinkControls((lc) => lc.UpdateActualGlyph());			
		}
		protected virtual void OnAlignmentChanged(DependencyPropertyChangedEventArgs e) {
			foreach(BarItemLink link in Links) {
				link.UpdateAlignment();
			}
		}
		protected virtual void OnAllowGlyphThemingChanged(bool? oldValue, bool? newValue) { 
			ExecuteActionOnLinkControls(lc => lc.UpdateActualAllowGlyphTheming());
		}		
		protected virtual void OnGlyphTemplateChanged(DataTemplate oldValue) {
			ExecuteActionOnLinkControls((lc) => lc.UpdateActualGlyphTemplate());
		}
		protected virtual void OnCategoryChanged(DependencyPropertyChangedEventArgs e) {			
		}
		protected virtual void OnCategoryNameChanged(DependencyPropertyChangedEventArgs e) {			
		}
		protected virtual void OnSuperTipChanged(SuperTip oldSuperTip) {
			UpdateProperties();
			ClearDataContext(oldSuperTip);
			SetDataContext(SuperTip);
		}
		protected virtual void OnLinksCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e) {			
		}
		protected internal void SetDataContext(DependencyObject element) {
			if(element == null) return;
			DependencyProperty property = null;
			if(element is FrameworkElement) {
				property = FrameworkElement.DataContextProperty;
				if(((FrameworkElement)element).GetBindingExpression(property) != null) return;
			} 
			if(element is FrameworkContentElement) {
				property = FrameworkContentElement.DataContextProperty;
				if(((FrameworkContentElement)element).GetBindingExpression(property) != null) return;
			}
			if(property == null) return;
			element.SetValue(property, DataContext);
		}
		protected internal void ClearDataContext(DependencyObject element) {
			if(element == null) return;
			DependencyProperty property = null;
			BindingExpression bindingExpression = null;
			if(element is FrameworkElement) {
				property = FrameworkElement.DataContextProperty;
				bindingExpression = ((FrameworkElement)element).GetBindingExpression(property);
			}
			if(element is FrameworkContentElement) {
				property = FrameworkContentElement.DataContextProperty;
				bindingExpression = ((FrameworkContentElement)element).GetBindingExpression(property);
			}
			if(property == null || bindingExpression != null) return;
			element.ClearValue(FrameworkElement.DataContextProperty);
		}
		protected virtual void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e) {
			ExecuteActionOnBaseLinkControls((lc) => lc.UpdateDataContext());			
			SetDataContext(SuperTip);
		}
		protected virtual void OnToolTipChanged(object oldValue, object newValue) {
			UpdateProperties();
		}
		protected internal virtual void UpdateProperties() {
			foreach(BarItemLinkBase link in Links)
				link.UpdateProperties();
			UpdateActualCustomizationContent();
			UpdateActualCustomizationGlyph();
		}
		internal bool isInMenu = false;		
		protected internal virtual void OnItemClick(BarItemLink link) {
			bool isCommandExecuteHandled = RaiseBeforeCommandExecute(link);
			var command = Command;
			var commandParameter = CommandParameter;
			var commandTarget = ActualCommandTarget;
			Action act = new Action(() => {
				RaiseItemClick(link);
				var manager = BarManager.GetBarManager(this);
				if (manager != null && !IsPrivate)
					manager.RaiseItemClick(this, link);
				if (!isCommandExecuteHandled) {
					ExecuteCommand(command, commandParameter, commandTarget);
				}					
			});
			if(isInMenu) {
				Dispatcher.BeginInvoke(act, DispatcherPriority.Render);
			} else {
				act();
			}
		}
		protected internal virtual void OnItemDoubleClick(BarItemLink link) {
			RaiseItemDoubleClick(link);
			var manager = BarManager.GetBarManager(this);
			if (manager != null && !IsPrivate)
				manager.RaiseItemClick(this, link);
		}
		public void PerformClick() {
			OnItemClick(null);
		}		
		public virtual BarItemLink CreateLink() {
			return CreateLink(IsPrivate);
		}
		public virtual BarItemLink CreateLink(bool isPrivate) {
			BarItemLink link = BarItemLinkCreator.Default.Create(GetType(), null);
			if(link != null) {
				link.BarItemName = this.Name;
				link.Item = this;
				link.IsPrivate = isPrivate;				
			}
			return link;
		}
		private void OnKeyGestureChanged(DependencyPropertyChangedEventArgs e) {			
			UpdateProperties();
			ExecuteActionOnLinkControls((lc) => lc.OnSourceKeyGestureChanged());
			BarNameScope.GetService<ICommandSourceService>(this).KeyGestureChanged(this, (KeyGesture)e.OldValue, (KeyGesture)e.NewValue);
		}
		IInputElement ActualCommandTarget {
			get { return CommandTarget ?? GetActualCommandTarget(); }
		}
		IInputElement GetActualCommandTarget() {
			return GetFocusedLinksContol() as IInputElement;
		}
		protected virtual bool GetCanExecute() {
			if(Command == null)
				return true;
			RoutedCommand command = Command as RoutedCommand;
			if(command != null) {
				return command.CanExecute(CommandParameter, ActualCommandTarget);
			}
			return Command.CanExecute(CommandParameter);
		}
		IInputElement GetFocusedLinksContol() {
			foreach(BarItemLinkControl linkControl in GetLinkControls()) {
				if(linkControl.IsVisible() && CheckIsInFocusScope(linkControl))
					return linkControl;
			}
			return null;
		}
		bool CheckIsInFocusScope(BarItemLinkControl linkControl) {
			return FocusManager.GetFocusScope(linkControl) != null;
		}
		protected virtual IEnumerable<BarItemLinkControlBase> GetLinkControls() {
			foreach(BarItemLink link in Links) {
				foreach(BarItemLinkInfo info in link.LinkInfos) {
					if(info.LinkControl != null)
						yield return info.LinkControl;
				}
			}
		}
		protected virtual void ExecuteCommand() {
			ExecuteCommand(Command, CommandParameter, ActualCommandTarget);
		}
		protected virtual void ExecuteCommand(ICommand command, object commandParameter, IInputElement actualCommandTarget) {
			if (command == null)
				return;
			RoutedCommand routedcommand = command as RoutedCommand;
			if (routedcommand != null && routedcommand.CanExecute(commandParameter, actualCommandTarget)) {
				routedcommand.Execute(commandParameter, actualCommandTarget);
			} else if (command.CanExecute(commandParameter)) {
				ICommandWithInvoker commandWithInvoker = command as ICommandWithInvoker;
				if (commandWithInvoker != null)
					commandWithInvoker.Execute(this, commandParameter);
				else
					command.Execute(commandParameter);
			}
		}
		protected virtual void OnCommandChanged(ICommand oldCommand, ICommand newCommand) {
			BarNameScope.GetService<ICommandSourceService>(this).CommandChanged(this, oldCommand, newCommand);
		}		
		protected internal void HookCommand(ICommand newCommand) {
			UnhookCommand(newCommand);
			if(newCommand != null) {
				newCommand.CanExecuteChanged += onCanExecuteChangedWeakEventHandler.Handler;
			}
			UpdateCanExecute();
		}
		protected internal void UnhookCommand(ICommand oldCommand) {
			if(oldCommand != null)
				oldCommand.CanExecuteChanged -= onCanExecuteChangedWeakEventHandler.Handler;
			UpdateCanExecute();
		}
		protected override bool IsEnabledCore {
			get {
				return base.IsEnabledCore && CanExecute;
			}
		}
		protected internal bool CanExecute {
			get {
				return canExecute;
			}
			set {
				if(CanExecute == value && IsEnabled == value)
					return;
				canExecute = value;
				CoerceIsEnabledProperty();
			}
		}
		protected virtual void CoerceIsEnabledProperty() {
			CoerceValue(IsEnabledProperty);
		}
		protected internal virtual bool CanKeyboardSelect { get { return true; } }
		void OnCanExecuteChanged(object sender, EventArgs e) { UpdateCanExecute(); }
		protected internal void UpdateCanExecute() {
			bool canExecute = false;
			if(Command is RoutedCommand) {
				if (!HasVisibleLinkControls()) {
					return;
				}
				canExecute = GetCanExecute();
			} else if(Command is ICommand) {
				canExecute = GetCanExecute();
			} else {
				CoerceIsEnabledProperty();
				return;
			}				
				CanExecute = canExecute;
		}
		CanExecuteCommandWeakEventHandler onCanExecuteChangedWeakEventHandler;
		bool HasVisibleLinkControls() {
			foreach(BarItemLinkBase link in Links) {
				if(link == null || link.LinkInfos == null || link.LinkInfos.Count == 0) continue;
				foreach(BarItemLinkInfo linkInfo in link.LinkInfos) {
					if(linkInfo == null || linkInfo.LinkControl == null) continue;
					if(linkInfo.LinkControl.LinksControl == null) continue;
					if(FrameworkElementHelper.GetIsLoaded(linkInfo.LinkControl)) return true;
				}
			}
			return false;
		}
		void UpdateLinkByItemIsEnabled() {
			ExecuteActionOnLinks(l => l.OnItemIsEnabledChanged());
		}
		internal void ForceUpdateCanExecute() {
			CanExecute = GetCanExecute();
			UpdateLinkByItemIsEnabled();
		}		
		void SubscribeCommandExecuteChangedEvent() {
			if(Command == null) return;
			UnsubscribeCommandExecuteChangedEvent();
			HookCommand(Command);
		}
		void UnsubscribeCommandExecuteChangedEvent() {
			if(Command == null) return;
			UnhookCommand(Command);
		}
		#region IBarManagerControllerAction Members
		protected virtual void BarManagerActionExecute(DependencyObject context) {
			CollectionActionHelper.Execute(new CollectionActionWrapper(this, context));
		}
		IActionContainer IControllerAction.Container { get; set; }
		void IControllerAction.Execute(DependencyObject context) {
			BarManagerActionExecute(context);
		}
		object IBarManagerControllerAction.GetObject() {
			return this;
		}
		#endregion
		protected internal virtual Type GetLinkType() {
			return BarItemLinkCreator.Default.GetItemType(GetType());
		}
		#region internal classes
		protected internal class ImageUpdater :DependencyObject {
			Image imageCore;
			RoutedEventHandler onImageLoadedRoutedEventHandler;
			public static ImageUpdater GetImageUpdater(DependencyObject obj) {
				return (ImageUpdater)obj.GetValue(ImageUpdaterProperty);
			}
			public static void SetImageUpdater(DependencyObject obj, ImageUpdater value) {
				obj.SetValue(ImageUpdaterProperty, value);
			}
			public static readonly DependencyProperty ImageUpdaterProperty =
				DependencyPropertyManager.RegisterAttached("ImageUpdater", typeof(ImageUpdater), typeof(ImageUpdater), new PropertyMetadata(null));
			public ImageUpdater(Image image) {
				imageCore = image;
				if(imageCore != null) {
					onImageLoadedRoutedEventHandler = new RoutedEventHandler(OnImageLoaded);
					imageCore.Loaded += onImageLoadedRoutedEventHandler;
					SetImageUpdater(imageCore, this);
				}
			}
			void OnImageLoaded(object sender, System.Windows.RoutedEventArgs e) {
				UpdateLinkControlGlyph();
			}
			void UpdateLinkControlGlyph() {
				if(imageCore == null)
					return;
				BarItemLinkControl linkControl = LayoutHelper.FindParentObject<BarItemLinkControl>(imageCore);
				if(linkControl != null) {
					imageCore.Visibility = Visibility.Collapsed;
					linkControl.Glyph = imageCore.Source;
				}
			}
		}
		#endregion
		#region INotifyPropertyChanged
		public event PropertyChangedEventHandler PropertyChanged;
		protected internal void RaisePropertyChange(string propertyName) {
			if(PropertyChanged != null)
				PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}
		#endregion
		object nsName;
		object NSName {
			get { return nsName; }
			set {
				if (Equals(nsName, value))
					return;
				var oldValue = nsName;
				nsName = value;
				BarNameScope.GetService<IElementRegistratorService>(this).NameChanged(this, typeof(BarItem), oldValue, value);
			}
		}
		#region IMultipleElementRegistratorSupport Members
		IEnumerable<object> IMultipleElementRegistratorSupport.RegistratorKeys {
			get { return new object[] { typeof(BarItem), typeof(IFrameworkInputElement) }.Concat(GetRegistratorKeys()).Distinct();}
		}		
		object IMultipleElementRegistratorSupport.GetName(object registratorKey) {
			if (Equals(registratorKey, typeof(IFrameworkInputElement)))
				return Name;
			if (Equals(registratorKey, typeof(BarItem)))
				return NSName;
			return GetRegistratorName(registratorKey);			
		}
		protected virtual object GetRegistratorName(object registratorKey) { throw new ArgumentException(); }
		protected virtual IEnumerable<object> GetRegistratorKeys() { return Enumerable.Empty<object>(); }
		#endregion
		protected internal virtual void OnLinkControlLoaded(object sender, BarItemLinkControlLoadedEventArgs e) { }
	}	
	public delegate T CreateObjectMethod<T>(object arg);
	public class BarItemClassInfo<T> where T : class {
		public Type ItemType { get; set; }
		public CreateObjectMethod<T> CreateMethod { get; set; }
	}
	public abstract class ObjectCreator<T> where T : class {
		Dictionary<Type, BarItemClassInfo<T>> objects;
		static object olock = new object();
		protected Dictionary<Type, BarItemClassInfo<T>> Storage {
			get {
				lock (olock) {
				if(objects == null) {
					objects = new Dictionary<Type, BarItemClassInfo<T>>();
					RegisterObjects();
					}
				}
				return objects;
			}
		}
		protected abstract void RegisterObjects();
		protected void RegisterObject(Type baseType, BarItemClassInfo<T> classInfo) {
			Storage[baseType] = classInfo;
		}
		public virtual T Create(Type baseType, object arg) {
			if(Storage.ContainsKey(baseType)) return Storage[baseType].CreateMethod.Invoke(arg);
			while(baseType != null) {
				baseType = baseType.BaseType;
				if(baseType != null && Storage.ContainsKey(baseType)) return Storage[baseType].CreateMethod.Invoke(arg);
			}
			return null;
		}
		public Type GetItemType(Type baseType) {
			BarItemClassInfo<T> cinfo;
			while(!Storage.TryGetValue(baseType, out cinfo)) {
				baseType = baseType.BaseType;
				if(baseType == null)
					return null;
			}
			return cinfo.ItemType;
		}
	}
	public class BarItemLinkCreator : ObjectCreator<BarItemLink> {
		static BarItemLinkCreator defaultCreator;
		public static BarItemLinkCreator Default {
			get {
				if(defaultCreator == null) defaultCreator = new BarItemLinkCreator();
				return defaultCreator;
			}
		}
		protected override void RegisterObjects() {
			RegisterObject(typeof(BarButtonItem), new BarItemClassInfo<BarItemLink>() { ItemType = typeof(BarButtonItemLink), CreateMethod = delegate(object arg) { return new BarButtonItemLink(); } });
			RegisterObject(typeof(BarSplitButtonItem), new BarItemClassInfo<BarItemLink>() { ItemType = typeof(BarSplitButtonItemLink), CreateMethod = delegate(object arg) { return new BarSplitButtonItemLink(); } });
			RegisterObject(typeof(BarCheckItem), new BarItemClassInfo<BarItemLink>() { ItemType = typeof(BarCheckItemLink), CreateMethod = delegate(object arg) { return new BarCheckItemLink(); } });
			RegisterObject(typeof(BarStaticItem), new BarItemClassInfo<BarItemLink>() { ItemType = typeof(BarStaticItemLink), CreateMethod = delegate(object arg) { return new BarStaticItemLink(); } });
			RegisterObject(typeof(BarSubItem), new BarItemClassInfo<BarItemLink>() { ItemType = typeof(BarSubItemLink), CreateMethod = delegate(object arg) { return new BarSubItemLink(); } });
			RegisterObject(typeof(BarLinkContainerItem), new BarItemClassInfo<BarItemLink>() { ItemType = typeof(BarLinkContainerItemLink), CreateMethod = delegate(object arg) { return new BarLinkContainerItemLink(); } });
			RegisterObject(typeof(BarEditItem), new BarItemClassInfo<BarItemLink>() { ItemType = typeof(BarEditItemLink), CreateMethod = delegate(object arg) { return new BarEditItemLink(); } });
			RegisterObject(typeof(BarListItem), new BarItemClassInfo<BarItemLink>() { ItemType = typeof(BarListItemLink), CreateMethod = delegate(object arg) { return new BarListItemLink(); } });
			RegisterObject(typeof(ToolbarListItem), new BarItemClassInfo<BarItemLink>() { ItemType = typeof(ToolbarListItemLink), CreateMethod = delegate(object arg) { return new ToolbarListItemLink(); } });
			RegisterObject(typeof(ToolbarCheckItem), new BarItemClassInfo<BarItemLink>() { ItemType = typeof(ToolbarCheckItemLink), CreateMethod = delegate(object arg) { return new ToolbarCheckItemLink(); } });
			RegisterObject(typeof(LinkListItem), new BarItemClassInfo<BarItemLink>() { ItemType = typeof(LinkListItemLink), CreateMethod = delegate(object arg) { return new LinkListItemLink(); } });
			RegisterObject(typeof(LinkListCheckItem), new BarItemClassInfo<BarItemLink>() { ItemType = typeof(LinkListCheckItemLink), CreateMethod = delegate(object arg) { return new LinkListCheckItemLink(); } });
			RegisterObject(typeof(BarSplitCheckItem), new BarItemClassInfo<BarItemLink>() { ItemType = typeof(BarSplitCheckItemLink), CreateMethod = delegate(object arg) { return new BarSplitCheckItemLink(); } });
			RegisterObject(typeof(BarItemSeparator), new BarItemClassInfo<BarItemLink>() { ItemType = typeof(BarItemLinkSeparator), CreateMethod = delegate(object arg) { return new BarItemLinkSeparator(); } });
			RegisterObject(typeof(BarItemMenuHeader), new BarItemClassInfo<BarItemLink>() { ItemType = typeof(BarItemLinkMenuHeader), CreateMethod = delegate(object arg) { return new BarItemLinkMenuHeader(); } });
			RegisterObject(typeof(BarItemSelector), new BarItemClassInfo<BarItemLink>() { ItemType = typeof(BarItemSelectorLink), CreateMethod = delegate (object arg) { return new BarItemSelectorLink(); } });
		}
		public void RegisterObject(Type itemType, Type linkType, CreateObjectMethod<BarItemLink> linkCreateMethod) {
			RegisterObject(itemType, new BarItemClassInfo<BarItemLink>() { ItemType = linkType, CreateMethod = linkCreateMethod});
		}
	}
	public class BarItemLinkControlCreator : ObjectCreator<BarItemLinkControlBase> {
		static BarItemLinkControlCreator defaultCreator;
		public static BarItemLinkControlCreator Default {
			get {
				if(defaultCreator == null) defaultCreator = new BarItemLinkControlCreator();
				return defaultCreator;
			}
		}
		protected override void RegisterObjects() {
			RegisterObject(typeof(BarItemLinkSeparator), new BarItemClassInfo<BarItemLinkControlBase>() { ItemType = typeof(BarItemLinkSeparatorControl), CreateMethod = delegate(object arg) { return new BarItemLinkSeparatorControl((BarItemLinkSeparator)arg); } });
			RegisterObject(typeof(BarItemLinkMenuHeader), new BarItemClassInfo<BarItemLinkControlBase>() { ItemType = typeof(BarItemLinkMenuHeaderControl), CreateMethod = delegate(object arg) { return new BarItemLinkMenuHeaderControl(); } });
			RegisterObject(typeof(BarStaticItemLink), new BarItemClassInfo<BarItemLinkControlBase>() { ItemType = typeof(BarStaticItemLinkControl), CreateMethod = delegate(object arg) { return new BarStaticItemLinkControl(); } });
			RegisterObject(typeof(BarSubItemLink), new BarItemClassInfo<BarItemLinkControlBase>() { ItemType = typeof(BarSubItemLinkControl), CreateMethod = delegate(object arg) { return new BarSubItemLinkControl(); } });
			RegisterObject(typeof(BarSplitButtonItemLink), new BarItemClassInfo<BarItemLinkControlBase>() { ItemType = typeof(BarSplitButtonItemLinkControl), CreateMethod = delegate(object arg) { return new BarSplitButtonItemLinkControl(); } });
			RegisterObject(typeof(BarCheckItemLink), new BarItemClassInfo<BarItemLinkControlBase>() { ItemType = typeof(BarCheckItemLinkControl), CreateMethod = delegate(object arg) { return new BarCheckItemLinkControl(); } });
			RegisterObject(typeof(BarButtonItemLink), new BarItemClassInfo<BarItemLinkControlBase>() { ItemType = typeof(BarButtonItemLinkControl), CreateMethod = delegate(object arg) { return new BarButtonItemLinkControl(); } });
			RegisterObject(typeof(BarEditItemLink), new BarItemClassInfo<BarItemLinkControlBase>() { ItemType = typeof(BarEditItemLinkControl), CreateMethod = delegate(object arg) { return new BarEditItemLinkControl(); } });
			RegisterObject(typeof(BarSplitCheckItemLink), new BarItemClassInfo<BarItemLinkControlBase>() { ItemType = typeof(BarSplitCheckItemLinkControl), CreateMethod = delegate(object arg) { return new BarSplitCheckItemLinkControl(); } });
		}
		public void RegisterObject(Type linkType, Type linkControlType, CreateObjectMethod<BarItemLinkControlBase> linkControlCreateMethod) {
			RegisterObject(linkType, new BarItemClassInfo<BarItemLinkControlBase>() { ItemType = linkControlType, CreateMethod = linkControlCreateMethod });
		}
	}
	public class ItemClickEventArgs : RoutedEventArgs {
		public ItemClickEventArgs(BarItem item, BarItemLink link) {
			Item = item;
			Link = link;
		}
		public BarItem Item { get; private set; }
		public BarItemLink Link { get; private set; }
	}
	public delegate void ItemClickEventHandler(object sender, ItemClickEventArgs e);
	public enum GlyphSize { Default, Small, Large }
	public class SingleLogicalChildEnumerator : IEnumerator {
		public SingleLogicalChildEnumerator(object child) {
			Child = child;
		}
		public object Child { get; private set; }
		#region IEnumerator Members
		object IEnumerator.Current {
			get { return moveCount == 0 ? Child : null; }
		}
		int moveCount = -1;
		bool IEnumerator.MoveNext() {
			DependencyObject obj = Child as DependencyObject;
			if(obj == null) return false;
			moveCount++;
			return moveCount == 0;
		}
		void IEnumerator.Reset() {
			moveCount = -1;
		}
		#endregion
	}
	public class BarItemLinkControlLoadedEventArgs : EventArgs {
		public BarItem Item { get; private set; }
		public BarItemLinkBase Link { get; private set; }
		public BarItemLinkControlBase LinkControl { get; private set; }
		public BarItemLinkControlLoadedEventArgs(BarItem item, BarItemLinkBase link, BarItemLinkControlBase linkControl) {
			Item = item;
			Link = link;
			LinkControl = linkControl;
		}
	}
	public delegate void BarItemLinkControlLoadedEventHandler(object sender, BarItemLinkControlLoadedEventArgs e);
}
namespace DevExpress.Xpf.Bars.Native {
	public interface ICommandWithInvoker {
		void Execute(object invoker, object parameter);
	}
	public class WeakList<T> : DispatcherObject, IList<T> where T : class {
		List<WeakReference> references = new List<WeakReference>();
		public int IndexOf(T item) {
			for(int i = 0; i < references.Count; i++) {
				WeakReference reference = references[i];
				if (!reference.IsAlive) {
					RequestCleanup();
					continue;
				}					
				if(reference.Target == item)
					return i;
			}
			return -1;
		}
		bool cleanupRequested = false;
		protected virtual void RequestCleanup() {
			if (cleanupRequested)
				return;
			cleanupRequested = true;
			Dispatcher.BeginInvoke(new Action(CleanupOnRequest), DispatcherPriority.ContextIdle);
		}
		protected virtual void CleanupOnRequest() {
			lock (this)
			{
				CleanResolvedReferences();
				cleanupRequested = false;
			}
		}		
		public virtual void Insert(int index, T item) {
			references.Insert(index, new WeakReference(item));
		}
		public virtual void RemoveAt(int index) {
			if (references.IsValidIndex(index))
				references.RemoveAt(index);
		}
		public T this[int index] {
			get {
				return references[index].Target as T;
			}
			set {
				references[index] = new WeakReference(value);
			}
		}
		public virtual void Add(T item) {
			Insert(references.Count, item);
		}
		public virtual void Clear() {
			references.Clear();
		}
		public bool Contains(T item) {
			foreach(var element in references) {
				if (!element.IsAlive) {
					RequestCleanup();
					continue;
				}
				if (Equals(element.Target, item))
					return true;
			}
			return false;
		}
		public void CopyTo(T[] array, int arrayIndex) {
			LockReferences();
			foreach(var item in this) {
				array[arrayIndex++] = item;
			}
			UnlockReferences();
		}
		public int Count {
			get { return references.Count(); }
		}
		public bool IsReadOnly {
			get { return false; }
		}
		public virtual bool Remove(T item) {
			for(int i = 0; i<references.Count; i++) {
				var refr = references[i];
				if (!refr.IsAlive) {
					RequestCleanup();
					continue;
				}
				if(Equals(refr.Target, item)) {
					references.Remove(refr);
					return true;
				}
			}
			return false;
		}
		public IEnumerator<T> GetEnumerator() {
			return (lockedCollection ?? GetItemsCollection()).GetEnumerator();			
		}
		IEnumerator IEnumerable.GetEnumerator() {
			return GetEnumerator();
		}
		public void CleanResolvedReferences() {
			for(int i = references.Count - 1; i >= 0; i--) {
				if(!references[i].IsAlive)
					references.RemoveAt(i);
			}  
		}		
		IEnumerable<T> lockedCollection = null;
		public void LockReferences() {
			lockedCollection = GetItemsCollection();
			CleanResolvedReferences();
		}
		private IEnumerable<T> GetItemsCollection() {
			var result = references.Select(wr => wr.Target as T).Distinct().Where(itm => itm != null);
			if (result.Count()!= Count) {
				RequestCleanup();
			}
			return result;
		}
		public void UnlockReferences() {
			lockedCollection = null;
		}
	}
	public class ObservableCollectionConverter<TSource, TResult> : ObservableCollection<TResult> {
		private IEnumerable source;
		private Func<TSource,TResult> selector;
		public Func<TSource,TResult> Selector {
			get { return selector; }
			set {
				bool raiseChange = value != selector;
				Func<TSource,TResult> oldValue = selector;
				selector = value;
				if(raiseChange)
					OnSelectorChanged(oldValue);
			}
		}				
		public IEnumerable Source {
			get { return source; }
			set {
				bool raiseChange = value != source;
				if(raiseChange)
					OnSourceChanging(value);
				IEnumerable oldValue = source;
				source = value;
				if(raiseChange)
					OnSourceChanged(oldValue);
			}
		}
		protected virtual void OnSourceChanging(IEnumerable newValue) {
			if((Source as INotifyCollectionChanged) != null) ((INotifyCollectionChanged)Source).CollectionChanged -= OnSourceCollectionChanged;
			if(Source is IBindingList) ((IBindingList)Source).ListChanged -= OnSourceListChanged;
			Clear();
		}
		protected virtual void OnSourceChanged(IEnumerable oldValue) {
			if((Source as INotifyCollectionChanged) != null) ((INotifyCollectionChanged)Source).CollectionChanged += OnSourceCollectionChanged;
			if(Source is IBindingList) ((IBindingList)Source).ListChanged += OnSourceListChanged;
			OnReset();
		}
		protected virtual void OnSelectorChanged(Func<TSource, TResult> oldValue) {
			OnReset();
		}
		void OnSourceCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e) {
			switch(e.Action) {
				case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
					if (e.NewStartingIndex == -1) {
						OnReset();
						break;
					}		   
					OnAdd(e.NewStartingIndex, e.NewItems);
					break;
				case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:
					if (e.OldStartingIndex == -1) {
						OnReset();
						break;
					}
					OnRemove(e.OldStartingIndex, e.OldItems);
					break;
				default:
					OnReset();
					break;
			}
		}
		void OnSourceListChanged(object sender, ListChangedEventArgs e) {
			switch(e.ListChangedType) {
				case ListChangedType.ItemAdded:
					OnAdd(e.NewIndex, new List<object>() { ((IList)Source)[e.NewIndex] });
					break;
				case ListChangedType.ItemDeleted:
					OnRemove(e.NewIndex, new List<object>() { null });
					break;
				default:
					OnReset();
					break;
			}
		}
		protected virtual void OnReset() {
			Clear();
			if(Source == null || Selector == null) return;
			foreach(TSource item in Source as IEnumerable) {
				Add(Selector(item));
			}
		}
		protected virtual void OnRemove(int p, IList list) {
			if(Source == null || Selector == null) return;
			int startingIndex = p >= 0 && p < Count ? p : 0;
			foreach(var item in list)
				RemoveAt(startingIndex++);
		}
		protected virtual void OnAdd(int p, IList list) {
			if(Source == null || Selector == null) return;
			int startingIndex = p >= 0 && p <= Count ? p : 0;
			foreach(TSource item in list)
				Insert(startingIndex++, Selector(item));
		}				
	}
	public class AsyncObservableCollectionConverter<TSource, TResult> : ObservableCollectionConverter<TSource, TResult> {
		public AsyncObservableCollectionConverter() {
			SleepTime = -1;
			bridgeList = new List<AOCCElementData>();
			addItemAction = new PostponedAction(() => true);
		}
		public System.Windows.Threading.Dispatcher Dispatcher { get; set; }
		public int SleepTime { get; set; }
		List<AOCCElementData> bridgeList;
		DispatcherTimer addTimer = null;
		DevExpress.Data.Utils.WeakEventHandler<AsyncObservableCollectionConverter<TSource, TResult>, EventArgs, EventHandler> addTimerTickHandler;
		AOCCElementData forceAddItem = null;
		PostponedAction addItemAction;
		protected override void OnReset() {
			Clear();
			bridgeList.Clear();
			if(Source == null || Selector == null) return;
			var enumerator = Source.GetEnumerator();
			int currentIndex = 0;
			while (enumerator.MoveNext()) {
				var item = (TSource)enumerator.Current;
				bridgeList.Add(new AOCCElementData() { Exists = false, InResultCollectionIndex = -1, InSourceCollectionIndex = currentIndex++, SourceElement = item, ResultElement = Selector(item) });
			}		   
			CheckStartItemAddition();
		}
		protected override void OnRemove(int p, System.Collections.IList list) {
			if(Source == null || Selector == null) return;
			foreach(TSource item in list) {
				AOCCElementData elementData = (bridgeList.Count > p && p >= 0 && (object.Equals(bridgeList[p].SourceElement, item) || item == null)) ? bridgeList[p] : bridgeList.FirstOrDefault(dt => object.Equals(dt.SourceElement, item));
				p++;
				if (elementData == null) {
				} else {
					if (elementData.Exists) {
						Remove(elementData.ResultElement);
						for (int i = elementData.InSourceCollectionIndex; i < bridgeList.Count; i++)
							if (bridgeList[i].Exists) bridgeList[i].InResultCollectionIndex--;
					}
					for (int i = elementData.InSourceCollectionIndex; i < bridgeList.Count; i++) {
						bridgeList[i].InSourceCollectionIndex--;
					}
					bridgeList.Remove(elementData);
				}
			}
		}		
		protected override void OnAdd(int p, System.Collections.IList list) {
			if(Source == null || Selector == null) return;
			foreach(TSource item in list) {
				var index = p++;
				bridgeList.Insert(index,new AOCCElementData() { Exists = false, InResultCollectionIndex = -1, InSourceCollectionIndex = index, SourceElement = item, ResultElement = Selector(item) });
				for (int i = p; i < bridgeList.Count; i++)
					bridgeList[i].InSourceCollectionIndex++;
			}
			CheckStartItemAddition();
		}
		void CheckStartItemAddition() {
			if (addTimer.Return(x => x.IsEnabled, () => false) || bridgeList.Count(dt => !dt.Exists) == 0) return;
			if (addTimer == null) {
				addTimer = new DispatcherTimer(
					DispatcherPriority.Background,
					Dispatcher ?? Dispatcher.CurrentDispatcher);
				addTimer.Interval = new TimeSpan(SleepTime == -1 ? 0 : SleepTime);
				addTimerTickHandler = new Data.Utils.WeakEventHandler<AsyncObservableCollectionConverter<TSource, TResult>, EventArgs, EventHandler>(
					this,
					(t, o, a) => t.PerformAddItemAction(),
					(h, o) => ((DispatcherTimer)o).Stop(),
					(h) => h.OnEvent
					);
				addTimer.Tick += addTimerTickHandler.Handler;
			}
			if (SleepTime == Int32.MaxValue)
				return;
			addTimer.Start();
		}		
		void PerformAddItemAction() {
			if (bridgeList.Count(dt => !dt.Exists) != 0) {
				AOCCElementData elementData = forceAddItem.If(x => !x.Exists) ?? bridgeList.First(dt => !dt.Exists);
				forceAddItem = null;
				InsertResultElementForData(elementData);
			} else {
				addTimer.Stop();
			}
		}						
		private void InsertResultElementForData(AOCCElementData elementData) {
			int currentAdditionIndex = 0;
			for (int i = elementData.InSourceCollectionIndex - 1; i >= 0; i--) {
				if (bridgeList[i].Exists) {
					currentAdditionIndex = bridgeList[i].InResultCollectionIndex + 1;
					break;
				}
			}
			elementData.InResultCollectionIndex = currentAdditionIndex;
			elementData.Exists = true;
			for (int i = elementData.InSourceCollectionIndex + 1; i < bridgeList.Count; i++) {
				if (bridgeList[i].Exists) bridgeList[i].InResultCollectionIndex++;
			}
			Insert(elementData.InResultCollectionIndex, elementData.ResultElement);
		}
		public void ForceAdd(TSource sourceObject) {
			if(sourceObject == null) return;
			if(Source == null || Selector == null) return;
			forceAddItem = bridgeList.FirstOrDefault(dt => object.Equals(dt.SourceElement, sourceObject));
			if (forceAddItem != null)
				PerformAddItemAction();
		}
		public void Recreate() {
			OnReset();
		}
		public void Recreate(Predicate<TSource> predicate) {
			foreach(var item in bridgeList)
				if (predicate(item.SourceElement)) {
					Remove(item.ResultElement);
					item.InResultCollectionIndex = -1;
					item.Exists = false;
				}
			CheckStartItemAddition();
		}
		public void ForceAddNextItem() {
			PerformAddItemAction();
		}
		internal class AOCCElementData {
			public bool Exists { get; set; }
			public TSource SourceElement { get; set; }
			public TResult ResultElement { get; set; }
			public int InSourceCollectionIndex { get; set; }
			public int InResultCollectionIndex { get; set; }
		}
	}
	class CanExecuteCommandWeakEventHandler : DevExpress.Data.Utils.WeakEventHandler<object, EventArgs, EventHandler> {
		static Action<DevExpress.Data.Utils.WeakEventHandler<object, EventArgs, EventHandler>, object> onDetachAction = (h, o) => {
			ICommand command = o as ICommand;
			if(command != null)
				command.CanExecuteChanged -= h.Handler;
		};
		static Func<DevExpress.Data.Utils.WeakEventHandler<object, EventArgs, EventHandler>, EventHandler> createHandlerFunction = h => h.OnEvent;
		public CanExecuteCommandWeakEventHandler(object owner, Action<object, object, EventArgs> onEventAction)
			: base(owner, onEventAction, onDetachAction, createHandlerFunction) {
		}
	}
#if DEBUGTEST
	public class DXBarsPerformanceHelper {
	}
#endif
	public class CommandSourceHelper {
		readonly CanExecuteCommandWeakEventHandler canExecuteChangedHanler;
		bool canExecute;
		readonly ICommandSource commandSource;
		ICommand command;
		public bool HasCommand { get { return command != null; } }
		public bool CanExecute {
			get { return canExecute; }
			private set {
				if (canExecute == value)
					return;
				canExecute = value;
				if (CanExecuteChanged == null)
					return;
				CanExecuteChanged(this, new EventArgs());
			}
		}
		public event EventHandler CanExecuteChanged;
		public CommandSourceHelper(ICommandSource commandSource) {
			this.canExecute = true;
			this.commandSource = commandSource;
			this.canExecuteChangedHanler = new CanExecuteCommandWeakEventHandler(this,
			   (item, sender, e) => {
				   CommandSourceHelper helper = item as CommandSourceHelper;
				   helper.OnCanExecuteChanged(sender, e);
			   });
			Update();
		}
		public void Update() {
			var newCommand = commandSource.Command;
			if (newCommand == command) {
				UpdateCanExecute();
				return;
			}				
			if (command != null) {
				command.CanExecuteChanged -= canExecuteChangedHanler.Handler;
			}
			if (newCommand != null) {
				newCommand.CanExecuteChanged += canExecuteChangedHanler.Handler;
			}
			command = newCommand;
			UpdateCanExecute();
		}
		public void Execute() {
			if (CanExecute == false)
				return;
			var routedCommand = command as RoutedCommand;
			if (routedCommand != null) {
				routedCommand.Execute(commandSource.CommandParameter, commandSource.CommandTarget);
				return;
			}
			command.Execute(commandSource.CommandParameter);
		}
		void OnCanExecuteChanged(object sender, EventArgs e) {
			UpdateCanExecute();
		}
		void UpdateCanExecute() {
			if (command == null) {
				CanExecute = true;
				return;
			}
			var routedCommand = command as RoutedCommand;
			if (routedCommand != null) {
				CanExecute = routedCommand.CanExecute(commandSource.CommandParameter, commandSource.CommandTarget);
				return;
			}
			CanExecute = command.CanExecute(commandSource.CommandParameter);
		}
	}
}
