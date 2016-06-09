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
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraCharts.Native;
namespace DevExpress.XtraCharts.Wizard.ChartDiagramControls {
	internal partial class ScrollBarOptionsControl : ChartUserControl {
		ScrollBarOptions scrollBarOptions;
		public ScrollBarOptionsControl() {
			InitializeComponent();
		}
		public void Initialize(XYDiagramPaneBase pane) {
			this.scrollBarOptions = pane.ScrollBarOptions;
			axisXScrollBarOptionsControl.Initialize(new AxisXScrollBarOptionsProperties(scrollBarOptions));
			axisYScrollBarOptionsControl.Initialize(new AxisYScrollBarOptionsProperties(scrollBarOptions));
			clreBarColor.EditValue = scrollBarOptions.BarColor;
			spnThichness.EditValue = scrollBarOptions.BarThickness;
			clreBackColor.EditValue = scrollBarOptions.BackColor;
			clreBorderColor.EditValue = scrollBarOptions.BorderColor;
		}
		void clreBarColor_EditValueChanged(object sender, EventArgs e) {
			scrollBarOptions.BarColor = (Color)clreBarColor.EditValue;
		}
		void spnThichness_EditValueChanged(object sender, EventArgs e) {
			scrollBarOptions.BarThickness = Convert.ToInt32(spnThichness.EditValue);
		}
		void clreBackColor_EditValueChanged(object sender, EventArgs e) {
			scrollBarOptions.BackColor = (Color)clreBackColor.EditValue;
		}
		void clreBorderColor_EditValueChanged(object sender, EventArgs e) {
			scrollBarOptions.BorderColor = (Color)clreBorderColor.EditValue;
		}
	}
}
