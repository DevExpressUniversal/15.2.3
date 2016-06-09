#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       eXpressApp Framework                                        }
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

using DevExpress.Utils;
using DevExpress.XtraBars.Docking2010;
using DevExpress.XtraBars.Docking2010.Views;
namespace DevExpress.ExpressApp.Win.Templates.Utils {
	public sealed class DocumentManagerViewPopupMenuHelper {
		private void documentManager_ViewChanging(object sender, ViewEventArgs args) {
			args.View.PopupMenuShowing -= View_PopupMenuShowing;
		}
		private void documentManager_ViewChanged(object sender, ViewEventArgs args) {
			args.View.PopupMenuShowing += View_PopupMenuShowing;
		}
		private void View_PopupMenuShowing(object sender, PopupMenuShowingEventArgs e) {
			e.Menu.Remove(BaseViewControllerCommand.FloatAll);
		}
		public void Attach(DocumentManager documentManager) {
			Guard.ArgumentNotNull(documentManager, "documentManager");
			documentManager.ViewChanging += documentManager_ViewChanging;
			documentManager.ViewChanged += documentManager_ViewChanged;
			if(documentManager.View != null) {
				documentManager.View.PopupMenuShowing += View_PopupMenuShowing;
			}
		}
	}
}
