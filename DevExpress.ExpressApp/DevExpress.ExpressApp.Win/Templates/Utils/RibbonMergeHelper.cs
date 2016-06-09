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

using DevExpress.Utils;
using DevExpress.XtraBars;
using DevExpress.XtraBars.Ribbon;
namespace DevExpress.ExpressApp.Win.Templates.Utils {
	public sealed class RibbonMergeHelper {
		private void Ribbon_Merge(object sender, RibbonMergeEventArgs e) {
			RibbonControl ownerRibbon = e.MergeOwner;
			RibbonControl childRibbon = e.MergedChild;
			if(ownerRibbon.StatusBar != null && childRibbon.StatusBar != null) {
				ownerRibbon.StatusBar.MergeStatusBar(childRibbon.StatusBar);
			}
			PopupMenu ownerApplicationMenu = ownerRibbon.ApplicationButtonDropDownControl as PopupMenu;
			PopupMenu childApplicationMenu = childRibbon.ApplicationButtonDropDownControl as PopupMenu;
			if(ownerApplicationMenu != null && childApplicationMenu != null) {
				ownerApplicationMenu.Merge(childApplicationMenu);
			}
		}
		private void Ribbon_UnMerge(object sender, RibbonMergeEventArgs e) {
			RibbonControl ownerRibbon = e.MergeOwner;
			if(ownerRibbon.StatusBar != null) {
				ownerRibbon.StatusBar.UnMergeStatusBar();
			}
			PopupMenu ownerApplicationMenu = ownerRibbon.ApplicationButtonDropDownControl as PopupMenu;
			if(ownerApplicationMenu != null) {
				ownerApplicationMenu.UnMerge();
			}
		}
		public void Attach(RibbonControl ribbon) {
			Guard.ArgumentNotNull(ribbon, "ribbon");
			ribbon.Merge += Ribbon_Merge;
			ribbon.UnMerge += Ribbon_UnMerge;
		}
	}
}
