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
using System.Drawing;
using System.Windows.Forms;
using DevExpress.Charts.Native;
using DevExpress.XtraBars;
using DevExpress.XtraCharts.Designer.Native;
using DevExpress.XtraEditors;
namespace DevExpress.XtraCharts.Designer {
	public partial class ChartActionsControl : XtraUserControl {
		DesignerChartModel chartModel;
		DesignerChartElementModelBase selectedmodel;
		internal DesignerChartModel ChartModel {
			get { return chartModel; }
			set { 
				chartModel = value;
				OnChartModelChanged();
			}
		}
		internal DesignerChartElementModelBase SelectedModel {
			get { return selectedmodel; }
			set {
				if(selectedmodel != value){
					selectedmodel = value;
					UpdateButtonsState();
				}
			}
		}
		internal event ActionButtonPressedEventHandler ActionButtonPressed;
		public ChartActionsControl() {
			InitializeComponent();
		}
		void OnChartModelChanged() {
			UpdateButtonsState();
		}
		void UpdateButtonsState() {
			if (chartModel != null) {
				btnChangeType.Enabled = chartModel.Series.Count > 0;
				btnUndo.Enabled = chartModel.CommandManager.CanUndo;
				btnRedo.Enabled = chartModel.CommandManager.CanRedo;
				SetBarItemVisibility(bbiAddPane, chartModel.Diagram is XYDiagram2DModel);
				SetBarItemVisibility(bbiAddAxisX, chartModel.Diagram is XYDiagramModel || chartModel.Diagram is SwiftPlotDiagramModel);
				SetBarItemVisibility(bbiAddAxisY, chartModel.Diagram is XYDiagramModel || chartModel.Diagram is SwiftPlotDiagramModel);
				SetBarItemVisibility(bbiAddIndicator, selectedmodel is DesignerSeriesModel && ((DesignerSeriesModel)selectedmodel).View is XYDiagram2DViewBaseModel);
				SetBarItemVisibility(bbiAddSeriesTitle, selectedmodel is DesignerSeriesModel && ((DesignerSeriesModel)selectedmodel).View is SimpleDiagramViewBaseModel);
				SetBarItemVisibility(bhiSeriesElements, bbiAddIndicator.Visibility == BarItemVisibility.Always || bbiAddSeriesTitle.Visibility == BarItemVisibility.Always);
				SetBarItemVisibility(bbiAddConstantLine, selectedmodel is Axis2DModel);
				SetBarItemVisibility(bbiAddStrip, selectedmodel is Axis2DModel);
				SetBarItemVisibility(bbiAddScaleBreak, selectedmodel is AxisModel);
				SetBarItemVisibility(bhiAxisElements, bbiAddConstantLine.Visibility == BarItemVisibility.Always ||
					bbiAddStrip.Visibility == BarItemVisibility.Always || bbiAddScaleBreak.Visibility == BarItemVisibility.Always);
			}
			else {
				btnChangeType.Enabled = false;
				btnUndo.Enabled = false;
				btnRedo.Enabled = false;
			}
		}
		void SetBarItemVisibility(BarItem item, bool visible) {
			item.Enabled = visible;
		}
		void OnUndoClick(object sender, EventArgs e) {
			if(ActionButtonPressed != null)
				ActionButtonPressed(this, new ActionButtonPressedEventArgs(ActionButtonType.Undo));
		}
		void OnRedoClick(object sender, EventArgs e) {
			if (ActionButtonPressed != null)
				ActionButtonPressed(this, new ActionButtonPressedEventArgs(ActionButtonType.Redo));
		}
		void OnChangeTypeClick(object sender, EventArgs e) {
			var viewTypes = new SortedSet<ViewType>();
			foreach (ViewType viewType in Enum.GetValues(typeof(ViewType)))
				viewTypes.Add(viewType);
			using (var uc = new AddSeriesUserControl(viewTypes)) {
				using (var wrapperForm = new WrapperForm(uc)) {
					wrapperForm.ShowDialog();
					if (wrapperForm.DialogResult == DialogResult.OK) {
						chartModel.ChangeChartType((ViewType)wrapperForm.Result);
					}
				}
			}
		}
		void OnAddSeriesItemClick(object sender, ItemClickEventArgs e) {
			var viewTypes = new SortedSet<ViewType>();
			foreach (ViewType viewType in Enum.GetValues(typeof(ViewType)))
				if (chartModel.Series.IsCompartibleViewType(viewType))
					viewTypes.Add(viewType);
			using (var uc = new AddSeriesUserControl(viewTypes)) {
				using (var wrapperForm = new WrapperForm(uc)) {
					wrapperForm.ShowDialog();
					if (wrapperForm.DialogResult == DialogResult.OK)
						chartModel.Series.AddNewElement((ViewType)wrapperForm.Result);
				}
			}
		}
		void OnAddPaneItemClick(object sender, ItemClickEventArgs e) {
			AddCollectionElement(((XYDiagram2DModel)chartModel.Diagram).Panes);
		}
		void OnAddAxisXItemClick(object sender, ItemClickEventArgs e) {
			if(chartModel.Diagram is XYDiagramModel)
				AddCollectionElement(((XYDiagramModel)chartModel.Diagram).SecondaryAxesX);
			if (chartModel.Diagram is SwiftPlotDiagramModel)
				AddCollectionElement(((SwiftPlotDiagramModel)chartModel.Diagram).SecondaryAxesX);
		}
		void OnAddAxisYItemClick(object sender, ItemClickEventArgs e) {
			if (chartModel.Diagram is XYDiagramModel)
				AddCollectionElement(((XYDiagramModel)chartModel.Diagram).SecondaryAxesY);
			if (chartModel.Diagram is SwiftPlotDiagramModel)
				AddCollectionElement(((SwiftPlotDiagramModel)chartModel.Diagram).SecondaryAxesY);
		}
		void OnAddChartTitleItemClick(object sender, ItemClickEventArgs e) {
			AddCollectionElement(chartModel.Titles);
		}
		void OnAddAnnotationItemClick(object sender, ItemClickEventArgs e) {
			using (var uc = new AddAnnotationUserControl()) {
				using (var wrapperForm = new WrapperForm(uc)) {
					wrapperForm.ShowDialog();
					if (wrapperForm.DialogResult == DialogResult.OK)
						chartModel.AnnotationRepository.AddNewElement(wrapperForm.Result);
				}
			}
		}
		void OnAddIndicatorItemClick(object sender, ItemClickEventArgs e) {
			using (var uc = new AddIndicatorUserControl()) {
				using (var wrapperForm = new WrapperForm(uc)) {
					wrapperForm.ShowDialog();
					if (wrapperForm.DialogResult == DialogResult.OK) {
						var seriesModel = SelectedModel as DesignerSeriesModel;
						if (seriesModel == null) {
							ChartDebug.Fail("DesignerSeriesModel expected to add indicator.");
							return;
						}
						var xySeriesViewModel = seriesModel.View as XYDiagram2DViewBaseModel;
						if (xySeriesViewModel == null) {
							ChartDebug.Fail("XYDiagram2DViewBaseModel expected to add indicator.");
							return;
						}
						xySeriesViewModel.Indicators.AddNewElement(wrapperForm.Result);
					}
				}
			}
		}
		void OnAddSeriesTitleItemClick(object sender, ItemClickEventArgs e) {
			AddCollectionElement(((SimpleDiagramViewBaseModel)((DesignerSeriesModel)selectedmodel).View).Titles);
		}
		void OnAddConstantLineItemClick(object sender, ItemClickEventArgs e) {
			AddCollectionElement(((Axis2DModel)selectedmodel).ConstantLines);
		}
		void OnAddStripItemClick(object sender, ItemClickEventArgs e) {
			AddCollectionElement(((Axis2DModel)selectedmodel).Strips);
		}
		void OnAddScaleBreakItemClick(object sender, ItemClickEventArgs e) {
			AddCollectionElement(((AxisModel)selectedmodel).ScaleBreaks);
		}
		void OnSmartBindingItemClick(object sender, ItemClickEventArgs e) {
			using (SmartBindingForm optionsForm = new SmartBindingForm()) {
				Form parentForm = FindForm();
				optionsForm.Location = new Point(parentForm.Location.X + parentForm.Width / 2 - optionsForm.Width / 2, parentForm.Location.Y + parentForm.Height / 2 - optionsForm.Height / 2);
				optionsForm.ShowDialog();
			}
		}
		void OnSeriesBindingItemClick(object sender, ItemClickEventArgs e) {
			using (SeriesBindingForm optionsForm = new SeriesBindingForm()) {
				Form parentForm = FindForm();
				optionsForm.Location = new Point(parentForm.Location.X + parentForm.Width / 2 - optionsForm.Width / 2, parentForm.Location.Y + parentForm.Height / 2 - optionsForm.Height / 2);
				optionsForm.ShowDialog();
			}
		}
		void AddCollectionElement(ChartCollectionBaseModel collectionModel) {
			collectionModel.AddNewElement(null);
		}
		public void UpdateControls() {
			UpdateButtonsState();
		}
	}
}
namespace DevExpress.XtraCharts.Designer.Native {
	public enum ActionButtonType {
		Undo,
		Redo
	}
	public class ActionButtonPressedEventArgs : EventArgs {
		readonly ActionButtonType buttonType;
		public ActionButtonType ButtonType { get { return buttonType; } }
		public ActionButtonPressedEventArgs(ActionButtonType buttonType) {
			this.buttonType = buttonType;
		}
	}
	public delegate void ActionButtonPressedEventHandler(object sender, ActionButtonPressedEventArgs e);
}
