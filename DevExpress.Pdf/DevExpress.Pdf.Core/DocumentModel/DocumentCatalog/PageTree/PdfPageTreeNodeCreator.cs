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

using System.Collections.Generic;
namespace DevExpress.Pdf.Native {
	public class PdfPageTreeNodeCreator {
		public static PdfPageTreeNode Create(IEnumerable<PdfPage> pages) {
			return new PdfPageTreeNodeCreator(pages).PageTreeNode;
		}
		readonly IEnumerable<PdfPage> pages;
		readonly PdfRectangle commonMediaBox = new PdfRectangle(0, 0, 1, 1);
		readonly PdfRectangle commonCropBox;
		readonly int commonRotate;
		PdfPageTreeNode PageTreeNode { get { return new PdfPageTreeNode(null, commonMediaBox, commonCropBox, commonRotate, pages); } }
		PdfPageTreeNodeCreator(IEnumerable<PdfPage> pages) {
			this.pages = pages;
			Dictionary<PdfRectangle, int> mediaBoxes = new Dictionary<PdfRectangle, int>();
			Dictionary<PdfRectangle, Dictionary<PdfRectangle, int>> cropBoxes = new Dictionary<PdfRectangle, Dictionary<PdfRectangle, int>>();
			Dictionary<PdfRectangle, Dictionary<int, int>> rotates = new Dictionary<PdfRectangle, Dictionary<int, int>>();
			foreach (PdfPage page in pages) {
				PdfRectangle mediaBox = page.MediaBox;
				PdfRectangle cropBox = page.CropBox;
				int rotate = page.Rotate;
				if (mediaBoxes.ContainsKey(mediaBox)) {
					mediaBoxes[mediaBox]++;
					Dictionary<PdfRectangle, int> cropBoxDictionary = cropBoxes[mediaBox];
					if (cropBoxDictionary.ContainsKey(cropBox))
						cropBoxDictionary[cropBox]++;
					else
						cropBoxDictionary.Add(cropBox, 1);
					Dictionary<int, int> rotateDictionary = rotates[mediaBox];
					if (rotateDictionary.ContainsKey(rotate))
						rotateDictionary[rotate]++;
					else
						rotateDictionary.Add(rotate, 1);
				}
				else {
					mediaBoxes.Add(mediaBox, 1);
					Dictionary<PdfRectangle, int> cropBoxDictionary = new Dictionary<PdfRectangle, int>();
					cropBoxDictionary.Add(cropBox, 1);
					cropBoxes.Add(mediaBox, cropBoxDictionary);
					Dictionary<int, int> rotateDictionary = new Dictionary<int, int>();
					rotateDictionary.Add(rotate, 1);
					rotates.Add(mediaBox, rotateDictionary);
				}
			}
			int maxCount = 0;
			foreach (KeyValuePair<PdfRectangle, int> pair in mediaBoxes) {
				int count = pair.Value;
				if (count > maxCount) {
					maxCount = count;
					commonMediaBox = pair.Key;
				}
			}
			commonCropBox = commonMediaBox;
			if (cropBoxes.Count > 0) {
				maxCount = 0;
				foreach (KeyValuePair<PdfRectangle, int> pair in cropBoxes[commonMediaBox]) {
					int count = pair.Value;
					if (count > maxCount) {
						maxCount = count;
						commonCropBox = pair.Key;
					}
				}
			}
			if (rotates.Count > 0) {
				maxCount = 0;
				foreach (KeyValuePair<int, int> pair in rotates[commonMediaBox]) {
					int count = pair.Value;
					if (count > maxCount) {
						maxCount = count;
						commonRotate = pair.Key;
					}
				}
			}
		}
	}
}
