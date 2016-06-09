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
using System.Collections.Generic;
using DevExpress.Office.Utils;
using DevExpress.Snap.Core.API;
using DevExpress.XtraReports.Native;
namespace DevExpress.Snap.Core.Native {
	public interface IDataSourceDispatcher : IDisposable {
		string DefaultDataSourceName { get; }
		void Initialize(SnapDocumentModel documentModel);
		ICollection<object> GetDataSources();
		object GetDataSource(string name);
		object DefaultDataSource { get; }
		DataSourceInfo DefaultDataSourceInfo { get; }
		bool IsDefaultDataSource(object dataSource);
		ICollection<DataSourceInfo> GetInfos();
		DataSourceInfo GetInfo(string dataSourceName);
		DataSourceInfo GetInfo(object dataSource);
		string FindDataSourceName(object dataSource);
		ICollection<ICalculatedField> GetCalculatedFields();
		ICollection<ICalculatedField> GetCalculatedFields(string dataSourceName);
		ICollection<ICalculatedField> GetCalculatedFields(object dataSource);
		ICalculatedField GetCalculatedField(string dataSourceName, string calculatedFieldName);
		void BeginUpdate();
		void EndUpdate();
		void IncRefCount();
		void DecRefCount();
		event EventHandler CollectionChanged;
		event EventHandler DataSourceChanged;
		IDataSourceDispatcher CreateNew();
		bool IsDisposed { get; }
	}
	public abstract class DataSourceDispatcherBase : IDataSourceDispatcher {
		int refCount = 0;
		bool isDisposed = false;
		public string DefaultDataSourceName { get { return string.Empty; } }
		public event EventHandler CollectionChanged;
		public event EventHandler DataSourceChanged;
		public bool IsDisposed { get { return isDisposed; } }
		public bool HasRef { get { return refCount > 0; } }
		public void IncRefCount() {
			refCount++;
		}
		public void DecRefCount() {
			if (refCount == 0)
				Exceptions.ThrowInternalException();
			refCount--;
		}
		protected virtual bool RaiseDataSourceChangedCore() {
			if (DataSourceChanged != null) {
				DataSourceChanged(this, EventArgs.Empty);
				return true;
			}
			return false;
		}
		protected virtual bool RaiseCollectionChangedCore() {
			if (CollectionChanged != null) {
				CollectionChanged(this, EventArgs.Empty);
				return true;
			}
			return false;
		}
		#region IDispose
		public void Dispose() {
			DecRefCount();
			Dispose(true);
			GC.SuppressFinalize(this);
		}
		protected virtual void Dispose(bool disposing) {
			if (disposing && !HasRef) {
				DisposeCore();
				this.isDisposed = true;
			}
		}
		~DataSourceDispatcherBase() {
			Dispose(false);
		}
		protected virtual void DisposeCore() { }
		#endregion
		public abstract void Initialize(SnapDocumentModel documentModel);
		public abstract ICollection<object> GetDataSources();
		public abstract object GetDataSource(string name);
		public abstract object DefaultDataSource { get; }
		public abstract DataSourceInfo DefaultDataSourceInfo { get; }
		public abstract bool IsDefaultDataSource(object dataSource);
		public abstract ICollection<DataSourceInfo> GetInfos();
		public abstract DataSourceInfo GetInfo(string dataSourceName);
		public abstract DataSourceInfo GetInfo(object dataSource);
		public abstract string FindDataSourceName(object dataSource);
		public abstract ICollection<ICalculatedField> GetCalculatedFields();
		public abstract ICollection<ICalculatedField> GetCalculatedFields(string dataSourceName);
		public abstract ICollection<ICalculatedField> GetCalculatedFields(object dataSource);
		public abstract ICalculatedField GetCalculatedField(string dataSourceName, string calculatedFieldName);
		public abstract void BeginUpdate();
		public abstract void EndUpdate();
		public abstract IDataSourceDispatcher CreateNew();
	}
	public abstract class CollectionBasedDataSourceDispatcher : DataSourceDispatcherBase {
		internal const string DefaultDataSourceInfoName = "";
		int updateCnt = 0;
		bool deferredRaiseEvent = false;
		bool deferredRaiseDataSourceChangedEvent;
		bool initialized = false;
		protected void RaiseCollectionChanged() {
			if (updateCnt == 0)
				this.deferredRaiseEvent &= !RaiseCollectionChangedCore();
			else
				deferredRaiseEvent = true;
		}
		protected void RaiseDataSourceChanged() {
			if (updateCnt == 0)
				this.deferredRaiseDataSourceChangedEvent &= !RaiseDataSourceChangedCore();
			else
				this.deferredRaiseDataSourceChangedEvent = true;
		}
		protected virtual void OnCollectionChanged(object sender, EventArgs e) {
			RaiseCollectionChanged();
		}
		protected virtual void OnDataSourceChanged(object sender, DataSourceChangedEventArgs e) {
			RaiseDataSourceChanged();
		}
		#region IDataSourceDispatcher Members
		public override void Initialize(SnapDocumentModel documentModel) {
			if(this.initialized)
				Exceptions.ThrowInternalException();
			this.initialized = true;
		}
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1820:TestForEmptyStringsUsingStringLength")]
		[System.CodeDom.Compiler.GeneratedCode("Suppress FxCop check", "")]
		public override bool IsDefaultDataSource(object dataSource) {
			DataSourceInfo info = GetInfo(dataSource);
			if (info == null)
				return false;
			return string.Equals(info.DataSourceName, DefaultDataSourceInfoName);
		}
		public override DataSourceInfo GetInfo(string dataSourceName) {
			foreach(DataSourceInfo info in GetInfos())
				if(string.Equals(info.DataSourceName, dataSourceName))
					return info;
			return null;
		}
		public override DataSourceInfo GetInfo(object dataSource) {
			foreach(DataSourceInfo info in GetInfos())
				if(object.ReferenceEquals(info.DataSource, dataSource))
					return info;
			return null;
		}
		public override ICollection<ICalculatedField> GetCalculatedFields(object dataSource) {
			string dataSourceName = FindDataSourceName(dataSource);
			if(object.ReferenceEquals(dataSourceName, null))
				return new List<ICalculatedField>(0);
			return GetCalculatedFields(dataSourceName);
		}
		public override void BeginUpdate() {
			updateCnt++;
		}
		public override void EndUpdate() {
			if(updateCnt <= 0)
				Exceptions.ThrowInvalidOperationException("EndUpdate without BeginUpdate");
			if(--updateCnt == 0) {
				if (deferredRaiseEvent)
					RaiseCollectionChangedCore();
				if (this.deferredRaiseDataSourceChangedEvent)
					RaiseDataSourceChangedCore();
			}
		}
		public override object DefaultDataSource { get { return GetDataSource(DefaultDataSourceName); } }
		public override DataSourceInfo DefaultDataSourceInfo { get { return GetInfo(DefaultDataSourceName) ?? new DataSourceInfo(DefaultDataSourceName, null); } }
		#endregion
	}
	public class ControlDataSourceDispatcher : CollectionBasedDataSourceDispatcher {
		ISnapControl snapControl;
		DataSourceInfoCollection documentDataSources;
		DataSourceInfoCollection controlDataSources;
		ICollection<DataSourceInfo> dataSources;
		ICollection<ICalculatedField> calculatedFields;
		internal ControlDataSourceDispatcher(ISnapControl snapControl)
			: this(snapControl.DataSources) {
			this.snapControl = snapControl;
		}
		protected ControlDataSourceDispatcher(DataSourceInfoCollection dataSources) {
			this.controlDataSources = dataSources;
			SubscribeToEvents(this.controlDataSources);
		}
		void SubscribeToEvents(DataSourceInfoCollection dataSources) {
			UnsubscribeFromEvents(dataSources);
			dataSources.CollectionChanged += OnCollectionChanged;
			dataSources.CalculatedFieldAdded += OnCalculatedFieldAdded;
			dataSources.DataSourceChanged += OnDataSourceChanged;
		}
		void UnsubscribeFromEvents(DataSourceInfoCollection dataSources) {
			dataSources.CollectionChanged -= OnCollectionChanged;
			dataSources.CalculatedFieldAdded -= OnCalculatedFieldAdded;
			dataSources.DataSourceChanged -= OnDataSourceChanged;
		}
		void OnCalculatedFieldAdded(object sender, CalculatedFieldAddedEventArgs e) {
			if(e.Field.DataSourceDispatcher == null)
				e.Field.DataSourceDispatcher = this;
		}
		protected override void OnDataSourceChanged(object sender, DataSourceChangedEventArgs e) {
			ResetCache();
			base.OnDataSourceChanged(sender, e);
		}
		protected override void OnCollectionChanged(object sender, EventArgs e) {
			ResetCache();
			base.OnCollectionChanged(sender, e);
		}
		void ResetCache() {
			this.dataSources = null;
			this.calculatedFields = null;
		}
		public override void Initialize(SnapDocumentModel documentModel) {
			base.Initialize(documentModel);
			this.documentDataSources = documentModel.DataSources;
			SubscribeToEvents(this.documentDataSources);
		}
		public override IDataSourceDispatcher CreateNew() {
			return new ControlDataSourceDispatcher(this.snapControl);
		}
		public override ICollection<object> GetDataSources() {
			List<object> result = new List<object>(this.documentDataSources.Count);
			HashSet<string> mask = new HashSet<string>();
			foreach(DataSourceInfo info in this.documentDataSources) {
				object infoDataSource = info.DataSource;
				if(!object.ReferenceEquals(infoDataSource, null)) {
					result.Add(infoDataSource);
					mask.Add(info.DataSourceName);
				}
			}
			foreach(DataSourceInfo info in this.controlDataSources) {
				object infoDataSource = info.DataSource;
				if(!object.ReferenceEquals(infoDataSource, null))
					if(!mask.Contains(info.DataSourceName))
						result.Add(infoDataSource);
			}
			return result;
		}
		public override object GetDataSource(string name) {
			object result = documentDataSources.GetDataSourceByName(name);
			if(!object.ReferenceEquals(result, null))
				return result;
			return controlDataSources.GetDataSourceByName(name);
		}
		public override ICollection<DataSourceInfo> GetInfos() {
			if(object.ReferenceEquals(this.dataSources, null))
				GetDataSourcesCore();
			return this.dataSources;
		}
		void GetDataSourcesCore() {
			this.dataSources = new List<DataSourceInfo>();
			this.calculatedFields = new List<ICalculatedField>();
			HashSet<string> realDocumentDataSources = new HashSet<string>();
			HashSet<string> stubDocumentDataSources = new HashSet<string>();
			foreach(DataSourceInfo sourceInfo in this.documentDataSources) {
				string dataSourceName = sourceInfo.DataSourceName;
				if(!object.ReferenceEquals(sourceInfo.DataSource, null)) {
					DataSourceInfo destInfo = new DataSourceInfo(dataSourceName, sourceInfo.DataSource);
					foreach(CalculatedField calcField in sourceInfo.CalculatedFields) {
						destInfo.CalculatedFields.Add(calcField);
						this.calculatedFields.Add(calcField);
					}
					this.dataSources.Add(destInfo);
					realDocumentDataSources.Add(dataSourceName);
				}
				else
					stubDocumentDataSources.Add(dataSourceName);
			}
			foreach(DataSourceInfo sourceInfo in this.controlDataSources) {
				string dataSourceName = sourceInfo.DataSourceName;
				if(realDocumentDataSources.Contains(dataSourceName))
					continue;
				if(object.ReferenceEquals(sourceInfo.DataSource, null))
					continue;
				DataSourceInfo destInfo = new DataSourceInfo(dataSourceName, sourceInfo.DataSource);
				if(stubDocumentDataSources.Contains(dataSourceName)) {
					HashSet<string> fieldNames = new HashSet<string>();
					foreach(CalculatedField docCalcField in this.documentDataSources.GetCalculatedFieldsByDataSourceName(dataSourceName)) {
						destInfo.CalculatedFields.Add(docCalcField);
						this.calculatedFields.Add(docCalcField);
						fieldNames.Add(docCalcField.Name);
					}
					foreach(CalculatedField ctrlCalcField in sourceInfo.CalculatedFields)
						if(!fieldNames.Contains(ctrlCalcField.Name)) {
							destInfo.CalculatedFields.Add(ctrlCalcField);
							this.calculatedFields.Add(ctrlCalcField);
						}
				}
				else
					foreach(CalculatedField calcField in sourceInfo.CalculatedFields) {
						destInfo.CalculatedFields.Add(calcField);
						this.calculatedFields.Add(calcField);
					}
				this.dataSources.Add(destInfo);
			}
		}
		public override string FindDataSourceName(object dataSource) {
			if(object.ReferenceEquals(dataSource, null))
				return null;
			HashSet<string> mask = new HashSet<string>();
			foreach(DataSourceInfo info in this.documentDataSources) {
				string dataSourceName = info.DataSourceName;
				if(object.ReferenceEquals(info.DataSource, dataSource))
					return dataSourceName;
				else
					if(!object.ReferenceEquals(info.DataSource, null))
						mask.Add(dataSourceName);
			}
			foreach(DataSourceInfo info in this.controlDataSources) {
				string dataSourceName = info.DataSourceName;
				if(object.ReferenceEquals(info.DataSource, dataSource) && !mask.Contains(dataSourceName))
					return dataSourceName;
			}
			return null;
		}
		public override ICollection<ICalculatedField> GetCalculatedFields() {
			if(object.ReferenceEquals(this.calculatedFields, null))
				GetDataSourcesCore();
			return this.calculatedFields;
		}
		public override ICollection<ICalculatedField> GetCalculatedFields(string dataSourceName) {
			List<ICalculatedField> result = new List<ICalculatedField>();
			DataSourceInfo docDsInfo = documentDataSources[dataSourceName];
			if(!object.ReferenceEquals(docDsInfo, null))
				result.AddRange(docDsInfo.CalculatedFields);
			if(object.ReferenceEquals(docDsInfo, null) || object.ReferenceEquals(docDsInfo.DataSource, null)) {
				DataSourceInfo ctrlDsInfo = controlDataSources[dataSourceName];
				if(!object.ReferenceEquals(ctrlDsInfo, null)) { 
					HashSet<string> namesInUse = new HashSet<string>();
					foreach(CalculatedField calcField in result)
						namesInUse.Add(calcField.Name);
					foreach(CalculatedField calcField in ctrlDsInfo.CalculatedFields)
						if(!namesInUse.Contains(calcField.Name))
							result.Add(calcField);
				}
			}
			return result;
		}
		public override ICalculatedField GetCalculatedField(string dataSourceName, string calculatedFieldName) {
			DataSourceInfo docDsInfo = documentDataSources[dataSourceName];
			if(!object.ReferenceEquals(docDsInfo, null)) {
				foreach(CalculatedField calcField in docDsInfo.CalculatedFields)
					if(string.Equals(calcField.Name, calculatedFieldName))
						return calcField;
				if(!object.ReferenceEquals(docDsInfo.DataSource, null))
					return null;
			}
			DataSourceInfo ctrlDsInfo = controlDataSources[dataSourceName];
			if(!object.ReferenceEquals(ctrlDsInfo, null)) {
				foreach(CalculatedField calcField in ctrlDsInfo.CalculatedFields)
					if(string.Equals(calcField.Name, calculatedFieldName))
						return calcField;
			}
			return null;
		}
		protected override void DisposeCore() {
			if (this.documentDataSources != null) {
				UnsubscribeFromEvents(this.documentDataSources);
				this.documentDataSources = null;
			}
			if (this.controlDataSources != null) {
				UnsubscribeFromEvents(this.controlDataSources);
				this.controlDataSources = null;
			}
		}
	}
	public class ServerDataSourceDispatcher : CollectionBasedDataSourceDispatcher {
		DataSourceInfoCollection dataSources;
		internal ServerDataSourceDispatcher() { }
		public override IDataSourceDispatcher CreateNew() {
			return new ServerDataSourceDispatcher();
		}
		public override void Initialize(SnapDocumentModel documentModel) {
			base.Initialize(documentModel);
			this.dataSources = documentModel.DataSources;
			SubscribeToEvents(this.dataSources);
		}
		void SubscribeToEvents(DataSourceInfoCollection dataSources) {
			UnsubscribeFromEvents(dataSources);
			dataSources.CollectionChanged += OnCollectionChanged;
			dataSources.DataSourceChanged += OnDataSourceChanged;
			dataSources.CalculatedFieldAdded += OnCalculatedFieldAdded;
		}
		void UnsubscribeFromEvents(DataSourceInfoCollection dataSources) {
			dataSources.CollectionChanged -= OnCollectionChanged;
			dataSources.DataSourceChanged -= OnDataSourceChanged;
			dataSources.CalculatedFieldAdded -= OnCalculatedFieldAdded;
		}
		void OnCalculatedFieldAdded(object sender, CalculatedFieldAddedEventArgs e) {
			if (e.Field.DataSourceDispatcher == null)
				e.Field.DataSourceDispatcher = this;
		}
		public override ICollection<object> GetDataSources() {
			return this.dataSources.GetDataSources();
		}
		public override object GetDataSource(string name) {
			return dataSources.GetDataSourceByName(name);
		}
		public override ICollection<DataSourceInfo> GetInfos() {
			return dataSources;
		}
		public override ICollection<ICalculatedField> GetCalculatedFields(string dataSourceName) {
			List<ICalculatedField> result = new List<ICalculatedField>();
			DataSourceInfo info = dataSources[dataSourceName];
			if(object.ReferenceEquals(info, null))
				return result;
			result.AddRange(info.CalculatedFields);
			return result;
		}
		public override ICalculatedField GetCalculatedField(string dataSourceName, string calculatedFieldName) {
			DataSourceInfo info = dataSources[dataSourceName];
			if(object.ReferenceEquals(info, null))
				return null;
			CalculatedFieldCollection calculatedFields = info.CalculatedFields;
			foreach(CalculatedField calcField in calculatedFields)
				if(string.Equals(calcField.Name, calculatedFieldName))
					return calcField;
			return null;
		}
		public override string FindDataSourceName(object dataSource) {
			foreach(DataSourceInfo info in this.dataSources)
				if(object.ReferenceEquals(info.DataSource, dataSource))
					return info.DataSourceName;
			return null;
		}
		public override ICollection<ICalculatedField> GetCalculatedFields() {
			return this.dataSources.GetCalculatedFields();
		}
		protected override void DisposeCore() {
			if (this.dataSources != null) {
				UnsubscribeFromEvents(this.dataSources);
				this.dataSources = null;
			}
		}
	}
	public class SingleContainerDataSourceDispatcher : DataSourceDispatcherBase {
		IDataSourceInfoContainer container;
		internal SingleContainerDataSourceDispatcher(IDataSourceInfoContainer container) {
			this.container = container;
		}
		void RaiseCollectionChanged() {
			RaiseCollectionChangedCore();
		}
		void RaiseDataSourceChanged() {
			RaiseDataSourceChangedCore();
		}
		#region IDataSourceDispatcher Members
		public override void Initialize(SnapDocumentModel documentModel) {
			throw new NotImplementedException();
		}
		public override ICollection<object> GetDataSources() {
			throw new NotImplementedException();
		}
		public override object GetDataSource(string name) {
			return container.GetDataSourceByName(name);
		}
		public override object DefaultDataSource {
			get { return container.DefaultDataSourceInfo.DataSource; }
		}
		public override ICollection<DataSourceInfo> GetInfos() {
			throw new NotImplementedException();
		}
		public override DataSourceInfo GetInfo(string dataSourceName) {
			throw new NotImplementedException();
		}
		public override DataSourceInfo GetInfo(object dataSource) {
			throw new NotImplementedException();
		}
		public override string FindDataSourceName(object dataSource) {
			throw new NotImplementedException();
		}
		public override ICollection<ICalculatedField> GetCalculatedFields() {
			throw new NotImplementedException();
		}
		public override ICollection<ICalculatedField> GetCalculatedFields(string dataSourceName) {
			throw new NotImplementedException();
		}
		public override ICollection<ICalculatedField> GetCalculatedFields(object dataSource) {
			return (ICollection<ICalculatedField>)(this.container.GetCalculatedFieldsByDataSource(dataSource)) ?? new List<ICalculatedField>(0);
		}
		public override ICalculatedField GetCalculatedField(string dataSourceName, string calculatedFieldName) {
			throw new NotImplementedException();
		}
		public override void BeginUpdate() {
			throw new NotImplementedException();
		}
		public override void EndUpdate() {
			throw new NotImplementedException();
		}
		public override IDataSourceDispatcher CreateNew() {
			throw new NotImplementedException();
		}
		public override DataSourceInfo DefaultDataSourceInfo {
			get { throw new NotImplementedException(); }
		}
		public override bool IsDefaultDataSource(object dataSource) {
			throw new NotImplementedException();
		}
		#endregion
	}
}
