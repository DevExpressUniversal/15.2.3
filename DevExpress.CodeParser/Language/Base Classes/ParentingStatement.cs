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

using System.Collections;
using System.ComponentModel;
#if DXCORE
namespace DevExpress.CodeRush.StructuralParser
#else
namespace DevExpress.CodeParser
#endif
{
	public class ParentingStatement : Statement, IHasParens
	{
		TextRangeWrapper _ParensRange;
	#region protected internal fields...
	bool _IsBreakable;
	#endregion
	#region public fields...
	  bool _HasBlock;
	#endregion
	#region ParentingStatement
	public ParentingStatement(): base()
	{
	}
	#endregion
		#region CloneDataFrom
		protected override void CloneDataFrom(BaseElement source, ElementCloneOptions options)
		{			
			if (source == null)
				return;
			base.CloneDataFrom(source, options);
			if (!(source is ParentingStatement))
				return;
			ParentingStatement lSource = (ParentingStatement)source;
			_IsBreakable = lSource._IsBreakable;
			_HasBlock = lSource._HasBlock;
			if (lSource._ParensRange != null)
				_ParensRange = lSource._ParensRange.Range;
		}
		#endregion
	protected override void UpdateRanges()
	{
	  base.UpdateRanges();
	  _ParensRange = ParensRange;
	}
	#region ToString
	public override string ToString()
	{
	  return ElementType + "()";
	}
	#endregion
		#region GetCyclomaticComplexity
		public override int GetCyclomaticComplexity()
		{
			return GetChildCyclomaticComplexity();
		}
		#endregion
		#region Clone
		public override BaseElement Clone(ElementCloneOptions options)
		{
			ParentingStatement lClone = new ParentingStatement();
			lClone.CloneDataFrom(this, options);
			return lClone;
		}
		#endregion
		public LanguageElementCollection GetUnusedDeclarations() 
		{
	  return StructuralParserServicesHolder.GetUnusedDeclarations(AllVariables);
	  }
		[EditorBrowsable(EditorBrowsableState.Never)]
		public void SetParensRange(Token parenOpen, Token parenClose)
		{
			SetParensRange(new SourceRange(parenOpen.Range.Start, parenClose.Range.End));			
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public void SetParensRange(SourceRange range)
		{
	  ClearHistory();
			if (range.IsEmpty)
				_ParensRange = null;
			else
				_ParensRange = range;
		}
		#region HasBlock
		[Description("True if this language element contains a block of code.")]
		[Category("Family")]
		public bool HasBlock
		{
			get
			{
				return _HasBlock;
			}
			set
			{
				_HasBlock = value;
			}
		}
		#endregion
		#region IsBreakable
		public bool IsBreakable
	{
		get
		{
			return _IsBreakable;
		}
	  set
	  {
		_IsBreakable = value;
	  }
	}
		#endregion
		public override bool IsNewContext
		{
			get 
			{
				return true;
			}
		}
		public IEnumerable AllStatements
		{
			get
			{
				return new ElementEnumerable(this, typeof(Statement), true);
			}
		}
		public IEnumerable AllVariables
		{
			get
			{
				return new ElementEnumerable(this, typeof(Variable), true);
			}
		}
		public IEnumerable AllFlowBreaks
		{
			get
			{
				return new ElementEnumerable(this, typeof(FlowBreak), true);
			}
		}		
		#region IHasParens Members
		public SourceRange ParensRange
		{
			get
			{				
				if (_ParensRange == null)
					return SourceRange.Empty;
				return GetTransformedRange(_ParensRange);
			}
		}
		#endregion
	}
}
