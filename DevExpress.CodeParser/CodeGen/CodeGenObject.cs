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
using System.IO;
using System.Collections;
using System.Collections.Generic;
#if SL
using DevExpress.Utils;
#endif
#if DXCORE
namespace DevExpress.CodeRush.StructuralParser
#else
namespace DevExpress.CodeParser
#endif
{
  public abstract class CodeGenObject
  {
	CodeWriter _Code;
	Stack<CodeWriter> _CodeStack;
	Hashtable _SkipElements = new Hashtable();
	bool _SaveFormat;
	protected CodeGenObject()
	{
	  _CodeStack = new Stack<CodeWriter>();
	}
	protected void PushCodeWriter()
	{
	  if (_Code == null)
		return;
	  _CodeStack.Push(_Code);
	}
	protected void PopCodeWriter()
	{
	  if (_CodeStack.Count == 0)
		return;
	  _Code = _CodeStack.Pop();
	}
	protected void SetCodeWriter(CodeWriter code)
	{
	  _Code = code;
	  CodeGenObject root = CodeGen;
	  if (root != null)
		root.SetCodeWriter(code);
	}
	public virtual void ClearSkipedElements()
	{
	  _SkipElements.Clear();
	}
	public virtual void AddSkiped(LanguageElement element)
	{
	  if (element == null || IsSkiped(element))
		return;
	  _SkipElements.Add(element, element);
	}
	public virtual bool IsSkiped(LanguageElement element)
	{
	  if (element == null)
		return true;
	  if (SaveFormat && element is CompilerDirective)
		return true;
	  return _SkipElements.ContainsKey(element);
	}
	protected internal  virtual void CalculateIndent(LanguageElement element)
	{
	}
	protected internal virtual void ResetIndent()
	{
	}
	public abstract void GenerateElement(LanguageElement languageElement);
	public virtual void GenerateCode(TextWriter writer, LanguageElement languageElement)
	{
	  GenerateCode(writer, languageElement, 0);
	}
	public virtual void GenerateCode(TextWriter writer, LanguageElement languageElement, int precedingWhiteSpaceCount)
	{
	  if (writer == null)
		throw new ArgumentNullException("writer");
	  if (languageElement == null)
		return;
	  CodeWriter lWriter = new CodeWriter(writer, Options, LineContinuation);
	  lWriter.PrecedingWhiteSpaceCount = precedingWhiteSpaceCount;
	  GenerateCode(lWriter, languageElement);
	}
	public virtual string GenerateCode(LanguageElement languageElement)
	{
	  return GenerateCode(languageElement, 0);
	}
	public virtual string GenerateCode(LanguageElement languageElement, bool calculateIndent)
	{
	  return GenerateCode(languageElement, 0, calculateIndent);
	}
	public virtual string GenerateCode(LanguageElement languageElement, int precedingWhiteSpaceCount)
	{
	  return GenerateCode(languageElement, precedingWhiteSpaceCount, false);
	}
	public virtual string GenerateCode(LanguageElement languageElement, int precedingWhiteSpaceCount, bool calculateIndent)
	{
	  if (languageElement == null)
		return String.Empty;
	  StringBuilder lWorker = new StringBuilder();
	  StringWriter lString = new StringWriter(lWorker);
	  CodeWriter lWriter = new CodeWriter(lString, Options, LineContinuation);
	  lWriter.PrecedingWhiteSpaceCount = precedingWhiteSpaceCount;
	  GenerateCode(lWriter, languageElement, calculateIndent);
	  return lWorker.ToString();
	}
	public virtual void GenerateCode(CodeWriter writer, LanguageElement languageElement)
	{
	  GenerateCode(writer, languageElement, false);
	}
	public virtual void GenerateCode(CodeWriter writer, LanguageElement languageElement, bool calculateIndent)
	{
	  if (writer == null)
		throw new ArgumentNullException("writer");
	  PushCodeWriter();
	  SetCodeWriter(writer);
	  if (calculateIndent)
		CalculateIndent(languageElement);
	  GenerateElement(languageElement);
	  if (calculateIndent)
		ResetIndent();
	  PopCodeWriter();
	}
	protected virtual CodeGen CodeGen
	{
	  get
	  {
		return null;
	  }
	}
	public abstract CodeGenOptions Options { get; }
	protected virtual string LineContinuation
	{
	  get
	  {
		return CodeGen != null ? CodeGen.LineContinuation : string.Empty;
	  }
	}
	public CodeWriter Code
	{
	  get
	  {
		if (_Code == null && CodeGen != null)
		  return CodeGen.Code;
		return _Code;
	  }
	  set
	  {
		if (value == null)
		  return;
		_Code = value;
	  }
	}
	public bool SaveFormat
	{
	  get
	  {
		if (CodeGen == null)
		  return _SaveFormat;
		return CodeGen.SaveFormat;
	  }
	  set
	  {
		if (CodeGen != null)
		  CodeGen.SaveFormat = value;
		else
		  _SaveFormat = value;
	  }
	}
  }
}
