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
using System.Drawing.Printing;
using System.Globalization;
using System.Linq;
namespace DevExpress.XtraPrinting.Preview {
	using System.Drawing;
	using System.Windows.Forms;
	using DevExpress.DocumentView;
	using DevExpress.DocumentView.Controls;
	using DevExpress.XtraEditors;
	using DevExpress.XtraPrinting;
	using DevExpress.XtraPrinting.Native;
	using System.ComponentModel;
	using DevExpress.XtraLayout;
	public class PageSetupEditorForm : XtraForm {
		#region inner class
		class CustomDocumentViewer : DocumentViewerBase {
			PrintingSystemBase PrintingSystem {
				get {
					if(Document == null)
						Document = CreateDocument();
					return (PrintingSystemBase)Document;
				}
			}
			protected override bool CanChangePageMargins { get { return true; } }
			internal CustomDocumentViewer() {
				fMinZoom = 0.00001f;
				vScrollBar.Visible = bottomPanel.Visible = false;
				ViewControl.EnablePaintBackground = true;
				ViewControl.BackColor = Color.Empty;
			}
			public void Apply(PageData pageData) {
				PrintingSystem.PageSettings.Assign(pageData);
				ViewWholePage();
				Invalidate(true);
			}
			static PrintingSystemBase CreateDocument() {
				PrintingSystemBase ps = new PrintingSystemBase();
				ps.Begin();
				ps.Graph.Modifier = BrickModifier.Detail;
				EmptyBrick brick = new EmptyBrick() { Rect = new RectangleF(0, 0, 1, 1) };
				ps.Graph.DrawBrick(brick);
				ps.End();
				return ps;
			}
			protected override void Dispose(bool disposing) {
				if(disposing) {
					if(PrintingSystem != null) {
						PrintingSystem.Dispose();
						Document = null;
					}
				}
				base.Dispose(disposing);
			}
			protected override BackgroundPreviewPainter CreateBackgroundPreviewPainter() {
				return null;
			}
		}
		class LabelControlWithMetric : DevExpress.XtraEditors.LabelControl {
			[Localizable(true)]
			public string MarginsTextInInches {
				get;
				set;
			}
			[Localizable(true)]
			public string MarginsTextInMillimeters {
				get;
				set;
			}
		}
		#endregion
		#region fields
		bool isRTLChanged = false;
		private DevExpress.XtraLayout.LayoutControl layoutControl1;
		private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
		private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup2;
		private DevExpress.XtraEditors.PanelControl panelControl1;
		private DevExpress.XtraLayout.LayoutControlItem layoutControlItem1;
		private DevExpress.XtraLayout.LayoutControlGroup grpButtons;
		private DevExpress.XtraEditors.SimpleButton btnOK;
		private DevExpress.XtraEditors.SimpleButton btnCancel;
		private DevExpress.XtraLayout.LayoutControlItem layoutControlItem16;
		private DevExpress.XtraLayout.LayoutControlItem layoutControlItem17;
		private System.Drawing.Printing.PrinterSettings.PaperSizeCollection sizes;
		private System.ComponentModel.IContainer components = null;
		PageSettings pageSettings;
		private LabelControlWithMetric lbMargins;
		private XtraLayout.LayoutControlGroup layoutControlGroup4;
		private XtraLayout.LayoutControlItem layoutControlItem7;
		private LabelControl lbMarginsBottom;
		private LabelControl lbMarginsRight;
		private LabelControl lbMarginsTop;
		private LabelControl lbMarginsLeft;
		private XtraLayout.LayoutControlItem layoutControlItem8;
		private XtraLayout.LayoutControlItem layoutControlItem9;
		private XtraLayout.LayoutControlItem layoutControlItem10;
		private XtraLayout.LayoutControlItem layoutControlItem11;
		private TextEdit txtMarginsBottom;
		private TextEdit txtMarginsRight;
		private TextEdit txtMarginsTop;
		private TextEdit txtMarginsLeft;
		private XtraLayout.LayoutControlItem layoutControlItem12;
		private XtraLayout.LayoutControlItem layoutControlItem13;
		private XtraLayout.LayoutControlItem layoutControlItem14;
		private XtraLayout.LayoutControlItem layoutControlItem15;
		private LabelControl lbOrientation;
		private LabelControl lbPaperSize;
		private XtraLayout.LayoutControlGroup layoutControlGroup5;
		private XtraLayout.LayoutControlItem layoutControlItem2;
		private XtraLayout.LayoutControlItem layoutControlItem3;
		private CheckEdit rbOrientationLandscape;
		private CheckEdit rbOrientationPortrait;
		private ComboBoxEdit cbPaperSize;
		private XtraLayout.LayoutControlItem layoutControlItem4;
		private XtraLayout.LayoutControlItem layoutControlItem5;
		private XtraLayout.LayoutControlItem layoutControlItem6;
		private CustomDocumentViewer documentPreview;
		private LabelControl emptySpaceLabelControl3;
		private LabelControl fakeLabelControl1;
		private XtraLayout.LayoutControlItem layoutControlItem18;
		private XtraLayout.LayoutControlItem layoutControlItem19;
		private LabelControl emptySpaceLabelControl5;
		private LabelControl emptySpaceLabelControl4;
		private LabelControl emptySpaceLabelControl2;
		private LabelControl emptySpaceLabelControl1;
		private XtraLayout.LayoutControlItem layoutControlItem20;
		private XtraLayout.LayoutControlItem layoutControlItem21;
		private XtraLayout.LayoutControlItem layoutControlItem22;
		private XtraLayout.LayoutControlItem layoutControlItem23;
		PageData pageData;
		#endregion
		public PageSettings PageSettings {
			get { return pageSettings; }
			set {
				pageSettings = value;
				pageData = new PageData(value.Margins, new Margins(0, 0, 0, 0), value.PaperSize, value.Landscape);
				UpdateControls();
			}
		}
		System.Drawing.Printing.PrinterSettings.PaperSizeCollection PaperSizes {
			get {
				if(sizes == null) {
					sizes = new PrinterSettings.PaperSizeCollection(PageSettings.PrinterSettings.PaperSizes.Cast<PaperSize>().ToArray<PaperSize>());
					if(PageSettings.PaperSize.Kind == PaperKind.Custom)
						sizes.Add(PageSettings.PaperSize);
				}
				return sizes;
			}
		}
		bool IsMetric {
			get {
				return System.Globalization.RegionInfo.CurrentRegion.IsMetric;
			}
		}
		float MarginsUnit {
			get { return IsMetric ? GraphicsDpi.Millimeter : GraphicsDpi.Inch; }
		}
		public PageSetupEditorForm() {
			InitializeComponent();
			lbMargins.Text = IsMetric ? lbMargins.MarginsTextInMillimeters : lbMargins.MarginsTextInInches;
			string mask = String.Format(@"\d*[{0}]?\d*", CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator);
			txtMarginsTop.Properties.Mask.EditMask = mask;
			txtMarginsLeft.Properties.Mask.EditMask = mask;
			txtMarginsBottom.Properties.Mask.EditMask = mask;
			txtMarginsRight.Properties.Mask.EditMask = mask;
		}
		#region Windows Form Designer generated code
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PageSetupEditorForm));
			DevExpress.XtraLayout.ColumnDefinition columnDefinition8 = new DevExpress.XtraLayout.ColumnDefinition();
			DevExpress.XtraLayout.ColumnDefinition columnDefinition9 = new DevExpress.XtraLayout.ColumnDefinition();
			DevExpress.XtraLayout.ColumnDefinition columnDefinition10 = new DevExpress.XtraLayout.ColumnDefinition();
			DevExpress.XtraLayout.ColumnDefinition columnDefinition11 = new DevExpress.XtraLayout.ColumnDefinition();
			DevExpress.XtraLayout.ColumnDefinition columnDefinition12 = new DevExpress.XtraLayout.ColumnDefinition();
			DevExpress.XtraLayout.RowDefinition rowDefinition7 = new DevExpress.XtraLayout.RowDefinition();
			DevExpress.XtraLayout.RowDefinition rowDefinition8 = new DevExpress.XtraLayout.RowDefinition();
			DevExpress.XtraLayout.RowDefinition rowDefinition9 = new DevExpress.XtraLayout.RowDefinition();
			DevExpress.XtraLayout.RowDefinition rowDefinition10 = new DevExpress.XtraLayout.RowDefinition();
			DevExpress.XtraLayout.RowDefinition rowDefinition11 = new DevExpress.XtraLayout.RowDefinition();
			DevExpress.XtraLayout.RowDefinition rowDefinition12 = new DevExpress.XtraLayout.RowDefinition();
			DevExpress.XtraLayout.RowDefinition rowDefinition13 = new DevExpress.XtraLayout.RowDefinition();
			DevExpress.XtraLayout.ColumnDefinition columnDefinition1 = new DevExpress.XtraLayout.ColumnDefinition();
			DevExpress.XtraLayout.ColumnDefinition columnDefinition2 = new DevExpress.XtraLayout.ColumnDefinition();
			DevExpress.XtraLayout.ColumnDefinition columnDefinition3 = new DevExpress.XtraLayout.ColumnDefinition();
			DevExpress.XtraLayout.ColumnDefinition columnDefinition4 = new DevExpress.XtraLayout.ColumnDefinition();
			DevExpress.XtraLayout.ColumnDefinition columnDefinition5 = new DevExpress.XtraLayout.ColumnDefinition();
			DevExpress.XtraLayout.RowDefinition rowDefinition1 = new DevExpress.XtraLayout.RowDefinition();
			DevExpress.XtraLayout.RowDefinition rowDefinition2 = new DevExpress.XtraLayout.RowDefinition();
			DevExpress.XtraLayout.RowDefinition rowDefinition3 = new DevExpress.XtraLayout.RowDefinition();
			DevExpress.XtraLayout.ColumnDefinition columnDefinition6 = new DevExpress.XtraLayout.ColumnDefinition();
			DevExpress.XtraLayout.ColumnDefinition columnDefinition7 = new DevExpress.XtraLayout.ColumnDefinition();
			DevExpress.XtraLayout.RowDefinition rowDefinition4 = new DevExpress.XtraLayout.RowDefinition();
			DevExpress.XtraLayout.RowDefinition rowDefinition5 = new DevExpress.XtraLayout.RowDefinition();
			DevExpress.XtraLayout.RowDefinition rowDefinition6 = new DevExpress.XtraLayout.RowDefinition();
			DevExpress.XtraLayout.ColumnDefinition columnDefinition13 = new DevExpress.XtraLayout.ColumnDefinition();
			DevExpress.XtraLayout.ColumnDefinition columnDefinition14 = new DevExpress.XtraLayout.ColumnDefinition();
			DevExpress.XtraLayout.ColumnDefinition columnDefinition15 = new DevExpress.XtraLayout.ColumnDefinition();
			DevExpress.XtraLayout.ColumnDefinition columnDefinition16 = new DevExpress.XtraLayout.ColumnDefinition();
			DevExpress.XtraLayout.ColumnDefinition columnDefinition17 = new DevExpress.XtraLayout.ColumnDefinition();
			DevExpress.XtraLayout.RowDefinition rowDefinition14 = new DevExpress.XtraLayout.RowDefinition();
			DevExpress.XtraLayout.RowDefinition rowDefinition15 = new DevExpress.XtraLayout.RowDefinition();
			this.layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
			this.emptySpaceLabelControl5 = new DevExpress.XtraEditors.LabelControl();
			this.emptySpaceLabelControl4 = new DevExpress.XtraEditors.LabelControl();
			this.emptySpaceLabelControl2 = new DevExpress.XtraEditors.LabelControl();
			this.emptySpaceLabelControl1 = new DevExpress.XtraEditors.LabelControl();
			this.emptySpaceLabelControl3 = new DevExpress.XtraEditors.LabelControl();
			this.fakeLabelControl1 = new DevExpress.XtraEditors.LabelControl();
			this.rbOrientationLandscape = new DevExpress.XtraEditors.CheckEdit();
			this.rbOrientationPortrait = new DevExpress.XtraEditors.CheckEdit();
			this.cbPaperSize = new DevExpress.XtraEditors.ComboBoxEdit();
			this.lbOrientation = new DevExpress.XtraEditors.LabelControl();
			this.lbPaperSize = new DevExpress.XtraEditors.LabelControl();
			this.txtMarginsBottom = new DevExpress.XtraEditors.TextEdit();
			this.txtMarginsRight = new DevExpress.XtraEditors.TextEdit();
			this.txtMarginsTop = new DevExpress.XtraEditors.TextEdit();
			this.txtMarginsLeft = new DevExpress.XtraEditors.TextEdit();
			this.lbMarginsBottom = new DevExpress.XtraEditors.LabelControl();
			this.lbMarginsRight = new DevExpress.XtraEditors.LabelControl();
			this.lbMarginsTop = new DevExpress.XtraEditors.LabelControl();
			this.lbMarginsLeft = new DevExpress.XtraEditors.LabelControl();
			this.lbMargins = new DevExpress.XtraPrinting.Preview.PageSetupEditorForm.LabelControlWithMetric();
			this.btnOK = new DevExpress.XtraEditors.SimpleButton();
			this.btnCancel = new DevExpress.XtraEditors.SimpleButton();
			this.panelControl1 = new DevExpress.XtraEditors.PanelControl();
			this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
			this.layoutControlGroup2 = new DevExpress.XtraLayout.LayoutControlGroup();
			this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutControlGroup4 = new DevExpress.XtraLayout.LayoutControlGroup();
			this.layoutControlItem7 = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutControlItem8 = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutControlItem9 = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutControlItem10 = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutControlItem11 = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutControlItem12 = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutControlItem13 = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutControlItem14 = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutControlItem15 = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutControlGroup5 = new DevExpress.XtraLayout.LayoutControlGroup();
			this.layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutControlItem3 = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutControlItem4 = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutControlItem5 = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutControlItem6 = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutControlItem18 = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutControlItem19 = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutControlItem20 = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutControlItem21 = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutControlItem22 = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutControlItem23 = new DevExpress.XtraLayout.LayoutControlItem();
			this.grpButtons = new DevExpress.XtraLayout.LayoutControlGroup();
			this.layoutControlItem16 = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutControlItem17 = new DevExpress.XtraLayout.LayoutControlItem();
			this.documentPreview = new CustomDocumentViewer();
			((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
			this.layoutControl1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.rbOrientationLandscape.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.rbOrientationPortrait.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.cbPaperSize.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.txtMarginsBottom.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.txtMarginsRight.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.txtMarginsTop.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.txtMarginsLeft.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.panelControl1)).BeginInit();
			this.panelControl1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup2)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup4)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem7)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem8)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem9)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem10)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem11)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem12)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem13)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem14)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem15)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup5)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem4)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem5)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem6)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem18)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem19)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem20)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem21)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem22)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem23)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.grpButtons)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem16)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem17)).BeginInit();
			this.SuspendLayout();
			this.layoutControl1.AllowCustomization = false;
			this.layoutControl1.Controls.Add(this.emptySpaceLabelControl5);
			this.layoutControl1.Controls.Add(this.emptySpaceLabelControl4);
			this.layoutControl1.Controls.Add(this.emptySpaceLabelControl2);
			this.layoutControl1.Controls.Add(this.emptySpaceLabelControl1);
			this.layoutControl1.Controls.Add(this.emptySpaceLabelControl3);
			this.layoutControl1.Controls.Add(this.fakeLabelControl1);
			this.layoutControl1.Controls.Add(this.rbOrientationLandscape);
			this.layoutControl1.Controls.Add(this.rbOrientationPortrait);
			this.layoutControl1.Controls.Add(this.cbPaperSize);
			this.layoutControl1.Controls.Add(this.lbOrientation);
			this.layoutControl1.Controls.Add(this.lbPaperSize);
			this.layoutControl1.Controls.Add(this.txtMarginsBottom);
			this.layoutControl1.Controls.Add(this.txtMarginsRight);
			this.layoutControl1.Controls.Add(this.txtMarginsTop);
			this.layoutControl1.Controls.Add(this.txtMarginsLeft);
			this.layoutControl1.Controls.Add(this.lbMarginsBottom);
			this.layoutControl1.Controls.Add(this.lbMarginsRight);
			this.layoutControl1.Controls.Add(this.lbMarginsTop);
			this.layoutControl1.Controls.Add(this.lbMarginsLeft);
			this.layoutControl1.Controls.Add(this.lbMargins);
			this.layoutControl1.Controls.Add(this.btnOK);
			this.layoutControl1.Controls.Add(this.btnCancel);
			this.layoutControl1.Controls.Add(this.panelControl1);
			resources.ApplyResources(this.layoutControl1, "layoutControl1");
			this.layoutControl1.Name = "layoutControl1";
			this.layoutControl1.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(696, 124, 1021, 765);
			this.layoutControl1.Root = this.layoutControlGroup1;
			resources.ApplyResources(this.emptySpaceLabelControl5, "emptySpaceLabelControl5");
			this.emptySpaceLabelControl5.Name = "emptySpaceLabelControl5";
			this.emptySpaceLabelControl5.StyleController = this.layoutControl1;
			resources.ApplyResources(this.emptySpaceLabelControl4, "emptySpaceLabelControl4");
			this.emptySpaceLabelControl4.Name = "emptySpaceLabelControl4";
			this.emptySpaceLabelControl4.StyleController = this.layoutControl1;
			resources.ApplyResources(this.emptySpaceLabelControl2, "emptySpaceLabelControl2");
			this.emptySpaceLabelControl2.Name = "emptySpaceLabelControl2";
			this.emptySpaceLabelControl2.StyleController = this.layoutControl1;
			resources.ApplyResources(this.emptySpaceLabelControl1, "emptySpaceLabelControl1");
			this.emptySpaceLabelControl1.Name = "emptySpaceLabelControl1";
			this.emptySpaceLabelControl1.StyleController = this.layoutControl1;
			resources.ApplyResources(this.emptySpaceLabelControl3, "emptySpaceLabelControl3");
			this.emptySpaceLabelControl3.Name = "emptySpaceLabelControl3";
			this.emptySpaceLabelControl3.StyleController = this.layoutControl1;
			resources.ApplyResources(this.fakeLabelControl1, "fakeLabelControl1");
			this.fakeLabelControl1.Name = "fakeLabelControl1";
			this.fakeLabelControl1.StyleController = this.layoutControl1;
			resources.ApplyResources(this.rbOrientationLandscape, "rbOrientationLandscape");
			this.rbOrientationLandscape.Name = "rbOrientationLandscape";
			this.rbOrientationLandscape.Properties.Caption = resources.GetString("rbOrientationLandscape.Properties.Caption");
			this.rbOrientationLandscape.Properties.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.Radio;
			this.rbOrientationLandscape.Properties.RadioGroupIndex = 0;
			this.rbOrientationLandscape.StyleController = this.layoutControl1;
			this.rbOrientationLandscape.TabStop = false;
			resources.ApplyResources(this.rbOrientationPortrait, "rbOrientationPortrait");
			this.rbOrientationPortrait.Name = "rbOrientationPortrait";
			this.rbOrientationPortrait.Properties.Caption = resources.GetString("rbOrientationPortrait.Properties.Caption");
			this.rbOrientationPortrait.Properties.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.Radio;
			this.rbOrientationPortrait.Properties.RadioGroupIndex = 0;
			this.rbOrientationPortrait.StyleController = this.layoutControl1;
			resources.ApplyResources(this.cbPaperSize, "cbPaperSize");
			this.cbPaperSize.Name = "cbPaperSize";
			this.cbPaperSize.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("cbPaperSize.Properties.Buttons"))))});
			this.cbPaperSize.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
			this.cbPaperSize.StyleController = this.layoutControl1;
			resources.ApplyResources(this.lbOrientation, "lbOrientation");
			this.lbOrientation.Name = "lbOrientation";
			this.lbOrientation.StyleController = this.layoutControl1;
			resources.ApplyResources(this.lbPaperSize, "lbPaperSize");
			this.lbPaperSize.Name = "lbPaperSize";
			this.lbPaperSize.StyleController = this.layoutControl1;
			resources.ApplyResources(this.txtMarginsBottom, "txtMarginsBottom");
			this.txtMarginsBottom.Name = "txtMarginsBottom";
			this.txtMarginsBottom.StyleController = this.layoutControl1;
			resources.ApplyResources(this.txtMarginsRight, "txtMarginsRight");
			this.txtMarginsRight.Name = "txtMarginsRight";
			this.txtMarginsRight.StyleController = this.layoutControl1;
			resources.ApplyResources(this.txtMarginsTop, "txtMarginsTop");
			this.txtMarginsTop.Name = "txtMarginsTop";
			this.txtMarginsTop.StyleController = this.layoutControl1;
			resources.ApplyResources(this.txtMarginsLeft, "txtMarginsLeft");
			this.txtMarginsLeft.Name = "txtMarginsLeft";
			this.txtMarginsLeft.StyleController = this.layoutControl1;
			resources.ApplyResources(this.lbMarginsBottom, "lbMarginsBottom");
			this.lbMarginsBottom.Name = "lbMarginsBottom";
			this.lbMarginsBottom.StyleController = this.layoutControl1;
			resources.ApplyResources(this.lbMarginsRight, "lbMarginsRight");
			this.lbMarginsRight.Name = "lbMarginsRight";
			this.lbMarginsRight.StyleController = this.layoutControl1;
			resources.ApplyResources(this.lbMarginsTop, "lbMarginsTop");
			this.lbMarginsTop.Name = "lbMarginsTop";
			this.lbMarginsTop.StyleController = this.layoutControl1;
			resources.ApplyResources(this.lbMarginsLeft, "lbMarginsLeft");
			this.lbMarginsLeft.Name = "lbMarginsLeft";
			this.lbMarginsLeft.StyleController = this.layoutControl1;
			resources.ApplyResources(this.lbMargins, "lbMargins");
			this.lbMargins.Name = "lbMargins";
			this.lbMargins.StyleController = this.layoutControl1;
			resources.ApplyResources(this.btnOK, "btnOK");
			this.btnOK.Name = "btnOK";
			this.btnOK.StyleController = this.layoutControl1;
			this.btnOK.Click += new System.EventHandler(this.btOK_Click);
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			resources.ApplyResources(this.btnCancel, "btnCancel");
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.StyleController = this.layoutControl1;
			this.panelControl1.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.panelControl1.Controls.Add(this.documentPreview);
			resources.ApplyResources(this.panelControl1, "panelControl1");
			this.panelControl1.Name = "panelControl1";
			resources.ApplyResources(this.documentPreview, "documentPreview");
			this.documentPreview.Name = "documentPreview";
			this.documentPreview.TabStop = false;
			this.documentPreview.Zoom = 0.2907197F;
			this.documentPreview.DocumentChanged += new System.EventHandler(this.documentPreview_DocumentChanged);
			this.documentPreview.MouseWheel += new System.Windows.Forms.MouseEventHandler(this.documentPreview_MouseWheel);
			this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
			this.layoutControlGroup1.GroupBordersVisible = false;
			this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
			this.layoutControlGroup2,
			this.grpButtons});
			this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
			this.layoutControlGroup1.Name = "Root";
			this.layoutControlGroup1.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
			this.layoutControlGroup1.Size = new System.Drawing.Size(566, 322);
			this.layoutControlGroup1.TextVisible = false;
			this.layoutControlGroup2.GroupBordersVisible = false;
			this.layoutControlGroup2.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
			this.layoutControlItem1,
			this.layoutControlGroup4,
			this.layoutControlGroup5,
			this.layoutControlItem19,
			this.layoutControlItem20,
			this.layoutControlItem21,
			this.layoutControlItem22,
			this.layoutControlItem23});
			this.layoutControlGroup2.LayoutMode = DevExpress.XtraLayout.Utils.LayoutMode.Table;
			this.layoutControlGroup2.Location = new System.Drawing.Point(0, 0);
			this.layoutControlGroup2.Name = "layoutControlGroup2";
			this.layoutControlGroup2.OptionsTableLayoutGroup.AutoSizeDefaultDefinitionLength = 1;
			columnDefinition8.SizeType = System.Windows.Forms.SizeType.Absolute;
			columnDefinition8.Width = 23D;
			columnDefinition9.SizeType = System.Windows.Forms.SizeType.Percent;
			columnDefinition9.Width = 49.2600422832981D;
			columnDefinition10.SizeType = System.Windows.Forms.SizeType.Absolute;
			columnDefinition10.Width = 21D;
			columnDefinition11.SizeType = System.Windows.Forms.SizeType.Percent;
			columnDefinition11.Width = 50.7399577167019D;
			columnDefinition12.SizeType = System.Windows.Forms.SizeType.Absolute;
			columnDefinition12.Width = 49D;
			this.layoutControlGroup2.OptionsTableLayoutGroup.ColumnDefinitions.AddRange(new DevExpress.XtraLayout.ColumnDefinition[] {
			columnDefinition8,
			columnDefinition9,
			columnDefinition10,
			columnDefinition11,
			columnDefinition12});
			rowDefinition7.Height = 25D;
			rowDefinition7.SizeType = System.Windows.Forms.SizeType.AutoSize;
			rowDefinition8.Height = 37D;
			rowDefinition8.SizeType = System.Windows.Forms.SizeType.AutoSize;
			rowDefinition9.Height = 72D;
			rowDefinition9.SizeType = System.Windows.Forms.SizeType.AutoSize;
			rowDefinition10.Height = 17D;
			rowDefinition10.SizeType = System.Windows.Forms.SizeType.AutoSize;
			rowDefinition11.Height = 72D;
			rowDefinition11.SizeType = System.Windows.Forms.SizeType.AutoSize;
			rowDefinition12.Height = 37D;
			rowDefinition12.SizeType = System.Windows.Forms.SizeType.AutoSize;
			rowDefinition13.Height = 25D;
			rowDefinition13.SizeType = System.Windows.Forms.SizeType.AutoSize;
			this.layoutControlGroup2.OptionsTableLayoutGroup.RowDefinitions.AddRange(new DevExpress.XtraLayout.RowDefinition[] {
			rowDefinition7,
			rowDefinition8,
			rowDefinition9,
			rowDefinition10,
			rowDefinition11,
			rowDefinition12,
			rowDefinition13});
			this.layoutControlGroup2.Size = new System.Drawing.Size(566, 285);
			this.layoutControlItem1.Control = this.panelControl1;
			this.layoutControlItem1.Location = new System.Drawing.Point(23, 25);
			this.layoutControlItem1.Name = "layoutControlItem1";
			this.layoutControlItem1.OptionsTableLayoutItem.ColumnIndex = 1;
			this.layoutControlItem1.OptionsTableLayoutItem.RowIndex = 1;
			this.layoutControlItem1.OptionsTableLayoutItem.RowSpan = 5;
			this.layoutControlItem1.Size = new System.Drawing.Size(233, 235);
			this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem1.TextVisible = false;
			this.layoutControlGroup4.GroupBordersVisible = false;
			this.layoutControlGroup4.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
			this.layoutControlItem7,
			this.layoutControlItem8,
			this.layoutControlItem9,
			this.layoutControlItem10,
			this.layoutControlItem11,
			this.layoutControlItem12,
			this.layoutControlItem13,
			this.layoutControlItem14,
			this.layoutControlItem15});
			this.layoutControlGroup4.LayoutMode = DevExpress.XtraLayout.Utils.LayoutMode.Table;
			this.layoutControlGroup4.Location = new System.Drawing.Point(277, 151);
			this.layoutControlGroup4.Name = "layoutControlGroup4";
			columnDefinition1.SizeType = System.Windows.Forms.SizeType.AutoSize;
			columnDefinition1.Width = 36D;
			columnDefinition2.SizeType = System.Windows.Forms.SizeType.Percent;
			columnDefinition2.Width = 50D;
			columnDefinition3.SizeType = System.Windows.Forms.SizeType.Absolute;
			columnDefinition3.Width = 25D;
			columnDefinition4.SizeType = System.Windows.Forms.SizeType.AutoSize;
			columnDefinition4.Width = 51D;
			columnDefinition5.SizeType = System.Windows.Forms.SizeType.Percent;
			columnDefinition5.Width = 50D;
			this.layoutControlGroup4.OptionsTableLayoutGroup.ColumnDefinitions.AddRange(new DevExpress.XtraLayout.ColumnDefinition[] {
			columnDefinition1,
			columnDefinition2,
			columnDefinition3,
			columnDefinition4,
			columnDefinition5});
			rowDefinition1.Height = 24D;
			rowDefinition1.SizeType = System.Windows.Forms.SizeType.AutoSize;
			rowDefinition2.Height = 24D;
			rowDefinition2.SizeType = System.Windows.Forms.SizeType.AutoSize;
			rowDefinition3.Height = 24D;
			rowDefinition3.SizeType = System.Windows.Forms.SizeType.AutoSize;
			this.layoutControlGroup4.OptionsTableLayoutGroup.RowDefinitions.AddRange(new DevExpress.XtraLayout.RowDefinition[] {
			rowDefinition1,
			rowDefinition2,
			rowDefinition3});
			this.layoutControlGroup4.OptionsTableLayoutItem.ColumnIndex = 3;
			this.layoutControlGroup4.OptionsTableLayoutItem.RowIndex = 4;
			this.layoutControlGroup4.Size = new System.Drawing.Size(240, 72);
			this.layoutControlItem7.Control = this.lbMargins;
			this.layoutControlItem7.ControlAlignment = System.Drawing.ContentAlignment.MiddleLeft;
			this.layoutControlItem7.Location = new System.Drawing.Point(0, 0);
			this.layoutControlItem7.Name = "layoutControlItem7";
			this.layoutControlItem7.OptionsTableLayoutItem.ColumnSpan = 5;
			this.layoutControlItem7.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
			this.layoutControlItem7.Size = new System.Drawing.Size(240, 24);
			this.layoutControlItem7.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem7.TextVisible = false;
			this.layoutControlItem7.TrimClientAreaToControl = false;
			this.layoutControlItem8.Control = this.lbMarginsLeft;
			this.layoutControlItem8.ControlAlignment = System.Drawing.ContentAlignment.MiddleLeft;
			this.layoutControlItem8.Location = new System.Drawing.Point(0, 24);
			this.layoutControlItem8.Name = "layoutControlItem8";
			this.layoutControlItem8.OptionsTableLayoutItem.RowIndex = 1;
			this.layoutControlItem8.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
			this.layoutControlItem8.Size = new System.Drawing.Size(36, 24);
			this.layoutControlItem8.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem8.TextVisible = false;
			this.layoutControlItem8.TrimClientAreaToControl = false;
			this.layoutControlItem9.Control = this.lbMarginsTop;
			this.layoutControlItem9.ControlAlignment = System.Drawing.ContentAlignment.MiddleLeft;
			this.layoutControlItem9.Location = new System.Drawing.Point(0, 48);
			this.layoutControlItem9.Name = "layoutControlItem9";
			this.layoutControlItem9.OptionsTableLayoutItem.RowIndex = 2;
			this.layoutControlItem9.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
			this.layoutControlItem9.Size = new System.Drawing.Size(36, 24);
			this.layoutControlItem9.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem9.TextVisible = false;
			this.layoutControlItem9.TrimClientAreaToControl = false;
			this.layoutControlItem10.Control = this.lbMarginsRight;
			this.layoutControlItem10.ControlAlignment = System.Drawing.ContentAlignment.MiddleLeft;
			this.layoutControlItem10.Location = new System.Drawing.Point(125, 24);
			this.layoutControlItem10.Name = "layoutControlItem10";
			this.layoutControlItem10.OptionsTableLayoutItem.ColumnIndex = 3;
			this.layoutControlItem10.OptionsTableLayoutItem.RowIndex = 1;
			this.layoutControlItem10.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
			this.layoutControlItem10.Size = new System.Drawing.Size(51, 24);
			this.layoutControlItem10.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem10.TextVisible = false;
			this.layoutControlItem10.TrimClientAreaToControl = false;
			this.layoutControlItem11.Control = this.lbMarginsBottom;
			this.layoutControlItem11.ControlAlignment = System.Drawing.ContentAlignment.MiddleLeft;
			this.layoutControlItem11.Location = new System.Drawing.Point(125, 48);
			this.layoutControlItem11.Name = "layoutControlItem11";
			this.layoutControlItem11.OptionsTableLayoutItem.ColumnIndex = 3;
			this.layoutControlItem11.OptionsTableLayoutItem.RowIndex = 2;
			this.layoutControlItem11.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
			this.layoutControlItem11.Size = new System.Drawing.Size(51, 24);
			this.layoutControlItem11.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem11.TextVisible = false;
			this.layoutControlItem11.TrimClientAreaToControl = false;
			this.layoutControlItem12.Control = this.txtMarginsLeft;
			this.layoutControlItem12.ControlAlignment = System.Drawing.ContentAlignment.MiddleLeft;
			this.layoutControlItem12.Location = new System.Drawing.Point(36, 24);
			this.layoutControlItem12.Name = "layoutControlItem12";
			this.layoutControlItem12.OptionsTableLayoutItem.ColumnIndex = 1;
			this.layoutControlItem12.OptionsTableLayoutItem.RowIndex = 1;
			this.layoutControlItem12.Size = new System.Drawing.Size(64, 24);
			this.layoutControlItem12.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem12.TextVisible = false;
			this.layoutControlItem12.TrimClientAreaToControl = false;
			this.layoutControlItem13.Control = this.txtMarginsTop;
			this.layoutControlItem13.ControlAlignment = System.Drawing.ContentAlignment.MiddleLeft;
			this.layoutControlItem13.Location = new System.Drawing.Point(36, 48);
			this.layoutControlItem13.Name = "layoutControlItem13";
			this.layoutControlItem13.OptionsTableLayoutItem.ColumnIndex = 1;
			this.layoutControlItem13.OptionsTableLayoutItem.RowIndex = 2;
			this.layoutControlItem13.Size = new System.Drawing.Size(64, 24);
			this.layoutControlItem13.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem13.TextVisible = false;
			this.layoutControlItem13.TrimClientAreaToControl = false;
			this.layoutControlItem14.Control = this.txtMarginsRight;
			this.layoutControlItem14.ControlAlignment = System.Drawing.ContentAlignment.MiddleLeft;
			this.layoutControlItem14.Location = new System.Drawing.Point(176, 24);
			this.layoutControlItem14.Name = "layoutControlItem14";
			this.layoutControlItem14.OptionsTableLayoutItem.ColumnIndex = 4;
			this.layoutControlItem14.OptionsTableLayoutItem.RowIndex = 1;
			this.layoutControlItem14.Size = new System.Drawing.Size(64, 24);
			this.layoutControlItem14.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem14.TextVisible = false;
			this.layoutControlItem14.TrimClientAreaToControl = false;
			this.layoutControlItem15.Control = this.txtMarginsBottom;
			this.layoutControlItem15.ControlAlignment = System.Drawing.ContentAlignment.MiddleLeft;
			this.layoutControlItem15.Location = new System.Drawing.Point(176, 48);
			this.layoutControlItem15.Name = "layoutControlItem15";
			this.layoutControlItem15.OptionsTableLayoutItem.ColumnIndex = 4;
			this.layoutControlItem15.OptionsTableLayoutItem.RowIndex = 2;
			this.layoutControlItem15.Size = new System.Drawing.Size(64, 24);
			this.layoutControlItem15.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem15.TextVisible = false;
			this.layoutControlItem15.TrimClientAreaToControl = false;
			this.layoutControlGroup5.GroupBordersVisible = false;
			this.layoutControlGroup5.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
			this.layoutControlItem2,
			this.layoutControlItem3,
			this.layoutControlItem4,
			this.layoutControlItem5,
			this.layoutControlItem6,
			this.layoutControlItem18});
			this.layoutControlGroup5.LayoutMode = DevExpress.XtraLayout.Utils.LayoutMode.Table;
			this.layoutControlGroup5.Location = new System.Drawing.Point(277, 62);
			this.layoutControlGroup5.Name = "layoutControlGroup5";
			this.layoutControlGroup5.OptionsTableLayoutGroup.AutoSizeDefaultDefinitionLength = 1;
			columnDefinition6.SizeType = System.Windows.Forms.SizeType.AutoSize;
			columnDefinition6.Width = 71D;
			columnDefinition7.SizeType = System.Windows.Forms.SizeType.Percent;
			columnDefinition7.Width = 100D;
			this.layoutControlGroup5.OptionsTableLayoutGroup.ColumnDefinitions.AddRange(new DevExpress.XtraLayout.ColumnDefinition[] {
			columnDefinition6,
			columnDefinition7});
			rowDefinition4.Height = 24D;
			rowDefinition4.SizeType = System.Windows.Forms.SizeType.AutoSize;
			rowDefinition5.Height = 24D;
			rowDefinition5.SizeType = System.Windows.Forms.SizeType.AutoSize;
			rowDefinition6.Height = 24D;
			rowDefinition6.SizeType = System.Windows.Forms.SizeType.AutoSize;
			this.layoutControlGroup5.OptionsTableLayoutGroup.RowDefinitions.AddRange(new DevExpress.XtraLayout.RowDefinition[] {
			rowDefinition4,
			rowDefinition5,
			rowDefinition6});
			this.layoutControlGroup5.OptionsTableLayoutItem.ColumnIndex = 3;
			this.layoutControlGroup5.OptionsTableLayoutItem.RowIndex = 2;
			this.layoutControlGroup5.Size = new System.Drawing.Size(240, 72);
			this.layoutControlItem2.Control = this.lbPaperSize;
			this.layoutControlItem2.ControlAlignment = System.Drawing.ContentAlignment.MiddleLeft;
			this.layoutControlItem2.Location = new System.Drawing.Point(0, 0);
			this.layoutControlItem2.Name = "layoutControlItem2";
			this.layoutControlItem2.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
			this.layoutControlItem2.Size = new System.Drawing.Size(71, 24);
			this.layoutControlItem2.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem2.TextVisible = false;
			this.layoutControlItem2.TrimClientAreaToControl = false;
			this.layoutControlItem3.Control = this.lbOrientation;
			this.layoutControlItem3.ControlAlignment = System.Drawing.ContentAlignment.MiddleLeft;
			this.layoutControlItem3.Location = new System.Drawing.Point(0, 24);
			this.layoutControlItem3.Name = "layoutControlItem3";
			this.layoutControlItem3.OptionsTableLayoutItem.RowIndex = 1;
			this.layoutControlItem3.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
			this.layoutControlItem3.Size = new System.Drawing.Size(71, 24);
			this.layoutControlItem3.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem3.TextVisible = false;
			this.layoutControlItem3.TrimClientAreaToControl = false;
			this.layoutControlItem4.Control = this.cbPaperSize;
			this.layoutControlItem4.ControlAlignment = System.Drawing.ContentAlignment.MiddleLeft;
			this.layoutControlItem4.Location = new System.Drawing.Point(71, 0);
			this.layoutControlItem4.Name = "layoutControlItem4";
			this.layoutControlItem4.OptionsTableLayoutItem.ColumnIndex = 1;
			this.layoutControlItem4.Size = new System.Drawing.Size(169, 24);
			this.layoutControlItem4.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem4.TextVisible = false;
			this.layoutControlItem4.TrimClientAreaToControl = false;
			this.layoutControlItem5.Control = this.rbOrientationPortrait;
			this.layoutControlItem5.ControlAlignment = System.Drawing.ContentAlignment.MiddleLeft;
			this.layoutControlItem5.Location = new System.Drawing.Point(71, 24);
			this.layoutControlItem5.Name = "layoutControlItem5";
			this.layoutControlItem5.OptionsTableLayoutItem.ColumnIndex = 1;
			this.layoutControlItem5.OptionsTableLayoutItem.RowIndex = 1;
			this.layoutControlItem5.Size = new System.Drawing.Size(169, 24);
			this.layoutControlItem5.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem5.TextVisible = false;
			this.layoutControlItem5.TrimClientAreaToControl = false;
			this.layoutControlItem6.Control = this.rbOrientationLandscape;
			this.layoutControlItem6.ControlAlignment = System.Drawing.ContentAlignment.MiddleLeft;
			this.layoutControlItem6.Location = new System.Drawing.Point(71, 48);
			this.layoutControlItem6.Name = "layoutControlItem6";
			this.layoutControlItem6.OptionsTableLayoutItem.ColumnIndex = 1;
			this.layoutControlItem6.OptionsTableLayoutItem.RowIndex = 2;
			this.layoutControlItem6.Size = new System.Drawing.Size(169, 24);
			this.layoutControlItem6.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem6.TextVisible = false;
			this.layoutControlItem6.TrimClientAreaToControl = false;
			this.layoutControlItem18.Control = this.fakeLabelControl1;
			this.layoutControlItem18.Location = new System.Drawing.Point(0, 48);
			this.layoutControlItem18.Name = "layoutControlItem18";
			this.layoutControlItem18.OptionsTableLayoutItem.RowIndex = 2;
			this.layoutControlItem18.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
			this.layoutControlItem18.Size = new System.Drawing.Size(71, 24);
			this.layoutControlItem18.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem18.TextVisible = false;
			this.layoutControlItem19.Control = this.emptySpaceLabelControl3;
			this.layoutControlItem19.Location = new System.Drawing.Point(277, 134);
			this.layoutControlItem19.Name = "layoutControlItem19";
			this.layoutControlItem19.OptionsTableLayoutItem.ColumnIndex = 3;
			this.layoutControlItem19.OptionsTableLayoutItem.RowIndex = 3;
			this.layoutControlItem19.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
			this.layoutControlItem19.Size = new System.Drawing.Size(240, 17);
			this.layoutControlItem19.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem19.TextVisible = false;
			this.layoutControlItem20.Control = this.emptySpaceLabelControl1;
			this.layoutControlItem20.Location = new System.Drawing.Point(23, 0);
			this.layoutControlItem20.Name = "layoutControlItem20";
			this.layoutControlItem20.OptionsTableLayoutItem.ColumnIndex = 1;
			this.layoutControlItem20.OptionsTableLayoutItem.ColumnSpan = 3;
			this.layoutControlItem20.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
			this.layoutControlItem20.Size = new System.Drawing.Size(494, 25);
			this.layoutControlItem20.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem20.TextVisible = false;
			this.layoutControlItem21.Control = this.emptySpaceLabelControl2;
			this.layoutControlItem21.Location = new System.Drawing.Point(277, 25);
			this.layoutControlItem21.Name = "layoutControlItem21";
			this.layoutControlItem21.OptionsTableLayoutItem.ColumnIndex = 3;
			this.layoutControlItem21.OptionsTableLayoutItem.RowIndex = 1;
			this.layoutControlItem21.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
			this.layoutControlItem21.Size = new System.Drawing.Size(240, 37);
			this.layoutControlItem21.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem21.TextVisible = false;
			this.layoutControlItem22.Control = this.emptySpaceLabelControl4;
			this.layoutControlItem22.Location = new System.Drawing.Point(277, 223);
			this.layoutControlItem22.Name = "layoutControlItem22";
			this.layoutControlItem22.OptionsTableLayoutItem.ColumnIndex = 3;
			this.layoutControlItem22.OptionsTableLayoutItem.RowIndex = 5;
			this.layoutControlItem22.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
			this.layoutControlItem22.Size = new System.Drawing.Size(240, 37);
			this.layoutControlItem22.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem22.TextVisible = false;
			this.layoutControlItem23.Control = this.emptySpaceLabelControl5;
			this.layoutControlItem23.Location = new System.Drawing.Point(23, 260);
			this.layoutControlItem23.Name = "layoutControlItem23";
			this.layoutControlItem23.OptionsTableLayoutItem.ColumnIndex = 1;
			this.layoutControlItem23.OptionsTableLayoutItem.ColumnSpan = 3;
			this.layoutControlItem23.OptionsTableLayoutItem.RowIndex = 6;
			this.layoutControlItem23.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
			this.layoutControlItem23.Size = new System.Drawing.Size(494, 25);
			this.layoutControlItem23.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem23.TextVisible = false;
			this.grpButtons.GroupBordersVisible = false;
			this.grpButtons.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
			this.layoutControlItem16,
			this.layoutControlItem17});
			this.grpButtons.LayoutMode = DevExpress.XtraLayout.Utils.LayoutMode.Table;
			this.grpButtons.Location = new System.Drawing.Point(0, 285);
			this.grpButtons.Name = "grpButtons";
			this.grpButtons.OptionsTableLayoutGroup.AutoSizeDefaultDefinitionLength = 1;
			columnDefinition13.SizeType = System.Windows.Forms.SizeType.Percent;
			columnDefinition13.Width = 100D;
			columnDefinition14.SizeType = System.Windows.Forms.SizeType.Absolute;
			columnDefinition14.Width = 80D;
			columnDefinition15.SizeType = System.Windows.Forms.SizeType.Absolute;
			columnDefinition15.Width = 1D;
			columnDefinition16.SizeType = System.Windows.Forms.SizeType.Absolute;
			columnDefinition16.Width = 80D;
			columnDefinition17.SizeType = System.Windows.Forms.SizeType.Absolute;
			columnDefinition17.Width = 10D;
			this.grpButtons.OptionsTableLayoutGroup.ColumnDefinitions.AddRange(new DevExpress.XtraLayout.ColumnDefinition[] {
			columnDefinition13,
			columnDefinition14,
			columnDefinition15,
			columnDefinition16,
			columnDefinition17});
			rowDefinition14.Height = 26D;
			rowDefinition14.SizeType = System.Windows.Forms.SizeType.AutoSize;
			rowDefinition15.Height = 11D;
			rowDefinition15.SizeType = System.Windows.Forms.SizeType.Absolute;
			this.grpButtons.OptionsTableLayoutGroup.RowDefinitions.AddRange(new DevExpress.XtraLayout.RowDefinition[] {
			rowDefinition14,
			rowDefinition15});
			this.grpButtons.Size = new System.Drawing.Size(566, 37);
			this.layoutControlItem16.Control = this.btnCancel;
			this.layoutControlItem16.Location = new System.Drawing.Point(476, 0);
			this.layoutControlItem16.Name = "layoutControlItem16";
			this.layoutControlItem16.OptionsTableLayoutItem.ColumnIndex = 3;
			this.layoutControlItem16.Size = new System.Drawing.Size(80, 26);
			this.layoutControlItem16.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem16.TextVisible = false;
			this.layoutControlItem17.Control = this.btnOK;
			this.layoutControlItem17.Location = new System.Drawing.Point(395, 0);
			this.layoutControlItem17.Name = "layoutControlItem17";
			this.layoutControlItem17.OptionsTableLayoutItem.ColumnIndex = 1;
			this.layoutControlItem17.Size = new System.Drawing.Size(80, 26);
			this.layoutControlItem17.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem17.TextVisible = false;
			this.AcceptButton = this.btnOK;
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.btnCancel;
			this.Controls.Add(this.layoutControl1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "PageSetupEditorForm";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.Load += new System.EventHandler(this.PageSetupEditorForm_Load);
			this.Shown += new System.EventHandler(this.PageSetupEditorForm_Shown);
			((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
			this.layoutControl1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.rbOrientationLandscape.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.rbOrientationPortrait.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.cbPaperSize.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.txtMarginsBottom.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.txtMarginsRight.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.txtMarginsTop.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.txtMarginsLeft.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.panelControl1)).EndInit();
			this.panelControl1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup2)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup4)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem7)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem8)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem9)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem10)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem11)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem12)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem13)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem14)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem15)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup5)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem4)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem5)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem6)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem18)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem19)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem20)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem21)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem22)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem23)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.grpButtons)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem16)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem17)).EndInit();
			this.ResumeLayout(false);
		}
		#endregion
		void btOK_Click(object sender, EventArgs e) {
			PageSettings.Margins = pageData.Margins;
			PageSettings.Landscape = pageData.Landscape;
			PageSettings.PaperSize = GetPaperSize();
			DialogResult = DialogResult.OK;
		}
		void UpdateControls() {
			cbPaperSize.SelectedIndexChanged -= cbPaperSize_SelectedIndexChanged;
			rbOrientationLandscape.CheckedChanged -= rbOrientationLandscape_CheckedChanged;
			IEnumerable<string> paperNames = PaperSizes.Cast<PaperSize>().Select<PaperSize, string>(x => x.PaperName).OrderBy(x => x);
			cbPaperSize.Properties.Items.AddRange(paperNames.ToList<string>());
			cbPaperSize.SelectedItem = PageSettings.PaperSize.PaperName;
			rbOrientationLandscape.Checked = PageSettings.Landscape;
			cbPaperSize.SelectedIndexChanged += cbPaperSize_SelectedIndexChanged;
			rbOrientationLandscape.CheckedChanged += rbOrientationLandscape_CheckedChanged;
			UpdateMarginsView();
		}
		void UpdateMarginsView() {
			UnsubscribeMarginControls();
			if(pageData != null) {
				txtMarginsBottom.Text = CorrectValue(pageData.MarginsF.Bottom);
				txtMarginsTop.Text = CorrectValue(pageData.MarginsF.Top);
				txtMarginsRight.Text = CorrectValue(pageData.MarginsF.Right);
				txtMarginsLeft.Text = CorrectValue(pageData.MarginsF.Left);
			}
			SubscribeMarginControls();
		}
		protected override void Dispose(bool disposing) {
			if(disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		protected override void OnLoad(EventArgs e) {
			base.OnLoad(e);
			int minHeightLayoutControl = (layoutControl1 as DevExpress.Utils.Controls.IXtraResizableControl).MinSize.Height;
			if(minHeightLayoutControl > ClientSize.Height)
				ClientSize = new Size(ClientSize.Width, minHeightLayoutControl);
			if(pageData != null)
				UpdateDocumentViewer();
		}
		private void rbOrientationLandscape_CheckedChanged(object sender, EventArgs e) {
			pageData.Landscape = ((CheckEdit)sender).Checked;
			if(pageData.Landscape) {
				pageData.MarginsF = new MarginsF(pageData.MarginsF.Bottom, pageData.MarginsF.Top, pageData.MarginsF.Left, pageData.MarginsF.Right);
			} else {
				pageData.MarginsF = new MarginsF(pageData.MarginsF.Top, pageData.MarginsF.Bottom, pageData.MarginsF.Right, pageData.MarginsF.Left);
			}
			UpdateMarginsView();
			UpdateDocumentViewer();
		}
		PaperSize GetPaperSize() {
			return PaperSizes.Cast<PaperSize>().First<PaperSize>(size => size.PaperName == cbPaperSize.EditValue.ToString());
		}
		private void cbPaperSize_SelectedIndexChanged(object sender, EventArgs e) {
			PaperSize paperSize = GetPaperSize();
			SizeF paperSizeInDocument = GraphicsUnitConverter.Convert(new Size(paperSize.Width, paperSize.Height), GraphicsDpi.HundredthsOfAnInch, GraphicsDpi.Document);
			if(paperSizeInDocument.Height < (pageData.MarginsF.Top + pageData.MarginsF.Bottom) || paperSizeInDocument.Width < (pageData.MarginsF.Right + pageData.MarginsF.Left)) {
				float scale = Math.Min(paperSizeInDocument.Height / pageData.PageSize.Height, paperSizeInDocument.Width / pageData.PageSize.Width);
				pageData.MarginsF = GraphicsUnitConverter.Convert(pageData.MarginsF, 1, scale);
				UpdateMarginsView();
			}
			pageData.Size = new Size(paperSize.Width, paperSize.Height);
			UpdateDocumentViewer();
		}
		void UpdateDocumentViewer() {
			documentPreview.Apply(pageData);
		}
		private void documentPreview_MouseWheel(object sender, MouseEventArgs e) {
			cbPaperSize.Focus();
		}
		private void txtMargins_EditValueChanged(object sender, EventArgs e) {
			string value = ((TextEdit)sender).EditValue.ToString();
			float editValue;
			if(Single.TryParse(value, out editValue)) {
				editValue = GraphicsUnitConverter.Convert(editValue, MarginsUnit, GraphicsDpi.Document);
				if(ReferenceEquals(sender, txtMarginsLeft))
					pageData.MarginsF.Left = Math.Min(editValue, pageData.PageSize.Width - pageData.MarginsF.Right);
				if(ReferenceEquals(sender, txtMarginsTop))
					pageData.MarginsF.Top = Math.Min(editValue, pageData.PageSize.Height - pageData.MarginsF.Bottom);
				if(ReferenceEquals(sender, txtMarginsRight))
					pageData.MarginsF.Right = Math.Min(editValue, pageData.PageSize.Width - pageData.MarginsF.Left);
				if(ReferenceEquals(sender, txtMarginsBottom))
					pageData.MarginsF.Bottom = Math.Min(editValue, pageData.PageSize.Height - pageData.MarginsF.Top);
				UpdateDocumentViewer();
			}
		}
		private void txtMargins_LostFocus(object sender, EventArgs e) {
			string editValue = ((TextEdit)sender).EditValue.ToString();
			string correctEditValue = CorrectValue(editValue);
			if(correctEditValue != editValue)
				((TextEdit)sender).Text = correctEditValue;
		}
		private void documentPreview_DocumentChanged(object sender, EventArgs e) {
			UpdateMarginsView();
		}
		void SubscribeMarginControls() {
			txtMarginsBottom.EditValueChanged += new EventHandler(this.txtMargins_EditValueChanged);
			txtMarginsTop.EditValueChanged += new EventHandler(this.txtMargins_EditValueChanged);
			txtMarginsRight.EditValueChanged += new EventHandler(this.txtMargins_EditValueChanged);
			txtMarginsLeft.EditValueChanged += new EventHandler(this.txtMargins_EditValueChanged);
		}
		void UnsubscribeMarginControls() {
			txtMarginsBottom.EditValueChanged -= new EventHandler(this.txtMargins_EditValueChanged);
			txtMarginsTop.EditValueChanged -= new EventHandler(this.txtMargins_EditValueChanged);
			txtMarginsRight.EditValueChanged -= new EventHandler(this.txtMargins_EditValueChanged);
			txtMarginsLeft.EditValueChanged -= new EventHandler(this.txtMargins_EditValueChanged);
		}
		string CorrectValue(float value) {
			float convertValue = GraphicsUnitConverter.Convert(value, GraphicsDpi.Document, MarginsUnit);
			return CorrectValue(convertValue.ToString(MarginStringFormat(convertValue)));
		}
		string CorrectValue(string editValue) {
			float result;
			Single.TryParse(editValue, out result);
			return result.ToString();
		}
		string MarginStringFormat(float value) {
			float remainder = value % 1;
			if(remainder == 0)
				return "F0";
			if(remainder.ToString().Length > 5)
				return "F3";
			return string.Format("F{0}", remainder.ToString().Length - 2);
		}
		private void PageSetupEditorForm_Shown(object sender, EventArgs e) {
			this.btnOK.Focus();
		}
		protected override void OnRightToLeftChanged(EventArgs e) {
			base.OnRightToLeftChanged(e);
			isRTLChanged = true;
		}
		private void PageSetupEditorForm_Load(object sender, EventArgs e) {
			InitializeLayout();
		}
		void InitializeLayout() {
			InitializeGroupButtonsLayout();
			Size minLayoutControlSize = (layoutControl1 as DevExpress.Utils.Controls.IXtraResizableControl).MinSize;
			if(minLayoutControlSize.Width > ClientSize.Width || minLayoutControlSize.Height > ClientSize.Height)
				ClientSize = new Size(Math.Max(minLayoutControlSize.Width, ClientSize.Width), Math.Max(minLayoutControlSize.Height, ClientSize.Height));
			if(isRTLChanged)
				DevExpress.XtraPrinting.Native.RTLHelper.ConvertGroupControlAlignments(layoutControlGroup1);
		}
		void InitializeGroupButtonsLayout() {
			int btnOKBestWidth = btnOK.CalcBestSize().Width;
			int btnCancelBestWidth = btnCancel.CalcBestSize().Width;
			if(btnOKBestWidth <= btnOK.Width && btnCancelBestWidth <= btnCancel.Width)
				return;
			int btnPrintOKActualSize = Math.Max(btnOKBestWidth, btnCancelBestWidth);
			grpButtons.OptionsTableLayoutGroup.ColumnDefinitions[1].Width =
			grpButtons.OptionsTableLayoutGroup.ColumnDefinitions[3].Width = btnPrintOKActualSize + 2 + 2;
		}
	}
}
