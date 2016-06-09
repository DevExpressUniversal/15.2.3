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
using System.Text;
#if DXCORE
namespace DevExpress.CodeRush.StructuralParser
#else
namespace DevExpress.CodeParser
#endif
{
	public class BitFieldVariable : Variable
	{
		Expression _Size = null;
		protected BitFieldVariable()
		{
		}
		public BitFieldVariable(string type, string name, Expression size)
			: base(type, name)
		{
			_Size = size;
			AddDetailNode(size);
		}
		protected override void CloneDataFrom(BaseElement source, ElementCloneOptions options)
		{			
			if (source == null)
				return;
			base.CloneDataFrom(source, options);
			if (!(source is BitFieldVariable))
				return;			
			BitFieldVariable bitField = (BitFieldVariable)source;
			if (bitField.Size != null)
			{				
				_Size = ParserUtils.GetCloneFromNodes(this, bitField, bitField.Size) as Expression;
				if (_Size == null)
					_Size = bitField.Size.Clone(options) as Expression;
			}
		}
		protected override void ReplaceOwnedReference(LanguageElement oldElement, LanguageElement newElement)
		{
			if (_Size == oldElement)
				_Size = newElement as Expression;
			else
				base.ReplaceOwnedReference(oldElement, newElement);
		}		
		public override BaseElement Clone(ElementCloneOptions options)
		{
			BitFieldVariable bitField = new BitFieldVariable();
			bitField.CloneDataFrom(this, options);
			return bitField;
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public override void CleanUpOwnedReferences()
		{
			_Size = null;
			base.CleanUpOwnedReferences();
		}
		public override LanguageElementType ElementType
		{
			get
			{
				return LanguageElementType.BitFieldVariable;
			}
		}
		public Expression Size
		{
			get
			{
				return _Size;
			}
		}
		public override bool IsBitField
		{
			get
			{
				return true;
			}
		}
		public override Expression BitFieldSize
		{
			get
			{
				return Size;
			}
		}
	}
}
