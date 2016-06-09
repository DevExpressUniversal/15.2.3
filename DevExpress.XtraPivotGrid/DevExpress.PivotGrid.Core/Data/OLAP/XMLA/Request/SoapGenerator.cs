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
using DevExpress.PivotGrid.Xmla.Helpers;
namespace DevExpress.PivotGrid.Xmla {
	class XmlaSoapMessage<TResult> where TResult : IXmlaMethodResult {
		IXmlaHeader xmlaHeader;
		IXmlaMethod<TResult> xmlaMethod;
		internal XmlaSoapMessage(IXmlaHeader xmlaHeader, IXmlaMethod<TResult> xmlaMethod) {
			if(xmlaMethod == null)
				throw new ArgumentNullException("XmlaMethod");
			this.xmlaHeader = xmlaHeader;
			this.xmlaMethod = xmlaMethod;
		}
		public TResult CreateMethodResult() {
			return xmlaMethod.CreateResult();
		}
		public string Generate() {
			XElement elemEnvelope = new XElement(XName.Get(XmlaName.Envelope, XmlaName.HttpEnvelope));
			elemEnvelope.Add(new XAttribute(XNamespace.Xmlns + XmlaName.soap, XmlaName.HttpEnvelope));
			DoHeader(elemEnvelope);
			DoBody(elemEnvelope);
			return elemEnvelope.ToString();
		}
		void DoHeader(XElement parent) {
			XElement elemHeader = new XElement(parent.Name.Namespace + XmlaName.Header);
			parent.Add(elemHeader);
			if(this.xmlaHeader == null) return;
			XElement subElement = this.xmlaHeader.Generate();
			if(subElement != null)
				elemHeader.Add(subElement);
		}
		void DoBody(XElement parent) {
			XElement elemBody = new XElement(parent.Name.Namespace + XmlaName.Body);
			parent.Add(elemBody);
			XElement elemMethod = new XElement(XGetter.GetBase(xmlaMethod.Name));
			elemBody.Add(elemMethod);
			IEnumerable<XElement> subElements = this.xmlaMethod.Generate();
			foreach(XElement element in subElements) {
				elemMethod.Add(element);
			}
		}
	}
}
