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
namespace DevExpress.CodeRush.StructuralParser.VB
#else
namespace DevExpress.CodeParser.VB
#endif
{
  public class VBDirectiveCodeGen : DirectiveCodeGenBase
  {
	#region VBDirectiveCodeGen
	public VBDirectiveCodeGen(CodeGen codeGen)
	  : base(codeGen)
	{
	}
	#endregion
	protected override void GenerateDefineDirective(DefineDirective directive)
	{
	}
	protected override void GenerateIfDirective(IfDirective directive)
	{
	}
	protected override void GenerateElifDirective(ElifDirective directive)
	{
	}
	protected override void GenerateElseDirective(ElseDirective directive)
	{
	}
	protected override void GenerateEndIfDirective(EndIfDirective directive)
	{
	}
	protected override void GenerateUndefDirective(UndefDirective directive)
	{
	}
	protected override void GenerateErrorDirective(ErrorDirective directive)
	{
	}
	protected override void GenerateWarningDirective(WarningDirective directive)
	{
	}
	protected override void GenerateLineDirective(LineDirective directive)
	{
	}
	protected override void GenerateRegion(RegionDirective directive)
	{
	}
	protected override void GenerateEndRegion(EndRegionDirective directive)
	{
	}
	protected override void GenerateIfDefDirective(IfDefDirective directive)
	{
	}
	protected override void GenerateIfnDefDirective(IfnDefDirective directive)
	{
	}
	protected override void GenerateIncludeDirective(IncludeDirective directive)
	{
	}
	protected override void GenerateImportDirective(ImportDirective directive)
	{
	}
  }
}
