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
using System.Reflection;
using DevExpress.Utils.Serializing;
using DevExpress.XtraBars.Ribbon.Customization;
namespace DevExpress.XtraBars.Ribbon {
	public class RibbonSaveLoadLayoutHelper {
		public static void SaveLayout(RibbonControl control, string path) {
			using(RunTimeRibbonTreeView tree = CustomizationHelperBase.CreateCustomizationTreeCore(control)) {
				RibbonCustomizationModel model = ResultModelCreator.Instance.Create(tree, control);
				RibbonCustomizationSerializationHelper.Serialize(model, path, control);
			}
		}
		public static void LoadLayout(RibbonControl control, string path) {
			RibbonCustomizationModel model = RibbonCustomizationSerializationHelper.Deserialize(path, control);
			if(model == null || !model.IsModelValid(control))
				return;
			control.ApplyCustomizationSettings(model);
		}
		public static string GetAutoSaveRestoreLayoutXmlFilePath(RibbonControl control) {
			string baseDir = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
			return Path.Combine(baseDir, control.AutoSaveLayoutToXmlPath);
		}
	}
	class RibbonCustomizationSerializationHelper {
		#region Public
		public static void Serialize(RibbonCustomizationModel model, string path, RibbonControl control) {
			try {
				SerializeCore(model, path);
			}
			catch(Exception e) {
				InvalidLayoutExceptionEventArgs args = new InvalidLayoutExceptionEventArgs(e);
				control.RaiseInvalidSaveRestoreLayoutException(args);
				if(!args.Handled) throw;
			}
		}
		public static RibbonCustomizationModel Deserialize(string path, RibbonControl control) {
			RibbonCustomizationModel res = null;
			try {
				res = DeserializeCore(path, control);
			}
			catch(Exception e) {
				InvalidLayoutExceptionEventArgs args = new InvalidLayoutExceptionEventArgs(e);
				control.RaiseInvalidSaveRestoreLayoutException(args);
				if(!args.Handled) throw;
			}
			return res;
		}
		static void SerializeCore(RibbonCustomizationModel model, string path) {
			using(FileStream fs = new FileStream(path, FileMode.Create, FileAccess.Write)) {
				RibbonCustomizationModelSerializer.Instance.Serialize(model, fs);
			}
		}
		static RibbonCustomizationModel DeserializeCore(string path, RibbonControl control) {
			RibbonCustomizationModel res;
			using(FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read)) {
				res = RibbonCustomizationModelSerializer.Instance.Deserialize(fs, control);
			}
			return res;
		}
		#endregion
		#region Helpers
		public static bool IsCustomValueFromItemEventArgs(XtraItemEventArgs e) {
			object res = e.Item.ChildProperties["IsCustom"].Value;
			return bool.Parse(res as string);
		}
		public static IdInfo IdInfoValueFromItemEventArgs(XtraItemEventArgs e) {
			return IdInfoValueFromItemEventArgsCore("IdInfo", e);
		}
		public static IdInfo SourceIdInfoValueFromItemEventArgs(XtraItemEventArgs e) {
			return IdInfoValueFromItemEventArgsCore("SourceIdInfo", e);
		}
		public static IdInfo ItemSourceIdInfoValueFromItemEventArgs(XtraItemEventArgs e) {
			return IdInfoValueFromItemEventArgsCore("ItemSourceIdInfo", e);
		}
		public static string TextValueFromItemEventArgs(XtraItemEventArgs e) {
			object res = e.Item.ChildProperties["Text"].Value;
			return res as string;
		}
		public static bool IsVisibleValueFromItemEventArgs(XtraItemEventArgs e) {
			object res = e.Item.ChildProperties["IsVisible"].Value;
			return bool.Parse(res as string);
		}
		public static IdInfoContainer InfoFromItemEventArgs(XtraItemEventArgs e) {
			IdInfo idInfo = RibbonCustomizationSerializationHelper.IdInfoValueFromItemEventArgs(e);
			IdInfo sourceIdInfo = RibbonCustomizationSerializationHelper.SourceIdInfoValueFromItemEventArgs(e);
			return new IdInfoContainer(idInfo, sourceIdInfo);
		}
		public static T GetEntry<T>(RibbonControl control, IdInfoContainer idInfo, XtraItemEventArgs e) where T : class {
			IdLinkTable linkTable = RibbonSourceStateInfo.Instance.GetLinkTable(control);
			return linkTable.GetEntry(idInfo.SourceIdInfo) as T;
		}
		static IdInfo IdInfoValueFromItemEventArgsCore(string elementName, XtraItemEventArgs e) {
			XmlXtraSerializer.XmlXtraPropertyInfo pi = e.Item.ChildProperties[elementName] as XmlXtraSerializer.XmlXtraPropertyInfo;
			return new IdInfo(int.Parse(pi.ChildProperties["Id"].Value as string));
		}
		#endregion
	}
	class RibbonCustomizationModelSerializer {
		static RibbonCustomizationModelSerializer() {
			Instance = new RibbonCustomizationModelSerializer();
		}
		protected RibbonCustomizationModelSerializer() { }
		public static RibbonCustomizationModelSerializer Instance { get; private set; }
		public void Serialize(RibbonCustomizationModel model, Stream fs) {
			XmlXtraSerializer serializer = new XmlXtraSerializer();
			serializer.SerializeObject(model, fs, GetApplicationName(model));
		}
		public RibbonCustomizationModel Deserialize(Stream fs, RibbonControl control) {
			RibbonCustomizationModel res = new RibbonCustomizationModel(control);
			XmlXtraSerializer serializer = new XmlXtraSerializer();
			serializer.DeserializeObject(res, fs, GetApplicationName(res));
			return res;
		}
		protected virtual string GetApplicationName(RibbonCustomizationModel model) {
			return model.GetType().FullName;
		}
	}
}
