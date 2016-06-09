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
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
namespace DevExpress.XtraCharts.Designer.Native {
	public partial class ModelSelectControl : XtraUserControl {
		internal class NamedModelPresentation {
			readonly string name;
			ChartElementNamedModel elementModel;
			public ChartElementNamedModel ElementModel { get { return elementModel; } }
			public NamedModelPresentation() {
			}
			public NamedModelPresentation(string name, ChartElementNamedModel element) {
				this.name = name;
				this.elementModel = element;
			}
			public override string ToString() {
				return name;
			}
		}
		ChartElementNamedModel currentModel;
		XYDiagram2DModel diagramModel;
		public event EventHandler ValueChanged;
		internal ComboBoxEdit BoxEdit {
			get { return comboBox; }
		}
		internal ChartElementNamedModel Element {
			get {
				return currentModel;
			}
			set {
				if (value != currentModel) {
					currentModel = value;
					OnPaneChanged();
					UpdateSelectedIndex();
				}
			}
		}
		public XYDiagram2DModel DiagramModel {
			get { return diagramModel; }
			set {
				if (value != diagramModel) {
					diagramModel = value;
					OnDiagramModelChanged();
				}
			}
		}
		public ModelSelectControl() {
			InitializeComponent();
			comboBox.SelectedIndexChanged += paneBoxEdit_SelectedIndexChanged;
			EnableIXtraResizeableControlInterfaceProxy = true;
		}
		void paneBoxEdit_SelectedIndexChanged(object sender, EventArgs e) {
			if (comboBox.SelectedItem == null)
				return;
			Element = ((NamedModelPresentation)comboBox.SelectedItem).ElementModel;
		}
		void OnDiagramModelChanged() {
			UpdateComboBox();
		}
		void OnPaneChanged() {
			if (ValueChanged != null)
				ValueChanged(this, new EventArgs());
		}
		void UpdateSelectedIndex() {
			foreach (NamedModelPresentation presentation in comboBox.Properties.Items) {
				if (presentation.ElementModel.Equals(Element)) {
					if (comboBox.SelectedItem != presentation)
						comboBox.SelectedItem = presentation;
					return;
				}
			}
		}
		public virtual void UpdateComboBox() { }
	}
}
