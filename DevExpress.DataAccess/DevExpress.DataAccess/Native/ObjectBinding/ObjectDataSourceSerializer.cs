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
using System.Diagnostics;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using DevExpress.Compatibility.System;
using DevExpress.DataAccess.Native.Sql;
using DevExpress.DataAccess.ObjectBinding;
using DevExpress.Utils;
using DevExpress.XtraReports.Native;
#if !DXPORTABLE
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
#endif
namespace DevExpress.DataAccess.Native.ObjectBinding {
	public class ObjectDataSourceSerializer : IDataSerializer {
		#region XML Tokens
		const string XmlObjectDataSource = "ObjectDataSource";
		const string XmlName = "Name";
		const string XmlDataSource = "DataSource";
		const string XmlDaraSourceType = "Type";
		const string XmlDataMember = "DataMember";
		const string XmlParameters = "Parameters";
		const string XmlConstructor = "Constructor";
		#endregion
		#region Save to XML
		public static XElement SaveToXml(ObjectDataSource data) { return SaveToXml(data, null); }
		public static XElement SaveToXml(ObjectDataSource data, IExtensionsProvider extensionProvider) {
			Guard.ArgumentNotNull(data, "data");
			XElement rootEl = new XElement(XmlObjectDataSource);
			SaveName(data, rootEl);
			SaveDataSource(data, rootEl, extensionProvider);
			SaveDataMember(data, rootEl);
			SaveParameters(data, rootEl, extensionProvider);
			SaveCtorParameters(data, rootEl, extensionProvider);
			return rootEl;
		}
		static void SaveName(ObjectDataSource sqlDataSource, XElement rootEl) {
			if (!string.IsNullOrEmpty(sqlDataSource.Name)) {
				XElement name = new XElement(XmlName) {Value = sqlDataSource.Name};
				rootEl.Add(name);
			}
		}
		static void SaveDataSource(ObjectDataSource data, XElement rootEl, IExtensionsProvider extensionsProvider) {
			if(data.DataSource == null)
				return;
			XElement dsEl = new XElement(XmlDataSource);
			Type type = data.ResolveType();
			XAttribute typeAt = new XAttribute(XmlDaraSourceType, type.AssemblyQualifiedName);
			dsEl.Add(typeAt);
			if(!(data.DataSource is Type)) {
				string value;
				if(SerializationService.SerializeObject(data.DataSource, out value, extensionsProvider))
					dsEl.Value = value;
				else
					return;
			}
			rootEl.Add(dsEl);
		}
		static void SaveDataMember(ObjectDataSource data, XElement rootEl) {
			if(data.DataMember == null)
				return;
			XElement memberEl = new XElement(XmlDataMember);
			memberEl.Value = data.DataMember;
			rootEl.Add(memberEl);
		}
		static void SaveParameters(ObjectDataSource data, XElement rootEl, IExtensionsProvider extensionsProvider) {
			if(data.Parameters == null || data.Parameters.Count == 0)
				return;
			XElement paramsEl = new XElement(XmlParameters);
			foreach(Parameter param in data.Parameters)
				paramsEl.Add(QuerySerializer.SerializeParameter(extensionsProvider, param));
			rootEl.Add(paramsEl);
		}
		static void SaveCtorParameters(ObjectDataSource data, XElement rootEl, IExtensionsProvider extensionsProvider) {
			if(data.Constructor == null)
				return;
			XElement ctorParamsEl = new XElement(XmlConstructor);
			foreach(Parameter param in data.Constructor.Parameters)
				ctorParamsEl.Add(QuerySerializer.SerializeParameter(extensionsProvider, param));
			rootEl.Add(ctorParamsEl);
		}
		#endregion
		#region Load from XML
		public static ObjectDataSource LoadFromXml(XElement element) { return LoadFromXml(element, null); }
		public static ObjectDataSource LoadFromXml(XElement element, IExtensionsProvider extensionsProvider) {
			ObjectDataSource ods = new ObjectDataSource();
			LoadFromXml(ods, element, extensionsProvider);
			return ods;
		}
		public static void LoadFromXml(ObjectDataSource target, XElement element) {
			LoadFromXml(target, element, null);
		}
		public static void LoadFromXml(ObjectDataSource target, XElement element, IExtensionsProvider extensionsProvider) {
			Guard.ArgumentNotNull(target, "target");
			Guard.ArgumentNotNull(element, "element");
			if(!string.Equals(element.Name.LocalName, XmlObjectDataSource, StringComparison.Ordinal))
				throw new ArgumentException(string.Format("Incorrect XML root element: <{0}>, <{1}> is expected.", element.Name.LocalName, XmlObjectDataSource));
			target.BeginUpdate();
			Clear(target);
			LoadName(target, element);
			LoadDataSource(target, element, extensionsProvider);
			LoadDataMember(target, element);
			LoadParameters(target, element, extensionsProvider);
			LoadCtorParameters(target, element, extensionsProvider);
			target.EndUpdate();
		}
		static void Clear(ObjectDataSource target) {
			target.DataSource = null;
			target.DataMember = null;
			target.Parameters.Clear();
			target.Constructor = null;
		}
		static void LoadName(ObjectDataSource target, XElement element) {
			var nameElement = element.Element(XmlName);
			if(nameElement != null)
				target.Name = nameElement.Value;
		}
		static void LoadDataSource(ObjectDataSource target, XElement element, IExtensionsProvider extensionsProvider) {
			XElement dsEl = element.Element(XmlDataSource);
			if(dsEl == null)
				return;
			Type type = Type.GetType(dsEl.GetAttributeValue(XmlDaraSourceType), true);
			string value = dsEl.Value;
			if(string.IsNullOrEmpty(value)) {
				target.DataSource = type;
				return;
			}
			object instance;
			if(SerializationService.DeserializeObject(value, type, out instance, extensionsProvider)) {
				target.DataSource = instance;
				return;
			}
#if !DXPORTABLE
			if(type.GetCustomAttributes(typeof(SerializableAttribute), false).Length > 0) {
				var formatter = new BinaryFormatter();
				byte[] buffer = Convert.FromBase64String(value.Trim());
				using(MemoryStream stream = new MemoryStream(buffer))
					target.DataSource = formatter.Deserialize(stream);
				return;
			}
#endif
			throw new NotSupportedException(string.Format("Cannot deserialize an instance of the specified type: {0}.", type.FullName));
		}
		static void LoadDataMember(ObjectDataSource target, XElement element) {
			XElement memberEl = element.Element(XmlDataMember);
			if(memberEl == null)
				return;
			target.DataMember = memberEl.Value.Trim();
		}
		static void LoadParameters(ObjectDataSource target, XElement element, IExtensionsProvider extensionsProvider) {
			XElement paramsEl = element.Element(XmlParameters);
			LoadParametersCore(target.Parameters, paramsEl, extensionsProvider);
		}
		static void LoadCtorParameters(ObjectDataSource target, XElement element, IExtensionsProvider extensionsProvider) {
			XElement ctorParamsEl = element.Element(XmlConstructor);
			if(ctorParamsEl == null)
				return;
			target.Constructor = new ObjectConstructorInfo();
			LoadParametersCore(target.Constructor.Parameters, ctorParamsEl, extensionsProvider);
		}
		static void LoadParametersCore(ParameterList target, XElement paramsEl, IExtensionsProvider extensionsProvider) {
			if(paramsEl == null)
				return;
			foreach(XElement paramEl in paramsEl.Elements(QuerySerializer.XML_Parameter)) {
				Parameter parameter = new Parameter();
				QuerySerializer.DeserializeParameter(parameter, paramEl, extensionsProvider);
				target.Add(parameter);
			}
		}
		#endregion
		#region Implementation of IDataSerializer
		bool IDataSerializer.CanSerialize(object data, object extensionProvider) {
			if(data == null)
				return false;
			return data.GetType() == typeof(ObjectDataSource);
		}
		string IDataSerializer.Serialize(object data, object extensionProvider) {
			return SaveToXml((ObjectDataSource)data, extensionProvider as IExtensionsProvider).ToString(SaveOptions.DisableFormatting);
		}
		bool IDataSerializer.CanDeserialize(string value, string typeName, object extensionProvider) {
			if(!string.Equals(typeName, typeof(ObjectDataSource).FullName, StringComparison.Ordinal))
				return false;
			try {
				return string.Equals(XElement.Parse(value).Name.LocalName, XmlObjectDataSource, StringComparison.Ordinal);
			}
			catch(XmlException) {
				return false;
			}
		}
		object IDataSerializer.Deserialize(string value, string typeName, object extensionProvider) {
			return LoadFromXml(XElement.Parse(value, LoadOptions.None), extensionProvider as IExtensionsProvider);
		}
		#endregion
	}
}
