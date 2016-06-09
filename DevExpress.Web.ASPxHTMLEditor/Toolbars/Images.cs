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
using DevExpress.Web;
using DevExpress.Web.Internal;
using System.Web.UI;
namespace DevExpress.Web.ASPxHtmlEditor {
	public class ToolbarItemImageProperties : DevExpress.Web.MenuItemImageProperties {
		public ToolbarItemImageProperties() : base() { }
		public ToolbarItemImageProperties(IPropertiesOwner owner) : base(owner) { }
		public ToolbarItemImageProperties(string url) : base(url) { }
		protected new string UrlChecked {
			get { return base.UrlChecked; }
			set { base.UrlChecked = value; }
		}
		protected new string UrlHottracked {
			get { return base.UrlHottracked; }
			set { base.UrlHottracked = value; }
		}
		protected new string UrlPressed {
			get { return base.UrlPressed; }
			set { base.UrlPressed = value; }
		}
		protected new string UrlSelected {
			get { return base.UrlSelected; }
			set { base.UrlSelected = value; }
		}
	}
	public class HtmlEditorRibbonItemImageProperties : DevExpress.Web.RibbonItemImageProperties {
		public HtmlEditorRibbonItemImageProperties(IPropertiesOwner owner) : base(owner) { }
		protected new string UrlChecked {
			get { return base.UrlChecked; }
			set { base.UrlChecked = value; }
		}
		protected new string UrlHottracked {
			get { return base.UrlHottracked; }
			set { base.UrlHottracked = value; }
		}
		protected new string UrlPressed {
			get { return base.UrlPressed; }
			set { base.UrlPressed = value; }
		}
		protected new string UrlSelected {
			get { return base.UrlSelected; }
			set { base.UrlSelected = value; }
		}
	}
}
