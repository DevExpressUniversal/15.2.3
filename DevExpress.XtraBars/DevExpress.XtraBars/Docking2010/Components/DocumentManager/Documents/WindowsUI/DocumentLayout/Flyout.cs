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
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.Utils;
using DevExpress.Utils.Serializing;
using DevExpress.Utils.Base;
namespace DevExpress.XtraBars.Docking2010.Views.WindowsUI {
	public enum FlyoutStyle { Popup, MessageBox }
	public interface IFlyoutProperties : IContentContainerProperties {
		ContentAlignment Alignment { get; set; }
		bool AllowHtmlDraw { get; set; }
		AppearanceObject Appearance { get; }
		AppearanceObject AppearanceButtons { get; }
		AppearanceObject AppearanceCaption { get; }
		AppearanceObject AppearanceDescription { get; }
		FlyoutStyle Style { get; set; }
		Size ButtonSize { get; set; }
	}
	public interface IFlyoutDefaultProperties : IContentContainerDefaultProperties {
		ContentAlignment Alignment { get; set; }
		DefaultBoolean AllowHtmlDraw { get; set; }
		AppearanceObject Appearance { get; }
		AppearanceObject AppearanceButtons { get; }
		AppearanceObject AppearanceCaption { get; }
		AppearanceObject AppearanceDescription { get; }
		FlyoutStyle Style { get; set; }
		Size ButtonSize { get; set; }
		[Browsable(false)]
		ContentAlignment ActualAlignment { get; }
		[Browsable(false)]
		bool CanHtmlDraw { get; }
		[Browsable(false)]
		AppearanceObject ActualAppearance { get; }
		[Browsable(false)]
		AppearanceObject ActualAppearanceButtons { get; }
		[Browsable(false)]
		AppearanceObject ActualAppearanceCaption { get; }
		[Browsable(false)]
		AppearanceObject ActualAppearanceDescription { get; }
		[Browsable(false)]
		FlyoutStyle ActualStyle { get; }
		[Browsable(false)]
		Size ActualButtonSize { get; }
	}
	public interface IFlyoutContainer : IDocumentContainer {
		IFlyoutDefaultProperties FlyoutProperties { get; }
	}
	[DesignTimeVisible(false)]
	[ToolboxItem(false)]
	public class Flyout : BaseContentContainer, IFlyoutContainer {
		Document documentCore;
		FlyoutAction flyoutActionCore;
		MessageBoxButtons? flyoutButtonsCore;
		public Flyout()
			: base((IContainer)null) {
		}
		public Flyout(IContainer container)
			: base(container) {
		}
		public Flyout(IFlyoutProperties defaultProperties)
			: base(defaultProperties) {
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("FlyoutDocument"),
#endif
 DefaultValue(null), Category("Layout")]
		public Document Document {
			get { return documentCore; }
			set { SetValue<Document>(ref documentCore, value); }
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("FlyoutAction"),
#endif
 DefaultValue(null), Browsable(false)]
		public FlyoutAction Action {
			get { return flyoutActionCore; }
			set { SetValue<FlyoutAction>(ref flyoutActionCore, value); }
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("FlyoutFlyoutButtons"),
#endif
 DefaultValue(null), Category("Layout")]
		public MessageBoxButtons? FlyoutButtons {
			get { return flyoutButtonsCore; }
			set { SetValue<MessageBoxButtons?>(ref flyoutButtonsCore, value); }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		protected new IFlyoutInfo Info {
			get { return base.Info as IFlyoutInfo; }
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
#if !SL
	[DevExpressXtraBarsLocalizedDescription("FlyoutProperties")]
#endif
		public new IFlyoutDefaultProperties Properties {
			get { return base.Properties as IFlyoutDefaultProperties; }
		}
		bool ShouldSerializeProperties() {
			return ((Properties != null) && Properties.ShouldSerialize());
		}
		void ResetFlyoutProperties() {
			Properties.Reset();
		}
		protected sealed override int Count {
			get { return ((Document != null) ? 1 : 0); }
		}
		[Browsable(false)]
		public DialogResult LastResult { get; private set; }
		protected override void OnBeforeActivate(WindowsUIView view) {
			LastResult = DialogResult.None;
		}
		protected override void OnBeforeDeactivate(WindowsUIView view) {
			LastResult = view.GetFlyoutAdornerResult();
		}
		protected override void OnDocumentActionsLoaded(IDocumentActionsArgs actionsArgs) {
			LoadedAction = actionsArgs.FlyoutAction;
		}
		internal FlyoutAction LoadedAction { get; set; }
		Document lastActiveDocument;
		protected override void OnActivated() {
			WindowsUIView view = GetView();
			if(view != null) {
				this.lastActiveDocument = GetContentContainerDocument(view);
				ActivateDocumentInView(Document);
			}
		}
		protected override void OnDeactivated() {
			WindowsUIView view = GetView();
			if(view != null) {
				view.flyoutDeactivation++;
				try {
					DeactivateDocumentInView(lastActiveDocument);
				}
				finally { view.flyoutDeactivation--; }
			}
			this.lastActiveDocument = null;
		}
		Document GetContentContainerDocument(WindowsUIView view) {
			if(view.ActiveContentContainer == null)
				return null;
			var documentContainer = view.ActiveContentContainer as IDocumentContainer;
			if(documentContainer != null)
				return documentContainer.Document;
			var documentSelector = view.ActiveContentContainer as IDocumentSelector;
			if(documentSelector != null)
				return documentSelector.SelectedDocument;
			var container = view.ActiveContentContainer as IContentContainerInternal;
			return view.ActiveDocument as Document ?? System.Linq.Enumerable.FirstOrDefault(container.GetDocuments());
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
		IFlyoutDefaultProperties Views.WindowsUI.IFlyoutContainer.FlyoutProperties {
			get { return Properties; }
		}
		protected sealed override bool ContainsCore(Document document) {
			return Document == document;
		}
		protected override IContentContainerInfo CreateContentContainerInfo(WindowsUIView view) {
			return new FlyoutInfo(view, this);
		}
		protected override IContentContainerDefaultProperties CreateDefaultProperties(IContentContainerProperties parentProperties) {
			return new FlyoutDefaultProperties(parentProperties as IFlyoutProperties); ;
		}
		protected sealed override void EnsureDeferredControlLoadDocuments() {
			if(Document != null) {
				Document.EnsureIsBoundToControl(Info.Owner);
			}
		}
		protected sealed override Document[] GetDocumentsCore() {
			return ((Document == null) ? new Document[0] : new Document[] { Document });
		}
		protected sealed override IBaseProperties GetParentProperties(WindowsUIView view) {
			return view.FlyoutProperties;
		}
		protected sealed override void PatchChildrenCore(Rectangle view, bool active) {
			if((Document != null) || (Action != null)) {
				Info.DocumentInfo.PatchChild(view, active);
				((WindowsUIView)Manager.View).ShowFlyoutAdorner();
			}
		}
		protected sealed override void ReleaseDeferredControlLoadDocuments() {
			if(Document != null && Info != null)
				Document.ReleaseDeferredLoadControl(Info.Owner);
			((WindowsUIView)Manager.View).HideFlyoutAdorner();
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override event ButtonEventHandler ButtonClick { add { } remove { } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override event ButtonEventHandler ButtonUnchecked { add { } remove { } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override event ButtonEventHandler ButtonChecked { add { } remove { } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override object ButtonBackgroundImages { get { return null; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override DevExpress.XtraEditors.ButtonsPanelControl.ButtonsPanelControlAppearance AppearanceButton { get { return base.AppearanceButton; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override ContentContainerButtonCollection Buttons { get { return base.Buttons; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override int ButtonInterval { get { return base.ButtonInterval; } set { } }
		protected override bool EnabledInSearch { get { return false; } }
	}
	public class FlyoutDefaultProperties : ContentContainerDefaultProperties, IFlyoutDefaultProperties {
		public FlyoutDefaultProperties(IFlyoutProperties parentProperties)
			: base(parentProperties) {
			InitContentPropertyCore("Appearance", new AppearanceObject(), (a) => a.Changed += OnAppearanceChanged);
			InitContentPropertyCore("AppearanceCaption", new AppearanceObject(), (a) => a.Changed += OnAppearanceCaptionChanged);
			InitContentPropertyCore("AppearanceDescription", new AppearanceObject(), (a) => a.Changed += OnAppearanceDescriptionChanged);
			InitContentPropertyCore("AppearanceButtons", new AppearanceObject(), (a) => a.Changed += OnAppearanceButtonsChanged);
			SetDefaultValueCore<bool>("AllowHtmlDraw", true);
			SetDefaultValueCore<FlyoutStyle>("Style", FlyoutStyle.MessageBox);
			SetDefaultValueCore<ContentAlignment>("Alignment", ContentAlignment.MiddleCenter);
			SetDefaultValueCore<DefaultBoolean>("AllowHtmlDraw", DefaultBoolean.Default);
			SetConverter<DefaultBoolean, bool>("AllowHtmlDraw", BaseDefaultProperties.GetDefaultBooleanConverter(true));
			SetConverter("DestroyOnRemovingChildren", GetDefaultBooleanConverter(false));
		}
		protected override IBaseProperties CloneCore() {
			return new FlyoutDefaultProperties(base.ParentProperties as IFlyoutProperties);
		}
		protected override void OnDispose() {
			Appearance.Changed -= OnAppearanceChanged;
			AppearanceCaption.Changed -= OnAppearanceCaptionChanged;
			AppearanceDescription.Changed -= OnAppearanceDescriptionChanged;
			AppearanceButtons.Changed -= OnAppearanceButtonsChanged;
			base.OnDispose();
		}
		void OnAppearanceButtonsChanged(object sender, EventArgs e) {
			OnObjectChanged("AppearanceButtons");
		}
		void OnAppearanceCaptionChanged(object sender, EventArgs e) {
			OnObjectChanged("AppearanceCaption");
		}
		void OnAppearanceChanged(object sender, EventArgs e) {
			OnObjectChanged("Appearance");
		}
		void OnAppearanceDescriptionChanged(object sender, EventArgs e) {
			base.OnObjectChanged("AppearanceDescription");
		}
		[XtraSerializableProperty(XtraSerializationFlags.DefaultValue), Category("Behavior"), DefaultValue(ContentAlignment.MiddleCenter)]
		public ContentAlignment Alignment {
			get { return GetValueCore<ContentAlignment>("Alignment"); }
			set { SetValueCore<ContentAlignment>("Alignment", value); }
		}
		[XtraSerializableProperty(XtraSerializationFlags.DefaultValue), Category("Appearance"), DefaultValue(DefaultBoolean.Default)]
		public DefaultBoolean AllowHtmlDraw {
			get { return GetValueCore<DefaultBoolean>("AllowHtmlDraw"); }
			set { SetValueCore<DefaultBoolean>("AllowHtmlDraw", value); }
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content), Category("Appearance")]
		public AppearanceObject Appearance {
			get { return GetContentCore<AppearanceObject>("Appearance"); }
		}
		bool ShouldSerializeAppearance() {
			return ((Appearance != null) && Appearance.ShouldSerialize());
		}
		void ResetAppearance() {
			Appearance.Reset();
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content), Category("Appearance")]
		public AppearanceObject AppearanceButtons {
			get { return GetContentCore<AppearanceObject>("AppearanceButtons"); }
		}
		bool ShouldSerializeAppearanceButtons() {
			return ((AppearanceButtons != null) && AppearanceButtons.ShouldSerialize());
		}
		void ResetAppearanceButtons() {
			AppearanceButtons.Reset();
		}
		[Category("Appearance"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceObject AppearanceCaption {
			get { return GetContentCore<AppearanceObject>("AppearanceCaption"); }
		}
		bool ShouldSerializeAppearanceCaption() {
			return ((AppearanceCaption != null) && AppearanceCaption.ShouldSerialize());
		}
		void ResetAppearanceCaption() {
			AppearanceCaption.Reset();
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content), Category("Appearance")]
		public AppearanceObject AppearanceDescription {
			get { return GetContentCore<AppearanceObject>("AppearanceDescription"); }
		}
		bool ShouldSerializeAppearanceDescription() {
			return ((AppearanceDescription != null) && AppearanceDescription.ShouldSerialize());
		}
		void ResetAppearanceDescription() {
			AppearanceDescription.Reset();
		}
		[Category("Behavior"), XtraSerializableProperty(XtraSerializationFlags.DefaultValue), DefaultValue(FlyoutStyle.MessageBox)]
		public FlyoutStyle Style {
			get { return GetValueCore<FlyoutStyle>("Style"); }
			set { SetValueCore<FlyoutStyle>("Style", value); }
		}
		[Category("Behavior"), XtraSerializableProperty(XtraSerializationFlags.DefaultValue), DefaultValue(typeof(Size), "0, 0")]
		public Size ButtonSize {
			get { return GetValueCore<Size>("ButtonSize"); }
			set { SetValueCore<Size>("ButtonSize", value); }
		}
		[Browsable(false)]
		public ContentAlignment ActualAlignment {
			get { return GetActualValue<ContentAlignment, ContentAlignment>("Alignment"); }
		}
		[Browsable(false)]
		public bool CanHtmlDraw {
			get { return base.GetActualValue<DefaultBoolean, bool>("AllowHtmlDraw"); }
		}
		[Browsable(false)]
		public AppearanceObject ActualAppearance {
			get { return GetActualAppearance(Appearance, "Appearance"); }
		}
		[Browsable(false)]
		public AppearanceObject ActualAppearanceButtons {
			get { return GetActualAppearance(AppearanceButtons, "AppearanceButtons"); }
		}
		[Browsable(false)]
		public AppearanceObject ActualAppearanceCaption {
			get { return GetActualAppearance(AppearanceCaption, "AppearanceCaption"); }
		}
		[Browsable(false)]
		public AppearanceObject ActualAppearanceDescription {
			get { return GetActualAppearance(AppearanceDescription, "AppearanceDescription"); }
		}
		[Browsable(false)]
		public FlyoutStyle ActualStyle {
			get { return GetActualValue<FlyoutStyle, FlyoutStyle>("Style"); }
		}
		[Browsable(false)]
		public Size ActualButtonSize {
			get { return GetActualValue<Size, Size>("ButtonSize"); }
		}
		protected AppearanceObject GetActualAppearance(AppearanceObject appearance, string propertyName) {
			FrozenAppearance result = new FrozenAppearance();
			if(ParentProperties != null)
				AppearanceHelper.Combine(result, new AppearanceObject[] { appearance, ParentProperties.GetContent<AppearanceObject>(propertyName) });
			return result;
		}
		protected override void AssignCore(IPropertiesProvider source) {
			base.AssignCore(source);
			IFlyoutDefaultProperties sourceProperties = source as IFlyoutDefaultProperties;
			if(source != null) {
				AllowHtmlDraw = sourceProperties.AllowHtmlDraw;
				Style = sourceProperties.Style;
				Alignment = sourceProperties.Alignment;
				Appearance.AssignInternal(sourceProperties.Appearance);
				AppearanceCaption.AssignInternal(sourceProperties.AppearanceCaption);
				AppearanceDescription.AssignInternal(sourceProperties.AppearanceDescription);
				AppearanceButtons.AssignInternal(sourceProperties.AppearanceButtons);
			}
		}
		protected override void ResetCore() {
			base.ResetCore();
			Appearance.Reset();
			AppearanceCaption.Reset();
			AppearanceDescription.Reset();
			AppearanceButtons.Reset();
		}
		protected override bool ShouldSerializeCore() {
			return base.ShouldSerializeCore() ||
				Appearance.ShouldSerialize() ||
				AppearanceCaption.ShouldSerialize() ||
				AppearanceDescription.ShouldSerialize() ||
				AppearanceButtons.ShouldSerialize();
		}
	}
	public class FlyoutProperties : ContentContainerProperties, IFlyoutProperties, IContentContainerProperties, IBaseProperties, IBaseObject, IDisposable, IPropertiesProvider, ISupportBatchUpdate {
		public FlyoutProperties() {
			InitContentPropertyCore("Appearance", new AppearanceObject(), (a) => a.Changed += OnAppearanceChanged);
			InitContentPropertyCore("AppearanceCaption", new AppearanceObject(), (a) => a.Changed += OnAppearanceCaptionChanged);
			InitContentPropertyCore("AppearanceDescription", new AppearanceObject(), (a) => a.Changed += OnAppearanceDescriptionChanged);
			InitContentPropertyCore("AppearanceButtons", new AppearanceObject(), (a) => a.Changed += OnAppearanceButtonsChanged);
			SetDefaultValueCore<bool>("AllowHtmlDraw", true);
			SetDefaultValueCore<FlyoutStyle>("Style", FlyoutStyle.MessageBox);
			SetDefaultValueCore<ContentAlignment>("Alignment", ContentAlignment.MiddleCenter);
			SetDefaultValueCore("DestroyOnRemovingChildren", false);
		}
		protected override IBaseProperties CloneCore() {
			return new FlyoutProperties();
		}
		protected override void OnDispose() {
			Appearance.Changed -= OnAppearanceChanged;
			AppearanceCaption.Changed -= OnAppearanceCaptionChanged;
			AppearanceDescription.Changed -= OnAppearanceDescriptionChanged;
			AppearanceButtons.Changed -= OnAppearanceButtonsChanged;
			base.OnDispose();
		}
		void OnAppearanceButtonsChanged(object sender, EventArgs e) {
			base.OnObjectChanged("AppearanceButtons");
		}
		void OnAppearanceCaptionChanged(object sender, EventArgs e) {
			base.OnObjectChanged("AppearanceCaption");
		}
		void OnAppearanceChanged(object sender, EventArgs e) {
			base.OnObjectChanged("Appearance");
		}
		void OnAppearanceDescriptionChanged(object sender, EventArgs e) {
			base.OnObjectChanged("AppearanceDescription");
		}
		[Category("Behavior"), DefaultValue(ContentAlignment.MiddleCenter), XtraSerializableProperty(XtraSerializationFlags.DefaultValue)]
		public ContentAlignment Alignment {
			get { return GetValueCore<ContentAlignment>("Alignment"); }
			set { SetValueCore<ContentAlignment>("Alignment", value); }
		}
		[Category("Appearance"), DefaultValue(true), XtraSerializableProperty(XtraSerializationFlags.DefaultValue)]
		public bool AllowHtmlDraw {
			get { return GetValueCore<bool>("AllowHtmlDraw"); }
			set { SetValueCore<bool>("AllowHtmlDraw", value); }
		}
		[Category("Appearance"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceObject Appearance {
			get { return GetContentCore<AppearanceObject>("Appearance"); }
		}
		bool ShouldSerializeAppearance() {
			return ((Appearance != null) && Appearance.ShouldSerialize());
		}
		void ResetAppearance() {
			Appearance.Reset();
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content), Category("Appearance")]
		public AppearanceObject AppearanceButtons {
			get { return GetContentCore<AppearanceObject>("AppearanceButtons"); }
		}
		bool ShouldSerializeAppearanceButtons() {
			return ((AppearanceButtons != null) && AppearanceButtons.ShouldSerialize());
		}
		void ResetAppearanceButtons() {
			AppearanceButtons.Reset();
		}
		[Category("Appearance"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceObject AppearanceCaption {
			get { return GetContentCore<AppearanceObject>("AppearanceCaption"); }
		}
		bool ShouldSerializeAppearanceCaption() {
			return ((AppearanceCaption != null) && AppearanceCaption.ShouldSerialize());
		}
		void ResetAppearanceCaption() {
			AppearanceCaption.Reset();
		}
		[Category("Appearance"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceObject AppearanceDescription {
			get { return GetContentCore<AppearanceObject>("AppearanceDescription"); }
		}
		bool ShouldSerializeAppearanceDescription() {
			return ((AppearanceDescription != null) && AppearanceDescription.ShouldSerialize());
		}
		void ResetAppearanceDescription() {
			AppearanceDescription.Reset();
		}
		[Category("Behavior"), XtraSerializableProperty(XtraSerializationFlags.DefaultValue), DefaultValue(FlyoutStyle.MessageBox)]
		public FlyoutStyle Style {
			get { return GetValueCore<FlyoutStyle>("Style"); }
			set { SetValueCore<FlyoutStyle>("Style", value); }
		}
		[Category("Behavior"), XtraSerializableProperty(XtraSerializationFlags.DefaultValue), DefaultValue(typeof(Size), "0, 0")]
		public Size ButtonSize {
			get { return GetValueCore<Size>("ButtonSize"); }
			set { SetValueCore<Size>("ButtonSize", value); }
		}
		protected override void AssignCore(IPropertiesProvider source) {
			base.AssignCore(source);
			IFlyoutProperties sourceProperties = source as IFlyoutProperties;
			if(source != null) {
				AllowHtmlDraw = sourceProperties.AllowHtmlDraw;
				Style = sourceProperties.Style;
				Alignment = sourceProperties.Alignment;
				Appearance.AssignInternal(sourceProperties.Appearance);
				AppearanceCaption.AssignInternal(sourceProperties.AppearanceCaption);
				AppearanceDescription.AssignInternal(sourceProperties.AppearanceDescription);
				AppearanceButtons.AssignInternal(sourceProperties.AppearanceButtons);
			}
		}
		protected override void ResetCore() {
			base.ResetCore();
			Appearance.Reset();
			AppearanceCaption.Reset();
			AppearanceDescription.Reset();
			AppearanceButtons.Reset();
		}
		protected override bool ShouldSerializeCore() {
			return base.ShouldSerializeCore() ||
				Appearance.ShouldSerialize() ||
				AppearanceCaption.ShouldSerialize() ||
				AppearanceDescription.ShouldSerialize() ||
				AppearanceButtons.ShouldSerialize();
		}
	}
}
