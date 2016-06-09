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

namespace DevExpress.Design.DataAccess {
	using System.Collections.Generic;
	static class CodeParserHelper {
		public static CodeParser.ParserLanguageID GetLanguageID(EnvDTE.CodeClass rootClass) {
			return (rootClass.ProjectItem.FileCodeModel.Language == EnvDTE.CodeModelLanguageConstants.vsCMLanguageCSharp) ?
				CodeParser.ParserLanguageID.CSharp : CodeParser.ParserLanguageID.Basic;
		}
		public static string GetGeneratedCode(CodeParser.ParserLanguageID languageID, string strCode) {
			return GetGeneratedCode(languageID, strCode, false);
		}
		public static string GetGeneratedCode(CodeParser.ParserLanguageID languageID, string strCode, bool isEventSubscription) {
			var parser = CodeParser.ParserFactory.CreateParser(CodeParser.ParserLanguageID.CSharp);
			var nodes = parser.ParseStatements(strCode);
			if(isEventSubscription && languageID == CodeParser.ParserLanguageID.Basic)
				nodes = ConvertNodesToVB(nodes);
			var codeGen = CodeParser.CodeGenFactory.CreateCodeGen(languageID);
			return codeGen.GenerateStatements(nodes);
		}
		static System.Collections.ICollection ConvertNodesToVB(System.Collections.ICollection elements) {
			var result = new List<CodeParser.LanguageElement>();
			if(elements != null) {
				foreach(CodeParser.LanguageElement languageElement in elements) {
					if(CanTransformAssignment(languageElement)) {
						result.AddRange(TransformAssignment(languageElement as CodeParser.Assignment));
						continue;
					}
					if(CanTransformIf(languageElement)) {
						result.AddRange(TransformIf(languageElement as CodeParser.If));
						continue;
					}
					if(CanTransformInitializedVariable(languageElement)) {
						result.AddRange(TransformInitializedVariable(languageElement as CodeParser.InitializedVariable));
						continue;
					}
					result.Add(languageElement);
				}
			}
			return result;
		}
		static bool CanTransformAssignment(CodeParser.LanguageElement node) {
			CodeParser.Assignment assignment = node as CodeParser.Assignment;
			if(assignment != null) {
				return (assignment.AssignmentOperator == CodeParser.AssignmentOperatorType.PlusEquals) ||
					(assignment.AssignmentOperator == CodeParser.AssignmentOperatorType.MinusEquals);
			}
			return false;
		}
		static IEnumerable<CodeParser.LanguageElement> TransformAssignment(CodeParser.Assignment assignment) {
			CodeParser.Expression leftSide = assignment.LeftSide;
			CodeParser.Expression rightSide = assignment.Expression;
			CodeParser.ObjectCreationExpression creation = rightSide as CodeParser.ObjectCreationExpression;
			if(creation != null) {
				if(creation.ArgumentsCount != 1)
					yield break;
				CodeParser.Expression arg = creation.Arguments[0];
				creation.Arguments.Clear();
				creation.AddArgument(new CodeParser.AddressOfExpression(arg));
			}
			else rightSide = new CodeParser.AddressOfExpression(rightSide);
			if(assignment.AssignmentOperator == CodeParser.AssignmentOperatorType.PlusEquals)
				yield return new CodeParser.AddHandler(leftSide, rightSide);
			else
				yield return new CodeParser.RemoveHandler(leftSide, rightSide);
		}
		static bool CanTransformIf(CodeParser.LanguageElement node) {
			CodeParser.If ifNode = node as CodeParser.If;
			if(ifNode != null) {
				CodeParser.RelationalOperation rOp = ifNode.Condition as CodeParser.RelationalOperation;
				if(rOp != null && (rOp.RelationalOperator == CodeParser.RelationalOperator.Equality || rOp.RelationalOperator == CodeParser.RelationalOperator.Inequality)) {
					return (rOp.RightSide.ElementType == CodeParser.LanguageElementType.ElementReferenceExpression) ||
						(rOp.RightSide.ElementType == CodeParser.LanguageElementType.PrimitiveExpression && ((CodeParser.PrimitiveExpression)rOp.RightSide).IsNullLiteral);
				}
			}
			return false;
		}
		static IEnumerable<CodeParser.LanguageElement> TransformIf(CodeParser.If ifNode) {
			CodeParser.RelationalOperation rOp = ifNode.Condition as CodeParser.RelationalOperation;
			if(rOp.RelationalOperator == CodeParser.RelationalOperator.Equality)
				ifNode.Condition = new CodeParser.Is(rOp.LeftSide, rOp.RightSide);
			else
				ifNode.Condition = new CodeParser.IsNot(rOp.LeftSide, rOp.RightSide);
			yield return ifNode;
		}
		static bool CanTransformInitializedVariable(CodeParser.LanguageElement node) {
			CodeParser.InitializedVariable ivNode = node as CodeParser.InitializedVariable;
			if(ivNode != null) {
				var typeCast = ivNode.Expression as CodeParser.ConditionalTypeCast;
				if(typeCast != null) {
					var typeref = typeCast.RightSide as CodeParser.TypeReferenceExpression;
					if(typeref != null && typeref.Name.EndsWith("IDisposable"))
						return true;
				}
			}
			return false;
		}
		static IEnumerable<CodeParser.LanguageElement> TransformInitializedVariable(CodeParser.InitializedVariable ivNode) {
			var typeCast = ivNode.Expression as CodeParser.ConditionalTypeCast;
			typeCast.IsIfOperator = false;
			yield return ivNode;
		}
	}
}
