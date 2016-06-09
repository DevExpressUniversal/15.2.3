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
using System.ComponentModel;
using System.Collections.Specialized;
using DevExpress.Web;
using DevExpress.Web.FilterControl;
using DevExpress.XtraPivotGrid;
namespace DevExpress.Web.ASPxPivotGrid.Internal {	
	public class PivotWebFilterControlPopup : WebFilterControlPopup {
		public PivotWebFilterControlPopup(IPopupFilterControlOwner filterPopupOwner) : base(filterPopupOwner) { }
		protected override ASPxPopupFilterControl CreatePopupFilterControl(IPopupFilterControlOwner filterPopupOwner) {
			return new ASPxPivotPopupFilterControl(filterPopupOwner);
		}
	}
	public class ASPxPivotPopupFilterControl : ASPxPopupFilterControl {
		public ASPxPivotPopupFilterControl(IPopupFilterControlOwner filterPopupOwner) : base(filterPopupOwner) { }
		protected internal ASPxPivotGrid PivotGrid { get { return (ASPxPivotGrid)FilterPopupOwner; } }
		protected override WebFilterControlRenderHelper CreateRenderHelper() {
			return new PivotWebFilterControlRenderHelper(this);
		}
	}
	public class PivotWebFilterControlRenderHelper : WebFilterControlRenderHelper {
		public PivotWebFilterControlRenderHelper(ASPxPivotPopupFilterControl pivotFilterControl) : base(pivotFilterControl) { }
		protected ASPxPivotPopupFilterControl Owner { get { return (ASPxPivotPopupFilterControl)FilterOwner; } }
		protected ASPxPivotGrid PivotGrid { get { return Owner.PivotGrid; } }
		protected override string GetColumnDisplayNameCore(string propertyName) {
			string columnName = base.GetColumnDisplayNameCore(propertyName);
			if(columnName != null || PivotGrid.Fields.IsPrefilterHiddenField(propertyName)) {
				return columnName;
			} else {
				return PivotGridFieldBase.InvalidPropertyDisplayText;
			}
		}
	}
	public class PivotWebFilterControlPopupRow : WebFilterControlPopupRow {
		public PivotWebFilterControlPopupRow(ASPxPivotGrid pivot) : base(pivot) { }
		protected override void PrepareCheckBox() {
			base.PrepareCheckBox();
			if(CheckBoxFilterEnabled != null && PivotGrid.Prefilter.State == PrefilterState.Invalid)
				CheckBoxFilterEnabled.Enabled = false;
		}
		protected ASPxPivotGrid PivotGrid { get { return (ASPxPivotGrid)FilterRowOwner; } }
		protected override WebFilterCriteriaDisplayTextGenerator CreateFilterCriteriaDisplayTextGenerator(IFilterControlOwner filterOwner, bool encodeValue) {
			return new PivotWebFilterCriteriaDisplayTextGenerator(filterOwner);
		}
	}
	public class PivotWebFilterCriteriaDisplayTextGenerator : WebFilterCriteriaDisplayTextGenerator {
		public PivotWebFilterCriteriaDisplayTextGenerator(IFilterControlOwner filterOwner)
			: base(filterOwner, filterOwner.FilterExpression, true) {
		}
		protected ASPxPivotGrid PivotGrid { get { return (ASPxPivotGrid)FilterOwner; } }
		protected override string GetFilterColumnName(string propertyName) {
			string columnName = base.GetFilterColumnName(propertyName);
			if(columnName != null || PivotGrid.Fields.IsPrefilterHiddenField(propertyName)) {
				return columnName;
			} else {
				return PivotGridFieldBase.InvalidPropertyDisplayText;
			}
		}
	}
}
