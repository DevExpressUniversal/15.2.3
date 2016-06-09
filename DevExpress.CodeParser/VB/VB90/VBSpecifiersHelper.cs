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
using System.Text;
using System.Collections;
using System.Collections.Specialized;
#if DXCORE
namespace DevExpress.CodeRush.StructuralParser.VB
#else
namespace DevExpress.CodeParser.VB
#endif
{
	public sealed class VBSpecifiersHelper
	{
		VBSpecifiersHelper(){}
		public static void ParseSpecifiers(TokenQueueBase queue, AccessSpecifiedElement element)
		{
			if (queue == null || element == null)
				return;
			AccessSpecifiers specifiers = new AccessSpecifiers();
			element.SetAccessSpecifiers(specifiers);
			while (queue.Count > 0)
			{
				Token token = queue.CurrentToken;
				switch (token.Type)
				{
					case TokenType.Private:
						element.SetVisibility(MemberVisibility.Private, token);
						queue.Dequeue();
						break;
					case TokenType.Public:
						element.SetVisibility(MemberVisibility.Public, token);
						queue.Dequeue();
						break;
					case TokenType.Protected:
						element.SetVisibility(MemberVisibility.Protected, token);		
						queue.Dequeue();
						break;
					case TokenType.Friend:
						element.SetVisibility(MemberVisibility.Internal, token);		
						queue.Dequeue();
						break;
					case TokenType.Overridable:
						element.IsVirtual = true;
						element.SetVirtualOverrideAbstractRange(token);
						queue.Dequeue();
						break;
					case TokenType.Overloads:
						element.IsOverloads = true;
						element.SetOverloadsRange(token);
						queue.Dequeue();
						break;
					case TokenType.Overrides:
						element.IsOverride = true;
						element.SetVirtualOverrideAbstractRange(token);
						queue.Dequeue();
						break;
					case TokenType.Shared:
					case TokenType.Static:
						element.IsStatic = true;
						element.SetStaticRange(token);
						queue.Dequeue();
						break;
					case TokenType.MustInherit:
						element.IsAbstract = true;
						element.SetVirtualOverrideAbstractRange(token);
						queue.Dequeue();
						break;
					case  TokenType.MustOverride:
						if (!element.IsAbstract)
						{
							element.IsAbstract = true;
							element.SetVirtualOverrideAbstractRange(token);
							queue.Dequeue();
						}
						break;
					case TokenType.Shadows:
						element.IsNew = true;
						element.SetNewRange(token);
						queue.Dequeue();
						break;
					case TokenType.WithEvents:
						element.IsWithEvents = true;
						element.SetWithEventsRange(token);
						queue.Dequeue();
						break;
					case TokenType.Readonly:
						element.IsReadOnly = true;
						element.SetReadOnlyRange(token);
						queue.Dequeue();
						break;
					case TokenType.WriteOnly:
						element.IsWriteOnly = true;
						element.SetWriteOnlyRange(token);
						queue.Dequeue();
						break;
					case TokenType.Default:
						element.IsDefault = true;
						element.SetDefaultRange(token);
						queue.Dequeue();
						break;
					case TokenType.NotInheritable:
						element.IsSealed = true;
						element.SetSealedRange(token);
						queue.Dequeue();
						break;
					case TokenType.NotOverridable:
						if (!element.IsSealed)
						{
							element.IsSealed = true;
							element.SetSealedRange(token);
							queue.Dequeue();
						}
						break;
					case TokenType.Dim:
						queue.Dequeue();
						break;
					case TokenType.Const:
						queue.Dequeue();
						break;
					case TokenType.Partial:
						queue.Dequeue();
						if (!(element is TypeDeclaration))
							break;
						TypeDeclaration lType = (TypeDeclaration)element;
						lType.IsPartial = true;						
						break;
					default:
						return;
				}
			}
		}
	}	
}
