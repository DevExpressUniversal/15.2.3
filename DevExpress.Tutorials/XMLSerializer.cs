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
using System.IO;				 
using System.IO.IsolatedStorage; 
using System.Xml.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Reflection;
namespace DevExpress.Demos.XmlSerialization {
	public enum SerializeFormat { Binary, XML }
	public static class XMLSerializer<T> where T : class {
		public static T Load(string path) { return LoadFromXmlFormat(null, path, null); }
		public static T Load(string path, SerializeFormat serializedFormat) {
			T serializableObject = null;
			switch(serializedFormat) {
				case SerializeFormat.Binary:
					serializableObject = LoadFromBinaryFormat(path, null);
					break;
				case SerializeFormat.XML:
				default:
					serializableObject = LoadFromXmlFormat(null, path, null);
					break;
			}
			return serializableObject;
		}
		public static T LoadXmlFromResources(Assembly assembly, string path, Type[] extraTypes) {
			T serializableObject = null;
			using(Stream stream = assembly.
					   GetManifestResourceStream(path)) {
				using(StreamReader sr = new StreamReader(stream)) {
					serializableObject = LoadFromXml(sr, extraTypes);					
				}
			}
			return serializableObject;
		}
		public static T LoadFromXml(TextReader textReader, Type[] extraTypes) {
			T serializableObject = null;
			XmlSerializer xmlSerializer = CreateXmlSerializer(extraTypes);
			serializableObject = xmlSerializer.Deserialize(textReader) as T;
			return serializableObject;
		}
		public static T Load(string path, System.Type[] extraTypes) {
			T serializableObject = LoadFromXmlFormat(extraTypes, path, null);
			return serializableObject;
		}
		public static T Load(string fileName, IsolatedStorageFile isolatedStorageDirectory) {
			T serializableObject = LoadFromXmlFormat(null, fileName, isolatedStorageDirectory);
			return serializableObject;
		}
		public static T Load(string fileName, IsolatedStorageFile isolatedStorageDirectory, SerializeFormat serializedFormat) {
			T serializableObject = null;
			switch(serializedFormat) {
				case SerializeFormat.Binary:
					serializableObject = LoadFromBinaryFormat(fileName, isolatedStorageDirectory);
					break;
				case SerializeFormat.XML:
				default:
					serializableObject = LoadFromXmlFormat(null, fileName, isolatedStorageDirectory);
					break;
			}
			return serializableObject;
		}
		public static T Load(string fileName, IsolatedStorageFile isolatedStorageDirectory, System.Type[] extraTypes) {
			T serializableObject = LoadFromXmlFormat(null, fileName, isolatedStorageDirectory);
			return serializableObject;
		}
		public static void Save(T serializableObject, string path) { SaveToXmlFormat(serializableObject, null, path, null); }
		public static void Save(T serializableObject, string path, SerializeFormat serializedFormat) {
			switch(serializedFormat) {
				case SerializeFormat.Binary:
					SaveToBinaryFormat(serializableObject, path, null);
					break;
				case SerializeFormat.XML:
				default:
					SaveToXmlFormat(serializableObject, null, path, null);
					break;
			}
		}
		public static void Save(T serializableObject, string path, System.Type[] extraTypes) {
			SaveToXmlFormat(serializableObject, extraTypes, path, null);
		}
		public static void Save(T serializableObject, string fileName, IsolatedStorageFile isolatedStorageDirectory) {
			SaveToXmlFormat(serializableObject, null, fileName, isolatedStorageDirectory);
		}
		public static void Save(T serializableObject, string fileName, IsolatedStorageFile isolatedStorageDirectory, SerializeFormat serializedFormat) {
			switch(serializedFormat) {
				case SerializeFormat.Binary:
					SaveToBinaryFormat(serializableObject, fileName, isolatedStorageDirectory);
					break;
				case SerializeFormat.XML:
				default:
					SaveToXmlFormat(serializableObject, null, fileName, isolatedStorageDirectory);
					break;
			}
		}
		public static void Save(T serializableObject, string fileName, IsolatedStorageFile isolatedStorageDirectory, System.Type[] extraTypes) {
			SaveToXmlFormat(serializableObject, null, fileName, isolatedStorageDirectory);
		}
		static T LoadFromBinaryFormat(string path, IsolatedStorageFile isolatedStorageFolder) {
			T serializableObject = null;
			using(FileStream fileStream = CreateFileStream(isolatedStorageFolder, path)) {
				BinaryFormatter binaryFormatter = new BinaryFormatter();
				serializableObject = binaryFormatter.Deserialize(fileStream) as T;
			}
			return serializableObject;
		}
		static T LoadFromXmlFormat(System.Type[] extraTypes, string path, IsolatedStorageFile isolatedStorageFolder) {
			T serializableObject = null;
			using(TextReader textReader = CreateTextReader(isolatedStorageFolder, path)) {
				XmlSerializer xmlSerializer = CreateXmlSerializer(extraTypes);
				serializableObject = xmlSerializer.Deserialize(textReader) as T;
			}
			return serializableObject;
		}
		static FileStream CreateFileStream(IsolatedStorageFile isolatedStorageFolder, string path) {
			return isolatedStorageFolder == null ? new FileStream(path, FileMode.OpenOrCreate) :
					new IsolatedStorageFileStream(path, FileMode.OpenOrCreate, isolatedStorageFolder);
		}
		static TextReader CreateTextReader(IsolatedStorageFile isolatedStorageFolder, string path) {
			return 
				isolatedStorageFolder == null ?  new StreamReader(path) :
					new StreamReader(new IsolatedStorageFileStream(path, FileMode.Open, isolatedStorageFolder));
		}
		static TextWriter CreateTextWriter(IsolatedStorageFile isolatedStorageFolder, string path) {
			return isolatedStorageFolder == null ? new StreamWriter(path) :
				 new StreamWriter(new IsolatedStorageFileStream(path, FileMode.OpenOrCreate, isolatedStorageFolder));
		}
		static XmlSerializer CreateXmlSerializer(Type[] extraTypes) {
			return extraTypes == null ? new XmlSerializer(typeof(T)) : new XmlSerializer(typeof(T), extraTypes);
		}
		static void SaveToXmlFormat(T serializableObject, Type[] extraTypes, string path, IsolatedStorageFile isolatedStorageFolder) {
			using(TextWriter textWriter = CreateTextWriter(isolatedStorageFolder, path)) {
				XmlSerializer xmlSerializer = CreateXmlSerializer(extraTypes);
				xmlSerializer.Serialize(textWriter, serializableObject);
			}
		}
		static void SaveToBinaryFormat(T serializableObject, string path, IsolatedStorageFile isolatedStorageFolder) {
			using(FileStream fileStream = CreateFileStream(isolatedStorageFolder, path)) {
				BinaryFormatter binaryFormatter = new BinaryFormatter();
				binaryFormatter.Serialize(fileStream, serializableObject);
			}
		}
	}
}
