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

using System.ComponentModel;
using System.Windows;
using DevExpress.Utils.Serializing;
using DevExpress.Xpf.Charts.Native;
using DevExpress.Xpf.Utils;
namespace DevExpress.Xpf.Charts {
	public class NavigationOptionsBase : ChartDependencyObject {
		public static readonly DependencyProperty UseKeyboardProperty = DependencyPropertyManager.Register("UseKeyboard",
			typeof(bool), typeof(NavigationOptionsBase), new PropertyMetadata(true, NotifyPropertyChanged));
		public static readonly DependencyProperty UseMouseProperty = DependencyPropertyManager.Register("UseMouse",
			typeof(bool), typeof(NavigationOptionsBase), new PropertyMetadata(true, NotifyPropertyChanged));
		public static readonly DependencyProperty UseTouchDeviceProperty = DependencyPropertyManager.Register("UseTouchDevice",
			typeof(bool), typeof(NavigationOptionsBase), new PropertyMetadata(true, NotifyPropertyChanged));
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("NavigationOptionsBaseUseKeyboard"),
#endif
		Category(Categories.Navigation),
		XtraSerializableProperty
		]
		public bool UseKeyboard {
			get { return (bool)GetValue(UseKeyboardProperty); }
			set { SetValue(UseKeyboardProperty, value); }
		}
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("NavigationOptionsBaseUseMouse"),
#endif
		Category(Categories.Navigation),
		XtraSerializableProperty
		]
		public bool UseMouse {
			get { return (bool)GetValue(UseMouseProperty); }
			set { SetValue(UseMouseProperty, value); }
		}
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("NavigationOptionsBaseUseTouchDevice"),
#endif
		Category(Categories.Navigation),
		XtraSerializableProperty
		]
		public bool UseTouchDevice {
			get { return (bool)GetValue(UseTouchDeviceProperty); }
			set { SetValue(UseTouchDeviceProperty, value); }
		}
		protected override ChartDependencyObject CreateObject() {
			return new NavigationOptionsBase();
		}
	}
	public class NavigationOptions3D : NavigationOptionsBase {
		protected override ChartDependencyObject CreateObject() {
			return new NavigationOptions3D();
		}
	}
	public class NavigationOptions : NavigationOptionsBase {
		public static readonly DependencyProperty UseScrollBarsProperty = DependencyPropertyManager.Register("UseScrollBars",
			typeof(bool), typeof(NavigationOptions), new PropertyMetadata(true, NotifyPropertyChanged));
		public static readonly DependencyProperty AxisXMaxZoomPercentProperty = DependencyPropertyManager.Register("AxisXMaxZoomPercent",
			typeof(double), typeof(NavigationOptions), new PropertyMetadata(10000.0, NotifyPropertyChanged));
		public static readonly DependencyProperty AxisYMaxZoomPercentProperty = DependencyPropertyManager.Register("AxisYMaxZoomPercent",
			typeof(double), typeof(NavigationOptions), new PropertyMetadata(10000.0, NotifyPropertyChanged));
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("NavigationOptionsUseScrollBars"),
#endif
		Category(Categories.Navigation),
		XtraSerializableProperty
		]
		public bool UseScrollBars {
			get { return (bool)GetValue(UseScrollBarsProperty); }
			set { SetValue(UseScrollBarsProperty, value); }
		}
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("NavigationOptionsAxisXMaxZoomPercent"),
#endif
		Category(Categories.Navigation),
		XtraSerializableProperty
		]
		public double AxisXMaxZoomPercent {
			get { return (double)GetValue(AxisXMaxZoomPercentProperty); }
			set { SetValue(AxisXMaxZoomPercentProperty, value); }
		}
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("NavigationOptionsAxisYMaxZoomPercent"),
#endif
		Category(Categories.Navigation),
		XtraSerializableProperty
		]
		public double AxisYMaxZoomPercent {
			get { return (double)GetValue(AxisYMaxZoomPercentProperty); }
			set { SetValue(AxisYMaxZoomPercentProperty, value); }
		}
		protected override ChartDependencyObject CreateObject() {
			return new NavigationOptions();
		}
	}
}
