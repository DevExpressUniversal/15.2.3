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
using System.IO;
using System.Xml;
using System.Xml.Linq;
using DevExpress.Utils;
using DevExpress.PivotGrid.OLAP;
using DevExpress.PivotGrid.Xmla.Helpers;
namespace DevExpress.PivotGrid.Xmla {
	interface IXmlaMethodResult : IXmlaSoapElement {
		string SessionId { get; set; }
		object ResultData { get; }
		XmlaErrors EnsureMethodResult(string result, IResponseFormatProvider provider);
	}
	abstract class XmlaMethodResult : IXmlaMethodResult {
		string sessionId;
		XmlReader xmlReader;
		public abstract object ResultData { get; }
		public XmlReader XmlReader {
			get { return xmlReader; }
			set { xmlReader = value; }
		}
		public string SessionId {
			get { return this.sessionId; }
			set {
				if(value == this.sessionId)
					return;
				this.sessionId = value;
			}
		}
		public abstract string Name { get; }
		public abstract XmlaErrors EnsureMethodResult(string result, IResponseFormatProvider provider);
		public XmlaMethodResult() { }
		protected XmlaErrors EnsureErrors(string result, bool discover) {
			XmlaErrors errors = null;
			try {
				xmlReader = new XmlTextReader(new StringReader(result));
				xmlReader.ReadStartElement(XmlaName.Envelope, XmlaName.HttpEnvelope);
				if(xmlReader.IsStartElement(XmlaName.Header, XmlaName.HttpEnvelope)) {
					xmlReader.ReadStartElement();
					while(xmlReader.IsStartElement()) {
						if(xmlReader.IsStartElement(XmlaEntityName.Session))
							SessionId = xmlReader.GetAttribute(XmlaAttributeName.SessionId);
						xmlReader.Skip();
					}
					xmlReader.ReadEndElement();
				}
				xmlReader.ReadStartElement(XmlaName.Body, XmlaName.HttpEnvelope);
				if(CheckFault(ref errors))
					return errors;
				if(discover)
					xmlReader.ReadStartElement(XmlaEntityName.DiscoverResponse, XmlaName.XmlnsBaseNamespace);
				else
					xmlReader.ReadStartElement(XmlaEntityName.ExecuteResponse,XmlaName.XmlnsBaseNamespace);
				if(CheckFault(ref errors))
					return errors;
				xmlReader.ReadStartElement(XmlaEntityName.return_);
				if(CheckFault(ref errors))
					return errors;
			} catch(Exception exception) {
				errors = new XmlaErrors();
				errors.Add(new XmlaError("-1", exception.Message, exception.Source));
				if(xmlReader != null) {
					xmlReader.Close();
					xmlReader = null;
				}
			}
			return errors;
		}
		bool CheckFault(ref XmlaErrors errors) {
			if(xmlReader != null && (xmlReader.IsStartElement(XmlaEntityName.Fault) || xmlReader.IsStartElement("soap:Fault"))) {
				errors = XParser.GetErrors(XFinder.FindErrors(XDocument.Parse(xmlReader.ReadOuterXml())));
				xmlReader.Close();
				xmlReader = null;
				return true;
			}
			return false;
		}
	}
	class XmlaDiscoverResult : XmlaMethodResult {
		XElement elementResult;
		public override string Name { get { return XmlaEntityName.DiscoverResponse; } }
		public override object ResultData {
			get { return elementResult; }
		}
		public XElement Value {
			get { return this.elementResult; }
			set {
				if(value == this.elementResult)
					return;
				this.elementResult = value;
			}
		}
		public override XmlaErrors EnsureMethodResult(string resultString, IResponseFormatProvider provider) {
			XmlaErrors errors = EnsureErrors(resultString, true);
			if(errors == null) {
				XDocument document = XDocument.Parse(resultString);
				Value = XFinder.FindResult(document, this, provider);
			}
			return errors;
		}
	}
	class XmlaExecuteResult : XmlaMethodResult {
		public override string Name { get { return XmlaEntityName.ExecuteResponse; } }
		public override object ResultData {
			get { return XmlReader; }
		}
		public override XmlaErrors EnsureMethodResult(string result, IResponseFormatProvider provider) {
			return EnsureErrors(result, false);
		}		
	}
	class XmlaErrors {
		IList<XmlaError> errors;
		public void Add(XmlaError item) {
			if(this.errors == null)
				this.errors = new List<XmlaError>();
			this.errors.Add(item);
		}
		public int Count {
			get {
				if(errors != null)
					return errors.Count;
				return 0;
			}
		}
		public XmlaError this[int index] {
			get {
				if(this.errors == null)
					return null;
				return errors[index];
			}
		}
	}
	class XmlaError {
		string description, errorCode, source;
		public XmlaError(string errorCode, string description, string source) {
			this.description = description;
			this.errorCode = errorCode;
			this.source = source;
		}
		public string Description { get { return this.description; } set { this.description = value; } }
		public string ErrorCode { get { return this.errorCode; } set { this.errorCode = value; } }
		public string Source { get { return this.source; } set { this.source = value; } }
	}
}
