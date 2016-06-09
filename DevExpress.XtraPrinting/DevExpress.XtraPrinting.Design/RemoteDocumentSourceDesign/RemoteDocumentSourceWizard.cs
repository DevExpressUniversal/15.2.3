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
using System.Diagnostics;
using DevExpress.Utils;
using DevExpress.Utils.IoC;
using DevExpress.Data.WizardFramework;
using DevExpress.XtraPrinting.Design.RemoteDocumentSourceDesign.Presenters;
namespace DevExpress.XtraPrinting.Design.RemoteDocumentSourceDesign {
	public class RemoteDocumentSourceWizard {
		#region Fields & Properties
		readonly IWizardView view;
		readonly List<IRemoteDocumentSourceWizardPage> pageList;
		readonly RemoteDocumentSourceWizardPageFactory pageFactory;
		readonly TimeMachine<RemoteDocumentSourceModel> timeMachine;
		RemoteDocumentSourceModel resultModel;
		IRemoteDocumentSourceWizardPage currentPage;
		IRemoteDocumentSourceWizardPage CurrentPage {
			get {
				return currentPage;
			}
		}
		public RemoteDocumentSourceModel CurrentStateModel {
			get {
				return timeMachine.CurrentValue;
			}
		}
		void currentPage_Error(object sender, WizardPageErrorEventArgs e) {
			view.ShowError(e.ErrorMessage);
		}
		void currentPage_Changed(object sender, EventArgs e) {
			RefreshView();
		}
		#endregion
		#region Events
		public event EventHandler Completed;
		public event EventHandler Cancelled;
		#endregion
		#region ctor
		public RemoteDocumentSourceWizard(IWizardView view, RemoteDocumentSourceModel model, RemoteDocumentSourceWizardPageFactory pageFactory) {
			if(view == null)
				throw new ArgumentNullException("view");
			if(model == null)
				throw new ArgumentNullException("model");
			if(pageFactory == null)
				throw new ArgumentNullException("pageFactory");
			this.view = view;
			view.Next += new EventHandler(view_Next);
			view.Previous += new EventHandler(view_Previous);
			view.Cancel += new EventHandler(view_Cancel);
			view.Finish += new EventHandler(view_Finish);
			timeMachine = new TimeMachine<RemoteDocumentSourceModel>(model);
			this.pageList = new List<IRemoteDocumentSourceWizardPage>();
			this.pageFactory = pageFactory;
			RefreshView();
		}
		#endregion
		public RemoteDocumentSourceModel GetResultModel() {
			if(resultModel == null) {
				throw new InvalidOperationException("Wizard must be finished before calling this method.");
			}
			return resultModel;
		}
		void view_Finish(object sender, EventArgs e) {
			CurrentPage.ValidatePage(() => {
				CurrentPage.Commit();
				timeMachine.MoveToTheEndOfHistory();
				resultModel = timeMachine.CurrentValue;
				if(Completed != null) {
					Completed(this, EventArgs.Empty);
				}
			});
		}
		void view_Cancel(object sender, EventArgs e) {
			if(Cancelled != null) {
				Cancelled(this, EventArgs.Empty);
			}
		}
		void view_Previous(object sender, EventArgs e) {
			CurrentPage.Commit();
			MoveToPage(pageList[pageList.IndexOf(currentPage) - 1], timeMachine.MoveToThePast, false);
		}
		void view_Next(object sender, EventArgs e) {
			CurrentPage.ValidatePage(() => {
				var nextPage = GetNextPage();
				if(nextPage == null)
					return;
				int currentPageIndex = pageList.IndexOf(CurrentPage);
				Action moveTimeMachine = timeMachine.MoveToTheFuture;
				if(currentPageIndex == pageList.Count - 1) {
					MoveToPage(nextPage, moveTimeMachine, true);
					return;
				}
				int nextPageIndex = currentPageIndex + 1;
				if(pageList[nextPageIndex].GetType() == nextPage.GetType()) {
					MoveToPage(pageList[nextPageIndex], moveTimeMachine, false);
				} else {
					pageList.RemoveRange(nextPageIndex, pageList.Count - nextPageIndex);
					MoveToPage(nextPage, moveTimeMachine, true);
				}
			});
		}
		bool ValidatePage(IWizardPage<RemoteDocumentSourceModel> page) {
			string errorMessage;
			if(!page.Validate(out errorMessage)) {
				view.ShowError(errorMessage);
				return false;
			}
			return true;
		}
		IRemoteDocumentSourceWizardPage GetNextPage() {
			var nextPageType = CurrentPage.GetNextPageType();
			return nextPageType != null ? pageFactory.GetPage(nextPageType) : null;
		}
		protected virtual void RefreshView() {
			view.EnablePrevious(CurrentPage != null && pageList.IndexOf(CurrentPage) > 0);
			view.EnableNext(CurrentPage != null && CurrentPage.MoveNextEnabled && CurrentPage.GetNextPageType() != null);
			view.EnableFinish(CurrentPage != null && CurrentPage.FinishEnabled);  
		}
		void MoveToPage(IRemoteDocumentSourceWizardPage page, Action moveTimeMachine, bool addToList) {
			if(currentPage == page)
				return;
			if(addToList) {
				Debug.Assert(!pageList.Contains(page));
				pageList.Add(page);
			}
			if(pageList.IndexOf(page) == -1)
				throw new ArgumentException("Page does not exist in the list and addToList = false.");
			if(currentPage != null) {
				currentPage.Commit();
				currentPage.Changed -= currentPage_Changed;
				currentPage.Error -= currentPage_Error;
				currentPage.Model = null;
			}
			if(moveTimeMachine != null)
				moveTimeMachine();
			currentPage = page;
			currentPage.Changed += currentPage_Changed;
			currentPage.Error += currentPage_Error;
			currentPage.Model = timeMachine.CurrentValue;
			currentPage.Begin();
			view.SetPageContent(CurrentPage.PageContent);
			RefreshView();
		}
		public void SetStartPage(Type pageType) {
			if(pageType == null)
				throw new ArgumentNullException("pageType");
			if(pageList.Count > 0)
				throw new InvalidOperationException();
			MoveToPage(pageFactory.GetPage(pageType), null, true);
		}
	}
}
