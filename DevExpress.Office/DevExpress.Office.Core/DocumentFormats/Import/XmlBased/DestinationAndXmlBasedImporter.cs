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
using System.Xml;
using DevExpress.Utils;
using DevExpress.Office.Localization;
using System.Text.RegularExpressions;
using DevExpress.Office.Utils;
using DevExpress.Office.Drawing;
using DevExpress.Office.Import.OpenXml;
#if SL
using System.Windows.Media;
#endif
namespace DevExpress.Office {
	#region DestinationAndXmlBasedImporter (abstract class)
	public abstract class DestinationAndXmlBasedImporter : DestinationBasedImporter {
		#region Fields
		Stream baseStream;
		IDocumentModel actualDocumentModel;
		#endregion
		protected DestinationAndXmlBasedImporter(IDocumentModel documentModel)
			: base(documentModel) {
				actualDocumentModel = DocumentModel;
		}
		#region Properties
		protected internal Stream BaseStream { get { return baseStream; } }
		public IDocumentModel ActualDocumentModel { get { return actualDocumentModel; }  set { actualDocumentModel = value; } }
		protected virtual bool CreateEmptyDocumentOnLoadError { get { return true; } }
		public abstract string RelationsNamespace { get; }
		public abstract string DocumentRootFolder { get; set; }
		public abstract OpenXmlRelationCollection DocumentRelations { get ; } 
		#endregion
		public virtual XmlReader CreateXmlReader(Stream stream) {
			return XmlReader.Create(stream, CreateXmlReaderSettings());
		}
		protected internal virtual XmlReaderSettings CreateXmlReaderSettings() {
			XmlReaderSettings settings = new XmlReaderSettings();
			settings.IgnoreComments = true;
			settings.IgnoreWhitespace = true;
			return settings;
		}
		public virtual bool ReadToRootElement(XmlReader reader, string name) {
			try {
				return reader.ReadToFollowing(name);
			}
			catch {
				return false;
			}
		}
		protected internal virtual bool ReadToRootElement(XmlReader reader, string name, string ns) {
			try {
				return reader.ReadToFollowing(name, ns);
			}
			catch {
				return false;
			}
		}
		public virtual void ImportContent(XmlReader reader, Destination initialDestination) {
			int destinationCount = DestinationStack.Count;
			DestinationStack.Push(initialDestination);
			ImportContent(reader);
			while (DestinationStack.Count > destinationCount)
				DestinationStack.Pop();
			System.Diagnostics.Debug.Assert(destinationCount == DestinationStack.Count);
		}
		void ImportContent(XmlReader reader) {
			for (; ; ) {
				try {
					reader.Read();
				}
				catch { 
				}
				if (reader.ReadState == ReadState.EndOfFile || reader.ReadState == ReadState.Error)
					break;
				if (BaseStream != null)
					ProgressIndication.SetProgress((int)BaseStream.Position);
				ProcessCurrentDestination(reader);
			}
		}
		protected internal virtual void ProcessCurrentDestination(XmlReader reader) {
			DestinationStack.Peek().Process(reader);
		}
		protected internal virtual void ImportMainDocument(XmlReader reader, Stream baseStream) {
			this.baseStream = baseStream;
			if (baseStream != null)
				ProgressIndication.Begin(OfficeLocalizer.GetString(OfficeStringId.Msg_Loading), (int)baseStream.Position, (int)(baseStream.Length - baseStream.Position), (int)(baseStream.Position));
			else
				ProgressIndication.Begin(OfficeLocalizer.GetString(OfficeStringId.Msg_Loading), 0, 1, 0);
			try {
				BeginSetMainDocumentContent();
				try {
					ImportMainDocument(reader);
				}
				catch {
					if (CreateEmptyDocumentOnLoadError)
						SetMainDocumentEmptyContent();
					throw;
				}
				finally {
					EndSetMainDocumentContent();
				}
			}
			finally {
				ProgressIndication.End();
				this.baseStream = null;
			}
		}
		protected internal virtual void BeforeImportMainDocument() {
		}
		public abstract void BeginSetMainDocumentContent();
		public abstract void EndSetMainDocumentContent();
		public abstract void SetMainDocumentEmptyContent();
		protected internal virtual void ImportMainDocument(XmlReader reader) {
			BeforeImportMainDocument();
			this.DestinationStack = new Stack<Destination>();
			ImportContent(reader, CreateMainDocumentDestination());
			AfterImportMainDocument();
		}
		protected internal virtual void AfterImportMainDocument() {
		}
		protected internal abstract Destination CreateMainDocumentDestination();
		public virtual OfficeImage LookupImageByRelationId(IDocumentModel documentModel, string relationId, string rootFolder) {
				   return null;
		}
		readonly Dictionary<string, UriBasedOfficeImage> uniqueUriBasedImages = new Dictionary<string, UriBasedOfficeImage>();
		protected internal OpenXmlRelation LookupExternalRelationById(string relationId) {
			OpenXmlRelation relation = DocumentRelations.LookupRelationById(relationId);
			if (StringExtensions.CompareInvariantCultureIgnoreCase(relation.TargetMode, "external") != 0)
				return null;
			return relation;
		}
		protected internal virtual OfficeImage LookupExternalImageByRelation(OpenXmlRelation relation, string rootFolder) {
			UriBasedOfficeImage result;
			if (uniqueUriBasedImages.TryGetValue(relation.Target, out result))
				return new UriBasedOfficeReferenceImage(result, 0, 0);
			return CreateUriBasedImageCore(relation.Target, 0, 0);
		}
	   public virtual UriBasedOfficeImageBase LookupExternalImageByRelationId(string relationId, string rootFolder) {
			OpenXmlRelation relation = DocumentRelations.LookupRelationById(relationId);
			if (StringExtensions.CompareInvariantCultureIgnoreCase(relation.TargetMode, "external") != 0)
				return null;
			UriBasedOfficeImage result;
			if (uniqueUriBasedImages.TryGetValue(relation.Target, out result))
				return new UriBasedOfficeReferenceImage(result, 0, 0);
			return CreateUriBasedImageCore(relation.Target, 0, 0);
		}
		UriBasedOfficeImage CreateUriBasedImageCore(string uri, int pixelTargetWidth, int pixelTargetHeight) {
			UriBasedOfficeImage image = new UriBasedOfficeImage(uri, pixelTargetWidth, pixelTargetHeight, DocumentModel, false);
			uniqueUriBasedImages.Add(uri, image);
			return image;
		}
		#region Conversion and Parsing utilities
		#region bool
		public abstract bool ConvertToBool(string value);
		public bool GetWpSTOnOffValue(XmlReader reader, string attributeName) {
			return GetWpSTOnOffValue(reader, attributeName, true);
		}
		public bool GetWpSTOnOffValue(XmlReader reader, string attributeName, bool defaultValue) {
			string value = ReadAttribute(reader, attributeName);
			return GetOnOffValue(value, defaultValue);
		}
		public bool GetOnOffValue(XmlReader reader, string attributeName) {
			return GetOnOffValue(reader, attributeName, true);
		}
		public bool GetOnOffValue(XmlReader reader, string attributeName, bool defaultValue) {
			string value = reader.GetAttribute(attributeName, null);
			return GetOnOffValue(value, defaultValue);
		}
		public bool GetOnOffValue(string value, bool defaultValue) {
			if (!String.IsNullOrEmpty(value))
				return ConvertToBool(value);
			else
				return defaultValue;
		}
		#endregion
		#region bool?
		public bool? GetWpSTOnOffNullValue(XmlReader reader, string attributeName) {
			string value = reader.GetAttribute(attributeName);
			if (String.IsNullOrEmpty(value))
				return null;
			return GetWpSTOnOffValue(reader, attributeName);
		}
		#endregion
		#region Integer
		public int GetWpSTIntegerValue(XmlReader reader, string attributeName) {
			return GetWpSTIntegerValue(reader, attributeName, Int32.MinValue);
		}
		public int GetWpSTIntegerValue(XmlReader reader, string attributeName, int defaultValue) {
			return GetWpSTIntegerValue(reader, attributeName, NumberStyles.Integer, defaultValue);
		}
		public int GetWpSTIntegerValue(XmlReader reader, string attributeName, NumberStyles numberStyles, int defaultValue) {
			string value = ReadAttribute(reader, attributeName);
			return GetIntegerValue(value, numberStyles, defaultValue);
		}
		public int GetIntegerValue(XmlReader reader, string attributeName) {
			return GetIntegerValue(reader, attributeName, Int32.MinValue);
		}
		public int GetIntegerValue(XmlReader reader, string attributeName, int defaultValue) {
			return GetIntegerValue(reader, attributeName, NumberStyles.Integer, defaultValue);
		}
		public int GetIntegerValueInPoints(XmlReader reader, string attributeName, int defaultValue) {
			string value = reader.GetAttribute(attributeName, null);
			if (!String.IsNullOrEmpty(value))
				return GetIntegerValue(value.Replace("pt", ""), NumberStyles.Integer, defaultValue);
			else
				return Int32.MinValue;
		}
		public int GetIntegerValue(XmlReader reader, string attributeName, NumberStyles numberStyles, int defaultValue) {
			string value = reader.GetAttribute(attributeName, null);
			return GetIntegerValue(value, numberStyles, defaultValue);
		}
		public int GetIntegerValue(string value, NumberStyles numberStyles, int defaultValue) {
			if (!String.IsNullOrEmpty(value)) {
				int result;
				if (Int32.TryParse(value, numberStyles, CultureInfo.InvariantCulture, out result))
					return result;
				else {
					long longResult;
					if (long.TryParse(value, numberStyles, CultureInfo.InvariantCulture, out longResult))
						return (int)longResult;
					else {
						if (Int32.TryParse(value, numberStyles | NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out result))
							return result;
						double doubleResult;
						if (double.TryParse(value, numberStyles | NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out doubleResult))
							return (int)doubleResult;
						if (IgnoreParseErrors)
							return defaultValue;
						ThrowInvalidFile();
					}
				}
			}
			return defaultValue;
		}
		#endregion
		#region int?
		public int? GetIntegerNullableValue(XmlReader reader, string attr) {
			string value = ReadAttribute(reader, attr);
			if (String.IsNullOrEmpty(value))
				return null;
			return GetIntegerValue(value, NumberStyles.Integer, Int32.MinValue);
		}
		#endregion
		#region Float
		public float GetFloatValueInPoints(XmlReader reader, string attributeName, float defaultValue) {
			string value = reader.GetAttribute(attributeName, null);
			if (!String.IsNullOrEmpty(value))
				return GetFloatValue(value.Replace("pt", ""), NumberStyles.Float, defaultValue);
			else
				return float.MinValue;
		}
		public float GetFloatValue(string value, NumberStyles numberStyles, float defaultValue) {
			if (!String.IsNullOrEmpty(value)) {
				float result;
				if (float.TryParse(value, numberStyles, CultureInfo.InvariantCulture, out result))
					return result;
				else
					return Int32.MinValue;
			}
			return defaultValue;
		}
		public float GetWpSTFloatValue(XmlReader reader, string attributeName, NumberStyles numberStyles, float defaultValue, string ns) {
			string value = ReadAttribute(reader, attributeName, ns);
			return GetWpSTFloatValue(value, numberStyles, defaultValue);
		}
		public float GetWpSTFloatValue(XmlReader reader, string attributeName, NumberStyles numberStyles, float defaultValue) {
			string value = ReadAttribute(reader, attributeName);
			return GetWpSTFloatValue(value, numberStyles, defaultValue);
		}
		public float GetWpSTFloatValue(string value, NumberStyles numberStyles, float defaultValue) {
			if (!String.IsNullOrEmpty(value)) {
				double result;
				if (Double.TryParse(value, numberStyles, CultureInfo.InvariantCulture, out result))
					return (float)result;
				else
					ThrowInvalidFile();
			}
			return defaultValue;
		}
		public virtual double GetWpDoubleValue(XmlReader reader, string attributeName, double defaultValue) {
			string value = ReadAttribute(reader, attributeName);
			if (!String.IsNullOrEmpty(value)) {
				double result;
				if (double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out result))
					return result;
				else
					ThrowInvalidFile();
			}
			return defaultValue;
		}
		#endregion
		#region Long
		public long GetLongValue(XmlReader reader, string attributeName) {
			string value = reader.GetAttribute(attributeName);
			return GetLongValue(value, long.MinValue);
		}
		public long GetLongValue(string value, long defaultValue) {
			if (!String.IsNullOrEmpty(value)) {
				long result;
				if (long.TryParse(value, NumberStyles.Number, CultureInfo.InvariantCulture, out result))
					return result;
			}
			return defaultValue;
		}
		public long GetLongValue(XmlReader reader, string attributeName, long defaultValue) {
			string value = reader.GetAttribute(attributeName);
			return GetLongValue(value, defaultValue);
		}
		#endregion
		#region enum
		public T GetWpEnumValue<T>(XmlReader reader, string attributeName, Dictionary<T, string> table, T defaultValue) where T : struct {
			string value = ReadAttribute(reader, attributeName);
			if (String.IsNullOrEmpty(value))
				return defaultValue;
			return GetWpEnumValueCore(value, table, defaultValue);
		}
		public T GetWpEnumValue<T>(XmlReader reader, string attributeName, Dictionary<T, string> table, T defaultValue, string ns) where T : struct {
			string value = ReadAttribute(reader, attributeName, ns);
			if (String.IsNullOrEmpty(value))
				return defaultValue;
			return GetWpEnumValueCore(value, table, defaultValue);
		}
		public T? GetWpEnumOnOffNullValue<T>(XmlReader reader, string attributeName, Dictionary<T, string> table) where T : struct {
			string value = ReadAttribute(reader, attributeName);
			if (String.IsNullOrEmpty(value))
				return null;
			return GetWpEnumOnOffNullValueCore(value, table);
		}
		public T? GetWpEnumOnOffNullValueCore<T>(string value, Dictionary<T, string> table) where T : struct {
			foreach (T key in table.Keys) {
				string valueString = table[key];
				if (value == valueString)
					return key;
			}
			return null;
		}
		public T GetWpEnumValueCore<T>(string value, Dictionary<T, string> table, T defaultValue) where T : struct {
			foreach (T key in table.Keys) {
				string valueString = table[key];
				if (value == valueString)
					return key;
			}
			return defaultValue;
		}
		public T GetWpEnumValue<T>(XmlReader reader, string attributeName, Dictionary<string, T> table, T defaultValue) where T : struct {
			string value = ReadAttribute(reader, attributeName);
			if (String.IsNullOrEmpty(value))
				return defaultValue;
			return GetWpEnumValueCore(value, table, defaultValue);
		}
		public T GetWpEnumValueCore<T>(string value, Dictionary<string, T> table, T defaultValue) where T : struct {
			T result;
			if (!table.TryGetValue(value, out result))
				return defaultValue;
			else
				return result;
		}
		#endregion
		public abstract string ReadAttribute(XmlReader reader, string attributeName);
		public abstract string ReadAttribute(XmlReader reader, string attributeName, string ns);
		public string DecodeXmlChars(string val) {
			return XmlCharsDecoder.Decode(val);
		}
		protected virtual void ImportThemeCore(XmlReader reader) {
			PrepareOfficeTheme();
			Destination destination = new OfficeThemeDestination(this);
			destination.ProcessElementOpen(reader);
			ImportContent(reader, destination);
			ActualDocumentModel = DocumentModel;	
		}
		protected abstract void PrepareOfficeTheme();
		#endregion
	}
	#endregion
}
