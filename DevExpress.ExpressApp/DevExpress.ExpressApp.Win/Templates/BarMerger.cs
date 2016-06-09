#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       eXpressApp Framework                                        }
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
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using DevExpress.Utils;
using DevExpress.XtraBars;
using DevExpress.XtraBars.Ribbon;
namespace DevExpress.ExpressApp.Win.Templates {
	public class BarMerger {
		private IEnumerable<Tuple<Bar, Bar>> GetBarPairsToProcess(BarManager hostBarManager, BarManager childBarManager) {
			Guard.ArgumentNotNull(hostBarManager, "hostBarManager");
			Guard.ArgumentNotNull(childBarManager, "childBarManager");
			List<Tuple<Bar, Bar>> result = new List<Tuple<Bar, Bar>>();
			foreach(Bar hostBar in hostBarManager.Bars) {
				if(hostBar != hostBarManager.MainMenu && hostBar != hostBarManager.StatusBar) {
					foreach(Bar childBar in childBarManager.Bars) {
						if(childBar.BarName == hostBar.BarName && childBar != childBarManager.MainMenu && childBar != childBarManager.StatusBar) {
							result.Add(new Tuple<Bar, Bar>(hostBar, childBar));
							break;
						}
					}
				}
			}
			if(hostBarManager.StatusBar != null && childBarManager.StatusBar != null) {
				result.Add(new Tuple<Bar, Bar>(hostBarManager.StatusBar, childBarManager.StatusBar));
			}
			return result;
		}
		public void MergeBars(BarManager hostBarManager, BarManager childBarManager) {
			IEnumerable<Tuple<Bar, Bar>> barPairsToProcess = GetBarPairsToProcess(hostBarManager, childBarManager);
			foreach(Tuple<Bar, Bar> barPair in barPairsToProcess) {
				Bar hostBar = barPair.Item1;
				Bar childBar = barPair.Item2;
				hostBar.Merge(childBar);
			}
		}
		public void UnMergeBars(BarManager hostBarManager, BarManager childBarManager) {
			IEnumerable<Tuple<Bar, Bar>> barPairsToProcess = GetBarPairsToProcess(hostBarManager, childBarManager);
			foreach(Tuple<Bar, Bar> barPair in barPairsToProcess) {
				Bar hostBar = barPair.Item1;
				hostBar.UnMerge();
			}
		}
		#region Obsolete 14.2
		[Obsolete("Use statusBar.Merge(childStatusBar) instead."), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public void MergeStatusBar(Bar statusBar, Bar childStatusBar) {
			if(statusBar != null && childStatusBar != null) {
				statusBar.Merge(childStatusBar);
			}
		}
		[Obsolete("Use statusBar.UnMerge() instead."), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		[SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters")]
		public void UnMergeStatusBar(Bar statusBar, Bar childStatusBar) {
			if(statusBar != null) {
				statusBar.UnMerge();
			}
		}
		#endregion
		#region Obsolete 15.2
		[Obsolete("Use statusBar.MergeStatusBar(childStatusBar) instead."), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public void MergeRibbonStatusBar(RibbonStatusBar statusBar, RibbonStatusBar childStatusBar) {
			if(statusBar != null && childStatusBar != null) {
				statusBar.MergeStatusBar(childStatusBar);
			}
		}
		[Obsolete("Use statusBar.UnMergeStatusBar() instead."), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		[SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters")]
		public void UnMergeRibbonStatusBar(RibbonStatusBar statusBar, RibbonStatusBar childStatusBar) {
			if(statusBar != null) {
				statusBar.UnMergeStatusBar();
			}
		}
		#endregion
	}
}
