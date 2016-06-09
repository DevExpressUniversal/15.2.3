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

using System.Collections.Generic;
using System.Windows.Controls;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.IO;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Bars.Themes;
using DevExpress.Xpf.Bars.Customization;
using DevExpress.Xpf.Bars.Helpers;
using DevExpress.Xpf.Utils;
using DevExpress.Utils;
using System;
using DevExpress.Xpf.Bars.Automation;
using System.Windows.Data;
using System.Text;
using System.Reflection;
using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Editors.Helpers;
using System.Linq;
using DevExpress.Xpf.Bars.Native;
namespace DevExpress.Xpf.Bars {
	public abstract class SLBarItemLinkControlBase : Control {
		public bool IsMouseLeftButtonPressed { get { return Mouse.LeftButton == MouseButtonState.Pressed; } }
	}
	public static class TransformExtension {
		public static Transform Identity { get { return Transform.Identity; } }		
	}
	public abstract class BarItemLinkControlBase : SLBarItemLinkControlBase {
		#region static
		public static readonly DependencyProperty ContainerTypeProperty;
		public static readonly DependencyProperty SpacingModeProperty;
		static BarItemLinkControlBase() {
			ContainerTypeProperty = DependencyPropertyManager.Register("ContainerType", typeof(LinkContainerType), typeof(BarItemLinkControlBase), new FrameworkPropertyMetadata(LinkContainerType.None, new PropertyChangedCallback(OnContainerTypePropertyChanged)));
			SpacingModeProperty = DependencyPropertyManager.Register("SpacingMode", typeof(SpacingMode), typeof(BarItemLinkControlBase), new FrameworkPropertyMetadata(SpacingMode.Mouse, FrameworkPropertyMetadataOptions.AffectsMeasure, (d, e) => ((BarItemLinkControlBase)d).OnSpacingModeChanged((SpacingMode)e.OldValue)));
			KeyboardNavigation.ControlTabNavigationProperty.OverrideMetadata(typeof(BarItemLinkControlBase), new FrameworkPropertyMetadata(KeyboardNavigationMode.None));
			KeyboardNavigation.DirectionalNavigationProperty.OverrideMetadata(typeof(BarItemLinkControlBase), new FrameworkPropertyMetadata(KeyboardNavigationMode.None));
			KeyboardNavigation.TabNavigationProperty.OverrideMetadata(typeof(BarItemLinkControlBase), new FrameworkPropertyMetadata(KeyboardNavigationMode.None));
		}
		protected static void OnContainerTypePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((BarItemLinkControlBase)d).OnContainerTypeChanged((LinkContainerType)e.OldValue);
		}
		#endregion
		#region dep props
		public LinkContainerType ContainerType {
			get { return (LinkContainerType)GetValue(ContainerTypeProperty); }
			set { SetValue(ContainerTypeProperty, value); }
		}
		public SpacingMode SpacingMode {
			get { return (SpacingMode)GetValue(SpacingModeProperty); }
			set { SetValue(SpacingModeProperty, value); }
		}
		#endregion
		BarItemLinkBase link;
		protected BarItemLinkControlBase() {
			Loaded += new RoutedEventHandler(OnLoaded);
			Unloaded += new RoutedEventHandler(OnUnloaded);
			IsTabStop = false;
		}
		protected virtual void OnLoaded(object sender, System.Windows.RoutedEventArgs e) {
			FrameworkElementHelper.SetIsLoaded(this, true);
			if (LinkBase != null) {
				UpdateByContainerType(ContainerType);
				LinkBase.OnLinkControlLoaded(sender, new BarItemLinkControlLoadedEventArgs(this.LinkInfo.With(x => x.Item), this.LinkInfo.With(x => x.Link), this));
			}				
		}
		protected virtual void OnUnloaded(object sender, System.Windows.RoutedEventArgs e) {
			FrameworkElementHelper.SetIsLoaded(this, false);			
		}
		protected BarItemLinkControlBase(BarItemLinkBase link) :this() {
			this.link = link;		
		}
		public BarItemLinkBase LinkBase { get { return link; } internal set { link = value; } }
		private BarItemLinkInfo linkInfoCore = null;
		public BarItemLinkInfo LinkInfo {
			get { return linkInfoCore; }
			set {
				if(linkInfoCore == value) return;
				BarItemLinkInfo oldValue = linkInfoCore;
				linkInfoCore = value;
				OnLinkInfoChanged(oldValue);
			}
		}
		public LinksControl LinksControl { get { return LinkInfo == null ? null : LinkInfo.LinksControl; } }
		protected virtual void OnLinkInfoChanged(BarItemLinkInfo oldValue) {
		}	  
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public int PageGroupRow { get; set; }
		BarContainerControl container;
		public BarContainerControl Container {
			get { return container; }
			set {
				if(Container == value)
					return;
				container = value;
				OnContainerChanged();
			}
		}
		protected virtual void OnContainerChanged() {
			UpdateOrientation();
		}
		protected internal double RowHeight { get; internal set; }
		protected internal int RowIndex { get; internal set; }
		protected internal virtual bool ProcessKeyDown(KeyEventArgs e) {
			return false;
		}
		protected internal virtual bool ProcessKeyUp(KeyEventArgs e) {
			return false;
		}
		protected internal virtual void OnClear() {			
			LinkInfo = null;
			Container = null;
			LinkBase = null;
			DataContext = null;
		}
		protected internal virtual void UpdateByContainerType(LinkContainerType type) {
			UpdateStyleByContainerType(type);
			UpdateTemplateByContainerType(type);
			UpdateLayoutByContainerType(type);
		}
		protected internal virtual void UpdateOrientation() {
		}
		protected internal virtual void UpdateStyleByContainerType(LinkContainerType type) {
		}
		protected internal virtual void UpdateTemplateByContainerType(LinkContainerType type) {
		}
		protected virtual void UpdateLayoutByContainerType(LinkContainerType type) {
		}
		protected internal virtual void OnMaxGlyphSizeChanged(Size MaxGlyphSize) {			
		}		
		protected virtual void OnSpacingModeChanged(SpacingMode oldValue) {
		}
		protected virtual bool IsDisabledByItem() { return (LinkBase != null && LinkInfo != null) ? !LinkBase.CalculateIsEnabled(LinkInfo) : false; }
		protected override bool IsEnabledCore { get { return !IsDisabledByItem(); } }
		protected internal virtual void UpdateIsEnabled() {
			CoerceValue(IsEnabledProperty);
		}
		protected internal virtual void UpdateVisibility() {
			if(LinkBase != null && LinkInfo != null)
				Visibility = LinkBase.CalculateVisibility(LinkInfo) ? Visibility.Visible : Visibility.Collapsed;		   
		}
		protected internal virtual void UpdateDataContext() {
			DataContext = GetDataContext();
		}
		protected internal virtual void UpdateVerticalAlignment() {
			VerticalAlignment = GetVerticalAlignment();
		}
		protected virtual VerticalAlignment GetVerticalAlignment() {
			return LinkBase == null ? VerticalAlignment.Stretch : LinkBase.VerticalAlignment;
		}
		protected virtual object GetDataContext() {
			if(LinkBase != null && LinkBase.OverrideItemDataContext)
				DataContext = LinkBase.DataContext;
			return null;
		}
		protected virtual internal void UpdateActualProperties() {
			UpdateDataContext();
			UpdateVisibility();
			UpdateIsEnabled();
			UpdateVerticalAlignment();
		}
		protected virtual void OnContainerTypeChanged(LinkContainerType oldValue) {
			UpdateByContainerType(ContainerType);
		}
		protected internal virtual void OnCustomizationModeChanged() {
			UpdateVisibility();
			UpdateIsEnabled();
		}
		protected virtual ResourceDictionary GetCustomResources() {
			if(LinkBase != null)
				return LinkBase.CustomResources;
			return null;
		}
		protected internal virtual bool GetIsSelectable() {
			return IsEnabled
				&& LinkInfo.Item.Return(x => x.IsVisible, () => true)
				&& LinkInfo.Link.Return(x => x.IsVisible, () => true)
				&& !LinkInfo.IsHidden();
		}
		protected internal virtual INavigationOwner GetBoundOwner() { return null; }
	}
	public class BarItemLinkControl : BarItemLinkControlBase, ISupportDragDrop {
		#region static
		public static readonly DependencyProperty ContentProperty;
		public static readonly DependencyProperty ActualBarItemDisplayModeProperty;
		protected static readonly DependencyPropertyKey ActualBarItemDisplayModePropertyKey;
		public static readonly DependencyProperty ActualKeyGestureTextProperty;
		protected static readonly DependencyPropertyKey ActualKeyGestureTextPropertyKey;
		public static readonly DependencyProperty ActualDescriptionProperty;
		protected static readonly DependencyPropertyKey ActualDescriptionPropertyKey;
		public static readonly DependencyProperty ActualContentTemplateProperty;
		protected static readonly DependencyPropertyKey ActualContentTemplatePropertyKey;
		public static readonly DependencyProperty ActualContentTemplateSelectorProperty;
		protected static readonly DependencyPropertyKey ActualContentTemplateSelectorPropertyKey;
		public static readonly DependencyProperty ActualContentProperty;
		protected static readonly DependencyPropertyKey ActualContentPropertyKey;
		protected static readonly DependencyPropertyKey ActualShowGlyphPropertyKey;
		public static readonly DependencyProperty ActualShowGlyphProperty;
		protected static readonly DependencyPropertyKey ActualShowArrowPropertyKey;
		public static readonly DependencyProperty ActualShowArrowProperty;
		protected static readonly DependencyPropertyKey ActualIsArrowEnabledPropertyKey;
		public static readonly DependencyProperty ActualIsArrowEnabledProperty;
		protected static readonly DependencyPropertyKey ActualIsContentEnabledPropertyKey;
		public static readonly DependencyProperty ActualIsContentEnabledProperty;
		protected static readonly DependencyPropertyKey ActualShowContentPropertyKey;
		public static readonly DependencyProperty ActualShowContentProperty;
		protected static readonly DependencyPropertyKey ActualShowDescriptionPropertyKey;
		public static readonly DependencyProperty ActualShowDescriptionProperty;
		public static readonly DependencyProperty ActualGlyphProperty;
		public static readonly DependencyProperty ToolTipGlyphProperty;
		static readonly DependencyPropertyKey ActualGlyphPropertyKey;
		static readonly DependencyPropertyKey ToolTipGlyphPropertyKey;
		public static readonly DependencyProperty ActualGlyphAlignmentProperty;
		internal static readonly DependencyPropertyKey ActualGlyphAlignmentPropertyKey;
		public static readonly DependencyProperty HasGlyphProperty;
		static readonly DependencyPropertyKey HasGlyphPropertyKey;
		public static readonly DependencyProperty IsLargeGlyphProperty;
		static readonly DependencyPropertyKey IsLargeGlyphPropertyKey;
		[IgnoreDependencyPropertiesConsistencyCheckerAttribute]
		protected static readonly DependencyProperty IsSelectedProperty;
		static readonly DependencyPropertyKey IsHighlightedPropertyKey;
		public static readonly DependencyProperty IsHighlightedProperty;
		internal static readonly RoutedEvent IsSelectedChangedEvent = null;
		internal static readonly RoutedEvent IsHighlightedChangedEvent = null;
		public static readonly DependencyProperty IsPressedProperty;
		public static readonly RoutedEvent ClickEvent;
		public static readonly DependencyProperty IsOnBarProperty;
		static readonly DependencyPropertyKey IsOnBarPropertyKey;
		protected static readonly DependencyPropertyKey ShowCustomizationBorderPropertyKey;
		public static readonly DependencyProperty ShowCustomizationBorderProperty;
		protected static readonly DependencyPropertyKey ShowHotBorderPropertyKey;
		public static readonly DependencyProperty ShowHotBorderProperty;
		protected static readonly DependencyPropertyKey ShowPressedBorderPropertyKey;
		public static readonly DependencyProperty ShowPressedBorderProperty;		
		static readonly DependencyPropertyKey ShowDescriptionPropertyKey;
		public static readonly DependencyProperty ShowDescriptionProperty;
		static readonly DependencyPropertyKey ShowKeyGesturePropertyKey;
		public static readonly DependencyProperty ShowKeyGestureProperty;
		static readonly DependencyPropertyKey IsRibbonStyleLargePropertyKey;
		public static readonly DependencyProperty IsRibbonStyleLargeProperty;
		protected static readonly DependencyPropertyKey ActualGlyphTemplatePropertyKey;
		public static readonly DependencyProperty ActualGlyphTemplateProperty;
		public static readonly DependencyProperty GlyphProperty;
		public static readonly DependencyProperty ActualAllowGlyphThemingProperty;
		protected static readonly DependencyPropertyKey ActualAllowGlyphThemingPropertyKey;
		protected static readonly DependencyPropertyKey ActualSectorIndexPropertyKey;
		public static readonly DependencyProperty ActualSectorIndexProperty;
		protected static readonly DependencyPropertyKey ActualIsCheckedPropertyKey;
		public static readonly DependencyProperty ActualIsCheckedProperty;
		protected static readonly DependencyPropertyKey ActualIsHoverEnabledPropertyKey;
		public static readonly DependencyProperty ActualIsHoverEnabledProperty;
		public static readonly DependencyProperty RotateWhenVerticalProperty;
		public static readonly DependencyProperty OrientationProperty;		
		static BarItemLinkControl() {
			OrientationProperty = DependencyPropertyManager.Register("Orientation", typeof(Orientation), typeof(BarItemLinkControl), new FrameworkPropertyMetadata(Orientation.Horizontal));
			RotateWhenVerticalProperty = DependencyPropertyManager.Register("RotateWhenVertical", typeof(bool), typeof(BarItemLinkControl), new FrameworkPropertyMetadata(true));
			DevExpress.Xpf.Bars.Automation.NavigationAutomationPeersCreator.Default.RegisterObject(typeof(BarItemLinkControl), typeof(BarItemLinkControlAutomationPeer), owner => new BarItemLinkControlAutomationPeer((BarItemLinkControl)owner));
			IsSelectedProperty = Selector.IsSelectedProperty.AddOwner(typeof(BarItemLinkControl), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, new PropertyChangedCallback(OnIsSelectedPropertyChanged)));
			VisibilityProperty.OverrideMetadata(typeof(BarItemLinkControl), new FrameworkPropertyMetadata(null, new CoerceValueCallback(OnVisibilityCoerce)));
			EventManager.RegisterClassHandler(typeof(BarItemLinkControl), AccessKeyManager.AccessKeyPressedEvent, new AccessKeyPressedEventHandler(OnAccessKeyPressed));
			ContentProperty = DependencyPropertyManager.Register("Content", typeof(object), typeof(BarItemLinkControl), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnContentPropertyChanged)));
			IsSelectedChangedEvent = EventManager.RegisterRoutedEvent("IsSelectedChanged", RoutingStrategy.Bubble, typeof(RoutedPropertyChangedEventHandler<bool>), typeof(BarItemLinkControl));
			IsHighlightedChangedEvent = EventManager.RegisterRoutedEvent("IsHighlightedChanged", RoutingStrategy.Bubble, typeof(RoutedPropertyChangedEventHandler<bool>), typeof(BarItemLinkControl));
			ClickEvent = EventManager.RegisterRoutedEvent("Click", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(BarItemLink));
			IsHighlightedPropertyKey = DependencyPropertyManager.RegisterReadOnly("IsHighlighted", typeof(bool), typeof(BarItemLinkControl), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, new PropertyChangedCallback(OnIsHighlightedPropertyChanged)));
			IsHighlightedProperty = IsHighlightedPropertyKey.DependencyProperty;
			IsPressedProperty = DependencyPropertyManager.Register("IsPressed", typeof(bool), typeof(BarItemLinkControl), new FrameworkPropertyMetadata(false, (d, e) => ((BarItemLinkControl)d).OnIsPressedChanged(e), (d, e) => ((BarItemLinkControl)d).OnIsPressedCoerce(e)));
			ActualAllowGlyphThemingPropertyKey = DependencyProperty.RegisterReadOnly("ActualAllowGlyphTheming", typeof(bool), typeof(BarItemLinkControl), new PropertyMetadata(false));
			ActualAllowGlyphThemingProperty = ActualAllowGlyphThemingPropertyKey.DependencyProperty;
			ActualShowArrowPropertyKey = DependencyPropertyManager.RegisterReadOnly("ActualShowArrow", typeof(bool), typeof(BarItemLinkControl), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsParentMeasure, new PropertyChangedCallback(OnActualShowArrowPropertyChanged)));
			ActualShowArrowProperty = ActualShowArrowPropertyKey.DependencyProperty;
			ActualIsArrowEnabledPropertyKey = DependencyPropertyManager.RegisterReadOnly("ActualIsArrowEnabled", typeof(bool), typeof(BarItemLinkControl), new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsParentMeasure));
			ActualIsArrowEnabledProperty = ActualIsArrowEnabledPropertyKey.DependencyProperty;
			ActualIsContentEnabledPropertyKey = DependencyPropertyManager.RegisterReadOnly("ActualIsContentEnabled", typeof(bool), typeof(BarItemLinkControl), new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsParentMeasure, new PropertyChangedCallback((d,e)=>((BarItemLinkControl)d).OnActualIsContentEnabledChanged((bool)e.OldValue))));
			ActualIsContentEnabledProperty = ActualIsContentEnabledPropertyKey.DependencyProperty;
			ActualShowGlyphPropertyKey = DependencyPropertyManager.RegisterReadOnly("ActualShowGlyph", typeof(bool), typeof(BarItemLinkControl), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsParentMeasure, new PropertyChangedCallback(OnActualShowGlyphPropertyChanged)));
			ActualShowGlyphProperty = ActualShowGlyphPropertyKey.DependencyProperty;
			ActualShowContentPropertyKey = DependencyPropertyManager.RegisterReadOnly("ActualShowContent", typeof(bool), typeof(BarItemLinkControl), new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsParentMeasure, new PropertyChangedCallback(OnActualShowContentPropertyChanged)));
			ActualShowContentProperty = ActualShowContentPropertyKey.DependencyProperty;
			ActualShowDescriptionPropertyKey = DependencyPropertyManager.RegisterReadOnly("ActualShowDescription", typeof(bool), typeof(BarItemLinkControl), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsParentMeasure));
			ActualShowDescriptionProperty = ActualShowDescriptionPropertyKey.DependencyProperty;
			IsOnBarPropertyKey = DependencyPropertyManager.RegisterReadOnly("IsOnBar", typeof(bool), typeof(BarItemLinkControl), new FrameworkPropertyMetadata(false));
			IsOnBarProperty = IsOnBarPropertyKey.DependencyProperty;
			ActualGlyphPropertyKey = DependencyPropertyManager.RegisterReadOnly("ActualGlyph", typeof(ImageSource), typeof(BarItemLinkControl), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsParentMeasure, new PropertyChangedCallback(OnActualGlyphPropertyChanged)));
			ActualGlyphProperty = ActualGlyphPropertyKey.DependencyProperty;
			ToolTipGlyphPropertyKey = DependencyPropertyManager.RegisterReadOnly("ToolTipGlyph", typeof(ImageSource), typeof(BarItemLinkControl), new FrameworkPropertyMetadata(null));
			ToolTipGlyphProperty = ToolTipGlyphPropertyKey.DependencyProperty;
			HasGlyphPropertyKey = DependencyPropertyManager.RegisterReadOnly("HasGlyph", typeof(bool), typeof(BarItemLinkControl), new FrameworkPropertyMetadata(false, new PropertyChangedCallback(OnHasGlyphPropertyChanged)));
			HasGlyphProperty = HasGlyphPropertyKey.DependencyProperty;
			IsLargeGlyphPropertyKey = DependencyPropertyManager.RegisterReadOnly("IsLargeGlyph", typeof(bool), typeof(BarItemLinkControl), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsParentMeasure, new PropertyChangedCallback(OnIsLargeGlyphPropertyChanged)));
			IsLargeGlyphProperty = IsLargeGlyphPropertyKey.DependencyProperty;
			ShowCustomizationBorderPropertyKey = DependencyPropertyManager.RegisterReadOnly("ShowCustomizationBorder", typeof(bool), typeof(BarItemLinkControl), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsParentMeasure, new PropertyChangedCallback(UpdateItemBorderVisualState)));
			ShowCustomizationBorderProperty = ShowCustomizationBorderPropertyKey.DependencyProperty;
			ShowHotBorderPropertyKey = DependencyPropertyManager.RegisterReadOnly("ShowHotBorder", typeof(bool), typeof(BarItemLinkControl), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsMeasure, new PropertyChangedCallback(UpdateItemBorderVisualState), new CoerceValueCallback((d, e) => ((BarItemLinkControl)d).OnShowHotBorderCoerce(e))));
			ShowHotBorderProperty = ShowHotBorderPropertyKey.DependencyProperty;
			ShowPressedBorderPropertyKey = DependencyPropertyManager.RegisterReadOnly("ShowPressedBorder", typeof(bool), typeof(BarItemLinkControl), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsMeasure, new PropertyChangedCallback(UpdateItemBorderVisualState)));
			ShowPressedBorderProperty = ShowPressedBorderPropertyKey.DependencyProperty;
			ActualGlyphAlignmentPropertyKey = DependencyPropertyManager.RegisterReadOnly("ActualGlyphAlignment", typeof(Dock), typeof(BarItemLinkControl), new FrameworkPropertyMetadata(Dock.Left, FrameworkPropertyMetadataOptions.None, new PropertyChangedCallback(OnActualGlyphAlignmentPropertyChanged)));
			ActualGlyphAlignmentProperty = ActualGlyphAlignmentPropertyKey.DependencyProperty;
			ShowDescriptionPropertyKey = DependencyPropertyManager.RegisterReadOnly("ShowDescription", typeof(bool), typeof(BarItemLinkControl), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsMeasure, new PropertyChangedCallback(OnShowDescriptionPropertyChanged)));
			ShowDescriptionProperty = ShowDescriptionPropertyKey.DependencyProperty;
			ShowKeyGesturePropertyKey = DependencyPropertyManager.RegisterReadOnly("ShowKeyGesture", typeof(bool), typeof(BarItemLinkControl), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsMeasure, new PropertyChangedCallback(OnShowKeyGesturePropertyChanged)));
			ShowKeyGestureProperty = ShowKeyGesturePropertyKey.DependencyProperty;
			IsRibbonStyleLargePropertyKey = DependencyPropertyManager.RegisterReadOnly("IsRibbonStyleLarge", typeof(bool), typeof(BarItemLinkControl), new FrameworkPropertyMetadata(false, new PropertyChangedCallback(OnIsRibbonStyleLargeChanged)));
			IsRibbonStyleLargeProperty = IsRibbonStyleLargePropertyKey.DependencyProperty;
			ActualContentPropertyKey = DependencyPropertyManager.RegisterReadOnly("ActualContent", typeof(object), typeof(BarItemLinkControl), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsParentMeasure, new PropertyChangedCallback(OnActualContentPropertyChanged)));
			ActualContentProperty = ActualContentPropertyKey.DependencyProperty;
			ActualContentTemplateSelectorPropertyKey = DependencyPropertyManager.RegisterReadOnly("ActualContentTemplateSelector", typeof(DataTemplateSelector), typeof(BarItemLinkControl), new FrameworkPropertyMetadata(null,FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsParentMeasure, new PropertyChangedCallback(OnActualContentTemplateSelectorPropertyChanged)));
			ActualContentTemplateSelectorProperty = ActualContentTemplateSelectorPropertyKey.DependencyProperty;
			ActualContentTemplatePropertyKey = DependencyPropertyManager.RegisterReadOnly("ActualContentTemplate", typeof(DataTemplate), typeof(BarItemLinkControl), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsParentMeasure, new PropertyChangedCallback(OnActualContentTemplatePropertyChanged)));
			ActualContentTemplateProperty = ActualContentTemplatePropertyKey.DependencyProperty;
			ActualDescriptionPropertyKey = DependencyPropertyManager.RegisterReadOnly("ActualDescription", typeof(object), typeof(BarItemLinkControl), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnActualDescriptionChanged)));
			ActualDescriptionProperty = ActualDescriptionPropertyKey.DependencyProperty;
			ActualKeyGestureTextPropertyKey = DependencyPropertyManager.RegisterReadOnly("ActualKeyGestureText", typeof(string), typeof(BarItemLinkControl), new FrameworkPropertyMetadata(string.Empty, new PropertyChangedCallback(OnActualKeyGestureTextChanged)));
			ActualKeyGestureTextProperty = ActualKeyGestureTextPropertyKey.DependencyProperty;
			ActualBarItemDisplayModePropertyKey = DependencyPropertyManager.RegisterReadOnly("ActualBarItemDisplayMode", typeof(BarItemDisplayMode), typeof(BarItemLinkControl), new FrameworkPropertyMetadata(BarItemDisplayMode.Default, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsParentMeasure));
			ActualBarItemDisplayModeProperty = ActualBarItemDisplayModePropertyKey.DependencyProperty;
			ActualGlyphTemplatePropertyKey = DependencyPropertyManager.RegisterReadOnly("ActualGlyphTemplate", typeof(DataTemplate), typeof(BarItemLinkControl), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsParentMeasure, (d, e) => { ((BarItemLinkControl)d).UpdateLayoutPanelGlyphTemplate(); }));
			ActualGlyphTemplateProperty = ActualGlyphTemplatePropertyKey.DependencyProperty;
			GlyphProperty = DependencyPropertyManager.Register("Glyph", typeof(ImageSource), typeof(BarItemLinkControl), new FrameworkPropertyMetadata(null,
			new PropertyChangedCallback(OnGlyphPropertyChanged)));
			ActualSectorIndexPropertyKey = DependencyPropertyManager.RegisterReadOnly("ActualSectorIndex", typeof(int), typeof(BarItemLinkControl), new FrameworkPropertyMetadata(-1, (d,e)=>((BarItemLinkControl)d).OnActualSectorIndexChanged()));
			ActualSectorIndexProperty = ActualSectorIndexPropertyKey.DependencyProperty;
			ActualIsCheckedPropertyKey = DependencyProperty.RegisterReadOnly("ActualIsChecked", typeof(bool?), typeof(BarItemLinkControl), new PropertyMetadata(false));
			ActualIsCheckedProperty = ActualIsCheckedPropertyKey.DependencyProperty;
			ActualIsHoverEnabledPropertyKey = DependencyProperty.RegisterReadOnly("ActualIsHoverEnabled", typeof(bool), typeof(BarItemLinkControl), new PropertyMetadata(true));
			ActualIsHoverEnabledProperty = ActualIsHoverEnabledPropertyKey.DependencyProperty;
		}		
		protected static void OnIsRibbonStyleLargeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((BarItemLinkControl)d).UpdateLayoutPanelSplitContent();
		}
		protected static void OnActualKeyGestureTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((BarItemLinkControl)d).UpdateLayoutPanelKeyGesture();
		}
		protected static void OnActualDescriptionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((BarItemLinkControl)d).UpdateLayoutPanelDescription();
		}
		protected static void OnActualContentPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((BarItemLinkControl)d).UpdateLayoutPanelContent();
		}
		protected static void OnContentPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((BarItemLinkControl)d).OnContentChanged((object)e.OldValue);
		}
		protected static void OnActualGlyphAlignmentPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((BarItemLinkControl)d).OnActualGlyphAlignmentChanged(e);
		}
		protected static void OnShowKeyGesturePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((BarItemLinkControl)d).OnShowKeyGestureChanged(e);
		}
		protected static void OnShowDescriptionPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((BarItemLinkControl)d).OnShowDescriptionChanged(e);
		}
		protected static void OnActualShowArrowPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((BarItemLinkControl)d).OnActualShowArrowChanged(e);
		}
		protected static void OnActualShowGlyphPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((BarItemLinkControl)d).OnActualShowGlyphChanged(e);
		}
		protected static void OnActualShowContentPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((BarItemLinkControl)d).OnActualShowContentChanged(e);
		}
		protected static void UpdateItemBorderVisualState(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((BarItemLinkControl)d).UpdateItemBorderAndContent();
			((BarItemLinkControl)d).UpdateItemVisualState();
		}
		protected static void OnIsLargeGlyphPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((BarItemLinkControl)d).OnIsLargeGlyphChanged(e);
		}
		protected static void OnIsSelectedPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((BarItemLinkControl)d).OnIsSelectedChanged(e);
		}
		protected static void OnIsHighlightedPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((BarItemLinkControl)d).OnIsHighlightedChanged(e);
		}
		protected static void OnActualContentTemplatePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((BarItemLinkControl)d).UpdateActualShowContent();
			((BarItemLinkControl)d).UpdateLayoutPanelContentTemplate();
		}
		protected static void OnActualContentTemplateSelectorPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((BarItemLinkControl)d).UpdateLayoutPanelContentTemplateSelector();
		}
		protected internal virtual object OnShowHotBorderCoerce(object obj) {
			return obj;
		}
		static object OnVisibilityCoerce(DependencyObject d, object baseValue) {
			return ((BarItemLinkControl)d).OnVisibilityCoerce(baseValue);
		}
		protected static void OnHasGlyphPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((BarItemLinkControl)d).OnHasGlyphChanged(e);
		}
		static void OnActualGlyphPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((BarItemLinkControl)d).OnActualGlyphChanged(e);
		}
		protected static void OnGlyphPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((BarItemLinkControl)d).OnGlyphChanged((ImageSource)e.OldValue);
		}
		private static ImageSource defaultGlyphCore = null;
		protected static ImageSource DefaultGlyph {
			get {
				if (defaultGlyphCore == null) {
					defaultGlyphCore = ImageHelper.CreateImageFromCoreEmbeddedResource("Bars.Images.Default_16x16.png");
					defaultGlyphCore.Freeze();
				}
				return defaultGlyphCore;
			}
		}
		private static ImageSource defaultLargeGlyphCore = null;
		protected static ImageSource DefaultLargeGlyph {
			get {
				if (defaultLargeGlyphCore == null) {
					defaultLargeGlyphCore = ImageHelper.CreateImageFromCoreEmbeddedResource("Bars.Images.Default_32x32.png");
					defaultLargeGlyphCore.Freeze();
				}
				return defaultLargeGlyphCore;
			}
		}
		DevExpress.Xpf.Bars.Native.WeakList<ValueChangedEventHandler<bool>> handlersWeakIsHighlightedChanged = new Bars.Native.WeakList<ValueChangedEventHandler<bool>>();
		public event ValueChangedEventHandler<bool> WeakIsHighlightedChanged {
			add { handlersWeakIsHighlightedChanged.Add(value); }
			remove { handlersWeakIsHighlightedChanged.Remove(value); }
		}
		void RaiseWeakIsHighlightedChanged(ValueChangedEventArgs<bool> args) {
			foreach (ValueChangedEventHandler<bool> e in handlersWeakIsHighlightedChanged)
				e(this, args);
		}
		#endregion
		#region dep props
		public Orientation Orientation {
			get { return (Orientation)GetValue(OrientationProperty); }
			set { SetValue(OrientationProperty, value); }
		}
		public bool RotateWhenVertical {
			get { return (bool)GetValue(RotateWhenVerticalProperty); }
			set { SetValue(RotateWhenVerticalProperty, value); }
		}
		public bool? ActualIsChecked {
			get { return (bool?)GetValue(ActualIsCheckedProperty); }
			protected set { SetValue(ActualIsCheckedPropertyKey, value); }
		}
		public bool ActualIsHoverEnabled {
			get { return (bool)GetValue(ActualIsHoverEnabledProperty); }
			protected set { SetValue(ActualIsHoverEnabledPropertyKey, value); }
		}
		public int ActualSectorIndex {
			get { return (int)GetValue(ActualSectorIndexProperty); }
			protected set { this.SetValue(ActualSectorIndexPropertyKey, value); }
		}
		public object Content {
			get { return (object)GetValue(ContentProperty); }
			set { SetValue(ContentProperty, value); }
		}
		public bool IsSelected {
			get { return (bool)base.GetValue(IsSelectedProperty); }
			set { base.SetValue(IsSelectedProperty, value); }
		}
		public bool ShowDescription {
			get { return (bool)GetValue(ShowDescriptionProperty); }
			protected internal set { this.SetValue(ShowDescriptionPropertyKey, value); }
		}
		public bool ShowKeyGesture {
			get { return (bool)GetValue(ShowKeyGestureProperty); }
			protected internal set { this.SetValue(ShowKeyGesturePropertyKey, value); }
		}
		public bool IsRibbonStyleLarge {
			get { return (bool)GetValue(IsRibbonStyleLargeProperty); }
			protected internal set { this.SetValue(IsRibbonStyleLargePropertyKey, value); }
		}
		public bool IsHighlighted {
			get { return (bool)base.GetValue(IsHighlightedProperty); }
			protected internal set { this.SetValue(IsHighlightedPropertyKey, value); }
		}
		public bool IsPressed {
			get { return (bool)base.GetValue(IsPressedProperty); }
			set { base.SetValue(IsPressedProperty, value); }
		}
		public bool ActualAllowGlyphTheming {
			get { return (bool)GetValue(ActualAllowGlyphThemingProperty); }
			protected set { SetValue(ActualAllowGlyphThemingPropertyKey, value); }
		}
		public bool ActualShowArrow {
			get { return (bool)GetValue(ActualShowArrowProperty); }
			internal set { this.SetValue(ActualShowArrowPropertyKey, value); }
		}
		public bool ActualIsArrowEnabled {
			get { return (bool)GetValue(ActualIsArrowEnabledProperty); }
			protected set { SetValue(ActualIsArrowEnabledPropertyKey, value); }
		}
		public bool ActualIsContentEnabled {
			get { return (bool)GetValue(ActualIsContentEnabledProperty); }
			protected set { SetValue(ActualIsContentEnabledPropertyKey, value); }
		}
		public bool ActualShowGlyph {
			get { return (bool)GetValue(ActualShowGlyphProperty); }
			internal set { this.SetValue(ActualShowGlyphPropertyKey, value); }
		}
		public bool ActualShowContent {
			get { return (bool)GetValue(ActualShowContentProperty); }
			internal set { this.SetValue(ActualShowContentPropertyKey, value); }
		}
		public bool ActualShowDescription {
			get { return (bool)GetValue(ActualShowDescriptionProperty); }
			internal set { this.SetValue(ActualShowDescriptionPropertyKey, value); }
		}
		public Dock ActualGlyphAlignment {
			get { return (Dock)GetValue(ActualGlyphAlignmentProperty); }
			protected internal set { this.SetValue(ActualGlyphAlignmentPropertyKey, value); }
		}
		public ImageSource ActualGlyph {
			get { return (ImageSource)GetValue(ActualGlyphProperty); }
			protected internal set { this.SetValue(ActualGlyphPropertyKey, value); }
		}
		public ImageSource ToolTipGlyph {
			get { return (ImageSource)GetValue(ToolTipGlyphProperty); }
			protected internal set { this.SetValue(ToolTipGlyphPropertyKey, value); }
		}
		public event RoutedEventHandler Click {
			add { AddHandler(ClickEvent, value); }
			remove { RemoveHandler(ClickEvent, value); }
		}
		public event RoutedEventHandler IsSelectedChanged {
			add { AddHandler(IsSelectedChangedEvent, value); }
			remove { RemoveHandler(IsSelectedChangedEvent, value); }
		}
		public event RoutedEventHandler IsHighlightedChanged {
			add { AddHandler(IsHighlightedChangedEvent, value); }
			remove { RemoveHandler(IsHighlightedChangedEvent, value); }
		}
		[Browsable(false), ReadOnly(true)]
		public bool HasGlyph {
			get { return (bool)GetValue(HasGlyphProperty); }
			private set { this.SetValue(HasGlyphPropertyKey, value); }
		}
		public bool IsLargeGlyph {
			get { return (bool)GetValue(IsLargeGlyphProperty); }
			protected internal set { this.SetValue(IsLargeGlyphPropertyKey, value); }
		}
		[Browsable(false), ReadOnly(true)]
		public bool IsOnBar {
			get { return (bool)base.GetValue(IsOnBarProperty); }
			protected internal set { this.SetValue(IsOnBarPropertyKey, value); }
		}
		[Browsable(false), ReadOnly(true)]
		public bool ShowCustomizationBorder {
			get { return (bool)GetValue(ShowCustomizationBorderProperty); }
			protected internal set { this.SetValue(ShowCustomizationBorderPropertyKey, value); }
		}
		[Browsable(false), ReadOnly(true)]
		public bool ShowHotBorder {
			get { return (bool)GetValue(ShowHotBorderProperty); }
			protected internal set { this.SetValue(ShowHotBorderPropertyKey, value); }
		}
		[Browsable(false), ReadOnly(true)]
		public bool ShowPressedBorder {
			get { return (bool)GetValue(ShowPressedBorderProperty); }
			protected internal set { this.SetValue(ShowPressedBorderPropertyKey, value); }
		}
		public object ActualContent {
			get { return (object)GetValue(ActualContentProperty); }
			protected set { this.SetValue(ActualContentPropertyKey, value); }
		}
		public DataTemplateSelector ActualContentTemplateSelector {
			get { return (DataTemplateSelector)GetValue(ActualContentTemplateSelectorProperty); }
			protected set { this.SetValue(ActualContentTemplateSelectorPropertyKey, value); }
		}
		public DataTemplate ActualContentTemplate {
			get { return (DataTemplate)GetValue(ActualContentTemplateProperty); }
			protected set { this.SetValue(ActualContentTemplatePropertyKey, value); }
		}
		public object ActualDescription {
			get { return (object)GetValue(ActualDescriptionProperty); }
			protected set { this.SetValue(ActualDescriptionPropertyKey, value); }
		}
		public string ActualKeyGestureText {
			get { return (string)GetValue(ActualKeyGestureTextProperty); }
			protected set { this.SetValue(ActualKeyGestureTextPropertyKey, value); }
		}
		public BarItemDisplayMode ActualBarItemDisplayMode {
			get { return (BarItemDisplayMode)GetValue(ActualBarItemDisplayModeProperty); }
			protected set { this.SetValue(ActualBarItemDisplayModePropertyKey, value); }
		}
		public DataTemplate ActualGlyphTemplate {
			get { return (DataTemplate)GetValue(ActualGlyphTemplateProperty); }
			protected internal set { this.SetValue(ActualGlyphTemplatePropertyKey, value); }
		}
		public ImageSource Glyph {
			get { return (ImageSource)GetValue(GlyphProperty); }
			set { SetValue(GlyphProperty, value); }
		}
		#endregion
		#region props
		RibbonItemStyles currentRibbonStyleCore = RibbonItemStyles.Default;
		public RibbonItemStyles CurrentRibbonStyle {
			get { return currentRibbonStyleCore; }
			set {
				if (currentRibbonStyleCore == value)
					return;
				RibbonItemStyles oldValue = currentRibbonStyleCore;
				currentRibbonStyleCore = value;
				OnCurrentRibbonStyleChanged(oldValue);
			}
		}
		#endregion
		DragDropElementHelper dragDropHelper;
		public BarItemLink Link {
			get { return base.LinkBase as BarItemLink; }
			internal set { base.LinkBase = value; }
		}
		protected BarItem Item {
			get { return Link == null ? null : Link.Item; }
		}
		public BarManager Manager {
			get { return (LinksControl==null ? null : LinksControl.Manager); }
		}
		public BarItemLinkControl()
			: this(null) {
		}
		public BarItemLinkControl(BarItemLink link)
			: base(link) {
			Initialize();			
		}
		protected virtual void Initialize() {
			IsTabStop = false;
			IsEnabledChanged += new DependencyPropertyChangedEventHandler(OnIsEnabledChanged);			
		}
		protected override void OnGotKeyboardFocus(KeyboardFocusChangedEventArgs e) {
			base.OnGotKeyboardFocus(e);
			if (LayoutHelper.FindLayoutOrVisualParentObject(e.OriginalSource as DependencyObject, x => x is BarItemLinkControl) == this)
				NavigationTree.StartNavigation(LinkInfo);
		}
		#region IsEnabled logic
		protected override bool IsEnabledCore { get { return base.IsEnabledCore || (ContainerType == LinkContainerType.RadialMenu); } }
		bool IsEnabledPropertyAssigned(DependencyObject item) {
			if(item is UIElement)
				return item.IsPropertyAssigned(UIElement.IsEnabledProperty);
			else if(item is ContentElement)
				return item.IsPropertyAssigned(ContentElement.IsEnabledProperty);
			return false;
		}
		DependencyObject GetFirstDisabledParent(DependencyObject item) {
			DependencyObject currentItem = item;
			while(currentItem != null) {
				DependencyObject parent = LogicalTreeHelper.GetParent(currentItem);
				if(parent == null) return currentItem;
				if(GetObjectIsEnabled(parent))
					return currentItem;
				currentItem = parent;
			}
			return null;
		}
		bool GetObjectIsEnabled(DependencyObject element) {
			if(element is UIElement)
				return (bool)element.GetValue(UIElement.IsEnabledProperty);
			else if(element is ContentElement)
				return (bool)element.GetValue(ContentElement.IsEnabledProperty);
			return true;
		}
		protected bool IsDisabledByParentCommandCanExecuteOnly(bool includeSelfItem = false) {
			if(!IsDisabledByItem() || Item == null || ContainerType != LinkContainerType.RadialMenu) return false;
			DependencyObject visualParent = this.VisualParents().FirstOrDefault();
			if(IsEnabledPropertyAssigned(this)) return false; 
			if(visualParent != null && !GetObjectIsEnabled(visualParent)) return false; 
			bool disabledByCanExecuteViaLink = false;
			BarItem firstDisabledBarItem = null;
			DependencyObject firstDisabledParent = null;
			if(Link != null && !Link.IsEnabled) {
				if(IsEnabledPropertyAssigned(Link)) return false;
				firstDisabledParent = GetFirstDisabledParent(Link);
				if(!(firstDisabledParent is BarItem) && !(firstDisabledParent is BarItemLinkBase)) return false;
				if(firstDisabledParent is BarItem) {
					firstDisabledBarItem = firstDisabledParent as BarItem;
					if(firstDisabledBarItem.Command != null && !firstDisabledBarItem.CanExecute && !IsEnabledPropertyAssigned(firstDisabledBarItem))
						disabledByCanExecuteViaLink = true;
				}
			}
			if(Item.IsEnabled) return disabledByCanExecuteViaLink;
			firstDisabledParent = GetFirstDisabledParent(Item);
			if(!includeSelfItem && firstDisabledParent == Item) return false;
			if(!(firstDisabledParent is BarItem)) return false;
			firstDisabledBarItem = firstDisabledParent as BarItem;
			if(firstDisabledBarItem.Command == null || firstDisabledBarItem.CanExecute || IsEnabledPropertyAssigned(firstDisabledBarItem))
				return false;
			return true;
		}
		protected internal override void UpdateIsEnabled() {
			base.UpdateIsEnabled();
			if(ContainerType == LinkContainerType.RadialMenu) {
				UpdateActualIsContentEnabled();
				UpdateActualIsArrowEnabled();
			}
		}
		bool CommandCanExecute { get { return Item != null ? Item.CanExecute : true; } }
		protected virtual void UpdateActualIsContentEnabled() {
			ActualIsContentEnabled = (!IsDisabledByItem() && IsEnabled) ? true : IsDisabledByParentCommandCanExecuteOnly() && CommandCanExecute;
		}
		protected internal virtual void UpdateActualIsArrowEnabled() { ActualIsArrowEnabled = !IsDisabledByItem() && IsEnabled; }
		#endregion
		protected override void OnIsKeyboardFocusWithinChanged(DependencyPropertyChangedEventArgs e) {
			base.OnIsKeyboardFocusWithinChanged(e);
			if (Manager == null)
				return;		   
		}
		protected override Size MeasureOverride(Size availableSize) {
			DevExpress.Xpf.Core.ClearAutomationEventsHelper.ClearAutomationEvents();
			for(int i = 0; i < VisualTreeHelper.GetChildrenCount(this); i++) {
				var child = VisualTreeHelper.GetChild(this, 0) as UIElement;
				if(child == null)
					continue;
				child.InvalidateMeasure();
			}
			return base.MeasureOverride(availableSize);
		}
		protected object GetTemplateFromProvider(DependencyProperty prop, BarItemThemeKeys themeKeys) {
			object res = GetCustomResources() == null ? null : GetCustomResources()[ResourceHelper.CorrectResourceKey(new BarItemThemeKeyExtension() { ResourceKey = themeKeys })];
			if (res != null)
				return res;
			return GetValue(prop);
		}
		protected internal virtual void OnSourceSectorIndexChanged() {
			UpdateActualSectorIndex();
		}
		protected internal virtual void OnSourceBarItemDisplayModeChanged() {
			UpdateActualBarItemDisplayMode();
		}
		protected internal virtual void OnSourceContentTemplateSelectorChanged() {
			UpdateActualContentTemplateSelector();
		}
		protected virtual void UpdateActualShowArrow() {
			ActualShowArrow = false;
		}
		protected virtual void UpdateActualSectorIndex() {
			ActualSectorIndex = GetSectorIndex();
		}
		protected virtual void UpdateLayoutPanelMaxGlyphSize() {
			if (LayoutPanel == null)
				return;
			if (IsLinkControlInMenu && !IsLinkInRadialMenu) {
				LayoutPanel.MaxGlyphSize = maxGlyphSize;
			} else
				LayoutPanel.MaxGlyphSize = new Size();
		}
		protected internal virtual void UpdateActualBarItemDisplayMode() {
			ActualBarItemDisplayMode = GetBarItemDisplayMode();
			UpdateActualShowGlyph();
			UpdateActualShowContent();
		}
		protected virtual void UpdateActualContentTemplateSelector() {
			ActualContentTemplateSelector = GetContentTemplateSelector();
		}
		protected virtual int GetSectorIndex() {
			return Link == null ? -1 : ((Link.IsPropertyAssigned(BarItemLinkBase.SectorIndexProperty) || Item == null) ? Link.SectorIndex : Item.SectorIndex);
		}
		protected virtual DataTemplateSelector GetContentTemplateSelector() {
			return Item == null ? null : Item.ContentTemplateSelector;
		}
		protected virtual BarItemDisplayMode GetBarItemDisplayMode() {			
			if (Link != null && Link.BarItemDisplayMode != BarItemDisplayMode.Default)
				return Link.BarItemDisplayMode;
			if (Item != null && Item.BarItemDisplayMode != BarItemDisplayMode.Default)
				return Item.BarItemDisplayMode;
			var bc = (LinksControl as BarControl).Return(x => x.BarItemDisplayMode, () => BarItemDisplayMode.Default);
			if (bc != BarItemDisplayMode.Default)
				return bc;
			return (LinksControl as BarControl).With(x => x.DockInfo.Container).Return(x => x.BarItemDisplayMode, () => BarItemDisplayMode.Default);
		}
		protected virtual void OnContentChanged(object oldValue) {
			InitializeRibbonStyle();
			OnSourceContentChanged();
		}
		protected internal virtual void OnSourceContentTemplateChanged() {
			InitializeRibbonStyle();
			UpdateActualContentTemplate();
		}
		protected internal virtual void OnSourceGlyphChanged() {
			InitializeRibbonStyle();
			UpdateActualGlyph();
		}
		protected internal virtual void OnSourceLargeGlyphChanged() {
			InitializeRibbonStyle();
			UpdateActualGlyph();
		}
		protected internal virtual void OnSourceGlyphAlignmentChanged() {
			UpdateActualGlyphAlignment();
		}
		protected virtual void UpdateActualContentTemplate() {
			ActualContentTemplate = GetContentTemplate();
		}
		protected virtual void UpdateActualGlyphAlignment() {
			if (IsLinkInRibbon)
				UpdateActualGlyphAlignmentInRibbon();
			else
				UpdateActualGlyphAlignmentInBars();
		}
		protected virtual DataTemplate GetContentTemplate() {
			return Item == null ? null : Item.GetContentTemplate();
		}
		protected internal virtual void OnSourceDescriptionChanged() {
			UpdateActualDescription();
			UpdateShowDescription();
		}
		protected virtual void UpdateActualDescription() {
			ActualDescription = GetDescription();
		}
		protected virtual object GetDescription() {
			return Item == null ? String.Empty : Item.GetDescription();
		}
		protected internal virtual void OnSourceKeyGestureChanged() {
			UpdateActualKeyGestureText();
			UpdateShowKeyGesture();
		}
		protected virtual void UpdateActualKeyGestureText() {
			ActualKeyGestureText = GetKeyGestureText();
		}
		protected virtual string GetKeyGestureText() {
			if (Item != null && Item.KeyGesture != null)
				return Item.KeyGesture.GetDisplayStringForCulture(System.Globalization.CultureInfo.CurrentCulture);
			else
				return string.Empty;
		}
		protected override void OnLinkInfoChanged(BarItemLinkInfo oldValue) {
			base.OnLinkInfoChanged(oldValue);
			if (LinksControl is BarControl) {
				var bc = (BarControl)LinksControl;
				SetBinding(OrientationProperty, new Binding() { Path = new PropertyPath(BarControl.ContainerOrientationProperty), Source = bc });
				SetBinding(RotateWhenVerticalProperty, new Binding() { Path = new PropertyPath(Bar.RotateWhenVerticalProperty), Source = bc.Bar });
			}
			OnMaxGlyphSizeChanged(LinksControl.Return(x => x.MaxGlyphSize, () => new Size()));
		}
		protected override void OnLoaded(object sender, System.Windows.RoutedEventArgs e) {
			FrameworkElementHelper.SetIsLoaded(this, true);
			base.OnLoaded(sender, e);
			CreateDragDropHelper();
			if (Link != null && Link.Item != null)
				Link.Item.UpdateCanExecute();
			UpdateItemBorderAndContent();
			UpdateLayoutByContainerType(ContainerType);
			Link.Do(x => x.RaiseLinkControlLoaded());
		}
		protected override void OnUnloaded(object sender, RoutedEventArgs e) {
			base.OnUnloaded(sender, e);
			DragDropHelper.Do(x => ((BarDragDropElementHelper)x).BeginDestroy());
		}
		protected internal virtual void UpdateActualShowGlyph() {
			if (IsLinkInRibbon && !IsLinkControlInMenu)
				UpdateActualShowGlyphInRibbon();
			else
				UpdateActualShowGlyphInBars();
		}
		protected internal virtual void UpdateShowCustomizationBorder() {
			if (!IsLinkInRibbon) {
				if (Manager != null && !BarManagerCustomizationHelper.IsInCustomizationMode(this))
					ShowCustomizationBorder = false;
			}
		}
		protected internal virtual void UpdateActualShowContent() {
			if (!IsLinkInRibbon) {
				if (ActualBarItemDisplayMode == BarItemDisplayMode.Default && (ActualGlyph != null || ActualGlyphTemplate != null) && !IsLinkControlInMenu || IsInHorizontalHeader())
					ActualShowContent = false;
				else
					ActualShowContent = (ActualContent != null || ActualContentTemplate != null);
			} else {
				if (CurrentRibbonStyle == RibbonItemStyles.SmallWithoutText)
					ActualShowContent = false;
				else
					ActualShowContent = (ActualContent != null || ActualContentTemplate != null);
			}
		}
		protected virtual bool IsInHorizontalHeader() {
			BarItemMenuHeaderItemsControl lControl = LinksControl as BarItemMenuHeaderItemsControl;
			if (ContainerType != LinkContainerType.MenuHeader || lControl == null || lControl.MenuHeaderControl == null)
				return false;
			return lControl.MenuHeaderControl.ActualItemsOrientation == HeaderOrientation.Horizontal;
		}
		protected internal virtual void UpdateActualGlyph() {
			UpdateActualBarItemDisplayMode();
			if (IsLinkInRibbon && !IsLinkInApplicationMenu)
				UpdateActualGlyphInRibbon();
			else {
				UpdateActualGlyphInBars();
			}
			UpdateActualShowGlyph();
			UpdateActualShowContent();
		}
		protected internal virtual void UpdateActualGlyphTemplate() {
			ActualGlyphTemplate = GetGlyphTemplate();
		}
		DataTemplate GetGlyphTemplate() {
			if (Item != null)
				return Item.GlyphTemplate;
			return (DataTemplate)null;
		}
		GlyphSize GetHolderGlyphSizeInRadialMenu(ILinksHolder holder) {
			ILinksHolder current = holder;
			while(current != null && current.ItemsGlyphSize == GlyphSize.Default) {
				current = RadialMenuTreeHelper.GetParent(current);
			}
			return current == null ? GlyphSize.Small : current.ItemsGlyphSize;
		}
		protected virtual GlyphSize GetHolderGlyphSize(BarItemLinkInfo info) {
			if (info == null || info.LinkBase == null || info.LinkBase.Links == null)
				return GlyphSize.Default;
			ILinksHolder holder = info.LinkBase.Links.Holder;
			if (holder == null)
				return GlyphSize.Default;
			if(ContainerType == LinkContainerType.RadialMenu) {
				return GetHolderGlyphSizeInRadialMenu(holder);
			}
			if (holder.ItemsGlyphSize != GlyphSize.Default)
				return holder.ItemsGlyphSize;
			GlyphSize sz = holder.GetDefaultItemsGlyphSize(ContainerType);
			if (sz != GlyphSize.Default)
				return sz;
			GlyphSize result = BarNameScope.GetService<ICustomizationService>(this).CustomizationHelper.Return(x => IsLinkControlInMenu ? x.MenuGlyphSize : x.ToolbarGlyphSize, () => GlyphSize.Default);
			if (result == GlyphSize.Default && IsLinkControlInMenu)
				result = GlyphSize.Small;
			return result;
		}
		protected virtual void UpdateActualGlyphInBars() {
			GlyphSize glyphSize = GetGlyphSize();
			if (glyphSize == GlyphSize.Default)
				glyphSize = GetHolderGlyphSize(LinkInfo);
			if (glyphSize == GlyphSize.Small) {
				SetActualGlyph(GetGlyph(), false);
			} else if (glyphSize == GlyphSize.Large) {
				SetActualGlyph(GetLargeGlyph() ?? GetGlyph(), true);
			} else {
				if (GetLargeGlyph() != null)
					SetActualGlyph(GetLargeGlyph(), true);
				else
					SetActualGlyph(GetGlyph(), false);
			}
			UpdateActualShowGlyph();
		}
		public virtual void SetExpandMode(BarPopupExpandMode expandMode) { }
		protected void SetActualGlyph(ImageSource actualGlyph, bool isLargeGlyph) {
			ActualGlyph = actualGlyph;
			IsLargeGlyph = isLargeGlyph;
			HasGlyph = actualGlyph != null;
		}
		protected virtual ImageSource GetLargeGlyph() {
			if (Item == null)
				return null;
			return Item.GetLargeGlyph();
		}
		protected virtual ImageSource GetGlyph() {
			if (Glyph != null)
				return Glyph;
			if (Item == null)
				return null;
			return Item.GetGlyph();
		}
		protected virtual void UpdateActualGlyphInRibbon() {
			if (CurrentRibbonStyle == RibbonItemStyles.Large || CurrentRibbonStyle == RibbonItemStyles.Default) {
				SetActualGlyph(GetLargeGlyph() ?? GetGlyph() ?? DefaultLargeGlyph, true);
				IsRibbonStyleLarge = true;
			} else {
				if (IsLinkInApplicationMenu)
					SetActualGlyph(GetGlyph(), true);
				else if(ContainerType == LinkContainerType.RibbonQuickAccessToolbar || ContainerType == LinkContainerType.RibbonQuickAccessToolbarFooter || ContainerType == LinkContainerType.RibbonPageHeader) {
					SetActualGlyph(GetGlyph() ?? DefaultGlyph, false);
				} else {
					ImageSource glyph = GetGlyph();
					SetActualGlyph(GetGlyph(), false);
				}
				IsRibbonStyleLarge = false;
			}
			UpdateActualShowGlyph();
		}
		protected internal virtual void UpdateActualShowCustomizationBorder() {
		}
		GlyphSize GetGlyphSize() {
			if (Link != null && Link.UserGlyphSize != GlyphSize.Default)
				return Link.UserGlyphSize;
			if (Item != null)
				return Item.GlyphSize;
			return GlyphSize.Default;
		}
		protected internal virtual void UpdateShowDescription() {
			if (LinkBase == null)
				return;
			if (!IsLinkInRibbon) {
				if (LinkBase.Links != null && LinkBase.Links.Holder != null)
					ShowDescription = LinkBase.Links.Holder.ShowDescription;
			} else {
				if (LinkBase.Links != null && LinkBase.Links.Holder != null) {
					if (ContainerType == LinkContainerType.ApplicationMenu) {
						ShowDescription = LinkBase.Links.Holder.ShowDescription && ActualDescription != null && !string.IsNullOrEmpty(ActualDescription.ToString());
					} else
						ShowDescription = LinkBase.Links.Holder.ShowDescription;
				}
			}
		}
		protected virtual bool GetActualShowKeyGesture() {
			var itemValue = Item == null ? true : Item.ShowKeyGesture;
			var result = (Link == null || !Link.ShowKeyGesture.HasValue) ? itemValue : Link.ShowKeyGesture.Value;
			var popupHolder = LinksControl.With(x => x.LinksHolder as PopupMenu);
			if (popupHolder != null) {
				if (popupHolder.ShowKeyGestures == false)
					return false;
			}
			return result;
		}
		protected internal virtual void UpdateShowKeyGesture() {
			if (!GetActualShowKeyGesture()) {
				ShowKeyGesture = false;
				return;
			}
			if (!IsLinkInRibbon) {
				ShowKeyGesture = !string.IsNullOrEmpty(ActualKeyGestureText) && IsLinkControlInMenu;
			} else {
				ShowKeyGesture = ContainerType == LinkContainerType.ApplicationMenu && (!string.IsNullOrEmpty(ActualKeyGestureText) || !string.IsNullOrEmpty(ActualDescription as String));
			}
		}
		protected virtual void OnIsEnabledChanged(object sender, System.Windows.DependencyPropertyChangedEventArgs e) {
			if (!(bool)e.NewValue) {
				IsHighlighted = false;
				IsPressed = false;
			}
			UpdateItemBorderAndContent();
			UpdateItemVisualState();
			UpdateActualIsContentEnabled();
			UpdateActualIsArrowEnabled();
		}
		protected DragDropElementHelper DragDropHelper {
			get { return dragDropHelper; }
		}
		protected virtual void CreateDragDropHelper() {
			if (dragDropHelper != null) {
				((BarDragDropElementHelper)dragDropHelper).CancelDestroy();
			}
			this.dragDropHelper = new BarDragDropElementHelper(this, false);
		}
		protected internal virtual void AfterDestroyDragDropHelper() {
			dragDropHelper = null;
		}
		RibbonItemInfo ribbonItemInfo;
		public RibbonItemInfo RibbonItemInfo {
			get {
				if (ribbonItemInfo == null)
					ribbonItemInfo = new RibbonItemInfo(this);
				return ribbonItemInfo;
			}
		}
		protected internal bool IsLinkControlInMenu {
			get { return ContainerType == LinkContainerType.Menu || ContainerType == LinkContainerType.ApplicationMenu || ContainerType == LinkContainerType.DropDownGallery || ContainerType == LinkContainerType.MenuHeader || ContainerType == LinkContainerType.RadialMenu; }
		}
		protected internal bool IsLinkControlInMainMenu {
			get {
				if (this.LinksControl == null)
					return false;
				if (this.LinksControl.LinksHolder as Bar == null)
					return false;
				return ((Bar)this.LinksControl.LinksHolder).IsMainMenu;
			}
		}
		protected bool IsLinkInMenu(PopupMenu menu) {
			return Link.Links.Holder == menu;
		}
		protected internal virtual void UpdateGlyphParams() { }
		protected virtual void OnGlyphChanged(ImageSource oldValue) {
			UpdateActualGlyph();
		}
		Size maxGlyphSize;
		protected internal override void OnMaxGlyphSizeChanged(Size MaxGlyphSize) {
			base.OnMaxGlyphSizeChanged(MaxGlyphSize);
			maxGlyphSize = MaxGlyphSize;
			UpdateLayoutPanelMaxGlyphSize();
		}
		protected virtual void OnActualGlyphChanged(DependencyPropertyChangedEventArgs e) {
			UpdateGlyphParams();
			ToolTipGlyph = GetGlyph() != null ? GetGlyph() : GetLargeGlyph();
			if (LinksControl != null)
				LinksControl.CalculateMaxGlyphSize();
			UpdateLayoutPanelActualGlyph();
		}		
		protected virtual void OnIsLargeGlyphChanged(DependencyPropertyChangedEventArgs e) {
			UpdateGlyphSize();
			if (LinksControl != null)
				LinksControl.CalculateMaxGlyphSize();			
		}
		protected internal virtual bool IsLinkInCustomizationMode {
			get { return BarManagerCustomizationHelper.IsInCustomizationMode(this) && !BarManagerCustomizationHelper.IsInCustomizationMenu(this); }
		}
		protected virtual void OnIsPressedChanged(DependencyPropertyChangedEventArgs e) {
			ShowPressedBorder = IsPressed && !IsLinkInCustomizationMode;			
		}
		protected virtual object OnIsPressedCoerce(object value) {
			return value;
		}
		protected bool ForceHideHotBorderInMenuMode { get; set; }
		protected virtual void OnIsHighlightedChanged(DependencyPropertyChangedEventArgs e) {			
			if (IsSelected && NavigationTree.CurrentElement != null && !IsKeyboardFocusWithin) {
				var focusTarget = LayoutHelper.FindElement(this, x => x != this && x.Focusable);
				if (focusTarget != null)
					focusTarget.Focus();
				else
				Focus();
			}
			if (Manager != null) {
				RaiseWeakIsHighlightedChanged(new ValueChangedEventArgs<bool>((bool)e.OldValue, (bool)e.NewValue));
				if (!IsOnBar) {
					this.RaiseEvent(new RoutedPropertyChangedEventArgs<bool>((bool)e.OldValue, (bool)e.NewValue, IsHighlightedChangedEvent));
					UpdateShowHotBorder();
				}
			}			
			if (IsHighlighted) {
				if (!(this is IPopupOwner))
					PopupMenuManager.CloseChildren(BarManagerHelper.GetPopup(this), false);
			}
			UpdateShowHotBorder();
		}
		protected internal virtual void UpdateShowHotBorder() {
			if (Manager != null && (PopupMenuManager.IsAnyPopupOpened && PopupMenuManager.TopPopup!=BarManagerHelper.GetPopup(this)) && !IsLinkControlInMenu && !IsLinkInRibbon)
				ShowHotBorder = false;
			else
				ShowHotBorder = IsHighlighted && !IsLinkInCustomizationMode || !ForceHideHotBorderInMenuMode && NavigationTree.CurrentElement != null && IsSelected;
		}
		protected internal virtual void ForceSetIsSelected(bool value) {
			bool ov = IsSelected;
			IsSelected = value;
			if (ov == value)
				OnIsSelectedChanged(new DependencyPropertyChangedEventArgs(IsSelectedProperty, value, value));
		}		
		protected virtual void OnIsSelectedChanged(DependencyPropertyChangedEventArgs e) {
			ForceHideHotBorderInMenuMode = false;
			if (IsSelected) {				
				SetFocus();
			}							
			UpdateIsHighlighted();
		}
		protected virtual bool SetFocus() {
			return (LayoutHelper.FindElement(this, x => x != this && x.Focusable) ?? this).Focus();
		}
		protected internal virtual void UpdateIsHighlighted() {
			bool calculatedIsHighlighted = ShouldHighlightItem();
			if (calculatedIsHighlighted == IsHighlighted)
				UpdateShowHotBorder();
			IsHighlighted = calculatedIsHighlighted;
		}			 
		protected override void OnMouseRightButtonDown(MouseButtonEventArgs e) {
			base.OnMouseRightButtonDown(e);			
				e.Handled = BarNameScope.GetService<ICustomizationService>(this).ShowCustomizationMenu(this);
		}
		protected object OnVisibilityCoerce(object baseValue) {
			Visibility value = (Visibility)baseValue;
			if (value != Visibility.Visible)
				return value;
			if (LinkBase != null && !LinkBase.ActualIsVisible)
				return Visibility.Collapsed;
			return baseValue;
		}
		protected override void OnMouseEnter(MouseEventArgs e) {
			base.OnMouseEnter(e);
			if(BarManagerCustomizationHelper.IsInCustomizationMode(this)) {
				LinksControl.Do((x) => x.OnItemMouseEnter(this));
				return;
			}
			UpdateToolTip();
			LinksControl.Do((x) => x.OnItemMouseEnter(this));
			(this as IPopupOwner).With(x => x.Popup).With(x => x.Popup).Do(PopupMenuManager.CancelPopupClosing);
			SelectInMenuMode();			
		}
		protected virtual void SelectInMenuMode() {
			if (!CanSelectOnHoverInMenuMode)
				return;
			if (!IsLinkControlInMainMenu && !IsLinkControlInMenu || IsLinkInCustomizationMode || IsLinkInRadialMenu)
				return;
			if ((NavigationTree.CurrentElement as BarItemLinkInfo).If(x=>x.IsKeyboardFocusWithin).ReturnSuccess()) {
				bool isLinkControlFocused = Keyboard.FocusedElement is BarItemLinkControl;
				var lOwner = (Keyboard.FocusedElement as DependencyObject).With(BarManagerHelper.GetPopup).With(x => x.OwnerLinkControl).With(x => x.LinkInfo);
				if (!isLinkControlFocused && lOwner == null)
					return;
			}
			if ((this as IPopupOwner).If(x => x.IsPopupOpen).ReturnSuccess())
				return;
			bool isPopupOpened = PopupMenuManager.TopPopup != null;			
			NavigationTree.SelectElement(LinkInfo);
			var ipo = this as IPopupOwner;
			if (ipo == null)
				return;
			if (ipo.ActAsDropdown && Equals(LinkInfo, NavigationTree.CurrentElement) && IsLinkControlInMainMenu && isPopupOpened) {
				ipo.ShowPopup();
			}
		}
		protected virtual bool ShouldHighlightItem() {
			var ownsOpenedPopup = PopupMenuManager.TopPopup == null ? false : PopupMenuManager.PopupAncestors(PopupMenuManager.TopPopup, false).Select(x => x.OwnerLinkControl).Any(x => x == this);
			bool isInOpenedPopup = PopupMenuManager.TopPopup == null || BarManagerHelper.GetPopup(this) != null;			
			return (ownsOpenedPopup || IsMouseOver && isInOpenedPopup) && !IsLinkInCustomizationMode;
		}
		protected override void OnMouseLeave(MouseEventArgs e) {
			base.OnMouseLeave(e);
			bool shouldHighlightItem = ShouldHighlightItem();
			ForceHideHotBorderInMenuMode = !shouldHighlightItem && IsLinkControlInMenu;
			IsHighlighted = shouldHighlightItem;			
			UpdateShowHotBorder();
			LinksControl.Do((x) => x.OnItemMouseLeave(this, e));
		}
		Point lastMousePosition = new Point();
		protected override void OnMouseMove(MouseEventArgs e) {
			if (e.GetPosition(this) != lastMousePosition) {
				IsHighlighted = ShouldHighlightItem();
				lastMousePosition = e.GetPosition(this);
			}
			if (this is BarSubItemLinkControl || this is BarSplitButtonItemLinkControl) {
				ToolTip toolTip = this.GetToolTip() as ToolTip;
				if (toolTip != null)
					if (this.LayoutPanel != null && !this.LayoutPanel.IsMouseOver) {
						toolTip.Visibility = System.Windows.Visibility.Collapsed;
						this.SetToolTip(null);
					}
			}
			base.OnMouseMove(e);
		}
		protected override void OnVisualParentChanged(DependencyObject oldParent) {
			base.OnVisualParentChanged(oldParent);
			IsOnBar = (LinksControl is BarControl);
		}
		protected virtual bool ShouldDeactivateMenuOnAccessKey {
			get { return true; }
		}
		protected override void OnAccessKey(AccessKeyEventArgs e) {
			if (Link == null || Link.Item == null)
				return;
			if (e.IsMultiple)
				this.IsHighlighted = true;
			else
				OnAccessKeyCore(e);
			if (Manager != null && ShouldDeactivateMenuOnAccessKey && !e.IsMultiple)
				Manager.DeactivateMenu();
		}
		protected virtual void OnAccessKeyCore(AccessKeyEventArgs e) {
			OnClick();		   
		}
		protected virtual void OnAccessKeyPressed(AccessKeyPressedEventArgs e) {
			if (Link == null || 
			(NavigationTree.CurrentElement == null && !(Keyboard.IsKeyDown(Key.LeftAlt) || Keyboard.IsKeyDown(Key.RightAlt))))
				return;
			OnAccessKeyPressedCore(e);
		}
		protected virtual void OnAccessKeyPressedCore(AccessKeyPressedEventArgs e) {
			if (e.Target == null)
				e.Target = this;
		}
		protected static void OnAccessKeyPressed(object sender, AccessKeyPressedEventArgs e) {
			BarItemLinkControl barItemLinkControl = sender as BarItemLinkControl;
			if (barItemLinkControl != null)
				barItemLinkControl.OnAccessKeyPressed(e);
		}
		protected virtual void OnHasGlyphChanged(DependencyPropertyChangedEventArgs e) {
		}
		protected internal virtual void OnClick() {
			if (IsLinkInCustomizationMode)
				return;
			LinksControl lc = LayoutHelper.FindParentObject<LinksControl>(this);
			BarItemLink link = Link; 
			if (lc != null) {
				lc.OnPreviewItemClick(this);
			}
			if (link != null) {
				if (Item != null)
					Item.isInMenu = IsLinkControlInMenu;
				link.OnClick();
				if (Item != null)
					Item.isInMenu = false;
			}
			if (lc != null) {
				lc.OnItemClick(this);
			}
			this.RaiseEvent(new RoutedEventArgs(ClickEvent, this));
		}
		protected internal Point ClientOffset { get; set; }
		protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e) {
			base.OnMouseLeftButtonUp(e);
			BarNameScope.GetService<ICustomizationService>(this).IsPreparedToQuickCustomizationMode = false;
		}		
		protected override void OnPreviewMouseLeftButtonUp(MouseButtonEventArgs e) {
			var toolTip = this.GetToolTip() as UIElement;
			if (toolTip != null) {
				toolTip.SetCurrentValue(UIElement.VisibilityProperty, Visibility.Collapsed);
				toolTip.UpdateLayout();
				toolTip.SetCurrentValue(UIElement.VisibilityProperty, Visibility.Visible);
			}
			base.OnPreviewMouseLeftButtonUp(e);
		}
		protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e) {
			if (IsLinkInCustomizationMode) {
				if (!IsLinkControlInMenu)
					PopupMenuManager.CloseAllPopups(this, e);
			}			
			ClientOffset = e.GetPosition(this);			
			OnMouseLeftButtonDownCore(e);
			base.OnMouseLeftButtonDown(e);
		}
		protected override void OnMouseDown(MouseButtonEventArgs e) {
			base.OnMouseDown(e);
		}
		protected override void OnMouseDoubleClick(MouseButtonEventArgs e) {
			base.OnMouseDoubleClick(e);
			if(!e.Handled && e.ChangedButton == MouseButton.Left)
			{
				OnMouseLeftButtonDownCore(e);
				if(Link != null) {
					Link.OnDoubleClick();
					e.Handled = true;
				}
			}
		}
		#region ISupportDragDrop Members
		bool ISupportDragDrop.CanStartDrag(object sender, MouseButtonEventArgs e) {
			return CanStartDragCore(sender, e);
		}
		protected virtual bool CanStartDragCore(object sender, MouseButtonEventArgs e) {
			if (VisualParent == null || IsLinkInRibbon)
				return false;
			bool enableCustomization = TreeHelper.GetParent<LinksControl>(this, x => x.LinksHolder.If(h => h.IsMergedState) != null) == null;
			bool res = enableCustomization &&
				(BarManagerCustomizationHelper.IsInCustomizationMode(this) || BarNameScope.GetService<ICustomizationService>(this).IsPreparedToQuickCustomizationMode);
			if (res) {
				return IsMouseOver;
			}			
			res = enableCustomization &&
				(Manager.Return(x => x.GetHotQuickCustomization(), () => true) && (Keyboard.Modifiers == ModifierKeys.Alt || Keyboard.Modifiers == (ModifierKeys.Alt | ModifierKeys.Control)));
			if (res && (IsMouseOver || LinkInfo.If(x=>x.IsMouseOver).ReturnSuccess())) {
				BarNameScope.GetService<ICustomizationService>(this).IsPreparedToQuickCustomizationMode = true;
				return true;
			}
			return false;
		}		
		internal IDragElement DragElement { get; private set; }
		IDragElement ISupportDragDrop.CreateDragElement(Point offset) {
			var content = new BarItemDragElementContent(LinkInfo.Link.Item);
			BarDragElementPopup popup = new BarDragElementPopup(content, BarNameScope.FindScope(this).With(x => x.Target as FrameworkElement), this, ((ISupportDragDrop)this).SourceElement);
			popup.FlowDirection = this.FlowDirection;
			popup.IsOpen = true;
			DragElement = popup;
			return popup;
		}
		IEnumerable<UIElement> ISupportDragDrop.GetTopLevelDropContainers() {
			return BarDragDropElementHelper.GetTopLevelDropContainers(this);
		}
		protected internal virtual BarContainerControl GetRootContainer() {
			return GetRootContainer(this);
		}
		protected internal virtual BarControl GetRootBarControl() {
			return GetRootBarControl(this);
		}
		protected virtual BarControl GetRootBarControl(BarItemLinkControl linkControl) {
			BarContainerControl c = linkControl.Container;
			if (c != null)
				return linkControl.LinksControl as BarControl;
			SubMenuBarControl sb = LinksControl as SubMenuBarControl;
			if (sb != null && sb.Popup.OwnerLinkControl != null)
				return GetRootBarControl(sb.Popup.OwnerLinkControl);
			return null;
		}
		protected virtual BarContainerControl GetRootContainer(BarItemLinkControl linkControl) {
			BarContainerControl c = linkControl.Container;
			if (c != null) {
				return c;
			}
			SubMenuBarControl sb = linkControl.LinksControl as SubMenuBarControl;
			if (sb.With(x=>x.Popup).With(x=>x.OwnerLinkControl) != null) {
				return GetRootContainer(sb.Popup.OwnerLinkControl);
			}
			return null;
		}
		FrameworkElement ISupportDragDrop.SourceElement {
			get {
				return GetRootContainer() ?? Manager ?? PresentationSource.FromDependencyObject(this).With(x => x.RootVisual as FrameworkElement);
			}
		}
		IDropTarget ISupportDragDrop.CreateEmptyDropTarget() {
			return new BarEmptyDropTarget() { Manager = Manager };
		}
		bool ISupportDragDrop.IsCompatibleDropTargetFactory(IDropTargetFactory factory, UIElement dropTargetElement) {
			return factory is BarControlDropTargetFactoryExtension || factory is SubMenuBarControlDropTargetFactoryExtension;
		}
		#endregion        
		protected virtual object CreateSuperTipControl() {
			if (Link == null)
				return null;
			SuperTip s = Link.ActualSuperTip;
			if (s == null) {
				s = new SuperTip();
				var tooltip = Link.With(x => x.ToolTip) ?? Item.With(x=>x.ToolTip);
				if (tooltip != null) {
					s.Items.Add(new SuperTipItem() { Content = tooltip });
				} else {
					if (ActualContent is string) {
						string actualContentWithoutAccessKey = GetContentWithoutAccessKey(ActualContent.ToString());
						if (Link.KeyGestureText != null && Link.KeyGestureText.Length != 0 && BarNameScope.GetService<ICustomizationService>(this).CustomizationHelper.Return(x => x.ShowShortcutInScreenTips, () => true))
							actualContentWithoutAccessKey += " (" + Link.KeyGestureText + ")";
						if (!string.IsNullOrEmpty(actualContentWithoutAccessKey))
							s.Items.Add(new SuperTipHeaderItem() { Content = actualContentWithoutAccessKey });
					} else if (ActualContent != null)
						s.Items.Add(new SuperTipHeaderItem() { Content = ActualContent });
					if (Link.ActualHint != null)
						s.Items.Add(new SuperTipItem() { Content = Link.ActualHint });
				}				
			}
			SuperTipControl superTipControl = new SuperTipControl(s);
			if (Link != null && Link.Item != null) {
				superTipControl.SetBinding(SuperTipControl.DataContextProperty, new Binding() { Source = Link.Item, Path = new PropertyPath(BarItem.DataContextProperty) });
			}
			return superTipControl;
		}
		protected virtual string GetContentWithoutAccessKey(string actualContent) {
			StringBuilder sb = new StringBuilder();
			for (int i = 0; i < actualContent.Length; i++) {
				if (actualContent[i] == '_') {
					if (i + 1 < actualContent.Length && actualContent[i + 1] == '_') {
						sb.Append(actualContent[i]);
						i++;
					}
					continue;
				}
				sb.Append(actualContent[i]);
			}
			return sb.ToString();
		}
		protected virtual object CreateToolTip() {
			return CreateSuperTipControl();
		}
		bool IsCustomToolTip() {
			return Link != null && Link.Item != null && Link.Item.SuperTip != null;
		}
		protected virtual bool IsEmptyToolTip() {
			if (IsCustomToolTip())
				return false;
			return (ActualContent == null || ActualContent.ToString() == "") && (Link==null || Link.Item == null || Link.ActualHint == null || Link.ActualHint.ToString() == "");
		}
		protected virtual bool IsUninformativeToolTip() {
			if (IsCustomToolTip())
				return false;
			if (Link != null && (Link.HasHint || Link.HasKeyGesture))
				return false;
			if (!ActualShowContent && ActualContent != null)
				return false;
			return true;
		}
		internal void RecreateToolTip() {
			this.SetToolTip(null);
			UpdateToolTip();
		}
		bool GetShowScreenTip() {
			if (Link != null && Link.ShowScreenTip != DefaultBoolean.Default)
				return Link.ShowScreenTip == DefaultBoolean.True;
			if (Item != null)
				return Item.ShowScreenTip;
			return true;
		}
		protected virtual bool CanShowToolTip() {
			if (Link == null)
				return false;
			bool showScreenTips = BarNameScope.GetService<ICustomizationService>(this).CustomizationHelper.Return(x => x.ShowScreenTips, () => true);
			bool showScreenTipsInPopupMenus = Manager == null ? true : Manager.ShowScreenTipsInPopupMenus;
			bool retValue = showScreenTips && !BarManagerCustomizationHelper.IsInCustomizationMode(this) && !IsEmptyToolTip() && !IsUninformativeToolTip() && GetShowScreenTip();
			if (IsLinkControlInMenu)
				retValue = retValue && showScreenTipsInPopupMenus;
			return retValue;
		}
		protected internal virtual void UpdateToolTip() {
			if (!CanShowToolTip()) {
				ToolTip toolTip = this.GetToolTip() as ToolTip;
				if (toolTip != null) {
					toolTip.Visibility = System.Windows.Visibility.Collapsed;
				}
				this.SetToolTip(null);
			} else {
				if(this.GetToolTip() == null) this.SetToolTip(new BarItemLinkControlToolTip() { UseToolTipPlacementTarget = true });
				ToolTip toolTip = this.GetToolTip() as ToolTip;
				if (toolTip.Content != null)
					ClearToolTip();
				toolTip.Content = CreateToolTip();
				toolTip.Visibility = Visibility.Visible;
			}
		}
		void ClearToolTip() {
			ToolTip toolTip = this.GetToolTip() as ToolTip;
			if (toolTip == null)
				return;
			SuperTipControl superTipControl = toolTip.Content as SuperTipControl;
			if (superTipControl != null && Link != null && Link.Item != null) {
				Link.Item.ClearDataContext(superTipControl.SuperTip);
				Link.Item.ClearDataContext(superTipControl);
			}
			toolTip.Content = null;
		}				
		protected override object GetDataContext() {
			if (Item != null)
				return Item.DataContext;
			return base.GetDataContext();
		}		
		protected override void OnContainerChanged() {
			UpdateOrientation();			
			UpdateLayoutPanelHorizontalAlignment();
		}
		protected override void UpdateLayoutByContainerType(LinkContainerType type) {
			if (LayoutPanel == null)
				return;
		}
		protected virtual Func<BarItemLayoutPanelThemeKeys, BarItemLayoutPanelThemeKeyExtension> GetThemeKeyExtensionFunc {
			get { return null; }
		}
		protected FrameworkElement ResourceSource { get { return (FrameworkElement)LinksControl ?? this; } }
		protected internal override void UpdateTemplateByContainerType(LinkContainerType type) {
			base.UpdateTemplateByContainerType(type);
			if (LayoutPanel == null || GetThemeKeyExtensionFunc == null)
				return;
			switch (type) {
				case LinkContainerType.ApplicationMenu:
					LayoutPanel.Style = ResourceStorage.TryFindResourceInStorage(ResourceSource, GetThemeKeyExtensionFunc(BarItemLayoutPanelThemeKeys.StyleInApplicationMenu)) as Style;
					break;
				case LinkContainerType.Menu:
					LayoutPanel.Style = ResourceStorage.TryFindResourceInStorage(ResourceSource, GetThemeKeyExtensionFunc(BarItemLayoutPanelThemeKeys.StyleInMenu)) as Style;
					break;
				case LinkContainerType.DropDownGallery:
					LayoutPanel.Style = ResourceStorage.TryFindResourceInStorage(ResourceSource, GetThemeKeyExtensionFunc(BarItemLayoutPanelThemeKeys.StyleInDropDownGallery)) as Style;
					break;
				case LinkContainerType.MenuHeader:
					LayoutPanel.Style = ResourceStorage.TryFindResourceInStorage(ResourceSource, GetThemeKeyExtensionFunc(BarItemLayoutPanelThemeKeys.StyleInMenuHeader)) as Style;
					break;
				case LinkContainerType.BarButtonGroup:
					LayoutPanel.Style = ResourceStorage.TryFindResourceInStorage(ResourceSource, GetThemeKeyExtensionFunc(BarItemLayoutPanelThemeKeys.StyleInButtonGroup)) as Style;
					break;
				case LinkContainerType.RibbonPageGroup:
					LayoutPanel.Style = ResourceStorage.TryFindResourceInStorage(ResourceSource, GetThemeKeyExtensionFunc(BarItemLayoutPanelThemeKeys.StyleInRibbonPageGroup)) as Style;
					break;
				case LinkContainerType.RibbonQuickAccessToolbar:
					LayoutPanel.Style = ResourceStorage.TryFindResourceInStorage(ResourceSource, GetThemeKeyExtensionFunc(BarItemLayoutPanelThemeKeys.StyleInQAT)) as Style;
					break;
				case LinkContainerType.RibbonQuickAccessToolbarFooter:
					LayoutPanel.Style = ResourceStorage.TryFindResourceInStorage(ResourceSource, GetThemeKeyExtensionFunc(BarItemLayoutPanelThemeKeys.StyleInQATFooter)) as Style;
					break;
				case LinkContainerType.MainMenu:
					LayoutPanel.Style = ResourceStorage.TryFindResourceInStorage(ResourceSource, GetThemeKeyExtensionFunc(BarItemLayoutPanelThemeKeys.StyleInMainMenu)) as Style;
					break;
				case LinkContainerType.StatusBar:
					LayoutPanel.Style = ResourceStorage.TryFindResourceInStorage(ResourceSource, GetThemeKeyExtensionFunc(BarItemLayoutPanelThemeKeys.StyleInStatusBar)) as Style;
					break;
				case LinkContainerType.RibbonPageHeader:
					LayoutPanel.Style = ResourceStorage.TryFindResourceInStorage(ResourceSource, GetThemeKeyExtensionFunc(BarItemLayoutPanelThemeKeys.StyleInRibbonPageHeader)) as Style;
					break;
				case LinkContainerType.RibbonStatusBarLeft:
					LayoutPanel.Style = ResourceStorage.TryFindResourceInStorage(ResourceSource, GetThemeKeyExtensionFunc(BarItemLayoutPanelThemeKeys.StyleInRibbonStatusBarLeft)) as Style;
					break;
				case LinkContainerType.RibbonStatusBarRight:
					LayoutPanel.Style = ResourceStorage.TryFindResourceInStorage(ResourceSource, GetThemeKeyExtensionFunc(BarItemLayoutPanelThemeKeys.StyleInRibbonStatusBarRight)) as Style;
					break;
				default:
					LayoutPanel.Style = ResourceStorage.TryFindResourceInStorage(ResourceSource, GetThemeKeyExtensionFunc(BarItemLayoutPanelThemeKeys.StyleInBar)) as Style;
					break;
			}
		}
		protected override void OnContainerTypeChanged(LinkContainerType oldValue) {
			if (ContainerType == LinkContainerType.ApplicationMenu || ContainerType == LinkContainerType.Menu) {
				this.SetToolTip(null);
			}
			UpdateActualGlyph();
			UpdateLayoutPanelElementContentProperties();
			base.OnContainerTypeChanged(oldValue);
		}
		protected virtual void CheckUpdateIsPressed(MouseEventArgs e) {
			if (MouseHelper.Captured == this && IsMouseLeftButtonPressed) {
				UpdateIsPressed();
				e.SetHandled(true);
			}
		}
		protected virtual void UpdateIsPressed() {
			Point position = MouseHelper.GetPosition(this);
			if (new Rect(RenderSize).Contains(position) || IsMouseOver) {
				IsPressed = true;
				return;
			}
			IsPressed = false;
		}
		protected internal virtual void OnMouseLeftButtonDownCore(MouseButtonEventArgs e) {
			BarNameScope.GetService<ICustomizationService>(this).Select(this);
		}
		protected internal bool IsLinkInRibbon {
			get {
				switch (ContainerType) {
					case LinkContainerType.RibbonPageGroup:
					case LinkContainerType.BarButtonGroup:
					case LinkContainerType.RibbonQuickAccessToolbar:
					case LinkContainerType.RibbonQuickAccessToolbarFooter:
					case LinkContainerType.RibbonPageHeader:
					case LinkContainerType.ApplicationMenu:
					case LinkContainerType.RibbonStatusBarLeft:
					case LinkContainerType.RibbonStatusBarRight:
					case LinkContainerType.DropDownGallery:
						return true;
				}
				return false;
			}
		}
		protected internal bool IsLinkInRadialMenu { get { return ContainerType == LinkContainerType.RadialMenu; } }
		protected bool IsLinkInStatusBar {
			get { return ContainerType == LinkContainerType.RibbonStatusBarLeft || ContainerType == LinkContainerType.RibbonStatusBarRight; }
		}
		protected internal bool IsLinkInApplicationMenu {
			get { return ContainerType == LinkContainerType.ApplicationMenu; }
		}
		public virtual void InitializeRibbonStyle() {
			if (!IsLinkInRibbon || Link == null)
				return;
			if (Link.Links == null)
				CurrentRibbonStyle = RibbonItemStyles.SmallWithText;
			switch (ContainerType) {
				case LinkContainerType.RibbonPageHeader:
					CurrentRibbonStyle = CalcRibbonStyleInPageHeader();
					break;
				case LinkContainerType.RibbonQuickAccessToolbar:
				case LinkContainerType.RibbonQuickAccessToolbarFooter:
					CurrentRibbonStyle = CalcRibbonStyleInQAT();
					break;
				case LinkContainerType.RibbonPageGroup:
					CurrentRibbonStyle = CalcRibbonStyleInPageGroup();
					break;
				case LinkContainerType.BarButtonGroup:
					CurrentRibbonStyle = CalcRibbonStyleInButtonGroup();
					break;
				case LinkContainerType.RibbonStatusBarLeft:
				case LinkContainerType.RibbonStatusBarRight:
					CurrentRibbonStyle = CalcRibbonStyleInStatusBar();
					break;
				case LinkContainerType.ApplicationMenu:
					CurrentRibbonStyle = CalcRibbonStyleInApplicationMenu();
					break;
				case LinkContainerType.DropDownGallery:
					CurrentRibbonStyle = CalcRibbonStyleInDropDownGallery();
					break;
			}
		}
		protected virtual RibbonItemStyles CalcRibbonStyleInPageHeader() {
			if(Link.ActualRibbonStyle == RibbonItemStyles.Default)
				return CalcRibbonStyleInQAT();
			return RibbonItemStyles.SmallWithText;
		}
		protected virtual RibbonItemStyles CalcRibbonStyleInStatusBar() {
			if (Link.ActualRibbonStyle == RibbonItemStyles.Default || (Link.ActualRibbonStyle & RibbonItemStyles.Large) == RibbonItemStyles.Large) {
				return RibbonItemStyles.SmallWithText;
			}
			return Link.ActualRibbonStyle;
		}
		protected virtual RibbonItemStyles CalcRibbonStyleInApplicationMenu() {
			if (Link.ActualRibbonStyle == RibbonItemStyles.Default || (Link.ActualRibbonStyle & RibbonItemStyles.Large) == RibbonItemStyles.Large) {
				return RibbonItemStyles.SmallWithText;
			}
			return Link.ActualRibbonStyle;
		}
		protected virtual RibbonItemStyles CalcRibbonStyleInDropDownGallery() {
			if (Link.ActualRibbonStyle == RibbonItemStyles.Default || (Link.ActualRibbonStyle & RibbonItemStyles.Large) == RibbonItemStyles.Large) {
				return RibbonItemStyles.SmallWithText;
			}
			return Link.ActualRibbonStyle;
		}
		protected virtual RibbonItemStyles CalcRibbonStyleInButtonGroup() {
			if (Item is BarEditItem)
				return RibbonItemStyles.SmallWithoutText;
			if (Item != null && (GetLargeGlyph() != null || GetGlyph() != null || GetGlyphTemplate() != null))
				return RibbonItemStyles.SmallWithoutText;
			return RibbonItemStyles.SmallWithText;
		}
		protected virtual RibbonItemStyles CalcRibbonStyleInQAT() {
			return RibbonItemStyles.SmallWithoutText;
		}
		protected virtual RibbonItemStyles CalcRibbonStyleInPageGroup() {
			if (Link.Item == null)
				return Link.ActualRibbonStyle;
			if (Link.ActualRibbonStyle == RibbonItemStyles.Default) {
				if (Link.Item.GetLargeGlyph() != null)
					return RibbonItemStyles.Large;
				else
					return RibbonItemStyles.SmallWithText;
			} else if (Link.ActualRibbonStyle == RibbonItemStyles.All || (Link.ActualRibbonStyle & RibbonItemStyles.Large) == RibbonItemStyles.Large) {
				return RibbonItemStyles.Large;
			}
			return Link.ActualRibbonStyle;
		}
		protected internal virtual bool SupportRibbonStyle(RibbonItemStyles style) {
			if (style == RibbonItemStyles.SmallWithoutText && ActualGlyph == null)
				return false;
			if (Link.ActualRibbonStyle == RibbonItemStyles.All)
				return true;
			if (Link.ActualRibbonStyle == RibbonItemStyles.Default && Link.Item.GetGlyph() != null)
				return true;
			return (Link.ActualRibbonStyle & style) == style;
		}
		public virtual bool SupportSmallWithTextRibbonStyle() {
			return SupportRibbonStyle(RibbonItemStyles.SmallWithText);
		}
		public virtual bool SupportSmallWithoutTextRibbonStyle() {
			return SupportRibbonStyle(RibbonItemStyles.SmallWithoutText);
		}
		protected virtual void UpdateActualShowGlyphInRibbon() {
			if (CurrentRibbonStyle == RibbonItemStyles.SmallWithoutText || IsLargeGlyph) {
				ActualShowGlyph = true;
				return;
			}
			if (CurrentRibbonStyle == RibbonItemStyles.SmallWithText && ActualContent == null && ActualContentTemplate == null) {
				ActualShowGlyph = true;
				return;
			}
			if ((ActualGlyph == null && ActualGlyphTemplate == null) || ActualGlyph == DefaultGlyph) {
				ActualShowGlyph = false;
				return;
			}
			ActualShowGlyph = true;
		}
		protected virtual void UpdateActualShowGlyphInBars() {
			if ((ActualBarItemDisplayMode == BarItemDisplayMode.ContentAndGlyph || ActualBarItemDisplayMode == BarItemDisplayMode.Default) && (ActualGlyph != null || ActualGlyphTemplate != null) || IsLinkControlInMenu || IsInHorizontalHeader())
				ActualShowGlyph = true;
			else
				ActualShowGlyph = false;
		}
		protected virtual void OnCurrentRibbonStyleChanged(RibbonItemStyles oldValue) {
			UpdateActualGlyph();
			UpdateActualGlyphAlignment();
			UpdateActualShowGlyph();
			UpdateGlyphParams();
			UpdateActualShowContent();
		}
		protected internal virtual void UpdateActualGlyphAlignmentInRibbon() {
			switch (CurrentRibbonStyle) {
				case RibbonItemStyles.Default:
					break;
				case RibbonItemStyles.Large:
					ActualGlyphAlignment = Dock.Top;
					break;
				default:
					ActualGlyphAlignment = Dock.Left;
					break;
			}
		}
		protected virtual void UpdateActualGlyphAlignmentInBars() {
			ActualGlyphAlignment = GetGlyphAlignment();
		}
		Dock GetGlyphAlignment() {
			if(ContainerType == LinkContainerType.RadialMenu)
				return Dock.Top;
			if(IsLinkControlInMenu)
				return Dock.Left;
			if(Link != null && Link.UserGlyphAlignment.HasValue)
				return Link.UserGlyphAlignment.Value;
			if(Item != null)
				return Item.GlyphAlignment;
			return Dock.Left;
		}
		protected internal BarItemLayoutPanel LayoutPanel { get; private set; }
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			if (LayoutPanel != null) {
				LayoutPanel.Clear();
			}
			LayoutPanel = GetTemplateChild("PART_LayoutPanel") as BarItemLayoutPanel;
			UpdateByContainerType(ContainerType);
			UpdateItemVisualState();
			UpdateActualGlyph();
			if (LayoutPanel != null) {
				LayoutPanel.BeginUpdate();
				UpdateLayoutPanel();
				LayoutPanel.EndUpdate();
			} else {
				UpdateActualSectorIndex();
				UpdateActualShowArrow();
			}
		}
		protected override void OnStyleChanged(Style oldStyle, Style newStyle) {
			base.OnStyleChanged(oldStyle, newStyle);
			UpdateByContainerType(ContainerType);
		}
		protected override System.Windows.Automation.Peers.AutomationPeer OnCreateAutomationPeer() {
			return NavigationAutomationPeersCreator.Default.Create(this);
		}
		protected virtual void UpdateStyleForItemContent(Style contentStyle, Style descriptionStyle, Style arrowStyle, Style editStyle) {
		}
		protected virtual bool? IsChecked {
			get { return false; }
		}
		protected virtual void UpdateItemBorderAndContent() {
			if (LayoutPanel == null)
				return;
			LayoutPanel.Opacity = 1.0d;
			if (ShowCustomizationBorder) {
				LayoutPanel.BorderState = BorderState.Customization;
				UpdateStyleForItemContent(null, null, null, null);
			} else if (!IsEnabled) {
				LayoutPanel.Opacity = LayoutPanel.DisabledOpacity;			  
				UpdateStyleForItemContent(null, null, null, null);
			}
			UpdateItemVisualState();
		}
		protected internal override void UpdateByContainerType(LinkContainerType type) {
			base.UpdateByContainerType(type);
			UpdateLayoutPanelFontSettingsByContainerType();
		}
		protected virtual void UpdateItemVisualState() {
			if (LayoutPanel == null)
				return;
			if (ShowCustomizationBorder) {
				LayoutPanel.BorderState = BorderState.Customization;
				return;
			}
			if (!IsEnabled && (IsChecked==false)) {
				LayoutPanel.BorderState = BorderState.Disabled;
				return;
			}
			if (ShowPressedBorder) {
				LayoutPanel.BorderState = BorderState.Pressed;
				return;
			}
			if (IsChecked==true && ShowHotBorder) {
				LayoutPanel.BorderState = BorderState.HoverChecked;
				return;
			}
			if (IsChecked==true) {
				LayoutPanel.BorderState = BorderState.Checked;
				return;
			}			
			if (!IsChecked.HasValue) {
				LayoutPanel.BorderState = BorderState.Indeterminate;
				return;
			}
			if (ShowHotBorder) {
				LayoutPanel.BorderState = BorderState.Hover;
				return;
			}
			LayoutPanel.BorderState = BorderState.Normal;
		}
		protected virtual void OnActualSectorIndexChanged() {			
			(this.VisualParents().OfType<Panel>().FirstOrDefault() as RadialMenuItemsPanel).Do(ip => ip.InvalidateArrange());
		}
		protected virtual void OnActualIsContentEnabledChanged(bool oldValue) {
			if(ContainerType == LinkContainerType.RadialMenu)
				ActualIsHoverEnabled = ActualIsContentEnabled;
		}
		protected virtual void OnActualShowArrowChanged(DependencyPropertyChangedEventArgs e) {
		}
		protected virtual void OnActualShowGlyphChanged(DependencyPropertyChangedEventArgs e) {
			UpdateLayoutPanelShowGlyph();
		}
		protected virtual void OnActualShowContentChanged(DependencyPropertyChangedEventArgs e) {
			UpdateLayoutPanelShowContent();
		}
		protected virtual void OnShowDescriptionChanged(DependencyPropertyChangedEventArgs e) {
			UpdateLayoutPanelShowDescription();
		}
		protected virtual void OnShowKeyGestureChanged(DependencyPropertyChangedEventArgs e) {
			UpdateLayoutPanelShowKeyGesture();
		}
		protected virtual void OnActualGlyphAlignmentChanged(DependencyPropertyChangedEventArgs e) {
			UpdateLayoutPanelGlyphToContentAlignment();
			UpdateLayoutPanelElementContentProperties();
		}		
		protected virtual void UpdateGlyphSize() {
			UpdateLayoutPanelGlyphSize();
		}		
		protected virtual void UpdateLayoutPanel() {
			if (LayoutPanel == null)
				return;
			UpdateItemBorderAndContent();
			UpdateLayoutPanelGlyphToContentAlignment();
			UpdateLayoutPanelShowContent();
			UpdateLayoutPanelShowGlyph();
			UpdateLayoutPanelShowDescription();
			UpdateLayoutPanelShowKeyGesture();
			UpdateLayoutPanelContent();
			UpdateLayoutPanelDescription();
			UpdateLayoutPanelKeyGesture();
			UpdateLayoutPanelActualGlyph();
			UpdateLayoutPanelContentTemplate();
			UpdateLayoutPanelContentTemplateSelector();
			UpdateLayoutPanelSplitContent();
			UpdateLayoutPanelFontSettingsByContainerType();
			UpdateLayoutPanelGlyphTemplate();
			UpdateLayoutPanelGlyphSize();
			UpdateLayoutPanelHorizontalItemPosition();
			UpdateLayoutPanelHorizontalAlignment();
			UpdateLayoutPanelElementContentProperties();
			UpdateActualAllowGlyphTheming();
			UpdateLayoutPanelSpacingMode();
			UpdateActualSectorIndex();
			UpdateLayoutPanelMaxGlyphSize();
		}
		protected string ThemeName {
			get {
				return ThemeHelper.GetEditorThemeName(this);
			}
		}
		protected virtual void UpdateLayoutPanelElementContentProperties() {
			if (LayoutPanel != null) {
				if ((ContainerType == LinkContainerType.Bar || ContainerType == LinkContainerType.MainMenu || ContainerType == LinkContainerType.StatusBar) && (ActualGlyphAlignment == Dock.Top || ActualGlyphAlignment == Dock.Bottom)) {
					LayoutPanel.ContentHorizontalAlignment = System.Windows.HorizontalAlignment.Center;
					return;
				}
				LayoutPanel.ContentHorizontalAlignment = System.Windows.HorizontalAlignment.Left;
			}
		}
		protected internal virtual void UpdateLayoutPanelSpacingMode() {
			if (LayoutPanel == null)
				return;
			LayoutPanel.SpacingMode = SpacingMode;
		}
		protected internal virtual void UpdateActualAllowGlyphTheming() {
			if (Item == null) {
				ActualAllowGlyphTheming = false;
				return;
			}
			if (Manager == null) {
				ActualAllowGlyphTheming = Item.AllowGlyphTheming.Return(x => x.Value, () => false);
			} else {
				ActualAllowGlyphTheming = !Item.AllowGlyphTheming.HasValue ? Manager.AllowGlyphTheming : Item.AllowGlyphTheming.Value;
			}
			if (LayoutPanel != null)
				LayoutPanel.ColorizeGlyph = ActualAllowGlyphTheming;
		}
		protected virtual void UpdateLayoutPanelGlyphTemplate() {
			if (LayoutPanel == null)
				return;
			LayoutPanel.GlyphTemplate = ActualGlyphTemplate;
		}
		protected internal virtual void UpdateLayoutPanelHorizontalItemPosition() {
			if (LayoutPanel == null)
				return;
			LayoutPanel.ItemPosition = ItemPositionTypeProvider.GetHorizontalItemPosition(this);
		}
		protected virtual void UpdateLayoutPanelGlyphSize() {
			if (LayoutPanel == null)
				return;
			if (IsLargeGlyph) {
				LayoutPanel.GlyphSize = GlyphSize.Large;
			} else {
				LayoutPanel.GlyphSize = GlyphSize.Small;
			}
		}
		protected virtual void UpdateLayoutPanelFontSettingsByContainerType() {
			if (LayoutPanel == null || LayoutPanel.ContentFontSettings != null)
				return;
			switch (ContainerType) {
				case LinkContainerType.MainMenu: 
					AssignLayoutPanelContentFontSettings(BarItemFontKeys.BarItemFontSettingsInMainMenu);
					AssignLayoutPaneImageColorizerSettings(BarItemImageColorizerSettingsKeys.SettingsInMainMenu);
					break;
				case LinkContainerType.StatusBar: 
					AssignLayoutPanelContentFontSettings(BarItemFontKeys.BarItemFontSettingsInStatusBar);
					AssignLayoutPaneImageColorizerSettings(BarItemImageColorizerSettingsKeys.SettingsInStatusBar);
					break;
				case LinkContainerType.Menu: 
				case LinkContainerType.DropDownGallery: 
					AssignLayoutPanelContentFontSettings(BarItemFontKeys.BarItemFontSettingsInMenu);
					AssignLayoutPaneImageColorizerSettings(BarItemImageColorizerSettingsKeys.SettingsInMenu);
					break;
				case LinkContainerType.RibbonQuickAccessToolbar: 
					AssignLayoutPanelContentFontSettings(BarItemFontKeys.BarItemFontSettingsInRibbonToolbar);
					AssignLayoutPaneImageColorizerSettings(BarItemImageColorizerSettingsKeys.SettingsInRibbonToolbar);
					break;
				case LinkContainerType.RibbonPageGroup:
					AssignLayoutPanelContentFontSettings(BarItemFontKeys.BarItemFontSettingsInRibbonPageGroup);
					AssignLayoutPaneImageColorizerSettings(BarItemImageColorizerSettingsKeys.SettingsInRibbonPageGroup);
					break;
				case LinkContainerType.BarButtonGroup:
					AssignLayoutPanelContentFontSettings(BarItemFontKeys.BarItemFontSettingsInButtonGroup);
					AssignLayoutPaneImageColorizerSettings(BarItemImageColorizerSettingsKeys.SettingsInButtonGroup);
					break;
				case LinkContainerType.RibbonPageHeader:
					AssignLayoutPanelContentFontSettings(BarItemFontKeys.BarItemFontSettingsInRibbonPageHeader);
					AssignLayoutPaneImageColorizerSettings(BarItemImageColorizerSettingsKeys.SettingsInRibbonPageHeader);
					break;
				case LinkContainerType.RibbonStatusBarLeft:
					AssignLayoutPanelContentFontSettings(BarItemFontKeys.BarItemFontSettingsInRibbonStatusBarLeft);
					AssignLayoutPaneImageColorizerSettings(BarItemImageColorizerSettingsKeys.SettingsInRibbonStatusBarLeft);
					break;
				case LinkContainerType.RibbonStatusBarRight:
					AssignLayoutPanelContentFontSettings(BarItemFontKeys.BarItemFontSettingsInRibbonStatusBarRight);
					AssignLayoutPaneImageColorizerSettings(BarItemImageColorizerSettingsKeys.SettingsInRibbonStatusBarRight);
					break;
				case LinkContainerType.ApplicationMenu:
					AssignLayoutPanelContentFontSettings(BarItemFontKeys.BarItemFontSettingsInApplicationMenu);
					AssignLayoutPanelDescriptionFontSettings(BarItemFontKeys.BarItemDescriptionStyleInApplicationMenu);
					AssignLayoutPaneImageColorizerSettings(BarItemImageColorizerSettingsKeys.SettingsInApplicationMenu);
					break;
				case LinkContainerType.RibbonQuickAccessToolbarFooter:
					AssignLayoutPanelContentFontSettings(BarItemFontKeys.BarItemFontSettingsInRibbonToolbarFooter);
					AssignLayoutPaneImageColorizerSettings(BarItemImageColorizerSettingsKeys.SettingsInRibbonToolbarFooter);
					break;
				case LinkContainerType.MenuHeader: 
					AssignLayoutPaneImageColorizerSettings(BarItemImageColorizerSettingsKeys.SettingsInMenu);
					break;
				default:
					AssignLayoutPanelContentFontSettings(BarItemFontKeys.BarItemFontSettings);
					AssignLayoutPaneImageColorizerSettings(BarItemImageColorizerSettingsKeys.DefaultSettings);
					break;
			}
		}
		void AssignLayoutPanelContentFontSettings(BarItemFontKeys key) {
			LayoutPanel.ContentFontSettings = ResourceStorage.TryFindResourceInStorage(ResourceSource, new BarItemFontThemeKeyExtension(key) { ThemeName = ThemeName }) as FontSettings;
		}
		void AssignLayoutPanelDescriptionFontSettings(BarItemFontKeys key) {
			LayoutPanel.DescriptionFontSettings = ResourceStorage.TryFindResourceInStorage(ResourceSource, new BarItemFontThemeKeyExtension(key) { ThemeName = ThemeName }) as FontSettings;
		}
		void AssignLayoutPaneImageColorizerSettings(BarItemImageColorizerSettingsKeys key) {
			LayoutPanel.ImageColorizerSettings = ResourceStorage.TryFindResourceInStorage(ResourceSource, new BarItemImageColorizerSettingsThemeKeyExtension(key) { ThemeName = ThemeName }) as BarItemImageColorizerSettings;
		}
		protected virtual void UpdateLayoutPanelContentTemplate() {
			if (LayoutPanel == null)
				return;
			LayoutPanel.ContentTemplate = ActualContentTemplate;
		}
		protected virtual void UpdateLayoutPanelContentTemplateSelector() {
			if (LayoutPanel == null)
				return;
			LayoutPanel.ContentTemplateSelector = ActualContentTemplateSelector;
		}
		protected virtual void UpdateLayoutPanelGlyphToContentAlignment() {
			if (LayoutPanel == null)
				return;
			LayoutPanel.GlyphToContentAlignment = ActualGlyphAlignment;
		}
		protected virtual void UpdateLayoutPanelActualGlyph() {
			if (LayoutPanel == null || ActualBarItemDisplayMode == BarItemDisplayMode.Content)
				return;
			LayoutPanel.ActualGlyph = ActualGlyph;
		}
		protected virtual void UpdateLayoutPanelShowContent() {
			if (LayoutPanel == null)
				return;
			LayoutPanel.ShowContent = ActualShowContent;
		}
		protected virtual void UpdateLayoutPanelShowGlyph() {
			if (LayoutPanel == null)
				return;
			LayoutPanel.ShowGlyph = ActualShowGlyph;
			UpdateLayoutPanelActualGlyph();
		}
		protected virtual void UpdateLayoutPanelShowDescription() {
			if (LayoutPanel == null)
				return;
			LayoutPanel.ShowDescription = ShowDescription;
		}
		protected virtual void UpdateLayoutPanelShowKeyGesture() {
			if (LayoutPanel == null)
				return;
			LayoutPanel.ShowKeyGesture = ShowKeyGesture;
		}
		protected virtual void UpdateLayoutPanelContent() {
			if (LayoutPanel == null)
				return;
			LayoutPanel.Content = ActualContent;
		}
		protected virtual void UpdateLayoutPanelDescription() {
			if (LayoutPanel == null)
				return;
			LayoutPanel.Description = ActualDescription;
		}
		protected virtual void UpdateLayoutPanelKeyGesture() {
			if (LayoutPanel == null)
				return;
			LayoutPanel.KeyGesture = ActualKeyGestureText;
		}
		protected virtual void UpdateLayoutPanelSplitContent() {
			if (LayoutPanel == null)
				return;
			LayoutPanel.SplitContent = IsRibbonStyleLarge;
		}
		protected virtual void UpdateLayoutPanelHorizontalAlignment() {
			if (LayoutPanel == null)
				return;
			LayoutPanel.HorizontalAlignment = HorizontalAlignment.Stretch;
		}
		internal Point TranslatePointWithoutTransform(Point point) {
			if (RenderTransform == null)
				return point;
			return RenderTransform.Inverse.Transform(point);
		}
		protected internal virtual bool CanSelectOnHoverInMenuMode {
			get { return true; }
		}
		bool IsInQuickAccessToolbar() {
			return LinkInfo.LinkContainerType == LinkContainerType.RibbonQuickAccessToolbar || LinkInfo.LinkContainerType == LinkContainerType.RibbonQuickAccessToolbarFooter;
		}
		protected internal override void OnClear() {
			base.OnClear();
			if (DragDropHelper != null)
				DragDropHelper.Destroy();
		}
		protected internal virtual void OnSourceContentChanged() {
			UpdateActualContent();
			UpdateActualShowGlyphInRibbon();
			RecreateToolTip();
		}
		protected internal virtual void UpdateActualContent() {
			ActualContent = GetContent();
			UpdateActualShowContent();
		}
		protected virtual object GetContent() {
			if (Content != null)
				return Content;
			if (Link != null && Link.UserContent != null)
				return Link.UserContent;
			if (Item != null)
				return Item.GetContent();
			return null;
		}
		void UpdateShowToolTipOnDisabled() {
			if (LinkBase != null && ToolTipService.GetShowOnDisabled(LinkBase)) {
				ToolTipService.SetShowOnDisabled(this, true);
				UpdateToolTip();
				return;
			}
			if (Item != null && ToolTipService.GetShowOnDisabled(Item)) {
				ToolTipService.SetShowOnDisabled(this, true);
				UpdateToolTip();
			}
		}
		protected override void OnSpacingModeChanged(SpacingMode oldValue) {
			UpdateLayoutPanelSpacingMode();
		}
		protected internal override void UpdateActualProperties() {
			base.UpdateActualProperties();
			UpdateShowToolTipOnDisabled();
			UpdateActualBarItemDisplayMode();
			UpdateActualGlyphAlignment();
			UpdateActualContent();
			UpdateActualContentTemplate();
			UpdateActualContentTemplateSelector();
			UpdateActualKeyGestureText();
			UpdateActualDescription();
			UpdateShowKeyGesture();
			UpdateActualShowContent();
			UpdateActualGlyphTemplate();
			UpdateActualGlyph();
			UpdateActualShowCustomizationBorder();
			UpdateShowDescription();
			UpdateActualAllowGlyphTheming();
		}
	}
	public class RibbonItemInfo {
		public RibbonItemInfo(BarItemLinkControl linkControl) {
			LinkControl = linkControl;
		}
		public BarItemLinkControl LinkControl { get; private set; }
		public RibbonItemStyles CurrentLevel { get { return LinkControl.CurrentRibbonStyle; } }
		public bool IsLargeButton { get { return CurrentLevel == RibbonItemStyles.Large; } }
	}
	public class BarItemLinkControlTemplateProvider : DependencyObject {
		public static readonly DependencyProperty BorderStyleInBarProperty = DependencyPropertyManager.RegisterAttached("BorderStyleInBar", typeof(Style), typeof(BarItemLinkControlTemplateProvider), new UIPropertyMetadata(null));
		public static readonly DependencyProperty BorderStyleInMainMenuProperty = DependencyPropertyManager.RegisterAttached("BorderStyleInMainMenu", typeof(Style), typeof(BarItemLinkControlTemplateProvider), new UIPropertyMetadata(null));
		public static readonly DependencyProperty BorderStyleInMenuProperty = DependencyPropertyManager.RegisterAttached("BorderStyleInMenu", typeof(Style), typeof(BarItemLinkControlTemplateProvider), new UIPropertyMetadata(null));
		public static readonly DependencyProperty BorderStyleInMenuHorizontalProperty = DependencyPropertyManager.RegisterAttached("BorderStyleInMenuHorizontal", typeof(Style), typeof(BarItemLinkControlTemplateProvider), new UIPropertyMetadata(null));
		public static readonly DependencyProperty BorderStyleInApplicationMenuProperty = DependencyPropertyManager.RegisterAttached("BorderStyleInApplicationMenu", typeof(Style), typeof(BarItemLinkControlTemplateProvider), new UIPropertyMetadata(null));
		public static readonly DependencyProperty BorderStyleInStatusBarProperty = DependencyPropertyManager.RegisterAttached("BorderStyleInStatusBar", typeof(Style), typeof(BarItemLinkControlTemplateProvider), new UIPropertyMetadata(null));
		public static readonly DependencyProperty BorderStyleInRibbonPageGroupProperty = DependencyPropertyManager.RegisterAttached("BorderStyleInRibbonPageGroup", typeof(Style), typeof(BarItemLinkControlTemplateProvider), new UIPropertyMetadata(null));
		public static readonly DependencyProperty BorderStyleInButtonGroupProperty = DependencyPropertyManager.RegisterAttached("BorderStyleInButtonGroup", typeof(Style), typeof(BarItemLinkControlTemplateProvider), new UIPropertyMetadata(null));
		public static readonly DependencyProperty BorderStyleInQuickAccessToolbarProperty = DependencyPropertyManager.RegisterAttached("BorderStyleInQuickAccessToolbar", typeof(Style), typeof(BarItemLinkControlTemplateProvider), new UIPropertyMetadata(null));
		public static readonly DependencyProperty BorderStyleInQuickAccessToolbarFooterProperty = DependencyPropertyManager.RegisterAttached("BorderStyleInQuickAccessToolbarFooter", typeof(Style), typeof(BarItemLinkControlTemplateProvider), new UIPropertyMetadata(null));
		public static readonly DependencyProperty BorderStyleInRibbonPageHeaderProperty = DependencyPropertyManager.RegisterAttached("BorderStyleInRibbonPageHeader", typeof(Style), typeof(BarItemLinkControlTemplateProvider), new UIPropertyMetadata(null));
		public static readonly DependencyProperty BorderStyleInRibbonStatusBarLeftProperty = DependencyPropertyManager.RegisterAttached("BorderStyleInRibbonStatusBarLeft", typeof(Style), typeof(BarItemLinkControlTemplateProvider), new UIPropertyMetadata(null));
		public static readonly DependencyProperty BorderStyleInRibbonStatusBarRightProperty = DependencyPropertyManager.RegisterAttached("BorderStyleInRibbonStatusBarRight", typeof(Style), typeof(BarItemLinkControlTemplateProvider), new UIPropertyMetadata(null));
		public static readonly DependencyProperty NormalContentStyleInBarProperty = DependencyPropertyManager.RegisterAttached("NormalContentStyleInBar", typeof(Style), typeof(BarItemLinkControlTemplateProvider), new UIPropertyMetadata(null));
		public static readonly DependencyProperty HotContentStyleInBarProperty = DependencyPropertyManager.RegisterAttached("HotContentStyleInBar", typeof(Style), typeof(BarItemLinkControlTemplateProvider), new UIPropertyMetadata(null));
		public static readonly DependencyProperty PressedContentStyleInBarProperty = DependencyPropertyManager.RegisterAttached("PressedContentStyleInBar", typeof(Style), typeof(BarItemLinkControlTemplateProvider), new UIPropertyMetadata(null));
		public static readonly DependencyProperty DisabledContentStyleInBarProperty = DependencyPropertyManager.RegisterAttached("DisabledContentStyleInBar", typeof(Style), typeof(BarItemLinkControlTemplateProvider), new UIPropertyMetadata(null));
		public static readonly DependencyProperty NormalContentStyleInMainMenuProperty = DependencyPropertyManager.RegisterAttached("NormalContentStyleInMainMenu", typeof(Style), typeof(BarItemLinkControlTemplateProvider), new UIPropertyMetadata(null));
		public static readonly DependencyProperty HotContentStyleInMainMenuProperty = DependencyPropertyManager.RegisterAttached("HotContentStyleInMainMenu", typeof(Style), typeof(BarItemLinkControlTemplateProvider), new UIPropertyMetadata(null));
		public static readonly DependencyProperty PressedContentStyleInMainMenuProperty = DependencyPropertyManager.RegisterAttached("PressedContentStyleInMainMenu", typeof(Style), typeof(BarItemLinkControlTemplateProvider), new UIPropertyMetadata(null));
		public static readonly DependencyProperty DisabledContentStyleInMainMenuProperty = DependencyPropertyManager.RegisterAttached("DisabledContentStyleInMainMenu", typeof(Style), typeof(BarItemLinkControlTemplateProvider), new UIPropertyMetadata(null));
		public static readonly DependencyProperty NormalContentStyleInMenuProperty = DependencyPropertyManager.RegisterAttached("NormalContentStyleInMenu", typeof(Style), typeof(BarItemLinkControlTemplateProvider), new UIPropertyMetadata(null));
		public static readonly DependencyProperty HotContentStyleInMenuProperty = DependencyPropertyManager.RegisterAttached("HotContentStyleInMenu", typeof(Style), typeof(BarItemLinkControlTemplateProvider), new UIPropertyMetadata(null));
		public static readonly DependencyProperty PressedContentStyleInMenuProperty = DependencyPropertyManager.RegisterAttached("PressedContentStyleInMenu", typeof(Style), typeof(BarItemLinkControlTemplateProvider), new UIPropertyMetadata(null));
		public static readonly DependencyProperty DisabledContentStyleInMenuProperty = DependencyPropertyManager.RegisterAttached("DisabledContentStyleInMenu", typeof(Style), typeof(BarItemLinkControlTemplateProvider), new UIPropertyMetadata(null));
		public static readonly DependencyProperty NormalContentStyleInApplicationMenuProperty = DependencyPropertyManager.RegisterAttached("NormalContentStyleInApplicationMenu", typeof(Style), typeof(BarItemLinkControlTemplateProvider), new UIPropertyMetadata(null));
		public static readonly DependencyProperty HotContentStyleInApplicationMenuProperty = DependencyPropertyManager.RegisterAttached("HotContentStyleInApplicationMenu", typeof(Style), typeof(BarItemLinkControlTemplateProvider), new UIPropertyMetadata(null));
		public static readonly DependencyProperty PressedContentStyleInApplicationMenuProperty = DependencyPropertyManager.RegisterAttached("PressedContentStyleInApplicationMenu", typeof(Style), typeof(BarItemLinkControlTemplateProvider), new UIPropertyMetadata(null));
		public static readonly DependencyProperty DisabledContentStyleInApplicationMenuProperty = DependencyPropertyManager.RegisterAttached("DisabledContentStyleInApplicationMenu", typeof(Style), typeof(BarItemLinkControlTemplateProvider), new UIPropertyMetadata(null));
		public static readonly DependencyProperty NormalDescriptionStyleInApplicationMenuProperty = DependencyPropertyManager.RegisterAttached("NormalDescriptionStyleInApplicationMenu", typeof(Style), typeof(BarItemLinkControlTemplateProvider), new UIPropertyMetadata(null));
		public static readonly DependencyProperty HotDescriptionStyleInApplicationMenuProperty = DependencyPropertyManager.RegisterAttached("HotDescriptionStyleInApplicationMenu", typeof(Style), typeof(BarItemLinkControlTemplateProvider), new UIPropertyMetadata(null));
		public static readonly DependencyProperty PressedDescriptionStyleInApplicationMenuProperty = DependencyPropertyManager.RegisterAttached("PressedDescriptionStyleInApplicationMenu", typeof(Style), typeof(BarItemLinkControlTemplateProvider), new UIPropertyMetadata(null));
		public static readonly DependencyProperty DisabledDescriptionStyleInApplicationMenuProperty = DependencyPropertyManager.RegisterAttached("DisabledDescriptionStyleInApplicationMenu", typeof(Style), typeof(BarItemLinkControlTemplateProvider), new UIPropertyMetadata(null));
		public static readonly DependencyProperty NormalContentStyleInStatusBarProperty = DependencyPropertyManager.RegisterAttached("NormalContentStyleInStatusBar", typeof(Style), typeof(BarItemLinkControlTemplateProvider), new UIPropertyMetadata(null));
		public static readonly DependencyProperty HotContentStyleInStatusBarProperty = DependencyPropertyManager.RegisterAttached("HotContentStyleInStatusBar", typeof(Style), typeof(BarItemLinkControlTemplateProvider), new UIPropertyMetadata(null));
		public static readonly DependencyProperty PressedContentStyleInStatusBarProperty = DependencyPropertyManager.RegisterAttached("PressedContentStyleInStatusBar", typeof(Style), typeof(BarItemLinkControlTemplateProvider), new UIPropertyMetadata(null));
		public static readonly DependencyProperty DisabledContentStyleInStatusBarProperty = DependencyPropertyManager.RegisterAttached("DisabledContentStyleInStatusBar", typeof(Style), typeof(BarItemLinkControlTemplateProvider), new UIPropertyMetadata(null));
		public static readonly DependencyProperty NormalContentStyleInRibbonPageGroupProperty = DependencyPropertyManager.RegisterAttached("NormalContentStyleInRibbonPageGroup", typeof(Style), typeof(BarItemLinkControlTemplateProvider), new UIPropertyMetadata(null));
		public static readonly DependencyProperty HotContentStyleInRibbonPageGroupProperty = DependencyPropertyManager.RegisterAttached("HotContentStyleInRibbonPageGroup", typeof(Style), typeof(BarItemLinkControlTemplateProvider), new UIPropertyMetadata(null));
		public static readonly DependencyProperty PressedContentStyleInRibbonPageGroupProperty = DependencyPropertyManager.RegisterAttached("PressedContentStyleInRibbonPageGroup", typeof(Style), typeof(BarItemLinkControlTemplateProvider), new UIPropertyMetadata(null));
		public static readonly DependencyProperty DisabledContentStyleInRibbonPageGroupProperty = DependencyPropertyManager.RegisterAttached("DisabledContentStyleInRibbonPageGroup", typeof(Style), typeof(BarItemLinkControlTemplateProvider), new UIPropertyMetadata(null));
		public static readonly DependencyProperty NormalContentStyleInButtonGroupProperty = DependencyPropertyManager.RegisterAttached("NormalContentStyleInButtonGroup", typeof(Style), typeof(BarItemLinkControlTemplateProvider), new UIPropertyMetadata(null));
		public static readonly DependencyProperty HotContentStyleInButtonGroupProperty = DependencyPropertyManager.RegisterAttached("HotContentStyleInButtonGroup", typeof(Style), typeof(BarItemLinkControlTemplateProvider), new UIPropertyMetadata(null));
		public static readonly DependencyProperty PressedContentStyleInButtonGroupProperty = DependencyPropertyManager.RegisterAttached("PressedContentStyleInButtonGroup", typeof(Style), typeof(BarItemLinkControlTemplateProvider), new UIPropertyMetadata(null));
		public static readonly DependencyProperty DisabledContentStyleInButtonGroupProperty = DependencyPropertyManager.RegisterAttached("DisabledContentStyleInButtonGroup", typeof(Style), typeof(BarItemLinkControlTemplateProvider), new UIPropertyMetadata(null));
		public static readonly DependencyProperty NormalContentStyleInQuickAccessToolbarProperty = DependencyPropertyManager.RegisterAttached("NormalContentStyleInQuickAccessToolbar", typeof(Style), typeof(BarItemLinkControlTemplateProvider), new UIPropertyMetadata(null));
		public static readonly DependencyProperty HotContentStyleInQuickAccessToolbarProperty = DependencyPropertyManager.RegisterAttached("HotContentStyleInQuickAccessToolbar", typeof(Style), typeof(BarItemLinkControlTemplateProvider), new UIPropertyMetadata(null));
		public static readonly DependencyProperty PressedContentStyleInQuickAccessToolbarProperty = DependencyPropertyManager.RegisterAttached("PressedContentStyleInQuickAccessToolbar", typeof(Style), typeof(BarItemLinkControlTemplateProvider), new UIPropertyMetadata(null));
		public static readonly DependencyProperty DisabledContentStyleInQuickAccessToolbarProperty = DependencyPropertyManager.RegisterAttached("DisabledContentStyleInQuickAccessToolbar", typeof(Style), typeof(BarItemLinkControlTemplateProvider), new UIPropertyMetadata(null));
		public static readonly DependencyProperty NormalContentStyleInQuickAccessToolbarFooterProperty = DependencyPropertyManager.RegisterAttached("NormalContentStyleInQuickAccessToolbarFooter", typeof(Style), typeof(BarItemLinkControlTemplateProvider), new UIPropertyMetadata(null));
		public static readonly DependencyProperty HotContentStyleInQuickAccessToolbarFooterProperty = DependencyPropertyManager.RegisterAttached("HotContentStyleInQuickAccessToolbarFooter", typeof(Style), typeof(BarItemLinkControlTemplateProvider), new UIPropertyMetadata(null));
		public static readonly DependencyProperty PressedContentStyleInQuickAccessToolbarFooterProperty = DependencyPropertyManager.RegisterAttached("PressedContentStyleInQuickAccessToolbarFooter", typeof(Style), typeof(BarItemLinkControlTemplateProvider), new UIPropertyMetadata(null));
		public static readonly DependencyProperty DisabledContentStyleInQuickAccessToolbarFooterProperty = DependencyPropertyManager.RegisterAttached("DisabledContentStyleInQuickAccessToolbarFooter", typeof(Style), typeof(BarItemLinkControlTemplateProvider), new UIPropertyMetadata(null));
		public static readonly DependencyProperty NormalContentStyleInRibbonPageHeaderProperty = DependencyPropertyManager.RegisterAttached("NormalContentStyleInRibbonPageHeader", typeof(Style), typeof(BarItemLinkControlTemplateProvider), new UIPropertyMetadata(null));
		public static readonly DependencyProperty HotContentStyleInRibbonPageHeaderProperty = DependencyPropertyManager.RegisterAttached("HotContentStyleInRibbonPageHeader", typeof(Style), typeof(BarItemLinkControlTemplateProvider), new UIPropertyMetadata(null));
		public static readonly DependencyProperty PressedContentStyleInRibbonPageHeaderProperty = DependencyPropertyManager.RegisterAttached("PressedContentStyleInRibbonPageHeader", typeof(Style), typeof(BarItemLinkControlTemplateProvider), new UIPropertyMetadata(null));
		public static readonly DependencyProperty DisabledContentStyleInRibbonPageHeaderProperty = DependencyPropertyManager.RegisterAttached("DisabledContentStyleInRibbonPageHeader", typeof(Style), typeof(BarItemLinkControlTemplateProvider), new UIPropertyMetadata(null));
		public static readonly DependencyProperty NormalContentStyleInRibbonStatusBarLeftProperty = DependencyPropertyManager.RegisterAttached("NormalContentStyleInRibbonStatusBarLeft", typeof(Style), typeof(BarItemLinkControlTemplateProvider), new UIPropertyMetadata(null));
		public static readonly DependencyProperty HotContentStyleInRibbonStatusBarLeftProperty = DependencyPropertyManager.RegisterAttached("HotContentStyleInRibbonStatusBarLeft", typeof(Style), typeof(BarItemLinkControlTemplateProvider), new UIPropertyMetadata(null));
		public static readonly DependencyProperty PressedContentStyleInRibbonStatusBarLeftProperty = DependencyPropertyManager.RegisterAttached("PressedContentStyleInRibbonStatusBarLeft", typeof(Style), typeof(BarItemLinkControlTemplateProvider), new UIPropertyMetadata(null));
		public static readonly DependencyProperty DisabledContentStyleInRibbonStatusBarLeftProperty = DependencyPropertyManager.RegisterAttached("DisabledContentStyleInRibbonStatusBarLeft", typeof(Style), typeof(BarItemLinkControlTemplateProvider), new UIPropertyMetadata(null));
		public static readonly DependencyProperty NormalContentStyleInRibbonStatusBarRightProperty = DependencyPropertyManager.RegisterAttached("NormalContentStyleInRibbonStatusBarRight", typeof(Style), typeof(BarItemLinkControlTemplateProvider), new UIPropertyMetadata(null));
		public static readonly DependencyProperty HotContentStyleInRibbonStatusBarRightProperty = DependencyPropertyManager.RegisterAttached("HotContentStyleInRibbonStatusBarRight", typeof(Style), typeof(BarItemLinkControlTemplateProvider), new UIPropertyMetadata(null));
		public static readonly DependencyProperty PressedContentStyleInRibbonStatusBarRightProperty = DependencyPropertyManager.RegisterAttached("PressedContentStyleInRibbonStatusBarRight", typeof(Style), typeof(BarItemLinkControlTemplateProvider), new UIPropertyMetadata(null));
		public static readonly DependencyProperty DisabledContentStyleInRibbonStatusBarRightProperty = DependencyPropertyManager.RegisterAttached("DisabledContentStyleInRibbonStatusBarRight", typeof(Style), typeof(BarItemLinkControlTemplateProvider), new UIPropertyMetadata(null));
		public static readonly DependencyProperty LayoutPanelStyleInBarProperty = DependencyPropertyManager.RegisterAttached("LayoutPanelStyleInBar", typeof(Style), typeof(BarItemLinkControlTemplateProvider), new UIPropertyMetadata(null));
		public static readonly DependencyProperty LayoutPanelStyleInMainMenuProperty = DependencyPropertyManager.RegisterAttached("LayoutPanelStyleInMainMenu", typeof(Style), typeof(BarItemLinkControlTemplateProvider), new UIPropertyMetadata(null));
		public static readonly DependencyProperty LayoutPanelStyleInStatusBarProperty = DependencyPropertyManager.RegisterAttached("LayoutPanelStyleInStatusBar", typeof(Style), typeof(BarItemLinkControlTemplateProvider), new UIPropertyMetadata(null));
		public static readonly DependencyProperty LayoutPanelStyleInRibbonPageGroupProperty = DependencyPropertyManager.RegisterAttached("LayoutPanelStyleInRibbonPageGroup", typeof(Style), typeof(BarItemLinkControlTemplateProvider), new UIPropertyMetadata(null));
		public static readonly DependencyProperty LayoutPanelStyleInButtonGroupProperty = DependencyPropertyManager.RegisterAttached("LayoutPanelStyleInButtonGroup", typeof(Style), typeof(BarItemLinkControlTemplateProvider), new UIPropertyMetadata(null));
		public static readonly DependencyProperty LayoutPanelStyleInQuickAccessToolbarProperty = DependencyPropertyManager.RegisterAttached("LayoutPanelStyleInQuickAccessToolbar", typeof(Style), typeof(BarItemLinkControlTemplateProvider), new UIPropertyMetadata(null));
		public static readonly DependencyProperty LayoutPanelStyleInQuickAccessToolbarFooterProperty = DependencyPropertyManager.RegisterAttached("LayoutPanelStyleInQuickAccessToolbarFooter", typeof(Style), typeof(BarItemLinkControlTemplateProvider), new UIPropertyMetadata(null));
		public static readonly DependencyProperty LayoutPanelStyleInRibbonPageHeaderProperty = DependencyPropertyManager.RegisterAttached("LayoutPanelStyleInRibbonPageHeader", typeof(Style), typeof(BarItemLinkControlTemplateProvider), new UIPropertyMetadata(null));
		public static readonly DependencyProperty LayoutPanelStyleInRibbonStatusBarLeftProperty = DependencyPropertyManager.RegisterAttached("LayoutPanelStyleInRibbonStatusBarLeft", typeof(Style), typeof(BarItemLinkControlTemplateProvider), new UIPropertyMetadata(null));
		public static readonly DependencyProperty LayoutPanelStyleInRibbonStatusBarRightProperty = DependencyPropertyManager.RegisterAttached("LayoutPanelStyleInRibbonStatusBarRight", typeof(Style), typeof(BarItemLinkControlTemplateProvider), new UIPropertyMetadata(null));
		public static readonly DependencyProperty DisableStateOpacityProperty = DependencyPropertyManager.RegisterAttached("DisableStateOpacity", typeof(double), typeof(BarItemLinkControlTemplateProvider), new UIPropertyMetadata(double.NaN));
		public static readonly DependencyProperty TemplateProperty = DependencyPropertyManager.RegisterAttached("Template", typeof(ControlTemplate), typeof(BarItemLinkControlTemplateProvider), new UIPropertyMetadata(null, new PropertyChangedCallback(OnTemplateChanged)));
		public static readonly DependencyProperty TemplateInMenuProperty = DependencyPropertyManager.RegisterAttached("TemplateInMenu", typeof(ControlTemplate), typeof(BarItemLinkControlTemplateProvider), new UIPropertyMetadata(null));
		public static readonly DependencyProperty TemplateInMenuHorizontalProperty = DependencyPropertyManager.RegisterAttached("TemplateInMenuHorizontal", typeof(ControlTemplate), typeof(BarItemLinkControlTemplateProvider), new UIPropertyMetadata(null));
		public static readonly DependencyProperty TemplateInRibbonPageGroupProperty = DependencyPropertyManager.RegisterAttached("TemplateInRibbonPageGroup", typeof(ControlTemplate), typeof(BarItemLinkControlTemplateProvider), new UIPropertyMetadata(null));
		public static readonly DependencyProperty TemplateInRibbonStatusBarLeftPartProperty = DependencyPropertyManager.RegisterAttached("TemplateInRibbonStatusBarLeftPart", typeof(ControlTemplate), typeof(BarItemLinkControlTemplateProvider), new UIPropertyMetadata(null));
		public static readonly DependencyProperty TemplateInRibbonStatusBarRightPartProperty = DependencyPropertyManager.RegisterAttached("TemplateInRibbonStatusBarRightPart", typeof(ControlTemplate), typeof(BarItemLinkControlTemplateProvider), new UIPropertyMetadata(null));
		public static readonly DependencyProperty TemplateInStatusBarProperty = DependencyPropertyManager.RegisterAttached("TemplateInStatusBar", typeof(ControlTemplate), typeof(BarItemLinkControlTemplateProvider), new UIPropertyMetadata(null));
		public static readonly DependencyProperty TemplateInRibbonQuickAccessToolbarProperty = DependencyPropertyManager.RegisterAttached("TemplateInRibbonQuickAccessToolbar", typeof(ControlTemplate), typeof(BarItemLinkControlTemplateProvider), new UIPropertyMetadata(null));
		public static readonly DependencyProperty TemplateInRibbonQuickAccessToolbarFooterProperty = DependencyPropertyManager.RegisterAttached("TemplateInRibbonQuickAccessToolbarFooter", typeof(ControlTemplate), typeof(BarItemLinkControlTemplateProvider), new UIPropertyMetadata(null));		
		public static readonly DependencyProperty TemplateInRibbonPageHeaderProperty = DependencyPropertyManager.RegisterAttached("TemplateInRibbonPageHeader", typeof(ControlTemplate), typeof(BarItemLinkControlTemplateProvider), new UIPropertyMetadata(null));
		static void OnTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) { 
		}
		public static ControlTemplate GetTemplateInRibbonPageGroup(DependencyObject target) {
			return (ControlTemplate)target.GetValue(TemplateInRibbonPageGroupProperty);
		}
		public static void SetTemplateInRibbonPageGroup(DependencyObject target, ControlTemplate value) {
			target.SetValue(TemplateInRibbonPageGroupProperty, value);
		}
		public static ControlTemplate GetTemplateInMenu(DependencyObject target) {
			return (ControlTemplate)target.GetValue(TemplateInMenuProperty);
		}
		public static void SetTemplateInMenu(DependencyObject target, ControlTemplate value) {
			target.SetValue(TemplateInMenuProperty, value);
		}
		public static ControlTemplate GetTemplateInRibbonStatusBarRightPart(DependencyObject target) {
			return (ControlTemplate)target.GetValue(TemplateInRibbonStatusBarRightPartProperty);
		}
		public static void SetTemplateInRibbonStatusBarRightPart(DependencyObject target, ControlTemplate value) {
			target.SetValue(TemplateInRibbonStatusBarRightPartProperty, value);
		}
		public static ControlTemplate GetTemplateInRibbonStatusBarLeftPart(DependencyObject target) {
			return (ControlTemplate)target.GetValue(TemplateInRibbonStatusBarLeftPartProperty);
		}
		public static void SetTemplateInRibbonStatusBarLeftPart(DependencyObject target, ControlTemplate value) {
			target.SetValue(TemplateInRibbonStatusBarLeftPartProperty, value);
		}
		public static ControlTemplate GetTemplateInStatusBar(DependencyObject target) {
			return (ControlTemplate)target.GetValue(TemplateInStatusBarProperty);
		}		
		public static void SetTemplateInStatusBar(DependencyObject target, ControlTemplate value) {
			target.SetValue(TemplateInStatusBarProperty, value);
		}			   
		public static ControlTemplate GetTemplateInMenuHorizontal(DependencyObject target) {
			return (ControlTemplate)target.GetValue(TemplateInMenuHorizontalProperty);
		}
		public static void SetTemplateInMenuHorizontal(DependencyObject target, ControlTemplate value) {
			target.SetValue(TemplateInMenuHorizontalProperty, value);
		}
		public static ControlTemplate GetTemplate(DependencyObject target) {
			return (ControlTemplate)target.GetValue(TemplateProperty);
		}
		public static void SetTemplate(DependencyObject target, ControlTemplate value) {
			target.SetValue(TemplateProperty, value);
		}
		public static ControlTemplate GetTemplateInRibbonQuickAccessToolbar(DependencyObject target) {
			return (ControlTemplate)target.GetValue(TemplateInRibbonQuickAccessToolbarProperty);
		}
		public static void SetTemplateInRibbonQuickAccessToolbar(DependencyObject target, ControlTemplate value) {
			target.SetValue(TemplateInRibbonQuickAccessToolbarProperty, value);
		}
		public static ControlTemplate GetTemplateInRibbonQuickAccessToolbarFooter(DependencyObject target) {
			return (ControlTemplate)target.GetValue(TemplateInRibbonQuickAccessToolbarFooterProperty);
		}
		public static void SetTemplateInRibbonQuickAccessToolbarFooter(DependencyObject target, ControlTemplate value) {
			target.SetValue(TemplateInRibbonQuickAccessToolbarFooterProperty, value);
		}
		public static ControlTemplate GetTemplateInRibbonPageHeader(DependencyObject target) {
			return (ControlTemplate)target.GetValue(TemplateInRibbonPageHeaderProperty);
		}
		public static void SetTemplateInRibbonPageHeader(DependencyObject target, ControlTemplate value) {
			target.SetValue(TemplateInRibbonPageHeaderProperty, value);
		}			
		public static double GetDisableStateOpacity(DependencyObject target) {
			return (double)target.GetValue(DisableStateOpacityProperty);
		}
		public static void SetDisableStateOpacity(DependencyObject target, double value) {
			target.SetValue(DisableStateOpacityProperty, value);
		}
		public static Style GetDisabledContentStyleInRibbonStatusBarRight(DependencyObject target) {
			return (Style)target.GetValue(DisabledContentStyleInRibbonStatusBarRightProperty);
		}
		public static void SetDisabledContentStyleInRibbonStatusBarRight(DependencyObject target, Style value) {
			target.SetValue(DisabledContentStyleInRibbonStatusBarRightProperty, value);
		}
		public static Style GetPressedContentStyleInRibbonStatusBarRight(DependencyObject target) {
			return (Style)target.GetValue(PressedContentStyleInRibbonStatusBarRightProperty);
		}
		public static void SetPressedContentStyleInRibbonStatusBarRight(DependencyObject target, Style value) {
			target.SetValue(PressedContentStyleInRibbonStatusBarRightProperty, value);
		}
		public static Style GetHotContentStyleInRibbonStatusBarRight(DependencyObject target) {
			return (Style)target.GetValue(HotContentStyleInRibbonStatusBarRightProperty);
		}
		public static void SetHotContentStyleInRibbonStatusBarRight(DependencyObject target, Style value) {
			target.SetValue(HotContentStyleInRibbonStatusBarRightProperty, value);
		}
		public static Style GetNormalContentStyleInRibbonStatusBarRight(DependencyObject target) {
			return (Style)target.GetValue(NormalContentStyleInRibbonStatusBarRightProperty);
		}
		public static void SetNormalContentStyleInRibbonStatusBarRight(DependencyObject target, Style value) {
			target.SetValue(NormalContentStyleInRibbonStatusBarRightProperty, value);
		}
		public static Style GetDisabledContentStyleInRibbonStatusBarLeft(DependencyObject target) {
			return (Style)target.GetValue(DisabledContentStyleInRibbonStatusBarLeftProperty);
		}
		public static void SetDisabledContentStyleInRibbonStatusBarLeft(DependencyObject target, Style value) {
			target.SetValue(DisabledContentStyleInRibbonStatusBarLeftProperty, value);
		}
		public static Style GetPressedContentStyleInRibbonStatusBarLeft(DependencyObject target) {
			return (Style)target.GetValue(PressedContentStyleInRibbonStatusBarLeftProperty);
		}
		public static void SetPressedContentStyleInRibbonStatusBarLeft(DependencyObject target, Style value) {
			target.SetValue(PressedContentStyleInRibbonStatusBarLeftProperty, value);
		}
		public static Style GetHotContentStyleInRibbonStatusBarLeft(DependencyObject target) {
			return (Style)target.GetValue(HotContentStyleInRibbonStatusBarLeftProperty);
		}
		public static void SetHotContentStyleInRibbonStatusBarLeft(DependencyObject target, Style value) {
			target.SetValue(HotContentStyleInRibbonStatusBarLeftProperty, value);
		}
		public static Style GetNormalContentStyleInRibbonStatusBarLeft(DependencyObject target) {
			return (Style)target.GetValue(NormalContentStyleInRibbonStatusBarLeftProperty);
		}
		public static void SetNormalContentStyleInRibbonStatusBarLeft(DependencyObject target, Style value) {
			target.SetValue(NormalContentStyleInRibbonStatusBarLeftProperty, value);
		}
		public static Style GetDisabledContentStyleInRibbonPageHeader(DependencyObject target) {
			return (Style)target.GetValue(DisabledContentStyleInRibbonPageHeaderProperty);
		}
		public static void SetDisabledContentStyleInRibbonPageHeader(DependencyObject target, Style value) {
			target.SetValue(DisabledContentStyleInRibbonPageHeaderProperty, value);
		}
		public static Style GetPressedContentStyleInRibbonPageHeader(DependencyObject target) {
			return (Style)target.GetValue(PressedContentStyleInRibbonPageHeaderProperty);
		}
		public static void SetPressedContentStyleInRibbonPageHeader(DependencyObject target, Style value) {
			target.SetValue(PressedContentStyleInRibbonPageHeaderProperty, value);
		}
		public static Style GetHotContentStyleInRibbonPageHeader(DependencyObject target) {
			return (Style)target.GetValue(HotContentStyleInRibbonPageHeaderProperty);
		}
		public static void SetHotContentStyleInRibbonPageHeader(DependencyObject target, Style value) {
			target.SetValue(HotContentStyleInRibbonPageHeaderProperty, value);
		}
		public static Style GetNormalContentStyleInRibbonPageHeader(DependencyObject target) {
			return (Style)target.GetValue(NormalContentStyleInRibbonPageHeaderProperty);
		}
		public static void SetNormalContentStyleInRibbonPageHeader(DependencyObject target, Style value) {
			target.SetValue(NormalContentStyleInRibbonPageHeaderProperty, value);
		}
		public static Style GetDisabledContentStyleInQuickAccessToolbarFooter(DependencyObject target) {
			return (Style)target.GetValue(DisabledContentStyleInQuickAccessToolbarFooterProperty);
		}
		public static void SetDisabledContentStyleInQuickAccessToolbarFooter(DependencyObject target, Style value) {
			target.SetValue(DisabledContentStyleInQuickAccessToolbarFooterProperty, value);
		}
		public static Style GetPressedContentStyleInQuickAccessToolbarFooter(DependencyObject target) {
			return (Style)target.GetValue(PressedContentStyleInQuickAccessToolbarFooterProperty);
		}
		public static void SetPressedContentStyleInQuickAccessToolbarFooter(DependencyObject target, Style value) {
			target.SetValue(PressedContentStyleInQuickAccessToolbarFooterProperty, value);
		}
		public static Style GetHotContentStyleInQuickAccessToolbarFooter(DependencyObject target) {
			return (Style)target.GetValue(HotContentStyleInQuickAccessToolbarFooterProperty);
		}
		public static void SetHotContentStyleInQuickAccessToolbarFooter(DependencyObject target, Style value) {
			target.SetValue(HotContentStyleInQuickAccessToolbarFooterProperty, value);
		}
		public static Style GetNormalContentStyleInQuickAccessToolbarFooter(DependencyObject target) {
			return (Style)target.GetValue(NormalContentStyleInQuickAccessToolbarFooterProperty);
		}
		public static void SetNormalContentStyleInQuickAccessToolbarFooter(DependencyObject target, Style value) {
			target.SetValue(NormalContentStyleInQuickAccessToolbarFooterProperty, value);
		}
		public static Style GetDisabledContentStyleInQuickAccessToolbar(DependencyObject target) {
			return (Style)target.GetValue(DisabledContentStyleInQuickAccessToolbarProperty);
		}
		public static void SetDisabledContentStyleInQuickAccessToolbar(DependencyObject target, Style value) {
			target.SetValue(DisabledContentStyleInQuickAccessToolbarProperty, value);
		}
		public static Style GetPressedContentStyleInQuickAccessToolbar(DependencyObject target) {
			return (Style)target.GetValue(PressedContentStyleInQuickAccessToolbarProperty);
		}
		public static void SetPressedContentStyleInQuickAccessToolbar(DependencyObject target, Style value) {
			target.SetValue(PressedContentStyleInQuickAccessToolbarProperty, value);
		}
		public static Style GetHotContentStyleInQuickAccessToolbar(DependencyObject target) {
			return (Style)target.GetValue(HotContentStyleInQuickAccessToolbarProperty);
		}
		public static void SetHotContentStyleInQuickAccessToolbar(DependencyObject target, Style value) {
			target.SetValue(HotContentStyleInQuickAccessToolbarProperty, value);
		}
		public static Style GetNormalContentStyleInQuickAccessToolbar(DependencyObject target) {
			return (Style)target.GetValue(NormalContentStyleInQuickAccessToolbarProperty);
		}
		public static void SetNormalContentStyleInQuickAccessToolbar(DependencyObject target, Style value) {
			target.SetValue(NormalContentStyleInQuickAccessToolbarProperty, value);
		}
		public static Style GetDisabledContentStyleInButtonGroup(DependencyObject target) {
			return (Style)target.GetValue(DisabledContentStyleInButtonGroupProperty);
		}
		public static void SetDisabledContentStyleInButtonGroup(DependencyObject target, Style value) {
			target.SetValue(DisabledContentStyleInButtonGroupProperty, value);
		}
		public static Style GetPressedContentStyleInButtonGroup(DependencyObject target) {
			return (Style)target.GetValue(PressedContentStyleInButtonGroupProperty);
		}
		public static void SetPressedContentStyleInButtonGroup(DependencyObject target, Style value) {
			target.SetValue(PressedContentStyleInButtonGroupProperty, value);
		}
		public static Style GetHotContentStyleInButtonGroup(DependencyObject target) {
			return (Style)target.GetValue(HotContentStyleInButtonGroupProperty);
		}
		public static void SetHotContentStyleInButtonGroup(DependencyObject target, Style value) {
			target.SetValue(HotContentStyleInButtonGroupProperty, value);
		}
		public static Style GetNormalContentStyleInButtonGroup(DependencyObject target) {
			return (Style)target.GetValue(NormalContentStyleInButtonGroupProperty);
		}
		public static void SetNormalContentStyleInButtonGroup(DependencyObject target, Style value) {
			target.SetValue(NormalContentStyleInButtonGroupProperty, value);
		}
		public static Style GetDisabledContentStyleInRibbonPageGroup(DependencyObject target) {
			return (Style)target.GetValue(DisabledContentStyleInRibbonPageGroupProperty);
		}
		public static void SetDisabledContentStyleInRibbonPageGroup(DependencyObject target, Style value) {
			target.SetValue(DisabledContentStyleInRibbonPageGroupProperty, value);
		}
		public static Style GetPressedContentStyleInRibbonPageGroup(DependencyObject target) {
			return (Style)target.GetValue(PressedContentStyleInRibbonPageGroupProperty);
		}
		public static void SetPressedContentStyleInRibbonPageGroup(DependencyObject target, Style value) {
			target.SetValue(PressedContentStyleInRibbonPageGroupProperty, value);
		}
		public static Style GetHotContentStyleInRibbonPageGroup(DependencyObject target) {
			return (Style)target.GetValue(HotContentStyleInRibbonPageGroupProperty);
		}
		public static void SetHotContentStyleInRibbonPageGroup(DependencyObject target, Style value) {
			target.SetValue(HotContentStyleInRibbonPageGroupProperty, value);
		}
		public static Style GetNormalContentStyleInRibbonPageGroup(DependencyObject target) {
			return (Style)target.GetValue(NormalContentStyleInRibbonPageGroupProperty);
		}
		public static void SetNormalContentStyleInRibbonPageGroup(DependencyObject target, Style value) {
			target.SetValue(NormalContentStyleInRibbonPageGroupProperty, value);
		}
		public static Style GetDisabledContentStyleInStatusBar(DependencyObject target) {
			return (Style)target.GetValue(DisabledContentStyleInStatusBarProperty);
		}
		public static void SetDisabledContentStyleInStatusBar(DependencyObject target, Style value) {
			target.SetValue(DisabledContentStyleInStatusBarProperty, value);
		}
		public static Style GetPressedContentStyleInStatusBar(DependencyObject target) {
			return (Style)target.GetValue(PressedContentStyleInStatusBarProperty);
		}
		public static void SetPressedContentStyleInStatusBar(DependencyObject target, Style value) {
			target.SetValue(PressedContentStyleInStatusBarProperty, value);
		}
		public static Style GetHotContentStyleInStatusBar(DependencyObject target) {
			return (Style)target.GetValue(HotContentStyleInStatusBarProperty);
		}
		public static void SetHotContentStyleInStatusBar(DependencyObject target, Style value) {
			target.SetValue(HotContentStyleInStatusBarProperty, value);
		}
		public static Style GetNormalContentStyleInStatusBar(DependencyObject target) {
			return (Style)target.GetValue(NormalContentStyleInStatusBarProperty);
		}
		public static void SetNormalContentStyleInStatusBar(DependencyObject target, Style value) {
			target.SetValue(NormalContentStyleInStatusBarProperty, value);
		}
		public static Style GetDisabledDescriptionStyleInApplicationMenu(DependencyObject target) {
			return (Style)target.GetValue(DisabledDescriptionStyleInApplicationMenuProperty);
		}
		public static void SetDisabledDescriptionStyleInApplicationMenu(DependencyObject target, Style value) {
			target.SetValue(DisabledDescriptionStyleInApplicationMenuProperty, value);
		}
		public static Style GetPressedDescriptionStyleInApplicationMenu(DependencyObject target) {
			return (Style)target.GetValue(PressedDescriptionStyleInApplicationMenuProperty);
		}
		public static void SetPressedDescriptionStyleInApplicationMenu(DependencyObject target, Style value) {
			target.SetValue(PressedDescriptionStyleInApplicationMenuProperty, value);
		}
		public static Style GetHotDescriptionStyleInApplicationMenu(DependencyObject target) {
			return (Style)target.GetValue(HotDescriptionStyleInApplicationMenuProperty);
		}
		public static void SetHotDescriptionStyleInApplicationMenu(DependencyObject target, Style value) {
			target.SetValue(HotDescriptionStyleInApplicationMenuProperty, value);
		}
		public static Style GetNormalDescriptionStyleInApplicationMenu(DependencyObject target) {
			return (Style)target.GetValue(NormalDescriptionStyleInApplicationMenuProperty);
		}
		public static void SetNormalDescriptionStyleInApplicationMenu(DependencyObject target, Style value) {
			target.SetValue(NormalDescriptionStyleInApplicationMenuProperty, value);
		}
		public static Style GetDisabledContentStyleInApplicationMenu(DependencyObject target) {
			return (Style)target.GetValue(DisabledContentStyleInApplicationMenuProperty);
		}
		public static void SetDisabledContentStyleInApplicationMenu(DependencyObject target, Style value) {
			target.SetValue(DisabledContentStyleInApplicationMenuProperty, value);
		}
		public static Style GetPressedContentStyleInApplicationMenu(DependencyObject target) {
			return (Style)target.GetValue(PressedContentStyleInApplicationMenuProperty);
		}
		public static void SetPressedContentStyleInApplicationMenu(DependencyObject target, Style value) {
			target.SetValue(PressedContentStyleInApplicationMenuProperty, value);
		}
		public static Style GetHotContentStyleInApplicationMenu(DependencyObject target) {
			return (Style)target.GetValue(HotContentStyleInApplicationMenuProperty);
		}
		public static void SetHotContentStyleInApplicationMenu(DependencyObject target, Style value) {
			target.SetValue(HotContentStyleInApplicationMenuProperty, value);
		}
		public static Style GetNormalContentStyleInApplicationMenu(DependencyObject target) {
			return (Style)target.GetValue(NormalContentStyleInApplicationMenuProperty);
		}
		public static void SetNormalContentStyleInApplicationMenu(DependencyObject target, Style value) {
			target.SetValue(NormalContentStyleInApplicationMenuProperty, value);
		}
		public static Style GetDisabledContentStyleInMenu(DependencyObject target) {
			return (Style)target.GetValue(DisabledContentStyleInMenuProperty);
		}
		public static void SetDisabledContentStyleInMenu(DependencyObject target, Style value) {
			target.SetValue(DisabledContentStyleInMenuProperty, value);
		}
		public static Style GetPressedContentStyleInMenu(DependencyObject target) {
			return (Style)target.GetValue(PressedContentStyleInMenuProperty);
		}
		public static void SetPressedContentStyleInMenu(DependencyObject target, Style value) {
			target.SetValue(PressedContentStyleInMenuProperty, value);
		}
		public static Style GetHotContentStyleInMenu(DependencyObject target) {
			return (Style)target.GetValue(HotContentStyleInMenuProperty);
		}
		public static void SetHotContentStyleInMenu(DependencyObject target, Style value) {
			target.SetValue(HotContentStyleInMenuProperty, value);
		}
		public static Style GetNormalContentStyleInMenu(DependencyObject target) {
			return (Style)target.GetValue(NormalContentStyleInMenuProperty);
		}
		public static void SetNormalContentStyleInMenu(DependencyObject target, Style value) {
			target.SetValue(NormalContentStyleInMenuProperty, value);
		}
		public static Style GetDisabledContentStyleInMainMenu(DependencyObject target) {
			return (Style)target.GetValue(DisabledContentStyleInMainMenuProperty);
		}
		public static void SetDisabledContentStyleInMainMenu(DependencyObject target, Style value) {
			target.SetValue(DisabledContentStyleInMainMenuProperty, value);
		}
		public static Style GetPressedContentStyleInMainMenu(DependencyObject target) {
			return (Style)target.GetValue(PressedContentStyleInMainMenuProperty);
		}
		public static void SetPressedContentStyleInMainMenu(DependencyObject target, Style value) {
			target.SetValue(PressedContentStyleInMainMenuProperty, value);
		}
		public static Style GetHotContentStyleInMainMenu(DependencyObject target) {
			return (Style)target.GetValue(HotContentStyleInMainMenuProperty);
		}
		public static void SetHotContentStyleInMainMenu(DependencyObject target, Style value) {
			target.SetValue(HotContentStyleInMainMenuProperty, value);
		}
		public static Style GetNormalContentStyleInMainMenu(DependencyObject target) {
			return (Style)target.GetValue(NormalContentStyleInMainMenuProperty);
		}
		public static void SetNormalContentStyleInMainMenu(DependencyObject target, Style value) {
			target.SetValue(NormalContentStyleInMainMenuProperty, value);
		}
		public static Style GetDisabledContentStyleInBar(DependencyObject target) {
			return (Style)target.GetValue(DisabledContentStyleInBarProperty);
		}
		public static void SetDisabledContentStyleInBar(DependencyObject target, Style value) {
			target.SetValue(DisabledContentStyleInBarProperty, value);
		}
		public static Style GetPressedContentStyleInBar(DependencyObject target) {
			return (Style)target.GetValue(PressedContentStyleInBarProperty);
		}
		public static void SetPressedContentStyleInBar(DependencyObject target, Style value) {
			target.SetValue(PressedContentStyleInBarProperty, value);
		}
		public static Style GetHotContentStyleInBar(DependencyObject target) {
			return (Style)target.GetValue(HotContentStyleInBarProperty);
		}
		public static void SetHotContentStyleInBar(DependencyObject target, Style value) {
			target.SetValue(HotContentStyleInBarProperty, value);
		}
		public static Style GetNormalContentStyleInBar(DependencyObject target) {
			return (Style)target.GetValue(NormalContentStyleInBarProperty);
		}
		public static void SetNormalContentStyleInBar(DependencyObject target, Style value) {
			target.SetValue(NormalContentStyleInBarProperty, value);
		}
		public static Style GetBorderStyleInRibbonStatusBarRight(DependencyObject target) {
			return (Style)target.GetValue(BorderStyleInRibbonStatusBarRightProperty);
		}
		public static void SetBorderStyleInRibbonStatusBarRight(DependencyObject target, Style value) {
			target.SetValue(BorderStyleInRibbonStatusBarRightProperty, value);
		}
		public static Style GetBorderStyleInRibbonStatusBarLeft(DependencyObject target) {
			return (Style)target.GetValue(BorderStyleInRibbonStatusBarLeftProperty);
		}
		public static void SetBorderStyleInRibbonStatusBarLeft(DependencyObject target, Style value) {
			target.SetValue(BorderStyleInRibbonStatusBarLeftProperty, value);
		}
		public static Style GetBorderStyleInRibbonPageHeader(DependencyObject target) {
			return (Style)target.GetValue(BorderStyleInRibbonPageHeaderProperty);
		}
		public static void SetBorderStyleInRibbonPageHeader(DependencyObject target, Style value) {
			target.SetValue(BorderStyleInRibbonPageHeaderProperty, value);
		}
		public static Style GetBorderStyleInQuickAccessToolbarFooter(DependencyObject target) {
			return (Style)target.GetValue(BorderStyleInQuickAccessToolbarFooterProperty);
		}
		public static void SetBorderStyleInQuickAccessToolbarFooter(DependencyObject target, Style value) {
			target.SetValue(BorderStyleInQuickAccessToolbarFooterProperty, value);
		}
		public static Style GetBorderStyleInQuickAccessToolbar(DependencyObject target) {
			return (Style)target.GetValue(BorderStyleInQuickAccessToolbarProperty);
		}
		public static void SetBorderStyleInQuickAccessToolbar(DependencyObject target, Style value) {
			target.SetValue(BorderStyleInQuickAccessToolbarProperty, value);
		}
		public static Style GetBorderStyleInButtonGroup(DependencyObject target) {
			return (Style)target.GetValue(BorderStyleInButtonGroupProperty);
		}
		public static void SetBorderStyleInButtonGroup(DependencyObject target, Style value) {
			target.SetValue(BorderStyleInButtonGroupProperty, value);
		}
		public static Style GetBorderStyleInRibbonPageGroup(DependencyObject target) {
			return (Style)target.GetValue(BorderStyleInRibbonPageGroupProperty);
		}
		public static void SetBorderStyleInRibbonPageGroup(DependencyObject target, Style value) {
			target.SetValue(BorderStyleInRibbonPageGroupProperty, value);
		}
		public static Style GetBorderStyleInStatusBar(DependencyObject target) {
			return (Style)target.GetValue(BorderStyleInStatusBarProperty);
		}
		public static void SetBorderStyleInStatusBar(DependencyObject target, Style value) {
			target.SetValue(BorderStyleInStatusBarProperty, value);
		}
		public static Style GetBorderStyleInApplicationMenu(DependencyObject target) {
			return (Style)target.GetValue(BorderStyleInApplicationMenuProperty);
		}
		public static void SetBorderStyleInApplicationMenu(DependencyObject target, Style value) {
			target.SetValue(BorderStyleInApplicationMenuProperty, value);
		}
		public static Style GetBorderStyleInMenu(DependencyObject target) {
			return (Style)target.GetValue(BorderStyleInMenuProperty);
		}
		public static void SetBorderStyleInMenu(DependencyObject target, Style value) {
			target.SetValue(BorderStyleInMenuProperty, value);
		}
		public static Style GetBorderStyleInMenuHorizontal(DependencyObject target) {
			return (Style)target.GetValue(BorderStyleInMenuProperty);
		}
		public static void SetBorderStyleInMenuHorizontal(DependencyObject target, Style value) {
			target.SetValue(BorderStyleInMenuProperty, value);
		}
		public static Style GetBorderStyleInMainMenu(DependencyObject target) {
			return (Style)target.GetValue(BorderStyleInMainMenuProperty);
		}
		public static void SetBorderStyleInMainMenu(DependencyObject target, Style value) {
			target.SetValue(BorderStyleInMainMenuProperty, value);
		}
		public static Style GetBorderStyleInBar(DependencyObject target) {
			return (Style)target.GetValue(BorderStyleInBarProperty);
		}
		public static void SetBorderStyleInBar(DependencyObject target, Style value) {
			target.SetValue(BorderStyleInBarProperty, value);
		}
		public static Style GetLayoutPanelStyleInRibbonStatusBarRight(DependencyObject target) {
			return (Style)target.GetValue(LayoutPanelStyleInRibbonStatusBarLeftProperty);
		}
		public static void SetLayoutPanelStyleInRibbonStatusBarRight(DependencyObject target, Style value) {
			target.SetValue(LayoutPanelStyleInRibbonStatusBarRightProperty, value);
		}
		public static Style GetLayoutPanelStyleInRibbonStatusBarLeft(DependencyObject target) {
			return (Style)target.GetValue(LayoutPanelStyleInRibbonStatusBarLeftProperty);
		}
		public static void SetLayoutPanelStyleInRibbonStatusBarLeft(DependencyObject target, Style value) {
			target.SetValue(LayoutPanelStyleInRibbonStatusBarLeftProperty, value);
		}
		public static Style GetLayoutPanelStyleInRibbonPageHeader(DependencyObject target) {
			return (Style)target.GetValue(LayoutPanelStyleInRibbonPageHeaderProperty);
		}
		public static void SetLayoutPanelStyleInRibbonPageHeader(DependencyObject target, Style value) {
			target.SetValue(LayoutPanelStyleInRibbonPageHeaderProperty, value);
		}
		public static Style GetLayoutPanelStyleInQuickAccessToolbarFooter(DependencyObject target) {
			return (Style)target.GetValue(LayoutPanelStyleInQuickAccessToolbarFooterProperty);
		}
		public static void SetLayoutPanelStyleInQuickAccessToolbarFooter(DependencyObject target, Style value) {
			target.SetValue(LayoutPanelStyleInQuickAccessToolbarFooterProperty, value);
		}
		public static Style GetLayoutPanelStyleInQuickAccessToolbar(DependencyObject target) {
			return (Style)target.GetValue(LayoutPanelStyleInQuickAccessToolbarProperty);
		}
		public static void SetLayoutPanelStyleInQuickAccessToolbar(DependencyObject target, Style value) {
			target.SetValue(LayoutPanelStyleInQuickAccessToolbarProperty, value);
		}
		public static Style GetLayoutPanelStyleInButtonGroup(DependencyObject target) {
			return (Style)target.GetValue(LayoutPanelStyleInButtonGroupProperty);
		}
		public static void SetLayoutPanelStyleInButtonGroup(DependencyObject target, Style value) {
			target.SetValue(LayoutPanelStyleInButtonGroupProperty, value);
		}
		public static Style GetLayoutPanelStyleInRibbonPageGroup(DependencyObject target) {
			return (Style)target.GetValue(LayoutPanelStyleInRibbonPageGroupProperty);
		}
		public static void SetLayoutPanelStyleInRibbonPageGroup(DependencyObject target, Style value) {
			target.SetValue(LayoutPanelStyleInRibbonPageGroupProperty, value);
		}
		public static Style GetLayoutPanelStyleInStatusBar(DependencyObject target) {
			return (Style)target.GetValue(LayoutPanelStyleInStatusBarProperty);
		}
		public static void SetLayoutPanelStyleInStatusBar(DependencyObject target, Style value) {
			target.SetValue(LayoutPanelStyleInStatusBarProperty, value);
		}
		public static Style GetLayoutPanelStyleInMainMenu(DependencyObject target) {
			return (Style)target.GetValue(LayoutPanelStyleInMainMenuProperty);
		}
		public static void SetLayoutPanelStyleInMainMenu(DependencyObject target, Style value) {
			target.SetValue(LayoutPanelStyleInMainMenuProperty, value);
		}
		public static Style GetLayoutPanelStyleInBar(DependencyObject target) {
			return (Style)target.GetValue(LayoutPanelStyleInBarProperty);
		}
		public static void SetLayoutPanelStyleInBar(DependencyObject target, Style value) {
			target.SetValue(LayoutPanelStyleInBarProperty, value);
		}
	}
	public class IsLargeGlyphToDoubleConverterExtension : System.Windows.Markup.MarkupExtension, IValueConverter {
		public IsLargeGlyphToDoubleConverterExtension() {}
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			if(!(value is Boolean)) return Double.NaN;
			bool bValue = (bool)value;
			return bValue ? 32d : 16d;
		}
		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			throw new NotImplementedException();
		}
		public override object ProvideValue(IServiceProvider serviceProvider) {
			return this;
		}
	}	
}
