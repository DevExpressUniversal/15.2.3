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
using System.Collections.Generic;
using DevExpress.Design.UI;
using System.Windows.Input;
using System.Diagnostics;
namespace DevExpress.Design.Mvvm.Wizards.UI {
	abstract class MvvmConstructorPageViewModelBase : ViewModelBase, IMvvmConstructorPageViewModel {
		MvvmConstructorContext contextCore;
		public MvvmConstructorPageViewModelBase(IViewModelBase parentViewModel, MvvmConstructorContext context)
			: base(parentViewModel) {
			AssertionException.IsNotNull(context);
			this.contextCore = context;
			NavigateToLearnMore = new WpfDelegateCommand(() => {
				try {
					Process.Start(SR_Mvvm.Wizard_LearnMoreNavigateUri);
				}
				catch (Exception ex) {
					Log.SendException(ex);
				}
			});
		}
		MvvmConstructorContext IStepByStepConfiguratorPageViewModel<MvvmConstructorContext>.Context {
			get { return contextCore; }
		}
		void IStepByStepConfiguratorPageViewModel<MvvmConstructorContext>.Enter() {
			OnEnter(contextCore);
		}
		void IStepByStepConfiguratorPageViewModel<MvvmConstructorContext>.Leave() {
			OnLeave(contextCore);
		}
		bool IStepByStepConfiguratorPageViewModel<MvvmConstructorContext>.IsCompleted {
			get { return CalcIsCompletedCore(); }
		}
		protected MvvmConstructorContext Context { get { return contextCore; } }
		protected abstract void OnEnter(MvvmConstructorContext context);
		protected abstract void OnLeave(MvvmConstructorContext context);
		protected abstract bool CalcIsCompletedCore();
		protected void UpdateIsCompleted() {
			RaisePropertyChanged("IsCompleted");
		}
		public abstract string StepDescription { get; }
		public virtual bool WithUndo {
			get { return true; }
		}
		public ICommand NavigateToLearnMore { get; set; }
	}
}
