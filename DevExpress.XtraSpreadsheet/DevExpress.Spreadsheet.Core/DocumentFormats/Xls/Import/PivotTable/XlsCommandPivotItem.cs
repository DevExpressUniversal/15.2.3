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
	#region XlsCommandPivotItem -- SXVI --
	public class XlsCommandPivotItem : XlsCommandBase {
		#region Fields
		private enum ItemFlags {
			Hidden = 0,
			HideDetails = 1,
			CalculatedItems = 3,
			Missing = 4
		}
		int indexCacheItem;
		BitwiseContainer itemFlags = new BitwiseContainer(16);
		XLUnicodeStringNoCch itemName = new XLUnicodeStringNoCch();
		#endregion
		#region Properties
		public PivotFieldItemType PivotItemType { get; set; }
		public int IndexCacheItem {
			get { return indexCacheItem; }
			set {
				ValueChecker.CheckValue(value, -1, short.MaxValue, "IndexCachePivotItem");
				indexCacheItem = value;
			}
		}
		public string ItemName {
			get { return itemName.Value; }
			set {
				ValueChecker.CheckLength(value, 255, "ItemName");
				itemName.Value = value;
				HasReadName = true;
			}
		}
		public bool IsHidden {
			get { return itemFlags.GetBoolValue((int)ItemFlags.Hidden); }
			set { itemFlags.SetBoolValue((int)ItemFlags.Hidden, value); }
		}
		public bool IsHideDetails {
			get { return itemFlags.GetBoolValue((int)ItemFlags.HideDetails); }
			set { itemFlags.SetBoolValue((int)ItemFlags.HideDetails, value); }
		}
		public bool IsCalculatedItems {
			get { return itemFlags.GetBoolValue((int)ItemFlags.CalculatedItems); }
			set { itemFlags.SetBoolValue((int)ItemFlags.CalculatedItems, value); }
		}
		public bool IsMissing {
			get { return itemFlags.GetBoolValue((int)ItemFlags.Missing); }
			set { itemFlags.SetBoolValue((int)ItemFlags.Missing, value); }
		}
		bool HasReadName { get; set; }
		#endregion
		#region Methods
		protected override void ReadCore(XlsReader reader, XlsContentBuilder contentBuilder) {
			PivotItemType = (PivotFieldItemType)(1 << reader.ReadInt16());
			itemFlags.ShortContainer = reader.ReadInt16();
			IndexCacheItem = reader.ReadInt16();
			int lenItemName = reader.ReadUInt16();
			HasReadName = false;
			if (lenItemName != 0xFFFF) {
				itemName = XLUnicodeStringNoCch.FromStream(reader, lenItemName);
				HasReadName = true;
			}
		}
		protected override void ApplySheetContent(XlsContentBuilder contentBuilder) {
			XlsBuildPivotView builder = contentBuilder.CurrentBuilderPivotView;
			if (builder != null && builder.PivotField != null) {
				PivotItem pivotItem = new PivotItem(builder.PivotTable);
				pivotItem.SetItemTypeCore(PivotItemType);
				pivotItem.SetIsHiddenCore(IsHidden);
				pivotItem.SetHideDetailsCore(IsHideDetails);
				pivotItem.SetCalculatedMemberCore(IsCalculatedItems);
				pivotItem.SetHasMissingValueCore(IsMissing);
				pivotItem.SetItemIndexCore(IndexCacheItem);
				if (HasReadName)
					pivotItem.SetItemUserCaptionCore(ItemName);
				builder.PivotField.Items.AddCore(pivotItem);
			}
		}
		protected override void WriteCore(BinaryWriter writer) {
			int numType = 0;
			while (true) {
				if ((int)PivotItemType == (1 << numType))
					break;
				numType++;
			}
			writer.Write((short)numType);
			writer.Write((ushort)itemFlags.ShortContainer);
			writer.Write((short)IndexCacheItem);
			if (ItemName.Length != 0) {
				writer.Write((ushort)ItemName.Length);
				itemName.Write(writer);
			}
			else {
				writer.Write((ushort)0xFFFF);
			}
		}
		protected override short GetSize() {
			int result = 8;
			if (ItemName.Length > 0)
				result += itemName.Length;
			return (short)result;
		}
		public override IXlsCommand GetInstance() {
			return this;
		}
		#endregion
	}
	#endregion
}
