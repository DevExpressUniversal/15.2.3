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
using DevExpress.Data;
using DevExpress.Data.Entity;
using DevExpress.DataAccess.Sql;
using DevExpress.DataAccess.UI.Wizard.Services;
using DevExpress.DataAccess.Wizard;
using DevExpress.DataAccess.Wizard.Services;
namespace DevExpress.DataAccess.UI.Wizard.Clients {
	public class SqlDataSourceWizardClientUI : SqlDataSourceWizardClient, ISqlDataSourceWizardClientUI {
		public SqlDataSourceWizardClientUI(IConnectionStorageService connectionProvider,
			IParameterService parameterService)
			: base(connectionProvider, parameterService, null) {}
		public SqlDataSourceWizardClientUI(IConnectionStorageService connectionProvider,
			IParameterService parameterService, IDBSchemaProvider dbSchemaProvider)
			: base(connectionProvider, parameterService, dbSchemaProvider) { }
		public SqlDataSourceWizardClientUI(IConnectionStorageService connectionProvider,
			IParameterService parameterService, IDBSchemaProvider dbSchemaProvider, IConnectionStringsProvider connectionStringsProvider)
			: base(connectionProvider, parameterService, connectionStringsProvider, null, dbSchemaProvider) {}
		public SqlDataSourceWizardClientUI(IConnectionStorageService connectionProvider,
			IParameterService parameterService, IServiceProvider propertyGridServices,
			IDBSchemaProvider dbSchemaProvider)
			: base(connectionProvider, parameterService, propertyGridServices, dbSchemaProvider) { }
		public SqlDataSourceWizardClientUI(IConnectionStorageService connectionProvider,
			IParameterService parameterService, IConnectionStringsProvider connectionStringsProvider, IServiceProvider propertyGridServices,
			IDBSchemaProvider dbSchemaProvider, SqlWizardOptions options, IDisplayNameProvider displayNameProvider)
			: base(connectionProvider, parameterService, connectionStringsProvider, propertyGridServices, dbSchemaProvider, options, displayNameProvider) { }
		#region Implementation of IDataSourceWizardClientUIBase
		public IRepositoryItemsProvider RepositoryItemsProvider { get; set; }
		#endregion
	}
}
