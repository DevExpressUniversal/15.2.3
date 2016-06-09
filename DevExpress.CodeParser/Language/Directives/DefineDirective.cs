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
	public class DefineDirective : CompilerDirective
	{
		#region fields...
		string _Value;
		string _Expression;
		ParamsStringCollection _Params;
		SourceRange _NameRange = SourceRange.Empty;
		#endregion
		public DefineDirective()
		{
			_Params = null;
		}
		#region CloneDataFrom
		protected override void CloneDataFrom(BaseElement source, ElementCloneOptions options)
		{			
			if (source == null)
				return;
			base.CloneDataFrom(source, options);
			if (!(source is DefineDirective))
				return;
			DefineDirective lSource = (DefineDirective)source;
			_Value = lSource._Value;
	  _Expression = lSource._Expression;
	  _NameRange = NameRange;
		}
		#endregion
	protected override void UpdateRanges()
	{
	  base.UpdateRanges();
	  _NameRange = NameRange;
	}
		#region Clone
		public override BaseElement Clone(ElementCloneOptions options)
		{
			DefineDirective lClone = new DefineDirective();
			lClone.CloneDataFrom(this, options);
			return lClone;
		}
		#endregion
		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("DefineNameRange is obsolete. Use NameRange instead.")]
		public SourceRange DefineNameRange
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
		[EditorBrowsable(EditorBrowsableState.Never)]
		public string Value
		{
			get
			{
				return _Value;
			}
			set
			{
				_Value = value;
			}
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("Symbol is obsolete. Use Value instead.")]
		public string Symbol
		{
			get
			{
				return Value;
			}
			set
			{
				Value = value;
			}
		}
		public string Expression
		{
			get
			{
				return _Expression;
			}
			set
			{
				_Expression = value;
			}
		}
		public override LanguageElementType ElementType
		{
			get
			{
				return LanguageElementType.DefineDirective;
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
				_NameRange = value;
			}
		}
		public ParamsStringCollection Params
		{
			get
			{
				return _Params;
			}
			set
			{
				_Params = value;
			}
		}
	}
}
