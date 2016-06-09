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

using System.Windows.Controls;
namespace DevExpress.Xpf.WindowsUI.Navigation {
	public interface INavigationTarget {
		void Navigate(object target, object source, object param);
		bool IsSupported(object target);
	}
	public abstract class NavigationTargetBase<T> : INavigationTarget where T : class {
		protected abstract void Navigate(T target, object source, object param);
		public bool IsSupported(object target) {
			return target is T;
		}
		#region INavigationTarget Members
		void INavigationTarget.Navigate(object target, object source, object param) {
			if(IsSupported(target))
				Navigate(target as T, source, param);
		}
		bool INavigationTarget.IsSupported(object target) {
			return IsSupported(target);
		}
		#endregion
	}
#if !SILVERLIGHT
	public class FrameNavigationTarget : NavigationTargetBase<Frame> {
		protected override void Navigate(Frame target, object source, object param) {
			target.Navigate(source, param);
		}
	}
#endif
	public class NavigationContainerTarget : NavigationTargetBase<INavigationContainer> {
		protected override void Navigate(INavigationContainer target, object source, object param) {
			target.Navigate(source, param);
		}
	}
	public class ContentControNavigationTarget : NavigationTargetBase<ContentControl> {
		protected override void Navigate(ContentControl target, object source, object param) {
			target.Content = source;
		}
	}
}
