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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace DevExpress.Xpf.Ribbon {
	internal class RibbonSelectionHelper {
		protected internal static RibbonPage SwitchPagesSelection(RibbonPage oldSelectedPage, int oldIndex) {
			if(oldSelectedPage.PageCategory == null || oldIndex == -1) return null;
			List<RibbonPage> availablePages = new List<RibbonPage>();
			oldIndex -= oldSelectedPage.PageCategory.ActualPagesCore.Count(i => i.IsVisible == false && oldSelectedPage.PageCategory.Pages.IndexOf(i) < oldIndex);
			if(oldSelectedPage.Ribbon == null) {
				availablePages.AddRange(oldSelectedPage.PageCategory.ActualPagesCore.Where(i => i.IsVisible == true));
			} else
				foreach(RibbonPageCategoryBase category in oldSelectedPage.Ribbon.ActualCategories) {
					availablePages.AddRange(category.ActualPagesCore.Where(i => i.IsVisible == true));
					oldIndex += oldSelectedPage.Ribbon.ActualCategories.IndexOf(category) < oldSelectedPage.Ribbon.ActualCategories.IndexOf(oldSelectedPage.PageCategory) ? category.ActualPagesCore.Count(i => i.IsVisible == true) : 0;
				}
			if(availablePages.Count == 0) return null;
			bool Contains = availablePages.Contains(oldSelectedPage);
			if(!Contains && availablePages.Count > oldIndex)
				return availablePages[oldIndex];
			if(Contains && availablePages.Count > oldIndex + 1)
				return availablePages[oldIndex + 1];
			if(oldIndex > 0)
				return availablePages[oldIndex - 1];
			return null;
		}
	}
}
