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
	enum XmlaSoapHeaderType {
		Empty,
		Session,
		BeginSession,
		EndSession
	}
	interface IXmlaSoapElement {
		string Name { get; }
	}
	interface IXmlaMethod<TResult> : IXmlaSoapElement where TResult : IXmlaMethodResult {
		IEnumerable<XElement> Generate();
		TResult CreateResult();
	}
	interface IXmlaHeader : IXmlaSoapElement {
		XElement Generate();
		XmlaSoapHeaderType HeaderType { get; set; }
	}
	class XmlaHeader : IXmlaHeader {
		string sessionId;
		XmlaSoapHeaderType headerType;
		public XmlaHeader(string sessionId) {
			this.sessionId = sessionId;
			if(string.IsNullOrEmpty(sessionId)) {
				this.headerType = XmlaSoapHeaderType.BeginSession;
			} else {
				this.headerType = XmlaSoapHeaderType.Session;
			}
		}
		public string Name { get { return this.headerType.ToString(); } }
		public XmlaSoapHeaderType HeaderType { get { return this.headerType; } set { this.headerType = value; } }
		public string SessionId {
			get { return this.sessionId; }
			set { this.sessionId = value; }
		}
		public XElement Generate() {
			switch(HeaderType) {
				case XmlaSoapHeaderType.BeginSession:
				case XmlaSoapHeaderType.Session:
				case XmlaSoapHeaderType.EndSession:
					return GenerateSessionHeader();
			}
			return null;
		}
		XElement GenerateSessionHeader() {
			XElement elementHeader = new XElement((XNamespace)XmlaName.XmlnsBaseNamespace + Name);
			if(HeaderType != XmlaSoapHeaderType.BeginSession) {
				if(string.IsNullOrEmpty(SessionId))
					throw new ArgumentNullException("SessionId is not defined.");
				elementHeader.Add(new XAttribute(XmlaAttributeName.MustUnderstand, "1"));
				elementHeader.Add(new XAttribute(XmlaAttributeName.SessionId, SessionId));
			}
			return elementHeader;
		}
	}
	abstract class XmlaMethodBase<TResult> : IXmlaMethod<TResult> where TResult : IXmlaMethodResult, new() {
		XmlaProperties props;
		protected XmlaMethodBase() {
			this.props = new XmlaProperties();
		}
		public TResult CreateResult() { 
			return new TResult();
		}
		public abstract string Name { get; }
		public XmlaProperties Properties { get { return this.props; } }
		public void AddProperty(string propertyName, string propertyValue) {
			if(!this.props.Contains(propertyName))
				this.props.Add(new XmlaProperty(propertyName, propertyValue));
		}
		public IEnumerable<XElement> Generate() {
			List<XElement> elements = new List<XElement>();
			AddElementsCore(elements);
			elements.Add(GenerateProperties());
			return elements;
		}
		protected abstract void AddElementsCore(IList<XElement> elements);
		XElement GenerateProperties() {
			XElement elemProperties = new XElement(XGetter.GetBase(XmlaEntityName.Properties)),
				elemPropertyList = new XElement(elemProperties.Name.Namespace + XmlaEntityName.PropertyList);
			elemProperties.Add(elemPropertyList);
			foreach(XmlaProperty property in Properties) {
				elemPropertyList.Add(new XElement(XGetter.GetBase(property.Name), property.Value));
			}
			return elemProperties;
		}
	}
	sealed class XmlaDiscoverMethod : XmlaMethodBase<XmlaDiscoverResult> {
		readonly Dictionary<string, object> restrictions;
		string requestType;
		public XmlaDiscoverMethod(string requestType) {
			this.requestType = requestType;
			this.restrictions = new Dictionary<string, object>();
		}
		public override string Name { get { return XmlaEntityName.DiscoverMethodName; } }
		public string RequestType { get { return this.requestType; } }
		public Dictionary<string, object> Restrictions { get { return this.restrictions; } }
		protected override void AddElementsCore(IList<XElement> elements) {
			elements.Add(GenerateRequestType());
			elements.Add(GenerateRestrictions());
		}
		XElement GenerateRequestType() {
			XElement elemRequestType = new XElement((XNamespace)XmlaName.XmlnsBaseNamespace + XmlaEntityName.RequestType);
			elemRequestType.SetValue(RequestType);
			return elemRequestType;
		}
		XElement GenerateRestrictions() {
			XElement elemRestrictions = new XElement(XGetter.GetBase(XmlaEntityName.Restrictions)),
				elemRestrictionList = new XElement(elemRestrictions.Name.Namespace + XmlaEntityName.RestrictionList);
			foreach(KeyValuePair<string, object> pair in Restrictions) {
				elemRestrictionList.Add(new XElement(pair.Key, pair.Value));
			}
			elemRestrictions.Add(elemRestrictionList);
			return elemRestrictions;
		}
	}
	sealed class XmlaExecuteMethod : XmlaMethodBase<XmlaExecuteResult> {
		string statement;
		public XmlaExecuteMethod(string statement) {
			this.statement = statement;
		}
		public override string Name { get { return XmlaEntityName.ExecuteMethodName; } }
		public string Statement { get { return this.statement; } }
		protected override void AddElementsCore(IList<XElement> elements) {
			elements.Add(GenerateCommand());
		}
		XElement GenerateCommand() {
			XNamespace namespaceBase = XmlaName.XmlnsBaseNamespace;
			XElement elemCommand = new XElement(namespaceBase + XmlaEntityName.Command),
				elemStatement = new XElement(namespaceBase + XmlaEntityName.Statement);
			elemStatement.SetValue(Statement);
			elemCommand.Add(elemStatement);
			return elemCommand;
		}
	}
}
