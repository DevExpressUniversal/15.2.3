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
using System.ComponentModel;
using DevExpress.ExpressApp.Utils;
using DevExpress.XtraBars.Docking2010.Views;
using DevExpress.XtraBars.Ribbon;
namespace DevExpress.ExpressApp.Win.Templates.ActionContainers.Ribbon {
	public class RibbonDefaultSelectedPageHelper : IDisposable, IDisposableExt {
		private bool forceUpdateSelectedPageOnNextRibbonMerge;
		private BaseView view;
		private RibbonControl ribbonControl;
		void view_DocumentRemoved(object sender, DocumentEventArgs e) {
			if(IsDisposed) {
				return;
			}
			if(((BaseView)sender).Documents.Count == 0) {
				forceUpdateSelectedPageOnNextRibbonMerge = true;
			}
		}
		private void view_ControlShown(object sender, DeferredControlLoadEventArgs e) {
			forceUpdateSelectedPageOnNextRibbonMerge = true;
		}
		private void Ribbon_Merge(object sender, RibbonMergeEventArgs e) {
			if(IsDisposed) {
				return;
			}
			if(forceUpdateSelectedPageOnNextRibbonMerge) {
				RibbonControl ribbon = (RibbonControl)sender;
				CustomRibbonMergeEventArgs args = new CustomRibbonMergeEventArgs(ribbon, e);
				if(CustomRibbonMerge != null) {
					CustomRibbonMerge(this, args);
				}
				if(!args.Handled) {
					ribbon.SelectedPage = ribbon.Pages[DefaultSelectedPage];
					forceUpdateSelectedPageOnNextRibbonMerge = false;
				}
			}
		}
		public RibbonDefaultSelectedPageHelper(string defaultSelectedPage) {
			Guard.ArgumentNotNullOrEmpty(defaultSelectedPage, "defaultSelectedPage");
			this.DefaultSelectedPage = defaultSelectedPage;
		}
		public void Dispose() {
			if(ribbonControl != null) {
				ribbonControl.Merge -= Ribbon_Merge;
				ribbonControl = null;
			}
			if(view != null) {
				view = null;
			}
			IsDisposed = true;
		}
		public void Attach(BaseView view, RibbonControl ribbonControl) {
			Guard.NotDisposed(this);
			Guard.ArgumentNotNull(view, "view");
			Guard.ArgumentNotNull(ribbonControl, "ribbonControl");
			if(this.view != null) {
				throw new InvalidOperationException("view is initialized");
			}
			if(this.ribbonControl != null) {
				throw new InvalidOperationException("ribbonControl is initialized");
			}
			this.ribbonControl = ribbonControl;
			this.view = view;
			forceUpdateSelectedPageOnNextRibbonMerge = view.Documents.Count == 0;
			ribbonControl.Merge += Ribbon_Merge;
			view.DocumentRemoved += view_DocumentRemoved;
			view.ControlShown += view_ControlShown;
		}
		public bool IsDisposed { get; private set; }
		[DefaultValue(null)]
		public string DefaultSelectedPage { get; private set; }
		public event EventHandler<HandledEventArgs> CustomRibbonMerge;
	}
	public class CustomRibbonMergeEventArgs : HandledEventArgs {
		public CustomRibbonMergeEventArgs(RibbonControl ribbon, RibbonMergeEventArgs ribbonMergeEventArgs) {
			this.Ribbon = ribbon;
			this.RibbonMergeEventArgs = ribbonMergeEventArgs;
		}
		public RibbonControl Ribbon { get; private set; }
		public RibbonMergeEventArgs RibbonMergeEventArgs { get; private set; }
	}
}
