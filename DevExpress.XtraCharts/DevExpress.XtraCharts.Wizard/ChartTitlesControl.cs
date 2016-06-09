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

using DevExpress.XtraCharts.Native;
using DevExpress.XtraCharts.Design;
namespace DevExpress.XtraCharts.Wizard {
	internal partial class ChartTitlesControl : SplitterWizardControlWithPreview {
		ChartTitle tempChartTitle = new ChartTitle();
		public ChartTitlesControl() {
			InitializeComponent();
		}
		public override void InitializeChart(DevExpress.XtraCharts.Native.WizardFormBase form) {
			base.InitializeChart(form);
			ChartDesignControl designControl = DesignControl;
			designControl.SelectionMode = ElementSelectionMode.Single;
			Chart chart = Chart;
			ChartTitleCollection titles = chart.Titles;
			if (titles.Count > 0) {
				titleControl.Enabled = true;
				titleListRedactControl.Initialize(titles);
			}
			else {
				titleControl.Enabled = false;
				titleListRedactControl.SetCollection(titles);
				titleControl.Initialize(tempChartTitle, AddTempTitleAndInitialize, WizardLookAndFeel, ((WizardTitlePage)WizardPage).HiddenPageTabs);
			}
			if (!chart.Is3DDiagram) {
				designControl.ObjectHotTracked += new HotTrackEventHandler(designControl_ObjectHotTracked);
				designControl.ObjectSelected += new HotTrackEventHandler(designControl_ObjectSelected);
			}
		}
		public override void Release() {
			Chart chart = Chart;
			chart.ClearSelection();
			ChartDesignControl designControl = DesignControl;
			if (!chart.Is3DDiagram) {
				designControl.ObjectHotTracked -= new HotTrackEventHandler(designControl_ObjectHotTracked);
				designControl.ObjectSelected -= new HotTrackEventHandler(designControl_ObjectSelected);
			}
			designControl.SelectionMode = ElementSelectionMode.None;
			base.Release();
		}
		void titleListRedactControl_SelectedElementChanged() {
			ChartTitle title = titleListRedactControl.CurrentElement as ChartTitle;
			if (title == null)
				title = tempChartTitle;
			titleControl.Initialize(title, WizardLookAndFeel, 
				((WizardTitlePage)WizardPage).HiddenPageTabs, titleControl == null ? null : titleControl.SelectedTabHandle);
			if (titleListRedactControl.CurrentElement == null) 
				titleControl.Enabled = false;
			else {
				titleControl.Enabled = true;
				Chart.SelectHitElement(title);
			}
		}
		void designControl_ObjectSelected(object sender, HotTrackEventArgs args) {
			ChartTitle selectedTitle = args.Object as ChartTitle;
			if (selectedTitle == null)
				args.Cancel = true;
			else 
				titleListRedactControl.CurrentElement = selectedTitle;
		}
		void designControl_ObjectHotTracked(object sender, HotTrackEventArgs args) {
			if (!(args.Object is ChartTitle))
				args.Cancel = true;
		}
		void AddTempTitleAndInitialize(DockableTitle title) {
			ChartTitleCollection titles = Chart.Titles;
			titles.Add((ChartTitle)title);
			titleListRedactControl.Initialize(titles);
			titleListRedactControl.CurrentElement = title;
		}
	}
}
