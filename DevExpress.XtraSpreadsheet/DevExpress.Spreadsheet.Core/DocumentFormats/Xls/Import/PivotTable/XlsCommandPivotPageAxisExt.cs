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
	#region XlsCommandPivotPageAxisExt  -- SXPIEx --
	public class XlsCommandPivotPageAxisExt : XlsCommandBase {
		#region Fields
		XLUnicodeString uniqueName = new XLUnicodeString();
		XLUnicodeString displayName = new XLUnicodeString();
		#endregion
		#region Properties
		public int OldHeader {
			get { return 0x080E; }
			set {
				if (value != 0x080E)
					throw new ArgumentException("The frtHeaderOld.rt field MUST be 0x080E");
			}
		}
		public int HierarchyIndex { get; set; }
		public string UniqueName {
			get { return uniqueName.Value; }
			set {
				ValueChecker.CheckLength(value, 255, "UniqueName");
				uniqueName.Value = value;
			}
		}
		public string DisplayName {
			get { return displayName.Value; }
			set {
				ValueChecker.CheckLength(value, 255, "DisplayName");
				displayName.Value = value;
			}
		}
		#endregion
		protected override void ReadCore(XlsReader reader, XlsContentBuilder contentBuilder) {
			OldHeader = reader.ReadInt32();
			HierarchyIndex = (int)reader.ReadUInt32();
			uniqueName = XLUnicodeString.FromStream(reader);
			displayName = XLUnicodeString.FromStream(reader);
		}
		protected override void ApplySheetContent(XlsContentBuilder contentBuilder) {
			XlsBuildPivotView builder = contentBuilder.CurrentBuilderPivotView;
			if (builder != null && builder.PivotTable != null && builder.PivotTable.PageFields.Count >= (builder.CurrentIndexPageAxis+1)) {
				PivotPageField page = builder.PivotTable.PageFields[builder.CurrentIndexPageAxis];
				page.SetHierarchyIndexCore(HierarchyIndex);
				page.SetHierarchyUniqueNameCore(UniqueName);
				page.SetHierarchyDisplayNameCore(DisplayName);
				builder.CurrentIndexPageAxis += 1;
			}
		}
		protected override void WriteCore(BinaryWriter writer) {
			writer.Write(OldHeader);
			writer.Write((uint)HierarchyIndex);
			uniqueName.Write(writer);
			displayName.Write(writer);
		}
		protected override short GetSize() {
			return (short)(8 + uniqueName.Length + displayName.Length);
		}
		public override IXlsCommand GetInstance() {
			return this;
		}
	}
	#endregion
}
