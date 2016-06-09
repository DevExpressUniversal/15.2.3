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
using System.Data;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraCharts.Native;
using DevExpress.XtraCharts.Wizard.SeriesLabelsControls;
using DevExpress.XtraCharts.Localization;
namespace DevExpress.XtraCharts.Wizard {
	internal partial class SeriesLabelsControl : SplitterWizardControlWithPreview {
		#region
		class SeriesLabelContainer {
			SeriesBase series;
			string seriesName;
			public SeriesLabelBase Label { get { return series.Label; } }
			public SeriesBase Series { get { return series; } }
			public SeriesLabelContainer(SeriesBase series, string seriesName) {
				this.series = series;
				this.seriesName = seriesName;
			}
			public override string ToString() {
				return seriesName;
			}
		}
		class SeriesLabelContainers : IEnumerable<SeriesLabelContainer> {
			#region IEnumerable Implementation
			IEnumerator<SeriesLabelContainer> IEnumerable<SeriesLabelContainer>.GetEnumerator() {
				return labelContainers.GetEnumerator();
			}
			System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
				return labelContainers.GetEnumerator();
			}
			#endregion
			readonly List<SeriesLabelContainer> labelContainers = new List<SeriesLabelContainer>();
			public void Add(SeriesLabelContainer container) {
				labelContainers.Add(container);
			}
			public SeriesLabelContainer GetBySeries(SeriesBase series) {
				foreach (SeriesLabelContainer container in labelContainers) {
					if (Object.ReferenceEquals(container.Series, series))
						return container;
				}
				return null;
			}
			public SeriesLabelContainer GetByLabel(SeriesLabelBase label) {
				foreach (SeriesLabelContainer container in labelContainers) {
					if (Object.ReferenceEquals(container.Label, label))
						return container;
				}
				return null;
			}
		}
		#endregion
		LabelControlBase currentControl;
		SeriesLabelContainer seriesTemplateContainer;
		SeriesLabelContainers labelContainers;
		object SelectedTabHandle { get { return currentControl != null ? currentControl.SelectedTabHandle : null; } }
		public SeriesLabelsControl() {
			InitializeComponent();
		}
		public override void InitializeChart(WizardFormBase form) {
			base.InitializeChart(form);
			DesignControl.SelectionMode = ElementSelectionMode.Single;
			if (!Chart.Is3DDiagram) {
				DesignControl.ObjectHotTracked += new HotTrackEventHandler(designControl_ObjectHotTracked);
				DesignControl.ObjectSelected += new HotTrackEventHandler(designControl_ObjectSelected);
			}
			this.labelContainers = new SeriesLabelContainers();
			foreach (Series series in Chart.Series)
				if (!series.IsAutoCreated)
					this.labelContainers.Add(new SeriesLabelContainer(series, series.Name));
			if (BindingHelper.HasBoundSeries(Chart)) {
				this.seriesTemplateContainer = new SeriesLabelContainer(DataContainer.SeriesTemplate,
					ChartLocalizer.GetString(ChartStringId.WizAutoCreatedSeries));
				this.labelContainers.Add(this.seriesTemplateContainer);
			}
			foreach (SeriesLabelContainer container in this.labelContainers)
				this.chSeries.Properties.Items.Add(container);
			tabPanel.Enabled = Chart.Series.Count > 0 || BindingHelper.HasBoundSeries(Chart);
			SeriesBase normalSeries = GetFirstNormalSeries();
			SeriesLabelContainer normalLabelContainer = this.labelContainers.GetBySeries(normalSeries);
			if (normalLabelContainer != null)
				ChangeSeriesLabelsControl(normalLabelContainer);
			chSeries.SelectedIndex = 0;
		}
		public override void Release() {
			Chart.ClearSelection();
			DesignControl.SelectionMode = ElementSelectionMode.None;
			if (!Chart.Is3DDiagram) {
				DesignControl.ObjectHotTracked -= new HotTrackEventHandler(designControl_ObjectHotTracked);
				DesignControl.ObjectSelected -= new HotTrackEventHandler(designControl_ObjectSelected);
			}
			base.Release();
		}
		void ChangeSeriesLabelsControl(SeriesLabelContainer labelContainer) {
			this.tabPanel.SuspendLayout();
			LabelControlBase control;
			if (labelContainer.Label != null)
				control = new SeriesLabelsControls.LabelControl();
			else
				control = new SeriesLabelNotSupportedControl();
			control.Initialize(labelContainer.Label, labelContainer.Series, Chart, WizardLookAndFeel, ((WizardSeriesLabelsPage)WizardPage).HiddenPageTabs, SelectedTabHandle);
			tabPanel.Controls.Add(control);
			if (this.currentControl != null) {
				control.Size = this.currentControl.Size;
				tabPanel.Controls.Remove(this.currentControl);
				this.currentControl.Dispose();
				this.currentControl = null;
			}
			this.currentControl = control;
			this.currentControl.Dock = DockStyle.Fill;
			this.tabPanel.ResumeLayout();
			Chart.SelectHitElement(labelContainer.Label);
		}
		SeriesBase GetFirstNormalSeries() {
			foreach (Series series in Chart.Series)
				if (!series.IsAutoCreated)
					return series;
			return DataContainer.SeriesTemplate;
		}
		private void chSeries_SelectedIndexChanged(object sender, EventArgs e) {
			SeriesLabelContainer container = (SeriesLabelContainer)chSeries.SelectedItem;
			if (container == null)
				return;
			ChangeSeriesLabelsControl(container);
		}
		void designControl_ObjectSelected(object sender, HotTrackEventArgs args) {
			SeriesLabelBase selectedLabel = args.Object as SeriesLabelBase;
			if (selectedLabel == null || this.labelContainers.GetByLabel(selectedLabel) == null)
				args.Cancel = true;
			else {
				foreach (SeriesLabelContainer container in this.chSeries.Properties.Items) {
					if (object.ReferenceEquals(container.Label, selectedLabel)) {
						this.chSeries.SelectedItem = container;
						return;
					}
				}
				this.chSeries.SelectedItem = this.seriesTemplateContainer;
			}
		}
		void designControl_ObjectHotTracked(object sender, HotTrackEventArgs args) {
			if (!(args.Object is SeriesLabelBase)) {
				args.Cancel = true;
				return;
			}
			SeriesLabelBase label = (SeriesLabelBase)args.Object;
			if (label == null || this.labelContainers.GetByLabel(label) == null)
				args.Cancel = true;
		}
	}
}
