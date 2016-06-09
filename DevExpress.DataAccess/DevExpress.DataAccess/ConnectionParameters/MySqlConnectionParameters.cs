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
using DevExpress.DataAccess.Native.Sql;
using DevExpress.Xpo.Helpers;
namespace DevExpress.DataAccess.ConnectionParameters {
	public class MySqlConnectionParameters : SqlServerConnectionParametersBase, IConnectionPageEx {
		public const string DefaultPort = "3306";
		public string Port { get; set; }
		public MySqlConnectionParameters() {
		}
		public MySqlConnectionParameters(string serverName, string databaseName, string userName, string password)
			: base(serverName, databaseName, userName, password) {
		}
		public MySqlConnectionParameters(string serverName, string databaseName, string userName, string password, string port)
			: this(serverName, databaseName, userName, password) {
			Port = port;
		}
		internal override void Assign(IConnectionPage source) {
			base.Assign(source);
			IConnectionPageEx connectionPageEx = source as IConnectionPageEx;
			if(connectionPageEx != null)
				Port = connectionPageEx.Port;
		}
		bool ShouldSerializePort() {
			return !string.IsNullOrEmpty(Port) && !string.Equals(Port, DefaultPort, StringComparison.InvariantCulture);
		}
	}
}
