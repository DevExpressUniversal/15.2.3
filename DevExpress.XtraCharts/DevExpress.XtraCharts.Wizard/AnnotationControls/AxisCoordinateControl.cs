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
using System.Globalization;
using System.Collections.Generic;
using DevExpress.Charts.Native;
namespace DevExpress.XtraCharts.Wizard.AnnotationControls {
	internal partial class AxisCoordinateControl : ChartUserControl {
		XYDiagram2D diagram;
		AxisXCoordinate xCoord;
		AxisYCoordinate yCoord;
		bool inUpdate;
		bool IsXCoord { get { return xCoord != null; } }
		List<Axis2D> TotalAxes { 
			get {			   
				if (IsXCoord) {
					return this.diagram.GetAllAxesX();
				}
				else {
					return this.diagram.GetAllAxesY();
				}
				throw new Exception("Incorrect axis type.");
			}
		}
		object AxisValue {
			get { return IsXCoord ? xCoord.AxisValue : yCoord.AxisValue; }
			set {
				if (IsXCoord)
					xCoord.AxisValue = value;
				else
					yCoord.AxisValue = value;
			}
		}
		public AxisCoordinateControl() {
			InitializeComponent();
		}
		Axis2D GetCurrentAxis() {
			return IsXCoord ? xCoord.Axis : yCoord.Axis;
		}
		void SetCurrentAxis(string name) {
			if (IsXCoord)
				xCoord.Axis = diagram.FindAxisXByName(name);
			else
				yCoord.Axis = diagram.FindAxisYByName(name);
		}
		public void InitializeByXCoord(AxisXCoordinate xCoord, XYDiagram2D diagram) {
			this.xCoord = xCoord;
			yCoord = null;
			Initialize(diagram);
		}
		public void InitializeByYCoord(AxisYCoordinate yCoord, XYDiagram2D diagram) {
			this.yCoord = yCoord;
			xCoord = null;
			Initialize(diagram);
		}
		void Initialize(XYDiagram2D diagram) {
			this.diagram = diagram;
			inUpdate = true;
			try {
				cbAxis.Properties.Items.Clear();
				foreach (Axis2D axis in TotalAxes)
					cbAxis.Properties.Items.Add(axis.Name);
				cbAxis.SelectedItem = GetCurrentAxis().Name;
				txtAxisValue.EditValue = AxisValue;
			}
			finally {
				inUpdate = false;
			}
		}
		void cbAxis_SelectedIndexChanged(object sender, EventArgs e) {
			if (!inUpdate)
				SetCurrentAxis((string)cbAxis.SelectedItem);
		}
		void txtAxisValue_Validating(object sender, CancelEventArgs e) {
			if (!inUpdate) {
				IAxisData axis = GetCurrentAxis();
				object nativeValue = axis.AxisScaleTypeMap.ConvertValue(txtAxisValue.EditValue, CultureInfo.CurrentCulture);
				if (axis.AxisScaleTypeMap.IsCompatible(nativeValue))
					AxisValue = nativeValue;
				else
					e.Cancel = true;
			}
		}
		void txtAxisValue_Validated(object sender, EventArgs e) {
			if (!inUpdate)
				AxisValue = txtAxisValue.EditValue;
		}
	}
}
