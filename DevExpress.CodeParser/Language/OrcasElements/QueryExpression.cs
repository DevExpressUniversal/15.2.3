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
	#region QueryExpression
	public class QueryExpression : QueryExpressionBase, IQueryExpression
	{
		Expression _Translation;
		bool _IsTranslation;
		Expression BuildQueryTranslation()
		{
	  return StructuralParserServicesHolder.BuildQueryTranslation(this);
		}
		#region CloneDataFrom
		protected override void CloneDataFrom(BaseElement source, ElementCloneOptions options)
		{			
			if (source == null)
				return;
			base.CloneDataFrom(source, options);
			if (!(source is QueryExpression))
				return;
			QueryExpression lSource = source as QueryExpression;
			if (lSource._Translation != null)
			{
				_Translation = lSource._Translation.Clone(options) as Expression;
				if (_Translation != null)
					_Translation.SetParent(this);
			}
		}
		#endregion
		#region Clone
		public override BaseElement Clone(ElementCloneOptions options)
		{
			QueryExpression clone = new QueryExpression();
			clone.CloneDataFrom(this, options);
			return clone;
		}
		#endregion
		#region ToString
		public override string ToString()
		{
			return "QueryExpression";
		}
		#endregion
		public override IElement Resolve(ISourceTreeResolver resolver)
		{
			return resolver.Resolve(this);
		}
		#region ElementType
		public override LanguageElementType ElementType
		{
			get
			{
				return LanguageElementType.QueryExpression;
			}
		}
		#endregion				
		public Expression Translation
		{
			get
			{
				if (_Translation == null)
					_Translation = BuildQueryTranslation();
				return _Translation;
			}
	  set
	  {
		_Translation = value;
	  }
		}
		public bool IsTranslation
		{
			get
			{
				return _IsTranslation;
			}
			set
			{
				_IsTranslation = value;
			}
		}
		#region IQueryExpression members...
		IExpression IQueryExpression.Translation
		{
			get
			{
				return Translation;
			}
		}
		#endregion
	}
	#endregion
}
