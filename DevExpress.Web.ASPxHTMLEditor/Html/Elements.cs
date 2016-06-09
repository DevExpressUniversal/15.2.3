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
namespace DevExpress.Web.ASPxHtmlEditor.Internal {
	public class BaseElement {
		protected BaseElement() {
		}
		protected BaseElement(string name, string value) {
			Name = name;
			Value = value;
		}
		public string Name { get; set; }
		public string Value { get; set; }
		public void Assign(string name, string value) {
			Name = name;
			Value = value;
		}
	}
	public sealed class Attribute : BaseElement {
		public Attribute()
			: base() {
		}
		public Attribute(string name, string value)
			: base(name, value) {
		}
	}
	public sealed class Node : BaseElement {
		internal const int AttributeStackGrowth = 10;
		internal Stack<Attribute> attributes = new Stack<Attribute>(AttributeStackGrowth);
		private XmlNodeType type;
		private bool isEmpty = true;
		public List<Node> ChildNodes = new List<Node>();
		public Node ParentNode;
		public Node()
			: base() {
		}
		public bool IsEmpty {
			get { return isEmpty; }
			set { isEmpty = value; }
		}
		public bool IsPCData {
			get {
				return type == XmlNodeType.Text || type == XmlNodeType.Whitespace || type == XmlNodeType.SignificantWhitespace ||
					type == XmlNodeType.CDATA || type == XmlNodeType.Comment || type == XmlNodeType.EntityReference;
			}
		}
		public XmlNodeType Type {
			get { return type; }
			set { type = value; }
		}
		public int AttributesCount {
			get { return attributes.Count; }
		}
		public Attribute AddAttribute(string name, string value) {
			Attribute attr;
			for(int i = 0; i < this.attributes.Count; i++) {
				attr = this.attributes[i];
				if(attr.Name == name)
					return null;
			}
			attr = attributes.PushSlot();
			if(attr == null)
				attributes.TopItemUnsafe = attr = new Attribute();
			attr.Assign(name, value);
			return attr;
		}
		public void Assign(string name, XmlNodeType type, string value) {
			base.Assign(name, value);
			this.type = type;
			this.isEmpty = true;
			this.attributes.Reset();
		}
		public Node CopyTo(Node node) {
			node.Assign(Name, Value);
			for(int i = 0; i < this.attributes.Count; i++) {
				Attribute attr = this.attributes[i];
				node.AddAttribute(attr.Name, attr.Value);
			}
			node.isEmpty = this.isEmpty;
			node.type = this.type;
			return node;
		}
		public Attribute GetAttribute(string name) {
			int count = this.attributes.Count;
			for(int i = 0; i < count; i++) {
				Attribute attr = this.attributes[i];
				if(attr.Name == name)
					return attr;
			}
			return null;
		}
		public Attribute GetAttribute(int i) {
			return (i >= 0 && i < this.attributes.Count) ? this.attributes[i] : null;
		}
		public int GetAttributeIndexByName(string name) {
			int count = this.attributes.Count;
			for(int i = 0; i < count; i++)
				if(this.attributes[i].Name == name)
					return i;
			return -1;
		}
#if DEBUG
		public override string ToString() {
			return string.Format("Node {0}[Type={1}, Value={2}] (attrs count = {3})", Name, Type, Value, AttributesCount);
		}
#endif
		public void RemoveAttribute(Attribute attr) {
			int attrIndex = GetAttributeIndexByName(attr.Name);
			this.attributes.RemoveAt(attrIndex);
		}
		public void Rename(string newName) {
			Name = newName;
		}
	}
}
