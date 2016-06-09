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
using DevExpress.Web.Design;
namespace DevExpress.Web.Design {
	public class ASPxNewsControlDesigner : ASPxDataViewControlDesignerBase {
		ASPxNewsControl NewsControl { get { return (ASPxNewsControl)Control; } }
		string[] fControlTemplateNames = new string[] { 
			"PagerPanelLeftTemplate", 
			"PagerPanelRightTemplate", 
			"EmptyDataTemplate"
		};
		protected override string[] ControlTemplateNames {
			get { return fControlTemplateNames; }
		}
		protected override void DataBind(ASPxDataWebControlBase dataControl) {
			ASPxNewsControl newsControl = dataControl as ASPxNewsControl;
			if (!string.IsNullOrEmpty(newsControl.DataSourceID) || (newsControl.DataSource != null) ||
				!newsControl.HasItems()) {
				newsControl.Items.Clear();
				base.DataBind(newsControl);
			}
		}
		protected override IEnumerable GetSampleDataSource() {
			return new NewsControlSampleData();
		}
		protected override void FillPropertyNameToCaptionMap(Dictionary<string, string> propertyNameToCaptionMap) {
			base.FillPropertyNameToCaptionMap(propertyNameToCaptionMap);
			propertyNameToCaptionMap.Add("Items", "Items");
		}
		public override void RunDesigner() {
			ShowDialog(new WrapperEditorForm(new NewsControlItemsOwner(NewsControl, DesignerHost)));
		}
	}
	public class NewsControlItemsOwner : FlatCollectionItemsOwner<NewsItem> {
		public NewsControlItemsOwner(object control, IServiceProvider provider) 
			: base(control, provider, ((ASPxNewsControl)control).Items) {
		}
	}
}
