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
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.Charts.Native;
using DevExpress.Data.Browsing;
using DevExpress.XtraCharts.Localization;
using DevExpress.XtraCharts.Native;
using DevExpress.XtraCharts.Wizard;
using DevExpress.XtraTab;
namespace DevExpress.XtraCharts.Wizard {
	internal partial class NewChartDataControl : InternalWizardControlBase {
		readonly string wizPivotGridOptionsPageName;
		WizardStateController controller;
		WizardDataPage Page { get { return (WizardDataPage)WizardPage; } }
		public NewChartDataControl() {
			InitializeComponent();
			wizPivotGridOptionsPageName = ChartLocalizer.GetString(ChartStringId.WizPivotGridDataSourcePageName);
		}
		void tbcDataPages_SelectedPageChanged(object sender, TabPageChangedEventArgs e) {
			if (controller != null && controller.CanChangeState())
				controller.CustomState(tbcDataPages.TabPages.IndexOf(e.Page));
		}
		public override void InitializeChart(WizardFormBase form) {
			base.InitializeChart(form);
			List<WizardPage> wizardPages = new List<WizardPage>();
			if (!Page.HiddenPageTabs.Contains(DataPageTab.Points))
				wizardPages.Add(WizardPage.CreateWizardPage(null, WizardPageType.UserDefined, typeof(SeriesPointEditControl),
					ChartLocalizer.GetString(ChartStringId.WizSeriesPointPageName), "", "", null));
			if (!Page.HiddenPageTabs.Contains(DataPageTab.SeriesBinding))
				wizardPages.Add(WizardPage.CreateWizardPage(null, WizardPageType.UserDefined, typeof(SeriesDataBindingControl),
					ChartLocalizer.GetString(ChartStringId.WizSeriesDataBindingPageName), "", "", null));
			if (!Page.HiddenPageTabs.Contains(DataPageTab.AutoCreatedSeries))
				wizardPages.Add(WizardPage.CreateWizardPage(null, WizardPageType.UserDefined, typeof(SeriesTemplateDataBindingControl),
					ChartLocalizer.GetString(ChartStringId.AutocreatedSeriesName), "", "", null));
			if (!Page.HiddenPageTabs.Contains(DataPageTab.PivotGridOptions))
				wizardPages.Add(WizardPage.CreateWizardPage(null, WizardPageType.UserDefined, typeof(PivotGridDataSourceControl),
					wizPivotGridOptionsPageName, "", "", null));
			foreach (WizardPage wizardPage in wizardPages) {
				XtraTabPage tabPage = tbcDataPages.TabPages.Add(wizardPage.Label);
				TabControlContainer container = new TabControlContainer(tabPage);
				wizardPage.LabelControl = container;
				wizardPage.ParentControl = container;
			}
			controller = new WizardTabStateController(form, wizardPages);
			UpdatePages();
		}
		public void UpdatePages() {
			XtraTabPage pivotGridOptionsTabPage = null;
			foreach (XtraTabPage tabPage in tbcDataPages.TabPages)
				if (tabPage.Text == wizPivotGridOptionsPageName)
					pivotGridOptionsTabPage = tabPage;
			if (pivotGridOptionsTabPage != null) {
				pivotGridOptionsTabPage.PageVisible = PivotGridDataSourceUtils.HasPivotGrid(WizardForm.Chart.DataContainer.PivotGridDataSourceOptions);
				if (pivotGridOptionsTabPage.PageVisible) {
					PivotGridDataSourceControl pivotGridOptionsControl = null;
					foreach (Control control in pivotGridOptionsTabPage.Controls)
						if (control is PivotGridDataSourceControl)
							pivotGridOptionsControl = (PivotGridDataSourceControl)control;
					if (pivotGridOptionsControl != null)
						pivotGridOptionsControl.UpdateControls();
				}
			}
		}
		public override bool ValidateContent() {
			return controller.State.ValidateContent();
		}
		public override void CompleteChanges() {
			controller.State.PrepareChangeState();
		}
	}
	class TabControlContainer : ILabelControl, IParentControl {
		XtraTabPage page;
		public Control Control { get { return this.page; } }
		public string Text { get { return page.Text; } }
		public Image Image { get { return null; } }
		public TabControlContainer(XtraTabPage page) {
			this.page = page;
		}
		public void Highlight() {
		}
		public void SetDescription(string description)	{
		}
		public void SetHeader(string header) {
		}
	}
}
namespace DevExpress.XtraCharts.Native {
	internal class WizardTabStateController : WizardStateController {
		public WizardTabStateController(WizardFormBase form, IList<WizardPage> pageData) : base(form, pageData) {
		}
		public override void UpdateWizardForm() {
		}
	}
	public static class WizardScaleTypeHelper {
		public static int GetArgumentScaleTypeIndex(ScaleType scaleType) {
			switch (scaleType) {
				case ScaleType.Qualitative:
					return 0;
				case ScaleType.Numerical:
					return 1;
				case ScaleType.DateTime:
					return 2;
				default:
					return 3;
			}
		}
		public static int GetValueScaleTypeIndex(ScaleType scaleType) {
			return scaleType == ScaleType.Numerical ? 0 : 1;
		}
		public static ScaleType GetArgumentScaleType(int index) {
			switch (index) {
				case 0:
					return ScaleType.Qualitative;
				case 1:
					return ScaleType.Numerical;
				case 2:
					return ScaleType.DateTime;
				default:
					return ScaleType.Auto;
			}
		}
		public static ScaleType GetValueScaleType(int index) {
			return index == 0 ? ScaleType.Numerical : ScaleType.DateTime;
		}
		public static ScaleType GetScaleType(DataContext dataContext, object dataSource, string dataMember) {
			DataContext actualDataContext = dataContext;
			bool shouldDisposeDataContext = false;
			if (actualDataContext == null) {
				actualDataContext = new DataContext();
				shouldDisposeDataContext = true;
			}
			try {
				DataBrowser browser = actualDataContext[dataSource];
				if (browser == null)
					return ScaleType.Qualitative;
				string browserDataMember = String.Empty;
				PropertyDescriptor desc;
				for (; ; ) {
					desc = browser.FindItemProperty(dataMember, false);
					if (desc != null)
						break;
					int dotIndex = dataMember.IndexOf('.');
					if (dotIndex == -1)
						break;
					if (browserDataMember.Length > 0)
						browserDataMember += ".";
					browserDataMember += dataMember.Substring(0, dotIndex);
					dataMember = dataMember.Substring(dotIndex + 1);
					browser = actualDataContext[dataSource, browserDataMember];
				}
				if (desc == null)
					return ScaleType.Qualitative;
				if (BindingHelperCore.CheckNumericDataType(desc.PropertyType))
					return ScaleType.Numerical;
				if (typeof(DateTime).IsAssignableFrom(desc.PropertyType) || typeof(DateTime?).IsAssignableFrom(desc.PropertyType))
					return ScaleType.DateTime;
				return ScaleType.Qualitative;
			}
			catch {
				return ScaleType.Qualitative;
			}
			finally {
				if (shouldDisposeDataContext)
					actualDataContext.Dispose();
			}
		}
	}
}
