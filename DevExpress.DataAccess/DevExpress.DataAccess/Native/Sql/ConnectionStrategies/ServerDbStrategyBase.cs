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
namespace DevExpress.DataAccess.Native.Sql.ConnectionStrategies {
	public abstract class ServerDbStrategyBase : DbStrategyBase {
		public override ConnectionParameterEdits GetEditsSet(IConnectionParametersControl control) {
			return ConnectionParameterEdits.ServerName
				   | ConnectionParameterEdits.UserName
				   | ConnectionParameterEdits.Password
				   | ConnectionParameterEdits.Database;
		}
		public override string GetDefaultText(ConnectionParameterEdits edit) {
			switch(edit) {
				case ConnectionParameterEdits.ServerName:
					return "localhost";
				case ConnectionParameterEdits.UserName:
				case ConnectionParameterEdits.Password:
				case ConnectionParameterEdits.Database:
					return string.Empty;
				default:
					return null;
			}
		}
		public override string GetConnectionName(IConnectionParametersControl control) {
			string server = control.ServerName;
			string db = control.Database;
			return GetConnectionName(server, db);
		}
		public static string GetConnectionName(string server, string db) {
			const string connection = "Connection";
			if(string.IsNullOrEmpty(server))
				return db == null ? connection : string.Format("{0}_{1}", db, connection);
			return string.IsNullOrEmpty(db)
				? string.Format("{0}_{1}", server, connection)
				: string.Format("{0}_{1}_{2}", server, db, connection);
		}
		public override IEnumerable<string> GetDatabases(IConnectionParametersControl control) {
			return Factory.GetDatabases(control.ServerName, control.UserName, control.Password);
		}
	}
}
