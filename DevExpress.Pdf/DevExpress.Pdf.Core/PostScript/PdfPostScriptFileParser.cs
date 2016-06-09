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
using System.IO;
namespace DevExpress.Pdf.Native {
	public class PdfPostScriptFileParser : PdfObjectParser {
		const byte beginProcedure = (byte)'{';
		const byte endProcedure = (byte)'}';
		const byte radixNumberIdentifier = (byte)'#';
		const byte zero = (byte)'0';
		const byte one = (byte)'1';
		const byte seven = (byte)'7';
		public static IList<object> Parse(byte[] data, int dataLength) {
			PdfPostScriptFileParser parser = new PdfPostScriptFileParser(new PdfArrayData(data, dataLength), 0);
			parser.Parse();
			return parser.operators;
		}
		public static IList<object> Parse(byte[] data) {
			return Parse(data, data.Length);
		}
		readonly List<object> operators;
		readonly bool shouldExpectClosing;
		bool closed;
		protected override bool CanContinueReading { 
			get { 
				byte current = Current;
				return base.CanContinueReading && current != beginProcedure && current != endProcedure; 
			} 
		}
		PdfPostScriptFileParser(PdfData data, int position) : base(data, position) {
			shouldExpectClosing = position != 0;
			operators = new List<object>();
		}
		public PdfPostScriptFileParser(byte[] data) : base(new PdfArrayData(data, data.Length), 0) {
		}
		public object ReadNextObject() {
			object obj = ReadObject(false, false);
			return obj == null ? null : PdfPostScriptOperator.Parse(obj);
		}
		void Parse() {
			for (;;) {
				object obj = ReadNextObject();
				if (obj == null)
					break;
				operators.Add(obj);
			}
			if (shouldExpectClosing && !closed)
				PdfDocumentReader.ThrowIncorrectDataException();
		}
		int ReadRadixNumber(Func<byte, bool> checkByte, Func<int, byte, int> accumulate) {
			int value = 0;
			while (ReadNext()) {
				byte current = Current;
				if (!checkByte(current))
					break;
				value = accumulate(value, current);			
			}
			return value;
		}
		protected override object ReadNumericObject() {
			object value = base.ReadNumericObject();
			if (Current == radixNumberIdentifier && (value is int))
				switch ((int)value) {
					case 2:
						return ReadRadixNumber(current => (current == zero || current == one), (result, current) => result * 2 + ConvertToDigit(current));
					case 8:
						return ReadRadixNumber(current => (current >= zero && current <= seven), (result, current) => result * 8 + ConvertToDigit(current));
					case 16:
						return ReadRadixNumber(current => IsHexadecimalDigitSymbol(current), (result, current) => result * 16 + ConvertToHexadecimalDigit(current));
				}
			return value;
		}
		protected override object ReadAlphabeticalObject(bool isHexadecimalStringSeparatedUsingWhiteSpaces, bool isIndirect) {
			switch (Current) {
				case beginProcedure:
					PdfPostScriptFileParser parser = new PdfPostScriptFileParser(Data, CurrentPosition + 1);
					parser.Parse();
					CurrentPosition = parser.CurrentPosition + 1;
					return parser.operators;
				case endProcedure:
					if (!shouldExpectClosing)
						PdfDocumentReader.ThrowIncorrectDataException();
					closed = true;
					return null;
				default:
					return base.ReadAlphabeticalObject(isHexadecimalStringSeparatedUsingWhiteSpaces, isIndirect);
			}
		}
	}
}
