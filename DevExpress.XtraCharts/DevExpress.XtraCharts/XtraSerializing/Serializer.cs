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
using System.IO;
using System.Text;
using System.Xml;
using System.ComponentModel;
using System.Drawing;
using System.Reflection;
using DevExpress.Utils;
using DevExpress.Utils.Serializing;
using DevExpress.Utils.Serializing.Helpers;
using DevExpress.XtraPrinting.Native;
namespace DevExpress.XtraCharts.Native {
	public class ChartXmlSerializer : XmlXtraSerializer {
		#region inner classes
		class ChartXmlPropertyInfo : XmlXtraPropertyInfo {
			protected override ObjectConverterImplementation ObjectConverterImplementation { get { return ChartObjectConverter; } }
			public ChartXmlPropertyInfo(string name, Type propertyType, object val, bool isKey) : base(name, propertyType, val, isKey) {
			}
		}
		class ImageConverter : IOneTypeObjectConverter {
			public static readonly ImageConverter Instance = new ImageConverter();
			public virtual Type Type { get { return typeof(Image); } }
			protected ImageConverter() {
			}
			public string ToString(object obj) {
				return Convert.ToBase64String(PSConvert.ImageToArray((Image)obj));
			}
			public object FromString(string str) {
				return PSConvert.ImageFromArray(Convert.FromBase64String(str));
			}
		}
		#endregion
		const string collectionItemNamePrefix = "Item";
		public const string Name = "ChartXmlSerializer";
		static readonly ObjectConverterImplementation ChartObjectConverter = new ObjectConverterImplementation();
		static bool IsCollectionItem(string name) {
			if(name.StartsWith(collectionItemNamePrefix)) { 
				int number;
				return int.TryParse(name.Substring(collectionItemNamePrefix.Length), out number);
			}
			return false;
		}
		static XtraPropertyInfo ReadInfo(XmlReader tr, bool skipZeroDepth) {
			tr.Read();
			if(tr.IsStartElement()) {
				XtraPropertyInfo info = new ChartXmlPropertyInfo(tr.Name, null, null, true);
				bool isEmptyElement = tr.IsEmptyElement;
				if(!(skipZeroDepth && tr.Depth == 0)) {
					while(tr.MoveToNextAttribute()) {
						Type type = null;
						if(tr.Name.EndsWith("_type")) {
							type = Type.GetType(tr.Value, false);
							tr.MoveToNextAttribute();
						}
						info.ChildProperties.Add(new ChartXmlPropertyInfo(tr.Name, type, tr.Value, false));
					}
				}
				if(isEmptyElement)
					return info;
				do {
					XtraPropertyInfo child = ReadInfo(tr, skipZeroDepth);
					if(child != null)
						info.ChildProperties.Add(child);
				} while(tr.NodeType != XmlNodeType.EndElement);
				tr.Read();
				return info;
			}
			return null;
		}
		static ChartXmlSerializer() {
			ChartObjectConverter.RegisterConverter(ImageConverter.Instance);
		}
		bool serializeVersion = true;
		protected override string SerializerName { get { return Name; } }
		protected override string Version { get { return serializeVersion ? AssemblyInfo.Version : string.Empty; } }
		public ChartXmlSerializer() : this(true) {
		}
		public ChartXmlSerializer(bool serializeVersion) : base() {
			this.serializeVersion = serializeVersion;
		}
		protected override void WriteApplicationAttribute(string appName, XmlWriter tw) {
		}
		protected override void WriteStartDocument(XmlWriter tw) {
			tw.WriteStartDocument();
		}
		protected override void SerializeLevelCore(XmlWriter tw, IXtraPropertyCollection props) {
			if(!props.IsSinglePass)
				foreach(XtraPropertyInfo p in props)
					if(!p.IsKey)
						SerializeAttributeProperty(tw, p);
			foreach(XtraPropertyInfo p in props)
				if(p.IsKey)
					SerializeContentProperty(tw, p);
		}
		void SerializeAttributeProperty(XmlWriter tw, XtraPropertyInfo p) {
			object val = p.Value;
			if(val != null) {
				Type type = val.GetType();
				if(!type.IsPrimitive)
					val = ChartObjectConverter.ObjectToString(val);
			}
			if(p.Value != null) {
				if(object.Equals(p.PropertyType, typeof(object)))
					tw.WriteAttributeString(p.Name + "_type", p.Value.GetType().FullName);
				tw.WriteAttributeString(p.Name, XmlObjectToString(val));
			}
		}
		void SerializeContentProperty(XmlWriter tw, XtraPropertyInfo p) {
			if(p.ChildProperties.Count > 0 || IsCollectionItem(p.Name)) {
				tw.WriteStartElement(p.Name.Replace("$", string.Empty));
				SerializeLevel(tw, p.ChildProperties);
				tw.WriteEndElement();
			}
		}
		protected override IXtraPropertyCollection DeserializeCore(Stream stream, string appName, IList objects) {
			IXtraPropertyCollection propertyCollection = new XtraPropertyCollection();
			XmlReader tr = CreateReader(stream);
			XtraPropertyInfo propertyInfo = ReadInfo(tr, true);
			propertyCollection.Add(propertyInfo);
			return propertyCollection;
		}
	}
	public class ChartSerializationContext : SerializationContext {
		static XtraPropertyInfo GetRootPropertyInfo(IXtraPropertyCollection store) {
			if(store.Count == 1) {
				XtraPropertyInfo serializerInfo = store[0];
				if(serializerInfo.ChildProperties.Count == 1)
					return serializerInfo.ChildProperties[0];
				else if(serializerInfo.ChildProperties.Count == 0)
					return null;
			}
			throw new XmlException();
		}
		protected
#if DXCommon
		internal
#endif
		override int GetCollectionItemsCount(XtraPropertyInfo root) {
			return root.ChildProperties.Count;
		}
		protected
#if DXCommon
		internal
#endif
		override object InvokeCreateCollectionItem(DeserializeHelper helper, string propertyName, XtraItemEventArgs e) {
			IXtraSupportDeserializeCollectionItem supportDeserializeCollectionItem = e.Owner as IXtraSupportDeserializeCollectionItem;
			if(supportDeserializeCollectionItem != null)
				return supportDeserializeCollectionItem.CreateCollectionItem(propertyName, e);
			return null;
		}
		protected
#if DXCommon
		internal
#endif
		override object InvokeCreateContentPropertyValueMethod(DeserializeHelper helper, XtraItemEventArgs e, PropertyDescriptor prop) {
			IXtraSupportCreateContentPropertyValue supportCreateContentPropertyValue = e.Owner as IXtraSupportCreateContentPropertyValue;
			if(supportCreateContentPropertyValue != null)
				return supportCreateContentPropertyValue.Create(e);
			return null;
		}
		protected
#if DXCommon
		internal
#endif
		override void InvokeAfterDeserialize(DeserializeHelper helper, object obj, XtraPropertyInfo bp, object value) {
			IXtraSupportAfterDeserialize supportAfterSerialize = obj as IXtraSupportAfterDeserialize;
			if(supportAfterSerialize != null) {
				bp.Value = value;
				supportAfterSerialize.AfterDeserialize(new XtraItemEventArgs(null, obj, null, bp));
			}
		}
		protected
#if DXCommon
		internal
#endif
		override void DeserializeObjectsCore(DeserializeHelper helper, IList objects, IXtraPropertyCollection store, OptionsLayoutBase options) {
			if(objects.Count == 1) {
				XtraObjectInfo rootObjectInfo = objects[0] as XtraObjectInfo;
				if(rootObjectInfo != null) {
					XtraPropertyInfo rootPropertyInfo = GetRootPropertyInfo(store);
					if(rootPropertyInfo != null) {
						if(rootObjectInfo.Name == rootPropertyInfo.Name) {
							helper.DeserializeObject(rootObjectInfo.Instance, rootPropertyInfo.ChildProperties, options);
							return;
						}
					}
					else
						return;
				}
			}
			throw new XmlException();
		}
		protected
#if DXCommon
		internal
#endif
		override bool ShouldSerializeCollectionItem(SerializeHelper helper, object owner, MethodInfo mi, XtraPropertyInfo itemProperty, object item, XtraItemEventArgs e) {
			IXtraSupportShouldSerializeCollectionItem supportShouldSerializeCollectionItem = owner as IXtraSupportShouldSerializeCollectionItem;
			if(supportShouldSerializeCollectionItem != null) {
				itemProperty.Value = item;
				try {
					return supportShouldSerializeCollectionItem.ShouldSerializeCollectionItem(e);
				}
				finally {
					itemProperty.Value = null;
				}
			}
			return true;
		}
	}
}
