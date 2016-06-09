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
using System.CodeDom.Compiler;
using System.Collections;
namespace DevExpress.XtraRichEdit.API.Word {
	#region XMLNode
	[GeneratedCode("Suppress FxCop check", "")]
	public interface XMLNode : IWordObject {
		string BaseName { get; }
		Range Range { get; }
		string Text { get; set; }
		string NamespaceURI { get; }
		string this[bool DataOnly] { get; }
		XMLNode NextSibling { get; }
		XMLNode PreviousSibling { get; }
		XMLNode ParentNode { get; }
		XMLNode FirstChild { get; }
		XMLNode LastChild { get; }
		Document OwnerDocument { get; }
		WdXMLNodeType NodeType { get; }
		XMLNodes ChildNodes { get; }
		XMLNodes Attributes { get; }
		string NodeValue { get; set; }
		bool HasChildNodes { get; }
		XMLNode SelectSingleNode(string XPath, string PrefixMapping, bool FastSearchSkippingTextNodes);
		XMLNodes SelectNodes(string XPath, string PrefixMapping, bool FastSearchSkippingTextNodes);
		XMLChildNodeSuggestions ChildNodeSuggestions { get; }
		WdXMLNodeLevel Level { get; }
		WdXMLValidationStatus ValidationStatus { get; }
		SmartTag SmartTag { get; }
		string PlaceholderText { get; set; }
		void Delete();
		void Copy();
		void RemoveChild(XMLNode ChildElement);
		void Cut();
		void Validate();
		void SetValidationError(WdXMLValidationStatus Status, ref object ErrorText, bool ClearedAutomatically);
		string WordOpenXML { get; }
	}
	#endregion
	#region XMLChildNodeSuggestion
	[GeneratedCode("Suppress FxCop check", "")]
	public interface XMLChildNodeSuggestion : IWordObject {
		string BaseName { get; }
		string NamespaceURI { get; }
		XMLSchemaReference XMLSchemaReference { get; }
		XMLNode Insert(ref object Range);
	}
	#endregion
	#region XMLSchemaReference
	[GeneratedCode("Suppress FxCop check", "")]
	public interface XMLSchemaReference : IWordObject {
		string NamespaceURI { get; }
		string Location { get; }
		void Delete();
		void Reload();
	}
	#endregion
	#region XMLNodes
	[GeneratedCode("Suppress FxCop check", "")]
	public interface XMLNodes : IWordObject, IEnumerable {
		int Count { get; }
		XMLNode this[int Index] { get; }
		XMLNode Add(string Name, string Namespace, ref object Range);
	}
	#endregion
	#region XMLChildNodeSuggestions
	[GeneratedCode("Suppress FxCop check", "")]
	public interface XMLChildNodeSuggestions : IWordObject, IEnumerable {
		int Count { get; }
		XMLChildNodeSuggestion this[object Index] { get; } 
	}
	#endregion
	#region WdXMLNodeType
	[GeneratedCode("Suppress FxCop check", "")]
	public enum WdXMLNodeType {
		wdXMLNodeAttribute = 2,
		wdXMLNodeElement = 1
	}
	#endregion
	#region WdXMLValidationStatus
	[GeneratedCode("Suppress FxCop check", "")]
	public enum WdXMLNodeLevel {
		wdXMLNodeLevelInline,
		wdXMLNodeLevelParagraph,
		wdXMLNodeLevelRow,
		wdXMLNodeLevelCell
	}
	#endregion
	#region WdXMLValidationStatus
	[GeneratedCode("Suppress FxCop check", "")]
	public enum WdXMLValidationStatus {
		wdXMLValidationStatusCustom = -1072898048,
		wdXMLValidationStatusOK = 0
	}
	#endregion
}
