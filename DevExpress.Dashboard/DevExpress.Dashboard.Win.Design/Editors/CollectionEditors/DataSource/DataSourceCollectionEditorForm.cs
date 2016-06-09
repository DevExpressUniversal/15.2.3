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
using System.Linq;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Collections.Generic;
using System.Windows.Forms;
using DevExpress.DashboardCommon;
using DevExpress.DashboardWin.Native;
using DevExpress.Utils.UI;
using DevExpress.Data.Utils;
namespace DevExpress.DashboardWin.Design {
	class DataSourceCollectionEditorForm : DashboardCollectionEditorFormBase {
		#region DataSourceCollectionEditorContentControl
		class DataSourceCollectionEditorContentControl : CollectionEditorContentControl {
			DashboardDesigner DashboardDesigner { get { return GetService<SelectedContextService>().Designer; } }
			public DataSourceCollectionEditorContentControl(IServiceProvider provider, DevExpress.Utils.UI.CollectionEditor collectionEditor)
				: base(provider, collectionEditor) {
			}
			protected override void buttonAdd_Click(object sender, EventArgs e) {
				OnCreateItem();
			}
			void OnCreateItem() {
				DashboardDesigner designer = DashboardDesigner;				
				IDashboardDataSource dataSource = DataSourceExecutor.AddNewDataSource(DashboardDesigner.ServiceProvider);
				if (dataSource != null) {
					Dashboard dashboard = designer.Dashboard;				
					IComponentChangeService changeService = GetService<IComponentChangeService>();
					IDesignerHost designerHost = GetService<IDesignerHost>();
					dataSource.Name = dashboard.DataSourceCaptionGenerator.GenerateName(dataSource);
					dataSource.ComponentName = DevExpress.DashboardCommon.Native.Helper.CreateDashboardComponentName(dashboard, dataSource.GetType());
					designerHost.Container.Add(dataSource, dataSource.ComponentName);
					dashboard.BeginUpdate();
					try {
						AddInstance(dataSource);
						dashboard.FillDataSource(dataSource);
					}
					finally {
						dashboard.EndUpdate();
					}
				}
			}
			protected override void DisposeInstanceOnFinish(object instance) {
				IDashboardDataSource dataSource = instance as IDashboardDataSource;
				Dashboard dashboard = Context.Instance as Dashboard;
				if(dataSource != null && dashboard != null) {
					IComponentChangeService changeService = GetService<IComponentChangeService>();
					if(changeService != null) {
						foreach(DataDashboardItem dashboardItem in dashboard.Items) {
							if(dashboardItem.DataSource == dataSource) {
								PropertyDescriptor dsProperty = TypeDescriptor.GetProperties(dashboardItem)["DataSource"];
								changeService.OnComponentChanging(dashboardItem, dsProperty);
								dashboardItem.DataSource = null;
								changeService.OnComponentChanged(dashboardItem, dsProperty, dataSource, null);
								if (dashboardItem.DataMember != null) {
									string oldDataMember = dashboardItem.DataMember;
									PropertyDescriptor dmProperty = TypeDescriptor.GetProperties(dashboardItem)["DataMember"];
									changeService.OnComponentChanging(dashboardItem, dmProperty);
									dashboardItem.DataMember = null;
									changeService.OnComponentChanged(dashboardItem, dmProperty, oldDataMember, null);
								}
							}
						}
					}
				}
				base.DisposeInstanceOnFinish(instance);
			}
		}
		#endregion
		const string DataSourcesTitle = "Data Sources";
		readonly List<IDashboardDataSource> initialDataSources = new List<IDashboardDataSource>();
		DashboardParameterCollection initialParameters;
		public override string Text { get { return DataSourcesTitle; } }
		public DataSourceCollectionEditorForm(IServiceProvider provider, Utils.UI.CollectionEditor collectionEditor)
			: base(provider, collectionEditor) {
		}
		protected override Utils.UI.CollectionEditorContentControl CreateCollectionEditorContentControl(IServiceProvider provider, Utils.UI.CollectionEditor collectionEditor) {
			return new DataSourceCollectionEditorContentControl(provider, collectionEditor);
		}
		protected override void OnShown(EventArgs e) {
			base.OnShown(e);
			initialDataSources.AddRange(DashboardDesigner.Dashboard.DataSources);
			initialParameters = DashboardDesigner.Dashboard.Parameters.Clone();
		}
		protected override void OnClosed(EventArgs e) {
			base.OnClosed(e);
			if (DialogResult == DialogResult.OK)
				SyncronizeDashboardDataConnections();
			else {
				DashboardParameterCollection dashboardParamCollection = Dashboard.Parameters;
				dashboardParamCollection.Clear();
				dashboardParamCollection.AddRange(initialParameters);
			}
		}
	}
}
