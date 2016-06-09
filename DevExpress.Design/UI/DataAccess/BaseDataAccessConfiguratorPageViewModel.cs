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

namespace DevExpress.Design.DataAccess.UI {
	using DevExpress.Design.UI;
	abstract class BaseDataAccessConfiguratorPageViewModel : WpfViewModelBase, IDataAccessConfiguratorPageViewModel {
		IDataAccessConfiguratorContext contextCore;
		public BaseDataAccessConfiguratorPageViewModel(IViewModelBase parentViewModel, IDataAccessConfiguratorContext context)
			: base(parentViewModel) {
			AssertionException.IsNotNull(context);
			this.contextCore = context;
			ItemDoubleClickCommand = new WpfDelegateCommand(OnItemDoubleClick);
		}
		IDataAccessConfiguratorContext IStepByStepConfiguratorPageViewModel<IDataAccessConfiguratorContext>.Context {
			get { return contextCore; }
		}
		void IStepByStepConfiguratorPageViewModel<IDataAccessConfiguratorContext>.Enter() {
			OnEnter(contextCore);
		}
		void IStepByStepConfiguratorPageViewModel<IDataAccessConfiguratorContext>.Leave() {
			OnLeave(contextCore);
		}
		bool IStepByStepConfiguratorPageViewModel<IDataAccessConfiguratorContext>.IsCompleted {
			get { return CalcIsCompleted(contextCore); }
		}
		public ICommand<object> ItemDoubleClickCommand { get; private set; }
		void OnItemDoubleClick() {
			var parentViewModel = GetParentViewModel<IDataAccessConfiguratorViewModel>();
			if(parentViewModel != null) {
				if(parentViewModel.NextPageCommand.CanExecute(this))
					parentViewModel.NextPageCommand.Execute(this);
			}
		}
		protected abstract void OnEnter(IDataAccessConfiguratorContext context);
		protected abstract void OnLeave(IDataAccessConfiguratorContext context);
		protected abstract bool CalcIsCompleted(IDataAccessConfiguratorContext context);
		protected void UpdateIsCompleted() {
			RaisePropertyChanged("IsCompleted");
		}
	}
}
