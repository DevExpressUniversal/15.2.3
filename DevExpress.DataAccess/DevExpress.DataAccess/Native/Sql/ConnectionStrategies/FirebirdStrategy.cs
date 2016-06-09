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
using DevExpress.DataAccess.ConnectionParameters;
using DevExpress.Xpo.DB;
namespace DevExpress.DataAccess.Native.Sql.ConnectionStrategies {
	public class FirebirdStrategy : DbStrategyBase {
		FirebirdProviderFactory factory;
		#region Overrides of DbStrategyBase
		public override ConnectionParameterEdits GetEditsSet(IConnectionParametersControl control) {
			return ConnectionParameterEdits.ServerType | ConnectionParameterEdits.UserName | ConnectionParameterEdits.Password |
				   (control.ServerBased
					   ? ConnectionParameterEdits.ServerName | ConnectionParameterEdits.Database
					   : ConnectionParameterEdits.FileName);
		}
		public override ConnectionParameterEdits Subscriptions { get { return ConnectionParameterEdits.ServerType; } }
		protected override ProviderFactory Factory { get { return this.factory ?? (this.factory = new FirebirdProviderFactory()); } }
		public override string GetDefaultText(ConnectionParameterEdits edit) {
			switch(edit) {
				case ConnectionParameterEdits.UserName:
				case ConnectionParameterEdits.Password:
				case ConnectionParameterEdits.ServerName:
				case ConnectionParameterEdits.FileName:
					return string.Empty;
				default:
					return null;
			}
		}
		public override DataConnectionParametersBase GetConnectionParameters(IConnectionParametersControl control) {
			return control.ServerBased
				? new FireBirdConnectionParameters(control.ServerName, control.Database, control.UserName,
					control.Password)
				: new FireBirdConnectionParameters(control.FileName, control.UserName, control.Password);
		}
		public override string GetConnectionName(IConnectionParametersControl control) {
			return control.ServerBased
				? ServerDbStrategyBase.GetConnectionName(control.ServerName, control.Database)
				: FileDbStrategyBase.GetConnectionName(control.FileName);
		}
		public override void InitializeControl(IConnectionParametersControl control, DataConnectionParametersBase value) {
			FireBirdConnectionParameters p = (FireBirdConnectionParameters)value;
			switch(p.ConnectionType) {
				case FireBirdConnectionType.ServerBased:
					control.ServerBased = true;
					control.ServerName = p.ServerName;
					control.Database = p.DatabaseName;
					break;
				case FireBirdConnectionType.Embedded:
					control.ServerBased = false;
					control.FileName = p.FileName;
					break;
				default:
					throw new ArgumentException();
			}
			control.UserName = p.UserName;
			control.Password = p.Password;
		}
		public override int GetDefaultIndex(ConnectionParameterEdits edit) { return edit == ConnectionParameterEdits.ServerType ? 0 : -1; }
		public override bool CanGetDatabases { get { return false; } }
		#endregion
	}
}
