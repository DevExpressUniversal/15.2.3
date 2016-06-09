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
using DevExpress.Charts.Native;
using DevExpress.Utils.Serializing;
using DevExpress.Xpf.Charts.Native;
using DevExpress.Xpf.Utils;
namespace DevExpress.Xpf.Charts {
	public enum DateTimeFormat {
		ShortDate = DateTimeOptionsFormat.ShortDate,
		LongDate = DateTimeOptionsFormat.LongDate,
		ShortTime = DateTimeOptionsFormat.ShortTime,
		LongTime = DateTimeOptionsFormat.LongTime,
		General = DateTimeOptionsFormat.General,
		MonthAndDay = DateTimeOptionsFormat.MonthAndDay,
		MonthAndYear = DateTimeOptionsFormat.MonthAndYear,
		QuarterAndYear = DateTimeOptionsFormat.QuarterAndYear,
		Custom = DateTimeOptionsFormat.Custom
	}
	public class DateTimeOptions : ChartDependencyObject, IDateTimeOptions {
		public static readonly DependencyProperty FormatProperty = DependencyPropertyManager.Register("Format",
			typeof(DateTimeFormat), typeof(DateTimeOptions), new PropertyMetadata(DateTimeFormat.ShortDate, NotifyPropertyChanged));
		public static readonly DependencyProperty FormatStringProperty = DependencyPropertyManager.Register("FormatString",
			typeof(string), typeof(DateTimeOptions), new PropertyMetadata(String.Empty, NotifyPropertyChanged));
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("DateTimeOptionsFormat"),
#endif
		Category(Categories.Presentation),
		XtraSerializableProperty
		]
		public DateTimeFormat Format {
			get { return (DateTimeFormat)GetValue(FormatProperty); }
			set { SetValue(FormatProperty, value); }
		}
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("DateTimeOptionsFormatString"),
#endif
		Category(Categories.Presentation),
		XtraSerializableProperty
		]
		public string FormatString {
			get { return (string)GetValue(FormatStringProperty); }
			set { SetValue(FormatStringProperty, value); }
		}
		#region IDateTimeOptions implementation
		DateTimeOptionsFormat IDateTimeOptions.Format { get { return (DateTimeOptionsFormat)Format; } }
		string IDateTimeOptions.QuarterFormat { get { return "Q{0}"; } }
		#endregion
		internal void Assign(DateTimeOptions dateTimeOptions) {
			if (dateTimeOptions != null) {
				CopyPropertyValueHelper.CopyPropertyValue(this, dateTimeOptions, FormatProperty);
				CopyPropertyValueHelper.CopyPropertyValue(this, dateTimeOptions, FormatStringProperty);
			}
		}
		protected override ChartDependencyObject CreateObject() {
			return new DateTimeOptions();
		}
	}
}
