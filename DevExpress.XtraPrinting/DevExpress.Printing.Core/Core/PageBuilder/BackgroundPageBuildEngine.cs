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
using System.Drawing;
using DevExpress.Utils;
namespace DevExpress.XtraPrinting.Native {
	public class BackgroundPageBuildEngine : PageBuildEngine {
		protected BackgroundPageBuildEngineStrategy svc;
		Dictionary<PSPage, Pair<int, int>> pages;
		PageRowBuilder rowBuilder;
		YPageContentEngine pageContentEngine = null;
		public event Action0 AfterBuild;
		public BackgroundPageBuildEngine(PrintingDocument document, BackgroundPageBuildEngineStrategy svc) : base(document.Root, document) {
			this.svc = svc;
		}
		public int GetBufferSize(int count) {
			return svc.GetBufferSize(count);
		}
		public override void Abort() {
			base.Abort();
			StopBackgroundBuild();
		}
		protected override void Build() {
			if(Root.Completed) {
				EnsureProgressReflectorRanges(rootBand);
				PrintingSystem.ProgressReflector.InitializeRange(rootBand.GetBandsCountRecursive());
			}
			pages = new Dictionary<PSPage, Pair<int, int>>();
			StartBackgroundBuild();
		}
		protected void BuildPage() {
			try {
				BuildPageCore();
			} catch(Exception exception) {
				Tracer.TraceError(NativeSR.TraceSource, exception);
				RaiseCreateDocumentException(exception);
			}
		}
		void BuildPageCore() {
			if(Aborted || Stopped)
				return;
			PSPage psPage = CreatePage(ActualPageSizeF);
			if(pageContentEngine != null)
				pageContentEngine.ResetObservableItems();
			pageContentEngine = CreateContentEngine(psPage, pageContentEngine);
			InitializeContentEngine(pageContentEngine);
			if(rowBuilder == null)
				rowBuilder = CreatePageRowBuilder(pageContentEngine);
			rowBuilder.BeforeFillPage(pageContentEngine);
			rowBuilder.FillPage(rootBand, psPage.Rect);
			AddPage(pages, psPage, rowBuilder.GetDetailRowIndexes(rootBand).Clone(), pageContentEngine);
			NextPageData = rowBuilder.NextPageData;
			if(rowBuilder.IsBuildCompleted(rootBand))
				Stop();
		}
		void BuildPageInBackground() {
			svc.BeginInvoke(BuildPage);
		}
		void StartBackgroundBuild() {
			svc.Tick += OnTick;
		}
		void StopBackgroundBuild() {
			svc.Tick -= OnTick;
			if(pageContentEngine != null)
				pageContentEngine.ResetObservableItems();
		}
		void OnTick(object sender, EventArgs e) {
			try {
				if(Stopped) {
					StopBackgroundBuild();
					if(AfterBuild != null)
						AfterBuild();
				} else
					BuildPageInBackground();
			} catch(Exception exception) {
				Tracer.TraceError(NativeSR.TraceSource, exception);
				RaiseCreateDocumentException(exception);
			}
		}
	}
}
