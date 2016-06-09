#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Dashboard                                                   }
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

using DevExpress.DashboardCommon;
using DevExpress.DashboardWin.Native;
using DevExpress.Utils;
using System;
namespace DevExpress.DashboardWin.ServiceModel {
	public interface IDashboardItemDesignerFactory {
		DashboardItemDesigner CreateDashboardItemDesigner(DashboardItemViewer itemViewer);
	}
	public class DashboardItemDesignerFactory : IDashboardItemDesignerFactory {
		readonly DashboardDesigner designer;
		readonly IServiceProvider serviceProvider;
		public DashboardItemDesignerFactory(DashboardDesigner designer, IServiceProvider serviceProvider) {
			Guard.ArgumentNotNull(designer, "designer");
			Guard.ArgumentNotNull(serviceProvider, "serviceProvider");
			this.designer = designer;
			this.serviceProvider = serviceProvider;
		}
		public DashboardItemDesigner CreateDashboardItemDesigner(DashboardItemViewer itemViewer) {
			DashboardItemDesignerAttribute designerAttribute = Attribute.GetCustomAttribute(itemViewer.GetType(), typeof(DashboardItemDesignerAttribute)) as DashboardItemDesignerAttribute;
			DashboardItemDesigner itemDesigner = (DashboardItemDesigner)Activator.CreateInstance(designerAttribute.DesignerType);
			IDashboardOwnerService ownerService = serviceProvider.RequestServiceStrictly<IDashboardOwnerService>();
			DashboardItem dashboadItem = ownerService.FindDashboardItemOrGroup(itemViewer.Name);
			itemDesigner.Initialize(designer, dashboadItem, itemViewer);
			return itemDesigner;
		}
	}
}
