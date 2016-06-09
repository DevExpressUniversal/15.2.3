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
using System.IO;
using System.Linq;
using DevExpress.DataAccess.ConnectionParameters;
using DevExpress.DataAccess.Native;
using DevExpress.DataAccess.Native.Sql;
using DevExpress.DataAccess.Native.Sql.ConnectionStrategies;
using DevExpress.DataAccess.Wizard.Views;
using DevExpress.Mvvm;
using DevExpress.Mvvm.DataAnnotations;
using DevExpress.Mvvm.Native;
using DevExpress.Mvvm.POCO;
using DevExpress.DataAccess.UI.Localization;
namespace DevExpress.Xpf.DataAccess.DataSourceWizard {
	[POCOViewModel]
	public class ConnectionPropertiesPage : DataSourceWizardPage, IConnectionPropertiesPageView, IConnectionParametersControl {
		public static ConnectionPropertiesPage Create(DataSourceWizardModelBase model) {
			return ViewModelSource.Create(() => new ConnectionPropertiesPage(model));
		}
		protected ConnectionPropertiesPage(DataSourceWizardModelBase model)
			: base(model) {
			providers = ProviderLookupItem.GetPredefinedItems();
			SelectedProvider = providers.FirstOrDefault();
		}
		readonly IEnumerable<ProviderLookupItem> providers;
		public IEnumerable<ProviderLookupItem> Providers { get { return providers; } }
		void IConnectionPropertiesPageView.SelectProvider(string provider) {
			SelectProvider(provider);
		}
		void SelectProvider(string provider) {
			SelectedProvider = Providers.Where(x => string.Equals(x.ProviderKey, provider, StringComparison.Ordinal)).FirstOrDefault();
		}
		public virtual ProviderLookupItem SelectedProvider { get; set; }
		protected void OnSelectedProviderChanged() {
			InitializeDefaultValues();
			RaiseChanged();
			POCOViewModelExtensions.RaisePropertyChanged(this, x => x.ConnectionEdits);
			POCOViewModelExtensions.RaisePropertyChanged(this, x => x.DisabledConnectionEdits);
		}
		public virtual IEnumerable<string> Databases { get; protected set; }
		string connectionName;
		string IConnectionPropertiesPageView.ConnectionName {
			get { return connectionName ?? SelectedProvider.With(x => CreateUniqueName(x.Strategy.GetConnectionName(this))); }
			set { connectionName = value; }
		}
		DataConnectionParametersBase IConnectionPropertiesPageView.DataConnectionParameters {
			get { return SelectedProvider.With(x => x.Strategy.GetConnectionParameters(this)); }
			set {
				SelectProvider(DataConnectionParametersRepository.Instance.GetKeyByItem(value));
				SelectedProvider.Do(x => x.Strategy.InitializeControl(this, value));
			}
		}
		string[] existingConnections;
		void IConnectionPropertiesPageView.SetConnections(IEnumerable<INamedItem> connections) {
			existingConnections = connections.Select(x => x.Name).ToArray();
			RaiseChanged();
		}
		string CreateUniqueName(string core) {
			if(!existingConnections.Contains(core))
				return core;
			int index = 1;
			for(; ; ) {
				string name = string.Format("{0} {1}", core, index++);
				if(!existingConnections.Contains(name))
					return name;
			}
		}
		void IConnectionPropertiesPageView.InitControls() { }
		readonly Lazy<IEnumerable<BooleanViewModel>> serverTypes = BooleanViewModel.CreateList(DataAccessUILocalizer.GetString(DataAccessUIStringId.ConnectionProperties_ServerTypeServer), DataAccessUILocalizer.GetString(DataAccessUIStringId.ConnectionProperties_ServerTypeEmbedded), false);
		public IEnumerable<BooleanViewModel> ServerTypes { get { return serverTypes.Value; } }
		public virtual BigQueryAuthorizationType AuthTypeBigQuery { get; set; }
		protected void OnAuthTypeBigQueryChanged() {
			POCOViewModelExtensions.RaisePropertyChanged(this, x => x.ConnectionEdits);
		}
		public virtual MsSqlAuthorizationType AuthTypeMsSql { get; set; }
		protected void OnAuthTypeMsSqlChanged() {
			POCOViewModelExtensions.RaisePropertyChanged(this, x => x.DisabledConnectionEdits);
		}
		public virtual string CustomString { get; set; }
		public virtual string Database { get; set; }
		public virtual string DataSetID { get; set; }
		public virtual string FileName { get; set; }
		public virtual string KeyFileName { get; set; }
		public virtual string OAuthClientID { get; set; }
		public virtual string OAuthClientSecret { get; set; }
		public virtual string OAuthRefreshToken { get; set; }
		public virtual string Password { get; set; }
		public virtual string Port { get; set; }
		public virtual string ProjectID { get; set; }
		public virtual bool ServerBased { get; set; }
		protected void OnServerBasedChanged() {
			POCOViewModelExtensions.RaisePropertyChanged(this, x => x.ConnectionEdits);
		}
		public virtual string Hostname { get; set; }
		public virtual AdvantageServerType ServerTypeAdvantage { get; set; }
		public virtual string ServerName { get; set; }
		public virtual string ServiceEmail { get; set; }
		public virtual string UserName { get; set; }
		public virtual ConnectionParameterEdits ConnectionEdits { get { return SelectedProvider.Strategy.GetEditsSet(this); } }
		public virtual ConnectionParameterEdits DisabledConnectionEdits { get { return SelectedProvider.Strategy.GetDisabledEdits(this); } }
		public virtual bool CanSelectDatabase { get; set; }
		public void BrowseForAssembly() {
			model.Parameters.DoWithOpenFileDialogService(dialog => {
				if(dialog.ShowDialog()) {
					FileName = Path.Combine(dialog.File.DirectoryName, dialog.File.Name);
				}
			});
		}
		public void GetDatabases() {
			CanSelectDatabase = false;
			Databases = SelectedProvider.With(x => x.Strategy.GetDatabases(this));
			CanSelectDatabase = true;
		}
		void InitializeDefaultValues() {
			CustomString = SelectedProvider.Strategy.GetDefaultText(ConnectionParameterEdits.CustomString);
			Database = SelectedProvider.Strategy.GetDefaultText(ConnectionParameterEdits.Database);
			DataSetID = SelectedProvider.Strategy.GetDefaultText(ConnectionParameterEdits.DataSetID);
			FileName = SelectedProvider.Strategy.GetDefaultText(ConnectionParameterEdits.FileName);
			KeyFileName = SelectedProvider.Strategy.GetDefaultText(ConnectionParameterEdits.KeyFileName);
			OAuthClientID = SelectedProvider.Strategy.GetDefaultText(ConnectionParameterEdits.OAuthClientID);
			OAuthClientSecret = SelectedProvider.Strategy.GetDefaultText(ConnectionParameterEdits.OAuthClientSecret);
			OAuthRefreshToken = SelectedProvider.Strategy.GetDefaultText(ConnectionParameterEdits.OAuthRefreshToken);
			Password = SelectedProvider.Strategy.GetDefaultText(ConnectionParameterEdits.Password);
			Port = SelectedProvider.Strategy.GetDefaultText(ConnectionParameterEdits.Port);
			ProjectID = SelectedProvider.Strategy.GetDefaultText(ConnectionParameterEdits.ProjectID);
			ServerName = SelectedProvider.Strategy.GetDefaultText(ConnectionParameterEdits.ServerName);
			ServiceEmail = SelectedProvider.Strategy.GetDefaultText(ConnectionParameterEdits.ServiceEmail);
			UserName = SelectedProvider.Strategy.GetDefaultText(ConnectionParameterEdits.UserName);
		}
	}
}
