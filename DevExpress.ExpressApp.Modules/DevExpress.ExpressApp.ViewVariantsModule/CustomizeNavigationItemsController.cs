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
using System.ComponentModel;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Utils;
namespace DevExpress.ExpressApp.ViewVariantsModule {
	public interface IModelNavigationItemsVariantSettings {
		[Category("Behavior")]
#if !SL
	[DevExpressExpressAppViewVariantsModuleLocalizedDescription("IModelNavigationItemsVariantSettingsGenerateRelatedViewVariantsGroup")]
#endif
		bool GenerateRelatedViewVariantsGroup { get; set; }
		[Localizable(true), DefaultValue(CustomizeNavigationItemsController.RelatedViewVariantsGroupCaptionDefault)]
#if !SL
	[DevExpressExpressAppViewVariantsModuleLocalizedDescription("IModelNavigationItemsVariantSettingsRelatedViewVariantsGroupCaption")]
#endif
		string RelatedViewVariantsGroupCaption { get; set; }
	}
	public class CustomizeNavigationItemsController : WindowController, IModelExtender {
		private const string ViewVariantsItemId = "ViewVariants";
		public const string RelatedViewVariantsGroupCaptionDefault = "View Variants";
		private ShowNavigationItemController showNavigationItemController;
		private void showNavigationItemController_NavigationItemCreated(object sender, NavigationItemCreatedEventArgs e) {
			NavigationItemCreatedHandler(e);
		}
		protected void NavigationItemCreatedHandler(NavigationItemCreatedEventArgs e) {
			if(!Settings.GenerateRelatedViewVariantsGroup) {
				return;
			}
			ChoiceActionItem navigationItem = e.NavigationItem;
			IModelNavigationItem modelItem = e.ModelNavigationItem;
			if(modelItem != null && modelItem.View != null && !string.IsNullOrEmpty(modelItem.View.Id)) {
				VariantsInfo variantsInfo = GetVariants(modelItem.View.Id);
				if(variantsInfo != null) {
					ChoiceActionItem viewVariantsNavigationGroup = new ChoiceActionItem(ViewVariantsItemId, Settings.RelatedViewVariantsGroupCaption, null);
					viewVariantsNavigationGroup.ImageName = "ModelEditor_Views";
					navigationItem.Items.Add(viewVariantsNavigationGroup);
					foreach(VariantInfo variantInfo in variantsInfo.Items) {
						ChoiceActionItem item = new ChoiceActionItem(variantInfo.Id, variantInfo.Caption, new ViewShortcut(variantInfo.ViewID, null));
						item.ImageName = "Navigation_Item_View";
						viewVariantsNavigationGroup.Items.Add(item);
					}
				}
			}
		}
		protected virtual VariantsInfo GetVariants(string viewId) {
			IVariantsProvider result = Application.Modules.FindModule<ViewVariantsModule>().VariantsProvider;
			if(result == null) {
				throw new InvalidOperationException();
			}
			return result.GetVariants(viewId);
		}
		protected virtual IModelNavigationItemsVariantSettings Settings {
			get {
				Guard.ArgumentNotNull(Application, "Application");
				Guard.ArgumentNotNull(Application.Model, "Application.ModelApplication");
				IModelRootNavigationItems navigationItems = ((IModelApplicationNavigationItems)Application.Model).NavigationItems;
				Guard.ArgumentNotNull(navigationItems, "navigationItems");
				return (IModelNavigationItemsVariantSettings)navigationItems;
			}
		}
		public CustomizeNavigationItemsController() { }
		protected override void OnFrameAssigned() {
			base.OnFrameAssigned();
			showNavigationItemController = Frame.GetController<ShowNavigationItemController>();
			if(showNavigationItemController != null) {
				showNavigationItemController.NavigationItemCreated -= new EventHandler<NavigationItemCreatedEventArgs>(showNavigationItemController_NavigationItemCreated);
				showNavigationItemController.NavigationItemCreated += new EventHandler<NavigationItemCreatedEventArgs>(showNavigationItemController_NavigationItemCreated);
			}
		}
		protected override void Dispose(bool disposing) {
			if(showNavigationItemController != null) {
				showNavigationItemController.NavigationItemCreated -= new EventHandler<NavigationItemCreatedEventArgs>(showNavigationItemController_NavigationItemCreated);
				showNavigationItemController = null;
			}
			base.Dispose(disposing);
		}
		#region IExtendModel Members
		void IModelExtender.ExtendModelInterfaces(ModelInterfaceExtenders extenders) {
			extenders.Add<IModelRootNavigationItems, IModelNavigationItemsVariantSettings>();
		}
		#endregion
#if DebugTest
		public const string DebugTest_ViewVariantsItemId = ViewVariantsItemId;
		public void DebugTest_NavigationItemCreatedHandler(NavigationItemCreatedEventArgs e) {
			NavigationItemCreatedHandler(e);
		}
		public IModelNavigationItemsVariantSettings DebugTest_Settings {
			get { return Settings; }
		}
#endif
	}
}
