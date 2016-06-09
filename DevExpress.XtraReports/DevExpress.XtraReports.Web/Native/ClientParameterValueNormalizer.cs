#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       XtraReports for ASP.NET                                     }
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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using DevExpress.XtraReports.Web.Native.ClientControls.Services;
using DevExpress.XtraReports.Web.Native.ParametersPanel;
namespace DevExpress.XtraReports.Web.Native {
	public class ClientParameterValueNormalizer {
		#region internal classes
		class ConverterParameterArgument {
			public object ClientValue { get; private set; }
			public Type OriginalParameterType { get; private set; }
			public ConverterParameterArgument(object clientValue, Type originalParameterType) {
				ClientValue = clientValue;
				OriginalParameterType = originalParameterType;
			}
		}
		class DelegateClientParameterConverter {
			readonly Predicate<ConverterParameterArgument> canConvert;
			readonly Func<ConverterParameterArgument, object> convert;
			public DelegateClientParameterConverter(Predicate<ConverterParameterArgument> canConvert, Func<ConverterParameterArgument, object> convert) {
				this.canConvert = canConvert;
				this.convert = convert;
			}
			public bool CanConvert(ConverterParameterArgument arg) {
				try {
					return canConvert(arg);
				} catch(Exception e) {
					Logger.Info("Warning - " + e);
					return false;
				}
			}
			public object Convert(ConverterParameterArgument arg) {
				return convert(arg);
			}
		}
		#endregion
		static readonly TypeCode[] validUnderlyingEnumTypeCodes = WebReportParameterHelper.DecimalTypes.Select(Type.GetTypeCode).ToArray();
		static ILoggingService Logger {
			get { return DefaultXRWebLoggingService.Instance; }
		}
		readonly bool strict;
		public ClientParameterValueNormalizer()
			: this(false) {
		}
		public ClientParameterValueNormalizer(bool strict) {
			this.strict = strict;
		}
		readonly DelegateClientParameterConverter[] parametersConverters = {
			new DelegateClientParameterConverter(
				x => x.ClientValue == null || x.OriginalParameterType == null || x.OriginalParameterType == x.ClientValue.GetType(), 
				x => x.ClientValue),
			new DelegateClientParameterConverter(
				x => x.ClientValue is string && x.OriginalParameterType != typeof(string) && HasConverterFromString(x.OriginalParameterType),
				x => TypeDescriptor.GetConverter(x.OriginalParameterType).ConvertFromInvariantString((string)x.ClientValue)),
			new DelegateClientParameterConverter(
				x => x.OriginalParameterType != null && x.OriginalParameterType.IsEnum && IsValidEnumUnderlyingType(x.ClientValue),
				x => Enum.ToObject(x.OriginalParameterType, Convert.ChangeType(x.ClientValue, Enum.GetUnderlyingType(x.OriginalParameterType)))),
			new DelegateClientParameterConverter(
				x => x.OriginalParameterType != null && x.OriginalParameterType.IsEnum && x.ClientValue is string,
				x => Enum.Parse(x.OriginalParameterType, (string)x.ClientValue)),
			new DelegateClientParameterConverter(
				x => x.ClientValue is IConvertible,
				x => Convert.ChangeType(x.ClientValue, x.OriginalParameterType, CultureInfo.InvariantCulture))
		};
#if DEBUGTEST
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
		public object NormalizeSingleValue_TEST<T>(object clientValue) {
			return Normalize(clientValue, typeof(T), multiValue: false);
		}
#endif
		public object NormalizeSafe(object clientValue, Type originalParameterType, bool multiValue) {
			try {
				return Normalize(clientValue, originalParameterType, multiValue);
			} catch(Exception e) {
				Logger.Error(e.ToString());
				return clientValue;
			}
		}
		public object Normalize(object clientValue, Type originalParameterType, bool multiValue) {
			if(multiValue) {
				return NormalizeCollection(clientValue as ICollection, originalParameterType);
			}
			object result;
			if(!TryNormalize(clientValue, originalParameterType, out result)) {
				result = clientValue;
			}
			return result;
		}
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters")]
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1007:UseGenericsWhereAppropriate")]
		protected virtual bool TryNormalize(object clientValue, Type originalParameterType, out object result) {
			result = null;
			var converterArgument = new ConverterParameterArgument(clientValue, originalParameterType);
			IEnumerable<DelegateClientParameterConverter> filteredConverters = parametersConverters.Where(x => x.CanConvert(converterArgument));
			foreach(DelegateClientParameterConverter converter in filteredConverters) {
				try {
					result = converter.Convert(converterArgument);
					return true;
				} catch(Exception e) {
					if(strict || (!ShouldPassException(e) && !ShouldPassException(e.InnerException))) {
						throw;
					}
				}
			}
			return false;
		}
		static bool ShouldPassException(Exception exception) {
			return exception is FormatException
				|| exception is ArgumentException
				|| exception is InvalidCastException;
		}
		Array NormalizeCollection(ICollection collection, Type type) {
			if(collection == null) {
				return null;
			}
			object newValue;
			int i = 0;
			var result = new object[collection.Count];
			foreach(var item in collection) {
				if(!TryNormalize(item, type, out newValue)) {
					newValue = item;
				}
				result[i] = newValue;
				i++;
			}
			return NormalizeArray(result, type);
		}
		static Array NormalizeArray(object[] array, Type type) {
			var hasDifferentTypes = false;
			foreach(var item in array) {
				hasDifferentTypes = item != null && !type.IsAssignableFrom(item.GetType());
				if(hasDifferentTypes) {
					break;
				}
			}
			if(hasDifferentTypes) {
				return array;
			}
			Array typedResult = Array.CreateInstance(type, array.Length);
			array.CopyTo(typedResult, 0);
			return typedResult;
		}
		static bool IsValidEnumUnderlyingType(object value) {
			return Array.IndexOf(validUnderlyingEnumTypeCodes, Convert.GetTypeCode(value)) >= 0;
		}
		static bool HasConverterFromString(Type type) {
			var converter = TypeDescriptor.GetConverter(type);
			return converter != null
				? converter.CanConvertFrom(typeof(string))
				: false;
		}
	}
}
