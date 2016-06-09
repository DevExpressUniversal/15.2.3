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
using System.ComponentModel;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.Web;
namespace DevExpress.Web.Internal {
	public class NewsItemContentInfo : ItemContentInfo {
		public NCHeadline Headline = null;
		public BackToTopControl BackToTopControl = null;
	}
	[ToolboxItem(false)]
	public class NCPager : DVPager {
		public NCPager(ASPxNewsControl newsControl)
			: base(newsControl) {
		}
		protected override PagerSettingsEx CreatePagerSettings(ASPxPagerBase pager) {
			return new NewsControlPagerSettings(pager);
		}
	}
	[ToolboxItem(false)]
	public class NCHeadline : DevExpress.Web.ASPxHeadline {
		private string headlineTailOnClick = "ASPx.HLTClick('{0}')";
		private ASPxNewsControl newsControl = null;
		public string HeadlineTailOnClick {
			get { return this.headlineTailOnClick; }
			set { this.headlineTailOnClick = value; }
		}
		public NCHeadline(ASPxNewsControl newsControl)
			: base() {
			this.newsControl = newsControl;
			EnableViewState = false;
			ParentSkinOwner = NewsControl;
		}
		protected ASPxNewsControl NewsControl {
			get { return newsControl; }
		}
		protected override bool HasContent() {
			return true;
		}
		protected internal override bool IsTailRequired() {
			return TailHasLink() || NewsControl.HasTailOnClick();
		}
		protected internal override string GetTailOnClick() {
			return HeadlineTailOnClick;
		}
	}
}
