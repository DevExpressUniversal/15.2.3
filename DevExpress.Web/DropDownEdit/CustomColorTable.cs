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
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.Web;
namespace DevExpress.Web.Internal {
	public class CustomColorTable : ColorTable {
		protected internal const string CustomColorTableImagesResourcePath = "DevExpress.Web.Images.";
		protected internal const int DefaultCustomTableRowCount = 1;
		public CustomColorTable()
			: this(null, new ColorEditItemCollection()) {
		}
		protected internal CustomColorTable(ASPxWebControl ownerControl, ColorEditItemCollection colorTableCollection)
			: base(ownerControl, colorTableCollection) {
		}
		public override int RowCount { get { return DefaultCustomTableRowCount; } }
		protected override void CreateControlHierarchy() {
			Controls.Add(new CustomColorTableControl(this));
		}
		protected override void AddHoverItems(StateScriptRenderHelper helper) {
			AppearanceStyleBase style = GetTableCellHoverStyle();
			for(int i = 0; i < RowCount * ColumnCount; i++) {
				helper.AddStyle(style, GetItemCellID(i), IsEnabled());
			}
		}
		protected override string GetClientObjectClassName() {
			return "ASPx.CustomColorTable";
		}
	}
	public class CustomColorTableControl : ColorTableControl {
		public CustomColorTableControl(ItemPickerBase itemsTableContol)
			: base(itemsTableContol) {
		}
		protected override bool AllowRenderCell(int index) {
			return true;
		}
	}
}
