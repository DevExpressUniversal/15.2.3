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
using System.Linq;
using DevExpress.Data.WizardFramework;
using DevExpress.DataAccess.Native;
using DevExpress.DataAccess.Sql;
using DevExpress.DataAccess.Wizard.Model;
using DevExpress.DataAccess.Wizard.Native;
using DevExpress.DataAccess.Wizard.Services;
using DevExpress.DataAccess.Wizard.Views;
using DevExpress.Entity.ProjectModel;
namespace DevExpress.DataAccess.Wizard.Presenters {
	public class ChooseDataSourceTypePage<TModel> : WizardPageBase<IChooseDataSourceTypePageView, TModel>
		where TModel : IDataSourceModel {
		readonly IWizardRunnerContext context;
		public override bool FinishEnabled { get { return false; } }
		public override bool MoveNextEnabled { get { return true; } }
		public ISolutionTypesProvider SolutionTypesProvider { get; set; }
		protected IEnumerable<SqlDataConnection> DataConnections { get; set; }
		protected virtual IWaitFormActivator WaitFormActivator { get { return context.WaitFormActivator; } }
		protected virtual IExceptionHandler ExceptionHandler { get { return context.CreateExceptionHandler(ExceptionHandlerKind.Default); } }
		public ChooseDataSourceTypePage(IChooseDataSourceTypePageView view, IWizardRunnerContext context, IEnumerable<SqlDataConnection> dataConnections, ISolutionTypesProvider solutionTypesProvider)
			: base(view) {
			this.context = context;
			DataConnections = dataConnections;
			SolutionTypesProvider = solutionTypesProvider;
		}
		public override void Begin() {
			View.DataSourceType = Model.DataSourceType;
		}
		public override bool Validate(out string errorMessage) {
			if(View.DataSourceType == DataSourceType.Entity) {
				IEFDataSourceModel efDataSourceModel = Model;
				if(efDataSourceModel.ModelHelper == null) {
					IEntityFrameworkModelHelper entityFrameworkModelHelper = EntityFrameworkModelHelper.Create(SolutionTypesProvider, ExceptionHandler, WaitFormActivator);
					if(entityFrameworkModelHelper == null) {
						errorMessage = string.Empty;
						return false;
					}
					efDataSourceModel.ModelHelper = entityFrameworkModelHelper;
				}
			}
			return base.Validate(out errorMessage);
		}
		public override void Commit() {
			Model.DataSourceType = View.DataSourceType;
		}
		public override Type GetNextPageType() {
			return DataSourceTypeHelper.GetNextPageType<TModel>(View.DataSourceType, DataConnections.Any());
		}
	}
}
