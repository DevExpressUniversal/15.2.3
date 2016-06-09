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
using System.Globalization;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Controls.Primitives;
#if !SL
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Utils.Themes;
using DevExpress.Xpf.Utils;
#else
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Editors.Internal;
using DevExpress.Xpf.Editors.WPFCompatibility.Extensions;
using DevExpress.WPFToSLUtils;
using DevExpress.Xpf.Core.WPFCompatibility;
#endif
#if SL
using DependencyPropertyChangedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLDependencyPropertyChangedEventArgs;
using PropertyMetadata = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyMetadata;
using PropertyChangedCallback = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyChangedCallback;
using RepeatButton = DevExpress.Xpf.Editors.WPFCompatibility.SLRepeatButton;
using RoutedEvent = DevExpress.Xpf.Core.WPFCompatibility.SLRoutedEvent;
using RoutedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLRoutedEventArgs;
using TransferContentControl = DevExpress.Xpf.Core.TransitionContentControl;
using TransferControl = DevExpress.Xpf.Core.TransitionControl;
#endif
namespace DevExpress.Xpf.Editors {
	public abstract class DateEditCalendarBase : ContentControl {
		public static readonly DependencyProperty DateTimeProperty;
		public static readonly DependencyProperty MinValueProperty;
		public static readonly DependencyProperty MaxValueProperty;
		public static readonly DependencyProperty ShowWeekNumbersProperty;
		public static readonly DependencyProperty ShowClearButtonProperty;
		public static readonly DependencyProperty ShowTodayProperty;
		public static readonly DependencyProperty MaskProperty;
		static DateEditCalendarBase() {
			Type ownerType = typeof(DateEditCalendarBase);
			DateTimeProperty = DependencyPropertyManager.RegisterAttached("DateTime", typeof(DateTime), ownerType, new FrameworkPropertyMetadata(DateTime.MinValue, OnDateTimePropertyChanged));
			MinValueProperty = DependencyPropertyManager.Register("MinValue", typeof(DateTime?), ownerType, new FrameworkPropertyMetadata(null, (d, e) => ((DateEditCalendarBase)d).MinValueChanged()));
			MaxValueProperty = DependencyPropertyManager.Register("MaxValue", typeof(DateTime?), ownerType, new FrameworkPropertyMetadata(null, (d, e) => ((DateEditCalendarBase)d).MaxValueChanged()));
			ShowWeekNumbersProperty = DependencyPropertyManager.RegisterAttached("ShowWeekNumbers", typeof(bool), ownerType, new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.Inherits));
			ShowClearButtonProperty = DependencyPropertyManager.Register("ShowClearButton", typeof(bool), ownerType, new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.AffectsParentMeasure, (d, e) => ((DateEditCalendarBase)d).OnShowClearButtonChanged()));
			ShowTodayProperty = DependencyPropertyManager.Register("ShowToday", typeof(bool), ownerType, new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.AffectsParentMeasure, (d, e) => ((DateEditCalendarBase)d).OnShowTodayChanged()));
			MaskProperty = DependencyPropertyManager.Register("Mask", typeof(string), ownerType, new PropertyMetadata("d", (d, e) => ((DateEditCalendarBase)d).OnMaskChanged((string)e.NewValue)));
		}
		static void OnDateTimePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			if (d is DateEditCalendarBase)
				((DateEditCalendarBase)d).OnDateTimeChanged();
		}
		public static object GetDateTime(DependencyObject obj) { return obj.GetValue(DateTimeProperty); }
		public static void SetDateTime(DependencyObject obj, object value) { obj.SetValue(DateTimeProperty, value); }
#if SL
		[TypeConverter(typeof(DateTimeTypeConverter))]
#endif
		public DateTime? MinValue { get { return (DateTime?)GetValue(MinValueProperty); } set { SetValue(MinValueProperty, value); } }
#if SL
		[TypeConverter(typeof(DateTimeTypeConverter))]
#endif
		public DateTime? MaxValue { get { return (DateTime?)GetValue(MaxValueProperty); } set { SetValue(MaxValueProperty, value); } }
#if SL
		[TypeConverter(typeof(DateTimeTypeConverter))]
#endif
		public DateTime DateTime { get { return (DateTime)GetValue(DateTimeProperty); } set { SetValue(DateTimeProperty, value); } }
		public bool ShowWeekNumbers { get { return (bool)GetValue(ShowWeekNumbersProperty); } set { SetValue(ShowWeekNumbersProperty, value); ShowWeekNumbersPropertySet(value); } }
		public bool ShowToday { get { return (bool)GetValue(ShowTodayProperty); } set { SetValue(ShowTodayProperty, value); } }
		public bool ShowClearButton { get { return (bool)GetValue(ShowClearButtonProperty); } set { SetValue(ShowClearButtonProperty, value); } }
		public string Mask { get { return (string)GetValue(MaskProperty); } set { SetValue(MaskProperty, value); } }
		protected internal virtual bool ProcessKeyDown(KeyEventArgs e) { return false; }
		protected virtual void OnMaskChanged(string newValue) { }
		protected virtual void OnDateTimeChanged() { }
		protected virtual void MinValueChanged() { }
		protected virtual void MaxValueChanged() { }
		protected virtual void OnShowClearButtonChanged() { }
		protected virtual void OnShowTodayChanged() { }
		protected virtual void ShowWeekNumbersPropertySet(bool value) { }
	}
}
