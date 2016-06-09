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
using System.ComponentModel;
#if !SL
using DevExpress.Xpf.Utils;
#else
using DevExpress.WPFToSLUtils;
using DevExpress.Xpf.Core;
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
#endif
namespace DevExpress.Xpf.Editors.Settings {
	public partial class SpinEditSettings : ButtonEditSettings {
		public static readonly DependencyProperty MinValueProperty;
		public static readonly DependencyProperty MaxValueProperty;
		public static readonly DependencyProperty IncrementProperty;
		public static readonly DependencyProperty IsFloatValueProperty;
		static SpinEditSettings() {
			Type ownerType = typeof(SpinEditSettings);
			MinValueProperty = DependencyPropertyManager.Register("MinValue", typeof(decimal?), ownerType, new FrameworkPropertyMetadata(null));
			MaxValueProperty = DependencyPropertyManager.Register("MaxValue", typeof(decimal?), ownerType, new FrameworkPropertyMetadata(null));
			IncrementProperty = DependencyPropertyManager.Register("Increment", typeof(decimal), ownerType, new FrameworkPropertyMetadata(decimal.One));
			IsFloatValueProperty = DependencyPropertyManager.Register("IsFloatValue", typeof(bool), ownerType, new FrameworkPropertyMetadata(true));
#if !SL
			MaskTypeProperty.OverrideMetadata(ownerType, new FrameworkPropertyMetadata(MaskType.Numeric));
#endif
		}
		[
#if !SL
	DevExpressXpfCoreLocalizedDescription("SpinEditSettingsMinValue"),
#endif
 Category(EditSettingsCategories.Behavior)]
#if SL
		[TypeConverter(typeof(NullableConverter<decimal>))]
#endif
		public decimal? MinValue {
			get { return (decimal?)GetValue(MinValueProperty); }
			set { SetValue(MinValueProperty, value); }
		}
		[
#if !SL
	DevExpressXpfCoreLocalizedDescription("SpinEditSettingsMaxValue"),
#endif
 Category(EditSettingsCategories.Behavior)]
#if SL
		[TypeConverter(typeof(NullableConverter<decimal>))]
#endif
		public decimal? MaxValue {
			get { return (decimal?)GetValue(MaxValueProperty); }
			set { SetValue(MaxValueProperty, value); }
		}
		[
#if !SL
	DevExpressXpfCoreLocalizedDescription("SpinEditSettingsIncrement"),
#endif
 Category(EditSettingsCategories.Behavior)]
#if SL
		[TypeConverter(typeof(NullableConverter<decimal>))]
#endif
		public decimal Increment {
			get { return (decimal)GetValue(IncrementProperty); }
			set { SetValue(IncrementProperty, value); }
		}
		[
#if !SL
	DevExpressXpfCoreLocalizedDescription("SpinEditSettingsIsFloatValue"),
#endif
 Category(EditSettingsCategories.Behavior)]
		public bool IsFloatValue {
			get { return (bool)GetValue(IsFloatValueProperty); }
			set { SetValue(IsFloatValueProperty, value); }
		}
		protected override void AssignToEditCore(IBaseEdit edit) {
			base.AssignToEditCore(edit);
			SpinEdit se = edit as SpinEdit;
			if(se == null)
				return;
			SetValueFromSettings(MinValueProperty, () => se.MinValue = MinValue);
			SetValueFromSettings(MaxValueProperty, () => se.MaxValue = MaxValue);
			SetValueFromSettings(IncrementProperty, () => se.Increment = Increment);
			SetValueFromSettings(IsFloatValueProperty, () => se.IsFloatValue = IsFloatValue);
		}
		protected internal override ButtonInfoBase CreateDefaultButtonInfo() {
			return new SpinButtonInfo() {
				IsDefaultButton = true,
			};
		}
	}
}
#if !SL
namespace DevExpress.Xpf.Editors.Settings.Extension {
	public class SpinSettingsExtension : ButtonSettingsExtension {
		public SpinSettingsExtension() {
			MaskType = MaskType.Numeric;
			AllowNullInput = true;
			MinValue = (decimal?)SpinEditSettings.MinValueProperty.DefaultMetadata.DefaultValue;
			MaxValue = (decimal?)SpinEditSettings.MaxValueProperty.DefaultMetadata.DefaultValue;
			Increment = (decimal)SpinEditSettings.IncrementProperty.DefaultMetadata.DefaultValue;
			IsFloatValue = (bool)SpinEditSettings.IsFloatValueProperty.DefaultMetadata.DefaultValue;
		}
		protected sealed override ButtonEditSettings CreateButtonEditSettings() {
			SpinEditSettings ses = CreateSpinEditSettings();
			ses.MinValue = MinValue;
			ses.MaxValue = MaxValue;
			ses.Increment = Increment;
			ses.IsFloatValue = IsFloatValue;
			return ses;
		}
		public virtual SpinEditSettings CreateSpinEditSettings() {
			return new SpinEditSettings();
		}
		public decimal? MinValue { get; set; }
		public decimal? MaxValue { get; set; }
		public decimal Increment { get; set; }
		public bool IsFloatValue { get; set; }
	}
}
#endif
