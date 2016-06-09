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
using DevExpress.Web;
namespace DevExpress.Web.Mvc {
	public class MVCxTreeViewNode: TreeViewNode {
		public MVCxTreeViewNode()
			: base() {
		}
		public MVCxTreeViewNode(string text)
			: base(text) {
		}
		public MVCxTreeViewNode(string text, string name)
			: base(text, name) {
		}
		public MVCxTreeViewNode(string text, string name, string imageUrl)
			: base(text, name, imageUrl) {
		}
		public MVCxTreeViewNode(string text, string name, string imageUrl, string navigateUrl)
			: base(text, name, imageUrl, navigateUrl) {
		}
		public MVCxTreeViewNode(string text, string name, string imageUrl, string navigateUrl, string target)
			: base(text, name, imageUrl, navigateUrl, target) {
		}
		public new MVCxTreeViewNodeCollection Nodes {
			get { return (MVCxTreeViewNodeCollection)base.Nodes; }
		}
		protected internal string TemplateContent { get; set; }
		protected internal Action<TreeViewNodeTemplateContainer> TemplateContentMethod { get; set; }
		protected internal string TextTemplateContent { get; set; }
		protected internal Action<TreeViewNodeTemplateContainer> TextTemplateContentMethod { get; set; }
		public void SetTemplateContent(Action<TreeViewNodeTemplateContainer> contentMethod) {
			TemplateContentMethod = contentMethod;
		}
		public void SetTemplateContent(string content) {
			TemplateContent = content;
		}
		public void SetTextTemplateContent(Action<TreeViewNodeTemplateContainer> contentMethod) {
			TextTemplateContentMethod = contentMethod;
		}
		public void SetTextTemplateContent(string content) {
			TextTemplateContent = content;
		}
		protected internal new string GetIndexPath() {
			return base.GetIndexPath();
		}
		protected override TreeViewNodeCollection CreateNodesCollection() {
			return new MVCxTreeViewNodeCollection(this);
		}
	}
	public class MVCxTreeViewNodeCollection: TreeViewNodeCollection {
		public new MVCxTreeViewNode this[int index] {
			get { return (GetItem(index) as MVCxTreeViewNode); }
		}
		public MVCxTreeViewNodeCollection()
			: base() {
		}
		public MVCxTreeViewNodeCollection(TreeViewNode node)
			: base(node) {
		}
		public void Add(Action<MVCxTreeViewNode> method) {
			method(Add());
		}
		public void Add(MVCxTreeViewNode node) {
			base.Add(node);
		}
		public new MVCxTreeViewNode Add() {
			MVCxTreeViewNode node = new MVCxTreeViewNode();
			Add(node);
			return node;
		}
		public new MVCxTreeViewNode Add(string text) {
			return Add(text, "", "", "", "");
		}
		public new MVCxTreeViewNode Add(string text, string name) {
			return Add(text, name, "", "", "");
		}
		public new MVCxTreeViewNode Add(string text, string name, string imageUrl) {
			return Add(text, name, imageUrl, "", "");
		}
		public new MVCxTreeViewNode Add(string text, string name, string imageUrl, string navigateUrl) {
			return Add(text, name, imageUrl, navigateUrl, "");
		}
		public new MVCxTreeViewNode Add(string text, string name, string imageUrl, string navigateUrl, string target) {
			MVCxTreeViewNode node = new MVCxTreeViewNode(text, name, imageUrl, navigateUrl, target);
			Add(node);
			return node;
		}
		public MVCxTreeViewNode GetVisibleNode(int index) {
			return GetVisibleItem(index) as MVCxTreeViewNode;
		}
		public int IndexOf(MVCxTreeViewNode node) {
			return base.IndexOf(node);
		}
		public void Insert(int index, MVCxTreeViewNode node) {
			base.Insert(index, node);
		}
		public void Remove(MVCxTreeViewNode node) {
			base.Remove(node);
		}
		public new MVCxTreeViewNode FindByName(string name) {
			int index = IndexOfName(name);
			return index != -1 ? this[index] : null;
		}
		public new MVCxTreeViewNode FindByText(string text) {
			int index = IndexOfText(text);
			return index != -1 ? this[index] : null;
		}
		protected override Type GetKnownType() {
			return typeof(MVCxTreeViewNode);
		}
	}
}
