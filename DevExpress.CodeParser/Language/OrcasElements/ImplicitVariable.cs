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
	public class ImplicitVariable : InitializedVariable, IImplicitVariable
	{
		protected ImplicitVariable() : base()
		{
			HasType = false;
		}
		public ImplicitVariable(string name, Expression initializer) : base (String.Empty, name, initializer)
		{
			HasType = false;		
		}
		public ImplicitVariable(string name) : this(name, null)
		{
		}
		#region Clone
		public override BaseElement Clone(ElementCloneOptions options)
		{
			ImplicitVariable lClone = new ImplicitVariable();
			lClone.CloneDataFrom(this, options);
			return lClone;
		}
		#endregion
		#region ToString
		public override string ToString()
		{
			string returnStrign = String.Empty;
			if (MemberType == null || MemberType == String.Empty)
				returnStrign = Name;
			else
				returnStrign = MemberType + " " + Name;
			if (Expression != null)
			{
				returnStrign = String.Format("{0} = {1}", returnStrign, Expression);
			}			
			return returnStrign;
		}
		#endregion
		public override LanguageElementType ElementType
		{
			get
			{
				return LanguageElementType.ImplicitVariable;
			}
		}
	}
}
