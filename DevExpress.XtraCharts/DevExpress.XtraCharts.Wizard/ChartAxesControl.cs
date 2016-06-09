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
using DevExpress.Charts.Native;
using DevExpress.XtraCharts.Native;
using DevExpress.XtraCharts.Design;
using DevExpress.XtraCharts.Wizard.ChartAxesControls;
namespace DevExpress.XtraCharts.Wizard {
	internal partial class ChartAxesControl : SplitterWizardControlWithPreview {
		readonly ItemContainerMap<AxisBase> axes;
		AxisControlBase currentAxisControl;
		int changeLocked = 0;
		public ChartAxesControl() {
			InitializeComponent();
			axes = new ItemContainerMap<AxisBase>(delegate(AxisBase axis) { return axis.Name; });
		}
		public override void InitializeChart(WizardFormBase form) {
			base.InitializeChart(form);
			ChartDesignControl designControl = DesignControl;
			designControl.SelectionMode = ElementSelectionMode.Single;
			designControl.ObjectHotTracked += new HotTrackEventHandler(designControl_ObjectHotTracked);
			designControl.ObjectSelected += new HotTrackEventHandler(designControl_ObjectSelected);
			pnlAxes.Visible = Chart.Diagram is IXYDiagram;
			Lock();
			try {
				Initialize();
			}
			finally {
				Unlock();
			}
		}
		public override void Release() {
			Chart.ClearSelection();
			ChartDesignControl designControl = DesignControl;
			designControl.SelectionMode = ElementSelectionMode.None;
			designControl.ObjectHotTracked -= new HotTrackEventHandler(designControl_ObjectHotTracked);
			designControl.ObjectSelected -= new HotTrackEventHandler(designControl_ObjectSelected);
			base.Release();
		}
		void Lock() {
			changeLocked++;
		}
		void Unlock() {
			changeLocked--;
		}
		void ChangeAxisControl(AxisBase axis) {
			AxisControlBase control = null;
			Diagram diagram = Chart.Diagram;
			if (diagram is XYDiagram2D) {
				if (!(currentAxisControl is ChartAxisControl))
					control = new ChartAxisControl();
			}
			else if (diagram is XYDiagram3D) {
				if (!(currentAxisControl is Axis3DControl))
					control = new Axis3DControl();
			}
			else if (diagram is RadarDiagram) {
 				if (axis is RadarAxisX) {
					if (!(currentAxisControl is RadarAxisXControl))
						control = new RadarAxisXControl();
				}
				else if (!(currentAxisControl is RadarAxisYControl))
					control = new RadarAxisYControl();
			}
			else if (!(currentAxisControl is AxesNotSupportedControl))
				control = new AxesNotSupportedControl();
			object selectedTabHandle = currentAxisControl == null ? null : currentAxisControl.SelectedTabHandle;
			if (control != null) {
				tabPanel.SuspendLayout();
				try {
					if (currentAxisControl != null)
						control.Size = currentAxisControl.Size;
					tabPanel.Controls.Add(control);
					control.Dock = DockStyle.Fill;
					if (currentAxisControl != null)
						tabPanel.Controls.Remove(currentAxisControl);
					currentAxisControl = control;
				}
				finally {
					tabPanel.ResumeLayout();
				}
			}
			if (axis != null) {
				currentAxisControl.Initialize(axis, Chart, cbAxes.UpdateText, 
					WizardLookAndFeel, ((WizardAxisPage)WizardPage).HiddenPageTabs, selectedTabHandle);
				Lock();
				try {
					cbAxes.SelectedItem = axes[axis];
				}
				finally {
					Unlock();
				}
			}
			Chart.SelectHitElement(axis);
		}
		void Initialize() {
			XYDiagram2D diagram2D = Chart.Diagram as XYDiagram2D;
			if (diagram2D == null) {
				XYDiagram3D diagram3D = Chart.Diagram as XYDiagram3D;
				if (diagram3D == null) {
					RadarDiagram radarDiagram = Chart.Diagram as RadarDiagram;
					if (radarDiagram == null)
						ChangeAxisControl(null);
					else {
						cbAxes.Properties.Items.Add(axes.Add(radarDiagram.AxisX));
						cbAxes.Properties.Items.Add(axes.Add(radarDiagram.AxisY));
						ChangeAxisControl(radarDiagram.AxisX);
					}
				}
				else {
					cbAxes.Properties.Items.Add(axes.Add(diagram3D.AxisX));
					cbAxes.Properties.Items.Add(axes.Add(diagram3D.AxisY));
					ChangeAxisControl(diagram3D.AxisX);
				}
			}
			else {
				foreach (AxisBase axis in diagram2D.GetAllAxesX())
					cbAxes.Properties.Items.Add(axes.Add(axis));
				foreach (AxisBase axis in diagram2D.GetAllAxesY())
					cbAxes.Properties.Items.Add(axes.Add(axis));
				ChangeAxisControl(((IXYDiagram2D)diagram2D).AxisX);
			}
		}
		void designControl_ObjectHotTracked(object sender, HotTrackEventArgs args) {
			if (!(args.Object is AxisBase))
				args.Cancel = true;
		}
		void designControl_ObjectSelected(object sender, HotTrackEventArgs args) {
			AxisBase axis = args.Object as AxisBase;
			if (axis == null || !ValidateContent())
				args.Cancel = true;
			else
				ChangeAxisControl(axis);
		}
		void cbAxes_SelectedIndexChanged(object sender, EventArgs e) {
			if (changeLocked == 0)
				ChangeAxisControl(((ItemContainer<AxisBase>)cbAxes.SelectedItem).Item);
		}
	}
}
