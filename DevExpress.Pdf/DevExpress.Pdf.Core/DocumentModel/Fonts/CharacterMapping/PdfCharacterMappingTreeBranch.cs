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
	public class PdfCharacterMappingTreeBranch : PdfCharacterMappingTreeNode {
		readonly Dictionary<byte, PdfCharacterMappingTreeNode> children = new Dictionary<byte, PdfCharacterMappingTreeNode>();
		public override bool IsFinal { get { return false; } }
		public PdfCharacterMappingTreeBranch() {
		}
		public override void Fill(byte[] code, int position, string value) {
			int codeLength = code.Length - position;
			if (codeLength > 0) {
				byte firstByte = code[position];
				PdfCharacterMappingTreeNode current;
				if (children.TryGetValue(firstByte, out current)) {
					if (!current.IsFinal)
						if (codeLength == 1)
							children[firstByte] = new PdfCharacterMappingTreeLeaf(value);
						else
							current.Fill(code, position + 1, value);
				}
				else if (codeLength == 1)
					children[firstByte] = new PdfCharacterMappingTreeLeaf(value);
				else {
					PdfCharacterMappingTreeBranch child = new PdfCharacterMappingTreeBranch();
					child.Fill(code, position + 1, value);
					children[firstByte] = child;
				}
			}
		}
		public override PdfCharacterMappingFindResult Find(byte[] code, int position) {
			if (code.Length - position > 0) {
				PdfCharacterMappingTreeNode current;
				if (children.TryGetValue(code[position], out current)) {
					PdfCharacterMappingFindResult findResult = current.Find(code, position + 1);
					return new PdfCharacterMappingFindResult(findResult.Value, findResult.CodeLength + 1);
				}
			}
			return new PdfCharacterMappingFindResult(String.Empty, 0);
		}
	}
}
