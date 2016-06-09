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
using System.ComponentModel.Design;
using System.Globalization;
using System.Collections.Generic;
namespace DevExpress.XtraScheduler.Reporting.Design {
	public class ReportRelatedControlConverter : ReferenceConverter {
		IDesignerHost host;
		public ReportRelatedControlConverter(Type type) : base(type) {
		}
		protected override bool IsValueAllowed(ITypeDescriptorContext context, object value) {
			IDesignerHost host = GetHost(context);
			ReportRelatedControlDesigner designer = GetDesigner(host, context.Instance as IComponent);
			if (designer != null) { 
				return designer.IsValueAllowed(context.PropertyDescriptor.Name, value.GetType());
			}
			return base.IsValueAllowed(context, value);
		}
		protected IDesignerHost GetHost(ITypeDescriptorContext context) {
			if (host == null)
				this.host = context.GetService(typeof(IDesignerHost)) as IDesignerHost;
			return host;
		}
		private ReportRelatedControlDesigner GetDesigner(IDesignerHost host, IComponent component) {
			if (host != null && component != null) {
				return host.GetDesigner(component) as ReportRelatedControlDesigner;
			}
			return null;
		}
	}
	public class WeekDaysConverter : DevExpress.Utils.Design.EnumTypeConverter {
		readonly IDictionary<WeekDays, string> shortNames;
		readonly IDictionary<string, WeekDays> weekDays;
		static class WeekDaysShortNames { 
			public const string Sunday = "Sun";
			public const string Monday = "Mon";
			public const string Tuesday = "Tue";
			public const string Wednesday = "Wed";
			public const string Thursday = "Thu";
			public const string Friday = "Fri";
			public const string Saturday = "Sat";
		}
		const char WeekDaysNameSeparator = ',';
		public WeekDaysConverter(Type type) : base(type) {
			this.shortNames = new Dictionary<WeekDays, string>();
			this.weekDays = new Dictionary<string, WeekDays>();
			PopulateDictionaries();
		}
		private void PopulateDictionaries() {
			shortNames.Add(WeekDays.Sunday, WeekDaysShortNames.Sunday);
			shortNames.Add(WeekDays.Monday, WeekDaysShortNames.Monday);
			shortNames.Add(WeekDays.Tuesday, WeekDaysShortNames.Tuesday);
			shortNames.Add(WeekDays.Wednesday, WeekDaysShortNames.Wednesday);
			shortNames.Add(WeekDays.Thursday, WeekDaysShortNames.Thursday);
			shortNames.Add(WeekDays.Friday, WeekDaysShortNames.Friday);
			shortNames.Add(WeekDays.Saturday, WeekDaysShortNames.Saturday);
			weekDays.Add(WeekDaysShortNames.Sunday, WeekDays.Sunday);
			weekDays.Add(WeekDaysShortNames.Monday, WeekDays.Monday);
			weekDays.Add(WeekDaysShortNames.Tuesday, WeekDays.Tuesday);
			weekDays.Add(WeekDaysShortNames.Wednesday, WeekDays.Wednesday);
			weekDays.Add(WeekDaysShortNames.Thursday, WeekDays.Thursday);
			weekDays.Add(WeekDaysShortNames.Friday, WeekDays.Friday);
			weekDays.Add(WeekDaysShortNames.Saturday, WeekDays.Saturday);
		}
		protected WeekDays CalculateFromShortNames(string value) {
			string[] parts = value.Split(WeekDaysNameSeparator);
			WeekDays result = 0;
			for (int i = 0; i < parts.Length; i++) {
				result = result | weekDays[parts[i]];
			}
			return result;
		}
		protected string CreateWeekDaysString(WeekDays val) {
			string result = string.Empty;
			foreach(WeekDays key in shortNames.Keys) {
				if ((key & val) != key)
					continue;
				if (!string.IsNullOrEmpty(result))
					result += WeekDaysNameSeparator;
				result += shortNames[key];
			}
			return result;
		}
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) {
			if(sourceType.Equals(typeof(string)))
				return true;
			return base.CanConvertFrom(context, sourceType);
		}
		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) {
			if (destinationType.Equals(typeof(string)))
				return true;
			return base.CanConvertTo(context, destinationType);
		}
		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value) {
			string stringValue = value as string;
			if (stringValue != null)
				return ConvertStringToValue(stringValue);
			return base.ConvertFrom(context, culture, value);
		}   
		protected object ConvertStringToValue(string value) {
			if (value.Contains(WeekDaysNameSeparator.ToString()))
				return CalculateFromShortNames(value);
			return (WeekDays)Enum.Parse(typeof(WeekDays), value);
		}
		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType) {
			if (destinationType.Equals(typeof(string)))
				return ConvertValueToString(value);
			return base.ConvertTo(context, culture, value, destinationType);
		}
		protected object ConvertValueToString(object value) {
			WeekDays val = (WeekDays)value;
			bool specalName = (val == WeekDays.EveryDay) || (val == WeekDays.WeekendDays) || (val == WeekDays.WorkDays);
			if (!specalName && !shortNames.ContainsKey(val)) {
				return CreateWeekDaysString(val);
			}
			return value.ToString();
		}
	}
}
