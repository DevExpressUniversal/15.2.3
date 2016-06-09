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
	public interface IDocumentGroupProperties : IContentContainerProperties {
	}
	public interface IDocumentGroupDefaultProperties : IContentContainerDefaultProperties {
	}
	[ToolboxItem(false), DesignTimeVisible(false)]
	[ProvideProperty("Length", "DevExpress.XtraBars.Docking2010.Views.WindowsUI.Document")]
	public abstract class DocumentGroup : BaseContentContainer, IExtenderProvider {
		DocumentCollection itemsCore;
		protected DocumentGroup(IContainer container)
			: base(container) {
		}
		protected DocumentGroup(IDocumentGroupProperties defaultProperties)
			: base(defaultProperties) {
		}
		protected override void OnCreate() {
			lengths = new Dictionary<Document, int>();
			itemsCore = CreateItems();
			Items.CollectionChanged += OnItemsCollectionChanged;
			base.OnCreate();
		}
		protected override void LockComponentBeforeDisposing() {
			base.LockComponentBeforeDisposing();
			Items.CollectionChanged -= OnItemsCollectionChanged;
		}
		protected override void OnDispose() {
			Ref.Dispose(ref itemsCore);
			Ref.Clear(ref lengths);
			base.OnDispose();
		}
		protected override int Count {
			get { return Items.Count; }
		}
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public DocumentCollection Items {
			get { return itemsCore; }
		}
		protected sealed override Document[] GetDocumentsCore() {
			return Items.ToArray();
		}
		IDictionary<Document, int> lengths;
		protected internal int this[Document document] {
			get { return GetLengthCore(document); }
			set { SetLengthCore(document, value); }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		protected internal int this[int index] {
			get { return GetLengthCore(GetDocument(index)); }
			set { SetLengthCore(GetDocument(index), value); }
		}
		protected Document GetDocument(int index) {
			return (index >= 0 && index < Items.Count) ? Items[index] : null;
		}
		protected int GetLengthCore(Document document) {
			int length;
			return (document != null) && lengths.TryGetValue(document, out length) ? length : 0;
		}
		protected void SetLengthCore(Document document, int value) {
			if(document != null) {
				int length;
				if(lengths.TryGetValue(document, out length))
					lengths[document] = value;
				else {
					if(IsInitializing)
						lengths.Add(document, value);
				}
			}
		}
		public int[] GetLengths() {
			int[] result = new int[Items.Count];
			for(int i = 0; i < result.Length; i++) {
				lengths.TryGetValue(Items[i], out result[i]);
			}
			return result;
		}
		protected virtual DocumentCollection CreateItems() {
			return new DocumentCollection(this);
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public new IDocumentGroupInfo Info {
			get { return base.Info as IDocumentGroupInfo; }
		}
		protected override void OnActivated() {
			using(BatchUpdate.Enter(Manager, true)) {
				foreach(Document document in Items)
					Info.Register(document);
			}
		}
		protected override void OnDeactivated() {
			base.OnDeactivated();
			using(BatchUpdate.Enter(Manager, true)) {
				foreach(Document document in Items)
					Info.Unregister(document);
			}
		}
		protected override void ReleaseDeferredControlLoadDocuments() {
			var deferredControlLoadDocuments = Items.Find((item) => item.IsDeferredControlLoad);
			using(BatchUpdate.Enter(Manager, true)) {
				foreach(Document document in deferredControlLoadDocuments)
					document.ReleaseDeferredLoadControl(Info.Owner);
			}
		}
		protected override void NotifyNavigatedTo() {
			foreach(Document document in Items)
				NotifyNavigatedTo(document);
		}
		protected override void NotifyNavigatedFrom() {
			foreach(Document document in Items)
				NotifyNavigatedFrom(document);
		}
		protected override void PatchChildrenCore(Rectangle view, bool active) {
			if(Info == null) return;
			var layoutDocuments = GetLayoutDocuments();
			foreach(Document document in layoutDocuments) {
				if(!document.IsControlLoaded) continue;
				IDocumentInfo dockInfo;
				if(!Info.TryGetValue(document, out dockInfo) || !IsVisibleChild(document)) {
					Control child = Manager.GetChild(document);
					child.Location = new Point(view.X - child.Width, view.Y - child.Height);
					Manager.LayeredWindowsNotifyHidden(child);
				}
				else dockInfo.PatchChild(view, active);
			}
		}
		protected virtual bool IsVisibleChild(Document document) {
			return true;
		}
		void OnItemsCollectionChanged(CollectionChangedEventArgs<Document> ea) {
			if(ea.ChangedType == CollectionChangedType.ElementAdded) {
				if(!lengths.ContainsKey(ea.Element))
					lengths.Add(ea.Element, 0);
				if(IsActive)
					Info.Register(ea.Element);
				OnAddComplete(ea.Element);
			}
			if(ea.ChangedType == CollectionChangedType.ElementDisposed) {
				lengths.Remove(ea.Element);
				if(IsActive)
					Info.Unregister(ea.Element);
				CheckDestroyAutomatically();
			}
			if(ea.ChangedType == CollectionChangedType.ElementRemoved) {
				lengths.Remove(ea.Element);
				if(IsActive)
					Info.Unregister(ea.Element);
				if(ea.Clear || !CheckDestroyAutomatically())
					OnRemoveComplete(ea.Element);
			}
			LayoutChanged();
		}
		protected virtual void OnAddComplete(Document document) { }
		protected virtual void OnRemoveComplete(Document document) { }
		protected virtual internal void OnInsert(int index) { }
		protected override bool ContainsCore(Document document) {
			return Items.Contains(document);
		}
		protected override void GetActualActionsCore(IList<IContentContainerAction> actions) {
			base.GetActualActionsCore(actions);
			if(GetZoomLevel() == ContextualZoomLevel.Normal) {
				foreach(Document document in Items)
					actions.Add(GetDetailAction(document));
			}
		}
		protected IContentContainerAction GetDetailAction(Document document) {
			return ContentContainerAction.CreateDetailAction(document);
		}
		protected override IDocumentContainer CreateDetailContainer(Document document) {
			return new DocumentGroupDetailContainer(document, this);
		}
		internal class DocumentGroupDetailContainer : DetailContainer {
			public DocumentGroupDetailContainer(Document document, DocumentGroup group)
				: base(document, group) {
			}
			protected override void GetActualActionsCore(IList<IContentContainerAction> actions) {
				base.GetActualActionsCore(actions);
				Document[] documents = ((DocumentGroup)Parent).Items.ToArray();
				for(int i = 0; i < documents.Length; i++) {
					if(documents[i] == Document) continue;
					actions.Add(((DocumentGroup)Parent).GetDetailAction(documents[i]));
				}
			}
		}
		protected virtual bool CanExtendCore(object extendee) {
			return IsDisposing ? false : extendee is Document && Items.Contains((Document)extendee);
		}
		#region IExtenderProvider Members
		bool IExtenderProvider.CanExtend(object extendee) {
			return CanExtendCore(extendee);
		}
		[Category("Layout"), DefaultValue(0)]
		public int GetLength(Document document) {
			return GetLengthCore(document);
		}
		[Category("Layout")]
		public void SetLength(Document document, int value) {
			SetLengthCore(document, value);
			LayoutChanged();
		}
		#endregion
		internal bool CanHeaderClick(Document document) {
			return RaiseHeaderClick(document);
		}
		bool RaiseHeaderClick(Document document) {
			DocumentHeaderClickEventArgs e = new DocumentHeaderClickEventArgs(document);
			WindowsUIView view = this.GetView();
			if(view != null)
				view.RaiseContentContainerHeaderClick(this, e);
			if(HeaderClick != null)
				HeaderClick(this, e);
			return !e.Handled;
		}
		public event DocumentHeaderClickEventHandler HeaderClick;
		protected override IEnumerable<Customization.ISearchObject> GetSearchObjectList() { return Items; }
	}
	public abstract class DocumentGroupProperties : ContentContainerProperties,
		IDocumentGroupProperties {
	}
	public abstract class DocumentGroupDefaultProperties : ContentContainerDefaultProperties,
		IDocumentGroupDefaultProperties {
		public DocumentGroupDefaultProperties(IDocumentGroupProperties parentProperties)
			: base(parentProperties) {
		}
	}
}
