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
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using DevExpress.DataAccess.UI.Localization;
using DevExpress.DataAccess.Wizard.Views;
using DevExpress.Utils;
using DevExpress.XtraTreeList;
using DevExpress.XtraTreeList.Nodes;
namespace DevExpress.DataAccess.UI.Wizard.Views {
	[ToolboxItem(false)]
	public partial class ChooseObjectTypePageView : WizardViewBase, IChooseObjectTypePageView {
		const string DataFieldName = "Data";
		const string HighlightedFieldName = "Highlighted";
		const string NodeTypeFieldName = "NodeType";
		bool showAll = true;
		public ChooseObjectTypePageView() {
			InitializeComponent();
			LocalizeComponent();
			ImageCollection images = new ImageCollection();
			images.AddImage(GetImage("Namespace.png"));
			images.AddImage(GetImage("Class.png"));
			images.AddImage(GetImage("Interface.png"));
			images.AddImage(GetImage("StaticClass.png"));
			images.AddImage(GetImage("AbstractClass.png"));
			treeList.SelectImageList = images;
		}
		public bool ShowAll {
			get { return showAll; }
			protected set {
				checkEditShowOnlyHighlighted.Checked = !value;
				if(value == showAll)
					return;
				showAll = value;
				treeList.OptionsBehavior.EnableFiltering = !value;
			}
		}
		#region Overrides of WizardViewBase
		public override string HeaderDescription {
			get { return DataAccessUILocalizer.GetString(DataAccessUIStringId.WizardPageChooseObjectType); }
		}
		protected override bool ProcessDialogKey(Keys keyData) {
			if(keyData == Keys.Back && treeList.Focused && treeList.State == TreeListState.IncrementalSearch)
				return false;
			return base.ProcessDialogKey(keyData);
		}
		#endregion
		#region Implementation of IChooseObjectTypePageView
		public void Initialize(IEnumerable<TypeViewInfo> items, bool showAll) {
			treeList.ClearNodes();
			treeList.BeginUnboundLoad();
			var trunk = new Dictionary<string, TreeListNode>();
			ShowAll = true;
			checkEditShowOnlyHighlighted.Enabled = false;
			foreach(TypeViewInfo item in items) {
				TreeListNode parent = GetNode(trunk, item.Namespace);
				AppendToList(item, parent, showAll);
			}
			treeList.EndUnboundLoad();
		}
		void AppendToList(TypeViewInfo item, TreeListNode parent, bool showAll) {
			TreeListNode node = treeList.AppendNode(new object[] { item.ClassType, 1, item.TypeName, item.Highlighted, item }, parent);
			if(item.Highlighted) {
				Highlight(parent);
				ShowAll = showAll;
				checkEditShowOnlyHighlighted.Enabled = true;
			}
			if(item.Nested != null)
				foreach(TypeViewInfo nested in item.Nested)
					AppendToList(nested, node, showAll);
		}
		static void Highlight(TreeListNode node) {
			node.SetValue(HighlightedFieldName, true);
			TreeListNode parent = node.ParentNode;
			if(parent != null)
				Highlight(parent);
		}
		public TypeViewInfo SelectedItem {
			get {
				var node = treeList.FocusedNode;
				if(node == null)
					return null;
				return node.GetValue(DataFieldName) as TypeViewInfo;
			}
			set { SetFocus(value); }
		}
		void SetFocus(TypeViewInfo value) {
			if(value != null) {
				if(!value.Highlighted)
					ShowAll = true;
				treeList.FocusedNode = treeList.FindNodeByFieldValue(DataFieldName, value);
			}
			else {
				TreeListNode soleDescendant = SoleDescendant();
				treeList.FocusedNode = soleDescendant;
				if(soleDescendant != null)
					soleDescendant.Expanded = true;
			}
			treeList.Focus();
		}
		TreeListNode SoleDescendant() {
			IEnumerable<TreeListNode> candidates = treeList.Nodes;
			if(!ShowAll)
				candidates = candidates.Where(node => (bool)node[HighlightedFieldName]);
			TreeListNode descendant = null;
			foreach(TreeListNode node in candidates) {
				if(descendant == null)
					descendant = node;
				else
					return null;
			}
			return descendant;
		}
		public event EventHandler Changed;
		#endregion
		protected virtual void OnTreeListCustomDrawNodeCell(CustomDrawNodeCellEventArgs e) {
			if(e.Node == treeList.FocusedNode) {
			treeList.Painter.DefaultPaintHelper.DrawNodeCell(e);
				e.Handled = true;
			}
		}
		TreeListNode GetNode(Dictionary<string, TreeListNode> trunk, string ns) {
			TreeListNode result;
			if(ns == null)
				return null;
			if(trunk.Any() && trunk.TryGetValue(ns, out result))
				return result;
			result = treeList.AppendNode(new object[] { TypeViewInfo.NodeType.Namespace, 0, ns, false, null }, null);
			trunk[ns] = result;
			return result;
		}
		protected void RaiseChanged() {
			if(Changed != null)
				Changed(this, EventArgs.Empty);
		}
		Image GetImage(string name) {
			return ResourceImageHelper.CreateImageFromResources("DevExpress.DataAccess.UI.Wizard.Images." + name, GetType().Assembly);
		}
		void LocalizeComponent() {
			checkEditShowOnlyHighlighted.Properties.Caption = DataAccessUILocalizer.GetString(DataAccessUIStringId.WizardPageChooseObjectType_ShowOnlyHighlighted);
		}
		void checkEditShowAll_CheckedChanged(object sender, EventArgs e) {
			ShowAll = !checkEditShowOnlyHighlighted.Checked;
		}
		void treeList_FocusedNodeChanged(object sender, FocusedNodeChangedEventArgs e) {
			RaiseChanged();
		}
		void treeList_DoubleClick(object sender, EventArgs e) {
			TreeListHitInfo hi = treeList.CalcHitInfo(treeList.PointToClient(MousePosition));
			if(hi.Node != null) {
				this.MoveForward();
			}
		}
		void treeList_CustomDrawNodeCell(object sender, CustomDrawNodeCellEventArgs e) {
			OnTreeListCustomDrawNodeCell(e);
		}
	}
}
