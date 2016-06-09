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

using System.ComponentModel;
using System.Drawing;
using DevExpress.Utils;
using DevExpress.XtraEditors;
namespace DevExpress.XtraBars.Docking2010.Views.WindowsUI {
	[ToolboxItem(false)]
	public class Document : BaseDocument, Customization.ISearchObject {
		ContentContainerActionCollection documentContainerActionsCore;
		public Document() { }
		public Document(IContainer container)
			: base(container) {
		}
		public Document(IDocumentProperties defaultProperties)
			: base(defaultProperties) {
		}
		protected override void OnCreate() {
			base.OnCreate();
			documentContainerActionsCore = CreateActions();
			DocumentContainerActions.CollectionChanged += OnActionsCollectionChanged;
		}
		protected override void LockComponentBeforeDisposing() {
			base.LockComponentBeforeDisposing();
			DocumentContainerActions.CollectionChanged -= OnActionsCollectionChanged;
		}
		protected override void OnDispose() {
			Ref.Dispose(ref documentContainerActionsCore);
			base.OnDispose();
		}
		Image actionImageCore;
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("DocumentActionImage"),
#endif
		DefaultValue(null), Category(CategoryName.Appearance), Editor("DevExpress.Utils.Design.DXImageEditor, " + AssemblyInfo.SRAssemblyDesign, typeof(System.Drawing.Design.UITypeEditor))]
		public Image ActionImage {
			get { return actionImageCore; }
			set { SetValue(ref actionImageCore, value); }
		}
		DxImageUri actionImageUriCore = new DxImageUri();
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("DocumentActionImageUri"),
#endif
Category("Appearance"), DefaultValue(null)]
		[TypeConverter(typeof(ExpandableObjectConverter)), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public DxImageUri ActionImageUri {
			get { return actionImageUriCore; }
			set { SetValue(ref actionImageUriCore, value); }
		}
		string actionCaptionCore;
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("DocumentActionCaption"),
#endif
		DefaultValue(null), Category(CategoryName.Appearance), Localizable(true)]
		public string ActionCaption {
			get { return actionCaptionCore; }
			set { SetValue(ref actionCaptionCore, value); }
		}
		[Browsable(false)]
		public ContentContainerActionCollection DocumentContainerActions {
			get { return documentContainerActionsCore; }
		}
		protected virtual ContentContainerActionCollection CreateActions() {
			return new ContentContainerActionCollection(this);
		}
		protected internal virtual Image GetActualActionImage() {
			if(ActionImageUri != null && ActionImageUri.HasImage)
				return ActionImageUri.GetImage();
			return ActionImage;
		}
		void OnActionsCollectionChanged(Base.CollectionChangedEventArgs<IContentContainerAction> ea) {
			LayoutChanged();
		}
		#region ISearchObject Members
		string[] searchTagsCore;
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("DocumentSearchTags"),
#endif
		Category("Search Properties"), DefaultValue(null), Localizable(true)]
		public string[] SearchTags {
			get { return searchTagsCore; }
			set {
				if(searchTagsCore == value) return;
				searchTagsCore = value;
			}
		}
		string Customization.ISearchObject.SearchText { get { return GetSearchText(); } }
		string Customization.ISearchObject.SearchTag { get { return GetSearchTag(); } }
		protected virtual string GetSearchText() { return string.IsNullOrEmpty(this.Caption) ? this.GetName() : this.Caption; }
		protected virtual string GetSearchTag() {
			if(SearchTags == null || SearchTags.Length <= 0) return GetSearchText();
			string searchTags = string.Empty;
			foreach(string tag in SearchTags) {
				if(string.IsNullOrEmpty(tag)) continue;
				searchTags += tag;
			}
			return searchTags;
		}
		bool excludeFromSearchCore;
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("DocumentExcludeFromSearch"),
#endif
 Category("Search Properties"), DefaultValue(false)]
		public bool ExcludeFromSearch {
			get { return excludeFromSearchCore; }
			set {
				if(ExcludeFromSearch == value) return;
				excludeFromSearchCore = value;
			}
		}
		bool Customization.ISearchObject.EnabledInSearch { get { return EnabledInSearch; } }
		protected virtual bool EnabledInSearch { get { return this.IsVisible && !this.IsDisposing && !ExcludeFromSearch; } }
		#endregion
	}
	public class DocumentCollection : BaseDocumentCollection<Document, DocumentGroup> {
		public DocumentCollection(DocumentGroup owner)
			: base(owner) {
		}
		protected override bool CanAdd(Document element) {
			return !Owner.IsFilledUp && base.CanAdd(element);
		}
		protected override void NotifyOwnerOnInsert(int index) {
			Owner.OnInsert(index);
		}
	}
}
