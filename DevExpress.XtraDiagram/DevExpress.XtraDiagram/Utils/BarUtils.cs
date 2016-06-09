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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevExpress.Utils;
using DevExpress.XtraBars;
using DevExpress.XtraBars.Ribbon;
using DevExpress.XtraDiagram.Base;
using DevExpress.XtraEditors.Repository;
namespace DevExpress.XtraDiagram.Utils {
	public class BarUtils {
		public static RibbonControl CreateSimpleRibbon() {
			RibbonControl ribbonControl = new RibbonControl();
			ribbonControl.BeginInit();
			try {
				ribbonControl.RibbonStyle = RibbonControlStyle.Office2013;
				RibbonPage page = CreateRibbonPage(ribbonControl, "Page");
				RibbonPageGroup group = CreateRibbonPageGroup(ribbonControl, page, "Group");
				SkinRibbonGalleryBarItem skinGallery = new SkinRibbonGalleryBarItem();
				ribbonControl.Items.Add(skinGallery);
				group.ItemLinks.Add(skinGallery);
			}
			finally {
				ribbonControl.EndInit();
			}
			return ribbonControl;
		}
		public static RibbonPage CreateRibbonPage(RibbonControl ribbon, string text) {
			RibbonPage page = new RibbonPage(text);
			ribbon.BeginInit();
			try {
				ribbon.Pages.Add(page);
			}
			finally {
				ribbon.EndInit();
			}
			return page;
		}
		public static RibbonPageGroup CreateRibbonPageGroup(RibbonControl ribbon, string pageText, string text, bool selectPage = false) {
			RibbonPage page = ribbon.Pages.GetPageByText(pageText);
			if(page == null) {
				throw new ArgumentException(pageText);
			}
			if(selectPage) ribbon.SelectedPage = page;
			return CreateRibbonPageGroup(ribbon, page, text);
		}
		public static RibbonPageGroup CreateRibbonPageGroup(RibbonControl ribbon, RibbonPage page, string text) {
			RibbonPageGroup group = new RibbonPageGroup(text);
			ribbon.BeginInit();
			try {
				page.Groups.Add(group);
			}
			finally {
				ribbon.EndInit();
			}
			return group;
		}
		public static SuperToolTip CreateTooltip(string value) {
			SuperToolTip superToolTip = new SuperToolTip();
			ToolTipTitleItem item = new ToolTipTitleItem();
			item.Text = value;
			superToolTip.Items.Add(item);
			return superToolTip;
		}
		public static void ForEachRibbonPageGroup(RibbonControl ribbon, Action<RibbonPageGroup> action) {
			foreach(RibbonPage page in ribbon.Pages) {
				foreach(RibbonPageGroup group in page.Groups) {
					action(group);
				}
			}
		}
	}
	public class BarEditPair<T> : Pair<T, BarEditItem> where T : RepositoryItem {
		public BarEditPair(T item, BarEditItem barItem)
			: base(item, barItem) {
		}
		public T Item { get { return First; } }
		public BarEditItem BarItem { get { return Second; } }
	}
}
