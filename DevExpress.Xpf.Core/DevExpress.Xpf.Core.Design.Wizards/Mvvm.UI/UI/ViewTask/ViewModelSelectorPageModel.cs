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
using DevExpress.Design.UI;
using System.Windows.Input;
using System.Collections.Generic;
using System.Threading;
using System.Windows;
using DevExpress.Utils.Design;
using DevExpress.Design.DataAccess;
namespace DevExpress.Design.Mvvm.Wizards.UI {
	class ViewModelSelectorPageModel : MvvmConstructorPageViewModelBase {		
		string viewName;
		public ViewModelSelectorPageModel(IViewModelBase parentViewModel, MvvmConstructorContext context)
			: base(parentViewModel, context) {				
		}
		IViewModelLayerService ViewModelLayerService { get { return ServiceContainer.Resolve<IViewModelLayerService>(); } }
		void UpdateViewName() {
			ViewName = GetViewName();
		}
		string GetViewName() {
			if(this.Context.SelectedViewModel == null)
				return ViewModelLayerService.GetViewName(null, null, ViewType.Entity);
			if(this.Context.SelectedViewModel.ViewModelType == ViewModelType.Entity)
				return ViewModelLayerService.GetViewName(this.Context.SelectedViewModel.EntityTypeName, this.Context.SelectedViewModel.Name, ViewType.Entity);
			else
				return ViewModelLayerService.GetViewName(this.Context.SelectedViewModel.EntityTypeName, this.Context.SelectedViewModel.Name, ViewType.Repository);
		}
		public string GetViewFolderName() {
			return this.Context.SelectedViewModel == null ? string.Empty : this.Context.SelectedViewModel.DefaultViewFolderName ?? string.Empty;
		}
		public void Init() {			
			UpdateViewName();
		}
		protected override void OnEnter(MvvmConstructorContext context) {			
			Init();
		}
		protected override void OnLeave(MvvmConstructorContext context) {			
			Context.SelectedViewName = ViewName;
			var templateContext = new TemplateGenerationContext(context);
			if(this.GetParentViewModel<IMvvmConstructorViewModel>().StepDirection == StepByStepDirection.Next) {
				ITemplatesCodeGen codeGen = this.ServiceContainer.Resolve<ITemplatesCodeGen>();
				codeGen.GenerateView(templateContext, SelectedViewModel, this.Context.SelectedViewName, GetViewFolderName(), this.Context.SelectedViewType, true);
			}
		}
		protected override bool CalcIsCompletedCore() {
			bool isViewModelNameValid = !string.IsNullOrEmpty(ViewName);
			return isViewModelNameValid && SelectedViewModel != null;
		}
		public string ViewName {
			get {
				return viewName;
			}
			set {
				if(SetProperty<string>(ref viewName, value, "ViewName"))
					UpdateIsCompleted();
			}
		}
		public IViewModelInfo SelectedViewModel {
			get {
				return Context.SelectedViewModel;
			}
		}
		public string DescriptionText {
			get {
				switch(Context.SelectedViewType) {
					case ViewType.Entity:
						if(Context.SelectedUIType == UIType.WindowsUI)
							return SR_Mvvm.ViewWizard_EntityWinUIViewShortDescription;
						else
						return SR_Mvvm.ViewWizard_EntityViewShortDescription;
					case ViewType.Repository:
						if (Context.SelectedUIType == UIType.WindowsUI)
							return SR_Mvvm.ViewWizard_RepositoryWinUIViewShortDescription;
						else
						return SR_Mvvm.ViewWizard_RepositoryViewShortDescription;
				}
				return SR_Mvvm.ViewWizard_EntityViewShortDescription;
			}
		}
		public override string StepDescription {
			get { return SR_Mvvm.ViewProperties; }
		}
	}
}
