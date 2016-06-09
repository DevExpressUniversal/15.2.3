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
using DevExpress.Data.PivotGrid;
using DevExpress.Utils;
namespace DevExpress.PivotGrid.OLAP {
	class XMLReaderCellSet : XmlReaderParserBase, IOLAPCellSet {
		readonly XMLReaderCell cell = new XMLReaderCell();
		public XMLReaderCellSet(XmlReader reader)
			: base(reader) {
			CheckException();
			reader.ReadStartElement();
			CheckException();
			reader.ReadToNextSibling(XmlaEntityName.Axes, XmlaName.XmlaMdDataSetNamespace);
			CheckException();
			reader.ReadStartElement();
			CheckException();
		}
		int index = 0;
		IEnumerable<IOLAPCell> IOLAPCellSet.Cells {
			get {
				if(reader.IsStartElement(XmlaEntityName.Axis, XmlaName.XmlaMdDataSetNamespace)) {
					while(reader.IsStartElement(XmlaEntityName.Axis, XmlaName.XmlaMdDataSetNamespace))
						reader.Skip();
				}
				reader.ReadEndElement();
				reader.ReadStartElement(XmlaEntityName.CellData);
				while(reader.IsStartElement(XmlaEntityName.Cell, XmlaName.XmlaMdDataSetNamespace)) {
					cell.Value = null;
					cell.Locale = -1;
					cell.FormatString = null;
					int curr_ordinal = Int32.Parse(reader.GetAttribute(XmlaAttributeName.CellOrdinal));
					cell.Locale = 1033;
					while(index != curr_ordinal) {
						index++;
						yield return null;
					}
					try {
						reader.ReadStartElement();
						while(reader.IsStartElement()) {
							if(reader.Name == OlapProperty.Value) {
								if(HasNillAttribute()) {
									reader.Skip();
									cell.Value = null;
								} else
									if(reader.IsEmptyElement) {
										reader.Skip();
										cell.Value = string.Empty;
									} else {
										string attr = reader.GetAttribute(XmlaAttributeName.type, XmlaName.HttpSchemaInstance);
										reader.ReadStartElement();
										if(reader.IsStartElement(XmlaEntityName.Error, XmlaName.XmlaMdDataSetNamespace)) {
											string errorText = reader.ReadInnerXml();
											cell.Value = PivotSummaryValue.ErrorValue;
										} else {
											cell.Value = XsdTypeConverter.GetConvertTo(attr)(reader);
										}
										reader.ReadEndElement();
									}
							} else
								if(reader.Name == OlapProperty.FormatString) {
									if(HasNillAttribute()) {
										reader.Skip();
									} else {
										reader.ReadStartElement();
										if(reader.IsStartElement(XmlaEntityName.Error, XmlaName.XmlaMdDataSetNamespace))
											reader.Skip();
										else
											cell.FormatString = OLAPFormatHelper.Intern(reader.ReadContentAsString());
										reader.ReadEndElement();
									}
								} else
									if(reader.Name == OlapProperty.LanguageProperty) {
										if(HasNillAttribute()) {
											reader.Skip();
										} else
											if(reader.IsEmptyElement) {
												reader.Skip();
											} else {
												reader.ReadStartElement();
												if(reader.NodeType == XmlNodeType.Element) {
													cell.Locale = 1033;
													reader.Skip();
												} else {
													try {
														cell.Locale = reader.ReadContentAsInt();
													} catch {
														cell.Locale = 1033;
													}
												}
												reader.ReadEndElement();
											}
									} else
										reader.Skip();
						}
						reader.ReadEndElement();
					} catch(Exception ex) {
						XmlReaderException outer = ex as XmlReaderException;
						if(outer == null)
							outer = new XmlReaderException(ex);
						throw outer;
					}
					if(cell.Value == null)
						yield return null;
					else
						yield return cell;
					index++;
				}
			}
		}
		ITupleCollection IOLAPCellSet.GetColumnAxis(AxisColumnsProviderBase axisColumnsProvider) {
			return new XMLReaderAxis(axisColumnsProvider, reader, true);
		}
		ITupleCollection IOLAPCellSet.GetRowAxis(AxisColumnsProviderBase axisColumnsProvider) {
			return new XMLReaderAxis(axisColumnsProvider, reader, false);
		}
		void IOLAPCellSet.OnParsed() {
			reader.Close();
		}
	}
}
