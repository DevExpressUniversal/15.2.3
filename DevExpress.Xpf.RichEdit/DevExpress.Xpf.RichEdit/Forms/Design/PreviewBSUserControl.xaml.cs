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
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using DevExpress.Office;
using DevExpress.Office.Drawing;
using DevExpress.Office.Internal;
using DevExpress.Office.Layout;
using DevExpress.Utils;
using DevExpress.XtraRichEdit.Drawing;
using DevExpress.XtraRichEdit.Forms;
using DevExpress.XtraRichEdit.Layout;
using DevExpress.XtraRichEdit.Layout.Export;
using DevExpress.XtraRichEdit.Layout.TableLayout;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Printing;
namespace DevExpress.Xpf.RichEdit.UI {
	public partial class PreviewBSUserControl : UserControl {
		const float deviceIndependentDPI = 96.0f;
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
		int updateCount;
		#endregion
		#region Property
		public DocumentModel DocumentModel { get { return documentModel; } set { documentModel = value; } }
		public BorderShadingUserControl BorderInfoSource { get; set; }
		public Color FillColor {
			get { return fillColor; }
			set {
				fillColor = value;
				RedrawCanvas();
			}
		}	   
		public BorderLineState VerticalLineRight {
			get { return verticalLineRight; }
			set {
				verticalLineRight = value;
				RedrawCanvas();
			}
		}
		public BorderLineState VerticalLineLeft {
			get { return verticalLineLeft; }
			set {
				verticalLineLeft = value;
				RedrawCanvas();
			}
		}
		public BorderLineState VerticalLineIn {
			get { return verticalLineIn; }
			set {
				verticalLineIn = value;
				RedrawCanvas();
			}
		}
		public BorderLineState HorizontalLineIn {
			get { return horizontalLineIn; }
			set {
				horizontalLineIn = value;
				RedrawCanvas();
			}
		}
		public BorderLineState HorizontalLineUp {
			get { return horizontalLineUp; }
			set {
				horizontalLineUp = value;
				RedrawCanvas();
			}
		}
		public BorderLineState HorizontalLineDown {
			get { return horizontalLineDown; }
			set {
				horizontalLineDown = value;
				RedrawCanvas();
			}
		}
		public bool DrawPageBorderHorizontalDown {
			get { return drawPageBorderHorizontalDown; }
			set {
				drawPageBorderHorizontalDown = value;
				RedrawCanvas();
			}
		}
		public bool DrawPageBorderHorizontalUp {
			get { return drawPageBorderHorizontalUp; }
			set {
				drawPageBorderHorizontalUp = value;
				RedrawCanvas();
			}
		}
		public bool DrawPageBorderVerticalRight {
			get { return drawPageBorderVerticalRight; }
			set {
				drawPageBorderVerticalRight = value;
				RedrawCanvas();
			}
		}
		public bool DrawPageBorderVerticalLeft {
			get { return drawPageBorderVerticalLeft; }
			set {
				drawPageBorderVerticalLeft = value;
				RedrawCanvas();
			}
		}
		public bool DrawColumns {
			get { return drawColumns; }
			set {
				drawColumns = value;
				RedrawCanvas();
			}
		}
		public bool DrawParagraph {
			get { return drawParagraph; }
			set {
				drawParagraph = value;
				RedrawCanvas();
			}
		}
		bool InsideSetProperties { get { return updateCount > 0; } }
		#endregion
		public PreviewBSUserControl() {
			InitializeComponent();
			fillPreview.SizeChanged += fillPreview_SizeChanged;
		}
		void fillPreview_SizeChanged(object sender, SizeChangedEventArgs e) {
			RedrawCanvas();
		}
		public void BeginSetProperties() {
			updateCount++;
		}
		public void EndSetProperties() {
			updateCount--;
		}		
		void MyRectangle(int width, int height, int x, int y) {
			int heightRectangle = 3;
			int[] pageBorder = { 1, 1, 1, 1 };
			int countRectangle = (height - pageBorder[0] - pageBorder[1]) / (2 * heightRectangle);
			for (int i = 0; i < countRectangle; i++){
				Rectangle rectangle = new Rectangle();
				rectangle.Width = width - pageBorder[3] - pageBorder[2];
				rectangle.Height = heightRectangle;
				Canvas.SetLeft(rectangle, x + pageBorder[2]);
				Canvas.SetTop(rectangle, y + i * 2 * heightRectangle + pageBorder[0]);
				rectangle.Fill = new SolidColorBrush(Colors.LightGray);
				canvasRect.Children.Add(rectangle);
			}
		}  
		void DrawRectangle (int x, int y, int width, int height) {
			Rectangle rectangle = new Rectangle();
			rectangle.Width = width;
			rectangle.Height = height;
			Canvas.SetLeft(rectangle, x);
			Canvas.SetTop(rectangle, y);
			rectangle.Fill = new SolidColorBrush(Colors.LightGray);
			canvasPreview.Children.Add(rectangle);
		}
		void RedrawCanvas() {
			if (InsideSetProperties || documentModel == null)
				return;
			canvasPreview.Children.Clear();
			canvasRect.Children.Clear();
			if (fillPreview.ActualWidth == 0 || fillPreview.ActualHeight == 0)
				return;
			DocumentLayoutUnitConverter converter = new DocumentLayoutUnitPixelsConverter(deviceIndependentDPI);
			XpfDrawingSurface surface = new XpfDrawingSurface(canvasPreview.Children);
			using (XpfPainterOverwrite painter = new XpfPainterOverwrite(converter, surface)) {
				painter.ZoomFactor = 1;
				RichEditPatternLinePainter horizontalLinePainter = new RichEditHorizontalPatternLinePainter(painter, converter);
				RichEditPatternLinePainter verticalLinePainter = new RichEditVerticalPatternLinePainter(painter, converter);
				GraphicsDocumentLayoutExporterTableBorder exporter = new GraphicsDocumentLayoutExporterTableBorder(documentModel, painter, horizontalLinePainter, verticalLinePainter);
				DocumentModelUnitToLayoutUnitConverter toLayoutUnitConverter = documentModel.UnitConverter.CreateConverterToLayoutUnits(DocumentLayoutUnit.Pixel, deviceIndependentDPI);
				BorderShadingFormHelper helper = new BorderShadingFormHelper();
				int verticalLineRightWidth = 0;
				int verticalLineLeftWidth = 0;
				int verticalLineInWidth = 0;
				int horizontalLineInWidth = 0;
				int horizontalLineUpWidth = 0;
				int horizontalLineDownWidth = 0;
				if (fillColor != XpfTypeConverter.ToPlatformColor(DXColor.Empty))
					fillPreview.Fill = new SolidColorBrush(fillColor);
				int width = (int)fillPreview.ActualWidth;
				int height = (int)fillPreview.ActualHeight;
				if (VerticalLineRight == BorderLineState.Known)
					verticalLineRightWidth = helper.DrawVerticalLineRight(BorderInfoSource, documentModel, toLayoutUnitConverter, exporter, width, height);
				if (VerticalLineRight == BorderLineState.Unknown) {
					verticalLineRightWidth = 10;
					DrawRectangle(width - verticalLineRightWidth, 0, verticalLineRightWidth, height);
				}
				if (VerticalLineLeft == BorderLineState.Known)
					verticalLineLeftWidth = helper.DrawVerticalLineLeft(BorderInfoSource, documentModel, toLayoutUnitConverter, exporter, width, height);
				if (VerticalLineLeft == BorderLineState.Unknown) {
					verticalLineLeftWidth = 10;
					DrawRectangle(0, 0, verticalLineLeftWidth, height);
				}
				if (VerticalLineIn == BorderLineState.Known)
					verticalLineInWidth = helper.DrawVerticalLineIn(BorderInfoSource, documentModel, toLayoutUnitConverter, exporter, width, height);
				if (VerticalLineIn == BorderLineState.Unknown) {
					verticalLineInWidth = 10;
					DrawRectangle(width / 2 - verticalLineInWidth / 2, 0, verticalLineInWidth, height);
				}
				if (HorizontalLineIn == BorderLineState.Known)
					horizontalLineInWidth = helper.DrawHorizontalLineIn(BorderInfoSource, documentModel, toLayoutUnitConverter, exporter, width, height);
				if (HorizontalLineIn == BorderLineState.Unknown) {
					horizontalLineInWidth = 10;
					DrawRectangle(0, height / 2 - horizontalLineInWidth / 2, width, horizontalLineInWidth);
				}
				if (HorizontalLineUp == BorderLineState.Known)
					horizontalLineUpWidth = helper.DrawHorizontalLineUp(BorderInfoSource, documentModel, toLayoutUnitConverter, exporter, width, height);
				if (HorizontalLineUp == BorderLineState.Unknown) {
					horizontalLineUpWidth = 10;
					DrawRectangle(0, 0, width, horizontalLineUpWidth);
				}
				if (HorizontalLineDown == BorderLineState.Known)
					horizontalLineDownWidth = helper.DrawHorizontalLineDown(BorderInfoSource, documentModel, toLayoutUnitConverter, exporter, width, height);
				if (HorizontalLineDown == BorderLineState.Unknown) {
					horizontalLineDownWidth = 10;
					DrawRectangle(0, height - horizontalLineDownWidth, width, horizontalLineDownWidth);
				}
				if (DrawColumns) {
					MyRectangle(width / 2 - verticalLineLeftWidth - (verticalLineInWidth / 2), height / 2 - horizontalLineUpWidth - (horizontalLineInWidth / 2), verticalLineLeftWidth, horizontalLineUpWidth);
					MyRectangle(width / 2 - verticalLineLeftWidth - (verticalLineInWidth / 2), height / 2 - horizontalLineDownWidth - (horizontalLineInWidth / 2), verticalLineLeftWidth, height / 2 + (horizontalLineInWidth / 2));
					MyRectangle(width / 2 - (verticalLineInWidth / 2) - verticalLineRightWidth, height / 2 - horizontalLineUpWidth - (horizontalLineInWidth / 2), width / 2 + (verticalLineInWidth / 2), horizontalLineUpWidth);
					MyRectangle(width / 2 - (verticalLineInWidth / 2) - verticalLineRightWidth, height / 2 - horizontalLineDownWidth - (horizontalLineInWidth / 2), width / 2 + (verticalLineInWidth / 2), height / 2 + (horizontalLineInWidth / 2));
				}
				else {
					if (DrawParagraph) {
						MyRectangle(width - verticalLineLeftWidth - verticalLineRightWidth, height / 2 - horizontalLineUpWidth - (horizontalLineInWidth / 2), verticalLineLeftWidth, horizontalLineUpWidth);
						MyRectangle(width - verticalLineLeftWidth - verticalLineRightWidth, height / 2 - horizontalLineDownWidth - (horizontalLineInWidth / 2), verticalLineLeftWidth, height / 2 + (horizontalLineInWidth / 2));
					}
					else
						MyRectangle(width - verticalLineLeftWidth - verticalLineRightWidth, height - horizontalLineUpWidth - horizontalLineDownWidth, verticalLineLeftWidth, horizontalLineUpWidth);
				}
			}
		}	   
	}
	public enum BorderLineState { Known, Unknown, No }
}
