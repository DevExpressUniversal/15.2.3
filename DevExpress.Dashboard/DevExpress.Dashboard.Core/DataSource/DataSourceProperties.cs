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
using System.Windows.Forms;
using DevExpress.DataAccess.Native.Data;
#if !DXPORTABLE
using DevExpress.DataAccess.ObjectBinding;
#endif
namespace DevExpress.DashboardCommon.Native {
	public class DataSourceProperties {
#if !DXPORTABLE
		static bool ShouldObjectDSUseFakeData(ObjectDataSource dataSource) {
			Type sourceType = dataSource.DataSource as Type;
			return sourceType != null && dataSource.Constructor == null && !sourceType.IsAbstract && dataSource.DataMember == null;				
		}
#endif
		readonly IDashboardDataSource dataSource;
		public DataSourceProperties(IDashboardDataSource dataSource) {
			this.dataSource = dataSource;
		}
		public bool SummaryTypesSupported { get { return !IsOlap; } }
		public bool DimensionGroupIntervalSupported { get { return !IsOlap; } }
		public bool SortOrderNoneSupported { get { return IsOlap; } }
		public bool SpecificSortModeSupported { get { return IsOlap; } }
		public bool IsSpecificValueFormatSupported { get { return !IsOlap; } }
		public bool DataLoadingSupported { get { return !IsOlap; } }
		public bool RangeFilterSupported { get { return !IsOlap; } }
		public bool DistinctCountSupported {
			get {
#if !DXPORTABLE
				return dataSource.SqlDataProvider != null || dataSource.DataProcessingMode == DataProcessingMode.Client;
#else
				return false;
#endif
			}
		}
		public bool ShouldProvideFakeData {
			get {
#if !DXPORTABLE
				DashboardObjectDataSource objectDataSource = dataSource as DashboardObjectDataSource;
				if(objectDataSource != null) {
					return IsVSDesignMode && (objectDataSource.DataSource is BindingSource || 
											!string.IsNullOrEmpty(objectDataSource.DataSchema) || 
											ShouldObjectDSUseFakeData(objectDataSource));
				}
#endif
				return false;
			}
		}
		internal bool IsVSDesignMode { get { return Helper.IsComponentVSDesignMode(dataSource); } }
		public virtual bool IsOlap {
			get {
#if !DXPORTABLE
				return dataSource.OlapDataProvider != null;
#else
				return false;
#endif
			}
		}
		public bool IsMultipleDataMemberSupported {
			get {
#if !DXPORTABLE
				return dataSource is DashboardSqlDataSource || dataSource is DashboardEFDataSource;
#else
				return false;
#endif
			}
		}
		public bool CalculatedFieldsSupported {
			get {
#if !DXPORTABLE
				return dataSource is DashboardSqlDataSource || dataSource is DashboardObjectDataSource || dataSource is DashboardExcelDataSource;
#else
				return false;
#endif
			}
		}
	}
	public class OlapDataSourceProperties : DataSourceProperties {
		public override bool IsOlap { get { return true; } }
		public OlapDataSourceProperties(IDashboardDataSource dataSource)
			: base(dataSource) {
		}
	}
}
