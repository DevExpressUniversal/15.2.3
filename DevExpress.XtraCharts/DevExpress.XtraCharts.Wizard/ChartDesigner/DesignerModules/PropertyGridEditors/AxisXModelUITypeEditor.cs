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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using DevExpress.Charts.Native;
using DevExpress.LookAndFeel;
using DevExpress.XtraCharts.Design;
using DevExpress.XtraCharts.Native;
using DevExpress.XtraEditors;
namespace DevExpress.XtraCharts.Designer.Native {
	public abstract class AxisModelUITypeEditor : UITypeEditor {
		IWindowsFormsEditorService editorService;
		public AxisModelUITypeEditor() {
		}
		void OnListBoxClick(object sender, EventArgs e) {
			this.editorService.CloseDropDown();
		}
		bool NeedAppendAxis(Axis2DModel axis, XYDiagramPaneBaseModel paneModel) {
			if(paneModel == null)
				return true;
			XYDiagramPaneBase pane = paneModel.Pane;
			return axis.VisibilityInPanes.Contains(pane);
		}
		List<Axis2DModel> GetAllAxes(XYDiagramModel xyDiagram, XYDiagramPaneBaseModel paneModel) {
			List<Axis2DModel> result = new List<Axis2DModel>();
			AxisModel primaryAxis = GetPrimaryAxis(xyDiagram);
			if(NeedAppendAxis(primaryAxis, paneModel))
				result.Add(primaryAxis);
			foreach(Axis2DModel secondaryAxisX in GetSecondaryAxes(xyDiagram))
				if(NeedAppendAxis(secondaryAxisX, paneModel))
					result.Add(secondaryAxisX);
			return result;
		}
		protected abstract AxisModel GetPrimaryAxis(XYDiagramModel xyDiagram);
		protected abstract SecondaryAxisCollectionModel GetSecondaryAxes(XYDiagramModel xyDiagram);
		public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context) {
			return UITypeEditorEditStyle.DropDown;
		}
		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value) {
			this.editorService = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
			AxisModel axis = (AxisModel)value;
			DesignerChartModel chart = axis.FindParent<DesignerChartModel>();
			if(chart == null) {
				ChartDebug.Fail("ChartModel not found.");
				return value;
			}
			XYDiagramModel xyDiagram = chart.Diagram as XYDiagramModel;
			if(xyDiagram == null) {
				ChartDebug.Fail("XYDiagramModel not found.");
				return value;
			}
			PaneAnchorPointModel anchorPoint = axis.FindParent<PaneAnchorPointModel>();
			List<Axis2DModel> axes = GetAllAxes(xyDiagram, anchorPoint != null ? anchorPoint.Pane : null);
			ListBoxControl listBox = new ListBoxControl();
			listBox.Click += OnListBoxClick;
			foreach(Axis2DModel axisModel in axes) {
				int index = listBox.Items.Add(axisModel);
				if(axisModel.Equals(value)) {
					listBox.SelectedIndex = index + 1;
				}
			}
			this.editorService.DropDownControl(listBox);
			return listBox.SelectedItem;
		}
	}
	public class AxisXModelUITypeEditor : AxisModelUITypeEditor {
		protected override AxisModel GetPrimaryAxis(XYDiagramModel xyDiagram) {
			return xyDiagram.AxisX;
		}
		protected override SecondaryAxisCollectionModel GetSecondaryAxes(XYDiagramModel xyDiagram) {
			return xyDiagram.SecondaryAxesX;
		}
	}
	public class AxisYModelUITypeEditor : AxisModelUITypeEditor {
		protected override AxisModel GetPrimaryAxis(XYDiagramModel xyDiagram) {
			return xyDiagram.AxisY;
		}
		protected override SecondaryAxisCollectionModel GetSecondaryAxes(XYDiagramModel xyDiagram) {
			return xyDiagram.SecondaryAxesY;
		}
	}
	public class SwiftPlotAxisXModelUITypeEditor : UITypeEditor {
		IWindowsFormsEditorService editorService;
		void OnListBoxSelectedValueChanged(object sender, EventArgs e) {
			this.editorService.CloseDropDown();
		}
		List<SwiftPlotDiagramAxisXBaseModel> GetAllAxesX(SwiftPlotDiagramAxisXBaseModel axisXModel, SwiftPlotDiagramModel xyDiagram) {
			var result = new List<SwiftPlotDiagramAxisXBaseModel>();
			result.Add(xyDiagram.AxisX);
			foreach (SwiftPlotDiagramAxisXBaseModel secondaryAxisX in xyDiagram.SecondaryAxesX)
				result.Add(secondaryAxisX);
			return result;
		}
		public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context) {
			return UITypeEditorEditStyle.DropDown;
		}
		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value) {
			this.editorService = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
			var axisX = value as SwiftPlotDiagramAxisXBaseModel;
			var xyDiagram = axisX.FindParent<SwiftPlotDiagramModel>();
			if (xyDiagram == null) {
				ChartDebug.Fail("XYDiagramModel not found.");
				return value;
			}
			IEnumerable<SwiftPlotDiagramAxisXBaseModel> axesX = GetAllAxesX(axisX, xyDiagram);
			var listBox = new ListBoxControl();
			listBox.SelectedValueChanged += OnListBoxSelectedValueChanged;
			foreach (SwiftPlotDiagramAxisXBaseModel axisXModel in axesX) {
				int index = listBox.Items.Add(axisXModel);
				if (axisXModel.Equals(value)) {
					listBox.SelectedIndex = index + 1;
				}
			}
			this.editorService.DropDownControl(listBox);
			return listBox.SelectedItem;
		}
	}
	public class SwiftPlotAxisYModelUITypeEditor : UITypeEditor {
		IWindowsFormsEditorService editorService;
		void OnListBoxSelectedValueChanged(object sender, EventArgs e) {
			this.editorService.CloseDropDown();
		}
		List<SwiftPlotDiagramAxisYBaseModel> GetAllAxesY(SwiftPlotDiagramAxisYBaseModel axisXModel, SwiftPlotDiagramModel xyDiagram) {
			var result = new List<SwiftPlotDiagramAxisYBaseModel>();
			result.Add(xyDiagram.AxisY);
			foreach (SwiftPlotDiagramAxisYBaseModel secondaryAxisY in xyDiagram.SecondaryAxesY)
				result.Add(secondaryAxisY);
			return result;
		}
		public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context) {
			return UITypeEditorEditStyle.DropDown;
		}
		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value) {
			this.editorService = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
			var axisY = value as SwiftPlotDiagramAxisYBaseModel;
			var xyDiagram = axisY.FindParent<SwiftPlotDiagramModel>();
			if (xyDiagram == null) {
				ChartDebug.Fail("XYDiagramModel not found.");
				return value;
			}
			IEnumerable<SwiftPlotDiagramAxisYBaseModel> axesY = GetAllAxesY(axisY, xyDiagram);
			var listBox = new ListBoxControl();
			listBox.SelectedValueChanged += OnListBoxSelectedValueChanged;
			foreach (SwiftPlotDiagramAxisYBaseModel axisYModel in axesY) {
				int index = listBox.Items.Add(axisYModel);
				if (axisYModel.Equals(value)) {
					listBox.SelectedIndex = index + 1;
				}
			}
			this.editorService.DropDownControl(listBox);
			return listBox.SelectedItem;
		}
	}
	public class AxisVisibilityInPanesModelEditor : ChartEditorBase {
		protected override bool ShouldCreateTransaction { get { return false; } }
		protected override Form CreateForm() {
			DesignerChartElementModelBase model = (DesignerChartElementModelBase)Instance;
			Axis2D axis = model.ChartElement as Axis2D;
			if (axis == null)
				return null;
			Chart chart = ((IOwnedElement)axis).ChartContainer.Chart;
			AxisVisibilityInPanesModelForm form = new AxisVisibilityInPanesModelForm(axis, chart, model.CommandManager);
			form.LookAndFeel.ParentLookAndFeel = (UserLookAndFeel)chart.Container.RenderProvider.LookAndFeel;
			return form;
		}
	}
	public class AxisVisibilityInPanesModelForm : AxisVisibilityInPanesForm {
		readonly SetVisibilityInPanesCommand command;
		public AxisVisibilityInPanesModelForm(Axis2D axis, Chart chart, CommandManager commandManager)
			: base(axis.VisibilityInPanes, chart) {
			this.command = new SetVisibilityInPanesCommand(commandManager, axis);
		}
		protected override void SetVisibilityInPane(XYDiagramPaneBase pane, bool visible) {
			command.Execute(new SetVisibilityInPanesParameter(pane, visible));
		}
	}
}
