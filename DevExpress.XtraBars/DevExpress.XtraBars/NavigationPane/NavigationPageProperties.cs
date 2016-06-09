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
using DevExpress.Utils;
using DevExpress.Utils.Design;
using DevExpress.Utils.Serializing;
using DevExpress.Utils.Base;
namespace DevExpress.XtraBars.Navigation {
	public interface INavigationPageProperties : IBaseProperties, IBaseNavigationPageProperties {
		bool AllowHtmlDraw { get; set; }
		bool AllowBorderColorBlending { get; set; }
		bool ShowExpandButton { get; set; }
		bool ShowCollapseButton { get; set; }
		AppearanceObject AppearanceCaption { get; }
	}
	public interface IBaseNavigationPageProperties : IBaseProperties {
		ItemShowMode ShowMode { get; set; }
	}
	public interface INavigationPageDefaultProperties : IBaseDefaultProperties, IBaseNavigationPageDefaultProperties {
		DefaultBoolean AllowHtmlDraw { get; set; }
		AppearanceObject AppearanceCaption { get;}
		DefaultBoolean AllowBorderColorBlending { get; set; }
		DefaultBoolean ShowExpandButton { get; set; }
		DefaultBoolean ShowCollapseButton { get; set; }
		[Browsable(false)]
		bool CanHtmlDraw { get; }
		[Browsable(false)]
		bool CanBorderColorBlending { get; }
		[Browsable(false)]
		bool CanShowExpandButton { get; }
		[Browsable(false)]
		bool CanShowCollapseButton { get; }
		[Browsable(false)]
		AppearanceObject ActualAppearanceCaption { get; }
	}
	public interface IBaseNavigationPageDefaultProperties : IBaseDefaultProperties {
		ItemShowMode ShowMode { get; set; }
		[Browsable(false)]
		ItemShowMode ActualShowMode { get; }
	}
	public class BaseNavigationPageProperties : DevExpress.XtraBars.Docking2010.Base.BaseProperties, IBaseNavigationPageProperties {
		public BaseNavigationPageProperties() {
			SetDefaultValueCore("ShowMode", ItemShowMode.Default);
		}
		[DefaultValue(ItemShowMode.Default), XtraSerializableProperty(XtraSerializationFlags.DefaultValue), Category(XtraEditors.CategoryName.Behavior), SmartTagProperty("Item Show Mode", "")]
		public ItemShowMode ShowMode {
			get { return GetValueCore<ItemShowMode>("ShowMode"); }
			set { SetValueCore("ShowMode", value); }
		}
		protected override void AssignCore(IPropertiesProvider source) {
			base.AssignCore(source);
			IBaseNavigationPageProperties sourceProperties = source as IBaseNavigationPageProperties;
			if(source != null) {
				ShowMode = sourceProperties.ShowMode;
			}
		}
		protected override IBaseProperties CloneCore() {
			return new BaseNavigationPageProperties();
		}
	}
	public class BaseNavigationPageDefaultProperties : DevExpress.XtraBars.Docking2010.Base.BaseDefaultProperties, IBaseNavigationPageDefaultProperties {
		public BaseNavigationPageDefaultProperties(IBaseNavigationPageProperties parentProperties)
			: base(parentProperties) {
			SetDefaultValueCore("ShowMode", ItemShowMode.Default);
		}
		[DefaultValue(ItemShowMode.Default), XtraSerializableProperty(XtraSerializationFlags.DefaultValue), Category(XtraEditors.CategoryName.Behavior), SmartTagProperty("Item Show Mode", "")]
		public ItemShowMode ShowMode {
			get { return GetValueCore<ItemShowMode>("ShowMode"); }
			set { SetValueCore("ShowMode", value); }
		}
		[Browsable(false)]
		public ItemShowMode ActualShowMode {
			get { return GetActualValue<ItemShowMode, ItemShowMode>("ShowMode"); }
		}
		protected override void AssignCore(IPropertiesProvider source) {
			base.AssignCore(source);
			IBaseNavigationPageDefaultProperties sourceProperties = source as IBaseNavigationPageDefaultProperties;
			if(source != null) {
				ShowMode = sourceProperties.ShowMode;
			}
		}
		protected override IBaseProperties CloneCore() {
			return new BaseNavigationPageDefaultProperties(ParentProperties as IBaseNavigationPageProperties);
		}
	}
	public class NavigationPageProperties : BaseNavigationPageProperties, INavigationPageProperties {
		public NavigationPageProperties() {
			InitContentPropertyCore("AppearanceCaption", new AppearanceObject(), (a) => a.Changed += OnAppearanceCaptionChanged);
			SetDefaultValueCore<bool>("AllowHtmlDraw", true);
			SetDefaultValueCore<bool>("ShowExpandButton", true);
			SetDefaultValueCore<bool>("ShowCollapseButton", true);
			SetDefaultValueCore<bool>("AllowBorderColorBlending", false);
		}
		[XtraSerializableProperty(XtraSerializationFlags.DefaultValue), Category("Appearance"), DefaultValue(true), SmartTagProperty("Allow Html Draw", "")]
		public bool AllowHtmlDraw {
			get { return GetValueCore<bool>("AllowHtmlDraw"); }
			set { SetValueCore<bool>("AllowHtmlDraw", value); }
		}
		[XtraSerializableProperty(XtraSerializationFlags.DefaultValue), Category("Appearance"), DefaultValue(false)]
		public bool AllowBorderColorBlending {
			get { return GetValueCore<bool>("AllowBorderColorBlending"); }
			set { SetValueCore<bool>("AllowBorderColorBlending", value); }
		}
		[XtraSerializableProperty(XtraSerializationFlags.DefaultValue), Category("Appearance"), DefaultValue(true)]
		public bool ShowExpandButton {
			get { return GetValue<bool>("ShowExpandButton"); }
			set { SetValueCore<bool>("ShowExpandButton", value); }
		}
		[XtraSerializableProperty(XtraSerializationFlags.DefaultValue), Category("Appearance"), DefaultValue(true)]
		public bool ShowCollapseButton {
			get { return GetValue<bool>("ShowCollapseButton"); }
			set { SetValueCore<bool>("ShowCollapseButton", value); }
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
		protected override IBaseProperties CloneCore() {
			return new NavigationPageProperties();
		}
		protected override void OnDispose() {
			AppearanceCaption.Changed -= OnAppearanceCaptionChanged;
			base.OnDispose();
		}
		void OnAppearanceCaptionChanged(object sender, EventArgs e) {
			OnObjectChanged("AppearanceCaption");
		}
		protected override void AssignCore(IPropertiesProvider source) {
			base.AssignCore(source);
			INavigationPageProperties sourceProperties = source as INavigationPageProperties;
			if(source != null) {
				AllowHtmlDraw = sourceProperties.AllowHtmlDraw;
				AppearanceCaption.AssignInternal(sourceProperties.AppearanceCaption);
			}
		}
		protected override void ResetCore() {
			base.ResetCore();
			AppearanceCaption.Reset();
		}
		protected override bool ShouldSerializeCore() {
			return base.ShouldSerializeCore() ||
				AppearanceCaption.ShouldSerialize();
		}
	}
	public class NavigationPageDefaultProperties : BaseNavigationPageDefaultProperties, INavigationPageDefaultProperties {
		public NavigationPageDefaultProperties(IBaseNavigationPageProperties parentProperties)
			: base(parentProperties) {
			InitContentPropertyCore("AppearanceCaption", new AppearanceObject(), (a) => a.Changed += OnAppearanceCaptionChanged);
			SetDefaultValueCore<DefaultBoolean>("AllowHtmlDraw", DefaultBoolean.Default);
			SetConverter<DefaultBoolean, bool>("AllowHtmlDraw", BaseDefaultProperties.GetDefaultBooleanConverter(true));
			SetDefaultValueCore<DefaultBoolean>("AllowBorderColorBlending", DefaultBoolean.Default);
			SetConverter<DefaultBoolean, bool>("AllowBorderColorBlending", BaseDefaultProperties.GetDefaultBooleanConverter(false));
			SetDefaultValueCore<DefaultBoolean>("ShowExpandButton", DefaultBoolean.Default);
			SetConverter<DefaultBoolean, bool>("ShowExpandButton", BaseDefaultProperties.GetDefaultBooleanConverter(true));
			SetDefaultValueCore<DefaultBoolean>("ShowCollapseButton", DefaultBoolean.Default);
			SetConverter<DefaultBoolean, bool>("ShowCollapseButton", BaseDefaultProperties.GetDefaultBooleanConverter(true));
		}
		[XtraSerializableProperty(XtraSerializationFlags.DefaultValue), Category("Appearance"), DefaultValue(DefaultBoolean.Default), SmartTagProperty("Allow Html Draw", "")]
		public DefaultBoolean AllowHtmlDraw {
			get { return GetValueCore<DefaultBoolean>("AllowHtmlDraw"); }
			set { SetValueCore<DefaultBoolean>("AllowHtmlDraw", value); }
		}
		[XtraSerializableProperty(XtraSerializationFlags.DefaultValue), Category("Appearance"), DefaultValue(DefaultBoolean.Default)]
		public DefaultBoolean AllowBorderColorBlending {
			get { return GetValueCore<DefaultBoolean>("AllowBorderColorBlending"); }
			set { SetValueCore<DefaultBoolean>("AllowBorderColorBlending", value); }
		}
		[XtraSerializableProperty(XtraSerializationFlags.DefaultValue), Category("Appearance"), DefaultValue(DefaultBoolean.Default)]
		public DefaultBoolean ShowExpandButton {
			get { return GetValueCore<DefaultBoolean>("ShowExpandButton"); }
			set { SetValueCore<DefaultBoolean>("ShowExpandButton", value); }
		}
		[XtraSerializableProperty(XtraSerializationFlags.DefaultValue), Category("Appearance"), DefaultValue(DefaultBoolean.Default)]
		public DefaultBoolean ShowCollapseButton {
			get { return GetValueCore<DefaultBoolean>("ShowCollapseButton"); }
			set { SetValueCore<DefaultBoolean>("ShowCollapseButton", value); }
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
		[Browsable(false)]
		public bool CanHtmlDraw {
			get { return base.GetActualValue<DefaultBoolean, bool>("AllowHtmlDraw"); }
		}
		[Browsable(false)]
		public bool CanBorderColorBlending {
			get { return base.GetActualValue<DefaultBoolean, bool>("AllowBorderColorBlending"); }
		}
		[Browsable(false)]
		public bool CanShowExpandButton {
			get { return GetActualValue<DefaultBoolean, bool>("ShowExpandButton"); }
		}
		[Browsable(false)]
		public bool CanShowCollapseButton {
			get { return GetActualValue<DefaultBoolean, bool>("ShowCollapseButton"); }
		}
		[Browsable(false)]
		public AppearanceObject ActualAppearanceCaption {
			get { return GetActualAppearance(AppearanceCaption, "AppearanceCaption"); }
		}
		protected AppearanceObject GetActualAppearance(AppearanceObject appearance, string propertyName) {
			FrozenAppearance result = new FrozenAppearance();
			if(ParentProperties != null)
				AppearanceHelper.Combine(result, new AppearanceObject[] { appearance, ParentProperties.GetContent<AppearanceObject>(propertyName) });
			return result;
		}
		protected override IBaseProperties CloneCore() {
			return new NavigationPageDefaultProperties(ParentProperties as IBaseNavigationPageProperties);
		}
		protected override void OnDispose() {
			AppearanceCaption.Changed -= OnAppearanceCaptionChanged;
			base.OnDispose();
		}
		void OnAppearanceCaptionChanged(object sender, EventArgs e) {
			OnObjectChanged("AppearanceCaption");
		}
		protected override void AssignCore(IPropertiesProvider source) {
			base.AssignCore(source);
			INavigationPageDefaultProperties sourceProperties = source as INavigationPageDefaultProperties;
			if(source != null) {
				AllowHtmlDraw = sourceProperties.AllowHtmlDraw;
				AppearanceCaption.AssignInternal(sourceProperties.AppearanceCaption);
			}
		}
		protected override void ResetCore() {
			base.ResetCore();
			AppearanceCaption.Reset();
		}
		protected override bool ShouldSerializeCore() {
			return base.ShouldSerializeCore() ||
				AppearanceCaption.ShouldSerialize();
		}
	}
}
