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
using DevExpress.Utils.Serializing;
namespace DevExpress.XtraBars.Docking2010.Views.NativeMdi {
	[ToolboxItem(false)]
	public class Document : BaseDocument {
		public Document() { }
		public Document(IContainer container)
			: base(container) {
		}
		public Document(IBaseDocumentProperties defaultProperties)
			: base(defaultProperties) {
		}
		protected override void OnCreate() {
			base.OnCreate();
			allowCaptionColorBlendingCore = true;
			appearanceActiveCaptionCore = new AppearanceObject();
			appearanceCaptionCore = new AppearanceObject();
			appearanceCaptionCore.Changed += OnAppearanceChanged;
			appearanceActiveCaptionCore.Changed += OnAppearanceChanged;
		}
		protected override void OnDispose() {
			base.OnDispose();
			appearanceCaptionCore.Changed -= OnAppearanceChanged;
			appearanceActiveCaptionCore.Changed -= OnAppearanceChanged;
		}
		void OnAppearanceChanged(object sender, System.EventArgs e) {
			LayoutChanged();
		}
#if !SL
	[DevExpressXtraBarsLocalizedDescription("DocumentProperties")]
#endif
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new IDocumentDefaultProperties Properties {
			get { return base.Properties as IDocumentDefaultProperties; }
		}
		protected override IBaseDocumentDefaultProperties CreateDefaultProperties(IBaseDocumentProperties parentProperties) {
			return new DocumentDefaultProperties(parentProperties as IDocumentProperties);
		}
		protected internal System.IntPtr ContentContainerHandle {
			get {
				if(IsDisposing)
					return System.IntPtr.Zero;
				if(IsNonMdi) {
					if(Control != null) {
						DocumentContainer container = Control.Parent as DocumentContainer;
						if(container != null)
							return container.Handle;
					}
				}
				else {
					if(Form != null)
						return Form.Handle;
				}
				return System.IntPtr.Zero;
			}
		}
		[Browsable(false)]
#if !SL
	[DevExpressXtraBarsLocalizedDescription("DocumentIsMaximized")]
#endif
		[Category(DevExpress.XtraEditors.CategoryName.Layout)]
		public override bool IsMaximized {
			get {
				if(IsDisposing) return false;
				if(IsNonMdi) {
					if(Control != null) {
						DocumentContainer container = Control.Parent as DocumentContainer;
						return (container != null) && container.IsMaximized;
					}
					return false;
				}
				return base.IsMaximized;
			}
		}
		protected internal override bool Borderless {
			get { return false; }
		}
		Rectangle boundsCore;
#if !SL
	[DevExpressXtraBarsLocalizedDescription("DocumentBounds")]
#endif
		[Category(DevExpress.XtraEditors.CategoryName.Layout)]
		public Rectangle Bounds {
			get { return boundsCore; }
			set {
				if(boundsCore == value) return;
				this.boundsCore = value;
				OnBoundsChanged();
			}
		}
		bool ShouldSerializeBounds() { return !Bounds.IsEmpty; }
		protected internal override Rectangle GetBoundsCore() {
			return boundsCore;
		}
		protected internal override void SetBoundsCore(Rectangle bounds) {
			if(boundsCore == bounds) return;
			this.boundsCore = bounds;
			BaseView view = Manager.View;
			if(view != null && IsDesignMode(view)) {
				view.SetLayoutModified();
				view.LayoutChanged();
			}
		}
		protected virtual void OnBoundsChanged() {
			if(IsDisposing || IsInitializing || IsFloating) return;
			UpdateDocumentBoundsInContainer();
			BaseView view = Manager.View;
			if(view != null && IsDesignMode(view))
				LayoutChanged();
		}
		protected void UpdateDocumentBoundsInContainer() {
			if(Bounds.IsEmpty || !IsControlLoaded) return;
			if(IsNonMdi) {
				DocumentContainer container = Control.Parent as DocumentContainer;
				if(container != null) {
					if(container.IsMaximized)
						container.RestoreBounds = boundsCore;
					else container.Bounds = boundsCore;
				}
			}
			if(Form != null)
				Form.Bounds = Bounds;
		}
		protected override void OnDeferredLoadControlComplete() {
			base.OnDeferredLoadControlComplete();
			UpdateDocumentBoundsInContainer();
		}
		protected override void OnInitialized() {
			base.OnInitialized();
			UpdateDocumentBoundsInContainer();
		}
		protected override void ApplySettingsCore(IBaseDocumentSettings settings) {
			base.ApplySettingsCore(settings);
			IDocumentSettings documentSettings = settings as IDocumentSettings;
			if(documentSettings != null)
				boundsCore = documentSettings.InitialBounds;
		}
		protected internal override bool HasMaximizeButton() {
			return Properties.CanMaximize && Properties.CanShowMaximizeButton;
		}
		protected internal override bool CanMaximize() {
			return Properties.CanMaximize;
		}
		bool allowCaptionColorBlendingCore;
		[Category("Appearance")]
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("DocumentAllowCaptionColorBlending"),
#endif
 DefaultValue(true)]
		[XtraSerializableProperty]
		public bool AllowCaptionColorBlending {
			get { return allowCaptionColorBlendingCore; }
			set { SetValue(ref allowCaptionColorBlendingCore, value); }
		}
		protected override bool CanBlendCaptionColor() {
			return AllowCaptionColorBlending;
		}
		AppearanceObject appearanceCaptionCore;
		[Category("Appearance"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
#if !SL
	[DevExpressXtraBarsLocalizedDescription("DocumentAppearanceCaption")]
#endif
		public AppearanceObject AppearanceCaption {
			get { return appearanceCaptionCore; }
		}
		bool ShouldSerializeAppearanceCaption() {
			return ((AppearanceCaption != null) && AppearanceCaption.ShouldSerialize());
		}
		void ResetAppearanceCaption() {
			AppearanceCaption.Reset();
		}
		AppearanceObject appearanceActiveCaptionCore;
		[Category("Appearance"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
#if !SL
	[DevExpressXtraBarsLocalizedDescription("DocumentAppearanceActiveCaption")]
#endif
		public AppearanceObject AppearanceActiveCaption {
			get { return appearanceActiveCaptionCore; }
		}
		bool ShouldSerializeAppearanceActiveCaption() {
			return ((AppearanceActiveCaption != null) && AppearanceActiveCaption.ShouldSerialize());
		}
		void ResetAppearanceActiveCaption() {
			AppearanceActiveCaption.Reset();
		}
		protected override AppearanceObject GetActiveDocumentCaptionAppearance() {
			return AppearanceActiveCaption;
		}
		protected override AppearanceObject GetDocumentCaptionAppearance() {
			return AppearanceCaption;
		}
	}
}
namespace DevExpress.XtraBars.Docking2010.Views.NativeMdi {
	using DevExpress.Utils;
	using DevExpress.Utils.Serializing;
	using DevExpress.XtraBars.Docking2010.Base;
	public interface IDocumentSettings : IBaseDocumentSettings {
		Rectangle InitialBounds { get; set; }
	}
	public class DocumentSettings : BaseDocumentSettings, IDocumentSettings {
		public Rectangle InitialBounds {
			get { return GetValueCore<Rectangle>("InitialBounds"); }
			set { SetValueCore("InitialBounds", value); }
		}
	}
	public interface IDocumentProperties : IBaseDocumentProperties {
		bool AllowMaximize { get; set; }
		bool ShowMaximizeButton { get; set; }
	}
	public interface IDocumentDefaultProperties : IBaseDocumentDefaultProperties {
		DefaultBoolean AllowMaximize { get; set; }
		DefaultBoolean ShowMaximizeButton { get; set; }
		[Browsable(false)]
		bool CanMaximize { get; }
		[Browsable(false)]
		bool CanShowMaximizeButton { get; }
	}
	public class DocumentProperties : BaseDocumentProperties, IDocumentProperties {
		public DocumentProperties() {
			SetDefaultValueCore("AllowMaximize", true);
			SetDefaultValueCore("ShowMaximizeButton", true);
		}
		[DefaultValue(true), XtraSerializableProperty(XtraSerializationFlags.DefaultValue)]
		public bool AllowMaximize {
			get { return GetValueCore<bool>("AllowMaximize"); }
			set { SetValueCore("AllowMaximize", value); }
		}
		[DefaultValue(true), XtraSerializableProperty(XtraSerializationFlags.DefaultValue)]
		public bool ShowMaximizeButton {
			get { return GetValueCore<bool>("ShowMaximizeButton"); }
			set { SetValueCore("ShowMaximizeButton", value); }
		}
		protected override DevExpress.Utils.Base.IBaseProperties CloneCore() {
			return new DocumentProperties();
		}
	}
	public class DocumentDefaultProperties : BaseDocumentDefaultProperties, IDocumentDefaultProperties {
		public DocumentDefaultProperties(IDocumentProperties parentProperties)
			: base(parentProperties) {
			SetDefaultValueCore("AllowMaximize", DevExpress.Utils.DefaultBoolean.Default);
			SetDefaultValueCore("ShowMaximizeButton", DevExpress.Utils.DefaultBoolean.Default);
			SetConverter("AllowMaximize", GetDefaultBooleanConverter(true));
			SetConverter("ShowMaximizeButton", GetDefaultBooleanConverter(true));
		}
		[DefaultValue(DefaultBoolean.Default), XtraSerializableProperty(XtraSerializationFlags.DefaultValue)]
		public DefaultBoolean AllowMaximize {
			get { return GetValueCore<DefaultBoolean>("AllowMaximize"); }
			set { SetValueCore("AllowMaximize", value); }
		}
		[DefaultValue(DefaultBoolean.Default), XtraSerializableProperty(XtraSerializationFlags.DefaultValue)]
		public DefaultBoolean ShowMaximizeButton {
			get { return GetValueCore<DefaultBoolean>("ShowMaximizeButton"); }
			set { SetValueCore("ShowMaximizeButton", value); }
		}
		protected override DevExpress.Utils.Base.IBaseProperties CloneCore() {
			return new DocumentDefaultProperties(ParentProperties as IDocumentProperties);
		}
		[Browsable(false)]
		public bool CanMaximize {
			get { return GetActualValue<DefaultBoolean, bool>("AllowMaximize"); }
		}
		[Browsable(false)]
		public bool CanShowMaximizeButton {
			get { return GetActualValue<DefaultBoolean, bool>("ShowMaximizeButton"); }
		}
	}
}
