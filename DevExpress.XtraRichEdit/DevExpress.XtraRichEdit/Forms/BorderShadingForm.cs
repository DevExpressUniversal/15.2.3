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
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DevExpress.Office.Utils;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraRichEdit.API.Native;
using DevExpress.XtraRichEdit.Forms;
using DevExpress.XtraRichEdit.Forms.Design;
using DevExpress.XtraRichEdit.Localization;
using DevExpress.XtraRichEdit.Model;
namespace DevExpress.XtraRichEdit.Forms {
	public partial class BorderShadingForm : XtraForm {
		BorderShadingFormController controller;
		static Image borderNoneImage;
		static Image borderBoxImage;
		static Image borderAllImage;
		static Image borderGridImage;
		static Image borderCustomImage;
		private const string imageResourcePrefix = "DevExpress.XtraRichEdit.Images";
		private static readonly System.Reflection.Assembly imageResourceAssembly = typeof(IRichEditControl).Assembly;
		static BorderShadingForm() {
			borderNoneImage = CommandResourceImageLoader.LoadLargeImage(imageResourcePrefix, "BordersNone", imageResourceAssembly);
			borderAllImage = CommandResourceImageLoader.LoadLargeImage(imageResourcePrefix, "BorderAll", imageResourceAssembly);
			borderBoxImage = CommandResourceImageLoader.LoadLargeImage(imageResourcePrefix, "BordersBox", imageResourceAssembly);
			borderGridImage = CommandResourceImageLoader.LoadLargeImage(imageResourcePrefix, "BordersGrid", imageResourceAssembly);
			borderCustomImage = CommandResourceImageLoader.LoadLargeImage(imageResourcePrefix, "BordersCustom", imageResourceAssembly);
		} 
		public BorderShadingForm() {
			InitializeComponent();
			btnNone.Image = borderNoneImage;
			btnBox.Image = borderBoxImage;
			btnAll.Image = borderAllImage;
			btnGrid.Image = borderGridImage;
			btnCustom.Image = borderCustomImage;
		}
		public void Initialize(BorderShadingFormController controller) {
			this.controller = controller;
			InitializePreviewControl(borderUserControl);
			borderUserControl.BtnsVisible = true;
			colorEdit.Color = controller.GetActiveColor();
			InitializePreviewControl(shadingUserControl);
			shadingUserControl.BtnsVisible = false;
			borderUserControl.DocumentModel = controller.DocumentModel;
			shadingUserControl.DocumentModel = controller.DocumentModel;
			borderShadingTypeLineUserControl1.DocumentModel = controller.DocumentModel;
			borderShadingTypeLineUserControl1.RichEditControl = controller.RichEditControl;
			borderShadingTypeLineUserControl1.SetInitialValue(controller.GetInitialBorder());
			SetButtonState();
		}
		void InitializePreviewControl(BorderShadingUserControl control) {
			control.BeginSetProperties();
			control.BorderLineUp = controller.BorderLineUp;
			control.BorderLineDown = controller.BorderLineDown;
			control.BorderLineHorizontalIn = controller.BorderLineHorizontalIn;
			control.BorderLineLeft = controller.BorderLineLeft;
			control.BorderLineRight = controller.BorderLineRight;
			control.BorderLineVerticalIn = controller.BorderLineVerticalIn;
			control.BorderLineHorizontalInVisible = controller.BorderLineHorizontalInVisible;
			control.BorderLineVerticalInVisible = controller.BorderLineVerticalInVisible;
			control.FillColor = controller.GetActiveColor();
			control.DrawColumns = SetDrawColumns();
			if (!control.DrawColumns)
				control.DrawParagraph = SetDrawParagraph();
			control.EndSetProperties();
		}
		bool SetDrawColumns() {
			return controller.BorderLineVerticalInVisible;
		}
		bool SetDrawParagraph() {
			return controller.BorderLineHorizontalInVisible;
		}
		void SetButtonState() {
			if (controller.SetModeButton == BorderShadingFormController.SetModeButtons.All)
				btnAll.Checked = true;
			if (controller.SetModeButton == BorderShadingFormController.SetModeButtons.Box)
				btnBox.Checked = true;
			if (controller.SetModeButton == BorderShadingFormController.SetModeButtons.Grid)
				btnGrid.Checked = true;
			if (controller.SetModeButton == BorderShadingFormController.SetModeButtons.Custom)
				btnCustom.Checked = true;
			if (controller.SetModeButton == BorderShadingFormController.SetModeButtons.None)
				btnNone.Checked = true;
		}
		private void borderShadingUserControl1_Load(object sender, EventArgs e) {
		}
	   private void btnCancel_Click_1(object sender, EventArgs e) {
			Close();
		}
		private void btnOK_Click(object sender, EventArgs e) {
			controller.ApplyChanges();
			Close();
		}
		private void borderUserControl_BorderLineUpChanged(object sender, EventArgs e) {
			controller.BorderLineUp = borderUserControl.BorderLineUp;
		}
		private void borderUserControl_BorderLineDownChanged(object sender, EventArgs e) {
			controller.BorderLineDown = borderUserControl.BorderLineDown;
		}
		private void borderUserControl_BorderLineHorizontalInChanged(object sender, EventArgs e) {
			controller.BorderLineHorizontalIn = borderUserControl.BorderLineHorizontalIn;
		}
		private void borderUserControl_BorderLineLeftChanged(object sender, EventArgs e) {
			controller.BorderLineLeft = borderUserControl.BorderLineLeft;
		}
		private void borderUserControl_BorderLineRightChanged(object sender, EventArgs e) {
			controller.BorderLineRight = borderUserControl.BorderLineRight;
		}
		private void borderUserControl_BorderLineVerticalInChanged(object sender, EventArgs e) {
			controller.BorderLineVerticalIn = borderUserControl.BorderLineVerticalIn;
		}  
		private void btnNone_CheckedChanged(object sender, EventArgs e) {
			if (btnNone.Checked) {
				borderUserControl.SetAllMode();
				borderUserControl.SetNoneMode();
				ButtonChecked(btnNone);
			}
		}
		private void borderShadingTypeLineUserControl1_BorderChanged(object sender, EventArgs e) {
			borderUserControl.CurrentBorderInfo = borderShadingTypeLineUserControl1.Border.Clone();
		}
		private void btnBox_CheckedChanged(object sender, EventArgs e) {
			if (btnBox.Checked) {
				borderUserControl.SetAllMode();
				borderUserControl.SetBoxMode();
				ButtonChecked(btnBox);
			}
		}
		private void btnAll_CheckedChanged(object sender, EventArgs e) {
			if (btnAll.Checked) {
				borderUserControl.SetNoneMode();
				borderUserControl.SetAllMode();
				ButtonChecked(btnAll);
			}
		}
		private void btnGrid_CheckedChanged(object sender, EventArgs e) {
			if (btnGrid.Checked) {
				borderUserControl.SetNoneMode();
				borderUserControl.SetGridMode();
				ButtonChecked(btnGrid);
			}
		}
		private void btnCustom_CheckedChanged(object sender, EventArgs e) {
			if (btnCustom.Checked) {
				borderUserControl.SetCustomMode();
				ButtonChecked(btnCustom);
			}
		}
		void ButtonChecked(CheckButton checkButton) {
			btnNone.CheckedChanged -= btnNone_CheckedChanged;
			btnAll.CheckedChanged -= btnAll_CheckedChanged;
			btnGrid.CheckedChanged -= btnGrid_CheckedChanged;
			btnBox.CheckedChanged -= btnBox_CheckedChanged;
			btnCustom.CheckedChanged -= btnCustom_CheckedChanged;
			btnNone.Checked = false;
			btnAll.Checked = false;
			btnGrid.Checked = false;
			btnBox.Checked = false;
			btnCustom.Checked = false;
			checkButton.Checked = true;
			btnNone.CheckedChanged += btnNone_CheckedChanged;
			btnAll.CheckedChanged += btnAll_CheckedChanged;
			btnGrid.CheckedChanged += btnGrid_CheckedChanged;
			btnBox.CheckedChanged += btnBox_CheckedChanged;
			btnCustom.CheckedChanged += btnCustom_CheckedChanged;
		}
		private void borderUserControl_Click(object sender, EventArgs e) {
		}
		private void colorEdit_ColorChanged(object sender, EventArgs e) {
			controller.FillColor = colorEdit.Color;
			borderUserControl.FillColor = colorEdit.Color;
			shadingUserControl.FillColor = colorEdit.Color;
		}
	}
}
