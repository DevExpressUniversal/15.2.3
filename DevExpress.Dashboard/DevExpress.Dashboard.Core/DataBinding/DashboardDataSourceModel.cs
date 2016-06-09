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
using DevExpress.DataAccess.Wizard.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
namespace DevExpress.DashboardCommon {
	public class DashboardDataSourceModel : DataSourceModel {
		readonly OlapDataSourceModel olapDataSourceModel = new OlapDataSourceModel();
		public DashboardDataSourceModel() {
		}
		public DashboardDataSourceModel(DashboardDataSourceModel other)
			: base(other) {
			this.olapDataSourceModel = other.olapDataSourceModel;
		}
		public override object Clone() {
			return new DashboardDataSourceModel(this);
		}
		public override bool Equals(object obj) {
			DashboardDataSourceModel other = obj as DashboardDataSourceModel;
			if(other == null)
				return false;
			if(DataSourceType != other.DataSourceType)
				return false;
			if(DataSourceType == DashboardDataSourceType.Olap) {
				return olapDataSourceModel.Equals(obj);
			}
			return base.Equals(obj);
		}
		public override int GetHashCode() {
			return 0;
		}
		protected override string ConnectionName {
			get {
				if(DataSourceType == DashboardDataSourceType.Olap)
					return olapDataSourceModel.ConnectionName;
				else
					return base.ConnectionName;
			}
			set {
				if(DataSourceType == DashboardDataSourceType.Olap)
					olapDataSourceModel.ConnectionName = value;
				else
					base.ConnectionName = value;
			}
		}
		protected override IDataConnection DataConnection {
			get {
				if(DataSourceType == DashboardDataSourceType.Olap)
					return olapDataSourceModel.Connection;
				else
					return base.DataConnection;
			}
			set {
				if(DataSourceType == DashboardDataSourceType.Olap)
					olapDataSourceModel.Connection = (OlapDataConnection)value;
				else
					base.DataConnection = value;
			}
		}
		protected override SaveConnectionMethod ShouldSaveConnection {
			get {
				if(DataSourceType == DashboardDataSourceType.Olap)
					return olapDataSourceModel.ShouldSaveConnection;
				else
					return base.ShouldSaveConnection;
			}
			set {
				if(DataSourceType == DashboardDataSourceType.Olap)
					olapDataSourceModel.ShouldSaveConnection = value;
				else
					base.ShouldSaveConnection = value;
			}
		}
	}
}
