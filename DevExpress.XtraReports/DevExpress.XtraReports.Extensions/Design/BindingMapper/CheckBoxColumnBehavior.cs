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
using DevExpress.XtraTreeList;
using System.Drawing;
using DevExpress.XtraTreeList.ViewInfo;
using DevExpress.XtraEditors.Repository;
using System.Windows.Forms;
using DevExpress.XtraTreeList.Nodes.Operations;
using DevExpress.XtraTreeList.Nodes;
using DevExpress.XtraEditors;
using DevExpress.XtraTreeList.Columns;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraEditors.Drawing;
namespace DevExpress.XtraReports.Design.BindingMapper {
	class CheckBoxColumnBehavior {
		class SetIsNodeSelectedOperation : TreeListOperation {
			readonly bool value;
			public SetIsNodeSelectedOperation(bool value) {
				this.value = value;
			}
			public override void Execute(TreeListNode node) {
				node.SetValue(0, value);
			}
		}
		TreeList treeList;
		TreeListColumn checkColumn;
		SimpleButton buttonOK;
		public void Attach(TreeList treeList, SimpleButton buttonOK, TreeListColumn checkColumn) {
			this.treeList = treeList;
			this.buttonOK = buttonOK;
			this.checkColumn = checkColumn;
			this.checkColumn.ColumnEdit.LookAndFeel.ParentLookAndFeel = treeList.LookAndFeel;
			treeList.MouseUp += treeList_MouseUp;
			treeList.CustomDrawColumnHeader += treeList_CustomDrawColumnHeader;
			checkColumn.ColumnEdit.EditValueChanged += checkColumnEdit_EditValueChanged;
		}
		public void OnRowChecked() {
			treeList.CloseEditor();
			buttonOK.Enabled = AnyNodeChecked;
			treeList.InvalidateColumnPanel();
		}
		public void Detach() {
			treeList.MouseUp -= treeList_MouseUp;
			treeList.CustomDrawColumnHeader -= treeList_CustomDrawColumnHeader;
			checkColumn.ColumnEdit.EditValueChanged -= checkColumnEdit_EditValueChanged;
		}
		void checkColumnEdit_EditValueChanged(object sender, EventArgs e) {
			OnRowChecked();
		}
		void treeList_CustomDrawColumnHeader(object sender, CustomDrawColumnHeaderEventArgs e) {
			if(e.Column != null && e.Column.VisibleIndex == 0) {
				ColumnInfo info = (ColumnInfo)e.ObjectArgs;
				info.Caption = string.Empty;
				e.Painter.DrawObject(info);
				DrawCheckBox(e.Graphics, checkColumn.ColumnEdit, e.Bounds, AllNodesChecked);
				e.Handled = true;
			}
		}
		static void DrawCheckBox(Graphics g, RepositoryItem edit, Rectangle bounds, bool isChecked) {
			using(CheckEditViewInfo info = edit.CreateViewInfo() as CheckEditViewInfo) {
				BaseEditPainter painter = edit.CreatePainter();
				info.EditValue = isChecked;
				info.Bounds = bounds;
				info.CalcViewInfo(g);
				ControlGraphicsInfoArgs args = new ControlGraphicsInfoArgs(info, new DevExpress.Utils.Drawing.GraphicsCache(g), bounds);
				painter.Draw(args);
				args.Cache.Dispose();
			}
		}
		bool AllNodesChecked {
			get {
				int count = 0;
				foreach(TreeListNode node in treeList.Nodes) {
					if(node.Visible) {
						count++;
						if(!Convert.ToBoolean(node.GetValue(checkColumn)))
							return false;
					}
				}
				return count > 0;
			}
		}
		bool AnyNodeChecked {
			get {
				foreach(TreeListNode node in treeList.Nodes)
					if(node.Visible && (bool)node.GetValue(checkColumn))
						return true;
				return false;
			}
		}
		void treeList_MouseUp(object sender, MouseEventArgs e) {
			TreeList tree = sender as TreeList;
			Point pt = new Point(e.X, e.Y);
			TreeListHitInfo hit = tree.CalcHitInfo(pt);
			if(hit.Column != null && hit.HitInfoType == HitInfoType.Column && hit.Column.Name == checkColumn.Name) {
				SetChecked(tree, !AllNodesChecked);
				buttonOK.Enabled = AnyNodeChecked;
			}
		}
		static void SetChecked(TreeList tree, bool value) {
			tree.BeginUpdate();
			tree.NodesIterator.DoOperation(new SetIsNodeSelectedOperation(value));
			tree.EndUpdate();
		}
	}
}
