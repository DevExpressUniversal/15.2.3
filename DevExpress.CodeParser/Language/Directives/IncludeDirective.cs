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
	public class IncludeDirective : CompilerDirective
	{
		string _Expression;
		bool _IsSystemFile;
		string _FilePath = String.Empty;
		public IncludeDirective()
		{
			_IsSystemFile = true;
		}
		#region Clone
		public override BaseElement Clone(ElementCloneOptions options)
		{
			IncludeDirective lClone = new IncludeDirective();
			lClone.CloneDataFrom(this, options);
			return lClone;
		}
		#endregion
	protected override void CloneDataFrom(BaseElement source, ElementCloneOptions options)
	{
	  base.CloneDataFrom(source, options);
	  IncludeDirective includeDirective = source as IncludeDirective;
	  if (includeDirective == null)
		return;
	  _Expression = includeDirective._Expression;
	  _IsSystemFile = includeDirective._IsSystemFile;
	  _FilePath = includeDirective._FilePath;
	}
		public override LanguageElementType ElementType
		{
			get 
			{
				return LanguageElementType.IncludeDirective;
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
		public bool IsSystemFile
		{
			get
			{
				return _IsSystemFile;
			}
			set
			{
				_IsSystemFile = value;
			}
		}
		public string FilePath
		{
			get
			{
				return _FilePath;
			}
			set
			{
				_FilePath = value;
			}
		}
	}
}
