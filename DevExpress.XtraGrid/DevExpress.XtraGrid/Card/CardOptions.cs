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
using System.Collections;
using System.Windows.Forms;
using DevExpress.Utils.Controls;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.Utils.Serializing;
namespace DevExpress.XtraGrid.Views.Card {
	public class CardOptionsBehavior : ColumnViewOptionsBehavior {
		bool autoFocusNewCard, autoHorzWidth, fieldAutoHeight, 
			sizeable, useTabKey, allowExpandCollapse;
		public CardOptionsBehavior(ColumnView view) : base(view) { 
			this.autoFocusNewCard = false;
			this.autoHorzWidth = false;
			this.fieldAutoHeight = false;
			this.sizeable = true;
			this.allowExpandCollapse = true;
			this.useTabKey = false;
		}
		public CardOptionsBehavior() : this(null) { }
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("CardOptionsBehaviorAutoFocusNewCard"),
#endif
 DefaultValue(false), XtraSerializableProperty()]
		public virtual bool AutoFocusNewCard {
			get { return autoFocusNewCard; }
			set {
				if(AutoFocusNewCard == value) return;
				bool prevValue = AutoFocusNewCard;
				autoFocusNewCard = value;
				OnChanged(new BaseOptionChangedEventArgs("AutoFocusNewCard", prevValue, AutoFocusNewCard));
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("CardOptionsBehaviorAutoHorzWidth"),
#endif
 DefaultValue(false), XtraSerializableProperty()]
		public virtual bool AutoHorzWidth {
			get { return autoHorzWidth; }
			set {
				if(AutoHorzWidth == value) return;
				bool prevValue = AutoHorzWidth;
				autoHorzWidth = value;
				OnChanged(new BaseOptionChangedEventArgs("AutoHorzWidth", prevValue, AutoHorzWidth));
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("CardOptionsBehaviorAllowExpandCollapse"),
#endif
 DefaultValue(true), XtraSerializableProperty()]
		public virtual bool AllowExpandCollapse {
			get { return allowExpandCollapse; }
			set {
				if(AllowExpandCollapse == value) return;
				bool prevValue = AllowExpandCollapse;
				allowExpandCollapse = value;
				OnChanged(new BaseOptionChangedEventArgs("AllowExpandCollapse", prevValue, AllowExpandCollapse));
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("CardOptionsBehaviorFieldAutoHeight"),
#endif
 DefaultValue(false), XtraSerializableProperty()]
		public virtual bool FieldAutoHeight {
			get { return fieldAutoHeight; }
			set {
				if(FieldAutoHeight == value) return;
				bool prevValue = FieldAutoHeight;
				fieldAutoHeight = value;
				OnChanged(new BaseOptionChangedEventArgs("FieldAutoHeight", prevValue, FieldAutoHeight));
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("CardOptionsBehaviorSizeable"),
#endif
 DefaultValue(true), XtraSerializableProperty()]
		public virtual bool Sizeable {
			get { return sizeable; }
			set {
				if(Sizeable == value) return;
				bool prevValue = Sizeable;
				sizeable = value;
				OnChanged(new BaseOptionChangedEventArgs("Sizeable", prevValue, Sizeable));
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("CardOptionsBehaviorUseTabKey"),
#endif
 DefaultValue(false), XtraSerializableProperty()]
		public virtual bool UseTabKey {
			get { return useTabKey; }
			set {
				if(UseTabKey == value) return;
				bool prevValue = UseTabKey;
				useTabKey = value;
				OnChanged(new BaseOptionChangedEventArgs("UseTabKey", prevValue, UseTabKey));
			}
		}
		public override void Assign(BaseOptions options) {
			BeginUpdate();
			try {
				base.Assign(options);
				CardOptionsBehavior opt = options as CardOptionsBehavior;
				if(opt == null) return;
				this.allowExpandCollapse = opt.AllowExpandCollapse;
				this.autoFocusNewCard = opt.AutoFocusNewCard;
				this.autoHorzWidth = opt.AutoHorzWidth;
				this.fieldAutoHeight = opt.FieldAutoHeight;
				this.sizeable = opt.Sizeable;
				this.useTabKey = opt.UseTabKey;
			}
			finally {
				EndUpdate();
			}
		}
	}
	public class CardOptionsView : ColumnViewOptionsView {
		bool showCardCaption, showEmptyFields, showHorzScrollBar, showLines, showFieldHints,
			showCardExpandButton,  
			showQuickCustomizeButton, showFieldCaptions;
		public CardOptionsView() {
			this.showFieldCaptions = true;
			this.showQuickCustomizeButton = true;
			this.showCardExpandButton = true;
			this.showFieldHints = true;
			this.showCardCaption = true;
			this.showEmptyFields = true;
			this.showHorzScrollBar = true;
			this.showLines = true;
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("CardOptionsViewShowCardCaption"),
#endif
 DefaultValue(true), XtraSerializableProperty()]
		public virtual bool ShowCardCaption {
			get { return showCardCaption; }
			set {
				if(ShowCardCaption == value) return;
				bool prevValue = ShowCardCaption;
				showCardCaption = value;
				OnChanged(new BaseOptionChangedEventArgs("ShowCardCaption", prevValue, ShowCardCaption));
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("CardOptionsViewShowFieldCaptions"),
#endif
 DefaultValue(true), XtraSerializableProperty()]
		public virtual bool ShowFieldCaptions {
			get { return showFieldCaptions; }
			set {
				if(ShowFieldCaptions == value) return;
				bool prevValue = ShowFieldCaptions;
				showFieldCaptions = value;
				OnChanged(new BaseOptionChangedEventArgs("ShowFieldCaptions", prevValue, ShowFieldCaptions));
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("CardOptionsViewShowCardExpandButton"),
#endif
 DefaultValue(true), XtraSerializableProperty()]
		public virtual bool ShowCardExpandButton {
			get { return showCardExpandButton; }
			set {
				if(ShowCardExpandButton == value) return;
				bool prevValue = ShowCardExpandButton;
				showCardExpandButton = value;
				OnChanged(new BaseOptionChangedEventArgs("ShowCardExpandButton", prevValue, ShowCardExpandButton));
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("CardOptionsViewShowQuickCustomizeButton"),
#endif
 DefaultValue(true), XtraSerializableProperty()]
		public virtual bool ShowQuickCustomizeButton {
			get { return showQuickCustomizeButton; }
			set {
				if(ShowQuickCustomizeButton == value) return;
				bool prevValue = ShowQuickCustomizeButton;
				showQuickCustomizeButton = value;
				OnChanged(new BaseOptionChangedEventArgs("ShowQuickCustomizeButton", prevValue, ShowQuickCustomizeButton));
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("CardOptionsViewShowFieldHints"),
#endif
 DefaultValue(true), XtraSerializableProperty()]
		public virtual bool ShowFieldHints {
			get { return showFieldHints; }
			set {
				if(ShowFieldHints == value) return;
				bool prevValue = ShowFieldHints;
				showFieldHints = value;
				OnChanged(new BaseOptionChangedEventArgs("ShowFieldHints", prevValue, ShowFieldHints));
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("CardOptionsViewShowEmptyFields"),
#endif
 DefaultValue(true), XtraSerializableProperty()]
		public virtual bool ShowEmptyFields {
			get { return showEmptyFields; }
			set {
				if(ShowEmptyFields == value) return;
				bool prevValue = ShowEmptyFields;
				showEmptyFields = value;
				OnChanged(new BaseOptionChangedEventArgs("ShowEmptyFields", prevValue, ShowEmptyFields));
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("CardOptionsViewShowHorzScrollBar"),
#endif
 DefaultValue(true), XtraSerializableProperty()]
		public virtual bool ShowHorzScrollBar {
			get { return showHorzScrollBar; }
			set {
				if(ShowHorzScrollBar == value) return;
				bool prevValue = ShowHorzScrollBar;
				showHorzScrollBar = value;
				OnChanged(new BaseOptionChangedEventArgs("ShowHorzScrollBar", prevValue, ShowHorzScrollBar));
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("CardOptionsViewShowLines"),
#endif
 DefaultValue(true), XtraSerializableProperty()]
		public virtual bool ShowLines {
			get { return showLines; }
			set {
				if(ShowLines == value) return;
				bool prevValue = ShowLines;
				showLines = value;
				OnChanged(new BaseOptionChangedEventArgs("ShowLines", prevValue, ShowLines));
			}
		}
		public override void Assign(BaseOptions options) {
			BeginUpdate();
			try {
				base.Assign(options);
				CardOptionsView opt = options as CardOptionsView;
				if(opt == null) return;
				this.showFieldCaptions = opt.ShowFieldCaptions;
				this.showFieldHints = opt.ShowFieldHints;
				this.showCardCaption = opt.ShowCardCaption;
				this.showQuickCustomizeButton = opt.ShowQuickCustomizeButton;
				this.showEmptyFields = opt.ShowEmptyFields;
				this.showHorzScrollBar = opt.ShowHorzScrollBar;
				this.showLines = opt.ShowLines;
				this.showCardExpandButton = opt.ShowCardExpandButton;
			}
			finally {
				EndUpdate();
			}
		}
	}
	public class CardOptionsPrint : ViewPrintOptionsBase {
		bool autoHorzWidth, printCardCaption, printEmptyFields, printSelectedCardsOnly, 
			usePrintStyles, printFilterInfo;
		public CardOptionsPrint() {
			this.printFilterInfo = false;
			this.autoHorzWidth = false;
			this.printCardCaption = true;
			this.printEmptyFields = true;
			this.printSelectedCardsOnly = false;
			this.usePrintStyles = true;
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("CardOptionsPrintAutoHorzWidth"),
#endif
 DefaultValue(false), XtraSerializableProperty()]
		public virtual bool AutoHorzWidth {
			get { return autoHorzWidth; }
			set {
				if(AutoHorzWidth == value) return;
				bool prevValue = AutoHorzWidth;
				autoHorzWidth = value;
				OnChanged(new BaseOptionChangedEventArgs("AutoHorzWidth", prevValue, AutoHorzWidth));
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("CardOptionsPrintPrintCardCaption"),
#endif
 DefaultValue(true), XtraSerializableProperty()]
		public virtual bool PrintCardCaption {
			get { return printCardCaption; }
			set {
				if(PrintCardCaption == value) return;
				bool prevValue = PrintCardCaption;
				printCardCaption = value;
				OnChanged(new BaseOptionChangedEventArgs("PrintCardCaption", prevValue, PrintCardCaption));
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("CardOptionsPrintPrintFilterInfo"),
#endif
 DefaultValue(false), XtraSerializableProperty()]
		public virtual bool PrintFilterInfo {
			get { return printFilterInfo; }
			set {
				if(PrintFilterInfo == value) return;
				bool prevValue = PrintFilterInfo;
				printFilterInfo = value;
				OnChanged(new BaseOptionChangedEventArgs("PrintFilterInfo", prevValue, PrintFilterInfo));
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("CardOptionsPrintPrintEmptyFields"),
#endif
 DefaultValue(true), XtraSerializableProperty()]
		public virtual bool PrintEmptyFields {
			get { return printEmptyFields; }
			set {
				if(PrintEmptyFields == value) return;
				bool prevValue = PrintEmptyFields;
				printEmptyFields = value;
				OnChanged(new BaseOptionChangedEventArgs("PrintEmptyFields", prevValue, PrintEmptyFields));
			}
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		Obsolete(ObsoleteText.SRCardOptionsPrint_PrintSelectedCardOnly)]
		public bool PrintSelectedCardOnly {
			get { return PrintSelectedCardsOnly; }
			set { PrintSelectedCardsOnly = value; }
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("CardOptionsPrintPrintSelectedCardsOnly"),
#endif
 DefaultValue(false), XtraSerializableProperty()]
		public virtual bool PrintSelectedCardsOnly {
			get { return printSelectedCardsOnly; }
			set {
				if(PrintSelectedCardsOnly == value) return;
				bool prevValue = PrintSelectedCardsOnly;
				printSelectedCardsOnly = value;
				OnChanged(new BaseOptionChangedEventArgs("PrintSelectedCardsOnly", prevValue, PrintSelectedCardsOnly));
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("CardOptionsPrintUsePrintStyles"),
#endif
 DefaultValue(true), XtraSerializableProperty()]
		public virtual bool UsePrintStyles {
			get { return usePrintStyles; }
			set {
				if(UsePrintStyles == value) return;
				bool prevValue = UsePrintStyles;
				usePrintStyles = value;
				OnChanged(new BaseOptionChangedEventArgs("UsePrintStyles", prevValue, UsePrintStyles));
			}
		}
		public override void Assign(BaseOptions options) {
			BeginUpdate();
			try {
				base.Assign(options);
				CardOptionsPrint opt = options as CardOptionsPrint;
				if(opt == null) return;
				this.printFilterInfo = opt.PrintFilterInfo;
				this.autoHorzWidth = opt.AutoHorzWidth;
				this.printCardCaption = opt.PrintCardCaption;
				this.printEmptyFields = opt.PrintEmptyFields;
				this.printSelectedCardsOnly = opt.PrintSelectedCardsOnly;
				this.usePrintStyles = opt.UsePrintStyles;
			}
			finally {
				EndUpdate();
			}
		}
	}
}
