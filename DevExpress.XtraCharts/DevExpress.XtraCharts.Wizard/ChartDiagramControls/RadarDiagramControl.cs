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
using System.Drawing;
using System.Data;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Forms;
using DevExpress.LookAndFeel;
using DevExpress.XtraEditors;
namespace DevExpress.XtraCharts.Wizard.ChartDiagramControls {
	internal partial class RadarDiagramControl : DiagramControlBase {
		RadarDiagram RadarDiagram { get { return (RadarDiagram)base.Diagram; } }
		public RadarDiagramControl() {
			InitializeComponent();
		}
		protected override void Initialize(UserLookAndFeel lookAndFeel) {
			base.Initialize(lookAndFeel);
			ceBorderColor.EditValue = RadarDiagram.BorderColor;
			chVisible.Checked = RadarDiagram.BorderVisible;
			drawingStyle.Initialize(RadarDiagram);
			marginsControl.Initialize(RadarDiagram.Margins);
			shadowControl.Initialize(RadarDiagram.Shadow);
			backgroundControl.Initialize(RadarDiagram, OriginalChart);
			UpdateControls();
		}
		protected override void InitializeTags() {
			tbGeneral.Tag = DiagramPageTab.RadarDiagramGeneral;
			tbAppearance.Tag = DiagramPageTab.RadarDiagramAppearance;
			tbBorder.Tag = DiagramPageTab.RadarDiagramBorder;
			tbShadow.Tag = DiagramPageTab.RadarDiagramShadow;
		}
		void UpdateControls() {
			ceBorderColor.Enabled = RadarDiagram.BorderVisible;
			lblColor.Enabled = RadarDiagram.BorderVisible;
		}
		void chVisible_CheckedChanged(object sender, EventArgs e) {
			RadarDiagram.BorderVisible = chVisible.Checked;
			UpdateControls();
		}
		void ceBorderColor_EditValueChanged(object sender, EventArgs e) {
			RadarDiagram.BorderColor = (Color)ceBorderColor.EditValue;
		}
	}
}
