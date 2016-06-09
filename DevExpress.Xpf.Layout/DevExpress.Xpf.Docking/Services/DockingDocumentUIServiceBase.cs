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

using DevExpress.Xpf.Core;
using DevExpress.Xpf.Docking;
using DevExpress.Mvvm;
using DevExpress.Mvvm.Native;
using DevExpress.Mvvm.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using DevExpress.Mvvm.UI.Interactivity;
using DevExpress.Xpf.Docking.Native;
using DevExpress.Xpf.Docking.Base;
using System.ComponentModel;
using DevExpress.Xpf.Layout.Core;
using System.Collections.ObjectModel;
using DevExpress.Xpf.Core.Serialization;
using System.Text.RegularExpressions;
namespace DevExpress.Xpf.Docking.Native {
	public abstract class DockingDocumentUIServiceBase<TPanel, TGroup> : DocumentUIServiceBase, IDocumentManagerService, IDocumentOwner
		where TPanel : LayoutPanel
		where TGroup : LayoutGroup {
		public class Document : BindableBase, IDocument, IDocumentInfo {
			public DockingDocumentUIServiceBase<TPanel, TGroup> Owner { get; private set; }
			public TPanel DocumentPanel { get; private set; }
			public event EventHandler RemoveDocumentPanel;
			public event EventHandler HideDocumentPanel;
			DocumentState state = DocumentState.Hidden;
			public Document(DockingDocumentUIServiceBase<TPanel, TGroup> owner, TPanel documentPanel, string type) {
				this.Owner = owner;
				this.DocumentPanel = documentPanel;
				DocumentType = type;
				SetDocument(documentPanel, this);
				UpdateCloseCommand();
			}
			void IDocument.Close(bool force) {
				CloseCore(force);
			}
			void RemoveFromItems() {
				DocumentViewModelHelper.OnDestroy(GetContent());
				if(RemoveDocumentPanel != null)
					RemoveDocumentPanel(this, EventArgs.Empty);
				var manager = DocumentPanel.GetDockLayoutManager();
				if(manager != null && manager.DockController != null) manager.DockController.RemovePanel(DocumentPanel);
				else DocumentPanel.Parent.Items.Remove(DocumentPanel);
				Owner.DocumentsCollection.Remove(this);
				(GetContent() as IDocumentContent).Do(x => x.DocumentOwner = null);
				DocumentPanel.Content = null;
				DocumentPanel = null;
				Owner = null;
			}
			void IDocument.Show() {
				DocumentPanel.Closed = false;
				DocumentPanel.IsActive = true;
				state = DocumentState.Visible;
			}
			void IDocument.Hide() {
				HidePanel();
				state = DocumentState.Hidden;
			}
			bool destroyOnClose = true;
			public bool DestroyOnClose {
				get { return destroyOnClose; }
				set { destroyOnClose = value; }
			}
			void UpdateCloseCommand() {
				DocumentPanel.CloseCommand = DelegateCommandFactory.Create(() => CloseCore(false), false);
			}
			void HidePanel() {
				DocumentPanel.Closed = true;
				DocumentPanel.IsActive = false;
				state = DocumentState.Hidden;
			}
			void CloseCore(bool force) {
				using(DocumentPanel.LockCloseCommand()) {
					if(!force) {
						CancelEventArgs e = new CancelEventArgs();
						DocumentViewModelHelper.OnClose(GetContent(), e);
						if(e.Cancel) return;
					}
					if(destroyOnClose) {
						RemoveFromItems();
						state = DocumentState.Destroyed;
					}
					else {
						if(HideDocumentPanel != null)
							HideDocumentPanel(this, EventArgs.Empty);
						HidePanel();
					}
				}
			}
			public object Title {
				get { return DocumentPanel.Caption; }
				set { DocumentPanel.Caption = value; }
			}
			public object Content { get { return GetContent(); } }
			public object Id {
				get { return BindableId; }
				set { BindableId = value; }
			}
			object GetContent() { return ViewHelper.GetViewModelFromView(DocumentPanel.Content); }
			DocumentState IDocumentInfo.State { get { return state; } }
			public string DocumentType {
				get;
				private set;
			}
			public object BindableId {
				get { return GetProperty(() => BindableId); }
				set { SetProperty(() => BindableId, value); }
			}
		}
		class IgnoreIncorrectNameValuesConverter : IValueConverter {
			public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
				var typed = value as string;
				if(typed == null || !Regex.IsMatch(typed, @"^[_a-zA-Z][a-zA-Z0-9]*$"))
					return null;
				return value;
			}
			public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
				throw new NotImplementedException();
			}
		}
		public static readonly DependencyProperty ActiveDocumentProperty =
			DependencyProperty.Register("ActiveDocument", typeof(IDocument), typeof(DockingDocumentUIServiceBase<TPanel, TGroup>),
			new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, (d, e) => ((DockingDocumentUIServiceBase<TPanel, TGroup>)d).OnActiveDocumentChanged(e.OldValue as IDocument, e.NewValue as IDocument)));
		static readonly DependencyPropertyKey ActiveViewPropertyKey =
			DependencyProperty.RegisterReadOnly("ActiveView", typeof(object), typeof(DockingDocumentUIServiceBase<TPanel, TGroup>), new PropertyMetadata(null));
		public static readonly DependencyProperty ActiveViewProperty = ActiveViewPropertyKey.DependencyProperty;
		static readonly DependencyPropertyKey DocumentsPropertyKey =
			DependencyProperty.RegisterReadOnly("Documents", typeof(IEnumerable<IDocument>), typeof(DockingDocumentUIServiceBase<TPanel, TGroup>), new PropertyMetadata(null));
		public static readonly DependencyProperty DocumentsProperty = DocumentsPropertyKey.DependencyProperty;
		public IDocument ActiveDocument {
			get { return (IDocument)GetValue(ActiveDocumentProperty); }
			set { SetValue(ActiveDocumentProperty, value); }
		}
		public object ActiveView {
			get { return GetValue(ActiveViewProperty); }
			private set { SetValue(ActiveViewPropertyKey, value); }
		}
		public IEnumerable<IDocument> Documents {
			get { return (IEnumerable<IDocument>)GetValue(DocumentsProperty); }
			private set { SetValue(DocumentsPropertyKey, value); }
		}
		ObservableCollection<IDocument> DocumentsCollection { get { return (ObservableCollection<IDocument>)Documents; } }
		Dictionary<TPanel, FloatGroup> removingPanelsCollection;
		List<TPanel> floatGroupPanelsCollection;
		public DockingDocumentUIServiceBase() {
			Documents = new ObservableCollection<IDocument>();
			removingPanelsCollection = new Dictionary<TPanel, FloatGroup>();
			floatGroupPanelsCollection = new List<TPanel>();
		}
		protected abstract TGroup GetActualDocumentGroup();
		protected abstract TPanel CreateDocumentPanel();
		protected abstract Style GetDocumentPanelStyle(TPanel documentPanel, object documentContentView);
		IDocument IDocumentManagerService.CreateDocument(string documentType, object viewModel, object parameter, object parentViewModel) {
			TGroup actualDocumentGroup = GetActualDocumentGroup();
			if(actualDocumentGroup == null)
				return null;
			if(actualDocumentGroup.Manager != null) {
				actualDocumentGroup.Manager.DockItemActivated -= DocumentItemActivated;
				actualDocumentGroup.Manager.DockItemActivated += DocumentItemActivated;
				actualDocumentGroup.Manager.DockItemClosing -= DockItemClosing;
				actualDocumentGroup.Manager.DockItemClosing += DockItemClosing;
			}
			object documentContentView = CreateAndInitializeView(documentType, viewModel, parameter, parentViewModel, this);
			var documentPanel = CreateDocumentPanel();
			InitializeDocumentContainer(documentPanel, LayoutPanel.ContentProperty, GetDocumentPanelStyle(documentPanel, documentContentView));
			var document = new Document(this, documentPanel, documentType);
			BindingOperations.SetBinding(documentPanel, LayoutPanel.BindableNameProperty, 
				new Binding("BindableId") { Source = document, Converter = new IgnoreIncorrectNameValuesConverter() });
			if(documentContentView is DependencyObject) {
				DXSerializer.SetEnabled((DependencyObject)documentContentView, false);
			}
			SetTitleBinding(documentContentView, DocumentPanel.CaptionProperty, documentPanel);
			documentPanel.PreventCaptionSerialization = true;
			documentPanel.Content = documentContentView;
			actualDocumentGroup.Items.Add(documentPanel);
			document.RemoveDocumentPanel += RemoveDocumentPanel;
			document.HideDocumentPanel += HideDocumentPanel;
			DocumentsCollection.Add(document);
			return document;
		}
		void DockItemClosing(object sender, ItemCancelEventArgs e) {
			FloatGroup floatGroup = e.Item as FloatGroup;
			if(floatGroup == null) return;
			e.Cancel = !DocumentsClose(floatGroup);
			if(e.Cancel == false)
				floatGroupPanelsCollection.Clear();
		}
		bool DocumentsClose(FloatGroup floatGroup) {
			bool allDocumentsClosed = true;
			foreach(TPanel panel in GetItemsOfFloatGroup(floatGroup)) {
				panel.ExecuteCloseCommand();
				if(((Document)GetDocument(panel)).DocumentPanel != null) {
					allDocumentsClosed = false;
					if(!removingPanelsCollection.ContainsKey(panel)) {
						removingPanelsCollection.Add(panel, floatGroup);
						panel.Unloaded += PanelUnloaded;
					}
				}
			}
			return allDocumentsClosed;
		}
		void PanelUnloaded(object sender, RoutedEventArgs e) {
			TPanel panel = (TPanel)sender;
			panel.Unloaded -= PanelUnloaded;
			FloatGroup floatGroup = removingPanelsCollection[panel];
			if(floatGroup == null) return;
			removingPanelsCollection.Remove(panel);
			if(GetItemsOfFloatGroup(floatGroup).Count() == 0)
				floatGroup.Manager.DockController.Close(floatGroup);
		}
		IEnumerable<TPanel> GetItemsOfFloatGroup(FloatGroup floatGroup) {
			floatGroupPanelsCollection.Clear();
			LayoutGroup layoutGroup = floatGroup.Items.FirstOrDefault() as LayoutGroup;
			if(layoutGroup != null)
				GetDocumentPanels(layoutGroup);
			return floatGroupPanelsCollection.OrderBy(x => x.IsActive == true);
		}
		void GetDocumentPanels(LayoutGroup group) {
			foreach(var item in group.Items) {
				LayoutGroup layoutGroupItem = item as LayoutGroup;
				if(layoutGroupItem != null)
					GetDocumentPanels(layoutGroupItem);
				TGroup tGroupItem = item as TGroup;
				if(tGroupItem != null) {
					foreach(var panelItem in tGroupItem.Items)
						floatGroupPanelsCollection.Add((TPanel)panelItem);
				}
			}
		}
		void IDocumentOwner.Close(IDocumentContent documentContent, bool force) {
			CloseDocument(this, documentContent, force);
		}
		public event ActiveDocumentChangedEventHandler ActiveDocumentChanged;
		bool activeDocumentChangeEnabled = true;
		bool ActiveDocumentChangeEnabled() {
			if(activeDocumentChangeEnabled) return true;
			activeDocumentChangeEnabled = true;
			return false;
		}
		void HideDocumentPanel(object sender, EventArgs e) {
			Document senderDocument = (Document)sender;
			ActiviteAfterRemoveOrHidePanelGroup(senderDocument);
		}
		void RemoveDocumentPanel(object sender, EventArgs e) {
			Document document = (Document)sender;
			document.RemoveDocumentPanel -= RemoveDocumentPanel;
			document.HideDocumentPanel -= HideDocumentPanel;
			ActiviteAfterRemovePanel(document);
			ActiviteAfterRemoveOrHidePanelGroup(document);
		}
		void ActiviteAfterRemovePanel(Document document) {
			if(document != ActiveDocument) return;
			if(GetCountOfOpenDocuments(document) > 1) {
				var collection = document.DocumentPanel.Parent.Items.Where(x => x as TPanel != null && x.Closed == false).Cast<TPanel>();
				int panelIndex = 0;
				foreach(TPanel panel in collection) {
					if(ActiveDocument == GetDocument(panel) && collection.Count() > 1) {
						TPanel newActiveDocumentPanel;
						if(panelIndex == 0)
							newActiveDocumentPanel = collection.ElementAt(panelIndex + 1);
						else
							newActiveDocumentPanel = collection.ElementAt(panelIndex - 1);
						this.SetCurrentValue(ActiveDocumentProperty, GetDocument(newActiveDocumentPanel));
						return;
					}
					panelIndex++;
				}
			}
		}
		void ActiviteAfterRemoveOrHidePanelGroup(Document document) {
			if(document != ActiveDocument) return;
			if(GetCountOfOpenDocuments(document) == 1 && DocumentsCollection.Count > 1) {
				foreach(TPanel item in DocumentsCollection.OfType<Document>().Select(x => x.DocumentPanel)) {
					if(item != document.DocumentPanel && item.IsSelectedItem) {
						activeDocumentChangeEnabled = false;
						SetCurrentValue(ActiveDocumentProperty, GetDocument(item));
						return;
					}
				}
			}
			if(DocumentsCollection.Count(x => ((Document)x).DocumentPanel.Closed == false) == 1)
				SetCurrentValue(ActiveDocumentProperty, null);
		}
		int GetCountOfOpenDocuments(Document document) {
			return document.DocumentPanel.Parent.Items.Count(x => x.Closed == false);
		}
		void DocumentItemActivated(object sender, DockItemActivatedEventArgs e) {
			TPanel newPanel = e.Item as TPanel;
			TPanel oldPanel = e.OldItem as TPanel;
			if(newPanel == null || !newPanel.IsActive || !ActiveDocumentChangeEnabled() || GetDocument(newPanel) == null) return;
			if(ActiveDocument != GetDocument(newPanel)) {
				activeDocumentChangeEnabled = false;
				SetCurrentValue(ActiveDocumentProperty, GetDocument(newPanel));
			}
		}
		void OnActiveDocumentChanged(IDocument oldValue, IDocument newValue) {
			var newDocument = (Document)newValue;
			if(newDocument != null && ActiveDocumentChangeEnabled()) {
				activeDocumentChangeEnabled = false;
				newDocument.DocumentPanel.Closed = false;
				newDocument.DocumentPanel.IsActive = true;
			}
			ActiveView = newDocument.With(x => x.DocumentPanel.Content);
			if(ActiveDocumentChanged != null)
				ActiveDocumentChanged(this, new ActiveDocumentChangedEventArgs(oldValue, newValue));
		}
	}
}
