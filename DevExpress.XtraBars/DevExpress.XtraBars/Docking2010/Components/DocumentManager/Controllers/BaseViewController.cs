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
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.XtraBars.Docking2010.Base;
using DevExpress.XtraBars.Docking2010.Customization;
namespace DevExpress.XtraBars.Docking2010.Views {
	abstract class BaseViewController : BaseObject, IBaseViewController, IBaseViewControllerInternal {
		BaseView viewCore;
		public BaseViewController(BaseView view) {
			viewCore = view;
		}
		protected override void OnDispose() {
			viewCore = null;
			base.OnDispose();
		}
		public BaseView View {
			get { return viewCore; }
		}
		public DocumentManager Manager {
			get { return View.Manager; }
		}
		public BaseDocument RegisterDockPanel(Control control) {
			if(View.IsDisposing || control == null) return null;
			BaseDocument document;
			if(!View.FloatPanels.TryGetValue(control, out document)) {
				document = CreateAndInitializeDocument(control);
				if(document != null)
					View.FloatPanels.Add(document);
			}
			return document;
		}
		public void UnregisterDockPanel(Control control) {
			if(View.IsDisposing || control == null) return;
			BaseDocument document;
			if(View.FloatPanels.TryGetValue(control, out document)) {
				View.FloatPanels.Remove(document);
			}
		}
		public BaseDocument AddDocument(Control control) {
			if(View.IsDisposing || control == null) return null;
			BaseDocument document;
			if(!View.Documents.TryGetValue(control, out document)) {
				using(BatchUpdate.Enter(Manager)) {
					document = CreateAndInitializeDocument(control);
					using(document.TrackContainerChange(View.Container)) {
						View.Documents.Add(document);
						if(document.IsFloatDocument) {
							FloatDocumentForm form = document.Form as FloatDocumentForm;
							form.SetDocument(document);
						}
						if(!document.IsDockPanel) {
							if(Manager != null && !Manager.InMdiClientControlAdded)
								Manager.AddDocumentToHost(document);
						}
					}
				}
			}
			if(document != null)
				PatchControlBeforeAdd(control);
			return document;
		}
		public BaseDocument AddFloatDocument(Control control) {
			if(View.IsDisposing || control == null) return null;
			BaseDocument document;
			if(!View.FloatDocuments.TryGetValue(control, out document)) {
				using(BatchUpdate.Enter(Manager)) {
					document = CreateAndInitializeDocument(control);
					using(document.TrackContainerChange(View.Container)) {
						Manager.PatchDocumentBeforeFloat(document);
						View.FloatDocuments.Add(document);
						Form form = document.Form;
						if(form.StartPosition != FormStartPosition.Manual) {
							form.StartPosition = FormStartPosition.Manual;
							var settings = BaseDocumentSettings.GetProvider<IBaseDocumentSettings>(control);
							Point floatLocation = (settings != null && settings.FloatLocation.HasValue) ?
								settings.FloatLocation.Value :
								Manager.ClientToScreen(new Point(
									(Manager.Bounds.Width - form.Width) / 2,
									(Manager.Bounds.Width - form.Width) / 2));
							if((settings != null && settings.FloatSize.HasValue)) 
								form.Size = settings.FloatSize.Value;
							form.Location = CheckScreenLocation(floatLocation, form.Size);
						}
						form.Owner = Manager.GetContainer().FindForm();
						form.Visible = true;
						Manager.LayeredWindowsRegisterNotificationSourceForFloatView(form);
					}
				}
			}
			return document;
		}
		public void RemoveDocument(Control control) {
			if(View.IsDisposing || control == null) return;
			BaseDocument document;
			if(View.Documents.TryGetValue(control, out document)) {
				if(!(document.IsDeferredControlLoad && View.IsReleasingDeferredControlLoadDocument)) {
					using(BatchUpdate.Enter(Manager)) {
						if(View.Documents.Remove(document)) {
							if(!document.IsDockPanel) {
								if(Manager != null && !Manager.InMdiClientControlAdded) {
									if(!Manager.IsDepopulating)
										Manager.RemoveDocumentFromHost(document);
								}
							}
							if(!IsDisposing && Manager != null) {
								if(Manager.IsContainerControlStrategyInUse && !Manager.InDocumentsHostControlRemoved)
									Manager.InvokePatchActiveChildren();
							}
						}
					}
				}
			}
			if(document != null)
				PatchControlAfterRemove(control);
		}
		protected abstract void PatchControlBeforeAdd(Control control);
		protected abstract void PatchControlAfterRemove(Control control);
		public bool AddDocument(BaseDocument document) {
			return AddDocumentCore(document);
		}
		public bool RemoveDocument(BaseDocument document) {
			return RemoveDocumentCore(document);
		}
		protected abstract bool AddDocumentCore(BaseDocument document);
		protected abstract bool RemoveDocumentCore(BaseDocument document);
		public BaseDocument CreateAndInitializeDocument(Control control) {
			BaseDocument document = ResolveDeferredControlLoadDocument(control) ??
				View.CreateDocument(control);
			if(document != null) {
				((ISupportInitialize)document).BeginInit();
				if(Manager != null)
					Manager.EnsureDocument(document);
				document.SetManager(Manager);
				if(document.IsDeferredControlLoad)
					View.OnDeferredLoadDocumentControlLoaded(control, document);
				if(!document.IsDeferredControlLoad || string.IsNullOrEmpty(document.Caption))
					document.Caption = document.GetCaptionFromControl(control);
				document.Control = control;
				if(document.IsDeferredControlLoad)
					View.OnDeferredLoadDocumentControlShown(control, document);
				((ISupportInitialize)document).EndInit();
			}
			return document;
		}
		protected BaseDocument ResolveDeferredControlLoadDocument(Control control) {
			BaseDocument result = null;
			DevExpress.XtraBars.Docking.IDockPanelInfo panel = control as DevExpress.XtraBars.Docking.IDockPanelInfo;
			if(!string.IsNullOrEmpty(panel != null ? panel.PanelName : control.Name)) {
				result = View.Documents.FindFirst(
						(document) => document.CanLoadControlByName(panel != null ? panel.PanelName : control.Name)
					);
			}
			if(result == null) {
				result = View.Documents.FindFirst(
						(document) => document.CanLoadControlByType(control.GetType())
					);
			}
			if(result == null) {
				result = View.Documents.FindFirst(
						(document) => document.CanLoadControl(control)
					);
			}
			return result;
		}
		public bool Activate(BaseDocument document) {
			if(View.IsDisposing || !View.IsLoaded) return false;
			if(document == null || !document.CanActivate())
				return false;
			var hostsContext = Manager.GetDocumentsHostContext();
			DocumentManager manager = Manager;
			if(hostsContext != null) {
				if(document.Manager != manager) {
					if(document.Manager != null && document.Manager.View != null) {
						manager = document.Manager;
						if(manager.View.IsDisposing || !manager.View.IsLoaded) return false;
					}
				}
				DocumentsHostContext.CheckHostFormActive(manager, hostForm =>
				{
					var child = manager.GetChild(document);
					if(DocumentsHostContext.IsChild(child, hostForm)) {
						hostForm.ActiveControl = child;
						hostForm.Select();
					}
				});
			}
			using(manager.ActivationInfo.GetRibbonMergingContext()) {
				return ActivateDocument(manager, manager.View, document);
			}
		}
		bool ActivateDocument(DocumentManager manager, BaseView view, BaseDocument document) {
			if(document.IsDeferredControlLoad) {
				if(!document.EnsureIsBoundToControl(view))
					return false;
			}
			Control activatedChild = manager.GetChild(document);
			if(!activatedChild.IsHandleCreated)
				return false;
			Control activeForm = Form.ActiveForm;
			if(document.IsFloating) {
				if(document.Form != null)
					document.Form.Select();
				else document.Control.Select();
				return true;
			}
			else {
				if(!view.IgnoreActiveFormOnActivation)
					activeForm = manager.GetActiveChild();
				else activeForm = null;
				if(!view.IsPaintingLocked) {
					if(!object.ReferenceEquals(activatedChild, activeForm)) {
						ActivateCore(document);
						if(!activatedChild.IsDisposed) {
							manager.PatchBeforeActivateChild(activatedChild);
							manager.Activate(activatedChild);
						}
						return true;
					}
					else {
						if(!view.IsFocused)
							manager.PostFocus(activatedChild);
					}
				}
			}
			return object.ReferenceEquals(activatedChild, activeForm);
		}
		protected virtual void ActivateCore(BaseDocument document) { }
		public bool Close(BaseDocument document) {
			if(View.IsDisposing || !View.IsLoaded) return false;
			if(document == null || document.IsDisposing || !document.Properties.CanClose)
				return false;
			if(!View.RaiseDocumentClosing(document)) {
				using(View.LockPainting()) {
					using(IDocumentClosedContext closeContext = View.BeginRaiseDocumentClosed(document)) {
						if(document.IsDockPanel) {
							Docking.DockPanel panel = document.GetDockPanel();
							if(panel != null) {
								bool isClosed = false;
								DocumentContainer container = panel.Parent as DocumentContainer;
								panel.Close();
								isClosed = panel.Visibility == Docking.DockVisibility.Hidden || panel.IsDisposed;
								if(isClosed) {
									Manager.CancelDragOperation();
									if(container != null)
										container.Dispose();
									closeContext.Flush();
									if(!IsDisposing)
										Manager.InvokePatchActiveChildren();
									return (container != null) ? container.IsDisposed : isClosed;
								}
								return isClosed;
							}
						}
						else {
							bool isClosed = false;
							if(document.IsControlLoaded) {
								Control child = Manager.GetChild(document);
								if(child is Form) {
									if(!DevExpress.Utils.Mdi.ControlState.IsCreatingHandle(child))
										((Form)child).Close();
								}
								else {
									document.EnsureFloatingBounds();
									child.Dispose();
								}
								isClosed = child.IsDisposed;
							}
							else isClosed = true;
							if(isClosed) {
								Manager.CancelDragOperation();
								document.Dispose();
								closeContext.Flush();
								if(!IsDisposing)
									Manager.InvokePatchActiveChildren();
								return true;
							}
						}
					}
				}
			}
			return false;
		}
		public bool CloseAll() {
			if(View.IsDisposing || !View.IsLoaded) return false;
			return CloseAllExceptOne(null, (docsToClose) => docsToClose.Length == 0);
		}
		public bool CloseAllButThis(BaseDocument document) {
			if(View.IsDisposing || !View.IsLoaded) return false;
			if(document == null) return false;
			return CloseAllExceptOne(document, (docsToClose) => docsToClose.Length == 0);
		}
		public bool CloseAllDocumentsAndHosts() {
			if(View.IsDisposing || !View.IsLoaded) return false;
			return CloseAllExceptOne(null, (docsToClose) => true);
		}
		bool CloseAllExceptOne(BaseDocument document, Func<BaseDocument[], bool> hostsCondition) {
			BaseDocument[] documents = View.Documents.ToArray();
			BaseDocument[] floatDocuments = View.FloatDocuments.ToArray();
			using(BatchUpdate.Enter(Manager)) {
				using(View.LockPainting()) {
					DocumentManager manager = Manager;
					bool closed = false;
					var documentsToClose = documents.Concat(floatDocuments)
						.Except(new BaseDocument[] { document })
						.Where(d => !d.IsDisposing && d.Properties.CanClose).ToArray();
					var hostsContext = manager.GetDocumentsHostContext();
					if(hostsContext != null && hostsCondition(documentsToClose)) {
						if(DocumentsHostContext.IsParented(manager)) {
							var parentView = hostsContext.startupManager.View;
							return ((BaseViewController)parentView.Controller).CloseAllExceptOne(document, hostsCondition);
						}
						else closed |= hostsContext.CloseAll();
					}
					for(int i = 0; i < documentsToClose.Length; i++)
						closed |= Close(documentsToClose[i]);
					if(closed)
						manager.InvokePatchActiveChildren();
					return closed;
				}
			}
		}
		public void ShowWindowsDialog() {
			if(View.IsDisposing || !View.IsLoaded) return;
			using(OpenedWindowsDialog windowsDialog = new OpenedWindowsDialog(View)) {
				if(DialogResult.OK == windowsDialog.ShowDialog()) {
					if(windowsDialog.Result != null)
						Activate(windowsDialog.Result);
				}
			}
		}
		public void ResetLayout() {
			if(View.IsDisposing || !View.IsLoaded) return;
			if(View.RaiseLayoutResetting()) {
				using(BatchUpdate.Enter(Manager)) {
					using(View.LockPainting()) {
						ResetLayoutCore();
						View.RaiseLayoutReset();
					}
				}
			}
		}
		protected virtual void ResetLayoutCore() { }
		protected static Point InvalidPoint = new Point(int.MinValue, int.MinValue);
		public bool Float(Docking.DockPanel dockPanel) {
			if(dockPanel == null || !dockPanel.IsMdiDocument) return false;
			return Float(View.Documents.FindFirst(doc => doc.GetDockPanel() == dockPanel));
		}
		public bool Float(BaseDocument document) {
			if(document == null) return false;
			Point floatLocation = document.FloatLocation.HasValue ? document.FloatLocation.Value : InvalidPoint;
			Size floatSize = document.FloatSize.HasValue ? document.FloatSize.Value : Size.Empty;
			return Float(document, floatLocation, floatSize);
		}
		public bool Float(BaseDocument document, Point floatLocation) {
			if(document == null) return false;
			Size floatSize = document.FloatSize.HasValue ? document.FloatSize.Value : Size.Empty;
			return Float(document, floatLocation, floatSize);
		}
		public bool Float(BaseDocument document, Point floatLocation, Size floatSize) {
			if(View.IsDisposing || !View.IsLoaded || document == null) return false;
			if(document.IsDeferredControlLoad) {
				if(!document.EnsureIsBoundToControl(View))
					return false;
			}
			if(document.Form != null && document.CanFloat()) {
				Form form = document.Form;
				using(View.LockPainting()) {
					using(View.FloatDocuments.Lock()) {
						using(var ctx = new DisposableObjectsContainer()) {
							ctx.Register(document.ReleaseDockPanel());
							ctx.Register(document.TrackContainerChange(View.Container));
							if(floatSize.IsEmpty && !document.IsActive)
								form.Size = View.GetBounds(document).Size;
							if(floatLocation == InvalidPoint)
								floatLocation = View.GetFloatLocation(document);
							using(new NestedDocumentManagerUpdateContext(document.Control)) {
								form.Visible = false;
								using(IDocumentFloatingContext context = GetDocumentFloatingContext(document)) {
									Manager.RemoveDocumentFromHost(document);
									UpdateFormOwner(form);
									if(!floatSize.IsEmpty)
										form.Size = floatSize;
									if(!document.IsDockPanel) {
										form.StartPosition = FormStartPosition.Manual;
										form.Location = CheckScreenLocation(floatLocation, form.Size);
										View.FloatDocuments.Add(document);
									}
									else {
										CheckDocumentLocation(document);
										View.FloatPanels.Add(document);
									}
									Manager.InvokePatchActiveChildren();
									if(!Manager.IsFloating) {
										View.OnFloating(document);
										form.Visible = true;
									}
									Manager.LayeredWindowsRegisterNotificationSourceForFloatView(form);
								}
							}
						}
						DevExpress.XtraBars.CodedUISupport.DockManagerCodedUIHelper.AddDockPanelDocumentDockInfo(document, true);
						return true;
					}
				}
			}
			return false;
		}
		void UpdateFormOwner(Form form) {
			if(form.Owner != null)
				form.Owner.RemoveOwnedForm(form);
			form.Owner = Manager.GetContainer().FindForm();
		}
		IDocumentFloatingContext GetDocumentFloatingContext(BaseDocument document) {
			if(document != null && document.IsControlLoaded) {
				var childManager = BarManager.FindManager(document.Control);
				if(childManager != null) {
					IDocumentFloatingContext context = childManager.CreateDocumentFloatingContext();
					if(context != null)
						context.SetDocument(document);
					return context;
				}
			}
			return null;
		}
		public bool FloatAll() {
			if(View.IsDisposing || !View.IsLoaded) return false;
			using(View.LockPainting()) {
				return View.AddFloatingDocumentsHost(GetChildren(View));
			}
		}
		void CheckDocumentLocation(BaseDocument document) {
			Screen screen = Screen.FromPoint(document.Form.Location);
			Rectangle workingArea = (screen != null) ? screen.WorkingArea : Rectangle.Empty;
			if(screen == null || workingArea.Contains(document.Form.Location)) return;
			int x = Math.Max(document.Form.Location.X, workingArea.Location.X);
			int y = Math.Max(document.Form.Location.Y, workingArea.Location.Y);
			document.Form.Location = new Point(x, y);
		}
		Point CheckScreenLocation(Point screenLocation, Size screenSize) {
			return CheckScreenBounds(new Rectangle(screenLocation, screenSize)).Location;
		}
		Rectangle CheckScreenBounds(Rectangle screenBounds) {
			Rectangle workingArea = GetWorkingArea(screenBounds);
			if(!workingArea.IsEmpty && !workingArea.IntersectsWith(screenBounds)) {
				workingArea = GetWorkingArea(new Rectangle(0, 0, 1, 1));
				screenBounds = new Rectangle(workingArea.Left + workingArea.Width - screenBounds.Width,
					Math.Min(Math.Max(screenBounds.Top, workingArea.Top), workingArea.Bottom - screenBounds.Height),
					screenBounds.Width, screenBounds.Height);
			}
			return screenBounds;
		}
		Rectangle GetWorkingArea(Rectangle screenBounds) {
			if(SystemInformation.MonitorCount > 1)
				return Screen.FromRectangle(screenBounds).WorkingArea;
			return SystemInformation.WorkingArea;
		}
		public bool Dock(BaseDocument document) {
			if(document == null || (!document.IsDockPanel && View.Documents.Contains(document)) || IsDockedAsDockPanel(document))
				return false;
			using(BatchUpdate.Enter(Manager)) {
				using(View.LockPainting()) {
					using(IDockOperation operation = View.BeginDockOperation(document)) {
						if(operation.Canceled) return false;
						if(document.IsFloating)
							DockFromFloating(document);
						if(document.IsDocumentsHost) {
							return DockFromDocumentsHost(document,
								(childDocument) => AddDocumentCore(childDocument));
						}
						else return DockCore(document);
					}
				}
			}
		}
		protected bool IsDockedAsDockPanel(BaseDocument document) {
			if(document != null) {
				var panelInfo = document.Control as Docking.IDockPanelInfo;
				if(panelInfo != null)
					return panelInfo.DockedAsMdiDocument;
			}
			return false;
		}
		public bool Dock(Docking.DockPanel dockPanel) {
			if(dockPanel == null || !dockPanel.Options.AllowDockAsTabbedDocument) return false;
			if(IsAlreadyDockedAsTabbedDocument(dockPanel))
				return true;
			Docking.FloatForm floatForm = dockPanel.EnsureFloatForm(true);
			if(floatForm == null) return false;
			using(BatchUpdate.Enter(Manager)) {
				using(View.LockPainting()) {
					bool registered = false;
					BaseDocument document;
					using(View.FloatPanels.LockFloatPanelRegistration(dockPanel)) {
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
							return Dock(document);
						}
					}
				}
			}
		}
		protected bool IsAlreadyDockedAsTabbedDocument(Docking.DockPanel dockPanel) {
			if(dockPanel.Visibility != Docking.DockVisibility.Visible)
				dockPanel.Visibility = Docking.DockVisibility.Visible;
			BaseDocument document;
			if(View.Documents.TryGetValue(dockPanel, out document))
				return dockPanel.DockedAsTabbedDocument;
			return false;
		}
		protected abstract bool DockCore(BaseDocument baseDocument);
		protected void DockFromFloating(BaseDocument document) {
			Manager.BeginDocking();
			try {
				using(new NestedDocumentManagerUpdateContext(document.Control)) {
					Form form = document.Form;
					document.EnsureFloatingBounds();
					ContainerControlManagementStrategy.Hide(form.Handle);
					if(document.IsDockPanel)
						DockFloatPanel(document);
					else
						DockFloatDocument(document);
					Manager.DockFloatForm(document, PatchControlBeforeAdd);
				}
			}
			finally { Manager.EndDocking(); }
		}
		protected bool DockFromDocumentsHost(BaseDocument document, Action<BaseDocument> dockFromHostView) {
			Manager.BeginDocking();
			document.LockFormDisposing();
			try {
				IDocumentsHostWindow hostWindow = document.Form as IDocumentsHostWindow;
				if(hostWindow != null) {
					var context = hostWindow.DocumentManager.GetDocumentsHostContext();
					context.QueueDelete(document, hostWindow);
					BaseView hostView = hostWindow.DocumentManager.View;
					var children = GetChildren(hostView);
					foreach(var childDocument in children) {
						if(childDocument.Control != null)
							hostWindow.DocumentManager.RemoveDocumentFromHost(childDocument);
						if(hostView.Container != null)
							hostView.Container.Remove(childDocument);
						hostView.Documents.Remove(childDocument);
						childDocument.SetManager(Manager);
						View.Documents.Add(childDocument);
						CheckOwnedForms((Form)hostWindow);
						dockFromHostView(childDocument);
						if(View.Container != null)
							View.Container.Add(childDocument);
						if(childDocument.Control != null)
							Manager.DockFloatForm(childDocument, PatchControlBeforeAdd);
					}
					context.DequeueOnDocking(document);
				}
				return hostWindow != null;
			}
			finally {
				document.UnLockFormDisposing();
				Manager.EndDocking();
			}
		}
		void CheckOwnedForms(Form hostWindow) {
			if(hostWindow == null) return;
			var ownedForms = new List<Form>(hostWindow.OwnedForms);
			var managerForm = DocumentsHostContext.GetForm(Manager);
			for(int i = 0; i < ownedForms.Count; i++) {
				if(hostWindow != managerForm)
					hostWindow.RemoveOwnedForm(ownedForms[i]);
				ownedForms[i].Owner = managerForm;
			}
		}
		protected virtual BaseDocument[] GetChildren(BaseView hostView) {
			return hostView.Documents.ToArray();
		}
		void DockFloatPanel(BaseDocument document) {
			using(document.TrackContainerChange(View.Container)) {
				View.FloatPanels.Remove(document);
				Docking.FloatForm fForm = document.Form as Docking.FloatForm;
				fForm.ResetSizingAdorner();
				if(fForm.FloatLayout.HasChildren)
					DockFloatPanelChildren(document, fForm);
				else View.Documents.Add(document);
				if(View.Documents.Contains(document) && document.Manager == null)
					document.SetManager(Manager);
			}
		}
		void DockFloatPanelChildren(BaseDocument document, Docking.FloatForm fForm) {
			List<Guid> ids = new List<Guid>();
			IList<Docking.Helpers.LayoutInfo> children = fForm.FloatLayout.GetChildren();
			foreach(Docking.Helpers.DockLayout child in children)
				ids.Add(child.Panel.ID);
			document.LockFormDisposing();
			try {
				foreach(Guid id in ids) {
					Docking.DockPanel panel = Manager.DockManager.Panels[id];
					panel.MakeFloat(new Point(-10000, -10000));
					panel.FloatForm.Visible = false;
					BaseDocument childDocument = CreateAndInitializeDocument(panel.FloatForm);
					if(childDocument != null) {
						View.Documents.Add(childDocument);
						document.AddChild(childDocument);
						Manager.DockFloatForm(childDocument, panel);
					}
				}
			}
			finally { document.UnLockFormDisposing(); }
		}
		void DockFloatDocument(BaseDocument document) {
			var sourceView = GetView(document);
			using(var ctx = new DisposableObjectsContainer()) {
				ctx.Register(sourceView.FloatDocuments.Lock());
				ctx.Register(Manager.ActivationInfo.LockActiveDocumentChanging());
				ctx.Register(document.TrackContainerChange(View.Container));
				sourceView.FloatDocuments.Remove(document);
				View.Documents.Add(document);
			}
		}
		public bool ShowContextMenu(Point point) {
			if(View.IsDisposing || !View.IsLoaded) return false;
			this.menuTargetInfoCore = Manager.CalcHitInfo(point).HitElement;
			try {
				BaseViewControllerMenu menu = CreateContextMenu();
				menu.Init(View);
				menu.PlacementTarget = Manager.GetOwnerControl();
				return ShowContextMenuCore(menu, point);
			}
			finally { this.menuTargetInfoCore = null; }
		}
		public bool ShowContextMenu(BaseDocument document, Point point) {
			if(View.IsDisposing || !View.IsLoaded) return false;
			if(document != null) {
				this.menuTargetInfoCore = Manager.CalcHitInfo(point).HitElement;
				try {
					BaseViewControllerMenu menu = CreateContextMenu();
					menu.Init(document);
					menu.PlacementTarget = CalculatePlacementTarget(document);
					return ShowContextMenuCore(menu, point);
				}
				finally { this.menuTargetInfoCore = null; }
			}
			return false;
		}
		protected virtual Control CalculatePlacementTarget(BaseDocument document) {
			return document.IsFloating ? document.Form : Manager.GetOwnerControl();
		}
		protected bool ShowContextMenuCore(BaseViewControllerMenu menu, Point point) {
			bool shown = false;
			Ref.Dispose(ref menuCore);
			if(View.CanShowContextMenu(menu, point)) {
				menuCore = menu;
				DevExpress.Utils.Menu.IDXMenuManager menuManager = Manager.GetMenuManager();
				if(menuManager != null) {
					Menu.CloseUp += Menu_CloseUp;
					menuManager.ShowPopupMenu(Menu, Menu.PlacementTarget, point);
					shown = true;
				}
			}
			else menu.Dispose();
			return shown;
		}
		void Menu_CloseUp(object sender, EventArgs e) {
			BaseViewControllerMenu menu = sender as BaseViewControllerMenu;
			if(menu != null)
				menu.CloseUp -= Menu_CloseUp;
			Ref.Dispose(ref menuCore);
		}
		public void CloseMenu() {
			if(Menu != null && Menu.Visible)
				Menu.Visible = false;
		}
		IBaseElementInfo menuTargetInfoCore;
		protected IBaseElementInfo MenuTargetInfo {
			get { return menuTargetInfoCore; }
		}
		protected BaseViewControllerMenu menuCore;
		public BaseViewControllerMenu Menu {
			get { return menuCore; }
		}
		protected virtual BaseViewControllerMenu CreateContextMenu() {
			return new BaseViewControllerMenu(this);
		}
		public IEnumerable<BaseViewControllerCommand> GetCommands(BaseDocument document) {
			List<BaseViewControllerCommand> commands = new List<BaseViewControllerCommand>();
			GetCommandsCore(document, commands);
			commands.ForEach(command => command.Parameter = document);
			commands.Sort(BaseViewControllerCommand.Compare);
			return commands;
		}
		public IEnumerable<BaseViewControllerCommand> GetCommands(BaseView view) {
			List<BaseViewControllerCommand> commands = new List<BaseViewControllerCommand>();
			GetCommandsCore(commands);
			commands.ForEach(command => InitViewCommandParameter(command, view));
			commands.Sort(BaseViewControllerCommand.Compare);
			return commands;
		}
		protected virtual void InitViewCommandParameter(BaseViewControllerCommand command, BaseView view) {
			command.Parameter = view;
		}
		public void CreateBarDockingMenuItemCommands(BarDockingMenuItem documentListItem) {
			List<BaseViewControllerCommand> commands = new List<BaseViewControllerCommand>();
			commands.AddRange(GetCommands(View));
			commands.Add(GetResetLayoutCommand());
			BaseDocument activeDocument = Manager.ActivationInfo.ActiveDocument;
			if(activeDocument != null) {
				var actualView = GetView(activeDocument);
				var documentCommands = actualView.Controller.GetCommands(activeDocument);
				CheckFloatAllCommandForActiveDocument(commands, documentCommands);
				commands.AddRange(documentCommands);
			}
			commands.Sort(BaseViewControllerCommand.Compare);
			using(BaseViewControllerMenu menu = CreateContextMenu()) {
				menu.Init(documentListItem, commands);
			}
		}
		public BaseView GetView(BaseDocument document) {
			BaseView view = View;
			var hostsContext = Manager.GetDocumentsHostContext();
			if(hostsContext != null) {
				if(document != null && document.Manager != Manager) {
					if(document.Manager != null && document.Manager.View != null)
						view = document.Manager.View;
				}
			}
			return view;
		}
		protected virtual BaseViewControllerCommand GetResetLayoutCommand() {
			BaseViewControllerCommand resetLayoutCommand = BaseViewControllerCommand.ResetLayout;
			resetLayoutCommand.Parameter = View;
			return resetLayoutCommand;
		}
		protected virtual bool CanFloatAll(BaseDocument document) {
			return View.CanFloatAll();
		}
		protected virtual bool CanFloatAll() {
			return View.CanFloatAll();
		}
		protected virtual BaseViewControllerCommand GetFloatAllCommand(BaseDocument document) {
			return BaseViewControllerCommand.FloatAll;
		}
		protected virtual BaseViewControllerCommand GetFloatAllCommand() {
			return BaseViewControllerCommand.FloatAll;
		}
		protected virtual void CheckFloatAllCommandForActiveDocument(List<BaseViewControllerCommand> commands, IEnumerable<BaseViewControllerCommand> documentCommands) {
			RemoveDocumentCommand(commands, documentCommands, BaseViewControllerCommand.FloatAll, BaseViewControllerCommand.FloatAll);
		}
		protected void RemoveDocumentCommand(List<BaseViewControllerCommand> commands, IEnumerable<BaseViewControllerCommand> documentCommands, BaseViewControllerCommand command, BaseViewControllerCommand documentCommand) {
			if(commands.Contains(command)) {
				if(documentCommands.Contains(documentCommand))
					commands.Remove(command);
			}
		}
		protected virtual void GetCommandsCore(BaseDocument document, IList<BaseViewControllerCommand> commands) {
			commands.Add(BaseViewControllerCommand.Close);
			if(View.CanCloseAllButThis(document))
				commands.Add(BaseViewControllerCommand.CloseAllButThis);
			if(!document.IsFloating)
				commands.Add(BaseViewControllerCommand.Float);
			if(CanFloatAll(document))
				commands.Add(GetFloatAllCommand(document));
		}
		protected virtual void GetCommandsCore(IList<BaseViewControllerCommand> commands) {
			int floatDocumentsCount = View.GetFloatDocumentCount();
			if(View.Documents.Count + floatDocumentsCount > 0) {
				if(View.CanCloseAll())
					commands.Add(BaseViewControllerCommand.CloseAll);
				if(CanFloatAll())
					commands.Add(GetFloatAllCommand());
				commands.Add(BaseViewControllerCommand.ShowWindowsDialog);
			}
		}
		public bool Execute(BaseViewControllerCommand command, object parameter) {
			if(command != null) {
				var args = new BaseViewControllerCommand.Args(command, parameter);
				if(BaseViewControllerCommand.CanExecute(this, args)) {
					BaseViewControllerCommand.Execute(this, args);
					return true;
				}
			}
			return false;
		}
	}
}
