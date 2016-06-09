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

namespace DevExpress.Utils.MVVM.Services {
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.Linq;
	using System.Windows.Forms;
	[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
	public interface IDocumentAdapter : IDisposable {
		void Show(Control control);
		void Close(Control control, bool force = true);
		event CancelEventHandler Closing;
		event EventHandler Closed;
	}
	public interface IDocumentAdapterFactory {
		IDocumentAdapter Create();
	}
	[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
	public interface IDocumentContentProvider : UI.IViewLocator {
		bool CanAddContent { get; }
		void AddContent(string title, string viewType);
	}
	public class DocumentManagerService : ViewServiceBase {
		#region static
		public static DocumentManagerService Create(IDocumentAdapterFactory factory) {
			return Create(() => factory.Create());
		}
		public static DocumentManagerService Create(Func<IDocumentAdapter> factoryMethod) {
			IMVVMServiceTypesResolver typesResolver = MVVMTypesResolver.Instance as IMVVMServiceTypesResolver;
			return DynamicServiceSource.Create<DocumentManagerService, Func<IDocumentAdapter>>(
					new Type[] { 
						typesResolver.GetIDocumentManagerServiceType(), 
						typesResolver.GetIDocumentOwnerType(), 
					}, factoryMethod);
		}
		#endregion static
		Func<IDocumentAdapter> factoryMethod;
		List<Document> documents = new List<Document>();
		protected DocumentManagerService(Func<IDocumentAdapter> factoryMethod) {
			this.factoryMethod = factoryMethod;
		}
		public object Documents {
			get { return Document.EnumerableCast(documents); } 
		}
		public object CreateDocument(string documentType, object viewModel, object parameter, object parentViewModel) {
			IDocumentAdapter documentAdapter = factoryMethod();
			if(documentAdapter != null)
				Initialize(documentAdapter);
			IDocumentContentProvider contentProvider = documentAdapter as IDocumentContentProvider;
			var viewLocator = (contentProvider != null) && contentProvider.CanAddContent ? contentProvider as UI.IViewLocator : null;
			var document = Document.Create(this, documentAdapter,
				() => CreateAndInitializeView(documentType, viewModel, parameter, parentViewModel, viewLocator));
			document.DocumentType = documentType;
			document.Id = parameter;
			return document;
		}
		public void Close(object documentContent, bool force) {
			var document = documents.FirstOrDefault((d) => object.Equals(d.Content, documentContent));
			if(document != null)
				document.Close(force);
		}
		protected virtual void Initialize(IDocumentAdapter adapter) {}
		#region Document
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public class Document : IDisposable {
			#region static
			protected internal static Document Create(DocumentManagerService service, IDocumentAdapter adapter, Func<Control> createView) {
				IMVVMServiceTypesResolver typesResolver = MVVMTypesResolver.Instance as IMVVMServiceTypesResolver;
				return DynamicServiceSource.Create<Document, DocumentManagerService, IDocumentAdapter, Func<Control>>(
					new Type[] { 
						typesResolver.GetIDocumentType(), 
						typesResolver.GetIDocumentInfoType(), 
					}, service, adapter, createView);
			}
			static Func<IEnumerable, object> documentsConverter;
			protected internal static object EnumerableCast(IEnumerable documents) {
				return DynamicCastHelper.GetEnumerableCast(ref documentsConverter, GetIDocumentType)(documents);
			}
			static Type GetIDocumentType() {
				return (MVVMTypesResolver.Instance as IMVVMServiceTypesResolver).GetIDocumentType();
			}
			#endregion
			DocumentManagerService service;
			IDocumentAdapter adapter;
			protected Document(DocumentManagerService service, IDocumentAdapter adapter, Func<Control> createView) {
				this.stateCore = 1 ;
				this.adapter = adapter;
				this.viewCore = new Lazy<Control>(new Func<Control>(() =>
				{
					var v = createView();
					if(!(v is UI.ViewActivator.ViewPlaceholder))
						v.Text = (titleCore ?? MVVMContext.GetTitle(v)) as string;
					return v;
				}));
				this.service = service;
				adapter.Closing += adapter_Closing;
				adapter.Closed += adapter_Closed;
				service.documents.Add(this);
			}
			public void Dispose() {
				stateCore = 2 ;
				OnDisposing();
				GC.SuppressFinalize(this);
			}
			void OnDisposing() {
				if(service != null) {
					service.documents.Remove(this);
					service = null;
				}
				if(adapter != null) {
					adapter.Closing -= adapter_Closing;
					adapter.Closed -= adapter_Closed;
					adapter.Dispose();
					adapter = null;
				}
				this.contentCore = null;
				this.viewCore = null;
			}
			Lazy<Control> viewCore;
			protected Control View {
				get { return (viewCore != null) ? viewCore.Value : null; }
			}
			protected bool IsViewCreated {
				get { return (viewCore != null) && viewCore.IsValueCreated; }
			}
			object contentCore;
			public object Content {
				get {
					if(!IsViewCreated)
						return null;
					if(contentCore == null)
						contentCore = MVVMContext.GetViewModel(View);
					return contentCore;
				}
			}
			public void Show() {
				if(!IsViewCreated) {
					IDocumentContentProvider provider = adapter as IDocumentContentProvider;
					if(provider != null && provider.CanAddContent)
						provider.AddContent(titleCore as string, DocumentType);
				}
				adapter.Show(View);
				stateCore = 0 ;
			}
			public void Close(bool force = true) {
				if(force) {
					ValidationHelper.Reset(View);
					adapter.Closing -= adapter_Closing;
				}
				adapter.Close(View, force);
				stateCore = 1 ;
			}
			public void Hide() {
				Close();
			}
			void adapter_Closing(object sender, CancelEventArgs e) {
				MVVMContext.OnClose(View, e);
			}
			void adapter_Closed(object sender, EventArgs e) {
				MVVMContext.OnDestroy(View);
				Dispose();
			}
			int stateCore;
			public int State {
				get { return stateCore; }
			}
			object titleCore;
			public object Title {
				get { return IsViewCreated ? View.Text : titleCore; }
				set {
					if(IsViewCreated)
						View.Text = value as string;
					else
						titleCore = value;
				}
			}
			public object Id { get; set; }
			public bool DestroyOnClose { get; set; }
			public string DocumentType { get; set; }
		}
		#endregion
	}
}
