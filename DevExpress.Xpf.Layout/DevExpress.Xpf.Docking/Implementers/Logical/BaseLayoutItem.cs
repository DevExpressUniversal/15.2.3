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
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using DevExpress.Utils.Serializing;
using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Docking.Internal;
using DevExpress.Xpf.Layout.Core;
using DependencyPropertyHelper = System.Windows.DependencyPropertyHelper;
#if SILVERLIGHT
using DependencyPropertyChangedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLDependencyPropertyChangedEventArgs;
using DevExpress.Xpf.ComponentModel;
using DevExpress.Xpf.Core.WPFCompatibility;
#endif
namespace DevExpress.Xpf.Docking {
	[TemplateVisualState(Name = "Active", GroupName = "ActiveStates")]
	[TemplateVisualState(Name = "Inactive", GroupName = "ActiveStates")]
	public abstract class BaseLayoutItem : psvFrameworkElement, ISerializableItem, ISupportHierarchy<BaseLayoutItem>, IUIElement {
		#region static
		public static readonly DependencyProperty ItemWidthProperty;
		public static readonly DependencyProperty ItemHeightProperty;
		public static readonly DependencyProperty FloatSizeProperty;
		public static readonly DependencyProperty LayoutSizeProperty;
		static readonly DependencyPropertyKey LayoutSizePropertyKey;
		static readonly DependencyPropertyKey ActualMinSizePropertyKey;
		static readonly DependencyPropertyKey ActualMaxSizePropertyKey;
		public static readonly DependencyProperty ActualMinSizeProperty;
		public static readonly DependencyProperty ActualMaxSizeProperty;
		public static readonly DependencyProperty CaptionProperty;
		public static readonly DependencyProperty DescriptionProperty;
		public static readonly DependencyProperty FooterDescriptionProperty;
		public static readonly DependencyProperty CaptionFormatProperty;
		public static readonly DependencyProperty CustomizationCaptionProperty;
		public static readonly DependencyProperty CaptionLocationProperty;
		public static readonly DependencyProperty CaptionWidthProperty;
		public static readonly DependencyProperty CaptionAlignModeProperty;
		public static readonly DependencyProperty CaptionImageProperty;
		public static readonly DependencyProperty CaptionImageLocationProperty;
		public static readonly DependencyProperty ImageToTextDistanceProperty;
		public static readonly DependencyProperty CaptionHorizontalAlignmentProperty;
		public static readonly DependencyProperty CaptionVerticalAlignmentProperty;
		public static readonly DependencyProperty ActualCaptionProperty;
		internal static readonly DependencyPropertyKey ActualCaptionPropertyKey;
		public static readonly DependencyProperty DesiredCaptionWidthProperty;
		internal static readonly DependencyPropertyKey DesiredCaptionWidthPropertyKey;
		public static readonly DependencyProperty HasDesiredCaptionWidthProperty;
		internal static readonly DependencyPropertyKey HasDesiredCaptionWidthPropertyKey;
		public static readonly DependencyProperty ActualCaptionWidthProperty;
		static readonly DependencyPropertyKey ActualCaptionWidthPropertyKey;
		public static readonly DependencyProperty HeaderBarContainerControlNameProperty;
		public static readonly DependencyProperty HeaderBarContainerControlAllowDropProperty;
		static readonly DependencyPropertyKey IsFloatingRootItemPropertyKey;
		public static readonly DependencyProperty IsFloatingRootItemProperty;
		static readonly DependencyPropertyKey IsSelectedPropertyKey;
		public static readonly DependencyProperty IsSelectedProperty;
		public static readonly new DependencyProperty MarginProperty;
		public static readonly new DependencyProperty PaddingProperty;
		static readonly DependencyPropertyKey ActualMarginPropertyKey;
		public static readonly DependencyProperty ActualMarginProperty;
		static readonly DependencyPropertyKey ActualPaddingPropertyKey;
		public static readonly DependencyProperty ActualPaddingProperty;
		public readonly static DependencyProperty IsActiveProperty;
		public readonly static DependencyProperty IsClosedProperty;
		public static readonly DependencyProperty ClosedProperty;
		public readonly static DependencyProperty IsHiddenProperty;
		public readonly static DependencyProperty HasImageProperty;
		public readonly static DependencyProperty HasCaptionProperty;
		public readonly static DependencyProperty HasCaptionTemplateProperty;
		static readonly DependencyPropertyKey IsClosedPropertyKey;
		static readonly DependencyPropertyKey IsHiddenPropertyKey;
		static readonly DependencyPropertyKey HasImagePropertyKey;
		static readonly DependencyPropertyKey HasCaptionPropertyKey;
		static readonly DependencyPropertyKey HasCaptionTemplatePropertyKey;
		public static readonly DependencyProperty AllowActivateProperty;
		public static readonly DependencyProperty AllowSelectionProperty;
		public static readonly DependencyProperty AllowDragProperty;
		public static readonly DependencyProperty AllowFloatProperty;
		public static readonly DependencyProperty AllowDockProperty;
		public static readonly DependencyProperty AllowHideProperty;
		public static readonly DependencyProperty AllowCloseProperty;
		public static readonly DependencyProperty AllowRestoreProperty;
		public static readonly DependencyProperty AllowMoveProperty;
		public static readonly DependencyProperty AllowRenameProperty;
		public static readonly DependencyProperty AllowContextMenuProperty;
		public static readonly DependencyProperty AllowMaximizeProperty;
		public static readonly DependencyProperty AllowMinimizeProperty;
		public static readonly DependencyProperty AllowSizingProperty;
		public static readonly DependencyProperty AllowDockToCurrentItemProperty;
		public static readonly DependencyProperty FloatOnDoubleClickProperty;
		public static readonly DependencyProperty IsControlItemsHostProperty;
		internal static readonly DependencyPropertyKey IsControlItemsHostPropertyKey;
		public static readonly new DependencyProperty VisibilityProperty;
#if SILVERLIGHT
		public static readonly DependencyProperty IsVisibleProperty;
#else
		public static readonly new DependencyProperty IsVisibleProperty;
#endif
		internal static readonly DependencyPropertyKey IsVisiblePropertyKey;
		public static readonly DependencyProperty ControlBoxContentProperty;
		public static readonly DependencyProperty ControlBoxContentTemplateProperty;
		public readonly static DependencyProperty IsTabPageProperty;
		static readonly DependencyPropertyKey IsTabPagePropertyKey;
		public readonly static DependencyProperty IsSelectedItemProperty;
		static readonly DependencyPropertyKey IsSelectedItemPropertyKey;
		public static readonly DependencyProperty ShowCaptionProperty;
		public static readonly DependencyProperty ShowCaptionImageProperty;
		public static readonly DependencyProperty ShowControlBoxProperty;
		public static readonly DependencyProperty ShowCloseButtonProperty;
		[IgnoreDependencyPropertiesConsistencyChecker]
		public static readonly DependencyProperty ShowPinButtonProperty;
		[IgnoreDependencyPropertiesConsistencyChecker]
		public static readonly DependencyProperty ShowMinimizeButtonProperty;
		[IgnoreDependencyPropertiesConsistencyChecker]
		public static readonly DependencyProperty ShowMaximizeButtonProperty;
		[IgnoreDependencyPropertiesConsistencyChecker]
		public static readonly DependencyProperty ShowRestoreButtonProperty;
		[IgnoreDependencyPropertiesConsistencyChecker]
		public static readonly DependencyProperty ShowDropDownButtonProperty;
		[IgnoreDependencyPropertiesConsistencyChecker]
		public static readonly DependencyProperty ShowScrollPrevButtonProperty;
		[IgnoreDependencyPropertiesConsistencyChecker]
		public static readonly DependencyProperty ShowScrollNextButtonProperty;
		static readonly DependencyPropertyKey IsCaptionVisiblePropertyKey;
		static readonly DependencyPropertyKey IsCaptionImageVisiblePropertyKey;
		static readonly DependencyPropertyKey IsControlBoxVisiblePropertyKey;
		static readonly DependencyPropertyKey IsCloseButtonVisiblePropertyKey;
		static readonly DependencyPropertyKey IsPinButtonVisiblePropertyKey;
		static readonly DependencyPropertyKey IsMinimizeButtonVisiblePropertyKey;
		static readonly DependencyPropertyKey IsMaximizeButtonVisiblePropertyKey;
		static readonly DependencyPropertyKey IsRestoreButtonVisiblePropertyKey;
		static readonly DependencyPropertyKey IsDropDownButtonVisiblePropertyKey;
		static readonly DependencyPropertyKey IsScrollPrevButtonVisiblePropertyKey;
		static readonly DependencyPropertyKey IsScrollNextButtonVisiblePropertyKey;
		public static readonly DependencyProperty IsCaptionVisibleProperty;
		public static readonly DependencyProperty IsCaptionImageVisibleProperty;
		public static readonly DependencyProperty IsControlBoxVisibleProperty;
		public static readonly DependencyProperty IsCloseButtonVisibleProperty;
		[IgnoreDependencyPropertiesConsistencyChecker]
		public static readonly DependencyProperty IsPinButtonVisibleProperty;
		[IgnoreDependencyPropertiesConsistencyChecker]
		public static readonly DependencyProperty IsMinimizeButtonVisibleProperty;
		[IgnoreDependencyPropertiesConsistencyChecker]
		public static readonly DependencyProperty IsMaximizeButtonVisibleProperty;
		[IgnoreDependencyPropertiesConsistencyChecker]
		public static readonly DependencyProperty IsRestoreButtonVisibleProperty;
		[IgnoreDependencyPropertiesConsistencyChecker]
		public static readonly DependencyProperty IsDropDownButtonVisibleProperty;
		[IgnoreDependencyPropertiesConsistencyChecker]
		public static readonly DependencyProperty IsScrollPrevButtonVisibleProperty;
		[IgnoreDependencyPropertiesConsistencyChecker]
		public static readonly DependencyProperty IsScrollNextButtonVisibleProperty;
		public static readonly DependencyProperty TabCaptionProperty;
		public static readonly DependencyProperty TabCaptionFormatProperty;
		internal static readonly DependencyPropertyKey ActualTabCaptionPropertyKey;
		public static readonly DependencyProperty ActualTabCaptionProperty;
		public static readonly DependencyProperty TabCaptionWidthProperty;
		public static readonly DependencyProperty HasTabCaptionProperty;
		static readonly DependencyPropertyKey HasTabCaptionPropertyKey;
		public static readonly DependencyProperty TextWrappingProperty;
		public static readonly DependencyProperty TextTrimmingProperty;
		[IgnoreDependencyPropertiesConsistencyChecker]
		public static readonly DependencyProperty AppearanceProperty;
		static readonly DependencyPropertyKey ActualAppearancePropertyKey;
		[IgnoreDependencyPropertiesConsistencyChecker]
		public static readonly DependencyProperty ActualAppearanceProperty;
		static readonly DependencyPropertyKey ActualAppearanceObjectPropertyKey;
		[IgnoreDependencyPropertiesConsistencyChecker]
		public static readonly DependencyProperty ActualAppearanceObjectProperty;
		public static readonly DependencyProperty ClosingBehaviorProperty;
		public static readonly DependencyProperty CaptionTemplateProperty;
#if !SILVERLIGHT
		public static readonly DependencyProperty CaptionTemplateSelectorProperty;
#endif
		public static readonly DependencyProperty CloseCommandProperty;
		public static readonly DependencyProperty BindableNameProperty;
#if SILVERLIGHT
		public static readonly DependencyProperty ToolTipProperty;
		public static readonly new DependencyProperty MinHeightProperty;
		public static readonly new DependencyProperty MinWidthProperty;
		public static readonly new DependencyProperty MaxHeightProperty;
		public static readonly new DependencyProperty MaxWidthProperty;
#else
		public static new readonly DependencyProperty ToolTipProperty;
		[IgnoreDependencyPropertiesConsistencyChecker]
		static readonly DependencyProperty IsLayoutChangeInProgressProperty;
#endif
		[IgnoreDependencyPropertiesConsistencyChecker]
		static readonly DependencyProperty LayoutItemDataProperty;
		static BaseLayoutItem() {
			var dProp = new DependencyPropertyRegistrator<BaseLayoutItem>();
			dProp.OverrideDefaultStyleKey(DefaultStyleKeyProperty);
			dProp.OverrideUIMetadata(DevExpress.Xpf.Core.Serialization.DXSerializer.SerializationProviderProperty,
				(DevExpress.Xpf.Core.Serialization.SerializationProvider)new BaseLayoutItemSerializationProvider());
			dProp.OverrideFrameworkMetadata(Bars.BarNameScope.IsScopeOwnerProperty, true);
			DataContextProperty.OverrideMetadata(typeof(BaseLayoutItem), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.Inherits,
				(dObj, e) => ((BaseLayoutItem)dObj).OnDataContextChanged(e.NewValue),
				(dObj, value) => ((BaseLayoutItem)dObj).CoerceDataContext(value)));
			dProp.Register("Appearance", ref AppearanceProperty, (Appearance)null,
				(dObj, e) => ((BaseLayoutItem)dObj).OnAppearanceChanged((Appearance)e.NewValue),
				(dObj, value) => ((BaseLayoutItem)dObj).CoerceAppearance((Appearance)value));
			dProp.RegisterReadonly("ActualAppearanceObject", ref ActualAppearanceObjectPropertyKey, ref ActualAppearanceObjectProperty, (AppearanceObject)null,
				(dObj, e) => ((BaseLayoutItem)dObj).OnActualAppearanceObjectChanged((AppearanceObject)e.NewValue),
				(dObj, value) => ((BaseLayoutItem)dObj).CoerceActualAppearanceObject((AppearanceObject)value));
			dProp.RegisterReadonly("ActualAppearance", ref ActualAppearancePropertyKey, ref ActualAppearanceProperty, (Appearance)null,
				(dObj, e) => ((BaseLayoutItem)dObj).OnActualAppearanceChanged((Appearance)e.NewValue),
				(dObj, value) => ((BaseLayoutItem)dObj).CoerceActualAppearance((Appearance)value));
			dProp.Register("ItemWidth", ref ItemWidthProperty, new GridLength(1, GridUnitType.Star),
				(dObj, e) => ((BaseLayoutItem)dObj).OnWidthChanged((GridLength)e.NewValue, (GridLength)e.OldValue),
				(dObj, value) => ((BaseLayoutItem)dObj).CoerceWidth((GridLength)value));
			dProp.Register("ItemHeight", ref ItemHeightProperty, new GridLength(1, GridUnitType.Star),
				(dObj, e) => ((BaseLayoutItem)dObj).OnHeightChanged((GridLength)e.NewValue, (GridLength)e.OldValue),
				(dObj, value) => ((BaseLayoutItem)dObj).CoerceHeight((GridLength)value));
			dProp.RegisterReadonly("ActualMinSize", ref ActualMinSizePropertyKey, ref ActualMinSizeProperty, SizeHelper.Zero,
				(dObj, e) => ((BaseLayoutItem)dObj).OnActualMinSizeChanged(),
				(dObj, value) => ((BaseLayoutItem)dObj).CoerceActualMinSize());
			dProp.RegisterReadonly("ActualMaxSize", ref ActualMaxSizePropertyKey, ref ActualMaxSizeProperty, new Size(double.NaN, double.NaN),
				(dObj, e) => ((BaseLayoutItem)dObj).OnActualMaxSizeChanged(),
				(dObj, value) => ((BaseLayoutItem)dObj).CoerceActualMaxSize());
			dProp.Register("FloatSize", ref FloatSizeProperty, new Size(200, 200), 
				(dObj, e) => ((BaseLayoutItem)dObj).OnFloatSizeChanged((Size)e.NewValue),
				(dObj, value) => ((BaseLayoutItem)dObj).CoerceFloatSize((Size)value));
			dProp.RegisterReadonly("LayoutSize", ref LayoutSizePropertyKey, ref LayoutSizeProperty, new Size());
#if !SILVERLIGHT
			NameProperty.OverrideMetadata(typeof(BaseLayoutItem), new FrameworkPropertyMetadata(string.Empty,
				(dObj, e) => ((BaseLayoutItem)dObj).OnNameChanged()));
#else
			dProp.Register("Name", ref NameProperty, (string)"", (dObj, e) => ((BaseLayoutItem)dObj).OnNameChanged());
#endif
			dProp.Register("Caption", ref CaptionProperty, (object)null,
				(dObj, e) => ((BaseLayoutItem)dObj).OnCaptionChanged(e.OldValue, e.NewValue));
			dProp.Register("Description", ref DescriptionProperty, (string)null);
			dProp.Register("FooterDescription", ref FooterDescriptionProperty, (string)null);
			dProp.Register("CaptionFormat", ref CaptionFormatProperty, (string)null,
				(dObj, e) => ((BaseLayoutItem)dObj).OnCaptionFormatChanged(),
				(dObj, value) => ((BaseLayoutItem)dObj).CoerceCaptionFormat((string)value));
			dProp.Register("CustomizationCaption", ref CustomizationCaptionProperty, (string)null, null,
				(dObj, value) => ((BaseLayoutItem)dObj).CoerceCustomizationCaption((string)value));
			dProp.Register("CaptionLocation", ref CaptionLocationProperty, CaptionLocation.Default,
				(dObj, e) => ((BaseLayoutItem)dObj).OnCaptionLocationChanged((CaptionLocation)e.NewValue));
			dProp.Register("CaptionAlignMode", ref CaptionAlignModeProperty, CaptionAlignMode.Default,
				(dObj, e) => ((BaseLayoutItem)dObj).OnCaptionAlignModeChanged((CaptionAlignMode)e.OldValue, (CaptionAlignMode)e.NewValue),
				(dObj, value) => ((BaseLayoutItem)dObj).CoerceCaptionAlignMode((CaptionAlignMode)value));
			dProp.Register("CaptionWidth", ref CaptionWidthProperty, double.NaN,
				(dObj, e) => ((BaseLayoutItem)dObj).OnCaptionWidthChanged((double)e.NewValue),
				(dObj, value) => ((BaseLayoutItem)dObj).CoerceCaptionWidth((double)value));
			dProp.Register("CaptionImage", ref CaptionImageProperty, (ImageSource)null,
				(dObj, e) => ((BaseLayoutItem)dObj).OnCaptionImageChanged((ImageSource)e.NewValue),
				(dObj, value) => ((BaseLayoutItem)dObj).CoerceCaptionImage((ImageSource)value));
			dProp.Register("CaptionImageLocation", ref CaptionImageLocationProperty, ImageLocation.Default);
			dProp.Register("ImageToTextDistance", ref ImageToTextDistanceProperty, 3.0);
			dProp.Register("CaptionHorizontalAlignment", ref CaptionHorizontalAlignmentProperty, HorizontalAlignment.Left);
			dProp.Register("CaptionVerticalAlignment", ref CaptionVerticalAlignmentProperty, VerticalAlignment.Center);
			dProp.RegisterReadonly("ActualCaption", ref ActualCaptionPropertyKey, ref ActualCaptionProperty, (string)null,
				(dObj, ea) => ((BaseLayoutItem)dObj).OnActualCaptionChanged((string)ea.NewValue),
				(dObj, value) => ((BaseLayoutItem)dObj).CoerceActualCaption((string)value));
			dProp.RegisterReadonly("DesiredCaptionWidth", ref DesiredCaptionWidthPropertyKey, ref DesiredCaptionWidthProperty, double.NaN,
				(dObj, e) => ((BaseLayoutItem)dObj).OnDesiredCaptionWidthChanged((double)e.NewValue));
			dProp.RegisterReadonly("HasDesiredCaptionWidth", ref HasDesiredCaptionWidthPropertyKey, ref HasDesiredCaptionWidthProperty, false);
			dProp.RegisterReadonly("ActualCaptionWidth", ref ActualCaptionWidthPropertyKey, ref ActualCaptionWidthProperty, double.NaN, null,
				(dObj, value) => ((BaseLayoutItem)dObj).CoerceActualCaptionWidth((double)value));
			dProp.Register("HeaderBarContainerControlName", ref HeaderBarContainerControlNameProperty, (string)null);
			dProp.Register("HeaderBarContainerControlAllowDrop", ref HeaderBarContainerControlAllowDropProperty, (bool?)null);
			dProp.Register("Margin", ref MarginProperty, new Thickness(double.NaN),
				(dObj, e) => ((BaseLayoutItem)dObj).OnMarginChanged((Thickness)e.NewValue));
			dProp.Register("Padding", ref PaddingProperty, new Thickness(double.NaN),
				(dObj, e) => ((BaseLayoutItem)dObj).OnPaddingChanged((Thickness)e.NewValue));
			dProp.RegisterReadonly("ActualMargin", ref ActualMarginPropertyKey, ref ActualMarginProperty, new Thickness(),
				(dObj, e) => ((BaseLayoutItem)dObj).OnActualMarginChanged((Thickness)e.NewValue),
				(dObj, value) => ((BaseLayoutItem)dObj).CoerceActualMargin((Thickness)value));
			dProp.RegisterReadonly("ActualPadding", ref ActualPaddingPropertyKey, ref ActualPaddingProperty, new Thickness(), null,
				(dObj, value) => ((BaseLayoutItem)dObj).CoerceActualPadding((Thickness)value));
			dProp.Register("IsActive", ref IsActiveProperty, false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
				(dObj, e) => ((BaseLayoutItem)dObj).OnIsActiveChanged((bool)e.NewValue));
			dProp.Register("Closed", ref ClosedProperty, (bool)false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
				(dObj, e) => ((BaseLayoutItem)dObj).OnClosedChanged((bool)e.OldValue, (bool)e.NewValue));
			dProp.RegisterReadonly("IsClosed", ref IsClosedPropertyKey, ref IsClosedProperty, false);
			dProp.RegisterReadonly("IsHidden", ref IsHiddenPropertyKey, ref IsHiddenProperty, false);
			dProp.RegisterReadonly("HasImage", ref HasImagePropertyKey, ref HasImageProperty, false);
			dProp.RegisterReadonly("HasCaption", ref HasCaptionPropertyKey, ref HasCaptionProperty, false,
				(dObj, ea) => ((BaseLayoutItem)dObj).OnHasCaptionChanged((bool)ea.NewValue));
			dProp.RegisterReadonly("HasCaptionTemplate", ref HasCaptionTemplatePropertyKey, ref HasCaptionTemplateProperty, false,
				(dObj, ea) => ((BaseLayoutItem)dObj).OnHasCaptionTemplateChanged((bool)ea.NewValue));			
			dProp.Register("AllowActivate", ref AllowActivateProperty, true);
			dProp.Register("AllowSelection", ref AllowSelectionProperty, true);
			dProp.Register("AllowDrag", ref AllowDragProperty, true);
			dProp.Register("AllowFloat", ref AllowFloatProperty, true);
			dProp.Register("AllowDock", ref AllowDockProperty, true, (dObj, e) => ((BaseLayoutItem)dObj).OnAllowDockChanged((bool)e.NewValue));
			dProp.Register("AllowHide", ref AllowHideProperty, true, (dObj, e) => ((BaseLayoutItem)dObj).OnAllowHideChanged((bool)e.NewValue));
			dProp.Register("AllowClose", ref AllowCloseProperty, true, (dObj, e) => ((BaseLayoutItem)dObj).OnAllowCloseChanged((bool)e.NewValue));
			dProp.Register("AllowRestore", ref AllowRestoreProperty, true);
			dProp.Register("AllowMove", ref AllowMoveProperty, true);
			dProp.Register("AllowRename", ref AllowRenameProperty, true);
			dProp.Register("AllowContextMenu", ref AllowContextMenuProperty, true);
			dProp.Register("AllowMaximize", ref AllowMaximizeProperty, true, (dObj, e) => ((BaseLayoutItem)dObj).OnAllowMaximizeChanged((bool)e.NewValue));
			dProp.Register("AllowMinimize", ref AllowMinimizeProperty, true, (dObj, e) => ((BaseLayoutItem)dObj).OnAllowMinimizeChanged((bool)e.NewValue));
			dProp.Register("AllowSizing", ref AllowSizingProperty, true,
				(dObj, e) => ((BaseLayoutItem)dObj).OnAllowSizingChanged((bool)e.NewValue),
				(dObj, value) => ((BaseLayoutItem)dObj).CoerceAllowSizing((bool)value));
			dProp.Register("AllowDockToCurrentItem", ref AllowDockToCurrentItemProperty, true);
			dProp.Register("FloatOnDoubleClick", ref FloatOnDoubleClickProperty, true);
			dProp.RegisterReadonly("IsFloatingRootItem", ref IsFloatingRootItemPropertyKey, ref IsFloatingRootItemProperty, false,
				(dObj, e) => ((BaseLayoutItem)dObj).OnIsFloatingRootItemChanged((bool)e.NewValue));
			dProp.RegisterReadonly("IsSelected", ref IsSelectedPropertyKey, ref IsSelectedProperty, false,
				(dObj, e) => ((BaseLayoutItem)dObj).OnIsSelectedChanged((bool)e.NewValue));
			dProp.RegisterReadonly("IsControlItemsHost", ref IsControlItemsHostPropertyKey, ref IsControlItemsHostProperty, false,
				(dObj, e) => ((BaseLayoutItem)dObj).OnIsControlItemsHostChanged((bool)e.NewValue),
				(dObj, value) => ((BaseLayoutItem)dObj).CoerceIsControlItemsHost((bool)value));
			dProp.Register("Visibility", ref VisibilityProperty, Visibility.Visible,
				(dObj, e) => ((BaseLayoutItem)dObj).OnVisibilityChanged((Visibility)e.NewValue),
				(dObj, value) => ((BaseLayoutItem)dObj).CoerceVisibility((Visibility)value));
			dProp.RegisterReadonly("IsVisible", ref IsVisiblePropertyKey, ref IsVisibleProperty, true,
				(dObj, e) => ((BaseLayoutItem)dObj).OnIsVisibleChanged((bool)e.NewValue),
				(dObj, value) => ((BaseLayoutItem)dObj).CoerceIsVisible());
			dProp.RegisterReadonly("IsTabPage", ref IsTabPagePropertyKey, ref IsTabPageProperty, false,
				(dObj, e) => ((BaseLayoutItem)dObj).OnIsTabPageChanged(),
				(dObj, value) => ((BaseLayoutItem)dObj).CoerceIsTabPage((bool)value));
			dProp.RegisterReadonly("IsSelectedItem", ref IsSelectedItemPropertyKey, ref IsSelectedItemProperty, false,
				(dObj, e) => ((BaseLayoutItem)dObj).OnIsSelectedItemChanged(),
				(dObj, value) => ((BaseLayoutItem)dObj).CoerceIsSelectedItem((bool)value));
			dProp.Register("ControlBoxContent", ref ControlBoxContentProperty, (object)null,
				(dObj, e) => ((BaseLayoutItem)dObj).OnControlBoxContentChanged());
			dProp.Register("ControlBoxContentTemplate", ref ControlBoxContentTemplateProperty, (DataTemplate)null,
				(dObj, e) => ((BaseLayoutItem)dObj).OnControlBoxContentTemplateChanged());
			dProp.Register("ShowCaption", ref ShowCaptionProperty, true,
				(dObj, e) => ((BaseLayoutItem)dObj).OnShowCaptionChanged((bool)e.NewValue));
			dProp.Register("ShowCaptionImage", ref ShowCaptionImageProperty, true,
				(dObj, e) => ((BaseLayoutItem)dObj).OnShowCaptionImageChanged((bool)e.NewValue));
			dProp.Register("ShowControlBox", ref ShowControlBoxProperty, true,
				(dObj, e) => ((BaseLayoutItem)dObj).OnShowControlBoxChanged((bool)e.NewValue));
			dProp.Register("ShowCloseButton", ref ShowCloseButtonProperty, true,
				(dObj, e) => ((BaseLayoutItem)dObj).OnShowCloseButtonChanged((bool)e.NewValue));
			dProp.Register("ShowPinButton", ref ShowPinButtonProperty, true,
				(dObj, e) => ((BaseLayoutItem)dObj).OnShowPinButtonChanged((bool)e.NewValue));
			dProp.Register("ShowMinimizeButton", ref ShowMinimizeButtonProperty, true,
				(dObj, e) => ((BaseLayoutItem)dObj).OnShowMinimizeButtonChanged((bool)e.NewValue));
			dProp.Register("ShowMaximizeButton", ref ShowMaximizeButtonProperty, true,
				(dObj, e) => ((BaseLayoutItem)dObj).OnShowMaximizeButtonChanged((bool)e.NewValue));
			dProp.Register("ShowRestoreButton", ref ShowRestoreButtonProperty, true,
				(dObj, e) => ((BaseLayoutItem)dObj).OnShowRestoreButtonChanged((bool)e.NewValue));
			dProp.Register("ShowDropDownButton", ref ShowDropDownButtonProperty, true,
				(dObj, e) => ((BaseLayoutItem)dObj).OnShowDropDownButtonChanged((bool)e.NewValue));
			dProp.Register("ShowScrollPrevButton", ref ShowScrollPrevButtonProperty, true,
				(dObj, e) => ((BaseLayoutItem)dObj).OnShowScrollPrevButtonChanged((bool)e.NewValue));
			dProp.Register("ShowScrollNextButton", ref ShowScrollNextButtonProperty, true,
				(dObj, e) => ((BaseLayoutItem)dObj).OnShowScrollNextButtonChanged((bool)e.NewValue));
			dProp.Register("TabCaption", ref TabCaptionProperty, (object)null,
				(dObj, e) => ((BaseLayoutItem)dObj).OnTabCaptionChanged(e.NewValue),
				(dObj, value) => ((BaseLayoutItem)dObj).CoerceTabCaption(value));
			dProp.Register("TabCaptionFormat", ref TabCaptionFormatProperty, (string)null,
				(dObj, e) => ((BaseLayoutItem)dObj).OnTabCaptionFormatChanged((string)e.NewValue),
				(dObj, value) => ((BaseLayoutItem)dObj).CoerceTabCaptionFormat((string)value));
			dProp.RegisterReadonly("ActualTabCaption", ref ActualTabCaptionPropertyKey, ref ActualTabCaptionProperty, (string)null,
				(dObj, e) => ((BaseLayoutItem)dObj).OnActualTabCaptionChanged((string)e.NewValue),
				(dObj, value) => ((BaseLayoutItem)dObj).CoerceActualTabCaption((string)value));
			dProp.Register("TabCaptionWidth", ref TabCaptionWidthProperty, double.NaN,
				(dObj, e) => ((BaseLayoutItem)dObj).OnTabCaptionWidthChanged((double)e.NewValue),
				(dObj, value) => ((BaseLayoutItem)dObj).CoerceTabCaptionWidth((double)value));
			dProp.RegisterReadonly("HasTabCaption", ref HasTabCaptionPropertyKey, ref HasTabCaptionProperty, (bool)false);
			dProp.Register("TextWrapping", ref TextWrappingProperty, TextWrapping.NoWrap);
#if!SILVERLIGHT
			dProp.Register("TextTrimming", ref TextTrimmingProperty, TextTrimming.CharacterEllipsis);
#else
			dProp.Register("TextTrimming", ref TextTrimmingProperty, TextTrimming.None);
#endif
			dProp.Register("ToolTip", ref ToolTipProperty, (object)null);
			dProp.Register("CaptionTemplate", ref CaptionTemplateProperty, (DataTemplate)null,
				(dObj, e) => ((BaseLayoutItem)dObj).OnCaptionTemplateChanged((DataTemplate)e.OldValue, (DataTemplate)e.NewValue));
#if !SILVERLIGHT
			dProp.Register("CaptionTemplateSelector", ref CaptionTemplateSelectorProperty, (DataTemplateSelector)null,
				(dObj, e) => ((BaseLayoutItem)dObj).OnCaptionTemplateSelectorChanged((DataTemplateSelector)e.OldValue, (DataTemplateSelector)e.NewValue));
			dProp.RegisterAttachedInherited("IsLayoutChangeInProgress", ref IsLayoutChangeInProgressProperty, false);
#endif
			dProp.Register("ClosingBehavior", ref ClosingBehaviorProperty, ClosingBehavior.Default);
			dProp.Register("LayoutItemData", ref LayoutItemDataProperty, (LayoutItemData)null);
			dProp.RegisterReadonly("IsCaptionVisible", ref IsCaptionVisiblePropertyKey, ref IsCaptionVisibleProperty, true,
				(dObj, ea) => ((BaseLayoutItem)dObj).OnIsCaptionVisibleChanged((bool)ea.NewValue),
				(dObj, value) => ((BaseLayoutItem)dObj).CoerceIsCaptionVisible((bool)value));
			dProp.RegisterReadonly("IsCaptionImageVisible", ref IsCaptionImageVisiblePropertyKey, ref IsCaptionImageVisibleProperty, false, null,
				(dObj, value) => ((BaseLayoutItem)dObj).CoerceIsCaptionImageVisible((bool)value));
			dProp.RegisterReadonly("IsControlBoxVisible", ref IsControlBoxVisiblePropertyKey, ref IsControlBoxVisibleProperty, false, null,
				(dObj, value) => ((BaseLayoutItem)dObj).CoerceIsControlBoxVisible((bool)value));
			dProp.RegisterReadonly("IsCloseButtonVisible", ref IsCloseButtonVisiblePropertyKey, ref IsCloseButtonVisibleProperty, false,
				(dObj, e) => ((BaseLayoutItem)dObj).OnIsCloseButtonVisibleChanged((bool)e.NewValue),
				(dObj, value) => ((BaseLayoutItem)dObj).CoerceIsCloseButtonVisible((bool)value));
			dProp.RegisterReadonly("IsPinButtonVisible", ref IsPinButtonVisiblePropertyKey, ref IsPinButtonVisibleProperty, false, null,
				(dObj, value) => ((BaseLayoutItem)dObj).CoerceIsPinButtonVisible((bool)value));
			dProp.RegisterReadonly("IsMinimizeButtonVisible", ref IsMinimizeButtonVisiblePropertyKey, ref IsMinimizeButtonVisibleProperty, false, null,
				(dObj, value) => ((BaseLayoutItem)dObj).CoerceIsMinimizeButtonVisible((bool)value));
			dProp.RegisterReadonly("IsMaximizeButtonVisible", ref IsMaximizeButtonVisiblePropertyKey, ref IsMaximizeButtonVisibleProperty, false,
				(dObj, e) => ((BaseLayoutItem)dObj).OnIsMaximizeButtonVisibleChanged((bool)e.NewValue),
				(dObj, value) => ((BaseLayoutItem)dObj).CoerceIsMaximizeButtonVisible((bool)value));
			dProp.RegisterReadonly("IsRestoreButtonVisible", ref IsRestoreButtonVisiblePropertyKey, ref IsRestoreButtonVisibleProperty, false,
				(dObj, e) => ((BaseLayoutItem)dObj).OnIsRestoreButtonVisibleChanged((bool)e.NewValue),
				(dObj, value) => ((BaseLayoutItem)dObj).CoerceIsRestoreButtonVisible((bool)value));
			dProp.RegisterReadonly("IsDropDownButtonVisible", ref IsDropDownButtonVisiblePropertyKey, ref IsDropDownButtonVisibleProperty, false, null,
				(dObj, value) => ((BaseLayoutItem)dObj).CoerceIsDropDownButtonVisible((bool)value));
			dProp.RegisterReadonly("IsScrollPrevButtonVisible", ref IsScrollPrevButtonVisiblePropertyKey, ref IsScrollPrevButtonVisibleProperty, false, null,
				(dObj, value) => ((BaseLayoutItem)dObj).CoerceIsScrollPrevButtonVisible((bool)value));
			dProp.RegisterReadonly("IsScrollNextButtonVisible", ref IsScrollNextButtonVisiblePropertyKey, ref IsScrollNextButtonVisibleProperty, false, null,
				(dObj, value) => ((BaseLayoutItem)dObj).CoerceIsScrollNextButtonVisible((bool)value));
			dProp.Register("CloseCommand", ref CloseCommandProperty, (ICommand)null);
			dProp.Register("BindableName", ref BindableNameProperty, string.Empty,
				(dObj, e) => ((BaseLayoutItem)dObj).OnBindableNameChanged((string)e.OldValue, (string)e.NewValue));
#if !SILVERLIGHT
			dProp.OverrideFrameworkMetadata(MinHeightProperty, 0d,
				(dObj, e) => ((BaseLayoutItem)dObj).OnMinHeightChanged((double)e.OldValue, (double)e.NewValue),
				(dObj, value) => ((BaseLayoutItem)dObj).OnCoerceMinHeight((double)value));
			dProp.OverrideFrameworkMetadata(MinWidthProperty, 0d,
				(dObj, e) => ((BaseLayoutItem)dObj).OnMinWidthChanged((double)e.OldValue, (double)e.NewValue),
				(dObj, value) => ((BaseLayoutItem)dObj).OnCoerceMinWidth((double)value));
			dProp.OverrideFrameworkMetadata(MaxWidthProperty, double.PositiveInfinity,
				(dObj, e) => ((BaseLayoutItem)dObj).OnMaxWidthChanged((double)e.OldValue, (double)e.NewValue));
			dProp.OverrideFrameworkMetadata(MaxHeightProperty, double.PositiveInfinity,
				(dObj, e) => ((BaseLayoutItem)dObj).OnMaxHeightChanged((double)e.OldValue, (double)e.NewValue));
#else
			dProp.Register("MinHeight", ref MinHeightProperty, 0d,
				(dObj, e) => ((BaseLayoutItem)dObj).OnMinHeightChanged((double)e.OldValue, (double)e.NewValue),
				(dObj, value) => ((BaseLayoutItem)dObj).OnCoerceMinHeight((double)value));
			dProp.Register("MinWidth", ref MinWidthProperty, 0d,
				(dObj, e) => ((BaseLayoutItem)dObj).OnMinWidthChanged((double)e.OldValue, (double)e.NewValue),
				(dObj, value) => ((BaseLayoutItem)dObj).OnCoerceMinWidth((double)value));
			dProp.Register("MaxHeight", ref MaxHeightProperty, double.PositiveInfinity,
				(dObj, e) => ((BaseLayoutItem)dObj).OnMaxHeightChanged((double)e.OldValue, (double)e.NewValue));
			dProp.Register("MaxWidth", ref MaxWidthProperty, double.PositiveInfinity,
				(dObj, e) => ((BaseLayoutItem)dObj).OnMaxWidthChanged((double)e.OldValue, (double)e.NewValue));
#endif
		}
#if !SILVERLIGHT
		internal static bool GetIsLayoutChangeInProgress(DependencyObject target) {
			return (bool)target.GetValue(IsLayoutChangeInProgressProperty);
		}
		internal static void SetIsLayoutChangeInProgress(DependencyObject target, bool value) {
			target.SetValue(IsLayoutChangeInProgressProperty, value);
		}
#endif
		protected virtual void OnFloatSizeChanged(Size newValue) { }
		#endregion static
		internal LayoutItemData LayoutItemData {
			get { return (LayoutItemData)GetValue(LayoutItemDataProperty); }
			set { SetValue(LayoutItemDataProperty, value); }
		}
		PlaceHolderInfoHelper dockInfo;
		internal PlaceHolderInfoHelper DockInfo {
			get {
				if(dockInfo == null) dockInfo = new PlaceHolderInfoHelper();
				return dockInfo;
			}
		}
#if !SILVERLIGHT
		LockHelper logicalTreeLockHelper;
		internal LockHelper LogicalTreeLockHelper {
			get {
				if(logicalTreeLockHelper == null) logicalTreeLockHelper = new LockHelper(UnlockLogicalTreeCore);
				return logicalTreeLockHelper;
			}
		}
		protected internal virtual void UnlockLogicalTree() {
			LogicalTreeLockHelper.Unlock();
		}
		protected virtual void UnlockLogicalTreeCore() { }
		internal bool IsLogicalTreeLocked { get { return LogicalTreeLockHelper; } }
#endif
		LockHelper parentLockHelper;
		internal LockHelper ParentLockHelper {
			get {
				if(parentLockHelper == null) parentLockHelper = new LockHelper();
				return parentLockHelper;
			}
		}
		LockHelper resizeLockHelper;
		internal LockHelper ResizeLockHelper {
			get {
				if(resizeLockHelper == null) resizeLockHelper = new LockHelper();
				return resizeLockHelper;
			}
		}
		internal bool PreventCaptionSerialization { get; set; }
		protected internal virtual bool IsPinnedTab { get { return false; } }
		protected BaseLayoutItem() {
#if SILVERLIGHT
			DefaultStyleKey = typeof(BaseLayoutItem);
			DevExpress.Xpf.Core.Serialization.DXSerializer.SetSerializationProvider(this, new BaseLayoutItemSerializationProvider());
#endif
			Bars.MergingProperties.SetElementMergingBehavior(this, Bars.ElementMergingBehavior.Undefined);
			LayoutItemData = new Docking.LayoutItemData(this);
			isInDesignTime = DesignerProperties.GetIsInDesignMode(this);
			OnCreate();
			SetIsVisible(true);
			if(!isInDesignTime) {
				CoerceValue(CaptionFormatProperty);
				CoerceValue(TabCaptionFormatProperty);
			}
			CoerceValue(CustomizationCaptionProperty);
			CoerceValue(ActualMarginProperty);
			CoerceValue(ActualPaddingProperty);
			CoerceValue(IsCloseButtonVisibleProperty);
			CoerceValue(IsPinButtonVisibleProperty);
			CoerceValue(IsMinimizeButtonVisibleProperty);
			CoerceValue(IsMaximizeButtonVisibleProperty);
			CoerceValue(IsRestoreButtonVisibleProperty);
			CoerceValue(IsDropDownButtonVisibleProperty);
			CoerceValue(IsScrollNextButtonVisibleProperty);
			CoerceValue(IsScrollPrevButtonVisibleProperty);
			CoerceValue(IsControlBoxVisibleProperty);
		}
		protected VisualElements.MultiTemplateControl PartMultiTemplateControl { get; private set; }
		protected bool IsTemplateApplied { get; private set; }
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			IsTemplateApplied = true;
			if(PartMultiTemplateControl != null && !LayoutItemsHelper.IsTemplateChild(PartMultiTemplateControl, this))
				PartMultiTemplateControl.Dispose();
			PartMultiTemplateControl = LayoutItemsHelper.GetChild<VisualElements.MultiTemplateControl>(this);
			SelectTemplate();
			UpdateVisualState();
#if !SILVERLIGHT
			LogicalTreeLockHelper.Unlock();
#endif
		}
		protected virtual void UpdateVisualState() {
			if(IsActive) {
				VisualStateManager.GoToState(this, "Active", false);
			}
			else {
				VisualStateManager.GoToState(this, "Inactive", false);
			}
		}
		protected internal virtual IUIElement FindUIScopeCore() {
#if !SILVERLIGHT
			IUIElement scope = Parent == null || Parent is FloatGroup || Parent is AutoHideGroup ? this.FindUIScope() : Parent;
#else
			IUIElement scope = this.FindUIScope();
#endif
			if(IsTabPage)
				return IsSelectedItem ? scope : null;
			return scope;
		}
		IUIElement EnsureUIScope() {
			IUIElement scope = FindUIScopeCore();
			SetValue(DockLayoutManager.UIScopeProperty, scope);
			if(scope != null) {
				DockLayoutManager.Ensure(this, true);
				SetValue(DockLayoutManager.LayoutItemProperty, this);
				UIElements.Add(this);
			}
			return scope;
		}
		protected internal virtual void SelectTemplate() {
#if SILVERLIGHT
			if(IsTabPage && !IsSelectedItem) return;
#endif
			if(PartMultiTemplateControl != null) {
				PartMultiTemplateControl.LayoutItem = this;
			}
			EnsureUIScope();
		}
		internal void EnsureTemplate() {
			if(IsTemplateApplied && PartMultiTemplateControl != null) {
				if(PartMultiTemplateControl.LayoutItem == null)
					PartMultiTemplateControl.LayoutItem = this;
			}
		}
		protected internal virtual void SelectTemplateIfNeeded() {
#if SILVERLIGHT
			if(((IUIElement)this).Scope == null) SelectTemplate();
#else
			SelectTemplate();
#endif
		}
		protected internal virtual void ClearTemplate() {
			ClearTemplateCore();
		}
		protected internal virtual void ClearTemplateCore() {
			if(PartMultiTemplateControl != null) {
				PartMultiTemplateControl.LayoutItem = null;
			}
			this.ClearValue(DockLayoutManager.UIScopeProperty);
#if SILVERLIGHT
			DockLayoutManager.Release(this);
#else
			this.ClearValue(DockLayoutManager.DockLayoutManagerProperty);
#endif
		}
		protected override void OnVisualParentChanged(DependencyObject oldParent) {
			base.OnVisualParentChanged(oldParent);
			if(VisualParent != null) {
				EnsureUIScope();
				if(IsFloating) Manager.Do(x => x.InvalidateView(this.GetRoot()));
			}
		}
		protected virtual void OnCreate() {
			EnsureAppearance();
		}
		void EnsureAppearance() {
#if SILVERLIGHT
			if(!isInDesignTime) 
#endif
				CoerceValue(AppearanceProperty);
		}
		protected virtual internal void LockInLogicalTree() {
			LockDataContext();
			LockDockItemState();
		}
		protected virtual internal void UnlockItemInLogicalTree() {
			UnlockDataContext();
			UnlockDockItemState();
		}
		int lockDataContext;
		int lockDataContextCount;
		protected virtual internal void LockDataContext() {
			lockDataContext++;
		}
		protected virtual internal void UnlockDataContext() {
			if(--lockDataContext == 0) {
				if(lockDataContextCount > 0) InvokeCoerceDataContext();
			}
		}
		bool IsDataContextLocked { get { return lockDataContext > 0; } }
		protected internal void InvokeCoerceDataContext() {
			if(!IsDataContextLocked) {
				CoerceValue(DataContextProperty);
				lockDataContextCount = 0;
			}
			else lockDataContextCount++;
		}
		int lockDockItemState;
		int lockDockItemStateCount;
		void LockDockItemState() {
			lockDockItemState++;
		}
		void UnlockDockItemState() {
			if(--lockDockItemState == 0) {
				if(lockDockItemStateCount > 0) InvokeCoerceDockItemState();
			}
		}
		bool IsDockItemStateLocked { get { return lockDockItemState > 0; } }
		protected internal void InvokeCoerceDockItemState() {
			if(!IsDockItemStateLocked) {
				InvokeCoerceDockItemStateCore();
				lockDockItemStateCount = 0;
			}
			else lockDockItemStateCount++;
		}
		protected virtual void InvokeCoerceDockItemStateCore() { }
		internal bool IsDockingTarget { get; set; }
		protected internal virtual bool ToggleTabPinStatus() {
			return false;
		}
		#region Sizes
		protected virtual void OnWidthChanged(GridLength value, GridLength prevValue) {
			if(Manager != null)
				Manager.RaiseItemSizeChangedEvent(this, true, value, prevValue);
		}
		protected virtual void OnHeightChanged(GridLength value, GridLength prevValue) {
			if(Manager != null)
				Manager.RaiseItemSizeChangedEvent(this, false, value, prevValue);
		}
		protected virtual Size CalcMinSizeValue(Size value) { return value; }
		protected virtual Size CalcMaxSizeValue(Size value) { return value; }
		protected virtual void OnPaddingChanged(Thickness value) {
			CoerceValue(ActualPaddingProperty);
		}
		protected virtual void OnMarginChanged(Thickness value) {
			CoerceValue(ActualMarginProperty);
		}
		protected virtual void OnActualMarginChanged(Thickness value) { }
		protected virtual Thickness CoerceActualPadding(Thickness value) {
			return MathHelper.AreEqual(Padding, new Thickness(double.NaN)) ? value : Padding;
		}
		protected virtual Thickness CoerceActualMargin(Thickness value) {
			return MathHelper.AreEqual(Margin, new Thickness(double.NaN)) ? value : Margin;
		}
		protected virtual void OnActualMinSizeChanged() {
			CoerceValue(ActualMaxSizeProperty);
			CoerceSizes();
			CoerceParentProperty(ActualMinSizeProperty);
			DefinitionsHelper.GetDefinition(this).Do(x =>x.SetMinSize(ActualMinSize));
		}
		protected virtual void OnActualMaxSizeChanged() {
			CoerceSizes();
			CoerceParentProperty(ActualMaxSizeProperty);
			DefinitionsHelper.GetDefinition(this).Do(x => x.SetMaxSize(ActualMaxSize));
		}
		protected virtual Size CoerceActualMinSize() {
			Size min = new Size(
				MathHelper.IsConstraintValid(MinWidth) ? MinWidth : 0d,
				MathHelper.IsConstraintValid(MinHeight) ? MinHeight : 0d);
			return CalcMinSizeValue(min);
		}
		protected virtual Size CoerceActualMaxSize() {
			Size max = new Size(
				MathHelper.IsConstraintValid(MaxWidth) ? MaxWidth : double.NaN,
				MathHelper.IsConstraintValid(MaxHeight) ? MaxHeight : double.NaN);
			Size value = CalcMaxSizeValue(max);
			double w = Math.Max(ActualMinSize.Width, value.Width);
			double h = Math.Max(ActualMinSize.Height, value.Height);
			return new Size(w, h);
		}
		protected virtual void CoerceSizes() {
			CoerceValue(FloatSizeProperty);
			CoerceValue(ItemWidthProperty);
			CoerceValue(ItemHeightProperty);
			CoerceValue(AutoHideGroup.AutoHideSizeProperty);
			if(Parent != null) Parent.CoerceSizes();
		}
		protected void CoerceParentProperty(DependencyProperty property) {
			if(Parent != null) Parent.CoerceValue(property);
		}
		protected virtual Size CoerceFloatSize(Size value) {
			return MathHelper.MeasureSize(ActualMinSize, ActualMaxSize, value);
		}
		protected virtual GridLength CoerceWidth(GridLength value) {
			if(value.IsAbsolute) {
				double width = MathHelper.MeasureDimension(ActualMinSize.Width, ActualMaxSize.Width, value.Value);
				value = new GridLength(width, GridUnitType.Pixel);
			}
			return value;
		}
		protected virtual GridLength CoerceHeight(GridLength value) {
			if(value.IsAbsolute) {
				double height = MathHelper.MeasureDimension(ActualMinSize.Height, ActualMaxSize.Height, value.Value);
				value = new GridLength(height, GridUnitType.Pixel);
			}
			return value;
		}
		protected virtual void OnHasCaptionChanged(bool hasCaption) { }
		protected virtual void OnHasCaptionTemplateChanged(bool hasCaptionTemplate) { }
		#endregion Sizes
		#region Hierarchical Properties
		protected virtual object CoerceDataContext(object value) {
			if(IsDataContextLocked) return DataContext;
			return value;
		}
		protected virtual CaptionAlignMode CoerceCaptionAlignMode(object value) {
			return GetHierarchyPropertyValue(CaptionAlignModeProperty, value, CaptionAlignMode.Default);
		}
		T GetHierarchyPropertyValue<T>(DependencyProperty property, object value, T defaultValue) {
			return new HierarchyPropertyValue<T>(property, defaultValue).Get(this, value);
		}
		#endregion Hierarchical Properties
		Locker activationCancelLocker = new Locker();
		protected bool isInDesignTime;
		int initCounter;
		bool isInitializedCore;
