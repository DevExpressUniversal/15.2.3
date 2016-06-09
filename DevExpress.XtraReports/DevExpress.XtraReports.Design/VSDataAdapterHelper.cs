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
using System.ComponentModel.Design;
using System.ComponentModel.Design.Data;
using System.Data;
using System.Windows.Forms;
using DevExpress.Data.Native;
using DevExpress.XtraReports.Native;
using DevExpress.XtraReports.UI;
namespace DevExpress.XtraReports.Design {
	public abstract class VSDataAdapterHelper {
		ComponentDesigner componentDesigner;
		IComponentChangeService componentChangeService;
		DataSourceProviderService dataSourceProviderService;
		protected abstract object DataSource { get; }
		protected abstract string DataMember { get; }
		protected abstract object DataAdapter { get; set; }
		protected ComponentDesigner ComponentDesigner { get { return componentDesigner; } }
		public VSDataAdapterHelper(ComponentDesigner componentDesigner) {
			this.componentDesigner = componentDesigner;
			IServiceProvider serviceProvider = componentDesigner.Component.Site;
			componentChangeService = (IComponentChangeService)serviceProvider.GetService(typeof(IComponentChangeService));
			dataSourceProviderService = (DataSourceProviderService)serviceProvider.GetService(typeof(DataSourceProviderService));
		}
		public void ValidateDataAdapter() {
			object dataSource = DataSource;
			if (dataSource != null && (dataSource is DataSet) && dataSourceProviderService != null) {
				componentChangeService.ComponentAdded += new ComponentEventHandler(ComponentAdded);
				DataTable table = DataSetHelper.GetDataTable((DataSet)dataSource, DataMember);
				dataSourceProviderService.NotifyDataSourceComponentAdded(new BindingSource(dataSource, table != null ? table.TableName : string.Empty));
				componentChangeService.ComponentAdded -= new ComponentEventHandler(ComponentAdded);
			}
		}
		void ComponentAdded(object sender, ComponentEventArgs e) {
			if (DataAdapter == null && BindingHelper.IsDataAdapter(e.Component)) {
				XRControlDesignerBase.RaiseComponentChanging(componentChangeService, componentDesigner.Component, "DataAdapter");
				try {
					DataAdapter = e.Component;
				} 
				finally {
					XRControlDesignerBase.RaiseComponentChanged(componentChangeService, componentDesigner.Component, 
						TypeDescriptor.GetProperties(componentDesigner.Component)["DataAdapter"]);
				}
			}
		}
	}
	public class VSChartDataAdapterHelper : VSDataAdapterHelper {
		XRChart XRChart {
			get { return (XRChart)ComponentDesigner.Component; }
		}
		protected override object DataSource { get { return XRChart.DataSource; } }
		protected override string DataMember { get { return XRChart.DataMember; } }
		protected override object DataAdapter { 
			get { return XRChart.DataAdapter; }
			set { XRChart.DataAdapter = value; }
		}
		public VSChartDataAdapterHelper(ComponentDesigner componentDesigner) : base(componentDesigner) {
		}
	}
	public class VSDataContainerDataAdapterHelper : VSDataAdapterHelper {
		IDataContainer DataContainer {
			get { return (IDataContainer)ComponentDesigner.Component; }
		}
		protected override object DataSource { get { return DataContainer.DataSource; } }
		protected override string DataMember { get { return DataContainer.DataMember; } }
		protected override object DataAdapter {
			get { return DataContainer.DataAdapter; }
			set { DataContainer.DataAdapter = value; }
		}
		public VSDataContainerDataAdapterHelper(ComponentDesigner componentDesigner)
			: base(componentDesigner) {
		}
	}
}
