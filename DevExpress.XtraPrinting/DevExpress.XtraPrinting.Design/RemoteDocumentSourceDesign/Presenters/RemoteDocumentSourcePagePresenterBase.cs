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
using DevExpress.Data.WizardFramework;
using DevExpress.XtraPrinting.Design.RemoteDocumentSourceDesign.Views;
namespace DevExpress.XtraPrinting.Design.RemoteDocumentSourceDesign.Presenters {
	public abstract class RemoteDocumentSourcePagePresenterBase<TView> : IRemoteDocumentSourceWizardPage
		where TView : IPageView {
		readonly TView view;
		public abstract bool FinishEnabled { get; }
		public abstract bool MoveNextEnabled { get; }
		public RemoteDocumentSourceModel Model { get; set; }
		object IWizardPage<RemoteDocumentSourceModel>.PageContent {
			get { return View; }
		}
		protected TView View { get { return view; } }
		public event EventHandler Changed;
		public event EventHandler<WizardPageErrorEventArgs> Error;
		protected RemoteDocumentSourcePagePresenterBase(TView view) {
			this.view = view;
		}
		public abstract void Begin();
		public abstract void Commit();
		public abstract Type GetNextPageType();
		bool IWizardPage<RemoteDocumentSourceModel>.Validate(out string errorMessage) {
			errorMessage = null;
			return true;
		}
		protected void RaiseChanged() {
			if(Changed != null) {
				Changed(this, EventArgs.Empty);
			}
		}
		protected void RaiseError(string errorMessage) {
			if(Error != null) {
				Error(this, new WizardPageErrorEventArgs(errorMessage));
			}
		}
		public virtual void ValidatePage(Action ifValidAction) {
			ifValidAction();
		}
	}
}
