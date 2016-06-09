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

using System.Diagnostics;
using System.Collections;
using System.Text;
using DevExpress.XtraRichEdit.Model;
using DevExpress.Utils;
namespace DevExpress.Web.ASPxRichEdit.Export {
	public class Chunk : IHashtableProvider {
		public const int DefaultMaxBufferSize = 4096;
		int maxBufferSize = DefaultMaxBufferSize;
		DocumentLogPosition start = DocumentLogPosition.MinValue;
		readonly StringBuilder textBuffer;
		readonly TextRunHashtableCollection runs;
		public Chunk() {
			this.textBuffer = new StringBuilder();
			this.runs = new TextRunHashtableCollection();
		}
		public StringBuilder TextBuffer {
			[DebuggerStepThrough]
			get { return textBuffer; }
		}
		protected internal int MaxBufferSize { get { return maxBufferSize; } set { maxBufferSize = value; } }
		public DocumentLogPosition Start { get { return start; } protected internal set { start = value; } }
		public int Length { get { return textBuffer.Length; } }
		public TextRunHashtableCollection Runs { get { return runs; } }
		public bool IsLast { get; internal set; }
		public void AppendRun(WebTextRunBase run, string runText) {
#if DEBUGTEST
			Debug.Assert(run.Length == runText.Length);
#endif
			Runs.Add(run);
			TextBuffer.Append(runText);
		}
		public string GetRunText(WebTextRunBase run) {
			return TextBuffer.ToString(run.StartIndex, run.Length);
		}
		public void FillHashtable(Hashtable result) {
			if (runs.Count == 0)
				return;
			result.Add("start", ((IConvertToInt<DocumentLogPosition>)Start).ToInt());
			result.Add("textBuffer", TextBuffer.ToString());
			result.Add("runs", this.runs.ToHashtableCollection());
			if (IsLast) result.Add("isLast", true);
		}
	}
	public class ChunkHashtableCollection : HashtableCollection<Chunk> {
	}
}
