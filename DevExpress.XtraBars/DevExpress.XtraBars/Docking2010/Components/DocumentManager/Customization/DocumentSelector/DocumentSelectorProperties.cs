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
namespace DevExpress.XtraBars.Docking2010.Customization {
	public interface IBaseDocumentSelectorProperties : DevExpress.Utils.Base.IBaseProperties {
		string ItemCaptionFormat { get; set; }
		string DocumentFooterFormat { get; set; }
		string DocumentHeaderFormat { get; set; }
		string PanelFooterFormat { get; set; }
		string PanelHeaderFormat { get; set; }
		ItemSortMode ItemSortMode { get; set; }
		bool ShowPreview { get; set; }
		DefaultBoolean AllowHtmlDraw { get; set; }
		DefaultBoolean AllowGlyphSkinning { get; set; }
		bool CanHtmlDraw { get; }
		bool CanUseCustomComparer { get; }
		bool CanUseAlphabeticalComparer { get; }
		bool CanUseGlyphSkinning { get; }
	}
	public class DocumentSelectorProperties : BaseProperties, IBaseDocumentSelectorProperties {
		public DocumentSelectorProperties() {
			SetDefaultValueCore("DocumentHeaderFormat", "{0}");
			SetDefaultValueCore("DocumentFooterFormat", "{0}");
			SetDefaultValueCore("ItemCaptionFormat", "{0}");
			SetDefaultValueCore("PanelFooterFormat", "{0}");
			SetDefaultValueCore("PanelHeaderFormat", "{0}");
			SetDefaultValueCore("ShowPreview", true);
			SetDefaultValueCore("AllowHtmlDraw", DefaultBoolean.Default);
			SetDefaultValueCore("AllowGlyphSkinning", DefaultBoolean.Default);
			SetDefaultValueCore("ItemSortMode", Customization.ItemSortMode.Default);
		}
		protected override DevExpress.Utils.Base.IBaseProperties CloneCore() {
			return new DocumentSelectorProperties();
		}
		[DefaultValue(Customization.ItemSortMode.Default), XtraSerializableProperty(XtraSerializationFlags.DefaultValue)]
		public Customization.ItemSortMode ItemSortMode {
			get { return GetValueCore<Customization.ItemSortMode>("ItemSortMode"); }
			set { SetValueCore("ItemSortMode", value); }
		}
		[DefaultValue("{0}"), XtraSerializableProperty(XtraSerializationFlags.DefaultValue)]
		public string ItemCaptionFormat {
			get { return GetValueCore<string>("ItemCaptionFormat"); }
			set { SetValueCore("ItemCaptionFormat", value); }
		}
		[DefaultValue("{0}"), XtraSerializableProperty(XtraSerializationFlags.DefaultValue)]
		public string PanelFooterFormat {
			get { return GetValueCore<string>("PanelFooterFormat"); }
			set { SetValueCore("PanelFooterFormat", value); }
		}
		[DefaultValue("{0}"), XtraSerializableProperty(XtraSerializationFlags.DefaultValue)]
		public string PanelHeaderFormat {
			get { return GetValueCore<string>("PanelHeaderFormat"); }
			set { SetValueCore("PanelHeaderFormat", value); }
		}
		[DefaultValue("{0}"), XtraSerializableProperty(XtraSerializationFlags.DefaultValue)]
		public string DocumentHeaderFormat {
			get { return GetValueCore<string>("DocumentHeaderFormat"); }
			set { SetValueCore("DocumentHeaderFormat", value); }
		}
		[DefaultValue("{0}"), XtraSerializableProperty(XtraSerializationFlags.DefaultValue)]
		public string DocumentFooterFormat {
			get { return GetValueCore<string>("DocumentFooterFormat"); }
			set { SetValueCore("DocumentFooterFormat", value); }
		}
		[DefaultValue(true), XtraSerializableProperty(XtraSerializationFlags.DefaultValue)]
		public bool ShowPreview {
			get { return GetValueCore<bool>("ShowPreview"); }
			set { SetValueCore("ShowPreview", value); }
		}
		[DefaultValue(DefaultBoolean.Default), XtraSerializableProperty(XtraSerializationFlags.DefaultValue)]
		public DefaultBoolean AllowHtmlDraw {
			get { return GetValueCore<DefaultBoolean>("AllowHtmlDraw"); }
			set { SetValueCore("AllowHtmlDraw", value); }
		}
		[DefaultValue(DefaultBoolean.Default), XtraSerializableProperty(XtraSerializationFlags.DefaultValue)]
		public DefaultBoolean AllowGlyphSkinning {
			get { return GetValueCore<DefaultBoolean>("AllowGlyphSkinning"); }
			set { SetValueCore("AllowGlyphSkinning", value); }
		}
		[Browsable(false)]
		public bool CanHtmlDraw {
			get { return (AllowHtmlDraw == DefaultBoolean.True); }
		}
		[Browsable(false)]
		public bool CanUseGlyphSkinning {
			get { return (AllowGlyphSkinning != DefaultBoolean.False); }
		}
		[Browsable(false)]
		public bool CanUseCustomComparer {
			get { return ItemSortMode == Customization.ItemSortMode.Custom; }
		}
		[Browsable(false)]
		public bool CanUseAlphabeticalComparer {
			get { return ItemSortMode == Customization.ItemSortMode.Alphabetical; }
		}
	}
}
