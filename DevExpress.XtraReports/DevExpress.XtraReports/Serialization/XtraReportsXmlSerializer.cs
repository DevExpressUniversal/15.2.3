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
using System.Drawing;
using System.IO;
using System.Xml;
using DevExpress.Utils;
using DevExpress.Utils.Serializing;
using DevExpress.Utils.Serializing.Helpers;
using DevExpress.XtraPrinting.Native;
using DevExpress.XtraReports.Native;
using DevExpress.XtraReports.UI;
namespace DevExpress.XtraReports.Serialization {
	public class XtraReportsXmlSerializerBase : PrintingSystemXmlSerializer {
		#region inner classes
		class XRColorConverter : PrintingSystemXmlSerializer.ColorConverter {
			public new static readonly XRColorConverter Instance = new XRColorConverter();
			protected override string[] GetValues(object obj) {
				Color color = (Color)obj;
				return (color.IsSystemColor)
					? new string[] { color.Name }
					: base.GetValues(obj);
			}
		}
		#endregion
		#region static
		static XtraReportsXmlSerializerBase() {
			PrintingSystemXmlSerializer.RegisterConverter(PaddingInfoConverter.Instance);
			PrintingSystemXmlSerializer.RegisterConverter(BrickStringFormatConverter.Instance);
		}
		#endregion
		protected override IXtraPropertyCollection DeserializeCore(Stream stream, string appName, IList objects) {
			XmlReader xmlTextReader = CreateReader(stream);
			return new XtraReportsPropertyCollection(xmlTextReader);
		}
		protected override void WriteVersionAttribute(XmlWriter xmlTextWriter) {
			xmlTextWriter.WriteAttributeString("SerializerVersion", Version);
		}
		protected override string GetObjectTypeName(object obj) {
			return obj.GetType().AssemblyQualifiedName;
		}
		public void SerializeRootObject(object root, Stream stream) {
			PrintingSystemXmlSerializer.RegisterLocalConverter(XRColorConverter.Instance);
			try {
				SerializeObject(root, stream, "");
			} finally {
				PrintingSystemXmlSerializer.UnregisterLocalConverter(typeof(Color));
			}
		}
	}
	public class XtraReportsXmlSerializer : XtraReportsXmlSerializerBase {
		#region Nested Types
		class CustomSerializeHelper : SerializeHelper {
			protected internal new XtraReportsSerializationContext Context {
				get { return base.Context as XtraReportsSerializationContext; }
			}
			public CustomSerializeHelper() {
			}
			public CustomSerializeHelper(object rootObj)
				: base(rootObj) {
			}
			protected internal override void SerializeCollection(XtraSerializableProperty attr, string name, XtraPropertyInfoCollection props, object owner, XtraSerializationFlags parentFlags, OptionsLayoutBase options, ICollection list) {
				if(object.ReferenceEquals(Context.ObjectStorage, list)) {
					while(true) {
						int oldObjectCount = Context.ReferencedObjects.Count;
						base.SerializeCollection(attr, name, props, owner, parentFlags, options, list);
						if(oldObjectCount == Context.ReferencedObjects.Count)
							break;
						props.RemoveAt(props.Count - 1);
						Context.FillObjectStorage();
					}
				} else
					base.SerializeCollection(attr, name, props, owner, parentFlags, options, list);
			}
			protected internal override CollectionItemSerializationStrategy CreateCollectionItemSerializationStrategy(XtraSerializableProperty attr, string name, ICollection collection, object owner, XtraSerializationFlags parentFlags, OptionsLayoutBase options) {
				if(attr.Visibility == XtraSerializationVisibility.Collection) {
					return new CustomSerializationStrategy(this, name, collection, owner, parentFlags, options, attr);
				}
				return base.CreateCollectionItemSerializationStrategy(attr, name, collection, owner, parentFlags, options);
			}
		}
		class CustomSerializationStrategy : CollectionItemSerializationStrategyCollection {
			public CustomSerializationStrategy(SerializeHelper helper, string name, ICollection collection, object owner, XtraSerializationFlags parentFlags, OptionsLayoutBase options, XtraSerializableProperty attr) : 
				base(helper, name, collection, owner, parentFlags, options, attr) { 
			}
			protected internal override bool AssignItemPropertyValue(XtraPropertyInfo itemProperty, object item) {
				bool result = base.AssignItemPropertyValue(itemProperty, item);
				if(result) {
					XtraReport rootObject = Owner as XtraReport;
					if(rootObject != null && Reference.Equals(rootObject.ComponentStorage, Collection)) {
						XtraPropertyInfoCollection store = new XtraPropertyInfoCollection();
						string value;
						if(SerializationService.SerializeObject(item, out value, rootObject)) {
							store.Add(new XtraPropertyInfo("Content", typeof(string), value));
							store.Add(new XtraPropertyInfo("Type", typeof(string), item.GetType().FullName));
							IComponent component = item as IComponent;
							if(component != null) {
								string name = GetComponentName(component, rootObject);
								if(!string.IsNullOrEmpty(name)) {
									store.Add(new XtraPropertyInfo("Name", typeof(string), name));
									rootObject.ComponentNames[component] = name;
								} else
									rootObject.ComponentNames.Remove(component);
							} 
						}
						itemProperty.ChildProperties.AddRange(store);
					}
				}
				return result;
			}
			string GetComponentName(IComponent component, XtraReport rootObject) {
				string name;
				return component.Site != null ? component.Site.Name :
					rootObject.ComponentNames.TryGetValue(component, out name) ? name :
					string.Empty;
			}
		}
		#endregion
		public const string Name = "XtraReportsLayoutSerializer";
		readonly XtraReport report;
		protected override string SerializerName {
			get { return XtraReportsXmlSerializer.Name; }
		}
		public event EventHandler<DeserializeExceptionEventArgs> ExceptionOccurred;
		public XtraReportsSerializationContext Context { get; private set; }
		public XtraReportsXmlSerializer(XtraReport report) {
			this.report = report;
		}
		protected override SerializeHelper CreateSerializeHelper(object rootObj, bool useRootObj) {
			SerializeHelper helper = useRootObj ? new CustomSerializeHelper(rootObj) : new CustomSerializeHelper();
			InitializeContext(helper.Context as XtraReportsSerializationContext);
			return helper;
		}
		protected override DeserializeHelper CreateDeserializeHelper(object rootObj, bool useRootObj) {
			DeserializeHelper helper = base.CreateDeserializeHelper(rootObj, useRootObj);
			InitializeContext(helper.Context as XtraReportsSerializationContext);
			helper.ExceptionOccurred += (_, e) => RaiseExceptionOccurred(e);
			return helper;
		}
		void InitializeContext(XtraReportsSerializationContext context) {
			if(context != null)
				context.RootObject = report;
			Context = context;
		}
		void RaiseExceptionOccurred(DeserializeExceptionEventArgs args) {
			if(ExceptionOccurred != null) {
				ExceptionOccurred(this, args);
			}
		}
	}
	class StyleSheetXmlSerializer : XtraReportsXmlSerializerBase {
		protected override string SerializerName {
			get { return "StyleSheetSerializer"; }
		}
	}
	public class XRControlXmlSerializer : XtraReportsXmlSerializerBase {
		protected override string SerializerName {
			get { return "XRControlSerializer"; }
		}
		protected override SerializeHelper CreateSerializeHelper(object rootObj, bool useRootObj) {
			return new SerializeHelper(rootObj, CreateSerializationContext(rootObj));
		}
		protected override DeserializeHelper CreateDeserializeHelper(object rootObj, bool useRootObj) {
			return useRootObj ? new DeserializeHelper(rootObj, true, CreateSerializationContext(rootObj)) : new DeserializeHelper();
		}
		protected virtual ReportSerializationContextBase CreateSerializationContext(object rootObj) {
			return rootObj is IRootXmlObject ? new XtraReportsSerializationContext() { RootObject = (IRootXmlObject)rootObj } :
				new ReportSerializationContextBase();
		}
	}
	public static class XtraPropertyInfoExtentions {
		public static bool TryGetChildPropertyValue(this XtraPropertyInfo propertyInfo, string name, out string value) {
			XtraPropertyInfo childProperty = propertyInfo.ChildProperties[name];
			if(childProperty != null) {
				value = childProperty.Value as string;
				return !string.IsNullOrEmpty(value);
			} else {
				value = null;
				return false;
			}
		}
	}
	public class RootXmlObject : IRootXmlObject, IXtraSupportDeserializeCollectionItem {
		Dictionary<string, string> extensions = new Dictionary<string, string>();
		Collection<IObject> objectStorage = new Collection<IObject>();
		List<XRControl> controls = new List<XRControl>();
		public RootXmlObject() {
		}
		public RootXmlObject(IEnumerable<XRControl> controls) {
			if(controls != null)
				this.controls.AddRange(controls);
		}
		[
		XtraSerializableProperty(XtraSerializationVisibility.Collection, true, false, false, 0, XtraSerializationFlags.Cached),
		]
		public IList<XRControl> Controls {
			get { return controls; }
		}
		#region IRootXmlObject Members
		[
		XtraSerializableProperty(XtraSerializationVisibility.Collection, true, false, false, 1000, XtraSerializationFlags.Cached),
		]
		public Collection<IObject> ObjectStorage {
			get { return objectStorage; }
		}
		System.Collections.Generic.IDictionary<string, string> IExtensionsProvider.Extensions {
			get { return extensions; }
		}
		void IRootXmlObject.PerformAsLoad(Action0 action) {
			action();
		}
		#endregion
		#region IXtraSupportDeserializeCollectionItem Members
		object IXtraSupportDeserializeCollectionItem.CreateCollectionItem(string propertyName, XtraItemEventArgs e) {
			if(propertyName == XtraReportsSerializationNames.ObjectStorage) {
				string typeName;
				if(e.Item.TryGetChildPropertyValue("ObjectType", out typeName))
					return CreateInstance(typeName);
				return new ObjectStorageInfo();
			} else if(propertyName == "Controls")
				return CreateControl(e);
			return null;
		}
		static XRControl CreateControl(XtraItemEventArgs e) {
			if(e.Item.ChildProperties["ControlType"] != null)
				return (XRControl)Activator.CreateInstance(XRControlExtensions.GetObjectType((string)e.Item.ChildProperties["ControlType"].Value));
			return new XRControl();
		}
		static object CreateInstance(string typeName) {
			Type itemType = XRControlExtensions.GetObjectType(typeName);
			return Activator.CreateInstance(itemType);
		}
		void IXtraSupportDeserializeCollectionItem.SetIndexCollectionItem(string propertyName, XtraSetItemIndexEventArgs e) {
			if(propertyName == XtraReportsSerializationNames.ObjectStorage)
				ObjectStorage.Add((IObject)e.Item.Value);
			else if(propertyName == "Controls")
				Controls.Add(e.Item.Value as XRControl);
		}
		#endregion
	}
}
