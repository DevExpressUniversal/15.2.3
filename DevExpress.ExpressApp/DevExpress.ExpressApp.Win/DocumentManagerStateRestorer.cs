#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       eXpressApp Framework                                        }
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
using System.IO;
using System.Windows.Forms;
using DevExpress.ExpressApp.Utils;
using DevExpress.XtraBars.Docking2010.Views;
namespace DevExpress.ExpressApp.Win {
	public class DocumentManagerStateRestorer : IDisposable {
		private Dictionary<BaseDocument, DocumentControlDescription> documentToSerializedControlMap = new Dictionary<BaseDocument, DocumentControlDescription>();
		private QueryControlEventHandler queryControlEventHandler;
		public void DeserializeDocumentManagerState(BaseView view, DocumentManagerState documentManagerState, DeserializeDocumentControlDelegate deserializeDocumentControlDelegate) {
			DeserializeDocumentManagerState(view, documentManagerState, deserializeDocumentControlDelegate, null);
		}
		public void DeserializeDocumentManagerState(BaseView view, DocumentManagerState documentManagerState, DeserializeDocumentControlDelegate deserializeDocumentControlDelegate, CanDeserializeDocumentControlDelegate canDeserializeDocumentControlDelegate) {
			Guard.ArgumentNotNull(deserializeDocumentControlDelegate, "deserializeDocumentControlDelegate");
			view.Disposed += new EventHandler(view_Disposed);
			queryControlEventHandler = new QueryControlEventHandler(delegate(object sender, QueryControlEventArgs e) {
				DocumentControlDescription docControlDescription;
				if(documentToSerializedControlMap.TryGetValue(e.Document, out docControlDescription)) {
					e.Control = deserializeDocumentControlDelegate(docControlDescription.SerializedControl);
					Guard.ArgumentNotNull(e.Control, "e.Control");
					e.Control.Name = e.Document.ControlName;
				}
			});
			view.QueryControl += queryControlEventHandler; 
			foreach(DocumentDescription description in documentManagerState.DocumentDescriptions) {
				DocumentControlDescription documentControlDescription = new DocumentControlDescription(description.SerializedControl, description.ImageName);
				if(canDeserializeDocumentControlDelegate== null || canDeserializeDocumentControlDelegate(documentControlDescription)) {
					BaseDocument document = view.AddDocument(description.Caption, description.ControlName);
					document.Image = ImageLoader.Instance.GetImageInfo(description.ImageName).Image;
					documentToSerializedControlMap.Add(document, documentControlDescription);
				}
			}
			if(!string.IsNullOrEmpty(documentManagerState.ViewLayout)) {
				MemoryStream stream = new MemoryStream(Convert.FromBase64String(documentManagerState.ViewLayout));
				stream.Position = 0;
				view.RestoreLayoutFromStream(stream);
			}
		}
		private void view_Disposed(object sender, EventArgs e) {
			BaseView view = (BaseView)sender;
			view.Disposed -= new EventHandler(view_Disposed);
			view.QueryControl -= queryControlEventHandler;
			queryControlEventHandler = null;
		}
		public DocumentManagerState SerializeDocumentManagerState(BaseView view, SerializeDocumentControlDelegate serializeDocumentControlDelegate) {
			DocumentManagerState result = new DocumentManagerState();
			List<DocumentDescription> recentPages = new List<DocumentDescription>();
			foreach(BaseDocument document in new List<BaseDocument>(view.Documents)) {
				DocumentDescription documentDescription = new DocumentDescription();
				documentDescription.Caption = document.Caption;
				if(document.Control == null) {
					DocumentControlDescription docControlDescription;
					if(documentToSerializedControlMap.TryGetValue(document, out docControlDescription)) {
						documentDescription.ControlName = document.ControlName;
						documentDescription.SerializedControl = docControlDescription.SerializedControl;
						documentDescription.ImageName = docControlDescription.ImageName;
					}
				}
				else {
					DocumentControlDescription controlDescription = serializeDocumentControlDelegate(document.Control);
					if(controlDescription != null) {
						documentDescription.SerializedControl = controlDescription.SerializedControl;
						documentDescription.ImageName = controlDescription.ImageName;
						documentDescription.ControlName = document.Control.Name;
					}
				}
				if(!string.IsNullOrEmpty(documentDescription.SerializedControl)) {
					string controlName = Guid.NewGuid().ToString();
					documentDescription.ControlName = controlName;
					document.ControlName = controlName;
					if(document.Control != null) {
						document.Control.Name = controlName;
					}
					recentPages.Add(documentDescription);
				}
			}
			result.DocumentDescriptions = recentPages.ToArray();
			MemoryStream stream = new MemoryStream();
			view.SaveLayoutToStream(stream);
			result.ViewLayout = Convert.ToBase64String(stream.GetBuffer());
			return result;
		}
		public string FindDocumentSerializedControl(BaseDocument document) {
			DocumentControlDescription docControlDescription;
			if(documentToSerializedControlMap.TryGetValue(document, out docControlDescription)) {
				return docControlDescription.SerializedControl;
			}
			return null;
		}
		public void Dispose() {
			if(documentToSerializedControlMap != null) {
				documentToSerializedControlMap.Clear();
				documentToSerializedControlMap = null;
			}
		}
#if DebugTest
		public void DebugTest_AddDocumentControlDescription(BaseDocument doc, DocumentControlDescription documentControlDescription) {
			documentToSerializedControlMap.Add(doc, documentControlDescription);
		}
		public bool DebugTest_TryGetDocumentControlDescription(BaseDocument doc, out DocumentControlDescription documentControlDescription) {
			return documentToSerializedControlMap.TryGetValue(doc, out documentControlDescription);
		}
#endif
	}
	public delegate Control DeserializeDocumentControlDelegate(string serializedControl);
	public delegate bool CanDeserializeDocumentControlDelegate(DocumentControlDescription description);
	public delegate DocumentControlDescription SerializeDocumentControlDelegate(Control control);
	public class CustomSaveViewLayoutEventArgs : HandledEventArgs {
		public CustomSaveViewLayoutEventArgs(BaseView view) {
			this.View = view;
		}
		public BaseView View {
			get;
			private set;
		}
	}
	public class CustomRestoreViewLayoutEventArgs : HandledEventArgs {
		public CustomRestoreViewLayoutEventArgs(BaseView view) {
			DocumentDescriptions = new List<DocumentDescription>();
			this.View = view;
		}
		public List<DocumentDescription> DocumentDescriptions { get; private set; }
		public string DocumentLayout { get; set; }
		public BaseView View { get; private set; }
	}
	public class DocumentControlDescription {
		public DocumentControlDescription() { }
		public DocumentControlDescription(string serializedControl, string imageName) {
			this.SerializedControl = serializedControl;
			this.ImageName = imageName;
		}
		public string SerializedControl { get; set; }
		public string ImageName { get; set; }
	}
	[Serializable]
	public class DocumentDescription : DocumentControlDescription {
		public string Caption { get; set; }
		public string ControlName { get; set; }
	}
	[Serializable]
	public class DocumentManagerState {
		public DocumentDescription[] DocumentDescriptions { get; set; }
		public string ViewLayout { get; set; }
	}
}
