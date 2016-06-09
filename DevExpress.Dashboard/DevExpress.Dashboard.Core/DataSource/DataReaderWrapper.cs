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
using DevExpress.Xpo.DB;
using DevExpress.PivotGrid.ServerMode;
using System.Linq;
using DevExpress.DataAccess.Native;
namespace DevExpress.DashboardCommon.Native {
	class DataReaderWrapperEx : DataReaderWrapperBase {
		SelectedDataEx data;
		public DataReaderWrapperEx(SelectedDataEx data, int aggregatesIndex, int maxCount)
			: base(aggregatesIndex >= 0 ? data.Schema.Skip(aggregatesIndex).Select((i) => i.Type).ToArray() : null, maxCount) {
			this.data = data;
			HasData = data != null;
			if(data != null)
				DataLength = data.Lists[0].Count;
		}
		protected override object[] GetValues(int counter) {
			object[] values = new object[data.Lists.Length];
			for(int i = 0; i < data.Lists.Length; i++)
				values[i] = data.Lists[i][counter];
			return values;
		}
	}
	class DataReaderWrapper : DataReaderWrapperBase {
		readonly SelectStatementResultRow[] data;
		public DataReaderWrapper(SelectedData data, bool metadataIncluded, Type[] aggregatesSchema, int maxCount)
			: base(aggregatesSchema, maxCount) {
			SelectStatementResult dataResult = metadataIncluded ? data.ResultSet[1] : data.ResultSet[0];
			this.data = dataResult.Rows;
			HasData = data != null;
			DataLength = this.data.Length;
			Metadata = metadataIncluded ? BuildMetadata(data.ResultSet[0]) : null;
		}
		Type[] BuildMetadata(SelectStatementResult metadataResult) {
			Type[] result = new Type[metadataResult.Rows.Length];
			int i = 0;
			foreach(SelectStatementResultRow schemaRow in metadataResult.Rows) {
				string name = (string)schemaRow.Values[0];
				DBColumnType type = (DBColumnType)Enum.Parse(typeof(DBColumnType), (string)schemaRow.Values[2]);
				result[i++] = DBColumn.GetType(type);
			}
			return result;
		}
		protected override object[] GetValues(int counter) {
			return data[counter].Values;
		}
	}
	abstract class DataReaderWrapperBase : IPivotQueryResult, IEnumerable<object[]>, IEnumerator<object[]> {
		readonly Type[] aggregatesSchema;
		object[] values = null;
		int counter = -1;
		int maxCount;
		protected int DataLength { get; set; }
		protected int Count { get { return maxCount > 0 && maxCount < DataLength ? maxCount : DataLength; } }
		public Type[] Metadata { get; protected set; }
		public bool HasData { get; protected set; }
		protected DataReaderWrapperBase(Type[] aggregatesSchema, int maxCount) {
			this.maxCount = maxCount;
			this.aggregatesSchema = aggregatesSchema;
		}
		protected abstract object[] GetValues(int counter);
		IEnumerator<object[]> IEnumerable<object[]>.GetEnumerator() {
			return this;
		}
		IEnumerator IEnumerable.GetEnumerator() {
			return this;
		}
		object[] IEnumerator<object[]>.Current {
			get { return values; }
		}
		void IDisposable.Dispose() {
		}
		object IEnumerator.Current {
			get { return values; }
		}
		bool IEnumerator.MoveNext() {
			bool result = ++counter < Count;
			if(result)
				values = GetValues(counter);
			return result;
		}
		void IEnumerator.Reset() {
			counter = -1;
		}
		Type[] IPivotQueryResult.AggregatesSchema {
			get { return aggregatesSchema; }
		}
		IEnumerable<object[]> IPivotQueryResult.Data {
			get { return this; }
		}
	}
}
