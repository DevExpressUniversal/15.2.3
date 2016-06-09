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
using DevExpress.Data.Entity;
using DevExpress.DataAccess.Native;
using DevExpress.DataAccess.Sql;
using DevExpress.DataAccess.Wizard.Services;
using DevExpress.Utils.Design;
using DevExpress.Data;
using DevExpress.XtraPrinting.Native;
namespace DevExpress.DataAccess.Design {
	public class VSConnectionProviderService : IConnectionProviderService {
		readonly IServiceProvider serviceProvider;
		readonly IConnectionStringsProvider connectionStringsService;
		public VSConnectionProviderService(IServiceProvider serviceProvider) {
			this.serviceProvider = serviceProvider;
			connectionStringsService = serviceProvider.GetService<IConnectionStringsProvider>();
		}
		#region IConnectionProviderService Members
		public SqlDataConnection LoadConnection(string connectionName) {
			if(string.IsNullOrEmpty(connectionName))
				throw new ArgumentException("The connectionName value cannot be null or empty. Specify a proper connection name.");
			SqlDataConnection result = AppConfigHelper.CreateSqlConnectionFromConnectionString(connectionStringsService, connectionName);
			PatchConnectionString(result);
			return result;
		}
		void PatchConnectionString(SqlDataConnection connection) {
			if(connection != null && connection.HasConnectionString)
				connection.ConnectionString = new VSConnectionStringsService(serviceProvider).PatchConnectionString(connection.ConnectionString);
		}
		#endregion
	}
  }
