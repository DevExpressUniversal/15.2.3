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
using System.Windows.Threading;
using System.Threading;
using System.Collections.Generic;
using System.Windows.Media.Animation;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Utils.Native;
using DevExpress.Xpf.Editors;
using System.Windows.Markup;
using System.Windows.Media;
using DevExpress.Xpf.Grid.Hierarchy;
namespace DevExpress.Xpf.Grid.Native {
	public class ExpandRowWithAnimationAction : IContinousAction {
		enum ExpandState { Expanding, Collapsing }
		const double AnimationEndLagTime = 50;
		GridDataPresenterBase dataPresenter;
		GroupNode groupNode;
		protected FrameworkElement GroupRowElement { get; private set; }
		ExpandState expandState;
		bool isDone;
		bool inProgress;
		Storyboard storyboard;
		bool recursive;
		SizeHelperBase SizeHelper { get { return SizeHelperBase.GetDefineSizeHelper(View.OrientationCore); } }
		GridViewBase View { get { return groupNode.GridView; } }
		GridControl Grid { get { return groupNode.Grid; } }
		public ExpandRowWithAnimationAction(DataPresenterBase dataPresenter, GroupNode groupNode, bool recursive) {
			this.dataPresenter = (GridDataPresenterBase)dataPresenter;
			this.groupNode = groupNode;
			this.recursive = recursive;
			GroupRowElement = View.GetRowElementByRowHandle(groupNode.RowHandle.Value);
		}
		void OnCollapsed() {
			groupNode.IsCollapsing = false;
			ChangeGroupExpanded();
			isDone = true;
			SetVisibleSize(ExpandHelper.DefaultVisibleSize);
			Grid.OnGroupRowCollapsed(groupNode.RowHandle.Value);
			View.UpdateRowData(rowData => rowData.EnsureRowLoaded());
		}
		void OnExpanded() {
			groupNode.CanGenerateItems = true;
			groupNode.IsExpanding = false;
			groupNode.ResumeUpdateState();
			groupNode.IsExpanded = true;
			View.VisualDataTreeBuilder.SynchronizeMasterNode();
			isDone = true;
			SetVisibleSize(ExpandHelper.DefaultVisibleSize);
			View.EnqueueImmediateAction(dataPresenter.ClearInvisibleItems);
			Grid.RaiseGroupRowExpanded(groupNode.RowHandle.Value);
		}
		void ExecuteCore() {
			Point itemsContainerLocation = LayoutHelper.GetRelativeElementRect(GroupRowElement, dataPresenter).BottomRight();
			double defineRenderSize = GetItemsContainerDefineSize();
			double defineVisibleSize = SizeHelper.GetDefineSize(dataPresenter.LastConstraint) - SizeHelper.GetDefinePoint(itemsContainerLocation);
			defineVisibleSize = Math.Max(0, Math.Min(defineVisibleSize, defineRenderSize));
			Size visibleSize = SizeHelper.CreateSize(defineVisibleSize, SizeHelper.GetSecondarySize(ExpandHelper.DefaultVisibleSize));
			SetVisibleSize(visibleSize);
			double speedRatio = defineVisibleSize == 0 ? 10000 : ExpandHelper.GetExpandSpeed(GroupRowElement) / defineVisibleSize;
			if(expandState == ExpandState.Expanding) {
				BeginAnimation(GetStoryboard(ExpandHelper.ExpandStoryboardProperty), speedRatio);
			} else {
				BeginAnimation(GetStoryboard(ExpandHelper.CollapseStoryboardProperty), speedRatio);
			}
			if(postponeForceComplete)
				ForceComplete(false);
		}
		GroupRowData groupRowData { get { return GroupRowElement.DataContext as GroupRowData; } }
		RowsContainer LogicalItemsContainer { get { return groupRowData.RowsContainer; } }
		Size RenderSize { get { return ((IItemsContainer)LogicalItemsContainer).RenderSize; } }
		DependencyObject AnimationTarget { get { return groupRowData.RowsContainer; } }
		protected virtual void SetVisibleSize(Size value) {
			ExpandHelper.SetVisibleSize(LogicalItemsContainer, value);
		}
		protected virtual void SetAnimationProgress(double animationProgress) {
			LogicalItemsContainer.AnimationProgress = animationProgress;
		}
		Storyboard GetStoryboard(DependencyProperty property) {
			object storyboard = View.GetValue(property);
#if SL
			return (Storyboard)XamlReader.Load(((string)storyboard).Replace("<Storyboard", "<Storyboard xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\" "));
#else
			return (Storyboard)storyboard;
#endif
		}
		double GetItemsContainerDefineSize() {
			return SizeHelper.GetDefineSize(RenderSize);
		}
		void BeginAnimation(Storyboard initialStoryboard, double speedRatio) {
			if(initialStoryboard == null)
				return;
#if !SL
			storyboard = initialStoryboard.Clone();
#else
			storyboard = initialStoryboard;
#endif
			Storyboard.SetTarget(storyboard, AnimationTarget);
			storyboard.SpeedRatio = speedRatio;
			storyboard.Completed += new EventHandler(OnStoryboardCompleted);
			storyboard.Begin();
		}
		void OnStoryboardCompleted(object sender, EventArgs e) {
			ForceComplete(true);
		}
		void ForceComplete(bool delayedCollapse) {
			storyboard.Completed -= new EventHandler(OnStoryboardCompleted);
			if(isDone) return;
			if(expandState == ExpandState.Expanding) {
				SetAnimationProgress(1);
				storyboard.Stop();
				OnExpanded();
			} else {
				if(delayedCollapse) {
					DispatcherTimer timer = new DispatcherTimer();
					timer.Tick += new EventHandler(timer_Tick);
					timer.Interval = TimeSpan.FromMilliseconds(AnimationEndLagTime);
					timer.Start();
				} else {
					ForceCompleteCollapseAnimation();
				}
			}
		}
		void timer_Tick(object sender, EventArgs e) {
			((DispatcherTimer)sender).Tick -= new EventHandler(timer_Tick);
			if(!isDone) ForceCompleteCollapseAnimation();
		}
		void ForceCompleteCollapseAnimation() {
			storyboard.Stop();
			OnCollapsed();
		}
		bool postponeForceComplete;
		void IContinousAction.ForceComplete() {
			if(storyboard == null) {
				postponeForceComplete = true;
				return;
			}
			ForceComplete(false);
		}
		bool IContinousAction.IsDone { get { return isDone; } }
		void IContinousAction.Prepare() {
			if(groupNode.IsExpanded) {
				if(!Grid.RaiseGroupRowCollapsing(groupNode.RowHandle.Value)) {
					isDone = true;
					return;
				}
				groupNode.IsCollapsing = true;
				expandState = ExpandState.Collapsing;
				groupNode.CanGenerateItems = false;
				dataPresenter.CollapseBufferSize = GetItemsContainerDefineSize();
			} else {
				if(!Grid.RaiseGroupRowExpanding(groupNode.RowHandle.Value)) {
					isDone = true;
					return;
				}
				groupNode.IsExpanding = true;
				View.VisualDataTreeBuilder.SynchronizeMasterNode();
				expandState = ExpandState.Expanding;
				groupNode.SupressUpdateState();
				ChangeGroupExpanded();
				groupNode.CanGenerateItems = true;
			}
		}
		void ChangeGroupExpanded() {
			Grid.ChangeGroupExpandedCore(groupNode.RowHandle.Value, recursive);
		}
		void IAction.Execute() {
			if(inProgress)
				return;
			inProgress = true;
			dataPresenter.CollapseBufferSize = 0;
			View.Dispatcher.BeginInvoke(
#if !SL
				DispatcherPriority.Render, 
#endif
				new ThreadStart(ExecuteCore));
		}
	}
}
