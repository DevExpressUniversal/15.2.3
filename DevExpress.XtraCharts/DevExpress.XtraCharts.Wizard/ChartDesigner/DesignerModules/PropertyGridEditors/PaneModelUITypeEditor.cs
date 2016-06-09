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
using System.Drawing.Design;
using System.Windows.Forms.Design;
using DevExpress.Charts.Native;
using DevExpress.XtraCharts.Design;
using DevExpress.XtraCharts.Native;
using DevExpress.XtraEditors;
namespace DevExpress.XtraCharts.Designer.Native {
	public class PaneModelUITypeEditor : UITypeEditor {
		IWindowsFormsEditorService editorService;
		void OnListBoxClick(object sender, EventArgs e) {
			this.editorService.CloseDropDown();
		}
		List<XYDiagramPaneBaseModel> GetAllPanes(XYDiagramPaneBaseModel pane, XYDiagram2DModel xyDiagram) {
			List<XYDiagramPaneBaseModel> result = new List<XYDiagramPaneBaseModel>();
			result.Add(xyDiagram.DefaultPane);
			foreach (XYDiagramPaneModel additionalPane in xyDiagram.Panes)
				result.Add(additionalPane);
			return result;
		}
		bool ArePaneModelsEqual(XYDiagramPaneBaseModel paneModel1, XYDiagramPaneBaseModel paneModel2) {
			XYDiagramPaneBase pane1 = paneModel1.Pane;
			XYDiagramPaneBase pane2 = paneModel2.Pane;
			return object.Equals(pane1, pane2);
		}
		public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context) {
			return UITypeEditorEditStyle.DropDown;
		}
		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value) {
			this.editorService = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
			var pane = value as XYDiagramPaneBaseModel;
			DesignerChartModel chartModel = pane.FindParent<DesignerChartModel>();
			XYDiagram2DModel xyDiagram = chartModel.Diagram as XYDiagram2DModel;
			IEnumerable<XYDiagramPaneBaseModel> panes = GetAllPanes(pane, xyDiagram);
			var listBox = new ListBoxControl();
			listBox.Click += OnListBoxClick;
			foreach (XYDiagramPaneBaseModel paneModel in panes) {
				int index = listBox.Items.Add(paneModel);
				if(ArePaneModelsEqual(pane, paneModel)) {
					listBox.SelectedIndex = index+1;
				}
			}
			this.editorService.DropDownControl(listBox);
			return listBox.SelectedItem;
		}
	}
}
