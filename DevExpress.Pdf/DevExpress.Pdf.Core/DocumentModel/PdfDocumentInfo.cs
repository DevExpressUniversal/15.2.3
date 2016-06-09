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
using DevExpress.Utils;
using System.Text;
namespace DevExpress.Pdf.Native {
	public class PdfDocumentInfo : PdfObject {
		const string titleDictionaryKey = "Title";
		const string authorDictionaryKey = "Author";
		const string subjectDictionaryKey = "Subject";
		const string keywordsDictionaryKey = "Keywords";
		const string creatorDictionaryKey = "Creator";
		const string producerDictionaryKey = "Producer";
		const string creationDateDictionaryKey = "CreationDate";
		const string modDateDictionaryKey = "ModDate";
		const string trappedDictionaryKey = "Trapped";
		static void AddValue(StringBuilder sb, string value, params string[] tags) {
			if (!String.IsNullOrEmpty(value)) {
				foreach (string tag in tags) {
					sb.Append('<');
					sb.Append(tag);
					sb.Append('>');
				}
				sb.Append(value);
				Array.Reverse(tags);
				foreach (string tag in tags) {
					sb.Append("</");
					int i = tag.IndexOf(' ');
					sb.Append(i > 0 ? tag.Substring(0, i) : tag);
					sb.Append('>');
				}
				sb.AppendLine();
			}
		}
		string title;
		string author;
		string subject;
		string keywords;
		string creator;
		string producer;
		DateTimeOffset? creationDate;
		DateTimeOffset? modDate;
		DefaultBoolean trapped = DefaultBoolean.Default;
		public string Title {
			get { return title; }
			set { title = value; }
		}
		public string Author {
			get { return author; }
			set { author = value; }
		}
		public string Subject {
			get { return subject; }
			set { subject = value; }
		}
		public string Keywords {
			get { return keywords; }
			set { keywords = value; }
		}
		public string Creator {
			get { return creator; }
			set { creator = value; }
		}
		public string Producer {
			get { return producer; }
			set { producer = value; }
		}
		public DateTimeOffset? CreationDate {
			get { return creationDate; }
			set { creationDate = value; }
		}
		public DateTimeOffset? ModDate {
			get { return modDate; }
			set { modDate = value; }
		}
		public DefaultBoolean Trapped {
			get { return trapped; }
			set { trapped = value; }
		}
		public PdfDocumentInfo() {
		}
		public PdfDocumentInfo(PdfReaderDictionary dictionary) {
			title = dictionary.GetString(titleDictionaryKey);
			author = dictionary.GetString(authorDictionaryKey);
			subject = dictionary.GetString(subjectDictionaryKey);
			keywords = dictionary.GetString(keywordsDictionaryKey);
			creator = dictionary.GetString(creatorDictionaryKey);
			producer = dictionary.GetString(producerDictionaryKey);
			creationDate = dictionary.GetDate(creationDateDictionaryKey);
			modDate = dictionary.GetDate(modDateDictionaryKey);
			object value;
			if (dictionary.TryGetValue(trappedDictionaryKey, out value))
				if (value is bool)
					trapped = ((bool)value) ? DefaultBoolean.True : DefaultBoolean.False;
				else {
					string trappedName;
					PdfName name = value as PdfName;
					if (name == null) {
						byte[] bytes = value as byte[];
						if (bytes == null)
							PdfDocumentReader.ThrowIncorrectDataException();
						trappedName = PdfDocumentReader.ConvertToString(bytes);
					}
					else
						trappedName = name.Name;
					switch (trappedName) {
						case "True":
							trapped = DefaultBoolean.True;
							break;
						case "False":
							trapped = DefaultBoolean.False;
							break;
						case "Unknown":
							trapped = DefaultBoolean.Default;
							break;
						default:
							PdfDocumentReader.ThrowIncorrectDataException();
							break;
					}
				}
		}
		internal PdfMetadata GetMetadata(PdfCompatibility compatibility) {
			StringBuilder sb = new StringBuilder();
			sb.Append("<?xpacket begin=\"");
			sb.AppendLine("\" id=\"W5M0MpCehiHzreSzNTczkc9d\"?><x:xmpmeta xmlns:x=\"adobe:ns:meta/\">");
			sb.AppendLine("<rdf:RDF xmlns:rdf=\"http://www.w3.org/1999/02/22-rdf-syntax-ns#\">");
			sb.AppendLine("<rdf:Description rdf:about=\"\" xmlns:pdfaid=\"http://www.aiim.org/pdfa/ns/id/\" xmlns:pdf=\"http://ns.adobe.com/pdf/1.3/\" xmlns:dc=\"http://purl.org/dc/elements/1.1/\" xmlns:xmp=\"http://ns.adobe.com/xap/1.0/\">");
			if(compatibility == PdfCompatibility.PdfA2b)
				sb.AppendLine("<pdfaid:part>2</pdfaid:part><pdfaid:conformance>B</pdfaid:conformance>");
			if (compatibility == PdfCompatibility.PdfA3b)
				sb.AppendLine("<pdfaid:part>3</pdfaid:part><pdfaid:conformance>B</pdfaid:conformance>");
			AddValue(sb, Producer, "pdf:Producer");
			AddValue(sb, Keywords, "pdf:Keywords");
			AddValue(sb, Author, "dc:creator", "rdf:Seq", "rdf:li");
			AddValue(sb, Title, "dc:title", "rdf:Alt", "rdf:li xml:lang=\"x-default\"");
			AddValue(sb, Subject, "dc:description", "rdf:Alt", "rdf:li xml:lang=\"x-default\"");
			sb.AppendLine("</rdf:Description>");
			sb.AppendLine("</rdf:RDF></x:xmpmeta><?xpacket end=\"w\"?>");
			return new PdfMetadata(sb.ToString());
		}
		protected internal override object ToWritableObject(PdfObjectCollection objects) {
			PdfWriterDictionary dictionary = new PdfWriterDictionary(objects);
			dictionary.Add(titleDictionaryKey, title, null);
			dictionary.Add(authorDictionaryKey, author, null);
			dictionary.Add(subjectDictionaryKey, subject, null);
			dictionary.Add(keywordsDictionaryKey, keywords, null);
			dictionary.Add(creatorDictionaryKey, creator, null);
			dictionary.Add(producerDictionaryKey, producer, null);
			dictionary.AddNullable(creationDateDictionaryKey, creationDate);
			dictionary.AddNullable(modDateDictionaryKey, modDate);
			switch (trapped) {
				case DefaultBoolean.True:
					dictionary.Add(trappedDictionaryKey, new PdfName("True"));
					break;
				case DefaultBoolean.False:
					dictionary.Add(trappedDictionaryKey, new PdfName("False"));
					break;
			}
			return dictionary.Count > 0 ? dictionary : null;
		}
	}
}
