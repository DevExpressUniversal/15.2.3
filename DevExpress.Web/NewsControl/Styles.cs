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
using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.Web.Internal;
namespace DevExpress.Web {
	using DevExpress.Web.Internal;
	public class NewsControlStyles : DataViewStyles {
		private HeadlineContentStyle itemContent = new HeadlineContentStyle();
		private HeadlineStyle itemHeader = new HeadlineStyle();
		private HeadlineStyle itemTail = new HeadlineStyle();
		private BackToTopStyle backToTop = new BackToTopStyle();
		private HeadlineDateStyle itemDate = new HeadlineDateStyle();
		private HeadlinePanelStyle itemLeftPanel = new HeadlinePanelStyle();
		private HeadlinePanelStyle itemRightPanel = new HeadlinePanelStyle();
		public NewsControlStyles(ASPxNewsControl newsControl)
			: base(newsControl) {
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("NewsControlStylesBackToTop"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true)]
		public BackToTopStyle BackToTop {
			get { return backToTop; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("NewsControlStylesItemDate"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true)]
		public HeadlineDateStyle ItemDate {
			get { return itemDate; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("NewsControlStylesItemContent"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true)]
		public HeadlineContentStyle ItemContent {
			get { return itemContent; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("NewsControlStylesItemHeader"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true)]
		public HeadlineStyle ItemHeader {
			get { return itemHeader; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("NewsControlStylesItemTail"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true)]
		public HeadlineStyle ItemTail {
			get { return itemTail; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("NewsControlStylesItemLeftPanel"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true)]
		public HeadlinePanelStyle ItemLeftPanel {
			get { return itemLeftPanel; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("NewsControlStylesItemRightPanel"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true)]
		public HeadlinePanelStyle ItemRightPanel {
			get { return itemRightPanel; }
		}
		protected internal override string GetCssClassNamePrefix() {
			return "dxnc";
		}
		protected internal virtual BackToTopStyle GetDefaultBackToTopStyle() {
			BackToTopStyle style = new BackToTopStyle();
			style.CopyFrom(CreateStyleByName("BackToTopStyle"));
			style.Spacing = GetBackToTopSpacing();
			style.ImageSpacing = GetBackToTopImageSpacing();
			return style;
		}
		protected virtual Unit GetBackToTopSpacing() {
			return 8;
		}
		protected virtual Unit GetBackToTopImageSpacing() {
			return GetImageSpacing();
		}
		protected override Unit GetItemSpacing() {
			return 0;
		}
		protected override Unit GetItemWidth() {
			return Unit.Empty;
		}
		protected override IStateManager[] GetStateManagedObjects() {
			return ViewStateUtils.GetMergedStateManagedObjects(base.GetStateManagedObjects(),
				new IStateManager[] { BackToTop, ItemDate, ItemContent, ItemHeader, 
				ItemTail, ItemLeftPanel, ItemRightPanel });
		}
	}
	public class NCHeadlineStyles : HeadlineStyles {
		public NCHeadlineStyles(NCHeadline headline)
			: base(headline) {
		}
		protected internal override AppearanceStyleBase GetDefaultDisabledStyle() {
			return new AppearanceStyleBase();
		}
	}
}
