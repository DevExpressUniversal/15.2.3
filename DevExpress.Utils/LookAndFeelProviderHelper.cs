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
using DevExpress.LookAndFeel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.Design;
namespace System.ComponentModel.Design {
	public interface IDebugService {
		bool IsDebugging { get; }
	}
	public static class ServiceProviderExtensions {
		public static bool IsDebugging(this IServiceProvider serviceProvider) {
			if(serviceProvider == null) return false;
			IDebugService serv = serviceProvider.GetService(typeof(IDebugService)) as IDebugService;
			return serv != null && serv.IsDebugging;
		}
		public static IWin32Window GetOwnerWindow(this IServiceProvider provider) {
			IUIService serv = provider.GetService(typeof(IUIService)) as IUIService;
			return serv != null ? serv.GetDialogOwnerWindow() : null;
		}
	}
}
namespace DevExpress.LookAndFeel.DesignService {
	public interface ILookAndFeelService {
		void InitializeRootLookAndFeel(UserLookAndFeel lookAndFeel);
		UserLookAndFeel LookAndFeel { get; }
	}
	public class LookAndFeelService : ILookAndFeelService {
		UserLookAndFeel userLookAndFeel;
		public LookAndFeelService() {
			userLookAndFeel = new UserLookAndFeel(new object());
		}
		public UserLookAndFeel LookAndFeel {
			get { return userLookAndFeel; }
		}
		public virtual void InitializeRootLookAndFeel(UserLookAndFeel lookAndFeel) {
			userLookAndFeel.ParentLookAndFeel = lookAndFeel;
		}
	}
	public static class LookAndFeelProviderHelper {
		public static void SetParentLookAndFeel(ISupportLookAndFeel control, IServiceProvider provider) {
			if(control != null) {
				UserLookAndFeel parentLookAndFeel = GetLookAndFeel(provider);
				if(parentLookAndFeel != null)
					((ISupportLookAndFeel)control).LookAndFeel.ParentLookAndFeel = parentLookAndFeel;
			}
		}
		public static UserLookAndFeel GetLookAndFeel(IServiceProvider provider) {
			if(DevExpress.XtraPrinting.Native.PSNativeMethods.AspIsRunning)
				return null;
			if(provider == null)
				return UserLookAndFeel.Default;
			ILookAndFeelService lookAndFeelProvider = provider.GetService(typeof(ILookAndFeelService)) as ILookAndFeelService;
			if(lookAndFeelProvider != null)
				return lookAndFeelProvider.LookAndFeel;
			UserLookAndFeel lookAndFeel = provider.GetService(typeof(UserLookAndFeel)) as UserLookAndFeel;
			return lookAndFeel == null ? UserLookAndFeel.Default : lookAndFeel;
		}
	}
}
