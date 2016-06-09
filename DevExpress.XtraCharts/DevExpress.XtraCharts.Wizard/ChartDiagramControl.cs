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
using DevExpress.XtraCharts.Native;
using DevExpress.XtraCharts.Wizard.ChartDiagramControls;
namespace DevExpress.XtraCharts.Wizard {
	internal partial class ChartDiagramControl : SplitterWizardControlWithPreview {
		static DiagramControlFactory Factory = new DiagramControlFactory();
		public ChartDiagramControl() {
			InitializeComponent();
		}
		public override void InitializeChart(WizardFormBase form) {
			base.InitializeChart(form);
			DiagramControlBase control = Factory.CreateInstance(Chart.Diagram);
			control.Initialize(Chart.Diagram, Chart, form.LookAndFeel, WizardForm.OriginalChart,((WizardDiagramPage)WizardPage).HiddenPageTabs);
			tabPanel.Controls.Add(control);
			control.Dock = DockStyle.Fill;
			DesignControl.NavigationController.EnableDragAndZoom = true;
			DesignControl.MouseMove += new MouseEventHandler(control.DesignControl_MouseMove);
			DesignControl.MouseWheel += new MouseEventHandler(control.DesignControl_MouseWheel);
			DesignControl.MouseWheelZooming = true;
		}
		public override void Release() {
			base.Release();
			DesignControl.NavigationController.EnableDragAndZoom = false;
			DesignControl.MouseWheelZooming = false;
		}
		public virtual void UpdateScrollingAndZooming() {
		}
	}
}
