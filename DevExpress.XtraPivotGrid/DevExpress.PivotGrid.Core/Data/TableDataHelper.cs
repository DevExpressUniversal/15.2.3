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
using System.Collections;
using DevExpress.Data;
using DevExpress.Data.Helpers;
using DevExpress.Data.Storage;
using System.IO;
using DevExpress.Data.Access;
using System.Collections.Generic;
using System.Globalization;
using DevExpress.Compatibility.System.ComponentModel;
#if !SL
using System.Data;
using System.IO.Compression;
using System.ComponentModel;
#else
using DevExpress.Xpf.Collections;
using DevExpress.Data.Browsing;
#endif
namespace DevExpress.Data.PivotGrid {
	public enum PivotGridDataControllerState { 
		UndefState, CreatingStorage, FilteringRows, SummaryExpressionCalculating 
	};
	public class PivotGridDataHelper : BaseDataControllerHelper {
		protected BaseDataControllerHelper fSource;
		protected DataStorageObjectComparer[] fStorage;
		protected int listRowCount;
		PivotGridDataControllerState state;
		bool[] delayedColumns;
		public PivotGridDataHelper(BaseDataControllerHelper source, DataControllerBase controller) : base(controller) {
			this.fSource = source;
			this.fStorage = null;
			this.listRowCount = 0;
			this.state = PivotGridDataControllerState.UndefState;
		}
		public override void Dispose() {
			base.Dispose();
			UnsubscribeEventsCore();
			ClearStorage();
			fStorage = null;
			fSource = null;
			delayedColumns = null;
		}
		public BaseDataControllerHelper Source { get { return fSource; } }
		public DataStorageObjectComparer[] Storage { get { return fStorage; } }
		protected PivotDataController PivotController { get { return Controller as PivotDataController; } }
		public PivotGridDataControllerState State { 
			get { return state; } 
			set {
				if(state == value) return;
				if(value != PivotGridDataControllerState.UndefState && state != PivotGridDataControllerState.UndefState)
					throw new Exception("can't switch data controller state");
				state = value;
			} 
		}
		public override IList<DataColumnInfo> RePopulateColumns() {
			IList<DataColumnInfo> columns = Source.RePopulateColumns();
			return columns;
		}
		public override void PopulateColumns() { 
			Source.PopulateColumns();
			this.delayedColumns = new bool[Columns.Count];
			PivotController.ClientPopulateColumns();
		}
		public override int Count { get { return listRowCount; } }
		protected override Delegate GetGetRowValueCore(DataColumnInfo column, Type expectedReturnType) {
			throw new NotSupportedException();
		}
		public override object GetRowValue(int listSourceRow, int column, OperationCompleted completed) {
			if(Storage == null || Storage[column] == null) {
				if(State == PivotGridDataControllerState.UndefState && delayedColumns != null && delayedColumns[column]) {
					delayedColumns[column] = false;
					EnsureStorageIsCreated(column);
					return GetStorageValue(listSourceRow, column);
				}
				object val = Source.GetRowValue(listSourceRow, column);
				if(object.ReferenceEquals(val, UnboundErrorObject.Value))
					val = PivotSummaryValue.ErrorValue;
				return val;
			} else
				return GetStorageValue(listSourceRow, column);
		}
		object GetStorageValue(int listSourceRow, int column) {
			return Storage[column].GetNullableRecordValue(listSourceRow);
		}
		public override object GetRowValueDetail(int listSourceRow, DataColumnInfo detailColumn) {
			object row = GetRow(listSourceRow);
			if(detailColumn.PropertyDescriptor != null || row != null)
				return detailColumn.PropertyDescriptor.GetValue(row);
			return base.GetRowValueDetail(listSourceRow, detailColumn);
		}
		public override object GetRow(int listSourceRow, OperationCompleted completed) {
			return Source.GetRow(listSourceRow);
		}
		public override void SetRowValue(int listSourceRow, int column, object val) {
			if(Source == null)
				return;
			Source.SetRowValue(listSourceRow, column, val);
			if(Storage != null && Storage[column] != null)
				Storage[column].SetRecordValue(listSourceRow, val);
		}
		public bool HasNullValue(int column) {
			return Storage != null && Storage[column] != null ? Storage[column].HasNullValue() : false;
		}
		public bool SupportComparerCache(int column) {
			return Storage != null && Storage[column] != null ? Storage[column].SupportComparerCache : false;
		}
		public bool HasComparerCache(int column) {
			return Storage != null && Storage[column] != null ? Storage[column].HasComparerCache : false;
		}
		public void SetComparerCache(int column, int[] cache, bool isAscending) {
			if(Storage == null || Storage[column] == null) return;
			Storage[column].SetComparerCache(cache, isAscending);
		}
		public virtual void RefreshData() {
			ClearStorage();
			CreateStorage();
		}
		public virtual void ClearStorage() {
			if(Storage == null) return;
			for(int i = 0; i < Storage.Length; i++) {
				if(Storage[i] != null)
					Storage[i].ClearStorage();
			}
			this.fStorage = null;
			this.listRowCount = 0;
		}
		protected virtual void CreateStorage() {
			this.listRowCount = Source.Count;
			if(Count == 0) return;
			VisibleListSourceRowCollection visibleRowCollection = new VisibleListSourceRowCollection(Controller);
			visibleRowCollection.ClearAndForceNonIdentity();
			this.fStorage = new DataStorageObjectComparer[Source.Columns.Count];
			try {
				State = PivotGridDataControllerState.CreatingStorage;
				for(int i = 0; i < Source.Columns.Count; i++) {
					Storage[i] = CreateColumnStorage(visibleRowCollection, i, false);
				}
			} catch {
				ClearStorage();
#if !SL
				throw;
#endif
			} finally {
				State = PivotGridDataControllerState.UndefState;
			}
		}
		public void EnsureStorageIsCreated(int columnIndex) {
			if(Storage == null || Storage[columnIndex] != null)
				return;
			EnsureStorageIsCreated(new int[] { columnIndex });
		}
		public void EnsureStorageIsCreated(DataColumnSortInfoCollection sortInfo) {
			if(IsStorageCreated(sortInfo)) return;
			List<int> columns = new List<int>(sortInfo.Count);
			foreach(DataColumnSortInfo si in sortInfo) {
				columns.Add(si.ColumnInfo.Index);
			}
			EnsureStorageIsCreated(columns);
		}
		public void EnsureStorageIsCreated(IList<int> columns) {
			if(IsStorageCreated(columns)) return;
			try {
				State = PivotGridDataControllerState.CreatingStorage;
				VisibleListSourceRowCollection visibleRowCollection = new VisibleListSourceRowCollection(Controller);
				visibleRowCollection.ClearAndForceNonIdentity();
				foreach(int columnIndex in columns) {
					if(Storage[columnIndex] != null && !Storage[columnIndex].IsStorageEmpty)
						continue;
					Storage[columnIndex] = CreateColumnStorage(visibleRowCollection, columnIndex, true);
				}
			} finally {
				State = PivotGridDataControllerState.UndefState;
			}
		}
		bool IsStorageCreated(DataColumnSortInfoCollection sortInfo) {
			for(int i = 0; i < sortInfo.Count; i++) {
				if(Storage[sortInfo[i].ColumnInfo.Index] == null)
					return false;
			}
			return true;
		}
		bool IsStorageCreated(IList<int> columns) {
			for(int i = 0; i < columns.Count; i++) {
				if(Storage[columns[i]] == null)
					return false;
			}
			return true;
		}
		protected virtual DataStorageObjectComparer CreateColumnStorage(VisibleListSourceRowCollection visibleRowCollection,
					int column, bool force) {
			DataColumnInfo columnInfo = Source.Columns[column];
			if(!force && PivotController.IsFilterAreaField(columnInfo)) {
				this.delayedColumns[column] = true;
				return null;
			}
			if(force || PivotController.HasCorrespondingField(columnInfo)) {
				return CreateColumnStorageCore(visibleRowCollection, column, columnInfo);
			} else 
				return null;
		}
		DataStorageObjectComparer CreateColumnStorageCore(VisibleListSourceRowCollection visibleRowCollection, int column, DataColumnInfo columnInfo) {
			DataStorageObjectComparer columnStorage = columnInfo.GetStorageComparer();
			columnInfo.CustomComparer = PivotController.StorageObjectComparer;
			if(columnInfo.Unbound) {
				columnStorage.ErrorObject = PivotSummaryValue.ErrorValue;
				columnStorage.TreatErrorAsNull = false;
			}
			columnStorage.CreateStorage(visibleRowCollection, this, column);
			return columnStorage;
		}
		public void UpdateStorage() {
			if(Count == 0) return;
			if(Storage.Length != Source.Columns.Count)
				throw new ArgumentException("Invalid columns count");			
			VisibleListSourceRowCollection visibleRowCollection = null;
			for(int i = 0; i < Source.Columns.Count; i++) {
				if(Storage[i] != null) continue;
				if(visibleRowCollection == null) {
					visibleRowCollection = new VisibleListSourceRowCollection(Controller);
					visibleRowCollection.ClearAndForceNonIdentity();
				}
				Storage[i] = CreateColumnStorage(visibleRowCollection, i, false);
			}
		}
		void CreateAllBoundStorages() {
			if(Count == 0) return;
			if(Storage.Length != Source.Columns.Count)
				throw new ArgumentException("Invalid columns count");
			VisibleListSourceRowCollection visibleRowCollection = null;
			for(int i = 0; i < Source.Columns.Count; i++) {
				if(Storage[i] != null || Source.Columns[i].Unbound) continue;
				if(visibleRowCollection == null) {
					visibleRowCollection = new VisibleListSourceRowCollection(Controller);
					visibleRowCollection.ClearAndForceNonIdentity();
				}
				Storage[i] = CreateColumnStorage(visibleRowCollection, i, true);
			}
		}
#if !SL
		public void SaveToStream(Stream stream, bool compress) {			
			BinaryWriter writer = new BinaryWriter(stream);
			writer.Write(PivotDataController.DataStreamSign);
			writer.Write(compress ? 2 : 1);
			long startPosition = stream.Position;
			writer.Write(0L);
			if(compress) {
				using(DeflateStream compressStream = new DeflateStream(stream, CompressionMode.Compress, true)) 
					SaveToStreamCore(compressStream);
			} else 
				SaveToStreamCore(stream);
			long endPosition = stream.Position;
			stream.Position = startPosition;
			writer.Write(endPosition);
			stream.Position = endPosition;
		}
		void SaveToStreamCore(Stream stream) {
			CreateAllBoundStorages();
			BinaryWriter writer = new BinaryWriter(stream);
			int storageLength = Storage == null ? 0 : Storage.Length;
			writer.Write(storageLength);
			writer.Write(Count);
			for(int i = 0; i < storageLength; i++) {
				if(Storage[i] != null && !Source.Columns[i].Unbound) {
					Storage[i].CustomObjectConverter = PivotController.CustomObjectConverter;
					Storage[i].SaveToStream(stream);
				} else {
					writer.Write(DataStorageObjectComparer.NullStreamSign);
				}
			}
		}	
#endif
	}
	public class PivotBaseDataControllerHelper : BaseDataControllerHelper {
		public PivotBaseDataControllerHelper(DataControllerBase controller)
			: base(controller) {
		}
		protected override UnboundPropertyDescriptor CreateUnboundPropertyDescriptor(UnboundColumnInfo info) {
			return new SafeUnboundPropertyDescriptor(Controller, info);
		}
		protected override DataColumnInfo CreateDataColumn(PropertyDescriptor descriptor) {
			return new LightDataColumnInfo(descriptor, !((PivotDataController)Controller).CaseSensitive);
		}
	}
#if !SL && !DXPORTABLE
	public class PivotDataViewDataControllerHelper : DataViewDataControllerHelper {
		public PivotDataViewDataControllerHelper(DataControllerBase controller)
			: base(controller) {
		}
		protected override UnboundPropertyDescriptor CreateUnboundPropertyDescriptor(UnboundColumnInfo info) {
			return new SafeUnboundPropertyDescriptor(Controller, info);
		}
		protected override DataColumnInfo CreateDataColumn(PropertyDescriptor descriptor) {
			return new LightDataColumnInfo(descriptor, !((PivotDataController)Controller).CaseSensitive);
		}
	}
	public class PivotBindingSourceDataControllerHelper : BindingSourceDataControllerHelper {
		public PivotBindingSourceDataControllerHelper(DataControllerBase controller)
			: base(controller) {
		}
		protected override UnboundPropertyDescriptor CreateUnboundPropertyDescriptor(UnboundColumnInfo info) {
			return new SafeUnboundPropertyDescriptor(Controller, info);
		}
		protected override DataColumnInfo CreateDataColumn(PropertyDescriptor descriptor) {
			return new LightDataColumnInfo(descriptor, !((PivotDataController)Controller).CaseSensitive);
		}
	}
#endif
	public class PivotListDataControllerHelper : ListDataControllerHelper {
		public PivotListDataControllerHelper(DataControllerBase controller)
			: base(controller) {
		}
		protected override UnboundPropertyDescriptor CreateUnboundPropertyDescriptor(UnboundColumnInfo info) {
			return new SafeUnboundPropertyDescriptor(Controller, info);
		}
		protected override DataColumnInfo CreateDataColumn(PropertyDescriptor descriptor) {
			return new LightDataColumnInfo(descriptor, !((PivotDataController)Controller).CaseSensitive);
		}
	}
	public class SafeUnboundPropertyDescriptor : UnboundPropertyDescriptor {
		int inEvaluatorGet;
		bool invalidExpression;
		public SafeUnboundPropertyDescriptor(DataControllerBase controller, UnboundColumnInfo unboundInfo)
			: base(controller, unboundInfo) {
		}
		protected override object GetEvaluatorValue(int row) {
			if(this.inEvaluatorGet > 0) 
				return UnboundErrorObject.Value;
			if(this.invalidExpression)
				return UnboundErrorObject.Value;
			object res = null;
			this.inEvaluatorGet++;
			try {
				try {
					res = Evaluator.Evaluate(row);
				} catch(ExpressionException e) {
					if(e.AppliesToAllRows) 
						this.invalidExpression = true;
					return UnboundErrorObject.Value;
				} catch {
					return UnboundErrorObject.Value;
				}
				res = Convert(res);
			} catch {
			} finally {
				this.inEvaluatorGet--;
			}
			return res;
		}
	}
	public class LightDataColumnInfo : DataColumnInfo {
		bool ignoreCase;
		DataStorageObjectComparer storageComparer;
		public DataStorageObjectComparer StorageComparer { get { return storageComparer; } }
		public override IComparer CustomComparer {
			get {
				return base.CustomComparer;
			}
			set {
				base.CustomComparer = value;
				StorageComparer.SetCustomComparer(CustomComparer);
			}
		}
		public LightDataColumnInfo(PropertyDescriptor descriptor, bool ignoreCase)
			: base(descriptor) {
			this.ignoreCase = ignoreCase;
			SetCompareOptions(StorageComparer, ignoreCase);
		}
		protected virtual DataStorageObjectComparer CreateStorageComparer() {
			DataStorageObjectComparer comparer = DataStorageObjectComparer.CreateComparer(Type);  
			comparer.SetCustomComparer(CustomComparer);
			SetCompareOptions(comparer, ignoreCase);
			return comparer;
		}
		protected override void SetPropertyDescriptor(PropertyDescriptor descriptor) {
			base.SetPropertyDescriptor(descriptor);
			this.storageComparer = CreateStorageComparer();
		}
		void SetCompareOptions(DataStorageObjectComparer comparer, bool ignoreCase) {
			if(ignoreCase) {
				DataStorageStrComparer strComparer = comparer as DataStorageStrComparer;
				if(strComparer != null)
					strComparer.CompareOptions = CompareOptions.IgnoreCase;
			}
		}
	}
}
