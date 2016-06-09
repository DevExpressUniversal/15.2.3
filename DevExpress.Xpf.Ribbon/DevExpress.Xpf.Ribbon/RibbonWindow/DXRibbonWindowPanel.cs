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

using System.Windows;
using DevExpress.Xpf.Ribbon;
using DevExpress.Xpf.Bars;
using System.Windows.Controls;
using DevExpress.Xpf.Core.Native;
using System.Windows.Media;
using System.Windows.Input;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Utils.Themes;
using System.Collections.Generic;
using System;
using DevExpress.Xpf.Utils;
namespace DevExpress.Xpf.Ribbon {
	public class DXRibbonWindowPanel : Panel {
		#region static
		public static readonly DependencyProperty ControlBoxProperty;
		public static readonly DependencyProperty BackgroundBorderProperty;
		public static readonly DependencyProperty HeaderBackgroundBorderProperty;
		public static readonly DependencyProperty TitleContainerProperty;
		public static readonly DependencyProperty IconContainerProperty;
		public static readonly DependencyProperty ContentProperty;
		public static readonly DependencyProperty ContentBackgroundBorderProperty;
		public static readonly DependencyProperty ContentBackgroundBorderMarginProperty;
		public static readonly DependencyProperty StatusBarContainerProperty;
		public static readonly DependencyProperty BackgroundBorderPaddingProperty;
		public static readonly DependencyProperty IsAeroModeProperty;
		public static readonly DependencyProperty ContentBackgroundBorderInAeroModeProperty;
		public static readonly DependencyProperty ContentBackgroundBorderMarginInAeroModeProperty;
		public static readonly DependencyProperty IsRibbonModeProperty;
		public static readonly DependencyProperty ContentBackgroundBorderPaddingProperty;
		static DXRibbonWindowPanel() {
			ControlBoxProperty = DependencyPropertyManager.Register("ControlBox", typeof(UIElement), typeof(DXRibbonWindowPanel), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnControlBoxPropertyChanged)));
			ContentProperty = DependencyPropertyManager.Register("Content", typeof(UIElement), typeof(DXRibbonWindowPanel), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnContentPropertyChanged)));
			IconContainerProperty = DependencyPropertyManager.Register("IconContainer", typeof(UIElement), typeof(DXRibbonWindowPanel), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnIconContainerPropertyChanged)));
			TitleContainerProperty = DependencyPropertyManager.Register("TitleContainer", typeof(UIElement), typeof(DXRibbonWindowPanel), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnTitleContainerPropertyChanged)));
			HeaderBackgroundBorderProperty = DependencyPropertyManager.Register("HeaderBackgroundBorder", typeof(UIElement), typeof(DXRibbonWindowPanel), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnHeaderBackgroundBorderPropertyChanged)));
			BackgroundBorderProperty = DependencyPropertyManager.Register("BackgroundBorder", typeof(UIElement), typeof(DXRibbonWindowPanel), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnBackgroundBorderPropertyChanged)));		
			ContentBackgroundBorderProperty = DependencyPropertyManager.Register("ContentBackgroundBorder", typeof(UIElement), typeof(DXRibbonWindowPanel), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnContentBackgroundBorderPropertyChanged)));
			ContentBackgroundBorderMarginProperty = DependencyPropertyManager.Register("ContentBackgroundBorderMargin", typeof(Thickness), typeof(DXRibbonWindowPanel), new FrameworkPropertyMetadata(new Thickness(), FrameworkPropertyMetadataOptions.AffectsMeasure, new PropertyChangedCallback(OnContentBackgroundBorderMarginPropertyChanged)));
			StatusBarContainerProperty = DependencyPropertyManager.Register("StatusBarContainer", typeof(UIElement), typeof(DXRibbonWindowPanel), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnStatusBarContainerPropertyChanged)));
			BackgroundBorderPaddingProperty = DependencyPropertyManager.Register("BackgroundBorderPadding", typeof(Thickness), typeof(DXRibbonWindowPanel), new FrameworkPropertyMetadata(new Thickness(), FrameworkPropertyMetadataOptions.AffectsMeasure, new PropertyChangedCallback(OnBackgroundBorderPaddingPropertyChanged)));
			IsAeroModeProperty = DependencyPropertyManager.Register("IsAeroMode", typeof(bool), typeof(DXRibbonWindowPanel), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsMeasure,  new PropertyChangedCallback(OnIsAeroModePropertyChanged)));
			ContentBackgroundBorderInAeroModeProperty = DependencyPropertyManager.Register("ContentBackgroundBorderInAeroMode", typeof(UIElement), typeof(DXRibbonWindowPanel), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnContentBackgroundBorderInAeroModePropertyChanged)));
			ContentBackgroundBorderMarginInAeroModeProperty = DependencyPropertyManager.Register("ContentBackgroundBorderMarginInAeroMode", typeof(Thickness), typeof(DXRibbonWindowPanel), new FrameworkPropertyMetadata(new Thickness(), new PropertyChangedCallback(OnContentBackgroundBorderMarginInAeroModePropertyChanged)));
			IsRibbonModeProperty = DependencyPropertyManager.Register("IsRibbonMode", typeof(bool), typeof(DXRibbonWindowPanel), new FrameworkPropertyMetadata(false, new PropertyChangedCallback(OnIsRibbonModePropertyChanged)));
			ContentBackgroundBorderPaddingProperty = DependencyPropertyManager.Register("ContentBackgroundBorderPadding", typeof(Thickness), typeof(DXRibbonWindowPanel), new FrameworkPropertyMetadata(new Thickness(), new PropertyChangedCallback(OnContentBackgroundBorderPaddingPropertyChanged)));
		}
		protected static void OnHeaderBackgroundBorderPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((DXRibbonWindowPanel)d).OnHeaderBackgroundBorderChanged((UIElement)e.OldValue);
		}		
		protected static void OnControlBoxPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((DXRibbonWindowPanel)d).OnControlBoxChanged((UIElement)e.OldValue);
		}
		protected static void OnContentPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((DXRibbonWindowPanel)d).OnContentChanged((UIElement)e.OldValue);
		}
		protected static void OnIconContainerPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((DXRibbonWindowPanel)d).OnIconContainerChanged((UIElement)e.OldValue);
		}
		protected static void OnTitleContainerPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((DXRibbonWindowPanel)d).OnTitleContainerChanged((UIElement)e.OldValue);
		}
		protected static void OnBackgroundBorderPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((DXRibbonWindowPanel)d).OnBackgroundBorderChanged((UIElement)e.OldValue);
		}
		protected static void OnContentBackgroundBorderPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((DXRibbonWindowPanel)d).OnContentBackgroundBorderChanged((UIElement)e.OldValue);
		}
		protected static void OnContentBackgroundBorderMarginPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((DXRibbonWindowPanel)d).OnContentBackgroundBorderMarginChanged((Thickness)e.OldValue);
		}
		protected static void OnStatusBarContainerPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((DXRibbonWindowPanel)d).OnStatusBarContainerChanged((UIElement)e.OldValue);
		}
		protected static void OnBackgroundBorderPaddingPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((DXRibbonWindowPanel)d).OnBackgroundBorderPaddingChanged((Thickness)e.OldValue);
		}
		protected static void OnIsAeroModePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((DXRibbonWindowPanel)d).OnIsAeroModeChanged((bool)e.OldValue);
		}
		protected static void OnContentBackgroundBorderInAeroModePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((DXRibbonWindowPanel)d).OnContentBackgroundBorderInAeroModeChanged((UIElement)e.OldValue);
		}
		protected static void OnContentBackgroundBorderMarginInAeroModePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((DXRibbonWindowPanel)d).OnContentBackgroundBorderMarginInAeroModeChanged((Thickness)e.OldValue);
		}
		protected static void OnIsRibbonModePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((DXRibbonWindowPanel)d).OnIsRibbonModeChanged((bool)e.OldValue);
		}
		protected static void OnContentBackgroundBorderPaddingPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((DXRibbonWindowPanel)d).OnContentBackgroundBorderPaddingChanged((Thickness)e.OldValue);
		}
		#endregion
		#region dep props
		public UIElement ControlBox {
			get { return (UIElement)GetValue(ControlBoxProperty); }
			set { SetValue(ControlBoxProperty, value); }
		}
		public UIElement Content {
			get { return (UIElement)GetValue(ContentProperty); }
			set { SetValue(ContentProperty, value); }
		}
		public UIElement IconContainer {
			get { return (UIElement)GetValue(IconContainerProperty); }
			set { SetValue(IconContainerProperty, value); }
		}
		public UIElement TitleContainer {
			get { return (UIElement)GetValue(TitleContainerProperty); }
			set { SetValue(TitleContainerProperty, value); }
		}
		public UIElement HeaderBackgroundBorder {
			get { return (UIElement)GetValue(HeaderBackgroundBorderProperty); }
			set { SetValue(HeaderBackgroundBorderProperty, value); }
		}
		public UIElement BackgroundBorder {
			get { return (UIElement)GetValue(BackgroundBorderProperty); }
			set { SetValue(BackgroundBorderProperty, value); }
		}
		public UIElement ContentBackgroundBorder {
			get { return (UIElement)GetValue(ContentBackgroundBorderProperty); }
			set { SetValue(ContentBackgroundBorderProperty, value); }
		}
		public Thickness ContentBackgroundBorderMargin {
			get { return (Thickness)GetValue(ContentBackgroundBorderMarginProperty); }
			set { SetValue(ContentBackgroundBorderMarginProperty, value); }
		}
		public UIElement StatusBarContainer {
			get { return (UIElement)GetValue(StatusBarContainerProperty); }
			set { SetValue(StatusBarContainerProperty, value); }
		}
		public Thickness BackgroundBorderPadding {
			get { return (Thickness)GetValue(BackgroundBorderPaddingProperty); }
			set { SetValue(BackgroundBorderPaddingProperty, value); }
		}
		public bool IsAeroMode {
			get { return (bool)GetValue(IsAeroModeProperty); }
			set { SetValue(IsAeroModeProperty, value); }
		}
		public UIElement ContentBackgroundBorderInAeroMode {
			get { return (UIElement)GetValue(ContentBackgroundBorderInAeroModeProperty); }
			set { SetValue(ContentBackgroundBorderInAeroModeProperty, value); }
		}
		public Thickness ContentBackgroundBorderMarginInAeroMode {
			get { return (Thickness)GetValue(ContentBackgroundBorderMarginInAeroModeProperty); }
			set { SetValue(ContentBackgroundBorderMarginInAeroModeProperty, value); }
		}
		public bool IsRibbonMode {
			get { return (bool)GetValue(IsRibbonModeProperty); }
			set { SetValue(IsRibbonModeProperty, value); }
		}
		public Thickness ContentBackgroundBorderPadding {
			get { return (Thickness)GetValue(ContentBackgroundBorderPaddingProperty); }
			set { SetValue(ContentBackgroundBorderPaddingProperty, value); }
		}
		#endregion
		public DXRibbonWindowPanel() {
		}
		private DXRibbonWindow ribbonWindowCore = null;
		public DXRibbonWindow RibbonWindow {
			get {
				if(ribbonWindowCore == null)
					ribbonWindowCore = LayoutHelper.FindParentObject<DXRibbonWindow>(this);
				return ribbonWindowCore;
			}
		}
		protected virtual void OnControlBoxChanged(UIElement oldValue) {
			OnChildChanged(oldValue, ControlBox);
		}
		protected virtual void OnContentChanged(UIElement oldValue) {
			OnChildChanged(oldValue, Content);
		}
		protected virtual void OnIconContainerChanged(UIElement oldValue) {
			OnChildChanged(oldValue, IconContainer);
		}
		protected virtual void OnTitleContainerChanged(UIElement oldValue) {
			OnChildChanged(oldValue, TitleContainer);
		}
		protected virtual void OnHeaderBackgroundBorderChanged(UIElement oldValue) {
			OnChildChanged(oldValue, HeaderBackgroundBorder);
		}
		protected virtual void OnBackgroundBorderChanged(UIElement oldValue) {
			OnChildChanged(oldValue, BackgroundBorder);
		}
		protected virtual void OnContentBackgroundBorderChanged(UIElement oldValue) {
			OnChildChanged(oldValue, ContentBackgroundBorder);
		}
		protected virtual void OnContentBackgroundBorderMarginChanged(Thickness oldValue) {
		}
		protected virtual void OnStatusBarContainerChanged(UIElement oldValue) {
			OnChildChanged(oldValue, StatusBarContainer);
		}
		protected virtual void OnContentBackgroundBorderInAeroModeChanged(UIElement oldValue) {
			OnChildChanged(oldValue, ContentBackgroundBorderInAeroMode);
		}
		protected virtual void OnContentBackgroundBorderMarginInAeroModeChanged(Thickness oldValue) {
		}
		protected virtual void OnContentBackgroundBorderPaddingChanged(Thickness oldValue) {
		}
		protected virtual void OnBackgroundBorderPaddingChanged(Thickness oldValue) {
		}
		protected virtual void OnIsAeroModeChanged(bool oldValue) {
		}
		protected virtual void OnIsRibbonModeChanged(bool oldValue) {
			SetVisibility(TitleContainer, !IsRibbonMode);
			SetVisibility(IconContainer, !IsRibbonMode);			
		}
		private void SetVisibility(UIElement element, bool isVisible) {
			if(element == null) return;
			element.Visibility = isVisible ? Visibility.Visible : Visibility.Collapsed;
		}
		void OnChildChanged(UIElement oldElement, UIElement newElement) {
			if(oldElement != null)
				Children.Remove(oldElement);
			if(newElement != null)
				Children.Add(newElement);
		}
		protected override Size MeasureOverride(Size availableSize) {
			if(BackgroundBorder != null) BackgroundBorder.Measure(availableSize);
			availableSize = SubstractHorizontalPadding(availableSize, BackgroundBorderPadding);
			availableSize = SubstractVerticalPadding(availableSize, BackgroundBorderPadding);
			MeasureHeader(availableSize);
			if(!double.IsPositiveInfinity(availableSize.Height)) {
				availableSize.Height = Math.Max(0, availableSize.Height - GetHeaderDesiredSize().Height);
			}			
			availableSize = SubstractHorizontalPadding(availableSize, ContentBackgroundBorderMargin);
			availableSize = SubstractVerticalPadding(availableSize, ContentBackgroundBorderMargin);
			MeasureElement(ContentBackgroundBorder, availableSize);
			availableSize = SubstractHorizontalPadding(availableSize, ContentBackgroundBorderPadding);
			availableSize = SubstractVerticalPadding(availableSize, ContentBackgroundBorderPadding);
			MeasureElement(StatusBarContainer, availableSize);
			if(!double.IsPositiveInfinity(availableSize.Height)) {
				availableSize.Height = Math.Max(0, availableSize.Height - GetDesiredSize(StatusBarContainer).Height);
			}
			MeasureElement(Content, availableSize);
			Size desiredSize = CalcDesiredSize();
			if(!double.IsPositiveInfinity(availableSize.Width))
				desiredSize.Width = Math.Min(desiredSize.Width, availableSize.Width);
			if(!double.IsPositiveInfinity(availableSize.Height))
				desiredSize.Height = Math.Min(desiredSize.Height, availableSize.Height);
			return desiredSize;
		}
		protected override Size ArrangeOverride(Size finalSize) {
			Rect finalRect = new Rect(0, 0, finalSize.Width, finalSize.Height);
			BackgroundBorder.Arrange(finalRect);
			finalRect = SubstractPadding(finalRect, BackgroundBorderPadding);
			Size size = GetHeaderDesiredSize();
			ArrangeHeader(new Rect(finalRect.X, finalRect.Y, finalRect.Width, size.Height));
			finalRect = SubstractPadding(finalRect, new Thickness(0, size.Height, 0, 0));
			finalRect = SubstractPadding(finalRect, ContentBackgroundBorderMargin);
			ArrangeElement(ContentBackgroundBorder, finalRect);
			finalRect = SubstractPadding(finalRect, ContentBackgroundBorderPadding);
			size = GetDesiredSize(StatusBarContainer);			
			ArrangeElement(StatusBarContainer, new Rect(finalRect.X, finalRect.Bottom - size.Height, finalRect.Width, size.Height));
			finalRect = SubstractPadding(finalRect, new Thickness(0, 0, 0, size.Height));
			ArrangeElement(Content, finalRect);			
			return finalSize;
		}
		Size CalcDesiredSize() {
			Size desiredSize = GetHeaderDesiredSize();
			desiredSize = GetMaxWidthAndSumOfHeight(desiredSize, GetDesiredSize(StatusBarContainer));
			desiredSize = GetMaxWidthAndSumOfHeight(desiredSize, GetDesiredSize(Content));
			desiredSize = AddVerticalPadding(desiredSize, BackgroundBorderPadding);
			desiredSize = AddVerticalPadding(desiredSize, ContentBackgroundBorderMargin);
			desiredSize = AddVerticalPadding(desiredSize, ContentBackgroundBorderPadding);
			desiredSize = AddHorizontalPadding(desiredSize, BackgroundBorderPadding);
			desiredSize = AddHorizontalPadding(desiredSize, ContentBackgroundBorderMargin);
			desiredSize = AddHorizontalPadding(desiredSize, ContentBackgroundBorderPadding);
			return desiredSize;
		}
		protected virtual void MeasureHeader(Size availableSize) {
			MeasureElement(HeaderBackgroundBorder, availableSize);
			MeasureElement(ControlBox, availableSize);
			if(ControlBox != null) {				
				if(!double.IsPositiveInfinity(availableSize.Width))
					availableSize.Width = Math.Max(0, availableSize.Width - ControlBox.DesiredSize.Width);
			}
			MeasureElement(IconContainer, availableSize);
			if(IconContainer != null) {			
				if(!double.IsPositiveInfinity(availableSize.Width))
					availableSize.Width = Math.Max(0, availableSize.Width - IconContainer.DesiredSize.Width);
			}
			MeasureElement(TitleContainer, availableSize);
		}
		protected virtual Size GetHeaderDesiredSize() {
			Size desiredSize = new Size();
			if(HeaderBackgroundBorder != null) {
				desiredSize.Height = HeaderBackgroundBorder.DesiredSize.Height;
			}			
			desiredSize = GetMaxHeightAndSumOfWidth(desiredSize, GetDesiredSize(ControlBox));
			desiredSize = GetMaxHeightAndSumOfWidth(desiredSize, GetDesiredSize(TitleContainer));
			desiredSize = GetMaxHeightAndSumOfWidth(desiredSize, GetDesiredSize(IconContainer));
			return desiredSize;
		}
		protected virtual void ArrangeHeader(Rect finalRect) {
			Rect headerRect = new Rect(finalRect.X, finalRect.Y, finalRect.Width, finalRect.Height);
			ArrangeElement(HeaderBackgroundBorder, headerRect);
			Size desiredSize = GetDesiredSize(ControlBox);
			ArrangeElement(ControlBox, new Rect(headerRect.Right - desiredSize.Width, headerRect.Y, desiredSize.Width, headerRect.Height));
			headerRect = SubstractPadding(headerRect, new Thickness(0, 0, desiredSize.Width, 0));
			desiredSize = GetDesiredSize(IconContainer);
			ArrangeElement(IconContainer, new Rect(headerRect.X, headerRect.Y, desiredSize.Width, headerRect.Height));
			headerRect = SubstractPadding(headerRect, new Thickness(desiredSize.Width, 0, 0, 0));						
			ArrangeElement(TitleContainer, headerRect);
		}
		Size GetDesiredSize(UIElement element) {
			if(element == null) return new Size();
			return element.DesiredSize;
		}
		Size GetMaxHeightAndSumOfWidth(Size size1, Size size2) {
			return new Size(size1.Width + size2.Width, Math.Max(size1.Height, size2.Height));
		}
		Size GetMaxWidthAndSumOfHeight(Size size1, Size size2) {
			return new Size(Math.Max(size1.Width, size2.Width), size1.Height + size2.Height);
		}
		Size SubstractHorizontalPadding(Size size, Thickness padding) {
			if(double.IsPositiveInfinity(size.Width))
				return size;
			return new Size(Math.Max(0, size.Width) - padding.Left - padding.Right, size.Height);			
		}
		Rect SubstractPadding(Rect rect, Thickness padding) {
			return new Rect(rect.X + padding.Left, rect.Y + padding.Top, Math.Max(0, rect.Width - padding.Left - padding.Right), Math.Max(0, rect.Height - padding.Top - padding.Bottom));
		}
		Size AddHorizontalPadding(Size size, Thickness padding) {
			if(double.IsPositiveInfinity(size.Width))
				return size;
			return new Size(size.Width + padding.Left + padding.Right, size.Height);
		}
		Size AddVerticalPadding(Size size, Thickness padding) {
			if(double.IsPositiveInfinity(size.Width))
				return size;
			return new Size(size.Width, size.Height + padding.Top + padding.Bottom);
		}
		Size SubstractVerticalPadding(Size size, Thickness padding) {
			if(double.IsPositiveInfinity(size.Height))
				return size;
			return new Size(size.Width, Math.Max(0, size.Height) - padding.Top - padding.Bottom);
		}
		void MeasureElement(UIElement element, Size availableSize) {
			if(element == null) return;
			element.Measure(availableSize);
		}
		void ArrangeElement(UIElement element, Rect finalRect) {
			if(element == null)
				return;
			element.Arrange(finalRect);
		}		
	}
}
