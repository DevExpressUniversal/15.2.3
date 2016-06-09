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
namespace DevExpress.XtraSpreadsheet.Model {
	#region Cell : ICellProtectionInfo
	partial class Cell {
		public ICellProtectionInfo Protection { get { return this; } }
		public IActualCellProtectionInfo ActualProtection { get { return this; } } 
		public virtual IActualCellProtectionInfo InnerActualProtection { get { return FormatInfo.ActualProtection; } }
		#region ICellProtectionInfo Members
		#region ICellProtectionInfo.Locked
		bool ICellProtectionInfo.Locked {
			get { return FormatInfo.Protection.Locked; }
			set {
				SetProtectionPropertyValue(SetProtectionLocked, value);
			}
		}
		DocumentModelChangeActions SetProtectionLocked(FormatBase info, bool value) {
			info.Protection.Locked = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region ICellProtectionInfo.Hidden
		bool ICellProtectionInfo.Hidden {
			get { return FormatInfo.Protection.Hidden; }
			set {
				SetProtectionPropertyValue(SetProtectionHidden, value);
			}
		}
		DocumentModelChangeActions SetProtectionHidden(FormatBase info, bool value) {
			info.Protection.Hidden = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		protected internal virtual void SetProtectionPropertyValue<U>(SetPropertyValueDelegate<U> setter, U newValue) {
			DocumentModel.BeginUpdate();
			try {
				FormatBase info = GetInfoForModification();
				DocumentModelChangeActions changeActions = setter(info, newValue);
				ReplaceInfo(info, changeActions);
			}
			finally {
				DocumentModel.EndUpdate();
			}
		}
		#endregion
		#region IActualCellProtectionInfo Members
		bool IActualCellProtectionInfo.Locked { get { return GetActualProtectionLocked(); } }
		bool IActualCellProtectionInfo.Hidden { get { return GetActualProtectionHidden(); } }
		#endregion
		#region GetActualProtectionValue
		protected bool GetActualProtectionValue(bool cellFormatActualValue, DifferentialFormatPropertyDescriptor propertyDescriptor) {
			return GetActualFormatValue(cellFormatActualValue, ActualApplyInfo.ApplyProtection, propertyDescriptor);
		}
		protected virtual bool GetActualProtectionLocked() {
			return GetActualProtectionValue(InnerActualProtection.Locked, DifferentialFormatPropertyDescriptor.ProtectionLocked);
		}
		protected virtual bool GetActualProtectionHidden() {
			return GetActualProtectionValue(InnerActualProtection.Hidden, DifferentialFormatPropertyDescriptor.ProtectionHidden);
		}
		#endregion
	}
	#endregion
}
