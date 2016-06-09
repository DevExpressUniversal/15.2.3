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
	public interface IResolvingCache
	{
		void AddElementType(IElement element, IElement type);
		IElement GetElementType(IElement element);
		bool ContainsElementType(IElement element);
		void AddResolvedElement(IElement element, IElement type);
	void AddResolvedElement(IElement element, IElement type, bool resolveMethodGroup);
		IElement GetResolvedElement(IElement element);
	IElement GetResolvedElement(IElement element, bool resolveMethodGroup);
		bool ContainsResolvedElement(IElement element);
	bool ContainsResolvedElement(IElement element, bool resolveMethodGroup);
		void AddElementModules(IElement element, IElementCollection modules);
		IElementCollection GetElementModules(IElement element);
		bool ContainsElementModules(IElement element);
		void AddModuleMembers(IElement element, string name, IElementCollection elements);
		IElementCollection GetModuleMembers(IElement element, string name, bool caseSensitive);
		bool ContainsModuleMembers(IElement element, string name, bool caseSensitive);
	void AddResolvedMethodGroup(IElement element, IElement type);
	IElement GetResolvedMethodGroup(IElement element);
	bool ContainsResolvedMethodGroup(IElement element);
	void AddMetaDataElement(string fullName, IElement element);
	IElement GetMetaDataElement(string fullName);
	bool ContainsMetaDataElement(string fullName);
	void AddTypeRef(IElement type, ITypeReferenceExpression reference);
	ITypeReferenceExpression GetTypeRef(IElement type);
		SiblingHelper GetSiblingHelper(IElement element);
	bool ContainsTypeRef(IElement type);
	}
}
