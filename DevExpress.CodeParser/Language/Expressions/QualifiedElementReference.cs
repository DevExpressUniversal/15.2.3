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
	public class QualifiedElementReference : ElementReferenceExpression, IQualifiedElementReferenceExpression
	{
		Expression _Source;
		internal QualifiedElementReference()			
		{
		}
		public QualifiedElementReference(string name)
			: base(name)
		{
		}
		public QualifiedElementReference(string name, SourceRange namerange)
			: this(name)
		{
			NameRange = namerange;
		}
		public QualifiedElementReference(Expression source, string name, SourceRange namerange)
			: this(name, namerange)
		{
	  SetSource(source);
		}
	void SetSource(Expression source)
	{
	  if (_Source != null)
		RemoveNode(_Source);
	  _Source = source;
	  if (_Source != null)
		AddNode(_Source);
	}
		#region CleanUpOwnedReferences
		[EditorBrowsable(EditorBrowsableState.Never)]
		public override void CleanUpOwnedReferences()
		{
			_Source = null;
			base.CleanUpOwnedReferences();
		}
		#endregion
		protected override void ReplaceOwnedReference(LanguageElement oldElement, LanguageElement newElement)
		{
			if (_Source == oldElement)
				_Source = (Expression)newElement;
			else
				base.ReplaceOwnedReference(oldElement, newElement);
		}
		#region CloneDataFrom
		protected override void CloneDataFrom(BaseElement source, ElementCloneOptions options)
		{
			if (source == null)
				return;
			base.CloneDataFrom(source, options);
			if (!(source is QualifiedElementReference))
				return;			
			QualifiedElementReference lSource = (QualifiedElementReference)source;			
			if (lSource.Qualifier != null)
			{				
				_Source = ParserUtils.GetCloneFromNodes(this, lSource, lSource.Qualifier) as Expression;
				if (_Source == null)
					_Source = lSource.Qualifier.Clone(options) as Expression;
			}			
		}
		#endregion
		#region Clone
		public override BaseElement Clone(ElementCloneOptions options)
		{
			QualifiedElementReference lClone = new QualifiedElementReference();
			lClone.CloneDataFrom(this, options);
			return lClone;
		}
		#endregion
		public override Expression Qualifier
		{
			get
			{
				return _Source;
			}
			set
			{
		SetSource(value);
			}
		}
		public override bool HasCleanReferences
		{
			get
			{
				if (Qualifier == null)
					return true;
				ElementReferenceExpression lSourceReference = Qualifier as ElementReferenceExpression;
				if (lSourceReference == null)
					return false;				
				return lSourceReference.HasCleanReferences;
			}
		}
	}
}
