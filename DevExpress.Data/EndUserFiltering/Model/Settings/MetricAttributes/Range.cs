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

namespace DevExpress.Utils.Filtering.Internal {
	using System;
	using System.Collections.Generic;
	using System.Linq.Expressions;
	partial class MetricAttributes {
		internal static bool IsRange(Type type) {
			switch(Type.GetTypeCode(type)) {
				case TypeCode.Object:
					if(TypeHelper.IsNullable(type))
						return IsRange(Nullable.GetUnderlyingType(type));
					return IsTimeSpan(type); 
				case TypeCode.Empty:
				case TypeCode.DBNull:
				case TypeCode.Boolean:
				case TypeCode.Char:
				case TypeCode.String:
					return false;
				default:
					return true;
			}
		}
		static bool IsDateTimeRange(Type type) {
			return IsDateTime(type) || IsTimeSpan(type);
		}
		static bool IsDateTime(Type type) {
			return (type == typeof(DateTime) || type == typeof(DateTime?)) || IsTimeSpan(type);
		}
		static bool IsTimeSpan(Type type) {
			return type == typeof(TimeSpan) || type == typeof(TimeSpan?);
		}
		static void CheckNumericRange(Type type, ref object min, ref object max) {
			if(!TypeHelper.IsNullable(type)) {
				object defaultValue = Activator.CreateInstance(type);
				min = min ?? defaultValue;
				max = max ?? defaultValue;
			}
		}
		static void CheckDataTimeRange(Type type, ref object min, ref object max) {
			if(min != null && !IsDateTime(min.GetType()))
				min = null;
			if(max != null && !IsDateTime(max.GetType()))
				max = null;
			CheckDataTimeRangeCore(type, ref min, ref max);
		}
		static void CheckDataTimeRangeCore(Type type, ref object min, ref object max) {
			if(!TypeHelper.IsNullable(type)) {
				min = min ?? (IsTimeSpan(type) ? (object)TimeSpan.MinValue : (object)DateTime.MinValue);
				max = max ?? (IsTimeSpan(type) ? (object)TimeSpan.MaxValue : (object)DateTime.MaxValue);
			}
		}
		static DateTimeRangeUIEditorType CheckDateTimeRangeUIEditorType(RangeUIEditorType value) {
			switch(value) {
				case RangeUIEditorType.Spin:
					return DateTimeRangeUIEditorType.Picker;
				case RangeUIEditorType.Range:
					return DateTimeRangeUIEditorType.Range;
				default:
					return DateTimeRangeUIEditorType.Default;
			}
		}
		static IDictionary<Type, Func<object, object, object, string, string, RangeUIEditorType, string[], IMetricAttributes>> rangeInitializers =
			new Dictionary<Type, Func<object, object, object, string, string, RangeUIEditorType, string[], IMetricAttributes>>();
		internal static IMetricAttributes CreateRange(Type type, object min, object max, object avg, string fromName, string toName, RangeUIEditorType editorType, string[] members) {
			Func<object, object, object, string, string, RangeUIEditorType, string[], IMetricAttributes> initializer;
			if(!IsRange(type))
				return null;
			if(IsDateTimeRange(type)) {
				CheckDataTimeRange(type, ref min, ref max);
				return CreateRange(type, min, max, fromName, toName, CheckDateTimeRangeUIEditorType(editorType), members);
			}
			if(!rangeInitializers.TryGetValue(type, out initializer)) {
				bool isNullableType = TypeHelper.IsNullable(type);
				var nullableType = isNullableType ? type : typeof(Nullable<>).MakeGenericType(type);
				var underlyingType = isNullableType ? Nullable.GetUnderlyingType(type) : type;
				var aType = typeof(RangeMetricAttributes<>).MakeGenericType(underlyingType);
				var pMin = Expression.Parameter(typeof(object), "min");
				var pMax = Expression.Parameter(typeof(object), "max");
				var pAvg = Expression.Parameter(typeof(object), "avg");
				var pFromName = Expression.Parameter(typeof(string), "fromName");
				var pToName = Expression.Parameter(typeof(string), "toName");
				var pEditorType = Expression.Parameter(typeof(RangeUIEditorType), "editorType");
				var pMembers = Expression.Parameter(typeof(string[]), "members");
				var ctorExpression = Expression.New(
							aType.GetConstructor(new Type[] { nullableType, nullableType, nullableType, typeof(string), typeof(string), typeof(RangeUIEditorType), typeof(string[]) }),
							Converter.GetConvertNullableExpression(underlyingType, pMin),
							Converter.GetConvertNullableExpression(underlyingType, pMax),
							Converter.GetConvertNullableExpression(underlyingType, pAvg),
							pFromName,
							pToName,
							pEditorType,
							pMembers);
				initializer = Expression.Lambda<Func<object, object, object, string, string, RangeUIEditorType, string[], IMetricAttributes>>(
					ctorExpression, pMin, pMax, pAvg, pFromName, pToName, pEditorType, pMembers).Compile();
				rangeInitializers.Add(type, initializer);
			}
			return initializer(min, max, avg, fromName, toName, editorType, members);
		}
		static IDictionary<Type, Func<object, object, string, string, DateTimeRangeUIEditorType, string[], IMetricAttributes>> dateTimeRangeInitializers =
			new Dictionary<Type, Func<object, object, string, string, DateTimeRangeUIEditorType, string[], IMetricAttributes>>();
		internal static IMetricAttributes CreateRange(Type type, object min, object max, string fromName, string toName, DateTimeRangeUIEditorType editorType, string[] members) {
			Func<object, object, string, string, DateTimeRangeUIEditorType, string[], IMetricAttributes> initializer;
			if(!IsRange(type) || !IsDateTimeRange(type))
				return null;
			if(!dateTimeRangeInitializers.TryGetValue(type, out initializer)) {
				bool isNullableType = TypeHelper.IsNullable(type);
				var nullableType = isNullableType ? type : typeof(Nullable<>).MakeGenericType(type);
				var underlyingType = isNullableType ? Nullable.GetUnderlyingType(type) : type;
				var aType = typeof(RangeMetricAttributes<>).MakeGenericType(underlyingType);
				var pMin = Expression.Parameter(typeof(object), "min");
				var pMax = Expression.Parameter(typeof(object), "max");
				var pFromName = Expression.Parameter(typeof(string), "fromName");
				var pToName = Expression.Parameter(typeof(string), "toName");
				var pEditorType = Expression.Parameter(typeof(DateTimeRangeUIEditorType), "editorType");
				var pMembers = Expression.Parameter(typeof(string[]), "members");
				var ctorExpression = Expression.New(
							aType.GetConstructor(new Type[] { nullableType, nullableType, typeof(string), typeof(string), typeof(DateTimeRangeUIEditorType), typeof(string[]) }),
							Converter.GetConvertNullableExpression(underlyingType, pMin),
							Converter.GetConvertNullableExpression(underlyingType, pMax),
							pFromName,
							pToName,
							pEditorType,
							pMembers);
				initializer = Expression.Lambda<Func<object, object, string, string, DateTimeRangeUIEditorType, string[], IMetricAttributes>>(
					ctorExpression, pMin, pMax, pFromName, pToName, pEditorType, pMembers).Compile();
				dateTimeRangeInitializers.Add(type, initializer);
			}
			return initializer(min, max, fromName, toName, editorType, members);
		}
		class RangeMetricAttributes<T> : MetricAttributes, IRangeMetricAttributes<T> where T : struct {
			readonly MemberNullableValueBox<T> minimum;
			readonly MemberNullableValueBox<T> maximum;
			readonly MemberNullableValueBox<T> average;
			readonly Lazy<RangeUIEditorType> numericRangeUIEditorType;
			readonly Lazy<DateTimeRangeUIEditorType> dateTimeRangeUIEditorType;
			readonly string fromName, toName;
			public RangeMetricAttributes(T? min, T? max, T? avg, string fromName, string toName, RangeUIEditorType editorType, string[] members)
				: this(min, max, avg, fromName, toName, members) {
				this.numericRangeUIEditorType = new Lazy<RangeUIEditorType>(() =>
					{
						if(editorType == Filtering.RangeUIEditorType.Default) {
							if(!minimum.HasMemberOrValue || !maximum.HasMemberOrValue)
								return Filtering.RangeUIEditorType.Text;
						}
						return editorType;
					});
			}
			public RangeMetricAttributes(T? min, T? max, string fromName, string toName, DateTimeRangeUIEditorType editorType, string[] members)
				: this(min, max, null, fromName, toName, members) {
				this.dateTimeRangeUIEditorType = new Lazy<DateTimeRangeUIEditorType>(() => editorType);
			}
			RangeMetricAttributes(T? min, T? max, T? avg, string fromName, string toName, string[] members)
				: base(members) {
				minimum = new MemberNullableValueBox<T>(min, 0, this, () => Minimum);
				maximum = new MemberNullableValueBox<T>(max, 1, this, () => Maximum);
				average = new MemberNullableValueBox<T>(avg, 2, this, () => Average);
				this.fromName = fromName;
				this.toName = toName;
				this.numericRangeUIEditorType = new Lazy<RangeUIEditorType>(() => Filtering.RangeUIEditorType.Default);
				this.dateTimeRangeUIEditorType = new Lazy<DateTimeRangeUIEditorType>(() => Filtering.DateTimeRangeUIEditorType.Default);
			}
			public bool IsNumericRange {
				get { return !IsDateTimeRange(typeof(T)); }
			}
			public bool IsTimeSpanRange {
				get { return IsTimeSpan(typeof(T)); }
			}
			public T? Minimum {
				get { return minimum.Value; }
			}
			public T? Maximum {
				get { return maximum.Value; }
			}
			public T? Average {
				get { return average.Value; }
			}
			public RangeUIEditorType NumericRangeUIEditorType {
				get { return numericRangeUIEditorType.Value; }
			}
			public DateTimeRangeUIEditorType DateTimeRangeUIEditorType {
				get { return dateTimeRangeUIEditorType.Value; }
			}
			public string FromName {
				get { return fromName ?? FilteringLocalizer.GetString(FilteringLocalizerStringId.FromName); }
			}
			public string ToName {
				get { return toName ?? FilteringLocalizer.GetString(FilteringLocalizerStringId.ToName); }
			}
		}
	}
}
