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
using DevExpress.XtraCharts.Wizard;
namespace DevExpress.XtraCharts.Wizard.ChartDiagramControls {
	internal partial class ScrollingOptionsControl : ChartUserControl {
		ScrollingOptions scrollingOptions;
		ScrollingOptions2D scrollingOptions2D;
		public ScrollingOptionsControl() {
			InitializeComponent();
		}
		void chbUseKeyboard_CheckedChanged(object sender, EventArgs e) {
			scrollingOptions.UseKeyboard = chbUseKeyboard.Checked;
		}
		void chbUseMouse_CheckedChanged(object sender, EventArgs e) {
			scrollingOptions.UseMouse = chbUseMouse.Checked;
		}
		void chbUseTouchDevice_CheckedChanged(object sender, EventArgs e) {
			if (scrollingOptions2D != null)
				scrollingOptions2D.UseTouchDevice = chbUseTouchDevice.Checked;
		}
		void chbUseScrollbars_CheckedChanged(object sender, EventArgs e) {
			if (scrollingOptions2D != null)
				scrollingOptions2D.UseScrollBars = chbUseScrollbars.Checked;
		}
		public void Initialize(ScrollingOptions scrollingOptions) {
			this.scrollingOptions = scrollingOptions;
			chbUseKeyboard.Checked = scrollingOptions.UseKeyboard;
			chbUseMouse.Checked = scrollingOptions.UseMouse;			
			scrollingOptions2D = scrollingOptions as ScrollingOptions2D;
			if (scrollingOptions2D != null) {
				chbUseScrollbars.Checked = scrollingOptions2D.UseScrollBars;
				chbUseTouchDevice.Checked = scrollingOptions2D.UseTouchDevice;
			}
			else {
				separator2.Visible = false;
				separator3.Visible = false;
				chbUseScrollbars.Visible = false;
				chbUseTouchDevice.Visible = false;
			}			
		}
	}
}
