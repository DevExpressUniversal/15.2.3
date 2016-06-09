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

using System.Linq;
using DevExpress.Data.Entity;
using DevExpress.DataAccess.EntityFramework;
using DevExpress.DataAccess.Wizard.Services;
namespace DevExpress.DataAccess.Wizard.Model {
	public class EFDataSourceModel : DataSourceModelWithConnectionBase, IEFDataSourceModel {
		public EFDataSourceModel() {
			ConnectionParameters = new EFConnectionParameters();
			ConnectionStringLocation = DataConnectionLocation.None;
		}
		public EFDataSourceModel(EFDataConnection connection) {
			DataConnection = connection;
			ConnectionName = connection.Name;
			ConnectionParameters = connection.CreateDataConnectionParameters();
			ConnectionStringLocation = !string.IsNullOrEmpty(ConnectionParameters.ConnectionStringName) ? DataConnectionLocation.SettingsFile : DataConnectionLocation.None;
		}
		public EFDataSourceModel(EFDataSourceModel other)
			: base(other) {
			DataConnection = other.DataConnection;
			ConnectionParameters = other.ConnectionParameters;
			ConnectionStringLocation = other.ConnectionStringLocation;
			StoredProceduresInfo = other.StoredProceduresInfo != null ? other.StoredProceduresInfo.Select(p => new EFStoredProcedureInfo(p)).ToArray() : null;
			DataMember = other.DataMember;
			ModelHelper = other.ModelHelper;
		}
		public override object Clone() {
			return new EFDataSourceModel(this);
		}
		public override bool Equals(object obj) {
			EFDataSourceModel other = obj as EFDataSourceModel;
			if(other == null)
				return false;
			if(!EFConnectionParameters.EqualityComparer.Equals(ConnectionParameters, other.ConnectionParameters))
				return false;
			if(!object.ReferenceEquals(DataConnection, other.DataConnection))
				return false;
			if(DataConnection != null && !EFConnectionParameters.EqualityComparer.Equals(DataConnection.ConnectionParameters, other.DataConnection.ConnectionParameters))
				return false;
			if(ConnectionStringLocation != other.ConnectionStringLocation)
				return false;
			return base.Equals(obj) && EFStoredProcedureInfo.EqualityComparer.Equals(StoredProceduresInfo, other.StoredProceduresInfo);
		}
		public override int GetHashCode() {
			return 0;
		}
		#region IEFDataSourceModel Members
		public string DataMember { get; set; }
		public EFDataConnection DataConnection { get; set; }
		public IEntityFrameworkModelHelper ModelHelper { get; set; }
		public EFConnectionParameters ConnectionParameters { get; set; }
		public DataConnectionLocation ConnectionStringLocation { get; set; }
		public EFStoredProcedureInfo[] StoredProceduresInfo { get; set; }
		#endregion
		#region IDataComponentModel Members
		IDataConnection IDataComponentModelWithConnection.DataConnection {
			get { return DataConnection; }
			set { DataConnection = (EFDataConnection)value; }
		}
		#endregion
	}
}
