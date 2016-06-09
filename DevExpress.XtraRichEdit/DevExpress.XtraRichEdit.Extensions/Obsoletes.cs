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
using DevExpress.XtraBars;
using DevExpress.XtraBars.Commands;
using DevExpress.XtraBars.Commands.Ribbon;
using DevExpress.XtraRichEdit.Internal;
using DevExpress.XtraRichEdit.Commands;
namespace DevExpress.XtraRichEdit.UI {
	#region RichEditCommandBarComponent
	[Obsolete(RichEditObsoleteText.RichEditCommandBarComponent, true)]
	public abstract class RichEditCommandBarComponent : CommandBarComponentBase, IBarItemContainer {
		#region Fields
		RichEditControl richEditControl;
		#endregion
		public RichEditCommandBarComponent()
			: base() {
		}
		public RichEditCommandBarComponent(IContainer container)
			: base(container) {
		}
		#region Properties
		[DefaultValue(null)]
		public RichEditControl RichEditControl {
			get { return richEditControl; }
			set {
				if (Object.ReferenceEquals(richEditControl, value))
					return;
				SetRichEditControl(value);
			}
		}
		#endregion
		protected override void DetachItemProviderControl() {
			this.richEditControl = null;
		}
		protected internal virtual void SetRichEditControl(RichEditControl value) {
			if (Initialization) {
				SetRichEditControlCore(value);
				return;
			}
			if (RichEditControl != null) {
				UnsubscribeRichEditControlEvents();
				DeleteVisualItems();
			}
			SetRichEditControlCore(value);
			if (RichEditControl != null) {
				CreateNewVisualItems();
				SubscribeRichEditControlEvents();
			}
		}
		protected internal virtual void SetRichEditControlCore(RichEditControl richEditControl) {
			this.richEditControl = richEditControl;
		}
		protected internal virtual void UnsubscribeRichEditControlEvents() {
			if (richEditControl != null)
				richEditControl.BeforeDispose -= new EventHandler(OnRichEditControlBeforeDispose);
		}
		protected  override void SubscribeItemProviderControlEvents() {
			SubscribeRichEditControlEvents();
		}
		protected  override void UnsubscribeItemProviderControlEvents() {
			UnsubscribeRichEditControlEvents();
		}
		protected internal virtual void SubscribeRichEditControlEvents() {
			if (richEditControl != null)
				richEditControl.BeforeDispose += new EventHandler(OnRichEditControlBeforeDispose);
		}
		protected internal virtual void OnRichEditControlBeforeDispose(object sender, EventArgs e) {
			RichEditControl = null;
		}
		protected  override void UpdateBarItem(BarItem item) {
			IControlCommandBarItem<RichEditControl, RichEditCommandId> btn = item as IControlCommandBarItem<RichEditControl, RichEditCommandId>;
			if (btn == null)
				return;
			btn.Control = richEditControl;
			base.UpdateBarItem(item);
		}
		protected override void PopulateNewItems(List<BarItem> items, BarCreationContextBase creationContext) {
			if (RichEditControl == null)
				return;
			base.PopulateNewItems(items, creationContext);
		}
		protected override bool CanCreateVisualItems() {
			return base.CanCreateVisualItems() && RichEditControl != null;
		}
		#region IBarItemContainer Members
		List<BarItem> IBarItemContainer.GetBarItems() {
			return base.GetSupportedBarItems();
		}
		#endregion
	}
	#endregion
	#region FontBarCreator
	[DXToolboxItem(false), Obsolete(RichEditObsoleteText.ObsoleteFontBarCreator, true), ToolboxBitmap(typeof(FakeClassInRootNamespace), DevExpress.Utils.ControlConstants.BitmapPath + "FontBarCreator.bmp"),
	ToolboxTabName(AssemblyInfo.DXTabRichEdit)]
	public partial class FontBarCreator : RichEditCommandBarComponent {
		public FontBarCreator()
			: base() {
		}
		public FontBarCreator(IContainer container)
			: base(container) {
		}
		protected override Type SupportedBarType { get { return typeof(FontBar); } }
		protected override Type SupportedBarItemType { get { return typeof(IRichEditControlFontBarItem); } }
		protected override CommandBasedBarItemBuilder CreateBarItemBuilder() {
			return new RichEditFontItemBuilder();
		}
		protected override CommandBasedBar CreateBarInstance() {
			return new FontBar();
		}
	}
	#endregion
	#region ParagraphBarCreator
	[DXToolboxItem(false), Obsolete(RichEditObsoleteText.ObsoleteParagraphBarCreator, true),
   ToolboxBitmap(typeof(FakeClassInRootNamespace), DevExpress.Utils.ControlConstants.BitmapPath + "ParagraphBarCreator.bmp"),
	ToolboxTabName(AssemblyInfo.DXTabRichEdit)]
	public partial class ParagraphBarCreator : RichEditCommandBarComponent {
		public ParagraphBarCreator()
			: base() {
		}
		public ParagraphBarCreator(IContainer container)
			: base(container) {
		}
		protected override Type SupportedBarType { get { return typeof(ParagraphBar); } }
		protected override Type SupportedBarItemType { get { return typeof(IRichEditControlParagraphBarItem); } }
		protected override CommandBasedBarItemBuilder CreateBarItemBuilder() {
			return new RichEditParagraphItemBuilder();
		}
		protected override CommandBasedBar CreateBarInstance() {
			return new ParagraphBar();
		}
	}
	#endregion
	#region ClipboardBarCreator
	[DXToolboxItem(false), Obsolete(RichEditObsoleteText.ObsoleteClipboardBarCreator, true),
   ToolboxBitmap(typeof(FakeClassInRootNamespace), DevExpress.Utils.ControlConstants.BitmapPath + "ClipboardBarCreator.bmp"),
	ToolboxTabName(AssemblyInfo.DXTabRichEdit)]
	public partial class ClipboardBarCreator : RichEditCommandBarComponent {
		public ClipboardBarCreator()
			: base() {
		}
		public ClipboardBarCreator(IContainer container)
			: base(container) {
		}
		protected override Type SupportedBarType { get { return typeof(ClipboardBar); } }
		protected override Type SupportedBarItemType { get { return typeof(IRichEditControlClipboardBarItem); } }
		protected override CommandBasedBarItemBuilder CreateBarItemBuilder() {
			return new RichEditClipboardItemBuilder();
		}
		protected override CommandBasedBar CreateBarInstance() {
			return new ClipboardBar();
		}
	}
	#endregion
	#region CommonBarCreator
	[DXToolboxItem(false), Obsolete(RichEditObsoleteText.ObsoleteCommonBarCreator, true),
   ToolboxBitmap(typeof(FakeClassInRootNamespace), DevExpress.Utils.ControlConstants.BitmapPath + "CommonBarCreator.bmp"),
	ToolboxTabName(AssemblyInfo.DXTabRichEdit)]
	public partial class CommonBarCreator : RichEditCommandBarComponent {
		public CommonBarCreator()
			: base() {
		}
		public CommonBarCreator(IContainer container)
			: base(container) {
		}
		protected override Type SupportedBarType { get { return typeof(CommonBar); } }
		protected override Type SupportedBarItemType { get { return typeof(IRichEditControlCommonBarItem); } }
		protected override CommandBasedBarItemBuilder CreateBarItemBuilder() {
			return new RichEditFileItemBuilder();
		}
		protected override CommandBasedBar CreateBarInstance() {
			return new CommonBar();
		}
	}
	#endregion
	#region EditingBarCreator
	[DXToolboxItem(false), Obsolete(RichEditObsoleteText.ObsoleteEditingBarCreator, true),
   ToolboxBitmap(typeof(FakeClassInRootNamespace), DevExpress.Utils.ControlConstants.BitmapPath + "EditingBarCreator.bmp"),
	ToolboxTabName(AssemblyInfo.DXTabRichEdit)]
	public partial class EditingBarCreator : RichEditCommandBarComponent {
		public EditingBarCreator()
			: base() {
		}
		public EditingBarCreator(IContainer container)
			: base(container) {
		}
		protected override Type SupportedBarType { get { return typeof(EditingBar); } }
		protected override Type SupportedBarItemType { get { return typeof(IRichEditControlEditingBarItem); } }
		protected override CommandBasedBarItemBuilder CreateBarItemBuilder() {
			return new RichEditEditingItemBuilder();
		}
		protected override CommandBasedBar CreateBarInstance() {
			return new EditingBar();
		}
	}
	#endregion
	#region StylesBarCreator
	[DXToolboxItem(false), Obsolete(RichEditObsoleteText.ObsoleteStylesBarCreator, true),
   ToolboxBitmap(typeof(FakeClassInRootNamespace), DevExpress.Utils.ControlConstants.BitmapPath + "StylesBarCreator.bmp"),
	ToolboxTabName(AssemblyInfo.DXTabRichEdit)]
	public partial class StylesBarCreator : RichEditCommandBarComponent {
		public StylesBarCreator()
			: base() {
		}
		public StylesBarCreator(IContainer container)
			: base(container) {
		}
		protected override Type SupportedBarType { get { return typeof(StylesBar); } }
		protected override Type SupportedBarItemType { get { return typeof(IRichEditControlStylesBarItem); } }
		protected override CommandBasedBarItemBuilder CreateBarItemBuilder() {
			return new RichEditStylesItemBuilder();
		}
		protected override CommandBasedBar CreateBarInstance() {
			return new StylesBar();
		}
	}
	#endregion
	#region ZoomBarCreator
	[DXToolboxItem(false), Obsolete(RichEditObsoleteText.ObsoleteZoomBarCreator, true),
   ToolboxBitmap(typeof(FakeClassInRootNamespace), DevExpress.Utils.ControlConstants.BitmapPath + "ZoomBarCreator.bmp"),
	ToolboxTabName(AssemblyInfo.DXTabRichEdit)]
	public partial class ZoomBarCreator : RichEditCommandBarComponent {
		public ZoomBarCreator()
			: base() {
		}
		public ZoomBarCreator(IContainer container)
			: base(container) {
		}
		protected override Type SupportedBarType { get { return typeof(ZoomBar); } }
		protected override Type SupportedBarItemType { get { return typeof(IRichEditControlZoomBarItem); } }
		protected override CommandBasedBarItemBuilder CreateBarItemBuilder() {
			return new RichEditZoomItemBuilder();
		}
		protected override CommandBasedBar CreateBarInstance() {
			return new ZoomBar();
		}
	}
	#endregion
	#region IllustrationsBarCreator
	[DXToolboxItem(false), Obsolete(RichEditObsoleteText.ObsoleteIllustrationsBarCreator, true),
   ToolboxBitmap(typeof(FakeClassInRootNamespace), DevExpress.Utils.ControlConstants.BitmapPath + "IllustrationsBarCreator.bmp"),
	ToolboxTabName(AssemblyInfo.DXTabRichEdit)]
	public partial class IllustrationsBarCreator : RichEditCommandBarComponent {
		public IllustrationsBarCreator()
			: base() {
		}
		public IllustrationsBarCreator(IContainer container)
			: base(container) {
		}
		protected override Type SupportedBarType { get { return typeof(IllustrationsBar); } }
		protected override Type SupportedBarItemType { get { return typeof(IRichEditControlIllustrationsBarItem); } }
		protected override CommandBasedBarItemBuilder CreateBarItemBuilder() {
			return new RichEditIllustrationsItemBuilder();
		}
		protected override CommandBasedBar CreateBarInstance() {
			return new IllustrationsBar();
		}
	}
	#endregion
	#region PagesBarCreator
	[DXToolboxItem(false), Obsolete(RichEditObsoleteText.ObsoletePagesBarCreator, true),
   ToolboxBitmap(typeof(FakeClassInRootNamespace), DevExpress.Utils.ControlConstants.BitmapPath + "PagesBarCreator.bmp"),
	ToolboxTabName(AssemblyInfo.DXTabRichEdit)]
	public partial class PagesBarCreator : RichEditCommandBarComponent {
		public PagesBarCreator()
			: base() {
		}
		public PagesBarCreator(IContainer container)
			: base(container) {
		}
		protected override Type SupportedBarType { get { return typeof(PagesBar); } }
		protected override Type SupportedBarItemType { get { return typeof(IRichEditControlPagesBarItem); } }
		protected override CommandBasedBarItemBuilder CreateBarItemBuilder() {
			return new RichEditPagesItemBuilder();
		}
		protected override CommandBasedBar CreateBarInstance() {
			return new PagesBar();
		}
	}
	#endregion
	#region FontRibbonGroupCreator
	[DXToolboxItem(false), Obsolete(RichEditObsoleteText.ObsoleteFontRibbonGroupCreator, true),
	ToolboxBitmap(typeof(FakeClassInRootNamespace), DevExpress.Utils.ControlConstants.BitmapPath + "FontRibbonGroupCreator.bmp"),
	ToolboxTabName(AssemblyInfo.DXTabRichEdit)]
	public partial class FontRibbonGroupCreator : RichEditRibbonCommandBarComponent {
		public FontRibbonGroupCreator()
			: base() {
		}
		public FontRibbonGroupCreator(IContainer container)
			: base(container) {
			container.Add(this);
		}
		#region Properties
		protected override Type SupportedRibbonPageType { get { return typeof(HomeRibbonPage); } }
		protected override Type SupportedRibbonPageGroupType { get { return typeof(FontRibbonPageGroup); } }
		protected override Type SupportedBarItemType { get { return typeof(IRichEditControlFontBarItem); } }
		#endregion
		protected override CommandBasedBarItemBuilder CreateBarItemBuilder() {
			return new RichEditFontItemBuilder();
		}
		protected override CommandBasedRibbonPage CreateRibbonPageInstance() {
			return new HomeRibbonPage();
		}
		protected override CommandBasedRibbonPageGroup CreateRibbonPageGroupInstance() {
			return new FontRibbonPageGroup();
		}
	}
	#endregion
	#region ParagraphRibbonGroupCreator
	[DXToolboxItem(false), Obsolete(RichEditObsoleteText.ObsoleteParagraphRibbonGroupCreator, true),
	ToolboxBitmap(typeof(FakeClassInRootNamespace), DevExpress.Utils.ControlConstants.BitmapPath + "ParagraphRibbonGroupCreator.bmp"),
	ToolboxTabName(AssemblyInfo.DXTabRichEdit)]
	public partial class ParagraphRibbonGroupCreator : RichEditRibbonCommandBarComponent {
		public ParagraphRibbonGroupCreator()
			: base() {
		}
		public ParagraphRibbonGroupCreator(IContainer container)
			: base(container) {
			container.Add(this);
		}
		#region Properties
		protected override Type SupportedRibbonPageType { get { return typeof(HomeRibbonPage); } }
		protected override Type SupportedRibbonPageGroupType { get { return typeof(ParagraphRibbonPageGroup); } }
		protected override Type SupportedBarItemType { get { return typeof(IRichEditControlParagraphBarItem); } }
		#endregion
		protected override CommandBasedBarItemBuilder CreateBarItemBuilder() {
			return new RichEditParagraphItemBuilder();
		}
		protected override CommandBasedRibbonPage CreateRibbonPageInstance() {
			return new HomeRibbonPage();
		}
		protected override CommandBasedRibbonPageGroup CreateRibbonPageGroupInstance() {
			return new ParagraphRibbonPageGroup();
		}
	}
	#endregion
	#region ClipboardRibbonGroupCreator
	[DXToolboxItem(false), Obsolete(RichEditObsoleteText.ObsoleteClipboardRibbonGroupCreator, true),
	ToolboxBitmap(typeof(FakeClassInRootNamespace), DevExpress.Utils.ControlConstants.BitmapPath + "ClipboardRibbonGroupCreator.bmp"),
	ToolboxTabName(AssemblyInfo.DXTabRichEdit)]
	public partial class ClipboardRibbonGroupCreator : RichEditRibbonCommandBarComponent {
		public ClipboardRibbonGroupCreator()
			: base() {
		}
		public ClipboardRibbonGroupCreator(IContainer container)
			: base(container) {
			container.Add(this);
		}
		#region Properties
		protected override Type SupportedRibbonPageType { get { return typeof(HomeRibbonPage); } }
		protected override Type SupportedRibbonPageGroupType { get { return typeof(ClipboardRibbonPageGroup); } }
		protected override Type SupportedBarItemType { get { return typeof(IRichEditControlClipboardBarItem); } }
		#endregion
		protected override CommandBasedBarItemBuilder CreateBarItemBuilder() {
			return new RichEditClipboardItemBuilder();
		}
		protected override CommandBasedRibbonPage CreateRibbonPageInstance() {
			return new HomeRibbonPage();
		}
		protected override CommandBasedRibbonPageGroup CreateRibbonPageGroupInstance() {
			return new ClipboardRibbonPageGroup();
		}
	}
	#endregion
	#region CommonRibbonGroupCreator
	[DXToolboxItem(false), Obsolete(RichEditObsoleteText.ObsoleteCommonRibbonGroupCreator, true),
	ToolboxBitmap(typeof(FakeClassInRootNamespace), DevExpress.Utils.ControlConstants.BitmapPath + "CommonRibbonGroupCreator.bmp"),
	ToolboxTabName(AssemblyInfo.DXTabRichEdit)]
	public partial class CommonRibbonGroupCreator : RichEditRibbonCommandBarComponent {
		public CommonRibbonGroupCreator()
			: base() {
		}
		public CommonRibbonGroupCreator(IContainer container)
			: base(container) {
			container.Add(this);
		}
		#region Properties
		protected override Type SupportedRibbonPageType { get { return typeof(HomeRibbonPage); } }
		protected override Type SupportedRibbonPageGroupType { get { return typeof(CommonRibbonPageGroup); } }
		protected override Type SupportedBarItemType { get { return typeof(IRichEditControlCommonBarItem); } }
		#endregion
		protected override CommandBasedBarItemBuilder CreateBarItemBuilder() {
			return new RichEditFileItemBuilder();
		}
		protected override CommandBasedRibbonPage CreateRibbonPageInstance() {
			return new HomeRibbonPage();
		}
		protected override CommandBasedRibbonPageGroup CreateRibbonPageGroupInstance() {
			return new CommonRibbonPageGroup();
		}
	}
	#endregion
	#region EditingRibbonGroupCreator
	[DXToolboxItem(false), Obsolete(RichEditObsoleteText.ObsoleteEditingRibbonGroupCreator, true),
	ToolboxBitmap(typeof(FakeClassInRootNamespace), DevExpress.Utils.ControlConstants.BitmapPath + "EditingRibbonGroupCreator.bmp"),
	ToolboxTabName(AssemblyInfo.DXTabRichEdit)]
	public partial class EditingRibbonGroupCreator : RichEditRibbonCommandBarComponent {
		public EditingRibbonGroupCreator()
			: base() {
		}
		public EditingRibbonGroupCreator(IContainer container)
			: base(container) {
			container.Add(this);
		}
		#region Properties
		protected override Type SupportedRibbonPageType { get { return typeof(HomeRibbonPage); } }
		protected override Type SupportedRibbonPageGroupType { get { return typeof(EditingRibbonPageGroup); } }
		protected override Type SupportedBarItemType { get { return typeof(IRichEditControlEditingBarItem); } }
		#endregion
		protected override CommandBasedBarItemBuilder CreateBarItemBuilder() {
			return new RichEditEditingItemBuilder();
		}
		protected override CommandBasedRibbonPage CreateRibbonPageInstance() {
			return new HomeRibbonPage();
		}
		protected override CommandBasedRibbonPageGroup CreateRibbonPageGroupInstance() {
			return new EditingRibbonPageGroup();
		}
	}
	#endregion
	#region StylesRibbonGroupCreator
	[DXToolboxItem(false), Obsolete(RichEditObsoleteText.ObsoleteStylesRibbonGroupCreator, true),
	ToolboxBitmap(typeof(FakeClassInRootNamespace), DevExpress.Utils.ControlConstants.BitmapPath + "StylesRibbonGroupCreator.bmp"),
	ToolboxTabName(AssemblyInfo.DXTabRichEdit)]
	public partial class StylesRibbonGroupCreator : RichEditRibbonCommandBarComponent {
		public StylesRibbonGroupCreator()
			: base() {
		}
		public StylesRibbonGroupCreator(IContainer container)
			: base(container) {
			container.Add(this);
		}
		#region Properties
		protected override Type SupportedRibbonPageType { get { return typeof(HomeRibbonPage); } }
		protected override Type SupportedRibbonPageGroupType { get { return typeof(StylesRibbonPageGroup); } }
		protected override Type SupportedBarItemType { get { return typeof(IRichEditControlStylesBarItem); } }
		#endregion
		protected override CommandBasedBarItemBuilder CreateBarItemBuilder() {
			return new RichEditStylesItemBuilder();
		}
		protected override CommandBasedRibbonPage CreateRibbonPageInstance() {
			return new HomeRibbonPage();
		}
		protected override CommandBasedRibbonPageGroup CreateRibbonPageGroupInstance() {
			return new StylesRibbonPageGroup();
		}
	}
	#endregion
	#region ZoomRibbonGroupCreator
	[DXToolboxItem(false), Obsolete(RichEditObsoleteText.ObsoleteZoomRibbonGroupCreator, true),
	ToolboxBitmap(typeof(FakeClassInRootNamespace), DevExpress.Utils.ControlConstants.BitmapPath + "ZoomRibbonGroupCreator.bmp"),
	ToolboxTabName(AssemblyInfo.DXTabRichEdit)]
	public partial class ZoomRibbonGroupCreator : RichEditRibbonCommandBarComponent {
		public ZoomRibbonGroupCreator()
			: base() {
		}
		public ZoomRibbonGroupCreator(IContainer container)
			: base(container) {
			container.Add(this);
		}
		#region Properties
		protected override Type SupportedRibbonPageType { get { return typeof(HomeRibbonPage); } }
		protected override Type SupportedRibbonPageGroupType { get { return typeof(ZoomRibbonPageGroup); } }
		protected override Type SupportedBarItemType { get { return typeof(IRichEditControlZoomBarItem); } }
		#endregion
		protected override CommandBasedBarItemBuilder CreateBarItemBuilder() {
			return new RichEditZoomItemBuilder();
		}
		protected override CommandBasedRibbonPage CreateRibbonPageInstance() {
			return new HomeRibbonPage();
		}
		protected override CommandBasedRibbonPageGroup CreateRibbonPageGroupInstance() {
			return new ZoomRibbonPageGroup();
		}
	}
	#endregion
	#region IllustrationsRibbonGroupCreator
	[DXToolboxItem(false), Obsolete(RichEditObsoleteText.ObsoleteIllustrationsRibbonGroupCreator, true),
	ToolboxBitmap(typeof(FakeClassInRootNamespace), DevExpress.Utils.ControlConstants.BitmapPath + "IllustrationsRibbonGroupCreator.bmp"),
	ToolboxTabName(AssemblyInfo.DXTabRichEdit)]
	public partial class IllustrationsRibbonGroupCreator : RichEditRibbonCommandBarComponent {
		public IllustrationsRibbonGroupCreator()
			: base() {
		}
		public IllustrationsRibbonGroupCreator(IContainer container)
			: base(container) {
			container.Add(this);
		}
		#region Properties
		protected override Type SupportedRibbonPageType { get { return typeof(InsertRibbonPage); } }
		protected override Type SupportedRibbonPageGroupType { get { return typeof(IllustrationsRibbonPageGroup); } }
		protected override Type SupportedBarItemType { get { return typeof(IRichEditControlIllustrationsBarItem); } }
		#endregion
		protected override CommandBasedBarItemBuilder CreateBarItemBuilder() {
			return new RichEditIllustrationsItemBuilder();
		}
		protected override CommandBasedRibbonPage CreateRibbonPageInstance() {
			return new InsertRibbonPage();
		}
		protected override CommandBasedRibbonPageGroup CreateRibbonPageGroupInstance() {
			return new IllustrationsRibbonPageGroup();
		}
	}
	#endregion
	#region PagesRibbonGroupCreator
	[DXToolboxItem(false), Obsolete(RichEditObsoleteText.ObsoletePagesRibbonGroupCreator, true),
	ToolboxBitmap(typeof(FakeClassInRootNamespace), DevExpress.Utils.ControlConstants.BitmapPath + "PagesRibbonGroupCreator.bmp"),
	ToolboxTabName(AssemblyInfo.DXTabRichEdit)]
	public partial class PagesRibbonGroupCreator : RichEditRibbonCommandBarComponent {
		public PagesRibbonGroupCreator()
			: base() {
		}
		public PagesRibbonGroupCreator(IContainer container)
			: base(container) {
			container.Add(this);
		}
		#region Properties
		protected override Type SupportedRibbonPageType { get { return typeof(InsertRibbonPage); } }
		protected override Type SupportedRibbonPageGroupType { get { return typeof(PagesRibbonPageGroup); } }
		protected override Type SupportedBarItemType { get { return typeof(IRichEditControlPagesBarItem); } }
		#endregion
		protected override CommandBasedBarItemBuilder CreateBarItemBuilder() {
			return new RichEditPagesItemBuilder();
		}
		protected override CommandBasedRibbonPage CreateRibbonPageInstance() {
			return new InsertRibbonPage();
		}
		protected override CommandBasedRibbonPageGroup CreateRibbonPageGroupInstance() {
			return new PagesRibbonPageGroup();
		}
	}
	#endregion
	#region IRichEditControlFontBarItem
	[Obsolete(RichEditObsoleteText.ObsoleteFontBarCreator, true)]
	public interface IRichEditControlFontBarItem : IControlCommandBarItem<RichEditControl, RichEditCommandId> {
	}
	#endregion
	#region IRichEditControlParagraphBarItem
	[Obsolete(RichEditObsoleteText.ObsoleteParagraphBarCreator, true)]
	public interface IRichEditControlParagraphBarItem : IControlCommandBarItem<RichEditControl, RichEditCommandId> {
	}
	#endregion
	#region IRichEditControlClipboardBarItem
	[Obsolete(RichEditObsoleteText.ObsoleteClipboardBarCreator, true)]
	public interface IRichEditControlClipboardBarItem : IControlCommandBarItem<RichEditControl, RichEditCommandId> {
	}
	#endregion
	#region IRichEditControlEditingBarItem
	[Obsolete(RichEditObsoleteText.ObsoleteEditingBarCreator, true)]
	public interface IRichEditControlEditingBarItem : IControlCommandBarItem<RichEditControl, RichEditCommandId> {
	}
	#endregion
	#region IRichEditControlCommonBarItem
	[Obsolete(RichEditObsoleteText.ObsoleteCommonBarCreator, true)]
	public interface IRichEditControlCommonBarItem : IControlCommandBarItem<RichEditControl, RichEditCommandId> {
	}
	#endregion
	#region IRichEditControlStylesBarItem
	[Obsolete(RichEditObsoleteText.ObsoleteStylesBarCreator, true)]
	public interface IRichEditControlStylesBarItem : IControlCommandBarItem<RichEditControl, RichEditCommandId> {
	}
	#endregion
	#region IRichEditControlZoomBarItem
	[Obsolete(RichEditObsoleteText.ObsoleteZoomBarCreator, true)]
	public interface IRichEditControlZoomBarItem : IControlCommandBarItem<RichEditControl, RichEditCommandId> {
	}
	#endregion
	#region IRichEditControlIllustrationsBarItem
	[Obsolete(RichEditObsoleteText.ObsoleteIllustrationsBarCreator, true)]
	public interface IRichEditControlIllustrationsBarItem : IControlCommandBarItem<RichEditControl, RichEditCommandId> {
	}
	#endregion
	#region IRichEditControlMailMergeBarItem
	[Obsolete(RichEditObsoleteText.ObsoleteCommonBarCreator, true)]
	public interface IRichEditControlMailMergeBarItem : IControlCommandBarItem<RichEditControl, RichEditCommandId> {
	}
	#endregion
	#region IRichEditControlPagesBarItem
	[Obsolete(RichEditObsoleteText.ObsoletePagesBarCreator, true)]
	public interface IRichEditControlPagesBarItem : IControlCommandBarItem<RichEditControl, RichEditCommandId> {
	}
	#endregion
}
namespace DevExpress.XtraRichEdit.Internal {
	internal static class RichEditObsoleteText {
		internal const string RichEditCommandBarComponent = "RichEditBarController is implemented instead. Use the RichEditControl smart tag action list to create bars.";
		internal const string ObsoleteFontBarCreator = "RichEditBarController is implemented instead. Use the RichEditControl smart tag action list to create a Font Bar.";
		internal const string ObsoleteParagraphBarCreator = "RichEditBarController is implemented instead. Use the RichEditControl smart tag action list to create a Paragraph Bar.";
		internal const string ObsoleteClipboardBarCreator = "RichEditBarController is implemented instead. Use the RichEditControl smart tag action list to create a Clipboard Bar.";
		internal const string ObsoleteCommonBarCreator = "RichEditBarController is implemented instead. Use the RichEditControl smart tag action list to create a Common Bar.";
		internal const string ObsoleteEditingBarCreator = "RichEditBarController is implemented instead. Use the RichEditControl smart tag action list to create an Editing Bar.";
		internal const string ObsoleteStylesBarCreator = "RichEditBarController is implemented instead. Use the RichEditControl smart tag action list to create a Styles Bar.";
		internal const string ObsoleteZoomBarCreator = "RichEditBarController is implemented instead. Use the RichEditControl smart tag action list to create a Zoom Bar.";
		internal const string ObsoleteIllustrationsBarCreator = "RichEditBarController is implemented instead. Use the RichEditControl smart tag action list to create an Illustrations Bar.";
		internal const string ObsoletePagesBarCreator = "RichEditBarController is implemented instead. Use the RichEditControl smart tag action list to create a Pages Bar.";
		internal const string ObsoleteFontRibbonGroupCreator = "RichEditBarController is implemented instead. To create a Font Ribbon group use the RichEditControl smart tag action list.";
		internal const string ObsoleteParagraphRibbonGroupCreator = "RichEditBarController is implemented instead. To create a Paragraph Ribbon group use the RichEditControl smart tag action list.";
		internal const string ObsoleteClipboardRibbonGroupCreator = "RichEditBarController is implemented instead. To create a Clipboard Ribbon group use the RichEditControl smart tag action list.";
		internal const string ObsoleteCommonRibbonGroupCreator = "RichEditBarController is implemented instead. To create a Common Ribbon group use the RichEditControl smart tag action list.";
		internal const string ObsoleteEditingRibbonGroupCreator = "RichEditBarController is implemented instead. To create an Editing Ribbon group use the RichEditControl smart tag action list.";
		internal const string ObsoleteStylesRibbonGroupCreator = "RichEditBarController is implemented instead. To create a Styles Ribbon group use the RichEditControl smart tag action list.";
		internal const string ObsoleteZoomRibbonGroupCreator = "RichEditBarController is implemented instead. To create a Zoom Ribbon group use the RichEditControl smart tag action list.";
		internal const string ObsoleteIllustrationsRibbonGroupCreator = "RichEditBarController is implemented instead. To create an Illustrations Ribbon group use the RichEditControl smart tag action list.";
		internal const string ObsoletePagesRibbonGroupCreator = "RichEditBarController is implemented instead. To create a Pages Ribbon group use the RichEditControl smart tag action list.";
	}
}
