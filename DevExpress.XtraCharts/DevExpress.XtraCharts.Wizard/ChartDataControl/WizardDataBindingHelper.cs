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
using System.Globalization;
using System.ComponentModel;
using System.ComponentModel.Design;
using DevExpress.Data;
using DevExpress.Data.Design;
using DevExpress.XtraEditors;
using DevExpress.XtraCharts.Native;
using DevExpress.Utils.UI;
using DevExpress.Data.Browsing.Design;
using DevExpress.Data.Browsing;
namespace DevExpress.XtraCharts.Wizard {
	class DataSourceTypeDescriptorContext : ITypeDescriptorContext {
		readonly IDesignerHost designerHost;
		public DataSourceTypeDescriptorContext(IDesignerHost designerHost) {
			this.designerHost = designerHost;
		}
		IContainer ITypeDescriptorContext.Container { get { return designerHost == null ? null : designerHost.Container; } }
		object ITypeDescriptorContext.Instance { get { return null; } }
		PropertyDescriptor ITypeDescriptorContext.PropertyDescriptor { get { return null; } }
		object IServiceProvider.GetService(Type serviceType) {
			return designerHost == null ? null : designerHost.GetService(serviceType);
		}
		bool ITypeDescriptorContext.OnComponentChanging() {
			return false;
		}
		void ITypeDescriptorContext.OnComponentChanged() {
		}
	}
	class DataSourceComboBoxItem {
		readonly object dataSource;
		readonly string name;
		public object DataSource { get { return dataSource; } }
		public string Name { get { return name; } }
		public DataSourceComboBoxItem(object dataSource, string name) {
			this.dataSource = dataSource;
			this.name = name;
		}
		public override string ToString() {
			return name;
		}
	}
	public static class WizardDataBindingHelper {
		public static string GetDataSourceName(ITypeDescriptorContext context, object dataSource) {
			IDisplayNameProvider displayNameProvider = dataSource as IDisplayNameProvider;
			if (displayNameProvider != null)
				return displayNameProvider.GetDataSourceDisplayName();
			ITypedList typedList = dataSource as ITypedList;
			if (typedList != null) {
				string listName = typedList.GetListName(new PropertyDescriptor[0]);
				if (!String.IsNullOrEmpty(listName))
					return listName;
			}
			DataSourceConverter typeConverter = new DataSourceConverter();
			string dataSourceName = (string)typeConverter.ConvertTo(context, CultureInfo.CurrentCulture, dataSource, typeof(string));
			if (dataSourceName == String.Empty)
				dataSourceName = dataSource.ToString();
			return dataSourceName;
		}
		static bool SetComboBoxEditValue(ComboBoxEdit cbDataSource, object selectedDataSource) {
			foreach (DataSourceComboBoxItem item in cbDataSource.Properties.Items)
				if (Object.ReferenceEquals(item.DataSource, selectedDataSource)) {
					cbDataSource.EditValue = item;
					return true;
				}
			return false;
		}
		public static void UpdateDataSourceComboBox(ComboBoxEdit cbDataSource, IChartContainer chartContainer, IServiceProvider serviceProvider, object selectedDataSource) {
			IDataSourceCollectorService dataSourceCollectorService = serviceProvider != null ? serviceProvider.GetService(typeof(IDataSourceCollectorService)) as IDataSourceCollectorService : null;
			if (dataSourceCollectorService != null) {
				cbDataSource.Properties.Items.Clear();
				DataContext dataContext = (chartContainer != null && chartContainer.DataProvider != null) ? chartContainer.DataProvider.DataContext : null;
				foreach (var dataSource in dataSourceCollectorService.GetDataSources()) {
					if (!(dataSource is DevExpress.XtraReports.Native.Parameters.ParametersDataSource)) {
						string dataSourceName = (dataContext != null) ? dataContext.GetDataSourceDisplayName(dataSource, string.Empty) : string.Empty;
						cbDataSource.Properties.Items.Add(new DataSourceComboBoxItem(dataSource, dataSourceName));
					}
				}
				SetComboBoxEditValue(cbDataSource, selectedDataSource);
				return;
			}
			IDesignerHost designerHost = serviceProvider != null ? serviceProvider.GetService(typeof(IDesignerHost)) as IDesignerHost : null;
			DataSourceTypeDescriptorContext dataSourceTypeDescriptorContext = new DataSourceTypeDescriptorContext(designerHost);
			cbDataSource.Properties.Items.Clear();
			DataSourceConverter typeConverter = new DataSourceConverter();
			ICollection dataSources = typeConverter.GetStandardValues(dataSourceTypeDescriptorContext);
			foreach (object dataSource in dataSources) {
				if (dataSource != null && designerHost != null) {
					IComponent component = dataSource as IComponent;
					if (component == null)
						continue;
					bool found = false;
					foreach (IComponent present in designerHost.Container.Components)
						if (Object.ReferenceEquals(component, present)) {
							found = true;
							break;
						}
					if (!found)
						continue;
				}
				string dataSourceName = (string)typeConverter.ConvertTo(dataSourceTypeDescriptorContext,
					CultureInfo.CurrentCulture, dataSource, typeof(string));
				cbDataSource.Properties.Items.Add(new DataSourceComboBoxItem(dataSource, dataSourceName));
			}
			string selectedDataSourceName = GetDataSourceName(dataSourceTypeDescriptorContext, selectedDataSource);
			if (designerHost == null)
				cbDataSource.EditValue = selectedDataSourceName;
			else if (String.IsNullOrEmpty(selectedDataSourceName))
				cbDataSource.SelectedIndex = -1;
			else {
				if (!SetComboBoxEditValue(cbDataSource, selectedDataSource))
					cbDataSource.EditValue = selectedDataSourceName;
			}
		}
	}
}
