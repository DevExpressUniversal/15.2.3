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
	#region DisplayUnitType
	public enum DisplayUnitType {
		None = 0,
		Billions,
		HundredMillions,
		Hundreds,
		HundredThousands,
		Millions,
		TenMillions,
		TenThousands,
		Thousands,
		Trillions,
		Custom
	}
	#endregion
	#region DisplayUnitInfo
	public class DisplayUnitInfo : ICloneable<DisplayUnitInfo>, ISupportsCopyFrom<DisplayUnitInfo>, ISupportsSizeOf {
		#region Static Members
		readonly static DisplayUnitInfo defaultInfo = new DisplayUnitInfo();
		public static DisplayUnitInfo DefaultInfo { get { return defaultInfo; } }
		#endregion
		#region Fields
		const uint maskUnitType = 0x000f;
		const uint maskShowLabel = 0x0010;
		uint packedValues = 0x0000;
		double customUnit = 1.0;
		#endregion
		#region Properties
		public DisplayUnitType UnitType {
			get { return (DisplayUnitType)PackedValues.GetIntBitValue(this.packedValues, maskUnitType); }
			set { PackedValues.SetIntBitValue(ref this.packedValues, maskUnitType, (int)value); }
		}
		public bool ShowLabel {
			get { return PackedValues.GetBoolBitValue(this.packedValues, maskShowLabel); }
			set { PackedValues.SetBoolBitValue(ref this.packedValues, maskShowLabel, value); }
		}
		public double CustomUnit {
			get { return customUnit; }
			set {
				Guard.ArgumentPositive((float)value, "CustomUnit");
				customUnit = value;
			}
		}
		#endregion
		#region ICloneable<DisplayUnitInfo> Members
		public DisplayUnitInfo Clone() {
			DisplayUnitInfo result = new DisplayUnitInfo();
			result.CopyFrom(this);
			return result;
		}
		#endregion
		#region ISupportsCopyFrom<DisplayUnitInfo> Members
		public void CopyFrom(DisplayUnitInfo value) {
			Guard.ArgumentNotNull(value, "value");
			this.packedValues = value.packedValues;
			this.customUnit = value.customUnit;
		}
		#endregion
		#region ISupportsSizeOf Members
		public int SizeOf() {
			return DXMarshal.SizeOf(GetType());
		}
		#endregion
		public override bool Equals(object obj) {
			DisplayUnitInfo other = obj as DisplayUnitInfo;
			if(other == null)
				return false;
			return this.packedValues == other.packedValues && this.customUnit == other.customUnit;
		}
		public override int GetHashCode() {
			return (int)((int)this.packedValues ^ this.customUnit.GetHashCode());
		}
	}
	#endregion
	#region DisplayUnitInfoCache
	public class DisplayUnitInfoCache : UniqueItemsCache<DisplayUnitInfo> {
		public DisplayUnitInfoCache(IDocumentModelUnitConverter converter)
			: base(converter) {
		}
		protected override DisplayUnitInfo CreateDefaultItem(IDocumentModelUnitConverter unitConverter) {
			return new DisplayUnitInfo();
		}
	}
	#endregion
	#region DisplayUnitOptions
	public class DisplayUnitOptions : SpreadsheetUndoableIndexBasedObject<DisplayUnitInfo>, ICloneable<DisplayUnitOptions>, ISupportsCopyFrom<DisplayUnitOptions>, IChartTextOwnerEx {
		#region Fields
		readonly IChart parent;
		ShapeProperties shapeProperties;
		TextProperties textProperties;
		LayoutOptions layout;
		IChartText text;
		#endregion
		public DisplayUnitOptions(IChart parent) 
			: base(parent.DocumentModel){
			this.parent = parent;
			this.shapeProperties = new ShapeProperties(DocumentModel) { Parent = parent };
			this.textProperties = new TextProperties(DocumentModel) { Parent = parent };
			this.layout = new LayoutOptions(parent);
			this.text = ChartText.Empty;
		}
		#region Properties
		public IChart Parent { get { return parent; } }
		#region UnitType
		public DisplayUnitType UnitType {
			get { return Info.UnitType; }
			set {
				if(UnitType == value)
					return;
				SetPropertyValue(SetUnitTypeCore, value);
			}
		}
		DocumentModelChangeActions SetUnitTypeCore(DisplayUnitInfo info, DisplayUnitType value) {
			info.UnitType = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region ShowLabel
		public bool ShowLabel {
			get { return Info.ShowLabel; }
			set {
				if(ShowLabel == value)
					return;
				SetPropertyValue(SetShowLabelCore, value);
			}
		}
		DocumentModelChangeActions SetShowLabelCore(DisplayUnitInfo info, bool value) {
			info.ShowLabel = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region CustomUnit
		public double CustomUnit {
			get { return Info.CustomUnit; }
			set {
				if(CustomUnit == value)
					return;
				SetPropertyValue(SetCustomUnitCore, value);
			}
		}
		DocumentModelChangeActions SetCustomUnitCore(DisplayUnitInfo info, double value) {
			info.CustomUnit = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		public ShapeProperties ShapeProperties { get { return shapeProperties; } }
		public TextProperties TextProperties { get { return textProperties; } }
		public LayoutOptions Layout { get { return layout; } }
		public IChartText Text {
			get { return text; }
			set {
				if(value == null)
					value = ChartText.Empty;
				if(text.Equals(value))
					return;
				SetText(value);
			}
		}
		void SetText(IChartText value) {
			ChartTextPropertyChangedHistoryItem historyItem = new ChartTextPropertyChangedHistoryItem(DocumentModel, this, text, value);
			DocumentModel.History.Add(historyItem);
			historyItem.Execute();
		}
		public void SetTextCore(IChartText value) {
			text = value;
			Parent.Invalidate();
		}
		#endregion
		#region SpreadsheetUndoableIndexBasedObject members
		public override DocumentModelChangeActions GetBatchUpdateChangeActions() {
			return DocumentModelChangeActions.None;
		}
		protected override UniqueItemsCache<DisplayUnitInfo> GetCache(IDocumentModel documentModel) {
			return DocumentModel.Cache.DisplayUnitInfoCache;
		}
		protected override void ApplyChanges(DocumentModelChangeActions changeActions) {
			DocumentModel.ApplyChanges(changeActions);
			Parent.Invalidate();
		}
		#endregion
		#region ICloneable<DisplayUnitOptions> Members
		public DisplayUnitOptions Clone() {
			DisplayUnitOptions result = new DisplayUnitOptions(this.Parent);
			result.CopyFrom(this);
			return result;
		}
		#endregion
		#region ISupportsCopyFrom<DisplayUnitOptions> Members
		public void CopyFrom(DisplayUnitOptions value) {
			Guard.ArgumentNotNull(value, "value");
			base.CopyFrom(value);
			this.shapeProperties.CopyFrom(value.shapeProperties);
			this.textProperties.CopyFrom(value.textProperties);
			this.layout.CopyFrom(value.layout);
			this.text = value.text.CloneTo(Parent);
		}
		#endregion
		public void ResetToStyle() {
			ShapeProperties.ResetToStyle();
			TextProperties.ResetToStyle();
		}
		#region Notifications
		public void OnRangeInserting(InsertRangeNotificationContext context) {
			Text.OnRangeInserting(context);
		}
		public void OnRangeRemoving(RemoveRangeNotificationContext context) {
			Text.OnRangeRemoving(context);
		}
		#endregion
	}
	#endregion
}
