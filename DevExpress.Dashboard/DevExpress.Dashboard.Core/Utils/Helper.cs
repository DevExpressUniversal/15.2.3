#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Dashboard                                                   }
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
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using DevExpress.DashboardCommon.DataProcessing;
using DevExpress.DashboardCommon.ViewerData;
using DevExpress.DashboardCommon.ViewModel;
using DevExpress.Data.Filtering;
using DevExpress.Data.PivotGrid;
using DevExpress.DataAccess.Native;
using DevExpress.Utils;
using DevExpress.XtraPivotGrid.Data;
using DevExpress.Compatibility.System.ComponentModel;
using DevExpress.Compatibility.System.Drawing;
namespace DevExpress.DashboardCommon.Native {
	public static class Helper {
		const bool DefaultBoolean = false;
		const int DefaultInt32 = 0;
		const double DefaultDouble = 0.0;
		static decimal DefaultDecimal = 0m;
		static EnumerableEqualityComparer<object> enumerableObjectComparer;
		static string excludingAllFilterFakeValue = "{B0715AF0-FC7E-4328-950C-01AB90EDEC0C}";
		public static CultureInfo CurrentCulture {
			get {
#if !DXPORTABLE
				return Thread.CurrentThread.CurrentCulture;
#else
				return CultureInfo.CurrentCulture;
#endif
			}
		}
		public static string DefaultCurrencyCultureName { get { return CurrentCulture.Name; } }
		public static string GetUniqueNamePropertyName(string propertyName) {
			return string.Format("{0}_UniqueName", propertyName);
		}
		public static string GetUnknownEnumValueMessage(Enum value) {
			Type enumType = value.GetType();
			return string.Format("'{0}.{1}' value isn't supported", enumType.Name, Enum.GetName(enumType, value));
		}
		public static bool IsPivotGridOthersValue(object value) {
			return object.ReferenceEquals(PivotGridNativeDataSource.OthersValue, value);
		}
		public static bool IsNotNullValue(object value) {
			return !IsNullValue(value);
		}
		public static bool IsNullValue(object value) {
			return value == null || value == DBNull.Value;
		}
		public static bool IsPivotErrorValue(object value) {
			return value is PivotErrorValue;
		}
		public static bool ConvertToBoolean(object value) {
			return ConvertToBoolean(value, false);
		}
		public static int ConvertToInt32(object value) {
			return ConvertToInt32(value, false);
		}
		public static double ConvertToDouble(object value) {
			return ConvertToDouble(value, false);
		}
		public static decimal ConvertToDecimal(object value) {
			return ConvertToDecimal(value, false);
		}
		public static bool ConvertToBoolean(object value, bool throwException) {
			try {
				if(IsNotNullValue(value))
					return Convert.ToBoolean(value);
			} catch(Exception e) {
				if(throwException)
					throw e;
			}
			return DefaultBoolean;
		}
		public static int ConvertToInt32(object value, bool throwException) {
			try {
				if(IsNotNullValue(value))
					return Convert.ToInt32(value);
			} catch(Exception e) {
				if(throwException)
					throw e;
			}
			return DefaultInt32;
		}
		public static double ConvertToDouble(object value, bool throwException) {
			try {
				if(!throwException && DashboardSpecialValues.IsNullValue(value))
					return DefaultDouble;
				if(IsNotNullValue(value))
					return Convert.ToDouble(value);
			} catch(Exception e) {
				if(throwException)
					throw e;
			}
			return DefaultDouble;
		}
		public static decimal ConvertToDecimal(object value, bool throwException) {
			try {
				if(IsNotNullValue(value))
					return Convert.ToDecimal(value);
			} catch(Exception e) {
				if(throwException)
					throw e;
			}
			return DefaultDecimal;
		}
		public static object ConvertToType(object value, DataFieldType type) {
			return ConvertToType(value, type, false);
		}
		public static object ConvertToType(object value, DataFieldType type, bool throwException) {
			switch(type) {
				case DataFieldType.Bool:
					return Helper.ConvertToBoolean(value, throwException);
				case DataFieldType.Decimal:
					return Helper.ConvertToDecimal(value, throwException);
				case DataFieldType.Float:
					return (float)Helper.ConvertToDouble(value, throwException);
				case DataFieldType.Double:
					return Helper.ConvertToDouble(value, throwException);
				case DataFieldType.Integer:
					return Helper.ConvertToInt32(value, throwException);
				default:
					return value;
			}
		}
		public static bool IsNumericType(this DataFieldType fieldType) {
			return fieldType == DataFieldType.Decimal || fieldType == DataFieldType.Float || fieldType == DataFieldType.Double || fieldType == DataFieldType.Integer;
		}
		public static string GetDashboardItemType(DashboardItem dashboardItem) {
			Guard.ArgumentNotNull(dashboardItem, "dashboardItem");
			IEnumerable<DashboardItemTypeAttribute> itemTypeAttributes = dashboardItem.GetType().GetCustomAttributes(true).OfType<DashboardItemTypeAttribute>();
			if(itemTypeAttributes != null && itemTypeAttributes.Count() > 0)
				return itemTypeAttributes.First().TypeName;
			return null;
		}
		public static bool EqualsWithConversion(object value, object value2) {
			object convertedValue2 = value2;
			if(value != null && value2 != null) {
				Type fieldType = value.GetType();
				try {
					if(fieldType.IsEnum()) {
						string value2Str = value2 as string;
						if(value2Str != null)
							convertedValue2 = Enum.Parse(fieldType, value2Str);
					}
					else 
						convertedValue2 = Convert.ChangeType(value2, fieldType);
				}
				catch { }
			}
			return Object.Equals(value, convertedValue2);
		}
		public static bool DataEquals(IList data, IList data2) {
			return DataEquals(data, data2, (value, value2) => { return Object.Equals(value, value2); });
		}
		public static bool DataEqualsWithConversion(IList data, IList data2) {
			return DataEquals(data, data2, (value, value2) => { return EqualsWithConversion(value, value2); });
		}
		static bool DataEquals(IList data, IList data2, Func<object, object, bool> equals) {
			if(data == null || data2 == null)
				return Object.Equals(data, data2);
			if(data.Count != data2.Count)
				return false;
			for(int i = 0; i < data.Count; i++) {
				if(!equals(data[i], data2[i]))
					return false;
			}
			return true;
		}
		public static int GetDataHashCode(IList data) {
			if(data.Count == 0)
				return data.GetHashCode();
			int hashCode = data[0].GetHashCode();
			for(int i = 1; i < data.Count; i++)
				hashCode ^= data[i].GetHashCode();
			return hashCode;
		}
		public static bool IsComponentVSDesignMode(IComponent component) {
			return component != null && component.Site != null && component.Site.DesignMode;
		}
		public static PropertyDescriptor GetProperty(object component, string propertyName) {
			return TypeDescriptor.GetProperties(component)[propertyName];
		}
		public static string CreateDashboardComponentName(Dashboard dashboard, Type componentType) {
#if !DXPORTABLE
			IDesignerHost designerHost = dashboard.GetService<IDesignerHost>();
			INameCreationService nameService = dashboard.GetService<INameCreationService>();
			if(designerHost != null && nameService != null)
				return nameService.CreateName(designerHost.Container, componentType);
#endif
			return null;
		}
		public static CriteriaOperator GetExcludingAllFilterCriteria() {
			return new BinaryOperator(new OperandValue(1), new OperandValue(0), BinaryOperatorType.Equal);
		}
		public static CriteriaOperator GetExcludingAllFilterCriteriaOLAP(string dataMember) {
			return new BinaryOperator(dataMember, excludingAllFilterFakeValue, BinaryOperatorType.Equal);
		}
		public static IEnumerable<T> ShiftLeft<T>(IEnumerable<T> enumerable, int count) {
			if (count != enumerable.Count()) {
				int i = count % enumerable.Count();
				return enumerable.Skip(i).Concat(enumerable.Take(i));
			} else
				return enumerable;
		}
		public static IEnumerable<T> ShiftRight<T>(IEnumerable<T> enumerable, int count) {
			if (count != enumerable.Count()) {
				int i = enumerable.Count() - (count % enumerable.Count());
				return enumerable.Skip(i).Concat(enumerable.Take(i));
			} else
				return enumerable;
		}
		public static IEnumerable<T> Duplicate<T>(IEnumerable<T> enumerable, int times) {
			foreach (T obj in enumerable)
				for (int i = 0; i < times; i++)
					yield return obj;
		}
		public static IEnumerable<T> Duplicate<T>(IEnumerable<T> enumerable, IEnumerable<int> dupCounts) {
			T[] values = enumerable.ToArray();
			int[] dupCountsArray = dupCounts.ToArray();
			for (int i = 0; i < values.Length; i++) {
				int dupCount = i < dupCountsArray.Length ? dupCountsArray[i] : 1;
				for (int j = 0; j < dupCount; j++)
					yield return values[i];
			}
		}
		public static IEnumerable<T> EnsureLength<T>(IEnumerable<T> enumerable, int length) {
			List<T> result = new List<T>();
			T[] array = enumerable.ToArray();
			if (array.Length != 0) {
				for (int i = 0; i < length / array.Length; i++)
					result.AddRange(array);
				for (int i = 0; i < length % array.Length; i++)
					result.Add(array[i]);
			}
			return result;
		}
		public static bool IsEqualEnums<T>(IEnumerable<T> enum1, IEnumerable<T> enum2) {
			return IsEqualEnums(enum1, enum2, EqualityComparer<T>.Default);
		}
		public static bool IsEqualEnums<T>(IEnumerable<T> enum1, IEnumerable<T> enum2, IEqualityComparer<T> elementComparer) {
			using (IEnumerator<T> enumerator1 = enum1.GetEnumerator(), enumerator2 = enum2.GetEnumerator()) {
				bool moveNext1 = true;
				bool moveNext2 = true;
				while (moveNext1 && moveNext2) {
					moveNext1 = enumerator1.MoveNext();
					moveNext2 = enumerator2.MoveNext();
					if (moveNext1 == false && moveNext2 == false)
						return true;
					else if (moveNext1 ^ moveNext2)
						return false;
					else if (moveNext1 == true && moveNext2 == true)
						if (!elementComparer.Equals(enumerator1.Current, enumerator2.Current))
							return false;
				}
			}
			return true;
		}
		public static IEnumerable<T> Append<T>(this IEnumerable<T> enumerable, T value) {
			return enumerable.Union(new T[] { value });
		}
		public static IEnumerable<T> AppendNotNull<T>(this IEnumerable<T> enumerable, T value) {
			if (value != null)
				return enumerable.Append(value);
			else
				return enumerable;
		}
		public static bool SequenceEqualsAsSet<T>(this IEnumerable<T> first, IEnumerable<T> second) {
			return SequenceEqualsAsSet(first, second, null);
		}
		public static bool IsSubsetOf<T>(this IEnumerable<T> subSet, IEnumerable<T> target) {
			return !subSet.Except(target).Any();
		}
		public static bool SequenceEqualsAsSet<T>(this IEnumerable<T> first, IEnumerable<T> second, IEqualityComparer<T> eqComparer) {
			HashSet<T> hashSet = new HashSet<T>(second, eqComparer);
			return hashSet.SetEquals(first);
		}
		public static IEnumerable<T> HashSetExcept<T>(this IEnumerable<T> first, IEnumerable<T> second) {
			HashSet<T> hashSet = new HashSet<T>(second);
			foreach (T row in first)
				if (!hashSet.Contains(row))
					yield return row;
		}
		public static IEnumerable<T> HashSetIntersect<T>(this IEnumerable<T> first, IEnumerable<T> second) {
			HashSet<T> hashSet = new HashSet<T>(second);
			foreach (T row in first)
				if (hashSet.Contains(row))
					yield return row;
		}
		public static int IndexOf<T>(this IEnumerable<T> enumerable, T value) {
			int i = -1;
			bool found = false;
			using(IEnumerator<T> enumerator = enumerable.GetEnumerator()) {
				while(enumerator.MoveNext() && !found) {
					found = EqualityComparer<T>.Default.Equals(enumerator.Current, value);
					i++;
				}
			}
			return found ? i : -1;
		}
		public static void PairwiseForEach<T1, T2>(this IEnumerable<T1> values1, IEnumerable<T2> values2, Action<T1, T2> action) {
			using(var e1 = values1.GetEnumerator()) {
				using(var e2 = values2.GetEnumerator()) {
					bool next1;
					bool next2;
					do {
						next1 = e1.MoveNext();
						next2 = e2.MoveNext();
						if(next1 ^ next2)
							throw new ArgumentException();
						if(next1 && next2)
							action(e1.Current, e2.Current);
						else
							break;
					} while(true);
				}
			}
		}
		public static void AssertStoragesAreEqual(SliceDataQuery query, DataStorage storage1, DataStorage storage2) {
			string msg = "DataStorages differs: ";
			DXContract.Requires(storage1.Count() == storage2.Count(), msg + "Slices count");
			if(query != null) {
				if(query.DataSlices.Count == 0)
					return;
				List<string> topNNames = query.DataSlices.First().DimensionsTopN.Select(s => s.DimensionModel.Name).ToList();
				if(query.Axis1.Skip(1).Any(s => topNNames.Contains(s.Name)))
					return;
				if(query.Axis2.Skip(1).Any(s => topNNames.Contains(s.Name)))
					return;
			}
			foreach(StorageSlice slice1 in storage1) {
				IEnumerable<string> names = slice1.KeyColumns.Select(c => c.Name);
				StorageSlice slice2 = storage2.GetSliceIfExists(names.Select(storage2.GetColumn));
				AssertSlicesAreEqual(query == null ? null : query.DataSlices.Where(s => s.Dimensions.Select(d => d.Name).SequenceEqualsAsSet(slice1.KeyColumns.Select(c => c.Name))).SingleOrDefault(), storage1, slice1, storage2, slice2);
			}
		}
		public static void AssertSlicesAreEqual(SliceModel sliceQuery, DataStorage storage1, StorageSlice slice1, DataStorage storage2, StorageSlice slice2) {
			string msg = "DataStorage slices differs: ";
			DXContract.Requires(slice1.KeyColumns.Count() == slice2.KeyColumns.Count(), msg + "KeyColumns count");
			DXContract.Requires(slice1.KeyColumns.Select(c => c.Name).SequenceEqualsAsSet(slice2.KeyColumns.Select(c => c.Name)), msg + "slice signature");
			List<string> measureNames;
			if(sliceQuery == null) {
				DXContract.Requires(slice1.MeasureColumns.Count() == slice2.MeasureColumns.Count(), msg + "MeasureColumns count");
				measureNames = slice1.MeasureColumns.Select(s => s.Name).ToList();
			} else {
				measureNames = sliceQuery.Measures.Select(m => m.Name).ToList();
				if(slice1.Count() > 0 && slice2.Count() > 0) {
					DXContract.Requires(slice1.MeasureColumns.Where(s => measureNames.Contains(s.Name)).Count() == sliceQuery.Measures.Count);
					DXContract.Requires(slice2.MeasureColumns.Where(s => measureNames.Contains(s.Name)).Count() == sliceQuery.Measures.Count);
				}
			}
			IEnumerable<StorageColumn> measures1 = slice1.MeasureColumns.Where(s => measureNames.Contains(s.Name));
			IEnumerable<StorageColumn> measures2 = slice2.MeasureColumns.Where(s => measureNames.Contains(s.Name));
			IList<StorageColumn> columns1 = slice1.KeyColumns.Concat(measures1).ToList();
			IList<StorageColumn> columns2 = slice2.KeyColumns.Concat(measures2).ToList();
			IList<string> columnNames = columns1.Select(c => c.Name).ToList();
			if(slice1.Count() == 0 && slice2.Count() == 1) {
				DXContract.Requires(slice2.Single().All(v => v.Value.MaterializedValue as string == DashboardSpecialValues.NullValue), "Empty data.");
			} else {
				DXContract.Requires(slice1.Count() == slice2.Count(), "Slice row count is different.");
				if(sliceQuery == null || sliceQuery.DimensionsSort == null || (sliceQuery.DimensionsSort.Count > 0 && !sliceQuery.DimensionsSort.Any(s => s.SortByMeasure != null))) {
					slice1.PairwiseForEach(slice2, (row1, row2) => {
						foreach(string columnName in columnNames) {
							StorageColumn col1 = storage1.GetColumn(columnName);
							StorageColumn col2 = storage2.GetColumn(columnName);
							object val1 = row1[col1].MaterializedValue;
							object val2 = row2[col2].MaterializedValue;
							if(val1 is IEnumerable) {
								IEnumerable<object> enum1 = (val1 as IEnumerable).Cast<object>();
								IEnumerable<object> enum2 = (val2 as IEnumerable).Cast<object>();
								DXContract.Requires(enum1.SequenceEqual(enum2), msg);
							} else
								DXContract.Requires(ValuesAreEqual(val1, val2),
									msg + String.Format("Values. Expected: {0} Actual: {1}", val1, val2));
						}
					});
				} else {
					List<List<string>> nameArray = columnNames.Select((n, i) => new { A = n, B = i }).GroupBy(v => (v.B + 0) / 7).Select((b) => b.Select(e => e.A).ToList()).ToList();
					Type[] tupleTypes = new Type[] { typeof(Tuple<>), typeof(Tuple<,>), typeof(Tuple<,,>), typeof(Tuple<,,,>), typeof(Tuple<,,,,>), typeof(Tuple<,,,,,>), typeof(Tuple<,,,,,,>) };
					Type tupleType = typeof(Tuple<,,,,,,,>);
					Type[] typeArray = new Type[nameArray.Count];
					for(int i = nameArray.Count - 1; i >= 0; i--) {
						if(i == nameArray.Count - 1)
							typeArray[i] = tupleTypes[nameArray[nameArray.Count - 1].Count - 1].MakeGenericType(Enumerable.Repeat(typeof(object), nameArray[nameArray.Count - 1].Count).ToArray());
						else {
							typeArray[i] = tupleType.MakeGenericType(Enumerable.Repeat(typeof(object), 7).Concat(new Type[] { typeArray[i + 1] }).ToArray());
						}
					}
					Dictionary<object, int> dataArray = new Dictionary<object, int>();
					foreach(var row in slice1) {
						object[] valsarr = new object[nameArray.Count];
						for(int i = nameArray.Count - 1; i >= 0; i--) {
							List<string> arr = nameArray[i];
							object[] vals = arr.Select(name => row[storage1.GetColumn(name)].MaterializedValue).ToArray();
							if(i == nameArray.Count - 1)
								valsarr[i] = Activator.CreateInstance(typeArray[i], vals);
							else
								valsarr[i] = Activator.CreateInstance(typeArray[i], vals.Concat(new object[] { valsarr[i + 1] }).ToArray());
						}
						dataArray[valsarr[0]] = 1;
					}
					DXContract.Requires(dataArray.Count == slice1.Count());
					foreach(var row in slice2) {
						object[] valsarr = new object[nameArray.Count];
						for(int i = nameArray.Count - 1; i >= 0; i--) {
							List<string> arr = nameArray[i];
							object[] vals = arr.Select(name => row[storage2.GetColumn(name)].MaterializedValue).ToArray();
							if(i == nameArray.Count - 1)
								valsarr[i] = Activator.CreateInstance(typeArray[i], vals);
							else
								valsarr[i] = Activator.CreateInstance(typeArray[i], vals.Concat(new object[] { valsarr[i + 1] }).ToArray());
						}
						int v0;
						if(!dataArray.TryGetValue(valsarr[0], out v0))
							DXContract.Requires(false, string.Format("slice values different: pivot: {0}, new engine: {1} ", dataArray.First().Key.ToString(), valsarr[0].ToString()));
					}
				}
			}
		}
		public static void CompareUnderlyingDataSet(DashboardUnderlyingDataSet expected, DashboardUnderlyingDataSet actual) {
			DXContract.Requires(expected.RowCount == actual.RowCount);
			List<string> expectedProps = expected.GetColumnNames();
			List<string> actualProps = actual.GetColumnNames();
			DXContract.Requires(expectedProps.Count == actualProps.Count);
			DXContract.Requires(expectedProps.SequenceEqualsAsSet(actualProps));
			Equal(actual.Select(s => GetValues(s, expectedProps)), expected.Select(s => GetValues(s, expectedProps)));
		}
		static object[] GetValues(DashboardDataRow row, List<string> expectedProps) {
			object[] data = new object[expectedProps.Count];
			for(int i = 0; i < expectedProps.Count; i++)
				data[i] = row[expectedProps[i]];
			return data;
		}
		public static bool Equal(IEnumerable<object[]> actualValues, IEnumerable<object[]> expectedValues) {
			Dictionary<ComplexObject, int> actual = new Dictionary<ComplexObject, int>();
			foreach(object[] val in actualValues) {
				ComplexObject key = new ComplexObject(val);
				if(!actual.ContainsKey(key))
					actual.Add(new ComplexObject(val), 1);
				else
					actual[key]++;
			}
			Dictionary<ComplexObject, int> expected = new Dictionary<ComplexObject, int>();
			foreach(object[] val in expectedValues) {
				ComplexObject key = new ComplexObject(val);
				if(!expected.ContainsKey(key))
					expected.Add(new ComplexObject(val), 1);
				else
					expected[key]++;
			}
			foreach(KeyValuePair<ComplexObject, int> pair in expected)
				if(!actual.ContainsKey(pair.Key)) {
					actual.Add(pair.Key, -1);
				} else {
					actual[pair.Key] = actual[pair.Key] - pair.Value;
				}
			if(actual.Any(v => v.Value != 0)) {
				DXContract.Requires(false, "actual: \r\n\r\n " + string.Join(",\r\n", actual.Where(v => v.Value > 0).Select(s => s.Key.ToString())) +
								  " \r\n\r\n expected: \r\n\r\n " + string.Join(",\r\n", actual.Where(v => v.Value < 0).Select(s => s.Key.ToString()))) ;
			}
			return true;
		}
		class ComplexObject {
			object[] values;
			public ComplexObject(object[] values) {
				this.values = values;
			}
			public override bool Equals(object obj) {
				ComplexObject other = obj as ComplexObject;
				if(other == null || other.values.Length != values.Length)
					return false;
				for(int i = 0; i < values.Length; i++)
					if(!object.Equals(values[i], other.values[i]))
						return false;
				return true;
			}
			public override int GetHashCode() {
				int hash = 0;
				for(int i = 0; i < values.Length; i++) {
					object val = values[i];
					if(val != null)
						hash = hash ^ val.GetHashCode();
				}
				return hash;
			}
			public override string ToString() {
				StringBuilder builder = new StringBuilder();
				for(int i = 0; i < values.Length; i++) {
					object val = values[i];
					if(i != 0)
						builder.Append(", ");
					if(val == null)
						builder.Append("null");
					else
						builder.Append("'").Append(val.ToString()).Append("'");
				}
				return builder.ToString();
			}
		}
		static bool ValuesAreEqual(object val1, object val2) {
			if(val1 != null && val2 != null) {
				if(val1.GetType() == typeof(double) && val2.GetType() == typeof(double))
					return ValuesAreEqual(Convert.ToDecimal(val1), Convert.ToDecimal(val2));
				if(val2.GetType() == typeof(decimal) && val2.GetType() == typeof(decimal)) {
					decimal dVal1 = (decimal)val1;
					decimal dVal2 = (decimal)val2;
					return Math.Abs(dVal1 - dVal2) <= 0.000000000001m;
				}
			}
			return object.Equals(val1, val2);
		}
#if DEBUG
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1006")]
#endif
		public static IEnumerable<IEnumerable<T>> Transpose<T>(IEnumerable<IEnumerable<T>> values) {
			IList<IEnumerator<T>> enumerators = values.Select(x => x.GetEnumerator()).ToList();
			if (enumerators.Count > 0) {
				while (true) {
					bool hasNext = enumerators.All(e => e.MoveNext());
					if (hasNext)
						yield return enumerators.Select(e => e.Current);
					else
						yield break;
				}
			}
			foreach (IEnumerator<T> enumerator in enumerators)
				enumerator.Dispose();
			yield break;
		}
#if DEBUG
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1006")]
#endif
		public static bool IsJagged<T>(IEnumerable<IEnumerable<T>> values) {
			IList<IList<T>> input = values.Select(x => (IList<T>)x.ToList()).ToList();
			int inputRows = input.Count();
			if (inputRows == 0)
				return false;
			else {
				var columnCounts = input.Select(x => x.Count).DefaultIfEmpty(0);
				return columnCounts.Max() != columnCounts.Min();
			}
		}
#if DEBUG
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1006")]
#endif
		public static IEnumerable<IEnumerable<object>> MatrixToList(object[,] matrix) {
			IList<IList> result = new List<IList>();
			for (int i = 0; i < matrix.GetLength(0); i++) {
				result.Add(new List<object>());
				for (int j = 0; j < matrix.GetLength(1); j++)
					result.Last().Add(matrix[i, j]);
			}
			return result.Cast<IEnumerable>().Select(x => x.Cast<object>());
		}
		public static bool IsEmpty(this IMasterFilter masterFilter) {
			return masterFilter.Parameters == null
				|| masterFilter.Parameters.Count() == 0
				|| masterFilter.Parameters.Any(p => Helper.IsParametersEmpty(p));
		}
		public static bool IsParametersEmpty(IMasterFilterParameters parameters) {
			return IsParametersValuesEmpty(parameters)
				&& IsParametersRangesEmpty(parameters)
				&& !parameters.IsExcludingAllFilter;
		}
		public static bool IsParametersValuesEmpty(IMasterFilterParameters parameters) {
			return (parameters.Values == null || parameters.Values.Count == 0);
		}
		public static bool IsParametersRangesEmpty(IMasterFilterParameters parameters) {
			return (parameters.Ranges == null || parameters.Ranges.Count == 0);
		}
		public static PrefixNameGenerator GetValuePrefixNameGenerator(string valueString) {
			return new PrefixNameGenerator(String.Format("{0}_", valueString));
		}
		public static IValuesSet GetAllSelectionValues(MultiDimensionalData mddata, IEnumerable<Dimension> dimensions) {
			if(mddata == null || mddata.IsEmpty)
				return ValuesSetHelper.EmptyValuesSet();
			else {
				IList<string> axisNames = new List<string>();
				foreach(Dimension dimension in dimensions) { 
					foreach(string axisName in mddata.GetAxisNames()) {
						if(mddata.GetAxis(axisName).Dimensions.Any(descr => descr.ID == dimension.ActualId) && !axisNames.Contains(axisName)) {
							axisNames.Add(axisName);
						}
					}
				}
				IList<IList<object>> selectionList = new List<IList<object>>();
				IList <object> selection = new List<object>();
				FillAvailableValues(mddata, axisNames, selection, selectionList, dimensions);
				return selectionList.Select(list => list.AsSelectionRow()).AsValuesSet();
			}
		}
		static void FillAvailableValues(MultiDimensionalData data, IEnumerable<string> axes, IList<object> row, IList<IList<object>> selection, IEnumerable<Dimension> dimensions) {
			string axisName = axes.First();
			foreach(AxisPoint axisPoint in data.GetAxisPoints(axisName)) {
				IList<object> newRow = new List<object>(row);
				foreach(AxisPoint pathPoint in axisPoint.RootPath) {
					if (dimensions.Any(d => d.ActualId == pathPoint.Dimension.ID))
						newRow.Add(pathPoint.UniqueValue);
				}
				if(axes.Count() > 1) 
					FillAvailableValues(data, axes.Skip(1), newRow, selection, dimensions);
				else 
					selection.Add(newRow);
			}
		}
		public static IEnumerable<DimensionDefinition> GetUniqueDimensionDefinitions(IEnumerable<Dimension> dimensions) {
			IEnumerable<DimensionDefinition> definitions = GetDimensionDefinitions(dimensions);
			return definitions != null ? definitions.Distinct() : null;
		}
		public static IEnumerable<DimensionDefinition> GetDimensionDefinitions(IEnumerable<Dimension> dimensions) {
			if(dimensions != null)
				return dimensions.Select(dimension => dimension.GetDimensionDefinition());
			return null;
		}
		public static IEnumerable<MeasureDefinition> GetMeasureDefinitions(IEnumerable<Measure> measures) {
			if (measures != null)
				return measures.Select(measure => measure.GetMeasureDefinition());
			return null;
		}
		public static EnumerableEqualityComparer<object> EnumerableObjectComparer {
			get {
				if (enumerableObjectComparer == null)
					enumerableObjectComparer = new EnumerableEqualityComparer<object>();
				return enumerableObjectComparer;
			}
		}
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1045")]
		public static void Swap<T>(ref T a, ref T b) {
			T temp = a;
			a = b;
			b = temp;
		}
		public static IEnumerable<T> ShuffleEnum<T>(IEnumerable<T> enumToShuffle, int shuffleParam) {
			T[] array = enumToShuffle.ToArray();
			if(shuffleParam == 0)
				return array;
			Func<int, int, bool> swap = (x, y) => {
				T val = array[x];
				array[x] = array[y];
				array[y] = val;
				return true;
			};
			IEnumerable<int> swapMap = ShuffleParams(shuffleParam, array.Length).Select(x => x % array.Length);
			foreach(int x in swapMap)
				swap(x, 0);
			return array;
		}
		static IEnumerable<int> ShuffleParams(int param, int count) {
			Func<int, IEnumerable<int>> getPath = (hash) => {
				List<int> array = new List<int>();
				do {
					array.Add(hash % count);
					hash = hash / count;
				} while(hash > count);
				return array;
			};
			Func<int, int> getHash = x => Math.Abs(x.ToString().GetHashCode());
			List<int> result = new List<int>();
			int currHash = param;
			for(int i = 0; i < count / 2; i++) {
				currHash = getHash(currHash);
				result.AddRange(getPath(currHash));
				if(result.Count > count)
					break;
			}
			return result;
		}
		internal static void AssertSummaryAggregationsAreEqual(SliceDataQuery query, IEnumerable<SummaryAggregationResult> actual, IEnumerable<SummaryAggregationResult> expected) {
			DXContract.Requires(actual.Count() == expected.Count());
			if(query != null) {
				if(query.DataSlices.Count == 0)
					return;
				List<string> topNNames = query.DataSlices.First().DimensionsTopN.Select(s => s.DimensionModel.Name).ToList();
				if(query.Axis1.Skip(1).Any(s => topNNames.Contains(s.Name)))
					return;
				if(query.Axis2.Skip(1).Any(s => topNNames.Contains(s.Name)))
					return;
			}
			foreach(SummaryAggregationResult actualAggregation in actual) {
				SummaryAggregationResult founded = expected.Where(f => {
					return f.AggModel == actualAggregation.AggModel && f.Slice.SequenceEqualsAsSet(actualAggregation.Slice);
				}).Single();
				if(founded.Value == null)
					DXContract.Requires(actualAggregation.Value == null);
				else {
					object[] foundedArr = founded.Value as object[];
					if(foundedArr == null) {
						DXContract.Requires(ValuesAreEqual(founded.Value, Convert.ChangeType(actualAggregation.Value, founded.Value.GetType())));
					} else {
						object[] actualArr = actualAggregation.Value as object[];
						DXContract.Requires(foundedArr.Length == actualArr.Length);
						for(int i = 0; i < foundedArr.Length; i++)
							if(foundedArr[i] == null)
								DXContract.Requires(actualArr[i] == null);
							else
								DXContract.Requires(ValuesAreEqual(foundedArr[i], Convert.ChangeType(actualArr[i], foundedArr[i].GetType())));
					}
				}
			}
		}
	}
	public static class DataUtils {
		public static bool AreListsEqual(IList values1, IList values2) {
			if(values1 != null && values2 != null && values1.Count == values2.Count && values2.Count > 0) {
				for(int i = 0; i < values1.Count; i++) {
					if(!Object.Equals(values1[i], values2[i]))
						return false;
				}
				return true;
			}
			return false;
		}
		public static bool ListContains(IList listCollection, IList list) {
			if(listCollection != null) {
				foreach(IList collectionValue in listCollection)
					if(AreListsEqual(collectionValue, list))
						return true;
			}
			return false;
		}
		public static IList<object> GetFullValue(string axisName, IList dimensionValues, IDictionary<string, IList> drillDownState) {
			List<object> valueList = new List<object>();
			if(drillDownState != null && drillDownState.Count > 0) {
				IList drillDownValues;
				if(drillDownState.TryGetValue(axisName, out drillDownValues)) {
					valueList.AddRange(drillDownValues.Cast<object>());
				}
			}
			valueList.AddRange(dimensionValues.Cast<object>());
			return valueList;
		}
		public static IList<object> CheckOlapNullValues(IList<object> values) {
			int i = values.Count - 1;
			while(i >= 0 && DashboardSpecialValues.IsOlapNullValue(values[i])) {
				values.RemoveAt(i);
				i--;
			}
			return values;
		}
	}
	public static class FormatterHelper {
		public static FormatterBase GetFormatter(ValueFormatViewModel viewModel) {
			CultureInfo culture = Helper.CurrentCulture;
			switch(viewModel.DataType) {
				case ValueDataType.DateTime:
					return DateTimeFormatter.CreateInstance(culture, viewModel.DateTimeFormat);
				case ValueDataType.Numeric:
					return NumericFormatter.CreateInstance(culture, viewModel.NumericFormat);
				default:
					return new StringFormatter();
			}
		}
	}
#if !DXPORTABLE
	internal static class AsmHelper {
			public static Type GetType(Assembly asm, string typeName) {
				if(typeName.Contains(',')) {
					return GetAssemblyType(typeName, null);
				}
				if(asm != null) {
					try {
						Type obj = GetAssemblyType(typeName, asm);
						if(obj != null)
							return obj;
						AssemblyName[] asmRefs = asm.GetReferencedAssemblies();
						foreach(AssemblyName asmRef in asmRefs) {
							obj = GetAssemblyType(typeName, Assembly.Load(asmRef));
							if(obj != null)
								return obj;
						}
					} catch {
					}
				}
				return FindInDomainAndCreate(typeName);
			}
			static Type FindInDomainAndCreate(string typeName) {
				return AppDomain.CurrentDomain
					.GetAssemblies()
					.Select(x => GetAssemblyType(typeName, x))
					.FirstOrDefault(x => x != null);
			}
			static Type GetAssemblyType(string typeName, Assembly asm) {
				try {
					if(asm != null && !typeName.Contains(","))
						return Type.GetType(typeName + "," + asm.FullName, false, true);
					else
						return Type.GetType(typeName, false, true);
				} catch { }
				return null;
			}
		}
#endif
#if !DXPORTABLE // TODO mode to "Viewer/Export"
	public static class DashboardStringHelper {
		public static int GetFontSizeByLineHeight(string fontName, int expectedLineHeight) {
			int baseFontSize = 10;
			Font baseFont = new Font(fontName, baseFontSize);
			double fontHeightCoef = (double)baseFont.Height / baseFontSize;
			baseFont.Dispose();
			return (int)Math.Floor((double)expectedLineHeight / fontHeightCoef);
		}
	}
#endif
	public class EnumerableEqualityComparer<TElement> : IEqualityComparer<IEnumerable<TElement>> {
		IEqualityComparer<TElement> elementComparer;
		public EnumerableEqualityComparer() : this(EqualityComparer<TElement>.Default) { }
		public EnumerableEqualityComparer(IEqualityComparer<TElement> elementComparer) {
			this.elementComparer = elementComparer;
		}
		public bool Equals(IEnumerable<TElement> x, IEnumerable<TElement> y) {
			return x.SequenceEqual(y);
		}
		public int GetHashCode(IEnumerable<TElement> obj) {
			using (IEnumerator<TElement> enumerator = obj.GetEnumerator()) {
				unchecked {
					int hash = 0;
					while (enumerator.MoveNext())
						hash = hash ^ elementComparer.GetHashCode(enumerator.Current);
					return hash;
				}
			}
		}
	}
	public static class EnumExtensions {
		public static DevExpress.Data.SummaryItemTypeEx ToSummaryItemTypeEx(this GridColumnTotalType summaryType) {
			switch (summaryType) {
				case GridColumnTotalType.Avg:
					return DevExpress.Data.SummaryItemTypeEx.Average;
				case GridColumnTotalType.Count:
					return DevExpress.Data.SummaryItemTypeEx.Count;
				case GridColumnTotalType.Max:
					return DevExpress.Data.SummaryItemTypeEx.Max;
				case GridColumnTotalType.Min:
					return DevExpress.Data.SummaryItemTypeEx.Min;
				case GridColumnTotalType.Sum:
					return DevExpress.Data.SummaryItemTypeEx.Sum;
				case GridColumnTotalType.Auto:
				default:
					throw new ArgumentException(summaryType.ToString());
			}
		}
	}
}
