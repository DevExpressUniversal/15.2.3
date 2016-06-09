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
using System.IO;
using System.Reflection;
using System.Text;
using DevExpress.Office;
using DevExpress.Office.Utils;
using DevExpress.Utils;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.XtraSpreadsheet.Utils;
using DevExpress.XtraSpreadsheet.Internal;
using DevExpress.Data.Export;
using DevExpress.Export.Xl;
using DevExpress.XtraExport.Xls;
#if !SL
using System.Drawing;
#else
using System.Windows.Media;
#endif
namespace DevExpress.XtraSpreadsheet.Import.Xls {
	#region XlsCommandFormat (XF)
	public class XlsCommandFormat : XlsCommandContentBase {
		XlsContentXF content = new XlsContentXF();
		#region Properties
		public XlsContentXF Content { get { return content; } }
		#endregion
		protected override void ReadCore(XlsReader reader, XlsContentBuilder contentBuilder) {
			content.CrcValue = contentBuilder.XFCRC;
			base.ReadCore(reader, contentBuilder);
			contentBuilder.XFCount = contentBuilder.XFCount + 1;
			contentBuilder.XFCRC = content.CrcValue;
		}
		public override void Execute(XlsContentBuilder contentBuilder) {
			XlsImportStyleSheet styleSheet = contentBuilder.StyleSheet;
			XlsExtendedFormatInfo info = new XlsExtendedFormatInfo();
			info.FontId = content.FontId;
			info.NumberFormatId = content.NumberFormatId;
			info.StyleXFIndex = content.StyleId;
			info.IsStyleFormat = content.IsStyleFormat;
			info.QuotePrefix = content.QuotePrefix;
			info.IsLocked = content.IsLocked;
			info.IsHidden = content.IsHidden;
			info.HorizontalAlignment = content.HorizontalAlignment;
			info.VerticalAlignment = content.VerticalAlignment;
			info.WrapText = content.WrapText;
			info.TextRotation = content.TextRotation;
			info.Indent = content.Indent;
			info.ShrinkToFit = content.ShrinkToFit;
			info.ReadingOrder = content.ReadingOrder;
			info.ApplyNumberFormat = content.ApplyNumberFormat;
			info.ApplyFont = content.ApplyFont;
			info.ApplyAlignment = content.ApplyAlignment;
			info.ApplyBorder = content.ApplyBorder;
			info.ApplyFill = content.ApplyFill;
			info.ApplyProtection = content.ApplyProtection;
			int colorIndex = styleSheet.GetBorderColorIndex(content.BorderLeftColorIndex);
			info.LeftBorderColor.ColorIndex = colorIndex;
			info.LeftBorderLineStyle = colorIndex == 0 ? XlBorderLineStyle.None : content.BorderLeftLineStyle;
			colorIndex = styleSheet.GetBorderColorIndex(content.BorderRightColorIndex);
			info.RightBorderColor.ColorIndex = colorIndex;
			info.RightBorderLineStyle = colorIndex == 0 ? XlBorderLineStyle.None : content.BorderRightLineStyle;
			colorIndex = styleSheet.GetBorderColorIndex(content.BorderTopColorIndex);
			info.TopBorderColor.ColorIndex = colorIndex;
			info.TopBorderLineStyle = colorIndex == 0 ? XlBorderLineStyle.None : content.BorderTopLineStyle;
			colorIndex = styleSheet.GetBorderColorIndex(content.BorderBottomColorIndex);
			info.BottomBorderColor.ColorIndex = colorIndex;
			info.BottomBorderLineStyle = colorIndex == 0 ? XlBorderLineStyle.None : content.BorderBottomLineStyle;
			colorIndex = styleSheet.GetBorderColorIndex(content.BorderDiagonalColorIndex);
			info.DiagonalBorderColor.ColorIndex = colorIndex;
			info.DiagonalBorderLineStyle = colorIndex == 0 ? XlBorderLineStyle.None : content.BorderDiagonalLineStyle;
			info.DiagonalDownBorder = content.BorderDiagonalDown;
			info.DiagonalUpBorder = content.BorderDiagonalUp;
			info.HasExtension = content.HasExtension;
			info.FillPatternType = content.FillPatternType;
			info.ForegroundColor.ColorIndex = styleSheet.GetPaletteColorIndex(content.FillForeColorIndex, true);
			info.BackgroundColor.ColorIndex = styleSheet.GetPaletteColorIndex(content.FillBackColorIndex, false);
			info.PivotButton = content.PivotButton;
			styleSheet.ExtendedFormats.Add(info);
		}
		public override IXlsCommand GetInstance() {
			return new XlsCommandFormat();
		}
		protected override IXlsContent GetContent() {
			return content;
		}
	}
	#endregion
	#region XlsCommandFormatCrc (XFCRC)
	public class XlsCommandFormatCrc : XlsCommandContentBase {
		XlsContentXFCrc content = new XlsContentXFCrc();
		#region Properties
		public int XFCount {
			get { return content.XFCount; }
			set { content.XFCount = value; }
		}
		public int XFCRC {
			get { return content.XFCRC; }
			set { content.XFCRC = value; }
		}
		#endregion
		public override void Execute(XlsContentBuilder contentBuilder) {
			contentBuilder.UseXFExt = (XFCount == contentBuilder.XFCount) && (XFCRC == contentBuilder.XFCRC);
		}
		protected override void WriteCore(BinaryWriter writer) {
			content.RecordHeader.RecordTypeId = XlsCommandFactory.GetTypeCodeByType(typeof(XlsCommandFormatCrc)); 
			base.WriteCore(writer);
		}
		public override IXlsCommand GetInstance() {
			return this;
		}
		protected override IXlsContent GetContent() {
			return content;
		}
	}
	#endregion
	#region XlsCommandFormatExt (XFExt)
	public class XlsCommandFormatExt : XlsCommandBase {
		#region Fields
		const int fixedPartSize = 16;
		int xfIndex;
		readonly XFExtProperties properties = new XFExtProperties();
		#endregion
		#region Properties
		public int XFIndex {
			get { return xfIndex; }
			set {
				ValueChecker.CheckValue(value, 0, ushort.MaxValue, "XFIndex");
				xfIndex = value;
			}
		}
		public XFExtProperties Properties { get { return properties; } }
		#endregion
		protected override void ReadCore(XlsReader reader, XlsContentBuilder contentBuilder) {
			FutureRecordHeader.FromStream(reader);
			reader.ReadUInt16(); 
			XFIndex = reader.ReadUInt16();
			Properties.Read(reader);
		}
		public override void Execute(XlsContentBuilder contentBuilder) {
			XlsImportStyleSheet styleSheet = contentBuilder.StyleSheet;
			if ((contentBuilder.UseXFExt ) && Properties.Count > 0) {
				XlsExtendedFormatInfo info = styleSheet.GetExtendedFormatInfo(XFIndex);
				XlsExtendedFormatInfoAdapter adapter = new XlsExtendedFormatInfoAdapter(info, styleSheet);
				Properties.ApplyContent(adapter);
				info.IsRegistered = false;
			}
		}
		protected override void WriteCore(BinaryWriter writer) {
			FutureRecordHeader header = new FutureRecordHeader();
			header.RecordTypeId = XlsCommandFactory.GetTypeCodeByType(typeof(XlsCommandFormatExt));
			header.Write(writer);
			writer.Write((ushort)0); 
			writer.Write((ushort)XFIndex);
			Properties.Write(writer);
		}
		protected override short GetSize() {
			return (short)(fixedPartSize + this.properties.GetSize());
		}
		public override IXlsCommand GetInstance() {
			return new XlsCommandFormatExt();
		}
	}
	#endregion
}
