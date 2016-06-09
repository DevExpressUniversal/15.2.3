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
	using DevExpress.Design.UI;
	using System.Collections.Generic;
	abstract class DataSourceSettingsModelBase : WpfBindableBase, IDataSourceSettingsModel, IServiceSettingsModel {
		IDataSourceInfo infoCore;
		static string[] EmptyFields = new string[] { };
		IDictionary<string, string> customBindingProperties;
		public DataSourceSettingsModelBase(IDataSourceInfo info) {
			AssertionException.IsNotNull(info);
			this.validationRules = new Dictionary<string, System.Func<IDataSourceSettingsModel, string>>();
			this.customBindingProperties = new Dictionary<string, string>();
			this.infoCore = info;
			this.fieldsCore = EmptyFields;
			this.selectedElementCore = System.Linq.Enumerable.FirstOrDefault(Elements);
			this.SelectedElementIsDataTable = selectedElementCore is IDataTableInfo;
			UpdateFields();
			InitializeServiceModel();
			RegisterValidationRules();
		}
		System.Type IDataSourceSettingsModel.Key {
			get { return GetKey(); }
		}
		IDictionary<string, string> IDataSourceSettingsModel.CustomBindingProperties {
			get { return customBindingProperties; }
		}
		protected abstract System.Type GetKey();
		public System.Type SourceType {
			get { return infoCore.SourceType; }
		}
		protected IDataSourceInfo Info {
			get { return infoCore; }
		}
		public object Component {
			get {
				IComponentDataSourceInfo componentDataSourceInfo = infoCore as IComponentDataSourceInfo;
				if(componentDataSourceInfo != null)
					return componentDataSourceInfo.Component;
				return null;
			}
		}
		public IEnumerable<IDataSourceElementInfo> Elements {
			get { return Info.Elements; }
		}
		public bool SelectedElementIsDataTable {
			get;
			private set;
		}
		public bool SelectedElementIsDataType {
			get { return !SelectedElementIsDataTable; }
		}
		public void Enter() {
			RaisePropertyChanged("SelectedElementIsDataType");
		}
		IDataSourceElementInfo selectedElementCore;
		public IDataSourceElementInfo SelectedElement {
			get { return selectedElementCore; }
			set { SetProperty(ref selectedElementCore, value, "SelectedElement", OnSelectedElementChanged); }
		}
		IEnumerable<string> fieldsCore;
		public IEnumerable<string> Fields {
			get { return fieldsCore; }
			private set { SetProperty(ref fieldsCore, value, "Fields", OnFieldsChanged); }
		}
		public bool IsDesignDataAllowed {
			get { return !(infoCore is IEnumDataSourceInfo); } 
		}
		bool showDesignDataCore = true;
		public bool ShowDesignData {
			get { return showDesignDataCore; }
			set { SetProperty(ref showDesignDataCore, value, "ShowDesignData", OnShowDesignDataChanged); }
		}
		bool showCodeBehindCore = true;
		public bool ShowCodeBehind {
			get { return showCodeBehindCore; }
			set { SetProperty(ref showCodeBehindCore, value, "ShowCodeBehind", OnShowShowCodeBehindChanged); }
		}
		int designDataRowCountCore = 5;
		public int DesignDataRowCount {
			get { return designDataRowCountCore; }
			set { SetProperty(ref designDataRowCountCore, value, "DesignDataRowCount", OnDesignDataRowCountChanged); }
		}
		protected virtual void OnSelectedElementChanged() {
			UpdateFields();
		}
		protected virtual void OnFieldsChanged() {
			customBindingProperties.Clear();
		}
		protected virtual void OnShowDesignDataChanged() { }
		protected virtual void OnDesignDataRowCountChanged() { }
		protected virtual void OnShowShowCodeBehindChanged() { }
		protected void UpdateFields() {
			Fields = (SelectedElement != null) ? SelectedElement.Fields : EmptyFields;
		}
		#region IDataErrorInfo Members
		IDictionary<string, System.Func<IDataSourceSettingsModel, string>> validationRules;
		protected void RegisterValidationRule(string propertyName, System.Func<IDataSourceSettingsModel, string> rule) {
			if(!string.IsNullOrEmpty(propertyName))
				validationRules.Add(propertyName, rule);
		}
		protected void RegisterValidationRule(DataSourcePropertyCodeName property, System.Func<IDataSourceSettingsModel, string> rule) {
			validationRules.Add(property.ToString(), rule);
		}
		protected virtual void RegisterValidationRules() {
			RegisterValidationRule("SelectedElement", (model) =>
			{
				return (model.SelectedElement == null) ? 
					(model.SelectedElementIsDataTable ? "Table must be specified" : "Element Type must be specified") : null;
			});
		}
		string System.ComponentModel.IDataErrorInfo.Error {
			get {
				var errorInfo = this as System.ComponentModel.IDataErrorInfo;
				string msg = "";
				foreach(var rule in validationRules)
					msg += errorInfo[rule.Key];
				return msg;
			}
		}
		string System.ComponentModel.IDataErrorInfo.this[string propertyName] {
			get {
				System.Func<IDataSourceSettingsModel, string> rule;
				if(validationRules.TryGetValue(propertyName, out rule))
					return rule(this);
				return null;
			}
		}
		#endregion
		#region Service
		string serviceRootCore;
		public string ServiceRoot {
			get { return serviceRootCore; }
			set { SetProperty(ref serviceRootCore, value, "ServiceRoot", OnServiceRootChanged); }
		}
		bool isValidServiceRootCore;
		public bool IsValidServiceRoot {
			get { return isValidServiceRootCore; }
			set { SetProperty(ref isValidServiceRootCore, value, "IsValidServiceRoot"); }
		}
		public bool HasServiceRoot {
			get { return !string.IsNullOrEmpty(ServiceRoot); }
		}
		public bool IsValidServiceRootUri {
			get { return HasServiceRoot && System.Uri.IsWellFormedUriString(ServiceRoot, System.UriKind.Absolute); }
		}
		public bool EnableTestConnection {
			get { return IsValidServiceRootUri && !IsValidServiceRoot; }
		}
		void OnServiceRootChanged() {
			IsValidServiceRoot = false;
			RaisePropertyChanged("HasServiceRoot");
			RaisePropertyChanged("IsValidServiceRootUri");
			RaisePropertyChanged("EnableTestConnection");
		}
		bool isServiceModel;
		void InitializeServiceModel() {
			var serviceInfo = System.Linq.Enumerable.FirstOrDefault(Elements, (e) => e is IDataServiceTableInfo);
			isServiceModel = (serviceInfo != null);
			serviceRootCore = isServiceModel ? ((IDataServiceTableInfo)serviceInfo).ServiceUri : null;
			ResetServiceRootCommand = new Design.UI.WpfDelegateCommand(ResetServiceRoot, () => isServiceModel);
			TestServiceRootCommand = new Design.UI.WpfDelegateCommand(TestServiceRoot, () => isServiceModel);
			if(isServiceModel) {
				RegisterValidationRule("ServiceRoot", (model) =>
				{
					IServiceSettingsModel serviceModel = model as IServiceSettingsModel;
					if(!serviceModel.HasServiceRoot)
						return "Service Root should be specified";
					if(!serviceModel.IsValidServiceRootUri)
						return "Valid Uri should be used as Service Root";
					return null;
				});
				RegisterValidationRule("EnableTestConnection", (model) =>
				{
					IServiceSettingsModel serviceModel = model as IServiceSettingsModel;
					if(!serviceModel.IsValidServiceRootUri)
						return null;
					if(!serviceModel.IsValidServiceRoot)
						return "Validate Service Root using Test Connection button";
					return null;
				});
			}
		}
		public Design.UI.ICommand<object> ResetServiceRootCommand {
			get;
			private set;
		}
		public Design.UI.ICommand<object> TestServiceRootCommand {
			get;
			private set;
		}
		void ResetServiceRoot() {
			ServiceRoot = null;
		}
		void TestServiceRoot() {
			try {
				var wcfServiceRoot = new System.Uri(ServiceRoot);
				var request = (System.Net.HttpWebRequest)System.Net.WebRequest.Create(wcfServiceRoot);
				request.Timeout = 30000;
				using(var response = request.GetResponse()) {
					IsValidServiceRoot = (response != null);
					RaisePropertyChanged("EnableTestConnection");
				}
			}
			catch { IsValidServiceRoot = false; }
		}
		#endregion
	}
}
