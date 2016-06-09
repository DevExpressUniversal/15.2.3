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
	class OverviewContainer : BaseContentContainer {
		public OverviewContainer(IContentContainer parent)
			: base((IContentContainerProperties)null) {
			((IContentContainerInternal)this).SetIsAutoCreated(true);
			Parent = parent;
		}
		protected override IContentContainerInfo CreateContentContainerInfo(WindowsUIView view) {
			return new OverviewContainerInfo(view, this);
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public new IOverviewContainerInfo Info {
			get { return base.Info as IOverviewContainerInfo; }
		}
		public override string Caption {
			get { return Parent.Caption; }
			set { }
		}
		public override Image Image {
			get { return Parent.Image; }
			set { }
		}
		protected override int Count {
			get { return ((IContentContainerInternal)Parent).Count; }
		}
		protected override Document[] GetDocumentsCore() {
			return ((IContentContainerInternal)Parent).GetDocuments();
		}
		protected override bool ContainsCore(Document document) {
			return Parent.Contains(document);
		}
		protected override void OnActivated() {
			((IContentContainerInternal)this).SetManager(Parent.Manager);
			using(BatchUpdate.Enter(Manager, true)) {
				Document[] documents = GetDocumentsCore();
				foreach(Document document in documents)
					Info.Register(document);
			}
		}
		protected override void OnDeactivated() {
			using(BatchUpdate.Enter(Manager, true)) {
				if(!Parent.IsDisposing) {
					Document[] documents = GetDocumentsCore();
					foreach(Document document in documents)
						Info.Unregister(document);
				}
			}
			Dispose();
		}
		protected sealed override ContextualZoomLevel GetZoomLevel() {
			return ContextualZoomLevel.Overview;
		}
		protected sealed override IBaseProperties GetParentProperties(WindowsUIView view) {
			return null;
		}
		protected override IContentContainerDefaultProperties CreateDefaultProperties(IContentContainerProperties parentProperties) {
			return new OverviewContainerDefaultProperties(parentProperties);
		}
		protected sealed override void ReleaseDeferredControlLoadDocuments() {
		}
		protected sealed override void EnsureDeferredControlLoadDocuments() {
		}
		protected override void PatchChildrenCore(Rectangle view, bool active) {
			BaseDocument[] documents = Manager.View.Documents.ToArray();
			foreach(BaseDocument document in documents) {
				if(document.IsControlLoaded) {
					Control child = Manager.GetChild(document);
					child.Location = new Point(view.X - child.Width, view.Y - child.Height);
				}
			}
		}
		class OverviewContainerDefaultProperties : ContentContainerDefaultProperties {
			public OverviewContainerDefaultProperties(IContentContainerProperties parentProperties)
				: base(parentProperties) {
			}
			protected override IBaseProperties CloneCore() {
				return new OverviewContainerDefaultProperties(ParentProperties as IContentContainerProperties);
			}
		}
	}
	public interface IOverviewContainerDefaultProperties : IBaseDefaultProperties {
		AppearanceObject AppearanceNormal { get; }
		AppearanceObject AppearanceHovered { get; }
		AppearanceObject AppearancePressed { get; }
		DefaultBoolean AllowHtmlDraw { get; set; }
		Size? ItemSize { get; set; }
		[Browsable(false)]
		AppearanceObject ActualAppearanceNormal { get; }
		[Browsable(false)]
		AppearanceObject ActualAppearanceHovered { get; }
		[Browsable(false)]
		AppearanceObject ActualAppearancePressed { get; }
		[Browsable(false)]
		bool CanHtmlDraw { get; }
		[Browsable(false)]
		Size? ActualItemSize { get; }
	}
	public interface IOverviewContainerProperties : IBaseProperties {
		AppearanceObject AppearanceNormal { get; }
		AppearanceObject AppearanceHovered { get; }
		AppearanceObject AppearancePressed { get; }
		bool AllowHtmlDraw { get; set; }
		Size? ItemSize { get; set; }
	}
	public class OverviewContainerProperties : DevExpress.XtraBars.Docking2010.Base.BaseProperties, IOverviewContainerProperties {
		AppearanceObject appearanceNormalCore;
		AppearanceObject appearanceHoveredCore;
		AppearanceObject appearancePressedCore;
		public OverviewContainerProperties() {
			SetDefaultValueCore("AllowHtmlDraw", true);
			SetDefaultValueCore("AppearanceNormal", AppearanceNormal);
			SetDefaultValueCore("AppearanceHovered", AppearanceHovered);
			SetDefaultValueCore("AppearancePressed", AppearancePressed);
		}
		protected override IBaseProperties CloneCore() {
			return new OverviewContainerProperties();
		}
		protected override void OnCreate() {
			base.OnCreate();
			appearanceNormalCore = new AppearanceObject();
			appearanceHoveredCore = new AppearanceObject();
			appearancePressedCore = new AppearanceObject();
			AppearanceNormal.Changed += OnAppearanceNormalChanged;
			AppearanceHovered.Changed += OnAppearanceHoveredChanged;
			AppearancePressed.Changed += OnAppearancePressedChanged;
		}
		protected override void OnDispose() {
			AppearanceNormal.Changed -= OnAppearanceNormalChanged;
			AppearanceHovered.Changed -= OnAppearanceHoveredChanged;
			AppearancePressed.Changed -= OnAppearancePressedChanged;
			Ref.Dispose(ref appearanceNormalCore);
			Ref.Dispose(ref appearanceHoveredCore);
			Ref.Dispose(ref appearancePressedCore);
			base.OnDispose();
		}
		void OnAppearanceNormalChanged(object sender, EventArgs e) {
			OnObjectChanged("AppearanceNormal");
		}
		void OnAppearanceHoveredChanged(object sender, EventArgs e) {
			OnObjectChanged("AppearanceHovered");
		}
		void OnAppearancePressedChanged(object sender, EventArgs e) {
			OnObjectChanged("AppearanceSelected");
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content), Category(XtraEditors.CategoryName.Appearance)]
		public AppearanceObject AppearanceNormal {
			get { return appearanceNormalCore; }
		}
		bool ShouldSerializeAppearanceNormal() { return AppearanceNormal.ShouldSerialize(); }
		void ResetAppearanceNormal() { AppearanceNormal.Reset(); }
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content), Category(XtraEditors.CategoryName.Appearance)]
		public AppearanceObject AppearanceHovered {
			get { return appearanceHoveredCore; }
		}
		void ResetAppearanceHovered() { AppearanceHovered.Reset(); }
		bool ShouldSerializeAppearanceHovered() { return AppearanceHovered.ShouldSerialize(); }
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content), Category(XtraEditors.CategoryName.Appearance)]
		public AppearanceObject AppearancePressed {
			get { return appearancePressedCore; }
		}
		bool ShouldSerializeAppearancePressed() { return AppearancePressed.ShouldSerialize(); }
		void ResetAppearancePressed() { AppearancePressed.Reset(); }
		[DefaultValue(null), XtraSerializableProperty(XtraSerializationFlags.DefaultValue), Category("Layout")]
		[TypeConverter(typeof(DevExpress.Utils.Design.NullableTypeConverter))]
		public Size? ItemSize {
			get { return GetValueCore<Size?>("ItemSize"); }
			set { SetValueCore("ItemSize", value); }
		}
		[DefaultValue(true), XtraSerializableProperty(XtraSerializationFlags.DefaultValue), Category(XtraEditors.CategoryName.Appearance)]
		public bool AllowHtmlDraw {
			get { return GetValueCore<bool>("AllowHtmlDraw"); }
			set { SetValueCore("AllowHtmlDraw", value); }
		}
		protected override void AssignCore(IPropertiesProvider source) {
			base.AssignCore(source);
			IOverviewContainerProperties sourceProperties = source as IOverviewContainerProperties;
			if(source != null) {
				ItemSize = sourceProperties.ItemSize;
				AllowHtmlDraw = sourceProperties.AllowHtmlDraw;
				AppearanceNormal.AssignInternal(sourceProperties.AppearanceNormal);
				AppearanceHovered.AssignInternal(sourceProperties.AppearanceHovered);
				AppearancePressed.AssignInternal(sourceProperties.AppearancePressed);
			}
		}
		protected override bool ShouldSerializeCore() {
			return base.ShouldSerializeCore() ||
				AppearanceNormal.ShouldSerialize() ||
				AppearanceHovered.ShouldSerialize() ||
				AppearancePressed.ShouldSerialize();
		}
		protected override void ResetCore() {
			base.ResetCore();
			AppearanceNormal.Reset();
			AppearanceHovered.Reset();
			AppearancePressed.Reset();
		}
	}
	public class OverviewContainerDefaultProperties : DevExpress.XtraBars.Docking2010.Base.BaseDefaultProperties, IOverviewContainerDefaultProperties {
		AppearanceObject appearanceNormalCore;
		AppearanceObject appearanceHoveredCore;
		AppearanceObject appearancePressedCore;
		public OverviewContainerDefaultProperties(IOverviewContainerProperties parentProperties)
			: base(parentProperties) {
			SetDefaultValueCore("AppearanceNormal", AppearanceNormal);
			SetDefaultValueCore("AppearanceHovered", AppearanceHovered);
			SetDefaultValueCore("AppearancePressed", AppearancePressed);
			SetDefaultValueCore("AllowHtmlDraw", DevExpress.Utils.DefaultBoolean.Default);
			SetConverter("AllowHtmlDraw", GetDefaultBooleanConverter(true));
		}
		protected override IBaseProperties CloneCore() {
			return new OverviewContainerProperties();
		}
		protected override void OnCreate() {
			base.OnCreate();
			appearanceNormalCore = new AppearanceObject();
			appearanceHoveredCore = new AppearanceObject();
			appearancePressedCore = new AppearanceObject();
			AppearanceNormal.Changed += OnAppearanceNormalChanged;
			AppearanceHovered.Changed += OnAppearanceHoveredChanged;
			AppearancePressed.Changed += OnAppearancePressedChanged;
		}
		protected override void OnDispose() {
			AppearanceNormal.Changed -= OnAppearanceNormalChanged;
			AppearanceHovered.Changed -= OnAppearanceHoveredChanged;
			AppearancePressed.Changed -= OnAppearancePressedChanged;
			Ref.Dispose(ref appearanceNormalCore);
			Ref.Dispose(ref appearanceHoveredCore);
			Ref.Dispose(ref appearancePressedCore);
			base.OnDispose();
		}
		protected override void AssignCore(IPropertiesProvider source) {
			base.AssignCore(source);
			IOverviewContainerDefaultProperties sourceProperties = source as IOverviewContainerDefaultProperties;
			if(source != null) {
				ItemSize = sourceProperties.ItemSize;
				AllowHtmlDraw = sourceProperties.AllowHtmlDraw;
				AppearanceNormal.AssignInternal(sourceProperties.AppearanceNormal);
				AppearanceHovered.AssignInternal(sourceProperties.AppearanceHovered);
				AppearancePressed.AssignInternal(sourceProperties.AppearancePressed);
			}
		}
		void OnAppearanceNormalChanged(object sender, EventArgs e) {
			OnObjectChanged("AppearanceNormal");
		}
		void OnAppearanceHoveredChanged(object sender, EventArgs e) {
			OnObjectChanged("AppearanceHovered");
		}
		void OnAppearancePressedChanged(object sender, EventArgs e) {
			OnObjectChanged("AppearanceSelected");
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content), Category(XtraEditors.CategoryName.Appearance)]
		public AppearanceObject AppearanceNormal {
			get { return appearanceNormalCore; }
		}
		bool ShouldSerializeAppearanceNormal() { return AppearanceNormal.ShouldSerialize(); }
		void ResetAppearanceNormal() { AppearanceNormal.Reset(); }
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content), Category(XtraEditors.CategoryName.Appearance)]
		public AppearanceObject AppearanceHovered {
			get { return appearanceHoveredCore; }
		}
		void ResetAppearanceHovered() { AppearanceHovered.Reset(); }
		bool ShouldSerializeAppearanceHovered() { return AppearanceHovered.ShouldSerialize(); }
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content), Category(XtraEditors.CategoryName.Appearance)]
		public AppearanceObject AppearancePressed {
			get { return appearancePressedCore; }
		}
		bool ShouldSerializeAppearancePressed() { return AppearancePressed.ShouldSerialize(); }
		void ResetAppearancePressed() { AppearancePressed.Reset(); }
		[DefaultValue(null), XtraSerializableProperty(XtraSerializationFlags.DefaultValue), Category("Layout")]
		[TypeConverter(typeof(DevExpress.Utils.Design.NullableTypeConverter))]
		public Size? ItemSize {
			get { return GetValueCore<Size?>("ItemSize"); }
			set { SetValueCore("ItemSize", value); }
		}
		[DefaultValue(DefaultBoolean.Default), XtraSerializableProperty(XtraSerializationFlags.DefaultValue), Category(XtraEditors.CategoryName.Appearance)]
		public DefaultBoolean AllowHtmlDraw {
			get { return GetValueCore<DefaultBoolean>("AllowHtmlDraw"); }
			set { SetValueCore("AllowHtmlDraw", value); }
		}
		protected AppearanceObject GetActualAppearance(AppearanceObject appearance, string propertyName) {
			AppearanceObject result = new AppearanceObject();
			AppearanceHelper.Combine(result, new AppearanceObject[] { appearance, ParentProperties.GetValue<AppearanceObject>(propertyName) });
			return result;
		}
		[Browsable(false)]
		public AppearanceObject ActualAppearanceNormal {
			get { return GetActualAppearance(AppearanceNormal, "AppearanceNormal"); }
		}
		[Browsable(false)]
		public AppearanceObject ActualAppearanceHovered {
			get { return GetActualAppearance(AppearanceHovered, "AppearanceHovered"); }
		}
		[Browsable(false)]
		public AppearanceObject ActualAppearancePressed {
			get { return GetActualAppearance(AppearancePressed, "AppearancePressed"); }
		}
		[Browsable(false)]
		public bool CanHtmlDraw {
			get { return GetActualValue<DefaultBoolean, bool>("AllowHtmlDraw"); }
		}
		[Browsable(false)]
		public Size? ActualItemSize {
			get { return GetActualValue<Size?, Size?>("ItemSize"); }
		}
		protected override bool ShouldSerializeCore() {
			return base.ShouldSerializeCore() ||
				AppearanceNormal.ShouldSerialize() ||
				AppearanceHovered.ShouldSerialize() ||
				AppearancePressed.ShouldSerialize();
		}
		protected override void ResetCore() {
			base.ResetCore();
			AppearanceNormal.Reset();
			AppearanceHovered.Reset();
			AppearancePressed.Reset();
		}
	}
	public interface IParentOverviewContainer {
		IOverviewContainerDefaultProperties OverviewContainerProperties { get; }
	}
}
