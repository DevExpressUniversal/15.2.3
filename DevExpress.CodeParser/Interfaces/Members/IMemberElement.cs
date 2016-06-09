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
namespace DevExpress.CodeRush.StructuralParser
#else
namespace DevExpress.CodeParser
#endif
{
	public interface IMemberElement : IElement, IHasAttributes
	{
		string GetOverrideCode();
		string GetOverrideCode(bool callBase);
		string GetOverrideCode(bool callBase, string codeBefore, string codeAfter);
		IExpressionCollection Implements { get; }
		IExpression NameQualifier { get; }
		MemberVisibility Visibility { get; }
		bool IsDefaultVisibility { get; }
		bool IsAbstract { get; }
		bool IsVirtual { get; }
		bool IsOverride { get; }
		bool IsNew { get; }
		bool IsPartial { get; }
	bool IsReadOnly { get; }
		bool IsStatic { get; }
		bool IsSealed { get; }
		bool IsExtern { get; }
		bool IsExplicitInterfaceMember { get; }
	bool IsIterator { get; }
		string Signature { get; }
		bool HasDelimitedBlock { get; }
	}
  public interface IMemberElementModifier : IElementModifier, IHasAttributesModifier
  {
	void SetVisibility(MemberVisibility visibility);
	void SetIsStatic(bool isStatic);
	void SetIsAbstract(bool isAbstract);
	void SetIsSealed(bool isSealed);
	void SetIsVirtual(bool isVirtual);
	void SetIsOverride(bool isOverride);
	void SetIsExtern(bool isExtern);
	void SetIsReadOnly(bool isReadOnly);
  }
}
