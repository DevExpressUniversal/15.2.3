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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
#if SL
using DevExpress.Utils;
#endif
#if DXCORE
namespace DevExpress.CodeRush.StructuralParser.VB.Preprocessor
#else
namespace DevExpress.CodeParser.VB.Preprocessor
#endif
{
  using VBParser = VB.VB90Parser;
	public partial class VBPreprocessor
	{
	const int minErrDist = 2;
		Stack _IfConditions;
	SourceFile _SourceFile;
	VBParser _VbParser = null;
	VB90Scanner _Scanner;
	Dictionary<string, object> _Defines;
	Dictionary<string, object> _OnlyThisFileDefines;
		protected const bool T = true;
		protected const bool x = false;
		protected int errDist = minErrDist;
		protected Token tToken;	
		protected Token la;   
		protected ParserErrorsBase errors;
		protected bool[,] set;
		protected int maxTokens;
	protected VB90Scanner Scanner
	{
	  get { return _Scanner; }
	  set { _Scanner = value; }
	}
		internal VBPreprocessor(VB90Scanner scanner, Dictionary<string, object> defines)
		{
			_Scanner = scanner;
			CreateDefinesCollections();
			AddProjectDefines(defines);
			InitStandartPrep();
		}
		protected void Get()
		{
			for (;;)
			{
				tToken = la;
				la = _Scanner.Scan();
				while (la.Type == Tokens.LineContinuation)
					la = _Scanner.Scan();
				if (la.Type <= Tokens.MaxTokens)
				{
					++errDist;
					break;
				}
				la = tToken;
			}
		}
		void CreateDefinesCollections()
		{
			_Defines = new Dictionary<string, object>();
			_OnlyThisFileDefines = new Dictionary<string, object>();
		}
		public VBPreprocessor(VB90Scanner scanner, SourceFile rootNode)
		{
			InitPreprocessor(scanner, rootNode);
			InitStandartPrep();
		}
		void InitStandartPrep()
		{
			errors = new PreprocessorErrors();
			set = CreateSetArray();
			maxTokens = Tokens.MaxTokens;
		}
		void AddProjectDefines(SourceFile sourceFile)
		{
			if (sourceFile == null)
				return;
			IProjectElement project = sourceFile.Project;
			if (project == null)
				return;
			AddProjectDefines(project.ValueDefines);
		}
		void AddProjectDefines(Dictionary<string, object> projectDefines)
		{
			if (projectDefines == null || projectDefines.Count == 0)
				return;
			ICollection<string> keys = projectDefines.Keys;
			foreach (string key in keys)
			{
				object val = projectDefines[key];
				_Defines.Add(key, val);
			}
		}
		protected void AddConst(string name, object value)
		{
			if (String.IsNullOrEmpty(name) || value == null)
				return;
			name = name.ToUpper();
			if (_Defines.ContainsKey(name))
				_Defines[name] = value;
			else
			{
				_Defines.Add(name, value);
				_OnlyThisFileDefines.Add(name, null);
			}
		}
		object GetConstValue(string name)
		{
			if (String.IsNullOrEmpty(name))
				return null;
			name = name.ToUpper();
			AddDefineCall(name);
			if (!_Defines.ContainsKey(name))
				return false;
			return _Defines[name];
		}
		public void InitPreprocessor(VB90Scanner scanner, SourceFile rootNode)
		{
			_IfConditions = new Stack();
			_Scanner = scanner;
			_SourceFile = rootNode;
			InitDefines(rootNode);
		}
		void InitDefines(SourceFile sourceFile)
		{
			CreateDefinesCollections();
			AddProjectDefines(sourceFile);
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public static PpResult EvaluateExpression(string str)
		{
			ISourceReader reader = new SourceStringReader(str);
			VB90Scanner scanner = new VB90Scanner(reader);
			VBPreprocessor pr = new VBPreprocessor(scanner, new SourceFile());
			return pr.EvaluateExpression();
		}
		public static object GetExpressionValue(string str, Dictionary<string, object> prevProjectDefines)
		{
			ISourceReader reader = new SourceStringReader(str);
			VB90Scanner scanner = new VB90Scanner(reader);
			VBPreprocessor pr = new VBPreprocessor(scanner, prevProjectDefines);
			return pr.EvaluateExpressionCore();
		}
		object EvaluateExpressionCore()
		{
			Get();
			Object result;
			Expression(out result);
			return result;
		}
		PpResult EvaluateExpression()
		{
			object result = EvaluateExpressionCore();
			PpResult ppResult = Evaluator.GetResult(result);
			return ppResult;
		}
		public VBParser VbParser
		{
			set
			{
				_VbParser = value;
			}
			get
			{
				return _VbParser;
			}
		}
		public SourceFile SourceFile
		{
			get
			{
				return _SourceFile;
			}
			set
			{
				_SourceFile = value;
			}
		}
		protected void AddRegion(RegionDirective regionDirective)
		{
			RegionDirective region = _SourceFile.RegionRootNode;
			if	(region == null)
				region = new RegionDirective();
			_SourceFile.RegionRootNode.AddNode(regionDirective);
		}
		protected void AddDirectiveNode(PreprocessorDirective directive)
		{
			if (_SourceFile != null)
	  {
		if(_SourceFile.CompilerDirectiveRootNode != null)
  				_SourceFile.CompilerDirectiveRootNode.AddNode(directive);
		_SourceFile.AddSimpleDirective(directive, false);
	  }
		}
		#region Expect
		protected void Expect(int n)
		{
			if (la.Type == n)
				Get();
			else
				SynErr(n);
		}
		#endregion
		#region SynErr
		protected void SynErr(int n)
		{
			if (errDist >= minErrDist)
				errors.SynErr(la.Line, la.Column, n);
			errDist = 0;
		}
		#endregion
		public Token Preprocess(Token laToken)
		{
			la = laToken;
			StartRule();
			return la;
		}
		#region GetZeroInt
		protected IntNum GetZeroInt()
		{
			IntNum lResult = new IntNum();
			lResult.Item = 0;
			return lResult;
		}
		#endregion
		#region StartOf
		protected bool StartOf(int s)
		{
			return set[s, la.Type];
		}
		#endregion
		protected Number GetDoubleValue(string val)
		{
			if (String.IsNullOrEmpty(val))
				return null;
			decimal value;
			try
			{
				object num = VBPrimitiveTypeUtils.ToPrimitiveValue(TokenType.FloatingPointLiteral, val);
				if (num is Decimal)
					value = (Decimal)num;
				else
					value = Convert.ToDecimal(val);
			}
			catch
			{
				value = 0;
			}
			FloatNum fNum = new FloatNum();
			fNum.Item = value;
			return fNum;
		}
		protected Number GetIntegerValue(string val)
		{
			if (String.IsNullOrEmpty(val))
				return null;
			int value;
			try
			{
				object num = VBPrimitiveTypeUtils.ToPrimitiveValue(TokenType.IntegerLiteral, val);
				if (num is Int32)
					value = (Int32)num;
				else
					value = Convert.ToInt32(num);
			}
			catch
			{
				value = 0;
			}
			IntNum iNum = new IntNum();
			iNum.Item = value;
			return iNum;
		}
		protected string GetStringValue(string val)
		{
			return GetStringValueCore(val, 0);
		}
		protected string GetStringValueFromChar(string val)
		{
			return GetStringValueCore(val, 1);
		}
		protected string GetStringValueCore(string val, int oddSymbols)
		{
			if (val == null)
				return String.Empty;
			int length = val.Length;
			if (length <= 2 + oddSymbols)
				return String.Empty;
			return val.Substring(1, length - (2 + oddSymbols));
		}
		void AddDefineCall(string name)
		{
			if (String.IsNullOrEmpty(name) || _SourceFile == null || _OnlyThisFileDefines.ContainsKey(name))
				return;
			string fileName = _SourceFile.Name;
			if (String.IsNullOrEmpty(name))
				return;
			_SourceFile.AddInvalidateMacro(name);
		}
		public object Eval(object lExp, object rExp, int operatorType)
		{
			return Evaluator.Eval(lExp, rExp, operatorType);
		}
	}
}
