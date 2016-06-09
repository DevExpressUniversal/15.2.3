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
using System.Collections.Specialized;
using DevExpress.Utils.Serializing.Helpers;
using System.IO;
using DevExpress.Utils;
using DevExpress.Compatibility.System.Collections.Specialized;
namespace DevExpress.Utils.Serializing {
	public class BinaryXtraSerializer : XtraSerializer {
		StringCollection stringTable = new StringCollection();
		public override bool CanUseStream { get { return true; } }
		protected override bool Serialize(Stream stream, IXtraPropertyCollection props, string appName) {
			return SerializeCore(stream, props, appName);
		}
		protected override bool Serialize(string path, IXtraPropertyCollection props, string appName) {
			bool res = false;
			using(FileStream stream = new FileStream(path, FileMode.Create, FileAccess.ReadWrite)) {
				res = SerializeCore(stream, props, appName);
			}
			return res;
		}
		protected override IXtraPropertyCollection Deserialize(Stream stream, string appName, IList objects) {
			return DeserializeCore(stream, appName);
		}
		protected override IXtraPropertyCollection Deserialize(string path, string appName, IList objects) {
			using(FileStream stream = new FileStream(path, FileMode.Open, FileAccess.ReadWrite)) {
				IXtraPropertyCollection result = DeserializeCore(stream, appName);
				return result;
			}
		}
		TypedBinaryReaderExWithStringTable CreateTypedReader(BinaryReader reader) {
			TypedBinaryReaderExWithStringTable typedReader = new TypedBinaryReaderExWithStringTable(reader);
			typedReader.CustomObjectConverter = CustomObjectConverter;
			return typedReader;
		}
		TypedBinaryWriterExWithStringTable CreateTypedWriter(BinaryWriter writer) {
			TypedBinaryWriterExWithStringTable typedWriter = new TypedBinaryWriterExWithStringTable(writer);
			typedWriter.CustomObjectConverter = CustomObjectConverter;
			return typedWriter;
		}
		IXtraPropertyCollection DeserializeCore(Stream stream, string appName) {
			IXtraPropertyCollection result;
			using(BinaryReader reader = new BinaryReader(stream)) {
				using(TypedBinaryReaderExWithStringTable typedReader = CreateTypedReader(reader)) {
					result = DeserializeLevel(typedReader);
					typedReader.Close();
				}
			}
			return result;
		}
		bool SerializeCore(Stream stream, IXtraPropertyCollection props, string appName) {
			using(MemoryStream memoryStream = new MemoryStream()) {
				using(BinaryWriter writer = new BinaryWriter(memoryStream)) {
					using(TypedBinaryWriterExWithStringTable typedWriter = CreateTypedWriter(writer)) {
						SerializeLevel(typedWriter, props);
						typedWriter.Close();
					}
				}
				byte[] bytes = memoryStream.ToArray();
				stream.Write(bytes, 0, bytes.Length);
			}
			return true;
		}
		void SerializeLevel(TypedBinaryWriterExWithStringTable typedWriter, IXtraPropertyCollection props) {
			typedWriter.WriteObject(props.Count);
			foreach(XtraPropertyInfo p in props)
				SerializeProperty(typedWriter, p);
		}
		IXtraPropertyCollection DeserializeLevel(TypedBinaryReaderExWithStringTable typedReader) {
			int count = (int)typedReader.ReadObject();
			IXtraPropertyCollection result = new XtraPropertyCollection();
			for(int i = 0; i < count; i++)
				result.Add(DeserializeProperty(typedReader));
			return result;
		}
		void SerializeProperty(TypedBinaryWriterExWithStringTable typedWriter, XtraPropertyInfo p) {
			object val = p.Value;
			bool isCustomObject = false;
			if(val != null) {
				Type type = val.GetType();
				if(!type.IsPrimitive() && type != typeof(TimeSpan) && type != typeof(DateTime) && type != typeof(Guid) && type != typeof(decimal) && type != typeof(byte[])) {
					val = ObjectConverterImpl.ObjectToString(val);
					isCustomObject = HasCustomObjectConverter && CustomObjectConverter.CanConvert(type);
				}
			}
			typedWriter.WriteObject(p.IsKey);
			typedWriter.WriteObject(p.Name);
			if(isCustomObject)
				typedWriter.WriteCustomObject(p.Value.GetType(), val.ToString());
			else
				typedWriter.WriteObject(val);
			if(p.IsKey)
				SerializeLevel(typedWriter, p.ChildProperties);
		}
		XtraPropertyInfo DeserializeProperty(TypedBinaryReaderExWithStringTable typedReader) {
			bool isKey = (bool)typedReader.ReadObject();
			string name = (string)typedReader.ReadObject();
			object value = typedReader.ReadObject();
			XtraPropertyInfo result = new XtraPropertyInfo(name, null, value, isKey);
			if(isKey)
				result.ChildProperties.AddRange(DeserializeLevel(typedReader));
			return result;
		}
	}
}
