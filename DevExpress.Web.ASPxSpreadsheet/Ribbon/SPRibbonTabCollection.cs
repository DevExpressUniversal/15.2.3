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

using System.ComponentModel;
using DevExpress.Web.Internal;
using DevExpress.Web.ASPxSpreadsheet.Internal;
namespace DevExpress.Web.ASPxSpreadsheet {
	[Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.Drawing.Design.UITypeEditor))]
	public class SpreadsheetRibbonTabCollection : Collection<RibbonTab> {		
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public ASPxSpreadsheet Spreadsheet { get { return Owner as ASPxSpreadsheet; } }
		public SpreadsheetRibbonTabCollection(IWebControlObject owner)
			: base(owner) {
		}
		public RibbonTab Add(string text) {
			RibbonTab tab = new RibbonTab(text);
			Add(tab);
			return tab;
		}
		protected internal void CreateDefaultRibbonTabs() {
			AddRange(new SpreadsheetDefaultRibbon(Spreadsheet).DefaultRibbonTabs);
		}
		protected override void OnChanged() {
			if(Owner != null)
				Owner.LayoutChanged();
			base.OnChanged();
		}
	}
	[Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.Drawing.Design.UITypeEditor))]
	public class SpreadsheetRibbonContextTabCategoryCollection : Collection<RibbonContextTabCategory> {
		public ASPxSpreadsheet Spreadsheet { get { return Owner as ASPxSpreadsheet; } }
		public SpreadsheetRibbonContextTabCategoryCollection(IWebControlObject owner)
			: base(owner) {
		}
		public RibbonContextTabCategory Add(string name) {
			RibbonContextTabCategory tabCategory = new RibbonContextTabCategory(name);
			Add(tabCategory);
			return tabCategory;
		}
		protected internal void CreateDefaultRibbonContextTabCategories() {
			AddRange(new SpreadsheetDefaultRibbon(Spreadsheet).DefaultRibbonContextTabCategories);
		}
		protected override void OnChanged() {
			if(Owner != null)
				Owner.LayoutChanged();
			base.OnChanged();
		}
	}
}
