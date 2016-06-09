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
using System.ComponentModel;
using DevExpress.DataAccess.Native;
using DevExpress.DataAccess.Wizard.Services;
using DevExpress.Mvvm;
using DevExpress.Mvvm.DataAnnotations;
using DevExpress.Xpf.DataAccess.WaitForm;
namespace DevExpress.Xpf.DataAccess.DataSourceWizard {
	public abstract class DataSourceWizardPage : ISupportWizard, ISupportWizardNextCommand, ISupportWizardBackCommand, ISupportWizardCancelCommand, ISupportWizardFinishCommand {
		protected readonly DataSourceWizardModelBase model;
		protected DataSourceWizardPage(DataSourceWizardModelBase model) {
			this.model = model;
			model.AddPage(this);
		}
		public virtual bool CanGoForward { get; protected internal set; }
		void ISupportWizardNextCommand.OnGoForward(CancelEventArgs e) {
			OnGoForward(e);
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		protected virtual void OnGoForward(CancelEventArgs e) { }
		void ISupportWizardBackCommand.OnGoBack(CancelEventArgs e) {
			OnGoBack(e);
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		protected virtual void OnGoBack(CancelEventArgs e) {
			model.NavigateToPreviousPage();
		}
		void ISupportWizard.NavigateToNextPage(WizardController wizardController) {
			NavigateToNextPage(wizardController);
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		protected virtual void NavigateToNextPage(WizardController wizardController) {
			model.NavigateToNextPage();
		}
		void ISupportWizardCancelCommand.OnCancel(CancelEventArgs e) {
			OnCancel(e);
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		protected virtual void OnCancel(CancelEventArgs e) {
			e.Cancel = !model.Cancel();
		}
		public virtual bool CanFinish { get; protected internal set; }
		void ISupportWizardFinishCommand.OnFinish(CancelEventArgs e) {
			OnFinish(e);
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		protected virtual void OnFinish(CancelEventArgs e) {
			e.Cancel = !model.Finish();
		}
		public void Initialize() { }
		public event EventHandler Changed;
		public event EventHandler ConnectionParametersChanged {
			add { Changed += value; }
			remove { Changed -= value; }
		}
		protected void RaiseChanged() {
			if(Changed != null)
				Changed(this, EventArgs.Empty);
		}
		protected class RaiseChangedAttribute : BindablePropertyAttribute {
			public RaiseChangedAttribute() {
				OnPropertyChangedMethodName = "RaiseChanged";
			}
		}
	}
}
