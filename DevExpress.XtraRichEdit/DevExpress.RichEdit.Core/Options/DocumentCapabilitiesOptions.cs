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
using System.ComponentModel;
using System.Runtime.InteropServices;
using DevExpress.Utils.Controls;
using DevExpress.Compatibility.System.ComponentModel;
namespace DevExpress.XtraRichEdit {
	[ComVisible(true)]
	public class DocumentCapabilitiesOptions : RichEditNotificationOptions {
		#region Fields
		DocumentCapability paragraphFrames;
		DocumentCapability characterFormatting;
		DocumentCapability paragraphFormatting;
		DocumentCapability inlinePictures;
		DocumentCapability paragraphs;
		DocumentCapability characterStyle;
		DocumentCapability paragraphStyle;
		DocumentCapability tableStyle;
		DocumentCapability tableCellStyle;
		DocumentCapability hyperlinks;
		DocumentCapability bookmarks;
		DocumentCapability paragraphTabs;
		DocumentCapability tabSymbol;
		DocumentCapability undo;
		DocumentCapability sections;
		DocumentCapability headersFooters;
		DocumentCapability tables;
		DocumentCapability footNotes;
		DocumentCapability endNotes;
		DocumentCapability floatingObjects;
		DocumentCapability fields;
		DocumentCapability comments;
		#endregion
		NumberingOptions numbering;
		CharacterFormattingDetailedOptions characterFormattingDetailed;
		public DocumentCapabilitiesOptions() {
			numbering.Changed += OnInnerOptionsChanged;
			characterFormattingDetailed.Changed += OnInnerOptionsChanged;
		}
		protected internal virtual void OnInnerOptionsChanged(object sender, BaseOptionChangedEventArgs e) {
			OnChanged(e);
		}
		#region Properties
		[DefaultValue(DocumentCapability.Default), NotifyParentProperty(true)]
		public virtual DocumentCapability ParagraphFrames {
			get { return paragraphFrames; }
			set {
				if (this.paragraphFrames == value)
					return;
				DocumentCapability oldValue = this.paragraphFrames;
				this.paragraphFrames = value;
				OnChanged("ParagraphFrames", oldValue, value);
			}
		}
		[
#if !SL
	DevExpressRichEditCoreLocalizedDescription("DocumentCapabilitiesOptionsCharacterFormatting"),
#endif
DefaultValue(DocumentCapability.Default), NotifyParentProperty(true)]
		public virtual DocumentCapability CharacterFormatting {
			get { return characterFormatting; }
			set {
				if (this.characterFormatting == value)
					return;
				DocumentCapability oldValue = this.characterFormatting;
				this.characterFormatting = value;
				OnChanged("CharacterFormatting", oldValue, value);
			}
		}
		[NotifyParentProperty(true), Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		internal CharacterFormattingDetailedOptions CharacterFormattingDetailed { get { return characterFormattingDetailed; } }
		[
#if !SL
	DevExpressRichEditCoreLocalizedDescription("DocumentCapabilitiesOptionsParagraphFormatting"),
#endif
DefaultValue(DocumentCapability.Default), NotifyParentProperty(true)]
		public virtual DocumentCapability ParagraphFormatting {
			get { return paragraphFormatting; }
			set {
				if (this.paragraphFormatting == value)
					return;
				DocumentCapability oldValue = this.paragraphFormatting;
				this.paragraphFormatting = value;
				OnChanged("ParagraphFormatting", oldValue, value);
			}
		}
		[
#if !SL
	DevExpressRichEditCoreLocalizedDescription("DocumentCapabilitiesOptionsInlinePictures"),
#endif
DefaultValue(DocumentCapability.Default), NotifyParentProperty(true)]
		public virtual DocumentCapability InlinePictures {
			get { return inlinePictures; }
			set {
				if (this.inlinePictures == value)
					return;
				DocumentCapability oldValue = this.inlinePictures;
				this.inlinePictures = value;
				OnChanged("InlinePictures", oldValue, value);
			}
		}
		[
#if !SL
	DevExpressRichEditCoreLocalizedDescription("DocumentCapabilitiesOptionsParagraphs"),
#endif
DefaultValue(DocumentCapability.Default), NotifyParentProperty(true)]
		public virtual DocumentCapability Paragraphs {
			get { return paragraphs; }
			set {
				if (this.paragraphs == value)
					return;
				DocumentCapability oldValue = this.paragraphs;
				this.paragraphs = value;
				OnChanged("Paragraphs", oldValue, value);
			}
		}
		[
#if !SL
	DevExpressRichEditCoreLocalizedDescription("DocumentCapabilitiesOptionsCharacterStyle"),
#endif
DefaultValue(DocumentCapability.Default), NotifyParentProperty(true)]
		public virtual DocumentCapability CharacterStyle {
			get { return characterStyle; }
			set {
				if (this.characterStyle == value)
					return;
				DocumentCapability oldValue = this.characterStyle;
				this.characterStyle = value;
				OnChanged("CharacterStyle", oldValue, value);
			}
		}
		[
#if !SL
	DevExpressRichEditCoreLocalizedDescription("DocumentCapabilitiesOptionsParagraphStyle"),
#endif
DefaultValue(DocumentCapability.Default), NotifyParentProperty(true)]
		public virtual DocumentCapability ParagraphStyle {
			get { return paragraphStyle; }
			set {
				if (this.paragraphStyle == value)
					return;
				DocumentCapability oldValue = this.paragraphStyle;
				this.paragraphStyle = value;
				OnChanged("ParagraphStyle", oldValue, value);
			}
		}
		[
#if !SL
	DevExpressRichEditCoreLocalizedDescription("DocumentCapabilitiesOptionsTableStyle"),
#endif
DefaultValue(DocumentCapability.Default), NotifyParentProperty(true)]
		public virtual DocumentCapability TableStyle {
			get { return tableStyle; }
			set {
				if (this.tableStyle == value)
					return;
				DocumentCapability oldValue = this.tableStyle;
				this.tableStyle = value;
				OnChanged("TableStyle", oldValue, value);
			}
		}
		[
DefaultValue(DocumentCapability.Default), NotifyParentProperty(true)]
		internal DocumentCapability TableCellStyle {
			get { return tableCellStyle; }
			set {
				if (this.tableCellStyle == value)
					return;
				DocumentCapability oldValue = this.tableCellStyle;
				this.tableCellStyle = value;
				OnChanged("TableCellStyle", oldValue, value);
			}
		}
		[
#if !SL
	DevExpressRichEditCoreLocalizedDescription("DocumentCapabilitiesOptionsHyperlinks"),
#endif
DefaultValue(DocumentCapability.Default), NotifyParentProperty(true)]
		public virtual DocumentCapability Hyperlinks {
			get { return hyperlinks; }
			set {
				if (this.hyperlinks == value)
					return;
				DocumentCapability oldValue = this.hyperlinks;
				this.hyperlinks = value;
				OnChanged("Hyperlinks", oldValue, value);
			}
		}
		[
#if !SL
	DevExpressRichEditCoreLocalizedDescription("DocumentCapabilitiesOptionsBookmarks"),
#endif
DefaultValue(DocumentCapability.Default), NotifyParentProperty(true)]
		public virtual DocumentCapability Bookmarks {
			get { return bookmarks; }
			set {
				if (this.bookmarks == value)
					return;
				DocumentCapability oldValue = this.bookmarks;
				this.bookmarks = value;
				OnChanged("Bookmarks", oldValue, value);
			}
		}
		[
#if !SL
	DevExpressRichEditCoreLocalizedDescription("DocumentCapabilitiesOptionsParagraphTabs"),
#endif
DefaultValue(DocumentCapability.Default), NotifyParentProperty(true)]
		public virtual DocumentCapability ParagraphTabs {
			get { return paragraphTabs; }
			set {
				if (this.paragraphTabs == value)
					return;
				DocumentCapability oldValue = this.paragraphTabs;
				this.paragraphTabs = value;
				OnChanged("ParagraphTabs", oldValue, value);
			}
		}
		[
#if !SL
	DevExpressRichEditCoreLocalizedDescription("DocumentCapabilitiesOptionsTabSymbol"),
#endif
DefaultValue(DocumentCapability.Default), NotifyParentProperty(true)]
		public virtual DocumentCapability TabSymbol {
			get { return tabSymbol; }
			set {
				if (this.tabSymbol == value)
					return;
				DocumentCapability oldValue = this.tabSymbol;
				this.tabSymbol = value;
				OnChanged("TabSymbol", oldValue, value);
			}
		}
		[
#if !SL
	DevExpressRichEditCoreLocalizedDescription("DocumentCapabilitiesOptionsUndo"),
#endif
DefaultValue(DocumentCapability.Default), NotifyParentProperty(true)]
		public virtual DocumentCapability Undo {
			get { return undo; }
			set {
				if (this.undo == value)
					return;
				DocumentCapability oldValue = this.undo;
				this.undo = value;
				OnChanged("Undo", oldValue, value);
			}
		}
		[
#if !SL
	DevExpressRichEditCoreLocalizedDescription("DocumentCapabilitiesOptionsSections"),
#endif
DefaultValue(DocumentCapability.Default), NotifyParentProperty(true)]
		public virtual DocumentCapability Sections {
			get { return sections; }
			set {
				if (this.sections == value)
					return;
				DocumentCapability oldValue = this.sections;
				this.sections = value;
				OnChanged("Sections", oldValue, value);
			}
		}
		[
#if !SL
	DevExpressRichEditCoreLocalizedDescription("DocumentCapabilitiesOptionsHeadersFooters"),
#endif
DefaultValue(DocumentCapability.Default), NotifyParentProperty(true)]
		public virtual DocumentCapability HeadersFooters {
			get { return headersFooters; }
			set {
				if (this.headersFooters == value)
					return;
				DocumentCapability oldValue = this.headersFooters;
				this.headersFooters = value;
				OnChanged("HeadersFooters", oldValue, value);
			}
		}
		[
#if !SL
	DevExpressRichEditCoreLocalizedDescription("DocumentCapabilitiesOptionsTables"),
#endif
DefaultValue(DocumentCapability.Default), NotifyParentProperty(true)]
		public virtual DocumentCapability Tables {
			get { return tables; }
			set {
				if (this.tables == value)
					return;
				DocumentCapability oldValue = this.tables;
				this.tables = value;
				OnChanged("Tables", oldValue, value);
			}
		}
		[
#if !SL
	DevExpressRichEditCoreLocalizedDescription("DocumentCapabilitiesOptionsFootNotes"),
#endif
DefaultValue(DocumentCapability.Default), NotifyParentProperty(true)]
		public virtual DocumentCapability FootNotes {
			get { return footNotes; }
			set {
				if (footNotes == value)
					return;
				DocumentCapability oldValue = this.footNotes;
				footNotes = value;
				OnChanged("FootNotes", oldValue, value);
			}
		}
		[
#if !SL
	DevExpressRichEditCoreLocalizedDescription("DocumentCapabilitiesOptionsEndNotes"),
#endif
DefaultValue(DocumentCapability.Default), NotifyParentProperty(true)]
		public virtual DocumentCapability EndNotes {
			get { return endNotes; }
			set {
				if (endNotes == value)
					return;
				DocumentCapability oldValue = this.endNotes;
				endNotes = value;
				OnChanged("EndNotes", oldValue, value);
			}
		}
		[
#if !SL
	DevExpressRichEditCoreLocalizedDescription("DocumentCapabilitiesOptionsFloatingObjects"),
#endif
 DefaultValue(DocumentCapability.Default), NotifyParentProperty(true)]
		public virtual DocumentCapability FloatingObjects {
			get { return floatingObjects; }
			set {
				if (floatingObjects == value)
					return;
				DocumentCapability oldValue = this.floatingObjects;
				floatingObjects = value;
				OnChanged("FloatingObjects", oldValue, value);
			}
		}
		[
#if !SL
	DevExpressRichEditCoreLocalizedDescription("DocumentCapabilitiesOptionsComments"),
#endif
	   DefaultValue(DocumentCapability.Default), NotifyParentProperty(true)]
		public virtual DocumentCapability Comments {
			get { return comments; }
			set {
				if (this.comments == value)
					return;
				DocumentCapability oldValue = this.comments;
				this.comments = value;
				OnChanged("Comments", oldValue, value);
			}
		}
		[DefaultValue(DocumentCapability.Default), NotifyParentProperty(true)]
		public virtual DocumentCapability Fields
		{
			get { return fields; }
			set
			{
				if (this.fields == value)
					return;
				DocumentCapability oldValue = this.fields;
				this.fields = value;
				OnChanged("Fields", oldValue, value);
			}
		}
		#region ListsOptions
		[
#if !SL
	DevExpressRichEditCoreLocalizedDescription("DocumentCapabilitiesOptionsNumbering"),
#endif
NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual NumberingOptions Numbering { get { return this.numbering; } }
		#endregion
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool ParagraphFramesAllowed { get { return IsAllowed(ParagraphFrames); } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool CharacterFormattingAllowed { get { return IsAllowed(CharacterFormatting); } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool ParagraphFormattingAllowed { get { return IsAllowed(ParagraphFormatting); } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool InlinePicturesAllowed { get { return IsAllowed(InlinePictures); } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool ParagraphsAllowed { get { return IsAllowed(Paragraphs); } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool CharacterStyleAllowed { get { return IsAllowed(CharacterStyle); } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool ParagraphStyleAllowed { get { return IsAllowed(ParagraphStyle); } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool TableStyleAllowed { get { return IsAllowed(TableStyle); } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		internal bool TableCellStyleAllowed { get { return IsAllowed(TableCellStyle); } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool HyperlinksAllowed { get { return IsAllowed(Hyperlinks); } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool BookmarksAllowed { get { return IsAllowed(Bookmarks); } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool ParagraphTabsAllowed { get { return IsAllowed(ParagraphTabs); } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool TabSymbolAllowed { get { return IsAllowed(TabSymbol); } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool UndoAllowed { get { return IsAllowed(Undo); } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool SectionsAllowed { get { return IsAllowed(Sections); } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool HeadersFootersAllowed { get { return IsAllowed(HeadersFooters); } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool TablesAllowed { get { return IsAllowed(Tables); } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool FootNotesAllowed { get { return IsAllowed(FootNotes); } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool EndNotesAllowed { get { return IsAllowed(EndNotes); } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool FloatingObjectsAllowed { get { return IsAllowed(FloatingObjects); } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool CommentsAllowed { get { return IsAllowed(Comments); } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool FieldsAllowed { get { return IsAllowed(Fields); } }
		#endregion
		bool IsAllowed(DocumentCapability option) { return option == DocumentCapability.Default || option == DocumentCapability.Enabled; }
		protected override void CreateInnerOptions() {
			this.numbering = CreateNumberingOptions();
			this.characterFormattingDetailed = CreateCharacterFormattingDetailedOptions();
		}
		protected internal virtual NumberingOptions CreateNumberingOptions() {
			return new NumberingOptions();
		}
		protected internal virtual CharacterFormattingDetailedOptions CreateCharacterFormattingDetailedOptions() {
			return new CharacterFormattingDetailedOptions();
		}
		public override void Assign(BaseOptions options) {
			base.Assign(options);
			DocumentCapabilitiesOptions opt = options as DocumentCapabilitiesOptions;
			if(opt != null) {
				this.CharacterFormatting = opt.CharacterFormatting;
				this.ParagraphFormatting = opt.ParagraphFormatting;
				this.InlinePictures = opt.InlinePictures;
				this.Paragraphs = opt.Paragraphs;
				this.CharacterStyle = opt.CharacterStyle;
				this.ParagraphStyle = opt.ParagraphStyle;
				this.TableStyle = opt.TableStyle;
				this.Hyperlinks = opt.Hyperlinks;
				this.Bookmarks = opt.Bookmarks;
				this.ParagraphTabs = opt.ParagraphTabs;
				this.TabSymbol = opt.TabSymbol;
				this.Undo = opt.Undo;
				this.Sections = opt.Sections;
				this.HeadersFooters = opt.HeadersFooters;
				this.Tables = opt.Tables;
				this.FootNotes = opt.FootNotes;
				this.EndNotes = opt.EndNotes;
				this.FloatingObjects = opt.FloatingObjects;
				this.Comments = opt.Comments;
				this.Fields = opt.Fields;
				this.ParagraphFrames = opt.ParagraphFrames;
				this.Numbering.Assign(opt.Numbering);
				this.CharacterFormattingDetailed.Assign(opt.CharacterFormattingDetailed);
			}
		}
		public void CopyFrom(DocumentCapabilitiesOptions documentCapabilitiesOptions) {
			this.CharacterFormatting = documentCapabilitiesOptions.CharacterFormatting;
			this.ParagraphFormatting = documentCapabilitiesOptions.ParagraphFormatting;
			this.InlinePictures = documentCapabilitiesOptions.InlinePictures;
			this.Paragraphs = documentCapabilitiesOptions.Paragraphs;
			this.CharacterStyle = documentCapabilitiesOptions.CharacterStyle;
			this.ParagraphStyle = documentCapabilitiesOptions.ParagraphStyle;
			this.TableStyle = documentCapabilitiesOptions.TableStyle;
			this.Hyperlinks = documentCapabilitiesOptions.Hyperlinks;
			this.Bookmarks = documentCapabilitiesOptions.Bookmarks;
			this.ParagraphTabs = documentCapabilitiesOptions.ParagraphTabs;
			this.TabSymbol = documentCapabilitiesOptions.TabSymbol;
			this.Undo = documentCapabilitiesOptions.Undo;
			this.Sections = documentCapabilitiesOptions.Sections;
			this.HeadersFooters = documentCapabilitiesOptions.HeadersFooters;
			this.Tables = documentCapabilitiesOptions.Tables;
			this.FootNotes = documentCapabilitiesOptions.FootNotes;
			this.EndNotes = documentCapabilitiesOptions.EndNotes;
			this.FloatingObjects = documentCapabilitiesOptions.FloatingObjects;
			this.Comments = documentCapabilitiesOptions.Comments;
			this.Fields = documentCapabilitiesOptions.Fields;
			this.Numbering.CopyFrom(documentCapabilitiesOptions.Numbering);
			this.CharacterFormattingDetailed.CopyFrom(documentCapabilitiesOptions.CharacterFormattingDetailed);
		}
		protected internal override void ResetCore() {
			this.CharacterFormatting = DocumentCapability.Default;
			this.ParagraphFormatting = DocumentCapability.Default;
			this.InlinePictures = DocumentCapability.Default;
			this.Paragraphs = DocumentCapability.Default;
			this.CharacterStyle = DocumentCapability.Default;
			this.ParagraphStyle = DocumentCapability.Default;
			this.TableStyle = DocumentCapability.Default;
			this.Hyperlinks = DocumentCapability.Default;
			this.Bookmarks = DocumentCapability.Default;
			this.ParagraphTabs = DocumentCapability.Default;
			this.TabSymbol = DocumentCapability.Default;
			this.Undo = DocumentCapability.Default;
			this.Sections = DocumentCapability.Default;
			this.HeadersFooters = DocumentCapability.Default;
			this.Tables = DocumentCapability.Default;
			this.FootNotes = DocumentCapability.Default;
			this.EndNotes = DocumentCapability.Default;
			this.FloatingObjects = DocumentCapability.Default;
			this.Numbering.ResetCore();
			this.CharacterFormattingDetailed.ResetCore();
			this.Comments = DocumentCapability.Default;
			this.Fields = DocumentCapability.Default;
		}
	}
	[ComVisible(true)]
	public class NumberingOptions : RichEditNotificationOptions {
		DocumentCapability bulleted;
		DocumentCapability simple;
		DocumentCapability multiLevel;
		#region Properties
		[
#if !SL
	DevExpressRichEditCoreLocalizedDescription("NumberingOptionsBulleted"),
#endif
DefaultValue(DocumentCapability.Default), NotifyParentProperty(true)]
		public virtual DocumentCapability Bulleted {
			get { return bulleted; }
			set {
				if (this.bulleted == value)
					return;
				DocumentCapability oldValue = this.bulleted;
				this.bulleted = value;
				OnChanged("Bulleted", oldValue, value);
			}
		}
		[
#if !SL
	DevExpressRichEditCoreLocalizedDescription("NumberingOptionsSimple"),
#endif
DefaultValue(DocumentCapability.Default), NotifyParentProperty(true)]
		public virtual DocumentCapability Simple {
			get { return simple; }
			set {
				if (this.simple == value)
					return;
				DocumentCapability oldValue = this.simple;
				this.simple = value;
				OnChanged("Simple", oldValue, value);
			}
		}
		[
#if !SL
	DevExpressRichEditCoreLocalizedDescription("NumberingOptionsMultiLevel"),
#endif
DefaultValue(DocumentCapability.Default), NotifyParentProperty(true)]
		public virtual DocumentCapability MultiLevel {
			get { return multiLevel; }
			set {
				if (this.multiLevel == value)
					return;
				DocumentCapability oldValue = this.multiLevel;
				this.multiLevel = value;
				OnChanged("MultiLevel", oldValue, value);
			}
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool BulletedAllowed { get { return IsAllowed(Bulleted); } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool SimpleAllowed { get { return IsAllowed(Simple); } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool MultiLevelAllowed { get { return IsAllowed(MultiLevel); } }
		#endregion
		bool IsAllowed(DocumentCapability option) { return option == DocumentCapability.Default || option == DocumentCapability.Enabled; }
		public void CopyFrom(NumberingOptions numberingOptions) {
			this.bulleted = numberingOptions.Bulleted;
			this.simple = numberingOptions.Simple;
			this.multiLevel = numberingOptions.MultiLevel;
		}
		protected internal override void ResetCore() {
			this.bulleted = DocumentCapability.Default;
			this.multiLevel = DocumentCapability.Default;
			this.simple = DocumentCapability.Default;
		}
		public override void Assign(BaseOptions options) {
			base.Assign(options);
			var opt = options as NumberingOptions;
			if(opt != null) {
				Bulleted = opt.Bulleted;
				MultiLevel = opt.MultiLevel;
				Simple = opt.Simple;
			}
		}
	}
	[ComVisible(true)]
	public class CharacterFormattingDetailedOptions : RichEditNotificationOptions {
		#region Mask
		[Flags]
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2217")]
		public enum Mask : long {
			FontName = 0x00000001,
			FontSize = 0x00000002,
			FontBold = 0x00000004,
			FontItalic = 0x00000008,
			FontStrikeout = 0x00000010,
			FontUnderline = 0x00000020,
			AllCaps = 0x00000040,
			ForeColor = 0x00000080,
			BackColor = 0x00000100,
			UnderlineColor = 0x00000200,
			StrikeoutColor = 0x00000400,
			UnderlineWordsOnly = 0x00000800,
			StrikeoutWordsOnly = 0x00001000,
			Script = 0x00002000,
			Hidden = 0x00004000,
			HiddenFontName = 0x00010000,
			HiddenFontSize = 0x00020000,
			HiddenFontBold = 0x00040000,
			HiddenFontItalic = 0x00080000,
			HiddenFontStrikeout = 0x00100000,
			HiddenFontUnderline = 0x00200000,
			HiddenAllCaps = 0x00400000,
			HiddenForeColor = 0x00800000,
			HiddenBackColor = 0x01000000,
			HiddenUnderlineColor = 0x02000000,
			HiddenStrikeoutColor = 0x04000000,
			HiddenUnderlineWordsOnly = 0x08000000,
			HiddenStrikeoutWordsOnly = 0x10000000,
			HiddenScript = 0x20000000,
			HiddenHidden = 0x40000000,
			Default = 0x7FFFFFFF
		}
		#endregion
		Mask val = Mask.Default;
		protected internal virtual Mask Val { get { return val; } set { val = value; } }
		#region Properties
		[
DefaultValue(DocumentCapability.Default), NotifyParentProperty(true)]
		public virtual DocumentCapability FontName {
			get { return GetVal(Mask.FontName, Mask.HiddenFontName); }
			set {
				if (this.FontName == value)
					return;
				DocumentCapability oldValue = this.FontName;
				SetVal(Mask.FontName, Mask.HiddenFontName, value);
				OnChanged("FontName", oldValue, value);
			}
		}
		[
DefaultValue(DocumentCapability.Default), NotifyParentProperty(true)]
		public virtual DocumentCapability FontSize {
			get { return GetVal(Mask.FontSize, Mask.HiddenFontSize); }
			set {
				if (this.FontSize == value)
					return;
				DocumentCapability oldValue = this.FontSize;
				SetVal(Mask.FontSize, Mask.HiddenFontSize, value);
				OnChanged("FontSize", oldValue, value);
			}
		}
		[
DefaultValue(DocumentCapability.Default), NotifyParentProperty(true)]
		public virtual DocumentCapability FontBold {
			get { return GetVal(Mask.FontBold, Mask.HiddenFontBold); }
			set {
				if (this.FontBold == value)
					return;
				DocumentCapability oldValue = this.FontBold;
				SetVal(Mask.FontBold, Mask.HiddenFontBold, value);
				OnChanged("FontBold", oldValue, value);
			}
		}
		[
DefaultValue(DocumentCapability.Default), NotifyParentProperty(true)]
		public virtual DocumentCapability FontItalic {
			get { return GetVal(Mask.FontItalic, Mask.HiddenFontItalic); }
			set {
				if (this.FontItalic == value)
					return;
				DocumentCapability oldValue = this.FontItalic;
				SetVal(Mask.FontItalic, Mask.HiddenFontItalic, value);
				OnChanged("FontItalic", oldValue, value);
			}
		}
		[
DefaultValue(DocumentCapability.Default), NotifyParentProperty(true)]
		public virtual DocumentCapability FontStrikeout {
			get { return GetVal(Mask.FontStrikeout, Mask.HiddenFontStrikeout); }
			set {
				if (this.FontStrikeout == value)
					return;
				DocumentCapability oldValue = this.FontStrikeout;
				SetVal(Mask.FontStrikeout, Mask.HiddenFontStrikeout, value);
				OnChanged("FontStrikeoutType", oldValue, value);
			}
		}
		[
DefaultValue(DocumentCapability.Default), NotifyParentProperty(true)]
		public virtual DocumentCapability FontUnderline {
			get { return GetVal(Mask.FontUnderline, Mask.HiddenFontUnderline); }
			set {
				if (this.FontUnderline == value)
					return;
				DocumentCapability oldValue = this.FontUnderline;
				SetVal(Mask.FontUnderline, Mask.HiddenFontUnderline, value);
				OnChanged("FontUnderlineType", oldValue, value);
			}
		}
		[
DefaultValue(DocumentCapability.Default), NotifyParentProperty(true)]
		public virtual DocumentCapability AllCaps {
			get { return GetVal(Mask.AllCaps, Mask.HiddenAllCaps); }
			set {
				if (this.AllCaps == value)
					return;
				DocumentCapability oldValue = this.AllCaps;
				SetVal(Mask.AllCaps, Mask.HiddenAllCaps, value);
				OnChanged("AllCaps", oldValue, value);
			}
		}
		[
DefaultValue(DocumentCapability.Default), NotifyParentProperty(true)]
		public virtual DocumentCapability UnderlineWordsOnly {
			get { return GetVal(Mask.UnderlineWordsOnly, Mask.HiddenUnderlineWordsOnly); }
			set {
				if (this.UnderlineWordsOnly == value)
					return;
				DocumentCapability oldValue = this.UnderlineWordsOnly;
				SetVal(Mask.UnderlineWordsOnly, Mask.HiddenUnderlineWordsOnly, value);
				OnChanged("UnderlineWordsOnly", oldValue, value);
			}
		}
		[
DefaultValue(DocumentCapability.Default), NotifyParentProperty(true)]
		public virtual DocumentCapability StrikeoutWordsOnly {
			get { return GetVal(Mask.StrikeoutWordsOnly, Mask.HiddenStrikeoutWordsOnly); }
			set {
				if (this.StrikeoutWordsOnly == value)
					return;
				DocumentCapability oldValue = this.StrikeoutWordsOnly;
				SetVal(Mask.StrikeoutWordsOnly, Mask.HiddenStrikeoutWordsOnly, value);
				OnChanged("StrikeoutWordsOnly", oldValue, value);
			}
		}
		[
DefaultValue(DocumentCapability.Default), NotifyParentProperty(true)]
		public virtual DocumentCapability ForeColor {
			get { return GetVal(Mask.ForeColor, Mask.HiddenForeColor); }
			set {
				if (this.ForeColor == value)
					return;
				DocumentCapability oldValue = this.ForeColor;
				SetVal(Mask.ForeColor, Mask.HiddenForeColor, value);
				OnChanged("ForeColor", oldValue, value);
			}
		}
		[
DefaultValue(DocumentCapability.Default), NotifyParentProperty(true)]
		public virtual DocumentCapability BackColor {
			get { return GetVal(Mask.BackColor, Mask.HiddenBackColor); }
			set {
				if (this.BackColor == value)
					return;
				DocumentCapability oldValue = this.BackColor;
				SetVal(Mask.BackColor, Mask.HiddenBackColor, value);
				OnChanged("BackColor", oldValue, value);
			}
		}
		[
DefaultValue(DocumentCapability.Default), NotifyParentProperty(true)]
		public virtual DocumentCapability UnderlineColor {
			get { return GetVal(Mask.UnderlineColor, Mask.HiddenUnderlineColor); }
			set {
				if (this.UnderlineColor == value)
					return;
				DocumentCapability oldValue = this.UnderlineColor;
				SetVal(Mask.UnderlineColor, Mask.HiddenUnderlineColor, value);
				OnChanged("UnderlineColor", oldValue, value);
			}
		}
		[
DefaultValue(DocumentCapability.Default), NotifyParentProperty(true)]
		public virtual DocumentCapability StrikeoutColor {
			get { return GetVal(Mask.StrikeoutColor, Mask.HiddenStrikeoutColor); }
			set {
				if (this.StrikeoutColor == value)
					return;
				DocumentCapability oldValue = this.StrikeoutColor;
				SetVal(Mask.StrikeoutColor, Mask.HiddenStrikeoutColor, value);
				OnChanged("StrikeoutColor", oldValue, value);
			}
		}
		[
DefaultValue(DocumentCapability.Default), NotifyParentProperty(true)]
		public virtual DocumentCapability Script {
			get { return GetVal(Mask.Script, Mask.HiddenScript); }
			set {
				if (this.Script == value)
					return;
				DocumentCapability oldValue = this.Script;
				SetVal(Mask.Script, Mask.HiddenScript, value);
				OnChanged("Script", oldValue, value);
			}
		}
		[
DefaultValue(DocumentCapability.Default), NotifyParentProperty(true)]
		public virtual DocumentCapability Hidden {
			get { return GetVal(Mask.Hidden, Mask.HiddenHidden); }
			set {
				if (this.Hidden == value)
					return;
				DocumentCapability oldValue = this.Hidden;
				SetVal(Mask.Hidden, Mask.HiddenHidden, value);
				OnChanged("Hidden", oldValue, value);
			}
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool FontNameAllowed { get { return IsAllowed(FontName); } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool FontSizeAllowed { get { return IsAllowed(FontSize); } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool FontBoldAllowed { get { return IsAllowed(FontBold); } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool FontItalicAllowed { get { return IsAllowed(FontItalic); } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool FontStrikeoutAllowed { get { return IsAllowed(FontStrikeout); } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool FontUnderlineAllowed { get { return IsAllowed(FontUnderline); } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool AllCapsAllowed { get { return IsAllowed(AllCaps); } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool StrikeoutWordsOnlyAllowed { get { return IsAllowed(StrikeoutWordsOnly); } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool UnderlineWordsOnlyAllowed { get { return IsAllowed(UnderlineWordsOnly); } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool ForeColorAllowed { get { return IsAllowed(ForeColor); } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool BackColorAllowed { get { return IsAllowed(BackColor); } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool UnderlineColorAllowed { get { return IsAllowed(UnderlineColor); } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool StrikeoutColorAllowed { get { return IsAllowed(StrikeoutColor); } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool ScriptAllowed { get { return IsAllowed(Script); } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool HiddenAllowed { get { return IsAllowed(Hidden); } }
		bool IsAllowed(DocumentCapability option) { return option == DocumentCapability.Default || option == DocumentCapability.Enabled; }
		#endregion
		#region GetVal/SetVal helpers
		public void SetVal(Mask mask, Mask hidden, DocumentCapability value) {
			if (value == DocumentCapability.Default) {
				SetVal(hidden, true);
				SetVal(mask, true);
			}
			else if (value == DocumentCapability.Disabled) {
				SetVal(hidden, false);
				SetVal(mask, false);
			}
			else if (value == DocumentCapability.Enabled) {
				SetVal(hidden, false);
				SetVal(mask, true);
			}
			else if (value == DocumentCapability.Hidden) {
				SetVal(hidden, true);
				SetVal(mask, false);
			}
		}
		void SetVal(Mask mask, bool bitVal) {
			if (bitVal)
				Val |= mask;
			else
				Val &= ~mask;
		}
		public DocumentCapability GetVal(Mask mask, Mask hidden) {
			if((Val & hidden) == 0 && (Val & mask) == 0)
				return DocumentCapability.Disabled;
			if((Val & hidden) == 0 && (Val & mask) != 0)
				return DocumentCapability.Enabled;
			if((Val & hidden) != 0 && (Val & mask) == 0)
				return DocumentCapability.Hidden;
			return DocumentCapability.Default;
		}
		#endregion
		protected internal override void ResetCore() {
			Val = Mask.Default;
		}
		public void CopyFrom(CharacterFormattingDetailedOptions characterFormattingDetailedOptions) {
			this.Val = characterFormattingDetailedOptions.Val;
		}
		public override void Assign(BaseOptions options) {
			base.Assign(options);
			var opt = options as CharacterFormattingDetailedOptions;
			if(opt != null)
				CopyFrom(opt);
		}
	}
}
