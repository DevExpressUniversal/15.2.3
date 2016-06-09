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
	public class PdfHuffmanTreeBranch : PdfHuffmanTreeNode {
		PdfHuffmanTreeNode zero;
		PdfHuffmanTreeNode one;
		public PdfHuffmanTreeNode Zero { get { return zero; } }
		public PdfHuffmanTreeNode One { get { return one; } }
		public void Fill(string sequence, int runLength) {
			if (String.IsNullOrEmpty(sequence))
				throw new ArgumentException("sequence");
			bool isOne;
			switch (sequence[0]) {
				case '0':
					isOne = false;
					break;
				case '1':
					isOne = true;
					break;
				default:
					throw new ArgumentException("sequence");
			}
			string newSequence = sequence.Remove(0, 1);
			if (String.IsNullOrEmpty(newSequence))
				if (isOne) {
					if (one != null)
						throw new ArgumentException("sequence");
					one = new PdfHuffmanTreeLeaf(runLength);
				}
				else {
					if (zero != null)
						throw new ArgumentException("sequence");
					zero = new PdfHuffmanTreeLeaf(runLength);
				}
			else {
				PdfHuffmanTreeBranch branch;
				if (isOne)
					if (one == null) {
						branch = new PdfHuffmanTreeBranch();
						one = branch;
					}
					else {
						branch = one as PdfHuffmanTreeBranch;
						if (branch == null)
							throw new ArgumentException("sequence");
					}
				else 
					if (zero == null) {
						branch = new PdfHuffmanTreeBranch();
						zero = branch;
					}
					else {
						branch = zero as PdfHuffmanTreeBranch;
						if (branch == null)
							throw new ArgumentException("sequence");
					}
				branch.Fill(newSequence, runLength);
			}
		}
		public void Fill(IDictionary<string, int> dictionary) {
			foreach (KeyValuePair<string, int> pair in dictionary) 
				Fill(pair.Key, pair.Value);
		}
	}
}
