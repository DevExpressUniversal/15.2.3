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

using DevExpress.Office;
using DevExpress.XtraSpreadsheet.Utils;
using System;
namespace DevExpress.XtraSpreadsheet.Model {
	#region PivotTuple
	public class PivotTuple : IBinaryConvartableData, ICloneable<PivotTuple>, ISupportsCopyFrom<PivotTuple> {
		#region Fields
		internal const int DefaultIndex = -1;
		int itemIndex;
		int fieldIndex = DefaultIndex;
		int hierarchyIndex = DefaultIndex;
		#endregion
		#region Properties
		public int ItemIndex { get { return itemIndex; } set { itemIndex = value; } }
		public int FieldIndex { get { return fieldIndex; } set { fieldIndex = value; } }
		public int HierarchyIndex { get { return hierarchyIndex; } set { hierarchyIndex = value; } }
		public bool HasFieldIndex { get { return fieldIndex != DefaultIndex; } }
		public bool HasHierarchyIndex { get { return hierarchyIndex != DefaultIndex; } } 
		#endregion
		#region IBinaryConvartableData members
		public int Size { get { return sizeof(int) * 3; } }
		public int WriteTo(byte[] array, int position) {
			int oldPosition = position;
			position += WriteIntTo(itemIndex, array, position);
			position += WriteIntTo(fieldIndex, array, position);
			position += WriteIntTo(hierarchyIndex, array, position);
			return position - oldPosition;
		}
		public int InitFromBinary(byte[] array, int position) {
			int intSize = sizeof(int);
			itemIndex = BitConverter.ToInt32(array, position);
			position += intSize;
			fieldIndex = BitConverter.ToInt32(array, position);
			position += intSize;
			hierarchyIndex = BitConverter.ToInt32(array, position);
			return intSize * 3;
		}
		#endregion
		int WriteIntTo(int value, byte[] array, int position) {
			byte[] convertedValue = BitConverter.GetBytes(value);
			convertedValue.CopyTo(array, position);
			return convertedValue.Length;
		}
		public PivotTuple Clone() {
			PivotTuple clone = new PivotTuple();
			clone.CopyFrom(this);
			return clone;
		}
		#region ISupportsCopyFrom<PivotTuple> Members
		public void CopyFrom(PivotTuple value) {
			this.itemIndex = value.itemIndex;
			this.fieldIndex = value.fieldIndex;
			this.hierarchyIndex = value.hierarchyIndex;
		}
		#endregion
	}
	#endregion
	#region PivotTupleCollection
	public class PivotTupleCollection : SimpleCollection<PivotTuple>, IBinaryConvartableData, ICloneable<PivotTupleCollection> {
		#region IBinaryConvartableData members
		public int Size { get { return sizeof(int) + (Count == 0 ? 0 : InnerList[0].Size * Count); } }
		public int WriteTo(byte[] array, int position) {
			int oldPosition = position;
			int count = InnerList.Count;
			position += WriteIntTo(count, array, position);
			for (int i = 0; i < count; i++)
				position += InnerList[i].WriteTo(array, position);
			return position - oldPosition;
		}
		public int InitFromBinary(byte[] array, int position) {
			int oldPosition = position;
			int count = BitConverter.ToInt32(array, position);
			position += sizeof(int);
			for (int i = 0; i < count; i++) {
				PivotTuple tuple = new PivotTuple();
				position += tuple.InitFromBinary(array, position);
				Add(tuple);
			}
			return position - oldPosition;
		}
		#endregion
		int WriteIntTo(int value, byte[] array, int position) {
			byte[] convertedValue = BitConverter.GetBytes(value);
			convertedValue.CopyTo(array, position);
			return convertedValue.Length;
		}
		public PivotTupleCollection Clone() {
			PivotTupleCollection clone = new PivotTupleCollection();
			foreach (PivotTuple tuple in this) {
				clone.Add(tuple.Clone());
			}
			return clone;
		}
	}
	#endregion
}
