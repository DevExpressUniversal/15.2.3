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
using DevExpress.XtraRichEdit.Model;
using DevExpress.Utils;
namespace DevExpress.XtraRichEdit.Import.Doc {
	public class PositionConverter {
		#region Fields
		bool editEnabled;
		int currentPositionIndex;
		List<int> originalPositions;
		Stack<Dictionary<int, DocumentLogPosition>> stackPositionsMapping;
		Dictionary<int, DocumentLogPosition> positionsMapping;
		#endregion
		public PositionConverter() {
			this.originalPositions = new List<int>();
			this.positionsMapping = new Dictionary<int, DocumentLogPosition>();
			this.stackPositionsMapping = new Stack<Dictionary<int, DocumentLogPosition>>();
		}
		public void BeginInit() {
			this.editEnabled = true;
		}
		public void EndInit() {
			this.editEnabled = false;
			this.originalPositions.Sort();
		}
		public void BeginEmbeddedContent() {
			stackPositionsMapping.Push(positionsMapping);
			positionsMapping = new Dictionary<int, DocumentLogPosition>();
		}
		public void EndEmbeddedContent() {
			positionsMapping = stackPositionsMapping.Pop();
		}
		public void AppendPositions(List<int> positions) {
			if (!this.editEnabled)
				return;
			int count = positions.Count;
			for (int i = 0; i < count; i++) {
				if (!this.originalPositions.Contains(positions[i]))
					this.originalPositions.Add(positions[i]);
			}
		}
		public void CalculateCurrentPositionIndex(int characterPosition) {
			int index = this.originalPositions.BinarySearch(characterPosition);
			if (index < 0) {
				index = ~index;
				if (index >= this.originalPositions.Count)
					index = -1;
			}
			this.currentPositionIndex = index;
		}
		public void AdvanceNext(DocumentLogPosition logPosition, int originalPosition, int length) {
			CalculateCurrentPositionIndex(originalPosition);
			if (this.currentPositionIndex < 0)
				return;
			while (this.currentPositionIndex < this.originalPositions.Count && this.originalPositions[this.currentPositionIndex] < originalPosition) {
				int currentPosition = this.originalPositions[this.currentPositionIndex];
				this.positionsMapping.Add(currentPosition, logPosition);
				this.currentPositionIndex++;
			}
			while (this.currentPositionIndex < this.originalPositions.Count && this.originalPositions[this.currentPositionIndex] < originalPosition + length) {
				int currentPosition = this.originalPositions[this.currentPositionIndex];
				this.positionsMapping.Add(currentPosition, logPosition + currentPosition - originalPosition);
				this.currentPositionIndex++;
			}
		}
		public bool TryConvert(int position, out DocumentLogPosition logPosition) {
			return this.positionsMapping.TryGetValue(position, out logPosition);
		}
		public void Clear() {
			this.positionsMapping = new Dictionary<int, DocumentLogPosition>();
		}
	}
}
