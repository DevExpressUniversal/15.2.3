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

using DevExpress.Mvvm;
using DevExpress.Mvvm.Native;
using DevExpress.Mvvm.UI;
using DevExpress.Xpf.Core;
using System;
using System.Linq;
using System.Windows;
namespace DevExpress.Xpf.Controls {
	public class WizardService : DialogService, IWizardService {
		WizardControl WizardControl { get; set; }
		WizardResult Result = WizardResult.Cancel;
		WizardResult IWizardService.ShowDialog(string title, string documentType, object viewModel, object parameter, object parentViewModel) {
			FrameworkElement view = (FrameworkElement)CreateAndInitializeView(documentType, viewModel, parameter, parentViewModel, this);
			view.Loaded += OnViewLoaded;
			ShowDialog(null, title, view);
			return Result;
		}
		void OnViewLoaded(object sender, RoutedEventArgs e) {
			FrameworkElement view = (FrameworkElement)sender;
			view.Loaded -= OnViewLoaded;
			WizardControl = LayoutTreeHelper.GetVisualChildren(view).OfType<WizardControl>().FirstOrDefault();
			WizardControl.Do(x => x.OwnerService = this);
		}
		protected override void OnDialogWindowClosed(object sender, EventArgs e) {
			base.OnDialogWindowClosed(sender, e);
			WizardControl.Do(x => x.OwnerService = null);
			WizardControl = null;
		}
		internal void SetResult(WizardResult result) {
			Result = result;
		}
		void IWizardService.Back() {
			WizardControl.Do(x => x.Back());
		}
		void IWizardService.Next() {
			WizardControl.Do(x => x.Next());
		}
		void IWizardService.Finish() {
			WizardControl.Do(x => x.Finish());
		}
		void IWizardService.Cancel() {
			WizardControl.Do(x => x.Cancel());
		}
		void IWizardService.NavigateTo(object id) {
			WizardControl.Do(x => x.NavigateTo(id));
		}
	}
}
