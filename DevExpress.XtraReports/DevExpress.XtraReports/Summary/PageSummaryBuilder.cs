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
using DevExpress.XtraReports.UI;
using DevExpress.XtraPrinting.Export;
using DevExpress.Data.Browsing;
using DevExpress.XtraPrinting;
using DevExpress.XtraPrinting.Native;
using System.Collections;
namespace DevExpress.XtraReports.Native.Summary {
	public class PageSummaryBuilder {
		Dictionary<DataContextBase, object> states = new Dictionary<DataContextBase, object>();
		SelfGeneratedDocumentBand groupHeader = null;
		PageSummaryContainer summaries;
		public PageSummaryBuilder(PageSummaryContainer summaries) {
			if(summaries == null)
				throw new ArgumentNullException();
			this.summaries = summaries;
		}
		public void FinishPageSummary() {
			summaries.OnPageFinished();
		}
		public void BuildPageSummary(DocumentBand docBand) {
			SelfGeneratedDocumentBand selfGeneratedBand = FindSelfGeneratedDocumentBand(docBand);
			if(selfGeneratedBand == null) return;
			if(groupHeader == null && selfGeneratedBand.Band is GroupHeaderBand)
				groupHeader = selfGeneratedBand;
			if(groupHeader != null && groupHeader == selfGeneratedBand) {
				summaries.OnGroupBegin(groupHeader.Band.Report);
			}
			if(selfGeneratedBand.Band.IsDetail) {
				SaveState(selfGeneratedBand.Band.Report.DataContext);
				selfGeneratedBand.SetDataPosition();
				summaries.OnRowChanged(selfGeneratedBand.Band.Report);
			}
			if(groupHeader != null && selfGeneratedBand.Band is GroupHeaderBand) {
				summaries.OnGroupFinished(selfGeneratedBand.Band.Report, (GroupHeaderBand)selfGeneratedBand.Band);
				groupHeader = selfGeneratedBand;
			}
			AddBrickViewDatas(docBand.Bricks);
			RestoreStates();
			states.Clear();
		}
		void SaveState(DataContextBase dataContext) {
			if(!states.ContainsKey(dataContext))
				states[dataContext] = dataContext.SaveState();
		}
		void RestoreStates() {
			foreach(KeyValuePair<DataContextBase, object> item in states)
				item.Key.LoadState(item.Value);
		}
		void AddBrickViewDatas(IEnumerable<Brick> bricks) {
			foreach(Brick brick in bricks) {
				if(!(brick is VisualBrick)) continue;
				NestedVisualBricksEnumerator enumerator = new NestedVisualBricksEnumerator((VisualBrick)brick);
				while(enumerator.MoveNext()) {
					if(enumerator.Current.BrickOwner.HasPageSummary) {
						summaries.AddSummaryBrick(enumerator.Current);
					}
				}
			}
		}
		static SelfGeneratedDocumentBand FindSelfGeneratedDocumentBand(DocumentBand docBand) {
			while(docBand != null && !(docBand is SelfGeneratedDocumentBand)) {
				docBand = docBand.Parent;
			}
			return (SelfGeneratedDocumentBand)docBand;
		}
	}
}
