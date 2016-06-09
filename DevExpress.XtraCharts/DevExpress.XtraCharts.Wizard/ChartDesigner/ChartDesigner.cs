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
using System.ComponentModel.Design;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
using DevExpress.Data.ChartDataSources;
using DevExpress.LookAndFeel;
using DevExpress.Skins;
using DevExpress.Utils;
using DevExpress.XtraCharts.Designer.Native;
using DevExpress.XtraCharts.Localization;
using DevExpress.XtraCharts.Native;
namespace DevExpress.XtraCharts.Designer {
	public class ChartDesigner {
		internal const string XtraChartsRegistryPath = "Software\\Developer Express\\XtraCharts\\";
		internal const string XtraChartsShowDesignerRegistryEntry = "ShowDesigner";
		IDesignerHost designerHost;
		Icon icon;
		string caption;
		IChartContainer chartContainer;
		bool hideStartupCheckBox;
		internal IChartContainer ChartContainer { get { return chartContainer; } }
		internal IDesignerHost DesignerHost { get { return designerHost; } }
		public ChartDesigner(object chart)
			: this(chart, null) {
		}
		public ChartDesigner(object chart, IDesignerHost designerHost) {
			this.designerHost = designerHost;
			this.hideStartupCheckBox = designerHost == null;
			Assembly assembly = Assembly.GetAssembly(typeof(Chart));
			icon = ResourceImageHelper.CreateIconFromResources("DevExpress.XtraCharts.Design.Wizard.Images.chart.ico", assembly);
			caption = ChartLocalizer.GetString(ChartStringId.WizFormTitle);
			chartContainer = (IChartContainer)chart;
			if (chartContainer.ShouldEnableFormsSkins)
				SkinManager.EnableFormSkins();
		}
		public DialogResult ShowDialog() {
			return ShowDialog(false);
		}
		public DialogResult ShowDialog(bool topMost) {
			DialogResult result = ShowDialog((UserLookAndFeel)chartContainer.RenderProvider.LookAndFeel, topMost);
			if (chartContainer.Chart.Is3DDiagram)
				chartContainer.Chart.ClearSelection();
			return result;
		}
		public DialogResult ShowDialog(UserLookAndFeel lookAndFeel) {
			return ShowDialog(lookAndFeel, false);
		}
		public DialogResult ShowDialog(UserLookAndFeel lookAndFeel, bool topMost) {
			using (DesignerFormBase form = new DesignerFormBase(this, lookAndFeel)) {
				if (hideStartupCheckBox)
					form.HideStartupCheckBox();
				form.TopMost = topMost;
				DialogResult result;
				PivotGridDataSourceOptionsSnapshot pivotGridSnapshot = PivotGridDataSourceOptionsSnapshot.Create(chartContainer.Chart.DataContainer.DataSource as IPivotGrid);
				chartContainer.Chart.LockBinding();
				try {
					IServiceProvider provider = designerHost != null ? (IServiceProvider)designerHost :
						chartContainer is IComponent ? ((IComponent)chartContainer).Site :
						null;
					result = provider != null ? DevExpress.XtraPrinting.Native.DialogRunner.ShowDialog(form, provider) :
						DevExpress.XtraPrinting.Native.DialogRunner.ShowDialog(form, (IWin32Window)null);
				}
				finally {
					chartContainer.Chart.UnlockBinding();
				}
				if (result == DialogResult.OK) {
					DesignerTransaction transaction = null;
					try {
						if (designerHost != null) {
							transaction = designerHost.CreateTransaction(ChartLocalizer.GetString(ChartStringId.TrnChartWizard));
							if (transaction != null)
								chartContainer.Changing();
						}
						chartContainer.Chart.Assign(form.Chart);
						chartContainer.Changed();
						if (transaction != null)
							transaction.Commit();
					}
					catch (Exception e) {
						if (transaction != null)
							transaction.Cancel();
						chartContainer.ShowErrorMessage(e.Message, String.Empty);
						result = DialogResult.Cancel;
					}
				}
				else if (pivotGridSnapshot != null)
					pivotGridSnapshot.RestoreOptions(chartContainer.Chart.DataContainer.DataSource as IPivotGrid);
				form.SaveLayoutToRegistry();
				return result;
			}
		}
	}
}
