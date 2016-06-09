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
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using DevExpress.Utils;
using DevExpress.Xpf.Bars.Helpers;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Utils;
using DevExpress.Xpf.Utils.Themes;
using System.Windows.Media.Animation;
using System.Collections;
using DevExpress.Xpf.Bars.Automation;
using DevExpress.Xpf.Editors.Helpers;
using DevExpress.Mvvm.Native;
using System.Linq;
namespace DevExpress.Xpf.Bars {
	public enum GalleryPlacementTarget { Ribbon, DropDown, Standalone }
	[ContentProperty("Gallery")]
	[DXToolboxBrowsable]
	public class GalleryControl : ContentControl, INavigationOwner
		, IManipulationClient 
	{
		#region static
		public static readonly DependencyProperty GalleryProperty;
		public static readonly DependencyProperty PlacementTargetProperty;
		public static readonly DependencyProperty ActualItemGlyphMarginProperty;
		protected static readonly DependencyPropertyKey ActualItemGlyphMarginPropertyKey;
		public static readonly DependencyProperty ActualItemDescriptionMarginProperty;
		protected static readonly DependencyPropertyKey ActualItemDescriptionMarginPropertyKey;
		public static readonly DependencyProperty ActualItemCaptionMarginProperty;
		protected static readonly DependencyPropertyKey ActualItemCaptionMarginPropertyKey;
		public static readonly DependencyProperty ActualItemMarginProperty;
		protected static readonly DependencyPropertyKey ActualItemMarginPropertyKey;
		public static readonly DependencyProperty DefaultItemMarginProperty;
		public static readonly DependencyProperty DefaultItemDescriptionMarginProperty;
		public static readonly DependencyProperty DefaultItemGlyphMarginProperty;
		public static readonly DependencyProperty DefaultItemCaptionMarginProperty;
		public static readonly DependencyProperty DefaultGroupCaptionMarginProperty;
		public static readonly DependencyProperty ItemCaptionTextStyleProperty;
		public static readonly DependencyProperty ItemCaptionTextStyleSelectorProperty;
		public static readonly DependencyProperty ActualItemCaptionTextStyleProperty;
		protected static readonly DependencyPropertyKey ActualItemCaptionTextStylePropertyKey;
		public static readonly DependencyProperty ItemDescriptionTextStyleProperty;
		public static readonly DependencyProperty ItemDescriptionTextStyleSelectorProperty;
		public static readonly DependencyProperty ActualItemDescriptionTextStyleProperty;
		protected static readonly DependencyPropertyKey ActualItemDescriptionTextStylePropertyKey;
		public static readonly DependencyProperty GroupCaptionTextStyleProperty;
		public static readonly DependencyProperty ActualGroupCaptionTextStyleProperty;
		protected static readonly DependencyPropertyKey ActualGroupCaptionTextStylePropertyKey;
		public static readonly DependencyProperty NormalFilterCaptionTextStyleProperty;
		public static readonly DependencyProperty ActualNormalFilterCaptionTextStyleProperty;
		protected static readonly DependencyPropertyKey ActualNormalFilterCaptionTextStylePropertyKey;
		public static readonly DependencyProperty SelectedFilterCaptionTextStyleProperty;
		public static readonly DependencyProperty ActualSelectedFilterCaptionTextStyleProperty;
		protected static readonly DependencyPropertyKey ActualSelectedFilterCaptionTextStylePropertyKey;
		public static readonly DependencyProperty HintTextStyleProperty;
		public static readonly DependencyProperty ActualHintTextStyleProperty;
		protected static readonly DependencyPropertyKey ActualHintTextStylePropertyKey;
		public static readonly DependencyProperty HintCaptionTextStyleProperty;
		public static readonly DependencyProperty ActualIsGroupCaptionVisibleProperty;
		protected static readonly DependencyPropertyKey ActualIsGroupCaptionVisiblePropertyKey;
		public static readonly DependencyProperty ActualGroupCaptionMarginProperty;
		protected static readonly DependencyPropertyKey ActualGroupCaptionMarginPropertyKey;
		public static readonly DependencyProperty DefaultIsGroupCaptionVisibleProperty;
		public static readonly DependencyProperty DesiredRowCountProperty;
		public static readonly DependencyProperty DesiredColCountProperty;
		public static readonly DependencyProperty DefaultItemBorderTemplateProperty;		
		public static readonly DependencyProperty ActualItemBorderTemplateProperty;
		protected static readonly DependencyPropertyKey ActualItemBorderTemplatePropertyKey;
		public static readonly DependencyProperty DefaultItemGlyphBorderTemplateProperty;
		public static readonly DependencyProperty ActualItemGlyphBorderTemplateProperty;
		protected static readonly DependencyPropertyKey ActualItemGlyphBorderTemplatePropertyKey;
		public static readonly DependencyProperty DefaultGroupItemsMarginProperty;
		public static readonly DependencyProperty ActualGroupItemsMarginProperty;
		protected static readonly DependencyPropertyKey ActualGroupItemsMarginPropertyKey;
		public static readonly DependencyProperty DefaultItemControlTemplateProperty;		
		protected static readonly DependencyPropertyKey ActualItemControlTemplatePropertyKey;
		public static readonly DependencyProperty ActualItemControlTemplateProperty;
		public static readonly DependencyProperty DefaultItemGlyphBorderPaddingProperty;
		protected static readonly DependencyPropertyKey ActualItemGlyphBorderPaddingPropertyKey;
		public static readonly DependencyProperty ActualItemGlyphBorderPaddingProperty;
		public static readonly DependencyProperty ActualVerticalScrollBarVisibilityProperty;
		protected static readonly DependencyPropertyKey ActualVerticalScrollBarVisibilityPropertyKey;
		public static readonly DependencyProperty DefaultGroupCaptionControlTemplateProperty;		
		public static readonly DependencyProperty ActualGroupCaptionControlTemplateProperty;
		protected static readonly DependencyPropertyKey ActualGroupCaptionControlTemplatePropertyKey;
		public static readonly DependencyProperty DefaultFilterControlTemplateProperty;		
		public static readonly DependencyProperty ActualFilterControlTemplateProperty;
		protected static readonly DependencyPropertyKey ActualFilterControlTemplatePropertyKey;
		public static readonly DependencyProperty ActualVerticalOffsetProperty;
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public static GalleryControl GetGalleryControl(DependencyObject obj) { return (GalleryControl)obj.GetValue(GalleryControlProperty); }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public static void SetGalleryControl(DependencyObject obj, GalleryControl value) { obj.SetValue(GalleryControlProperty, value); }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public static readonly DependencyProperty GalleryControlProperty = DependencyPropertyManager.RegisterAttached("GalleryControl", typeof(GalleryControl), typeof(GalleryControl), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.Inherits));
		static GalleryControl() {
			DevExpress.Xpf.Bars.Automation.NavigationAutomationPeersCreator.Default.RegisterObject(typeof(GalleryControl), typeof(GalleryControlAutomationPeer), owner => new GalleryControlAutomationPeer((GalleryControl)owner));
			DefaultStyleKeyProperty.OverrideMetadata(typeof(GalleryControl), new FrameworkPropertyMetadata(typeof(GalleryControl)));
			GalleryProperty = DependencyPropertyManager.Register("Gallery", typeof(Gallery), typeof(GalleryControl), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnGalleryPropertyChanged)));
			PlacementTargetProperty = DependencyPropertyManager.Register("PlacementTarget", typeof(GalleryPlacementTarget), typeof(GalleryControl), new FrameworkPropertyMetadata(GalleryPlacementTarget.Standalone));
			ActualItemGlyphMarginPropertyKey = DependencyPropertyManager.RegisterReadOnly("ActualItemGlyphMargin", typeof(Thickness), typeof(GalleryControl), new FrameworkPropertyMetadata(new Thickness()));
			ActualItemGlyphMarginProperty = ActualItemGlyphMarginPropertyKey.DependencyProperty;
			ActualItemDescriptionMarginPropertyKey = DependencyPropertyManager.RegisterReadOnly("ActualItemDescriptionMargin", typeof(Thickness), typeof(GalleryControl), new FrameworkPropertyMetadata(new Thickness()));
			ActualItemDescriptionMarginProperty = ActualItemDescriptionMarginPropertyKey.DependencyProperty;
			ActualItemCaptionMarginPropertyKey = DependencyPropertyManager.RegisterReadOnly("ActualItemCaptionMargin", typeof(Thickness), typeof(GalleryControl), new FrameworkPropertyMetadata(new Thickness()));
			ActualItemCaptionMarginProperty = ActualItemCaptionMarginPropertyKey.DependencyProperty;
			ActualItemMarginPropertyKey = DependencyPropertyManager.RegisterReadOnly("ActualItemMargin", typeof(Thickness), typeof(GalleryControl), new FrameworkPropertyMetadata(new Thickness()));
			ActualItemMarginProperty = ActualItemMarginPropertyKey.DependencyProperty;
			DefaultItemMarginProperty = DependencyPropertyManager.Register("DefaultItemMargin", typeof(Thickness?), typeof(GalleryControl),
				new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnDefaultItemMarginPropertyChanged)));
			DefaultItemGlyphMarginProperty = DependencyPropertyManager.Register("DefaultItemGlyphMargin", typeof(Thickness?), typeof(GalleryControl),
				new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnDefaultItemGlyphMarginPropertyChanged)));
			DefaultItemCaptionMarginProperty = DependencyPropertyManager.Register("DefaultItemCaptionMargin", typeof(Thickness?), typeof(GalleryControl),
				new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnDefaultItemCaptionMarginPropertyChanged)));
			DefaultItemDescriptionMarginProperty = DependencyPropertyManager.Register("DefaultItemDescriptionMargin", typeof(Thickness?), typeof(GalleryControl),
				new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnDefaultItemDescriptionMarginPropertyChanged)));
			ItemCaptionTextStyleProperty = DependencyPropertyManager.Register("ItemCaptionTextStyle", typeof(Style), typeof(GalleryControl),
				new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnItemCaptionTextStylePropertyChanged)));
			ItemCaptionTextStyleSelectorProperty = DependencyPropertyManager.Register("ItemCaptionTextStyleSelector", typeof(StatedStyleSelector), typeof(GalleryControl),
				new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnItemCaptionTextStyleSelectorPropertyChanged)));
			ActualItemCaptionTextStylePropertyKey = DependencyPropertyManager.RegisterReadOnly("ActualItemCaptionTextStyle", typeof(Style), typeof(GalleryControl),
				new FrameworkPropertyMetadata(null));
			ActualItemCaptionTextStyleProperty = ActualItemCaptionTextStylePropertyKey.DependencyProperty;
			ItemDescriptionTextStyleProperty = DependencyPropertyManager.Register("ItemDescriptionTextStyle", typeof(Style), typeof(GalleryControl),
				new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnItemDescriptionTextStylePropertyChanged)));
			ItemDescriptionTextStyleSelectorProperty = DependencyPropertyManager.Register("ItemDescriptionTextStyleSelector", typeof(StatedStyleSelector), typeof(GalleryControl), new FrameworkPropertyMetadata(null,
				new PropertyChangedCallback(OnItemDescriptionTextStyleSelectorPropertyChanged)));
			ActualItemDescriptionTextStylePropertyKey = DependencyPropertyManager.RegisterReadOnly("ActualItemDescriptionTextStyle", typeof(Style), typeof(GalleryControl),
				new FrameworkPropertyMetadata(null));
			ActualItemDescriptionTextStyleProperty = ActualItemDescriptionTextStylePropertyKey.DependencyProperty;
			GroupCaptionTextStyleProperty = DependencyPropertyManager.Register("GroupCaptionTextStyle", typeof(Style), typeof(GalleryControl),
				new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnGroupCaptionTextStylePropertyChanged)));
			ActualGroupCaptionTextStylePropertyKey = DependencyPropertyManager.RegisterReadOnly("ActualGroupCaptionTextStyle", typeof(Style), typeof(GalleryControl),
				new FrameworkPropertyMetadata(null));
			ActualGroupCaptionTextStyleProperty = ActualGroupCaptionTextStylePropertyKey.DependencyProperty;
			NormalFilterCaptionTextStyleProperty = DependencyPropertyManager.Register("NormalFilterCaptionTextStyle", typeof(Style), typeof(GalleryControl),
				new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnNormalFilterCaptionTextStylePropertyChanged)));
			ActualNormalFilterCaptionTextStylePropertyKey = DependencyPropertyManager.RegisterReadOnly("ActualNormalFilterCaptionTextStyle", typeof(Style), typeof(GalleryControl),
				new FrameworkPropertyMetadata(null));
			ActualNormalFilterCaptionTextStyleProperty = ActualNormalFilterCaptionTextStylePropertyKey.DependencyProperty;
			HintTextStyleProperty = DependencyPropertyManager.Register("HintTextStyle", typeof(Style), typeof(GalleryControl),
				new FrameworkPropertyMetadata(null));
			ActualHintTextStylePropertyKey = DependencyPropertyManager.RegisterReadOnly("ActualHintTextStyle", typeof(Style), typeof(GalleryControl),
				new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnActualHintTextStylePropertyChanged)));
			ActualHintTextStyleProperty = ActualHintTextStylePropertyKey.DependencyProperty;
			HintCaptionTextStyleProperty = DependencyPropertyManager.Register("HintCaptionTextStyle", typeof(Style), typeof(GalleryControl),
				new FrameworkPropertyMetadata(null));
			SelectedFilterCaptionTextStyleProperty = DependencyPropertyManager.Register("SelectedFilterCaptionTextStyle", typeof(Style), typeof(GalleryControl),
				new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnSelectedFilterCaptionTextStylePropertyChanged)));
			ActualSelectedFilterCaptionTextStylePropertyKey = DependencyPropertyManager.RegisterReadOnly("ActualSelectedFilterCaptionTextStyle", typeof(Style), typeof(GalleryControl),
				new FrameworkPropertyMetadata(null));
			ActualSelectedFilterCaptionTextStyleProperty = ActualSelectedFilterCaptionTextStylePropertyKey.DependencyProperty;
			ActualIsGroupCaptionVisiblePropertyKey = DependencyPropertyManager.RegisterReadOnly("ActualIsGroupCaptionVisible", typeof(bool), typeof(GalleryControl),
				new FrameworkPropertyMetadata(true));
			ActualIsGroupCaptionVisibleProperty = ActualIsGroupCaptionVisiblePropertyKey.DependencyProperty;
			DefaultIsGroupCaptionVisibleProperty = DependencyPropertyManager.Register("DefaultIsGroupCaptionVisible", typeof(bool), typeof(GalleryControl),
				new FrameworkPropertyMetadata(true, new PropertyChangedCallback(OnDefaultIsGroupCaptionVisiblePropertyChanged)));
			DefaultGroupCaptionMarginProperty = DependencyPropertyManager.Register("DefaultGroupCaptionMargin", typeof(Thickness?), typeof(GalleryControl),
				new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnDefaultGroupCaptionMarginPropertyChanged)));
			ActualGroupCaptionMarginPropertyKey = DependencyPropertyManager.RegisterReadOnly("ActualGroupCaptionMargin", typeof(Thickness), typeof(GalleryControl), new FrameworkPropertyMetadata(new Thickness()));
			ActualGroupCaptionMarginProperty = ActualGroupCaptionMarginPropertyKey.DependencyProperty;
			DesiredRowCountProperty = DependencyPropertyManager.Register("DesiredRowCount", typeof(int), typeof(GalleryControl), new FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.AffectsMeasure, new PropertyChangedCallback(OnDesiredRowCountPropertyChanged)));
			DesiredColCountProperty = DependencyPropertyManager.Register("DesiredColCount", typeof(int), typeof(GalleryControl), new FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.AffectsMeasure, new PropertyChangedCallback(OnDesiredColCountPropertyChanged)));
			DefaultItemBorderTemplateProperty = DependencyPropertyManager.Register("DefaultItemBorderTemplate", typeof(ControlTemplate), typeof(GalleryControl),
				new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnDefaultItemBorderTemplatePropertyChanged)));
			DefaultItemGlyphBorderTemplateProperty = DependencyPropertyManager.Register("DefaultItemGlyphBorderTemplate", typeof(ControlTemplate), typeof(GalleryControl),
				new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnDefaultItemGlyphBorderTemplatePropertyChanged)));
			ActualItemBorderTemplatePropertyKey = DependencyPropertyManager.RegisterReadOnly("ActualItemBorderTemplate", typeof(ControlTemplate), typeof(GalleryControl), new FrameworkPropertyMetadata(null));
			ActualItemBorderTemplateProperty = ActualItemBorderTemplatePropertyKey.DependencyProperty;
			ActualItemGlyphBorderTemplatePropertyKey = DependencyPropertyManager.RegisterReadOnly("ActualItemGlyphBorderTemplate", typeof(ControlTemplate), typeof(GalleryControl), new FrameworkPropertyMetadata(null));
			ActualItemGlyphBorderTemplateProperty = ActualItemGlyphBorderTemplatePropertyKey.DependencyProperty;
			DefaultGroupItemsMarginProperty = DependencyPropertyManager.Register("DefaultGroupItemsMargin", typeof(Thickness?), typeof(GalleryControl),
				new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnDefaultGroupItemsMarginPropertyChanged)));
			ActualGroupItemsMarginPropertyKey = DependencyPropertyManager.RegisterReadOnly("ActualGroupItemsMargin", typeof(Thickness), typeof(GalleryControl), new FrameworkPropertyMetadata(new Thickness()));
			ActualGroupItemsMarginProperty = ActualGroupItemsMarginPropertyKey.DependencyProperty;
			DefaultItemControlTemplateProperty = DependencyPropertyManager.Register("DefaultItemControlTemplate", typeof(ControlTemplate), typeof(GalleryControl),
				new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnDefaultItemControlTemplatePropertyChanged)));
			ActualItemControlTemplatePropertyKey = DependencyPropertyManager.RegisterReadOnly("ActualItemControlTemplate", typeof(ControlTemplate), typeof(GalleryControl), new FrameworkPropertyMetadata(null));
			ActualItemControlTemplateProperty = ActualItemControlTemplatePropertyKey.DependencyProperty;		
			DefaultItemGlyphBorderPaddingProperty = DependencyPropertyManager.Register("DefaultItemGlyphBorderPadding", typeof(Thickness?), typeof(GalleryControl),
				new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnDefaultItemGlyphBorderPaddingPropertyChanged)));
			ActualItemGlyphBorderPaddingPropertyKey = DependencyPropertyManager.RegisterReadOnly("ActualItemGlyphBorderPadding", typeof(Thickness), typeof(GalleryControl), new FrameworkPropertyMetadata(new Thickness()));
			ActualItemGlyphBorderPaddingProperty = ActualItemGlyphBorderPaddingPropertyKey.DependencyProperty;
			ActualVerticalScrollBarVisibilityPropertyKey = DependencyPropertyManager.RegisterReadOnly("ActualVerticalScrollBarVisibility", typeof(Visibility), typeof(GalleryControl),
				new FrameworkPropertyMetadata(Visibility.Collapsed));
			ActualVerticalScrollBarVisibilityProperty = ActualVerticalScrollBarVisibilityPropertyKey.DependencyProperty;
			DefaultGroupCaptionControlTemplateProperty = DependencyPropertyManager.Register("DefaultGroupCaptionControlTemplate", typeof(ControlTemplate), typeof(GalleryControl),
				new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnDefaultGroupCaptionControlTemplatePropertyChanged)));
			ActualGroupCaptionControlTemplatePropertyKey = DependencyPropertyManager.RegisterReadOnly("ActualGroupCaptionControlTemplate", typeof(ControlTemplate), typeof(GalleryControl), new FrameworkPropertyMetadata(null));
			ActualGroupCaptionControlTemplateProperty = ActualGroupCaptionControlTemplatePropertyKey.DependencyProperty;
			DefaultFilterControlTemplateProperty = DependencyPropertyManager.Register("DefaultFilterControlTemplate", typeof(ControlTemplate), typeof(GalleryControl),
				new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnDefaultFilterControlTemplatePropertyChanged)));			
			ActualFilterControlTemplatePropertyKey = DependencyPropertyManager.RegisterReadOnly("ActualFilterControlTemplate", typeof(ControlTemplate), typeof(GalleryControl), new FrameworkPropertyMetadata(null));
			ActualFilterControlTemplateProperty = ActualFilterControlTemplatePropertyKey.DependencyProperty;
			ActualVerticalOffsetProperty = DependencyPropertyManager.Register("ActualVerticalOffset", typeof(double), typeof(GalleryControl),
				new FrameworkPropertyMetadata(0d, new PropertyChangedCallback(OnActualVerticalOffsetPropertyChanged)));
			IsManipulationEnabledProperty.OverrideMetadata(typeof(GalleryControl), new PropertyMetadata(true));
		}
		static void OnNormalFilterCaptionTextStylePropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e) {
			((GalleryControl)obj).UpdateActualTextStyles();
		}						
		static void OnGroupCaptionTextStylePropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e) {
			((GalleryControl)obj).UpdateActualTextStyles();
		}				
		static void OnItemDescriptionTextStylePropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e) {
			((GalleryControl)obj).UpdateActualTextStyles();
		}
		protected static void OnItemDescriptionTextStyleSelectorPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((GalleryControl)d).OnItemDescriptionTextStyleSelectorChanged((StatedStyleSelector)e.OldValue);
		}
		static void OnActualHintTextStylePropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e) {
			((GalleryControl)obj).UpdateActualTextStyles();
		}		
		static void OnActualDesiredColCountPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e) {
			((GalleryControl)obj).InvalidateMeasure();
		}
		static void OnDesiredColCountPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e) {
			((GalleryControl)obj).InvalidateMeasure();
		}
		static void OnGalleryPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e) {
			((GalleryControl)obj).InvalidateMeasure();
			((GalleryControl)obj).OnGalleryChanged(e.OldValue as Gallery);
		}
		protected static void OnItemCaptionTextStylePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((GalleryControl)d).UpdateActualTextStyles();
		}
		protected static void OnItemCaptionTextStyleSelectorPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((GalleryControl)d).OnItemCaptionTextStyleSelectorChanged((StatedStyleSelector)e.OldValue);
		}
		protected static void OnDefaultItemMarginPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((GalleryControl)d).UpdateActualMargins();
		}
		protected static void OnDefaultItemDescriptionMarginPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((GalleryControl)d).UpdateActualMargins();
		}
		protected static void OnDefaultItemGlyphMarginPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((GalleryControl)d).UpdateActualMargins();			
		}
		protected static void OnDefaultItemCaptionMarginPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((GalleryControl)d).UpdateActualMargins();			
		}
		protected static void OnDefaultIsGroupCaptionVisiblePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((GalleryControl)d).UpdateActualIsGroupCaptionVisible();
		}
		protected static void OnDefaultGroupCaptionMarginPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((GalleryControl)d).UpdateActualMargins();
		}
		protected static void OnDesiredRowCountPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e) {
		}
		protected static void OnSelectedFilterCaptionTextStylePropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e) {
			((GalleryControl)obj).UpdateActualTextStyles();
		}
		protected static void OnDefaultItemBorderTemplatePropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e) {
			((GalleryControl)obj).UpdateActualCustomTemplates();
		}
		protected static void OnDefaultItemGlyphBorderTemplatePropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e) {
			((GalleryControl)obj).UpdateActualCustomTemplates();
		}
		protected static void OnDefaultItemControlTemplatePropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e) {
			((GalleryControl)obj).UpdateActualCustomTemplates();
		}
		protected static void OnDefaultGroupCaptionControlTemplatePropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e) {
			((GalleryControl)obj).UpdateActualCustomTemplates();
		}
		protected static void OnDefaultFilterControlTemplatePropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e) {
			((GalleryControl)obj).UpdateActualCustomTemplates();
		}
		protected static void OnDefaultGroupItemsMarginPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((GalleryControl)d).UpdateActualMargins();
		}
		static void OnDefaultItemGlyphBorderPaddingPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e) {
			((GalleryControl)obj).UpdateActualMargins();
		}
		static void OnActualVerticalOffsetPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e) {
			((GalleryControl)obj).OnActualVerticalOffsetChanged((double)e.OldValue);
		}
		#endregion
		#region dep props
