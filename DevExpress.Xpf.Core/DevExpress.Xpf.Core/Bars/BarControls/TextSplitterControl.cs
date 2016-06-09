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

using System.Windows.Controls;
using System.Windows;
using DevExpress.Xpf.Utils;
using System;
using System.Windows.Data;
using DevExpress.Xpf.Core;
namespace DevExpress.Xpf.Bars {
	public enum SplitTextMode { None, Auto, ByBreakLine, BySpace }
	public class TextSplitterControl : Panel {
		#region static
		public static readonly DependencyProperty ContentProperty;
		public static readonly DependencyProperty SplitMethodProperty;
		public static readonly DependencyProperty FirstStringHorizontalAlignmentProperty;
		public static readonly DependencyProperty SecondStringHorizontalAlignmentProperty;
		public static readonly DependencyProperty SecondStringProperty;
		public static readonly DependencyProperty FirstStringProperty;
		public static readonly DependencyProperty ContentVisibilityProperty;
		public static readonly DependencyProperty FirstStringOpacityProperty;
		public static readonly DependencyProperty SecondStringOpacityProperty;
		public static readonly DependencyProperty ContentTemplateProperty;
		public static readonly DependencyProperty IsUserContentVisibleProperty;
		protected static readonly DependencyPropertyKey IsUserContentVisiblePropertyKey;
		public static readonly DependencyProperty FirstStringMarginProperty;
		public static readonly DependencyProperty SecondStringMarginProperty;
		public static readonly DependencyProperty NormalTextStyleProperty;
		public static readonly DependencyProperty SelectedTextStyleProperty;
		public static readonly DependencyProperty ActualTextStyleProperty;
		protected static readonly DependencyPropertyKey ActualTextStylePropertyKey;
		public static readonly DependencyProperty IsRightSideArrowVisibleProperty;
		protected static readonly DependencyPropertyKey IsRightSideArrowVisiblePropertyKey;
		public static readonly DependencyProperty ActualArrowTemplateProperty;
		protected static readonly DependencyPropertyKey ActualArrowTemplatePropertyKey;
		public static readonly DependencyProperty RightSideArrowContainerStyleProperty;
		public static readonly DependencyProperty BottomSideArrowContainerStyleProperty;
		public static readonly DependencyProperty NormalArrowTemplateProperty;
		public static readonly DependencyProperty SelectedArrowTemplateProperty;
		public static readonly DependencyProperty IsArrowVisibleProperty;
		protected static readonly DependencyPropertyKey IsBottomSideArrowVisiblePropertyKey;
		public static readonly DependencyProperty IsBottomSideArrowVisibleProperty;
		public static readonly DependencyProperty IsSelectedProperty;
		public static readonly DependencyProperty DisabledTextStyleProperty;		
		static TextSplitterControl() {
			ContentProperty = DependencyPropertyManager.Register("Content", typeof(object), typeof(TextSplitterControl), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnContentPropertyChanged)));
			SplitMethodProperty = DependencyPropertyManager.Register("SplitMethod", typeof(SplitTextMode), typeof(TextSplitterControl),
				new FrameworkPropertyMetadata(SplitTextMode.Auto, new PropertyChangedCallback(OnSplitMethodPropertyChanged)));
			FirstStringProperty = DependencyPropertyManager.Register("FirstString", typeof(string), typeof(TextSplitterControl), new FrameworkPropertyMetadata(string.Empty));
			SecondStringProperty = DependencyPropertyManager.Register("SecondString", typeof(string), typeof(TextSplitterControl), new FrameworkPropertyMetadata(string.Empty));
			FirstStringHorizontalAlignmentProperty = DependencyPropertyManager.Register("FirstStringHorizontalAlignment", typeof(HorizontalAlignment), typeof(TextSplitterControl), new FrameworkPropertyMetadata(HorizontalAlignment.Center));
			SecondStringHorizontalAlignmentProperty = DependencyPropertyManager.Register("SecondStringHorizontalAlignment", typeof(HorizontalAlignment), typeof(TextSplitterControl), new FrameworkPropertyMetadata(HorizontalAlignment.Center));
			ContentVisibilityProperty = DependencyPropertyManager.Register("ContentVisibility", typeof(Visibility), typeof(TextSplitterControl), new FrameworkPropertyMetadata(Visibility.Collapsed));
			SecondStringOpacityProperty = DependencyPropertyManager.Register("SecondStringOpacity", typeof(double), typeof(TextSplitterControl), new FrameworkPropertyMetadata(0d));
			FirstStringOpacityProperty = DependencyPropertyManager.Register("FirstStringOpacity", typeof(double), typeof(TextSplitterControl), new FrameworkPropertyMetadata(0d));
			ContentTemplateProperty = DependencyPropertyManager.Register("ContentTemplate", typeof(ControlTemplate), typeof(TextSplitterControl), new UIPropertyMetadata(null));
			IsUserContentVisiblePropertyKey = DependencyPropertyManager.RegisterReadOnly("IsUserContentVisible", typeof(bool), typeof(TextSplitterControl), new UIPropertyMetadata(false));
			IsUserContentVisibleProperty = IsUserContentVisiblePropertyKey.DependencyProperty;
			FirstStringMarginProperty = DependencyPropertyManager.Register("FirstStringMargin", typeof(Thickness), typeof(TextSplitterControl), new FrameworkPropertyMetadata(new Thickness(), new PropertyChangedCallback(OnFirstStringMarginPropertyChanged)));
			SecondStringMarginProperty = DependencyPropertyManager.Register("SecondStringMargin", typeof(Thickness), typeof(TextSplitterControl), new FrameworkPropertyMetadata(new Thickness(), new PropertyChangedCallback(OnSecondStringMarginPropertyChanged)));
			NormalTextStyleProperty = DependencyPropertyManager.Register("NormalTextStyle", typeof(Style), typeof(TextSplitterControl), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnNormalTextStylePropertyChanged)));
			DisabledTextStyleProperty = DependencyPropertyManager.Register("DisabledTextStyle", typeof(Style), typeof(TextSplitterControl), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnDisabledTextStylePropertyChanged)));
			SelectedTextStyleProperty = DependencyPropertyManager.Register("SelectedTextStyle", typeof(Style), typeof(TextSplitterControl), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnSelectedTextStylePropertyChanged)));
			ActualTextStylePropertyKey = DependencyPropertyManager.RegisterReadOnly("ActualTextStyle", typeof(Style), typeof(TextSplitterControl), new UIPropertyMetadata(null));
			ActualTextStyleProperty = ActualTextStylePropertyKey.DependencyProperty;
			RightSideArrowContainerStyleProperty = DependencyPropertyManager.Register("RightSideArrowContainerStyle", typeof(Style), typeof(TextSplitterControl), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnRightSideArrowContainerStylePropertyChanged)));
			BottomSideArrowContainerStyleProperty = DependencyPropertyManager.Register("BottomSideArrowContainerStyle", typeof(Style), typeof(TextSplitterControl), new UIPropertyMetadata(null));
			IsRightSideArrowVisiblePropertyKey = DependencyPropertyManager.RegisterReadOnly("IsRightSideArrowVisible", typeof(bool), typeof(TextSplitterControl), new UIPropertyMetadata(false));
			IsRightSideArrowVisibleProperty = IsRightSideArrowVisiblePropertyKey.DependencyProperty;
			ActualArrowTemplatePropertyKey = DependencyPropertyManager.RegisterReadOnly("ActualArrowTemplate", typeof(ControlTemplate), typeof(TextSplitterControl), new FrameworkPropertyMetadata(null));
			ActualArrowTemplateProperty = ActualArrowTemplatePropertyKey.DependencyProperty;
			NormalArrowTemplateProperty = DependencyPropertyManager.Register("NormalArrowTemplate", typeof(ControlTemplate), typeof(TextSplitterControl), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnNormalArrowTemplatePropertyChanged)));
			SelectedArrowTemplateProperty = DependencyPropertyManager.Register("SelectedArrowTemplate", typeof(ControlTemplate), typeof(TextSplitterControl), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnSelectedArrowTemplatePropertyChanged)));
			IsArrowVisibleProperty = DependencyPropertyManager.Register("IsArrowVisible", typeof(bool), typeof(TextSplitterControl), new FrameworkPropertyMetadata(false, new PropertyChangedCallback(OnIsArrowVisiblePropertyChanged)));
			IsBottomSideArrowVisiblePropertyKey = DependencyPropertyManager.RegisterReadOnly("IsBottomSideArrowVisible", typeof(bool), typeof(TextSplitterControl), new UIPropertyMetadata(false));
			IsBottomSideArrowVisibleProperty = IsBottomSideArrowVisiblePropertyKey.DependencyProperty;
			IsSelectedProperty = DependencyPropertyManager.Register("IsSelected", typeof(bool), typeof(TextSplitterControl), new FrameworkPropertyMetadata(false, new PropertyChangedCallback(OnIsSelectedPropertyChanged)));
		}
		protected static void OnContentPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e) {
			((TextSplitterControl)obj).OnContentChanged(e.OldValue as ContentControl);
		}
		protected static void OnFirstStringMarginPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e) {
			((TextSplitterControl)obj).InvalidateMeasure();
		}
		protected static void OnSecondStringMarginPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e) {
			((TextSplitterControl)obj).InvalidateMeasure();
		}
		protected static void OnNormalTextStylePropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e) {
			((TextSplitterControl)obj).UpdateLayoutProperties();
		}		
		protected static void OnDisabledTextStylePropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e) {
			((TextSplitterControl)obj).UpdateLayoutProperties();
		}
		protected static void OnSelectedTextStylePropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e) {
			((TextSplitterControl)obj).UpdateLayoutProperties();
		}
		protected static void OnRightSideArrowContainerStylePropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e) {
			((TextSplitterControl)obj).UpdateLayoutProperties();
		}
		protected static void OnNormalArrowTemplatePropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e) {
			((TextSplitterControl)obj).UpdateLayoutProperties();
		}
		protected static void OnSelectedArrowTemplatePropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e) {
			((TextSplitterControl)obj).UpdateLayoutProperties();
		}
		protected static void OnIsSelectedPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e) {
			((TextSplitterControl)obj).UpdateLayoutProperties();
			((TextSplitterControl)obj).OnFontSettingsChanged(null);
		}
		protected static void OnIsArrowVisiblePropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e) {
			((TextSplitterControl)obj).UpdateLayoutProperties();
		}
		#endregion
		#region prop defs
		public object Content {
			get { return (object)GetValue(ContentProperty); }
			set { SetValue(ContentProperty, value); }
		}
		public string FirstString {
			get { return (string)GetValue(FirstStringProperty); }
			set { SetValue(FirstStringProperty, value); }
		}
		public string SecondString {
			get { return (string)GetValue(SecondStringProperty); }
			set { SetValue(SecondStringProperty, value); }
		}
		public SplitTextMode SplitMethod {
			get { return (SplitTextMode)GetValue(SplitMethodProperty); }
			set { SetValue(SplitMethodProperty, value); }
		}
		protected static void OnSplitMethodPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((TextSplitterControl)d).OnSplitMethodChanged((SplitTextMode)e.OldValue);
		}
		public HorizontalAlignment FirstStringHorizontalAlignment {
			get { return (HorizontalAlignment)GetValue(FirstStringHorizontalAlignmentProperty); }
			set { SetValue(FirstStringHorizontalAlignmentProperty, value); }
		}
		public HorizontalAlignment SecondStringHorizontalAlignment {
			get { return (HorizontalAlignment)GetValue(SecondStringHorizontalAlignmentProperty); }
			set { SetValue(SecondStringHorizontalAlignmentProperty, value); }
		}
		public Visibility ContentVisibility {
			get { return (Visibility)GetValue(ContentVisibilityProperty); }
			set { SetValue(ContentVisibilityProperty, value); }
		}
		public double FirstStringOpacity {
			get { return (double)GetValue(FirstStringOpacityProperty); }
			set { SetValue(FirstStringOpacityProperty, value); }
		}
		public double SecondStringOpacity {
			get { return (double)GetValue(SecondStringOpacityProperty); }
			set { SetValue(SecondStringOpacityProperty, value); }
		}
		public ControlTemplate ContentTemplate {
			get { return (ControlTemplate)GetValue(ContentTemplateProperty); }
			set { SetValue(ContentTemplateProperty, value); }
		}
		public bool IsUserContentVisible {
			get { return (bool)GetValue(IsUserContentVisibleProperty); }
			protected set { this.SetValue(IsUserContentVisiblePropertyKey, value); }
		}
		public Thickness FirstStringMargin {
			get { return (Thickness)GetValue(FirstStringMarginProperty); }
			set { SetValue(FirstStringMarginProperty, value); }
		}
		public Thickness SecondStringMargin {
			get { return (Thickness)GetValue(SecondStringMarginProperty); }
			set { SetValue(SecondStringMarginProperty, value); }
		}
		public Style NormalTextStyle {
			get { return (Style)GetValue(NormalTextStyleProperty); }
			set { SetValue(NormalTextStyleProperty, value); }
		}
		public Style DisabledTextStyle {
			get { return (Style)GetValue(DisabledTextStyleProperty); }
			set { SetValue(DisabledTextStyleProperty, value); }
		}
		public Style SelectedTextStyle {
			get { return (Style)GetValue(SelectedTextStyleProperty); }
			set { SetValue(SelectedTextStyleProperty, value); }
		}
		public Style ActualTextStyle {
			get { return (Style)GetValue(ActualTextStyleProperty); }
			protected set { this.SetValue(ActualTextStylePropertyKey, value); }
		}
		public Style RightSideArrowContainerStyle {
			get { return (Style)GetValue(RightSideArrowContainerStyleProperty); }
			set { SetValue(RightSideArrowContainerStyleProperty, value); }
		}
		public Style BottomSideArrowContainerStyle {
			get { return (Style)GetValue(BottomSideArrowContainerStyleProperty); }
			set { SetValue(BottomSideArrowContainerStyleProperty, value); }
		}
		public bool IsRightSideArrowVisible {
			get { return (bool)GetValue(IsRightSideArrowVisibleProperty); }
			protected set { this.SetValue(IsRightSideArrowVisiblePropertyKey, value); }
		}
		public ControlTemplate ActualArrowTemplate {
			get { return (ControlTemplate)GetValue(ActualArrowTemplateProperty); }
			protected set { this.SetValue(ActualArrowTemplatePropertyKey, value); }
		}
		public ControlTemplate NormalArrowTemplate {
			get { return (ControlTemplate)GetValue(NormalArrowTemplateProperty); }
			set { SetValue(NormalArrowTemplateProperty, value); }
		}
		public ControlTemplate SelectedArrowTemplate {
			get { return (ControlTemplate)GetValue(SelectedArrowTemplateProperty); }
			set { SetValue(SelectedArrowTemplateProperty, value); }
		}
		public bool IsArrowVisible {
			get { return (bool)GetValue(IsArrowVisibleProperty); }
			set { SetValue(IsArrowVisibleProperty, value); }
		}
		public bool IsBottomSideArrowVisible {
			get { return (bool)GetValue(IsBottomSideArrowVisibleProperty); }
			protected set { this.SetValue(IsBottomSideArrowVisiblePropertyKey, value); }
		}
		public bool IsSelected {
			get { return (bool)GetValue(IsSelectedProperty); }
			set { SetValue(IsSelectedProperty, value); }
		}
		private FontSettings fontSettings;
		private BorderState borderState;
		public BorderState BorderState {
			get { return borderState; }
			set {
				if (value == borderState) return;
				BorderState oldValue = borderState;
				borderState = value;
				OnBorderStateChanged(oldValue);
			}
		}		
		protected internal FontSettings FontSettings {
			get { return fontSettings; }
			set {
				bool raiseChange = value != fontSettings;
				FontSettings oldValue = fontSettings;
				fontSettings = value;
				if(raiseChange)
				OnFontSettingsChanged(oldValue);
			}
		}				
		#endregion
		public TextSplitterControl() {
			IsEnabledChanged += OnIsEnabledChanged;
		}
		protected virtual void OnIsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e) {
			UpdateLayoutProperties();			
			ApplyFontSettings();
		}
		protected virtual void OnFontSettingsChanged(FontSettings oldValue) {
			if(FontSettings == null && oldValue != null) {
				FontSettings.Clear(firstStringControl);
				FontSettings.Clear(secondStringControl);
			}
			ApplyFontSettings();
		}
		protected virtual void OnBorderStateChanged(BorderState oldValue) {
			ApplyFontSettings();
		}
		void ApplyFontSettings() {			
			if (FontSettings != null) {
				BorderState borderState = BorderState;
				if (borderState == Bars.BorderState.Default) {
					if (IsMouseOver)
						borderState = BorderState.Hover;
					if (IsSelected)
						borderState = BorderState.Pressed;
					if (!IsEnabled)
						borderState = BorderState.Disabled;
				}
				FontSettings.Apply(firstStringControl, borderState);
				FontSettings.Apply(secondStringControl, borderState);
			}
		}
		protected virtual void OnContentChanged(object oldValue) {
			UpdateLayoutProperties();
		}
		protected virtual void OnSplitMethodChanged(SplitTextMode oldValue) {
			UpdateLayoutProperties();
		}
		protected virtual void SplitTextAutomatically(string text, ref string firstString, ref string secondString) {
			SplitTextBySpace(text, ref firstString, ref secondString);
		}
		protected virtual void SplitTextByBreakLine(string text, ref string firstString, ref string secondString) {
			SplitTextBySpace(text, ref firstString, ref secondString);
		}
		protected virtual void SplitTextBySpace(string text, ref string firstString, ref string secondString) {
			int indexOfSplitSpace = text.IndexOf(' ');
			int spaceIndex = -1;
			if(indexOfSplitSpace == -1) {
				firstString = text;
				secondString = string.Empty;
				return;
			}
			do {
				spaceIndex = text.IndexOf(' ', indexOfSplitSpace + 1);
				if(spaceIndex != -1) {
					if(text.Length / 2 >= indexOfSplitSpace && text.Length / 2 <= spaceIndex) {
						indexOfSplitSpace = spaceIndex;
						break;
					}
					indexOfSplitSpace = spaceIndex;
				}
			} while(spaceIndex != -1);
			firstString = text.Substring(0, indexOfSplitSpace);
			secondString = text.Substring(indexOfSplitSpace + 1);			
		}
		protected virtual void SplitText(string text, ref string firstString, ref string secondString) {
			switch(SplitMethod) {
				case SplitTextMode.Auto:
					SplitTextAutomatically(text, ref firstString, ref secondString);
					return;
				case SplitTextMode.ByBreakLine:
					SplitTextByBreakLine(text, ref firstString, ref secondString);
					return;
				case SplitTextMode.BySpace:
					SplitTextBySpace(text, ref firstString, ref secondString);
					return;
				case SplitTextMode.None:
					firstString = text;
					secondString = string.Empty;
					return;
			}
		}
		protected override void OnMouseEnter(System.Windows.Input.MouseEventArgs e) {
			base.OnMouseEnter(e);
			UpdateLayoutProperties();
		}
		protected override void OnMouseLeave(System.Windows.Input.MouseEventArgs e) {
			base.OnMouseLeave(e);
			UpdateLayoutProperties();
		}
		protected virtual void OnPropertiesChanged(DependencyPropertyChangedEventArgs e) {
		}
		protected virtual void UpdateLayoutProperties() {
			if(Content == null) {
				ContentVisibility = System.Windows.Visibility.Collapsed;
				UpdateControls();
				return;
			}
			string firstString = string.Empty;
			string secondString = string.Empty;
			string content = Content is string ? Content as string : Content.ToString();
			SplitText(content, ref firstString, ref secondString);
			FirstStringOpacity = firstString.Length == 0 ? 0 : 1;
			SecondStringOpacity = secondString.Length == 0 ? 0 : 1;
			FirstString = firstString;
			SecondString = secondString;
			ContentVisibility = Visibility.Visible;
			if(IsArrowVisible) {
				IsBottomSideArrowVisible = FirstString.Length == 0 || SecondString.Length == 0;
				IsRightSideArrowVisible = !IsBottomSideArrowVisible;
			}
			else {
				IsBottomSideArrowVisible = false;
				IsRightSideArrowVisible = false;
			}
			if(IsSelected) {
				ActualArrowTemplate = SelectedArrowTemplate;
				ActualTextStyle = SelectedTextStyle;
			}
			else {
				ActualArrowTemplate = NormalArrowTemplate;
				ActualTextStyle = IsEnabled ? NormalTextStyle : (DisabledTextStyle ?? NormalTextStyle);
			}
			UpdateControls();
		}
		protected virtual void UpdateControls() {
			if(ContentVisibility == Visibility.Collapsed) {
				Children.Clear();
				return;
			}
			if(FirstStringOpacity == 0) {
				if(this.firstStringControl != null)
					Children.Remove(FirstStringControl);
			}
			else {
				if(!Children.Contains(FirstStringControl))
					Children.Insert(0, FirstStringControl);
				FirstStringControl.Style = ActualTextStyle;
				FirstStringControl.Content = FirstString;
				FirstStringControl.HorizontalAlignment = FirstStringHorizontalAlignment;
			}
			if(SecondStringOpacity == 0) {
				if(this.secondStringControl != null)
					Children.Remove(SecondStringControl);
			}
			else {
				if(!Children.Contains(SecondStringControl))
					Children.Insert(Children.Count, SecondStringControl);
				SecondStringControl.Style = ActualTextStyle;
				SecondStringControl.Content = SecondString;
			}
			if(!IsArrowVisible) {
				if(this.arrowControl != null)
					Children.Remove(ArrowControl);
			}
			else {
				if(!Children.Contains(ArrowControl))
					Children.Add(ArrowControl);
				ArrowControl.Style = IsRightSideArrowVisible ? RightSideArrowContainerStyle : BottomSideArrowContainerStyle;
				ArrowControl.Template = ActualArrowTemplate;
			}
			ApplyFontSettings();
		}
		ContentControl firstStringControl, secondStringControl, arrowControl;
		protected internal ContentControl FirstStringControl {
			get {
				if(firstStringControl == null) {
					firstStringControl = CreateFirstStringControl();
					OnFontSettingsChanged(null);
				}
				return firstStringControl;
			}
		}
		protected virtual ContentControl CreateFirstStringControl() {
			return new ContentControl() { 
				Focusable = false,				 
				IsTabStop = false
			};			
		}
		protected internal ContentControl SecondStringControl {
			get {
				if(secondStringControl == null) {
					secondStringControl = CreateSecondStringControl();
					OnFontSettingsChanged(null);
				}
				return secondStringControl;
			}
		}
		protected virtual ContentControl CreateSecondStringControl() {
			return new ContentControl() { 
				Focusable = false, 
				IsTabStop = false
			};
		}
		protected internal ContentControl ArrowControl {
			get {
				if(arrowControl == null)
					arrowControl = CreateArrowControl();
				return arrowControl;
			}
		}
		protected virtual ContentControl CreateArrowControl() {
			return new ContentControl() { 
				Focusable = false, 
				IsTabStop = false
			};
		}
		Size GetActualSize(UIElement element, Thickness margin) {
			return new Size(Math.Max(0, element.DesiredSize.Width + margin.Left + margin.Right),
					Math.Max(0, element.DesiredSize.Height + margin.Bottom + margin.Top));
		}
		protected override Size MeasureOverride(Size availableSize) {
			Size text1 = new Size(0,0), text2 = new Size(0,0), arrow = new Size(0,0);
			if(FirstStringOpacity == 1) {
				FirstStringControl.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
				text1 = GetActualSize(FirstStringControl, FirstStringMargin);
			}
			if (SecondStringOpacity == 1) {
				SecondStringControl.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
				text2 = GetActualSize(SecondStringControl, SecondStringMargin);
			} else {
				text2 = new Size(0d, Math.Max(text1.Height + SecondStringMargin.Top + SecondStringMargin.Bottom,0));
			}
			if(IsArrowVisible) {
				ArrowControl.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
				arrow = ArrowControl.DesiredSize;
			}
			if(!IsArrowVisible) {
				return new Size(Math.Max(text1.Width, text2.Width), text1.Height + text2.Height);
			}
			if(IsBottomSideArrowVisible) {
				return new Size(Math.Max(text1.Width, arrow.Width), text1.Height + Math.Max(text2.Height, arrow.Height));
			}
			return new Size(Math.Max(text1.Width, text2.Width + arrow.Width), text1.Height + Math.Max(text2.Height, arrow.Height));
		}
		protected override Size ArrangeOverride(Size finalSize) {
			double top = 0;
			if(FirstStringOpacity == 1) {
				top = FirstStringMargin.Top;
				FirstStringControl.Arrange(new Rect(FirstStringMargin.Left, top, finalSize.Width - FirstStringMargin.Left - FirstStringMargin.Right, FirstStringControl.DesiredSize.Height));
				top += Math.Ceiling(firstStringControl.ActualHeight);
				top += FirstStringMargin.Bottom;
			}
			if(SecondStringOpacity == 1) {
				top += SecondStringMargin.Top;
				if(IsArrowVisible) {
					ArrangeRow(SecondStringControl, ArrowControl, SecondStringHorizontalAlignment, top, finalSize.Width - SecondStringMargin.Left - SecondStringMargin.Right);
				}
				else {
					ArrangeRow(SecondStringControl, null, SecondStringHorizontalAlignment, top, finalSize.Width);					
				}
			}
			else {
				if(IsArrowVisible) {
					ArrowControl.Arrange(new Rect(new Point((finalSize.Width - ArrowControl.DesiredSize.Width) / 2, FirstStringControl.DesiredSize.Height), ArrowControl.DesiredSize));
				}
			}
			return finalSize;
		}
		protected virtual void ArrangeRow(UIElement child1, UIElement child2, HorizontalAlignment horzAlignment, double top, double finalWidth) {
			double height = child1.DesiredSize.Height;
			double width = child1.DesiredSize.Width;
			if(child2 != null) {
				height = Math.Max(height, child2.DesiredSize.Height);
				width += child2.DesiredSize.Width;
			}
			double left = 0;
			if(horzAlignment == System.Windows.HorizontalAlignment.Right)
				left = finalWidth - left;
			else if(horzAlignment == System.Windows.HorizontalAlignment.Center)
				left = (finalWidth - width) / 2;
			child1.Arrange(new Rect(left, top, child1.DesiredSize.Width, height));
			if(child2 != null) {
				child2.Arrange(new Rect(left + child1.DesiredSize.Width, top, child2.DesiredSize.Width, height));
			}
		}
	}
	public class ContentSelector : Panel {
		public static readonly DependencyProperty SelectedIndexProperty = DependencyPropertyManager.Register("SelectedIndex", typeof(int), typeof(ContentSelector), new FrameworkPropertyMetadata(0, new PropertyChangedCallback(OnSelectedIndexChanged)));
		public static readonly DependencyProperty SnapperTypeProperty = DependencyPropertyManager.Register("SnapperType", typeof(SnapperType), typeof(ContentSelector), new FrameworkPropertyMetadata(SnapperType.Ceil, FrameworkPropertyMetadataOptions.AffectsMeasure));
		public static readonly DependencyProperty Content1Property = DependencyPropertyManager.Register("Content1", typeof(UIElement), typeof(ContentSelector), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnContent1Changed)));
		public static readonly DependencyProperty Content2Property = DependencyPropertyManager.Register("Content2", typeof(UIElement), typeof(ContentSelector), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnContent2Changed)));
		private static void OnContent1Changed(DependencyObject o, DependencyPropertyChangedEventArgs e) {
			ContentSelector contentSelector = o as ContentSelector;
			if(contentSelector != null)
				contentSelector.OnContent1Changed((UIElement)e.OldValue, (UIElement)e.NewValue);
		}
		protected virtual void OnContent1Changed(UIElement oldValue, UIElement newValue) {
			Content1Presenter.Content = newValue;
		}
		private static void OnContent2Changed(DependencyObject o, DependencyPropertyChangedEventArgs e) {
			ContentSelector contentSelector = o as ContentSelector;
			if(contentSelector != null)
				contentSelector.OnContent2Changed((UIElement)e.OldValue, (UIElement)e.NewValue);
		}
		protected virtual void OnContent2Changed(UIElement oldValue, UIElement newValue) {
			Content2Presenter.Content = newValue;
		}
		public UIElement Content1 {
			get { return (UIElement)GetValue(Content1Property); }
			set { SetValue(Content1Property, value); }
		}
		public UIElement Content2 {
			get { return (UIElement)GetValue(Content2Property); }
			set { SetValue(Content2Property, value); }
		}
		public SnapperType SnapperType {
			get { return (SnapperType)GetValue(SnapperTypeProperty); }
			set { SetValue(SnapperTypeProperty, value); }
		}
		ContentPresenter Content1Presenter { get; set; }
		ContentPresenter Content2Presenter { get; set; }
		public ContentSelector() {
			Content1Presenter = new ContentPresenter();
			Content2Presenter = new ContentPresenter() { Visibility = Visibility.Collapsed };
			Children.Add(Content1Presenter);
			Children.Add(Content2Presenter);
		}
		private static void OnSelectedIndexChanged(DependencyObject o, DependencyPropertyChangedEventArgs e) {
			ContentSelector contentSelector = o as ContentSelector;
			if(contentSelector != null)
				contentSelector.OnSelectedIndexChanged((int)e.OldValue, (int)e.NewValue);
		}
		protected virtual void OnSelectedIndexChanged(int oldValue, int newValue) {
				Content1Presenter.Visibility = SelectedIndex == 0 ? Visibility.Visible : Visibility.Collapsed;
				Content2Presenter.Visibility = SelectedIndex == 1 ? Visibility.Visible : Visibility.Collapsed;
		}
		public int SelectedIndex {
			get { return (int)GetValue(SelectedIndexProperty); }
			set { SetValue(SelectedIndexProperty, value); }
		}
		protected override Size MeasureOverride(Size availableSize) {
			Size res = new Size();
			Content1Presenter.Measure(availableSize);
			Content2Presenter.Measure(availableSize);
			if(SelectedIndex == 0) {				
				res = Content1Presenter.DesiredSize;
			} else {			
				res = Content2Presenter.DesiredSize;
			}
			return MeasurePixelSnapperHelper.MeasureOverride(res, SnapperType);
		}
		protected override Size ArrangeOverride(Size finalSize) {
			if(SelectedIndex == 0) {
				Content1Presenter.Arrange(new Rect(0, 0, finalSize.Width, finalSize.Height));
			} else {
				Content2Presenter.Arrange(new Rect(0, 0, finalSize.Width, finalSize.Height));
			}
			return finalSize;
		}
	}
	public class BooleanToIntegerConverter : IValueConverter {
		object IValueConverter.Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			return ((bool)value) ? 1 : 0;
		}
		object IValueConverter.ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			return ((int)value) == 0 ? false : true;
		}
	} 
}
