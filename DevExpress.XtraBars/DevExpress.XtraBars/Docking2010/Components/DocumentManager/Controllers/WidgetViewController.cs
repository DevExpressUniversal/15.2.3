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

using System.Windows.Forms;
namespace DevExpress.XtraBars.Docking2010.Views.Widget {
	class WidgetViewController : BaseViewController, IWidgetViewController, IWidgetViewControllerInternal {
		public WidgetViewController(WidgetView view)
			: base(view) {
		}
		public new WidgetView View {
			get { return base.View as WidgetView; }
		}
		protected override bool DockCore(BaseDocument baseDocument) {
			Document document = baseDocument as Document;
			if(document == null) return false;
			if(AddDocumentCore(document)) {
				Manager.InvokePatchActiveChildren();
				return true;
			}
			return false;
		}
		protected override bool AddDocumentCore(BaseDocument document) {
			if(document == null) return false;
			return AddDocumentInStackGroups(View.StackGroups, document) != null; 
		}
		protected StackGroup AddDocumentInStackGroups(StackGroupCollection groups, BaseDocument document) {
			StackGroup result = null;
			if(groups.Count == 0)
				return null;
			foreach(StackGroup group in groups) {
				if(AddDocumentToGroup(document, group))
					return group;
			}
			return result;
		}
		protected bool AddDocumentToGroup(BaseDocument document, StackGroup group) {
			if(group != null) {
				Document widgetDocument = document as Document;
				if(widgetDocument != null) {
					if(group.Items.Contains(widgetDocument)) return true;
					if(!group.Items.Add(widgetDocument)) return false;
					widgetDocument.SetParent(group);
					if(group.Info != null)
						group.Info.Register(widgetDocument);
					return true;
				}
			}
			return false;
		}
		protected bool AddDocumentToGroup(BaseDocument document, TableGroup group) {
			if(group != null) {
				Document widgetDocument = document as Document;
				if(widgetDocument != null) {
					if(group.Items.Contains(widgetDocument)) return true;
					if(!group.Items.Add(widgetDocument)) return false;
					return true;
				}
			}
			return false;
		}
		public bool Dock(Document document, TableGroup group) {
			if(document == null || group == null) return false;
			using(View.LockPainting()) {
				using(IDockOperation operation = View.BeginDockOperation(document)) {
					if(operation.Canceled) return false;
					if(document.IsFloating)
						DockFromFloating(document);
					return DockCore(group, document);
				}
			}
		}
		public bool Dock(Document document, StackGroup group) {
			if(document == null || group == null) return false;
			using(View.LockPainting()) {
				using(IDockOperation operation = View.BeginDockOperation(document)) {
					if(operation.Canceled) return false;
					if(document.IsFloating)
						DockFromFloating(document);
					return DockCore(group, document);
				}
			}
		}
		public bool Dock(Document document, StackGroup group, int insertIndex) {
			if(document == null || group == null) return false;
			using(View.LockPainting()) {
				using(IDockOperation operation = View.BeginDockOperation(document)) {
					if(operation.Canceled) return false;
					if(document.IsFloating)
						DockFromFloating(document);
					return InsertCore(group, document, insertIndex);
				}
			}
		}
		public void Maximize(Document document) {
			if(document.IsMaximized) return;
			if(document.Control != null && document.Control.Parent != null)
				DevExpress.Skins.XtraForm.FormPainter.PostSysCommand(null, document.Control.Parent.Handle, DevExpress.Utils.Drawing.Helpers.NativeMethods.SC.SC_MAXIMIZE);
		}
		public void Restore(Document document) {
			if(!document.IsMaximized) return;
			if(document.Control != null && document.Control.Parent != null)
				DevExpress.Skins.XtraForm.FormPainter.PostSysCommand(null, document.Control.Parent.Handle, DevExpress.Utils.Drawing.Helpers.NativeMethods.SC.SC_RESTORE);
		}
		bool InsertCore(StackGroup group, Document document, int index) {
			StackGroup sourceGroup = document.Parent;
			if(sourceGroup != group)
				RemoveDocumentFromGroup(document, sourceGroup);
			if(InsertDocumentInGroup(document, group, index)) {
				Manager.InvokePatchActiveChildren();
				return true;
			}
			return false;
		}
		protected bool InsertDocumentInGroup(Document document, StackGroup group, int index) {
			if(group != null) {
				if(document.IsContainer) {
					BaseDocument[] children = document.GetChildren();
					for(int i = 0; i < children.Length; i++) {
						Document childDocument = children[i] as Document;
						if(!InsertDocumentInGroupCore(childDocument, group, index)) {
							StackGroupCollection groups = View.StackGroups;
							group = View.CreateStackGroup();
							View.StackGroups.Add(group);
							InsertDocumentInGroupCore(childDocument, group, index);
						}
					}
					return true;
				}
				else return InsertDocumentInGroupCore(document, group, index);
			}
			return false;
		}
		protected bool InsertDocumentInGroupCore(Document document, StackGroup group, int index) {
			if(!group.Items.Contains(document) && group.Items.Insert(index, document)) {
				group.Info.Register(document);
				return true;
			}
			group.Items.Insert(index, document);
			return group.Items.Move(index, document);
		}
		bool DockCore(StackGroup group, Document document) {
			StackGroup sourceGroup = document.Parent;
			if(sourceGroup != null && sourceGroup != group)
				RemoveDocumentFromGroup(document, sourceGroup);
			if(AddDocumentToGroup(document, group)) return true;
			return false;
		}
		bool DockCore(TableGroup group, Document document) {
			if(AddDocumentToGroup(document, group)) return true;
			return false;
		}
		protected StackGroup AddStackGroup(StackGroupCollection groups) {
			if(View.Site == null || !View.Site.DesignMode) return null;
			StackGroup newGroup = View.CreateStackGroup();
			groups.Add(newGroup);
			return newGroup;
		}
		protected override void GetCommandsCore(BaseDocument document, System.Collections.Generic.IList<BaseViewControllerCommand> commands) {
			base.GetCommandsCore(document, commands);
			if(document.IsFloating)
				commands.Add(WidgetViewControllerCommand.DockWidget);
		}
		protected override void GetCommandsCore(System.Collections.Generic.IList<BaseViewControllerCommand> commands) {
			base.GetCommandsCore(commands);
		}
		protected override bool RemoveDocumentCore(BaseDocument document) {
			Document doc = document as Document;
			return (doc != null) && RemoveDocumentFromGroup(doc, doc.Parent);
		}
		bool RemoveDocumentFromGroup(Document document, StackGroup group) {
			if(group == null) return false;
			using(View.LockPainting()) {
				if(!group.Items.Remove(document)) return false;
				UnregisterInfo(group, document);
			}
			return true;
		}
		void UnregisterInfo(StackGroup group, Document document) {
			if(group.IsDisposing) return;
			if(group.Info == null) return;
			group.Info.Unregister(document);
		}
		protected override void PatchControlBeforeAdd(Control control) {
			if(Manager != null) Manager.PatchControlBeforeAdd(control);
		}
		protected override void PatchControlAfterRemove(Control control) {
			if(Manager != null) Manager.PatchControlAfterRemove(control);
		}
		protected override Control CalculatePlacementTarget(BaseDocument document) {
			return Manager.GetChild(document);
		}
		void IWidgetViewControllerInternal.OnOrientationChanged() {
			foreach(Document document in View.Documents) {
				ChangeDocumentOrientation(document);
			}
		}
		void ChangeDocumentOrientation(Document document) {
			int width = document.Width;
			document.Width = document.Height;
			document.Height = width;
		}
	}
}
