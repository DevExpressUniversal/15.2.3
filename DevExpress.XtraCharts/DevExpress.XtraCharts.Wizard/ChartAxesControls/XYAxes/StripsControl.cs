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

using System.Windows.Forms;
using DevExpress.XtraCharts.Native;
namespace DevExpress.XtraCharts.Wizard.ChartAxesControls {
	internal partial class StripsControl : ValidateControl {
		Axis2D axis;
		public StripsControl() {
			InitializeComponent();
			this.stripsListControl.SelectedElementChanged += new SelectedElementChangedEventHandler(stripsListControl_SelectedElementChanged);
			this.stripsListControl.AddStrip += new NewStripAddedEventHandler(stripsListControl_AddStrip);
			this.stripsRedactControl.UpdateByContent += new MethodInvoker(this.stripsListControl.UpdateList);
		}
		void stripsListControl_AddStrip(object sender, AddStripEventArgs args) {
			Strip strip = args.Strip;
			if (strip != null && axis != null) {
				if (StripLimitsUtils.CheckLimits(axis, axis.WholeRange.MinValue, axis.WholeRange.MaxValue)) {
					strip.MaxLimit.AxisValue = axis.WholeRange.MaxValue;
					strip.MinLimit.AxisValue = axis.WholeRange.MinValue;
				}
			}
		}
		void stripsListControl_SelectedElementChanged() {
			Strip strip = (Strip)this.stripsListControl.CurrentElement;
			this.tbcStripSettings.Enabled = strip != null;
			if (strip != null) {
				this.stripsRedactControl.Initialize(strip);
				this.stripsGeneralControl.Initialize(strip, axis);
				this.stripAppearanceControl.Initialize(strip);
			}
		}
		public void Initialize(Axis2D axis) {
			this.axis = axis;
			this.stripsListControl.Initialize(axis.Strips);
		}
	}
}
