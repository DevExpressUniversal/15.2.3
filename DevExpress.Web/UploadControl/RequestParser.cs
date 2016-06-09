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
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
namespace DevExpress.Web {
	public enum ParserState { RawContent, PartSeparator };
	public class RequestParser : IDisposable {
		private ParserState parserState = ParserState.RawContent;
		private byte[] partSeparatorMarker = null;
		private byte[] requestEndMarker = null;
		private byte[] partHeaderEndMarker = null;
		private byte[] crlf = null;
		private MemoryStream currentHeader = null;
		private MemoryStream currentContent = null;
		private int position = 0;
		private Encoding encoding = null;
		TempFile requestTempFile = null;
		private string currentContentType = "";
		private string currentFile = "";
		public RequestParser(string partSeparatorMarker, Encoding encoding) {
			this.requestTempFile = new TempFile();
			this.encoding = encoding;
			this.partSeparatorMarker = Encoding.GetBytes(partSeparatorMarker);
			this.requestEndMarker = Encoding.GetBytes(partSeparatorMarker + "--\r\n");
			this.partHeaderEndMarker = Encoding.GetBytes("\r\n\r\n");
			this.crlf = Encoding.GetBytes("\r\n");
		}
		public TempFile RequestTempFile {
			get { return requestTempFile; }
		}
		public byte[] PartSeparatorMarker {
			get { return partSeparatorMarker; }
		}
		public byte[] PartHeaderEndMarker {
			get { return this.partHeaderEndMarker; }
		}
		public ParserState ParserState {
			get { return parserState; }
		}
		public string CurrentFile {
			get { return currentFile; }
		}
		public string CurrentContentType {
			get { return currentContentType; }
		}
		public byte[] ContentBody {
			get { return RequestTempFile.GetAllBytes(); }
		}
		protected int Position {
			get { return position; }
			set { position = value; }
		}
		protected Encoding Encoding {
			get { return encoding; }
		}
		MemoryStream CurrentHeader {
			get { return currentHeader; }
		}
		MemoryStream CurrentContent {
			get { return currentContent; }
		}
		int curHeaderStartIndex = -1;
		int curHeaderEndIndex = 0;
		public void Write(byte[] data, int length) {
			RequestTempFile.AddBytes(data, 0, length);
			byte[] dataForProcessing = RequestTempFile.GetLastBytes(data.Length + UploadProgressManager.BufferSize);
			int curHeaderEndIndexInData = 
				Math.Max(curHeaderEndIndex - RequestTempFile.DataLength + dataForProcessing.Length, 0);
			ParseData(curHeaderEndIndexInData, dataForProcessing, RequestTempFile.DataLength);
		}
		protected void ParseData(int offset, byte[] data, int totalDataLength) {
			if(ParserState == ParserState.RawContent) {
				int index = IndexOf(data, PartSeparatorMarker, offset);
				if(index > -1) {
					this.parserState = ParserState.PartSeparator;
					curHeaderStartIndex = totalDataLength - data.Length + index + 
						PartSeparatorMarker.Length + this.crlf.Length;
					ParseData(index, data, totalDataLength);
				} else
					this.parserState = ParserState.RawContent;
			} else if(ParserState == ParserState.PartSeparator) {
				int index = IndexOf(data, PartHeaderEndMarker, offset);
				if(index > -1) {
					int headerEndGlobalPos = GetHeaderEndMarkerGlobalPosition(index, curHeaderStartIndex, data, totalDataLength);
					if (headerEndGlobalPos < 0)
						return;
					curHeaderEndIndex = headerEndGlobalPos;
					int headerLength = curHeaderEndIndex - curHeaderStartIndex;
					ParseHeader(RequestTempFile.GetBytes(headerLength, curHeaderStartIndex));
					this.parserState = ParserState.RawContent;
					ParseData(index, data, totalDataLength);
				} else
					this.parserState = ParserState.PartSeparator;
			}
		}
		protected int GetHeaderEndMarkerGlobalPosition(int localPos, int headerStartGlobalPos, 
			byte[] data, int totalDataLength) {
			if (localPos < 0)
				return -1;
			int curEndGlobalPos = totalDataLength - data.Length + localPos;
			int curLength = curEndGlobalPos - headerStartGlobalPos;
			while (curEndGlobalPos <= headerStartGlobalPos && curEndGlobalPos <= totalDataLength && localPos >= 0) {
				localPos = IndexOf(data, PartHeaderEndMarker, localPos + 1);
				curEndGlobalPos = localPos >=0 ? totalDataLength - data.Length + localPos : -1;
			}
			return curEndGlobalPos;
		}
		protected void ParseHeader(byte[] data) {
			Dictionary<string, string> attributes = GetHeaderAttributes(data);
			if(attributes.ContainsKey("Content-Disposition") && attributes["Content-Disposition"].Equals("form-data")) {
				if(attributes.ContainsKey("filename"))
					this.currentFile = attributes["filename"];
				if(attributes.ContainsKey("Content-Type"))
					this.currentContentType = attributes["Content-Type"];
			}
		}
		protected Dictionary<string, string> GetHeaderAttributes(byte[] data) {
			Dictionary<string, string> attributes = new Dictionary<string, string>();
			string header = Encoding.GetString(data);
			string pattern = "([^ :;\r\n]*)(=|: )([^;\r\n]*)";
			Regex regExp = new Regex(pattern);
			MatchCollection collection = regExp.Matches(header);
			foreach(Match match in collection)
				attributes.Add(match.Groups[1].Value, match.Groups[3].Value.Trim('"'));
			return attributes;
		}
		protected int IndexOf(byte[] buffer, byte[] checkFor) {
			return IndexOf(buffer, checkFor, 0, buffer.Length);
		}
		protected int IndexOf(byte[] buffer, byte[] checkFor, int start) {
			return IndexOf(buffer, checkFor, start, buffer.Length - start);
		}
		protected int IndexOf(byte[] buffer, byte[] checkFor, int start, int count) {
			int index = 0;
			int startPos = Array.IndexOf(buffer, checkFor[0], start);
			int endPos = startPos + count;
			if(endPos > buffer.Length)
				endPos = buffer.Length;
			if(startPos != -1) {
				while((startPos + index) < endPos) {
					if(buffer[startPos + index] == checkFor[index]) {
						if(index == (checkFor.Length - 1))
							return startPos;
						index++;
					} else {
						startPos = Array.IndexOf<byte>(buffer, checkFor[0], startPos + index);
						if(startPos == -1)
							return -1;
						index = 0;
					}
				}
			}
			return -1;
		}
		void IDisposable.Dispose() {
			if(this.currentHeader != null)
				this.currentHeader.Flush();
			this.currentHeader = null;
			if(this.currentContent != null)
				this.currentContent.Flush();
			this.currentContent = null;
		}
	}
	public class TempFile : IDisposable {
		FileStream fileStream = null;
		public TempFile() {
			this.fileStream = new FileStream(GenerateFileName(), FileMode.Create, FileAccess.ReadWrite, FileShare.None, 0x1000, FileOptions.DeleteOnClose);
		}
		public FileStream FileStream {
			get { return fileStream; }
		}
		public int DataLength {
			get { return (int)FileStream.Length; }
		}
		public void AddBytes(byte[] data, int offset, int length) {
			if(FileStream == null)
				throw new InvalidOperationException();
			FileStream.Seek(0, SeekOrigin.End);
			FileStream.Write(data, offset, length);
		}
		public byte[] GetBytes(int length, int offset) {
			int curLength = Math.Min(length, DataLength);
			byte[] ret = new byte[curLength];
			GetBytesCore(offset, curLength, ret, 0);
			return ret;
		}
		public byte[] GetAllBytes() {
			return GetLastBytes(DataLength);
		}
		public byte[] GetLastBytes(int length) {
			int curLength = Math.Min(length, DataLength);
			byte[] ret = new byte[curLength];
			GetBytesCore(DataLength - curLength, curLength, ret, 0);
			return ret;
		}
		protected int GetBytesCore(int offset, int length, byte[] buffer, int bufferOffset) {
			if(FileStream == null)
				throw new InvalidOperationException();
			FileStream.Seek((long)offset, SeekOrigin.Begin);
			return FileStream.Read(buffer, bufferOffset, length);
		}
		public void Dispose() {
			if(FileStream != null)
				FileStream.Close();
		}
		protected string GenerateFileName() {
			string tempDir = Path.GetTempPath();
			string fileName = Path.GetRandomFileName();
			if(tempDir.EndsWith(@"\", StringComparison.Ordinal))
				return tempDir + fileName;
			return (tempDir + @"\" + fileName);
		}
	}
}
