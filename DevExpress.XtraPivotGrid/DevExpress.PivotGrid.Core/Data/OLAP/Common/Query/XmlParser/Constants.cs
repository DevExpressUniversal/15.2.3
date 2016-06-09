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

namespace DevExpress.PivotGrid.OLAP {
	public static class XmlaName {
		public const string Body = "Body";
		public const string Envelope = "Envelope";
		public const string Header = "Header";
		public const string soap = "soap";
		public const string HttpEnvelope = "http://schemas.xmlsoap.org/soap/envelope/";
		public const string HttpSchema = "http://www.w3.org/2001/XMLSchema";
		public const string HttpSchemaInstance = HttpSchema + "-instance";
		public const string XmlnsBaseNamespace = "urn:schemas-microsoft-com:xml-analysis";
		public const string XmlnsSqlNamespace = "urn:schemas-microsoft-com:xml-sql";
		public const string XmlaExceptionNamespace = XmlnsBaseNamespace + ":exception";
		public const string XmlaMdDataSetNamespace = XmlnsBaseNamespace + ":mddataset";
		public const string XmlaRowSetNamespace = XmlnsBaseNamespace + ":rowset";
	}
	public static class XmlaEntityName {
		public const string Axis = "Axis";
		public const string Axes = "Axes";
		public const string Cell = "Cell";
		public const string CellData = "CellData";
		public const string Member = "Member";
		public const string Tuple = "Tuple";
		public const string Tuples = "Tuples";
		public const string Messages = "Messages";
		public const string Fault = "Fault";
		public const string faultActor = "faultactor";
		public const string faultCode = "faultcode";
		public const string detail = "detail";
		public const string faultString = "faultstring";
		public const string Error = "Error";
		public const string Command = "Command";
		public const string DiscoverMethodName = "Discover";
		public const string ExecuteMethodName = "Execute";
		public const string Properties = "Properties";
		public const string PropertyList = "PropertyList";
		public const string RequestType = "RequestType";
		public const string RestrictionList = "RestrictionList";
		public const string Restrictions = "Restrictions";
		public const string Statement = "Statement";
		public const string DiscoverResponse = "DiscoverResponse";
		public const string ExecuteResponse = "ExecuteResponse";
		public const string Session = "Session";
		public const string schema = "schema";
		public const string return_ = "return";
		public const string root = "root";
		public const string Row = "row";
		public const string complexType = "complexType";
		public const string sequence = "sequence";
		public const string element = "element";
	}
	public static class XmlaAttributeName {
		public const string CellOrdinal = "CellOrdinal";
		public const string Description = "Description";
		public const string ErrorCode = "ErrorCode";
		public const string Hierarchy = "Hierarchy";
		public const string field = "field";
		public const string MustUnderstand = "mustUnderstand";
		public const string name = "name";
		public const string SessionId = "SessionId";
		public const string Size = "Size";
		public const string Source = "Source";
		public const string type = "type";
		public const string Nil = "nil";
		public const string true_ = "true";
		public const string SlicerAxis = "SlicerAxis";
	}
}
