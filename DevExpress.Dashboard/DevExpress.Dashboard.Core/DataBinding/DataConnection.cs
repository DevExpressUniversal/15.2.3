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

using System.Collections.Generic;
using DevExpress.DataAccess.ConnectionParameters;
using DevExpress.DataAccess.Native;
using DevExpress.DataAccess.Sql;
using DevExpress.Xpo.DB;
namespace DevExpress.DataAccess {
	public class ReferenceCounter {
		int refCount;
		public bool HasReferences { get { return this.refCount > 0; } }
		public void AddReference() {
			this.refCount++;
		}
		public void RemoveReference() {
			this.refCount--;
		}
	}
	public class DataConnection : SqlDataConnection, IReferenceCountObject, INamedItem {
		readonly ReferenceCounter counter = new ReferenceCounter();
		public DataConnection() {
		}
		public DataConnection(string name, DataConnectionParametersBase connectionParameters)
			: base(name, connectionParameters) {
		}
#if DEBUGTEST
		internal DataConnection(IDataStore dataStore)
			: base(dataStore) {
		}
#endif
		public void AddReference() {
			this.counter.AddReference();
		}
		public void RemoveReference() {
			this.counter.RemoveReference();
			if(!this.counter.HasReferences)
				Dispose(true);
		}
		public override DBSchema GetDBSchema() {
			if(this.dbSchema != null)
				return this.dbSchema;
			if(SchemaExplorer == null)
				return new DBSchema(new DBTable[] {}, new DBTable[] {});
			DBTable[] tables = GetDBSchemaTables(SchemaExplorer);
			DBTable[] views = GetDBSchemaViews(SchemaExplorer);
			this.dbSchema = new DBSchema(tables, views);
			return this.dbSchema;
		}
		public override DBSchema GetDBSchema(string[] tableList) {
			if(SchemaExplorer == null)
				return new DBSchema(new DBTable[] {}, new DBTable[] {});
			List<DBTable> tables = new List<DBTable>();
			List<DBTable> views = new List<DBTable>();
			GetTablesAndViewsDBSchema(tableList, SchemaExplorer, tables, views);
			return new DBSchema(tables.ToArray(), views.ToArray());
		}
		string INamedItem.Name { get { return Name; } set { Name = value; } }
	}
}
