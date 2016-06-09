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
using System.Drawing;
namespace DevExpress.XtraPrinting.Native {
	public class BuildInfoContainer {
		Dictionary<int, int> buildInfoDictionary = new Dictionary<int, int>();
		Dictionary<int, ProcessState> processStateDictionary = new Dictionary<int, ProcessState>();
		Dictionary<int, float> negativeOffsets;
		public Dictionary<int, float> NegativeOffsets {
			get {
				if(negativeOffsets == null)
					negativeOffsets = new Dictionary<int, float>();
				return negativeOffsets;
			}
		}
		public int GetBuildInfo(DocumentBand band) {
			if(band == null)
				return -1;
			int bi;
			if(!buildInfoDictionary.TryGetValue(band.ID, out bi)) {
				bi = 0;
				buildInfoDictionary[band.ID] = bi;
			}
			return bi;
		}
		public void SetBuildInfo(DocumentBand band, int value) {
			if(band != null)
				buildInfoDictionary[band.ID] = value;
		}
		public DocumentBand GetPrintingDetail(DocumentBand detailContainer) {
			if(detailContainer == null)
				return null;
			int bi = GetBuildInfo(detailContainer);
			DocumentBand item = detailContainer.GetBand(bi);
			while(item != null && !item.IsDetailBand)
				item = detailContainer.GetBand(++bi);
			return item;
		}
		public DocumentBand GetDetailContainer(DocumentBand rootBand, PageRowBuilderBase pageRowBuilderBase, RectangleF bounds) {
			if(rootBand.HasDetailBands)
				return rootBand;
			int i = GetStartBandIndex(rootBand);
			DocumentBand item = rootBand.GetBand(i);
			while(item != null) {
				if(item is DocumentBandContainer) {
					DocumentBand result = GetDetailContainer(item, pageRowBuilderBase, bounds);
					if(result != null) 
						return result;
				}
				item = rootBand.GetBand(++i);
				if(rootBand.HasDetailBands)
					return rootBand;
			}
			return null;
		}
		int GetStartBandIndex(DocumentBand rootBand) {
			int bi = GetBuildInfo(rootBand);
			int index = bi >= rootBand.Bands.Count ? 0 : bi;
			DocumentBand docBand = rootBand.GetBand(index);
			if(docBand != null && docBand.IsKindOf(DocumentBandKind.Footer))
				return index - 1;
			return index;
		}
		public ProcessState GetProcessState(DocumentBand rootBand) {
			ProcessState processState = ProcessState.None;
			processStateDictionary.TryGetValue(rootBand.ID, out processState);
			return processState;
		}
		public void SetProcessState(DocumentBand rootBand, ProcessState processState) {
			processStateDictionary[rootBand.ID] = processState;
		}
	}
}
