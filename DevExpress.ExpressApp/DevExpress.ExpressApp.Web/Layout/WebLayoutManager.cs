#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       eXpressApp Framework                                        }
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
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Layout;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Web.Editors.ASPx;
using DevExpress.ExpressApp.Web.SystemModule;
using DevExpress.ExpressApp.Web.Templates;
using DevExpress.ExpressApp.Web.TestScripts;
namespace DevExpress.ExpressApp.Web.Layout {
	public interface ILinkedToControl {
		void BreakLinksToControl();
	}
	public class WebLayoutItemAppearanceAdapter : IAppearanceFormat, IAppearanceBase, IAppearanceVisibility {
		public const string AppearanceControlVisibilityKey = "AppearanceControlVisibility"; 
		private ViewItemVisibility defaultVisibility;
		internal Color? delayedBackColor = null;
		internal Color? delayedFontColor = null;
		internal FontStyle? delayedFontStyle = null;
		private void item_Load(object sender, EventArgs e) {
			((LayoutItemTemplateContainerBase)sender).Load -= new EventHandler(item_Load);
			if(delayedBackColor.HasValue) {
				BackColor = delayedBackColor.Value;
			}
			if(delayedFontColor.HasValue) {
				FontColor = delayedFontColor.Value;
			}
			if(delayedFontStyle.HasValue) {
				FontStyle = delayedFontStyle.Value;
			}
		}
		private void item_Unload(object sender, EventArgs e) {
			Item.Unload -= new EventHandler(item_Unload);
			Item = null;
		}
		public WebLayoutItemAppearanceAdapter(LayoutItemTemplateContainerBase item) {
			Guard.ArgumentNotNull(item, "item");
			this.Item = item;
			Item.Load += new EventHandler(item_Load);
			Item.Unload += new EventHandler(item_Unload);
			defaultVisibility = Visibility;
		}
		public LayoutItemTemplateContainerBase Item { get; private set; }
		#region IAppearanceFormat Members
		public Color BackColor {
			get {
				if(Item != null && Item.CaptionControl != null) {
					return Item.CaptionControl.BackColor;
				}
				return Color.Empty;
			}
			set {
				if(Item != null && Item.CaptionControl != null) {
					Item.CaptionControl.BackColor = value;
					delayedBackColor = null;
				}
				else {
					delayedBackColor = value;
				}
			}
		}
		public Color FontColor {
			get {
				if(Item != null && Item.CaptionControl != null) {
					return Item.CaptionControl.ForeColor;
				}
				return Color.Empty;
			}
			set {
				if(Item != null && Item.CaptionControl != null) {
					Item.CaptionControl.ForeColor = value;
					delayedFontColor = null;
				}
				else {
					delayedFontColor = value;
				}
			}
		}
		public FontStyle FontStyle {
			get {
				FontStyle result = FontStyle.Regular;
				if(Item != null && Item.CaptionControl != null) {
					if(Item.CaptionControl.Font.Bold) {
						result = result & (~FontStyle.Regular);
						result = result | FontStyle.Bold;
					}
					if(Item.CaptionControl.Font.Italic) {
						result = result & (~FontStyle.Regular);
						result = result | FontStyle.Italic;
					}
					if(Item.CaptionControl.Font.Underline) {
						result = result & (~FontStyle.Regular);
						result = result | FontStyle.Underline;
					}
					if(Item.CaptionControl.Font.Strikeout) {
						result = result & (~FontStyle.Regular);
						result = result | FontStyle.Strikeout;
					}
				}
				return result;
			}
			set {
				if(Item != null && Item.CaptionControl != null) {
					RenderHelper.SetFontStyle(Item.CaptionControl, value);
					delayedFontStyle = null;
				}
				else {
					delayedFontStyle = value;
				}
			}
		}
		public void ResetFontStyle() {
			if(Item != null && Item.CaptionControl != null) {
				FontStyle = FontStyle.Regular;
			}
		}
		public void ResetFontColor() {
			if(Item != null && Item.CaptionControl != null) {
				FontColor = new Color();
			}
		}
		public void ResetBackColor() {
			if(Item != null && Item.CaptionControl != null) {
				BackColor = new Color();
			}
		}
		#endregion
		#region IAppearanceVisibility Members
		public ViewItemVisibility Visibility {
			get {
				if(Item != null) {
					return Item.Visible ? ViewItemVisibility.Show : ViewItemVisibility.Hide;
				}
				return ViewItemVisibility.Hide;
			}
			set {
				if(Item != null) {
					bool itemVisible = value == ViewItemVisibility.Show;
					Item.Visibility.SetItemValue(AppearanceControlVisibilityKey, itemVisible);
					IAppearanceVisibility viewItem = Item.ViewItem as IAppearanceVisibility;
					if(viewItem != null && viewItem.Visibility != value) {
						viewItem.Visibility = value;
					}
				}
			}
		}
		public void ResetVisibility() {
			Visibility = defaultVisibility;
		}
		#endregion
	}
	[ToolboxItem(false)]
	public class CustomPanel : Panel, INamingContainer { }
	public class WebLayoutManager : LayoutManager, ISupportViewEditMode {
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DefaultValue(true)]
		public static bool UpdateVisibilityForDelayedControlInstantiationStrategy { get; set; }
		private bool isSimpleLayout;
		private ViewEditMode viewEditMode;
		private CustomPanel container;
		private Dictionary<string, LayoutItemTemplateContainerBase> items;
		private LayoutBaseTemplate tabbedGroupTemplate;
		private LayoutBaseTemplate layoutGroupTemplate;
		private LayoutBaseTemplate layoutItemTemplate;
		private Unit layoutItemCaptionWidth;
		private DevExpress.Utils.Locations defaultLayoutItemCaptionLocation;
		private DevExpress.Utils.HorzAlignment defaultLayoutItemCaptionHorizontalAlignment;
		private DevExpress.Utils.VertAlignment defaultLayoutItemCaptionVerticalAlignment;
		private static ViewItem FindViewItem(ViewItemsCollection viewItems, IModelLayoutViewItem layoutItemModel) {
			string viewItemId = layoutItemModel.ViewItem != null ? layoutItemModel.ViewItem.Id : layoutItemModel.Id;
			return viewItems[viewItemId];
		}
		private void tabbedGroupTemplate_PageControlCreated(object sender, PageControlCreatedEventArgs e) {
			OnPageControlCreated(e);
		}
		private void layoutTemplate_Instantiated(object sender, TemplateInstantiatedEventArgs e) {
			UpdateTemplateContainerVisibility(e.Container, false);
		}
		private void callbackManager_PreRenderInternal(object sender, EventArgs e) {
			((XafCallbackManager)sender).PreRenderInternal -= new EventHandler<EventArgs>(callbackManager_PreRenderInternal);
			UpdateItemsVisibility();
		}
		private void container_PreRender(object sender, EventArgs e) {
			UpdateItemsVisibility();
		}
		private void UpdateItemsVisibility() {
			foreach(LayoutItemTemplateContainerBase templateContainer in Items.Values) {
				UpdateTemplateContainerVisibility(templateContainer, true);
			}
		}
		private void UpdateTemplateContainerVisibility(LayoutItemTemplateContainerBase templateContainer, bool forceUpdateParent) {
			if(templateContainer.ViewItem is IAppearanceVisibility) {
				bool itemVsibile = ((IAppearanceVisibility)templateContainer.ViewItem).Visibility == ViewItemVisibility.Show;
				bool needUpdate = templateContainer.Visibility != itemVsibile;
				if(needUpdate) {
					templateContainer.Visibility.SetItemValue(LayoutItemTemplateContainerBase.ControlVisibilityKey, itemVsibile);
				}
				if(needUpdate || forceUpdateParent) { 
					UpdateParentContainerVisibility(templateContainer, itemVsibile);
				}
			}
		}
		private void UpdateParentContainerVisibility(LayoutItemTemplateContainerBase templateContainer, bool visible) {
			Control parent = templateContainer.Parent;
			if(parent == null && DelayedItemsInitialization) {
				IModelViewLayoutElement parentLayoutItemModel = templateContainer.Model.Parent as IModelViewLayoutElement;
				if(parentLayoutItemModel != null) {
					LayoutItemTemplateContainerBase parentEntry;
					Items.TryGetValue(WebIdHelper.GetLayoutItemId(parentLayoutItemModel), out parentEntry);
					parent = parentEntry;
				}
			}
			while(parent != null) {
				LayoutGroupTemplateContainerBase container = parent as LayoutGroupTemplateContainerBase;
				if(container != null && IsTheSameVisibility(container.Items.Values, visible)) {
					container.Visibility.SetItemValue(LayoutItemTemplateContainerBase.ControlVisibilityKey, visible);
				}
				parent = parent.Parent;
			}
		}
		private bool IsTheSameVisibility(IEnumerable<LayoutItemTemplateContainerBase> items, bool visible) {
			if(!visible) {
				foreach(LayoutItemTemplateContainerBase item in items) {
					if(item.Visibility != visible) {
						return false;
					}
				}
			}
			return true;
		}
		private void EnsureTemplates() {
			if(TabbedGroupTemplate == null) {
				TabbedGroupTemplate = CreateTabbedGroupTemplate();
			}
			if(LayoutGroupTemplate == null) {
				LayoutGroupTemplate = CreateLayoutGroupTemplate();
			}
			if(LayoutItemTemplate == null) {
				LayoutItemTemplate = CreateLayoutItemTemplate();
			}
		}
		protected virtual LayoutBaseTemplate CreateTabbedGroupTemplate() {
			TabbedGroupTemplate result;
			if(DelayedItemsInitialization) {
				result = new TabbedGroupTemplateWithDelayedTabsInstantiation(CreateControlInstantiationStrategy());
			}
			else {
				result = new TabbedGroupTemplate(CreateControlInstantiationStrategy());
			}
			result.PageControlCreated += new EventHandler<PageControlCreatedEventArgs>(tabbedGroupTemplate_PageControlCreated);
			OnLayoutTemplateCreated(result);
			return result;
		}
		protected virtual LayoutBaseTemplate CreateLayoutGroupTemplate() {
			LayoutBaseTemplate result = new LayoutGroupTemplate(CreateControlInstantiationStrategy());
			OnLayoutTemplateCreated(result);
			return result;
		}
		protected virtual LayoutBaseTemplate CreateLayoutItemTemplate() {
			LayoutBaseTemplate result = new LayoutItemTemplate(CreateControlInstantiationStrategy());
			OnLayoutTemplateCreated(result);
			return result;
		}
		protected virtual ControlInstantiationStrategyBase CreateControlInstantiationStrategy() {
			if(DelayedItemsInitialization) {
				return new DelayedControlInstantiationStrategy();
			}
			else {
				return new SimpleControlInstantiationStrategy();
			}
		}
		private void OnLayoutTemplateCreated(LayoutBaseTemplate layoutTemplate) {
			if(UpdateVisibilityForDelayedControlInstantiationStrategy) {
				layoutTemplate.Instantiated += new EventHandler<TemplateInstantiatedEventArgs>(layoutTemplate_Instantiated);
			}
		}
		protected CustomPanel CreateContainerControl() {
			CustomPanel container = new CustomPanel();
			ContainerControlCreatedEventArgs args = new ContainerControlCreatedEventArgs(container);
			OnContainerControlCreated(args);
			return container;
		}
		private void SetupContainerControl(WebControl container, IModelViewLayout layoutModel, ViewItemsCollection viewItems) {
			container.CssClass = "Layout";
			if(layoutModel == null || viewItems.Count == 0) {
				return;
			}
			if(isSimpleLayout && viewItems.Count == 1) {
				return;
			}
			if(ViewEditMode == ViewEditMode.View) {
				container.ID = "MainLayoutView";
				container.CssClass += " LayoutViewMode";
			}
			else {
				container.ID = "MainLayoutEdit";
				container.CssClass += " LayoutEditMode";
			}
			if(layoutModel.Parent is IModelDashboardView) {
				container.CssClass += " DashboardView";
			}
			LayoutCSSInfo layoutCSSInfo = LayoutCSSCalculator.GetLayoutCSSInfo(layoutModel);
			container.CssClass += " " + layoutCSSInfo.CardCssClassNameCore;
		}
		protected virtual Unit CalculateLayoutItemCaptionWidth(IModelViewLayout layoutModel, ViewItemsCollection viewItems) {
			List<string> captions = new List<string>();
			CollectLayoutItemVisibleCaptions<IModelViewLayoutElement>(captions, layoutModel, viewItems);
			return GetMaxStringWidth(captions);
		}
		protected void CollectLayoutItemVisibleCaptions<T>(IList<string> captions, IEnumerable<T> layoutInfo, ViewItemsCollection viewItems) {
			foreach(T itemInfo in layoutInfo) {
				if(itemInfo is IModelLayoutViewItem) {
					IModelLayoutViewItem layoutItemModel = (IModelLayoutViewItem)itemInfo;
					ViewItem viewItem = FindViewItem(viewItems, layoutItemModel);
					if(viewItem != null && GetIsLayoutItemCaptionVisible(layoutItemModel, viewItem) && GetIsItemForCaptionCalculation(layoutItemModel, viewItem)) {
						MarkRequiredFieldCaptionEventArgs args = new MarkRequiredFieldCaptionEventArgs(viewItem, false);
						OnMarkRequiredFieldCaption(args);
						captions.Add(BuildItemCaption(viewItem.Caption, args.NeedMarkRequiredField, args.RequiredFieldMark));
					}
				}
				else if(itemInfo is IEnumerable<IModelViewLayoutElement>) {
					CollectLayoutItemVisibleCaptions<IModelViewLayoutElement>(captions, (IEnumerable<IModelViewLayoutElement>)itemInfo, viewItems);
				}
			}
		}
		protected virtual bool GetIsItemForCaptionCalculation(IModelLayoutViewItem layoutItemModel, ViewItem viewItem) {
			DevExpress.Utils.Locations captionLocation = layoutItemModel.CaptionLocation;
			return captionLocation != DevExpress.Utils.Locations.Top && captionLocation != DevExpress.Utils.Locations.Bottom;
		}
		protected virtual Unit GetMaxStringWidth(IList<string> captions) {
			Graphics graphics = Graphics.FromImage(new Bitmap(1, 1));
			Font font = new Font("Tahoma", 10);
			float maxWidth = 0;
			foreach(string caption in captions) {
				SizeF size = graphics.MeasureString(caption, font, 300);
				if(maxWidth < size.Width) {
					maxWidth = size.Width;
				}
			}
			return Unit.Pixel((int)maxWidth);
		}
		protected virtual LayoutItemTemplateContainerBase ProcessItem(ViewItemsCollection viewItems, IModelViewLayoutElement layoutItemModel) {
			LayoutItemTemplateContainerBase result = null;
			if(layoutItemModel is IModelLayoutViewItem) {
				result = LayoutItem(viewItems, (IModelLayoutViewItem)layoutItemModel);
			}
			else if(layoutItemModel is IModelLayoutGroup) {
				result = ProcessStandardGroup(viewItems, (IModelLayoutGroup)layoutItemModel);
			}
			else if(layoutItemModel is IModelTabbedGroup) {
				result = ProcessTabbedGroup(viewItems, (IModelTabbedGroup)layoutItemModel);
			}
			return result;
		}
		protected override void OnMarkRequiredFieldCaption(MarkRequiredFieldCaptionEventArgs args) {
			base.OnMarkRequiredFieldCaption(args);
			args.NeedMarkRequiredField = args.NeedMarkRequiredField && ViewEditMode == ExpressApp.Editors.ViewEditMode.Edit;
		}
		protected virtual LayoutItemTemplateContainer LayoutItem(ViewItemsCollection viewItems, IModelLayoutViewItem layoutItemModel) {
			LayoutItemTemplateContainer templateContainer = new LayoutItemTemplateContainer(this, viewItems, layoutItemModel);
			templateContainer.ID = WebIdHelper.GetCorrectedLayoutItemId(layoutItemModel);
			ViewItem viewItem = FindViewItem(viewItems, layoutItemModel);
			if(viewItem != null) {
				MarkRequiredFieldCaptionEventArgs args = new MarkRequiredFieldCaptionEventArgs(viewItem, false);
				OnMarkRequiredFieldCaption(args);
				templateContainer.Caption = BuildItemCaption(viewItem.Caption, args.NeedMarkRequiredField, args.RequiredFieldMark);
			}
			templateContainer.ShowCaption = GetIsLayoutItemCaptionVisible(layoutItemModel, viewItem);
			templateContainer.CaptionWidth = LayoutItemCaptionWidth;
			templateContainer.CaptionLocation = GetCaptionLocation(layoutItemModel);
			templateContainer.CaptionHorizontalAlignment = GetCaptionHorizontalAlignment(layoutItemModel);
			templateContainer.CaptionVerticalAlignment = GetCaptionVerticalAlignment(layoutItemModel);
			LayoutItemTemplate.InstantiateIn(templateContainer);
			OnLayoutItemCreated(templateContainer, layoutItemModel, viewItem);
			return templateContainer;
		}
		protected bool GetIsLayoutItemCaptionVisible(IModelLayoutViewItem layoutItemModel, ViewItem viewItem) {
			if(viewItem == null) {
				return false;
			}
			return layoutItemModel.ShowCaption.HasValue ? layoutItemModel.ShowCaption.Value : viewItem.IsCaptionVisible;
		}
		private DevExpress.Utils.Locations GetCaptionLocation(IModelLayoutViewItem layoutItemModel) {
			DevExpress.Utils.Locations captionLocation = layoutItemModel.CaptionLocation;
			return captionLocation == DevExpress.Utils.Locations.Default ? DefaultLayoutItemCaptionLocation : captionLocation;
		}
		private DevExpress.Utils.HorzAlignment GetCaptionHorizontalAlignment(IModelLayoutViewItem layoutItemModel) {
			DevExpress.Utils.HorzAlignment captionHorizontalAlignment = layoutItemModel.CaptionHorizontalAlignment;
			return captionHorizontalAlignment == DevExpress.Utils.HorzAlignment.Default ? DefaultLayoutItemCaptionHorizontalAlignment : captionHorizontalAlignment;
		}
		private DevExpress.Utils.VertAlignment GetCaptionVerticalAlignment(IModelLayoutViewItem layoutItemModel) {
			DevExpress.Utils.VertAlignment captionVerticalAlignment = layoutItemModel.CaptionVerticalAlignment;
			return captionVerticalAlignment == DevExpress.Utils.VertAlignment.Default ? DefaultLayoutItemCaptionVerticalAlignment : captionVerticalAlignment;
		}
		protected virtual LayoutGroupTemplateContainer ProcessStandardGroup(ViewItemsCollection viewItems, IModelLayoutGroup layoutGroupModel) {
			ImageInfo headerImageInfo = ImageLoader.Instance.GetImageInfo(layoutGroupModel.ImageName);
			LayoutGroupTemplateContainer templateContainer = new LayoutGroupTemplateContainer(this, viewItems, layoutGroupModel, headerImageInfo);
			templateContainer.Visibility.SetItemValue(LayoutItemTemplateContainerBase.CollectionsEditModeVisibilityKey, GroupNeedsToBeShown(viewItems, layoutGroupModel));
			foreach(IModelViewLayoutElement layoutItemModel in layoutGroupModel) {
				LayoutItemTemplateContainerBase innerItemTemplate = ProcessItem(viewItems, layoutItemModel);
				templateContainer.Items.Add(layoutItemModel.Id, innerItemTemplate);
			}
			LayoutGroupTemplate.InstantiateIn(templateContainer);
			OnLayoutItemCreated(templateContainer, layoutGroupModel, null);
			return templateContainer;
		}
		protected virtual bool GroupNeedsToBeShown(ViewItemsCollection viewItems, IModelLayoutGroup layoutGroupModel) {
			if(ViewEditMode == ViewEditMode.View) {
				return true;
			}
			PropertyEditor propertyEditor = null;
			if(layoutGroupModel.Count == 1 && layoutGroupModel[0] is IModelLayoutViewItem) {
				propertyEditor = FindViewItem(viewItems, (IModelLayoutViewItem)layoutGroupModel[0]) as PropertyEditor;
			}
			if(!IsCollectionPropertyEditor(propertyEditor)) {
				return true;
			}
			DetailView detailView = propertyEditor.View as DetailView;
			return ShowViewStrategy.GetCollectionsEditMode(detailView) == ViewEditMode.Edit;
		}
		private bool IsCollectionPropertyEditor(PropertyEditor propertyEditor) {
			return propertyEditor is ListPropertyEditor || (propertyEditor is ASPxDefaultPropertyEditor && propertyEditor.MemberInfo.IsList);
		}
		protected virtual TabbedGroupTemplateContainer ProcessTabbedGroup(ViewItemsCollection viewItems, IModelTabbedGroup layoutGroupModel) {
			TabbedGroupTemplateContainer templateContainer = new TabbedGroupTemplateContainer(this, viewItems, layoutGroupModel);
			foreach(IModelLayoutGroup layoutItemModel in layoutGroupModel) {
				LayoutItemTemplateContainerBase innerItemTemplate = ProcessItem(viewItems, layoutItemModel);
				templateContainer.Items.Add(layoutItemModel.Id, innerItemTemplate);
			}
			TabbedGroupTemplate.InstantiateIn(templateContainer);
			OnLayoutItemCreated(templateContainer, layoutGroupModel, null);
			return templateContainer;
		}
		protected override object GetContainerCore() {
			return container;
		}
		protected void BreakLinksToTemplateControls(object target) {
			ILinkedToControl linkedToControl = target as ILinkedToControl;
			if(linkedToControl != null) {
				linkedToControl.BreakLinksToControl();
			}
		}
		protected void DisposeTemplate(object template) {
			IDisposable disposable = template as IDisposable;
			if(disposable != null) {
				disposable.Dispose();
			}
		}
		private void OnPageControlCreated(PageControlCreatedEventArgs args) {
			if(PageControlCreated != null) {
				PageControlCreated(this, args);
			}
		}
		protected virtual void OnContainerControlCreated(ContainerControlCreatedEventArgs args) {
			if(ContainerControlCreated != null) {
				ContainerControlCreated(this, args);
			}
		}
		private void OnLayoutItemCreated(LayoutItemTemplateContainerBase templateContainer, IModelViewLayoutElement layoutItemModel, ViewItem viewItem) {
			OnLayoutItemCreated(new ItemCreatedEventArgs(layoutItemModel, viewItem, templateContainer));
			OnCustomizeAppearance(new CustomizeAppearanceEventArgs(layoutItemModel.Id, new WebLayoutItemAppearanceAdapter(templateContainer), null));
		}
		protected virtual void OnLayoutItemCreated(ItemCreatedEventArgs args) {
			if(ItemCreated != null) {
				ItemCreated(this, args);
			}
		}
		internal List<ITestable> GetTestableControls() {
			List<ITestable> result = new List<ITestable>(); ;
			CollectTestableControls(LayoutItemTemplate, result);
			CollectTestableControls(LayoutGroupTemplate, result);
			CollectTestableControls(TabbedGroupTemplate, result);
			return result;
		}
		private void CollectTestableControls(object template, List<ITestable> additionalTestControls) {
			ITestableContainer container = template as ITestableContainer;
			if(container != null) {
				additionalTestControls.AddRange(container.GetTestableControls());
			}
		}
		static WebLayoutManager() {
			UpdateVisibilityForDelayedControlInstantiationStrategy = true;
		}
		public WebLayoutManager() : this(false, false) { }
		public WebLayoutManager(Boolean simple, Boolean delayedItemsInitialization) : this(simple, delayedItemsInitialization, false) { }
		public WebLayoutManager(Boolean simple, Boolean delayedItemsInitialization, bool newStyle) {
			this.DelayedItemsInitialization = delayedItemsInitialization;
			this.isSimpleLayout = simple;
			this.items = new Dictionary<string, LayoutItemTemplateContainerBase>();
			ViewEditMode = ViewEditMode.View;
			DefaultLayoutItemCaptionLocation = newStyle ? DevExpress.Utils.Locations.Top : DevExpress.Utils.Locations.Left;
			DefaultLayoutItemCaptionHorizontalAlignment = DevExpress.Utils.HorzAlignment.Default;
			DefaultLayoutItemCaptionVerticalAlignment = DevExpress.Utils.VertAlignment.Default;
		}
		public override object LayoutControls(IModelNode layoutNode, ViewItemsCollection viewItems) {
			EnsureTemplates();
			if(layoutNode is IModelSplitLayout) {
				Panel itemPanel = new Panel();
				itemPanel.CssClass = "Item ListViewItem";
				itemPanel.Controls.Add((Control)viewItems[0].Control);
				return itemPanel;
			}
			else {
				IModelViewLayout layoutModel = layoutNode as IModelViewLayout;
				if(layoutModel != null) {
					InitializeLayoutOptions(layoutModel);
				}
				Items.Clear();
				if(!isSimpleLayout) {
					base.LayoutControls(layoutNode, viewItems);
				}
				if(WebWindow.CurrentRequestPage != null) {
					Guard.TypeArgumentIs(typeof(ICallbackManagerHolder), WebWindow.CurrentRequestPage.GetType(), "Page");
					((ICallbackManagerHolder)WebWindow.CurrentRequestPage).CallbackManager.PreRenderInternal += new EventHandler<EventArgs>(callbackManager_PreRenderInternal);
				}
				container = CreateContainerControl();
				container.PreRender += new EventHandler(container_PreRender);
				SetupContainerControl(container, layoutModel, viewItems);
				if(layoutModel != null && viewItems.Count > 0) {
					if(isSimpleLayout && viewItems.Count == 1) {
						container.Controls.Add((Control)viewItems[0].Control);
					}
					else {
						LayoutItemCaptionWidth = CalculateLayoutItemCaptionWidth(layoutModel, viewItems);
						foreach(IModelViewLayoutElement layoutItemModel in layoutModel) {
							container.Controls.Add(ProcessItem(viewItems, layoutItemModel));
						}
					}
				}
				OnLayoutCreated();
				return container;
			}
		}
		public override void UpdateViewItem(ViewItem viewItem) {
			foreach(LayoutItemTemplateContainerBase templateContainer in Items.Values) {
				LayoutItemTemplateContainer layoutItemTemplateContainer = templateContainer as LayoutItemTemplateContainer;
				if(layoutItemTemplateContainer != null && layoutItemTemplateContainer.ViewItem != null && layoutItemTemplateContainer.ViewItem.Id == viewItem.Id) {
					layoutItemTemplateContainer.Controls.Clear();
					LayoutItemTemplate.InstantiateIn(layoutItemTemplateContainer);
					break;
				}
			}
		}
		public override void BreakLinksToControls() {
			BreakLinksToTemplateControls(LayoutItemTemplate);
			BreakLinksToTemplateControls(LayoutGroupTemplate);
			BreakLinksToTemplateControls(TabbedGroupTemplate);
			container = null;
			Items.Clear();
			base.BreakLinksToControls();
		}
		public override void Dispose() {
			try {
				if(container != null) {
					container.Dispose();
					BreakLinksToControls();
				}
				DisposeTemplate(LayoutItemTemplate);
				DisposeTemplate(LayoutGroupTemplate);
				DisposeTemplate(TabbedGroupTemplate);
				LayoutItemTemplate = null;
				LayoutGroupTemplate = null;
				TabbedGroupTemplate = null;
			}
			finally {
				base.Dispose();
			}
		}
		public ViewEditMode ViewEditMode {
			get { return viewEditMode; }
			set { viewEditMode = value; }
		}
		public Dictionary<String, LayoutItemTemplateContainerBase> Items {
			get { return items; }
		}
		public LayoutBaseTemplate TabbedGroupTemplate {
			get { return tabbedGroupTemplate; }
			set { tabbedGroupTemplate = value; }
		}
		public LayoutBaseTemplate LayoutGroupTemplate {
			get { return layoutGroupTemplate; }
			set { layoutGroupTemplate = value; }
		}
		public LayoutBaseTemplate LayoutItemTemplate {
			get { return layoutItemTemplate; }
			set { layoutItemTemplate = value; }
		}
		public Unit LayoutItemCaptionWidth {
			get { return layoutItemCaptionWidth; }
			set { layoutItemCaptionWidth = value; }
		}
		public DevExpress.Utils.Locations DefaultLayoutItemCaptionLocation {
			get { return defaultLayoutItemCaptionLocation; }
			set { defaultLayoutItemCaptionLocation = value; }
		}
		public DevExpress.Utils.HorzAlignment DefaultLayoutItemCaptionHorizontalAlignment {
			get { return defaultLayoutItemCaptionHorizontalAlignment; }
			set { defaultLayoutItemCaptionHorizontalAlignment = value; }
		}
		public DevExpress.Utils.VertAlignment DefaultLayoutItemCaptionVerticalAlignment {
			get { return defaultLayoutItemCaptionVerticalAlignment; }
			set { defaultLayoutItemCaptionVerticalAlignment = value; }
		}
		public event EventHandler<PageControlCreatedEventArgs> PageControlCreated;
		public event EventHandler<ContainerControlCreatedEventArgs> ContainerControlCreated;
		public event EventHandler<ItemCreatedEventArgs> ItemCreated;
	}
	public class ContainerControlCreatedEventArgs : EventArgs {
		private CustomPanel container;
		public ContainerControlCreatedEventArgs(CustomPanel container) {
			this.container = container;
		}
		public CustomPanel ContainerControl {
			get { return this.container; }
		}
	}
	public class ItemCreatedEventArgs : EventArgs {
		private LayoutItemTemplateContainerBase templateContainer;
		private IModelViewLayoutElement modelLayoutElement;
		private ViewItem viewItem;
		public ItemCreatedEventArgs(IModelViewLayoutElement modelLayoutElement, ViewItem viewItem, LayoutItemTemplateContainerBase itemTemplateContainer) {
			this.templateContainer = itemTemplateContainer;
			this.modelLayoutElement = modelLayoutElement;
			this.viewItem = viewItem;
		}
		public LayoutItemTemplateContainerBase TemplateContainer {
			get { return templateContainer; }
		}
		public IModelViewLayoutElement ModelLayoutElement {
			get { return modelLayoutElement; }
		}
		public ViewItem ViewItem {
			get { return viewItem; }
		}
	}
}
