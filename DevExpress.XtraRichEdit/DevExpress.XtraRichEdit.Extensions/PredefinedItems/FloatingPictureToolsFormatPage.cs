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
using System.Windows.Forms;
using DevExpress.Utils;
using DevExpress.Utils.Commands;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraBars;
using DevExpress.XtraBars.Commands;
using DevExpress.XtraBars.Commands.Internal;
using DevExpress.XtraBars.Commands.Ribbon;
using DevExpress.XtraRichEdit.Commands;
using DevExpress.XtraRichEdit.Forms.Design;
using DevExpress.XtraRichEdit.Localization;
using DevExpress.Office.UI;
using DevExpress.XtraBars.Ribbon;
namespace DevExpress.XtraRichEdit.UI {
	#region RichEditFloatingPictureArrangeBarCreator
	public class RichEditFloatingPictureArrangeBarCreator : ControlCommandBarCreator {
		public override Type SupportedRibbonPageType { get { return typeof(FloatingPictureToolsFormatPage); } }
		public override Type SupportedRibbonPageGroupType { get { return typeof(FloatingPictureToolsArrangePageGroup); } }
		public override Type SupportedRibbonPageCategoryType { get { return typeof(FloatingPictureToolsRibbonPageCategory); } }
		public override Type SupportedBarType { get { return typeof(FloatingPictureArrangeBar); } }
		public override int DockRow { get { return 3; } }
		public override int DockColumn { get { return 4; } }
		public override Bar CreateBar() {
			return new FloatingPictureArrangeBar();
		}
		public override CommandBasedBarItemBuilder CreateBarItemBuilder() {
			return new RichEditFloatingPictureArrangeItemBuilder();
		}
		public override CommandBasedRibbonPage CreateRibbonPageInstance() {
			return new FloatingPictureToolsFormatPage();
		}
		public override CommandBasedRibbonPageGroup CreateRibbonPageGroupInstance() {
			return new FloatingPictureToolsArrangePageGroup();
		}
		public override CommandBasedRibbonPageCategory CreateRibbonPageCategoryInstance() {
			return new FloatingPictureToolsRibbonPageCategory();
		}
	}
	#endregion
	#region RichEditFloatingPictureShapeStylesBarCreator
	public class RichEditFloatingPictureShapeStylesBarCreator : ControlCommandBarCreator {
		public override Type SupportedRibbonPageType { get { return typeof(FloatingPictureToolsFormatPage); } }
		public override Type SupportedRibbonPageGroupType { get { return typeof(FloatingPictureToolsShapeStylesPageGroup); } }
		public override Type SupportedRibbonPageCategoryType { get { return typeof(FloatingPictureToolsRibbonPageCategory); } }
		public override Type SupportedBarType { get { return typeof(FloatingPictureShapeStylesBar); } }
		public override int DockRow { get { return 3; } }
		public override int DockColumn { get { return 5; } }
		public override Bar CreateBar() {
			return new FloatingPictureShapeStylesBar();
		}
		public override CommandBasedBarItemBuilder CreateBarItemBuilder() {
			return new RichEditFloatingPictureShapeStylesItemBuilder();
		}
		public override CommandBasedRibbonPage CreateRibbonPageInstance() {
			return new FloatingPictureToolsFormatPage();
		}
		public override CommandBasedRibbonPageGroup CreateRibbonPageGroupInstance() {
			return new FloatingPictureToolsShapeStylesPageGroup();
		}
		public override CommandBasedRibbonPageCategory CreateRibbonPageCategoryInstance() {
			return new FloatingPictureToolsRibbonPageCategory();
		}
	}
	#endregion
	#region FloatingPictureToolsArrangePageGroup
	public class FloatingPictureToolsArrangePageGroup : RichEditControlRibbonPageGroup {
		public FloatingPictureToolsArrangePageGroup() {
		}
		public FloatingPictureToolsArrangePageGroup(string text)
			: base(text) {
		}
		public override string DefaultText { get { return RichEditExtensionsLocalizer.GetString(RichEditExtensionsStringId.Caption_GroupFloatingPictureToolsArrange); } }
	}
	#endregion
	#region FloatingPictureToolsFormatPage
	public class FloatingPictureToolsFormatPage : ControlCommandBasedRibbonPage {
		public FloatingPictureToolsFormatPage() {
		}
		public FloatingPictureToolsFormatPage(string text)
			: base(text) {
		}
		public override string DefaultText { get { return RichEditExtensionsLocalizer.GetString(RichEditExtensionsStringId.Caption_PageFloatingObjectPictureToolsFormat); } }
	}
	#endregion
	#region FloatingPictureArrangeBar
	public class FloatingPictureArrangeBar : ControlCommandBasedBar<RichEditControl, RichEditCommandId> {
		public FloatingPictureArrangeBar()
			: base() {
		}
		public FloatingPictureArrangeBar(BarManager manager)
			: base(manager) {
		}
		public FloatingPictureArrangeBar(BarManager manager, string name)
			: base(manager, name) {
		}
		public override string DefaultText { get { return RichEditExtensionsLocalizer.GetString(RichEditExtensionsStringId.Caption_GroupFloatingPictureToolsArrange); } }
	}
	#endregion
	#region FloatingPictureShapeStylesBar
	public class FloatingPictureShapeStylesBar : ControlCommandBasedBar<RichEditControl, RichEditCommandId> {
		public FloatingPictureShapeStylesBar()
			: base() {
		}
		public FloatingPictureShapeStylesBar(BarManager manager)
			: base(manager) {
		}
		public FloatingPictureShapeStylesBar(BarManager manager, string name)
			: base(manager, name) {
		}
		public override string DefaultText { get { return RichEditExtensionsLocalizer.GetString(RichEditExtensionsStringId.Caption_GroupFloatingPictureToolsShapeStyles); } }
	}
	#endregion
	#region RichEditFloatingPictureArrangeItemBuilder
	public class RichEditFloatingPictureArrangeItemBuilder : CommandBasedBarItemBuilder {
		public override void PopulateItems(List<BarItem> items, BarCreationContextBase creationContext) {
			ChangeFloatingObjectTextWrapTypeItem marginsTextWrapTypeItem = new ChangeFloatingObjectTextWrapTypeItem();
			items.Add(marginsTextWrapTypeItem);
			IBarSubItem marginsTextWrapTypeSubItem = marginsTextWrapTypeItem;
			marginsTextWrapTypeSubItem.AddBarItem(new SetFloatingObjectSquareTextWrapTypeItem());
			marginsTextWrapTypeSubItem.AddBarItem(new SetFloatingObjectTightTextWrapTypeItem());
			marginsTextWrapTypeSubItem.AddBarItem(new SetFloatingObjectThroughTextWrapTypeItem());
			marginsTextWrapTypeSubItem.AddBarItem(new SetFloatingObjectTopAndBottomTextWrapTypeItem());
			marginsTextWrapTypeSubItem.AddBarItem(new SetFloatingObjectBehindTextWrapTypeItem());
			marginsTextWrapTypeSubItem.AddBarItem(new SetFloatingObjectInFrontOfTextWrapTypeItem());
			ChangeFloatingObjectAlignmentItem marginsAlignmentItem = new ChangeFloatingObjectAlignmentItem();
			items.Add(marginsAlignmentItem);
			IBarSubItem marginsAlignmentSubItem = marginsAlignmentItem;
			marginsAlignmentSubItem.AddBarItem(new SetFloatingObjectTopLeftAlignmentItem());
			marginsAlignmentSubItem.AddBarItem(new SetFloatingObjectTopCenterAlignmentItem());
			marginsAlignmentSubItem.AddBarItem(new SetFloatingObjectTopRightAlignmentItem());
			marginsAlignmentSubItem.AddBarItem(new SetFloatingObjectMiddleLeftAlignmentItem());
			marginsAlignmentSubItem.AddBarItem(new SetFloatingObjectMiddleCenterAlignmentItem());
			marginsAlignmentSubItem.AddBarItem(new SetFloatingObjectMiddleRightAlignmentItem());
			marginsAlignmentSubItem.AddBarItem(new SetFloatingObjectBottomLeftAlignmentItem());
			marginsAlignmentSubItem.AddBarItem(new SetFloatingObjectBottomCenterAlignmentItem());
			marginsAlignmentSubItem.AddBarItem(new SetFloatingObjectBottomRightAlignmentItem());
			FloatingObjectBringForwardSubItem marginsBringForwardItem = new FloatingObjectBringForwardSubItem();
			items.Add(marginsBringForwardItem);
			IBarSubItem marginsBringForwardSubItem = marginsBringForwardItem;
			marginsBringForwardSubItem.AddBarItem(new FloatingObjectBringForwardItem());
			marginsBringForwardSubItem.AddBarItem(new FloatingObjectBringToFrontItem());
			marginsBringForwardSubItem.AddBarItem(new FloatingObjectBringInFrontOfTextItem());
			FloatingObjectSendBackwardSubItem marginsSendBackwardItem = new FloatingObjectSendBackwardSubItem();
			items.Add(marginsSendBackwardItem);
			IBarSubItem marginsSendBackwardSubItem = marginsSendBackwardItem;
			marginsSendBackwardSubItem.AddBarItem(new FloatingObjectSendBackwardItem());
			marginsSendBackwardSubItem.AddBarItem(new FloatingObjectSendToBackItem());
			marginsSendBackwardSubItem.AddBarItem(new FloatingObjectSendBehindTextItem());
		}
	}
	#endregion
	#region ChangeFloatingObjectTextWrapTypeItem
	public class ChangeFloatingObjectTextWrapTypeItem : RichEditCommandBarSubItem {
		public ChangeFloatingObjectTextWrapTypeItem() {
		}
		public ChangeFloatingObjectTextWrapTypeItem(BarManager manager)
			: base(manager) {
		}
		public ChangeFloatingObjectTextWrapTypeItem(string caption)
			: base(caption) {
		}
		public ChangeFloatingObjectTextWrapTypeItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		protected override RichEditCommandId CommandId { get { return RichEditCommandId.ChangeFloatingObjectTextWrapType; } }
	}
	#endregion
	#region SetFloatingObjectSquareTextWrapTypeItem
	public class SetFloatingObjectSquareTextWrapTypeItem : RichEditCommandBarCheckItem {
		public SetFloatingObjectSquareTextWrapTypeItem() {
		}
		public SetFloatingObjectSquareTextWrapTypeItem(BarManager manager)
			: base(manager) {
		}
		public SetFloatingObjectSquareTextWrapTypeItem(string caption)
			: base(caption) {
		}
		public SetFloatingObjectSquareTextWrapTypeItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		protected override RichEditCommandId CommandId { get { return RichEditCommandId.SetFloatingObjectSquareTextWrapType; } }
	}
	#endregion
	#region SetFloatingObjectTightTextWrapTypeItem
	public class SetFloatingObjectTightTextWrapTypeItem : RichEditCommandBarCheckItem {
		public SetFloatingObjectTightTextWrapTypeItem() {
		}
		public SetFloatingObjectTightTextWrapTypeItem(BarManager manager)
			: base(manager) {
		}
		public SetFloatingObjectTightTextWrapTypeItem(string caption)
			: base(caption) {
		}
		public SetFloatingObjectTightTextWrapTypeItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		protected override RichEditCommandId CommandId { get { return RichEditCommandId.SetFloatingObjectTightTextWrapType; } }
	}
	#endregion
	#region SetFloatingObjectThroughTextWrapTypeItem
	public class SetFloatingObjectThroughTextWrapTypeItem : RichEditCommandBarCheckItem {
		public SetFloatingObjectThroughTextWrapTypeItem() {
		}
		public SetFloatingObjectThroughTextWrapTypeItem(BarManager manager)
			: base(manager) {
		}
		public SetFloatingObjectThroughTextWrapTypeItem(string caption)
			: base(caption) {
		}
		public SetFloatingObjectThroughTextWrapTypeItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		protected override RichEditCommandId CommandId { get { return RichEditCommandId.SetFloatingObjectThroughTextWrapType; } }
	}
	#endregion
	#region SetFloatingObjectTopAndBottomTextWrapTypeItem
	public class SetFloatingObjectTopAndBottomTextWrapTypeItem : RichEditCommandBarCheckItem {
		public SetFloatingObjectTopAndBottomTextWrapTypeItem() {
		}
		public SetFloatingObjectTopAndBottomTextWrapTypeItem(BarManager manager)
			: base(manager) {
		}
		public SetFloatingObjectTopAndBottomTextWrapTypeItem(string caption)
			: base(caption) {
		}
		public SetFloatingObjectTopAndBottomTextWrapTypeItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		protected override RichEditCommandId CommandId { get { return RichEditCommandId.SetFloatingObjectTopAndBottomTextWrapType; } }
	}
	#endregion
	#region SetFloatingObjectBehindTextWrapTypeItem
	public class SetFloatingObjectBehindTextWrapTypeItem : RichEditCommandBarCheckItem {
		public SetFloatingObjectBehindTextWrapTypeItem() {
		}
		public SetFloatingObjectBehindTextWrapTypeItem(BarManager manager)
			: base(manager) {
		}
		public SetFloatingObjectBehindTextWrapTypeItem(string caption)
			: base(caption) {
		}
		public SetFloatingObjectBehindTextWrapTypeItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		protected override RichEditCommandId CommandId { get { return RichEditCommandId.SetFloatingObjectBehindTextWrapType; } }
	}
	#endregion
	#region SetFloatingObjectInFrontOfTextWrapTypeItem
	public class SetFloatingObjectInFrontOfTextWrapTypeItem : RichEditCommandBarCheckItem {
		public SetFloatingObjectInFrontOfTextWrapTypeItem() {
		}
		public SetFloatingObjectInFrontOfTextWrapTypeItem(BarManager manager)
			: base(manager) {
		}
		public SetFloatingObjectInFrontOfTextWrapTypeItem(string caption)
			: base(caption) {
		}
		public SetFloatingObjectInFrontOfTextWrapTypeItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		protected override RichEditCommandId CommandId { get { return RichEditCommandId.SetFloatingObjectInFrontOfTextWrapType; } }
	}
	#endregion
	#region ChangeFloatingObjectAlignmentItem
	public class ChangeFloatingObjectAlignmentItem : RichEditCommandBarSubItem {
		public ChangeFloatingObjectAlignmentItem() {
		}
		public ChangeFloatingObjectAlignmentItem(BarManager manager)
			: base(manager) {
		}
		public ChangeFloatingObjectAlignmentItem(string caption)
			: base(caption) {
		}
		public ChangeFloatingObjectAlignmentItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		protected override RichEditCommandId CommandId { get { return RichEditCommandId.ChangeFloatingObjectAlignment; } }
	}
	#endregion
	#region ChangeFloatingObjectAligmentItem (obsolete)
	[Obsolete("Please use the ChangeFloatingObjectAlignmentItem instead (with correct spelling of 'Alignment' word).", false)]
	public class ChangeFloatingObjectAligmentItem : ChangeFloatingObjectAlignmentItem {
		public ChangeFloatingObjectAligmentItem() {
		}
		public ChangeFloatingObjectAligmentItem(BarManager manager)
			: base(manager) {
		}
		public ChangeFloatingObjectAligmentItem(string caption)
			: base(caption) {
		}
		public ChangeFloatingObjectAligmentItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
	}
	#endregion
	#region SetFloatingObjectTopLeftAlignmentItem
	public class SetFloatingObjectTopLeftAlignmentItem: RichEditCommandBarButtonItem {
		public SetFloatingObjectTopLeftAlignmentItem() {
		}
		public SetFloatingObjectTopLeftAlignmentItem(BarManager manager)
			: base(manager) {
		}
		public SetFloatingObjectTopLeftAlignmentItem(string caption)
			: base(caption) {
		}
		public SetFloatingObjectTopLeftAlignmentItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		protected override RichEditCommandId CommandId { get { return RichEditCommandId.SetFloatingObjectTopLeftAlignment; } }
	}
	#endregion
	#region SetFloatingObjectTopCenterAlignmentItem
	public class SetFloatingObjectTopCenterAlignmentItem: RichEditCommandBarButtonItem {
		public SetFloatingObjectTopCenterAlignmentItem() {
		}
		public SetFloatingObjectTopCenterAlignmentItem(BarManager manager)
			: base(manager) {
		}
		public SetFloatingObjectTopCenterAlignmentItem(string caption)
			: base(caption) {
		}
		public SetFloatingObjectTopCenterAlignmentItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		protected override RichEditCommandId CommandId { get { return RichEditCommandId.SetFloatingObjectTopCenterAlignment; } }
	}
	#endregion
	#region SetFloatingObjectTopRightAlignmentItem
	public class SetFloatingObjectTopRightAlignmentItem: RichEditCommandBarButtonItem {
		public SetFloatingObjectTopRightAlignmentItem() {
		}
		public SetFloatingObjectTopRightAlignmentItem(BarManager manager)
			: base(manager) {
		}
		public SetFloatingObjectTopRightAlignmentItem(string caption)
			: base(caption) {
		}
		public SetFloatingObjectTopRightAlignmentItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		protected override RichEditCommandId CommandId { get { return RichEditCommandId.SetFloatingObjectTopRightAlignment; } }
	}
	#endregion
	#region SetFloatingObjectMiddleLeftAlignmentItem
	public class SetFloatingObjectMiddleLeftAlignmentItem: RichEditCommandBarButtonItem {
		public SetFloatingObjectMiddleLeftAlignmentItem() {
		}
		public SetFloatingObjectMiddleLeftAlignmentItem(BarManager manager)
			: base(manager) {
		}
		public SetFloatingObjectMiddleLeftAlignmentItem(string caption)
			: base(caption) {
		}
		public SetFloatingObjectMiddleLeftAlignmentItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		protected override RichEditCommandId CommandId { get { return RichEditCommandId.SetFloatingObjectMiddleLeftAlignment; } }
	}
	#endregion
	#region SetFloatingObjectMiddleCenterAlignmentItem
	public class SetFloatingObjectMiddleCenterAlignmentItem: RichEditCommandBarButtonItem {
		public SetFloatingObjectMiddleCenterAlignmentItem() {
		}
		public SetFloatingObjectMiddleCenterAlignmentItem(BarManager manager)
			: base(manager) {
		}
		public SetFloatingObjectMiddleCenterAlignmentItem(string caption)
			: base(caption) {
		}
		public SetFloatingObjectMiddleCenterAlignmentItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		protected override RichEditCommandId CommandId { get { return RichEditCommandId.SetFloatingObjectMiddleCenterAlignment; } }
	}
	#endregion
	#region SetFloatingObjectMiddleRightAlignmentItem
	public class SetFloatingObjectMiddleRightAlignmentItem: RichEditCommandBarButtonItem {
		public SetFloatingObjectMiddleRightAlignmentItem() {
		}
		public SetFloatingObjectMiddleRightAlignmentItem(BarManager manager)
			: base(manager) {
		}
		public SetFloatingObjectMiddleRightAlignmentItem(string caption)
			: base(caption) {
		}
		public SetFloatingObjectMiddleRightAlignmentItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		protected override RichEditCommandId CommandId { get { return RichEditCommandId.SetFloatingObjectMiddleRightAlignment; } }
	}
	#endregion
	#region SetFloatingObjectBottomLeftAlignmentItem
	public class SetFloatingObjectBottomLeftAlignmentItem: RichEditCommandBarButtonItem {
		public SetFloatingObjectBottomLeftAlignmentItem() {
		}
		public SetFloatingObjectBottomLeftAlignmentItem(BarManager manager)
			: base(manager) {
		}
		public SetFloatingObjectBottomLeftAlignmentItem(string caption)
			: base(caption) {
		}
		public SetFloatingObjectBottomLeftAlignmentItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		protected override RichEditCommandId CommandId { get { return RichEditCommandId.SetFloatingObjectBottomLeftAlignment; } }
	}
	#endregion
	#region SetFloatingObjectBottomCenterAlignmentItem
	public class SetFloatingObjectBottomCenterAlignmentItem: RichEditCommandBarButtonItem {
		public SetFloatingObjectBottomCenterAlignmentItem() {
		}
		public SetFloatingObjectBottomCenterAlignmentItem(BarManager manager)
			: base(manager) {
		}
		public SetFloatingObjectBottomCenterAlignmentItem(string caption)
			: base(caption) {
		}
		public SetFloatingObjectBottomCenterAlignmentItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		protected override RichEditCommandId CommandId { get { return RichEditCommandId.SetFloatingObjectBottomCenterAlignment; } }
	}
	#endregion
	#region SetFloatingObjectBottomRightAlignmentItem
	public class SetFloatingObjectBottomRightAlignmentItem: RichEditCommandBarButtonItem {
		public SetFloatingObjectBottomRightAlignmentItem() {
		}
		public SetFloatingObjectBottomRightAlignmentItem(BarManager manager)
			: base(manager) {
		}
		public SetFloatingObjectBottomRightAlignmentItem(string caption)
			: base(caption) {
		}
		public SetFloatingObjectBottomRightAlignmentItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		protected override RichEditCommandId CommandId { get { return RichEditCommandId.SetFloatingObjectBottomRightAlignment; } }
	}
	#endregion
	#region FloatingObjectBringForwardSubItem
	public class FloatingObjectBringForwardSubItem : RichEditCommandBarSubItem {
		public FloatingObjectBringForwardSubItem() {
		}
		public FloatingObjectBringForwardSubItem(BarManager manager)
			: base(manager) {
		}
		public FloatingObjectBringForwardSubItem(string caption)
			: base(caption) {
		}
		public FloatingObjectBringForwardSubItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		protected override RichEditCommandId CommandId { get { return RichEditCommandId.FloatingObjectBringForwardPlaceholder; } }
	}
	#endregion
	#region FloatingObjectBringForwardItem
	public class FloatingObjectBringForwardItem: RichEditCommandBarButtonItem {
		public FloatingObjectBringForwardItem() {
		}
		public FloatingObjectBringForwardItem(BarManager manager)
			: base(manager) {
		}
		public FloatingObjectBringForwardItem(string caption)
			: base(caption) {
		}
		public FloatingObjectBringForwardItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		protected override RichEditCommandId CommandId { get { return RichEditCommandId.FloatingObjectBringForward; } }
	}
	#endregion
	#region FloatingObjectBringToFrontItem
	public class FloatingObjectBringToFrontItem: RichEditCommandBarButtonItem {
		public FloatingObjectBringToFrontItem() {
		}
		public FloatingObjectBringToFrontItem(BarManager manager)
			: base(manager) {
		}
		public FloatingObjectBringToFrontItem(string caption)
			: base(caption) {
		}
		public FloatingObjectBringToFrontItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		protected override RichEditCommandId CommandId { get { return RichEditCommandId.FloatingObjectBringToFront; } }
	}
	#endregion
	#region FloatingObjectBringInFrontOfTextItem
	public class FloatingObjectBringInFrontOfTextItem: RichEditCommandBarButtonItem {
		public FloatingObjectBringInFrontOfTextItem() {
		}
		public FloatingObjectBringInFrontOfTextItem(BarManager manager)
			: base(manager) {
		}
		public FloatingObjectBringInFrontOfTextItem(string caption)
			: base(caption) {
		}
		public FloatingObjectBringInFrontOfTextItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		protected override RichEditCommandId CommandId { get { return RichEditCommandId.FloatingObjectBringInFrontOfText; } }
	}
	#endregion
	#region FloatingObjectSendBackwardSubItem
	public class FloatingObjectSendBackwardSubItem : RichEditCommandBarSubItem {
		public FloatingObjectSendBackwardSubItem() {
		}
		public FloatingObjectSendBackwardSubItem(BarManager manager)
			: base(manager) {
		}
		public FloatingObjectSendBackwardSubItem(string caption)
			: base(caption) {
		}
		public FloatingObjectSendBackwardSubItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		protected override RichEditCommandId CommandId { get { return RichEditCommandId.FloatingObjectSendBackwardPlaceholder; } }
	}
	#endregion
	#region FloatingObjectSendBackwardItem
	public class FloatingObjectSendBackwardItem: RichEditCommandBarButtonItem {
		public FloatingObjectSendBackwardItem() {
		}
		public FloatingObjectSendBackwardItem(BarManager manager)
			: base(manager) {
		}
		public FloatingObjectSendBackwardItem(string caption)
			: base(caption) {
		}
		public FloatingObjectSendBackwardItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		protected override RichEditCommandId CommandId { get { return RichEditCommandId.FloatingObjectSendBackward; } }
	}
	#endregion
	#region FloatingObjectSendToBackItem
	public class FloatingObjectSendToBackItem: RichEditCommandBarButtonItem {
		public FloatingObjectSendToBackItem() {
		}
		public FloatingObjectSendToBackItem(BarManager manager)
			: base(manager) {
		}
		public FloatingObjectSendToBackItem(string caption)
			: base(caption) {
		}
		public FloatingObjectSendToBackItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		protected override RichEditCommandId CommandId { get { return RichEditCommandId.FloatingObjectSendToBack; } }
	}
	#endregion
	#region FloatingObjectSendBehindTextItem
	public class FloatingObjectSendBehindTextItem: RichEditCommandBarButtonItem {
		public FloatingObjectSendBehindTextItem() {
		}
		public FloatingObjectSendBehindTextItem(BarManager manager)
			: base(manager) {
		}
		public FloatingObjectSendBehindTextItem(string caption)
			: base(caption) {
		}
		public FloatingObjectSendBehindTextItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		protected override RichEditCommandId CommandId { get { return RichEditCommandId.FloatingObjectSendBehindText; } }
	}
	#endregion
	#region FloatingPictureToolsShapeStylesPageGroup
	public class FloatingPictureToolsShapeStylesPageGroup : RichEditControlRibbonPageGroup {
		public FloatingPictureToolsShapeStylesPageGroup() {
		}
		public FloatingPictureToolsShapeStylesPageGroup(string text)
			: base(text) {
		}
		public override string DefaultText { get { return RichEditExtensionsLocalizer.GetString(RichEditExtensionsStringId.Caption_GroupFloatingPictureToolsShapeStyles); } }
	}
	#endregion
	#region RichEditFloatingPictureShapeStylesItemBuilder
	public class RichEditFloatingPictureShapeStylesItemBuilder : CommandBasedBarItemBuilder {
		public override void PopulateItems(List<BarItem> items, BarCreationContextBase creationContext) {
			items.Add(new ChangeFloatingObjectFillColorItem());
			items.Add(new ChangeFloatingObjectOutlineColorItem());
			items.Add(new ChangeFloatingObjectOutlineWeightItem());
		}
	}
	#endregion
	#region ChangeFloatingObjectFillColorItem
	public class ChangeFloatingObjectFillColorItem : ChangeColorItemBase<RichEditControl, RichEditCommandId> {
		public ChangeFloatingObjectFillColorItem() {
		}
		public ChangeFloatingObjectFillColorItem(BarManager manager)
			: base(manager) {
		}
		public ChangeFloatingObjectFillColorItem(string caption)
			: base(caption) {
		}
		public ChangeFloatingObjectFillColorItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		protected override RichEditCommandId CommandId { get { return RichEditCommandId.None; } }
		protected override string DefaultColorButtonCaption { get { return RichEditExtensionsLocalizer.GetString(RichEditExtensionsStringId.Caption_NoFill); } }
		protected override RibbonItemStyles DefaultRibbonStyle { get { return RibbonItemStyles.SmallWithoutText | RibbonItemStyles.SmallWithText; } }
		#region Properties
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public RichEditControl RichEditControl { get { return this.Control; } set { this.Control = value; } }
		#endregion
		protected override Command CreateCommand() {
			if (Control == null)
				return null;
			Command command = new ChangeFloatingObjectFillColorCommand(Control);
			command.CommandSourceType = CommandSourceType.Menu;
			return command;
		}
	}
	#endregion
	#region ChangeFloatingObjectOutlineColorItem
	public class ChangeFloatingObjectOutlineColorItem : ChangeColorItemBase<RichEditControl, RichEditCommandId> {
		public ChangeFloatingObjectOutlineColorItem() {
		}
		public ChangeFloatingObjectOutlineColorItem(BarManager manager)
			: base(manager) {
		}
		public ChangeFloatingObjectOutlineColorItem(string caption)
			: base(caption) {
		}
		public ChangeFloatingObjectOutlineColorItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		protected override RichEditCommandId CommandId { get { return RichEditCommandId.None; } }
		protected override string DefaultColorButtonCaption { get { return RichEditExtensionsLocalizer.GetString(RichEditExtensionsStringId.Caption_NoOutline); } }
		protected override RibbonItemStyles DefaultRibbonStyle { get { return RibbonItemStyles.SmallWithoutText | RibbonItemStyles.SmallWithText; } }
		#region Properties
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public RichEditControl RichEditControl { get { return this.Control; } set { this.Control = value; } }
		#endregion
		protected override Command CreateCommand() {
			if (Control == null)
				return null;
			Command command = new ChangeFloatingObjectOutlineColorCommand(Control);
			command.CommandSourceType = CommandSourceType.Menu;
			return command;
		}
	}
	#endregion
	#region ChangeFloatingObjectOutlineWeightItem
	public class ChangeFloatingObjectOutlineWeightItem : RichEditCommandBarEditItem<int> {
		const int defaultWidth = 130;
		public ChangeFloatingObjectOutlineWeightItem() {
			Width = defaultWidth;
		}
		public ChangeFloatingObjectOutlineWeightItem(BarManager manager, string caption)
			: base(manager, caption) {
			Width = defaultWidth;
		}
		[DefaultValue(defaultWidth)]
		public override int Width { get { return base.Width; } set { base.Width = value; } }
		protected override RichEditCommandId CommandId { get { return RichEditCommandId.ChangeFloatingObjectOutlineWeight; } }
		protected override ICommandUIState CreateCommandUIState(Command command) {
			int weight;
			if (EditValue is int)
				weight = (int)EditValue;
			else
				weight = 0; 
			DefaultValueBasedCommandUIState<int> state = new DefaultValueBasedCommandUIState<int>();
			state.Value = weight;
			return state;
		}
		protected override RepositoryItem CreateEdit() {
			RepositoryItemFloatingObjectOutlineWeight edit = new RepositoryItemFloatingObjectOutlineWeight();
			if (Control != null)
				edit.Control = Control;
			return edit;
		}
		protected override void OnControlChanged() {
			RepositoryItemFloatingObjectOutlineWeight edit = (RepositoryItemFloatingObjectOutlineWeight)Edit;
			if (edit != null) {
				edit.Control = Control;
				if (Control == null)
					EditValue = null;
				else
					EditValue = Math.Max(1, Control.DocumentModel.UnitConverter.PointsToModelUnits(1));
			}
		}
	}
	#endregion
	#region FloatingPictureToolsRibbonPageCategory
	public class FloatingPictureToolsRibbonPageCategory : ControlCommandBasedRibbonPageCategory<RichEditControl, RichEditCommandId> {
		public FloatingPictureToolsRibbonPageCategory() {
			this.Color = DXColor.FromArgb(0xff, 0xc9, 0x0, 0x77);
		}
		public override string DefaultText { get { return RichEditExtensionsLocalizer.GetString(RichEditExtensionsStringId.Caption_PageCategoryFloatingPictureTools); } }
		protected override RichEditCommandId EmptyCommandId { get { return RichEditCommandId.None; } }
		public override RichEditCommandId CommandId { get { return RichEditCommandId.ToolsFloatingPictureCommandGroup; } }
		protected override RibbonPageCategory CreateRibbonPageCategory() {
			return new FloatingPictureToolsRibbonPageCategory();
		}
	}
	#endregion
}
