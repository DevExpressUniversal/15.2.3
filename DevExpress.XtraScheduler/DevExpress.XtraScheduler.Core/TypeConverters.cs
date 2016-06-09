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
using System.Collections.Specialized;
using System.Globalization;
using DevExpress.XtraScheduler.Native;
using System.ComponentModel;
using DevExpress.Utils.Design;
using System.Collections.Generic;
using System.Reflection;
using System.Collections.ObjectModel;
using DevExpress.XtraScheduler.Internal;
#if SL
using DevExpress.Xpf.ComponentModel;
#endif
namespace DevExpress.XtraScheduler.Design {
	#region ColumnNameConverter<T>
	public abstract class ColumnNameConverter<T> : StringConverter where T : IPersistentObject {
		public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context) {
			StringCollection columnNames = GetColumnNames(context);
			columnNames.Add(String.Empty);
			return new StandardValuesCollection(columnNames);
		}
		public override bool GetStandardValuesSupported(ITypeDescriptorContext context) {
			return true;
		}
		protected internal virtual StringCollection GetColumnNames(ITypeDescriptorContext context) {
			if (context == null)
				return new StringCollection();
			IPersistentObjectStorage<T> objectStorage = GetObjectStorage(context.Instance);
			if (objectStorage == null)
				return new StringCollection();
			return !objectStorage.UnboundMode ? objectStorage.GetColumnNames() : new StringCollection();
		}
		protected internal virtual IPersistentObjectStorage<T> GetObjectStorage(object instance) {
			if (instance == null)
				return null;
			IPersistentObjectStorageProvider<T> provider = instance as IPersistentObjectStorageProvider<T>;
			return (provider != null) ? provider.ObjectStorage : null;
		}
		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType) {
			if (String.IsNullOrEmpty(value as String)) 
				return Design.DesignSR.NoneString;
			return base.ConvertTo(context, culture, value, destinationType);
		}
		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value) {
			string s = value as string;
			if (String.IsNullOrEmpty(s) 
#if (!SL)
				|| String.Compare(s, DesignSR.NoneString, true, CultureInfo.CurrentCulture) == 0)
#else
				|| String.Compare(s, DesignSR.NoneString,CultureInfo.CurrentCulture, CompareOptions.IgnoreCase) == 0)
#endif
				return String.Empty;
			return base.ConvertFrom(context, culture, value);
		}
	}
	#endregion
	#region AppointmentColumnNameConverter
	public class AppointmentColumnNameConverter : ColumnNameConverter<Appointment> {
	}
	#endregion
	#region ResourceColumnNameConverter
	public class ResourceColumnNameConverter : ColumnNameConverter<Resource> {
	}
	#endregion
	#region AppointmentDependencyColumnNameConverter
	public class AppointmentDependencyColumnNameConverter : ColumnNameConverter<AppointmentDependency> {
	}
	   #endregion
	#region TimeZoneIdStringTypeConverter
	public class TimeZoneIdStringTypeConverter : StringConverter {
		static List<string> defaultValues = CreateDefaultValues();
		static List<string> CreateDefaultValues() {
			List<string> result = new List<string>();
			ReadOnlyCollection<TimeZoneInfo> timeZones = TimeZoneInfo.GetSystemTimeZones();
			foreach(TimeZoneInfo timeZone in timeZones)
				result.Add(timeZone.Id);
			return result;
		}
		public override bool GetStandardValuesSupported(ITypeDescriptorContext context) {
			return true;
		}
		public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context) {
			return new StandardValuesCollection(defaultValues);
		}
	}
	#endregion
	#region TimeZoneIdTypeConverter
	public class TimeZoneIdTypeConverter : StringConverter {
		static List<string> defaultValues = CreateDefaultValues();
		static List<string> CreateDefaultValues() {
			List<string> result = new List<String>();
			FieldInfo[] fields = typeof(TimeZoneId).GetFields(BindingFlags.Static | BindingFlags.Public);
			int count = fields.Length;
			for (int i = 0; i < count; i++) {
				FieldInfo field = fields[i];
				if (field.FieldType == typeof(string) && field.IsInitOnly) {
					String defaultValue = (string)field.GetValue(null);
					result.Add(defaultValue);
				}
			}
			return result;
		}
		public override bool GetStandardValuesSupported(ITypeDescriptorContext context) {
			return true;
		}
		public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context) {
			return new StandardValuesCollection(defaultValues);
		}
	}
	#endregion
}
