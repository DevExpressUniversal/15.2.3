﻿#region Copyright (c) 2000-2015 Developer Express Inc.
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
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model.NodeGenerators;
using System.Collections.Generic;
namespace DevExpress.ExpressApp.Model.DomainLogics {
	[DomainLogic(typeof(IModelDashboardView))]
	public static class ModelDashboardViewLogic {
		public const string DefaultDashboardViewImageName = "Navigation_Item_Dashboard";
		public static string Get_ImageName(IModelDashboardView modelDashboardView) {
			return DefaultDashboardViewImageName;
		}
		public static string Get_Caption(IModelDashboardView modelView) {
			return modelView.Id;
		}
	}
	[DomainLogic(typeof(IModelDashboardViewItem))]
	public static class ModelDashboardViewItemLogic {
		public static IModelList<IModelView> Get_Views(IModelDashboardViewItem dashboardViewItem) {
			List<IModelView> result = new List<IModelView>();
			foreach(IModelView view in dashboardViewItem.Application.Views) {
				if(view is IModelDashboardView) {
					continue;
				}
				result.Add(view);
			}
			return new CalculatedModelNodeList<IModelView>(result);
		}
	}
}
