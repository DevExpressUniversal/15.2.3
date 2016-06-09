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
using System.Globalization;
using System.Collections.Generic;
using DevExpress.Utils;
namespace DevExpress.Pdf.Native {
	public class PdfWriterDictionary : PdfDictionary {
		readonly PdfObjectCollection objects;
		public PdfObjectCollection Objects { get { return objects; } }
		public PdfWriterDictionary(PdfObjectCollection objects) {
			this.objects = objects;
		}
		public void Add(string key, object value, object defaultValue) {
			if ((value == null && defaultValue != null) || (value != null && !value.Equals(defaultValue)))
				Add(key, value);
		}
		public void Add(string key, PdfObject value) {
			Add(key, value, null);
		}
		public void Add(string key, PdfRectangle value) {
			if (value != null)
				base.Add(key, value.ToWritableObject());
		}
		public void Add(string key, PdfRectangle value, PdfRectangle defaultValue) {
			if (value != null && !value.Equals(defaultValue))
				base.Add(key, value.ToWritableObject());
		}
		public void Add(string key, PdfStream value) {
			AddReference(key, value);
		}
		public void Add(string key, PdfCompressedStream value) {
			AddReference(key, value);
		}
		public void Add(string key, PdfObject value, PdfObject defaultValue) {
			if (value != null && !value.Equals(defaultValue))
				Add(key, objects.AddObject(value));
		}
		public void Add(string key, PdfColor color) {
			if (color != null)
				Add(key, color.ToWritableObject());
		}
		public void Add(string key, IEnumerable<double> value) {
			if (value != null)
				Add(key, new PdfWritableDoubleArray(value));
		}
		public void AddIfPresent(string key, object value) {
			if (value != null)
				Add(key, value);
		}
		public void AddNotNullOrEmptyString(string key, string value) {
			if (!String.IsNullOrEmpty(value))
				Add(key, value);
		}
		public void AddLanguage(CultureInfo culture) {
			if (culture != CultureInfo.InvariantCulture)
				Add(DictionaryLanguageKey, PdfDocumentStream.ConvertToArray(culture.Name));
		}
		public void AddNullable<T>(string key, T? value) where T : struct {
			if (value.HasValue)
				Add(key, value.Value);
		}
		public void AddEnumName<T>(string key, T value, bool useDefaultValue) where T : struct {
			AddName(key, PdfEnumToStringConverter.Convert<T>(value, useDefaultValue));
		}
		public void AddEnumName<T>(string key, T value) where T : struct {
			AddEnumName(key, value, true);
		}
		public void AddIntent(string key, PdfOptionalContentIntent value) {
			if (value == PdfOptionalContentIntent.All)
				AddList(key, new PdfOptionalContentIntent[] { PdfOptionalContentIntent.View, PdfOptionalContentIntent.Design },
						v => new PdfName(PdfEnumToStringConverter.Convert<PdfOptionalContentIntent>((PdfOptionalContentIntent)v, false)));
			else
				AddEnumName(key, value);
		}
		public void AddName(string key, string value) {
			if (!String.IsNullOrEmpty(value))
				Add(key, new PdfName(value));
		}
		public void AddName(string key, string value, string defaultValue) {
			if (value != defaultValue)
				AddName(key, value);
		}
		public void AddStream(string key, string data) {
			if (!String.IsNullOrEmpty(data))
				Add(key, objects.AddStream(data));
		}
		public void AddRequiredList<T>(string key, IList<T> value) where T : PdfObject {
			Add(key, new PdfWritableObjectArray(value, objects));
		}
		public void AddList<T>(string key, IList<T> value) where T : PdfObject {
			if (value != null && value.Count > 0)
				AddRequiredList(key, value);
		}
		public void AddList<T>(string key, IList<T> value, Func<T, object> converter) {
			if (value != null && value.Count > 0)
				Add(key, new PdfWritableConvertableArray<T>(value, converter));
		}
		public void AddEnumerable<T>(string key, IEnumerable<T> value, Func<T, object> converter) {
			if (value != null)
				Add(key, new PdfWritableConvertableArray<T>(value, converter));
		}
		public void AddASCIIString(string key, string value) {
			if (value != null)
				Add(key, DXEncoding.GetEncoding("Windows-1252").GetBytes(value));
		}
		void AddReference(string key, PdfStream value) {
			if (value != null && objects != null)
				Add(key, objects.AddStream(value));
		}
	}
}
