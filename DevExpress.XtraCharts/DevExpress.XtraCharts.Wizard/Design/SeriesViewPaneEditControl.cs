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
using System.ComponentModel;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using System.Collections.Generic;
using DevExpress.XtraEditors;
using DevExpress.XtraCharts.Native;
namespace DevExpress.XtraCharts.Design {
	public partial class SeriesViewPaneEditControl : XtraUserControl {
		class PaneItem {
			readonly XYDiagramPaneBase pane;
			public XYDiagramPaneBase Pane { get { return pane; } }
			public PaneItem(XYDiagramPaneBase pane) {
				this.pane = pane;
			}
			public override string ToString() {
				return pane.Name;
			}
			public override bool Equals(object obj) {
				PaneItem item = obj as PaneItem;
				return item != null && Object.ReferenceEquals(item.pane, pane);
			}
			public override int GetHashCode() {
				return base.GetHashCode();
			}
		}
		readonly IWindowsFormsEditorService edSvc;
		readonly XYDiagram2D diagram;
		XYDiagramPaneBase currentPane;
		bool shouldClose = false;
		bool loading = false;
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public XYDiagramPaneBase CurrentPane { get { return currentPane; } }
		SeriesViewPaneEditControl() {
			InitializeComponent();
		}
		public SeriesViewPaneEditControl(IWindowsFormsEditorService edSvc, XYDiagram2D diagram, XYDiagramPaneBase currentPane) : this() {
			this.edSvc = edSvc;
			this.diagram = diagram;
			this.currentPane = currentPane;
			loading = true;
			try {
				List<XYDiagramPaneBase> allPanes = diagram.GetAllPanes();
				foreach (XYDiagramPaneBase pane in allPanes)
					lbPanes.Items.Add(new PaneItem(pane));
				lbPanes.SelectedItem = new PaneItem(currentPane);
			}
			finally {
				loading = false;
			}
		}
		void lbPanes_MouseClick(object sender, MouseEventArgs e) {
			if (e.Button == MouseButtons.Left)
				shouldClose = true;
		}
		void lbPanes_SelectedIndexChanged(object sender, EventArgs e) {
			if (!loading) {
				currentPane = ((PaneItem)lbPanes.SelectedValue).Pane;
				if (shouldClose)
					edSvc.CloseDropDown();
			}
		}
		void btnNewPane_Click(object sender, EventArgs e) {
			XYDiagramPane pane = new XYDiagramPane(diagram.Panes.GenerateName());
			CommonUtils.AddWithoutChanged(diagram.Panes, pane);
			currentPane = pane;
			edSvc.CloseDropDown();
		}
	}
}
