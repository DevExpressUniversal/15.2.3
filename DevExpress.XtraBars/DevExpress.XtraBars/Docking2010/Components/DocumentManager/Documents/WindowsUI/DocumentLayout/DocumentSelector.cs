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
using DevExpress.Utils.Serializing;
namespace DevExpress.XtraBars.Docking2010.Views.WindowsUI {
	public interface IDocumentSelectorProperties : IDocumentGroupProperties {
		Customization.TransitionAnimation SwitchDocumentAnimationMode { get; set; }
		int SwitchDocumentAnimationFrameInterval { get; set; }
		int SwitchDocumentAnimationFramesCount { get; set; }
		bool AllowHtmlDrawHeaders { get; set; }
	}
	public interface IDocumentSelectorDefaultProperties : IDocumentGroupDefaultProperties {
		Customization.TransitionAnimation? SwitchDocumentAnimationMode { get; set; }
		int? SwitchDocumentAnimationFrameInterval { get; set; }
		int? SwitchDocumentAnimationFramesCount { get; set; }
		DevExpress.Utils.DefaultBoolean AllowHtmlDrawHeaders { get; set; }
		bool CanHtmlDrawHeaders { get; }
		Customization.TransitionAnimation ActualSwitchDocumentAnimationMode { get; }
		int ActualSwitchDocumentAnimationFrameInterval { get; }
		int ActualSwitchDocumentAnimationFramesCount { get; }
	}
	[ToolboxItem(false), DesignTimeVisible(false)]
	public abstract class DocumentSelector : DocumentGroup, IDocumentSelector {
		protected DocumentSelector(IContainer container)
			: base(container) {
		}
		protected DocumentSelector(IDocumentSelectorProperties defaultProperties)
			: base(defaultProperties) {
		}
		protected override void ActivateDocumentCore(Document document) {
			SetSelected(document, false);
		}
		protected override void OnCreate() {
			base.OnCreate();
			appearanceHeaderButtonCore = new DevExpress.XtraEditors.ButtonsPanelControl.ButtonsPanelControlAppearance(this);
			AppearanceHeaderButton.Changed += OnAppearanceHeaderButtonChanged;
		}
		protected override void LockComponentBeforeDisposing() {
			AppearanceHeaderButton.Changed -= OnAppearanceHeaderButtonChanged;
			base.LockComponentBeforeDisposing();
		}
		protected override void OnDispose() {
			Ref.Dispose(ref appearanceHeaderButtonCore);
			base.OnDispose();
		}
		object headerButtonBackgroundImagesCore;
		[
		DefaultValue(null), Category("Headers"), TypeConverter(typeof(DevExpress.Utils.Design.ImageCollectionImagesConverter))]
		public virtual object HeaderButtonBackgroundImages {
			get { return headerButtonBackgroundImagesCore; }
			set {
				if(HeaderButtonBackgroundImages == value) return;
				headerButtonBackgroundImagesCore = value;
				ColoredElementsCache.Reset();
				if(Manager != null)
					Manager.LayoutChanged();
			}
		}
		DevExpress.XtraEditors.ButtonsPanelControl.ButtonsPanelControlAppearance appearanceHeaderButtonCore;
		[
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), Category("Headers")]
		public virtual DevExpress.XtraEditors.ButtonsPanelControl.ButtonsPanelControlAppearance AppearanceHeaderButton {
			get { return appearanceHeaderButtonCore; }
		}
		void ResetAppearanceHeaderButton() { AppearanceHeaderButton.Reset(); }
		bool ShouldSerializeAppearanceHeaderButton() { return !IsDisposing && AppearanceHeaderButton.ShouldSerialize(); }
		void OnAppearanceHeaderButtonChanged(object sender, System.EventArgs e) {
			LayoutChanged();
		}
		[Browsable(false)]
		public new IDocumentSelectorDefaultProperties Properties {
			get { return base.Properties as IDocumentSelectorDefaultProperties; }
		}
		int selectedItemIndexCore = -1;
		[Browsable(false)]
		public int SelectedItemIndex {
			get { return selectedItemIndexCore; }
		}
		[Browsable(false)]
		public Document SelectedDocument {
			get { return GetDocument(SelectedItemIndex); }
		}
		public bool IsSelected(Document document) {
			return document == GetDocument(SelectedItemIndex);
		}
		public void SetSelected(Document document) {
			SetSelected(document, true);
		}
		public void SetSelected(Document document, bool showAnimation) {
			int index = Items.IndexOf(document);
			if(selectedItemIndexCore == index) return;
			if(showAnimation)
				StartSwitchDocumentAnimation(document);
			selectedItemIndexCore = index;
			ActivateDocumentInView(document);
			RequestInvokePatchActiveChildren();
			RaiseSelectionChanged(document);
		}
		void RequestInvokePatchActiveChildren() {
			BaseView view = (Info != null) ? Info.Owner : null;
			if(view != null)
				view.RequestInvokePatchActiveChild();
		}
		protected override void OnActivated() {
			base.OnActivated();
			ActivateDocumentInView(SelectedDocument);
		}
		void RaiseSelectionChanged(Document document) {
			if(SelectionChanged != null)
				SelectionChanged(this, new DocumentEventArgs(document));
		}
		public event DocumentEventHandler SelectionChanged;
		protected void StartSwitchDocumentAnimation(Document document) {
			if(Info == null || Info.Owner == null || Info.Owner.IsInitializing) return;
			if(Properties.ActualSwitchDocumentAnimationMode == Customization.TransitionAnimation.None) return;
			((WindowsUIView)Info.Owner).StartTransitionAnimation(
				SelectedDocument, document, CreateAnimationParameters(document));
		}
		Customization.AnimationParameters CreateAnimationParameters(Document document) {
			return new Customization.AnimationParameters(
										Properties.ActualSwitchDocumentAnimationMode,
										Properties.ActualSwitchDocumentAnimationFrameInterval,
										Properties.ActualSwitchDocumentAnimationFramesCount,
										Items.IndexOf(document) > SelectedItemIndex);
		}
		protected internal override void OnInsert(int index) {
			if(index <= selectedItemIndexCore)
				selectedItemIndexCore++;
		}
		protected override void OnAddComplete(Document document) {
			if(IsInitializing) { 
				if(documentToActivate == null)
					documentToActivate = document;
				return;
			}
			if(selectedItemIndexCore == -1)
				SetSelected(document);
		}
		Document documentToActivate;
		protected override void OnInitialized() {
			base.OnInitialized();
			SetSelected(documentToActivate);
			documentToActivate = null;
		}
		protected override void OnRemoveComplete(Document document) {
			CheckSelectedIndex();
		}
		void CheckSelectedIndex() {
			if(selectedItemIndexCore == Items.Count) {
				selectedItemIndexCore--;
			}
		}
		protected override void EnsureDeferredControlLoadDocuments() {
			if(SelectedDocument != null)
				SelectedDocument.EnsureIsBoundToControl(Info.Owner);
		}
		public sealed override void UpdateDocumentActions() {
			if(SelectedDocument != null)
				UpdateDocumentActions(SelectedDocument);
		}
		protected sealed override void NotifyNavigatedTo() {
			if(SelectedDocument != null)
				NotifyNavigatedTo(SelectedDocument);
		}
		protected sealed override void NotifyNavigatedFrom() {
			if(SelectedDocument != null)
				NotifyNavigatedFrom(SelectedDocument);
		}
		protected override bool IsVisibleChild(Document document) {
			return document == SelectedDocument;
		}
		protected override bool CanExtendCore(object extendee) {
			return false;
		}
		protected override DocumentCollection CreateItems() {
			return new DocumentSelectorDocumentCollection(this);
		}
		protected override void GetActualActionsCore(System.Collections.Generic.IList<IContentContainerAction> actions) {
			base.GetActualActionsCore(actions);
			if(SelectedDocument != null) {
				foreach(IContentContainerAction action in SelectedDocument.DocumentContainerActions)
					actions.Add(action);
			}
		}
		protected class DocumentSelectorDocumentCollection : DocumentCollection {
			public DocumentSelectorDocumentCollection(DocumentSelector owner)
				: base(owner) {
			}
			public new DocumentSelector Owner {
				get { return base.Owner as DocumentSelector; }
			}
			Document selectedDocument;
			protected override void OnBeforeElementRemoved(Document element) {
				base.OnBeforeElementRemoved(element);
				selectedDocument = (Owner != null) ? Owner.SelectedDocument : null;
			}
			protected override void OnElementRemoved(Document element) {
				if(element != selectedDocument && selectedDocument != null && Owner != null)
					Owner.SetSelected(selectedDocument, !element.IsDisposing);
				selectedDocument = null;
				base.OnElementRemoved(element);
			}
		}
	}
	public abstract class DocumentSelectorProperties : DocumentGroupProperties, IDocumentSelectorProperties {
		public DocumentSelectorProperties() {
			SetDefaultValueCore("SwitchDocumentAnimationFrameInterval", 10000);
			SetDefaultValueCore("SwitchDocumentAnimationFramesCount", 1000);
		}
		[DefaultValue(Customization.TransitionAnimation.HorizontalSlide), XtraSerializableProperty(XtraSerializationFlags.DefaultValue), Category("Behavior")]
		public Customization.TransitionAnimation SwitchDocumentAnimationMode {
			get { return GetValueCore<Customization.TransitionAnimation>("SwitchDocumentAnimationMode"); }
			set { SetValueCore("SwitchDocumentAnimationMode", value); }
		}
		[DefaultValue(10000), XtraSerializableProperty(XtraSerializationFlags.DefaultValue), Category("Behavior")]
		public int SwitchDocumentAnimationFrameInterval {
			get { return GetValueCore<int>("SwitchDocumentAnimationFrameInterval"); }
			set { SetValueCore("SwitchDocumentAnimationFrameInterval", value); }
		}
		[DefaultValue(1000), XtraSerializableProperty(XtraSerializationFlags.DefaultValue), Category("Behavior")]
		public int SwitchDocumentAnimationFramesCount {
			get { return GetValueCore<int>("SwitchDocumentAnimationFramesCount"); }
			set { SetValueCore("SwitchDocumentAnimationFramesCount", value); }
		}
		[DefaultValue(false), XtraSerializableProperty(XtraSerializationFlags.DefaultValue), Category("Appearance")]
		public bool AllowHtmlDrawHeaders {
			get { return GetValueCore<bool>("AllowHtmlDrawHeaders"); }
			set { SetValueCore("AllowHtmlDrawHeaders", value); }
		}
	}
	public abstract class DocumentSelectorDefaultProperties : DocumentGroupDefaultProperties, IDocumentSelectorDefaultProperties {
		public DocumentSelectorDefaultProperties(IDocumentSelectorProperties parentProperties)
			: base(parentProperties) {
			SetDefaultValueCore("AllowHtmlDrawHeaders", DevExpress.Utils.DefaultBoolean.Default);
			SetConverter("SwitchDocumentAnimationFrameInterval", GetNullableValueConverter(10000));
			SetConverter("SwitchDocumentAnimationFramesCount", GetNullableValueConverter(1000));
			SetConverter("AllowHtmlDrawHeaders", GetDefaultBooleanConverter(false));
		}
		[DefaultValue(null), XtraSerializableProperty(XtraSerializationFlags.DefaultValue), Category("Behavior")]
		[TypeConverter(typeof(DevExpress.Utils.Design.NullableTypeConverter))]
		public Customization.TransitionAnimation? SwitchDocumentAnimationMode {
			get { return GetValueCore<Customization.TransitionAnimation?>("SwitchDocumentAnimationMode"); }
			set { SetValueCore("SwitchDocumentAnimationMode", value); }
		}
		[DefaultValue(null), XtraSerializableProperty(XtraSerializationFlags.DefaultValue), Category("Behavior")]
		[TypeConverter(typeof(DevExpress.Utils.Design.NullableTypeConverter))]
		public int? SwitchDocumentAnimationFrameInterval {
			get { return GetValueCore<int?>("SwitchDocumentAnimationFrameInterval"); }
			set { SetValueCore("SwitchDocumentAnimationFrameInterval", value); }
		}
		[DefaultValue(null), XtraSerializableProperty(XtraSerializationFlags.DefaultValue), Category("Behavior")]
		[TypeConverter(typeof(DevExpress.Utils.Design.NullableTypeConverter))]
		public int? SwitchDocumentAnimationFramesCount {
			get { return GetValueCore<int?>("SwitchDocumentAnimationFramesCount"); }
			set { SetValueCore("SwitchDocumentAnimationFramesCount", value); }
		}
		[DefaultValue(DevExpress.Utils.DefaultBoolean.Default), XtraSerializableProperty(XtraSerializationFlags.DefaultValue), Category("Appearance")]
		public DevExpress.Utils.DefaultBoolean AllowHtmlDrawHeaders {
			get { return GetValueCore<DevExpress.Utils.DefaultBoolean>("AllowHtmlDrawHeaders"); }
			set { SetValueCore("AllowHtmlDrawHeaders", value); }
		}
		[Browsable(false)]
		public Customization.TransitionAnimation ActualSwitchDocumentAnimationMode {
			get { return GetActualValueFromNullable<Customization.TransitionAnimation>("SwitchDocumentAnimationMode"); }
		}
		[Browsable(false)]
		public int ActualSwitchDocumentAnimationFrameInterval {
			get { return GetActualValueFromNullable<int>("SwitchDocumentAnimationFrameInterval"); }
		}
		[Browsable(false)]
		public int ActualSwitchDocumentAnimationFramesCount {
			get { return GetActualValueFromNullable<int>("SwitchDocumentAnimationFramesCount"); }
		}
		[Browsable(false)]
		public bool CanHtmlDrawHeaders {
			get { return GetActualValue<DevExpress.Utils.DefaultBoolean, bool>("AllowHtmlDrawHeaders"); }
		}
	}
}
