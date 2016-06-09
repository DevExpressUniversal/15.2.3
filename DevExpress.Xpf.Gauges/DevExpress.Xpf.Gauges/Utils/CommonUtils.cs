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

using System.Diagnostics;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
namespace DevExpress.Xpf.Gauges.Native {
	public static class CommonUtils {
		public static Panel GetChildPanel(ItemsControl itemsControl) {
			return LayoutHelper.FindElement(itemsControl, element => element is Panel) as Panel;
		}
		public static void SubscribePropertyChangedWeakEvent(INotifyPropertyChanged oldSource, INotifyPropertyChanged newSource, IWeakEventListener listener) {
			if (listener != null) {
				if (oldSource != null)
					PropertyChangedWeakEventManager.RemoveListener(oldSource, listener);
				if (newSource != null)
					PropertyChangedWeakEventManager.AddListener(newSource, listener);
			}
		}
	}
	public static class DebugHelper {
		public static void Fail(string message) {
			Debug.Assert(false, message);
		}
	}
	public static class ObsoleteMessages {
		public const string
			StartSpacesProperty = "The StartSpaces property is now obsolete. Use the InitialMoves property instead.",
			AdditionalSpacesProperty = "The AdditionalSpaces property is now obsolete. Instead, use the RepeatSpaces property for animations with Repeat=True and the FinalMoves property for animations with Repeat=False.";
	}
}
