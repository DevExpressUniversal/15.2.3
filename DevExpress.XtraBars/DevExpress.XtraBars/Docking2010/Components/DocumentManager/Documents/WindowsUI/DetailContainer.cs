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

using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.Utils.Serializing;
using DevExpress.XtraBars.Docking2010.Base;
namespace DevExpress.XtraBars.Docking2010.Views.WindowsUI {
	class DetailContainer : BaseContentContainer, IDocumentContainer {
		Document documentCore;
		public DetailContainer(Document document, IContentContainer parent)
			: base((IContentContainerProperties)null) {
			((IContentContainerInternal)this).SetIsAutoCreated(true);
			Parent = parent;
			this.documentCore = document;
			Document.Disposed += OnDocumentDisposed;
		}
		protected override void LockComponentBeforeDisposing() {
			Document.Disposed -= OnDocumentDisposed;
			base.LockComponentBeforeDisposing();
		}
		protected override void OnDispose() {
			base.OnDispose();
			this.documentCore = null;
		}
		void OnDocumentDisposed(object sender, System.EventArgs e) {
			Dispose();
		}
		public override string Caption {
			get { return Document.Caption; }
			set { }
		}
		public override Image Image {
			get { return Document.Image ?? Resources.ContentContainterActionResourceLoader.GetImage(DocumentManagerStringId.CommandDetail); }
			set { }
		}
		protected override bool CanUpdateActionsOnActivation() {
			return (Info != null);
		}
		protected override void OnActivated() {
			((IContentContainerInternal)this).SetManager(Parent.Manager);
			ActivateDocumentInView(Document);
		}
		protected override void OnDeactivated() {
			base.OnDeactivated();
			Dispose();
		}
		protected sealed override ContextualZoomLevel GetZoomLevel() {
			return ContextualZoomLevel.Detail;
		}
		public Document Document {
			get { return documentCore; }
		}
		protected override IContentContainerInfo CreateContentContainerInfo(WindowsUIView view) {
			return new DetailContainerInfo(view, this);
		}
		protected override IContentContainerDefaultProperties CreateDefaultProperties(IContentContainerProperties parentProperties) {
			return new DetailContainerDefaultProperties(parentProperties);
		}
		protected sealed override bool ContainsCore(Document document) {
			return Document == document;
		}
		protected sealed override int Count {
			get { return 1; }
		}
		protected sealed override Document[] GetDocumentsCore() {
			return new Document[] { Document };
		}
		protected sealed override DevExpress.Utils.Base.IBaseProperties GetParentProperties(WindowsUIView view) {
			return null;
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		protected new IDocumentContainerInfo Info {
			get { return base.Info as IDocumentContainerInfo; }
		}
		protected sealed override void EnsureDeferredControlLoadDocuments() {
			Document.EnsureIsBoundToControl(Info.Owner);
		}
		protected sealed override void ReleaseDeferredControlLoadDocuments() {
			Document.ReleaseDeferredLoadControl(Info.Owner);
		}
		public sealed override void UpdateDocumentActions() {
			UpdateDocumentActions(Document);
		}
		protected sealed override void NotifyNavigatedTo() {
			NotifyNavigatedTo(Document);
		}
		protected sealed override void NotifyNavigatedFrom() {
			NotifyNavigatedFrom(Document);
		}
		protected sealed override void PatchChildrenCore(Rectangle view, bool active) {
			var layoutDocuments = GetLayoutDocuments();
			foreach(BaseDocument document in layoutDocuments) {
				if(document == Document)
					Info.DocumentInfo.PatchChild(view, active);
				else {
					if(document.IsControlLoaded) {
						Control child = Manager.GetChild(document);
						child.Location = new Point(view.X - child.Width, view.Y - child.Height);
					}
				}
			}
		}
		protected override void GetActualActionsCore(IList<IContentContainerAction> actions) {
			base.GetActualActionsCore(actions);
			if(Document != null) {
				foreach(IContentContainerAction action in Document.DocumentContainerActions)
					actions.Add(action);
			}
		}
	}
	public class DetailContainerProperties : ContentContainerProperties {
		protected override DevExpress.Utils.Base.IBaseProperties CloneCore() {
			return new DetailContainerProperties();
		}
	}
	public class DetailContainerDefaultProperties : ContentContainerDefaultProperties {
		public DetailContainerDefaultProperties(IContentContainerProperties parentProperties)
			: base(parentProperties) {
		}
		protected override DevExpress.Utils.Base.IBaseProperties CloneCore() {
			return new DetailContainerDefaultProperties(ParentProperties as IContentContainerProperties);
		}
	}
}
