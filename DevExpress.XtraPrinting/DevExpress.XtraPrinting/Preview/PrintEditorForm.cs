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

using DevExpress.Printing;
using DevExpress.Printing.Native;
using DevExpress.Printing.Native.PrintEditor;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Preview;
using DevExpress.XtraLayout;
using DevExpress.XtraPrinting.Localization;
using DevExpress.XtraPrinting.Native;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
namespace DevExpress.XtraPrinting.Preview {
	public class PrintEditorForm : XtraForm, IPrintForm {
		#region fields
		bool isRTLChanged = false; 
		private DevExpress.XtraLayout.LayoutControl layoutControl1;
		private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
		private DevExpress.XtraEditors.LabelControl lbPrinterDocumentsInQueueCaption;
		private DevExpress.XtraEditors.LabelControl lbPrinterCommentCaption;
		private DevExpress.XtraEditors.LabelControl lbPrinterLocationCaption;
		private DevExpress.XtraEditors.LabelControl lbPrinterStatusCaption;
		private DevExpress.XtraEditors.LabelControl lbPrinterName;
		private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup2;
		private DevExpress.XtraLayout.LayoutControlItem layoutControlItem1;
		private DevExpress.XtraLayout.LayoutControlItem layoutControlItem2;
		private DevExpress.XtraLayout.LayoutControlItem layoutControlItem3;
		private DevExpress.XtraLayout.LayoutControlItem layoutControlItem4;
		private DevExpress.XtraLayout.LayoutControlItem layoutControlItem5;
		private DevExpress.XtraEditors.SimpleButton btnPrinterPreferences;
		private DevExpress.XtraEditors.SpinEdit spCopies;
		private DevExpress.XtraEditors.ImageComboBoxEdit icbInstalledPrinters;
		private DevExpress.XtraEditors.LabelControl lbCopies;
		private DevExpress.XtraLayout.LayoutControlItem layoutControlItem6;
		private DevExpress.XtraLayout.LayoutControlItem layoutControlItem7;
		private DevExpress.XtraLayout.LayoutControlItem layoutControlItem8;
		private DevExpress.XtraLayout.LayoutControlItem layoutControlItem9;
		private DevExpress.XtraEditors.CheckEdit rbSomePages;
		private DevExpress.XtraEditors.CheckEdit rbSelection;
		private DevExpress.XtraEditors.CheckEdit rbCurrentPage;
		private DevExpress.XtraEditors.CheckEdit rbRangeAll;
		private DevExpress.XtraEditors.LabelControl lbPageRange;
		private DevExpress.XtraLayout.LayoutControlItem layoutControlItem10;
		private DevExpress.XtraLayout.LayoutControlItem layoutControlItem11;
		private DevExpress.XtraLayout.LayoutControlItem layoutControlItem12;
		private DevExpress.XtraLayout.LayoutControlItem layoutControlItem13;
		private DevExpress.XtraLayout.LayoutControlItem layoutControlItem14;
		private DevExpress.XtraEditors.TextEdit txtPageRange;
		private DevExpress.XtraEditors.CheckEdit chbPrintToFile;
		private DevExpress.XtraEditors.CheckEdit chbCollate;
		private DevExpress.XtraLayout.LayoutControlItem layoutControlItem15;
		private DevExpress.XtraLayout.LayoutControlItem layoutControlItem16;
		private DevExpress.XtraLayout.LayoutControlItem layoutControlItem17;
		private DevExpress.XtraEditors.ComboBoxEdit cbPaperSources;
		private DevExpress.XtraEditors.LabelControl lbPaperSource;
		private DevExpress.XtraLayout.LayoutControlItem layoutControlItem19;
		private DevExpress.XtraLayout.LayoutControlItem layoutControlItem20;
		private DevExpress.XtraEditors.LabelControl lbFilePath;
		private DevExpress.XtraLayout.LayoutControlItem layoutControlItem21;
		private DevExpress.XtraEditors.ButtonEdit btneBrowse;
		private DevExpress.XtraLayout.LayoutControlItem layoutControlItem22;
		private DevExpress.XtraEditors.SimpleButton btnCancel;
		private DevExpress.XtraEditors.SimpleButton btnPrint;
		private DevExpress.XtraLayout.LayoutControlGroup grpButtons;
		private DevExpress.XtraLayout.LayoutControlItem layoutControlItem23;
		private DevExpress.XtraLayout.LayoutControlItem layoutControlItem24;
		private DevExpress.XtraEditors.LabelControl lbPrinterDocumentsInQueue;
		private DevExpress.XtraEditors.LabelControl lbPrinterComment;
		private DevExpress.XtraEditors.LabelControl lbPrinterLocation;
		private DevExpress.XtraEditors.LabelControl lbPrinterStatus;
		private DevExpress.XtraLayout.LayoutControlItem layoutControlItem25;
		private DevExpress.XtraLayout.LayoutControlItem layoutControlItem26;
		private DevExpress.XtraLayout.LayoutControlItem layoutControlItem27;
		private DevExpress.XtraLayout.LayoutControlItem layoutControlItem28;
		private DevExpress.XtraEditors.LabelControl lbPrintToFile;
		private DevExpress.XtraLayout.LayoutControlItem layoutControlItem18;
		private DevExpress.XtraEditors.LabelControl lbPageRangeComment;
		private XtraLayout.LayoutControlItem layoutControlItem29;
		private System.ComponentModel.IContainer components = null;
		PrintEditorController controller;
		PrintRange printRange = PrintRange.AllPages;
		List<PrintRangeRadioButton> printRanges = new List<PrintRangeRadioButton>();
		PrinterImagesContainer imagesContainer = new PrinterImagesContainer();
		private LabelControl emptySpaceLabelControl4;
		private LabelControl emptySpaceLabelControl3;
		private LabelControl emptySpaceLabelControl2;
		private LabelControl emptySpaceLabelControl1;
		private XtraLayout.LayoutControlItem layoutControlItem30;
		private XtraLayout.LayoutControlItem layoutControlItem31;
		private XtraLayout.LayoutControlItem layoutControlItem32;
		private XtraLayout.LayoutControlItem layoutControlItem33;
		private LabelControl emptySpaceLabelControl6;
		private LabelControl emptySpaceLabelControl5;
		private XtraLayout.LayoutControlItem layoutControlItem34;
		private XtraLayout.LayoutControlItem layoutControlItem35;
		private LabelControl fakeLabelControl3;
		private LabelControl fakeLabelControl2;
		private LabelControl fakeLabelControl1;
		private XtraLayout.LayoutControlItem layoutControlItem36;
		private XtraLayout.LayoutControlItem layoutControlItem37;
		private XtraLayout.LayoutControlItem layoutControlItem38;
		private LabelControl emptySpaceLabelControl7;
		private XtraLayout.LayoutControlItem layoutControlItem39;
		PrinterItemContainer printerItemContainer = new PrinterItemContainer();
		#endregion
		#region inner class
		class PrintRangeRadioButton {
			CheckEdit editor;
			PrintRange printRange;
			public CheckEdit Editor { get { return editor; } }
			public PrintRange PrintRange { get { return printRange; } }
			public PrintRangeRadioButton(CheckEdit editor, PrintRange printRange) {
				this.editor = editor;
				this.printRange = printRange;
			}
		}
		#endregion
		public PrintDocument Document { get; set; }
		public string PrintFileName {
			get { return btneBrowse.Text; }
			set { btneBrowse.Text = value; }
		}
		public PrintRange PrintRange {
			get { return printRange; }
			private set { printRange = value; }
		}
		public bool AllowAllPages {
			get { return rbRangeAll.Enabled; }
			set { rbRangeAll.Enabled = value; }
		}
		public bool AllowSomePages {
			get { return rbSomePages.Enabled; }
			set { rbSomePages.Enabled = value; }
		}
		public bool AllowCurrentPage {
			get { return rbCurrentPage.Enabled; }
			set { rbCurrentPage.Enabled = value; }
		}
		public bool AllowSelection {
			get { return rbSelection.Enabled; }
			set { rbSelection.Enabled = value; }
		}
		public bool AllowPrintToFile {
			get { return chbPrintToFile.Enabled; }
			set { chbPrintToFile.Enabled = value; }
		}
		public string PageRangeText {
			get { return txtPageRange.Text; }
			set { txtPageRange.Text = value; }
		}
		public short Copies {
			get { return (short)spCopies.Value; }
			set { spCopies.Value = value; }
		}
		public bool Collate {
			get { return chbCollate.Checked; }
			set { chbCollate.Checked = value; }
		}
		public string PaperSource {
			get { return cbPaperSources.SelectedItem.ToString(); }
			set { cbPaperSources.SelectedItem = value; }
		}
		public bool PrintToFile {
			get { return chbPrintToFile.Checked; }
			set {
				chbPrintToFile.Checked = value;
				btneBrowse.Enabled = value;
			}
		}
		public string PrinterStatus { set { lbPrinterStatus.Text = value; } }
		public string PrinterLocation { set { lbPrinterLocation.Text = value; } }
		public string PrinterComment { set { lbPrinterComment.Text = value.Contains(Environment.NewLine) ? value.Replace(Environment.NewLine, " ") : value; } }
		public string PrinterDocumentsInQueue { set { lbPrinterDocumentsInQueue.Text = value; } }
		public PrintEditorForm() {
			InitializeComponent();
			controller = new PrintEditorController(this);
			icbInstalledPrinters.Properties.LargeImages = imagesContainer.LargeImages;
			icbInstalledPrinters.Properties.SmallImages = imagesContainer.SmallImages;
			InitPrintRangesList();
		}
		void UpdatePrinterInfo(PrinterItem item) {
			PrinterLocation = item.Location;
			PrinterComment = item.Comment;
			PrinterDocumentsInQueue = item.PrinterDocumentsInQueue;
			PrinterStatus = item.Status;
		}
		void UpdatePaperSources() {
			cbPaperSources.Properties.Items.Clear();
			cbPaperSources.Text = string.Empty;
			if(PrinterIsValid) {
				IEnumerable<string> paperSources = Document.PrinterSettings.PaperSources.Cast<PaperSource>().Select<PaperSource, string>(x => x.SourceName).OrderBy(x => x);
				cbPaperSources.Properties.Items.AddRange(paperSources.Where<string>(x => x != string.Empty).ToList());
				PaperSource source = Document.DefaultPageSettings.PaperSource;
				if(source != null && !string.IsNullOrEmpty(source.SourceName))
					PaperSource = source.SourceName;
			}
		}
		internal void AddPrinterItem(PrinterItem item) {
			icbInstalledPrinters.Properties.Items.Add(new DevExpress.XtraEditors.Controls.ImageComboBoxItem(item.DisplayName, imagesContainer.GetImageIndex(item)));
		}
		void IPrintForm.AddPrinterItem(PrinterItem item) {
			AddPrinterItem(item);
		}
		internal void SetSelectedPrinter(string printerName) {
			for(int i = 0; i < printerItemContainer.Items.Count; i++) {
				if(printerItemContainer.Items[i].FullName == printerName) {
					icbInstalledPrinters.SelectedIndex = i;
					break;
				}
			}
		}
		void IPrintForm.SetSelectedPrinter(string printerName) {
			SetSelectedPrinter(printerName);
		}
		protected override void Dispose(bool disposing) {
			if(disposing) {
				if(components != null) {
					components.Dispose();
					components = null;
				}
				if(printerItemContainer != null) {
					printerItemContainer.Dispose();
					printerItemContainer = null;
				}
				if(imagesContainer != null) {
					imagesContainer.Dispose();
					imagesContainer = null;
				}
			}
			base.Dispose(disposing);
		}
		#region Windows Form Designer generated code
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PrintEditorForm));
			DevExpress.XtraLayout.ColumnDefinition columnDefinition1 = new DevExpress.XtraLayout.ColumnDefinition();
			DevExpress.XtraLayout.ColumnDefinition columnDefinition2 = new DevExpress.XtraLayout.ColumnDefinition();
			DevExpress.XtraLayout.ColumnDefinition columnDefinition3 = new DevExpress.XtraLayout.ColumnDefinition();
			DevExpress.XtraLayout.ColumnDefinition columnDefinition4 = new DevExpress.XtraLayout.ColumnDefinition();
			DevExpress.XtraLayout.ColumnDefinition columnDefinition5 = new DevExpress.XtraLayout.ColumnDefinition();
			DevExpress.XtraLayout.ColumnDefinition columnDefinition6 = new DevExpress.XtraLayout.ColumnDefinition();
			DevExpress.XtraLayout.ColumnDefinition columnDefinition7 = new DevExpress.XtraLayout.ColumnDefinition();
			DevExpress.XtraLayout.ColumnDefinition columnDefinition8 = new DevExpress.XtraLayout.ColumnDefinition();
			DevExpress.XtraLayout.ColumnDefinition columnDefinition9 = new DevExpress.XtraLayout.ColumnDefinition();
			DevExpress.XtraLayout.RowDefinition rowDefinition1 = new DevExpress.XtraLayout.RowDefinition();
			DevExpress.XtraLayout.RowDefinition rowDefinition2 = new DevExpress.XtraLayout.RowDefinition();
			DevExpress.XtraLayout.RowDefinition rowDefinition3 = new DevExpress.XtraLayout.RowDefinition();
			DevExpress.XtraLayout.RowDefinition rowDefinition4 = new DevExpress.XtraLayout.RowDefinition();
			DevExpress.XtraLayout.RowDefinition rowDefinition5 = new DevExpress.XtraLayout.RowDefinition();
			DevExpress.XtraLayout.RowDefinition rowDefinition6 = new DevExpress.XtraLayout.RowDefinition();
			DevExpress.XtraLayout.RowDefinition rowDefinition7 = new DevExpress.XtraLayout.RowDefinition();
			DevExpress.XtraLayout.RowDefinition rowDefinition8 = new DevExpress.XtraLayout.RowDefinition();
			DevExpress.XtraLayout.RowDefinition rowDefinition9 = new DevExpress.XtraLayout.RowDefinition();
			DevExpress.XtraLayout.RowDefinition rowDefinition10 = new DevExpress.XtraLayout.RowDefinition();
			DevExpress.XtraLayout.RowDefinition rowDefinition11 = new DevExpress.XtraLayout.RowDefinition();
			DevExpress.XtraLayout.RowDefinition rowDefinition12 = new DevExpress.XtraLayout.RowDefinition();
			DevExpress.XtraLayout.RowDefinition rowDefinition13 = new DevExpress.XtraLayout.RowDefinition();
			DevExpress.XtraLayout.RowDefinition rowDefinition14 = new DevExpress.XtraLayout.RowDefinition();
			DevExpress.XtraLayout.RowDefinition rowDefinition15 = new DevExpress.XtraLayout.RowDefinition();
			DevExpress.XtraLayout.RowDefinition rowDefinition16 = new DevExpress.XtraLayout.RowDefinition();
			DevExpress.XtraLayout.RowDefinition rowDefinition17 = new DevExpress.XtraLayout.RowDefinition();
			DevExpress.XtraLayout.RowDefinition rowDefinition18 = new DevExpress.XtraLayout.RowDefinition();
			DevExpress.XtraLayout.RowDefinition rowDefinition19 = new DevExpress.XtraLayout.RowDefinition();
			DevExpress.XtraLayout.RowDefinition rowDefinition20 = new DevExpress.XtraLayout.RowDefinition();
			DevExpress.XtraLayout.ColumnDefinition columnDefinition10 = new DevExpress.XtraLayout.ColumnDefinition();
			DevExpress.XtraLayout.ColumnDefinition columnDefinition11 = new DevExpress.XtraLayout.ColumnDefinition();
			DevExpress.XtraLayout.ColumnDefinition columnDefinition12 = new DevExpress.XtraLayout.ColumnDefinition();
			DevExpress.XtraLayout.ColumnDefinition columnDefinition13 = new DevExpress.XtraLayout.ColumnDefinition();
			DevExpress.XtraLayout.ColumnDefinition columnDefinition14 = new DevExpress.XtraLayout.ColumnDefinition();
			DevExpress.XtraLayout.RowDefinition rowDefinition21 = new DevExpress.XtraLayout.RowDefinition();
			DevExpress.XtraLayout.RowDefinition rowDefinition22 = new DevExpress.XtraLayout.RowDefinition();
			this.layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
			this.emptySpaceLabelControl7 = new DevExpress.XtraEditors.LabelControl();
			this.fakeLabelControl3 = new DevExpress.XtraEditors.LabelControl();
			this.fakeLabelControl2 = new DevExpress.XtraEditors.LabelControl();
			this.fakeLabelControl1 = new DevExpress.XtraEditors.LabelControl();
			this.emptySpaceLabelControl6 = new DevExpress.XtraEditors.LabelControl();
			this.emptySpaceLabelControl5 = new DevExpress.XtraEditors.LabelControl();
			this.emptySpaceLabelControl4 = new DevExpress.XtraEditors.LabelControl();
			this.emptySpaceLabelControl3 = new DevExpress.XtraEditors.LabelControl();
			this.emptySpaceLabelControl2 = new DevExpress.XtraEditors.LabelControl();
			this.emptySpaceLabelControl1 = new DevExpress.XtraEditors.LabelControl();
			this.lbPageRangeComment = new DevExpress.XtraEditors.LabelControl();
			this.lbPrintToFile = new DevExpress.XtraEditors.LabelControl();
			this.lbPrinterDocumentsInQueue = new DevExpress.XtraEditors.LabelControl();
			this.lbPrinterComment = new DevExpress.XtraEditors.LabelControl();
			this.lbPrinterLocation = new DevExpress.XtraEditors.LabelControl();
			this.lbPrinterStatus = new DevExpress.XtraEditors.LabelControl();
			this.btnCancel = new DevExpress.XtraEditors.SimpleButton();
			this.btneBrowse = new DevExpress.XtraEditors.ButtonEdit();
			this.lbFilePath = new DevExpress.XtraEditors.LabelControl();
			this.cbPaperSources = new DevExpress.XtraEditors.ComboBoxEdit();
			this.lbPaperSource = new DevExpress.XtraEditors.LabelControl();
			this.txtPageRange = new DevExpress.XtraEditors.TextEdit();
			this.chbPrintToFile = new DevExpress.XtraEditors.CheckEdit();
			this.chbCollate = new DevExpress.XtraEditors.CheckEdit();
			this.rbSomePages = new DevExpress.XtraEditors.CheckEdit();
			this.rbSelection = new DevExpress.XtraEditors.CheckEdit();
			this.rbCurrentPage = new DevExpress.XtraEditors.CheckEdit();
			this.rbRangeAll = new DevExpress.XtraEditors.CheckEdit();
			this.lbPageRange = new DevExpress.XtraEditors.LabelControl();
			this.btnPrinterPreferences = new DevExpress.XtraEditors.SimpleButton();
			this.spCopies = new DevExpress.XtraEditors.SpinEdit();
			this.icbInstalledPrinters = new DevExpress.XtraEditors.ImageComboBoxEdit();
			this.lbCopies = new DevExpress.XtraEditors.LabelControl();
			this.lbPrinterDocumentsInQueueCaption = new DevExpress.XtraEditors.LabelControl();
			this.lbPrinterCommentCaption = new DevExpress.XtraEditors.LabelControl();
			this.lbPrinterLocationCaption = new DevExpress.XtraEditors.LabelControl();
			this.lbPrinterStatusCaption = new DevExpress.XtraEditors.LabelControl();
			this.lbPrinterName = new DevExpress.XtraEditors.LabelControl();
			this.btnPrint = new DevExpress.XtraEditors.SimpleButton();
			this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
			this.layoutControlGroup2 = new DevExpress.XtraLayout.LayoutControlGroup();
			this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutControlItem3 = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutControlItem4 = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutControlItem5 = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutControlItem6 = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutControlItem7 = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutControlItem8 = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutControlItem9 = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutControlItem10 = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutControlItem11 = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutControlItem12 = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutControlItem13 = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutControlItem14 = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutControlItem15 = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutControlItem16 = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutControlItem17 = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutControlItem19 = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutControlItem20 = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutControlItem21 = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutControlItem22 = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutControlItem25 = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutControlItem26 = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutControlItem27 = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutControlItem28 = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutControlItem18 = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutControlItem29 = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutControlItem30 = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutControlItem31 = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutControlItem32 = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutControlItem33 = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutControlItem34 = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutControlItem35 = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutControlItem36 = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutControlItem37 = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutControlItem38 = new DevExpress.XtraLayout.LayoutControlItem();
			this.grpButtons = new DevExpress.XtraLayout.LayoutControlGroup();
			this.layoutControlItem23 = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutControlItem24 = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutControlItem39 = new DevExpress.XtraLayout.LayoutControlItem();
			((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
			this.layoutControl1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.btneBrowse.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.cbPaperSources.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.txtPageRange.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.chbPrintToFile.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.chbCollate.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.rbSomePages.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.rbSelection.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.rbCurrentPage.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.rbRangeAll.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.spCopies.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.icbInstalledPrinters.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup2)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem4)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem5)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem6)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem7)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem8)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem9)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem10)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem11)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem12)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem13)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem14)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem15)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem16)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem17)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem19)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem20)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem21)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem22)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem25)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem26)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem27)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem28)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem18)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem29)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem30)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem31)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem32)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem33)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem34)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem35)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem36)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem37)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem38)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.grpButtons)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem23)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem24)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem39)).BeginInit();
			this.SuspendLayout();
			this.layoutControl1.AllowCustomization = false;
			this.layoutControl1.Controls.Add(this.emptySpaceLabelControl7);
			this.layoutControl1.Controls.Add(this.fakeLabelControl3);
			this.layoutControl1.Controls.Add(this.fakeLabelControl2);
			this.layoutControl1.Controls.Add(this.fakeLabelControl1);
			this.layoutControl1.Controls.Add(this.emptySpaceLabelControl6);
			this.layoutControl1.Controls.Add(this.emptySpaceLabelControl5);
			this.layoutControl1.Controls.Add(this.emptySpaceLabelControl4);
			this.layoutControl1.Controls.Add(this.emptySpaceLabelControl3);
			this.layoutControl1.Controls.Add(this.emptySpaceLabelControl2);
			this.layoutControl1.Controls.Add(this.emptySpaceLabelControl1);
			this.layoutControl1.Controls.Add(this.lbPageRangeComment);
			this.layoutControl1.Controls.Add(this.lbPrintToFile);
			this.layoutControl1.Controls.Add(this.lbPrinterDocumentsInQueue);
			this.layoutControl1.Controls.Add(this.lbPrinterComment);
			this.layoutControl1.Controls.Add(this.lbPrinterLocation);
			this.layoutControl1.Controls.Add(this.lbPrinterStatus);
			this.layoutControl1.Controls.Add(this.btnCancel);
			this.layoutControl1.Controls.Add(this.btneBrowse);
			this.layoutControl1.Controls.Add(this.lbFilePath);
			this.layoutControl1.Controls.Add(this.cbPaperSources);
			this.layoutControl1.Controls.Add(this.lbPaperSource);
			this.layoutControl1.Controls.Add(this.txtPageRange);
			this.layoutControl1.Controls.Add(this.chbPrintToFile);
			this.layoutControl1.Controls.Add(this.chbCollate);
			this.layoutControl1.Controls.Add(this.rbSomePages);
			this.layoutControl1.Controls.Add(this.rbSelection);
			this.layoutControl1.Controls.Add(this.rbCurrentPage);
			this.layoutControl1.Controls.Add(this.rbRangeAll);
			this.layoutControl1.Controls.Add(this.lbPageRange);
			this.layoutControl1.Controls.Add(this.btnPrinterPreferences);
			this.layoutControl1.Controls.Add(this.spCopies);
			this.layoutControl1.Controls.Add(this.icbInstalledPrinters);
			this.layoutControl1.Controls.Add(this.lbCopies);
			this.layoutControl1.Controls.Add(this.lbPrinterDocumentsInQueueCaption);
			this.layoutControl1.Controls.Add(this.lbPrinterCommentCaption);
			this.layoutControl1.Controls.Add(this.lbPrinterLocationCaption);
			this.layoutControl1.Controls.Add(this.lbPrinterStatusCaption);
			this.layoutControl1.Controls.Add(this.lbPrinterName);
			this.layoutControl1.Controls.Add(this.btnPrint);
			resources.ApplyResources(this.layoutControl1, "layoutControl1");
			this.layoutControl1.Name = "layoutControl1";
			this.layoutControl1.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(777, 27, 929, 894);
			this.layoutControl1.Root = this.layoutControlGroup1;
			resources.ApplyResources(this.emptySpaceLabelControl7, "emptySpaceLabelControl7");
			this.emptySpaceLabelControl7.Name = "emptySpaceLabelControl7";
			this.emptySpaceLabelControl7.StyleController = this.layoutControl1;
			resources.ApplyResources(this.fakeLabelControl3, "fakeLabelControl3");
			this.fakeLabelControl3.Name = "fakeLabelControl3";
			this.fakeLabelControl3.StyleController = this.layoutControl1;
			resources.ApplyResources(this.fakeLabelControl2, "fakeLabelControl2");
			this.fakeLabelControl2.Name = "fakeLabelControl2";
			this.fakeLabelControl2.StyleController = this.layoutControl1;
			resources.ApplyResources(this.fakeLabelControl1, "fakeLabelControl1");
			this.fakeLabelControl1.Name = "fakeLabelControl1";
			this.fakeLabelControl1.StyleController = this.layoutControl1;
			resources.ApplyResources(this.emptySpaceLabelControl6, "emptySpaceLabelControl6");
			this.emptySpaceLabelControl6.Name = "emptySpaceLabelControl6";
			this.emptySpaceLabelControl6.StyleController = this.layoutControl1;
			resources.ApplyResources(this.emptySpaceLabelControl5, "emptySpaceLabelControl5");
			this.emptySpaceLabelControl5.Name = "emptySpaceLabelControl5";
			this.emptySpaceLabelControl5.StyleController = this.layoutControl1;
			resources.ApplyResources(this.emptySpaceLabelControl4, "emptySpaceLabelControl4");
			this.emptySpaceLabelControl4.Name = "emptySpaceLabelControl4";
			this.emptySpaceLabelControl4.StyleController = this.layoutControl1;
			resources.ApplyResources(this.emptySpaceLabelControl3, "emptySpaceLabelControl3");
			this.emptySpaceLabelControl3.Name = "emptySpaceLabelControl3";
			this.emptySpaceLabelControl3.StyleController = this.layoutControl1;
			resources.ApplyResources(this.emptySpaceLabelControl2, "emptySpaceLabelControl2");
			this.emptySpaceLabelControl2.Name = "emptySpaceLabelControl2";
			this.emptySpaceLabelControl2.StyleController = this.layoutControl1;
			resources.ApplyResources(this.emptySpaceLabelControl1, "emptySpaceLabelControl1");
			this.emptySpaceLabelControl1.Name = "emptySpaceLabelControl1";
			this.emptySpaceLabelControl1.StyleController = this.layoutControl1;
			resources.ApplyResources(this.lbPageRangeComment, "lbPageRangeComment");
			this.lbPageRangeComment.Name = "lbPageRangeComment";
			this.lbPageRangeComment.StyleController = this.layoutControl1;
			resources.ApplyResources(this.lbPrintToFile, "lbPrintToFile");
			this.lbPrintToFile.Name = "lbPrintToFile";
			this.lbPrintToFile.StyleController = this.layoutControl1;
			resources.ApplyResources(this.lbPrinterDocumentsInQueue, "lbPrinterDocumentsInQueue");
			this.lbPrinterDocumentsInQueue.Name = "lbPrinterDocumentsInQueue";
			this.lbPrinterDocumentsInQueue.StyleController = this.layoutControl1;
			this.lbPrinterComment.Appearance.TextOptions.Trimming = DevExpress.Utils.Trimming.EllipsisCharacter;
			resources.ApplyResources(this.lbPrinterComment, "lbPrinterComment");
			this.lbPrinterComment.Name = "lbPrinterComment";
			this.lbPrinterComment.StyleController = this.layoutControl1;
			resources.ApplyResources(this.lbPrinterLocation, "lbPrinterLocation");
			this.lbPrinterLocation.Name = "lbPrinterLocation";
			this.lbPrinterLocation.StyleController = this.layoutControl1;
			resources.ApplyResources(this.lbPrinterStatus, "lbPrinterStatus");
			this.lbPrinterStatus.Name = "lbPrinterStatus";
			this.lbPrinterStatus.StyleController = this.layoutControl1;
			this.btnCancel.CausesValidation = false;
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			resources.ApplyResources(this.btnCancel, "btnCancel");
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.StyleController = this.layoutControl1;
			this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
			resources.ApplyResources(this.btneBrowse, "btneBrowse");
			this.btneBrowse.Name = "btneBrowse";
			this.btneBrowse.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton()});
			this.btneBrowse.StyleController = this.layoutControl1;
			this.btneBrowse.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.btneBrowse_ButtonClick);
			resources.ApplyResources(this.lbFilePath, "lbFilePath");
			this.lbFilePath.Name = "lbFilePath";
			this.lbFilePath.StyleController = this.layoutControl1;
			resources.ApplyResources(this.cbPaperSources, "cbPaperSources");
			this.cbPaperSources.Name = "cbPaperSources";
			this.cbPaperSources.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("cbPaperSources.Properties.Buttons"))))});
			this.cbPaperSources.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
			this.cbPaperSources.StyleController = this.layoutControl1;
			resources.ApplyResources(this.lbPaperSource, "lbPaperSource");
			this.lbPaperSource.Name = "lbPaperSource";
			this.lbPaperSource.StyleController = this.layoutControl1;
			resources.ApplyResources(this.txtPageRange, "txtPageRange");
			this.txtPageRange.Name = "txtPageRange";
			this.txtPageRange.Properties.Mask.EditMask = resources.GetString("txtPageRange.Properties.Mask.EditMask");
			this.txtPageRange.Properties.Mask.MaskType = ((DevExpress.XtraEditors.Mask.MaskType)(resources.GetObject("txtPageRange.Properties.Mask.MaskType")));
			this.txtPageRange.Properties.Mask.ShowPlaceHolders = ((bool)(resources.GetObject("txtPageRange.Properties.Mask.ShowPlaceHolders")));
			this.txtPageRange.StyleController = this.layoutControl1;
			resources.ApplyResources(this.chbPrintToFile, "chbPrintToFile");
			this.chbPrintToFile.Name = "chbPrintToFile";
			this.chbPrintToFile.Properties.Caption = resources.GetString("chbPrintToFile.Properties.Caption");
			this.chbPrintToFile.StyleController = this.layoutControl1;
			this.chbPrintToFile.CheckedChanged += new System.EventHandler(this.chbPrintToFile_CheckedChanged);
			resources.ApplyResources(this.chbCollate, "chbCollate");
			this.chbCollate.Name = "chbCollate";
			this.chbCollate.Properties.Caption = resources.GetString("chbCollate.Properties.Caption");
			this.chbCollate.StyleController = this.layoutControl1;
			this.rbSomePages.AutoSizeInLayoutControl = true;
			resources.ApplyResources(this.rbSomePages, "rbSomePages");
			this.rbSomePages.Name = "rbSomePages";
			this.rbSomePages.Properties.Caption = resources.GetString("rbSomePages.Properties.Caption");
			this.rbSomePages.Properties.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.Radio;
			this.rbSomePages.Properties.RadioGroupIndex = 0;
			this.rbSomePages.StyleController = this.layoutControl1;
			this.rbSomePages.TabStop = false;
			this.rbSomePages.CheckedChanged += new System.EventHandler(this.rbPageRange_CheckedChanged);
			resources.ApplyResources(this.rbSelection, "rbSelection");
			this.rbSelection.Name = "rbSelection";
			this.rbSelection.Properties.Caption = resources.GetString("rbSelection.Properties.Caption");
			this.rbSelection.Properties.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.Radio;
			this.rbSelection.Properties.RadioGroupIndex = 0;
			this.rbSelection.StyleController = this.layoutControl1;
			this.rbSelection.TabStop = false;
			this.rbSelection.CheckedChanged += new System.EventHandler(this.rbPageRange_CheckedChanged);
			resources.ApplyResources(this.rbCurrentPage, "rbCurrentPage");
			this.rbCurrentPage.Name = "rbCurrentPage";
			this.rbCurrentPage.Properties.Caption = resources.GetString("rbCurrentPage.Properties.Caption");
			this.rbCurrentPage.Properties.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.Radio;
			this.rbCurrentPage.Properties.RadioGroupIndex = 0;
			this.rbCurrentPage.StyleController = this.layoutControl1;
			this.rbCurrentPage.TabStop = false;
			this.rbCurrentPage.CheckedChanged += new System.EventHandler(this.rbPageRange_CheckedChanged);
			resources.ApplyResources(this.rbRangeAll, "rbRangeAll");
			this.rbRangeAll.Name = "rbRangeAll";
			this.rbRangeAll.Properties.Caption = resources.GetString("rbRangeAll.Properties.Caption");
			this.rbRangeAll.Properties.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.Radio;
			this.rbRangeAll.Properties.RadioGroupIndex = 0;
			this.rbRangeAll.StyleController = this.layoutControl1;
			this.rbRangeAll.CheckedChanged += new System.EventHandler(this.rbPageRange_CheckedChanged);
			resources.ApplyResources(this.lbPageRange, "lbPageRange");
			this.lbPageRange.Name = "lbPageRange";
			this.lbPageRange.StyleController = this.layoutControl1;
			resources.ApplyResources(this.btnPrinterPreferences, "btnPrinterPreferences");
			this.btnPrinterPreferences.Name = "btnPrinterPreferences";
			this.btnPrinterPreferences.StyleController = this.layoutControl1;
			this.btnPrinterPreferences.Click += new System.EventHandler(this.btnPrinterPreferences_Click);
			resources.ApplyResources(this.spCopies, "spCopies");
			this.spCopies.Name = "spCopies";
			this.spCopies.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("spCopies.Properties.Buttons"))))});
			this.spCopies.Properties.Mask.EditMask = resources.GetString("spCopies.Properties.Mask.EditMask");
			this.spCopies.Properties.Mask.MaskType = ((DevExpress.XtraEditors.Mask.MaskType)(resources.GetObject("spCopies.Properties.Mask.MaskType")));
			this.spCopies.Properties.Mask.ShowPlaceHolders = ((bool)(resources.GetObject("spCopies.Properties.Mask.ShowPlaceHolders")));
			this.spCopies.Properties.MaxValue = new decimal(new int[] {
			-1,
			-1,
			-1,
			0});
			this.spCopies.Properties.MinValue = new decimal(new int[] {
			1,
			0,
			0,
			0});
			this.spCopies.StyleController = this.layoutControl1;
			this.spCopies.InvalidValue += new DevExpress.XtraEditors.Controls.InvalidValueExceptionEventHandler(this.spCopies_InvalidValue);
			this.spCopies.EditValueChanged += new System.EventHandler(this.spCopies_ValueChanged);
			this.spCopies.Validating += new System.ComponentModel.CancelEventHandler(this.spCopies_Validating);
			resources.ApplyResources(this.icbInstalledPrinters, "icbInstalledPrinters");
			this.icbInstalledPrinters.Name = "icbInstalledPrinters";
			this.icbInstalledPrinters.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("icbInstalledPrinters.Properties.Buttons"))))});
			this.icbInstalledPrinters.Properties.DropDownItemHeight = 46;
			this.icbInstalledPrinters.Properties.DropDownRows = 6;
			this.icbInstalledPrinters.StyleController = this.layoutControl1;
			resources.ApplyResources(this.lbCopies, "lbCopies");
			this.lbCopies.Name = "lbCopies";
			this.lbCopies.StyleController = this.layoutControl1;
			resources.ApplyResources(this.lbPrinterDocumentsInQueueCaption, "lbPrinterDocumentsInQueueCaption");
			this.lbPrinterDocumentsInQueueCaption.Name = "lbPrinterDocumentsInQueueCaption";
			this.lbPrinterDocumentsInQueueCaption.StyleController = this.layoutControl1;
			resources.ApplyResources(this.lbPrinterCommentCaption, "lbPrinterCommentCaption");
			this.lbPrinterCommentCaption.Name = "lbPrinterCommentCaption";
			this.lbPrinterCommentCaption.StyleController = this.layoutControl1;
			resources.ApplyResources(this.lbPrinterLocationCaption, "lbPrinterLocationCaption");
			this.lbPrinterLocationCaption.Name = "lbPrinterLocationCaption";
			this.lbPrinterLocationCaption.StyleController = this.layoutControl1;
			resources.ApplyResources(this.lbPrinterStatusCaption, "lbPrinterStatusCaption");
			this.lbPrinterStatusCaption.Name = "lbPrinterStatusCaption";
			this.lbPrinterStatusCaption.StyleController = this.layoutControl1;
			resources.ApplyResources(this.lbPrinterName, "lbPrinterName");
			this.lbPrinterName.Name = "lbPrinterName";
			this.lbPrinterName.StyleController = this.layoutControl1;
			resources.ApplyResources(this.btnPrint, "btnPrint");
			this.btnPrint.Name = "btnPrint";
			this.btnPrint.StyleController = this.layoutControl1;
			this.btnPrint.Click += new System.EventHandler(this.btnPrint_Click);
			this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
			this.layoutControlGroup1.GroupBordersVisible = false;
			this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
			this.layoutControlGroup2,
			this.grpButtons});
			this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
			this.layoutControlGroup1.Name = "Root";
			this.layoutControlGroup1.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
			this.layoutControlGroup1.Size = new System.Drawing.Size(481, 471);
			this.layoutControlGroup1.TextVisible = false;
			this.layoutControlGroup2.GroupBordersVisible = false;
			this.layoutControlGroup2.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
			this.layoutControlItem1,
			this.layoutControlItem2,
			this.layoutControlItem3,
			this.layoutControlItem4,
			this.layoutControlItem5,
			this.layoutControlItem6,
			this.layoutControlItem7,
			this.layoutControlItem8,
			this.layoutControlItem9,
			this.layoutControlItem10,
			this.layoutControlItem11,
			this.layoutControlItem12,
			this.layoutControlItem13,
			this.layoutControlItem14,
			this.layoutControlItem15,
			this.layoutControlItem16,
			this.layoutControlItem17,
			this.layoutControlItem19,
			this.layoutControlItem20,
			this.layoutControlItem21,
			this.layoutControlItem22,
			this.layoutControlItem25,
			this.layoutControlItem26,
			this.layoutControlItem27,
			this.layoutControlItem28,
			this.layoutControlItem18,
			this.layoutControlItem29,
			this.layoutControlItem30,
			this.layoutControlItem31,
			this.layoutControlItem32,
			this.layoutControlItem33,
			this.layoutControlItem34,
			this.layoutControlItem35,
			this.layoutControlItem36,
			this.layoutControlItem37,
			this.layoutControlItem38});
			this.layoutControlGroup2.LayoutMode = DevExpress.XtraLayout.Utils.LayoutMode.Table;
			this.layoutControlGroup2.Location = new System.Drawing.Point(0, 0);
			this.layoutControlGroup2.Name = "layoutControlGroup2";
			this.layoutControlGroup2.OptionsTableLayoutGroup.AutoSizeDefaultDefinitionLength = 1;
			columnDefinition1.SizeType = System.Windows.Forms.SizeType.Absolute;
			columnDefinition1.Width = 18D;
			columnDefinition2.SizeType = System.Windows.Forms.SizeType.AutoSize;
			columnDefinition2.Width = 119D;
			columnDefinition3.SizeType = System.Windows.Forms.SizeType.Percent;
			columnDefinition3.Width = 1.1494252873563218D;
			columnDefinition4.SizeType = System.Windows.Forms.SizeType.AutoSize;
			columnDefinition4.Width = 59D;
			columnDefinition5.SizeType = System.Windows.Forms.SizeType.Percent;
			columnDefinition5.Width = 5.74712643678161D;
			columnDefinition6.SizeType = System.Windows.Forms.SizeType.Percent;
			columnDefinition6.Width = 45.593869731800766D;
			columnDefinition7.SizeType = System.Windows.Forms.SizeType.Percent;
			columnDefinition7.Width = 0.76628352490421459D;
			columnDefinition8.SizeType = System.Windows.Forms.SizeType.Percent;
			columnDefinition8.Width = 46.743295019157088D;
			columnDefinition9.SizeType = System.Windows.Forms.SizeType.Absolute;
			columnDefinition9.Width = 24D;
			this.layoutControlGroup2.OptionsTableLayoutGroup.ColumnDefinitions.AddRange(new DevExpress.XtraLayout.ColumnDefinition[] {
			columnDefinition1,
			columnDefinition2,
			columnDefinition3,
			columnDefinition4,
			columnDefinition5,
			columnDefinition6,
			columnDefinition7,
			columnDefinition8,
			columnDefinition9});
			rowDefinition1.Height = 16D;
			rowDefinition1.SizeType = System.Windows.Forms.SizeType.AutoSize;
			rowDefinition2.Height = 24D;
			rowDefinition2.SizeType = System.Windows.Forms.SizeType.AutoSize;
			rowDefinition3.Height = 24D;
			rowDefinition3.SizeType = System.Windows.Forms.SizeType.AutoSize;
			rowDefinition4.Height = 24D;
			rowDefinition4.SizeType = System.Windows.Forms.SizeType.AutoSize;
			rowDefinition5.Height = 24D;
			rowDefinition5.SizeType = System.Windows.Forms.SizeType.AutoSize;
			rowDefinition6.Height = 24D;
			rowDefinition6.SizeType = System.Windows.Forms.SizeType.AutoSize;
			rowDefinition7.Height = 17D;
			rowDefinition7.SizeType = System.Windows.Forms.SizeType.AutoSize;
			rowDefinition8.Height = 24D;
			rowDefinition8.SizeType = System.Windows.Forms.SizeType.AutoSize;
			rowDefinition9.Height = 17D;
			rowDefinition9.SizeType = System.Windows.Forms.SizeType.AutoSize;
			rowDefinition10.Height = 24D;
			rowDefinition10.SizeType = System.Windows.Forms.SizeType.AutoSize;
			rowDefinition11.Height = 24D;
			rowDefinition11.SizeType = System.Windows.Forms.SizeType.AutoSize;
			rowDefinition12.Height = 24D;
			rowDefinition12.SizeType = System.Windows.Forms.SizeType.AutoSize;
			rowDefinition13.Height = 24D;
			rowDefinition13.SizeType = System.Windows.Forms.SizeType.AutoSize;
			rowDefinition14.Height = 19D;
			rowDefinition14.SizeType = System.Windows.Forms.SizeType.AutoSize;
			rowDefinition15.Height = 17D;
			rowDefinition15.SizeType = System.Windows.Forms.SizeType.AutoSize;
			rowDefinition16.Height = 24D;
			rowDefinition16.SizeType = System.Windows.Forms.SizeType.AutoSize;
			rowDefinition17.Height = 17D;
			rowDefinition17.SizeType = System.Windows.Forms.SizeType.AutoSize;
			rowDefinition18.Height = 24D;
			rowDefinition18.SizeType = System.Windows.Forms.SizeType.AutoSize;
			rowDefinition19.Height = 24D;
			rowDefinition19.SizeType = System.Windows.Forms.SizeType.AutoSize;
			rowDefinition20.Height = 17D;
			rowDefinition20.SizeType = System.Windows.Forms.SizeType.AutoSize;
			this.layoutControlGroup2.OptionsTableLayoutGroup.RowDefinitions.AddRange(new DevExpress.XtraLayout.RowDefinition[] {
			rowDefinition1,
			rowDefinition2,
			rowDefinition3,
			rowDefinition4,
			rowDefinition5,
			rowDefinition6,
			rowDefinition7,
			rowDefinition8,
			rowDefinition9,
			rowDefinition10,
			rowDefinition11,
			rowDefinition12,
			rowDefinition13,
			rowDefinition14,
			rowDefinition15,
			rowDefinition16,
			rowDefinition17,
			rowDefinition18,
			rowDefinition19,
			rowDefinition20});
			this.layoutControlGroup2.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
			this.layoutControlGroup2.Size = new System.Drawing.Size(481, 432);
			this.layoutControlItem1.AppearanceItemCaption.Options.UseTextOptions = true;
			this.layoutControlItem1.AppearanceItemCaption.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
			this.layoutControlItem1.Control = this.lbPrinterName;
			this.layoutControlItem1.ControlAlignment = System.Drawing.ContentAlignment.MiddleLeft;
			this.layoutControlItem1.Location = new System.Drawing.Point(18, 16);
			this.layoutControlItem1.Name = "layoutControlItem1";
			this.layoutControlItem1.OptionsTableLayoutItem.ColumnIndex = 1;
			this.layoutControlItem1.OptionsTableLayoutItem.RowIndex = 1;
			this.layoutControlItem1.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
			this.layoutControlItem1.Size = new System.Drawing.Size(119, 24);
			this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem1.TextVisible = false;
			this.layoutControlItem1.TrimClientAreaToControl = false;
			this.layoutControlItem2.AppearanceItemCaption.Options.UseTextOptions = true;
			this.layoutControlItem2.AppearanceItemCaption.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
			this.layoutControlItem2.Control = this.lbPrinterStatusCaption;
			this.layoutControlItem2.Location = new System.Drawing.Point(18, 40);
			this.layoutControlItem2.Name = "layoutControlItem2";
			this.layoutControlItem2.OptionsTableLayoutItem.ColumnIndex = 1;
			this.layoutControlItem2.OptionsTableLayoutItem.RowIndex = 2;
			this.layoutControlItem2.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
			this.layoutControlItem2.Size = new System.Drawing.Size(119, 24);
			this.layoutControlItem2.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem2.TextVisible = false;
			this.layoutControlItem3.AppearanceItemCaption.Options.UseTextOptions = true;
			this.layoutControlItem3.AppearanceItemCaption.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
			this.layoutControlItem3.Control = this.lbPrinterLocationCaption;
			this.layoutControlItem3.Location = new System.Drawing.Point(18, 64);
			this.layoutControlItem3.Name = "layoutControlItem3";
			this.layoutControlItem3.OptionsTableLayoutItem.ColumnIndex = 1;
			this.layoutControlItem3.OptionsTableLayoutItem.RowIndex = 3;
			this.layoutControlItem3.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
			this.layoutControlItem3.Size = new System.Drawing.Size(119, 24);
			this.layoutControlItem3.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem3.TextVisible = false;
			this.layoutControlItem4.AppearanceItemCaption.Options.UseTextOptions = true;
			this.layoutControlItem4.AppearanceItemCaption.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
			this.layoutControlItem4.Control = this.lbPrinterCommentCaption;
			this.layoutControlItem4.Location = new System.Drawing.Point(18, 88);
			this.layoutControlItem4.Name = "layoutControlItem4";
			this.layoutControlItem4.OptionsTableLayoutItem.ColumnIndex = 1;
			this.layoutControlItem4.OptionsTableLayoutItem.RowIndex = 4;
			this.layoutControlItem4.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
			this.layoutControlItem4.Size = new System.Drawing.Size(119, 24);
			this.layoutControlItem4.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem4.TextVisible = false;
			this.layoutControlItem5.AppearanceItemCaption.Options.UseTextOptions = true;
			this.layoutControlItem5.AppearanceItemCaption.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
			this.layoutControlItem5.Control = this.lbPrinterDocumentsInQueueCaption;
			this.layoutControlItem5.Location = new System.Drawing.Point(18, 112);
			this.layoutControlItem5.Name = "layoutControlItem5";
			this.layoutControlItem5.OptionsTableLayoutItem.ColumnIndex = 1;
			this.layoutControlItem5.OptionsTableLayoutItem.RowIndex = 5;
			this.layoutControlItem5.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
			this.layoutControlItem5.Size = new System.Drawing.Size(119, 24);
			this.layoutControlItem5.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem5.TextVisible = false;
			this.layoutControlItem6.Control = this.lbCopies;
			this.layoutControlItem6.ControlAlignment = System.Drawing.ContentAlignment.MiddleLeft;
			this.layoutControlItem6.Location = new System.Drawing.Point(18, 153);
			this.layoutControlItem6.Name = "layoutControlItem6";
			this.layoutControlItem6.OptionsTableLayoutItem.ColumnIndex = 1;
			this.layoutControlItem6.OptionsTableLayoutItem.RowIndex = 7;
			this.layoutControlItem6.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
			this.layoutControlItem6.Size = new System.Drawing.Size(119, 24);
			this.layoutControlItem6.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem6.TextVisible = false;
			this.layoutControlItem6.TrimClientAreaToControl = false;
			this.layoutControlItem7.Control = this.icbInstalledPrinters;
			this.layoutControlItem7.ControlAlignment = System.Drawing.ContentAlignment.MiddleLeft;
			this.layoutControlItem7.Location = new System.Drawing.Point(140, 16);
			this.layoutControlItem7.Name = "layoutControlItem7";
			this.layoutControlItem7.OptionsTableLayoutItem.ColumnIndex = 3;
			this.layoutControlItem7.OptionsTableLayoutItem.ColumnSpan = 3;
			this.layoutControlItem7.OptionsTableLayoutItem.RowIndex = 1;
			this.layoutControlItem7.Size = new System.Drawing.Size(193, 24);
			this.layoutControlItem7.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem7.TextVisible = false;
			this.layoutControlItem7.TrimClientAreaToControl = false;
			this.layoutControlItem8.Control = this.spCopies;
			this.layoutControlItem8.ControlAlignment = System.Drawing.ContentAlignment.MiddleLeft;
			this.layoutControlItem8.Location = new System.Drawing.Point(140, 153);
			this.layoutControlItem8.Name = "layoutControlItem8";
			this.layoutControlItem8.OptionsTableLayoutItem.ColumnIndex = 3;
			this.layoutControlItem8.OptionsTableLayoutItem.ColumnSpan = 2;
			this.layoutControlItem8.OptionsTableLayoutItem.RowIndex = 7;
			this.layoutControlItem8.Size = new System.Drawing.Size(74, 24);
			this.layoutControlItem8.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem8.TextVisible = false;
			this.layoutControlItem8.TrimClientAreaToControl = false;
			this.layoutControlItem9.Control = this.btnPrinterPreferences;
			this.layoutControlItem9.ControlAlignment = System.Drawing.ContentAlignment.MiddleCenter;
			this.layoutControlItem9.Location = new System.Drawing.Point(335, 16);
			this.layoutControlItem9.Name = "layoutControlItem9";
			this.layoutControlItem9.OptionsTableLayoutItem.ColumnIndex = 7;
			this.layoutControlItem9.OptionsTableLayoutItem.RowIndex = 1;
			this.layoutControlItem9.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 1, 1);
			this.layoutControlItem9.Size = new System.Drawing.Size(122, 24);
			this.layoutControlItem9.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem9.TextVisible = false;
			this.layoutControlItem9.TrimClientAreaToControl = false;
			this.layoutControlItem10.Control = this.lbPageRange;
			this.layoutControlItem10.ControlAlignment = System.Drawing.ContentAlignment.MiddleLeft;
			this.layoutControlItem10.Location = new System.Drawing.Point(18, 194);
			this.layoutControlItem10.Name = "layoutControlItem10";
			this.layoutControlItem10.OptionsTableLayoutItem.ColumnIndex = 1;
			this.layoutControlItem10.OptionsTableLayoutItem.RowIndex = 9;
			this.layoutControlItem10.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
			this.layoutControlItem10.Size = new System.Drawing.Size(119, 24);
			this.layoutControlItem10.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem10.TextVisible = false;
			this.layoutControlItem10.TrimClientAreaToControl = false;
			this.layoutControlItem11.Control = this.rbRangeAll;
			this.layoutControlItem11.ControlAlignment = System.Drawing.ContentAlignment.MiddleLeft;
			this.layoutControlItem11.Location = new System.Drawing.Point(140, 194);
			this.layoutControlItem11.Name = "layoutControlItem11";
			this.layoutControlItem11.OptionsTableLayoutItem.ColumnIndex = 3;
			this.layoutControlItem11.OptionsTableLayoutItem.ColumnSpan = 5;
			this.layoutControlItem11.OptionsTableLayoutItem.RowIndex = 9;
			this.layoutControlItem11.Size = new System.Drawing.Size(317, 24);
			this.layoutControlItem11.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem11.TextVisible = false;
			this.layoutControlItem11.TrimClientAreaToControl = false;
			this.layoutControlItem12.Control = this.rbCurrentPage;
			this.layoutControlItem12.ControlAlignment = System.Drawing.ContentAlignment.MiddleLeft;
			this.layoutControlItem12.Location = new System.Drawing.Point(140, 218);
			this.layoutControlItem12.Name = "layoutControlItem12";
			this.layoutControlItem12.OptionsTableLayoutItem.ColumnIndex = 3;
			this.layoutControlItem12.OptionsTableLayoutItem.ColumnSpan = 5;
			this.layoutControlItem12.OptionsTableLayoutItem.RowIndex = 10;
			this.layoutControlItem12.Size = new System.Drawing.Size(317, 24);
			this.layoutControlItem12.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem12.TextVisible = false;
			this.layoutControlItem12.TrimClientAreaToControl = false;
			this.layoutControlItem13.Control = this.rbSelection;
			this.layoutControlItem13.ControlAlignment = System.Drawing.ContentAlignment.MiddleLeft;
			this.layoutControlItem13.Location = new System.Drawing.Point(140, 242);
			this.layoutControlItem13.Name = "layoutControlItem13";
			this.layoutControlItem13.OptionsTableLayoutItem.ColumnIndex = 3;
			this.layoutControlItem13.OptionsTableLayoutItem.ColumnSpan = 5;
			this.layoutControlItem13.OptionsTableLayoutItem.RowIndex = 11;
			this.layoutControlItem13.Size = new System.Drawing.Size(317, 24);
			this.layoutControlItem13.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem13.TextVisible = false;
			this.layoutControlItem13.TrimClientAreaToControl = false;
			this.layoutControlItem14.Control = this.rbSomePages;
			this.layoutControlItem14.ControlAlignment = System.Drawing.ContentAlignment.MiddleLeft;
			this.layoutControlItem14.Location = new System.Drawing.Point(140, 266);
			this.layoutControlItem14.Name = "layoutControlItem14";
			this.layoutControlItem14.OptionsTableLayoutItem.ColumnIndex = 3;
			this.layoutControlItem14.OptionsTableLayoutItem.RowIndex = 12;
			this.layoutControlItem14.Size = new System.Drawing.Size(59, 24);
			this.layoutControlItem14.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem14.TextVisible = false;
			this.layoutControlItem14.TrimClientAreaToControl = false;
			this.layoutControlItem15.Control = this.chbCollate;
			this.layoutControlItem15.ControlAlignment = System.Drawing.ContentAlignment.MiddleLeft;
			this.layoutControlItem15.Location = new System.Drawing.Point(214, 153);
			this.layoutControlItem15.Name = "layoutControlItem15";
			this.layoutControlItem15.OptionsTableLayoutItem.ColumnIndex = 5;
			this.layoutControlItem15.OptionsTableLayoutItem.ColumnSpan = 3;
			this.layoutControlItem15.OptionsTableLayoutItem.RowIndex = 7;
			this.layoutControlItem15.Padding = new DevExpress.XtraLayout.Utils.Padding(8, 0, 0, 0);
			this.layoutControlItem15.Size = new System.Drawing.Size(243, 24);
			this.layoutControlItem15.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem15.TextVisible = false;
			this.layoutControlItem15.TrimClientAreaToControl = false;
			this.layoutControlItem16.Control = this.chbPrintToFile;
			this.layoutControlItem16.ControlAlignment = System.Drawing.ContentAlignment.MiddleLeft;
			this.layoutControlItem16.Location = new System.Drawing.Point(140, 367);
			this.layoutControlItem16.Name = "layoutControlItem16";
			this.layoutControlItem16.OptionsTableLayoutItem.ColumnIndex = 3;
			this.layoutControlItem16.OptionsTableLayoutItem.ColumnSpan = 5;
			this.layoutControlItem16.OptionsTableLayoutItem.RowIndex = 17;
			this.layoutControlItem16.Size = new System.Drawing.Size(317, 24);
			this.layoutControlItem16.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem16.TextVisible = false;
			this.layoutControlItem16.TrimClientAreaToControl = false;
			this.layoutControlItem17.Control = this.txtPageRange;
			this.layoutControlItem17.ControlAlignment = System.Drawing.ContentAlignment.MiddleLeft;
			this.layoutControlItem17.Location = new System.Drawing.Point(199, 266);
			this.layoutControlItem17.Name = "layoutControlItem17";
			this.layoutControlItem17.OptionsTableLayoutItem.ColumnIndex = 4;
			this.layoutControlItem17.OptionsTableLayoutItem.ColumnSpan = 4;
			this.layoutControlItem17.OptionsTableLayoutItem.RowIndex = 12;
			this.layoutControlItem17.Size = new System.Drawing.Size(258, 24);
			this.layoutControlItem17.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem17.TextVisible = false;
			this.layoutControlItem17.TrimClientAreaToControl = false;
			this.layoutControlItem19.AppearanceItemCaption.Options.UseTextOptions = true;
			this.layoutControlItem19.AppearanceItemCaption.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
			this.layoutControlItem19.Control = this.lbPaperSource;
			this.layoutControlItem19.ControlAlignment = System.Drawing.ContentAlignment.MiddleLeft;
			this.layoutControlItem19.Location = new System.Drawing.Point(18, 326);
			this.layoutControlItem19.Name = "layoutControlItem19";
			this.layoutControlItem19.OptionsTableLayoutItem.ColumnIndex = 1;
			this.layoutControlItem19.OptionsTableLayoutItem.RowIndex = 15;
			this.layoutControlItem19.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
			this.layoutControlItem19.Size = new System.Drawing.Size(119, 24);
			this.layoutControlItem19.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem19.TextVisible = false;
			this.layoutControlItem19.TrimClientAreaToControl = false;
			this.layoutControlItem20.Control = this.cbPaperSources;
			this.layoutControlItem20.ControlAlignment = System.Drawing.ContentAlignment.MiddleLeft;
			this.layoutControlItem20.Location = new System.Drawing.Point(140, 326);
			this.layoutControlItem20.Name = "layoutControlItem20";
			this.layoutControlItem20.OptionsTableLayoutItem.ColumnIndex = 3;
			this.layoutControlItem20.OptionsTableLayoutItem.ColumnSpan = 5;
			this.layoutControlItem20.OptionsTableLayoutItem.RowIndex = 15;
			this.layoutControlItem20.Size = new System.Drawing.Size(317, 24);
			this.layoutControlItem20.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem20.TextVisible = false;
			this.layoutControlItem20.TrimClientAreaToControl = false;
			this.layoutControlItem21.AppearanceItemCaption.Options.UseTextOptions = true;
			this.layoutControlItem21.AppearanceItemCaption.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
			this.layoutControlItem21.Control = this.lbFilePath;
			this.layoutControlItem21.ControlAlignment = System.Drawing.ContentAlignment.MiddleLeft;
			this.layoutControlItem21.Location = new System.Drawing.Point(18, 391);
			this.layoutControlItem21.Name = "layoutControlItem21";
			this.layoutControlItem21.OptionsTableLayoutItem.ColumnIndex = 1;
			this.layoutControlItem21.OptionsTableLayoutItem.RowIndex = 18;
			this.layoutControlItem21.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
			this.layoutControlItem21.Size = new System.Drawing.Size(119, 24);
			this.layoutControlItem21.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem21.TextVisible = false;
			this.layoutControlItem21.TrimClientAreaToControl = false;
			this.layoutControlItem22.Control = this.btneBrowse;
			this.layoutControlItem22.ControlAlignment = System.Drawing.ContentAlignment.MiddleLeft;
			this.layoutControlItem22.Location = new System.Drawing.Point(140, 391);
			this.layoutControlItem22.Name = "layoutControlItem22";
			this.layoutControlItem22.OptionsTableLayoutItem.ColumnIndex = 3;
			this.layoutControlItem22.OptionsTableLayoutItem.ColumnSpan = 5;
			this.layoutControlItem22.OptionsTableLayoutItem.RowIndex = 18;
			this.layoutControlItem22.Size = new System.Drawing.Size(317, 24);
			this.layoutControlItem22.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem22.TextVisible = false;
			this.layoutControlItem22.TrimClientAreaToControl = false;
			this.layoutControlItem25.AppearanceItemCaption.Options.UseTextOptions = true;
			this.layoutControlItem25.AppearanceItemCaption.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
			this.layoutControlItem25.Control = this.lbPrinterStatus;
			this.layoutControlItem25.ControlAlignment = System.Drawing.ContentAlignment.MiddleLeft;
			this.layoutControlItem25.Location = new System.Drawing.Point(140, 40);
			this.layoutControlItem25.Name = "layoutControlItem25";
			this.layoutControlItem25.OptionsTableLayoutItem.ColumnIndex = 3;
			this.layoutControlItem25.OptionsTableLayoutItem.ColumnSpan = 5;
			this.layoutControlItem25.OptionsTableLayoutItem.RowIndex = 2;
			this.layoutControlItem25.Padding = new DevExpress.XtraLayout.Utils.Padding(2, 0, 0, 0);
			this.layoutControlItem25.Size = new System.Drawing.Size(317, 24);
			this.layoutControlItem25.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem25.TextVisible = false;
			this.layoutControlItem25.TrimClientAreaToControl = false;
			this.layoutControlItem26.AppearanceItemCaption.Options.UseTextOptions = true;
			this.layoutControlItem26.AppearanceItemCaption.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
			this.layoutControlItem26.Control = this.lbPrinterLocation;
			this.layoutControlItem26.ControlAlignment = System.Drawing.ContentAlignment.MiddleLeft;
			this.layoutControlItem26.Location = new System.Drawing.Point(140, 64);
			this.layoutControlItem26.Name = "layoutControlItem26";
			this.layoutControlItem26.OptionsTableLayoutItem.ColumnIndex = 3;
			this.layoutControlItem26.OptionsTableLayoutItem.ColumnSpan = 5;
			this.layoutControlItem26.OptionsTableLayoutItem.RowIndex = 3;
			this.layoutControlItem26.Padding = new DevExpress.XtraLayout.Utils.Padding(2, 0, 0, 0);
			this.layoutControlItem26.Size = new System.Drawing.Size(317, 24);
			this.layoutControlItem26.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem26.TextVisible = false;
			this.layoutControlItem26.TrimClientAreaToControl = false;
			this.layoutControlItem27.AppearanceItemCaption.Options.UseTextOptions = true;
			this.layoutControlItem27.AppearanceItemCaption.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
			this.layoutControlItem27.Control = this.lbPrinterComment;
			this.layoutControlItem27.ControlAlignment = System.Drawing.ContentAlignment.MiddleLeft;
			this.layoutControlItem27.Location = new System.Drawing.Point(140, 88);
			this.layoutControlItem27.Name = "layoutControlItem27";
			this.layoutControlItem27.OptionsTableLayoutItem.ColumnIndex = 3;
			this.layoutControlItem27.OptionsTableLayoutItem.ColumnSpan = 5;
			this.layoutControlItem27.OptionsTableLayoutItem.RowIndex = 4;
			this.layoutControlItem27.Padding = new DevExpress.XtraLayout.Utils.Padding(2, 0, 0, 0);
			this.layoutControlItem27.Size = new System.Drawing.Size(317, 24);
			this.layoutControlItem27.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem27.TextVisible = false;
			this.layoutControlItem27.TrimClientAreaToControl = false;
			this.layoutControlItem28.AppearanceItemCaption.Options.UseTextOptions = true;
			this.layoutControlItem28.AppearanceItemCaption.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
			this.layoutControlItem28.Control = this.lbPrinterDocumentsInQueue;
			this.layoutControlItem28.ControlAlignment = System.Drawing.ContentAlignment.MiddleLeft;
			this.layoutControlItem28.Location = new System.Drawing.Point(140, 112);
			this.layoutControlItem28.Name = "layoutControlItem28";
			this.layoutControlItem28.OptionsTableLayoutItem.ColumnIndex = 3;
			this.layoutControlItem28.OptionsTableLayoutItem.ColumnSpan = 5;
			this.layoutControlItem28.OptionsTableLayoutItem.RowIndex = 5;
			this.layoutControlItem28.Padding = new DevExpress.XtraLayout.Utils.Padding(2, 0, 0, 0);
			this.layoutControlItem28.Size = new System.Drawing.Size(317, 24);
			this.layoutControlItem28.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem28.TextVisible = false;
			this.layoutControlItem28.TrimClientAreaToControl = false;
			this.layoutControlItem18.Control = this.lbPrintToFile;
			this.layoutControlItem18.ControlAlignment = System.Drawing.ContentAlignment.MiddleLeft;
			this.layoutControlItem18.Location = new System.Drawing.Point(18, 367);
			this.layoutControlItem18.Name = "layoutControlItem18";
			this.layoutControlItem18.OptionsTableLayoutItem.ColumnIndex = 1;
			this.layoutControlItem18.OptionsTableLayoutItem.RowIndex = 17;
			this.layoutControlItem18.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
			this.layoutControlItem18.Size = new System.Drawing.Size(119, 24);
			this.layoutControlItem18.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem18.TextVisible = false;
			this.layoutControlItem18.TrimClientAreaToControl = false;
			this.layoutControlItem29.Control = this.lbPageRangeComment;
			this.layoutControlItem29.Location = new System.Drawing.Point(199, 290);
			this.layoutControlItem29.Name = "layoutControlItem29";
			this.layoutControlItem29.OptionsTableLayoutItem.ColumnIndex = 4;
			this.layoutControlItem29.OptionsTableLayoutItem.ColumnSpan = 4;
			this.layoutControlItem29.OptionsTableLayoutItem.RowIndex = 13;
			this.layoutControlItem29.Padding = new DevExpress.XtraLayout.Utils.Padding(2, 2, 3, 3);
			this.layoutControlItem29.Size = new System.Drawing.Size(258, 19);
			this.layoutControlItem29.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem29.TextVisible = false;
			this.layoutControlItem30.Control = this.emptySpaceLabelControl1;
			this.layoutControlItem30.Location = new System.Drawing.Point(18, 136);
			this.layoutControlItem30.Name = "layoutControlItem30";
			this.layoutControlItem30.OptionsTableLayoutItem.ColumnIndex = 1;
			this.layoutControlItem30.OptionsTableLayoutItem.ColumnSpan = 7;
			this.layoutControlItem30.OptionsTableLayoutItem.RowIndex = 6;
			this.layoutControlItem30.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
			this.layoutControlItem30.Size = new System.Drawing.Size(439, 17);
			this.layoutControlItem30.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem30.TextVisible = false;
			this.layoutControlItem31.Control = this.emptySpaceLabelControl2;
			this.layoutControlItem31.Location = new System.Drawing.Point(18, 177);
			this.layoutControlItem31.Name = "layoutControlItem31";
			this.layoutControlItem31.OptionsTableLayoutItem.ColumnIndex = 1;
			this.layoutControlItem31.OptionsTableLayoutItem.ColumnSpan = 7;
			this.layoutControlItem31.OptionsTableLayoutItem.RowIndex = 8;
			this.layoutControlItem31.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
			this.layoutControlItem31.Size = new System.Drawing.Size(439, 17);
			this.layoutControlItem31.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem31.TextVisible = false;
			this.layoutControlItem32.Control = this.emptySpaceLabelControl3;
			this.layoutControlItem32.Location = new System.Drawing.Point(18, 309);
			this.layoutControlItem32.Name = "layoutControlItem32";
			this.layoutControlItem32.OptionsTableLayoutItem.ColumnIndex = 1;
			this.layoutControlItem32.OptionsTableLayoutItem.ColumnSpan = 7;
			this.layoutControlItem32.OptionsTableLayoutItem.RowIndex = 14;
			this.layoutControlItem32.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
			this.layoutControlItem32.Size = new System.Drawing.Size(439, 17);
			this.layoutControlItem32.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem32.TextVisible = false;
			this.layoutControlItem33.Control = this.emptySpaceLabelControl4;
			this.layoutControlItem33.Location = new System.Drawing.Point(18, 350);
			this.layoutControlItem33.Name = "layoutControlItem33";
			this.layoutControlItem33.OptionsTableLayoutItem.ColumnIndex = 1;
			this.layoutControlItem33.OptionsTableLayoutItem.ColumnSpan = 7;
			this.layoutControlItem33.OptionsTableLayoutItem.RowIndex = 16;
			this.layoutControlItem33.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
			this.layoutControlItem33.Size = new System.Drawing.Size(439, 17);
			this.layoutControlItem33.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem33.TextVisible = false;
			this.layoutControlItem34.Control = this.emptySpaceLabelControl5;
			this.layoutControlItem34.Location = new System.Drawing.Point(18, 0);
			this.layoutControlItem34.Name = "layoutControlItem34";
			this.layoutControlItem34.OptionsTableLayoutItem.ColumnIndex = 1;
			this.layoutControlItem34.OptionsTableLayoutItem.ColumnSpan = 7;
			this.layoutControlItem34.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
			this.layoutControlItem34.Size = new System.Drawing.Size(439, 16);
			this.layoutControlItem34.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem34.TextVisible = false;
			this.layoutControlItem35.Control = this.emptySpaceLabelControl6;
			this.layoutControlItem35.Location = new System.Drawing.Point(18, 415);
			this.layoutControlItem35.Name = "layoutControlItem35";
			this.layoutControlItem35.OptionsTableLayoutItem.ColumnIndex = 1;
			this.layoutControlItem35.OptionsTableLayoutItem.ColumnSpan = 7;
			this.layoutControlItem35.OptionsTableLayoutItem.RowIndex = 19;
			this.layoutControlItem35.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
			this.layoutControlItem35.Size = new System.Drawing.Size(439, 17);
			this.layoutControlItem35.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem35.TextVisible = false;
			this.layoutControlItem36.Control = this.fakeLabelControl1;
			this.layoutControlItem36.Location = new System.Drawing.Point(18, 218);
			this.layoutControlItem36.Name = "layoutControlItem36";
			this.layoutControlItem36.OptionsTableLayoutItem.ColumnIndex = 1;
			this.layoutControlItem36.OptionsTableLayoutItem.RowIndex = 10;
			this.layoutControlItem36.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
			this.layoutControlItem36.Size = new System.Drawing.Size(119, 24);
			this.layoutControlItem36.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem36.TextVisible = false;
			this.layoutControlItem37.Control = this.fakeLabelControl2;
			this.layoutControlItem37.Location = new System.Drawing.Point(18, 242);
			this.layoutControlItem37.Name = "layoutControlItem37";
			this.layoutControlItem37.OptionsTableLayoutItem.ColumnIndex = 1;
			this.layoutControlItem37.OptionsTableLayoutItem.RowIndex = 11;
			this.layoutControlItem37.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
			this.layoutControlItem37.Size = new System.Drawing.Size(119, 24);
			this.layoutControlItem37.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem37.TextVisible = false;
			this.layoutControlItem38.Control = this.fakeLabelControl3;
			this.layoutControlItem38.Location = new System.Drawing.Point(18, 266);
			this.layoutControlItem38.Name = "layoutControlItem38";
			this.layoutControlItem38.OptionsTableLayoutItem.ColumnIndex = 1;
			this.layoutControlItem38.OptionsTableLayoutItem.RowIndex = 12;
			this.layoutControlItem38.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
			this.layoutControlItem38.Size = new System.Drawing.Size(119, 24);
			this.layoutControlItem38.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem38.TextVisible = false;
			this.grpButtons.GroupBordersVisible = false;
			this.grpButtons.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
			this.layoutControlItem23,
			this.layoutControlItem24,
			this.layoutControlItem39});
			this.grpButtons.LayoutMode = DevExpress.XtraLayout.Utils.LayoutMode.Table;
			this.grpButtons.Location = new System.Drawing.Point(0, 432);
			this.grpButtons.Name = "grpButtons";
			this.grpButtons.OptionsTableLayoutGroup.AutoSizeDefaultDefinitionLength = 1;
			columnDefinition10.SizeType = System.Windows.Forms.SizeType.AutoSize;
			columnDefinition10.Width = 312D;
			columnDefinition11.SizeType = System.Windows.Forms.SizeType.Absolute;
			columnDefinition11.Width = 79D;
			columnDefinition12.SizeType = System.Windows.Forms.SizeType.Absolute;
			columnDefinition12.Width = 1D;
			columnDefinition13.SizeType = System.Windows.Forms.SizeType.Absolute;
			columnDefinition13.Width = 79D;
			columnDefinition14.SizeType = System.Windows.Forms.SizeType.Absolute;
			columnDefinition14.Width = 10D;
			this.grpButtons.OptionsTableLayoutGroup.ColumnDefinitions.AddRange(new DevExpress.XtraLayout.ColumnDefinition[] {
			columnDefinition10,
			columnDefinition11,
			columnDefinition12,
			columnDefinition13,
			columnDefinition14});
			rowDefinition21.Height = 26D;
			rowDefinition21.SizeType = System.Windows.Forms.SizeType.AutoSize;
			rowDefinition22.Height = 13D;
			rowDefinition22.SizeType = System.Windows.Forms.SizeType.AutoSize;
			this.grpButtons.OptionsTableLayoutGroup.RowDefinitions.AddRange(new DevExpress.XtraLayout.RowDefinition[] {
			rowDefinition21,
			rowDefinition22});
			this.grpButtons.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
			this.grpButtons.Size = new System.Drawing.Size(481, 39);
			this.layoutControlItem23.Control = this.btnPrint;
			this.layoutControlItem23.Location = new System.Drawing.Point(312, 0);
			this.layoutControlItem23.Name = "layoutControlItem23";
			this.layoutControlItem23.OptionsTableLayoutItem.ColumnIndex = 1;
			this.layoutControlItem23.Size = new System.Drawing.Size(79, 26);
			this.layoutControlItem23.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem23.TextVisible = false;
			this.layoutControlItem24.Control = this.btnCancel;
			this.layoutControlItem24.Location = new System.Drawing.Point(392, 0);
			this.layoutControlItem24.Name = "layoutControlItem24";
			this.layoutControlItem24.OptionsTableLayoutItem.ColumnIndex = 3;
			this.layoutControlItem24.Size = new System.Drawing.Size(79, 26);
			this.layoutControlItem24.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem24.TextVisible = false;
			this.layoutControlItem39.Control = this.emptySpaceLabelControl7;
			this.layoutControlItem39.Location = new System.Drawing.Point(0, 26);
			this.layoutControlItem39.Name = "layoutControlItem39";
			this.layoutControlItem39.OptionsTableLayoutItem.ColumnSpan = 4;
			this.layoutControlItem39.OptionsTableLayoutItem.RowIndex = 1;
			this.layoutControlItem39.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
			this.layoutControlItem39.Size = new System.Drawing.Size(471, 13);
			this.layoutControlItem39.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem39.TextVisible = false;
			this.AcceptButton = this.btnPrint;
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.btnCancel;
			this.Controls.Add(this.layoutControl1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "PrintEditorForm";
			this.ShowInTaskbar = false;
			this.Load += new System.EventHandler(this.PrintEditorForm_Load);
			this.Shown += new System.EventHandler(this.PrintEditorForm_Shown);
			((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
			this.layoutControl1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.btneBrowse.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.cbPaperSources.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.txtPageRange.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chbPrintToFile.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chbCollate.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.rbSomePages.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.rbSelection.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.rbCurrentPage.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.rbRangeAll.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.spCopies.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.icbInstalledPrinters.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup2)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem4)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem5)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem6)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem7)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem8)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem9)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem10)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem11)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem12)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem13)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem14)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem15)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem16)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem17)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem19)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem20)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem21)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem22)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem25)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem26)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem27)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem28)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem18)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem29)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem30)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem31)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem32)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem33)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem34)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem35)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem36)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem37)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem38)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.grpButtons)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem23)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem24)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem39)).EndInit();
			this.ResumeLayout(false);
		}
		#endregion
		private void spCopies_ValueChanged(object sender, EventArgs e) {
			chbCollate.Enabled = ((SpinEdit)sender).Value > 1 ? true : false;
		}
		private void rbPageRange_CheckedChanged(object sender, EventArgs e) {
			CheckEdit editor = sender as CheckEdit;
			if(editor != null && editor.Checked) {
				PrintRange = printRanges.Where<PrintRangeRadioButton>(item => item.Editor.Equals(editor)).First<PrintRangeRadioButton>().PrintRange;
				txtPageRange.Enabled = PrintRange == System.Drawing.Printing.PrintRange.SomePages;
				CheckedChanged();
			}
		}
		private void btneBrowse_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e) {
			SaveFileDialog fileDialog = new SaveFileDialog()
			{
				Filter = "Printable Files (*.prn)|*.prn|All Files (*.*)|*.*",
				OverwritePrompt = false,
				CheckFileExists = false,
				CheckPathExists = false
			};
			if(DialogRunner.ShowDialog(fileDialog) == System.Windows.Forms.DialogResult.OK)
				PrintFileName = fileDialog.FileName;
		}
		protected override void OnRightToLeftChanged(EventArgs e) {
			base.OnRightToLeftChanged(e);
			isRTLChanged = true; 
		}
		private void PrintEditorForm_Load(object sender, EventArgs e) {
			InitializeLayout();
			if(Document == null)
				return;
			controller.LoadForm(printerItemContainer);
			if(icbInstalledPrinters.SelectedIndex < 0 && icbInstalledPrinters.Properties.Items.Count > 0)
				icbInstalledPrinters.SelectedIndex = 0;
			icbInstalledPrinters_SelectedIndexChanged(icbInstalledPrinters, EventArgs.Empty);
			icbInstalledPrinters.SelectedIndexChanged += icbInstalledPrinters_SelectedIndexChanged;
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
			int btnPrintBestWidth = btnPrint.CalcBestSize().Width;
			int btnCancelBestWidth = btnCancel.CalcBestSize().Width;
			if(btnPrintBestWidth <= btnPrint.Width && btnCancelBestWidth <= btnCancel.Width)
				return;
			int btnPrintOKActualSize = Math.Max(btnPrintBestWidth, btnCancelBestWidth);
			grpButtons.OptionsTableLayoutGroup.ColumnDefinitions[1].Width =
			grpButtons.OptionsTableLayoutGroup.ColumnDefinitions[3].Width = btnPrintOKActualSize + 2 + 2;
		}
		private void btnPrint_Click(object sender, EventArgs e) {
			if(PrintRange == PrintRange.SomePages && !ValidatePageRange() || PrintToFile && !ValidateFilePath())
				return;
			controller.AssignPrinterSettings();
			DialogResult = DialogResult.OK;
		}
		bool ValidateFilePath() {
			string messageText;
			bool result = PrintEditorController.ValidateFilePath(PrintFileName, out messageText);
			if(!result)
				ShowMessage(messageText, MessageBoxButtons.OK, MessageBoxIcon.Error);
			else if(messageText != null)
				result = DialogResult.OK == ShowMessage(PreviewStringId.Msg_FileAlreadyExists.GetString(), MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
			return result;
		}
		bool ValidatePageRange() {
			int[] pageIndices = PageRangeParser.GetIndices(PageRangeText, Document.PrinterSettings.MaximumPage);
			if(string.IsNullOrEmpty(PageRangeText) || pageIndices.Length <= 0 || (pageIndices.Length == 1 && pageIndices[0] == -1)) {
				ShowMessage(PreviewStringId.Msg_IncorrectPageRange.GetString(), MessageBoxButtons.OK, MessageBoxIcon.Error);
				return false;
			}
			return true;
		}
		DialogResult ShowMessage(string message, MessageBoxButtons buttons, MessageBoxIcon icon) {
			return XtraMessageBox.Show(LookAndFeel, this, message, this.Text, buttons, icon);
		}
		private void btnCancel_Click(object sender, EventArgs e) {
			controller.CancelPrinterSettings();
			DialogResult = DialogResult.Cancel;
		}
		private void btnPrinterPreferences_Click(object sender, EventArgs e) {
			controller.ShowPrinterPreferences(Handle);
			UpdatePaperSources();
		}
		private void icbInstalledPrinters_SelectedIndexChanged(object sender, EventArgs e) {
			if(PrinterIsValid) {
				PrinterItem item = printerItemContainer.Items[icbInstalledPrinters.SelectedIndex];
				Document.PrinterSettings.PrinterName = item.FullName;
				UpdatePrinterInfo(item);
				UpdateToolTips(item);
			}
			UpdatePaperSources();
		}
		void UpdateToolTips(PrinterItem item) {
			icbInstalledPrinters.ToolTip = item.DisplayName;
			lbPrinterComment.ToolTip = item.Comment;
		}
		bool PrinterIsValid {
			get {
				return icbInstalledPrinters.SelectedIndex >= 0 && icbInstalledPrinters.SelectedIndex < printerItemContainer.Items.Count;
			}
		}
		private void spCopies_Validating(object sender, System.ComponentModel.CancelEventArgs e) {
			short result;
			e.Cancel = !short.TryParse(spCopies.Text, out result);
		}
		private void spCopies_InvalidValue(object sender, InvalidValueExceptionEventArgs e) {
			e.ErrorText = Localizer.Active.GetLocalizedString(StringId.IncorrectNumberCopies);
		}
		protected override bool ProcessDialogKey(Keys keyData) {
			if(keyData == Keys.Escape) {
				DialogResult = DialogResult.Cancel;
				controller.CancelPrinterSettings();
				return true;
			}
			return base.ProcessDialogKey(keyData);
		}
		void InitPrintRangesList() {
			printRanges.Add(new PrintRangeRadioButton(rbRangeAll, PrintRange.AllPages));
			printRanges.Add(new PrintRangeRadioButton(rbCurrentPage, PrintRange.CurrentPage));
			printRanges.Add(new PrintRangeRadioButton(rbSelection, PrintRange.Selection));
			printRanges.Add(new PrintRangeRadioButton(rbSomePages, PrintRange.SomePages));
		}
		private void CheckedChanged() {
			printRanges.Where<PrintRangeRadioButton>(item => item.PrintRange != PrintRange).ForEach<PrintRangeRadioButton>(item => item.Editor.Checked = false);
		}
		internal void SetPrintRange(PrintRange printRange) {
			var printRangeRadioButton = printRanges.Where<PrintRangeRadioButton>(item => item.PrintRange == printRange).First<PrintRangeRadioButton>();
			if(printRangeRadioButton.Editor.Enabled && !printRangeRadioButton.Editor.Checked)
				printRangeRadioButton.Editor.Checked = true;
		}
		void IPrintForm.SetPrintRange(PrintRange printRange) {
			SetPrintRange(printRange);
		}
		private void chbPrintToFile_CheckedChanged(object sender, EventArgs e) {
			btneBrowse.Enabled = chbPrintToFile.Checked;
		}
		private void PrintEditorForm_Shown(object sender, EventArgs e) {
			this.btnPrint.Focus();
		}
	}
}
