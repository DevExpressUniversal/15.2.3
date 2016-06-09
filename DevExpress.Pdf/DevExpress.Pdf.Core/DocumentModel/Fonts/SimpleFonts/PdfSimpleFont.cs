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
	public abstract class PdfSimpleFont : PdfFont {
		const string firstCharDictionaryKey = "FirstChar";
		const string lastCharDictionaryKey = "LastChar";
		const string widthsDictionaryKey = "Widths";
		internal const string CourierNewFontName = "CourierNew";
		internal const string HelveticaFontName = "Helvetica";
		internal const string HelveticaBoldFontName = "Helvetica-Bold";
		internal const string ArialFontName = "Arial";
		protected const string ArialBoldFontName = "Arial,Bold";
		protected const string TimesNewRomanFontName = "TimesNewRoman";
		protected const string TimesNewRomanBoldFontName = "TimesNewRoman,Bold";
		static readonly Dictionary<string, short> timesRomanWidths = new Dictionary<string, short>() {
			{ PdfGlyphNames._notdef, 250 }, { PdfGlyphNames.space, 250 }, { PdfGlyphNames.exclam, 333 }, { PdfGlyphNames.quotedbl, 408 }, { PdfGlyphNames.numbersign, 500 }, { PdfGlyphNames.dollar, 500 }, 
			{ PdfGlyphNames.percent, 833 }, { PdfGlyphNames.ampersand, 778 }, { PdfGlyphNames.quoteright, 333 }, { PdfGlyphNames.parenleft, 333 }, { PdfGlyphNames.parenright, 333 }, 
			{ PdfGlyphNames.asterisk, 500 }, { PdfGlyphNames.plus, 564 }, { PdfGlyphNames.comma, 250 }, { PdfGlyphNames.hyphen, 333 }, { PdfGlyphNames.period, 250 }, { PdfGlyphNames.slash, 278 }, 
			{ PdfGlyphNames.zero, 500 }, { PdfGlyphNames.one, 500 }, { PdfGlyphNames.two, 500 }, { PdfGlyphNames.three, 500 }, { PdfGlyphNames.four, 500 }, { PdfGlyphNames.five, 500 }, { PdfGlyphNames.six, 500 }, 
			{ PdfGlyphNames.seven, 500 }, { PdfGlyphNames.eight, 500 }, { PdfGlyphNames.nine, 500 }, { PdfGlyphNames.colon, 278 }, { PdfGlyphNames.semicolon, 278 }, { PdfGlyphNames.less, 564 }, 
			{ PdfGlyphNames.equal, 564 }, { PdfGlyphNames.greater, 564 }, { PdfGlyphNames.question, 444 }, { PdfGlyphNames.at, 921 }, { PdfGlyphNames.A, 722 }, { PdfGlyphNames.B, 667 }, { PdfGlyphNames.C, 667 }, 
			{ PdfGlyphNames.D, 722 }, { PdfGlyphNames.E, 611 }, { PdfGlyphNames.F, 556 }, { PdfGlyphNames.G, 722 }, { PdfGlyphNames.H, 722 }, { PdfGlyphNames.I, 333 }, { PdfGlyphNames.J, 389 }, 
			{ PdfGlyphNames.K, 722 }, { PdfGlyphNames.L, 611 }, { PdfGlyphNames.M, 889 }, { PdfGlyphNames.N, 722 }, { PdfGlyphNames.O, 722 }, { PdfGlyphNames.P, 556 }, { PdfGlyphNames.Q, 722 }, 
			{ PdfGlyphNames.R, 667 }, { PdfGlyphNames.S, 556 }, { PdfGlyphNames.T, 611 }, { PdfGlyphNames.U, 722 }, { PdfGlyphNames.V, 722 }, { PdfGlyphNames.W, 944 }, { PdfGlyphNames.X, 722 }, 
			{ PdfGlyphNames.Y, 722 }, { PdfGlyphNames.Z, 611 }, { PdfGlyphNames.bracketleft, 333 }, { PdfGlyphNames.backslash, 278 }, { PdfGlyphNames.bracketright, 333 }, { PdfGlyphNames.asciicircum, 469 }, 
			{ PdfGlyphNames.underscore, 500 }, { PdfGlyphNames.quoteleft, 333 }, { PdfGlyphNames.a, 444 }, { PdfGlyphNames.b, 500 }, { PdfGlyphNames.c, 444 }, { PdfGlyphNames.d, 500 }, { PdfGlyphNames.e, 444 }, 
			{ PdfGlyphNames.f, 333 }, { PdfGlyphNames.g, 500 }, { PdfGlyphNames.h, 500 }, { PdfGlyphNames.i, 278 }, { PdfGlyphNames.j, 278 }, { PdfGlyphNames.k, 500 }, { PdfGlyphNames.l, 278 }, 
			{ PdfGlyphNames.m, 778 }, { PdfGlyphNames.n, 500 }, { PdfGlyphNames.o, 500 }, { PdfGlyphNames.p, 500 }, { PdfGlyphNames.q, 500 }, { PdfGlyphNames.r, 333 }, { PdfGlyphNames.s, 389 }, 
			{ PdfGlyphNames.t, 278 }, { PdfGlyphNames.u, 500 }, { PdfGlyphNames.v, 500 }, { PdfGlyphNames.w, 722 }, { PdfGlyphNames.x, 500 }, { PdfGlyphNames.y, 500 }, { PdfGlyphNames.z, 444 }, 
			{ PdfGlyphNames.braceleft, 480 }, { PdfGlyphNames.bar, 200 }, { PdfGlyphNames.braceright, 480 }, { PdfGlyphNames.asciitilde, 541 }, { PdfGlyphNames.exclamdown, 333 }, { PdfGlyphNames.cent, 500 }, 
			{ PdfGlyphNames.sterling, 500 }, { PdfGlyphNames.fraction, 167 }, { PdfGlyphNames.yen, 500 }, { PdfGlyphNames.florin, 500 }, { PdfGlyphNames.section, 500 }, { PdfGlyphNames.currency, 500 }, 
			{ PdfGlyphNames.quotesingle, 180 }, { PdfGlyphNames.quotedblleft, 444 }, { PdfGlyphNames.guillemotleft, 500 }, { PdfGlyphNames.guilsinglleft, 333 }, { PdfGlyphNames.guilsinglright, 333 }, 
			{ PdfGlyphNames.fi, 556 }, { PdfGlyphNames.fl, 556 }, { PdfGlyphNames.endash, 500 }, { PdfGlyphNames.dagger, 500 }, { PdfGlyphNames.daggerdbl, 500 }, { PdfGlyphNames.periodcentered, 250 }, 
			{ PdfGlyphNames.paragraph, 453 }, { PdfGlyphNames.bullet, 350 }, { PdfGlyphNames.quotesinglbase, 333 }, { PdfGlyphNames.quotedblbase, 444 }, { PdfGlyphNames.quotedblright, 444 }, 
			{ PdfGlyphNames.guillemotright, 500 }, { PdfGlyphNames.ellipsis, 1000 }, { PdfGlyphNames.perthousand, 1000 }, { PdfGlyphNames.questiondown, 444 }, { PdfGlyphNames.grave, 333 }, 
			{ PdfGlyphNames.acute, 333 }, { PdfGlyphNames.circumflex, 333 }, { PdfGlyphNames.tilde, 333 }, { PdfGlyphNames.macron, 333 }, { PdfGlyphNames.breve, 333 }, { PdfGlyphNames.dotaccent, 333 }, 
			{ PdfGlyphNames.dieresis, 333 }, { PdfGlyphNames.ring, 333 }, { PdfGlyphNames.cedilla, 333 }, { PdfGlyphNames.hungarumlaut, 333 }, { PdfGlyphNames.ogonek, 333 }, { PdfGlyphNames.caron, 333 }, 
			{ PdfGlyphNames.emdash, 1000 }, { PdfGlyphNames.AE, 889 }, { PdfGlyphNames.ordfeminine, 276 }, { PdfGlyphNames.Lslash, 611 }, { PdfGlyphNames.Oslash, 722 }, { PdfGlyphNames.OE, 889 }, 
			{ PdfGlyphNames.ordmasculine, 310 }, { PdfGlyphNames.ae, 667 }, { PdfGlyphNames.dotlessi, 278 }, { PdfGlyphNames.lslash, 278 }, { PdfGlyphNames.oslash, 500 }, { PdfGlyphNames.oe, 722 }, 
			{ PdfGlyphNames.germandbls, 500 }, { PdfGlyphNames.Idieresis, 333 }, { PdfGlyphNames.eacute, 444 }, { PdfGlyphNames.abreve, 444 }, { PdfGlyphNames.uhungarumlaut, 500 }, { PdfGlyphNames.ecaron, 444 }, 
			{ PdfGlyphNames.Ydieresis, 722 }, { PdfGlyphNames.divide, 564 }, { PdfGlyphNames.Yacute, 722 }, { PdfGlyphNames.Acircumflex, 722 }, { PdfGlyphNames.aacute, 444 }, { PdfGlyphNames.Ucircumflex, 722 }, 
			{ PdfGlyphNames.yacute, 500 }, { PdfGlyphNames.scommaaccent, 389 }, { PdfGlyphNames.ecircumflex, 444 }, { PdfGlyphNames.Uring, 722 }, { PdfGlyphNames.Udieresis, 722 }, { PdfGlyphNames.aogonek, 444 }, 
			{ PdfGlyphNames.Uacute, 722 }, { PdfGlyphNames.uogonek, 500 }, { PdfGlyphNames.Edieresis, 611 }, { PdfGlyphNames.Dcroat, 722 }, { PdfGlyphNames.commaaccent, 250 }, { PdfGlyphNames.copyright, 760 }, 
			{ PdfGlyphNames.Emacron, 611 }, { PdfGlyphNames.ccaron, 444 }, { PdfGlyphNames.aring, 444 }, { PdfGlyphNames.Ncommaaccent, 722 }, { PdfGlyphNames.lacute, 278 }, { PdfGlyphNames.agrave, 444 }, 
			{ PdfGlyphNames.Tcommaaccent, 611 }, { PdfGlyphNames.Cacute, 667 }, { PdfGlyphNames.atilde, 444 }, { PdfGlyphNames.Edotaccent, 611 }, { PdfGlyphNames.scaron, 389 }, { PdfGlyphNames.scedilla, 389 }, 
			{ PdfGlyphNames.iacute, 278 }, { PdfGlyphNames.lozenge, 471 }, { PdfGlyphNames.Rcaron, 667 }, { PdfGlyphNames.Gcommaaccent, 722 }, { PdfGlyphNames.ucircumflex, 500 }, 
			{ PdfGlyphNames.acircumflex, 444 }, { PdfGlyphNames.Amacron, 722 }, { PdfGlyphNames.rcaron, 333 }, { PdfGlyphNames.ccedilla, 444 }, { PdfGlyphNames.Zdotaccent, 611 }, { PdfGlyphNames.Thorn, 556 }, 
			{ PdfGlyphNames.Omacron, 722 }, { PdfGlyphNames.Racute, 667 }, { PdfGlyphNames.Sacute, 556 }, { PdfGlyphNames.dcaron, 588 }, { PdfGlyphNames.Umacron, 722 }, { PdfGlyphNames.uring, 500 }, 
			{ PdfGlyphNames.threesuperior, 300 }, { PdfGlyphNames.Ograve, 722 }, { PdfGlyphNames.Agrave, 722 }, { PdfGlyphNames.Abreve, 722 }, { PdfGlyphNames.multiply, 564 }, { PdfGlyphNames.uacute, 500 }, 
			{ PdfGlyphNames.Tcaron, 611 }, { PdfGlyphNames.partialdiff, 476 }, { PdfGlyphNames.ydieresis, 500 }, { PdfGlyphNames.Nacute, 722 }, { PdfGlyphNames.icircumflex, 278 }, 
			{ PdfGlyphNames.Ecircumflex, 611 }, { PdfGlyphNames.adieresis, 444 }, { PdfGlyphNames.edieresis, 444 }, { PdfGlyphNames.cacute, 444 }, { PdfGlyphNames.nacute, 500 }, { PdfGlyphNames.umacron, 500 }, 
			{ PdfGlyphNames.Ncaron, 722 }, { PdfGlyphNames.Iacute, 333 }, { PdfGlyphNames.plusminus, 564 }, { PdfGlyphNames.brokenbar, 200 }, { PdfGlyphNames.registered, 760 }, { PdfGlyphNames.Gbreve, 722 }, 
			{ PdfGlyphNames.Idotaccent, 333 }, { PdfGlyphNames.summation, 600 }, { PdfGlyphNames.Egrave, 611 }, { PdfGlyphNames.racute, 333 }, { PdfGlyphNames.omacron, 500 }, { PdfGlyphNames.Zacute, 611 }, 
			{ PdfGlyphNames.Zcaron, 611 }, { PdfGlyphNames.greaterequal, 549 }, { PdfGlyphNames.Eth, 722 }, { PdfGlyphNames.Ccedilla, 667 }, { PdfGlyphNames.lcommaaccent, 278 }, { PdfGlyphNames.tcaron, 326 }, 
			{ PdfGlyphNames.eogonek, 444 }, { PdfGlyphNames.Uogonek, 722 }, { PdfGlyphNames.Aacute, 722 }, { PdfGlyphNames.Adieresis, 722 }, { PdfGlyphNames.egrave, 444 }, { PdfGlyphNames.zacute, 444 }, 
			{ PdfGlyphNames.iogonek, 278 }, { PdfGlyphNames.Oacute, 722 }, { PdfGlyphNames.oacute, 500 }, { PdfGlyphNames.amacron, 444 }, { PdfGlyphNames.sacute, 389 }, { PdfGlyphNames.idieresis, 278 }, 
			{ PdfGlyphNames.Ocircumflex, 722 }, { PdfGlyphNames.Ugrave, 722 }, { PdfGlyphNames.Delta, 612 }, { PdfGlyphNames.thorn, 500 }, { PdfGlyphNames.twosuperior, 300 }, { PdfGlyphNames.Odieresis, 722 }, 
			{ PdfGlyphNames.mu, 500 }, { PdfGlyphNames.igrave, 278 }, { PdfGlyphNames.ohungarumlaut, 500 }, { PdfGlyphNames.Eogonek, 611 }, { PdfGlyphNames.dcroat, 500 }, { PdfGlyphNames.threequarters, 750 }, 
			{ PdfGlyphNames.Scedilla, 556 }, { PdfGlyphNames.lcaron, 344 }, { PdfGlyphNames.Kcommaaccent, 722 }, { PdfGlyphNames.Lacute, 611 }, { PdfGlyphNames.trademark, 980 }, { PdfGlyphNames.edotaccent, 444 }, 
			{ PdfGlyphNames.Igrave, 333 }, { PdfGlyphNames.Imacron, 333 }, { PdfGlyphNames.Lcaron, 611 }, { PdfGlyphNames.onehalf, 750 }, { PdfGlyphNames.lessequal, 549 }, { PdfGlyphNames.ocircumflex, 500 }, 
			{ PdfGlyphNames.ntilde, 500 }, { PdfGlyphNames.Uhungarumlaut, 722 }, { PdfGlyphNames.Eacute, 611 }, { PdfGlyphNames.emacron, 444 }, { PdfGlyphNames.gbreve, 500 }, { PdfGlyphNames.onequarter, 750 }, 
			{ PdfGlyphNames.Scaron, 556 }, { PdfGlyphNames.Scommaaccent, 556 }, { PdfGlyphNames.Ohungarumlaut, 722 }, { PdfGlyphNames.degree, 400 }, { PdfGlyphNames.ograve, 500 }, { PdfGlyphNames.Ccaron, 667 }, 
			{ PdfGlyphNames.ugrave, 500 }, { PdfGlyphNames.radical, 453 }, { PdfGlyphNames.Dcaron, 722 }, { PdfGlyphNames.rcommaaccent, 333 }, { PdfGlyphNames.Ntilde, 722 }, { PdfGlyphNames.otilde, 500 }, 
			{ PdfGlyphNames.Rcommaaccent, 667 }, { PdfGlyphNames.Lcommaaccent, 611 }, { PdfGlyphNames.Atilde, 722 }, { PdfGlyphNames.Aogonek, 722 }, { PdfGlyphNames.Aring, 722 }, { PdfGlyphNames.Otilde, 722 }, 
			{ PdfGlyphNames.zdotaccent, 444 }, { PdfGlyphNames.Ecaron, 611 }, { PdfGlyphNames.Iogonek, 333 }, { PdfGlyphNames.kcommaaccent, 500 }, { PdfGlyphNames.minus, 564 }, { PdfGlyphNames.Icircumflex, 333 }, 
			{ PdfGlyphNames.ncaron, 500 }, { PdfGlyphNames.tcommaaccent, 278 }, { PdfGlyphNames.logicalnot, 564 }, { PdfGlyphNames.odieresis, 500 }, { PdfGlyphNames.udieresis, 500 }, 
			{ PdfGlyphNames.notequal, 549 }, { PdfGlyphNames.gcommaaccent, 500 }, { PdfGlyphNames.eth, 500 }, { PdfGlyphNames.zcaron, 444 }, { PdfGlyphNames.ncommaaccent, 500 }, 
			{ PdfGlyphNames.onesuperior, 300 }, { PdfGlyphNames.imacron, 278 }, { PdfGlyphNames.Euro, 500 }
		};
		static readonly Dictionary<string, short> timesBoldWidths = new Dictionary<string, short>() {
			{ PdfGlyphNames._notdef, 250 }, { PdfGlyphNames.space, 250 }, { PdfGlyphNames.exclam, 333 }, { PdfGlyphNames.quotedbl, 555 }, { PdfGlyphNames.numbersign, 500 }, { PdfGlyphNames.dollar, 500 }, 
			{ PdfGlyphNames.percent, 1000 }, { PdfGlyphNames.ampersand, 833 }, { PdfGlyphNames.quoteright, 333 }, { PdfGlyphNames.parenleft, 333 }, { PdfGlyphNames.parenright, 333 }, 
			{ PdfGlyphNames.asterisk, 500 }, { PdfGlyphNames.plus, 570 }, { PdfGlyphNames.comma, 250 }, { PdfGlyphNames.hyphen, 333 }, { PdfGlyphNames.period, 250 }, { PdfGlyphNames.slash, 278 }, 
			{ PdfGlyphNames.zero, 500 }, { PdfGlyphNames.one, 500 }, { PdfGlyphNames.two, 500 }, { PdfGlyphNames.three, 500 }, { PdfGlyphNames.four, 500 }, { PdfGlyphNames.five, 500 }, { PdfGlyphNames.six, 500 }, 
			{ PdfGlyphNames.seven, 500 }, { PdfGlyphNames.eight, 500 }, { PdfGlyphNames.nine, 500 }, { PdfGlyphNames.colon, 333 }, { PdfGlyphNames.semicolon, 333 }, { PdfGlyphNames.less, 570 }, 
			{ PdfGlyphNames.equal, 570 }, { PdfGlyphNames.greater, 570 }, { PdfGlyphNames.question, 500 }, { PdfGlyphNames.at, 930 }, { PdfGlyphNames.A, 722 }, { PdfGlyphNames.B, 667 }, { PdfGlyphNames.C, 722 }, 
			{ PdfGlyphNames.D, 722 }, { PdfGlyphNames.E, 667 }, { PdfGlyphNames.F, 611 }, { PdfGlyphNames.G, 778 }, { PdfGlyphNames.H, 778 }, { PdfGlyphNames.I, 389 }, { PdfGlyphNames.J, 500 }, 
			{ PdfGlyphNames.K, 778 }, { PdfGlyphNames.L, 667 }, { PdfGlyphNames.M, 944 }, { PdfGlyphNames.N, 722 }, { PdfGlyphNames.O, 778 }, { PdfGlyphNames.P, 611 }, { PdfGlyphNames.Q, 778 }, 
			{ PdfGlyphNames.R, 722 }, { PdfGlyphNames.S, 556 }, { PdfGlyphNames.T, 667 }, { PdfGlyphNames.U, 722 }, { PdfGlyphNames.V, 722 }, { PdfGlyphNames.W, 1000 }, { PdfGlyphNames.X, 722 }, 
			{ PdfGlyphNames.Y, 722 }, { PdfGlyphNames.Z, 667 }, { PdfGlyphNames.bracketleft, 333 }, { PdfGlyphNames.backslash, 278 }, { PdfGlyphNames.bracketright, 333 }, { PdfGlyphNames.asciicircum, 581 }, 
			{ PdfGlyphNames.underscore, 500 }, { PdfGlyphNames.quoteleft, 333 }, { PdfGlyphNames.a, 500 }, { PdfGlyphNames.b, 556 }, { PdfGlyphNames.c, 444 }, { PdfGlyphNames.d, 556 }, { PdfGlyphNames.e, 444 }, 
			{ PdfGlyphNames.f, 333 }, { PdfGlyphNames.g, 500 }, { PdfGlyphNames.h, 556 }, { PdfGlyphNames.i, 278 }, { PdfGlyphNames.j, 333 }, { PdfGlyphNames.k, 556 }, { PdfGlyphNames.l, 278 }, 
			{ PdfGlyphNames.m, 833 }, { PdfGlyphNames.n, 556 }, { PdfGlyphNames.o, 500 }, { PdfGlyphNames.p, 556 }, { PdfGlyphNames.q, 556 }, { PdfGlyphNames.r, 444 }, { PdfGlyphNames.s, 389 }, 
			{ PdfGlyphNames.t, 333 }, { PdfGlyphNames.u, 556 }, { PdfGlyphNames.v, 500 }, { PdfGlyphNames.w, 722 }, { PdfGlyphNames.x, 500 }, { PdfGlyphNames.y, 500 }, { PdfGlyphNames.z, 444 }, 
			{ PdfGlyphNames.braceleft, 394 }, { PdfGlyphNames.bar, 220 }, { PdfGlyphNames.braceright, 394 }, { PdfGlyphNames.asciitilde, 520 }, { PdfGlyphNames.exclamdown, 333 }, { PdfGlyphNames.cent, 500 }, 
			{ PdfGlyphNames.sterling, 500 }, { PdfGlyphNames.fraction, 167 }, { PdfGlyphNames.yen, 500 }, { PdfGlyphNames.florin, 500 }, { PdfGlyphNames.section, 500 }, { PdfGlyphNames.currency, 500 }, 
			{ PdfGlyphNames.quotesingle, 278 }, { PdfGlyphNames.quotedblleft, 500 }, { PdfGlyphNames.guillemotleft, 500 }, { PdfGlyphNames.guilsinglleft, 333 }, { PdfGlyphNames.guilsinglright, 333 }, 
			{ PdfGlyphNames.fi, 556 }, { PdfGlyphNames.fl, 556 }, { PdfGlyphNames.endash, 500 }, { PdfGlyphNames.dagger, 500 }, { PdfGlyphNames.daggerdbl, 500 }, { PdfGlyphNames.periodcentered, 250 }, 
			{ PdfGlyphNames.paragraph, 540 }, { PdfGlyphNames.bullet, 350 }, { PdfGlyphNames.quotesinglbase, 333 }, { PdfGlyphNames.quotedblbase, 500 }, { PdfGlyphNames.quotedblright, 500 }, 
			{ PdfGlyphNames.guillemotright, 500 }, { PdfGlyphNames.ellipsis, 1000 }, { PdfGlyphNames.perthousand, 1000 }, { PdfGlyphNames.questiondown, 500 }, { PdfGlyphNames.grave, 333 }, 
			{ PdfGlyphNames.acute, 333 }, { PdfGlyphNames.circumflex, 333 }, { PdfGlyphNames.tilde, 333 }, { PdfGlyphNames.macron, 333 }, { PdfGlyphNames.breve, 333 }, { PdfGlyphNames.dotaccent, 333 }, 
			{ PdfGlyphNames.dieresis, 333 }, { PdfGlyphNames.ring, 333 }, { PdfGlyphNames.cedilla, 333 }, { PdfGlyphNames.hungarumlaut, 333 }, { PdfGlyphNames.ogonek, 333 }, { PdfGlyphNames.caron, 333 }, 
			{ PdfGlyphNames.emdash, 1000 }, { PdfGlyphNames.AE, 1000 }, { PdfGlyphNames.ordfeminine, 300 }, { PdfGlyphNames.Lslash, 667 }, { PdfGlyphNames.Oslash, 778 }, { PdfGlyphNames.OE, 1000 }, 
			{ PdfGlyphNames.ordmasculine, 330 }, { PdfGlyphNames.ae, 722 }, { PdfGlyphNames.dotlessi, 278 }, { PdfGlyphNames.lslash, 278 }, { PdfGlyphNames.oslash, 500 }, { PdfGlyphNames.oe, 722 }, 
			{ PdfGlyphNames.germandbls, 556 }, { PdfGlyphNames.Idieresis, 389 }, { PdfGlyphNames.eacute, 444 }, { PdfGlyphNames.abreve, 500 }, { PdfGlyphNames.uhungarumlaut, 556 }, { PdfGlyphNames.ecaron, 444 }, 
			{ PdfGlyphNames.Ydieresis, 722 }, { PdfGlyphNames.divide, 570 }, { PdfGlyphNames.Yacute, 722 }, { PdfGlyphNames.Acircumflex, 722 }, { PdfGlyphNames.aacute, 500 }, { PdfGlyphNames.Ucircumflex, 722 }, 
			{ PdfGlyphNames.yacute, 500 }, { PdfGlyphNames.scommaaccent, 389 }, { PdfGlyphNames.ecircumflex, 444 }, { PdfGlyphNames.Uring, 722 }, { PdfGlyphNames.Udieresis, 722 }, { PdfGlyphNames.aogonek, 500 }, 
			{ PdfGlyphNames.Uacute, 722 }, { PdfGlyphNames.uogonek, 556 }, { PdfGlyphNames.Edieresis, 667 }, { PdfGlyphNames.Dcroat, 722 }, { PdfGlyphNames.commaaccent, 250 }, { PdfGlyphNames.copyright, 747 },
			{ PdfGlyphNames.Emacron, 667 }, { PdfGlyphNames.ccaron, 444 }, { PdfGlyphNames.aring, 500 }, { PdfGlyphNames.Ncommaaccent, 722 }, { PdfGlyphNames.lacute, 278 }, { PdfGlyphNames.agrave, 500 }, 
			{ PdfGlyphNames.Tcommaaccent, 667 }, { PdfGlyphNames.Cacute, 722 }, { PdfGlyphNames.atilde, 500 }, { PdfGlyphNames.Edotaccent, 667 }, { PdfGlyphNames.scaron, 389 }, { PdfGlyphNames.scedilla, 389 }, 
			{ PdfGlyphNames.iacute, 278 }, { PdfGlyphNames.lozenge, 494 }, { PdfGlyphNames.Rcaron, 722 }, { PdfGlyphNames.Gcommaaccent, 778 }, { PdfGlyphNames.ucircumflex, 556 }, 
			{ PdfGlyphNames.acircumflex, 500 }, { PdfGlyphNames.Amacron, 722 }, { PdfGlyphNames.rcaron, 444 }, { PdfGlyphNames.ccedilla, 444 }, { PdfGlyphNames.Zdotaccent, 667 }, { PdfGlyphNames.Thorn, 611 }, 
			{ PdfGlyphNames.Omacron, 778 }, { PdfGlyphNames.Racute, 722 }, { PdfGlyphNames.Sacute, 556 }, { PdfGlyphNames.dcaron, 672 }, { PdfGlyphNames.Umacron, 722 }, { PdfGlyphNames.uring, 556 }, 
			{ PdfGlyphNames.threesuperior, 300 }, { PdfGlyphNames.Ograve, 778 }, { PdfGlyphNames.Agrave, 722 }, { PdfGlyphNames.Abreve, 722 }, { PdfGlyphNames.multiply, 570 }, { PdfGlyphNames.uacute, 556 }, 
			{ PdfGlyphNames.Tcaron, 667 }, { PdfGlyphNames.partialdiff, 494 }, { PdfGlyphNames.ydieresis, 500 }, { PdfGlyphNames.Nacute, 722 }, { PdfGlyphNames.icircumflex, 278 }, 
			{ PdfGlyphNames.Ecircumflex, 667 }, { PdfGlyphNames.adieresis, 500 }, { PdfGlyphNames.edieresis, 444 }, { PdfGlyphNames.cacute, 444 }, { PdfGlyphNames.nacute, 556 }, { PdfGlyphNames.umacron, 556 }, 
			{ PdfGlyphNames.Ncaron, 722 }, { PdfGlyphNames.Iacute, 389 }, { PdfGlyphNames.plusminus, 570 }, { PdfGlyphNames.brokenbar, 220 }, { PdfGlyphNames.registered, 747 }, { PdfGlyphNames.Gbreve, 778 }, 
			{ PdfGlyphNames.Idotaccent, 389 }, { PdfGlyphNames.summation, 600 }, { PdfGlyphNames.Egrave, 667 }, { PdfGlyphNames.racute, 444 }, { PdfGlyphNames.omacron, 500 }, { PdfGlyphNames.Zacute, 667 }, 
			{ PdfGlyphNames.Zcaron, 667 }, { PdfGlyphNames.greaterequal, 549 }, { PdfGlyphNames.Eth, 722 }, { PdfGlyphNames.Ccedilla, 722 }, { PdfGlyphNames.lcommaaccent, 278 }, { PdfGlyphNames.tcaron, 416 }, 
			{ PdfGlyphNames.eogonek, 444 }, { PdfGlyphNames.Uogonek, 722 }, { PdfGlyphNames.Aacute, 722 }, { PdfGlyphNames.Adieresis, 722 }, { PdfGlyphNames.egrave, 444 }, { PdfGlyphNames.zacute, 444 }, 
			{ PdfGlyphNames.iogonek, 278 }, { PdfGlyphNames.Oacute, 778 }, { PdfGlyphNames.oacute, 500 }, { PdfGlyphNames.amacron, 500 }, { PdfGlyphNames.sacute, 389 }, { PdfGlyphNames.idieresis, 278 }, 
			{ PdfGlyphNames.Ocircumflex, 778 }, { PdfGlyphNames.Ugrave, 722 }, { PdfGlyphNames.Delta, 612 }, { PdfGlyphNames.thorn, 556 }, { PdfGlyphNames.twosuperior, 300 }, { PdfGlyphNames.Odieresis, 778 }, 
			{ PdfGlyphNames.mu, 556 }, { PdfGlyphNames.igrave, 278 }, { PdfGlyphNames.ohungarumlaut, 500 }, { PdfGlyphNames.Eogonek, 667 }, { PdfGlyphNames.dcroat, 556 }, { PdfGlyphNames.threequarters, 750 }, 
			{ PdfGlyphNames.Scedilla, 556 }, { PdfGlyphNames.lcaron, 394 }, { PdfGlyphNames.Kcommaaccent, 778 }, { PdfGlyphNames.Lacute, 667 }, { PdfGlyphNames.trademark, 1000 }, 
			{ PdfGlyphNames.edotaccent, 444 }, { PdfGlyphNames.Igrave, 389 }, { PdfGlyphNames.Imacron, 389 }, { PdfGlyphNames.Lcaron, 667 }, { PdfGlyphNames.onehalf, 750 }, { PdfGlyphNames.lessequal, 549 }, 
			{ PdfGlyphNames.ocircumflex, 500 }, { PdfGlyphNames.ntilde, 556 }, { PdfGlyphNames.Uhungarumlaut, 722 }, { PdfGlyphNames.Eacute, 667 }, { PdfGlyphNames.emacron, 444 }, { PdfGlyphNames.gbreve, 500 }, 
			{ PdfGlyphNames.onequarter, 750 }, { PdfGlyphNames.Scaron, 556 }, { PdfGlyphNames.Scommaaccent, 556 }, { PdfGlyphNames.Ohungarumlaut, 778 }, { PdfGlyphNames.degree, 400 }, 
			{ PdfGlyphNames.ograve, 500 }, { PdfGlyphNames.Ccaron, 722 }, { PdfGlyphNames.ugrave, 556 }, { PdfGlyphNames.radical, 549 }, { PdfGlyphNames.Dcaron, 722 }, { PdfGlyphNames.rcommaaccent, 444 }, 
			{ PdfGlyphNames.Ntilde, 722 }, { PdfGlyphNames.otilde, 500 }, { PdfGlyphNames.Rcommaaccent, 722 }, { PdfGlyphNames.Lcommaaccent, 667 }, { PdfGlyphNames.Atilde, 722 }, { PdfGlyphNames.Aogonek, 722 }, 
			{ PdfGlyphNames.Aring, 722 }, { PdfGlyphNames.Otilde, 778 }, { PdfGlyphNames.zdotaccent, 444 }, { PdfGlyphNames.Ecaron, 667 }, { PdfGlyphNames.Iogonek, 389 }, { PdfGlyphNames.kcommaaccent, 556 }, 
			{ PdfGlyphNames.minus, 570 }, { PdfGlyphNames.Icircumflex, 389 }, { PdfGlyphNames.ncaron, 556 }, { PdfGlyphNames.tcommaaccent, 333 }, { PdfGlyphNames.logicalnot, 570 }, 
			{ PdfGlyphNames.odieresis, 500 }, { PdfGlyphNames.udieresis, 556 }, { PdfGlyphNames.notequal, 549 }, { PdfGlyphNames.gcommaaccent, 500 }, { PdfGlyphNames.eth, 500 }, { PdfGlyphNames.zcaron, 444 }, 
			{ PdfGlyphNames.ncommaaccent, 556 }, { PdfGlyphNames.onesuperior, 300 }, { PdfGlyphNames.imacron, 278 }, { PdfGlyphNames.Euro, 500 } 
		};
		static readonly Dictionary<string, short> timesItalicWidths = new Dictionary<string, short>() {
			{ PdfGlyphNames._notdef, 250 }, { PdfGlyphNames.space, 250 }, { PdfGlyphNames.exclam, 333 }, { PdfGlyphNames.quotedbl, 420 }, { PdfGlyphNames.numbersign, 500 }, { PdfGlyphNames.dollar, 500 }, 
			{ PdfGlyphNames.percent, 833 }, { PdfGlyphNames.ampersand, 778 }, { PdfGlyphNames.quoteright, 333 }, { PdfGlyphNames.parenleft, 333 }, { PdfGlyphNames.parenright, 333 }, 
			{ PdfGlyphNames.asterisk, 500 }, { PdfGlyphNames.plus, 675 }, { PdfGlyphNames.comma, 250 }, { PdfGlyphNames.hyphen, 333 }, { PdfGlyphNames.period, 250 }, { PdfGlyphNames.slash, 278 }, 
			{ PdfGlyphNames.zero, 500 }, { PdfGlyphNames.one, 500 }, { PdfGlyphNames.two, 500 }, { PdfGlyphNames.three, 500 }, { PdfGlyphNames.four, 500 }, { PdfGlyphNames.five, 500 }, { PdfGlyphNames.six, 500 }, 
			{ PdfGlyphNames.seven, 500 }, { PdfGlyphNames.eight, 500 }, { PdfGlyphNames.nine, 500 }, { PdfGlyphNames.colon, 333 }, { PdfGlyphNames.semicolon, 333 }, { PdfGlyphNames.less, 675 }, 
			{ PdfGlyphNames.equal, 675 }, { PdfGlyphNames.greater, 675 }, { PdfGlyphNames.question, 500 }, { PdfGlyphNames.at, 920 }, { PdfGlyphNames.A, 611 }, { PdfGlyphNames.B, 611 }, { PdfGlyphNames.C, 667 }, 
			{ PdfGlyphNames.D, 722 }, { PdfGlyphNames.E, 611 }, { PdfGlyphNames.F, 611 }, { PdfGlyphNames.G, 722 }, { PdfGlyphNames.H, 722 }, { PdfGlyphNames.I, 333 }, { PdfGlyphNames.J, 444 }, 
			{ PdfGlyphNames.K, 667 }, { PdfGlyphNames.L, 556 }, { PdfGlyphNames.M, 833 }, { PdfGlyphNames.N, 667 }, { PdfGlyphNames.O, 722 }, { PdfGlyphNames.P, 611 }, { PdfGlyphNames.Q, 722 }, 
			{ PdfGlyphNames.R, 611 }, { PdfGlyphNames.S, 500 }, { PdfGlyphNames.T, 556 }, { PdfGlyphNames.U, 722 }, { PdfGlyphNames.V, 611 }, { PdfGlyphNames.W, 833 }, { PdfGlyphNames.X, 611 }, 
			{ PdfGlyphNames.Y, 556 }, { PdfGlyphNames.Z, 556 }, { PdfGlyphNames.bracketleft, 389 }, { PdfGlyphNames.backslash, 278 }, { PdfGlyphNames.bracketright, 389 }, { PdfGlyphNames.asciicircum, 422 }, 
			{ PdfGlyphNames.underscore, 500 }, { PdfGlyphNames.quoteleft, 333 }, { PdfGlyphNames.a, 500 }, { PdfGlyphNames.b, 500 }, { PdfGlyphNames.c, 444 }, { PdfGlyphNames.d, 500 }, { PdfGlyphNames.e, 444 }, 
			{ PdfGlyphNames.f, 278 }, { PdfGlyphNames.g, 500 }, { PdfGlyphNames.h, 500 }, { PdfGlyphNames.i, 278 }, { PdfGlyphNames.j, 278 }, { PdfGlyphNames.k, 444 }, { PdfGlyphNames.l, 278 }, 
			{ PdfGlyphNames.m, 722 }, { PdfGlyphNames.n, 500 }, { PdfGlyphNames.o, 500 }, { PdfGlyphNames.p, 500 }, { PdfGlyphNames.q, 500 }, { PdfGlyphNames.r, 389 }, { PdfGlyphNames.s, 389 }, 
			{ PdfGlyphNames.t, 278 }, { PdfGlyphNames.u, 500 }, { PdfGlyphNames.v, 444 }, { PdfGlyphNames.w, 667 }, { PdfGlyphNames.x, 444 }, { PdfGlyphNames.y, 444 }, { PdfGlyphNames.z, 389 }, 
			{ PdfGlyphNames.braceleft, 400 }, { PdfGlyphNames.bar, 275 }, { PdfGlyphNames.braceright, 400 }, { PdfGlyphNames.asciitilde, 541 }, { PdfGlyphNames.exclamdown, 389 }, { PdfGlyphNames.cent, 500 }, 
			{ PdfGlyphNames.sterling, 500 }, { PdfGlyphNames.fraction, 167 }, { PdfGlyphNames.yen, 500 }, { PdfGlyphNames.florin, 500 }, { PdfGlyphNames.section, 500 }, { PdfGlyphNames.currency, 500 }, 
			{ PdfGlyphNames.quotesingle, 214 }, { PdfGlyphNames.quotedblleft, 556 }, { PdfGlyphNames.guillemotleft, 500 }, { PdfGlyphNames.guilsinglleft, 333 }, { PdfGlyphNames.guilsinglright, 333 }, 
			{ PdfGlyphNames.fi, 500 }, { PdfGlyphNames.fl, 500 }, { PdfGlyphNames.endash, 500 }, { PdfGlyphNames.dagger, 500 }, { PdfGlyphNames.daggerdbl, 500 }, { PdfGlyphNames.periodcentered, 250 }, 
			{ PdfGlyphNames.paragraph, 523 }, { PdfGlyphNames.bullet, 350 }, { PdfGlyphNames.quotesinglbase, 333 }, { PdfGlyphNames.quotedblbase, 556 }, { PdfGlyphNames.quotedblright, 556 }, 
			{ PdfGlyphNames.guillemotright, 500 }, { PdfGlyphNames.ellipsis, 889 }, { PdfGlyphNames.perthousand, 1000 }, { PdfGlyphNames.questiondown, 500 }, { PdfGlyphNames.grave, 333 }, 
			{ PdfGlyphNames.acute, 333 }, { PdfGlyphNames.circumflex, 333 }, { PdfGlyphNames.tilde, 333 }, { PdfGlyphNames.macron, 333 }, { PdfGlyphNames.breve, 333 }, { PdfGlyphNames.dotaccent, 333 }, 
			{ PdfGlyphNames.dieresis, 333 }, { PdfGlyphNames.ring, 333 }, { PdfGlyphNames.cedilla, 333 }, { PdfGlyphNames.hungarumlaut, 333 }, { PdfGlyphNames.ogonek, 333 }, { PdfGlyphNames.caron, 333 }, 
			{ PdfGlyphNames.emdash, 889 }, { PdfGlyphNames.AE, 889 }, { PdfGlyphNames.ordfeminine, 276 }, { PdfGlyphNames.Lslash, 556 }, { PdfGlyphNames.Oslash, 722 }, { PdfGlyphNames.OE, 944 }, 
			{ PdfGlyphNames.ordmasculine, 310 }, { PdfGlyphNames.ae, 667 }, { PdfGlyphNames.dotlessi, 278 }, { PdfGlyphNames.lslash, 278 }, { PdfGlyphNames.oslash, 500 }, { PdfGlyphNames.oe, 667 }, 
			{ PdfGlyphNames.germandbls, 500 }, { PdfGlyphNames.Idieresis, 333 }, { PdfGlyphNames.eacute, 444 }, { PdfGlyphNames.abreve, 500 }, { PdfGlyphNames.uhungarumlaut, 500 }, { PdfGlyphNames.ecaron, 444 }, 
			{ PdfGlyphNames.Ydieresis, 556 }, { PdfGlyphNames.divide, 675 }, { PdfGlyphNames.Yacute, 556 }, { PdfGlyphNames.Acircumflex, 611 }, { PdfGlyphNames.aacute, 500 }, { PdfGlyphNames.Ucircumflex, 722 }, 
			{ PdfGlyphNames.yacute, 444 }, { PdfGlyphNames.scommaaccent, 389 }, { PdfGlyphNames.ecircumflex, 444 }, { PdfGlyphNames.Uring, 722 }, { PdfGlyphNames.Udieresis, 722 }, { PdfGlyphNames.aogonek, 500 }, 
			{ PdfGlyphNames.Uacute, 722 }, { PdfGlyphNames.uogonek, 500 }, { PdfGlyphNames.Edieresis, 611 }, { PdfGlyphNames.Dcroat, 722 }, { PdfGlyphNames.commaaccent, 250 }, { PdfGlyphNames.copyright, 760 }, 
			{ PdfGlyphNames.Emacron, 611 }, { PdfGlyphNames.ccaron, 444 }, { PdfGlyphNames.aring, 500 }, { PdfGlyphNames.Ncommaaccent, 667 }, { PdfGlyphNames.lacute, 278 }, { PdfGlyphNames.agrave, 500 }, 
			{ PdfGlyphNames.Tcommaaccent, 556 }, { PdfGlyphNames.Cacute, 667 }, { PdfGlyphNames.atilde, 500 }, { PdfGlyphNames.Edotaccent, 611 }, { PdfGlyphNames.scaron, 389 }, { PdfGlyphNames.scedilla, 389 }, 
			{ PdfGlyphNames.iacute, 278 }, { PdfGlyphNames.lozenge, 471 }, { PdfGlyphNames.Rcaron, 611 }, { PdfGlyphNames.Gcommaaccent, 722 }, { PdfGlyphNames.ucircumflex, 500 }, 
			{ PdfGlyphNames.acircumflex, 500 }, { PdfGlyphNames.Amacron, 611 }, { PdfGlyphNames.rcaron, 389 }, { PdfGlyphNames.ccedilla, 444 }, { PdfGlyphNames.Zdotaccent, 556 }, { PdfGlyphNames.Thorn, 611 }, 
			{ PdfGlyphNames.Omacron, 722 }, { PdfGlyphNames.Racute, 611 }, { PdfGlyphNames.Sacute, 500 }, { PdfGlyphNames.dcaron, 544 }, { PdfGlyphNames.Umacron, 722 }, { PdfGlyphNames.uring, 500 }, 
			{ PdfGlyphNames.threesuperior, 300 }, { PdfGlyphNames.Ograve, 722 }, { PdfGlyphNames.Agrave, 611 }, { PdfGlyphNames.Abreve, 611 }, { PdfGlyphNames.multiply, 675 }, { PdfGlyphNames.uacute, 500 }, 
			{ PdfGlyphNames.Tcaron, 556 }, { PdfGlyphNames.partialdiff, 476 }, { PdfGlyphNames.ydieresis, 444 }, { PdfGlyphNames.Nacute, 667 }, { PdfGlyphNames.icircumflex, 278 }, 
			{ PdfGlyphNames.Ecircumflex, 611 }, { PdfGlyphNames.adieresis, 500 }, { PdfGlyphNames.edieresis, 444 }, { PdfGlyphNames.cacute, 444 }, { PdfGlyphNames.nacute, 500 }, { PdfGlyphNames.umacron, 500 }, 
			{ PdfGlyphNames.Ncaron, 667 }, { PdfGlyphNames.Iacute, 333 }, { PdfGlyphNames.plusminus, 675 }, { PdfGlyphNames.brokenbar, 275 }, { PdfGlyphNames.registered, 760 }, { PdfGlyphNames.Gbreve, 722 }, 
			{ PdfGlyphNames.Idotaccent, 333 }, { PdfGlyphNames.summation, 600 }, { PdfGlyphNames.Egrave, 611 }, { PdfGlyphNames.racute, 389 }, { PdfGlyphNames.omacron, 500 }, { PdfGlyphNames.Zacute, 556 }, 
			{ PdfGlyphNames.Zcaron, 556 }, { PdfGlyphNames.greaterequal, 549 }, { PdfGlyphNames.Eth, 722 }, { PdfGlyphNames.Ccedilla, 667 }, { PdfGlyphNames.lcommaaccent, 278 }, { PdfGlyphNames.tcaron, 300 }, 
			{ PdfGlyphNames.eogonek, 444 }, { PdfGlyphNames.Uogonek, 722 }, { PdfGlyphNames.Aacute, 611 }, { PdfGlyphNames.Adieresis, 611 }, { PdfGlyphNames.egrave, 444 }, { PdfGlyphNames.zacute, 389 }, 
			{ PdfGlyphNames.iogonek, 278 }, { PdfGlyphNames.Oacute, 722 }, { PdfGlyphNames.oacute, 500 }, { PdfGlyphNames.amacron, 500 }, { PdfGlyphNames.sacute, 389 }, { PdfGlyphNames.idieresis, 278 }, 
			{ PdfGlyphNames.Ocircumflex, 722 }, { PdfGlyphNames.Ugrave, 722 }, { PdfGlyphNames.Delta, 612 }, { PdfGlyphNames.thorn, 500 }, { PdfGlyphNames.twosuperior, 300 }, { PdfGlyphNames.Odieresis, 722 }, 
			{ PdfGlyphNames.mu, 500 }, { PdfGlyphNames.igrave, 278 }, { PdfGlyphNames.ohungarumlaut, 500 }, { PdfGlyphNames.Eogonek, 611 }, { PdfGlyphNames.dcroat, 500 }, { PdfGlyphNames.threequarters, 750 }, 
			{ PdfGlyphNames.Scedilla, 500 }, { PdfGlyphNames.lcaron, 300 }, { PdfGlyphNames.Kcommaaccent, 667 }, { PdfGlyphNames.Lacute, 556 }, { PdfGlyphNames.trademark, 980 }, { PdfGlyphNames.edotaccent, 444 }, 
			{ PdfGlyphNames.Igrave, 333 }, { PdfGlyphNames.Imacron, 333 }, { PdfGlyphNames.Lcaron, 611 }, { PdfGlyphNames.onehalf, 750 }, { PdfGlyphNames.lessequal, 549 }, { PdfGlyphNames.ocircumflex, 500 }, 
			{ PdfGlyphNames.ntilde, 500 }, { PdfGlyphNames.Uhungarumlaut, 722 }, { PdfGlyphNames.Eacute, 611 }, { PdfGlyphNames.emacron, 444 }, { PdfGlyphNames.gbreve, 500 }, { PdfGlyphNames.onequarter, 750 }, 
			{ PdfGlyphNames.Scaron, 500 }, { PdfGlyphNames.Scommaaccent, 500 }, { PdfGlyphNames.Ohungarumlaut, 722 }, { PdfGlyphNames.degree, 400 }, { PdfGlyphNames.ograve, 500 }, { PdfGlyphNames.Ccaron, 667 }, 
			{ PdfGlyphNames.ugrave, 500 }, { PdfGlyphNames.radical, 453 }, { PdfGlyphNames.Dcaron, 722 }, { PdfGlyphNames.rcommaaccent, 389 }, { PdfGlyphNames.Ntilde, 667 }, { PdfGlyphNames.otilde, 500 }, 
			{ PdfGlyphNames.Rcommaaccent, 611 }, { PdfGlyphNames.Lcommaaccent, 556 }, { PdfGlyphNames.Atilde, 611 }, { PdfGlyphNames.Aogonek, 611 }, { PdfGlyphNames.Aring, 611 }, { PdfGlyphNames.Otilde, 722 }, 
			{ PdfGlyphNames.zdotaccent, 389 }, { PdfGlyphNames.Ecaron, 611 }, { PdfGlyphNames.Iogonek, 333 }, { PdfGlyphNames.kcommaaccent, 444 }, { PdfGlyphNames.minus, 675 }, { PdfGlyphNames.Icircumflex, 333 }, 
			{ PdfGlyphNames.ncaron, 500 }, { PdfGlyphNames.tcommaaccent, 278 }, { PdfGlyphNames.logicalnot, 675 }, { PdfGlyphNames.odieresis, 500 }, { PdfGlyphNames.udieresis, 500 }, 
			{ PdfGlyphNames.notequal, 549 }, { PdfGlyphNames.gcommaaccent, 500 }, { PdfGlyphNames.eth, 500 }, { PdfGlyphNames.zcaron, 389 }, { PdfGlyphNames.ncommaaccent, 500 }, 
			{ PdfGlyphNames.onesuperior, 300 }, { PdfGlyphNames.imacron, 278 }, { PdfGlyphNames.Euro, 500 } 
		};
		static readonly Dictionary<string, short> timesBoldItalicWidths = new Dictionary<string, short>() {
			{ PdfGlyphNames._notdef, 250 }, { PdfGlyphNames.space, 250 }, { PdfGlyphNames.exclam, 389 }, { PdfGlyphNames.quotedbl, 555 }, { PdfGlyphNames.numbersign, 500 }, { PdfGlyphNames.dollar, 500 }, 
			{ PdfGlyphNames.percent, 833 }, { PdfGlyphNames.ampersand, 778 }, { PdfGlyphNames.quoteright, 333 }, { PdfGlyphNames.parenleft, 333 }, { PdfGlyphNames.parenright, 333 }, 
			{ PdfGlyphNames.asterisk, 500 }, { PdfGlyphNames.plus, 570 }, { PdfGlyphNames.comma, 250 }, { PdfGlyphNames.hyphen, 333 }, { PdfGlyphNames.period, 250 }, { PdfGlyphNames.slash, 278 }, 
			{ PdfGlyphNames.zero, 500 }, { PdfGlyphNames.one, 500 }, { PdfGlyphNames.two, 500 }, { PdfGlyphNames.three, 500 }, { PdfGlyphNames.four, 500 }, { PdfGlyphNames.five, 500 }, { PdfGlyphNames.six, 500 }, 
			{ PdfGlyphNames.seven, 500 }, { PdfGlyphNames.eight, 500 }, { PdfGlyphNames.nine, 500 }, { PdfGlyphNames.colon, 333 }, { PdfGlyphNames.semicolon, 333 }, { PdfGlyphNames.less, 570 }, 
			{ PdfGlyphNames.equal, 570 }, { PdfGlyphNames.greater, 570 }, { PdfGlyphNames.question, 500 }, { PdfGlyphNames.at, 832 }, { PdfGlyphNames.A, 667 }, { PdfGlyphNames.B, 667 }, { PdfGlyphNames.C, 667 }, 
			{ PdfGlyphNames.D, 722 }, { PdfGlyphNames.E, 667 }, { PdfGlyphNames.F, 667 }, { PdfGlyphNames.G, 722 }, { PdfGlyphNames.H, 778 }, { PdfGlyphNames.I, 389 }, { PdfGlyphNames.J, 500 }, 
			{ PdfGlyphNames.K, 667 }, { PdfGlyphNames.L, 611 }, { PdfGlyphNames.M, 889 }, { PdfGlyphNames.N, 722 }, { PdfGlyphNames.O, 722 }, { PdfGlyphNames.P, 611 }, { PdfGlyphNames.Q, 722 }, 
			{ PdfGlyphNames.R, 667 }, { PdfGlyphNames.S, 556 }, { PdfGlyphNames.T, 611 }, { PdfGlyphNames.U, 722 }, { PdfGlyphNames.V, 667 }, { PdfGlyphNames.W, 889 }, { PdfGlyphNames.X, 667 }, 
			{ PdfGlyphNames.Y, 611 }, { PdfGlyphNames.Z, 611 }, { PdfGlyphNames.bracketleft, 333 }, { PdfGlyphNames.backslash, 278 }, { PdfGlyphNames.bracketright, 333 }, { PdfGlyphNames.asciicircum, 570 }, 
			{ PdfGlyphNames.underscore, 500 }, { PdfGlyphNames.quoteleft, 333 }, { PdfGlyphNames.a, 500 }, { PdfGlyphNames.b, 500 }, { PdfGlyphNames.c, 444 }, { PdfGlyphNames.d, 500 }, { PdfGlyphNames.e, 444 }, 
			{ PdfGlyphNames.f, 333 }, { PdfGlyphNames.g, 500 }, { PdfGlyphNames.h, 556 }, { PdfGlyphNames.i, 278 }, { PdfGlyphNames.j, 278 }, { PdfGlyphNames.k, 500 }, { PdfGlyphNames.l, 278 }, 
			{ PdfGlyphNames.m, 778 }, { PdfGlyphNames.n, 556 }, { PdfGlyphNames.o, 500 }, { PdfGlyphNames.p, 500 }, { PdfGlyphNames.q, 500 }, { PdfGlyphNames.r, 389 }, { PdfGlyphNames.s, 389 }, 
			{ PdfGlyphNames.t, 278 }, { PdfGlyphNames.u, 556 }, { PdfGlyphNames.v, 444 }, { PdfGlyphNames.w, 667 }, { PdfGlyphNames.x, 500 }, { PdfGlyphNames.y, 444 }, { PdfGlyphNames.z, 389 }, 
			{ PdfGlyphNames.braceleft, 348 }, { PdfGlyphNames.bar, 220 }, { PdfGlyphNames.braceright, 348 }, { PdfGlyphNames.asciitilde, 570 }, { PdfGlyphNames.exclamdown, 389 }, { PdfGlyphNames.cent, 500 }, 
			{ PdfGlyphNames.sterling, 500 }, { PdfGlyphNames.fraction, 167 }, { PdfGlyphNames.yen, 500 }, { PdfGlyphNames.florin, 500 }, { PdfGlyphNames.section, 500 }, { PdfGlyphNames.currency, 500 }, 
			{ PdfGlyphNames.quotesingle, 278 }, { PdfGlyphNames.quotedblleft, 500 }, { PdfGlyphNames.guillemotleft, 500 }, { PdfGlyphNames.guilsinglleft, 333 }, { PdfGlyphNames.guilsinglright, 333 }, 
			{ PdfGlyphNames.fi, 556 }, { PdfGlyphNames.fl, 556 }, { PdfGlyphNames.endash, 500 }, { PdfGlyphNames.dagger, 500 }, { PdfGlyphNames.daggerdbl, 500 }, { PdfGlyphNames.periodcentered, 250 }, 
			{ PdfGlyphNames.paragraph, 500 }, { PdfGlyphNames.bullet, 350 }, { PdfGlyphNames.quotesinglbase, 333 }, { PdfGlyphNames.quotedblbase, 500 }, { PdfGlyphNames.quotedblright, 500 }, 
			{ PdfGlyphNames.guillemotright, 500 }, { PdfGlyphNames.ellipsis, 1000 }, { PdfGlyphNames.perthousand, 1000 }, { PdfGlyphNames.questiondown, 500 }, { PdfGlyphNames.grave, 333 }, 
			{ PdfGlyphNames.acute, 333 }, { PdfGlyphNames.circumflex, 333 }, { PdfGlyphNames.tilde, 333 }, { PdfGlyphNames.macron, 333 }, { PdfGlyphNames.breve, 333 }, { PdfGlyphNames.dotaccent, 333 }, 
			{ PdfGlyphNames.dieresis, 333 }, { PdfGlyphNames.ring, 333 }, { PdfGlyphNames.cedilla, 333 }, { PdfGlyphNames.hungarumlaut, 333 }, { PdfGlyphNames.ogonek, 333 }, { PdfGlyphNames.caron, 333 }, 
			{ PdfGlyphNames.emdash, 1000 }, { PdfGlyphNames.AE, 944 }, { PdfGlyphNames.ordfeminine, 266 }, { PdfGlyphNames.Lslash, 611 }, { PdfGlyphNames.Oslash, 722 }, { PdfGlyphNames.OE, 944 }, 
			{ PdfGlyphNames.ordmasculine, 300 }, { PdfGlyphNames.ae, 722 }, { PdfGlyphNames.dotlessi, 278 }, { PdfGlyphNames.lslash, 278 }, { PdfGlyphNames.oslash, 500 }, { PdfGlyphNames.oe, 722 }, 
			{ PdfGlyphNames.germandbls, 500 }, { PdfGlyphNames.Idieresis, 389 }, { PdfGlyphNames.eacute, 444 }, { PdfGlyphNames.abreve, 500 }, { PdfGlyphNames.uhungarumlaut, 556 }, { PdfGlyphNames.ecaron, 444 }, 
			{ PdfGlyphNames.Ydieresis, 611 }, { PdfGlyphNames.divide, 570 }, { PdfGlyphNames.Yacute, 611 }, { PdfGlyphNames.Acircumflex, 667 }, { PdfGlyphNames.aacute, 500 }, { PdfGlyphNames.Ucircumflex, 722 }, 
			{ PdfGlyphNames.yacute, 444 }, { PdfGlyphNames.scommaaccent, 389 }, { PdfGlyphNames.ecircumflex, 444 }, { PdfGlyphNames.Uring, 722 }, { PdfGlyphNames.Udieresis, 722 }, { PdfGlyphNames.aogonek, 500 }, 
			{ PdfGlyphNames.Uacute, 722 }, { PdfGlyphNames.uogonek, 556 }, { PdfGlyphNames.Edieresis, 667 }, { PdfGlyphNames.Dcroat, 722 }, { PdfGlyphNames.commaaccent, 250 }, { PdfGlyphNames.copyright, 747 }, 
			{ PdfGlyphNames.Emacron, 667 }, { PdfGlyphNames.ccaron, 444 }, { PdfGlyphNames.aring, 500 }, { PdfGlyphNames.Ncommaaccent, 722 }, { PdfGlyphNames.lacute, 278 }, { PdfGlyphNames.agrave, 500 }, 
			{ PdfGlyphNames.Tcommaaccent, 611 }, { PdfGlyphNames.Cacute, 667 }, { PdfGlyphNames.atilde, 500 }, { PdfGlyphNames.Edotaccent, 667 }, { PdfGlyphNames.scaron, 389 }, { PdfGlyphNames.scedilla, 389 }, 
			{ PdfGlyphNames.iacute, 278 }, { PdfGlyphNames.lozenge, 494 }, { PdfGlyphNames.Rcaron, 667 }, { PdfGlyphNames.Gcommaaccent, 722 }, { PdfGlyphNames.ucircumflex, 556 }, 
			{ PdfGlyphNames.acircumflex, 500 }, { PdfGlyphNames.Amacron, 667 }, { PdfGlyphNames.rcaron, 389 }, { PdfGlyphNames.ccedilla, 444 }, { PdfGlyphNames.Zdotaccent, 611 }, { PdfGlyphNames.Thorn, 611 }, 
			{ PdfGlyphNames.Omacron, 722 }, { PdfGlyphNames.Racute, 667 }, { PdfGlyphNames.Sacute, 556 }, { PdfGlyphNames.dcaron, 608 }, { PdfGlyphNames.Umacron, 722 }, { PdfGlyphNames.uring, 556 }, 
			{ PdfGlyphNames.threesuperior, 300 }, { PdfGlyphNames.Ograve, 722 }, { PdfGlyphNames.Agrave, 667 }, { PdfGlyphNames.Abreve, 667 }, { PdfGlyphNames.multiply, 570 }, { PdfGlyphNames.uacute, 556 }, 
			{ PdfGlyphNames.Tcaron, 611 }, { PdfGlyphNames.partialdiff, 494 }, { PdfGlyphNames.ydieresis, 444 }, { PdfGlyphNames.Nacute, 722 }, { PdfGlyphNames.icircumflex, 278 }, 
			{ PdfGlyphNames.Ecircumflex, 667 }, { PdfGlyphNames.adieresis, 500 }, { PdfGlyphNames.edieresis, 444 }, { PdfGlyphNames.cacute, 444 }, { PdfGlyphNames.nacute, 556 }, { PdfGlyphNames.umacron, 556 }, 
			{ PdfGlyphNames.Ncaron, 722 }, { PdfGlyphNames.Iacute, 389 }, { PdfGlyphNames.plusminus, 570 }, { PdfGlyphNames.brokenbar, 220 }, { PdfGlyphNames.registered, 747 }, { PdfGlyphNames.Gbreve, 722 }, 
			{ PdfGlyphNames.Idotaccent, 389 }, { PdfGlyphNames.summation, 600 }, { PdfGlyphNames.Egrave, 667 }, { PdfGlyphNames.racute, 389 }, { PdfGlyphNames.omacron, 500 }, { PdfGlyphNames.Zacute, 611 }, 
			{ PdfGlyphNames.Zcaron, 611 }, { PdfGlyphNames.greaterequal, 549 }, { PdfGlyphNames.Eth, 722 }, { PdfGlyphNames.Ccedilla, 667 }, { PdfGlyphNames.lcommaaccent, 278 }, { PdfGlyphNames.tcaron, 366 }, 
			{ PdfGlyphNames.eogonek, 444 }, { PdfGlyphNames.Uogonek, 722 }, { PdfGlyphNames.Aacute, 667 }, { PdfGlyphNames.Adieresis, 667 }, { PdfGlyphNames.egrave, 444 }, { PdfGlyphNames.zacute, 389 }, 
			{ PdfGlyphNames.iogonek, 278 }, { PdfGlyphNames.Oacute, 722 }, { PdfGlyphNames.oacute, 500 }, { PdfGlyphNames.amacron, 500 }, { PdfGlyphNames.sacute, 389 }, { PdfGlyphNames.idieresis, 278 }, 
			{ PdfGlyphNames.Ocircumflex, 722 }, { PdfGlyphNames.Ugrave, 722 }, { PdfGlyphNames.Delta, 612 }, { PdfGlyphNames.thorn, 500 }, { PdfGlyphNames.twosuperior, 300 }, { PdfGlyphNames.Odieresis, 722 }, 
			{ PdfGlyphNames.mu, 576 }, { PdfGlyphNames.igrave, 278 }, { PdfGlyphNames.ohungarumlaut, 500 }, { PdfGlyphNames.Eogonek, 667 }, { PdfGlyphNames.dcroat, 500 }, { PdfGlyphNames.threequarters, 750 }, 
			{ PdfGlyphNames.Scedilla, 556 }, { PdfGlyphNames.lcaron, 382 }, { PdfGlyphNames.Kcommaaccent, 667 }, { PdfGlyphNames.Lacute, 611 }, { PdfGlyphNames.trademark, 1000 }, 
			{ PdfGlyphNames.edotaccent, 444 }, { PdfGlyphNames.Igrave, 389 }, { PdfGlyphNames.Imacron, 389 }, { PdfGlyphNames.Lcaron, 611 }, { PdfGlyphNames.onehalf, 750 }, { PdfGlyphNames.lessequal, 549 }, 
			{ PdfGlyphNames.ocircumflex, 500 }, { PdfGlyphNames.ntilde, 556 }, { PdfGlyphNames.Uhungarumlaut, 722 }, { PdfGlyphNames.Eacute, 667 }, { PdfGlyphNames.emacron, 444 }, { PdfGlyphNames.gbreve, 500 }, 
			{ PdfGlyphNames.onequarter, 750 }, { PdfGlyphNames.Scaron, 556 }, { PdfGlyphNames.Scommaaccent, 556 }, { PdfGlyphNames.Ohungarumlaut, 722 }, { PdfGlyphNames.degree, 400 }, 
			{ PdfGlyphNames.ograve, 500 }, { PdfGlyphNames.Ccaron, 667 }, { PdfGlyphNames.ugrave, 556 }, { PdfGlyphNames.radical, 549 }, { PdfGlyphNames.Dcaron, 722 }, { PdfGlyphNames.rcommaaccent, 389 }, 
			{ PdfGlyphNames.Ntilde, 722 }, { PdfGlyphNames.otilde, 500 }, { PdfGlyphNames.Rcommaaccent, 667 }, { PdfGlyphNames.Lcommaaccent, 611 }, { PdfGlyphNames.Atilde, 667 }, { PdfGlyphNames.Aogonek, 667 }, 
			{ PdfGlyphNames.Aring, 667 }, { PdfGlyphNames.Otilde, 722 }, { PdfGlyphNames.zdotaccent, 389 }, { PdfGlyphNames.Ecaron, 667 }, { PdfGlyphNames.Iogonek, 389 }, { PdfGlyphNames.kcommaaccent, 500 }, 
			{ PdfGlyphNames.minus, 606 }, { PdfGlyphNames.Icircumflex, 389 }, { PdfGlyphNames.ncaron, 556 }, { PdfGlyphNames.tcommaaccent, 278 }, { PdfGlyphNames.logicalnot, 606 }, 
			{ PdfGlyphNames.odieresis, 500 }, { PdfGlyphNames.udieresis, 556 }, { PdfGlyphNames.notequal, 549 }, { PdfGlyphNames.gcommaaccent, 500 }, { PdfGlyphNames.eth, 500 }, { PdfGlyphNames.zcaron, 389 }, 
			{ PdfGlyphNames.ncommaaccent, 556 }, { PdfGlyphNames.onesuperior, 300 }, { PdfGlyphNames.imacron, 278 }, { PdfGlyphNames.Euro, 500 } 
		};
		static readonly Dictionary<string, short> helveticaWidths = new Dictionary<string, short>() {
			{ PdfGlyphNames._notdef, 278 }, { PdfGlyphNames.space, 278 }, { PdfGlyphNames.exclam, 278 }, { PdfGlyphNames.quotedbl, 355 }, { PdfGlyphNames.numbersign, 556 }, { PdfGlyphNames.dollar, 556 }, 
			{ PdfGlyphNames.percent, 889 }, { PdfGlyphNames.ampersand, 667 }, { PdfGlyphNames.quoteright, 222 }, { PdfGlyphNames.parenleft, 333 }, { PdfGlyphNames.parenright, 333 },
			{ PdfGlyphNames.asterisk, 389 }, { PdfGlyphNames.plus, 584 }, { PdfGlyphNames.comma, 278 }, { PdfGlyphNames.hyphen, 333 }, { PdfGlyphNames.period, 278 }, { PdfGlyphNames.slash, 278 }, 
			{ PdfGlyphNames.zero, 556 }, { PdfGlyphNames.one, 556 }, { PdfGlyphNames.two, 556 }, { PdfGlyphNames.three, 556 }, { PdfGlyphNames.four, 556 }, { PdfGlyphNames.five, 556 }, { PdfGlyphNames.six, 556 }, 
			{ PdfGlyphNames.seven, 556 }, { PdfGlyphNames.eight, 556 }, { PdfGlyphNames.nine, 556 }, { PdfGlyphNames.colon, 278 }, { PdfGlyphNames.semicolon, 278 }, { PdfGlyphNames.less, 584 }, 
			{ PdfGlyphNames.equal, 584 }, { PdfGlyphNames.greater, 584 }, { PdfGlyphNames.question, 556 }, { PdfGlyphNames.at, 1015 }, { PdfGlyphNames.A, 667 }, { PdfGlyphNames.B, 667 }, { PdfGlyphNames.C, 722 }, 
			{ PdfGlyphNames.D, 722 }, { PdfGlyphNames.E, 667 }, { PdfGlyphNames.F, 611 }, { PdfGlyphNames.G, 778 }, { PdfGlyphNames.H, 722 }, { PdfGlyphNames.I, 278 }, { PdfGlyphNames.J, 500 }, 
			{ PdfGlyphNames.K, 667 }, { PdfGlyphNames.L, 556 }, { PdfGlyphNames.M, 833 }, { PdfGlyphNames.N, 722 }, { PdfGlyphNames.O, 778 }, { PdfGlyphNames.P, 667 }, { PdfGlyphNames.Q, 778 }, 
			{ PdfGlyphNames.R, 722 }, { PdfGlyphNames.S, 667 }, { PdfGlyphNames.T, 611 }, { PdfGlyphNames.U, 722 }, { PdfGlyphNames.V, 667 }, { PdfGlyphNames.W, 944 }, { PdfGlyphNames.X, 667 }, 
			{ PdfGlyphNames.Y, 667 }, { PdfGlyphNames.Z, 611 }, { PdfGlyphNames.bracketleft, 278 }, { PdfGlyphNames.backslash, 278 }, { PdfGlyphNames.bracketright, 278 }, { PdfGlyphNames.asciicircum, 469 }, 
			{ PdfGlyphNames.underscore, 556 }, { PdfGlyphNames.quoteleft, 222 }, { PdfGlyphNames.a, 556 }, { PdfGlyphNames.b, 556 }, { PdfGlyphNames.c, 500 }, { PdfGlyphNames.d, 556 }, { PdfGlyphNames.e, 556 }, 
			{ PdfGlyphNames.f, 278 }, { PdfGlyphNames.g, 556 }, { PdfGlyphNames.h, 556 }, { PdfGlyphNames.i, 222 }, { PdfGlyphNames.j, 222 }, { PdfGlyphNames.k, 500 }, { PdfGlyphNames.l, 222 }, 
			{ PdfGlyphNames.m, 833 }, { PdfGlyphNames.n, 556 }, { PdfGlyphNames.o, 556 }, { PdfGlyphNames.p, 556 }, { PdfGlyphNames.q, 556 }, { PdfGlyphNames.r, 333 }, { PdfGlyphNames.s, 500 }, 
			{ PdfGlyphNames.t, 278 }, { PdfGlyphNames.u, 556 }, { PdfGlyphNames.v, 500 }, { PdfGlyphNames.w, 722 }, { PdfGlyphNames.x, 500 }, { PdfGlyphNames.y, 500 }, { PdfGlyphNames.z, 500 }, 
			{ PdfGlyphNames.braceleft, 334 }, { PdfGlyphNames.bar, 260 }, { PdfGlyphNames.braceright, 334 }, { PdfGlyphNames.asciitilde, 584 }, { PdfGlyphNames.exclamdown, 333 }, { PdfGlyphNames.cent, 556 }, 
			{ PdfGlyphNames.sterling, 556 }, { PdfGlyphNames.fraction, 167 }, { PdfGlyphNames.yen, 556 }, { PdfGlyphNames.florin, 556 }, { PdfGlyphNames.section, 556 }, { PdfGlyphNames.currency, 556 }, 
			{ PdfGlyphNames.quotesingle, 191 }, { PdfGlyphNames.quotedblleft, 333 }, { PdfGlyphNames.guillemotleft, 556 }, { PdfGlyphNames.guilsinglleft, 333 }, { PdfGlyphNames.guilsinglright, 333 }, 
			{ PdfGlyphNames.fi, 500 }, { PdfGlyphNames.fl, 500 }, { PdfGlyphNames.endash, 556 }, { PdfGlyphNames.dagger, 556 }, { PdfGlyphNames.daggerdbl, 556 }, { PdfGlyphNames.periodcentered, 278 }, 
			{ PdfGlyphNames.paragraph, 537 }, { PdfGlyphNames.bullet, 350 }, { PdfGlyphNames.quotesinglbase, 222 }, { PdfGlyphNames.quotedblbase, 333 }, { PdfGlyphNames.quotedblright, 333 }, 
			{ PdfGlyphNames.guillemotright, 556 }, { PdfGlyphNames.ellipsis, 1000 }, { PdfGlyphNames.perthousand, 1000 }, { PdfGlyphNames.questiondown, 611 }, { PdfGlyphNames.grave, 333 }, 
			{ PdfGlyphNames.acute, 333 }, { PdfGlyphNames.circumflex, 333 }, { PdfGlyphNames.tilde, 333 }, { PdfGlyphNames.macron, 333 }, { PdfGlyphNames.breve, 333 }, { PdfGlyphNames.dotaccent, 333 }, 
			{ PdfGlyphNames.dieresis, 333 }, { PdfGlyphNames.ring, 333 }, { PdfGlyphNames.cedilla, 333 }, { PdfGlyphNames.hungarumlaut, 333 }, { PdfGlyphNames.ogonek, 333 }, { PdfGlyphNames.caron, 333 }, 
			{ PdfGlyphNames.emdash, 1000 }, { PdfGlyphNames.AE, 1000 }, { PdfGlyphNames.ordfeminine, 370 }, { PdfGlyphNames.Lslash, 556 }, { PdfGlyphNames.Oslash, 778 }, { PdfGlyphNames.OE, 1000 }, 
			{ PdfGlyphNames.ordmasculine, 365 }, { PdfGlyphNames.ae, 889 }, { PdfGlyphNames.dotlessi, 278 }, { PdfGlyphNames.lslash, 222 }, { PdfGlyphNames.oslash, 611 }, { PdfGlyphNames.oe, 944 }, 
			{ PdfGlyphNames.germandbls, 611 }, { PdfGlyphNames.Idieresis, 278 }, { PdfGlyphNames.eacute, 556 }, { PdfGlyphNames.abreve, 556 }, { PdfGlyphNames.uhungarumlaut, 556 }, { PdfGlyphNames.ecaron, 556 }, 
			{ PdfGlyphNames.Ydieresis, 667 }, { PdfGlyphNames.divide, 584 }, { PdfGlyphNames.Yacute, 667 }, { PdfGlyphNames.Acircumflex, 667 }, { PdfGlyphNames.aacute, 556 }, { PdfGlyphNames.Ucircumflex, 722 }, 
			{ PdfGlyphNames.yacute, 500 }, { PdfGlyphNames.scommaaccent, 500 }, { PdfGlyphNames.ecircumflex, 556 }, { PdfGlyphNames.Uring, 722 }, { PdfGlyphNames.Udieresis, 722 }, { PdfGlyphNames.aogonek, 556 }, 
			{ PdfGlyphNames.Uacute, 722 }, { PdfGlyphNames.uogonek, 556 }, { PdfGlyphNames.Edieresis, 667 }, { PdfGlyphNames.Dcroat, 722 }, { PdfGlyphNames.commaaccent, 250 }, { PdfGlyphNames.copyright, 737 }, 
			{ PdfGlyphNames.Emacron, 667 }, { PdfGlyphNames.ccaron, 500 }, { PdfGlyphNames.aring, 556 }, { PdfGlyphNames.Ncommaaccent, 722 }, { PdfGlyphNames.lacute, 222 }, { PdfGlyphNames.agrave, 556 }, 
			{ PdfGlyphNames.Tcommaaccent, 611 }, { PdfGlyphNames.Cacute, 722 }, { PdfGlyphNames.atilde, 556 }, { PdfGlyphNames.Edotaccent, 667 }, { PdfGlyphNames.scaron, 500 }, { PdfGlyphNames.scedilla, 500 }, 
			{ PdfGlyphNames.iacute, 278 }, { PdfGlyphNames.lozenge, 471 }, { PdfGlyphNames.Rcaron, 722 }, { PdfGlyphNames.Gcommaaccent, 778 }, { PdfGlyphNames.ucircumflex, 556 }, 
			{ PdfGlyphNames.acircumflex, 556 }, { PdfGlyphNames.Amacron, 667 }, { PdfGlyphNames.rcaron, 333 }, { PdfGlyphNames.ccedilla, 500 }, { PdfGlyphNames.Zdotaccent, 611 }, { PdfGlyphNames.Thorn, 667 }, 
			{ PdfGlyphNames.Omacron, 778 }, { PdfGlyphNames.Racute, 722 }, { PdfGlyphNames.Sacute, 667 }, { PdfGlyphNames.dcaron, 643 }, { PdfGlyphNames.Umacron, 722 }, { PdfGlyphNames.uring, 556 }, 
			{ PdfGlyphNames.threesuperior, 333 }, { PdfGlyphNames.Ograve, 778 }, { PdfGlyphNames.Agrave, 667 }, { PdfGlyphNames.Abreve, 667 }, { PdfGlyphNames.multiply, 584 }, { PdfGlyphNames.uacute, 556 }, 
			{ PdfGlyphNames.Tcaron, 611 }, { PdfGlyphNames.partialdiff, 476 }, { PdfGlyphNames.ydieresis, 500 }, { PdfGlyphNames.Nacute, 722 }, { PdfGlyphNames.icircumflex, 278 }, 
			{ PdfGlyphNames.Ecircumflex, 667 }, { PdfGlyphNames.adieresis, 556 }, { PdfGlyphNames.edieresis, 556 }, { PdfGlyphNames.cacute, 500 }, { PdfGlyphNames.nacute, 556 }, { PdfGlyphNames.umacron, 556 }, 
			{ PdfGlyphNames.Ncaron, 722 }, { PdfGlyphNames.Iacute, 278 }, { PdfGlyphNames.plusminus, 584 }, { PdfGlyphNames.brokenbar, 260 }, { PdfGlyphNames.registered, 737 }, { PdfGlyphNames.Gbreve, 778 }, 
			{ PdfGlyphNames.Idotaccent, 278 }, { PdfGlyphNames.summation, 600 }, { PdfGlyphNames.Egrave, 667 }, { PdfGlyphNames.racute, 333 }, { PdfGlyphNames.omacron, 556 }, { PdfGlyphNames.Zacute, 611 }, 
			{ PdfGlyphNames.Zcaron, 611 }, { PdfGlyphNames.greaterequal, 549 }, { PdfGlyphNames.Eth, 722 }, { PdfGlyphNames.Ccedilla, 722 }, { PdfGlyphNames.lcommaaccent, 222 }, { PdfGlyphNames.tcaron, 317 }, 
			{ PdfGlyphNames.eogonek, 556 }, { PdfGlyphNames.Uogonek, 722 }, { PdfGlyphNames.Aacute, 667 }, { PdfGlyphNames.Adieresis, 667 }, { PdfGlyphNames.egrave, 556 }, { PdfGlyphNames.zacute, 500 }, 
			{ PdfGlyphNames.iogonek, 222 }, { PdfGlyphNames.Oacute, 778 }, { PdfGlyphNames.oacute, 556 }, { PdfGlyphNames.amacron, 556 }, { PdfGlyphNames.sacute, 500 }, { PdfGlyphNames.idieresis, 278 }, 
			{ PdfGlyphNames.Ocircumflex, 778 }, { PdfGlyphNames.Ugrave, 722 }, { PdfGlyphNames.Delta, 612 }, { PdfGlyphNames.thorn, 556 }, { PdfGlyphNames.twosuperior, 333 }, { PdfGlyphNames.Odieresis, 778 }, 
			{ PdfGlyphNames.mu, 556 }, { PdfGlyphNames.igrave, 278 }, { PdfGlyphNames.ohungarumlaut, 556 }, { PdfGlyphNames.Eogonek, 667 }, { PdfGlyphNames.dcroat, 556 }, { PdfGlyphNames.threequarters, 834 }, 
			{ PdfGlyphNames.Scedilla, 667 }, { PdfGlyphNames.lcaron, 299 }, { PdfGlyphNames.Kcommaaccent, 667 }, { PdfGlyphNames.Lacute, 556 }, { PdfGlyphNames.trademark, 1000 }, 
			{ PdfGlyphNames.edotaccent, 556 }, { PdfGlyphNames.Igrave, 278 }, { PdfGlyphNames.Imacron, 278 }, { PdfGlyphNames.Lcaron, 556 }, { PdfGlyphNames.onehalf, 834 }, { PdfGlyphNames.lessequal, 549 }, 
			{ PdfGlyphNames.ocircumflex, 556 }, { PdfGlyphNames.ntilde, 556 }, { PdfGlyphNames.Uhungarumlaut, 722 }, { PdfGlyphNames.Eacute, 667 }, { PdfGlyphNames.emacron, 556 }, { PdfGlyphNames.gbreve, 556 }, 
			{ PdfGlyphNames.onequarter, 834 }, { PdfGlyphNames.Scaron, 667 }, { PdfGlyphNames.Scommaaccent, 667 }, { PdfGlyphNames.Ohungarumlaut, 778 }, { PdfGlyphNames.degree, 400 }, 
			{ PdfGlyphNames.ograve, 556 }, { PdfGlyphNames.Ccaron, 722 }, { PdfGlyphNames.ugrave, 556 }, { PdfGlyphNames.radical, 453 }, { PdfGlyphNames.Dcaron, 722 }, { PdfGlyphNames.rcommaaccent, 333 }, 
			{ PdfGlyphNames.Ntilde, 722 }, { PdfGlyphNames.otilde, 556 }, { PdfGlyphNames.Rcommaaccent, 722 }, { PdfGlyphNames.Lcommaaccent, 556 }, { PdfGlyphNames.Atilde, 667 }, { PdfGlyphNames.Aogonek, 667 }, 
			{ PdfGlyphNames.Aring, 667 }, { PdfGlyphNames.Otilde, 778 }, { PdfGlyphNames.zdotaccent, 500 }, { PdfGlyphNames.Ecaron, 667 }, { PdfGlyphNames.Iogonek, 278 }, { PdfGlyphNames.kcommaaccent, 500 }, 
			{ PdfGlyphNames.minus, 584 }, { PdfGlyphNames.Icircumflex, 278 }, { PdfGlyphNames.ncaron, 556 }, { PdfGlyphNames.tcommaaccent, 278 }, { PdfGlyphNames.logicalnot, 584 }, 
			{ PdfGlyphNames.odieresis, 556 }, { PdfGlyphNames.udieresis, 556 }, { PdfGlyphNames.notequal, 549 }, { PdfGlyphNames.gcommaaccent, 556 }, { PdfGlyphNames.eth, 556 }, { PdfGlyphNames.zcaron, 500 }, 
			{ PdfGlyphNames.ncommaaccent, 556 }, { PdfGlyphNames.onesuperior, 333 }, { PdfGlyphNames.imacron, 278 }, { PdfGlyphNames.Euro, 556 } 
		};
		static readonly Dictionary<string, short> helveticaBoldWidths = new Dictionary<string, short>() {
			{ PdfGlyphNames._notdef, 278 }, { PdfGlyphNames.space, 278 }, { PdfGlyphNames.exclam, 333 }, { PdfGlyphNames.quotedbl, 474 }, { PdfGlyphNames.numbersign, 556 }, { PdfGlyphNames.dollar, 556 }, 
			{ PdfGlyphNames.percent, 889 }, { PdfGlyphNames.ampersand, 722 }, { PdfGlyphNames.quoteright, 278 }, { PdfGlyphNames.parenleft, 333 }, { PdfGlyphNames.parenright, 333 }, 
			{ PdfGlyphNames.asterisk, 389 }, { PdfGlyphNames.plus, 584 }, { PdfGlyphNames.comma, 278 }, { PdfGlyphNames.hyphen, 333 }, { PdfGlyphNames.period, 278 }, { PdfGlyphNames.slash, 278 }, 
			{ PdfGlyphNames.zero, 556 }, { PdfGlyphNames.one, 556 }, { PdfGlyphNames.two, 556 }, { PdfGlyphNames.three, 556 }, { PdfGlyphNames.four, 556 }, { PdfGlyphNames.five, 556 }, { PdfGlyphNames.six, 556 }, 
			{ PdfGlyphNames.seven, 556 }, { PdfGlyphNames.eight, 556 }, { PdfGlyphNames.nine, 556 }, { PdfGlyphNames.colon, 333 }, { PdfGlyphNames.semicolon, 333 }, { PdfGlyphNames.less, 584 }, 
			{ PdfGlyphNames.equal, 584 }, { PdfGlyphNames.greater, 584 }, { PdfGlyphNames.question, 611 }, { PdfGlyphNames.at, 975 }, { PdfGlyphNames.A, 722 }, { PdfGlyphNames.B, 722 }, { PdfGlyphNames.C, 722 }, 
			{ PdfGlyphNames.D, 722 }, { PdfGlyphNames.E, 667 }, { PdfGlyphNames.F, 611 }, { PdfGlyphNames.G, 778 }, { PdfGlyphNames.H, 722 }, { PdfGlyphNames.I, 278 }, { PdfGlyphNames.J, 556 }, 
			{ PdfGlyphNames.K, 722 }, { PdfGlyphNames.L, 611 }, { PdfGlyphNames.M, 833 }, { PdfGlyphNames.N, 722 }, { PdfGlyphNames.O, 778 }, { PdfGlyphNames.P, 667 }, { PdfGlyphNames.Q, 778 }, 
			{ PdfGlyphNames.R, 722 }, { PdfGlyphNames.S, 667 }, { PdfGlyphNames.T, 611 }, { PdfGlyphNames.U, 722 }, { PdfGlyphNames.V, 667 }, { PdfGlyphNames.W, 944 }, { PdfGlyphNames.X, 667 }, 
			{ PdfGlyphNames.Y, 667 }, { PdfGlyphNames.Z, 611 }, { PdfGlyphNames.bracketleft, 333 }, { PdfGlyphNames.backslash, 278 }, { PdfGlyphNames.bracketright, 333 }, { PdfGlyphNames.asciicircum, 584 }, 
			{ PdfGlyphNames.underscore, 556 }, { PdfGlyphNames.quoteleft, 278 }, { PdfGlyphNames.a, 556 }, { PdfGlyphNames.b, 611 }, { PdfGlyphNames.c, 556 }, { PdfGlyphNames.d, 611 }, { PdfGlyphNames.e, 556 }, 
			{ PdfGlyphNames.f, 333 }, { PdfGlyphNames.g, 611 }, { PdfGlyphNames.h, 611 }, { PdfGlyphNames.i, 278 }, { PdfGlyphNames.j, 278 }, { PdfGlyphNames.k, 556 }, { PdfGlyphNames.l, 278 }, 
			{ PdfGlyphNames.m, 889 }, { PdfGlyphNames.n, 611 }, { PdfGlyphNames.o, 611 }, { PdfGlyphNames.p, 611 }, { PdfGlyphNames.q, 611 }, { PdfGlyphNames.r, 389 }, { PdfGlyphNames.s, 556 }, 
			{ PdfGlyphNames.t, 333 }, { PdfGlyphNames.u, 611 }, { PdfGlyphNames.v, 556 }, { PdfGlyphNames.w, 778 }, { PdfGlyphNames.x, 556 }, { PdfGlyphNames.y, 556 }, { PdfGlyphNames.z, 500 }, 
			{ PdfGlyphNames.braceleft, 389 }, { PdfGlyphNames.bar, 280 }, { PdfGlyphNames.braceright, 389 }, { PdfGlyphNames.asciitilde, 584 }, { PdfGlyphNames.exclamdown, 333 }, { PdfGlyphNames.cent, 556 }, 
			{ PdfGlyphNames.sterling, 556 }, { PdfGlyphNames.fraction, 167 }, { PdfGlyphNames.yen, 556 }, { PdfGlyphNames.florin, 556 }, { PdfGlyphNames.section, 556 }, { PdfGlyphNames.currency, 556 }, 
			{ PdfGlyphNames.quotesingle, 238 }, { PdfGlyphNames.quotedblleft, 500 }, { PdfGlyphNames.guillemotleft, 556 }, { PdfGlyphNames.guilsinglleft, 333 }, { PdfGlyphNames.guilsinglright, 333 }, 
			{ PdfGlyphNames.fi, 611 }, { PdfGlyphNames.fl, 611 }, { PdfGlyphNames.endash, 556 }, { PdfGlyphNames.dagger, 556 }, { PdfGlyphNames.daggerdbl, 556 }, { PdfGlyphNames.periodcentered, 278 }, 
			{ PdfGlyphNames.paragraph, 556 }, { PdfGlyphNames.bullet, 350 }, { PdfGlyphNames.quotesinglbase, 278 }, { PdfGlyphNames.quotedblbase, 500 }, { PdfGlyphNames.quotedblright, 500 }, 
			{ PdfGlyphNames.guillemotright, 556 }, { PdfGlyphNames.ellipsis, 1000 }, { PdfGlyphNames.perthousand, 1000 }, { PdfGlyphNames.questiondown, 611 }, { PdfGlyphNames.grave, 333 }, 
			{ PdfGlyphNames.acute, 333 }, { PdfGlyphNames.circumflex, 333 }, { PdfGlyphNames.tilde, 333 }, { PdfGlyphNames.macron, 333 }, { PdfGlyphNames.breve, 333 }, { PdfGlyphNames.dotaccent, 333 }, 
			{ PdfGlyphNames.dieresis, 333 }, { PdfGlyphNames.ring, 333 }, { PdfGlyphNames.cedilla, 333 }, { PdfGlyphNames.hungarumlaut, 333 }, { PdfGlyphNames.ogonek, 333 }, { PdfGlyphNames.caron, 333 }, 
			{ PdfGlyphNames.emdash, 1000 }, { PdfGlyphNames.AE, 1000 }, { PdfGlyphNames.ordfeminine, 370 }, { PdfGlyphNames.Lslash, 611 }, { PdfGlyphNames.Oslash, 778 }, { PdfGlyphNames.OE, 1000 }, 
			{ PdfGlyphNames.ordmasculine, 365 }, { PdfGlyphNames.ae, 889 }, { PdfGlyphNames.dotlessi, 278 }, { PdfGlyphNames.lslash, 278 }, { PdfGlyphNames.oslash, 611 }, { PdfGlyphNames.oe, 944 }, 
			{ PdfGlyphNames.germandbls, 611 }, { PdfGlyphNames.Idieresis, 278 }, { PdfGlyphNames.eacute, 556 }, { PdfGlyphNames.abreve, 556 }, { PdfGlyphNames.uhungarumlaut, 611 }, { PdfGlyphNames.ecaron, 556 }, 
			{ PdfGlyphNames.Ydieresis, 667 }, { PdfGlyphNames.divide, 584 }, { PdfGlyphNames.Yacute, 667 }, { PdfGlyphNames.Acircumflex, 722 }, { PdfGlyphNames.aacute, 556 }, { PdfGlyphNames.Ucircumflex, 722 }, 
			{ PdfGlyphNames.yacute, 556 }, { PdfGlyphNames.scommaaccent, 556 }, { PdfGlyphNames.ecircumflex, 556 }, { PdfGlyphNames.Uring, 722 }, { PdfGlyphNames.Udieresis, 722 }, { PdfGlyphNames.aogonek, 556 },
			{ PdfGlyphNames.Uacute, 722 }, { PdfGlyphNames.uogonek, 611 }, { PdfGlyphNames.Edieresis, 667 }, { PdfGlyphNames.Dcroat, 722 }, { PdfGlyphNames.commaaccent, 250 }, { PdfGlyphNames.copyright, 737 }, 
			{ PdfGlyphNames.Emacron, 667 }, { PdfGlyphNames.ccaron, 556 }, { PdfGlyphNames.aring, 556 }, { PdfGlyphNames.Ncommaaccent, 722 }, { PdfGlyphNames.lacute, 278 }, { PdfGlyphNames.agrave, 556 }, 
			{ PdfGlyphNames.Tcommaaccent, 611 }, { PdfGlyphNames.Cacute, 722 }, { PdfGlyphNames.atilde, 556 }, { PdfGlyphNames.Edotaccent, 667 }, { PdfGlyphNames.scaron, 556 }, { PdfGlyphNames.scedilla, 556 }, 
			{ PdfGlyphNames.iacute, 278 }, { PdfGlyphNames.lozenge, 494 }, { PdfGlyphNames.Rcaron, 722 }, { PdfGlyphNames.Gcommaaccent, 778 }, { PdfGlyphNames.ucircumflex, 611 }, 
			{ PdfGlyphNames.acircumflex, 556 }, { PdfGlyphNames.Amacron, 722 }, { PdfGlyphNames.rcaron, 389 }, { PdfGlyphNames.ccedilla, 556 }, { PdfGlyphNames.Zdotaccent, 611 }, { PdfGlyphNames.Thorn, 667 }, 
			{ PdfGlyphNames.Omacron, 778 }, { PdfGlyphNames.Racute, 722 }, { PdfGlyphNames.Sacute, 667 }, { PdfGlyphNames.dcaron, 743 }, { PdfGlyphNames.Umacron, 722 }, { PdfGlyphNames.uring, 611 }, 
			{ PdfGlyphNames.threesuperior, 333 }, { PdfGlyphNames.Ograve, 778 }, { PdfGlyphNames.Agrave, 722 }, { PdfGlyphNames.Abreve, 722 }, { PdfGlyphNames.multiply, 584 }, { PdfGlyphNames.uacute, 611 }, 
			{ PdfGlyphNames.Tcaron, 611 }, { PdfGlyphNames.partialdiff, 494 }, { PdfGlyphNames.ydieresis, 556 }, { PdfGlyphNames.Nacute, 722 }, { PdfGlyphNames.icircumflex, 278 }, 
			{ PdfGlyphNames.Ecircumflex, 667 }, { PdfGlyphNames.adieresis, 556 }, { PdfGlyphNames.edieresis, 556 }, { PdfGlyphNames.cacute, 556 }, { PdfGlyphNames.nacute, 611 }, { PdfGlyphNames.umacron, 611 }, 
			{ PdfGlyphNames.Ncaron, 722 }, { PdfGlyphNames.Iacute, 278 }, { PdfGlyphNames.plusminus, 584 }, { PdfGlyphNames.brokenbar, 280 }, { PdfGlyphNames.registered, 737 }, { PdfGlyphNames.Gbreve, 778 }, 
			{ PdfGlyphNames.Idotaccent, 278 }, { PdfGlyphNames.summation, 600 }, { PdfGlyphNames.Egrave, 667 }, { PdfGlyphNames.racute, 389 }, { PdfGlyphNames.omacron, 611 }, { PdfGlyphNames.Zacute, 611 }, 
			{ PdfGlyphNames.Zcaron, 611 }, { PdfGlyphNames.greaterequal, 549 }, { PdfGlyphNames.Eth, 722 }, { PdfGlyphNames.Ccedilla, 722 }, { PdfGlyphNames.lcommaaccent, 278 }, { PdfGlyphNames.tcaron, 389 }, 
			{ PdfGlyphNames.eogonek, 556 }, { PdfGlyphNames.Uogonek, 722 }, { PdfGlyphNames.Aacute, 722 }, { PdfGlyphNames.Adieresis, 722 }, { PdfGlyphNames.egrave, 556 }, { PdfGlyphNames.zacute, 500 }, 
			{ PdfGlyphNames.iogonek, 278 }, { PdfGlyphNames.Oacute, 778 }, { PdfGlyphNames.oacute, 611 }, { PdfGlyphNames.amacron, 556 }, { PdfGlyphNames.sacute, 556 }, { PdfGlyphNames.idieresis, 278 }, 
			{ PdfGlyphNames.Ocircumflex, 778 }, { PdfGlyphNames.Ugrave, 722 }, { PdfGlyphNames.Delta, 612 }, { PdfGlyphNames.thorn, 611 }, { PdfGlyphNames.twosuperior, 333 }, { PdfGlyphNames.Odieresis, 778 }, 
			{ PdfGlyphNames.mu, 611 }, { PdfGlyphNames.igrave, 278 }, { PdfGlyphNames.ohungarumlaut, 611 }, { PdfGlyphNames.Eogonek, 667 }, { PdfGlyphNames.dcroat, 611 }, { PdfGlyphNames.threequarters, 834 }, 
			{ PdfGlyphNames.Scedilla, 667 }, { PdfGlyphNames.lcaron, 400 }, { PdfGlyphNames.Kcommaaccent, 722 }, { PdfGlyphNames.Lacute, 611 }, { PdfGlyphNames.trademark, 1000 }, 
			{ PdfGlyphNames.edotaccent, 556 }, { PdfGlyphNames.Igrave, 278 }, { PdfGlyphNames.Imacron, 278 }, { PdfGlyphNames.Lcaron, 611 }, { PdfGlyphNames.onehalf, 834 }, { PdfGlyphNames.lessequal, 549 }, 
			{ PdfGlyphNames.ocircumflex, 611 }, { PdfGlyphNames.ntilde, 611 }, { PdfGlyphNames.Uhungarumlaut, 722 }, { PdfGlyphNames.Eacute, 667 }, { PdfGlyphNames.emacron, 556 }, { PdfGlyphNames.gbreve, 611 }, 
			{ PdfGlyphNames.onequarter, 834 }, { PdfGlyphNames.Scaron, 667 }, { PdfGlyphNames.Scommaaccent, 667 }, { PdfGlyphNames.Ohungarumlaut, 778 }, { PdfGlyphNames.degree, 400 }, 
			{ PdfGlyphNames.ograve, 611 }, { PdfGlyphNames.Ccaron, 722 }, { PdfGlyphNames.ugrave, 611 }, { PdfGlyphNames.radical, 549 }, { PdfGlyphNames.Dcaron, 722 }, { PdfGlyphNames.rcommaaccent, 389 }, 
			{ PdfGlyphNames.Ntilde, 722 }, { PdfGlyphNames.otilde, 611 }, { PdfGlyphNames.Rcommaaccent, 722 }, { PdfGlyphNames.Lcommaaccent, 611 }, { PdfGlyphNames.Atilde, 722 }, { PdfGlyphNames.Aogonek, 722 }, 
			{ PdfGlyphNames.Aring, 722 }, { PdfGlyphNames.Otilde, 778 }, { PdfGlyphNames.zdotaccent, 500 }, { PdfGlyphNames.Ecaron, 667 }, { PdfGlyphNames.Iogonek, 278 }, { PdfGlyphNames.kcommaaccent, 556 }, 
			{ PdfGlyphNames.minus, 584 }, { PdfGlyphNames.Icircumflex, 278 }, { PdfGlyphNames.ncaron, 611 }, { PdfGlyphNames.tcommaaccent, 333 }, { PdfGlyphNames.logicalnot, 584 }, 
			{ PdfGlyphNames.odieresis, 611 }, { PdfGlyphNames.udieresis, 611 }, { PdfGlyphNames.notequal, 549 }, { PdfGlyphNames.gcommaaccent, 611 }, { PdfGlyphNames.eth, 611 }, { PdfGlyphNames.zcaron, 500 }, 
			{ PdfGlyphNames.ncommaaccent, 611 }, { PdfGlyphNames.onesuperior, 333 }, { PdfGlyphNames.imacron, 278 }, { PdfGlyphNames.Euro, 556 } 
		};
		protected static IDictionary<string, short> TimesRomanWidths { get { return timesRomanWidths; } }
		protected static IDictionary<string, short> TimesBoldWidths { get { return timesBoldWidths; } }
		protected static IDictionary<string, short> TimesItalicWidths { get { return timesItalicWidths; } }
		protected static IDictionary<string, short> TimesBoldItalicWidths { get { return timesBoldItalicWidths; } }
		protected static IDictionary<string, short> HelveticaWidths { get { return helveticaWidths; } }
		protected static IDictionary<string, short> HelveticaBoldWidths { get { return helveticaBoldWidths; } }
		internal static PdfSimpleFont Create(string subtype, string baseFont, PdfReaderDictionary dictionary) {
			PdfSimpleFontEncoding encoding = PdfSimpleFontEncoding.Create(baseFont, GetEncodingValue(dictionary));
			PdfReaderStream toUnicode = dictionary.GetStream(ToUnicodeKey);
			int? firstChar = dictionary.GetInteger(firstCharDictionaryKey);
			int? lastChar = dictionary.GetInteger(lastCharDictionaryKey);
			IList<object> widths = dictionary.GetArray(widthsDictionaryKey);
			double[] glyphWidths;
			if (widths == null)
				glyphWidths = null;
			else {
				int count = widths.Count;
				glyphWidths = new double[count];
				for (int i = 0; i < count; i++)
					glyphWidths[i] = PdfDocumentReader.ConvertToDouble(widths[i]);
			}
			PdfReaderDictionary fontDescriptor = dictionary.GetDictionary(FontDescriptorDictionaryKey);
			if (String.IsNullOrEmpty(subtype)) {
				if (!firstChar.HasValue || !lastChar.HasValue || widths == null || fontDescriptor == null)
					PdfDocumentReader.ThrowIncorrectDataException();
				return new PdfUnknownFont(baseFont, toUnicode, fontDescriptor, encoding, firstChar.Value, lastChar.Value, glyphWidths);
			}
			switch (subtype) {
				case PdfType1Font.Name:
					return new PdfType1Font(dictionary.Objects, baseFont, toUnicode, fontDescriptor, encoding, firstChar, lastChar, glyphWidths);
				case PdfMMType1Font.Name:
					if (!firstChar.HasValue || !lastChar.HasValue || widths == null || fontDescriptor == null)
						PdfDocumentReader.ThrowIncorrectDataException();
					return new PdfMMType1Font(dictionary.Objects, baseFont, toUnicode, fontDescriptor, encoding, firstChar.Value, lastChar.Value, glyphWidths);
				case PdfTrueTypeFont.Name:
					return new PdfTrueTypeFont(baseFont, toUnicode, fontDescriptor, encoding, firstChar, lastChar, glyphWidths);
				case PdfType3Font.Name:
					if (!firstChar.HasValue || !lastChar.HasValue || widths == null)
						PdfDocumentReader.ThrowIncorrectDataException();
					return new PdfType3Font(toUnicode, fontDescriptor, encoding, firstChar.Value, lastChar.Value, glyphWidths, dictionary);
				default:
					PdfDocumentReader.ThrowIncorrectDataException();
					break;
			}
			return null;
		}
		readonly PdfSimpleFontEncoding encoding;
		readonly int? firstChar;
		readonly int? lastChar;
		readonly double[] widths;
		readonly SortedDictionary<int, double> widthsList = new SortedDictionary<int, double>();
		double[] actualWidths;
		public PdfSimpleFontEncoding Encoding { get { return encoding; } }
		public int FirstChar { get { return firstChar ?? 1; } }
		public int LastChar { get { return lastChar ?? 255; } }
		public double[] Widths { 
			get { 
				if (actualWidths == null) {
					actualWidths = widths;
					if (actualWidths == null) {
						int firstChar = FirstChar;
						int size = LastChar - firstChar + 1;
						if (IsCourierFont) {
							actualWidths = new double[size];
							for (int i = 0; i < size; i++)
								actualWidths[i] = 600;
						}
						else {
							IDictionary<string, short> dictionary = DefaultWidthsDictionary;
							if (dictionary != null) {
								actualWidths = new double[size];
								byte j = (byte)firstChar;
								for (int i = 0; i < size; i++, j++) {
									string glyphName = encoding.GetGlyphName(j);
									if (String.IsNullOrEmpty(glyphName))
										actualWidths[i] = 0;
									else {
										short code;
										actualWidths[i] = dictionary.TryGetValue(glyphName, out code) ? code : 0;
									}
								}
							}
						}
					}
				}
				return actualWidths;
			} 
		}
		protected internal override PdfEncoding ActualEncoding { get { return encoding; } }
		protected override IEnumerable<double> ActualWidths { get { return Widths; } }
		protected virtual bool IsCourierFont { get { return false; } }
		protected virtual IDictionary<string, short> DefaultWidthsDictionary { get { return null; } }
		protected PdfSimpleFont(string baseFont, PdfReaderStream toUnicode, PdfReaderDictionary fontDescriptor, PdfSimpleFontEncoding encoding, int? firstChar, int? lastChar, double[] widths) 
				: base(baseFont, toUnicode, fontDescriptor) {
			this.encoding = encoding;
			this.firstChar = firstChar;
			this.lastChar = lastChar;
			this.widths = widths;
			double[] actualWidths = Widths;
			if (actualWidths != null) {
				int actualFirstChar = FirstChar;
				int newLastChar = actualFirstChar + Math.Min(LastChar - actualFirstChar, actualWidths.Length - 1);
				for (int i = actualFirstChar, index = 0; i <= newLastChar; i++)
					widthsList.Add(i, actualWidths[index++]);
			}
		}
		protected internal override double[] GetGlyphPositions(PdfStringData stringData, double fontSizeFactor, double characterSpacing, double wordSpacing, double scalingFactor, double horizontalScalingFactor) {
			PdfFontDescriptor fontDescriptor = FontDescriptor;
			double missingWidth = fontDescriptor == null ? 0 : fontDescriptor.MissingWidth;
			byte[][] charCodes = stringData.CharCodes;
			short[] str = stringData.Str;
			double[] glyphOffsets = stringData.Offsets;
			int stringLength = str.Length;
			double[] positions = new double[stringLength];
			double tx = 0;
			for (int i = 0, j = 1; i < stringLength; i++, j++) {
				short chr = str[i];
				double w0;
				if (!widthsList.TryGetValue(chr, out w0))
					w0 = missingWidth;
				tx += ((w0 - glyphOffsets[j]) * fontSizeFactor * horizontalScalingFactor + characterSpacing + (charCodes[i][0] == 32 ? wordSpacing : 0)) * scalingFactor;
				positions[i] = tx;
			}
			return positions;
		}
		protected override PdfWriterDictionary CreateDictionary(PdfObjectCollection objects) {
			PdfWriterDictionary dictionary = base.CreateDictionary(objects);
			dictionary.AddNullable(firstCharDictionaryKey,  firstChar);
			dictionary.AddNullable(lastCharDictionaryKey, lastChar);
			dictionary.Add(widthsDictionaryKey, widths);
			dictionary.AddIfPresent(FontDescriptorDictionaryKey, objects.AddObject(RawFontDescriptor));
			return dictionary;
		}
	}
}
