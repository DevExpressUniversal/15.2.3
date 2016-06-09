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
using DevExpress.Xpf.Utils;
namespace DevExpress.Xpf.NavBar {
	internal interface IScrollMode {
		ScrollControl ScrollControl { get; }		
	}
	public enum ScrollMode {
		Buttons,
		ScrollBar,
		None
	}
	public class ScrollControl : ContentControl {
		public static readonly DependencyProperty ScrollButtonsControlTemplateProperty;
		public static readonly DependencyProperty ScrollBarControlTemplateProperty;
		public static readonly DependencyProperty NoScrollControlTemplateProperty;
		public static readonly DependencyProperty ActualScrollModeProperty;		
		static ScrollControl() {
			ScrollButtonsControlTemplateProperty = DependencyPropertyManager.Register("ScrollButtonsControlTemplate", typeof(ControlTemplate), typeof(ScrollControl), new PropertyMetadata(null, (d, e) => ((ScrollControl)d).RefreshScrollMode()));
			ScrollBarControlTemplateProperty = DependencyPropertyManager.Register("ScrollBarControlTemplate", typeof(ControlTemplate), typeof(ScrollControl), new PropertyMetadata(null, (d, e) => ((ScrollControl)d).RefreshScrollMode()));
			NoScrollControlTemplateProperty = DependencyPropertyManager.Register("NoScrollControlTemplate", typeof(ControlTemplate), typeof(ScrollControl), new PropertyMetadata(null, (d, e) => ((ScrollControl)d).RefreshScrollMode()));
			ActualScrollModeProperty = DependencyPropertyManager.Register("ActualScrollMode", typeof(ScrollMode), typeof(ScrollControl), new FrameworkPropertyMetadata(ScrollMode.Buttons, (d, e) => ((ScrollControl)d).RefreshScrollMode()));
			FocusableProperty.OverrideMetadata(typeof(ScrollControl), new FrameworkPropertyMetadata(false));
		}
		public ControlTemplate ScrollButtonsControlTemplate {
			get { return (ControlTemplate)GetValue(ScrollButtonsControlTemplateProperty); }
			set { SetValue(ScrollButtonsControlTemplateProperty, value); }
		}
		public ControlTemplate ScrollBarControlTemplate {
			get { return (ControlTemplate)GetValue(ScrollBarControlTemplateProperty); }
			set { SetValue(ScrollBarControlTemplateProperty, value); }
		}
		public ControlTemplate NoScrollControlTemplate {
			get { return (ControlTemplate)GetValue(NoScrollControlTemplateProperty); }
			set { SetValue(NoScrollControlTemplateProperty, value); }
		}
		public ScrollMode ActualScrollMode {
			get { return (ScrollMode)GetValue(ActualScrollModeProperty); }
			set { SetValue(ActualScrollModeProperty, value); }
		}
		protected internal virtual void RefreshScrollMode() {
			switch(ActualScrollMode) { 
				case ScrollMode.ScrollBar:
					Template = ScrollBarControlTemplate;
					break;
				case ScrollMode.Buttons:
					Template = ScrollButtonsControlTemplate;
					break;
				case ScrollMode.None:
					Template = NoScrollControlTemplate != null ? NoScrollControlTemplate : ScrollButtonsControlTemplate;
					break;
			}
		}
	}
	public class ScrollingSettings : DependencyObject {
		public static readonly DependencyProperty ScrollSpeedProperty;
		public static readonly DependencyProperty TopBottomIndentProperty;
		public static readonly DependencyProperty ClickModeProperty;
		public static readonly DependencyProperty ScrollModeProperty;
		public static readonly DependencyProperty AccelerationRatioProperty;
		public static readonly DependencyProperty DecelerationRatioProperty;
		public static readonly DependencyProperty MinScrollValueProperty;
		public static readonly DependencyProperty IsManipulationEnabledProperty;						
		static ScrollingSettings() {
			ScrollSpeedProperty = DependencyPropertyManager.RegisterAttached("ScrollSpeed", typeof(double), typeof(ScrollingSettings), new FrameworkPropertyMetadata(200d));
			AccelerationRatioProperty = DependencyPropertyManager.RegisterAttached("AccelerationRatio", typeof(double), typeof(ScrollingSettings), new FrameworkPropertyMetadata(200d));
			DecelerationRatioProperty = DependencyPropertyManager.RegisterAttached("DecelerationRatio", typeof(double), typeof(ScrollingSettings), new FrameworkPropertyMetadata(200d));
			TopBottomIndentProperty = DependencyPropertyManager.RegisterAttached("TopBottomIndent", typeof(double), typeof(ScrollingSettings), new FrameworkPropertyMetadata(0d));
			ClickModeProperty = DependencyPropertyManager.RegisterAttached("ClickMode", typeof(ClickMode), typeof(ScrollingSettings), new FrameworkPropertyMetadata(ClickMode.Press));
			ScrollModeProperty = DependencyPropertyManager.RegisterAttached("ScrollMode", typeof(ScrollMode), typeof(ScrollingSettings), new PropertyMetadata(ScrollMode.Buttons, (d,e) => OnScrollModePropertyChanged(d,e)));
			MinScrollValueProperty = DependencyProperty.RegisterAttached("MinScrollValue", typeof(double?), typeof(ScrollingSettings), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.Inherits));
			IsManipulationEnabledProperty = DependencyProperty.RegisterAttached("IsManipulationEnabled", typeof(bool), typeof(ScrollingSettings), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.Inherits));
		}
		static void OnScrollModePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			UpdateScrollModeStates(d as Control, (ScrollMode)e.NewValue);
		}
		public static void SetClickMode(DependencyObject d, ClickMode value) {
			d.SetValue(ClickModeProperty, value);
		}
		public static ClickMode GetClickMode(DependencyObject d) {
			return (ClickMode)d.GetValue(ClickModeProperty);
		}
		public static void SetScrollSpeed(DependencyObject d, double value) {
			d.SetValue(ScrollSpeedProperty, value);
		}
		public static double GetScrollSpeed(DependencyObject d) {
			return (double)d.GetValue(ScrollSpeedProperty);
		}
		public static double? GetMinScrollValue(DependencyObject obj) {
			return (double?)obj.GetValue(MinScrollValueProperty);
		}
		public static void SetMinScrollValue(DependencyObject obj, double? value) {
			obj.SetValue(MinScrollValueProperty, value);
		}
		public static void SetAccelerationRatio(DependencyObject d, double value) {
			d.SetValue(AccelerationRatioProperty, value);
		}
		public static double GetAccelerationRatio(DependencyObject d) {
			return (double)d.GetValue(AccelerationRatioProperty);
		}
		public static void SetDecelerationRatio(DependencyObject d, double value) {
			d.SetValue(DecelerationRatioProperty, value);
		}
		public static double GetDecelerationRatio(DependencyObject d) {
			return (double)d.GetValue(DecelerationRatioProperty);
		}
		public static bool GetIsManipulationEnabled(DependencyObject obj) {
			return (bool)obj.GetValue(IsManipulationEnabledProperty);
		}
		public static void SetIsManipulationEnabled(DependencyObject obj, bool value) {
			obj.SetValue(IsManipulationEnabledProperty, value);
		}				
		public static void SetTopBottomIndent(DependencyObject d, double value) {
			d.SetValue(TopBottomIndentProperty, value);
		}
		public static double GetTopBottomIndent(DependencyObject d) {
			return (double)d.GetValue(TopBottomIndentProperty);
		}
		public static void SetScrollMode(DependencyObject obj, ScrollMode value) {
			obj.SetValue(ScrollModeProperty, value);			
		}
		public static ScrollMode GetScrollMode(DependencyObject obj) {
			return (ScrollMode)obj.GetValue(ScrollModeProperty);
		}
		internal static void UpdateScrollModeStates(Control source) {
			ScrollMode scrollMode = ScrollingSettings.GetScrollMode(source);
			UpdateScrollModeStates(source, scrollMode);
		}
		static void UpdateScrollModeStates(Control source, ScrollMode scrollMode) {
			if (source is IScrollMode && ((IScrollMode)source).ScrollControl != null)
				((IScrollMode)source).ScrollControl.ActualScrollMode = scrollMode;
		}
	}
}
