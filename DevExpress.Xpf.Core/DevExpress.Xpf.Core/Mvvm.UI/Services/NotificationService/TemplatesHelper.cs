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

using DevExpress.Internal;
using System;
using System.Windows;
namespace DevExpress.Mvvm.UI.Native {
	public static class NotificationServiceTemplatesHelper {
		static ResourceDictionary resourceDictionary = null;
		static ResourceDictionary GetResourceDictionary() {
			if(resourceDictionary == null) {
				resourceDictionary = new ResourceDictionary();
#if !FREE
				string path = string.Format("pack://application:,,,/{0};component/Mvvm.UI/Services/NotificationService/PredefinedToastNotification.xaml", AssemblyInfo.SRAssemblyXpfCore);
#else
				string path = string.Format("pack://application:,,,/{0};component/Services/NotificationService/PredefinedToastNotification.xaml", AssemblyInfo.SRAssemblyXpfMvvmUIFree);
#endif
				resourceDictionary.Source = new Uri(path, UriKind.Absolute);
			}
			return resourceDictionary;
		}
		static DataTemplate defaultCustomToastTemplate = null;
		public static DataTemplate DefaultCustomToastTemplate {
			get {
				if(defaultCustomToastTemplate == null) {
					ResourceDictionary dict = GetResourceDictionary();
					defaultCustomToastTemplate = (DataTemplate)dict["DefaultCustomToastTemplate"];
				}
				return defaultCustomToastTemplate;
			}
		}
		static DataTemplate predefinedToastTemplate = null;
		public static DataTemplate PredefinedToastTemplate {
			get {
				if(predefinedToastTemplate == null) {
					ResourceDictionary dict = GetResourceDictionary();
					predefinedToastTemplate = (DataTemplate)dict["PredefinedToastTemplate"];
				}
				return predefinedToastTemplate;
			}
		}
	}
}
