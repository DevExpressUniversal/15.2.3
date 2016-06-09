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
using System.Reflection;
using System.Text;
using DevExpress.Office;
using DevExpress.Utils;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.XtraSpreadsheet.Utils;
using DevExpress.Office.Utils;
using DevExpress.XtraSpreadsheet.Internal;
using DevExpress.XtraExport.Xls;
namespace DevExpress.XtraSpreadsheet.Import.Xls {
	#region XlsCommandPivotCacheProperties
	public class XlsCommandPivotCacheProperties : XlsCommandBase {
		int numberOfRecords;
		int streamId;
		int dataCount;
		int totalCount;
		int usedCount;
		XLUnicodeStringNoCch userName = new XLUnicodeStringNoCch();
		#region Properties
		public int NumberOfRecords {
			get { return numberOfRecords; }
			set {
				Guard.ArgumentNonNegative(value, "NumberOfRecords");
				numberOfRecords = value;
			}
		}
		public int StreamId {
			get { return streamId; }
			set {
				ValueChecker.CheckValue(value, 0, ushort.MaxValue, "StreamId");
				streamId = value;
			}
		}
		public bool SaveData { get; set; }
		public bool Invalid { get; set; }
		public bool RefreshOnLoad { get; set; }
		public bool OptimizeCache { get; set; }
		public bool BackgroundQuery { get; set; }
		public bool EnableRefresh { get; set; }
		public int DataCount {
			get { return dataCount; }
			set {
				ValueChecker.CheckValue(value, 0, short.MaxValue, "DataCount");
				dataCount = value;
			}
		}
		public int TotalCount {
			get { return totalCount; }
			set {
				ValueChecker.CheckValue(value, 0, short.MaxValue, "TotalCount");
				totalCount = value;
			}
		}
		public int UsedCount {
			get { return usedCount; }
			set {
				ValueChecker.CheckValue(value, 0, ushort.MaxValue, "UsedCount");
				usedCount = value;
			}
		}
		public PivotCacheType CacheType { get; set; }
		public string UserName {
			get { return userName.Value; }
			set {
				ValueChecker.CheckLength(value, 255, "UserName");
				userName.Value = value; 
			}
		}
		#endregion
		protected override void ReadCore(XlsReader reader, XlsContentBuilder contentBuilder) {
			NumberOfRecords = reader.ReadInt32();
			StreamId = reader.ReadUInt16();
			ushort bitwiseField = reader.ReadUInt16();
			SaveData = (bitwiseField & 0x0001) != 0;
			Invalid = (bitwiseField & 0x0002) != 0;
			RefreshOnLoad = (bitwiseField & 0x0004) != 0;
			OptimizeCache = (bitwiseField & 0x0008) != 0;
			BackgroundQuery = (bitwiseField & 0x0010) != 0;
			EnableRefresh = (bitwiseField & 0x0020) != 0;
			reader.ReadUInt16(); 
			DataCount = reader.ReadInt16();
			TotalCount = reader.ReadInt16();
			UsedCount = reader.ReadUInt16();
			CacheType = XlsCommandPivotCacheType.CodeToType(reader.ReadInt16(), contentBuilder);
			ushort cch = reader.ReadUInt16();
			if (cch != 0xffff)
				userName = XLUnicodeStringNoCch.FromStream(reader, cch);
			else
				userName.Value = string.Empty;
		}
		protected override void ApplyPivotCacheContent(XlsContentBuilder contentBuilder) {
			if (contentBuilder == null)
				return;
			XlsPivotCacheBuilderBase pivotCacheBuilder = contentBuilder.CurrentPivotCacheBuilder as XlsPivotCacheBuilderBase;
			pivotCacheBuilder.MaxValuesInRecord = DataCount;
			pivotCacheBuilder.StreamId = StreamId;
			PivotCache pivotCache = contentBuilder.DocumentModel.PivotCaches.Last;
			pivotCache.SaveData = SaveData;
			pivotCache.Invalid = Invalid;
			pivotCache.RefreshOnLoad = RefreshOnLoad;
			pivotCache.OptimizeMemory = OptimizeCache;
			pivotCache.BackgroundQuery = BackgroundQuery;
			pivotCache.EnableRefresh = EnableRefresh;
			pivotCache.RefreshedBy = UserName;
		}
		protected override void WriteCore(BinaryWriter writer) {
			writer.Write(NumberOfRecords);
			writer.Write((ushort)StreamId);
			ushort bitwiseField = 0;
			if(SaveData)
				bitwiseField |= 0x0001;
			if(Invalid)
				bitwiseField |= 0x0002;
			if(RefreshOnLoad)
				bitwiseField |= 0x0004;
			if(OptimizeCache)
				bitwiseField |= 0x0008;
			if(BackgroundQuery)
				bitwiseField |= 0x0010;
			if(EnableRefresh)
				bitwiseField |= 0x0020;
			writer.Write(bitwiseField);
			writer.Write((ushort)0); 
			writer.Write((short)DataCount);
			writer.Write((short)TotalCount);
			writer.Write((ushort)UsedCount);
			writer.Write(XlsCommandPivotCacheType.TypeToCode(CacheType));
			if (UserName.Length > 0) {
				writer.Write((ushort)UserName.Length);
				userName.Write(writer);
			}
			else
				writer.Write((ushort)0xffff);
		}
		protected override short GetSize() {
			int result = 20;
			if (UserName.Length > 0)
				result += userName.Length;
			return (short)result;
		}
		public override IXlsCommand GetInstance() {
			return this;
		}
	}
	#endregion
	#region XlsCommandPivotCachePropertiesExt
	public class XlsCommandPivotCachePropertiesExt : XlsCommandBase {
		#region Properties
		public double LastRefreshed { get; set; }
		public int FieldIndex { get; set; }
		#endregion
		protected override void ReadCore(XlsReader reader, XlsContentBuilder contentBuilder) {
			LastRefreshed = reader.ReadDouble();
			reader.ReadUInt16(); 
			FieldIndex = reader.ReadInt16();
		}
		protected override void ApplyPivotCacheContent(XlsContentBuilder contentBuilder) {
			if (contentBuilder == null)
				return;
			DocumentModel documentModel = contentBuilder.DocumentModel;
			PivotCache pivotCache = documentModel.PivotCaches.Last;
			pivotCache.RefreshedDate = GetRefreshedDate(documentModel.DataContext);
		}
		DateTime GetRefreshedDate(WorkbookDataContext context) {
			if (WorkbookDataContext.IsErrorDateTimeSerial(LastRefreshed, context.DateSystem))
				return LastRefreshed < 0 ? DateTime.MinValue : DateTime.MaxValue;
			else
				return context.FromDateTimeSerial(LastRefreshed);
		}
		protected override void WriteCore(BinaryWriter writer) {
			writer.Write(LastRefreshed);
			writer.Write((ushort)0); 
			writer.Write((short)FieldIndex);
		}
		protected override short GetSize() {
			return 12;
		}
		public override IXlsCommand GetInstance() {
			return this;
		}
	}
	#endregion
	#region XlsCommandPivotCacheFormula
	public class XlsCommandPivotCacheFormula : XlsCommandBase {
		#region Fields
		byte[] formulaBytes = new byte[0];
		ParsedExpression parsedExpression = new ParsedExpression();
		int numberOfNames;
		#endregion
		#region Properties
		public ParsedExpression ParsedExpression { get { return parsedExpression; } }
		public int NumberOfNames {
			get { return numberOfNames; }
			set {
				ValueChecker.CheckValue(value, 0, ushort.MaxValue, "NumberOfNames");
				numberOfNames = value;
			}
		}
		#endregion
		public void SetParsedExpression(ParsedExpression expression, IRPNContext context) {
			if (expression == null)
				expression = new ParsedExpression();
			this.parsedExpression = expression;
			if (expression.Count > 0) {
				byte[] buf = context.ExpressionToBinary(expression);
				int size = BitConverter.ToUInt16(buf, 0);
				this.formulaBytes = new byte[size];
				Array.Copy(buf, 2, this.formulaBytes, 0, size);
			}
			else
				this.formulaBytes = new byte[0];
		}
		protected override void ApplyPivotCacheContent(XlsContentBuilder contentBuilder) {
			if (contentBuilder == null)
				return;
			IXlsPivotCacheBuilder pivotCacheBuilder = contentBuilder.CurrentPivotCacheBuilder;
			int count = contentBuilder.DocumentModel.PivotCaches.Last.CacheFields.Count;
			if (count == 0)
				pivotCacheBuilder.CalculatedItems.Add(new XlsPivotCacheCalculatedItem(parsedExpression));
			else
				pivotCacheBuilder.CalculatedFields.Add(new XlsPivotCacheCalculatedField(parsedExpression, count - 1));
		}
		protected override void ReadCore(XlsReader reader, XlsContentBuilder contentBuilder) {
			int formulaSize = reader.ReadUInt16();
			numberOfNames = reader.ReadUInt16();
			this.formulaBytes = reader.ReadBytes(formulaSize);
			this.parsedExpression = contentBuilder.RPNContext.BinaryToExpression(formulaBytes, formulaSize);
		}
		protected override void WriteCore(BinaryWriter writer) {
			writer.Write((ushort)formulaBytes.Length);
			writer.Write((ushort)numberOfNames);
			writer.Write(formulaBytes);
		}
		protected override short GetSize() {
			return (short)(formulaBytes.Length + 4);
		}
		public override IXlsCommand GetInstance() {
			return this;
		}
	}
	#endregion
	#region XlsCommandPivotCacheItemName
	public class XlsCommandPivotCacheItemName : XlsCommandBase {
		int fieldIndex;
		int pairCount;
		#region Properties
		public bool IsErrorName { get; set; }
		public int FieldIndex {
			get { return fieldIndex; }
			set {
				ValueChecker.CheckValue(value, -1, short.MaxValue, "FieldIndex");
				fieldIndex = value;
			}
		}
		public int PairCount {
			get { return pairCount; }
			set {
				ValueChecker.CheckValue(value, 0, ushort.MaxValue, "PairCount");
				pairCount = value;
			}
		}
		#endregion
		protected override void ReadCore(XlsReader reader, XlsContentBuilder contentBuilder) {
			ushort bitwiseField = reader.ReadUInt16();
			IsErrorName = (bitwiseField & 0x0002) != 0;
			FieldIndex = reader.ReadInt16();
			reader.ReadInt16(); 
			PairCount = reader.ReadUInt16();
		}
		protected override void ApplyPivotCacheContent(XlsContentBuilder contentBuilder) {
			if (contentBuilder == null)
				return;
			if (contentBuilder.DocumentModel.PivotCaches.Last.CacheFields.Count != 0) {
				List<XlsPivotCacheCalculatedField> calculatedFields = contentBuilder.CurrentPivotCacheBuilder.CalculatedFields;
				int count = calculatedFields.Count;
				if (count > 0)
					calculatedFields[count - 1].CacheFieldIndexes.Add(fieldIndex);
			}
			else {
				List<XlsPivotCacheCalculatedItem> calculatedItems = contentBuilder.CurrentPivotCacheBuilder.CalculatedItems;
				int count = calculatedItems.Count;
				if (count > 0)
					calculatedItems[count - 1].ItemPairCollections.Add(new ItemPairCollection());
			}
		}
		protected override void WriteCore(BinaryWriter writer) {
			ushort bitwiseField = 0;
			if(IsErrorName)
				bitwiseField |= 0x0002;
			writer.Write(bitwiseField);
			writer.Write((short)FieldIndex);
			writer.Write((short)-1); 
			writer.Write((ushort)PairCount);
		}
		protected override short GetSize() {
			return 8;
		}
		public override IXlsCommand GetInstance() {
			return this;
		}
	}
	#endregion
	#region XlsCommandPivotCacheItemPair
	public class XlsCommandPivotCacheItemPair : XlsCommandBase {
		int fieldIndex;
		int itemIndex;
		#region Properties
		public int FieldIndex {
			get { return fieldIndex; }
			set {
				ValueChecker.CheckValue(value, 0, ushort.MaxValue, "FieldIndex");
				fieldIndex = value;
			}
		}
		public int ItemIndex {
			get { return itemIndex; }
			set {
				ValueChecker.CheckValue(value, short.MinValue, short.MaxValue, "ItemIndex");
				itemIndex = value;
			}
		}
		public bool IsCalculatedItem { get; set; }
		public bool IsPosition { get; set; }
		public bool IsRelative { get; set; }
		#endregion
		protected override void ReadCore(XlsReader reader, XlsContentBuilder contentBuilder) {
			FieldIndex = reader.ReadUInt16();
			ItemIndex = reader.ReadInt16();
			reader.ReadInt16(); 
			ushort bitwiseField = reader.ReadUInt16();
			IsCalculatedItem = (bitwiseField & 0x0001) != 0;
			IsPosition = (bitwiseField & 0x0008) != 0;
			IsRelative = (bitwiseField & 0x0010) != 0;
		}
		protected override void ApplyPivotCacheContent(XlsContentBuilder contentBuilder) {
			if (contentBuilder == null)
				return;
			List<XlsPivotCacheCalculatedItem> calculatedItems = contentBuilder.CurrentPivotCacheBuilder.CalculatedItems;
			int count = calculatedItems.Count;
			if (count > 0) {
				XlsPivotCacheCalculatedItem calculatedItem = calculatedItems[count - 1];
				int collectionsCount = calculatedItem.ItemPairCollections.Count;
				if (collectionsCount > 0) {
					ItemPair itemPair = new ItemPair(fieldIndex, itemIndex);
					itemPair.IsCalculatedItem = IsCalculatedItem;
					itemPair.IsPosition = IsPosition;
					itemPair.IsRelative = IsRelative;
					calculatedItem.ItemPairCollections[collectionsCount - 1].Add(itemPair);
				}
			}
		}
		protected override void WriteCore(BinaryWriter writer) {
			writer.Write((ushort)FieldIndex);
			writer.Write((short)ItemIndex);
			writer.Write((ushort)0); 
			ushort bitwiseField = 0;
			if(IsCalculatedItem)
				bitwiseField |= 0x0001;
			if(IsPosition)
				bitwiseField |= 0x0008;
			if(IsRelative)
				bitwiseField |= 0x0010;
			writer.Write(bitwiseField);
		}
		protected override short GetSize() {
			return 8;
		}
		public override IXlsCommand GetInstance() {
			return this;
		}
	}
	#endregion
	#region XlsCommandPivotCacheCalculatedItem
	public class XlsCommandPivotCacheCalculatedItem : XlsCommandBase {
		int fieldIndex;
		#region Properties
		public int FieldIndex {
			get { return fieldIndex; }
			set {
				ValueChecker.CheckValue(value, -1, short.MaxValue, "FieldIndex");
				fieldIndex = value;
			}
		}
		#endregion
		protected override void ReadCore(XlsReader reader, XlsContentBuilder contentBuilder) {
			reader.ReadUInt16(); 
			FieldIndex = reader.ReadInt16();
		}
		protected override void WriteCore(BinaryWriter writer) {
			writer.Write((ushort)0); 
			writer.Write((short)FieldIndex);
		}
		protected override short GetSize() {
			return 4;
		}
		public override IXlsCommand GetInstance() {
			return this;
		}
	}
	#endregion
	#region XlsCommandPivotCacheFieldProperties
	public class XlsCommandPivotCacheFieldProperties : XlsCommandBase {
		int parentIndex;
		int baseIndex;
		int numberOfItemsInThisField;
		int numberOfItemsInBaseField;
		int numberOfCacheItems;
		XLUnicodeString fieldName = new XLUnicodeString();
		int uniqueCountItems;
		#region Properties
		public bool AllAtoms { get; set; }
		public bool HasGroupAtoms { get; set; }
		public bool HasParent { get; set; }
		public bool RangeGroup { get; set; }
		public bool IsNumericField { get; set; }
		public bool ContainsTextData { get; set; }
		public bool MinMaxValid { get; set; }
		public bool MoreThan255Items { get; set; }
		public bool HasDates { get; set; }
		public bool IsServerBased { get; set; }
		public bool CantGetUniqueItems { get; set; }
		public bool IsCalculatedField { get; set; }
		public int ParentIndex {
			get { return parentIndex; }
			set {
				ValueChecker.CheckValue(value, 0, ushort.MaxValue, "ParentIndex");
				parentIndex = value;
			}
		}
		public int BaseIndex {
			get { return baseIndex; }
			set {
				ValueChecker.CheckValue(value, 0, ushort.MaxValue, "BaseIndex");
				baseIndex = value;
			}
		}
		public int NumberOfItemsInThisField {
			get { return numberOfItemsInThisField; }
			set {
				ValueChecker.CheckValue(value, 0, ushort.MaxValue, "NumberOfItemsInThisField");
				numberOfItemsInThisField = value;
			}
		}
		public int NumberOfItemsInBaseField {
			get { return numberOfItemsInBaseField; }
			set {
				ValueChecker.CheckValue(value, 0, ushort.MaxValue, "NumberOfItemsInBaseField");
				numberOfItemsInBaseField = value;
			}
		}
		public int NumberOfCacheItems {
			get { return numberOfCacheItems; }
			set {
				ValueChecker.CheckValue(value, 0, ushort.MaxValue, "NumberOfCacheItems");
				numberOfCacheItems = value;
			}
		}
		public int UniqueCountItems { get { return uniqueCountItems; } set { uniqueCountItems = value; } }
		public string FieldName {
			get { return fieldName.Value; }
			set {
				ValueChecker.CheckLength(value, 255, "FieldName");
				fieldName.Value = value;
			}
		}
		bool IsGroupField { get { return RangeGroup && AllAtoms && NumberOfCacheItems == 0; } }
		#endregion
		protected override void ReadCore(XlsReader reader, XlsContentBuilder contentBuilder) {
			ushort bitwiseField = reader.ReadUInt16();
			AllAtoms = (bitwiseField & 0x0001) != 0;
			HasParent = (bitwiseField & 0x0008) != 0;
			RangeGroup = (bitwiseField & 0x0010) != 0;
			IsNumericField = (bitwiseField & 0x0020) != 0;
			ContainsTextData = (bitwiseField & 0x0080) != 0;
			MinMaxValid = (bitwiseField & 0x0100) != 0;
			MoreThan255Items = (bitwiseField & 0x0200) != 0;
			HasDates = (bitwiseField & 0x0800) != 0;
			IsServerBased = (bitwiseField & 0x2000) != 0;
			CantGetUniqueItems = (bitwiseField & 0x4000) != 0;
			IsCalculatedField = (bitwiseField & 0x8000) != 0;
			ParentIndex = reader.ReadUInt16();
			BaseIndex = reader.ReadUInt16();
			reader.ReadUInt16(); 
			NumberOfItemsInThisField = reader.ReadUInt16();
			NumberOfItemsInBaseField = reader.ReadUInt16();
			NumberOfCacheItems = reader.ReadUInt16();
			fieldName = XLUnicodeString.FromStream(reader);
		}
		protected override void ApplyPivotCacheContent(XlsContentBuilder contentBuilder) {
			if (contentBuilder == null)
				return;
			DocumentModel documentModel = contentBuilder.DocumentModel;
			PivotCache pivotCache = documentModel.PivotCaches.Last;
			PivotCacheFieldsCollection cacheFields = pivotCache.CacheFields;
			int fieldIndex = cacheFields.Count;
			if (!AllAtoms)
				contentBuilder.CurrentPivotCacheBuilder.EmptyCacheFieldIndexes.Add(fieldIndex);
			contentBuilder.NumberOfPivotCacheMappingItems = numberOfItemsInBaseField;
			contentBuilder.CurrentPivotCacheBuilder.ResetRecordsCount(NumberOfItemsInThisField, NumberOfCacheItems);
			contentBuilder.CurrentPivotCacheBuilder.SharedItemsCount.Add(NumberOfCacheItems);
			PivotCacheField cacheField = new PivotCacheField(documentModel);
			if (HasParent)
				cacheField.FieldGroup.Parent = ParentIndex;
			if (BaseIndex != 0xFFFF)
				cacheField.FieldGroup.FieldBase = BaseIndex;
			cacheField.Name = FieldName;
			if (IsCalculatedField)
				cacheField.DatabaseField = false;
			else
				cacheField.DatabaseField = (!IsGroupField && (NumberOfItemsInThisField == 0 || NumberOfItemsInBaseField == 0));
			cacheField.ServerField = IsServerBased;
			cacheField.UniqueList = !CantGetUniqueItems; 
			cacheFields.Add(cacheField);
		}
		protected override void WriteCore(BinaryWriter writer) {
			ushort bitwiseField = 0;
			if (AllAtoms) 
				bitwiseField |= 0x0001;
			if (HasGroupAtoms)
				bitwiseField |= 0x0001;
			if(HasParent)
				bitwiseField |= 0x0008;
			if(RangeGroup)
				bitwiseField |= 0x0010;
			if(IsNumericField)
				bitwiseField |= 0x0020;
			if(ContainsTextData)
				bitwiseField |= 0x0080;
			if(MinMaxValid)
				bitwiseField |= 0x0100;
			if(MoreThan255Items)
				bitwiseField |= 0x0200;
			if (HasDates)
				bitwiseField |= 0x0800;
			else if (HasGroupAtoms || RangeGroup || !HasDates)
				bitwiseField |= 0x0400;
			if(IsServerBased)
				bitwiseField |= 0x2000;
			if(CantGetUniqueItems)
				bitwiseField |= 0x4000;
			if(IsCalculatedField)
				bitwiseField |= 0x8000;
			if (AllAtoms || HasGroupAtoms)
				bitwiseField &= 0xFFFD;
			writer.Write(bitwiseField);
			writer.Write((ushort)ParentIndex);
			writer.Write((ushort)BaseIndex);
			if (NumberOfItemsInThisField != 0)
				writer.Write((ushort)NumberOfItemsInThisField); 
			else
				writer.Write((ushort)UniqueCountItems); 
			writer.Write((ushort)NumberOfItemsInThisField);
			writer.Write((ushort)NumberOfItemsInBaseField);
			writer.Write((ushort)NumberOfCacheItems);
			fieldName.Write(writer);
		}
		protected override short GetSize() {
			return (short)(14 + fieldName.Length);
		}
		public override IXlsCommand GetInstance() {
			return this;
		}
	}
	#endregion
	#region XlsCommandPivotCacheFieldDataType
	public class XlsCommandPivotCacheFieldDataType : XlsCommandBase {
		public SqlDataType DataType { get; set; }
		protected override void ReadCore(XlsReader reader, XlsContentBuilder contentBuilder) {
			DataType = (SqlDataType)reader.ReadInt16();
		}
		protected override void WriteCore(BinaryWriter writer) {
			writer.Write((short)DataType);
		}
		public override IXlsCommand GetInstance() {
			return this;
		}
	}
	#endregion
	#region XlsCommandPivotCacheValueNil
	public class XlsCommandPivotCacheValueNil : XlsCommandBase {
		protected override void ReadCore(XlsReader reader, XlsContentBuilder contentBuilder) {
			if (Size > 0)
				reader.ReadBytes(Size);
		}
		protected override void WriteCore(BinaryWriter writer) {
		}
		protected override void ApplyPivotCacheContent(XlsContentBuilder contentBuilder) {
			if (contentBuilder == null)
				return;
			contentBuilder.CurrentPivotCacheBuilder.AddRecord(contentBuilder, new PivotCacheRecordEmptyValue());
		}
		protected override short GetSize() {
			return 0;
		}
		public override IXlsCommand GetInstance() {
			return this;
		}
	}
	#endregion
	#region XlsCommandPivotCacheValueNum
	public class XlsCommandPivotCacheValueNum : XlsCommandDoublePropertyValueBase {
		public override IXlsCommand GetInstance() {
			return this;
		}
		protected override void ApplyPivotCacheContent(XlsContentBuilder contentBuilder) {
			if (contentBuilder == null)
				return;
			contentBuilder.CurrentPivotCacheBuilder.AddRecord(contentBuilder, new PivotCacheRecordNumericValue(Value));
		}
	}
	#endregion
	#region XlsCommandPivotCacheValueBool
	public class XlsCommandPivotCacheValueBool : XlsCommandBoolPropertyBase {
		public override IXlsCommand GetInstance() {
			return this;
		}
		protected override void ApplyPivotCacheContent(XlsContentBuilder contentBuilder) {
			if (contentBuilder == null)
				return;
			contentBuilder.CurrentPivotCacheBuilder.AddRecord(contentBuilder, new PivotCacheRecordBooleanValue(Value));
		}
	}
	#endregion
	#region XlsCommandPivotCacheValueErr
	public class XlsCommandPivotCacheValueErr : XlsCommandBase {
		public VariantValue Value { get; set; }
		protected override void ReadCore(XlsReader reader, XlsContentBuilder contentBuilder) {
			Value = ErrorConverter.ErrorCodeToValue(reader.ReadUInt16());
		}
		protected override void WriteCore(BinaryWriter writer) {
			writer.Write((ushort)(byte)ErrorConverter.ValueToErrorCode(Value));
		}
		public override IXlsCommand GetInstance() {
			return this;
		}
		protected override void ApplyPivotCacheContent(XlsContentBuilder contentBuilder) {
			if (contentBuilder == null)
				return;
			contentBuilder.CurrentPivotCacheBuilder.AddRecord(contentBuilder, new PivotCacheRecordErrorValue(Value.ErrorValue));
		}
	}
	#endregion
	#region XlsCommandPivotCacheValueDateTime
	public class XlsCommandPivotCacheValueDateTime : XlsCommandBase {
		DateTime innerValue = VariantValue.BaseDate;
		public DateTime Value {
			get { return innerValue; }
			set {
				if (value < VariantValue.BaseDate || value.Year > 9999)
					throw new ArgumentOutOfRangeException();
				innerValue = value;
			}
		}
		protected override void ReadCore(XlsReader reader, XlsContentBuilder contentBuilder) {
			int year = reader.ReadUInt16();
			int month = reader.ReadUInt16();
			int day = reader.ReadByte();
			int hour = reader.ReadByte();
			int minute = reader.ReadByte();
			int second = reader.ReadByte();
			if (day == 0) {
				if (year != 1900)
					contentBuilder.ThrowInvalidFile("Invalid pivot cache date/time value");
				innerValue = VariantValue.BaseDate.AddHours(hour).AddMinutes(minute).AddSeconds(second);
			}
			else {
				innerValue = new DateTime(year, month, day, hour, minute, second);
			}
		}
		protected override void WriteCore(BinaryWriter writer) {
			if (innerValue.Date == VariantValue.BaseDate) {
				writer.Write((ushort)1900);
				writer.Write((ushort)1);
				writer.Write((byte)0);
			}
			else {
				writer.Write((ushort)innerValue.Year);
				writer.Write((ushort)innerValue.Month);
				writer.Write((byte)innerValue.Day);
			}
			writer.Write((byte)innerValue.Hour);
			writer.Write((byte)innerValue.Minute);
			writer.Write((byte)innerValue.Second);
		}
		protected override short GetSize() {
			return 8;
		}
		public override IXlsCommand GetInstance() {
			return this;
		}
		protected override void ApplyPivotCacheContent(XlsContentBuilder contentBuilder) {
			if (contentBuilder == null)
				return;
			contentBuilder.CurrentPivotCacheBuilder.AddRecord(contentBuilder, new PivotCacheRecordDateTimeValue(Value));
		}
	}
	#endregion
	#region XlsCommandPivotCacheValueInt
	public class XlsCommandPivotCacheValueInt : XlsCommandShortPropertyValueBase {
		public override IXlsCommand GetInstance() {
			return this;
		}
		protected override void ApplyPivotCacheContent(XlsContentBuilder contentBuilder) {
			if (contentBuilder == null)
				return;
			contentBuilder.CurrentPivotCacheBuilder.AddRecord(contentBuilder, new PivotCacheRecordNumericValue(Value));
		}
	}
	#endregion
	#region XlsCommandPivotCacheRangeGroup
	public class XlsCommandPivotCacheRangeGroup : XlsCommandBase {
		#region Properties
		public bool AutoStart { get; set; }
		public bool AutoStop { get; set; }
		public PivotCacheGroupValuesBy GroupValuesBy { get; set; }
		#endregion
		protected override void ReadCore(XlsReader reader, XlsContentBuilder contentBuilder) {
			ushort bitwiseField = reader.ReadUInt16();
			AutoStart = (bitwiseField & 0x0001) != 0;
			AutoStop = (bitwiseField & 0x0002) != 0;
			GroupValuesBy = CodeToGroupBy((bitwiseField & 0x01c) >> 2);
		}
		protected override void WriteCore(BinaryWriter writer) {
			ushort bitwiseField = (ushort)(GroupByToCode(GroupValuesBy) << 2);
			if (AutoStart)
				bitwiseField |= 0x0001;
			if (AutoStop)
				bitwiseField |= 0x0002;
			writer.Write(bitwiseField);
		}
		public override IXlsCommand GetInstance() {
			return this;
		}
		protected override void ApplyPivotCacheContent(XlsContentBuilder contentBuilder) {
			if (contentBuilder == null)
				return;
			PivotCacheRangeGroupingProperties properties = contentBuilder.DocumentModel.PivotCaches.Last.CacheFields.Last.FieldGroup.RangeGroupingProperties;
			properties.AutoStart = AutoStart;
			properties.AutoEnd = AutoStop;
			properties.GroupBy = GroupValuesBy;
			contentBuilder.CurrentPivotCacheBuilder.ResetGroupingDataCount();
		}
		PivotCacheGroupValuesBy CodeToGroupBy(int code) {
			switch(code) {
				case 1: return PivotCacheGroupValuesBy.Seconds;
				case 2: return PivotCacheGroupValuesBy.Minutes;
				case 3: return PivotCacheGroupValuesBy.Hours;
				case 4: return PivotCacheGroupValuesBy.Days;
				case 5: return PivotCacheGroupValuesBy.Months;
				case 6: return PivotCacheGroupValuesBy.Quarters;
				case 7: return PivotCacheGroupValuesBy.Years;
			}
			return PivotCacheGroupValuesBy.NumericRanges;
		}
		int GroupByToCode(PivotCacheGroupValuesBy groupBy) {
			switch (groupBy) {
				case PivotCacheGroupValuesBy.Seconds: return 1;
				case PivotCacheGroupValuesBy.Minutes: return 2;
				case PivotCacheGroupValuesBy.Hours: return 3;
				case PivotCacheGroupValuesBy.Days: return 4;
				case PivotCacheGroupValuesBy.Months: return 5;
				case PivotCacheGroupValuesBy.Quarters: return 6;
				case PivotCacheGroupValuesBy.Years: return 7;
			}
			return 0;
		}
	}
	#endregion
	#region XlsCommandPivotCacheMapping
	public class XlsCommandPivotCacheMapping : XlsCommandRecordBase {
		static short[] typeCodes = new short[] {
			0x003c 
		};
		readonly List<int> items = new List<int>();
		public List<int> Items { get { return items; } }
		protected override void ReadCore(XlsReader reader, XlsContentBuilder contentBuilder) {
			items.Clear();
			using (XlsCommandStream contentStream = new XlsCommandStream(contentBuilder, reader, typeCodes, Size)) {
				using (BinaryReader contentReader = new BinaryReader(contentStream)) {
					for (int i = 0; i < contentBuilder.NumberOfPivotCacheMappingItems; i++)
						items.Add(contentReader.ReadUInt16());
				}
			}
		}
		protected override void ApplyPivotCacheContent(XlsContentBuilder contentBuilder) {
			if (contentBuilder == null)
				return;
			PivotCacheDiscreteGroupingProperties properties = contentBuilder.DocumentModel.PivotCaches.Last.CacheFields.Last.FieldGroup.DiscreteGroupingProperties;
			foreach (int index in Items) {
				PivotCacheRecordSharedItemsIndexValue value = new PivotCacheRecordSharedItemsIndexValue(index);
				properties.Add(value);
			}
		}
		protected override void CheckPosition(XlsReader reader, XlsContentBuilder contentBuilder, long initialPosition, long expectedPosition) {
		}
		public override IXlsCommand GetInstance() {
			return this;
		}
	}
	#endregion
	#region XlsCommandPivotCacheItemIndexes
	public class XlsCommandPivotCacheItemIndexes : XlsCommandBase {
		readonly List<XlsPivotCacheSharedIndex> indexes = new List<XlsPivotCacheSharedIndex>();
		public List<XlsPivotCacheSharedIndex> Indexes { get { return indexes; } }
		protected override void ReadCore(XlsReader reader, XlsContentBuilder contentBuilder) {
			indexes.Clear();
			List<int> sharedItemsCounts = contentBuilder.CurrentPivotCacheBuilder.SharedItemsCount;
			for (int i = 0; i < sharedItemsCounts.Count; ++i) {
				int sharedItemsCount = sharedItemsCounts[i];
				if (sharedItemsCount == 0)
					continue;
				XlsPivotCacheSharedIndex indexItem;
				if (sharedItemsCount > 255)
					indexItem = new XlsPivotCacheSharedIndex(reader.ReadUInt16(), false);
				else
					indexItem = new XlsPivotCacheSharedIndex(reader.ReadByte(), true);
				indexes.Add(indexItem);
			}
		}
		protected override void WriteCore(BinaryWriter writer) {
			foreach (XlsPivotCacheSharedIndex indexItem in Indexes) {
				int index = indexItem.Index;
				if (indexItem.IsShort)
					writer.Write((byte)index);
				else
					writer.Write((ushort)index);
			}
		}
		protected override void ApplyPivotCacheContent(XlsContentBuilder contentBuilder) {
			if (contentBuilder == null)
				return;
			foreach (XlsPivotCacheSharedIndex indexItem in Indexes) {
				PivotCacheRecordSharedItemsIndexValue value = new PivotCacheRecordSharedItemsIndexValue(indexItem.Index);
				contentBuilder.CurrentPivotCacheBuilder.AddRecord(contentBuilder, value);
			}
		}
		protected override short GetSize() {
			int size = 0;
			foreach (XlsPivotCacheSharedIndex indexItem in Indexes)
				size += indexItem.IsShort ? 1 : 2;
			return (short)size;
		}
		public override IXlsCommand GetInstance() {
			return this;
		}
	}
	public class XlsPivotCacheSharedIndex {
		int index;
		bool isShort;
		public XlsPivotCacheSharedIndex(int index, bool isShort) {
			this.index = index;
			this.isShort = isShort;
		}
		public int Index { get { return index; } }
		public bool IsShort { get { return isShort; } }
	}
	#endregion
}
