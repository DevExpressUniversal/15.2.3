#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Document Server                                             }
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
using DevExpress.Utils;
using DevExpress.XtraPrinting;
using DevExpress.XtraPrinting.Native;
#if !SL
using System.Windows.Forms;
using DevExpress.Office.Utils;
using System.Drawing.Drawing2D;
using DevExpress.XtraPrinting.Export.Pdf;
#else
using System.Windows.Controls;
#endif
namespace DevExpress.BarCodes.Internal {
	public class BarCodePrinter : IPrintable {
		readonly BarCode barCode;
		PrintingSystemBase printingSystem;
		public BarCodePrinter(BarCode barCode) {
			Guard.ArgumentNotNull(barCode, "barCode");
			this.barCode = barCode;
		}
		#region IPrintable
		void IPrintable.AcceptChanges() {
		}
		bool IPrintable.CreatesIntersectedBricks { get { return true; } }
		bool IPrintable.HasPropertyEditor() { return false; }
		UserControl IPrintable.PropertyEditorControl { get { return null; } }
		void IPrintable.RejectChanges() {
		}
		void IPrintable.ShowHelp() {
		}
		bool IPrintable.SupportsHelp() {
			return false;
		}
		#endregion
		PrintingSystemBase PrintingSystem { get { return printingSystem; } }
		void IBasePrintable.Initialize(IPrintingSystem ps, ILink link) {
			this.printingSystem = ps as PrintingSystemBase;
		}
		void IBasePrintable.Finalize(IPrintingSystem ps, ILink link) {
			this.printingSystem = null;
		}
		void IBasePrintable.CreateArea(string areaName, IBrickGraphics graph) {
			if (printingSystem != null && areaName == SR.Detail)
				CreateDetail(graph);
		}
		void CreateDetail(IBrickGraphics graph) {
			BarCodeLayoutInfo layoutInfo = barCode.CalculateLayout();
			TransformationBrick2 panel = new TransformationBrick2();
			RectangleF bounds = new RectangleF(PointF.Empty, new SizeF(layoutInfo.BorderBounds.Right, layoutInfo.BorderBounds.Bottom));
			bounds = GraphicsUnitConverter.Convert(bounds, GraphicsDpi.UnitToDpi(barCode.Unit), 96.0f);
			VisualBrickHelper.InitializeBrick(panel, PrintingSystem, bounds);
			PointF offset = new PointF(barCode.Margins.Left, barCode.Margins.Top);
			offset = GraphicsUnitConverter.Convert(offset, barCode.Unit, GraphicsUnit.Document);
			panel.Offset = offset;
			offset = RectangleUtils.CenterPoint(layoutInfo.BorderBounds);
			offset = GraphicsUnitConverter.Convert(offset, barCode.Unit, GraphicsUnit.Document);
			panel.RotationCenter = offset;
			panel.RotationAngle = layoutInfo.ActualRotationAngle;
			graph.DrawBrick(panel, bounds);
			panel.Bricks.Add(DrawBackground(graph, layoutInfo.BorderBounds));
			Brick brick = DrawCaption(graph, barCode.TopCaption, layoutInfo.TopCaptionBounds);
			if (brick != null)
				panel.Bricks.Add(brick);
			brick = DrawCaption(graph, barCode.BottomCaption, layoutInfo.BottomCaptionBounds);
			if (brick != null)
				panel.Bricks.Add(brick);
			panel.Bricks.Add(DrawBarCode(graph, layoutInfo.BarCodeBounds));
		}
		Brick DrawCaption(IBrickGraphics graphics, BarCodeCaption caption, RectangleF bounds) {
			if (bounds.Height <= 0)
				return null;
			BarCodePadding padding = caption.Paddings;
			bounds.X += padding.Left;
			bounds.Y += padding.Top;
			bounds.Width -= padding.Left + padding.Right;
			bounds.Height -= padding.Bottom + padding.Top;
			LabelBrick brick = new LabelBrick();
			bounds = GraphicsUnitConverter.Convert(bounds, barCode.Unit, GraphicsUnit.Document);
			VisualBrickHelper.InitializeBrick(brick, PrintingSystem, bounds);
			brick.Text = caption.Text;
			brick.ForeColor = caption.ForeColor;
			brick.Font = caption.Font;
			brick.HorzAlignment = ConvertHorizontalAlignment(caption.HorizontalAlignment);
			brick.StringFormat = new BrickStringFormat(caption.StringFormat);
			brick.BorderWidth = 0;
			brick.BackColor = DXColor.Transparent;
			return brick;
		}
		HorzAlignment ConvertHorizontalAlignment(StringAlignment stringAlignment) {
			switch (stringAlignment) {
				default:
				case StringAlignment.Near:
					return HorzAlignment.Near;
				case StringAlignment.Far:
					return HorzAlignment.Far;
				case StringAlignment.Center:
					return HorzAlignment.Center;
			}
		}
		Brick DrawBackground(IBrickGraphics graphics, RectangleF bounds) {
			bounds = GraphicsUnitConverter.Convert(bounds, barCode.Unit, GraphicsUnit.Document);
			PanelBrick brick = new PanelBrick();
			VisualBrickHelper.InitializeBrick(brick, PrintingSystem, bounds);
			brick.BorderWidth = GraphicsUnitConverter.Convert(barCode.BorderWidth, GraphicsDpi.UnitToDpi(barCode.Unit), 96.0f); 
			brick.BorderColor = barCode.BorderColor;
			brick.BorderStyle = BrickBorderStyle.Inset;
			brick.BackColor = barCode.BackColor;
			return brick;
		}
		Brick DrawBarCode(IBrickGraphics graphics, RectangleF bounds) {
			bounds = GraphicsUnitConverter.Convert(bounds, barCode.Unit, GraphicsUnit.Document);
			BarCodeBrick brick = new BarCodeBrick();
			VisualBrickHelper.InitializeBrick(brick, PrintingSystem, bounds );
			BarCodeViewInfo viewInfo = new BarCodeViewInfo(barCode);
			brick.Generator = barCode.Generator;
			brick.Text = viewInfo.Text;
			brick.ShowText = viewInfo.ShowText;
			brick.Style = viewInfo.Style;
			brick.Alignment = viewInfo.Alignment;
			brick.AutoModule = true; 
			brick.Module = viewInfo.Module;
			brick.Orientation = viewInfo.Orientation;
			brick.BinaryData = viewInfo.BinaryData;
			brick.BackColor = DXColor.Transparent;
			brick.ForeColor = barCode.ForeColor;
			return brick;
		}
	}
	[DevExpress.XtraPrinting.BrickExporters.BrickExporter(typeof(TransformationBrick2Exporter))]
	public class TransformationBrick2 : PanelBrick {
		public TransformationBrick2() {
			Init();
		}
		public PointF Offset { get; set; }
		public float RotationAngle { get; set; }
		public PointF RotationCenter { get; set; }
		void Init() {
			BackColor = DXColor.Transparent;
			BorderWidth = 0f;
			NoClip = true;
			SeparableHorz = false;
			SeparableVert = false;
		}
	}
	public class TransformationBrick2Exporter : DevExpress.XtraPrinting.BrickExporters.PanelBrickExporter {
		public override void Draw(IGraphics gr, RectangleF rect, RectangleF parentRect) {
			TransformationBrick2 brick = this.Brick as TransformationBrick2;
			if (brick == null)
				base.Draw(gr, rect, parentRect);
			SmoothingMode smoothingMode = gr.SmoothingMode;
			gr.SaveTransformState();
			try {
				gr.ResetTransform();
				gr.ApplyTransformState(MatrixOrder.Append, false);
				PointF offset = brick.Offset;
				if (offset != PointF.Empty)
					gr.TranslateTransform(offset.X, offset.Y);
				gr.TranslateTransform(rect.X, rect.Y);
				if ((brick.RotationAngle % 90) != 0)
					gr.SmoothingMode = SmoothingMode.AntiAlias;
				if (brick.RotationAngle != 0) {
					PointF rotationCenter = brick.RotationCenter;
					gr.TranslateTransform(rotationCenter.X, rotationCenter.Y);
					gr.RotateTransform(brick.RotationAngle);
					gr.TranslateTransform(-rotationCenter.X, -rotationCenter.Y);
				}
				base.Draw(gr, new RectangleF(PointF.Empty, rect.Size), new RectangleF(PointF.Empty, parentRect.Size));
			}
			finally {
				gr.SmoothingMode = smoothingMode;
				gr.ResetTransform();
				gr.ApplyTransformState(MatrixOrder.Append, true);
			}
		}
	}
}
