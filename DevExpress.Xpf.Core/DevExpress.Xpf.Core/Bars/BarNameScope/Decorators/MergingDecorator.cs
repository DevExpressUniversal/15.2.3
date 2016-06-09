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

using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Ribbon;
using DevExpress.Xpf.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
namespace DevExpress.Xpf.Bars.Native {	
	public interface IMergingService : IElementBinderService{
		bool CanMerge(IMergingSupport first, IMergingSupport second);
	}
	public interface IMergingSupport : IMultipleElementRegistratorSupport {
		object MergingKey { get; }
		bool IsMerged { get; }
		bool CanMerge(IMergingSupport second);
		void Merge(IMergingSupport second);
		void Unmerge(IMergingSupport second);		
		bool IsMergedParent(IMergingSupport second);
		bool IsAutomaticallyMerged { get; set; }
	}
	public sealed class MergingService : ElementBinderService, IMergingService {
		readonly MergingElementBinder binder;
		public MergingService(MergingElementBinder binder) : base(binder) { this.binder = binder; }
		public bool CanMerge(IMergingSupport first, IMergingSupport second) {
			return binder.CanMergeInternal(first, second);
		}
	}
	public class MergingElementBinder : ElementBinder<IMergingService> {
		public MergingElementBinder()
			: base(typeof(IMergingSupport), typeof(IMergingSupport), ScopeSearchSettings.Ancestors, ScopeSearchSettings.Descendants) {
		}		
		protected override sealed void Link(IBarNameScopeSupport element, IBarNameScopeSupport link) {
			Merge((IMergingSupport)element, (IMergingSupport)link);
		}
		protected override sealed void Unlink(IBarNameScopeSupport element, IBarNameScopeSupport link) {
			Unmerge((IMergingSupport)element, (IMergingSupport)link);
		}
		protected override sealed bool CanLink(IBarNameScopeSupport first, IBarNameScopeSupport second) {
			return CanMerge((IMergingSupport)first, (IMergingSupport)second);
		}
		protected override sealed bool CanUnlink(IBarNameScopeSupport first, IBarNameScopeSupport second) {
			return CanUnmerge((IMergingSupport)first, (IMergingSupport)second);
		}
		protected virtual bool CanMerge(IMergingSupport first, IMergingSupport second) {
			return 
				first!=second
				&& Equals(first.MergingKey, second.MergingKey)
				&& GetAllowMerging(first).Return(x => x.Value, () => true)
				&& Equals(true, GetAllowMerging(second))
				&& first.CanMerge(second)
				&& !second.IsMerged
				&& MergingProperties.CheckRegions(first, second);
		}
		protected internal bool CanMergeInternal(IMergingSupport first, IMergingSupport second) {
			return CanMerge(first, second);
		}	 
		protected virtual bool CanUnmerge(IMergingSupport first, IMergingSupport second) {
			if (second.IsMergedParent(first))
				return second.IsAutomaticallyMerged;
			else if (first.IsMergedParent(second))
				return first.IsAutomaticallyMerged;
			return false;
		}
		protected virtual void Merge(IMergingSupport first, IMergingSupport second) {
			using (MergingPropertiesHelper.GetLocker(first).Lock()) {
				using (MergingPropertiesHelper.GetLocker(second).Lock()) {
			first.Merge(second);
				}
			}			
		}
		protected virtual void Unmerge(IMergingSupport first, IMergingSupport second) {			
			if (second.IsMergedParent(first)) {
				first.Unmerge(second);
				second.IsAutomaticallyMerged = true;
			}				
			else if (first.IsMergedParent(second)) {
				second.Unmerge(first);
				first.IsAutomaticallyMerged = true;
			}				
		}
		bool? GetAllowMerging(IMergingSupport first) {
			return (first as DependencyObject).Return(MergingProperties.GetAllowMergingCore, () => true);
		}
	}
	public static class MergingPropertiesHelper {
		public const string MainMenuID = "F020CBB7-09DC-418B-B4AF-FE115813F810";
		public const string StatusBarID = "A9BEF2DE-618F-4405-BD48-154F4D96BD49";
		public const string RibbonID = "D45AE02E-DB32-474C-81C7-E0091B49E222";
		public const string RibbonStatusBarID = "6D0B2EBE-5D39-43E7-A4E3-E57B1A4E8D09";
		public static void OnPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e, string specialName = null, DependencyProperty nameProperty = null) {
			object feName = specialName ?? (nameProperty.With(d.GetValue) as string).WithString(x => x);
			object oldName = feName;
			object newName = (MergingProperties.GetAllowMergingCore(d) == false) ? null : feName;
			bool any = false;
			if (nameProperty!=null && Equals(nameProperty, e.Property)) {
				if (String.IsNullOrEmpty(MergingProperties.GetName(d))) {
					oldName = (e.OldValue as string).WithString(x => x) ?? specialName;
					newName = (e.NewValue as string).WithString(x => x) ?? specialName;
					any = true;
				}
			}
			if (Equals(MergingProperties.NameProperty, e.Property)) {
				oldName = ((string)e.OldValue).WithString(x => x) ?? feName;
				newName = ((string)e.NewValue).WithString(x => x) ?? feName;
				any = true;
			}			
			if (Equals(MergingProperties.AllowMergingCoreProperty, e.Property)) {
				oldName = ((IMultipleElementRegistratorSupport)d).GetName(typeof(IMergingSupport));
				newName = oldName;
				var bv = (bool?)e.NewValue;
				if (bv.HasValue) {
					if (bv.Value)
						oldName = ((bool?)e.OldValue).HasValue ? null : oldName;
					else
						newName = null;
				}
				any = true;
			}
			if(Equals(MergingProperties.ToolBarMergeStyleProperty, e.Property)
				|| Equals(MergingProperties.BlockMergingRegionIDCoreProperty, e.Property)
				|| Equals(MergingProperties.RegionIDCoreProperty, e.Property)) {
				any = true;
			}				
			if (any)
				BarNameScope.GetService<IElementRegistratorService>(d).NameChanged((IBarNameScopeSupport)d, typeof(IMergingSupport), oldName, newName, true);
		}
		static Dictionary<Locker, object> lockedObjects = new Dictionary<Locker, object>();
		static Dictionary<object, Locker> objectLockers = new Dictionary<object, Locker>();
		internal static Locker GetLocker(object obj) {
			if (objectLockers.ContainsKey(obj)) {
				return objectLockers[obj];
			}
			Locker result = new Locker();
			result.Unlocked += OnLockerUnlocked;
			objectLockers[obj] = result;
			lockedObjects[result] = obj;
			return result;
		}
		internal static void RemoveLocker(Locker locker) {
			var oSender = lockedObjects[locker];
			lockedObjects.Remove(locker);
			objectLockers.Remove(oSender);
			locker.Unlocked -= OnLockerUnlocked;
		}
		public static bool IsAutomaticMergingInProcess(object obj) {
			var locker = GetLocker(obj);
			if(locker.IsLocked)			
				return true;
			RemoveLocker(locker);
			return false;
		}
		static void OnLockerUnlocked(object sender, EventArgs e) {
			Locker lSender = (Locker)sender;
			RemoveLocker(lSender);
		}		
	}
	public static class BarManagerMergingHelper {
		static List<Action> GetMergingActions(DependencyObject obj) {
			return (List<Action>)obj.GetValue(MergingActionsProperty);
		}
		static void SetMergingActions(DependencyObject obj, List<Action> value) {
			obj.SetValue(MergingActionsProperty, value);
		}
		[IgnoreDependencyPropertiesConsistencyChecker]
		static readonly DependencyProperty MergingActionsProperty = DependencyProperty.RegisterAttached("MergingActions", typeof(List<Action>), typeof(BarManagerMergingHelper), new PropertyMetadata(null));
		public static void BeginMerging(BarManager parentManager) {
			if (GetMergingActions(parentManager) != null)
				return;
			SetMergingActions(parentManager, new List<Action>());
		}
		public static void EndMerging(BarManager parentManager) {
			var actions = GetMergingActions(parentManager);
			if (actions == null)
				return;
			foreach (var action in actions)
				action();
			SetMergingActions(parentManager, null);
		}
		public static bool Merge(BarManager parentManager, BarManager childManager, ILinksHolder extraItems) {
			bool res = false;
			bool extraItemsWasMerged = false;
			if (parentManager != null) {
				List<Action> mergeActions = GetMergingActions(parentManager);
				IRibbonControl parentRibbonControl = BarManagerHelper.GetChildRibbonControl(parentManager) as IRibbonControl;
				IRibbonStatusBarControl parentStatusBarControl = BarManagerHelper.GetChildRibbonStatusBar(parentManager) as IRibbonStatusBarControl;
				IRibbonControl childRibbonControl = null;
				IRibbonStatusBarControl childRibbonStatusBarControl = null;
				if (childManager != null) {
					childRibbonControl = BarManagerHelper.GetChildRibbonControl(childManager) as IRibbonControl;
					childRibbonStatusBarControl = BarManagerHelper.GetChildRibbonStatusBar(childManager) as IRibbonStatusBarControl;
				}
				if (parentRibbonControl != null) {
					if (childRibbonControl != null && childRibbonControl.GetMDIMergeStyle() != MDIMergeStyle.Never)
						PerformMergeOperation(new Action(() => parentRibbonControl.Merge(childRibbonControl, extraItems)), mergeActions);
					else
						PerformMergeOperation(new Action(() => parentRibbonControl.Merge(null, extraItems)), mergeActions);
					extraItemsWasMerged = true;
					res = true;
				}
				if (parentStatusBarControl != null && childRibbonStatusBarControl != null && childRibbonStatusBarControl.GetMDIMergeStyle() != MDIMergeStyle.Never) {
					PerformMergeOperation(new Action(() => parentStatusBarControl.Merge(childRibbonStatusBarControl)), mergeActions);
					res = true;
				}
				if (parentManager.MainMenu != null) {
					if (childManager != null && childManager.MainMenu != null && childManager.MDIMergeStyle != MDIMergeStyle.Never) {
						PerformMergeOperation(new Action(() => parentManager.MainMenu.Merge(childManager.MainMenu)), mergeActions);
					}
					if (extraItems != null && !extraItemsWasMerged)
						PerformMergeOperation(new Action(() => ((ILinksHolder)parentManager.MainMenu).Merge(extraItems)), mergeActions);
					res = true;
				}
			}
			return res;
		}
		public static void UnMerge(BarManager parentManager, BarManager childManager, ILinksHolder extraItems) {
			if (parentManager == null)
				return;
			List<Action> mergeActions = GetMergingActions(parentManager);
			if (BarManagerHelper.GetChildRibbonControl(parentManager) != null) {
				var childRibbon = BarManagerHelper.GetChildRibbonControl(childManager);
				var parentRibbon = ((IRibbonControl)BarManagerHelper.GetChildRibbonControl(parentManager));
				PerformMergeOperation(new Action(() => parentRibbon.UnMerge(childRibbon, extraItems)), mergeActions);
			}
			if (BarManagerHelper.GetChildRibbonStatusBar(parentManager) != null && BarManagerHelper.GetChildRibbonStatusBar(childManager) != null) {
				var parentStatusBar = ((IRibbonStatusBarControl)BarManagerHelper.GetChildRibbonStatusBar(parentManager));
				var childStatusBar = BarManagerHelper.GetChildRibbonStatusBar(childManager);
				PerformMergeOperation(new Action(() => parentStatusBar.UnMerge(childStatusBar)), mergeActions);
			}
			if (parentManager.MainMenu != null) {
				var mainMenu = (ILinksHolder)parentManager.MainMenu;
				PerformMergeOperation(new Action(() => (mainMenu).UnMerge(extraItems)), mergeActions);
				if (childManager != null && childManager.MainMenu != null) {
					var childMenu = childManager.MainMenu;
					PerformMergeOperation(new Action(() => mainMenu.UnMerge(childMenu)), mergeActions);
				}
			}
			return;
		}
		public static bool IsMergedParent(BarManager parentManager) {
			if (parentManager == null) return false;
			bool result = false;
			if (BarManagerHelper.GetChildRibbonControl(parentManager) != null) result |= ((IRibbonControl)BarManagerHelper.GetChildRibbonControl(parentManager)).IsMerged;
			if (BarManagerHelper.GetChildRibbonStatusBar(parentManager) != null) result |= ((IRibbonStatusBarControl)BarManagerHelper.GetChildRibbonStatusBar(parentManager)).IsMerged;
			foreach (Bar bar in parentManager.Bars)
				result |= (bar.MergedLinksHolders.Count != 0);
			return result;
		}
		public static bool IsMergedChild(BarManager childManager) {
			if (childManager == null) return false;
			bool result = false;
			if (BarManagerHelper.GetChildRibbonControl(childManager) != null) result |= ((IRibbonControl)BarManagerHelper.GetChildRibbonControl(childManager)).IsChild;
			if (BarManagerHelper.GetChildRibbonStatusBar(childManager) != null) result |= ((IRibbonStatusBarControl)BarManagerHelper.GetChildRibbonStatusBar(childManager)).IsChild;
			foreach (Bar bar in childManager.Bars)
				result |= (((ILinksHolder)bar).MergedParent != null);
			return result;
		}
		static void PerformMergeOperation(Action act, List<Action> mergingActions) {
			if (mergingActions == null)
				act();
			else
				mergingActions.Add(act);
		}
	}
}
