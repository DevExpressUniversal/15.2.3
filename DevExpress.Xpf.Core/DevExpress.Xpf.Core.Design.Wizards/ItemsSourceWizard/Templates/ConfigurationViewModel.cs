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
using System.Windows;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Input;
using System.Net.NetworkInformation;
using System.Net;
using System.Reflection;
using System.Linq;
using System.Linq.Expressions;
using System.Collections.ObjectModel;
using DevExpress.Xpf.Core.Design.Wizards.Utils;
using System.ComponentModel.Design;
using VSLangProj;
using DevExpress.Mvvm;
namespace DevExpress.Xpf.Core.Design.Wizards.ItemsSourceWizard.Templates {
	public class ConfigurationViewModelBase : DependencyObject, IDataErrorInfo {
		bool isConnectionSucceed;
		IEnumerable<string> displayTypes;
		#region static
		public static readonly DependencyProperty TablesProperty;
		public static readonly DependencyProperty SelectedTableProperty;
		public static readonly DependencyProperty ServiceUriStringProperty;
		public static readonly DependencyProperty IsServiceRootEnabledProperty;
		public static readonly DependencyProperty IsTableEnabledProperty;
		public static readonly DependencyProperty IsQueryEnabledProperty;
		public static readonly DependencyProperty IsTypeNameEnabledProperty;
		public static readonly DependencyProperty IsValidProperty;
		public static readonly DependencyProperty TypeNameProperty;
		public static readonly DependencyProperty FieldNamesProperty;
		static ConfigurationViewModelBase() {
			SelectedTableProperty =
	DependencyProperty.Register("SelectedTable", typeof(DataTable), typeof(ConfigurationViewModelBase),
	new FrameworkPropertyMetadata((o, e) => ((ConfigurationViewModelBase)o).OnSelectedTableChanged()));
			TablesProperty =
DependencyProperty.Register("Tables", typeof(List<DataTable>), typeof(ConfigurationViewModelBase), new FrameworkPropertyMetadata());
			ServiceUriStringProperty =
DependencyProperty.Register("ServiceUriString", typeof(string), typeof(ConfigurationViewModelBase),
new FrameworkPropertyMetadata("localhost://", (o, e) => ((ConfigurationViewModelBase)o).OnServiceUriStringChanged()));
			IsServiceRootEnabledProperty =
DependencyProperty.Register("IsServiceRootEnabled", typeof(bool), typeof(ConfigurationViewModelBase),
new FrameworkPropertyMetadata(false, (o, e) => ((ConfigurationViewModelBase)o).OnIsServiceRootEnabledChanged()));
			IsTableEnabledProperty =
DependencyProperty.Register("IsTableEnabled", typeof(bool), typeof(ConfigurationViewModelBase),
new FrameworkPropertyMetadata(false, (o, e) => ((ConfigurationViewModelBase)o).OnIsTableEnabledChanged()));
			IsQueryEnabledProperty =
DependencyProperty.Register("IsQueryEnabled", typeof(bool), typeof(ConfigurationViewModelBase),
new FrameworkPropertyMetadata(false, (o, e) => ((ConfigurationViewModelBase)o).OnIsQueryEnabledChanged()));
			IsTypeNameEnabledProperty =
DependencyProperty.Register("IsTypeNameEnabled", typeof(bool), typeof(ConfigurationViewModelBase),
new FrameworkPropertyMetadata(false, (o, e) => ((ConfigurationViewModelBase)o).OnIsTypeNameEnabledChanged()));
			IsValidProperty =
DependencyProperty.Register("IsValid", typeof(bool), typeof(ConfigurationViewModelBase), new FrameworkPropertyMetadata());
			TypeNameProperty = DependencyProperty.Register("TypeName", typeof(string), typeof(ConfigurationViewModelBase),
new FrameworkPropertyMetadata((o, e) => ((ConfigurationViewModelBase)o).OnTypeNameChanged()));
			FieldNamesProperty =
				DependencyProperty.Register("FieldNames", typeof(ObservableCollection<string>), typeof(ConfigurationViewModelBase), new FrameworkPropertyMetadata());
		}
		#endregion
		public ConfigurationViewModelBase(List<DataTable> tables) {
			Tables = tables;
			if(Tables != null && Tables.Count > 0)
				SelectedTable = Tables[0];
			DesignData = new DesignDataViewModel();
			TestConnectionCmd = new DelegateCommand(OnTestConnection);
			isConnectionSucceed = true;
		}
		public DesignDataViewModel DesignData { get; protected set; }
		public List<DataTable> Tables {
			get { return (List<DataTable>)GetValue(TablesProperty); }
			set { SetValue(TablesProperty, value); }
		}
		public bool IsServiceRootEnabled {
			get { return (bool)GetValue(IsServiceRootEnabledProperty); }
			set { SetValue(IsServiceRootEnabledProperty, value); }
		}
		public bool IsTableEnabled {
			get { return (bool)GetValue(IsTableEnabledProperty); }
			set { SetValue(IsTableEnabledProperty, value); }
		}
		public bool IsQueryEnabled {
			get { return (bool)GetValue(IsQueryEnabledProperty); }
			set { SetValue(IsQueryEnabledProperty, value); }
		}
		public bool IsTypeNameEnabled {
			get { return (bool)GetValue(IsTypeNameEnabledProperty); }
			set { SetValue(IsTypeNameEnabledProperty, value); }
		}
		public bool IsValid {
			get { UpdateIsValid(); return (bool)GetValue(IsValidProperty); }
			set { SetValue(IsValidProperty, value); }
		}
		public bool IsConnectionSucceed {
			get { return isConnectionSucceed; }
			set {
				isConnectionSucceed = value;
				UpdateIsValid();
			}
		}
		public DataTable SelectedTable {
			get { return (DataTable)GetValue(SelectedTableProperty); }
			set { SetValue(SelectedTableProperty, value); }
		}
		public string ServiceUriString {
			get { return (string)GetValue(ServiceUriStringProperty); }
			set { SetValue(ServiceUriStringProperty, value); }
		}
		public string TypeName {
			get { return (string)GetValue(TypeNameProperty); }
			set { SetValue(TypeNameProperty, value); }
		}
		public Uri WcfServiceRoot { get; private set; }
		public ICommand TestConnectionCmd { get; private set; }
		public ObservableCollection<string> FieldNames {
			get { return (ObservableCollection<string>)GetValue(FieldNamesProperty); }
			set { SetValue(FieldNamesProperty, value); }
		}
		public Type SelectedType { get; private set; }
		public IEnumerable<string> DisplayTypes { get { return displayTypes ?? (displayTypes = AssemblyTypeIterator.Instance.Select(a => a.Name).ToList()); } }
		public virtual void OnSelectedTableChanged() {
			UpdateIsValid();
			if(SelectedTable != null)
				FieldNames = new ObservableCollection<string>(SelectedTable.FieldsName);
		}
		public virtual void OnTypeNameChanged() {
			UpdateIsValid();
			if(string.IsNullOrEmpty(TypeName)) {
				FieldNames.Clear();
				return;
			}
			FieldNames = new ObservableCollection<string>();
			SelectedType = AssemblyTypeIterator.Instance.Where(t => t.Name == TypeName).Select(t => t).FirstOrDefault();
			if(SelectedType == null)
				return;
			PropertyInfo[] typeInfo = SelectedType.GetProperties(BindingFlags.Public | BindingFlags.Instance);
			foreach(PropertyInfo prop in typeInfo)
				FieldNames.Add(prop.Name);
		}
		private void OnIsQueryEnabledChanged() {
			UpdateIsValid();
		}
		private void OnIsTableEnabledChanged() {
			UpdateIsValid();
		}
		private void OnIsTypeNameEnabledChanged() {
			UpdateIsValid();
		}
		private void OnIsServiceRootEnabledChanged() {
			if(IsServiceRootEnabled) {
				isConnectionSucceed = false;
				OnTestConnection();
			}
		}
		private bool ValidateWcfSeviceRoot() {
			try {
				if(ServiceUriString.EndsWith(".svc") || ServiceUriString.EndsWith(".svc/"))
					WcfServiceRoot = new Uri(ServiceUriString);
				else
					return false;
			} catch {
				return false;
			}
			return true;
		}
		private void OnServiceUriStringChanged() {
			try {
				UpdateIsValid();
			} catch { }
		}
		private void OnTestConnection() {
			try {
				WcfServiceRoot = new Uri(ServiceUriString);
				HttpWebRequest req = (HttpWebRequest)WebRequest.Create(WcfServiceRoot);
				req.Timeout = 30000;
				Mouse.SetCursor(Cursors.Wait);
				WebResponse response = req.GetResponse();
				Mouse.SetCursor(Cursors.Arrow);
				IsConnectionSucceed = response != null ? true : false;
				UpdateIsValid();
			} catch {
				IsConnectionSucceed = false;
				return;
			}
		}
		private void UpdateIsValid() {
			bool isNoError = Error == null || Error == string.Empty;
			IsValid = isNoError && IsConnectionSucceed;
		}
		#region IDataErrorInfo Members
		public string Error {
			get {
				string msg = "";
				msg += ((IDataErrorInfo)this)["SelectedTable"];
				msg += ((IDataErrorInfo)this)["ServiceUriString"];
				msg += ((IDataErrorInfo)this)["TypeName"];
				return msg;
			}
		}
		public string this[string propertyName] {
			get {
				string msg = null;
				switch(propertyName) {
					case "SelectedTable":
						if(IsTableEnabled)
							msg = SelectedTable == null ? "Table must be selected" : string.Empty;
						if(IsQueryEnabled)
							msg = SelectedTable == null ? "Query must be selected" : string.Empty;
						break;
					case "ServiceUriString":
						if(IsServiceRootEnabled)
							msg = !ValidateWcfSeviceRoot() ? "Incorrect uri" : string.Empty;
						break;
					case "TypeName":
						if(IsTypeNameEnabled)
							msg = string.IsNullOrEmpty(TypeName) ? "Type can't be null or empty" : string.Empty;
						break;
					default:
						break;
				}
				IsValid = string.IsNullOrEmpty(msg) && IsConnectionSucceed;
				return msg;
			}
		}
		#endregion
	}
	public class SimpleConfigurationViewModel : ConfigurationViewModelBase {
		public SimpleConfigurationViewModel(List<DataTable> tables)
			: base(tables) {
		}
	}
	public class DesignDataViewModel : DependencyObject {
		#region static
		public static readonly DependencyProperty IsEnableProperty;
		public static readonly DependencyProperty IsDifferentValuesProperty;
		public static readonly DependencyProperty RowCountProperty;
		public static readonly DependencyProperty FlattenHierarchyProperty;
		static DesignDataViewModel() {
			Type ownerType = typeof(DesignDataViewModel);
			IsEnableProperty = DependencyProperty.Register("IsEnable", typeof(bool), ownerType, new FrameworkPropertyMetadata(true));
			IsDifferentValuesProperty = DependencyProperty.Register("IsDifferentValues", typeof(bool), ownerType, new FrameworkPropertyMetadata(true));
			RowCountProperty = DependencyProperty.Register("RowCount", typeof(int), ownerType, new FrameworkPropertyMetadata(5));
			FlattenHierarchyProperty = DependencyProperty.Register("FlattenHierarchy", typeof(bool), ownerType, new FrameworkPropertyMetadata(false));
		}
		#endregion
		public bool IsEnable {
			get { return (bool)GetValue(IsEnableProperty); }
			set { SetValue(IsEnableProperty, value); }
		}
		public bool IsDifferentValues {
			get { return (bool)GetValue(IsDifferentValuesProperty); }
			set { SetValue(IsDifferentValuesProperty, value); }
		}
		public int RowCount {
			get { return (int)GetValue(RowCountProperty); }
			set { SetValue(RowCountProperty, value); }
		}
		public bool FlattenHierarchy {
			get { return (bool)GetValue(FlattenHierarchyProperty); }
			set { SetValue(FlattenHierarchyProperty, value); }
		}
	}
	public class DataTable {
		public DataTable(string name, List<string> fieldsName) {
			this.Name = name;
			this.FieldsName = fieldsName;
		}
		public string Name { get; private set; }
		public List<string> FieldsName { get; private set; }
		public List<KeyExpression> KeyExpressions { get; set; }
	}
	public class KeyExpression {
		public string Expression { get; private set; }
		public bool IsAutoDetected { get; private set; }
		public KeyExpression(string expression, bool isAutoDetected) {
			Expression = expression;
			IsAutoDetected = isAutoDetected;
		}
		public static List<KeyExpression> Wrap(IEnumerable<string> expressions, bool isAutoDetected) {
			return expressions.Select(ex => new KeyExpression(ex, isAutoDetected)).ToList();
		}
		public override string ToString() {
			return IsAutoDetected ? string.Format("{0} (AutoDetected)", Expression) : Expression;
		}
	}
}
