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
using System.Web.UI.WebControls;
using DevExpress.ExpressApp.Model;
using DevExpress.Web;
namespace DevExpress.ExpressApp.Web.Layout {
	public abstract class LayoutBaseTemplate : ISupportToolTip {
		private ControlInstantiationStrategyBase controlInstantiationStrategy;
		private void InstantiateInCallback(LayoutItemTemplateContainerBase container) {
			InstantiateInCore(container);
			OnInstantiated(container);
		}
		protected abstract void InstantiateInCore(LayoutItemTemplateContainerBase container);
		protected virtual void OnInstantiated(LayoutItemTemplateContainerBase container) {
			if(Instantiated != null) {
				Instantiated(this, new TemplateInstantiatedEventArgs(container));
			}
		}
		public LayoutBaseTemplate(ControlInstantiationStrategyBase controlInstantiationStrategy) {
			this.controlInstantiationStrategy = controlInstantiationStrategy;
		}
		public void InstantiateIn(LayoutItemTemplateContainerBase container) {
			controlInstantiationStrategy.InstantiateIn(container, InstantiateInCallback);
		}
		public event EventHandler<TemplateInstantiatedEventArgs> Instantiated;
		void ISupportToolTip.SetToolTip(object element, IModelToolTip toolTipModel) {
			string toolTip = toolTipModel != null ? toolTipModel.ToolTip : null;
			if(!string.IsNullOrEmpty(toolTip)) {
				WebControl layoutItemCaption = element as WebControl;
				if(layoutItemCaption != null) {
					RenderHelper.SetToolTip(layoutItemCaption, toolTip);
				}
				else {
					TabPage tabPage = element as TabPage;
					if(tabPage != null) {
						tabPage.ToolTip = toolTip;
					}
				}
			}
		}
	}
	public class TemplateInstantiatedEventArgs : EventArgs {
		LayoutItemTemplateContainerBase container;
		public TemplateInstantiatedEventArgs(LayoutItemTemplateContainerBase container) {
			this.container = container;
		}
		public LayoutItemTemplateContainerBase Container {
			get { return container; }
		}
	}
}
