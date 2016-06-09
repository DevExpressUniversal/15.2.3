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

namespace DevExpress.Pdf.Native {
	public class PdfPostScriptStandardEncodingOperator : PdfPostScriptOperator {
		public const string Token = "StandardEncoding";
		static readonly object[] array = new object[] { new PdfName(PdfGlyphNames._notdef), new PdfName(PdfGlyphNames._notdef), new PdfName(PdfGlyphNames._notdef), new PdfName(PdfGlyphNames._notdef), 
														new PdfName(PdfGlyphNames._notdef), new PdfName(PdfGlyphNames._notdef), new PdfName(PdfGlyphNames._notdef), new PdfName(PdfGlyphNames._notdef), 
														new PdfName(PdfGlyphNames._notdef), new PdfName(PdfGlyphNames._notdef), new PdfName(PdfGlyphNames._notdef), new PdfName(PdfGlyphNames._notdef), 
														new PdfName(PdfGlyphNames._notdef), new PdfName(PdfGlyphNames._notdef), new PdfName(PdfGlyphNames._notdef), new PdfName(PdfGlyphNames._notdef), 
														new PdfName(PdfGlyphNames._notdef), new PdfName(PdfGlyphNames._notdef), new PdfName(PdfGlyphNames._notdef), new PdfName(PdfGlyphNames._notdef), 
														new PdfName(PdfGlyphNames._notdef), new PdfName(PdfGlyphNames._notdef), new PdfName(PdfGlyphNames._notdef), new PdfName(PdfGlyphNames._notdef), 
														new PdfName(PdfGlyphNames._notdef), new PdfName(PdfGlyphNames._notdef), new PdfName(PdfGlyphNames._notdef), new PdfName(PdfGlyphNames._notdef), 
														new PdfName(PdfGlyphNames._notdef), new PdfName(PdfGlyphNames._notdef), new PdfName(PdfGlyphNames._notdef), new PdfName(PdfGlyphNames._notdef), 
														new PdfName(PdfGlyphNames.space), new PdfName(PdfGlyphNames.exclam), new PdfName(PdfGlyphNames.quotedbl), new PdfName(PdfGlyphNames.numbersign),
														new PdfName(PdfGlyphNames.dollar), new PdfName(PdfGlyphNames.percent), new PdfName(PdfGlyphNames.ampersand), new PdfName(PdfGlyphNames.quoteright),
														new PdfName(PdfGlyphNames.parenleft), new PdfName(PdfGlyphNames.parenright), new PdfName(PdfGlyphNames.asterisk), new PdfName(PdfGlyphNames.plus),
														new PdfName(PdfGlyphNames.comma), new PdfName(PdfGlyphNames.hyphen), new PdfName(PdfGlyphNames.period), new PdfName(PdfGlyphNames.slash), 
														new PdfName(PdfGlyphNames.zero), new PdfName(PdfGlyphNames.one), new PdfName(PdfGlyphNames.two), new PdfName(PdfGlyphNames.three), 
														new PdfName(PdfGlyphNames.four), new PdfName(PdfGlyphNames.five), new PdfName(PdfGlyphNames.six), new PdfName(PdfGlyphNames.seven),
														new PdfName(PdfGlyphNames.eight), new PdfName(PdfGlyphNames.nine), new PdfName(PdfGlyphNames.colon), new PdfName(PdfGlyphNames.semicolon),
														new PdfName(PdfGlyphNames.less), new PdfName(PdfGlyphNames.equal), new PdfName(PdfGlyphNames.greater), new PdfName(PdfGlyphNames.question),
														new PdfName(PdfGlyphNames.at), new PdfName(PdfGlyphNames.A), new PdfName(PdfGlyphNames.B), new PdfName(PdfGlyphNames.C), new PdfName(PdfGlyphNames.D),
														new PdfName(PdfGlyphNames.E), new PdfName(PdfGlyphNames.F), new PdfName(PdfGlyphNames.G), new PdfName(PdfGlyphNames.H), new PdfName(PdfGlyphNames.I),
														new PdfName(PdfGlyphNames.J), new PdfName(PdfGlyphNames.K), new PdfName(PdfGlyphNames.L), new PdfName(PdfGlyphNames.M), new PdfName(PdfGlyphNames.N),
														new PdfName(PdfGlyphNames.O), new PdfName(PdfGlyphNames.P), new PdfName(PdfGlyphNames.Q), new PdfName(PdfGlyphNames.R), new PdfName(PdfGlyphNames.S),
														new PdfName(PdfGlyphNames.T), new PdfName(PdfGlyphNames.U), new PdfName(PdfGlyphNames.V), new PdfName(PdfGlyphNames.W), new PdfName(PdfGlyphNames.X),
														new PdfName(PdfGlyphNames.Y), new PdfName(PdfGlyphNames.Z), new PdfName(PdfGlyphNames.bracketleft), new PdfName(PdfGlyphNames.backslash),
														new PdfName(PdfGlyphNames.bracketright), new PdfName(PdfGlyphNames.asciicircum), new PdfName(PdfGlyphNames.underscore), new PdfName(PdfGlyphNames.quoteleft), 
														new PdfName(PdfGlyphNames.a), new PdfName(PdfGlyphNames.b), new PdfName(PdfGlyphNames.c), new PdfName(PdfGlyphNames.d), new PdfName(PdfGlyphNames.e),
														new PdfName(PdfGlyphNames.f), new PdfName(PdfGlyphNames.g), new PdfName(PdfGlyphNames.h), new PdfName(PdfGlyphNames.i), new PdfName(PdfGlyphNames.j),
														new PdfName(PdfGlyphNames.k), new PdfName(PdfGlyphNames.l), new PdfName(PdfGlyphNames.m), new PdfName(PdfGlyphNames.n), new PdfName(PdfGlyphNames.o),
														new PdfName(PdfGlyphNames.p), new PdfName(PdfGlyphNames.q), new PdfName(PdfGlyphNames.r), new PdfName(PdfGlyphNames.s), new PdfName(PdfGlyphNames.t),
														new PdfName(PdfGlyphNames.u), new PdfName(PdfGlyphNames.v), new PdfName(PdfGlyphNames.w), new PdfName(PdfGlyphNames.x), new PdfName(PdfGlyphNames.y),
														new PdfName(PdfGlyphNames.z), new PdfName(PdfGlyphNames.braceleft), new PdfName(PdfGlyphNames.bar), new PdfName(PdfGlyphNames.braceright),
														new PdfName(PdfGlyphNames.asciitilde), new PdfName(PdfGlyphNames._notdef), new PdfName(PdfGlyphNames._notdef), new PdfName(PdfGlyphNames._notdef), 
														new PdfName(PdfGlyphNames._notdef), new PdfName(PdfGlyphNames._notdef), new PdfName(PdfGlyphNames._notdef), new PdfName(PdfGlyphNames._notdef), 
														new PdfName(PdfGlyphNames._notdef), new PdfName(PdfGlyphNames._notdef), new PdfName(PdfGlyphNames._notdef), new PdfName(PdfGlyphNames._notdef), 
														new PdfName(PdfGlyphNames._notdef), new PdfName(PdfGlyphNames._notdef), new PdfName(PdfGlyphNames._notdef), new PdfName(PdfGlyphNames._notdef), 
														new PdfName(PdfGlyphNames._notdef), new PdfName(PdfGlyphNames._notdef), new PdfName(PdfGlyphNames._notdef), new PdfName(PdfGlyphNames._notdef), 
														new PdfName(PdfGlyphNames._notdef), new PdfName(PdfGlyphNames._notdef), new PdfName(PdfGlyphNames._notdef), new PdfName(PdfGlyphNames._notdef), 
														new PdfName(PdfGlyphNames._notdef), new PdfName(PdfGlyphNames._notdef), new PdfName(PdfGlyphNames._notdef), new PdfName(PdfGlyphNames._notdef), 
														new PdfName(PdfGlyphNames._notdef), new PdfName(PdfGlyphNames._notdef), new PdfName(PdfGlyphNames._notdef), new PdfName(PdfGlyphNames._notdef), 
														new PdfName(PdfGlyphNames._notdef), new PdfName(PdfGlyphNames._notdef), new PdfName(PdfGlyphNames._notdef), new PdfName(PdfGlyphNames.exclamdown),
														new PdfName(PdfGlyphNames.cent), new PdfName(PdfGlyphNames.sterling), new PdfName(PdfGlyphNames.fraction), new PdfName(PdfGlyphNames.yen),
														new PdfName(PdfGlyphNames.florin), new PdfName(PdfGlyphNames.section), new PdfName(PdfGlyphNames.currency), new PdfName(PdfGlyphNames.quotesingle),
														new PdfName(PdfGlyphNames.quotedblleft), new PdfName(PdfGlyphNames.guillemotleft), new PdfName(PdfGlyphNames.guilsinglleft), 
														new PdfName(PdfGlyphNames.guilsinglright), new PdfName(PdfGlyphNames.fi), new PdfName(PdfGlyphNames.fl), new PdfName(PdfGlyphNames._notdef),
														new PdfName(PdfGlyphNames.endash), new PdfName(PdfGlyphNames.dagger), new PdfName(PdfGlyphNames.daggerdbl), new PdfName(PdfGlyphNames.periodcentered),
														new PdfName(PdfGlyphNames._notdef), new PdfName(PdfGlyphNames.paragraph), new PdfName(PdfGlyphNames.bullet), new PdfName(PdfGlyphNames.quotesinglbase),
														new PdfName(PdfGlyphNames.quotedblbase), new PdfName(PdfGlyphNames.quotedblright), new PdfName(PdfGlyphNames.guillemotright), 
														new PdfName(PdfGlyphNames.ellipsis), new PdfName(PdfGlyphNames.perthousand), new PdfName(PdfGlyphNames._notdef), new PdfName(PdfGlyphNames.questiondown),
														new PdfName(PdfGlyphNames._notdef), new PdfName(PdfGlyphNames.grave), new PdfName(PdfGlyphNames.acute), new PdfName(PdfGlyphNames.circumflex),
														new PdfName(PdfGlyphNames.tilde), new PdfName(PdfGlyphNames.macron), new PdfName(PdfGlyphNames.breve), new PdfName(PdfGlyphNames.dotaccent),
														new PdfName(PdfGlyphNames.dieresis), new PdfName(PdfGlyphNames._notdef), new PdfName(PdfGlyphNames.degree), new PdfName(PdfGlyphNames.cedilla),
														new PdfName(PdfGlyphNames._notdef), new PdfName(PdfGlyphNames.hungarumlaut), new PdfName(PdfGlyphNames.ogonek), new PdfName(PdfGlyphNames.caron),
														new PdfName(PdfGlyphNames.emdash), new PdfName(PdfGlyphNames._notdef), new PdfName(PdfGlyphNames._notdef), new PdfName(PdfGlyphNames._notdef),
														new PdfName(PdfGlyphNames._notdef), new PdfName(PdfGlyphNames._notdef), new PdfName(PdfGlyphNames._notdef), new PdfName(PdfGlyphNames._notdef), 
														new PdfName(PdfGlyphNames._notdef), new PdfName(PdfGlyphNames._notdef), new PdfName(PdfGlyphNames._notdef), new PdfName(PdfGlyphNames._notdef), 
														new PdfName(PdfGlyphNames._notdef), new PdfName(PdfGlyphNames.AE), new PdfName(PdfGlyphNames._notdef), new PdfName(PdfGlyphNames.ordfeminine),
														new PdfName(PdfGlyphNames._notdef), new PdfName(PdfGlyphNames._notdef), new PdfName(PdfGlyphNames._notdef), new PdfName(PdfGlyphNames._notdef), 
														new PdfName(PdfGlyphNames.Lslash), new PdfName(PdfGlyphNames.Oslash), new PdfName(PdfGlyphNames.OE), new PdfName(PdfGlyphNames.ordmasculine),
														new PdfName(PdfGlyphNames._notdef), new PdfName(PdfGlyphNames._notdef), new PdfName(PdfGlyphNames._notdef), new PdfName(PdfGlyphNames._notdef), 
														new PdfName(PdfGlyphNames._notdef), new PdfName(PdfGlyphNames.ae), new PdfName(PdfGlyphNames._notdef), new PdfName(PdfGlyphNames._notdef), 
														new PdfName(PdfGlyphNames._notdef), new PdfName(PdfGlyphNames.onesuperior), new PdfName(PdfGlyphNames._notdef), new PdfName(PdfGlyphNames._notdef), 
														new PdfName(PdfGlyphNames.lslash), new PdfName(PdfGlyphNames.oslash), new PdfName(PdfGlyphNames.oe), new PdfName(PdfGlyphNames.germandbls),
														new PdfName(PdfGlyphNames._notdef), new PdfName(PdfGlyphNames._notdef), new PdfName(PdfGlyphNames._notdef), new PdfName(PdfGlyphNames._notdef) };
		public override void Execute(PdfPostScriptInterpreter interpreter) {
			interpreter.Stack.Push(array);
		}
	}
}
