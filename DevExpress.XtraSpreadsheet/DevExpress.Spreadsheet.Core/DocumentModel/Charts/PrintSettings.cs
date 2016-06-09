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
using DevExpress.Utils;
using DevExpress.Office;
using DevExpress.Office.Utils;
using DevExpress.XtraSpreadsheet.Utils;
namespace DevExpress.XtraSpreadsheet.Model {
	public class PrintSettings : ICloneable<PrintSettings>, ISupportsCopyFrom<PrintSettings> {
		#region Fields
		readonly DocumentModel documentModel;
		readonly HeaderFooterOptions headerFooter;
		readonly Margins pageMargins;
		readonly PrintSetup pageSetup;
		#endregion
		public PrintSettings(DocumentModel documentModel) {
			this.documentModel = documentModel;
			this.headerFooter = new HeaderFooterOptions(documentModel);
			this.pageMargins = new Margins(documentModel);
			this.pageSetup = new PrintSetup(documentModel);
		}
		#region Properties
		public HeaderFooterOptions HeaderFooter { get { return headerFooter; } }
		public Margins PageMargins { get { return pageMargins; } }
		public PrintSetup PageSetup { get { return pageSetup; } }
		#endregion
		#region ICloneable<PrintSettings> Members
		public PrintSettings Clone() {
			PrintSettings result = new PrintSettings(this.documentModel);
			result.CopyFrom(this);
			return result;
		}
		#endregion
		#region ISupportsCopyFrom<PrintSettings> Members
		public void CopyFrom(PrintSettings value) {
			Guard.ArgumentNotNull(value, "value");
			this.headerFooter.CopyFrom(value.headerFooter);
			this.pageMargins.CopyFrom(value.pageMargins);
			this.pageSetup.CopyFrom(value.pageSetup);
		}
		#endregion
	}
}
