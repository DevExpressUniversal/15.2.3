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
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using DevExpress.Xpf.Core.Native;
namespace DevExpress.Xpf.TreeMap.Native {
	public enum ToolTipNavigationAction {
		MouseUp,
		MouseMove,
		MouseLeave,
	}
	public class ToolTipController {
		readonly TreeMapControl treeMap;
		readonly DispatcherTimer initialTimer = new DispatcherTimer();
		readonly DispatcherTimer popTimer = new DispatcherTimer();
		Point mousePosition;
		TreeMapItemPresentation previousElement;
		TreeMapItemPresentation hitElement;
		bool HitItemIsGroup { get { return HitItem != null && ((TreeMapItem)HitItem).IsGroup; } }
		bool InHitElement { get { return hitElement != null; } }
		bool ElementChanged { get { return !Object.Equals(hitElement, previousElement); } }
		bool CloseOnClick { get { return Options.CloseOnClick; } }
		ToolTipOptions Options { get { return treeMap.ActualToolTipOptions; } }
		ToolTipOpenMode OpenMode { get { return Options.OpenMode; } }
		TimeSpan InitialDelay { get { return treeMap.ToolTipOptions != null ? Options.InitialDelay : TimeSpan.FromMilliseconds(0); } }
		DataTemplate ContentTemplate { get { return HitItemIsGroup ? treeMap.ToolTipGroupContentTemplate : treeMap.ToolTipContentTemplate; } }
		string Pattern { get { return HitItemIsGroup ? treeMap.ToolTipGroupPattern : treeMap.ToolTipPattern; } }
		ToolTipInfo Info { get { return treeMap.ToolTipInfo; } }
		ITreeMapItem HitItem { get { return InHitElement ? hitElement.TreeMapItem : null; } }
		public ToolTipController(TreeMapControl treeMap) {
			this.treeMap = treeMap;
			initialTimer.Tick += new EventHandler(InitialTimerTick);
			popTimer.Tick += new EventHandler(PopTimerTick);
		}
		void InitialTimerTick(object sender, EventArgs e) {
			ShowToolTip();
		}
		void PopTimerTick(object sender, EventArgs e) {
			HideToolTip(OpenMode == ToolTipOpenMode.OnClick);
		}
		string CreateToolTipText() {
			PatternParser parser = new PatternParser(Pattern, Info);
			parser.SetContext(HitItem);
			return parser.GetText();
		}
		void ShowToolTip() {
			initialTimer.Stop();
			if (InHitElement) {
				Point position = Options.ActualPosition.CalclulateToolTipPoint(mousePosition, hitElement, treeMap);
				Info.Layout = ToolTipLayoutHelper.CalculateLayout(position, treeMap);
				Info.ToolTipText = CreateToolTipText();
				Info.ContentTemplate = ContentTemplate;
				Info.Item = HitItem != null ? HitItem.Source : null;
				Info.Visible = true;
				RunTimer(popTimer, Options.AutoPopDelay);
			}
		}
		bool RunTimer(DispatcherTimer timer, TimeSpan interval) {
			if (interval.TotalMilliseconds > 0.0) {
				timer.Interval = interval;
				timer.Start();
				return true;
			}
			return false;
		}
		void CreateToolTip() {
			if (hitElement != null && treeMap.ToolTipEnabled) {
				if (ElementChanged) {
					HideToolTip(true);
					previousElement = hitElement;
					if (!RunTimer(initialTimer, InitialDelay))
						ShowToolTip();
				}
			}
			else
				HideToolTip(true);
		}
		void HideToolTip(bool resetLastElement) {
			if (resetLastElement)
				previousElement = null;
			initialTimer.Stop();
			popTimer.Stop();
			Info.Visible = false;
		}
		void ProcessToolTipOnHover(ToolTipNavigationAction navigationAction) {
			switch (navigationAction) {
				case ToolTipNavigationAction.MouseUp:
					if (CloseOnClick)
						HideToolTip(false);
					break;
				case ToolTipNavigationAction.MouseMove:
					CreateToolTip();
					break;
				case ToolTipNavigationAction.MouseLeave:
					HideToolTip(true);
					break;
			}
		}
		void ProcessToolTipOnClick(ToolTipNavigationAction navigationAction) {
			switch (navigationAction) {
				case ToolTipNavigationAction.MouseUp:
					if (!Info.Visible || ElementChanged)
						CreateToolTip();
					else if (CloseOnClick)
						HideToolTip(true);
					break;
				case ToolTipNavigationAction.MouseMove:
					if (ElementChanged)
						HideToolTip(true);
					break;
				case ToolTipNavigationAction.MouseLeave:
					HideToolTip(true);
					break;
			}
		}
		internal void UpdateToolTip(Point position, ToolTipNavigationAction navigationAction, TreeMapItemPresentation hitElement) {
			mousePosition = position;
			if (treeMap.ToolTipEnabled) {
				this.hitElement = hitElement;
				if (OpenMode == ToolTipOpenMode.OnClick)
					ProcessToolTipOnClick(navigationAction);
				else
					ProcessToolTipOnHover(navigationAction);
			}
			else if (Info.Visible)
				HideToolTip(true);
		}
	}
}
