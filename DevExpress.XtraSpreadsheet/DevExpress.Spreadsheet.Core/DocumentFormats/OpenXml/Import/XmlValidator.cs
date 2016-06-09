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
using System.Diagnostics;
using System.Text;
using DevExpress.Utils;
namespace DevExpress.XtraSpreadsheet.Import.OpenXml {
	#region XmlValidator
	static class XmlValidator {
		public static string ValidateXml(string xml) {
			XmlValidatorTag previousTag = null;
			XmlWordSplitter splitter = new XmlWordSplitter(xml);
			StringBuilder resultBuilder = new StringBuilder();
			while (splitter.Read()) {
				switch (splitter.CurrentState) {
					case XmlWordSplitterState.StartTag:
						XmlValidatorTag tag = new XmlValidatorTag();
						if (!tag.ReadFromSplitter(splitter))
							return xml; 
						if (previousTag != null &&
							StringExtensions.CompareInvariantCultureIgnoreCase(previousTag.Name, "br") == 0 &&
							StringExtensions.CompareInvariantCultureIgnoreCase(tag.Name, "br") != 0 &&
							previousTag.Type == XmlValidatorTagType.Open)
							resultBuilder.Append("</br>");
						resultBuilder.Append(tag.ValidatedStringValue);
						previousTag = tag;
						break;
					case XmlWordSplitterState.EndTag: 
						resultBuilder.Append("&gt;");
						break;
					case XmlWordSplitterState.BreakSymbol: 
						resultBuilder.Append(splitter.CurrentValue);
						break;
					case XmlWordSplitterState.SimpleString: 
					case XmlWordSplitterState.QoutedString:
						resultBuilder.Append(splitter.CurrentValue.Replace("&nbsp;", " "));
						break;
					default:
						return xml; 
				}
			}
			return resultBuilder.ToString();
		}
	}
	#endregion
	#region enum XmlValidatorTagType
	enum XmlValidatorTagType {
		Open,
		Close,
		SelfClosing,
	}
	#endregion
	#region enum XmlValidatorTagState
	enum XmlValidatorTagState {
		Initial = 0,
		Name = 1,
		AttributeName = 2,
		AttributeValue = 3,
		Break = 4,
		Finish = 5,
		Error = 6,
	}
	#endregion
	#region XmlValidatorTag
	class XmlValidatorTag {
		#region Fields
		string name;
		XmlValidatorTagType type;
		XmlValidatorTagState currentState;
		string validatedStringValue;
		byte[,] transitionMatrix = { 
								 { 6, 0, 6, 4, 1, 6, 6 }, 
								 { 6, 6, 5, 4, 2, 2, 6 }, 
								 { 6, 6, 6, 6, 3, 3, 6 }, 
								 { 6, 6, 5, 4, 2, 2, 6 }, 
								 { 6, 6, 5, 6, 1, 1, 6 }, 
								 { 6, 6, 6, 6, 6, 6, 6 }, 
								 { 6, 6, 6, 6, 6, 6, 6 }, 
								};
		#endregion
		#region Properties
		public XmlValidatorTagType Type { get { return type; } }
		public string Name { get { return name; } }
		public string ValidatedStringValue { get { return validatedStringValue; } }
		#endregion
		protected internal bool ReadFromSplitter(XmlWordSplitter splitter) {
			Debug.Assert(splitter.CurrentState == XmlWordSplitterState.StartTag);
			Dictionary<string, string> attributes = new Dictionary<string, string>();
			StringBuilder validatedValueBuilder = new StringBuilder(splitter.CurrentValue);
			currentState = XmlValidatorTagState.Initial;
			type = XmlValidatorTagType.Open;
			string currentAttribute = null;
			while (splitter.Read()) {
				currentState = (XmlValidatorTagState)transitionMatrix[(int)currentState, (int)splitter.CurrentState];
				if (currentState != XmlValidatorTagState.AttributeName && currentState != XmlValidatorTagState.AttributeValue)
					validatedValueBuilder.Append(splitter.CurrentValue);
				switch (currentState) {
					case XmlValidatorTagState.AttributeName: currentAttribute = splitter.CurrentValue.Trim(); break;
					case XmlValidatorTagState.AttributeValue:
						if (!attributes.ContainsKey(currentAttribute)) {
							attributes.Add(currentAttribute, splitter.CurrentValue);
							validatedValueBuilder.Append(currentAttribute);
							if (splitter.CurrentState == XmlWordSplitterState.QoutedString)
								validatedValueBuilder.Append(splitter.CurrentValue);
							else
								validatedValueBuilder.AppendFormat("\"{0}\" ", splitter.CurrentValue.Trim());
						}
						break;
					case XmlValidatorTagState.Name: name = splitter.CurrentValue; break;
					case XmlValidatorTagState.Break: type = string.IsNullOrEmpty(name) ? XmlValidatorTagType.SelfClosing : XmlValidatorTagType.Close; break;
					case XmlValidatorTagState.Finish: validatedStringValue = validatedValueBuilder.ToString(); return true;
					case XmlValidatorTagState.Error: return false;
				}
			}
			return false;
		}
	}
	#endregion
	#region enum XmlWordSplitterState
	enum XmlWordSplitterState {
		Initial,
		StartTag,	   
		EndTag,		 
		BreakSymbol,	
		SimpleString,   
		QoutedString,   
		Error,
	}
	#endregion
	#region XmlWordSplitter
	class XmlWordSplitter {
		#region Fields
		string xml;
		int position;
		string currentValue;
		XmlWordSplitterState currentState;
		#endregion
		public XmlWordSplitter(string xml) {
			this.xml = xml;
			currentState = XmlWordSplitterState.Initial;
			position = 0;
		}
		#region Properties
		public string CurrentValue { get { return currentValue; } }
		public XmlWordSplitterState CurrentState { get { return currentState; } }
		#endregion
		public bool Read() {
			int startPosition = position;
			if (!ReadCore())
				return false;
			while (position < xml.Length && (xml[position] == ' ' || xml[position] == '\r' || xml[position] == '\n')) {
				position++;
			}
			int count = position - startPosition;
			if (count <= 0) {
				position++;
				count = Math.Min(position - startPosition, xml.Length - startPosition);
			}
			currentValue = xml.Substring(startPosition, count);
			return true;
		}
		bool ReadCore() {
			if (position >= xml.Length || currentState == XmlWordSplitterState.Error)
				return false;
			currentState = XmlWordSplitterState.SimpleString;
			bool isQuotationMarksOpen = false;
			bool isSingleQuotationMarksOpen = false;
			char currentChar = xml[position];
			switch (currentChar) {
				case '/': currentState = XmlWordSplitterState.BreakSymbol; position++; return true;
				case '<': currentState = XmlWordSplitterState.StartTag; position++; return true;
				case '>': currentState = XmlWordSplitterState.EndTag; position++; return true;
			}
			while (position < xml.Length) {
				currentChar = xml[position];
				if (currentChar == '"' && !isSingleQuotationMarksOpen) {
					isQuotationMarksOpen = !isQuotationMarksOpen;
					if (isQuotationMarksOpen)
						currentState = XmlWordSplitterState.QoutedString;
					else {
						position++;
						break;
					}
				}
				else
					if (currentChar == '\'' && !isQuotationMarksOpen) {
						isSingleQuotationMarksOpen = !isSingleQuotationMarksOpen;
						if (isSingleQuotationMarksOpen)
							currentState = XmlWordSplitterState.QoutedString;
						else {
							position++;
							break;
						}
					}
					else if (!isQuotationMarksOpen && !isSingleQuotationMarksOpen) {
						if (currentChar == '/' || currentChar == '<' || currentChar == '>' || currentChar == ' ' || currentChar == '\r' || currentChar == '\n')
							return true;
						if (currentChar == '=') {
							position++;
							return true;
						}
					}
				position++;
			}
			if (isQuotationMarksOpen)
				currentState = XmlWordSplitterState.Error;
			return true;
		}
	}
	#endregion
}
