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
using DevExpress.Utils.Serializing;
using DevExpress.XtraBars.Docking2010.Base;
namespace DevExpress.XtraBars.Docking2010.Views.Tabbed {
	public interface IDocumentProperties : IBaseDocumentProperties {
		bool AllowFloatOnDoubleClick { get; set; }
		bool AllowDockFill { get; set; }
		bool AllowTabReordering { get; set; }
		int TabWidth { get; set; }
		int MaxTabWidth { get; set; }
		bool AllowAnimation { get; set; }
		bool AllowPin { get; set; }
		bool ShowPinButton { get; set; }
		bool ShowInDocumentSelector { get; set; }
		string ThumbnailCaptionFormat { get; set; }
	}
	public interface IDocumentDefaultProperties : IBaseDocumentDefaultProperties {
		DefaultBoolean AllowFloatOnDoubleClick { get; set; }
		DefaultBoolean AllowDockFill { get; set; }
		DefaultBoolean AllowTabReordering { get; set; }
		int? TabWidth { get; set; }
		int? MaxTabWidth { get; set; }
		DefaultBoolean AllowAnimation { get; set; }
		DefaultBoolean AllowPin { get; set; }
		DefaultBoolean ShowPinButton { get; set; }
		DefaultBoolean ShowInDocumentSelector { get; set; }
		string ThumbnailCaptionFormat { get; set; }
		[Browsable(false)]
		bool CanFloatOnDoubleClick { get; }
		[Browsable(false)]
		bool CanDockFill { get; }
		[Browsable(false)]
		bool CanReorderTab { get; }
		[Browsable(false)]
		int ActualTabWidth { get; }
		[Browsable(false)]
		int ActualMaxTabWidth { get; }		
		[Browsable(false)]
		bool CanAnimate { get; }
		[Browsable(false)]
		bool CanPin { get; }
		[Browsable(false)]
		bool CanShowPinButton { get; }
		[Browsable(false)]
		bool CanShowInDocumentSelector { get; }
	}
	public class DocumentProperties : BaseDocumentProperties, IDocumentProperties {
		public DocumentProperties() {
			SetDefaultValueCore("AllowFloatOnDoubleClick", true);
			SetDefaultValueCore("AllowDockFill", true);
			SetDefaultValueCore("AllowTabReordering", true);
			SetDefaultValueCore("AllowAnimation", true);
			SetDefaultValueCore("AllowPin", false);
			SetDefaultValueCore("ShowPinButton", true);
			SetDefaultValueCore("ShowInDocumentSelector", true);
			SetDefaultValueCore("ThumbnailCaptionFormat", "{0} - {1}");
		}
		[DefaultValue(true), XtraSerializableProperty(XtraSerializationFlags.DefaultValue)]
		public bool AllowFloatOnDoubleClick {
			get { return GetValueCore<bool>("AllowFloatOnDoubleClick"); }
			set { SetValueCore("AllowFloatOnDoubleClick", value); }
		}
		[DefaultValue(true), XtraSerializableProperty(XtraSerializationFlags.DefaultValue)]
		public bool AllowDockFill {
			get { return GetValueCore<bool>("AllowDockFill"); }
			set { SetValueCore("AllowDockFill", value); }
		}
		[DefaultValue(true), XtraSerializableProperty(XtraSerializationFlags.DefaultValue)]
		public bool AllowTabReordering {
			get { return GetValueCore<bool>("AllowTabReordering"); }
			set { SetValueCore("AllowTabReordering", value); }
		}
		[XtraSerializablePropertyId(OptionsLayout.UIProperty)]
		[DefaultValue(0), XtraSerializableProperty(XtraSerializationFlags.DefaultValue)]
		public int TabWidth {
			get { return GetValueCore<int>("TabWidth"); }
			set { SetValueCore("TabWidth", value); }
		}
		[DefaultValue(0), XtraSerializableProperty(XtraSerializationFlags.DefaultValue)]
		[XtraSerializablePropertyId(OptionsLayout.UIProperty)]
		public int MaxTabWidth {
			get { return GetValueCore<int>("MaxTabWidth"); }
			set { SetValueCore("MaxTabWidth", value); }
		}
		[DefaultValue(true), XtraSerializableProperty(XtraSerializationFlags.DefaultValue)]
		public bool AllowAnimation {
			get { return GetValueCore<bool>("AllowAnimation"); }
			set { SetValueCore("AllowAnimation", value); }
		}
		[DefaultValue(false), XtraSerializableProperty(XtraSerializationFlags.DefaultValue)]
		public bool AllowPin {
			get { return GetValueCore<bool>("AllowPin"); }
			set { SetValueCore("AllowPin", value); }
		}
		[DefaultValue(true), XtraSerializableProperty(XtraSerializationFlags.DefaultValue)]
		public bool ShowPinButton {
			get { return GetValueCore<bool>("ShowPinButton"); }
			set { SetValueCore("ShowPinButton", value); }
		} 
		[DefaultValue(true), XtraSerializableProperty(XtraSerializationFlags.DefaultValue)]
		public bool ShowInDocumentSelector {
			get { return GetValueCore<bool>("ShowInDocumentSelector"); }
			set { SetValueCore("ShowInDocumentSelector", value); }
		}
		[DefaultValue("{0} - {1}"), XtraSerializableProperty(XtraSerializationFlags.DefaultValue)]
		public string ThumbnailCaptionFormat {
			get { return GetValueCore<string>("ThumbnailCaptionFormat"); }
			set { SetValueCore("ThumbnailCaptionFormat", value); }
		}
		protected override DevExpress.Utils.Base.IBaseProperties CloneCore() {
			return new DocumentProperties();
		}
	}
	public class DocumentDefaultProperties : BaseDocumentDefaultProperties, IDocumentDefaultProperties {
		public DocumentDefaultProperties(IDocumentProperties parentProperties)
			: base(parentProperties) {
			SetDefaultValueCore("AllowFloatOnDoubleClick", DevExpress.Utils.DefaultBoolean.Default);
			SetDefaultValueCore("AllowDockFill", DevExpress.Utils.DefaultBoolean.Default);
			SetDefaultValueCore("AllowTabReordering", DevExpress.Utils.DefaultBoolean.Default);
			SetDefaultValueCore("AllowAnimation", DevExpress.Utils.DefaultBoolean.Default);
			SetDefaultValueCore("AllowPin", DevExpress.Utils.DefaultBoolean.Default);
			SetDefaultValueCore("ShowPinButton", DevExpress.Utils.DefaultBoolean.Default);
			SetDefaultValueCore("ShowInDocumentSelector", DevExpress.Utils.DefaultBoolean.Default);
			SetDefaultValueCore("ThumbnailCaptionFormat", "{0} - {1}");
			SetConverter("AllowFloatOnDoubleClick", GetDefaultBooleanConverter(true));
			SetConverter("AllowDockFill", GetDefaultBooleanConverter(true));
			SetConverter("AllowTabReordering", GetDefaultBooleanConverter(true));
			SetConverter("AllowAnimation", GetDefaultBooleanConverter(true));
			SetConverter("AllowPin", GetDefaultBooleanConverter(false));
			SetConverter("ShowPinButton", GetDefaultBooleanConverter(true));
			SetConverter("ShowInDocumentSelector", GetDefaultBooleanConverter(true));			
		}
		[DefaultValue(DefaultBoolean.Default), XtraSerializableProperty(XtraSerializationFlags.DefaultValue)]
		public DefaultBoolean AllowFloatOnDoubleClick {
			get { return GetValueCore<DefaultBoolean>("AllowFloatOnDoubleClick"); }
			set { SetValueCore("AllowFloatOnDoubleClick", value); }
		}
		[DefaultValue(DefaultBoolean.Default), XtraSerializableProperty(XtraSerializationFlags.DefaultValue)]
		public DefaultBoolean AllowDockFill {
			get { return GetValueCore<DefaultBoolean>("AllowDockFill"); }
			set { SetValueCore("AllowDockFill", value); }
		}
		[DefaultValue(DefaultBoolean.Default), XtraSerializableProperty(XtraSerializationFlags.DefaultValue)]
		public DefaultBoolean AllowTabReordering {
			get { return GetValueCore<DefaultBoolean>("AllowTabReordering"); }
			set { SetValueCore("AllowTabReordering", value); }
		}
		[DefaultValue(DefaultBoolean.Default), XtraSerializableProperty(XtraSerializationFlags.DefaultValue)]
		public DefaultBoolean AllowAnimation {
			get { return GetValueCore<DefaultBoolean>("AllowAnimation"); }
			set { SetValueCore("AllowAnimation", value); }
		}
		[DefaultValue(null), XtraSerializableProperty(XtraSerializationFlags.DefaultValue)]
		[XtraSerializablePropertyId(OptionsLayout.UIProperty)]
		public int? TabWidth {
			get { return GetValueCore<int?>("TabWidth"); }
			set { SetValueCore("TabWidth", value); }
		}
		[DefaultValue(null), XtraSerializableProperty(XtraSerializationFlags.DefaultValue)]
		[XtraSerializablePropertyId(OptionsLayout.UIProperty)]
		public int? MaxTabWidth {
			get { return GetValueCore<int?>("MaxTabWidth"); }
			set { SetValueCore("MaxTabWidth", value); }
		}
		[DefaultValue(DefaultBoolean.Default), XtraSerializableProperty(XtraSerializationFlags.DefaultValue)]
		public DefaultBoolean AllowPin {
			get { return GetValueCore<DefaultBoolean>("AllowPin"); }
			set { SetValueCore("AllowPin", value); }
		}
		[DefaultValue(DefaultBoolean.Default), XtraSerializableProperty(XtraSerializationFlags.DefaultValue)]
		public DefaultBoolean ShowPinButton {
			get { return GetValueCore<DefaultBoolean>("ShowPinButton"); }
			set { SetValueCore("ShowPinButton", value); }
		}
		[DefaultValue(DefaultBoolean.Default), XtraSerializableProperty(XtraSerializationFlags.DefaultValue)]
		public DefaultBoolean ShowInDocumentSelector {
			get { return GetValueCore<DefaultBoolean>("ShowInDocumentSelector"); }
			set { SetValueCore("ShowInDocumentSelector", value); }
		}
		[DefaultValue("{0} - {1}"), XtraSerializableProperty(XtraSerializationFlags.DefaultValue)]
		public string ThumbnailCaptionFormat {
			get { return GetValueCore<string>("ThumbnailCaptionFormat"); }
			set { SetValueCore("ThumbnailCaptionFormat", value); }
		}
		protected override DevExpress.Utils.Base.IBaseProperties CloneCore() {
			return new DocumentDefaultProperties(ParentProperties as IDocumentProperties);
		}
		[Browsable(false)]
		public bool CanFloatOnDoubleClick {
			get { return GetActualValue<DefaultBoolean, bool>("AllowFloatOnDoubleClick"); }
		}
		[Browsable(false)]
		public bool CanDockFill {
			get { return GetActualValue<DefaultBoolean, bool>("AllowDockFill"); }
		}
		[Browsable(false)]
		public bool CanReorderTab {
			get { return GetActualValue<DefaultBoolean, bool>("AllowTabReordering"); }
		}
		[Browsable(false)]
		public int ActualTabWidth {
			get { return GetActualValueFromNullable<int>("TabWidth"); }
		}
		[Browsable(false)]
		public int ActualMaxTabWidth {
			get { return GetActualValueFromNullable<int>("MaxTabWidth"); }
		}	   
		[Browsable(false)]
		public bool CanAnimate {
			get { return GetActualValue<DefaultBoolean, bool>("AllowAnimation"); }
		}
		[Browsable(false)]
		public bool CanPin {
			get { return GetActualValue<DefaultBoolean, bool>("AllowPin"); }
		}
		[Browsable(false)]
		public bool CanShowPinButton {
			get { return GetActualValue<DefaultBoolean, bool>("ShowPinButton"); }
		}
		[Browsable(false)]
		public bool CanShowInDocumentSelector {
			get { return GetActualValue<DefaultBoolean, bool>("ShowInDocumentSelector"); }
		}
	}
	public class DocumentSettings : BaseDocumentSettings { 
	}
}
