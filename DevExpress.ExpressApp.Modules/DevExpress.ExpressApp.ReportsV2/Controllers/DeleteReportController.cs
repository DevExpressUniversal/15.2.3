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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevExpress.ExpressApp.SystemModule;
namespace DevExpress.ExpressApp.ReportsV2 {
	public class DeleteReportController : ReportDataViewController {
		private DeleteObjectsViewController deleteObjectsViewController;
		protected override void OnActivated() {
			base.OnActivated();
			if(ReportsModuleV2.ActivateReportController(this)) {
				deleteObjectsViewController = Frame.GetController<DeleteObjectsViewController>();
				View.SelectionChanged += new EventHandler(View_SelectionChanged);
				View.CurrentObjectChanged += new EventHandler(View_CurrentObjectChanged);
				UpdateActionState();
			}
		}
		protected override void OnDeactivated() {
			base.OnDeactivated();
			View.SelectionChanged -= new EventHandler(View_SelectionChanged);
			View.CurrentObjectChanged -= new EventHandler(View_CurrentObjectChanged);
			if(deleteObjectsViewController != null) {
				deleteObjectsViewController.DeleteAction.Enabled.RemoveItem("PredefinedReportData");
			}
		}
		protected virtual void UpdateActionState() {
			if(deleteObjectsViewController != null) {
				if(View.SelectedObjects.Count > 0) {
					bool predefinedReportData = true;
					foreach(object obj in View.SelectedObjects) {
						if(GetReportData(obj).IsPredefined) {
							predefinedReportData = false;
							break;
						}
					}
					deleteObjectsViewController.DeleteAction.Enabled.SetItemValue("PredefinedReportData", predefinedReportData);
				}
			}
		}
		private void View_SelectionChanged(object sender, EventArgs e) {
			UpdateActionState();
		}
		private void View_CurrentObjectChanged(object sender, EventArgs e) {
			UpdateActionState();
		}
	}
}
