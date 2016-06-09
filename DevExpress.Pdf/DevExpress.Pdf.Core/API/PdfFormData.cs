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
using System.Text;
using System.IO;
using System.Collections.Generic;
using System.Xml.Linq;
using DevExpress.Pdf.Native;
using DevExpress.Pdf.Localization;
namespace DevExpress.Pdf {
	public class PdfFormData {
		const string kidsDictionaryKey = "Kids";
		const string fieldsDictionaryKey = "Fields";
		const string originalAttributeName = "{http://ns.adobe.com/xfdf-transition/}original";
		const string nameAttributeName = "name";
		const char tabulationCharacter = '\t';
		const char quotaCharacter = '\"';
		static readonly XNamespace xfdfNamespace = "http://ns.adobe.com/xfdf/";
		static readonly char[] separator = new char[] { (char)0x2E };
		static PdfFormDataFormat Detect(Stream stream) {
			string line = PdfDocumentStream.ReadString(stream);
			line = line.Replace("\u00ef\u00bb\u00bf", "");
			stream.Position = 0;
			if (line.StartsWith("%FDF-"))
				return PdfFormDataFormat.Fdf;
			if (line.StartsWith("<?xml"))
				return PdfFormDataFormat.Xml;
			return PdfFormDataFormat.Txt;
		}
		readonly IList<Action> rebuildAppearanceActions = new List<Action>();
		readonly PdfInteractiveFormField formField;
		bool allowAddNewKids;
		string name;
		object value;
		Dictionary<string, PdfFormData> kids;
		internal bool AllowAddNewKids {
			get { return allowAddNewKids; }
			set { allowAddNewKids = value; }
		}
		public string Name { get { return name; } }
		public object Value {
			get {
				if (kids != null)
					return kids.Values;
				return value;
			}
			set {
				if (formField != null) {
					formField.SetValue(value, rebuildAppearanceActions);
					this.value = formField.Value;
				}
				else
					this.value = value;
			}
		}
		public PdfFormData this[string name] {
			get {
				PdfFormData result;
				if (kids == null)
					kids = new Dictionary<string, PdfFormData>();
				if (kids.TryGetValue(name, out result))
					return result;
				string[] splittedName = name.Split(separator, 2);
				if (splittedName.Length > 1)
					return this[splittedName[0]][splittedName[1]];
				PdfFormData data = new PdfFormData();
				this[name] = data;
				data.name = name;
				return data;
			}
			set {
				if (!allowAddNewKids)
					throw new KeyNotFoundException(PdfCoreLocalizer.GetString(PdfCoreStringId.MsgFormDataNotFound));
				if (kids == null)
					kids = new Dictionary<string, PdfFormData>();
				if (value != null) {
					kids[name] = value;
					value.name = name;
				}
				else
					kids.Remove(name);
			}
		}
		internal PdfFormData(bool allowAddNewKids) {
			this.allowAddNewKids = allowAddNewKids;
		}
		internal PdfFormData(PdfInteractiveFormField formField) {
			this.formField = formField;
			name = formField.Name;
			value = formField.Value;
			allowAddNewKids = false;
		}
		public PdfFormData() {
			this.allowAddNewKids = true;
		}
		public PdfFormData(Stream stream, PdfFormDataFormat format) : this() {
			Load(stream, format);
		}
		public PdfFormData(Stream stream) : this(stream, Detect(stream)) { 
		}
#if !WINRT
		public PdfFormData(string fileName) : this() {
			using (Stream stream = File.OpenRead(fileName))
				Load(stream, Detect(stream));
		}
		public PdfFormData(string fileName, PdfFormDataFormat format) : this() {
			using (Stream stream = File.OpenRead(fileName))
				Load(stream, format);
		}
#endif
		public void Save(Stream stream, PdfFormDataFormat format) {
			switch (format) {
				case PdfFormDataFormat.Fdf:
					FdfDocumentWriter.Write(stream, this);
					break;
				case PdfFormDataFormat.Xml:
					WriteXml(stream);
					break;
				case PdfFormDataFormat.Xfdf:
					WriteXfdf(stream);
					break;
				case PdfFormDataFormat.Txt:
					WriteTxt(stream);
					break;
			}
		}
#if !WINRT
		public void Save(string fileName, PdfFormDataFormat format) {
			using (Stream stream = File.Create(fileName)) {
				Save(stream, format);
			}
		}
#endif
		public IList<string> GetFieldNames() {
			IList<string> result = new List<string>();
			if (kids != null)
				foreach (PdfFormData kid in kids.Values) {
					if (kid.kids == null)
						result.Add(kid.name);
					else
						foreach (string kidsKidName in kid.GetFieldNames())
							result.Add(kid.name + "." + kidsKidName);
				}
			return result;
		}
		internal PdfFormData GetKid(string name) {
			PdfFormData result;
			if (!kids.TryGetValue(name, out result))
				throw new ArgumentException("name");
			return result;
		}
		internal void AddKid(string name, PdfFormData kid) {
			if (kids == null)
				kids = new Dictionary<string, PdfFormData>();
			kids[name] = kid;
		}
		internal void AddAppearanceBuilder(Action rebuildAppearanceAction) {
			rebuildAppearanceActions.Add(rebuildAppearanceAction);
		}
		internal void Reset() {
			if (formField != null)
				Value = formField.DefaultValue;
			if (kids != null)
				foreach (PdfFormData kid in kids.Values)
					kid.Reset();
		}
		void Load(Stream stream, PdfFormDataFormat format) {
			try {
				switch (format) {
					case PdfFormDataFormat.Fdf:
						FdfDocumentReader.Read(stream, this);
						break;
					case PdfFormDataFormat.Xml:
					case PdfFormDataFormat.Xfdf:
						ReadXml(stream);
						break;
					case PdfFormDataFormat.Txt:
						ReadTxt(stream);
						break;
				}
			}
			catch {
				throw new ArgumentException(PdfCoreLocalizer.GetString(PdfCoreStringId.MsgIncorrectFormDataFile));
			}
		}
		void ReadTxt(Stream stream) {
			StreamReader reader = new StreamReader(stream);
			string strNames = reader.ReadLine();
			string strValues = reader.ReadToEnd().TrimEnd(new char[] { '\r', '\n' });
			if (string.IsNullOrEmpty(strNames) || string.IsNullOrEmpty(strValues))
				return;
			string[] names = strNames.Split(tabulationCharacter);
			string[] values = strValues.Split(tabulationCharacter);
			int valuesCount = values.Length;
			for (int i = 0; i < names.Length; i++) {
				string str = i < valuesCount ? values[i] : "";
				if (str.StartsWith(quotaCharacter.ToString())) {
					str = str.Substring(1, str.Length - 2);
					str = str.Replace("\"\"", "\"");
					string[] strs = str.Split(new char[] { '\n' });
					if (strs.Length == 1)
						this[names[i]].Value = strs[0];
					else
						this[names[i]].Value = new List<string>(strs);
				}
				else
					this[names[i]].Value = str;
			}
		}
		void ReadXml(Stream stream) {
			XDocument document = XDocument.Load(stream);
			XElement fields = document.Root;
			if (String.Equals(fields.Name.LocalName, "xfdf", StringComparison.OrdinalIgnoreCase))
				fields = fields.Element(xfdfNamespace + "fields");
			if (String.Equals(fields.Name.LocalName, "fields", StringComparison.OrdinalIgnoreCase))
				foreach (XElement child in fields.Elements()) {
					XAttribute nameAttribute = child.Attribute(originalAttributeName) ?? child.Attribute(nameAttributeName);
					string name = nameAttribute == null ? child.Name.LocalName : nameAttribute.Value;
					List<string> values = new List<string>();
					foreach (XElement value in child.Elements())
						values.Add(value.Value);
					switch (values.Count) {
						case 0:
							this[name].Value = child.Value;
							break;
						case 1:
							this[name].Value = values[0];
							break;
						default:
							this[name].Value = values;
							break;
					}
				}
		}
		void WriteTxt(Stream stream) {
			string names = "";
			string values = "";
			WriteFields((name, value) => {
				names += name + tabulationCharacter;
				IEnumerable<string> lst = value as IEnumerable<string>;
				if (lst != null) {
					string strs = quotaCharacter.ToString();
					foreach (string str in lst)
						strs += str + "\n";
					strs = strs.TrimEnd(new char[] { '\n' });
					strs += quotaCharacter;
					values += strs + tabulationCharacter;
				}
				else {
					string strValue = ((string)value).Replace("\r\n", "\n");
					strValue = strValue.Replace("\"", "\"\"");
					if (strValue.Contains("\n"))
						strValue = quotaCharacter + strValue + quotaCharacter;
					values += strValue + tabulationCharacter;
				}
			});
			names = names.Substring(0, Math.Max(0, names.Length - 1));
			values = values.Substring(0, Math.Max(0, values.Length - 1));
			Encoding encoding = Encoding.UTF8;
			byte[] byteNames = encoding.GetBytes(names + "\r\n");
			stream.Write(byteNames, 0, byteNames.Length);
			byte[] byteValues = encoding.GetBytes(values + "\r\n");
			stream.Write(byteValues, 0, byteValues.Length);
		}
		void WriteXml(Stream stream) {
			XDocument document = new XDocument(WriteXmlFields("", true));
			document.Save(stream);
		}
		void WriteXfdf(Stream stream) {
			XDocument document = new XDocument();
			XElement xfdf = new XElement(xfdfNamespace + "xfdf");
			xfdf.Add(WriteXmlFields(xfdfNamespace, false));
			document.Add(xfdf);
			document.Save(stream);
		}
		XElement WriteXmlFields(XNamespace ns, bool writeOriginal) {
			XNamespace original = "http://ns.adobe.com/xfdf-transition/";
			XElement fields = new XElement(ns + "fields");
			WriteFields((name, value) => {
				List<string> listValue = value as List<string>;
				XElement element = new XElement(ns + "field");
				if (writeOriginal)
					element.SetAttributeValue(original + "original", name);
				element.SetAttributeValue(nameAttributeName, name);
				if (listValue != null) {
					foreach (string val in listValue)
						element.Add(new XElement(ns + "value", val));
				}
				else
					if (writeOriginal)
						element.Add(value);
					else
						element.Add(new XElement(ns + "value", value));
				fields.Add(element);
			});
			return fields;
		}
		void WriteFields(Action<string, object> fieldWriter) {
			Queue<KeyValuePair<string, PdfFormData>> queue = new Queue<KeyValuePair<string, PdfFormData>>();
			if (kids != null)
				foreach (PdfFormData kid in kids.Values)
					queue.Enqueue(new KeyValuePair<string, PdfFormData>(kid.Name, kid));
			while (queue.Count > 0) {
				KeyValuePair<string, PdfFormData> kid = queue.Dequeue();
				if (kid.Value.kids != null)
					foreach (PdfFormData kidKid in kid.Value.kids.Values)
						queue.Enqueue(new KeyValuePair<string, PdfFormData>(kid.Key + "." + kidKid.Name, kidKid));
				else {
					if (kid.Value.Value != null)
						fieldWriter(kid.Key, kid.Value.Value);
				}
			}
		}
		internal void Apply(PdfFormData data) {
			if (data == null || data.Name != Name)
				throw new ArgumentNullException("data");
			IEnumerable<PdfFormData> dataKids = data.Value as IEnumerable<PdfFormData>;
			if (dataKids == null) 
				Value = data.Value;
			else if (kids != null)
				foreach (PdfFormData dataKid in dataKids) {
					PdfFormData kid;
					if (kids.TryGetValue(dataKid.Name, out kid))
						kid.Apply(dataKid);
				}
		}
		internal PdfWriterDictionary CreateRootDictionary(PdfObjectCollection collection) {
			PdfWriterDictionary dictionary = new PdfWriterDictionary(collection);
			if (kids != null) {
				IList<object> result = new List<object>();
				foreach (KeyValuePair<string, PdfFormData> kvp in kids) {
					PdfFormData formData = kvp.Value;
					if (formData.formField == null || !formData.formField.Flags.HasFlag(PdfInteractiveFormFieldFlags.Password))
						result.Add(formData.CreateDictionary(collection));
				}
				dictionary.Add(fieldsDictionaryKey, new PdfWritableArray(result));
			}
			return dictionary;
		}
		internal PdfWriterDictionary CreateDictionary(PdfObjectCollection collection) {
			PdfWriterDictionary dictionary = new PdfWriterDictionary(collection);
			if (name != null)
				dictionary.AddASCIIString("T", name);
			if (value != null)
				dictionary.Add("V", value);
			if (kids != null) {
				IList<object> result = new List<object>();
				foreach (KeyValuePair<string, PdfFormData> kvp in kids) {
					PdfFormData formData = kvp.Value;
					if (formData.formField == null || !formData.formField.Flags.HasFlag(PdfInteractiveFormFieldFlags.Password))
						result.Add(formData.CreateDictionary(collection));
				}
				dictionary.Add(kidsDictionaryKey, new PdfWritableArray(result));
			}
			return dictionary;
		}
	}
}
