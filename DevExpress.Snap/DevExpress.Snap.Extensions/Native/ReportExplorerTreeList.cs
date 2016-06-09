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
using System.Collections;
using DevExpress.Snap.Core.Commands;
using DevExpress.Snap.Core.Fields;
using DevExpress.Snap.Core.Native;
using DevExpress.Snap.Native;
using DevExpress.XtraPrinting.Native;
using DevExpress.XtraRichEdit.Commands;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraTreeList;
using DevExpress.XtraTreeList.Native;
namespace DevExpress.Snap.Extensions.Native {
	public class ReportExplorerTreeList : ReportExplorerTreeListBase, IDesignControl {
		SnapControl snapControl;
		public ReportExplorerTreeList() {
		}
		protected internal override void SubscribeControlEvents() {
			this.BeforeFocusNode += new BeforeFocusNodeEventHandler(ReportExplorerTreeList_BeforeFocusNode);
			this.AfterFocusNode += new NodeEventHandler(ReportExplorerTreeList_AfterFocusNode);			
		}
		void ReportExplorerTreeList_BeforeFocusNode(object sender, BeforeFocusNodeEventArgs e) {
			if (e.Node != null && e.Node.Tag is Pair<string, int>) {
				string fieldName = ((Pair<string, int>)e.Node.Tag).First;
				Field field = snapControl.DocumentModel.MainPieceTable.FindFieldNearestToSelection(fieldName, true);
				if (field != null) {
					e.CanFocus = IsFieldResultVisible(field);					
				}
			}
		}
		void ReportExplorerTreeList_AfterFocusNode(object sender, NodeEventArgs e) {
			Pair<string, int> tag = GetFocusedNodeTag();
			if (tag == null)
				return;
			string fieldName = tag.First;
			int groupIndex = tag.Second;
			if (groupIndex >= 0)
				ProcessSelectedGroup(fieldName, groupIndex);
			else
				ProcessSelectedList(fieldName);
		}
		protected void SetFocusedNodeWithoutNotification(XtraListNode node) {
			this.AfterFocusNode -= new NodeEventHandler(ReportExplorerTreeList_AfterFocusNode);
			SetFocusedNode(node);
			this.AfterFocusNode += new NodeEventHandler(ReportExplorerTreeList_AfterFocusNode);
		}
		bool IsFieldResultVisible(Field field) {
			while (field != null) {
				if (field.IsCodeView)
					return false;
				field = field.Parent;				
			}
			return true;
		}
		protected virtual void ProcessSelectedGroup(string fieldName, int groupIndex) {
			Field field = snapControl.DocumentModel.MainPieceTable.FindFieldNearestToSelection(fieldName, false);
			if (field == null)
				return;
			SnapFieldCalculatorService calc = new SnapFieldCalculatorService();
			SNListField lst = calc.ParseField(snapControl.DocumentModel.MainPieceTable, field) as SNListField;
			if (lst != null) {
				SnapBookmark foundBookmark = lst.GetGroupBookmarkByGroupIndex(this.snapControl.DocumentModel, field, groupIndex, GroupBookmarkKind.GroupHeader);
				if(foundBookmark == null)
					foundBookmark = lst.GetGroupBookmarkByGroupIndex(this.snapControl.DocumentModel, field, groupIndex, GroupBookmarkKind.GroupFooter);
				if (foundBookmark != null)
					SetSelection(foundBookmark.Interval.Start.LogPosition);
			}
		}
		protected virtual void ProcessSelectedList(string fieldName) {
			Field field = snapControl.DocumentModel.MainPieceTable.FindFieldNearestToSelection(fieldName, true);
			if (field != null) {
				SnapFieldCalculatorService calculator = new SnapFieldCalculatorService();
				SNListField list = calculator.ParseField(snapControl.DocumentModel.MainPieceTable, field) as SNListField;
				if (list == null)
					return;
				SnapBookmark start = list.GetFirstContentBookmark(snapControl.DocumentModel.MainPieceTable, field);
				if (start != null)
					SetSelection(start.Interval.Start.LogPosition);
				else if(field.Result.Start == field.Result.End)
					SetSelection(snapControl.DocumentModel.MainPieceTable.GetRunLogPosition(field.Result.Start));
			}
		}
		void SetSelection(DocumentLogPosition position) {
			Selection selection = snapControl.DocumentModel.Selection;
			snapControl.DocumentModel.Selection.BeginUpdate();
			try {
				selection.ClearMultiSelection();
				selection.Start = position;
				selection.End = position;
				selection.SetStartCell(position);
				selection.UpdateTableSelectionEnd(position);
			}
			finally {
				snapControl.DocumentModel.Selection.EndUpdate();
			}
			EnsureCaretVisibleVerticallyCommand ensureCaretVisibleVerticallyCommand = new EnsureCaretVisibleVerticallyCommand(snapControl);
			ensureCaretVisibleVerticallyCommand.Execute();
		}
		public SnapControl SnapControl {
			get {
				return snapControl;
			}
			set {
				if (!DesignMode) {
					if (snapControl != null) {
						snapControl.DocumentModel.FieldsChanged -= new EventHandler(DocumentModel_FieldsChanged);
						snapControl.DocumentModel.CurrentSnapBookmarkChanged -= new EventHandler(DocumentModel_CurrentSnapBookmarkChanged);
					}
					snapControl = value;
					Controller.Control = snapControl;
					if (snapControl != null) {
						snapControl.DocumentModel.FieldsChanged += new EventHandler(DocumentModel_FieldsChanged);
						snapControl.DocumentModel.CurrentSnapBookmarkChanged += new EventHandler(DocumentModel_CurrentSnapBookmarkChanged);
					}
				}
			}
		}
		void DocumentModel_CurrentSnapBookmarkChanged(object sender, EventArgs e) {
			SnapControl.BeginInvoke((Action)DocumentModel_CurrentSnapBookmarkChangedCore);
		}
		void DocumentModel_CurrentSnapBookmarkChangedCore() {
			if(SnapControl.DocumentModel.SelectionInfo == null)
				SetFocusedNodeWithoutNotification(null);
			Selection selection = SnapControl.DocumentModel.Selection;
			SnapDocumentModel documentModel = (SnapDocumentModel)SnapControl.DocumentModel;
			SnapListFieldInfo snapFieldsInfo = FieldsHelper.GetSelectedSNListField(documentModel);
			if (snapFieldsInfo == null) {
				SetFocusedNodeWithoutNotification(null);
				return;
			}
			SnapBookmarkController controller = new SnapBookmarkController(documentModel.ActivePieceTable);
			SnapBookmark bookmark = controller.FindInnermostTemplateBookmarkByPosition(selection.Start);
			if (bookmark == null)
				return;
			Pair<string, int> tag = new SnapObjectModelController(documentModel.ActivePieceTable).GetTag(bookmark.TemplateInterval.TemplateInfo, snapFieldsInfo);
			XtraListNode node = FindNodeByTag(Nodes, tag);
			SetFocusedNodeWithoutNotification(node);
		}
		void DocumentModel_FieldsChanged(object sender, EventArgs e) {
			FillNodes();
		}
		void FillNodes() {
			BeginInit();
			((IList)Nodes).Clear();
			Controller.FillNodes(null, Nodes);
			ExpandAll();
			EndInit();
		}
		protected override void OnGotFocus(EventArgs e) {
			base.OnGotFocus(e);
			SetHighlightActiveElementCommand command = new SetHighlightActiveElementCommand(snapControl);
			command.Visible = true;
			command.Execute();
		}
		protected override void OnLostFocus(EventArgs e) {
			SetHighlightActiveElementCommand command = new SetHighlightActiveElementCommand(snapControl);
			command.Visible = false;
			command.Execute();
			base.OnLostFocus(e);
		}
	}
}
