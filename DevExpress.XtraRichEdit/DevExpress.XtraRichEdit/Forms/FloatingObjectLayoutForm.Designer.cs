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

namespace DevExpress.XtraRichEdit.Forms {
	partial class FloatingObjectLayoutForm {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Windows Form Designer generated code
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FloatingObjectLayoutForm));
			this.tabControl = new DevExpress.XtraTab.XtraTabControl();
			this.tabPagePosition = new DevExpress.XtraTab.XtraTabPage();
			this.lblVerticalAbsolutePosition = new DevExpress.XtraEditors.LabelControl();
			this.lblHorizontalAbsolutePosition = new DevExpress.XtraEditors.LabelControl();
			this.lblVerticalPositionType = new DevExpress.XtraEditors.LabelControl();
			this.lblHorizontalPositionType = new DevExpress.XtraEditors.LabelControl();
			this.spnVerticalAbsolutePosition = new DevExpress.XtraRichEdit.Design.RichTextIndentEdit();
			this.spnHorizontalAbsolutePosition = new DevExpress.XtraRichEdit.Design.RichTextIndentEdit();
			this.chkLock = new DevExpress.XtraEditors.CheckEdit();
			this.cbVerticalAbsolutePositionBelow = new DevExpress.XtraEditors.ImageComboBoxEdit();
			this.cbVerticalPositionType = new DevExpress.XtraEditors.ImageComboBoxEdit();
			this.cbVerticalAlignment = new DevExpress.XtraEditors.ImageComboBoxEdit();
			this.cbHorizontalAbsolutePositionRightOf = new DevExpress.XtraEditors.ImageComboBoxEdit();
			this.cbHorizontalPositionType = new DevExpress.XtraEditors.ImageComboBoxEdit();
			this.cbHorizontalAlignment = new DevExpress.XtraEditors.ImageComboBoxEdit();
			this.rgVertical = new DevExpress.XtraEditors.RadioGroup();
			this.rgHorizontal = new DevExpress.XtraEditors.RadioGroup();
			this.lblOptions = new DevExpress.XtraEditors.LabelControl();
			this.lblVertical = new DevExpress.XtraEditors.LabelControl();
			this.lblHorizontal = new DevExpress.XtraEditors.LabelControl();
			this.tabPageTextWrapping = new DevExpress.XtraTab.XtraTabPage();
			this.spnRight = new DevExpress.XtraRichEdit.Design.RichTextIndentEdit();
			this.spnLeft = new DevExpress.XtraRichEdit.Design.RichTextIndentEdit();
			this.spnBottom = new DevExpress.XtraRichEdit.Design.RichTextIndentEdit();
			this.spnTop = new DevExpress.XtraRichEdit.Design.RichTextIndentEdit();
			this.lblRight = new DevExpress.XtraEditors.LabelControl();
			this.lblLeft = new DevExpress.XtraEditors.LabelControl();
			this.lblBottom = new DevExpress.XtraEditors.LabelControl();
			this.lblTop = new DevExpress.XtraEditors.LabelControl();
			this.rgTextWrapSide = new DevExpress.XtraEditors.RadioGroup();
			this.lblDistance = new DevExpress.XtraEditors.LabelControl();
			this.WrapText = new DevExpress.XtraEditors.LabelControl();
			this.columnsPresetControlBehind = new DevExpress.XtraRichEdit.Forms.Design.ColumnsPresetControl();
			this.columnsPresetControlInFrontOf = new DevExpress.XtraRichEdit.Forms.Design.ColumnsPresetControl();
			this.columnsPresetControlThought = new DevExpress.XtraRichEdit.Forms.Design.ColumnsPresetControl();
			this.columnsPresetControlTopAndBottom = new DevExpress.XtraRichEdit.Forms.Design.ColumnsPresetControl();
			this.columnsPresetControlTight = new DevExpress.XtraRichEdit.Forms.Design.ColumnsPresetControl();
			this.columnsPresetControlSquare = new DevExpress.XtraRichEdit.Forms.Design.ColumnsPresetControl();
			this.lblWrappingStyle = new DevExpress.XtraEditors.LabelControl();
			this.tabPageSize = new DevExpress.XtraTab.XtraTabPage();
			this.btnReset = new DevExpress.XtraEditors.SimpleButton();
			this.lblOriginalSizeWidthValue = new DevExpress.XtraEditors.LabelControl();
			this.lblOriginalSizeHeightValue = new DevExpress.XtraEditors.LabelControl();
			this.lblWidthAbsolute = new DevExpress.XtraEditors.LabelControl();
			this.lblHeightAbsolute = new DevExpress.XtraEditors.LabelControl();
			this.lblOriginalSizeWidth = new DevExpress.XtraEditors.LabelControl();
			this.lblOriginalSizeHeight = new DevExpress.XtraEditors.LabelControl();
			this.lblOriginalSize = new DevExpress.XtraEditors.LabelControl();
			this.spnWidthAbs = new DevExpress.XtraRichEdit.Design.RichTextIndentEdit();
			this.lblWidth = new DevExpress.XtraEditors.LabelControl();
			this.spnHeightAbs = new DevExpress.XtraRichEdit.Design.RichTextIndentEdit();
			this.lblHeight = new DevExpress.XtraEditors.LabelControl();
			this.lblRotation = new DevExpress.XtraEditors.LabelControl();
			this.lblRotate = new DevExpress.XtraEditors.LabelControl();
			this.spnRotation = new DevExpress.XtraEditors.SpinEdit();
			this.chkLockAspectRatio = new DevExpress.XtraEditors.CheckEdit();
			this.lblScale = new DevExpress.XtraEditors.LabelControl();
			this.btnOk = new DevExpress.XtraEditors.SimpleButton();
			this.btnCancel = new DevExpress.XtraEditors.SimpleButton();
			((System.ComponentModel.ISupportInitialize)(this.tabControl)).BeginInit();
			this.tabControl.SuspendLayout();
			this.tabPagePosition.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.spnVerticalAbsolutePosition.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.spnHorizontalAbsolutePosition.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.chkLock.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.cbVerticalAbsolutePositionBelow.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.cbVerticalPositionType.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.cbVerticalAlignment.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.cbHorizontalAbsolutePositionRightOf.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.cbHorizontalPositionType.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.cbHorizontalAlignment.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.rgVertical.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.rgHorizontal.Properties)).BeginInit();
			this.tabPageTextWrapping.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.spnRight.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.spnLeft.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.spnBottom.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.spnTop.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.rgTextWrapSide.Properties)).BeginInit();
			this.tabPageSize.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.spnWidthAbs.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.spnHeightAbs.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.spnRotation.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.chkLockAspectRatio.Properties)).BeginInit();
			this.SuspendLayout();
			resources.ApplyResources(this.tabControl, "tabControl");
			this.tabControl.Name = "tabControl";
			this.tabControl.SelectedTabPage = this.tabPagePosition;
			this.tabControl.TabPages.AddRange(new DevExpress.XtraTab.XtraTabPage[] {
			this.tabPagePosition,
			this.tabPageTextWrapping,
			this.tabPageSize});
			this.tabPagePosition.Controls.Add(this.lblVerticalAbsolutePosition);
			this.tabPagePosition.Controls.Add(this.lblHorizontalAbsolutePosition);
			this.tabPagePosition.Controls.Add(this.lblVerticalPositionType);
			this.tabPagePosition.Controls.Add(this.lblHorizontalPositionType);
			this.tabPagePosition.Controls.Add(this.spnVerticalAbsolutePosition);
			this.tabPagePosition.Controls.Add(this.spnHorizontalAbsolutePosition);
			this.tabPagePosition.Controls.Add(this.chkLock);
			this.tabPagePosition.Controls.Add(this.cbVerticalAbsolutePositionBelow);
			this.tabPagePosition.Controls.Add(this.cbVerticalPositionType);
			this.tabPagePosition.Controls.Add(this.cbVerticalAlignment);
			this.tabPagePosition.Controls.Add(this.cbHorizontalAbsolutePositionRightOf);
			this.tabPagePosition.Controls.Add(this.cbHorizontalPositionType);
			this.tabPagePosition.Controls.Add(this.cbHorizontalAlignment);
			this.tabPagePosition.Controls.Add(this.rgVertical);
			this.tabPagePosition.Controls.Add(this.rgHorizontal);
			this.tabPagePosition.Controls.Add(this.lblOptions);
			this.tabPagePosition.Controls.Add(this.lblVertical);
			this.tabPagePosition.Controls.Add(this.lblHorizontal);
			this.tabPagePosition.Name = "tabPagePosition";
			resources.ApplyResources(this.tabPagePosition, "tabPagePosition");
			resources.ApplyResources(this.lblVerticalAbsolutePosition, "lblVerticalAbsolutePosition");
			this.lblVerticalAbsolutePosition.Name = "lblVerticalAbsolutePosition";
			resources.ApplyResources(this.lblHorizontalAbsolutePosition, "lblHorizontalAbsolutePosition");
			this.lblHorizontalAbsolutePosition.Name = "lblHorizontalAbsolutePosition";
			resources.ApplyResources(this.lblVerticalPositionType, "lblVerticalPositionType");
			this.lblVerticalPositionType.Name = "lblVerticalPositionType";
			resources.ApplyResources(this.lblHorizontalPositionType, "lblHorizontalPositionType");
			this.lblHorizontalPositionType.Name = "lblHorizontalPositionType";
			resources.ApplyResources(this.spnVerticalAbsolutePosition, "spnVerticalAbsolutePosition");
			this.spnVerticalAbsolutePosition.Name = "spnVerticalAbsolutePosition";
			this.spnVerticalAbsolutePosition.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton()});
			this.spnVerticalAbsolutePosition.Properties.IsValueInPercent = false;
			resources.ApplyResources(this.spnHorizontalAbsolutePosition, "spnHorizontalAbsolutePosition");
			this.spnHorizontalAbsolutePosition.Name = "spnHorizontalAbsolutePosition";
			this.spnHorizontalAbsolutePosition.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton()});
			this.spnHorizontalAbsolutePosition.Properties.IsValueInPercent = false;
			resources.ApplyResources(this.chkLock, "chkLock");
			this.chkLock.Name = "chkLock";
			this.chkLock.Properties.AutoWidth = true;
			this.chkLock.Properties.Caption = resources.GetString("chkLock.Properties.Caption");
			resources.ApplyResources(this.cbVerticalAbsolutePositionBelow, "cbVerticalAbsolutePositionBelow");
			this.cbVerticalAbsolutePositionBelow.Name = "cbVerticalAbsolutePositionBelow";
			this.cbVerticalAbsolutePositionBelow.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("cbVerticalAbsolutePositionBelow.Properties.Buttons"))))});
			resources.ApplyResources(this.cbVerticalPositionType, "cbVerticalPositionType");
			this.cbVerticalPositionType.Name = "cbVerticalPositionType";
			this.cbVerticalPositionType.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("cbVerticalPositionType.Properties.Buttons"))))});
			resources.ApplyResources(this.cbVerticalAlignment, "cbVerticalAlignment");
			this.cbVerticalAlignment.Name = "cbVerticalAlignment";
			this.cbVerticalAlignment.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("cbVerticalAlignment.Properties.Buttons"))))});
			resources.ApplyResources(this.cbHorizontalAbsolutePositionRightOf, "cbHorizontalAbsolutePositionRightOf");
			this.cbHorizontalAbsolutePositionRightOf.Name = "cbHorizontalAbsolutePositionRightOf";
			this.cbHorizontalAbsolutePositionRightOf.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("cbHorizontalAbsolutePositionRightOf.Properties.Buttons"))))});
			resources.ApplyResources(this.cbHorizontalPositionType, "cbHorizontalPositionType");
			this.cbHorizontalPositionType.Name = "cbHorizontalPositionType";
			this.cbHorizontalPositionType.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("cbHorizontalPositionType.Properties.Buttons"))))});
			resources.ApplyResources(this.cbHorizontalAlignment, "cbHorizontalAlignment");
			this.cbHorizontalAlignment.Name = "cbHorizontalAlignment";
			this.cbHorizontalAlignment.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("cbHorizontalAlignment.Properties.Buttons"))))});
			resources.ApplyResources(this.rgVertical, "rgVertical");
			this.rgVertical.Name = "rgVertical";
			this.rgVertical.Properties.Appearance.BackColor = ((System.Drawing.Color)(resources.GetObject("rgVertical.Properties.Appearance.BackColor")));
			this.rgVertical.Properties.Appearance.Options.UseBackColor = true;
			this.rgVertical.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.rgVertical.Properties.Items.AddRange(new DevExpress.XtraEditors.Controls.RadioGroupItem[] {
			new DevExpress.XtraEditors.Controls.RadioGroupItem(((object)(resources.GetObject("rgVertical.Properties.Items"))), resources.GetString("rgVertical.Properties.Items1")),
			new DevExpress.XtraEditors.Controls.RadioGroupItem(((object)(resources.GetObject("rgVertical.Properties.Items2"))), resources.GetString("rgVertical.Properties.Items3"))});
			resources.ApplyResources(this.rgHorizontal, "rgHorizontal");
			this.rgHorizontal.Name = "rgHorizontal";
			this.rgHorizontal.Properties.Appearance.BackColor = ((System.Drawing.Color)(resources.GetObject("rgHorizontal.Properties.Appearance.BackColor")));
			this.rgHorizontal.Properties.Appearance.Options.UseBackColor = true;
			this.rgHorizontal.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.rgHorizontal.Properties.Items.AddRange(new DevExpress.XtraEditors.Controls.RadioGroupItem[] {
			new DevExpress.XtraEditors.Controls.RadioGroupItem(((object)(resources.GetObject("rgHorizontal.Properties.Items"))), resources.GetString("rgHorizontal.Properties.Items1")),
			new DevExpress.XtraEditors.Controls.RadioGroupItem(((object)(resources.GetObject("rgHorizontal.Properties.Items2"))), resources.GetString("rgHorizontal.Properties.Items3"))});
			resources.ApplyResources(this.lblOptions, "lblOptions");
			this.lblOptions.LineVisible = true;
			this.lblOptions.Name = "lblOptions";
			resources.ApplyResources(this.lblVertical, "lblVertical");
			this.lblVertical.LineVisible = true;
			this.lblVertical.Name = "lblVertical";
			resources.ApplyResources(this.lblHorizontal, "lblHorizontal");
			this.lblHorizontal.LineVisible = true;
			this.lblHorizontal.Name = "lblHorizontal";
			this.tabPageTextWrapping.Controls.Add(this.spnRight);
			this.tabPageTextWrapping.Controls.Add(this.spnLeft);
			this.tabPageTextWrapping.Controls.Add(this.spnBottom);
			this.tabPageTextWrapping.Controls.Add(this.spnTop);
			this.tabPageTextWrapping.Controls.Add(this.lblRight);
			this.tabPageTextWrapping.Controls.Add(this.lblLeft);
			this.tabPageTextWrapping.Controls.Add(this.lblBottom);
			this.tabPageTextWrapping.Controls.Add(this.lblTop);
			this.tabPageTextWrapping.Controls.Add(this.rgTextWrapSide);
			this.tabPageTextWrapping.Controls.Add(this.lblDistance);
			this.tabPageTextWrapping.Controls.Add(this.WrapText);
			this.tabPageTextWrapping.Controls.Add(this.columnsPresetControlBehind);
			this.tabPageTextWrapping.Controls.Add(this.columnsPresetControlInFrontOf);
			this.tabPageTextWrapping.Controls.Add(this.columnsPresetControlThought);
			this.tabPageTextWrapping.Controls.Add(this.columnsPresetControlTopAndBottom);
			this.tabPageTextWrapping.Controls.Add(this.columnsPresetControlTight);
			this.tabPageTextWrapping.Controls.Add(this.columnsPresetControlSquare);
			this.tabPageTextWrapping.Controls.Add(this.lblWrappingStyle);
			this.tabPageTextWrapping.Name = "tabPageTextWrapping";
			resources.ApplyResources(this.tabPageTextWrapping, "tabPageTextWrapping");
			resources.ApplyResources(this.spnRight, "spnRight");
			this.spnRight.Name = "spnRight";
			this.spnRight.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton()});
			this.spnRight.Properties.IsValueInPercent = false;
			resources.ApplyResources(this.spnLeft, "spnLeft");
			this.spnLeft.Name = "spnLeft";
			this.spnLeft.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton()});
			this.spnLeft.Properties.IsValueInPercent = false;
			resources.ApplyResources(this.spnBottom, "spnBottom");
			this.spnBottom.Name = "spnBottom";
			this.spnBottom.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton()});
			this.spnBottom.Properties.IsValueInPercent = false;
			resources.ApplyResources(this.spnTop, "spnTop");
			this.spnTop.Name = "spnTop";
			this.spnTop.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton()});
			this.spnTop.Properties.IsValueInPercent = false;
			resources.ApplyResources(this.lblRight, "lblRight");
			this.lblRight.Name = "lblRight";
			resources.ApplyResources(this.lblLeft, "lblLeft");
			this.lblLeft.Name = "lblLeft";
			resources.ApplyResources(this.lblBottom, "lblBottom");
			this.lblBottom.Name = "lblBottom";
			resources.ApplyResources(this.lblTop, "lblTop");
			this.lblTop.Name = "lblTop";
			resources.ApplyResources(this.rgTextWrapSide, "rgTextWrapSide");
			this.rgTextWrapSide.Name = "rgTextWrapSide";
			this.rgTextWrapSide.Properties.Appearance.BackColor = ((System.Drawing.Color)(resources.GetObject("rgTextWrapSide.Properties.Appearance.BackColor")));
			this.rgTextWrapSide.Properties.Appearance.Options.UseBackColor = true;
			this.rgTextWrapSide.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.rgTextWrapSide.Properties.Items.AddRange(new DevExpress.XtraEditors.Controls.RadioGroupItem[] {
			new DevExpress.XtraEditors.Controls.RadioGroupItem(((object)(resources.GetObject("rgTextWrapSide.Properties.Items"))), resources.GetString("rgTextWrapSide.Properties.Items1")),
			new DevExpress.XtraEditors.Controls.RadioGroupItem(((object)(resources.GetObject("rgTextWrapSide.Properties.Items2"))), resources.GetString("rgTextWrapSide.Properties.Items3")),
			new DevExpress.XtraEditors.Controls.RadioGroupItem(((object)(resources.GetObject("rgTextWrapSide.Properties.Items4"))), resources.GetString("rgTextWrapSide.Properties.Items5")),
			new DevExpress.XtraEditors.Controls.RadioGroupItem(((object)(resources.GetObject("rgTextWrapSide.Properties.Items6"))), resources.GetString("rgTextWrapSide.Properties.Items7"))});
			resources.ApplyResources(this.lblDistance, "lblDistance");
			this.lblDistance.LineVisible = true;
			this.lblDistance.Name = "lblDistance";
			resources.ApplyResources(this.WrapText, "WrapText");
			this.WrapText.LineVisible = true;
			this.WrapText.Name = "WrapText";
			this.columnsPresetControlBehind.Appearance.BackColor = ((System.Drawing.Color)(resources.GetObject("columnsPresetControlBehind.Appearance.BackColor")));
			this.columnsPresetControlBehind.Appearance.Options.UseBackColor = true;
			this.columnsPresetControlBehind.Image = ((System.Drawing.Image)(resources.GetObject("columnsPresetControlBehind.Image")));
			resources.ApplyResources(this.columnsPresetControlBehind, "columnsPresetControlBehind");
			this.columnsPresetControlBehind.Name = "columnsPresetControlBehind";
			this.columnsPresetControlInFrontOf.Appearance.BackColor = ((System.Drawing.Color)(resources.GetObject("columnsPresetControlInFrontOf.Appearance.BackColor")));
			this.columnsPresetControlInFrontOf.Appearance.Options.UseBackColor = true;
			this.columnsPresetControlInFrontOf.Image = ((System.Drawing.Image)(resources.GetObject("columnsPresetControlInFrontOf.Image")));
			resources.ApplyResources(this.columnsPresetControlInFrontOf, "columnsPresetControlInFrontOf");
			this.columnsPresetControlInFrontOf.Name = "columnsPresetControlInFrontOf";
			this.columnsPresetControlThought.Appearance.BackColor = ((System.Drawing.Color)(resources.GetObject("columnsPresetControlThought.Appearance.BackColor")));
			this.columnsPresetControlThought.Appearance.Options.UseBackColor = true;
			this.columnsPresetControlThought.Image = ((System.Drawing.Image)(resources.GetObject("columnsPresetControlThought.Image")));
			resources.ApplyResources(this.columnsPresetControlThought, "columnsPresetControlThought");
			this.columnsPresetControlThought.Name = "columnsPresetControlThought";
			this.columnsPresetControlTopAndBottom.Appearance.BackColor = ((System.Drawing.Color)(resources.GetObject("columnsPresetControlTopAndBottom.Appearance.BackColor")));
			this.columnsPresetControlTopAndBottom.Appearance.Options.UseBackColor = true;
			this.columnsPresetControlTopAndBottom.Image = ((System.Drawing.Image)(resources.GetObject("columnsPresetControlTopAndBottom.Image")));
			resources.ApplyResources(this.columnsPresetControlTopAndBottom, "columnsPresetControlTopAndBottom");
			this.columnsPresetControlTopAndBottom.Name = "columnsPresetControlTopAndBottom";
			this.columnsPresetControlTight.Appearance.BackColor = ((System.Drawing.Color)(resources.GetObject("columnsPresetControlTight.Appearance.BackColor")));
			this.columnsPresetControlTight.Appearance.Options.UseBackColor = true;
			this.columnsPresetControlTight.Image = ((System.Drawing.Image)(resources.GetObject("columnsPresetControlTight.Image")));
			resources.ApplyResources(this.columnsPresetControlTight, "columnsPresetControlTight");
			this.columnsPresetControlTight.Name = "columnsPresetControlTight";
			this.columnsPresetControlSquare.Appearance.BackColor = ((System.Drawing.Color)(resources.GetObject("columnsPresetControlSquare.Appearance.BackColor")));
			this.columnsPresetControlSquare.Appearance.ForeColor = ((System.Drawing.Color)(resources.GetObject("columnsPresetControlSquare.Appearance.ForeColor")));
			this.columnsPresetControlSquare.Appearance.Options.UseBackColor = true;
			this.columnsPresetControlSquare.Appearance.Options.UseForeColor = true;
			this.columnsPresetControlSquare.Image = ((System.Drawing.Image)(resources.GetObject("columnsPresetControlSquare.Image")));
			resources.ApplyResources(this.columnsPresetControlSquare, "columnsPresetControlSquare");
			this.columnsPresetControlSquare.Name = "columnsPresetControlSquare";
			resources.ApplyResources(this.lblWrappingStyle, "lblWrappingStyle");
			this.lblWrappingStyle.LineVisible = true;
			this.lblWrappingStyle.Name = "lblWrappingStyle";
			this.tabPageSize.Controls.Add(this.btnReset);
			this.tabPageSize.Controls.Add(this.lblOriginalSizeWidthValue);
			this.tabPageSize.Controls.Add(this.lblOriginalSizeHeightValue);
			this.tabPageSize.Controls.Add(this.lblWidthAbsolute);
			this.tabPageSize.Controls.Add(this.lblHeightAbsolute);
			this.tabPageSize.Controls.Add(this.lblOriginalSizeWidth);
			this.tabPageSize.Controls.Add(this.lblOriginalSizeHeight);
			this.tabPageSize.Controls.Add(this.lblOriginalSize);
			this.tabPageSize.Controls.Add(this.spnWidthAbs);
			this.tabPageSize.Controls.Add(this.lblWidth);
			this.tabPageSize.Controls.Add(this.spnHeightAbs);
			this.tabPageSize.Controls.Add(this.lblHeight);
			this.tabPageSize.Controls.Add(this.lblRotation);
			this.tabPageSize.Controls.Add(this.lblRotate);
			this.tabPageSize.Controls.Add(this.spnRotation);
			this.tabPageSize.Controls.Add(this.chkLockAspectRatio);
			this.tabPageSize.Controls.Add(this.lblScale);
			this.tabPageSize.Name = "tabPageSize";
			resources.ApplyResources(this.tabPageSize, "tabPageSize");
			resources.ApplyResources(this.btnReset, "btnReset");
			this.btnReset.Name = "btnReset";
			resources.ApplyResources(this.lblOriginalSizeWidthValue, "lblOriginalSizeWidthValue");
			this.lblOriginalSizeWidthValue.Name = "lblOriginalSizeWidthValue";
			resources.ApplyResources(this.lblOriginalSizeHeightValue, "lblOriginalSizeHeightValue");
			this.lblOriginalSizeHeightValue.Name = "lblOriginalSizeHeightValue";
			resources.ApplyResources(this.lblWidthAbsolute, "lblWidthAbsolute");
			this.lblWidthAbsolute.Name = "lblWidthAbsolute";
			resources.ApplyResources(this.lblHeightAbsolute, "lblHeightAbsolute");
			this.lblHeightAbsolute.Name = "lblHeightAbsolute";
			resources.ApplyResources(this.lblOriginalSizeWidth, "lblOriginalSizeWidth");
			this.lblOriginalSizeWidth.Name = "lblOriginalSizeWidth";
			resources.ApplyResources(this.lblOriginalSizeHeight, "lblOriginalSizeHeight");
			this.lblOriginalSizeHeight.Name = "lblOriginalSizeHeight";
			resources.ApplyResources(this.lblOriginalSize, "lblOriginalSize");
			this.lblOriginalSize.LineVisible = true;
			this.lblOriginalSize.Name = "lblOriginalSize";
			resources.ApplyResources(this.spnWidthAbs, "spnWidthAbs");
			this.spnWidthAbs.Name = "spnWidthAbs";
			this.spnWidthAbs.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton()});
			this.spnWidthAbs.Properties.IsValueInPercent = false;
			this.spnWidthAbs.Properties.MaxValue = 100000;
			this.spnWidthAbs.Properties.MinValue = 1;
			resources.ApplyResources(this.lblWidth, "lblWidth");
			this.lblWidth.LineVisible = true;
			this.lblWidth.Name = "lblWidth";
			resources.ApplyResources(this.spnHeightAbs, "spnHeightAbs");
			this.spnHeightAbs.Name = "spnHeightAbs";
			this.spnHeightAbs.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton()});
			this.spnHeightAbs.Properties.IsValueInPercent = false;
			this.spnHeightAbs.Properties.MaxValue = 100000;
			this.spnHeightAbs.Properties.MinValue = 1;
			resources.ApplyResources(this.lblHeight, "lblHeight");
			this.lblHeight.LineVisible = true;
			this.lblHeight.Name = "lblHeight";
			resources.ApplyResources(this.lblRotation, "lblRotation");
			this.lblRotation.Name = "lblRotation";
			resources.ApplyResources(this.lblRotate, "lblRotate");
			this.lblRotate.LineVisible = true;
			this.lblRotate.Name = "lblRotate";
			resources.ApplyResources(this.spnRotation, "spnRotation");
			this.spnRotation.Name = "spnRotation";
			this.spnRotation.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton()});
			this.spnRotation.Properties.IsFloatValue = false;
			this.spnRotation.Properties.Mask.EditMask = resources.GetString("spnRotation.Properties.Mask.EditMask");
			this.spnRotation.Properties.Mask.ShowPlaceHolders = ((bool)(resources.GetObject("spnRotation.Properties.Mask.ShowPlaceHolders")));
			this.spnRotation.Properties.Mask.UseMaskAsDisplayFormat = ((bool)(resources.GetObject("spnRotation.Properties.Mask.UseMaskAsDisplayFormat")));
			resources.ApplyResources(this.chkLockAspectRatio, "chkLockAspectRatio");
			this.chkLockAspectRatio.Name = "chkLockAspectRatio";
			this.chkLockAspectRatio.Properties.AutoWidth = true;
			this.chkLockAspectRatio.Properties.Caption = resources.GetString("chkLockAspectRatio.Properties.Caption");
			resources.ApplyResources(this.lblScale, "lblScale");
			this.lblScale.LineVisible = true;
			this.lblScale.Name = "lblScale";
			resources.ApplyResources(this.btnOk, "btnOk");
			this.btnOk.Name = "btnOk";
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			resources.ApplyResources(this.btnCancel, "btnCancel");
			this.btnCancel.Name = "btnCancel";
			this.AcceptButton = this.btnOk;
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.btnCancel;
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.btnOk);
			this.Controls.Add(this.tabControl);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FloatingObjectLayoutForm";
			this.ShowInTaskbar = false;
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			((System.ComponentModel.ISupportInitialize)(this.tabControl)).EndInit();
			this.tabControl.ResumeLayout(false);
			this.tabPagePosition.ResumeLayout(false);
			this.tabPagePosition.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.spnVerticalAbsolutePosition.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.spnHorizontalAbsolutePosition.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chkLock.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.cbVerticalAbsolutePositionBelow.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.cbVerticalPositionType.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.cbVerticalAlignment.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.cbHorizontalAbsolutePositionRightOf.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.cbHorizontalPositionType.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.cbHorizontalAlignment.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.rgVertical.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.rgHorizontal.Properties)).EndInit();
			this.tabPageTextWrapping.ResumeLayout(false);
			this.tabPageTextWrapping.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.spnRight.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.spnLeft.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.spnBottom.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.spnTop.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.rgTextWrapSide.Properties)).EndInit();
			this.tabPageSize.ResumeLayout(false);
			this.tabPageSize.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.spnWidthAbs.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.spnHeightAbs.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.spnRotation.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chkLockAspectRatio.Properties)).EndInit();
			this.ResumeLayout(false);
		}
		#endregion
		protected DevExpress.XtraTab.XtraTabControl tabControl;
		protected DevExpress.XtraTab.XtraTabPage tabPagePosition;
		protected DevExpress.XtraTab.XtraTabPage tabPageTextWrapping;
		protected DevExpress.XtraTab.XtraTabPage tabPageSize;
		protected DevExpress.XtraEditors.SimpleButton btnOk;
		protected DevExpress.XtraEditors.SimpleButton btnCancel;
		protected DevExpress.XtraEditors.RadioGroup rgHorizontal;
		protected DevExpress.XtraEditors.RadioGroup rgVertical;
		protected DevExpress.XtraEditors.ImageComboBoxEdit cbVerticalAbsolutePositionBelow;
		protected DevExpress.XtraEditors.ImageComboBoxEdit cbVerticalPositionType;
		protected DevExpress.XtraEditors.ImageComboBoxEdit cbVerticalAlignment;
		protected DevExpress.XtraEditors.ImageComboBoxEdit cbHorizontalAbsolutePositionRightOf;
		protected DevExpress.XtraEditors.ImageComboBoxEdit cbHorizontalPositionType;
		protected DevExpress.XtraEditors.ImageComboBoxEdit cbHorizontalAlignment;
		protected DevExpress.XtraEditors.CheckEdit chkLock;
		protected DevExpress.XtraRichEdit.Forms.Design.ColumnsPresetControl columnsPresetControlSquare;
		protected DevExpress.XtraRichEdit.Forms.Design.ColumnsPresetControl columnsPresetControlBehind;
		protected DevExpress.XtraRichEdit.Forms.Design.ColumnsPresetControl columnsPresetControlInFrontOf;
		protected DevExpress.XtraRichEdit.Forms.Design.ColumnsPresetControl columnsPresetControlThought;
		protected DevExpress.XtraRichEdit.Forms.Design.ColumnsPresetControl columnsPresetControlTopAndBottom;
		protected DevExpress.XtraRichEdit.Forms.Design.ColumnsPresetControl columnsPresetControlTight;
		protected DevExpress.XtraEditors.RadioGroup rgTextWrapSide;
		protected DevExpress.XtraEditors.LabelControl lblRight;
		protected DevExpress.XtraEditors.LabelControl lblLeft;
		protected DevExpress.XtraEditors.LabelControl lblBottom;
		protected DevExpress.XtraEditors.LabelControl lblTop;
		protected DevExpress.XtraEditors.CheckEdit chkLockAspectRatio;
		protected DevExpress.XtraRichEdit.Design.RichTextIndentEdit spnVerticalAbsolutePosition;
		protected DevExpress.XtraRichEdit.Design.RichTextIndentEdit spnHorizontalAbsolutePosition;
		protected DevExpress.XtraRichEdit.Design.RichTextIndentEdit spnRight;
		protected DevExpress.XtraRichEdit.Design.RichTextIndentEdit spnLeft;
		protected DevExpress.XtraRichEdit.Design.RichTextIndentEdit spnBottom;
		protected DevExpress.XtraRichEdit.Design.RichTextIndentEdit spnTop;
		protected DevExpress.XtraEditors.LabelControl lblHorizontalPositionType;
		protected DevExpress.XtraEditors.LabelControl lblVerticalPositionType;
		protected DevExpress.XtraEditors.LabelControl lblHorizontalAbsolutePosition;
		protected DevExpress.XtraEditors.LabelControl lblVerticalAbsolutePosition;
		protected DevExpress.XtraEditors.LabelControl lblHorizontal;
		protected DevExpress.XtraEditors.LabelControl lblOptions;
		protected DevExpress.XtraEditors.LabelControl lblVertical;
		protected DevExpress.XtraEditors.LabelControl lblWrappingStyle;
		protected DevExpress.XtraEditors.LabelControl WrapText;
		protected DevExpress.XtraEditors.LabelControl lblDistance;
		protected DevExpress.XtraEditors.LabelControl lblScale;
		protected XtraEditors.SpinEdit spnRotation;
		protected XtraEditors.LabelControl lblRotate;
		protected XtraEditors.LabelControl lblRotation;
		protected XtraRichEdit.Design.RichTextIndentEdit spnHeightAbs;
		protected XtraEditors.LabelControl lblHeight;
		protected XtraRichEdit.Design.RichTextIndentEdit spnWidthAbs;
		protected XtraEditors.LabelControl lblWidth;
		protected XtraEditors.LabelControl lblWidthAbsolute;
		protected XtraEditors.LabelControl lblHeightAbsolute;
		protected XtraEditors.LabelControl lblOriginalSizeWidth;
		protected XtraEditors.LabelControl lblOriginalSizeHeight;
		protected XtraEditors.LabelControl lblOriginalSize;
		protected XtraEditors.LabelControl lblOriginalSizeWidthValue;
		protected XtraEditors.LabelControl lblOriginalSizeHeightValue;
		protected XtraEditors.SimpleButton btnReset;
	}
}
