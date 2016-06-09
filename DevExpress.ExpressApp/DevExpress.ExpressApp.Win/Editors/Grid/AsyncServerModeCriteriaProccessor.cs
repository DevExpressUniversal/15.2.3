#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       eXpressApp Framework                                        }
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
using DevExpress.ExpressApp;
using DevExpress.Persistent.Base;
using DevExpress.ExpressApp.DC;
using DevExpress.Data.Filtering;
namespace DevExpress.ExpressApp.Win.Editors {
	public class AsyncServerModeCriteriaProccessor : CriteriaProcessorBase {
		private ITypeInfo objectTypeInfo;
		public AsyncServerModeCriteriaProccessor(ITypeInfo objectTypeInfo) {
			this.objectTypeInfo = objectTypeInfo;
		}
		protected override void Process(OperandValue theOperand) {
			base.Process(theOperand);
			object value = theOperand.Value;
			if(!ReferenceEquals(theOperand.Value, null)) {
				ITypeInfo typeInfo = XafTypesInfo.Instance.FindTypeInfo(value.GetType());
				if(typeInfo != null && typeInfo.DeclaredDefaultMember != null)
					theOperand.Value = ReflectionHelper.GetMemberValue(value, typeInfo.DeclaredDefaultMember.BindingName);
			}
		}
		protected override void Process(OperandProperty theOperand) {
			base.Process(theOperand);
			IMemberInfo displayableMemberDescriptor = ReflectionHelper.FindDisplayableMemberDescriptor(objectTypeInfo, theOperand.PropertyName);
			if(displayableMemberDescriptor != null)
				theOperand.PropertyName = displayableMemberDescriptor.BindingName;
		}
	}
}
