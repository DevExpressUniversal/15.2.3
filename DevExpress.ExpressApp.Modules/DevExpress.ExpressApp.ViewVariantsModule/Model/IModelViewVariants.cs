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
using System.Linq;
using System.Text;
using DevExpress.ExpressApp.Model;
using System.ComponentModel;
using DevExpress.Persistent.Base;
using DevExpress.ExpressApp.DC;
namespace DevExpress.ExpressApp.ViewVariantsModule {
	public interface IModelViewVariants : IModelNode {
#if !SL
	[DevExpressExpressAppViewVariantsModuleLocalizedDescription("IModelViewVariantsVariants")]
#endif
		IModelVariants Variants { get; }
	}
	[ImageName("ModelEditor_Views")]
#if !SL
	[DevExpressExpressAppViewVariantsModuleLocalizedDescription("ViewVariantsModuleIModelVariants")]
#endif
	public interface IModelVariants : IModelNode, IModelList<IModelVariant> {
		[DataSourceProperty("this")]
		[
#if !SL
	DevExpressExpressAppViewVariantsModuleLocalizedDescription("IModelVariantsCurrent"),
#endif
 Category("Behavior")]
		IModelVariant Current { get; set; }
	}
	[DisplayProperty("Caption")]
	[ImageName("ModelEditor_ListView")] 
#if !SL
	[DevExpressExpressAppViewVariantsModuleLocalizedDescription("ViewVariantsModuleIModelVariant")]
#endif
	public interface IModelVariant : IModelNode {
#if !SL
	[DevExpressExpressAppViewVariantsModuleLocalizedDescription("IModelVariantId")]
#endif
		string Id { get; set; }
		[Browsable(false)]
		IModelList<IModelView> Views { get; } 
		[Required()]
		[DataSourceProperty("Views")]
		[ModelPersistentName("ViewID")]
		[Category("Appearance")]
#if !SL
	[DevExpressExpressAppViewVariantsModuleLocalizedDescription("IModelVariantView")]
#endif
		IModelView View { get; set; }
		[
#if !SL
	DevExpressExpressAppViewVariantsModuleLocalizedDescription("IModelVariantCaption"),
#endif
 Localizable(true)]
		string Caption { get; set; }
	}
	[DomainLogic(typeof(IModelVariant))]
	public static class ModelVariantLogic {
		public static IModelList<IModelView> Get_Views(IModelVariant modelVariant) {
			CalculatedModelNodeList<IModelView> views = new CalculatedModelNodeList<IModelView>();
			if(modelVariant.Parent == null) {
				return views;
			}
			IModelObjectView parentView = modelVariant.Parent.Parent as IModelObjectView;
			if(parentView == null) {
				views.AddRange(modelVariant.Application.Views);
				return views;
			}
			if(parentView.ModelClass == null) {
				return views;
			}
			Type parentViewType = parentView.GetType();
			ITypeInfo parentClassTypeInfo = parentView.ModelClass.TypeInfo;
			foreach(IModelView modelView in modelVariant.Application.Views) {
				if(modelView.GetType() == parentViewType) {
					if(modelView is IModelObjectView) {
						IModelClass modelViewClass = ((IModelObjectView)modelView).ModelClass;
						if(modelViewClass != null && modelViewClass.TypeInfo.IsAssignableFrom(parentClassTypeInfo)) {
							views.Add(modelView);
						}
					}
				}
			}
			return views;
		}
		public static string Get_Caption(IModelVariant modelVariant) {
			string caption = string.Empty;
			if(modelVariant.View != null) {
				caption = string.IsNullOrEmpty(modelVariant.View.Caption) ? modelVariant.View.Id : modelVariant.View.Caption;
			}
			return caption;
		}
	}
}
