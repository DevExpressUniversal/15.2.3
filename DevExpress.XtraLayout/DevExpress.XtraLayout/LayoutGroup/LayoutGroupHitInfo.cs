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
using System.Drawing;
namespace DevExpress.XtraLayout.HitInfo {
	public enum LayoutGroupHitTypes{ExpandedButton, ChildHitInfo, None, Caption, CaptionImage, TableDefinition}
	public class DashboardHitInfo : LayoutGroupHitInfo {
		public DashboardHitInfo(BaseLayoutItemHitInfo hitInfo, LayoutGroupHitTypes groupHitType)
			: base(hitInfo, groupHitType) {
				if(hitInfo is DashboardHitInfo) buttonIndexCore = (hitInfo as DashboardHitInfo).ButtonIndex; 
		}
		public DashboardHitInfo(BaseLayoutItemHitInfo hitInfo, int index)
			: base(hitInfo, LayoutGroupHitTypes.Caption) {
			buttonIndexCore = index;
		}
		int buttonIndexCore = -1;
		public int ButtonIndex {get {return buttonIndexCore;}}
		public override bool IsExpandButton {
			get {
				return buttonIndexCore >= 0;
			}
		}
	}
	public class LayoutGroupHitInfo: BaseLayoutItemHitInfo {
		LayoutGroupHitTypes additionalHitType;
		public LayoutGroupHitInfo(BaseLayoutItemHitInfo hitInfo, LayoutGroupHitTypes groupHitType) : base(hitInfo) {
			additionalHitType = groupHitType;
		}
		public int rowIndex { get; set; }
		public int columnIndex { get; set; }
		protected internal void SetRowColumnIndex(int row, int column) {
			rowIndex = row;
			columnIndex = column;
		}
		public LayoutGroupHitTypes AdditionalHitType{
			get { 
				return additionalHitType;
			}
		}
		public override bool IsGroup {
			get {
				return true;
			}
		}
		public override bool IsExpandButton{
			get {
				return additionalHitType == LayoutGroupHitTypes.ExpandedButton;
			}
		}
		protected internal void SetAdditionalHitType(LayoutGroupHitTypes newVal) {
			additionalHitType =  newVal;
		}
		public override string ToString() {
			return  Environment.NewLine + "base:"+ base.ToString();
		}
	}
}
