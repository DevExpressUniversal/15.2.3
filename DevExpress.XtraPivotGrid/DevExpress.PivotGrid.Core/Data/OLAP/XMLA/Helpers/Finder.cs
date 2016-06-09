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

using System.Collections.Generic;
using System.Xml.Linq;
using DevExpress.PivotGrid.OLAP;
namespace DevExpress.PivotGrid.Xmla.Helpers {
	static class XFinder {
		public static IEnumerable<XElement> FindErrors(XDocument document) {
			string nameSpace = string.Empty;
			XContainer container = FindFault(document);
			if(container != null) {
				container = container.Element(XName.Get(XmlaEntityName.detail, XmlaName.HttpEnvelope));
				nameSpace = XmlaName.HttpEnvelope;
			} else {
				container = FindMessages(document);
				nameSpace = XmlaName.XmlaExceptionNamespace;
			}
			if(container == null)
				return null;
			return container.Elements(XName.Get(XmlaEntityName.Error, nameSpace));
		}
		static XElement FindFault(XDocument document) {
			return FindRecursive(document.Elements(), XmlaEntityName.Fault);
		}
		static XElement FindMessages(XDocument document) {
			return FindRecursive(document.Elements(), XmlaEntityName.Messages);
		}
		public static XElement FindResult(XDocument document, IXmlaMethodResult methodResult, IResponseFormatProvider responseFormatProvider) {
			XElement elementResult = FindRecursive(document.Elements(), methodResult.Name);
			if(elementResult == null)
				return null;
			XElement elementReturn = elementResult.Element(XGetter.GetBase(XmlaEntityName.return_));
			if(elementReturn == null)
				return null;
			return elementReturn.Element(XName.Get(XmlaEntityName.root, GetNameSpace(responseFormatProvider)));
		}
		public static string GetNameSpace(IResponseFormatProvider provider) {
			ResponseFormat responseFormat = provider != null ? provider.ResponseFormat : ResponseFormat.Tabular;
			switch(responseFormat) {
				case ResponseFormat.Multidimensional:
					return XmlaName.XmlaMdDataSetNamespace;
				default:
					return XmlaName.XmlaRowSetNamespace;
			}
		}
		static XElement FindRecursive(IEnumerable<XElement> childElements, string name) {
			foreach(XElement element in childElements) {
				if(name == element.Name.LocalName)
					return element;
				XElement subElement = XFinder.FindRecursive(element.Elements(), name);
				if(subElement != null)
					return subElement;
			}
			return null;
		}
	}
}
