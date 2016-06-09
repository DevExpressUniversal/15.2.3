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
#if SL
using DevExpress.Utils;
#endif
#if DXCORE
namespace DevExpress.CodeRush.StructuralParser
#else
namespace DevExpress.CodeParser
#endif
{
	public class CodeGenStyle
	{
		Hashtable _ElementStyles;
		public CodeGenStyle()
		{
			_ElementStyles = new Hashtable();
		}
		public void RegisterElementStyle(ElementCodeGenStyle style)
		{
			if (style == null)
				return;
			ElementCodeGenStyle lStyle = GetElementStyle(style.ElementType);
			if (lStyle == null)
				_ElementStyles.Add(style.ElementType, style);
		}
		public void RegisterElementStyles(ElementCodeGenStyleCollection styles)
		{
			if (styles == null)
				return;
			int lCount = styles.Count;
			for (int i = 0; i < lCount; i++)
				RegisterElementStyle(styles[i]);
		}
		public ElementCodeGenStyle GetElementStyle(LanguageElementType type)
		{
			object lObj = _ElementStyles[type];
			if (lObj == null)
				return null;
			return (ElementCodeGenStyle)lObj;
		}
	}
}
