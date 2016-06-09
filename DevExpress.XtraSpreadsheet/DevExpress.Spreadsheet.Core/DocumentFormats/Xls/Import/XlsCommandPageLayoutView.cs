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
	public class XlsCommandPageLayoutView : XlsCommandBase {
		const short recordSize = 16;
		int zoomScale;
		#region Properties
		public int ZoomScale {
			get { return zoomScale; }
			set {
				ValueChecker.CheckValue(value, 0, ushort.MaxValue, "ZoomScale");
				zoomScale = value;
			}
		}
		public bool InPageLayoutView { get; set; }
		public bool RullerVisible { get; set; }
		public bool WhitespaceHidden { get; set; }
		#endregion
		protected override void ReadCore(XlsReader reader, XlsContentBuilder contentBuilder) {
			FutureRecordHeader.FromStream(reader);
			ZoomScale = reader.ReadUInt16();
			ushort bitwiseField = reader.ReadUInt16();
			InPageLayoutView = Convert.ToBoolean(bitwiseField & 0x0001);
			RullerVisible = Convert.ToBoolean(bitwiseField & 0x0002);
			WhitespaceHidden = Convert.ToBoolean(bitwiseField & 0x0004);
		}
		protected override void ApplySheetContent(XlsContentBuilder contentBuilder) {
			ModelWorksheetView view = contentBuilder.CurrentSheet.ActiveView;
			view.ZoomScalePageLayoutView = ZoomScale;
			if(InPageLayoutView)
				view.ViewType = SheetViewType.PageLayout;
			view.ShowRuler = RullerVisible;
			view.ShowWhiteSpace = !WhitespaceHidden;
		}
		protected override void WriteCore(BinaryWriter writer) {
			FutureRecordHeader header = new FutureRecordHeader();
			header.RecordTypeId = XlsCommandFactory.GetTypeCodeByType(typeof(XlsCommandPageLayoutView));
			header.Write(writer);
			writer.Write((ushort)ZoomScale);
			ushort bitwiseField = 0;
			if(InPageLayoutView) bitwiseField |= 0x0001;
			if(RullerVisible) bitwiseField |= 0x0002;
			if(WhitespaceHidden) bitwiseField |= 0x0004;
			writer.Write(bitwiseField);
		}
		protected override short GetSize() {
			return recordSize;
		}
		public override IXlsCommand GetInstance() {
			return this;
		}
	}
}
