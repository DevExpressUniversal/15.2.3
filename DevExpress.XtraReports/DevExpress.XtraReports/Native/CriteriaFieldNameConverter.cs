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
using System.Text;
using DevExpress.Data.Filtering;
using DevExpress.XtraReports.UI;
using DevExpress.Data.Native;
using DevExpress.Data.Browsing;
using System.ComponentModel;
using DevExpress.XtraReports.Native.Data;
using DevExpress.Data.Filtering.Helpers;
namespace DevExpress.XtraReports.Native {
	public static class CriteriaFieldNameConverter {
		public static string ToDisplayNames(XtraReportBase report, object dataSource, string dataMember, string source) {
			using(var provider = new DataContextProvider(report)) {
				return ToDisplayNames(provider.DataContext, dataSource, dataMember, source);
			}
		}
		public static string ToRealNames(XtraReportBase report, object dataSource, string dataMember, string source) {
			using(var provider = new DataContextProvider(report)) {
				return ToRealNames(provider.DataContext, dataSource, dataMember, source);
			}
		}
		public static string ToDisplayNames(XRDataContextBase dataContext, object dataSource, string dataMember, string source) {
			return Convert(source, new DisplayNameFilterStringVisitor(dataContext, dataSource, dataMember));
		}
		public static string ToRealNames(XRDataContextBase dataContext, object dataSource, string dataMember, string source) {
			return Convert(source, new RealNameFilterStringVisitor(dataContext, dataSource, dataMember));
		}
		public static string ToDisplayName(XtraReportBase report, object dataSource, string dataMember, string realName) {
			using(var provider = new DataContextProvider(report)) {
				return EmbeddedFieldsHelper.GetShortFieldDisplayName(provider.DataContext, dataSource, dataMember, realName);
			}
		}
		static string Convert(string source, FilterStringVisitorBase visitor) {
			CriteriaOperator result = CriteriaOperator.Parse(source);
			if(!Object.ReferenceEquals(result, null)) {
				result = (CriteriaOperator)result.Accept(visitor);
				try {
					int i = 0;
					return new CriteriaLexerTokenHelper(source).ConvertProperties(true, (l, n) => EscapeFieldName(visitor.ResultNames[i++]));
				} catch(ArgumentOutOfRangeException e) {
					throw new Exception("Invalid expression: " + source, e);
				}
			}
			return new CriteriaLexerTokenHelper(source).ConvertProperties(true, (l, n) => EscapeFieldName(n));
		}
		public static string EscapeFieldName(string fieldName) {
			return fieldName.Replace("]", "\\]");
		}
	}
	public class DisplayNameFilterStringVisitor : FilterStringVisitorBase {
		public DisplayNameFilterStringVisitor(XRDataContextBase dataContext, object dataSource, string dataMember)
			: base(dataContext, dataSource, dataMember) {
		}
		protected override string ConvertNameCore(string name) {
			return EmbeddedFieldsHelper.GetShortFieldDisplayName(dataContext, dataSource, dataMember, name);
		}
	}
	public class RealNameFilterStringVisitor : FilterStringVisitorBase {
		public RealNameFilterStringVisitor(XRDataContextBase dataContext, object dataSource, string dataMember)
			: base(dataContext, dataSource, dataMember) {
		}
		protected override string ConvertNameCore(string name) {
			return EmbeddedFieldsHelper.GetShortFieldRealName(dataContext, dataSource, dataMember, name);
		}
	}
	public abstract class FilterStringVisitorBase : ClientCriteriaVisitorBase {
		protected XRDataContextBase dataContext;
		protected object dataSource;
		protected string dataMember;
		List<string> proprtyElements = new List<string>();
		List<string> resultNames = new List<string>();
		public IList<string> ResultNames { get { return resultNames; } }
		public FilterStringVisitorBase(XRDataContextBase dataContext, object dataSource, string dataMember) {
			this.dataContext = dataContext;
			this.dataSource = dataSource;
			this.dataMember = dataMember;
		}
		protected abstract string ConvertNameCore(string name);
		public override CriteriaOperator Visit(OperandProperty theOperand) {
			resultNames.Add(ConvertName(theOperand.PropertyName));
			return theOperand;
		}
		public override CriteriaOperator Visit(AggregateOperand theOperand) {
			if(object.ReferenceEquals(theOperand.CollectionProperty, null))
				return base.Visit(theOperand);
			if(theOperand.CollectionProperty.PropertyName == string.Empty) {
				resultNames.Add(string.Empty);
				return base.Visit(theOperand);
			}
			string originalName = theOperand.CollectionProperty.PropertyName;
			resultNames.Add(ConvertName(originalName));
			string[] originalNameParts = originalName.Split('.');
			proprtyElements.AddRange(originalNameParts);
			try {
				return base.Visit(theOperand);
			} finally {
				proprtyElements.RemoveRange(proprtyElements.Count - originalNameParts.Length, originalNameParts.Length);
			}
		}
		string ConvertName(string name) {
			return SkipPrefix(ConvertNameCore(GetPropertyName(name)));
		}
		string SkipPrefix(string displayName) {
			string[] elements = displayName.Split('.');
			return string.Join(".", elements, proprtyElements.Count, elements.Length - proprtyElements.Count);
		}
		string GetPropertyName(string propertyName) {
			proprtyElements.Add(propertyName);
			try {
				return string.Join(".", proprtyElements.ToArray());
			} finally {
				proprtyElements.RemoveAt(proprtyElements.Count - 1);
			}
		}
	}
}
