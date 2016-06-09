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

namespace DevExpress.Utils.Filtering.Internal {
	using System;
	using System.Collections.Generic;
	public interface IMetricAttributes {
		void UpdateMemberBindings(object viewModel, string propertyName, IMetricAttributesQuery membersQuery);
	}
	public interface IMetricAttributes<T> : IMetricAttributes { }
	public interface IRangeMetricAttributes : IMetricAttributes {
		bool IsNumericRange { get; }
		bool IsTimeSpanRange { get; }
		RangeUIEditorType NumericRangeUIEditorType { get; }
		DateTimeRangeUIEditorType DateTimeRangeUIEditorType { get; }
		string FromName { get; }
		string ToName { get; }
	}
	public interface IRangeMetricAttributes<T> : IMetricAttributes<T>,
		IRangeMetricAttributes where T : struct {
		T? Minimum { get; }
		T? Maximum { get; }
		T? Average { get; }
	}
	public interface IBaseLookupMetricAttributes : IMetricAttributes {
		LookupUIEditorType EditorType { get; }
		bool UseFlags { get; }
		bool UseSelectAll { get; }
		string SelectAllName { get; }
		string NullName { get; }
	}
	public interface ILookupMetricAttributes : IBaseLookupMetricAttributes {
		int? Top { get; }
		int? MaxCount { get; }
		object DataSource { get; }
		string DisplayMember { get; }
		string ValueMember { get; }
	}
	public interface ILookupMetricAttributes<T> : IMetricAttributes<T>,
		ILookupMetricAttributes {
	}
	public interface IChoiceMetricAttributes<T> :
		IMetricAttributes<T> where T : struct {
	}
	public interface IBooleanChoiceMetricAttributes : IChoiceMetricAttributes<bool> {
		BooleanUIEditorType EditorType { get; }
		string TrueName { get; }
		string FalseName { get; }
		string DefaultName { get; }
		bool? DefaultValue { get; }
	}
	public interface IEnumChoiceMetricAttributes : IBaseLookupMetricAttributes {
		Type EnumType { get; }
	}
	public interface IEnumChoiceMetricAttributes<T> : IChoiceMetricAttributes<T>,
		IEnumChoiceMetricAttributes where T : struct {
	}
}
