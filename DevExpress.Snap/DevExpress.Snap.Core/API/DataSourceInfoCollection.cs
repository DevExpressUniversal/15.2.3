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
using System.ComponentModel;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using DevExpress.Utils;
using DevExpress.XtraReports.Native;
using DevExpress.Snap.Core.Native;
using DevExpress.Office.Utils;
namespace DevExpress.Snap.Core.API {
	public class DataSourceInfoCollection : Collection<DataSourceInfo>, IDataSourceInfoContainer, IDisposable {
		internal const string DefaultDataSourceInfoName = "";
		internal DataSourceInfoCollection(bool createDefaultDataSource) {
			if (createDefaultDataSource)
				SetDefaultDataSource(null);
		}
		public DataSourceInfoCollection()
			: this(true) {
		}
		#region Events
		#region CollectionChanged
		EventHandler collectionChanged;
#if !SL
	[DevExpressSnapCoreLocalizedDescription("DataSourceInfoCollectionCollectionChanged")]
#endif
		public event EventHandler CollectionChanged { add { collectionChanged += value; } remove { collectionChanged = Delegate.Remove(collectionChanged, value) as EventHandler; } }
		protected internal virtual void RaiseCollectionChanged() {
			if (collectionChanged != null)
				collectionChanged(this, EventArgs.Empty);
		}
		#endregion
		#region DataSourceChanged
		DataSourceChangedEventHandler dataSourceChanged;
#if !SL
	[DevExpressSnapCoreLocalizedDescription("DataSourceInfoCollectionDataSourceChanged")]
#endif
		public event DataSourceChangedEventHandler DataSourceChanged { add { dataSourceChanged += value; } remove { dataSourceChanged = Delegate.Remove(dataSourceChanged, value) as DataSourceChangedEventHandler; } }
		protected internal virtual void RaiseNamedDataSourceChanged(DataSourceInfo changedDataSourceInfo, DataSourceChangeType changeType) {
			if (dataSourceChanged != null)
				dataSourceChanged(this, new DataSourceChangedEventArgs(changedDataSourceInfo, changeType));
		}
		#endregion
		#region DataSourceNameChanged
		DataSourceNameChangedEventHandler onDataSourceNameChanged;
		internal event DataSourceNameChangedEventHandler DataSourceNameChanged { add { onDataSourceNameChanged += value; } remove { onDataSourceNameChanged -= value; } }
		void RaiseDataSourceNameChanged(DataSourceInfo changedDataSourceInfo) {
			DataSourceNameChangedEventHandler handler = onDataSourceNameChanged;
			if (handler != null)
				handler(this, new DataSourceNameChangedEventArgs(changedDataSourceInfo));
		}
		#endregion
		#endregion
		#region Properties
#if !SL
	[DevExpressSnapCoreLocalizedDescription("DataSourceInfoCollectionDefaultDataSourceInfo")]
#endif
		public DataSourceInfo DefaultDataSourceInfo {
			get {
				DataSourceInfo result = this[DefaultDataSourceInfoName];
				if (result == null) {
					SetDefaultDataSource(null);
					result = this[DefaultDataSourceInfoName];
				}
				return result;
			}
		}
		public DataSourceInfo this[string dataSourceName] {
			get {
				foreach (DataSourceInfo info in this) {
					if (info.DataSourceName == dataSourceName)
						return info;
				}
				return null;
			}
		}
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1820:TestForEmptyStringsUsingStringLength")]
		[System.CodeDom.Compiler.GeneratedCode("Suppress FxCop check", "")]
		public bool IsDefaultDataSource(object dataSource) {
			DataSourceInfo info = GetInfo(dataSource);
			if (info == null)
				return false;
			return string.Equals(info.DataSourceName, DefaultDataSourceInfoName);
		}
		protected internal string MailMergeDataSourceName { get; set; }
		protected internal bool MailMergeMode { get; set; }
		#endregion
		internal DataSourceInfo SetDefaultDataSource(object dataSource) {
			int index = GetDataSourceInfoIndex(DefaultDataSourceInfoName);
			if (index < 0)
				return Add(DefaultDataSourceInfoName, dataSource);
			DataSourceInfo result = this[index];
			result.DataSource = dataSource;
			return result;
		}
		public DataSourceInfo GetInfo(object dataSource) {
			foreach (DataSourceInfo item in this) {
				if (item.DataSource == dataSource)
					return item;
			}
			return null;
		}
		public CalculatedFieldCollection GetCalculatedFieldsByDataSourceName(string dataSourceName) {
			DataSourceInfo info = this[dataSourceName];
			return info != null ? info.CalculatedFields : null;
		}
		public CalculatedFieldCollection GetCalculatedFieldsByDataSource(object dataSource) {
			DataSourceInfo info = GetInfo(dataSource);
			return info != null ? info.CalculatedFields : null;
		}
		public string GetDataSourceNameByDataSource(object dataSource) {
			DataSourceInfo info = GetInfo(dataSource);
			return info != null ? info.DataSourceName : null;
		}
		public object GetDataSourceByName(string name) {
			DataSourceInfo info = this[name];
			if (info == null)
				return null;
			return info.DataSource;
		}
		public void Remove(string name) {
			if (MailMergeMode && string.Compare(name, MailMergeDataSourceName) == 0)
				Exceptions.ThrowInvalidOperationException(Office.Localization.OfficeStringId.Msg_InvalidRemoveDataSource);
			int index = GetDataSourceInfoIndex(name);
			if (index >= 0)
				RemoveAt(index);
		}
		public DataSourceInfo Add(string name, object dataSource) {
			DataSourceInfo result = new DataSourceInfo(name, dataSource);
			Add(result);
			return result;
		}
		public IList<ICalculatedField> GetCalculatedFields() {
			List<ICalculatedField> result = new List<ICalculatedField>();
			foreach (DataSourceInfo info in this) {
				result.AddRange(info.CalculatedFields);
			}
			return result;
		}
		public IList<object> GetDataSources() {
			List<object> result = new List<object>();
			foreach (DataSourceInfo info in this) {
				if (info.DataSource != null)
					result.Add(info.DataSource);
			}
			return result;
		}
		protected internal void AddWithReplace(DataSourceInfo item) {
			int index = GetDataSourceInfoIndex(item.DataSourceName);
			if (index < 0)
				Add(item);
			else
				this[index] = item;
		}
		protected int GetDataSourceInfoIndex(string name) {
			for (int i = Count - 1; i >= 0; i--)
				if (this[i].DataSourceName == name)
					return i;
			return -1;
		}
		protected override void InsertItem(int index, DataSourceInfo item) {
			int existingItemIndex = GetDataSourceInfoIndex(item.DataSourceName);
			if (existingItemIndex >= 0)
				SnapExceptions.ThrowInvalidOperationException(Localization.SnapStringId.Msg_DataSourceNameExists);
			base.InsertItem(index, item);
			SubscribeDataSourceInfoEvents(item);
			RaiseCollectionChanged();
			item.CalculatedFieldAdded += OnCalculatedFieldAdded;
		}
		protected internal event CalculatedFieldAddedEventHandler CalculatedFieldAdded;
		protected virtual void RaiseCalculatedFieldAdded(CalculatedField field) {
			if (CalculatedFieldAdded != null)
				CalculatedFieldAdded(this, new CalculatedFieldAddedEventArgs(field));
		}
		void OnCalculatedFieldAdded(object sender, CalculatedFieldAddedEventArgs e) {
			RaiseCalculatedFieldAdded(e.Field);
		}
		protected override void ClearItems() {
			foreach (DataSourceInfo info in this)
				UnsubscribeDataSourceInfoEvents(info);
			base.ClearItems();
			RaiseCollectionChanged();
		}
		protected override void RemoveItem(int index) {
			UnsubscribeDataSourceInfoEvents(this[index]);
			base.RemoveItem(index);
			RaiseCollectionChanged();
		}
		protected override void SetItem(int index, DataSourceInfo item) {
			int existingIndex = GetDataSourceInfoIndex(item.DataSourceName);
			if (existingIndex >= 0 && existingIndex != index)
				SnapExceptions.ThrowInvalidOperationException(Localization.SnapStringId.Msg_DataSourceNameExists);
			UnsubscribeDataSourceInfoEvents(this[index]);
			base.SetItem(index, item);
			SubscribeDataSourceInfoEvents(item);
			RaiseCollectionChanged();
		}
		protected virtual void SubscribeDataSourceInfoEvents(DataSourceInfo info) {
			info.DataSourceChanged += OnDataSourceChanged;
			info.DataSourceNameChanged += OnDataSourceNameChanged;
		}
		protected virtual void UnsubscribeDataSourceInfoEvents(DataSourceInfo info) {
			info.DataSourceChanged -= OnDataSourceChanged;
			info.DataSourceNameChanged -= OnDataSourceNameChanged;
		}
		protected virtual void OnDataSourceChanged(object sender, DataSourceChangedEventArgs e) {
			DataSourceInfo info = (DataSourceInfo)sender;
			RaiseNamedDataSourceChanged(info, e.DataSourceChangeType);
		}
		void OnDataSourceNameChanged(object sender, EventArgs e) {
			RaiseDataSourceNameChanged((DataSourceInfo)sender);
		}		
		#region IDisposable
		public void Dispose() {
			Dispose(true);
			GC.SuppressFinalize(this);
		}
		~DataSourceInfoCollection() {
			Dispose(false);
		}
		private void Dispose(bool disposing) {
			if (disposing) {
				collectionChanged = null;
				dataSourceChanged = null;
				Clear();
			}
		}
		#endregion
	}
	public enum DataSourceChangeType { DataSource, CalculatedFields }
	public class DataSourceChangedEventArgs : EventArgs {
		public DataSourceChangedEventArgs(DataSourceInfo changedDataSourceInfo, DataSourceChangeType dataSourceChangeType) {
			ChangedDataSourceInfo = changedDataSourceInfo;
			DataSourceChangeType = dataSourceChangeType;
		}
		public DataSourceInfo ChangedDataSourceInfo { get; private set; }
		public DataSourceChangeType DataSourceChangeType { get; private set; }
		public string Name { get { return ChangedDataSourceInfo.DataSourceName; } }
	}
	public delegate void DataSourceChangedEventHandler(object sender, DataSourceChangedEventArgs e);
	internal delegate void DataSourceNameChangedEventHandler(object sender, DataSourceNameChangedEventArgs e);
	internal class DataSourceNameChangedEventArgs : EventArgs {
		public DataSourceNameChangedEventArgs(DataSourceInfo changedDataSourceInfo) {
			ChangedDataSourceInfo = changedDataSourceInfo;
		}
		public DataSourceInfo ChangedDataSourceInfo { get; private set; }
	}
}
