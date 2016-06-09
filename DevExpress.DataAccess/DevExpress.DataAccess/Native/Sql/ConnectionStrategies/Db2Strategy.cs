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

using DevExpress.DataAccess.ConnectionParameters;
using DevExpress.Xpo.DB;
namespace DevExpress.DataAccess.Native.Sql.ConnectionStrategies {
	public class Db2Strategy : ServerDbStrategyBase {
		DB2ProviderFactory factory;
		#region Overrides of ServerDbStrategyBase
		protected override ProviderFactory Factory { get { return this.factory ?? (this.factory = new DB2ProviderFactory()); } }
		public override DataConnectionParametersBase GetConnectionParameters(IConnectionParametersControl control) {
			return new DB2ConnectionParameters(control.ServerName, control.Database, control.UserName, control.Password);
		}
		public override void InitializeControl(IConnectionParametersControl control, DataConnectionParametersBase value) {
			DB2ConnectionParameters p = (DB2ConnectionParameters)value;
			control.ServerName = p.ServerName;
			control.Database = p.DatabaseName;
			control.UserName = p.UserName;
			control.Password = p.Password;
		}
		#endregion
		#region Overrides of DbStrategyBase
		public override bool CanGetDatabases { get { return true; } }
		#endregion
	}
}
