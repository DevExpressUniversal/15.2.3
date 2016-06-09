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
using System.Xml;
namespace DevExpress.PivotGrid.OLAP {
	class XMLReaderAxis : XmlReaderParserBase, ITupleCollection {
		const string columnAxisName = XmlaEntityName.Axis + "0";
		const string rowAxisName = XmlaEntityName.Axis + "1";
		int count = 0;
		bool readed = false;
		readonly bool isColumn;
		readonly AxisColumnsProviderBase axisColumnsProvider;
		int ITupleCollection.ReadedCount {
			get {
				if(!readed)
					throw new NotImplementedException("not reader yet");
				return count;
			}
		}
		public XMLReaderAxis(AxisColumnsProviderBase axisColumnsProvider, XmlReader reader, bool isColumn)
			: base(reader) {
			this.isColumn = isColumn;
			this.axisColumnsProvider = axisColumnsProvider;
		}
		IEnumerator<IOLAPTuple> IEnumerable<IOLAPTuple>.GetEnumerator() {
			XmlMemberReaderStorage storage = new XmlMemberReaderStorage(axisColumnsProvider);
			string attribute = reader.GetAttribute(XmlaAttributeName.name);
			if(attribute != XmlaAttributeName.SlicerAxis && (attribute == columnAxisName || attribute == rowAxisName)) {
				if(isColumn && attribute != columnAxisName)
					throw new Exception("unexpected Axis");
				if(!isColumn && attribute == columnAxisName) {
					reader.Skip();
					attribute = reader.GetAttribute(XmlaAttributeName.name);
					if(attribute != rowAxisName)
						throw new Exception("unexpected Axis");
				}
				reader.ReadStartElement();
				if(!reader.IsEmptyElement) {
					reader.ReadStartElement(XmlaEntityName.Tuples, XmlaName.XmlaMdDataSetNamespace);
					while(reader.IsStartElement(XmlaEntityName.Tuple, XmlaName.XmlaMdDataSetNamespace)) {
						count++;
						XMLReaderTuple tuple = new XMLReaderTuple();
						try {
							reader.ReadStartElement();
							while(reader.IsStartElement(XmlaEntityName.Member, XmlaName.XmlaMdDataSetNamespace)) {
								string hName = reader.GetAttribute("Hierarchy");
								reader.ReadStartElement();
								tuple.Add(storage.Get(hName, reader, isColumn).Read());
								reader.ReadEndElement();
							}
							CheckException();
							if(!reader.IsStartElement(XmlaEntityName.Tuple, XmlaName.XmlaMdDataSetNamespace))
								reader.ReadEndElement();
						} catch(Exception ex) {
							XmlReaderException outer = ex as XmlReaderException;
							if(outer == null)
								outer = new XmlReaderException(ex);
							throw outer;
						}
						yield return tuple;
					}
					CheckException();
					reader.ReadEndElement();
				} else {
					CheckException();
					reader.ReadStartElement();
				}
				reader.ReadEndElement();
			}
			if(reader.GetAttribute(XmlaAttributeName.name) == XmlaAttributeName.SlicerAxis)
				reader.Skip();
			CheckException();
			readed = true;
		}
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
			throw new NotImplementedException();
		}
	}
}
