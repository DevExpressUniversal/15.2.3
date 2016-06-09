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
using DevExpress.XtraPrinting.Design.RemoteDocumentSourceDesign.Views;
namespace DevExpress.XtraPrinting.Design.RemoteDocumentSourceDesign.Presenters {
	class ChooseDocumentSourceTypePresenter<TView> : RemoteDocumentSourcePagePresenterBase<IPageView>
		where TView : IChooseDocumentSourceTypeView {
		public override bool FinishEnabled {
			get { return false; }
		}
		public override bool MoveNextEnabled {
			get { return true; }
		}
		protected new IChooseDocumentSourceTypeView View { get { return (IChooseDocumentSourceTypeView)base.View; } }
		public ChooseDocumentSourceTypePresenter(IChooseDocumentSourceTypeView view)
			: base(view) {
			view.DocumentSourceTypeChanged += view_DocumentSourceTypeChanged;
		}
		public override void Begin() {
			View.DocumentSourceType = Model.DocumentSourceType;
		}
		public override void Commit() {
			if(Model.DocumentSourceType != View.DocumentSourceType)
				Model.Clear();
			Model.DocumentSourceType = View.DocumentSourceType;
		}
		public override Type GetNextPageType() {
			return View.DocumentSourceType == RemoteDocumentSourceType.ReportService
				? typeof(ISetReportServiceReportNamePresenter)
				: typeof(SetReportServerCredentialsPresenter<ISetReportServerCredentialsView>);
		}
		void view_DocumentSourceTypeChanged(object sender, EventArgs e) {
			RaiseChanged();
		}
	}
}
