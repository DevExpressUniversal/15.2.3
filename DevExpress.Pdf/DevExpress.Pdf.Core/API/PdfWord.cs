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
using DevExpress.Pdf.Native;
namespace DevExpress.Pdf {
	public class PdfWord {
		readonly IList<PdfWordPart> parts;
		string text;
		int wordNumber;
		IList<PdfOrientedRectangle> rectangles;
		List<PdfCharacter> characters;
		public string Text { 
			get {
				if (String.IsNullOrEmpty(text)) {
					text = String.Empty;
					int partCount = parts.Count - 1;
					for (int i = 0; i < partCount; i++) {
						string partText = parts[i].Text;
						text += partText.Remove(partText.Length - 1);
					}
					text += parts[partCount].Text;
				}
				return text; 
			} 
		}
		public IList<PdfOrientedRectangle> Rectangles {
			get {
				if (rectangles == null) {
					rectangles = new List<PdfOrientedRectangle>();
					foreach (PdfWordPart part in parts)
						rectangles.Add(part.Rectangle);
				}
				return rectangles; 
			} 
		}
		public IList<PdfCharacter> Characters {
			get {
				if (characters == null) {
					characters = new List<PdfCharacter>();
					foreach (PdfWordPart part in parts)
						characters.AddRange(part.Characters);
				}
				return characters; 
			} 
		}
		internal int WordNumber { 
			get { return wordNumber; }
			set {
				foreach (PdfWordPart part in parts)
					part.WordNumber = value;
				wordNumber = value; 
			}
		}
		internal IList<PdfWordPart> Parts { get { return parts; } }
		internal PdfWord(IList<PdfWordPart> parts) {
			this.parts = parts;
		}
		internal bool Overlaps(PdfWord word) {
			if (word.Text == Text) {
				IList<PdfOrientedRectangle> thisWordRectangles = Rectangles;
				IList<PdfOrientedRectangle> anotherWordRectangles = word.Rectangles;
				int thisCount = thisWordRectangles.Count;
				int anotherCount = anotherWordRectangles.Count;
				if (thisCount == anotherCount) {
					for (int i = 0; i < thisCount; i++)
						if (!thisWordRectangles[i].Overlaps(anotherWordRectangles[i]))
							return false;
					return true;
				}
			}
			return false;
		}
	}
}
