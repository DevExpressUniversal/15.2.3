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
namespace DevExpress.XtraCharts.Designer.Native {
	public partial class SeparatePaneIndicatorControl : ElementsOptionsControlBase {
		public SeparatePaneIndicatorControl() {
			InitializeComponent();
		}
		protected virtual PaneControl PaneControl { get { throw new NotImplementedException("This property must be overriden in a derived class"); } }
		protected virtual AxisControl AxisYControl { get { throw new NotImplementedException("This property must be overriden in a derived class"); } }
		void UpdatePaneControl(DesignerChartElementModelBase model) {
			DesignerChartModel chart = model.FindParent<DesignerChartModel>();
			if (chart == null)
				return;
			XYDiagram2DModel diagramModel = chart.Diagram as XYDiagram2DModel;
			if (diagramModel == null)
				return;
			if (PaneControl == null)
				return;
			PaneControl.DiagramModel = diagramModel;
			PaneControl.UpdateComboBox();
		}
		void UpdateAxisControl(DesignerChartElementModelBase model) {
			DesignerChartModel chart = model.FindParent<DesignerChartModel>();
			if (chart == null)
				return;
			XYDiagram2DModel diagramModel = chart.Diagram as XYDiagram2DModel;
			if (diagramModel == null)
				return;
			if (AxisYControl == null)
				return;
			AxisYControl.DiagramModel = diagramModel;
			AxisYControl.UpdateComboBox();
		}
		internal override void LoadModel(DesignerChartElementModelBase model) {
			base.LoadModel(model);
			UpdatePaneControl(model);
			UpdateAxisControl(model);
		}
	}
}
