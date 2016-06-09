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
namespace DevExpress.CodeRush.StructuralParser
#else
namespace DevExpress.CodeParser
#endif 
{
	public abstract class DirectiveCodeGenBase : LanguageElementCodeGenBase 
	{
		#region DirectiveCodeGenBase
		public DirectiveCodeGenBase(CodeGen codeGen) : base(codeGen) 
		{
		}
		#endregion
		#region Generate
		public override void GenerateElement(LanguageElement languageElement) 
		{
			if (languageElement == null)
				return;
			GenerateDirective(languageElement as PreprocessorDirective);
		}
		#endregion
		#region GenerateDirective
	bool GenerateDirective(PreprocessorDirective directive)
	{
	  if (directive == null)
		return false;
	  if (directive.ElementType != LanguageElementType.Region && directive.ElementType != LanguageElementType.EndRegionDirective)
		Code.WriteWithoutIndentStringOneTime = true;
	  switch (directive.ElementType)
	  {
		case LanguageElementType.Region:
		  GenerateRegion(directive as RegionDirective);
		  return true;
		case LanguageElementType.EndRegionDirective:
		  GenerateEndRegion(directive as EndRegionDirective);
		  return true;
		case LanguageElementType.DefineDirective:
		  GenerateDefineDirective(directive as DefineDirective);
		  return true;
		case LanguageElementType.IfDirective:
		  GenerateIfDirective(directive as IfDirective);
		  return true;
		case LanguageElementType.ElifDirective:
		  GenerateElifDirective(directive as ElifDirective);
		  return true;
		case LanguageElementType.ElseDirective:
		  GenerateElseDirective(directive as ElseDirective);
		  return true;
		case LanguageElementType.EndifDirective:
		  GenerateEndIfDirective(directive as EndIfDirective);
		  return true;
		case LanguageElementType.UndefineDirective:
		  GenerateUndefDirective(directive as UndefDirective);
		  return true;
		case LanguageElementType.ErrorDirective:
		  GenerateErrorDirective(directive as ErrorDirective);
		  return true;
		case LanguageElementType.WarningDirective:
		  GenerateWarningDirective(directive as WarningDirective);
		  return true;
		case LanguageElementType.LineDirective:
		  GenerateLineDirective(directive as LineDirective);
		  return true;
		case LanguageElementType.IfDefDirective:
		  GenerateIfDefDirective(directive as IfDefDirective);
		  return true;
		case LanguageElementType.IfnDefDirective:
		  GenerateIfnDefDirective(directive as IfnDefDirective);
		  return true;
		case LanguageElementType.IncludeDirective:
		  GenerateIncludeDirective(directive as IncludeDirective);
		  return true;
		case LanguageElementType.PragmaDirective:
		  GeneratePragmaDirective(directive as PragmaDirective);
		  return true;
	  }
	  if (directive.ElementType != LanguageElementType.Region && directive.ElementType != LanguageElementType.EndRegionDirective)
		Code.WriteWithoutIndentStringOneTime = false;
	  return false;
	}
		#endregion
		protected abstract void GenerateDefineDirective(DefineDirective directive);
		protected abstract void GenerateIfDirective(IfDirective directive);
		protected abstract void GenerateElifDirective(ElifDirective directive);
		protected abstract void GenerateElseDirective(ElseDirective directive);
		protected abstract void GenerateEndIfDirective(EndIfDirective directive);
		protected abstract void GenerateUndefDirective(UndefDirective directive);
		protected abstract void GenerateErrorDirective(ErrorDirective directive);
		protected abstract void GenerateWarningDirective(WarningDirective directive);
		protected abstract void GenerateLineDirective(LineDirective directive);
		protected abstract void GenerateRegion(RegionDirective directive);
		protected abstract void GenerateEndRegion(EndRegionDirective directive);
		protected abstract void GenerateIfDefDirective(IfDefDirective directive);
		protected abstract void GenerateIfnDefDirective(IfnDefDirective directive);
		protected abstract void GenerateIncludeDirective(IncludeDirective directive);
		protected abstract void GenerateImportDirective(ImportDirective directive);
	protected virtual void GeneratePragmaDirective(PragmaDirective directive)
	{
	}
		internal void GenerateTopDirectives(SourceFile sourceFile)
		{
			if (sourceFile == null)
				return;
			CompilerDirective directiveRootNode = sourceFile.CompilerDirectiveRootNode;
			NodeList directives = directiveRootNode.Nodes;
			if (directives == null || directives.Count == 0)
				return;
			NodeList nodes = sourceFile.Nodes;
			SourceRange? fRange = null;
			if (nodes != null && nodes.Count != 0)
			{
				LanguageElement firstElement = nodes[0] as LanguageElement;
				fRange = firstElement.Range;
			}
			GenerateDirectives(directives, fRange);
		}
		void GenerateDirectives(NodeList directives, SourceRange? stopRange)
		{
			if (directives == null || directives.Count == 0)
				return;
			int count = directives.Count;
			for (int i = 0; i < count; i++)
			{
				DefineDirective currentElement = directives[i] as DefineDirective;
				if (currentElement == null)
					continue;
				SourceRange curRange = currentElement.Range;
		FormattingElements fElements = new FormattingElements() { new FormattingElement(FormattingElementType.EOL) };
		if (stopRange == null || currentElement.StartsBefore(stopRange.Value.Start))
		{
		  CodeGen.GenerateElement(currentElement);
		  TokenGen.TokenGenArgs.SetUserFormattingElements(fElements);
		}
			}
		}
	}
}
