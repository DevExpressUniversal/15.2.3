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

using DevExpress.Utils.Frames;
using DevExpress.XtraEditors;
using DevExpress.XtraCharts.Printing;
using DevExpress.XtraCharts.Native;
using DevExpress.XtraCharts.Localization;
using DevExpress.Charts.Native;
namespace DevExpress.XtraCharts.Design {
	public partial class ChartPrintingDesigner : DevExpress.XtraEditors.Designer.Utils.XtraFrame {
		PrintSizeMode sizeMode = PrintSizeMode.None;
		ChartOptionsPrint optionsPrint;
		public ChartPrintingDesigner() {
			InitializeComponent();
			InitComponents();
		}
		public void Initialize(ChartOptionsPrint optionsPrint) {
			this.optionsPrint = optionsPrint;
			this.sizeMode = optionsPrint.SizeMode;
			SetChecked(optionsPrint.SizeMode);
		}
		public void ApplyOptions() {
			if (optionsPrint != null)
				optionsPrint.SizeMode = sizeMode;
		}
		void SetChecked(PrintSizeMode mode) {
			switch (mode) {
				case PrintSizeMode.None:
					cheNone.Checked = true;
					break;
				case PrintSizeMode.Stretch:
					cheStretch.Checked = true;
					break;
				case PrintSizeMode.Zoom:
					cheZoom.Checked = true;
					break;
				default:
					throw new DefaultSwitchException();
			}
		}
		PrintSizeMode GetChecked(CheckEdit edit) {
			if (edit.Name == "cheNone")
				return PrintSizeMode.None;
			if (edit.Name == "cheStretch")
				return PrintSizeMode.Stretch;
			if (edit.Name == "cheZoom")
				return PrintSizeMode.Zoom;
			ChartDebug.Assert(false, "Wrong edit");
			return PrintSizeMode.None;
		}
		void InitComponents() {
			cheNone.Text = ChartLocalizer.GetString(ChartStringId.PrintSizeModeNone);
			cheStretch.Text = ChartLocalizer.GetString(ChartStringId.PrintSizeModeStretch);
			cheZoom.Text = ChartLocalizer.GetString(ChartStringId.PrintSizeModeZoom);
		}
		void Item_CheckedChanged(object sender, System.EventArgs e) {
			CheckEdit edit = sender as CheckEdit;
			if (edit == null)
				return;
			if (edit.Checked)
				sizeMode = GetChecked(edit);
		}
	}
}
