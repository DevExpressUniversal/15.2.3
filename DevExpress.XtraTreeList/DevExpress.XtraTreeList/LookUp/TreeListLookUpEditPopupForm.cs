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
using System.Windows.Forms;
using System.ComponentModel;
using System.Drawing;
using DevExpress.XtraTreeList;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraEditors.Controls;
using System.IO;
namespace DevExpress.XtraEditors.Popup {
	[ToolboxItem(false)]
	public class TreeListLookUpEditPopupForm : CustomBlobPopupForm {
		public readonly Size EmptySize = new Size(400, 300);
		public TreeListLookUpEditPopupForm(TreeListLookUpEdit ownerEdit)
			: base(ownerEdit) {
			Controls.Add(TreeList);
			ViewInfo.ShowSizeBar = Properties.ShowFooter;
		}
		public new TreeListLookUpEdit OwnerEdit { get { return base.OwnerEdit as TreeListLookUpEdit; } }
		public new RepositoryItemTreeListLookUpEdit Properties { get { return OwnerEdit.Properties as RepositoryItemTreeListLookUpEdit; } }
		protected TreeList TreeList { get { return Properties.TreeList; } }
		protected override Control EmbeddedControl { get { return TreeList; } }
		protected override Size DefaultEmptySize { get { return EmptySize; } }
		public override object ResultValue { get { return GetResultValue(); } }
		public override bool AllowSizing { get { return Properties.PopupSizeable; } set { } }
		protected virtual bool ShouldSaveTreeListLayout { get { return OwnerEdit != null && OwnerEdit.InplaceType != InplaceType.Standalone; } }
		protected bool IsFindPanelVisible { get { return TreeList.FindPanelVisible && TreeList.FindPanel != null; } }
		protected bool IsFindPanelActive { get { return IsFindPanelVisible && TreeList.FindPanel.ContainsFocus; } }
		public override bool FormContainsFocus {
			get {
				if(base.FormContainsFocus) return true;
				if(TreeList.Focused) return true;
				return false;
			}
		}
		protected override Size DefaultBlobFormSize {
			get {
				Size size = base.DefaultBlobFormSize;
				int bestWidth = CalcBestPopupFormWidth(size);
				if(bestWidth != 0)
					size.Width = bestWidth;
				return size;
			}
		}
		protected virtual int CalcBestPopupFormWidth(Size defaultSize) {
			if(Properties.BestFitMode == BestFitMode.None || IsPopupWidthStored) return 0;
			object savedWidth = Properties.PropertyStoreCore[LookUpPropertyNames.PopupBestWidth];
			if(savedWidth != null) 
				return (int)savedWidth;
			int resultWidth = 0;
			if(Properties.BestFitMode == BestFitMode.BestFit) 
				TreeList.BestFitColumns();
			else 
				resultWidth = CalcTreeListBestFitWidth();
			Properties.PropertyStoreCore[LookUpPropertyNames.PopupBestWidth] = resultWidth;
			return resultWidth;
		}
		int CalcTreeListBestFitWidth() {
			int result = 0;
			bool prevAutoWidth = TreeList.OptionsView.AutoWidth;
			TreeList.OptionsView.AutoWidth = false;
			TreeList.BestFitColumns();
			result = TreeList.ViewInfo.ViewRects.ColumnTotalWidth + (TreeList.ViewInfo.ViewRects.Window.Width - TreeList.ViewInfo.ViewRects.Client.Width);
			TreeList.OptionsView.AutoWidth = prevAutoWidth;
			return result;
		}
		public override void ShowPopupForm() {
			BeforeShowPopupForm();
			base.ShowPopupForm();
			AfterShowPopupForm();
		}
		public override bool AllowMouseClick(Control control, Point mousePosition) {
			if(base.AllowMouseClick(control, mousePosition)) return true;
			if(control == OwnerEdit.Parent || (control != null && control.FindForm() != null && control.FindForm().Contains(OwnerEdit))) return false;
			return true;
		}
		protected override void OnBeforeShowPopup() {
			base.OnBeforeShowPopup();
			TreeList.ForceInitialize();
		}
		protected virtual void BeforeShowPopupForm() {
			TreeList.CreateHandleCore();
			if(TreeList.OptionsBehavior.AutoPopulateColumns && TreeList.Columns.Count == 0)
				TreeList.PopulateColumns();
			Properties.ExpandAllNodes();
			RestoreTreeListLayout();
			Properties.UpdateDisplayFilter();
			if(Properties.TextEditStyle == TextEditStyles.DisableTextEditor && TreeList.VisibleNodesCount == 0 && TreeList.ExtraFilter != string.Empty)
				ResetFilter();
			Properties.UpdateTreeListFocusedNode(OwnerEdit.EditValue);
			if(TreeList.OptionsFind.AllowFindPanel && TreeList.OptionsFind.AlwaysVisible)
				TreeList.ShowFindPanel();
		}
		void ResetFilter() {
			TreeList.SetExtraFilter("");
			TreeList.SetFocusedNodeCore(null);
		}
		protected virtual void AfterShowPopupForm() {
			FocusFormControl(EmbeddedControl);
		}
		protected virtual object GetResultValue() {
			if(TreeList.FocusedNode != null && TreeList.FocusedNode != TreeList.Nodes.AutoFilterNode) 
				return Properties.GetRowValue(TreeList.FocusedNode.Id, Properties.ValueMember);
			return OldEditValue;
		}
		protected override object QueryResultValue() {
			if(TreeList.FocusedNode == null) {
				if(Properties.TextEditStyle == TextEditStyles.Standard && OwnerEdit.AutoSearchText != string.Empty) {
					object val;
					if(Properties.OnProcessInputNewValue(OwnerEdit.AutoSearchText, out val)) 
						return val;
				}
			}
			return base.QueryResultValue();
		}
		public override void ProcessKeyDown(KeyEventArgs e) {
			if(IsFindPanelActive) {
				if(e.KeyData == Keys.Escape && !TreeList.OptionsFind.AlwaysVisible || e.KeyData == Keys.Enter)
					return;
			}
			if(!Focused) {
				if(e.KeyData == Keys.Enter) {
					e.Handled = true;
					ClosePopup();
				}
			}
			base.ProcessKeyDown(e);
		}
		public override void ProcessKeyPress(KeyPressEventArgs e) {
			base.ProcessKeyPress(e);
			if(e.Handled) return;
			if(CanProcessAutoSearchChar(e))
				OwnerEdit.ProcessAutoSearchCharCore(e);
		}
		public override void HidePopupForm() {
			SaveTreeListLayout();
			base.HidePopupForm();
		}
		protected virtual bool CanProcessAutoSearchChar(KeyPressEventArgs e) {
			if(IsFindPanelActive)
				return false;
			return TreeList.FocusedNode != TreeList.Nodes.AutoFilterNode;
		}
		protected override void Dispose(bool disposing) {
			Controls.Remove(TreeList);
			base.Dispose(disposing);
		}
		bool treeListLayoutSaved = false;
		protected void SaveTreeListLayout() {
			if(treeListLayoutSaved) return;
			this.treeListLayoutSaved = true;
			try {
				if(ShouldSaveTreeListLayout) {
					MemoryStream stream = new MemoryStream();
					TreeList.SaveLayoutToStream(stream);
					IDisposable prev = Properties.PropertyStoreCore[RepositoryItemTreeListLookUpEdit.TreeListLayoutKeyName] as IDisposable;
					if(prev != null) prev.Dispose();
					Properties.PropertyStoreCore[RepositoryItemTreeListLookUpEdit.TreeListLayoutKeyName] = stream;
				}
			}
			catch { }
		}
		protected void RestoreTreeListLayout() {
			if(ShouldSaveTreeListLayout) {
				MemoryStream stream = Properties.PropertyStoreCore[RepositoryItemTreeListLookUpEdit.TreeListLayoutKeyName] as MemoryStream;
				if(stream != null) {
					stream.Seek(0, SeekOrigin.Begin);
					TreeList.RestoreLayoutFromStream(stream);
				}
			}
			this.treeListLayoutSaved = false;
		}
	}
}
