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
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.DC;
using System.ComponentModel;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.Model;
namespace DevExpress.ExpressApp.Win.SystemModule {
	public interface IHtmlFormattingSupport {
		void SetHtmlFormattingEnabled(bool htmlFormattingEnabled);
	}
	public class HtmlFormattingController : ViewController, IModelExtender {
		private bool isHtmlFormattingEnabled;
		private void View_InfoChanged(object sender, EventArgs e) {
			SetHtmlFormattingEnabled();
		}
		private void SetHtmlFormattingEnabled() {
			isHtmlFormattingEnabled = ((IModelOptionsEnableHtmlFormatting)View.Model.Application.Options).EnableHtmlFormatting;
			if(View is ListView) {
				ListView listView = (ListView)View;
				IHtmlFormattingSupport htmlFormattingSupport = listView.Editor as IHtmlFormattingSupport;
				if(htmlFormattingSupport != null) {
					htmlFormattingSupport.SetHtmlFormattingEnabled(IsHtmlFormattingEnabled);
				}
			}
			else {
				CompositeView compositeView = (CompositeView)View;
				foreach(IHtmlFormattingSupport htmlFormattingSupport in compositeView.GetItems<IHtmlFormattingSupport>()) {
					htmlFormattingSupport.SetHtmlFormattingEnabled(IsHtmlFormattingEnabled);
				}
				IHtmlFormattingSupport layoutManagerHtmlFormattingSupport = compositeView.LayoutManager as IHtmlFormattingSupport;
				if(layoutManagerHtmlFormattingSupport != null) {
					layoutManagerHtmlFormattingSupport.SetHtmlFormattingEnabled(IsHtmlFormattingEnabled);
				}
			}
		}
		protected override void OnActivated() {
			View.ModelChanged += new EventHandler(View_InfoChanged);
			SetHtmlFormattingEnabled();
		}
		protected override void OnDeactivated() {
			View.ModelChanged -= new EventHandler(View_InfoChanged);
			base.OnDeactivated();
		}
		public bool IsHtmlFormattingEnabled {
			get {
				return isHtmlFormattingEnabled;
			}
		}
		public HtmlFormattingController()
			: base() {
			TypeOfView = typeof(CompositeView);
		}
		#region IExtendModel Members
		void IModelExtender.ExtendModelInterfaces(ModelInterfaceExtenders extenders) {
			extenders.Add<IModelOptions, IModelOptionsEnableHtmlFormatting>();
		}
		#endregion
	}
	public interface IModelOptionsEnableHtmlFormatting {
		[
#if !SL
	DevExpressExpressAppWinLocalizedDescription("IModelOptionsEnableHtmlFormattingEnableHtmlFormatting"),
#endif
 DefaultValue(false)]
		[Category("Appearance")]
		bool EnableHtmlFormatting { get; set; }
	}
}
