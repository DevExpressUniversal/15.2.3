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

using DevExpress.DashboardCommon.Native;
using DevExpress.XtraPivotGrid.Data;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
namespace DevExpress.DashboardCommon.DataProcessing {
	public class DataSourceModel {
		public DataSourceInfo DataSourceInfo { get; set; }
		public IList ListSource { get; set; }
		public bool FakeData { get; set; }
		public IPivotGridDataSource PivotDataSource { get; set; }
		public IStorage Storage { get {
			IStorage storage = DataSourceInfo.GetStorage();
#if !DXPORTABLE
			if(FakeData) {
				IDashboardDataSource ds = new DashboardObjectDataSource();
				ITypedList typedList = (ITypedList)ListSource;
				PropertyDescriptorCollection pds = typedList.GetItemProperties(null);
				((IExternalSchemaConsumer)ds).SetSchema(null, pds.Cast<PropertyDescriptor>().Select(p => p.Name).ToArray());
				ds.Data = ListSource;
				storage = ds.GetStorage(null);
			}
#endif
			return storage;
		} }
		public DataProcessingMode DataProcessingMode { get { return DataSourceInfo.GetDataProcessingMode(); } }
		public bool IsOlap { get { return DataSourceInfo.GetIsOlap(); } }
		public DataSourceModel(IDashboardDataSource dataSource, string dataMember, IList listSource, IPivotGridDataSource pivotDataSource) :
			this(new DataSourceInfo(dataSource, dataMember), listSource, pivotDataSource) {
		}
		public DataSourceModel(DataSourceInfo dashboardDataSource, IList listSource, IPivotGridDataSource pivotDataSource) {
			DXContract.Requires(dashboardDataSource != null);
			this.DataSourceInfo = dashboardDataSource;			
			this.ListSource = listSource;
			this.PivotDataSource = pivotDataSource;
		}		
	}
}
