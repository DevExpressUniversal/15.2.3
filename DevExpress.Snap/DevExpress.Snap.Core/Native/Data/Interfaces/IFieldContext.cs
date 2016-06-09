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

using DevExpress.Snap.Core.Native.Data.Implementations;
using DevExpress.Data.Browsing.Design;
using DevExpress.Snap.Core.Fields;
using DevExpress.Data;
using DevExpress.Office.Utils;
using DevExpress.Data.Browsing;
using System;
using DevExpress.XtraPrinting.Native;
using DevExpress.XtraRichEdit.Fields;
using DevExpress.Snap.Core.API;
namespace DevExpress.Snap.Core.Native.Data {
	public interface IFieldContext {
		void Accept(IFieldContextVisitor visitor);
		T Accept<T>(IFieldContextVisitor<T> visitor);
	}
	public interface IFieldContextService {
		ICalculationContext BeginCalculation(IDataSourceDispatcher dataSourceDispatcher);
		void EndCalculation(ICalculationContext calculationContext);
		ParameterCollection Parameters { get; }
	}
	public interface ICalculationContext {
		IDataEnumerator GetChildDataEnumerator(IFieldContext fieldContext, FieldPathDataMemberInfo path);
		object GetRawValue(IFieldContext fieldContext, FieldPathDataMemberInfo path);
		object GetSummaryValue(IFieldContext fieldContext, FieldPathDataMemberInfo path, SummaryRunning summaryRunning, SummaryItemType summaryFunc, bool ignoreNullValues, int groupLevel);
		SnapListController RestoreListController(IFieldContext fieldContext);
		IFieldContext GetRawValueFieldContext(IFieldContext fieldContext, FieldPathDataMemberInfo fieldPathDataMemberInfo);
		void PrepareDataContext(DataContext dataContext, IFieldContext fieldContext);
		string[] FieldNames { get; set; }
	}
	public interface IFieldContextVisitor<T> {
		T Visit(EmptyFieldContext context);
		T Visit(ProxyFieldContext context);
		T Visit(RootFieldContext context);
		T Visit(SnapMailMergeRootFieldContext context);
		T Visit(SingleListItemFieldContext context);
		T Visit(ListFieldContext context);
		T Visit(SimplePropertyFieldContext context);
	}
	public interface IFieldContextVisitor {
		void Visit(EmptyFieldContext context);
		void Visit(ProxyFieldContext context);
		void Visit(RootFieldContext context);
		void Visit(SnapMailMergeRootFieldContext context);
		void Visit(SingleListItemFieldContext context);
		void Visit(ListFieldContext context);
		void Visit(SimplePropertyFieldContext context);
	}   
}
