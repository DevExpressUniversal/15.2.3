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
using System.Drawing;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Windows.Forms;
using DevExpress.Data.ChartDataSources;
using DevExpress.LookAndFeel;
using DevExpress.Skins;
using DevExpress.Utils.Controls;
using DevExpress.Utils.Serializing;
using DevExpress.XtraCharts.Native;
using DevExpress.XtraCharts.Design;
using DevExpress.XtraCharts.Wizard;
using DevExpress.XtraCharts.Localization;
using DevExpress.Utils;
namespace DevExpress.XtraCharts.Wizard {
	public enum WizardPageType {
		ChartType,
		Appearance,
		SeriesSettings,
		Data,
		Chart,
		View,
		SeriesLabels,
		Axes,
		Legend,
		Titles,
		Diagram,
		Panes,
		UserDefined,
		Annotations,
	}
	public class ChartWizard {
		internal const string XtraChartsRegistryPath = "Software\\Developer Express\\XtraCharts\\";
		internal const string XtraChartsShowWizardRegistryEntry = "ShowWizard";
		string caption;
		string description;
		Image leftImage;
		Icon icon;
		Image rightImage;
		WizardFormLayout layout;
		FilterSeriesTypesCollection filterSeriesCollection = new FilterSeriesTypesCollection();
		IChartContainer chartContainer;
		IDesignerHost designerHost;
		bool hideStartupCheckBox;
		PageOrderAlgorithm pageOrderAlg = PageOrderAlgorithm.GenerateDefaultWizard();
		internal WizardFormLayout Layout { get { return layout; } }
		internal IChartContainer ChartContainer { get { return chartContainer; } }
		internal IDesignerHost DesignerHost { get { return designerHost; } }
#if !SL
	[DevExpressXtraChartsWizardLocalizedDescription("ChartWizardConstructionGroup")]
#endif
		public WizardGroup ConstructionGroup { get { return pageOrderAlg.FindGroupByName(WizardGroup.CustructionGroupName); } }
#if !SL
	[DevExpressXtraChartsWizardLocalizedDescription("ChartWizardPresentationGroup")]
#endif
		public WizardGroup PresentationGroup { get { return pageOrderAlg.FindGroupByName(WizardGroup.PresentationGroupName); } }
#if !SL
	[DevExpressXtraChartsWizardLocalizedDescription("ChartWizardChartTypePage")]
#endif
		public WizardPage ChartTypePage { get { return pageOrderAlg.FindPageByPageType(WizardPageType.ChartType); } }
#if !SL
	[DevExpressXtraChartsWizardLocalizedDescription("ChartWizardAppearancePage")]
#endif
		public WizardPage AppearancePage { get { return pageOrderAlg.FindPageByPageType(WizardPageType.Appearance); } }
#if !SL
	[DevExpressXtraChartsWizardLocalizedDescription("ChartWizardSeriesPage")]
#endif
		public WizardSeriesPage SeriesPage { get { return (WizardSeriesPage)pageOrderAlg.FindPageByPageType(WizardPageType.SeriesSettings); } }
#if !SL
	[DevExpressXtraChartsWizardLocalizedDescription("ChartWizardDataPage")]
#endif
		public WizardDataPage DataPage { get { return (WizardDataPage)pageOrderAlg.FindPageByPageType(WizardPageType.Data); } }
#if !SL
	[DevExpressXtraChartsWizardLocalizedDescription("ChartWizardChartPage")]
#endif
		public WizardChartPage ChartPage { get { return (WizardChartPage)pageOrderAlg.FindPageByPageType(WizardPageType.Chart); } }
#if !SL
	[DevExpressXtraChartsWizardLocalizedDescription("ChartWizardDiagramPage")]
#endif
		public WizardDiagramPage DiagramPage { get { return (WizardDiagramPage)pageOrderAlg.FindPageByPageType(WizardPageType.Diagram); } }
#if !SL
	[DevExpressXtraChartsWizardLocalizedDescription("ChartWizardPanePage")]
#endif
		public WizardPanePage PanePage { get { return (WizardPanePage)pageOrderAlg.FindPageByPageType(WizardPageType.Panes); } }
#if !SL
	[DevExpressXtraChartsWizardLocalizedDescription("ChartWizardAxisPage")]
#endif
		public WizardAxisPage AxisPage { get { return (WizardAxisPage)pageOrderAlg.FindPageByPageType(WizardPageType.Axes); } }
#if !SL
	[DevExpressXtraChartsWizardLocalizedDescription("ChartWizardSeriesViewPage")]
#endif
		public WizardSeriesViewPage SeriesViewPage { get { return (WizardSeriesViewPage)pageOrderAlg.FindPageByPageType(WizardPageType.View); } }
#if !SL
	[DevExpressXtraChartsWizardLocalizedDescription("ChartWizardSeriesLabelsPage")]
#endif
		public WizardSeriesLabelsPage SeriesLabelsPage { get { return (WizardSeriesLabelsPage)pageOrderAlg.FindPageByPageType(WizardPageType.SeriesLabels); } }
#if !SL
	[DevExpressXtraChartsWizardLocalizedDescription("ChartWizardTitlePage")]
#endif
		public WizardTitlePage TitlePage { get { return (WizardTitlePage)pageOrderAlg.FindPageByPageType(WizardPageType.Titles); } }
#if !SL
	[DevExpressXtraChartsWizardLocalizedDescription("ChartWizardLegendPage")]
#endif
		public WizardLegendPage LegendPage { get { return (WizardLegendPage)pageOrderAlg.FindPageByPageType(WizardPageType.Legend); } }
#if !SL
	[DevExpressXtraChartsWizardLocalizedDescription("ChartWizardAnnotationPage")]
#endif
		public WizardAnnotationPage AnnotationPage { get { return (WizardAnnotationPage)pageOrderAlg.FindPageByPageType(WizardPageType.Annotations); } }
#if !SL
	[DevExpressXtraChartsWizardLocalizedDescription("ChartWizardGroups")]
#endif
		public WizardGroup[] Groups {
			get { return pageOrderAlg.GroupOrder; }
			set { pageOrderAlg.GroupOrder = value; }
		}
#if !SL
	[DevExpressXtraChartsWizardLocalizedDescription("ChartWizardFilterSeriesTypes")]
#endif
		public FilterSeriesTypesCollection FilterSeriesTypes { get { return filterSeriesCollection; } }
#if !SL
	[DevExpressXtraChartsWizardLocalizedDescription("ChartWizardIcon")]
#endif
		public Icon Icon {
			get { return icon; }
			set { icon = value; }
		}
#if !SL
	[DevExpressXtraChartsWizardLocalizedDescription("ChartWizardLeftImage")]
#endif
		public Image LeftImage {
			get { return leftImage; }
			set { leftImage = value; }
		}
#if !SL
	[DevExpressXtraChartsWizardLocalizedDescription("ChartWizardRightImage")]
#endif
		public Image RightImage {
			get { return rightImage; }
			set { rightImage = value; }
		}
#if !SL
	[DevExpressXtraChartsWizardLocalizedDescription("ChartWizardCaption")]
#endif
		public string Caption {
			get { return caption; }
			set { caption = value; }
		}
#if !SL
	[DevExpressXtraChartsWizardLocalizedDescription("ChartWizardDescription")]
#endif
		public string Description {
			get { return description; }
			set { description = value; }
		}
#if !SL
	[DevExpressXtraChartsWizardLocalizedDescription("ChartWizardSize")]
#endif
		public Size Size {
			get { return layout.Size; }
			set { layout.Size = value; }
		}
#if !SL
	[DevExpressXtraChartsWizardLocalizedDescription("ChartWizardLocation")]
#endif
		public Point Location {
			get { return layout.Location; }
			set { layout.Location = value; }
		}
#if !SL
	[DevExpressXtraChartsWizardLocalizedDescription("ChartWizardSplitterPositionPercentage")]
#endif
		public double SplitterPositionPercentage {
			get { return layout.SplitterPositionPercentage; }
			set {
				if (value < 0.0 || value > 100.0)
					throw new ArgumentException(ChartLocalizer.GetString(ChartStringId.MsgIncorrectPercentValue));
				layout.SplitterPositionPercentage = Convert.ToInt32(value);
			}
		}
#if !SL
	[DevExpressXtraChartsWizardLocalizedDescription("ChartWizardStartPosition")]
#endif
		public FormStartPosition StartPosition {
			get { return layout.StartPosition; }
			set { layout.StartPosition = value; }
		}
		public ChartWizard(object chart) : this(chart, null) {
		}
		public ChartWizard(object chart, IDesignerHost designerHost) {
			this.designerHost = designerHost;
			this.hideStartupCheckBox = designerHost == null;
			Assembly assembly = Assembly.GetAssembly(typeof(Chart));
			icon = ResourceImageHelper.CreateIconFromResources("DevExpress.XtraCharts.Design.Wizard.Images.chart.ico", assembly);
			caption = ChartLocalizer.GetString(ChartStringId.WizFormTitle);
			chartContainer = (IChartContainer)chart;
			layout = new WizardFormLayout(chartContainer.DesignMode, chartContainer.IsEndUserDesigner);
			if (chartContainer.ShouldEnableFormsSkins)
				SkinManager.EnableFormSkins();
		}
		public WizardGroup RegisterGroup(string groupName) {
			return pageOrderAlg.RegisterGroup(groupName);
		}
		public void UnregisterGroup(WizardGroup group) {
			pageOrderAlg.UnregisterGroup(group);
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
			IList<WizardPage> pageData = GenerateWizardPageDataCollection();
			using (WizardFormBase form = new WizardFormBase(this, pageData, lookAndFeel)) {
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
				form.FormLayout.SaveLayoutToRegistryIfDesignTime();
				return result;
			}
		}
		public void SaveLayoutToRegistry(string path) {
			layout.SaveLayoutToRegistry(path);
		}
		public void LoadLayoutFromRegistry(string path) {
			layout.LoadLayoutFromRegistry(path);
		}
		public int GetGroupIndex(WizardGroup group) {
			return pageOrderAlg.IndexOfGroup(group);
		}
		internal IList<WizardPage> GenerateWizardPageDataCollection() {
			List<WizardPage> pageData = new List<WizardPage>();
			foreach (WizardGroup group in pageOrderAlg)
				foreach (WizardPage page in group)
					pageData.Add(page);
			return pageData;
		}
	}
	public class FilterSeriesTypesCollection : CollectionBase {
		static int maxAllowFilterSeries = SeriesViewFactory.ViewTypes.Length - 1;
		internal FilterSeriesTypesCollection() {
		}
		public void Add(ViewType viewType) {
			if (InnerList.Contains(viewType))
				throw new ArgumentException(ChartLocalizer.GetString(ChartStringId.MsgAddPresentViewType));
			if (InnerList.Count == maxAllowFilterSeries)
				throw new ArgumentException(ChartLocalizer.GetString(ChartStringId.MsgAddLastViewType));
			InnerList.Add(viewType);
		}
		public void AddRange(ViewType[] viewTypes) {
			foreach (ViewType viewType in viewTypes)
				Add(viewType);
		}
		public void Remove(ViewType viewType) {
			InnerList.Remove(viewType);
		}
		internal bool Contains(ViewType viewType) {
			return InnerList.Contains(viewType);
		}
	}
}
namespace DevExpress.XtraCharts.Native {
	public class WizardFormLayout {
		const string DefaultKey = "WizardFormLayout";
		bool changed = false;
		bool designTime;
		bool isEndUserDesigner;
		bool maximized = false;
		Size dataContainerSize;
		Size size;
		Point location;
		int splitterPosition = 60;
		FormStartPosition formPosition = FormStartPosition.CenterScreen;
		[XtraSerializableProperty(XtraSerializationVisibility.Visible)]
		public bool Maximized {
			get { return maximized; }
			set {
				if (value != maximized) {
					maximized = value;
					changed = true;
				}
			}
		}
		[XtraSerializableProperty(XtraSerializationVisibility.Visible)]
		public Size DataContainerSize { 
			get { return dataContainerSize; } 
			set {
				if (value != dataContainerSize) {
					dataContainerSize = value;
					changed = true;
				}
			} 
		}
		[XtraSerializableProperty(XtraSerializationVisibility.Visible)]
		public Size Size { 
			get { return formPosition == FormStartPosition.WindowsDefaultBounds ? Size.Empty : size; } 
			set {
				if (value != size) {
					size = value;
					changed = true;
				}
			} 
		}
		[XtraSerializableProperty(XtraSerializationVisibility.Visible)]
		public Point Location { 
			get { return formPosition == FormStartPosition.Manual ? location : Point.Empty; } 
			set {
				if (value != location) {
					location = value;
					changed = true;
				} 
			}
		}
		[XtraSerializableProperty(XtraSerializationVisibility.Visible)]
		public int SplitterPositionPercentage { 
			get { return splitterPosition; } 
			set {
				if (value != splitterPosition) {
					splitterPosition = value;
					changed = true;
				}
			} 
		}
		[XtraSerializableProperty(XtraSerializationVisibility.Visible)]
		public FormStartPosition StartPosition { 
			get { return formPosition; } 
			set {
				if (value != formPosition) {
					formPosition = value;
					changed = true;
				}
			} 
		}
		public bool DesignTime { get { return designTime; } }
		public bool IsEndUserDesigner { get { return isEndUserDesigner; } }
		public WizardFormLayout(bool designTime, bool isEndUserDesigner) {
			this.designTime = designTime;
			this.isEndUserDesigner = isEndUserDesigner;
			LoadLayoutFromRegistryIfDesignTime();
		}
		public void SaveLayoutToRegistryIfDesignTime() {
			if (designTime && changed)
				SaveLayoutToRegistry(ChartWizard.XtraChartsRegistryPath);
		}
		public void SaveLayoutToRegistry(string registryNode) {
			if (designTime)
				formPosition = FormStartPosition.Manual;
			SaveLayout(registryNode);
		}
		public void LoadLayoutFromRegistryIfDesignTime() {
			if (designTime)
				LoadLayoutFromRegistry(ChartWizard.XtraChartsRegistryPath);
		}
		public void LoadLayoutFromRegistry(string registryNode) {
			try {
				LoadLayout(registryNode);
			}
			catch {
			}
		}
		public void SaveLayout(string registryNode) {
			new RegistryXtraSerializer().SerializeObject(this, registryNode, DefaultKey);
		}
		public void LoadLayout(string registryNode) {
			new RegistryXtraSerializer().DeserializeObject(this, registryNode, DefaultKey);
		}
	}
	public class PivotGridDataSourceOptionsSnapshot {
		public static PivotGridDataSourceOptionsSnapshot Create(IPivotGrid pivotGrid) {
			return pivotGrid == null ? null : new PivotGridDataSourceOptionsSnapshot(pivotGrid);
		}
		bool retrieveDataByColumns;
		bool retrieveEmptyCells;
		bool selectionOnly;
		bool singlePageOnly;
		bool retrieveColumnTotals;
		bool retrieveColumnGrandTotals;
		bool retrieveColumnCustomTotals;
		bool retrieveRowTotals;
		bool retrieveRowGrandTotals;
		bool retrieveRowCustomTotals;
		int maxAllowedSeriesCount;
		int maxAllowedPointCountInSeries;
		int updateDelay;
		PivotGridDataSourceOptionsSnapshot(IPivotGrid pivotGrid) {
			retrieveDataByColumns = pivotGrid.RetrieveDataByColumns;
			retrieveEmptyCells = pivotGrid.RetrieveEmptyCells;
			selectionOnly = pivotGrid.SelectionOnly;
			singlePageOnly = pivotGrid.SinglePageOnly;
			retrieveColumnTotals = pivotGrid.RetrieveColumnTotals;
			retrieveColumnGrandTotals = pivotGrid.RetrieveColumnGrandTotals;
			retrieveColumnCustomTotals = pivotGrid.RetrieveColumnCustomTotals;
			retrieveRowTotals = pivotGrid.RetrieveRowTotals;
			retrieveRowGrandTotals = pivotGrid.RetrieveRowGrandTotals;
			retrieveRowCustomTotals = pivotGrid.RetrieveRowCustomTotals;
			maxAllowedSeriesCount = pivotGrid.MaxAllowedSeriesCount;
			maxAllowedPointCountInSeries = pivotGrid.MaxAllowedPointCountInSeries;
			updateDelay = pivotGrid.UpdateDelay;
		}
		public void RestoreOptions(IPivotGrid pivotGrid) {
			if (pivotGrid == null)
				return;
			pivotGrid.LockListChanged();
			try {
				pivotGrid.RetrieveDataByColumns = pivotGrid.RetrieveDataByColumns;
				pivotGrid.RetrieveEmptyCells = retrieveEmptyCells;
				pivotGrid.SelectionOnly = selectionOnly;
				pivotGrid.SinglePageOnly = singlePageOnly;
				pivotGrid.RetrieveColumnTotals = retrieveColumnTotals;
				pivotGrid.RetrieveColumnGrandTotals = retrieveColumnGrandTotals;
				pivotGrid.RetrieveColumnCustomTotals = retrieveColumnCustomTotals;
				pivotGrid.RetrieveRowTotals = retrieveRowTotals;
				pivotGrid.RetrieveRowGrandTotals = retrieveRowGrandTotals;
				pivotGrid.RetrieveRowCustomTotals = retrieveRowCustomTotals;
				pivotGrid.MaxAllowedSeriesCount = maxAllowedSeriesCount;
				pivotGrid.MaxAllowedPointCountInSeries = maxAllowedPointCountInSeries;
				pivotGrid.UpdateDelay = updateDelay;
			}
			finally {
				pivotGrid.UnlockListChanged();
			}
		}
	}
}
