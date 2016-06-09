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

using System.ComponentModel;
using DevExpress.Utils.Localization;
using DevExpress.Utils.Localization.Internal;
namespace DevExpress.XtraGrid.Localization {
	public enum EnumStringID {
		Locations_Default, Locations_Left, Locations_Top, Locations_Right, Locations_Bottom,
		FieldTextAlignMode_AlignGlobal, FieldTextAlignMode_AlignInGroups, FieldTextAlignMode_AutoSize, FieldTextAlignMode_CustomSize,
		CardsAlignment_Near, CardsAlignment_Center, CardsAlignment_Far,
		ScrollVisibility_Never, ScrollVisibility_Always, ScrollVisibility_Auto,
		ShowFilterPanelMode_Default, ShowFilterPanelMode_ShowAlways, ShowFilterPanelMode_Never,
		LayoutViewMode_SingleRecord, LayoutViewMode_Row, LayoutViewMode_Column, LayoutViewMode_MultiRow, LayoutViewMode_MultiColumn, LayoutViewMode_Carousel,
		LayoutCardArrangeRule_ShowWholeCards, LayoutCardArrangeRule_AllowPartialCards
	}
	[ToolboxItem(false)]
	public class LayoutViewEnumLocalizer : XtraLocalizer<EnumStringID> {
		static LayoutViewEnumLocalizer() {
			SetActiveLocalizerProvider(new DefaultActiveLocalizerProvider<EnumStringID>(CreateDefaultLocalizer()));
		}
		public new static XtraLocalizer<EnumStringID> Active {
			get { return XtraLocalizer<EnumStringID>.Active; }
			set { XtraLocalizer<EnumStringID>.Active = value; }
		}
		public override XtraLocalizer<EnumStringID> CreateResXLocalizer() {
			return new LayoutViewEnumResLocalizer();
		}
		public static XtraLocalizer<EnumStringID> CreateDefaultLocalizer() {
			return new LayoutViewEnumResLocalizer();
		}
		#region AddString
		protected override void PopulateStringTable() {
			AddString(EnumStringID.Locations_Default, "Default");
			AddString(EnumStringID.Locations_Left, "Left");
			AddString(EnumStringID.Locations_Top, "Top");
			AddString(EnumStringID.Locations_Right, "Right");
			AddString(EnumStringID.Locations_Bottom, "Bottom");
			AddString(EnumStringID.FieldTextAlignMode_AlignGlobal, "AlignGlobal");
			AddString(EnumStringID.FieldTextAlignMode_AlignInGroups, "AlignInGroups");
			AddString(EnumStringID.FieldTextAlignMode_AutoSize, "AutoSize");
			AddString(EnumStringID.FieldTextAlignMode_CustomSize, "CustomSize");
			AddString(EnumStringID.CardsAlignment_Near, "Near");
			AddString(EnumStringID.CardsAlignment_Center, "Center");
			AddString(EnumStringID.CardsAlignment_Far, "Far");
			AddString(EnumStringID.ScrollVisibility_Never, "Never");
			AddString(EnumStringID.ScrollVisibility_Always, "Always");
			AddString(EnumStringID.ScrollVisibility_Auto, "Auto");
			AddString(EnumStringID.ShowFilterPanelMode_Default, "Default");
			AddString(EnumStringID.ShowFilterPanelMode_ShowAlways, "ShowAlways");
			AddString(EnumStringID.ShowFilterPanelMode_Never, "Never");
			AddString(EnumStringID.LayoutViewMode_SingleRecord, "SingleRecord");
			AddString(EnumStringID.LayoutViewMode_Row, "Row");
			AddString(EnumStringID.LayoutViewMode_Column, "Column");
			AddString(EnumStringID.LayoutViewMode_MultiRow, "MultiRow");
			AddString(EnumStringID.LayoutViewMode_MultiColumn, "MultiColumn");
			AddString(EnumStringID.LayoutViewMode_Carousel, "Carousel");
			AddString(EnumStringID.LayoutCardArrangeRule_ShowWholeCards, "ShowWholeCards");
			AddString(EnumStringID.LayoutCardArrangeRule_AllowPartialCards, "AllowPartialCards");
		}
		#endregion
	}
	public class LayoutViewEnumResLocalizer : XtraResXLocalizer<EnumStringID> {
		public LayoutViewEnumResLocalizer()
			: base(new LayoutViewEnumLocalizer()) {
		}
		protected override System.Resources.ResourceManager CreateResourceManagerCore() {
			return new System.Resources.ResourceManager("DevExpress.XtraGrid.LayoutView.Designer.LocalizationRes",
				typeof(LayoutViewEnumResLocalizer).Assembly);
		}
	}
}
#if DEBUGTEST
namespace DevExpress.XtraGrid.Localization {
	using System;
	using System.Collections.Generic;
	using System.Reflection;
	using NUnit.Framework;
	using DevExpress.Utils;
	[TestFixture]
	public class LocalizerTest {
		[Test]
		public void CompareLocalizers_EnumStringID() {
			LocalizerHelper.CompareLocalizers<EnumStringID>(new LayoutViewEnumLocalizer(), new LayoutViewEnumResLocalizer());
		}
		[Test]
		public void CompareLocalizers_GridStringId() {
			LocalizerHelper.CompareLocalizers<GridStringId>(new GridLocalizer(), new GridResLocalizer());
		}
	}
	public static class LocalizerHelper {
		public static void CompareLocalizers<T>(XtraLocalizer<T> simpleLocalizer, XtraResXLocalizer<T> resXLocalizer)
			where T : struct {
			List<T> emptyBoth = new List<T>();
			List<T> emptySimple = new List<T>();
			List<T> emptyRes = new List<T>();
			List<T> different = new List<T>();
			string enumTypeName = typeof(T).Name;
			foreach(T stringID in EnumExtensions.GetValues(typeof(T))) {
				string simpleString = simpleLocalizer.GetLocalizedString(stringID);
				string resString = GetLocalizedStringFromResources(resXLocalizer, stringID);
				if(string.IsNullOrEmpty(simpleString) && string.IsNullOrEmpty(resString) && !IsObsolete(stringID)) {
					emptyBoth.Add(stringID);
					continue;
				}
				if(string.IsNullOrEmpty(simpleString) && !string.IsNullOrEmpty(resString)) {
					emptySimple.Add(stringID);
					continue;
				}
				if(string.IsNullOrEmpty(resString) && !string.IsNullOrEmpty(simpleString)) {
					emptyRes.Add(stringID);
					continue;
				}
				if(simpleString != resString) {
					different.Add(stringID);
					continue;
				}
			}
			if(emptyBoth.Count > 0) {
				WriteOutput("Add following stubs to localization.cs: ");
				foreach(T stringID in emptyBoth) {
					WriteOutput(string.Format(@"            AddString({0}.{1}, """");", enumTypeName, stringID.ToString()));
				}
				WriteOutput("");
			}
			if(emptySimple.Count > 0) {
				WriteOutput("Add following strings to simple localizer :");
				foreach(T stringID in emptySimple) {
					WriteOutput(string.Format(@"            AddString({0}.{1}, ""{2}"");", enumTypeName, stringID.ToString(), GetLocalizedStringFromResources(resXLocalizer, stringID)));
				}
				WriteOutput("");
			}
			if(emptyRes.Count > 0) {
				WriteOutput("Add following strings to resources :");
				foreach(T stringID in emptyRes) {
					WriteOutput(string.Format(@"  <data name=""{0}.{1}"" xml:space=""preserve"">", enumTypeName, stringID.ToString()));
					WriteOutput(string.Format(@"    <value>{0}</value>", simpleLocalizer.GetLocalizedString(stringID)));
					WriteOutput("  </data>");
				}
				WriteOutput("");
			}
			if(different.Count > 0) {
				WriteOutput("Following strings are different: ");
				foreach(T stringID in different) {
					WriteOutput(stringID.ToString());
					WriteOutput(simpleLocalizer.GetLocalizedString(stringID));
					WriteOutput(GetLocalizedStringFromResources(resXLocalizer, stringID));
				}
				WriteOutput("");
			}
			if(emptyBoth.Count > 0 || emptySimple.Count > 0 || emptyRes.Count > 0 || different.Count > 0)
				Assert.Fail("There mismatches between localizer. See output fo more details.");
		}
		static void WriteOutput(string str) {
			Console.Out.WriteLine(str);
		}
		static bool IsObsolete<T>(T stringID) {
			object[] attributes = typeof(T).GetField(stringID.ToString()).GetCustomAttributes(typeof(ObsoleteAttribute), false);
			if(attributes.Length == 1 && attributes[0] is ObsoleteAttribute)
				return true;
			return false;
		}
		static string GetLocalizedStringFromResources<T>(XtraResXLocalizer<T> localizer, T id) where T : struct {
			MethodInfo mInfo = typeof(XtraResXLocalizer<T>).GetMethod("GetLocalizedStringFromResources", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
			return (string)mInfo.Invoke(localizer, new object[] { id });
		}
	}
}
#endif
