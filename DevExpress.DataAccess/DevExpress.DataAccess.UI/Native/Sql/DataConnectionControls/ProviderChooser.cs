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
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DevExpress.DataAccess.ConnectionParameters;
using DevExpress.DataAccess.Native;
using DevExpress.DataAccess.Native.Sql;
using DevExpress.DataAccess.UI.Localization;
using DevExpress.LookAndFeel;
using DevExpress.Utils;
using DevExpress.XtraEditors;
namespace DevExpress.DataAccess.UI.Native.Sql.DataConnectionControls {
	[ToolboxItem(false)]
	public partial class ProviderChooser : XtraUserControl {
		#region Inner classes
		public class ConnectionNamesHelper {
			public const string DefaultConnectionSuffix = "Connection";
			public IEnumerable<INamedItem> Connections { get; set; }
			public bool ContainsConnectionWithName(string name) {
				return Connections.Any(c => c.Name == name);
			}
			public string GetNextConnectionName(string namePrefix) {
				string name;
				int index = 1;
				do
					name = String.Format("{0} {1}", namePrefix, index++); while(ContainsConnectionWithName(name));
				return name;
			}
			public string GetName(string name) {
				return ContainsConnectionWithName(name) ?
					GetNextConnectionName(name) : name;
			}
		}
		#endregion
		static ParameterControlsRepository CreateControlsRepository() {
			ParameterControlsRepository controlsRepository = new ParameterControlsRepository();
			return controlsRepository;
		}
		readonly Locker locker = new Locker();
		readonly Dictionary<string, ControlItem> parameterItems = new Dictionary<string, ControlItem>();
		readonly List<string> providerKeys = new List<string> {
		};
		ConnectionNamesHelper connectionNamesHelper;
		bool connectionNameVisible;
		ParameterControlsRepository controlsRepository;
		public string DataConnectionName { get { return this.connectionNameControl.ConnectionName; } }
		public Point ConnectionNameLocation { get { return this.connectionNameControl.ConnectionNameLocation; } }
		public bool ConnectionNameVisible
		{
			get { return this.connectionNameVisible; }
			set
			{
				this.connectionNameVisible = value;
				this.connectionNameControl.Visible = value;
			}
		}
		ParameterControlsRepository ControlsRepository { get { return this.controlsRepository ?? (this.controlsRepository = CreateControlsRepository()); } }
		public event EventHandler ProviderChanged;
		internal void RaiseChanged() {
			if(ProviderChanged != null)
				ProviderChanged(this, EventArgs.Empty);
		}
		public ProviderChooser() {
			InitializeComponent();
			this.providersList.LookAndFeel.ParentLookAndFeel = LookAndFeel;
			this.connectionNameControl.LookAndFeel.ParentLookAndFeel = LookAndFeel;
			this.contentPanelControl.LookAndFeel.ParentLookAndFeel = LookAndFeel;
			SetSupportedDataConnectionTypes(this.providerKeys);
		}
		void ShowControl(BaseContentControl control, bool showConnectionName) {
			this.contentPanelControl.Hide();
			try {
				var oldControl = this.contentPanelControl.Controls.OfType<BaseContentControl>().FirstOrDefault();
				if(oldControl != null)
					oldControl.Changed -= ProviderChanged;
				this.contentPanelControl.Controls.Clear();
				this.contentPanelControl.Controls.Add(control);
				control.Changed += ProviderChanged;
				this.contentPanelControl.Controls.Add(this.connectionNameControl);
				control.Dock = DockStyle.Top;
				control.LookAndFeel.ParentLookAndFeel = this.contentPanelControl.LookAndFeel;
				foreach(Control subControl in control.Controls) {
					var slnf = subControl as ISupportLookAndFeel;
					if(slnf == null)
						continue;
					slnf.LookAndFeel.ParentLookAndFeel = control.LookAndFeel;
				}
				this.connectionNameControl.Dock = DockStyle.Fill;
				this.connectionNameControl.Visible = showConnectionName;
				this.connectionNameControl.BringToFront();
			} finally {
				this.contentPanelControl.Show();
			}
		}
		void providersList_SelectedValueChanged(object sender, EventArgs e) {
			ControlItem item = this.providersList.SelectedItem as ControlItem;
			if(item != null) {
				if(item.Control == null) {
					item.Control = CreateParametersControl(item.FactoryKey);
					item.Control.Initialize(DataConnectionHelper.GetProviderFactory(item.FactoryKey), this);
				}
				if(!this.connectionNameControl.IsConnectionNameChanged && this.connectionNamesHelper != null) {
					this.locker.Lock();
					ChangeConnectionName(true);
					this.locker.Unlock();
				}
				ShowControl(item.Control, ConnectionNameVisible);
				RaiseChanged();
			}
		}
		BaseContentControl CreateParametersControl(string factoryKey) {
			ParametersControlTypeItem controlParameters = ControlsRepository.GetControlParameters(factoryKey);
			return (BaseContentControl)Activator.CreateInstance(controlParameters.ControlType, new object[] {});
		}
		void OnDispose(bool disposing) {
			if(disposing && !this.providersList.IsDisposed)
				foreach(ControlItem item in this.providersList.Properties.Items)
					if(item.Control != null)
						item.Control.Dispose();
		}
		public void Initialize(IEnumerable<INamedItem> connections) {
			this.connectionNamesHelper = new ConnectionNamesHelper {Connections = connections};
		}
		public void SelectProvider(string provider) {
			this.providersList.SelectedItem = this.parameterItems[provider];
		}
		public void SetParameters(string connectionName, DataConnectionParametersBase parameters) {
			this.connectionNameControl.ConnectionName = connectionName;
			ControlItem item = this.parameterItems[DataConnectionParametersRepository.Instance.GetKeyByItem(parameters)];
			this.providersList.SelectedItem = item;
			item.Control.Parameters = parameters;
		}
		void SetSupportedDataConnectionTypes(List<string> items) {
			Guard.ArgumentNotNull(items, "ProviderItems");
			this.providersList.Properties.Items.Clear();
			this.parameterItems.Clear();
			foreach(string factoryKey in items) {
				ControlItem parametersItem = new ControlItem();
				parametersItem.FactoryKey = factoryKey;
				parametersItem.DisplayName = ControlsRepository.GetControlParameters(factoryKey).DisplayName;
				this.providersList.Properties.Items.Add(parametersItem);
				this.parameterItems.Add(factoryKey, parametersItem);
			}
			this.providersList.Properties.DropDownRows = items.Count;
		}
		public DataConnectionParametersBase GetParameters() {
			ControlItem item = this.providersList.SelectedItem as ControlItem;
			if(item == null || item.Control == null)
				return null;
			return item.Control.Parameters;
		}
		public void ChangeConnectionName(bool isAutoMode) {
			ControlItem currentItem = this.providersList.SelectedItem as ControlItem;
			if(isAutoMode && currentItem != null && !this.connectionNameControl.IsConnectionNameChanged && this.connectionNamesHelper != null) {
				this.locker.Lock();
				StringBuilder stringBuilder = new StringBuilder();
				string serverName = currentItem.Control.ConnectionNamePatternServerPart;
				if(!string.IsNullOrEmpty(serverName)) {
					stringBuilder.Append(serverName);
					stringBuilder.Append("_");
				}
				string dataBase = currentItem.Control.ConnectionNamePatternDatabasePart;
				if(!string.IsNullOrEmpty(dataBase)) {
					try {
						dataBase = Path.GetFileNameWithoutExtension(dataBase);
						stringBuilder.Append(dataBase);
					} catch {
					}
				}
				stringBuilder.Append(ConnectionNamesHelper.DefaultConnectionSuffix);
				string name = stringBuilder.ToString();
				this.connectionNameControl.ConnectionName = this.connectionNamesHelper.GetName(name);
				this.locker.Unlock();
			}
		}
		internal void SetProvider(string providerKey) {
			this.providersList.SelectedItem = this.parameterItems[providerKey];
		}
		internal BaseContentControl GetContentControl(string providerKey) {
			ControlItem item = this.parameterItems[providerKey];
			return item.Control;
		}
	}
	public class ControlItem {
		public string FactoryKey { get; set; }
		public string DisplayName { get; set; }
		public BaseContentControl Control { get; set; }
		public override string ToString() {
			return DisplayName;
		}
	}
	public class ParametersControlTypeItem {
		public string DisplayName { get; set; }
		public Type ControlType { get; set; }
		public ParametersControlTypeItem(string displayName, Type controlType) {
			DisplayName = displayName;
			ControlType = controlType;
		}
	}
	public class ParameterControlsRepository {
		readonly Dictionary<string, ParametersControlTypeItem> items = new Dictionary<string, ParametersControlTypeItem>();
		public void RegisterItemType(string key, ParametersControlTypeItem type) {
			if(this.items.ContainsKey(key))
				throw new ArgumentException(String.Format("Repository already contains the '{0}' key", key));
			this.items.Add(key, type);
		}
		public ParametersControlTypeItem GetControlParameters(string key) {
			ParametersControlTypeItem item;
			if(this.items.TryGetValue(key, out item)) {
				return item;
			}
			throw new ArgumentException(String.Format("Unsupported  key '{0}'", key));
		}
	}
}
