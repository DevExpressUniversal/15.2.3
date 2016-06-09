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
using DevExpress.XtraBars;
using System.ComponentModel;
namespace DevExpress.ExpressApp.Win.Templates {
	public class MainMenuItem : BarSubItem {
		private bool visibleInRibbon = true;
		private bool IsVisible(BarItem item) {
			BarLinksHolder linksHolder = item as BarLinksHolder;
			if(linksHolder != null) {
				foreach(BarItemLink link in linksHolder.ItemLinks) {
					if(IsVisible(link.Item))
						return true;
				}
				return false;
			}
			return item.Visibility == BarItemVisibility.Always || item.Visibility == BarItemVisibility.OnlyInRuntime;
		}
		private void UpdateVisibility() {
			if(!DesignMode) {
				Visibility = IsVisible(this) ? BarItemVisibility.Always : BarItemVisibility.OnlyInCustomizing;
			}
		}
		private void Manager_EndCustomization(object sender, EventArgs e) {
			UpdateVisibility();
		}
		private void Manager_Merge(object sender, BarManagerMergeEventArgs e) {
			UpdateVisibility();
		}
		private void Manager_UnMerge(object sender, BarManagerMergeEventArgs e) {
			UpdateVisibility();
		}
		protected override void OnManagerChanged() {
			base.OnManagerChanged();
			if(Manager != null) {
				Manager.EndCustomization += new EventHandler(Manager_EndCustomization);
				Manager.Merge += new BarManagerMergeEventHandler(Manager_Merge);
				Manager.UnMerge += new BarManagerMergeEventHandler(Manager_UnMerge);
			}
		}
		protected override void Dispose(bool disposing) {
			try {
				if(Manager != null) {
					Manager.EndCustomization -= new EventHandler(Manager_EndCustomization);
					Manager.Merge -= new BarManagerMergeEventHandler(Manager_Merge);
					Manager.UnMerge -= new BarManagerMergeEventHandler(Manager_UnMerge);
				}
			}
			finally {
				base.Dispose(disposing);
			}
		}
		public override void EndUpdate() {
			UpdateVisibility();
			base.EndUpdate();
		}
		[DefaultValue(true), Category("Ribbon")]
		public bool VisibleInRibbon {
			get { return visibleInRibbon; }
			set {
				visibleInRibbon = value;
			}
		}
	}
}
