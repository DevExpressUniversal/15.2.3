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
using System.Windows.Forms;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraCharts.Native;
using DevExpress.XtraCharts.Design;
using DevExpress.XtraCharts.Localization;
using DevExpress.XtraCharts.Wizard.SeriesViewControls;
namespace DevExpress.XtraCharts.Wizard {
	internal partial class SeriesViewConfigControl : SplitterWizardControlWithPreview {
		class SeriesContainer {
			readonly SeriesBase series;
			readonly string seriesName;
			public SeriesBase Series { get { return series; } }
			public SeriesContainer(SeriesBase series, string seriesName) {
				this.series = series;
				this.seriesName = seriesName;
			}
			public override string ToString() {
				return seriesName;
			}
		}
		readonly static SeriesViewControlFactory factory = new SeriesViewControlFactory();
		SeriesViewControlBase seriesViewControl;
		public SeriesViewConfigControl() {
			InitializeComponent();
		}
		public override void InitializeChart(WizardFormBase form) {
			base.InitializeChart(form);
			Chart chart = Chart;
			ChartDesignControl designControl = DesignControl;
			designControl.SelectionMode = ElementSelectionMode.Single;
			if (!chart.Is3DDiagram) {
				designControl.ObjectHotTracked += new HotTrackEventHandler(designControl_ObjectHotTracked);
				designControl.ObjectSelected += new HotTrackEventHandler(designControl_ObjectSelected);
			}
			ComboBoxItemCollection items = chSeries.Properties.Items;
			foreach (Series series in chart.Series)
				if (!series.IsAutoCreated)
					items.Add(new SeriesContainer(series, series.Name));
			if (BindingHelper.HasBoundSeries(chart))
				items.Add(new SeriesContainer(chart.DataContainer.SeriesTemplate, ChartLocalizer.GetString(ChartStringId.WizAutoCreatedSeries)));
			if (items.Count > 0)
				chSeries.SelectedIndex = 0;
		}
		public override void Release() {
			Chart chart = Chart;
			ChartDesignControl designControl = DesignControl;
			chart.ClearSelection();
			designControl.SelectionMode = ElementSelectionMode.None;
			if (!chart.Is3DDiagram) {
				designControl.ObjectSelected -= new HotTrackEventHandler(designControl_ObjectSelected);
				designControl.ObjectHotTracked -= new HotTrackEventHandler(designControl_ObjectHotTracked);
			}
			base.Release();
		}
		void designControl_ObjectHotTracked(object sender, HotTrackEventArgs args) {
			if (!((args.Object is SeriesBase) || (args.Object is SeriesTitle) || (args.Object is Indicator)))
				args.Cancel = true;
		}
		void designControl_ObjectSelected(object sender, HotTrackEventArgs args) {
			object hitObject = args.Object;
			SeriesBase series = hitObject as SeriesBase;
			SeriesTitle title = hitObject as SeriesTitle;
			Indicator indicator = hitObject as Indicator;
			if (title != null)
				series = ChartDesignHelper.GetOwner<SeriesBase>(title);
			if (indicator != null)
				series = ChartDesignHelper.GetOwner<SeriesBase>(indicator);
			if (series == null)
				args.Cancel = true;
			else {
				foreach (SeriesContainer container in chSeries.Properties.Items)
					if (Object.ReferenceEquals(container.Series, series)) {
						chSeries.SelectedItem = container;
						break;
					}
				if (title != null)
					seriesViewControl.SelectHitTestElement(title);
				else if (indicator != null)
					seriesViewControl.SelectHitTestElement(indicator);
			}
		}
		void cbSeries_SelectedIndexChanged(object sender, EventArgs e) {
			if (chSeries.SelectedIndex > -1) {
				SeriesBase series = ((SeriesContainer)chSeries.SelectedItem).Series;
				SeriesViewBase view = series.View;
				tabPanel.SuspendLayout();
				SeriesViewControlBase control = factory.CreateInstance(view);
				control.Initialize(view, series, Chart, WizardLookAndFeel, 
					((WizardSeriesViewPage)WizardPage).HiddenPageTabs, seriesViewControl == null ? null : seriesViewControl.SelectedTabHandle);
				tabPanel.Controls.Add(control);
				if (seriesViewControl == null) 
					control.tbcPagesControl.SelectedTabPageIndex = 0;
				else {
					control.Size = seriesViewControl.Size;
					tabPanel.Controls.Remove(seriesViewControl);
					seriesViewControl.Dispose();
					seriesViewControl = null;
				}
				seriesViewControl = control;
				seriesViewControl.Dock = DockStyle.Fill;
				tabPanel.ResumeLayout();
				Chart.SelectHitElement(series);
			}
		}
	}
}
