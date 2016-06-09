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
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
namespace DevExpress.Internal.WinApi.Window.Data.Xml.Dom {
	[CLSCompliant(false)]
	[ComImport]
	[Guid("F7F3A506-1E87-42D6-BCFB-B8C809FA5494")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	public interface IXmlDocument : IInspectable {
		void IInspectableStub1();
		void IInspectableStub2();
		void IInspectableStub3();
		void get_Doctype();
		void get_Implementation();
		void get_DocumentElement();
		int CreateElement([In, MarshalAs(47)] string name, out IXmlElement element);
		void CreateDocumentFragment();
		int CreateTextNode([In, MarshalAs(47)] string data,
						   out IXmlText newTextNode);
		void CreateComment();
		void CreateProcessingInstruction();
		void CreateAttribute();
		void CreateEntityReference();
		int GetElementsByTagName([In, MarshalAs(47)] string tagName, out IXmlNodeList elements);
	}
	[CLSCompliant(false)]
	[ComImport]
	[Guid("2DFB8A1F-6B10-4EF8-9F83-EFCCE8FAEC37")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	public interface IXmlElement : IInspectable {
		void IInspectableStub1();
		void IInspectableStub2();
		void IInspectableStub3();
		string TagName { get; }
		void GetAttribute();
		int SetAttribute([In, MarshalAs(47)] string attributeName,
						 [In, MarshalAs(47)] string attributeValue);
	}
	[CLSCompliant(false)]
	[ComImport]
	[Guid("5CC5B382-E6DD-4991-ABEF-06D8D2E7BD0C")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	public interface IXmlNodeSerializer : IInspectable {
		void IInspectableStub1();
		void IInspectableStub2();
		void IInspectableStub3();
		int GetXml([Out, MarshalAs(47)] out string xml);
	}
	[CLSCompliant(false)]
	[ComImport]
	[Guid("8C60AD77-83A4-4EC1-9C54-7BA429E13DA6")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	public interface IXmlNodeList : IInspectable {
		void IInspectableStub1();
		void IInspectableStub2();
		void IInspectableStub3();
		int Length { get; }
		int Item(UInt32 index, out IXmlNode node);
	}
	[ComImport]
	[Guid("1C741D59-2122-47D5-A856-83F3D4214875")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	public interface IXmlNode : IInspectable {
		void IInspectableStub1();
		void IInspectableStub2();
		void IInspectableStub3();
		void get_NodeValue();
		void put_NodeValue();
		void get_NodeType();
		void get_NodeName();
		void get_ParentNode();
		void get_ChildNodes();
		void get_FirstChild();
		void get_LastChild();
		void get_PreviousSibling();
		void get_NextSibling();
		IXmlNamedNodeMap Attributes { get; }
		void HasChildNodes();
		void get_OwnerDocument();
		void InsertBefore();
		void ReplaceChild();
		void RemoveChild();
		int AppendChild(IXmlNode childNode, out IXmlNode appendedChild);
	}
	[ComImport]
	[Guid("F931A4CB-308D-4760-A1D5-43B67450AC7E")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	public interface IXmlText { }
	[ComImport]
	[Guid("B3A69EB0-AAB0-4B82-A6FA-B1453F7C021B")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	public interface IXmlNamedNodeMap : IInspectable {
		void IInspectableStub1();
		void IInspectableStub2();
		void IInspectableStub3();
		void get_Length();
		void Item();
		int GetNamedItem([In, MarshalAs(47)] string name, out IXmlNode node);
	}
}
