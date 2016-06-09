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

using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
namespace DevExpress.ExpressApp.ViewVariantsModule {
	public interface IVariantsProvider {
		VariantsInfo GetVariants(string rootVariantViewId);
		void SaveCurrentVariantId(string rootViewId, string currentVariantId);
	}
	public class ModelVariantsProvider : IVariantsProvider {
		private const bool GenerateDefaultVariantDefaultValue = false;
		private const bool AllowSingleVariantDefaultValue = false;
		private static bool allowSingleVariant = AllowSingleVariantDefaultValue;
		private static bool generateDefaultVariant = GenerateDefaultVariantDefaultValue;
		public ModelVariantsProvider(IModelApplication model) : this(model == null ? null : model.Views) {
		}
		public ModelVariantsProvider(IModelList<IModelView> views) {
			Guard.ArgumentNotNull(views, "views");
			this.Views = views;
		}
		public VariantsInfo GetVariants(string rootVariantViewId) {
			IModelViewVariants viewNode = (IModelViewVariants)Views[rootVariantViewId];
			if(viewNode == null) {
				return null;
			}
			IModelVariants variantsNode = viewNode.Variants;
			if(variantsNode != null) {
				List<VariantInfo> variants = new List<VariantInfo>();
				foreach(IModelVariant node in variantsNode.OrderBy((i) => i.Index)) { 
					if(node.View != null) {
						variants.Add(new VariantInfo(node.Id, node.View.Id, node.Caption));
					}
					else {
						variants.Add(new VariantInfo(node.Id, string.Empty, node.Caption));
					}
				}
#pragma warning disable 0618
				if(GenerateDefaultVariant && (variants.Count > 0) &&
					(variantsNode[ChangeVariantController.DefaultVariantId] == null)) {
					variants.Insert(0, new VariantInfo(ChangeVariantController.DefaultVariantId, ((IModelView)((IModelNode)variantsNode).Parent).Id, ChangeVariantController.DefaultVariantId));
				}
#pragma warning restore 0618
				if(variants.Count >= 2 || (AllowSingleVariant && variants.Count == 1)) {
					string currentVariantId = (variantsNode.Current != null) ? variantsNode.Current.Id : "";
					return new VariantsInfo(rootVariantViewId, currentVariantId, variants);
				}
			}
			return null;
		}
		public void SaveCurrentVariantId(string rootViewId, string currentVariantId) {
			Guard.ArgumentNotNullOrEmpty(rootViewId, "rootViewId");
			Guard.ArgumentNotNullOrEmpty(currentVariantId, "currentVariantId");
			IModelViewVariants viewNode = (IModelViewVariants)Views[rootViewId];
			if(viewNode == null) {
				throw new InvalidOperationException(string.Format("Cannot find '{0}' view.", currentVariantId));
			}
			if(viewNode.Variants == null) {
				throw new InvalidOperationException("viewNode.Variants is null.");
			}
			IModelVariant targetVariant = viewNode.Variants[currentVariantId];
			if(targetVariant == null) {
				throw new InvalidOperationException(string.Format("Cannot find '{0}' variant.", currentVariantId));
			}
			viewNode.Variants.Current = targetVariant;
		}
		public IModelList<IModelView> Views { get; private set; }
		[DefaultValue(AllowSingleVariantDefaultValue)]
		[Browsable(false)]
		public static bool AllowSingleVariant { get { return allowSingleVariant; } set { allowSingleVariant = value; } }
		#region Obsoleted since 15.1
		internal const string DefaultVariantObsoleteText = "Introduce a new ModelNodesGeneratorUpdater/ModelNodesGenerator instead."; 
		[Obsolete(DefaultVariantObsoleteText), EditorBrowsable(EditorBrowsableState.Never)] 
		[DefaultValue(GenerateDefaultVariantDefaultValue)]
		public static bool GenerateDefaultVariant {
			get { return generateDefaultVariant; }
			set { generateDefaultVariant = value; }
		}
		#endregion
	}
}
