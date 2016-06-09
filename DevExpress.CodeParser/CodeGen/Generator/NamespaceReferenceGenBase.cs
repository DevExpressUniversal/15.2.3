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
using System.Collections;
#if DXCORE
namespace DevExpress.CodeRush.StructuralParser
#else
namespace DevExpress.CodeParser
#endif
{
  public abstract class NamespaceReferenceGenBase : LanguageElementCodeGenBase
  {
	public NamespaceReferenceGenBase(CodeGen codeGen)
	  : base(codeGen)
	{
	}
	protected abstract void GenerateExternAlias(ExternAlias element);
	protected abstract void GenerateNamespaceReference(NamespaceReference element);
	protected virtual void GenerateXmlNamespaceReference(XmlNamespaceReference element)
	{
	}
	#region Generate
	public override void GenerateElement(LanguageElement languageElement)
	{
	  if (languageElement == null)
		return;
	  switch (languageElement.ElementType)
	  {
		case LanguageElementType.NamespaceReference:
		  GenerateNamespaceReference(languageElement as NamespaceReference);
		  break;
		case LanguageElementType.ExternAlias:
		  GenerateExternAlias(languageElement as ExternAlias);
		  break;
		case LanguageElementType.XmlNamespaceReference:
		  GenerateXmlNamespaceReference(languageElement as XmlNamespaceReference);
		  break;
	  }
	}
	#endregion
	public override bool GenerateElementTail(LanguageElement element)
	{
	  if (element == null || (element.ElementType != LanguageElementType.ExternAlias
	   && element.ElementType != LanguageElementType.NamespaceReference
	   && element.ElementType != LanguageElementType.XmlNamespaceReference))
		return false;
	  LanguageElement codeSibling = element.NextSibling;
	  if (codeSibling == null)
		return false;
	  CodeGen.AddNewLineIfNeeded();
	  return true;
	}
  }
}
