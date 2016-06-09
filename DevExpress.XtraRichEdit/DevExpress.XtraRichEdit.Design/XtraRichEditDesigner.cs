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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Reflection;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using DevExpress.Utils.Design;
using DevExpress.Utils.Menu;
using DevExpress.XtraBars;
using DevExpress.XtraBars.Commands;
using DevExpress.XtraBars.Commands.Design;
using DevExpress.XtraBars.Commands.Internal;
using DevExpress.XtraRichEdit.Commands;
using DevExpress.XtraRichEdit.Design;
using DevExpress.XtraRichEdit.UI;
using DevExpress.XtraSpellChecker;
using DevExpress.XtraBars.Ribbon;
using DevExpress.XtraBars.Docking;
using DevExpress.XtraRichEdit;
using DevExpress.XtraRichEdit.Localization;
namespace DevExpress.XtraRichEdit.Design {
	#region XtraRichEditDesigner
	public class XtraRichEditDesigner : BaseControlDesigner, IServiceProvider {
		#region Fields
		IComponentChangeService changeService;
		IDesignerHost host;
		DesignerVerbCollection verbs;
		#endregion
		public XtraRichEditDesigner() {
			XtraRichEditRepositoryItemsRegistrator.Register();
		}
		#region Properties
		public RichEditControl RichEditControl { get { return Control as RichEditControl; } }
		public IComponentChangeService ChangeService { get { return changeService; } }
		public IDesignerHost DesignerHost {
			get {
				if (host == null) {
					host = (IDesignerHost)GetService(typeof(IDesignerHost));
				}
				return host;
			}
		}
		protected override bool AllowHookDebugMode { get { return true; } }
		public override DesignerVerbCollection DXVerbs {
			get {
				if (this.verbs == null)
					CreateVerbs();
				return verbs;
			}
		}
		#endregion
		protected virtual void CreateVerbs() {
			this.verbs = new DesignerVerbCollection(new DesignerVerb[] { new DesignerVerb("About", new EventHandler(OnAboutClick)) });
		}
		protected virtual void OnAboutClick(object sender, EventArgs e) {
			RichEditControl.About();
		}
		object IServiceProvider.GetService(Type type) {
			return base.GetService(type);
		}
		public override void InitializeNewComponent(IDictionary defaultValues) {
			base.InitializeNewComponent(defaultValues);
			RichEditAutomaticBindHelper.BindToSpellChecker(this);
			RichEditAutomaticBindHelper.BindToCommentsControl(this);
			DesignerActionUIService service = EditorContextHelperEx.GetDesignerUIService(Component);
			if (service != null)
				service.ShowUI(Component);
		}
		public override void Initialize(IComponent component) {
			base.Initialize(component);
			Control control = component as Control;
			if(control != null)
				control.AllowDrop = false;
			this.changeService = (IComponentChangeService)GetService(typeof(IComponentChangeService));
			if (changeService != null) {
				changeService.ComponentAdded += OnComponentAdded;
				changeService.ComponentRemoved += OnComponentRemoved;
			}
		}
		protected override void RegisterActionLists(DesignerActionListCollection list) {
			list.Add(CreateRichEditAllBarsActionList());
			list.Add(CreateRichEditActionList());
			DesignerActionList commentsActionList = CreateRichEditCommentsActionList();
			if (commentsActionList != null)
				list.Add(commentsActionList);
			base.RegisterActionLists(list);
		}
		protected virtual DesignerActionList CreateRichEditAllBarsActionList() {
			return new RichEditAllBarsActionList(this);
		}
		protected virtual DesignerActionList CreateRichEditActionList() {
			return new RichEditBarsActionList(this);
		}
		protected virtual DesignerActionList CreateRichEditCommentsActionList() {
			return new RichEditCommentsActionList(this);
		}
		protected override DXAboutActionList GetAboutAction() {
			return new DXAboutActionList(Component, new MethodInvoker(RichEditControl.About));
		}
		protected virtual void OnComponentRemoved(object sender, ComponentEventArgs e) {
			AutomaticBindHelper.UnbindFromRemovedComponent(this, e.Component, "SpellChecker", typeof(ISpellChecker));
			AutomaticBindHelper.UnbindFromRemovedComponent(this, e.Component, "CommentsControl", typeof(IRichEditControl));
			AutomaticBindHelper.UnbindFromRemovedComponent(this, e.Component, "MenuManager", typeof(IDXMenuManager));
		}
		protected virtual void OnComponentAdded(object sender, ComponentEventArgs e) {
			RichEditAutomaticBindHelper.BindToSpellChecker(this);
			RichEditAutomaticBindHelper.BindToCommentsControl(this);
			AutomaticBindHelper.BindToComponent(this, "MenuManager", typeof(IDXMenuManager));
		}
		public void AddAllBars(bool clearExistingItemsBefore, BarInsertMode insertMode) {
			if (RichEditDesignTimeBarsGenerator.IsExistBarContainer(Component)) {
				RichEditAllBarsActionList actionList = new RichEditAllBarsActionList(this);
				actionList.CreateAllBarsCore(clearExistingItemsBefore, insertMode);
			}
		}
	}
	#endregion
	#region XtraRichEditCommentDesigner
	public class XtraRichEditCommentDesigner : XtraRichEditDesigner {
		protected override DesignerActionListCollection CreateActionLists() {
			return null;
		}
		protected override DesignerActionList CreateRichEditActionList() {
			return null;
		}
		protected override void OnComponentAdded(object sender, ComponentEventArgs e) {
			base.OnComponentAdded(sender, e);
		}
	}
	#endregion
	#region RichEditAutomaticBindHelper
	public static class RichEditAutomaticBindHelper {
		public static bool BindToSpellChecker(ControlDesigner designer) {
			return AutomaticBindHelper.BindToComponent(designer, "SpellChecker", typeof(ISpellChecker));
		}
		public static bool BindToCommentsControl(ControlDesigner designer) {
			return AutomaticBindHelper.BindToComponent(designer, "CommentsControl", typeof(IRichEditControl));
		} 
	}
	#endregion
	#region RichEditBarsActionList
	public class RichEditBarsActionList : DesignerActionList {
		readonly XtraRichEditDesigner designer;
		public RichEditBarsActionList(XtraRichEditDesigner designer)
			: base(designer.Component) {
			this.designer = designer;
		}
		public XtraRichEditDesigner Designer { get { return designer; } }
		public override DesignerActionItemCollection GetSortedActionItems() {
			DesignerActionItemCollection result = new DesignerActionItemCollection();
			if(designer.IsDebugging())
				return result;
			if (!RichEditDesignTimeBarsGenerator.IsExistBarContainer(Component)) {
				PopulateCreateBarsItems(result);
				return result;
			}
			RichEditDesignTimeBarsGenerator generator = CreateGenerator();
			bool hasRibbonControl = generator != null ? generator.HasRibbonControl() : false;
			PopulateSortedActionItems(result, hasRibbonControl);
			return result;
		}
		protected internal virtual void PopulateCreateBarsItems(DesignerActionItemCollection items) {
			items.Add(new DesignerActionMethodItem(this, "CreateRibbon", " Create Ribbon", "Toolbar"));
			items.Add(new DesignerActionMethodItem(this, "CreateBarManager", " Create BarManager", "Toolbar"));
		}
		protected string GetActionSuffix(bool isRibbon) {
			return isRibbon ? "Tab" : "Bars";
		}
		protected internal virtual void PopulateSortedActionItems(DesignerActionItemCollection items, bool isRibbon) {
			string suffix = GetActionSuffix(isRibbon);
			items.Add(new DesignerActionMethodItem(this, "CreateFileBar", String.Format(" Create File {0}", suffix), "Toolbar"));
			items.Add(new DesignerActionMethodItem(this, "CreateHomeBar", String.Format(" Create Home {0}", suffix), "Toolbar"));
			items.Add(new DesignerActionMethodItem(this, "CreateInsertBar", String.Format(" Create Insert {0}", suffix), "Toolbar"));
			items.Add(new DesignerActionMethodItem(this, "CreatePageLayoutBar", String.Format(" Create Page Layout {0}", suffix), "Toolbar"));
			items.Add(new DesignerActionMethodItem(this, "CreateMailMergeBar", String.Format(" Create Mail Merge {0}", suffix), "Toolbar"));
			items.Add(new DesignerActionMethodItem(this, "CreateDocumentViewsBar", String.Format(" Create View {0}", suffix), "Toolbar"));
			items.Add(new DesignerActionMethodItem(this, "CreateHeaderFooterBar", String.Format(" Create Header && Footer {0}", suffix), "Toolbar"));
			items.Add(new DesignerActionMethodItem(this, "CreateTableBar", String.Format(" Create Table {0}", suffix), "Toolbar"));
			items.Add(new DesignerActionMethodItem(this, "CreateDocumentReviewBar", String.Format(" Create Review {0}", suffix), "Toolbar"));
			items.Add(new DesignerActionMethodItem(this, "CreateDocumentReferencesBar", String.Format(" Create References {0}", suffix), "Toolbar"));
			items.Add(new DesignerActionMethodItem(this, "CreateFloatingObjectBar", String.Format(" Create Floating Object {0}", suffix), "Toolbar"));
		}
		[RefreshProperties(RefreshProperties.All)]
		public void CreateRibbon() {
			RichEditControl control = Designer.RichEditControl;
			if (control == null)
				return;
			IContainer container = Designer.DesignerHost.Container;
			if (container == null)
				return;
			Control form = DevExpress.XtraBars.Design.DesignHelpers.GetContainerControl(container);
			if (form == null)
				return;
			using (DesignerTransaction transaction = Designer.DesignerHost.CreateTransaction("Create Ribbon")) {
				RibbonControl ribbon = (RibbonControl)Designer.DesignerHost.CreateComponent(typeof(RibbonControl));
				Designer.ChangeService.OnComponentChanging(form, null);
				form.Controls.Add(ribbon);
				RibbonForm ribbonForm = form as RibbonForm;
				if (ribbonForm != null)
					ribbonForm.Ribbon = ribbon;
				Designer.ChangeService.OnComponentChanging(form, null);
				Designer.ChangeService.OnComponentChanging(control, null);
				control.MenuManager = ribbon;
				Designer.ChangeService.OnComponentChanged(control, null, null, null);
				transaction.Commit();
				EditorContextHelperEx.RefreshSmartPanel(Component);
			}
		}
		[RefreshProperties(RefreshProperties.All)]
		public void CreateBarManager() {
			RichEditControl control = Designer.RichEditControl;
			IContainer container = Designer.DesignerHost.Container;
			if (container == null)
				return;
			Control form = DevExpress.XtraBars.Design.DesignHelpers.GetContainerControl(container);
			if (form == null)
				return;
			using (DesignerTransaction transaction = Designer.DesignerHost.CreateTransaction("Create BarManager")) {
				Designer.ChangeService.OnComponentChanging(form, null);
				Designer.ChangeService.OnComponentChanged(form, null, null, null);
				BarManager barManager = (BarManager)Designer.DesignerHost.CreateComponent(typeof(BarManager));
				Designer.ChangeService.OnComponentChanging(container, null);
				container.Add(barManager);
				Designer.ChangeService.OnComponentChanged(container, null, null, null);
				Designer.ChangeService.OnComponentChanging(barManager, null);
				barManager.Form = form;
				Designer.ChangeService.OnComponentChanged(barManager, null, null, null);
				Designer.ChangeService.OnComponentChanging(control, null);
				control.MenuManager = barManager;
				Designer.ChangeService.OnComponentChanged(control, null, null, null);
				transaction.Commit();
				EditorContextHelperEx.RefreshSmartPanel(Component);
			}
		}
		#region CreateFileBar
		[RefreshProperties(RefreshProperties.All)]
		public void CreateFileBar() {
			AddNewBars(GetFileBarCreators(), "File", BarInsertMode.Add);
		}
		protected virtual ControlCommandBarCreator[] GetFileBarCreators() {
			ControlCommandBarCreator creator = CreateRichEditFileBarCreatorInstance();
			return new ControlCommandBarCreator[] { creator };
		}
		protected virtual ControlCommandBarCreator CreateRichEditFileBarCreatorInstance() {
			return new RichEditFileBarCreator();
		}
		#endregion
		#region CreateHomeBar
		[RefreshProperties(RefreshProperties.All)]
		public void CreateHomeBar() {
			AddNewBars(GetHomeBarCreators(), "Home", BarInsertMode.Add);
		}
		protected ControlCommandBarCreator[] GetHomeBarCreators() {
			ControlCommandBarCreator clipboard = new RichEditClipboardBarCreator();
			ControlCommandBarCreator font = new RichEditFontBarCreator();
			ControlCommandBarCreator paragraph = new RichEditParagraphBarCreator();
			ControlCommandBarCreator styles = new RichEditStylesBarCreator();
			ControlCommandBarCreator editing = new RichEditEditingBarCreator();
			return new ControlCommandBarCreator[] { clipboard, font, paragraph, styles, editing };
		}
		#endregion
		#region CreateInsertBar
		[RefreshProperties(RefreshProperties.All)]
		public void CreateInsertBar() {
			AddNewBars(GetInsertBarCreators(), "Insert", BarInsertMode.Add);
		}
		protected ControlCommandBarCreator[] GetInsertBarCreators() {
			return GetInsertBarCreatorsCore().ToArray();
		}
		protected virtual List<ControlCommandBarCreator> GetInsertBarCreatorsCore() {
			List<ControlCommandBarCreator> result = new List<ControlCommandBarCreator>();
			result.Add(new RichEditPagesBarCreator());
			result.Add(new RichEditTablesBarCreator());
			result.Add(new RichEditIllustrationsBarCreator());
			result.Add(new RichEditLinksBarCreator());
			result.Add(new RichEditHeaderFooterBarCreator());
			result.Add(new RichEditTextBarCreator());
			result.Add(new RichEditSymbolsBarCreator());
			return result;
		}
		#endregion
		#region CreateHeaderFooterBar
		[RefreshProperties(RefreshProperties.All)]
		public void CreateHeaderFooterBar() {
			AddNewBars(GetHeaderFooterBarCreators(), "Headers & Footers", BarInsertMode.Add);
		}
		protected ControlCommandBarCreator[] GetHeaderFooterBarCreators() {
			ControlCommandBarCreator headersNavigation = new RichEditHeaderFooterToolsDesignNavigationBarCreator();
			ControlCommandBarCreator headersOptions = new RichEditHeaderFooterToolsDesignOptionsBarCreator();
			ControlCommandBarCreator headersClose = new RichEditHeaderFooterToolsDesignCloseBarCreator();
			return new ControlCommandBarCreator[] { headersNavigation, headersOptions, headersClose };
		}
		#endregion
		#region CreateTableBar
		[RefreshProperties(RefreshProperties.All)]
		public void CreateTableBar() {
			AddNewBars(GetTableBarCreators(), "Tables", BarInsertMode.Add);
		}
		protected ControlCommandBarCreator[] GetTableBarCreators() {
			ControlCommandBarCreator options = new RichEditTableStyleOptionsBarCreator();
			ControlCommandBarCreator styles = new RichEditTableStylesBarCreator();
			ControlCommandBarCreator cellStyles = CreateTableCellStylesBarCreatorInstance();
			ControlCommandBarCreator borders = new RichEditTableDrawBordersBarCreator();
			ControlCommandBarCreator table = new RichEditTableTableBarCreator();
			ControlCommandBarCreator rowsAndColumns = new RichEditTableRowsAndColumnsBarCreator();
			ControlCommandBarCreator merge = new RichEditTableMergeBarCreator();
			ControlCommandBarCreator cellSize = new RichEditTableCellSizeBarCreator();
			ControlCommandBarCreator alignment = new RichEditTableAlignmentBarCreator();
			if (cellStyles != null)
				return new ControlCommandBarCreator[] { options, styles, cellStyles, borders, table, rowsAndColumns, merge, cellSize, alignment };
			else
			return new ControlCommandBarCreator[] { options, styles, borders, table, rowsAndColumns, merge, cellSize, alignment };
		}
		protected virtual ControlCommandBarCreator CreateTableCellStylesBarCreatorInstance() {
			return null;
		}
		#endregion
		#region CreatePageLayoutBar
		[RefreshProperties(RefreshProperties.All)]
		public void CreatePageLayoutBar() {
			AddNewBars(GetPageLayoutBarCreators(), "Page Layout", BarInsertMode.Add);
		}
		protected ControlCommandBarCreator[] GetPageLayoutBarCreators() {
			ControlCommandBarCreator pageSetup = new RichEditPageSetupBarCreator();
			ControlCommandBarCreator pageBackground = new RichEditPageBackgroundBarCreator();
			return new ControlCommandBarCreator[] { pageSetup, pageBackground };
		}
		#endregion
		#region CreateMailMergeBar
		[RefreshProperties(RefreshProperties.All)]
		public void CreateMailMergeBar() {
			AddNewBars(GetMailMergeBarCreators(), "Mail Merge", BarInsertMode.Add);
		}
		protected ControlCommandBarCreator[] GetMailMergeBarCreators() {
			ControlCommandBarCreator creator = CreateRichEditMailMergeBarCreatorInstance();
			return new ControlCommandBarCreator[] { creator };
		}
		protected virtual ControlCommandBarCreator CreateRichEditMailMergeBarCreatorInstance() {
			return new RichEditMailMergeBarCreator();
		}
		#endregion
		#region CreateDocumentViewsBar
		[RefreshProperties(RefreshProperties.All)]
		public void CreateDocumentViewsBar() {
			AddNewBars(GetDocumentViewsBarCreators(), "Document Views", BarInsertMode.Add);
		}
		protected ControlCommandBarCreator[] GetDocumentViewsBarCreators() {
			return GetDocumentViewsBarCreatorsCore().ToArray();
		}
		protected virtual List<ControlCommandBarCreator> GetDocumentViewsBarCreatorsCore() {
			List<ControlCommandBarCreator> result = new List<ControlCommandBarCreator>();
			result.Add(new RichEditDocumentViewsBarCreator());
			result.Add(new RichEditShowBarCreator());
			result.Add(new RichEditZoomBarCreator());
			return result;
		}
		#endregion
		#region CreateDocumentReviewBar
		[RefreshProperties(RefreshProperties.All)]
		public void CreateDocumentReviewBar() {
			AddNewBars(GetDocumentReviewBarCreators(), "Review", BarInsertMode.Add);
		}
		protected virtual ControlCommandBarCreator[] GetDocumentReviewBarCreators() {
			ControlCommandBarCreator proofing = new RichEditDocumentProofingBarCreator();
			ControlCommandBarCreator language = new RichEditDocumentLanguageBarCreator();
			ControlCommandBarCreator protection = new RichEditDocumentProtectionBarCreator();
			ControlCommandBarCreator comment = new RichEditDocumentCommentBarCreator();
			ControlCommandBarCreator tracking = new RichEditDocumentTrackingBarCreator();
			return new ControlCommandBarCreator[] { proofing, language, protection, comment, tracking};
		}
		#endregion
		#region CreateDocumentReferencesBar
		[RefreshProperties(RefreshProperties.All)]
		public void CreateDocumentReferencesBar() {
			AddNewBars(GetDocumentReferencesBarCreators(), "References", BarInsertMode.Add);
		}
		protected ControlCommandBarCreator[] GetDocumentReferencesBarCreators() {
			ControlCommandBarCreator tableOfContents = new RichEditTableOfContentsBarCreator();
			ControlCommandBarCreator captions = new RichEditCaptionsBarCreator();
			return new ControlCommandBarCreator[] { tableOfContents, captions };
		}
		#endregion
		#region CreateFloatingObjectBar
		[RefreshProperties(RefreshProperties.All)]
		public void CreateFloatingObjectBar() {
			AddNewBars(GetFloatingObjectBarCreators(), "Picture Tools", BarInsertMode.Add);
		}
		protected ControlCommandBarCreator[] GetFloatingObjectBarCreators() {
			ControlCommandBarCreator floatingPictureShapeStyles = new RichEditFloatingPictureShapeStylesBarCreator();
			ControlCommandBarCreator floatingPictureArrange = new RichEditFloatingPictureArrangeBarCreator();
			return new ControlCommandBarCreator[] { floatingPictureShapeStyles, floatingPictureArrange };
		}
		#endregion
		#region CreateStatusBar
		[RefreshProperties(RefreshProperties.All)]
		public void CreateStatusBar() {
			ControlCommandBarCreator views = new RichEditDocumentViewsBarCreator();
			ControlCommandBarCreator[] creators = new ControlCommandBarCreator[] { views };
			RichEditDesignTimeStatusBarGenerator generator = new RichEditDesignTimeStatusBarGenerator(this.Designer.DesignerHost, Designer.Control);
			generator.AddNewBars(creators, "Status Bar", BarInsertMode.Add);
		}
		#endregion
		protected void AddNewBars(ControlCommandBarCreator[] creators, string barName, BarInsertMode insertMode) {
			RichEditDesignTimeBarsGenerator generator = CreateGenerator();
			if (generator == null)
				return;
			generator.AddNewBars(creators, barName, insertMode);
		}
		public void ClearExistingItems() {
			ControlCommandBarCreator fakeBarCreator = new RichEditDocumentViewsBarCreator();
			RichEditDesignTimeBarsGenerator generator = CreateGenerator();
			if (generator == null)
				return;
			generator.ClearExistingItems(fakeBarCreator);
		}
		protected virtual RichEditDesignTimeBarsGenerator CreateGenerator() {
			if (Designer.Control == null)
				return null;
			return new RichEditDesignTimeBarsGenerator(this.Designer.DesignerHost, Designer.Control );
		}
	}
	#endregion
	#region RichEditAllBarsActionList
	public class RichEditAllBarsActionList : RichEditBarsActionList {
		public RichEditAllBarsActionList(XtraRichEditDesigner designer)
			: base(designer) {
		}
		protected internal override void PopulateCreateBarsItems(DesignerActionItemCollection items) {
		}
		protected internal override void PopulateSortedActionItems(DesignerActionItemCollection items, bool isRibbon) {
			string name = String.Format(" Create All {0}", isRibbon ? "Tabs" : "Bars");
			items.Add(new DesignerActionMethodItem(this, "CreateAllBars", name, "Toolbar"));
		}
		[RefreshProperties(RefreshProperties.All)]
		public void CreateAllBars() {
			CreateAllBarsCore(false, BarInsertMode.Add);
		}
		public void CreateAllBarsCore(bool clearExistingItemsBefore, BarInsertMode insertMode) {
			using (DesignerTransaction transaction = Designer.DesignerHost.CreateTransaction("Create RichEdit Bars")) {
				if (clearExistingItemsBefore)
					ClearExistingItems();
				List<ControlCommandBarCreator[]> creators = new List<ControlCommandBarCreator[]>();
				creators.Add(GetFileBarCreators());
				creators.Add(GetHomeBarCreators());
				creators.Add(GetInsertBarCreators());
				creators.Add(GetPageLayoutBarCreators());
				creators.Add(GetDocumentReferencesBarCreators());
				creators.Add(GetMailMergeBarCreators());
				creators.Add(GetDocumentReviewBarCreators());
				creators.Add(GetDocumentViewsBarCreators());
				creators.Add(GetHeaderFooterBarCreators());
				creators.Add(GetTableBarCreators());
				creators.Add(GetFloatingObjectBarCreators());
				int first, last, step;
				if (insertMode == BarInsertMode.Add) {
					first = 0;
					last = creators.Count;
					step = 1;
				}
				else {
					first = creators.Count - 1;
					last = -1;
					step = -1;
				}
				for (int i = first; i != last; i += step)
					AddNewBars(creators[i], "Create RichEdit Bars", insertMode);
				transaction.Commit();
			}
		}
	}
	#endregion
	#region RichEditCommentsActionList
	public class RichEditCommentsActionList : DesignerActionList {
		readonly XtraRichEditDesigner designer;
		RichEditCommentControl commentControl;
		public RichEditCommentsActionList(XtraRichEditDesigner designer)
			: base(designer.Component) {
			this.designer = designer;
		}
		public XtraRichEditDesigner Designer { get { return designer; } }
		RichEditCommentControl CommentControl { get { return commentControl; } set { commentControl = value; } }
		public override DesignerActionItemCollection GetSortedActionItems() {
			DesignerActionItemCollection result = new DesignerActionItemCollection();
			if (this.CommentControl == null) {
				PopulateCreateCommentsItems(result);
				return result;
			}
			return result;
		}
		protected internal virtual void PopulateCreateCommentsItems(DesignerActionItemCollection items) {
			RichEditControl control = Designer.RichEditControl;
			if (control == null)
				return;
			if (!EnsureReferences(control))
			items.Add(new DesignerActionMethodItem(this, "CreateCommentsDockPanel", "Create Reviewing Pane", "Toolbar"));
		}
		bool EnsureReferences(RichEditControl control) { 
			List<RichEditCommentControl> listCommentsControl = ComponentFinder.FindComponentsOfType<RichEditCommentControl>(Component.Site);
			if (listCommentsControl.Count > 0)
				return (listCommentsControl[0].RichEditControl == control);
			else
				return false;
		}
		[RefreshProperties(System.ComponentModel.RefreshProperties.All)]
		public void CreateCommentsDockPanel() {
			RichEditControl control = Designer.RichEditControl;
			if (control == null)
				return;
			IContainer container = Designer.DesignerHost.Container;
			if (container == null)
				return;
			Control form = DevExpress.XtraBars.Design.DesignHelpers.GetContainerControl(container);
			if (form == null)
				return;
			using (DesignerTransaction transaction = Designer.DesignerHost.CreateTransaction("Create Reviewing Pane")) {
				DockManager manager = CreateDockManager(form);
				Designer.ChangeService.OnComponentChanging(manager, null);
				manager.Form = form as ContainerControl;
				Designer.ChangeService.OnComponentChanged(manager, null, null, null);
				DockPanel panel = manager.AddPanel(DockingStyle.Left);
				panel.Text = XtraRichEditLocalizer.GetString(XtraRichEditStringId.Caption_MainDocumentComments); 
				panel.Width = Math.Min(400, form.Width / 2);
				DevExpress.XtraBars.Docking.ControlContainer containerControl = panel.ControlContainer;
				RichEditCommentControl commentControl = (RichEditCommentControl)Designer.DesignerHost.CreateComponent(typeof(RichEditCommentControl));
				Designer.ChangeService.OnComponentChanging(containerControl, null);
				containerControl.Controls.Add(commentControl);
				commentControl.Dock = DockStyle.Fill;
				commentControl.RichEditControl = control;
				commentControl.ActiveViewType = DevExpress.XtraRichEdit.RichEditViewType.Simple;
				Designer.ChangeService.OnComponentChanged(containerControl, null, null, null);
				this.CommentControl = commentControl;
				this.CommentControl.ReadOnly = false;
				transaction.Commit();
				EditorContextHelperEx.RefreshSmartPanel(Component);
			}
		}
		DockManager CreateDockManager(Control form) { 
			List<DockManager> listManager = ComponentFinder.FindComponentsOfType<DockManager>(Component.Site);
			if (listManager.Count > 0)
				return listManager[0];			
			return (DockManager)Designer.DesignerHost.CreateComponent(typeof(DockManager)); ;
		}
	}
	#endregion
	#region RichEditDesignTimeBarsGenerator
	public class RichEditDesignTimeBarsGenerator : DesignTimeBarsGenerator<RichEditControl, RichEditCommandId> {
		public RichEditDesignTimeBarsGenerator(IDesignerHost host, IComponent component)
			: base(host, component) {
		}
		protected override BarGenerationManagerFactory<RichEditControl, RichEditCommandId> CreateBarGenerationManagerFactory() {
			return new RichEditBarGenerationManagerFactory();
		}
		protected override ControlCommandBarControllerBase<RichEditControl, RichEditCommandId> CreateBarController() {
			RichEditControl control = Component as RichEditControl;
			if (control != null)
				control.Options.Printing.PrintPreviewFormKind = this.HasRibbonControl() ? PrintPreviewFormKind.Ribbon 
					: PrintPreviewFormKind.Bars;
			return new RichEditBarController();
		}
		protected override void EnsureReferences(IDesignerHost designerHost) {
			ReferencesHelper.EnsureReferences(designerHost, AssemblyInfo.SRAssemblyRichEditExtensions);
		}
	}
	#endregion
	#region RichEditDesignTimeStatusBarGenerator
	public class RichEditDesignTimeStatusBarGenerator : DesignTimeBarsGenerator<RichEditControl, RichEditCommandId> {
		public RichEditDesignTimeStatusBarGenerator(IDesignerHost host, IComponent component)
			: base(host, component) {
		}
		protected override BarGenerationManagerFactory<RichEditControl, RichEditCommandId> CreateBarGenerationManagerFactory() {
			return new RichEditStatusBarGenerationManagerFactory();
		}
		protected override ControlCommandBarControllerBase<RichEditControl, RichEditCommandId> CreateBarController() {
			return new RichEditBarController();
		}
		protected override void EnsureReferences(IDesignerHost designerHost) {
			ReferencesHelper.EnsureReferences(designerHost, AssemblyInfo.SRAssemblyRichEditExtensions);
		}
	}
	#endregion
	#region RichEditBarGenerationManager
	public class RichEditBarGenerationManager : BarGenerationManager<RichEditControl, RichEditCommandId> {
		public RichEditBarGenerationManager(ControlCommandBarCreator creator, Component container, ControlCommandBarControllerBase<RichEditControl, RichEditCommandId> barController)
			: base(creator, container, barController) {
		}
	}
	#endregion
	#region RichEditRibbonGenerationManager
	public class RichEditRibbonGenerationManager : RibbonGenerationManager<RichEditControl, RichEditCommandId> {
		static readonly List<Type> requiringNewGroupItems = CreateRequiringNewGroupItemsList();
		static List<Type> CreateRequiringNewGroupItemsList() {
			List<Type> result = new List<Type>();
			result.Add(typeof(ShowPageMarginsSetupFormItem));
			result.Add(typeof(ShowLineNumberingFormItem));
			result.Add(typeof(ShowColumnsSetupFormItem));
			result.Add(typeof(ShowPagePaperSetupFormItem));
			result.Add(typeof(ResetTableCellsAllBordersItem));
			result.Add(typeof(ToggleTableCellsInsideHorizontalBorderItem));
			result.Add(typeof(ToggleShowTableGridLinesItem));
			return result;
		}
		public RichEditRibbonGenerationManager(ControlCommandBarCreator creator, Component container, ControlCommandBarControllerBase<RichEditControl, RichEditCommandId> barController)
			: base(creator, container, barController) {
		}
		protected override RichEditCommandId EmptyCommandId { get { return RichEditCommandId.None; } }
		protected List<Type> RequiringNewGroupItems { get { return requiringNewGroupItems; } }
		protected override void AddBarItemsCore(List<BarItem> items, List<BarItem> containerBarItems) {
			base.AddBarItemsCore(items, containerBarItems);
			foreach (BarItem item in items) {
				BarSubItem subItem = item as BarSubItem;
				if (subItem != null)
					foreach (LinkPersistInfo info in subItem.LinksPersistInfo)
						if (RequiringNewGroupItems.Contains(info.Item.GetType()))
							info.BeginGroup = true;
			}
		}
	}
	#endregion
	#region RichEditBarGenerationManagerFactory
	public class RichEditBarGenerationManagerFactory : BarGenerationManagerFactory<RichEditControl, RichEditCommandId> {
		protected override RibbonGenerationManager<RichEditControl, RichEditCommandId> CreateRibbonGenerationManagerInstance(ControlCommandBarCreator creator, Component container, ControlCommandBarControllerBase<RichEditControl, RichEditCommandId> barController) {
			return new RichEditRibbonGenerationManager(creator, container, barController);
		}
		protected override BarGenerationManager<RichEditControl, RichEditCommandId> CreateBarGenerationManagerInstance(ControlCommandBarCreator creator, Component container, ControlCommandBarControllerBase<RichEditControl, RichEditCommandId> barController) {
			return new RichEditBarGenerationManager(creator, container, barController);
		}
	}
	#endregion
	#region RichEditStatusBarGenerationManager
	public class RichEditStatusBarGenerationManager : StatusBarGenerationManager<RichEditControl, RichEditCommandId> {
		public RichEditStatusBarGenerationManager(ControlCommandBarCreator creator, Component container, ControlCommandBarControllerBase<RichEditControl, RichEditCommandId> barController)
			: base(creator, container, barController) {
		}
	}
	#endregion
	#region RichEditRibbonStatusBarGenerationManager
	public class RichEditRibbonStatusBarGenerationManager : RibbonStatusBarGenerationManager<RichEditControl, RichEditCommandId> {
		public RichEditRibbonStatusBarGenerationManager(ControlCommandBarCreator creator, Component container, ControlCommandBarControllerBase<RichEditControl, RichEditCommandId> barController)
			: base(creator, container, barController) {
		}
		protected override RichEditCommandId EmptyCommandId { get { return RichEditCommandId.None; } }
	}
	#endregion
	#region RichEditStatusBarGenerationManagerFactory
	public class RichEditStatusBarGenerationManagerFactory : BarGenerationManagerFactory<RichEditControl, RichEditCommandId> {
		protected override RibbonGenerationManager<RichEditControl, RichEditCommandId> CreateRibbonGenerationManagerInstance(ControlCommandBarCreator creator, Component container, ControlCommandBarControllerBase<RichEditControl, RichEditCommandId> barController) {
			return new RichEditRibbonStatusBarGenerationManager(creator, container, barController);
		}
		protected override BarGenerationManager<RichEditControl, RichEditCommandId> CreateBarGenerationManagerInstance(ControlCommandBarCreator creator, Component container, ControlCommandBarControllerBase<RichEditControl, RichEditCommandId> barController) {
			return new RichEditStatusBarGenerationManager(creator, container, barController);
		}
	}
	#endregion
}
