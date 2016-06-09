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
using System.Linq;
using System.Text;
using System.Drawing;
using System.IO;
using DevExpress.XtraRichEdit.Model;
using DevExpress.Snap.Core.Import;
using DevExpress.Utils;
using DevExpress.Office.Utils;
namespace DevExpress.Snap.Core.Native.Services {
	public interface IThemeService {
		void BeginUpdate();
		void EndUpdate();
		void AddTheme(ThemeInfo theme);
		void RemoveTheme(ThemeInfo theme);
		IEnumerable<ThemeInfo> GetThemes();
		ThemeInfo GetDefaultTheme();
		event EventHandler ThemesChanged;
	}
	public class ThemeService : IThemeService {
		const string defaultThemeCaption = "Casual";
		List<ThemeInfo> themes = new List<ThemeInfo>();
		EventHandler themesChanged;
		int updateCount;
		public event EventHandler ThemesChanged { add { themesChanged += value; } remove { themesChanged -= value; } }
		public virtual void AddTheme(ThemeInfo theme) {
			themes.Add(theme);
			OnThemeChanged();
		}
		public virtual void RemoveTheme(ThemeInfo theme) {
			themes.Remove(theme);
			OnThemeChanged();
		}
		public virtual IEnumerable<ThemeInfo> GetThemes() {
			return themes;
		}
		public virtual ThemeInfo GetDefaultTheme() {
			foreach (var item in themes) {
				if (item.Caption == defaultThemeCaption)
					return item;
			}
			return null;
		}
		protected void OnThemeChanged() {
			if (updateCount == 0 && themesChanged != null)
				themesChanged(this, EventArgs.Empty);
		}
		public void BeginUpdate() {
			updateCount++;
		}
		public void EndUpdate() {
			updateCount--;
			OnThemeChanged();
		}
	}
}
