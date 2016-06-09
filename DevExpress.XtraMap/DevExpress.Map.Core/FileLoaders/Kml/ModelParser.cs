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

using DevExpress.Data.Svg;
using DevExpress.Map.Kml.Model;
using DevExpress.Map.Native;
using DevExpress.Utils;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
namespace DevExpress.Map.Kml {
	public abstract class KmlModelParserBase {
		public const int KmlBooleanTrue = 1;
		FormatElementFactory<Element> elementFactory;
		KmlModelConvert convert;
		protected IEnumerable<XElement> Styles { get; set; }
		protected IEnumerable<XElement> StyleMaps { get; set; }
		protected IEnumerable<XElement> Placemarks { get; set; }
		public string WorkingFolder { get; set; }
		public FormatElementFactory<Element> ElementFactory { get { return elementFactory; } }
		public KmlModelConvert Convert { get { return convert; } }
		public abstract KmlVersion Version { get; }
		protected KmlModelParserBase() {
			this.elementFactory = new FormatElementFactory<Element>();
			this.elementFactory.RegisterSourceTypes(GetType().GetAssembly());
			this.convert = new KmlModelConvert();
		}
		public virtual void Parse(Element element, XDocument doc) {
			Guard.ArgumentNotNull(element, "root");
			Guard.ArgumentNotNull(doc, "doc");
			Root root = element as Root;
			if (root == null)
				return;
			XNamespace ns = doc.Root.Name.Namespace;
			Styles = doc.Descendants(ns + KmlTokens.Style).Where(x => x.Parent.Name.LocalName == KmlTokens.Document);
			StyleMaps = doc.Descendants(ns + KmlTokens.StyleMap).Where(x => x.Parent.Name.LocalName == KmlTokens.Document);
			Placemarks = doc.Descendants(ns + KmlTokens.Placemark);
			CreateElements(Styles, root.Styles);
			CreateElements(StyleMaps, root.StyleMap);
			CreateElements(Placemarks, root.Placemarks);
		}
		void CreateElements<TElement>(IEnumerable<XElement> sourceElements, ElementList<TElement> targetList) where TElement : Element {
			foreach (XElement item in sourceElements) {
				Element el = CreateElement(item);
				targetList.Add(el as TElement);
				el.Parse(this, item);
			}
		}
		Element CreateElement(XElement node) {
			return ElementFactory.CreateInstance(node.Name.LocalName);
		}
		protected internal XAttribute GetAttributeByName(XElement node, string name) {
			return node.Attributes().Where(x => x.Name.LocalName == name).FirstOrDefault();
		}
		protected internal XElement GetElementByName(XElement node, string name) {
			return node.Elements().Where(x => x.Name.LocalName == name).FirstOrDefault();
		}
		protected internal XElement GetElementByName(XElement node, string[] names) {
			return node.Elements().Where(x => names.Contains(x.Name.LocalName)).FirstOrDefault();
		}
		protected Element CreateAndParseElementCore(XElement node) {
			if (node == null || ElementFactory == null)
				return null;
			Element el = ElementFactory.CreateInstance(node.Name.LocalName);
			if (el != null) el.Parse(this, node);
			return el;
		}
		protected List<T> CreateAndParseElements<T>(XElement node, string name) where T : Element {
			List<T> result = new List<T>();
			IEnumerable<XElement> children = node.Elements().Where(x => x.Name.LocalName == name);
			foreach (XElement item in children) {
				T el = CreateAndParseElementCore(item) as T;
				if (el != null) result.Add(el);
			}
			return result;
		}
		protected List<T> CreateAndParseElements<T>(XElement node, string[] names) where T : Element {
			List<T> result = new List<T>();
			IEnumerable<XElement> children = node.Elements().Where(x => names.Contains(x.Name.LocalName));
			foreach (XElement item in children) {
				T el = CreateAndParseElementCore(item) as T;
				if (el != null) result.Add(el);
			}
			return result;
		}
		public List<Geometry> CreateAndParseGeometries(XElement node, string[] names) {
			return CreateAndParseElements<Geometry>(node, names);
		}
		public List<Pair> CreateAndParsePairs(XElement node) {
			return CreateAndParseElements<Pair>(node, KmlTokens.Pair);
		}
		public List<ItemIcon> CreateAndParseItemIcons(XElement node) {
			return CreateAndParseElements<ItemIcon>(node, KmlTokens.ItemIcon);
		}
		public List<InnerBoundary> CreateAndParseInnerBoundaryIs(XElement node) {
			return CreateAndParseElements<InnerBoundary>(node, KmlTokens.InnerBoundaryIs);
		}
		public Element CreateAndParseElement(XElement node, string[] names) {
			XElement el = GetElementByName(node, names);
			return CreateAndParseElementCore(el);
		}
		public Element CreateAndParseElement(XElement node, string name) {
			XElement el = GetElementByName(node, name);
			return CreateAndParseElementCore(el);
		}
		public string ParseStringAttribute(XElement xml, string elementName, string defaultValue) {
			XAttribute el = GetAttributeByName(xml, elementName);
			return el != null ? Convert.ToString(el.Value, defaultValue) : null;
		}
		public string ParseStringElement(XElement xml, string elementName, string defaultValue) {
			XElement el = GetElementByName(xml, elementName);
			return el != null ? Convert.ToString(el.Value, defaultValue) : null;
		}
		public double ParseDoubleElement(XElement xml, string elementName, double defaultValue) {
			XElement el = GetElementByName(xml, elementName);
			return el != null ? Convert.ToDouble(el.Value, defaultValue) : defaultValue;
		}
		public double ParseDoubleAttribute(XElement xml, string elementName, double defaultValue) {
			XAttribute el = GetAttributeByName(xml, elementName);
			return el != null ? Convert.ToDouble(el.Value, defaultValue) : defaultValue;
		}
		public bool ParseBooleanElement(XElement xml, string elementName, bool defaultValue) {
			XElement el = GetElementByName(xml, elementName);
			return el != null ? Convert.ToInt(el.Value, KmlBooleanTrue) == KmlBooleanTrue : defaultValue;
		}
		public int ParseIntElement(XElement xml, string elementName, int defaultValue) {
			XElement el = GetElementByName(xml, elementName);
			return el != null ? Convert.ToInt(el.Value, defaultValue) : defaultValue;
		}
		public double ParseIntAttribute(XElement xml, string elementName, int defaultValue) {
			XAttribute el = GetAttributeByName(xml, elementName);
			return el != null ? Convert.ToInt(el.Value, defaultValue) : defaultValue;
		}
		public ColorABGR ParseColorABGR(XElement xml, string name, ColorABGR defaultColor) {
			XElement elColor = GetElementByName(xml, name);
			return (elColor != null) ? ColorABGR.Parse(elColor.Value) : defaultColor;
		}
		public ColorMode ParseColorMode(XElement xml, string name, ColorMode defaultMode) {
			XElement el = GetElementByName(xml, name);
			if (el == null) return defaultMode;
			return el.Value == KmlValueTokens.Random ? ColorMode.Random : defaultMode;
		}
		public Unit ParseUnit(XElement xml, string name, Unit defaultUnit) {
			XAttribute el = GetAttributeByName(xml, name);
			return (el != null) ? Convert.ToUnit(el.Value) : defaultUnit;
		}
		public Uri ParseUriElement(XElement xml, string name, Uri defaultValue) {
			XElement el = GetElementByName(xml, name);
			if(el == null)
				return defaultValue;
			return Convert.ToUri(el.Value, WorkingFolder) ?? defaultValue;
		}
		public Uri ParseStyleUrlElement(XElement xml, string name, Uri defaultValue) {
			XElement el = GetElementByName(xml, name);
			if (el == null) return defaultValue;
			string styleName = el.Value;
			if (KmlModelConvert.IsStyleReference(styleName))
				return Convert.ToRelativeUri(KmlModelConvert.ToStyleId(styleName));
			return Convert.ToUri(styleName, WorkingFolder);
		}
		public List<LatLonPoint> ParseCoordinates(XElement xml) {
			return Convert.ToLatLonPointList(xml.Value);
		}
		protected T ParseEnumType<T>(XElement xml, string name, T defaultValue) {
			XElement el = GetElementByName(xml, name);
			if (el == null) return defaultValue;
			return (T)Enum.Parse(typeof(T), el.Value, true);
		}
		public StyleState ParseStyleState(XElement xml, string name, StyleState defaultValue) {
			return ParseEnumType<StyleState>(xml, name, defaultValue);
		}
		public DisplayMode ParseDisplayMode(XElement xml, string name, DisplayMode defaultValue) {
			return ParseEnumType<DisplayMode>(xml, name, defaultValue);
		}
		public ListItemType ParseListItemType(XElement xml, string name, ListItemType defaultValue) {
			return ParseEnumType<ListItemType>(xml, name, defaultValue);
		}
		public IconItemMode ParseIconItemMode(XElement xml, string name, IconItemMode defaultValue) {
			return ParseEnumType<IconItemMode>(xml, name, defaultValue);
		}
	}
	public class KmlModelParser : KmlModelParserBase {
		public KmlModelParser() { 
		}
		public override KmlVersion Version { get { return KmlVersion.v22; } }
	}
}
