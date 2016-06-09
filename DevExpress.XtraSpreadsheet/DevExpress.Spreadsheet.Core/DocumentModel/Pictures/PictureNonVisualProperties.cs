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
using System.Runtime.InteropServices;
using DevExpress.Office;
using DevExpress.Utils;
using DevExpress.XtraSpreadsheet.Utils;
namespace DevExpress.XtraSpreadsheet.Model {
	#region PictureHyperlink
	#endregion
	#region CommonDrawingLocksInfo
	public class CommonDrawingLocksInfo : ICloneable<CommonDrawingLocksInfo>, ISupportsCopyFrom<CommonDrawingLocksInfo>, ISupportsSizeOf, IEquatable<CommonDrawingLocksInfo> {
		#region Fields
		const uint MaskNoGroup = 0x00000001;			
		const uint MaskNoSelect = 0x00000002;		   
		const uint MaskNoRotate = 0x00000004;		   
		const uint MaskNoChangeAspect = 0x00000008;	 
		const uint MaskNoMove = 0x00000010;			 
		const uint MaskNoResize = 0x00000020;		   
		const uint MaskNoEditPoints = 0x00000040;	   
		const uint MaskNoAdjustHandles = 0x00000080;	
		const uint MaskNoChangeArrowheads = 0x00000100; 
		const uint MaskNoChangeShapeType = 0x00000200;  
		const uint MaskNoTextEdit = 0x00000400;		 
		const uint MaskNoCrop = 0x00000800;			 
		const uint MaskNoDrillDown = 0x00001000;		
		const uint MaskNoUngroup = 0x00002000;		  
		uint packedValues;
		#endregion
		#region Properties
		public bool NoGroup { get { return PackedValues.GetBoolBitValue(packedValues, MaskNoGroup); } set { PackedValues.SetBoolBitValue(ref packedValues, MaskNoGroup, value); } }
		public bool NoSelect { get { return PackedValues.GetBoolBitValue(packedValues, MaskNoSelect); } set { PackedValues.SetBoolBitValue(ref packedValues, MaskNoSelect, value); } }
		public bool NoRotate { get { return PackedValues.GetBoolBitValue(packedValues, MaskNoRotate); } set { PackedValues.SetBoolBitValue(ref packedValues, MaskNoRotate, value); } }
		public bool NoChangeAspect { get { return PackedValues.GetBoolBitValue(packedValues, MaskNoChangeAspect); } set { PackedValues.SetBoolBitValue(ref packedValues, MaskNoChangeAspect, value); } }
		public bool NoMove { get { return PackedValues.GetBoolBitValue(packedValues, MaskNoMove); } set { PackedValues.SetBoolBitValue(ref packedValues, MaskNoMove, value); } }
		public bool NoResize { get { return PackedValues.GetBoolBitValue(packedValues, MaskNoResize); } set { PackedValues.SetBoolBitValue(ref packedValues, MaskNoResize, value); } }
		public bool NoEditPoints { get { return PackedValues.GetBoolBitValue(packedValues, MaskNoEditPoints); } set { PackedValues.SetBoolBitValue(ref packedValues, MaskNoEditPoints, value); } }
		public bool NoAdjustHandles { get { return PackedValues.GetBoolBitValue(packedValues, MaskNoAdjustHandles); } set { PackedValues.SetBoolBitValue(ref packedValues, MaskNoAdjustHandles, value); } }
		public bool NoChangeArrowheads { get { return PackedValues.GetBoolBitValue(packedValues, MaskNoChangeArrowheads); } set { PackedValues.SetBoolBitValue(ref packedValues, MaskNoChangeArrowheads, value); } }
		public bool NoChangeShapeType { get { return PackedValues.GetBoolBitValue(packedValues, MaskNoChangeShapeType); } set { PackedValues.SetBoolBitValue(ref packedValues, MaskNoChangeShapeType, value); } }
		public bool NoTextEdit { get { return PackedValues.GetBoolBitValue(packedValues, MaskNoTextEdit); } set { PackedValues.SetBoolBitValue(ref packedValues, MaskNoTextEdit, value); } }
		public bool NoCrop { get { return PackedValues.GetBoolBitValue(packedValues, MaskNoCrop); } set { PackedValues.SetBoolBitValue(ref packedValues, MaskNoCrop, value); } }
		public bool NoDrillDown { get { return PackedValues.GetBoolBitValue(packedValues, MaskNoDrillDown); } set { PackedValues.SetBoolBitValue(ref packedValues, MaskNoDrillDown, value); } }
		public bool NoUngroup { get { return PackedValues.GetBoolBitValue(packedValues, MaskNoUngroup); } set { PackedValues.SetBoolBitValue(ref packedValues, MaskNoUngroup, value); } }
		#endregion
		public bool IsEmpty() {
			return packedValues == 0;
		}
		#region Implementation of ICloneable<CommonDrawingLocksInfo>
		public CommonDrawingLocksInfo Clone() {
			CommonDrawingLocksInfo other = new CommonDrawingLocksInfo();
			other.CopyFrom(this);
			return other;
		}
		#endregion
		#region Implementation of ISupportsCopyFrom<CommonDrawingLocksInfo>
		public void CopyFrom(CommonDrawingLocksInfo value) {
			packedValues = value.packedValues;
		}
		#endregion
		#region Implementation of ISupportsSizeOf
		public int SizeOf() {
			return DXMarshal.SizeOf(GetType());
		}
		#endregion
		#region Overrides of Object
		public override bool Equals(object obj) {
			if(ReferenceEquals(null, obj))
				return false;
			if(ReferenceEquals(this, obj))
				return true;
			if(obj.GetType() != GetType())
				return false;
			return Equals((CommonDrawingLocksInfo) obj);
		}
		#endregion
		#region Equality members
		public bool Equals(CommonDrawingLocksInfo other) {
			return packedValues == other.packedValues;
		}
		public override int GetHashCode() {
			return (int)packedValues;
		}
		#endregion
	}
	#endregion
	#region CommonDrawingLocksInfoCache
	public class CommonDrawingLocksInfoCache : UniqueItemsCache<CommonDrawingLocksInfo> {
		public CommonDrawingLocksInfoCache(IDocumentModelUnitConverter converter)
			: base(converter) {}
		protected override CommonDrawingLocksInfo CreateDefaultItem(IDocumentModelUnitConverter unitConverter) {
			CommonDrawingLocksInfo info = new CommonDrawingLocksInfo();
			return info;
		}
	}
	#endregion
	#region CommonDrawingLocks
	public class CommonDrawingLocks : SpreadsheetUndoableIndexBasedObject<CommonDrawingLocksInfo>, ISupportsNoChangeAspect {
		public CommonDrawingLocks(IDocumentModelPartWithApplyChanges part) : base(part) {}
		#region Properties
		public bool NoGroup {
			get { return Info.NoGroup; }
			set {
				if(value == NoGroup)
					return;
				SetPropertyValue(SetNoGroupCore, value);
			}
		}
		DocumentModelChangeActions SetNoGroupCore(CommonDrawingLocksInfo info, bool value) {
			info.NoGroup = value;
			return DocumentModelChangeActions.None;
		}
		public bool NoSelect {
			get { return Info.NoSelect; }
			set {
				if(value == NoSelect)
					return;
				SetPropertyValue(SetNoSelectCore, value);
			}
		}
		DocumentModelChangeActions SetNoSelectCore(CommonDrawingLocksInfo info, bool value) {
			info.NoSelect = value;
			return DocumentModelChangeActions.None;
		}
		public bool NoRotate {
			get { return Info.NoRotate; }
			set {
				if(value == NoRotate)
					return;
				SetPropertyValue(SetNoRotateCore, value);
			}
		}
		DocumentModelChangeActions SetNoRotateCore(CommonDrawingLocksInfo info, bool value) {
			info.NoRotate = value;
			return DocumentModelChangeActions.None;
		}
		public bool NoChangeAspect {
			get { return Info.NoChangeAspect; }
			set {
				if(value == NoChangeAspect)
					return;
				SetPropertyValue(SetNoChangeAspectCore, value);
			}
		}
		DocumentModelChangeActions SetNoChangeAspectCore(CommonDrawingLocksInfo info, bool value) {
			info.NoChangeAspect = value;
			return DocumentModelChangeActions.None;
		}
		public bool NoMove {
			get { return Info.NoMove; }
			set {
				if(value == NoMove)
					return;
				SetPropertyValue(SetNoMoveCore, value);
			}
		}
		DocumentModelChangeActions SetNoMoveCore(CommonDrawingLocksInfo info, bool value) {
			info.NoMove = value;
			return DocumentModelChangeActions.None;
		}
		public bool NoResize {
			get { return Info.NoResize; }
			set {
				if(value == NoResize)
					return;
				SetPropertyValue(SetNoResizeCore, value);
			}
		}
		DocumentModelChangeActions SetNoResizeCore(CommonDrawingLocksInfo info, bool value) {
			info.NoResize = value;
			return DocumentModelChangeActions.None;
		}
		public bool NoEditPoints {
			get { return Info.NoEditPoints; }
			set {
				if(value == NoEditPoints)
					return;
				SetPropertyValue(SetNoEditPointsCore, value);
			}
		}
		DocumentModelChangeActions SetNoEditPointsCore(CommonDrawingLocksInfo info, bool value) {
			info.NoEditPoints = value;
			return DocumentModelChangeActions.None;
		}
		public bool NoAdjustHandles {
			get { return Info.NoAdjustHandles; }
			set {
				if(value == NoAdjustHandles)
					return;
				SetPropertyValue(SetNoAdjustHandlesCore, value);
			}
		}
		DocumentModelChangeActions SetNoAdjustHandlesCore(CommonDrawingLocksInfo info, bool value) {
			info.NoAdjustHandles = value;
			return DocumentModelChangeActions.None;
		}
		public bool NoChangeArrowheads {
			get { return Info.NoChangeArrowheads; }
			set {
				if(value == NoChangeArrowheads)
					return;
				SetPropertyValue(SetNoChangeArrowheadsCore, value);
			}
		}
		DocumentModelChangeActions SetNoChangeArrowheadsCore(CommonDrawingLocksInfo info, bool value) {
			info.NoChangeArrowheads = value;
			return DocumentModelChangeActions.None;
		}
		public bool NoChangeShapeType {
			get { return Info.NoChangeShapeType; }
			set {
				if(value == NoChangeShapeType)
					return;
				SetPropertyValue(SetNoChangeShapeTypeCore, value);
			}
		}
		DocumentModelChangeActions SetNoChangeShapeTypeCore(CommonDrawingLocksInfo info, bool value) {
			info.NoChangeShapeType = value;
			return DocumentModelChangeActions.None;
		}
		public bool NoTextEdit {
			get { return Info.NoTextEdit; }
			set {
				if(value == NoTextEdit)
					return;
				SetPropertyValue(SetNoTextEditCore, value);
			}
		}
		DocumentModelChangeActions SetNoTextEditCore(CommonDrawingLocksInfo info, bool value) {
			info.NoTextEdit = value;
			return DocumentModelChangeActions.None;
		}
		public bool NoCrop {
			get { return Info.NoCrop; }
			set {
				if(value == NoCrop)
					return;
				SetPropertyValue(SetNoCropCore, value);
			}
		}
		DocumentModelChangeActions SetNoCropCore(CommonDrawingLocksInfo info, bool value) {
			info.NoCrop = value;
			return DocumentModelChangeActions.None;
		}
		public bool NoDrillDown {
			get { return Info.NoDrillDown; }
			set {
				if(value == NoDrillDown)
					return;
				SetPropertyValue(SetNoDrillDownCore, value);
			}
		}
		DocumentModelChangeActions SetNoDrillDownCore(CommonDrawingLocksInfo info, bool value) {
			info.NoDrillDown = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region NoUngroup
		public bool NoUngroup {
			get { return Info.NoUngroup; }
			set {
				if(value == NoUngroup)
					return;
				SetPropertyValue(SetNoUngroupCore, value);
			}
		}
		DocumentModelChangeActions SetNoUngroupCore(CommonDrawingLocksInfo info, bool value) {
			info.NoUngroup = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region Overrides of UndoableIndexBasedObject<CommonDrawingLocksInfo,DocumentModelChangeActions>
		protected override UniqueItemsCache<CommonDrawingLocksInfo> GetCache(IDocumentModel documentModel) {
			return DocumentModel.Cache.CommonDrawingLocksInfoCache;
		}
		public override DocumentModelChangeActions GetBatchUpdateChangeActions() {
			return DocumentModelChangeActions.None;
		}
		#endregion
		public bool IsEmpty() {
			return Info.IsEmpty();
		}
	}
	#endregion
	#region ISupportsNoChangeAspect
	public interface ISupportsNoChangeAspect {
		bool NoChangeAspect { get; set; }
	}
	#endregion
	#region IDrawingLocks
	public interface ICommonDrawingLocks {
		bool NoGroup { get; set; }
		bool NoSelect { get; set; }
		bool NoChangeAspect { get; set; }
		bool NoMove { get; set; }
		bool IsEmpty { get; }
	}
	public interface IDrawingLocks : ICommonDrawingLocks {
		bool NoRotate { get; set; }
		bool NoResize { get; set; }
		bool NoEditPoints { get; set; }
		bool NoAdjustHandles { get; set; }
		bool NoChangeArrowheads { get; set; }
		bool NoChangeShapeType { get; set; }
	}
	#endregion
	#region IConnectionLocks
	public interface IConnectionLocks : IDrawingLocks {
	}
	#endregion
	#region IShapeLocks
	public interface IShapeLocks : IDrawingLocks {
		bool NoTextEdit { get; set; }
	}
	#endregion
	#region PictureLocks
	public interface IPictureLocks:IDrawingLocks {
		bool NoCrop { get; set; }
	}
	#endregion
	#region IChartLocks
	public interface IChartLocks : ICommonDrawingLocks {
		bool NoDrillDown { get; set; }
	}
	#endregion
	#region IGroupLocks
	public interface IGroupLocks : ICommonDrawingLocks {
		bool NoUngroup { get; set; }
		bool NoRotate { get; set; }
		bool NoResize { get; set; }
	}
	#endregion
	#region PictureLocks
	public class PictureLocks : IPictureLocks, ISupportsNoChangeAspect {
		#region Fields
		readonly CommonDrawingLocks locks;
		#endregion
		#region Properties
		#region Implementation of IDrawingLocks
		public bool NoGroup { get { return locks.NoGroup; } set { locks.NoGroup = value; } }
		public bool NoSelect { get { return locks.NoSelect; } set { locks.NoSelect = value; } }
		public bool NoRotate { get { return locks.NoRotate; } set { locks.NoRotate = value; } }
		public bool NoChangeAspect { get { return locks.NoChangeAspect; } set { locks.NoChangeAspect = value; } }
		public bool NoMove { get { return locks.NoMove; } set { locks.NoMove = value; } }
		public bool NoResize { get { return locks.NoResize; } set { locks.NoResize = value; } }
		public bool NoEditPoints { get { return locks.NoEditPoints; } set { locks.NoEditPoints = value; } }
		public bool NoAdjustHandles { get { return locks.NoAdjustHandles; } set { locks.NoAdjustHandles = value; } }
		public bool NoChangeArrowheads { get { return locks.NoChangeArrowheads; } set { locks.NoChangeArrowheads = value; } }
		public bool NoChangeShapeType { get { return locks.NoChangeShapeType; } set { locks.NoChangeShapeType = value; } }
		public bool IsEmpty { get { return locks.IsEmpty(); } }
		#endregion
		#region Implementation of IPictureLocks
		public bool NoCrop { get { return locks.NoCrop; } set { locks.NoCrop = value; } }
		#endregion
		public CommonDrawingLocks InnerLocks { get { return locks; } }
		#endregion
		public PictureLocks(CommonDrawingLocks innerLocks) {
			locks = innerLocks;
		}
	}
	#endregion
	#region ChartLocks
	public class ChartLocks : IChartLocks, ISupportsNoChangeAspect {
		#region Fields
		readonly CommonDrawingLocks locks;
		#endregion
		#region Properties
		#region Implementation of IChartLocks
		public bool NoGroup { get { return locks.NoGroup; } set { locks.NoGroup = value; } }
		public bool NoDrillDown { get { return locks.NoDrillDown; } set { locks.NoDrillDown = value; } }
		public bool NoSelect { get { return locks.NoSelect; } set { locks.NoSelect = value; } }
		public bool NoChangeAspect { get { return locks.NoChangeAspect; } set { locks.NoChangeAspect = value; } }
		public bool NoMove { get { return locks.NoMove; } set { locks.NoMove = value; } }
		public bool IsEmpty { get { return locks.IsEmpty(); } }
		#endregion
		public CommonDrawingLocks InnerLocks { get { return locks; } }
		#endregion
		public ChartLocks(CommonDrawingLocks innerLocks) {
			locks = innerLocks;
		}
	}
	#endregion
	#region ShapeLocks
	public class ShapeLocks : IShapeLocks, ISupportsNoChangeAspect {
		#region Fields
		readonly CommonDrawingLocks locks;
		#endregion
		#region Properties
		#region Implementation of IDrawingLocks
		public bool NoGroup { get { return locks.NoGroup; } set { locks.NoGroup = value; } }
		public bool NoSelect { get { return locks.NoSelect; } set { locks.NoSelect = value; } }
		public bool NoRotate { get { return locks.NoRotate; } set { locks.NoRotate = value; } }
		public bool NoChangeAspect { get { return locks.NoChangeAspect; } set { locks.NoChangeAspect = value; } }
		public bool NoMove { get { return locks.NoMove; } set { locks.NoMove = value; } }
		public bool NoResize { get { return locks.NoResize; } set { locks.NoResize = value; } }
		public bool NoEditPoints { get { return locks.NoEditPoints; } set { locks.NoEditPoints = value; } }
		public bool NoAdjustHandles { get { return locks.NoAdjustHandles; } set { locks.NoAdjustHandles = value; } }
		public bool NoChangeArrowheads { get { return locks.NoChangeArrowheads; } set { locks.NoChangeArrowheads = value; } }
		public bool NoChangeShapeType { get { return locks.NoChangeShapeType; } set { locks.NoChangeShapeType = value; } }
		public bool NoTextEdit { get { return locks.NoTextEdit; } set { locks.NoTextEdit = value; } }
		public bool IsEmpty { get { return locks.IsEmpty(); } }
		#endregion
		#region Implementation of IPictureLocks
		public bool NoCrop { get { return locks.NoCrop; } set { locks.NoCrop = value; } }
		#endregion
		public CommonDrawingLocks InnerLocks { get { return locks; } }
		#endregion
		public ShapeLocks(CommonDrawingLocks innerLocks) {
			locks = innerLocks;
		}
	}
	#endregion
	#region GroupShapeLocks
	public class GroupShapeLocks : IGroupLocks, ISupportsNoChangeAspect {
		#region Fields
		readonly CommonDrawingLocks locks;
		#endregion
		#region Properties
		#region Implementation of IDrawingLocks
		public bool NoGroup { get { return locks.NoGroup; } set { locks.NoGroup = value; } }
		public bool NoSelect { get { return locks.NoSelect; } set { locks.NoSelect = value; } }
		public bool NoUngroup { get { return locks.NoUngroup; } set { locks.NoUngroup = value; } }
		public bool NoRotate { get { return locks.NoRotate; } set { locks.NoRotate = value; } }
		public bool NoChangeAspect { get { return locks.NoChangeAspect; } set { locks.NoChangeAspect = value; } }
		public bool NoMove { get { return locks.NoMove; } set { locks.NoMove = value; } }
		public bool NoResize { get { return locks.NoResize; } set { locks.NoResize = value; } }
		public bool IsEmpty { get { return locks.IsEmpty(); } }
		#endregion
		#region Implementation of IPictureLocks
		public bool NoCrop { get { return locks.NoCrop; } set { locks.NoCrop = value; } }
		#endregion
		public CommonDrawingLocks InnerLocks { get { return locks; } }
		#endregion
		public GroupShapeLocks(CommonDrawingLocks innerLocks) {
			locks = innerLocks;
		}
	}
	#endregion
	#region ConnectionShapeLocks
	public class ConnectionShapeLocks : IConnectionLocks, ISupportsNoChangeAspect {
		#region Fields
		readonly CommonDrawingLocks locks;
		#endregion
		#region Properties
		#region Implementation of IDrawingLocks
		public bool NoGroup { get { return locks.NoGroup; } set { locks.NoGroup = value; } }
		public bool NoSelect { get { return locks.NoSelect; } set { locks.NoSelect = value; } }
		public bool NoRotate { get { return locks.NoRotate; } set { locks.NoRotate = value; } }
		public bool NoChangeAspect { get { return locks.NoChangeAspect; } set { locks.NoChangeAspect = value; } }
		public bool NoMove { get { return locks.NoMove; } set { locks.NoMove = value; } }
		public bool NoResize { get { return locks.NoResize; } set { locks.NoResize = value; } }
		public bool NoEditPoints { get { return locks.NoEditPoints; } set { locks.NoEditPoints = value; } }
		public bool NoAdjustHandles { get { return locks.NoAdjustHandles; } set { locks.NoAdjustHandles = value; } }
		public bool NoChangeArrowheads { get { return locks.NoChangeArrowheads; } set { locks.NoChangeArrowheads = value; } }
		public bool NoChangeShapeType { get { return locks.NoChangeShapeType; } set { locks.NoChangeShapeType = value; } }
		public bool IsEmpty { get { return locks.IsEmpty(); } }
		#endregion
		public CommonDrawingLocks InnerLocks { get { return locks; } }
		#endregion
		public ConnectionShapeLocks(CommonDrawingLocks innerLocks) {
			locks = innerLocks;
		}
	}
	#endregion
	#region PictureNonVisualProperties
	public interface IPictureNonVisualProperties {
		#region cNvPicPr (Non-Visual Picture Drawing Properties) §5.6.2.7
		bool PreferRelativeResize { get; set; }
		#endregion
	}
	#endregion
	#region 5.6.2.8	cNvPr (Non-Visual Drawing Properties)
	public interface IDrawingObjectNonVisualProperties {
		#region cNvPr (Non-Visual Drawing Properties) §5.6.2.8
		int Id { get; set; }
		string Name { get; set; }
		string Description { get; set; }
		bool Hidden { get; set; }
		string HyperlinkClickTooltip { get; set; }
		string HyperlinkClickTargetFrame { get; set; }
		string HyperlinkClickUrl { get; set; }
		bool HyperlinkClickIsExternal { get; set; }
		#endregion
		void RemoveHyperlink();
	}
	#endregion
}
