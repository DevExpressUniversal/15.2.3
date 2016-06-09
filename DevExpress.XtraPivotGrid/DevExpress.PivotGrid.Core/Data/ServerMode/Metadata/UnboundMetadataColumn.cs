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
using System.Threading;
using DevExpress.Data.Filtering;
using DevExpress.Data.Filtering.Helpers;
using DevExpress.PivotGrid.QueryMode;
using DevExpress.XtraPivotGrid;
using DevExpress.XtraPivotGrid.Data;
using System.Globalization;
namespace DevExpress.PivotGrid.ServerMode {
	abstract class UnboundMetadataColumn : MetadataColumn, IUnboundMetadataColumn {
		IServerModeHelpersOwner currentOwner;
		CriteriaOperator criteria;
		public abstract bool IsServer { get; }
		public CriteriaOperator Criteria { get { return criteria; } }
		public UnboundMetadataColumn(PivotGridFieldBase field, IServerModeHelpersOwner currentOwner)
			: base(field.ExpressionFieldName, string.Empty, string.Empty, string.Empty, field.ActualDataType, string.Empty, string.Empty) {
			this.currentOwner = currentOwner;
		}
		protected IServerModeHelpersOwner CurrentOwner { get { return currentOwner; } }
		public virtual bool UpdateCriteria(PivotGridFieldBase field) {
			bool isValid = true;
			bool isChanged = false;
			CriteriaOperator newCriteria = null;
			try {
				newCriteria = CriteriaOperator.Parse(field.UnboundExpression);
				newCriteria = object.ReferenceEquals(newCriteria, null) ? newCriteria : currentOwner.PatchCriteria(newCriteria);
				if(IsServer && !object.ReferenceEquals(newCriteria, null))
					newCriteria = ColumnCriteriaHelper.WrapToType(newCriteria, field.UnboundType);
			} catch {
				isValid = false;
			}
			isChanged = !object.Equals(newCriteria, criteria);
			criteria = newCriteria;
			isValid = isValid && ValidateCriteria();			
			if(!isValid) {
				if(!object.ReferenceEquals(null, criteria))
					isChanged = true;
				criteria = null;
			}
			return isChanged;
		}
		protected virtual bool ValidateCriteria() {
			return true;
		}
	}
	class EmptyDataColumn : UnboundSummaryLevelColumn {
		public EmptyDataColumn(PivotGridFieldBase field, IServerModeHelpersOwner currentOwner, EvaluatorContextDescriptor descriptor)
			: base(field, currentOwner, descriptor) {
		}
		public override bool UpdateCriteria(PivotGridFieldBase field) {
			return base.UpdateCriteria(new PivotGridFieldBase());
		}
	}
	class UnboundSummaryLevelErrorColumn : UnboundSummaryLevelColumn {
		public UnboundSummaryLevelErrorColumn(PivotGridFieldBase field, IServerModeHelpersOwner currentOwner, EvaluatorContextDescriptor descriptor)
			: base(field, currentOwner, descriptor) {
		}
		public override object EvaluateValue(MeasuresStorage storage) {
			return ErrorCell.Value;
		}
		public override PivotCellValue EvaluatePivotCellValue(MeasuresStorage storage) {
			storage.SetFormattedValue(this, ErrorCell.Value, ErrorCell.DisplayText, -1);
			return ErrorCell;
		}
	}
	class UnboundSummaryLevelColumn : UnboundMetadataColumn, IUnboundSummaryLevelMetadataColumn {
		Type dataType;
		EvaluatorContextDescriptor descriptor;
		ExpressionEvaluator expressionEvaluator;
		public override bool IsServer { get { return false; } }
		public override bool IsMeasure { get { return true; } }
		public UnboundSummaryLevelColumn(PivotGridFieldBase field, IServerModeHelpersOwner currentOwner, EvaluatorContextDescriptor descriptor)
			: base(field, currentOwner) {
			this.descriptor = descriptor;
		}
		internal void EnsureExpressionEvaluator() {
			expressionEvaluator = new ExpressionEvaluator(descriptor, Criteria);
		}
		public override bool UpdateCriteria(PivotGridFieldBase field) {
			bool result = base.UpdateCriteria(field);
			this.dataType = field.ActualDataType;
			EnsureExpressionEvaluator();
			return result;
		}
		public virtual PivotCellValue EvaluatePivotCellValue(MeasuresStorage storage) {
			object value;
			try {
				value = expressionEvaluator.Evaluate(storage);
				if(value == null)
					return null;
				if(DataType != typeof(object))
					value = Convert.ChangeType(value, DataType, CultureInfo.CurrentCulture);
			} catch {
				value = null; 
			}
			if(value == null)
				return null;
			storage.SetFormattedValue(this, value, null, -1);
			return new PivotCellValue(value);
		}
		public virtual object EvaluateValue(MeasuresStorage storage) {
			object value;
			try {
				value = expressionEvaluator.Evaluate(storage);
				if(value == null)
					return null;
				if(DataType != typeof(object))
					value = Convert.ChangeType(value, DataType, CultureInfo.CurrentCulture);
			} catch {
				value = null; 
			}
			if(value == null)
				return null;
			storage.SetFormattedValue(this, value, null, -1);
			return value;
		}
		Type IUnboundSummaryLevelMetadataColumn.DataType {
			get { return dataType; }
		}
		public override CriteriaOperator GetGroupCriteria() {
			throw new NotImplementedException("Summary level column unsupports this method");
		}
		public override CriteriaOperator GetRawCriteria() {
			throw new NotImplementedException("Summary level column unsupports this method");
		}
	}
	class UnboundDataSourceLevelColumn : UnboundMetadataColumn {
		ServerModeColumn ownerColumn;
		bool isMeasure;
		CriteriaOperator ServerCriteria {
			get {
				if(ReferenceEquals(null, Criteria) || (CurrentOwner.Executor.CriteriaSyntax & CriteriaSyntax.ServerCriteria) == 0)
					return Criteria;
				return PropertyToQueryOperandPatcher.Patch(Criteria, CurrentOwner);
			}
		}
		public override bool IsServer { get { return true; } }
		public override bool IsMeasure { get { return isMeasure; } }
		internal ServerModeColumn OwnerColumn {
			get { return ownerColumn; }
			set { ownerColumn = value; }
		}
		public override Type SafeDataType {
			get { return base.SafeDataType; }
		}
		public UnboundDataSourceLevelColumn(PivotGridFieldBase field, IServerModeHelpersOwner currentOwner)
			: base(field, currentOwner) {
		}
		protected override bool ValidateCriteria() {
			bool result = base.ValidateCriteria();
			try {
				PropertyToQueryOperandPatcher.Patch(Criteria, CurrentOwner);
			}
			catch {
				return false;
			}
			return result;
		}
		public override CriteriaOperator GetGroupCriteria() {
			return ((IGroupCriteriaConvertible)ownerColumn).GetGroupCriteria();
		}
		public override CriteriaOperator GetRawCriteria() {
			return ServerCriteria;
		}
		public override bool UpdateCriteria(PivotGridFieldBase field) {
			isMeasure = field.Area == PivotArea.DataArea;
			return base.UpdateCriteria(field);
		}
	}
}
