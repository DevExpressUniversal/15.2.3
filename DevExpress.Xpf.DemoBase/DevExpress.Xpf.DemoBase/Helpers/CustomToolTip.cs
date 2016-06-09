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
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using DevExpress.Xpf.Core;
namespace DevExpress.Xpf.DemoBase.Helpers {
	class CustomToolTipConverter : TypeConverter {
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) {
			if(sourceType == typeof(string))
				return true;
			return base.CanConvertFrom(context, sourceType);
		}
		public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value) {
			string stringValue = value as string;
			if(!string.IsNullOrEmpty(stringValue)) return new CustomToolTip() { StringContent = stringValue };
			return base.ConvertFrom(context, culture, value);
		}
		public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType) {
			if(destinationType == typeof(string))  return ((CustomToolTip)value).Content;
			return base.ConvertTo(context, culture, value, destinationType);
		}
	}
	[TypeConverter(typeof(CustomToolTipConverter)), ContentProperty("Content"), DefaultProperty("Content")]
	class CustomToolTip : ToolTip {
		#region Dependency Properties
		public static readonly DependencyProperty CustomToolTipProperty;
		public static readonly DependencyProperty StringContentProperty;
		static CustomToolTip() {
			CustomToolTipProperty = DependencyProperty.RegisterAttached("ToolTip", typeof(CustomToolTip), typeof(CustomToolTip), new PropertyMetadata(null, RaiseToolTipChanged));
			StringContentProperty = DependencyProperty.RegisterAttached("StringContent", typeof(string), typeof(CustomToolTip), new PropertyMetadata(null));
		}
		#endregion
		public static CustomToolTip GetToolTip(FrameworkElement d) { return (CustomToolTip)d.GetValue(CustomToolTipProperty); }
		public static void SetToolTip(FrameworkElement d, CustomToolTip value) { d.SetValue(CustomToolTipProperty, value); }
		static void RaiseToolTipChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			FrameworkElement element = (FrameworkElement)d;
			ToolTipService.SetToolTip(d, (CustomToolTip)e.NewValue);
		}
		public CustomToolTip() {
			this.SetDefaultStyleKey(typeof(CustomToolTip));
		}
		public string StringContent { get { return (string)GetValue(StringContentProperty); } set { SetValue(StringContentProperty, value); } }
	}
}
