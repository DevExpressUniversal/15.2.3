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
	public class PdfCompactFontFormatEncoding {
		readonly bool isIdentityEncoding;
		readonly IDictionary<string, short> encoding;
		public bool IsIdentityEncoding { get { return isIdentityEncoding; } }
		public IDictionary<string, short> Encoding { get { return encoding; } }
		public PdfCompactFontFormatEncoding(bool isIdentityEncoding, IDictionary<string, short> encoding) {
			this.isIdentityEncoding = isIdentityEncoding;
			this.encoding = encoding;
		}
	}
	public static class PdfCompactFontFormatParser {
		static Dictionary<string, short> isoAdobeEncoding = new Dictionary<string, short> {
			{ PdfGlyphNames.space, 32 }, { PdfGlyphNames.exclam, 33 }, { PdfGlyphNames.quotedbl, 34 }, { PdfGlyphNames.numbersign, 35 }, { PdfGlyphNames.dollar, 36 }, { PdfGlyphNames.percent, 37 }, 
			{ PdfGlyphNames.ampersand, 38 }, { PdfGlyphNames.quoteright, 39 }, { PdfGlyphNames.parenleft, 40 }, { PdfGlyphNames.parenright, 41 }, { PdfGlyphNames.asterisk, 42 }, 
			{ PdfGlyphNames.plus, 43 }, { PdfGlyphNames.comma, 44 }, { PdfGlyphNames.hyphen, 45 }, { PdfGlyphNames.period, 46 }, { PdfGlyphNames.slash, 47 }, { PdfGlyphNames.zero, 48 }, 
			{ PdfGlyphNames.one, 49 }, { PdfGlyphNames.two, 50 }, { PdfGlyphNames.three, 51 }, { PdfGlyphNames.four, 52 }, { PdfGlyphNames.five, 53 }, { PdfGlyphNames.six, 54 }, 
			{ PdfGlyphNames.seven, 55 }, { PdfGlyphNames.eight, 56 }, { PdfGlyphNames.nine, 57 }, { PdfGlyphNames.colon, 58 }, { PdfGlyphNames.semicolon, 59 }, { PdfGlyphNames.less, 60 }, 
			{ PdfGlyphNames.equal, 61 }, { PdfGlyphNames.greater, 62 }, { PdfGlyphNames.question, 63 }, { PdfGlyphNames.at, 64 }, { PdfGlyphNames.A, 65 }, { PdfGlyphNames.B, 66 }, 
			{ PdfGlyphNames.C, 67 }, { PdfGlyphNames.D, 68 }, { PdfGlyphNames.E, 69 }, { PdfGlyphNames.F, 70 }, { PdfGlyphNames.G, 71 }, { PdfGlyphNames.H, 72 }, { PdfGlyphNames.I, 73 }, 
			{ PdfGlyphNames.J, 74 }, { PdfGlyphNames.K, 75 }, { PdfGlyphNames.L, 76 }, { PdfGlyphNames.M, 77 }, { PdfGlyphNames.N, 78 }, { PdfGlyphNames.O, 79 }, { PdfGlyphNames.P, 80 }, 
			{ PdfGlyphNames.Q, 81 }, { PdfGlyphNames.R, 82 }, { PdfGlyphNames.S, 83 }, { PdfGlyphNames.T, 84 }, { PdfGlyphNames.U, 85 }, { PdfGlyphNames.V, 86 }, { PdfGlyphNames.W, 87 }, 
			{ PdfGlyphNames.X, 88 }, { PdfGlyphNames.Y, 89 }, { PdfGlyphNames.Z, 90 }, { PdfGlyphNames.bracketleft, 91 }, { PdfGlyphNames.backslash, 92 }, { PdfGlyphNames.bracketright, 93 }, 
			{ PdfGlyphNames.asciicircum, 94 }, { PdfGlyphNames.underscore, 95 }, { PdfGlyphNames.quoteleft, 96 }, { PdfGlyphNames.a, 97 }, { PdfGlyphNames.b, 98 }, { PdfGlyphNames.c, 99 }, 
			{ PdfGlyphNames.d, 100 }, { PdfGlyphNames.e, 101 }, { PdfGlyphNames.f, 102 }, { PdfGlyphNames.g, 103 }, { PdfGlyphNames.h, 104 }, { PdfGlyphNames.i, 105 }, { PdfGlyphNames.j, 106 }, 
			{ PdfGlyphNames.k, 107 }, { PdfGlyphNames.l, 108 }, { PdfGlyphNames.m, 109 }, { PdfGlyphNames.n, 110 }, { PdfGlyphNames.o, 111 }, { PdfGlyphNames.p, 112 }, { PdfGlyphNames.q, 113 }, 
			{ PdfGlyphNames.r, 114 }, { PdfGlyphNames.s, 115 }, { PdfGlyphNames.t, 116 }, { PdfGlyphNames.u, 117 }, { PdfGlyphNames.v, 118 }, { PdfGlyphNames.w, 119 }, { PdfGlyphNames.x, 120 }, 
			{ PdfGlyphNames.y, 121 }, { PdfGlyphNames.z, 122 }, { PdfGlyphNames.braceleft, 123 }, { PdfGlyphNames.bar, 124 }, { PdfGlyphNames.braceright, 125 }, { PdfGlyphNames.asciitilde, 126 }, 
			{ PdfGlyphNames.exclamdown, 127 }, { PdfGlyphNames.cent, 128 }, { PdfGlyphNames.sterling, 129 }, { PdfGlyphNames.fraction, 130 }, { PdfGlyphNames.yen, 131 }, { PdfGlyphNames.florin, 132 }, 
			{ PdfGlyphNames.section, 133 }, { PdfGlyphNames.currency, 134 }, { PdfGlyphNames.quotesingle, 135 }, { PdfGlyphNames.quotedblleft, 136 }, { PdfGlyphNames.guillemotleft, 137 }, 
			{ PdfGlyphNames.guilsinglleft, 138 }, { PdfGlyphNames.guilsinglright, 139 }, { PdfGlyphNames.fi, 140 }, { PdfGlyphNames.fl, 141 }, { PdfGlyphNames.endash, 142 }, 
			{ PdfGlyphNames.dagger, 143 }, { PdfGlyphNames.daggerdbl, 144 }, { PdfGlyphNames.periodcentered, 145 }, { PdfGlyphNames.paragraph, 146 }, { PdfGlyphNames.bullet, 147 }, 
			{ PdfGlyphNames.quotesinglbase, 148 }, { PdfGlyphNames.quotedblbase, 149 }, { PdfGlyphNames.quotedblright, 150 }, { PdfGlyphNames.guillemotright, 151 }, { PdfGlyphNames.ellipsis, 152 }, 
			{ PdfGlyphNames.perthousand, 153 }, { PdfGlyphNames.questiondown, 154 }, { PdfGlyphNames.grave, 155 }, { PdfGlyphNames.acute, 156 }, { PdfGlyphNames.circumflex, 157 }, 
			{ PdfGlyphNames.tilde, 158 }, { PdfGlyphNames.macron, 159 }, { PdfGlyphNames.breve, 160 }, { PdfGlyphNames.dotaccent, 161 }, { PdfGlyphNames.dieresis, 162 }, { PdfGlyphNames.ring, 163 }, 
			{ PdfGlyphNames.cedilla, 164 }, { PdfGlyphNames.hungarumlaut, 165 }, { PdfGlyphNames.ogonek, 166 }, { PdfGlyphNames.caron, 167 }, { PdfGlyphNames.emdash, 168 }, { PdfGlyphNames.AE, 169 }, 
			{ PdfGlyphNames.ordfeminine, 170 }, { PdfGlyphNames.Lslash, 171 }, { PdfGlyphNames.Oslash, 172 }, { PdfGlyphNames.OE, 173 }, { PdfGlyphNames.ordmasculine, 174 }, { PdfGlyphNames.ae, 175 }, 
			{ PdfGlyphNames.dotlessi, 176 }, { PdfGlyphNames.lslash, 177 }, { PdfGlyphNames.oslash, 178 }, { PdfGlyphNames.oe, 179 }, { PdfGlyphNames.germandbls, 180 }, 
			{ PdfGlyphNames.onesuperior, 181 }, { PdfGlyphNames.logicalnot, 182 }, { PdfGlyphNames.mu, 183 }, { PdfGlyphNames.trademark, 184 }, { PdfGlyphNames.Eth, 185 }, 
			{ PdfGlyphNames.onehalf, 186 }, { PdfGlyphNames.plusminus, 187 }, { PdfGlyphNames.Thorn, 188 }, { PdfGlyphNames.onequarter, 189 }, { PdfGlyphNames.divide, 190 }, 
			{ PdfGlyphNames.brokenbar, 191 }, { PdfGlyphNames.degree, 192 }, { PdfGlyphNames.thorn, 193 }, { PdfGlyphNames.threequarters, 194 }, { PdfGlyphNames.twosuperior, 195 }, 
			{ PdfGlyphNames.registered, 196 }, { PdfGlyphNames.minus, 197 }, { PdfGlyphNames.eth, 198 }, { PdfGlyphNames.multiply, 199 }, { PdfGlyphNames.threesuperior, 200 }, 
			{ PdfGlyphNames.copyright, 201 }, { PdfGlyphNames.Aacute, 202 }, { PdfGlyphNames.Acircumflex, 203 }, { PdfGlyphNames.Adieresis, 204 }, { PdfGlyphNames.Agrave, 205 }, 
			{ PdfGlyphNames.Aring, 206 }, { PdfGlyphNames.Atilde, 207 }, { PdfGlyphNames.Ccedilla, 208 }, { PdfGlyphNames.Eacute, 209 }, { PdfGlyphNames.Ecircumflex, 210 }, 
			{ PdfGlyphNames.Edieresis, 211 }, { PdfGlyphNames.Egrave, 212 }, { PdfGlyphNames.Iacute, 213 }, { PdfGlyphNames.Icircumflex, 214 }, { PdfGlyphNames.Idieresis, 215 }, 
			{ PdfGlyphNames.Igrave, 216 }, { PdfGlyphNames.Ntilde, 217 }, { PdfGlyphNames.Oacute, 218 }, { PdfGlyphNames.Ocircumflex, 219 }, { PdfGlyphNames.Odieresis, 220 }, 
			{ PdfGlyphNames.Ograve, 221 }, { PdfGlyphNames.Otilde, 222 }, { PdfGlyphNames.Scaron, 223 }, { PdfGlyphNames.Uacute, 224 }, { PdfGlyphNames.Ucircumflex, 225 }, 
			{ PdfGlyphNames.Udieresis, 226 }, { PdfGlyphNames.Ugrave, 227 }, { PdfGlyphNames.Yacute, 228 }, { PdfGlyphNames.Ydieresis, 229 }, { PdfGlyphNames.Zcaron, 230 }, 
			{ PdfGlyphNames.aacute, 231 }, { PdfGlyphNames.acircumflex, 232 }, { PdfGlyphNames.adieresis, 233 }, { PdfGlyphNames.agrave, 234 }, { PdfGlyphNames.aring, 235 }, 
			{ PdfGlyphNames.atilde, 236 }, { PdfGlyphNames.ccedilla, 237 }, { PdfGlyphNames.eacute, 238 }, { PdfGlyphNames.ecircumflex, 239 }, { PdfGlyphNames.edieresis, 240 }, 
			{ PdfGlyphNames.egrave, 241 }, { PdfGlyphNames.iacute, 242 }, { PdfGlyphNames.icircumflex, 243 }, { PdfGlyphNames.idieresis, 244 }, { PdfGlyphNames.igrave, 245 }, 
			{ PdfGlyphNames.ntilde, 246 }, { PdfGlyphNames.oacute, 247 }, { PdfGlyphNames.ocircumflex, 248 }, { PdfGlyphNames.odieresis, 249 }, { PdfGlyphNames.ograve, 250 }, 
			{ PdfGlyphNames.otilde, 251 }, { PdfGlyphNames.scaron, 252 }, { PdfGlyphNames.uacute, 253 }, { PdfGlyphNames.ucircumflex, 254 }, { PdfGlyphNames.udieresis, 255 }, 
			{ PdfGlyphNames.ugrave, 256 }, { PdfGlyphNames.yacute, 257 }, { PdfGlyphNames.ydieresis, 258 }, { PdfGlyphNames.zcaron, 259 } 
		}; 
		static readonly Dictionary<string, short> expertEncoding = new Dictionary<string, short> {
			{ PdfGlyphNames.space, 32 }, { PdfGlyphNames.exclamsmall, 260 }, { PdfGlyphNames.Hungarumlautsmall, 261 }, { PdfGlyphNames.dollaroldstyle, 262 }, { PdfGlyphNames.dollarsuperior, 263 }, 
			{ PdfGlyphNames.ampersandsmall, 264 }, { PdfGlyphNames.Acutesmall, 265 }, { PdfGlyphNames.parenleftsuperior, 266 }, { PdfGlyphNames.parenrightsuperior, 267 }, 
			{ PdfGlyphNames.twodotenleader, 268 }, { PdfGlyphNames.onedotenleader, 269 }, { PdfGlyphNames.comma, 44 }, { PdfGlyphNames.hyphen, 45 }, { PdfGlyphNames.period, 46 }, 
			{ PdfGlyphNames.fraction, 130 }, { PdfGlyphNames.zerooldstyle, 270 }, { PdfGlyphNames.oneoldstyle, 271 }, { PdfGlyphNames.twooldstyle, 272 }, { PdfGlyphNames.threeoldstyle, 273 }, 
			{ PdfGlyphNames.fouroldstyle, 274 }, { PdfGlyphNames.fiveoldstyle, 275 }, { PdfGlyphNames.sixoldstyle, 276 }, { PdfGlyphNames.sevenoldstyle, 277 }, { PdfGlyphNames.eightoldstyle, 278 }, 
			{ PdfGlyphNames.nineoldstyle, 279 }, { PdfGlyphNames.colon, 58 }, { PdfGlyphNames.semicolon, 59 }, { PdfGlyphNames.commasuperior, 280 }, { PdfGlyphNames.threequartersemdash, 281 }, 
			{ PdfGlyphNames.periodsuperior, 282 }, { PdfGlyphNames.questionsmall, 283 }, { PdfGlyphNames.asuperior, 284 }, { PdfGlyphNames.bsuperior, 285 }, { PdfGlyphNames.centsuperior, 286 }, 
			{ PdfGlyphNames.dsuperior, 287 }, { PdfGlyphNames.esuperior, 288 }, { PdfGlyphNames.isuperior, 289 }, { PdfGlyphNames.lsuperior, 290 }, { PdfGlyphNames.msuperior, 291 }, 
			{ PdfGlyphNames.nsuperior, 292 }, { PdfGlyphNames.osuperior, 293 }, { PdfGlyphNames.rsuperior, 294 }, { PdfGlyphNames.ssuperior, 295 }, { PdfGlyphNames.tsuperior, 296 }, 
			{ PdfGlyphNames.ff, 297 }, { PdfGlyphNames.fi, 140 }, { PdfGlyphNames.fl, 141 }, { PdfGlyphNames.ffi, 298 }, { PdfGlyphNames.ffl, 299 }, { PdfGlyphNames.parenleftinferior, 300 }, 
			{ PdfGlyphNames.parenrightinferior, 301 }, { PdfGlyphNames.Circumflexsmall, 302 }, { PdfGlyphNames.hyphensuperior, 303 }, { PdfGlyphNames.Gravesmall, 304 }, { PdfGlyphNames.Asmall, 305 }, 
			{ PdfGlyphNames.Bsmall, 306 }, { PdfGlyphNames.Csmall, 307 }, { PdfGlyphNames.Dsmall, 308 }, { PdfGlyphNames.Esmall, 309 }, { PdfGlyphNames.Fsmall, 310 }, { PdfGlyphNames.Gsmall, 311 }, 
			{ PdfGlyphNames.Hsmall, 312 }, { PdfGlyphNames.Ismall, 313 }, { PdfGlyphNames.Jsmall, 314 }, { PdfGlyphNames.Ksmall, 315 }, { PdfGlyphNames.Lsmall, 316 }, { PdfGlyphNames.Msmall, 317 }, 
			{ PdfGlyphNames.Nsmall, 318 }, { PdfGlyphNames.Osmall, 319 }, { PdfGlyphNames.Psmall, 320 }, { PdfGlyphNames.Qsmall, 321 }, { PdfGlyphNames.Rsmall, 322 }, { PdfGlyphNames.Ssmall, 323 }, 
			{ PdfGlyphNames.Tsmall, 324 }, { PdfGlyphNames.Usmall, 325 }, { PdfGlyphNames.Vsmall, 326 }, { PdfGlyphNames.Wsmall, 327 }, { PdfGlyphNames.Xsmall, 328 }, { PdfGlyphNames.Ysmall, 329 }, 
			{ PdfGlyphNames.Zsmall, 330 }, { PdfGlyphNames.colonmonetary, 331 }, { PdfGlyphNames.onefitted, 332 }, { PdfGlyphNames.rupiah, 333 }, { PdfGlyphNames.Tildesmall, 334 }, 
			{ PdfGlyphNames.exclamdownsmall, 335 }, { PdfGlyphNames.centoldstyle, 336 }, { PdfGlyphNames.Lslashsmall, 337 }, { PdfGlyphNames.Scaronsmall, 338 }, { PdfGlyphNames.Zcaronsmall, 339 }, 
			{ PdfGlyphNames.Dieresissmall, 340 }, { PdfGlyphNames.Brevesmall, 341 }, { PdfGlyphNames.Caronsmall, 342 }, { PdfGlyphNames.Dotaccentsmall, 343 }, { PdfGlyphNames.Macronsmall, 344 }, 
			{ PdfGlyphNames.figuredash, 345 }, { PdfGlyphNames.hypheninferior, 346 }, { PdfGlyphNames.Ogoneksmall, 347 }, { PdfGlyphNames.Ringsmall, 348 }, { PdfGlyphNames.Cedillasmall, 349 }, 
			{ PdfGlyphNames.onequarter, 189 }, { PdfGlyphNames.onehalf, 186 }, { PdfGlyphNames.threequarters, 194 }, { PdfGlyphNames.questiondownsmall, 350 }, { PdfGlyphNames.oneeighth, 351 }, 
			{ PdfGlyphNames.threeeighths, 352 }, { PdfGlyphNames.fiveeighths, 353 }, { PdfGlyphNames.seveneighths, 354 }, { PdfGlyphNames.onethird, 355 }, { PdfGlyphNames.twothirds, 356 }, 
			{ PdfGlyphNames.zerosuperior, 357 }, { PdfGlyphNames.onesuperior, 181 }, { PdfGlyphNames.twosuperior, 195 }, { PdfGlyphNames.threesuperior, 200 }, { PdfGlyphNames.foursuperior, 358 }, 
			{ PdfGlyphNames.fivesuperior, 359 }, { PdfGlyphNames.sixsuperior, 360 }, { PdfGlyphNames.sevensuperior, 361 }, { PdfGlyphNames.eightsuperior, 362 }, { PdfGlyphNames.ninesuperior, 363 }, 
			{ PdfGlyphNames.zeroinferior, 364 }, { PdfGlyphNames.oneinferior, 365 }, { PdfGlyphNames.twoinferior, 366 }, { PdfGlyphNames.threeinferior, 367 }, { PdfGlyphNames.fourinferior, 368 }, 
			{ PdfGlyphNames.fiveinferior, 369 }, { PdfGlyphNames.sixinferior, 370 }, { PdfGlyphNames.seveninferior, 371 }, { PdfGlyphNames.eightinferior, 372 }, { PdfGlyphNames.nineinferior, 373 }, 
			{ PdfGlyphNames.centinferior, 374 }, { PdfGlyphNames.dollarinferior, 375 }, { PdfGlyphNames.periodinferior, 376 }, { PdfGlyphNames.commainferior, 377 }, 
			{ PdfGlyphNames.Agravesmall, 378 }, { PdfGlyphNames.Aacutesmall, 379 }, { PdfGlyphNames.Acircumflexsmall, 380 }, { PdfGlyphNames.Atildesmall, 381 }, { PdfGlyphNames.Adieresissmall, 382 },
			{ PdfGlyphNames.Aringsmall, 383 }, { PdfGlyphNames.AEsmall, 384 }, { PdfGlyphNames.Ccedillasmall, 385 }, { PdfGlyphNames.Egravesmall, 386 }, { PdfGlyphNames.Eacutesmall, 387 }, 
			{ PdfGlyphNames.Ecircumflexsmall, 388 }, { PdfGlyphNames.Edieresissmall, 389 }, { PdfGlyphNames.Igravesmall, 390 }, { PdfGlyphNames.Iacutesmall, 391 }, 
			{ PdfGlyphNames.Icircumflexsmall, 392 }, { PdfGlyphNames.Idieresissmall, 393 }, { PdfGlyphNames.Ethsmall, 394 }, { PdfGlyphNames.Ntildesmall, 395 }, { PdfGlyphNames.Ogravesmall, 396 }, 
			{ PdfGlyphNames.Oacutesmall, 397 }, { PdfGlyphNames.Ocircumflexsmall, 398 }, { PdfGlyphNames.Otildesmall, 399 }, { PdfGlyphNames.Odieresissmall, 400 }, { PdfGlyphNames.OEsmall, 401 }, 
			{ PdfGlyphNames.Oslashsmall, 402 }, { PdfGlyphNames.Ugravesmall, 403 }, { PdfGlyphNames.Uacutesmall, 404 }, { PdfGlyphNames.Ucircumflexsmall, 405 }, { PdfGlyphNames.Udieresissmall, 406 },
			{ PdfGlyphNames.Yacutesmall, 407 }, { PdfGlyphNames.Thornsmall, 408 }, { PdfGlyphNames.Ydieresissmall, 409 }
		};
		static readonly Dictionary<string, short> expertSubsetEncoding = new Dictionary<string, short> {
			{ PdfGlyphNames.space, 32 }, { PdfGlyphNames.dollaroldstyle, 262 }, { PdfGlyphNames.dollarsuperior, 263 }, { PdfGlyphNames.parenleftsuperior, 266 }, { PdfGlyphNames.parenrightsuperior, 267 }, 
			{ PdfGlyphNames.twodotenleader, 268 }, { PdfGlyphNames.onedotenleader, 269 }, { PdfGlyphNames.comma, 44 }, { PdfGlyphNames.hyphen, 45 }, { PdfGlyphNames.period, 46 }, 
			{ PdfGlyphNames.fraction, 130 }, { PdfGlyphNames.zerooldstyle, 270 }, { PdfGlyphNames.oneoldstyle, 271 }, { PdfGlyphNames.twooldstyle, 272 }, { PdfGlyphNames.threeoldstyle, 273 }, 
			{ PdfGlyphNames.fouroldstyle, 274 }, { PdfGlyphNames.fiveoldstyle, 275 }, { PdfGlyphNames.sixoldstyle, 276 }, { PdfGlyphNames.sevenoldstyle, 277 }, { PdfGlyphNames.eightoldstyle, 278 }, 
			{ PdfGlyphNames.nineoldstyle, 279 }, { PdfGlyphNames.colon, 58 }, { PdfGlyphNames.semicolon, 59 }, { PdfGlyphNames.commasuperior, 280 }, { PdfGlyphNames.threequartersemdash, 281 },
			{ PdfGlyphNames.periodsuperior, 282 }, { PdfGlyphNames.asuperior, 284 }, { PdfGlyphNames.bsuperior, 285 }, { PdfGlyphNames.centsuperior, 286 }, { PdfGlyphNames.dsuperior, 287 }, 
			{ PdfGlyphNames.esuperior, 288 }, { PdfGlyphNames.isuperior, 289 }, { PdfGlyphNames.lsuperior, 290 }, { PdfGlyphNames.msuperior, 291 }, { PdfGlyphNames.nsuperior, 292 }, 
			{ PdfGlyphNames.osuperior, 293 }, { PdfGlyphNames.rsuperior, 294 }, { PdfGlyphNames.ssuperior, 295 }, { PdfGlyphNames.tsuperior, 296 }, { PdfGlyphNames.ff, 297 }, { PdfGlyphNames.fi, 140 }, 
			{ PdfGlyphNames.fl, 141 }, { PdfGlyphNames.ffi, 298 }, { PdfGlyphNames.ffl, 299 }, { PdfGlyphNames.parenleftinferior, 300 }, { PdfGlyphNames.parenrightinferior, 301 }, 
			{ PdfGlyphNames.hyphensuperior, 303 }, { PdfGlyphNames.colonmonetary, 331 }, { PdfGlyphNames.onefitted, 332 }, { PdfGlyphNames.rupiah, 333 }, { PdfGlyphNames.centoldstyle, 336 }, 
			{ PdfGlyphNames.figuredash, 345 }, { PdfGlyphNames.hypheninferior, 346 }, { PdfGlyphNames.onequarter, 189 }, { PdfGlyphNames.onehalf, 186 }, { PdfGlyphNames.threequarters, 194 }, 
			{ PdfGlyphNames.oneeighth, 351 }, { PdfGlyphNames.threeeighths, 352 }, { PdfGlyphNames.fiveeighths, 353 }, { PdfGlyphNames.seveneighths, 354 }, { PdfGlyphNames.onethird, 355 }, 
			{ PdfGlyphNames.twothirds, 356 }, { PdfGlyphNames.zerosuperior, 357 }, { PdfGlyphNames.onesuperior, 181 }, { PdfGlyphNames.twosuperior, 195 }, { PdfGlyphNames.threesuperior, 200 }, 
			{ PdfGlyphNames.foursuperior, 358 }, { PdfGlyphNames.fivesuperior, 359 }, { PdfGlyphNames.sixsuperior, 360 }, { PdfGlyphNames.sevensuperior, 361 }, { PdfGlyphNames.eightsuperior, 362 }, 
			{ PdfGlyphNames.ninesuperior, 363 }, { PdfGlyphNames.zeroinferior, 364 }, { PdfGlyphNames.oneinferior, 365 }, { PdfGlyphNames.twoinferior, 366 }, { PdfGlyphNames.threeinferior, 367 }, 
			{ PdfGlyphNames.fourinferior, 368 }, { PdfGlyphNames.fiveinferior, 369 }, { PdfGlyphNames.sixinferior, 370 }, { PdfGlyphNames.seveninferior, 371 }, { PdfGlyphNames.eightinferior, 372 }, 
			{ PdfGlyphNames.nineinferior, 373 }, { PdfGlyphNames.centinferior, 374 }, { PdfGlyphNames.dollarinferior, 375 }, { PdfGlyphNames.periodinferior, 376 }, { PdfGlyphNames.commainferior, 377 }		 
		};
		static readonly Dictionary<short, string> standartGlyphNames = new Dictionary<short, string> {	  
			{ 0, PdfGlyphNames._notdef }, { 1, PdfGlyphNames.space }, { 2, PdfGlyphNames.exclam }, { 3, PdfGlyphNames.quotedbl }, { 4, PdfGlyphNames.numbersign }, { 5, PdfGlyphNames.dollar }, 
			{ 6, PdfGlyphNames.percent }, { 7, PdfGlyphNames.ampersand }, { 8, PdfGlyphNames.quoteright }, { 9, PdfGlyphNames.parenleft }, { 10, PdfGlyphNames.parenright }, 
			{ 11, PdfGlyphNames.asterisk }, { 12, PdfGlyphNames.plus }, { 13, PdfGlyphNames.comma }, { 14, PdfGlyphNames.hyphen }, { 15, PdfGlyphNames.period }, { 16, PdfGlyphNames.slash }, 
			{ 17, PdfGlyphNames.zero }, { 18, PdfGlyphNames.one }, { 19, PdfGlyphNames.two }, { 20, PdfGlyphNames.three }, { 21, PdfGlyphNames.four }, { 22, PdfGlyphNames.five }, 
			{ 23, PdfGlyphNames.six }, { 24, PdfGlyphNames.seven }, { 25, PdfGlyphNames.eight }, { 26, PdfGlyphNames.nine }, { 27, PdfGlyphNames.colon }, { 28, PdfGlyphNames.semicolon }, 
			{ 29, PdfGlyphNames.less }, { 30, PdfGlyphNames.equal }, { 31, PdfGlyphNames.greater }, { 32, PdfGlyphNames.question }, { 33, PdfGlyphNames.at }, { 34, PdfGlyphNames.A }, 
			{ 35, PdfGlyphNames.B }, { 36, PdfGlyphNames.C }, { 37, PdfGlyphNames.D }, { 38, PdfGlyphNames.E }, { 39, PdfGlyphNames.F }, { 40, PdfGlyphNames.G }, { 41, PdfGlyphNames.H }, 
			{ 42, PdfGlyphNames.I }, { 43, PdfGlyphNames.J }, { 44, PdfGlyphNames.K }, { 45, PdfGlyphNames.L }, { 46, PdfGlyphNames.M }, { 47, PdfGlyphNames.N }, { 48, PdfGlyphNames.O }, 
			{ 49, PdfGlyphNames.P }, { 50, PdfGlyphNames.Q }, { 51, PdfGlyphNames.R }, { 52, PdfGlyphNames.S }, { 53, PdfGlyphNames.T }, { 54, PdfGlyphNames.U }, { 55, PdfGlyphNames.V }, 
			{ 56, PdfGlyphNames.W }, { 57, PdfGlyphNames.X }, { 58, PdfGlyphNames.Y }, { 59, PdfGlyphNames.Z }, { 60, PdfGlyphNames.bracketleft }, { 61, PdfGlyphNames.backslash }, 
			{ 62, PdfGlyphNames.bracketright }, { 63, PdfGlyphNames.asciicircum }, { 64, PdfGlyphNames.underscore }, { 65, PdfGlyphNames.quoteleft }, { 66, PdfGlyphNames.a }, { 67, PdfGlyphNames.b }, 
			{ 68, PdfGlyphNames.c }, { 69, PdfGlyphNames.d }, { 70, PdfGlyphNames.e }, { 71, PdfGlyphNames.f }, { 72, PdfGlyphNames.g }, { 73, PdfGlyphNames.h }, { 74, PdfGlyphNames.i }, 
			{ 75, PdfGlyphNames.j }, { 76, PdfGlyphNames.k }, { 77, PdfGlyphNames.l }, { 78, PdfGlyphNames.m }, { 79, PdfGlyphNames.n }, { 80, PdfGlyphNames.o }, { 81, PdfGlyphNames.p }, 
			{ 82, PdfGlyphNames.q }, { 83, PdfGlyphNames.r }, { 84, PdfGlyphNames.s }, { 85, PdfGlyphNames.t }, { 86, PdfGlyphNames.u }, { 87, PdfGlyphNames.v }, { 88, PdfGlyphNames.w }, 
			{ 89, PdfGlyphNames.x }, { 90, PdfGlyphNames.y }, { 91, PdfGlyphNames.z }, { 92, PdfGlyphNames.braceleft }, { 93, PdfGlyphNames.bar }, { 94, PdfGlyphNames.braceright }, 
			{ 95, PdfGlyphNames.asciitilde }, { 96, PdfGlyphNames.exclamdown }, { 97, PdfGlyphNames.cent }, { 98, PdfGlyphNames.sterling }, { 99, PdfGlyphNames.fraction }, { 100, PdfGlyphNames.yen}, 
			{ 101, PdfGlyphNames.florin}, { 102, PdfGlyphNames.section }, { 103, PdfGlyphNames.currency }, { 104, PdfGlyphNames.quotesingle }, { 105, PdfGlyphNames.quotedblleft }, 
			{ 106, PdfGlyphNames.guillemotleft }, { 107, PdfGlyphNames.guilsinglleft }, { 108, PdfGlyphNames.guilsinglright }, { 109, PdfGlyphNames.fi }, { 110, PdfGlyphNames.fl }, 
			{ 111, PdfGlyphNames.endash }, { 112, PdfGlyphNames.dagger }, { 113, PdfGlyphNames.daggerdbl }, { 114, PdfGlyphNames.periodcentered }, { 115, PdfGlyphNames.paragraph }, 
			{ 116, PdfGlyphNames.bullet }, { 117, PdfGlyphNames.quotesinglbase }, { 118, PdfGlyphNames.quotedblbase }, { 119, PdfGlyphNames.quotedblright }, { 120, PdfGlyphNames.guillemotright }, 
			{ 121, PdfGlyphNames.ellipsis }, { 122, PdfGlyphNames.perthousand }, { 123, PdfGlyphNames.questiondown }, { 124, PdfGlyphNames.grave }, { 125, PdfGlyphNames.acute }, 
			{ 126, PdfGlyphNames.circumflex }, { 127, PdfGlyphNames.tilde }, { 128, PdfGlyphNames.macron }, { 129, PdfGlyphNames.breve }, { 130, PdfGlyphNames.dotaccent }, 
			{ 131, PdfGlyphNames.dieresis }, { 132, PdfGlyphNames.ring }, { 133, PdfGlyphNames.cedilla }, { 134, PdfGlyphNames.hungarumlaut }, { 135, PdfGlyphNames.ogonek }, 
			{ 136, PdfGlyphNames.caron }, { 137, PdfGlyphNames.emdash }, { 138, PdfGlyphNames.AE }, { 139, PdfGlyphNames.ordfeminine }, { 140, PdfGlyphNames.Lslash }, { 141, PdfGlyphNames.Oslash }, 
			{ 142, PdfGlyphNames.OE }, { 143, PdfGlyphNames.ordmasculine }, { 144, PdfGlyphNames.ae }, { 145, PdfGlyphNames.dotlessi }, { 146, PdfGlyphNames.lslash }, { 147, PdfGlyphNames.oslash }, 
			{ 148, PdfGlyphNames.oe }, { 149, PdfGlyphNames.germandbls }, { 150, PdfGlyphNames.onesuperior }, { 151, PdfGlyphNames.logicalnot }, { 152, PdfGlyphNames.mu }, 
			{ 153, PdfGlyphNames.trademark }, { 154, PdfGlyphNames.Eth }, { 155, PdfGlyphNames.onehalf }, { 156, PdfGlyphNames.plusminus }, { 157, PdfGlyphNames.Thorn }, 
			{ 158, PdfGlyphNames.onequarter }, { 159, PdfGlyphNames.divide }, { 160, PdfGlyphNames.brokenbar }, { 161, PdfGlyphNames.degree }, { 162, PdfGlyphNames.thorn }, 
			{ 163, PdfGlyphNames.threequarters }, { 164, PdfGlyphNames.twosuperior }, { 165, PdfGlyphNames.registered }, { 166, PdfGlyphNames.minus }, { 167, PdfGlyphNames.eth }, 
			{ 168, PdfGlyphNames.multiply }, { 169, PdfGlyphNames.threesuperior }, { 170, PdfGlyphNames.copyright }, { 171, PdfGlyphNames.Aacute }, { 172, PdfGlyphNames.Acircumflex }, 
			{ 173, PdfGlyphNames.Adieresis }, { 174, PdfGlyphNames.Agrave }, { 175, PdfGlyphNames.Aring }, { 176, PdfGlyphNames.Atilde }, { 177, PdfGlyphNames.Ccedilla }, { 178, PdfGlyphNames.Eacute },
			{ 179, PdfGlyphNames.Ecircumflex }, { 180, PdfGlyphNames.Edieresis }, { 181, PdfGlyphNames.Egrave }, { 182, PdfGlyphNames.Iacute }, { 183, PdfGlyphNames.Icircumflex }, 
			{ 184, PdfGlyphNames.Idieresis }, { 185, PdfGlyphNames.Igrave }, { 186, PdfGlyphNames.Ntilde }, { 187, PdfGlyphNames.Oacute }, { 188, PdfGlyphNames.Ocircumflex }, 
			{ 189, PdfGlyphNames.Odieresis }, { 190, PdfGlyphNames.Ograve }, { 191, PdfGlyphNames.Otilde }, { 192, PdfGlyphNames.Scaron }, { 193, PdfGlyphNames.Uacute }, 
			{ 194, PdfGlyphNames.Ucircumflex }, { 195, PdfGlyphNames.Udieresis }, { 196, PdfGlyphNames.Ugrave }, { 197, PdfGlyphNames.Yacute }, { 198, PdfGlyphNames.Ydieresis }, 
			{ 199, PdfGlyphNames.Zcaron }, { 200, PdfGlyphNames.aacute }, { 201, PdfGlyphNames.acircumflex }, { 202, PdfGlyphNames.adieresis }, { 203, PdfGlyphNames.agrave }, 
			{ 204, PdfGlyphNames.aring }, { 205, PdfGlyphNames.atilde }, { 206, PdfGlyphNames.ccedilla }, { 207, PdfGlyphNames.eacute }, { 208, PdfGlyphNames.ecircumflex }, 
			{ 209, PdfGlyphNames.edieresis }, { 210, PdfGlyphNames.egrave }, { 211, PdfGlyphNames.iacute }, { 212, PdfGlyphNames.icircumflex }, { 213, PdfGlyphNames.idieresis }, 
			{ 214, PdfGlyphNames.igrave }, { 215, PdfGlyphNames.ntilde }, { 216, PdfGlyphNames.oacute }, { 217, PdfGlyphNames.ocircumflex }, { 218, PdfGlyphNames.odieresis }, 
			{ 219, PdfGlyphNames.ograve }, { 220, PdfGlyphNames.otilde }, { 221, PdfGlyphNames.scaron }, { 222, PdfGlyphNames.uacute }, { 223, PdfGlyphNames.ucircumflex }, 
			{ 224, PdfGlyphNames.udieresis }, { 225, PdfGlyphNames.ugrave }, { 226, PdfGlyphNames.yacute }, { 227, PdfGlyphNames.ydieresis }, { 228, PdfGlyphNames.zcaron }, 
			{ 229, PdfGlyphNames.exclamsmall }, { 230, PdfGlyphNames.Hungarumlautsmall }, { 231, PdfGlyphNames.dollaroldstyle }, { 232, PdfGlyphNames.dollarsuperior }, 
			{ 233, PdfGlyphNames.ampersandsmall }, { 234, PdfGlyphNames.Acutesmall }, { 235, PdfGlyphNames.parenleftsuperior }, { 236, PdfGlyphNames.parenrightsuperior }, 
			{ 237, PdfGlyphNames.twodotenleader }, { 238, PdfGlyphNames.onedotenleader }, { 239, PdfGlyphNames.zerooldstyle }, { 240, PdfGlyphNames.oneoldstyle }, { 241, PdfGlyphNames.twooldstyle },
			{ 242, PdfGlyphNames.threeoldstyle }, { 243, PdfGlyphNames.fouroldstyle }, { 244, PdfGlyphNames.fiveoldstyle }, { 245, PdfGlyphNames.sixoldstyle }, { 246, PdfGlyphNames.sevenoldstyle }, 
			{ 247, PdfGlyphNames.eightoldstyle }, { 248, PdfGlyphNames.nineoldstyle }, { 249, PdfGlyphNames.commasuperior }, { 250, PdfGlyphNames.threequartersemdash }, 
			{ 251, PdfGlyphNames.periodsuperior }, { 252, PdfGlyphNames.questionsmall }, { 253, PdfGlyphNames.asuperior }, { 254, PdfGlyphNames.bsuperior }, { 255, PdfGlyphNames.centsuperior }, 
			{ 256, PdfGlyphNames.dsuperior }, { 257, PdfGlyphNames.esuperior }, { 258, PdfGlyphNames.isuperior }, { 259, PdfGlyphNames.lsuperior }, { 260, PdfGlyphNames.msuperior }, 
			{ 261, PdfGlyphNames.nsuperior }, { 262, PdfGlyphNames.osuperior }, { 263, PdfGlyphNames.rsuperior }, { 264, PdfGlyphNames.ssuperior }, { 265, PdfGlyphNames.tsuperior }, 
			{ 266, PdfGlyphNames.ff }, { 267, PdfGlyphNames.ffi }, { 268, PdfGlyphNames.ffl }, { 269, PdfGlyphNames.parenleftinferior }, { 270, PdfGlyphNames.parenrightinferior }, 
			{ 271, PdfGlyphNames.Circumflexsmall }, { 272, PdfGlyphNames.hyphensuperior }, { 273, PdfGlyphNames.Gravesmall }, { 274, PdfGlyphNames.Asmall }, { 275, PdfGlyphNames.Bsmall }, 
			{ 276, PdfGlyphNames.Csmall }, { 277, PdfGlyphNames.Dsmall }, { 278, PdfGlyphNames.Esmall }, { 279, PdfGlyphNames.Fsmall }, { 280, PdfGlyphNames.Gsmall }, { 281, PdfGlyphNames.Hsmall }, 
			{ 282, PdfGlyphNames.Ismall }, { 283, PdfGlyphNames.Jsmall }, { 284, PdfGlyphNames.Ksmall }, { 285, PdfGlyphNames.Lsmall }, { 286, PdfGlyphNames.Msmall }, { 287, PdfGlyphNames.Nsmall },
			{ 288, PdfGlyphNames.Osmall }, { 289, PdfGlyphNames.Psmall }, { 290, PdfGlyphNames.Qsmall }, { 291, PdfGlyphNames.Rsmall }, { 292, PdfGlyphNames.Ssmall }, { 293, PdfGlyphNames.Tsmall }, 
			{ 294, PdfGlyphNames.Usmall }, { 295, PdfGlyphNames.Vsmall }, { 296, PdfGlyphNames.Wsmall }, { 297, PdfGlyphNames.Xsmall }, { 298, PdfGlyphNames.Ysmall }, { 299, PdfGlyphNames.Zsmall }, 
			{ 300, PdfGlyphNames.colonmonetary }, { 301, PdfGlyphNames.onefitted }, { 302, PdfGlyphNames.rupiah }, { 303, PdfGlyphNames.Tildesmall }, { 304, PdfGlyphNames.exclamdownsmall }, 
			{ 305, PdfGlyphNames.centoldstyle }, { 306, PdfGlyphNames.Lslashsmall }, { 307, PdfGlyphNames.Scaronsmall }, { 308, PdfGlyphNames.Zcaronsmall }, { 309, PdfGlyphNames.Dieresissmall }, 
			{ 310, PdfGlyphNames.Brevesmall }, { 311, PdfGlyphNames.Caronsmall }, { 312, PdfGlyphNames.Dotaccentsmall }, { 313, PdfGlyphNames.Macronsmall }, { 314, PdfGlyphNames.figuredash }, 
			{ 315, PdfGlyphNames.hypheninferior }, { 316, PdfGlyphNames.Ogoneksmall }, { 317, PdfGlyphNames.Ringsmall }, { 318, PdfGlyphNames.Cedillasmall }, { 319, PdfGlyphNames.questiondownsmall },
			{ 320, PdfGlyphNames.oneeighth }, { 321, PdfGlyphNames.threeeighths }, { 322, PdfGlyphNames.fiveeighths }, { 323, PdfGlyphNames.seveneighths }, { 324, PdfGlyphNames.onethird }, 
			{ 325, PdfGlyphNames.twothirds }, { 326, PdfGlyphNames.zerosuperior }, { 327, PdfGlyphNames.foursuperior }, { 328, PdfGlyphNames.fivesuperior }, { 329, PdfGlyphNames.sixsuperior }, 
			{ 330, PdfGlyphNames.sevensuperior }, { 331, PdfGlyphNames.eightsuperior }, { 332, PdfGlyphNames.ninesuperior }, { 333, PdfGlyphNames.zeroinferior }, { 334, PdfGlyphNames.oneinferior }, 
			{ 335, PdfGlyphNames.twoinferior }, { 336, PdfGlyphNames.threeinferior }, { 337, PdfGlyphNames.fourinferior }, { 338, PdfGlyphNames.fiveinferior }, { 339, PdfGlyphNames.sixinferior }, 
			{ 340, PdfGlyphNames.seveninferior }, { 341, PdfGlyphNames.eightinferior }, { 342, PdfGlyphNames.nineinferior }, { 343, PdfGlyphNames.centinferior }, { 344, PdfGlyphNames.dollarinferior },
			{ 345, PdfGlyphNames.periodinferior }, { 346, PdfGlyphNames.commainferior }, { 347, PdfGlyphNames.Agravesmall }, { 348, PdfGlyphNames.Aacutesmall }, { 349, PdfGlyphNames.Acircumflexsmall },
			{ 350, PdfGlyphNames.Atildesmall }, { 351, PdfGlyphNames.Adieresissmall }, { 352, PdfGlyphNames.Aringsmall }, { 353, PdfGlyphNames.AEsmall }, { 354, PdfGlyphNames.Ccedillasmall }, 
			{ 355, PdfGlyphNames.Egravesmall }, { 356, PdfGlyphNames.Eacutesmall }, { 357, PdfGlyphNames.Ecircumflexsmall }, { 358, PdfGlyphNames.Edieresissmall }, { 359, PdfGlyphNames.Igravesmall },
			{ 360, PdfGlyphNames.Iacutesmall }, { 361, PdfGlyphNames.Icircumflexsmall }, { 362, PdfGlyphNames.Idieresissmall }, { 363, PdfGlyphNames.Ethsmall }, { 364, PdfGlyphNames.Ntildesmall },
			{ 365, PdfGlyphNames.Ogravesmall }, { 366, PdfGlyphNames.Oacutesmall }, { 367, PdfGlyphNames.Ocircumflexsmall }, { 368, PdfGlyphNames.Otildesmall }, { 369, PdfGlyphNames.Odieresissmall }, 
			{ 370, PdfGlyphNames.OEsmall }, { 371, PdfGlyphNames.Oslashsmall }, { 372, PdfGlyphNames.Ugravesmall }, { 373, PdfGlyphNames.Uacutesmall }, { 374, PdfGlyphNames.Ucircumflexsmall }, 
			{ 375, PdfGlyphNames.Udieresissmall }, { 376, PdfGlyphNames.Yacutesmall }, { 377, PdfGlyphNames.Thornsmall }, { 378, PdfGlyphNames.Ydieresissmall }, { 379, PdfGlyphNames._001_000 }, 
			{ 380, PdfGlyphNames._001_001 }, { 381, PdfGlyphNames._001_002 }, { 382, PdfGlyphNames._001_003 }, { 383, PdfGlyphNames.Black }, { 384, PdfGlyphNames.Bold }, { 385, PdfGlyphNames.Book }, 
			{ 386, PdfGlyphNames.Light }, { 387, PdfGlyphNames.Medium }, { 388, PdfGlyphNames.Regular }, { 389, PdfGlyphNames.Roman }, { 390, PdfGlyphNames.Semibold } 
		};
		static IList<short> ReadRangeFormat(PdfBinaryStream stream, int glyphCount, bool twoBytes) {
			List<short> charset = new List<short>(glyphCount);
			long index = 0;
			while (index < glyphCount) {
				short first = stream.ReadShort();
				short count = (short)((twoBytes ? stream.ReadShort() : stream.ReadByte()) + 1);
				for (short i = 0, sid = first; i < count; i++, sid++)
					charset.Add(sid);
				index += count;
			}
			return charset;
		}
		public static PdfCompactFontFormatEncoding GetEncoding(byte[] cff) {
			using (PdfBinaryStream stream = new PdfBinaryStream(cff)) {
				stream.ReadShort();
				int headerSize = stream.ReadByte();
				stream.Position = headerSize;
				new PdfCompactFontFormatIndex(stream);
				PdfCompactFontFormatTopDictIndex topDictIndex = new PdfCompactFontFormatTopDictIndex(stream);
				PdfCompactFontFormatStringIndex stringIndex = new PdfCompactFontFormatStringIndex(stream);
				IList<short> charset;
				int charSetOffset = topDictIndex.CharSet;
				switch (charSetOffset) {
					case 0:
						return new PdfCompactFontFormatEncoding(topDictIndex.IsIdentityEncoding, isoAdobeEncoding);
					case 1:
						return new PdfCompactFontFormatEncoding(topDictIndex.IsIdentityEncoding, expertEncoding);
					case 2:
						return new PdfCompactFontFormatEncoding(topDictIndex.IsIdentityEncoding, expertSubsetEncoding);
					default:
						int charStrings = topDictIndex.CharStrings;
						if (charStrings == 0)
							return new PdfCompactFontFormatEncoding(topDictIndex.IsIdentityEncoding, isoAdobeEncoding);
						stream.Position = charStrings;
						int glyphCount = new PdfCompactFontFormatIndex(stream).Objects.Count - 1;
						if (glyphCount <= 0)
							return new PdfCompactFontFormatEncoding(topDictIndex.IsIdentityEncoding, isoAdobeEncoding);
						stream.Position = charSetOffset;
						switch (stream.ReadByte()) {
							case 1:
								charset = ReadRangeFormat(stream, glyphCount, false);
								break;
							case 2:
								charset = ReadRangeFormat(stream, glyphCount, true);
								break;
							default:
								charset = new List<short>(glyphCount);
								for (int i = 0; i < glyphCount; i++) 
									charset.Add(stream.ReadShort());
								break;
						}
						Dictionary<string, short> encoding = new Dictionary<string, short>();
						if (topDictIndex.IsIdentityEncoding) 
							foreach (short sid in charset)
								encoding.Add(sid.ToString(), sid);
						else  
							try {
								string[] strings = stringIndex.Strings;
								short standartGlyphNamesCount = (short)standartGlyphNames.Count;
								foreach (short sid in charset) {
									short customSid = (short)(sid - standartGlyphNamesCount);
									encoding.Add(customSid >= 0 ? strings[customSid] : standartGlyphNames[sid], (short)(sid + 31));				
								}
							}
							catch {
								return new PdfCompactFontFormatEncoding(topDictIndex.IsIdentityEncoding, isoAdobeEncoding);
							}
						return new PdfCompactFontFormatEncoding(topDictIndex.IsIdentityEncoding, encoding);
				}
			}
		}
	}
}
