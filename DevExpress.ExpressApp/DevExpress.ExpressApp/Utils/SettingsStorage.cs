#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       eXpressApp Framework                                        }
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
using DevExpress.Persistent.Base;
namespace DevExpress.ExpressApp.Utils {
	public abstract class SettingsStorage {
		public virtual bool IsPathExist(string optionPath) { return false; }
		public abstract void SaveOption(string optionPath, string optionName, string optionValue);
		public abstract string LoadOption(string optionPath, string optionName);
		public virtual int LoadIntOption(string optionPath, string optionName) {
			return LoadIntOption(optionPath, optionName, 0);
		}
		public virtual int LoadIntOption(string optionPath, string optionName, int defaultValue) {
			int result;
			return int.TryParse(LoadOption(optionPath, optionName), out result) ? result : defaultValue;
		}
		public virtual bool LoadBoolOption(string optionPath, string optionName, bool defaultValue) {
			bool result;
			return bool.TryParse(LoadOption(optionPath, optionName), out result) ? result : defaultValue;
		}
	}
	public class NullSettingsStorage : SettingsStorage {
		public override void SaveOption(string optionPath, string optionName, string optionValue) { }
		public override string LoadOption(string optionPath, string optionName) { return string.Empty; }
	}
	public class SettingsStorageOnHashtable : SettingsStorage {
		private Dictionary<string, string> settings;
		private string CreateKey(string path, string optionName) {
			return path + "\\" + optionName;
		}
		public override bool IsPathExist(string optionPath) {
			foreach(string key in settings.Keys) {
				if (key.StartsWith(optionPath)) {
					return true;
				}
			}
			return false;
		}
		public override void SaveOption(string optionPath, string optionName, string optionValue) {
			settings[CreateKey(optionPath, optionName)] = optionValue;
		}
		public override string LoadOption(string optionPath, string optionName) {
			string result = string.Empty;
			settings.TryGetValue(CreateKey(optionPath, optionName), out result);
			return result;
		}
		public SettingsStorageOnHashtable() : this(new Dictionary<string, string>()) { }
		public SettingsStorageOnHashtable(Dictionary<string, string> settings) {
			this.settings = settings;
		}
		public Dictionary<string, string> Settings {
			get {
				return settings;
			}
		}
	}
}
