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
using System.Collections.Generic;
using System.Data;
using System.Linq;
using DevExpress.Compatibility.System.Data;
namespace DevExpress.DashboardCommon.DataProcessing {
	public abstract class TypedColumnBase {
		public string Name { get; private set; }
		public abstract Type DataType { get; }
		protected TypedColumnBase(string name) {
			this.Name = name;
		}
		public abstract bool IsEqual(int index1, int index2);
		public abstract int Compare(int index1, int index2);
		public abstract int CalcHash(int index);
		public abstract object GetUntypedValue(int index);
		public abstract int[] GetSurrogateValuesArray();
	}
	public abstract class TypedColumn<T> : TypedColumnBase {
		public override Type DataType { get { return typeof(T); } }
		protected TypedColumn(string name) : base(name) { }
		public abstract T GetValue(int index);
		public override object GetUntypedValue(int index) {
			return (object)GetValue(index);
		}
	}
	public class IntTypedColumn : TypedColumn<int> {
		int[] data;
		public IntTypedColumn(string name, int rowsCount, IEnumerable<object> columnData)
			: base(name) {
			data = columnData.Cast<int>().ToArray();
		}
		public override bool IsEqual(int index1, int index2) {
			return data[index1] == data[index2];
		}
		public override int CalcHash(int index) {
			return data[index];
		}
		public override int GetValue(int index) {
			return data[index];
		}
		public override int Compare(int index1, int index2) {
			return data[index1] - data[index2];
		}
		public override int[] GetSurrogateValuesArray() {
			return data;
		}
	}
	[CLSCompliant(false)]
	public class UIntTypedColumn : TypedColumn<UInt16> {
		UInt16[] data;
		public UIntTypedColumn(string name, int rowsCount, IEnumerable<object> columnData)
			: base(name) {
			data = columnData.Cast<UInt16>().ToArray();
		}
		public override bool IsEqual(int index1, int index2) {
			return data[index1] == data[index2];
		}
		public override int CalcHash(int index) {
			return data[index].GetHashCode();
		}
		public override UInt16 GetValue(int index) {
			return data[index];
		}
		public override int Compare(int index1, int index2) {
			return data[index1] - data[index2];
		}
		public override int[] GetSurrogateValuesArray() {
			throw new NotSupportedException();
		}
	}
	public class DecimalTypedColumn : TypedColumn<decimal> {
		decimal[] data;
		public DecimalTypedColumn(string name, int rowsCount, IEnumerable<object> columnData)
			: base(name) {
			data = columnData.Cast<decimal>().ToArray();
		}
		public override bool IsEqual(int index1, int index2) {
			return data[index1] == data[index2];
		}
		public override int CalcHash(int index) {
			return data[index].GetHashCode();
		}
		public override decimal GetValue(int index) {
			return data[index];
		}
		public override int Compare(int index1, int index2) {
			decimal v1 = data[index1];
			decimal v2 = data[index2];
			return v1 > v2 ? 1 : (v1 < v2 ? -1 : 0);
		}
		public override int[] GetSurrogateValuesArray() {
			throw new NotSupportedException();
		}
	}
	public class FloatTypedColumn : TypedColumn<float> {
		float[] data;
		public FloatTypedColumn(string name, int rowsCount, IEnumerable<object> columnData)
			: base(name) {
			data = columnData.Cast<float>().ToArray();
		}
		public override bool IsEqual(int index1, int index2) {
			return data[index1] == data[index2];
		}
		public override int CalcHash(int index) {
			return data[index].GetHashCode();
		}
		public override float GetValue(int index) {
			return data[index];
		}
		public override int Compare(int index1, int index2) {
			double v1 = data[index1];
			double v2 = data[index2];
			return v1 > v2 ? 1 : (v1 < v2 ? -1 : 0);
		}
		public override int[] GetSurrogateValuesArray() {
			throw new NotSupportedException();
		}
	}
	public class DoubleTypedColumn : TypedColumn<double> {
		double[] data;
		public DoubleTypedColumn(string name, int rowsCount, IEnumerable<object> columnData)
			: base(name) {
			data = columnData.Cast<double>().ToArray();
		}
		public override bool IsEqual(int index1, int index2) {
			return data[index1] == data[index2];
		}
		public override int CalcHash(int index) {
			return data[index].GetHashCode();
		}
		public override double GetValue(int index) {
			return data[index];
		}
		public override int Compare(int index1, int index2) {
			double v1 = data[index1];
			double v2 = data[index2];
			return v1 > v2 ? 1 : (v1 < v2 ? -1 : 0);
		}
		public override int[] GetSurrogateValuesArray() {
			throw new NotSupportedException();
		}
	}
	[CLSCompliant(false)]
	public class ByteTypedColumn : TypedColumn<byte> {
		byte[] data;
		public ByteTypedColumn(string name, int rowsCount, IEnumerable<object> columnData)
			: base(name) {
			data = columnData.Cast<byte>().ToArray();
		}
		public override bool IsEqual(int index1, int index2) {
			return data[index1] == data[index2];
		}
		public override int CalcHash(int index) {
			return data[index].GetHashCode();
		}
		public override byte GetValue(int index) {
			return data[index];
		}
		public override int Compare(int index1, int index2) {
			return data[index1] - data[index2];
		}
		public override int[] GetSurrogateValuesArray() {
			throw new NotSupportedException();
		}
	}
	class StringTypedColumnHeavyPP : TypedColumn<string> {
		InternatedColumnValues<string> storage;
		public StringTypedColumnHeavyPP(string name, int rowsCount, IEnumerable<object> columnData)
			: base(name) {
			storage = new InternatedColumnValues<string>(columnData.Cast<string>(), rowsCount, StringComparer.CurrentCulture);
		}
		public override bool IsEqual(int index1, int index2) {
			return storage.IsEqual(index1, index2);
		}
		public override int CalcHash(int index) {
			return storage.CalcHash(index);
		}
		public override string GetValue(int index) {
			return storage.GetValue(index);
		}
		public override int Compare(int index1, int index2) {
			return storage.Compare(index1, index2);
		}
		public override int[] GetSurrogateValuesArray() {
			return storage.GetSurrogateValuesArray();
		}
	}
	class StringTypedColumnLightPP : TypedColumn<string> {
		int[] data;
		int lastIndex = -1;
		Dictionary<int, string> stringStorage = new Dictionary<int, string>();
		Dictionary<string, int> indexStorage = new Dictionary<string, int>(StringComparer.Ordinal);
		public StringTypedColumnLightPP(string name, int rowsCount, IEnumerable<object> columnData)
			: base(name) {
			data = new int[rowsCount];
			int i = 0;
			foreach (object value in columnData) {
				string str = (string)value;
				int strIndex;
				if (!indexStorage.TryGetValue(str, out strIndex)) {
					strIndex = NextIndex();
					indexStorage.Add(str, strIndex);
					stringStorage.Add(strIndex, str);
				}
				data[i] = strIndex;
				i++;
			}
		}
		int NextIndex() {
			lastIndex++;
			return lastIndex;
		}
		public override bool IsEqual(int index1, int index2) {
			return data[index1] == data[index2];
		}
		public override int CalcHash(int index) {
			return data[index];
		}
		public override string GetValue(int index) {
			return stringStorage[data[index]];
		}
		public override int Compare(int index1, int index2) {
			return String.Compare(GetValue(index1), GetValue(index2));
		}
		public override int[] GetSurrogateValuesArray() {
			return data;
		}
	}
	public class BoolTypedColumn : TypedColumn<bool> {
		bool[] data;
		public BoolTypedColumn(string name, int rowsCount, IEnumerable<object> columnData)
			: base(name) {
			data = columnData.Cast<bool>().ToArray();
		}
		public override bool IsEqual(int index1, int index2) {
			return data[index1] == data[index2];
		}
		public override int CalcHash(int index) {
			return data[index].GetHashCode();
		}
		public override bool GetValue(int index) {
			return data[index];
		}
		public override int Compare(int index1, int index2) {
			if (!data[index1] && data[index2])
				return -1;
			if (data[index1] == data[index2])
				return 0;
			return 1;
		}
		public override int[] GetSurrogateValuesArray() {
			throw new NotSupportedException();
		}
	}
	class InternatedColumnValues<T> {
		Dictionary<T, int> distinctValues = new Dictionary<T, int>();
		int[] data;
		T[] valueStorage;
		public T this[int index] { get { return default(T); } }
		public InternatedColumnValues(IEnumerable<T> initialValues, int count, IComparer<T> comparer) {
			List<T> sortedSet = new List<T>(new HashSet<T>(initialValues));
			sortedSet.Sort(comparer);
			valueStorage = sortedSet.ToArray();
			int i = 0;
			foreach (T value in sortedSet)
				distinctValues.Add(value, i++);
			data = new int[count];
			i = 0;
			foreach (T value in initialValues)
				data[i++] = InternValue(value);
			distinctValues = null;
		}
		int InternValue(T value) {
			int index;
			if (!distinctValues.TryGetValue(value, out index)) {
				index = distinctValues.Count;
				distinctValues.Add(value, index);
			}
			return index;
		}
		public bool IsEqual(int index1, int index2) {
			return data[index1] == data[index2];
		}
		public int CalcHash(int index) {
			return data[index];
		}
		public T GetValue(int index) {
			return valueStorage[data[index]];
		}
		public int Compare(int index1, int index2) {
			return data[index1] - data[index2];
		}
		public int[] GetSurrogateValuesArray() {
			return data;
		}
	}
}
