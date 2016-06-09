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
using System.Windows.Forms.Design;
using DevExpress.XtraEditors;
namespace DevExpress.XtraCharts.Design {
	public abstract partial class SeriesViewAxisEditControl : XtraUserControl {
		#region inner classes
		class AxisItem {
			Axis2D axis;
			public Axis2D Axis { get { return axis; } }
			public AxisItem(Axis2D axis) {
				this.axis = axis;
			}
			public override string ToString() {
				return axis.Name;
			}
		}
		#endregion
		IWindowsFormsEditorService edSvc;
		XYDiagram2D diagram;
		Axis2D currentAxis;
		bool shouldClose = false;
		bool loading = false;
		protected XYDiagram2D Diagram { get { return diagram; } }
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public Axis2D CurrentAxis { get { return currentAxis; } }
		SeriesViewAxisEditControl() {
			InitializeComponent();
		}
		public SeriesViewAxisEditControl(IWindowsFormsEditorService edSvc, XYDiagram2D diagram, Axis2D currentAxis) : this() {
			this.edSvc = edSvc;
			this.diagram = diagram;
			this.currentAxis = currentAxis;
			loading = true;
			InitializeListBox();
			loading = false;
		}
		void InitializeListBox() {
			List<Axis2D> totalAxes = GetTotalAxes();
			foreach(Axis2D axis in totalAxes)
				lbAxes.Items.Add(new AxisItem(axis));
			lbAxes.SelectedIndex = totalAxes.IndexOf(currentAxis);
		}
		void Close() {
			edSvc.CloseDropDown();
		}
		void lbAxes_MouseClick(object sender, MouseEventArgs e) {
			if(e.Button == MouseButtons.Left)
				shouldClose = true;
		}
		void lbAxes_SelectedIndexChanged(object sender, EventArgs e) {
			if(!loading) {
				currentAxis = ((AxisItem)lbAxes.SelectedValue).Axis;
				if(shouldClose)
					Close();
			}
		}
		void btnNewAxis_Click(object sender, EventArgs e) {
			currentAxis = CreateNewAxis();
			Close();
		}
		protected abstract List<Axis2D> GetTotalAxes();
		protected abstract Axis2D CreateNewAxis();
	}
}
