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

using DevExpress.Web.ASPxRichEdit.Internal;
using DevExpress.Web.Internal;
using DevExpress.XtraRichEdit;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Web.UI;
using DevExpress.Utils.Controls;
using System.Drawing;
namespace DevExpress.Web.ASPxRichEdit {
	public class ASPxRichEditBookmarkSettings : BookmarkOptions, ISettingsWithExternalStateManager {
		RichEditOptionsStateManager stateManager = new RichEditOptionsStateManager();
		RichEditOptionsStateManager ISettingsWithExternalStateManager.StateManager { get { return this.stateManager; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override bool AllowNameResolution {
			get { return stateManager.GetPropertyValue("AllowNameResolution", true); }
			set { stateManager.SetPropertyValue("AllowNameResolution", true, value); }
		}
		public override Color Color {
			get { return stateManager.GetPropertyValue("Color", BookmarkOptions.defaultColor); }
			set { stateManager.SetPropertyValue("Color", BookmarkOptions.defaultColor, value); }
		}
		[DefaultValue(RichEditBookmarkVisibility.Auto)]
		public override RichEditBookmarkVisibility Visibility {
			get { return stateManager.GetPropertyValue("Visibility", RichEditBookmarkVisibility.Auto); }
			set { stateManager.SetPropertyValue("Visibility", RichEditBookmarkVisibility.Auto, value); }
		}
		public override void Assign(BaseOptions options) {
			base.Assign(options);
			ASPxRichEditBookmarkSettings opt = options as ASPxRichEditBookmarkSettings;
			if(opt != null) {
				Color = opt.Color;
				Visibility = opt.Visibility;
				AllowNameResolution = opt.AllowNameResolution;
			}
		}
	}
	public class ASPxRichEditHorizontalRulerSettings : HorizontalRulerOptions, ISettingsWithExternalStateManager {
		RichEditOptionsStateManager stateManager = new RichEditOptionsStateManager();
		RichEditOptionsStateManager ISettingsWithExternalStateManager.StateManager { get { return this.stateManager; } }
		[DefaultValue(RichEditRulerVisibility.Auto)]
		public override RichEditRulerVisibility Visibility {
			get { return stateManager.GetPropertyValue("Visibility", RichEditRulerVisibility.Auto); }
			set { stateManager.SetPropertyValue("Visibility", RichEditRulerVisibility.Auto, value); }
		}
		[DefaultValue(true)]
		public override bool ShowLeftIndent {
			get { return stateManager.GetPropertyValue("ShowLeftIndent", true); }
			set { stateManager.SetPropertyValue("ShowLeftIndent", true, value); }
		}
		[DefaultValue(true)]
		public override bool ShowRightIndent {
			get { return stateManager.GetPropertyValue("ShowRightIndent", true); }
			set { stateManager.SetPropertyValue("ShowRightIndent", true, value); }
		}
		[DefaultValue(true)]
		public override bool ShowTabs {
			get { return stateManager.GetPropertyValue("ShowTabs", true); }
			set { stateManager.SetPropertyValue("ShowTabs", true, value); }
		}
		public override void Assign(BaseOptions options) {
			base.Assign(options);
			ASPxRichEditHorizontalRulerSettings opt = options as ASPxRichEditHorizontalRulerSettings;
			if(opt != null) {
				Visibility = opt.Visibility;
				ShowLeftIndent = opt.ShowLeftIndent;
				ShowRightIndent = opt.ShowRightIndent;
				ShowTabs = opt.ShowTabs;
			}
		}
	}
	public class ASPxRichEditBehaviorSettings : RichEditBehaviorOptions, ISettingsWithExternalStateManager {
		RichEditOptionsStateManager stateManager = new RichEditOptionsStateManager();
		RichEditOptionsStateManager ISettingsWithExternalStateManager.StateManager { get { return this.stateManager; } }
		[DefaultValue(DocumentCapability.Default)]
		public override DocumentCapability Printing {
			get { return stateManager.GetPropertyValue("Printing", DocumentCapability.Default); }
			set { stateManager.SetPropertyValue("Printing", DocumentCapability.Default, value); }
		}
		public override DocumentCapability Copy {
			get { return stateManager.GetPropertyValue("Copy", DocumentCapability.Default); }
			set { stateManager.SetPropertyValue("Copy", DocumentCapability.Default, value); }
		}
		[DefaultValue(DocumentCapability.Default)]
		public override DocumentCapability CreateNew {
			get { return stateManager.GetPropertyValue("CreateNew", DocumentCapability.Default); }
			set { stateManager.SetPropertyValue("CreateNew", DocumentCapability.Default, value); }
		}
		public override DocumentCapability Cut {
			get { return stateManager.GetPropertyValue("Cut", DocumentCapability.Default); }
			set { stateManager.SetPropertyValue("Cut", DocumentCapability.Default, value); }
		}
		[DefaultValue(DocumentCapability.Default)]
		public override DocumentCapability Open {
			get { return stateManager.GetPropertyValue("Open", DocumentCapability.Default); }
			set { stateManager.SetPropertyValue("Open", DocumentCapability.Default, value); }
		}
		public override DocumentCapability Paste {
			get { return stateManager.GetPropertyValue("Paste", DocumentCapability.Default); }
			set { stateManager.SetPropertyValue("Paste", DocumentCapability.Default, value); }
		}
		[DefaultValue(DocumentCapability.Default)]
		public override DocumentCapability Save {
			get { return stateManager.GetPropertyValue("Save", DocumentCapability.Default); }
			set { stateManager.SetPropertyValue("Save", DocumentCapability.Default, value); }
		}
		[DefaultValue(DocumentCapability.Default)]
		public override DocumentCapability SaveAs {
			get { return stateManager.GetPropertyValue("SaveAs", DocumentCapability.Default); }
			set { stateManager.SetPropertyValue("SaveAs", DocumentCapability.Default, value); }
		}
		public override DocumentCapability Drag {
			get { return stateManager.GetPropertyValue("Drag", DocumentCapability.Disabled); }
			set { stateManager.SetPropertyValue("Drag", DocumentCapability.Disabled, value); }
		}
		public override DocumentCapability Drop {
			get { return stateManager.GetPropertyValue("Drop", DocumentCapability.Disabled); }
			set { stateManager.SetPropertyValue("Drop", DocumentCapability.Disabled, value); }
		}
		public override string TabMarker {
			get { return stateManager.GetPropertyValue("TabMarker", "\t"); }
			set { stateManager.SetPropertyValue("TabMarker", "\t", value); }
		}
		public override PageBreakInsertMode PageBreakInsertMode {
			get { return stateManager.GetPropertyValue("PageBreakInsertMode", PageBreakInsertMode.NewLine); }
			set { stateManager.SetPropertyValue("PageBreakInsertMode", PageBreakInsertMode.NewLine, value); }
		}
		[DefaultValue(DocumentCapability.Default), NotifyParentProperty(true)]
		public DocumentCapability FullScreen {
			get { return stateManager.GetPropertyValue("FullScreen", DocumentCapability.Default); }
			set { stateManager.SetPropertyValue("FullScreen", DocumentCapability.Default, value); }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public override DocumentCapability ShowPopupMenu {
			get { return stateManager.GetPropertyValue("DocumentCapability", DocumentCapability.Default); }
			set { stateManager.SetPropertyValue("DocumentCapability", DocumentCapability.Default, value); }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public override LineBreakSubstitute PasteLineBreakSubstitution {
			get { return stateManager.GetPropertyValue("PasteLineBreakSubstitution", LineBreakSubstitute.None); }
			set { stateManager.SetPropertyValue("PasteLineBreakSubstitution", LineBreakSubstitute.None, value); }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public override DocumentCapability Touch {
			get { return stateManager.GetPropertyValue("Touch", DocumentCapability.Default); }
			set { stateManager.SetPropertyValue("Touch", DocumentCapability.Default, value); }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public override bool UseFontSubstitution {
			get { return stateManager.GetPropertyValue("UseFontSubstitution", true); }
			set { stateManager.SetPropertyValue("UseFontSubstitution", true, value); }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public override bool PasteSingleCellAsText {
			get { return stateManager.GetPropertyValue("PasteSingleCellAsText", false); }
			set { stateManager.SetPropertyValue("PasteSingleCellAsText", false, value); }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public override bool OvertypeAllowed {
			get { return stateManager.GetPropertyValue("OvertypeAllowed", true); }
			set { stateManager.SetPropertyValue("OvertypeAllowed", true, value); }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public override DocumentCapability OfficeScrolling {
			get { return stateManager.GetPropertyValue("OfficeScrolling", DocumentCapability.Default); }
			set { stateManager.SetPropertyValue("OfficeScrolling", DocumentCapability.Default, value); }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never),
		DefaultValue(DocumentCapability.Default)]
		public override DocumentCapability Zooming {
			get { return stateManager.GetPropertyValue("Zooming", DocumentCapability.Default); }
			set { stateManager.SetPropertyValue("Zooming", DocumentCapability.Default, value); }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public override float MaxZoomFactor {
			get { return stateManager.GetPropertyValue("MaxZoomFactor", DefaultMaxZoomFactor); }
			set {
				float newZoom = Math.Max(MinZoomFactor, value);
				stateManager.SetPropertyValue("MaxZoomFactor", DefaultMaxZoomFactor, newZoom);
			}
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public override float MinZoomFactor {
			get { return stateManager.GetPropertyValue("MinZoomFactor", DefaultMinZoomFactor); }
			set {
				float newZoom = Math.Max(DefaultMinZoomFactor, value);
				newZoom = Math.Min(newZoom, MaxZoomFactor);
				stateManager.SetPropertyValue("MinZoomFactor", DefaultMinZoomFactor, newZoom);
			}
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public override RichEditBaseValueSource FontSource {
			get { return stateManager.GetPropertyValue("FontSource", RichEditBaseValueSource.Auto); }
			set { stateManager.SetPropertyValue("FontSource", RichEditBaseValueSource.Auto, value); }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public override RichEditBaseValueSource ForeColorSource {
			get { return stateManager.GetPropertyValue("ForeColorSource", RichEditBaseValueSource.Auto); }
			set { stateManager.SetPropertyValue("ForeColorSource", RichEditBaseValueSource.Auto, value); }
		}
		public override void Assign(Utils.Controls.BaseOptions options) {
			base.Assign(options);
			var opt = options as ASPxRichEditBehaviorSettings;
			if(opt != null) {
				FullScreen = opt.FullScreen;
			}
		}
	}
	public class ASPxRichEditDocumentCapabilitiesSettings : DocumentCapabilitiesOptions, ISettingsWithExternalStateManager {
		RichEditOptionsStateManager stateManager = new RichEditOptionsStateManager();
		RichEditOptionsStateManager ISettingsWithExternalStateManager.StateManager { get { return this.stateManager; } }
		public override DocumentCapability Bookmarks {
			get { return stateManager.GetPropertyValue("Bookmarks", DocumentCapability.Default); }
			set { stateManager.SetPropertyValue("Bookmarks", DocumentCapability.Default, value); }
		}
		public override DocumentCapability CharacterFormatting {
			get { return stateManager.GetPropertyValue("CharacterFormatting", DocumentCapability.Default); }
			set { stateManager.SetPropertyValue("CharacterFormatting", DocumentCapability.Default, value); }
		}
		public override DocumentCapability CharacterStyle {
			get { return stateManager.GetPropertyValue("CharacterStyle", DocumentCapability.Default); }
			set { stateManager.SetPropertyValue("CharacterStyle", DocumentCapability.Default, value); }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public override DocumentCapability Comments {
			get { return stateManager.GetPropertyValue("Comments", DocumentCapability.Default); }
			set { stateManager.SetPropertyValue("Comments", DocumentCapability.Default, value); }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public override DocumentCapability EndNotes {
			get { return stateManager.GetPropertyValue("EndNotes", DocumentCapability.Default); }
			set { stateManager.SetPropertyValue("EndNotes", DocumentCapability.Default, value); }
		}
		public override DocumentCapability Fields {
			get { return stateManager.GetPropertyValue("Fields", DocumentCapability.Default); }
			set { stateManager.SetPropertyValue("Fields", DocumentCapability.Default, value); }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public override DocumentCapability FloatingObjects {
			get { return stateManager.GetPropertyValue("FloatingObjects", DocumentCapability.Default); }
			set { stateManager.SetPropertyValue("FloatingObjects", DocumentCapability.Default, value); }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public override DocumentCapability FootNotes {
			get { return stateManager.GetPropertyValue("FootNotes", DocumentCapability.Default); }
			set { stateManager.SetPropertyValue("FootNotes", DocumentCapability.Default, value); }
		}
		public override DocumentCapability HeadersFooters {
			get { return stateManager.GetPropertyValue("HeadersFooters", DocumentCapability.Default); }
			set { stateManager.SetPropertyValue("HeadersFooters", DocumentCapability.Default, value); }
		}
		public override DocumentCapability Hyperlinks {
			get { return stateManager.GetPropertyValue("Hyperlinks", DocumentCapability.Default); }
			set { stateManager.SetPropertyValue("Hyperlinks", DocumentCapability.Default, value); }
		}
		public override DocumentCapability InlinePictures {
			get { return stateManager.GetPropertyValue("InlinePictures", DocumentCapability.Default); }
			set { stateManager.SetPropertyValue("InlinePictures", DocumentCapability.Default, value); }
		}
		public override DocumentCapability ParagraphFormatting {
			get { return stateManager.GetPropertyValue("ParagraphFormatting", DocumentCapability.Default); }
			set { stateManager.SetPropertyValue("ParagraphFormatting", DocumentCapability.Default, value); }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public override DocumentCapability ParagraphFrames {
			get { return stateManager.GetPropertyValue("ParagraphFrames", DocumentCapability.Default); }
			set { stateManager.SetPropertyValue("ParagraphFrames", DocumentCapability.Default, value); }
		}
		public override DocumentCapability ParagraphStyle {
			get { return stateManager.GetPropertyValue("ParagraphStyle", DocumentCapability.Default); }
			set { stateManager.SetPropertyValue("ParagraphStyle", DocumentCapability.Default, value); }
		}
		public override DocumentCapability Paragraphs {
			get { return stateManager.GetPropertyValue("Paragraphs", DocumentCapability.Default); }
			set { stateManager.SetPropertyValue("Paragraphs", DocumentCapability.Default, value); }
		}
		public override DocumentCapability ParagraphTabs {
			get { return stateManager.GetPropertyValue("ParagraphTabs", DocumentCapability.Default); }
			set { stateManager.SetPropertyValue("ParagraphTabs", DocumentCapability.Default, value); }
		}
		public override DocumentCapability Sections {
			get { return stateManager.GetPropertyValue("Sections", DocumentCapability.Default); }
			set { stateManager.SetPropertyValue("Sections", DocumentCapability.Default, value); }
		}
		public override DocumentCapability Tables {
			get { return stateManager.GetPropertyValue("Tables", DocumentCapability.Default); }
			set { stateManager.SetPropertyValue("Tables", DocumentCapability.Default, value); }
		}
		public override DocumentCapability TableStyle {
			get { return stateManager.GetPropertyValue("TableStyle", DocumentCapability.Default); }
			set { stateManager.SetPropertyValue("TableStyle", DocumentCapability.Default, value); }
		}
		public override DocumentCapability TabSymbol {
			get { return stateManager.GetPropertyValue("TabSymbol", DocumentCapability.Default); }
			set { stateManager.SetPropertyValue("TabSymbol", DocumentCapability.Default, value); }
		}
		public override DocumentCapability Undo {
			get { return stateManager.GetPropertyValue("Undo", DocumentCapability.Default); }
			set { stateManager.SetPropertyValue("Undo", DocumentCapability.Default, value); }
		}
		protected internal override CharacterFormattingDetailedOptions CreateCharacterFormattingDetailedOptions() {
			var options = new ASPxRichEditCharacterFormattingDetailedSettings();
			stateManager.RegisterChildSettings(options);
			return options;
		}
		protected internal override NumberingOptions CreateNumberingOptions() {
			var options = new ASPxRichEditNumberingSettings();
			stateManager.RegisterChildSettings(options);
			return options;
		}
	}
	public class ASPxRichEditNumberingSettings : NumberingOptions, ISettingsWithExternalStateManager {
		RichEditOptionsStateManager stateManager = new RichEditOptionsStateManager();
		RichEditOptionsStateManager ISettingsWithExternalStateManager.StateManager { get { return this.stateManager; } }
		public override DocumentCapability Bulleted {
			get { return stateManager.GetPropertyValue("Bulleted", DocumentCapability.Default); }
			set { stateManager.SetPropertyValue("Bulleted", DocumentCapability.Default, value); }
		}
		public override DocumentCapability MultiLevel {
			get { return stateManager.GetPropertyValue("MultiLevel", DocumentCapability.Default); }
			set { stateManager.SetPropertyValue("MultiLevel", DocumentCapability.Default, value); }
		}
		public override DocumentCapability Simple {
			get { return stateManager.GetPropertyValue("Simple", DocumentCapability.Default); }
			set { stateManager.SetPropertyValue("Simple", DocumentCapability.Default, value); }
		}
	}
	public class ASPxRichEditCharacterFormattingDetailedSettings : CharacterFormattingDetailedOptions, ISettingsWithExternalStateManager {
		RichEditOptionsStateManager stateManager = new RichEditOptionsStateManager();
		RichEditOptionsStateManager ISettingsWithExternalStateManager.StateManager { 
			get { return this.stateManager; } 
		}
		[DefaultValue(CharacterFormattingDetailedOptions.Mask.Default)]
		protected internal override CharacterFormattingDetailedOptions.Mask Val {
			get { return stateManager.GetPropertyValue("Val", CharacterFormattingDetailedOptions.Mask.Default); }
			set { stateManager.SetPropertyValue("Val", CharacterFormattingDetailedOptions.Mask.Default, value); }
		}
	}
}
namespace DevExpress.Web.ASPxRichEdit.Internal {
	public interface ISettingsWithExternalStateManager {
		RichEditOptionsStateManager StateManager { get; }
	}
	public class RichEditOptionsStateManager : StateManager {
		List<ISettingsWithExternalStateManager> childSettings = new List<ISettingsWithExternalStateManager>();
		protected internal void RegisterChildSettings(ISettingsWithExternalStateManager settings) {
			childSettings.Add(settings);
		}
		protected internal T GetPropertyValue<T>(string key, T defaultValue) {
			return (T)GetObjectProperty(key, defaultValue);
		}
		protected internal void SetPropertyValue<T>(string key, T defaultValue, T value) {
			SetObjectProperty(key, defaultValue, value);
		}
		protected override IStateManager[] GetStateManagedObjects() {
			return ViewStateUtils.GetMergedStateManagedObjects(base.GetStateManagedObjects(), childSettings.Select(s => s.StateManager).ToArray());
		}
	}
}
