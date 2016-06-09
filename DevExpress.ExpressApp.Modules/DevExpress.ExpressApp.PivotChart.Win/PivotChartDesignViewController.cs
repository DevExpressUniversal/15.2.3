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
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.XtraCharts;
using DevExpress.XtraCharts.Wizard;
using DevExpress.XtraCharts.Native;
using DevExpress.XtraPivotGrid;
using DevExpress.Persistent.Base;
namespace DevExpress.ExpressApp.PivotChart.Win {
	public enum ChartDesignerType { ChartWizard, ChartDesigner }
	public partial class PivotChartDesignViewController : WinAnalysisViewControllerBase {
		[System.ComponentModel.DefaultValue(ChartDesignerType.ChartDesigner)]
		public static ChartDesignerType ChartDesignerType = ChartDesignerType.ChartDesigner;
		private void chartWizardAction_Execute(object sender, SimpleActionExecuteEventArgs e) {
			ShowChartWizard();
		}
		protected override void OnAnalysisControlCreated() {
			base.OnAnalysisControlCreated();
			if(analysisEditor != null) {
				if(analysisEditor.Control != null) {
					SubscribeToEvents();
				}
				else {
					analysisEditor.ControlCreated += analysisEditor_ControlCreated;
				}
			}
		}
		private void analysisEditor_ControlCreated(object sender, EventArgs e) {
			analysisEditor.ControlCreated -= analysisEditor_ControlCreated;
			SubscribeToEvents();
		}
		private void ChartDesignViewController_ItemsChanged(object sender, ViewItemsChangedEventArgs e) {
			UpdateActionState();
		}
		protected override void UpdateActionState() {
			base.UpdateActionState();
			chartWizardAction.Active.SetItemValue("AnalysisEditor presents", analysisEditor != null);
			chartWizardAction.Enabled.SetItemValue("Data source is ready", IsDataSourceReady);
			bool hasControl = analysisEditor != null && analysisEditor.Control != null;
			chartWizardAction.Active.SetItemValue("AnalysisEditor isn't read-only", hasControl && !analysisEditor.Control.ReadOnly);
			if(hasControl) {
				chartWizardAction.Enabled.SetItemValue("Chart is visible", ((AnalysisEditorWin)analysisEditor).Control.IsChartVisible);
			}
			else {
				chartWizardAction.Enabled.SetItemValue("Chart is visible", false);
			}
		}
		protected virtual void SubscribeToEvents() {
			if(analysisEditor != null && ((AnalysisEditorWin)analysisEditor).Control != null) {
				((AnalysisEditorWin)analysisEditor).Control.ShowChartWizard += new EventHandler(Control_ShowChartWizard);
				((AnalysisEditorWin)analysisEditor).Control.ChartShowWizardMenuItem.Text = ChartWizardAction.Caption;
			}
			((DetailView)View).ItemsChanged += new EventHandler<ViewItemsChangedEventArgs>(ChartDesignViewController_ItemsChanged);
		}
		private void Control_ShowChartWizard(object sender, EventArgs e) {
			ShowChartWizard();
		}
		protected virtual void UnsubscribeFromEvents() {
			if(analysisEditor != null && ((AnalysisEditorWin)analysisEditor).Control != null) {
				((AnalysisEditorWin)analysisEditor).Control.ShowChartWizard -= new EventHandler(Control_ShowChartWizard);
			}
			((DetailView)View).ItemsChanged -= new EventHandler<ViewItemsChangedEventArgs>(ChartDesignViewController_ItemsChanged);
		}
		protected virtual void ShowChartWizard() {
			if(analysisEditor != null && analysisEditor.Control != null) {
				ChartControl chart = ((AnalysisEditorWin)analysisEditor).Control.ChartControl;
				if(ChartDesignerType == ChartDesignerType.ChartWizard) {
					ChartWizard chartWizard = new ChartWizard(chart);
					chartWizard.ConstructionGroup.UnregisterPage(chartWizard.DataPage);
					chartWizard.ShowDialog();
				}
				else {
					XtraCharts.Designer.ChartDesigner designer = new XtraCharts.Designer.ChartDesigner(chart);
					designer.ShowDialog();
				}
				ChartSettingsHelper.SaveChartSettings(analysisEditor.Control.Chart, (IAnalysisInfo)analysisEditor.CurrentObject);
			}
		}
		protected override void OnDeactivated() {
			base.OnDeactivated();
			UnsubscribeFromEvents();
			ChartWizardAction.Changed -= new EventHandler<ActionChangedEventArgs>(ChartWizardAction_Changed);
		}
		protected override void OnActivated() {
			base.OnActivated();
			ChartWizardAction.Changed += new EventHandler<ActionChangedEventArgs>(ChartWizardAction_Changed);
		}
		private void ChartWizardAction_Changed(object sender, ActionChangedEventArgs e) {
			if(analysisEditor != null && ((AnalysisEditorWin)analysisEditor).Control != null) {
				if(e.ChangedPropertyType == ActionChangedType.Active) {
					((AnalysisEditorWin)analysisEditor).Control.ChartShowWizardMenuItem.Visible = ChartWizardAction.Active;
				}
				else if(e.ChangedPropertyType == ActionChangedType.Enabled) {
					((AnalysisEditorWin)analysisEditor).Control.ChartShowWizardMenuItem.Enabled = ChartWizardAction.Enabled;
				}
			}
		}
		public PivotChartDesignViewController() {
			InitializeComponent();
			RegisterActions(components);
		}
		public SimpleAction ChartWizardAction {
			get { return chartWizardAction; }
		}
	}
	public class WinAnalysisViewControllerBase : AnalysisViewControllerBase {
		private void Control_ChartVisibilityChanged(object sender, EventArgs e) {
			UpdateActionState();
		}
		protected override void OnAnalysisControlCreated() {
			base.OnAnalysisControlCreated();
			if(analysisEditor != null && analysisEditor.Control != null) {
				((AnalysisEditorWin)analysisEditor).Control.ChartVisibilityChanged += new EventHandler<EventArgs>(Control_ChartVisibilityChanged);
			}
		}
		protected override void OnDeactivated() {
			if(analysisEditor != null && analysisEditor.Control != null) {
				((AnalysisEditorWin)analysisEditor).Control.ChartVisibilityChanged -= new EventHandler<EventArgs>(Control_ChartVisibilityChanged);
			}
			base.OnDeactivated();
		}
	}
}
