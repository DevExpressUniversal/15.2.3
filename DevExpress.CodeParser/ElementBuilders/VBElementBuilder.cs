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
#if DXCORE
using DevExpress.CodeRush.StructuralParser;
using DevExpress.CodeRush.StructuralParser.VB;
namespace DevExpress.CodeRush.StructuralParser
#else
using DevExpress.CodeParser.VB;
namespace DevExpress.CodeParser
#endif
{
	public class VBElementBuilder: ElementBuilder
	{
		const string VBSTR_SystemInt32 = "System.Int32";
	public VBElementBuilder()
	{
	}
	public override Method BuildMethod(string memberType, string name)
	{
	  Method method = new VBMethod(memberType, name);
	  SetMethodType(method, memberType);
	  return method;
	}
	public override Event BuildEvent()
	{
	  return new VBEvent();
	}
	public override Event BuildEvent(string name)
	{
	  return new VBEvent(name);
	}
	public VBFor BuildFor(string iteratorVar, string toExpr, string stepExp)
		{
	  VBFor lNewFor = new VBFor();
			InitializedVariable lInitializedVariable = BuildInitializedVariable(VBSTR_SystemInt32, iteratorVar, "0");
			lNewFor.AddInitializer(lInitializedVariable);
			if (toExpr != null && toExpr != String.Empty)
			{
				Expression toExpression = new PrimitiveExpression(toExpr);
		((VBFor)lNewFor).ToExpression = toExpression;
			}
			if (stepExp != null && stepExp != String.Empty)
			{
				Expression stepExpression = new PrimitiveExpression(stepExp);
		((VBFor)lNewFor).StepExpression = stepExpression;				
			}
			return lNewFor;
		}
		public override For BuildFor(string iteratorVar, Expression endCondition)
		{
			return BuildFor(iteratorVar, ((BinaryOperatorExpression)endCondition).RightSide.Name, "1");
		}
		public override For BuildFor(Expression endCondition)
		{
			return BuildFor("i", endCondition);
		}
		#region AddFor(string iteratorVar, string toExpr, string stepExp)
		public For AddFor(LanguageElement parent, string iteratorVar, string toExpr, string stepExp)
		{
	  return (VBFor)AddNode(parent, BuildFor(iteratorVar, toExpr, stepExp));
		}
		#endregion
	public override Method BuildDestructor(string className)
	{
	  Method destructor = base.BuildDestructor(className);
	  destructor.Visibility = MemberVisibility.Protected;
	  destructor.SetAccessSpecifiers(new AccessSpecifiers() { IsOverride = true });
	  return destructor;
	}
	}
}
