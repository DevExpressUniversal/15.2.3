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

using DevExpress.DashboardCommon;
using DevExpress.DashboardCommon.Native;
using DevExpress.DashboardWin.ServiceModel;
using DevExpress.Utils;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
namespace DevExpress.DashboardWin.Native {
	public class EditColorSchemeFormController {
		public static EditColorSchemeFormController CreateGlobal(IServiceProvider serviceProvider, Dashboard dashboard) {
			IColoringCacheProvider cacheProvider = serviceProvider.RequestServiceStrictly<IDashboardColoringCacheAccessService>().GetColoringCacheProvider();
			return new EditColorSchemeFormController(
				serviceProvider,
				dashboard,
				dashboard.ColorSchemeContainer.Repository,
				() => cacheProvider.GetGlobalColoringCache(),
				new DashboardEditColorSchemeHistoryItemCreator(dashboard));
		}
		public static EditColorSchemeFormController CreateLocal(IServiceProvider serviceProvider, Dashboard dashboard, DataDashboardItem dashboardItem) {
			IColoringCacheProvider cacheProvider = serviceProvider.RequestServiceStrictly<IDashboardColoringCacheAccessService>().GetColoringCacheProvider();
			return new EditColorSchemeFormController(
				serviceProvider,
				dashboard,
				dashboardItem.ColorSchemeContainer.Repository,
				() => cacheProvider.GetLocalColoringCache(dashboardItem.ComponentName),
				new EditLocalColorSchemeHistoryItemCreator(dashboardItem));
		}
		readonly IServiceProvider serviceProvider;
		readonly Dashboard dashboard;	
		readonly ColorRepository userColors;
		readonly Func<ColorRepository> getCacheCallcack;
		ColorRepository cache;
		DashboardPalette palette = new DashboardPalette();
		bool isChanged = false;
		EditColorSchemeHistoryItemCreatorBase historyItemCreator;
		public int PaletteColorCount { get { return palette.ColorsCount; } }
		public IEnumerable<Color> PaletteColors {
			get {
				for(int i = 0; i < PaletteColorCount; i++)
					yield return palette.GetColor(i);
			}
		}
		public IEnumerable<ColorRepositoryKey> ColorSchemeKeys {
			get {
				return Cache != null
					? Cache.Records.Keys.Union(userColors.Records.Keys).
					  Distinct(new ColorRepositoryKeyComparer()).
					  Where(key => key.DimensionDefinitions.Count > 0)
					: userColors.Records.Keys;
			}
		}
		public bool IsChanged { get { return isChanged; } }
		public IServiceProvider ServiceProvider { get { return serviceProvider; } }
		public ColorRepository UserColors { get { return userColors; } }
		public string CurrencyCultureName { get { return dashboard.CurrencyCultureName; } }
		public IDataSourceInfoProvider DataInfoProvider { get { return dashboard; } }
		public ColorRepository Cache { get { return cache; } }
		EditColorSchemeFormController(IServiceProvider serviceProvider, Dashboard dashboard, ColorRepository userColors, Func<ColorRepository> getCacheCallcack, EditColorSchemeHistoryItemCreatorBase historyItemCreator) {
			Guard.ArgumentNotNull(serviceProvider, "serviceProvider");
			Guard.ArgumentNotNull(dashboard, "dashboard");
			Guard.ArgumentNotNull(userColors, "userColors");
			Guard.ArgumentNotNull(getCacheCallcack, "getCacheCallcack");
			Guard.ArgumentNotNull(historyItemCreator, "historyItemCreator");
			this.serviceProvider = serviceProvider;
			this.dashboard = dashboard;
			this.userColors = userColors.Clone();
			this.getCacheCallcack = getCacheCallcack;
			this.historyItemCreator = historyItemCreator;
			UpdateCache();
		}
		public IEnumerable<ColorTableGridRow> GetUserColors(ColorRepositoryKey repositoryKey) {
			ServerColorTable colorTable;
			if(userColors.Records.TryGetValue(repositoryKey, out colorTable))
				return colorTable.Rows.Select(pair => new ColorTableGridRow() { Key = pair.Key, Value = new DesignerColor(pair.Value) });
			return null;
		}
		public IEnumerable<ColorTableGridRow> GetCacheColors(ColorRepositoryKey repositoryKey) {
			DataSourceInfo dataInfo = DataInfoProvider.GetDataSourceInfo(repositoryKey.DataSourceName, repositoryKey.DataMember);
			IDashboardDataSource dataSource = dataInfo != null ? dataInfo.DataSource : null;
			bool fakeData = dataSource != null && dataSource.GetShouldProvideFakeData();
			if (!fakeData) {
				ServerColorTable colorTable;
				if (Cache != null && Cache.Records.TryGetValue(repositoryKey, out colorTable))
					return colorTable.Rows.Select(pair => new ColorTableGridRow() { Key = pair.Key, Value = new DesignerColor(pair.Value) });
			}
			return null;
		}
		public IList<DataSourceInfo> GetDataSourceInfos() {
			return DataInfoProvider.GetAllDataSourceInfo();
		}
		public DesignerColor GetInheritedColor(ColorRepositoryKey repositoryKey, ColorTableServerKey tableKey) {
			DataSourceInfo dataInfo = DataInfoProvider.GetDataSourceInfo(repositoryKey.DataSourceName, repositoryKey.DataMember);
			if (dataInfo == null)
				return null;
			ColorRepositoryKey dataSourceNeutralKey = new ColorRepositoryKey(repositoryKey.DimensionDefinitions);
			if (!ColorSchemeKeys.Contains(dataSourceNeutralKey, new ColorRepositoryKeyComparer()))
				return null;
			if (!IsColorCustom(repositoryKey, tableKey) && IsColorCustom(dataSourceNeutralKey, tableKey))
				return new InheritedColor(UserColors.Records[dataSourceNeutralKey].Rows[tableKey]);
			return null;
		}
		public bool IsColorCustom(ColorRepositoryKey repositoryKey, ColorTableServerKey tableKey) {
			return IsColorInRepository(userColors, repositoryKey, tableKey);
		}
		public bool IsColorInCache(ColorRepositoryKey repositoryKey, ColorTableServerKey tableKey) {
			return Cache != null && IsColorInRepository(Cache, repositoryKey, tableKey);
		}
		public bool IsTableInCache(ColorRepositoryKey repositoryKey) {
			return Cache != null && Cache.Records.ContainsKey(repositoryKey);
		}
		public void AddColor(ColorRepositoryKey repositoryKey, ColorTableServerKey tableKey) {
			ServerColorTable colorTable;
			if(!userColors.Records.TryGetValue(repositoryKey, out colorTable)) {
				colorTable = new ServerColorTable();
				userColors.Records.Add(repositoryKey, colorTable);
			}
			colorTable.Rows.Add(tableKey, new UserColor(System.Drawing.Color.Empty));
			isChanged = true;
		}
		public void SetColor(ColorRepositoryKey repositoryKey, ColorTableServerKey tableKey, DesignerColor color) {
			ServerColorTable colorTable;
			if(!userColors.Records.TryGetValue(repositoryKey, out colorTable)) {
				colorTable = new ServerColorTable();
				userColors.Records.Add(repositoryKey, colorTable);
			}
			colorTable.Rows[tableKey] = color.GetDefinition();
			isChanged = true;
		}
		public void RemoveColor(ColorRepositoryKey repositoryKey, ColorTableServerKey tableKey) {
			ServerColorTable colorTable;
			if(userColors.Records.TryGetValue(repositoryKey, out colorTable) && colorTable.Rows.ContainsKey(tableKey)) {
				colorTable.Rows.Remove(tableKey);
				if(colorTable.Rows.Count == 0 && IsTableInCache(repositoryKey))
					userColors.Records.Remove(repositoryKey);
				isChanged = true;
			}
		}
		public void AddTable(ColorRepositoryKey key) {
			userColors.Records.Add(key, new ServerColorTable());
			isChanged = true;
		}
		public void RemoveTable(ColorRepositoryKey key) {
			if(userColors.Records.ContainsKey(key)) {
				userColors.Records.Remove(key);
				isChanged = true;
			}
		}
		public void Redo() {
			if(isChanged) {
				IDashboardDesignerHistoryService historyService = serviceProvider.RequestServiceStrictly<IDashboardDesignerHistoryService>();
				IHistoryItem historyItem = historyItemCreator.CreateHistoryItem(userColors);
				historyService.RedoAndAdd(historyItem);
				UpdateCache();
				isChanged = false;
			}			
		}
		public void UpdateCache() {
			cache = getCacheCallcack();
		}
		bool IsColorInRepository(ColorRepository repository, ColorRepositoryKey repositoryKey, ColorTableServerKey tableKey) {
			ServerColorTable colorTable;
			if(repository.Records.TryGetValue(repositoryKey, out colorTable)) {
				if(colorTable.Rows.ContainsKey(tableKey))
					return true;
			}
			return false;
		}
	}
}
