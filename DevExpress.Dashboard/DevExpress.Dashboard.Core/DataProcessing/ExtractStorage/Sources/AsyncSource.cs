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

using DevExpress.DashboardCommon.DataProcessing.InMemoryDataProcessor;
using System;
using System.Collections.Generic;
namespace DevExpress.DashboardCommon.DataProcessing {
	public class AsyncSource : IStorage, IDisposable {
		AsyncRequestBroker requestBroker;
		IFileStorage storage;
		IStorageTable dataTable;
		List<TaskLocker> lockers = new List<TaskLocker>();
		public AsyncSource(string fileName) {
			storage = FileStorage.CreateFileStorage(fileName);
			dataTable = storage.GetTable();
			requestBroker = new AsyncRequestBroker(fileName, false);
			((IThreadObject)requestBroker).Start();
		}
		~AsyncSource() {
			Dispose();
		}
		void Dispose() {
			foreach (TaskLocker locker in lockers)
				((IRequestBroker)requestBroker).DestroyTaskLocker(locker);
			((IThreadObject)requestBroker).Stop();
		}
		#region IStorage
		IDataFlow<T> IStorage.OpenDataStream<T>(Query query) {
			IStorageColumn column = dataTable[query.ColumnName];
			if (column == null)
				return null;
			requestBroker.AddDataStream(column);
			TaskLocker locker = ((IRequestBroker)requestBroker).CreateTaskLocker();
			lockers.Add(locker);
			return new AsyncDataFlow<T>(column.Name, locker, column.UniqueCount);
		}
		Type IStorage.GetColumnType(string columnName) {
			return dataTable[columnName].Type;
		}
		List<string> IStorage.Columns {
			get {
				List<String> columns = new List<string>();
				foreach (IStorageColumn column in dataTable.Columns)
					columns.Add(column.Name);
				return columns;
			}
		}
		#endregion IStorage
		#region IDisposable
		void IDisposable.Dispose() {
			Dispose();
		}
		#endregion IDisposable
	}
	public class AsyncDataFlow<T> : IDataFlow<T> {
		TaskLocker locker;
		string columnName;
		int index = 0;
		bool isEnded = false;
		int uniqueCount;
		IDataVector<T> uniqueVector;
		public AsyncDataFlow(string columnName, TaskLocker locker, int uniqueCount) {
			this.columnName = columnName;
			this.locker = locker;
			this.uniqueCount = uniqueCount;
		}
		#region IDataFlow
		string IDataFlow<T>.ColumnName { get { return columnName; } }
		bool IDataFlow<T>.IsEnded { get { return isEnded; } }
		int IDataFlow<T>.NextMaterialized(IDataVector<T> result) {
			ITask task = new TaskForBroker<T>(index, columnName, result, TaskValuesKind.Materialized);
			locker.WaitUntilReady(task);
			if (result.Count != result.Data.Length)
				this.isEnded = true;
			index += result.Count;
			return result.Count;
		}
		int IDataFlow<T>.NextSubstitutes(IDataVector<int> result) {
			ITask task = new TaskForBroker<int>(index, columnName, result, TaskValuesKind.Substitutes);
			locker.WaitUntilReady(task);
			if (result.Count != result.Data.Length)
				this.isEnded = true;
			index += result.Count;
			return result.Count;
		}
		public int Materialize(IDataVector<int> substitutes, IDataVector<T> result) {
			if (uniqueVector == null) {
				uniqueVector = new DataVector<T>(uniqueCount);
				ITask task = new TaskForBroker<T>(0, columnName, uniqueVector, TaskValuesKind.Unique);
				locker.WaitUntilReady(task);
			}
			for (int i = 0; i < substitutes.Count; i++) {
				int index = substitutes.Data[i];
				if (index == 0)
					result.Data[i] = default(T);
				else
					result.Data[i] = uniqueVector.Data[index];
				result.SpecialData[i] = substitutes.SpecialData[i];
			}
			result.Count = substitutes.Count;
			return result.Count;
		}
		#endregion IDataFlow
	}
}
