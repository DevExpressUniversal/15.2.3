#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       UWP Controls                                                }
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

using System.Collections.Generic;
namespace DevExpress.Pdf.Native {
	public class PdfFontPostTableDirectoryEntry : PdfFontTableDirectoryEntry {
		public const string EntryTag = "post";
		const int version = 0x00030000;
		const int fontIsProportionallySpaced = 0;
		const int fontIsMonospaced = 1;
		const short minMemType42 = 0;
		const short maxMemType42 = 0;
		const short minMemType1 = 0;
		const short maxMemType1 = 0;
		readonly static string[] standardMacCharacterSet = new string[] { 
			PdfGlyphNames._notdef, PdfGlyphNames.Null, PdfGlyphNames.nonmarkingreturn, PdfGlyphNames.space, PdfGlyphNames.exclam, PdfGlyphNames.quotedbl, PdfGlyphNames.numbersign, PdfGlyphNames.dollar,
			PdfGlyphNames.percent, PdfGlyphNames.ampersand, PdfGlyphNames.quotesingle, PdfGlyphNames.parenleft, PdfGlyphNames.parenright, PdfGlyphNames.asterisk, PdfGlyphNames.plus, 
			PdfGlyphNames.comma, PdfGlyphNames.hyphen, PdfGlyphNames.period, PdfGlyphNames.slash, PdfGlyphNames.zero, PdfGlyphNames.one, PdfGlyphNames.two, PdfGlyphNames.three, PdfGlyphNames.four, 
			PdfGlyphNames.five, PdfGlyphNames.six, PdfGlyphNames.seven, PdfGlyphNames.eight, PdfGlyphNames.nine, PdfGlyphNames.colon, PdfGlyphNames.semicolon, PdfGlyphNames.less, PdfGlyphNames.equal, 
			PdfGlyphNames.greater, PdfGlyphNames.question, PdfGlyphNames.at, PdfGlyphNames.A, PdfGlyphNames.B, PdfGlyphNames.C, PdfGlyphNames.D, PdfGlyphNames.E, PdfGlyphNames.F, PdfGlyphNames.G, 
			PdfGlyphNames.H, PdfGlyphNames.I, PdfGlyphNames.J, PdfGlyphNames.K, PdfGlyphNames.L, PdfGlyphNames.M, PdfGlyphNames.N, PdfGlyphNames.O, PdfGlyphNames.P, PdfGlyphNames.Q, PdfGlyphNames.R, 
			PdfGlyphNames.S, PdfGlyphNames.T, PdfGlyphNames.U, PdfGlyphNames.V, PdfGlyphNames.W, PdfGlyphNames.X, PdfGlyphNames.Y, PdfGlyphNames.Z, PdfGlyphNames.bracketleft, PdfGlyphNames.backslash,
			PdfGlyphNames.bracketright, PdfGlyphNames.asciicircum, PdfGlyphNames.underscore, PdfGlyphNames.grave, PdfGlyphNames.a, PdfGlyphNames.b, PdfGlyphNames.c, PdfGlyphNames.d, PdfGlyphNames.e, 
			PdfGlyphNames.f, PdfGlyphNames.g, PdfGlyphNames.h, PdfGlyphNames.i, PdfGlyphNames.j, PdfGlyphNames.k, PdfGlyphNames.l, PdfGlyphNames.m, PdfGlyphNames.n, PdfGlyphNames.o, PdfGlyphNames.p, 
			PdfGlyphNames.q, PdfGlyphNames.r, PdfGlyphNames.s, PdfGlyphNames.t, PdfGlyphNames.u, PdfGlyphNames.v, PdfGlyphNames.w, PdfGlyphNames.x, PdfGlyphNames.y, PdfGlyphNames.z, 
			PdfGlyphNames.braceleft, PdfGlyphNames.bar, PdfGlyphNames.braceright, PdfGlyphNames.asciitilde, PdfGlyphNames.Adieresis, PdfGlyphNames.Aring, PdfGlyphNames.Ccedilla, PdfGlyphNames.Eacute, 
			PdfGlyphNames.Ntilde, PdfGlyphNames.Odieresis, PdfGlyphNames.Udieresis, PdfGlyphNames.aacute, PdfGlyphNames.agrave, PdfGlyphNames.acircumflex, PdfGlyphNames.adieresis, 
			PdfGlyphNames.atilde, PdfGlyphNames.aring, PdfGlyphNames.ccedilla, PdfGlyphNames.eacute, PdfGlyphNames.egrave, PdfGlyphNames.ecircumflex, PdfGlyphNames.edieresis, PdfGlyphNames.iacute, 
			PdfGlyphNames.igrave, PdfGlyphNames.icircumflex, PdfGlyphNames.idieresis, PdfGlyphNames.ntilde, PdfGlyphNames.oacute, PdfGlyphNames.ograve, PdfGlyphNames.ocircumflex, 
			PdfGlyphNames.odieresis, PdfGlyphNames.otilde, PdfGlyphNames.uacute, PdfGlyphNames.ugrave, PdfGlyphNames.ucircumflex, PdfGlyphNames.udieresis, PdfGlyphNames.dagger, PdfGlyphNames.degree, 
			PdfGlyphNames.cent, PdfGlyphNames.sterling, PdfGlyphNames.section, PdfGlyphNames.bullet, PdfGlyphNames.paragraph, PdfGlyphNames.germandbls, PdfGlyphNames.registered, 
			PdfGlyphNames.copyright, PdfGlyphNames.trademark, PdfGlyphNames.acute, PdfGlyphNames.dieresis, PdfGlyphNames.notequal, PdfGlyphNames.AE, PdfGlyphNames.Oslash, PdfGlyphNames.infinity, 
			PdfGlyphNames.plusminus, PdfGlyphNames.lessequal, PdfGlyphNames.greaterequal, PdfGlyphNames.yen, PdfGlyphNames.mu, PdfGlyphNames.partialdiff, PdfGlyphNames.summation, 
			PdfGlyphNames.product, PdfGlyphNames.pi, PdfGlyphNames.integral, PdfGlyphNames.ordfeminine, PdfGlyphNames.ordmasculine, PdfGlyphNames.Omega, PdfGlyphNames.ae, PdfGlyphNames.oslash, 
			PdfGlyphNames.questiondown, PdfGlyphNames.exclamdown, PdfGlyphNames.logicalnot, PdfGlyphNames.radical, PdfGlyphNames.florin, PdfGlyphNames.approxequal, PdfGlyphNames.increment, 
			PdfGlyphNames.guillemotleft, PdfGlyphNames.guillemotright, PdfGlyphNames.ellipsis, PdfGlyphNames.nonbreakingspace, PdfGlyphNames.Agrave, PdfGlyphNames.Atilde, PdfGlyphNames.Otilde, 
			PdfGlyphNames.OE, PdfGlyphNames.oe, PdfGlyphNames.endash, PdfGlyphNames.emdash, PdfGlyphNames.quotedblleft, PdfGlyphNames.quotedblright, PdfGlyphNames.quoteleft, PdfGlyphNames.quoteright,
			PdfGlyphNames.divide, PdfGlyphNames.lozenge, PdfGlyphNames.ydieresis, PdfGlyphNames.Ydieresis, PdfGlyphNames.fraction, PdfGlyphNames.currency, PdfGlyphNames.guilsinglleft, 
			PdfGlyphNames.guilsinglright, PdfGlyphNames.fi, PdfGlyphNames.fl, PdfGlyphNames.daggerdbl, PdfGlyphNames.periodcentered, PdfGlyphNames.quotesinglbase, PdfGlyphNames.quotedblbase, 
			PdfGlyphNames.perthousand, PdfGlyphNames.Acircumflex, PdfGlyphNames.Ecircumflex, PdfGlyphNames.Aacute, PdfGlyphNames.Edieresis, PdfGlyphNames.Egrave, PdfGlyphNames.Iacute, 
			PdfGlyphNames.Icircumflex, PdfGlyphNames.Idieresis, PdfGlyphNames.Igrave, PdfGlyphNames.Oacute, PdfGlyphNames.Ocircumflex, PdfGlyphNames.apple, PdfGlyphNames.Ograve, 
			PdfGlyphNames.Uacute, PdfGlyphNames.Ucircumflex, PdfGlyphNames.Ugrave, PdfGlyphNames.dotlessi, PdfGlyphNames.circumflex, PdfGlyphNames.tilde, PdfGlyphNames.macron, PdfGlyphNames.breve,
			PdfGlyphNames.dotaccent, PdfGlyphNames.ring, PdfGlyphNames.cedilla, PdfGlyphNames.hungarumlaut, PdfGlyphNames.ogonek, PdfGlyphNames.caron, PdfGlyphNames.Lslash, PdfGlyphNames.lslash, 
			PdfGlyphNames.Scaron, PdfGlyphNames.scaron, PdfGlyphNames.Zcaron, PdfGlyphNames.zcaron, PdfGlyphNames.brokenbar, PdfGlyphNames.Eth, PdfGlyphNames.eth, PdfGlyphNames.Yacute, 
			PdfGlyphNames.yacute, PdfGlyphNames.Thorn, PdfGlyphNames.thorn, PdfGlyphNames.minus, PdfGlyphNames.multiply, PdfGlyphNames.onesuperior, PdfGlyphNames.twosuperior, 
			PdfGlyphNames.threesuperior, PdfGlyphNames.onehalf, PdfGlyphNames.onequarter, PdfGlyphNames.threequarters, PdfGlyphNames.franc, PdfGlyphNames.Gbreve, PdfGlyphNames.gbreve, 
			PdfGlyphNames.Idotaccent, PdfGlyphNames.Scedilla, PdfGlyphNames.scedilla, PdfGlyphNames.Cacute, PdfGlyphNames.cacute, PdfGlyphNames.Ccaron, PdfGlyphNames.ccaron, PdfGlyphNames.dcroat
		};
		readonly float italicAngle;
		readonly short underlinePosition;
		readonly IList<string> glyphNames;
		public float ItalicAngle { get { return italicAngle; } }
		public short UnderlinePosition { get { return underlinePosition; } }
		public IList<string> GlyphNames { get { return glyphNames; } }
		public PdfFontPostTableDirectoryEntry(byte[] tableData) : base(EntryTag, tableData) {
			const int headerSize = 32;
			PdfBinaryStream tableStream = TableStream;
			int version = tableStream.ReadInt();
			italicAngle = tableStream.ReadFixed();
			if (version == 0x20000 && tableStream.Length > headerSize) {
				tableStream.Position = headerSize;
				int glyphCount = tableStream.ReadUshort();
				glyphNames = new List<string>(glyphCount);
				long glyphNamePosition = tableStream.Position + glyphCount * 2;
				for (int i = 0; i < glyphCount; i++) {
					ushort value = (ushort)tableStream.ReadShort();
					if (value < 258)
						glyphNames.Add(standardMacCharacterSet[value]);
					else {
						long position = tableStream.Position;
						tableStream.Position = glyphNamePosition;
						int length = tableStream.ReadByte();
						glyphNames.Add(tableStream.ReadString(length));
						glyphNamePosition = tableStream.Position;
						tableStream.Position = position;
					}
				}
			}
		}
		public PdfFontPostTableDirectoryEntry(IFont font) : base(EntryTag) {
			glyphNames = new List<string>();
			int proportion;
			PdfFontDescriptor fontDesctiptor = font.FontDescriptor;
			if (fontDesctiptor == null) {
				italicAngle = 0;
				underlinePosition = 0;
				proportion = fontIsProportionallySpaced;
			}
			else {
				italicAngle = (float)fontDesctiptor.ItalicAngle;
				PdfRectangle fontBBox = fontDesctiptor.FontBBox;
				underlinePosition = fontBBox == null ? (short)0 : (short)(fontBBox.Top / 2);
				proportion = ((fontDesctiptor.Flags & PdfFontFlags.FixedPitch) == PdfFontFlags.FixedPitch) ? fontIsMonospaced : fontIsProportionallySpaced;
			}
			PdfBinaryStream stream = TableStream;
			stream.WriteInt(version);
			stream.WriteFixed(italicAngle);
			stream.WriteShort(underlinePosition);
			stream.WriteShort((short)(underlinePosition / 5));
			stream.WriteInt(proportion);
			stream.WriteInt(minMemType42);
			stream.WriteInt(maxMemType42);
			stream.WriteInt(minMemType1);
			stream.WriteInt(maxMemType1);
		}
	}
}
