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
#if DXCORE
namespace DevExpress.CodeRush.StructuralParser.VB
#else
namespace DevExpress.CodeParser.VB
#endif
{
  public class VBMethod : Method
  {
	public VBMethod()
	  : base()
	{
	}
	public VBMethod(string typeName, string name)
	  : base(typeName, name)
	{
	}
	#region ToString
	public override string ToString()
	{
	  StringBuilder sb = new StringBuilder();
	  if (MethodType == MethodTypeEnum.Void)
		sb.Append("Sub ");
	  if (MethodType == MethodTypeEnum.Function)
		sb.Append("Function ");
	  sb.Append(InternalName + "(");
	  if (Parameters != null)
		for (int i = 0; i < Parameters.Count; i++)
		  sb.AppendFormat("{0} As {1}{2}",
			((Param)Parameters[i]).InternalName,
			((Param)Parameters[i]).ParamType,
			i == Parameters.Count - 1 ? "" : ","
			);
	  sb.Append(")");
	  if (MethodType == MethodTypeEnum.Function)
		sb.Append(" As " + MemberType);
	  return sb.ToString();
	}
	#endregion
	#region Clone
	public override BaseElement Clone(ElementCloneOptions options)
	{
	  VBMethod clone = new VBMethod();
	  clone.CloneDataFrom(this, options);
	  return clone;
	}
	#endregion
	public override MemberVisibility[] ValidVisibilities
	{
	  get
	  {
		if (IsClassOperator)
		  return new MemberVisibility[] { MemberVisibility.Public };
		MemberVisibility[] visibilites = BaseValidVisibilities;
		if (IsOverride | IsVirtual)
		{
		  visibilites = base.ValidVisibilities;
		  if (visibilites == null)
			return null;
		  List<MemberVisibility> list = new List<MemberVisibility>();
		  foreach (MemberVisibility visibility in visibilites)
			if (visibility != MemberVisibility.Private)
			  list.Add(visibility);
		  return list.ToArray();
		}
		return visibilites;
	  }
	}
	public override bool VisibilityIsFixed
	{
	  get
	  {
		if ((IsStatic && IsConstructor) || IsDestructor || IsClassOperator)
		  return true;
		else
		  return false;
	  }
	}
  }
}
