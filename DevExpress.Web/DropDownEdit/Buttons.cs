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
using System.Text;
using System.Web.UI;
using System.Web;
using System.Web.UI.WebControls;
using System.ComponentModel;
using DevExpress.Web;
using DevExpress.Web.Internal;
namespace DevExpress.Web {
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class DropDownButton : EditButton {
		private IWebControlObject owner = null;
		public DropDownButton()
			: base() {
		}
		public DropDownButton(string text)
			: base(text) {
		}
		public DropDownButton(IWebControlObject owner)
			: base() {
			this.owner = owner;
		}
		protected internal override ImageProperties GetDefaultImage(Page page, EditorImages images, bool rtl) {
			return images.GetImageProperties(page, EditorImages.DropDownEditDropDownImageName);
		}
		protected override bool IsDesignMode() {
			if(this.owner != null)
				return this.owner.IsDesignMode();
			return false;
		}
		protected override bool IsLoading() {
			if(this.owner != null)
				return this.owner.IsLoading();
			return false;
		}
		protected override void LayoutChanged() {
			if(this.owner != null)
				this.owner.LayoutChanged();
		}
		protected override void TemplatesChanged() {
			if(this.owner != null)
				this.owner.TemplatesChanged();
		}
		public override string ToString() {
			return Text;
		}
	}
}
