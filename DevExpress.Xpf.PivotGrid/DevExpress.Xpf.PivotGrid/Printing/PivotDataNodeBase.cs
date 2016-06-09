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
using System.Collections.ObjectModel;
using System.Windows;
using DevExpress.Xpf.Printing;
using DevExpress.XtraPrinting.DataNodes;
namespace DevExpress.Xpf.PivotGrid.Printing {
	public abstract class PivotDataNodeBase : IVisualDetailNode {
		internal List<PivotDataNodeBase> childNodes;
		WeakReference parent;
		protected PivotDataNodeBase(){
			childNodes = new List<PivotDataNodeBase>();
		}
		public ReadOnlyCollection<PivotDataNodeBase> ChildNodes {
			get { return new ReadOnlyCollection<PivotDataNodeBase>(childNodes); }
		}
		public virtual IDataNode Parent {
			get { return parent.IsAlive ? (IDataNode)parent.Target : null; }
			protected set { parent = new WeakReference(value); }
		}
		public virtual void Add(PivotDataNodeBase node) {
			childNodes.Add(node);
			node.Parent = this;
		}
		public void Remove(PivotDataNodeBase node) {
			childNodes.Remove(node);
			if(node.Parent == this)
				node.Parent = null;
		}
		public void Remove(int index) {
			childNodes[index].Parent = null;
			childNodes.RemoveAt(index);
		}
		#region IDataNode
		public int Index {
			get {
				if(Parent == null)
					return -1;
				else
					return ((PivotDataNodeBase)Parent).ChildNodes.IndexOf(this);
			}
		}
		bool IDataNode.IsDetailContainer { get { return true; } }
		bool IDataNode.CanGetChild(int index) { return childNodes.Count > index; }
		IDataNode IDataNode.GetChild(int index) {
			if(childNodes.Count > index)
				return childNodes[index];
			throw new ArgumentOutOfRangeException();
		}
		bool IDataNode.PageBreakBefore { get { return false; } }
		bool IDataNode.PageBreakAfter { get { return false; } }
		#endregion
		#region IVisualDetailNode
		public abstract RowViewInfo GetDetail(bool allowContentReuse);
		#endregion
	}
}
