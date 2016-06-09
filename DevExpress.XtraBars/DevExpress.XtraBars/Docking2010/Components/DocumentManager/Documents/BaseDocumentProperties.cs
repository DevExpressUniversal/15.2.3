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
using DevExpress.Utils.Base;
namespace DevExpress.XtraBars.Docking2010.Views {
	public interface IBaseDocumentProperties : IBaseProperties {
		bool AllowClose { get; set; }
		bool AllowActivate { get; set; }
		bool AllowFloat { get; set; }
		bool AllowDock { get; set; }
		bool UseFormIconAsDocumentImage { get; set; }
		bool AllowGlyphSkinning { get; set; }
	}
	public interface IBaseDocumentDefaultProperties : IBaseDefaultProperties {
		DefaultBoolean AllowClose { get; set; }
		DefaultBoolean AllowActivate { get; set; }
		DefaultBoolean AllowFloat { get; set; }
		DefaultBoolean AllowDock { get; set; }
		DefaultBoolean UseFormIconAsDocumentImage { get; set; }
		DefaultBoolean AllowGlyphSkinning { get; set; }
		[Browsable(false)]
		bool CanClose { get; }
		[Browsable(false)]
		bool CanActivate { get; }
		[Browsable(false)]
		bool CanFloat { get; }
		[Browsable(false)]
		bool CanDock { get; }
		[Browsable(false)]
		bool CanUseFormIconAsDocumentImage { get; }
		[Browsable(false)]
		bool CanUseGlyphSkinning { get; }
		[Browsable(false)]
		string ActualThumbnailCaptionFormat { get; }
	}
	public interface IBaseDocumentSettings : IPropertiesProvider {
		string Caption { get; set; }
		Image Image { get; set; }
		Size? FloatSize { get; set; }
		Point? FloatLocation { get; set; }
	}
	public class BaseDocumentProperties : DevExpress.XtraBars.Docking2010.Base.BaseProperties, IBaseDocumentProperties {
		public BaseDocumentProperties() {
			SetDefaultValueCore("AllowClose", true);
			SetDefaultValueCore("AllowActivate", true);
			SetDefaultValueCore("AllowFloat", true);
			SetDefaultValueCore("AllowDock", true);
		}
		[DefaultValue(true), XtraSerializableProperty(XtraSerializationFlags.DefaultValue), Category(XtraEditors.CategoryName.Behavior)]
		public bool AllowClose {
			get { return GetValueCore<bool>("AllowClose"); }
			set { SetValueCore("AllowClose", value); }
		}
		[DefaultValue(true), XtraSerializableProperty(XtraSerializationFlags.DefaultValue), Category(XtraEditors.CategoryName.Behavior)]
		public bool AllowActivate {
			get { return GetValueCore<bool>("AllowActivate"); }
			set { SetValueCore("AllowActivate", value); }
		}
		[DefaultValue(true), XtraSerializableProperty(XtraSerializationFlags.DefaultValue), Category(XtraEditors.CategoryName.Behavior)]
		public bool AllowFloat {
			get { return GetValueCore<bool>("AllowFloat"); }
			set { SetValueCore("AllowFloat", value); }
		}
		[DefaultValue(true), XtraSerializableProperty(XtraSerializationFlags.DefaultValue), Category(XtraEditors.CategoryName.Behavior)]
		public bool AllowDock {
			get { return GetValueCore<bool>("AllowDock"); }
			set { SetValueCore("AllowDock", value); }
		}
		[DefaultValue(false), XtraSerializableProperty(XtraSerializationFlags.DefaultValue), Category(XtraEditors.CategoryName.Appearance)]
		public bool UseFormIconAsDocumentImage {
			get { return GetValueCore<bool>("UseFormIconAsDocumentImage"); }
			set { SetValueCore("UseFormIconAsDocumentImage", value); }
		}
		[DefaultValue(false), XtraSerializableProperty(XtraSerializationFlags.DefaultValue), Category(XtraEditors.CategoryName.Appearance)]
		public bool AllowGlyphSkinning {
			get { return GetValueCore<bool>("AllowGlyphSkinning"); }
			set { SetValueCore("AllowGlyphSkinning", value); }
		}
		protected override IBaseProperties CloneCore() {
			return new BaseDocumentProperties();
		}
	}
	public class BaseDocumentDefaultProperties : DevExpress.XtraBars.Docking2010.Base.BaseDefaultProperties, IBaseDocumentDefaultProperties {
		public BaseDocumentDefaultProperties(IBaseDocumentProperties parentProperties)
			: base(parentProperties) {
			SetDefaultValueCore("AllowClose", DevExpress.Utils.DefaultBoolean.Default);
			SetDefaultValueCore("AllowActivate", DevExpress.Utils.DefaultBoolean.Default);
			SetDefaultValueCore("AllowFloat", DevExpress.Utils.DefaultBoolean.Default);
			SetDefaultValueCore("AllowDock", DevExpress.Utils.DefaultBoolean.Default);
			SetDefaultValueCore("UseFormIconAsDocumentImage", DevExpress.Utils.DefaultBoolean.Default);
			SetDefaultValueCore("AllowGlyphSkinning", DevExpress.Utils.DefaultBoolean.Default);
			SetConverter("AllowClose", GetDefaultBooleanConverter(true));
			SetConverter("AllowActivate", GetDefaultBooleanConverter(true));
			SetConverter("AllowFloat", GetDefaultBooleanConverter(true));
			SetConverter("AllowDock", GetDefaultBooleanConverter(true));
			SetConverter("UseFormIconAsDocumentImage", GetDefaultBooleanConverter(false));
			SetConverter("AllowGlyphSkinning", GetDefaultBooleanConverter(false));
		}
		[DefaultValue(DefaultBoolean.Default), XtraSerializableProperty(XtraSerializationFlags.DefaultValue), Category(XtraEditors.CategoryName.Behavior)]
		public DefaultBoolean AllowClose {
			get { return GetValueCore<DefaultBoolean>("AllowClose"); }
			set { SetValueCore("AllowClose", value); }
		}
		[DefaultValue(DefaultBoolean.Default), XtraSerializableProperty(XtraSerializationFlags.DefaultValue), Category(XtraEditors.CategoryName.Behavior)]
		public DefaultBoolean AllowActivate {
			get { return GetValueCore<DefaultBoolean>("AllowActivate"); }
			set { SetValueCore("AllowActivate", value); }
		}
		[DefaultValue(DefaultBoolean.Default), XtraSerializableProperty(XtraSerializationFlags.DefaultValue), Category(XtraEditors.CategoryName.Behavior)]
		public DefaultBoolean AllowFloat {
			get { return GetValueCore<DefaultBoolean>("AllowFloat"); }
			set { SetValueCore("AllowFloat", value); }
		}
		[DefaultValue(DefaultBoolean.Default), XtraSerializableProperty(XtraSerializationFlags.DefaultValue), Category(XtraEditors.CategoryName.Behavior)]
		public DefaultBoolean AllowDock {
			get { return GetValueCore<DefaultBoolean>("AllowDock"); }
			set { SetValueCore("AllowDock", value); }
		}
		[DefaultValue(DefaultBoolean.Default), XtraSerializableProperty(XtraSerializationFlags.DefaultValue), Category(XtraEditors.CategoryName.Appearance)]
		public DefaultBoolean UseFormIconAsDocumentImage {
			get { return GetValueCore<DefaultBoolean>("UseFormIconAsDocumentImage"); }
			set { SetValueCore("UseFormIconAsDocumentImage", value); }
		}
		[DefaultValue(DefaultBoolean.Default), XtraSerializableProperty(XtraSerializationFlags.DefaultValue), Category(XtraEditors.CategoryName.Appearance)]
		public DefaultBoolean AllowGlyphSkinning {
			get { return GetValueCore<DefaultBoolean>("AllowGlyphSkinning"); }
			set { SetValueCore("AllowGlyphSkinning", value); }
		}
		protected override IBaseProperties CloneCore() {
			return new BaseDocumentDefaultProperties(ParentProperties as IBaseDocumentProperties);
		}
		[Browsable(false)]
		public bool CanClose {
			get { return GetActualValue<DefaultBoolean, bool>("AllowClose"); }
		}
		[Browsable(false)]
		public bool CanActivate {
			get { return GetActualValue<DefaultBoolean, bool>("AllowActivate"); }
		}
		[Browsable(false)]
		public bool CanFloat {
			get { return GetActualValue<DefaultBoolean, bool>("AllowFloat"); }
		}
		[Browsable(false)]
		public string ActualThumbnailCaptionFormat {
			get { return GetActualValue<string, string>("ThumbnailCaptionFormat"); }
		}
		[Browsable(false)]
		public bool CanDock {
			get { return GetActualValue<DefaultBoolean, bool>("AllowDock"); }
		}
		[Browsable(false)]
		public bool CanUseFormIconAsDocumentImage {
			get { return GetActualValue<DefaultBoolean, bool>("UseFormIconAsDocumentImage"); }
		}
		[Browsable(false)]
		public bool CanUseGlyphSkinning {
			get { return GetActualValue<DefaultBoolean, bool>("AllowGlyphSkinning"); }
		}
	}
	public class BaseDocumentSettings : BasePropertiesProvider, IBaseDocumentSettings {
		public string Caption {
			get { return GetValueCore<string>("Caption"); }
			set { SetValueCore("Caption", value); }
		}
		public Image Image {
			get { return GetValueCore<Image>("Image"); }
			set { SetValueCore("Image", value); }
		}
		[TypeConverter(typeof(DevExpress.Utils.Design.NullableTypeConverter))]
		public Size? FloatSize {
			get { return GetValueCore<Size?>("FloatSize"); }
			set { SetValueCore("FloatSize", value); }
		}
		[TypeConverter(typeof(DevExpress.Utils.Design.NullableTypeConverter))]
		public Point? FloatLocation {
			get { return GetValueCore<Point?>("FloatLocation"); }
			set { SetValueCore("FloatLocation", value); }
		}
	}
}
