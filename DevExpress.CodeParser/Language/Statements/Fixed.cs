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
using System.Collections.Generic;
namespace DevExpress.CodeRush.StructuralParser
#else
namespace DevExpress.CodeParser
#endif
{
	public class Fixed: ParentToSingleStatement, IFixedStatement
	{
		private const int INT_MaintainanceComplexity = 3;
	LanguageElementList _Initializers = new LanguageElementList();
		public Fixed()
		{
		}
	public Fixed(LanguageElementCollection elements)
	{
	  AddInitializers(elements);
	}
	public void AddInitializer(LanguageElement element)
	{
	  if (element == null)
		return;
	  _Initializers.Add(element);
	  AddDetailNode(element);
	}
	public void AddInitializers(LanguageElementCollection elements)
	{
	  if (elements == null)
		return;
	  foreach (LanguageElement initializer in elements)
		AddInitializer(initializer);
	}
		public override int GetImageIndex()
		{
			return ImageIndex.FixedBlock;
		}
		#region Clone
		public override BaseElement Clone(ElementCloneOptions options)
		{
			Fixed lClone = new Fixed();
			lClone.CloneDataFrom(this, options);
			return lClone;
		}
		#endregion
	protected override void CloneDataFrom(BaseElement source, ElementCloneOptions options)
	{
	  if (source == null)
				return;
			base.CloneDataFrom(source, options);
	  Fixed fixedStatement = source as Fixed;
	  if (fixedStatement == null)
		return;
	  if (fixedStatement._Initializers != null)
	  {
		_Initializers = new LanguageElementList();
		ParserUtils.GetClonesFromNodes(DetailNodes, fixedStatement.DetailNodes, _Initializers, fixedStatement._Initializers);
		if (_Initializers.Count == 0 && fixedStatement._Initializers.Count > 0)
		  _Initializers = fixedStatement._Initializers.DeepClone(options) as LanguageElementList;
	  }
	}
	public override void CleanUpOwnedReferences()
	{
	  if (_Initializers != null)
	  {
		_Initializers.Clear();
		_Initializers = null;
	  }
	  base.CleanUpOwnedReferences();
	}
	protected override void ReplaceOwnedReference(LanguageElement oldElement, LanguageElement newElement)
	{
	  if(_Initializers != null && _Initializers.Contains(oldElement))
		_Initializers.Replace(oldElement, newElement);
	  else base.ReplaceOwnedReference(oldElement, newElement);
	}
		public override LanguageElementType ElementType
		{
			get 
			{
				return LanguageElementType.Fixed;
			}
		}
		protected override int ThisMaintenanceComplexity
		{
			get
			{
				return INT_MaintainanceComplexity;
			}
		}
		IVariableDeclarationStatementCollection IFixedStatement.Initializers
		{
	  get
	  {
		LiteVariableDeclarationStatementCollection result = new LiteVariableDeclarationStatementCollection();
		result.AddRange(_Initializers);
		return result;
	  }
		}
	}
}
