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
using System.Text;
using System.ComponentModel.Design;
using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms.Design;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Collections;
using System.Drawing;
namespace DevExpress.Design {
	class RealTimeSourcePropertiesEditor : CollectionEditor {
		public class Entry {
			string property;
			ITypedList list;
			public Entry() {
			}
			public Entry(ITypedList list, string val) {
				property = val;
				this.list = list;
			}
			[Browsable(false)]
			public ITypedList List {
				get {
					return list;
				}
			}
			[Browsable(false)]
			public string Name {
				get {
					return PropertyName == String.Empty || PropertyName == null ? "empty" : PropertyName;
				}
			}
			[Category()]
			[Editor(typeof(PropertyNameEditor), typeof(UITypeEditor))]
			public string PropertyName {
				get {
					return property;
				}
				set {
					property = value;
				}
			}
		}
		public RealTimeSourcePropertiesEditor()
			: base(typeof(object[])) {
		}
		protected override Type CreateCollectionItemType() {
			return typeof(Entry);
		}
		protected override object CreateInstance(Type itemType) {
			return new Entry(list, String.Empty);
		}
		protected override bool CanSelectMultipleInstances() {
			return false;
		}
		ITypedList list;
		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value) {
			try {
				DevExpress.Data.RealTimeSource source = context.Instance as DevExpress.Data.RealTimeSource;
				list = source.DataSource as ITypedList;
				return base.EditValue(context, provider, value);
			} finally {
				list = null;
			}
		}
		protected override object[] GetItems(object editValue) {
			string[] values = ((string)editValue).Split(';');
			object[] entries = new Entry[values.Length];
			for(int i = 0; i < values.Length; i++) {
				entries[i] = new Entry(list, values[i]);
			}
			return entries;
		}
		protected override object SetItems(object editValue, object[] value) {
			string[] values = new string[value.Length];
			for(int i = 0; i < values.Length; i++) {
				if(((Entry)value[i]).PropertyName != null && ((Entry)value[i]).PropertyName != String.Empty)
					values[i] = ((Entry)value[i]).PropertyName;
			}
			return string.Join(";", values);
		}
	}
	public class PropertyNameEditor : UITypeEditor {
		public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context) {
			return UITypeEditorEditStyle.DropDown;
		}
		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object obj) {
			DevExpress.Design.RealTimeSourcePropertiesEditor.Entry entry = context.Instance as DevExpress.Design.RealTimeSourcePropertiesEditor.Entry;
			ITypedList list = entry.List;
			if(context != null && list != null && provider != null) {
				IWindowsFormsEditorService edSvc = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
				if(edSvc != null) {
					PropertyPicker picker = new PropertyPicker();
					picker.Show(edSvc, list, (string)obj, true);
					string s = picker.SelectedItemPath;
					if((string)obj != s)
						return s;
				}
			}
			return obj;
		}
	}
	[ToolboxItemAttribute(false)]
	[DesignTimeVisibleAttribute(false)]
	public class PropertyPicker : TreeView {
		[ComVisible(false)]
		[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 0, CharSet = CharSet.Auto)]
		public struct TV_HITTESTINFO {
			public int pt_x;
			public int pt_y;
			public int flags;
			public int hItem;
		}
		const int TVM_HITTEST = 4369;
		const int TVHT_ONITEMLABEL = 4;
		const int WM_LBUTTONDOWN = 513;
		bool expansionSignClicked;
		TreeNode selectedItem;
		bool addRef;
		IWindowsFormsEditorService edSvc;
		int GetMaxItemWidth(TreeNodeCollection nodes) {
			int result = 0;
			foreach(TreeNode treeNode in nodes) {
				bool isExpanded = treeNode.IsExpanded;
				Rectangle rectangle = treeNode.Bounds;
				result = Math.Max(rectangle.Left + rectangle.Width, result);
				if(treeNode.IsExpanded)
					result = Math.Max(result, GetMaxItemWidth(treeNode.Nodes));
			}
			return result;
		}
		void SetSelectedItem(TreeNode node) {
			selectedItem = node;
		}
		void SelectNodeByPath(string selectedItemPath) {
			string[] path = selectedItemPath.Split('.');
			int level = 0;
			TreeNodeCollection nodes = base.Nodes;
			while((nodes.Count > 0) && (level < path.Length)) {
				foreach(TreeNode node in nodes) {
					if(node.Text.Equals(path[level])) {
						if(level < path.Length - 1) {
							nodes = node.Nodes;
							ExpandNode(node);
							node.Expand();
						} else {
							this.SelectedNode = node;
							SetSelectedItem(node);
						}
						break;
					}
				}
				level++;
			}
		}
		void FillData(string selectedItemPath, ITypedList list) {
			base.Nodes.Clear();
			FillNodeDataByTypedList(list, base.Nodes);
			if(selectedItemPath != null) {
				SelectNodeByPath(selectedItemPath);
			}
		}
		void FillNodeDataByTypedList(ITypedList path, TreeNodeCollection nodes) {
			SortedList list = new SortedList();
			foreach(PropertyDescriptor member in path.GetItemProperties(null)) {
				TreeNode node = new TreeNode(member.Name);
				node.Tag = member;
				list.Add(member.Name, node);
			}
			foreach(TreeNode node in list.Values)
				nodes.Add(node);
		}
		[System.Runtime.InteropServices.DllImport("user32.dll")]
		static extern IntPtr SendMessage(IntPtr hwnd, int msg, int wparam, ref TV_HITTESTINFO lparam);
		TreeNode GetNodeAtXAndY(int x, int y) {
			TV_HITTESTINFO tV_HITTESTINFO = new TV_HITTESTINFO();
			tV_HITTESTINFO.pt_x = x;
			tV_HITTESTINFO.pt_y = y;
			if(SendMessage(base.Handle, TVM_HITTEST, 0, ref tV_HITTESTINFO) == IntPtr.Zero) {
				return null;
			}
			if(tV_HITTESTINFO.flags == TVHT_ONITEMLABEL) {
				return base.GetNodeAt(x, y);
			} else {
				return null;
			}
		}
		protected override void OnAfterExpand(TreeViewEventArgs e) {
			base.OnAfterExpand(e);
			expansionSignClicked = false;
		}
		protected override void OnBeforeExpand(TreeViewCancelEventArgs e) {
			expansionSignClicked = true;
			ExpandNode(e.Node);
			base.OnBeforeExpand(e);
		}
		private void ExpandNode(TreeNode node) {
			if(node.Nodes[0].Text.Length == 0) {
				ITypedList path = (ITypedList)node.Nodes[0].Tag;
				node.Nodes.Clear();
				FillNodeDataByTypedList(path, node.Nodes);
				base.Width = GetMaxItemWidth(base.Nodes) + SystemInformation.VerticalScrollBarWidth * 3;
			}
		}
		protected override void OnBeforeCollapse(TreeViewCancelEventArgs e) {
			expansionSignClicked = true;
			base.OnBeforeCollapse(e);
		}
		protected override void OnAfterCollapse(TreeViewEventArgs e) {
			base.OnAfterCollapse(e);
			expansionSignClicked = false;
		}
		protected override void WndProc(ref Message m) {
			if(m.Msg == WM_LBUTTONDOWN) {
				base.WndProc(ref m);
				SetSelectedItem(GetNodeAtXAndY((m.LParam.ToInt32() & 0xffff), m.LParam.ToInt32() >> 16));
				if(selectedItem != null && !expansionSignClicked) {
					edSvc.CloseDropDown();
				}
				expansionSignClicked = false;
				return;
			}
			base.WndProc(ref m);
		}
		protected override bool IsInputKey(Keys key) {
			if(key == Keys.Return) {
				return true;
			} else {
				return base.IsInputKey(key);
			}
		}
		protected override void OnKeyUp(KeyEventArgs e) {
			base.OnKeyUp(e);
			if(e.KeyData == Keys.Return) {
				SetSelectedItem(base.SelectedNode);
				if(selectedItem != null) {
					edSvc.CloseDropDown();
				}
			}
		}
		public PropertyPicker() {
			this.selectedItem = null;
		}
		public void Show(IWindowsFormsEditorService edSvc, ITypedList list, string selectedItemPath, bool addRef) {
			this.edSvc = edSvc;
			this.addRef = addRef;
			expansionSignClicked = false;
			FillData(selectedItemPath, list);
			base.Width = GetMaxItemWidth(base.Nodes) + SystemInformation.VerticalScrollBarWidth * 2;
			edSvc.DropDownControl(this);
		}
		public string SelectedItemPath {
			get {
				string result = "";
				TreeNode node = selectedItem;
				while(node != null) {
					result = node.Text + (result.Length > 0 ? "." : "") + result;
					node = node.Parent;
				}
				return result;
			}
		}
	}
}
