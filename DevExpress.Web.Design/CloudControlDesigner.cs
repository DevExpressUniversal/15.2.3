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
using DevExpress.Web;
using DevExpress.Web.Internal;
namespace DevExpress.Web.Design {
	public class ASPxCloudControlDesigner : ASPxDataWebControlDesigner {
		public ASPxCloudControl CloudControl { get; private set; }
		public override void Initialize(IComponent component) {
			CloudControl = (ASPxCloudControl)component;
			base.Initialize(component);
		}
		protected override void FillPropertyNameToCaptionMap(Dictionary<string, string> propertyNameToCaptionMap) {
			base.FillPropertyNameToCaptionMap(propertyNameToCaptionMap);
			propertyNameToCaptionMap.Add("Items", "Items");
			propertyNameToCaptionMap.Add("RankProperties", "RankProperties");
		}
		public override void RunDesigner() {
			ShowDialog(new WrapperEditorForm(new CloudControlCommonFormDesigner(CloudControl, DesignerHost)));
		}
		protected override void DataBind(ASPxDataWebControlBase dataControl) {
			ASPxCloudControl cloudControl = dataControl as ASPxCloudControl;
			if (!string.IsNullOrEmpty(cloudControl.DataSourceID) || (cloudControl.DataSource != null) || cloudControl.Items.Count < 1) {
				cloudControl.Items.Clear();
				base.DataBind(cloudControl);
			}
		}
		protected override IEnumerable GetSampleDataSource() {
			return new CloudControlSampleData();
		}
	}
	public class CloudControlCommonFormDesigner : CommonFormDesigner {
		ASPxCloudControl cloudControl;
		FlatCollectionItemsOwner<CloudControlItem> itemsOwner;
		public CloudControlCommonFormDesigner(ASPxCloudControl cloudControl, IServiceProvider provider)
			: base(cloudControl, provider) { 
		}
		ASPxCloudControl CloudControl {
			get {
				if(cloudControl == null)
					cloudControl = (ASPxCloudControl)Control;
				return cloudControl;
			}
		}
		public override ItemsEditorOwner ItemsOwner {
			get {
				if(itemsOwner == null)
					itemsOwner = new FlatCollectionItemsOwner<CloudControlItem>(CloudControl, Provider, CloudControl.Items);
				return itemsOwner;
			}
		}
		protected override void CreateMainGroupItems() {
			base.CreateMainGroupItems();
			AddRankPropertiesItem();
		}
		protected void AddRankPropertiesItem() {
			var rankPropertiesOwner = new RankPropertiesItemsOwner(CloudControl, Provider);
			var insertBefore = MainGroup.IndexOf(MainGroup.GetItemByCaption(ItemsOwner.GetNavBarItemsGroupName()));
			MainGroup.Insert(insertBefore, CreateDesignerItem("Rank Properties", "Rank Properties", typeof(ItemsEditorFrame), CloudControl, RankItemImageIndex, rankPropertiesOwner));
		}
	}
	public class RankPropertiesItemsOwner : FlatCollectionItemsOwner<RankProperties> {
		public RankPropertiesItemsOwner(ASPxCloudControl cloudControl, IServiceProvider provider)
			: base(cloudControl, provider, cloudControl.RankProperties, "RankProperties") {
		}
	}
}
