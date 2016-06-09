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
namespace DevExpress.XtraSpreadsheet.Model {
	public class GroupAndOutlinePropertiesInfo :ICloneable<GroupAndOutlinePropertiesInfo>, ISupportsCopyFrom<GroupAndOutlinePropertiesInfo>, ISupportsSizeOf {
		#region Fields
		bool applyStyles;
		bool showRowSumsBelow = true;
		bool showColumnSumsRight = true;
		#endregion
		#region Properties
		public bool ApplyStyles { get { return applyStyles; } set { applyStyles = value; } }
		public bool ShowRowSumsBelow { get { return showRowSumsBelow; } set { showRowSumsBelow = value; } }
		public bool ShowColumnSumsRight { get { return showColumnSumsRight; } set { showColumnSumsRight = value; } }
		#endregion
		public GroupAndOutlinePropertiesInfo Clone() {
			GroupAndOutlinePropertiesInfo result = new GroupAndOutlinePropertiesInfo();
			result.CopyFrom(this);
			return result;
		}
		public void CopyFrom(GroupAndOutlinePropertiesInfo value) {
			this.applyStyles = value.applyStyles;
			this.showColumnSumsRight = value.showColumnSumsRight;
			this.showRowSumsBelow = value.showRowSumsBelow;
		}
		public int SizeOf() {
			return DXMarshal.SizeOf(GetType());
		}
		public override bool Equals(object obj) {
			GroupAndOutlinePropertiesInfo info = obj as GroupAndOutlinePropertiesInfo;
			if (info == null)
				return false;
			return this.applyStyles == info.applyStyles &&
				this.showColumnSumsRight == info.showColumnSumsRight && 
				this.showRowSumsBelow == info.showRowSumsBelow;
		}
		public override int GetHashCode() {
			return this.applyStyles.GetHashCode() ^ this.showColumnSumsRight.GetHashCode() ^ this.showRowSumsBelow.GetHashCode(); 
		}
	}
	#region PrintSetupInfoCache
	public class GroupAndOutlinePropertiesInfoCache :UniqueItemsCache<GroupAndOutlinePropertiesInfo> {
		public GroupAndOutlinePropertiesInfoCache(IDocumentModelUnitConverter unitConverter)
			: base(unitConverter) {
		}
		protected override GroupAndOutlinePropertiesInfo CreateDefaultItem(IDocumentModelUnitConverter unitConverter) {
			GroupAndOutlinePropertiesInfo info = new GroupAndOutlinePropertiesInfo();
			info.ApplyStyles = false;
			info.ShowColumnSumsRight = true;
			info.ShowRowSumsBelow = true;
			return info;
		}
	}
	#endregion
	public class GroupAndOutlineProperties :SpreadsheetUndoableIndexBasedObject<GroupAndOutlinePropertiesInfo> {
		public GroupAndOutlineProperties(IDocumentModelPartWithApplyChanges part)
			: base(part) {
		}
		#region Properties
		public Worksheet Sheet { get { return DocumentModelPart as Worksheet; } }
		#region ApplyStyles
		public bool ApplyStyles { 
			get { return Info.ApplyStyles; } 
			set {
				if (ApplyStyles == value)
					return;
				SetPropertyValue(SetApplyStylesCore, value);
			} 
		}
		DocumentModelChangeActions SetApplyStylesCore(GroupAndOutlinePropertiesInfo info, bool value) {
			info.ApplyStyles = value;
			return DocumentModelChangeActions.RaiseUpdateUI;
		}
		#endregion
		#region ShowRowSumsBelow
		public bool ShowRowSumsBelow { 
			get { return Info.ShowRowSumsBelow; } 
			set {
				if (ShowRowSumsBelow == value)
					return;
				SetPropertyValue(SetShowRowSumsBelowCore, value);
			} 
		}
		DocumentModelChangeActions SetShowRowSumsBelowCore(GroupAndOutlinePropertiesInfo info, bool value) {
			info.ShowRowSumsBelow = value;
			return DocumentModelChangeActions.RaiseUpdateUI;
		}
		#endregion
		#region ShowColumnSumsRight
		public bool ShowColumnSumsRight { 
			get { return Info.ShowColumnSumsRight; } 
			set {
				if (ShowColumnSumsRight == value)
					return;
				SetPropertyValue(SetShowColumnSumsRightCore, value);
			} 
		}
		DocumentModelChangeActions SetShowColumnSumsRightCore(GroupAndOutlinePropertiesInfo info, bool value) {
			info.ShowColumnSumsRight = value;
			return DocumentModelChangeActions.RaiseUpdateUI;
		}
		#endregion
		#endregion
		public override DocumentModelChangeActions GetBatchUpdateChangeActions() {
			return DocumentModelChangeActions.None;
		}
		protected override UniqueItemsCache<GroupAndOutlinePropertiesInfo> GetCache(IDocumentModel documentModel) {
			return DocumentModel.Cache.GroupAndOutlinePropertiesInfoCache;
		}
		public bool IsDefault() {
			return Index == 0;
		}
	}
}
