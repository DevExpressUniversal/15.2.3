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
using DevExpress.Compatibility.System.ComponentModel;
namespace DevExpress.XtraReports.Native {
	public class ClientCriteriaVisitorBase : IClientCriteriaVisitor<CriteriaOperator> {
		protected CriteriaOperator Process(CriteriaOperator criteriaOperator) {
			if(ReferenceEquals(criteriaOperator, null))
				return null;
			return criteriaOperator.Accept(this);
		}
		void ProcessCollection(CriteriaOperatorCollection operands) {
			CriteriaOperatorCollection newOperands = new CriteriaOperatorCollection();
			foreach(CriteriaOperator criteriaOperator in operands)
				newOperands.Add(Process(criteriaOperator));
			operands.Clear();
			foreach(CriteriaOperator criteriaOperator in newOperands)
				operands.Add(criteriaOperator);
		}
		#region IClientCriteriaVisitor Members
		public virtual CriteriaOperator Visit(OperandProperty theOperand) {
			return theOperand;
		}
		public virtual CriteriaOperator Visit(AggregateOperand theOperand) {
			theOperand.Condition = Process(theOperand.Condition);
			theOperand.AggregatedExpression = Process(theOperand.AggregatedExpression);
			return theOperand;
		}
		CriteriaOperator IClientCriteriaVisitor<CriteriaOperator>.Visit(JoinOperand theOperand) {
			theOperand.Condition = Process(theOperand.Condition);
			theOperand.AggregatedExpression = Process(theOperand.AggregatedExpression);
			return theOperand;
		}
		#endregion
		#region ICriteriaVisitor Members
		public virtual CriteriaOperator Visit(FunctionOperator theOperator) {
			ProcessCollection(theOperator.Operands);
			return theOperator;
		}
		public virtual CriteriaOperator Visit(OperandValue theOperand) {
			return theOperand;
		}
		CriteriaOperator ICriteriaVisitor<CriteriaOperator>.Visit(GroupOperator theOperator) {
			ProcessCollection(theOperator.Operands);
			return theOperator;
		}
		public virtual CriteriaOperator Visit(InOperator theOperator) {
			theOperator.LeftOperand = Process(theOperator.LeftOperand);
			ProcessCollection(theOperator.Operands);
			return theOperator;
		}
		CriteriaOperator ICriteriaVisitor<CriteriaOperator>.Visit(UnaryOperator theOperator) {
			theOperator.Operand = Process(theOperator.Operand);
			return theOperator;
		}
		public virtual CriteriaOperator Visit(BinaryOperator theOperator) {
			theOperator.LeftOperand = Process(theOperator.LeftOperand);
			theOperator.RightOperand = Process(theOperator.RightOperand);
			return theOperator;
		}
		CriteriaOperator ICriteriaVisitor<CriteriaOperator>.Visit(BetweenOperator theOperator) {
			theOperator.TestExpression = Process(theOperator.TestExpression);
			theOperator.BeginExpression = Process(theOperator.BeginExpression);
			theOperator.EndExpression = Process(theOperator.EndExpression);
			return theOperator;
		}
		#endregion
	}
	public class DeserializationFilterStringVisitor : ClientCriteriaVisitorBase {
		IExtensionsProvider rootComponent;
		DataContext dataContext;
		object dataSource;
		string dataMember;
		public DeserializationFilterStringVisitor(IExtensionsProvider rootComponent, DataContext dataContext, object dataSource, string dataMember) {
			this.rootComponent = rootComponent;
			this.dataContext = dataContext;
			this.dataSource = dataSource;
			this.dataMember = dataMember;
		}
		public override CriteriaOperator Visit(BinaryOperator theOperator) {
			BinaryOperator result = base.Visit(theOperator) as BinaryOperator;
			OperandProperty leftOperand = theOperator.LeftOperand as OperandProperty;
			if(!ReferenceEquals(leftOperand, null)) {
				TryConvertOperand(leftOperand, theOperator.RightOperand);
			}
			return result;
		}
		public override CriteriaOperator Visit(InOperator theOperator) {
			InOperator result = base.Visit(theOperator) as InOperator;
			OperandProperty leftOperand = result.LeftOperand as OperandProperty;
			CriteriaOperatorCollection operands = result.Operands;
			if(ReferenceEquals(leftOperand, null) || operands == null)
				return result;
			foreach(CriteriaOperator criteriaOperator in operands) {
				TryConvertOperand(leftOperand, criteriaOperator);
			}
			return result;
		}
		void TryConvertOperand(OperandProperty leftOperand, CriteriaOperator criteriaOperator) {
			OperandValue operand = criteriaOperator as OperandValue;
			if(!ReferenceEquals(operand, null) && operand.Value is string) {
				object value;
				Type type = DetectType(leftOperand.PropertyName);
				if(type != null && SerializationService.DeserializeObject((string)operand.Value, type, out value, rootComponent))
					operand.Value = value;
			}
		}
		Type DetectType(string dataMember) {
			string firstPart;
			string secondPart;
			SplitDataMember(dataMember, out firstPart, out secondPart);
			DataBrowser dataBrowser = dataContext.GetDataBrowser(dataSource, BindingHelper.JoinStrings(".", this.dataMember, firstPart), true);
			if(dataBrowser != null && !string.IsNullOrEmpty(secondPart)) {
				PropertyDescriptorCollection properties = dataBrowser.GetItemProperties();
				List<PropertyDescriptor> list = new List<PropertyDescriptor>(PropertyAggregator.Aggregate(properties));
				PropertyDescriptor item = list.Find(delegate(PropertyDescriptor property) { return property.Name == secondPart; });
				if(item != null)
					return item.PropertyType;
			}
			return null;
		}
		static void SplitDataMember(string dataMember, out string first, out string second) {
			int index = dataMember.LastIndexOf(".");
			if(index >= 0) {
				first = dataMember.Substring(0, index);
				second = dataMember.Substring(index + 1, dataMember.Length - index - 1);
			} else {
				first = string.Empty;
				second = dataMember;
			}
		}
	}
}
namespace DevExpress.XtraReports.Native {
	public interface IExtensionsProvider {
		IDictionary<String, String> Extensions { get; }
	}
}
