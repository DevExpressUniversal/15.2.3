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

#if DXCORE
namespace DevExpress.CodeRush.StructuralParser.VB
#else
namespace DevExpress.CodeParser.VB
#endif
{
	public enum CorruptedType
	{
		Class,
		Struct,
		Module,
		Interface,
		Event,
		Namespace,
		AddHandler,
		RemoveHandler,
		RaiseEvent,
		Get,
		Set,
		Enum,
		Property,
		Sub,
		Function,
		Operator,
		Block
	}
	public class CorruptedLanguageElement : LanguageElement, ICorruptedElement
	{
		CorruptedType _CorruptedType = CorruptedType.Block;
		SourceRange _EndBlockRange = SourceRange.Empty;
		public CorruptedLanguageElement()
		{
		}
		protected override void UpdateRanges()
		{
			base.UpdateRanges();
			_EndBlockRange = EndBlockRange;
		}
		#region CloneDataFrom
		protected override void CloneDataFrom(BaseElement source, ElementCloneOptions options)
		{
			if (source == null)
				return;
			base.CloneDataFrom(source, options);
			CorruptedLanguageElement corruptedElement = source as CorruptedLanguageElement;
			if (corruptedElement == null)
				return;
			_CorruptedType = corruptedElement._CorruptedType;
			_EndBlockRange = corruptedElement._EndBlockRange;
		}
		#endregion
		#region Clone
		public override BaseElement Clone(ElementCloneOptions options)
		{
			CorruptedLanguageElement clone = new CorruptedLanguageElement();
			clone.CloneDataFrom(this, options);
			return clone;
		}
		#endregion
		public SourceRange EndBlockRange
		{
			get
			{
				return GetTransformedRange(_EndBlockRange);
			}
			set
			{
				ClearHistory();
				_EndBlockRange = value;
			}
		}
		public override LanguageElementType ElementType
		{
			get { return LanguageElementType.CorruptedLanguageElement; }
		}
		public CorruptedType CorruptedType
		{
			get { return _CorruptedType; }
			set { _CorruptedType = value; }
		}
		public bool IsCorrupted
		{
			get { return true; }
		}
	}
}
