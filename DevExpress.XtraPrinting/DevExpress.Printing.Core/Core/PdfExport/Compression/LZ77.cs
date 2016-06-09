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
using System.Collections;
namespace DevExpress.XtraPrinting.Export.Pdf.Compression {
	public interface ILZ77 {
		void Reset();
		void Reset(byte[] input);
		bool Next(LZ77ResultValue resultValue);
	}
	public class LZ77ResultValue {
		public byte Literal;
		public int Offset;
		public int Length;
		public bool IsLiteral;
	}
	public class LZ77DefaultAlgorithm : ILZ77 {
		int windowSize;
		int dictionarySize;
		int position;
		byte[] input;
		const int minLength = 3;
		const int maxLength = 258;
		const int maxOffset = 1 << 15;
		int[] literalOffsets;
		public LZ77DefaultAlgorithm(byte[] input) : this(input, 15) {
		}
		public LZ77DefaultAlgorithm(byte[] input, int windowSizeExponent) {
			if(windowSizeExponent < 8 || windowSizeExponent > 15)
				windowSizeExponent = 15;
			windowSize = 1 << windowSizeExponent;
			dictionarySize = windowSize - maxLength;
			Reset(input);
		}
		int CreateLiteralOffsets(byte literal) {
			int count = (dictionarySize < position) ? dictionarySize: position;
			UpdateLiteralOfssetsArray(count, true);
			int literalOffsetIndex = 0;
			for(int i = 1; i <= count; i++) {
				if(input[position - i] == literal) {
					literalOffsets[literalOffsetIndex] = i;
					literalOffsetIndex ++;
				}
			}
			return literalOffsetIndex;
		}
		int GetMatchLength(int offset) {
			int inputTail = input.Length - position;
			int result = 1;
			int count = (maxLength < inputTail) ? maxLength : inputTail;
			int leftPosition = position - offset;
			for(int i = 1; i < count; i++) {
				if(input[leftPosition + i] != input[position + i])
					break;
				result++;
			}			
			return result;
		}
		void UpdateWindow(int length) {
			position += length;
		}
		void UpdateLiteralOfssetsArray(int newLength, bool failOnRecreate) {
			if(literalOffsets == null || literalOffsets.Length < newLength) {
				literalOffsets = new int[newLength]; 
				if(failOnRecreate)
					System.Diagnostics.Debug.Assert(false, "Unexpected recreating literalOffsets array in LZ77DefaultAlgorithm");
			}
		}
		#region ILZ77 implementation
		public void Reset() {
			position = 0;
		}
		public void Reset(byte[] input) {
			this.input = input;
			UpdateLiteralOfssetsArray(input.Length, false);
			Reset();
		}
		public bool Next(LZ77ResultValue resultValue) {
			if(resultValue == null) return false;
			if(position < input.Length) {
				byte literal = input[position];
				int literalOffsetsCount = CreateLiteralOffsets(literal);
				if(literalOffsetsCount > 0) {
					int offset = 0;
					int length = 0;
					for(int i = 0; i < literalOffsetsCount; i++) {
						int matchLength = GetMatchLength(literalOffsets[i]); 
						if(matchLength > length) {
							offset = (int)literalOffsets[i];
							length = matchLength;
							if(length > maxLength) {
								length = maxLength;
								break;
							}
						}
					}
					if(length < minLength) 
						length = 1;
					if(length > 1) {
						if(offset > maxOffset)
							throw new CompressException("The phrase offset is out of bounds");
						resultValue.Offset = offset;
						resultValue.Length = length;
						resultValue.IsLiteral = false;
					} else {
						resultValue.Literal = literal;
						resultValue.IsLiteral = true;
					}
					UpdateWindow(length);
				} else {
					resultValue.Literal = literal;
					resultValue.IsLiteral = true;
					UpdateWindow(1);
				}
				return true;
			} else
				return false;
		}
		#endregion
	}
}
