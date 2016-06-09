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
using DevExpress.Persistent.Base;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Runtime.InteropServices;
using System.Text;
using DevExpress.ExpressApp.Model;
namespace DevExpress.ExpressApp.Utils {
	public class SettingsStorageOnModel : SettingsStorage {
		private IModelNode settingsNode;
		private IModelNode ExplorePath(string path, bool createIfAbsent) {
			IModelNode result = settingsNode;
			foreach (string node in path.Split('\\')) {
				if (string.IsNullOrEmpty(node))
					continue;
				if (createIfAbsent) {
					if(result.GetNode(node) == null) {
						result = result.AddNode<IModelNode>(node);
					}
					result = result.GetNode(node);	  
				} else {
					result = result.GetNode(node);
					if (result == null)
						break;
				}
			}
			return result;
		}
		public SettingsStorageOnModel(IModelNode settingsNode)
			: base() {
			if (settingsNode == null)
				throw new ArgumentNullException("settingsNode");
			this.settingsNode = settingsNode;
		}
		public override bool IsPathExist(string optionPath) {
			return ExplorePath(optionPath, false) != null;
		}
		public override void SaveOption(string optionPath, string optionName, string optionValue) {
			ExplorePath(optionPath, true).SetValue<string>(optionName, optionValue);
		}
		public override string LoadOption(string optionPath, string optionName) {
			IModelNode optionNode = ExplorePath(optionPath, false);
			return optionNode != null ? optionNode.GetValue<string>(optionName) : string.Empty;
		}
	}
	public class SettingsStorageOnString : SettingsStorage {
		public static char KeyValueDelimiter = '\t';
		public static char PairDelimiter = '\n';
		private NameValueCollection values = new NameValueCollection();
		public SettingsStorageOnString()
			: base() {
		}
		public override bool IsPathExist(string optionPath) {
			return string.IsNullOrEmpty(values[optionPath]);
		}
		public override void SaveOption(string optionPath, string optionName, string optionValue) {
			values[optionPath + "\\" + optionName] = optionValue;
		}
		public override string LoadOption(string optionPath, string optionName) {
			return values[optionPath + "\\" + optionName];
		}
		public string GetContentAsString() {
			StringBuilder sb = new StringBuilder();
			foreach(string key in values.Keys) {
				sb.Append(key);
				sb.Append(KeyValueDelimiter);
				sb.Append(values[key]);
				sb.Append(PairDelimiter);
			}
			if(sb.Length > 0) {
				sb.Remove(sb.Length - 1, 1);
			}
			return sb.ToString();
		}
		public void SetContentFromString(string str) {
			try {
				values.Clear();
				string[] pairs = str.Split(PairDelimiter);
				foreach(string pair in pairs) {
					if(!string.IsNullOrEmpty(pair)) {
						try {
							string[] keyValue = pair.Split(KeyValueDelimiter);
							values.Add(keyValue[0], keyValue[1]);
						}
						catch(Exception e) {
							Tracing.Tracer.LogSubSeparator("Error occurs on parsing key-value string: '" + pair + "'");
							Tracing.Tracer.LogError(e);
						}
					}
				}
			}
			catch(Exception e) {
				Tracing.Tracer.LogSubSeparator("Exception occurs on parsing string:");
				Tracing.Tracer.LogValue("value", str);
				Tracing.Tracer.LogError(e);
			}
		}
		public NameValueCollection Values {
			get { return values; }
		}
	}
}
