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
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
using DevExpress.Utils;
using DevExpress.Utils.Design;
namespace DevExpress.XtraNavBar.Frames {
	public class XtraColors {
		public static void SetPanelBkColor(Control pnl) {
			SetPanelBkColor(pnl, true);
		}
		public static void SetPanelBkColor(Control pnl, bool allColors) {
			pnl.BackColor = ColorUtils.FlatBarBackColor;
			if(allColors)
				foreach(Control c in pnl.Controls) 
					c.BackColor = SystemColors.Control;
			SetTransparentBitmap(pnl);
		}
		public static void SetTransparentBitmap(Control pnl) {
			foreach(object obj in pnl.Controls) 
				if(obj is Button) {
					Button btn = (Button)obj;
					if(btn.Image != null)
						((Bitmap)btn.Image).MakeTransparent();
				} 
		}
	}
	public class DragItem {
		System.Windows.Forms.ListView.SelectedIndexCollection indexes;
		NavItemCollection items;
		private bool inner;
		public DragItem(System.Windows.Forms.ListView.SelectedIndexCollection indexes, NavItemCollection items) : this(indexes, items, false) {}
		public DragItem(System.Windows.Forms.ListView.SelectedIndexCollection indexes, NavItemCollection items, bool inner) {
			this.indexes = indexes;
			this.items = items;
			this.inner = inner;
		}
		public bool Inner { get { return inner; }}
		public NavBarItem[] Item {
			get { 
				NavBarItem[] ret = new NavBarItem[indexes.Count];
				for(int i = 0; i < indexes.Count; i++) 
					ret[i] = items[indexes[i]];
				return ret; 
			}
		}
	}
	public class DragInfo {
		NavBarItemLink dragLink;
		NavBarItemLink[] dragLinks;
		NavBarItem[] items;
		public DragInfo(NavBarItemLink[] links) {
			this.dragLinks = links;
			this.dragLink = null;
			this.items = new NavBarItem[links.Length];
			for(int n = 0; n < links.Length; n++) {
				NavBarItemLink link = links[n];
				if(this.dragLink == null) this.dragLink = link;
				this.items[n] = links[n].Item;
			}
		}
		public DragInfo(TreeNode[] nodes, NavItemCollection items)  {
			this.dragLink = null;
			this.items = new NavBarItem[nodes.Length];
			for(int i = 0; i < nodes.Length; i++)
				this.items[i] = nodes[i].Tag as NavBarItem;
		}
		public NavBarItemLink[] DragLinks {
			get {
				return dragLinks;
			}
		}
		public NavBarItemLink Link { get { return dragLink; } }
		public bool IsLinkDragging { get { return Link != null; }}
		public NavBarItem[] Items { get { return items; } }
	}
	public class MultiSelectTreeView : DXTreeView {
		protected override bool AllowNodeBranches(TreeNode checkNode) {
			return checkNode.Tag is NavBarItem;
		}
		protected override bool CanMultiSelectNode(TreeNode node) {
			return node.Tag is NavBarItem;
		}
		public void AddNodeToSelection(TreeNode node) {
			if(!IsNodeSelected(node)) ForceSelectNode(node);
			SetNodeSelection(node, !IsNodeSelected(node));
		}
		protected override bool UseThemes { get { return true; } }
	}
}
namespace DevExpress.XtraNavBar.Utils {
	internal class SkinHelper {
		public static bool InitSkins(IServiceProvider sp) {
			Type type = ProjectHelper.TryLoadType(sp, Info.TypeName);
			if(type == null) return false;
			MethodInfo mi = type.GetMethod(Info.MethodName, BindingFlags.Public | BindingFlags.Static);
			if(mi == null) return false;
			mi.Invoke(null, null);
			return true;
		}
		static SkinInitTypeInfo Info { get { return new SkinInitTypeInfo("DevExpress.UserSkins.BonusSkins", "Register"); } }
	}
	internal class SkinInitTypeInfo {
		public SkinInitTypeInfo(string typeName, string methodName) {
			this.TypeName = typeName;
			this.MethodName = methodName;
		}
		public string TypeName { get; private set; }
		public string MethodName { get; private set; }
	}
}
