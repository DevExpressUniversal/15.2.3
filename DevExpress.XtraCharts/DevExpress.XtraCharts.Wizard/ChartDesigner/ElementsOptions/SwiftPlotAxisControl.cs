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
namespace DevExpress.XtraCharts.Designer.Native {
	public partial class SwiftPlotAxisControl : ModelSelectControl {
		bool isArgumentAxis;
		public SwiftPlotDiagramAxisModel Axis {
			get {
				return Element as SwiftPlotDiagramAxisModel;
			}
			set {
				Element = value as SwiftPlotDiagramAxisModel;
			}
		}
		public bool IsArgumentAxis {
			get { return isArgumentAxis; }
			set { isArgumentAxis = value; }
		}
		SwiftPlotDiagramAxisModel DefaultAxis {
			get {
				return isArgumentAxis ? SwiftPlotDiagramModel.AxisX as SwiftPlotDiagramAxisModel : SwiftPlotDiagramModel.AxisY as SwiftPlotDiagramAxisModel;
			}
		}
		SwiftPlotDiagramModel SwiftPlotDiagramModel {
			get { return DiagramModel as SwiftPlotDiagramModel; }
		}
		public SwiftPlotAxisControl()
			: base() {
			InitializeComponent();
		}
		public override void UpdateComboBox() {
			if (SwiftPlotDiagramModel == null)
				return;
			BoxEdit.Properties.Items.Clear();
			ModelSelectControl.NamedModelPresentation currentPresentation = null;
			ModelSelectControl.NamedModelPresentation presentation = null;
			currentPresentation = AddAxis(DefaultAxis);
			if (presentation != null)
				currentPresentation = presentation;
			if (IsArgumentAxis) {
				foreach (SwiftPlotDiagramAxisModel axis in SwiftPlotDiagramModel.SecondaryAxesX) {
					presentation = AddAxis(axis);
					if (presentation != null)
						currentPresentation = presentation;
				}
			}
			else {
				foreach (SwiftPlotDiagramAxisModel axis in SwiftPlotDiagramModel.SecondaryAxesY) {
					presentation = AddAxis(axis);
					if (presentation != null)
						currentPresentation = presentation;
				}
			}
			if (currentPresentation != null)
				BoxEdit.SelectedItem = currentPresentation;
		}
		ModelSelectControl.NamedModelPresentation AddAxis(SwiftPlotDiagramAxisModel axis) {
			NamedModelPresentation presentation = new NamedModelPresentation(axis.Name, axis);
			BoxEdit.Properties.Items.Add(presentation);
			if (axis.Equals(Element))
				return presentation;
			return null;
		}
		#region ShouldSerialize
		bool ShouldSerializeIsArgumentAxis() {
			return true;
		}
		#endregion
	}
}
