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
using System.Windows.Input;
using DevExpress.Xpf.Bars.Themes;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Utils;
using System.Windows.Media;
namespace DevExpress.Xpf.Bars {
	public class BarItemSeparator : BarItem {
		protected internal override bool CanKeyboardSelect { get { return false; } }	   
	}	
	public class BarItemLinkSeparator : BarItemLink {
		public BarItemLinkSeparator() {
		}
		protected internal override void Initialize() {
			UpdateSeparatorsVisibility();			
		}		
		protected override bool GetActualIsVisible() {
			if (Item != null) {
				IsVisible = Item.IsVisible;
			}
			return base.GetActualIsVisible();
		}
		protected internal override void UpdateProperties() {			
			if (Item == null) {
				UpdateSeparatorsVisibility();
				return;
			}
			base.UpdateProperties();
			CoerceValue(UIElement.IsEnabledProperty);
			if(!IsPrivate)
				IsPrivate = Item.IsPrivate;
			ExecuteActionOnLinkControls((lc) => lc.UpdateActualGlyph());
		}
	}
	public class BarItemLinkSeparatorControl : BarItemLinkControl {
		#region static
		public static readonly DependencyProperty InMenuContentOffsetProperty;
		public Orientation InMenuOrientation {
			get { return (Orientation)GetValue(InMenuOrientationProperty); }
			protected internal set { SetValue(InMenuOrientationPropertyKey, value); }
		}
		protected static readonly DependencyPropertyKey InMenuOrientationPropertyKey = DependencyPropertyManager.RegisterReadOnly("InMenuOrientation", typeof(Orientation), typeof(BarItemLinkSeparatorControl), new FrameworkPropertyMetadata(Orientation.Vertical, FrameworkPropertyMetadataOptions.AffectsMeasure, new PropertyChangedCallback(OnInMenuOrientationPropertyChanged)));
		public static readonly DependencyProperty InMenuOrientationProperty = InMenuOrientationPropertyKey.DependencyProperty;
		protected static void OnInMenuOrientationPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((BarItemLinkSeparatorControl)d).OnInMenuOrientationChanged((Orientation)e.OldValue);
		}
		protected virtual void OnInMenuOrientationChanged(Orientation oldValue) {
		}
		static BarItemLinkSeparatorControl() {
			Automation.NavigationAutomationPeersCreator.Default.RegisterObject(typeof(BarItemLinkSeparatorControl), typeof(System.Windows.Automation.Peers.FrameworkElementAutomationPeer), owner => null);
			DefaultStyleKeyProperty.OverrideMetadata(typeof(BarItemLinkSeparatorControl), new FrameworkPropertyMetadata(typeof(BarItemLinkSeparatorControl)));
			InMenuContentOffsetProperty = DependencyProperty.Register("InMenuContentOffset", typeof(double), typeof(BarItemLinkSeparatorControl), new PropertyMetadata(0.0));
		}
		#endregion
		public BarItemLinkSeparatorControl(BarItemLinkSeparator separator)
			: base(separator) {
		}
		protected override bool CanStartDragCore(object sender, MouseButtonEventArgs e) {
			return false;
		}
		protected override void OnLoaded(object sender, RoutedEventArgs e) {
			base.OnLoaded(sender, e);
			UpdateInMenuContentOffset();
		}
		protected internal override void OnMaxGlyphSizeChanged(Size MaxGlyphSize) {
			base.OnMaxGlyphSizeChanged(MaxGlyphSize);
			UpdateInMenuContentOffset();
		}
		protected internal override void UpdateOrientation() { }
		public BarItemLinkSeparator LinkSeparator { get { return Link as BarItemLinkSeparator; } }
		public double InMenuContentOffset {
			get { return (double)GetValue(InMenuContentOffsetProperty); }
			set { SetValue(InMenuContentOffsetProperty, value); }
		}
		protected object GetTemplateFromProvider(DependencyProperty prop, BarItemSeparatorThemeKeys themeKeys) {
			object res = GetCustomResources() == null ? null : GetCustomResources()[ResourceHelper.CorrectResourceKey(new BarItemSeparatorThemeKeyExtension() { ResourceKey = themeKeys })];
			if (res == null)
				res = GetValue(prop);
			return res;
		}
		protected internal override void UpdateStyleByContainerType(LinkContainerType type) {
			base.UpdateStyleByContainerType(type);
			string themeName = BarManagerHelper.GetThemeName(this);			
			switch (type) { 
				case LinkContainerType.Menu:
				case LinkContainerType.ApplicationMenu:
				case LinkContainerType.DropDownGallery:
					if (InMenuOrientation == Orientation.Vertical)
						Template = (ControlTemplate)GetTemplateFromProvider(BarItemLinkControlTemplateProvider.TemplateInMenuProperty, BarItemSeparatorThemeKeys.InMenuTemplate);
					else
						Template = (ControlTemplate)GetTemplateFromProvider(BarItemLinkControlTemplateProvider.TemplateProperty, BarItemSeparatorThemeKeys.Template);
					break;
				case LinkContainerType.RibbonStatusBarLeft:
					Template = (ControlTemplate)GetTemplateFromProvider(BarItemLinkControlTemplateProvider.TemplateInRibbonStatusBarLeftPartProperty, BarItemSeparatorThemeKeys.InRibbonStatusBarLeftPartTemplate);
					break;
				case LinkContainerType.RibbonStatusBarRight:
					Template = (ControlTemplate)GetTemplateFromProvider(BarItemLinkControlTemplateProvider.TemplateInRibbonStatusBarRightPartProperty, BarItemSeparatorThemeKeys.InRibbonStatusBarRightPartTemplate);
					break;
				case LinkContainerType.StatusBar:
					Template = (ControlTemplate)GetTemplateFromProvider(BarItemLinkControlTemplateProvider.TemplateInStatusBarProperty, BarItemSeparatorThemeKeys.InStatusBarTemplate);
					break;
				case LinkContainerType.RibbonQuickAccessToolbar:
					Template = (ControlTemplate)GetTemplateFromProvider(BarItemLinkControlTemplateProvider.TemplateInRibbonQuickAccessToolbarProperty, BarItemSeparatorThemeKeys.InRibbonQuickAccessToolbarTemplate);
					break;
				case LinkContainerType.RibbonQuickAccessToolbarFooter:
					Template = (ControlTemplate)GetTemplateFromProvider(BarItemLinkControlTemplateProvider.TemplateInRibbonQuickAccessToolbarFooterProperty, BarItemSeparatorThemeKeys.InRibbonQuickAccessToolbarFooterTemplate);
					break;
				case LinkContainerType.RibbonPageGroup:
					Template = (ControlTemplate)GetTemplateFromProvider(BarItemLinkControlTemplateProvider.TemplateInRibbonPageGroupProperty, BarItemSeparatorThemeKeys.InRibbonPageGroupTemplate);
					break;
				case LinkContainerType.RibbonPageHeader:
					Template = (ControlTemplate)GetTemplateFromProvider(BarItemLinkControlTemplateProvider.TemplateInRibbonPageHeaderProperty, BarItemSeparatorThemeKeys.InRibbonPageHeaderTemplate);
					break;
				default:
					Template = (ControlTemplate)GetTemplateFromProvider(BarItemLinkControlTemplateProvider.TemplateProperty, BarItemSeparatorThemeKeys.Template);
					break;
			}
		}
		protected virtual void UpdateInMenuContentOffset() { 
			if (LinksControl == null || (ContainerType != LinkContainerType.ApplicationMenu && ContainerType != LinkContainerType.Menu && ContainerType != LinkContainerType.DropDownGallery))
				return;
			InMenuContentOffset = LinksControl.MaxGlyphSize.Width + LinksControl.GlyphPadding.Left + LinksControl.GlyphPadding.Right;
		}
		Orientation layoutOrientation = Orientation.Horizontal;
		protected internal Orientation LayoutOrientation {
			get { return layoutOrientation; }
			set {
				if (value == layoutOrientation) return;
				layoutOrientation = value;
			}
		}		
		protected override Size MeasureOverride(Size availableSize) {
			return GetCorrectSize(base.MeasureOverride(availableSize));
		}
		protected override Size ArrangeOverride(Size arrangeBounds) {
			Transform transform = Transform.Identity;
			if (LayoutOrientation == Orientation.Vertical) {
				TransformGroup group = new TransformGroup();
				RotateTransform rt = new RotateTransform() { Angle = 90 };
				TranslateTransform tt = new TranslateTransform() { X = arrangeBounds.Width, Y = 0d };
				group.Children.Add(rt);
				group.Children.Add(tt);
				transform = group;
			}
			RenderTransform = transform;
			return GetCorrectSize(base.ArrangeOverride(GetCorrectSize(arrangeBounds)));
		}		
		private Size GetCorrectSize(Size size) {
			return LayoutOrientation == Orientation.Horizontal ? size : new Size(size.Height, size.Width);
		}
		protected internal override bool GetIsSelectable() {
			return false;
		}
	}	
}
