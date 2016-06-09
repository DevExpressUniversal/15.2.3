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
using System.Linq;
using System.Text;
namespace DevExpress.DashboardCommon.DataProcessing {
	public struct StorageValue : IEquatable<StorageValue> {
		#region internal types
		internal enum BindType { Unbound, MeasureValue, KeyValue };
		internal struct KeyValueBindData {
			public StorageColumn Column { get; set; }
			public int EncodedValue { get; set; }
			public object GetMaterializedValue() {
				return ((IEncoder)Column).Decode(EncodedValue);
			}
		}
		struct MeasureValueBindData {
			public Dictionary<int, object> MeasureColumnsValueList { get; set; }
			public int Index { get; set; }
			public object GetMaterializedValue() {
				object result;
				return MeasureColumnsValueList.TryGetValue(Index, out result) ? result : null;
			}
			public void SetMaterializedValue(object value) {
				if(MeasureColumnsValueList.ContainsKey(Index))
					MeasureColumnsValueList[Index] = value;
				else
					MeasureColumnsValueList.Add(Index, value);
			}
		}
		#endregion
		BindType bindType;
		MeasureValueBindData measureBindData;
		KeyValueBindData keyBindData;
		object unboundMaterializedValue;
		#region for internal DataStorage use
		internal BindType ValueBindType { get { return bindType; } }
		internal KeyValueBindData KeyBindData { get { return keyBindData; } }
		#endregion
		public object MaterializedValue {
			get { return GetMaterializedValue(); }
			set { SetMaterializedValue(value); }
		}
		public override int GetHashCode() {
			if(bindType != BindType.KeyValue)
				throw new NotSupportedException();
			else
				return keyBindData.EncodedValue.GetHashCode();
		}
		public bool Equals(StorageValue other) {
			if(bindType != BindType.KeyValue && bindType != other.bindType)
				throw new NotSupportedException();
			return keyBindData.Column == other.keyBindData.Column && keyBindData.EncodedValue == other.keyBindData.EncodedValue;
		}
		public override string ToString() {
			object value = MaterializedValue;
			return value != null ? value.ToString() : String.Empty;
		}
		object GetMaterializedValue() {
			switch(bindType) {
				case BindType.Unbound:
					return unboundMaterializedValue;
				case BindType.MeasureValue:
					return measureBindData.GetMaterializedValue();
				case BindType.KeyValue:
					return keyBindData.GetMaterializedValue();
			}
			throw new Exception("");
		}
		void SetMaterializedValue(object value) {
			switch(bindType) {
				case BindType.Unbound:
					unboundMaterializedValue = value;
					break;
				case BindType.MeasureValue:
					measureBindData.SetMaterializedValue(value);
					break;
				case BindType.KeyValue:
					throw new InvalidOperationException("Can't change value in bound StorageValue.");
			}
		}
		public static StorageValue CreateUnbound(object materializedValue) {
			StorageValue result = new StorageValue();
			result.bindType = BindType.Unbound;
			result.measureBindData = new MeasureValueBindData();
			result.keyBindData = new KeyValueBindData();
			result.unboundMaterializedValue = materializedValue;
			return result;
		}
		public static StorageValue CreateKeyValue(StorageColumn column, int encodedValue) {
			StorageValue result = new StorageValue();
			result.bindType = BindType.KeyValue;
			result.measureBindData = new MeasureValueBindData();
			result.keyBindData = new KeyValueBindData();
			result.keyBindData.Column = column;
			result.keyBindData.EncodedValue = encodedValue;
			return result;
		}
		public static StorageValue CreateMeasureValue(Dictionary<int, object> measureColumnsValueList, int index) {
			StorageValue result = new StorageValue();
			result.bindType = BindType.MeasureValue;
			result.measureBindData = new MeasureValueBindData();
			result.measureBindData.MeasureColumnsValueList = measureColumnsValueList;
			result.measureBindData.Index = index;
			result.keyBindData = new KeyValueBindData();
			return result;
		}
	}
}
