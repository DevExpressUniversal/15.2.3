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
  public class ControlDirective : AspDirective, IMarkupElement
  {
	public ControlDirective()
	  : base()
	{
	  Name = "Control";
	}
	#region Clone
	public override BaseElement Clone(ElementCloneOptions options)
	{
	  ControlDirective lClone = new ControlDirective();
	  lClone.CloneDataFrom(this, options);
	  return lClone;
	}
	#endregion
	#region ElementType
	public override LanguageElementType ElementType
	{
	  get
	  {
		return LanguageElementType.ControlDirective;
	  }
	}
	#endregion
	#region Inherits
	public string Inherits
	{
	  get
	  {
		return GetAttributeValue("Inherits");
	  }
	  set
	  {
		SetAttributeValue("Inherits", value);
	  }
	}
	#endregion
	#region CodeFile
	public string CodeFile
	{
	  get
	  {
		return GetAttributeValue("CodeFile");
	  }
	  set
	  {
		SetAttributeValue("CodeFile", value);
	  }
	}
	#endregion
	#region CodeBehind
	public string CodeBehind
	{
	  get
	  {
		return GetAttributeValue("CodeBehind");
	  }
	  set
	  {
		SetAttributeValue("CodeBehind", value);
	  }
	}
	#endregion
	#region Src
	public string Src
	{
	  get
	  {
		return GetAttributeValue("Src");
	  }
	  set
	  {
		SetAttributeValue("Src", value);
	  }
	}
	#endregion
	#region Language
	public string Language
	{
	  get
	  {
		return GetAttributeValue("Language");
	  }
	  set
	  {
		SetAttributeValue("Language", value);
	  }
	}
	#endregion
	#region AutoEventWireUp
	public string AutoEventWireUp
	{
	  get
	  {
		return GetAttributeValue("AutoEventWireUp");
	  }
	  set
	  {
		SetAttributeValue("AutoEventWireUp", value);
	  }
	}
	#endregion
  }
}
