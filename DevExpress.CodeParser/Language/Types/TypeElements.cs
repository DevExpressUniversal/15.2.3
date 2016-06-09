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
#if DXCORE
namespace DevExpress.CodeRush.StructuralParser
#else
namespace DevExpress.CodeParser
#endif
{
	public class ArrayNameModifier : LanguageElement, IArrayNameModifier
	{
		int _Rank;
	SourceRange _NameRange = SourceRange.Empty;
		ExpressionCollection _SizeInitializers;
		public ArrayNameModifier(int rank, ExpressionCollection sizeInitializers)
		{
			_Rank = rank;
			_SizeInitializers = sizeInitializers;
			AddDetailNodes(sizeInitializers);
		}
	#region CloneDataFrom
	protected override void CloneDataFrom(BaseElement source, ElementCloneOptions options)
	{
	  if (source == null)
		return;
	  base.CloneDataFrom(source, options);
	  if (!(source is ArrayNameModifier))
		return;
	  ArrayNameModifier lSource = (ArrayNameModifier)source;
	  _Rank = lSource._Rank;
	  _NameRange = lSource.NameRange;
	  if (lSource._SizeInitializers != null)
	  {
		_SizeInitializers = new ExpressionCollection();
		ParserUtils.GetClonesFromNodes(DetailNodes, lSource.DetailNodes, _SizeInitializers, lSource._SizeInitializers);
		if (_SizeInitializers.Count == 0 && lSource._SizeInitializers.Count > 0)
		  _SizeInitializers = lSource._SizeInitializers.DeepClone(options) as ExpressionCollection;
	  }
	}
	#endregion
	protected override void UpdateRanges()
	{
	  base.UpdateRanges();
	  _NameRange = NameRange;
	}
		protected override void ReplaceOwnedReference(LanguageElement oldElement, LanguageElement newElement)
		{
			if (_SizeInitializers != null && _SizeInitializers.Contains(oldElement))
				_SizeInitializers.Replace(oldElement, newElement);
			else
				base.ReplaceOwnedReference(oldElement, newElement);
		}
		public override int GetImageIndex()
		{
			return ImageIndex.TypeReference;
		}
		public override string ToString()
		{
			StringBuilder lResult = new StringBuilder();
			lResult.Append("[");
			if (SizeInitializers != null && SizeInitializers.Count > 0)
			{
				string lComma = String.Empty;
				for (int i = 0; i < Rank; i++)
				{
					lResult.AppendFormat("{0}{1}", lComma, SizeInitializers[i].ToString());
					lComma = ", ";
				}
			}
			else if (Rank > 1)
			{
				for (int i = 0; i < Rank - 1; i++)
					lResult.Append(",");
			}
			lResult.Append("]");
			return lResult.ToString();
		}
		public override LanguageElementType ElementType
		{
			get
			{
				return LanguageElementType.ArrayNameModifier;
			}
		}
		public int Rank
		{
			get
			{
				return _Rank;
			}
			set
			{
				_Rank = value;
			}
		}
		public ExpressionCollection SizeInitializers
		{
			get
			{
				if (_SizeInitializers == null)
					_SizeInitializers = new ExpressionCollection();
				return _SizeInitializers;
			}
		}
	public override SourceRange NameRange
	{
	  get
	  {
		return GetTransformedRange(_NameRange);
	  }
	  set
	  {
		ClearHistory();
		_NameRange = value;
	  }
	}
		IExpressionCollection IArrayNameModifier.SizeInitializers
		{
			get
			{
				if (_SizeInitializers == null)
					_SizeInitializers = new ExpressionCollection();
				return _SizeInitializers;
			}
		}
	}
	public class NullableTypeModifier: LanguageElement
	{
		public override LanguageElementType ElementType
		{
			get
			{
				return LanguageElementType.NullableTypeModifier;
			}
		}
		#region Clone
		public override BaseElement Clone(ElementCloneOptions options)
		{
			NullableTypeModifier lClone = new NullableTypeModifier();
			lClone.CloneDataFrom(this, options);
			return lClone;
		}
		#endregion
		public override string ToString()
		{
			return "NullableModifier";
		} 
	}
}
