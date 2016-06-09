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
using System.Text;
using DevExpress.XtraCharts;
using DevExpress.XtraCharts.Wizard;
using DevExpress.XtraCharts.Native;
using DevExpress.LookAndFeel;
using System.Windows.Forms;
using DevExpress.ExpressApp.Utils;
namespace DevExpress.ExpressApp.Chart.Win {
	public enum ChartDesignerType { ChartWizard, ChartDesigner }
	public class WinChartControlContainer : ChartControlContainer {
		[System.ComponentModel.DefaultValue(ChartDesignerType.ChartDesigner)]
		public static ChartDesignerType ChartDesignerType = ChartDesignerType.ChartDesigner;
		private void ShowWizard() {
			if(ChartContainer != null) {
				ResetAutoRange();
				if(ChartDesignerType == ChartDesignerType.ChartWizard) {
					ChartWizard wizard = new ChartWizard(ChartContainer);
					wizard.ShowDialog();
				}
				else {
					XtraCharts.Designer.ChartDesigner designer = new XtraCharts.Designer.ChartDesigner(ChartContainer);
					designer.ShowDialog();
				}
				SaveChartSettings();
			}
		}
		private void ChartClearSettings() {
			if(ChartContainer != null) {
				ClearSettings();
			}
		}
		protected override void DefaultInitialization(IChartDataSourceProvider dataSourceProvider) {
			base.DefaultInitialization(dataSourceProvider);
			ICustomizationEnabledProvider customizationEnabledProvider = dataSourceProvider as ICustomizationEnabledProvider;
			ChartControl chartContainer = ChartContainer as ChartControl;
			if(chartContainer != null) {
				if(chartContainer.ContextMenuStrip != null) {
					chartContainer.ContextMenuStrip.Dispose();
					chartContainer.ContextMenuStrip = null;
				}
				if(customizationEnabledProvider == null || customizationEnabledProvider.CustomizationEnabled) {
					chartContainer.ContextMenuStrip = new ContextMenuStrip();
					chartContainer.ContextMenuStrip.Text = "Tasks";
					ToolStripMenuItem miWizard = new ToolStripMenuItem(CaptionHelper.GetLocalizedText(ChartModule.LocalizationGroup, "InvokeWizard"), ImageLoader.Instance.GetImageInfo("Action_Chart_ShowDesigner").Image);
					miWizard.ShortcutKeys = Keys.Control | Keys.W;
					miWizard.Click += delegate(object s, EventArgs args) {
						ShowWizard();
					};
					ToolStripMenuItem miClear = new ToolStripMenuItem(CaptionHelper.GetLocalizedText(ChartModule.LocalizationGroup, "ClearSettings"), ImageLoader.Instance.GetImageInfo("Action_Clear_Settings").Image);
					miClear.ShortcutKeys = Keys.Control | Keys.C;
					miClear.Click += delegate(object s, EventArgs args) {
						ChartClearSettings();
					};
					chartContainer.ContextMenuStrip.Items.AddRange(new ToolStripMenuItem[] { miWizard, miClear });
				}
			}
		}
		public WinChartControlContainer(ChartControl chartContainer)
			: base(chartContainer) {
			IChartContainer container = (IChartContainer)chartContainer;
			UserLookAndFeel lookAndFeel = (UserLookAndFeel)container.RenderProvider.LookAndFeel;
			lookAndFeel.Assign(UserLookAndFeel.Default);
		}
	}
}
