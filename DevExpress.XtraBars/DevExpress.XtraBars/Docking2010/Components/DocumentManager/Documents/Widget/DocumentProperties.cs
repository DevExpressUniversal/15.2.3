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
using DevExpress.Utils;
using DevExpress.Utils.Serializing;
using DevExpress.XtraBars.Docking2010.Base;
namespace DevExpress.XtraBars.Docking2010.Views.Widget {
	public interface IDocumentProperties : IBaseDocumentProperties {
		bool AllowResize { get; set; }
		bool AllowCollapse { get; set; }
		bool AllowMaximize { get; set; }
		bool AllowDragging { get; set; }
		bool ShowMaximizeButton { get; set; }
		bool ShowBorders { get; set; }
	}
	public interface IDocumentDefaultProperties : IBaseDocumentDefaultProperties {
		DefaultBoolean AllowResize { get; set; }
		DefaultBoolean AllowCollapse { get; set; }
		DefaultBoolean AllowMaximize { get; set; }
		DefaultBoolean AllowDragging { get; set; }
		DefaultBoolean ShowMaximizeButton { get; set; }
		DefaultBoolean ShowBorders { get; set; }
		[Browsable(false)]
		bool CanResize { get; }
		[Browsable(false)]
		bool CanCollapse { get; }
		[Browsable(false)]
		bool CanMaximize { get; }
		[Browsable(false)]
		bool CanDragging { get; }
		[Browsable(false)]
		bool CanShowMaximizeButton { get; }
		[Browsable(false)]
		bool CanShowBorders { get; }
	}
	public class DocumentProperties : BaseDocumentProperties, IDocumentProperties {
		public DocumentProperties() {
			SetDefaultValueCore("AllowResize", true);
			SetDefaultValueCore("AllowCollapse", true);
			SetDefaultValueCore("AllowMaximize", true);
			SetDefaultValueCore("AllowDragging", true);
			SetDefaultValueCore("ShowMaximizeButton", true);
			SetDefaultValueCore("ShowBorders", true);
		}
		protected override DevExpress.Utils.Base.IBaseProperties CloneCore() {
			return new DocumentProperties();
		}
		[DefaultValue(true), XtraSerializableProperty(XtraSerializationFlags.DefaultValue)]
		public bool AllowResize {
			get { return GetValueCore<bool>("AllowResize"); }
			set { SetValueCore("AllowResize", value); }
		}
		[DefaultValue(true), XtraSerializableProperty(XtraSerializationFlags.DefaultValue)]
		public bool AllowCollapse {
			get { return GetValueCore<bool>("AllowCollapse"); }
			set { SetValueCore("AllowCollapse", value); }
		}
		[DefaultValue(true), XtraSerializableProperty(XtraSerializationFlags.DefaultValue)]
		public bool AllowMaximize {
			get { return GetValueCore<bool>("AllowMaximize"); }
			set { SetValueCore("AllowMaximize", value); }
		}
		[DefaultValue(true), XtraSerializableProperty(XtraSerializationFlags.DefaultValue)]
		public bool AllowDragging {
			get { return GetValueCore<bool>("AllowDragging"); }
			set { SetValueCore("AllowDragging", value); }
		}
		[DefaultValue(true), XtraSerializableProperty(XtraSerializationFlags.DefaultValue)]
		public bool ShowMaximizeButton {
			get { return GetValueCore<bool>("ShowMaximizeButton"); }
			set { SetValueCore("ShowMaximizeButton", value); }
		}
		[DefaultValue(true), XtraSerializableProperty(XtraSerializationFlags.DefaultValue)]
		public bool ShowBorders {
			get { return GetValueCore<bool>("ShowBorders"); }
			set { SetValueCore("ShowBorders", value); }
		}
	}
	public class DocumentDefaultProperties : BaseDocumentDefaultProperties, IDocumentDefaultProperties {
		public DocumentDefaultProperties(IDocumentProperties parentProperties)
			: base(parentProperties) {
			SetDefaultValueCore("AllowResize", DefaultBoolean.Default);
			SetDefaultValueCore("AllowCollapse", DefaultBoolean.Default);
			SetDefaultValueCore("AllowMaximize", DefaultBoolean.Default);
			SetDefaultValueCore("AllowDragging", DefaultBoolean.Default);
			SetDefaultValueCore("ShowMaximizeButton", DefaultBoolean.Default);
			SetDefaultValueCore("ShowBorders", DefaultBoolean.Default);
			SetConverter("AllowResize", GetDefaultBooleanConverter(true));
			SetConverter("AllowCollapse", GetDefaultBooleanConverter(true));
			SetConverter("AllowMaximize", GetDefaultBooleanConverter(true));
			SetConverter("AllowDragging", GetDefaultBooleanConverter(true));
			SetConverter("ShowMaximizeButton", GetDefaultBooleanConverter(true));
			SetConverter("ShowBorders", GetDefaultBooleanConverter(true));
		}
		protected override DevExpress.Utils.Base.IBaseProperties CloneCore() {
			return new DocumentDefaultProperties(ParentProperties as IDocumentProperties);
		}
		[DefaultValue(DefaultBoolean.Default), XtraSerializableProperty(XtraSerializationFlags.DefaultValue)]
		public DefaultBoolean AllowResize {
			get { return GetValueCore<DefaultBoolean>("AllowResize"); }
			set { SetValueCore("AllowResize", value); }
		}
		[DefaultValue(DefaultBoolean.Default), XtraSerializableProperty(XtraSerializationFlags.DefaultValue)]
		public DefaultBoolean AllowCollapse {
			get { return GetValueCore<DefaultBoolean>("AllowCollapse"); }
			set { SetValueCore("AllowCollapse", value); }
		}
		[DefaultValue(DefaultBoolean.Default), XtraSerializableProperty(XtraSerializationFlags.DefaultValue)]
		public DefaultBoolean AllowMaximize {
			get { return GetValueCore<DefaultBoolean>("AllowMaximize"); }
			set { SetValueCore("AllowMaximize", value); }
		}
		[DefaultValue(DefaultBoolean.Default), XtraSerializableProperty(XtraSerializationFlags.DefaultValue)]
		public DefaultBoolean AllowDragging {
			get { return GetValueCore<DefaultBoolean>("AllowDragging"); }
			set { SetValueCore("AllowDragging", value); }
		}
		[DefaultValue(DefaultBoolean.Default), XtraSerializableProperty(XtraSerializationFlags.DefaultValue)]
		public DefaultBoolean ShowMaximizeButton {
			get { return GetValueCore<DefaultBoolean>("ShowMaximizeButton"); }
			set { SetValueCore("ShowMaximizeButton", value); }
		}
		[DefaultValue(DefaultBoolean.Default), XtraSerializableProperty(XtraSerializationFlags.DefaultValue)]
		public DefaultBoolean ShowBorders {
			get { return GetValueCore<DefaultBoolean>("ShowBorders"); }
			set { SetValueCore("ShowBorders", value); }
		}
		[Browsable(false)]
		public bool CanResize {
			get { return GetActualValue<DefaultBoolean, bool>("AllowResize"); }
		}
		[Browsable(false)]
		public bool CanCollapse {
			get { return GetActualValue<DefaultBoolean, bool>("AllowCollapse"); }
		}
		[Browsable(false)]
		public bool CanMaximize {
			get { return GetActualValue<DefaultBoolean, bool>("AllowMaximize"); }
		}
		[Browsable(false)]
		public bool CanDragging {
			get { return GetActualValue<DefaultBoolean, bool>("AllowDragging"); }
		}
		[Browsable(false)]
		public bool CanShowMaximizeButton {
			get { return GetActualValue<DefaultBoolean, bool>("ShowMaximizeButton"); }
		}
		[Browsable(false)]
		public bool CanShowBorders {
			get { return GetActualValue<DefaultBoolean, bool>("ShowBorders"); }
		}
	}
}
