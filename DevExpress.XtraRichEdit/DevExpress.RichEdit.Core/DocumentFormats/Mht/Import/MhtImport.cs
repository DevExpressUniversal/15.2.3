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
using System.IO;
using System.Text.RegularExpressions;
using DevExpress.XtraRichEdit.Import.Html;
using DevExpress.XtraRichEdit.Model;
using DevExpress.Utils;
using DevExpress.XtraRichEdit.Export.Mht;
using System.Text;
using System.Collections.Generic;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.XtraRichEdit.Services;
using DevExpress.XtraRichEdit.Internal;
using DevExpress.Office.Services;
using DevExpress.Office.Utils;
using DevExpress.Compatibility.System.Text;
namespace DevExpress.XtraRichEdit.Import.Mht {
	#region MhtImporter
	public class MhtImporter : RichEditDocumentModelImporter, IMhtImporter {
		#region MhtContentEncodingHelper
		private static class MhtContentEncodingHelper {
			static UTF32Encoding unt32BE = new UTF32Encoding(true, true);
			static byte[] utf8Bom = Encoding.UTF8.GetPreamble();
			static byte[] utf16LittleEndianBom = Encoding.Unicode.GetPreamble();
			static byte[] utf16BigEndianBom = Encoding.BigEndianUnicode.GetPreamble();
			static byte[] utf32LittleEndianBom = Encoding.UTF32.GetPreamble();
			static byte[] utf32BigEndianBom = unt32BE.GetPreamble();
			public static Encoding GetEncodingFromContent(string content) {
				byte[] bom = GetByteOrderMarkFromContent(content);
				return GetEncodingByByteOrderMark(bom);
			}
			public static Encoding GetEncodingByByteOrderMark(byte[] bom) {
				if(bom == null || bom.Length == 0)
					return null;
				if(AreBomsEquals(bom, utf8Bom))
					return Encoding.UTF8;
				if(AreBomsEquals(bom, utf16LittleEndianBom))
					return Encoding.Unicode;
				if(AreBomsEquals(bom, utf16BigEndianBom))
					return Encoding.BigEndianUnicode;
				if(AreBomsEquals(bom, utf32LittleEndianBom))
					return Encoding.UTF32;
				if(AreBomsEquals(bom, utf32BigEndianBom))
					return unt32BE;
				return null;
			}
			public static byte[] GetByteOrderMarkFromContent(string content) {
				List<byte> bytes = new List<byte>();
				int count = content.Length;
				for(int i = 0; i < count; i++) {
					char ch = content[i];
					if(ch == '=')
						i += DecodeQuotedChar(content, i, bytes);
					else
						break;
				}
				byte[] byteArray = bytes.ToArray();
				return byteArray;
			}
			private static bool AreBomsEquals(byte[] bom1, byte[] bom2) {
				if(bom1 == null || bom1.Length == 0 || bom2 == null || bom2.Length == 0 || bom1.Length != bom2.Length)
					return false;
				for(int i = 0; i < bom1.Length; i++) {
					if(!Byte.Equals(bom1[i], bom2[i]))
						return false;
				}
				return true;
			}
			private static int DecodeQuotedChar(string value, int index, List<byte> target) {
				if(index + 2 >= value.Length) { 
					target.Add((byte)value[index]);
					return 0;
				}
				string hexValue = value.Substring(index + 1, 2);
				int charValue;
				if(!Int32.TryParse(hexValue, System.Globalization.NumberStyles.HexNumber, System.Globalization.CultureInfo.InvariantCulture, out charValue)) {
					target.Add((byte)value[index]);
					return 0;
				}
				target.Add((byte)charValue);
				return 2;
			}
		}
		#endregion MhtContentEncodingHelper
		readonly IHtmlImporter htmlImporter;
		MhtPartCollection parts;
		List<string> partMarkers;
		string rootPartBaseUri;
		public MhtImporter(DocumentModel documentModel, MhtDocumentImporterOptions options)
			: base(documentModel, options) {
			HtmlDocumentImporterOptions htmlImporterOptions = new HtmlDocumentImporterOptions();
			htmlImporterOptions.CopyFrom(Options);
			if (!options.ShouldSerializeEncoding())
				htmlImporterOptions.ResetEncoding();
			this.htmlImporter = DocumentModel.InternalAPI.ImporterFactory.CreateHtmlImporter(DocumentModel, htmlImporterOptions);
		}
		protected internal IHtmlImporter HtmlImporter { get { return htmlImporter; } }
		public MhtDocumentImporterOptions Options { get { return (MhtDocumentImporterOptions)InnerOptions; } }
		public virtual void Import(Stream stream) {
			using (StreamReader reader = new StreamReader(stream, Options.ActualEncoding)) {
				ImportCore(reader);
			}
			DocumentModel.NormalizeZOrder();
		}
		protected internal virtual void ImportCore(StreamReader reader) {
			MhtPart headerPart = ReadMhtHeader(reader);
			if (headerPart == null)
				return;
			if (CompareStringsNoCase(headerPart.Content, "multipart/related") ||
				CompareStringsNoCase(headerPart.Content, "multipart/alternative") ||
				CompareStringsNoCase(headerPart.Content, "multipart/mixed") ||
				CompareStringsNoCase(headerPart.Content, "multipart/signed") ||
				CompareStringsNoCase(headerPart.Content, "multipart/report")) 
				ImportMultiPartContent(reader, headerPart);
			else if (CompareStringsNoCase(headerPart.Content, "text/html")) {
				headerPart.ContentType = headerPart.Content;
				ImportSinglePartContent(reader, headerPart);
			}
			else if (CompareStringsNoCase(headerPart.Content, "text/plain")) {
				headerPart.ContentType = headerPart.Content;
				ImportSinglePartContent(reader, headerPart);
			}
		}
		protected internal virtual void ImportMultiPartContent(StreamReader reader, MhtPart headerPart) {
			if (String.IsNullOrEmpty(headerPart.ContentId))
				return;
			if (String.IsNullOrEmpty(headerPart.ContentType))
				headerPart.ContentType = "text/html";
			if (!CompareStringsNoCase(headerPart.ContentType, "text/html") && !CompareStringsNoCase(headerPart.ContentType, "multipart/alternative") && !CompareStringsNoCase(headerPart.ContentType, "delivery-status"))
				return;
			string packageId = headerPart.ContentId;
			this.partMarkers = new List<string>();
			RegisterPartId(packageId);
			this.parts = ReadMhtParts(reader);
			ImportCore(parts);
		}
		protected internal virtual void ImportSinglePartContent(StreamReader reader, MhtPart headerPart) {
			this.partMarkers = new List<string>();
			this.parts = new MhtPartCollection();
			MhtPart part = new MhtPart();
			part.ContentType = headerPart.ContentType;
			part.ContentEncoding = headerPart.ContentEncoding;
			part.ContentTransferEncoding = headerPart.ContentTransferEncoding;
			part.Content = reader.ReadToEnd();
			this.parts.Add(part);
			ImportCore(parts);
		}
		protected internal virtual void RegisterPartId(string packageId) {
			this.partMarkers.Add("--" + packageId);
			this.partMarkers.Add("--" + packageId + "--");
		}
		protected internal virtual void ImportCore(MhtPartCollection parts) {
			MhtPart rootPart = parts.Find(IsHtmlTextPart);
			if (rootPart == null) {
				rootPart = parts.Find(IsPlainTextPart);
				if (rootPart == null)
					return;
				ImportPlainTextContent(rootPart);
				return;
			}
			ImportHtmlContent(rootPart);
		}
		string CalculateBaseUri(string uri) {
			try {
				Uri url = new Uri(uri);
#if !SL
				if (!url.IsFile)
					return String.Empty;
#else
				int slashIndex = Math.Max(uri.LastIndexOf('/'), uri.LastIndexOf('\\'));
				int dotIndex = uri.LastIndexOf('.');
				if (dotIndex <= slashIndex)
					return String.Empty;
#endif
			}
			catch {
				return String.Empty;
			}
			int index = Math.Max(uri.LastIndexOf('/'), uri.LastIndexOf('\\'));
			if (index <= 0)
				return String.Empty;
			if (index + 1 >= uri.Length)
				return String.Empty;
			return uri.Remove(index + 1);
		}
		protected internal virtual void ImportHtmlContent(MhtPart rootPart) {
			MhtUriStreamProvider provider = new MhtUriStreamProvider(this);
			IUriStreamService service = DocumentModel.GetService<IUriStreamService>();
			if (service != null)
				service.RegisterProvider(provider);
			this.rootPartBaseUri = CalculateBaseUri(rootPart.SourceUri);
			try {
				MhtTextPartContentStream contentStream = GetTextPartContentStream(rootPart);
				using (Stream stream = contentStream.Stream) {
					HtmlImporter.Options.Encoding = contentStream.Encoding;
					HtmlImporter.Encoding = contentStream.Encoding;
					HtmlImporter.Import(stream);
				}
			}
			finally {
				this.rootPartBaseUri = null;
				if (service != null)
					service.UnregisterProvider(provider);
			}
		}
		protected internal virtual void ImportPlainTextContent(MhtPart part) {
			Stream stream;
			if (CompareStringsNoCase(part.ContentTransferEncoding, "base64"))
				stream = GetStreamCore(Convert.FromBase64String(part.Content));
			else if (CompareStringsNoCase(part.ContentTransferEncoding, "quoted-printable")) {
				string singleLineString = QuotedPrintableEncoding.Instance.FromMultilineQuotedPrintableString(part.Content);
				string decodedString = QuotedPrintableEncoding.Instance.FromQuotedPrintableString(singleLineString, EmptyEncoding.Instance);
				byte[] bytes = EmptyEncoding.Instance.GetBytes(decodedString);
				stream = GetStreamCore(bytes);
			}
			else
				stream = GetStreamCore(EmptyEncoding.Instance.GetBytes(part.Content));
			PlainTextDocumentImporterOptions options = new PlainTextDocumentImporterOptions();
			options.Encoding = GetEncodingByWebName(part.ContentEncoding);
			DocumentModel.InternalAPI.LoadDocumentPlainTextContentCore(stream, options);
		}
		public Stream GetPartStream(string uri) {
			MhtPart part = LookupPartByUri(uri);
			if (part == null)
				return null;
			if (part.ContentType.StartsWith("text"))
				return GetTextPartContentStream(part).Stream;
			else
				return GetBinaryPartContentStream(part);
		}
		protected internal virtual MhtPart LookupPartByUri(string uri) {
			if (StringExtensions.StartsWithInvariantCultureIgnoreCase(uri, "cid:"))
				return LookupPartById(uri.Substring(4).Trim());
			else
				return LookupPartBySourceUri(uri);
		}
		protected internal virtual MhtPart LookupPartBySourceUri(string uri) {
			MhtPart part = LookupPartBySourceUriCore(uri);
			if (part != null)
				return part;
			return LookupPartBySourceUriCore(this.rootPartBaseUri + uri);
		}
		protected internal virtual MhtPart LookupPartBySourceUriCore(string uri) {
			int count = parts.Count;
			for (int i = 0; i < count; i++)
				if (CompareStringsNoCase(parts[i].SourceUri, uri))
					return parts[i];
			return null;
		}
		protected internal virtual MhtPart LookupPartById(string uri) {
			int count = parts.Count;
			for (int i = 0; i < count; i++)
				if (CompareStringsNoCase(parts[i].ContentId, uri))
					return parts[i];
			return null;
		}
		protected internal virtual string GetTextPartContent(MhtPart part, Encoding encoding) {
			if (CompareStringsNoCase(part.ContentTransferEncoding, "quoted-printable")) {
				QuotedPrintableEncoding quotedPrintable = QuotedPrintableEncoding.Instance;
				string content = quotedPrintable.FromMultilineQuotedPrintableString(part.Content);
				if (Options.ActualEncoding.Equals(EmptyEncoding.Instance))
					return quotedPrintable.FromQuotedPrintableString(content, encoding);
				else
					return quotedPrintable.FromQuotedPrintableString(content);
			}
			else if (CompareStringsNoCase(part.ContentTransferEncoding, "base64")) {
				byte[] bytes = Convert.FromBase64String(part.Content);
				return encoding.GetString(bytes, 0, bytes.Length);
			}
			else {
				if (Options.ActualEncoding.Equals(EmptyEncoding.Instance)) {
					byte[] bytes = EmptyEncoding.Instance.GetBytes(part.Content);
					return encoding.GetString(bytes, 0, bytes.Length);
				}
				else
					return part.Content;
			}
		}
		protected internal virtual MhtTextPartContentStream GetTextPartContentStream(MhtPart part) {
			Encoding encoding = GetEncodingByWebName(part.ContentEncoding);
			if (Object.ReferenceEquals(encoding, EmptyEncoding.Instance))
				encoding = Options.ActualEncoding;
			if(CompareStringsNoCase(part.ContentTransferEncoding, "quoted-printable")) {
				Encoding encFromCont = MhtContentEncodingHelper.GetEncodingFromContent(part.Content);
				if(encFromCont != null)
					encoding = encFromCont;
			}
			string content = GetTextPartContent(part, encoding);
			if (encoding == Encoding.UTF7)
				encoding = Encoding.UTF8;
			return new MhtTextPartContentStream(GetStreamCore(encoding.GetBytes(content)), encoding);
		}
		protected internal virtual Stream GetBinaryPartContentStream(MhtPart part) {
			if (!CompareStringsNoCase(part.ContentTransferEncoding, "base64"))
				return null;
			return GetStreamCore(Convert.FromBase64String(part.Content));
		}
		protected internal virtual Stream GetStreamCore(byte[] bytes) {
			MemoryStream stream = new MemoryStream();
			stream.Write(bytes, 0, bytes.Length);
			stream.Seek(0, SeekOrigin.Begin);
			return stream;
		}
		protected internal virtual Encoding GetEncodingByWebName(string webName) {
			EncodingInfo[] encodings = DXEncoding.GetEncodings();
			int count = encodings.Length;
			for (int i = 0; i < count; i++) {
				if (CompareStringsNoCase(encodings[i].Name, webName))
					return encodings[i].GetEncoding();
			}
			return EmptyEncoding.Instance;
		}
		protected internal bool IsHtmlTextPart(MhtPart part) {
			return CompareStringsNoCase(part.ContentType, "text/html");
		}
		protected internal bool IsPlainTextPart(MhtPart part) {
			return CompareStringsNoCase(part.ContentType, "text/plain");
		}
		protected internal virtual MhtPart ReadMhtHeader(StreamReader reader) {
			MhtPart result = new MhtPart();
			IList<string> controlWords = ReadControlWords(reader, true);
			int count = controlWords.Count;
			for (int i = 0; i < count; i++) {
				MhtControlWord controlWord = MhtControlWord.Parse(controlWords[i]);
				if (controlWord != null) {
					if (CompareStringsNoCase(controlWord.Name, "content-type")) {
						string packageId;
						if (controlWord.Attributes.TryGetValue("boundary", out packageId))
							result.ContentId = packageId;
						string contentType;
						if (controlWord.Attributes.TryGetValue("type", out contentType))
							result.ContentType = contentType;
						string charset;
						if (controlWord.Attributes.TryGetValue("charset", out charset))
							result.ContentEncoding = charset;
						result.Content = controlWord.Value;
					}
					if (CompareStringsNoCase(controlWord.Name, "content-transfer-encoding"))
						result.ContentTransferEncoding = controlWord.Value;
				}
			}
			return result;
		}
		protected internal virtual MhtPartCollection ReadMhtParts(StreamReader reader) {
			MhtPartCollection result = new MhtPartCollection();
			if (!ReadUntilFound(reader, partMarkers))
				return result;
			for (; ; ) {
				MhtControlWordCollection controlWords = ReadKeywordCollection(reader);
				MhtPart part = CreateMhtPart(controlWords);
				if (part != null) {
					result.Add(part);
					if (!ReadPartContent(reader, part))
						return result;
				}
				else {
					if (!SeekToNextPart(reader))
						return result;
				}
			}
		}
		protected internal virtual bool ReadPartContent(StreamReader reader, MhtPart part) {
			ChunkedStringBuilder content = new ChunkedStringBuilder();
			if (ReadUntilFound(reader, partMarkers, content)) {
				part.Content = content.ToString();
				return true;
			}
			else
				return false;
		}
		protected internal virtual bool SeekToNextPart(StreamReader reader) {
			ChunkedStringBuilder content = new ChunkedStringBuilder();
			return ReadUntilFound(reader, partMarkers, content);
		}
		protected internal virtual MhtPart CreateMhtPart(MhtControlWordCollection controlWords) {
			MhtControlWord contentTypeControlWord = controlWords["content-type"];
			if (contentTypeControlWord == null)
				return null;
			if (String.IsNullOrEmpty(contentTypeControlWord.Value))
				return null;
			string charset;
			if (!contentTypeControlWord.Attributes.TryGetValue("charset", out charset))
				charset = String.Empty;
			if (CompareStringsNoCase(contentTypeControlWord.Value, "multipart/alternative") ||
				CompareStringsNoCase(contentTypeControlWord.Value, "multipart/mixed") || 
				CompareStringsNoCase(contentTypeControlWord.Value, "multipart/related")) {
				string packageId;
				if (contentTypeControlWord.Attributes.TryGetValue("boundary", out packageId))
					RegisterPartId(packageId);
			}
			MhtControlWord contentTransferEncodingControlWord = controlWords["content-transfer-encoding"];
			if (contentTransferEncodingControlWord == null)
				contentTransferEncodingControlWord = new MhtControlWord();
			else if (String.IsNullOrEmpty(contentTransferEncodingControlWord.Value))
				return null;
			MhtPart part = new MhtPart();
			part.ContentEncoding = charset;
			part.ContentTransferEncoding = contentTransferEncodingControlWord.Value;
			part.ContentType = contentTypeControlWord.Value;
			MhtControlWord contentLocationControlWord = controlWords["content-location"];
			if (contentLocationControlWord != null)
				part.SourceUri = System.Uri.UnescapeDataString(contentLocationControlWord.Value);
			MhtControlWord contentIdControlWord = controlWords["content-id"];
			if (contentIdControlWord != null) {
				string id = contentIdControlWord.Value;
				id = id.Trim();
				id = id.Trim(new char[] { '<', '>' });
				id = id.Trim();
				part.ContentId = id;
			}
			return part;
		}
		protected internal IList<string> ReadControlWords(StreamReader reader, bool stopOnEmptyLine) {
			List<string> result = new List<string>();
			bool concatenate = false;
			for (; ; ) {
				if (reader.EndOfStream)
					return result;
				string line = reader.ReadLine();
				if (stopOnEmptyLine && String.IsNullOrEmpty(line))
					return result;
				line = DecodeLine(line);
				Match match = MhtControlWord.controlWordRegex.Match(line);
				bool lineStartLWSP = line.StartsWith(" ") || line.StartsWith("\t");
				if (match.Success && !(lineStartLWSP && concatenate)) {
					result.Add(line);
					concatenate = true;
				}
				else if (lineStartLWSP) {
					if (concatenate)
						result[result.Count - 1] += line.Substring(1, line.Length - 1);
				}
				else
					concatenate = false;
			}
		}
		const string qEncodingPattern = @"=\?(?<charset>.+)\?(?<encodingType>[B|Q|b|q])\?(?<content>.*)\?=";
		static Regex qEncodingRegex = new Regex(qEncodingPattern);
		protected internal string DecodeLine(string line) {
			MatchCollection matches = qEncodingRegex.Matches(line);
			int count = matches.Count;
			if (count <= 0)
				return line;
			for (int i = count - 1; i >= 0; i--) {
				Match match = matches[i];
				if (!match.Success)
					continue;
				string charset = match.Groups["charset"].Value.Trim();
				string encodingType = match.Groups["encodingType"].Value;
				string content = match.Groups["content"].Value;
				if (CompareStringsNoCase(encodingType, "B")) {
					byte[] bytes = Convert.FromBase64String(content);
					string decodedContent = GetEncodingByWebName(charset).GetString(bytes, 0, bytes.Length);
					line = line.Remove(match.Index, match.Length);
					line = line.Insert(match.Index, decodedContent);
				}
				if (CompareStringsNoCase(encodingType, "Q")) {
					string decodedContent = QEncoding.Instance.FromQuotedPrintableString(content, GetEncodingByWebName(charset));
					line = line.Remove(match.Index, match.Length);
					line = line.Insert(match.Index, decodedContent);
				}
			}
			return line;
		}
		protected internal MhtControlWord ParseContentType(string controlWordLine) {
			MhtControlWord controlWord = MhtControlWord.Parse(controlWordLine);
			if (!CompareStringsNoCase(controlWord.Name, "content-type"))
				return null;
			else
				return controlWord;
		}
		protected internal bool ReadUntilFound(StreamReader reader, List<string> searchPatterns) {
			return ReadUntilFound(reader, searchPatterns, null);
		}
		protected internal bool ReadUntilFound(StreamReader reader, List<string> searchPatterns, ChunkedStringBuilder target) {
			for (; ; ) {
				if (reader.EndOfStream)
					return false;
				string line = reader.ReadLine();
				if (MatchPatterns(line, searchPatterns))
					return true;
				if (target != null)
					target.AppendLine(line);
			}
		}
		protected internal virtual bool MatchPatterns(string value, List<string> searchPatterns) {
			int count = searchPatterns.Count;
			for (int i = 0; i < count; i++) {
				if (CompareStringsNoCase(value, searchPatterns[i]))
					return true;
			}
			return false;
		}
		public MhtControlWordCollection ReadKeywordCollection(StreamReader reader) {
			MhtControlWordCollection result = new MhtControlWordCollection();
			IList<string> controlWords = ReadControlWords(reader, true);
			int count = controlWords.Count;
			for (int i = 0; i < count; i++) {
				MhtControlWord controlWord = MhtControlWord.Parse(controlWords[i]);
				if (controlWord != null)
					result.Add(controlWord);
			}
			return result;
		}
		protected internal virtual bool CompareStringsNoCase(string value1, string value2) {
			return StringExtensions.CompareInvariantCultureIgnoreCase(value1, value2) == 0;
		}
		public override void ThrowInvalidFile() {
			throw new ArgumentException("Invalid mht file");
		}
	}
	#endregion
	public class MhtTextPartContentStream {
		readonly Stream stream;
		readonly Encoding encoding;
		public MhtTextPartContentStream(Stream stream, Encoding encoding) {
			Guard.ArgumentNotNull(stream, "stream");
			Guard.ArgumentNotNull(encoding, "encoding");
			this.stream = stream;
			this.encoding = encoding;
		}
		public Stream Stream { get { return stream; } }
		public Encoding Encoding { get { return encoding; } }
	}
	#region MhtControlWord
	public class MhtControlWord {
		#region Fields
		const string controlWordPattern = @"(?<controlWord>[\w-]+):\s{0,1}(?<content>.*)";
		internal static Regex controlWordRegex = new Regex(controlWordPattern);
		string name = String.Empty;
		string value = String.Empty;
		string content = String.Empty;
		MhtAttributeCollection attributes = new MhtAttributeCollection();
		#endregion
		#region Properties
		public string Name { get { return name; } set { name = value; } }
		public string Content { get { return content; } set { this.content = value; } }
		public string Value { get { return value; } set { this.value = value; } }
		public MhtAttributeCollection Attributes { get { return attributes; } }
		#endregion
		public static MhtControlWord Parse(string value) {
			if (String.IsNullOrEmpty(value))
				return null;
			MhtControlWord result = new MhtControlWord();
			Match match = controlWordRegex.Match(value);
			if (!match.Success)
				return null;
			result.Name = match.Groups["controlWord"].Value.Trim();
			result.Content = match.Groups["content"].Value;
			result.ParseValueAndAtributes(result.Content);
			return result;
		}
		const string attrNamePattern = @"(?<attrName>\w+)\s*=\s*(?<content>.*)";
		static Regex attrNameRegex = new Regex(attrNamePattern);
		protected internal virtual void ParseValueAndAtributes(string content) {
			string[] lines = content.Split(';');
			if (lines == null || lines.Length <= 0)
				return;
			int count = lines.Length;
			for (int i = 0; i < count; i++) {
				string line = lines[i];
				string trimLine = line.Trim();
				if (trimLine.StartsWith("<") && trimLine.EndsWith(">")) {
					if (String.IsNullOrEmpty(value))
						Value = trimLine;
					continue;
				}
				Match match = attrNameRegex.Match(line);
				if (!match.Success) {
					if (String.IsNullOrEmpty(value))
						Value = lines[i].Trim();
				}
				else {
					string attributeName = match.Groups["attrName"].Value;
					string attributeValue = match.Groups["content"].Value;
					Attributes[attributeName] = attributeValue.Trim('\"').Trim('\'').Trim();
				}
			}
		}
	}
	#endregion
	#region MhtAttributeCollection
	public class MhtAttributeCollection : Dictionary<string, string> {
		public MhtAttributeCollection()
			: base(StringExtensions.ComparerInvariantCultureIgnoreCase) {
		}
	}
	#endregion
	#region MhtControlWordCollection
	public class MhtControlWordCollection : DXCollection<MhtControlWord> {
		readonly Dictionary<string, MhtControlWord> nameHash = new Dictionary<string, MhtControlWord>(StringExtensions.ComparerInvariantCultureIgnoreCase);
		protected internal Dictionary<string, MhtControlWord> NameHash { get { return nameHash; } }
		public MhtControlWord this[string name] {
			get {
				MhtControlWord result;
				if (NameHash.TryGetValue(name, out result))
					return result;
				else
					return null;
			}
		}
		protected override void OnInsertComplete(int index, MhtControlWord value) {
			base.OnInsertComplete(index, value);
			if (NameHash.ContainsKey(value.Name))
				NameHash[value.Name] = value;
			else
				NameHash.Add(value.Name, value);
		}
		protected override void OnRemoveComplete(int index, MhtControlWord value) {
			base.OnRemoveComplete(index, value);
			if (NameHash.ContainsKey(value.Name))
				NameHash.Remove(value.Name);
		}
		protected override void OnClearComplete() {
			base.OnClearComplete();
			NameHash.Clear();
		}
	}
	#endregion
	public class MhtUriStreamProvider : IUriStreamProvider {
		readonly MhtImporter importer;
		public MhtUriStreamProvider(MhtImporter importer) {
			Guard.ArgumentNotNull(importer, "importer");
			this.importer = importer;
		}
		#region IUriStreamProvider Members
		public Stream GetStream(string uri) {
			return importer.GetPartStream(uri);
		}
		#endregion
	}
}
