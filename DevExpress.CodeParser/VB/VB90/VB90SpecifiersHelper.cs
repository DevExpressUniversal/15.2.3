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
namespace DevExpress.CodeRush.StructuralParser.VB
#else
namespace DevExpress.CodeParser.VB
#endif
{
	public sealed class VB90SpecifiersHelper
	{
		VB90SpecifiersHelper(){}
		public static void ParseSpecifiers(TokenQueueBase queue, Accessor element)
		{
			if (queue == null || element == null)
				return;
			SetBlockRange(element, queue);
			while (queue.Count > 0)
			{
				Token token = queue.CurrentToken;
				switch (token.Type)
				{
					case Tokens.Private:
						element.Visibility = MemberVisibility.Private;
			element.VisibilityRange = token.Range;
						queue.Dequeue();
						break;
					case Tokens.Public:
						element.Visibility = MemberVisibility.Public;
			element.VisibilityRange = token.Range;
						queue.Dequeue();
						break;
					case Tokens.Protected:
						element.Visibility = MemberVisibility.Protected;
			element.VisibilityRange = token.Range;
						queue.Dequeue();
						if (queue.Count > 0 && queue.CurrentToken.Type == Tokens.Friend)
						{
							element.Visibility = MemberVisibility.ProtectedInternal;
			  element.VisibilityRange = new SourceRange(element.VisibilityRange.Start, queue.CurrentToken.Range.End);
			  queue.Dequeue();
						}
						break;
					case Tokens.Friend:
						element.Visibility = MemberVisibility.Internal;
			element.VisibilityRange = token.Range;
						queue.Dequeue();
						break;
					case Tokens.Overridable:
						element.IsVirtual = true;
						element.VirtualRange = token.Range;
						queue.Dequeue();
						break;
					case Tokens.Overloads:
						element.IsOverloads = true;
						queue.Dequeue();
						break;
					case Tokens.Overrides:
						element.IsOverride = true;
						element.OverrideRange = token.Range;
						queue.Dequeue();
						break;
					case Tokens.Shared:
					case Tokens.Static:
						element.IsStatic = true;
						queue.Dequeue();
						break;
					case Tokens.MustInherit:
						element.IsAbstract = true;
						queue.Dequeue();
						break;
					case Tokens.MustOverride:
						if (!element.IsAbstract)
						{
							element.IsAbstract = true;
							queue.Dequeue();
						}
						break;
					case Tokens.Shadows:
						element.IsNew = true;
						queue.Dequeue();
						break;
					case Tokens.WithEvents:
						element.IsWithEvents = true;
						queue.Dequeue();
						break;
					case Tokens.ReadOnly:
						element.IsReadOnly = true;
						queue.Dequeue();
						break;
					case Tokens.WriteOnly:
						element.IsWriteOnly = true;
						queue.Dequeue();
						break;
					case Tokens.Default:
						element.IsDefault = true;
						queue.Dequeue();
						break;
					case Tokens.NotInheritable:
						element.IsSealed = true;
						queue.Dequeue();
						break;
					case Tokens.NotOverridable:
						if (!element.IsSealed)
						{
							element.IsSealed = true;
							queue.Dequeue();
						}
						break;
					case Tokens.Dim:
						queue.Dequeue();
						break;
					case Tokens.Const:
						queue.Dequeue();
						break;
					case Tokens.Partial:
						queue.Dequeue();
						break;
					default:
						return;
				}
			}
		}
		static void SetBlockRange(DelimiterCapableBlock block, TokenQueueBase queue)
		{
			if (block == null || queue == null || queue.Count == 0)
				return;
			Token startToken = queue.CurrentToken;
			block.SetBlockStart(startToken.Range);
		}
		public static void ParseSpecifiers(TokenQueueBase queue, AccessSpecifiedElement element)
		{
			if (queue == null || element == null)
				return;
			SetBlockRange(element, queue);
	  if (element.AccessSpecifiers == null)
	  {
		AccessSpecifiers specifiers = new AccessSpecifiers();
		element.SetAccessSpecifiers(specifiers);
	  }
			int currentPos = 0;
			int count = queue.Count;
			while (count > currentPos)
			{
				Token token = queue.Peek(currentPos); 
				currentPos++;
				switch (token.Type)
				{
					case Tokens.Partial:
						element.IsPartial = true;
						element.SetPartialRange(token);
						break;
					case Tokens.Widening:
						if ((element != null) && (element is Method))
						{
							((Method)element).IsImplicitCast = true;
							((Method)element).Name = "op_Implicit";							
						}
						break;
					case Tokens.Narrowing:
						if ((element != null) && (element is Method))
						{
							((Method)element).IsExplicitCast = true;
							((Method)element).Name = "op_Explicit";
						}
						break;
					case Tokens.Private:
						element.SetVisibility(MemberVisibility.Private, token);
						break;
		  case Tokens.Identifier:
			string value = token.Value.ToLower();
			switch(value)
			{
			  case "async":
				element.AccessSpecifiers.SetAsynchronousRange(token);
				element.AccessSpecifiers.IsAsynchronous = true;
				break;
			  case "iterator":
				element.AccessSpecifiers.SetIteratorRange(token);
				element.AccessSpecifiers.IsIterator = true;
				break;
			}
			break;
					case Tokens.Public:
						element.SetVisibility(MemberVisibility.Public, token);
						break;
					case Tokens.Protected:
						element.SetVisibility(MemberVisibility.Protected, token);		
						break;
					case Tokens.Friend:
						element.SetVisibility(MemberVisibility.Internal, token);		
						break;
					case Tokens.Overridable:
						element.IsVirtual = true;
						element.SetVirtualOverrideAbstractRange(token);
						break;
					case Tokens.Overloads:
						element.IsOverloads = true;
						element.SetOverloadsRange(token);
						break;
					case Tokens.Overrides:
						element.IsOverride = true;
						element.SetVirtualOverrideAbstractRange(token);
						break;
					case Tokens.Shared:
					case Tokens.Static:
						element.IsStatic = true;
						element.SetStaticRange(token);
						break;
					case Tokens.MustInherit:
						element.IsAbstract = true;
						element.SetVirtualOverrideAbstractRange(token);
						break;
					case Tokens.MustOverride:
						if (!element.IsAbstract)
						{
							element.IsAbstract = true;
							element.SetVirtualOverrideAbstractRange(token);
						}
						break;
					case Tokens.Shadows:
						element.IsNew = true;
						element.SetNewRange(token);
						break;
					case Tokens.WithEvents:
						element.IsWithEvents = true;
						element.SetWithEventsRange(token);
						break;
					case Tokens.ReadOnly:
						element.IsReadOnly = true;
						element.SetReadOnlyRange(token);
						break;
					case Tokens.WriteOnly:
						element.IsWriteOnly = true;
						element.SetWriteOnlyRange(token);
						break;
					case Tokens.Default:
						element.IsDefault = true;
						element.SetDefaultRange(token);
						break;
					case Tokens.NotInheritable:
						element.IsSealed = true;
						element.SetSealedRange(token);
						break;
					case Tokens.NotOverridable:
						if (!element.IsSealed)
						{
							element.IsSealed = true;
							element.SetSealedRange(token);
						}
						break;
					case Tokens.Dim:
						break;
					case Tokens.Const:
						break;
					default:
						return;
				}
			}
		}
	}
}
