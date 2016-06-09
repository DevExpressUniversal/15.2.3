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
using DevExpress.ExpressApp.SystemModule;
namespace DevExpress.ExpressApp.ReportsV2.Win {
	public class ReportDataSelectionDialogController : DialogController {
		private const string IsSingleReportSelected = "IsSingleReportSelected";
		private void ListViewProcessCurrentObjectController_CustomProcessSelectedItem(object sender, CustomProcessListViewSelectedItemEventArgs e) {
			e.Handled = true;
		}
		private void View_SelectionChanged(object sender, EventArgs e) {
			UpdateactionState((View)sender);
		}
		private void Frame_ViewChanging(object sender, ViewChangingEventArgs e) {
			if(Frame.View != null) {
				Frame.View.SelectionChanged -= new EventHandler(View_SelectionChanged);
			}
			if(e.View != null) {
				e.View.SelectionChanged += new EventHandler(View_SelectionChanged);
			}
			UpdateactionState(e.View);
		}
		private void UpdateactionState(View view) {
			if(view != null) {
				AcceptAction.Enabled[IsSingleReportSelected] = view.SelectedObjects.Count == 1;
			}
		}
		protected override void OnActivated() {
			base.OnActivated();
			ListViewProcessCurrentObjectController processCurrentObjectController = Frame.GetController<ListViewProcessCurrentObjectController>();
			if(processCurrentObjectController != null) {
				processCurrentObjectController.CustomProcessSelectedItem += new EventHandler<CustomProcessListViewSelectedItemEventArgs>(ListViewProcessCurrentObjectController_CustomProcessSelectedItem);
			}
			Frame.ViewChanging += new EventHandler<ViewChangingEventArgs>(Frame_ViewChanging);
		}
		protected override void OnDeactivated() {
			Frame.ViewChanging -= new EventHandler<ViewChangingEventArgs>(Frame_ViewChanging);
			ListViewProcessCurrentObjectController processCurrentObjectController = Frame.GetController<ListViewProcessCurrentObjectController>();
			if(processCurrentObjectController != null) {
				processCurrentObjectController.CustomProcessSelectedItem -= new EventHandler<CustomProcessListViewSelectedItemEventArgs>(ListViewProcessCurrentObjectController_CustomProcessSelectedItem);
			}
			base.OnDeactivated();
		}
	}
}
