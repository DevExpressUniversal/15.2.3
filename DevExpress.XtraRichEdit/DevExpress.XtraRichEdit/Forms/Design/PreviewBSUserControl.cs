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
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraRichEdit.Layout.Export;
using DevExpress.Office.Drawing;
using DevExpress.Utils.Drawing;
using DevExpress.XtraRichEdit.Drawing;
using DevExpress.Office.Layout;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Layout;
using DevExpress.XtraRichEdit.Layout.TableLayout;
using DevExpress.Office;
namespace DevExpress.XtraRichEdit.Forms.Design {
	[DXToolboxItem(false)]
	public partial class PreviewBSUserControl : UserControl {
		#region Fields
		DocumentModel documentModel;
		Color fillColor;
		BorderLineState verticalLineRight;
		BorderLineState verticalLineLeft;
		BorderLineState verticalLineIn;
		BorderLineState horizontalLineIn;
		BorderLineState horizontalLineUp;
		BorderLineState horizontalLineDown;
		bool drawPageBorderHorizontalDown;
		bool drawPageBorderHorizontalUp;
		bool drawPageBorderVerticalRight;
		bool drawPageBorderVerticalLeft;
		bool drawColumns;
		bool drawParagraph;
		#endregion
		#region Property
		public DocumentModel DocumentModel { get { return documentModel; } set { documentModel = value; } }
		public BorderShadingUserControl BorderInfoSource { get; set; }
		public Color FillColor {
			get { return fillColor; }
			set {
				fillColor = value;
				Invalidate();
				Update();
			}
		}
		public BorderLineState VerticalLineRight {
			get { return verticalLineRight; }
			set {
				verticalLineRight = value;
				Invalidate();
				Update();
			}
		}
		public BorderLineState VerticalLineLeft {
			get { return verticalLineLeft; }
			set {
				verticalLineLeft = value;
				Invalidate();
				Update();
			}
		}
		public BorderLineState VerticalLineIn {
			get { return verticalLineIn; }
			set {
				verticalLineIn = value;
				Invalidate();
				Update();
			}
		}
		public BorderLineState HorizontalLineIn {
			get { return horizontalLineIn; }
			set {
				horizontalLineIn = value;
				Invalidate();
				Update();
			}
		}
		public BorderLineState HorizontalLineUp {
			get { return horizontalLineUp; }
			set {
				horizontalLineUp = value;
				Invalidate();
				Update();
			}
		}
		public BorderLineState HorizontalLineDown {
			get { return horizontalLineDown; }
			set {
				horizontalLineDown = value;
				Invalidate();
				Update();
			}
		}
		public bool DrawPageBorderHorizontalDown {
			get { return drawPageBorderHorizontalDown; }
			set {
				drawPageBorderHorizontalDown = value;
				Invalidate();
				Update();
			}
		}
		public bool DrawPageBorderHorizontalUp {
			get { return drawPageBorderHorizontalUp; }
			set {
				drawPageBorderHorizontalUp = value;
				Invalidate();
				Update();
			}
		}
		public bool DrawPageBorderVerticalRight {
			get { return drawPageBorderVerticalRight; }
			set {
				drawPageBorderVerticalRight = value;
				Invalidate();
				Update();
			}
		}
		public bool DrawPageBorderVerticalLeft {
			get { return drawPageBorderVerticalLeft; }
			set {
				drawPageBorderVerticalLeft = value;
				Invalidate();
				Update();
			}
		}
		public bool DrawColumns {
			get { return drawColumns; }
			set {
				drawColumns = value;
				Invalidate();
				Update();
			}
		}
		public bool DrawParagraph {
			get { return drawParagraph; }
			set {
				drawParagraph = value;
				Invalidate();
				Update();
			}
		}
		#endregion
		public PreviewBSUserControl() {			
			InitializeComponent();
		}
		protected override CreateParams CreateParams {
			get {
				return DevExpress.XtraRichEdit.Native.RightToLeftHelper.PatchCreateParams(base.CreateParams, this);
			}
		}
		void MyRectangle(PaintEventArgs e, int width, int height, int x, int y) {
			int heightRectangle = 3;
			int[] pageBorder = { 1, 1, 1, 1 };
			int countRectangle = (height - pageBorder[0] - pageBorder[1]) / (2 * heightRectangle);
			for (int i = 0; i < countRectangle; i++)
				e.Graphics.FillRectangle(Brushes.LightGray, 
					new Rectangle(x + pageBorder[2], y + i * 2 * heightRectangle + pageBorder[0], width - pageBorder[3] - pageBorder[2], heightRectangle));
		}		
		protected override void OnPaint(PaintEventArgs e) {
			if (documentModel == null)
				return;
			using (Painter painter = new GdiPlusPainter(new GraphicsCache(e.Graphics))) {
				DocumentLayoutUnitConverter converter = new DocumentLayoutUnitPixelsConverter(e.Graphics.DpiX);
				RichEditPatternLinePainter horizontalLinePainter = new RichEditHorizontalPatternLinePainter(painter, converter);
				RichEditPatternLinePainter verticalLinePainter = new RichEditVerticalPatternLinePainter(painter, converter);
				GraphicsDocumentLayoutExporterTableBorder exporter = new GraphicsDocumentLayoutExporterTableBorder(documentModel, painter, horizontalLinePainter, verticalLinePainter);
				DocumentModelUnitToLayoutUnitConverter toLayoutUnitConverter = documentModel.UnitConverter.CreateConverterToLayoutUnits(Office.DocumentLayoutUnit.Pixel, e.Graphics.DpiX);
				BorderShadingFormHelper helper = new BorderShadingFormHelper();
				int verticalLineRightWidth = 0;
				int verticalLineLeftWidth = 0;
				int verticalLineInWidth = 0;
				int horizontalLineInWidth = 0;
				int horizontalLineUpWidth = 0;
				int horizontalLineDownWidth = 0;
				if (fillColor != Color.Empty)
					e.Graphics.FillRectangle(new SolidBrush(fillColor), 0, 0, Width, Height);
				if (VerticalLineRight == BorderLineState.Known) 
				   verticalLineRightWidth = helper.DrawVerticalLineRight(BorderInfoSource, documentModel, toLayoutUnitConverter, exporter, Width, Height);
				if (VerticalLineRight == BorderLineState.Unknown) {
					e.Graphics.DrawLine(new Pen(Color.LightGray, 10), Width - 1, 0, Width - 1, Height);
					verticalLineRightWidth = 10;
				}
				if (VerticalLineLeft == BorderLineState.Known)
					verticalLineLeftWidth = helper.DrawVerticalLineLeft(BorderInfoSource, documentModel, toLayoutUnitConverter, exporter, Width, Height);
				if (VerticalLineLeft == BorderLineState.Unknown) {
					e.Graphics.DrawLine(new Pen(Color.LightGray, 10), 0, 0, 0, Height);
					verticalLineLeftWidth = 10;
				}
				if (VerticalLineIn == BorderLineState.Known) 
					verticalLineInWidth = helper.DrawVerticalLineIn(BorderInfoSource, documentModel, toLayoutUnitConverter, exporter, Width, Height);
				if (VerticalLineIn == BorderLineState.Unknown) {
					e.Graphics.DrawLine(new Pen(Color.LightGray, 10), Width / 2, 0, Width / 2, Height);
					verticalLineInWidth = 10;
				}
				if (HorizontalLineIn == BorderLineState.Known)
					horizontalLineInWidth = helper.DrawHorizontalLineIn(BorderInfoSource, documentModel, toLayoutUnitConverter, exporter, Width, Height);
				if (HorizontalLineIn == BorderLineState.Unknown) {
					e.Graphics.DrawLine(new Pen(Color.LightGray, 10), 0, Height / 2, Width, Height / 2);
					horizontalLineInWidth = 10;
				}
				if (HorizontalLineUp == BorderLineState.Known)
					horizontalLineUpWidth = helper.DrawHorizontalLineUp(BorderInfoSource, documentModel, toLayoutUnitConverter, exporter, Width, Height);
				if (HorizontalLineUp == BorderLineState.Unknown) {
					e.Graphics.DrawLine(new Pen(Color.LightGray, 10), 0, 0, Width, 0);
					horizontalLineUpWidth = 10;
				}
				if (HorizontalLineDown == BorderLineState.Known)
					horizontalLineDownWidth = helper.DrawHorizontalLineDown(BorderInfoSource, documentModel, toLayoutUnitConverter, exporter, Width, Height);
				if (HorizontalLineDown == BorderLineState.Unknown){
					e.Graphics.DrawLine(new Pen(Color.LightGray, 10), 0, Height - 1, Width, Height - 1);
					horizontalLineDownWidth = 10;
				}
				if (DrawColumns) {
					MyRectangle(e, (Width - verticalLineRightWidth) / 2 - verticalLineLeftWidth - (verticalLineInWidth / 2), (Height) / 2 - horizontalLineUpWidth - (horizontalLineInWidth / 2), verticalLineLeftWidth, horizontalLineUpWidth);
					MyRectangle(e, (Width - verticalLineRightWidth) / 2 - verticalLineLeftWidth - (verticalLineInWidth / 2), (Height) / 2 - horizontalLineDownWidth - (horizontalLineInWidth / 2), verticalLineLeftWidth, (Height) / 2 + (horizontalLineInWidth / 2));
					MyRectangle(e, (Width - verticalLineRightWidth) / 2 - (verticalLineInWidth / 2) - verticalLineRightWidth, (Height) / 2 - horizontalLineUpWidth - (horizontalLineInWidth / 2), (Width) / 2 + (verticalLineInWidth / 2), horizontalLineUpWidth);
					MyRectangle(e, (Width - verticalLineRightWidth) / 2 - (verticalLineInWidth / 2) - verticalLineRightWidth, (Height) / 2 - horizontalLineDownWidth - (horizontalLineInWidth / 2), (Width ) / 2 + (verticalLineInWidth / 2), (Height) / 2 + (horizontalLineInWidth / 2));
				}
				else {
					if (DrawParagraph) {
						MyRectangle(e, Width - verticalLineLeftWidth - verticalLineRightWidth, Height / 2 - horizontalLineUpWidth - (horizontalLineInWidth / 2), verticalLineLeftWidth, horizontalLineUpWidth);
						MyRectangle(e, Width - verticalLineLeftWidth - verticalLineRightWidth, Height / 2 - horizontalLineDownWidth - (horizontalLineInWidth / 2), verticalLineLeftWidth, Height / 2 + (horizontalLineInWidth / 2));
					}
					else
						MyRectangle(e, Width - verticalLineLeftWidth - verticalLineRightWidth, Height - horizontalLineUpWidth - horizontalLineDownWidth, verticalLineLeftWidth, horizontalLineUpWidth);
				}
			}
		}
		private void PreviewBSUserControl_Paint(object sender, PaintEventArgs e) {
		}	   
	}
	public enum BorderLineState { Known, Unknown, No }
}
