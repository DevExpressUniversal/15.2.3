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
using System.Drawing;
using System.Collections;
using DevExpress.XtraPrinting.Control;
using DevExpress.XtraPrinting.Localization;
using DevExpress.LookAndFeel;
namespace DevExpress.XtraPrinting.Native.WinControls {
	using System.Windows.Forms;
	using DevExpress.XtraEditors;
	using DevExpress.XtraPrinting.Drawing;
	using DevExpress.DocumentView;
	using System.IO;
	using DevExpress.XtraLayout;
	public class WatermarkEditorForm : XtraForm {
		#region inner classes
		public class DirectionModeItem {
			string text;
			DirectionMode directionMode;
			public DirectionMode DirectionMode {
				get { return directionMode; }
			}
			public string Text {
				get { return text; }
			}
			public DirectionModeItem(DirectionMode directionMode, string text) {
				this.directionMode = directionMode;
				this.text = text;
			}
		}
		public class ViewModeItem {
			ImageViewMode viewMode;
			string text;
			public ImageViewMode ViewMode {
				get { return viewMode; }
			}
			public string Text {
				get { return text; }
			}
			public ViewModeItem(ImageViewMode viewMode, string text) {
				this.viewMode = viewMode;
				this.text = text;
			}
		}
		public class ImageAlignItem {
			string text;
			string alignment;
			public string Alignment {
				get { return alignment; }
			}
			public string Text {
				get { return text; }
			}
			public ImageAlignItem(string alignment, string text) {
				this.alignment = alignment;
				this.text = text;
			}
		}
		private class MyPrintControl : DevExpress.DocumentView.Controls.DocumentViewerBase {
			private DevExpress.XtraPrinting.PrintingSystemBase ps;
			public MyPrintControl() {
				SetControlVisibility(new Control[] { vScrollBar, bottomPanel }, false);
				ps = new DevExpress.XtraPrinting.PrintingSystemBase();
				Document = ps;
				fMinZoom = 0.00001f;
				MakeViewControlTransparent();
			}
			void SetControlVisibility(Control[] controls, bool visible) {
				foreach(Control control in controls)
					control.Visible = visible;
			}
			protected override void DrawBorder(Graphics graph, RectangleF rect, PaintEventArgs e, bool selected) {
				base.DrawBorder(graph, rect, e, false);
			}
			protected override void OnHandleCreated(EventArgs e) {
				base.OnHandleCreated(e);
				CreateDocument();
				ViewWholePage();
			}
			protected override bool HandleKey(Keys key) {
				return true;
			}
			private void CreateDocument() {
				ps.Begin();
				ps.Graph.Modifier = BrickModifier.Detail;
				EmptyBrick brick = new EmptyBrick();
				brick.Rect = new RectangleF(0, 0, 100, 100);
				ps.Graph.DrawBrick(brick);
				ps.End();
			}
			public void Update(Watermark watermark) {
				ps.Watermark.CopyFrom(watermark);
				ps.Watermark.PageRange = "";
				Invalidate(true);
			}
			protected override void Dispose(bool disposing) {
				if(disposing) {
					ps.Dispose();
				}
				base.Dispose(disposing);
			}
			void MakeViewControlTransparent() {
				ViewControl.EnablePaintBackground = true;
				ViewControl.BackColor = Color.Empty;
			}
			protected override BackgroundPreviewPainter CreateBackgroundPreviewPainter() {
				return null;
			}
		}
		#endregion
		#region const
		const string defaultFilterText = "All Picture Files{0}"; 
		const string alignTop = "Top", alignMiddle = "Middle", alignBottom = "Bottom", alignLeft = "Left", alignCenter = "Center", alignRight = "Right";
		static string[]
			alignList = new string[] { ToString(alignBottom, alignCenter), 
										 ToString(alignBottom, alignLeft),
										 ToString(alignBottom, alignRight),
										 ToString(alignMiddle, alignCenter),
										 ToString(alignMiddle, alignLeft),
										 ToString(alignMiddle, alignRight),
										 ToString(alignTop, alignCenter),
										 ToString(alignTop, alignLeft),
										 ToString(alignTop, alignRight)};
		static ContentAlignment[] contentAlignList = new ContentAlignment[] {
			ContentAlignment.BottomCenter,
			ContentAlignment.BottomLeft,
			ContentAlignment.BottomRight,
			ContentAlignment.MiddleCenter,
			ContentAlignment.MiddleLeft,
			ContentAlignment.MiddleRight,
			ContentAlignment.TopCenter,
			ContentAlignment.TopLeft,
			ContentAlignment.TopRight};
		#endregion
		static string ToString(string vAlign, string hAlign) {
			return String.Format("{0},{1}", hAlign, vAlign);
		}
		#region fields
		bool isRTLChanged = false;
		private XtraLayout.LayoutControl layoutControl1;
		private XtraLayout.LayoutControlGroup layoutControlGroup1;
		private XtraLayout.TabbedControlGroup layoutControlItem1;
		private XtraLayout.LayoutControlGroup grpBoxZOrder;
		private XtraLayout.LayoutControlItem layoutControlItem2;
		private XtraLayout.LayoutControlItem layoutControlItem3;
		private XtraLayout.LayoutControlItem layoutControlItem4;
		private XtraLayout.LayoutControlGroup grpBoxPageRange;
		private XtraLayout.LayoutControlItem layoutControlItem22;
		private XtraLayout.LayoutControlItem layoutControlItem23;
		private XtraLayout.LayoutControlItem layoutControlItem24;
		private XtraLayout.LayoutControlGroup grpButtons;
		private LabelControl lbPictureTransparency;
		private XtraLayout.LayoutControlGroup tpTextWaterMark;
		private XtraLayout.LayoutControlItem layoutControlItem5;
		private XtraLayout.LayoutControlGroup tpPictureWatermark;
		private CheckEdit rbBehind;
		private CheckEdit rbInFront;
		private XtraLayout.LayoutControlItem layoutControlItem7;
		private XtraLayout.LayoutControlItem layoutControlItem9;
		private LabelControl lbPageRange;
		private XtraLayout.LayoutControlItem layoutControlItem10;
		private LabelControl labelControl5;
		private XtraLayout.LayoutControlItem layoutControlItem12;
		private PanelControl panelControl2;
		private MyPrintControl myPrintControl1;
		private XtraLayout.LayoutControlItem layoutControlItem8;
		private XtraLayout.LayoutControlGroup layoutControlGroup8;
		private LabelControl lbTextTransparency;
		private XtraLayout.LayoutControlGroup layoutControlGroup9;
		private XtraLayout.LayoutControlItem layoutControlItem11;
		private TextEdit teTransparentValue;
		private XtraLayout.LayoutControlItem layoutControlItem13;
		private TrackBarControl trBarTextTransparency;
		private XtraLayout.LayoutControlItem layoutControlItem14;
		private TrackBarControl trBarImageTransparency;
		private TextEdit teImageTransparentValue;
		private XtraLayout.LayoutControlItem layoutControlItem15;
		private XtraLayout.LayoutControlItem layoutControlItem16;
		private XtraLayout.LayoutControlGroup layoutControlGroup10;
		private LabelControl lbFontColor;
		private LabelControl lbFontSize;
		private XtraLayout.LayoutControlItem layoutControlItem20;
		private XtraLayout.LayoutControlItem layoutControlItem21;
		private ComboBoxEdit cmbWatermarkText;
		private XtraLayout.LayoutControlItem layoutControlItem25;
		private LookUpEdit lkpTextDirection;
		private XtraLayout.LayoutControlItem layoutControlItem26;
		private ComboBoxEdit cmbWatermarkFontSize;
		private XtraLayout.LayoutControlItem layoutControlItem27;
		private ColorEdit ceWatermarkColor;
		private ComboBoxEdit cmbWatermarkFont;
		private XtraLayout.LayoutControlItem layoutControlItem28;
		private XtraLayout.LayoutControlItem layoutControlItem29;
		private CheckEdit chbItalic;
		private CheckEdit chbBold;
		private XtraLayout.LayoutControlItem layoutControlItem30;
		private XtraLayout.LayoutControlItem layoutControlItem31;
		private XtraLayout.LayoutControlGroup layoutControlGroup11;
		private LabelControl lbText;
		private LabelControl lbLayout;
		private LabelControl lbFont;
		private XtraLayout.LayoutControlItem layoutControlItem18;
		private XtraLayout.LayoutControlItem layoutControlItem19;
		private XtraLayout.LayoutControlItem layoutControlItem32;
		private LabelControl lbPosition;
		private LabelControl lbHorzAlign;
		private LabelControl lbVertAlign;
		private XtraLayout.LayoutControlGroup layoutControlGroup7;
		private XtraLayout.LayoutControlItem layoutControlItem6;
		private XtraLayout.LayoutControlItem layoutControlItem17;
		private XtraLayout.LayoutControlItem layoutControlItem34;
		private CheckEdit chbTiling;
		private LookUpEdit lkpImageVAlign;
		private LookUpEdit lkpImageHAlign;
		private LookUpEdit lkpImageView;
		private XtraLayout.LayoutControlItem layoutControlItem37;
		private XtraLayout.LayoutControlItem layoutControlItem38;
		private XtraLayout.LayoutControlGroup layoutControlGroup12;
		private XtraLayout.LayoutControlItem layoutControlItem39;
		private XtraLayout.LayoutControlItem layoutControlItem36;
		private SimpleButton btnSelectPicture;
		private LabelControl lbLoadImage;
		private XtraLayout.LayoutControlItem layoutControlItem33;
		private XtraLayout.LayoutControlItem layoutControlItem35;
		private LabelControl emptySpaceLabelControl2;
		private LabelControl emptySpaceLabelControl3;
		private XtraLayout.LayoutControlItem emptySpaceLabelControl1;
		private XtraLayout.LayoutControlItem layoutControlItem40;
		private XtraLayout.LayoutControlItem layoutControlItem41;
		private LabelControl emptySpaceLabelControl4;
		private XtraLayout.LayoutControlItem layoutControlItem43;
		private LabelControl emptySpaceLabelControl7;
		private XtraLayout.LayoutControlItem layoutControlItem46;
		private LabelControl labelControl2;
		private XtraLayout.LayoutControlItem layoutControlItem47;
		private DirectionModeItem[] dsDirectionMode;
		private ViewModeItem[] dsImageViewMode;
		private ImageAlignItem[] dsImageHAlign;
		private ImageAlignItem[] dsImageVAlign;
		private System.ComponentModel.IContainer components = null;
		private DevExpress.XtraEditors.SimpleButton btnOK;
		private DevExpress.XtraEditors.TextEdit tePageRange;
		private Watermark watermark = null;
		private DevExpress.XtraEditors.SimpleButton btnCancel;
		private DevExpress.XtraEditors.SimpleButton btnClear;
		private DevExpress.XtraEditors.CheckEdit rbAll;
		private DevExpress.XtraEditors.CheckEdit rbPages;
		private bool canSync = false;
		string filterText = defaultFilterText;
		#endregion
		public Watermark Watermark {
			get { return watermark; }
		}
		public XtraPageSettingsBase PageSettings {
			get { return ((PrintingSystemBase)myPrintControl1.Document).PageSettings; }
		}
		public WatermarkEditorForm() {
			InitializeComponent();
			this.filterText = new System.ComponentModel.ComponentResourceManager(typeof(WatermarkEditorForm)).GetString("openFileFilter.Text");
			this.watermark = new Watermark();
			InitComboBoxes();
			EditorContextMenuLookAndFeelHelper.InitBarManager(ref this.components, this);
			canSync = true;
		}
		public new DialogResult ShowDialog() {
			return Native.DialogRunner.ShowDialog(this);
		}
		protected override void Dispose(bool disposing) {
			if(disposing) {
				if(components != null)
					components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Windows Form Designer generated code
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(WatermarkEditorForm));
			DevExpress.XtraLayout.ColumnDefinition columnDefinition43 = new DevExpress.XtraLayout.ColumnDefinition();
			DevExpress.XtraLayout.ColumnDefinition columnDefinition44 = new DevExpress.XtraLayout.ColumnDefinition();
			DevExpress.XtraLayout.ColumnDefinition columnDefinition45 = new DevExpress.XtraLayout.ColumnDefinition();
			DevExpress.XtraLayout.ColumnDefinition columnDefinition46 = new DevExpress.XtraLayout.ColumnDefinition();
			DevExpress.XtraLayout.RowDefinition rowDefinition35 = new DevExpress.XtraLayout.RowDefinition();
			DevExpress.XtraLayout.RowDefinition rowDefinition36 = new DevExpress.XtraLayout.RowDefinition();
			DevExpress.XtraLayout.RowDefinition rowDefinition37 = new DevExpress.XtraLayout.RowDefinition();
			DevExpress.XtraLayout.ColumnDefinition columnDefinition17 = new DevExpress.XtraLayout.ColumnDefinition();
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
			DevExpress.XtraLayout.ColumnDefinition columnDefinition10 = new DevExpress.XtraLayout.ColumnDefinition();
			DevExpress.XtraLayout.ColumnDefinition columnDefinition11 = new DevExpress.XtraLayout.ColumnDefinition();
			DevExpress.XtraLayout.ColumnDefinition columnDefinition12 = new DevExpress.XtraLayout.ColumnDefinition();
			DevExpress.XtraLayout.ColumnDefinition columnDefinition13 = new DevExpress.XtraLayout.ColumnDefinition();
			DevExpress.XtraLayout.ColumnDefinition columnDefinition14 = new DevExpress.XtraLayout.ColumnDefinition();
			DevExpress.XtraLayout.ColumnDefinition columnDefinition15 = new DevExpress.XtraLayout.ColumnDefinition();
			DevExpress.XtraLayout.ColumnDefinition columnDefinition16 = new DevExpress.XtraLayout.ColumnDefinition();
			DevExpress.XtraLayout.RowDefinition rowDefinition6 = new DevExpress.XtraLayout.RowDefinition();
			DevExpress.XtraLayout.RowDefinition rowDefinition7 = new DevExpress.XtraLayout.RowDefinition();
			DevExpress.XtraLayout.RowDefinition rowDefinition8 = new DevExpress.XtraLayout.RowDefinition();
			DevExpress.XtraLayout.RowDefinition rowDefinition9 = new DevExpress.XtraLayout.RowDefinition();
			DevExpress.XtraLayout.RowDefinition rowDefinition10 = new DevExpress.XtraLayout.RowDefinition();
			DevExpress.XtraLayout.RowDefinition rowDefinition11 = new DevExpress.XtraLayout.RowDefinition();
			DevExpress.XtraLayout.ColumnDefinition columnDefinition6 = new DevExpress.XtraLayout.ColumnDefinition();
			DevExpress.XtraLayout.ColumnDefinition columnDefinition7 = new DevExpress.XtraLayout.ColumnDefinition();
			DevExpress.XtraLayout.ColumnDefinition columnDefinition8 = new DevExpress.XtraLayout.ColumnDefinition();
			DevExpress.XtraLayout.ColumnDefinition columnDefinition9 = new DevExpress.XtraLayout.ColumnDefinition();
			DevExpress.XtraLayout.RowDefinition rowDefinition4 = new DevExpress.XtraLayout.RowDefinition();
			DevExpress.XtraLayout.RowDefinition rowDefinition5 = new DevExpress.XtraLayout.RowDefinition();
			DevExpress.XtraLayout.ColumnDefinition columnDefinition29 = new DevExpress.XtraLayout.ColumnDefinition();
			DevExpress.XtraLayout.RowDefinition rowDefinition24 = new DevExpress.XtraLayout.RowDefinition();
			DevExpress.XtraLayout.RowDefinition rowDefinition25 = new DevExpress.XtraLayout.RowDefinition();
			DevExpress.XtraLayout.ColumnDefinition columnDefinition18 = new DevExpress.XtraLayout.ColumnDefinition();
			DevExpress.XtraLayout.ColumnDefinition columnDefinition19 = new DevExpress.XtraLayout.ColumnDefinition();
			DevExpress.XtraLayout.ColumnDefinition columnDefinition20 = new DevExpress.XtraLayout.ColumnDefinition();
			DevExpress.XtraLayout.ColumnDefinition columnDefinition21 = new DevExpress.XtraLayout.ColumnDefinition();
			DevExpress.XtraLayout.ColumnDefinition columnDefinition22 = new DevExpress.XtraLayout.ColumnDefinition();
			DevExpress.XtraLayout.RowDefinition rowDefinition14 = new DevExpress.XtraLayout.RowDefinition();
			DevExpress.XtraLayout.RowDefinition rowDefinition15 = new DevExpress.XtraLayout.RowDefinition();
			DevExpress.XtraLayout.ColumnDefinition columnDefinition25 = new DevExpress.XtraLayout.ColumnDefinition();
			DevExpress.XtraLayout.ColumnDefinition columnDefinition26 = new DevExpress.XtraLayout.ColumnDefinition();
			DevExpress.XtraLayout.ColumnDefinition columnDefinition27 = new DevExpress.XtraLayout.ColumnDefinition();
			DevExpress.XtraLayout.ColumnDefinition columnDefinition28 = new DevExpress.XtraLayout.ColumnDefinition();
			DevExpress.XtraLayout.RowDefinition rowDefinition18 = new DevExpress.XtraLayout.RowDefinition();
			DevExpress.XtraLayout.RowDefinition rowDefinition19 = new DevExpress.XtraLayout.RowDefinition();
			DevExpress.XtraLayout.RowDefinition rowDefinition20 = new DevExpress.XtraLayout.RowDefinition();
			DevExpress.XtraLayout.RowDefinition rowDefinition21 = new DevExpress.XtraLayout.RowDefinition();
			DevExpress.XtraLayout.RowDefinition rowDefinition22 = new DevExpress.XtraLayout.RowDefinition();
			DevExpress.XtraLayout.RowDefinition rowDefinition23 = new DevExpress.XtraLayout.RowDefinition();
			DevExpress.XtraLayout.ColumnDefinition columnDefinition23 = new DevExpress.XtraLayout.ColumnDefinition();
			DevExpress.XtraLayout.ColumnDefinition columnDefinition24 = new DevExpress.XtraLayout.ColumnDefinition();
			DevExpress.XtraLayout.RowDefinition rowDefinition16 = new DevExpress.XtraLayout.RowDefinition();
			DevExpress.XtraLayout.RowDefinition rowDefinition17 = new DevExpress.XtraLayout.RowDefinition();
			DevExpress.XtraLayout.ColumnDefinition columnDefinition30 = new DevExpress.XtraLayout.ColumnDefinition();
			DevExpress.XtraLayout.RowDefinition rowDefinition26 = new DevExpress.XtraLayout.RowDefinition();
			DevExpress.XtraLayout.RowDefinition rowDefinition27 = new DevExpress.XtraLayout.RowDefinition();
			DevExpress.XtraLayout.RowDefinition rowDefinition28 = new DevExpress.XtraLayout.RowDefinition();
			DevExpress.XtraLayout.ColumnDefinition columnDefinition31 = new DevExpress.XtraLayout.ColumnDefinition();
			DevExpress.XtraLayout.ColumnDefinition columnDefinition32 = new DevExpress.XtraLayout.ColumnDefinition();
			DevExpress.XtraLayout.ColumnDefinition columnDefinition33 = new DevExpress.XtraLayout.ColumnDefinition();
			DevExpress.XtraLayout.ColumnDefinition columnDefinition34 = new DevExpress.XtraLayout.ColumnDefinition();
			DevExpress.XtraLayout.ColumnDefinition columnDefinition35 = new DevExpress.XtraLayout.ColumnDefinition();
			DevExpress.XtraLayout.RowDefinition rowDefinition29 = new DevExpress.XtraLayout.RowDefinition();
			DevExpress.XtraLayout.RowDefinition rowDefinition30 = new DevExpress.XtraLayout.RowDefinition();
			DevExpress.XtraLayout.RowDefinition rowDefinition31 = new DevExpress.XtraLayout.RowDefinition();
			DevExpress.XtraLayout.RowDefinition rowDefinition32 = new DevExpress.XtraLayout.RowDefinition();
			DevExpress.XtraLayout.ColumnDefinition columnDefinition36 = new DevExpress.XtraLayout.ColumnDefinition();
			DevExpress.XtraLayout.ColumnDefinition columnDefinition37 = new DevExpress.XtraLayout.ColumnDefinition();
			DevExpress.XtraLayout.ColumnDefinition columnDefinition38 = new DevExpress.XtraLayout.ColumnDefinition();
			DevExpress.XtraLayout.ColumnDefinition columnDefinition39 = new DevExpress.XtraLayout.ColumnDefinition();
			DevExpress.XtraLayout.ColumnDefinition columnDefinition40 = new DevExpress.XtraLayout.ColumnDefinition();
			DevExpress.XtraLayout.ColumnDefinition columnDefinition41 = new DevExpress.XtraLayout.ColumnDefinition();
			DevExpress.XtraLayout.ColumnDefinition columnDefinition42 = new DevExpress.XtraLayout.ColumnDefinition();
			DevExpress.XtraLayout.RowDefinition rowDefinition33 = new DevExpress.XtraLayout.RowDefinition();
			DevExpress.XtraLayout.RowDefinition rowDefinition34 = new DevExpress.XtraLayout.RowDefinition();
			this.btnCancel = new DevExpress.XtraEditors.SimpleButton();
			this.layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
			this.labelControl2 = new DevExpress.XtraEditors.LabelControl();
			this.emptySpaceLabelControl3 = new DevExpress.XtraEditors.LabelControl();
			this.panelControl2 = new DevExpress.XtraEditors.PanelControl();
			this.myPrintControl1 = new DevExpress.XtraPrinting.Native.WinControls.WatermarkEditorForm.MyPrintControl();
			this.labelControl5 = new DevExpress.XtraEditors.LabelControl();
			this.lbPageRange = new DevExpress.XtraEditors.LabelControl();
			this.rbBehind = new DevExpress.XtraEditors.CheckEdit();
			this.rbInFront = new DevExpress.XtraEditors.CheckEdit();
			this.btnClear = new DevExpress.XtraEditors.SimpleButton();
			this.btnOK = new DevExpress.XtraEditors.SimpleButton();
			this.tePageRange = new DevExpress.XtraEditors.TextEdit();
			this.rbAll = new DevExpress.XtraEditors.CheckEdit();
			this.rbPages = new DevExpress.XtraEditors.CheckEdit();
			this.emptySpaceLabelControl4 = new DevExpress.XtraEditors.LabelControl();
			this.emptySpaceLabelControl2 = new DevExpress.XtraEditors.LabelControl();
			this.lbText = new DevExpress.XtraEditors.LabelControl();
			this.lbLayout = new DevExpress.XtraEditors.LabelControl();
			this.lbFont = new DevExpress.XtraEditors.LabelControl();
			this.chbItalic = new DevExpress.XtraEditors.CheckEdit();
			this.chbBold = new DevExpress.XtraEditors.CheckEdit();
			this.ceWatermarkColor = new DevExpress.XtraEditors.ColorEdit();
			this.cmbWatermarkFont = new DevExpress.XtraEditors.ComboBoxEdit();
			this.cmbWatermarkFontSize = new DevExpress.XtraEditors.ComboBoxEdit();
			this.lkpTextDirection = new DevExpress.XtraEditors.LookUpEdit();
			this.cmbWatermarkText = new DevExpress.XtraEditors.ComboBoxEdit();
			this.lbFontColor = new DevExpress.XtraEditors.LabelControl();
			this.lbFontSize = new DevExpress.XtraEditors.LabelControl();
			this.trBarTextTransparency = new DevExpress.XtraEditors.TrackBarControl();
			this.teTransparentValue = new DevExpress.XtraEditors.TextEdit();
			this.lbTextTransparency = new DevExpress.XtraEditors.LabelControl();
			this.emptySpaceLabelControl7 = new DevExpress.XtraEditors.LabelControl();
			this.btnSelectPicture = new DevExpress.XtraEditors.SimpleButton();
			this.lbLoadImage = new DevExpress.XtraEditors.LabelControl();
			this.chbTiling = new DevExpress.XtraEditors.CheckEdit();
			this.lkpImageVAlign = new DevExpress.XtraEditors.LookUpEdit();
			this.lkpImageHAlign = new DevExpress.XtraEditors.LookUpEdit();
			this.lkpImageView = new DevExpress.XtraEditors.LookUpEdit();
			this.lbPosition = new DevExpress.XtraEditors.LabelControl();
			this.lbHorzAlign = new DevExpress.XtraEditors.LabelControl();
			this.lbVertAlign = new DevExpress.XtraEditors.LabelControl();
			this.trBarImageTransparency = new DevExpress.XtraEditors.TrackBarControl();
			this.teImageTransparentValue = new DevExpress.XtraEditors.TextEdit();
			this.lbPictureTransparency = new DevExpress.XtraEditors.LabelControl();
			this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
			this.layoutControlItem1 = new DevExpress.XtraLayout.TabbedControlGroup();
			this.tpTextWaterMark = new DevExpress.XtraLayout.LayoutControlGroup();
			this.layoutControlGroup9 = new DevExpress.XtraLayout.LayoutControlGroup();
			this.layoutControlItem11 = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutControlItem13 = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutControlItem14 = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutControlGroup10 = new DevExpress.XtraLayout.LayoutControlGroup();
			this.layoutControlItem20 = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutControlItem21 = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutControlItem25 = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutControlItem26 = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutControlItem27 = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutControlItem28 = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutControlItem29 = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutControlGroup11 = new DevExpress.XtraLayout.LayoutControlGroup();
			this.layoutControlItem30 = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutControlItem31 = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutControlItem19 = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutControlItem32 = new DevExpress.XtraLayout.LayoutControlItem();
			this.emptySpaceLabelControl1 = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutControlItem43 = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutControlItem18 = new DevExpress.XtraLayout.LayoutControlItem();
			this.tpPictureWatermark = new DevExpress.XtraLayout.LayoutControlGroup();
			this.layoutControlGroup8 = new DevExpress.XtraLayout.LayoutControlGroup();
			this.layoutControlItem5 = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutControlItem15 = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutControlItem16 = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutControlGroup7 = new DevExpress.XtraLayout.LayoutControlGroup();
			this.layoutControlItem17 = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutControlItem34 = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutControlItem37 = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutControlItem38 = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutControlGroup12 = new DevExpress.XtraLayout.LayoutControlGroup();
			this.layoutControlItem39 = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutControlItem36 = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutControlItem6 = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutControlItem33 = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutControlItem35 = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutControlItem46 = new DevExpress.XtraLayout.LayoutControlItem();
			this.grpBoxZOrder = new DevExpress.XtraLayout.LayoutControlGroup();
			this.layoutControlItem7 = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutControlItem9 = new DevExpress.XtraLayout.LayoutControlItem();
			this.grpBoxPageRange = new DevExpress.XtraLayout.LayoutControlGroup();
			this.layoutControlItem22 = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutControlItem23 = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutControlItem24 = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutControlItem10 = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutControlItem47 = new DevExpress.XtraLayout.LayoutControlItem();
			this.grpButtons = new DevExpress.XtraLayout.LayoutControlGroup();
			this.layoutControlItem4 = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutControlItem3 = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutControlItem12 = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutControlItem40 = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutControlItem8 = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutControlItem41 = new DevExpress.XtraLayout.LayoutControlItem();
			((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
			this.layoutControl1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.panelControl2)).BeginInit();
			this.panelControl2.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.rbBehind.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.rbInFront.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.tePageRange.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.rbAll.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.rbPages.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.chbItalic.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.chbBold.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.ceWatermarkColor.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.cmbWatermarkFont.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.cmbWatermarkFontSize.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.lkpTextDirection.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.cmbWatermarkText.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.trBarTextTransparency)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.trBarTextTransparency.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.teTransparentValue.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.chbTiling.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.lkpImageVAlign.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.lkpImageHAlign.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.lkpImageView.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.trBarImageTransparency)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.trBarImageTransparency.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.teImageTransparentValue.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.tpTextWaterMark)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup9)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem11)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem13)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem14)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup10)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem20)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem21)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem25)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem26)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem27)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem28)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem29)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup11)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem30)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem31)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem19)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem32)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.emptySpaceLabelControl1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem43)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem18)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.tpPictureWatermark)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup8)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem5)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem15)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem16)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup7)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem17)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem34)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem37)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem38)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup12)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem39)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem36)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem6)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem33)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem35)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem46)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.grpBoxZOrder)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem7)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem9)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.grpBoxPageRange)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem22)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem23)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem24)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem10)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem47)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.grpButtons)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem4)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem12)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem40)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem8)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem41)).BeginInit();
			this.SuspendLayout();
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			resources.ApplyResources(this.btnCancel, "btnCancel");
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.StyleController = this.layoutControl1;
			this.layoutControl1.AllowCustomization = false;
			this.layoutControl1.Controls.Add(this.labelControl2);
			this.layoutControl1.Controls.Add(this.emptySpaceLabelControl3);
			this.layoutControl1.Controls.Add(this.panelControl2);
			this.layoutControl1.Controls.Add(this.labelControl5);
			this.layoutControl1.Controls.Add(this.lbPageRange);
			this.layoutControl1.Controls.Add(this.rbBehind);
			this.layoutControl1.Controls.Add(this.rbInFront);
			this.layoutControl1.Controls.Add(this.btnClear);
			this.layoutControl1.Controls.Add(this.btnCancel);
			this.layoutControl1.Controls.Add(this.btnOK);
			this.layoutControl1.Controls.Add(this.tePageRange);
			this.layoutControl1.Controls.Add(this.rbAll);
			this.layoutControl1.Controls.Add(this.rbPages);
			this.layoutControl1.Controls.Add(this.emptySpaceLabelControl4);
			this.layoutControl1.Controls.Add(this.emptySpaceLabelControl2);
			this.layoutControl1.Controls.Add(this.lbText);
			this.layoutControl1.Controls.Add(this.lbLayout);
			this.layoutControl1.Controls.Add(this.lbFont);
			this.layoutControl1.Controls.Add(this.chbItalic);
			this.layoutControl1.Controls.Add(this.chbBold);
			this.layoutControl1.Controls.Add(this.ceWatermarkColor);
			this.layoutControl1.Controls.Add(this.cmbWatermarkFont);
			this.layoutControl1.Controls.Add(this.cmbWatermarkFontSize);
			this.layoutControl1.Controls.Add(this.lkpTextDirection);
			this.layoutControl1.Controls.Add(this.cmbWatermarkText);
			this.layoutControl1.Controls.Add(this.lbFontColor);
			this.layoutControl1.Controls.Add(this.lbFontSize);
			this.layoutControl1.Controls.Add(this.trBarTextTransparency);
			this.layoutControl1.Controls.Add(this.teTransparentValue);
			this.layoutControl1.Controls.Add(this.lbTextTransparency);
			this.layoutControl1.Controls.Add(this.emptySpaceLabelControl7);
			this.layoutControl1.Controls.Add(this.btnSelectPicture);
			this.layoutControl1.Controls.Add(this.lbLoadImage);
			this.layoutControl1.Controls.Add(this.chbTiling);
			this.layoutControl1.Controls.Add(this.lkpImageVAlign);
			this.layoutControl1.Controls.Add(this.lkpImageHAlign);
			this.layoutControl1.Controls.Add(this.lkpImageView);
			this.layoutControl1.Controls.Add(this.lbPosition);
			this.layoutControl1.Controls.Add(this.lbHorzAlign);
			this.layoutControl1.Controls.Add(this.lbVertAlign);
			this.layoutControl1.Controls.Add(this.trBarImageTransparency);
			this.layoutControl1.Controls.Add(this.teImageTransparentValue);
			this.layoutControl1.Controls.Add(this.lbPictureTransparency);
			resources.ApplyResources(this.layoutControl1, "layoutControl1");
			this.layoutControl1.Name = "layoutControl1";
			this.layoutControl1.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(-968, 27, 871, 1040);
			this.layoutControl1.Root = this.layoutControlGroup1;
			resources.ApplyResources(this.labelControl2, "labelControl2");
			this.labelControl2.Name = "labelControl2";
			this.labelControl2.StyleController = this.layoutControl1;
			resources.ApplyResources(this.emptySpaceLabelControl3, "emptySpaceLabelControl3");
			this.emptySpaceLabelControl3.Name = "emptySpaceLabelControl3";
			this.emptySpaceLabelControl3.StyleController = this.layoutControl1;
			this.panelControl2.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.panelControl2.Controls.Add(this.myPrintControl1);
			resources.ApplyResources(this.panelControl2, "panelControl2");
			this.panelControl2.Name = "panelControl2";
			resources.ApplyResources(this.myPrintControl1, "myPrintControl1");
			this.myPrintControl1.Name = "myPrintControl1";
			this.myPrintControl1.ShowPageMargins = false;
			this.myPrintControl1.TabStop = false;
			this.myPrintControl1.Zoom = 0.2632576F;
			this.labelControl5.LineVisible = true;
			resources.ApplyResources(this.labelControl5, "labelControl5");
			this.labelControl5.Name = "labelControl5";
			this.labelControl5.StyleController = this.layoutControl1;
			resources.ApplyResources(this.lbPageRange, "lbPageRange");
			this.lbPageRange.Name = "lbPageRange";
			this.lbPageRange.StyleController = this.layoutControl1;
			resources.ApplyResources(this.rbBehind, "rbBehind");
			this.rbBehind.Name = "rbBehind";
			this.rbBehind.Properties.Caption = resources.GetString("rbBehind.Properties.Caption");
			this.rbBehind.Properties.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.Radio;
			this.rbBehind.Properties.RadioGroupIndex = 1;
			this.rbBehind.StyleController = this.layoutControl1;
			this.rbBehind.TabStop = false;
			this.rbBehind.CheckedChanged += new System.EventHandler(this.OnEditValueChanged);
			resources.ApplyResources(this.rbInFront, "rbInFront");
			this.rbInFront.Name = "rbInFront";
			this.rbInFront.Properties.Caption = resources.GetString("rbInFront.Properties.Caption");
			this.rbInFront.Properties.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.Radio;
			this.rbInFront.Properties.RadioGroupIndex = 1;
			this.rbInFront.StyleController = this.layoutControl1;
			this.rbInFront.CheckedChanged += new System.EventHandler(this.OnEditValueChanged);
			resources.ApplyResources(this.btnClear, "btnClear");
			this.btnClear.Name = "btnClear";
			this.btnClear.StyleController = this.layoutControl1;
			this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
			resources.ApplyResources(this.btnOK, "btnOK");
			this.btnOK.Name = "btnOK";
			this.btnOK.StyleController = this.layoutControl1;
			this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
			resources.ApplyResources(this.tePageRange, "tePageRange");
			this.tePageRange.Name = "tePageRange";
			this.tePageRange.Properties.Mask.EditMask = resources.GetString("tePageRange.Properties.Mask.EditMask");
			this.tePageRange.Properties.Mask.MaskType = ((DevExpress.XtraEditors.Mask.MaskType)(resources.GetObject("tePageRange.Properties.Mask.MaskType")));
			this.tePageRange.Properties.Mask.ShowPlaceHolders = ((bool)(resources.GetObject("tePageRange.Properties.Mask.ShowPlaceHolders")));
			this.tePageRange.Properties.MaxLength = 30;
			this.tePageRange.StyleController = this.layoutControl1;
			this.tePageRange.Tag = "";
			this.tePageRange.EditValueChanged += new System.EventHandler(this.tePageRange_EditValueChanged);
			this.rbAll.AutoSizeInLayoutControl = true;
			resources.ApplyResources(this.rbAll, "rbAll");
			this.rbAll.Name = "rbAll";
			this.rbAll.Properties.Caption = resources.GetString("rbAll.Properties.Caption");
			this.rbAll.Properties.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.Radio;
			this.rbAll.Properties.RadioGroupIndex = 0;
			this.rbAll.StyleController = this.layoutControl1;
			this.rbPages.AutoSizeInLayoutControl = true;
			resources.ApplyResources(this.rbPages, "rbPages");
			this.rbPages.Name = "rbPages";
			this.rbPages.Properties.Caption = resources.GetString("rbPages.Properties.Caption");
			this.rbPages.Properties.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.Radio;
			this.rbPages.Properties.RadioGroupIndex = 0;
			this.rbPages.StyleController = this.layoutControl1;
			this.rbPages.TabStop = false;
			this.rbPages.CheckedChanged += new System.EventHandler(this.rbPages_EditValueChanged);
			resources.ApplyResources(this.emptySpaceLabelControl4, "emptySpaceLabelControl4");
			this.emptySpaceLabelControl4.Name = "emptySpaceLabelControl4";
			this.emptySpaceLabelControl4.StyleController = this.layoutControl1;
			resources.ApplyResources(this.emptySpaceLabelControl2, "emptySpaceLabelControl2");
			this.emptySpaceLabelControl2.Name = "emptySpaceLabelControl2";
			this.emptySpaceLabelControl2.StyleController = this.layoutControl1;
			resources.ApplyResources(this.lbText, "lbText");
			this.lbText.Name = "lbText";
			this.lbText.StyleController = this.layoutControl1;
			resources.ApplyResources(this.lbLayout, "lbLayout");
			this.lbLayout.Name = "lbLayout";
			this.lbLayout.StyleController = this.layoutControl1;
			resources.ApplyResources(this.lbFont, "lbFont");
			this.lbFont.Name = "lbFont";
			this.lbFont.StyleController = this.layoutControl1;
			resources.ApplyResources(this.chbItalic, "chbItalic");
			this.chbItalic.Name = "chbItalic";
			this.chbItalic.Properties.Caption = resources.GetString("chbItalic.Properties.Caption");
			this.chbItalic.StyleController = this.layoutControl1;
			this.chbItalic.CheckedChanged += new System.EventHandler(this.OnEditValueChanged);
			resources.ApplyResources(this.chbBold, "chbBold");
			this.chbBold.Name = "chbBold";
			this.chbBold.Properties.Caption = resources.GetString("chbBold.Properties.Caption");
			this.chbBold.StyleController = this.layoutControl1;
			this.chbBold.CheckedChanged += new System.EventHandler(this.OnEditValueChanged);
			resources.ApplyResources(this.ceWatermarkColor, "ceWatermarkColor");
			this.ceWatermarkColor.Name = "ceWatermarkColor";
			this.ceWatermarkColor.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("ceWatermarkColor.Properties.Buttons"))))});
			this.ceWatermarkColor.Properties.ColorAlignment = DevExpress.Utils.HorzAlignment.Center;
			this.ceWatermarkColor.StyleController = this.layoutControl1;
			this.ceWatermarkColor.EditValueChanged += new System.EventHandler(this.OnEditValueChanged);
			resources.ApplyResources(this.cmbWatermarkFont, "cmbWatermarkFont");
			this.cmbWatermarkFont.Name = "cmbWatermarkFont";
			this.cmbWatermarkFont.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("cmbWatermarkFont.Properties.Buttons"))))});
			this.cmbWatermarkFont.StyleController = this.layoutControl1;
			this.cmbWatermarkFont.EditValueChanged += new System.EventHandler(this.OnEditValueChanged);
			resources.ApplyResources(this.cmbWatermarkFontSize, "cmbWatermarkFontSize");
			this.cmbWatermarkFontSize.Name = "cmbWatermarkFontSize";
			this.cmbWatermarkFontSize.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("cmbWatermarkFontSize.Properties.Buttons"))))});
			this.cmbWatermarkFontSize.StyleController = this.layoutControl1;
			this.cmbWatermarkFontSize.TextChanged += new System.EventHandler(this.OnEditValueChanged);
			resources.ApplyResources(this.lkpTextDirection, "lkpTextDirection");
			this.lkpTextDirection.Name = "lkpTextDirection";
			this.lkpTextDirection.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("lkpTextDirection.Properties.Buttons"))))});
			this.lkpTextDirection.Properties.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[] {
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo(resources.GetString("lkpTextDirection.Properties.Columns"), resources.GetString("lkpTextDirection.Properties.Columns1"))});
			this.lkpTextDirection.Properties.ShowFooter = false;
			this.lkpTextDirection.Properties.ShowHeader = false;
			this.lkpTextDirection.Properties.ShowLines = false;
			this.lkpTextDirection.StyleController = this.layoutControl1;
			this.lkpTextDirection.EditValueChanged += new System.EventHandler(this.OnEditValueChanged);
			resources.ApplyResources(this.cmbWatermarkText, "cmbWatermarkText");
			this.cmbWatermarkText.Name = "cmbWatermarkText";
			this.cmbWatermarkText.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("cmbWatermarkText.Properties.Buttons"))))});
			this.cmbWatermarkText.StyleController = this.layoutControl1;
			this.cmbWatermarkText.TextChanged += new System.EventHandler(this.cmbWatermarkText_TextChanged);
			resources.ApplyResources(this.lbFontColor, "lbFontColor");
			this.lbFontColor.Name = "lbFontColor";
			this.lbFontColor.StyleController = this.layoutControl1;
			resources.ApplyResources(this.lbFontSize, "lbFontSize");
			this.lbFontSize.Name = "lbFontSize";
			this.lbFontSize.StyleController = this.layoutControl1;
			resources.ApplyResources(this.trBarTextTransparency, "trBarTextTransparency");
			this.trBarTextTransparency.Name = "trBarTextTransparency";
			this.trBarTextTransparency.Properties.LabelAppearance.Options.UseTextOptions = true;
			this.trBarTextTransparency.Properties.LabelAppearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
			this.trBarTextTransparency.Properties.Maximum = 255;
			this.trBarTextTransparency.Properties.TickFrequency = 15;
			this.trBarTextTransparency.StyleController = this.layoutControl1;
			this.trBarTextTransparency.Value = 50;
			this.trBarTextTransparency.EditValueChanged += new System.EventHandler(this.trBarTextTransparency_EditValueChanged);
			resources.ApplyResources(this.teTransparentValue, "teTransparentValue");
			this.teTransparentValue.Name = "teTransparentValue";
			this.teTransparentValue.StyleController = this.layoutControl1;
			this.teTransparentValue.EditValueChanged += new System.EventHandler(this.teTransparentValue_EditValueChanged);
			resources.ApplyResources(this.lbTextTransparency, "lbTextTransparency");
			this.lbTextTransparency.Name = "lbTextTransparency";
			this.lbTextTransparency.StyleController = this.layoutControl1;
			resources.ApplyResources(this.emptySpaceLabelControl7, "emptySpaceLabelControl7");
			this.emptySpaceLabelControl7.Name = "emptySpaceLabelControl7";
			this.emptySpaceLabelControl7.StyleController = this.layoutControl1;
			resources.ApplyResources(this.btnSelectPicture, "btnSelectPicture");
			this.btnSelectPicture.Name = "btnSelectPicture";
			this.btnSelectPicture.StyleController = this.layoutControl1;
			this.btnSelectPicture.Click += new System.EventHandler(this.btnSelectPicture_Click);
			resources.ApplyResources(this.lbLoadImage, "lbLoadImage");
			this.lbLoadImage.Name = "lbLoadImage";
			this.lbLoadImage.StyleController = this.layoutControl1;
			this.chbTiling.AutoSizeInLayoutControl = true;
			resources.ApplyResources(this.chbTiling, "chbTiling");
			this.chbTiling.Name = "chbTiling";
			this.chbTiling.Properties.Caption = resources.GetString("chbTiling.Properties.Caption");
			this.chbTiling.StyleController = this.layoutControl1;
			this.chbTiling.CheckedChanged += new System.EventHandler(this.OnEditValueChanged);
			resources.ApplyResources(this.lkpImageVAlign, "lkpImageVAlign");
			this.lkpImageVAlign.Name = "lkpImageVAlign";
			this.lkpImageVAlign.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("lkpImageVAlign.Properties.Buttons"))))});
			this.lkpImageVAlign.Properties.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[] {
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo(resources.GetString("lkpImageVAlign.Properties.Columns"), resources.GetString("lkpImageVAlign.Properties.Columns1"))});
			this.lkpImageVAlign.Properties.ShowFooter = false;
			this.lkpImageVAlign.Properties.ShowHeader = false;
			this.lkpImageVAlign.Properties.ShowLines = false;
			this.lkpImageVAlign.StyleController = this.layoutControl1;
			this.lkpImageVAlign.EditValueChanged += new System.EventHandler(this.OnEditValueChanged);
			resources.ApplyResources(this.lkpImageHAlign, "lkpImageHAlign");
			this.lkpImageHAlign.Name = "lkpImageHAlign";
			this.lkpImageHAlign.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("lkpImageHAlign.Properties.Buttons"))))});
			this.lkpImageHAlign.Properties.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[] {
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo(resources.GetString("lkpImageHAlign.Properties.Columns"), resources.GetString("lkpImageHAlign.Properties.Columns1"))});
			this.lkpImageHAlign.Properties.ShowFooter = false;
			this.lkpImageHAlign.Properties.ShowHeader = false;
			this.lkpImageHAlign.Properties.ShowLines = false;
			this.lkpImageHAlign.StyleController = this.layoutControl1;
			this.lkpImageHAlign.EditValueChanged += new System.EventHandler(this.OnEditValueChanged);
			resources.ApplyResources(this.lkpImageView, "lkpImageView");
			this.lkpImageView.Name = "lkpImageView";
			this.lkpImageView.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("lkpImageView.Properties.Buttons"))))});
			this.lkpImageView.Properties.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[] {
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo(resources.GetString("lkpImageView.Properties.Columns"), resources.GetString("lkpImageView.Properties.Columns1"))});
			this.lkpImageView.Properties.ShowFooter = false;
			this.lkpImageView.Properties.ShowHeader = false;
			this.lkpImageView.Properties.ShowLines = false;
			this.lkpImageView.StyleController = this.layoutControl1;
			this.lkpImageView.EditValueChanged += new System.EventHandler(this.OnEditValueChanged);
			resources.ApplyResources(this.lbPosition, "lbPosition");
			this.lbPosition.Name = "lbPosition";
			this.lbPosition.StyleController = this.layoutControl1;
			resources.ApplyResources(this.lbHorzAlign, "lbHorzAlign");
			this.lbHorzAlign.Name = "lbHorzAlign";
			this.lbHorzAlign.StyleController = this.layoutControl1;
			resources.ApplyResources(this.lbVertAlign, "lbVertAlign");
			this.lbVertAlign.Name = "lbVertAlign";
			this.lbVertAlign.StyleController = this.layoutControl1;
			resources.ApplyResources(this.trBarImageTransparency, "trBarImageTransparency");
			this.trBarImageTransparency.Name = "trBarImageTransparency";
			this.trBarImageTransparency.Properties.LabelAppearance.Options.UseTextOptions = true;
			this.trBarImageTransparency.Properties.LabelAppearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
			this.trBarImageTransparency.Properties.Maximum = 255;
			this.trBarImageTransparency.Properties.TickFrequency = 15;
			this.trBarImageTransparency.StyleController = this.layoutControl1;
			this.trBarImageTransparency.Value = 50;
			this.trBarImageTransparency.EditValueChanged += new System.EventHandler(this.trBarPictureTransparency_EditValueChanged);
			resources.ApplyResources(this.teImageTransparentValue, "teImageTransparentValue");
			this.teImageTransparentValue.Name = "teImageTransparentValue";
			this.teImageTransparentValue.StyleController = this.layoutControl1;
			this.teImageTransparentValue.EditValueChanged += new System.EventHandler(this.teImageTransparentValue_EditValueChanged);
			resources.ApplyResources(this.lbPictureTransparency, "lbPictureTransparency");
			this.lbPictureTransparency.Name = "lbPictureTransparency";
			this.lbPictureTransparency.StyleController = this.layoutControl1;
			this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
			this.layoutControlGroup1.GroupBordersVisible = false;
			this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
			this.layoutControlItem1,
			this.grpBoxZOrder,
			this.grpBoxPageRange,
			this.grpButtons,
			this.layoutControlItem8});
			this.layoutControlGroup1.LayoutMode = DevExpress.XtraLayout.Utils.LayoutMode.Table;
			this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
			this.layoutControlGroup1.Name = "Root";
			this.layoutControlGroup1.OptionsTableLayoutGroup.AutoSizeDefaultDefinitionLength = 1;
			columnDefinition43.SizeType = System.Windows.Forms.SizeType.Percent;
			columnDefinition43.Width = 42.139384116693684D;
			columnDefinition44.SizeType = System.Windows.Forms.SizeType.Absolute;
			columnDefinition44.Width = 10D;
			columnDefinition45.SizeType = System.Windows.Forms.SizeType.Percent;
			columnDefinition45.Width = 16.855753646677474D;
			columnDefinition46.SizeType = System.Windows.Forms.SizeType.Percent;
			columnDefinition46.Width = 41.004862236628853D;
			this.layoutControlGroup1.OptionsTableLayoutGroup.ColumnDefinitions.AddRange(new DevExpress.XtraLayout.ColumnDefinition[] {
			columnDefinition43,
			columnDefinition44,
			columnDefinition45,
			columnDefinition46});
			rowDefinition35.Height = 222D;
			rowDefinition35.SizeType = System.Windows.Forms.SizeType.AutoSize;
			rowDefinition36.Height = 94D;
			rowDefinition36.SizeType = System.Windows.Forms.SizeType.AutoSize;
			rowDefinition37.Height = 44D;
			rowDefinition37.SizeType = System.Windows.Forms.SizeType.AutoSize;
			this.layoutControlGroup1.OptionsTableLayoutGroup.RowDefinitions.AddRange(new DevExpress.XtraLayout.RowDefinition[] {
			rowDefinition35,
			rowDefinition36,
			rowDefinition37});
			this.layoutControlGroup1.Size = new System.Drawing.Size(647, 380);
			this.layoutControlGroup1.TextVisible = false;
			this.layoutControlItem1.Location = new System.Drawing.Point(270, 0);
			this.layoutControlItem1.Name = "panelControl1item";
			this.layoutControlItem1.OptionsTableLayoutItem.ColumnIndex = 2;
			this.layoutControlItem1.OptionsTableLayoutItem.ColumnSpan = 2;
			this.layoutControlItem1.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
			this.layoutControlItem1.SelectedTabPage = this.tpTextWaterMark;
			this.layoutControlItem1.SelectedTabPageIndex = 0;
			this.layoutControlItem1.Size = new System.Drawing.Size(357, 222);
			this.layoutControlItem1.TabPages.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
			this.tpTextWaterMark,
			this.tpPictureWatermark});
			this.tpTextWaterMark.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
			this.layoutControlGroup9,
			this.layoutControlGroup10});
			this.tpTextWaterMark.LayoutMode = DevExpress.XtraLayout.Utils.LayoutMode.Table;
			this.tpTextWaterMark.Location = new System.Drawing.Point(0, 0);
			this.tpTextWaterMark.Name = "tpTextWaterMark";
			this.tpTextWaterMark.OptionsTableLayoutGroup.AutoSizeDefaultDefinitionLength = 1;
			columnDefinition17.SizeType = System.Windows.Forms.SizeType.AutoSize;
			columnDefinition17.Width = 351D;
			this.tpTextWaterMark.OptionsTableLayoutGroup.ColumnDefinitions.AddRange(new DevExpress.XtraLayout.ColumnDefinition[] {
			columnDefinition17});
			rowDefinition12.Height = 122D;
			rowDefinition12.SizeType = System.Windows.Forms.SizeType.AutoSize;
			rowDefinition13.Height = 72D;
			rowDefinition13.SizeType = System.Windows.Forms.SizeType.AutoSize;
			this.tpTextWaterMark.OptionsTableLayoutGroup.RowDefinitions.AddRange(new DevExpress.XtraLayout.RowDefinition[] {
			rowDefinition12,
			rowDefinition13});
			this.tpTextWaterMark.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
			this.tpTextWaterMark.Size = new System.Drawing.Size(351, 194);
			resources.ApplyResources(this.tpTextWaterMark, "tpTextWaterMark");
			this.layoutControlGroup9.GroupBordersVisible = false;
			this.layoutControlGroup9.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
			this.layoutControlItem11,
			this.layoutControlItem13,
			this.layoutControlItem14});
			this.layoutControlGroup9.LayoutMode = DevExpress.XtraLayout.Utils.LayoutMode.Table;
			this.layoutControlGroup9.Location = new System.Drawing.Point(0, 122);
			this.layoutControlGroup9.Name = "layoutControlGroup9";
			this.layoutControlGroup9.OptionsTableLayoutGroup.AutoSizeDefaultDefinitionLength = 1;
			columnDefinition1.SizeType = System.Windows.Forms.SizeType.Absolute;
			columnDefinition1.Width = 9D;
			columnDefinition2.SizeType = System.Windows.Forms.SizeType.AutoSize;
			columnDefinition2.Width = 122D;
			columnDefinition3.SizeType = System.Windows.Forms.SizeType.Percent;
			columnDefinition3.Width = 73.93364928909952D;
			columnDefinition4.SizeType = System.Windows.Forms.SizeType.Percent;
			columnDefinition4.Width = 26.066350710900473D;
			columnDefinition5.SizeType = System.Windows.Forms.SizeType.Absolute;
			columnDefinition5.Width = 9D;
			this.layoutControlGroup9.OptionsTableLayoutGroup.ColumnDefinitions.AddRange(new DevExpress.XtraLayout.ColumnDefinition[] {
			columnDefinition1,
			columnDefinition2,
			columnDefinition3,
			columnDefinition4,
			columnDefinition5});
			rowDefinition1.Height = 26D;
			rowDefinition1.SizeType = System.Windows.Forms.SizeType.AutoSize;
			rowDefinition2.Height = 45D;
			rowDefinition2.SizeType = System.Windows.Forms.SizeType.AutoSize;
			rowDefinition3.Height = 1D;
			rowDefinition3.SizeType = System.Windows.Forms.SizeType.AutoSize;
			this.layoutControlGroup9.OptionsTableLayoutGroup.RowDefinitions.AddRange(new DevExpress.XtraLayout.RowDefinition[] {
			rowDefinition1,
			rowDefinition2,
			rowDefinition3});
			this.layoutControlGroup9.OptionsTableLayoutItem.RowIndex = 1;
			this.layoutControlGroup9.Size = new System.Drawing.Size(351, 72);
			this.layoutControlItem11.Control = this.lbTextTransparency;
			this.layoutControlItem11.ControlAlignment = System.Drawing.ContentAlignment.MiddleLeft;
			this.layoutControlItem11.Location = new System.Drawing.Point(9, 0);
			this.layoutControlItem11.Name = "layoutControlItem11";
			this.layoutControlItem11.OptionsTableLayoutItem.ColumnIndex = 1;
			this.layoutControlItem11.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
			this.layoutControlItem11.Size = new System.Drawing.Size(122, 26);
			this.layoutControlItem11.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem11.TextVisible = false;
			this.layoutControlItem11.TrimClientAreaToControl = false;
			this.layoutControlItem13.Control = this.teTransparentValue;
			this.layoutControlItem13.ControlAlignment = System.Drawing.ContentAlignment.MiddleLeft;
			this.layoutControlItem13.Location = new System.Drawing.Point(287, 0);
			this.layoutControlItem13.Name = "layoutControlItem13";
			this.layoutControlItem13.OptionsTableLayoutItem.ColumnIndex = 3;
			this.layoutControlItem13.Size = new System.Drawing.Size(55, 26);
			this.layoutControlItem13.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem13.TextVisible = false;
			this.layoutControlItem13.TrimClientAreaToControl = false;
			this.layoutControlItem14.Control = this.trBarTextTransparency;
			this.layoutControlItem14.Location = new System.Drawing.Point(9, 26);
			this.layoutControlItem14.Name = "layoutControlItem14";
			this.layoutControlItem14.OptionsTableLayoutItem.ColumnIndex = 1;
			this.layoutControlItem14.OptionsTableLayoutItem.ColumnSpan = 3;
			this.layoutControlItem14.OptionsTableLayoutItem.RowIndex = 1;
			this.layoutControlItem14.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
			this.layoutControlItem14.Size = new System.Drawing.Size(333, 45);
			this.layoutControlItem14.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem14.TextVisible = false;
			this.layoutControlGroup10.GroupBordersVisible = false;
			this.layoutControlGroup10.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
			this.layoutControlItem20,
			this.layoutControlItem21,
			this.layoutControlItem25,
			this.layoutControlItem26,
			this.layoutControlItem27,
			this.layoutControlItem28,
			this.layoutControlItem29,
			this.layoutControlGroup11,
			this.layoutControlItem19,
			this.layoutControlItem32,
			this.emptySpaceLabelControl1,
			this.layoutControlItem43,
			this.layoutControlItem18});
			this.layoutControlGroup10.LayoutMode = DevExpress.XtraLayout.Utils.LayoutMode.Table;
			this.layoutControlGroup10.Location = new System.Drawing.Point(0, 0);
			this.layoutControlGroup10.Name = "layoutControlGroup10";
			this.layoutControlGroup10.OptionsTableLayoutGroup.AutoSizeDefaultDefinitionLength = 1;
			columnDefinition10.SizeType = System.Windows.Forms.SizeType.Absolute;
			columnDefinition10.Width = 9D;
			columnDefinition11.SizeType = System.Windows.Forms.SizeType.AutoSize;
			columnDefinition11.Width = 59D;
			columnDefinition12.SizeType = System.Windows.Forms.SizeType.Percent;
			columnDefinition12.Width = 51.724137931034477D;
			columnDefinition13.SizeType = System.Windows.Forms.SizeType.Percent;
			columnDefinition13.Width = 4.3103448275862064D;
			columnDefinition14.SizeType = System.Windows.Forms.SizeType.AutoSize;
			columnDefinition14.Width = 42D;
			columnDefinition15.SizeType = System.Windows.Forms.SizeType.Percent;
			columnDefinition15.Width = 43.96551724137931D;
			columnDefinition16.SizeType = System.Windows.Forms.SizeType.Absolute;
			columnDefinition16.Width = 9D;
			this.layoutControlGroup10.OptionsTableLayoutGroup.ColumnDefinitions.AddRange(new DevExpress.XtraLayout.ColumnDefinition[] {
			columnDefinition10,
			columnDefinition11,
			columnDefinition12,
			columnDefinition13,
			columnDefinition14,
			columnDefinition15,
			columnDefinition16});
			rowDefinition6.Height = 13D;
			rowDefinition6.SizeType = System.Windows.Forms.SizeType.AutoSize;
			rowDefinition7.Height = 26D;
			rowDefinition7.SizeType = System.Windows.Forms.SizeType.AutoSize;
			rowDefinition8.Height = 26D;
			rowDefinition8.SizeType = System.Windows.Forms.SizeType.AutoSize;
			rowDefinition9.Height = 26D;
			rowDefinition9.SizeType = System.Windows.Forms.SizeType.AutoSize;
			rowDefinition10.Height = 26D;
			rowDefinition10.SizeType = System.Windows.Forms.SizeType.AutoSize;
			rowDefinition11.Height = 5D;
			rowDefinition11.SizeType = System.Windows.Forms.SizeType.Absolute;
			this.layoutControlGroup10.OptionsTableLayoutGroup.RowDefinitions.AddRange(new DevExpress.XtraLayout.RowDefinition[] {
			rowDefinition6,
			rowDefinition7,
			rowDefinition8,
			rowDefinition9,
			rowDefinition10,
			rowDefinition11});
			this.layoutControlGroup10.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
			this.layoutControlGroup10.Size = new System.Drawing.Size(351, 122);
			this.layoutControlItem20.Control = this.lbFontSize;
			this.layoutControlItem20.ControlAlignment = System.Drawing.ContentAlignment.MiddleLeft;
			this.layoutControlItem20.Location = new System.Drawing.Point(198, 65);
			this.layoutControlItem20.Name = "layoutControlItem20";
			this.layoutControlItem20.OptionsTableLayoutItem.ColumnIndex = 4;
			this.layoutControlItem20.OptionsTableLayoutItem.RowIndex = 3;
			this.layoutControlItem20.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
			this.layoutControlItem20.Size = new System.Drawing.Size(42, 26);
			this.layoutControlItem20.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem20.TextVisible = false;
			this.layoutControlItem20.TrimClientAreaToControl = false;
			this.layoutControlItem21.Control = this.lbFontColor;
			this.layoutControlItem21.ControlAlignment = System.Drawing.ContentAlignment.MiddleLeft;
			this.layoutControlItem21.Location = new System.Drawing.Point(198, 39);
			this.layoutControlItem21.Name = "layoutControlItem21";
			this.layoutControlItem21.OptionsTableLayoutItem.ColumnIndex = 4;
			this.layoutControlItem21.OptionsTableLayoutItem.RowIndex = 2;
			this.layoutControlItem21.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
			this.layoutControlItem21.Size = new System.Drawing.Size(42, 26);
			this.layoutControlItem21.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem21.TextVisible = false;
			this.layoutControlItem21.TrimClientAreaToControl = false;
			this.layoutControlItem25.Control = this.cmbWatermarkText;
			this.layoutControlItem25.Location = new System.Drawing.Point(68, 13);
			this.layoutControlItem25.Name = "layoutControlItem25";
			this.layoutControlItem25.OptionsTableLayoutItem.ColumnIndex = 2;
			this.layoutControlItem25.OptionsTableLayoutItem.ColumnSpan = 4;
			this.layoutControlItem25.OptionsTableLayoutItem.RowIndex = 1;
			this.layoutControlItem25.Size = new System.Drawing.Size(274, 26);
			this.layoutControlItem25.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem25.TextVisible = false;
			this.layoutControlItem26.Control = this.lkpTextDirection;
			this.layoutControlItem26.ControlAlignment = System.Drawing.ContentAlignment.MiddleLeft;
			this.layoutControlItem26.Location = new System.Drawing.Point(68, 39);
			this.layoutControlItem26.Name = "layoutControlItem26";
			this.layoutControlItem26.OptionsTableLayoutItem.ColumnIndex = 2;
			this.layoutControlItem26.OptionsTableLayoutItem.RowIndex = 2;
			this.layoutControlItem26.Size = new System.Drawing.Size(120, 26);
			this.layoutControlItem26.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem26.TextVisible = false;
			this.layoutControlItem26.TrimClientAreaToControl = false;
			this.layoutControlItem27.Control = this.cmbWatermarkFontSize;
			this.layoutControlItem27.ControlAlignment = System.Drawing.ContentAlignment.MiddleLeft;
			this.layoutControlItem27.Location = new System.Drawing.Point(240, 65);
			this.layoutControlItem27.Name = "layoutControlItem27";
			this.layoutControlItem27.OptionsTableLayoutItem.ColumnIndex = 5;
			this.layoutControlItem27.OptionsTableLayoutItem.RowIndex = 3;
			this.layoutControlItem27.Size = new System.Drawing.Size(102, 26);
			this.layoutControlItem27.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem27.TextVisible = false;
			this.layoutControlItem27.TrimClientAreaToControl = false;
			this.layoutControlItem28.Control = this.cmbWatermarkFont;
			this.layoutControlItem28.ControlAlignment = System.Drawing.ContentAlignment.MiddleLeft;
			this.layoutControlItem28.Location = new System.Drawing.Point(68, 65);
			this.layoutControlItem28.Name = "layoutControlItem28";
			this.layoutControlItem28.OptionsTableLayoutItem.ColumnIndex = 2;
			this.layoutControlItem28.OptionsTableLayoutItem.RowIndex = 3;
			this.layoutControlItem28.Size = new System.Drawing.Size(120, 26);
			this.layoutControlItem28.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem28.TextVisible = false;
			this.layoutControlItem28.TrimClientAreaToControl = false;
			this.layoutControlItem29.Control = this.ceWatermarkColor;
			this.layoutControlItem29.ControlAlignment = System.Drawing.ContentAlignment.MiddleLeft;
			this.layoutControlItem29.Location = new System.Drawing.Point(240, 39);
			this.layoutControlItem29.Name = "layoutControlItem29";
			this.layoutControlItem29.OptionsTableLayoutItem.ColumnIndex = 5;
			this.layoutControlItem29.OptionsTableLayoutItem.RowIndex = 2;
			this.layoutControlItem29.Size = new System.Drawing.Size(102, 26);
			this.layoutControlItem29.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem29.TextVisible = false;
			this.layoutControlItem29.TrimClientAreaToControl = false;
			this.layoutControlGroup11.GroupBordersVisible = false;
			this.layoutControlGroup11.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
			this.layoutControlItem30,
			this.layoutControlItem31});
			this.layoutControlGroup11.LayoutMode = DevExpress.XtraLayout.Utils.LayoutMode.Table;
			this.layoutControlGroup11.Location = new System.Drawing.Point(68, 91);
			this.layoutControlGroup11.Name = "layoutControlGroup11";
			this.layoutControlGroup11.OptionsTableLayoutGroup.AutoSizeDefaultDefinitionLength = 1;
			columnDefinition6.SizeType = System.Windows.Forms.SizeType.AutoSize;
			columnDefinition6.Width = 46D;
			columnDefinition7.SizeType = System.Windows.Forms.SizeType.Percent;
			columnDefinition7.Width = 11.627906976744187D;
			columnDefinition8.SizeType = System.Windows.Forms.SizeType.AutoSize;
			columnDefinition8.Width = 50D;
			columnDefinition9.SizeType = System.Windows.Forms.SizeType.Percent;
			columnDefinition9.Width = 88.372093023255815D;
			this.layoutControlGroup11.OptionsTableLayoutGroup.ColumnDefinitions.AddRange(new DevExpress.XtraLayout.ColumnDefinition[] {
			columnDefinition6,
			columnDefinition7,
			columnDefinition8,
			columnDefinition9});
			rowDefinition4.Height = 23D;
			rowDefinition4.SizeType = System.Windows.Forms.SizeType.AutoSize;
			rowDefinition5.Height = 3D;
			rowDefinition5.SizeType = System.Windows.Forms.SizeType.AutoSize;
			this.layoutControlGroup11.OptionsTableLayoutGroup.RowDefinitions.AddRange(new DevExpress.XtraLayout.RowDefinition[] {
			rowDefinition4,
			rowDefinition5});
			this.layoutControlGroup11.OptionsTableLayoutItem.ColumnIndex = 2;
			this.layoutControlGroup11.OptionsTableLayoutItem.ColumnSpan = 4;
			this.layoutControlGroup11.OptionsTableLayoutItem.RowIndex = 4;
			this.layoutControlGroup11.Size = new System.Drawing.Size(274, 26);
			this.layoutControlItem30.Control = this.chbBold;
			this.layoutControlItem30.Location = new System.Drawing.Point(0, 0);
			this.layoutControlItem30.Name = "layoutControlItem30";
			this.layoutControlItem30.Size = new System.Drawing.Size(46, 23);
			this.layoutControlItem30.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem30.TextVisible = false;
			this.layoutControlItem31.Control = this.chbItalic;
			this.layoutControlItem31.Location = new System.Drawing.Point(67, 0);
			this.layoutControlItem31.Name = "layoutControlItem31";
			this.layoutControlItem31.OptionsTableLayoutItem.ColumnIndex = 2;
			this.layoutControlItem31.Size = new System.Drawing.Size(50, 23);
			this.layoutControlItem31.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem31.TextVisible = false;
			this.layoutControlItem19.Control = this.lbLayout;
			this.layoutControlItem19.ControlAlignment = System.Drawing.ContentAlignment.MiddleLeft;
			this.layoutControlItem19.Location = new System.Drawing.Point(9, 39);
			this.layoutControlItem19.Name = "layoutControlItem19";
			this.layoutControlItem19.OptionsTableLayoutItem.ColumnIndex = 1;
			this.layoutControlItem19.OptionsTableLayoutItem.RowIndex = 2;
			this.layoutControlItem19.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
			this.layoutControlItem19.Size = new System.Drawing.Size(59, 26);
			this.layoutControlItem19.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem19.TextVisible = false;
			this.layoutControlItem19.TrimClientAreaToControl = false;
			this.layoutControlItem32.Control = this.lbText;
			this.layoutControlItem32.ControlAlignment = System.Drawing.ContentAlignment.MiddleLeft;
			this.layoutControlItem32.Location = new System.Drawing.Point(9, 13);
			this.layoutControlItem32.Name = "layoutControlItem32";
			this.layoutControlItem32.OptionsTableLayoutItem.ColumnIndex = 1;
			this.layoutControlItem32.OptionsTableLayoutItem.RowIndex = 1;
			this.layoutControlItem32.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
			this.layoutControlItem32.Size = new System.Drawing.Size(59, 26);
			this.layoutControlItem32.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem32.TextVisible = false;
			this.layoutControlItem32.TrimClientAreaToControl = false;
			this.emptySpaceLabelControl1.Control = this.emptySpaceLabelControl2;
			this.emptySpaceLabelControl1.Location = new System.Drawing.Point(9, 91);
			this.emptySpaceLabelControl1.Name = "emptySpaceLabelControl1";
			this.emptySpaceLabelControl1.OptionsTableLayoutItem.ColumnIndex = 1;
			this.emptySpaceLabelControl1.OptionsTableLayoutItem.RowIndex = 4;
			this.emptySpaceLabelControl1.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
			this.emptySpaceLabelControl1.Size = new System.Drawing.Size(59, 26);
			this.emptySpaceLabelControl1.TextSize = new System.Drawing.Size(0, 0);
			this.emptySpaceLabelControl1.TextVisible = false;
			this.layoutControlItem43.Control = this.emptySpaceLabelControl4;
			this.layoutControlItem43.Location = new System.Drawing.Point(9, 0);
			this.layoutControlItem43.Name = "layoutControlItem43";
			this.layoutControlItem43.OptionsTableLayoutItem.ColumnIndex = 1;
			this.layoutControlItem43.OptionsTableLayoutItem.ColumnSpan = 5;
			this.layoutControlItem43.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
			this.layoutControlItem43.Size = new System.Drawing.Size(333, 13);
			this.layoutControlItem43.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem43.TextVisible = false;
			this.layoutControlItem18.Control = this.lbFont;
			this.layoutControlItem18.ControlAlignment = System.Drawing.ContentAlignment.MiddleLeft;
			this.layoutControlItem18.Location = new System.Drawing.Point(9, 65);
			this.layoutControlItem18.Name = "layoutControlItem18";
			this.layoutControlItem18.OptionsTableLayoutItem.ColumnIndex = 1;
			this.layoutControlItem18.OptionsTableLayoutItem.RowIndex = 3;
			this.layoutControlItem18.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
			this.layoutControlItem18.Size = new System.Drawing.Size(59, 26);
			this.layoutControlItem18.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem18.TextVisible = false;
			this.layoutControlItem18.TrimClientAreaToControl = false;
			this.tpPictureWatermark.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
			this.layoutControlGroup8,
			this.layoutControlGroup7});
			this.tpPictureWatermark.LayoutMode = DevExpress.XtraLayout.Utils.LayoutMode.Table;
			this.tpPictureWatermark.Location = new System.Drawing.Point(0, 0);
			this.tpPictureWatermark.Name = "tpPictureWatermark";
			this.tpPictureWatermark.OptionsTableLayoutGroup.AutoSizeDefaultDefinitionLength = 1;
			columnDefinition29.SizeType = System.Windows.Forms.SizeType.AutoSize;
			columnDefinition29.Width = 351D;
			this.tpPictureWatermark.OptionsTableLayoutGroup.ColumnDefinitions.AddRange(new DevExpress.XtraLayout.ColumnDefinition[] {
			columnDefinition29});
			rowDefinition24.Height = 1D;
			rowDefinition24.SizeType = System.Windows.Forms.SizeType.AutoSize;
			rowDefinition25.Height = 193D;
			rowDefinition25.SizeType = System.Windows.Forms.SizeType.AutoSize;
			this.tpPictureWatermark.OptionsTableLayoutGroup.RowDefinitions.AddRange(new DevExpress.XtraLayout.RowDefinition[] {
			rowDefinition24,
			rowDefinition25});
			this.tpPictureWatermark.Size = new System.Drawing.Size(351, 194);
			resources.ApplyResources(this.tpPictureWatermark, "tpPictureWatermark");
			this.layoutControlGroup8.GroupBordersVisible = false;
			this.layoutControlGroup8.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
			this.layoutControlItem5,
			this.layoutControlItem15,
			this.layoutControlItem16});
			this.layoutControlGroup8.LayoutMode = DevExpress.XtraLayout.Utils.LayoutMode.Table;
			this.layoutControlGroup8.Location = new System.Drawing.Point(0, 1);
			this.layoutControlGroup8.Name = "layoutControlGroup8";
			this.layoutControlGroup8.OptionsTableLayoutGroup.AutoSizeDefaultDefinitionLength = 1;
			columnDefinition18.SizeType = System.Windows.Forms.SizeType.Absolute;
			columnDefinition18.Width = 9D;
			columnDefinition19.SizeType = System.Windows.Forms.SizeType.AutoSize;
			columnDefinition19.Width = 1D;
			columnDefinition20.SizeType = System.Windows.Forms.SizeType.Percent;
			columnDefinition20.Width = 73.93364928909952D;
			columnDefinition21.SizeType = System.Windows.Forms.SizeType.Percent;
			columnDefinition21.Width = 26.066350710900473D;
			columnDefinition22.SizeType = System.Windows.Forms.SizeType.Absolute;
			columnDefinition22.Width = 9D;
			this.layoutControlGroup8.OptionsTableLayoutGroup.ColumnDefinitions.AddRange(new DevExpress.XtraLayout.ColumnDefinition[] {
			columnDefinition18,
			columnDefinition19,
			columnDefinition20,
			columnDefinition21,
			columnDefinition22});
			rowDefinition14.Height = 1D;
			rowDefinition14.SizeType = System.Windows.Forms.SizeType.AutoSize;
			rowDefinition15.Height = 192D;
			rowDefinition15.SizeType = System.Windows.Forms.SizeType.AutoSize;
			this.layoutControlGroup8.OptionsTableLayoutGroup.RowDefinitions.AddRange(new DevExpress.XtraLayout.RowDefinition[] {
			rowDefinition14,
			rowDefinition15});
			this.layoutControlGroup8.OptionsTableLayoutItem.RowIndex = 1;
			this.layoutControlGroup8.Size = new System.Drawing.Size(351, 193);
			this.layoutControlItem5.Control = this.lbPictureTransparency;
			this.layoutControlItem5.ControlAlignment = System.Drawing.ContentAlignment.MiddleLeft;
			this.layoutControlItem5.Location = new System.Drawing.Point(9, 0);
			this.layoutControlItem5.Name = "layoutControlItem5";
			this.layoutControlItem5.OptionsTableLayoutItem.ColumnIndex = 1;
			this.layoutControlItem5.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
			this.layoutControlItem5.Size = new System.Drawing.Size(1, 1);
			this.layoutControlItem5.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem5.TextVisible = false;
			this.layoutControlItem5.TrimClientAreaToControl = false;
			this.layoutControlItem15.Control = this.teImageTransparentValue;
			this.layoutControlItem15.ControlAlignment = System.Drawing.ContentAlignment.MiddleLeft;
			this.layoutControlItem15.Location = new System.Drawing.Point(255, 0);
			this.layoutControlItem15.Name = "layoutControlItem15";
			this.layoutControlItem15.OptionsTableLayoutItem.ColumnIndex = 3;
			this.layoutControlItem15.Size = new System.Drawing.Size(87, 1);
			this.layoutControlItem15.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem15.TextVisible = false;
			this.layoutControlItem15.TrimClientAreaToControl = false;
			this.layoutControlItem16.Control = this.trBarImageTransparency;
			this.layoutControlItem16.Location = new System.Drawing.Point(9, 1);
			this.layoutControlItem16.Name = "layoutControlItem16";
			this.layoutControlItem16.OptionsTableLayoutItem.ColumnIndex = 1;
			this.layoutControlItem16.OptionsTableLayoutItem.ColumnSpan = 3;
			this.layoutControlItem16.OptionsTableLayoutItem.RowIndex = 1;
			this.layoutControlItem16.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
			this.layoutControlItem16.Size = new System.Drawing.Size(333, 192);
			this.layoutControlItem16.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem16.TextVisible = false;
			this.layoutControlGroup7.GroupBordersVisible = false;
			this.layoutControlGroup7.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
			this.layoutControlItem17,
			this.layoutControlItem34,
			this.layoutControlItem37,
			this.layoutControlItem38,
			this.layoutControlGroup12,
			this.layoutControlItem6,
			this.layoutControlItem33,
			this.layoutControlItem35,
			this.layoutControlItem46});
			this.layoutControlGroup7.LayoutMode = DevExpress.XtraLayout.Utils.LayoutMode.Table;
			this.layoutControlGroup7.Location = new System.Drawing.Point(0, 0);
			this.layoutControlGroup7.Name = "layoutControlGroup7";
			this.layoutControlGroup7.OptionsTableLayoutGroup.AutoSizeDefaultDefinitionLength = 1;
			columnDefinition25.SizeType = System.Windows.Forms.SizeType.Absolute;
			columnDefinition25.Width = 9D;
			columnDefinition26.SizeType = System.Windows.Forms.SizeType.AutoSize;
			columnDefinition26.Width = 1D;
			columnDefinition27.SizeType = System.Windows.Forms.SizeType.Percent;
			columnDefinition27.Width = 100D;
			columnDefinition28.SizeType = System.Windows.Forms.SizeType.Absolute;
			columnDefinition28.Width = 9D;
			this.layoutControlGroup7.OptionsTableLayoutGroup.ColumnDefinitions.AddRange(new DevExpress.XtraLayout.ColumnDefinition[] {
			columnDefinition25,
			columnDefinition26,
			columnDefinition27,
			columnDefinition28});
			rowDefinition18.Height = 1D;
			rowDefinition18.SizeType = System.Windows.Forms.SizeType.AutoSize;
			rowDefinition19.Height = 1D;
			rowDefinition19.SizeType = System.Windows.Forms.SizeType.AutoSize;
			rowDefinition20.Height = 1D;
			rowDefinition20.SizeType = System.Windows.Forms.SizeType.AutoSize;
			rowDefinition21.Height = 1D;
			rowDefinition21.SizeType = System.Windows.Forms.SizeType.AutoSize;
			rowDefinition22.Height = 1D;
			rowDefinition22.SizeType = System.Windows.Forms.SizeType.AutoSize;
			rowDefinition23.Height = 5D;
			rowDefinition23.SizeType = System.Windows.Forms.SizeType.Absolute;
			this.layoutControlGroup7.OptionsTableLayoutGroup.RowDefinitions.AddRange(new DevExpress.XtraLayout.RowDefinition[] {
			rowDefinition18,
			rowDefinition19,
			rowDefinition20,
			rowDefinition21,
			rowDefinition22,
			rowDefinition23});
			this.layoutControlGroup7.Size = new System.Drawing.Size(351, 1);
			this.layoutControlItem17.Control = this.lbHorzAlign;
			this.layoutControlItem17.ControlAlignment = System.Drawing.ContentAlignment.MiddleLeft;
			this.layoutControlItem17.Location = new System.Drawing.Point(9, 3);
			this.layoutControlItem17.Name = "layoutControlItem17";
			this.layoutControlItem17.OptionsTableLayoutItem.ColumnIndex = 1;
			this.layoutControlItem17.OptionsTableLayoutItem.RowIndex = 3;
			this.layoutControlItem17.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
			this.layoutControlItem17.Size = new System.Drawing.Size(1, 1);
			this.layoutControlItem17.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem17.TextVisible = false;
			this.layoutControlItem17.TrimClientAreaToControl = false;
			this.layoutControlItem34.Control = this.lbPosition;
			this.layoutControlItem34.ControlAlignment = System.Drawing.ContentAlignment.MiddleLeft;
			this.layoutControlItem34.Location = new System.Drawing.Point(9, 2);
			this.layoutControlItem34.Name = "layoutControlItem34";
			this.layoutControlItem34.OptionsTableLayoutItem.ColumnIndex = 1;
			this.layoutControlItem34.OptionsTableLayoutItem.RowIndex = 2;
			this.layoutControlItem34.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
			this.layoutControlItem34.Size = new System.Drawing.Size(1, 1);
			this.layoutControlItem34.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem34.TextVisible = false;
			this.layoutControlItem34.TrimClientAreaToControl = false;
			this.layoutControlItem37.Control = this.lkpImageHAlign;
			this.layoutControlItem37.ControlAlignment = System.Drawing.ContentAlignment.MiddleLeft;
			this.layoutControlItem37.Location = new System.Drawing.Point(10, 3);
			this.layoutControlItem37.Name = "layoutControlItem37";
			this.layoutControlItem37.OptionsTableLayoutItem.ColumnIndex = 2;
			this.layoutControlItem37.OptionsTableLayoutItem.RowIndex = 3;
			this.layoutControlItem37.Size = new System.Drawing.Size(332, 1);
			this.layoutControlItem37.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem37.TextVisible = false;
			this.layoutControlItem37.TrimClientAreaToControl = false;
			this.layoutControlItem38.Control = this.lkpImageVAlign;
			this.layoutControlItem38.ControlAlignment = System.Drawing.ContentAlignment.MiddleLeft;
			this.layoutControlItem38.Location = new System.Drawing.Point(10, 4);
			this.layoutControlItem38.Name = "layoutControlItem38";
			this.layoutControlItem38.OptionsTableLayoutItem.ColumnIndex = 2;
			this.layoutControlItem38.OptionsTableLayoutItem.RowIndex = 4;
			this.layoutControlItem38.Size = new System.Drawing.Size(332, 1);
			this.layoutControlItem38.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem38.TextVisible = false;
			this.layoutControlItem38.TrimClientAreaToControl = false;
			this.layoutControlGroup12.GroupBordersVisible = false;
			this.layoutControlGroup12.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
			this.layoutControlItem39,
			this.layoutControlItem36});
			this.layoutControlGroup12.LayoutMode = DevExpress.XtraLayout.Utils.LayoutMode.Table;
			this.layoutControlGroup12.Location = new System.Drawing.Point(10, 2);
			this.layoutControlGroup12.Name = "layoutControlGroup12";
			this.layoutControlGroup12.OptionsTableLayoutGroup.AutoSizeDefaultDefinitionLength = 1;
			columnDefinition23.SizeType = System.Windows.Forms.SizeType.Percent;
			columnDefinition23.Width = 100D;
			columnDefinition24.SizeType = System.Windows.Forms.SizeType.AutoSize;
			columnDefinition24.Width = 1D;
			this.layoutControlGroup12.OptionsTableLayoutGroup.ColumnDefinitions.AddRange(new DevExpress.XtraLayout.ColumnDefinition[] {
			columnDefinition23,
			columnDefinition24});
			rowDefinition16.Height = 1D;
			rowDefinition16.SizeType = System.Windows.Forms.SizeType.AutoSize;
			rowDefinition17.Height = 1D;
			rowDefinition17.SizeType = System.Windows.Forms.SizeType.AutoSize;
			this.layoutControlGroup12.OptionsTableLayoutGroup.RowDefinitions.AddRange(new DevExpress.XtraLayout.RowDefinition[] {
			rowDefinition16,
			rowDefinition17});
			this.layoutControlGroup12.OptionsTableLayoutItem.ColumnIndex = 2;
			this.layoutControlGroup12.OptionsTableLayoutItem.RowIndex = 2;
			this.layoutControlGroup12.Size = new System.Drawing.Size(332, 1);
			this.layoutControlItem39.Control = this.chbTiling;
			this.layoutControlItem39.ControlAlignment = System.Drawing.ContentAlignment.MiddleLeft;
			this.layoutControlItem39.Location = new System.Drawing.Point(331, 0);
			this.layoutControlItem39.Name = "layoutControlItem39";
			this.layoutControlItem39.OptionsTableLayoutItem.ColumnIndex = 1;
			this.layoutControlItem39.Size = new System.Drawing.Size(1, 1);
			this.layoutControlItem39.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem39.TextVisible = false;
			this.layoutControlItem39.TrimClientAreaToControl = false;
			this.layoutControlItem36.Control = this.lkpImageView;
			this.layoutControlItem36.ControlAlignment = System.Drawing.ContentAlignment.MiddleLeft;
			this.layoutControlItem36.Location = new System.Drawing.Point(0, 0);
			this.layoutControlItem36.Name = "layoutControlItem36";
			this.layoutControlItem36.Size = new System.Drawing.Size(331, 1);
			this.layoutControlItem36.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem36.TextVisible = false;
			this.layoutControlItem36.TrimClientAreaToControl = false;
			this.layoutControlItem6.Control = this.lbVertAlign;
			this.layoutControlItem6.ControlAlignment = System.Drawing.ContentAlignment.MiddleLeft;
			this.layoutControlItem6.Location = new System.Drawing.Point(9, 4);
			this.layoutControlItem6.Name = "layoutControlItem6";
			this.layoutControlItem6.OptionsTableLayoutItem.ColumnIndex = 1;
			this.layoutControlItem6.OptionsTableLayoutItem.RowIndex = 4;
			this.layoutControlItem6.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
			this.layoutControlItem6.Size = new System.Drawing.Size(1, 1);
			this.layoutControlItem6.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem6.TextVisible = false;
			this.layoutControlItem6.TrimClientAreaToControl = false;
			this.layoutControlItem33.Control = this.lbLoadImage;
			this.layoutControlItem33.ControlAlignment = System.Drawing.ContentAlignment.MiddleLeft;
			this.layoutControlItem33.Location = new System.Drawing.Point(9, 1);
			this.layoutControlItem33.Name = "layoutControlItem33";
			this.layoutControlItem33.OptionsTableLayoutItem.ColumnIndex = 1;
			this.layoutControlItem33.OptionsTableLayoutItem.RowIndex = 1;
			this.layoutControlItem33.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
			this.layoutControlItem33.Size = new System.Drawing.Size(1, 1);
			this.layoutControlItem33.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem33.TextVisible = false;
			this.layoutControlItem33.TrimClientAreaToControl = false;
			this.layoutControlItem35.Control = this.btnSelectPicture;
			this.layoutControlItem35.ControlAlignment = System.Drawing.ContentAlignment.MiddleCenter;
			this.layoutControlItem35.Location = new System.Drawing.Point(10, 1);
			this.layoutControlItem35.Name = "layoutControlItem35";
			this.layoutControlItem35.OptionsTableLayoutItem.ColumnIndex = 2;
			this.layoutControlItem35.OptionsTableLayoutItem.RowIndex = 1;
			this.layoutControlItem35.Size = new System.Drawing.Size(332, 1);
			this.layoutControlItem35.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem35.TextVisible = false;
			this.layoutControlItem35.TrimClientAreaToControl = false;
			this.layoutControlItem46.Control = this.emptySpaceLabelControl7;
			this.layoutControlItem46.Location = new System.Drawing.Point(9, 0);
			this.layoutControlItem46.Name = "layoutControlItem46";
			this.layoutControlItem46.OptionsTableLayoutItem.ColumnIndex = 1;
			this.layoutControlItem46.OptionsTableLayoutItem.ColumnSpan = 2;
			this.layoutControlItem46.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
			this.layoutControlItem46.Size = new System.Drawing.Size(333, 1);
			this.layoutControlItem46.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem46.TextVisible = false;
			this.grpBoxZOrder.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
			this.layoutControlItem7,
			this.layoutControlItem9});
			this.grpBoxZOrder.LayoutMode = DevExpress.XtraLayout.Utils.LayoutMode.Table;
			this.grpBoxZOrder.Location = new System.Drawing.Point(270, 222);
			this.grpBoxZOrder.Name = "grpBoxZOrder";
			this.grpBoxZOrder.OptionsTableLayoutGroup.AutoSizeDefaultDefinitionLength = 1;
			columnDefinition30.SizeType = System.Windows.Forms.SizeType.AutoSize;
			columnDefinition30.Width = 82D;
			this.grpBoxZOrder.OptionsTableLayoutGroup.ColumnDefinitions.AddRange(new DevExpress.XtraLayout.ColumnDefinition[] {
			columnDefinition30});
			rowDefinition26.Height = 19D;
			rowDefinition26.SizeType = System.Windows.Forms.SizeType.AutoSize;
			rowDefinition27.Height = 100D;
			rowDefinition27.SizeType = System.Windows.Forms.SizeType.Percent;
			rowDefinition28.Height = 19D;
			rowDefinition28.SizeType = System.Windows.Forms.SizeType.AutoSize;
			this.grpBoxZOrder.OptionsTableLayoutGroup.RowDefinitions.AddRange(new DevExpress.XtraLayout.RowDefinition[] {
			rowDefinition26,
			rowDefinition27,
			rowDefinition28});
			this.grpBoxZOrder.OptionsTableLayoutItem.ColumnIndex = 2;
			this.grpBoxZOrder.OptionsTableLayoutItem.RowIndex = 1;
			this.grpBoxZOrder.Padding = new DevExpress.XtraLayout.Utils.Padding(8, 8, 10, 10);
			this.grpBoxZOrder.Size = new System.Drawing.Size(104, 94);
			resources.ApplyResources(this.grpBoxZOrder, "grpBoxZOrder");
			this.layoutControlItem7.Control = this.rbInFront;
			this.layoutControlItem7.ControlAlignment = System.Drawing.ContentAlignment.MiddleLeft;
			this.layoutControlItem7.Location = new System.Drawing.Point(0, 0);
			this.layoutControlItem7.Name = "layoutControlItem7";
			this.layoutControlItem7.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
			this.layoutControlItem7.Size = new System.Drawing.Size(82, 19);
			this.layoutControlItem7.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem7.TextVisible = false;
			this.layoutControlItem7.TrimClientAreaToControl = false;
			this.layoutControlItem9.Control = this.rbBehind;
			this.layoutControlItem9.ControlAlignment = System.Drawing.ContentAlignment.MiddleLeft;
			this.layoutControlItem9.Location = new System.Drawing.Point(0, 31);
			this.layoutControlItem9.Name = "layoutControlItem9";
			this.layoutControlItem9.OptionsTableLayoutItem.RowIndex = 2;
			this.layoutControlItem9.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
			this.layoutControlItem9.Size = new System.Drawing.Size(82, 19);
			this.layoutControlItem9.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem9.TextVisible = false;
			this.layoutControlItem9.TrimClientAreaToControl = false;
			this.grpBoxPageRange.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
			this.layoutControlItem22,
			this.layoutControlItem23,
			this.layoutControlItem24,
			this.layoutControlItem10,
			this.layoutControlItem47});
			this.grpBoxPageRange.LayoutMode = DevExpress.XtraLayout.Utils.LayoutMode.Table;
			this.grpBoxPageRange.Location = new System.Drawing.Point(374, 222);
			this.grpBoxPageRange.Name = "grpBoxPageRange";
			this.grpBoxPageRange.OptionsTableLayoutGroup.AutoSizeDefaultDefinitionLength = 1;
			columnDefinition31.SizeType = System.Windows.Forms.SizeType.Absolute;
			columnDefinition31.Width = 5D;
			columnDefinition32.SizeType = System.Windows.Forms.SizeType.AutoSize;
			columnDefinition32.Width = 45D;
			columnDefinition33.SizeType = System.Windows.Forms.SizeType.AutoSize;
			columnDefinition33.Width = 59D;
			columnDefinition34.SizeType = System.Windows.Forms.SizeType.Percent;
			columnDefinition34.Width = 100D;
			columnDefinition35.SizeType = System.Windows.Forms.SizeType.Absolute;
			columnDefinition35.Width = 5D;
			this.grpBoxPageRange.OptionsTableLayoutGroup.ColumnDefinitions.AddRange(new DevExpress.XtraLayout.ColumnDefinition[] {
			columnDefinition31,
			columnDefinition32,
			columnDefinition33,
			columnDefinition34,
			columnDefinition35});
			rowDefinition29.Height = 5D;
			rowDefinition29.SizeType = System.Windows.Forms.SizeType.Absolute;
			rowDefinition30.Height = 30D;
			rowDefinition30.SizeType = System.Windows.Forms.SizeType.AutoSize;
			rowDefinition31.Height = 30D;
			rowDefinition31.SizeType = System.Windows.Forms.SizeType.AutoSize;
			rowDefinition32.Height = 5D;
			rowDefinition32.SizeType = System.Windows.Forms.SizeType.Absolute;
			this.grpBoxPageRange.OptionsTableLayoutGroup.RowDefinitions.AddRange(new DevExpress.XtraLayout.RowDefinition[] {
			rowDefinition29,
			rowDefinition30,
			rowDefinition31,
			rowDefinition32});
			this.grpBoxPageRange.OptionsTableLayoutItem.ColumnIndex = 3;
			this.grpBoxPageRange.OptionsTableLayoutItem.RowIndex = 1;
			this.grpBoxPageRange.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
			this.grpBoxPageRange.Size = new System.Drawing.Size(253, 94);
			resources.ApplyResources(this.grpBoxPageRange, "grpBoxPageRange");
			this.layoutControlItem22.Control = this.tePageRange;
			this.layoutControlItem22.ControlAlignment = System.Drawing.ContentAlignment.MiddleLeft;
			this.layoutControlItem22.Location = new System.Drawing.Point(109, 5);
			this.layoutControlItem22.Name = "tePageRangeitem";
			this.layoutControlItem22.OptionsTableLayoutItem.ColumnIndex = 3;
			this.layoutControlItem22.OptionsTableLayoutItem.RowIndex = 1;
			this.layoutControlItem22.Padding = new DevExpress.XtraLayout.Utils.Padding(2, 2, 5, 5);
			this.layoutControlItem22.Size = new System.Drawing.Size(133, 30);
			this.layoutControlItem22.TextLocation = DevExpress.Utils.Locations.Bottom;
			this.layoutControlItem22.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem22.TextVisible = false;
			this.layoutControlItem22.TrimClientAreaToControl = false;
			this.layoutControlItem23.Control = this.rbAll;
			this.layoutControlItem23.ControlAlignment = System.Drawing.ContentAlignment.MiddleLeft;
			this.layoutControlItem23.Location = new System.Drawing.Point(5, 5);
			this.layoutControlItem23.Name = "rbAllitem";
			this.layoutControlItem23.OptionsTableLayoutItem.ColumnIndex = 1;
			this.layoutControlItem23.OptionsTableLayoutItem.RowIndex = 1;
			this.layoutControlItem23.Padding = new DevExpress.XtraLayout.Utils.Padding(2, 10, 2, 2);
			this.layoutControlItem23.Size = new System.Drawing.Size(45, 30);
			this.layoutControlItem23.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem23.TextVisible = false;
			this.layoutControlItem23.TrimClientAreaToControl = false;
			this.layoutControlItem24.Control = this.rbPages;
			this.layoutControlItem24.ControlAlignment = System.Drawing.ContentAlignment.MiddleLeft;
			this.layoutControlItem24.Location = new System.Drawing.Point(50, 5);
			this.layoutControlItem24.Name = "rbPagesitem";
			this.layoutControlItem24.OptionsTableLayoutItem.ColumnIndex = 2;
			this.layoutControlItem24.OptionsTableLayoutItem.RowIndex = 1;
			this.layoutControlItem24.Size = new System.Drawing.Size(59, 30);
			this.layoutControlItem24.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem24.TextVisible = false;
			this.layoutControlItem24.TrimClientAreaToControl = false;
			this.layoutControlItem10.Control = this.lbPageRange;
			this.layoutControlItem10.Location = new System.Drawing.Point(5, 35);
			this.layoutControlItem10.Name = "layoutControlItem10";
			this.layoutControlItem10.OptionsTableLayoutItem.ColumnIndex = 1;
			this.layoutControlItem10.OptionsTableLayoutItem.ColumnSpan = 3;
			this.layoutControlItem10.OptionsTableLayoutItem.RowIndex = 2;
			this.layoutControlItem10.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
			this.layoutControlItem10.Size = new System.Drawing.Size(237, 30);
			this.layoutControlItem10.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem10.TextVisible = false;
			this.layoutControlItem47.Control = this.labelControl2;
			this.layoutControlItem47.Location = new System.Drawing.Point(242, 5);
			this.layoutControlItem47.Name = "layoutControlItem47";
			this.layoutControlItem47.OptionsTableLayoutItem.ColumnIndex = 4;
			this.layoutControlItem47.OptionsTableLayoutItem.RowIndex = 1;
			this.layoutControlItem47.OptionsTableLayoutItem.RowSpan = 2;
			this.layoutControlItem47.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
			this.layoutControlItem47.Size = new System.Drawing.Size(5, 60);
			this.layoutControlItem47.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem47.TextVisible = false;
			this.grpButtons.GroupBordersVisible = false;
			this.grpButtons.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
			this.layoutControlItem4,
			this.layoutControlItem2,
			this.layoutControlItem3,
			this.layoutControlItem12,
			this.layoutControlItem40});
			this.grpButtons.LayoutMode = DevExpress.XtraLayout.Utils.LayoutMode.Table;
			this.grpButtons.Location = new System.Drawing.Point(0, 316);
			this.grpButtons.Name = "grpButtons";
			this.grpButtons.OptionsTableLayoutGroup.AutoSizeDefaultDefinitionLength = 1;
			columnDefinition36.SizeType = System.Windows.Forms.SizeType.Absolute;
			columnDefinition36.Width = 2D;
			columnDefinition37.SizeType = System.Windows.Forms.SizeType.Percent;
			columnDefinition37.Width = 15.359477124183007D;
			columnDefinition38.SizeType = System.Windows.Forms.SizeType.Percent;
			columnDefinition38.Width = 53.921568627450981D;
			columnDefinition39.SizeType = System.Windows.Forms.SizeType.Percent;
			columnDefinition39.Width = 15.359477124183007D;
			columnDefinition40.SizeType = System.Windows.Forms.SizeType.Absolute;
			columnDefinition40.Width = 1D;
			columnDefinition41.SizeType = System.Windows.Forms.SizeType.Percent;
			columnDefinition41.Width = 15.359477124183007D;
			columnDefinition42.SizeType = System.Windows.Forms.SizeType.Absolute;
			columnDefinition42.Width = 2D;
			this.grpButtons.OptionsTableLayoutGroup.ColumnDefinitions.AddRange(new DevExpress.XtraLayout.ColumnDefinition[] {
			columnDefinition36,
			columnDefinition37,
			columnDefinition38,
			columnDefinition39,
			columnDefinition40,
			columnDefinition41,
			columnDefinition42});
			rowDefinition33.Height = 18D;
			rowDefinition33.SizeType = System.Windows.Forms.SizeType.AutoSize;
			rowDefinition34.Height = 26D;
			rowDefinition34.SizeType = System.Windows.Forms.SizeType.AutoSize;
			this.grpButtons.OptionsTableLayoutGroup.RowDefinitions.AddRange(new DevExpress.XtraLayout.RowDefinition[] {
			rowDefinition33,
			rowDefinition34});
			this.grpButtons.OptionsTableLayoutItem.ColumnSpan = 4;
			this.grpButtons.OptionsTableLayoutItem.RowIndex = 2;
			this.grpButtons.Size = new System.Drawing.Size(627, 44);
			this.layoutControlItem4.Control = this.btnOK;
			this.layoutControlItem4.ControlAlignment = System.Drawing.ContentAlignment.MiddleCenter;
			this.layoutControlItem4.Location = new System.Drawing.Point(432, 18);
			this.layoutControlItem4.Name = "btnOKitem";
			this.layoutControlItem4.OptionsTableLayoutItem.ColumnIndex = 3;
			this.layoutControlItem4.OptionsTableLayoutItem.RowIndex = 1;
			this.layoutControlItem4.Size = new System.Drawing.Size(96, 26);
			this.layoutControlItem4.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem4.TextVisible = false;
			this.layoutControlItem4.TrimClientAreaToControl = false;
			this.layoutControlItem2.Control = this.btnClear;
			this.layoutControlItem2.ControlAlignment = System.Drawing.ContentAlignment.MiddleCenter;
			this.layoutControlItem2.Location = new System.Drawing.Point(2, 18);
			this.layoutControlItem2.Name = "btnClearitem";
			this.layoutControlItem2.OptionsTableLayoutItem.ColumnIndex = 1;
			this.layoutControlItem2.OptionsTableLayoutItem.RowIndex = 1;
			this.layoutControlItem2.Size = new System.Drawing.Size(95, 26);
			this.layoutControlItem2.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem2.TextVisible = false;
			this.layoutControlItem2.TrimClientAreaToControl = false;
			this.layoutControlItem3.Control = this.btnCancel;
			this.layoutControlItem3.ControlAlignment = System.Drawing.ContentAlignment.MiddleCenter;
			this.layoutControlItem3.Location = new System.Drawing.Point(529, 18);
			this.layoutControlItem3.Name = "btnCancelitem";
			this.layoutControlItem3.OptionsTableLayoutItem.ColumnIndex = 5;
			this.layoutControlItem3.OptionsTableLayoutItem.RowIndex = 1;
			this.layoutControlItem3.Size = new System.Drawing.Size(96, 26);
			this.layoutControlItem3.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem3.TextVisible = false;
			this.layoutControlItem3.TrimClientAreaToControl = false;
			this.layoutControlItem12.Control = this.labelControl5;
			this.layoutControlItem12.Location = new System.Drawing.Point(0, 0);
			this.layoutControlItem12.Name = "layoutControlItem12";
			this.layoutControlItem12.OptionsTableLayoutItem.ColumnSpan = 7;
			this.layoutControlItem12.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
			this.layoutControlItem12.Size = new System.Drawing.Size(627, 18);
			this.layoutControlItem12.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem12.TextVisible = false;
			this.layoutControlItem40.Control = this.emptySpaceLabelControl3;
			this.layoutControlItem40.Location = new System.Drawing.Point(97, 18);
			this.layoutControlItem40.Name = "layoutControlItem40";
			this.layoutControlItem40.OptionsTableLayoutItem.ColumnIndex = 2;
			this.layoutControlItem40.OptionsTableLayoutItem.RowIndex = 1;
			this.layoutControlItem40.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
			this.layoutControlItem40.Size = new System.Drawing.Size(335, 26);
			this.layoutControlItem40.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem40.TextVisible = false;
			this.layoutControlItem8.Control = this.panelControl2;
			this.layoutControlItem8.Location = new System.Drawing.Point(0, 0);
			this.layoutControlItem8.Name = "layoutControlItem8";
			this.layoutControlItem8.OptionsTableLayoutItem.RowSpan = 2;
			this.layoutControlItem8.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
			this.layoutControlItem8.Size = new System.Drawing.Size(260, 316);
			this.layoutControlItem8.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem8.TextVisible = false;
			this.layoutControlItem41.Control = this.emptySpaceLabelControl2;
			this.layoutControlItem41.Location = new System.Drawing.Point(0, 83);
			this.layoutControlItem41.Name = "layoutControlItem41";
			this.layoutControlItem41.OptionsTableLayoutItem.RowIndex = 4;
			this.layoutControlItem41.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
			this.layoutControlItem41.Size = new System.Drawing.Size(63, 34);
			this.layoutControlItem41.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem41.TextVisible = false;
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.btnCancel;
			this.Controls.Add(this.layoutControl1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "WatermarkEditorForm";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.Load += new System.EventHandler(this.WatermarkEditorForm_Load);
			this.Shown += new System.EventHandler(this.WatermarkEditorForm_Shown);
			((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
			this.layoutControl1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.panelControl2)).EndInit();
			this.panelControl2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.rbBehind.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.rbInFront.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.tePageRange.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.rbAll.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.rbPages.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chbItalic.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chbBold.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.ceWatermarkColor.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.cmbWatermarkFont.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.cmbWatermarkFontSize.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.lkpTextDirection.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.cmbWatermarkText.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.trBarTextTransparency.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.trBarTextTransparency)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.teTransparentValue.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chbTiling.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.lkpImageVAlign.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.lkpImageHAlign.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.lkpImageView.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.trBarImageTransparency.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.trBarImageTransparency)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.teImageTransparentValue.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.tpTextWaterMark)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup9)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem11)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem13)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem14)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup10)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem20)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem21)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem25)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem26)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem27)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem28)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem29)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup11)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem30)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem31)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem19)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem32)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.emptySpaceLabelControl1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem43)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem18)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.tpPictureWatermark)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup8)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem5)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem15)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem16)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup7)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem17)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem34)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem37)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem38)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup12)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem39)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem36)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem6)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem33)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem35)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem46)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.grpBoxZOrder)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem7)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem9)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.grpBoxPageRange)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem22)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem23)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem24)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem10)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem47)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.grpButtons)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem4)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem12)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem40)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem8)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem41)).EndInit();
			this.ResumeLayout(false);
		}
		#endregion
		private void InitComboBoxes() {
			int n = 0;
			dsDirectionMode = new DirectionModeItem[4];
			dsDirectionMode[n++] = new DirectionModeItem(DirectionMode.Horizontal, PreviewLocalizer.GetString(PreviewStringId.WMForm_Direction_Horizontal));
			dsDirectionMode[n++] = new DirectionModeItem(DirectionMode.Vertical, PreviewLocalizer.GetString(PreviewStringId.WMForm_Direction_Vertical));
			dsDirectionMode[n++] = new DirectionModeItem(DirectionMode.BackwardDiagonal, PreviewLocalizer.GetString(PreviewStringId.WMForm_Direction_BackwardDiagonal));
			dsDirectionMode[n++] = new DirectionModeItem(DirectionMode.ForwardDiagonal, PreviewLocalizer.GetString(PreviewStringId.WMForm_Direction_ForwardDiagonal));
			lkpTextDirection.Properties.DataSource = dsDirectionMode;
			lkpTextDirection.Properties.DropDownRows = dsDirectionMode.Length;
			lkpTextDirection.Properties.DisplayMember = "Text";
			lkpTextDirection.Properties.ValueMember = "DirectionMode";
			n = 0;
			dsImageViewMode = new ViewModeItem[3];
			dsImageViewMode[n++] = new ViewModeItem(ImageViewMode.Clip, PreviewLocalizer.GetString(PreviewStringId.WMForm_ImageClip));
			dsImageViewMode[n++] = new ViewModeItem(ImageViewMode.Stretch, PreviewLocalizer.GetString(PreviewStringId.WMForm_ImageStretch));
			dsImageViewMode[n++] = new ViewModeItem(ImageViewMode.Zoom, PreviewLocalizer.GetString(PreviewStringId.WMForm_ImageZoom));
			lkpImageView.Properties.DataSource = dsImageViewMode;
			lkpImageView.Properties.DropDownRows = dsImageViewMode.Length;
			lkpImageView.Properties.DisplayMember = "Text";
			lkpImageView.Properties.ValueMember = "ViewMode";
			n = 0;
			dsImageHAlign = new ImageAlignItem[3];
			dsImageHAlign[n++] = new ImageAlignItem(alignLeft, PreviewLocalizer.GetString(PreviewStringId.WMForm_HorzAlign_Left));
			dsImageHAlign[n++] = new ImageAlignItem(alignCenter, PreviewLocalizer.GetString(PreviewStringId.WMForm_HorzAlign_Center));
			dsImageHAlign[n++] = new ImageAlignItem(alignRight, PreviewLocalizer.GetString(PreviewStringId.WMForm_HorzAlign_Right));
			lkpImageHAlign.Properties.DataSource = dsImageHAlign;
			lkpImageHAlign.Properties.DropDownRows = dsImageHAlign.Length;
			lkpImageHAlign.Properties.DisplayMember = "Text";
			lkpImageHAlign.Properties.ValueMember = "Alignment";
			n = 0;
			dsImageVAlign = new ImageAlignItem[3];
			dsImageVAlign[n++] = new ImageAlignItem(alignTop, PreviewLocalizer.GetString(PreviewStringId.WMForm_VertAlign_Top));
			dsImageVAlign[n++] = new ImageAlignItem(alignMiddle, PreviewLocalizer.GetString(PreviewStringId.WMForm_VertAlign_Middle));
			dsImageVAlign[n++] = new ImageAlignItem(alignBottom, PreviewLocalizer.GetString(PreviewStringId.WMForm_VertAlign_Bottom));
			lkpImageVAlign.Properties.DataSource = dsImageVAlign;
			lkpImageVAlign.Properties.DropDownRows = dsImageVAlign.Length;
			lkpImageVAlign.Properties.DisplayMember = "Text";
			lkpImageVAlign.Properties.ValueMember = "Alignment";
			cmbWatermarkText.EditValue = String.Empty;
			cmbWatermarkText.Properties.Items.AddRange(Design.WMTextConverter.StandardValues);
			byte[] fontSizes = { 36, 40, 44, 48, 54, 60, 66, 72, 80, 90, 96, 105, 120, 144 };
			foreach(byte size in fontSizes)
				cmbWatermarkFontSize.Properties.Items.Add(size.ToString().Trim());
			cmbWatermarkFontSize.SelectedIndex = 0;
			foreach(FontFamily family in FontFamily.Families)
				cmbWatermarkFont.Properties.Items.Add(family.Name);
			cmbWatermarkFont.SelectedIndex = 0;
		}
		private void btnSelectPicture_Click(object sender, System.EventArgs e) {
			System.Diagnostics.Debug.Assert(filterText != null);
			if(filterText == null)
				filterText = defaultFilterText;
			OpenFileDialog openDialog = new OpenFileDialog() {
				Title = PreviewStringId.WMForm_PictureDlg_Title.GetString(),
				Filter = String.Format(filterText, "|*.bmp;*.dib;*.gif;*.jpg;*.jpeg;*.png;*.tiff;*.tif;*.emf;*.wmf")
			};
			if(Native.DialogRunner.ShowDialog(openDialog) == DialogResult.OK && openDialog.FileName.Length > 0) {
				Image image = null;
				try {
					image = GetImageCore(openDialog.FileName);
				} catch(OutOfMemoryException) {
					throw new Exception(PreviewStringId.Msg_BigBitmapToCreate.GetString());
				}
				if(image == null)
					return;
				SetControlEnabled(new Control[] { lkpImageView, lkpImageHAlign, lkpImageVAlign, chbTiling, teImageTransparentValue, trBarImageTransparency }, image != null);
				watermark.Image = image;
				UpdateWatermarkView();
			}
		}
		static Image GetImageCore(string path) {
			if(!File.Exists(path))
				throw new FileNotFoundException(path);
			using(Image image = Image.FromStream(new MemoryStream(File.ReadAllBytes(path)))) {
				return Watermark.CloneImage(image);
			}
		}
		private void UpdateWatermarkView() {
			myPrintControl1.Update(watermark);
		}
		public void Assign(Watermark watermark) {
			this.watermark.CopyFrom(watermark);
			InitControls();
		}
		private void InitControls() {
			canSync = false;
			try {
				InitTextTab();
				InitPictureTab();
				if(watermark.ShowBehind)
					rbBehind.Checked = true;
				else 
					rbInFront.Checked = true;
			} finally {
				canSync = true;
			}
			SyncWatermark();
		}
		private void InitTextTab() {
			this.tePageRange.EditValue = watermark.PageRange;
			cmbWatermarkText.EditValue = watermark.Text;
			ceWatermarkColor.Color = watermark.ForeColor;
			Font f = watermark.Font;
			if(f != null) {
				cmbWatermarkFontSize.EditValue = ((int)f.Size).ToString().Trim();
				cmbWatermarkFont.EditValue = f.Name;
				chbBold.Checked = f.Bold;
				chbItalic.Checked = f.Italic;
			}
			SetControlEnabled(new Control[] { cmbWatermarkFont, cmbWatermarkFontSize, chbBold, chbItalic, ceWatermarkColor, lkpTextDirection, teTransparentValue, trBarTextTransparency }, !String.IsNullOrEmpty(watermark.Text));
			UpdateTransparentValue(watermark.TextTransparency, trBarTextTransparency, teTransparentValue);
			cmbWatermarkText.EditValue = watermark.Text;
			lkpTextDirection.EditValue = watermark.TextDirection;
		}
		private void InitPictureTab() {
			lkpImageView.EditValue = watermark.ImageViewMode;
			int index = Array.IndexOf(contentAlignList, watermark.ImageAlign);
			string[] items = alignList[index].Split(',');
			lkpImageHAlign.EditValue = items[0];
			lkpImageVAlign.EditValue = items[1];
			chbTiling.Checked = watermark.ImageTiling;
			SetControlEnabled(new Control[] { lkpImageView, lkpImageHAlign, lkpImageVAlign, chbTiling, teImageTransparentValue, trBarImageTransparency }, watermark.Image != null);
			UpdateTransparentValue(watermark.ImageTransparency, trBarImageTransparency, teImageTransparentValue);
		}
		private void SetControlEnabled(Control[] controls, bool val) {
			foreach(Control control in controls)
				control.Enabled = val;
		}
		private void OnEditValueChanged(object sender, System.EventArgs e) {
			SyncWatermark();
		}
		void SyncWatermark() {
			if(!canSync)
				return;
			watermark.Text = (string)cmbWatermarkText.EditValue;
			watermark.ShowBehind = rbBehind.Checked;
			watermark.PageRange = (bool)rbPages.EditValue ? (string)tePageRange.EditValue : String.Empty;
			watermark.ImageTiling = chbTiling.Checked;
			watermark.TextTransparency = trBarTextTransparency.Value;
			watermark.ImageTransparency = trBarImageTransparency.Value;
			watermark.ForeColor = ceWatermarkColor.Color;
			watermark.ImageViewMode = GetImageViewMode();
			watermark.TextDirection = GetWatermarkDirection();
			watermark.ImageAlign = GetImageAlignment();
			try {
				try {
					watermark.Font = new Font((string)cmbWatermarkFont.EditValue, GetFontSize(), GetFontStyle());
				} catch { }
				UpdateWatermarkView();
			} catch { }
		}
		private DirectionMode GetWatermarkDirection() {
			return (DirectionMode)lkpTextDirection.EditValue;
		}
		private ImageViewMode GetImageViewMode() {
			return (ImageViewMode)lkpImageView.EditValue;
		}
		private ContentAlignment GetImageAlignment() {
			string vertAling = (string)lkpImageVAlign.EditValue;
			string horzAling = (string)lkpImageHAlign.EditValue;
			int index = Array.IndexOf(alignList, ToString(vertAling, horzAling));
			return (index != -1 && contentAlignList.Length > index) ? contentAlignList[index] :
				ContentAlignment.MiddleCenter;
		}
		FontStyle GetFontStyle() {
			FontStyle style = FontStyle.Regular;
			if(chbBold.Checked)
				style |= FontStyle.Bold;
			if(chbItalic.Checked)
				style |= FontStyle.Italic;
			return style;
		}
		float GetFontSize() {
			return Convert.ToSingle(cmbWatermarkFontSize.EditValue);
		}
		private void teImageTransparentValue_EditValueChanged(object sender, System.EventArgs e) {
			UpdateTransparentValue(ToInt32(((TextEdit)sender).EditValue), trBarImageTransparency, teImageTransparentValue);
			OnEditValueChanged(null, EventArgs.Empty);
		}
		private void trBarPictureTransparency_EditValueChanged(object sender, EventArgs e) {
			UpdateTransparentValue(ToInt32(((TrackBarControl)sender).EditValue), trBarImageTransparency, teImageTransparentValue);
			OnEditValueChanged(null, EventArgs.Empty);
		}
		private void teTransparentValue_EditValueChanged(object sender, System.EventArgs e) {
			UpdateTransparentValue(ToInt32(ToInt32(((TextEdit)sender).EditValue)), trBarTextTransparency, teTransparentValue);
			OnEditValueChanged(null, EventArgs.Empty);
		}
		private void trBarTextTransparency_EditValueChanged(object sender, EventArgs e) {
			UpdateTransparentValue(ToInt32(((TrackBarControl)sender).EditValue), trBarTextTransparency, teTransparentValue);
			OnEditValueChanged(null, EventArgs.Empty);
		}
		int ToInt32(object obj) {
			try {
				return Convert.ToInt32(obj);
			} catch { }
			return 0;
		}
		private void UpdateTransparentValue(int val, DevExpress.XtraEditors.TrackBarControl trackBar, TextEdit textEdit) {
			val = Math.Max(0, Math.Min(val, 255));
			trackBar.Value = val;
			textEdit.EditValue = Convert.ToString(val, 10).Trim();
		}
		private void btnClear_Click(object sender, System.EventArgs e) {
			watermark = new Watermark();
			InitControls();
			rbAll.Checked = true;
			UpdateWatermarkView();
		}
		private void rbPages_EditValueChanged(object sender, System.EventArgs e) {
			if(rbPages.Checked)
				tePageRange.Focus();
			else
				tePageRange.EditValue = "";
		}
		private void tePageRange_EditValueChanged(object sender, System.EventArgs e) {
			rbPages.Checked = !tePageRange.EditValue.Equals("");
			SyncWatermark();
		}
		private void btnOK_Click(object sender, System.EventArgs e) {
			try {
				if(GetFontSize() <= 0)
					throw new FormatException(PreviewStringId.Msg_FontInvalidNumber.GetString());
				DialogResult = DialogResult.OK;
			} catch(Exception ex) {
				Tracer.TraceError(NativeSR.TraceSource, ex);
				NotificationService.ShowException<PrintingSystemBase>(LookAndFeel, this, ex);
				cmbWatermarkFontSize.Focus();
			}
		}
		protected override void OnRightToLeftChanged(EventArgs e) {
			base.OnRightToLeftChanged(e);
			isRTLChanged = true;
		}
		private void WatermarkEditorForm_Load(object sender, EventArgs e) {
			InitializeLayout();
			myPrintControl1.ViewWholePage();
		}
		void InitializeLayout() {
			InitializeGroupButtonsLayout();
			layoutControlItem1.SelectedTabPageIndex = 1;
			Size minLayoutControlSize1 = (layoutControl1 as DevExpress.Utils.Controls.IXtraResizableControl).MinSize;
			layoutControlItem1.SelectedTabPageIndex = 0;
			Size minLayoutControlSize2 = (layoutControl1 as DevExpress.Utils.Controls.IXtraResizableControl).MinSize;
			Size minLayoutControlSize = new Size(Math.Max(minLayoutControlSize1.Width, minLayoutControlSize2.Width),
				Math.Max(minLayoutControlSize1.Height, minLayoutControlSize2.Height));
			if(minLayoutControlSize.Width > ClientSize.Width || minLayoutControlSize.Height > ClientSize.Height)
				ClientSize = new Size(Math.Max(minLayoutControlSize.Width, ClientSize.Width), Math.Max(minLayoutControlSize.Height, ClientSize.Height));
			if(isRTLChanged)
				DevExpress.XtraPrinting.Native.RTLHelper.ConvertGroupControlAlignments(layoutControlGroup1);
		}
		void InitializeGroupButtonsLayout() {
			int btnOKBestWidth = btnOK.CalcBestSize().Width;
			int btnCancelBestWidth = btnCancel.CalcBestSize().Width;
			int btnClearBestWidth = btnClear.CalcBestSize().Width;
			if(btnOKBestWidth <= btnOK.Width && btnCancelBestWidth <= btnCancel.Width && btnClearBestWidth <= btnClear.Width)
				return;
			int delta = 0;
			if(btnClearBestWidth > btnClear.Width) {
				delta += btnClearBestWidth - btnClear.Width;
				grpButtons.OptionsTableLayoutGroup.ColumnDefinitions[1].Width = btnClearBestWidth + 2 + 2;
			}
			if(btnOKBestWidth > btnOK.Width || btnCancelBestWidth > btnCancel.Width) {
				int btnCancelOKActualSize = Math.Max(btnOKBestWidth, btnCancelBestWidth);
				delta += 2 * btnCancelOKActualSize - btnCancel.Width - btnOK.Width;
				grpButtons.OptionsTableLayoutGroup.ColumnDefinitions[3].Width =
				grpButtons.OptionsTableLayoutGroup.ColumnDefinitions[5].Width = btnCancelOKActualSize + 2 + 2;
			} else {  
				grpButtons.OptionsTableLayoutGroup.ColumnDefinitions[3].Width = grpButtons.OptionsTableLayoutGroup.ColumnDefinitions[3].Width;
				grpButtons.OptionsTableLayoutGroup.ColumnDefinitions[5].Width = grpButtons.OptionsTableLayoutGroup.ColumnDefinitions[5].Width;
			}
			grpButtons.OptionsTableLayoutGroup.ColumnDefinitions[2].Width -= delta;
		}
		private void cmbWatermarkText_TextChanged(object sender, EventArgs e) {
			SyncWatermark();
			SetControlEnabled(new Control[] { cmbWatermarkFont, cmbWatermarkFontSize, chbBold, chbItalic, ceWatermarkColor, lkpTextDirection, teTransparentValue, trBarTextTransparency }, !String.IsNullOrEmpty(watermark.Text));
		}
		private void WatermarkEditorForm_Shown(object sender, EventArgs e) {
			this.btnOK.Focus();
		}
	}
}
