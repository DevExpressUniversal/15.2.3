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
	#region XlsCommandPivotFilter  -- SxFilt --
	public class XlsCommandPivotFilter : XlsCommandBase {
		#region Fields
		private enum FilterFirst {
			AxisType = 0,   
			PivotFieldAxis = 2 
		}
		private enum FilterSecond {
			FieldIndex = 0, 
			Selected = 1, 
		}
		BitwiseContainer first = new BitwiseContainer(16, new int[] { 4, 2, 10 });
		BitwiseContainer second = new BitwiseContainer(16, new int[] { 10 });
		#endregion
		#region Properties
		public PivotTableAxis Axis {
			get { return (PivotTableAxis)first.GetIntValue((int)FilterFirst.AxisType); }
			set { first.SetIntValue((int)FilterFirst.AxisType, (int)value); }
		}
		public int PivotFieldAxis {
			get { return first.GetIntValue((int)FilterFirst.PivotFieldAxis); }
			set {
				ValueChecker.CheckValue(value, 0, 31, "PivotFieldAxis");
				first.SetIntValue((int)FilterFirst.PivotFieldAxis, value);
			}
		}
		public int FieldIndex {
			get {
				int result = second.GetIntValue((int)FilterSecond.FieldIndex);
				if ((result & 0x200) > 0)
					result = -2;
				return result;
			}
			set {
				ValueChecker.CheckValue(value, -2, 255, "FilterRefers");
				second.SetIntValue((int)FilterSecond.FieldIndex, value);
			}
		}
		public bool IsSelected {
			get { return second.GetBoolValue((int)FilterSecond.Selected); }
			set { second.SetBoolValue((int)FilterSecond.Selected, value); }
		}
		public PivotFieldItemType FilterSubtotal { get; set; }
		public int CountItem { get; set; }
		#endregion
		protected override void ReadCore(XlsReader reader, XlsContentBuilder contentBuilder) {
			first.IntContainer = (int)reader.ReadUInt16();
			second.ShortContainer = (short)reader.ReadInt16();
			FilterSubtotal = (PivotFieldItemType)reader.ReadInt16();
			CountItem = reader.ReadUInt16();
		}
		protected override void ApplySheetContent(XlsContentBuilder contentBuilder) {
			XlsBuildPivotView builder = contentBuilder.CurrentBuilderPivotView;
			if (builder != null) {
				builder.FilterCountItem = CountItem;
				PivotAreaReferenceCollection references = null;
				builder.Reference = null;
				if (builder.IsPivotSelection)
					references = contentBuilder.CurrentSheet.PivotSelection.PivotArea.References;
				if (builder.IsPivotFormat)
					references = builder.Format.PivotArea.References;
				PivotAreaReference reference = new PivotAreaReference(contentBuilder.CurrentSheet.Workbook);
				reference.Subtotal = FilterSubtotal;
				reference.IsSelected = IsSelected;
				reference.Field = FieldIndex;
				reference.IsByPosition = PivotFieldAxis > 0;
				builder.Reference = reference;
				if (references != null)
					references.AddCore(reference);
			}
		}
		protected override void ApplyPivotCacheContent(XlsContentBuilder contentBuilder) {
			if (contentBuilder == null)
				return;
			DocumentModel documentModel = contentBuilder.DocumentModel;
			PivotCacheCalculatedItemCollection collection = documentModel.PivotCaches.Last.CalculatedItems;
			int count = collection.Count;
			PivotAreaReferenceCollection referenceCollection = collection[count - 1].PivotArea.References;
			PivotAreaReference reference = new PivotAreaReference(documentModel);
			reference.Subtotal = FilterSubtotal;
			reference.IsSelected = IsSelected;
			reference.Field = FieldIndex;
			referenceCollection.Add(reference);
			contentBuilder.CurrentPivotCacheBuilder.FilterCountItem = CountItem;
		}
		protected override void WriteCore(BinaryWriter writer) {
			writer.Write((ushort)first.IntContainer);
			writer.Write((ushort)second.ShortContainer);
			writer.Write((short)FilterSubtotal);
			writer.Write((ushort)CountItem);
		}
		protected override short GetSize() {
			return (short)8;
		}
		public override IXlsCommand GetInstance() {
			return this;
		}
	}
	#endregion
}
