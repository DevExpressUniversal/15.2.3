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
using DevExpress.Utils;
using System.ComponentModel;
using DevExpress.Compatibility.System.ComponentModel;
namespace DevExpress.XtraEditors.Filtering {
	public interface IFilterParameter {
		string Name { get; }
		Type Type { get; }
	}
}
namespace DevExpress.Data {
	public interface IParameterSupplierBase {
		IEnumerable<IParameter> GetIParameters();
	}
	public interface IParameter : DevExpress.XtraEditors.Filtering.IFilterParameter {
		object Value { get; set; }
	}
	public interface IParametersRenamer {
		void RenameParameters(IDictionary<string, string> renamingMap);
		void RenameParameter(string oldName, string newName);
	}
	public interface IMultiValueParameter : IParameter {
		bool MultiValue { get; set; }
	}
}
namespace DevExpress.XtraReports.Parameters {
	using DevExpress.Data;
	public static class ParameterHelper {
		public static IParameter GetByName(this IEnumerable<IParameter> parameters, string parameterName) {
			foreach(IParameter parameter in parameters)
				if(parameter.Name == parameterName)
					return parameter;
			return null;
		}
		static object CreateDefaultValue(Type type) {
			if(type.IsClass() || type.IsInterface()) {
				return null;
			}
			if(type.IsEnum()) {
				return type.GetValues().GetValue(0);
			}
			return Activator.CreateInstance(type);
		}
		public static object GetDefaultValue(Type type) {
			var typeCode = DXTypeExtensions.GetTypeCode(type);
			switch(typeCode) {
				case TypeCode.String:
					return string.Empty;
				case TypeCode.DateTime:
					return DateTime.MinValue;
				default:
					return CreateDefaultValue(type);
			}
		}
		public static bool ShouldConvertValue(object value, Type type) {
			return value == null || value.GetType() != type;
		}
		public static object ConvertFrom(object value, Type type, object defaultValue) {
			return ConvertFrom(value, type, defaultValue, System.Globalization.CultureInfo.InvariantCulture);
		}
		public static object ConvertFrom(object value, Type type, object defaultValue, System.Globalization.CultureInfo culture) {
			try {
				TypeConverter converter = TypeDescriptor.GetConverter(type);
				if(value != null && converter != null && converter.CanConvertFrom(null, value.GetType()))
					return converter.ConvertFrom(null, culture, value);
			} catch {
			}
			return defaultValue;
		}
		public static string ConvertValueToString(object value) {
			try {
				if(value != null) {
					TypeConverter converter = TypeDescriptor.GetConverter(value.GetType());
					if(converter != null && converter.CanConvertTo(null, typeof(string)))
						return (string)converter.ConvertTo(null, System.Globalization.CultureInfo.InvariantCulture, value, typeof(string));
				}
			} catch {
			}
			return string.Empty;
		}
	}
}
