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
	#region XlsCommandPivotHierarchy  -- SXTH --
	public class XlsCommandPivotHierarchy : XlsCommandRecordBase {
		#region Fields
		static short[] typeCodes = new short[] {
			0x0812  
		};
		XlsPivotHierarchyPropertyReWriter pivotHierarchy = new XlsPivotHierarchyPropertyReWriter();
		#endregion
		#region Properties
		public XlsPivotHierarchyPropertyReWriter PivotHierarchy { get { return pivotHierarchy; } }
		#endregion
		#region Methods
		protected override void ReadCore(XlsReader reader, XlsContentBuilder contentBuilder) {
			XlsBuildPivotView builderPivotView = contentBuilder.CurrentBuilderPivotView;
			if (contentBuilder.ContentType == XlsContentType.Sheet && builderPivotView != null) {
				FutureRecordHeaderOld header = FutureRecordHeaderOld.FromStream(reader);
				using (XlsCommandStream stream = new XlsCommandStream(contentBuilder, reader, typeCodes, Size - header.GetSize())) {
					using (BinaryReader hierarchyReader = new BinaryReader(stream)) {
						pivotHierarchy = XlsPivotHierarchyPropertyReWriter.FromStream(hierarchyReader, builderPivotView.PivotFieldCount);
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
	public class XlsPivotHierarchyPropertyReWriter : IEquatable<XlsPivotHierarchyPropertyReWriter> {
		#region Fields
		private enum PropNameFirst {
			Measure = 0,					
			OutlineMode = 2,				
			EnableMultiplePageItems = 3,	
			SubtotalAtTop = 4,			  
			NamedSet = 5,				   
			DontShowFList = 6,			  
			AttributeHierarchy = 7,		 
			TimeHierarchy = 8,			  
			FilterInclusive = 9,			
			KeyAttributeHierarchy = 11,	 
			KeyPerformanceIndicator = 12,   
		}
		private enum PropNameSecond {
			DragToRow = 0,				  
			DragToColumn = 1,			   
			DragToPage = 2,				 
			DragToData = 3,				 
			DragToHide = 4,				 
		}
		BitwiseContainer propFirst = new BitwiseContainer(32);
		BitwiseContainer propSecond = new BitwiseContainer(16);
		XLUnicodeString uniqueName = new XLUnicodeString();
		XLUnicodeString displayName = new XLUnicodeString();
		XLUnicodeString defaultName = new XLUnicodeString();
		XLUnicodeString allName = new XLUnicodeString();
		XLUnicodeString dimensionName = new XLUnicodeString();
		int fieldIndex;
		int fieldCount;
		#endregion
		#region Properties
		public int MaxValuePivotField { get; set; }
		public bool IsMeasure {
			get { return propFirst.GetBoolValue((int)PropNameFirst.Measure); }
			set { propFirst.SetBoolValue((int)PropNameFirst.Measure, value); }
		}
		public bool IsOutlineMode {
			get { return propFirst.GetBoolValue((int)PropNameFirst.OutlineMode); }
			set { propFirst.SetBoolValue((int)PropNameFirst.OutlineMode, value); }
		}
		public bool IsEnableMultiplePageItems {
			get { return propFirst.GetBoolValue((int)PropNameFirst.EnableMultiplePageItems); }
			set { propFirst.SetBoolValue((int)PropNameFirst.EnableMultiplePageItems, value); }
		}
		public bool IsSubtotalAtTop {
			get { return propFirst.GetBoolValue((int)PropNameFirst.SubtotalAtTop); }
			set { propFirst.SetBoolValue((int)PropNameFirst.SubtotalAtTop, value); }
		}
		public bool IsNamedSet {
			get { return propFirst.GetBoolValue((int)PropNameFirst.NamedSet); }
			set {
				if (IsMeasure && value)
					throw new ArgumentException("MUST be 0 if fMeasure is 1");
				propFirst.SetBoolValue((int)PropNameFirst.NamedSet, value);
			}
		}
		public bool IsDontShowFieldList {
			get { return propFirst.GetBoolValue((int)PropNameFirst.DontShowFList); }
			set { propFirst.SetBoolValue((int)PropNameFirst.DontShowFList, value); }
		}
		public bool IsAttributeHierarchy {
			get { return propFirst.GetBoolValue((int)PropNameFirst.AttributeHierarchy); }
			set { propFirst.SetBoolValue((int)PropNameFirst.AttributeHierarchy, value); }
		}
		public bool IsTimeHierarchy {
			get { return propFirst.GetBoolValue((int)PropNameFirst.TimeHierarchy); }
			set { propFirst.SetBoolValue((int)PropNameFirst.TimeHierarchy, value); }
		}
		public bool IsFilterInclusive {
			get { return propFirst.GetBoolValue((int)PropNameFirst.FilterInclusive); }
			set { propFirst.SetBoolValue((int)PropNameFirst.FilterInclusive, value); }
		}
		public bool IsKeyAttributeHierarchy {
			get { return propFirst.GetBoolValue((int)PropNameFirst.KeyAttributeHierarchy); }
			set { propFirst.SetBoolValue((int)PropNameFirst.KeyAttributeHierarchy, value); }
		}
		public bool IsKeyPerformanceIndicator {
			get { return propFirst.GetBoolValue((int)PropNameFirst.KeyPerformanceIndicator); }
			set { propFirst.SetBoolValue((int)PropNameFirst.KeyPerformanceIndicator, value); }
		}
		public bool IsDragToRow {
			get { return propSecond.GetBoolValue((int)PropNameSecond.DragToRow); }
			set {
				if (IsMeasure && value)
					throw new ArgumentException("MUST be 0 because fMeasure is 1.");
				propSecond.SetBoolValue((int)PropNameSecond.DragToRow, value);
			}
		}
		public bool IsDragToColumn {
			get { return propSecond.GetBoolValue((int)PropNameSecond.DragToColumn); }
			set {
				if (IsMeasure && value)
					throw new ArgumentException("MUST be 0 because fMeasure is 1.");
				propSecond.SetBoolValue((int)PropNameSecond.DragToColumn, value);
			}
		}
		public bool IsDragToPage {
			get { return propSecond.GetBoolValue((int)PropNameSecond.DragToPage); }
			set {
				if (IsMeasure && value)
					throw new ArgumentException("MUST be 0 because fMeasure is 1.");
				propSecond.SetBoolValue((int)PropNameSecond.DragToPage, value);
			}
		}
		public bool IsDragToData {
			get { return propSecond.GetBoolValue((int)PropNameSecond.DragToData); }
			set { propSecond.SetBoolValue((int)PropNameSecond.DragToData, value); }
		}
		public bool IsDragToHide {
			get { return propSecond.GetBoolValue((int)PropNameSecond.DragToHide); }
			set { propSecond.SetBoolValue((int)PropNameSecond.DragToHide, value); }
		}
		public PivotTableAxis AxisPivotHierarchy { get; set; }
		public int PivotFieldIndex {
			get { return fieldIndex; }
			set {
				if (AxisPivotHierarchy == PivotTableAxis.Page || AxisPivotHierarchy == PivotTableAxis.Value)
					ValueChecker.CheckValue(value, 0, MaxValuePivotField, "PivotFieldIndex");
				fieldIndex = value;
			}
		}
		public int PivotFieldCount {
			get { return fieldCount; }
			set {
				if ((AxisPivotHierarchy == PivotTableAxis.Page || AxisPivotHierarchy == PivotTableAxis.Value) && value != 1)
					throw new ArgumentException("The value of this field MUST be 1 because sxaxis.sxaxisPage is 1 or sxaxis.sxaxisData is 1");
				if (AxisPivotHierarchy == PivotTableAxis.None && value != 0)
					throw new ArgumentException("The value of this field MUST be 0 because sxaxis.sxaxisPage, sxaxis.sxaxisCol, sxaxis.sxaxisRw and sxaxis.sxaxisData is 0");
				fieldCount = value;
			}
		}
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
		public string DefaultName {
			get { return defaultName.Value; }
			set {
				ValueChecker.CheckLength(value, 255, "DefaultName");
				defaultName.Value = value;
			}
		}
		public string AllName {
			get { return allName.Value; }
			set {
				ValueChecker.CheckLength(value, 255, "AllName");
				allName.Value = value;
			}
		}
		public string DimensionName {
			get { return dimensionName.Value; }
			set {
				if (IsMeasure && !String.IsNullOrEmpty(value))
					throw new ArgumentException("The length of the string MUST be zero because fMeasure is 1");
				ValueChecker.CheckLength(value, 255, "DimensionName");
				dimensionName.Value = value;
			}
		}
		public int CountAssociatedField {
			get {
				if (ArrayAssociatedFields != null && (AxisPivotHierarchy == PivotTableAxis.Row || AxisPivotHierarchy == PivotTableAxis.Column))
					return ArrayAssociatedFields.Length;
				return 0;
			}
			set {
				if (AxisPivotHierarchy != PivotTableAxis.Row && AxisPivotHierarchy != PivotTableAxis.Column && value > 0)
					throw new ArgumentException("MUST be zero because sxaxis.sxaxisRw is 0 and sxaxis.sxaxisCol is 0");
				ArrayAssociatedFields = new int[value];
			}
		}
		public int[] ArrayAssociatedFields { get; set; }
		public int CountHiddenMemberSets {
			get {
				if (IsFilterInclusive || ArrayHiddenMemberSets == null)
					return 0;
				return ArrayHiddenMemberSets.Length;
			}
			set {
				if (!IsFilterInclusive) {
					ArrayHiddenMemberSets = new HiddenMemberSet[value];
				}
			}
		}
		public HiddenMemberSet[] ArrayHiddenMemberSets { get; set; }
		#endregion
		#region Methods
		public static XlsPivotHierarchyPropertyReWriter FromStream(BinaryReader reader, int maxField) {
			XlsPivotHierarchyPropertyReWriter result = new XlsPivotHierarchyPropertyReWriter();
			result.MaxValuePivotField = maxField;
			result.Read(reader);
			return result;
		}
		protected void Read(BinaryReader reader) {
			propFirst.IntContainer = reader.ReadInt32();
			AxisPivotHierarchy = (PivotTableAxis)(reader.ReadInt32() & 0xF);
			PivotFieldIndex = reader.ReadInt32();
			PivotFieldCount = reader.ReadInt32();
			propSecond.ShortContainer = reader.ReadInt16();
			uniqueName = XLUnicodeString.FromStream(reader);
			displayName = XLUnicodeString.FromStream(reader);
			defaultName = XLUnicodeString.FromStream(reader);
			allName = XLUnicodeString.FromStream(reader);
			dimensionName = XLUnicodeString.FromStream(reader);
			CountAssociatedField = (int)reader.ReadUInt32();
			for (int index = 0; index < CountAssociatedField; index++) {
				int value = reader.ReadInt32();
				ValueChecker.CheckValue(value, -1, int.MaxValue, "ArrayAssociatedFields index : " + index);
				ArrayAssociatedFields[index] = value;
			}
			CountHiddenMemberSets = (int)reader.ReadUInt32();
			for (int index = 0; index < CountHiddenMemberSets; index++)
				ArrayHiddenMemberSets[index] = HiddenMemberSet.FromStream(reader);
		}
		public void Write(BinaryWriter writer) {
			writer.Write(propFirst.IntContainer);
			writer.Write((int)AxisPivotHierarchy);
			writer.Write(PivotFieldIndex);
			writer.Write(PivotFieldCount);
			writer.Write((short)propSecond.ShortContainer);
			uniqueName.Write(writer);
			displayName.Write(writer);
			defaultName.Write(writer);
			allName.Write(writer);
			if(IsMeasure)
				(new XLUnicodeString()).Write(writer);
			else
				dimensionName.Write(writer);
			writer.Write((uint)CountAssociatedField);
			for (int index = 0; index < CountAssociatedField; index++)
				writer.Write(ArrayAssociatedFields[index]);
			writer.Write((uint)CountHiddenMemberSets);
			for (int index = 0; index < CountHiddenMemberSets; index++)
				ArrayHiddenMemberSets[index].Write(writer);
		}
		public override bool Equals(Object other) {
			if (typeof(XlsPivotHierarchyPropertyReWriter) != other.GetType())
				return false;
			return this.Equals((XlsPivotHierarchyPropertyReWriter)other);
		}
		public bool Equals(XlsPivotHierarchyPropertyReWriter other) {
			if (this.propFirst.Equals(other.propFirst))
				if (this.propSecond.Equals(other.propSecond))
					if (this.uniqueName.Value.Equals(other.uniqueName.Value))
						if (this.displayName.Value.Equals(other.displayName.Value))
							if (this.defaultName.Value.Equals(other.defaultName.Value))
								if (this.allName.Value.Equals(other.allName.Value))
									if (this.dimensionName.Value.Equals(other.dimensionName.Value))
										if (this.fieldIndex == other.fieldIndex && this.fieldCount == other.fieldCount)
											if (this.AxisPivotHierarchy == other.AxisPivotHierarchy)
												if (this.CountAssociatedField == other.CountAssociatedField) {
													for (int index = 0; index < this.CountAssociatedField; index++) {
														if (this.ArrayAssociatedFields[index] != other.ArrayAssociatedFields[index])
															return false;
													}
													if (this.CountHiddenMemberSets == other.CountHiddenMemberSets) {
														for (int index = 0; index < this.CountHiddenMemberSets; index++)
															if (!this.ArrayHiddenMemberSets[index].Equals(other.ArrayHiddenMemberSets[index]))
																return false;
														return true;
													}
												}
			return false;
		}
		public override int GetHashCode() {
			return base.GetHashCode();
		}
		#endregion
	}
	public class HiddenMemberSet : IEquatable<HiddenMemberSet> {
		List<XLUnicodeString> listName;
		public int NameCollectionCount { get { return listName != null ? listName.Count : 0; } }
		public List<XLUnicodeString> NameCollection {
			get { return listName; }
			set { listName = value; }
		}
		protected void Read(BinaryReader reader) {
			uint memberNameCount = reader.ReadUInt32();
			listName = new List<XLUnicodeString>();
			for (int index = 0; index < memberNameCount; index++)
				listName.Add(XLUnicodeString.FromStream(reader));
		}
		public void Write(BinaryWriter writer) {
			writer.Write(NameCollectionCount);
			for (int index = 0; index < NameCollectionCount; index++)
				listName[index].Write(writer);
		}
		public override bool Equals(Object other) {
			if (typeof(HiddenMemberSet) != other.GetType())
				return false;
			return this.Equals((HiddenMemberSet)other);
		}
		public bool Equals(HiddenMemberSet other) {
			if (this.NameCollectionCount == other.NameCollectionCount) {
				for (int index = 0; index < this.NameCollectionCount; index++)
					if (!other.listName[index].Value.Equals(this.listName[index].Value))
						return false;
				return true;
			}
			return false;
		}
		public override int GetHashCode() {
			return base.GetHashCode();
		}
		public static HiddenMemberSet FromStream(BinaryReader reader) {
			HiddenMemberSet result = new HiddenMemberSet();
			result.Read(reader);
			return result;
		}
	}
	#endregion
}
