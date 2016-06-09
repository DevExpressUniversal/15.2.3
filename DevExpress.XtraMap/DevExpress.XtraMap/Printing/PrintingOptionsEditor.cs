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
using DevExpress.Charts.Native;
using DevExpress.XtraPrinting;
using DevExpress.XtraMap.Printing;
using DevExpress.XtraMap.Native;
using System.Diagnostics;
using System;
using DevExpress.Map.Localization;
namespace DevExpress.XtraMap.Printing {
	public partial class PrintingOptionsEditor : DevExpress.XtraEditors.Designer.Utils.XtraFrame {
		MapPrintSizeMode sizeMode = MapPrintSizeMode.Normal;
		PrintOptions printOptions;
		bool printMiniMap;
		bool printNavPanel;
		public PrintingOptionsEditor() {
			InitializeComponent();
			InitControls();
		}
		public void Initialize(PrintOptions printOptions) {
			this.printOptions = printOptions;
			this.sizeMode = printOptions.SizeMode;
			this.printMiniMap = printOptions.PrintMiniMap;
			this.printNavPanel = printOptions.PrintNavigationPanel;
			SetSizeModeChecked(printOptions.SizeMode);
			SetItemsChecked(printOptions);
		}
		void OnLocalizerChanged(object sender, EventArgs e) {
			InitControls();
		}
		public void ApplyOptions() {
			if (printOptions != null) {
				printOptions.SizeMode = sizeMode;
				printOptions.PrintMiniMap = printMiniMap;
				printOptions.PrintNavigationPanel = printNavPanel;
			}
		}
		protected void InitControls() {
			lcSizeModeTitle.Text = MapLocalizer.GetString(MapStringId.PrintSizeModeTitle);
			lcItemsTitle.Text = MapLocalizer.GetString(MapStringId.PrintItemsTitle); 
			cheNone.Text = MapLocalizer.GetString(MapStringId.PrintSizeModeNormal);
			cheStretch.Text = MapLocalizer.GetString(MapStringId.PrintSizeModeStretch);
			cheZoom.Text = MapLocalizer.GetString(MapStringId.PrintSizeModeZoom);
			cheMiniMap.Text = MapLocalizer.GetString(MapStringId.PrintMiniMap);
			cheNavPanel.Text = MapLocalizer.GetString(MapStringId.PrintNavPanel);
		}
		void SetSizeModeChecked(MapPrintSizeMode mode) {
			switch(mode) {
				case MapPrintSizeMode.Normal:
					cheNone.Checked = true;
					break;
				case MapPrintSizeMode.Stretch:
					cheStretch.Checked = true;
					break;
				case MapPrintSizeMode.Zoom:
					cheZoom.Checked = true;
					break;
				default:
					throw new Exception();
			}
		}
		void SetItemsChecked(PrintOptions printOptions) {
			cheNavPanel.Checked = printOptions.PrintNavigationPanel;
			cheMiniMap.Checked = printOptions.PrintMiniMap;
		}
		MapPrintSizeMode GetSizeModeChecked(CheckEdit edit) {
			if(edit.Name == "cheNone")
				return MapPrintSizeMode.Normal;
			if(edit.Name == "cheStretch")
				return MapPrintSizeMode.Stretch;
			if(edit.Name == "cheZoom")
				return MapPrintSizeMode.Zoom;
			MapUtils.DebugAssert(false);
			return MapPrintSizeMode.Normal;
		}
		void SizeModeCheckedChanged(object sender, EventArgs e) {
			CheckEdit edit = sender as CheckEdit;
			if(edit == null)
				return;
			if(edit.Checked)
				sizeMode = GetSizeModeChecked(edit);
		}
		void ItemsCheckedChanged(object sender, EventArgs e) {
			CheckEdit edit = sender as CheckEdit;
			if (edit == null)
				return;
			if (edit.Name == "cheNavPanel")
				printNavPanel = edit.Checked;
			if (edit.Name == "cheMiniMap")
				printMiniMap = edit.Checked;
		}
	}
}
