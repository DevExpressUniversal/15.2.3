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
using DevExpress.XtraBars.Docking2010.Base;
namespace DevExpress.XtraBars.Docking2010.Views.WindowsUI {
	public interface IPageProperties : IContentContainerProperties { }
	public interface IPageDefaultProperties : IContentContainerDefaultProperties { }
	[ToolboxItem(false), DesignTimeVisible(false)]
	public class Page : BaseContentContainer, IDocumentContainer {
		public Page()
			: base((IContainer)null) {
		}
		public Page(IContainer container)
			: base(container) {
		}
		public Page(IPageProperties defaultProperties)
			: base(defaultProperties) {
		}
		protected override void LockComponentBeforeDisposing() {
			UnsubscribeDocument(Document);
			base.LockComponentBeforeDisposing();
		}
		protected override void OnDispose() {
			base.OnDispose();
			documentCore = null;
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("PageProperties"),
#endif
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new IPageDefaultProperties Properties {
			get { return base.Properties as IPageDefaultProperties; }
		}
		protected override IContentContainerDefaultProperties CreateDefaultProperties(IContentContainerProperties parentProperties) {
			return new PageDefaultProperties(parentProperties as IPageProperties);
		}
		protected sealed override int Count {
			get { return (Document != null) ? 1 : 0; }
		}
		Document documentCore;
#if !SL
	[DevExpressXtraBarsLocalizedDescription("PageDocument")]
#endif
		public Document Document {
			get { return documentCore; }
			set { SetValue(ref documentCore, value, OnDocumentChanged); }
		}
		void OnDocumentChanged(Document oldValue, Document value) {
			UnsubscribeDocument(oldValue);
			SubscribeDocument(value);
			LayoutChanged();
		}
		void SubscribeDocument(Document document) {
			if(document != null)
				document.Disposed += OnDocumentDisposed;
		}
		void UnsubscribeDocument(Document document) {
			if(document != null)
				document.Disposed -= OnDocumentDisposed;
		}
		void OnDocumentDisposed(object sender, System.EventArgs e) {
			UnsubscribeDocument(Document);
			documentCore = null;
		}
		protected sealed override Document[] GetDocumentsCore() {
			return Document == null ? new Document[0] : new Document[] { Document };
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		protected new IPageInfo Info {
			get { return base.Info as IPageInfo; }
		}
		protected sealed override bool ContainsCore(Document document) {
			return Document == document;
		}
		protected sealed override void EnsureDeferredControlLoadDocuments() {
			if(Document != null) Document.EnsureIsBoundToControl(Info.Owner);
		}
		protected sealed override void ReleaseDeferredControlLoadDocuments() {
			if(Document != null) Document.ReleaseDeferredLoadControl(Info.Owner);
		}
		public sealed override void UpdateDocumentActions() {
			if(Document != null) UpdateDocumentActions(Document);
		}
		protected sealed override void NotifyNavigatedTo() {
			if(Document != null) NotifyNavigatedTo(Document);
		}
		protected sealed override void NotifyNavigatedFrom() {
			if(Document != null) NotifyNavigatedFrom(Document);
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
		protected sealed override DevExpress.Utils.Base.IBaseProperties GetParentProperties(WindowsUIView view) {
			return view.PageProperties;
		}
		protected override IContentContainerInfo CreateContentContainerInfo(WindowsUIView view) {
			return new PageInfo(view, this);
		}
		protected sealed override void GetActualActionsCore(IList<IContentContainerAction> actions) {
			base.GetActualActionsCore(actions);
			if(Document != null) {
				foreach(IContentContainerAction action in Document.DocumentContainerActions)
					actions.Add(action);
			}
		}
		protected override void OnActivated() {
			ActivateDocumentInView(Document);
		}
		protected override IEnumerable<Customization.ISearchObject> GetSearchObjectList() {
			List < Customization.ISearchObject> list = new List<Customization.ISearchObject>();
			list.Add(documentCore);
			return list;
		}
	}
	public class PageProperties : ContentContainerProperties, IPageProperties {
		protected override DevExpress.Utils.Base.IBaseProperties CloneCore() {
			return new PageProperties();
		}
	}
	public class PageDefaultProperties : ContentContainerDefaultProperties, IPageDefaultProperties {
		public PageDefaultProperties(IPageProperties parentProperties)
			: base(parentProperties) {
		}
		protected override DevExpress.Utils.Base.IBaseProperties CloneCore() {
			return new PageDefaultProperties(ParentProperties as IPageProperties);
		}
	}
}
