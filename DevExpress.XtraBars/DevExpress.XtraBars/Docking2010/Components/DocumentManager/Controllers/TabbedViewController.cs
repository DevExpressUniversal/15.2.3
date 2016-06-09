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
using System.Collections.Generic;
using System.Windows.Forms;
using DevExpress.XtraBars.Docking2010.Base;
using DevExpress.XtraBars.Docking2010.Customization;
using DevExpress.XtraBars.CodedUISupport;
namespace DevExpress.XtraBars.Docking2010.Views.Tabbed {
	class TabbedViewController : BaseViewController, ITabbedViewController {
		public TabbedViewController(TabbedView view)
			: base(view) {
		}
		public new TabbedView View {
			get { return base.View as TabbedView; }
		}
		protected override BaseDocument[] GetChildren(BaseView hostView) {
			return ((TabbedView)hostView).GetDocuments();
		}
		public bool Select(Document document) {
			if(View.IsDisposing || !View.IsLoaded) return false;
			if(document == null || !document.CanActivate())
				return false;
			if(document.IsFloating || document.Parent == null) return false;
			using(View.LockPainting()) {
				document.Parent.SetSelected(document);
				View.RequestInvokePatchActiveChild();
			}
			return true;
		}
		public bool SelectNextTab(bool forward) {
			if(View.IsDisposing || !View.IsLoaded || View.Documents.Count < 2) return false;
			BaseDocument nextDocument = View.GetNextDocument(View.ActiveDocument, forward);
			return Activate(nextDocument);
		}
		public bool CreateNewDocumentGroup(Document document) {
			Orientation orientation = View.Orientation == Orientation.Horizontal ? Orientation.Vertical : Orientation.Horizontal;
			return CreateNewDocumentGroup(document, orientation);
		}
		public bool CreateNewDocumentGroup(Document document, Orientation orientation) {
			int index = (document.Parent != null) ? View.DocumentGroups.IndexOf(document.Parent) + 1 : -1;
			return CreateNewDocumentGroup(document, orientation, index);
		}
		public bool CreateNewDocumentGroup(Document document, Orientation orientation, int insertIndex) {
			Orientation = orientation == Orientation.Horizontal ? Orientation.Vertical : Orientation.Horizontal;
			if(document == null) return false;
			DockManagerCodedUIHelper.AddDockPanelDocumentDockInfo(document, orientation, insertIndex);
			using(BatchUpdate.Enter(Manager)) {
				using(View.LockPainting()) {
					if(!View.AllowFreeLayoutMode())
						View.Orientation = Orientation;
					using(IDockOperation operation = View.BeginDockOperation(document)) {
						if(operation.Canceled) return false;
						if(document.IsFloating) {
							DockFromFloating(document);
							AddDocumentInNewGroup(View.DocumentGroups, document, insertIndex);
							Manager.InvokePatchActiveChildren();
							return true;
						}
						if(document.IsDocumentsHost) {
							DocumentGroup group = AddDocumentGroup(View.DocumentGroups, insertIndex);
							bool result = DockFromDocumentsHost(document,
								(childDocument) => AddDocumentToGroup((Document)childDocument, group));
							Manager.InvokePatchActiveChildren();
							return result;
						}
						DocumentGroup sourceGroup = document.Parent;
						if(RemoveDocumentFromGroup(document, sourceGroup)) {
							AddDocumentInNewGroup(View.DocumentGroups, document, insertIndex);
							Manager.InvokePatchActiveChildren();
							return true;
						}
					}
					return false;
				}
			}
		}
		public bool MoveToMainDocumentGroup(Document document) {
			if(document == null || document.Parent == null || document.Manager == null) return false;
			if(!DocumentsHostContext.IsChild(document.Manager)) return false;
			IDocumentsHostWindow hostWindow = DocumentsHostContext.GetDocumentsHostWindow(document.Manager);
			var context = document.Manager.GetDocumentsHostContext();
			DocumentManager mainManager = context.startupManager;
			using(mainManager.View.LockPainting()) {
				BaseView hostView = document.Manager.View;
				document.Manager.RemoveDocumentFromHost(document);
				if(hostView.Container != null)
					hostView.Container.Remove(document);
				hostView.Documents.Remove(document);
				mainManager.BeginDocking();
				try {
					document.SetManager(mainManager);
					bool deleteQueued = false;
					bool isEmpty = hostView.Documents.Count == 0;
					if(isEmpty && hostWindow.DestroyOnRemovingChildren) {
						context.QueueDelete(document, hostWindow);
						deleteQueued = true;
					}
					mainManager.View.Documents.Add(document);
					((TabbedViewController)mainManager.View.Controller).AddDocumentCore(document);
					if(mainManager.View.Container != null)
						mainManager.View.Container.Add(document);
					mainManager.DockFloatForm(document, PatchControlBeforeAdd);
					if(deleteQueued)
						context.DequeueOnDocking(document);
				}
				finally { mainManager.EndDocking(); }
			}
			return false;
		}
		public bool MoveToDocumentGroup(Document document, bool next) {
			if(document == null || document.Parent == null) return false;
			using(View.LockPainting()) {
				using(IDockOperation operation = View.BeginDockOperation(document)) {
					if(operation.Canceled)
						return false;
					DocumentGroup sourceGroup = document.Parent;
					int index = View.DocumentGroups.IndexOf(sourceGroup) + (next ? 1 : -1);
					if(index >= 0 && index < View.DocumentGroups.Count) {
						DocumentGroup targetGroup = View.DocumentGroups[index];
						return DockCore(targetGroup, document);
					}
				}
				return false;
			}
		}
		public bool Move(Document document, int position) {
			if(document == null || document.Parent == null || position < 0)
				return false;
			bool result = false;
			Document[] documents = View.GetDocuments();
			if(position < documents.Length) {
				Document target = documents[position];
				if(target == document)
					return false;
				DocumentGroup targetGroup = target.Parent;
				int targetIndex = targetGroup.Items.IndexOf(target);
				using(BatchUpdate.Enter(View)) {
					bool isSelected = document.IsSelected;
					if(targetGroup == document.Parent) {
						result = targetGroup.Items.Move(targetIndex, document);
					}
					else {
						if(document.Parent.Items.Remove(document)) {
							result = targetGroup.Items.Insert(targetIndex, document);
						}
					}
					if(result && isSelected) {
						targetGroup.SetSelected(document);
					}
				}
			}
			return result;
		}		
		public bool Dock(Document document, DocumentGroup group) {
			if(document == null || group == null || document.Parent == group) return false;
			DockManagerCodedUIHelper.AddDockPanelDocumentDockInfo(document, group, -1);
			using(BatchUpdate.Enter(Manager)) {
				using(View.LockPainting()) {
					using(IDockOperation operation = View.BeginDockOperation(document)) {
						if(operation.Canceled) return false;
						if(document.IsFloating)
							DockFromFloating(document);
						if(document.IsDocumentsHost) {
							return DockFromDocumentsHost(document,
								(childDocument) => DockCore(group, (Document)childDocument));
						}
						else return DockCore(group, document);
					}
				}
			}
		}
		public bool Dock(Docking.DockPanel dockPanel, DocumentGroup group) {
			if(dockPanel == null || !dockPanel.Options.AllowDockAsTabbedDocument) return false;
			if(IsAlreadyDockedAsTabbedDocument(dockPanel))
				return true;
			Docking.FloatForm floatForm = dockPanel.EnsureFloatForm(true);
			if(floatForm == null) return false;			
			using(BatchUpdate.Enter(Manager)) {
				using(View.LockPainting()) {
					bool registered = false;
					BaseDocument document;
					if(!View.Documents.TryGetValue(floatForm, out document)) {
						document = RegisterDockPanel(floatForm);
						registered = true;
					}
					using(IDockOperation operation = View.BeginDockOperation(document)) {
						if(operation.Canceled) {
							if(registered)
								UnregisterDockPanel(floatForm);
							return false;
						}
						return Dock((Document)document, group);
					}
				}
			}
		}
		public bool Dock(Document document, DocumentGroup group, int insertIndex) {
			if(document == null || group == null) return false;
			DockManagerCodedUIHelper.AddDockPanelDocumentDockInfo(document, group, insertIndex);
			using(BatchUpdate.Enter(Manager)) {
				using(View.LockPainting()) {
					using(IDockOperation operation = View.BeginDockOperation(document)) {
						if(operation.Canceled) return false;
						if(document.IsFloating)
							DockFromFloating(document);
						if(document.IsDocumentsHost) {
							return DockFromDocumentsHost(document,
								(childDocument) => InsertCore(group, (Document)childDocument, insertIndex));
						}
						return InsertCore(group, document, insertIndex);
					}
				}
			}
		}
		public bool Dock(Docking.DockPanel dockPanel, DocumentGroup group, int insertIndex) {
			if(dockPanel == null || !dockPanel.Options.AllowDockAsTabbedDocument) return false;
			if(IsAlreadyDockedAsTabbedDocument(dockPanel))
				return true;
			Docking.FloatForm floatForm = dockPanel.EnsureFloatForm(true);
			if(floatForm == null) return false;			
			using(BatchUpdate.Enter(Manager)) {
				using(View.LockPainting()) {
					bool registered = false;
					BaseDocument document;
					if(!View.Documents.TryGetValue(floatForm, out document)) {
						document = RegisterDockPanel(floatForm);
						registered = true;
					}
					using(IDockOperation operation = View.BeginDockOperation(document)) {
						if(operation.Canceled) {
							if(registered)
								UnregisterDockPanel(floatForm);
							return false;
						}
						return Dock((Document)document, group, insertIndex);
					}
				}
			}
		}
		public bool CloseAllButPinned() {
			if(View.IsDisposing || !View.IsLoaded) return false;
			BaseDocument[] documents = View.Documents.ToArray();
			BaseDocument[] floatDocuments = View.FloatDocuments.ToArray();
			using(View.LockPainting()) {
				bool closed = false;
				Document tabbedDocument = null;
				for(int i = 0; i < documents.Length; i++) {
					tabbedDocument = documents[i] as Document;
					if(!CanCloseDocument(tabbedDocument)) continue;
					closed |= Close(documents[i]);
				}
				tabbedDocument = null;
				for(int i = 0; i < floatDocuments.Length; i++) {
					tabbedDocument = floatDocuments[i] as Document;
					if(!CanCloseDocument(tabbedDocument)) continue;
					Close(floatDocuments[i]);
				}
				if(closed)
					Manager.InvokePatchActiveChildren();
				return closed;
			}
		}
		protected bool CanCloseDocument(Document document) {
			if(document == null || document.IsDisposing) return false;
			if(!document.Properties.CanPin || document.Pinned) return false;
			return true;
		}
		bool InsertCore(DocumentGroup group, Document document, int index) {
			DocumentGroup sourceGroup = document.Parent;
			if(sourceGroup != group)
				RemoveDocumentFromGroup(document, sourceGroup);
			if(InsertDocumentInGroup(document, group, index)) {
				if(!document.IsContainer)
					group.SetSelected(document);
				if(sourceGroup != group)
					Manager.InvokePatchActiveChildren();
				return true;
			}
			return false;
		}
		protected override bool DockCore(BaseDocument baseDocument) {
			Document document = baseDocument as Document;
			if(document == null) return false;
			DocumentGroup sourceGroup = document.Parent;
			if(sourceGroup != null) {
				if(IsDockedAsDockPanel(document)) 
					return true;
				RemoveDocumentFromGroup(document, sourceGroup);
			}
			if(AddDocumentCore(document)) {
				if(!document.IsContainer) {
					bool shouldSelect = true;
					if(!Manager.IsMdiStrategyInUse && document.IsDockPanel) {
						Docking.DockPanel panel = document.GetDockPanel();
						if(panel != null && !panel.ActivateWhenDockingAsMdiDocument)
							shouldSelect = false;
					}
					if(shouldSelect)
						document.Parent.SetSelected(document);
				}
				Manager.InvokePatchActiveChildren();
				return true;
			}
			return false;
		}
		bool DockCore(DocumentGroup group, Document document) {
			DocumentGroup sourceGroup = document.Parent;
			if(sourceGroup != group && sourceGroup != null)
				RemoveDocumentFromGroup(document, sourceGroup);
			if(AddDocumentToGroup(document, group)) {
				if(!document.IsContainer)
					group.SetSelected(document);
				if(sourceGroup != group)
					Manager.InvokePatchActiveChildren();
				return true;
			}
			return false;
		}
		protected override void PatchControlBeforeAdd(Control control) {
			if(Manager != null) Manager.PatchControlBeforeAdd(control);
		}
		protected override void PatchControlAfterRemove(Control control) {
			if(Manager != null) Manager.PatchControlAfterRemove(control);
		}
		protected override bool AddDocumentCore(BaseDocument document) {
			Document doc = document as Document;
			return (doc != null) && AddDocumentInGroups(View.DocumentGroups, doc) != null;
		}
		protected override bool RemoveDocumentCore(BaseDocument document) {
			Document doc = document as Document;
			return (doc != null) && RemoveDocumentFromGroup(doc, doc.Parent);
		}
		protected DocumentGroup AddDocumentInGroups(DocumentGroupCollection groups, Document document) {
			if(groups.Count == 0)
				AddDocumentGroup(groups);
			Document activeDocument = View.ActiveDocument as Document;
			var orderedGroups = (activeDocument == null || activeDocument.Parent == null) ?
				groups : GetGroupsSortedByActiveDocument(groups, activeDocument);
			foreach(DocumentGroup group in orderedGroups) {
				if(AddDocumentToGroup(document, group))
					return group;
			}
			return AddDocumentInNewGroup(groups, document);
		}
		IEnumerable<DocumentGroup> GetGroupsSortedByActiveDocument(DocumentGroupCollection groups, Document activeDocument) {
			yield return activeDocument.Parent;
			foreach(DocumentGroup item in groups) {
				if(item != activeDocument.Parent)
					yield return item;
			}
		}
		protected DocumentGroup GetActiveDocumentGroup(DocumentGroupCollection groups) {
			foreach(DocumentGroup activeGroup in groups) {
				if(activeGroup.Items.Contains(View.ActiveDocument as Document)) {
					return activeGroup;
				}
			}
			return null;
		}
		protected DocumentGroup AddDocumentInNewGroup(DocumentGroupCollection groups, Document document) {
			return AddDocumentInNewGroup(groups, document, -1);
		}
		public DocumentGroup  TargetGroup { get; set; }
		public Orientation Orientation { get; set; }
		public bool  After { get; set; }
		protected DocumentGroup AddDocumentInNewGroup(DocumentGroupCollection groups, Document document, int insertIndex) {
			DocumentGroup result = AddDocumentGroup(groups, insertIndex);
			if(TargetGroup != null) {
				result.DockTo(TargetGroup, Orientation, After);
			}
			if(AddDocumentToGroup(document, result))
				if(!document.IsContainer)
					result.SetSelected(document);
			return result;
		}
		protected bool AddDocumentToGroup(Document document, DocumentGroup group) {
			if(group != null) {
				if(document.IsContainer) {
					BaseDocument[] children = document.GetChildren();
					for(int i = 0; i < children.Length; i++) {
						Document childDocument = children[i] as Document;
						if(childDocument.Parent != group && !AddDocumentToGroupCore(childDocument, group)) {
							DocumentGroupCollection groups = View.DocumentGroups;
							group = AddDocumentGroup(groups, groups.IndexOf(group) + 1);
							AddDocumentToGroupCore(childDocument, group);
						}
						group.SetSelected(childDocument);
					}
					return true;
				}
				else return AddDocumentToGroupCore(document, group);
			}
			return false;
		}
		protected bool AddDocumentToGroupCore(Document document, DocumentGroup group) {
			if(!group.Items.Add(document)) return false;
			View.RegisterDocumentInfo(document, group);
			return true;
		}
		protected bool InsertDocumentInGroup(Document document, DocumentGroup group, int index) {
			if(group != null) {
				if(document.IsContainer) {
					BaseDocument[] children = document.GetChildren();
					for(int i = 0; i < children.Length; i++) {
						Document childDocument = children[children.Length - 1 - i] as Document;
						if(!InsertDocumentInGroupCore(childDocument, group, index)) {
							DocumentGroupCollection groups = View.DocumentGroups;
							group = AddDocumentGroup(groups, groups.IndexOf(group) + 1);
							InsertDocumentInGroupCore(childDocument, group, index);
						}
						group.SetSelected(childDocument);
					}
					return true;
				}
				else return InsertDocumentInGroupCore(document, group, index);
			}
			return false;
		}
		protected bool InsertDocumentInGroupCore(Document document, DocumentGroup group, int index) {
			if(!group.Items.Contains(document) && group.Items.Insert(index, document)) {
				View.RegisterDocumentInfo(document, group);
				return true;
			}
			return group.Items.Move(index, document);
		}
		protected bool RemoveDocumentFromGroup(Document document, DocumentGroup group) {
			if(group != null) {
				if(group.Items.Remove(document)) {
					View.UnregisterDocumentInfo(document, group);
					if(View.IsDesignMode() && !View.groupsToRestore.ContainsKey(document))						
						View.groupsToRestore.Add(document, group);
					return true;
				}
			}
			return false;
		}
		protected DocumentGroup AddDocumentGroup(DocumentGroupCollection groups) {
			return AddDocumentGroup(groups, -1);
		}
		protected DocumentGroup AddDocumentGroup(DocumentGroupCollection groups, int insertIndex) {
			DocumentGroup group = View.CreateDocumentGroup();
			if(insertIndex < 0 || insertIndex > groups.Count)
				groups.Add(group);
			else groups.Insert(insertIndex, group);
			return group;
		}
		protected override void ResetLayoutCore() {
			if(View.DocumentGroups != null && View.DocumentGroups.Count > 0) {
				foreach(Document document in View.Documents) {
					DockCore(View.DocumentGroups[0], document);
				}
				DockAllCore();
			}
		}
		public bool FloatAll(Document document) {
			return (document != null) && FloatAll(document.Parent);
		}
		public bool FloatAll(DocumentGroup group) {
			if(group == null || View.IsDisposing || !View.IsLoaded) return false;
			using(View.LockPainting()) {
				return View.AddFloatingDocumentsHost(group.Items.ToArray());
			}
		}
		public void DockAll() {
			if(View.IsDisposing || !View.IsLoaded) return;
			using(BatchUpdate.Enter(Manager)) {
				using(View.LockPainting()) {
					DockAllCore();
				}
			}
		}
		protected virtual void DockAllCore() {
			if(View.DocumentGroups != null && View.DocumentGroups.Count > 0) {
				for(int i = View.FloatDocuments.Count - 1; i >= 0; i--) {
					Dock(View.FloatDocuments[i] as Document, View.DocumentGroups[0]);
				}
			}
		}
		protected override void GetCommandsCore(BaseDocument document, IList<BaseViewControllerCommand> commands) {
			base.GetCommandsCore(document, commands);
			Document doc = document as Document;
			if(doc != null) {
				if(document.IsFloating) {
					if(document.IsFloatDocument)
						commands.Add(TabbedViewControllerCommand.Dock);
					return;
				}
				if(View.GetPinnedDocumentCount() > 0)
					commands.Add(TabbedViewControllerCommand.CloseAllButPinned);
				if(doc.Properties.CanPin) {
					commands.Add(doc.Pinned ? TabbedViewControllerCommand.UnpinTab : TabbedViewControllerCommand.PinTab);
				}
				DocumentGroup group = doc.Parent;
				int groupsCount = View.DocumentGroups.Count;
				if(groupsCount == 1) {
					if(group.Items.Count > 1) {
						commands.Add(TabbedViewControllerCommand.NewHorizontalDocumentGroup);
						commands.Add(TabbedViewControllerCommand.NewVerticalDocumentGroup);
					}
				}
				if(groupsCount > 1) {
					int index = View.DocumentGroups.IndexOf(group);
					if(index != -1) {
						if(group.Items.Count > 1) {
							commands.Add(View.IsHorizontal ?
								TabbedViewControllerCommand.NewVerticalDocumentGroup :
								TabbedViewControllerCommand.NewHorizontalDocumentGroup);
						}
						if(index > 0) {
							if(!View.DocumentGroups[index - 1].IsFilledUp)
								commands.Add(TabbedViewControllerCommand.MoveToPrevDocumentGroup);
						}
						if(index < groupsCount - 1) {
							if(!View.DocumentGroups[index + 1].IsFilledUp)
								commands.Add(TabbedViewControllerCommand.MoveToNextDocumentGroup);
						}
					}
				}
				if(DocumentsHostContext.IsChild(Manager))
					commands.Add(TabbedViewControllerCommand.MoveToMainDocumentGroup);
			}
		}
		protected override void GetCommandsCore(IList<BaseViewControllerCommand> commands) {
			base.GetCommandsCore(commands);
			if(View.DocumentGroups.Count > 1) {
				commands.Add(View.IsHorizontal ?
					Views.Tabbed.TabbedViewControllerCommand.Vertical :
					Views.Tabbed.TabbedViewControllerCommand.Horizontal);
			}
			if(View.FloatDocuments.Count > 0)
				commands.Add(TabbedViewControllerCommand.DockAll);
		}
		protected override bool CanFloatAll() {
			var group = GetMenuTargetGroup();
			if(group != null)
				return CanFloatAll(group);
			return CanFloatAll(View.ActiveDocument);
		}
		protected override BaseViewControllerCommand GetFloatAllCommand() {
			var group = GetMenuTargetGroup();
			if(group != null)
				return GetFloatAllCommand(group.SelectedDocument);
			return GetFloatAllCommand(View.ActiveDocument);
		}
		protected override bool CanFloatAll(BaseDocument document) {
			Document tabDocument = document as Document;
			if(tabDocument == null) return false;
			return CanFloatAll(tabDocument.Parent);
		}
		protected override BaseViewControllerCommand GetFloatAllCommand(BaseDocument document) {
			return TabbedViewControllerCommand.FloatAllDocumentGroup;
		}
		protected override void InitViewCommandParameter(BaseViewControllerCommand command, BaseView view) {
			if(command == TabbedViewControllerCommand.FloatAllDocumentGroup) {
				var group = GetMenuTargetGroup();
				if(group != null) {
					command.Parameter = group.SelectedDocument;
					return;
				}
			}
			base.InitViewCommandParameter(command, view);
		}
		protected DocumentGroup GetMenuTargetGroup() {
			IDocumentGroupInfo groupInfo = MenuTargetInfo as IDocumentGroupInfo;
			if(groupInfo != null)
				return groupInfo.Group;
			return null;
		}
		protected bool CanFloatAll(DocumentGroup documentGroup) {
			if(documentGroup == null) return false;
			return View.CanFloatAll(documentGroup.Items.ToArray());
		}
		protected override void CheckFloatAllCommandForActiveDocument(List<BaseViewControllerCommand> commands, IEnumerable<BaseViewControllerCommand> documentCommands) {
			RemoveDocumentCommand(commands, documentCommands, TabbedViewControllerCommand.FloatAllDocumentGroup, TabbedViewControllerCommand.FloatAllDocumentGroup);
		}
		protected override BaseViewControllerMenu CreateContextMenu() {
			return new TabbedViewControllerMenu(this);
		}
		public bool ShowDocumentSelectorMenu(DocumentGroup group) {
			if(View.IsDisposing || !View.IsLoaded) return false;
			IDocumentGroupInfo groupInfo = group.Info;
			if(groupInfo != null && groupInfo.Owner == View) {
				groupInfo.ShowDocumentSelectorMenu(this);
				return true;
			}
			return false;
		}
		public bool ShowContextMenu(DocumentGroup group, Point point) {
			if(View.IsDisposing || !View.IsLoaded) return false;
			if(group != null) {
				TabbedViewControllerMenu menu = CreateContextMenu() as TabbedViewControllerMenu;
				menu.Init(group);
				menu.PlacementTarget = Manager.GetOwnerControl();
				return ShowContextMenuCore(menu, point);
			}
			return false;
		}
	}
}
