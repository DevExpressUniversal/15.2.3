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
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.Win.Templates;
using DevExpress.ExpressApp.Win.Templates.ActionContainers;
using DevExpress.ExpressApp.Win.Templates.ActionContainers.Items;
using DevExpress.LookAndFeel;
using DevExpress.Persistent.Base;
using DevExpress.Skins;
using DevExpress.XtraBars;
using DevExpress.XtraBars.Helpers;
using DevExpress.XtraBars.Ribbon;
namespace DevExpress.ExpressApp.Win.SystemModule {
	public class ChooseSkinController : WindowController, IModelExtender {
		private DevExpress.ExpressApp.Actions.SingleChoiceAction chooseSkinAction;
		private System.ComponentModel.IContainer components;
		private DefaultSkinListGenerator generator;
		private IClassicToRibbonTransformerHolder ribbonTransformerHolder;
		private void chooseSkinAction_OnExecute(Object sender, SingleChoiceActionExecuteEventArgs args) {
			string newSkin = (string)args.SelectedChoiceActionItem.Data;
			ChooseSkin(newSkin);
		}
		private void InitializeComponent() {
			this.components = new System.ComponentModel.Container();
			this.chooseSkinAction = new DevExpress.ExpressApp.Actions.SingleChoiceAction(this.components);
			this.chooseSkinAction.Caption = "Choose Skin";
			this.chooseSkinAction.Category = "Appearance";
			this.chooseSkinAction.Id = "ChooseSkin";
			this.chooseSkinAction.ImageName = "MenuBar_ChooseSkin";
			this.chooseSkinAction.Execute += new DevExpress.ExpressApp.Actions.SingleChoiceActionExecuteEventHandler(this.chooseSkinAction_OnExecute);
			this.TargetWindowType = DevExpress.ExpressApp.WindowType.Main;
		}
		private void Window_TemplateChanged(object sender, EventArgs e) {
			OnWindowTemplateChanged();
		}
		private void UnsubscribeFromRibbonTransformerHolder() {
			if(ribbonTransformerHolder != null && ribbonTransformerHolder.RibbonTransformer != null) {
				ribbonTransformerHolder.RibbonTransformer.BarItemAdding -= new EventHandler<BarItemAddingEventArgs>(RibbonTransformer_BarItemAdding);
				ribbonTransformerHolder = null;
			}
		}
		private void RibbonTransformer_BarItemAdding(object sender, BarItemAddingEventArgs e) {
			if(e.Action != null && e.Action.Id == ChooseSkinAction.Id) {
				e.Item = new RibbonGallerySingleChoiceActionItem(ChooseSkinAction, e.Item.Manager).Control;
			}
		}
		private void BarActionItemsFactory_CustomizeActionControl(object sender, CustomizeActionControlEventArgs e) {
			if(e.Action == chooseSkinAction) {
				BarGallerySingleChoiceActionItem barGalleryActionItem = new BarGallerySingleChoiceActionItem(chooseSkinAction, e.ActionControl.Manager, BarButtonStyle.Check);
				e.ActionControl = barGalleryActionItem;
			}
		}
		private void SkinHelper_CreateCustomGalleryItem(object sender, CreateCustomGalleryItemEventArgs e) {
			ChoiceActionItem currentActionItem = chooseSkinAction.Items.Find(e.SkinName, ChoiceActionItemFindType.Recursive, ChoiceActionItemFindTarget.Any);
			if(currentActionItem != null && currentActionItem.Active && currentActionItem.Enabled) {
				e.GalleryItem = new ChoiceActionGalleryItem(currentActionItem);
				if(currentActionItem.Caption != e.SkinName) {
					e.GalleryItem.Caption = currentActionItem.Caption;
				}
			}
		}
		private void AddSkinActionItem(string skinName) {
			ChoiceActionItem newItem = new ChoiceActionItem(skinName, skinName);
			if(SkinManager.Default.Skins[skinName] != null) {
				chooseSkinAction.Items[0].Items.Add(newItem);
			}
			else {
				chooseSkinAction.Items.Add(newItem);
			}
		}
		private bool IsStandardFormStyle {
			get { return ((IModelOptionsWin)Application.Model.Options).FormStyle == RibbonFormStyle.Standard; }
		}
		protected virtual void ChooseSkin(string skinName) {
			generator.SetLookAndFeelStyle(skinName);
		}
		protected virtual void OnWindowTemplateChanged() {
			UnsubscribeFromRibbonTransformerHolder();
			ribbonTransformerHolder = Window.Template as IClassicToRibbonTransformerHolder;
			if(ribbonTransformerHolder != null && ribbonTransformerHolder.RibbonTransformer != null) {
				ribbonTransformerHolder.RibbonTransformer.BarItemAdding += new EventHandler<BarItemAddingEventArgs>(RibbonTransformer_BarItemAdding);
			}
		}
		protected override void OnDeactivated() {
			UnsubscribeFromRibbonTransformerHolder();
			Window.TemplateChanged -= new EventHandler(Window_TemplateChanged);
			if(IsStandardFormStyle) {
				BarActionItemsFactory.CustomizeActionControl -= new EventHandler<CustomizeActionControlEventArgs>(BarActionItemsFactory_CustomizeActionControl);
			}
			base.OnDeactivated();
			generator = null;
		}
		protected override void OnActivated() {
			base.OnActivated();
			generator.SetModel(Application.Model);
			if(!IsStandardFormStyle) {
				foreach(ChoiceActionItem choiceActionItem in chooseSkinAction.Items) {
					if(choiceActionItem.Id != "Skins") {
						choiceActionItem.Active["Ribbon form style"] = false;
					}
				}
			}
			ChoiceActionItem predefinedSkinActionItem = chooseSkinAction.Items.Find(generator.GetPredefinedLookAndFeelStyle(), ChoiceActionItemFindType.Recursive, ChoiceActionItemFindTarget.Any);
			if(predefinedSkinActionItem != null) {
				chooseSkinAction.DoExecute(predefinedSkinActionItem);
			}
			Window.TemplateChanged += new EventHandler(Window_TemplateChanged);
			if(IsStandardFormStyle) {
				BarActionItemsFactory.CustomizeActionControl += new EventHandler<CustomizeActionControlEventArgs>(BarActionItemsFactory_CustomizeActionControl);
			}
		}
		public ChooseSkinController()
			: base() {
			InitializeComponent();
			RegisterActions(components);
			Initialize();
		}
		private void Initialize() {
			generator = new DefaultSkinListGenerator();
			if(generator.Skins != null) {
				try {
					chooseSkinAction.BeginUpdate();
					chooseSkinAction.Items.Clear();
					chooseSkinAction.Items.Add(new ChoiceActionItem("Skins", null));
					foreach(string skinNode in generator.Skins) {
						AddSkinActionItem(skinNode);
					}
				}
				finally {
					chooseSkinAction.EndUpdate();
				}
			}
		}
		public void FillSkinGallery(GalleryDropDown skinGallery) {
			SkinHelper.CreateCustomGalleryItem += new EventHandler<CreateCustomGalleryItemEventArgs>(SkinHelper_CreateCustomGalleryItem);
			SkinHelper.InitSkinGalleryDropDown(skinGallery, false);
			SkinHelper.CreateCustomGalleryItem -= new EventHandler<CreateCustomGalleryItemEventArgs>(SkinHelper_CreateCustomGalleryItem);
		}
		public void FillSkinGallery(RibbonGalleryBarItem skinGalleryBarItem) {
			SkinHelper.CreateCustomGalleryItem += new EventHandler<CreateCustomGalleryItemEventArgs>(SkinHelper_CreateCustomGalleryItem);
			SkinHelper.InitSkinGallery(skinGalleryBarItem, true, false);
			SkinHelper.CreateCustomGalleryItem -= new EventHandler<CreateCustomGalleryItemEventArgs>(SkinHelper_CreateCustomGalleryItem);
		}
		public SingleChoiceAction ChooseSkinAction {
			get { return chooseSkinAction; }
		}
		#region IExtendModel Members
		void IModelExtender.ExtendModelInterfaces(ModelInterfaceExtenders extenders) {
			extenders.Add<IModelOptions, IModelApplicationOptionsSkin>();
		}
		#endregion
	}
	public class DefaultSkinListGenerator {
		public const string WindowsThemeName = "Windows Theme";
		public const string DevExpressStyleSkinName = "DevExpress Style";
		public const string BlueSkinName = "Blue";
		private IModelApplicationOptionsSkin model;
		private List<string> skins = null;
		private static bool enableFormSkins = true;
		static DefaultSkinListGenerator() {
			DevExpress.UserSkins.BonusSkins.Register();
		}
		public DefaultSkinListGenerator() { }
		public DefaultSkinListGenerator(IModelApplication model) {
			SetModel(model);
		}
		public void SetModel(IModelApplication model) {
			this.model = (IModelApplicationOptionsSkin)model.Options;
		}
		internal static void FillSkins(List<string> skins, bool generateWindowsTheme) {
			if(generateWindowsTheme) {
				skins.Add(WindowsThemeName);
				foreach(LookAndFeelStyle style in Enum.GetValues(typeof(LookAndFeelStyle))) {
					if(style != LookAndFeelStyle.Skin) {
						skins.Add(style.ToString());
					}
				}
			}
			foreach(SkinContainer skinContainer in SkinManager.Default.Skins) {
				skins.Add(skinContainer.SkinName);
			}
			skins.Sort();
		}
		public List<string> Skins {
			get {
				if(skins == null) {
					skins = new List<string>();
					FillSkins(skins, true);
				}
				return skins;
			}
		}
		public void SetLookAndFeelStyle(string styleName) {
			if(!string.IsNullOrEmpty(styleName)) {
				if(styleName == DefaultSkinListGenerator.WindowsThemeName) {
					UserLookAndFeel.Default.UseWindowsXPTheme = true;
					UserLookAndFeel.Default.SetWindowsXPStyle();
				}
				else {
					UserLookAndFeel.Default.UseWindowsXPTheme = false;
					bool styleFound = false;
					foreach(LookAndFeelStyle style in Enum.GetValues(typeof(LookAndFeelStyle))) {
						if(style.ToString() == styleName) {
							UserLookAndFeel.Default.Style = style;
							styleFound = true;
							break;
						}
					}
					if(!styleFound) {
						UserLookAndFeel.Default.Style = LookAndFeelStyle.Skin;
						UserLookAndFeel.Default.SkinName = styleName;
					}
				}
				StorePredefinedLookAndFeelStyle(styleName);
				if(enableFormSkins) {
					DevExpress.Skins.SkinManager.EnableFormSkins();
				}
				else {
					DevExpress.Skins.SkinManager.DisableFormSkins();
					DevExpress.Skins.SkinManager.EnableFormSkinsIfNotVista();
				}
				DevExpress.Skins.SkinManager.EnableMdiFormSkins();
			}
		}
		public void StorePredefinedLookAndFeelStyle(string styleName) {
			if((model != null && model.Skin != styleName)) {
				model.Skin = styleName;
			}
		}
		public void SetPredefinedLookAndFeelStyle() {
			SetLookAndFeelStyle(GetPredefinedLookAndFeelStyle());
		}
		public string GetPredefinedLookAndFeelStyle() {
			string result = null;
			if(model != null) {
				result = model.Skin;
				if(string.IsNullOrEmpty(result)) {
					result = Skins[0];
				}
			}
			return result;
		}
		public static bool EnableFormSkins {
			get { return enableFormSkins; }
			set { enableFormSkins = value; }
		}
	}
	public class ChoiceActionGalleryItem : GalleryItem {
		private ChoiceActionItem actionItem;
		public ChoiceActionItem ActionItem {
			get { return actionItem; }
		}
		public ChoiceActionGalleryItem(ChoiceActionItem actionItem) {
			this.actionItem = actionItem;
		}
		protected override GalleryItem CreateItem() {
			return new ChoiceActionGalleryItem(actionItem);
		}
	}
	public class BarGallerySingleChoiceActionItem : BarSingleChoiceOperationActionItem {
		private GalleryDropDown skinGalleryDropDown;
		private void skinGallery_GalleryItemClick(object sender, GalleryItemClickEventArgs e) {
			ChoiceActionGalleryItem galleryItem = (ChoiceActionGalleryItem)e.Item;
			Action.DoExecute(galleryItem.ActionItem);
		}
		protected override void CreateSubItems(BarItemLinkCollection itemLinks, ChoiceActionItemCollection items) {
			foreach(ChoiceActionItem actionItem in items) {
				if(actionItem.Active) {
					BarChoiceActionItem barItem = null;
					barItem = CreateDropDownMenuButtonItem(actionItem);
					ChoiceActionItemToWrapperMap.Add(actionItem, barItem);
					itemLinks.Add(barItem.Control).BeginGroup = actionItem.BeginGroup;
					if(actionItem.Items.Count > 0) {
						skinGalleryDropDown = new GalleryDropDown();
						skinGalleryDropDown.Manager = Manager;
						((ChooseSkinController)Action.Controller).FillSkinGallery(skinGalleryDropDown);
						skinGalleryDropDown.GalleryItemClick += new GalleryItemClickEventHandler(skinGallery_GalleryItemClick);
						BarButtonItem barButtonItem = (BarButtonItem)barItem.Control;
						barButtonItem.ButtonStyle = BarButtonStyle.DropDown;
						barButtonItem.DropDownControl = skinGalleryDropDown;
						barButtonItem.ActAsDropDown = true;
					}
				}
			}
		}
		public BarGallerySingleChoiceActionItem(SingleChoiceAction singleChoiceAction, BarManager manager, BarButtonStyle itemStyle)
			: base(singleChoiceAction, manager, itemStyle) {
		}
		public override void Dispose() {
			if(skinGalleryDropDown != null) {
				skinGalleryDropDown.GalleryItemClick -= new GalleryItemClickEventHandler(skinGallery_GalleryItemClick);
				skinGalleryDropDown.Dispose();
				skinGalleryDropDown = null;
			}
			base.Dispose();
		}
		public GalleryDropDown SkinGalleryDropDown {
			get { return skinGalleryDropDown; }
		}
	}
	public class RibbonGallerySingleChoiceActionItem : BarSingleChoiceActionItemBase {
		private RibbonGalleryBarItem skinGalleryBarItem;
		private void skinGalleryBarItem_GalleryItemClick(object sender, DevExpress.XtraBars.Ribbon.GalleryItemClickEventArgs e) {
			ChoiceActionGalleryItem galleryItem = (ChoiceActionGalleryItem)e.Item;
			Action.DoExecute(galleryItem.ActionItem);
		}
		protected override BarItem CreateControlCore() {
			skinGalleryBarItem = new RibbonGalleryBarItem(Manager);
			skinGalleryBarItem.GalleryItemClick += new GalleryItemClickEventHandler(skinGalleryBarItem_GalleryItemClick);
			((ChooseSkinController)Action.Controller).FillSkinGallery(skinGalleryBarItem);
			return skinGalleryBarItem;
		}
		protected override void RebuildItemsCore() {
			((ChooseSkinController)Action.Controller).FillSkinGallery(skinGalleryBarItem);
		}
		public RibbonGallerySingleChoiceActionItem(SingleChoiceAction singleChoiceAction, BarManager manager) : base(singleChoiceAction, manager) { }
		public override void Dispose() {
			if(skinGalleryBarItem != null) {
				skinGalleryBarItem.GalleryItemClick -= new GalleryItemClickEventHandler(skinGalleryBarItem_GalleryItemClick);
				skinGalleryBarItem.Dispose();
				skinGalleryBarItem = null;
			}
			base.Dispose();
		}
	}
	public interface IModelApplicationOptionsSkin {
		[Browsable(false)]
		List<string> Skins { get; }
		[
#if !SL
	DevExpressExpressAppWinLocalizedDescription("IModelApplicationOptionsSkinSkin"),
#endif
 DataSourceProperty("Skins")]
		[Category("Appearance")]
		string Skin { get; set; }
	}
	[DomainLogic(typeof(IModelOptionsWin))]
	public static class ModelOptionsWinForSkin {
		public static void BeforeSet_FormStyle(IModelOptionsWin node, object value) {
			List<string> skins = new List<string>();
			DefaultSkinListGenerator.FillSkins(skins, (RibbonFormStyle)value != DevExpress.XtraBars.Ribbon.RibbonFormStyle.Ribbon);
			if(!skins.Contains(((IModelApplicationOptionsSkin)node).Skin)) {
				((ModelNode)(IModelApplicationOptionsSkin)node).ClearValue("Skin");
			}
		}
	}
	[DomainLogic(typeof(IModelApplicationOptionsSkin))]
	public static class ModelApplicationOptionsSkinLogic {
		private static List<string> skins = null;
		public static List<string> Get_Skins(IModelApplicationOptionsSkin modelSkin) {
			skins = new List<string>();
			DefaultSkinListGenerator.FillSkins(skins, ((IModelOptionsWin)modelSkin).FormStyle != DevExpress.XtraBars.Ribbon.RibbonFormStyle.Ribbon);
			return skins;
		}
		public static string Get_Skin(IModelApplicationOptionsSkin modelSkin) {
			bool generateWindowsTheme = ((IModelOptionsWin)((IModelNode)modelSkin).Application.Options).FormStyle != DevExpress.XtraBars.Ribbon.RibbonFormStyle.Ribbon;
			if(DevExpress.Utils.WXPaint.WXPPainter.Default.IsVista && generateWindowsTheme) {
				return DefaultSkinListGenerator.DevExpressStyleSkinName;
			}
			else {
				return DefaultSkinListGenerator.BlueSkinName;
			}
		}
	}
}
