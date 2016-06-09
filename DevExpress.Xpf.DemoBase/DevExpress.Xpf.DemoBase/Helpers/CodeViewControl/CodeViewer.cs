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
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Resources;
using DevExpress.Xpf.DemoBase.Helpers;
using DevExpress.Xpf.DemoBase.Helpers.Internal;
namespace DevExpress.Xpf.DemoBase.Helpers {
	public class CodeViewer : CodeViewPresenter {
		#region Dependency Properties
		public static readonly DependencyProperty CodePathProperty =
			DependencyProperty.Register("CodePath", typeof(string), typeof(CodeViewer), new PropertyMetadata(new PropertyChangedCallback(OnUpdateCode)));
		public static readonly DependencyProperty CurrentItemProperty =
			DependencyProperty.Register("CurrentItem", typeof(object), typeof(CodeViewer), new PropertyMetadata(new PropertyChangedCallback(OnUpdateCode)));
		static void OnUpdateCode(DependencyObject o, DependencyPropertyChangedEventArgs e) {
			((CodeViewer)o).OnUpdateCode();
		}
		#endregion Dependency Properties
		public CodeViewer() {
			FontFamily = new FontFamily("Consolas");
			FontSize = 13;
		}
		public object CurrentItem {
			get { return (object)GetValue(CurrentItemProperty); }
			set { SetValue(CurrentItemProperty, value); }
		}
		public Type CurrentItemType {
			get { return CurrentItem != null ? CurrentItem.GetType() : null; }
		}
		public string CodePath {
			get { return (string)GetValue(CodePathProperty); }
			set { SetValue(CodePathProperty, value); }
		}
		string LoadSourceCode(Type type) {
			string resourcePath =
				string.Format("/{0};component/{1}/{2}{3}",
					type.Assembly.FullName.Split(',')[0],
					CodePath,
					type.Name,
					DemoHelper.GetCodeSuffix(type.Assembly));
			StreamResourceInfo resource = Application.GetResourceStream(new Uri(resourcePath, UriKind.Relative));
			return resource != null ? new StreamReader(resource.Stream).ReadToEnd() : null;
		}
		void OnUpdateCode() {
			if(CurrentItemType != null && !string.IsNullOrEmpty(CodePath))
				CodeText = new CodeLanguageText(DemoHelper.GetDemoLanguage(CurrentItemType.Assembly), LoadSourceCode(CurrentItemType));
			else
				CodeText = null;
		}
	}
}
