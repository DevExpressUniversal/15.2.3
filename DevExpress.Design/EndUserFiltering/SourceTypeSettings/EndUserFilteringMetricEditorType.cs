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

namespace DevExpress.Design.Filtering {
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using DevExpress.Utils.Filtering;
	using DevExpress.Utils.Filtering.Internal;
	using System.Linq;
	class EndUserFilteringMetricEditorType : LocalizableEndUserFilteringObject<MetricEditorTypeCodeName>, IFilteringModelMetricEditorType {
		public EndUserFilteringMetricEditorType(MetricEditorTypeCodeName codeName, IMetricAttributes metricAttributes)
			: base(codeName) {
			SetProperties(metricAttributes);
		}
		object currentValue;
		public virtual object CurrentValue {
			get { return this.currentValue; }
			set {
				if(this.currentValue == value) return;
				this.currentValue = value;
			}
		}
		IEnumerable possibleValues;
		public virtual IEnumerable PossibleValues { get { return this.possibleValues; } }
		protected override int GetHash(MetricEditorTypeCodeName codeName) {
			return (int)codeName;
		}
		void SetProperties(IMetricAttributes metricAttributes) {
			if(metricAttributes is IRangeMetricAttributes) {
				IRangeMetricAttributes attributes = metricAttributes as IRangeMetricAttributes;
				if(attributes.IsNumericRange) {
					currentValue = attributes.NumericRangeUIEditorType;
					possibleValues = Enum.GetValues(typeof(RangeUIEditorType));
				}
				if(attributes.IsTimeSpanRange) {
					currentValue = attributes.DateTimeRangeUIEditorType;
					possibleValues = Enum.GetValues(typeof(DateTimeRangeUIEditorType));
				}
			}
			if(metricAttributes is ILookupMetricAttributes) {
				ILookupMetricAttributes attributes = metricAttributes as ILookupMetricAttributes;
				currentValue = attributes.EditorType;
				possibleValues = Enum.GetValues(typeof(LookupUIEditorType));
			}
			if(metricAttributes is IBooleanChoiceMetricAttributes) {
				IBooleanChoiceMetricAttributes attributes = metricAttributes as IBooleanChoiceMetricAttributes;
				currentValue = attributes.EditorType;
				possibleValues = Enum.GetValues(typeof(BooleanUIEditorType));
			}
			if(metricAttributes is IEnumChoiceMetricAttributes) {
				IEnumChoiceMetricAttributes attributes = metricAttributes as IEnumChoiceMetricAttributes;
				currentValue = attributes.EditorType;
				possibleValues = Enum.GetValues(typeof(LookupUIEditorType));
			}
		}
	}
}
