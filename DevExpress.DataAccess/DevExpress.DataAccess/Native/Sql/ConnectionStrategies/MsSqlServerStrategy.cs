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
using DevExpress.DataAccess.ConnectionParameters;
using DevExpress.Xpo.DB;
namespace DevExpress.DataAccess.Native.Sql.ConnectionStrategies {
	public class MsSqlServerStrategy : ServerDbStrategyBase {
		#region Overrides of ServerDbStrategyBase
		public override IEnumerable<string> GetDatabases(IConnectionParametersControl control) {
			switch(control.AuthTypeMsSql) {
				case MsSqlAuthorizationType.SqlServer:
					return Factory.GetDatabases(control.ServerName, control.UserName, control.Password);
				case MsSqlAuthorizationType.Windows:
					return Factory.GetDatabases(control.ServerName, string.Empty, string.Empty);
			}
			throw new ArgumentException();
		}
		#endregion
		MSSqlProviderFactory factory;
		#region Overrides of ServerDbStrategyBase
		public override ConnectionParameterEdits GetEditsSet(IConnectionParametersControl control) {
			return base.GetEditsSet(control) | ConnectionParameterEdits.AuthTypeMsSql;
		}
		public override ConnectionParameterEdits GetDisabledEdits(IConnectionParametersControl control) {
			return control.AuthTypeMsSql == MsSqlAuthorizationType.Windows
				? ConnectionParameterEdits.UserName | ConnectionParameterEdits.Password
				: ConnectionParameterEdits.None;
		}
		public override ConnectionParameterEdits Subscriptions { get { return ConnectionParameterEdits.AuthTypeMsSql; } }
		protected override ProviderFactory Factory { get { return this.factory ?? (this.factory = new MSSqlProviderFactory()); } }
		public override int GetDefaultIndex(ConnectionParameterEdits edit) { return edit == ConnectionParameterEdits.AuthTypeMsSql ? 0 : base.GetDefaultIndex(edit); }
		public override DataConnectionParametersBase GetConnectionParameters(IConnectionParametersControl control) {
			switch(control.AuthTypeMsSql) {
				case MsSqlAuthorizationType.SqlServer:
					return new MsSqlConnectionParameters(control.ServerName, control.Database, control.UserName, control.Password, MsSqlAuthorizationType.SqlServer);
				case MsSqlAuthorizationType.Windows:
					return new MsSqlConnectionParameters(control.ServerName, control.Database, string.Empty, string.Empty, MsSqlAuthorizationType.Windows);
			}
			throw new ArgumentException();
		}
		public override void InitializeControl(IConnectionParametersControl control, DataConnectionParametersBase value) {
			MsSqlConnectionParameters p = (MsSqlConnectionParameters)value;
			control.ServerName = p.ServerName;
			control.Database = p.DatabaseName;
			control.UserName = p.UserName ?? string.Empty;
			control.Password = p.Password ?? string.Empty;
			control.AuthTypeMsSql = p.AuthorizationType;
		}
		#endregion
		#region Overrides of DbStrategyBase
		public override bool CanGetDatabases { get { return true; } }
		#endregion
	}
}
