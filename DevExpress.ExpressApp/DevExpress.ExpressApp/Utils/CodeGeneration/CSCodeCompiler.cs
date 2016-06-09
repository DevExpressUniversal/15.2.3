#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       eXpressApp Framework                                        }
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
using System.CodeDom.Compiler;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using Microsoft.CSharp;
namespace DevExpress.ExpressApp.Utils.CodeGeneration {
	public sealed class CSCodeCompiler {
		public Assembly Compile(String sourceCode, String[] references, String assemblyFile) {
			if(!String.IsNullOrEmpty(assemblyFile)) {
				String directoryName = Path.GetDirectoryName(assemblyFile);
				if(!Directory.Exists(directoryName)) {
					Directory.CreateDirectory(directoryName);
				}
			}
			CompilerParameters compilerParameters = GetCompilerParameters(references, IsDebug, assemblyFile);
			CodeDomProvider codeProvider = new CSharpCodeProvider();
			CompilerResults compilerResults = codeProvider.CompileAssemblyFromSource(compilerParameters, new String[] { sourceCode });
			if(compilerResults.Errors.Count > 0) {
				RaiseCompilerException(sourceCode, compilerResults);
			}
			return compilerResults.CompiledAssembly;
		}
		private static CompilerParameters GetCompilerParameters(String[] references, Boolean isDebug, String assemblyFile) {
			CompilerParameters compilerParameters = new CompilerParameters();
			compilerParameters.ReferencedAssemblies.AddRange(references);
			if(String.IsNullOrEmpty(assemblyFile)) {
				compilerParameters.GenerateInMemory = !isDebug;
			}
			else {
				compilerParameters.OutputAssembly = assemblyFile;
			}
			if(isDebug) {
				compilerParameters.TempFiles = new TempFileCollection(Environment.GetEnvironmentVariable("TEMP"), true);
				compilerParameters.IncludeDebugInformation = true;
			}
			return compilerParameters;
		}
		private static void RaiseCompilerException(String sourceCode, CompilerResults compilerResults) {
			StringBuilder errorMessage = new StringBuilder();
			foreach(CompilerError compilerError in compilerResults.Errors) {
				errorMessage.AppendFormat("({0}, {1}): {2}", compilerError.Line, compilerError.Column, compilerError.ErrorText).AppendLine();
			}
			throw new CompilerErrorException(compilerResults, sourceCode, errorMessage.ToString());
		}
		public Boolean IsDebug { get; set; }
	}
	[Serializable]
	public class CompilerErrorException : Exception {
		[Serializable]
		private struct CompilerErrorExceptionState : ISafeSerializationData {
			public CompilerResults CompilerResults { get; set; }
			public String SourceCode { get; set; }
			void ISafeSerializationData.CompleteDeserialization(Object obj) {
				CompilerErrorException exception = (CompilerErrorException)obj;
				exception.state = this;
			}
		}
		[NonSerialized]
		private CompilerErrorExceptionState state = new CompilerErrorExceptionState();
		public CompilerErrorException(CompilerResults compilerResults, String source, String errors)
			: base(String.Format("Cannot compile the generated code. Please inspect the generated code via this exception's SourceCode property. The following errors occurred: \r\n{0}", errors)) {
			state.SourceCode = source;
			state.CompilerResults = compilerResults;
			SerializeObjectState += (exception, eventArgs) => eventArgs.AddSerializedState(state);
		}
		public CompilerResults CompilerResults { get { return state.CompilerResults; } }
		public String SourceCode { get { return state.SourceCode; } }
	}
}
