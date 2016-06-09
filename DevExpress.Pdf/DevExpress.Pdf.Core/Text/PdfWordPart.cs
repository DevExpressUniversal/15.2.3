#region Copyright (c) 2000-2015 Developer Express Inc.
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

using System;
using System.Collections.Generic;
namespace DevExpress.Pdf.Native {
	public class PdfWordPart {
		readonly PdfOrientedRectangle rectangle;
		readonly IList<PdfCharacter> characters;
		readonly string text;
		readonly int wrapOffset;
		readonly int length;
		int wordNumber;
		bool wordEnded;
		public int WrapOffset { get { return wrapOffset; } }
		public int Length { get { return length; } }
		public PdfOrientedRectangle Rectangle { get { return rectangle; } }
		public bool WordEnded { get { return wordEnded; } }
		public int WordNumber {
			get { return wordNumber; }
			set { wordNumber = value; }
		}
		public string Text { get { return text; }  }
		public IList<PdfCharacter> Characters { get { return characters; } }
		public int NextWrapOffset { get { return wrapOffset + length; } }
		public int EndWordPosition { 
			get { 
				int position = NextWrapOffset;
				return wordEnded ? position : (position - 1);
			} 
		}
		public bool IsNotEmpty { get { return !String.IsNullOrWhiteSpace(text); } }
		internal PdfWordPart(string text, PdfOrientedRectangle rectangle, IList<PdfCharacter> characters, int wrapOffset, bool wordEnded) {
			this.text = text;
			this.wrapOffset = wrapOffset;
			this.length = text.Length;
			this.rectangle = rectangle;
			this.characters = characters;
			this.wrapOffset = wrapOffset;
			this.wordEnded = wordEnded;
		}
		public bool IsSuitable(int wordNumber, int offset) {
			return this.wordNumber == wordNumber && wrapOffset <= offset && offset <= NextWrapOffset;
		}
	}
}
