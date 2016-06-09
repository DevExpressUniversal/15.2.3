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
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Templates;
using DevExpress.ExpressApp.Utils;
using DevExpress.Persistent.Base;
using DevExpress.ExpressApp.Core;
using DevExpress.ExpressApp.Editors;
namespace DevExpress.ExpressApp.Kpi {
#if !SL
	[DevExpressExpressAppKpiLocalizedDescription("KpiIModelKpiDashboardViewItem")]
#endif
	public interface IModelKpiDashboardViewItem : IModelDashboardViewItem {
#if !SL
	[DevExpressExpressAppKpiLocalizedDescription("IModelKpiDashboardViewItemKpiName")]
#endif
		[Required]
		string KpiName { get; set; }
	}
	[DomainLogic(typeof(IModelKpiDashboardViewItem))]
	public class ModelKpiDashboardViewItemLogic {
		public static string Get_Criteria(IModelKpiDashboardViewItem node) {
			if(!string.IsNullOrEmpty(node.KpiName)) {
				return string.Format("KpiName = '{0}'", node.KpiName);
			}
			return string.Empty;
		}
		public static IModelView Get_View(IModelKpiDashboardViewItem node) {
			return node.Views["KpiInstance_Chart_DetailView"];
		}
	}
	public class KpiDashboardItemPropertiesVisibilityCalculator : IModelIsVisible {
		public bool IsVisible(IModelNode node, String propertyName) {
			return !(node is IModelKpiDashboardViewItem && (propertyName == "Criteria" || propertyName == "View"));
		}
	} 
	public class KpiDashboardOrganizationItemController : ViewController<DetailView> {
		KpiDashboardOrganizationItem kpiItem;
		private void kpiItem_QueryObjectSpace(object sender, QueryObjectSpaceEventArgs e) {
			e.ObjectSpace = View.ObjectSpace;
		}
		private void SubscribeOnQueryObjectSpace() {
			UnsubscribeFromQueryObjectSpace();
			kpiItem = View.CurrentObject as KpiDashboardOrganizationItem;
			if(kpiItem != null) {
				kpiItem.QueryObjectSpace += new EventHandler<QueryObjectSpaceEventArgs>(kpiItem_QueryObjectSpace);
			}
		}
		private void UnsubscribeFromQueryObjectSpace() {
			if(kpiItem != null) {
				kpiItem.QueryObjectSpace -= new EventHandler<QueryObjectSpaceEventArgs>(kpiItem_QueryObjectSpace);
			}
		}
		private void View_CurrentObjectChanged(object sender, EventArgs e) {
			SubscribeOnQueryObjectSpace();
		}
		protected override void OnActivated() {
			base.OnActivated();
			SubscribeOnQueryObjectSpace();
			View.CurrentObjectChanged += new EventHandler(View_CurrentObjectChanged);
		}
		protected override void OnDeactivated() {
			base.OnDeactivated();
			View.CurrentObjectChanged -= new EventHandler(View_CurrentObjectChanged);
			UnsubscribeFromQueryObjectSpace();
		}
		public KpiDashboardOrganizationItemController() {
			TargetObjectType = typeof(KpiDashboardOrganizationItem);
		}
	}
	public class QueryObjectSpaceEventArgs : EventArgs {
		private IObjectSpace objectSpace;
		public IObjectSpace ObjectSpace {
			get { return objectSpace; }
			set { objectSpace = value; }
		}
	}
	[DomainComponent]
	[XafDisplayName("KPI")]
	public class KpiDashboardOrganizationItem : DashboardOrganizationItem<IModelKpiDashboardViewItem> {
		private string kpiName = string.Empty;
		private IObjectSpace FindObjectSpace() {
			QueryObjectSpaceEventArgs args = new QueryObjectSpaceEventArgs();
			if(QueryObjectSpace != null) {
				QueryObjectSpace(this, args);
			}
			return args.ObjectSpace;
		}
		private IList GetAvailableIndicators() {
			IObjectSpace os = FindObjectSpace();
			if(os != null) {
				return os.GetObjects(KpiDefinitionType);
			}
			return new List<IKpiDefinition>();
		}
		private IKpiDefinition FindKpiByName(string kpiName) {
			IKpiDefinition result = null;
			if(!string.IsNullOrEmpty(kpiName)) {
				IObjectSpace os = FindObjectSpace();
				if(os != null) {
					result = os.FindObject(KpiDefinitionType, CriteriaOperator.Parse(string.Format("Name='{0}'", kpiName))) as IKpiDefinition;
				}
			}
			return result;
		}
		protected override void InitializeFromViewItemCore(IModelKpiDashboardViewItem sourceModelViewItem) {
			this.kpiName = sourceModelViewItem.KpiName;
			this.ActionsToolbarVisibility = sourceModelViewItem.ActionsToolbarVisibility;
		}
		protected override void SetupItemCore(IModelKpiDashboardViewItem modelViewItem) {
			modelViewItem.KpiName = kpiName;
			modelViewItem.ActionsToolbarVisibility = this.ActionsToolbarVisibility;
		}
		public KpiDashboardOrganizationItem(IModelApplication modelApplication) : base(modelApplication) { }
		public override string ToString() {
			return string.Format("{0}: {1}", CaptionHelper.GetLocalizedText(LocalizationGroupName, "Kpi"), kpiName);
		}
		[Browsable(false)]
		public Type KpiDefinitionType {
			get {
				Type kpiDefinitionType = null;
				foreach (ITypeInfo exportedTypeInfo in XafTypesInfo.Instance.PersistentTypes) {
					Type exportedType = exportedTypeInfo.Type;
					if (typeof(IKpiDefinition).IsAssignableFrom(exportedType)) {
						kpiDefinitionType = exportedType;
					}
				}
				return kpiDefinitionType; 
			}
		}
		[DataSourceProperty("AvailableIndicators")]
		[EditorAlias(EditorAliases.LookupPropertyEditor)]
		[DataTypeProperty("KpiDefinitionType")]
		public IKpiDefinition Kpi {
			get { return FindKpiByName(kpiName); }
			set { kpiName = value.Name; }
		}
		public ActionsToolbarVisibility ActionsToolbarVisibility { get; set; }
		[Browsable(false)]
		[ElementTypeProperty("KpiDefinitionType")]
		public IList AvailableIndicators {
			get { return GetAvailableIndicators(); }
		}
		public event EventHandler<QueryObjectSpaceEventArgs> QueryObjectSpace;
	}
}
