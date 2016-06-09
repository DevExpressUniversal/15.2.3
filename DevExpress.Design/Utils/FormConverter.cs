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
using EnvDTE;
namespace DevExpress.Utils.Design {
	public class FormTypeConverter {
		public static void ToType(IServiceProvider serviceProvider, Type targetType) {
			ToType(serviceProvider, targetType.FullName);
		}
		public static void ToType(IServiceProvider serviceProvider, string fullName) {
			ProjectItem projectItem = serviceProvider.GetService(typeof(ProjectItem)) as ProjectItem;
			ToType(projectItem, fullName);
		}
		public static void ToType(object projectItemObject, string fullName) {
			ProjectItem projectItem = projectItemObject as ProjectItem;
			if(projectItem == null)
				return;
			ToTypeCore(projectItem, fullName);
			foreach(ProjectItem childItem in projectItem.ProjectItems) {
				ToTypeCore(childItem, fullName);
			}
		}
		static void ToTypeCore(ProjectItem projectItem, string fullName) {
			try {
				CodeClass targetClass = GetTargetClass(projectItem);
				if(targetClass == null)
					return;
				if(!projectItem.IsOpen)
					projectItem.Open(EnvDTE.Constants.vsViewKindCode);
				projectItem.Save();
				ProcessCodeElement(targetClass, fullName);
			}
			catch {
			}
		}
		static CodeClass GetTargetClass(ProjectItem projectItem) {
			return ProjectHelper.FindCodeElement(projectItem, element => IsTopFormDefinition(element)) as CodeClass;
		}
		static bool IsTopFormDefinition(EnvDTE.CodeElement element) {
			if(element.Kind != vsCMElement.vsCMElementClass)
				return false;
			return element is CodeClass;
		}
		static void ProcessCodeElement(CodeClass codeClass, string newBaseClassFullName) {
			ConvertStrategyBase strategy = StrategyFactory.Create(codeClass.Language);
			strategy.Process(codeClass, newBaseClassFullName);
		}
		#region Strategy Factory
		public static class StrategyFactory {
			public static ConvertStrategyBase Create(string language) {
				switch(language) {
					case CodeModelLanguageConstants.vsCMLanguageCSharp:
						return new CSharpConvertStrategy();
					case CodeModelLanguageConstants.vsCMLanguageVB:
						return new VBConvertStrategy();
					default: throw new ArgumentException(string.Format("Specified {0} language is not supported", language));
				}
			}
		}
		#endregion
		#region Strategies
		public class StatementPosInfo {
			int startPos;
			int length;
			public StatementPosInfo(int startPos, int length) {
				this.startPos = startPos;
				this.length = length;
			}
			public int StartPos { get { return startPos; } }
			public int Length { get { return length; } }
			public static StatementPosInfo Empty = new StatementPosInfo(-1, 0);
			public override bool Equals(object obj) {
				StatementPosInfo sample = obj as StatementPosInfo;
				if(sample == null) return false;
				return sample.StartPos == StartPos && sample.Length == Length;
			}
			public override int GetHashCode() {
				return base.GetHashCode();
			}
			public bool IsEmpty { get { return Equals(Empty); } }
			protected internal string ReplaceStatement(string code, string newStatement) {
				code = code.Remove(StartPos, Length);
				return code.Insert(StartPos, newStatement);
			}
		}
		public abstract class ConvertStrategyBase {
			char[] separators = new char[] { ',', '{', '\n', ' ', '\t', ':', '\r' };
			[CLSCompliant(false)]
			public void Process(CodeClass codeClass, string newBaseClassFullName) {
				try {
					EditPoint editPoint = codeClass.GetStartPoint(vsCMPart.vsCMPartHeader).CreateEditPoint();
					TextPoint endPoint = codeClass.GetStartPoint(vsCMPart.vsCMPartBody);
					string source = editPoint.GetText(endPoint);
					StatementPosInfo baseClassStatementPos = GetBaseClassStatementPos(source);
					if(baseClassStatementPos.IsEmpty) return;
					string newSource = baseClassStatementPos.ReplaceStatement(source, newBaseClassFullName);
					editPoint.ReplaceText(endPoint, newSource, (int)vsEPReplaceTextOptions.vsEPReplaceTextAutoformat);
				}
				catch { }
			}
			protected internal abstract StatementPosInfo GetBaseClassStatementPos(string text);
			protected internal char[] Separators { get { return separators; } }
		}
		public class CSharpConvertStrategy : ConvertStrategyBase {
			protected internal override StatementPosInfo GetBaseClassStatementPos(string text) {
				int baseClassIndex = text.IndexOf(':');
				if(baseClassIndex != -1) {
					string substring = text.Substring(baseClassIndex + 1);
					string[] wrappedSource = substring.Split(Separators);
					for(int i = 0; i < wrappedSource.Length; i++) {
						baseClassIndex++;
						if(wrappedSource[i].Length == 0) continue;
						return new StatementPosInfo(baseClassIndex, wrappedSource[i].Length);
					}
				}
				return StatementPosInfo.Empty;
			}
		}
		public class VBConvertStrategy : ConvertStrategyBase {
			static string vbInheritsKeyword = "Inherits";
			protected internal override StatementPosInfo GetBaseClassStatementPos(string text) {
				int baseClassIndex = -1;
				bool vbInheritsKeywordFound = false;
				string[] wrappedSource = text.Split(Separators);
				for(int i = 0; i < wrappedSource.Length; i++) {
					baseClassIndex++;
					if(wrappedSource[i].Length == 0) continue;
					if(vbInheritsKeywordFound)
						return new StatementPosInfo(baseClassIndex, wrappedSource[i].Length);
					baseClassIndex += wrappedSource[i].Length;
					if(wrappedSource[i] == vbInheritsKeyword) vbInheritsKeywordFound = true;
				}
				return StatementPosInfo.Empty;
			}
		}
		#endregion
	}
}
