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
using System.Text;
using DevExpress.XtraEditors;
using DevExpress.XtraCharts.Native;
using DevExpress.XtraCharts.Design;
using System.ComponentModel.Design;
using System.ComponentModel;
using System.Windows.Forms;
using DevExpress.XtraCharts.Localization;
namespace DevExpress.XtraCharts.Wizard {
	[DXToolboxItem(false)]
	public class WizardControlBase : XtraUserControl {
		IChartContainer userChart;
		Chart chart;
		WizardPage wizardPage;
		[
#if !SL
	DevExpressXtraChartsWizardLocalizedDescription("WizardControlBaseCurrentChartControl"),
#endif
DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public object CurrentChartControl {
			get {
				if (userChart == null)
					userChart = FindChart(this);
				return userChart;
			}
			set {
				if (!(value is IChartContainer))
					throw new ArgumentException(ChartLocalizer.GetString(ChartStringId.MsgNotChartControl));
				if (!TestChartBelong((IChartContainer)value))
					throw new ArgumentException(ChartLocalizer.GetString(ChartStringId.MsgNotBelongingChart));
				userChart = (IChartContainer)value;
			}
		}
		internal IChartContainer UserChart { get { return (IChartContainer)CurrentChartControl; } }
		public WizardControlBase() {
		}
		internal bool ValidateContent(bool callOnValidateHandler = true) {
			ApplyChangesEventArgs args = new ApplyChangesEventArgs(this);
			if (callOnValidateHandler) 
				this.wizardPage.OnValidateHandler(args);
			return !args.Cancel;
		}
		internal void CompleteChanges() {
			this.chart.Assign(UserChart.Chart);
		}
		internal void Initialize(Chart chart, WizardPage page) {
			this.chart = chart;
			this.wizardPage = page;
			this.wizardPage.OnInitializePage(new InitializePageEventArgs(this));
			InitializeChart();
		}
		void InitializeChart() {
			if (UserChart == null)
				throw new ArgumentException(ChartLocalizer.GetString(ChartStringId.MsgInitializeChartNotFound));
			UserChart.Site = new CustomWizardSite(UserChart);
			UserChart.Assign(this.chart);
		}
		bool TestChartBelong(IChartContainer testChart) {
			IList<IChartContainer> list = ScanControlHierarchyForChart(this);
			return list.IndexOf(testChart) > -1;
		}
		IChartContainer FindChart(Control control) {
			IList<IChartContainer> charts = ScanControlHierarchyForChart(control);
			return charts.Count != 1 ? null : charts[0];
		}
		IList<IChartContainer> ScanControlHierarchyForChart(Control parentControl) {
			List<IChartContainer> charts = new List<IChartContainer>();
			foreach (Control control in parentControl.Controls) {
				if (control is IChartContainer)
					charts.Add((IChartContainer)control);
				else
					charts.AddRange(ScanControlHierarchyForChart(control));
			}
			return charts;
		}
		void InitializeComponent() {
			this.SuspendLayout();
			this.Name = "WizardControlBase";
			this.Size = new System.Drawing.Size(658, 440);
			this.ResumeLayout(false);
		}
	}
	public delegate void InitializePageEventHandler(object sender, InitializePageEventArgs e);
	public delegate void ApplyChangesEventHandler(object sender, ApplyChangesEventArgs e);
	public class InitializePageEventArgs : EventArgs {
		WizardControlBase control;
		public WizardControlBase Control { get { return control; } }
		public object Chart { get { return control.CurrentChartControl; } set { control.CurrentChartControl = value; } }
		internal InitializePageEventArgs(WizardControlBase control) {
			this.control = control;
		}
	}
	public class ApplyChangesEventArgs : CancelEventArgs {
		WizardControlBase control;
		public WizardControlBase Control { get { return control; } }
		internal ApplyChangesEventArgs(WizardControlBase control) {
			this.control = control;
		}
	}
	class CustomWizardSite : ISite {
		IChartContainer container;
		string name;
		public CustomWizardSite(IChartContainer container) {
			this.container = container;
		}
		#region ISite Members
		public IComponent Component {
			get { return null; }
		}
		public IContainer Container {
			get { return null; }
		}
		public bool DesignMode {
			get { return true; }
		}
		public string Name {
			get {
				return name;
			}
			set {
				name = value;
			}
		}
		#endregion
		#region IServiceProvider Members
		public object GetService(Type serviceType) {
			return null;
		}
		#endregion
	}
}
