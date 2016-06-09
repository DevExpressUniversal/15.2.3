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
using System.Collections.Generic;
using System.Text;
using System.IO;
using DevExpress.Utils;
namespace DevExpress.XtraRichEdit.Import.Doc {
	public class DocHeadersFootersPositions {
		#region static
		public static DocHeadersFootersPositions FromStream(BinaryReader reader, int offset, int size) {
			DocHeadersFootersPositions result = new DocHeadersFootersPositions();
			result.Read(reader, offset, size);
			return result;
		}
		#endregion
		#region Fields
		const int positionSize = 4;
		int currentStoryIndex;
		List<int> characterPositions;
		#endregion
		#region Constructors
		public DocHeadersFootersPositions() {
			this.characterPositions = new List<int>();
		}
		#endregion
		#region Properties
		public List<int> CharacterPositions { get { return this.characterPositions; } }
		#endregion
		public void AdvanceNext() {
			this.currentStoryIndex++;
		}
		public bool IsEmpty() {
			return this.characterPositions.Count == 0;
		}
		public bool IsLastHeaderFooter() {
			return this.characterPositions.Count - 2 == this.currentStoryIndex; 
		}
		public int GetNextStoryPosition() {
			return CharacterPositions[this.currentStoryIndex + 1];
		}
		protected void Read(BinaryReader reader, int offset, int size) {
			Guard.ArgumentNotNull(reader, "reader");
			reader.BaseStream.Seek(offset, SeekOrigin.Begin);
			for (int positionIndex = 0; positionIndex < size / positionSize; positionIndex++) {
				this.characterPositions.Add(reader.ReadInt32());
			}
		}
		public void Write(BinaryWriter writer) {
			for (int positionIndex = 0; positionIndex < this.characterPositions.Count; positionIndex++) {
				writer.Write(this.characterPositions[positionIndex]);
			}
		}
	}
	public class DocHeadersFooters {
		const int footnoteCollectionsCount = 6;
		const int itemsPerSection = 6;
		const int evenPageHeaderPosition = 0;
		const int oddPageHeaderPosition = 1;
		const int evenPageFooterPosition = 2;
		const int oddPageFooterPosition = 3;
		const int firstPageHeaderPosition = 4;
		const int firstPageFooterPosition = 5;
		List<DocObjectCollection> headersFooters;
		public DocHeadersFooters() {
			this.headersFooters = new List<DocObjectCollection>();
		}
		public List<DocObjectCollection> HeadersFooters { get { return this.headersFooters; } }
		public DocObjectCollection ActiveCollection { get { return this.headersFooters[this.headersFooters.Count - 1]; } }
		public DocObjectCollection GetEvenPageHeaderObjects(int sectionIndex) {
			int collectionIndex = footnoteCollectionsCount + (itemsPerSection * sectionIndex) + evenPageHeaderPosition;
			return GetCollectionByIndex(collectionIndex);
		}
		public DocObjectCollection GetEvenPageFooterObjects(int sectionIndex) {
			int collectionIndex = footnoteCollectionsCount + (itemsPerSection * sectionIndex) + evenPageFooterPosition;
			return GetCollectionByIndex(collectionIndex);
		}
		public DocObjectCollection GetOddPageHeaderObjects(int sectionIndex) {
			int collectionIndex = footnoteCollectionsCount + (itemsPerSection * sectionIndex) + oddPageHeaderPosition;
			return GetCollectionByIndex(collectionIndex);
		}
		public DocObjectCollection GetOddPageFooterObjects(int sectionIndex) {
			int collectionIndex = footnoteCollectionsCount + (itemsPerSection * sectionIndex) + oddPageFooterPosition;
			return GetCollectionByIndex(collectionIndex);
		}
		public DocObjectCollection GetFirstPageHeaderObjects(int sectionIndex) {
			int collectionIndex = footnoteCollectionsCount + (itemsPerSection * sectionIndex) + firstPageHeaderPosition;
			return GetCollectionByIndex(collectionIndex);
		}
		public DocObjectCollection GetFirstPageFooterObjects(int sectionIndex) {
			int collectionIndex = footnoteCollectionsCount + (itemsPerSection * sectionIndex) + firstPageFooterPosition;
			return GetCollectionByIndex(collectionIndex);
		}
		DocObjectCollection GetCollectionByIndex(int collectionIndex) {
			if (this.headersFooters.Count > collectionIndex)
				return this.headersFooters[collectionIndex];
			return new DocObjectCollection();
		}
	}
}
