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

using DevExpress.Compatibility.System.Drawing;
using DevExpress.Utils;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Xml;
namespace DevExpress.Data.Svg {
	public class SvgImporter {
		readonly IList<SvgItem> svgItems = new List<SvgItem>();
		SvgRect bounds = SvgRect.Empty;
		XmlNode svgNode;
		public SvgRect Bounds { get { return bounds; } }
		public IList<SvgItem> SvgItems { get { return svgItems; } }
		public XmlNode SvgNode { get { return svgNode; } }
		void ProcessingElements(XmlNode node) {
			FormatElementFactory<SvgElement> elementsFactory = new FormatElementFactory<SvgElement>();
			foreach (XmlNode childNode in node.ChildNodes) {
				if (childNode.NodeType != XmlNodeType.Element) continue;
				XmlElement element = (XmlElement)childNode;
				string elementName = element.LocalName;
				SvgElement svgElement = elementsFactory.CreateInstance(elementName);
				if (svgElement != null) {
					SvgElementDataAgent dataAgent = new SvgElementDataAgent(element);
					svgElement.FillData(dataAgent);
					svgElement.FillStyle(dataAgent);
					SvgItems.Add(svgElement);
					SvgRect elementBounds = svgElement.GetBoundaryPoints();
					bounds = SvgRect.Union(bounds, elementBounds);
				}
				if (childNode.HasChildNodes)
					ProcessingElements(childNode);
			}
		}
		internal void StartProcessing(XmlNode xmlNode) {
			svgNode = xmlNode;
			svgItems.Clear();
			ProcessingElements(SvgNode);
		}
		public bool IsSvgNode(XmlNode xmlNode) {
			return xmlNode != null && xmlNode.LocalName == SvgTokens.Svg;
		}
		public bool SvgProcessing(XmlNode xmlNode) {
			if (!IsSvgNode(svgNode)) return false;
			StartProcessing(svgNode);
			return true;
		}
		public bool Import(Stream stream) {
			XmlDocument doc = new XmlDocument();
#if !DXPORTABLE
			doc.XmlResolver = null;
#endif
			doc.Load(stream);
			if (!IsSvgNode(doc.DocumentElement)) return false;
			StartProcessing(doc.DocumentElement);
			return true;
		}
	}
	public class SvgElementDataAgent {
		static readonly NumberStyles DecimalStyles = NumberStyles.AllowDecimalPoint | NumberStyles.AllowExponent | NumberStyles.AllowLeadingSign;
		static readonly NumberStyles NumberStyles = NumberStyles.Number;
		readonly XmlElement element;
		public XmlElement Element { get { return element; } }
		public SvgElementDataAgent(XmlElement svgElement) {
			element = svgElement;
		}
		public string GetString(string key) {
			return Element.GetAttribute(key);
		}
		public int GetInt(string key, int defaultValue, IFormatProvider provider) {
			string data = GetString(key);
			int result;
			if (String.IsNullOrEmpty(data) || !Int32.TryParse(data, NumberStyles, provider, out result))
				result = defaultValue;
			return result;
		}
		public int GetInt(string key, IFormatProvider provider) {
			return this.GetInt(key, 0, provider);
		}
		public double GetDouble(string key, double defaultValue, IFormatProvider provider) {
			string data = GetString(key);
			double result;
			if (String.IsNullOrEmpty(data) || !Double.TryParse(data, DecimalStyles, provider, out result))
				result = defaultValue;
			return result;
		}
		public double GetDouble(string key, IFormatProvider provider) {
			return this.GetDouble(key, 0.0, provider);
		}
		public Color GetColor(string key, Color defaultValue) {
			string data = GetString(key);
			if (data.Length == 4 && data[0] == '#')
				for (int i = 3; i > 0; i--)
					data = data.Insert(i, data[i].ToString());
			Color result = defaultValue;
			if (!String.IsNullOrEmpty(data) && data != SvgTokens.None) {
				try {
					result = DXColor.FromHtml(data);
				} catch { }
			}
			return result;
		}
		public Color GetColor(string key) {
			return this.GetColor(key, Color.Empty);
		}
	}
}
