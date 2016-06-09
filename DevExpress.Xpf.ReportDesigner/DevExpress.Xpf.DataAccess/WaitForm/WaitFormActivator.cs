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
using DevExpress.DataAccess.Native;
using DevExpress.Mvvm;
using DevExpress.Mvvm.Native;
namespace DevExpress.Xpf.DataAccess.WaitForm {
	public class WaitFormActivator : IWaitFormActivator {
		readonly WaitFormState state;
		readonly Action<Action<ISplashScreenService>> doWithSplashScreenService;
		readonly DelegateCommand cancelCommand;
		bool canCancel;
		ISupportCancel waitFormObject;
		public WaitFormActivator(Action<Action<ISplashScreenService>> doWithSplashScreenService) {
			this.doWithSplashScreenService = doWithSplashScreenService;
			this.cancelCommand = new DelegateCommand(() => waitFormObject.Do(x => x.Cancel()), () => canCancel, false);
			this.state = WaitFormState.Create(this.cancelCommand);
		}
		void IWaitFormActivator.ShowWaitForm(bool fadeIn, bool fadeOut, bool useDelay) {
			doWithSplashScreenService(splashScreenService => {
				splashScreenService.ShowSplashScreen();
				splashScreenService.SetSplashScreenState(state);
			});
		}
		void IWaitFormActivator.CloseWaitForm() {
			CloseWaitForm(false);
		}
		void IWaitFormActivator.CloseWaitForm(bool throwException, int delay, bool waitForClose) {
			CloseWaitForm(throwException);
		}
		void CloseWaitForm(bool throwException) {
			doWithSplashScreenService(splashScreenService => {
				if(throwException || splashScreenService.IsSplashScreenActive)
					splashScreenService.HideSplashScreen();
			});
		}
		void IWaitFormActivator.EnableCancelButton(bool enable) {
			canCancel = enable;
			cancelCommand.RaiseCanExecuteChanged();
		}
		void IWaitFormActivator.SetWaitFormObject(ISupportCancel waitFormObject) {
			this.waitFormObject = waitFormObject;
		}
		void IWaitFormActivator.SetWaitFormCaption(string caption) {
			state.Caption = caption;
		}
		string description;
		bool showDescription;
		void IWaitFormActivator.EnableWaitFormDescription(bool show) {
			showDescription = show;
			UpdateDescription();
		}
		void IWaitFormActivator.SetWaitFormDescription(string message) {
			description = message;
			UpdateDescription();
		}
		void UpdateDescription() {
			state.Description = showDescription ? description : null;
		}
	}
}
