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
using System.ComponentModel;
using System.Collections;
using DevExpress.Xpf.Core.Native;
#if !SL
using DevExpress.Xpf.Editors.Helpers;
using DevExpress.Xpf.Utils;
using DevExpress.Xpf.Core;
#else
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Editors.Internal;
using DevExpress.WPFToSLUtils;
using DevExpress.Xpf.Core.WPFCompatibility;
#endif
#if SL
using ContextMenu = System.Windows.Controls.SLContextMenu;
using Control = DevExpress.Xpf.Core.WPFCompatibility.SLControl;
using DependencyPropertyChangedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLDependencyPropertyChangedEventArgs;
using PropertyMetadata = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyMetadata;
using PropertyChangedCallback = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyChangedCallback;
using RoutedEvent = DevExpress.Xpf.Core.WPFCompatibility.SLRoutedEvent;
using RoutedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLRoutedEventArgs;
using TextBox = DevExpress.Xpf.Editors.Controls.SLTextBox;
using DateTimeTypeConverter = DevExpress.Xpf.Core.DateTimeTypeConverter;
#endif
namespace DevExpress.Xpf.Editors.Settings {
	public partial class DateEditSettings : PopupBaseEditSettings {
		#region static
		public static readonly DependencyProperty MinValueProperty;
		public static readonly DependencyProperty MaxValueProperty;
		public static readonly DependencyProperty ShowWeekNumbersProperty;
		static DateEditSettings() {
			Type ownerType = typeof(DateEditSettings);
			MinValueProperty = DependencyPropertyManager.Register("MinValue", typeof(DateTime?), typeof(DateEditSettings), new PropertyMetadata(null, OnMinValuePropertyChanged));
			MaxValueProperty = DependencyPropertyManager.Register("MaxValue", typeof(DateTime?), typeof(DateEditSettings), new PropertyMetadata(null, OnMaxValuePropertyChanged));
			ShowWeekNumbersProperty = DependencyPropertyManager.Register("ShowWeekNumbers", typeof(bool), typeof(DateEditSettings), new PropertyMetadata(false));
#if !SL
			DisplayFormatProperty.OverrideMetadata(ownerType, new FrameworkPropertyMetadata("d"));
			MaskTypeProperty.OverrideMetadata(ownerType, new FrameworkPropertyMetadata(MaskType.DateTime));
			MaskProperty.OverrideMetadata(ownerType, new FrameworkPropertyMetadata("d"));
			AllowNullInputProperty.OverrideMetadata(ownerType, new FrameworkPropertyMetadata(true));
			ShowSizeGripProperty.OverrideMetadata(ownerType, new FrameworkPropertyMetadata(false));
#endif
		}
		protected static void OnMinValuePropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e) {
			((DateEditSettings)obj).OnMinValueChanged();
		}
		protected static void OnNullValuePropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e) {
			((DateEditSettings)obj).OnNullValueChanged();
		}
		protected static void OnMaxValuePropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e) {
			((DateEditSettings)obj).OnMaxValueChanged();
		}
		#endregion
		public DateEditSettings() {
#if SL
			ConstructorSLPart();
#endif
		}
		private void SetDefaultDisplayFormat() {
			if (this.HasDefaultValue(DisplayFormatProperty)) DisplayFormat = "d";
		}
		void SetDefaultMask() {
			if (this.HasDefaultValue(MaskProperty)) Mask = "d";
		}
		[
#if !SL
	DevExpressXpfCoreLocalizedDescription("DateEditSettingsShowWeekNumbers"),
#endif
 Category(EditSettingsCategories.Behavior)]
		public bool ShowWeekNumbers {
			get { return (bool)GetValue(ShowWeekNumbersProperty); }
			set { SetValue(ShowWeekNumbersProperty, value); }
		}
		[
#if !SL
	DevExpressXpfCoreLocalizedDescription("DateEditSettingsMinValue"),
#endif
 Category(EditSettingsCategories.Behavior)]
#if SL
		[TypeConverter(typeof(DateTimeTypeConverter))]
#endif
		public DateTime? MinValue {
			get { return (DateTime?)GetValue(MinValueProperty); }
			set { SetValue(MinValueProperty, value); }
		}
		[
#if !SL
	DevExpressXpfCoreLocalizedDescription("DateEditSettingsMaxValue"),
#endif
 Category(EditSettingsCategories.Behavior)]
#if SL
		[TypeConverter(typeof(DateTimeTypeConverter))]
#endif
		public DateTime? MaxValue {
			get { return (DateTime?)GetValue(MaxValueProperty); }
			set { SetValue(MaxValueProperty, value); }
		}
		protected virtual void OnMinValueChanged() {
		}
		protected virtual void OnMaxValueChanged() {
		}
		protected virtual void OnNullValueChanged() {
		}
		protected override void OnInitialized(EventArgs e) {
			base.OnInitialized(e);
			SetDefaultDisplayFormat();
#if SL
			SetDefaultMask();
#endif
		}
		protected override void AssignToEditCore(IBaseEdit edit) {
			base.AssignToEditCore(edit);
			DateEdit de = edit as DateEdit;
			if (de == null) return;
			SetValueFromSettings(ShowWeekNumbersProperty, () => de.ShowWeekNumbers = ShowWeekNumbers);
			SetValueFromSettings(MinValueProperty, () => de.MinValue = MinValue);
			SetValueFromSettings(MaxValueProperty, () => de.MaxValue = MaxValue);
			SetValueFromSettings(NullValueProperty, () => de.NullValue = NullValue);
		}
	}
}
#if !SL
namespace DevExpress.Xpf.Editors.Settings.Extension {
	public class DateSettingsExtension : PopupBaseSettingsExtension {
		public bool ShowWeekNumbers { get; set; }
		public DateTime? MinValue { get; set; }
		public DateTime? MaxValue { get; set; }
		public object NullValue { get; set; }
		public DateSettingsExtension() {
			ShowSizeGrip = false;
			MaskType = MaskType.DateTime;
			Mask = "d";
			DisplayFormat = "d";
			MinValue = (DateTime?)DateEditSettings.MinValueProperty.DefaultMetadata.DefaultValue;
			MaxValue = (DateTime?)DateEditSettings.MaxValueProperty.DefaultMetadata.DefaultValue;
			ShowWeekNumbers = (bool)DateEditSettings.ShowWeekNumbersProperty.DefaultMetadata.DefaultValue;
		}
		protected override PopupBaseEditSettings CreatePopupBaseEditSettings() {
			DateEditSettings de = new DateEditSettings();
			de.NullValue = NullValue;
			de.MinValue = MinValue;
			de.MaxValue = MaxValue;
			de.ShowWeekNumbers = ShowWeekNumbers;
			return de;
		}
	}
}
#endif
