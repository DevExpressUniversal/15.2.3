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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Layout;
using DevExpress.ExpressApp.Localization;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.Model.NodeGenerators;
using DevExpress.ExpressApp.Win.Controls;
using DevExpress.ExpressApp.Win.Layout;
using DevExpress.Persistent.Validation;
using DevExpress.XtraEditors;
using DevExpress.XtraLayout;
using DevExpress.XtraLayout.Customization;
namespace DevExpress.ExpressApp.Win.SystemModule {
	public interface IModelWinLayoutManagerOptions : IModelNode {
		[
#if !SL
	DevExpressExpressAppWinLocalizedDescription("IModelWinLayoutManagerOptionsCustomizationEnabled"),
#endif
 DefaultValue(true)]
		[Category("Behavior")]
		bool CustomizationEnabled { get; set; }
		[
#if !SL
	DevExpressExpressAppWinLocalizedDescription("IModelWinLayoutManagerOptionsItemToItemVerticalDistance"),
#endif
 DefaultValue(4)]
		[Category("Layout")]
		int ItemToItemVerticalDistance { get; set; }
		[
#if !SL
	DevExpressExpressAppWinLocalizedDescription("IModelWinLayoutManagerOptionsItemToItemHorizontalDistance"),
#endif
 DefaultValue(10)]
		[Category("Layout")]
		int ItemToItemHorizontalDistance { get; set; }
		[
#if !SL
	DevExpressExpressAppWinLocalizedDescription("IModelWinLayoutManagerOptionsItemToBorderVerticalDistance"),
#endif
 DefaultValue(10)]
		[Category("Layout")]
		int ItemToBorderVerticalDistance { get; set; }
		[
#if !SL
	DevExpressExpressAppWinLocalizedDescription("IModelWinLayoutManagerOptionsItemToBorderHorizontalDistance"),
#endif
 DefaultValue(10)]
		[Category("Layout")]
		int ItemToBorderHorizontalDistance { get; set; }
		[
#if !SL
	DevExpressExpressAppWinLocalizedDescription("IModelWinLayoutManagerOptionsInvisibleGroupVerticalDistance"),
#endif
 DefaultValue(10)]
		[Category("Layout")]
		int InvisibleGroupVerticalDistance { get; set; }
		[
#if !SL
	DevExpressExpressAppWinLocalizedDescription("IModelWinLayoutManagerOptionsItemToTabBorderDistance"),
#endif
 DefaultValue(2)]
		[Category("Layout")]
		int ItemToTabBorderDistance { get; set; }
		[
#if !SL
	DevExpressExpressAppWinLocalizedDescription("IModelWinLayoutManagerOptionsTextToControlDistance"),
#endif
 DefaultValue(5)]
		[Category("Layout")]
		int TextToControlDistance { get; set; }
		[
#if !SL
	DevExpressExpressAppWinLocalizedDescription("IModelWinLayoutManagerOptionsTextAlignModeItem"),
#endif
 DefaultValue(TextAlignModeItem.UseParentOptions)]
		[Category("Layout")]
		TextAlignModeItem TextAlignModeItem { get; set; }
		[
#if !SL
	DevExpressExpressAppWinLocalizedDescription("IModelWinLayoutManagerOptionsTextAlignModeGroup"),
#endif
 DefaultValue(TextAlignModeGroup.UseParentOptions)]
		[Category("Layout")]
		TextAlignModeGroup TextAlignModeGroup { get; set; }
	}
	[RuleCriteria(
		@"[UIType] != ##Enum#DevExpress.ExpressApp.UIType,StandardMDI# Or [FormStyle] != ##Enum#DevExpress.XtraBars.Ribbon.RibbonFormStyle,Ribbon#",
		UsedProperties = "UIType,FormStyle",
		CustomMessageTemplate = "The combination of UIType = 'StandardMDI' and FormStyle = 'Ribbon' settings is not supported. Please change one of these settings to proceed."
	)]
	public interface IModelOptionsWin : IModelNode {
		[
#if !SL
	DevExpressExpressAppWinLocalizedDescription("IModelOptionsWinFormStyle"),
#endif
 DefaultValue(DevExpress.XtraBars.Ribbon.RibbonFormStyle.Standard)]
		[Category("Appearance")]
		[RefreshPropertiesAttribute(RefreshProperties.All)]
		DevExpress.XtraBars.Ribbon.RibbonFormStyle FormStyle { get; set; }
#if !SL
	[DevExpressExpressAppWinLocalizedDescription("IModelOptionsWinRibbonOptions")]
#endif
		IModelOptionsRibbon RibbonOptions { get; }
		[
#if !SL
	DevExpressExpressAppWinLocalizedDescription("IModelOptionsWinMdiDefaultNewWindowTarget"),
#endif
 DefaultValue(NewWindowTarget.Default)]
		[Category("Behavior")]
		NewWindowTarget MdiDefaultNewWindowTarget { get; set; }
		[
#if !SL
	DevExpressExpressAppWinLocalizedDescription("IModelOptionsWinMessaging"),
#endif
 DefaultValue("DevExpress.ExpressApp.Win.Core.Messaging")]
		[Category("Behavior")]
		string Messaging { get; set; }
		[
#if !SL
	DevExpressExpressAppWinLocalizedDescription("IModelOptionsWinUIType"),
#endif
 DefaultValue(DevExpress.ExpressApp.UIType.MultipleWindowSDI)]
		[Category("Appearance")]
		UIType UIType { get; set; }
	}
#if !SL
	[DevExpressExpressAppWinLocalizedDescription("SystemModuleIModelOptionsRibbon")]
#endif
	public interface IModelOptionsRibbon : IModelNode{
		[
#if !SL
	DevExpressExpressAppWinLocalizedDescription("IModelOptionsRibbonRibbonControlStyle"),
#endif
 DefaultValue(DevExpress.XtraBars.Ribbon.RibbonControlStyle.Office2010)]
		[Category("Appearance")]
		DevExpress.XtraBars.Ribbon.RibbonControlStyle RibbonControlStyle { get; set; }
		[
#if !SL
	DevExpressExpressAppWinLocalizedDescription("IModelOptionsRibbonMinimizeRibbon"),
#endif
 DefaultValue(false)]
		[Category("Appearance")]
		bool MinimizeRibbon { get; set; }
	}
	public interface IModelWinLayoutManagerDetailViewOptions : IModelWinLayoutManagerOptions {
	}
	[DomainLogic(typeof(IModelWinLayoutManagerDetailViewOptions))]
	public static class WinLayoutManagerDetailViewOptionsLogic {
		public static bool Get_CustomizationEnabled(IModelWinLayoutManagerDetailViewOptions viewOptionsModel) {
			if(viewOptionsModel.Application != null) {
				return ((IModelWinLayoutManagerOptions)viewOptionsModel.Application.Options.LayoutManagerOptions).CustomizationEnabled;
			}
			return true;
		}
		public static int Get_ItemToItemVerticalDistance(IModelWinLayoutManagerDetailViewOptions viewOptionsModel) {
			if(viewOptionsModel.Application != null) {
				return ((IModelWinLayoutManagerOptions)viewOptionsModel.Application.Options.LayoutManagerOptions).ItemToItemVerticalDistance;
			}
			return 10;
		}
		public static int Get_ItemToItemHorizontalDistance(IModelWinLayoutManagerDetailViewOptions viewOptionsModel) {
			if(viewOptionsModel.Application != null) {
				return ((IModelWinLayoutManagerOptions)viewOptionsModel.Application.Options.LayoutManagerOptions).ItemToItemHorizontalDistance;
			}
			return 10;
		}
		public static int Get_ItemToBorderVerticalDistance(IModelWinLayoutManagerDetailViewOptions viewOptionsModel) {
			if(viewOptionsModel.Application != null) {
				return ((IModelWinLayoutManagerOptions)viewOptionsModel.Application.Options.LayoutManagerOptions).ItemToBorderVerticalDistance;
			}
			return 10;
		}
		public static int Get_ItemToBorderHorizontalDistance(IModelWinLayoutManagerDetailViewOptions viewOptionsModel) {
			if(viewOptionsModel.Application != null) {
				return ((IModelWinLayoutManagerOptions)viewOptionsModel.Application.Options.LayoutManagerOptions).ItemToBorderHorizontalDistance;
			}
			return 10;
		}
		public static int Get_InvisibleGroupVerticalDistance(IModelWinLayoutManagerDetailViewOptions viewOptionsModel) {
			if(viewOptionsModel.Application != null) {
				return ((IModelWinLayoutManagerOptions)viewOptionsModel.Application.Options.LayoutManagerOptions).InvisibleGroupVerticalDistance;
			}
			return 10;
		}
		public static int Get_ItemToTabBorderDistance(IModelWinLayoutManagerDetailViewOptions viewOptionsModel) {
			if(viewOptionsModel.Application != null) {
				return ((IModelWinLayoutManagerOptions)viewOptionsModel.Application.Options.LayoutManagerOptions).ItemToTabBorderDistance;
			}
			return 2;
		}
		public static int Get_TextToControlDistance(IModelWinLayoutManagerDetailViewOptions viewOptionsModel) {
			if(viewOptionsModel.Application != null) {
				return ((IModelWinLayoutManagerOptions)viewOptionsModel.Application.Options.LayoutManagerOptions).TextToControlDistance;
			}
			return 5;
		}
		public static TextAlignModeItem Get_TextAlignModeItem(IModelWinLayoutManagerDetailViewOptions viewOptionsModel) {
			if(viewOptionsModel.Application != null) {
				return ((IModelWinLayoutManagerOptions)viewOptionsModel.Application.Options.LayoutManagerOptions).TextAlignModeItem;
			}
			return TextAlignModeItem.UseParentOptions;
		}
		public static TextAlignModeGroup Get_TextAlignModeGroup(IModelWinLayoutManagerDetailViewOptions viewOptionsModel) {
			if(viewOptionsModel.Application != null) {
				return ((IModelWinLayoutManagerOptions)viewOptionsModel.Application.Options.LayoutManagerOptions).TextAlignModeGroup;
			}
			return TextAlignModeGroup.UseParentOptions;
		}
	}
	[ModelPersistentName("SplitterItem")]
#if !SL
	[DevExpressExpressAppWinLocalizedDescription("SystemModuleIModelSplitter")]
#endif
	public interface IModelSplitter : IModelLayoutItem {
	}
	[ModelPersistentName("SeparatorItem")]
#if !SL
	[DevExpressExpressAppWinLocalizedDescription("SystemModuleIModelSeparator")]
#endif
	public interface IModelSeparator : IModelLayoutItem {
	}
	[ModelPersistentName("LabelItem")]
#if !SL
	[DevExpressExpressAppWinLocalizedDescription("SystemModuleIModelLabel")]
#endif
	public interface IModelLabel : IModelLayoutItem {
		[
#if !SL
	DevExpressExpressAppWinLocalizedDescription("IModelLabelText"),
#endif
 Localizable(true)]
		[Category("Data")]
		string Text { get; set; }
	}
	public interface IModelWinLayoutItem : IModelNode, IModelLayoutElementWithCaptionOptions {
		[
#if !SL
	DevExpressExpressAppWinLocalizedDescription("IModelWinLayoutItemTextAlignMode"),
#endif
 DefaultValue(TextAlignModeItem.UseParentOptions)]
		[Category("Behavior")]
		TextAlignModeItem TextAlignMode { get; set; }
	}
	[DomainLogic(typeof(IModelWinLayoutItem))]
	public static class WinLayoutItemLogic {
		internal static IModelWinLayoutManagerOptions GetWinViewLayoutOptions(IModelNode elementModel) {
			IModelNode parent = elementModel.Parent as IModelNode;
			IModelDetailView detailViewModel = parent as IModelDetailView;
			while(parent != null && detailViewModel == null) {
				parent = parent.Parent;
				detailViewModel = parent as IModelDetailView;
			}
			return detailViewModel as IModelWinLayoutManagerOptions;
		}
		public static TextAlignModeItem Get_TextAlignMode(IModelWinLayoutItem modelWinLayoutItem) {
			IModelWinLayoutManagerOptions optionsModel = GetWinViewLayoutOptions(modelWinLayoutItem);
			if(optionsModel != null) {
				return optionsModel.TextAlignModeItem;
			}
			return TextAlignModeItem.UseParentOptions;
		}
	}
	public interface IModelWinLayoutGroup : IModelNode, IModelLayoutElementWithCaption {
		[
#if !SL
	DevExpressExpressAppWinLocalizedDescription("IModelWinLayoutGroupTextAlignMode"),
#endif
 DefaultValue(TextAlignModeGroup.UseParentOptions)]
		[Category("Behavior")]
		TextAlignModeGroup TextAlignMode { get; set; }
	}
	[DomainLogic(typeof(IModelWinLayoutGroup))]
	public static class ModelWinLayoutGroupLogic {
		public static TextAlignModeGroup Get_TextAlignMode(IModelWinLayoutGroup modelWinLayoutItem) {
			IModelWinLayoutManagerOptions optionsModel = WinLayoutItemLogic.GetWinViewLayoutOptions(modelWinLayoutItem);
			if(optionsModel != null) {
				return optionsModel.TextAlignModeGroup;
			}
			return TextAlignModeGroup.UseParentOptions;
		}
	}
	public class WinLayoutManagerController : ColumnChooserControllerBase, IModelExtender {
		private LayoutControl layoutControl;
		private DetailView DetailView {
			get { return (DetailView)View; }
		}
		private LayoutManager LayoutManager {
			get { return (DetailView != null) ? DetailView.LayoutManager : null; }
		}
		private CustomizationForm CustomizationForm {
			get { return (layoutControl != null) ? layoutControl.CustomizationForm as CustomizationForm : null; }
		}
		private void DeleteEditorForLayoutControlItem(LayoutControlItem item) {
			DetailView.RemoveItem(item.Name);
		}
		private void DeleteEditorsForLayoutControlGroup(LayoutControlGroup layoutControlGroup) {
			IList items = layoutControlGroup.Items;
			while(items.Count > 0) {
				object item = items[0];
				if(item is LayoutControlItem) {
					DeleteEditorForLayoutControlItem((LayoutControlItem)item);
				}
				else if(item is LayoutControlGroup) {
					DeleteEditorsForLayoutControlGroup((LayoutControlGroup)item);
				}
				if(items.Contains(item)) {
					items.Remove(item);
				}
			}
		}
		private void DeleteEditorForTabbedControlGroup(TabbedControlGroup tabbedControlGroup) {
			foreach(BaseLayoutItem item in tabbedControlGroup.TabPages) {
				DeleteEditorsForLayoutItem(item);
			}
		}
		private void DeleteEditorsForLayoutItem(BaseLayoutItem item) {
			if(item is LayoutControlItem) {
				DeleteEditorForLayoutControlItem((LayoutControlItem)item);
			}
			else if(item is LayoutControlGroup) {
				DeleteEditorsForLayoutControlGroup((LayoutControlGroup)item);
			}
			else if(item is TabbedControlGroup) {
				DeleteEditorForTabbedControlGroup((TabbedControlGroup)item);
			}
		}
		private void LayoutControl_ShowCustomization(object sender, EventArgs e) {
			if(CustomizationForm != null) {
				InsertButtons();
				EnableRemoveButtonByCurrentSelection();
				((ListBoxControl)ActiveListBox).SelectedIndexChanged += new EventHandler(ActiveListBox_SelectedIndexChanged);
				ActiveListBox.MouseClick += new MouseEventHandler(ActiveListBox_MouseClick);
				CustomizationForm.VisibleChanged += new EventHandler(CustomizationForm_VisibleChanged);
			}
		}
		private void CustomizationForm_VisibleChanged(object sender, EventArgs e) {
			if(CustomizationForm != null && !CustomizationForm.Visible && ActiveListBox != null) {
				CustomizationForm.VisibleChanged -= new EventHandler(CustomizationForm_VisibleChanged);
				((ListBoxControl)ActiveListBox).SelectedIndexChanged -= new EventHandler(ActiveListBox_SelectedIndexChanged);
				ActiveListBox.MouseClick -= new MouseEventHandler(ActiveListBox_MouseClick);
			}
		}
		private void EnableRemoveButtonByCurrentSelection() {
			RemoveButton.Enabled = CanRemoveSelectedColumn;
		}
		private bool CanRemoveSelectedColumn {
			get {
				BaseLayoutItem layoutItem = (BaseLayoutItem)CustomizationForm.hiddenItemsList1.SelectedItem;
				if(layoutItem != null) {
					return layoutControl.HiddenItems.IndexOf(layoutItem) >= 0;
				}
				return false;
			}
		}
		private void ActiveListBox_MouseClick(object sender, MouseEventArgs e) {
			EnableRemoveButtonByCurrentSelection();
		}
		private void ActiveListBox_SelectedIndexChanged(object sender, EventArgs e) {
			EnableRemoveButtonByCurrentSelection();
		}
		private void layoutControl_HideCustomization(object sender, EventArgs e) {
			if(CustomizationForm != null) {
				CustomizationForm.hiddenItemsList1.PressedItemIndex = -1;
				DeleteButtons();
			}
		}
		private void SetMenuManager() {
			if(Frame.Template == null) {
				Frame.TemplateChanged += new EventHandler(Frame_TemplateChanged);
			}
			else {
				IBarManagerHolder holder = Frame.Template as IBarManagerHolder;
				if(holder != null) {
					layoutControl.MenuManager = holder.BarManager;
					holder.BarManagerChanged += new EventHandler(holder_BarManagerChanged);
				}
			}
		}
		private void holder_BarManagerChanged(object sender, EventArgs e) {
			((IBarManagerHolder)sender).BarManagerChanged -= new EventHandler(holder_BarManagerChanged);
			SetMenuManager();
		}
		private void view_ControlsCreated(object sender, EventArgs e) {
			layoutControl = (XafLayoutControl)LayoutManager.Container;
			layoutControl.ShowCustomization += new EventHandler(LayoutControl_ShowCustomization);
			layoutControl.HideCustomization += new EventHandler(layoutControl_HideCustomization);
			layoutControl.PopupMenuShowing += new PopupMenuShowingEventHandler(layoutControl_PopupMenuShowing);
			SetMenuManager();
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public bool CalculateAllowLayoutPopupMenu() {
			return !(View.ObjectTypeInfo.Type == typeof(DevExpress.ExpressApp.Utils.ParametersObject));
		}
		private void layoutControl_PopupMenuShowing(object sender, PopupMenuShowingEventArgs e) {
			e.Allow = CalculateAllowLayoutPopupMenu();
		}
		private void Frame_TemplateChanged(object sender, EventArgs e) {
			SetMenuManager();
		}
		private void layoutControl_ResetLayout(object sender, EventArgs e) {
			Cursor cursor = Cursor.Current;
			Cursor.Current = Cursors.WaitCursor;
			layoutControl.BeginUpdate();
			((ModelNode)(DetailView.Model.Layout)).Undo();
			((ModelNode)(DetailView.Model.Items)).Undo();
			DetailView.BreakLinksToControls();
			DetailView.LoadModel();
			DetailView.CreateControls();
			layoutControl.EndUpdate();
			Cursor.Current = cursor;
		}
		protected override void OnActivated() {
			base.OnActivated();
			layoutControl = (XafLayoutControl)LayoutManager.Container;
			if(layoutControl != null) {
				layoutControl.ShowCustomization += new EventHandler(LayoutControl_ShowCustomization);
				layoutControl.HideCustomization += new EventHandler(layoutControl_HideCustomization);
				layoutControl.DefaultLayoutLoaded += new EventHandler(layoutControl_ResetLayout);
				layoutControl.PopupMenuShowing += new PopupMenuShowingEventHandler(layoutControl_PopupMenuShowing);
				SetMenuManager();
			}
			else {
				View.ControlsCreated += new EventHandler(view_ControlsCreated);
			}
		}
		protected override void OnDeactivated() {
			IBarManagerHolder holder = Frame.Template as IBarManagerHolder;
			if(holder != null) {
				holder.BarManagerChanged -= new EventHandler(holder_BarManagerChanged);
			}
			View.ControlsCreated -= new EventHandler(view_ControlsCreated);
			Frame.TemplateChanged -= new EventHandler(Frame_TemplateChanged);
			if(layoutControl != null) {
				layoutControl.ShowCustomization -= new EventHandler(LayoutControl_ShowCustomization);
				layoutControl.HideCustomization -= new EventHandler(layoutControl_HideCustomization);
				layoutControl.DefaultLayoutLoaded -= new EventHandler(layoutControl_ResetLayout);
				layoutControl.PopupMenuShowing -= new PopupMenuShowingEventHandler(layoutControl_PopupMenuShowing);
				if(layoutControl.IsHandleCreated) { layoutControl.HideCustomizationForm(); }
				layoutControl = null;
			}
			base.OnDeactivated();
		}
		protected override void AddButtonsToCustomizationForm() {
			CustomizationForm.hiddenItemsList1.AllowResetFocusOnMouseLeave = false;
			CustomizationForm.layoutControl1.Controls.Add(this.RemoveButton);
			CustomizationForm.layoutControl1.Controls.Add(this.AddButton);
			LayoutControlItem addButtonLayoutItem = CustomizationForm.hiddenItemsGroup.AddItem();
			addButtonLayoutItem.Control = this.AddButton;
			addButtonLayoutItem.Padding = new DevExpress.XtraLayout.Utils.Padding(5, 5, 0, 0);
			addButtonLayoutItem.TextVisible = false;
			LayoutControlItem removeButtonLayoutItem = CustomizationForm.hiddenItemsGroup.AddItem();
			removeButtonLayoutItem.Control = this.RemoveButton;
			removeButtonLayoutItem.Padding = new DevExpress.XtraLayout.Utils.Padding(5, 5, 0, 5);
			removeButtonLayoutItem.TextVisible = false;			
		}
		protected override Control ActiveListBox {
			get { return CustomizationForm != null ? CustomizationForm.hiddenItemsList1 : null; }
		}
		protected override List<string> GetUsedProperties() {
			List<string> displayedPropertyNames = new List<string>();
			foreach(ViewItem detailViewItem in DetailView.Items) {
				if(detailViewItem is PropertyEditor) {
					displayedPropertyNames.Add(((PropertyEditor)detailViewItem).PropertyName);
				}
			}
			return displayedPropertyNames;
		}
		protected override ITypeInfo DisplayedTypeInfo {
			get { return ((DetailView)View).ObjectTypeInfo; }
		}
		protected override void AddColumn(string propertyName) {
			foreach(ViewItem detailViewItem in DetailView.Items) {
				if((detailViewItem is PropertyEditor) && (propertyName == ((PropertyEditor)detailViewItem).PropertyName)) {
					throw new Exception(SystemExceptionLocalizer.GetExceptionMessage(ExceptionId.CannotAddDuplicateProperty, propertyName));
				}
			}
			IModelPropertyEditor itemModel = (IModelPropertyEditor)DetailView.Model.Items[propertyName];
			if(itemModel == null) {
				itemModel = DetailView.Model.Items.AddNode<IModelPropertyEditor>(propertyName);
				itemModel.PropertyName = propertyName;
				IMemberInfo memberDescriptor = ((DetailView)View).ObjectTypeInfo.FindMember(propertyName);
				if((memberDescriptor != null) && memberDescriptor.IsList && (memberDescriptor.ListElementType != null)) {
					string viewId = ModelNestedListViewNodesGeneratorHelper.GetNestedListViewId(memberDescriptor);
					itemModel.View = Application.Model.Views[viewId];
				}
			}
			if(LayoutManager is WinLayoutManager) {
				((WinLayoutManager)LayoutManager).CreateControlItem(DetailView.AddItem(itemModel));
			}
		}
		protected override void RemoveSelectedColumn() {
			if(CanRemoveSelectedColumn) {
				BaseLayoutItem layoutItem = (BaseLayoutItem)CustomizationForm.hiddenItemsList1.SelectedItem;
				layoutControl.HiddenItems.RemoveAt(layoutControl.HiddenItems.IndexOf(layoutItem));
				DeleteEditorsForLayoutItem(layoutItem);
				CustomizationForm.hiddenItemsList1.PressedItemIndex = -1;
				CustomizationForm.Update();
			}
		}
		public WinLayoutManagerController() {
			TypeOfView = typeof(DetailView);
			CanAddListProperty = true;
			MultiSelect = true;
		}
		void IModelExtender.ExtendModelInterfaces(ModelInterfaceExtenders extenders) {
			extenders.Add<IModelLayoutManagerOptions, IModelWinLayoutManagerOptions>();
			extenders.Add<IModelDetailView, IModelWinLayoutManagerDetailViewOptions>();
			extenders.Add<IModelDashboardView, IModelWinLayoutManagerDetailViewOptions>();
			extenders.Add<IModelLayoutViewItem, IModelWinLayoutItem>();
			extenders.Add<IModelLayoutGroup, IModelWinLayoutGroup>();
			extenders.Add<IModelTabbedGroup, IModelLayoutElementWithCaption>();
		}
	}
}
