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
using System.Xml.Linq;
using DevExpress.PivotGrid.OLAP;
using DevExpress.PivotGrid.OLAP.SchemaEntities;
namespace DevExpress.PivotGrid.Xmla.Helpers {
	static class XGetter {
		public static string GetChildRowString(XElement element, string localName) {
			XElement childElement = element.Element(XGetter.GetRowSet(localName));
			return childElement == null ? null : (string)childElement;
		}
		public static int GetChildRowInt(XElement element, string localName) {
			XElement childElement = element.Element(XGetter.GetRowSet(localName));
			return childElement == null ? 0 : (int)childElement;
		}
		public static XName GetBase(string localName) {
			return XName.Get(localName, XmlaName.XmlnsBaseNamespace);
		}
		public static XName GetMdDataSet(string localName) {
			return XName.Get(localName, XmlaName.XmlaMdDataSetNamespace);
		}
		public static XName GetRowSet(string localName) {
			return XName.Get(localName, XmlaName.XmlaRowSetNamespace);
		}
		public static XName GetSchema(string localName) {
			return XName.Get(localName, XmlaName.HttpSchema);
		}
		public static void ForEachInRowSet(XContainer container, Action<XElement> action) {
			IEnumerable<XElement> elements = container.Elements(XGetter.GetRowSet(XmlaEntityName.Row));
			foreach(XElement elem in elements) {
				action(elem);
			}
		}
	}
	static class XParser {
		#region Schema
		public static IList<string> GetNames(XContainer container, XmlaConnection connection, string name) {
			List<string> namesList = new List<string>();
			XGetter.ForEachInRowSet(container, delegate(XElement elem) {
				namesList.Add(XGetter.GetChildRowString(elem, name));
			});
			return namesList;
		}
		public static void FillConnectionProps(XContainer container, XmlaConnection connection) {
			XGetter.ForEachInRowSet(container, delegate(XElement elem) {
				string name = XGetter.GetChildRowString(elem, OlapProperty.PropertyName),
					value = XGetter.GetChildRowString(elem, OlapProperty.Value);
				switch(name) {
					case OlapProperty.DBMSVersion:
						connection.ServerVersion = value;
						break;
					case OlapProperty.Catalog:
						connection.Database = value;
						break;
				}
			});
		}
		#endregion
		#region Errors (from <Fault> and <Messages>)
		public static XmlaErrors GetErrors(IEnumerable<XElement> errorElements) {
			if(errorElements == null)
				return null;
			XmlaErrors xmlaErrors = new XmlaErrors();
			foreach(XElement elem in errorElements) {
				xmlaErrors.Add(new XmlaError((string)elem.Attribute(XmlaAttributeName.ErrorCode),
					(string)elem.Attribute(XmlaAttributeName.Description),
					(string)elem.Attribute(XmlaAttributeName.Source)));
			}
			return xmlaErrors;
		}
		#endregion
	}
}
