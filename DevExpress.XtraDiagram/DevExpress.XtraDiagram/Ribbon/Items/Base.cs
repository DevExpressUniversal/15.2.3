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
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using DevExpress.Diagram.Core.Localization;
using DevExpress.Utils;
using DevExpress.Utils.Commands;
using DevExpress.Utils.Localization;
using DevExpress.XtraBars;
using DevExpress.XtraBars.Commands;
using DevExpress.XtraDiagram.Options;
namespace DevExpress.XtraDiagram.Bars {
	public abstract class DiagramCommandBarButtonItem : ControlCommandBarButtonItem<DiagramControl, DiagramCommandId>, IDiagramCommandBarItem {
		public DiagramCommandBarButtonItem() {
			this.AllowGlyphSkinning = DefaultBoolean.False;
		}
		protected override void UpdateCaption() {
			base.UpdateCaption();
			UpdateDescription();
		}
		protected virtual void UpdateDescription() {
			if(string.IsNullOrEmpty(Description)) Description = GetDefaultSuperTipDescription();
		}
		protected override void UpdateItemGlyphs() {
			base.UpdateItemGlyphs();
			if(Manager != null || Ribbon != null) {
				ImageUri = GetImageUri();
			}
		}
		protected virtual DxImageUri GetImageUri() {
			return null;
		}
		protected virtual bool AddToApplicationMenu { get { return false; } }
		protected virtual bool AddToQuickAccessToolbar { get { return false; } }
		protected virtual bool AddToRibbonPage { get { return true; } }
		#region IDiagramCommandBarItem
		bool IDiagramCommandBarItem.AddToApplicationMenu { get { return AddToApplicationMenu; } }
		bool IDiagramCommandBarItem.AddToQuickAccessToolbar { get { return AddToQuickAccessToolbar; } }
		bool IDiagramCommandBarItem.AddToRibbonPage { get { return AddToRibbonPage; } }
		#endregion
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public DiagramControl Diagram { get { return Control; } }
	}
	public abstract class DiagramCommandBarCheckItem : ControlCommandBarCheckItem<DiagramControl, DiagramCommandId>, IDiagramCommandBarItem {
		public DiagramCommandBarCheckItem() {
			this.AllowGlyphSkinning = DefaultBoolean.False;
		}
		protected override void UpdateItemGlyphs() {
			base.UpdateItemGlyphs();
			if(Manager != null || Ribbon != null) {
				ImageUri = GetImageUri();
			}
		}
		protected virtual DxImageUri GetImageUri() {
			return null;
		}
		protected virtual bool AddToApplicationMenu { get { return false; } }
		protected virtual bool AddToQuickAccessToolbar { get { return false; } }
		protected virtual bool AddToRibbonPage { get { return true; } }
		#region IDiagramCommandBarItem
		bool IDiagramCommandBarItem.AddToApplicationMenu { get { return AddToApplicationMenu; } }
		bool IDiagramCommandBarItem.AddToQuickAccessToolbar { get { return AddToQuickAccessToolbar; } }
		bool IDiagramCommandBarItem.AddToRibbonPage { get { return AddToRibbonPage; } }
		#endregion
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public DiagramControl Diagram { get { return Control; } }
	}
	public abstract class DiagramPopupMenuCommandBarButtonItem : DiagramCommandBarButtonItem {
		protected override bool AddToRibbonPage { get { return false; } }
	}
	public abstract class DiagramPopupMenuCommandBarCheckItem : DiagramCommandBarCheckItem {
		protected override bool AddToRibbonPage { get { return false; } }
	}
	public abstract class DiagramCommandBarSubItem : ControlCommandBarSubItem<DiagramControl, DiagramCommandId> {
		public DiagramCommandBarSubItem() {
			this.AllowGlyphSkinning = DefaultBoolean.False;
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public DiagramControl Diagram { get { return Control; } }
	}
	public abstract class DiagramCommandSkinGalleryBarItem : ControlCommandSkinGalleryBarItem<DiagramControl, DiagramCommandId> {
		public DiagramCommandSkinGalleryBarItem() {
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public DiagramControl Diagram { get { return Control; } }
	}
	public abstract class DiagramCommandBarEditItem<T> : ControlCommandBarEditItem<DiagramControl, DiagramCommandId, T> {
		protected DiagramCommandBarEditItem()
			: base() {
		}
		protected DiagramCommandBarEditItem(string caption)
			: base(caption) {
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public DiagramControl Diagram { get { return Control; } }
	}
	public abstract class DiagramCommand : ControlCommand<DiagramControl, DiagramCommandId, DiagramControlStringId> {
		protected DiagramCommand(DiagramControl control)
			: base(control) {
		}
		protected DiagramControl Diagram { get { return Control; } }
		public override void ForceExecute(ICommandUIState state) {
			ExecuteInternal();
			Control.RaiseUpdateUI();
		}
		protected virtual void ApplyCommandsRestriction(ICommandUIState state, DocumentCapability option) {
			state.Enabled = (option != DocumentCapability.Disabled && option != DocumentCapability.Hidden);
			state.Visible = (option != DocumentCapability.Hidden);
		}
		protected virtual string GetCaptionPostfix() { return string.Empty; }
		protected override string ImageResourcePrefix { get { return string.Empty; } }
		protected override void UpdateUIStateCore(ICommandUIState state) { }
		protected virtual void ExecuteInternal() {
		}
		protected virtual BarShortcut ItemShortcut { get { return BarShortcut.Empty; } }
		protected override XtraLocalizer<DiagramControlStringId> Localizer { get { return DiagramControlLocalizer.Active; } }
	}
	public static class GroupId {
		public static readonly string FontGroupId = "Font";
		public static readonly string FontStyleGroupId = "FontStyle";
		public static readonly string VertAlignmentGroupId = "VerticalAlignment";
		public static readonly string HorzAlignmentGroupId = "HorizontalAlignment";
	}
}
