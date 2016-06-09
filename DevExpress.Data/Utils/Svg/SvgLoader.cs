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
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;
namespace DevExpress.Utils.Svg {
	public class SvgLoader {
		public static SvgImage LoadFromFile(string path) {
			SvgImage result = null;
			using(Stream stream = new FileStream(path, FileMode.Open)) {
				result = LoadFromStream(stream);
			}
			return result;
		}
		public static SvgImage LoadFromStream(Stream stream) {
			using(XmlTextReader reader = new XmlTextReader(stream)) {
				reader.XmlResolver = new SvgDtdResolver();
				reader.WhitespaceHandling = WhitespaceHandling.None;
				return ParseDocument(reader);
			}
		}
		public static SvgImage ParseDocument(XmlReader reader) {
			SvgImage result = new SvgImage();
			var elementStack = new Stack<SvgElement>();
			var elements = result.Elements;
			SvgElement element = null;
			bool elementEmpty = false;
			while(reader.Read()) {
				try {
					elementEmpty = reader.IsEmptyElement;
					if(reader.NodeType == XmlNodeType.Element) {
						SvgElement parent = null;
						if(elementStack.Count > 0)
							parent = elementStack.Peek();
						element = SvgElementCreator.CreateElement(reader);
						if(element != null)
							elements.Add(element);
						if(parent != null) {
							element.Fill = element.Fill ?? parent.Fill;
							element.Opacity = element.Opacity ?? parent.Opacity;
						}
						if(!elementEmpty)
							elementStack.Push(element);
					}
					if(reader.NodeType == XmlNodeType.EndElement) {
						elementStack.Pop();
					}
				}
				catch { }
			}
			return result;
		}
	}
	public class SvgElementCreator {
		static Dictionary<string, TypeInfo> elementTypesCore;
		public static Dictionary<string, TypeInfo> ElementTypes {
			get {
				if(elementTypesCore == null) {
					var types = typeof(SvgElement).Assembly.GetExportedTypes().Where(x => x.GetCustomAttributes(typeof(SvgElementNameAliasAttribute), true).Length > 0).
						Select(x => new TypeInfo(x, ((SvgElementNameAliasAttribute)x.GetCustomAttributes(typeof(SvgElementNameAliasAttribute), true)[0]).Name));
					elementTypesCore = types.ToDictionary(x => x.Name, x => x);
				}
				return elementTypesCore;
			}
		}
		public static SvgElement CreateElement(XmlReader reader) {
			SvgElement result = null;
			string elementName = reader.LocalName;
			TypeInfo type = null;
			if(ElementTypes.TryGetValue(elementName, out type)) {
				result = (SvgElement)Activator.CreateInstance(type.Type);
			}
			if(result != null) {
				SetAttributes(result, reader);
			}
			return result;
		}
		static void SetAttributes(SvgElement element, XmlReader reader) {
			while(reader.MoveToNextAttribute()) {
				SetPropertyValue(element, reader.LocalName, reader.Value);
			}
		}
		static Dictionary<Type, Dictionary<string, PropertyDescriptor>> propertyDescriptors = new Dictionary<Type, Dictionary<string, PropertyDescriptor>>();
		internal static void SetPropertyValue(SvgElement element, string attributeName, string value) {
			var elementType = element.GetType();
			PropertyDescriptor property = null;
			if(propertyDescriptors.Keys.Contains(elementType)) {
				if(propertyDescriptors[elementType].Keys.Contains(attributeName)) {
					property = propertyDescriptors[elementType][attributeName];
				}
				else {
					property = AddPropeties(attributeName, elementType, property);
				}
			}
			else {
				propertyDescriptors.Add(elementType, new Dictionary<string, PropertyDescriptor>());
				property = AddPropeties(attributeName, elementType, property);
			}
			if(property != null) {
				try {
					property.SetValue(element, property.Converter.ConvertFrom(null, CultureInfo.InvariantCulture, value));
				}
				catch { }
			}
		}
		static PropertyDescriptor AddPropeties(string attributeName, Type elementType, PropertyDescriptor property) {
			var porperties = TypeDescriptor.GetProperties(elementType, new[] { new SvgPropertyNameAliasAttribute(attributeName) });
			if(porperties.Count > 0)
				property = porperties[0];
			propertyDescriptors[elementType].Add(attributeName, property);
			return property;
		}
	}
	public class TypeInfo {
		public TypeInfo(Type type) {
			Type = type;
		}
		public TypeInfo(Type type, string name) {
			Type = type;
			Name = name;
		}
		public string Name { get; set; }
		public Type Type { get; set; }
	}
	class SvgDtdResolver : XmlUrlResolver {
		public override object GetEntity(Uri absoluteUri, string role, Type ofObjectToReturn) {
			if(absoluteUri.ToString().IndexOf("svg", StringComparison.InvariantCultureIgnoreCase) > -1) {
				return MemoryStream.Null;
			}
			else {
				return base.GetEntity(absoluteUri, role, ofObjectToReturn);
			}
		}
	}
}
