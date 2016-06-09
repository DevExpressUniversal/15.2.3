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
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Web.Templates;
using DevExpress.Web.ASPxPivotGrid;
namespace DevExpress.ExpressApp.PivotGrid.Web {
	public class PivotGridPopupMenuManager : IXafCallbackHandler {
		private DevExpress.Web.MenuItem miChartOptions;
		private DevExpress.Web.MenuItem miChartDataVertical;
		private DevExpress.Web.MenuItem miShowRowTotals;
		private DevExpress.Web.MenuItem miShowRowGrandTotals;
		private DevExpress.Web.MenuItem miShowColumnTotals;
		private DevExpress.Web.MenuItem miShowColumnGrandTotals;
		bool registerScriptOnLoad = false;
		private ASPxPivotGrid control;
		private const string ChartOptionsMenuItemName = "ChartOptions";
		private void pivotGridControl_PopupMenuCreated(object sender, PivotPopupMenuCreatedEventArgs e) {
			if(e.MenuType == PivotGridPopupMenuType.HeaderMenu) {
				ProcessMenu(e.Menu);
				RegisterItemClickScript();
			}
		}
		protected void ProcessMenu(ASPxPivotGridPopupMenu menu) {
			miChartOptions = new DevExpress.Web.MenuItem(CaptionHelper.GetLocalizedText(PivotGridModule.LocalizationGroup, "ChartOptions"), ChartOptionsMenuItemName, ImageLoader.Instance.GetImageInfo("Action_Chart_Options").ImageUrl);
			miChartOptions.Visible = pivotSettings.ShowChart;
			miChartDataVertical = new DevExpress.Web.MenuItem(CaptionHelper.GetLocalizedText(PivotGridModule.LocalizationGroup, "ChartDataVertical"), "ChartDataVertical", ImageLoader.Instance.GetImageInfo("Action_ChartDataVertical").ImageUrl);
			miChartDataVertical.Checked = pivotSettings.ChartDataVertical;
			miChartOptions.Items.Add(miChartDataVertical);
			miShowRowTotals = new DevExpress.Web.MenuItem(CaptionHelper.GetLocalizedText(PivotGridModule.LocalizationGroup, "ShowRowTotals"), "ShowRowTotals", ImageLoader.Instance.GetImageInfo("Action_Totals_Row").ImageUrl);
			miShowRowTotals.Checked = pivotSettings.ShowRowTotals;
			miChartOptions.Items.Add(miShowRowTotals);
			miShowRowGrandTotals = new DevExpress.Web.MenuItem(CaptionHelper.GetLocalizedText(PivotGridModule.LocalizationGroup, "ShowRowGrandTotals"), "ShowRowGrandTotals", ImageLoader.Instance.GetImageInfo("Action_Grand_Totals_Row").ImageUrl);
			miShowRowGrandTotals.Checked = pivotSettings.ShowRowGrandTotals;
			miChartOptions.Items.Add(miShowRowGrandTotals);
			miShowColumnTotals = new DevExpress.Web.MenuItem(CaptionHelper.GetLocalizedText(PivotGridModule.LocalizationGroup, "ShowColumnTotals"), "ShowColumnTotals", ImageLoader.Instance.GetImageInfo("Action_Totals_Column").ImageUrl);
			miShowColumnTotals.Checked = pivotSettings.ShowColumnTotals;
			miChartOptions.Items.Add(miShowColumnTotals);
			miShowColumnGrandTotals = new DevExpress.Web.MenuItem(CaptionHelper.GetLocalizedText(PivotGridModule.LocalizationGroup, "ShowColumnGrandTotals"), "ShowColumnGrandTotals", ImageLoader.Instance.GetImageInfo("Action_Grand_Totals_Column").ImageUrl);
			miShowColumnGrandTotals.Checked = pivotSettings.ShowColumnGrandTotals;
			miChartOptions.Items.Add(miShowColumnGrandTotals);
			menu.Items.Add(miChartOptions);
			menu.AutoPostBack = false;
		}
		public string GetChartOptionsMenuItemClickScript(ICallbackManagerHolder callbackManagerHolder, string handlerID, string parameter) {
			string result = string.Empty;
			if(callbackManagerHolder != null && callbackManagerHolder.CallbackManager != null) {
				result = string.Format(
@"function (s, e) {{
    if(e.MenuType === '{0}') {{
        {1}
    }}
}}", PivotGridPopupMenuType.HeaderMenu.ToString(), callbackManagerHolder.CallbackManager.GetScript(handlerID, parameter));
			}
			return result;		  
		}
		IPivotSettings pivotSettings;
		public void RegisterItemClickScript() {
			if(this.control != null) {
				if(this.control.Page != null) {
					this.control.ClientSideEvents.PopupMenuItemClick = GetChartOptionsMenuItemClickScript(this.control.Page as ICallbackManagerHolder, this.control.UniqueID, "e.MenuItemName");
				}
				else {
					registerScriptOnLoad = true;
				}
			}
		}
		private void control_Load(object sender, EventArgs e) {
			ASPxPivotGrid pivotGrid = ((ASPxPivotGrid)sender);
			Guard.TypeArgumentIs(typeof(ICallbackManagerHolder), pivotGrid.Page.GetType(), "Page");
			XafCallbackManager callbackManager = ((ICallbackManagerHolder)pivotGrid.Page).CallbackManager;
			callbackManager.RegisterHandler(pivotGrid.UniqueID, this);
			if(registerScriptOnLoad) {
				RegisterItemClickScript();
				registerScriptOnLoad = false;
			}
		}
		public void Attach(ASPxPivotGrid control, IPivotSettings pivotSettings) {
			Guard.ArgumentNotNull(control, "control");
			Guard.ArgumentNotNull(pivotSettings, "pivotSettings");
			Detach();
			this.control = control;
			this.pivotSettings = pivotSettings;
			this.control.Load += new EventHandler(control_Load);
			this.control.PopupMenuCreated += new PivotPopupMenuCreatedEventHandler(pivotGridControl_PopupMenuCreated);
#if DebugTest
			DebugTest_IsAttached = true;
#endif
		}
		public void Detach() {
			if(control != null) {
				control.Load -= new EventHandler(control_Load);
				control.PopupMenuCreated -= new PivotPopupMenuCreatedEventHandler(pivotGridControl_PopupMenuCreated);
				control = null;
			}
			pivotSettings = null;
			miChartDataVertical = null;
			miShowRowTotals = null;
			miShowRowGrandTotals = null;
			miShowColumnTotals = null;
			miShowColumnGrandTotals = null;
#if DebugTest
			DebugTest_IsAttached = false;
#endif
		}
		#region IXafCallbackHandler Members
		public void ProcessAction(string parameter) {
			DevExpress.Web.MenuItem menuItem = miChartOptions != null ? miChartOptions.Items.FindByName(parameter) : null;
			if(menuItem != null) {
				menuItem.Checked = !menuItem.Checked;
				if(menuItem.Text == miChartDataVertical.Text) {
					pivotSettings.ChartDataVertical = !pivotSettings.ChartDataVertical;
				}
				if(menuItem.Text == miShowRowTotals.Text) {
					pivotSettings.ShowRowTotals = !pivotSettings.ShowRowTotals;
				}
				if(menuItem.Text == miShowRowGrandTotals.Text) {
					pivotSettings.ShowRowGrandTotals = !pivotSettings.ShowRowGrandTotals;
				}
				if(menuItem.Text == miShowColumnTotals.Text) {
					pivotSettings.ShowColumnTotals = !pivotSettings.ShowColumnTotals;
				}
				if(menuItem.Text == miShowColumnGrandTotals.Text) {
					pivotSettings.ShowColumnGrandTotals = !pivotSettings.ShowColumnGrandTotals;
				}
			}
		}
		#endregion
		public void UpdateMenuItemVisibility(bool isVisible) {
			if(miChartOptions != null) {
				miChartOptions.Visible = isVisible;
			}
		}
#if DebugTest
		public bool DebugTest_IsAttached { get; private set; }
#endif
	}
}
