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

using System;
using System.Collections.Generic;
using DevExpress.Pdf.Native;
namespace DevExpress.Pdf {
	public enum PdfBaseEncoding { Standard, MacRoman, WinAnsi };
	public class PdfSimpleFontEncoding : PdfEncoding {
		internal const string baseEncodingDictionaryKey = "BaseEncoding";
		const string differencesDictionaryKey = "Differences";
		const string macRomanEncodingName = "MacRomanEncoding";
		const string winAnsiEncodingName = "WinAnsiEncoding";
		internal static readonly Dictionary<byte, string> MacRomanEncoding = new Dictionary<byte, string> {
			{ 65, PdfGlyphNames.A }, { 174, PdfGlyphNames.AE }, { 231, PdfGlyphNames.Aacute }, { 229, PdfGlyphNames.Acircumflex }, { 128, PdfGlyphNames.Adieresis }, 
			{ 203, PdfGlyphNames.Agrave }, { 129, PdfGlyphNames.Aring }, { 204, PdfGlyphNames.Atilde }, { 66, PdfGlyphNames.B }, { 67, PdfGlyphNames.C }, { 130, PdfGlyphNames.Ccedilla }, 
			{ 68, PdfGlyphNames.D }, { 69, PdfGlyphNames.E }, { 131, PdfGlyphNames.Eacute }, { 230, PdfGlyphNames.Ecircumflex }, { 232, PdfGlyphNames.Edieresis }, 
			{ 233, PdfGlyphNames.Egrave }, { 70, PdfGlyphNames.F }, { 71, PdfGlyphNames.G }, { 72, PdfGlyphNames.H }, { 73, PdfGlyphNames.I }, { 234, PdfGlyphNames.Iacute }, 
			{ 235, PdfGlyphNames.Icircumflex }, { 236, PdfGlyphNames.Idieresis }, { 237, PdfGlyphNames.Igrave }, { 74, PdfGlyphNames.J }, { 75, PdfGlyphNames.K }, { 76, PdfGlyphNames.L }, 
			{ 77, PdfGlyphNames.M }, { 78, PdfGlyphNames.N }, { 132, PdfGlyphNames.Ntilde }, { 79, PdfGlyphNames.O }, { 206, PdfGlyphNames.OE }, { 238, PdfGlyphNames.Oacute }, 
			{ 239, PdfGlyphNames.Ocircumflex }, { 133, PdfGlyphNames.Odieresis }, { 241, PdfGlyphNames.Ograve }, { 175, PdfGlyphNames.Oslash }, { 205, PdfGlyphNames.Otilde }, 
			{ 80, PdfGlyphNames.P }, { 81, PdfGlyphNames.Q }, { 82, PdfGlyphNames.R }, { 83, PdfGlyphNames.S }, { 84, PdfGlyphNames.T }, { 85, PdfGlyphNames.U }, 
			{ 242, PdfGlyphNames.Uacute }, { 243, PdfGlyphNames.Ucircumflex }, { 134, PdfGlyphNames.Udieresis }, { 244, PdfGlyphNames.Ugrave }, { 86, PdfGlyphNames.V }, 
			{ 87, PdfGlyphNames.W }, { 88, PdfGlyphNames.X }, { 89, PdfGlyphNames.Y }, { 217, PdfGlyphNames.Ydieresis }, { 90, PdfGlyphNames.Z }, { 97, PdfGlyphNames.a }, 
			{ 135, PdfGlyphNames.aacute }, { 137, PdfGlyphNames.acircumflex }, { 171, PdfGlyphNames.acute }, { 138, PdfGlyphNames.adieresis }, { 190, PdfGlyphNames.ae }, 
			{ 136, PdfGlyphNames.agrave }, { 38, PdfGlyphNames.ampersand }, { 140, PdfGlyphNames.aring }, { 94, PdfGlyphNames.asciicircum }, { 126, PdfGlyphNames.asciitilde }, 
			{ 42, PdfGlyphNames.asterisk }, { 64, PdfGlyphNames.at }, { 139, PdfGlyphNames.atilde }, { 98, PdfGlyphNames.b }, { 92, PdfGlyphNames.backslash }, { 124, PdfGlyphNames.bar }, 
			{ 123, PdfGlyphNames.braceleft }, { 125, PdfGlyphNames.braceright }, { 91, PdfGlyphNames.bracketleft }, { 93, PdfGlyphNames.bracketright }, { 249, PdfGlyphNames.breve }, 
			{ 165, PdfGlyphNames.bullet }, { 99, PdfGlyphNames.c }, { 255, PdfGlyphNames.caron }, { 141, PdfGlyphNames.ccedilla }, { 252, PdfGlyphNames.cedilla }, { 162, PdfGlyphNames.cent }, 
			{ 246, PdfGlyphNames.circumflex }, { 58, PdfGlyphNames.colon }, { 44, PdfGlyphNames.comma }, { 169, PdfGlyphNames.copyright }, { 219, PdfGlyphNames.currency }, 
			{ 100, PdfGlyphNames.d }, { 160, PdfGlyphNames.dagger }, { 224, PdfGlyphNames.daggerdbl }, { 161, PdfGlyphNames.degree }, { 172, PdfGlyphNames.dieresis }, { 214, PdfGlyphNames.divide }, 
			{ 36, PdfGlyphNames.dollar }, { 250, PdfGlyphNames.dotaccent }, { 245, PdfGlyphNames.dotlessi }, { 101, PdfGlyphNames.e }, { 142, PdfGlyphNames.eacute }, 
			{ 144, PdfGlyphNames.ecircumflex }, { 145, PdfGlyphNames.edieresis }, { 143, PdfGlyphNames.egrave }, { 56, PdfGlyphNames.eight }, { 201, PdfGlyphNames.ellipsis }, 
			{ 209, PdfGlyphNames.emdash }, { 208, PdfGlyphNames.endash }, { 61, PdfGlyphNames.equal }, { 33, PdfGlyphNames.exclam }, { 193, PdfGlyphNames.exclamdown }, { 102, PdfGlyphNames.f },
			{ 222, PdfGlyphNames.fi }, { 53, PdfGlyphNames.five }, { 223, PdfGlyphNames.fl }, { 196, PdfGlyphNames.florin }, { 52, PdfGlyphNames.four }, { 218, PdfGlyphNames.fraction }, 
			{ 103, PdfGlyphNames.g }, { 167, PdfGlyphNames.germandbls }, { 96, PdfGlyphNames.grave }, { 62, PdfGlyphNames.greater }, { 199, PdfGlyphNames.guillemotleft }, 
			{ 200, PdfGlyphNames.guillemotright }, { 220, PdfGlyphNames.guilsinglleft }, { 221, PdfGlyphNames.guilsinglright }, { 104, PdfGlyphNames.h }, { 253, PdfGlyphNames.hungarumlaut }, 
			{ 45, PdfGlyphNames.hyphen }, { 105, PdfGlyphNames.i }, { 146, PdfGlyphNames.iacute }, { 148, PdfGlyphNames.icircumflex }, { 149, PdfGlyphNames.idieresis }, 
			{ 147, PdfGlyphNames.igrave }, { 106, PdfGlyphNames.j }, { 107, PdfGlyphNames.k }, { 108, PdfGlyphNames.l }, { 60, PdfGlyphNames.less }, { 194, PdfGlyphNames.logicalnot }, 
			{ 109, PdfGlyphNames.m }, { 248, PdfGlyphNames.macron }, { 181, PdfGlyphNames.mu }, { 110, PdfGlyphNames.n }, { 57, PdfGlyphNames.nine }, { 150, PdfGlyphNames.ntilde }, 
			{ 35, PdfGlyphNames.numbersign }, { 111, PdfGlyphNames.o }, { 151, PdfGlyphNames.oacute }, { 153, PdfGlyphNames.ocircumflex }, { 154, PdfGlyphNames.odieresis },
			{ 207, PdfGlyphNames.oe }, { 254, PdfGlyphNames.ogonek }, { 152, PdfGlyphNames.ograve }, { 49, PdfGlyphNames.one }, { 187, PdfGlyphNames.ordfeminine }, 
			{ 188, PdfGlyphNames.ordmasculine }, { 191, PdfGlyphNames.oslash }, { 155, PdfGlyphNames.otilde }, { 112, PdfGlyphNames.p }, { 166, PdfGlyphNames.paragraph }, 
			{ 40, PdfGlyphNames.parenleft }, { 41, PdfGlyphNames.parenright }, { 37, PdfGlyphNames.percent }, { 46, PdfGlyphNames.period }, { 225, PdfGlyphNames.periodcentered },
			{ 228, PdfGlyphNames.perthousand }, { 43, PdfGlyphNames.plus }, { 177, PdfGlyphNames.plusminus }, { 113, PdfGlyphNames.q }, { 63, PdfGlyphNames.question }, 
			{ 192, PdfGlyphNames.questiondown }, { 34, PdfGlyphNames.quotedbl }, { 227, PdfGlyphNames.quotedblbase }, { 210, PdfGlyphNames.quotedblleft }, { 211, PdfGlyphNames.quotedblright }, 
			{ 212, PdfGlyphNames.quoteleft }, { 213, PdfGlyphNames.quoteright }, { 226, PdfGlyphNames.quotesinglbase }, { 39, PdfGlyphNames.quotesingle }, { 114, PdfGlyphNames.r }, 
			{ 168, PdfGlyphNames.registered }, { 251, PdfGlyphNames.ring }, { 115, PdfGlyphNames.s }, { 164, PdfGlyphNames.section }, { 59, PdfGlyphNames.semicolon }, { 55, PdfGlyphNames.seven }, 
			{ 54, PdfGlyphNames.six }, { 47, PdfGlyphNames.slash }, { 32, PdfGlyphNames.space }, { 163, PdfGlyphNames.sterling }, { 116, PdfGlyphNames.t }, { 51, PdfGlyphNames.three }, 
			{ 247, PdfGlyphNames.tilde }, { 170, PdfGlyphNames.trademark }, { 50, PdfGlyphNames.two }, { 117, PdfGlyphNames.u }, { 156, PdfGlyphNames.uacute }, { 158, PdfGlyphNames.ucircumflex },
			{ 159, PdfGlyphNames.udieresis }, { 157, PdfGlyphNames.ugrave }, { 95, PdfGlyphNames.underscore }, { 118, PdfGlyphNames.v }, { 119, PdfGlyphNames.w }, { 120, PdfGlyphNames.x }, 
			{ 121, PdfGlyphNames.y }, { 216, PdfGlyphNames.ydieresis }, { 180, PdfGlyphNames.yen }, { 122, PdfGlyphNames.z }, { 48, PdfGlyphNames.zero } 
		};
		static readonly Dictionary<byte, string> standardEncoding = new Dictionary<byte, string> {
			{ 65, PdfGlyphNames.A }, { 225, PdfGlyphNames.AE }, { 66, PdfGlyphNames.B }, { 67, PdfGlyphNames.C }, { 68, PdfGlyphNames.D }, { 69, PdfGlyphNames.E }, 
			{ 70, PdfGlyphNames.F }, { 71, PdfGlyphNames.G }, { 72, PdfGlyphNames.H }, { 73, PdfGlyphNames.I }, { 74, PdfGlyphNames.J }, { 75, PdfGlyphNames.K }, { 76, PdfGlyphNames.L }, 
			{ 232, PdfGlyphNames.Lslash }, { 77, PdfGlyphNames.M }, { 78, PdfGlyphNames.N }, { 79, PdfGlyphNames.O }, { 234, PdfGlyphNames.OE }, { 233, PdfGlyphNames.Oslash }, 
			{ 80, PdfGlyphNames.P }, { 81, PdfGlyphNames.Q }, { 82, PdfGlyphNames.R }, { 83, PdfGlyphNames.S }, { 84, PdfGlyphNames.T }, { 85, PdfGlyphNames.U }, { 86, PdfGlyphNames.V }, 
			{ 87, PdfGlyphNames.W }, { 88, PdfGlyphNames.X }, { 89, PdfGlyphNames.Y }, { 90, PdfGlyphNames.Z }, { 97, PdfGlyphNames.a }, { 194, PdfGlyphNames.acute }, 
			{ 241, PdfGlyphNames.ae }, { 38, PdfGlyphNames.ampersand }, { 94, PdfGlyphNames.asciicircum }, { 126, PdfGlyphNames.asciitilde }, { 42, PdfGlyphNames.asterisk }, 
			{ 64, PdfGlyphNames.at }, { 98, PdfGlyphNames.b }, { 92, PdfGlyphNames.backslash }, { 124, PdfGlyphNames.bar }, { 123, PdfGlyphNames.braceleft }, 
			{ 125, PdfGlyphNames.braceright }, { 91, PdfGlyphNames.bracketleft }, { 93, PdfGlyphNames.bracketright }, { 198, PdfGlyphNames.breve }, { 183, PdfGlyphNames.bullet }, 
			{ 99, PdfGlyphNames.c }, { 207, PdfGlyphNames.caron }, { 203, PdfGlyphNames.cedilla }, { 162, PdfGlyphNames.cent }, { 195, PdfGlyphNames.circumflex }, 
			{ 58, PdfGlyphNames.colon }, { 44, PdfGlyphNames.comma }, { 168, PdfGlyphNames.currency }, { 100, PdfGlyphNames.d }, { 178, PdfGlyphNames.dagger }, 
			{ 179, PdfGlyphNames.daggerdbl }, { 200, PdfGlyphNames.dieresis }, { 36, PdfGlyphNames.dollar }, { 199, PdfGlyphNames.dotaccent }, { 245, PdfGlyphNames.dotlessi }, 
			{ 101, PdfGlyphNames.e }, { 56, PdfGlyphNames.eight }, { 188, PdfGlyphNames.ellipsis }, { 208, PdfGlyphNames.emdash }, { 177, PdfGlyphNames.endash }, { 61, PdfGlyphNames.equal }, 
			{ 33, PdfGlyphNames.exclam }, { 161, PdfGlyphNames.exclamdown }, { 102, PdfGlyphNames.f }, { 174, PdfGlyphNames.fi }, { 53, PdfGlyphNames.five }, { 175, PdfGlyphNames.fl }, 
			{ 166, PdfGlyphNames.florin }, { 52, PdfGlyphNames.four }, { 164, PdfGlyphNames.fraction }, { 103, PdfGlyphNames.g }, { 251, PdfGlyphNames.germandbls }, 
			{ 193, PdfGlyphNames.grave }, { 62, PdfGlyphNames.greater }, { 171, PdfGlyphNames.guillemotleft }, { 187, PdfGlyphNames.guillemotright }, { 172, PdfGlyphNames.guilsinglleft }, 
			{ 173, PdfGlyphNames.guilsinglright }, { 104, PdfGlyphNames.h }, { 205, PdfGlyphNames.hungarumlaut }, { 45, PdfGlyphNames.hyphen }, { 105, PdfGlyphNames.i }, 
			{ 106, PdfGlyphNames.j }, { 107, PdfGlyphNames.k }, { 108, PdfGlyphNames.l }, { 60, PdfGlyphNames.less }, { 248, PdfGlyphNames.lslash }, { 109, PdfGlyphNames.m }, 
			{ 197, PdfGlyphNames.macron }, { 110, PdfGlyphNames.n }, { 57, PdfGlyphNames.nine }, { 35, PdfGlyphNames.numbersign }, { 111, PdfGlyphNames.o }, { 250, PdfGlyphNames.oe },
			{ 206, PdfGlyphNames.ogonek }, { 49, PdfGlyphNames.one }, { 227, PdfGlyphNames.ordfeminine }, { 235, PdfGlyphNames.ordmasculine }, { 249, PdfGlyphNames.oslash }, 
			{ 112, PdfGlyphNames.p }, { 182, PdfGlyphNames.paragraph }, { 40, PdfGlyphNames.parenleft }, { 41, PdfGlyphNames.parenright }, { 37, PdfGlyphNames.percent }, 
			{ 46, PdfGlyphNames.period }, { 180, PdfGlyphNames.periodcentered }, { 189, PdfGlyphNames.perthousand }, { 43, PdfGlyphNames.plus }, { 113, PdfGlyphNames.q }, 
			{ 63, PdfGlyphNames.question }, { 191, PdfGlyphNames.questiondown }, { 34, PdfGlyphNames.quotedbl }, { 185, PdfGlyphNames.quotedblbase }, { 170, PdfGlyphNames.quotedblleft }, 
			{ 186, PdfGlyphNames.quotedblright }, { 96, PdfGlyphNames.quoteleft }, { 39, PdfGlyphNames.quoteright }, { 184, PdfGlyphNames.quotesinglbase }, 
			{ 169, PdfGlyphNames.quotesingle }, { 114, PdfGlyphNames.r }, { 202, PdfGlyphNames.ring }, { 115, PdfGlyphNames.s }, { 167, PdfGlyphNames.section }, 
			{ 59, PdfGlyphNames.semicolon }, { 55, PdfGlyphNames.seven }, { 54, PdfGlyphNames.six }, { 47, PdfGlyphNames.slash }, { 32, PdfGlyphNames.space }, 
			{ 163, PdfGlyphNames.sterling }, { 116, PdfGlyphNames.t }, { 51, PdfGlyphNames.three }, { 196, PdfGlyphNames.tilde }, { 50, PdfGlyphNames.two }, { 117, PdfGlyphNames.u }, 
			{ 95, PdfGlyphNames.underscore }, { 118, PdfGlyphNames.v }, { 119, PdfGlyphNames.w }, { 120, PdfGlyphNames.x }, { 121, PdfGlyphNames.y }, { 165, PdfGlyphNames.yen }, 
			{ 122, PdfGlyphNames.z }, { 48, PdfGlyphNames.zero }
		}; 
		static readonly Dictionary<byte, string> winAnsiEncoding = new Dictionary<byte, string> {
			{ 65, PdfGlyphNames.A }, { 198, PdfGlyphNames.AE }, { 193, PdfGlyphNames.Aacute }, { 194, PdfGlyphNames.Acircumflex }, { 196, PdfGlyphNames.Adieresis }, { 192, PdfGlyphNames.Agrave }, 
			{ 197, PdfGlyphNames.Aring }, { 195, PdfGlyphNames.Atilde }, { 66, PdfGlyphNames.B }, { 67, PdfGlyphNames.C }, { 199, PdfGlyphNames.Ccedilla }, { 68, PdfGlyphNames.D }, { 69, PdfGlyphNames.E },
			{ 201, PdfGlyphNames.Eacute }, { 202, PdfGlyphNames.Ecircumflex }, { 203, PdfGlyphNames.Edieresis }, { 200, PdfGlyphNames.Egrave }, { 208, PdfGlyphNames.Eth }, { 128, PdfGlyphNames.Euro }, 
			{ 70, PdfGlyphNames.F }, { 71, PdfGlyphNames.G }, { 72, PdfGlyphNames.H }, { 73, PdfGlyphNames.I }, { 205, PdfGlyphNames.Iacute }, { 206, PdfGlyphNames.Icircumflex }, 
			{ 207, PdfGlyphNames.Idieresis }, { 204, PdfGlyphNames.Igrave }, { 74, PdfGlyphNames.J }, { 75, PdfGlyphNames.K }, { 76, PdfGlyphNames.L }, { 77, PdfGlyphNames.M }, { 78, PdfGlyphNames.N }, 
			{ 209, PdfGlyphNames.Ntilde }, { 79, PdfGlyphNames.O }, { 140, PdfGlyphNames.OE }, { 211, PdfGlyphNames.Oacute }, { 212, PdfGlyphNames.Ocircumflex }, { 214, PdfGlyphNames.Odieresis }, 
			{ 210, PdfGlyphNames.Ograve }, { 216, PdfGlyphNames.Oslash }, { 213, PdfGlyphNames.Otilde }, { 80, PdfGlyphNames.P }, { 81, PdfGlyphNames.Q }, { 82, PdfGlyphNames.R }, { 83, PdfGlyphNames.S }, 
			{ 138, PdfGlyphNames.Scaron }, { 84, PdfGlyphNames.T }, { 222, PdfGlyphNames.Thorn }, { 85, PdfGlyphNames.U }, { 218, PdfGlyphNames.Uacute }, { 219, PdfGlyphNames.Ucircumflex }, 
			{ 220, PdfGlyphNames.Udieresis }, { 217, PdfGlyphNames.Ugrave }, { 86, PdfGlyphNames.V }, { 87, PdfGlyphNames.W }, { 88, PdfGlyphNames.X }, { 89, PdfGlyphNames.Y }, 
			{ 221, PdfGlyphNames.Yacute }, { 159, PdfGlyphNames.Ydieresis }, { 90, PdfGlyphNames.Z }, { 142, PdfGlyphNames.Zcaron }, { 97, PdfGlyphNames.a }, { 225, PdfGlyphNames.aacute }, 
			{ 226, PdfGlyphNames.acircumflex }, { 180, PdfGlyphNames.acute }, { 228, PdfGlyphNames.adieresis }, { 230, PdfGlyphNames.ae }, { 224, PdfGlyphNames.agrave }, { 38, PdfGlyphNames.ampersand }, 
			{ 229, PdfGlyphNames.aring }, { 94, PdfGlyphNames.asciicircum }, { 126, PdfGlyphNames.asciitilde }, { 42, PdfGlyphNames.asterisk }, { 64, PdfGlyphNames.at }, { 227, PdfGlyphNames.atilde }, 
			{ 98, PdfGlyphNames.b }, { 92, PdfGlyphNames.backslash }, { 124, PdfGlyphNames.bar }, { 123, PdfGlyphNames.braceleft }, { 125, PdfGlyphNames.braceright }, { 91, PdfGlyphNames.bracketleft }, 
			{ 93, PdfGlyphNames.bracketright }, { 166, PdfGlyphNames.brokenbar }, { 127, PdfGlyphNames.bullet }, { 149, PdfGlyphNames.bullet }, { 99, PdfGlyphNames.c }, { 231, PdfGlyphNames.ccedilla }, 
			{ 184, PdfGlyphNames.cedilla }, { 162, PdfGlyphNames.cent }, { 136, PdfGlyphNames.circumflex }, { 58, PdfGlyphNames.colon }, { 44, PdfGlyphNames.comma }, { 169, PdfGlyphNames.copyright }, 
			{ 164, PdfGlyphNames.currency }, { 100, PdfGlyphNames.d }, { 134, PdfGlyphNames.dagger }, { 135, PdfGlyphNames.daggerdbl }, { 176, PdfGlyphNames.degree }, { 168, PdfGlyphNames.dieresis }, 
			{ 247, PdfGlyphNames.divide }, { 36, PdfGlyphNames.dollar }, { 101, PdfGlyphNames.e }, { 233, PdfGlyphNames.eacute }, { 234, PdfGlyphNames.ecircumflex }, { 235, PdfGlyphNames.edieresis }, 
			{ 232, PdfGlyphNames.egrave }, { 56, PdfGlyphNames.eight }, { 133, PdfGlyphNames.ellipsis }, { 151, PdfGlyphNames.emdash }, { 150, PdfGlyphNames.endash }, { 61, PdfGlyphNames.equal }, 
			{ 240, PdfGlyphNames.eth }, { 33, PdfGlyphNames.exclam }, { 161, PdfGlyphNames.exclamdown }, { 102, PdfGlyphNames.f }, { 53, PdfGlyphNames.five }, { 131, PdfGlyphNames.florin }, 
			{ 52, PdfGlyphNames.four }, { 103, PdfGlyphNames.g }, { 223, PdfGlyphNames.germandbls }, { 96, PdfGlyphNames.grave }, { 62, PdfGlyphNames.greater }, { 171, PdfGlyphNames.guillemotleft }, 
			{ 187, PdfGlyphNames.guillemotright }, { 139, PdfGlyphNames.guilsinglleft }, { 155, PdfGlyphNames.guilsinglright }, { 104, PdfGlyphNames.h }, { 45, PdfGlyphNames.hyphen }, 
			{ 105, PdfGlyphNames.i }, { 237, PdfGlyphNames.iacute }, { 238, PdfGlyphNames.icircumflex }, { 239, PdfGlyphNames.idieresis }, { 236, PdfGlyphNames.igrave }, { 106, PdfGlyphNames.j }, 
			{ 107, PdfGlyphNames.k }, { 108, PdfGlyphNames.l }, { 60, PdfGlyphNames.less }, { 172, PdfGlyphNames.logicalnot }, { 109, PdfGlyphNames.m }, { 175, PdfGlyphNames.macron }, 
			{ 181, PdfGlyphNames.mu }, { 215, PdfGlyphNames.multiply }, { 110, PdfGlyphNames.n }, { 57, PdfGlyphNames.nine }, { 241, PdfGlyphNames.ntilde }, { 35, PdfGlyphNames.numbersign }, 
			{ 111, PdfGlyphNames.o }, { 243, PdfGlyphNames.oacute }, { 244, PdfGlyphNames.ocircumflex }, { 246, PdfGlyphNames.odieresis }, { 156, PdfGlyphNames.oe }, { 242, PdfGlyphNames.ograve }, 
			{ 49, PdfGlyphNames.one }, { 189, PdfGlyphNames.onehalf }, { 188, PdfGlyphNames.onequarter }, { 185, PdfGlyphNames.onesuperior }, { 170, PdfGlyphNames.ordfeminine }, 
			{ 186, PdfGlyphNames.ordmasculine }, { 248, PdfGlyphNames.oslash }, { 245, PdfGlyphNames.otilde }, { 112, PdfGlyphNames.p }, { 182, PdfGlyphNames.paragraph }, { 40, PdfGlyphNames.parenleft },
			{ 41, PdfGlyphNames.parenright }, { 37, PdfGlyphNames.percent }, { 46, PdfGlyphNames.period }, { 183, PdfGlyphNames.periodcentered }, { 137, PdfGlyphNames.perthousand }, 
			{ 43, PdfGlyphNames.plus }, { 177, PdfGlyphNames.plusminus }, { 113, PdfGlyphNames.q }, { 63, PdfGlyphNames.question }, { 191, PdfGlyphNames.questiondown }, { 34, PdfGlyphNames.quotedbl }, 
			{ 132, PdfGlyphNames.quotedblbase }, { 147, PdfGlyphNames.quotedblleft }, { 148, PdfGlyphNames.quotedblright }, { 145, PdfGlyphNames.quoteleft }, { 146, PdfGlyphNames.quoteright }, 
			{ 130, PdfGlyphNames.quotesinglbase }, { 39, PdfGlyphNames.quotesingle }, { 114, PdfGlyphNames.r }, { 174, PdfGlyphNames.registered }, { 115, PdfGlyphNames.s }, { 154, PdfGlyphNames.scaron },
			{ 167, PdfGlyphNames.section }, { 59, PdfGlyphNames.semicolon }, { 55, PdfGlyphNames.seven }, { 54, PdfGlyphNames.six }, { 47, PdfGlyphNames.slash }, { 10, PdfGlyphNames.space }, 
			{ 13, PdfGlyphNames.space }, { 32, PdfGlyphNames.space }, { 160, PdfGlyphNames.space }, { 163, PdfGlyphNames.sterling }, { 116, PdfGlyphNames.t }, { 254, PdfGlyphNames.thorn }, 
			{ 51, PdfGlyphNames.three }, { 190, PdfGlyphNames.threequarters }, { 179, PdfGlyphNames.threesuperior }, { 152, PdfGlyphNames.tilde }, { 153, PdfGlyphNames.trademark }, 
			{ 50, PdfGlyphNames.two }, { 178, PdfGlyphNames.twosuperior }, { 117, PdfGlyphNames.u }, { 250, PdfGlyphNames.uacute }, { 251, PdfGlyphNames.ucircumflex }, { 252, PdfGlyphNames.udieresis }, 
			{ 249, PdfGlyphNames.ugrave }, { 95, PdfGlyphNames.underscore }, { 118, PdfGlyphNames.v }, { 119, PdfGlyphNames.w }, { 120, PdfGlyphNames.x }, { 121, PdfGlyphNames.y }, 
			{ 253, PdfGlyphNames.yacute }, { 255, PdfGlyphNames.ydieresis }, { 165, PdfGlyphNames.yen }, { 122, PdfGlyphNames.z }, { 158, PdfGlyphNames.zcaron }, { 48, PdfGlyphNames.zero } 
		};
		static readonly Dictionary<byte, string> symbolEncoding = new Dictionary<byte, string> {
			{ 65, PdfGlyphNames.Alpha }, { 66, PdfGlyphNames.Beta }, { 67, PdfGlyphNames.Chi }, { 68, PdfGlyphNames.Delta }, { 69, PdfGlyphNames.Epsilon }, { 72, PdfGlyphNames.Eta }, 
			{ 160, PdfGlyphNames.Euro }, { 71, PdfGlyphNames.Gamma }, { 193, PdfGlyphNames.Ifraktur }, { 73, PdfGlyphNames.Iota }, { 75, PdfGlyphNames.Kappa }, { 76, PdfGlyphNames.Lambda },
			{ 77, PdfGlyphNames.Mu }, { 78, PdfGlyphNames.Nu }, { 87, PdfGlyphNames.Omega }, { 79, PdfGlyphNames.Omicron }, { 70, PdfGlyphNames.Phi }, { 80, PdfGlyphNames.Pi }, { 89, PdfGlyphNames.Psi },
			{ 194, PdfGlyphNames.Rfraktur }, { 82, PdfGlyphNames.Rho }, { 83, PdfGlyphNames.Sigma }, { 84, PdfGlyphNames.Tau }, { 81, PdfGlyphNames.Theta }, { 85, PdfGlyphNames.Upsilon }, 
			{ 161, PdfGlyphNames.Upsilon1 }, { 88, PdfGlyphNames.Xi }, { 90, PdfGlyphNames.Zeta }, { 192, PdfGlyphNames.aleph }, { 97, PdfGlyphNames.alpha }, { 38, PdfGlyphNames.ampersand },
			{ 208, PdfGlyphNames.angle }, { 225, PdfGlyphNames.angleleft }, { 241, PdfGlyphNames.angleright }, { 187, PdfGlyphNames.approxequal }, { 171, PdfGlyphNames.arrowboth }, 
			{ 219, PdfGlyphNames.arrowdblboth }, { 223, PdfGlyphNames.arrowdbldown }, { 220, PdfGlyphNames.arrowdblleft }, { 222, PdfGlyphNames.arrowdblright }, { 221, PdfGlyphNames.arrowdblup },
			{ 175, PdfGlyphNames.arrowdown }, { 190, PdfGlyphNames.arrowhorizex }, { 172, PdfGlyphNames.arrowleft }, { 174, PdfGlyphNames.arrowright }, { 173, PdfGlyphNames.arrowup }, 
			{ 189, PdfGlyphNames.arrowvertex }, { 42, PdfGlyphNames.asteriskmath }, { 124, PdfGlyphNames.bar }, { 98, PdfGlyphNames.beta }, { 123, PdfGlyphNames.braceleft }, 
			{ 125, PdfGlyphNames.braceright }, { 236, PdfGlyphNames.bracelefttp }, { 237, PdfGlyphNames.braceleftmid }, { 238, PdfGlyphNames.braceleftbt }, { 252, PdfGlyphNames.bracerighttp },
			{ 253, PdfGlyphNames.bracerightmid }, { 254, PdfGlyphNames.bracerightbt }, { 239, PdfGlyphNames.braceex }, { 91, PdfGlyphNames.bracketleft }, { 93, PdfGlyphNames.bracketright },
			{ 233, PdfGlyphNames.bracketlefttp }, { 234, PdfGlyphNames.bracketleftex }, { 235, PdfGlyphNames.bracketleftbt }, { 249, PdfGlyphNames.bracketrighttp }, { 250, PdfGlyphNames.bracketrightex },
			{ 251, PdfGlyphNames.bracketrightbt }, { 183, PdfGlyphNames.bullet }, { 191, PdfGlyphNames.carriagereturn }, { 99, PdfGlyphNames.chi }, { 196, PdfGlyphNames.circlemultiply },
			{ 197, PdfGlyphNames.circleplus }, { 167, PdfGlyphNames.club }, { 58, PdfGlyphNames.colon }, { 44, PdfGlyphNames.comma }, { 64, PdfGlyphNames.congruent }, { 227, PdfGlyphNames.copyrightsans },
			{ 211, PdfGlyphNames.copyrightserif }, { 176, PdfGlyphNames.degree }, { 100, PdfGlyphNames.delta }, { 168, PdfGlyphNames.diamond }, { 184, PdfGlyphNames.divide }, { 215, PdfGlyphNames.dotmath },
			{ 56, PdfGlyphNames.eight }, { 206, PdfGlyphNames.element }, { 188, PdfGlyphNames.ellipsis }, { 198, PdfGlyphNames.emptyset }, { 101, PdfGlyphNames.epsilon }, { 61, PdfGlyphNames.equal },
			{ 186, PdfGlyphNames.equivalence }, { 104, PdfGlyphNames.eta }, { 33, PdfGlyphNames.exclam }, { 36, PdfGlyphNames.existential }, { 53, PdfGlyphNames.five }, { 166, PdfGlyphNames.florin },
			{ 52, PdfGlyphNames.four }, { 164, PdfGlyphNames.fraction }, { 103, PdfGlyphNames.gamma }, { 209, PdfGlyphNames.gradient }, { 62, PdfGlyphNames.greater }, { 179, PdfGlyphNames.greaterequal },
			{ 169, PdfGlyphNames.heart }, { 165, PdfGlyphNames.infinity }, { 242, PdfGlyphNames.integral }, { 243, PdfGlyphNames.integraltp }, { 244, PdfGlyphNames.integralex }, 
			{ 245, PdfGlyphNames.integralbt }, { 199, PdfGlyphNames.intersection }, { 105, PdfGlyphNames.iota }, { 107, PdfGlyphNames.kappa }, { 108, PdfGlyphNames.lambda }, { 60, PdfGlyphNames.less },
			{ 163, PdfGlyphNames.lessequal }, { 217, PdfGlyphNames.logicaland }, { 216, PdfGlyphNames.logicalnot }, { 218, PdfGlyphNames.logicalor }, { 224, PdfGlyphNames.lozenge }, 
			{ 45, PdfGlyphNames.minus }, { 162, PdfGlyphNames.minute }, { 109, PdfGlyphNames.mu }, { 180, PdfGlyphNames.multiply }, { 57, PdfGlyphNames.nine }, { 207, PdfGlyphNames.notelement },
			{ 185, PdfGlyphNames.notequal }, { 203, PdfGlyphNames.notsubset }, { 110, PdfGlyphNames.nu }, { 35, PdfGlyphNames.numbersign }, { 119, PdfGlyphNames.omega }, { 118, PdfGlyphNames.omega1 },
			{ 111, PdfGlyphNames.omicron }, { 49, PdfGlyphNames.one }, { 40, PdfGlyphNames.parenleft }, { 41, PdfGlyphNames.parenright }, { 230, PdfGlyphNames.parenlefttp }, 
			{ 231, PdfGlyphNames.parenleftex }, { 232, PdfGlyphNames.parenleftbt }, { 246, PdfGlyphNames.parenrighttp }, { 247, PdfGlyphNames.parenrightex }, { 248, PdfGlyphNames.parenrightbt },
			{ 182, PdfGlyphNames.partialdiff }, { 37, PdfGlyphNames.percent }, { 46, PdfGlyphNames.period }, { 94, PdfGlyphNames.perpendicular }, { 102, PdfGlyphNames.phi }, { 106, PdfGlyphNames.phi1 },
			{ 112, PdfGlyphNames.pi }, { 43, PdfGlyphNames.plus }, { 177, PdfGlyphNames.plusminus }, { 213, PdfGlyphNames.product }, { 204, PdfGlyphNames.propersubset }, 
			{ 201, PdfGlyphNames.propersuperset }, { 181, PdfGlyphNames.proportional }, { 121, PdfGlyphNames.psi }, { 63, PdfGlyphNames.question }, { 214, PdfGlyphNames.radical }, 
			{ 96, PdfGlyphNames.radicalex }, { 205, PdfGlyphNames.reflexsubset }, { 202, PdfGlyphNames.reflexsuperset }, { 226, PdfGlyphNames.registersans }, { 210, PdfGlyphNames.registerserif },
			{ 114, PdfGlyphNames.rho }, { 178, PdfGlyphNames.second }, { 59, PdfGlyphNames.semicolon }, { 55, PdfGlyphNames.seven }, { 115, PdfGlyphNames.sigma }, { 86, PdfGlyphNames.sigma1 },
			{ 126, PdfGlyphNames.similar }, { 54, PdfGlyphNames.six }, { 47, PdfGlyphNames.slash }, { 32, PdfGlyphNames.space }, { 170, PdfGlyphNames.spade }, { 39, PdfGlyphNames.suchthat },
			{ 229, PdfGlyphNames.summation }, { 116, PdfGlyphNames.tau }, { 92, PdfGlyphNames.therefore }, { 113, PdfGlyphNames.theta }, { 74, PdfGlyphNames.theta1 }, { 51, PdfGlyphNames.three },
			{ 228, PdfGlyphNames.trademarksans }, { 212, PdfGlyphNames.trademarkserif }, { 50, PdfGlyphNames.two }, { 95, PdfGlyphNames.underscore }, { 200, PdfGlyphNames.union }, 
			{ 34, PdfGlyphNames.universal }, { 117, PdfGlyphNames.upsilon }, { 195, PdfGlyphNames.weierstrass }, { 120, PdfGlyphNames.xi }, { 48, PdfGlyphNames.zero }, { 122, PdfGlyphNames.zeta }
		};
		static readonly Dictionary<byte, string> zapfDingbatsEncoding = new Dictionary<byte, string> {
			{ 32, PdfGlyphNames.space }, { 33, PdfGlyphNames.a1 }, { 34, PdfGlyphNames.a2 }, { 35, PdfGlyphNames.a202 }, { 36, PdfGlyphNames.a3 }, { 37, PdfGlyphNames.a4 }, { 38, PdfGlyphNames.a5 },
			{ 39, PdfGlyphNames.a119 }, { 40, PdfGlyphNames.a118 }, { 41, PdfGlyphNames.a117 }, { 42, PdfGlyphNames.a11 }, { 43, PdfGlyphNames.a12 }, { 44, PdfGlyphNames.a13 }, { 45, PdfGlyphNames.a14 },
			{ 46, PdfGlyphNames.a15 }, { 47, PdfGlyphNames.a16 }, { 48, PdfGlyphNames.a105 }, { 49, PdfGlyphNames.a17 }, { 50, PdfGlyphNames.a18 }, { 51, PdfGlyphNames.a19 }, { 52, PdfGlyphNames.a20 },
			{ 53, PdfGlyphNames.a21 }, { 54, PdfGlyphNames.a22 }, { 55, PdfGlyphNames.a23 }, { 56, PdfGlyphNames.a24 }, { 57, PdfGlyphNames.a25 }, { 58, PdfGlyphNames.a26 }, { 59, PdfGlyphNames.a27 },
			{ 60, PdfGlyphNames.a28 }, { 61, PdfGlyphNames.a6 }, { 62, PdfGlyphNames.a7 }, { 63, PdfGlyphNames.a8 }, { 64, PdfGlyphNames.a9 }, { 65, PdfGlyphNames.a10 }, { 66, PdfGlyphNames.a29 },
			{ 67, PdfGlyphNames.a30 }, { 68, PdfGlyphNames.a31 }, { 69, PdfGlyphNames.a32 }, { 70, PdfGlyphNames.a33 }, { 71, PdfGlyphNames.a34 }, { 72, PdfGlyphNames.a35 }, { 73, PdfGlyphNames.a36 },
			{ 74, PdfGlyphNames.a37 }, { 75, PdfGlyphNames.a38 }, { 76, PdfGlyphNames.a39 }, { 77, PdfGlyphNames.a40 }, { 78, PdfGlyphNames.a41 }, { 79, PdfGlyphNames.a42 }, { 80, PdfGlyphNames.a43 },
			{ 81, PdfGlyphNames.a44 }, { 82, PdfGlyphNames.a45 }, { 83, PdfGlyphNames.a46 }, { 84, PdfGlyphNames.a47 }, { 85, PdfGlyphNames.a48 }, { 86, PdfGlyphNames.a49 }, { 87, PdfGlyphNames.a50 },
			{ 88, PdfGlyphNames.a51 }, { 89, PdfGlyphNames.a52 }, { 90, PdfGlyphNames.a53 }, { 91, PdfGlyphNames.a54 }, { 92, PdfGlyphNames.a55 }, { 93, PdfGlyphNames.a56 }, { 94, PdfGlyphNames.a57 },
			{ 95, PdfGlyphNames.a58 }, { 96, PdfGlyphNames.a59 }, { 97, PdfGlyphNames.a60 }, { 98, PdfGlyphNames.a61 }, { 99, PdfGlyphNames.a62 }, { 100, PdfGlyphNames.a63 }, { 101, PdfGlyphNames.a64 },
			{ 102, PdfGlyphNames.a65 }, { 103, PdfGlyphNames.a66 }, { 104, PdfGlyphNames.a67 }, { 105, PdfGlyphNames.a68 }, { 106, PdfGlyphNames.a69 }, { 107, PdfGlyphNames.a70 }, 
			{ 108, PdfGlyphNames.a71 }, { 109, PdfGlyphNames.a72 }, { 110, PdfGlyphNames.a73 }, { 111, PdfGlyphNames.a74 }, { 112, PdfGlyphNames.a203 }, { 113, PdfGlyphNames.a75 },
			{ 114, PdfGlyphNames.a204 }, { 115, PdfGlyphNames.a76 }, { 116, PdfGlyphNames.a77 }, { 117, PdfGlyphNames.a78 }, { 118, PdfGlyphNames.a79 }, { 119, PdfGlyphNames.a81 },
			{ 120, PdfGlyphNames.a82 }, { 121, PdfGlyphNames.a83 }, { 122, PdfGlyphNames.a84 }, { 123, PdfGlyphNames.a97 }, { 124, PdfGlyphNames.a98 }, { 125, PdfGlyphNames.a99 }, 
			{ 126, PdfGlyphNames.a100 }, { 161, PdfGlyphNames.a101 }, { 162, PdfGlyphNames.a102 }, { 163, PdfGlyphNames.a103 }, { 164, PdfGlyphNames.a104 }, { 165, PdfGlyphNames.a106 },
			{ 166, PdfGlyphNames.a107 }, { 167, PdfGlyphNames.a108 }, { 168, PdfGlyphNames.a112 }, { 169, PdfGlyphNames.a111 }, { 170, PdfGlyphNames.a110 }, { 171, PdfGlyphNames.a109 },
			{ 172, PdfGlyphNames.a120 }, { 173, PdfGlyphNames.a121 }, { 174, PdfGlyphNames.a122 }, { 175, PdfGlyphNames.a123 }, { 176, PdfGlyphNames.a124 }, { 177, PdfGlyphNames.a125 },
			{ 178, PdfGlyphNames.a126 }, { 179, PdfGlyphNames.a127 }, { 180, PdfGlyphNames.a128 }, { 181, PdfGlyphNames.a129 }, { 182, PdfGlyphNames.a130 }, { 183, PdfGlyphNames.a131 },
			{ 184, PdfGlyphNames.a132 }, { 185, PdfGlyphNames.a133 }, { 186, PdfGlyphNames.a134 }, { 187, PdfGlyphNames.a135 }, { 188, PdfGlyphNames.a136 }, { 189, PdfGlyphNames.a137 },
			{ 190, PdfGlyphNames.a138 }, { 191, PdfGlyphNames.a139 }, { 192, PdfGlyphNames.a140 }, { 193, PdfGlyphNames.a141 }, { 194, PdfGlyphNames.a142 }, { 195, PdfGlyphNames.a143 },
			{ 196, PdfGlyphNames.a144 }, { 197, PdfGlyphNames.a145 }, { 198, PdfGlyphNames.a146 }, { 199, PdfGlyphNames.a147 }, { 200, PdfGlyphNames.a148 }, { 201, PdfGlyphNames.a149 },
			{ 202, PdfGlyphNames.a150 }, { 203, PdfGlyphNames.a151 }, { 204, PdfGlyphNames.a152 }, { 205, PdfGlyphNames.a153 }, { 206, PdfGlyphNames.a154 }, { 207, PdfGlyphNames.a155 },
			{ 208, PdfGlyphNames.a156 }, { 209, PdfGlyphNames.a157 }, { 210, PdfGlyphNames.a158 }, { 211, PdfGlyphNames.a159 }, { 212, PdfGlyphNames.a160 }, { 213, PdfGlyphNames.a161 },
			{ 214, PdfGlyphNames.a163 }, { 215, PdfGlyphNames.a164 }, { 216, PdfGlyphNames.a196 }, { 217, PdfGlyphNames.a165 }, { 218, PdfGlyphNames.a192 }, { 219, PdfGlyphNames.a166 },
			{ 220, PdfGlyphNames.a167 }, { 221, PdfGlyphNames.a168 }, { 222, PdfGlyphNames.a169 }, { 223, PdfGlyphNames.a170 }, { 224, PdfGlyphNames.a171 }, { 225, PdfGlyphNames.a172 },
			{ 226, PdfGlyphNames.a173 }, { 227, PdfGlyphNames.a162 }, { 228, PdfGlyphNames.a174 }, { 229, PdfGlyphNames.a175 }, { 230, PdfGlyphNames.a176 }, { 231, PdfGlyphNames.a177 },
			{ 232, PdfGlyphNames.a178 }, { 233, PdfGlyphNames.a179 }, { 234, PdfGlyphNames.a193 }, { 235, PdfGlyphNames.a180 }, { 236, PdfGlyphNames.a199 }, { 237, PdfGlyphNames.a181 },
			{ 238, PdfGlyphNames.a200 }, { 239, PdfGlyphNames.a182 }, { 240, PdfGlyphNames.a201 }, { 241, PdfGlyphNames.a183 }, { 242, PdfGlyphNames.a184 }, { 243, PdfGlyphNames.a197 },
			{ 244, PdfGlyphNames.a185 }, { 245, PdfGlyphNames.a194 }, { 246, PdfGlyphNames.a198 }, { 247, PdfGlyphNames.a186 }, { 249, PdfGlyphNames.a195 }, { 250, PdfGlyphNames.a187 },
			{ 251, PdfGlyphNames.a188 }, { 252, PdfGlyphNames.a189 }, { 253, PdfGlyphNames.a190 }, { 254, PdfGlyphNames.a191 }
		};
		internal static PdfSimpleFontEncoding Create(string baseFont, object value) {
			if (value == null)
				return new PdfSimpleFontEncoding(baseFont, String.Empty);
			PdfName name = value as PdfName;
			if (name != null)
				return new PdfSimpleFontEncoding(baseFont, name.Name);
			PdfReaderDictionary encodingDictionary = value as PdfReaderDictionary;
			if (encodingDictionary == null)
				PdfDocumentReader.ThrowIncorrectDataException();
			string type = encodingDictionary.GetName(PdfDictionary.DictionaryTypeKey);
			if (type != null && type != "Encoding")
				PdfDocumentReader.ThrowIncorrectDataException();
			string encodingName = encodingDictionary.GetName(baseEncodingDictionaryKey);
			SortedDictionary<int, string> differences = new SortedDictionary<int, string>();
			IList<object> diff = encodingDictionary.GetArray(differencesDictionaryKey);
			if (diff != null) {
				int count = diff.Count;
				if (count > 0) {
					object firstCode = diff[0];
					if (!(firstCode is int))
						PdfDocumentReader.ThrowIncorrectDataException();
					int code = (int)firstCode;
					for (int i = 1; i < count; i++) {
						object diffValue = diff[i];
						PdfName diffName = diffValue as PdfName;
						if (diffName == null) {
							if (!(diffValue is int))
								PdfDocumentReader.ThrowIncorrectDataException();
							code = (int)diffValue;
						}
						else
							differences[code++] = diffName.Name;
					}
				}
			}
			return new PdfSimpleFontEncoding(baseFont, encodingName, differences);
		}
		readonly PdfBaseEncoding baseEncoding;
		readonly IDictionary<int, string> differences;
		readonly IDictionary<byte, string> baseEncodingDictionary;
		public PdfBaseEncoding BaseEncoding { get { return baseEncoding; } }
		public IDictionary<int, string> Differences { get { return differences; } }
		internal PdfSimpleFontEncoding(string baseFont, string encodingName, IDictionary<int, string> differences = null) {
			this.differences = differences ?? new SortedDictionary<int, string>();
			if (encodingName == null)
				encodingName = String.Empty;
			switch (encodingName) {
				case macRomanEncodingName:
					baseEncoding = PdfBaseEncoding.MacRoman;
					baseEncodingDictionary = MacRomanEncoding;
					break;
				case winAnsiEncodingName:
					baseEncoding = PdfBaseEncoding.WinAnsi;
					baseEncodingDictionary = winAnsiEncoding;
					break;
				default:
					baseEncoding = PdfBaseEncoding.Standard;
					switch (baseFont) {
						case PdfType1Font.SymbolFontName:
							baseEncodingDictionary = symbolEncoding;
							break;
						case PdfType1Font.ZapfDingbatsFontName:
							baseEncodingDictionary = zapfDingbatsEncoding;
							break;
						default:
							baseEncodingDictionary = standardEncoding;
							break;
					}
					break;
			}
		}
		internal string GetGlyphName(byte code) {
			string glyphName;
			if (differences.TryGetValue(code, out glyphName))
				return glyphName;
			return baseEncodingDictionary.TryGetValue(code, out glyphName) ? glyphName : PdfGlyphNames._notdef;
		}
		protected internal override PdfStringData GetStringData(byte[] bytes, double[] glyphOffsets) {
			int length = bytes.Length;
			short[] chars = new short[length];
			byte[][] codes = new byte[length][];
			for (int i = 0; i < length; i++) {
				chars[i] = (short)bytes[i];
				codes[i] = new byte[] { bytes[i] };
			}
			return new PdfStringData(codes, chars, glyphOffsets ?? new double[length + 1]);
		}
		protected internal override object Write(PdfObjectCollection objects) {
			string encodingName;
			switch (baseEncoding) {
				case PdfBaseEncoding.MacRoman:
					encodingName = macRomanEncodingName;
					break;
				case PdfBaseEncoding.WinAnsi:
					encodingName = winAnsiEncodingName;
					break;
				default:
					encodingName = null;
					break;
			}
			if (differences.Count == 0)
				return String.IsNullOrEmpty(encodingName) ? null : new PdfName(encodingName);
			PdfDictionary dicionary = new PdfDictionary();
			if (!String.IsNullOrEmpty(encodingName))
				dicionary.Add(baseEncodingDictionaryKey, new PdfName(encodingName));
			List<object> diffs = new List<object>();
			int previous = int.MinValue;
			foreach (KeyValuePair<int, string> diff in differences) {
				if (diff.Key != ++previous) {
					diffs.Add(diff.Key);
					previous = diff.Key;
				}
				diffs.Add(new PdfName(diff.Value));
			}
			dicionary.Add(differencesDictionaryKey, diffs);
			return dicionary;
		}
		protected internal override object ToWritableObject(PdfObjectCollection objects) {
			return Write(objects);
		}
	}
}
