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
using System.Globalization;
using System.Linq;
using DevExpress.Data.Entity;
using DevExpress.Data.WizardFramework;
using DevExpress.DataAccess.EntityFramework;
using DevExpress.DataAccess.Localization;
using DevExpress.DataAccess.Native;
using DevExpress.DataAccess.Wizard.Model;
using DevExpress.DataAccess.Wizard.Services;
using DevExpress.DataAccess.Wizard.Views;
using DevExpress.Entity.Model;
using DevExpress.Entity.ProjectModel;
namespace DevExpress.DataAccess.Wizard.Presenters {
	public class ChooseEFContextPage<TModel> : WizardPageBase<IChooseEFContextPageView, TModel>
		where TModel : IEFDataSourceModel {
		readonly IWizardRunnerContext context;
		bool shouldShowConnectionList;
		IEntityFrameworkModelHelper modelHelper;
		ISolutionTypesProvider solutionTypesProvider;
		EFDataConnection dataConnection;
		public ChooseEFContextPage(IChooseEFContextPageView view, IWizardRunnerContext context, ISolutionTypesProvider solutionTypesProvider, IConnectionStringsProvider connectionStringsProvider)
			: base(view) {
			this.context = context;
			SolutionTypesProvider = solutionTypesProvider;
			ConnectionStringsProvider = connectionStringsProvider;
		}
		public override bool MoveNextEnabled { get { return GetDataConnection() != null; } }
		protected virtual IExceptionHandler ExceptionHandler { get { return context.CreateExceptionHandler(ExceptionHandlerKind.Default); } }
		ISolutionTypesProvider SolutionTypesProvider { get; set; }
		IConnectionStringsProvider ConnectionStringsProvider { get; set; }
		public override void Begin() {
			this.modelHelper = Model.ModelHelper;
			this.solutionTypesProvider = SolutionTypesProvider;
			View.BrowseForAssembly += View_BrowseForAssembly;
			View.ContextNameChanged += View_ContextNameChanged;
			View.Initialize();
			View.RefreshContextList(this.modelHelper.Containers);
			if(Model.ConnectionParameters == null)
				Model.ConnectionParameters = new EFConnectionParameters();
			if(Model.ConnectionParameters.Source != null)
				View.ContextName = Model.ConnectionParameters.Source.FullName;
			else {
				IContainerInfo container = this.modelHelper.Containers.FirstOrDefault();
				if(container != null)
					View.ContextName = container.FullName;
			}
			this.shouldShowConnectionList = (ConnectionStringsProvider.GetConnections().Count() + ConnectionStringsProvider.GetConfigFileConnections().Count()) > 0;
		}
		void View_ContextNameChanged(object sender, ContextNameChangedEventArgs e) {
			RaiseChanged();
		}
		void View_BrowseForAssembly(object sender, BrowseForAssemblyEventArgs e) {
			if(!this.modelHelper.OpenAssembly(e.AssemblyPath, ExceptionHandler))
				return;
			View.RefreshContextList(this.modelHelper.Containers);
			if(!this.modelHelper.Containers.Any())
				ShowMessage(DataAccessLocalizer.GetString(DataAccessStringId.WizardNoEFDataContextsMessage));
		}
		public override bool Validate(out string errorMessage) {
			errorMessage = string.Empty;
			try {
				this.dataConnection = GetDataConnection();
				if(this.modelHelper.IsCustomAssembly) {
					SolutionTypesProvider.AddReferenceFromFile(this.modelHelper.CustomAssemblyPath);
					List<string> assemblyNames = new List<string>();
					foreach(string assemblyName in this.modelHelper.ReferencedAssemblies)
						if(!SolutionTypesProvider.IsReferenceExists(assemblyName))
							assemblyNames.Add(assemblyName);
					const string addReferenceMessage = "Please add reference to following assemblies:";
					if(assemblyNames.Count > 0)
						throw new InvalidOperationException(String.Format(CultureInfo.InvariantCulture, "{0} {1}", addReferenceMessage, string.Join("; ", assemblyNames)));
				}
			} catch(Exception exc) {
				ExceptionHandler.HandleException(exc);
				return false;
			}
			return true;
		}
		public override void Commit() {
			View.ContextNameChanged -= View_ContextNameChanged;
			View.BrowseForAssembly -= View_BrowseForAssembly;
			Model.DataConnection = dataConnection;
		}
		public override Type GetNextPageType() {
			return this.shouldShowConnectionList
				? typeof(ChooseEFConnectionStringPage<TModel>)
				: typeof(ConfigureEFConnectionStringPage<TModel>);
		}
		public EFDataConnection GetDataConnection() {
			if(this.modelHelper == null || this.modelHelper.EntityFrameworkModel == null || View.ContextName == null)
				return null;
			IContainerInfo sourceContainer = this.modelHelper.Containers.FirstOrDefault(c => c.FullName == View.ContextName);
			if(sourceContainer == null)
				return null;
			Model.ConnectionParameters.Source = sourceContainer.ResolveType();
			if(this.modelHelper.IsCustomAssembly) {
				if(this.solutionTypesProvider is RuntimeSolutionTypesProvider) {
					if(Model.ConnectionParameters.Source != null) {
						Model.ConnectionParameters = new EFConnectionParameters(Model.ConnectionParameters.Source.FullName, this.modelHelper.CustomAssemblyPath);
						this.solutionTypesProvider = new RuntimeSolutionTypesProvider(Model.ConnectionParameters.Source.Assembly.GetTypes);
					}
				}
			}
			return new EFDataConnection(string.Empty, new EFConnectionParameters(Model.ConnectionParameters)) {
				SolutionTypesProvider = this.solutionTypesProvider,
				ConnectionStringsProvider = ConnectionStringsProvider
			};
		}
		public virtual void ShowMessage(string message) { context.ShowMessage(message); }
	}
}
