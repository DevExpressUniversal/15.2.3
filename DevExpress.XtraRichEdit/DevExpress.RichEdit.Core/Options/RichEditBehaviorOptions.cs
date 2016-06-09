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
using DevExpress.Utils;
using DevExpress.XtraRichEdit.Model;
using DevExpress.Compatibility.System.ComponentModel;
namespace DevExpress.XtraRichEdit {
	[ComVisible(true)]
	public class RichEditBehaviorOptions : RichEditNotificationOptions {
		const float defaultMaxZoomFactor = float.PositiveInfinity;
		const float defaultMinZoomFactor = 0.09f;
		DocumentCapability allowDrag;
		DocumentCapability allowDrop;
		DocumentCapability allowCopy;
		DocumentCapability allowPaste;
		DocumentCapability allowCut;
		DocumentCapability allowPrinting;
		DocumentCapability allowZooming;
		DocumentCapability allowSaveAs;
		DocumentCapability allowSave;
		DocumentCapability allowCreateNew;
		DocumentCapability allowOpen;
		DocumentCapability allowShowPopupMenu;
		DocumentCapability allowOfficeScrolling;
		DocumentCapability allowTouch;
		float minZoomFactor;
		float maxZoomFactor;
		RichEditBaseValueSource fontSource;
		RichEditBaseValueSource foreColorSource;
		bool pasteSingleCellAsText;
		string tabMarker;
		bool useFontSubstitution;
		bool overtypeAllowed;
		LineBreakSubstitute pasteLineBreakSubstitution;
		PageBreakInsertMode pageBreakInsertMode;
		#region Properties
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public float DefaultMinZoomFactor { get { return defaultMinZoomFactor; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public float DefaultMaxZoomFactor { get { return defaultMaxZoomFactor; } }
		#region Drag
		[
#if !SL
	DevExpressRichEditCoreLocalizedDescription("RichEditBehaviorOptionsDrag"),
#endif
 DefaultValue(DocumentCapability.Default), NotifyParentProperty(true)]
		public virtual DocumentCapability Drag {
			get { return allowDrag; }
			set {
				if (this.allowDrag == value)
					return;
				DocumentCapability oldValue = this.allowDrag;
				this.allowDrag = value;
				OnChanged("Drag", oldValue, value);
			}
		}
		#endregion
		#region Drop
		[
#if !SL
	DevExpressRichEditCoreLocalizedDescription("RichEditBehaviorOptionsDrop"),
#endif
 DefaultValue(DocumentCapability.Default), NotifyParentProperty(true)]
		public virtual DocumentCapability Drop {
			get { return allowDrop; }
			set {
				if (this.allowDrop == value)
					return;
				DocumentCapability oldValue = this.allowDrop;
				this.allowDrop = value;
				OnChanged("Drop", oldValue, value);
			}
		}
		#endregion
		#region Copy
		[
#if !SL
	DevExpressRichEditCoreLocalizedDescription("RichEditBehaviorOptionsCopy"),
#endif
 DefaultValue(DocumentCapability.Default), NotifyParentProperty(true)]
		public virtual DocumentCapability Copy {
			get { return allowCopy; }
			set {
				if (this.allowCopy == value)
					return;
				DocumentCapability oldValue = this.allowCopy;
				this.allowCopy = value;
				OnChanged("Copy", oldValue, value);
			}
		}
		#endregion
		#region Printing
		[
#if !SL
	DevExpressRichEditCoreLocalizedDescription("RichEditBehaviorOptionsPrinting"),
#endif
 NotifyParentProperty(true)]
		public virtual DocumentCapability Printing {
			get { return allowPrinting; }
			set {
				if (this.allowPrinting == value)
					return;
				DocumentCapability oldValue = this.allowPrinting;
				this.allowPrinting = value;
				OnChanged("Printing", oldValue, value);
			}
		}
		protected internal virtual bool ShouldSerializePrinting() { return Printing != DocumentCapability.Default; }
		protected internal virtual void ResetPrinting() { Printing = DocumentCapability.Default; }
		#endregion
		#region SaveAs
		[
#if !SL
	DevExpressRichEditCoreLocalizedDescription("RichEditBehaviorOptionsSaveAs"),
#endif
 NotifyParentProperty(true)]
		public virtual DocumentCapability SaveAs {
			get { return allowSaveAs; }
			set {
				if (this.allowSaveAs == value)
					return;
				DocumentCapability oldValue = this.allowSaveAs;
				this.allowSaveAs = value;
				OnChanged("SaveAs", oldValue, value);
			}
		}
		protected internal virtual bool ShouldSerializeSaveAs() { return SaveAs != DocumentCapability.Default; }
		protected internal virtual void ResetSaveAs() { SaveAs = DocumentCapability.Default; }
		#endregion
		#region Save
		[
#if !SL
	DevExpressRichEditCoreLocalizedDescription("RichEditBehaviorOptionsSave"),
#endif
 NotifyParentProperty(true)]
		public virtual DocumentCapability Save {
			get { return allowSave; }
			set {
				if (this.allowSave == value)
					return;
				DocumentCapability oldValue = this.allowSave;
				this.allowSave = value;
				OnChanged("Save", oldValue, value);
			}
		}
		protected internal virtual bool ShouldSerializeSave() { return Save != DocumentCapability.Default; }
		protected internal virtual void ResetSave() { Save = DocumentCapability.Default; }
		#endregion
		#region Zooming
		[
#if !SL
	DevExpressRichEditCoreLocalizedDescription("RichEditBehaviorOptionsZooming"),
#endif
 NotifyParentProperty(true)]
		public virtual DocumentCapability Zooming {
			get { return allowZooming; }
			set {
				if (this.allowZooming == value)
					return;
				DocumentCapability oldValue = this.allowZooming;
				this.allowZooming = value;
				OnChanged("Zooming", oldValue, value);
			}
		}
		protected internal virtual bool ShouldSerializeZooming() { return Zooming != DocumentCapability.Default; }
		protected internal virtual void ResetZooming() { Zooming = DocumentCapability.Default; }
		#endregion
		#region ShowPopupMenu
		[
#if !SL
	DevExpressRichEditCoreLocalizedDescription("RichEditBehaviorOptionsShowPopupMenu"),
#endif
 DefaultValue(DocumentCapability.Default), NotifyParentProperty(true)]
		public virtual DocumentCapability ShowPopupMenu {
			get { return allowShowPopupMenu; }
			set {
				if (this.allowShowPopupMenu == value)
					return;
				DocumentCapability oldValue = this.allowShowPopupMenu;
				this.allowShowPopupMenu = value;
				OnChanged("ShowPopupMenu", oldValue, value);
			}
		}
		#endregion
		#region OfficeScrolling
		[
#if !SL
	DevExpressRichEditCoreLocalizedDescription("RichEditBehaviorOptionsOfficeScrolling"),
#endif
 DefaultValue(DocumentCapability.Default), NotifyParentProperty(true)]
		public virtual DocumentCapability OfficeScrolling {
			get { return allowOfficeScrolling; }
			set {
				if (this.allowOfficeScrolling == value)
					return;
				DocumentCapability oldValue = this.allowOfficeScrolling;
				this.allowOfficeScrolling = value;
				OnChanged("OfficeScrolling", oldValue, value);
			}
		}
		#endregion
		#region Touch
		[
#if !SL
	DevExpressRichEditCoreLocalizedDescription("RichEditBehaviorOptionsTouch"),
#endif
 DefaultValue(DocumentCapability.Default), NotifyParentProperty(true)]
		public virtual DocumentCapability Touch {
			get { return allowTouch; }
			set {
				if (this.allowTouch == value)
					return;
				DocumentCapability oldValue = this.allowTouch;
				this.allowTouch = value;
				OnChanged("Touch", oldValue, value);
			}
		}
		#endregion
		#region PageBreakInsertMode
		[
#if !SL
	DevExpressRichEditCoreLocalizedDescription("RichEditBehaviorOptionsPageBreakInsertMode"),
#endif
		DefaultValue(PageBreakInsertMode.NewLine)]
		public virtual PageBreakInsertMode PageBreakInsertMode
		{
			get { return pageBreakInsertMode; }
			set
			{
				if (this.pageBreakInsertMode == value)
					return;
				PageBreakInsertMode oldValue = this.pageBreakInsertMode;
				pageBreakInsertMode = value;
				OnChanged("PageBreakInsertMode", oldValue, value);
			}
		}
		#endregion
		#region MinZoomFactor
		[
#if !SL
	DevExpressRichEditCoreLocalizedDescription("RichEditBehaviorOptionsMinZoomFactor"),
#endif
 DefaultValue(defaultMinZoomFactor), NotifyParentProperty(true)]
		public virtual float MinZoomFactor {
			get { return minZoomFactor; }
			set {
				float newZoom = Math.Max(defaultMinZoomFactor, value);
				newZoom = Math.Min(newZoom, MaxZoomFactor);
				if (this.minZoomFactor == newZoom)
					return;
				float oldValue = this.minZoomFactor;
				this.minZoomFactor = newZoom;
				OnChanged("MinZoomFactor", oldValue, newZoom);
			}
		}
		#endregion
		#region MaxZoomFactor
		[
#if !SL
	DevExpressRichEditCoreLocalizedDescription("RichEditBehaviorOptionsMaxZoomFactor"),
#endif
 DefaultValue(defaultMaxZoomFactor), NotifyParentProperty(true)]
		public virtual float MaxZoomFactor {
			get { return maxZoomFactor; }
			set {
				float newZoom = Math.Max(minZoomFactor, value);
				if (this.maxZoomFactor == newZoom)
					return;
				float oldValue = this.maxZoomFactor;
				this.maxZoomFactor = newZoom;
				OnChanged("MaxZoomFactor", oldValue, newZoom);
			}
		}
		#endregion
		#region Paste
		[
#if !SL
	DevExpressRichEditCoreLocalizedDescription("RichEditBehaviorOptionsPaste"),
#endif
 DefaultValue(DocumentCapability.Default), NotifyParentProperty(true)]
		public virtual DocumentCapability Paste {
			get { return allowPaste; }
			set {
				if (this.allowPaste == value)
					return;
				DocumentCapability oldValue = this.allowPaste;
				this.allowPaste = value;
				OnChanged("Paste", oldValue, value);
			}
		}
		#endregion
		#region Cut
		[
#if !SL
	DevExpressRichEditCoreLocalizedDescription("RichEditBehaviorOptionsCut"),
#endif
 DefaultValue(DocumentCapability.Default), NotifyParentProperty(true)]
		public virtual DocumentCapability Cut {
			get { return allowCut; }
			set {
				if (this.allowCut == value)
					return;
				DocumentCapability oldValue = this.allowCut;
				this.allowCut = value;
				OnChanged("Cut", oldValue, value);
			}
		}
		#endregion
		#region CreateNew
		[
#if !SL
	DevExpressRichEditCoreLocalizedDescription("RichEditBehaviorOptionsCreateNew"),
#endif
 NotifyParentProperty(true)]
		public virtual DocumentCapability CreateNew {
			get { return allowCreateNew; }
			set {
				if (this.allowCreateNew == value)
					return;
				DocumentCapability oldValue = this.allowCreateNew;
				this.allowCreateNew = value;
				OnChanged("CreateNew", oldValue, value);
			}
		}
		protected internal virtual bool ShouldSerializeCreateNew() { return CreateNew != DocumentCapability.Default; }
		protected internal virtual void ResetCreateNew() { CreateNew = DocumentCapability.Default; }
		#endregion
		#region Open
		[
#if !SL
	DevExpressRichEditCoreLocalizedDescription("RichEditBehaviorOptionsOpen"),
#endif
 NotifyParentProperty(true)]
		public virtual DocumentCapability Open {
			get { return allowOpen; }
			set {
				if (this.allowOpen == value)
					return;
				DocumentCapability oldValue = this.allowOpen;
				this.allowOpen = value;
				OnChanged("Open", oldValue, value);
			}
		}
		protected internal virtual bool ShouldSerializeOpen() { return Open != DocumentCapability.Default; }
		protected internal virtual void ResetOpen() { Open = DocumentCapability.Default; }
		#endregion
		#region LineBreakSubstitute
		[
#if !SL
	DevExpressRichEditCoreLocalizedDescription("RichEditBehaviorOptionsPasteLineBreakSubstitution"),
#endif
		DefaultValue(LineBreakSubstitute.None), NotifyParentProperty(true)]
		public virtual LineBreakSubstitute PasteLineBreakSubstitution
		{
			get { return pasteLineBreakSubstitution; }
			set
			{
				if (this.pasteLineBreakSubstitution == value)
					return;
				LineBreakSubstitute oldValue = this.pasteLineBreakSubstitution;
				pasteLineBreakSubstitution = value;
				OnChanged("PasteLineBreakSubstitution", oldValue, value);
			}
		}
		#endregion
		#region PasteSingleCellAsText
		[
#if !SL
	DevExpressRichEditCoreLocalizedDescription("RichEditBehaviorOptionsPasteSingleCellAsText"),
#endif
 DefaultValue(false), NotifyParentProperty(true)]
		public virtual bool PasteSingleCellAsText {
			get { return pasteSingleCellAsText; }
			set {
				if (this.pasteSingleCellAsText == value)
					return;
				bool oldValue = pasteSingleCellAsText;
				this.pasteSingleCellAsText = value;
				OnChanged("PasteSingleCellAsText", oldValue, value);
			}
		}
		#endregion
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool DragAllowed { get { return IsAllowed(Drag); } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool DropAllowed { get { return IsAllowed(Drop); } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool CopyAllowed { get { return IsAllowed(Copy); } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool PasteAllowed { get { return IsAllowed(Paste); } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool CutAllowed { get { return IsAllowed(Cut); } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool PrintingAllowed { get { return IsAllowed(Printing); } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool SaveAllowed { get { return IsAllowed(Save); } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool SaveAsAllowed { get { return IsAllowed(SaveAs); } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool CreateNewAllowed { get { return IsAllowed(CreateNew); } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool OpenAllowed { get { return IsAllowed(Open); } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool ZoomingAllowed { get { return IsAllowed(Zooming); } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool ShowPopupMenuAllowed { get { return IsAllowed(ShowPopupMenu); } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool OfficeScrollingAllowed { get { return IsAllowed(OfficeScrolling); } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool TouchAllowed { get { return IsAllowed(Touch); } }
		#region FontSource
		[
#if !SL
	DevExpressRichEditCoreLocalizedDescription("RichEditBehaviorOptionsFontSource"),
#endif
 NotifyParentProperty(true), DefaultValue(RichEditBaseValueSource.Auto)]
		public virtual RichEditBaseValueSource FontSource {
			get { return fontSource; }
			set {
				if (FontSource == value)
					return;
				RichEditBaseValueSource previousValue = FontSource;
				this.fontSource = value;
				OnChanged("FontSource", previousValue, value);
			}
		}
		#endregion
		#region ForeColorSource
		[
#if !SL
	DevExpressRichEditCoreLocalizedDescription("RichEditBehaviorOptionsForeColorSource"),
#endif
 NotifyParentProperty(true), DefaultValue(RichEditBaseValueSource.Auto)]
		public virtual RichEditBaseValueSource ForeColorSource {
			get { return foreColorSource; }
			set {
				if (ForeColorSource == value)
					return;
				RichEditBaseValueSource previousValue = ForeColorSource;
				this.foreColorSource = value;
				OnChanged("ForeColorSource", previousValue, value);
			}
		}
		#endregion
		#region TabMarker
		[
#if !SL
	DevExpressRichEditCoreLocalizedDescription("RichEditBehaviorOptionsTabMarker"),
#endif
 NotifyParentProperty(true), DefaultValue("\t")]
		public virtual string TabMarker {
			get { return tabMarker; }
			set {
				if (tabMarker == value)
					return;
				string oldValue = this.tabMarker;
				this.tabMarker = value;
				OnChanged("TabMarker", oldValue, value);
			}
		}
		#endregion
		#region UseFontSubstitution
		[
#if !SL
	DevExpressRichEditCoreLocalizedDescription("RichEditBehaviorOptionsUseFontSubstitution"),
#endif
 NotifyParentProperty(true), DefaultValue(true)]
		public virtual bool UseFontSubstitution {
			get { return useFontSubstitution; }
			set {
				if (useFontSubstitution == value)
					return;
				useFontSubstitution = value;
				OnChanged("UseFontSubstitution", !value, value);
			}
		}
		#endregion
		#region OvertypeAllowed
		[
#if !SL
	DevExpressRichEditCoreLocalizedDescription("RichEditBehaviorOptionsOvertypeAllowed"),
#endif
		NotifyParentProperty(true), DefaultValue(true)]
		public virtual bool OvertypeAllowed
		{
			get { return overtypeAllowed; }
			set
			{
				if (overtypeAllowed == value)
					return;
				overtypeAllowed = value;
				OnChanged("OvertypeAllowed", !value, value);
			}
		}
		#endregion
		#endregion
		bool IsAllowed(DocumentCapability option) { return option == DocumentCapability.Default || option == DocumentCapability.Enabled; }
		protected internal override void ResetCore() {
			this.Drag = DocumentCapability.Default;
			this.Drop = DocumentCapability.Default;
			this.Copy = DocumentCapability.Default;
			this.Paste = DocumentCapability.Default;
			this.Cut = DocumentCapability.Default;
			this.ShowPopupMenu = DocumentCapability.Default;
			this.PageBreakInsertMode = PageBreakInsertMode.NewLine;
			this.MaxZoomFactor = DefaultMaxZoomFactor;
			this.MinZoomFactor = DefaultMinZoomFactor;
			this.FontSource = RichEditBaseValueSource.Auto;
			this.ForeColorSource = RichEditBaseValueSource.Auto;
			this.TabMarker = "\t";
			this.PasteSingleCellAsText = false;
			this.UseFontSubstitution = true;
			this.OvertypeAllowed = true;
			ResetPrinting();
			ResetZooming();
			ResetSaveAs();
			ResetSave();
			ResetCreateNew();
			ResetOpen();
		}
		protected internal void CopyFrom(RichEditBehaviorOptions value) {
			this.allowDrag = value.Drag;
			this.allowDrop = value.Drop;
			this.allowCopy = value.Copy;
			this.allowPaste = value.Paste;
			this.allowCut = value.Cut;
			this.allowPrinting = value.Printing;
			this.allowZooming = value.Zooming;
			this.allowSaveAs = value.SaveAs;
			this.allowSave = value.Save;
			this.allowCreateNew = value.CreateNew;
			this.allowOpen = value.Open;
			this.allowShowPopupMenu = value.ShowPopupMenu;
			this.minZoomFactor = value.MinZoomFactor;
			this.maxZoomFactor = value.MaxZoomFactor;
			this.fontSource = value.FontSource;
			this.foreColorSource = value.ForeColorSource;
			this.pasteSingleCellAsText = value.PasteSingleCellAsText;
			this.tabMarker = value.TabMarker;
			this.useFontSubstitution = value.UseFontSubstitution;
			this.overtypeAllowed = value.OvertypeAllowed;
			this.pageBreakInsertMode = value.pageBreakInsertMode;
		}
		public override void Assign(DevExpress.Utils.Controls.BaseOptions options) {
			base.Assign(options);
			RichEditBehaviorOptions opt = options as RichEditBehaviorOptions;
			if(opt != null) {
				Copy = opt.Copy;
				CreateNew = opt.CreateNew;
				Cut = opt.Cut;
				Drag = opt.Drag;
				Drop = opt.Drop;
				FontSource = opt.FontSource;
				ForeColorSource = opt.ForeColorSource;
				MaxZoomFactor = opt.MaxZoomFactor;
				MinZoomFactor = opt.MinZoomFactor;
				OfficeScrolling = opt.OfficeScrolling;
				Open = opt.Open;
				PageBreakInsertMode = opt.PageBreakInsertMode;
				Paste = opt.Paste;
				PasteLineBreakSubstitution = opt.PasteLineBreakSubstitution;
				PasteSingleCellAsText = opt.PasteSingleCellAsText;
				Printing = opt.Printing;
				Save = opt.Save;
				SaveAs = opt.SaveAs;
				ShowPopupMenu = opt.ShowPopupMenu;
				TabMarker = opt.TabMarker;
				Touch = opt.Touch;
				UseFontSubstitution = opt.UseFontSubstitution;
				Zooming = opt.Zooming;
			}
		}
	}
	#region RichEditBaseValueSource
	[ComVisible(true)]
	public enum RichEditBaseValueSource {
		Auto = 0,
		Document,
		Control
	}
	#endregion
	#region LineBreakSubstitute
	public enum LineBreakSubstitute {
		None,
		Paragraph,
		Space
	}
	#endregion
	#region PageBreakInsertMode
	public enum PageBreakInsertMode {
		NewLine = 0,
		CurrentLine = 1
	}
	#endregion
}