#if !SL
	[DevExpressXpfDockingLocalizedDescription("BaseLayoutItemIsInitializing")]
#endif
		public bool IsInitializing {
			get { return initCounter > 0; }
		}
#if SILVERLIGHT
		public bool IsInitialized {
			get { return initCounter == 0 && isInitializedCore; }
		}
		public event EventHandler Initialized;
		protected void RaiseInitialized() {
			if(Initialized != null)
				Initialized(this, EventArgs.Empty);
		}
#endif
		public override void BeginInit() {
			base.BeginInit();
			initCounter++;
		}
		public override void EndInit() {
			if(--initCounter == 0) {
				if(!isInitializedCore) {
					isInitializedCore = true;
					OnInitialized();
#if SILVERLIGHT
					RaiseInitialized();
#endif
				}
			}
			base.EndInit();
		}
		UIChildren uiChildren;
		protected internal UIChildren UIElements {
			get {
				if(uiChildren == null) uiChildren = new UIChildren();
				return uiChildren;
			}
		}
		protected internal virtual T GetUIElement<T>() where T : class {
			IUIElement[] elements = this.UIElements.GetElements();
			return System.Linq.Enumerable.FirstOrDefault(elements, (element) => element is T) as T;
		}
		int zIndexCore;
		internal int ZIndex {
			get { return zIndexCore; }
			set {
				if(ZIndex == value) return;
				zIndexCore = value;
				SetValue(VisualElements.MDIPanel.ZIndexProperty, value);
				OnZIndexChanged();
			}
		}
		DockLayoutManager managerCore;
		protected internal DockLayoutManager Manager {
			get { return managerCore; }
			set {
				if(managerCore == value) return;
				DockLayoutManager oldValue = managerCore;
				managerCore = value;
				OnDockLayoutManagerChanged(oldValue, value);
			}
		}
		protected internal event EventHandler VisualChanged;
		protected internal event EventHandler GeometryChanged;
		protected void RaiseVisualChanged() {
			if(VisualChanged != null)
				VisualChanged(this, EventArgs.Empty);
		}
		protected void RaiseGeometryChanged() {
			if(GeometryChanged != null)
				GeometryChanged(this, EventArgs.Empty);
		}
		protected virtual void OnDockLayoutManagerChanged() {
			InvokeCoerceDataContext();
			CoerceValue(CaptionImageProperty);
			CheckClosedState();
		}
		protected virtual void OnDockLayoutManagerChanged(DockLayoutManager oldValue, DockLayoutManager newValue) {
			OnDockLayoutManagerChanged();
			if(newValue != null) {
				ParentLockHelper.DoWhenUnlocked(() =>
				{
					if(IsActive) TryToActivate();
				});
			}
		}
		protected virtual void OnZIndexChanged() { }
		protected void RegisterName() {
#if SILVERLIGHT
			if(!isDeserializing && !string.IsNullOrEmpty(Name) && string.IsNullOrEmpty((string)this.GetValue(FrameworkElement.NameProperty)))
				SetValue(FrameworkElement.NameProperty, Name);
#endif
		}
		protected virtual void OnInitialized() {
			InvokeCoerceDataContext();
			RegisterName();
		}
		protected virtual void OnDataContextChanged(object value) { }
		protected virtual void OnIsTabPageChanged() { }
		protected virtual bool CoerceIsTabPage(bool value) {
			return (Parent != null) && Parent.GroupBorderStyle == GroupBorderStyle.Tabbed;
		}
		protected virtual void OnIsSelectedItemChanged() { }
		protected virtual bool CoerceIsSelectedItem(bool value) {
			return (Parent != null) && Parent.SelectedItem == this;
		}
		protected virtual void OnNameChanged() {
			CoerceValue(CustomizationCaptionProperty);
		}
		protected virtual void OnCaptionFormatChanged() {
			CoerceValue(ActualCaptionProperty);
		}
		protected virtual void OnCaptionLocationChanged(CaptionLocation value) { }
		protected virtual void OnCaptionWidthChanged(double value) { }
		protected virtual void OnDesiredCaptionWidthChanged(double value) {
			SetValue(HasDesiredCaptionWidthPropertyKey, !double.IsNaN(value));
			if(double.IsNaN(value)) {
				InvalidateTabHeader();
			}
			CoerceValue(ActualCaptionWidthProperty);
		}
		protected virtual double CoerceCaptionWidth(double value) {
			return CaptionAlignHelper.GetCaptionWidth(this, value);
		}
		protected virtual string CoerceCaptionFormat(string captionFormat) {
			return captionFormat;
		}
		protected virtual void OnCaptionAlignModeChanged(CaptionAlignMode oldValue, CaptionAlignMode value) { }
		protected virtual double CoerceActualCaptionWidth(double value) { return value; }
		protected virtual string CoerceActualCaption(string value) {
			if(!string.IsNullOrEmpty(CaptionFormat) && Caption != null) {
				if(Caption is string || Caption.GetType().IsPrimitive) return string.Format(CaptionFormat, Caption);
			}
			return Caption as string;
		}
		protected virtual void OnActualCaptionChanged(string value) {
			SetValue(HasCaptionPropertyKey, !string.IsNullOrEmpty(value));
		}
		protected virtual string CoerceCustomizationCaption(string value) {
#if SILVERLIGHT
			if(isInDesignTime) return value;
#endif
			string caption = Caption as string;
			if(!string.IsNullOrEmpty(value)) return value;
			if(!string.IsNullOrEmpty(caption)) return caption;
			if(!string.IsNullOrEmpty(Name)) return Name;
			return TypeName;
		}
		protected virtual ImageSource CoerceCaptionImage(ImageSource value) {
#if !SILVERLIGHT
			DockLayoutManager manager = this.FindDockLayoutManager();
			if(manager == null) return value;
			if(value == null && string.IsNullOrEmpty(Caption as string)) {
				if(IsTabPage)
					return manager.DefaultTabPageCaptionImage;
				if(IsAutoHidden)
					return manager.DefaultAutoHidePanelCaptionImage;
			}
#endif
			return value;
		}
		protected virtual void OnAllowDockChanged(bool value) {
			CoerceValue(IsPinButtonVisibleProperty);
		}
		protected virtual void OnAllowHideChanged(bool value) {
			CoerceValue(IsPinButtonVisibleProperty);
		}
		protected virtual void OnAllowCloseChanged(bool value) {
			CoerceValue(IsCloseButtonVisibleProperty);
		}
		protected virtual void OnAllowMaximizeChanged(bool value) {
			CoerceValue(IsMaximizeButtonVisibleProperty);
		}
		protected virtual void OnAllowMinimizeChanged(bool value) {
			CoerceValue(IsMinimizeButtonVisibleProperty);
		}
		protected virtual object CoerceAllowSizing(bool value) { return value; }
		protected virtual void OnAllowSizingChanged(bool value) {
			if(IsFloatingRootItem) Parent.CoerceValue(AllowSizingProperty);
		}
		protected virtual void OnIsActiveChanged(bool value) {
			if(ParentLockHelper.IsLocked) ParentLockHelper.AddUnlockAction(OnIsActiveChangedCore);
			else
#if !SILVERLIGHT
				if(LogicalTreeLockHelper.IsLocked && value) LogicalTreeLockHelper.AddUnlockAction(OnIsActiveChangedCore);
				else
#endif
					OnIsActiveChangedCore();
		}
		protected virtual void OnIsActiveChangedCore() {
			if(!activationCancelLocker.IsLocked) TryToActivate();
			CoerceValue(ActualAppearanceObjectProperty);
			RaiseVisualChanged();
			UpdateVisualState();
		}
		protected void TryToActivate() {
			DockLayoutManager dockManager = this.Manager;
#if !SILVERLIGHT
			if(dockManager == null) {
				LayoutGroup logicalParent = Parent ?? LogicalTreeHelper.GetParent(this) as LayoutGroup;
				if(logicalParent != null) dockManager = logicalParent.GetRoot().GetDockLayoutManager();
			}
			bool canActivate = !IsActivationLocked && dockManager != null && !LogicalTreeLockHelper.IsLocked;
#else
			bool canActivate = !IsActivationLocked && dockManager != null;
#endif
			if(canActivate) {
				if(IsActive) dockManager.Activate(this);
				else dockManager.Deactivate(this);
			}
		}
		internal void InvokeCancelActivation(BaseLayoutItem activeItem) {
			Dispatcher.BeginInvoke(new Action<BaseLayoutItem>(TryCancelActivation), activeItem);
		}
		void TryCancelActivation(BaseLayoutItem activeItem) {
			if(!IsActive) return;
			activationCancelLocker.Lock();
			if(activeItem != this) this.SetCurrentValue(IsActiveProperty, false);
			activationCancelLocker.Unlock();
		}
		protected virtual void OnIsControlItemsHostChanged(bool value) { }
		protected virtual bool CoerceIsControlItemsHost(bool value) { return false; }
		protected virtual void OnIsSelectedChanged(bool selected) {
			if(Manager != null)
				Manager.RaiseItemSelectionChangedEvent(this, selected);
		}
		protected virtual void OnShowCaptionChanged(bool value) {
			CoerceValue(IsCaptionVisibleProperty);
			CoerceValue(IsCaptionImageVisibleProperty);
		}
		protected virtual void OnShowCaptionImageChanged(bool value) {
			CoerceValue(IsCaptionImageVisibleProperty);
		}
		protected virtual void OnShowControlBoxChanged(bool value) {
			CoerceValue(IsControlBoxVisibleProperty);
			CoerceValue(IsCloseButtonVisibleProperty);
		}
		protected virtual void OnCaptionChanged(object oldValue, object newValue) {
			CoerceValue(ActualCaptionProperty);
			CoerceValue(CaptionImageProperty);
			CoerceValue(CustomizationCaptionProperty);
			CoerceValue(IsCaptionVisibleProperty);
			ClearValue(DesiredCaptionWidthPropertyKey);
			CoerceValue(TabCaptionProperty);
		}
		protected virtual void OnCaptionImageChanged(ImageSource value) {
			SetValue(HasImagePropertyKey, value != null);
			CoerceValue(IsCaptionImageVisibleProperty);
		}
		protected virtual Visibility CoerceVisibility(Visibility visibility) {
			DockLayoutManager manager = this.FindDockLayoutManager();
			if(manager == null) return visibility;
			bool? show = manager.ShowInvisibleItems;
			bool actualShow = show.HasValue ? show.Value :
				(isInDesignTime ? manager.IsCustomization : manager.IsCustomization && manager.ShowInvisibleItemsInCustomizationForm);
			return VisibilityHelper.Convert(actualShow, visibility);
		}
		protected virtual void OnVisibilityChanged(Visibility visibility) {
			var container = this.FindDockLayoutManager();
			if(container != null && container.IsThemeChangedLocked) Dispatcher.BeginInvoke(new Action(() => { OnVisibilityChangedOverride(visibility); }));
			else
				OnVisibilityChangedOverride(visibility);
		}
		protected virtual void OnVisibilityChangedOverride(Visibility visibility) {
			SetIsVisible(visibility == System.Windows.Visibility.Visible);
			if(Parent != null)
				Parent.OnItemVisibilityChanged(this, visibility);
			SetValue(FrameworkElement.VisibilityProperty, visibility);
		}
		protected virtual bool CoerceIsVisible() {
			return VisibilityHelper.GetIsVisible(this, isVisibleCore);
		}
		protected virtual void OnIsVisibleChanged(bool isVisible) {
			RaiseIsVisibleChanged(isVisible);
			if(Parent != null)
				Parent.OnItemIsVisibleChanged(this);
		}
		bool isVisibleCore;
		void SetIsVisible(bool value) {
			if(isVisibleCore == value) return;
			isVisibleCore = value;
			CoerceValue(IsVisibleProperty);
		}
		void RaiseIsVisibleChanged(bool isVisible) {
			if(Manager != null)
				Manager.RaiseEvent(new DevExpress.Xpf.Docking.Base.ItemIsVisibleChangedEventArgs(this, isVisible));
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool IsFloatingRootItem {
			get { return (bool)GetValue(IsFloatingRootItemProperty); }
			private set { SetValue(IsFloatingRootItemPropertyKey, value); }
		}
		protected void UpdateIsFloatingRootItem() {
			IsFloatingRootItem = (Parent is FloatGroup && Parent.Items.Count == 1);
		}
		protected virtual void OnTabCaptionWidthChanged(double width) {
			if(!IsTabPage) return;
			InvalidateTabHeader();
		}
		protected virtual object CoerceTabCaptionWidth(double value) {
			return CaptionAlignHelper.GetTabCaptionWidth(this, value);
		}
		protected virtual bool CoerceIsCaptionVisible(bool visible) {
			return ShowCaption;
		}
		protected virtual void OnIsCaptionVisibleChanged(bool isCaptionVisible) { }
		protected virtual bool CoerceIsCaptionImageVisible(bool visible) {
			return HasImage && ShowCaption && ShowCaptionImage;
		}
		Appearance _DefaultAppearance;
		internal Appearance DefaultAppearance {
			get {
				if(_DefaultAppearance == null) _DefaultAppearance = new Appearance();
				return _DefaultAppearance;
			}
		}
		protected virtual object CoerceAppearance(Appearance value) {
			return value ?? DefaultAppearance;
		}
		protected virtual void OnAppearanceChanged(Appearance newValue) {
			CoerceValue(ActualAppearanceProperty);
			if(newValue != null) newValue.Owner = this;
		}
		protected virtual object CoerceActualAppearanceObject(AppearanceObject value) {
			return ActualAppearance == null ? value :
				(IsActive ? ActualAppearance.Active : ActualAppearance.Normal);
		}
		protected virtual void OnActualAppearanceChanged(Appearance newValue) {
			CoerceValue(ActualAppearanceObjectProperty);
			if(newValue != null) newValue.Owner = this;
		}
		protected virtual object CoerceActualAppearance(Appearance value) {
			return AppearanceHelper.GetActualAppearance(Parent != null ? Parent.ActualItemsAppearance : null, Appearance);
		}
		protected virtual void OnActualAppearanceObjectChanged(AppearanceObject newValue) {
			ClearValue(DesiredCaptionWidthPropertyKey);
			RaiseVisualChanged();
		}
		protected internal virtual void OnAppearanceObjectPropertyChanged(DependencyPropertyChangedEventArgs e) {
			CoerceValue(ActualAppearanceProperty);
			CoerceValue(ActualAppearanceObjectProperty);
			RaiseVisualChanged();
		}
		protected virtual void OnTabCaptionChanged(object tabCaption) {
			CoerceValue(ActualTabCaptionProperty);
		}
		protected virtual object CoerceTabCaption(object value) {
			return value ?? Caption;
		}
		protected virtual object CoerceTabCaptionFormat(string captionFormat) {
			if(!string.IsNullOrEmpty(captionFormat)) return captionFormat;
			return DockLayoutManagerParameters.TabCaptionFormat;
		}
		protected virtual void OnTabCaptionFormatChanged(string captionFormat) {
			CoerceValue(ActualTabCaptionProperty);
		}
		protected virtual void OnActualTabCaptionChanged(string value) {
			SetValue(HasTabCaptionPropertyKey, !string.IsNullOrEmpty(value));
		}
		protected virtual object CoerceActualTabCaption(string captionFormat) {
			object caption = TabCaption ?? Caption;
			if(!string.IsNullOrEmpty(TabCaptionFormat) && caption != null) {
				if(caption is string || caption.GetType().IsPrimitive) return string.Format(TabCaptionFormat, caption);
			}
			return caption as string;
		}
		protected virtual void OnClosedChanged(bool oldValue, bool newValue) {
			CheckClosedState();
			InvokeCoerceDataContext();
		}
		void CheckClosedState() {
			if(ParentLockHelper.IsLocked) ParentLockHelper.AddUnlockAction(CheckClosedStateCore);
			else
				CheckClosedStateCore();
		}
		void CheckClosedStateCore() {
			DockLayoutManager manager = this.FindDockLayoutManager();
			if(manager == null || Closed == IsClosed || isInDesignTime) return;
			manager.CheckClosedState(this);
		}
		protected internal virtual void UpdateSizeToContent() { }
		protected override void OnWidthInternalChanged(double oldValue, double newValue) {
			throw new NotSupportedException(DockLayoutManagerHelper.GetRule(DockLayoutManagerRule.WidthIsNotSupported));
		}
		protected override void OnHeightInternalChanged(double oldValue, double newValue) {
			throw new NotSupportedException(DockLayoutManagerHelper.GetRule(DockLayoutManagerRule.HeightIsNotSupported));
		}
		protected virtual double OnCoerceMinWidth(double value) { return value; }
		protected virtual void OnMinWidthChanged(double oldValue, double newValue) {
			CoerceValue(ActualMinSizeProperty);
		}
		protected virtual double OnCoerceMinHeight(double value) { return value; }
		protected virtual void OnMinHeightChanged(double oldValue, double newValue) {
			CoerceValue(ActualMinSizeProperty);
		}
		protected virtual void OnMaxWidthChanged(double oldValue, double newValue) {
			CoerceValue(ActualMaxSizeProperty);
		}
		protected virtual void OnMaxHeightChanged(double oldValue, double newValue) {
			CoerceValue(ActualMaxSizeProperty);
		}
		protected virtual void OnBindableNameChanged(string oldValue, string newValue) {
			if(!string.IsNullOrEmpty(newValue))
				Name = newValue;
		}
		protected internal virtual void UpdateButtons() { }
		protected virtual void OnIsFloatingRootItemChanged(bool newValue) { }
		protected void HasCaptionTemplateEval() {
			HasCaptionTemplate = CaptionTemplate != null || CaptionTemplateSelector != null;
		}
		protected virtual void OnCaptionTemplateChanged(DataTemplate oldValue, DataTemplate newValue) { HasCaptionTemplateEval(); }
		protected virtual void OnCaptionTemplateSelectorChanged(DataTemplateSelector oldValue, DataTemplateSelector newValue) { HasCaptionTemplateEval(); }
		internal virtual void SetIsMaximized(bool isMaximized) { }
		internal virtual void PrepareContainer(object content) { }
		internal virtual void PrepareForModification(bool isDeserializing) {
			if(isDeserializing) {
				if(!LogicalTreeLockHelper.IsLocked)
					LogicalTreeLockHelper.Lock();
				PlaceHolderHelper.ClearPlaceHolder(this);
				ManagerReference = new WeakReference(Manager);
			}
		}
		protected WeakReference ManagerReference { get; private set; }
		[
#if !SL
	DevExpressXpfDockingLocalizedDescription("BaseLayoutItemToolTip"),
#endif
		XtraSerializableProperty]
#if SILVERLIGHT
		public object ToolTip {
#else
		public new object ToolTip {
#endif
			get { return (object)GetValue(ToolTipProperty); }
			set { SetValue(ToolTipProperty, value); }
		}
		[
			XtraSerializableProperty, Category("Content")]
		public new Brush Background {
			get { return (Brush)GetValue(BackgroundProperty); }
			set { SetValue(BackgroundProperty, value); }
		}
		[ XtraSerializableProperty, Category("Layout")]
		public GridLength ItemWidth {
			get { return (GridLength)GetValue(ItemWidthProperty); }
			set { SetValue(ItemWidthProperty, value); }
		}
		[ XtraSerializableProperty, Category("Layout")]
		public GridLength ItemHeight {
			get { return (GridLength)GetValue(ItemHeightProperty); }
			set { SetValue(ItemHeightProperty, value); }
		}
		[
#if !SL
	DevExpressXpfDockingLocalizedDescription("BaseLayoutItemFloatSize"),
#endif
 XtraSerializableProperty, Category("Layout")]
		public Size FloatSize {
			get { return (Size)GetValue(FloatSizeProperty); }
			set { SetValue(FloatSizeProperty, value); }
		}
#if !SL
	[DevExpressXpfDockingLocalizedDescription("BaseLayoutItemLayoutSize")]
#endif
		public Size LayoutSize {
			get { return (Size)GetValue(LayoutSizeProperty); }
			internal set { SetValue(LayoutSizePropertyKey, value); }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public Size ActualMinSize {
			get { return (Size)GetValue(ActualMinSizeProperty); }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public Size ActualMaxSize {
			get { return (Size)GetValue(ActualMaxSizeProperty); }
		}
		[
#if !SL
	DevExpressXpfDockingLocalizedDescription("BaseLayoutItemCaption"),
#endif
		XtraSerializableProperty(1), Category("Caption")]
		[TypeConverter(typeof(StringConverter))]
		public object Caption {
			get { return GetValue(CaptionProperty); }
			set { SetValue(CaptionProperty, value); }
		}
		[
#if !SL
	DevExpressXpfDockingLocalizedDescription("BaseLayoutItemDescription"),
#endif
 XtraSerializableProperty]
		public string Description {
			get { return (string)GetValue(DescriptionProperty); }
			set { SetValue(DescriptionProperty, value); }
		}
		[
#if !SL
	DevExpressXpfDockingLocalizedDescription("BaseLayoutItemFooterDescription"),
#endif
 XtraSerializableProperty]
		public string FooterDescription {
			get { return (string)GetValue(FooterDescriptionProperty); }
			set { SetValue(FooterDescriptionProperty, value); }
		}
		[
#if !SL
	DevExpressXpfDockingLocalizedDescription("BaseLayoutItemCaptionFormat"),
#endif
		XtraSerializableProperty, Category("Caption")]
		public string CaptionFormat {
			get { return (string)GetValue(CaptionFormatProperty); }
			set { SetValue(CaptionFormatProperty, value); }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Advanced), Category("Caption")]
		public string ActualCaption {
			get { return (string)GetValue(ActualCaptionProperty); }
		}
		[
#if !SL
	DevExpressXpfDockingLocalizedDescription("BaseLayoutItemCustomizationCaption"),
#endif
		XtraSerializableProperty, Category("Caption")]
		public string CustomizationCaption {
			get { return (string)GetValue(CustomizationCaptionProperty); }
			set { SetValue(CustomizationCaptionProperty, value); }
		}
		[
#if !SL
	DevExpressXpfDockingLocalizedDescription("BaseLayoutItemCaptionLocation"),
#endif
		XtraSerializableProperty, Category("Caption")]
		public CaptionLocation CaptionLocation {
			get { return (CaptionLocation)GetValue(CaptionLocationProperty); }
			set { SetValue(CaptionLocationProperty, value); }
		}
		[
#if !SL
	DevExpressXpfDockingLocalizedDescription("BaseLayoutItemCaptionAlignMode"),
#endif
		XtraSerializableProperty, Category("Caption")]
		public CaptionAlignMode CaptionAlignMode {
			get { return (CaptionAlignMode)GetValue(CaptionAlignModeProperty); }
			set { SetValue(CaptionAlignModeProperty, value); }
		}
		[
#if !SL
	DevExpressXpfDockingLocalizedDescription("BaseLayoutItemCaptionWidth"),
#endif
		XtraSerializableProperty, Category("Caption")]
		public double CaptionWidth {
			get { return (double)GetValue(CaptionWidthProperty); }
			set { SetValue(CaptionWidthProperty, value); }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public double DesiredCaptionWidth {
			get { return (double)GetValue(DesiredCaptionWidthProperty); }
			internal set { SetValue(DesiredCaptionWidthPropertyKey, value); }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public bool HasDesiredCaptionWidth {
			get { return (bool)GetValue(HasDesiredCaptionWidthProperty); }
		}
		[
#if !SL
	DevExpressXpfDockingLocalizedDescription("BaseLayoutItemActualCaptionWidth"),
#endif
 Category("Caption")]
		public double ActualCaptionWidth {
			get { return (double)GetValue(ActualCaptionWidthProperty); }
			internal set { SetValue(ActualCaptionWidthPropertyKey, value); }
		}
		[
#if !SL
	DevExpressXpfDockingLocalizedDescription("BaseLayoutItemCaptionHorizontalAlignment"),
#endif
 XtraSerializableProperty, Category("Caption")]
		public HorizontalAlignment CaptionHorizontalAlignment {
			get { return (HorizontalAlignment)GetValue(CaptionHorizontalAlignmentProperty); }
			set { SetValue(CaptionHorizontalAlignmentProperty, value); }
		}
		[
#if !SL
	DevExpressXpfDockingLocalizedDescription("BaseLayoutItemCaptionVerticalAlignment"),
#endif
 XtraSerializableProperty, Category("Caption")]
		public VerticalAlignment CaptionVerticalAlignment {
			get { return (VerticalAlignment)GetValue(CaptionVerticalAlignmentProperty); }
			set { SetValue(CaptionVerticalAlignmentProperty, value); }
		}
		[
#if !SL
	DevExpressXpfDockingLocalizedDescription("BaseLayoutItemCaptionImageLocation"),
#endif
		XtraSerializableProperty, Category("Caption")]
		public ImageLocation CaptionImageLocation {
			get { return (ImageLocation)GetValue(CaptionImageLocationProperty); }
			set { SetValue(CaptionImageLocationProperty, value); }
		}
		[
#if !SL
	DevExpressXpfDockingLocalizedDescription("BaseLayoutItemShowCaptionImage"),
#endif
		XtraSerializableProperty, Category("Caption")]
		public bool ShowCaptionImage {
			get { return (bool)GetValue(ShowCaptionImageProperty); }
			set { SetValue(ShowCaptionImageProperty, value); }
		}
		[
#if !SL
	DevExpressXpfDockingLocalizedDescription("BaseLayoutItemImageToTextDistance"),
#endif
		XtraSerializableProperty, Category("Caption")]
		public double ImageToTextDistance {
			get { return (double)GetValue(ImageToTextDistanceProperty); }
			set { SetValue(ImageToTextDistanceProperty, value); }
		}
#if !SL
	[DevExpressXpfDockingLocalizedDescription("BaseLayoutItemIsCaptionVisible")]
#endif
		public bool IsCaptionVisible {
			get { return (bool)GetValue(IsCaptionVisibleProperty); }
		}
#if !SL
	[DevExpressXpfDockingLocalizedDescription("BaseLayoutItemIsCaptionImageVisible")]
#endif
		public bool IsCaptionImageVisible {
			get { return (bool)GetValue(IsCaptionImageVisibleProperty); }
		}
#if !SL
	[DevExpressXpfDockingLocalizedDescription("BaseLayoutItemIsControlBoxVisible")]
#endif
		public bool IsControlBoxVisible {
			get { return (bool)GetValue(IsControlBoxVisibleProperty); }
		}
		[
#if !SL
	DevExpressXpfDockingLocalizedDescription("BaseLayoutItemShowCaption"),
#endif
 XtraSerializableProperty, Category("Caption")]
		public bool ShowCaption {
			get { return (bool)GetValue(ShowCaptionProperty); }
			set { SetValue(ShowCaptionProperty, value); }
		}
		[
#if !SL
	DevExpressXpfDockingLocalizedDescription("BaseLayoutItemShowControlBox"),
#endif
 XtraSerializableProperty, Category("Layout")]
		public bool ShowControlBox {
			get { return (bool)GetValue(ShowControlBoxProperty); }
			set { SetValue(ShowControlBoxProperty, value); }
		}
		[
#if !SL
	DevExpressXpfDockingLocalizedDescription("BaseLayoutItemCaptionImage"),
#endif
 Category("Caption")]
		public ImageSource CaptionImage {
			get { return (ImageSource)GetValue(CaptionImageProperty); }
			set { SetValue(CaptionImageProperty, value); }
		}
		[
#if !SL
	DevExpressXpfDockingLocalizedDescription("BaseLayoutItemHeaderBarContainerControlName"),
#endif
		XtraSerializableProperty]
		public string HeaderBarContainerControlName {
			get { return (string)GetValue(HeaderBarContainerControlNameProperty); }
			set { SetValue(HeaderBarContainerControlNameProperty, value); }
		}
		[
#if !SL
	DevExpressXpfDockingLocalizedDescription("BaseLayoutItemHeaderBarContainerControlAllowDrop"),
#endif
		XtraSerializableProperty]
		public bool? HeaderBarContainerControlAllowDrop {
			get { return (bool?)GetValue(HeaderBarContainerControlAllowDropProperty); }
			set { SetValue(HeaderBarContainerControlAllowDropProperty, value); }
		}
		[
#if !SL
	DevExpressXpfDockingLocalizedDescription("BaseLayoutItemMargin"),
#endif
		XtraSerializableProperty, Category("Layout")]
		public new Thickness Margin {
			get { return (Thickness)GetValue(MarginProperty); }
			set { SetValue(MarginProperty, value); }
		}
#if !SL
	[DevExpressXpfDockingLocalizedDescription("BaseLayoutItemActualMargin")]
#endif
		public Thickness ActualMargin {
			get { return (Thickness)GetValue(ActualMarginProperty); }
			private set { SetValue(ActualMarginPropertyKey, value); }
		}
		[
#if !SL
	DevExpressXpfDockingLocalizedDescription("BaseLayoutItemPadding"),
#endif
		XtraSerializableProperty, Category("Layout")]
		public new Thickness Padding {
			get { return (Thickness)GetValue(PaddingProperty); }
			set { SetValue(PaddingProperty, value); }
		}
#if !SL
	[DevExpressXpfDockingLocalizedDescription("BaseLayoutItemActualPadding")]
#endif
		public Thickness ActualPadding {
			get { return (Thickness)GetValue(ActualPaddingProperty); }
			private set { SetValue(ActualPaddingPropertyKey, value); }
		}
#if !SL
	[DevExpressXpfDockingLocalizedDescription("BaseLayoutItemIsSelected")]
#endif
		public bool IsSelected {
			get { return (bool)GetValue(IsSelectedProperty); }
			private set { SetValue(IsSelectedPropertyKey, value); }
		}
#if !SL
	[DevExpressXpfDockingLocalizedDescription("BaseLayoutItemIsActive")]
#endif
		[XtraSerializableProperty]
		public bool IsActive {
			get { return (bool)GetValue(IsActiveProperty); }
			set { SetValue(IsActiveProperty, value); }
		}
#if !SL
	[DevExpressXpfDockingLocalizedDescription("BaseLayoutItemIsHidden")]
#endif
		public bool IsHidden {
			get { return (bool)GetValue(IsHiddenProperty); }
			private set { SetValue(IsHiddenPropertyKey, value); }
		}
#if !SL
	[DevExpressXpfDockingLocalizedDescription("BaseLayoutItemIsClosed")]
#endif
		public bool IsClosed {
			get { return (bool)GetValue(IsClosedProperty); }
			private set { SetValue(IsClosedPropertyKey, value); }
		}
#if !SL
	[DevExpressXpfDockingLocalizedDescription("BaseLayoutItemClosed")]
#endif
		public bool Closed {
			get { return (bool)GetValue(ClosedProperty); }
			set { SetValue(ClosedProperty, value); }
		}
#if !SL
	[DevExpressXpfDockingLocalizedDescription("BaseLayoutItemHasImage")]
#endif
		public bool HasImage {
			get { return (bool)GetValue(HasImageProperty); }
			private set { SetValue(HasImagePropertyKey, value); }
		}
#if !SL
	[DevExpressXpfDockingLocalizedDescription("BaseLayoutItemHasCaption")]
#endif
		public bool HasCaption {
			get { return (bool)GetValue(HasCaptionProperty); }
			private set { SetValue(HasCaptionPropertyKey, value); }
		}
#if !SL
	[DevExpressXpfDockingLocalizedDescription("BaseLayoutItemHasCaptionTemplate")]
#endif
		public bool HasCaptionTemplate {
			get { return (bool)GetValue(HasCaptionTemplateProperty); }
			private set { SetValue(HasCaptionTemplatePropertyKey, value); }
		}
#if !SL
	[DevExpressXpfDockingLocalizedDescription("BaseLayoutItemIsTabPage")]
#endif
		public bool IsTabPage {
			get { return (bool)GetValue(IsTabPageProperty); }
			private set { SetValue(IsTabPagePropertyKey, value); }
		}
#if !SL
	[DevExpressXpfDockingLocalizedDescription("BaseLayoutItemCaptionTemplate")]
#endif
		public DataTemplate CaptionTemplate {
			get { return (DataTemplate)GetValue(CaptionTemplateProperty); }
			set { SetValue(CaptionTemplateProperty, value); }
		}
#if !SILVERLIGHT
		public DataTemplateSelector CaptionTemplateSelector {
			get { return (DataTemplateSelector)GetValue(CaptionTemplateSelectorProperty); }
			set { SetValue(CaptionTemplateSelectorProperty, value); }
		}
#endif
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public bool IsControlItemsHost {
			get { return (bool)GetValue(IsControlItemsHostProperty); }
			private set { SetValue(IsControlItemsHostPropertyKey, value); }
		}
#if !SL
	[DevExpressXpfDockingLocalizedDescription("BaseLayoutItemIsSelectedItem")]
#endif
		public bool IsSelectedItem {
			get { return (bool)GetValue(IsSelectedItemProperty); }
			private set { SetValue(IsSelectedItemPropertyKey, value); }
		}
		bool IsActivationLocked { get { return lockActivation > 0; } }
		int lockActivation = 0;
		protected internal void SetActive(bool value) {
			lockActivation++;
			if(IsActive != value)
				IsActive = value;
			lockActivation--;
		}
		protected internal virtual void SetAutoHidden(bool autoHidden) { }
		protected internal void SetClosed(bool value) { IsClosed = value; Closed = value; }
		protected internal virtual void SetHidden(bool value, LayoutGroup customizationRoot) {
			IsHidden = value;
			parentCore = customizationRoot;
		}
		protected internal void SetSelected(bool value) {
			IsSelected = value;
		}
		protected internal virtual void SetSelected(DockLayoutManager manager, bool value) {
			bool detached = (Manager == null);
			if(detached) Manager = manager;
			IsSelected = value;
			if(detached) Manager = null;
		}
		[
#if !SL
	DevExpressXpfDockingLocalizedDescription("BaseLayoutItemAllowActivate"),
#endif
		XtraSerializableProperty, Category("Behavior")]
		public bool AllowActivate {
			get { return (bool)GetValue(AllowActivateProperty); }
			set { SetValue(AllowActivateProperty, value); }
		}
		[
#if !SL
	DevExpressXpfDockingLocalizedDescription("BaseLayoutItemAllowSelection"),
#endif
		XtraSerializableProperty, Category("Behavior")]
		public bool AllowSelection {
			get { return (bool)GetValue(AllowSelectionProperty); }
			set { SetValue(AllowSelectionProperty, value); }
		}
		[
#if !SL
	DevExpressXpfDockingLocalizedDescription("BaseLayoutItemAllowDrag"),
#endif
		XtraSerializableProperty, Category("Behavior")]
		public bool AllowDrag {
			get { return (bool)GetValue(AllowDragProperty); }
			set { SetValue(AllowDragProperty, value); }
		}
		[
#if !SL
	DevExpressXpfDockingLocalizedDescription("BaseLayoutItemAllowFloat"),
#endif
		XtraSerializableProperty, Category("Behavior")]
		public bool AllowFloat {
			get { return (bool)GetValue(AllowFloatProperty); }
			set { SetValue(AllowFloatProperty, value); }
		}
		[
#if !SL
	DevExpressXpfDockingLocalizedDescription("BaseLayoutItemAllowDock"),
#endif
		XtraSerializableProperty, Category("Behavior")]
		public bool AllowDock {
			get { return (bool)GetValue(AllowDockProperty); }
			set { SetValue(AllowDockProperty, value); }
		}
		[
#if !SL
	DevExpressXpfDockingLocalizedDescription("BaseLayoutItemAllowHide"),
#endif
		XtraSerializableProperty, Category("Behavior")]
		public bool AllowHide {
			get { return (bool)GetValue(AllowHideProperty); }
			set { SetValue(AllowHideProperty, value); }
		}
		[
#if !SL
	DevExpressXpfDockingLocalizedDescription("BaseLayoutItemAllowClose"),
#endif
		XtraSerializableProperty, Category("Behavior")]
		public bool AllowClose {
			get { return (bool)GetValue(AllowCloseProperty); }
			set { SetValue(AllowCloseProperty, value); }
		}
		[
#if !SL
	DevExpressXpfDockingLocalizedDescription("BaseLayoutItemAllowRestore"),
#endif
		XtraSerializableProperty, Category("Behavior")]
		public bool AllowRestore {
			get { return (bool)GetValue(AllowRestoreProperty); }
			set { SetValue(AllowRestoreProperty, value); }
		}
		[
#if !SL
	DevExpressXpfDockingLocalizedDescription("BaseLayoutItemAllowMove"),
#endif
		XtraSerializableProperty, Category("Behavior")]
		public bool AllowMove {
			get { return (bool)GetValue(AllowMoveProperty); }
			set { SetValue(AllowMoveProperty, value); }
		}
		[
#if !SL
	DevExpressXpfDockingLocalizedDescription("BaseLayoutItemAllowRename"),
#endif
		XtraSerializableProperty, Category("Behavior")]
		public bool AllowRename {
			get { return (bool)GetValue(AllowRenameProperty); }
			set { SetValue(AllowRenameProperty, value); }
		}
		[
#if !SL
	DevExpressXpfDockingLocalizedDescription("BaseLayoutItemAllowContextMenu"),
#endif
		XtraSerializableProperty, Category("Behavior")]
		public bool AllowContextMenu {
			get { return (bool)GetValue(AllowContextMenuProperty); }
			set { SetValue(AllowContextMenuProperty, value); }
		}
		[
#if !SL
	DevExpressXpfDockingLocalizedDescription("BaseLayoutItemAllowMaximize"),
#endif
		XtraSerializableProperty, Category("Behavior")]
		public bool AllowMaximize {
			get { return (bool)GetValue(AllowMaximizeProperty); }
			set { SetValue(AllowMaximizeProperty, value); }
		}
		[
#if !SL
	DevExpressXpfDockingLocalizedDescription("BaseLayoutItemAllowMinimize"),
#endif
		XtraSerializableProperty, Category("Behavior")]
		public bool AllowMinimize {
			get { return (bool)GetValue(AllowMinimizeProperty); }
			set { SetValue(AllowMinimizeProperty, value); }
		}
		[
#if !SL
	DevExpressXpfDockingLocalizedDescription("BaseLayoutItemAllowSizing"),
#endif
		XtraSerializableProperty, Category("Behavior")]
		public bool AllowSizing {
			get { return (bool)GetValue(AllowSizingProperty); }
			set { SetValue(AllowSizingProperty, value); }
		}
		public bool AllowDockToCurrentItem {
			get { return (bool)GetValue(AllowDockToCurrentItemProperty); }
			set { SetValue(AllowDockToCurrentItemProperty, value); }
		}
		[
#if !SL
	DevExpressXpfDockingLocalizedDescription("BaseLayoutItemFloatOnDoubleClick"),
#endif
		XtraSerializableProperty, Category("Behavior")]
		public bool FloatOnDoubleClick {
			get { return (bool)GetValue(FloatOnDoubleClickProperty); }
			set { SetValue(FloatOnDoubleClickProperty, value); }
		}
		#region ISupportVisitor<BaseLayoutItem> Members
		public virtual void Accept(VisitDelegate<BaseLayoutItem> visit) {
			visit(this);
		}
		public virtual void Accept(IVisitor<BaseLayoutItem> visitor) {
			visitor.Visit(this);
		}
		#endregion
		LayoutGroup parentCore = null;
#if !SL
	[DevExpressXpfDockingLocalizedDescription("BaseLayoutItemItemType")]
#endif
		public LayoutItemType ItemType {
			get { return GetLayoutItemTypeCore(); }
		}
#if !SL
	[DevExpressXpfDockingLocalizedDescription("BaseLayoutItemParent")]
#endif
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new LayoutGroup Parent {
			get { return parentCore; }
			internal set {
				if(parentCore == value) return;
				var oldParent = parentCore;
				if(parentCore != null && parentCore.Items.Contains(this)) parentCore.Items.Remove(this);
				this.parentCore = value;
				if(parentCore != null && !parentCore.Items.Contains(this)) parentCore.Items.Add(this);
				OnParentChanged(oldParent, parentCore);
			}
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public DependencyObject LogicalParent { get { return base.Parent; } }
		protected internal bool IsTabDocument { get { return IsTabPage && Parent is DocumentGroup; } }
		protected virtual void OnParentChanged(LayoutGroup oldParent, LayoutGroup newParent) {
			OnParentChanged();
		}
		protected virtual void OnParentChanged() {
			InvokeCoerceDataContext();
			CoerceValue(CaptionAlignModeProperty);
			CoerceValue(CaptionWidthProperty);
			CoerceValue(IsTabPageProperty);
			CoerceValue(IsSelectedItemProperty);
			CoerceValue(TabCaptionWidthProperty);
			UpdateIsFloatingRootItem();
			CoerceValue(IsVisibleProperty);
			CoerceValue(ActualAppearanceProperty);
			CheckClosedState();
			CoerceValue(CaptionImageProperty);
		}
		protected internal virtual void OnParentLoaded() { }
		protected internal virtual void OnParentUnloaded() { }
		protected internal virtual void OnParentItemsChanged() {
			CoerceValue(CaptionAlignModeProperty);
			CoerceValue(CaptionWidthProperty);
			CoerceValue(TabCaptionWidthProperty);
			UpdateIsFloatingRootItem();
		}
#if !SILVERLIGHT
		internal void BeginLayoutChange() {
			SetIsLayoutChangeInProgress(this, true);
		}
		internal void EndLayoutChange() {
			ClearValue(IsLayoutChangeInProgressProperty);
		}
#endif
		BaseLayoutItem ISupportHierarchy<BaseLayoutItem>.Parent {
			get { return parentCore; }
		}
		BaseLayoutItem[] ISupportHierarchy<BaseLayoutItem>.Nodes {
			get { return GetNodesCore(); }
		}
		protected static BaseLayoutItem[] EmptyNodes = new BaseLayoutItem[0];
		protected virtual BaseLayoutItem[] GetNodesCore() {
			return EmptyNodes;
		}
		protected abstract LayoutItemType GetLayoutItemTypeCore();
		protected DockSituation DockSituation { get; set; }
		protected internal void UpdateDockSituation(BaseLayoutItem dockTarget, DockType type) {
			if(DockSituation == null)
				DockSituation = new DockSituation(type, dockTarget);
			else {
				DockSituation.Type = type;
				DockSituation.DockTarget = dockTarget;
			}
		}
		protected internal void UpdateDockSituation(DockSituation situation, BaseLayoutItem dockTarget, BaseLayoutItem originalItem = null) {
			if(DockSituation == null || situation == null)
				DockSituation = situation;
			else {
				DockSituation.Type = situation.Type;
				if(!DockSituation.Width.IsAbsolute || situation.Width.IsAbsolute)
					DockSituation.Width = situation.Width;
				if(!DockSituation.Height.IsAbsolute || situation.Height.IsAbsolute)
					DockSituation.Height = situation.Height;
			}
			if(DockSituation != null) {
				DockSituation.DockTarget = dockTarget;
				DockSituation.OriginalItem = originalItem;
			}
		}
		protected internal void UpdateAutoHideSituation(AutoHideType type) {
			DockSituation.AutoHideType = type;
		}
		protected internal void UpdateAutoHideSituation() {
			UpdateAutoHideSituation(AutoHideType.Default);
		}
		protected internal DockSituation GetLastDockSituation() { return DockSituation; }
		protected internal Size CheckSize(Size layoutSize) {
			ValueSource valueSource = DependencyPropertyHelper.GetValueSource(this, FloatSizeProperty);
			return (valueSource.BaseValueSource != BaseValueSource.Default) ? FloatSize : layoutSize;
		}
		protected virtual bool GetIsAutoHidden() {
			return Parent is AutoHideGroup;
		}
		Locker CloseCommandLocker = new Locker();
		internal IDisposable LockCloseCommand() {
			return CloseCommandLocker.Lock();
		}
		internal bool ExecuteCloseCommand() {
			if(CloseCommandLocker) return false;
			using(CloseCommandLocker.Lock()) {
				ICommand command = CloseCommand;
				RoutedCommand routedCommand = command as RoutedCommand;
				if(command != null) {
					if(routedCommand != null) {
						if(routedCommand.CanExecute(this, this)) {
							routedCommand.Execute(this, this);
							return true;
						}
					}
					if(command.CanExecute(this)) {
						command.Execute(this);
						return true;
					}
				}
				return false;
			}
		}
#if !SL
	[DevExpressXpfDockingLocalizedDescription("BaseLayoutItemIsAutoHidden")]
#endif
		public bool IsAutoHidden {
			get { return GetIsAutoHidden(); }
		}
#if !SL
	[DevExpressXpfDockingLocalizedDescription("BaseLayoutItemIsFloating")]
#endif
		public bool IsFloating {
			get { return this.GetRoot() is FloatGroup; }
		}
		[
#if !SL
	DevExpressXpfDockingLocalizedDescription("BaseLayoutItemVisibility"),
#endif
		XtraSerializableProperty, Category("Visibility")]
		public new Visibility Visibility {
			get { return (Visibility)GetValue(VisibilityProperty); }
			set { SetValue(VisibilityProperty, value); }
		}
#if !SL
	[DevExpressXpfDockingLocalizedDescription("BaseLayoutItemIsVisible")]
#endif
#if SILVERLIGHT
		public bool IsVisible {
#else
		public new bool IsVisible {
#endif
			get { return (bool)GetValue(IsVisibleProperty); }
		}
		[
#if !SL
	DevExpressXpfDockingLocalizedDescription("BaseLayoutItemTabCaptionWidth"),
#endif
 Category("TabHeader")]
		public double TabCaptionWidth {
			get { return (double)GetValue(TabCaptionWidthProperty); }
			set { SetValue(TabCaptionWidthProperty, value); }
		}
		[
#if !SL
	DevExpressXpfDockingLocalizedDescription("BaseLayoutItemTextWrapping"),
#endif
		XtraSerializableProperty, Category("Caption")]
		public TextWrapping TextWrapping {
			get { return (TextWrapping)GetValue(TextWrappingProperty); }
			set { SetValue(TextWrappingProperty, value); }
		}
		[
#if !SL
	DevExpressXpfDockingLocalizedDescription("BaseLayoutItemTextTrimming"),
#endif
		XtraSerializableProperty, Category("Caption")]
		public TextTrimming TextTrimming {
			get { return (TextTrimming)GetValue(TextTrimmingProperty); }
			set { SetValue(TextTrimmingProperty, value); }
		}
		[
#if !SL
	DevExpressXpfDockingLocalizedDescription("BaseLayoutItemAppearance"),
#endif
		Category("Caption")]
		public Appearance Appearance {
			get { return (Appearance)GetValue(AppearanceProperty); }
			set { SetValue(AppearanceProperty, value); }
		}
		internal Appearance ActualAppearance {
			get { return (Appearance)GetValue(ActualAppearanceProperty); }
			private set { SetValue(ActualAppearancePropertyKey, value); }
		}
		internal AppearanceObject ActualAppearanceObject {
			get { return (AppearanceObject)GetValue(ActualAppearanceObjectProperty); }
			private set { SetValue(ActualAppearanceObjectPropertyKey, value); }
		}
		[
#if !SL
	DevExpressXpfDockingLocalizedDescription("BaseLayoutItemTabCaption"),
#endif
 Category("TabHeader"),
		XtraSerializableProperty(1)]
		public object TabCaption {
			get { return ((object)GetValue(TabCaptionProperty)); }
			set { SetValue(TabCaptionProperty, value); }
		}
		[
#if !SL
	DevExpressXpfDockingLocalizedDescription("BaseLayoutItemTabCaptionFormat"),
#endif
 Category("TabHeader")]
		public string TabCaptionFormat {
			get { return (string)GetValue(TabCaptionFormatProperty); }
			set { SetValue(TabCaptionFormatProperty, value); }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Advanced), Category("TabHeader")]
		public string ActualTabCaption {
			get { return (string)GetValue(ActualTabCaptionProperty); }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Advanced), Category("TabHeader")]
		public bool HasTabCaption {
			get { return (bool)GetValue(HasTabCaptionProperty); }
		}
		[
#if !SL
	DevExpressXpfDockingLocalizedDescription("BaseLayoutItemClosingBehavior"),
#endif
 Category("Behavior")]
		public ClosingBehavior ClosingBehavior {
			get { return (ClosingBehavior)GetValue(ClosingBehaviorProperty); }
			set { SetValue(ClosingBehaviorProperty, value); }
		}
		[
#if !SL
	DevExpressXpfDockingLocalizedDescription("BaseLayoutItemCloseCommand"),
#endif
 Category("Behavior")]
		public ICommand CloseCommand {
			get { return (ICommand)GetValue(CloseCommandProperty); }
			set { SetValue(CloseCommandProperty, value); }
		}
		public string BindableName {
			get { return (string)GetValue(BindableNameProperty); }
			set { SetValue(BindableNameProperty, value); }
		}
		protected internal virtual bool IsMaximizable { get { return false; } }
		internal bool IsLayoutChangeInProgress {
			get {
#if !SILVERLIGHT
				return GetIsLayoutChangeInProgress(this);
#else
				return false;
#endif
			}
		}
		protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo) {
			base.OnRenderSizeChanged(sizeInfo);
			OnRenderSizeChangedCore(sizeInfo);
			ResizeLockHelper.Unlock();
		}
		protected virtual void OnRenderSizeChangedCore(SizeChangedInfo sizeInfo) { }
		[XtraSerializableProperty(XtraSerializationVisibility.Collection, true, false, false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public PlaceHolderCollection SerializableDockSituation {
			get { return DockInfo.AsCollection(); }
			set { }
		}
#if SILVERLIGHT
		public new double MinWidth {
			get { return (double)GetValue(MinWidthProperty); }
			set { SetValue(MinWidthProperty, value); }
		}
		public new double MinHeight {
			get { return (double)GetValue(MinHeightProperty); }
			set { SetValue(MinHeightProperty, value); }
		}
		public new double MaxWidth {
			get { return (double)GetValue(MaxWidthProperty); }
			set { SetValue(MaxWidthProperty, value); }
		}
		public new double MaxHeight {
			get { return (double)GetValue(MaxHeightProperty); }
			set { SetValue(MaxHeightProperty, value); }
		}
#endif
		#region ISerializableItem
		[XtraSerializableProperty, Core.Serialization.XtraResetProperty(Core.Serialization.ResetPropertyMode.None),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public string TypeName {
			get { return GetType().Name; }
#if SILVERLIGHT
			set { }
#endif
		}
		[XtraSerializableProperty, DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public string ParentName { get; set; }
		[XtraSerializableProperty, DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public string ParentCollectionName { get; set; }
		#endregion ISerializableItem
		internal string BarItemName { get; set; }
		internal bool isDeserializing;
		internal WeakReference AttachedSerializationController;
		internal bool IsClosing;
		#region ControlBoxButtons
		[
#if !SL
	DevExpressXpfDockingLocalizedDescription("BaseLayoutItemControlBoxContent"),
#endif
 Category("Content")]
		public object ControlBoxContent {
			get { return GetValue(ControlBoxContentProperty); }
			set { SetValue(ControlBoxContentProperty, value); }
		}
		[
#if !SL
	DevExpressXpfDockingLocalizedDescription("BaseLayoutItemControlBoxContentTemplate"),
#endif
 Category("Content")]
		public DataTemplate ControlBoxContentTemplate {
			get { return (DataTemplate)GetValue(ControlBoxContentTemplateProperty); }
			set { SetValue(ControlBoxContentTemplateProperty, value); }
		}
#if !SL
	[DevExpressXpfDockingLocalizedDescription("BaseLayoutItemIsCloseButtonVisible")]
#endif
		public bool IsCloseButtonVisible {
			get { return (bool)GetValue(IsCloseButtonVisibleProperty); }
		}
		[
#if !SL
	DevExpressXpfDockingLocalizedDescription("BaseLayoutItemShowCloseButton"),
#endif
 XtraSerializableProperty, Category("Layout")]
		public bool ShowCloseButton {
			get { return (bool)GetValue(ShowCloseButtonProperty); }
			set { SetValue(ShowCloseButtonProperty, value); }
		}
		protected virtual void OnShowCloseButtonChanged(bool show) {
			CoerceValue(IsCloseButtonVisibleProperty);
		}
		protected virtual void OnShowPinButtonChanged(bool show) {
			CoerceValue(IsPinButtonVisibleProperty);
		}
		protected virtual void OnShowMinimizeButtonChanged(bool show) {
			CoerceValue(IsMinimizeButtonVisibleProperty);
		}
		protected virtual void OnShowMaximizeButtonChanged(bool show) {
			CoerceValue(IsMaximizeButtonVisibleProperty);
		}
		protected virtual void OnShowRestoreButtonChanged(bool show) {
			CoerceValue(IsRestoreButtonVisibleProperty);
		}
		protected virtual void OnShowDropDownButtonChanged(bool show) {
			CoerceValue(IsDropDownButtonVisibleProperty);
		}
		protected virtual void OnShowScrollPrevButtonChanged(bool show) {
			CoerceValue(IsScrollPrevButtonVisibleProperty);
		}
		protected virtual void OnShowScrollNextButtonChanged(bool show) {
			CoerceValue(IsScrollNextButtonVisibleProperty);
		}
		protected virtual bool CoerceIsControlBoxVisible(bool visible) {
			return ShowControlBox;
		}
		protected virtual bool CoerceIsCloseButtonVisible(bool visible) {
			return AllowClose && ShowCloseButton;
		}
		protected virtual bool CoerceIsPinButtonVisible(bool visible) { return false; }
		protected virtual bool CoerceIsMinimizeButtonVisible(bool visible) { return false; }
		protected virtual bool CoerceIsMaximizeButtonVisible(bool visible) { return false; }
		protected virtual bool CoerceIsRestoreButtonVisible(bool visible) { return false; }
		protected virtual bool CoerceIsDropDownButtonVisible(bool visible) { return false; }
		protected virtual bool CoerceIsScrollPrevButtonVisible(bool visible) { return false; }
		protected virtual bool CoerceIsScrollNextButtonVisible(bool visible) { return false; }
		protected virtual void OnIsCloseButtonVisibleChanged(bool visible) {
			InvalidateTabHeader();
		}
		protected void InvalidateTabHeader() {
			var tabItem = UIElements.GetElement<VisualElements.TabbedPaneItem>();
			if(tabItem != null) VisualElements.BaseHeadersPanel.Invalidate(tabItem);
		}
		protected virtual void OnControlBoxContentChanged() {
			CoerceParentProperty(ControlBoxContentProperty);
		}
		protected virtual void OnControlBoxContentTemplateChanged() {
			CoerceParentProperty(ControlBoxContentTemplateProperty);
		}
		protected virtual void OnIsMaximizeButtonVisibleChanged(bool visible) { }
		protected virtual void OnIsRestoreButtonVisibleChanged(bool visible) { }
		#endregion ControlBoxButtons
		#region IUIElement
		IUIElement IUIElement.Scope { get { return DockLayoutManager.GetUIScope(this); } }
		UIChildren children = new UIChildren();
		UIChildren IUIElement.Children {
			get {
				if(children == null) uiChildren = new UIChildren();
				return children;
			}
		}
		#endregion IUIElement
		#region UIAutomation
		protected override System.Windows.Automation.Peers.AutomationPeer OnCreateAutomationPeer() {
			if(this.IsAutoHidden) return new UIAutomation.AutoHideItemAutomationPeer(this);
			if(this.IsClosed) return new UIAutomation.ClosedPanelItemAutomationPeer(this);
			if(IsTabPage) return new UIAutomation.TabbedItemAutomationPeer(this);
			return new UIAutomation.BaseLayoutItemAutomationPeer(this);
		}
		#endregion
		#region MinSize/MaxSize
#if DEBUGTEST
		[Obsolete("MinSize property is available only in the DEBUGTEST configuration")]
		public Size MinSize {
			get { return new Size(MinWidth, MinHeight); }
			set {
				MinHeight = value.Height;
				MinWidth = value.Width;
			}
		}
		[Obsolete("MaxSize property is available only in the DEBUGTEST configuration")]
		public Size MaxSize {
			get { return new Size(MaxWidth, MaxHeight); }
			set {
				MaxHeight = value.Height;
				MaxWidth = value.Width;
			}
		}
#endif
		#endregion
	}
	#region LockHelper
	internal class LockHelper : IDisposable {
		public delegate void LockHelperDelegate();
		LockHelperDelegate UnlockDelegate;
		int lockCount;
		public bool IsLocked { get { return lockCount > 0; } }
		public LockHelper(LockHelperDelegate unlockDelegate) {
			UnlockDelegate = unlockDelegate;
		}
		Stack<LockHelperDelegate> actions = new Stack<LockHelperDelegate>();
		public LockHelper() {
		}
		public void AddUnlockAction(LockHelperDelegate action) {
			if(!IsLocked || actions == null || actions.Contains(action)) return;
			actions.Push(action);
		}
		public LockHelper Lock() {
			lockCount++;
			return this;
		}
		public LockHelper LockOnce() {
			if(!IsLocked)
				Lock();
			return this;
		}
		public void Unlock() {
			if(!IsLocked) return;
			lockCount--;
			if(!IsLocked) {
				while(actions.Count > 0) {
					var action = actions.Pop();
					action();
				}
				if(UnlockDelegate != null) UnlockDelegate();
			}
		}
		public void DoWhenUnlocked(LockHelperDelegate action) {
			if(IsLocked) AddUnlockAction(action);
			else action();
		}
		public void Reset() {
			actions.Clear();
			UnlockDelegate = null;
			Unlock();
		}
		#region for using directive usage
		void IDisposable.Dispose() {
			Unlock();
		}
		#endregion
		#region implicit convert to bool
		public static implicit operator bool(LockHelper locker) {
			return locker.IsLocked;
		}
		#endregion
	}
	#endregion
}
