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
using System.Linq;
using System.Text;
using System.Collections;
using System.Drawing;
using DevExpress.XtraPrinting.Export.Pdf;
using System.IO;
using System.Drawing.Drawing2D;
namespace DevExpress.Printing.Core.PdfExport.Metafile {
	public class WmfFontObject {
		public short Height { get; set; }
		public short Width { get; set; }
		public float Angle { get; set; }
		public short Orientation { get; set; }
		public short Weight { get; set; }
		public bool IsItalic { get; set; }
		public bool Underline { get; set; }
		public bool StrikeOut { get; set; }
		public byte CharSet { get; set; }
		public byte OutPrecision { get; set; }
		public byte ClipPrecision { get; set; }
		public byte Quality { get; set; }
		public int PitchAndFamily { get; set; }
		public string Facename { get; set; }
		public int Italic { get { return IsItalic ? 2 : 0; } }
		public int Bold { get { return Width > 600 ? 1 : 0; } }
		static string[] fontNames = {
										"Courier", "Courier-Bold", "Courier-Oblique", "Courier-BoldOblique",
										"Helvetica", "Helvetica-Bold", "Helvetica-Oblique", "Helvetica-BoldOblique",
										"Times-Roman", "Times-Bold", "Times-Italic", "Times-BoldItalic",
										"Symbol", "ZapfDingbats"};
		internal const int MarkerBold = 1;
		internal const int MarkerItalic = 2;
		internal const int MarkerCourier = 0;
		internal const int MarkerHelvetica = 4;
		internal const int MarkerTimes = 8;
		internal const int MarkerSymbol = 12;
		internal const int FontDontCare = 0;
		internal const int FontRoman = 1;
		internal const int FontSwiss = 2;
		internal const int FontModern = 3;
		internal const int FontScript = 4;
		internal const int FontDecorative = 5;
		internal const int FixedPitch = 1;
		PdfFont font = null;
		public PdfFont GetFont(float fontSize, IPdfContentsOwner owner) {
			if(font != null)
				return font;
			string fontName;
			if(Facename.IndexOf("courier") != -1 || Facename.IndexOf("terminal") != -1
				|| Facename.IndexOf("fixedsys") != -1) {
				fontName = fontNames[MarkerCourier + Italic + Bold];
			} else if(Facename.IndexOf("ms sans serif") != -1 || Facename.IndexOf("arial") != -1
				  || Facename.IndexOf("system") != -1) {
				fontName = fontNames[MarkerHelvetica + Italic + Bold];
			} else if(Facename.IndexOf("arial black") != -1) {
				fontName = fontNames[MarkerHelvetica + Italic + MarkerBold];
			} else if(Facename.IndexOf("times") != -1 || Facename.IndexOf("ms serif") != -1
				  || Facename.IndexOf("roman") != -1) {
				fontName = fontNames[MarkerTimes + Italic + Bold];
			} else if(Facename.IndexOf("symbol") != -1) {
				fontName = fontNames[MarkerSymbol];
			} else {
				int pitch = PitchAndFamily & 3;
				int family = (PitchAndFamily >> 4) & 7;
				switch(family) {
					case FontModern:
						fontName = fontNames[MarkerCourier + Italic + Bold];
						break;
					case FontRoman:
						fontName = fontNames[MarkerTimes + Italic + Bold];
						break;
					case FontSwiss:
					case FontScript:
					case FontDecorative:
						fontName = fontNames[MarkerHelvetica + Italic + Bold];
						break;
					default: {
							switch(pitch) {
								case FixedPitch:
									fontName = fontNames[MarkerCourier + Italic + Bold];
									break;
								default:
									fontName = fontNames[MarkerHelvetica + Italic + Bold];
									break;
							}
							break;
						}
				}
			}
			bool isBold = Bold == 1;
			FontStyle fontStyle = FontStyle.Regular;
			if(isBold)
				fontStyle = fontStyle | FontStyle.Bold;
			if(IsItalic)
				fontStyle = fontStyle | FontStyle.Italic;
			if(StrikeOut)
				fontStyle = fontStyle | FontStyle.Strikeout;
			if(Underline)
				fontStyle = fontStyle | FontStyle.Underline;
			font = PdfFonts.CreatePdfFont(new System.Drawing.Font(fontName, fontSize, fontStyle), false);
			owner.Fonts.AddUnique(font);
			return font;
		}
		public void Read(MetaReader reader) {
			Height = reader.ReadInt16();
			Width = reader.ReadInt16();
			Angle = (float)(reader.ReadInt16() / 1800 * Math.PI);
			Orientation = reader.ReadInt16();
			Weight = reader.ReadInt16();
			IsItalic = reader.ReadByte() != 0;
			Underline = reader.ReadByte() != 0;
			StrikeOut = reader.ReadByte() != 0;
			CharSet = reader.ReadByte();
			reader.ReadByte();
			ClipPrecision = reader.ReadByte();
			Quality = reader.ReadByte();
			PitchAndFamily = reader.ReadByte();
			int nameSize = 32;
			byte[] name = new byte[nameSize];
			int k;
			for(k = 0; k < nameSize; ++k) {
				int c = reader.ReadByte();
				if(c == 0) {
					break;
				}
				name[k] = (byte)c;
			}
			try {
				Facename = System.Text.Encoding.GetEncoding(1252).GetString(name, 0, k);
			} catch {
				Facename = System.Text.ASCIIEncoding.ASCII.GetString(name, 0, k);
			}
			Facename = Facename.ToLower(System.Globalization.CultureInfo.InvariantCulture);
		}		
	}
	public enum TextAlignmentMode {
		TA_NOUPDATECP = 0x0000,
		TA_LEFT = 0x0000,
		TA_TOP = 0x0000,
		TA_UPDATECP = 0x0001,
		TA_RIGHT = 0x0002,
		TA_CENTER = 0x0006,
		TA_BOTTOM = 0x0008,
		TA_BASELINE = 0x0018,
		TA_RTLREADING = 0x0100
	}
}
