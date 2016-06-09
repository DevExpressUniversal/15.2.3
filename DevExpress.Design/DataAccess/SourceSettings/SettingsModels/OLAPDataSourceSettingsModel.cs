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

namespace DevExpress.Design.DataAccess {
	using System.Collections.Generic;
	using System.Globalization;
	using DevExpress.XtraPivotGrid;
	using DevExpress.XtraPivotGrid.Data;
	class OLAPDataSourceSettingsModel : DataSourceSettingsModelBase, IOLAPDataSourceSettingsModel {
		public OLAPDataSourceSettingsModel(IDataProcessingMode processingMode, IDataSourceInfo info)
			: base(info) {
			Cultures = CultureInfo.GetCultures(CultureTypes.AllCultures);
			DataProvider = GetOLAPDataProvider(processingMode.GetCodeName());
			Providers = OLAPMetaGetter.GetProviders();
			ResetProviderCommand = new Design.UI.WpfDelegateCommand(ResetProvider);
			RetrieveSchemaCommand = new Design.UI.WpfDelegateCommand(RetrieveSchema);
			ResetServerCommand = new Design.UI.WpfDelegateCommand(ResetServer);
			ResetCatalogCommand = new Design.UI.WpfDelegateCommand(ResetCatalog);
			ResetCubeCommand = new Design.UI.WpfDelegateCommand(ResetCube);
		}
		protected override System.Type GetKey() {
			return typeof(IOLAPDataSourceSettingsModel);
		}
		protected override void RegisterValidationRules() {
			RegisterValidationRule(DataSourcePropertyCodeName.Server, (model) =>
			{
				return string.IsNullOrEmpty(((IOLAPDataSourceSettingsModel)model).Server) ? "Server Name must be specified" :
					(((OLAPDataSourceSettingsModel)model).isSchemaRetrieved ? null : "Try to Retrieve DataBase Schema from server");
			});
			RegisterValidationRule("SelectedCatalog", (model) =>
			{
				if(!((OLAPDataSourceSettingsModel)model).isSchemaRetrieved) return null;
				return !((IOLAPDataSourceSettingsModel)model).HasCatalog ? "Catalog Name must be specified" : null;
			});
			RegisterValidationRule("SelectedCube", (model) =>
			{
				if(!((OLAPDataSourceSettingsModel)model).isSchemaRetrieved) return null;
				return !((IOLAPDataSourceSettingsModel)model).HasCube ? "Cube Name must be specified" : null;
			});
		}
		public object DataProvider {
			get;
			private set;
		}
		#region Commands
		public Design.UI.ICommand<object> ResetProviderCommand {
			get;
			private set;
		}
		public Design.UI.ICommand<object> RetrieveSchemaCommand {
			get;
			private set;
		}
		public Design.UI.ICommand<object> ResetServerCommand {
			get;
			private set;
		}
		public Design.UI.ICommand<object> ResetCatalogCommand {
			get;
			private set;
		}
		public Design.UI.ICommand<object> ResetCubeCommand {
			get;
			private set;
		}
		#endregion Commands
		public string ConnectionString {
			get {
				OLAPConnectionStringBuilder builder = new OLAPConnectionStringBuilder();
				builder.Provider = SelectedProvider;
				builder.ServerName = Server;
				builder.CatalogName = SelectedCatalog;
				builder.CubeName = SelectedCube;
				builder.QueryTimeout = QueryTimeout;
				builder.ConnectionTimeout = ConnectionTimeout;
				builder.Password = Password;
				builder.UserId = UserId;
				if(SelectedCulture != null)
					builder.LocaleIdentifier = SelectedCulture.LCID;
				return builder.FullConnectionString;
			}
		}
		public IEnumerable<CultureInfo> Cultures {
			get;
			private set;
		}
		CultureInfo selectedCultureCore = CultureInfo.CurrentCulture;
		public CultureInfo SelectedCulture {
			get { return selectedCultureCore; }
			set { SetProperty(ref selectedCultureCore, value, "SelectedCulture", OLAPConnectionStringChanged); }
		}
		public IEnumerable<string> Providers {
			get;
			private set;
		}
		string selectedProviderCore;
		public string SelectedProvider {
			get { return selectedProviderCore; }
			set { SetProperty(ref selectedProviderCore, value, "SelectedProvider", OnSelectedProviderChanged); }
		}
		public bool HasProvider {
			get { return !string.IsNullOrEmpty(SelectedProvider); }
		}
		string serverCore;
		public string Server {
			get { return serverCore; }
			set { SetProperty(ref serverCore, value, "Server", OnServerChanged); }
		}
		public bool HasServer {
			get { return !string.IsNullOrEmpty(Server); }
		}
		public IEnumerable<string> Catalogs {
			get;
			private set;
		}
		string catalogCore;
		public string SelectedCatalog {
			get { return catalogCore; }
			set { SetProperty(ref catalogCore, value, "SelectedCatalog", OnSelectedCatalogChanged); }
		}
		public bool HasCatalog {
			get { return !string.IsNullOrEmpty(SelectedCatalog); }
		}
		public IEnumerable<string> Cubes {
			get;
			private set;
		}
		string cubeCore;
		public string SelectedCube {
			get { return cubeCore; }
			set { SetProperty(ref cubeCore, value, "SelectedCube", OnSelectedCubeChanged); }
		}
		public bool HasCube {
			get { return !string.IsNullOrEmpty(SelectedCube); }
		}
		int connectionTimeoutCore = 60;
		public int ConnectionTimeout {
			get { return connectionTimeoutCore; }
			set { SetProperty(ref connectionTimeoutCore, value, "ConnectionTimeout", OLAPConnectionStringChanged); }
		}
		int queryTimeoutCore = 30;
		public int QueryTimeout {
			get { return queryTimeoutCore; }
			set { SetProperty(ref queryTimeoutCore, value, "QueryTimeout", OLAPConnectionStringChanged); }
		}
		string userIdCore;
		public string UserId {
			get { return userIdCore; }
			set { SetProperty(ref userIdCore, value, "UserId", OLAPConnectionStringChanged); }
		}
		string passwordCore;
		public string Password {
			get { return passwordCore; }
			set { SetProperty(ref passwordCore, value, "Password", OLAPConnectionStringChanged); }
		}
		bool isSchemaRetrieved;
		void RetrieveSchema() {
			OLAPConnectionStringBuilder builder = new OLAPConnectionStringBuilder();
			builder.Provider = SelectedProvider;
			builder.ServerName = Server;
			using(OLAPMetaGetter metaGetter = new OLAPMetaGetter()) {
				metaGetter.ConnectionString = builder.ConnectionString;
				this.isSchemaRetrieved = metaGetter.Connected;
				Catalogs = metaGetter.Connected ? 
					(IEnumerable<string>)metaGetter.GetCatalogs() : new string[] { };
				RaisePropertyChanged("Catalogs");
				SelectedCatalog = System.Linq.Enumerable.FirstOrDefault(Catalogs);
				RaisePropertyChanged("Server");
				RaisePropertyChanged("SelectedCatalog");
				RaisePropertyChanged("SelectedCube");
			}
			OLAPConnectionStringChanged();
		}
		void ResetServer() {
			this.isSchemaRetrieved = false;
			Server = null;
			SelectedCatalog = null;
			SelectedCube = null;
			RaisePropertyChanged("SelectedCatalog");
			RaisePropertyChanged("SelectedCube");
			RaisePropertyChanged("ConnectionString");
		}
		void ResetProvider() {
			SelectedProvider = null;
		}
		void ResetCatalog() {
			SelectedCatalog = null;
		}
		void ResetCube() {
			SelectedCube = null;
		}
		void OnSelectedProviderChanged() {
			RaisePropertyChanged("HasProvider");
			if(!string.IsNullOrEmpty(Server)) {
				OLAPConnectionStringBuilder builder = new OLAPConnectionStringBuilder();
				builder.Provider = SelectedProvider;
				builder.ServerName = Server;
				using(IOLAPMetaGetter metaGetter = new OLAPMetaGetter()) {
					metaGetter.ConnectionString = builder.ConnectionString;
					Catalogs = metaGetter.Connected ? 
						(IEnumerable<string>)metaGetter.GetCatalogs() : new string[] { };
					RaisePropertyChanged("Catalogs");
					SelectedCatalog = System.Linq.Enumerable.FirstOrDefault(Catalogs);
				}
			}
			OLAPConnectionStringChanged();
		}
		void OnServerChanged() {
			RaisePropertyChanged("HasServer");
		}
		void OnSelectedCatalogChanged() {
			RaisePropertyChanged("HasCatalog");
			OLAPConnectionStringBuilder builder = new OLAPConnectionStringBuilder();
			builder.Provider = SelectedProvider;
			builder.ServerName = Server;
			builder.CatalogName = SelectedCatalog;
			using(OLAPMetaGetter metaGetter = new OLAPMetaGetter()) {
				metaGetter.ConnectionString = builder.ConnectionString;
				Cubes = !string.IsNullOrEmpty(SelectedCatalog) ? 
					(IEnumerable<string>)metaGetter.GetCubes(SelectedCatalog) : new string[] { };
				RaisePropertyChanged("Cubes");
				SelectedCube = System.Linq.Enumerable.FirstOrDefault(Cubes);
			}
			OLAPConnectionStringChanged();
		}
		void OnSelectedCubeChanged() {
			RaisePropertyChanged("HasCube");
			OLAPConnectionStringChanged();
		}
		void OLAPConnectionStringChanged() {
			RaisePropertyChanged("ConnectionString");
		}
		static OLAPDataProvider GetOLAPDataProvider(DataProcessingModeCodeName modeCodeName) {
			switch(modeCodeName) {
				case DataProcessingModeCodeName.ADOMDforOLAP:
					return XtraPivotGrid.OLAPDataProvider.Adomd;
				case DataProcessingModeCodeName.OLEDBforOLAP:
					return XtraPivotGrid.OLAPDataProvider.OleDb;
				case DataProcessingModeCodeName.XMLAforOLAP:
					return XtraPivotGrid.OLAPDataProvider.Xmla;
			}
			throw new System.NotSupportedException(modeCodeName.ToString());
		}
	}
}
