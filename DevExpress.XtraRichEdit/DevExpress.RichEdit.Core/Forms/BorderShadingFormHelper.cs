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
using System.Drawing;
using System.Linq;
using System.Text;
using DevExpress.Office;
using DevExpress.XtraRichEdit.Layout;
using DevExpress.XtraRichEdit.Layout.Export;
using DevExpress.XtraRichEdit.Layout.TableLayout;
using DevExpress.XtraRichEdit.Model;
using DevExpress.Compatibility.System.Drawing;
namespace DevExpress.XtraRichEdit.Forms {
	public class BorderShadingFormHelper {
		public virtual int DrawVerticalLineRight(IBorderShadingUserControl borderInfoSource, DocumentModel documentModel, DocumentModelUnitToLayoutUnitConverter toLayoutUnitConverter, GraphicsDocumentLayoutExporterTableBorder exporter, int width, int height) {
			PreviewBorderViewInfo viewInfo = new PreviewBorderViewInfo();
			viewInfo.Border = borderInfoSource.BorderLineRight;
			viewInfo.BorderType = BorderTypes.Right;
			int borderWidth = GetActualBorderWidth(viewInfo.Border, documentModel);
			CornerViewInfoBase topCorner = CornerViewInfoBase.CreateCorner(CornerViewInfoType.InnerTopRight, toLayoutUnitConverter, borderInfoSource.BorderLineUp, null, null, borderInfoSource.BorderLineRight, 0);
			CornerViewInfoBase bottomCorner = CornerViewInfoBase.CreateCorner(CornerViewInfoType.InnerBottomRight, toLayoutUnitConverter, borderInfoSource.BorderLineDown, borderInfoSource.BorderLineRight, null, null, 0);
			CornerViewInfoBase middleCorner = CornerViewInfoBase.CreateCorner(CornerViewInfoType.InnerRightMiddle, toLayoutUnitConverter, borderInfoSource.BorderLineHorizontalIn, borderInfoSource.BorderLineRight, null, borderInfoSource.BorderLineRight, 0);
			viewInfo.StartCorner = topCorner;
			viewInfo.EndCorner = middleCorner;
			exporter.ExportTableBorderCorner(topCorner, width - borderWidth, 0);
			exporter.ExportTableBorderCorner(middleCorner, width - borderWidth, (height - borderWidth) / 2);
			exporter.ExportTableBorder(viewInfo.Border, new Rectangle(0, 0, width - borderWidth, (height - borderWidth) / 2), toLayoutUnitConverter, viewInfo);
			PreviewBorderViewInfo viewInfoBottom = new PreviewBorderViewInfo();
			viewInfoBottom.Border = borderInfoSource.BorderLineRight;
			viewInfoBottom.BorderType = BorderTypes.Right;
			viewInfoBottom.StartCorner = middleCorner;
			viewInfoBottom.EndCorner = bottomCorner;
			exporter.ExportTableBorderCorner(middleCorner, width - borderWidth, (height - borderWidth) / 2);
			exporter.ExportTableBorderCorner(bottomCorner, width - borderWidth, height - borderWidth);
			exporter.ExportTableBorder(viewInfoBottom.Border, new Rectangle(0, (height - borderWidth) / 2, width - borderWidth, (height - borderWidth) / 2), toLayoutUnitConverter, viewInfoBottom);
			return borderWidth;
		}
		public virtual int DrawVerticalLineLeft(IBorderShadingUserControl borderInfoSource, DocumentModel documentModel, DocumentModelUnitToLayoutUnitConverter toLayoutUnitConverter, GraphicsDocumentLayoutExporterTableBorder exporter, int width, int height) {
			PreviewBorderViewInfo viewInfo = new PreviewBorderViewInfo();
			viewInfo.Border = borderInfoSource.BorderLineLeft;
			viewInfo.BorderType = BorderTypes.Left;
			int borderWidth = GetActualBorderWidth(viewInfo.Border, documentModel);
			CornerViewInfoBase topCorner = CornerViewInfoBase.CreateCorner(CornerViewInfoType.InnerTopLeft, toLayoutUnitConverter, null, null, borderInfoSource.BorderLineUp, borderInfoSource.BorderLineLeft, 0);
			CornerViewInfoBase bottomCorner = CornerViewInfoBase.CreateCorner(CornerViewInfoType.InnerBottomLeft, toLayoutUnitConverter, null, borderInfoSource.BorderLineLeft, borderInfoSource.BorderLineDown, null, 0);
			CornerViewInfoBase middleCorner = CornerViewInfoBase.CreateCorner(CornerViewInfoType.InnerLeftMiddle, toLayoutUnitConverter, null, borderInfoSource.BorderLineLeft, borderInfoSource.BorderLineHorizontalIn, borderInfoSource.BorderLineLeft, 0);
			viewInfo.StartCorner = topCorner;
			viewInfo.EndCorner = middleCorner;
			exporter.ExportTableBorderCorner(topCorner, 0, 0);
			exporter.ExportTableBorderCorner(middleCorner, 0, (height - borderWidth) / 2);
			exporter.ExportTableBorder(viewInfo.Border, new Rectangle(0, 0, (width - borderWidth) / 2, (height - borderWidth) / 2), toLayoutUnitConverter, viewInfo);
			PreviewBorderViewInfo viewInfoBottom = new PreviewBorderViewInfo();
			viewInfoBottom.Border = borderInfoSource.BorderLineLeft;
			viewInfoBottom.BorderType = BorderTypes.Left;
			viewInfoBottom.StartCorner = middleCorner;
			viewInfoBottom.EndCorner = bottomCorner;
			exporter.ExportTableBorderCorner(middleCorner, 0, (height - borderWidth) / 2);
			exporter.ExportTableBorderCorner(bottomCorner, 0, height - borderWidth);
			exporter.ExportTableBorder(viewInfoBottom.Border, new Rectangle(0, (height - borderWidth) / 2, (width - borderWidth) / 2, (height - borderWidth) / 2), toLayoutUnitConverter, viewInfoBottom);
			return borderWidth;
		}
		public virtual int DrawVerticalLineIn(IBorderShadingUserControl borderInfoSource, DocumentModel documentModel, DocumentModelUnitToLayoutUnitConverter toLayoutUnitConverter, GraphicsDocumentLayoutExporterTableBorder exporter, int width, int height) {
			PreviewBorderViewInfo viewInfoTop = new PreviewBorderViewInfo();
			viewInfoTop.Border = borderInfoSource.BorderLineVerticalIn;
			viewInfoTop.BorderType = BorderTypes.Right;
			int borderWidth = GetActualBorderWidth(viewInfoTop.Border, documentModel);
			CornerViewInfoBase topCorner = CornerViewInfoBase.CreateCorner(CornerViewInfoType.InnerTopMiddle, toLayoutUnitConverter, borderInfoSource.BorderLineUp, null, borderInfoSource.BorderLineUp, borderInfoSource.BorderLineVerticalIn, 0);
			CornerViewInfoBase middleCorner = CornerViewInfoBase.CreateCorner(CornerViewInfoType.InnerNormal, toLayoutUnitConverter, borderInfoSource.BorderLineHorizontalIn, borderInfoSource.BorderLineVerticalIn, borderInfoSource.BorderLineHorizontalIn, borderInfoSource.BorderLineVerticalIn, 0);
			viewInfoTop.StartCorner = topCorner;
			viewInfoTop.EndCorner = middleCorner;
			exporter.ExportTableBorderCorner(topCorner, (width - borderWidth) / 2, 0);
			exporter.ExportTableBorderCorner(middleCorner, (width - borderWidth) / 2, (height - borderWidth) / 2);
			exporter.ExportTableBorder(viewInfoTop.Border, new Rectangle(0, 0, (width - borderWidth) / 2, (height - borderWidth) / 2), toLayoutUnitConverter, viewInfoTop);
			PreviewBorderViewInfo viewInfoBottom = new PreviewBorderViewInfo();
			viewInfoBottom.Border = borderInfoSource.BorderLineVerticalIn;
			viewInfoBottom.BorderType = BorderTypes.Right;
			CornerViewInfoBase bottomCorner = CornerViewInfoBase.CreateCorner(CornerViewInfoType.InnerBotomMiddle, toLayoutUnitConverter, borderInfoSource.BorderLineDown, borderInfoSource.BorderLineVerticalIn, borderInfoSource.BorderLineDown, null, 0);
			viewInfoBottom.StartCorner = middleCorner;
			viewInfoBottom.EndCorner = bottomCorner;
			exporter.ExportTableBorderCorner(middleCorner, (width - borderWidth) / 2, (height - borderWidth) / 2);
			exporter.ExportTableBorderCorner(bottomCorner, (width - borderWidth) / 2, height - borderWidth);
			exporter.ExportTableBorder(viewInfoBottom.Border, new Rectangle(0, (height - borderWidth) / 2, (width - borderWidth) / 2, (height - borderWidth) / 2), toLayoutUnitConverter, viewInfoBottom);
			return borderWidth;
		}
		public virtual int DrawHorizontalLineIn(IBorderShadingUserControl borderInfoSource, DocumentModel documentModel, DocumentModelUnitToLayoutUnitConverter toLayoutUnitConverter, GraphicsDocumentLayoutExporterTableBorder exporter, int width, int height) {
			PreviewBorderViewInfo viewInfoLeft = new PreviewBorderViewInfo();
			viewInfoLeft.Border = borderInfoSource.BorderLineHorizontalIn;
			viewInfoLeft.BorderType = BorderTypes.Bottom;
			int borderWidth = GetActualBorderWidth(viewInfoLeft.Border, documentModel);
			CornerViewInfoBase leftCorner = CornerViewInfoBase.CreateCorner(CornerViewInfoType.InnerLeftMiddle, toLayoutUnitConverter, null, borderInfoSource.BorderLineLeft, borderInfoSource.BorderLineHorizontalIn, borderInfoSource.BorderLineLeft, 0);
			CornerViewInfoBase rightCorner = CornerViewInfoBase.CreateCorner(CornerViewInfoType.InnerRightMiddle, toLayoutUnitConverter, borderInfoSource.BorderLineHorizontalIn, borderInfoSource.BorderLineRight, null, borderInfoSource.BorderLineRight, 0);
			CornerViewInfoBase middleCorner = CornerViewInfoBase.CreateCorner(CornerViewInfoType.InnerNormal, toLayoutUnitConverter, borderInfoSource.BorderLineHorizontalIn, borderInfoSource.BorderLineVerticalIn, borderInfoSource.BorderLineHorizontalIn, borderInfoSource.BorderLineVerticalIn, 0);
			viewInfoLeft.StartCorner = leftCorner;
			viewInfoLeft.EndCorner = middleCorner;
			exporter.ExportTableBorderCorner(leftCorner, 0, (height - borderWidth) / 2);
			exporter.ExportTableBorderCorner(middleCorner, (width - borderWidth) / 2, (height - borderWidth) / 2);
			exporter.ExportTableBorder(viewInfoLeft.Border, new Rectangle(0, 0, (width - borderWidth) / 2, (height - borderWidth) / 2), toLayoutUnitConverter, viewInfoLeft);
			PreviewBorderViewInfo viewInfoRight = new PreviewBorderViewInfo();
			viewInfoRight.Border = borderInfoSource.BorderLineHorizontalIn;
			viewInfoRight.BorderType = BorderTypes.Bottom;
			viewInfoRight.StartCorner = middleCorner;
			viewInfoRight.EndCorner = rightCorner;
			exporter.ExportTableBorderCorner(middleCorner, (width - borderWidth) / 2, (height - borderWidth) / 2);
			exporter.ExportTableBorderCorner(rightCorner, width - borderWidth, (height - borderWidth) / 2);
			exporter.ExportTableBorder(viewInfoRight.Border, new Rectangle((width - borderWidth) / 2, 0, (width - borderWidth) / 2, (height - borderWidth) / 2), toLayoutUnitConverter, viewInfoRight);
			return borderWidth;
		}
		public virtual int DrawHorizontalLineUp(IBorderShadingUserControl borderInfoSource, DocumentModel documentModel, DocumentModelUnitToLayoutUnitConverter toLayoutUnitConverter, GraphicsDocumentLayoutExporterTableBorder exporter, int width, int height) {
			PreviewBorderViewInfo viewInfo = new PreviewBorderViewInfo();
			viewInfo.Border = borderInfoSource.BorderLineUp;
			viewInfo.BorderType = BorderTypes.Top;
			int borderWidth = GetActualBorderWidth(viewInfo.Border, documentModel);
			CornerViewInfoBase rightCorner = CornerViewInfoBase.CreateCorner(CornerViewInfoType.InnerTopRight, toLayoutUnitConverter, borderInfoSource.BorderLineUp, null, null, borderInfoSource.BorderLineRight, 0);
			CornerViewInfoBase leftCorner = CornerViewInfoBase.CreateCorner(CornerViewInfoType.InnerTopLeft, toLayoutUnitConverter, null, null, borderInfoSource.BorderLineUp, borderInfoSource.BorderLineLeft, 0);
			CornerViewInfoBase middleCorner = CornerViewInfoBase.CreateCorner(CornerViewInfoType.InnerTopMiddle, toLayoutUnitConverter, borderInfoSource.BorderLineUp, null, borderInfoSource.BorderLineUp, borderInfoSource.BorderLineVerticalIn, 0);
			viewInfo.StartCorner = leftCorner;
			viewInfo.EndCorner = middleCorner;
			exporter.ExportTableBorderCorner(leftCorner, 0, 0);
			exporter.ExportTableBorderCorner(middleCorner, (width - borderWidth) / 2, 0);
			exporter.ExportTableBorder(viewInfo.Border, new Rectangle(0, 0, (width - borderWidth) / 2, height - borderWidth), toLayoutUnitConverter, viewInfo);
			PreviewBorderViewInfo viewInfoRight = new PreviewBorderViewInfo();
			viewInfoRight.Border = borderInfoSource.BorderLineUp;
			viewInfoRight.BorderType = BorderTypes.Top;
			viewInfoRight.StartCorner = middleCorner;
			viewInfoRight.EndCorner = rightCorner;
			exporter.ExportTableBorderCorner(middleCorner, (width - borderWidth) / 2, 0);
			exporter.ExportTableBorderCorner(rightCorner, width - borderWidth, 0);
			exporter.ExportTableBorder(viewInfoRight.Border, new Rectangle((width - borderWidth) / 2, 0, (width - borderWidth) / 2, height - borderWidth), toLayoutUnitConverter, viewInfoRight);
			return borderWidth;
		}
		public virtual int DrawHorizontalLineDown(IBorderShadingUserControl borderInfoSource, DocumentModel documentModel, DocumentModelUnitToLayoutUnitConverter toLayoutUnitConverter, GraphicsDocumentLayoutExporterTableBorder exporter, int width, int height) {
			PreviewBorderViewInfo viewInfo = new PreviewBorderViewInfo();
			viewInfo.Border = borderInfoSource.BorderLineDown;
			viewInfo.BorderType = BorderTypes.Bottom;
			int borderWidth = GetActualBorderWidth(viewInfo.Border, documentModel);
			CornerViewInfoBase rightCorner = CornerViewInfoBase.CreateCorner(CornerViewInfoType.InnerBottomRight, toLayoutUnitConverter, borderInfoSource.BorderLineDown, borderInfoSource.BorderLineRight, null, null, 0);
			CornerViewInfoBase leftCorner = CornerViewInfoBase.CreateCorner(CornerViewInfoType.InnerBottomLeft, toLayoutUnitConverter, null, borderInfoSource.BorderLineLeft, borderInfoSource.BorderLineDown, null, 0);
			CornerViewInfoBase middleCorner = CornerViewInfoBase.CreateCorner(CornerViewInfoType.InnerBotomMiddle, toLayoutUnitConverter, borderInfoSource.BorderLineDown, borderInfoSource.BorderLineVerticalIn, borderInfoSource.BorderLineDown, null, 0);
			viewInfo.StartCorner = leftCorner;
			viewInfo.EndCorner = middleCorner;
			exporter.ExportTableBorderCorner(leftCorner, 0, height - borderWidth);
			exporter.ExportTableBorderCorner(middleCorner, (width - borderWidth) / 2, height - borderWidth);
			exporter.ExportTableBorder(viewInfo.Border, new Rectangle(0, 0, (width - borderWidth) / 2, height - borderWidth), toLayoutUnitConverter, viewInfo);
			PreviewBorderViewInfo viewInfoRight = new PreviewBorderViewInfo();
			viewInfoRight.Border = borderInfoSource.BorderLineDown;
			viewInfoRight.BorderType = BorderTypes.Bottom;
			viewInfoRight.StartCorner = middleCorner;
			viewInfoRight.EndCorner = rightCorner;
			exporter.ExportTableBorderCorner(middleCorner, (width - borderWidth) / 2, height - borderWidth);
			exporter.ExportTableBorderCorner(rightCorner, width - borderWidth, height - borderWidth);
			exporter.ExportTableBorder(viewInfoRight.Border, new Rectangle((width - borderWidth) / 2, 0, (width - borderWidth) / 2, height - borderWidth), toLayoutUnitConverter, viewInfoRight);
			return borderWidth;
		}
		int GetActualBorderWidth(BorderInfo border, DocumentModel documentModel) {
			TableBorderCalculator borderCalculator = new TableBorderCalculator();
			int scaleFactor = 1;
			int thickness = Math.Max(1, documentModel.UnitConverter.ModelUnitsToPixels(border.Width * scaleFactor));
			return borderCalculator.GetActualWidth(border.Style, thickness);
		}
	}
	#region IBorderShadingUserControl
	public interface IBorderShadingUserControl {
		BorderInfo BorderLineUp { get;}
		BorderInfo BorderLineDown { get; }
		BorderInfo BorderLineHorizontalIn { get; }
		BorderInfo BorderLineLeft { get; }
		BorderInfo BorderLineRight { get; }
		BorderInfo BorderLineVerticalIn { get; }
	}
	#endregion
	#region PreviewBorderViewInfo
	public class PreviewBorderViewInfo : ITableBorderViewInfoBase {
		public BorderInfo Border { get; set; }
		public BorderTypes BorderType { get; set; }
		public CornerViewInfoBase EndCorner { get; set; }
		public CornerViewInfoBase StartCorner { get; set; }
		public bool HasStartCorner { get { return StartCorner != null && !(StartCorner is NoneLineCornerViewInfo); } }
		public bool HasEndCorner { get { return EndCorner != null && !(EndCorner is NoneLineCornerViewInfo); } }
	}
	#endregion
}
