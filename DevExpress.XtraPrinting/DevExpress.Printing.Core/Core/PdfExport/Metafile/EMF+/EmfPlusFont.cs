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

using DevExpress.XtraPrinting.Export.Pdf;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
namespace DevExpress.Printing.Core.PdfExport.Metafile {
	[CLSCompliant(false)]
	public class EmfPlusFont {
		IPdfContentsOwner owner;
		PdfFont pdfFont;
		public float EmSize { get; set; }
		public UnitType SizeUnit { get; set; }
		public FontStyle FontStyleFlags { get; set; }
		public UInt32 Length { get; set; }
		public string FamilyName { get; set; }
		public PdfFont PdfFont {
			get {
				if(pdfFont != null)
					return pdfFont;
				pdfFont = PdfFonts.CreatePdfFont(new System.Drawing.Font(FamilyName, EmSize, FontStyleFlags), false);
				owner.Fonts.AddUnique(pdfFont);
				return pdfFont;
			}
		}
		public EmfPlusFont(MetaReader reader, IPdfContentsOwner owner) {
			new EmfPlusGraphicsVersion(reader);
			EmSize = reader.ReadSingle();
			SizeUnit = (UnitType)reader.ReadUInt32();
			FontStyleFlags = (FontStyle)reader.ReadInt32();
			reader.ReadUInt32();
			Length = reader.ReadUInt32();
			FamilyName = MetaImage.GetUnicodeStringData(reader, (int)Length);
			this.owner = owner;
		}
	}
	public enum UnitType {
		UnitTypeWorld = 0x00, 
		UnitTypeDisplay = 0x01, 
		UnitTypePixel = 0x02,
		UnitTypePoint = 0x03,
		UnitTypeInch = 0x04,
		UnitTypeDocument = 0x05,
		UnitTypeMillimeter = 0x06
	}
}
