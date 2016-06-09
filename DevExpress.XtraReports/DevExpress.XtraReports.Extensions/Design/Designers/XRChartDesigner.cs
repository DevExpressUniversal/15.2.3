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
using System.Collections;
using System.Diagnostics;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Drawing.Design;
using System.Windows.Forms;
using DevExpress.Data.Design;
using DevExpress.XtraPrinting;
using DevExpress.XtraCharts.Native;
using DevExpress.XtraCharts.Design;
using DevExpress.XtraCharts.Wizard;
using DevExpress.XtraReports.UI;
using DevExpress.XtraReports.Design.Adapters;
using DevExpress.XtraReports.Design.MouseTargets;
using DevExpress.XtraReports.Design.Behaviours;
using DevExpress.XtraReports.Configuration;
using DevExpress.DataAccess.Wizard.Services;
namespace DevExpress.XtraReports.Configuration {
	public class DesignSettings {
		static DesignSettings defaultInstance;
		public static DesignSettings Default {
			get {
				if(defaultInstance == null)
					defaultInstance = new DesignSettings();
				return defaultInstance;
			}
			set {
				defaultInstance = value;
			}
		}
		public ChartDesignOptions ChartOptions { get; private set; }
		public DesignSettings() {
			ChartOptions = new ChartDesignOptions();
		}
	}
	public class ChartDesignOptions {
		bool showWizard = false;
		bool showDesigner = true;
		public bool ShowAnyEditor {
			get {
				return ShowWizard || ShowDesigner;
			}
		}
		public bool ShowWizard {
			get {
				return showWizard;
			}
			set {
				showWizard = value;
				if(showWizard) showDesigner = false;
			}
		}
		public bool ShowDesigner {
			get {
				return showDesigner;
			}
			set {
				showDesigner = value;
				if(showDesigner) showWizard = false;
			}
		}
	} 
}
namespace DevExpress.XtraReports.Design {
	[
	MouseTarget(typeof(ChartMouseTarget)),
	DesignerBehaviour(typeof(ChartDesignerBehaviour)),
	]
	public class XRChartDesigner : XRControlDesigner {
		[Obsolete("Use the DevExpress.XtraReports.Configuration.DesignSettings.ChartOptions property instead."),
		EditorBrowsable(EditorBrowsableState.Never)]
		public static bool ShouldShowWizard = true;
		ChartDesigner designer;
		ChartNavigationController navigationController;
		XRChartDesignerActionList chartDesignerActionList;
		internal XRChart XRChart { get { return Component as XRChart; } }
		internal ChartDesigner ChartDesigner { get { return designer; } }
		internal ChartNavigationController NavigationController { get { return navigationController; } }
		protected internal override bool IsRotatable { get { return true; } }
		public override bool CanDrag {
			get { return NavigationController.NavigationState != NavigationState.Scrolling; }
		}
		public new Color BackColor_ {
			get { return XRChart.BackColor; }
			set { XRChart.BackColor = value; }
		}
		public new PaddingInfo Padding_ {
			get { return XRChart.Padding; }
			set { XRChart.Padding = value; }
		}
		public XRChartDesigner() {
		}
		protected override void Dispose(bool disposing) {
			if (disposing && designer != null)
				designer.Dispose();
			base.Dispose(disposing);
		}
		protected override string[] GetFilteredProperties() {
			return new string[] { XRComponentPropertyNames.Text, XRComponentPropertyNames.ForeColor };
		}
		protected override void RegisterActionLists(DesignerActionListCollection list) {
			if (Component != null) {
				list.Add(new DataSourceItemDesignerList(this, XRChart));
				list.Add(new XRFormattingControlDesignerActionList(this));
				list.Add(new CommonXRChartItemsDesignerList(this, chartDesignerActionList));
				list.Add(new SerializingXRChartsItemsDesignerList(this, chartDesignerActionList));
				Verbs.Clear();
				foreach (DesignerActionList actionItems in list)
					foreach (DesignerActionItem actionItem in actionItems.GetSortedActionItems()) {
						DesignerActionMethodItem methodItem = actionItem as DesignerActionMethodItem;
						if (methodItem != null)
							Verbs.Add(new XRDesignerMethodWrapper(methodItem));
					}
			}
		}
		protected virtual void ValidateDataAdapter() {
			XRChart.DataAdapter = DataAdapterHelper.ValidateDataAdapter(fDesignerHost, XRChart.DataSource, XRChart.DataMember);
		}
		public override void Initialize(IComponent component) {
			base.Initialize(component);
			designer = new ChartDesigner(component as IChartContainer);
			chartDesignerActionList = new XRChartDesignerActionList(this, component);
			navigationController = new ChartNavigationController(XRChart);
		}
		public override void InitializeNewComponent(IDictionary defaultValues) {
			base.InitializeNewComponent(defaultValues);
			if (WizardFormBase.ShowWizardOnChartAdding && DesignSettings.Default.ChartOptions.ShowAnyEditor) {
				ShowWizard();
			}
		}
		protected internal void ShowWizard() {
			IDesignerHost designerHost = (IDesignerHost)GetService(typeof(IDesignerHost));
			if(designerHost != null) {
				if(DesignSettings.Default.ChartOptions.ShowDesigner) {
					DevExpress.XtraCharts.Designer.ChartDesigner chartDesigner = new DevExpress.XtraCharts.Designer.ChartDesigner(XRChart, designerHost);
					chartDesigner.ShowDialog();
				} else if(DesignSettings.Default.ChartOptions.ShowWizard) {
					ChartWizard wizard = CreateWizard(XRChart, designerHost);
					wizard.ShowDialog();
				}
			}
		}
		protected virtual ChartWizard CreateWizard(XRChart chart, IDesignerHost designerHost) {
			return new ChartWizard(chart, designerHost);
		}
		public override void OnComponentChanged(ComponentChangedEventArgs e) {
			base.OnComponentChanged(e);
			if (e.Member != null && (e.Member.Name == XRComponentPropertyNames.DataSource || e.Member.Name == XRComponentPropertyNames.DataMember))
				ValidateDataAdapter();
			InvalidateControl();
		}
		void InvalidateControl() {
			Control control = GetService(typeof(IBandViewInfoService)) as Control;
			if(control != null) control.Invalidate();
		}
		public void ValidateAdapter() {
			ValidateDataAdapter();
		}
	}
	public class XRChartDesignerActionList : ChartDesignerActionList {
		XRChartDesigner chartDesigner;
		public XRChartDesignerActionList(XRChartDesigner chartDesigner, IComponent component) : base(chartDesigner.ChartDesigner, component) {
			this.chartDesigner = chartDesigner;
		}
		public void FillCommonActionItems(DesignerActionItemCollection actionItems) {
			bool enabled = ChartDesigner.GetInheritanceAttribute(chartDesigner.XRChart.Chart.Container) == InheritanceAttribute.NotInherited;
			AnnotationsActionEnabled = enabled;
			SeriesActionEnabled = enabled;
			EditPalettesEnabled = enabled;
			WizardEnabled = enabled;
			SaveLayoutEnabled = true;
			LoadLayoutEnabled = enabled;
			AddWizardAction(actionItems, String.Empty);
			ReplaceLastItem(actionItems, "OnWizardAction");
			AddSeriesAction(actionItems, String.Empty);
			AddAnnotationsAction(actionItems, String.Empty);
			AddEditPalettesAction(actionItems, String.Empty);
		}
		public void FillSerializingActionItems(DesignerActionItemCollection actionItems) {
			AddSaveLayoutAction(actionItems, String.Empty);
			AddLoadLayoutAction(actionItems, String.Empty);
		}
		void ReplaceLastItem(DesignerActionItemCollection actionItems, string methodName) {
			int lastIndex = actionItems.Count - 1;
			DesignerActionMethodItem methodItem = actionItems[lastIndex] as DesignerActionMethodItem;
			if(methodItem != null) {
				actionItems[lastIndex] = new DesignerActionMethodItem(this, methodName, methodItem.DisplayName, methodItem.Category, methodItem.Description, methodItem.IncludeAsDesignerVerb);
			}
		}
		protected void OnWizardAction() {
			chartDesigner.ShowWizard();
		}
	}
	public class CommonXRChartItemsDesignerList : XRComponentDesignerActionList {
		XRChartDesignerActionList designerActionList;
		public CommonXRChartItemsDesignerList(XRComponentDesigner componentDesigner, XRChartDesignerActionList designerActionList) : base(componentDesigner) {
			this.designerActionList = designerActionList;
		}
		protected override void FillActionItemCollection(DesignerActionItemCollection actionItems) {
			designerActionList.FillCommonActionItems(actionItems);
		}
	}
	public class SerializingXRChartsItemsDesignerList : XRComponentDesignerActionList {
		XRChartDesignerActionList designerActionList;
		public SerializingXRChartsItemsDesignerList(XRComponentDesigner componentDesigner, XRChartDesignerActionList designerActionList) : base(componentDesigner) {
			this.designerActionList = designerActionList;
		}
		protected override void FillActionItemCollection(DesignerActionItemCollection actionItems) {
			designerActionList.FillSerializingActionItems(actionItems);
		}
	}
	public class DataSourceItemDesignerList : XRComponentDesignerActionList {
		XRChart chart;
		[
		TypeConverter(typeof(DataMemberTypeConverter)),
		Editor(typeof(DataContainerDataMemberEditor), typeof(UITypeEditor)),
		RefreshProperties(RefreshProperties.All)
		]
		public string DataMember {
			get { return chart.DataMember; }
			set {
				chart.DataMember = value;
				((XRChartDesigner)designer).ValidateAdapter();
			}
		}
		[
		Editor(typeof(DataAdapterEditor), typeof(UITypeEditor)),
		TypeConverterAttribute(typeof(DataAdapterConverter))
		]
		public object DataAdapter { 
			get { return chart.DataAdapter; } 
			set { chart.DataAdapter = value; } 
		}
		[
		Editor(typeof(DataSourceEditor), typeof(UITypeEditor)),
		TypeConverter(typeof(DataSourceConverter)),
		RefreshProperties(RefreshProperties.All),
		]
		public object DataSource {
			get { return chart.DataSource; }
			set {
				chart.DataSource = value;
				((XRChartDesigner)designer).ValidateAdapter();
			}
		}
		public DataSourceItemDesignerList(XRComponentDesigner componentDesigner, XRChart chart) : base(componentDesigner) {
			this.chart = chart;
		}
		protected override void FillActionItemCollection(DesignerActionItemCollection actionItems) {
			AddPropertyItem(actionItems, "DataSource", "DataSource");
			AddPropertyItem(actionItems, "DataMember", "DataMember");
			AddPropertyItem(actionItems, "DataAdapter", "DataAdapter");
		}
	}
}
