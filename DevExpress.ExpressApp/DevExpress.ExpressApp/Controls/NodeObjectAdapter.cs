#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       eXpressApp Framework                                        }
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
using System.Collections;
using System.Drawing;
using DevExpress.Persistent.Base;
namespace DevExpress.ExpressApp.Controls {
	public interface INodeObjectAdapterProvider {
		NodeObjectAdapter Adapter { get; }
	}
	public interface ISupportNodeDragDrop {
		void SetParent(object node, object newParentNode);
	}
	public abstract class NodeObjectAdapter {
		private IObjectSpace objectSpace;
		private object rootValue;
		protected virtual void OnChanged() {
			if(Changed != null) {
				Changed(this, EventArgs.Empty);
			}
		}
		public NodeObjectAdapter() {
			objectSpace = null;
			rootValue = null;
		}
		public abstract bool HasChildren(object nodeObject);
		public abstract object GetParent(object nodeObject);
		public abstract IEnumerable GetChildren(object nodeObject);
		public virtual Image GetImage(object nodeObject, out string imageName) {
			imageName = "";
			return null;
		}
		public abstract string GetDisplayPropertyValue(object nodeObject);
		public virtual bool IsRoot(object nodeObject) {
			return GetParent(nodeObject) == RootValue;
		}
		public abstract string DisplayPropertyName { get; }
		public object RootValue {
			get { return rootValue; }
			set {
				if(rootValue != value) {
					rootValue = value;
					OnChanged();
				}
			}
		}
		public IObjectSpace ObjectSpace {
			get { return objectSpace; }
			set {
				if(objectSpace != value) {
					objectSpace = value;
					OnChanged();
				}
			}
		}
		public event EventHandler Changed;
	}
	public class NodeObjectAdapterOnProperties : NodeObjectAdapter {
		private string parentPropertyName;
		private string childrenPropertyName;
		private string displayPropertyName;
		public NodeObjectAdapterOnProperties(string parentPropertyName, string childrenPropertyName, string displayPropertyName) {
			this.parentPropertyName = parentPropertyName;
			this.childrenPropertyName = childrenPropertyName;
			this.displayPropertyName = displayPropertyName;
		}
		public NodeObjectAdapterOnProperties(string parentPropertyName, string childrenPropertyName) : this(parentPropertyName, childrenPropertyName, string.Empty) { }
		public override bool HasChildren(object nodeObject) {
			IEnumerator enumerator = GetChildren(nodeObject).GetEnumerator();
			return enumerator.MoveNext();
		}
		public override object GetParent(object nodeObject) {
			return ReflectionHelper.GetMemberValue(nodeObject, parentPropertyName);
		}
		public override IEnumerable GetChildren(object nodeObject) {
			return (IEnumerable)ReflectionHelper.GetMemberValue(nodeObject, childrenPropertyName);
		}
		public override string GetDisplayPropertyValue(object nodeObject) {
			object result = ReflectionHelper.GetMemberValue(nodeObject, displayPropertyName);
			return (result == null) ? string.Empty : result.ToString();
		}
		public override string DisplayPropertyName {
			get { return displayPropertyName; }
		}
	}
}
