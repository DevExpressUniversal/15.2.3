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
using DevExpress.XtraPrinting.DataNodes;
namespace DevExpress.Xpf.Printing.DataNodes {
	abstract class DataNodeBase : IDataNode {
		readonly Dictionary<int, IDataNode> childNodes;
		IDataNode root;
		protected IDataNode Root {
			get {
				if(root == null)
					root = GetRoot();
				return root;
			}
		}
		IDataNode GetRoot() {
			IDataNode item = this;
			while(item.Parent != null)
				item = item.Parent;
			return item;
		}
		protected DataNodeBase(IDataNode parent, int index) {
			childNodes = new Dictionary<int, IDataNode>();
			Parent = parent;
			Index = index;
		}
		#region IDataNode Members
		public int Index { get; private set; }
		public IDataNode Parent { get; private set; }
		public IDataNode GetChild(int index) {
			if(!CanGetChild(index))
				throw new ArgumentOutOfRangeException("index");
			IDataNode childNode;
			if(!childNodes.TryGetValue(index, out childNode)) {
				childNode = CreateChildNode(index);
				childNodes.Add(index, childNode);
			}
			return childNode;
		}
		public virtual bool PageBreakAfter {
			get { return false; }
		}
		public virtual bool PageBreakBefore {
			get { return false; }
		}
		public abstract bool CanGetChild(int index);
		public virtual bool IsDetailContainer { get { return false; } }
		#endregion
		protected abstract IDataNode CreateChildNode(int index);
		protected virtual int GetLevel() {
			int level = 0;
			for(IDataNode node = Parent; node.Parent != null; node = node.Parent) {
				level++;
			}
			return level;
		}
	}
}
