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

using System.ComponentModel;
using System;
using DevExpress.Persistent.Base;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Templates;
namespace DevExpress.ExpressApp.Model {
	[ImageName("ModelEditor_DashboardView")]
#if !SL
	[DevExpressExpressAppLocalizedDescription("ModelIModelDashboardView")]
#endif
	public interface IModelDashboardView : IModelView, IModelCompositeView, IModelLayoutManagerOptions {
	}
#if !SL
	[DevExpressExpressAppLocalizedDescription("ModelIModelDashboardViewItem")]
#endif
	public interface IModelDashboardViewItem : IModelViewItem {
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("IModelDashboardViewItemView"),
#endif
 DataSourceProperty("Views")]
		[ModelPersistentName("ViewId")]
		[Category("Appearance")]
		[Required]
		IModelView View { get; set; }
		[Browsable(false)]
		IModelList<IModelView> Views { get; }
		[Category("Behavior")]
#if !SL
	[DevExpressExpressAppLocalizedDescription("IModelDashboardViewItemCriteria")]
#endif
		[CriteriaOptions("View.ModelClass.TypeInfo")]
		[Editor("DevExpress.ExpressApp.Win.Core.ModelEditor.CriteriaModelEditorControl, DevExpress.ExpressApp.Win" + XafAssemblyInfo.VersionSuffix + XafAssemblyInfo.AssemblyNamePostfix, typeof(System.Drawing.Design.UITypeEditor))]
		String Criteria { get; set; }		
		[DefaultValue(ActionsToolbarVisibility.Default)]
		[Category("Behavior")]
#if !SL
	[DevExpressExpressAppLocalizedDescription("IModelDashboardViewItemActionsToolbarVisibility")]
#endif
		ActionsToolbarVisibility ActionsToolbarVisibility { get; set; }
	}
}
