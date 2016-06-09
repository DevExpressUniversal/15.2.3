#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Dashboard                                                   }
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

using System.Collections.Generic;
using DevExpress.DashboardCommon;
using DevExpress.DashboardCommon.Native;
using DevExpress.Data;
using DevExpress.XtraEditors.Filtering;
namespace DevExpress.DashboardWin.Native {
	public interface IFormatRuleBaseViewInitializationContext {
	}
	public interface IFormatRuleControlViewInitializationContext : IFormatRuleBaseViewInitializationContext {
		string Description { get; }
		bool IsApplyToReadOnly { get; }
		bool IsIntersectionLevel { get; }
		bool IsApplyToColumnSupported { get; }
	}
	public interface IFormatRuleViewTypedContext : IFormatRuleControlViewInitializationContext {
		DataFieldType DataType { get; }
		DateTimeGroupInterval DateTimeGroupInterval { get; }
		bool IsPercentsSupported { get; set; }
	}
	public interface IFormatRuleViewValueContext : IFormatRuleViewTypedContext {
		bool IsValue2Required { get; }
	}
	public interface IFormatRuleViewRangeSetContext : IFormatRuleViewTypedContext {
		StyleMode StyleMode { get; }
	}
	public interface IFormatRuleViewBarContext : IFormatRuleViewTypedContext {
		IFormatRuleBarOptionsInitializationContext BarOptions { get; }
	}
	public interface IFormatRuleViewColorRangeBarContext : IFormatRuleViewRangeSetContext {
		IFormatRuleBarOptionsInitializationContext BarOptions { get; }
	}
	public interface IFormatRuleViewRangeGradientContext : IFormatRuleViewTypedContext {
		FormatConditionRangeGradientPredefinedType PredefinedType { get; }
	}
	public interface IFormatRuleViewGradientRangeBarContext : IFormatRuleViewRangeGradientContext {
		IFormatRuleBarOptionsInitializationContext BarOptions { get; }
	}
	public interface IFormatRuleViewExpressionContext : IFormatRuleControlViewInitializationContext {
		IFilteredComponent FilteredComponent { get; }
		IList<IParameter> Parameters { get; }
		string DefaultItem { get; }
	}
	public interface IFormatRuleBarOptionsInitializationContext {
		bool DrawAxis { get; set; }
		bool AllowNegativeAxis { get; set; }
		bool ShowBarOnly { get; set; }
	}
	public class FormatRulesManageViewInitializationContext : IFormatRuleBaseViewInitializationContext {
	}
	public class FormatRuleControlViewInitializationContext : IFormatRuleControlViewInitializationContext {
		public string Description { get; set; }
		public bool IsApplyToReadOnly { get; set; }
		public bool IsIntersectionLevel { get; set; }
		public bool IsApplyToColumnSupported { get; set; }
	}
	public class FormatRuleControlViewTypedContext : FormatRuleControlViewInitializationContext, IFormatRuleViewTypedContext {
		public DataFieldType DataType { get; set; }
		public DateTimeGroupInterval DateTimeGroupInterval { get; set; }
		public bool IsPercentsSupported { get; set; }
	}
	public class FormatRuleControlViewValueContext : FormatRuleControlViewTypedContext, IFormatRuleViewValueContext {
		public bool IsValue2Required { get; set; }
	}
	public class FormatRuleControlViewRangeSetContext : FormatRuleControlViewTypedContext, IFormatRuleViewRangeSetContext {
		public StyleMode StyleMode { get; set; }
	}
	public class FormatRuleControlViewRangeGradientContext : FormatRuleControlViewTypedContext, IFormatRuleViewRangeGradientContext {
		public FormatConditionRangeGradientPredefinedType PredefinedType { get; set; }
	}
	public class FormatRuleControlViewExpressionContext : FormatRuleControlViewInitializationContext, IFormatRuleViewExpressionContext {
		public IFilteredComponent FilteredComponent { get; set; }
		public IList<IParameter> Parameters { get; set; }
		public string DefaultItem { get; set; }
	}
	public class FormatRuleViewBarContext : FormatRuleControlViewTypedContext, IFormatRuleViewBarContext {
		FormatRuleBarOptionsInitializationContext barOptions = new FormatRuleBarOptionsInitializationContext();
		public IFormatRuleBarOptionsInitializationContext BarOptions { get { return barOptions; } }
	}
	public class FormatRuleViewColorRangeBarContext : FormatRuleControlViewRangeSetContext, IFormatRuleViewColorRangeBarContext {
		FormatRuleBarOptionsInitializationContext barOptions = new FormatRuleBarOptionsInitializationContext();
		public IFormatRuleBarOptionsInitializationContext BarOptions { get { return barOptions; } }
	}
	public class FormatRuleViewGradientRangeBarContext : FormatRuleControlViewRangeGradientContext, IFormatRuleViewGradientRangeBarContext {
		FormatRuleBarOptionsInitializationContext barOptions = new FormatRuleBarOptionsInitializationContext();
		public IFormatRuleBarOptionsInitializationContext BarOptions { get { return barOptions; } }
	}
	public class FormatRuleBarOptionsInitializationContext : IFormatRuleBarOptionsInitializationContext {
		public bool DrawAxis { get; set; }
		public bool AllowNegativeAxis { get; set; }
		public bool ShowBarOnly { get; set; }
	}
}
