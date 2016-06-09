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
using System.ComponentModel;
using System.Drawing;
using DevExpress.Utils;
using DevExpress.Utils.Commands;
using DevExpress.Utils.Drawing;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Repository;
using DevExpress.Office;
using DevExpress.Office.Internal;
using DevExpress.Office.Model;
using DevExpress.Office.UI;
using DevExpress.XtraBars;
using DevExpress.XtraBars.Commands;
using DevExpress.XtraBars.Commands.Internal;
using DevExpress.XtraBars.Commands.Ribbon;
using DevExpress.XtraBars.Ribbon;
using DevExpress.XtraBars.Ribbon.ViewInfo;
using DevExpress.XtraRichEdit.Localization;
using DevExpress.XtraRichEdit.Commands;
using DevExpress.XtraRichEdit.Design;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Internal;
using DevExpress.XtraRichEdit.Forms;
using DevExpress.XtraRichEdit.Services;
using DevExpress.Office.Utils;
namespace DevExpress.XtraRichEdit.UI {
	#region RichEditFontItemBuilder
	public class RichEditFontItemBuilder : CommandBasedBarItemBuilder {
		public override void PopulateItems(List<BarItem> items, BarCreationContextBase creationContext) {
			items.Add(new ChangeFontNameItem());
			items.Add(new ChangeFontSizeItem());
			items.Add(new FontSizeIncreaseItem());
			items.Add(new FontSizeDecreaseItem());
			items.Add(new ToggleFontBoldItem());
			items.Add(new ToggleFontItalicItem());
			items.Add(new ToggleFontUnderlineItem());
			items.Add(new ToggleFontDoubleUnderlineItem());
			items.Add(new ToggleFontStrikeoutItem());
			items.Add(new ToggleFontDoubleStrikeoutItem());
			items.Add(new ToggleFontSuperscriptItem());
			items.Add(new ToggleFontSubscriptItem());
			items.Add(new ChangeFontColorItem());
			items.Add(new ChangeFontBackColorItem());
			ChangeTextCaseItem caseItem = new ChangeTextCaseItem();
			items.Add(caseItem);
			caseItem.AddBarItem(new MakeTextUpperCaseItem());
			caseItem.AddBarItem(new MakeTextLowerCaseItem());
			caseItem.AddBarItem(new CapitalizeEachWordCaseItem());
			caseItem.AddBarItem(new ToggleTextCaseItem());
			items.Add(new ClearFormattingItem());
			items.Add(new ShowFontFormItem());
		}
	}
	#endregion
	#region ChangeFontNameItem
	public class ChangeFontNameItem : RichEditCommandBarEditItem<string>, IBarButtonGroupMember {
		const int defaultWidth = 130;
		public ChangeFontNameItem() {
			Width = defaultWidth;
		}
		public ChangeFontNameItem(BarManager manager, string caption)
			: base(manager, caption) {
			Width = defaultWidth;
		}
		[DefaultValue(defaultWidth)]
		public override int Width { get { return base.Width; } set { base.Width = value; } }
		protected override RichEditCommandId CommandId { get { return RichEditCommandId.ChangeFontName; } }
		protected override RepositoryItem CreateEdit() {
			return new RepositoryItemFontEdit();
		}
		#region IBarButtonGroupMember Members
		object IBarButtonGroupMember.ButtonGroupTag { get { return HomeButtonGroups.FontNameAndSizeButtonGroup; } }
		#endregion
	}
	#endregion
	#region ChangeFontSizeItem
	public class ChangeFontSizeItem : RichEditCommandBarEditItem<int?>, IBarButtonGroupMember {
		public ChangeFontSizeItem() {
		}
		public ChangeFontSizeItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		protected override RichEditCommandId CommandId { get { return RichEditCommandId.ChangeDoubleFontSize; } }
		protected override ICommandUIState CreateCommandUIState(Command command) {
			DefaultValueBasedCommandUIState<int?> value = new DefaultValueBasedCommandUIState<int?>();
			int editValue = 0;
			if (EditValue != null)
				OfficeFontSizeEditHelper.TryGetHalfSizeValue(EditValue.ToString(), out editValue);
			value.Value = editValue;
			return value;
		}
		protected override BarEditItemUIState<int?> CreateBarEditItemUIState() {
			return new FontSizeEditItemUIState(this);
		}
		protected override RepositoryItem CreateEdit() {
			RepositoryItemRichEditFontSizeEdit edit = new RepositoryItemRichEditFontSizeEdit();
			if (Control != null)
				edit.Control = Control;
			return edit;
		}
		protected override void OnControlChanged() {
			RepositoryItemRichEditFontSizeEdit edit = (RepositoryItemRichEditFontSizeEdit)Edit;
			if (edit != null)
				edit.Control = Control;
		}
		protected override void OnEditValidating(object sender, CancelEventArgs e) {
			ComboBoxEdit edit = (ComboBoxEdit)sender;
			string text = String.Empty;
			e.Cancel = !EditStyleHelper.IsFontSizeValid(edit.EditValue, out text);
			if (e.Cancel)
				edit.ErrorText = text;
		}
		protected virtual bool IsValidFontSize(int fontSize) {
			return fontSize >= PredefinedFontSizeCollection.MinFontSize && fontSize <= PredefinedFontSizeCollection.MaxFontSize;
		}
		#region IBarButtonGroupMember Members
		object IBarButtonGroupMember.ButtonGroupTag { get { return HomeButtonGroups.FontNameAndSizeButtonGroup; } }
		#endregion
	}
	public class FontSizeEditItemUIState : BarEditItemUIState<int?> {
		public FontSizeEditItemUIState(BarEditItem item) : base(item) {
		}
		public override int? Value { 
			get {
				if ((int?)Item.EditValue == null)
					return null;
				else 
				   return (int)((float)Item.EditValue * 2); }
			set {
				if (value != null)
					Item.EditValue = value / 2f;
				else
					Item.EditValue = null;
			} 
		}
	}
	#endregion
	#region ChangeFontColorItem
	public class ChangeFontColorItem : ChangeColorItemBase<RichEditControl, RichEditCommandId>, IBarButtonGroupMember {
		public ChangeFontColorItem() {
		}
		public ChangeFontColorItem(BarManager manager)
			: base(manager) {
		}
		public ChangeFontColorItem(string caption)
			: base(caption) {
		}
		public ChangeFontColorItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		protected override RichEditCommandId CommandId { get { return RichEditCommandId.None; } }
		protected override string DefaultColorButtonCaption { get { return RichEditExtensionsLocalizer.GetString(RichEditExtensionsStringId.Caption_ColorAutomatic); } }
		#region Properties
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public RichEditControl RichEditControl { get { return this.Control; } set { this.Control = value; } }
		#endregion
		protected override Command CreateCommand() {
			if (Control == null)
				return null;
			IRichEditCommandFactoryService service = Control.GetService(typeof(IRichEditCommandFactoryService)) as IRichEditCommandFactoryService;
			if (service == null)
				return null;
			Command command = service.CreateCommand(RichEditCommandId.ChangeFontForeColor);
			if (command == null)
				return null;
			command.CommandSourceType = CommandSourceType.Menu;
			return command;
		}
		#region IBarButtonGroupMember Members
		object IBarButtonGroupMember.ButtonGroupTag { get { return HomeButtonGroups.FontColorButtonGroup; } }
		#endregion
	}
	#endregion
	#region ChangeFontBackColorItem
	public class ChangeFontBackColorItem : ChangeColorItemBase<RichEditControl, RichEditCommandId>, IBarButtonGroupMember {
		public ChangeFontBackColorItem() {
		}
		public ChangeFontBackColorItem(BarManager manager)
			: base(manager) {
		}
		public ChangeFontBackColorItem(string caption)
			: base(caption) {
		}
		public ChangeFontBackColorItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		protected override RichEditCommandId CommandId { get { return  RichEditCommandId.ChangeFontBackColor; } }
		protected override string DefaultColorButtonCaption { get { return RichEditExtensionsLocalizer.GetString(RichEditExtensionsStringId.Caption_NoColor); } }
		#region Properties
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public RichEditControl RichEditControl { get { return this.Control; } set { this.Control = value; } }
		#endregion
		protected override Command CreateCommand() {
			if (Control == null)
				return null;
			IRichEditCommandFactoryService service = Control.GetService(typeof(IRichEditCommandFactoryService)) as IRichEditCommandFactoryService;
			if (service == null)
				return null;
			Command command = service.CreateCommand(RichEditCommandId.ChangeFontBackColor);
			if (command == null)
				return null;
			command.CommandSourceType = CommandSourceType.Menu;
			return command;
		}
		protected override void DrawColorRectangle(Image image, Rectangle rect) {
			if (Color == Color.Empty) {
				DrawColorRectangleCore(image, rect, Color.FromArgb(232, 232, 232));
				using (Graphics gr = Graphics.FromImage(image)) {
					using (Pen pen = new Pen(Color.FromArgb(160, 160, 160))) {
						gr.DrawRectangle(pen, rect.X, rect.Y, rect.Width - 1, rect.Height - 1);
					}
				}
			}
			else
				DrawColorRectangleCore(image, rect, Color);
		}
		#region IBarButtonGroupMember Members
		object IBarButtonGroupMember.ButtonGroupTag { get { return HomeButtonGroups.FontColorButtonGroup; } }
		#endregion
	}
	#endregion
	#region ToggleFontBoldItem
	public class ToggleFontBoldItem : RichEditCommandBarCheckItem, IBarButtonGroupMember {
		public ToggleFontBoldItem() {
		}
		public ToggleFontBoldItem(BarManager manager)
			: base(manager) {
		}
		public ToggleFontBoldItem(string caption)
			: base(caption) {
		}
		public ToggleFontBoldItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		protected override RichEditCommandId CommandId { get { return RichEditCommandId.ToggleFontBold; } }
		protected override RibbonItemStyles DefaultRibbonStyle { get { return RibbonItemStyles.SmallWithoutText; } }
		#region IBarButtonGroupMember Members
		object IBarButtonGroupMember.ButtonGroupTag { get { return HomeButtonGroups.FontStyleButtonGroup; } }
		#endregion
	}
	#endregion
	#region ToggleFontItalicItem
	public class ToggleFontItalicItem : RichEditCommandBarCheckItem, IBarButtonGroupMember {
		public ToggleFontItalicItem() {
		}
		public ToggleFontItalicItem(BarManager manager)
			: base(manager) {
		}
		public ToggleFontItalicItem(string caption)
			: base(caption) {
		}
		public ToggleFontItalicItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		protected override RichEditCommandId CommandId { get { return RichEditCommandId.ToggleFontItalic; } }
		protected override RibbonItemStyles DefaultRibbonStyle { get { return RibbonItemStyles.SmallWithoutText; } }
		#region IBarButtonGroupMember Members
		object IBarButtonGroupMember.ButtonGroupTag { get { return HomeButtonGroups.FontStyleButtonGroup; } }
		#endregion
	}
	#endregion
	#region ToggleFontUnderlineItem
	public class ToggleFontUnderlineItem : RichEditCommandBarCheckItem, IBarButtonGroupMember {
		public ToggleFontUnderlineItem() {
		}
		public ToggleFontUnderlineItem(BarManager manager)
			: base(manager) {
		}
		public ToggleFontUnderlineItem(string caption)
			: base(caption) {
		}
		public ToggleFontUnderlineItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		protected override RichEditCommandId CommandId { get { return RichEditCommandId.ToggleFontUnderline; } }
		protected override RibbonItemStyles DefaultRibbonStyle { get { return RibbonItemStyles.SmallWithoutText; } }
		#region IBarButtonGroupMember Members
		object IBarButtonGroupMember.ButtonGroupTag { get { return HomeButtonGroups.FontStyleButtonGroup; } }
		#endregion
	}
	#endregion
	#region ToggleFontDoubleUnderlineItem
	public class ToggleFontDoubleUnderlineItem : RichEditCommandBarCheckItem, IBarButtonGroupMember {
		public ToggleFontDoubleUnderlineItem() {
		}
		public ToggleFontDoubleUnderlineItem(BarManager manager)
			: base(manager) {
		}
		public ToggleFontDoubleUnderlineItem(string caption)
			: base(caption) {
		}
		public ToggleFontDoubleUnderlineItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		protected override RichEditCommandId CommandId { get { return RichEditCommandId.ToggleFontDoubleUnderline; } }
		protected override RibbonItemStyles DefaultRibbonStyle { get { return RibbonItemStyles.SmallWithoutText; } }
		#region IBarButtonGroupMember Members
		object IBarButtonGroupMember.ButtonGroupTag { get { return HomeButtonGroups.FontStyleButtonGroup; } }
		#endregion
	}
	#endregion
	#region ToggleFontStrikeoutItem
	public class ToggleFontStrikeoutItem : RichEditCommandBarCheckItem, IBarButtonGroupMember {
		public ToggleFontStrikeoutItem() {
		}
		public ToggleFontStrikeoutItem(BarManager manager)
			: base(manager) {
		}
		public ToggleFontStrikeoutItem(string caption)
			: base(caption) {
		}
		public ToggleFontStrikeoutItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		protected override RichEditCommandId CommandId { get { return RichEditCommandId.ToggleFontStrikeout; } }
		protected override RibbonItemStyles DefaultRibbonStyle { get { return RibbonItemStyles.SmallWithoutText; } }
		#region IBarButtonGroupMember Members
		object IBarButtonGroupMember.ButtonGroupTag { get { return HomeButtonGroups.FontStyleButtonGroup; } }
		#endregion
	}
	#endregion
	#region ToggleFontDoubleStrikeoutItem
	public class ToggleFontDoubleStrikeoutItem : RichEditCommandBarCheckItem, IBarButtonGroupMember {
		public ToggleFontDoubleStrikeoutItem() {
		}
		public ToggleFontDoubleStrikeoutItem(BarManager manager)
			: base(manager) {
		}
		public ToggleFontDoubleStrikeoutItem(string caption)
			: base(caption) {
		}
		public ToggleFontDoubleStrikeoutItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		protected override RichEditCommandId CommandId { get { return RichEditCommandId.ToggleFontDoubleStrikeout; } }
		protected override RibbonItemStyles DefaultRibbonStyle { get { return RibbonItemStyles.SmallWithoutText; } }
		#region IBarButtonGroupMember Members
		object IBarButtonGroupMember.ButtonGroupTag { get { return HomeButtonGroups.FontStyleButtonGroup; } }
		#endregion
	}
	#endregion
	#region ToggleFontSuperscriptItem
	public class ToggleFontSuperscriptItem : RichEditCommandBarCheckItem, IBarButtonGroupMember {
		public ToggleFontSuperscriptItem() {
		}
		public ToggleFontSuperscriptItem(BarManager manager)
			: base(manager) {
		}
		public ToggleFontSuperscriptItem(string caption)
			: base(caption) {
		}
		public ToggleFontSuperscriptItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		protected override RichEditCommandId CommandId { get { return RichEditCommandId.ToggleFontSuperscript; } }
		protected override RibbonItemStyles DefaultRibbonStyle { get { return RibbonItemStyles.SmallWithoutText; } }
		#region IBarButtonGroupMember Members
		object IBarButtonGroupMember.ButtonGroupTag { get { return HomeButtonGroups.FontStyleButtonGroup; } }
		#endregion
	}
	#endregion
	#region ToggleFontSubscriptItem
	public class ToggleFontSubscriptItem : RichEditCommandBarCheckItem, IBarButtonGroupMember {
		public ToggleFontSubscriptItem() {
		}
		public ToggleFontSubscriptItem(BarManager manager)
			: base(manager) {
		}
		public ToggleFontSubscriptItem(string caption)
			: base(caption) {
		}
		public ToggleFontSubscriptItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		protected override RichEditCommandId CommandId { get { return RichEditCommandId.ToggleFontSubscript; } }
		protected override RibbonItemStyles DefaultRibbonStyle { get { return RibbonItemStyles.SmallWithoutText; } }
		#region IBarButtonGroupMember Members
		object IBarButtonGroupMember.ButtonGroupTag { get { return HomeButtonGroups.FontStyleButtonGroup; } }
		#endregion
	}
	#endregion
	#region ChangeTextCaseItem
	public class ChangeTextCaseItem : RichEditCommandBarSubItem {
		public ChangeTextCaseItem() {
		}
		public ChangeTextCaseItem(BarManager manager)
			: base(manager) {
		}
		public ChangeTextCaseItem(string caption)
			: base(caption) {
		}
		public ChangeTextCaseItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		protected override RichEditCommandId CommandId { get { return RichEditCommandId.ChangeTextCasePlaceholder; } }
		protected override RibbonItemStyles DefaultRibbonStyle { get { return RibbonItemStyles.SmallWithoutText; } }
	}
	#endregion
	#region MakeTextUpperCaseItem
	public class MakeTextUpperCaseItem: RichEditCommandBarButtonItem {
		public MakeTextUpperCaseItem() {
		}
		public MakeTextUpperCaseItem(BarManager manager)
			: base(manager) {
		}
		public MakeTextUpperCaseItem(string caption)
			: base(caption) {
		}
		public MakeTextUpperCaseItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		protected override RichEditCommandId CommandId { get { return RichEditCommandId.MakeTextUpperCase; } }
	}
	#endregion
	#region MakeTextLowerCaseItem
	public class MakeTextLowerCaseItem: RichEditCommandBarButtonItem {
		public MakeTextLowerCaseItem() {
		}
		public MakeTextLowerCaseItem(BarManager manager)
			: base(manager) {
		}
		public MakeTextLowerCaseItem(string caption)
			: base(caption) {
		}
		public MakeTextLowerCaseItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		protected override RichEditCommandId CommandId { get { return RichEditCommandId.MakeTextLowerCase; } }
	}
	#endregion
	#region ToggleTextCaseItem
	public class ToggleTextCaseItem: RichEditCommandBarButtonItem {
		public ToggleTextCaseItem() {
		}
		public ToggleTextCaseItem(BarManager manager)
			: base(manager) {
		}
		public ToggleTextCaseItem(string caption)
			: base(caption) {
		}
		public ToggleTextCaseItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		protected override RichEditCommandId CommandId { get { return RichEditCommandId.ToggleTextCase; } }
	}
	#endregion
	#region CapitalizeEachWordCasiItem
	public class CapitalizeEachWordCaseItem : RichEditCommandBarButtonItem {
		public CapitalizeEachWordCaseItem() {
		}
		public CapitalizeEachWordCaseItem(BarManager manager)
			: base(manager) {
		}
		public CapitalizeEachWordCaseItem(string caption)
			: base(caption) {
		}
		public CapitalizeEachWordCaseItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		protected override RichEditCommandId CommandId { get { return RichEditCommandId.CapitalizeEachWordTextCase; } }
	}
	#endregion
	#region FontSizeIncreaseItem
	public class FontSizeIncreaseItem: RichEditCommandBarButtonItem, IBarButtonGroupMember {
		public FontSizeIncreaseItem() {
		}
		public FontSizeIncreaseItem(BarManager manager)
			: base(manager) {
		}
		public FontSizeIncreaseItem(string caption)
			: base(caption) {
		}
		public FontSizeIncreaseItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		protected override RichEditCommandId CommandId { get { return RichEditCommandId.IncreaseFontSize; } }
		protected override RibbonItemStyles DefaultRibbonStyle { get { return RibbonItemStyles.SmallWithoutText; } }
		#region IBarButtonGroupMember Members
		object IBarButtonGroupMember.ButtonGroupTag { get { return HomeButtonGroups.FontNameAndSizeButtonGroup; } }
		#endregion
	}
	#endregion
	#region FontSizeDecreaseItem
	public class FontSizeDecreaseItem: RichEditCommandBarButtonItem, IBarButtonGroupMember {
		public FontSizeDecreaseItem() {
		}
		public FontSizeDecreaseItem(BarManager manager)
			: base(manager) {
		}
		public FontSizeDecreaseItem(string caption)
			: base(caption) {
		}
		public FontSizeDecreaseItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		protected override RichEditCommandId CommandId { get { return RichEditCommandId.DecreaseFontSize; } }
		protected override RibbonItemStyles DefaultRibbonStyle { get { return RibbonItemStyles.SmallWithoutText; } }
		#region IBarButtonGroupMember Members
		object IBarButtonGroupMember.ButtonGroupTag { get { return HomeButtonGroups.FontNameAndSizeButtonGroup; } }
		#endregion
	}
	#endregion
	#region ClearFormattingItem
	public class ClearFormattingItem: RichEditCommandBarButtonItem {
		public ClearFormattingItem() {
		}
		public ClearFormattingItem(BarManager manager)
			: base(manager) {
		}
		public ClearFormattingItem(string caption)
			: base(caption) {
		}
		public ClearFormattingItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		protected override RichEditCommandId CommandId { get { return RichEditCommandId.ClearFormatting; } }
		protected override RibbonItemStyles DefaultRibbonStyle { get { return RibbonItemStyles.SmallWithoutText; } }
	}
	#endregion
	#region ShowFontFormItem
	public class ShowFontFormItem: RichEditCommandBarButtonItem {
		public ShowFontFormItem() {
		}
		public ShowFontFormItem(BarManager manager)
			: base(manager) {
		}
		public ShowFontFormItem(string caption)
			: base(caption) {
		}
		public ShowFontFormItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		protected override RichEditCommandId CommandId { get { return RichEditCommandId.ShowFontForm; } }
		protected override RibbonItemStyles DefaultRibbonStyle { get { return RibbonItemStyles.SmallWithoutText; } }
	}
	#endregion
	#region RichEditParagraphItemBuilder
	public class RichEditParagraphItemBuilder : CommandBasedBarItemBuilder {
		public override void PopulateItems(List<BarItem> items, BarCreationContextBase creationContext) {
			items.Add(new ToggleBulletedListItem());
			items.Add(new ToggleNumberingListItem());
			items.Add(new ToggleMultiLevelListItem());
			items.Add(new DecreaseIndentItem());
			items.Add(new IncreaseIndentItem());
			items.Add(new ToggleParagraphAlignmentLeftItem());
			items.Add(new ToggleParagraphAlignmentCenterItem());
			items.Add(new ToggleParagraphAlignmentRightItem());
			items.Add(new ToggleParagraphAlignmentJustifyItem());
			items.Add(new ToggleShowWhitespaceItem());
			ChangeParagraphLineSpacingItem lineSpacingItem = new ChangeParagraphLineSpacingItem();
			items.Add(lineSpacingItem);
			IBarSubItem lineSpacingSubItem = lineSpacingItem;
			lineSpacingSubItem.AddBarItem(new SetSingleParagraphSpacingItem());
			lineSpacingSubItem.AddBarItem(new SetSesquialteralParagraphSpacingItem());
			lineSpacingSubItem.AddBarItem(new SetDoubleParagraphSpacingItem());
			lineSpacingSubItem.AddBarItem(new ShowLineSpacingFormItem());
			lineSpacingSubItem.AddBarItem(new AddSpacingBeforeParagraphItem());
			lineSpacingSubItem.AddBarItem(new RemoveSpacingBeforeParagraphItem());
			lineSpacingSubItem.AddBarItem(new AddSpacingAfterParagraphItem());
			lineSpacingSubItem.AddBarItem(new RemoveSpacingAfterParagraphItem());
			items.Add(new ChangeParagraphBackColorItem());
			items.Add(new ShowParagraphFormItem());
		}
	}
	#endregion
	#region ToggleParagraphAlignmentLeftItem
	public class ToggleParagraphAlignmentLeftItem : RichEditCommandBarCheckItem, IBarButtonGroupMember {
		public ToggleParagraphAlignmentLeftItem() {
		}
		public ToggleParagraphAlignmentLeftItem(BarManager manager)
			: base(manager) {
		}
		public ToggleParagraphAlignmentLeftItem(string caption)
			: base(caption) {
		}
		public ToggleParagraphAlignmentLeftItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		protected override RichEditCommandId CommandId { get { return RichEditCommandId.ToggleParagraphAlignmentLeft; } }
		protected override RibbonItemStyles DefaultRibbonStyle { get { return RibbonItemStyles.SmallWithoutText; } }
		#region IBarButtonGroupMember Members
		object IBarButtonGroupMember.ButtonGroupTag { get { return HomeButtonGroups.ParagraphAlignmentButtonGroup; } }
		#endregion
	}
	#endregion
	#region ToggleParagraphAlignmentRightItem
	public class ToggleParagraphAlignmentRightItem : RichEditCommandBarCheckItem, IBarButtonGroupMember {
		public ToggleParagraphAlignmentRightItem() {
		}
		public ToggleParagraphAlignmentRightItem(BarManager manager)
			: base(manager) {
		}
		public ToggleParagraphAlignmentRightItem(string caption)
			: base(caption) {
		}
		public ToggleParagraphAlignmentRightItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		protected override RichEditCommandId CommandId { get { return RichEditCommandId.ToggleParagraphAlignmentRight; } }
		protected override RibbonItemStyles DefaultRibbonStyle { get { return RibbonItemStyles.SmallWithoutText; } }
		#region IBarButtonGroupMember Members
		object IBarButtonGroupMember.ButtonGroupTag { get { return HomeButtonGroups.ParagraphAlignmentButtonGroup; } }
		#endregion
	}
	#endregion
	#region ToggleParagraphAlignmentCenterItem
	public class ToggleParagraphAlignmentCenterItem : RichEditCommandBarCheckItem, IBarButtonGroupMember {
		public ToggleParagraphAlignmentCenterItem() {
		}
		public ToggleParagraphAlignmentCenterItem(BarManager manager)
			: base(manager) {
		}
		public ToggleParagraphAlignmentCenterItem(string caption)
			: base(caption) {
		}
		public ToggleParagraphAlignmentCenterItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		protected override RichEditCommandId CommandId { get { return RichEditCommandId.ToggleParagraphAlignmentCenter; } }
		protected override RibbonItemStyles DefaultRibbonStyle { get { return RibbonItemStyles.SmallWithoutText; } }
		#region IBarButtonGroupMember Members
		object IBarButtonGroupMember.ButtonGroupTag { get { return HomeButtonGroups.ParagraphAlignmentButtonGroup; } }
		#endregion
	}
	#endregion
	#region ToggleParagraphAlignmentJustifyItem
	public class ToggleParagraphAlignmentJustifyItem : RichEditCommandBarCheckItem, IBarButtonGroupMember {
		public ToggleParagraphAlignmentJustifyItem() {
		}
		public ToggleParagraphAlignmentJustifyItem(BarManager manager)
			: base(manager) {
		}
		public ToggleParagraphAlignmentJustifyItem(string caption)
			: base(caption) {
		}
		public ToggleParagraphAlignmentJustifyItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		protected override RichEditCommandId CommandId { get { return RichEditCommandId.ToggleParagraphAlignmentJustify; } }
		protected override RibbonItemStyles DefaultRibbonStyle { get { return RibbonItemStyles.SmallWithoutText; } }
		#region IBarButtonGroupMember Members
		object IBarButtonGroupMember.ButtonGroupTag { get { return HomeButtonGroups.ParagraphAlignmentButtonGroup; } }
		#endregion
	}
	#endregion
	#region ChangeParagraphLineSpacingItem
	public class ChangeParagraphLineSpacingItem : RichEditCommandBarSubItem, IBarButtonGroupMember {
		public ChangeParagraphLineSpacingItem() {
		}
		public ChangeParagraphLineSpacingItem(BarManager manager)
			: base(manager) {
		}
		public ChangeParagraphLineSpacingItem(string caption)
			: base(caption) {
		}
		public ChangeParagraphLineSpacingItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		protected override RichEditCommandId CommandId { get { return RichEditCommandId.ChangeParagraphLineSpacing; } }
		protected override RibbonItemStyles DefaultRibbonStyle { get { return RibbonItemStyles.SmallWithoutText; } }
		#region IBarButtonGroupMember Members
		object IBarButtonGroupMember.ButtonGroupTag { get { return HomeButtonGroups.ParagraphShadingButtonGroup; } }
		#endregion
	}
	#endregion
	#region SetSingleParagraphSpacingItem
	public class SetSingleParagraphSpacingItem : RichEditCommandBarCheckItem {
		public SetSingleParagraphSpacingItem() {
		}
		public SetSingleParagraphSpacingItem(BarManager manager)
			: base(manager) {
		}
		public SetSingleParagraphSpacingItem(string caption)
			: base(caption) {
		}
		public SetSingleParagraphSpacingItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		protected override RichEditCommandId CommandId { get { return RichEditCommandId.SetSingleParagraphSpacing; } }
		protected override RibbonItemStyles DefaultRibbonStyle { get { return RibbonItemStyles.SmallWithoutText; } }
	}
	#endregion
	#region SetSesquialteralParagraphSpacingItem
	public class SetSesquialteralParagraphSpacingItem : RichEditCommandBarCheckItem {
		public SetSesquialteralParagraphSpacingItem() {
		}
		public SetSesquialteralParagraphSpacingItem(BarManager manager)
			: base(manager) {
		}
		public SetSesquialteralParagraphSpacingItem(string caption)
			: base(caption) {
		}
		public SetSesquialteralParagraphSpacingItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		protected override RichEditCommandId CommandId { get { return RichEditCommandId.SetSesquialteralParagraphSpacing; } }
	}
	#endregion
	#region SetDoubleParagraphSpacingItem
	public class SetDoubleParagraphSpacingItem : RichEditCommandBarCheckItem {
		public SetDoubleParagraphSpacingItem() {
		}
		public SetDoubleParagraphSpacingItem(BarManager manager)
			: base(manager) {
		}
		public SetDoubleParagraphSpacingItem(string caption)
			: base(caption) {
		}
		public SetDoubleParagraphSpacingItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		protected override RichEditCommandId CommandId { get { return RichEditCommandId.SetDoubleParagraphSpacing; } }
	}
	#endregion
	#region ShowLineSpacingFormItem
	public class ShowLineSpacingFormItem: RichEditCommandBarButtonItem {
		public ShowLineSpacingFormItem() {
		}
		public ShowLineSpacingFormItem(BarManager manager)
			: base(manager) {
		}
		public ShowLineSpacingFormItem(string caption)
			: base(caption) {
		}
		public ShowLineSpacingFormItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		protected override RichEditCommandId CommandId { get { return RichEditCommandId.ShowLineSpacingForm; } }
	}
	#endregion
	#region AddSpacingBeforeParagraphItem
	public class AddSpacingBeforeParagraphItem: RichEditCommandBarButtonItem {
		public AddSpacingBeforeParagraphItem() {
		}
		public AddSpacingBeforeParagraphItem(BarManager manager)
			: base(manager) {
		}
		public AddSpacingBeforeParagraphItem(string caption)
			: base(caption) {
		}
		public AddSpacingBeforeParagraphItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		protected override RichEditCommandId CommandId { get { return RichEditCommandId.AddSpacingBeforeParagraph; } }
	}
	#endregion
	#region RemoveSpacingBeforeParagraphItem
	public class RemoveSpacingBeforeParagraphItem: RichEditCommandBarButtonItem {
		public RemoveSpacingBeforeParagraphItem() {
		}
		public RemoveSpacingBeforeParagraphItem(BarManager manager)
			: base(manager) {
		}
		public RemoveSpacingBeforeParagraphItem(string caption)
			: base(caption) {
		}
		public RemoveSpacingBeforeParagraphItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		protected override RichEditCommandId CommandId { get { return RichEditCommandId.RemoveSpacingBeforeParagraph; } }
	}
	#endregion
	#region AddSpacingAfterParagraphItem
	public class AddSpacingAfterParagraphItem: RichEditCommandBarButtonItem {
		public AddSpacingAfterParagraphItem() {
		}
		public AddSpacingAfterParagraphItem(BarManager manager)
			: base(manager) {
		}
		public AddSpacingAfterParagraphItem(string caption)
			: base(caption) {
		}
		public AddSpacingAfterParagraphItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		protected override RichEditCommandId CommandId { get { return RichEditCommandId.AddSpacingAfterParagraph; } }
	}
	#endregion
	#region RemoveSpacingAfterParagraphItem
	public class RemoveSpacingAfterParagraphItem: RichEditCommandBarButtonItem {
		public RemoveSpacingAfterParagraphItem() {
		}
		public RemoveSpacingAfterParagraphItem(BarManager manager)
			: base(manager) {
		}
		public RemoveSpacingAfterParagraphItem(string caption)
			: base(caption) {
		}
		public RemoveSpacingAfterParagraphItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		protected override RichEditCommandId CommandId { get { return RichEditCommandId.RemoveSpacingAfterParagraph; } }
	}
	#endregion
	#region ToggleShowWhitespaceItem
	public class ToggleShowWhitespaceItem : RichEditCommandBarCheckItem, IBarButtonGroupMember {
		public ToggleShowWhitespaceItem() {
		}
		public ToggleShowWhitespaceItem(BarManager manager)
			: base(manager) {
		}
		public ToggleShowWhitespaceItem(string caption)
			: base(caption) {
		}
		public ToggleShowWhitespaceItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		protected override RichEditCommandId CommandId { get { return RichEditCommandId.ToggleShowWhitespace; } }
		protected override RibbonItemStyles DefaultRibbonStyle { get { return RibbonItemStyles.SmallWithoutText; } }
		#region IBarButtonGroupMember Members
		object IBarButtonGroupMember.ButtonGroupTag { get { return HomeButtonGroups.ParagraphIndentButtonGroup; } }
		#endregion
	}
	#endregion
	#region ToggleNumberingListItem
	public class ToggleNumberingListItem : RichEditCommandBarCheckItem, IBarButtonGroupMember {
		public ToggleNumberingListItem() {
		}
		public ToggleNumberingListItem(BarManager manager)
			: base(manager) {
		}
		public ToggleNumberingListItem(string caption)
			: base(caption) {
		}
		public ToggleNumberingListItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		protected override RichEditCommandId CommandId { get { return RichEditCommandId.ToggleNumberingListItem; } }
		protected override RibbonItemStyles DefaultRibbonStyle { get { return RibbonItemStyles.SmallWithoutText; } }
		#region IBarButtonGroupMember Members
		object IBarButtonGroupMember.ButtonGroupTag { get { return HomeButtonGroups.NumberingListButtonGroup; } }
		#endregion
	}
	#endregion
	#region ToggleMultiLevelListItem
	public class ToggleMultiLevelListItem : RichEditCommandBarCheckItem, IBarButtonGroupMember {
		public ToggleMultiLevelListItem() {
		}
		public ToggleMultiLevelListItem(BarManager manager)
			: base(manager) {
		}
		public ToggleMultiLevelListItem(string caption)
			: base(caption) {
		}
		public ToggleMultiLevelListItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		protected override RichEditCommandId CommandId { get { return RichEditCommandId.ToggleMultilevelListItem; } }
		protected override RibbonItemStyles DefaultRibbonStyle { get { return RibbonItemStyles.SmallWithoutText; } }
		#region IBarButtonGroupMember Members
		object IBarButtonGroupMember.ButtonGroupTag { get { return HomeButtonGroups.NumberingListButtonGroup; } }
		#endregion
	}
	#endregion
	#region ToggleBulletedListItem
	public class ToggleBulletedListItem : RichEditCommandBarCheckItem, IBarButtonGroupMember {
		public ToggleBulletedListItem() {
		}
		public ToggleBulletedListItem(BarManager manager)
			: base(manager) {
		}
		public ToggleBulletedListItem(string caption)
			: base(caption) {
		}
		public ToggleBulletedListItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		protected override RichEditCommandId CommandId { get { return RichEditCommandId.ToggleBulletedListItem; } }
		protected override RibbonItemStyles DefaultRibbonStyle { get { return RibbonItemStyles.SmallWithoutText; } }
		#region IBarButtonGroupMember Members
		object IBarButtonGroupMember.ButtonGroupTag { get { return HomeButtonGroups.NumberingListButtonGroup; } }
		#endregion
	}
	#endregion
	#region IncreaseIndentItem
	public class IncreaseIndentItem: RichEditCommandBarButtonItem, IBarButtonGroupMember {
		public IncreaseIndentItem() {
		}
		public IncreaseIndentItem(BarManager manager)
			: base(manager) {
		}
		public IncreaseIndentItem(string caption)
			: base(caption) {
		}
		public IncreaseIndentItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		protected override RichEditCommandId CommandId { get { return RichEditCommandId.IncreaseIndent; } }
		protected override RibbonItemStyles DefaultRibbonStyle { get { return RibbonItemStyles.SmallWithoutText; } }
		#region IBarButtonGroupMember Members
		object IBarButtonGroupMember.ButtonGroupTag { get { return HomeButtonGroups.ParagraphIndentButtonGroup; } }
		#endregion
	}
	#endregion
	#region DecreaseIndentItem
	public class DecreaseIndentItem: RichEditCommandBarButtonItem, IBarButtonGroupMember {
		public DecreaseIndentItem() {
		}
		public DecreaseIndentItem(BarManager manager)
			: base(manager) {
		}
		public DecreaseIndentItem(string caption)
			: base(caption) {
		}
		public DecreaseIndentItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		protected override RichEditCommandId CommandId { get { return RichEditCommandId.DecreaseIndent; } }
		protected override RibbonItemStyles DefaultRibbonStyle { get { return RibbonItemStyles.SmallWithoutText; } }
		#region IBarButtonGroupMember Members
		object IBarButtonGroupMember.ButtonGroupTag { get { return HomeButtonGroups.ParagraphIndentButtonGroup; } }
		#endregion
	}
	#endregion
	#region ChangeParagraphBackColorItem
	public class ChangeParagraphBackColorItem : ChangeColorItemBase<RichEditControl, RichEditCommandId>, IBarButtonGroupMember {
		public ChangeParagraphBackColorItem() {
		}
		public ChangeParagraphBackColorItem(BarManager manager)
			: base(manager) {
		}
		public ChangeParagraphBackColorItem(string caption)
			: base(caption) {
		}
		public ChangeParagraphBackColorItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		protected override RichEditCommandId CommandId { get { return RichEditCommandId.None; } }
		protected override string DefaultColorButtonCaption { get { return RichEditExtensionsLocalizer.GetString(RichEditExtensionsStringId.Caption_NoColor); } }
		protected override RibbonItemStyles DefaultRibbonStyle { get { return RibbonItemStyles.SmallWithoutText; } }
		#region Properties
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public RichEditControl RichEditControl { get { return this.Control; } set { this.Control = value; } }
		#endregion
		protected override Command CreateCommand() {
			if (Control == null)
				return null;
			Command command = Control.CreateCommand(RichEditCommandId.ChangeParagraphBackColor);
			if (command != null) {
				command.CommandSourceType = CommandSourceType.Menu;
				return command;
			}
			return null;
		}
		protected override void DrawColorRectangle(Image image, Rectangle rect) {
			if (Color == Color.Empty) {
				DrawColorRectangleCore(image, rect, Color.FromArgb(232, 232, 232));
				using (Graphics gr = Graphics.FromImage(image)) {
					using (Pen pen = new Pen(Color.FromArgb(160, 160, 160))) {
						gr.DrawRectangle(pen, rect.X, rect.Y, rect.Width - 1, rect.Height - 1);
					}
				}
			}
			else
				DrawColorRectangleCore(image, rect, Color);
		}
		#region IBarButtonGroupMember Members
		object IBarButtonGroupMember.ButtonGroupTag { get { return HomeButtonGroups.ParagraphShadingButtonGroup; } }
		#endregion
	}
	#endregion
	#region ShowParagraphFormItem
	public class ShowParagraphFormItem: RichEditCommandBarButtonItem {
		public ShowParagraphFormItem() {
		}
		public ShowParagraphFormItem(BarManager manager)
			: base(manager) {
		}
		public ShowParagraphFormItem(string caption)
			: base(caption) {
		}
		public ShowParagraphFormItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		protected override RichEditCommandId CommandId { get { return RichEditCommandId.ShowParagraphForm; } }
		protected override RibbonItemStyles DefaultRibbonStyle { get { return RibbonItemStyles.SmallWithoutText; } }
	}
	#endregion
	#region RichEditClipboardItemBuilder
	public class RichEditClipboardItemBuilder : CommandBasedBarItemBuilder {
		public override void PopulateItems(List<BarItem> items, BarCreationContextBase creationContext) {
			items.Add(new PasteItem());
			items.Add(new CutItem());
			items.Add(new CopyItem());
			items.Add(new PasteSpecialItem());
		}
	}
	#endregion
	#region CopyItem
	public class CopyItem: RichEditCommandBarButtonItem {
		public CopyItem() {
		}
		public CopyItem(BarManager manager)
			: base(manager) {
		}
		public CopyItem(string caption)
			: base(caption) {
		}
		public CopyItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		protected override RichEditCommandId CommandId { get { return RichEditCommandId.CopySelection; } }
		protected override RibbonItemStyles DefaultRibbonStyle { get { return RibbonItemStyles.SmallWithoutText | RibbonItemStyles.SmallWithText; } }
	}
	#endregion
	#region PasteItem
	public class PasteItem: RichEditCommandBarButtonItem {
		public PasteItem() {
		}
		public PasteItem(BarManager manager)
			: base(manager) {
		}
		public PasteItem(string caption)
			: base(caption) {
		}
		public PasteItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		protected override RichEditCommandId CommandId { get { return RichEditCommandId.PasteSelection; } }
		protected override RibbonItemStyles DefaultRibbonStyle { get { return RibbonItemStyles.Large; } }
	}
	#endregion
	#region PasteSpecialItem
	public class PasteSpecialItem: RichEditCommandBarButtonItem {
		public PasteSpecialItem() {
		}
		public PasteSpecialItem(BarManager manager)
			: base(manager) {
		}
		public PasteSpecialItem(string caption)
			: base(caption) {
		}
		public PasteSpecialItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		protected override RichEditCommandId CommandId { get { return RichEditCommandId.ShowPasteSpecialForm; } }
		protected override RibbonItemStyles DefaultRibbonStyle { get { return RibbonItemStyles.SmallWithoutText | RibbonItemStyles.SmallWithText; } }
	}
	#endregion
	#region CutItem
	public class CutItem: RichEditCommandBarButtonItem {
		public CutItem() {
		}
		public CutItem(BarManager manager)
			: base(manager) {
		}
		public CutItem(string caption)
			: base(caption) {
		}
		public CutItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		protected override RichEditCommandId CommandId { get { return RichEditCommandId.CutSelection; } }
		protected override RibbonItemStyles DefaultRibbonStyle { get { return RibbonItemStyles.SmallWithoutText | RibbonItemStyles.SmallWithText; } }
	}
	#endregion
	#region RichEditEditingItemBuilder
	public class RichEditEditingItemBuilder : CommandBasedBarItemBuilder {
		public override void PopulateItems(List<BarItem> items, BarCreationContextBase creationContext) {
			items.Add(new FindItem());
			items.Add(new ReplaceItem());
		}
	}
	#endregion
	#region FindItem
	public class FindItem: RichEditCommandBarButtonItem {
		public FindItem() {
		}
		public FindItem(BarManager manager)
			: base(manager) {
		}
		public FindItem(string caption)
			: base(caption) {
		}
		public FindItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		protected override RichEditCommandId CommandId { get { return RichEditCommandId.Find; } }
		protected override RibbonItemStyles DefaultRibbonStyle { get { return RibbonItemStyles.SmallWithoutText | RibbonItemStyles.SmallWithText; } }
		protected override bool ShouldSetControlFocusAfterInvokeCommand { get { return false; } }
	}
	#endregion
	#region ReplaceItem
	public class ReplaceItem: RichEditCommandBarButtonItem {
		public ReplaceItem() {
		}
		public ReplaceItem(BarManager manager)
			: base(manager) {
		}
		public ReplaceItem(string caption)
			: base(caption) {
		}
		public ReplaceItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		protected override RichEditCommandId CommandId { get { return RichEditCommandId.Replace; } }
		protected override RibbonItemStyles DefaultRibbonStyle { get { return RibbonItemStyles.SmallWithoutText | RibbonItemStyles.SmallWithText; } }
		protected override bool ShouldSetControlFocusAfterInvokeCommand { get { return false; } }
	}
	#endregion
	#region RichEditStylesItemBuilder
	public class RichEditStylesItemBuilder : CommandBasedBarItemBuilder {
		public override void PopulateItems(List<BarItem> items, BarCreationContextBase creationContext) {
			items.Add(GetChangeStyleItem(creationContext.IsRibbon));
			items.Add(new ShowEditStyleFormItem());
		}
		BarItem GetChangeStyleItem(bool isRibbon) {
			if (!isRibbon)
				return new ChangeStyleItem();
			GalleryChangeStyleItem gallery = new GalleryChangeStyleItem();
			gallery.Gallery.Groups.Add(new GalleryItemGroup());
			return gallery;
		}
	}
	#endregion
	#region GalleryChangeStyleItem
	public class GalleryChangeStyleItem : RichEditCommandGalleryBarItem {
		#region Fields
		const string text = "AaBbCcDdEe";
		static StringFormat format = CreateFormat();
		static StringFormat styleNameFormat = CreateStyleNameFormat();
		static Font styleNameFont = CreateStyleNameFont();
		RepositoryItemRichEditStyleEdit repository;
		Size defaultImageSize = new Size(70, 50);
		const int minColumnCount = 1;
		const int maxColumnCount = 10;
		#endregion
		static StringFormat CreateFormat() {
			StringFormat result = (StringFormat)StringFormat.GenericTypographic.Clone();
			result.FormatFlags |= StringFormatFlags.NoWrap; 
			return result;
		}
		static StringFormat CreateStyleNameFormat() {
			StringFormat result = new StringFormat(StringFormatFlags.NoWrap);
			result.LineAlignment = StringAlignment.Far;
			result.Alignment = StringAlignment.Center;
			result.Trimming = StringTrimming.EllipsisCharacter;
			return result;
		}
		static Font CreateStyleNameFont() {
			return new Font("Arial", 9);
		}
		public GalleryChangeStyleItem() {
		}
		protected override void Initialize() {
			base.Initialize();
			Gallery.MinimumColumnCount = minColumnCount;
			Gallery.ColumnCount = maxColumnCount;
			this.repository = new RepositoryItemRichEditStyleEdit();
		}
		protected override void SubscribeEvents() {
			base.SubscribeEvents();
			GalleryCustomDrawItemImage += OnCustomDrawItem;
			repository.Items.CollectionChanged += OnRepositoryItemsChanged;
		}
		void OnRepositoryItemsChanged(object sender, CollectionChangeEventArgs e) {
			if (DesignMode)
				return;
			Gallery.BeginUpdate();
			try {
				PopulateGalleryItems();
			}
			finally {
				Gallery.EndUpdate();
			}
		}
		protected internal virtual void PopulateGalleryItems() {
			GalleryItemCollection galleryItems = Gallery.Groups[0].Items;
			galleryItems.Clear();
			int count = repository.Items.Count;
			for (int i = 0; i < count; i++) {
				GalleryItem galleryItem = new GalleryItem();
				RichEditStyleItem currenItem = repository.Items[i] as RichEditStyleItem;
				string localizedCaption = currenItem.Formatting.GetLocalizedCaption(Control.Model);
				galleryItem.Caption = localizedCaption;
				galleryItem.Hint = localizedCaption;
				galleryItem.Tag = currenItem.Formatting;
				galleryItems.Add(galleryItem);
			}
		}
		void OnCustomDrawItem(object sender, GalleryItemCustomDrawEventArgs e) {
			if (Control == null || Control.IsDisposed)
				return;
			GalleryItemViewInfo itemInfo = (GalleryItemViewInfo)e.ItemInfo;
			if(itemInfo.Gallery is DevExpress.XtraBars.Ribbon.Gallery.InRibbonGallery)
				Gallery.ImageSize = CalculateItemNewSize(itemInfo);
			MergedCharacterProperties characterProperties;
			string styleName = ((StyleFormattingBase)e.Item.Tag).GetLocalizedCaption(Control.Model);
			if (styleName != e.Item.Caption)
				ActualizeGalaryItem(e.Item, styleName);
			Color paragraphBackColor = Color.Empty;
			ParagraphStyleFormatting paragraphFormatting = e.Item.Tag as ParagraphStyleFormatting;
			if (paragraphFormatting != null) {
				styleName = Characters.PilcrowSign + " " + styleName;
				ParagraphStyle paragraphStyle = Control.DocumentModel.ParagraphStyles.GetStyleById(paragraphFormatting.StyleId);
				characterProperties = paragraphStyle.GetMergedWithDefaultCharacterProperties();
				paragraphBackColor = paragraphStyle.ParagraphProperties.BackColor;
			}
			else {
				CharacterStyleFormatting characterStyleFormatting = (CharacterStyleFormatting)e.Item.Tag;
				characterProperties = Control.DocumentModel.CharacterStyles.GetStyleById(characterStyleFormatting.StyleId).GetMergedWithDefaultCharacterProperties();
			}
			Rectangle itemBounds = e.Bounds;
			int itemHeight = itemBounds.Height;
			int topHeight = (int)(itemHeight * 0.45f);
			int bottomHeight = itemHeight - topHeight;
			int itemWidth = itemBounds.Width;
			GraphicsCache cache = e.Cache;
			Rectangle backgroundRect = new Rectangle(itemBounds.Left, itemBounds.Top, itemWidth, itemHeight);
			DrawGalleryBackground(Color.FromArgb(255, 255, 255), cache, backgroundRect);
			RectangleF rect = new RectangleF(itemBounds.Left, itemBounds.Top, itemWidth, topHeight);
			DrawStyleBackground(paragraphBackColor, cache, rect);
			DrawStyle(characterProperties.Info, cache, rect, itemBounds);
			Rectangle styleNameRect = new Rectangle(itemBounds.Left, itemBounds.Top + topHeight, itemWidth, bottomHeight);
			DrawStyleName(styleName, cache.Graphics, styleNameRect);
			e.Handled = true;
		}
		protected void DrawGalleryBackground(Color backgroundColor, GraphicsCache cache, RectangleF rect) {
			if (!DXColor.IsTransparentOrEmpty(backgroundColor))
				cache.Graphics.FillRectangle(cache.GetSolidBrush(backgroundColor), rect);
		}
		void ActualizeGalaryItem(GalleryItem galleryItem, string styleName) {
			galleryItem.Caption = styleName;
			galleryItem.Hint = styleName;
		}
		protected internal virtual Size CalculateItemNewSize(GalleryItemViewInfo itemInfo) {
			BaseGalleryViewInfo galleryInfo = itemInfo.GalleryInfo;
			int height = galleryInfo.ContentBounds.Height - galleryInfo.ImageBackgroundIndent.Height * 2 - galleryInfo.BackgroundPaddings.Top;
			return new Size(defaultImageSize.Width, height);
		}
		private static void DrawStyleBackground(Color backgroundColor, GraphicsCache cache, RectangleF rect) {
			if (!DXColor.IsTransparentOrEmpty(backgroundColor))
				cache.Graphics.FillRectangle(cache.GetSolidBrush(backgroundColor), rect);
		}
		protected internal virtual void DrawStyle(CharacterFormattingInfo charInfo, GraphicsCache cache, RectangleF rect, Rectangle itemBounds) {
			FontStyle style = CalculateFontStyle(charInfo);
			Font font = new Font(charInfo.FontName, charInfo.DoubleFontSize / 2f, style);
			FontFamily fontFamily = font.FontFamily;
			float ratio = font.Size / fontFamily.GetEmHeight(style);
			float height = font.GetHeight(cache.Graphics.DpiY);
			float dpi = cache.Graphics.DpiY;
			float ascent = Units.PointsToPixelsF((fontFamily.GetCellAscent(style) * ratio), dpi);
			float descent = Units.PointsToPixelsF((fontFamily.GetCellDescent(style) * ratio), dpi);
			float lineSpacing = Units.PointsToPixelsF((fontFamily.GetLineSpacing(style) * ratio), dpi);
			float free = lineSpacing - ascent - descent;
			RectangleF oldClipBounds = cache.Graphics.ClipBounds;
			RectangleF newClipBounds = new RectangleF(itemBounds.X, itemBounds.Y, itemBounds.Width, itemBounds.Height);
			newClipBounds.Intersect(oldClipBounds);
			cache.Graphics.SetClip(newClipBounds);
			rect.Y = rect.Bottom - height + descent + free;
			rect.Height = height;
			Brush brush = CalculateBrush(cache, charInfo.ForeColor);
			cache.Graphics.DrawString(text, font, brush, rect, format);
			cache.Graphics.SetClip(oldClipBounds);
		}
		protected internal virtual Brush CalculateBrush(GraphicsCache cache, Color charForeColor) {
			Color foreColor = DXColor.IsTransparentOrEmpty(charForeColor) ? Color.Black : charForeColor;
			return cache.GetSolidBrush(foreColor);
		}
		protected internal virtual FontStyle CalculateFontStyle(CharacterFormattingInfo charInfo) {
			FontStyle result = FontStyle.Regular;
			if (charInfo.FontBold)
				result |= FontStyle.Bold;
			if (charInfo.FontItalic)
				result |= FontStyle.Italic;
			return result;
		}
		protected internal virtual void DrawStyleName(string styleName, Graphics graphics, Rectangle rect) {
			graphics.DrawString(styleName, styleNameFont, Brushes.Black, rect, styleNameFormat);
		}
		protected override RichEditCommandId CommandId { get { return RichEditCommandId.ChangeFontStyle; } }
		protected override void InvokeCommand() {
			Command command = CreateCommand();
			if (command != null) {
				ICommandUIState state = CreateCommandUIState(command);
				if (command.CanExecute())
					command.ForceExecute(state);
			}
		}
		protected internal virtual ICommandUIState CreateCommandUIState(Command command) {
			IXtraRichEditFormatting formatting = SelectedItem != null ? SelectedItem.Tag as IXtraRichEditFormatting : null;
			if (formatting == null) {
				ParagraphStyle defaultStyle = Control.DocumentModel.ParagraphStyles.DefaultItem;
				formatting = new ParagraphStyleFormatting(defaultStyle.Id);
			}
			DefaultValueBasedCommandUIState<IXtraRichEditFormatting> state = new DefaultValueBasedCommandUIState<IXtraRichEditFormatting>();
			state.Value = formatting;
			return state;
		}
		protected override void OnControlChanged() {
			repository.Control = Control;
		}
		protected override ICommandUIState CreateGalleryItemUIState() {
			return new BarGalleryItemValueUIState<IXtraRichEditFormatting>(this);
		}
		protected override void OnEnabledChanged() {
			if (!Enabled) {
				GalleryItemProcessorDelegate itemsUpdateCaption = delegate(GalleryItem item) {
					item.Checked = false;
				};
				ForEachGalleryItems(itemsUpdateCaption);
			}
			base.OnEnabledChanged();
		}
	}
	#endregion
	#region ChangeStyleItem
	public class ChangeStyleItem : RichEditCommandBarEditItem<IXtraRichEditFormatting> {
		const int defaultWidth = 130;
		public ChangeStyleItem() {
			Width = defaultWidth;
		}
		public ChangeStyleItem(BarManager manager, string caption)
			: base(manager, caption) {
			Width = defaultWidth;
		}
		[DefaultValue(defaultWidth)]
		public override int Width { get { return base.Width; } set { base.Width = value; } }
		protected override RichEditCommandId CommandId { get { return RichEditCommandId.ChangeFontStyle; } }
		public override object EditValue {
			get {
				return base.EditValue;
			}
			set {
				IXtraRichEditFormatting formatting = value as IXtraRichEditFormatting;
				if (formatting != null && !(formatting is RichEditStyleItem))
					base.EditValue = new RichEditStyleItem(Control.DocumentModel, formatting);
				else
					base.EditValue = value;
			}
		}
		protected override ICommandUIState CreateCommandUIState(Command command) {
			RichEditStyleItem styleItem = EditValue as RichEditStyleItem;
			if (styleItem == null) {
				ParagraphStyle defaultStyle = Control.DocumentModel.ParagraphStyles.DefaultItem;
				styleItem = new RichEditStyleItem(Control.DocumentModel, new ParagraphStyleFormatting(defaultStyle.Id));
			}
			DefaultValueBasedCommandUIState<IXtraRichEditFormatting> state = new DefaultValueBasedCommandUIState<IXtraRichEditFormatting>();
			state.Value = styleItem.Formatting;
			return state;
		}
		protected override RepositoryItem CreateEdit() {
			RepositoryItemRichEditStyleEdit edit = new RepositoryItemRichEditStyleEdit();
			if (Control != null)
				edit.Control = Control;
			return edit;
		}
		protected override void OnControlChanged() {
			RepositoryItemRichEditStyleEdit edit = (RepositoryItemRichEditStyleEdit)Edit;
			if (edit != null)
				edit.Control = Control;
		}
		protected override void OnEditValidating(object sender, CancelEventArgs e) {
			ComboBoxEdit edit = (ComboBoxEdit)sender;
			RichEditStyleItem styleItem = edit.EditValue as RichEditStyleItem;
			if (styleItem == null) {
				edit.ErrorText = XtraRichEditLocalizer.GetString(XtraRichEditStringId.Msg_InvalidStyleName);
				e.Cancel = true;
			}
		}
	}
	#endregion
	#region ShowEditStyleFormItem
	public class ShowEditStyleFormItem: RichEditCommandBarButtonItem {
		public ShowEditStyleFormItem() {
		}
		public ShowEditStyleFormItem(BarManager manager)
			: base(manager) {
		}
		public ShowEditStyleFormItem(string caption)
			: base(caption) {
		}
		public ShowEditStyleFormItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		protected override RichEditCommandId CommandId { get { return RichEditCommandId.ShowEditStyleForm; } }
		protected override RibbonItemStyles DefaultRibbonStyle { get { return RibbonItemStyles.SmallWithoutText; } }
	}
	#endregion
	#region RichEditFontBarCreator
	public class RichEditFontBarCreator : ControlCommandBarCreator {
		public override Type SupportedRibbonPageType { get { return typeof(HomeRibbonPage); } }
		public override Type SupportedRibbonPageGroupType { get { return typeof(FontRibbonPageGroup); } }
		public override Type SupportedBarType { get { return typeof(FontBar); } }
		public override int DockRow { get { return 0; } }
		public override int DockColumn { get { return 3; } }
		public override Bar CreateBar() {
			return new FontBar();
		}
		public override CommandBasedBarItemBuilder CreateBarItemBuilder() {
			return new RichEditFontItemBuilder();
		}
		public override CommandBasedRibbonPageGroup CreateRibbonPageGroupInstance() {
			return new FontRibbonPageGroup();
		}
		public override CommandBasedRibbonPage CreateRibbonPageInstance() {
			return new HomeRibbonPage();
		}
	}
	#endregion
	#region RichEditParagraphBarCreator
	public class RichEditParagraphBarCreator : ControlCommandBarCreator {
		public override Type SupportedRibbonPageType { get { return typeof(HomeRibbonPage); } }
		public override Type SupportedRibbonPageGroupType { get { return typeof(ParagraphRibbonPageGroup); } }
		public override Type SupportedBarType { get { return typeof(ParagraphBar); } }
		public override int DockRow { get { return 0; } }
		public override int DockColumn { get { return 4; } }
		public override Bar CreateBar() {
			return new ParagraphBar();
		}
		public override CommandBasedBarItemBuilder CreateBarItemBuilder() {
			return new RichEditParagraphItemBuilder();
		}
		public override CommandBasedRibbonPageGroup CreateRibbonPageGroupInstance() {
			return new ParagraphRibbonPageGroup();
		}
		public override CommandBasedRibbonPage CreateRibbonPageInstance() {
			return new HomeRibbonPage();
		}
	}
	#endregion
	#region RichEditClipboardBarCreator
	public class RichEditClipboardBarCreator : ControlCommandBarCreator {
		public override Type SupportedRibbonPageType { get { return typeof(HomeRibbonPage); } }
		public override Type SupportedRibbonPageGroupType { get { return typeof(ClipboardRibbonPageGroup); } }
		public override Type SupportedBarType { get { return typeof(ClipboardBar); } }
		public override int DockRow { get { return 0; } }
		public override int DockColumn { get { return 1; } }
		public override Bar CreateBar() {
			return new ClipboardBar();
		}
		public override CommandBasedBarItemBuilder CreateBarItemBuilder() {
			return new RichEditClipboardItemBuilder();
		}
		public override CommandBasedRibbonPage CreateRibbonPageInstance() {
			return new HomeRibbonPage();
		}
		public override CommandBasedRibbonPageGroup CreateRibbonPageGroupInstance() {
			return new ClipboardRibbonPageGroup();
		}
	}
	#endregion
	#region RichEditEditingBarCreator
	public class RichEditEditingBarCreator : ControlCommandBarCreator {
		public override Type SupportedRibbonPageType { get { return typeof(HomeRibbonPage); } }
		public override Type SupportedRibbonPageGroupType { get { return typeof(EditingRibbonPageGroup); } }
		public override Type SupportedBarType { get { return typeof(EditingBar); } }
		public override int DockRow { get { return 0; } }
		public override int DockColumn { get { return 5; } }
		public override Bar CreateBar() {
			return new EditingBar();
		}
		public override CommandBasedBarItemBuilder CreateBarItemBuilder() {
			return new RichEditEditingItemBuilder();
		}
		public override CommandBasedRibbonPage CreateRibbonPageInstance() {
			return new HomeRibbonPage();
		}
		public override CommandBasedRibbonPageGroup CreateRibbonPageGroupInstance() {
			return new EditingRibbonPageGroup();
		}
	}
	#endregion
	#region RichEditStylesBarCreator
	public class RichEditStylesBarCreator : ControlCommandBarCreator {
		private static readonly System.Reflection.Assembly imageResourceAssembly = typeof(IRichEditControl).Assembly;
		private const string imageResourcePrefix = "DevExpress.XtraRichEdit.Images";
		private const string pageGroupImageName = "ChangeFontStyle";
		public override Type SupportedRibbonPageType { get { return typeof(HomeRibbonPage); } }
		public override Type SupportedRibbonPageGroupType { get { return typeof(StylesRibbonPageGroup); } }
		public override Type SupportedBarType { get { return typeof(StylesBar); } }
		public override int DockRow { get { return 0; } }
		public override int DockColumn { get { return 2; } }
		public override Bar CreateBar() {
			return new StylesBar();
		}
		public override CommandBasedBarItemBuilder CreateBarItemBuilder() {
			return new RichEditStylesItemBuilder();
		}
		public override CommandBasedRibbonPage CreateRibbonPageInstance() {
			return new HomeRibbonPage();
		}
		public override CommandBasedRibbonPageGroup CreateRibbonPageGroupInstance() {
			Image glyph = CommandResourceImageLoader.LoadSmallImage(imageResourcePrefix, pageGroupImageName, imageResourceAssembly);
			return new StylesRibbonPageGroup() { Glyph = glyph };
		}
	}
	#endregion
	#region FontBar
	public class FontBar : ControlCommandBasedBar<RichEditControl, RichEditCommandId> {
		public FontBar() {
		}
		public FontBar(BarManager manager)
			: base(manager) {
		}
		public FontBar(BarManager manager, string name)
			: base(manager, name) {
		}
		public override string DefaultText { get { return RichEditExtensionsLocalizer.GetString(RichEditExtensionsStringId.Caption_GroupFont); } }
	}
	#endregion
	#region ParagraphBar
	public class ParagraphBar : ControlCommandBasedBar<RichEditControl, RichEditCommandId> {
		public ParagraphBar() {
		}
		public ParagraphBar(BarManager manager)
			: base(manager) {
		}
		public ParagraphBar(BarManager manager, string name)
			: base(manager, name) {
		}
		public override string DefaultText { get { return RichEditExtensionsLocalizer.GetString(RichEditExtensionsStringId.Caption_GroupParagraph); } }
	}
	#endregion
	#region ClipboardBar
	public class ClipboardBar : ControlCommandBasedBar<RichEditControl, RichEditCommandId> {
		public ClipboardBar() {
		}
		public ClipboardBar(BarManager manager)
			: base(manager) {
		}
		public ClipboardBar(BarManager manager, string name)
			: base(manager, name) {
		}
		public override string DefaultText { get { return RichEditExtensionsLocalizer.GetString(RichEditExtensionsStringId.Caption_GroupClipboard); } }
	}
	#endregion
	#region EditingBar
	public class EditingBar : ControlCommandBasedBar<RichEditControl, RichEditCommandId> {
		public EditingBar() {
		}
		public EditingBar(BarManager manager)
			: base(manager) {
		}
		public EditingBar(BarManager manager, string name)
			: base(manager, name) {
		}
		public override string DefaultText { get { return RichEditExtensionsLocalizer.GetString(RichEditExtensionsStringId.Caption_GroupEditing); } }
	}
	#endregion
	#region StylesBar
	public class StylesBar : ControlCommandBasedBar<RichEditControl, RichEditCommandId> {
		public StylesBar() {
		}
		public StylesBar(BarManager manager)
			: base(manager) {
		}
		public StylesBar(BarManager manager, string name)
			: base(manager, name) {
		}
		public override string DefaultText { get { return RichEditExtensionsLocalizer.GetString(RichEditExtensionsStringId.Caption_GroupStyles); } }
	}
	#endregion
	#region HomeRibbonPage
	public class HomeRibbonPage : ControlCommandBasedRibbonPage {
		public HomeRibbonPage() {
		}
		public HomeRibbonPage(string text)
			: base(text) {
		}
		public override string DefaultText { get { return RichEditExtensionsLocalizer.GetString(RichEditExtensionsStringId.Caption_PageHome); } }
		protected override RibbonPage CreatePage() {
			return new HomeRibbonPage();
		}
	}
	#endregion
	#region FontRibbonPageGroup
	public class FontRibbonPageGroup : RichEditControlRibbonPageGroup {
		public FontRibbonPageGroup() {
		}
		public FontRibbonPageGroup(string text)
			: base(text) {
		}
		public override string DefaultText { get { return RichEditExtensionsLocalizer.GetString(RichEditExtensionsStringId.Caption_GroupFont); } }
		public override RichEditCommandId CommandId { get { return RichEditCommandId.ShowFontForm; } }
	}
	#endregion
	#region ParagraphRibbonPageGroup
	public class ParagraphRibbonPageGroup : RichEditControlRibbonPageGroup {
		public ParagraphRibbonPageGroup() {
		}
		public ParagraphRibbonPageGroup(string text)
			: base(text) {
		}
		public override string DefaultText { get { return RichEditExtensionsLocalizer.GetString(RichEditExtensionsStringId.Caption_GroupParagraph); } }
		public override RichEditCommandId CommandId { get { return RichEditCommandId.ShowParagraphForm; } }
	}
	#endregion
	#region ClipboardRibbonPageGroup
	public class ClipboardRibbonPageGroup : RichEditControlRibbonPageGroup {
		public ClipboardRibbonPageGroup() {
		}
		public ClipboardRibbonPageGroup(string text)
			: base(text) {
		}
		public override string DefaultText { get { return RichEditExtensionsLocalizer.GetString(RichEditExtensionsStringId.Caption_GroupClipboard); } }
	}
	#endregion
	#region EditingRibbonPageGroup
	public class EditingRibbonPageGroup : RichEditControlRibbonPageGroup {
		public EditingRibbonPageGroup() {
		}
		public EditingRibbonPageGroup(string text)
			: base(text) {
		}
		public override string DefaultText { get { return RichEditExtensionsLocalizer.GetString(RichEditExtensionsStringId.Caption_GroupEditing); } }
	}
	#endregion
	#region StylesRibbonPageGroup
	public class StylesRibbonPageGroup : RichEditControlRibbonPageGroup {
		public StylesRibbonPageGroup() {
		}
		public StylesRibbonPageGroup(string text)
			: base(text) {
		}
		public override string DefaultText { get { return RichEditExtensionsLocalizer.GetString(RichEditExtensionsStringId.Caption_GroupStyles); } }
		public override RichEditCommandId CommandId { get { return RichEditCommandId.ShowEditStyleForm; } }
		protected override void SetPage(RibbonPage page) {
			base.SetPage(page);
			AddReduceOperation(page);
		}
		protected internal virtual void AddReduceOperation(RibbonPage page) {
			if (Ribbon == null)
				return;
			ReduceOperation operation = new ReduceOperation();
			operation.Operation = ReduceOperationType.Gallery;
			operation.Behavior = ReduceOperationBehavior.UntilAvailable;
			operation.Group = this;
			operation.ItemLinkIndex = 0;
			page.ReduceOperations.Add(operation);
		}
	}
	#endregion
	#region HomeButtonGroups
	public static class HomeButtonGroups {
		public static string FontNameAndSizeButtonGroup = "{97BBE334-159B-44d9-A168-0411957565E8}";
		public static string FontStyleButtonGroup = "{433DA7F0-03E2-4650-9DB5-66DD92D16E39}";
		public static string FontColorButtonGroup = "{DF8C5334-EDE3-47c9-A42C-FE9A9247E180}";
		public static string ParagraphAlignmentButtonGroup = "{8E89E775-996E-49a0-AADA-DE338E34732E}";
		public static string ParagraphIndentButtonGroup = "{4747D5AB-2BEB-4ea6-9A1D-8E4FB36F1B40}";
		public static string ParagraphShadingButtonGroup = "{9A8DEAD8-3890-4857-A395-EC625FD02217}";
		public static string NumberingListButtonGroup = "{0B3A7A43-3079-4ce0-83A8-3789F5F6DC9F}";
	}
	#endregion
}
