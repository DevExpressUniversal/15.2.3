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
	#region View3DInfo
	public class View3DInfo : ICloneable<View3DInfo>, ISupportsCopyFrom<View3DInfo>, ISupportsSizeOf {
		#region Fields
		const int offsetHeightPercent = 11;
		const int offsetPerspective = 20;
		const int offsetYRotation = 8;
		const uint maskDepthPercent   = 0x000007ff;
		const uint maskHeightPercent  = 0x000ff800;
		const uint maskPerspective	= 0x0ff00000;
		const uint maskRightAngleAxes = 0x10000000;
		const uint maskAutoHeight	 = 0x20000000;
		const uint maskXRotation	  = 0x0000007f;
		const uint maskXRotationSign  = 0x00000080;
		const uint maskYRotation	  = 0x0001ff00;
		uint packedValues = 0x31e32064;
		uint packedRotation = 0x00000000;
		#endregion
		#region Properties
		public int DepthPercent {
			get { return PackedValues.GetIntBitValue(this.packedValues, maskDepthPercent); }
			set {
				ValueChecker.CheckValue(value, 20, 2000, "DepthPercent");
				PackedValues.SetIntBitValue(ref this.packedValues, maskDepthPercent, value);
			}
		}
		public int HeightPercent {
			get { return PackedValues.GetIntBitValue(this.packedValues, maskHeightPercent, offsetHeightPercent); }
			set {
				ValueChecker.CheckValue(value, 5, 500, "HeightPercent");
				PackedValues.SetIntBitValue(ref this.packedValues, maskHeightPercent, offsetHeightPercent, value);
			}
		}
		public int Perspective {
			get { return PackedValues.GetIntBitValue(this.packedValues, maskPerspective, offsetPerspective); }
			set {
				ValueChecker.CheckValue(value, 0, 240, "Perspective");
				PackedValues.SetIntBitValue(ref this.packedValues, maskPerspective, offsetPerspective, value);
			}
		}
		public bool RightAngleAxes {
			get { return PackedValues.GetBoolBitValue(this.packedValues, maskRightAngleAxes); }
			set { PackedValues.SetBoolBitValue(ref this.packedValues, maskRightAngleAxes, value); } 
		}
		public bool AutoHeight {
			get { return PackedValues.GetBoolBitValue(this.packedValues, maskAutoHeight); }
			set { PackedValues.SetBoolBitValue(ref this.packedValues, maskAutoHeight, value); }
		}
		public int XRotation {
			get {
				int value = PackedValues.GetIntBitValue(this.packedRotation, maskXRotation);
				return PackedValues.GetBoolBitValue(this.packedRotation, maskXRotationSign) ? -value : value; 
			}
			set {
				ValueChecker.CheckValue(value, -90, 90, "XRotation");
				PackedValues.SetIntBitValue(ref this.packedRotation, maskXRotation, Math.Abs(value));
				PackedValues.SetBoolBitValue(ref this.packedRotation, maskXRotationSign, value < 0);
			}
		}
		public int YRotation {
			get { return PackedValues.GetIntBitValue(this.packedRotation, maskYRotation, offsetYRotation); }
			set {
				ValueChecker.CheckValue(value, 0, 360, "YRotation");
				PackedValues.SetIntBitValue(ref this.packedRotation, maskYRotation, offsetYRotation, value);
			}
		}
		#endregion
		#region ICloneable<View3DInfo> Members
		public View3DInfo Clone() {
			View3DInfo result = new View3DInfo();
			result.CopyFrom(this);
			return result;
		}
		#endregion
		#region ISupportsCopyFrom<View3DInfo> Members
		public void CopyFrom(View3DInfo value) {
			Guard.ArgumentNotNull(value, "value");
			this.packedValues = value.packedValues;
			this.packedRotation = value.packedRotation;
		}
		#endregion
		#region ISupportsSizeOf Members
		public int SizeOf() {
			return DXMarshal.SizeOf(GetType());
		}
		#endregion
		public override bool Equals(object obj) {
			View3DInfo other = obj as View3DInfo;
			if(other == null)
				return false;
			return this.packedValues == other.packedValues && this.packedRotation == other.packedRotation;
		}
		public override int GetHashCode() {
			return (int)(this.packedValues ^ this.packedRotation);
		}
	}
	#endregion
	#region View3DInfoCache
	public class View3DInfoCache : UniqueItemsCache<View3DInfo> {
		public View3DInfoCache(IDocumentModelUnitConverter converter)
			: base(converter) {
		}
		protected override View3DInfo CreateDefaultItem(IDocumentModelUnitConverter unitConverter) {
			return new View3DInfo();
		}
	}
	#endregion
	#region View3DOptions
	public class View3DOptions : SpreadsheetUndoableIndexBasedObject<View3DInfo>, ICloneable<View3DOptions>, ISupportsCopyFrom<View3DOptions> {
		readonly IChart parent;
		public View3DOptions(IChart parent) 
			: base(parent.DocumentModel) {
			this.parent = parent;
		}
		#region Properties
		protected internal IChart Parent { get { return parent; } }
		#region DepthPercent
		public int DepthPercent {
			get { return Info.DepthPercent; }
			set {
				if(DepthPercent == value)
					return;
				SetPropertyValue(SetDepthPercentCore, value);
			}
		}
		DocumentModelChangeActions SetDepthPercentCore(View3DInfo info, int value) {
			info.DepthPercent = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region HeightPercent
		public int HeightPercent {
			get { return Info.HeightPercent; }
			set {
				if(HeightPercent == value)
					return;
				SetPropertyValue(SetHeightPercentCore, value);
			}
		}
		DocumentModelChangeActions SetHeightPercentCore(View3DInfo info, int value) {
			info.HeightPercent = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region Perspective
		public int Perspective {
			get { return Info.Perspective; }
			set {
				if(Perspective == value)
					return;
				SetPropertyValue(SetPerspectiveCore, value);
			}
		}
		DocumentModelChangeActions SetPerspectiveCore(View3DInfo info, int value) {
			info.Perspective = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region RightAngleAxes
		public bool RightAngleAxes {
			get { return Info.RightAngleAxes; }
			set {
				if(RightAngleAxes == value)
					return;
				SetPropertyValue(SetRightAngleAxesCore, value);
			}
		}
		DocumentModelChangeActions SetRightAngleAxesCore(View3DInfo info, bool value) {
			info.RightAngleAxes = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region AutoHeight
		public bool AutoHeight {
			get { return Info.AutoHeight; }
			set {
				if (AutoHeight == value)
					return;
				SetPropertyValue(SetAutoHeightCore, value);
			}
		}
		DocumentModelChangeActions SetAutoHeightCore(View3DInfo info, bool value) {
			info.AutoHeight = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region XRotation
		public int XRotation {
			get { return Info.XRotation; }
			set {
				if(XRotation == value)
					return;
				SetPropertyValue(SetXRotationCore, value);
			}
		}
		DocumentModelChangeActions SetXRotationCore(View3DInfo info, int value) {
			info.XRotation = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region YRotation
		public int YRotation {
			get { return Info.YRotation; }
			set {
				if(YRotation == value)
					return;
				SetPropertyValue(SetYRotationCore, value);
			}
		}
		DocumentModelChangeActions SetYRotationCore(View3DInfo info, int value) {
			info.YRotation = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#endregion
		#region SpreadsheetUndoableIndexBasedObject members
		public override DocumentModelChangeActions GetBatchUpdateChangeActions() {
			return DocumentModelChangeActions.None;
		}
		protected override UniqueItemsCache<View3DInfo> GetCache(IDocumentModel documentModel) {
			return DocumentModel.Cache.View3DInfoCache;
		}
		protected override void ApplyChanges(DocumentModelChangeActions changeActions) {
			DocumentModel.ApplyChanges(changeActions);
			Parent.Invalidate();
		}
		#endregion
		#region ICloneable<View3DOptions> Members
		public View3DOptions Clone() {
			View3DOptions result = new View3DOptions(Parent);
			result.CopyFrom(this);
			return result;
		}
		#endregion
		#region ISupportsCopyFrom<View3DOptions> Members
		public void CopyFrom(View3DOptions value) {
			Guard.ArgumentNotNull(value, "value");
			base.CopyFrom(value);
		}
		#endregion
	}
	#endregion
}
