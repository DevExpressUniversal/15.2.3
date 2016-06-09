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
using System.IO;
using System.Text;
using DevExpress.Office;
using DevExpress.Utils;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.XtraSpreadsheet.Utils;
using DevExpress.Office.Utils;
using DevExpress.XtraSpreadsheet.Internal;
using DevExpress.XtraExport.Xls;
namespace DevExpress.XtraSpreadsheet.Import.Xls {
	#region XlsCommandPivotDifferentialFormat  -- SxDXF --
	public class XlsCommandPivotDifferentialFormat : XlsCommandBase {
		#region Fields
		DxfN12ListInfo dfx;
		#endregion
		#region Properties
		public DxfN12ListInfo DifferentialFormat {
			get { return dfx; }
			set { dfx = value; }
		}
		#endregion
		protected override void ReadCore(XlsReader reader, XlsContentBuilder contentBuilder) {
			if (contentBuilder.CurrentBuilderPivotView != null) {
				using (XlsCommandStream extStream = new XlsCommandStream(reader, Size)) {
					using (BinaryReader binaryReader = new BinaryReader(extStream)) {
						dfx = DxfN12ListInfo.FromStream(binaryReader, contentBuilder.CurrentBuilderPivotView.DifferentialFormatLength);
					}
				}
			}
		}
		protected override void ApplySheetContent(XlsContentBuilder contentBuilder) {
			XlsBuildPivotView builder = contentBuilder.CurrentBuilderPivotView;
			if (builder != null && builder.IsPivotFormat) {
				DifferentialFormat differentialFormat = DifferentialFormat.GetDifferentialFormat(contentBuilder);
				int index = contentBuilder.DocumentModel.Cache.CellFormatCache.AddItem(differentialFormat);
				builder.Format.SetIndexInitial(index);
			}
		}
		protected override void WriteCore(BinaryWriter writer) {
			if (dfx != null && GetSize() > 0)
				dfx.Write(writer);
		}
		protected override short GetSize() {
			return dfx.GetSize();
		}
		public override IXlsCommand GetInstance() {
			return this;
		}
	}
	#endregion
}
