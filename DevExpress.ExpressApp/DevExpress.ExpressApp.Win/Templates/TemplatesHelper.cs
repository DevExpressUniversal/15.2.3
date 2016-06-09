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
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Win.SystemModule;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.XtraBars.Ribbon;
namespace DevExpress.ExpressApp.Win.Templates {
	public class TemplatesHelper {
		private IModelTemplateWin modelTemplate;
		public TemplatesHelper(IModelTemplateWin modelTemplate) {
			Guard.ArgumentNotNull(modelTemplate, "modelTemplate");
			this.modelTemplate = modelTemplate;
		}
		public IModelLocalizationGroup GetLocalizationNode() {
			string templateName = ((ModelNode)modelTemplate).Id;
			return (IModelLocalizationGroup)((IModelLocalizationGroup)(modelTemplate.Application.Localization["FrameTemplates"]))[templateName];
		}
		public IModelTemplateXtraBarsCustomization GetBarsCustomizationNode() {
			return GetBarsCustomizationNode(String.Empty);
		}
		public IModelTemplateXtraBarsCustomization GetBarsCustomizationNode(string viewId) {
			string nodeId = string.IsNullOrEmpty(viewId) ? XtraBarsCustomizationNodeGenerator.DefaultCustomizationId : viewId + "_BarsCustomization";
			((ModelNode)modelTemplate.XtraBarsCustomizations).RunCustomGenerator(new XtraBarsCustomizationNodeGenerator(nodeId));
			return modelTemplate.XtraBarsCustomizations[nodeId];
		}
		public IModelTemplateNavBarCustomization GetNavBarCustomizationNode() {
			return modelTemplate.NavBarCustomization;
		}
		public IModelFormState GetFormStateNode() {
			return GetFormStateNode(String.Empty);
		}
		public IModelFormState GetFormStateNode(string viewId) {
			string nodeId = string.IsNullOrEmpty(viewId) ? XtraBarsCustomizationNodeGenerator.DefaultCustomizationId : viewId;
			IModelFormState result = modelTemplate.FormStates[nodeId];
			if(result == null) {
				result = modelTemplate.FormStates.AddNode<IModelFormState>(nodeId);
			}
			return result;
		}
		public void SetRibbonSettings(RibbonControl ribbon) {
			IModelOptionsWin optionsWin = modelTemplate.Application.Options as IModelOptionsWin;
			if(optionsWin != null) {
				IModelOptionsRibbon ribbonOptions = optionsWin.RibbonOptions;
				ribbon.RibbonStyle = ribbonOptions.RibbonControlStyle;
				ribbon.Minimized = ribbonOptions.MinimizeRibbon;
				ribbon.MinimizedChanged += (s, e) => {
					ribbonOptions.MinimizeRibbon = ribbon.Minimized;
				};
			}
		}
	}
}
