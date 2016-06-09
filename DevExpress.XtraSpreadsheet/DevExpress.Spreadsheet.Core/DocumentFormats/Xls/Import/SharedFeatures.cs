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
using DevExpress.Utils;
using DevExpress.XtraSpreadsheet.Internal;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.XtraSpreadsheet.Export.Xls;
using DevExpress.XtraSpreadsheet.Utils;
using DevExpress.XtraExport.Xls;
namespace DevExpress.XtraSpreadsheet.Import.Xls {
#if !DOTNET
	using System.Security.AccessControl;
#endif
	using DevExpress.XtraSpreadsheet.Services.Implementation;
	using DevExpress.Office.Utils;
	#region XlsSharedFeatureType
	public enum XlsSharedFeatureType {
		Protection = 2,
		Fec2 = 3,
		Factoid = 4,
		List = 5
	}
	#endregion
	#region Ref8U
	public class Ref8U {
		internal const short Size = 8;
		public CellRangeInfo CellRangeInfo { get; set; }
		public static Ref8U FromStream(BinaryReader reader) {
			Ref8U result = new Ref8U();
			result.Read(reader);
			return result;
		}
		public static Ref8U FromStream(XlsReader reader) {
			Ref8U result = new Ref8U();
			result.Read(reader);
			return result;
		}
		void Read(BinaryReader reader) {
			ushort rowFirst = (ushort)reader.ReadInt16();
			ushort rowLast = (ushort)reader.ReadInt16();
			ushort columnFirst = (ushort)reader.ReadInt16();
			ushort columnLast = (ushort)reader.ReadInt16();
			CreateCellRangeInfo(rowFirst, rowLast, columnFirst, columnLast);
		}
		void Read(XlsReader reader) {
			ushort rowFirst = (ushort)reader.ReadInt16();
			ushort rowLast = (ushort)reader.ReadInt16();
			ushort columnFirst = (ushort)reader.ReadInt16();
			ushort columnLast = (ushort)reader.ReadInt16();
			CreateCellRangeInfo(rowFirst, rowLast, columnFirst, columnLast);
		}
		void CreateCellRangeInfo(int rowFirst, int rowLast, int columnFirst, int columnLast) {
			if (rowLast < rowFirst || columnLast < columnFirst)
				throw new ArgumentException("Invalid Ref8U structure");
			CellPosition topLeft = new CellPosition(columnFirst, rowFirst);
			CellPosition bottomRight = new CellPosition(columnLast, rowLast);
			CellRangeInfo = new CellRangeInfo(topLeft, bottomRight);
		}
		public void Write(BinaryWriter writer) {
			writer.Write((ushort)CellRangeInfo.First.Row);
			writer.Write((ushort)CellRangeInfo.Last.Row);
			writer.Write((ushort)CellRangeInfo.First.Column);
			writer.Write((ushort)CellRangeInfo.Last.Column);
		}
	}
	#endregion
	#region XlsSharedFeatureBase
	public abstract class XlsSharedFeatureBase {
		#region Fields
		readonly List<Ref8U> refs = new List<Ref8U>();
		#endregion
		#region Properties
		public abstract XlsSharedFeatureType FeatureType { get; }
		public List<Ref8U> Refs { get { return refs; } }
		#endregion
		protected virtual void Read(BinaryReader reader) {
			reader.ReadByte(); 
			reader.ReadInt32(); 
			int count = reader.ReadUInt16();
			reader.ReadInt32(); 
			reader.ReadInt16(); 
			for (int i = 0; i < count; i++)
				Refs.Add(Ref8U.FromStream(reader));
		}
		public virtual void Write(BinaryWriter writer) {
			writer.Write((ushort)FeatureType);
			writer.Write((byte)0); 
			writer.Write((int)0); 
			int count = Refs.Count;
			writer.Write((ushort)count);
			writer.Write((int)(FeatureType == XlsSharedFeatureType.Fec2 ? 4 : 0)); 
			writer.Write((ushort)0); 
			for (int i = 0; i < count; i++)
				Refs[i].Write(writer);
		}
		public virtual void Execute(XlsContentBuilder contentBuilder) {
		}
		protected CellRangeBase GetCellRange(XlsContentBuilder contentBuilder) {
			if(Refs.Count == 0)
				return null;
			if (Refs.Count == 1) {
				CellRangeInfo range = Refs[0].CellRangeInfo;
				return XlsCellRangeFactory.CreateCellRange(contentBuilder.CurrentSheet, range.First, range.Last);
			}
			CellRangeList list = new CellRangeList();
			for (int i = 0; i < Refs.Count; i++) {
				CellRangeInfo range = Refs[i].CellRangeInfo;
				list.Add(XlsCellRangeFactory.CreateCellRange(contentBuilder.CurrentSheet, range.First, range.Last));
			}
			return new CellUnion(list);
		}
	}
	#endregion
	#region XlsSharedFeatureProtection
	public class XlsSharedFeatureProtection : XlsSharedFeatureBase {
		#region Fields
		XLUnicodeString title = new XLUnicodeString();
		byte[] securityDescriptor = null;
		bool isValid;
		#endregion
		public static XlsSharedFeatureProtection FromStream(BinaryReader reader) {
			XlsSharedFeatureProtection result = new XlsSharedFeatureProtection();
			result.Read(reader);
			return result;
		}
		#region Properties
		public override XlsSharedFeatureType FeatureType { get { return XlsSharedFeatureType.Protection; } }
		public int PasswordVerifier { get; set; }
		public string Title { get { return title.Value; } set { title.Value = value; } }
		public byte[] SecurityDescriptor { get { return securityDescriptor; } set { securityDescriptor = value; } }
		#endregion
		protected override void Read(BinaryReader reader) {
			base.Read(reader);
			uint bitwiseField = reader.ReadUInt32();
			isValid = bitwiseField != 0xffffffff;
			if (isValid) {
				bool hasSecurityDescriptor = (bitwiseField & 0x0001) != 0;
				PasswordVerifier = reader.ReadInt32();
				title = XLUnicodeString.FromStream(reader);
				if (hasSecurityDescriptor) {
					int sdBytesCount = reader.ReadInt32();
					securityDescriptor = reader.ReadBytes(sdBytesCount);
				}
				else
					securityDescriptor = null;
			}
		}
		public override void Write(BinaryWriter writer) {
			base.Write(writer);
			writer.Write((int)(securityDescriptor != null ? 1 : 0));
			writer.Write(PasswordVerifier);
			title.Write(writer);
			if (securityDescriptor != null) {
				writer.Write((int)securityDescriptor.Length);
				writer.Write(securityDescriptor);
			}
		}
		public override void Execute(XlsContentBuilder contentBuilder) {
			if (!isValid)
				return;
			CellRangeBase cellRange = GetCellRange(contentBuilder);
			if (cellRange == null)
				return;
			ModelProtectedRange range = new ModelProtectedRange(GetUniqueName(contentBuilder, Title), cellRange);
			ProtectionCredentials credentials = new ProtectionCredentials();
			credentials.RegisterPasswordVerifier(new ProtectionByPasswordVerifier((ushort)PasswordVerifier));
			range.Credentials = credentials;
#if !SL && !DOTNET
			if (SecurityDescriptor != null) {
				RangeSecurity rangeSecurity = new RangeSecurity();
				rangeSecurity.SetSecurityDescriptorBinaryForm(SecurityDescriptor);
				range.SecurityDescriptor = rangeSecurity.GetSecurityDescriptorSddlForm(AccessControlSections.All);
			}
#endif
			contentBuilder.CurrentSheet.ProtectedRanges.Add(range);
		}
		string GetUniqueName(XlsContentBuilder contentBuilder, string name) {
			int modifier = 1;
			string result = name;
			while (contentBuilder.UniqueProtectedRangeNames.Contains(result)) {
				result = string.Format("{0}_{1}", name, modifier);
				modifier++;
			}
			contentBuilder.UniqueProtectedRangeNames.Add(result);
			return result;
		}
	}
	#endregion
	#region XlsSharedFeatureErrorCheck
	public class XlsSharedFeatureErrorCheck : XlsSharedFeatureBase {
		public static XlsSharedFeatureErrorCheck FromStream(BinaryReader reader) {
			XlsSharedFeatureErrorCheck result = new XlsSharedFeatureErrorCheck();
			result.Read(reader);
			return result;
		}
		#region Properties
		public override XlsSharedFeatureType FeatureType { get { return XlsSharedFeatureType.Fec2; } }
		public bool CalculationErrors { get; set; }
		public bool EmptyCellRef { get; set; }
		public bool NumberStoredAsText { get; set; }
		public bool InconsistRange { get; set; }
		public bool InconsistFormula { get; set; }
		public bool TextDateInsuff { get; set; }
		public bool UnprotectedFormula { get; set; }
		public bool DataValidation { get; set; }
		public bool InconsistColumnFormula { get; set; }
		#endregion
		protected override void Read(BinaryReader reader) {
			base.Read(reader);
			uint bitwiseField = reader.ReadUInt32();
			CalculationErrors = (bitwiseField & 0x0001) != 0;
			EmptyCellRef = (bitwiseField & 0x0002) != 0;
			NumberStoredAsText = (bitwiseField & 0x0004) != 0;
			InconsistRange = (bitwiseField & 0x0008) != 0;
			InconsistFormula = (bitwiseField & 0x0010) != 0;
			TextDateInsuff = (bitwiseField & 0x0020) != 0;
			UnprotectedFormula = (bitwiseField & 0x0040) != 0;
			DataValidation = (bitwiseField & 0x0080) != 0;
			InconsistColumnFormula = (bitwiseField & 0x0100) != 0;
		}
		public override void Write(BinaryWriter writer) {
			base.Write(writer);
			uint bitwiseField = 0;
			if (CalculationErrors)
				bitwiseField |= 0x0001;
			if (EmptyCellRef)
				bitwiseField |= 0x0002;
			if (NumberStoredAsText)
				bitwiseField |= 0x0004;
			if (InconsistRange)
				bitwiseField |= 0x0008;
			if (InconsistFormula)
				bitwiseField |= 0x0010;
			if (TextDateInsuff)
				bitwiseField |= 0x0020;
			if (UnprotectedFormula)
				bitwiseField |= 0x0040;
			if (DataValidation)
				bitwiseField |= 0x0080;
			if (InconsistColumnFormula)
				bitwiseField |= 0x0100;
			writer.Write(bitwiseField);
		}
		public override void Execute(XlsContentBuilder contentBuilder) {
			CellRangeBase errorRange = GetCellRange(contentBuilder);
			if (errorRange == null)
				return;
			Worksheet sheet = contentBuilder.CurrentSheet;
			IgnoredError ignoredError = new IgnoredError(sheet, errorRange);
			ignoredError.BeginUpdate();
			try {
				ignoredError.EmptyCellReferences = EmptyCellRef;
				ignoredError.EvaluateToError = CalculationErrors;
				ignoredError.FormulaRangeError = InconsistRange;
				ignoredError.InconsistentFormula = InconsistFormula;
				ignoredError.InconsistentColumnFormula = InconsistColumnFormula;
				ignoredError.ListDataValidation = DataValidation;
				ignoredError.NumberAsText = NumberStoredAsText;
				ignoredError.TwoDidgitTextYear = TextDateInsuff;
				ignoredError.UnlockedFormula = UnprotectedFormula;
			}
			finally {
				ignoredError.EndUpdate();
			}
			sheet.IgnoredErrors.AddCore(ignoredError);
		}
	}
	#endregion
	#region XlsSmartTagProperty
	public class XlsSmartTagProperty {
		public static XlsSmartTagProperty FromStream(BinaryReader reader) {
			XlsSmartTagProperty result = new XlsSmartTagProperty();
			result.Read(reader);
			return result;
		}
		#region Properties
		public int KeyIndex { get; set; }
		public int ValueIndex { get; set; }
		#endregion
		protected void Read(BinaryReader reader) {
			KeyIndex = reader.ReadInt32();
			ValueIndex = reader.ReadInt32();
		}
		public void Write(BinaryWriter writer) {
			writer.Write(KeyIndex);
			writer.Write(ValueIndex);
		}
	}
	#endregion
	#region XlsSmartTagData
	public class XlsSmartTagData {
		#region Fields
		int typeId;
		readonly List<XlsSmartTagProperty> properties = new List<XlsSmartTagProperty>();
		#endregion
		public static XlsSmartTagData FromStream(BinaryReader reader) {
			XlsSmartTagData result = new XlsSmartTagData();
			result.Read(reader);
			return result;
		}
		#region Properties
		public bool Deleted { get; set; }
		public bool XMLBased { get; set; }
		public int TypeId {
			get { return typeId; }
			set {
				ValueChecker.CheckValue(value, ushort.MinValue, ushort.MaxValue, "TypeId");
				typeId = value;
			}
		}
		public List<XlsSmartTagProperty> Properties { get { return properties; } }
		#endregion
		protected void Read(BinaryReader reader) {
			byte bitwiseField = reader.ReadByte();
			Deleted = (bitwiseField & 0x01) != 0;
			XMLBased = (bitwiseField & 0x02) != 0;
			typeId = reader.ReadUInt16();
			int count = reader.ReadUInt16();
			reader.ReadUInt16(); 
			for (int i = 0; i < count; i++)
				Properties.Add(XlsSmartTagProperty.FromStream(reader));
		}
		public void Write(BinaryWriter writer) {
			byte bitwiseField = 0;
			if (Deleted)
				bitwiseField |= 0x01;
			if (XMLBased)
				bitwiseField |= 0x02;
			writer.Write(bitwiseField);
			writer.Write((ushort)typeId);
			int count = Properties.Count;
			writer.Write((ushort)count);
			writer.Write((ushort)0); 
			for (int i = 0; i < count; i++)
				Properties[i].Write(writer);
		}
	}
	#endregion
	#region XlsSharedFeatureSmartTags
	public class XlsSharedFeatureSmartTags : XlsSharedFeatureBase {
		#region Fields
		readonly List<XlsSmartTagData> items = new List<XlsSmartTagData>();
		#endregion
		public static XlsSharedFeatureSmartTags FromStream(BinaryReader reader) {
			XlsSharedFeatureSmartTags result = new XlsSharedFeatureSmartTags();
			result.Read(reader);
			return result;
		}
		#region Properties
		public override XlsSharedFeatureType FeatureType { get { return XlsSharedFeatureType.Factoid; } }
		public int HashValue { get; set; }
		public List<XlsSmartTagData> Items { get { return items; } }
		#endregion
		protected override void Read(BinaryReader reader) {
			base.Read(reader);
			HashValue = reader.ReadInt32();
			int count = reader.ReadByte();
			for (int i = 0; i < count; i++)
				Items.Add(XlsSmartTagData.FromStream(reader));
		}
		public override void Write(BinaryWriter writer) {
			base.Write(writer);
			writer.Write(HashValue);
			int count = Items.Count;
			writer.Write((byte)count);
			for (int i = 0; i < count; i++)
				Items[i].Write(writer);
		}
	}
	#endregion
}
