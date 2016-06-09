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
	#region XlsCommandPivotFieldOLAPExt  -- SXVDTEx --
	public class XlsCommandPivotFieldOLAPExt : XlsCommandRecordBase {
		#region Fields
		static short[] typeCodes = new short[] {
			0x0812  
		};
		XlsPivotFieldOLAPExtReWriter pivotOlap;
		#endregion
		#region Properties
		public XlsPivotFieldOLAPExtReWriter PivotFieldOLAP { get { return pivotOlap; } }
		#endregion
		#region Methods
		protected override void ReadCore(XlsReader reader, XlsContentBuilder contentBuilder) {
			if (contentBuilder.ContentType == XlsContentType.Sheet) {
				FutureRecordHeaderOld header = FutureRecordHeaderOld.FromStream(reader);
				using (XlsCommandStream stream = new XlsCommandStream(contentBuilder, reader, typeCodes, Size - header.GetSize())) {
					using (BinaryReader olapReader = new BinaryReader(stream)) {
						pivotOlap = XlsPivotFieldOLAPExtReWriter.FromStream(olapReader);
					}
				}
				return;
			}
			base.ReadCore(reader, contentBuilder);
		}
		protected override void CheckPosition(XlsReader reader, XlsContentBuilder contentBuilder, long initialPosition, long expectedPosition) {
		}
		protected override void ApplySheetContent(XlsContentBuilder contentBuilder) {
		}
		public override IXlsCommand GetInstance() {
			return this;
		}
		#endregion
	}
	public class XlsPivotFieldOLAPExtReWriter : IEquatable<XlsPivotFieldOLAPExtReWriter> {
		#region Fields
		private enum FieldOlapFirst {
			TensorSort = 0,				 
			DrilledLevel = 1,			   
			ItemsDrilledByDefault = 2,	  
			MemPropDisplayInReport = 3,	 
			MemPropDisplayInTip = 4,		
			MemPropDisplayInCaption = 5,	
		}
		BitwiseContainer first = new BitwiseContainer(16);
		int pivotHierarchy;
		List<PivotItemFlags> flags;
		#endregion
		#region Properties
		public bool IsTensorSort {
			get { return first.GetBoolValue((int)FieldOlapFirst.TensorSort); }
			set { first.SetBoolValue((int)FieldOlapFirst.TensorSort, value); }
		}
		public bool IsDrilledLevel {
			get { return first.GetBoolValue((int)FieldOlapFirst.DrilledLevel); }
			set { first.SetBoolValue((int)FieldOlapFirst.DrilledLevel, value); }
		}
		public bool IsItemsDrilledByDefault {
			get { return first.GetBoolValue((int)FieldOlapFirst.ItemsDrilledByDefault); }
			set { first.SetBoolValue((int)FieldOlapFirst.ItemsDrilledByDefault, value); }
		}
		public bool IsMemPropDisplayInReport {
			get { return first.GetBoolValue((int)FieldOlapFirst.MemPropDisplayInReport); }
			set { first.SetBoolValue((int)FieldOlapFirst.MemPropDisplayInReport, value); }
		}
		public bool IsMemPropDisplayInTip {
			get { return first.GetBoolValue((int)FieldOlapFirst.MemPropDisplayInTip); }
			set { first.SetBoolValue((int)FieldOlapFirst.MemPropDisplayInTip, value); }
		}
		public bool IsMemPropDisplayInCaption {
			get { return first.GetBoolValue((int)FieldOlapFirst.MemPropDisplayInCaption); }
			set { first.SetBoolValue((int)FieldOlapFirst.MemPropDisplayInCaption, value); }
		}
		public int PivotHierarchyIndex {
			get { return pivotHierarchy; }
			set {
				ValueChecker.CheckValue(value, -1, int.MaxValue, "PivotHierarchyIndex");
				pivotHierarchy = value;
			}
		}
		public int BaseOlapIndex { get; set; }
		public int CountFlags {
			get {
				if (flags != null)
					return flags.Count;
				return 0;
			}
		}
		public List<PivotItemFlags> FlagCollection {
			get { return flags; }
			set { flags = value; }
		}
		#endregion
		#region Methods
		public static XlsPivotFieldOLAPExtReWriter FromStream(BinaryReader reader) {
			XlsPivotFieldOLAPExtReWriter result = new XlsPivotFieldOLAPExtReWriter();
			result.Read(reader);
			return result;
		}
		protected void Read(BinaryReader reader) {
			first.ShortContainer = reader.ReadInt16();
			PivotHierarchyIndex = reader.ReadInt16();
			BaseOlapIndex = reader.ReadInt32();
			int countItems = reader.ReadInt32();
			flags = new List<PivotItemFlags>();
			for (int index = 0; index < countItems; index++)
				flags.Add(new PivotItemFlags(reader.ReadInt16()));
		}
		public void Write(BinaryWriter writer) {
			writer.Write((short)first.ShortContainer);
			writer.Write((short)PivotHierarchyIndex);
			writer.Write((int)BaseOlapIndex);
			writer.Write((int)CountFlags);
			for (int index = 0; index < CountFlags; index++)
				writer.Write((short)flags[index].Value);
		}
		public override bool Equals(Object other) {
			if (typeof(XlsPivotFieldOLAPExtReWriter) != other.GetType())
				return false;
			return this.Equals((XlsPivotFieldOLAPExtReWriter)other);
		}
		public bool Equals(XlsPivotFieldOLAPExtReWriter other) {
			if (this.first.Equals(other.first))
				if (this.pivotHierarchy == other.pivotHierarchy)
					if (this.BaseOlapIndex == other.BaseOlapIndex)
						if (this.flags.Count == other.flags.Count) {
							for (int index = 0; index < this.flags.Count; index++)
								if (!this.flags[index].Equals(other.flags[index]))
									return false;
							return true;
						}
			return false;
		}
		public override int GetHashCode() {
			return base.GetHashCode();
		}
		#endregion
	}
	public class PivotItemFlags : IEquatable<PivotItemFlags> {
		#region Fields
		enum FlagNames {
			DrilledMember = 0,	  
			HasChildren = 2,		
			CollapsedMember = 3,	
			HasChildrenEst = 4,	 
			OlapFilterSelected = 5, 
		}
		BitwiseContainer flags;
		#endregion
		#region Properties
		public bool IsDrilledMember {
			get { return flags.GetBoolValue((int)FlagNames.DrilledMember); }
			set { flags.SetBoolValue((int)FlagNames.DrilledMember, value); }
		}
		public bool IsHasChildren {
			get { return flags.GetBoolValue((int)FlagNames.HasChildren); }
			set { flags.SetBoolValue((int)FlagNames.HasChildren, value); }
		}
		public bool IsCollapsedMember {
			get { return flags.GetBoolValue((int)FlagNames.CollapsedMember); }
			set { flags.SetBoolValue((int)FlagNames.CollapsedMember, value); }
		}
		public bool IsHasChildrenEst {
			get { return flags.GetBoolValue((int)FlagNames.HasChildrenEst); }
			set { flags.SetBoolValue((int)FlagNames.HasChildrenEst, value); }
		}
		public bool IsOlapFilterSelected {
			get { return flags.GetBoolValue((int)FlagNames.OlapFilterSelected); }
			set { flags.SetBoolValue((int)FlagNames.OlapFilterSelected, value); }
		}
		public short Value { get { return flags.ShortContainer; } }
		#endregion
		#region Constructors
		public PivotItemFlags(short flags) {
			this.flags = new BitwiseContainer(16) { ShortContainer = flags };
		}
		#endregion
		#region Methods
		public override bool Equals(Object other) {
			if (typeof(PivotItemFlags) != other.GetType())
				return false;
			return this.Equals((PivotItemFlags)other);
		}
		public bool Equals(PivotItemFlags other) {
			return this.flags.Equals(other.flags);
		}
		public override int GetHashCode() {
			return base.GetHashCode();
		}
		#endregion
	}
	#endregion    
}
