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
using System.Runtime.InteropServices;
using DevExpress.Utils;
using DevExpress.Office;
using DevExpress.Office.History;
using DevExpress.Office.Utils;
using DevExpress.XtraSpreadsheet.Model.History;
using DevExpress.XtraSpreadsheet.Utils;
namespace DevExpress.XtraSpreadsheet.Model {
	#region PictureFormat
	public enum PictureFormat {
		Stretch = 0,
		Stack = 1,
		StackScale = 2
	}
	#endregion
	#region PictureOptionsInfo
	public class PictureOptionsInfo : ICloneable<PictureOptionsInfo>, ISupportsCopyFrom<PictureOptionsInfo>, ISupportsSizeOf {
		#region Fields
		const int offsetPictureFormat = 4;
		const uint maskApplyToEnd = 0x0001;
		const uint maskApplyToFront = 0x0002;
		const uint maskApplyToSides = 0x0004;
		const uint maskPictureFormat = 0x0030;
		uint packedValues = 0x0007;
		#endregion
		#region Properties
		public bool ApplyToEnd {
			get { return PackedValues.GetBoolBitValue(this.packedValues, maskApplyToEnd); }
			set { PackedValues.SetBoolBitValue(ref this.packedValues, maskApplyToEnd, value); }
		}
		public bool ApplyToFront {
			get { return PackedValues.GetBoolBitValue(this.packedValues, maskApplyToFront); }
			set { PackedValues.SetBoolBitValue(ref this.packedValues, maskApplyToFront, value); }
		}
		public bool ApplyToSides {
			get { return PackedValues.GetBoolBitValue(this.packedValues, maskApplyToSides); }
			set { PackedValues.SetBoolBitValue(ref this.packedValues, maskApplyToSides, value); }
		}
		public PictureFormat PictureFormat {
			get { return (PictureFormat)PackedValues.GetIntBitValue(this.packedValues, maskPictureFormat, offsetPictureFormat); }
			set { PackedValues.SetIntBitValue(ref this.packedValues, maskPictureFormat, offsetPictureFormat, (int)value); }
		}
		#endregion
		#region ICloneable<PictureOptionsInfo> Members
		public PictureOptionsInfo Clone() {
			PictureOptionsInfo result = new PictureOptionsInfo();
			result.CopyFrom(this);
			return result;
		}
		#endregion
		#region ISupportsCopyFrom<PictureOptionsInfo> Members
		public void CopyFrom(PictureOptionsInfo value) {
			Guard.ArgumentNotNull(value, "value");
			this.packedValues = value.packedValues;
		}
		#endregion
		#region ISupportsSizeOf Members
		public int SizeOf() {
			return DXMarshal.SizeOf(GetType());
		}
		#endregion
		public override bool Equals(object obj) {
			PictureOptionsInfo other = obj as PictureOptionsInfo;
			if(other == null)
				return false;
			return this.packedValues == other.packedValues;
		}
		public override int GetHashCode() {
			return (int)this.packedValues;
		}
	}
	#endregion
	#region PictureOptionsInfoCache
	public class PictureOptionsInfoCache : UniqueItemsCache<PictureOptionsInfo> {
		public PictureOptionsInfoCache(IDocumentModelUnitConverter converter)
			: base(converter) {
		}
		protected override PictureOptionsInfo CreateDefaultItem(IDocumentModelUnitConverter unitConverter) {
			return new PictureOptionsInfo();
		}
	}
	#endregion
	#region PictureOptions
	public class PictureOptions : SpreadsheetUndoableIndexBasedObject<PictureOptionsInfo>, ICloneable<PictureOptions>, ISupportsCopyFrom<PictureOptions> {
		#region Fields
		readonly IChart parent;
		double pictureStackUnit;
		#endregion
		public PictureOptions(IChart parent) 
			: base(parent.DocumentModel) {
			this.parent = parent;
			this.pictureStackUnit = 1.0;
		}
		#region Properties
		protected internal IChart Parent { get { return parent; } }
		#region ApplyToEnd
		public bool ApplyToEnd {
			get { return Info.ApplyToEnd; }
			set {
				if(ApplyToEnd == value)
					return;
				SetPropertyValue(SetApplyToEndCore, value);
			}
		}
		DocumentModelChangeActions SetApplyToEndCore(PictureOptionsInfo info, bool value) {
			info.ApplyToEnd = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region ApplyToFront
		public bool ApplyToFront {
			get { return Info.ApplyToFront; }
			set {
				if(ApplyToFront == value)
					return;
				SetPropertyValue(SetApplyToFrontCore, value);
			}
		}
		DocumentModelChangeActions SetApplyToFrontCore(PictureOptionsInfo info, bool value) {
			info.ApplyToFront = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region ApplyToSides
		public bool ApplyToSides {
			get { return Info.ApplyToSides; }
			set {
				if(ApplyToSides == value)
					return;
				SetPropertyValue(SetApplyToSidesCore, value);
			}
		}
		DocumentModelChangeActions SetApplyToSidesCore(PictureOptionsInfo info, bool value) {
			info.ApplyToSides = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region PictureFormat
		public PictureFormat PictureFormat {
			get { return Info.PictureFormat; }
			set {
				if(PictureFormat == value)
					return;
				SetPropertyValue(SetPictureFormatCore, value);
			}
		}
		DocumentModelChangeActions SetPictureFormatCore(PictureOptionsInfo info, PictureFormat value) {
			info.PictureFormat = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region PictureStackUnit
		public double PictureStackUnit {
			get { return pictureStackUnit; }
			set {
				if(value <= 0.0)
					throw new ArgumentOutOfRangeException("PictureStackUnit less or equal to 0");
				if(pictureStackUnit == value)
					return;
				SetPictureStackUnit(value);
			}
		}
		void SetPictureStackUnit(double value) {
			PictureOptionsStackUnitPropertyChangedHistoryItem historyItem = new PictureOptionsStackUnitPropertyChangedHistoryItem(DocumentModel, this, pictureStackUnit, value);
			DocumentModel.History.Add(historyItem);
			historyItem.Execute();
		}
		protected internal void SetPictureStackUnitCore(double value) {
			pictureStackUnit = value;
			Parent.Invalidate();
		}
		#endregion
		#endregion
		#region SpreadsheetUndoableIndexBasedObject members
		public override DocumentModelChangeActions GetBatchUpdateChangeActions() {
			return DocumentModelChangeActions.None;
		}
		protected override UniqueItemsCache<PictureOptionsInfo> GetCache(IDocumentModel documentModel) {
			return DocumentModel.Cache.PictureOptionsInfoCache;
		}
		protected override void ApplyChanges(DocumentModelChangeActions changeActions) {
			DocumentModel.ApplyChanges(changeActions);
			Parent.Invalidate();
		}
		#endregion
		#region ICloneable<PictureOptions> Members
		public PictureOptions Clone() {
			PictureOptions result = new PictureOptions(Parent);
			result.CopyFrom(this);
			return result;
		}
		#endregion
		#region ISupportsCopyFrom<PictureOptions> Members
		public void CopyFrom(PictureOptions value) {
			base.CopyFrom(value);
			this.pictureStackUnit = value.pictureStackUnit;
		}
		#endregion
	}
	#endregion
}
