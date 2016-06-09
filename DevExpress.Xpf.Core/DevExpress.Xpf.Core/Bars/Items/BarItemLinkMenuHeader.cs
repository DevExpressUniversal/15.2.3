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
using System.Windows.Controls;
using System.Windows.Media;
using DevExpress.Xpf.Bars.Themes;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Utils;
using DevExpress.Xpf.Utils.Themes;
using DevExpress.Xpf.Editors.Helpers;
namespace DevExpress.Xpf.Bars {
	public class BarItemLinkMenuHeader : BarItemLink {
		#region static
		public static readonly DependencyProperty ItemsOrientationProperty;
		public static readonly DependencyProperty MinColCountProperty;
		static BarItemLinkMenuHeader() {
			ItemsOrientationProperty = DependencyPropertyManager.Register("ItemsOrientation", typeof(HeaderOrientation), typeof(BarItemLinkMenuHeader), new FrameworkPropertyMetadata(HeaderOrientation.Default, new PropertyChangedCallback(OnItemsOrientationPropertyChanged)));
			MinColCountProperty = DependencyPropertyManager.Register("MinColCount", typeof(int), typeof(BarItemLinkMenuHeader), new FrameworkPropertyMetadata(-1, new PropertyChangedCallback(OnMinColCountPropertyChanged)));
		}
		[Obsolete("This property is obsolete. Use the UserContent property instead.")]
		public object Content {
			get { return (object)GetValue(ContentProperty); }
			set { SetValue(ContentProperty, value); }
		}
		[Obsolete("This property is obsolete. Use the BarItemMenuHeader.ContentTemplate property instead.")]
		public DataTemplate ContentTemplate {
			get { return (DataTemplate)GetValue(ContentTemplateProperty); }
			set { SetValue(ContentTemplateProperty, value); }
		}
		internal WeakReference contentTemplateWeakReference = null;
		public static readonly DependencyProperty ContentTemplateProperty =
			DependencyPropertyManager.Register("ContentTemplate", typeof(DataTemplate), typeof(BarItemLinkMenuHeader), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnContentTemplatePropertyChanged)));
		public static readonly DependencyProperty ContentProperty =
			DependencyPropertyManager.Register("Content", typeof(object), typeof(BarItemLinkMenuHeader), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnContentPropertyChanged)));
		protected static void OnContentTemplatePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((BarItemLinkMenuHeader)d).contentTemplateWeakReference = new WeakReference(e.NewValue);
		}
		protected static void OnContentPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((BarItemLinkMenuHeader)d).UserContent = e.NewValue;
		}
		protected static void OnItemsOrientationPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((BarItemLinkMenuHeader)d).ExecuteActionOnLinkControls(lControl => ((BarItemLinkMenuHeaderControl)lControl).UpdateActualItemsOrientation());
		}
		protected static void OnMinColCountPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((BarItemLinkMenuHeader)d).ExecuteActionOnLinkControls(lControl => ((BarItemLinkMenuHeaderControl)lControl).UpdateActualMinColCount());
		}
		#endregion
		public BarItemMenuHeader MenuHeader { get { return (BarItemMenuHeader)Item; } }
		public int MinColCount {
			get { return (int)GetValue(MinColCountProperty); }
			set { SetValue(MinColCountProperty, value); }
		}
		public HeaderOrientation ItemsOrientation {
			get { return (HeaderOrientation)GetValue(ItemsOrientationProperty); }
			set { SetValue(ItemsOrientationProperty, value); }
		}
	}
	public enum CodedPanelElements {
		Glyph = 0x1,
		TemplatedGlyph = 0x2,
		Content = 0x4,
		Content2 = 0x8,
		Description = 0x10,
		KeyGesture = 0x20,
		TextSplitter = 0x40,
		FirstBorderControl = 0x80,
		SecondBorderControl = 0x100,
		ArrowControl = 0x200
	}
	public enum CodedPanelOwnerKind {
		Button
	}
	public enum SecondBorderPlacement { Arrow, ContentAndArrow }
	public class BarItemLayoutPanel : Panel {
		#region DependencyProperties
		public Thickness GlyphMargin {
			get { return (Thickness)GetValue(GlyphMarginProperty); }
			set { SetValue(GlyphMarginProperty, value); }
		}
		public Thickness ContentMargin {
			get { return (Thickness)GetValue(ContentMarginProperty); }
			set { SetValue(ContentMarginProperty, value); }
		}
		public Thickness Content2Margin {
			get { return (Thickness)GetValue(Content2MarginProperty); }
			set { SetValue(Content2MarginProperty, value); }
		}
		public Thickness DescriptionMargin {
			get { return (Thickness)GetValue(DescriptionMarginProperty); }
			set { SetValue(DescriptionMarginProperty, value); }
		}
		public Thickness KeyGestureMargin {
			get { return (Thickness)GetValue(KeyGestureMarginProperty); }
			set { SetValue(KeyGestureMarginProperty, value); }
		}
		public Thickness ArrowMargin {
			get { return (Thickness)GetValue(ArrowMarginProperty); }
			set { SetValue(ArrowMarginProperty, value); }
		}
		public FrameworkElement AdditionalContent {
			get { return (FrameworkElement)GetValue(AdditionalContentProperty); }
			set { SetValue(AdditionalContentProperty, value); }
		}
		public Thickness AdditionalContentMargin {
			get { return (Thickness)GetValue(AdditionalContentMarginProperty); }
			set { SetValue(AdditionalContentMarginProperty, value); }
		}
		public Thickness CommonMargin {
			get { return (Thickness)GetValue(CommonMarginProperty); }
			set { SetValue(CommonMarginProperty, value); }
		}
		public BarItemBorderThemeKeyExtension BorderThemeKey {
			get { return (BarItemBorderThemeKeyExtension)GetValue(BorderThemeKeyProperty); }
			set { SetValue(BorderThemeKeyProperty, value); }
		}
		public ThemeKeyExtensionGeneric ArrowThemeKey {
			get { return (ThemeKeyExtensionGeneric)GetValue(ArrowThemeKeyProperty); }
			set { SetValue(ArrowThemeKeyProperty, value); }
		}
		public ControlTemplate NormalBorderTemplate {
			get { return (ControlTemplate)GetValue(NormalBorderTemplateProperty); }
			set { SetValue(NormalBorderTemplateProperty, value); }
		}
		public ControlTemplate HoverBorderTemplate {
			get { return (ControlTemplate)GetValue(HoverBorderTemplateProperty); }
			set { SetValue(HoverBorderTemplateProperty, value); }
		}
		public ControlTemplate CustomizationBorderTemplate {
			get { return (ControlTemplate)GetValue(CustomizationBorderTemplateProperty); }
			set { SetValue(CustomizationBorderTemplateProperty, value); }
		}
		public ControlTemplate PressedBorderTemplate {
			get { return (ControlTemplate)GetValue(PressedBorderTemplateProperty); }
			set { SetValue(PressedBorderTemplateProperty, value); }
		}
		public ControlTemplate ArrowTemplate {
			get { return (ControlTemplate)GetValue(ArrowTemplateProperty); }
			set { SetValue(ArrowTemplateProperty, value); }
		}
		public double DisabledOpacity {
			get { return (double)GetValue(DisabledOpacityProperty); }
			set { SetValue(DisabledOpacityProperty, value); }
		}
		public Thickness? TopGlyphMargin {
			get { return (Thickness?)GetValue(TopGlyphMarginProperty); }
			set { SetValue(TopGlyphMarginProperty, value); }
		}
		public Thickness? BottomGlyphMargin {
			get { return (Thickness?)GetValue(BottomGlyphMarginProperty); }
			set { SetValue(BottomGlyphMarginProperty, value); }
		}
		public Thickness? LeftGlyphMargin {
			get { return (Thickness?)GetValue(LeftGlyphMarginProperty); }
			set { SetValue(LeftGlyphMarginProperty, value); }
		}
		public Thickness? RightGlyphMargin {
			get { return (Thickness?)GetValue(RightGlyphMarginProperty); }
			set { SetValue(RightGlyphMarginProperty, value); }
		}
		public Thickness? TopCommonContentMargin {
			get { return (Thickness?)GetValue(TopCommonContentMarginProperty); }
			set { SetValue(TopCommonContentMarginProperty, value); }
		}
		public Thickness? BottomCommonContentMargin {
			get { return (Thickness?)GetValue(BottomCommonContentMarginProperty); }
			set { SetValue(BottomCommonContentMarginProperty, value); }
		}
		public Thickness? LeftCommonContentMargin {
			get { return (Thickness?)GetValue(LeftCommonContentMarginProperty); }
			set { SetValue(LeftCommonContentMarginProperty, value); }
		}
		public Thickness? RightCommonContentMargin {
			get { return (Thickness?)GetValue(RightCommonContentMarginProperty); }
			set { SetValue(RightCommonContentMarginProperty, value); }
		}
		public Thickness? TopArrowMargin {
			get { return (Thickness?)GetValue(TopArrowMarginProperty); }
			set { SetValue(TopArrowMarginProperty, value); }
		}
		public Thickness? BottomArrowMargin {
			get { return (Thickness?)GetValue(BottomArrowMarginProperty); }
			set { SetValue(BottomArrowMarginProperty, value); }
		}
		public Thickness? LeftArrowMargin {
			get { return (Thickness?)GetValue(LeftArrowMarginProperty); }
			set { SetValue(LeftArrowMarginProperty, value); }
		}
		public Thickness? RightArrowMargin {
			get { return (Thickness?)GetValue(RightArrowMarginProperty); }
			set { SetValue(RightArrowMarginProperty, value); }
		}
		public Thickness ContentAndGlyphMargin {
			get { return (Thickness)GetValue(ContentAndGlyphMarginProperty); }
			set { SetValue(ContentAndGlyphMarginProperty, value); }
		}
		public Thickness? TopContentAndGlyphMargin {
			get { return (Thickness?)GetValue(TopContentAndGlyphMarginProperty); }
			set { SetValue(TopContentAndGlyphMarginProperty, value); }
		}
		public Thickness? BottomContentAndGlyphMargin {
			get { return (Thickness?)GetValue(BottomContentAndGlyphMarginProperty); }
			set { SetValue(BottomContentAndGlyphMarginProperty, value); }
		}
		public Thickness? LeftContentAndGlyphMargin {
			get { return (Thickness?)GetValue(LeftContentAndGlyphMarginProperty); }
			set { SetValue(LeftContentAndGlyphMarginProperty, value); }
		}
		public Thickness? RightContentAndGlyphMargin {
			get { return (Thickness?)GetValue(RightContentAndGlyphMarginProperty); }
			set { SetValue(RightContentAndGlyphMarginProperty, value); }
		}
		public Thickness CommonContentMargin {
			get { return (Thickness)GetValue(CommonContentMarginProperty); }
			set { SetValue(CommonContentMarginProperty, value); }
		}
		public Thickness GlyphBackgroundThickness {
			get { return (Thickness)GetValue(GlyphBackgroundThicknessProperty); }
			set { SetValue(GlyphBackgroundThicknessProperty, value); }
		}
		public ControlTemplate GlyphBackgroundTemplate {
			get { return (ControlTemplate)GetValue(GlyphBackgroundTemplateProperty); }
			set { SetValue(GlyphBackgroundTemplateProperty, value); }
		}
		public FontSettings ContentFontSettings {
			get { return (FontSettings)GetValue(ContentFontSettingsProperty); }
			set { SetValue(ContentFontSettingsProperty, value); }
		}
		public BarItemImageColorizerSettings ImageColorizerSettings {
			get { return (BarItemImageColorizerSettings)GetValue(ImageColorizerSettingsProperty); }
			set { SetValue(ImageColorizerSettingsProperty, value); }
		}		
		public FontSettings Content2FontSettings {
			get { return (FontSettings)GetValue(Content2FontSettingsProperty); }
			set { SetValue(Content2FontSettingsProperty, value); }
		}
		public FontSettings KeyGestureFontSettings {
			get { return (FontSettings)GetValue(KeyGestureFontSettingsProperty); }
			set { SetValue(KeyGestureFontSettingsProperty, value); }
		}
		public FontSettings DescriptionFontSettings {
			get { return (FontSettings)GetValue(DescriptionFontSettingsProperty); }
			set { SetValue(DescriptionFontSettingsProperty, value); }
		}
		public bool FirstBorderNormalStateIsEmpty {
			get { return (bool)GetValue(FirstBorderNormalStateIsEmptyProperty); }
			set { SetValue(FirstBorderNormalStateIsEmptyProperty, value); }
		}
		public bool SecondBorderNormalStateIsEmpty {
			get { return (bool)GetValue(SecondBorderNormalStateIsEmptyProperty); }
			set { SetValue(SecondBorderNormalStateIsEmptyProperty, value); }
		}
		public bool FirstBorderUseNormalStateOnly {
			get { return (bool)GetValue(FirstBorderUseNormalStateOnlyProperty); }
			set { SetValue(FirstBorderUseNormalStateOnlyProperty, value); }
		}
		public bool SecondBorderUseNormalStateOnly {
			get { return (bool)GetValue(SecondBorderUseNormalStateOnlyProperty); }
			set { SetValue(SecondBorderUseNormalStateOnlyProperty, value); }
		}
		public ThemeKeyExtensionGeneric TextSplitterStyleKey {
			get { return (ThemeKeyExtensionGeneric)GetValue(TextSplitterStyleKeyProperty); }
			set { SetValue(TextSplitterStyleKeyProperty, value); }
		}		
		public ThemeKeyExtensionGeneric TouchSplitterThemeKey {
			get { return (ThemeKeyExtensionGeneric)GetValue(TouchSplitterThemeKeyProperty); }
			set { SetValue(TouchSplitterThemeKeyProperty, value); }
		}
		public Transform ArrowTransform {
			get { return (Transform)GetValue(ArrowTransformProperty); }
			set { SetValue(ArrowTransformProperty, value); }
		}
		public Transform GlyphRenderTransform {
			get { return (Transform)GetValue(GlyphRenderTransformProperty); }
			set { SetValue(GlyphRenderTransformProperty, value); }
		}
		public static readonly DependencyProperty GlyphRenderTransformProperty =
			DependencyPropertyManager.Register("GlyphRenderTransform", typeof(Transform), typeof(BarItemLayoutPanel), new FrameworkPropertyMetadata(null, (d, e) => ((BarItemLayoutPanel)d).OnGlyphRenderTransformChanged((Transform)e.OldValue)));
		public static readonly DependencyProperty ArrowTransformProperty =
			DependencyPropertyManager.Register("ArrowTransform", typeof(Transform), typeof(BarItemLayoutPanel), new FrameworkPropertyMetadata(null, (d, e) => ((BarItemLayoutPanel)d).UpdateArrowControl()));
		public static readonly DependencyProperty TouchSplitterThemeKeyProperty =
			DependencyPropertyManager.Register("TouchSplitterThemeKey", typeof(ThemeKeyExtensionGeneric), typeof(BarItemLayoutPanel), new FrameworkPropertyMetadata(null, (d, e) => ((BarItemLayoutPanel)d).InvalidateBorderControl()));		
		public static readonly DependencyProperty TextSplitterStyleKeyProperty =
			DependencyPropertyManager.Register("TextSplitterStyleKey", typeof(ThemeKeyExtensionGeneric), typeof(BarItemLayoutPanel), new FrameworkPropertyMetadata(null, new PropertyChangedCallback((d, e) => ((BarItemLayoutPanel)d).OnTextSplitterStyleKeyChanged((ThemeKeyExtensionGeneric)e.OldValue))));				
		public static readonly DependencyProperty SecondBorderUseNormalStateOnlyProperty =
			DependencyPropertyManager.Register("SecondBorderUseNormalStateOnly", typeof(bool), typeof(BarItemLayoutPanel), new FrameworkPropertyMetadata(false));
		public static readonly DependencyProperty FirstBorderUseNormalStateOnlyProperty =
			DependencyPropertyManager.Register("FirstBorderUseNormalStateOnly", typeof(bool), typeof(BarItemLayoutPanel), new FrameworkPropertyMetadata(false));
		public static readonly DependencyProperty SecondBorderNormalStateIsEmptyProperty =
			DependencyPropertyManager.Register("SecondBorderNormalStateIsEmpty", typeof(bool), typeof(BarItemLayoutPanel), new FrameworkPropertyMetadata(true, new PropertyChangedCallback((d, e) => ((BarItemLayoutPanel)d).OnSecondBorderNormalStateIsEmptyChanged((bool)e.OldValue))));
		public static readonly DependencyProperty FirstBorderNormalStateIsEmptyProperty =
			DependencyPropertyManager.Register("FirstBorderNormalStateIsEmpty", typeof(bool), typeof(BarItemLayoutPanel), new FrameworkPropertyMetadata(true, new PropertyChangedCallback((d, e) => ((BarItemLayoutPanel)d).OnFirstBorderNormalStateIsEmptyChanged((bool)e.OldValue))));
		public static readonly DependencyProperty DescriptionFontSettingsProperty =
			DependencyPropertyManager.Register("DescriptionFontSettings", typeof(FontSettings), typeof(BarItemLayoutPanel), new FrameworkPropertyMetadata(null, new PropertyChangedCallback((d, e) => ((BarItemLayoutPanel)d).UpdateDescriptionFontSettings())));				
		public static readonly DependencyProperty KeyGestureFontSettingsProperty =
			DependencyPropertyManager.Register("KeyGestureFontSettings", typeof(FontSettings), typeof(BarItemLayoutPanel), new FrameworkPropertyMetadata(null, new PropertyChangedCallback((d, e) => ((BarItemLayoutPanel)d).UpdateKeyGestureFontSettings())));		
		public static readonly DependencyProperty Content2FontSettingsProperty =
			DependencyPropertyManager.Register("Content2FontSettings", typeof(FontSettings), typeof(BarItemLayoutPanel), new FrameworkPropertyMetadata(null, new PropertyChangedCallback((d, e) => ((BarItemLayoutPanel)d).UpdateContent2FontSettings())));		
		public static readonly DependencyProperty ContentFontSettingsProperty =
			DependencyPropertyManager.Register("ContentFontSettings", typeof(FontSettings), typeof(BarItemLayoutPanel), new FrameworkPropertyMetadata(null, new PropertyChangedCallback((d, e) => ((BarItemLayoutPanel)d).UpdateContentFontSettings())));
		public static readonly DependencyProperty ImageColorizerSettingsProperty =
			DependencyPropertyManager.Register("ImageColorizerSettings", typeof(BarItemImageColorizerSettings), typeof(BarItemLayoutPanel), new FrameworkPropertyMetadata(null, (d, e) => ((BarItemLayoutPanel)d).UpdateImageColorizerSettings()));		
		public static readonly DependencyProperty GlyphBackgroundTemplateProperty =
			DependencyPropertyManager.Register("GlyphBackgroundTemplate", typeof(ControlTemplate), typeof(BarItemLayoutPanel), new FrameworkPropertyMetadata(null, new PropertyChangedCallback((d, e) => ((BarItemLayoutPanel)d).OnGlyphBackgroundTemplateChanged((ControlTemplate)e.OldValue))));
		public static readonly DependencyProperty GlyphBackgroundThicknessProperty =
			DependencyPropertyManager.Register("GlyphBackgroundThickness", typeof(Thickness), typeof(BarItemLayoutPanel), new FrameworkPropertyMetadata(new Thickness(), FrameworkPropertyMetadataOptions.AffectsMeasure));
		public static readonly DependencyProperty CommonContentMarginProperty =
			DependencyPropertyManager.Register("CommonContentMargin", typeof(Thickness), typeof(BarItemLayoutPanel), new FrameworkPropertyMetadata(null));
		public static readonly DependencyProperty RightContentAndGlyphMarginProperty =
			DependencyPropertyManager.Register("RightContentAndGlyphMargin", typeof(Thickness?), typeof(BarItemLayoutPanel), new FrameworkPropertyMetadata(null));
		public static readonly DependencyProperty LeftContentAndGlyphMarginProperty =
			DependencyPropertyManager.Register("LeftContentAndGlyphMargin", typeof(Thickness?), typeof(BarItemLayoutPanel), new FrameworkPropertyMetadata(null));
		public static readonly DependencyProperty BottomContentAndGlyphMarginProperty =
			DependencyPropertyManager.Register("BottomContentAndGlyphMargin", typeof(Thickness?), typeof(BarItemLayoutPanel), new FrameworkPropertyMetadata(null));
		public static readonly DependencyProperty TopContentAndGlyphMarginProperty =
			DependencyPropertyManager.Register("TopContentAndGlyphMargin", typeof(Thickness?), typeof(BarItemLayoutPanel), new FrameworkPropertyMetadata(null));
		public static readonly DependencyProperty ContentAndGlyphMarginProperty =
			DependencyPropertyManager.Register("ContentAndGlyphMargin", typeof(Thickness), typeof(BarItemLayoutPanel), new FrameworkPropertyMetadata(new Thickness()));
		public static readonly DependencyProperty RightArrowMarginProperty =
			DependencyPropertyManager.Register("RightArrowMargin", typeof(Thickness?), typeof(BarItemLayoutPanel), new FrameworkPropertyMetadata(null));
		public static readonly DependencyProperty LeftArrowMarginProperty =
			DependencyPropertyManager.Register("LeftArrowMargin", typeof(Thickness?), typeof(BarItemLayoutPanel), new FrameworkPropertyMetadata(null));
		public static readonly DependencyProperty BottomArrowMarginProperty =
			DependencyPropertyManager.Register("BottomArrowMargin", typeof(Thickness?), typeof(BarItemLayoutPanel), new FrameworkPropertyMetadata(null));
		public static readonly DependencyProperty TopArrowMarginProperty =
			DependencyPropertyManager.Register("TopArrowMargin", typeof(Thickness?), typeof(BarItemLayoutPanel), new FrameworkPropertyMetadata(null));
		public static readonly DependencyProperty RightCommonContentMarginProperty =
			DependencyPropertyManager.Register("RightCommonContentMargin", typeof(Thickness?), typeof(BarItemLayoutPanel), new FrameworkPropertyMetadata(null));
		public static readonly DependencyProperty LeftCommonContentMarginProperty =
			DependencyPropertyManager.Register("LeftCommonContentMargin", typeof(Thickness?), typeof(BarItemLayoutPanel), new FrameworkPropertyMetadata(null));
		public static readonly DependencyProperty BottomCommonContentMarginProperty =
			DependencyPropertyManager.Register("BottomCommonContentMargin", typeof(Thickness?), typeof(BarItemLayoutPanel), new FrameworkPropertyMetadata(null));
		public static readonly DependencyProperty TopCommonContentMarginProperty =
			DependencyPropertyManager.Register("TopCommonContentMargin", typeof(Thickness?), typeof(BarItemLayoutPanel), new FrameworkPropertyMetadata(null));
		public static readonly DependencyProperty RightGlyphMarginProperty =
			DependencyPropertyManager.Register("RightGlyphMargin", typeof(Thickness?), typeof(BarItemLayoutPanel), new FrameworkPropertyMetadata(null));
		public static readonly DependencyProperty LeftGlyphMarginProperty =
			DependencyPropertyManager.Register("LeftGlyphMargin", typeof(Thickness?), typeof(BarItemLayoutPanel), new FrameworkPropertyMetadata(null));
		public static readonly DependencyProperty BottomGlyphMarginProperty =
			DependencyPropertyManager.Register("BottomGlyphMargin", typeof(Thickness?), typeof(BarItemLayoutPanel), new FrameworkPropertyMetadata(null));
		public static readonly DependencyProperty TopGlyphMarginProperty =
			DependencyPropertyManager.Register("TopGlyphMargin", typeof(Thickness?), typeof(BarItemLayoutPanel), new FrameworkPropertyMetadata(null));
		public static readonly DependencyProperty DisabledOpacityProperty =
			DependencyPropertyManager.Register("DisabledOpacity", typeof(double), typeof(BarItemLayoutPanel), new FrameworkPropertyMetadata(0.35d));
		public static readonly DependencyProperty ArrowTemplateProperty =
			DependencyPropertyManager.Register("ArrowTemplate", typeof(ControlTemplate), typeof(BarItemLayoutPanel), new FrameworkPropertyMetadata(null, new PropertyChangedCallback((d, e) => ((BarItemLayoutPanel)d).InvalidateArrowControl())));
		public static readonly DependencyProperty PressedBorderTemplateProperty =
			DependencyPropertyManager.Register("PressedBorderTemplate", typeof(ControlTemplate), typeof(BarItemLayoutPanel), new FrameworkPropertyMetadata(null, new PropertyChangedCallback((d, e) => ((BarItemLayoutPanel)d).InvalidateBorderControl())));
		public static readonly DependencyProperty CustomizationBorderTemplateProperty =
			DependencyPropertyManager.Register("CustomizationBorderTemplate", typeof(ControlTemplate), typeof(BarItemLayoutPanel), new FrameworkPropertyMetadata(null, new PropertyChangedCallback((d, e) => ((BarItemLayoutPanel)d).InvalidateBorderControl())));
		public static readonly DependencyProperty HoverBorderTemplateProperty =
			DependencyPropertyManager.Register("HoverBorderTemplate", typeof(ControlTemplate), typeof(BarItemLayoutPanel), new FrameworkPropertyMetadata(null, new PropertyChangedCallback((d, e) => ((BarItemLayoutPanel)d).InvalidateBorderControl())));
		public static readonly DependencyProperty NormalBorderTemplateProperty =
			DependencyPropertyManager.Register("NormalBorderTemplate", typeof(ControlTemplate), typeof(BarItemLayoutPanel), new FrameworkPropertyMetadata(null, new PropertyChangedCallback((d, e) => ((BarItemLayoutPanel)d).InvalidateBorderControl())));
		public static readonly DependencyProperty ArrowThemeKeyProperty =
			DependencyPropertyManager.Register("ArrowThemeKey", typeof(ThemeKeyExtensionGeneric), typeof(BarItemLayoutPanel), new FrameworkPropertyMetadata(null, new PropertyChangedCallback((d, e) => ((BarItemLayoutPanel)d).InvalidateArrowControl())));
		public static readonly DependencyProperty BorderThemeKeyProperty =
			DependencyPropertyManager.Register("BorderThemeKey", typeof(BarItemBorderThemeKeyExtension), typeof(BarItemLayoutPanel), new FrameworkPropertyMetadata(null, new PropertyChangedCallback((d, e) => ((BarItemLayoutPanel)d).InvalidateBorderControl())));
		public static readonly DependencyProperty CommonMarginProperty =
			DependencyPropertyManager.Register("CommonMargin", typeof(Thickness), typeof(BarItemLayoutPanel), new FrameworkPropertyMetadata(new Thickness(), FrameworkPropertyMetadataOptions.AffectsMeasure));
		public static readonly DependencyProperty AdditionalContentMarginProperty =
			DependencyPropertyManager.Register("AdditionalContentMargin", typeof(Thickness), typeof(BarItemLayoutPanel), new FrameworkPropertyMetadata(new Thickness(), FrameworkPropertyMetadataOptions.AffectsMeasure));
		public static readonly DependencyProperty AdditionalContentProperty =
			DependencyPropertyManager.Register("AdditionalContent", typeof(FrameworkElement), typeof(BarItemLayoutPanel), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsMeasure, new PropertyChangedCallback((d, e) => ((BarItemLayoutPanel)d).OnAdditionalContentChanged(e.OldValue as UIElement))));
		public static readonly DependencyProperty ArrowMarginProperty =
			DependencyPropertyManager.Register("ArrowMargin", typeof(Thickness), typeof(BarItemLayoutPanel), new FrameworkPropertyMetadata(new Thickness(), FrameworkPropertyMetadataOptions.AffectsMeasure));
		public static readonly DependencyProperty KeyGestureMarginProperty =
			DependencyPropertyManager.Register("KeyGestureMargin", typeof(Thickness), typeof(BarItemLayoutPanel), new FrameworkPropertyMetadata(new Thickness(), FrameworkPropertyMetadataOptions.AffectsMeasure));
		public static readonly DependencyProperty DescriptionMarginProperty =
			DependencyPropertyManager.Register("DescriptionMargin", typeof(Thickness), typeof(BarItemLayoutPanel), new FrameworkPropertyMetadata(new Thickness(), FrameworkPropertyMetadataOptions.AffectsMeasure));
		public static readonly DependencyProperty Content2MarginProperty =
			DependencyPropertyManager.Register("Content2Margin", typeof(Thickness), typeof(BarItemLayoutPanel), new FrameworkPropertyMetadata(new Thickness(), FrameworkPropertyMetadataOptions.AffectsMeasure));
		public static readonly DependencyProperty ContentMarginProperty =
			DependencyPropertyManager.Register("ContentMargin", typeof(Thickness), typeof(BarItemLayoutPanel), new FrameworkPropertyMetadata(new Thickness(), FrameworkPropertyMetadataOptions.AffectsMeasure));
		public static readonly DependencyProperty GlyphMarginProperty =
			DependencyPropertyManager.Register("GlyphMargin", typeof(Thickness), typeof(BarItemLayoutPanel), new FrameworkPropertyMetadata(new Thickness(), FrameworkPropertyMetadataOptions.AffectsMeasure));
		static BarItemLayoutPanel() {
			MarginProperty.OverrideMetadata(typeof(BarItemLayoutPanel), new FrameworkPropertyMetadata(default(Thickness), null, new CoerceValueCallback(CoerceThickness)));
		}
		static object CoerceThickness(DependencyObject d, object baseValue) {
			var baseThickness = (Thickness)baseValue;
			if (ScreenHelper.ScaleX.AreClose(1d))
				return baseValue;
			return new Thickness(
				baseThickness.Left / ScreenHelper.ScaleX,
				baseThickness.Top / ScreenHelper.ScaleX,
				baseThickness.Right / ScreenHelper.ScaleX,
				baseThickness.Bottom / ScreenHelper.ScaleX);
		}
		protected virtual void OnGlyphBackgroundTemplateChanged(ControlTemplate oldValue) {
			UpdateGlyphBackground();
		}
		protected virtual void InvalidateBorderControl() {
			ClearBorderControls();
			UpdateBorderControl();
		}
		protected virtual void InvalidateArrowControl() {
			ClearArrowControl();
			UpdateArrowControl();
		}
		protected virtual void OnAdditionalContentChanged(UIElement oldContent) {
			UpdateAdditionalContentHost();
			OnAdditionalContentHorizontalAlignmentChanged(System.Windows.HorizontalAlignment.Left);
			OnAdditionalContentSizeSettingsChangedEvent(null, null);
		}
		protected virtual void OnTextSplitterStyleKeyChanged(ThemeKeyExtensionGeneric oldValue) {
			UpdateContent();
		}
		#endregion
		#region NormalProperties
		private bool showGlyph;
		private ImageSource actualGlyph;
		private GlyphSize glyphSize;
		private Size maxGlyphSize;
		private DataTemplate glyphTemplate;
		private Dock glyphToContentAlignment;
		private object content;
		private object content2;
		private object description;
		private object keyGesture;
		private bool splitContent;
		private DataTemplate contentTemplate;
		private DataTemplateSelector contentTemplateSelector;
		private bool showFirstBorder;
		private bool showSecondBorder;
		private BorderState borderState;
		private bool showArrow;
		private bool isFirstBorderActive;
		private bool isSecondBorderActive;
		private bool showContent;
		private bool showContent2;
		private bool showDescription;
		private bool showKeyGesture;
		private HorizontalItemPositionType itemPosition;
		private CodedPanelOwnerKind ownerKind;
		private bool showAdditionalContent;		
		private SecondBorderPlacement secondBorderPlacement;
		private Dock contentAndGlyphToArrowAlignment;
		private bool showGlyphBackground;
		private BorderState firstBorderState;
		private BorderState secondBorderState;
		private DataTemplate additionalContentTemplate;
		private DataTemplate content2Template;
		private SizeSettings additionalContentSizeSettings;
		private HorizontalAlignment contentHorizontalAlignment;
		private HorizontalAlignment additionalContentHorizontalAlignment;
		private Style additionalContentStyle;
		private HorizontalItemPositionType firstBorderItemPosition;
		private HorizontalItemPositionType secondBorderItemPosition;
		private bool stretchAdditionalContentVertically = false;
		private bool colorizeGlyph;
		private SpacingMode spacingMode;
		private bool actAsDropDown;
		public SpacingMode SpacingMode {
			get { return spacingMode; }
			set {
				if (value == spacingMode)
					return;
				SpacingMode oldValue = spacingMode;
				spacingMode = value;
				OnSpacingModeChanged(oldValue);
			}
		}
		public bool StretchAdditionalContentVertically {
			get { return stretchAdditionalContentVertically; }
			set {
				bool raiseChange = value != stretchAdditionalContentVertically;
				bool oldValue = stretchAdditionalContentVertically;
				stretchAdditionalContentVertically = value;
				if(raiseChange)
					OnStretchAdditionalContentVerticallyChanged(oldValue);
			}
		}
		public HorizontalItemPositionType SecondBorderItemPosition {
			get { return secondBorderItemPosition; }
			set {
				bool raiseChange = value != secondBorderItemPosition;
				HorizontalItemPositionType oldValue = secondBorderItemPosition;
				secondBorderItemPosition = value;
				if(raiseChange)
					OnSecondBorderItemPositionChanged(oldValue);
			}
		}		
		public HorizontalItemPositionType FirstBorderItemPosition {
			get { return firstBorderItemPosition; }
			set {
				bool raiseChange = value != firstBorderItemPosition;
				HorizontalItemPositionType oldValue = firstBorderItemPosition;
				firstBorderItemPosition = value;
				if(raiseChange)
					OnFirstBorderItemPositionChanged(oldValue);
			}
		}				
		public Style AdditionalContentStyle {
			get { return additionalContentStyle; }
			set {
				bool raiseChange = value != additionalContentStyle;
				Style oldValue = additionalContentStyle;
				additionalContentStyle = value;
				if(raiseChange)
					OnAdditionalContentStyleChanged(oldValue);
			}
		}
		public HorizontalAlignment AdditionalContentHorizontalAlignment {
			get { return additionalContentHorizontalAlignment; }
			set {
				bool raiseChange = value != additionalContentHorizontalAlignment;
				HorizontalAlignment oldValue = additionalContentHorizontalAlignment;
				additionalContentHorizontalAlignment = value;
				if(raiseChange)
					OnAdditionalContentHorizontalAlignmentChanged(oldValue);
			}
		}
		public HorizontalAlignment ContentHorizontalAlignment {
			get { return contentHorizontalAlignment; }
			set {
				if(value == contentHorizontalAlignment) return;				
				HorizontalAlignment oldValue = contentHorizontalAlignment;
				contentHorizontalAlignment = value;
				OnContentHorizontalAlignmentChanged(oldValue);
			}
		}						
		public SizeSettings AdditionalContentSizeSettings {
			get { return additionalContentSizeSettings; }
			set {
				bool raiseChange = value != additionalContentSizeSettings;
				SizeSettings oldValue = additionalContentSizeSettings;
				additionalContentSizeSettings = value;
				if(raiseChange)
					OnAdditionalContentSizeSettingsChanged(oldValue);
			}
		}						
		public DataTemplate Content2Template {
			get { return content2Template; }
			set {
				bool raiseChange = value != content2Template;
				DataTemplate oldValue = content2Template;
				content2Template = value;
				if(raiseChange)
					OnContent2TemplateChanged(oldValue);
			}
		}		
		public DataTemplate AdditionalContentTemplate {
			get { return additionalContentTemplate; }
			set {
				bool raiseChange = value != additionalContentTemplate;
				DataTemplate oldValue = additionalContentTemplate;
				additionalContentTemplate = value;
				if(raiseChange)
					OnAdditionalContentTemplateChanged(oldValue);
			}
		}
		public BorderState SecondBorderState {
			get { return secondBorderState; }
			set {
				bool raiseChange = value != secondBorderState;
				BorderState oldValue = secondBorderState;
				secondBorderState = value;
				if(raiseChange)
					OnSecondBorderStateChanged(oldValue);
			}
		}		
		public BorderState FirstBorderState {
			get { return firstBorderState; }
			set {
				bool raiseChange = value != firstBorderState;
				BorderState oldValue = firstBorderState;
				firstBorderState = value;
				if(raiseChange)
					OnFirstBorderStateChanged(oldValue);
			}
		}						
		public bool ShowGlyphBackground {
			get { return showGlyphBackground; }
			set {
				bool raiseChange = value != showGlyphBackground;
				bool oldValue = showGlyphBackground;
				showGlyphBackground = value;
				if(raiseChange)
					OnShowGlyphBackgroundChanged(oldValue);
			}
		}
		public SecondBorderPlacement SecondBorderPlacement {
			get { return secondBorderPlacement; }
			set {
				bool raiseChange = value != secondBorderPlacement;
				SecondBorderPlacement oldValue = secondBorderPlacement;
				secondBorderPlacement = value;
				if(raiseChange)
					OnSecondBorderPlacementChanged(oldValue);
			}
		}
		public bool ShowAdditionalContent {
			get { return showAdditionalContent; }
			set {
				bool raiseChange = value != showAdditionalContent;
				bool oldValue = showAdditionalContent;
				showAdditionalContent = value;
				if(raiseChange)
					OnShowAdditionalContentChanged(oldValue);
			}
		}
		public CodedPanelOwnerKind OwnerKind {
			get { return ownerKind; }
			set {
				bool raiseChange = value != ownerKind;
				CodedPanelOwnerKind oldValue = ownerKind;
				ownerKind = value;
				if(raiseChange)
					OnOwnerKindChanged(oldValue);
			}
		}
		public HorizontalItemPositionType ItemPosition {
			get { return itemPosition; }
			set {
				bool raiseChange = value != itemPosition;
				HorizontalItemPositionType oldValue = itemPosition;
				itemPosition = value;
				if(raiseChange)
					OnItemPositionChanged(oldValue);
			}
		}
		public bool ShowKeyGesture {
			get { return showKeyGesture; }
			set {
				bool raiseChange = value != showKeyGesture;
				bool oldValue = showKeyGesture;
				showKeyGesture = value;
				if(raiseChange)
					OnShowKeyGestureChanged(oldValue);
			}
		}
		public bool ShowDescription {
			get { return showDescription; }
			set {
				bool raiseChange = value != showDescription;
				bool oldValue = showDescription;
				showDescription = value;
				if(raiseChange)
					OnShowDescriptionChanged(oldValue);
			}
		}
		public bool ShowContent2 {
			get { return showContent2; }
			set {
				bool raiseChange = value != showContent2;
				bool oldValue = showContent2;
				showContent2 = value;
				if(raiseChange)
					OnShowContent2Changed(oldValue);
			}
		}
		public bool ShowContent {
			get { return showContent; }
			set {
				bool raiseChange = value != showContent;
				bool oldValue = showContent;
				showContent = value;
				if(raiseChange)
					OnShowContentChanged(oldValue);
			}
		}
		public bool IsSecondBorderActive {
			get { return isSecondBorderActive; }
			set {
				bool raiseChange = value != isSecondBorderActive;
				bool oldValue = isSecondBorderActive;
				isSecondBorderActive = value;
				if(raiseChange)
					OnIsSecondBorderActiveChanged(oldValue);
			}
		}
		public bool IsFirstBorderActive {
			get { return isFirstBorderActive; }
			set {
				bool raiseChange = value != isFirstBorderActive;
				bool oldValue = isFirstBorderActive;
				isFirstBorderActive = value;
				if(raiseChange)
					OnIsFirstBorderActiveChanged(oldValue);
			}
		}
		public bool ShowArrow {
			get { return showArrow; }
			set {
				bool raiseChange = value != showArrow;
				bool oldValue = showArrow;
				showArrow = value;
				if(raiseChange)
					OnShowArrowChanged(oldValue);
			}
		}				
		public bool ActualShowArrow { get { return ShowArrow && !ShowArrowInTextSplitter; } }
		public bool ShouldShowTouchSplitter { get { return SpacingMode == SpacingMode.Touch && ShowFirstBorder && ShowSecondBorder && !ActAsDropDown; } }
		public bool ActualShowTouchSplitter { get { return ShouldShowTouchSplitter && ElementTouchSplitterTransformPanel != null; } }
		public bool ShowArrowInTextSplitter { get { return (ContentAndGlyphToArrowAlignment == Dock.Top && GlyphToContentAlignment == Dock.Top && SplitContent); } }
		public BorderState BorderState {
			get { return borderState; }
			set {
				bool raiseChange = value != borderState;
				BorderState oldValue = borderState;
				borderState = value;
				if(raiseChange)
					OnBorderStateChanged(oldValue);
			}
		}
		public bool SecondBorderShowing { get { return ElementSecondBorderControl != null && ShowSecondBorder; } }
		public bool ShowSecondBorder {
			get { return showSecondBorder; }
			set {
				bool raiseChange = value != showSecondBorder;
				bool oldValue = showSecondBorder;
				showSecondBorder = value;
				if(raiseChange)
					OnShowSecondBorderChanged(oldValue);
			}
		}
		public bool FirstBorderShowing { get { return ElementFirstBorderControl != null && ShowFirstBorder; } }
		public bool ShowFirstBorder {
			get { return showFirstBorder; }
			set {
				bool raiseChange = value != showFirstBorder;
				bool oldValue = showFirstBorder;
				showFirstBorder = value;
				if(raiseChange)
					OnShowFirstBorderChanged(oldValue);
			}
		}
		public DataTemplateSelector ContentTemplateSelector {
			get { return contentTemplateSelector; }
			set {
				bool raiseChange = value != contentTemplateSelector;
				DataTemplateSelector oldValue = contentTemplateSelector;
				contentTemplateSelector = value;
				if(raiseChange)
					OnContentTemplateSelectorChanged(oldValue);
			}
		}
		public DataTemplate ContentTemplate {
			get { return contentTemplate; }
			set {
				bool raiseChange = value != contentTemplate;
				DataTemplate oldValue = contentTemplate;
				contentTemplate = value;
				if(raiseChange)
					OnContentTemplateChanged(oldValue);
			}
		}
		public bool SplitContent {
			get { return splitContent; }
			set {
				bool raiseChange = value != splitContent;
				bool oldValue = splitContent;
				splitContent = value;
				if(raiseChange)
					OnSplitContentChanged(oldValue);
			}
		}
		public object KeyGesture {
			get { return keyGesture; }
			set {
				bool raiseChange = value != keyGesture;
				object oldValue = keyGesture;
				keyGesture = value;
				if(raiseChange)
					OnKeyGestureChanged(oldValue);
			}
		}
		public object Description {
			get { return description; }
			set {
				bool raiseChange = value != description;
				object oldValue = description;
				description = value;
				if(raiseChange)
					OnDescriptionChanged(oldValue);
			}
		}
		public object Content2 {
			get { return content2; }
			set {
				bool raiseChange = value != content2;
				object oldValue = content2;
				content2 = value;
				if(raiseChange)
					OnContent2Changed(oldValue);
			}
		}
		public object Content {
			get { return content; }
			set {
				bool raiseChange = value != content;
				object oldValue = content;
				content = value;
				if(raiseChange)
					OnContentChanged(oldValue);
			}
		}
		public Dock GlyphToContentAlignment {
			get { return glyphToContentAlignment; }
			set {
				bool raiseChange = value != glyphToContentAlignment;
				Dock oldValue = glyphToContentAlignment;
				glyphToContentAlignment = value;
				if(raiseChange)
					OnGlyphToContentAlignmentChanged(oldValue);
			}
		}
		public DataTemplate GlyphTemplate {
			get { return glyphTemplate; }
			set {
				bool raiseChange = value != glyphTemplate;
				DataTemplate oldValue = glyphTemplate;
				glyphTemplate = value;
				if(raiseChange)
					OnGlyphTemplateChanged(oldValue);
			}
		}
		public Size MaxGlyphSize {
			get { return maxGlyphSize; }
			set {
				if (maxGlyphSize == value)
					return;
				var oldValue = maxGlyphSize;
				maxGlyphSize = value;
				OnMaxGlyphSizeChanged(oldValue);
			}
		}
		public GlyphSize GlyphSize {
			get { return glyphSize; }
			set {
				bool raiseChange = value != glyphSize;
				GlyphSize oldValue = glyphSize;
				glyphSize = value;
				if(raiseChange)
					OnGlyphSizeChanged(oldValue);
			}
		}
		public ImageSource ActualGlyph {
			get { return actualGlyph; }
			set {
				bool raiseChange = value != actualGlyph;
				ImageSource oldValue = actualGlyph;
				actualGlyph = value;
				if(raiseChange)
					OnActualGlyphChanged(oldValue);
			}
		}
		public bool ShowGlyph {
			get { return showGlyph; }
			set {
				bool raiseChange = value != showGlyph;
				bool oldValue = showGlyph;
				showGlyph = value;
				if(raiseChange)
					OnShowGlyphChanged(oldValue);
			}
		}
		public Dock ContentAndGlyphToArrowAlignment {
			get { return contentAndGlyphToArrowAlignment; }
			set {
				bool raiseChange = value != contentAndGlyphToArrowAlignment;
				Dock oldValue = contentAndGlyphToArrowAlignment;
				contentAndGlyphToArrowAlignment = value;
				if(raiseChange)
					OnContentAndGlyphToArrowAlignmentChanged(oldValue);
			}
		}		
		public bool ColorizeGlyph {
			get { return colorizeGlyph; }
			set {
				if (value == colorizeGlyph) return;
				bool oldValue = colorizeGlyph;
				colorizeGlyph = value;
				OnColorizeGlyphChanged(oldValue);
			}
		}
		public bool ActAsDropDown {
			get { return actAsDropDown; }
			set {
				if (value == actAsDropDown)
					return;
				bool oldValue = actAsDropDown;
				actAsDropDown = value;
				OnActAsDropDownChanged(oldValue);
			}
		}
		protected string ThemeName {
			get { return ThemeHelper.GetEditorThemeName(this); }
		}
		protected virtual void OnSpacingModeChanged(SpacingMode oldValue) {
			UpdateTouchSplitter();
			InvalidateMeasure();			
		}
		protected virtual void OnShowGlyphChanged(bool oldValue) {
			UpdateGlyph();			
		}
		protected virtual void OnActualGlyphChanged(ImageSource oldValue) {
			UpdateGlyph();
			UpdateGlyphBackground();
		}
		protected virtual void OnMaxGlyphSizeChanged(Size oldValue) {
			UpdateGlyph();
		}
		protected virtual void OnGlyphSizeChanged(GlyphSize oldValue) {
			UpdateGlyph();
		}
		protected virtual void OnGlyphTemplateChanged(DataTemplate oldValue) {
			UpdateGlyph();
			UpdateGlyphBackground();
		}
		protected virtual void OnGlyphToContentAlignmentChanged(Dock oldValue) {
			UpdateGlyph();
			UpdateElementTextSplitterIsArrowVisible();
		}
		protected virtual void OnGlyphRenderTransformChanged(Transform oldValue) {
			UpdateGlyph();
		}
		protected virtual void OnContentChanged(object oldValue) {
			UpdateContent();
		}
		protected virtual void OnContent2Changed(object oldValue) {
			UpdateContent2();
		}
		protected virtual void OnDescriptionChanged(object oldValue) {
			UpdateDescription();
		}
		protected virtual void OnKeyGestureChanged(object oldValue) {
			UpdateKeyGesture();
		}
		protected virtual void OnStretchAdditionalContentVerticallyChanged(bool oldValue) {
			InvalidateMeasure();
			UpdateAdditionalContentHost();
		}
		protected virtual void OnShowContentChanged(bool oldValue) {
			UpdateContent();
		}
		protected virtual void OnShowContent2Changed(bool oldValue) {
			UpdateContent2();
		}
		protected virtual void OnShowDescriptionChanged(bool oldValue) {
			UpdateDescription();
		}
		protected virtual void OnShowKeyGestureChanged(bool oldValue) {
			UpdateKeyGesture();
		}
		protected virtual void OnSplitContentChanged(bool oldValue) {
			UpdateContent();
			UpdateElementTextSplitterIsArrowVisible();
		}
		protected virtual void OnContentTemplateChanged(DataTemplate oldValue) {
			UpdateContent();			
		}
		protected virtual void OnContent2TemplateChanged(DataTemplate oldValue) {
			UpdateContent2();
		}
		protected virtual void OnContentTemplateSelectorChanged(DataTemplateSelector oldValue) {
			UpdateContent();			
		}
		protected virtual void OnShowFirstBorderChanged(bool oldValue) {
			UpdateBorderControl();
			UpdateTouchSplitter();
		}
		protected virtual void OnShowSecondBorderChanged(bool oldValue) {
			UpdateBorderControl();
			UpdateTouchSplitter();
		}
		protected virtual void OnBorderStateChanged(BorderState oldValue) {
			UpdateBorderControl();
			UpdateFontSettings();
			UpdateImageColorizerSettings();
		}
		protected virtual void OnSecondBorderStateChanged(BorderState oldValue) {
			UpdateBorderControl();
			UpdateFontSettings();
		}
		protected virtual void OnAdditionalContentTemplateChanged(DataTemplate oldValue) {
			UpdateAdditionalContentHost();
		}		
		protected virtual void OnFirstBorderStateChanged(BorderState oldValue) {
			UpdateBorderControl();
			UpdateFontSettings();
			UpdateImageColorizerSettings();
		}
		protected virtual void OnIsFirstBorderActiveChanged(bool oldValue) {
			UpdateBorderControl();
		}
		protected virtual void OnIsSecondBorderActiveChanged(bool oldValue) {
			UpdateBorderControl();
		}
		protected virtual void OnShowArrowChanged(bool oldValue) {
			UpdateArrowControl();
		}
		protected virtual void OnItemPositionChanged(HorizontalItemPositionType oldValue) {
			UpdateBorderControl();
		}
		protected virtual void OnFirstBorderItemPositionChanged(HorizontalItemPositionType oldValue) {
			UpdateBorderControl();
		}
		protected virtual void OnSecondBorderItemPositionChanged(HorizontalItemPositionType oldValue) {
			UpdateBorderControl();
		}
		protected virtual void OnSecondBorderNormalStateIsEmptyChanged(bool oldValue) {
			UpdateBorderControl();
		}
		protected virtual void OnFirstBorderNormalStateIsEmptyChanged(bool oldValue) {
			UpdateBorderControl();
		}	
		protected virtual void OnOwnerKindChanged(CodedPanelOwnerKind oldValue) {
			InvalidateMeasure();
		}
		protected virtual void OnShowAdditionalContentChanged(bool oldValue) {
			UpdateAdditionalContentHost();
		}
		protected virtual void OnContentAndGlyphToArrowAlignmentChanged(Dock oldValue) {
			UpdateArrowControl();
			UpdateBorderControl();
			UpdateElementTextSplitterIsArrowVisible();
			InvalidateMeasure();
			UpdateTouchSplitterOrientation();
		}		
		protected virtual void OnColorizeGlyphChanged(bool oldValue) {
			UpdateGlyph();
			UpdateImageColorizerSettings();
		}
		protected virtual void OnSecondBorderPlacementChanged(SecondBorderPlacement oldValue) {
			InvalidateMeasure();
		}
		protected virtual void OnShowGlyphBackgroundChanged(bool oldValue) {
			UpdateGlyphBackground();
		}
		protected virtual void OnContentHorizontalAlignmentChanged(HorizontalAlignment oldValue) {
			UpdateContent();
		}		
		protected virtual void OnAdditionalContentSizeSettingsChanged(SizeSettings oldValue) {
			if(oldValue != null) oldValue.Changed -= OnAdditionalContentSizeSettingsChangedEvent;
			UpdateAdditionalContentHost();
			if(AdditionalContentSizeSettings != null) AdditionalContentSizeSettings.Changed += OnAdditionalContentSizeSettingsChangedEvent;
			OnAdditionalContentSizeSettingsChangedEvent(null, null);
		}
		void OnAdditionalContentSizeSettingsChangedEvent(object sender, EventArgs e) {
			if(AdditionalContentSizeSettings == null || AdditionalContent==null) return;
			AdditionalContentSizeSettings.Apply(AdditionalContent);
		}
		protected virtual void OnAdditionalContentHorizontalAlignmentChanged(HorizontalAlignment oldValue) {
			if((AdditionalContent as FrameworkElement) != null)
				(AdditionalContent as FrameworkElement).HorizontalAlignment = AdditionalContentHorizontalAlignment;
				if(ElementAdditionalContentHost != null)
					ElementAdditionalContentHost.HorizontalContentAlignment = AdditionalContentHorizontalAlignment;
			}
		protected virtual void OnAdditionalContentStyleChanged(Style oldValue) {
			UpdateAdditionalContentHost();
		}
		protected virtual void OnActAsDropDownChanged(bool oldValue) {
		}
		#endregion
		#region Controls
		protected internal FrameworkElement ElementTargetGlyph { get { return GlyphTemplate != null ? (FrameworkElement)ElementTemplatedGlyph : (FrameworkElement)ElementGlyph; } }
		protected internal Image ElementGlyph { get; private set; }
		protected internal MeasurePixelSnapperContentControl ElementTemplatedGlyph { get; private set; }
		protected internal ContentControl ElementContent { get; private set; }
		protected internal ContentControl ElementContent2 { get; private set; }
		protected internal ContentControl ElementDescription { get; private set; }
		protected internal ContentControl ElementKeyGesture { get; private set; }
		protected internal TextSplitterControl ElementTextSplitter { get; private set; }
		protected internal ItemBorderControl ElementFirstBorderControl { get; private set; }
		protected internal ItemBorderControl ElementSecondBorderControl { get; private set; }
		protected internal ArrowControl ElementArrowControl { get; private set; }
		protected internal ContentControl ElementGlyphBackground { get; private set; }
		protected internal ContentControl ElementAdditionalContentHost { get; private set; }
		protected internal LayoutTransformPanel ElementTouchSplitterTransformPanel { get; set; }
		protected virtual void CreateElementGlyphBackground() {
			if(ElementGlyphBackground != null) return;
			ElementGlyphBackground = new ContentControlEx();
			ElementGlyphBackground.SetZIndex(2);
			Children.Add(ElementGlyphBackground);
		}
		protected virtual void CreateElementGlyph() {
			if(ElementGlyph != null) return;
			ElementGlyph = new DXImage();
			ElementGlyph.SetZIndex(3);
			Children.Add(ElementGlyph);
			UpdateImageColorizerSettings();
		}
		protected virtual void CreateElementTemplatedGlyph() {
			if(ElementTemplatedGlyph != null) return;
			ElementTemplatedGlyph = new MeasurePixelSnapperContentControl() { VerticalContentAlignment = VerticalAlignment.Center, HorizontalContentAlignment = HorizontalAlignment.Center };
			ElementTemplatedGlyph.SetZIndex(3);
			Children.Add(ElementTemplatedGlyph);
		}
		protected virtual void CreateElementContent() {
			if(ElementContent != null) return;
			ElementContent = new ContentControlEx();
			ElementContent.SetZIndex(3);
			UpdateContentFontSettings();
			Children.Add(ElementContent);
		}
		protected virtual void CreateElementContent2() {
			if(ElementContent2 != null) return;
			ElementContent2 = new ContentControlEx();
			ElementContent2.SetZIndex(3);
			UpdateContent2FontSettings();
			Children.Add(ElementContent2);
		}
		protected virtual void CreateElementDescription() {
			if(ElementDescription != null) return;
			ElementDescription = new ContentControlEx();
			ElementDescription.SetZIndex(3);
			UpdateDescriptionFontSettings();
			Children.Add(ElementDescription);
		}
		protected virtual void CreateElementKeyGesture() {
			if(ElementKeyGesture != null) return;
			ElementKeyGesture = new ContentControlEx();
			ElementKeyGesture.SetZIndex(3);
			UpdateKeyGestureFontSettings();
			Children.Add(ElementKeyGesture);
		}
		protected virtual void CreateElementTextSplitter() {
			if(ElementTextSplitter != null) return;
			ElementTextSplitter = new TextSplitterControl();
			UpdateElementTextSplitterIsArrowVisible();
			UpdateContentFontSettings();
			ElementTextSplitter.SetZIndex(3);
			Children.Add(ElementTextSplitter);
		}
		protected virtual void CreateElementFirstBorderControl() {
			if(ElementFirstBorderControl != null || FirstBorderNormalStateIsEmpty && !ShowFirstBorder && (FirstBorderState == Bars.BorderState.Default ? (BorderState == Bars.BorderState.Normal || BorderState == Bars.BorderState.Default) : (FirstBorderState == Bars.BorderState.Normal))) return;
			ElementFirstBorderControl = new ItemBorderControl();
			ElementFirstBorderControl.SetZIndex(1);
			Children.Add(ElementFirstBorderControl);
		}
		protected virtual void CreateElementSecondBorderControl() {
			if(ElementSecondBorderControl != null || SecondBorderNormalStateIsEmpty && (SecondBorderState == Bars.BorderState.Default ? (BorderState == Bars.BorderState.Normal || BorderState == Bars.BorderState.Default) : (SecondBorderState == Bars.BorderState.Normal))) return;
			ElementSecondBorderControl = new ItemBorderControl();
			ElementSecondBorderControl.SetZIndex(1);
			Children.Add(ElementSecondBorderControl);
		}
		protected virtual void CreateElementArrowControl() {
			if(ElementArrowControl != null) return;
			ElementArrowControl = new ArrowControl();
			ElementArrowControl.SetZIndex(3);
			Children.Add(ElementArrowControl);
		}
		protected virtual void CreateElementAdditionalContentHost() {
			if(ElementAdditionalContentHost != null) return;
			ElementAdditionalContentHost = new ContentControlEx();
			ElementAdditionalContentHost.HorizontalContentAlignment = AdditionalContentHorizontalAlignment;
			ElementAdditionalContentHost.SetZIndex(3);
			Children.Add(ElementAdditionalContentHost);
		}
		protected virtual void CreateElementTouchSplitter() {
			if (ElementTouchSplitterTransformPanel != null || !ShouldShowTouchSplitter)
				return;
			var elementTouchSplitter = new Control();
			ElementTouchSplitterTransformPanel = new LayoutTransformPanel() { };
			ElementTouchSplitterTransformPanel.Children.Add(elementTouchSplitter);
			ElementTouchSplitterTransformPanel.SetZIndex(0);
			Children.Add(ElementTouchSplitterTransformPanel);
		}
		#endregion
		public BarItemLayoutPanel() {
			IsUpdating = false;
			IsHitTestVisible = true;
			Background = new SolidColorBrush(Colors.Transparent);
			AdditionalContentSizeSettings = new SizeSettings();
		}
		#region Clear
		public virtual void Clear() {
			ClearValue(StyleProperty);
			UpdateFontSettings(true);
			ClearArrowControl();
			ClearBorderControls();
			ClearTextSplitterControl();			
		}
		protected virtual void ClearArrowControl() {
			if(ElementArrowControl != null) {
				ElementArrowControl.ClearValue(ArrowControl.TemplateProperty);
			}			
		}
		protected virtual void ClearTextSplitterControl() {
			if(ElementTextSplitter != null) {
				ElementTextSplitter.ClearValue(TextSplitterControl.StyleProperty);
			}
		}
		protected virtual void ClearBorderControls() {
			if(IsUpdating) return;
			if(ElementFirstBorderControl != null)
				ClearBorderControl(ElementFirstBorderControl);
			if(ElementSecondBorderControl != null)
				ClearBorderControl(ElementSecondBorderControl);
		}
		protected virtual void ClearBorderControl(ItemBorderControl borderControl) {
			borderControl.ClearValue(ItemBorderControl.NormalTemplateProperty);
			borderControl.ClearValue(ItemBorderControl.HoverTemplateProperty);
			borderControl.ClearValue(ItemBorderControl.PressedTemplateProperty);
			borderControl.ClearValue(ItemBorderControl.CustomizationTemplateProperty);
		}
		#endregion
		static Size CreateNewSize() { return new Size(); }
		static Size CreateNewSize(double width, double height) {
			width = Math.Max(0d, width);
			height = Math.Max(0d, height);
			return new Size(width, height);
		}
		#region Updates
		public bool IsUpdating { get; private set; }
		public void BeginUpdate() {
			IsUpdating = true;
		}
		public virtual void EndUpdate() {
			IsUpdating = false;
			ClearBorderControls();
			UpdateArrowControl();
			UpdateGlyph();
			UpdateKeyGesture();
			UpdateDescription();
			UpdateContent();
			UpdateContent2();
			UpdateBorderControl();
			UpdateGlyphBackground();
			UpdateAdditionalContentHost();
			UpdateTouchSplitter();			
		}
		protected virtual void UpdateFontSettings(bool clear = false) {
			if(clear) {
				ClearFontSettings(ElementContent);
				ClearFontSettings(ElementContent2);
				ClearFontSettings(ElementKeyGesture);
				ClearFontSettings(ElementDescription);
				if(ElementTextSplitter != null) {
					ElementTextSplitter.FontSettings = null;
				}
				return;
			}
			UpdateContentFontSettings();
			UpdateContent2FontSettings();
			UpdateKeyGestureFontSettings();
			UpdateDescriptionFontSettings();
		}
		protected virtual void UpdateContentFontSettings() {
			if(ElementContent != null)
				UpdateFontSettings(ElementContent, ContentFontSettings);
			if(ElementTextSplitter != null) {
				ElementTextSplitter.FontSettings = ContentFontSettings;
				ElementTextSplitter.BorderState = FirstBorderState == BorderState.Default ? BorderState : FirstBorderState;
			}
			if(ElementArrowControl != null)
				UpdateFontSettings(ElementArrowControl, ContentFontSettings);
		}
		protected virtual void UpdateImageColorizerSettings() {
			if (ElementGlyph == null || ImageColorizerSettings == null) return;
			ImageColorizerSettings.Apply(ElementGlyph, FirstBorderState == Bars.BorderState.Default ? BorderState : FirstBorderState);
		}
		protected virtual void UpdateContent2FontSettings() {
			if(ElementContent2 != null)
				UpdateFontSettings(ElementContent2, Content2FontSettings ?? ContentFontSettings);
		}
		protected virtual void UpdateKeyGestureFontSettings() {
			if(ElementKeyGesture != null)
				UpdateFontSettings(ElementKeyGesture, KeyGestureFontSettings ?? ContentFontSettings);
		}
		protected virtual void UpdateDescriptionFontSettings() {
			if(ElementDescription != null)
				UpdateFontSettings(ElementDescription, DescriptionFontSettings ?? ContentFontSettings);
		}
		protected virtual void ClearFontSettings(Control element) {
			FontSettings.Clear(element);
		}
		protected virtual void UpdateFontSettings(Control element, FontSettings settings) {
			if(element == null || settings == null) return;			
			BorderState actualState = BorderState;
			if(ShowSecondBorder && (SecondBorderPlacement == Bars.SecondBorderPlacement.ContentAndArrow || element == ElementArrowControl)) {
				actualState = SecondBorderState == Bars.BorderState.Default ? BorderState : SecondBorderState;
			} else {
				actualState = FirstBorderState == Bars.BorderState.Default ? BorderState : FirstBorderState;
			}
			settings.Apply(element, actualState);
		}
		protected virtual void UpdateGlyphBackground() {
			if(IsUpdating) return;
			if(!ShowGlyphBackground) return;
			CreateElementGlyphBackground();
			ElementGlyphBackground.Template = GlyphBackgroundTemplate;
			if (ActualGlyph!=null || GlyphTemplate!=null) {
				ElementGlyphBackground.HorizontalContentAlignment = HorizontalAlignment.Stretch;
				ElementGlyphBackground.VerticalContentAlignment = VerticalAlignment.Stretch;
			} else {
				ElementGlyphBackground.HorizontalContentAlignment = HorizontalAlignment.Center;
				ElementGlyphBackground.VerticalContentAlignment = VerticalAlignment.Center;
			}
			InvalidateMeasure();
		}
		protected virtual void UpdateArrowControl() {
			if(IsUpdating) return;
			if(!ActualShowArrow) return;
			CreateElementArrowControl();
			ElementArrowControl.Template = ArrowTemplate ?? ResourceStorage.TryFindResourceInStorage(this, ArrowThemeKey) as ControlTemplate;
			ElementArrowControl.RenderTransform = ArrowTransform;
			ElementArrowControl.RenderTransformOrigin = new Point(0.5d, 0.5d);
			InvalidateMeasure();
		}
		protected virtual void UpdateGlyph() {
			if(IsUpdating) return;
			if (ShowGlyph) {
				FrameworkElement targetElement = null;
				if (GlyphTemplate == null) {
					CreateElementGlyph();
					targetElement = ElementGlyph;
					ElementGlyph.Source = ActualGlyph;
				} else {
					CreateElementTemplatedGlyph();
					targetElement = ElementTemplatedGlyph;
					ElementTemplatedGlyph.ContentTemplate = GlyphTemplate;
				}
				targetElement.Width = GlyphSize == GlyphSize.Large ? 32d : GlyphSize == GlyphSize.Small ? 16d : Double.NaN;
				targetElement.Height = GlyphSize == GlyphSize.Large ? 32d : GlyphSize == GlyphSize.Small ? 16d : Double.NaN;
				Thickness thickness = new Thickness();
				if (targetElement.Width < MaxGlyphSize.Width)
					thickness.Left = thickness.Right = (MaxGlyphSize.Width - targetElement.Width) / 2;
				if (targetElement.Height < MaxGlyphSize.Height)
					thickness.Top = thickness.Bottom = (MaxGlyphSize.Height - targetElement.Height) / 2;
				targetElement.Margin = thickness;
				targetElement.RenderTransform = GlyphRenderTransform ?? Transform.Identity;
				targetElement.RenderTransformOrigin = new Point(0.5, 0.5);
				ImageColorizer.SetIsEnabled(targetElement, ColorizeGlyph);
			}
			InvalidateMeasure();
		}
		protected virtual void UpdateKeyGesture() {
			if(IsUpdating) return;
			if(ShowKeyGesture) {
				CreateElementKeyGesture();
				ElementKeyGesture.Content = KeyGesture;
			}
			InvalidateMeasure();
		}
		protected virtual void UpdateDescription() {
			if(IsUpdating) return;
			if(ShowDescription) {
				CreateElementDescription();
				ElementDescription.Content = Description;
			}
			InvalidateMeasure();
		}
		protected virtual void UpdateContent2() {
			if(IsUpdating) return;
			if(ShowContent2) {
				CreateElementContent2();
				ElementContent2.Content = Content2;
				ElementContent2.ContentTemplate = Content2Template;
			}
			InvalidateMeasure();
		}
		protected virtual void UpdateTouchSplitter() {
			if (IsUpdating)
				return;
			CreateElementTouchSplitter();
			UpdateElementTouchSplitterTemplate();
			UpdateTouchSplitterOrientation();
		}
		protected virtual void UpdateTouchSplitterOrientation() {
			if (!ActualShowTouchSplitter)
				return;
			var isVerticalOrientation = (ContentAndGlyphToArrowAlignment == Dock.Left || ContentAndGlyphToArrowAlignment == Dock.Right);
			ElementTouchSplitterTransformPanel.Orientation = isVerticalOrientation ? Orientation.Vertical : Orientation.Horizontal;
		}
		static Dictionary<int,ControlTemplate> accessTextControlTemplate = new Dictionary<int,ControlTemplate>();
		static object olock = new object();
		static ControlTemplate AccessTextControlTemplate {
			get {
				lock (olock) {
					if (!accessTextControlTemplate.ContainsKey(System.Threading.Thread.CurrentThread.ManagedThreadId)) {
						string text = @"<ControlTemplate " +
		"xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation' " +
		"xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml' TargetType='{x:Type ContentControl}'>" +
	 "<ContentPresenter RecognizesAccessKey='True'/>" +
	"</ControlTemplate>";
						accessTextControlTemplate[System.Threading.Thread.CurrentThread.ManagedThreadId] = (ControlTemplate)System.Windows.Markup.XamlReader.Parse(text);
					}
				}
				return accessTextControlTemplate[System.Threading.Thread.CurrentThread.ManagedThreadId];
			}
		}
		protected virtual void UpdateContent() {
			if(IsUpdating) return;
			if(ShowContent) {
				if(SplitContent && ContentTemplate == null) {
					CreateElementTextSplitter();
					ElementTextSplitter.Content = Content;
					UpdateElementTextSplitterStyle();
					UpdateElementTouchSplitterTemplate();
				} else {
					CreateElementContent();
					ElementContent.Content = Content;
					ElementContent.ContentTemplate = ContentTemplate;
					ElementContent.Template = AccessTextControlTemplate;
					ElementContent.ContentTemplateSelector = ContentTemplateSelector;
				}
				if (ElementContent != null) {
					ElementContent.HorizontalContentAlignment = ContentHorizontalAlignment;
					ElementContent.InvalidateMeasure();
				}
			}			
			InvalidateMeasure();
		}
		protected virtual void UpdateElementTextSplitterStyle() {
			if(!ShowContent || !SplitContent || ElementTextSplitter == null) return;
			if(TextSplitterStyleKey == null) {
				ElementTextSplitter.ClearValue(TextSplitterControl.StyleProperty);
				return;
			}
			ElementTextSplitter.Style = ResourceStorage.TryFindResourceInStorage(this, TextSplitterStyleKey) as Style;
		}
		protected virtual void UpdateElementTouchSplitterTemplate() {
			if (!ActualShowTouchSplitter)
				return;
			var touchSplitter = ElementTouchSplitterTransformPanel.Children[0] as Control;
			if (TouchSplitterThemeKey == null) {
				touchSplitter.ClearValue(Control.TemplateProperty);
				return;
			}
			touchSplitter.Template = ResourceStorage.TryFindResourceInStorage(this, TouchSplitterThemeKey) as ControlTemplate;
		}
		protected virtual void UpdateElementTextSplitterIsArrowVisible() {
			if(ElementTextSplitter == null) return;
			ElementTextSplitter.IsArrowVisible = ShowArrowInTextSplitter;
		}
		protected virtual void UpdateAdditionalContentHost() {
			if(IsUpdating) return;
			if(!ShowAdditionalContent) return;
			CreateElementAdditionalContentHost();
			ElementAdditionalContentHost.Content = AdditionalContent;
			ElementAdditionalContentHost.ContentTemplate = AdditionalContentTemplate;
			ElementAdditionalContentHost.VerticalContentAlignment = StretchAdditionalContentVertically ? VerticalAlignment.Stretch : VerticalAlignment.Center;
			if(AdditionalContent != null) AdditionalContent.Style = additionalContentStyle;
			InvalidateMeasure();
		}
		protected virtual void UpdateBorderControl() {
			if(IsUpdating) return;
			bool invalidate = false;
			invalidate |= !ShowFirstBorder && ElementFirstBorderControl != null || !ShowSecondBorder && ElementSecondBorderControl != null;
			var themeKeyExtension = BorderThemeKey;
			if(ShowFirstBorder) {
				CreateElementFirstBorderControl();				
				UpdateBorderControlTemplate(ElementFirstBorderControl, themeKeyExtension, FirstBorderState == Bars.BorderState.Default ? BorderState : FirstBorderState, FirstBorderUseNormalStateOnly);
			}
			if(ShowSecondBorder) {
				CreateElementSecondBorderControl();
				UpdateBorderControlTemplate(ElementSecondBorderControl, themeKeyExtension, SecondBorderState == Bars.BorderState.Default ? BorderState : SecondBorderState, SecondBorderUseNormalStateOnly);
			}			
			UpdateBorderControlsState();
			if(FirstBorderShowing && SecondBorderShowing) {
				switch(ContentAndGlyphToArrowAlignment) {
					case Dock.Bottom:
						ElementFirstBorderControl.HideBorderSide = HideBorderSide.Top;
						ElementSecondBorderControl.HideBorderSide = HideBorderSide.Bottom;
						break;
					case Dock.Left:
						ElementFirstBorderControl.HideBorderSide = HideBorderSide.Right;
						ElementSecondBorderControl.HideBorderSide = HideBorderSide.Left;
						break;
					case Dock.Right:
						ElementFirstBorderControl.HideBorderSide = HideBorderSide.Left;
						ElementSecondBorderControl.HideBorderSide = HideBorderSide.Right;
						break;
					case Dock.Top:
						ElementFirstBorderControl.HideBorderSide = HideBorderSide.Bottom;
						ElementSecondBorderControl.HideBorderSide = HideBorderSide.Top;
						break;
					default:
						break;
				}
			} else {
				if(ElementFirstBorderControl!=null) ElementFirstBorderControl.HideBorderSide = HideBorderSide.None;
				if(ElementSecondBorderControl != null) ElementSecondBorderControl.HideBorderSide = HideBorderSide.None;
			}
			if(FirstBorderShowing) {
				ElementFirstBorderControl.IsActive = IsFirstBorderActive;
				ElementFirstBorderControl.ItemPosition = FirstBorderItemPosition == HorizontalItemPositionType.Default ? ItemPosition : FirstBorderItemPosition;
			}
			if(SecondBorderShowing) {
				ElementSecondBorderControl.IsActive = IsSecondBorderActive;
				ElementSecondBorderControl.ItemPosition = SecondBorderItemPosition == HorizontalItemPositionType.Default ? ItemPosition : SecondBorderItemPosition;
			}
			if(invalidate) InvalidateMeasure();
		}
		protected virtual void UpdateBorderControlsState() {
			if(FirstBorderShowing) ElementFirstBorderControl.State = FirstBorderState == Bars.BorderState.Default ? BorderState : FirstBorderState;
			if(SecondBorderShowing) ElementSecondBorderControl.State = SecondBorderState == Bars.BorderState.Default ? BorderState : SecondBorderState;
		}
		protected virtual void UpdateBorderControlTemplate(ItemBorderControl borderControl, BarItemBorderThemeKeyExtension themeKeyExtension, BorderState state, bool useNormalState) {
			if (themeKeyExtension == null)
				return;
			if(useNormalState) {
				ModifyThemeKey(ref themeKeyExtension, BarItemBorderThemeKeys.Normal);
				UpdateNormalTemplate(borderControl, themeKeyExtension);
				return;
			}
			switch(state) {
				case BorderState.Indeterminate:
				case BorderState.Normal:
					ModifyThemeKey(ref themeKeyExtension, BarItemBorderThemeKeys.Normal);
				UpdateNormalTemplate(borderControl, themeKeyExtension);
				return;
				case BorderState.Hover:
				case BorderState.HoverChecked:
					ModifyThemeKey(ref themeKeyExtension, BarItemBorderThemeKeys.Hover);
					UpdateHoverTemplate(borderControl, themeKeyExtension);
					return;
				case BorderState.Pressed:
				case BorderState.Checked:
					ModifyThemeKey(ref themeKeyExtension, BarItemBorderThemeKeys.Pressed);
					UpdatePressedTemplate(borderControl, themeKeyExtension);
					return;
				case BorderState.Customization:
					ModifyThemeKey(ref themeKeyExtension, BarItemBorderThemeKeys.Customization);
					UpdateCustomizationTemplate(borderControl, themeKeyExtension);
					return;
				case BorderState.Default:
					return;
			}			
		}
		protected virtual void ModifyThemeKey(ref BarItemBorderThemeKeyExtension themeKeyExtension, BarItemBorderThemeKeys key) {
			themeKeyExtension.ResourceKey = key;
			themeKeyExtension.ThemeName = ThemeName;
		}
		protected virtual void UpdateCustomizationTemplate(ItemBorderControl borderControl, BarItemBorderThemeKeyExtension themeKeyExtension) {
			ControlTemplate template = null;
			if(borderControl == null) return;
			if(borderControl.CustomizationTemplate == null)
				template = CustomizationBorderTemplate ?? ResourceStorage.TryFindResourceInStorage(this, themeKeyExtension) as ControlTemplate;
			else return;
			borderControl.CustomizationTemplate = template;
		}
		protected virtual void UpdatePressedTemplate(ItemBorderControl borderControl, BarItemBorderThemeKeyExtension themeKeyExtension) {
			ControlTemplate template = null;
			if(borderControl == null) return;
			if(borderControl.PressedTemplate == null)
				template = PressedBorderTemplate ?? ResourceStorage.TryFindResourceInStorage(this, themeKeyExtension) as ControlTemplate;
			else return;
			borderControl.PressedTemplate = template;
		}
		protected virtual void UpdateHoverTemplate(ItemBorderControl borderControl, BarItemBorderThemeKeyExtension themeKeyExtension) {
			ControlTemplate template = null;
			if(borderControl == null) return;
			if(borderControl.HoverTemplate == null)
				template = HoverBorderTemplate ?? ResourceStorage.TryFindResourceInStorage(this, themeKeyExtension) as ControlTemplate;
			else return;
			borderControl.HoverTemplate = template;
		}
		protected virtual void UpdateNormalTemplate(ItemBorderControl borderControl, BarItemBorderThemeKeyExtension themeKeyExtension) {
			ControlTemplate template = null;
			if(borderControl == null) return;
			if(borderControl.NormalTemplate == null)
				template = NormalBorderTemplate ?? ResourceStorage.TryFindResourceInStorage(this, themeKeyExtension) as ControlTemplate;
			else return;
			borderControl.NormalTemplate = template;
		}
		#endregion
		#region Measure Arrange
		protected Size ContentDesiredSize { get; set; }
		protected Size ContentAndGlyphDesiredSize { get; set; }
		protected override Size MeasureOverride(Size availableSize) {
			ClearAutomationEventsHelper.ClearAutomationEvents();
			return MeasureButton(availableSize);
		}
		protected override Size ArrangeOverride(Size finalSize) {
			return ArrangeButton(finalSize);
		}
		protected bool ActualShowAdditionalContent { get { return AdditionalContent != null || AdditionalContentTemplate != null; } }
		protected bool ShowAnyContent { get { return ShowContent || ShowContent2 || ShowDescription || ShowKeyGesture || ActualShowAdditionalContent; } }
		protected virtual Size MeasureContents(Size measureSize) {
			if(!ShowAnyContent)
				return CreateNewSize();
			if(ShowContent && !ShowContent2 && !ShowDescription && !ShowKeyGesture && !ShowAdditionalContent) {
				FrameworkElement targetElement = SplitContent && ContentTemplate == null ? (FrameworkElement)ElementTextSplitter : (FrameworkElement)ElementContent;
				targetElement.Measure(SizeHelper.Infinite);
				return CreateNewSize(targetElement.DesiredSize.Width + GetLeftRight(ContentMargin) + GetLeftRight(actualCommonContentMargin), targetElement.DesiredSize.Height + GetTopBottom(ContentMargin) + GetTopBottom(actualCommonContentMargin));
			}
			if(ShowAdditionalContent) {
				double dHeight = 0d;
				double dWidth = 0d;
				bool measurecontent = ShowContent && (ElementContent != null);
				bool measureadditionalcontent = ShowAdditionalContent && ((AdditionalContent != null || AdditionalContentTemplate != null) && ElementAdditionalContentHost != null);
				bool measurecontent2 = ShowContent2 && (ElementContent2 != null);
				Thickness contentmargin = !measurecontent ? new Thickness(0) : ContentMargin;
				Thickness additionalcontentmargin = !measureadditionalcontent ? new Thickness(0) : AdditionalContentMargin;
				Thickness content2margin = !measurecontent2 ? new Thickness(0) : Content2Margin;
				if(measurecontent) {
					ElementContent.Measure(SizeHelper.Infinite);
					dHeight += ElementContent.DesiredSize.Height + GetTopBottom(ContentMargin);
					dWidth += ElementContent.DesiredSize.Width + GetLeftRight(ContentMargin);
				}
				if(measureadditionalcontent) {
					ElementAdditionalContentHost.Measure(CreateNewSize(double.PositiveInfinity, measureSize.Height - GetTopBottom(actualCommonContentMargin) - GetTopBottom(additionalcontentmargin)));
					dWidth += ElementAdditionalContentHost.DesiredSize.Width + GetLeftRight(additionalcontentmargin);
					dHeight = Math.Max(dHeight, ElementAdditionalContentHost.DesiredSize.Height + GetTopBottom(additionalcontentmargin));
				}
				if(measurecontent2) {
					ElementContent2.Measure(SizeHelper.Infinite);
					dWidth += ElementContent2.DesiredSize.Width + GetLeftRight(content2margin);
					dHeight = Math.Max(dHeight, ElementContent2.DesiredSize.Height + GetTopBottom(content2margin));
				}
				return CreateNewSize(dWidth + GetLeftRight(actualCommonContentMargin), dHeight + GetTopBottom(actualCommonContentMargin));
			}
			if(ShowContent && ShowKeyGesture && !ShowDescription && !ShowAdditionalContent) {
				ElementContent.Measure(SizeHelper.Infinite);
				ElementKeyGesture.Measure(SizeHelper.Infinite);
				return CreateNewSize(ElementContent.DesiredSize.Width + ElementKeyGesture.DesiredSize.Width + GetLeftRight(ContentMargin) + GetLeftRight(KeyGestureMargin) + GetLeftRight(actualCommonContentMargin),
					Math.Max(ElementContent.DesiredSize.Height + GetTopBottom(ContentMargin), ElementKeyGesture.DesiredSize.Height + GetTopBottom(KeyGestureMargin)) + GetTopBottom(actualCommonContentMargin));
			}
			if (ShowContent && !ShowKeyGesture && ShowDescription && !ShowAdditionalContent) {
				ElementContent.Measure(SizeHelper.Infinite);
				ElementDescription.Measure(SizeHelper.Infinite);
				return CreateNewSize(
					Math.Max(ElementContent.DesiredSize.Width + GetLeftRight(ContentMargin), ElementDescription.DesiredSize.Width + GetLeftRight(DescriptionMargin)) + GetLeftRight(actualCommonContentMargin),
					ElementContent.DesiredSize.Height + GetTopBottom(ContentMargin) + ElementDescription.DesiredSize.Height + GetTopBottom(DescriptionMargin) + GetTopBottom(actualCommonContentMargin)
					);
			}
			if(ShowContent && ShowKeyGesture && ShowDescription && !ShowAdditionalContent) {
				ElementContent.Measure(SizeHelper.Infinite);
				ElementDescription.Measure(SizeHelper.Infinite);
				ElementKeyGesture.Measure(SizeHelper.Infinite);
				return CreateNewSize(
					Math.Max(ElementContent.DesiredSize.Width + GetLeftRight(ContentMargin), ElementDescription.DesiredSize.Width + GetLeftRight(DescriptionMargin)) + ElementKeyGesture.DesiredSize.Width + GetLeftRight(KeyGestureMargin) + GetLeftRight(actualCommonContentMargin),
					Math.Max(ElementContent.DesiredSize.Height + GetTopBottom(ContentMargin) + ElementDescription.DesiredSize.Height + GetTopBottom(DescriptionMargin), ElementKeyGesture.DesiredSize.Height + GetTopBottom(KeyGestureMargin)) + GetTopBottom(actualCommonContentMargin)
					);
			}
			return CreateNewSize();
		}
		protected virtual void ArrangeContents(Point startPoint, Size availableSize) {
			if(!ShowAnyContent) return;
			double sX = startPoint.X + actualCommonContentMargin.Left;
			double sY = startPoint.Y + actualCommonContentMargin.Top;
			availableSize.Width = Math.Max(0, availableSize.Width - GetLeftRight(actualCommonContentMargin));
			availableSize.Height = Math.Max(0, availableSize.Height - GetTopBottom(actualCommonContentMargin));
			if(ShowContent && !ShowContent2 && !ShowDescription && !ShowKeyGesture && !ShowAdditionalContent) {
				FrameworkElement targetElement = SplitContent && ContentTemplate == null ? (FrameworkElement)ElementTextSplitter : (FrameworkElement)ElementContent;
				Size arrangeSize = HorizontalAlignment == HorizontalAlignment.Stretch ? CreateNewSize(Math.Max(availableSize.Width - GetLeftRight(ContentMargin), 0d), targetElement.DesiredSize.Height) : targetElement.DesiredSize;
				ArrangeElement(targetElement, new Rect(
					new Point(sX + (GlyphToContentAlignment == Dock.Left ? 0d : (availableSize.Width - arrangeSize.Width - GetLeftRight(ContentMargin)) / 2) + ContentMargin.Left, sY + (availableSize.Height - arrangeSize.Height - GetTopBottom(ContentMargin)) / 2 + ContentMargin.Top)
					, arrangeSize));
				return;
			}
			if(ShowAdditionalContent) {
				double maxHeight = availableSize.Height;
				double content1Size = 0d;
				double content2Size = 0d;
				bool arrangecontent = ShowContent && (ElementContent != null);
				bool arrangeadditionalcontent = ShowAdditionalContent && ((AdditionalContent != null || AdditionalContentTemplate != null) && ElementAdditionalContentHost != null);
				bool arrangecontent2 = ShowContent2 && (ElementContent2 != null);
				Thickness contentmargin = !arrangecontent ? new Thickness(0) : ContentMargin;
				Thickness additionalcontentmargin = !arrangeadditionalcontent ? new Thickness(0) : AdditionalContentMargin;
				Thickness content2margin = !arrangecontent2 ? new Thickness(0) : Content2Margin;
				if(arrangecontent) {
					content1Size += contentmargin.Left;
					ArrangeElement(ElementContent, new Rect(
						new Point(sX + contentmargin.Left, sY + maxHeight / 2 - (ElementContent.DesiredSize.Height + GetTopBottom(contentmargin)) / 2 + contentmargin.Top),
						ElementContent.DesiredSize));
					content1Size += contentmargin.Right + ElementContent.DesiredSize.Width;
				}
				switch (AdditionalContentHorizontalAlignment) {
					case HorizontalAlignment.Left:
						if (arrangeadditionalcontent) {
							ArrangeElement(ElementAdditionalContentHost, new Rect(
								new Point(sX + additionalcontentmargin.Left + content1Size, sY + maxHeight / 2 - (ElementAdditionalContentHost.DesiredSize.Height + GetTopBottom(contentmargin)) / 2 + contentmargin.Top),
								CreateNewSize(ElementAdditionalContentHost.DesiredSize.Width, StretchAdditionalContentVertically ? availableSize.Height - GetTopBottom(additionalcontentmargin) : ElementAdditionalContentHost.DesiredSize.Height)));
							sX += ElementAdditionalContentHost.DesiredSize.Width + GetLeftRight(additionalcontentmargin) + content1Size;
						}
						if (arrangecontent2) {
							ArrangeElement(ElementContent2, new Rect(
								new Point(sX + content2margin.Left, sY + maxHeight / 2 - (ElementContent2.DesiredSize.Height + GetTopBottom(contentmargin)) / 2 + contentmargin.Top),
								ElementContent2.DesiredSize));							
						}						
						break;
					case HorizontalAlignment.Center:
						var delta = ((arrangeadditionalcontent ? ElementAdditionalContentHost.DesiredSize.Width : 0d) + (arrangecontent2 ? ElementContent2.DesiredSize.Width : 0d) + GetLeftRight(additionalcontentmargin) + GetLeftRight(content2margin));
						if (availableSize.Width - content1Size > delta)
							delta = (availableSize.Width - content1Size - delta) / 2d;
						else
							delta = 0d;
						sX += delta;
						if (arrangeadditionalcontent) {
							ArrangeElement(ElementAdditionalContentHost, new Rect(
								new Point(sX + content1Size + additionalcontentmargin.Left, sY + maxHeight / 2 - (ElementAdditionalContentHost.DesiredSize.Height + GetTopBottom(contentmargin)) / 2 + contentmargin.Top),
								CreateNewSize(ElementAdditionalContentHost.DesiredSize.Width, StretchAdditionalContentVertically ? availableSize.Height - GetTopBottom(additionalcontentmargin) : ElementAdditionalContentHost.DesiredSize.Height)));
							sX += ElementAdditionalContentHost.DesiredSize.Width + GetLeftRight(additionalcontentmargin);
						}
						if (arrangecontent2) {
							ArrangeElement(ElementContent2, new Rect(
								new Point(sX + content1Size + content2margin.Left, sY + maxHeight / 2 - (ElementContent2.DesiredSize.Height + GetTopBottom(contentmargin)) / 2 + contentmargin.Top),
								ElementContent2.DesiredSize));							
						}   
						break;
					case HorizontalAlignment.Right:
						 if (arrangecontent2) {							 
							ArrangeElement(ElementContent2, new Rect(
								new Point(sX + availableSize.Width - ElementContent2.DesiredSize.Width - content2margin.Right, sY + maxHeight / 2 - (ElementContent2.DesiredSize.Height + GetTopBottom(contentmargin)) / 2 + contentmargin.Top),
								ElementContent2.DesiredSize));
							content2Size = ElementContent2.DesiredSize.Width + GetLeftRight(content2margin);
						}
						if (arrangeadditionalcontent) {							
							ArrangeElement(ElementAdditionalContentHost, new Rect(
								new Point(sX + availableSize.Width - content2Size - additionalcontentmargin.Right - ElementAdditionalContentHost.DesiredSize.Width, sY + maxHeight / 2 - (ElementAdditionalContentHost.DesiredSize.Height + GetTopBottom(contentmargin)) / 2 + contentmargin.Top),
								CreateNewSize(ElementAdditionalContentHost.DesiredSize.Width, StretchAdditionalContentVertically ? availableSize.Height - GetTopBottom(additionalcontentmargin) : ElementAdditionalContentHost.DesiredSize.Height)));
						}
						break;
					case HorizontalAlignment.Stretch:
				if(arrangecontent2) {
				ArrangeElement(ElementContent2, new Rect(
					new Point(sX + availableSize.Width - ElementContent2.DesiredSize.Width - content2margin.Right, sY + maxHeight / 2 - (ElementContent2.DesiredSize.Height + GetTopBottom(contentmargin)) / 2 + contentmargin.Top),
					ElementContent2.DesiredSize));
				content2Size = ElementContent2.DesiredSize.Width + GetLeftRight(content2margin);
				}
				if(arrangeadditionalcontent) {
					ArrangeElement(ElementAdditionalContentHost, new Rect(
						new Point(sX + content1Size + additionalcontentmargin.Left, sY + maxHeight / 2 - (ElementAdditionalContentHost.DesiredSize.Height + GetTopBottom(contentmargin)) / 2 + contentmargin.Top),
								CreateNewSize((availableSize.Width - content1Size - GetLeftRight(additionalcontentmargin) - content2Size), StretchAdditionalContentVertically ? availableSize.Height - GetTopBottom(additionalcontentmargin) : ElementAdditionalContentHost.DesiredSize.Height)));
						}
						break;
				}
				return;
			}
			if(ShowContent && ShowKeyGesture && !ShowDescription && !ShowAdditionalContent) {
				ArrangeElement(ElementContent, new Rect(
					new Point(sX + ContentMargin.Left, sY + availableSize.Height / 2 - (ElementContent.DesiredSize.Height + GetTopBottom(ContentMargin)) / 2),
					ElementContent.DesiredSize));
				ArrangeElement(ElementKeyGesture, new Rect(
					new Point(sX + availableSize.Width - KeyGestureMargin.Right - ElementKeyGesture.DesiredSize.Width, sY + availableSize.Height / 2 - (ElementKeyGesture.DesiredSize.Height + GetTopBottom(KeyGestureMargin)) / 2),
					ElementKeyGesture.DesiredSize));
				return;
			}
			if (ShowContent && !ShowKeyGesture && ShowDescription && !ShowAdditionalContent) {
				ArrangeElement(ElementContent, new Rect(
					new Point(sX + ContentMargin.Left, sY + ContentMargin.Top),
					ElementContent.DesiredSize));
				ArrangeElement(ElementDescription, new Rect(
					new Point(sX + DescriptionMargin.Left, sY + DescriptionMargin.Top + ContentMargin.Top + ElementContent.DesiredSize.Height),
					ElementDescription.DesiredSize));				
			}
			if(ShowContent && ShowKeyGesture && ShowDescription && !ShowAdditionalContent) {
				ArrangeElement(ElementContent, new Rect(
					new Point(sX + ContentMargin.Left, sY + ContentMargin.Top),
					ElementContent.DesiredSize));
				ArrangeElement(ElementDescription, new Rect(
					new Point(sX + DescriptionMargin.Left, sY + DescriptionMargin.Top + ContentMargin.Top + ElementContent.DesiredSize.Height),
					ElementDescription.DesiredSize));
				ArrangeElement(ElementKeyGesture, new Rect(
					new Point(sX + availableSize.Width - KeyGestureMargin.Right - ElementKeyGesture.DesiredSize.Width, sY + availableSize.Height / 2 - (ElementKeyGesture.DesiredSize.Height + GetTopBottom(KeyGestureMargin)) / 2 + KeyGestureMargin.Top),
					ElementKeyGesture.DesiredSize));
				return;
			}
		}		
		List<UIElement> elementsToArrage = new List<UIElement>();
		void BeginArrange() {
			if(ElementGlyph != null) elementsToArrage.Add(ElementGlyph);
			if(ElementTemplatedGlyph != null) elementsToArrage.Add(ElementTemplatedGlyph);
			if(ElementContent != null) elementsToArrage.Add(ElementContent);
			if(ElementContent2 != null) elementsToArrage.Add(ElementContent2);
			if(ElementDescription != null) elementsToArrage.Add(ElementDescription);
			if(ElementKeyGesture != null) elementsToArrage.Add(ElementKeyGesture);
			if(ElementTextSplitter != null) elementsToArrage.Add(ElementTextSplitter);
			if(ElementFirstBorderControl != null) elementsToArrage.Add(ElementFirstBorderControl);
			if(ElementSecondBorderControl != null) elementsToArrage.Add(ElementSecondBorderControl);
			if(ElementArrowControl != null) elementsToArrage.Add(ElementArrowControl);
			if(ElementGlyphBackground != null) elementsToArrage.Add(ElementGlyphBackground);
			if(ElementAdditionalContentHost != null) elementsToArrage.Add(ElementAdditionalContentHost);
			if(ElementTouchSplitterTransformPanel != null) elementsToArrage.Add(ElementTouchSplitterTransformPanel);
		}
		void EndArrange() {
			foreach(var item in elementsToArrage)
				item.Arrange(new Rect(new Point(), CreateNewSize()));
			elementsToArrage.Clear();
		}
		void ArrangeElement(UIElement element, Rect rect) {
			if(elementsToArrage != null && elementsToArrage.Contains(element))
				elementsToArrage.Remove(element);
			element.Arrange(rect);
		}
		protected virtual Size ArrangeButton(Size finalSize) {
			BeginArrange();
			Size retValue = finalSize;
			Point glyphStartPoint = new Point();
			Point contentStartPoint = new Point();
			Point arrowStartPoint = new Point();
			finalSize.Height = Math.Max(0d, finalSize.Height - GetTopBottom(CommonMargin));
			finalSize.Width = Math.Max(0d, finalSize.Width - GetLeftRight(CommonMargin));
			Size contentAvailableSize = CreateNewSize();
			Size glyphDesiredSize = ShowGlyph ? ElementTargetGlyph.DesiredSize : CreateNewSize();
			Thickness glyphThickness = ShowGlyph ? actualGlyphMargin : new Thickness();
			bool verticalGlyph = ShowGlyph == true && (GlyphToContentAlignment == Dock.Top || GlyphToContentAlignment == Dock.Bottom);
			bool verticalArrow = ContentAndGlyphToArrowAlignment == Dock.Top || ContentAndGlyphToArrowAlignment == Dock.Bottom;
			bool sameAlignment = verticalGlyph == verticalArrow;
			bool inPopupMenu = ShowContent && ShowKeyGesture && !ShowDescription;
			bool inApplicationMenu = ShowContent && ShowKeyGesture && ShowDescription;
			bool inMenu = inPopupMenu || inApplicationMenu;
			Size arrowDesiredSize = ActualShowArrow ? CreateNewSize(Math.Max(0d, GetLeftRight(actualArrowMargin) + ElementArrowControl.DesiredSize.Width), GetTopBottom(actualArrowMargin) + ElementArrowControl.DesiredSize.Height) : CreateNewSize();
			if(verticalGlyph) {
				if (ShowGlyph) {
					var gspleft = finalSize.Width / 2 - (ElementTargetGlyph.DesiredSize.Width + GetLeftRight(actualGlyphMargin)) / 2 + actualGlyphMargin.Left;
					double gsptop = 0d;
					if (ShowAnyContent || SplitContent) {
						gsptop = GlyphToContentAlignment == Dock.Top ? actualGlyphMargin.Top : finalSize.Height - glyphDesiredSize.Height - glyphThickness.Bottom;
					} else {
						gsptop = (finalSize.Height - glyphDesiredSize.Height - GetTopBottom(glyphThickness)) / 2 + glyphThickness.Bottom;
					}
					glyphStartPoint = new Point(gspleft, gsptop);
				}
				contentStartPoint = new Point(0d, GlyphToContentAlignment == Dock.Top ? glyphDesiredSize.Height + GetTopBottom(glyphThickness) : 0d);
				contentAvailableSize = CreateNewSize(Math.Max(0d, finalSize.Width - contentStartPoint.X), Math.Max(0d, finalSize.Height - glyphDesiredSize.Height - GetTopBottom(glyphThickness)));
			} else {
				if(ShowGlyph) glyphStartPoint = new Point(GlyphToContentAlignment == Dock.Left ? actualGlyphMargin.Left : finalSize.Width - glyphDesiredSize.Width - actualGlyphMargin.Right, finalSize.Height / 2 - (ElementTargetGlyph.DesiredSize.Height + GetTopBottom(actualGlyphMargin)) / 2 + actualGlyphMargin.Top);
				contentStartPoint = new Point(GlyphToContentAlignment == Dock.Left ? GetLeftRight(glyphThickness) + glyphDesiredSize.Width : 0d, 0d);
				contentAvailableSize = CreateNewSize(Math.Max(finalSize.Width - glyphDesiredSize.Width - GetLeftRight(glyphThickness), 0), finalSize.Height);
			}
			if(ActualShowArrow) {
				double xdelta = (ElementArrowControl.DesiredSize.Width + GetLeftRight(actualArrowMargin));
				double ydelta = (ElementArrowControl.DesiredSize.Height + GetTopBottom(actualArrowMargin));
				switch(ContentAndGlyphToArrowAlignment) {
					case Dock.Top:
						arrowStartPoint.X = (finalSize.Width - ElementArrowControl.DesiredSize.Width - GetLeftRight(actualArrowMargin)) / 2 + actualArrowMargin.Left;
						arrowStartPoint.Y = (finalSize.Height - ElementArrowControl.DesiredSize.Height - actualArrowMargin.Bottom);
						if(!verticalGlyph) {
							glyphStartPoint.Y -= arrowDesiredSize.Height / 2;
						}
						if(GlyphToContentAlignment == Dock.Bottom)
							glyphStartPoint.Y -= arrowDesiredSize.Height;
						break;
					case Dock.Right:
						arrowStartPoint.Y = (finalSize.Height - ElementArrowControl.DesiredSize.Height - GetTopBottom(actualArrowMargin)) / 2 + actualArrowMargin.Top;
						arrowStartPoint.X = actualArrowMargin.Left;
						if(verticalGlyph && !inMenu) {
							contentStartPoint.X += arrowDesiredSize.Width;
						} else {
							contentStartPoint.X += arrowDesiredSize.Width;
							if(GlyphToContentAlignment == Dock.Left)
								glyphStartPoint.X += arrowDesiredSize.Width;
						}
						break;
					case Dock.Left:
						arrowStartPoint.Y = (finalSize.Height - ElementArrowControl.DesiredSize.Height - GetTopBottom(actualArrowMargin)) / 2 + actualArrowMargin.Top;
						arrowStartPoint.X = (finalSize.Width - ElementArrowControl.DesiredSize.Width - actualArrowMargin.Right);
						if(GlyphToContentAlignment == Dock.Right)
							glyphStartPoint.X -= arrowDesiredSize.Width;
						if(verticalGlyph) {
							glyphStartPoint.X -= arrowDesiredSize.Width / 2;
						}
						break;
					case Dock.Bottom:
						arrowStartPoint.X = (finalSize.Width - ElementArrowControl.DesiredSize.Width - GetLeftRight(actualArrowMargin)) / 2 + actualArrowMargin.Left;
						arrowStartPoint.Y = actualArrowMargin.Top;
						if(!verticalGlyph) {
							glyphStartPoint.Y += arrowDesiredSize.Height / 2;
							contentStartPoint.Y += arrowDesiredSize.Height;
						} else {
							if(GlyphToContentAlignment != Dock.Bottom) {
								glyphStartPoint.Y += arrowDesiredSize.Height;
							}
							contentStartPoint.Y += arrowDesiredSize.Height;
						}
						break;
				}
				if (verticalArrow)
					contentAvailableSize.Height = Math.Max(0d, contentAvailableSize.Height - (ElementArrowControl.DesiredSize.Height + GetTopBottom(actualArrowMargin)));
				else
					contentAvailableSize.Width = Math.Max(contentAvailableSize.Width - (ElementArrowControl.DesiredSize.Width + GetLeftRight(actualArrowMargin)), 0d);
			}
			glyphStartPoint.X += CommonMargin.Left;
			contentStartPoint.X += CommonMargin.Left;
			arrowStartPoint.X += CommonMargin.Left;
			glyphStartPoint.Y += CommonMargin.Top;
			contentStartPoint.Y += CommonMargin.Top;
			arrowStartPoint.Y += CommonMargin.Top;
			if(ShowGlyph) {
				if (ElementTargetGlyph != ElementTemplatedGlyph) {
					ArrangeElement(ElementTargetGlyph, new Rect(glyphStartPoint, ElementTargetGlyph.DesiredSize));
				} else {
					ArrangeElement(ElementTargetGlyph, new Rect(
						new Point(Math.Ceiling(glyphStartPoint.X), Math.Ceiling(glyphStartPoint.Y)),
						CreateNewSize(Math.Ceiling(glyphDesiredSize.Width), Math.Ceiling(glyphDesiredSize.Height))));
				}
				if(ShowGlyphBackground)
					ArrangeElement(ElementGlyphBackground, new Rect(
						new Point(Math.Max(0, glyphStartPoint.X + GlyphBackgroundThickness.Left), Math.Max(0, glyphStartPoint.Y + GlyphBackgroundThickness.Top)),
						CreateNewSize(Math.Max(0, ElementTargetGlyph.DesiredSize.Width - GetLeftRight(GlyphBackgroundThickness)), Math.Max(0, ElementTargetGlyph.DesiredSize.Height - GetTopBottom(GlyphBackgroundThickness)))
						));
			}
			if (ActualShowArrow) { 
				ArrangeElement(ElementArrowControl, new Rect(arrowStartPoint, ElementArrowControl.DesiredSize));
			}
			ArrangeContents(contentStartPoint, contentAvailableSize);
			if(FirstBorderShowing && (!SecondBorderShowing || !ShowArrow)) ArrangeElement(ElementFirstBorderControl, new Rect(new Point(), retValue));
			Point firstStartPoint = new Point();
			Point secondStartPoint = new Point();
			Size firstSize = CreateNewSize();
			Size secondSize = CreateNewSize();
			if (ShowArrow) {
				if (ActualShowArrow && (SecondBorderPlacement == SecondBorderPlacement.Arrow || GlyphToContentAlignment != ContentAndGlyphToArrowAlignment)) {
					firstStartPoint.X = ContentAndGlyphToArrowAlignment == Dock.Right ? ElementArrowControl.DesiredSize.Width + GetLeftRight(actualArrowMargin) + CommonMargin.Left : 0d;
					secondStartPoint.X = ContentAndGlyphToArrowAlignment == Dock.Left ? retValue.Width - ElementArrowControl.DesiredSize.Width - GetLeftRight(actualArrowMargin) - CommonMargin.Left : 0d;
					firstStartPoint.Y = ContentAndGlyphToArrowAlignment == Dock.Bottom ? ElementArrowControl.DesiredSize.Width + GetTopBottom(actualArrowMargin) + CommonMargin.Top : 0d;
					secondStartPoint.Y = ContentAndGlyphToArrowAlignment == Dock.Top ? retValue.Height - ElementArrowControl.DesiredSize.Height - GetTopBottom(actualArrowMargin) - CommonMargin.Top : 0d;
					if (verticalArrow) {
						firstSize.Width = retValue.Width;
						secondSize.Width = retValue.Width;
						secondSize.Height = ElementArrowControl.DesiredSize.Height + GetTopBottom(actualArrowMargin) + CommonMargin.Bottom;
						firstSize.Height = Math.Max(0d, retValue.Height - secondSize.Height);
					}
					else {
						firstSize.Height = retValue.Height;
						secondSize.Height = retValue.Height;
						secondSize.Width = ElementArrowControl.DesiredSize.Width + GetLeftRight(actualArrowMargin) + CommonMargin.Right;
						firstSize.Width = Math.Max(0d, retValue.Width - secondSize.Width);
					}
				}
				else {
					if (ShowArrowInTextSplitter) {
						switch (GlyphToContentAlignment) {
							case Dock.Left:
								break;
							case Dock.Top:
								firstStartPoint = new Point(0, 0);
								double contentstartY = contentStartPoint.Y - actualCommonContentMargin.Top;
								firstSize = CreateNewSize(retValue.Width, contentstartY);
								secondStartPoint = new Point(0, contentstartY);
								secondSize = CreateNewSize(retValue.Width, retValue.Height - contentstartY);
								break;
							case Dock.Right:
								break;
							case Dock.Bottom:
								break;
							default:
								break;
						}
					}
				}
			}
			if (FirstBorderShowing && SecondBorderShowing && ShowArrow) {
				ArrangeElement(ElementFirstBorderControl, new Rect(firstStartPoint, firstSize));
				ArrangeElement(ElementSecondBorderControl, new Rect(secondStartPoint, secondSize));
			}
			if (ActualShowTouchSplitter) {
				var desiredWidth = ElementTouchSplitterTransformPanel.DesiredSize.Width;
				var desiredHeight = ElementTouchSplitterTransformPanel.DesiredSize.Height;
				var left = firstSize.Width - Math.Round(desiredWidth / 2);
				var top = firstSize.Height - Math.Round(desiredHeight / 2);
				var bounds = ContentAndGlyphToArrowAlignment == Dock.Left || ContentAndGlyphToArrowAlignment == Dock.Right ? new Rect(left, 0, desiredWidth, retValue.Height) : new Rect(0, top, retValue.Width, desiredHeight);
				ArrangeElement(ElementTouchSplitterTransformPanel, bounds);
			}				
			EndArrange();
			return retValue;
		}
		Thickness actualArrowMargin = new Thickness();
		Thickness actualGlyphMargin = new Thickness();
		Thickness actualCommonContentMargin = new Thickness();
		void CalculateActualMargins() {
			actualGlyphMargin = GlyphMargin;
			actualCommonContentMargin = CommonContentMargin;
			Thickness actualGlyphAndContentMargin = ContentAndGlyphMargin;
			if(ActualShowArrow) {
				actualArrowMargin = ArrowMargin;
				switch(ContentAndGlyphToArrowAlignment) {
					case Dock.Bottom:
						if(BottomContentAndGlyphMargin != null)
							actualGlyphAndContentMargin = (Thickness)BottomContentAndGlyphMargin;
						if(TopArrowMargin != null)
							actualArrowMargin = (Thickness)TopArrowMargin;
						break;
					case Dock.Left:
						if(LeftContentAndGlyphMargin != null)
							actualGlyphAndContentMargin = (Thickness)LeftContentAndGlyphMargin;
						if(RightArrowMargin != null)
							actualArrowMargin = (Thickness)RightArrowMargin;
						break;
					case Dock.Right:
						if(RightContentAndGlyphMargin != null)
							actualGlyphAndContentMargin = (Thickness)RightContentAndGlyphMargin;
						if(LeftArrowMargin != null)
							actualArrowMargin = (Thickness)LeftArrowMargin;
						break;
					case Dock.Top:
						if(TopContentAndGlyphMargin != null)
							actualGlyphAndContentMargin = (Thickness)TopContentAndGlyphMargin;
						if(BottomArrowMargin != null)
							actualArrowMargin = (Thickness)BottomArrowMargin;
						break;
				}
			}
			if(ShowGlyph && (ShowAnyContent)) {
				switch(GlyphToContentAlignment) {
					case Dock.Bottom:
						if(BottomGlyphMargin != null)
							actualGlyphMargin = (Thickness)BottomGlyphMargin;
						if(TopCommonContentMargin != null)
							actualCommonContentMargin = (Thickness)TopCommonContentMargin;
						actualGlyphMargin.Left += actualGlyphAndContentMargin.Left;
						actualGlyphMargin.Right += actualGlyphAndContentMargin.Right;
						actualGlyphMargin.Bottom += actualGlyphAndContentMargin.Bottom;
						actualCommonContentMargin.Left += actualGlyphAndContentMargin.Left;
						actualCommonContentMargin.Right += actualGlyphAndContentMargin.Right;
						actualCommonContentMargin.Top += actualGlyphAndContentMargin.Top;
						break;
					case Dock.Left:
						if(LeftGlyphMargin != null)
							actualGlyphMargin = (Thickness)LeftGlyphMargin;
						if(RightCommonContentMargin != null)
							actualCommonContentMargin = (Thickness)RightCommonContentMargin;
						actualGlyphMargin.Left += actualGlyphAndContentMargin.Left;
						actualGlyphMargin.Top += actualGlyphAndContentMargin.Top;
						actualGlyphMargin.Bottom += actualGlyphAndContentMargin.Bottom;
						actualCommonContentMargin.Right += actualGlyphAndContentMargin.Right;
						actualCommonContentMargin.Top += actualGlyphAndContentMargin.Top;
						actualCommonContentMargin.Bottom += actualGlyphAndContentMargin.Bottom;
						break;
					case Dock.Right:
						if(RightGlyphMargin != null)
							actualGlyphMargin = (Thickness)RightGlyphMargin;
						if(LeftCommonContentMargin != null)
							actualCommonContentMargin = (Thickness)LeftCommonContentMargin;
						actualGlyphMargin.Right += actualGlyphAndContentMargin.Right;
						actualGlyphMargin.Top += actualGlyphAndContentMargin.Top;
						actualGlyphMargin.Bottom += actualGlyphAndContentMargin.Bottom;
						actualCommonContentMargin.Left += actualGlyphAndContentMargin.Left;
						actualCommonContentMargin.Top += actualGlyphAndContentMargin.Top;
						actualCommonContentMargin.Bottom += actualGlyphAndContentMargin.Bottom;
						break;
					case Dock.Top:
						if(TopGlyphMargin != null)
							actualGlyphMargin = (Thickness)TopGlyphMargin;
						if(BottomCommonContentMargin != null)
							actualCommonContentMargin = (Thickness)BottomCommonContentMargin;
						actualGlyphMargin.Left += actualGlyphAndContentMargin.Left;
						actualGlyphMargin.Right += actualGlyphAndContentMargin.Right;
						actualGlyphMargin.Top += actualGlyphAndContentMargin.Top;
						actualCommonContentMargin.Left += actualGlyphAndContentMargin.Left;
						actualCommonContentMargin.Right += actualGlyphAndContentMargin.Right;
						actualCommonContentMargin.Bottom += actualGlyphAndContentMargin.Bottom;
						break;
				}
			} else {
				if(ShowGlyph) {
					actualGlyphMargin.Left += actualGlyphAndContentMargin.Left;
					actualGlyphMargin.Right += actualGlyphAndContentMargin.Right;
					actualGlyphMargin.Top += actualGlyphAndContentMargin.Top;
					actualGlyphMargin.Bottom += actualGlyphAndContentMargin.Bottom;
				} else {
					actualCommonContentMargin.Left += actualGlyphAndContentMargin.Left;
					actualCommonContentMargin.Right += actualGlyphAndContentMargin.Right;
					actualCommonContentMargin.Top += actualGlyphAndContentMargin.Top;
					actualCommonContentMargin.Bottom += actualGlyphAndContentMargin.Bottom;
				}
			}
		}
		protected virtual Size MeasureButton(Size availableSize) {
			CalculateActualMargins();
			double dHeight = 0d;
			double dWidth = 0d;
			FrameworkElement elementGlyph = ElementTargetGlyph;
			if(ShowGlyph) {
				if(ShowGlyphBackground)
					ElementGlyphBackground.Measure(SizeHelper.Infinite);
				elementGlyph.Measure(SizeHelper.Infinite);
				dHeight = elementGlyph.DesiredSize.Height + GetTopBottom(actualGlyphMargin);
				dWidth = elementGlyph.DesiredSize.Width + GetLeftRight(actualGlyphMargin);
			}
			ContentDesiredSize = MeasureContents(availableSize);
			if(GlyphToContentAlignment == Dock.Bottom || GlyphToContentAlignment == Dock.Top) {
				dWidth = Math.Max(dWidth, ContentDesiredSize.Width);
				dHeight += ContentDesiredSize.Height;
			} else {
				dWidth += ContentDesiredSize.Width;
				dHeight = Math.Max(dHeight, ContentDesiredSize.Height);
			}
			ContentAndGlyphDesiredSize = CreateNewSize(dWidth, dHeight);
			if(ActualShowArrow) {
				ElementArrowControl.Measure(SizeHelper.Infinite);				
				if(ContentAndGlyphToArrowAlignment == Dock.Top || ContentAndGlyphToArrowAlignment == Dock.Bottom) {
					dWidth = Math.Max(dWidth, ElementArrowControl.DesiredSize.Width + GetLeftRight(actualArrowMargin));
					dHeight += ElementArrowControl.DesiredSize.Height + GetTopBottom(actualArrowMargin);
				} else {
					dHeight = Math.Max(dHeight, ElementArrowControl.DesiredSize.Height + GetTopBottom(actualArrowMargin));
					dWidth += ElementArrowControl.DesiredSize.Width + GetLeftRight(actualArrowMargin);
				}
			}
			if (ActualShowTouchSplitter) {
				ElementTouchSplitterTransformPanel.Measure(SizeHelper.Infinite);
			}
			if(FirstBorderShowing)
				ElementFirstBorderControl.Measure(SizeHelper.Infinite);
			if(SecondBorderShowing)
				ElementSecondBorderControl.Measure(SizeHelper.Infinite);
			dWidth += GetLeftRight(CommonMargin);
			dHeight += GetTopBottom(CommonMargin);
			return CreateNewSize(dWidth, dHeight);
		}
		#endregion
		#region IsMouseOver
		bool isMouseOverFirstBorder = false;
		bool isMouseOverSecondBorder = false;
		public bool IsMouseOverFirstBorder { get { return isMouseOverFirstBorder; } }
		public bool IsMouseOverSecondBorder { get { return isMouseOverSecondBorder; } }
		public bool CalculateIsMouseOver { get; set; }		
		protected override 
			void OnMouseMove(System.Windows.Input.MouseEventArgs e) {
			base.OnMouseMove(e);
			if(!CalculateIsMouseOver) return;
			if(FirstBorderShowing) {
				Point p = e.GetPosition(ElementFirstBorderControl);
				if(p.X >= 0 && p.X <= ElementFirstBorderControl.ActualWidth && p.Y >= 0 && p.Y <= ElementFirstBorderControl.ActualHeight) {
					isMouseOverFirstBorder = true;
					isMouseOverSecondBorder = false;
					return;
				}
			}
			if(SecondBorderShowing) {
				Point p = e.GetPosition(ElementSecondBorderControl);
				if(p.X >= 0 && p.X <= ElementSecondBorderControl.ActualWidth && p.Y >= 0 && p.Y <= ElementSecondBorderControl.ActualHeight) {
					isMouseOverSecondBorder = true;
					isMouseOverFirstBorder = false;
					return;
				}
			}
		}
		#endregion
		protected double GetLeftRight(Thickness t) {
			return t.Left + t.Right;
		}
		protected double GetTopBottom(Thickness t) {
			return t.Top + t.Bottom;
		}
	}
	class ContentControlEx : ContentControl {
		public ContentControlEx() {
			Focusable = false;
			IsTabStop = false;
		}
		protected override Size MeasureOverride(Size constraint) {
			Size sz = MeasureArrange(constraint, false);
			sz.Width = Math.Ceiling(sz.Width);
			sz.Height = Math.Ceiling(sz.Height);
			return sz;
		}
		protected override Size ArrangeOverride(Size arrangeBounds) {			
			return MeasureArrange(arrangeBounds, true);
		}
		private Size MeasureArrange(Size arrangeBounds, bool arrange) {
			if(VisualTreeHelper.GetChildrenCount(this) == 0) return new Size();
			var vChild = VisualTreeHelper.GetChild(this, 0) as FrameworkElement;
			if(vChild == null) return new Size();
			double left = 0d;
			double top = 0d;
			if(arrange) {
				if(HorizontalContentAlignment == System.Windows.HorizontalAlignment.Right)
					left = arrangeBounds.Width - vChild.DesiredSize.Width;
				if(HorizontalContentAlignment == System.Windows.HorizontalAlignment.Center)
					left = (arrangeBounds.Width - vChild.DesiredSize.Width) / 2;
				if(VerticalContentAlignment == System.Windows.VerticalAlignment.Center)
					top = (arrangeBounds.Height - vChild.DesiredSize.Height) / 2;
				if(VerticalContentAlignment == System.Windows.VerticalAlignment.Bottom)
					top = (arrangeBounds.Height - vChild.DesiredSize.Height);
			}
			Size arrangeSize = new Size();
			Size vcds = arrange ? vChild.DesiredSize : SizeHelper.Infinite;
			arrangeSize.Height = VerticalContentAlignment == System.Windows.VerticalAlignment.Stretch ? arrangeBounds.Height : vcds.Height;
			arrangeSize.Width = HorizontalContentAlignment == System.Windows.HorizontalAlignment.Stretch ? arrangeBounds.Width : vcds.Width;
			if(arrange) {
				vChild.Arrange(new Rect(new Point(left, top), arrangeSize));
				return arrangeBounds;
			} else {
				vChild.Measure(arrangeSize);
				bool modified = false;
				if (vChild.DesiredSize.Height > arrangeBounds.Height) { arrangeSize.Height = arrangeBounds.Height; modified = true; }
				if (vChild.DesiredSize.Width > arrangeBounds.Width) { arrangeSize.Width = arrangeBounds.Width; modified = true; }
				if (modified) vChild.Measure(arrangeSize);
				return vChild.DesiredSize;
			}
		}
	}
	public static class ResourceStorage {
		public static bool UseResourceStorage { get; set; }
		static ResourceStorage() {
			UseResourceStorage = false;
			ThemeManager.ThemeChanged += OnThemeChanged;
		}
		static void OnThemeChanged(DependencyObject sender, ThemeChangedRoutedEventArgs e) {
			Resources.Clear();
			RaiseThemeChanged(null);
		}
		static DevExpress.Xpf.Bars.Native.WeakList<EventHandler> handlersThemeChanged = new Bars.Native.WeakList<EventHandler>();
		public static event EventHandler ThemeChanged {
			add { handlersThemeChanged.Add(value); }
			remove { handlersThemeChanged.Remove(value); }
		}
		static void RaiseThemeChanged(EventArgs args) {
			foreach(EventHandler e in handlersThemeChanged)
				e(null, args);
		}		
		[ThreadStatic]
		static Dictionary<ThemeKeyExtensionGeneric, object> resources;
		static Dictionary<ThemeKeyExtensionGeneric, object> Resources { get { return resources ?? (resources = new Dictionary<ThemeKeyExtensionGeneric, object>()); } }
		public static object TryGetResource(ThemeKeyExtensionGeneric key) {
			object value = null;
			if(Resources.TryGetValue(key, out value))
				return value;
			return null;
		}
		public static void AddResource(ThemeKeyExtensionGeneric key, object resource) {
			Resources.Add(key, resource);
		}
		public static object TryFindResourceInStorage(this FrameworkElement element, ThemeKeyExtensionGeneric key) {
			if(key == null) return null;
			object value = null;
			if(UseResourceStorage && Resources.TryGetValue(key, out value))
				return value;
			value = element.TryFindResource(key);
			if(UseResourceStorage && value != null) Resources.Add(key, value);
			return value;
		}
	}
	public class FontSettings {
		Brush normal;
		Brush hover;
		Brush pressed;
		Brush disabled;
		Brush _checked;
		Brush hoverChecked;
		public Brush Normal { get { return normal; } set { normal = FreezeValue(value); } }
		public Brush Hover { get { return hover; } set { hover = FreezeValue(value); } }
		public Brush Pressed { get { return pressed; } set { pressed = FreezeValue(value); } }
		public Brush Disabled { get { return disabled; } set { disabled = FreezeValue(value); } }
		public Brush HoverChecked { get { return hoverChecked; } set { hoverChecked = FreezeValue(value); } }
		public Brush Checked { get { return _checked; } set { _checked = FreezeValue(value); } }
		public FontFamily Family { get; set; }
		public FontWeight? Weight { get; set; }
		public double? Size { get; set; }
		Brush FreezeValue(Brush value) {
			if (value == null || !value.CanFreeze) return value;
			value.Freeze();
			return value;
		}
		public void Apply(Control element, BorderState actualState) {
			if(element == null) return;
			var action = new Action(() => {
				Normal = CorrectBrush(Normal);
				Hover = CorrectBrush(Hover);
				Pressed = CorrectBrush(Pressed);
				Disabled = CorrectBrush(Disabled);
				if(Family != null) element.FontFamily = Family;
				if(Weight != null) element.FontWeight = (FontWeight)Weight;
				if(Size != null) element.FontSize = (Double)Size;
				var checkedBrush = Checked ?? Pressed;
				var hoverCheckedBrush = HoverChecked ?? checkedBrush;
				switch (actualState) {
					case BorderState.Focused:
					case BorderState.Hover:
						if (Hover != null) element.Foreground = Hover;
						return;
					case BorderState.HoverChecked:
						if (hoverCheckedBrush != null) element.Foreground = hoverCheckedBrush;
						return;
					case BorderState.Checked:
						if (checkedBrush != null) element.Foreground = checkedBrush;
						return;
					case BorderState.Pressed:
						if (Pressed != null) element.Foreground = Pressed;
						return;
					case BorderState.Disabled:
						if (Disabled != null) element.Foreground = Disabled;
						return;
				}
				if (Normal != null)
					element.Foreground = Normal;
				else
					element.ClearValue(ContentControl.ForegroundProperty);
			});
			if(element.Dispatcher.CheckAccess()) {
				action();
			} else {
				element.Dispatcher.BeginInvoke(action);
			}
		}
		private Brush CorrectBrush(Brush brush) {
			if (brush == null) return null;
			if (brush == null || !brush.CanFreeze) return brush;
			brush = brush.Clone();
			brush.Freeze();
			return brush;
		}
		public static void Clear(Control element) {
			if(element == null) return;
			element.ClearValue(Control.FontFamilyProperty);
			element.ClearValue(Control.FontWeightProperty);
			element.ClearValue(Control.FontSizeProperty);
			if(element is ContentControl) element.ClearValue(ContentControl.ForegroundProperty);
		}
	}
	public class BarItemImageColorizerSettings : ImageColorizerSettings<BorderState> {
		public Color Normal {
			get { return States[BorderState.Normal]; }
			set { States[BorderState.Normal] = value; }
		}
		public Color Hover {
			get { return States[BorderState.Hover]; }
			set { States[BorderState.Hover] = value; }
		}
		public Color Pressed {
			get { return States[BorderState.Pressed]; }
			set { States[BorderState.Pressed] = value; }
		}
		public Color Checked {
			get { return States[BorderState.Checked]; }
			set { States[BorderState.Checked] = value; }
		}
		public Color Disabled {
			get { return States[BorderState.Disabled]; }
			set { States[BorderState.Disabled] = value; }
		}
		public override void Apply(DependencyObject target, BorderState state) {
			var color = Color.FromArgb(0, 0, 0, 0);
			if (States.ContainsKey(state)) {
				color = States[state];
			} else if (States.ContainsKey(BorderState.Normal)) {
				color = States[BorderState.Normal];
			}
			ImageColorizer.SetColor(target, color);
		}
	}
	public class SizeSettings {
		private double? minWidth;
		private double? minHeight;
		private double? maxWidth;
		private double? maxHeight;
		private double? width;
		private double? height;
		public double? Height {
			get { return height; }
			set {
				bool raiseChange = value != height;
				height = value;
				if(raiseChange)
					RaiseChanged();
			}
		}
		public double? Width {
			get { return width; }
			set {
				bool raiseChange = value != width;
				width = value;
				if(raiseChange)
					RaiseChanged();
			}
		}
		public double? MaxHeight {
			get { return maxHeight; }
			set {
				bool raiseChange = value != maxHeight;
				maxHeight = value;
				if(raiseChange)
					RaiseChanged();
			}
		}
		public double? MaxWidth {
			get { return maxWidth; }
			set {
				bool raiseChange = value != maxWidth;
				maxWidth = value;
				if(raiseChange)
					RaiseChanged();
			}
		}
		public double? MinHeight {
			get { return minHeight; }
			set {
				bool raiseChange = value != minHeight;
				minHeight = value;
				if(raiseChange)
					RaiseChanged();
			}
		}
		public double? MinWidth {
			get { return minWidth; }
			set {
				bool raiseChange = value != minWidth;
				minWidth = value;
				if(raiseChange)
					RaiseChanged();
			}
		}
		public event EventHandler Changed;
		protected void RaiseChanged() {
			if(Changed != null)
				Changed(this, new EventArgs());
		}
		public void Apply(FrameworkElement element) {
			if(element == null) return;
			if(minWidth != null) element.MinWidth = (double)minWidth;
			if(minHeight != null) element.MinHeight = (double)minHeight;
			if(maxWidth != null) element.MaxWidth = (double)maxWidth;
			if(maxHeight != null) element.MaxHeight = (double)maxHeight;
			if(width != null) element.Width = (double)width;
			if(height != null) element.Height = (double)height;
		}
		public static void Clear(FrameworkElement element) {
			if(element == null) return;
			element.ClearValue(FrameworkElement.MinWidthProperty);
			element.ClearValue(FrameworkElement.MinHeightProperty);
			element.ClearValue(FrameworkElement.MaxWidthProperty);
			element.ClearValue(FrameworkElement.MaxHeightProperty);
			element.ClearValue(FrameworkElement.WidthProperty);
			element.ClearValue(FrameworkElement.HeightProperty);
		}
	}
	public class FontSettingsExtension : System.Windows.Markup.MarkupExtension {
		public Brush Normal { get; set; }
		public Brush Hover { get; set; }
		public Brush Pressed { get; set; }
		public Brush Disabled { get; set; }
		public FontFamily Family { get; set; }
		public FontWeight? Weight { get; set; }
		public double? Size { get; set; }
		public FontSettingsExtension() {
			Normal = null;
			Hover = null;
			Pressed = null;
			Disabled = null;
			Family = null;
			Weight = null;
			Size = null;
		}
		public override object ProvideValue(IServiceProvider serviceProvider) {
			return new FontSettings() { Normal = this.Normal, Hover = this.Hover, Pressed = this.Pressed, Disabled = this.Disabled, Family = this.Family, Weight = this.Weight, Size = this.Size };
		}
	}
	public class BarItemLayoutPanelTouchProperties : DependencyObject {
		public static readonly DependencyProperty GlyphBackgroundThicknessProperty = Register("GlyphBackgroundThickness");
		public static void SetGlyphBackgroundThickness(DependencyObject d, TouchInfo value) {
			d.SetValue(GlyphBackgroundThicknessProperty, value);
		}
		public static TouchInfo GetGlyphBackgroundThickness(DependencyObject obj) {
			return (TouchInfo)obj.GetValue(GlyphBackgroundThicknessProperty);
		}
		public static readonly DependencyProperty CommonContentMarginProperty = Register("CommonContentMargin");
		public static void SetCommonContentMargin(DependencyObject d, TouchInfo value) {
			d.SetValue(CommonContentMarginProperty, value);
		}
		public static TouchInfo GetCommonContentMargin(DependencyObject obj) {
			return (TouchInfo)obj.GetValue(CommonContentMarginProperty);
		}
		public static readonly DependencyProperty RightContentAndGlyphMarginProperty = Register("RightContentAndGlyphMargin");
		public static void SetRightContentAndGlyphMargin(DependencyObject d, TouchInfo value) {
			d.SetValue(RightContentAndGlyphMarginProperty, value);
		}
		public static TouchInfo GetRightContentAndGlyphMargin(DependencyObject obj) {
			return (TouchInfo)obj.GetValue(RightContentAndGlyphMarginProperty);
		}
		public static readonly DependencyProperty LeftContentAndGlyphMarginProperty = Register("LeftContentAndGlyphMargin");
		public static void SetLeftContentAndGlyphMargin(DependencyObject d, TouchInfo value) {
			d.SetValue(LeftContentAndGlyphMarginProperty, value);
		}
		public static TouchInfo GetLeftContentAndGlyphMargin(DependencyObject obj) {
			return (TouchInfo)obj.GetValue(LeftContentAndGlyphMarginProperty);
		}
		public static readonly DependencyProperty BottomContentAndGlyphMarginProperty = Register("BottomContentAndGlyphMargin");
		public static void SetBottomContentAndGlyphMargin(DependencyObject d, TouchInfo value) {
			d.SetValue(BottomContentAndGlyphMarginProperty, value);
		}
		public static TouchInfo GetBottomContentAndGlyphMargin(DependencyObject obj) {
			return (TouchInfo)obj.GetValue(BottomContentAndGlyphMarginProperty);
		}
		public static readonly DependencyProperty TopContentAndGlyphMarginProperty = Register("TopContentAndGlyphMargin");
		public static void SetTopContentAndGlyphMargin(DependencyObject d, TouchInfo value) {
			d.SetValue(TopContentAndGlyphMarginProperty, value);
		}
		public static TouchInfo GetTopContentAndGlyphMargin(DependencyObject obj) {
			return (TouchInfo)obj.GetValue(TopContentAndGlyphMarginProperty);
		}
		public static readonly DependencyProperty ContentAndGlyphMarginProperty = Register("ContentAndGlyphMargin");
		public static void SetContentAndGlyphMargin(DependencyObject d, TouchInfo value) {
			d.SetValue(ContentAndGlyphMarginProperty, value);
		}
		public static TouchInfo GetContentAndGlyphMargin(DependencyObject obj) {
			return (TouchInfo)obj.GetValue(ContentAndGlyphMarginProperty);
		}
		public static readonly DependencyProperty RightArrowMarginProperty = Register("RightArrowMargin");
		public static void SetRightArrowMargin(DependencyObject d, TouchInfo value) {
			d.SetValue(RightArrowMarginProperty, value);
		}
		public static TouchInfo GetRightArrowMargin(DependencyObject obj) {
			return (TouchInfo)obj.GetValue(RightArrowMarginProperty);
		}
		public static readonly DependencyProperty LeftArrowMarginProperty = Register("LeftArrowMargin");
		public static void SetLeftArrowMargin(DependencyObject d, TouchInfo value) {
			d.SetValue(LeftArrowMarginProperty, value);
		}
		public static TouchInfo GetLeftArrowMargin(DependencyObject obj) {
			return (TouchInfo)obj.GetValue(LeftArrowMarginProperty);
		}
		public static readonly DependencyProperty BottomArrowMarginProperty = Register("BottomArrowMargin");
		public static void SetBottomArrowMargin(DependencyObject d, TouchInfo value) {
			d.SetValue(BottomArrowMarginProperty, value);
		}
		public static TouchInfo GetBottomArrowMargin(DependencyObject obj) {
			return (TouchInfo)obj.GetValue(BottomArrowMarginProperty);
		}
		public static readonly DependencyProperty TopArrowMarginProperty = Register("TopArrowMargin");
		public static void SetTopArrowMargin(DependencyObject d, TouchInfo value) {
			d.SetValue(TopArrowMarginProperty, value);
		}
		public static TouchInfo GetTopArrowMargin(DependencyObject obj) {
			return (TouchInfo)obj.GetValue(TopArrowMarginProperty);
		}
		public static readonly DependencyProperty RightCommonContentMarginProperty = Register("RightCommonContentMargin");
		public static void SetRightCommonContentMargin(DependencyObject d, TouchInfo value) {
			d.SetValue(RightCommonContentMarginProperty, value);
		}
		public static TouchInfo GetRightCommonContentMargin(DependencyObject obj) {
			return (TouchInfo)obj.GetValue(RightCommonContentMarginProperty);
		}
		public static readonly DependencyProperty LeftCommonContentMarginProperty = Register("LeftCommonContentMargin");
		public static void SetLeftCommonContentMargin(DependencyObject d, TouchInfo value) {
			d.SetValue(LeftCommonContentMarginProperty, value);
		}
		public static TouchInfo GetLeftCommonContentMargin(DependencyObject obj) {
			return (TouchInfo)obj.GetValue(LeftCommonContentMarginProperty);
		}
		public static readonly DependencyProperty BottomCommonContentMarginProperty = Register("BottomCommonContentMargin");
		public static void SetBottomCommonContentMargin(DependencyObject d, TouchInfo value) {
			d.SetValue(BottomCommonContentMarginProperty, value);
		}
		public static TouchInfo GetBottomCommonContentMargin(DependencyObject obj) {
			return (TouchInfo)obj.GetValue(BottomCommonContentMarginProperty);
		}
		public static readonly DependencyProperty TopCommonContentMarginProperty = Register("TopCommonContentMargin");
		public static void SetTopCommonContentMargin(DependencyObject d, TouchInfo value) {
			d.SetValue(TopCommonContentMarginProperty, value);
		}
		public static TouchInfo GetTopCommonContentMargin(DependencyObject obj) {
			return (TouchInfo)obj.GetValue(TopCommonContentMarginProperty);
		}
		public static readonly DependencyProperty RightGlyphMarginProperty = Register("RightGlyphMargin");
		public static void SetRightGlyphMargin(DependencyObject d, TouchInfo value) {
			d.SetValue(RightGlyphMarginProperty, value);
		}
		public static TouchInfo GetRightGlyphMargin(DependencyObject obj) {
			return (TouchInfo)obj.GetValue(RightGlyphMarginProperty);
		}
		public static readonly DependencyProperty LeftGlyphMarginProperty = Register("LeftGlyphMargin");
		public static void SetLeftGlyphMargin(DependencyObject d, TouchInfo value) {
			d.SetValue(LeftGlyphMarginProperty, value);
		}
		public static TouchInfo GetLeftGlyphMargin(DependencyObject obj) {
			return (TouchInfo)obj.GetValue(LeftGlyphMarginProperty);
		}
		public static readonly DependencyProperty BottomGlyphMarginProperty = Register("BottomGlyphMargin");
		public static void SetBottomGlyphMargin(DependencyObject d, TouchInfo value) {
			d.SetValue(BottomGlyphMarginProperty, value);
		}
		public static TouchInfo GetBottomGlyphMargin(DependencyObject obj) {
			return (TouchInfo)obj.GetValue(BottomGlyphMarginProperty);
		}
		public static readonly DependencyProperty TopGlyphMarginProperty = Register("TopGlyphMargin");
		public static void SetTopGlyphMargin(DependencyObject d, TouchInfo value) {
			d.SetValue(TopGlyphMarginProperty, value);
		}
		public static TouchInfo GetTopGlyphMargin(DependencyObject obj) {
			return (TouchInfo)obj.GetValue(TopGlyphMarginProperty);
		}
		public static readonly DependencyProperty CommonMarginProperty = Register("CommonMargin");
		public static void SetCommonMargin(DependencyObject d, TouchInfo value) {
			d.SetValue(CommonMarginProperty, value);
		}
		public static TouchInfo GetCommonMargin(DependencyObject obj) {
			return (TouchInfo)obj.GetValue(CommonMarginProperty);
		}
		public static readonly DependencyProperty AdditionalContentMarginProperty = Register("AdditionalContentMargin");
		public static void SetAdditionalContentMargin(DependencyObject d, TouchInfo value) {
			d.SetValue(AdditionalContentMarginProperty, value);
		}
		public static TouchInfo GetAdditionalContentMargin(DependencyObject obj) {
			return (TouchInfo)obj.GetValue(AdditionalContentMarginProperty);
		}
		public static readonly DependencyProperty ArrowMarginProperty = Register("ArrowMargin");
		public static void SetArrowMargin(DependencyObject d, TouchInfo value) {
			d.SetValue(ArrowMarginProperty, value);
		}
		public static TouchInfo GetArrowMargin(DependencyObject obj) {
			return (TouchInfo)obj.GetValue(ArrowMarginProperty);
		}
		public static readonly DependencyProperty KeyGestureMarginProperty = Register("KeyGestureMargin");
		public static void SetKeyGestureMargin(DependencyObject d, TouchInfo value) {
			d.SetValue(KeyGestureMarginProperty, value);
		}
		public static TouchInfo GetKeyGestureMargin(DependencyObject obj) {
			return (TouchInfo)obj.GetValue(KeyGestureMarginProperty);
		}
		public static readonly DependencyProperty DescriptionMarginProperty = Register("DescriptionMargin");
		public static void SetDescriptionMargin(DependencyObject d, TouchInfo value) {
			d.SetValue(DescriptionMarginProperty, value);
		}
		public static TouchInfo GetDescriptionMargin(DependencyObject obj) {
			return (TouchInfo)obj.GetValue(DescriptionMarginProperty);
		}
		public static readonly DependencyProperty Content2MarginProperty = Register("Content2Margin");
		public static void SetContent2Margin(DependencyObject d, TouchInfo value) {
			d.SetValue(Content2MarginProperty, value);
		}
		public static TouchInfo GetContent2Margin(DependencyObject obj) {
			return (TouchInfo)obj.GetValue(Content2MarginProperty);
		}
		public static readonly DependencyProperty ContentMarginProperty = Register("ContentMargin");
		public static void SetContentMargin(DependencyObject d, TouchInfo value) {
			d.SetValue(ContentMarginProperty, value);
		}
		public static TouchInfo GetContentMargin(DependencyObject obj) {
			return (TouchInfo)obj.GetValue(ContentMarginProperty);
		}
		public static readonly DependencyProperty GlyphMarginProperty = Register("GlyphMargin");
		public static void SetGlyphMargin(DependencyObject d, TouchInfo value) {
			d.SetValue(GlyphMarginProperty, value);
		}
		public static TouchInfo GetGlyphMargin(DependencyObject obj) {
			return (TouchInfo)obj.GetValue(GlyphMarginProperty);
		}
		static DependencyProperty Register(string name) {
			return DependencyPropertyManager.RegisterAttached(name, typeof(TouchInfo), typeof(BarItemLayoutPanelTouchProperties), new PropertyMetadata(new PropertyChangedCallback(TouchInfo.OnTouchPropertyChanged)));
		}
	}
}
