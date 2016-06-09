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
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Drawing.Design;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using DevExpress.Data;
using DevExpress.Data.Entity;
using DevExpress.Data.Filtering;
using DevExpress.Data.Utils;
using DevExpress.DataAccess.Native;
using DevExpress.DataAccess.Native.EntityFramework;
using DevExpress.DataAccess.Native.Sql;
using DevExpress.Entity.ProjectModel;
using DevExpress.Utils;
using DevExpress.Utils.Serializing;
using DevExpress.XtraReports;
namespace DevExpress.DataAccess.EntityFramework {
	[Designer("DevExpress.DataAccess.Design.VSEFDataSourceDesigner," + AssemblyInfo.SRAssemblyDataAccessDesign, typeof(IDesigner))]
	[XRDesigner("DevExpress.DataAccess.UI.Design.XREFDataSourceDesigner," + AssemblyInfo.SRAssemblyDataAccessUI, typeof(IDesigner))]
	[ToolboxItem(false)]
	[ToolboxBitmap(typeof(ResFinder), "Bitmaps256.EFDataSource.bmp")]
	[DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.DataAccess.EntityFramework.EFDataSource", "EFDataSource")]
	public class EFDataSource : DataComponentBase, IListSource, IListAdapter, ISupportInitialize, ITypedList {
		const string xmlConnectionName = "ConnectionName";
		const string xmlName = "Name";
		const string xmlConnectionStringName = "ConnectionStringName";
		const string xmlConnectionString = "ConnectionString";
		const string xmlCustomAssemblyPath = "CustomAssemblyPath";
		const string xmlCustomContextName = "CustomContextName";
		const string xmlSource = "Source";
		const string xmlProcedures = "Procedures";
		const string xmlDataSourceNodeName = "EFDataSource";
		readonly EFStoredProcedureInfoCollection storedProcedures;
		string connectionStringSerialized;
		string connectionNameSerialized;
		string connectionStringNameSerialized;
		string sourceSerialized;
		string customAssemblyPathSerialized;
		string customContextNameSerialized;
		EFDataConnection connection;
		EFContextWrapper efTypedList;
		bool loading;
		EFConnectionParameters connectionParameters;
		bool filled;
		public EFDataSource(IContainer container) : this() {
			Guard.ArgumentNotNull(container, "container");
			container.Add(this);
		}
		public EFDataSource() {
			storedProcedures = new EFStoredProcedureInfoCollection(this);
			filled = false;
		}
		public EFDataSource(EFConnectionParameters connectionParameters)
			: this() {
			this.connectionParameters = connectionParameters;
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public EFDataSource(EFConnectionParameters connectionParameters, string storedProcedureName)
			: this(connectionParameters) {
			if(storedProcedureName != null)
				StoredProcedures.Add(new EFStoredProcedureInfo(storedProcedureName));
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[DXDisplayName(typeof(ResFinder), "DevExpress.DataAccess.EntityFramework.EFDataSource.StoredProcedures")]
#if !SL
	[DevExpressDataAccessLocalizedDescription("EFDataSourceStoredProcedures")]
#endif
		[Category("Data")]
		public EFStoredProcedureInfoCollection StoredProcedures {
			get { return storedProcedures; }
			set {
				this.storedProcedures.Clear();
				this.storedProcedures.AddRange(value);
			}
		}
		[DXDisplayName(typeof(ResFinder), "DevExpress.DataAccess.EntityFramework.EFDataSource.Connection")]
#if !SL
	[DevExpressDataAccessLocalizedDescription("EFDataSourceConnection")]
#endif
		[Category("Data")]
		public EFDataConnection Connection {
			get {
				if(connection == null && (ConnectionParameters != null || sourceSerialized != null))
					CreateDataConnection();
				return connection;
			}
		}
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		public EFConnectionParameters ConnectionParameters {
			get { return connectionParameters; }
			set {
				if(connectionParameters != value) {
					connectionParameters = value;
					efTypedList = null;
					filled = false;
					CreateDataConnection();
				}
			}
		}
		internal string DataMember { get; set; }
		protected override IEnumerable<IParameter> AllParameters { get { return StoredProcedures.SelectMany(p => p.Parameters); } }
		EFContextWrapper ContextWrapper {
			get {
				if(efTypedList == null)
					CreateEFContextWrapper();
				return efTypedList;
			}
		}
		#region for xml serialization only (properties)
		[XtraSerializableProperty(0)]
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public string ConnectionName { get { return Connection == null ? connectionNameSerialized : Connection.Name; } set { connectionNameSerialized = value; } }
		[XtraSerializableProperty(1)]
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public string ConnectionStringName { get { return Connection == null ? connectionStringNameSerialized : Connection.ConnectionParameters.ConnectionStringName; } set { connectionStringNameSerialized = value; } }
		[XtraSerializableProperty(2)]
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public string ConnectionString { get { return Connection == null ? connectionStringSerialized : Connection.ConnectionParameters.ConnectionString; } set { connectionStringSerialized = value; } }
		[XtraSerializableProperty(3)]
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public string Source { get { return Connection == null ? sourceSerialized : Connection.ConnectionParameters.Source.AssemblyQualifiedName; } set { sourceSerialized = value; } }
		[XtraSerializableProperty(4)]
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public string ProceduresSerialized { get { return GetSerializedProcedures(); } set { DeserializeProcedures(value); } }
		internal string CustomAssemblyPath {
			get {
				return Connection == null ? customAssemblyPathSerialized : Connection.ConnectionParameters.CustomAssemblyPath;
			}
			set {
				customAssemblyPathSerialized = value;
			}
		}
		internal string CustomContextName {
			get {
				return Connection == null ? customContextNameSerialized : Connection.ConnectionParameters.CustomContextName;
			}
			set {
				customContextNameSerialized = value;
			}
		}
		#endregion
		#region ISupportInitialize
		void ISupportInitialize.BeginInit() {
			loading = true;
		}
		void ISupportInitialize.EndInit() {
			loading = false;
		}
		#endregion
		#region ITypedList Members
		public string GetListName(PropertyDescriptor[] listAccessors) {
			return this.Name;
		}
		public PropertyDescriptorCollection GetItemProperties(PropertyDescriptor[] listAccessors) {
			EFContextWrapper wrapper = ContextWrapper;
			if(wrapper == null)
				return new PropertyDescriptorCollection(new PropertyDescriptor[] {});
			ContextWrapper.StoredProcedures.Clear();
			ContextWrapper.StoredProcedures.AddRange(StoredProcedures);
			return ContextWrapper.GetItemProperties(listAccessors);
		}
		#endregion
		#region IListSource Members
		IList IListSource.GetList() {
			return ContextWrapper;
		}
		bool IListSource.ContainsListCollection {
			get { return ContextWrapper != null; }
		}
		#endregion
		#region IListAdapter Members
		void IListAdapter.FillList(IServiceProvider serviceProvider) {
			ReplaceServiceFromProvider(typeof(ISolutionTypesProvider), serviceProvider);
			ReplaceServiceFromProvider(typeof(IConnectionStringsProvider), serviceProvider);
			if(Connection != null) {
				AddServicesToConection(this);
				ContextWrapper.StoredProcedures.Clear();
				IParameterSupplierBase parameterSupplier = serviceProvider != null ? serviceProvider.GetService<IParameterSupplierBase>() : null;
				List<IParameter> parentParameters = parameterSupplier != null ? parameterSupplier.GetIParameters().ToList() : null;
				foreach(EFStoredProcedureInfo storedProcedureInfo in StoredProcedures) {
					object evaluatedParameters = parentParameters != null ? ParametersEvaluator.EvaluateParameters(parentParameters, storedProcedureInfo.Parameters) : storedProcedureInfo.Parameters;
					ContextWrapper.StoredProcedures.Add(new EFStoredProcedureInfo(storedProcedureInfo.Name, (IEnumerable<IParameter>)evaluatedParameters));
				}
				Connection.CreateDataStore();
				if(!Connection.IsConnected)
					return;
				ContextWrapper.FillData();
				filled = true;
			}
		}
		bool IListAdapter.IsFilled { get { return filled; } }
		#endregion
		public void Fill() {
			((IListAdapter)this).FillList(null);
		}
		protected override void Fill(IEnumerable<IParameter> sourceParameters) {
			ServiceContainer serviceContainer = new ServiceContainer();
			serviceContainer.AddService(typeof(IParameterSupplierBase), new ParameterSupplier(sourceParameters));
			((IListAdapter)this).FillList(serviceContainer);
			OnAfterFill(sourceParameters);
		}
		protected override void Dispose(bool disposing) {
			if(disposing) {
				if(this.connection != null)
					this.connection.Dispose();
			}
			base.Dispose(disposing);
		}
		protected override string GetDataMember() {
			return DataMember;
		}
		void AddServicesToConection(IServiceProvider provider) {
			Connection.SolutionTypesProvider = provider.GetService<ISolutionTypesProvider>();
			Connection.ConnectionStringsProvider = provider.GetService<IConnectionStringsProvider>();
		}
		void CreateDataConnection() {
			if(loading)
				return;
			string customAssemblyPath = ConnectionParameters != null ? ConnectionParameters.CustomAssemblyPath : customAssemblyPathSerialized;
			EFConnectionParameters parameters;
			if(customAssemblyPath != null)
				parameters = ConnectionParameters ?? new EFConnectionParameters(this.customContextNameSerialized, this.customAssemblyPathSerialized, this.connectionStringNameSerialized, this.connectionStringSerialized);
			else
				parameters = ConnectionParameters ?? new EFConnectionParameters(Type.GetType(this.sourceSerialized, true), this.connectionStringNameSerialized, this.connectionStringSerialized);
			this.connection = new EFDataConnection(connectionNameSerialized, parameters);
			RefreshTypesProviderService();
			AddServicesToConection(this);
		}
		void CreateEFContextWrapper() {
			if(Connection != null)
				this.efTypedList = CreateContextWrapperCore();
		}
		protected virtual EFContextWrapper CreateContextWrapperCore() {
			return new EFContextWrapper(Connection, StoredProcedures);
		}
		void RefreshTypesProviderService() {
			Assembly assembly = Connection != null && Connection.ConnectionParameters.Source != null ? Connection.ConnectionParameters.Source.Assembly : Assembly.GetEntryAssembly();
			if(assembly == null)
				return;
			ISolutionTypesProvider provider = ((IServiceContainer)this).GetService(typeof(ISolutionTypesProvider)) as ISolutionTypesProvider;
			if(provider is RuntimeSolutionTypesProvider) {
				((IServiceContainer)this).RemoveService(typeof(ISolutionTypesProvider));
				provider = null;
			}
			if(provider != null)
				return;
			RuntimeSolutionTypesProvider typesProvider = new RuntimeSolutionTypesProvider(assembly.GetTypes);
			((IServiceContainer)this).AddService(typeof(ISolutionTypesProvider), typesProvider);
		}
		#region for xml serialization only (methods)
		string GetSerializedProcedures() {
			XElement root = SaveProceduresToXml();
			return root == null ? null : Base64Helper.Encode(root);
		}
		XElement SaveProceduresToXml() {
			if(StoredProcedures.Count == 0)
				return null;
			XElement root = new XElement(xmlProcedures);
			foreach(EFStoredProcedureInfo procedure in StoredProcedures)
				root.Add(procedure.SaveToXml());
			return root;
		}
		void DeserializeProcedures(string value) {
			XElement element = Base64Helper.Decode(value);
			LoadProceduresFromXml(element);
		}
		void LoadProceduresFromXml(XElement element) {
			StoredProcedures.Clear();
			foreach(XElement procedureElement in element.Elements(EFStoredProcedureInfo.storedProcInfoXml)) {
				EFStoredProcedureInfo procedureInfo = new EFStoredProcedureInfo();
				procedureInfo.LoadFromXMl(procedureElement);
				StoredProcedures.Add(procedureInfo);
			}
		}
		public override XElement SaveToXml() {
			XElement root = new XElement(xmlDataSourceNodeName);
			if(!string.IsNullOrEmpty(Name))
				root.Add(new XAttribute(xmlName, Name));
			if(ConnectionName != null)
				root.Add(new XAttribute(xmlConnectionName, ConnectionName));
			if(ConnectionStringName != null)
				root.Add(new XAttribute(xmlConnectionStringName, ConnectionStringName));
			if(ConnectionString != null)
				root.Add(new XAttribute(xmlConnectionString, ConnectionString));
			if(Source != null)
				root.Add(new XAttribute(xmlSource, Source));
			if(CustomAssemblyPath != null)
				root.Add(new XAttribute(xmlCustomAssemblyPath, CustomAssemblyPath));
			if(CustomAssemblyPath != null)
				root.Add(new XAttribute(xmlCustomContextName, CustomContextName));
			XElement procedures = SaveProceduresToXml();
			if(procedures != null)
				root.Add(procedures);
			return root;
		}
		public override void LoadFromXml(XElement element) {
			Guard.ArgumentNotNull(element, "element");
			Name = element.GetAttributeValue(xmlName);
			ConnectionName = element.GetAttributeValue(xmlConnectionName);
			ConnectionStringName = element.GetAttributeValue(xmlConnectionStringName);
			ConnectionString = element.GetAttributeValue(xmlConnectionString);
			Source = element.GetAttributeValue(xmlSource);
			CustomAssemblyPath = element.GetAttributeValue(xmlCustomAssemblyPath);
			CustomContextName = element.GetAttributeValue(xmlCustomContextName);
			XElement procedures = element.Element(xmlProcedures);
			if(procedures != null)
				LoadProceduresFromXml(procedures);
		}
		protected virtual void OnAfterFill(IEnumerable<IParameter> parameters) {
		}
		#endregion
	}
}
