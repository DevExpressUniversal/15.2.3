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

using System.Collections.Generic;
using DevExpress.PivotGrid.QueryMode;
using DevExpress.PivotGrid.Xmla;
using DevExpress.XtraPivotGrid;
using DevExpress.XtraPivotGrid.Data;
namespace DevExpress.PivotGrid.OLAP {
	public class XmlaMetadata : OLAPMetadata {
		new XmlaConnection Connection { get { return (XmlaConnection)base.Connection; } }
		protected internal override string ServerVersion { get { return Connection.ServerVersion; } }
		public string ConnectionString {
			get { return base.FullConnectionString; }
			set { base.FullConnectionString = value; }
		}
		public XmlaMetadata() : base() { }
		protected override IQueryExecutor<OLAPCubeColumn> CreateQueryExecutor() {
			return new OLAPDataSourceQueryExecutor(new XmlaCellSetParser(this), this);
		}
		protected override void OpenConnection(IOLAPConnection connection) {
			XmlaConnection xmlaConnection = (XmlaConnection)connection;
			try {
				base.OpenConnection(connection);
			} catch(XmlaErrorResponseException e) {
				if(e.ErrorCode == 3238789130) {
					xmlaConnection.SessionId = null;
					base.OpenConnection(xmlaConnection);
				}
			} finally {
				SessionID = xmlaConnection.SessionId;
			}
		}
		protected internal override bool PopulateColumnsCore(IDataSourceHelpersOwner<OLAPCubeColumn> owner) {
			return PopulateColumns(false, (IOLAPHelpersOwner)owner);
		}
		protected internal override IOLAPConnection CreateConnection(IOLAPConnectionSettings connectionSettings, IOLAPMetadata data) {
			XmlaConnection connection = new XmlaConnection(data, connectionSettings);
			return connection;
		}
		public override bool DimensionPropertiesSupported {
			get { return !OLAPMetadataHelper.IsAS2000(ServerVersion); }
		}
		public override IOLAPRowSet GetShemaRowSet(string name, Dictionary<string, object> restrictions) {
			return XmlaSchemaLoader.Create(Connection).ExecuteSchemaRowSet(name, restrictions);
		}
	}
}
