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
using System.Linq;
using System.Reflection;
using DevExpress.Data.WizardFramework;
using DevExpress.DataAccess.Localization;
using DevExpress.DataAccess.Native;
using DevExpress.DataAccess.ObjectBinding;
using DevExpress.DataAccess.Wizard.Model;
using DevExpress.DataAccess.Wizard.Native;
using DevExpress.DataAccess.Wizard.Services;
using DevExpress.DataAccess.Wizard.Views;
using DevExpress.Entity.ProjectModel;
namespace DevExpress.DataAccess.Wizard.Presenters {
	public class ChooseObjectAssemblyPage<TModel> : WizardPageBase<IChooseObjectAssemblyPageView, TModel> where TModel : IObjectDataSourceModel {
		readonly IWizardRunnerContext context;
		readonly ISolutionTypesProvider solutionTypesProvider;
		IDXAssemblyInfo[] dxAssemblyInfos;
		AssemblyViewInfo[] assemblyViewInfos;
		public ChooseObjectAssemblyPage(IChooseObjectAssemblyPageView view, IWizardRunnerContext context, ISolutionTypesProvider solutionTypesProvider) : base(view) {
			this.context = context;
			this.solutionTypesProvider = solutionTypesProvider;
		}
		protected virtual IWaitFormActivator WaitFormActivator { get { return context.WaitFormActivator; } }
		protected virtual IExceptionHandler ExceptionHandler { get { return context.CreateExceptionHandler(ExceptionHandlerKind.Default); } }
		ISolutionTypesProvider SolutionTypesProvider { get { return solutionTypesProvider; } }
		#region Overrides of WizardPageBase<IChooseObjectAssemblyPageView,TModel>
		public override bool MoveNextEnabled { get { return GetResult() != null; } }
		public override Type GetNextPageType() { return ObjectBindingPagesRouter.AfterChooseObjectAssemblyPage<TModel>(); }
		public override void Begin() {
			View.Initialize();
			WaitFormActivator.ShowWaitForm(false, true, true);
			WaitFormActivator.SetWaitFormCaption(DataAccessLocalizer.GetString(DataAccessStringId.LoadingDataPanelText));
			WaitFormActivator.SetWaitFormDescription(string.Empty);
			WaitFormActivator.EnableCancelButton(false);
			Exception exception = null;
			try { dxAssemblyInfos = SolutionTypesProvider.ActiveProjectTypes.ToArray(); }
			catch(Exception ex) {
				exception = new CannotGetTypesException(ex);
				throw;
			}
			finally {
				WaitFormActivator.CloseWaitForm();
				if(exception != null)
					ExceptionHandler.HandleException(exception);
			}
			AssemblyViewInfo selected = null;
			assemblyViewInfos = dxAssemblyInfos.Where(info => info.TypesInfo.Any()).Select(info => {
				AssemblyName name = new AssemblyName(info.AssemblyFullName);
				Assembly assembly = info.TypesInfo.First().ResolveType().Assembly;
				int priority = info.IsProjectAssembly ? 2 : info.IsSolutionAssembly ? 1 : 0;
				var item =
					new AssemblyViewInfo(
						assembly.GetCustomAttributesData()
							.Any(data => data.Constructor.DeclaringType == typeof(HighlightedAssemblyAttribute)),
						name.Name, name.Version, priority);
				if(Model.Assembly != null && string.Equals(info.AssemblyFullName, Model.Assembly.AssemblyFullName))
					selected = item;
				return item;
			}).ToArray();
			View.SetData(assemblyViewInfos, Model.ShowAllState.HasFlag(ShowAllState.Assemblies));
			View.SelectedItem = selected;
			RaiseChanged();
			View.Changed += OnViewOnChanged;
		}
		void OnViewOnChanged(object sender, EventArgs args) { RaiseChanged(); }
		public override void Commit() {
			View.Changed -= OnViewOnChanged;
			Model.Assembly = GetResult();
			if(View.ShowAll)
				Model.ShowAllState |= ShowAllState.Assemblies;
			else
				Model.ShowAllState &= ~ShowAllState.Assemblies;
		}
		public override bool Validate(out string errorMessage) {
			errorMessage = null;
			return GetResult() != null;
		}
		#endregion
		protected IDXAssemblyInfo GetResult() {
			if(dxAssemblyInfos == null)
				return null;
			int index = assemblyViewInfos.ToList().IndexOf(View.SelectedItem);
			if(index < 0)
				return null;
			return dxAssemblyInfos[index];
		}
	}
}
