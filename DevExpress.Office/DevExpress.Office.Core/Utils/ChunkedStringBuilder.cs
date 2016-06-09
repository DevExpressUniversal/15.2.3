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
using System.Globalization;
using System.IO;
using System.Text;
namespace DevExpress.Office.Utils {
	#region ChunkedStringBuilder
	public class ChunkedStringBuilder : IStringBuilder {
		public const int DefaultMaxBufferSize = 8192;
		int maxBufferSize = DefaultMaxBufferSize;
		List<StringBuilder> buffers = new List<StringBuilder>();
		int totalLength;
		#region IStringValueAdapter
		interface IStringValueAdapter {
			int Length { get; }
			void AppendToStringBuilder(StringBuilder stringBuilder, int startIndex, int count);
		}
		#endregion
		#region StringValueAdapter
		class StringValueAdapter : IStringValueAdapter {
			readonly string value;
			internal StringValueAdapter(string value) {
				this.value = value;
			}
			#region IStringValueAdapter Members
			public int Length { get { return value.Length; } }
			public void AppendToStringBuilder(StringBuilder stringBuilder, int startIndex, int count) {
				stringBuilder.Append(value, startIndex, count);
			}
			#endregion
		}
		#endregion
		#region CharArrayValueAdapter
		class CharArrayValueAdapter : IStringValueAdapter {
			readonly char[] value;
			internal CharArrayValueAdapter(char[] value) {
				this.value = value;
			}
			#region IStringValueAdapter Members
			public int Length { get { return value.Length; } }
			public void AppendToStringBuilder(StringBuilder stringBuilder, int startIndex, int count) {
				stringBuilder.Append(value, startIndex, count);
			}
			#endregion
		}
		#endregion
		public ChunkedStringBuilder() {
			Initialize();
		}
		public ChunkedStringBuilder(string value) {
			Initialize();
			Append(value);
		}
		protected internal int MaxBufferSize { get { return maxBufferSize; } set { maxBufferSize = value; } }
		protected internal List<StringBuilder> Buffers { get { return buffers; } }
		public int Length { get { return totalLength; } }
		public char this[int index] {
			get {
				int bufferIndex = index / MaxBufferSize;
				int offset = index % MaxBufferSize;
				return buffers[bufferIndex][offset];
			}
			set {
				int bufferIndex = index / MaxBufferSize;
				int offset = index % MaxBufferSize;
				buffers[bufferIndex][offset] = value;
			}
		}
		protected internal void Initialize() {
			int buffersCount = buffers.Count;
			if (buffersCount <= 0)
				this.buffers.Add(new StringBuilder());
			else {
				if (buffersCount > 1)
					this.buffers.RemoveRange(1, buffersCount - 1);
				this.buffers[0].Length = 0;
			}
			this.totalLength = 0;
		}
		public ChunkedStringBuilder Append(string value) {
			if (String.IsNullOrEmpty(value))
				return this;
			return Append(value, 0, value.Length);
		}
		ChunkedStringBuilder Append(IStringValueAdapter value, int startIndex, int count) {
			if (count <= 0)
				return this;
			StringBuilder buffer = buffers[buffers.Count - 1];
			int space = MaxBufferSize - buffer.Length;
			if (space >= count)
				value.AppendToStringBuilder(buffer, startIndex, count);
			else {
				int index = startIndex;
				value.AppendToStringBuilder(buffer, startIndex, space);
				index += space;
				int endIndex = startIndex + count;
				while (index < endIndex) {
					int length = Math.Min(endIndex - index, MaxBufferSize);
					buffer = new StringBuilder(MaxBufferSize);
					value.AppendToStringBuilder(buffer, index, length);
					buffers.Add(buffer);
					index += length;
				}
			}
			totalLength += count;
			return this;
		}
		public ChunkedStringBuilder Append(string value, int startIndex, int count) {
			return Append(new StringValueAdapter(value), startIndex, count);
		}
		public ChunkedStringBuilder Append(char[] value, int startIndex, int count) {
			return Append(new CharArrayValueAdapter(value), startIndex, count);
		}
		public IStringBuilder Append(char value) {
			StringBuilder buffer = buffers[buffers.Count - 1];
			int space = MaxBufferSize - buffer.Length;
			if (space >= 1)
				buffer.Append(value);
			else {
				buffer = new StringBuilder(MaxBufferSize);
				buffer.Append(value);
				buffers.Add(buffer);
			}
			totalLength++;
			return this;
		}
		public ChunkedStringBuilder Append(bool value) {
			return Append(value.ToString());
		}
		public ChunkedStringBuilder Append(byte value) {
			return Append(value.ToString(CultureInfo.CurrentCulture));
		}
		public ChunkedStringBuilder Append(decimal value) {
			return Append(value.ToString(CultureInfo.CurrentCulture));
		}
		public ChunkedStringBuilder Append(double value) {
			return Append(value.ToString(CultureInfo.CurrentCulture));
		}
		public ChunkedStringBuilder Append(short value) {
			return Append(value.ToString(CultureInfo.CurrentCulture));
		}
		public ChunkedStringBuilder Append(int value) {
			return Append(value.ToString(CultureInfo.CurrentCulture));
		}
		public ChunkedStringBuilder Append(long value) {
			return Append(value.ToString(CultureInfo.CurrentCulture));
		}
		public ChunkedStringBuilder Append(object value) {
			if (value == null)
				return this;
			return Append(value.ToString());
		}
		public ChunkedStringBuilder Append(float value) {
			return Append(value.ToString(CultureInfo.CurrentCulture));
		}
		public ChunkedStringBuilder AppendLine() {
			return Append(Environment.NewLine);
		}
		public ChunkedStringBuilder AppendLine(string value) {
			Append(value);
			return Append(Environment.NewLine);
		}
		public override string ToString() {
			StringBuilder result = new StringBuilder(Length);
			int count = buffers.Count;
			for (int i = 0; i < count; i++)
				result.Append(buffers[i].ToString());
			return result.ToString();
		}
		public string ToString(int startIndex, int length) {
			int firstBufferIndex = startIndex / MaxBufferSize;
			int firstBufferOffset = startIndex % MaxBufferSize;
			int lastBufferIndex = (startIndex + length - 1) / MaxBufferSize;
			if (firstBufferIndex == lastBufferIndex)
				return buffers[firstBufferIndex].ToString(firstBufferOffset, length);
			StringBuilder result = new StringBuilder(length);
			result.Append(buffers[firstBufferIndex].ToString(firstBufferOffset, MaxBufferSize - firstBufferOffset));
			for (int i = firstBufferIndex + 1; i < lastBufferIndex; i++)
				result.Append(buffers[i].ToString());
			int endIndex = startIndex + length;
			result.Append(buffers[lastBufferIndex].ToString(0, endIndex - lastBufferIndex * MaxBufferSize));
			return result.ToString();
		}
		public void Clear() {
			Initialize();
		}
		public void AppendExistingBuffersUnsafe(ChunkedStringBuilder stringBuilder) {
			if (stringBuilder == null || stringBuilder.Length <= 0)
				return;
			if (this.MaxBufferSize != stringBuilder.MaxBufferSize)
				Exceptions.ThrowArgumentException("stringBuilder.MaxBufferSize", stringBuilder.MaxBufferSize);
			this.Buffers.AddRange(stringBuilder.Buffers);
			this.totalLength += stringBuilder.totalLength;
			stringBuilder.Buffers.Clear(); 
			stringBuilder.Initialize(); 
		}
	}
	#endregion
	#region ChunkedStringBuilderExtensions
	public static class ChunkedStringBuilderExtensions {
		public static void Write(this Stream stream, ChunkedStringBuilder value, Encoding encoding) {
			byte[] bytes = new byte[value.MaxBufferSize * 4]; 
			List<StringBuilder> buffers = value.Buffers;
			int count = buffers.Count;
			for (int i = 0; i < count; i++) {
				string bufferContent = buffers[i].ToString();
				int byteCount = encoding.GetBytes(bufferContent, 0, bufferContent.Length, bytes, 0);
				stream.Write(bytes, 0, byteCount);
			}
		}
	}
	#endregion
}
