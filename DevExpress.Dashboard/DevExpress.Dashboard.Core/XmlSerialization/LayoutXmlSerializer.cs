#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Dashboard                                                   }
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

using System.Xml;
using System.Xml.Linq;
namespace DevExpress.DashboardCommon.Native {
	public static class DashboardLayoutSerializer {
		public const string XmlLayoutTree = "LayoutTree";
		const string XmlDashboardItem = "DashboardItem";
		const string XmlWeight = "Weight";
		const string XmlOrientation = "Orientation";
		const string XmlLayoutGroup = "LayoutGroup";
		const string XmlLayoutItem = "LayoutItem";
		public static IDashboardLayoutNode LoadLayoutFromXml(XElement layoutTreeElement, IDashboardLayoutNodeFactory factory) {
			XElement layoutRootElement = layoutTreeElement.Element(XmlLayoutGroup);
			return LoadFromXml(layoutRootElement, factory);
		}
		public static XElement SaveLayoutToXml(IDashboardLayoutNode layoutRoot) {
			XElement layoutTreeElement = new XElement(XmlLayoutTree);
			SaveToXml(layoutTreeElement, layoutRoot);
			return layoutTreeElement;
		}
		static IDashboardLayoutNode LoadFromXml(XElement element, IDashboardLayoutNodeFactory factory) {
			IDashboardLayoutNode newNode = null;
			if(element.Name.LocalName == XmlLayoutGroup) {
				newNode = factory.CreateGroup();
				newNode.Orientation = GetOrientation(element);
				foreach(XElement clildElement in element.Elements())
					newNode.AddChildNode(LoadFromXml(clildElement, factory));
			} else if(element.Name.LocalName == XmlLayoutItem) {
				newNode = factory.CreateItem();
			} else
				throw new XmlException();
			newNode.DashboardItemName = XmlHelper.GetAttributeValue(element, XmlDashboardItem);
			newNode.Weight = GetWeight(element);
			return newNode;
		}
		static void SaveToXml(XElement parentElement, IDashboardLayoutNode node) {
			XElement element;
			if(node.IsGroup) {
				element = new XElement(XmlLayoutGroup);
				if(node.Orientation != DashboardLayoutGroup.DefaultOrientation)
					element.Add(new XAttribute(XmlOrientation, (DashboardLayoutGroupOrientation)node.Orientation));
				foreach(IDashboardLayoutNode childNode in node.ChildNodes)
					SaveToXml(element, childNode);
			} else
				element = new XElement(XmlLayoutItem);
			if(node.DashboardItemName != null)
				element.Add(new XAttribute(XmlDashboardItem, node.DashboardItemName));
			if(node.Weight != DashboardLayoutNode.DefaultWeight)
				element.Add(new XAttribute(XmlWeight, node.Weight));
			parentElement.Add(element);
		}
		static DashboardLayoutGroupOrientation GetOrientation(XElement element) {
			string orientationAttribute = element.GetAttributeValue(XmlOrientation);
			if(!string.IsNullOrEmpty(orientationAttribute))
				return XmlHelper.EnumFromString<DashboardLayoutGroupOrientation>(orientationAttribute);
			return DashboardLayoutGroup.DefaultOrientation;
		}
		static double GetWeight(XElement element) {
			string weightAttribute = XmlHelper.GetAttributeValue(element, XmlWeight);
			if(!string.IsNullOrEmpty(weightAttribute))
				return XmlHelper.FromString<double>(weightAttribute);
			return DashboardLayoutNode.DefaultWeight;
		}
	}
}
