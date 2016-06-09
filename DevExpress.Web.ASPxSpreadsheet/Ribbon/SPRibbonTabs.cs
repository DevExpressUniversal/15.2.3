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

using DevExpress.Web.ASPxSpreadsheet.Internal;
using DevExpress.XtraSpreadsheet.Localization;
namespace DevExpress.Web.ASPxSpreadsheet {
	public class SRFileTab : SRTab {
		protected override string DefaultName {
			get {
				return "File";
			}
		}
		protected override string DefaultText {
			get {
				return XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_PageFile);
			}
		}
	}
	public class SRHomeTab : SRTab {
		protected override string DefaultName {
			get {
				return "Home";
			}
		}
		protected override string DefaultText {
			get {
				return XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_PageHome);
			}
		}
	}
	public class SRInsertTab : SRTab {
		protected override string DefaultName {
			get {
				return "Insert";
			}
		}
		protected override string DefaultText {
			get {
				return XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_PageInsert);
			}
		}
	}
	public class SRPageLayoutTab : SRTab {
		protected override string DefaultName {
			get {
				return "PageLayout";
			}
		}
		protected override string DefaultText {
			get {
				return XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_PagePageLayout);
			}
		}
	}
	public class SRFormulasTab : SRTab {
		protected override string DefaultName {
			get {
				return "Formulas";
			}
		}
		protected override string DefaultText {
			get {
				return XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_PageFormulas);
			}
		}
	}
	public class SRDataTab : SRTab {
		protected override string DefaultName {
			get {
				return "Data";
			}
		}
		protected override string DefaultText {
			get {
				return XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_PageData);
			}
		}
	}
	public class SRViewTab : SRTab {
		protected override string DefaultName {
			get {
				return "View";
			}
		}
		protected override string DefaultText {
			get {
				return XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_PageView);
			}
		}
	}
	public class SRTableDesignContextTab : SRTab {
		protected override string DefaultName {
			get {
				return "Design";
			}
		}
		protected override string DefaultText {
			get {
				return XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_TableToolsDesignPage);
			}
		}
	}
	public class SRPictureFormatContextTab : SRTab {
		protected override string DefaultName {
			get {
				return "Format";
			}
		}
		protected override string DefaultText {
			get {
				return XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_PageFormat);
			}
		}
	}
	public class SRChartDesignContextTab : SRTab {
		protected override string DefaultName {
			get {
				return "Design";
			}
		}
		protected override string DefaultText {
			get {
				return XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_PageChartsDesign);
			}
		}
	}
	public class SRChartLayoutContextTab : SRTab {
		protected override string DefaultName {
			get {
				return "Layout";
			}
		}
		protected override string DefaultText {
			get {
				return XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_PageChartsLayout);
			}
		}
	}
	public class SRChartFormatContextTab : SRTab {
		protected override string DefaultName {
			get {
				return "Format";
			}
		}
		protected override string DefaultText {
			get {
				return XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_PageFormat);
			}
		}
	}
}
