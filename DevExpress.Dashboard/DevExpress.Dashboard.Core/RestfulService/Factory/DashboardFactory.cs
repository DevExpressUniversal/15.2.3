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

using DevExpress.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
namespace DevExpress.DashboardCommon.Native.DashboardRestfulService {
	public abstract class DashboardFactoryBase : IDataSourceFactory, IDashboardItemFactory, IColorRepositoryFactory {
		#region ColorSchemeContext
		class ColorSchemeContext : IColorSchemeContext {
			public bool Loading { get { return false; } }
			public bool IsChangeLocked { get { return false; } }
			public IChangeService ChangeService { get { return null; } }
		}
		#endregion
		protected abstract XDocument GetDashboardDocument(string dashboardId);
		XElement GetDashboardRootElement(string dashboardId) {
			XDocument document = GetDashboardDocument(dashboardId);
			if (document != null)
				return DashboardClientXmlAdapter.GetDashboardRootElement(document);
			return null;
		}
		IEnumerable<IDashboardDataSource> CreateDataSources(string dashboardId) {
			XElement rootElement = GetDashboardRootElement(dashboardId);
			if (rootElement != null) {
				DataSourceCollection dataSources = new DataSourceCollection();
				dataSources.LoadFromXml(rootElement);
				return dataSources;
			}
			return null;
		}
		#region IDataSourceFactory
		IDashboardDataSource IDataSourceFactory.CreateDataSource(string dashboardId, string dataSourceId) {
			IEnumerable<IDashboardDataSource> dataSources = CreateDataSources(dashboardId);
			if (dataSources != null)
				return dataSources.FirstOrDefault(dataSource => dataSource.ComponentName == dataSourceId);
			return null;
		}
		#endregion
		#region IDashboardItemFactory
		DashboardItem IDashboardItemFactory.CreateDashboardItem(string dashboardId, string dashboardItemId) {
			IEnumerable<DashboardItem> items = ((IDashboardItemFactory)this).CreateDashboardItems(dashboardId);
			DashboardItem result = null;
			if(items != null) {
				result = items.FirstOrDefault(item => item.ComponentName == dashboardItemId);
			}
			if(result == null) {
				IEnumerable<DashboardItem> groups = this.CreateDashboardGroups(dashboardId);
				if(groups != null) {
					result = groups.FirstOrDefault(item => item.ComponentName == dashboardItemId);
				}
			}
			return result;
		}
		IEnumerable<DashboardItem> CreateDashboardGroups(string dashboardId) {
			XElement rootElement = GetDashboardRootElement(dashboardId);
			if (rootElement != null) {
				var groups = new DashboardItemGroupCollection();
				groups.LoadFromXml(rootElement);
				return groups;
			}
			return null;
		}
		IEnumerable<DashboardItem> IDashboardItemFactory.CreateDashboardItems(string dashboardId) {
			XElement rootElement = GetDashboardRootElement(dashboardId);
			if (rootElement != null) {
				DashboardItemCollection items = new DashboardItemCollection();
				items.LoadFromXml(rootElement);
				return items;
			}
			return null;
		}
		#endregion
		#region IColorRepositoryFactory
		ColorRepository IColorRepositoryFactory.CreateColorRepository(string dashboardId) {
			XElement rootElement = GetDashboardRootElement(dashboardId);
			if (rootElement != null) {
				ColorSchemeContainer container = new ColorSchemeContainer(new ColorSchemeContext());
				container.Scheme.LoadFromXml(rootElement);
				return container.Repository;
			}
			return null;
		}
		#endregion
	}
	public class DashboardFactory : DashboardFactoryBase {
		readonly IDashboardRepository repository;
		public DashboardFactory(IDashboardRepository repository) {
			Guard.ArgumentNotNull(repository, "repository");
			this.repository = repository;
		}
		protected override XDocument GetDashboardDocument(string dashboardId) {
			return repository.GetDashboard(dashboardId);
		}
	}
	public class OnSpotDashboardFactory : DashboardFactoryBase {
		readonly XDocument document;
		public OnSpotDashboardFactory(XDocument document) {
			Guard.ArgumentNotNull(document, "document");
			this.document = document;
		}
		protected override XDocument GetDashboardDocument(string dashboardId) {
			return document;
		}
	}
} 
