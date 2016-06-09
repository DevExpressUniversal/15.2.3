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

using DevExpress.DashboardWin.ServiceModel;
using DevExpress.Data.Entity;
using DevExpress.DataAccess.Native.Excel;
using DevExpress.DataAccess.Sql;
using DevExpress.DataAccess.UI.Wizard.Clients;
using DevExpress.DataAccess.Wizard;
using DevExpress.DataAccess.Wizard.Services;
using DevExpress.Entity.ProjectModel;
using DevExpress.Utils.Design;
using System;
namespace DevExpress.DashboardWin.Native {
	public interface IDashboardDataSourceWizardClientUI : IDataSourceWizardClientUI {
		ISupportedDataSourceTypesService DataSourceTypesService { get; }
		IDTEService DTEService { get; }
	}
	public class DashboardDataSourceWizardClientUI : DataSourceWizardClientUI, IDashboardDataSourceWizardClientUI {
		public ISupportedDataSourceTypesService DataSourceTypesService { get; private set; }
		public IDTEService DTEService { get; private set; }
		public DashboardDataSourceWizardClientUI(IConnectionStorageService connectionStorageService, IConnectionStringsProvider connectionStringsProvider)
			: this(connectionStorageService, null, null, connectionStringsProvider, null, null, null, null, null, SqlWizardOptions.None, DataSourceTypes.All, null) {
		}
		public DashboardDataSourceWizardClientUI(IConnectionStorageService connectionStorageService, IParameterService parameterService, ISolutionTypesProvider solutionTypesProvider,
			IConnectionStringsProvider connectionStringsProvider, IDBSchemaProvider dbSchemaProvider, IDataSourceNameCreationService dataSourceNameCreationService,
			ISupportedDataSourceTypesService dataSourceTypesService, IDTEService DTEService, IServiceProvider propertyGridServices, SqlWizardOptions sqlWizardOptions, 
			DataSourceTypes dataSourceTypes, ICustomQueryValidator customQueryValidator)
			: base(connectionStorageService, parameterService, solutionTypesProvider, connectionStringsProvider, dbSchemaProvider, dataSourceNameCreationService, propertyGridServices,
			OperationMode.Both, sqlWizardOptions, null, new ExcelSchemaProvider(), dataSourceTypes, null, customQueryValidator) {
				DataSourceTypesService = dataSourceTypesService;
				this.DTEService = DTEService;
		}
	}
}
