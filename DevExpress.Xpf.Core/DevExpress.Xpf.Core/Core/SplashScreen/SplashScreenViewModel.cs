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

using DevExpress.Mvvm;
#if !FREE
namespace DevExpress.Xpf.Core {
#else
namespace DevExpress.Mvvm.UI {
#endif
	public class SplashScreenViewModel : ViewModelBase, ISupportSplashScreen {
		public const double ProgressDefaultValue = 0d;
		public const double MaxProgressDefaultValue = 100d;
		public const string StateDefaultValue = "Loading...";
		static SplashScreenViewModel designTimeData = null;
		public static SplashScreenViewModel DesignTimeData {
			get { return designTimeData ?? (designTimeData = new SplashScreenViewModel()); }
		}
		public bool IsIndeterminate {
			get { return GetProperty(() => IsIndeterminate); }
			set { SetProperty(() => IsIndeterminate, value); }
		}
		public double MaxProgress {
			get { return GetProperty(() => MaxProgress); }
			set { SetProperty(() => MaxProgress, value, DisableMarquee); }
		}
		public double Progress {
			get { return GetProperty(() => Progress); }
			set { SetProperty(() => Progress, value, DisableMarquee); }
		}
		public object State {
			get { return GetProperty(() => State); }
			set { SetProperty(() => State, value); }
		}
		public SplashScreenViewModel() {
			allowDisableMarquee = false;
			IsIndeterminate = true;
			MaxProgress = MaxProgressDefaultValue;
			Progress = ProgressDefaultValue;
			State = StateDefaultValue;
			allowDisableMarquee = true;
		}
		void DisableMarquee() {
			if(!allowDisableMarquee) return;
			IsIndeterminate = false;
		}
		bool allowDisableMarquee = false;
	}
}
