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
using System.Collections;
using System.Drawing;
using System.ComponentModel.Design;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.Data.Browsing;
using DevExpress.Data.Browsing.Design;
using DevExpress.XtraPrinting.Native;
using DevExpress.XtraReports.UI;
using DevExpress.XtraReports.Native.Data;
using DevExpress.XtraPrinting.Control;
using DevExpress.XtraPrinting;
using System.Drawing.Printing;
using System.ComponentModel;
using System.Collections.Generic;
namespace DevExpress.XtraReports.Design {
	public abstract class SummaryEditorFormBase : ReportsEditorFormBase {
		#region static
		protected static string[] GetEnumDisplayNames(Type type) {
			TypeConverter typeConverter = TypeDescriptor.GetConverter(type);
			ICollection values = Enum.GetValues(type);
			List<string> displayNames = new List<string>();
			foreach(object name in values) {
				displayNames.Add(typeConverter.ConvertToString(new StubTypeDescriptorContext(), name));
			}
			return displayNames.ToArray();
		}
		#endregion
		#region inner classes
		class MyPrintControl : DevExpress.DocumentView.Controls.DocumentViewerBase {
			public MyPrintControl() {
				vScrollBar.Visible = bottomPanel.Visible = false;
				fMinZoom = 0.00001f;
			}
			protected override void DrawBorder(Graphics graph, RectangleF rect, PaintEventArgs e, bool selected) {
				base.DrawBorder(graph, rect, e, false);
			}
		}
		class WizSummaryPreviewPainter : DevExpress.XtraPrinting.Native.PagePreviewPainterBase {
			SummaryEditorFormBase form;
			Font defaultFont = new Font(FontFamily.GenericSansSerif, 10);
			Font summaryFont = new Font(FontFamily.GenericSansSerif, 10, FontStyle.Bold);
			StringFormat stringFormat = StringFormat.GenericDefault.Clone() as StringFormat;
			public WizSummaryPreviewPainter(SummaryEditorFormBase form) {
				this.form = form;
				stringFormat.FormatFlags |= StringFormatFlags.NoWrap;
				stringFormat.FormatFlags &= ~StringFormatFlags.NoClip;
			}
			public override void Dispose() {
				base.Dispose();
				stringFormat.Dispose();
				defaultFont.Dispose();
				summaryFont.Dispose();
			}
			internal ArrayList PrepareSummaryValues(ArrayList values) {
				if (!form.chkIgnoreNullValues.Checked)
					return values;
				ArrayList result = new ArrayList();
				int count = values.Count;
				for (int i = 0; i < count; i++) {
					object obj = values[i];
					if (obj != null && !(obj is DBNull))
						result.Add(obj);
				}
				return result;
			}
			protected override void DrawImage(Graphics gr, int w, int h) {
				base.DrawImage(gr, w, h);
				ArrayList array = form.previewData;
				float x = 2 * padding;
				float y = 2 * padding;
				SizeF size = new SizeF(w - 2 * x, h - 2 * y);
				int count = array.Count;
				for (int i = 0; i < count; i++) {
					string str = form.FormatValueString(array[i], false);
					RectangleF rect = new RectangleF(new PointF(x, y), size);
					gr.DrawString(str, defaultFont, blackBrush, rect);
					y += gr.MeasureString(str, defaultFont, size, stringFormat).Height;
				}
				y += shadowWidth;
				gr.DrawLine(borderPen, x, y, w - x, y);
				y += shadowWidth;
				try {
					gr.DrawString(form.FormatValueString(form.CalcSummaryResult(PrepareSummaryValues(array)), true),
						summaryFont, blackBrush, new RectangleF(new PointF(x, y), size));
				}
				catch {
				}
			}
		}
		#endregion
		static object[] floatData = new object[] { 1.5f, null, 0.3f, -0.4f, 1.5f, 5f, 2.8f, 3.3f };
		static object[] intData = new Object[] { 5, null, 7, -2, 5, 4, 10, 3 };
		static object[] boolData = new Object[] { true, null, false, false, true, true, true, false };
		private DevExpress.XtraEditors.SimpleButton btnCancel;
		private PanelControl preview;
		private LabelControl bottomLine;
		private DevExpress.XtraEditors.SimpleButton btnOk;
		public LabelControl lblSummaryFunction;
		public LabelControl lblSummaryField;
		public DevExpress.XtraEditors.ComboBoxEdit cbSummaryFunction;
		public CheckEdit chkIgnoreNullValues;
		protected DevExpress.XtraEditors.PopupContainerEdit cbBoundField;
		PopupBindingPickerBase bindingPicker;
		private System.ComponentModel.IContainer components;
		ArrayList previewData = new ArrayList();
		protected IServiceProvider serviceProvider;
		WizSummaryPreviewPainter previewPainter;
		MyPrintControl pc;
		Link link;
		DevExpress.XtraPrinting.PrintingSystemBase ps;
		IDataContextService dataContextService;
		protected SummaryEditorFormBase(bool ignoreNullValues, IServiceProvider serviceProvider)
			: base(serviceProvider) {
			InitializeComponent();
			ps.PageSettings.Assign(
				new System.Drawing.Printing.Margins(20, 20, 20, 20),
				new System.Drawing.Printing.Margins(0, 0, 0, 0),
				PaperKind.Letter,
				new Size(850, 1100),
				false);
			this.serviceProvider = serviceProvider;
			this.dataContextService = (IDataContextService)serviceProvider.GetService(typeof(IDataContextService));
			previewPainter = new WizSummaryPreviewPainter(this);
			chkIgnoreNullValues.Checked = ignoreNullValues;
			cbBoundField.Properties.PopupControl = new PopupContainerControl();
			cbSummaryFunction.EditValueChanged += new System.EventHandler(this.cbSummaryFunction_EditValueChanged);
			chkIgnoreNullValues.CheckedChanged += new System.EventHandler(this.chkIgnoreNullValues_CheckedChanged);
		}
		protected SummaryEditorFormBase() {
			InitializeComponent();
		}
		public bool IgnoreNullValues {
			get { return chkIgnoreNullValues.Checked; }
		}
		public virtual string FormatString {
			get { return string.Empty; }
		}
		protected PopupBindingPickerBase BindingPicker { get { return bindingPicker; } }
		private void InitializeComponent() {
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SummaryEditorFormBase));
			this.lblSummaryField = new DevExpress.XtraEditors.LabelControl();
			this.btnCancel = new DevExpress.XtraEditors.SimpleButton();
			this.preview = new DevExpress.XtraEditors.PanelControl();
			this.pc = new DevExpress.XtraReports.Design.SummaryEditorFormBase.MyPrintControl();
			this.link = new DevExpress.XtraPrinting.Link(this.components);
			this.ps = new DevExpress.XtraPrinting.PrintingSystemBase(this.components);
			this.bottomLine = new DevExpress.XtraEditors.LabelControl();
			this.btnOk = new DevExpress.XtraEditors.SimpleButton();
			this.lblSummaryFunction = new DevExpress.XtraEditors.LabelControl();
			this.cbSummaryFunction = new DevExpress.XtraEditors.ComboBoxEdit();
			this.chkIgnoreNullValues = new DevExpress.XtraEditors.CheckEdit();
			this.cbBoundField = new DevExpress.XtraEditors.PopupContainerEdit();
			((System.ComponentModel.ISupportInitialize)(this.preview)).BeginInit();
			this.preview.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.link.ImageCollection)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.cbSummaryFunction.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.chkIgnoreNullValues.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.cbBoundField.Properties)).BeginInit();
			this.SuspendLayout();
			resources.ApplyResources(this.lblSummaryField, "lblSummaryField");
			this.lblSummaryField.Name = "lblSummaryField";
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			resources.ApplyResources(this.btnCancel, "btnCancel");
			this.btnCancel.Name = "btnCancel";
			this.preview.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.Simple;
			this.preview.Controls.Add(this.pc);
			resources.ApplyResources(this.preview, "preview");
			this.preview.Name = "preview";
			resources.ApplyResources(this.pc, "pc");
			this.pc.Name = "pc";
			this.pc.ShowPageMargins = false;
			this.pc.TabStop = false;
			this.pc.Zoom = 0.17F;
			this.pc.Document = this.ps;
			this.link.ImageCollection.ImageStream = ((DevExpress.Utils.ImageCollectionStreamer)(resources.GetObject("link.ImageCollection.ImageStream")));
			this.link.Margins = new System.Drawing.Printing.Margins(20, 20, 20, 20);
			this.link.PrintingSystemBase = ps;
			this.link.CreateDetailArea += new DevExpress.XtraPrinting.CreateAreaEventHandler(this.link_CreateDetailArea);
			resources.ApplyResources(this.bottomLine, "bottomLine");
			this.bottomLine.LineOrientation = DevExpress.XtraEditors.LabelLineOrientation.Horizontal;
			this.bottomLine.LineVisible = true;
			this.bottomLine.Name = "bottomLine";
			this.btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
			resources.ApplyResources(this.btnOk, "btnOk");
			this.btnOk.Name = "btnOk";
			this.btnOk.Click += new EventHandler(btnOk_Click);
			resources.ApplyResources(this.lblSummaryFunction, "lblSummaryFunction");
			this.lblSummaryFunction.Name = "lblSummaryFunction";
			resources.ApplyResources(this.cbSummaryFunction, "cbSummaryFunction");
			this.cbSummaryFunction.Name = "cbSummaryFunction";
			this.cbSummaryFunction.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("cbSummaryFunction.Properties.Buttons"))))});
			this.cbSummaryFunction.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
			resources.ApplyResources(this.chkIgnoreNullValues, "chkIgnoreNullValues");
			this.chkIgnoreNullValues.Name = "chkIgnoreNullValues";
			this.chkIgnoreNullValues.Properties.Caption = resources.GetString("chkIgnoreNullValues.Properties.Caption");
			resources.ApplyResources(this.cbBoundField, "cbBoundField");
			this.cbBoundField.Name = "cbBoundField";
			this.cbBoundField.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("cbBoundField.Properties.Buttons"))))});
			this.cbBoundField.Closed += new DevExpress.XtraEditors.Controls.ClosedEventHandler(this.cbBoundField_Closed);
			this.cbBoundField.QueryPopUp += new System.ComponentModel.CancelEventHandler(this.cbBoundField_QueryPopUp);
			this.AcceptButton = this.btnOk;
			resources.ApplyResources(this, "$this");
			this.CancelButton = this.btnCancel;
			this.Controls.Add(this.cbBoundField);
			this.Controls.Add(this.chkIgnoreNullValues);
			this.Controls.Add(this.cbSummaryFunction);
			this.Controls.Add(this.preview);
			this.Controls.Add(this.bottomLine);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.btnOk);
			this.Controls.Add(this.lblSummaryField);
			this.Controls.Add(this.lblSummaryFunction);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "SummaryEditorFormBase";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			((System.ComponentModel.ISupportInitialize)(this.preview)).EndInit();
			this.preview.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.link.ImageCollection)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.cbSummaryFunction.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chkIgnoreNullValues.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.cbBoundField.Properties)).EndInit();
			this.ResumeLayout(false);
		}
		void btnOk_Click(object sender, EventArgs e) {
			ValidateFormatString();
		}
		protected virtual void ValidateFormatString() {
		}
		string FormatValueString(object val, bool applyFormatString) {
			string str;
			if (val == null)
				str = "<null>";
			else if (applyFormatString && !String.IsNullOrEmpty(GetFormatString())) {
				try {
					str = String.Format(GetFormatString(), val);
				} catch {
					str = val.ToString();
				}
			} else
				str = val.ToString();
			return str;
		}
		protected virtual string GetFormatString() {
			return FormatString;
		}
		void link_CreateDetailArea(object sender, CreateAreaEventArgs e) {
			int heigthOfBrick = XRConvert.Convert(80, GraphicsDpi.DeviceIndependentPixel, GraphicsDpi.Pixel);
			int widhtOfPage = 775;
			int heigthOfSummaryBrick = XRConvert.Convert(325, GraphicsDpi.DeviceIndependentPixel, GraphicsDpi.Pixel);
			Font brickFont = new Font(FontFamily.GenericSansSerif, 56);
			for (int i = 0; i < this.previewData.Count; i++) {
				TextBrick brick = new TextBrick();
				brick.Text = this.previewData[i] != null ? this.previewData[i].ToString() : "<null>";
				brick.Rect = new RectangleF(0, 0 + i * heigthOfBrick, widhtOfPage, heigthOfBrick);
				brick.BorderColor = Color.Transparent;
				brick.ForeColor = Color.Black;
				brick.Font = brickFont;
				e.Graph.DrawBrick(brick);
			}
			LineBrick lb = new LineBrick();
			lb.LineDirection = LineDirection.Horizontal;
			lb.BorderColor = Color.Transparent;
			lb.Rect = new RectangleF(0, 0 + this.previewData.Count * heigthOfBrick, widhtOfPage, heigthOfBrick);
			e.Graph.DrawBrick(lb);
			try {
				TextBrick tb = new TextBrick();
				tb.Text = GetResultString(CalcSummaryResult(this.previewPainter.PrepareSummaryValues(this.previewData)));
				tb.Rect = new RectangleF(0, 50 + this.previewData.Count * heigthOfBrick, widhtOfPage, heigthOfSummaryBrick);
				tb.BorderColor = Color.Transparent;
				tb.Font = brickFont;
				e.Graph.DrawBrick(tb);
			} catch {
			}
		}
		protected virtual string GetResultString(object val) {
			return this.FormatValueString(val, true);
		}
		void cbSummaryFunction_EditValueChanged(object sender, System.EventArgs e) {
			UpdatePreview();
		}
		void chkIgnoreNullValues_CheckedChanged(object sender, System.EventArgs e) {
			UpdatePreview();
		}
		void UpdatePreviewData() {
			using (DataContext dataContext = dataContextService.CreateDataContext(new DataContextOptions(true, true), false)) {
				try {
					previewData.Clear();
					Type type = GetPropertyType(dataContext);
					if (type == null)
						previewData.AddRange(intData);
					else if (type.Equals(typeof(System.Single)) || type.Equals(typeof(System.Single?)) || type.Equals(typeof(System.Double)) || type.Equals(typeof(System.Double?)))
						previewData.AddRange(floatData);
					else if (type.Equals(typeof(System.Boolean)) || type.Equals(typeof(bool?)))
						previewData.AddRange(boolData);
					else
						previewData.AddRange(intData);
				}
				catch {
					previewData.Clear();
					previewData.AddRange(intData);
				}
			}
		}
		protected override void Dispose(bool disposing) {
			if (disposing) {
				if(components != null) {
					components.Dispose();
				}
				if(previewPainter != null)
					previewPainter.Dispose();
			}
			base.Dispose(disposing);
		}
		protected abstract object CalcSummaryResult(ArrayList values);
		protected void UpdatePreview() {
			UpdatePreviewData();
			this.link.CreateDocument();
		}
		protected void UpdateAcceptButtonDialogResult() {
			this.btnOk.DialogResult = GetAcceptButtonDialogResult();
		}
		protected virtual DialogResult GetAcceptButtonDialogResult() {
			return DialogResult.OK;
		}
		#region field name edit
		void cbBoundField_QueryPopUp(object sender, System.ComponentModel.CancelEventArgs e) {
			bool cancel = true;
			try {
				if (bindingPicker == null || bindingPicker.IsDisposed) {
					bindingPicker = CreatePopupBindingPicker();
					bindingPicker.Width = cbBoundField.Width;
					StartFieldNameEdit();
				}
				cancel = false;
			}
			finally {
				e.Cancel = cancel;
			}
		}
		void cbBoundField_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e) {
			if (bindingPicker != null) {
				EndFieldNameEdit();
				bindingPicker.Dispose();
			}
		}
		protected virtual void StartFieldNameEdit() {
		}
		protected virtual void EndFieldNameEdit() {
		}
		protected virtual Type GetPropertyType(DataContext dataContext) {
			return null;
		}
		protected virtual PopupBindingPickerBase CreatePopupBindingPicker() {
			return null;
		}
		#endregion
	}
}
