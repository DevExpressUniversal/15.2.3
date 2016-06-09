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
using System.Text;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Actions;
using DevExpress.Data.Filtering;
using DevExpress.Persistent.Base;
using DevExpress.ExpressApp.DC;
using System.Collections;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Utils;
using System.ComponentModel;
namespace DevExpress.ExpressApp.PivotChart {
	public class AnalysisInfoLink {
		private string keyValue;
		private string name;
		private string viewId;
		private Type dataType;
		public string KeyValue {
			get { return keyValue; }
			set { keyValue = value; }
		}
		public string Name {
			get { return name; }
			set { name = value; }
		}
		public string ViewId {
			get { return viewId; }
			set { viewId = value; }
		}
		public Type DataType {
			get { return dataType; }
			set { dataType = value; }
		}
		public AnalysisInfoLink() {
		}
		public AnalysisInfoLink(string keyValue, string name, string viewId, Type dataType) {
			this.keyValue = keyValue;
			this.name = name;
			this.viewId = viewId;
			this.dataType = dataType;
		}
	}
	public class CustomCollectAnalysisLinksEventArgs : HandledEventArgs {
		public CustomCollectAnalysisLinksEventArgs(XafApplication application, Dictionary<Type, List<AnalysisInfoLink>> links) {
			this.Application = application;
			this.Links = links;
		}
		public Dictionary<Type, List<AnalysisInfoLink>> Links { get; private set; }
		public XafApplication Application { get; private set; }
	}
	public class CustomizeNavigationItemsController : WindowController {
		private Dictionary<Type, List<AnalysisInfoLink>> links = new Dictionary<Type, List<AnalysisInfoLink>>();
		private ShowNavigationItemController showNavigationItemController;
		private void showNavigationItemController_NavigationItemCreated(object sender, NavigationItemCreatedEventArgs e) {
			ChoiceActionItem navigationItem = e.NavigationItem;
			IModelView modelView = ((IModelNavigationItem)e.NavigationItem.Model).View;
			if(modelView is IModelObjectView) {
				List<AnalysisInfoLink> linkList;
				if(links.TryGetValue(((IModelObjectView)modelView).ModelClass.TypeInfo.Type, out linkList) && linkList.Count > 0) {
					ChoiceActionItem analysisNavigationGroup = new ChoiceActionItem("Analysis", RelatedAnalysisGroupCaption, null);
					analysisNavigationGroup.Model.ImageName = "BO_Analysis";
					e.NavigationItem.Items.Add(analysisNavigationGroup);
					foreach(AnalysisInfoLink link in linkList) {
						ChoiceActionItem item = new ChoiceActionItem(link.KeyValue.ToString(), link.Name, new ViewShortcut(link.ViewId, link.KeyValue));
						item.Model.ImageName = "Navigation_Item_PivotChart";
						analysisNavigationGroup.Items.Add(item);
					}
				}
			}
		}
		private bool GenerateRelatedAnalysisGroup {
			get {
				IModelPivotChartNavigation navigationItems = (IModelPivotChartNavigation)((IModelApplicationNavigationItems)Application.Model).NavigationItems;
				return navigationItems.GenerateRelatedAnalysisGroup;
			}
		}
		private string RelatedAnalysisGroupCaption {
			get {
				IModelPivotChartNavigation navigationItems = (IModelPivotChartNavigation)((IModelApplicationNavigationItems)Application.Model).NavigationItems;
				return navigationItems.RelatedAnalysisGroupCaption ?? PivotChartModuleBase.DefaultAnalysisDataNavigationItemCaption;
			}
		}
		protected override void OnFrameAssigned() {
			base.OnFrameAssigned();
			if(GenerateRelatedAnalysisGroup && Frame.Context == TemplateContext.ApplicationWindow) {
				showNavigationItemController = Frame.GetController<ShowNavigationItemController>();
				if(showNavigationItemController != null) {
					CollectAnalysisLinks();
					showNavigationItemController.NavigationItemCreated -= new EventHandler<NavigationItemCreatedEventArgs>(showNavigationItemController_NavigationItemCreated);
					showNavigationItemController.NavigationItemCreated += new EventHandler<NavigationItemCreatedEventArgs>(showNavigationItemController_NavigationItemCreated);
				}
			}
		}
		private void CollectAnalysisLinks() {
			links.Clear();
			CustomCollectAnalysisLinksEventArgs args = new CustomCollectAnalysisLinksEventArgs(Application, links);
			if(CustomCollectAnalysisLinks != null) {
				CustomCollectAnalysisLinks(this, args);
			}
			if(!args.Handled) {
				ITypeInfo analysisTypeInfo = XafTypesInfo.Instance.FindTypeInfo(typeof(IAnalysisInfo));
				foreach(ITypeInfo typeInfo in analysisTypeInfo.Implementors) {
					if(typeInfo.IsPersistent) {
						Type analysisType = XafTypesInfo.CastTypeInfoToType(typeInfo);
						IMemberInfo defaultMember = typeInfo.DefaultMember;
						List<DevExpress.Xpo.SortProperty> sorting = new List<DevExpress.Xpo.SortProperty>();
						if(defaultMember.IsPersistent) {
							sorting.Add(new DevExpress.Xpo.SortProperty(defaultMember.Name, DevExpress.Xpo.DB.SortingDirection.Ascending));
						}
						using(IObjectSpace objectSpace = Application.CreateObjectSpace(analysisType)) {
							IList analysisObjects = objectSpace.GetObjects(analysisType, CriteriaOperator.Parse(""));
							foreach(IAnalysisInfo analysis in analysisObjects) {
								if(analysis.DataType != null) {
									List<AnalysisInfoLink> listToAddTo;
									if(!links.TryGetValue(analysis.DataType, out listToAddTo)) {
										listToAddTo = new List<AnalysisInfoLink>();
										links.Add(analysis.DataType, listToAddTo);
									}
									string viewId = Application.GetDetailViewId(analysis.GetType());
									string keyValue = objectSpace.GetKeyValueAsString(analysis);
									Guard.ArgumentNotNull(defaultMember, "defaultMember");
									object name = defaultMember.GetValue(analysis);
									AnalysisInfoLink link = new AnalysisInfoLink(keyValue, name == null ? string.Empty : name.ToString(), viewId, analysis.DataType);
									listToAddTo.Add(link);
								}
							}
						}
					}
				}
			}
		}
		protected override void Dispose(bool disposing) {
			if(showNavigationItemController != null) {
				showNavigationItemController.NavigationItemCreated -= new EventHandler<NavigationItemCreatedEventArgs>(showNavigationItemController_NavigationItemCreated);
				showNavigationItemController = null;
			}
			base.Dispose(disposing);
		}
		public CustomizeNavigationItemsController() {
			this.TargetWindowType = WindowType.Main;
		}
		public event EventHandler<CustomCollectAnalysisLinksEventArgs> CustomCollectAnalysisLinks;
	}
}
