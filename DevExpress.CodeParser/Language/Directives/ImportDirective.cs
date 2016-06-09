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
namespace DevExpress.CodeRush.StructuralParser
#else
namespace DevExpress.CodeParser
#endif
{
	public class ImportDirective : CompilerDirective
	{
		string _Expression;
		bool _IsQuotationMarks;
		public ImportDirective ()
		{
			_IsQuotationMarks = true;
		}
		#region Clone
		public override BaseElement Clone(ElementCloneOptions options)
		{
			ImportDirective clone = new ImportDirective();
			clone.CloneDataFrom(this, options);
			return clone;
		}
		#endregion
	protected override void CloneDataFrom(BaseElement source, ElementCloneOptions options)
	{
	  base.CloneDataFrom(source, options);
	  ImportDirective importDirective = source as ImportDirective;
	  if (importDirective == null)
		return;
	  _Expression = importDirective._Expression;
	  _IsQuotationMarks = importDirective._IsQuotationMarks;
	}
		public override LanguageElementType ElementType
		{
			get 
			{
				return LanguageElementType.ImportDirective;
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
		public bool IsQuotationMarks
		{
			get
			{
				return _IsQuotationMarks;
			}
			set
			{
				_IsQuotationMarks = value;
			}
		}
	}
}
