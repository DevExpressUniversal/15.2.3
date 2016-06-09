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

using DevExpress.DashboardCommon.DataProcessing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
namespace DevExpress.DashboardCommon.DataProcessing.InMemoryDataProcessor {
	public abstract class DataVectorBase {
		int count;
		int maxCount;
		public int Count { get { return count; } set { count = value; } }
		public int MaxCount { get { return maxCount; } protected set { maxCount = value; } }
		public abstract Type DataType { get; }
		public abstract IEnumerable<object> GetUntypedData();
		public static DataVectorBase New(Type dataType, int maxCount) {
			return GenericActivator.New<DataVectorBase>(typeof(DataVector<>), dataType, maxCount);
		}
		public abstract Expression MakeDataArrayAccess();
		public abstract Expression MakeSpecialDataArrayAccess();
		public abstract void CopyValueFrom(int index, DataVectorBase source, int sourceIndex);
		public abstract int GetHashCode(int index);
		public abstract void Extend(int min);
	}
	public enum SpecialDataValue { None, Null, Error, Others }
	public class DataVector<T> : DataVectorBase, IDataVector<T> {
		T[] data;
		SpecialDataValue[] specialData;
		public override Type DataType { get { return typeof(T); } }
		public T[] Data { get { return data; } }
		public SpecialDataValue[] SpecialData { get { return specialData; } }
		public DataVector(int maxCount) {
			this.data = new T[maxCount];
			this.specialData = new SpecialDataValue[maxCount];
			this.Count = 0;
			this.MaxCount = maxCount;
		}
		public void CopyFrom(DataVector<T> vector) {
			CopyFrom(Count, vector, 0, vector.Count);
		}
		public void CopyFrom(DataVector<T> vector, int start, int count) {
			CopyFrom(Count, vector, start, count);
		}
		public void CopyFrom(int position, DataVector<T> source, int sourcePosition, int sourceCount) {			
			int newCount = position + sourceCount;
			if (newCount > MaxCount)
				Extend(newCount);
			Array.Copy(source.data, sourcePosition, data, position, sourceCount);
			Array.Copy(source.specialData, sourcePosition, specialData, position, sourceCount);
			Count = newCount;
		}
		public override void CopyValueFrom(int index, DataVectorBase source, int sourceIndex) {
			DataVector<T> typedSource = (DataVector<T>)source;
			data[index] = typedSource.data[sourceIndex];
			specialData[index] = typedSource.specialData[sourceIndex];
		}
		public override int GetHashCode(int index) {
			return data[index].GetHashCode();
		}
		public override void Extend(int min) {
			int newSize = data.Length * 2;
			if (newSize < min)
				newSize = min;
			if (newSize > int.MaxValue)
				newSize = int.MaxValue;
			T[] newData = new T[newSize];
			SpecialDataValue[] newSpecialData = new SpecialDataValue[newSize];
			Array.Copy(data, newData, data.Length);
			Array.Copy(specialData, newSpecialData, data.Length);			
			BitVector newNullFlags = new BitVector(newSize);
			data = newData;
			specialData = newSpecialData;
			MaxCount = newSize;
		}
		public void Extend() {
			Extend(-1);
		}
		public override IEnumerable<object> GetUntypedData() {
			for(int i = 0; i < Count; i++)
				if(specialData[i] == SpecialDataValue.None)
					yield return data[i];
				else
					yield return specialData[i];
		}
		public override Expression MakeDataArrayAccess() {
			return Expression.Constant(data, typeof(T[]));
		}
		public override Expression MakeSpecialDataArrayAccess() {
			return Expression.Constant(specialData, typeof(SpecialDataValue[]));
		}
		#region IDataVector
		int IDataVector<T>.Count {
			get { return this.Count; }
			set { this.Count = value; }
		}
		T[] IDataVector<T>.Data { get { return data; } }
		SpecialDataValue[] IDataVector<T>.SpecialData { get { return specialData; } }
		#endregion
	}
}
