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

extern alias Platform;
using System;
using Microsoft.Windows.Design.Model;
using Microsoft.Windows.Design.Services;
using Microsoft.Windows.Design.Features;
using Microsoft.Windows.Design;
using Platform::DevExpress.Xpf.Bars;
using Platform::DevExpress.Xpf.Ribbon;
using Microsoft.Windows.Design.Interaction;
using System.Windows;
namespace DevExpress.Xpf.Ribbon.Design {
	class RibbonControlDesignFeatureConnector : FeatureConnector<RibbonControlAdornerProviderBase> {
		public RibbonControlDesignFeatureConnector(FeatureManager manager)
			: base(manager) {
			Context.Services.Publish<RibbonControlDesignService>(RibbonControlDesignServiceFactory.CreateRibbonDesignService(Context));
		}
	}
	class RibbonControlDesignService {
		public EditingContext Context { get; private set; }
		public ModelService ModelService {
			get {
				if(service == null)
					service = Context.Services.GetService<ModelService>();
				return service;
			}
		}
		public RibbonControlDesignService(EditingContext context) {
			Context = context;
			Context.Items.Subscribe<Selection>(new SubscribeContextCallback<Selection>(OnSelectionChanged));
		}
		public void SelectRibbonPage(ModelItem page) {
			ModelItem ribbon = RibbonDesignTimeHelper.FindRibbonCotnrol(page);
			if(ribbon == null || page == null)
				return;
			var ribbonControl = ribbon.GetCurrentValue() as RibbonControl;
			if(ribbonControl != null) 
				ribbonControl.SelectedPage = page.GetCurrentValue() as RibbonPage;
		}
		public void SelectFirstRibbonPageCategory(ModelItem ribbonControl) {
			if(ribbonControl.Properties["Items"].Collection.Count == 0) return;
			ModelItem firstCategory = ribbonControl.Properties["Items"].Collection[0];
			SelectionOperations.SelectOnly(Context, firstCategory);
		}
		protected virtual void OnSelectionChanged(Selection newSelection) {
			ModelItem page = RibbonDesignTimeHelper.FindRibbonPage(newSelection.PrimarySelection);
			if(page == null) return;
			SelectRibbonPage(page);
		}
		ModelService service;
	}
	static class RibbonControlDesignServiceFactory {
		public static RibbonControlDesignService CreateRibbonDesignService(EditingContext context) {
			return new RibbonControlDesignService(context);
		}
	}
}