#if !SL
	[DevExpressXpfCoreLocalizedDescription("GalleryControlDesiredColCount")]
#endif
		public int DesiredColCount {
			get { return (int)GetValue(DesiredColCountProperty); }
			set { SetValue(DesiredColCountProperty, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("GalleryControlPlacementTarget")]
#endif
		public GalleryPlacementTarget PlacementTarget {
			get { return (GalleryPlacementTarget)GetValue(PlacementTargetProperty); }
			set { SetValue(PlacementTargetProperty, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("GalleryControlActualItemGlyphMargin")]
#endif
		public Thickness ActualItemGlyphMargin {
			get { return (Thickness)GetValue(ActualItemGlyphMarginProperty); }
			protected set { this.SetValue(ActualItemGlyphMarginPropertyKey, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("GalleryControlActualItemDescriptionMargin")]
#endif
		public Thickness ActualItemDescriptionMargin {
			get { return (Thickness)GetValue(ActualItemDescriptionMarginProperty); }
			protected set { this.SetValue(ActualItemDescriptionMarginPropertyKey, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("GalleryControlActualItemCaptionMargin")]
#endif
		public Thickness ActualItemCaptionMargin {
			get { return (Thickness)GetValue(ActualItemCaptionMarginProperty); }
			protected set { this.SetValue(ActualItemCaptionMarginPropertyKey, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("GalleryControlActualItemMargin")]
#endif
		public Thickness ActualItemMargin {
			get { return (Thickness)GetValue(ActualItemMarginProperty); }
			protected set { this.SetValue(ActualItemMarginPropertyKey, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("GalleryControlDefaultItemMargin")]
#endif
		public Thickness? DefaultItemMargin {
			get { return (Thickness?)GetValue(DefaultItemMarginProperty); }
			set { SetValue(DefaultItemMarginProperty, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("GalleryControlDefaultItemGlyphMargin")]
#endif
		public Thickness? DefaultItemGlyphMargin {
			get { return (Thickness?)GetValue(DefaultItemGlyphMarginProperty); }
			set { SetValue(DefaultItemGlyphMarginProperty, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("GalleryControlDefaultItemCaptionMargin")]
#endif
		public Thickness? DefaultItemCaptionMargin {
			get { return (Thickness?)GetValue(DefaultItemCaptionMarginProperty); }
			set { SetValue(DefaultItemCaptionMarginProperty, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("GalleryControlDefaultItemDescriptionMargin")]
#endif
		public Thickness? DefaultItemDescriptionMargin {
			get { return (Thickness?)GetValue(DefaultItemDescriptionMarginProperty); }
			set { SetValue(DefaultItemDescriptionMarginProperty, value); }
		}
		[Obsolete("Use GalleryControl.ItemCaptionTextStyleSelector property instead")]
#if !SL
	[DevExpressXpfCoreLocalizedDescription("GalleryControlItemCaptionTextStyle")]
#endif
		public Style ItemCaptionTextStyle {
			get { return (Style)GetValue(ItemCaptionTextStyleProperty); }
			set { SetValue(ItemCaptionTextStyleProperty, value); }
		}
		public StatedStyleSelector ItemCaptionTextStyleSelector {
			get { return (StatedStyleSelector)GetValue(ItemCaptionTextStyleSelectorProperty); }
			set { SetValue(ItemCaptionTextStyleSelectorProperty, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("GalleryControlActualItemCaptionTextStyle")]
#endif
		public Style ActualItemCaptionTextStyle {
			get { return (Style)GetValue(ActualItemCaptionTextStyleProperty); }
			protected set { this.SetValue(ActualItemCaptionTextStylePropertyKey, value); }
		}
		[Obsolete("Use GalleryControl.ItemDescriptionTextStyleSelector property instead")]
#if !SL
	[DevExpressXpfCoreLocalizedDescription("GalleryControlItemDescriptionTextStyle")]
#endif
		public Style ItemDescriptionTextStyle {
			get { return (Style)GetValue(ItemDescriptionTextStyleProperty); }
			set { SetValue(ItemDescriptionTextStyleProperty, value); }
		}
		public StatedStyleSelector ItemDescriptionTextStyleSelector {
			get { return (StatedStyleSelector)GetValue(ItemDescriptionTextStyleSelectorProperty); }
			set { SetValue(ItemDescriptionTextStyleSelectorProperty, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("GalleryControlActualItemDescriptionTextStyle")]
#endif
		public Style ActualItemDescriptionTextStyle {
			get { return (Style)GetValue(ActualItemDescriptionTextStyleProperty); }
			protected set { this.SetValue(ActualItemDescriptionTextStylePropertyKey, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("GalleryControlGroupCaptionTextStyle")]
#endif
		public Style GroupCaptionTextStyle {
			get { return (Style)GetValue(GroupCaptionTextStyleProperty); }
			set { SetValue(GroupCaptionTextStyleProperty, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("GalleryControlActualGroupCaptionTextStyle")]
#endif
		public Style ActualGroupCaptionTextStyle {
			get { return (Style)GetValue(ActualGroupCaptionTextStyleProperty); }
			protected set { this.SetValue(ActualGroupCaptionTextStylePropertyKey, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("GalleryControlNormalFilterCaptionTextStyle")]
#endif
		public Style NormalFilterCaptionTextStyle {
			get { return (Style)GetValue(NormalFilterCaptionTextStyleProperty); }
			set { SetValue(NormalFilterCaptionTextStyleProperty, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("GalleryControlActualNormalFilterCaptionTextStyle")]
#endif
		public Style ActualNormalFilterCaptionTextStyle {
			get { return (Style)GetValue(ActualNormalFilterCaptionTextStyleProperty); }
			protected set { this.SetValue(ActualNormalFilterCaptionTextStylePropertyKey, value); }
		}
		[Obsolete("Use Gallery.HintTemplate property instead")]
#if !SL
	[DevExpressXpfCoreLocalizedDescription("GalleryControlHintTextStyle")]
#endif
		public Style HintTextStyle {
			get { return (Style)GetValue(HintTextStyleProperty); }
			set { SetValue(HintTextStyleProperty, value); }
		}
		[Obsolete("Use Gallery.HintTemplate property instead")]
#if !SL
	[DevExpressXpfCoreLocalizedDescription("GalleryControlActualHintTextStyle")]
#endif
		public Style ActualHintTextStyle {
			get { return (Style)GetValue(ActualHintTextStyleProperty); }
			protected set { this.SetValue(ActualHintTextStylePropertyKey, value); }
		}
		[Obsolete("Use Gallery.HintCaptionTemplate property instead")]
#if !SL
	[DevExpressXpfCoreLocalizedDescription("GalleryControlHintCaptionTextStyle")]
#endif
		public Style HintCaptionTextStyle {
			get { return (Style)GetValue(HintCaptionTextStyleProperty); }
			set { SetValue(HintCaptionTextStyleProperty, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("GalleryControlSelectedFilterCaptionTextStyle")]
#endif
		public Style SelectedFilterCaptionTextStyle {
			get { return (Style)GetValue(SelectedFilterCaptionTextStyleProperty); }
			set { SetValue(SelectedFilterCaptionTextStyleProperty, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("GalleryControlActualSelectedFilterCaptionTextStyle")]
#endif
		public Style ActualSelectedFilterCaptionTextStyle {
			get { return (Style)GetValue(ActualSelectedFilterCaptionTextStyleProperty); }
			protected set { this.SetValue(ActualSelectedFilterCaptionTextStylePropertyKey, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("GalleryControlActualIsGroupCaptionVisible")]
#endif
		public bool ActualIsGroupCaptionVisible {
			get { return (bool)GetValue(ActualIsGroupCaptionVisibleProperty); }
			protected set { this.SetValue(ActualIsGroupCaptionVisiblePropertyKey, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("GalleryControlDefaultIsGroupCaptionVisible")]
#endif
		public bool DefaultIsGroupCaptionVisible {
			get { return (bool)GetValue(DefaultIsGroupCaptionVisibleProperty); }
			set { SetValue(DefaultIsGroupCaptionVisibleProperty, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("GalleryControlDefaultGroupCaptionMargin")]
#endif
		public Thickness? DefaultGroupCaptionMargin {
			get { return (Thickness?)GetValue(DefaultGroupCaptionMarginProperty); }
			set { this.SetValue(DefaultGroupCaptionMarginProperty, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("GalleryControlActualGroupCaptionMargin")]
#endif
		public Thickness ActualGroupCaptionMargin {
			get { return (Thickness)GetValue(ActualGroupCaptionMarginProperty); }
			protected set { this.SetValue(ActualGroupCaptionMarginPropertyKey, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("GalleryControlDesiredRowCount")]
#endif
		public int DesiredRowCount {
			get { return (int)GetValue(DesiredRowCountProperty); }
			set { SetValue(DesiredRowCountProperty, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("GalleryControlDefaultItemBorderTemplate")]
#endif
		public ControlTemplate DefaultItemBorderTemplate {
			get { return (ControlTemplate)GetValue(DefaultItemBorderTemplateProperty); }
			set { SetValue(DefaultItemBorderTemplateProperty, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("GalleryControlDefaultItemGlyphBorderTemplate")]
#endif
		public ControlTemplate DefaultItemGlyphBorderTemplate {
			get { return (ControlTemplate)GetValue(DefaultItemGlyphBorderTemplateProperty); }
			set { SetValue(DefaultItemGlyphBorderTemplateProperty, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("GalleryControlActualItemBorderTemplate")]
#endif
		public ControlTemplate ActualItemBorderTemplate {
			get { return (ControlTemplate)GetValue(ActualItemBorderTemplateProperty); }
			protected set { this.SetValue(ActualItemBorderTemplatePropertyKey, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("GalleryControlActualItemGlyphBorderTemplate")]
#endif
		public ControlTemplate ActualItemGlyphBorderTemplate {
			get { return (ControlTemplate)GetValue(ActualItemGlyphBorderTemplateProperty); }
			protected set { this.SetValue(ActualItemGlyphBorderTemplatePropertyKey, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("GalleryControlActualGroupItemsMargin")]
#endif
		public Thickness ActualGroupItemsMargin {
			get { return (Thickness)GetValue(ActualGroupItemsMarginProperty); }
			protected set { this.SetValue(ActualGroupItemsMarginPropertyKey, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("GalleryControlDefaultGroupItemsMargin")]
#endif
		public Thickness? DefaultGroupItemsMargin {
			get { return (Thickness?)GetValue(DefaultGroupItemsMarginProperty); }
			set { SetValue(DefaultGroupItemsMarginProperty, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("GalleryControlDefaultItemControlTemplate")]
#endif
		public ControlTemplate DefaultItemControlTemplate {
			get { return (ControlTemplate)GetValue(DefaultItemControlTemplateProperty); }
			set { SetValue(DefaultItemControlTemplateProperty, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("GalleryControlActualItemControlTemplate")]
#endif
		public ControlTemplate ActualItemControlTemplate {
			get { return (ControlTemplate)GetValue(ActualItemControlTemplateProperty); }
			protected set { this.SetValue(ActualItemControlTemplatePropertyKey, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("GalleryControlDefaultItemGlyphBorderPadding")]
#endif
		public Thickness? DefaultItemGlyphBorderPadding {
			get { return (Thickness?)GetValue(DefaultItemGlyphBorderPaddingProperty); }
			set { SetValue(DefaultItemGlyphBorderPaddingProperty, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("GalleryControlActualItemGlyphBorderPadding")]
#endif
		public Thickness ActualItemGlyphBorderPadding {
			get { return (Thickness)GetValue(ActualItemGlyphBorderPaddingProperty); }
			protected set { this.SetValue(ActualItemGlyphBorderPaddingPropertyKey, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("GalleryControlActualVerticalScrollBarVisibility")]
#endif
		public Visibility ActualVerticalScrollBarVisibility {
			get { return (Visibility)GetValue(ActualVerticalScrollBarVisibilityProperty); }
			protected set { this.SetValue(ActualVerticalScrollBarVisibilityPropertyKey, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("GalleryControlDefaultGroupCaptionControlTemplate")]
#endif
		public ControlTemplate DefaultGroupCaptionControlTemplate {
			get { return (ControlTemplate)GetValue(DefaultGroupCaptionControlTemplateProperty); }
			set { SetValue(DefaultGroupCaptionControlTemplateProperty, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("GalleryControlActualGroupCaptionControlTemplate")]
#endif
		public ControlTemplate ActualGroupCaptionControlTemplate {
			get { return (ControlTemplate)GetValue(ActualGroupCaptionControlTemplateProperty); }
			protected set { this.SetValue(ActualGroupCaptionControlTemplatePropertyKey, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("GalleryControlDefaultFilterControlTemplate")]
#endif
		public ControlTemplate DefaultFilterControlTemplate {
			get { return (ControlTemplate)GetValue(DefaultFilterControlTemplateProperty); }
			set { SetValue(DefaultFilterControlTemplateProperty, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("GalleryControlActualFilterControlTemplate")]
#endif
		public ControlTemplate ActualFilterControlTemplate {
			get { return (ControlTemplate)GetValue(ActualFilterControlTemplateProperty); }
			protected set { this.SetValue(ActualFilterControlTemplatePropertyKey, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("GalleryControlActualVerticalOffset")]
#endif
		public double ActualVerticalOffset {
			get { return (double)GetValue(ActualVerticalOffsetProperty); }
			set { SetValue(ActualVerticalOffsetProperty, value); }
		}
		#endregion
		public GalleryControl() {
			GalleryControl.SetGalleryControl(this, this);
			navigationManager = new GalleryControlNavigationManager(this);
			AllowCyclicNavigation = true;
			Loaded += new RoutedEventHandler(OnLoadedCore);
			LayoutUpdated += new System.EventHandler(OnLayoutUpdatedCore);
			Unloaded += new RoutedEventHandler(OnUnloadedCore);
			IsTabStop = false;
			}		
		void OnUnloadedCore(object sender, RoutedEventArgs e) {
			LayoutUpdated -= OnLayoutUpdatedCore;
			UnsubscribeTemplateEvents();
			OnUnloaded();
		}
		protected virtual void OnUnloaded() {
		}
		protected virtual void OnLoaded() {
		}
		protected override System.Collections.IEnumerator LogicalChildren {
			get {
				List<object> newLogicalChildren = new List<object>();
				IEnumerator oldLogicalChildren = base.LogicalChildren;
				while(oldLogicalChildren.MoveNext())
					newLogicalChildren.Add(oldLogicalChildren.Current);
				if(Gallery != null && Gallery.Parent == this)
					newLogicalChildren.Add(Gallery);
				return newLogicalChildren.GetEnumerator();
			}
		}
		bool isPanning = false;
		IInputElement IManipulationClient.GetManipulationContainer() {
			return this;
		}
		Vector IManipulationClient.GetScrollValue() {
			return new Vector(ScrollHost.ContentHorizontalOffset, ScrollHost.ContentVerticalOffset);
		}
		Vector IManipulationClient.GetMaxScrollValue() {
			return new Vector(ScrollHost.ScrollableSize.Width - ScrollHost.ViewportSize.Width, ScrollHost.ScrollableSize.Height - ScrollHost.ViewportSize.Height);
		}
		Vector IManipulationClient.GetMinScrollValue() {
			return new Vector(0, 0);
		}
		void IManipulationClient.Scroll(Vector scrollValue) {
			SetVerticalOffset(scrollValue.Y);
		}
		ManipulationHelper manipulationHelper;
		ManipulationHelper ManipulationHelper {
			get {
				if(manipulationHelper == null)
					manipulationHelper = new ManipulationHelper(this);
				return manipulationHelper;
			}
		}
		protected override void OnManipulationStarting(ManipulationStartingEventArgs e) {
			base.OnManipulationStarting(e);
			ManipulationHelper.OnManipulationStarting(e);
		}
		protected override void OnManipulationInertiaStarting(ManipulationInertiaStartingEventArgs e) {
			base.OnManipulationInertiaStarting(e);
			if(!isPanning)
				e.Cancel();
			ManipulationHelper.OnManipulationInertiaStarting(e);
		}
		protected override void OnManipulationDelta(ManipulationDeltaEventArgs e) {
			base.OnManipulationDelta(e);
			isPanning = true;
			ManipulationHelper.OnManipulationDelta(e);
		}
		protected override void OnManipulationCompleted(ManipulationCompletedEventArgs e) {
			base.OnManipulationCompleted(e);
			isPanning = false;
			e.Handled = true;
		}
		protected override System.Windows.Automation.Peers.AutomationPeer OnCreateAutomationPeer() {
			return NavigationAutomationPeersCreator.Default.Create(this);
		}
		void OnLoadedCore(object sender, RoutedEventArgs e) {
			LayoutUpdated -= OnLayoutUpdatedCore;
			LayoutUpdated += new System.EventHandler(OnLayoutUpdatedCore);
			if(GroupsControl == null) return;
			UnsubscribeTemplateEvents();
			SubscribeTemplateEvents();
			OnLoaded();			
		}
		protected virtual void UpdateVerticalScrollBarVisibility() {
			if(PlacementTarget == GalleryPlacementTarget.Ribbon) {
				ActualVerticalScrollBarVisibility = Visibility.Collapsed;
				return;
			}
			if(ScrollHost == null || Gallery == null) return;
			switch(Gallery.VerticalScrollbarVisibility) {
				case ScrollBarVisibility.Auto:
					if(ScrollHost.ScrollableSize.Height <= ScrollHost.ActualHeight) {
						ActualVerticalScrollBarVisibility = Visibility.Collapsed;
					}
					else {
						ActualVerticalScrollBarVisibility = Visibility.Visible;
					}
					break;
				case ScrollBarVisibility.Disabled:
				case ScrollBarVisibility.Hidden:
					ActualVerticalScrollBarVisibility = Visibility.Collapsed;
					break;
				case ScrollBarVisibility.Visible:
					ActualVerticalScrollBarVisibility = Visibility.Visible;
					break;
			}
		}
		void UpdateVerticalScrollBarChangeValue() {
			if(VerticalScrollBar == null) return;
			double value = 0;
			if(ScrollHost != null) value = ScrollHost.GetRowsHeight(1);
			if(value <= 0) value = 1;
			VerticalScrollBar.LargeChange = value / 2;
			VerticalScrollBar.SmallChange = value / 2;
		}
		void UpdateVerticalScrollBarViewPortSize() {
			if(VerticalScrollBar == null) return;
			VerticalScrollBar.ViewportSize = ViewportSize.Height;
		}
		protected virtual void OnLayoutUpdated() {
			UpdateVerticalScrollBarVisibility();
			UpdateVerticalScrollBarChangeValue();
		}
		void OnLayoutUpdatedCore(object sender, System.EventArgs e) {
			OnLayoutUpdated();
		}
		Thickness GetActualMargin(Thickness? hightPriorityMargin, Thickness? lowPriorityMargin) {
			if(hightPriorityMargin != null) return hightPriorityMargin.Value;
			if(lowPriorityMargin != null) return lowPriorityMargin.Value;
			return new Thickness();
		}
		protected virtual void UpdateActualCustomTemplates() {
			ActualFilterControlTemplate = (Gallery == null || Gallery.FilterControlTemplate == null) ? DefaultFilterControlTemplate : Gallery.FilterControlTemplate;
			ActualItemControlTemplate = (Gallery == null || Gallery.ItemControlTemplate == null) ? DefaultItemControlTemplate : Gallery.ItemControlTemplate;
			ActualGroupCaptionControlTemplate = (Gallery == null || Gallery.GroupCaptionControlTemplate == null) ? DefaultGroupCaptionControlTemplate : Gallery.GroupCaptionControlTemplate;
			ActualItemBorderTemplate =  (Gallery == null || Gallery.ItemBorderTemplate == null) ? DefaultItemBorderTemplate : Gallery.ItemBorderTemplate;
			ActualItemGlyphBorderTemplate = (Gallery == null || Gallery.ItemGlyphBorderTemplate == null) ? DefaultItemGlyphBorderTemplate : Gallery.ItemGlyphBorderTemplate;
		}
		protected virtual void UpdateActualTextStyles() {
			ActualGroupCaptionTextStyle = (Gallery == null || Gallery.GroupCaptionTextStyle == null) ? GroupCaptionTextStyle : Gallery.GroupCaptionTextStyle;
			ActualNormalFilterCaptionTextStyle = (Gallery == null || Gallery.NormalFilterCaptionTextStyle == null) ? NormalFilterCaptionTextStyle : Gallery.NormalFilterCaptionTextStyle;
			ActualSelectedFilterCaptionTextStyle = (Gallery == null || Gallery.SelectedFilterCaptionTextStyle == null) ? SelectedFilterCaptionTextStyle : Gallery.SelectedFilterCaptionTextStyle;
		}
		protected virtual void OnItemCaptionTextStyleSelectorChanged(StatedStyleSelector oldValue) {
		}
		protected virtual void OnItemDescriptionTextStyleSelectorChanged(StatedStyleSelector oldValue) {
		}
		protected virtual void UpdateActualMargins() {
			if(Gallery == null) return;
			ActualItemMargin = GetActualMargin(Gallery.ItemMargin, DefaultItemMargin);
			ActualItemGlyphMargin = GetActualMargin(Gallery.ItemGlyphMargin, DefaultItemGlyphMargin);
			ActualItemCaptionMargin = GetActualMargin(Gallery.ItemCaptionMargin, DefaultItemCaptionMargin);
			ActualItemDescriptionMargin = GetActualMargin(Gallery.ItemDescriptionMargin, DefaultItemDescriptionMargin);
			ActualGroupCaptionMargin = GetActualMargin(Gallery.GroupCaptionMargin, DefaultGroupCaptionMargin);
			ActualGroupItemsMargin = GetActualMargin(Gallery.GroupItemsMargin, DefaultGroupItemsMargin);
			ActualItemGlyphBorderPadding = GetActualMargin(Gallery.ItemGlyphBorderPadding, DefaultItemGlyphBorderPadding);
		}
		protected virtual void UpdateActualIsGroupCaptionVisible() {
			if(Gallery != null && Gallery.IsGroupCaptionVisible != DefaultBoolean.Default) {
				ActualIsGroupCaptionVisible = Gallery.IsGroupCaptionVisible == DefaultBoolean.True ? true : false;
			}
			else {
				ActualIsGroupCaptionVisible = DefaultIsGroupCaptionVisible;
			}
		}
		ContentControl BarManagerContainer { get; set; }
		protected internal GalleryGroupsViewer ScrollHost { get; protected set; }
		internal protected GalleryItemGroupsControl GroupsControl { get; private set; }
		internal ScrollBar VerticalScrollBar { get; set; }
		internal ToggleButton Caption { get; set; }
		PopupMenu filterMenu = null;
		bool SupressUncheckCaption { get; set; }
#if !SL
	[DevExpressXpfCoreLocalizedDescription("GalleryControlViewportSize")]
#endif
		public Size ViewportSize {
			get {
				if(ScrollHost == null) return new Size();
				return ScrollHost.ViewportSize;
			}
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("GalleryControlContentVerticalOffset")]
#endif
		public double ContentVerticalOffset {
			get {
				if(ScrollHost == null) return 0;
				return ScrollHost.ContentVerticalOffset;
			}
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("GalleryControlScrollableSize")]
#endif
		public Size ScrollableSize {
			get {
				if(ScrollHost == null) return new Size();
				return ScrollHost.ScrollableSize;
			}
		}
		public int GetActualColCount() {
			if(ScrollHost == null) return 0;
			return ScrollHost.GetColCount();
		}
		public int GetActualRowCount() {
			if(ScrollHost == null) return 0;
			return ScrollHost.GetRowCount();
		}
		public int GetFirstVisibleRowIndex() {
			if(ScrollHost == null) return 0;
			return ScrollHost.GetFirstVisibleRowIndex();
		}
		public int GetLastVisibleRowIndex() {
			if(ScrollHost == null) return 0;
			return ScrollHost.GetLastVisibleRowIndex();
		}
		Storyboard ScrollAnimation { get; set; }
		void StopScrollingAnimation() {
			if(ScrollAnimation != null) {
				ActualVerticalOffset = ActualVerticalOffset;
				ScrollAnimation.Stop();
			}
		}
		double FinalVerticalOffset { get; set; }
		void StartScrollingAnimation(double time, double verticalOffset) {
			if(time == 0) {
				StopScrollingAnimation();
				SetVerticalOffset(verticalOffset);
				return;
			}
			if(ScrollAnimation == null) {
				ScrollAnimation = new Storyboard();
				DoubleAnimation doubleAnimation = new DoubleAnimation();
				doubleAnimation.FillBehavior = FillBehavior.HoldEnd;
				doubleAnimation.To = verticalOffset;
				Storyboard.SetTarget(doubleAnimation, this);
				Storyboard.SetTargetProperty(doubleAnimation, new PropertyPath("ActualVerticalOffset"));
				doubleAnimation.Duration = TimeSpan.FromMilliseconds(time);
				ScrollAnimation.Children.Add(doubleAnimation);
			}
			else {
				DoubleAnimation doubleAnimation = ScrollAnimation.Children[0] as DoubleAnimation;
				doubleAnimation.To = verticalOffset;
				doubleAnimation.FillBehavior = FillBehavior.HoldEnd;
				doubleAnimation.Duration = TimeSpan.FromMilliseconds(time);
			}
			FinalVerticalOffset = verticalOffset;
			ScrollAnimation.Begin();
		}
		protected bool SetVerticalOffset(double value) {
			if(Gallery == null)
				return false;
			ActualVerticalOffset = CoerceActualVerticalOffsetValue(value);
			return true;
		}
		protected virtual bool ScrollToVerticalOffsetCore(double value, bool shouldStop) {
			if(shouldStop)
				StopScrollingAnimation();
			if(Gallery == null)
				return false;
			if(!Gallery.AllowSmoothScrolling) {
				StartScrollingAnimation(0, CoerceActualVerticalOffsetValue(value));
				return true;
			}
			else {
				StartScrollingAnimation(Gallery.SmoothScrollingTime, CoerceActualVerticalOffsetValue(value));
			}
			return true;
		}
		public bool ScrollToVerticalOffset(double value) {
			return ScrollToVerticalOffsetCore(value, true);
		}
		public bool ScrollToItem(GalleryItem item) {
			return ScrollToVerticalOffset(ScrollHost.GetItemVerticalOffset(item));
		}
		public bool ScrollToRowByIndex(int rowIndex) {
			return ScrollToVerticalOffset(ScrollHost.GetRowsHeight(rowIndex));
		}
		public bool ScrollToGroupByIndex(int groupIndex) {
			if(Gallery == null || groupIndex < 0 || groupIndex >= Gallery.Groups.Count) return false;
			return ScrollToVerticalOffset(ScrollHost.GetGroupVerticalOffset(Gallery.Groups[groupIndex]));
		}
		public double GetGroupVerticalOffset(GalleryItemGroup group) {
			return ScrollHost.GetGroupVerticalOffset(group);
		}
		protected internal PopupMenu FilterMenu {
			get {
				if(filterMenu == null) {
					filterMenu = new PopupMenu();
					filterMenu.Closed += new EventHandler(OnFilterMenuClosed);
				}
				return filterMenu;
			}
		}
		protected virtual void OnGalleryChanged(Gallery oldValue) {
			if(oldValue != null) {
				oldValue.UserMarginsChanged -= OnGalleryMarginsChanged;
				oldValue.IsGroupCaptionVisibleChanged -= OnGalleryIsGroupCaptionVisibleChanged;
				oldValue.CustomTextStyleChanged -= OnGalleryTextStyleChanged;
				oldValue.CustomTemplateChanged -= OnGalleryCustomTemplateChanged;
				oldValue.VerticalScrollBarVisibilityChanged -= OnGalleryVerticalScrollBarVisibilityChanged;
				oldValue.VerticalScrollBarStyleChanged -= OnGalleryVerticalScrollBarStyleChanged;
				if(oldValue.GalleryControl == this) oldValue.GalleryControl = null;
				RemoveLogicalChild(oldValue);
			}
			if(Gallery != null) {
				Gallery.UserMarginsChanged += new EventHandler(OnGalleryMarginsChanged);
				Gallery.IsGroupCaptionVisibleChanged += new EventHandler(OnGalleryIsGroupCaptionVisibleChanged);
				Gallery.CustomTextStyleChanged += new EventHandler(OnGalleryTextStyleChanged);
				Gallery.CustomTemplateChanged += new EventHandler(OnGalleryCustomTemplateChanged);
				Gallery.VerticalScrollBarVisibilityChanged += new EventHandler(OnGalleryVerticalScrollBarVisibilityChanged);
				Gallery.VerticalScrollBarStyleChanged += new EventHandler(OnGalleryVerticalScrollBarStyleChanged);
				Gallery.GalleryControl = this;
				if(Gallery.Parent == null)
				AddLogicalChild(Gallery);
			}
			UpdateActualMargins();
			UpdateActualIsGroupCaptionVisible();
			UpdateActualTextStyles();
			UpdateActualCustomTemplates();
			UpdateVerticalScrollBarStyle();
		}		
		protected virtual void OnGalleryVerticalScrollBarStyleChanged(object sender, EventArgs e) {
			UpdateVerticalScrollBarStyle();
		}
		protected virtual void UpdateVerticalScrollBarStyle() {
			if(VerticalScrollBar == null || Gallery == null)
				return;
			if(Gallery.VerticalScrollBarStyle == null)
				return;
			VerticalScrollBar.Style = Gallery.VerticalScrollBarStyle;
		}
		protected virtual void OnGalleryVerticalScrollBarVisibilityChanged(object sender, EventArgs e) {
			InvalidateMeasure();
		}	   
		internal void OnGalleryItemControlClick(GalleryItemControl itemControl) {
			if(ScrollHost == null || itemControl == null) return;
			Rect bounds = ScrollHost.GetItemPosInScrollViewer(itemControl);
			if(bounds.Top < 0) {				
				ScrollToVerticalOffset(ScrollHost.ContentVerticalOffset + bounds.Top);
				return;
			}
			if(bounds.Bottom > ScrollHost.ViewportSize.Height && bounds.Top != 0) {
				ScrollToVerticalOffset(ScrollHost.ContentVerticalOffset + bounds.Bottom - ScrollHost.ViewportSize.Height);
			}			
		}
		protected virtual void OnGalleryCustomTemplateChanged(object sender, EventArgs e) {
			UpdateActualCustomTemplates();
		}
		protected virtual void OnGalleryTextStyleChanged(object sender, EventArgs e) {
			UpdateActualTextStyles();
		}
		protected virtual void OnGalleryIsGroupCaptionVisibleChanged(object sender, EventArgs e) {
			UpdateActualIsGroupCaptionVisible();
		}
		void OnGalleryMarginsChanged(object sender, EventArgs e) {
			UpdateActualMargins();   
		}
		protected override Size MeasureOverride(Size constraint) {
			Size sz = base.MeasureOverride(constraint);
			return sz;
		}
		protected virtual double CoerceActualVerticalOffsetValue(double newValue) {
			if(ScrollHost == null)
				return 0;
			if(newValue > ScrollHost.ScrollableSize.Height - ScrollHost.ViewportSize.Height)
				return ScrollHost.ScrollableSize.Height - ScrollHost.ViewportSize.Height;
			if(newValue < 0)
				return 0;
			return newValue;
		}
		protected virtual void OnActualVerticalOffsetChanged(double oldValue) {
			if(ScrollHost == null)
				return;
			ScrollHost.ScrollToVerticalOffset(ActualVerticalOffset);
			if(VerticalScrollBar != null)
				VerticalScrollBar.Value = ActualVerticalOffset;
			UpdateVerticalScrollBarVisibility();
		}
		void OnFilterMenuClosed(object sender, EventArgs e) {
			if(!SupressUncheckCaption) Caption.IsChecked = false;
			if(BarManagerContainer.Content != null) {
				FilterMenu.ItemLinks.Clear();
				BarManagerContainer.Content = null;
			}
		}
		protected virtual BarManager CreatePrivateBarManager() {
			return new BarManager();
		}
		protected virtual void ShowFilterMenu() {
			if(Gallery == null || BarManagerContainer == null)
				return;
			BarManager manager = BarManagerHelper.GetOrFindBarManager(this);
			if(manager == null) {
				manager = CreatePrivateBarManager();
				BarManagerContainer.Content = manager;
			}
			FilterMenu.ItemLinks.Clear();
			foreach(GalleryItemGroup group in Gallery.Groups) {
				FilterMenu.ItemLinks.Add(CreateFilterMenuItem(manager, group));
			}
			FilterMenu.ItemLinks.Add(CreateFilterMenuSeparator(manager));
			FilterMenu.ItemLinks.Add(CreateShowAllMenuItem(manager));
			FilterMenu.IsBranchHeader = true;
			FilterMenu.ItemClickBehaviour = PopupItemClickBehaviour.ClosePopup;
			Dispatcher.BeginInvoke(new Action(() => {
				FilterMenu.ShowPopup(Caption);
			}));
		}
		protected virtual BarItemLinkBase CreateFilterMenuItem(BarManager manager, GalleryItemGroup group) {
			BarCheckItemLink link = new BarCheckItemLink();
			link.IsPrivate = true;
			BarCheckItem item = new BarCheckItem() { Content = group.Caption, IsPrivate = true, IsChecked = group.IsVisible, IsEnabled = group.IsEnabled, Tag = group };
			item.CheckedChanged += new ItemClickEventHandler(OnFilterMenuItemCheckedChanged);
			link.Link(item);
			return link;
		}
		void OnFilterMenuItemCheckedChanged(object sender, ItemClickEventArgs e) {
			BarCheckItem item = sender as BarCheckItem;
			if(item == null) return;
			GalleryItemGroup group = item.Tag as GalleryItemGroup;
			if(group == null) {
				SetGroupsVisibility((bool)item.IsChecked);
				return;
			}
			group.IsVisible = (bool)item.IsChecked;
		}
		protected virtual BarItemLinkBase CreateFilterMenuSeparator(BarManager manager) {			
			return new BarItemLinkSeparator() { IsPrivate = true };
		}
		bool IsAllGroupsVisible() {
			foreach(GalleryItemGroup group in Gallery.Groups) {
				if(!group.IsVisible) return false;
			}
			return true;
		}
		void SetGroupsVisibility(bool isVisible) {
			foreach(GalleryItemGroup group in Gallery.Groups) group.IsVisible = isVisible;
		}
		protected virtual BarItemLinkBase CreateShowAllMenuItem(BarManager manager) {
			BarCheckItemLink link = new BarCheckItemLink();
			link.IsPrivate = true;
			BarCheckItem item = new BarCheckItem() { Content = BarsLocalizer.GetString(BarsStringId.GalleryControlFilterMenu_ShowAllItemCaption), IsPrivate = true, IsChecked = IsAllGroupsVisible() };				   
			item.CheckedChanged += new ItemClickEventHandler(OnFilterMenuItemCheckedChanged);
			link.Link(item);
			return link;
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("GalleryControlGallery")]
#endif
		public Gallery Gallery {
			get { return (Gallery)GetValue(GalleryProperty); }
			set { SetValue(GalleryProperty, value); }
		}
		INavigationElement INavigationOwner.BoundElement { get { return null; }  }
		Orientation INavigationOwner.Orientation { get { return Orientation.Horizontal; } }
		NavigationKeys INavigationOwner.NavigationKeys { get { return NavigationKeys.All; } }
		KeyboardNavigationMode INavigationOwner.NavigationMode { get { return KeyboardNavigationMode.Cycle; } }
		readonly NavigationManager navigationManager;
		protected internal bool AllowCyclicNavigation { get; set; }
		NavigationManager INavigationOwner.NavigationManager { get { return navigationManager; } }
		IList<INavigationElement> INavigationOwner.Elements { get { return GetNavigationElements(); } }
		bool INavigationOwner.CanEnterMenuMode { get { return false; } }
		IBarsNavigationSupport IBarsNavigationSupport.Parent { get { return GetNavigationParent(); } }
		int IBarsNavigationSupport.ID { get { return Gallery.Return(x => x.GetHashCode(), GetHashCode); } }
		bool IBarsNavigationSupport.IsSelectable { get { return false; } }
		bool IBarsNavigationSupport.ExitNavigationOnMouseUp { get { return false; } }
		bool IBarsNavigationSupport.ExitNavigationOnFocusChangedWithin { get { return false; } }
		protected virtual IList<INavigationElement> GetNavigationElements() {
			var emptyList = new List<INavigationElement>();
			var gc = GroupsControl;
			if (gc == null)
				return emptyList;
			var groupControls = Gallery.Groups.Select(gc.ItemContainerGenerator.ContainerFromItem).OfType<GalleryItemGroupControl>();
			var itemControls = groupControls.SelectMany(x => x.Group.Items.Select(x.ItemContainerGenerator.ContainerFromItem)).OfType<GalleryItemControl>();
			var orderedItemControls = itemControls
				.OrderBy(x => Gallery.Groups.IndexOf(x.Item.Group))
				.ThenBy(x => x.DesiredRowIndex)
				.ThenBy(x => x.DesiredColIndex);
			return orderedItemControls.OfType<INavigationElement>().ToList();
		}
		protected virtual IBarsNavigationSupport GetNavigationParent() {
			return Native.TreeHelper.GetParent<IBarsNavigationSupport>(this, x => true, false, false);
		}
		public override void OnApplyTemplate() {
			UnsubscribeTemplateEvents();
			base.OnApplyTemplate();
			BarManagerContainer = GetTemplateChild("PART_StandaloneBarManagerContainer") as ContentControl;
			ScrollHost = GetTemplateChild("PART_ScrollHost") as GalleryGroupsViewer;
			GroupsControl = GetTemplateChild("PART_Groups") as GalleryItemGroupsControl;
			Caption = GetTemplateChild("PART_Caption") as ToggleButton;
			VerticalScrollBar = GetTemplateChild("PART_VerticalScrollBar") as ScrollBar;
			UpdateVerticalScrollBarVisibility();
			SubscribeTemplateEvents();
			UpdateVerticalScrollBarStyle();
			UpdateVerticalScrollBarViewPortSize();
		}
		protected virtual void UnsubscribeTemplateEvents() {
			if(GroupsControl != null) {
				GroupsControl.GalleryControl = null;
			}
			if(Caption != null) {
				Caption.MouseEnter -= OnCaptionMouseEnter;
				Caption.MouseLeave -= OnCaptionMouseLeave;
				Caption.Click -= OnCaptionClick;
			}
			if(VerticalScrollBar != null) {
				VerticalScrollBar.Scroll -= new ScrollEventHandler(OnVerticalScroll);								
			}
			if(ScrollHost != null) {
				ScrollHost.ScrollableSizeChanged -= OnScrollHostScrollableSizeChanged;
				ScrollHost.ViewportSizeChanged -= OnScrollHostViewportSizeChanged;
			}
		}
		protected virtual void SubscribeTemplateEvents() {
			if(GroupsControl != null) {
				GroupsControl.GalleryControl = this;
			}
			if(Caption != null) {
				Caption.MouseEnter += new MouseEventHandler(OnCaptionMouseEnter);
				Caption.MouseLeave += new MouseEventHandler(OnCaptionMouseLeave);
				Caption.Click += new RoutedEventHandler(OnCaptionClick);
			}
			if(VerticalScrollBar != null) {
				VerticalScrollBar.Scroll += new ScrollEventHandler(OnVerticalScroll);
			}
			if(ScrollHost != null) {
				ScrollHost.ScrollableSizeChanged += new EventHandler(OnScrollHostScrollableSizeChanged);
				ScrollHost.ViewportSizeChanged += new EventHandler(OnScrollHostViewportSizeChanged);
			}
		}
		void OnScrollHostViewportSizeChanged(object sender, EventArgs e) {
			UpdateScrollBarsPosition();
			UpdateVerticalScrollBarViewPortSize();
		}
		void OnScrollHostScrollableSizeChanged(object sender, EventArgs e) {
			UpdateScrollBarsPosition();
		}
		protected virtual void UpdateScrollBarsPosition() {
			if(ScrollHost == null || VerticalScrollBar == null) return;
			VerticalScrollBar.Maximum = ScrollHost.GetMaxContentVerticalOffsetValue();
			VerticalScrollBar.SmallChange = 100;
			if(ScrollHost.ScrollableSize.Height - ScrollHost.ViewportSize.Height < ScrollHost.ContentVerticalOffset) {
				ScrollHost.ScrollToVerticalOffset(VerticalScrollBar.Maximum);
			}
			if(VerticalScrollBar.Maximum == 0) {
				ScrollHost.ScrollToVerticalOffset(0);
				VerticalScrollBar.Value = 0;
			}			 
		}
		protected virtual void OnVerticalScroll(object sender, ScrollEventArgs e) {
			StartScrollingAnimation(0, e.NewValue);
		}
		internal void OnCaptionClick(object sender, RoutedEventArgs e) {
			if(Caption.IsChecked == true) ShowFilterMenu();
			else FilterMenu.IsOpen = false;
		}
		void OnCaptionMouseLeave(object sender, MouseEventArgs e) {
			SupressUncheckCaption = false;
		}
		void OnCaptionMouseEnter(object sender, MouseEventArgs e) {
			SupressUncheckCaption = true;
		}
		protected override void OnMouseWheel(MouseWheelEventArgs e) {
			e.Handled = true;
			if(e.Delta == 0) return;
			ScrollToVerticalOffsetCore(ActualVerticalOffset - e.Delta / Math.Abs(e.Delta) * ScrollHost.GetRowsHeight(1), false);
		}
		Rect GetItemBoundsInScrollHost(FrameworkElement item) {
			if(!ScrollHost.IsAncestorOf(item)) return new Rect();
			GeneralTransform transform = item.TransformToAncestor(ScrollHost);
			return transform.TransformBounds(new Rect(new Size(item.ActualWidth, item.ActualHeight)));
		}
		protected double GetContentVerticalOffset() {
			return ScrollHost.ContentVerticalOffset;
		}		
		void INavigationOwner.OnAddedToSelection() { }
		void INavigationOwner.OnRemovedFromSelection(bool destroying) { }
	}
	[ContentProperty("Child")]
	public class GalleryGroupsViewer : Panel {
		#region static
		private static readonly object scrollableSizeChangedEventHandler;
		private static readonly object viewportSizeChangedEventHandler;
		public static readonly DependencyProperty ChildProperty;
		public static readonly DependencyProperty ViewportSizeProperty;
		protected static readonly DependencyPropertyKey ViewportSizePropertyKey;
		static GalleryGroupsViewer() {
			ChildProperty = DependencyPropertyManager.Register("Child", typeof(GalleryItemGroupsControl), typeof(GalleryGroupsViewer), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsMeasure, new PropertyChangedCallback(OnChildPropertyChanged)));
			ViewportSizePropertyKey = DependencyPropertyManager.RegisterReadOnly("ViewportSize", typeof(Size), typeof(GalleryGroupsViewer), new FrameworkPropertyMetadata(new Size(), new PropertyChangedCallback(OnViewportSizePropertyChanged)));
			ViewportSizeProperty = ViewportSizePropertyKey.DependencyProperty;
			scrollableSizeChangedEventHandler = new object();
			viewportSizeChangedEventHandler = new object();
		}
		static void OnChildPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e) {
			((GalleryGroupsViewer)obj).OnChildChanged(e.OldValue as GalleryItemGroupsControl);
		}
		static void OnViewportSizePropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e) {
			Size oldValue = (Size)e.OldValue;
			Size newValue = (Size)e.NewValue;
			if(oldValue.Height != newValue.Height || oldValue.Width != newValue.Width)
				((GalleryGroupsViewer)obj).OnViewportSizeChanged(oldValue);
		}
		#endregion
		#region dep props
		public GalleryItemGroupsControl Child {
			get { return (GalleryItemGroupsControl)GetValue(ChildProperty); }
			set { SetValue(ChildProperty, value); }
		}
		public Size ViewportSize {
			get { return (Size)GetValue(ViewportSizeProperty); }
			protected set { this.SetValue(ViewportSizePropertyKey, value); }
		}
		#endregion
		public GalleryGroupsViewer() {
			ClipToBounds = true;
		}
		double contentHorizontalOffsetCore = 0;
		double actualContentHorizontalOffset = 0;
		internal double contentVerticalOffsetCore = 0;
		double actualContentVerticalOffset = 0;
		public double ContentHorizontalOffset {
			get { return actualContentHorizontalOffset; }
		}
		public double ContentVerticalOffset {
			get { return actualContentVerticalOffset; }
		}
		protected internal double AvailableHeight { get; protected set; }
		Size scrollableSizeCore = new Size();
		public Size ScrollableSize {
			get {
				return scrollableSizeCore;
			}
			protected set {
				if(value.Width != ScrollableSize.Width || value.Height != ScrollableSize.Height) {
					scrollableSizeCore = value;
					OnScrollableSizeChanged();
				}
				else {
					scrollableSizeCore = value;
				}
			}
		}
		EventHandlerList events;
		protected EventHandlerList Events {
			get {
				if(events == null)
					events = new EventHandlerList();
				return events;
			}
		}
		public event EventHandler ScrollableSizeChanged {
			add { Events.AddHandler(scrollableSizeChangedEventHandler, value); }
			remove { Events.RemoveHandler(scrollableSizeChangedEventHandler, value); }
		}
		public event EventHandler ViewportSizeChanged {
			add { Events.AddHandler(viewportSizeChangedEventHandler, value); }
			remove { Events.RemoveHandler(viewportSizeChangedEventHandler, value); }
		}
		protected void RaiseEventByHandler(object eventHandler, EventArgs args) {
			EventHandler h = Events[eventHandler] as EventHandler;
			if(h != null) h(this, args);
		}
		protected virtual void OnScrollableSizeChanged() {
			RaiseEventByHandler(scrollableSizeChangedEventHandler, new EventArgs());
		}
		protected virtual void OnViewportSizeChanged(Size oldValue) {
			RaiseEventByHandler(viewportSizeChangedEventHandler, new EventArgs());
		}
		protected virtual void OnChildChanged(GalleryItemGroupsControl oldValue) {
			if(oldValue != null) Children.Remove(oldValue);
			if(Child != null) Children.Add(Child);
		}
		protected override Size MeasureOverride(Size availableSize) {
			AvailableHeight = availableSize.Height;
			Size retVal = availableSize;			
			if(Child != null) {
				Child.Measure(new Size(retVal.Width, double.PositiveInfinity));
				if(Child.GalleryControl != null && Child.GalleryControl.DesiredRowCount != 0) {
					retVal.Height = GetRowsHeight(Child.GalleryControl.DesiredRowCount);
				}
				else {
					retVal.Height = Child.DesiredSize.Height;
				}
				retVal.Width = Child.DesiredSize.Width;
				ScrollableSize = new Size(Child.DesiredSize.Width, Child.DesiredSize.Height);
			}
			if(double.IsPositiveInfinity(retVal.Width)) retVal.Width = 0;
			if(double.IsPositiveInfinity(retVal.Height)) retVal.Height = 0;
			if(!double.IsPositiveInfinity(availableSize.Width)) retVal.Width = Math.Min(availableSize.Width, retVal.Width);
			if(!double.IsPositiveInfinity(availableSize.Height)) retVal.Height = Math.Min(availableSize.Height, retVal.Height);						
			return retVal;
		}
		public virtual int GetFirstVisibleRowIndex() {
			if(Child == null || Child.Items.Count == 0) return 0;
			double height = 0;
			int rowCount = 0;
			for(int i = 0; i < Child.Items.Count; i++) {
				GalleryItemGroupControl group = Child.GetItem(i);
				if (group == null || group.Group == null || !group.Group.IsVisible) continue;
				for(int j = 0; j < group.Items.Count; j++) {
					GalleryItemControl item = group.GetItem(j);
					if(item ==null || item.Item==null || !item.Item.IsVisible) continue;
					if(item.DesiredStartOfLine) {
						if(height + group.GetCaptionOffset() + (item.DesiredRowIndex + 1) * GetItemDesiredSize(item).Height > Math.Round(ContentVerticalOffset, 2)) return rowCount;
						rowCount++;
					}
				}
				height += group.DesiredSize.Height;
			}
			return 0;
		}
		protected Rect GetLineBounds(double groupVerticalOffset, GalleryItemGroupControl group, GalleryItemControl item) {
			return new Rect(new Point(0, groupVerticalOffset + group.GetCaptionOffset() + item.DesiredRowIndex * GetItemDesiredSize(item).Height), GetItemDesiredSize(item));
		}
		public virtual int GetLastVisibleRowIndex() {
			if(Child == null || Child.Items.Count == 0)
				return 0;
			double height = 0;
			int rowCount = 0;
			int lastVisibleRowIndex = -1;
			for(int i = 0; i < Child.Items.Count; i++) {
				GalleryItemGroupControl group = Child.GetItem(i);
				if(group==null || group.Group==null || !group.Group.IsVisible)
					continue;
				for(int j = 0; j < group.Items.Count; j++) {
					GalleryItemControl item = group.GetItem(j);
					if (item == null || item.Item == null || !item.Item.IsVisible)
						continue;
					if(item.DesiredStartOfLine) {
						Rect lineBounds = GetLineBounds(height, group, item);
						if(lineBounds.Top >= ContentVerticalOffset && lineBounds.Top <= ContentVerticalOffset + ViewportSize.Height) {
							lastVisibleRowIndex = rowCount;
						}
						else if(lineBounds.Top + lineBounds.Height >= ContentVerticalOffset && lineBounds.Top + lineBounds.Height <= ContentVerticalOffset + ViewportSize.Height) {
							lastVisibleRowIndex = rowCount;
						}
						else if(lineBounds.Top < ContentVerticalOffset && lineBounds.Top + lineBounds.Height > ContentVerticalOffset + ViewportSize.Height) {
							lastVisibleRowIndex = rowCount;
						}
						if(lineBounds.Top + lineBounds.Height >= ContentVerticalOffset + ViewportSize.Height) {
							return lastVisibleRowIndex;
						}
						rowCount++;
					}
				}
				height += group.DesiredSize.Height;
			}
			return lastVisibleRowIndex;
		}
		public virtual double GetColsWidth(int colCount) {
			if(Child == null || Child.Items.Count == 0 || colCount == 0) return 0;
			for(int i = 0; i < Child.Items.Count; i++) {
				GalleryItemGroupControl group = Child.GetItem(i);
				if(!group.Group.IsVisible) continue;
				for(int j = 0; j < group.Items.Count; j++) {
					GalleryItemControl item = group.GetItem(j);
					if(item == null || item.Item == null || !item.Item.IsVisible) continue;
					return GetItemDesiredSize(item).Width * colCount;
				}
			}
			return 0;
		}
		public virtual int GetRowCount() {
			if(Child == null || Child.Items.Count == 0) return 0;
			int actualRowCount = 0;
			for(int i = 0; i < Child.Items.Count; i++) {
				GalleryItemGroupControl group = Child.GetItem(i);
				if(!group.Group.IsVisible)
					continue;
				actualRowCount += group.GetRowCount();
			}
			return actualRowCount;
		}
		public virtual int GetColCount() {
			if(Child == null || Child.Items.Count == 0) return 0;
			int colCount = 0;
			for(int i = 0; i < Child.Items.Count; i++) {
				GalleryItemGroupControl group = Child.GetItem(i);
				if(!group.Group.IsVisible)
					continue;
				for(int j = 0; j < group.Items.Count; j++) {
					GalleryItemControl item = group.GetItem(j);
					if(!item.Item.IsVisible)
						continue;
					colCount = Math.Max(colCount, item.DesiredColIndex + 1);
				}
			}
			return colCount;
		}
		Size GetItemDesiredSize(GalleryItemControl item) {
			if(Child.GalleryControl == null || Child.GalleryControl.Gallery == null)
				return new Size();
			var result = Child.GalleryControl.Gallery.ItemSize;
			if (double.IsNaN(result.Width) || double.IsNaN(result.Height)) {
				result = item.DesiredSize;
			}
			if (item.Gallery.ItemAutoHeight && GalleryHelper.GetIsInRibbonControl(Child.GalleryControl)) {
				result.Height = ViewportSize.Height;
			}
			return result;
		}
		public virtual double GetRowsHeight(int rowCount) {
			if(Child == null || Child.Items.Count == 0 || rowCount == 0)
				return 0;
			int actualRowCount = 0;
			double height = 0;
			for(int i = 0; i < Child.Items.Count; i++) {
				GalleryItemGroupControl group = Child.GetItem(i);
				if(group == null || !group.Group.IsVisible) continue;
				for(int j = 0; j < group.Items.Count; j++) {
					GalleryItemControl item = group.GetItem(j);
					if(item == null || item.Item == null || !item.Item.IsVisible) continue;
					if(item.DesiredStartOfLine) {
						actualRowCount++;
						if(actualRowCount == rowCount) {
							height += group.GetCaptionOffset();
							height += (item.DesiredRowIndex + 1) * GetItemDesiredSize(item).Height;
							return height;
						}
					}
				}
				height += group.DesiredSize.Height;
			}
			return height;
		}
		public virtual double GetItemVerticalOffset(GalleryItem item) {
			if(Child == null || Child.Items.Count == 0 || item == null)
				return 0;
			double height = 0;
			for(int i = 0; i < Child.Items.Count; i++) {
				GalleryItemGroupControl groupControl = Child.GetItem(i);
				if (groupControl == null) return 0d;
				for(int j = 0; j < groupControl.Items.Count; j++) {
					GalleryItemControl itemControl = groupControl.GetItem(j);
					if (itemControl == null) return 0d;
					if(itemControl.Item == item)
						return height + groupControl.GetCaptionOffset() + itemControl.DesiredRowIndex * GetItemDesiredSize(itemControl).Height;
					if(!itemControl.Item.IsVisible || !groupControl.Group.IsVisible) continue;				   
				}
				height += groupControl.DesiredSize.Height;
			}
			return 0;
		}
		public virtual double GetGroupVerticalOffset(GalleryItemGroup group) {
			if(Child == null || Child.Items.Count == 0 || group == null) return 0;
			double verticalOffset = 0;
			for(int i = 0; i < Child.Items.Count; i++) {
				GalleryItemGroupControl groupControl = Child.GetItem(i);
				if(groupControl.Group == group) {
					return verticalOffset;
				}
				verticalOffset += groupControl.DesiredSize.Height;
			}
			return verticalOffset;
		}
		protected override Size ArrangeOverride(Size finalSize) {
			if(ViewportSize.Height != finalSize.Height) {
				InvalidateArrangeForAllItems();
			}
			ViewportSize = finalSize;
			if(Child != null) {
				actualContentHorizontalOffset = contentHorizontalOffsetCore;
				actualContentVerticalOffset = contentVerticalOffsetCore;
				double actualWidth = Math.Max(Child.DesiredSize.Width, finalSize.Width);
				Child.Arrange(new Rect(-actualContentHorizontalOffset, -actualContentVerticalOffset, actualWidth, Child.DesiredSize.Height));
				return finalSize;
			}
			return finalSize;
		}
		public virtual double GetMaxContentVerticalOffsetValue() {
			return Math.Max(ScrollableSize.Height - ViewportSize.Height, 0);
		}
		public void ScrollToVerticalOffset(double offset) {
			if(Child == null || Child.GalleryControl == null || Child.GalleryControl.Gallery == null)
				return;
			contentVerticalOffsetCore = Math.Max(Math.Min(offset, GetMaxContentVerticalOffsetValue()), 0);			
			if(double.IsNaN(Child.GalleryControl.Gallery.ItemSize.Width) || double.IsNaN(Child.GalleryControl.Gallery.ItemSize.Height)) {
				InvalidateArrange();
			}
			else {
				InvalidateArrangeForAllItems();
				InvalidateArrange();
			}
		}
		void InvalidateArrangeForAllItems() {
			for(int i = 0; i < Child.Items.Count; i++) {
				GalleryItemGroupControl group = Child.GetItem(i);
				if(group == null || !group.Group.IsVisible) continue;
				for(int j = 0; j < group.Items.Count; j++) {
					GalleryItemControl item = group.GetItem(j);
					if(item != null) ((UIElement)LayoutHelper.GetParent(item)).InvalidateArrange();
				}
			}
		}
		internal bool IsItemFullyVisible(GalleryItemControl itemControl) {
			Rect bounds = GetItemPosInScrollViewer(itemControl);
			Rect parentBounds = new Rect(0, 0, ViewportSize.Width, ViewportSize.Height);
			return parentBounds.Contains(new Point(bounds.X, bounds.Y)) && parentBounds.Contains(new Point(bounds.X + bounds.Width, bounds.Y))
				&& parentBounds.Contains(new Point(bounds.X + bounds.Width, bounds.Y + bounds.Height)) && parentBounds.Contains(new Point(bounds.X, bounds.Y + bounds.Height));			
		}
		internal Rect GetItemPosInScrollViewer(GalleryItemControl itemControl) {
			if(!LayoutHelper.IsChildElement(this, itemControl)) return new Rect();			
			GeneralTransform transformParent = itemControl.TransformToVisual(this);
			return transformParent.TransformBounds(new Rect(0, 0, itemControl.ActualWidth, itemControl.ActualHeight));
		}
	}
	public class SplitLayoutPanel : Panel {
		#region static
		public static readonly DependencyProperty Content1Property;
		public static readonly DependencyProperty Content1LocationProperty;
		public static readonly DependencyProperty Content2Property;
		public static readonly DependencyProperty SingleContent1MarginProperty;
		public static readonly DependencyProperty TopContent1MarginProperty;
		public static readonly DependencyProperty LeftContent1MarginProperty;
		public static readonly DependencyProperty RightContent1MarginProperty;
		public static readonly DependencyProperty BottomContent1MarginProperty;
		public static readonly DependencyProperty SingleContent2MarginProperty;
		public static readonly DependencyProperty TopContent2MarginProperty;
		public static readonly DependencyProperty LeftContent2MarginProperty;
		public static readonly DependencyProperty RightContent2MarginProperty;
		public static readonly DependencyProperty BottomContent2MarginProperty;
		public static readonly DependencyProperty MaximizeContent1Property;
		static SplitLayoutPanel() {
			Content1Property = DependencyPropertyManager.Register("Content1", typeof(UIElement), typeof(SplitLayoutPanel),
				new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnContent1PropertyChanged)));
			Content1LocationProperty = DependencyPropertyManager.Register("Content1Location", typeof(Dock), typeof(SplitLayoutPanel),
				new FrameworkPropertyMetadata(Dock.Left, new PropertyChangedCallback(OnContent1LocationPropertyChanged)));
			Content2Property = DependencyPropertyManager.Register("Content2", typeof(UIElement), typeof(SplitLayoutPanel),
				new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnContent2PropertyChanged)));
			SingleContent1MarginProperty = DependencyPropertyManager.Register("SingleContent1Margin", typeof(Thickness), typeof(SplitLayoutPanel),
				new FrameworkPropertyMetadata(new Thickness(), new PropertyChangedCallback(OnMarginsPropertyChanged)));
			TopContent1MarginProperty = DependencyPropertyManager.Register("TopContent1Margin", typeof(Thickness), typeof(SplitLayoutPanel),
				new FrameworkPropertyMetadata(new Thickness(), new PropertyChangedCallback(OnMarginsPropertyChanged)));
			BottomContent1MarginProperty = DependencyPropertyManager.Register("BottomContent1Margin", typeof(Thickness), typeof(SplitLayoutPanel),
				new FrameworkPropertyMetadata(new Thickness(), new PropertyChangedCallback(OnMarginsPropertyChanged)));
			LeftContent1MarginProperty = DependencyPropertyManager.Register("LeftContent1Margin", typeof(Thickness), typeof(SplitLayoutPanel),
				new FrameworkPropertyMetadata(new Thickness(), new PropertyChangedCallback(OnMarginsPropertyChanged)));
			RightContent1MarginProperty = DependencyPropertyManager.Register("RightContent1Margin", typeof(Thickness), typeof(SplitLayoutPanel),
				new FrameworkPropertyMetadata(new Thickness(), new PropertyChangedCallback(OnMarginsPropertyChanged)));
			SingleContent2MarginProperty = DependencyPropertyManager.Register("SingleContent2Margin", typeof(Thickness), typeof(SplitLayoutPanel),
				new FrameworkPropertyMetadata(new Thickness(), new PropertyChangedCallback(OnMarginsPropertyChanged)));
			TopContent2MarginProperty = DependencyPropertyManager.Register("TopContent2Margin", typeof(Thickness), typeof(SplitLayoutPanel),
				new FrameworkPropertyMetadata(new Thickness(), new PropertyChangedCallback(OnMarginsPropertyChanged)));
			BottomContent2MarginProperty = DependencyPropertyManager.Register("BottomContent2Margin", typeof(Thickness), typeof(SplitLayoutPanel),
				new FrameworkPropertyMetadata(new Thickness(), new PropertyChangedCallback(OnMarginsPropertyChanged)));
			LeftContent2MarginProperty = DependencyPropertyManager.Register("LeftContent2Margin", typeof(Thickness), typeof(SplitLayoutPanel),
				new FrameworkPropertyMetadata(new Thickness(), new PropertyChangedCallback(OnMarginsPropertyChanged)));
			RightContent2MarginProperty = DependencyPropertyManager.Register("RightContent2Margin", typeof(Thickness), typeof(SplitLayoutPanel),
				new FrameworkPropertyMetadata(new Thickness(), new PropertyChangedCallback(OnMarginsPropertyChanged)));
			MaximizeContent1Property = DependencyPropertyManager.Register("MaximizeContent1", typeof(bool), typeof(SplitLayoutPanel),
				new FrameworkPropertyMetadata(false, new PropertyChangedCallback(OnMaximizeContent1PropertyChanged)));
		}
		static protected void OnContent2PropertyChanged(DependencyObject o, DependencyPropertyChangedEventArgs e) {
			((SplitLayoutPanel)o).OnContent2Changed(e.OldValue as UIElement);
		}
		static protected void OnContent1PropertyChanged(DependencyObject o, DependencyPropertyChangedEventArgs e) {
			((SplitLayoutPanel)o).OnContent1Changed(e.OldValue as UIElement);
		}
		static protected void OnContent1LocationPropertyChanged(DependencyObject o, DependencyPropertyChangedEventArgs e) {
			((SplitLayoutPanel)o).InvalidateMeasure();
		}
		static protected void OnMarginsPropertyChanged(DependencyObject o, DependencyPropertyChangedEventArgs e) {
			((SplitLayoutPanel)o).InvalidateMeasure();
		}
		static protected void OnMaximizeContent1PropertyChanged(DependencyObject o, DependencyPropertyChangedEventArgs e) {
			((SplitLayoutPanel)o).InvalidateArrange();
		}
		#endregion
		#region dep props
		public UIElement Content2 {
			get { return (UIElement)GetValue(Content2Property); }
			set { SetValue(Content2Property, value); }
		}
		public UIElement Content1 {
			get { return (UIElement)GetValue(Content1Property); }
			set { SetValue(Content1Property, value); }
		}
		public Dock Content1Location {
			get { return (Dock)GetValue(Content1LocationProperty); }
			set { SetValue(Content1LocationProperty, value); }
		}
		public Thickness SingleContent1Margin {
			get { return (Thickness)GetValue(SingleContent1MarginProperty); }
			set { SetValue(SingleContent1MarginProperty, value); }
		}
		public Thickness TopContent1Margin {
			get { return (Thickness)GetValue(TopContent1MarginProperty); }
			set { SetValue(TopContent1MarginProperty, value); }
		}
		public Thickness LeftContent1Margin {
			get { return (Thickness)GetValue(LeftContent1MarginProperty); }
			set { SetValue(LeftContent1MarginProperty, value); }
		}
		public Thickness RightContent1Margin {
			get { return (Thickness)GetValue(RightContent1MarginProperty); }
			set { SetValue(RightContent1MarginProperty, value); }
		}
		public Thickness BottomContent1Margin {
			get { return (Thickness)GetValue(BottomContent1MarginProperty); }
			set { SetValue(BottomContent1MarginProperty, value); }
		}
		public Thickness SingleContent2Margin {
			get { return (Thickness)GetValue(SingleContent2MarginProperty); }
			set { SetValue(SingleContent2MarginProperty, value); }
		}
		public Thickness TopContent2Margin {
			get { return (Thickness)GetValue(TopContent2MarginProperty); }
			set { SetValue(TopContent2MarginProperty, value); }
		}
		public Thickness LeftContent2Margin {
			get { return (Thickness)GetValue(LeftContent2MarginProperty); }
			set { SetValue(LeftContent2MarginProperty, value); }
		}
		public Thickness RightContent2Margin {
			get { return (Thickness)GetValue(RightContent2MarginProperty); }
			set { SetValue(RightContent2MarginProperty, value); }
		}
		public Thickness BottomContent2Margin {
			get { return (Thickness)GetValue(BottomContent2MarginProperty); }
			set { SetValue(BottomContent2MarginProperty, value); }
		}
		public bool MaximizeContent1 {
			get { return (bool)GetValue(MaximizeContent1Property); }
			set { SetValue(MaximizeContent1Property, value); }
		}
		#endregion
		protected virtual void OnContent2Changed(UIElement oldValue) {
			if(oldValue != null)
				Children.Remove(oldValue);
			if(Content2 != null)
				Children.Add(Content2);
		}
		protected virtual void OnContent1Changed(UIElement oldValue) {
			if(oldValue != null)
				Children.Remove(oldValue);
			if(Content1 != null)
				Children.Add(Content1);
		}
		Thickness ActualContent1Margin {
			get {
				if(Content1 == null || Content1.Visibility == System.Windows.Visibility.Collapsed) return new Thickness();
				if(Content2 == null || Content2.Visibility == System.Windows.Visibility.Collapsed) return SingleContent1Margin;
				switch(Content1Location) {
					case Dock.Bottom:
						return BottomContent1Margin;
					case Dock.Left:
						return LeftContent1Margin;
					case Dock.Right:
						return RightContent1Margin;
					case Dock.Top:
						return TopContent1Margin;
				}
				return new Thickness();
			}
		}
		Thickness ActualContent2Margin {
			get {
				if(Content2 == null || Content2.Visibility == System.Windows.Visibility.Collapsed) return new Thickness();
				if(Content1 == null || Content1.Visibility == System.Windows.Visibility.Collapsed) return SingleContent2Margin;
				switch(Content1Location) {
					case Dock.Bottom:
						return TopContent2Margin;
					case Dock.Left:
						return RightContent2Margin;
					case Dock.Right:
						return LeftContent2Margin;
					case Dock.Top:
						return BottomContent2Margin;
				}
				return new Thickness();
			}
		}
		Size CalcMaxDesiredSize() {
			Size glyphSize = new Size(ActualContent2Margin.Left + ActualContent2Margin.Right, ActualContent2Margin.Top + ActualContent2Margin.Bottom);
			Size contentSize = new Size(ActualContent1Margin.Left + ActualContent1Margin.Right, ActualContent1Margin.Top + ActualContent1Margin.Bottom);
			if(Content2 != null) {
				glyphSize.Width += Content2.DesiredSize.Width;
				glyphSize.Height += Content2.DesiredSize.Height;
			}
			if(Content1 != null) {
				contentSize.Width += Content1.DesiredSize.Width;
				contentSize.Height += Content1.DesiredSize.Height;
			}
			if(Content1Location == Dock.Top || Content1Location == Dock.Bottom) {
				return new Size(Math.Max(glyphSize.Width, contentSize.Width), glyphSize.Height + contentSize.Height);
			}
			return new Size(glyphSize.Width + contentSize.Width, Math.Max(glyphSize.Height, contentSize.Height));
		}
		protected virtual void ArrangeBottom(UIElement element1, UIElement element2, Thickness element1Margin, Thickness element2Margin, Size finalSize) {
			Rect pos = new Rect();
			if(element1 != null) {
				pos.X = element1Margin.Left;
				pos.Y = finalSize.Height - element1Margin.Bottom;
				pos.Width = Math.Max(0, finalSize.Width - element1Margin.Left - element1Margin.Right);
				if(element2 != null) {
					pos.Height = element1.DesiredSize.Height;
					pos.Y -= pos.Height;
				}
				else {
					pos.Height = Math.Max(0, finalSize.Height - element1Margin.Top - element1Margin.Bottom);
					pos.Y -= pos.Height;
				}
				element1.Arrange(pos);
				pos.Y -= element1Margin.Top;
			}
			if(element2 != null) {
				pos.Height = Math.Max(0, pos.Y - element2Margin.Top - element2Margin.Bottom);
				pos.Width = Math.Max(0, finalSize.Width - element2Margin.Left - element2Margin.Right);
				pos.X = element2Margin.Left;
				pos.Y -= element2Margin.Bottom + pos.Height;
				element2.Arrange(pos);
			}
		}
		protected virtual void ArrangeRight(UIElement element1, UIElement element2, Thickness element1Margin, Thickness element2Margin, Size finalSize) {
			Rect pos = new Rect();
			if(element1 != null) {
				pos.X = finalSize.Width - element1Margin.Right;
				pos.Y = element1Margin.Top;
				pos.Height = Math.Max(0, finalSize.Height - element1Margin.Top - element1Margin.Bottom);
				if(element2 != null) {
					pos.Width = element1.DesiredSize.Width;
				}
				else {
					pos.Width = Math.Max(0, finalSize.Width - element1Margin.Left - element1Margin.Right);
				}
				pos.X -= pos.Width;
				element1.Arrange(pos);
				pos.X -= element1Margin.Left;
			} else {
				pos.X = finalSize.Width;
			}
			if(element2 != null) {
				pos.Height = Math.Max(0, finalSize.Height - element2Margin.Top - element2Margin.Bottom);
				pos.Width = Math.Max(0, pos.X - element2Margin.Left - element2Margin.Right);
				pos.X -= element2Margin.Right + pos.Width;
				pos.Y = element2Margin.Top;
				element2.Arrange(pos);
			}
		}
		protected virtual void ArrangeLeft(UIElement element1, UIElement element2, Thickness element1Margin, Thickness element2Margin, Size finalSize) {
			Rect pos = new Rect();
			if(element1 != null) {
				pos.X = element1Margin.Left;
				pos.Y = element1Margin.Top;
				pos.Height = Math.Max(0, finalSize.Height - element1Margin.Top - element1Margin.Bottom);
				if(element2 != null) {
					pos.Width = element1.DesiredSize.Width;
				}
				else {
					pos.Width = Math.Max(0, finalSize.Width - element1Margin.Left - element1Margin.Right);
				}
				element1.Arrange(pos);
				pos.X += pos.Width + element1Margin.Right;
			}
			if(element2 != null) {
				pos.Height = Math.Max(0, finalSize.Height - element2Margin.Top - element2Margin.Bottom);
				pos.Width = Math.Max(0, finalSize.Width - pos.X - element2Margin.Left - element2Margin.Right);
				pos.X += element2Margin.Left;
				pos.Y = element2Margin.Top;
				element2.Arrange(pos);
			}
		}
		protected virtual void ArrangeTop(UIElement element1, UIElement element2, Thickness element1Margin, Thickness element2Margin, Size finalSize) {
			Rect pos = new Rect();
			if(element1 != null) {
				pos.X = element1Margin.Left;
				pos.Y = element1Margin.Top;
				pos.Width = Math.Max(0, finalSize.Width - element1Margin.Left - element1Margin.Right);
				if(element2 != null) {
					pos.Height = element1.DesiredSize.Height;
				}
				else {
					pos.Height = Math.Max(0, finalSize.Height - element1Margin.Top - element1Margin.Bottom);
				}
				element1.Arrange(pos);
				pos.Y += pos.Height + element1Margin.Bottom;
			}
			if(element2 != null) {
				pos.Height = Math.Max(0, finalSize.Height - pos.Y - element2Margin.Top - element2Margin.Bottom);
				pos.Width = Math.Max(0, finalSize.Width - element2Margin.Left - element2Margin.Right);
				pos.X = element2Margin.Left;
				pos.Y += element2Margin.Top;
				element2.Arrange(pos);
			}
		}
		protected override Size ArrangeOverride(Size finalSize) {
			UIElement element1 = null;
			UIElement element2 = null;
			Thickness element1Margin = new Thickness();
			Thickness element2Margin = new Thickness();
			Dock element1Position = Content1Location;
			if(MaximizeContent1) {
				element1 = Content2 == null ? null : Content2.Visibility != Visibility.Collapsed ? Content2 : null;
				element2 = Content1 == null ? null : Content1.Visibility != Visibility.Collapsed ? Content1 : null;
				element1Margin = ActualContent2Margin;
				element2Margin = ActualContent1Margin;
				switch(Content1Location) {
					case Dock.Bottom:
						element1Position = Dock.Top;
						break;
					case Dock.Top:
						element1Position = Dock.Bottom;
						break;
					case Dock.Left:
						element1Position = Dock.Right;
						break;
					case Dock.Right:
						element1Position = Dock.Left;
						break;
				}
			}
			else {
				element1 = Content1 == null ? null : Content1.Visibility != Visibility.Collapsed ? Content1 : null;
				element2 = Content2 == null ? null : Content2.Visibility != Visibility.Collapsed ? Content2 : null;				
				element1Margin = ActualContent1Margin;
				element2Margin = ActualContent2Margin;				
			}
			Size actualFinalSize = CalcMaxDesiredSize();
			actualFinalSize.Height = Math.Max(actualFinalSize.Height, finalSize.Height);
			actualFinalSize.Width = Math.Max(actualFinalSize.Width, finalSize.Width);
			switch(element1Position) {
				case Dock.Bottom:
					ArrangeBottom(element1, element2, element1Margin, element2Margin, actualFinalSize);
					break;
				case Dock.Top:
					ArrangeTop(element1, element2, element1Margin, element2Margin, actualFinalSize);
					break;
				case Dock.Left:
					ArrangeLeft(element1, element2, element1Margin, element2Margin, actualFinalSize);
					break;
				case Dock.Right:
					ArrangeRight(element1, element2, element1Margin, element2Margin, actualFinalSize);
					break;
			}
			return finalSize;
		}
		protected override Size MeasureOverride(Size availableSize) {
			UIElement element1 = null;
			UIElement element2 = null;
			Size element1Size = new Size();
			Size element2Size = new Size();
			if(MaximizeContent1) {
				element1 = Content2 == null ? null : Content2.Visibility != Visibility.Collapsed ? Content2 : null;
				element2 = Content1 == null ? null : Content1.Visibility != Visibility.Collapsed ? Content1 : null;
				element1Size = new Size(ActualContent2Margin.Left + ActualContent2Margin.Right, ActualContent2Margin.Top + ActualContent2Margin.Bottom);
				element2Size = new Size(ActualContent1Margin.Left + ActualContent1Margin.Right, ActualContent1Margin.Top + ActualContent1Margin.Bottom);			   
			}
			else {
				element1 = Content1 == null ? null : Content1.Visibility != Visibility.Collapsed ? Content1 : null;
				element2 = Content2 == null ? null : Content2.Visibility != Visibility.Collapsed ? Content2 : null;				
				element1Size = new Size(ActualContent1Margin.Left + ActualContent1Margin.Right, ActualContent1Margin.Top + ActualContent1Margin.Bottom);
				element2Size = new Size(ActualContent2Margin.Left + ActualContent2Margin.Right, ActualContent2Margin.Top + ActualContent2Margin.Bottom);				
			}
			Size element1AvailableSize = availableSize;
			if(!double.IsPositiveInfinity(element1AvailableSize.Width)) {
				element1AvailableSize.Width = Math.Max(0, element1AvailableSize.Width - element1Size.Width);
			}
			if(!double.IsPositiveInfinity(element1AvailableSize.Height)) {
				element1AvailableSize.Height = Math.Max(0, element1AvailableSize.Height - element1Size.Height);
			}
			if(element1 != null) {
				element1.Measure(element1AvailableSize);
				element1Size.Width += element1.DesiredSize.Width;
				element1Size.Height += element1.DesiredSize.Height;
			}
			Size element2AvailableSize = availableSize;
			if(Content1Location == Dock.Top || Content1Location == Dock.Bottom) {
				if(!double.IsPositiveInfinity(element2AvailableSize.Height)) {
					element2AvailableSize.Height = Math.Max(0, element2AvailableSize.Height - element1Size.Height - element2Size.Height);
				}
				if(!double.IsPositiveInfinity(element2AvailableSize.Width)) {
					element2AvailableSize.Width = Math.Max(0, element2AvailableSize.Width - element2Size.Width);
				}
				if(element2 != null) {
					element2.Measure(element2AvailableSize);
					element2Size.Width += element2.DesiredSize.Width;
					element2Size.Height += element2.DesiredSize.Height;
				}
				return new Size(Math.Max(element1Size.Width, element2Size.Width), element1Size.Height + element2Size.Height);
			}
			else {
				if(!double.IsPositiveInfinity(element2AvailableSize.Height)) {
					element2AvailableSize.Height = Math.Max(0, element2AvailableSize.Height - element2Size.Height);
				}
				if(!double.IsPositiveInfinity(element2AvailableSize.Width)) {
					element2AvailableSize.Width = Math.Max(0, element2AvailableSize.Width - element2Size.Width - element1Size.Width);
				}
				if(element2 != null) {
					element2.Measure(element2AvailableSize);
					element2Size.Width += element2.DesiredSize.Width;
					element2Size.Height += element2.DesiredSize.Height;
				}
				return new Size(element1Size.Width + element2Size.Width, Math.Max(element1Size.Height, element2Size.Height));
			}
		}
	}
}
