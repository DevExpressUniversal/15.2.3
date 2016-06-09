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
using System.Text;
using System.Data;
using System.ComponentModel;
namespace DevExpress.XtraReports.Serialization {
	public static class DataSourceHelper {
		public static object ConvertToSerializableDataSource(object dataSource) {
			DataSet dataSet = ConvertToDataSet(dataSource);
			return dataSet == null ? (dataSource as IComponent) : dataSet;
		}
		public static DataSet ConvertToDataSet(object dataSource) {
			DataSet dataSet = dataSource as DataSet;
			if(dataSet != null)
				return dataSet;
			DataTable dataTable = dataSource as DataTable;
			if(dataTable != null)
				return dataTable.DataSet;
			DataViewManager dataViewManager = dataSource as DataViewManager;
			if(dataViewManager != null)
				return dataViewManager.DataSet;
			DataView dataView = dataSource as DataView;
			if(dataView != null && dataView.Table != null)
				return dataView.Table.DataSet;
			return null;
		}
	}
}
namespace DevExpress.XtraReports.Serialization {
	using System.ComponentModel.Design;
	public static class SerializationUtils {
		public static bool IsFakedComponent(object obj) {
			IComponent comp = obj as IComponent;
			if(comp == null || comp.Site == null)
				return true;
			IDesignerHost designerHost = (IDesignerHost)comp.Site.GetService(typeof(IDesignerHost));
			if(designerHost == null)
				return true;
			return designerHost.RootComponent.Site.GetType() != comp.Site.GetType();
		}
	}
}
