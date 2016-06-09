﻿#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       UWP Controls                                                }
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

using System.Collections.Generic;
namespace DevExpress.Pdf.Native {
	public class PdfPostScriptInterpreter {
		readonly PdfPostScriptStack stack = new PdfPostScriptStack();
		readonly Stack<PdfPostScriptDictionary> dictionaryStack = new Stack<PdfPostScriptDictionary>();
		readonly Dictionary<string, PdfPostScriptDictionary> fontDictionary = new Dictionary<string, PdfPostScriptDictionary>();
		PdfPostScriptFileParser parser;
		public PdfPostScriptStack Stack { get { return stack; } }
		public Stack<PdfPostScriptDictionary> DictionaryStack { get { return dictionaryStack; } }
		public Dictionary<string, PdfPostScriptDictionary> FontDictionary { get { return fontDictionary; } }
		public PdfPostScriptFileParser Parser { get { return parser; } }
		public PdfPostScriptStack Execute(IEnumerable<object> operators) {
			foreach (object obj in operators)
				Execute(obj);
			return stack;
		}
		public PdfPostScriptStack Execute(byte[] data) {
			parser = new PdfPostScriptFileParser(data);
			for (;;) {
				object obj = parser.ReadNextObject();
				if (obj == null)
					return stack;
				Execute(obj);
			}
		}
		void Execute(object obj) {
			PdfPostScriptOperator op = obj as PdfPostScriptOperator;
			if (op == null)
				stack.Push(obj);
			else
				op.Execute(this);
		}
	}
}
