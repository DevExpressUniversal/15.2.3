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
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraCharts.Native;
using DevExpress.XtraCharts.Wizard.ChartDiagramControls;
namespace DevExpress.XtraCharts.Wizard {
	internal partial class ChartPanesControl : SplitterWizardControlWithPreview {
		int isLockChanged;
		ItemContainerMap<XYDiagramPaneBase> panes;
		PaneControlBase currentPaneControl;
		object SelectedTabHandle { get { return currentPaneControl != null ? currentPaneControl.SelectedTabHandle : null; } }
		bool ShowPaneControl { get { return Chart.Diagram is XYDiagram2D; } }
		bool IsLockChanges { get { return isLockChanged > 0; } }
		public ChartPanesControl() {
			InitializeComponent();
			this.panes = new ItemContainerMap<XYDiagramPaneBase>(ExtractPaneName);
		}
		PaneControlBase CreatePaneControl(XYDiagramPaneBase pane) {
			if(pane != null)
				return new PaneControl();
			else
				return new PanesNotSupportedControl();
		}
		void ChangePaneControl(XYDiagramPaneBase pane) {
			PaneControlBase control = CreatePaneControl(pane);
			SelectPane(control, pane);
			if(!object.ReferenceEquals(control, this.currentPaneControl)) {
				pnlTab.SuspendLayout();
				if(this.currentPaneControl != null)
					control.Size = this.currentPaneControl.Size;
				this.pnlTab.Controls.Add(control);
				control.Dock = DockStyle.Fill;
				if(this.currentPaneControl != null)
					this.pnlTab.Controls.Remove(this.currentPaneControl);
				this.currentPaneControl = control;
				this.pnlTab.ResumeLayout();
			}
			SelectPaneAsHitElement(pane);
		}
		void SelectPaneAsHitElement(XYDiagramPaneBase pane) {
			if(pane == null)
				return;
			IHitTest hitTest = pane as IHitTest;
			if(hitTest == null)
				hitTest = Chart.Diagram as IHitTest;
			if(hitTest != null)
				Chart.SelectHitElement(hitTest);
		}
		void SelectPane(PaneControlBase control, XYDiagramPaneBase pane) {
			if(pane == null)
				return;
			control.Initialize(pane, cbPanes.UpdateText, WizardForm.OriginalChart, WizardLookAndFeel, ((WizardPanePage)WizardPage).HiddenPageTabs, SelectedTabHandle);
			try {
				Lock();
				this.cbPanes.SelectedItem = this.panes[pane];
			}
			finally {
				Unlock();
			}		   
		}
		string ExtractPaneName(XYDiagramPaneBase pane) {
			return pane.Name;
		}
		void designControl_ObjectSelected(object sender, HotTrackEventArgs args) {
			if(!ValidateContent()) {
				args.Cancel = true;
				return;
			}
			XYDiagram2D diagram = args.Object as XYDiagram2D;
			if(diagram != null)
				ChangePaneControl(diagram.DefaultPane);
			else {
				XYDiagramPane pane = args.Object as XYDiagramPane;
				if(pane != null)
					ChangePaneControl(pane);
				else
					args.Cancel = true;
			}
		}
		void designControl_ObjectHotTracked(object sender, HotTrackEventArgs args) {
			if(!(args.Object is XYDiagram2D) && !(args.Object is XYDiagramPane))
				args.Cancel = true;
		}
		void FillCombobox() {
			XYDiagram2D diagram = Chart.Diagram as XYDiagram2D;
			if(diagram != null) {
				cbPanes.Properties.Items.Add(panes.Add(diagram.DefaultPane));
				foreach(XYDiagramPane pane in diagram.Panes)
					cbPanes.Properties.Items.Add(panes.Add(pane));
			}
		}
		void Lock() {
			isLockChanged++;
		}
		void Unlock() {
			isLockChanged--;
		}
		void InitializePaneControl() {
			XYDiagram2D diagram = Chart.Diagram as XYDiagram2D;
			if(diagram != null)
				ChangePaneControl(diagram.DefaultPane);
			else
				ChangePaneControl(null);
		}
		void cbPanes_SelectedIndexChanged(object sender, EventArgs e) {
			if(!IsLockChanges)
				ChangePaneControl(((ItemContainer<XYDiagramPaneBase>)this.cbPanes.SelectedItem).Item);
		}
		public override void InitializeChart(WizardFormBase form) {
			base.InitializeChart(form);
			DesignControl.SelectionMode = ElementSelectionMode.Single;
			DesignControl.ObjectHotTracked += new HotTrackEventHandler(designControl_ObjectHotTracked);
			DesignControl.ObjectSelected += new HotTrackEventHandler(designControl_ObjectSelected);
			this.pnlPanes.Visible = ShowPaneControl;
			FillCombobox();
			try {
				Lock();
				InitializePaneControl();
			}
			finally {
				Unlock();
			}
		}
		public override void Release() {
			Chart.ClearSelection();
			DesignControl.SelectionMode = ElementSelectionMode.None;
			DesignControl.ObjectHotTracked -= new HotTrackEventHandler(designControl_ObjectHotTracked);
			DesignControl.ObjectSelected -= new HotTrackEventHandler(designControl_ObjectSelected);
			base.Release();
		}		
	}
}
