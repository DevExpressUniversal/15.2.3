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
using System.Windows.Data;
using System.Globalization;
using System.Windows;
using System.Windows.Media;
namespace DevExpress.Xpf.Controls {
	internal static class FindControlHelper {
		public static DependencyObject FindChildControlByType(DependencyObject reference, Type childType) {
			if(reference != null) {
				if(reference.GetType() == childType) {
					return reference;
				}
				int childrenCount = VisualTreeHelper.GetChildrenCount(reference);
				for(int i = 0; i < childrenCount; i++) {
					DependencyObject child = VisualTreeHelper.GetChild(reference, i);
					if(child.GetType() == childType) {
						return child;
					}
					DependencyObject res = FindChildControlByType(child, childType);
					if(res != null) {
						return res;
					}
				}
			}
			return null;
		}
		public static DependencyObject FindParentControlByType(DependencyObject reference, Type parentType) {
			if(reference == null) {				
				return null;
			}
			if(reference.GetType() == parentType) {
				return reference;
			}
			return FindParentControlByType(VisualTreeHelper.GetParent(reference), parentType);
		}
		public static MediaManager GetMediaManager(DependencyObject reference) {
			return (MediaManager)FindParentControlByType(reference, typeof(MediaManager));
		}
	}
}
