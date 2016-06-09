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

namespace DevExpress.XtraPdfViewer.Forms {
	partial class PdfPageSetupDialog {
		private System.ComponentModel.IContainer components = null;
		#region Windows Form Designer generated code
		private void InitializeComponent() {
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PdfPageSetupDialog));
			this.errorProvider = new DevExpress.XtraEditors.DXErrorProvider.DXErrorProvider(this.components);
			this.pcPreviewImage = new DevExpress.XtraEditors.PanelControl();
			this.tlpPager = new System.Windows.Forms.TableLayoutPanel();
			this.cnPagePreviewNumber = new DevExpress.XtraEditors.ControlNavigator();
			this.lbFitScale = new DevExpress.XtraEditors.LabelControl();
			this.splitter = new System.Windows.Forms.Splitter();
			this.pcOptions = new System.Windows.Forms.Panel();
			this.scrollableControl = new DevExpress.XtraEditors.XtraScrollableControl();
			this.tlpProperties = new System.Windows.Forms.TableLayoutPanel();
			this.lbPrinterName = new DevExpress.XtraEditors.LabelControl();
			this.btnPrinterPreferences = new DevExpress.XtraEditors.SimpleButton();
			this.icbInstalledPrinters = new DevExpress.XtraEditors.ImageComboBoxEdit();
			this.lbPrinterStatusCaption = new DevExpress.XtraEditors.LabelControl();
			this.lbPrinterStatus = new DevExpress.XtraEditors.LabelControl();
			this.lcPrinterStatus = new DevExpress.XtraEditors.LabelControl();
			this.lbPrinterLocationCaption = new DevExpress.XtraEditors.LabelControl();
			this.lcPrinterLocation = new DevExpress.XtraEditors.LabelControl();
			this.lbPrinterLocation = new DevExpress.XtraEditors.LabelControl();
			this.lbPrinterCommentCaption = new DevExpress.XtraEditors.LabelControl();
			this.lcPrinterComment = new DevExpress.XtraEditors.LabelControl();
			this.lbPrinterComment = new DevExpress.XtraEditors.LabelControl();
			this.lbPrinterDocumentsInQueueCaption = new DevExpress.XtraEditors.LabelControl();
			this.lciPrinterDocumentsInQueue = new DevExpress.XtraEditors.LabelControl();
			this.lbDocumentsInQueue = new DevExpress.XtraEditors.LabelControl();
			this.spPrintingDpi = new DevExpress.XtraEditors.SpinEdit();
			this.lbPrintingDpi = new DevExpress.XtraEditors.LabelControl();
			this.spCopies = new DevExpress.XtraEditors.SpinEdit();
			this.lbCopies = new DevExpress.XtraEditors.LabelControl();
			this.cbCollate = new DevExpress.XtraEditors.CheckEdit();
			this.lbPageRange = new DevExpress.XtraEditors.LabelControl();
			this.rbPrintRangeAll = new DevExpress.XtraEditors.CheckEdit();
			this.rbPrintRangeCurrent = new DevExpress.XtraEditors.CheckEdit();
			this.rbPrintRangeSome = new DevExpress.XtraEditors.CheckEdit();
			this.lbPageRangeExample = new DevExpress.XtraEditors.LabelControl();
			this.tePageRange = new DevExpress.XtraEditors.TextEdit();
			this.lbPageSizing = new DevExpress.XtraEditors.LabelControl();
			this.rbPageScaleFit = new DevExpress.XtraEditors.CheckEdit();
			this.rbPageScaleActualSize = new DevExpress.XtraEditors.CheckEdit();
			this.rbPageScaleCustom = new DevExpress.XtraEditors.CheckEdit();
			this.teCustomScale = new DevExpress.XtraEditors.TextEdit();
			this.lbPercent = new DevExpress.XtraEditors.LabelControl();
			this.lbPageOrientation = new DevExpress.XtraEditors.LabelControl();
			this.rbAutoOrientation = new DevExpress.XtraEditors.CheckEdit();
			this.rbPortraitOrientation = new DevExpress.XtraEditors.CheckEdit();
			this.rbLandscapeOrientation = new DevExpress.XtraEditors.CheckEdit();
			this.lbPaperSource = new DevExpress.XtraEditors.LabelControl();
			this.cbPaperSource = new DevExpress.XtraEditors.ComboBoxEdit();
			this.lbFilePath = new DevExpress.XtraEditors.LabelControl();
			this.cbPrintToFile = new DevExpress.XtraEditors.CheckEdit();
			this.beBrowse = new DevExpress.XtraEditors.ButtonEdit();
			this.panelMinWidth = new System.Windows.Forms.Panel();
			this.tlpButtons = new System.Windows.Forms.TableLayoutPanel();
			this.btnPrint = new DevExpress.XtraEditors.SimpleButton();
			this.btnCancel = new DevExpress.XtraEditors.SimpleButton();
			this.pePreviewImage = new DevExpress.XtraPdfViewer.Native.PdfPreviewPictureEdit();
			((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pcPreviewImage)).BeginInit();
			this.pcPreviewImage.SuspendLayout();
			this.tlpPager.SuspendLayout();
			this.pcOptions.SuspendLayout();
			this.scrollableControl.SuspendLayout();
			this.tlpProperties.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.icbInstalledPrinters.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.spPrintingDpi.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.spCopies.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.cbCollate.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.rbPrintRangeAll.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.rbPrintRangeCurrent.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.rbPrintRangeSome.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.tePageRange.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.rbPageScaleFit.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.rbPageScaleActualSize.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.rbPageScaleCustom.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.teCustomScale.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.rbAutoOrientation.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.rbPortraitOrientation.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.rbLandscapeOrientation.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.cbPaperSource.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.cbPrintToFile.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.beBrowse.Properties)).BeginInit();
			this.tlpButtons.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pePreviewImage.Properties)).BeginInit();
			this.SuspendLayout();
			this.errorProvider.ContainerControl = this;
			this.pcPreviewImage.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.pcPreviewImage.Controls.Add(this.pePreviewImage);
			this.pcPreviewImage.Controls.Add(this.tlpPager);
			resources.ApplyResources(this.pcPreviewImage, "pcPreviewImage");
			this.pcPreviewImage.Name = "pcPreviewImage";
			resources.ApplyResources(this.tlpPager, "tlpPager");
			this.tlpPager.Controls.Add(this.cnPagePreviewNumber, 1, 0);
			this.tlpPager.Controls.Add(this.lbFitScale, 0, 0);
			this.tlpPager.Name = "tlpPager";
			this.cnPagePreviewNumber.Buttons.Append.Visible = false;
			this.cnPagePreviewNumber.Buttons.CancelEdit.Visible = false;
			this.cnPagePreviewNumber.Buttons.Edit.Visible = false;
			this.cnPagePreviewNumber.Buttons.EndEdit.Visible = false;
			this.cnPagePreviewNumber.Buttons.NextPage.Visible = false;
			this.cnPagePreviewNumber.Buttons.PrevPage.Visible = false;
			this.cnPagePreviewNumber.Buttons.Remove.Visible = false;
			resources.ApplyResources(this.cnPagePreviewNumber, "cnPagePreviewNumber");
			this.cnPagePreviewNumber.Name = "cnPagePreviewNumber";
			this.cnPagePreviewNumber.TabStop = true;
			resources.ApplyResources(this.lbFitScale, "lbFitScale");
			this.lbFitScale.Name = "lbFitScale";
			resources.ApplyResources(this.splitter, "splitter");
			this.splitter.Name = "splitter";
			this.splitter.TabStop = false;
			this.splitter.SplitterMoved += new System.Windows.Forms.SplitterEventHandler(this.OnSplitterMoved);
			this.pcOptions.Controls.Add(this.scrollableControl);
			this.pcOptions.Controls.Add(this.tlpButtons);
			resources.ApplyResources(this.pcOptions, "pcOptions");
			this.pcOptions.Name = "pcOptions";
			resources.ApplyResources(this.scrollableControl, "scrollableControl");
			this.scrollableControl.Controls.Add(this.tlpProperties);
			this.scrollableControl.Controls.Add(this.panelMinWidth);
			this.scrollableControl.Name = "scrollableControl";
			this.scrollableControl.TabStop = false;
			resources.ApplyResources(this.tlpProperties, "tlpProperties");
			this.tlpProperties.Controls.Add(this.lbPrinterName, 0, 0);
			this.tlpProperties.Controls.Add(this.btnPrinterPreferences, 3, 0);
			this.tlpProperties.Controls.Add(this.icbInstalledPrinters, 1, 0);
			this.tlpProperties.Controls.Add(this.lbPrinterStatusCaption, 0, 1);
			this.tlpProperties.Controls.Add(this.lbPrinterStatus, 1, 1);
			this.tlpProperties.Controls.Add(this.lcPrinterStatus, 3, 1);
			this.tlpProperties.Controls.Add(this.lbPrinterLocationCaption, 0, 3);
			this.tlpProperties.Controls.Add(this.lcPrinterLocation, 1, 3);
			this.tlpProperties.Controls.Add(this.lbPrinterLocation, 3, 3);
			this.tlpProperties.Controls.Add(this.lbPrinterCommentCaption, 0, 5);
			this.tlpProperties.Controls.Add(this.lcPrinterComment, 1, 5);
			this.tlpProperties.Controls.Add(this.lbPrinterComment, 3, 5);
			this.tlpProperties.Controls.Add(this.lbPrinterDocumentsInQueueCaption, 0, 7);
			this.tlpProperties.Controls.Add(this.lciPrinterDocumentsInQueue, 3, 7);
			this.tlpProperties.Controls.Add(this.lbDocumentsInQueue, 1, 7);
			this.tlpProperties.Controls.Add(this.spPrintingDpi, 1, 9);
			this.tlpProperties.Controls.Add(this.lbPrintingDpi, 0, 9);
			this.tlpProperties.Controls.Add(this.spCopies, 1, 10);
			this.tlpProperties.Controls.Add(this.lbCopies, 0, 10);
			this.tlpProperties.Controls.Add(this.cbCollate, 3, 10);
			this.tlpProperties.Controls.Add(this.lbPageRange, 0, 12);
			this.tlpProperties.Controls.Add(this.rbPrintRangeAll, 1, 12);
			this.tlpProperties.Controls.Add(this.rbPrintRangeCurrent, 1, 13);
			this.tlpProperties.Controls.Add(this.rbPrintRangeSome, 1, 14);
			this.tlpProperties.Controls.Add(this.lbPageRangeExample, 3, 14);
			this.tlpProperties.Controls.Add(this.tePageRange, 2, 14);
			this.tlpProperties.Controls.Add(this.lbPageSizing, 0, 16);
			this.tlpProperties.Controls.Add(this.rbPageScaleFit, 1, 16);
			this.tlpProperties.Controls.Add(this.rbPageScaleActualSize, 1, 17);
			this.tlpProperties.Controls.Add(this.rbPageScaleCustom, 1, 18);
			this.tlpProperties.Controls.Add(this.teCustomScale, 2, 18);
			this.tlpProperties.Controls.Add(this.lbPercent, 3, 18);
			this.tlpProperties.Controls.Add(this.lbPageOrientation, 0, 20);
			this.tlpProperties.Controls.Add(this.rbAutoOrientation, 1, 20);
			this.tlpProperties.Controls.Add(this.rbPortraitOrientation, 1, 21);
			this.tlpProperties.Controls.Add(this.rbLandscapeOrientation, 1, 22);
			this.tlpProperties.Controls.Add(this.lbPaperSource, 0, 24);
			this.tlpProperties.Controls.Add(this.cbPaperSource, 1, 24);
			this.tlpProperties.Controls.Add(this.lbFilePath, 0, 25);
			this.tlpProperties.Controls.Add(this.cbPrintToFile, 3, 25);
			this.tlpProperties.Controls.Add(this.beBrowse, 1, 25);
			this.tlpProperties.Name = "tlpProperties";
			resources.ApplyResources(this.lbPrinterName, "lbPrinterName");
			this.lbPrinterName.Name = "lbPrinterName";
			resources.ApplyResources(this.btnPrinterPreferences, "btnPrinterPreferences");
			this.btnPrinterPreferences.Name = "btnPrinterPreferences";
			this.btnPrinterPreferences.Click += new System.EventHandler(this.OnPrinterPreferencesClick);
			this.tlpProperties.SetColumnSpan(this.icbInstalledPrinters, 2);
			resources.ApplyResources(this.icbInstalledPrinters, "icbInstalledPrinters");
			this.icbInstalledPrinters.Name = "icbInstalledPrinters";
			this.icbInstalledPrinters.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("icbInstalledPrinters.Properties.Buttons"))))});
			this.icbInstalledPrinters.Properties.DropDownItemHeight = 46;
			this.icbInstalledPrinters.Properties.DropDownRows = 6;
			resources.ApplyResources(this.lbPrinterStatusCaption, "lbPrinterStatusCaption");
			this.lbPrinterStatusCaption.Name = "lbPrinterStatusCaption";
			this.tlpProperties.SetColumnSpan(this.lbPrinterStatus, 2);
			resources.ApplyResources(this.lbPrinterStatus, "lbPrinterStatus");
			this.lbPrinterStatus.Name = "lbPrinterStatus";
			resources.ApplyResources(this.lcPrinterStatus, "lcPrinterStatus");
			this.lcPrinterStatus.Name = "lcPrinterStatus";
			resources.ApplyResources(this.lbPrinterLocationCaption, "lbPrinterLocationCaption");
			this.lbPrinterLocationCaption.Name = "lbPrinterLocationCaption";
			this.tlpProperties.SetColumnSpan(this.lcPrinterLocation, 2);
			resources.ApplyResources(this.lcPrinterLocation, "lcPrinterLocation");
			this.lcPrinterLocation.Name = "lcPrinterLocation";
			resources.ApplyResources(this.lbPrinterLocation, "lbPrinterLocation");
			this.lbPrinterLocation.Name = "lbPrinterLocation";
			resources.ApplyResources(this.lbPrinterCommentCaption, "lbPrinterCommentCaption");
			this.lbPrinterCommentCaption.Name = "lbPrinterCommentCaption";
			this.tlpProperties.SetColumnSpan(this.lcPrinterComment, 2);
			this.lcPrinterComment.Cursor = System.Windows.Forms.Cursors.Default;
			resources.ApplyResources(this.lcPrinterComment, "lcPrinterComment");
			this.lcPrinterComment.Name = "lcPrinterComment";
			resources.ApplyResources(this.lbPrinterComment, "lbPrinterComment");
			this.lbPrinterComment.Name = "lbPrinterComment";
			resources.ApplyResources(this.lbPrinterDocumentsInQueueCaption, "lbPrinterDocumentsInQueueCaption");
			this.lbPrinterDocumentsInQueueCaption.Name = "lbPrinterDocumentsInQueueCaption";
			resources.ApplyResources(this.lciPrinterDocumentsInQueue, "lciPrinterDocumentsInQueue");
			this.lciPrinterDocumentsInQueue.Name = "lciPrinterDocumentsInQueue";
			this.tlpProperties.SetColumnSpan(this.lbDocumentsInQueue, 2);
			resources.ApplyResources(this.lbDocumentsInQueue, "lbDocumentsInQueue");
			this.lbDocumentsInQueue.Name = "lbDocumentsInQueue";
			this.tlpProperties.SetColumnSpan(this.spPrintingDpi, 2);
			resources.ApplyResources(this.spPrintingDpi, "spPrintingDpi");
			this.spPrintingDpi.Name = "spPrintingDpi";
			this.spPrintingDpi.Properties.AutoHeight = ((bool)(resources.GetObject("spPrintingDpi.Properties.AutoHeight")));
			this.spPrintingDpi.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("spPrintingDpi.Properties.Buttons"))))});
			this.spPrintingDpi.Properties.Increment = new decimal(new int[] {
			100,
			0,
			0,
			0});
			this.spPrintingDpi.Properties.Mask.EditMask = resources.GetString("spPrintingDpi.Properties.Mask.EditMask");
			this.spPrintingDpi.Properties.Mask.MaskType = ((DevExpress.XtraEditors.Mask.MaskType)(resources.GetObject("spPrintingDpi.Properties.Mask.MaskType")));
			this.spPrintingDpi.Properties.MaxValue = new decimal(new int[] {
			100,
			0,
			0,
			0});
			this.spPrintingDpi.Properties.MinValue = new decimal(new int[] {
			100,
			0,
			0,
			0});
			this.spPrintingDpi.Properties.ValidateOnEnterKey = true;
			resources.ApplyResources(this.lbPrintingDpi, "lbPrintingDpi");
			this.lbPrintingDpi.Name = "lbPrintingDpi";
			this.tlpProperties.SetColumnSpan(this.spCopies, 2);
			resources.ApplyResources(this.spCopies, "spCopies");
			this.spCopies.Name = "spCopies";
			this.spCopies.Properties.AutoHeight = ((bool)(resources.GetObject("spCopies.Properties.AutoHeight")));
			this.spCopies.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("spCopies.Properties.Buttons"))))});
			this.spCopies.Properties.Mask.EditMask = resources.GetString("spCopies.Properties.Mask.EditMask");
			this.spCopies.Properties.Mask.MaskType = ((DevExpress.XtraEditors.Mask.MaskType)(resources.GetObject("spCopies.Properties.Mask.MaskType")));
			this.spCopies.Properties.Mask.ShowPlaceHolders = ((bool)(resources.GetObject("spCopies.Properties.Mask.ShowPlaceHolders")));
			this.spCopies.Properties.MaxValue = new decimal(new int[] {
			32767,
			0,
			0,
			0});
			this.spCopies.Properties.MinValue = new decimal(new int[] {
			1,
			0,
			0,
			0});
			resources.ApplyResources(this.lbCopies, "lbCopies");
			this.lbCopies.Name = "lbCopies";
			this.cbCollate.AutoSizeInLayoutControl = true;
			resources.ApplyResources(this.cbCollate, "cbCollate");
			this.cbCollate.Name = "cbCollate";
			this.cbCollate.Properties.AutoHeight = ((bool)(resources.GetObject("cbCollate.Properties.AutoHeight")));
			this.cbCollate.Properties.AutoWidth = true;
			this.cbCollate.Properties.Caption = resources.GetString("cbCollate.Properties.Caption");
			resources.ApplyResources(this.lbPageRange, "lbPageRange");
			this.lbPageRange.Name = "lbPageRange";
			this.rbPrintRangeAll.AutoSizeInLayoutControl = true;
			this.tlpProperties.SetColumnSpan(this.rbPrintRangeAll, 3);
			resources.ApplyResources(this.rbPrintRangeAll, "rbPrintRangeAll");
			this.rbPrintRangeAll.Name = "rbPrintRangeAll";
			this.rbPrintRangeAll.Properties.AutoHeight = ((bool)(resources.GetObject("rbPrintRangeAll.Properties.AutoHeight")));
			this.rbPrintRangeAll.Properties.Caption = resources.GetString("rbPrintRangeAll.Properties.Caption");
			this.rbPrintRangeAll.Properties.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.Radio;
			this.rbPrintRangeAll.Properties.RadioGroupIndex = 0;
			this.rbPrintRangeCurrent.AutoSizeInLayoutControl = true;
			this.tlpProperties.SetColumnSpan(this.rbPrintRangeCurrent, 3);
			resources.ApplyResources(this.rbPrintRangeCurrent, "rbPrintRangeCurrent");
			this.rbPrintRangeCurrent.Name = "rbPrintRangeCurrent";
			this.rbPrintRangeCurrent.Properties.AutoHeight = ((bool)(resources.GetObject("rbPrintRangeCurrent.Properties.AutoHeight")));
			this.rbPrintRangeCurrent.Properties.Caption = resources.GetString("rbPrintRangeCurrent.Properties.Caption");
			this.rbPrintRangeCurrent.Properties.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.Radio;
			this.rbPrintRangeCurrent.Properties.RadioGroupIndex = 0;
			this.rbPrintRangeCurrent.TabStop = false;
			resources.ApplyResources(this.rbPrintRangeSome, "rbPrintRangeSome");
			this.rbPrintRangeSome.Name = "rbPrintRangeSome";
			this.rbPrintRangeSome.Properties.AutoHeight = ((bool)(resources.GetObject("rbPrintRangeSome.Properties.AutoHeight")));
			this.rbPrintRangeSome.Properties.Caption = resources.GetString("rbPrintRangeSome.Properties.Caption");
			this.rbPrintRangeSome.Properties.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.Radio;
			this.rbPrintRangeSome.Properties.RadioGroupIndex = 0;
			this.rbPrintRangeSome.TabStop = false;
			resources.ApplyResources(this.lbPageRangeExample, "lbPageRangeExample");
			this.lbPageRangeExample.Name = "lbPageRangeExample";
			resources.ApplyResources(this.tePageRange, "tePageRange");
			this.tePageRange.Name = "tePageRange";
			this.tePageRange.Properties.AutoHeight = ((bool)(resources.GetObject("tePageRange.Properties.AutoHeight")));
			this.tePageRange.Properties.Mask.EditMask = resources.GetString("tePageRange.Properties.Mask.EditMask");
			this.tePageRange.Properties.Mask.MaskType = ((DevExpress.XtraEditors.Mask.MaskType)(resources.GetObject("tePageRange.Properties.Mask.MaskType")));
			this.tePageRange.Properties.Mask.ShowPlaceHolders = ((bool)(resources.GetObject("tePageRange.Properties.Mask.ShowPlaceHolders")));
			resources.ApplyResources(this.lbPageSizing, "lbPageSizing");
			this.lbPageSizing.Name = "lbPageSizing";
			this.rbPageScaleFit.AutoSizeInLayoutControl = true;
			this.tlpProperties.SetColumnSpan(this.rbPageScaleFit, 3);
			resources.ApplyResources(this.rbPageScaleFit, "rbPageScaleFit");
			this.rbPageScaleFit.Name = "rbPageScaleFit";
			this.rbPageScaleFit.Properties.AutoHeight = ((bool)(resources.GetObject("rbPageScaleFit.Properties.AutoHeight")));
			this.rbPageScaleFit.Properties.Caption = resources.GetString("rbPageScaleFit.Properties.Caption");
			this.rbPageScaleFit.Properties.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.Radio;
			this.rbPageScaleFit.Properties.RadioGroupIndex = 1;
			this.rbPageScaleActualSize.AutoSizeInLayoutControl = true;
			this.tlpProperties.SetColumnSpan(this.rbPageScaleActualSize, 3);
			resources.ApplyResources(this.rbPageScaleActualSize, "rbPageScaleActualSize");
			this.rbPageScaleActualSize.Name = "rbPageScaleActualSize";
			this.rbPageScaleActualSize.Properties.AutoHeight = ((bool)(resources.GetObject("rbPageScaleActualSize.Properties.AutoHeight")));
			this.rbPageScaleActualSize.Properties.Caption = resources.GetString("rbPageScaleActualSize.Properties.Caption");
			this.rbPageScaleActualSize.Properties.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.Radio;
			this.rbPageScaleActualSize.Properties.RadioGroupIndex = 1;
			this.rbPageScaleActualSize.TabStop = false;
			resources.ApplyResources(this.rbPageScaleCustom, "rbPageScaleCustom");
			this.rbPageScaleCustom.Name = "rbPageScaleCustom";
			this.rbPageScaleCustom.Properties.AutoHeight = ((bool)(resources.GetObject("rbPageScaleCustom.Properties.AutoHeight")));
			this.rbPageScaleCustom.Properties.Caption = resources.GetString("rbPageScaleCustom.Properties.Caption");
			this.rbPageScaleCustom.Properties.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.Radio;
			this.rbPageScaleCustom.Properties.RadioGroupIndex = 1;
			this.rbPageScaleCustom.TabStop = false;
			resources.ApplyResources(this.teCustomScale, "teCustomScale");
			this.teCustomScale.Name = "teCustomScale";
			this.teCustomScale.Properties.AutoHeight = ((bool)(resources.GetObject("teCustomScale.Properties.AutoHeight")));
			this.teCustomScale.Properties.Mask.MaskType = ((DevExpress.XtraEditors.Mask.MaskType)(resources.GetObject("teCustomScale.Properties.Mask.MaskType")));
			resources.ApplyResources(this.lbPercent, "lbPercent");
			this.lbPercent.Name = "lbPercent";
			resources.ApplyResources(this.lbPageOrientation, "lbPageOrientation");
			this.lbPageOrientation.Name = "lbPageOrientation";
			this.rbAutoOrientation.AutoSizeInLayoutControl = true;
			this.tlpProperties.SetColumnSpan(this.rbAutoOrientation, 3);
			resources.ApplyResources(this.rbAutoOrientation, "rbAutoOrientation");
			this.rbAutoOrientation.Name = "rbAutoOrientation";
			this.rbAutoOrientation.Properties.AutoHeight = ((bool)(resources.GetObject("rbAutoOrientation.Properties.AutoHeight")));
			this.rbAutoOrientation.Properties.Caption = resources.GetString("rbAutoOrientation.Properties.Caption");
			this.rbAutoOrientation.Properties.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.Radio;
			this.rbAutoOrientation.Properties.RadioGroupIndex = 2;
			this.rbPortraitOrientation.AutoSizeInLayoutControl = true;
			this.tlpProperties.SetColumnSpan(this.rbPortraitOrientation, 3);
			resources.ApplyResources(this.rbPortraitOrientation, "rbPortraitOrientation");
			this.rbPortraitOrientation.Name = "rbPortraitOrientation";
			this.rbPortraitOrientation.Properties.AutoHeight = ((bool)(resources.GetObject("rbPortraitOrientation.Properties.AutoHeight")));
			this.rbPortraitOrientation.Properties.Caption = resources.GetString("rbPortraitOrientation.Properties.Caption");
			this.rbPortraitOrientation.Properties.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.Radio;
			this.rbPortraitOrientation.Properties.RadioGroupIndex = 2;
			this.rbPortraitOrientation.TabStop = false;
			this.rbLandscapeOrientation.AutoSizeInLayoutControl = true;
			this.tlpProperties.SetColumnSpan(this.rbLandscapeOrientation, 3);
			resources.ApplyResources(this.rbLandscapeOrientation, "rbLandscapeOrientation");
			this.rbLandscapeOrientation.Name = "rbLandscapeOrientation";
			this.rbLandscapeOrientation.Properties.AutoHeight = ((bool)(resources.GetObject("rbLandscapeOrientation.Properties.AutoHeight")));
			this.rbLandscapeOrientation.Properties.Caption = resources.GetString("rbLandscapeOrientation.Properties.Caption");
			this.rbLandscapeOrientation.Properties.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.Radio;
			this.rbLandscapeOrientation.Properties.RadioGroupIndex = 2;
			this.rbLandscapeOrientation.TabStop = false;
			resources.ApplyResources(this.lbPaperSource, "lbPaperSource");
			this.lbPaperSource.Name = "lbPaperSource";
			this.tlpProperties.SetColumnSpan(this.cbPaperSource, 2);
			resources.ApplyResources(this.cbPaperSource, "cbPaperSource");
			this.cbPaperSource.Name = "cbPaperSource";
			this.cbPaperSource.Properties.AutoHeight = ((bool)(resources.GetObject("cbPaperSource.Properties.AutoHeight")));
			this.cbPaperSource.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("cbPaperSource.Properties.Buttons"))))});
			this.cbPaperSource.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
			resources.ApplyResources(this.lbFilePath, "lbFilePath");
			this.lbFilePath.Name = "lbFilePath";
			this.cbPrintToFile.AutoSizeInLayoutControl = true;
			resources.ApplyResources(this.cbPrintToFile, "cbPrintToFile");
			this.cbPrintToFile.Name = "cbPrintToFile";
			this.cbPrintToFile.Properties.AutoHeight = ((bool)(resources.GetObject("cbPrintToFile.Properties.AutoHeight")));
			this.cbPrintToFile.Properties.AutoWidth = true;
			this.cbPrintToFile.Properties.Caption = resources.GetString("cbPrintToFile.Properties.Caption");
			this.tlpProperties.SetColumnSpan(this.beBrowse, 2);
			resources.ApplyResources(this.beBrowse, "beBrowse");
			this.beBrowse.Name = "beBrowse";
			this.beBrowse.Properties.AutoHeight = ((bool)(resources.GetObject("beBrowse.Properties.AutoHeight")));
			this.beBrowse.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton()});
			this.beBrowse.ButtonPressed += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.OnBrowseClick);
			resources.ApplyResources(this.panelMinWidth, "panelMinWidth");
			this.panelMinWidth.Name = "panelMinWidth";
			resources.ApplyResources(this.tlpButtons, "tlpButtons");
			this.tlpButtons.Controls.Add(this.btnPrint, 1, 1);
			this.tlpButtons.Controls.Add(this.btnCancel, 2, 1);
			this.tlpButtons.Name = "tlpButtons";
			resources.ApplyResources(this.btnPrint, "btnPrint");
			this.btnPrint.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.btnPrint.Name = "btnPrint";
			resources.ApplyResources(this.btnCancel, "btnCancel");
			this.btnCancel.CausesValidation = false;
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Name = "btnCancel";
			resources.ApplyResources(this.pePreviewImage, "pePreviewImage");
			this.pePreviewImage.Name = "pePreviewImage";
			this.pePreviewImage.Properties.Appearance.BackColor = ((System.Drawing.Color)(resources.GetObject("pePreviewImage.Properties.Appearance.BackColor")));
			this.pePreviewImage.Properties.Appearance.Options.UseBackColor = true;
			this.pePreviewImage.Properties.ShowMenu = false;
			this.pePreviewImage.Properties.ShowZoomSubMenu = DevExpress.Utils.DefaultBoolean.False;
			this.AcceptButton = this.btnPrint;
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.btnCancel;
			this.Controls.Add(this.pcOptions);
			this.Controls.Add(this.splitter);
			this.Controls.Add(this.pcPreviewImage);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "PdfPageSetupDialog";
			((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pcPreviewImage)).EndInit();
			this.pcPreviewImage.ResumeLayout(false);
			this.tlpPager.ResumeLayout(false);
			this.pcOptions.ResumeLayout(false);
			this.scrollableControl.ResumeLayout(false);
			this.scrollableControl.PerformLayout();
			this.tlpProperties.ResumeLayout(false);
			this.tlpProperties.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.icbInstalledPrinters.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.spPrintingDpi.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.spCopies.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.cbCollate.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.rbPrintRangeAll.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.rbPrintRangeCurrent.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.rbPrintRangeSome.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.tePageRange.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.rbPageScaleFit.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.rbPageScaleActualSize.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.rbPageScaleCustom.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.teCustomScale.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.rbAutoOrientation.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.rbPortraitOrientation.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.rbLandscapeOrientation.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.cbPaperSource.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.cbPrintToFile.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.beBrowse.Properties)).EndInit();
			this.tlpButtons.ResumeLayout(false);
			this.tlpButtons.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.pePreviewImage.Properties)).EndInit();
			this.ResumeLayout(false);
		}
		#endregion
		private XtraEditors.DXErrorProvider.DXErrorProvider errorProvider;
		private XtraEditors.PanelControl pcPreviewImage;
		private System.Windows.Forms.Splitter splitter;
		private System.Windows.Forms.Panel pcOptions;
		private System.Windows.Forms.TableLayoutPanel tlpButtons;
		private XtraEditors.SimpleButton btnPrint;
		private XtraEditors.SimpleButton btnCancel;
		private XtraEditors.XtraScrollableControl scrollableControl;
		private XtraEditors.LabelControl lbPaperSource;
		private XtraEditors.LabelControl lbFilePath;
		private XtraEditors.LabelControl lbDocumentsInQueue;
		private XtraEditors.LabelControl lbPrinterComment;
		private XtraEditors.LabelControl lbPrinterLocation;
		private XtraEditors.LabelControl lcPrinterStatus;
		private XtraEditors.SimpleButton btnPrinterPreferences;
		private XtraEditors.CheckEdit rbPortraitOrientation;
		private XtraEditors.CheckEdit rbLandscapeOrientation;
		private XtraEditors.ComboBoxEdit cbPaperSource;
		private XtraEditors.ButtonEdit beBrowse;
		private XtraEditors.CheckEdit cbPrintToFile;
		private System.Windows.Forms.TableLayoutPanel tlpPager;
		private XtraEditors.ControlNavigator cnPagePreviewNumber;
		private XtraEditors.LabelControl lbFitScale;
		private Native.PdfPreviewPictureEdit pePreviewImage;
		private System.Windows.Forms.TableLayoutPanel tlpProperties;
		private XtraEditors.LabelControl lcPrinterComment;
		private XtraEditors.LabelControl lbPrinterCommentCaption;
		private XtraEditors.LabelControl lcPrinterLocation;
		private XtraEditors.LabelControl lbPrinterStatus;
		private XtraEditors.LabelControl lbPrinterLocationCaption;
		private XtraEditors.LabelControl lbPrinterStatusCaption;
		private XtraEditors.ImageComboBoxEdit icbInstalledPrinters;
		private XtraEditors.LabelControl lbPrinterName;
		private XtraEditors.CheckEdit rbPageScaleFit;
		private XtraEditors.LabelControl lbPageSizing;
		private XtraEditors.TextEdit tePageRange;
		private XtraEditors.CheckEdit rbPrintRangeSome;
		private XtraEditors.LabelControl lbPageRangeExample;
		private XtraEditors.CheckEdit rbPrintRangeCurrent;
		private XtraEditors.CheckEdit rbPrintRangeAll;
		private XtraEditors.LabelControl lbPageRange;
		private XtraEditors.CheckEdit cbCollate;
		private XtraEditors.SpinEdit spCopies;
		private XtraEditors.LabelControl lbCopies;
		private XtraEditors.SpinEdit spPrintingDpi;
		private XtraEditors.LabelControl lbPrintingDpi;
		private XtraEditors.LabelControl lciPrinterDocumentsInQueue;
		private XtraEditors.LabelControl lbPrinterDocumentsInQueueCaption;
		private XtraEditors.LabelControl lbPageOrientation;
		private XtraEditors.LabelControl lbPercent;
		private XtraEditors.CheckEdit rbPageScaleCustom;
		private XtraEditors.CheckEdit rbPageScaleActualSize;
		private XtraEditors.TextEdit teCustomScale;
		private XtraEditors.CheckEdit rbAutoOrientation;
		private System.Windows.Forms.Panel panelMinWidth;
	}
}
