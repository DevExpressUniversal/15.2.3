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
using System.Globalization;
using System.Diagnostics;
using System.Runtime.InteropServices;
using DevExpress.Utils;
using DevExpress.Office;
using DevExpress.Office.History;
using DevExpress.Office.Utils;
using DevExpress.XtraSpreadsheet.Model.History;
using DevExpress.XtraSpreadsheet.Utils;
namespace DevExpress.XtraSpreadsheet.Model {
	#region ScalingInfo
	public class ScalingInfo : ICloneable<ScalingInfo>, ISupportsCopyFrom<ScalingInfo>, ISupportsSizeOf {
		readonly static ScalingInfo defaultInfo = new ScalingInfo();
		public static ScalingInfo DefaultInfo { get { return defaultInfo; } }
		#region Fields
		const uint maskLogScale = 0x0001;
		const uint maskFixedMax = 0x0002;
		const uint maskFixedMin = 0x0004;
		const uint maskOrientation = 0x0008;
		uint packedValues;
		double logBase = 2.0;
		double min = 0.0;
		double max = 0.0;
		#endregion
		#region Properties
		public bool LogScale {
			get { return PackedValues.GetBoolBitValue(this.packedValues, maskLogScale); }
			set { PackedValues.SetBoolBitValue(ref this.packedValues, maskLogScale, value); }
		}
		public bool FixedMax {
			get { return PackedValues.GetBoolBitValue(this.packedValues, maskFixedMax); }
			set { PackedValues.SetBoolBitValue(ref this.packedValues, maskFixedMax, value); }
		}
		public bool FixedMin {
			get { return PackedValues.GetBoolBitValue(this.packedValues, maskFixedMin); }
			set { PackedValues.SetBoolBitValue(ref this.packedValues, maskFixedMin, value); }
		}
		public AxisOrientation Orientation { 
			get { return PackedValues.GetBoolBitValue(this.packedValues, maskOrientation) ? AxisOrientation.MaxMin : AxisOrientation.MinMax; } 
			set { PackedValues.SetBoolBitValue(ref this.packedValues, maskOrientation, value == AxisOrientation.MaxMin); } 
		}
		public double LogBase {
			get { return logBase; }
			set {
				ValueChecker.CheckValue(value, 2.0, 1000.0, "LogBase");
				logBase = value;
			}
		}
		public double Min { get { return min; } set { min = value; } }
		public double Max { get { return max; } set { max = value; } }
		#endregion
		#region ICloneable<ScalingInfo> Members
		public ScalingInfo Clone() {
			ScalingInfo result = new ScalingInfo();
			result.CopyFrom(this);
			return result;
		}
		#endregion
		#region ISupportsCopyFrom<ScalingInfo> Members
		public void CopyFrom(ScalingInfo value) {
			Guard.ArgumentNotNull(value, "value");
			this.packedValues = value.packedValues;
			this.logBase = value.logBase;
			this.min = value.min;
			this.max = value.max;
		}
		#endregion
		#region ISupportsSizeOf Members
		public int SizeOf() {
			return DXMarshal.SizeOf(GetType());
		}
		#endregion
		public override bool Equals(object obj) {
			ScalingInfo other = obj as ScalingInfo;
			if(other == null)
				return false;
			return this.packedValues == other.packedValues &&
				this.logBase == other.logBase &&
				this.min == other.min &&
				this.max == other.max;
		}
		public override int GetHashCode() {
			return (int)((int)packedValues ^ logBase.GetHashCode() ^ min.GetHashCode() ^ max.GetHashCode());
		}
	}
	#endregion
	#region ScalingInfoCache
	public class ScalingInfoCache : UniqueItemsCache<ScalingInfo> {
		public ScalingInfoCache(IDocumentModelUnitConverter converter)
			: base(converter) {
		}
		protected override ScalingInfo CreateDefaultItem(IDocumentModelUnitConverter unitConverter) {
			return new ScalingInfo();
		}
	}
	#endregion
	#region ScalingOptions
	public class ScalingOptions : SpreadsheetUndoableIndexBasedObject<ScalingInfo>, ICloneable<ScalingOptions>, ISupportsCopyFrom<ScalingOptions> {
		readonly IChart parent;
		public ScalingOptions(IChart parent)
			: base(parent.DocumentModel) {
			this.parent = parent;
		}
		#region Properties
		protected internal IChart Parent { get { return parent; } }
		#region Orientation
		public AxisOrientation Orientation {
			get { return Info.Orientation; }
			set {
				if(Orientation == value)
					return;
				SetPropertyValue(SetOrientationCore, value);
			}
		}
		DocumentModelChangeActions SetOrientationCore(ScalingInfo info, AxisOrientation value) {
			info.Orientation = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region LogBase
		public double LogBase {
			get { return Info.LogBase; }
			set {
				System.Diagnostics.Debug.Assert(value >= 2 && value <= 1000);
				if(LogBase == value)
					return;
				SetPropertyValue(SetLogBaseCore, value);
			}
		}
		DocumentModelChangeActions SetLogBaseCore(ScalingInfo info, double value) {
			info.LogBase = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region Min
		public double Min {
			get { return Info.Min; }
			set {
				if(Min == value)
					return;
				SetPropertyValue(SetMinCore, value);
			}
		}
		DocumentModelChangeActions SetMinCore(ScalingInfo info, double value) {
			info.Min = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region Max
		public double Max {
			get { return Info.Max; }
			set {
				if(Max == value)
					return;
				SetPropertyValue(SetMaxCore, value);
			}
		}
		DocumentModelChangeActions SetMaxCore(ScalingInfo info, double value) {
			info.Max = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region LogScale
		public bool LogScale {
			get { return Info.LogScale; }
			set {
				if (LogScale == value)
					return;
				SetPropertyValue(SetLogScaleCore, value);
			}
		}
		DocumentModelChangeActions SetLogScaleCore(ScalingInfo info, bool value) {
			info.LogScale = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region FixedMax
		public bool FixedMax {
			get { return Info.FixedMax; }
			set {
				if (FixedMax == value)
					return;
				SetPropertyValue(SetFixedMaxCore, value);
			}
		}
		DocumentModelChangeActions SetFixedMaxCore(ScalingInfo info, bool value) {
			info.FixedMax = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region FixedMin
		public bool FixedMin {
			get { return Info.FixedMin; }
			set {
				if (FixedMin == value)
					return;
				SetPropertyValue(SetFixedMinCore, value);
			}
		}
		DocumentModelChangeActions SetFixedMinCore(ScalingInfo info, bool value) {
			info.FixedMin = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#endregion
		#region SpreadsheetUndoableIndexBasedObject members
		public override DocumentModelChangeActions GetBatchUpdateChangeActions() {
			return DocumentModelChangeActions.None;
		}
		protected override UniqueItemsCache<ScalingInfo> GetCache(IDocumentModel documentModel) {
			return DocumentModel.Cache.ScalingInfoCache;
		}
		protected override void ApplyChanges(DocumentModelChangeActions changeActions) {
			DocumentModel.ApplyChanges(changeActions);
			Parent.Invalidate();
		}
		#endregion
		#region ICloneable<ScalingOptions> Members
		public ScalingOptions Clone() {
			ScalingOptions result = new ScalingOptions(this.parent);
			result.CopyFrom(this);
			return result;
		}
		#endregion
		#region ISupportsCopyFrom<ScalingOptions> Members
		public void CopyFrom(ScalingOptions value) {
			Guard.ArgumentNotNull(value, "value");
			base.CopyFrom(value);
		}
		#endregion
	}
	#endregion
}
