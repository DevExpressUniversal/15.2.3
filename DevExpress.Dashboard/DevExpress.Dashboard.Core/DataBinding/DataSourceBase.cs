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
using System.ComponentModel;
using System.Linq;
using System.Xml.Linq;
using DevExpress.DashboardCommon.Native;
using DevExpress.Data;
using DevExpress.DataAccess.Localization;
using DevExpress.DataAccess.Native;
using DevExpress.DataAccess.Native.Data;
using DevExpress.Utils.Controls;
using DevExpress.Compatibility.System.ComponentModel;
namespace DevExpress.DataAccess {
	[Obsolete ("The DataSourceBase class is obsolete now. Use the DashboardSqlDataSource class instead")]
	public abstract class DataSourceBase : DisposableObject, IDataSource, ISupportPrefix {
		public const string XmlDataSource = "DataSource";
		public const string XmlDataConnection = "DataConnection";
		public const string XmlSupportSql = "SupportSql";
		internal const string XmlDataProvider = "DataProvider";
		readonly Locker loadingLocker = new Locker();
		readonly DataSourceComponentNameController componentNameController;
		string dataConnectionName;
		IDataProvider dataProvider;
		[
		Category("General"),
		DefaultValue(null)
		]
		public string ComponentName {
			get { return componentNameController.ComponentName; }
			set {
				componentNameController.ComponentName = value;
			}
		}
		[
		Category("General"),
		DefaultValue(null)
		]
		public string Name {
			get { return componentNameController.Name; }
			set {
				componentNameController.Name= value;				
			}
		}
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DefaultValue(null)
		]
		public string DataProviderSerializable {
			get { return DataProvider != null ? Base64XmlSerializer.GetBase64String(SaveDataProviderToXml()) : null; }
			set {
				XElement element = Base64XmlSerializer.GetXElement(value);
				if(element != null)
					LoadDataProviderFromXml(element);
			}
		}
		protected DataSourceComponentNameController NameController { get { return componentNameController; } }
		public virtual object Data { get { return null; } set { } }
		protected internal virtual bool Loading { get { return loadingLocker.IsLocked; } }
		protected internal virtual IDataProvider DataProvider {
			get { return dataProvider; }
			set {
				if(dataProvider != value) {
					dataProvider = value;
				}
			}
		}
		protected abstract XmlRepository<IDataProvider> DataProvidersRepository { get; }
		protected virtual ISite Site { get { return null; } }
		bool IDataSource.SupportCancel { get { return DataProvider != null; } }
		IDataProvider IDataSource.DataProvider { get { return DataProvider; } set { DataProvider = value; } }
		string ISupportPrefix.Prefix { get { return DataAccessLocalizer.GetString(DataAccessStringId.DefaultNameDataSource); } }
		string INamedItem.Name { get { return ComponentName; } set { ComponentName = value; } }
		string IDataSource.DisplayName { get { return Name; } set { Name = value; } }
		event EventHandler DataChanging;
		protected event EventHandler<DataChangedEventArgs> DataChanged;
		public  event EventHandler<DataSchemaChangedEventArgs> DisplayNamesChanged;
		event EventHandler<NameChangedEventArgs> NameChanged;
		event EventHandler DataLoaded;
		event EventHandler<NameChangingEventArgs> NameChanging;
		event EventHandler IDataSource.DataChanging {
			add { DataChanging = (EventHandler)Delegate.Combine(DataChanging, value); }
			remove { DataChanging = (EventHandler)Delegate.Remove(DataChanging, value); }
		}
		event EventHandler<DataChangedEventArgs> IDataSource.DataChanged {
			add { AddDataChangedHandler(value); }
			remove { RemoveDataChangedHandler(value); }
		}		
		event EventHandler IDataSource.DataLoaded {
			add { DataLoaded += value; }
			remove { DataLoaded -= value; }
		}
		event EventHandler<NameChangingEventArgs> INameContainer.NameChanging {
			add { AddNameChangingHandler(value); }
			remove { RemoveNameChangingHandler(value); }
		}	   
		public event EventHandler CaptionChanged;
		protected DataSourceBase()
			: this(null) {
		}
		protected DataSourceBase(string componentName)
			: this(componentName, null) {
		}
		protected DataSourceBase(string componentName, string name) {
			componentNameController = new DataSourceComponentNameController(name, loadingLocker, () => Site);
			componentNameController.NameChanged += componentNameController_NameChanged;
			componentNameController.NameChanging += componentNameController_NameChanging;
			componentNameController.CaptionChanged += componentNameController_CaptionChanged;
			ComponentName = componentName;			
		}
		void componentNameController_CaptionChanged(object sender, EventArgs e) {
			if(CaptionChanged != null)
				CaptionChanged(this, e);
		}
		void componentNameController_NameChanging(object sender, NameChangingEventArgs e) {
			if(NameChanging != null)
				NameChanging(this, e);
		}
		void componentNameController_NameChanged(object sender, NameChangedEventArgs e) {
			if(NameChanged != null)
				NameChanged(this, e);
		}
		public override string ToString() {
			string name = componentNameController.Name;
			return string.IsNullOrEmpty(name) ? base.ToString() : name;
		}
		protected override void Dispose(bool disposing) {
			if (disposing) {
				if (DataProvider != null && !DataProvider.IsDisposed) {
					DataProvider.Dispose();
				}
			}
			base.Dispose(disposing);
		}
		void IDataSource.ExecutePreview() {
		  }
		void IDataSource.SetParameters(IEnumerable<IParameter> parameters) {
			SetParametersCore(parameters);
		}
		protected virtual void SetParametersCore(IEnumerable<IParameter> parameters) {
	   }
		void IDataSource.ReloadData() {
			ReloadDataCore(null);
		}
		void IDataSource.ReloadData(object newData) {
			ReloadDataCore(newData);
		}
		object IDataSource.RequestData() {
			return RequestDataCore();
		}
		protected virtual object RequestDataCore() {
			return null;
		}
		protected virtual void ReloadDataCore(object newData) {
		  }
		void IDataSource.CancelExecute() {
			if (DataProvider != null)
				DataProvider.CancelExecute();
		}
		public XElement SaveToXml() {
			return SaveToXmlCore();
		}
		protected virtual XElement SaveToXmlCore() {
			XElement element = new XElement(XmlDataSource);
			componentNameController.SaveToXml(element);
			if(DataProvider != null) {
				XElement dataProviderElement = SaveDataProviderToXml();
				if(dataProviderElement != null)
					element.Add(dataProviderElement);
			}
			return element;
		}
		protected void RaiseDisplayNamesChanged(DataSchemaChangedEventArgs e) {
			if (DisplayNamesChanged != null)
				DisplayNamesChanged(this, e);
		}
		protected void RaiseDataChanging() {
			if (DataChanging != null)
				DataChanging(this, EventArgs.Empty);
		}
		protected void RaiseDataChanged(bool isSchemaChanged) {
			if (DataChanged != null)
				DataChanged(this, new DataChangedEventArgs(isSchemaChanged));
		}
		protected void RaiseDataLoaded() {
			if (DataLoaded != null)
				DataLoaded(this, EventArgs.Empty);
		}
		protected void BeginLoading() {
			loadingLocker.Lock();
		}
		protected void EndLoading() {
			loadingLocker.Unlock();
		}
		protected void AddDataChangedHandler(EventHandler<DataChangedEventArgs> value) {
			DataChanged +=value;
		}
		protected void RemoveDataChangedHandler(EventHandler<DataChangedEventArgs> value) {
			DataChanged -= value;
		}
		protected void AddNameChangedHandler(EventHandler<NameChangedEventArgs> value) {
			NameChanged += value;
		}
		protected void RemoveNameChangedHandler(EventHandler<NameChangedEventArgs> value) {
			NameChanged -= value;
		}
		protected void AddNameChangingHandler(EventHandler<NameChangingEventArgs> value) {
			NameChanging += value;
		}
		protected void RemoveNameChangingHandler(EventHandler<NameChangingEventArgs> value) {
			NameChanging -= value;
		}
		public void LoadFromXml(XElement dataSourceElement) {
			BeginLoading();
			try {
				LoadFromXmlCore(dataSourceElement);
			}
			finally {
				EndLoading();
			}
		}
		protected virtual void Load_13_1_NameFromXml(string name) {
		}
		protected virtual void LoadFromXmlCore(XElement dataSourceElement) {
			componentNameController.LoadFromXml(dataSourceElement, true);
			XElement dataProviderElement = dataSourceElement.Elements().FirstOrDefault();
			if(dataProviderElement != null)
				LoadDataProviderFromXml(dataProviderElement);
		}
		XElement SaveDataProviderToXml() {
			XmlSerializer<IDataProvider> serializer = DataProvidersRepository.GetSerializer(DataProvider);
			if(serializer != null)
				return serializer.SaveToXml(DataProvider);
			return null;
		}
		void LoadDataProviderFromXml(XElement dataProviderElement) {
			XmlSerializer<IDataProvider> serializer = DataProvidersRepository.GetSerializer(dataProviderElement.Name.LocalName);
			if(serializer != null) {
				DataProvider = serializer.LoadFromXml(dataProviderElement);
				dataConnectionName = XmlHelperBase.GetAttributeValue(dataProviderElement, DataProviderBase.XmlDataConnection);
			}			
		}
		void IDataSource.SetConnection(IEnumerable<DataConnectionBase> connections) {
			DataConnectionBase connection = connections.FirstOrDefault(item => item.Name == dataConnectionName);
			if(connection != null)
				DataProvider.Connection = connection;
		}
	}
}
