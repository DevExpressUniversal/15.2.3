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
using DevExpress.XtraPrinting.HtmlExport.Native;
namespace DevExpress.XtraPrinting.HtmlExport.Controls {
	public class DXHtmlAnchor : DXHtmlContainerControl {
		public DXHtmlAnchor()
			: base(DXHtmlTextWriterTag.A) {
		}
		public virtual bool CausesValidation {
			get {
				object causesValidation = ViewState["CausesValidation"];
				if(causesValidation != null)
					return (bool)causesValidation;
				return true;
			}
			set { ViewState["CausesValidation"] = value; }
		}
		public string HRef {
			get {
				string str = Attributes["href"];
				if(str == null)
					return string.Empty;
				return str;
			}
			set { Attributes["href"] = DXHtmlControl.MapStringAttributeToString(value); }
		}
		public string Name {
			get {
				string str = Attributes["name"];
				if(str == null)
					return string.Empty;
				return str;
			}
			set { Attributes["name"] = DXHtmlControl.MapStringAttributeToString(value); }
		}
		public string Target {
			get {
				string str = Attributes["target"];
				if(str == null)
					return string.Empty;
				return str;
			}
			set { Attributes["target"] = DXHtmlControl.MapStringAttributeToString(value); }
		}
		public string Title {
			get {
				string str = Attributes["title"];
				if(str == null)
					return string.Empty;
				return str;
			}
			set { Attributes["title"] = DXHtmlControl.MapStringAttributeToString(value); }
		}
		public virtual string ValidationGroup {
			get {
				string str = (string)ViewState["ValidationGroup"];
				if(str != null)
					return str;
				return string.Empty;
			}
			set { ViewState["ValidationGroup"] = value; }
		}
	}
}
