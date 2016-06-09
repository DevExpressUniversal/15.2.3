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

using System.Collections.Generic;
using System.Drawing.Printing;
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.Web.ASPxSpreadsheet.Internal;
using DevExpress.Web.Internal;
using DevExpress.XtraSpreadsheet.Commands;
using DevExpress.Web.ASPxSpreadsheet.Internal.Commands;
namespace DevExpress.Web.ASPxSpreadsheet {
	public class SRPageSetupMarginsCommand : SRDropDownCommandBase {
		protected override SpreadsheetCommandId CommandID {
			get {
				return SpreadsheetCommandId.PageSetupMarginsCommandGroup;
			}
		}
		protected override bool DefaultDropDownMode {
			get {
				return false;
			}
		}
		protected override RibbonItemSize DefaultItemSize {
			get {
				return RibbonItemSize.Large;
			}
		}
		protected override void FillItems() {
			Items.Add(new SRPageSetupMarginsNormalCommand());
			Items.Add(new SRPageSetupMarginsWideCommand());
			Items.Add(new SRPageSetupMarginsNarrowCommand());
			Items.Add(new SRPageSetupMarginsCustomCommand());
		}
	}
	public class SRMarginsCommandBase : SRDropDownToggleCommandBase {
		protected ITemplate textItemTmplate = null;
		protected override ITemplate TextTemplate {
			get {
				if(textItemTmplate == null)
					textItemTmplate = new MarginTemplate(DefaultText);
				return textItemTmplate;
			}
			set {
				textItemTmplate = value;
			}
		}
		internal void ResetTemplateCaption(string defaultText) {
			if(TextTemplate != null) {
				((MarginTemplate)TextTemplate).ResetHierarchy(defaultText);
			}
		}
	}
	public class SRPageSetupMarginsNormalCommand : SRMarginsCommandBase {
		protected override SpreadsheetCommandId CommandID {
			get {
				return SpreadsheetCommandId.PageSetupMarginsNormal;
			}
		}
	}
	public class SRPageSetupMarginsWideCommand : SRMarginsCommandBase {
		protected override SpreadsheetCommandId CommandID {
			get {
				return SpreadsheetCommandId.PageSetupMarginsWide;
			}
		}
	}
	public class SRPageSetupMarginsNarrowCommand : SRMarginsCommandBase {
		protected override SpreadsheetCommandId CommandID {
			get {
				return SpreadsheetCommandId.PageSetupMarginsNarrow;
			}
		}
	}
	public class SRPageSetupMarginsCustomCommand : SRDropDownCommandBase {
		protected override SpreadsheetCommandId CommandID {
			get {
				return SpreadsheetCommandId.PageSetupCustomMargins;
			}
		}
		protected override WebSpreadsheetCommandID WebCommandID {
			get {
				return WebSpreadsheetCommandID.PageSetupWebCommand;
			}
		}
	}
	public class SRPageSetupOrientationCommand : SRDropDownCommandBase {
		protected override SpreadsheetCommandId CommandID {
			get {
				return SpreadsheetCommandId.PageSetupOrientationCommandGroup;
			}
		}
		protected override bool DefaultDropDownMode {
			get {
				return false;
			}
		}
		protected override RibbonItemSize DefaultItemSize {
			get {
				return RibbonItemSize.Large;
			}
		}
		protected override void FillItems() {
			Items.Add(new SRPageSetupOrientationPortraitCommand());
			Items.Add(new SRPageSetupOrientationLandscapeCommand());
		}
	}
	public class SRPageSetupOrientationPortraitCommand : SRDropDownToggleCommandBase {
		protected override SpreadsheetCommandId CommandID {
			get {
				return SpreadsheetCommandId.PageSetupOrientationPortrait;
			}
		}
	}
	public class SRPageSetupOrientationLandscapeCommand : SRDropDownToggleCommandBase {
		protected override SpreadsheetCommandId CommandID {
			get {
				return SpreadsheetCommandId.PageSetupOrientationLandscape;
			}
		}
	}
	public class SRPageSetupPaperKindCommand : SRDropDownCommandBase {
		protected override SpreadsheetCommandId CommandID {
			get {
				return SpreadsheetCommandId.PageSetupPaperKindCommandGroup;
			}
		}
		protected override bool DefaultDropDownMode {
			get {
				return false;
			}
		}
		protected override RibbonItemSize DefaultItemSize {
			get {
				return RibbonItemSize.Large;
			}
		}
		protected override void FillItems() {
			IList<PaperKind> defaultPaperKind = PageSetupSetPaperKindCommand.DefaultPaperKindList;
			foreach(PaperKind paperSize in defaultPaperKind) {
				Items.Add(new SRPagePaperKindBase(paperSize));
			}
		}
	}
	public class MarginTemplate : ITemplate {
		Control ContainerControl = null;
		protected string TemplateText { get; private set; }
		public MarginTemplate(string itemText) {
			TemplateText = itemText;
		}
		public void InstantiateIn(Control container) {
			ContainerControl = container;
			WebControl div = RenderUtils.CreateDiv();
			div.Style.Add("display", "inline-table");
			div.Width = Unit.Percentage(100);
			div.Controls.Add(new LiteralControl(SpreadsheetRibbonHelper.GetMarginItemCaption(TemplateText)));
			ContainerControl.Controls.Add(div);
		}
		public void ResetHierarchy(string itemText) {
			if(ContainerControl != null) {
				TemplateText = itemText;
				ContainerControl.Controls.Clear();
				InstantiateIn(ContainerControl);
			}
		}
	}
}
