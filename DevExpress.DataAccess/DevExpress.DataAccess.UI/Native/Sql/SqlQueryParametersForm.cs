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
using DevExpress.DataAccess.Sql;
using DevExpress.DataAccess.UI.Native.ParametersGrid;
using DevExpress.DataAccess.UI.Wizard.Services;
using DevExpress.DataAccess.Wizard.Native;
using DevExpress.DataAccess.Wizard.Services;
using DevExpress.LookAndFeel;
namespace DevExpress.DataAccess.UI.Native.Sql {
	public class SqlQueryParametersForm : ParametersGridFormBase {
		readonly SqlQuery editQuery;
		readonly IDataConnectionParametersService dataConnectionParametersService;
		readonly ICustomQueryValidator customQueryValidator;
		public SqlQueryParametersForm(SqlQuery query, bool previewDataRowLimit, bool isFixedParameters, UserLookAndFeel lookAndFeel, IServiceProvider propertyGridServices, IParameterService parameterService, IDBSchemaProvider dbSchemaProvider, IDataConnectionParametersService dataConnectionParametersService, IRepositoryItemsProvider repositoryItemsProvider, ICustomQueryValidator validator)
			: base(lookAndFeel, parameterService, dbSchemaProvider) {
			this.customQueryValidator = validator;
			this.editQuery = query;
			this.dataConnectionParametersService = dataConnectionParametersService;
			var parametersGridModel = new ParametersGridModel(query.Parameters);
			var parametersGridViewModel = new ParametersGridViewModel(parametersGridModel, parameterService, propertyGridServices, GetPreviewDataSql, isFixedParameters, previewDataRowLimit);
			this.parametersGrid.Initialize(parametersGridViewModel, propertyGridServices, parameterService, repositoryItemsProvider);
			this.parametersGrid.SetButtons(this.btnAdd, this.btnRemove, this.btnPreview);
			if(isFixedParameters)
				HideAddRemove();
		}
		SqlQueryParametersForm() { }
		object GetPreviewDataSql() {
			var previewContext = new PreviewContext(
				editQuery,
				editQuery.DataSource.Connection,
				EvaluatedParameters,
				dataConnectionParametersService,
				DbSchemaProvider,
				customQueryValidator,
				new WaitFormActivator(this, typeof(WaitFormWithCancel), true),
				new ExceptionHandler(LookAndFeel, this, "Validation"),
				new ConnectionExceptionHandler(this, LookAndFeel),
				new LoaderExceptionHandler(this, LookAndFeel)
				);
			return SqlQueryHelper.LoadPreviewData(previewContext);
		}
	}
}
