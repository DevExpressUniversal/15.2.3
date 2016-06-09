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
using DevExpress.XtraEditors;
namespace DevExpress.XtraCharts.Wizard.ChartDiagramControls {
	internal partial class RadarDiagramDrawingStyleControl : ChartUserControl {
		RadarDiagram diagram;
		public RadarDiagramDrawingStyleControl() {
			InitializeComponent();
		}
		public void Initialize(RadarDiagram diagram) {
			this.diagram = diagram;
			this.chRotate.Checked = diagram.RotationDirection == RadarDiagramRotationDirection.Clockwise;
			this.cbDrawingStyle.SelectedIndex = (int)diagram.DrawingStyle;
			this.txtAngle.EditValue = diagram.StartAngleInDegrees;
		}
		private void chRotate_CheckedChanged(object sender, EventArgs e) {
			this.diagram.RotationDirection = this.chRotate.Checked ?
				RadarDiagramRotationDirection.Clockwise : RadarDiagramRotationDirection.Counterclockwise;
		}
		private void txtAngle_EditValueChanged(object sender, EventArgs e) {
			this.diagram.StartAngleInDegrees = Convert.ToInt32(this.txtAngle.EditValue);
		}
		private void cbDrawingStyle_SelectedIndexChanged(object sender, EventArgs e) {
			this.diagram.DrawingStyle = (RadarDiagramDrawingStyle)cbDrawingStyle.SelectedIndex;
		}
	}
}
