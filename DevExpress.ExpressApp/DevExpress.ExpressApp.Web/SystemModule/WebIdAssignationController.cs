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
using System.Text.RegularExpressions;
using System.Web.UI;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Web.Editors;
namespace DevExpress.ExpressApp.Web.SystemModule {
	public class WebIdAssignationController : ViewController {
		public const string DetailViewItemIdPrefix = "dvi";
		private void detailView_ControlsCreating(object sender, EventArgs e) {
			UnsubscribeItemsControlCreated((DetailView)sender);
		}
		private void UnsubscribeItemsControlCreated(DetailView detailView) {
			if(detailView != null) {
				foreach(ViewItem item in detailView.Items) {
					item.ControlCreated -= new EventHandler<EventArgs>(item_ControlCreated);
				}
			}
		}
		private void detailView_ControlsCreated(object sender, EventArgs e) {
			SubscribeItemsControlCreated((DetailView)sender);
		}
		private void SubscribeItemsControlCreated(DetailView detailView) {
			if(detailView != null) {
				foreach(ViewItem item in detailView.Items) {
					SetItemId(item);
					item.ControlCreated += new EventHandler<EventArgs>(item_ControlCreated);
				}
			}
		}
		private void item_ControlCreated(object sender, EventArgs e) {
			SetItemId((ViewItem)sender);
		}
		protected virtual void SetItemId(ViewItem item) {
			string id = WebIdHelper.GetCorrectedId(item.Id, DetailViewItemIdPrefix);
			if(!string.IsNullOrEmpty(item.Id)) {
				if(item is WebPropertyEditor) {
					WebPropertyEditor propertyEditor = (WebPropertyEditor)item;
					propertyEditor.SetControlId(id);
				}
				else {
					if(item.Control is Control) {
						((Control)item.Control).ID = id;
					}
				}
			}
		}
		protected override void OnViewChanging(View view) {
			base.OnViewChanging(view);
			DetailView oldView = View as DetailView;
			if(oldView != null) {
				oldView.ControlsCreated -= new EventHandler(detailView_ControlsCreated);
			}
			DetailView detailView = view as DetailView;
			if(detailView != null) {
				detailView.ControlsCreated += new EventHandler(detailView_ControlsCreated);
				detailView.ControlsCreating += new EventHandler(detailView_ControlsCreating);
			}
		}
		protected override void Dispose(bool disposing) {
			base.Dispose(disposing);
			DetailView detailView = View as DetailView;
			if(detailView != null) {
				detailView.ControlsCreated -= new EventHandler(detailView_ControlsCreated);
				UnsubscribeItemsControlCreated(detailView);
				detailView.ControlsCreating -= new EventHandler(detailView_ControlsCreating);
			}
		}
		public WebIdAssignationController() {
			TargetViewType = ViewType.DetailView;
		}
	}
	public static class WebIdHelper {
		private static MatchEvaluator evaluator = new MatchEvaluator(Replace);
		private static bool simplifyId = true;
		private static Regex patchEx = new Regex("\\W");
		private static Dictionary<string, string> actionIdToShortNameMap = new Dictionary<string, string>();
		private static Dictionary<string, string> layoutItemIdToShortNameMap = new Dictionary<string, string>();
		private static Dictionary<string, string> viewIdToShortNameMap = new Dictionary<string, string>();		
		private static object lockObject = new object();
		private static string GetShortName(Dictionary<string, string> map, string id, string prefix) {
			if(SimplifyId) {
				string result;
				lock(lockObject) {
					if(!map.TryGetValue(id, out result)) {
						result = prefix + map.Count;
						map.Add(id, result);
					}
				}
				return result;
			}
			return id;
		}
		private static string GetLayoutItemShortName(string layoutItemId) {
			return GetShortName(layoutItemIdToShortNameMap, layoutItemId, "l");
		}
		private static string GetActionId(string actionId) {
			return GetShortName(actionIdToShortNameMap, actionId, "a");
		}
		internal static string GetViewShortName(View view) {
			return GetViewShortName(GetCorrectedId(view.Id)) + "_" + view.GetHashCode().ToString();
		}
		internal static string GetViewShortName(string viewId) {
			return GetShortName(viewIdToShortNameMap, viewId, "v");
		}
		public const string DetailViewItemIdPrefix = "xaf_";
		public static bool SimplifyId {
			get { return simplifyId; }
			set { simplifyId = value; }
		}
		public static string GetCorrectedId(string stringId) {
			return GetCorrectedId(stringId, string.Empty);
		}
		public static string GetCorrectedId(string stringId, string prefix) {
			return GetCorrectedId(stringId, prefix, string.Empty);
		}
		public static string GetCorrectedId(string stringId, string prefix, string postfix) {
			return DetailViewItemIdPrefix + prefix + patchEx.Replace(stringId, evaluator) + postfix;
		}
		static string Replace(Match match) {
			return "_";
		}
		public static string GetCorrectedActionId(ActionBase action) {
			string idPrefix = action.Controller != null && !SimplifyId ? action.Controller.GetType().Name + "_" : "";
			return GetCorrectedId(idPrefix + GetActionId(action.Id));
		}
		public static string GetLayoutItemId(IModelViewLayoutElement layoutItemInfo) {
			string result = string.Empty;
			if(layoutItemInfo != null) {
				result = GetLayoutItemShortName(layoutItemInfo.Id);
				IModelNode layoutItemInfoTmp = layoutItemInfo.Parent;
				while(layoutItemInfoTmp is IModelViewLayoutElement) {
					result = GetLayoutItemShortName(GetLayoutItemShortName(((IModelViewLayoutElement)layoutItemInfoTmp).Id) + "_" + result);
					layoutItemInfoTmp = layoutItemInfoTmp.Parent;
				}
			}
			return result;
		}
		public static string GetCorrectedLayoutItemId(IModelViewLayoutElement layoutItemInfo) {
			return GetCorrectedLayoutItemId(layoutItemInfo, "", "");
		}
		public static string GetCorrectedLayoutItemId(IModelViewLayoutElement layoutItemInfo, string prefix, string postfix) {
			return GetCorrectedId(GetLayoutItemId(layoutItemInfo), prefix, postfix);
		}
		public static string GetListEditorControlId(string viewId) {
			return "LE_" + GetViewShortName(GetCorrectedId(viewId));
		}
#if DebugTest
		public static void DebugTest_ClearCache() {
			actionIdToShortNameMap.Clear();
			layoutItemIdToShortNameMap.Clear();
			viewIdToShortNameMap.Clear();
		}
		public static string DebugTest_GetViewShortName(View view) {
			return GetViewShortName(view);
		}
#endif
	}
	public class WebControlIdModelSynchronizer : ModelSynchronizer<Control, IModelListView> {
		public WebControlIdModelSynchronizer(Control control, IModelListView model)
			: base(control, model) {
		}
		protected override void ApplyModelCore() {
			Control.ID = WebIdHelper.GetListEditorControlId(Model.Id); 
		}
		public override void SynchronizeModel() {
		}
	}
}
