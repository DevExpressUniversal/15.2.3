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

using DevExpress.XtraSpreadsheet.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
namespace DevExpress.XtraSpreadsheet.Model {
	#region PivotCacheSharedItemsCollectionFlags (enum)
	[Flags]
	public enum PivotCacheSharedItemsCollectionFlags {
		None = 0,
		Date = 1, 
		String = 2, 
		Blank = 4,
		Number = 8,
		Integer = 16, 
		LongText = 32, 
		MixedTypes = 64, 
		NonDate = 128, 
		SemiMixedTypes = 256, 
	}
	#endregion
	#region PivotCacheSharedItemsCollection
	public class PivotCacheSharedItemsCollection : UniqueItemCollection<IPivotCacheRecordValue>, IPivotCacheRecordValueVisitor {
		#region Fields
		PivotCacheSharedItemsCollectionFlags flags;
		double minValue = double.MaxValue;
		double maxValue = double.MinValue;
		DateTime minDate;
		DateTime maxDate;
		#endregion
		public PivotCacheSharedItemsCollection()
			: base(new SharedItemsEqualityComparer()) {
			InitializeDefault();
		}
		#region Properties
		public PivotCacheSharedItemsCollectionFlags Flags { get { return flags; } }
		public bool ContainsDate { get { return (flags & PivotCacheSharedItemsCollectionFlags.Date) > 0; } }
		public bool ContainsString { get { return (flags & PivotCacheSharedItemsCollectionFlags.String) > 0; } }
		public bool ContainsBlank { get { return (flags & PivotCacheSharedItemsCollectionFlags.Blank) > 0; } }
		public bool ContainsNumber { get { return (flags & PivotCacheSharedItemsCollectionFlags.Number) > 0; } }
		public bool ContainsInteger { get { return (flags & PivotCacheSharedItemsCollectionFlags.Integer) > 0; } }
		public bool ContainsLongText { get { return (flags & PivotCacheSharedItemsCollectionFlags.LongText) > 0; } }
		public bool ContainsMixedTypes { get { return (flags & PivotCacheSharedItemsCollectionFlags.MixedTypes) > 0; } }
		public bool ContainsNonDate { get { return (flags & PivotCacheSharedItemsCollectionFlags.NonDate) > 0; } }
		public bool ContainsSemiMixedTypes { get { return (flags & PivotCacheSharedItemsCollectionFlags.SemiMixedTypes) > 0; } }
		public double MinValue { get { return minValue; } }
		public double MaxValue { get { return maxValue; } }
		public DateTime MinDate { get { return minDate; } }
		public DateTime MaxDate { get { return maxDate; } }
		public bool HasMinMaxValues { get { return minValue != double.MaxValue && maxValue != double.MinValue; } }
		public bool ContainsOnlyNumbers {
			get {
				return ((PivotCacheSharedItemsCollectionFlags.Blank | PivotCacheSharedItemsCollectionFlags.String |
						PivotCacheSharedItemsCollectionFlags.Date | PivotCacheSharedItemsCollectionFlags.Number) & flags) == PivotCacheSharedItemsCollectionFlags.Number;
			}
		}
		#endregion
		void InitializeDefault() {
			Initialize(PivotCacheSharedItemsCollectionFlags.None);
		}
		protected internal void Initialize(PivotCacheSharedItemsCollectionFlags flags) {
			Initialize(flags, double.MaxValue, double.MinValue, DateTime.MaxValue, DateTime.MinValue);
		}
		protected internal void Initialize(PivotCacheSharedItemsCollectionFlags flags, double minValue, double maxValue, DateTime minDate, DateTime maxDate) {
			this.flags = flags;
			this.minValue = minValue;
			this.maxValue = maxValue;
			this.minDate = minDate;
			this.maxDate = maxDate;
		}
		protected override void OnItemInserted(int index, IPivotCacheRecordValue item) {
			base.OnItemInserted(index, item);
			item.Visit(this);
		}
		public override void Clear() {
			ClearCore();
			InitializeDefault();
		}
		public void ClearCore() {
			base.Clear();
		}
		#region IPivotCacheRecordValueVisitor
		public void Visit(PivotCacheRecordSharedItemsIndexValue value) {
			DevExpress.Office.Utils.Exceptions.ThrowInvalidOperationException("PivotCacheSharedItemsCollection have to contain only non-sharedItems.");
		}
		public void Visit(PivotCacheRecordDateTimeValue value) {
			VisitDateTime(value.Value);
		}
		public void Visit(PivotCacheRecordEmptyValue value) {
			flags |= PivotCacheSharedItemsCollectionFlags.Blank | PivotCacheSharedItemsCollectionFlags.SemiMixedTypes;
		}
		public void Visit(PivotCacheRecordNumericValue value) {
			VisitNumeric(value.Value);
		}
		public void Visit(PivotCacheRecordBooleanValue value) {
			VisitString();
		}
		public void Visit(PivotCacheRecordErrorValue value) {
			VisitString();
		}
		public void Visit(PivotCacheRecordCharacterValue value) {
			VisitString();
			if (value.Value.Length > 255)
				flags |= PivotCacheSharedItemsCollectionFlags.LongText;
		}
		public void Visit(PivotCacheRecordOrdinalDateTimeValue value) {
			VisitDateTime(value.Value);
		}
		public void Visit(PivotCacheRecordOrdinalEmptyValue value) {
			flags |= PivotCacheSharedItemsCollectionFlags.Blank | PivotCacheSharedItemsCollectionFlags.SemiMixedTypes;
		}
		public void Visit(PivotCacheRecordOrdinalNumericValue value) {
			VisitNumeric(value.Value);
		}
		public void Visit(PivotCacheRecordOrdinalBooleanValue value) {
			VisitString();
		}
		public void Visit(PivotCacheRecordOrdinalErrorValue value) {
			VisitString();
		}
		public void Visit(PivotCacheRecordOrdinalCharacterValue value) {
			VisitString();
			if (value.Value.Length > 255)
				flags |= PivotCacheSharedItemsCollectionFlags.LongText;
		}
		void VisitString() {
			if ((flags & (PivotCacheSharedItemsCollectionFlags.Date | PivotCacheSharedItemsCollectionFlags.Number)) > 0)
				flags |= PivotCacheSharedItemsCollectionFlags.MixedTypes;
			flags |= PivotCacheSharedItemsCollectionFlags.String;
			flags |= PivotCacheSharedItemsCollectionFlags.NonDate;
			flags |= PivotCacheSharedItemsCollectionFlags.SemiMixedTypes;
		}
		void VisitDateTime(DateTime dateTimeValue) {
			if (dateTimeValue < minDate)
				minDate = dateTimeValue;
			if (dateTimeValue > maxDate)
				maxDate = dateTimeValue;
			if ((flags & (PivotCacheSharedItemsCollectionFlags.Number | PivotCacheSharedItemsCollectionFlags.String)) > 0)
				flags |= PivotCacheSharedItemsCollectionFlags.MixedTypes;
			flags |= PivotCacheSharedItemsCollectionFlags.Date;
			flags &= ~(PivotCacheSharedItemsCollectionFlags.Number | PivotCacheSharedItemsCollectionFlags.Integer);
		}
		void VisitNumeric(double numericValue) {
			if (numericValue < minValue)
				minValue = numericValue;
			if (numericValue > maxValue)
				maxValue = numericValue;
			if (ContainsDate)
				flags |= PivotCacheSharedItemsCollectionFlags.MixedTypes;
			else {
				if (ContainsString)
					flags |= PivotCacheSharedItemsCollectionFlags.MixedTypes;
				if (ContainsInteger) {
					if (Math.Truncate(numericValue) != numericValue)
						flags &= ~PivotCacheSharedItemsCollectionFlags.Integer;
				}
				else
					if (!ContainsNumber)
						if (Math.Truncate(numericValue) == numericValue)
							flags |= PivotCacheSharedItemsCollectionFlags.Integer;
				flags |= PivotCacheSharedItemsCollectionFlags.Number;
			}
			flags |= PivotCacheSharedItemsCollectionFlags.NonDate;
		}
		#endregion
		public void CopyFrom(PivotCacheSharedItemsCollection source) {
			this.Capacity = source.Count; 
			Debug.Assert(this.Count == 0);
			foreach (IPivotCacheRecordValue sourceRecordValue in source) {
				IPivotCacheRecordValue target = sourceRecordValue.Clone();
				this.AddWithoutNotification(target);
			}
		}
	}
	#endregion
	#region SharedItemsEqualityComparer
	public class SharedItemsEqualityComparer : IEqualityComparer<IPivotCacheRecordValue> {
		public bool Equals(IPivotCacheRecordValue x, IPivotCacheRecordValue y) {
			if (x.ValueType != y.ValueType)
				return false;
			return x.AreBaseDataEqual(y);
		}
		public int GetHashCode(IPivotCacheRecordValue obj) {
			return obj.GetHashCodeForBaseData();
		}
	}
	#endregion
}
