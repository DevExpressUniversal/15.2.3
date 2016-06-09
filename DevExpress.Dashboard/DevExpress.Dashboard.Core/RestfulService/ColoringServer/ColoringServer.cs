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

using DevExpress.DashboardCommon.DataProcessing;
using DevExpress.Utils;
using System.Collections.Generic;
using System.Linq;
namespace DevExpress.DashboardCommon.Native.DashboardRestfulService {   
	public class ColoringServer : Server<ColoringServer.Key, ColoringServerSession>, IColoringServer {
		#region Key
		public class Key {
			public string DashboardId { get; private set; }
			public Key(string dashboardId) {
				DashboardId = dashboardId;
			}
			public override string ToString() {
				return DashboardId;
			}
		}
		#endregion
		readonly IDashboardItemFactory dashboardItemFactory;
		readonly IColorRepositoryFactory colorRepositoryFactory;
		public ColoringServer(IDashboardItemFactory dashboardItemFactory, IColorRepositoryFactory colorRepositoryFactory) {
			Guard.ArgumentNotNull(dashboardItemFactory, "dashboardItemFactory");
			Guard.ArgumentNotNull(colorRepositoryFactory, "colorRepositoryFactory");
			this.dashboardItemFactory = dashboardItemFactory;
			this.colorRepositoryFactory = colorRepositoryFactory;
		}
		protected override ColoringServerSession CreateSessionInstance(Key key) {
			return new ColoringServerSession();
		}
		ColorRepository GetGlobalColoringCache(string dashboardId, IDataSourceInfoProvider dataInfoProvider, IDataSessionProvider dataSessionProvider) {
			ServerResult<ColoringServerSession> coloringServerResult = GetSession(new Key(dashboardId));
			if (coloringServerResult.Session.Cache == null) {
				IEnumerable<DashboardItem> globalyColoredItems = dashboardItemFactory.CreateDashboardItems(dashboardId).Where(item => {
					DataDashboardItem dataItem = item as DataDashboardItem;
					return dataItem != null && dataItem.IsGloballyColored;
				});
				ColorRepository colorRepository = colorRepositoryFactory.CreateColorRepository(dashboardId);
				IColorSchemeProvider colorSchemeProvider = new DashboardColorSchemeProvider(globalyColoredItems, dataSessionProvider, dataInfoProvider);
				coloringServerResult.Session.Cache = ColorSchemeGenerator.GenerateColoringCache(colorSchemeProvider, colorRepository, dataSessionProvider);
			}
			return coloringServerResult.Session.Cache;
		}
		ColorRepository GetLocalColoringCache(DataDashboardItem dashboardItem, IDataSourceInfoProvider dataInfoProvider, IDataSessionProvider dataSessionProvider) {
			ColorRepository colorRepository = dashboardItem.ColorSchemeContainer.Repository;
			IColorSchemeProvider colorSchemeProvider = new DashboardItemColorSchemeProvider(dashboardItem, dataSessionProvider, dataInfoProvider);
			return ColorSchemeGenerator.GenerateColoringCache(colorSchemeProvider, colorRepository, dataSessionProvider);
		}
		#region IColoringServer
		ColorRepository IColoringServer.GetColoringCache(string dashboardId, DataDashboardItem dashboardItem, IDataSourceInfoProvider dataInfoProvider, IDataSessionProvider dataSessionProvider) {
			if (dashboardItem.IsColoringSupported) {
				if (dashboardItem.IsGloballyColored)
					return GetGlobalColoringCache(dashboardId, dataInfoProvider, dataSessionProvider);
				return GetLocalColoringCache(dashboardItem, dataInfoProvider, dataSessionProvider);
			}
			return null;
		}
		#endregion
	}
}
