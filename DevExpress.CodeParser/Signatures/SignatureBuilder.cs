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
using System.ComponentModel;
#if DXCORE
namespace DevExpress.CodeRush.StructuralParser
#else
namespace DevExpress.CodeParser
#endif
{	
	[EditorBrowsable(EditorBrowsableState.Never)]
	public static class SignatureBuilder
	{
		public static string GetSignature(IElement element)
		{
			string name; 
			int nameIndex;
			return GetSignature(element, false, out name, out nameIndex);
		}
	[EditorBrowsable(EditorBrowsableState.Never)]
	public static string GetSignatureForXmlDocumentation(IMemberElement member)
	{
	  if (member == null)
		return String.Empty;
	  string name;
	  int nameIndex;
	  return MemberSignatureBuilder.GetSignatureForXmlDocumentation(member, false, out name, out nameIndex);
	}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public static string GetSignature(IElement element, bool buildGenerics, out string name, out int nameIndex)
	{
			name = String.Empty;
			nameIndex = -1;
			if (element == null)
				return String.Empty;
			name = element.Name;
			nameIndex = 0;
			if (element is IExpression)
				return ExpressionSignatureBuilder.GetSignature((IExpression)element);
	  if (element is IMemberElement)
		return MemberSignatureBuilder.GetSignature((IMemberElement)element, buildGenerics, out name, out nameIndex);
			if (element is XmlNode)
		return XmlSignatureBuilder.GetSignature((XmlNode)element);
	  if (element is BaseCssElement)
		return XmlSignatureBuilder.GetSignature((BaseCssElement)element);
			return element.Name;
	}
	}
}
