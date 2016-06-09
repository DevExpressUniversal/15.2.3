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
using System.Collections.Generic;
using System.Text;
namespace DevExpress.Web.ASPxHtmlEditor {
	using System.ComponentModel;
	using System.Web.UI;
	using DevExpress.Web.ASPxHtmlEditor.Internal;
	public class HtmlEditorHtmlEditingSettings : ASPxHtmlEditorSettingsBase, IHtmlProcessingSettings {
		protected internal static readonly HtmlEditorHtmlEditingSettings Default = new HtmlEditorHtmlEditingSettings();
		HtmlEditorContentElementFiltering contentElementFiltering;
		public HtmlEditorHtmlEditingSettings() : 
			this(null) {
		}
		protected HtmlEditorHtmlEditingSettings(IPropertiesOwner owner)
			: base(owner) {
		}
		[Category("Behavior"), AutoFormatDisable, DefaultValue(true), NotifyParentProperty(true)]
		public bool UpdateBoldItalic {
			get { return GetBoolProperty("UpdateBoldItalic", true); }
			set { SetBoolPropertyAndMakeHtmlDirty("UpdateBoldItalic", true, value); }
		}
		[Category("Behavior"), AutoFormatDisable, DefaultValue(true), NotifyParentProperty(true)]
		public bool UpdateDeprecatedElements {
			get { return GetBoolProperty("UpdateDeprecatedElements", true); }
			set { SetBoolPropertyAndMakeHtmlDirty("UpdateDeprecatedElements", true, value); }
		}
		[Category("Behavior"), AutoFormatDisable, DefaultValue(ResourcePathMode.NotSet), NotifyParentProperty(true)]
		public ResourcePathMode ResourcePathMode {
			get { return (ResourcePathMode)GetEnumProperty("ResourcePathMode", ResourcePathMode.NotSet); }
			set { SetEnumPropertyAndMakeHtmlDirty("ResourcePathMode", ResourcePathMode.NotSet, value); }
		}
		[Category("Behavior"), NotifyParentProperty(true), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), AutoFormatDisable]
		public HtmlEditorContentElementFiltering ContentElementFiltering {
			get {
				if(this.contentElementFiltering == null)
					this.contentElementFiltering = new HtmlEditorContentElementFiltering(Owner);
				return this.contentElementFiltering;
			}
		}
		[Category("Behavior"), AutoFormatDisable, DefaultValue(true), NotifyParentProperty(true)]
		public bool AllowIdAttributes {
			get { return GetBoolProperty("AllowIdAttributes", true); }
			set { SetBoolPropertyAndMakeHtmlDirty("AllowIdAttributes", true, value); }
		}
		[Category("Behavior"), AutoFormatDisable, DefaultValue(true), NotifyParentProperty(true)]
		public bool AllowStyleAttributes {
			get { return GetBoolProperty("AllowStyleAttributes", true); }
			set { SetBoolPropertyAndMakeHtmlDirty("AllowStyleAttributes", true, value); }
		}
		[Category("Behavior"), AutoFormatDisable, DefaultValue(false), NotifyParentProperty(true)]
		public bool AllowScripts {
			get { return GetBoolProperty("AllowScripts", false); }
			set { SetBoolPropertyAndMakeHtmlDirty("AllowScripts", false, value); }
		}
		[Category("Behavior"), AutoFormatDisable, DefaultValue(false), NotifyParentProperty(true)]
		public bool AllowIFrames {
			get { return GetBoolProperty("AllowIFrames", false); }
			set { SetBoolPropertyAndMakeHtmlDirty("AllowIFrames", false, value); }
		}
		[Category("Behavior"), AutoFormatDisable, DefaultValue(false), NotifyParentProperty(true)]
		public bool AllowFormElements {
			get { return GetBoolProperty("AllowFormElements", false); }
			set { SetBoolPropertyAndMakeHtmlDirty("AllowFormElements", false, value); }
		}
		[Category("Behavior"), AutoFormatDisable, DefaultValue(AllowedDocumentType.XHTML), NotifyParentProperty(true)]
		public AllowedDocumentType AllowedDocumentType {
			get { return (AllowedDocumentType)GetEnumProperty("AllowedDocumentType", AllowedDocumentType.XHTML); }
			set { SetEnumPropertyAndMakeHtmlDirty("AllowedDocumentType", AllowedDocumentType.XHTML, value); }
		}
		[Category("Behavior"), AutoFormatDisable, DefaultValue(false), NotifyParentProperty(true)]
		public bool AllowHTML5MediaElements {
			get { return GetBoolProperty("AllowHTML5MediaElements", false); }
			set { SetBoolPropertyAndMakeHtmlDirty("AllowHTML5MediaElements", false, value); }
		}
		[Category("Behavior"), AutoFormatDisable, DefaultValue(false), NotifyParentProperty(true)]
		public bool AllowObjectAndEmbedElements {
			get { return GetBoolProperty("AllowObjectAndEmbedElements", false); }
			set { SetBoolPropertyAndMakeHtmlDirty("AllowObjectAndEmbedElements", false, value); }
		}
		[Category("Behavior"), AutoFormatDisable, DefaultValue(false), NotifyParentProperty(true)]
		public bool AllowYouTubeVideoIFrames {
			get { return GetBoolProperty("AllowYouTubeVideoIFrames", false); }
			set { SetBoolPropertyAndMakeHtmlDirty("AllowYouTubeVideoIFrames", false, value); }
		}
		[Category("Behavior"), AutoFormatDisable, DefaultValue(false), NotifyParentProperty(true)]
		public bool AllowEditFullDocument {
			get { return GetBoolProperty("AllowEditFullDocument", false); }
			set { SetBoolPropertyAndMakeHtmlDirty("AllowEditFullDocument", false, value); }
		}
		public override void Assign(PropertiesBase source) {
			BeginUpdate();
			try {
				base.Assign(source);
				HtmlEditorHtmlEditingSettings src = source as HtmlEditorHtmlEditingSettings;
				if(src != null) {
					AllowScripts = src.AllowScripts;
					AllowIFrames = src.AllowIFrames;
					AllowFormElements = src.AllowFormElements;
					UpdateBoldItalic = src.UpdateBoldItalic;
					UpdateDeprecatedElements = src.UpdateDeprecatedElements;
					AllowIdAttributes = src.AllowIdAttributes;
					AllowStyleAttributes = src.AllowStyleAttributes;
					AllowedDocumentType = src.AllowedDocumentType;
					AllowHTML5MediaElements = src.AllowHTML5MediaElements;
					AllowObjectAndEmbedElements = src.AllowObjectAndEmbedElements;
					AllowYouTubeVideoIFrames = src.AllowYouTubeVideoIFrames;
					AllowEditFullDocument = src.AllowEditFullDocument;
					ResourcePathMode = src.ResourcePathMode;
					ContentElementFiltering.Assign(src.ContentElementFiltering);
				}
			} finally {
				EndUpdate();
			}
		}
		protected void SetBoolPropertyAndMakeHtmlDirty(string key, bool defaultValue, bool value) {
			SetBoolProperty(key, defaultValue, value);
			MakeHtmlDirty();
		}
		protected void SetEnumPropertyAndMakeHtmlDirty(string key, object defaultValue, object value) {
			SetEnumProperty(key, defaultValue, value);
			MakeHtmlDirty();
		}
		protected override IStateManager[] GetStateManagedObjects() {
			return new IStateManager[] { ContentElementFiltering };
		}
		bool IHtmlFormattingSettings.AutoIndentation {
			get { return false; }
		}
		int IHtmlFormattingSettings.IndentSize {
			get { return 4; }
		}
		char IHtmlFormattingSettings.IndentChar {
			get { return ' '; }
		}
	}
}
namespace DevExpress.Web.ASPxHtmlEditor.Internal {
	public interface IHtmlFormattingSettings {
		bool AutoIndentation { get; }
		int IndentSize { get; }
		char IndentChar { get; }
	}
	public interface IHtmlValidationSettings {
		bool AllowScripts { get; }
		bool AllowIFrames { get; }
		bool AllowFormElements { get; }
		bool UpdateBoldItalic { get; }
		bool UpdateDeprecatedElements { get; }
		bool AllowIdAttributes { get; }
		bool AllowStyleAttributes { get; }
		AllowedDocumentType AllowedDocumentType { get; }
		bool AllowHTML5MediaElements { get; }
		bool AllowEditFullDocument { get; }
		bool AllowObjectAndEmbedElements { get; }
		bool AllowYouTubeVideoIFrames { get; }
		HtmlEditorContentElementFiltering ContentElementFiltering { get; }
		ResourcePathMode ResourcePathMode { get; }
	}
	public interface IHtmlProcessingSettings : IHtmlValidationSettings, IHtmlFormattingSettings { }
	public class HtmlEditingDefaultValues {
	}
}
