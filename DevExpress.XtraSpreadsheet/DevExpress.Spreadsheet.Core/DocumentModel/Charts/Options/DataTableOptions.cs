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
	#region DataTableInfo
	public class DataTableInfo : ICloneable<DataTableInfo>, ISupportsCopyFrom<DataTableInfo>, ISupportsSizeOf {
		#region Fields
		const uint maskShowHorizontalBorder = 0x0001;
		const uint maskShowVerticalBorder = 0x0002;
		const uint maskShowOutline = 0x0004;
		const uint maskShowLegendKeys = 0x0008;
		const uint maskVisible = 0x0010;
		uint packedValues = 0x000f;
		#endregion
		#region Properties
		public bool ShowHorizontalBorder {
			get { return PackedValues.GetBoolBitValue(this.packedValues, maskShowHorizontalBorder); }
			set { PackedValues.SetBoolBitValue(ref this.packedValues, maskShowHorizontalBorder, value); }
		}
		public bool ShowVerticalBorder {
			get { return PackedValues.GetBoolBitValue(this.packedValues, maskShowVerticalBorder); }
			set { PackedValues.SetBoolBitValue(ref this.packedValues, maskShowVerticalBorder, value); }
		}
		public bool ShowOutline {
			get { return PackedValues.GetBoolBitValue(this.packedValues, maskShowOutline); }
			set { PackedValues.SetBoolBitValue(ref this.packedValues, maskShowOutline, value); }
		}
		public bool ShowLegendKeys {
			get { return PackedValues.GetBoolBitValue(this.packedValues, maskShowLegendKeys); }
			set { PackedValues.SetBoolBitValue(ref this.packedValues, maskShowLegendKeys, value); }
		}
		public bool Visible {
			get { return PackedValues.GetBoolBitValue(this.packedValues, maskVisible); }
			set { PackedValues.SetBoolBitValue(ref this.packedValues, maskVisible, value); }
		}
		#endregion
		#region ICloneable<DataTableInfo> Members
		public DataTableInfo Clone() {
			DataTableInfo result = new DataTableInfo();
			result.CopyFrom(this);
			return result;
		}
		#endregion
		#region ISupportsCopyFrom<DataTableInfo> Members
		public void CopyFrom(DataTableInfo value) {
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
			DataTableInfo other = obj as DataTableInfo;
			if(other == null)
				return false;
			return this.packedValues == other.packedValues;
		}
		public override int GetHashCode() {
			return (int)this.packedValues;
		}
	}
	#endregion
	#region DataTableInfoCache
	public class DataTableInfoCache : UniqueItemsCache<DataTableInfo> {
		public DataTableInfoCache(IDocumentModelUnitConverter converter)
			: base(converter) {
		}
		protected override DataTableInfo CreateDefaultItem(IDocumentModelUnitConverter unitConverter) {
			return new DataTableInfo();
		}
	}
	#endregion
	#region DataTableOptions
	public class DataTableOptions : SpreadsheetUndoableIndexBasedObject<DataTableInfo>, ICloneable<DataTableOptions>, ISupportsCopyFrom<DataTableOptions> {
		#region Fields
		readonly IChart parent;
		readonly ShapeProperties shapeProperties;
		readonly TextProperties textProperties;
		#endregion
		public DataTableOptions(IChart parent)
			: base(parent.DocumentModel) {
			this.parent = parent;
			this.shapeProperties = new ShapeProperties(DocumentModel) { Parent = parent };
			this.textProperties = new TextProperties(DocumentModel) { Parent = parent };
		}
		#region Properties
		protected internal IChart Parent { get { return parent; } }
		#region ShowHorizontalBorder
		public bool ShowHorizontalBorder {
			get { return Info.ShowHorizontalBorder; }
			set { 
				if(ShowHorizontalBorder == value)
					return;
				SetPropertyValue(SetShowHorizontalBorderCore, value);
			}
		}
		DocumentModelChangeActions SetShowHorizontalBorderCore(DataTableInfo info, bool value) {
			info.ShowHorizontalBorder = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region ShowVerticalBorder
		public bool ShowVerticalBorder {
			get { return Info.ShowVerticalBorder; }
			set {
				if(ShowVerticalBorder == value)
					return;
				SetPropertyValue(SetShowVerticalBorderCore, value);
			}
		}
		DocumentModelChangeActions SetShowVerticalBorderCore(DataTableInfo info, bool value) {
			info.ShowVerticalBorder = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region ShowOutline
		public bool ShowOutline {
			get { return Info.ShowOutline; }
			set {
				if(ShowOutline == value)
					return;
				SetPropertyValue(SetShowOutlineCore, value);
			}
		}
		DocumentModelChangeActions SetShowOutlineCore(DataTableInfo info, bool value) {
			info.ShowOutline = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region ShowLegendKeys
		public bool ShowLegendKeys {
			get { return Info.ShowLegendKeys; }
			set {
				if(ShowLegendKeys == value)
					return;
				SetPropertyValue(SetShowLegendKeysCore, value);
			}
		}
		DocumentModelChangeActions SetShowLegendKeysCore(DataTableInfo info, bool value) {
			info.ShowLegendKeys = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region Visible
		public bool Visible {
			get { return Info.Visible; }
			set {
				if(Visible == value)
					return;
				SetPropertyValue(SetVisibleCore, value);
			}
		}
		DocumentModelChangeActions SetVisibleCore(DataTableInfo info, bool value) {
			info.Visible = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		public ShapeProperties ShapeProperties { get { return shapeProperties; } }
		public TextProperties TextProperties { get { return textProperties; } }
		#endregion
		#region SpreadsheetUndoableIndexBasedObject members
		public override DocumentModelChangeActions GetBatchUpdateChangeActions() {
			return DocumentModelChangeActions.None;
		}
		protected override UniqueItemsCache<DataTableInfo> GetCache(IDocumentModel documentModel) {
			return DocumentModel.Cache.DataTableInfoCache;
		}
		protected override void ApplyChanges(DocumentModelChangeActions changeActions) {
			DocumentModel.ApplyChanges(changeActions);
			Parent.Invalidate();
		}
		#endregion
		#region ICloneable<DataTableOptions> Members
		public DataTableOptions Clone() {
			DataTableOptions result = new DataTableOptions(this.parent);
			result.CopyFrom(this);
			return result;
		}
		#endregion
		#region ISupportsCopyFrom<DataTableOptions> Members
		public void CopyFrom(DataTableOptions value) {
			Guard.ArgumentNotNull(value, "value");
			base.CopyFrom(value);
			this.shapeProperties.CopyFrom(value.shapeProperties);
			this.textProperties.CopyFrom(value.textProperties);
		}
		#endregion
		public void ResetToStyle() {
			ShapeProperties.ResetToStyle();
			TextProperties.ResetToStyle();
		}
	}
	#endregion
}
