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
	public class PdfOptionalContentUsageCreatorInfo : PdfObject {
		const string creatorDictionaryKey = "Creator";
		const string incorrectSubtypeDictionaryKey = "SubType";
		static string GetContentType(PdfReaderDictionary dictionary, string key) {
			object value;
			if (!dictionary.TryGetValue(key, out value) || value == null)
				return null;
			value = dictionary.Objects.TryResolve(value);
			PdfName name = value as PdfName;
			if (name != null)
				return name.Name;
			byte[] bytes = value as byte[];
			if (bytes == null)
				PdfDocumentReader.ThrowIncorrectDataException();
			return PdfDocumentReader.ConvertToString(bytes);
		}
		readonly string creator;
		readonly string contentType;
		readonly Dictionary<string, object> customProperties = new Dictionary<string, object>();
		public string Creator { get { return creator; } }
		public string ContentType { get { return contentType; } }
		public IDictionary<string, object> CustomProperties { get { return customProperties; } }
		internal PdfOptionalContentUsageCreatorInfo(PdfReaderDictionary dictionary) : base(dictionary.Number) {
			creator = dictionary.GetString(creatorDictionaryKey);
			if (String.IsNullOrEmpty(creator))
				PdfDocumentReader.ThrowIncorrectDataException();
			contentType = GetContentType(dictionary, PdfDictionary.DictionarySubtypeKey) ?? GetContentType(dictionary, incorrectSubtypeDictionaryKey);
			PdfObjectCollection objects = dictionary.Objects;
			foreach (KeyValuePair<string, object> pair in dictionary) {
				string key = pair.Key;
				if (key != creatorDictionaryKey && key != PdfDictionary.DictionarySubtypeKey && key != incorrectSubtypeDictionaryKey)
					customProperties.Add(key, PdfPrivateData.TryResolve(null, objects, pair.Value));
			}
		}
		protected internal override object ToWritableObject(PdfObjectCollection objects) {
			PdfWriterDictionary dictionary = new PdfWriterDictionary(objects);
			dictionary.Add(creatorDictionaryKey, creator);
			dictionary.AddName(PdfReaderDictionary.DictionarySubtypeKey, contentType);
			foreach (KeyValuePair<string, object> pair in customProperties) {
				object value = pair.Value;
				PdfPrivateData data = value as PdfPrivateData;
				dictionary.Add(pair.Key, data == null ? value : objects.AddObject(data));
			}
			return dictionary;
		}
	}
}
