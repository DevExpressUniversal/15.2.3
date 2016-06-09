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
using DevExpress.Web.ASPxTreeList;
namespace DevExpress.Web.Mvc {
	public class MVCxTreeListNode : TreeListNode {
		internal static readonly object NewNodeKeyObject = new object();
		object keyObject;
		MVCxTreeListNodeCollection childNodes;
		Dictionary<string, object> nodeFields;
		public MVCxTreeListNode(object keyObject, Dictionary<string, object> nodeFields)
			: base() {
			KeyObject = keyObject;
			ChildNodes = new MVCxTreeListNodeCollection();
			NodeFields = nodeFields ?? new Dictionary<string, object>();
			AllowSelect = true;
			Expanded = false;
			Selected = false;
		}
		public new object this[string fieldName] {
			get { return NodeFields[fieldName]; }
			set {
				if(NodeFields.ContainsKey(fieldName))
					NodeFields[fieldName] = value;
				else
					NodeFields.Add(fieldName, value);
			}
		}
		public new bool AllowSelect { get; set; }
		public new bool Expanded { get; set; }
		public new bool Selected { get; set; }
		public new MVCxTreeListNodeCollection ChildNodes { 
			get { return childNodes; }
			set { childNodes = value; }
		}
		public object KeyObject {
			set { 
				keyObject = value;
				if(keyObject != MVCxTreeListNode.NewNodeKeyObject)
					SetKeyValue(keyObject);
			}
			get { return keyObject; } 
		}
		public Dictionary<string, object> NodeFields {
			get { return nodeFields; }
			set { nodeFields = value; }
		}
	}
	public class MVCxTreeListNodeCollection : List<MVCxTreeListNode> {
		public MVCxTreeListNodeCollection()
			: base() {
		}
		public MVCxTreeListNode Add(object keyObject, Dictionary<string, object> fieldsValues) {
			MVCxTreeListNode node = new MVCxTreeListNode(keyObject, fieldsValues);
			Add(node);
			return node;
		}
		public void Add(Action<MVCxTreeListNode> method) {
			method(Add());
		}
		public MVCxTreeListNode Add() {
			MVCxTreeListNode node = new MVCxTreeListNode(MVCxTreeListNode.NewNodeKeyObject, null);
			Add(node);
			return node;
		}
	}
}
