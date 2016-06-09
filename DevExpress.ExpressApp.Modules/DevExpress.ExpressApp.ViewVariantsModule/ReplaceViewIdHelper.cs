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

using DevExpress.ExpressApp.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
namespace DevExpress.ExpressApp.ViewVariantsModule {
	public class ReplaceViewIdHelper : IDisposable {
		public const string ViewShortcut_ViewVariantViewId = "ViewVariant_ViewId";
		private static Dictionary<int, VariantedViewItem> variantedViewItems = new Dictionary<int, VariantedViewItem>();
		private static object RootViewIdsLockObject = new object();
		private IViewsFactory factoryEvents;
		private Stack<string> intialViewIdList = new Stack<string>();
		private IVariantsProvider variantsProvider;
		private class RedirectViewShortcutToRootViewIdCustomizer {
			private string rootViewId;
			private void View_CustomizeViewShortcut_ChangeViewIdToRootViewId(object sender, CustomizeViewShortcutArgs e) {
				e.ViewShortcut[ViewShortcut_ViewVariantViewId] = e.ViewShortcut.ViewId;
				e.ViewShortcut.ViewId = rootViewId;
			}
			public RedirectViewShortcutToRootViewIdCustomizer(string rootViewId) {
				Guard.ArgumentNotNullOrEmpty(rootViewId, "rootViewId");
				this.rootViewId = rootViewId;
			}
			public void Attach(View view) {
				Guard.ArgumentNotNull(view, "view");
				view.CustomizeViewShortcut += new EventHandler<CustomizeViewShortcutArgs>(View_CustomizeViewShortcut_ChangeViewIdToRootViewId);
			}
			public void Detach(View view) {
				Guard.ArgumentNotNull(view, "view");
				view.CustomizeViewShortcut -= new EventHandler<CustomizeViewShortcutArgs>(View_CustomizeViewShortcut_ChangeViewIdToRootViewId);
			}
		}
		private static void view_Disposing(object sender, CancelEventArgs e) {
			UnregisterRootViewVariant((View)sender);
		}
		private const string NoVariantsId = "{29C04109-35F9-4503-912D-EE3151F1512D}";
		private void factoryEvents_ViewCreating(object sender, ViewCreatingEventArgs e) {
			if(e.IsRoot) {
				intialViewIdList.Clear();
			}
			string viewId = NoVariantsId;
			VariantsInfo variantsInfo = variantsProvider.GetVariants(e.ViewID);
			if(variantsInfo != null) {
				VariantInfo currentVariantInfo = variantsInfo.GetCurrentVariantInfo();
				if(!string.IsNullOrEmpty(currentVariantInfo.ViewID) && (e.ViewID != currentVariantInfo.ViewID)) {
					viewId = variantsInfo.RootViewId;
					e.ViewID = currentVariantInfo.ViewID;
				}
			}
			intialViewIdList.Push(viewId);
		}
		private void factoryEvents_ViewCreated(object sender, ViewCreatedEventArgs e) {
			if((e.View != null) && (intialViewIdList.Count > 0)) {
				string intialViewId = intialViewIdList.Pop();
				if((intialViewId != NoVariantsId) && (e.View.Id != intialViewId)) {
					RegisterRootViewVariant(intialViewId, e.View);
				}
			}
		}
		static ReplaceViewIdHelper() {
			ViewShortcut.EqualsDefaultIgnoredParameters.Add(ViewShortcut_ViewVariantViewId);
		}
		public ReplaceViewIdHelper(IVariantsProvider variantsProvider) {
			Guard.ArgumentNotNull(variantsProvider, "variantsProvider");
			this.variantsProvider = variantsProvider;
		}
		public void Attach(IViewsFactory factoryEvents) {
			Guard.ArgumentNotNull(factoryEvents, "factoryEvents");
			this.factoryEvents = factoryEvents;
			factoryEvents.ViewCreating += factoryEvents_ViewCreating;
			factoryEvents.ViewCreated += factoryEvents_ViewCreated;
		}
		public void Dispose() {
			if(factoryEvents != null) {
				factoryEvents.ViewCreating -= factoryEvents_ViewCreating;
				factoryEvents.ViewCreated -= factoryEvents_ViewCreated;
				factoryEvents = null;
			}
		}
		public static void RegisterRootViewVariant(string rootViewId, View view) {
			Guard.ArgumentNotNull(view, "view");
			lock(RootViewIdsLockObject) {
				view.Disposing += view_Disposing;
				RedirectViewShortcutToRootViewIdCustomizer customizer = new RedirectViewShortcutToRootViewIdCustomizer(rootViewId);
				variantedViewItems.Add(view.GetHashCode(), new VariantedViewItem(rootViewId, customizer));
				customizer.Attach(view);
			}
		}
		public static void UnregisterRootViewVariant(View view) {
			Guard.ArgumentNotNull(view, "view");
			lock(RootViewIdsLockObject) {
				VariantedViewItem item;
				if(variantedViewItems.TryGetValue(view.GetHashCode(), out item)) {
					item.Customizer.Detach(view);
				}
				variantedViewItems.Remove(view.GetHashCode());
			}
		}
		public static bool TryGetRegisteredRootViewId(View view, out string rootViewId) {
			Guard.ArgumentNotNull(view, "view");
			rootViewId = null;
			lock(RootViewIdsLockObject) {
				VariantedViewItem item;
				if(variantedViewItems.TryGetValue(view.GetHashCode(), out item)) {
					rootViewId = item.RootViewId;
					return true;
				}
			}
			return false;
		}
		private class VariantedViewItem {
			public VariantedViewItem(string rootViewId, RedirectViewShortcutToRootViewIdCustomizer customizer) {
				Guard.ArgumentNotNull(customizer, "customizer");
				this.RootViewId = rootViewId;
				this.Customizer = customizer;
			}
			public string RootViewId { get; private set; }
			public RedirectViewShortcutToRootViewIdCustomizer Customizer { get; private set; }
		}
	}
}
