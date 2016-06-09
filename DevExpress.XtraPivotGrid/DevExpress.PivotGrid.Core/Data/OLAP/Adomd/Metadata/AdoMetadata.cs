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
using System.Data;
using DevExpress.PivotGrid.OLAP.AdoWrappers;
using DevExpress.PivotGrid.QueryMode;
using DevExpress.XtraPivotGrid;
using DevExpress.XtraPivotGrid.Data;
namespace DevExpress.PivotGrid.OLAP {
	public class AdoMetadata : OLAPMetadata {
		readonly AdomdVersion adomdVersion;
		protected internal override string ServerVersion { get { return Connection.ServerVersion; } }
		public  new AdomdConnection Connection { get { return (AdomdConnection)base.Connection; } }
		protected AdomdVersion AdomdVersion { get { return adomdVersion; } }
		public AdoMetadata() : this(AdomdMetaGetter.GetNewestAdomdVersion()) { }
		public AdoMetadata(AdomdVersion adomdVersion) : base() {
			this.adomdVersion = adomdVersion;
		}
		protected override IQueryExecutor<OLAPCubeColumn> CreateQueryExecutor() {
			return new OLAPDataSourceQueryExecutor(new AdoCellSetParser(this), this);
		}
		protected override void OpenConnection(IOLAPConnection connection) {
			AdomdConnection adomdConnection = (AdomdConnection)connection;
			try {
				base.OpenConnection(adomdConnection);
			} catch(AdomdConnectionException e) {
				if((e.InnerException is XmlaException) && ((XmlaException)e.InnerException).IsIncorrectSessionException) {
					adomdConnection.SessionID = null;
					base.OpenConnection(adomdConnection);
				}
			} finally {
				SessionID = adomdConnection.SessionID;
			}
		}
		protected internal override bool PopulateColumnsCore(IDataSourceHelpersOwner<OLAPCubeColumn> owner) {
			return PopulateColumns(true, (IOLAPHelpersOwner)owner);
		}
		protected internal override IOLAPConnection CreateConnection(IOLAPConnectionSettings connectionSettings, IOLAPMetadata data) {
			AdomdConnection connection = new AdomdConnection(AdomdVersion, connectionSettings.ConnectionString);
			return connection;
		}
		public override bool DimensionPropertiesSupported {
			get { return !OLAPMetadataHelper.IsAS2000(ServerVersion); }
		}
		public override IOLAPRowSet GetShemaRowSet(string schemaName, Dictionary<string, object> restrictions) {
			return AdoMetadataHelper.GetSchemaRowSet(Connection, schemaName, CubeName, restrictions);
		}
	}
}
