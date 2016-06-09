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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Xml;
using DevExpress.Diagram.Core.TypeConverters;
using DevExpress.Internal;
using DevExpress.Utils;
using DevExpress.Utils.Serializing;
using DevExpress.Utils.Serializing.Helpers;
namespace DevExpress.Diagram.Core.Serialization {
	public sealed class DiagramSerializer : XmlXtraSerializer {
		static DiagramSerializer() {
			CustomSerializableObjectRegistrator.Instance.Register(new CustomSerializableObject<DefaultSelectionLayer>(typeof(DefaultSelectionLayer), () => (DefaultSelectionLayer)DefaultSelectionLayer.Instance));
		}
		public DiagramSerializer() {
		}
		protected override SerializeHelper CreateSerializeHelper(object rootObj, bool useRootObj) {
			return new DiagramSerializeHelper(rootObj);
		}
		protected override DeserializeHelper CreateDeserializeHelper(object rootObj, bool useRootObj) {
			return new DiagramDeserializeHelper(rootObj);
		}
		public override bool SerializeObject(object obj, Stream stream, string appName, OptionsLayoutBase options) {
			OnStartSerialization();
			try {
				return base.SerializeObject(obj, stream, appName, options);
			}
			finally {
				OnFinishSerialization();
			}
		}
		public override void DeserializeObject(object obj, Stream stream, string appName, OptionsLayoutBase options) {
			OnStartDeserialization();
			try {
				base.DeserializeObject(obj, stream, appName, options);
			}
			finally {
				OnFinishDeserialization();
			}
		}
		void OnStartSerialization() { RegisterConverters(); }
		void OnFinishSerialization() { UnregisterConverters(); }
		void OnStartDeserialization() { RegisterConverters(); }
		void OnFinishDeserialization() {
			UnregisterConverters();
		}
		void RegisterConverters() {
			ObjectConverter.Instance.RegisterConverter(new DiagramTextDecorationCollectionConverter());
		}
		void UnregisterConverters() {
			ObjectConverter.Instance.UnregisterConverter(typeof(DiagramTextDecorationCollectionConverter));
		}
		#region Serialization
		protected override void WriteApplicationAttribute(string appName, XmlWriter tw) {
		}
		protected override void WriteStartDocument(XmlWriter tw) {
			tw.WriteStartDocument();
		}
		protected override void SerializeLevelCore(XmlWriter tw, IXtraPropertyCollection props) {
			if(!props.IsSinglePass) {
				foreach(XtraPropertyInfo p in props) {
					if(!p.IsKey) SerializeAttributeProperty(tw, p);
				}
			}
			foreach(XtraPropertyInfo p in props) {
				if(p.IsKey) SerializeContentProperty(tw, p);
			}
		}
		protected override IXtraPropertyCollection DeserializeCore(Stream stream, string appName, IList objects) {
			return new DiagramDeserializationPropertyCollection(this, CreateReader(stream));
		}
		void SerializeContentProperty(XmlWriter tw, XtraPropertyInfo p) {
			if(p.ChildProperties.Count != 0) SerializeContentPropertyCore(tw, p);
		}
		void SerializeContentPropertyCore(XmlWriter tw, XtraPropertyInfo p) {
			tw.WriteStartElement(p.Name);
			if(p.IsKey && p.Value != null)
				tw.WriteAttributeString("value", p.Value.ToString());
			SerializeLevel(tw, p.ChildProperties);
			tw.WriteEndElement();
		}
		void SerializeAttributeProperty(XmlWriter tw, XtraPropertyInfo p) {
			object val = p.Value;
			if(val != null) {
				Type type = val.GetType();
				if(!type.IsPrimitive()) val = ObjectConverterImpl.ObjectToString(val);
			}
			string stringValue = p.Value != null ? XmlObjectToString(val) : null;
			tw.WriteAttributeString(p.Name, stringValue);
		}
		internal XtraPropertyInfo ReadInfoCore(XmlReader reader) {
			reader.Read();
			reader.MoveToContent();
			return ReadInfoCore(reader, true);
		}
		XtraPropertyInfo ReadInfo(XmlReader reader, bool skipZeroDepth) {
			reader.MoveToContent();
			return ReadInfoCore(reader, skipZeroDepth);
		}
		XtraPropertyInfo ReadInfoCore(XmlReader reader, bool skipZeroDepth) {
			if(reader.NodeType == XmlNodeType.Element) {
				XtraPropertyInfo info = new XmlXtraPropertyInfo(reader.Name, null, null, true);
				bool isEmptyElement = reader.IsEmptyElement;
				ReadAttributes(reader, info, skipZeroDepth);
				if(isEmptyElement)
					return info;
				while(reader.ReadState != ReadState.EndOfFile) {
					reader.Read();
					if(reader.NodeType == XmlNodeType.EndElement)
						break;
					XtraPropertyInfo child = ReadInfoCore(reader, false);
					if(child != null)
						info.ChildProperties.Add(child);
				}
				return info;
			}
			return null;
		}
		void ReadAttributes(XmlReader reader, XtraPropertyInfo info, bool skipZeroDepth) {
			if(skipZeroDepth && reader.Depth == 0) return;
			while(reader.MoveToNextAttribute()) {
				Type type = null;
				if(reader.Name.EndsWith("_type")) {
					type = Type.GetType(reader.Value, false);
					if(type == null) {
						type = ObjectConverterImpl.ResolveType(reader.Value);
					}
					reader.MoveToNextAttribute();
				}
				if(reader.Name == "value") {
					info.Value = reader.Value;
				}
				info.ChildProperties.Add(new XmlXtraPropertyInfo(reader.Name, type, reader.Value, false));
			}
		}
		#endregion
	}
	public class DiagramDeserializationPropertyCollection : VirtualXtraPropertyCollectionBase, IDisposable {
		XmlReader reader;
		DiagramSerializer serializer;
		public DiagramDeserializationPropertyCollection(DiagramSerializer serializer, XmlReader reader) {
			this.serializer = serializer;
			this.reader = reader;
		}
		protected override CollectionItemInfosEnumeratorBase CreateEnumerator() {
			return new DiagramDeserializationItemInfosEnumerator(serializer, reader);
		}
		public void Dispose() {
			Dispose(true);
		}
		protected virtual void Dispose(bool disposing) {
			this.reader = null;
			this.serializer = null;
		}
	}
	public class DiagramDeserializationItemInfosEnumerator : CollectionItemInfosEnumeratorBase, IDisposable {
		XmlReader reader;
		DiagramSerializer serializer;
		public DiagramDeserializationItemInfosEnumerator(DiagramSerializer serializer, XmlReader reader) {
			this.reader = reader;
			this.serializer = serializer;
			Reset();
		}
		bool finished;
		public override void Reset() {
			base.Reset();
			finished = !reader.IsStartElement();
		}
		protected override bool MoveNextCore() {
			if(finished) return false;
			this.currentInfo = serializer.ReadInfoCore(reader);
			finished = this.currentInfo == null;
			return !finished;
		}
		public void Dispose() {
			Dispose(true);
		}
		protected virtual void Dispose(bool disposing) {
			this.reader = null;
			this.serializer = null;
		}
	}
	public class DiagramSerializeHelper : SerializeHelper {
		public DiagramSerializeHelper(object rootObject)
			: base(rootObject) {
		}
		protected override XtraPropertyInfoCollection SerializeLayoutVersion(OptionsLayoutBase options, object obj) {
			return new XtraPropertyInfoCollection();
		}
		protected override SerializationContext CreateSerializationContext() {
			return new DiagramSerializationContext();
		}
	}
	public class DiagramDeserializeHelper : DeserializeHelper {
		public DiagramDeserializeHelper(object rootObject) : base(rootObject) {
		}
		protected override SerializationContext CreateSerializationContext() {
			return new DiagramDeserializationContext();
		}
	}
	public abstract class DiagramSerializationContextBase : SerializationContext {
		protected override PropertyDescriptorCollection GetProperties(object obj, IXtraPropertyCollection store) {
			IDiagramItem item = obj as IDiagramItem;
			return item != null ? GetSerializableProperties(item) : base.GetProperties(obj, store);
		}
		protected override PropertyDescriptorCollection GetProperties(object obj) {
			IDiagramItem item = obj as IDiagramItem;
			return item != null ? GetSerializableProperties(item) : base.GetProperties(obj);
		}
		protected PropertyDescriptorCollection GetSerializableProperties(IDiagramItem item) {
			PropertyDescriptor[] properties = GetSerializablePropertiesCore(item).ToArray();
			return new PropertyDescriptorCollection(properties);
		}
		protected abstract IEnumerable<PropertyDescriptor> GetSerializablePropertiesCore(IDiagramItem item);
	}
	public class DiagramSerializationContext : DiagramSerializationContextBase {
		public DiagramSerializationContext() {
		}
		protected override IEnumerable<PropertyDescriptor> GetSerializablePropertiesCore(IDiagramItem item) {
			return item.Controller.GetCloneableProperties();
		}
	}
	public class DiagramDeserializationContext : DiagramSerializationContextBase {
		public DiagramDeserializationContext() {
		}
		protected override IEnumerable<PropertyDescriptor> GetSerializablePropertiesCore(IDiagramItem item) {
			return item.Controller.GetItemProperties();
		}
		protected override void InvokeAfterDeserialize(DeserializeHelper helper, object obj, XtraPropertyInfo bp, object value) {
			base.InvokeAfterDeserialize(helper, obj, bp, value);
		}
	}
}
