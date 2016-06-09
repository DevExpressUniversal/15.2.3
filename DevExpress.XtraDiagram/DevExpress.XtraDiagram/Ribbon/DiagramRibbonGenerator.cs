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

using DevExpress.Utils;
using DevExpress.XtraBars;
using DevExpress.XtraBars.Commands;
using DevExpress.XtraBars.Commands.Design;
using DevExpress.XtraBars.Commands.Internal;
using DevExpress.XtraBars.Commands.Ribbon;
using DevExpress.XtraBars.Ribbon;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using DevExpress.Diagram.Core.Localization;
namespace DevExpress.XtraDiagram.Bars {
	#region RibbonPage Home
	public class DiagramHomeRibbonPage : ControlCommandBasedRibbonPage {
		public override string DefaultText { get { return DiagramControlLocalizer.GetString(DiagramControlStringId.RibbonPage_Home); } }
	}
	public abstract class DiagramHomeBarCreator : ControlCommandBarCreator {
		public override Type SupportedRibbonPageType { get { return typeof(DiagramHomeRibbonPage); } }
		public override Type SupportedBarType { get { return typeof(DiagramCommandBar); } }
		public override CommandBasedRibbonPage CreateRibbonPageInstance() { return new DiagramHomeRibbonPage(); }
		public override Bar CreateBar() { return new DiagramCommandBar(); }
	}
	#endregion
	#region RibbonPage View
	public class DiagramViewRibbonPage : ControlCommandBasedRibbonPage {
		public override string DefaultText { get { return DiagramControlLocalizer.GetString(DiagramControlStringId.RibbonPage_View); } }
	}
	public abstract class DiagramViewBarCreator : ControlCommandBarCreator {
		public override Type SupportedRibbonPageType { get { return typeof(DiagramViewRibbonPage); } }
		public override Type SupportedBarType { get { return typeof(DiagramCommandBar); } }
		public override CommandBasedRibbonPage CreateRibbonPageInstance() { return new DiagramViewRibbonPage(); }
		public override Bar CreateBar() { return new DiagramCommandBar(); }
	}
	#endregion
	#region RibbonPageGroup Document
	public class DiagramDocumentBarItemBuilder : DiagramCommandBasedBarItemBuilder {
		public override void PopulateItems(List<BarItem> items, BarCreationContextBase creationContext) {
			items.Add(new DiagramOpenDocumentBarItem());
			items.Add(new DiagramSaveDocumentBarItem());
			items.Add(new DiagramSaveDocumentAsBarItem());
			items.Add(new DiagramUndoBarItem());
			items.Add(new DiagramRedoBarItem());
		}
	}
	#endregion
	#region RibbonPageGroup File
	public class DiagramClipboardBarItemBuilder : DiagramCommandBasedBarItemBuilder {
		public override void PopulateItems(List<BarItem> items, BarCreationContextBase creationContext) {
			items.Add(new DiagramPasteBarItem());
			items.Add(new DiagramCutBarItem());
			items.Add(new DiagramCopyBarItem());
		}
	}
	#region RibbonPageGroup Appearance
	public class DiagramAppearanceBarItemBuilder : DiagramCommandBasedBarItemBuilder {
		public override void PopulateItems(List<BarItem> items, BarCreationContextBase creationContext) {
			items.Add(new DiagramSkinGalleryBarItem());
		}
	}
	#endregion
	public class DiagramDocumentRibbonPageGroup : DiagramRibbonPageGroup {
		public override string DefaultText { get { return DiagramControlLocalizer.GetString(DiagramControlStringId.RibbonPageGroup_Document); } }
	}
	public class DiagramClipboardRibbonPageGroup : DiagramRibbonPageGroup {
		public override string DefaultText { get { return DiagramControlLocalizer.GetString(DiagramControlStringId.RibbonPageGroup_Clipboard); } }
	}
	public class DiagramDocumentBarCreator : DiagramHomeBarCreator {
		public override Type SupportedRibbonPageGroupType { get { return typeof(DiagramDocumentRibbonPageGroup); } }
		public override CommandBasedBarItemBuilder CreateBarItemBuilder() { return new DiagramDocumentBarItemBuilder(); }
		public override CommandBasedRibbonPageGroup CreateRibbonPageGroupInstance() { return new DiagramDocumentRibbonPageGroup(); }
	}
	public class DiagramClipboardBarCreator : DiagramHomeBarCreator {
		public override Type SupportedRibbonPageGroupType { get { return typeof(DiagramClipboardRibbonPageGroup); } }
		public override CommandBasedBarItemBuilder CreateBarItemBuilder() { return new DiagramClipboardBarItemBuilder(); }
		public override CommandBasedRibbonPageGroup CreateRibbonPageGroupInstance() { return new DiagramClipboardRibbonPageGroup(); }
	}
	#endregion
	#region RibbonPageGroup Font
	public class DiagramFontBarItemBuilder : DiagramCommandBasedBarItemBuilder {
		public override void PopulateItems(List<BarItem> items, BarCreationContextBase creationContext) {
			items.Add(new DiagramFontNameEditItem());
			items.Add(new DiagramFontSizeBarItem());
			items.Add(new DiagramFontSizeIncreaseBarItem());
			items.Add(new DiagramFontSizeDecreaseBarItem());
			items.Add(new DiagramFontBoldStyleBarItem());
			items.Add(new DiagramFontItalicStyleBarItem());
			items.Add(new DiagramFontUnderlineStyleBarItem());
			items.Add(new DiagramFontStrikethroughStyleBarItem());
			items.Add(new DiagramFontBackColorBarItem());
			items.Add(new DiagramFontColorBarItem());
		}
	}
	public class DiagramFontRibbonPageGroup : DiagramRibbonPageGroup {
		public override string DefaultText { get { return DiagramControlLocalizer.GetString(DiagramControlStringId.RibbonPageGroup_Font); } }
	}
	public class DiagramFontBarCreator : DiagramHomeBarCreator {
		public override Type SupportedRibbonPageGroupType { get { return typeof(DiagramFontRibbonPageGroup); } }
		public override CommandBasedBarItemBuilder CreateBarItemBuilder() { return new DiagramFontBarItemBuilder(); }
		public override CommandBasedRibbonPageGroup CreateRibbonPageGroupInstance() { return new DiagramFontRibbonPageGroup(); }
	}
	#endregion
	#region RibbonPageGroup Paragraph
	public class DiagramParagraphBarItemBuilder : DiagramCommandBasedBarItemBuilder {
		public override void PopulateItems(List<BarItem> items, BarCreationContextBase creationContext) {
			items.Add(new DiagramAlignTopBarItem());
			items.Add(new DiagramAlignMiddleBarItem());
			items.Add(new DiagramAlignBottomBarItem());
			items.Add(new DiagramAlignLeftBarItem());
			items.Add(new DiagramAlignCenterBarItem());
			items.Add(new DiagramAlignRightBarItem());
		}
	}
	public class DiagramParagraphRibbonPageGroup : DiagramRibbonPageGroup {
		public override string DefaultText { get { return DiagramControlLocalizer.GetString(DiagramControlStringId.RibbonPageGroup_Paragraph); } }
	}
	public class DiagramParagraphBarCreator : DiagramHomeBarCreator {
		public override Type SupportedRibbonPageGroupType { get { return typeof(DiagramParagraphRibbonPageGroup); } }
		public override CommandBasedBarItemBuilder CreateBarItemBuilder() { return new DiagramParagraphBarItemBuilder(); }
		public override CommandBasedRibbonPageGroup CreateRibbonPageGroupInstance() { return new DiagramParagraphRibbonPageGroup(); }
	}
	#endregion
	#region RibbonPageGroup Tools
	public class DiagramToolsBarItemBuilder : DiagramCommandBasedBarItemBuilder {
		public override void PopulateItems(List<BarItem> items, BarCreationContextBase creationContext) {
			items.Add(new DiagramPointerToolBarItem());
			items.Add(new DiagramConnectorToolBarItem());
			items.Add(CreateShapeToolItem(items));
		}
		protected DiagramShapeToolSelectionBarItem CreateShapeToolItem(List<BarItem> items) {
			DiagramShapeToolSelectionBarItem item = new DiagramShapeToolSelectionBarItem();
			InitializeShapeToolItem(item, items);
			return item;
		}
		protected virtual void InitializeShapeToolItem(DiagramShapeToolSelectionBarItem item, List<BarItem> items) {
			BarItem[] shapeToolItems = CreateShapeToolItems();
			CreatePopupMenu(item, shapeToolItems);
			items.AddRange(shapeToolItems);
		}
		protected BarItem[] CreateShapeToolItems() {
			return new BarItem[] { new DiagramRectangleToolBarItem(), new DiagramEllipseToolBarItem(), new DiagramRightTriangleToolBarItem(), new DiagramHexagonToolBarItem() };
		}
	}
	public class DiagramToolsRibbonPageGroup : DiagramRibbonPageGroup {
		public override string DefaultText { get { return DiagramControlLocalizer.GetString(DiagramControlStringId.RibbonPageGroup_Tools); } }
	}
	public class DiagramToolsBarCreator : DiagramHomeBarCreator {
		public override Type SupportedRibbonPageGroupType { get { return typeof(DiagramToolsRibbonPageGroup); } }
		public override CommandBasedBarItemBuilder CreateBarItemBuilder() { return new DiagramToolsBarItemBuilder(); }
		public override CommandBasedRibbonPageGroup CreateRibbonPageGroupInstance() { return new DiagramToolsRibbonPageGroup(); }
	}
	#endregion
	#region RibbonPageGroup Arrange
	public class DiagramArrangeBarItemBuilder : DiagramCommandBasedBarItemBuilder {
		public override void PopulateItems(List<BarItem> items, BarCreationContextBase creationContext) {
			items.Add(CreateBringToFrontItem(items));
			items.Add(CreateSendToBackItem(items));
		}
		protected DiagramBringToFrontBarItem CreateBringToFrontItem(List<BarItem> items) {
			DiagramBringToFrontBarItem item = new DiagramBringToFrontBarItem();
			InitializeBringToFrontItem(item, items);
			return item;
		}
		protected virtual void InitializeBringToFrontItem(DiagramBringToFrontBarItem item, List<BarItem> items) {
			BarItem[] bringToFrontItems = CreateBringToFrontItems();
			CreatePopupMenu(item, bringToFrontItems);
			items.AddRange(bringToFrontItems);
		}
		protected BarItem[] CreateBringToFrontItems() {
			return new BarItem[] { new DiagramBringForwardPopupMenuBarItem(), new DiagramBringToFrontPopupMenuBarItem() };
		}
		protected DiagramSendToBackBarItem CreateSendToBackItem(List<BarItem> items) {
			DiagramSendToBackBarItem item = new DiagramSendToBackBarItem();
			InitializeSendToBackItem(item, items);
			return item;
		}
		protected virtual void InitializeSendToBackItem(DiagramSendToBackBarItem item, List<BarItem> items) {
			BarItem[] sendToBackItems = CreateSendToBackItems();
			CreatePopupMenu(item, sendToBackItems);
			items.AddRange(sendToBackItems);
		}
		protected BarItem[] CreateSendToBackItems() {
			return new BarItem[] { new DiagramSendBackwardPopupMenuBarItem(), new DiagramSendToBackPopupMenuBarItem() };
		}
	}
	public class DiagramArrangeRibbonPageGroup : DiagramRibbonPageGroup {
		public override string DefaultText { get { return DiagramControlLocalizer.GetString(DiagramControlStringId.RibbonPageGroup_Arrange); } }
	}
	public class DiagramArrangeBarCreator : DiagramHomeBarCreator {
		public override Type SupportedRibbonPageGroupType { get { return typeof(DiagramArrangeRibbonPageGroup); } }
		public override CommandBasedBarItemBuilder CreateBarItemBuilder() { return new DiagramArrangeBarItemBuilder(); }
		public override CommandBasedRibbonPageGroup CreateRibbonPageGroupInstance() { return new DiagramArrangeRibbonPageGroup(); }
	}
   #endregion
	#region RibbonPageGroup Show
	public class DiagramShowBarItemBuilder : DiagramCommandBasedBarItemBuilder {
		public override void PopulateItems(List<BarItem> items, BarCreationContextBase creationContext) {
			items.Add(new DiagramShowRulerCheckItem());
			items.Add(new DiagramShowGridCheckItem());
		}
	}
	public class DiagramShowRibbonPageGroup : DiagramRibbonPageGroup {
		public override string DefaultText { get { return DiagramControlLocalizer.GetString(DiagramControlStringId.RibbonPageGroup_Show); } }
	}
	public class DiagramShowBarCreator : DiagramViewBarCreator {
		public override Type SupportedRibbonPageGroupType { get { return typeof(DiagramShowRibbonPageGroup); } }
		public override CommandBasedBarItemBuilder CreateBarItemBuilder() { return new DiagramShowBarItemBuilder(); }
		public override CommandBasedRibbonPageGroup CreateRibbonPageGroupInstance() { return new DiagramShowRibbonPageGroup(); }
	}
	#endregion
	#region RibbonPageGroup Appearance
	public class DiagramAppearanceBarCreator : DiagramViewBarCreator {
		public override Type SupportedRibbonPageGroupType { get { return typeof(DiagramAppearanceRibbonPageGroup); } }
		public override CommandBasedBarItemBuilder CreateBarItemBuilder() { return new DiagramAppearanceBarItemBuilder(); }
		public override CommandBasedRibbonPageGroup CreateRibbonPageGroupInstance() { return new DiagramAppearanceRibbonPageGroup(); }
	}
	public class DiagramAppearanceRibbonPageGroup : DiagramRibbonPageGroup {
		public override string DefaultText { get { return DiagramControlLocalizer.GetString(DiagramControlStringId.RibbonPageGroup_Appearance); } }
	}
	#endregion
	public abstract class DiagramRibbonPageGroup : ControlCommandBasedRibbonPageGroup<DiagramControl, DiagramCommandId> {
		protected override DiagramCommandId EmptyCommandId { get { return DiagramCommandId.None; } }
	}
	public class DiagramCommandBar : ControlCommandBasedBar<DiagramControl, DiagramCommandId> {
		public override string DefaultText { get { return string.Empty; } }
	}
	public abstract class DiagramCommandBarCreator : ControlCommandBarCreator {
		public virtual bool ShouldCreateNewRibbonPageGroupInstance { get { return false; } }
		public virtual bool ShouldCreateNewRibbonPageInstance { get { return false; } }
	}
	public class DiagramBarGenerationManager : BarGenerationManager<DiagramControl, DiagramCommandId> {
		public DiagramBarGenerationManager(ControlCommandBarCreator creator, Component container, ControlCommandBarControllerBase<DiagramControl, DiagramCommandId> barController)
			: base(creator, container, barController) {
		}
	}
	public class DiagramBarGenerationManagerFactory : BarGenerationManagerFactory<DiagramControl, DiagramCommandId> {
		protected override RibbonGenerationManager<DiagramControl, DiagramCommandId> CreateRibbonGenerationManagerInstance(ControlCommandBarCreator creator, Component container, ControlCommandBarControllerBase<DiagramControl, DiagramCommandId> barController) {
			return new DiagramRibbonGenerationManager(creator, container, barController);
		}
		protected override BarGenerationManager<DiagramControl, DiagramCommandId> CreateBarGenerationManagerInstance(ControlCommandBarCreator creator, Component container, ControlCommandBarControllerBase<DiagramControl, DiagramCommandId> barController) {
			return new DiagramBarGenerationManager(creator, container, barController);
		}
	}
	public class DiagramRibbonGenerationManager : RibbonGenerationManager<DiagramControl, DiagramCommandId> {
		public DiagramRibbonGenerationManager(ControlCommandBarCreator creator, Component container, ControlCommandBarControllerBase<DiagramControl, DiagramCommandId> barController)
			: base(creator, container, barController) {
		}
		protected override void AddItemsToBarItemGroup(Component barItemGroup, List<BarItem> items) {
			base.AddItemsToBarItemGroup(barItemGroup, items);
			foreach(BarItem item in items) {
				IDiagramCommandBarItem diagramCmdItem = item as IDiagramCommandBarItem;
				if(diagramCmdItem != null && !diagramCmdItem.AddToRibbonPage)
					RibbonControl.Items.Add(item);
			}
		}
		protected override void AddItemToBarItemGroup(CommandBasedRibbonPageGroup pageGroup, BarItem item) {
			base.AddItemToBarItemGroup(pageGroup, item);
			IDiagramCommandBarItem diagramCmdItem = item as IDiagramCommandBarItem;
			if(diagramCmdItem != null) {
				if(diagramCmdItem.AddToQuickAccessToolbar) AddItemLink(RibbonControl.Toolbar.ItemLinks, item);
				if(diagramCmdItem.AddToApplicationMenu && ContainsApplicationMenu()) AddItemLink(GetApplicationMenu().ItemLinks, item);
			}
		}
		protected override bool ShouldAddItemToBarItemGroup(CommandBasedRibbonPageGroup pageGroup, BarItem item) {
			IDiagramCommandBarItem diagramCmdItem = item as IDiagramCommandBarItem;
			if(diagramCmdItem != null) {
				if(!diagramCmdItem.AddToRibbonPage) return false;
			}
			return base.ShouldAddItemToBarItemGroup(pageGroup, item);
		}
		protected override void AddBarItemToContainer(BarItem item) {
			base.AddBarItemToContainer(item);
			BarButtonItem buttonItem = item as BarButtonItem;
			if(buttonItem != null && buttonItem.DropDownControl is PopupMenu) {
				PopupMenu popupMenu = (PopupMenu)buttonItem.DropDownControl;
				GenerationStrategy.AddToContainer(popupMenu);
				popupMenu.Ribbon = RibbonControl;
			}
		}
		protected override CommandBasedRibbonPageGroup FindCommandRibbonPageGroupInPages(RibbonPageCollection pages) {
			DiagramCommandBarCreator diagramBarCreator = BarCreator as DiagramCommandBarCreator;
			if(diagramBarCreator != null && diagramBarCreator.ShouldCreateNewRibbonPageGroupInstance)
				if(!pages.Category.GetType().IsAssignableFrom(BarCreator.SupportedRibbonPageCategoryType)) return null;
			int pagesCount = pages.Count;
			for(int i = 0; i < pagesCount; i++) {
				CommandBasedRibbonPageGroup result = FindCommandRibbonPageGroupInPage(pages[i]);
				if(result != null) return result;
			}
			return null;
		}
		protected ApplicationMenu GetApplicationMenu() { return RibbonControl.ApplicationButtonDropDownControl as ApplicationMenu; }
		protected override CommandBasedRibbonPage FindCommandRibbonPage() {
			if(RibbonControl == null) return null;
			DiagramCommandBarCreator diagramBarCreator = BarCreator as DiagramCommandBarCreator;
			if(diagramBarCreator != null && diagramBarCreator.ShouldCreateNewRibbonPageInstance) return null;
			CommandBasedRibbonPage page = FindCommandRibbonPage(RibbonControl.Pages);
			if(page != null) return page;
			foreach(RibbonPageCategory category in RibbonControl.PageCategories) {
				if(BarCreator.SupportedRibbonPageCategoryType.IsAssignableFrom(category.GetType())) {
					page = FindCommandRibbonPage(category.Pages);
					if(page != null) return page;
				}
			}
			return null;
		}
		protected bool ContainsApplicationMenu() {
			return RibbonControl != null && RibbonControl.ApplicationButtonDropDownControl is ApplicationMenu;
		}
		protected override DiagramCommandId EmptyCommandId { get { return DiagramCommandId.None; } }
	}
	public class DiagramBarsHelper {
		public static void AddDefaultBars(RunTimeBarsGenerator<DiagramControl, DiagramCommandId> bg) {
			bg.BeginAddNewBars();
			try {
				bg.AddNewBars(ViewBarCreators, string.Empty, BarInsertMode.Insert);
				bg.AddNewBars(HomeBarCreators, string.Empty, BarInsertMode.Insert);
			}
			finally {
				bg.EndAddNewBars();
			}
		}
		static readonly ControlCommandBarCreator[] HomeBarCreators = new ControlCommandBarCreator[] {
			new DiagramDocumentBarCreator(),
			new DiagramClipboardBarCreator(),
			new DiagramFontBarCreator(),
			new DiagramParagraphBarCreator(),
			new DiagramToolsBarCreator(),
			new DiagramArrangeBarCreator()
		};
		static readonly ControlCommandBarCreator[] ViewBarCreators = new ControlCommandBarCreator[] {
			new DiagramShowBarCreator(),
			new DiagramAppearanceBarCreator()
		};
	}
	public class DiagramRunTimeBarsGenerator : RunTimeBarsGenerator<DiagramControl, DiagramCommandId> {
		BarManager barManager;
		DiagramControl diagram;
		public DiagramRunTimeBarsGenerator(DiagramControl diagram, BarManager barManager) : base(diagram) {
			this.diagram = diagram;
			this.barManager = barManager;
		}
		public override Component GetBarContainer() {
			List<Component> supportedBarContainerCollection = new List<Component>();
			Control control = diagram.Parent;
			while(control != null) {
				foreach(Control ctrl in control.Controls)
					if(ctrl is RibbonControl)
						supportedBarContainerCollection.Add(ctrl);
				control = control.Parent;
			}
			if(barManager != null)
				supportedBarContainerCollection.Add(barManager);
			return ChooseBarContainer(supportedBarContainerCollection);
		}
		protected override ControlCommandBarControllerBase<DiagramControl, DiagramCommandId> CreateBarController() {
			return new DiagramBarController();
		}
		protected override BarGenerationManagerFactory<DiagramControl, DiagramCommandId> CreateBarGenerationManagerFactory() {
			return new DiagramBarGenerationManagerFactory();
		}
	}
}
