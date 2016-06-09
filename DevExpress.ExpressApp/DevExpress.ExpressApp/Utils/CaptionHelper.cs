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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Text;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Localization;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;
namespace DevExpress.ExpressApp.Utils {
	public enum CompoundNameConvertStyle { None, SplitOnly, SplitAndCapitalization }
	public class CaptionHelperClassCaptionProvider : IClassCaptionProvider {
		public string GetClassCaption(string className) {
			return CaptionHelper.GetClassCaption(className);
		}
		public IClassCaptionProvider Instance {
			get { return this; }
		}
	}
	public class CustomizeConvertCompoundNameEventArgs: HandledEventArgs {
		private string name;
		private CompoundNameConvertStyle style;
		public CustomizeConvertCompoundNameEventArgs(CompoundNameConvertStyle style, string name): base(false) {
			this.style = style;
			this.name = name;
		}
		public CompoundNameConvertStyle Style {
			get { return style;	}
		}
		public string Name {
			get { return name; }
			set { name = value; }
		}
	}
	public class CaptionHelper {
		public const string NullValueTextNodeName = "NullValueText";
		public const string DefaultNullValueText = "N/A";
		public const string NoneValue = "(none)";
		public const string DefaultLanguage = "(Default language)";
		public const string UserLanguage = "(User language)";
		public const string CaptionsLocalizationGroup = "Captions";
		public const string TextsLocalizationGroup = "Texts";
		private static bool removeAcceleratorSymbol = false;
		private static string RemoveAccelerator(string result) {
			int index = result.IndexOf('&');
			while(index > -1) {
				result = result.Remove(index, 1);
				int nextIndex = result.IndexOf('&', index);
				if(nextIndex == index) {
					index = result.IndexOf('&', nextIndex + 1);
				}
				else {
					index = nextIndex;
				}
			}
			return result;
		}
		private static IModelBOModel BoModel {
			get {
				IValueManager<IModelApplication> manager = ValueManager.GetValueManager<IModelApplication>("CaptionHelper_IModelApplication");
				if(manager.Value != null) {
					return manager.Value.BOModel;
				}
				return null;
			}
		}
		public static IModelLocalizationGroup GetModelLocalizationGroup(string groupPath) {
			if(ApplicationModel == null) return null;
			string[] pathItems = groupPath.Replace('\\', '/').Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
			IModelLocalizationGroup groupNode = null;
			foreach(string groupName in pathItems) {
				IModelLocalizationGroup grn = groupNode == null ? ApplicationModel.Localization[groupName] : groupNode[groupName] as IModelLocalizationGroup;
				if(grn == null) {
					grn = groupNode == null ? ApplicationModel.Localization.AddNode<IModelLocalizationGroup>(groupName) : groupNode.AddNode<IModelLocalizationGroup>(groupName);
				}
				groupNode = grn;
			}
			return groupNode;
		}
		private static void SetLocalizedItemValue(IModelLocalizationGroup groupNode, string itemName, string itemValue) {
			if(groupNode != null) {
				IModelLocalizationItem item = (IModelLocalizationItem)groupNode[itemName];
				if(item == null) {
					item = groupNode.AddNode<IModelLocalizationItem>(itemName);
				}
				item.Value = itemValue;
			}
		}
		private static string SplitCompoundName(string name) {
			if(String.IsNullOrEmpty(name)) {
				return "";
			}
			else {
				StringBuilder resultBuilder = new StringBuilder(name.Length + 5);
				bool suppressSpace = true;
				bool previousCharIsDigit = false;
				int charIndex = 0;
				foreach(char c in name) {
					bool isLetter = char.IsLetter(c);
					bool isDigit = char.IsDigit(c);
					if(!(isLetter || isDigit))
						return name;
					bool isLetterUpper = isLetter && char.IsUpper(c);
					bool needSpace = isLetterUpper || isDigit;
					if(needSpace && (!suppressSpace || (previousCharIsDigit && isLetterUpper))) {
						resultBuilder.Append(' ');
					}
					suppressSpace = needSpace || (charIndex == 0 && isLetter && char.IsLower(c));
					resultBuilder.Append(c);
					charIndex++;
					previousCharIsDigit = isDigit;
				}
				return resultBuilder.ToString();
			}
		}
		private static object GetDisplayValue(object theObject) {
			Object result = theObject;
			if(theObject != null) {
				ITypeInfo typeInfo = XafTypesInfo.Instance.FindTypeInfo(theObject.GetType());
				if(typeInfo.DefaultMember != null) {
					result = typeInfo.DefaultMember.GetValue(theObject);
				}
			}
			return result;
		}
		private static IList<IMemberInfo> GetMemberChain(Type type, String memberName) {
			List<IMemberInfo> result = new List<IMemberInfo>();
			String[] memberNameParts = memberName.Split('.');
			Type currentType = type;
			foreach(String memberNamePart in memberNameParts) {
				IMemberInfo memberInfo = XafTypesInfo.Instance.FindTypeInfo(currentType).FindMember(memberNamePart);
				if(memberInfo == null) {
					result = null;
					break;
				}
				else {
					result.Add(memberInfo);
					if(memberInfo.IsList) {
						currentType = memberInfo.ListElementType;
					}
					else {
						currentType = memberInfo.MemberType;
					}
				}
			}
			return result;
		}
		public static IModelLocalizationGroup FindGroupNode(string groupPath) {
			return FindGroupNode(ApplicationModel, groupPath);
		}
		public static IModelLocalizationGroup FindGroupNode(IModelApplication model, string groupPath) {
			IModelApplication applicationNode = model;
			if(applicationNode == null)
				return null;
			IModelLocalization localizationNode = applicationNode.Localization;
			Guard.Assert(localizationNode != null, "localizationNode is null"); 
			IModelLocalizationGroup groupNode = localizationNode[groupPath];
			if(!string.IsNullOrEmpty(groupPath)) {
				string[] pathItems = groupPath.Replace('\\', '/').Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
				if(pathItems.Length > 1) {
					foreach(string groupName in pathItems) {
						groupNode = groupNode == null ? localizationNode[groupName] : (IModelLocalizationGroup)groupNode[groupName];
						if(groupNode == null)
							break;
					}
				}
			}
			return groupNode;
		}
		public static string ConvertCompoundName(string name) {
			return ConvertCompoundName(name, CompoundNameConvertStyle.SplitOnly);
		}
		public static string ConvertCompoundName(string name, CompoundNameConvertStyle style) {
			if(CustomizeConvertCompoundName != null) {
				CustomizeConvertCompoundNameEventArgs eventArgs = new CustomizeConvertCompoundNameEventArgs(style, name);
				CustomizeConvertCompoundName(null, eventArgs);
				if(eventArgs.Handled) { return eventArgs.Name; }
			}
			if(style == CompoundNameConvertStyle.None) {
				return name;
			}
			string result = SplitCompoundName(name);
			if(style == CompoundNameConvertStyle.SplitAndCapitalization) {
				string[] words = result.Split(' ');
				List<string> resultWords = new List<string>(words.Length);
				for(int i = 0; i < words.Length; i++) {
					string word = words[i];
					if(word.Length == 1 && i == 0)
						word = word.ToUpper();
					if(word.Length > 1 && char.IsLower(word[1])) {
						if(i == 0) {
							word = char.ToUpper(word[0]) + word.Substring(1).ToLower();
						}
						else {
							word = word.ToLower();
						}
					}
					resultWords.Add(word);
				}
				result = string.Join(" ", resultWords.ToArray());
			}
			return result;
		}
		public static string GetClassCaption(string classFullName) {
			if(BoModel == null) {
				return classFullName;
			}
			IModelClass classInfo = BoModel[classFullName];
			return classInfo == null ? classFullName : classInfo.Caption;
		}
		public static string GetActionCaption(string actionName) {
			if(ApplicationModel == null || ApplicationModel.ActionDesign == null || ApplicationModel.ActionDesign.Actions == null) {
				return actionName;
			}
			IModelActions modelActions = ApplicationModel.ActionDesign.Actions;
			IModelAction modelAction = modelActions[actionName];
			return modelAction == null ? actionName : modelAction.Caption;
		}
		public static string GetMemberCaption(Type objectType, string memberName) {
			ITypeInfo typeInfo = XafTypesInfo.Instance.FindTypeInfo(objectType);
			IMemberInfo member = typeInfo != null ? typeInfo.FindMember(memberName) : null;
			if(member != null) {
				return GetMemberCaption(member);
			}
			return StringHelper.GetFirstPart('.', memberName);
		}
		public static string GetMemberCaption(ITypeInfo typeInfo, string memberName) {
			Guard.ArgumentNotNull(typeInfo, "typeInfo");
			IMemberInfo member = typeInfo.FindMember(memberName);
			if(member != null) {
				return GetMemberCaption(member);
			}
			return StringHelper.GetFirstPart('.', memberName);
		}
		public static string GetMemberCaption(IMemberInfo memberInfo) {
			Guard.ArgumentNotNull(memberInfo, "memberInfo");
#if DebugTest
			if(TestCustomGetMemberCaption != null) {
				TestCustomGetMemberCaptionEventArgs args = new TestCustomGetMemberCaptionEventArgs(memberInfo.Owner, memberInfo.Name);
				TestCustomGetMemberCaption(null, args);
				if(args.Handled) {
					return args.Result;
				}
			}
#endif
			IModelBOModel boModel = BoModel;
			if(boModel != null) {
				foreach(IMemberInfo currentMember in memberInfo.GetPath()) {
					IModelClass modelClass = boModel.GetClass(currentMember.Owner.Type);
					IModelMember propertyNode = modelClass != null ? modelClass.FindOwnMember(currentMember.Name) : null;
					if(propertyNode != null && !string.IsNullOrEmpty(propertyNode.Caption)) {
						return propertyNode.Caption;
					}
				}
			}
			return StringHelper.GetFirstPart('.', memberInfo.Name);
		}
		public static String GetLastMemberPartCaption(Type objectType, String memberName) {
			ITypeInfo typeInfo = XafTypesInfo.Instance.FindTypeInfo(objectType);
			if(typeInfo == null) {
				return "";
			}
			String result = StringHelper.GetLastPart('.', memberName);
			IMemberInfo memberInfo = typeInfo.FindMember(memberName);
			IList<IMemberInfo> memberInfoChain = null;
			if(memberInfo != null) {
				memberInfoChain = memberInfo.GetPath();
			}
			else {
				memberInfoChain = GetMemberChain(typeInfo.Type, memberName);
			}
			if(memberInfoChain != null) {
				result = GetMemberCaption(memberInfoChain[memberInfoChain.Count - 1].Owner, memberInfoChain[memberInfoChain.Count - 1].Name);
			}
			return result;
		}
		public static String GetFullMemberCaption(ITypeInfo typeInfo, String memberName) {
			if(typeInfo == null) {
				return "";
			}
			String result = "";
			IList<IMemberInfo> memberInfoChain = GetMemberChain(typeInfo.Type, memberName);
			IModelBOModel boModel = BoModel;
			if(memberInfoChain != null && boModel != null) {
				foreach(IMemberInfo memberInfo in memberInfoChain) {
					IModelClass modelClass = boModel.GetClass(memberInfo.Owner.Type);
					IModelMember modelMember = modelClass != null ? modelClass.FindOwnMember(memberInfo.Name) : null;
					if(modelMember != null && !string.IsNullOrEmpty(modelMember.Caption)) {
						result += modelMember.Caption + ".";
					}
					else {
						result += memberInfo.Name + ".";
					}
				}
				result = result.TrimEnd('.');
			}
			if(result == "") {
				result = memberName;
			}
			return result;
		}
		public static string GetBoolText(bool value) {
			return value ? "True" : "False";
		}
		public static string GetDisplayText(Object theObject) {
			if(theObject != null) {
				Object result = GetDisplayValue(theObject);
				if(result != null) {
					if(result.GetType().IsEnum) {
						EnumDescriptor enumDescriptor = new EnumDescriptor(result.GetType());
						result = enumDescriptor.GetCaption(result);
					}
					if(BoModel != null) {
						IModelClass modelClass = BoModel[theObject.GetType().FullName];
						string format = modelClass != null ? modelClass.ObjectCaptionFormat : string.Empty;
						if(!String.IsNullOrEmpty(format)) {
							result = String.Format(format, result);
						}
					}
					return result.ToString();
				}
				return "";
			}
			return NullValueText;
		}
		public static string GetLocalizedText(string groupPath, string itemName, string defaultText) {
			string result = defaultText ?? string.Empty;
			IModelLocalizationGroup groupNode = FindGroupNode(groupPath);
			if(groupNode != null) {
				IModelLocalizationItem itemNode = groupNode[itemName] as IModelLocalizationItem;
				if(itemNode != null) {
					if(itemNode.Value != null){
						result = itemNode.Value;
						if(RemoveAcceleratorSymbol) {
							result = RemoveAccelerator(result);
						}
					}
				}
				IModelLocalizationGroup childGroupNode = groupNode[itemName] as IModelLocalizationGroup;
				if(childGroupNode != null) {
					if(childGroupNode.Value != null) {
						result = childGroupNode.Value;
						if(RemoveAcceleratorSymbol) {
							result = RemoveAccelerator(result);
						}
					}
				}
			}
			return result.Replace("\\n", "\n").Replace("\\r", "\r");
		}
		public static string GetLocalizedText(string groupPath, string itemName) {
			return GetLocalizedText(groupPath, itemName, "");
		}		
		public static string GetLocalizedText(string groupPath, string itemName, params object[] args) {
			string text = GetLocalizedText(groupPath, itemName);
			if(!string.IsNullOrEmpty(text)) {
				return string.Format(text, args);
			}
			else {
				return itemName;
			}
		}
		public static Dictionary<string, string> GetLocalizedItems(string groupPath) {
			IModelLocalizationGroup groupNode = GetModelLocalizationGroup(groupPath);
			if(groupNode == null) {
				return new Dictionary<string, string>();
			}
			Dictionary<string, string> result = new Dictionary<string, string>(groupNode.Count);
			foreach(IModelLocalizationItem child in groupNode) {
				result.Add(child.Name, child.Value);
			}
			return result;
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public static void SetLocalizedText(string groupPath, string itemName, string itemValue) {
				SetLocalizedItemValue(GetModelLocalizationGroup(groupPath), itemName, itemValue);
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public static void SetLocalizedText(string groupPath, IDictionary<string, string> itemsAndValues) {
				IModelLocalizationGroup groupNode = GetModelLocalizationGroup(groupPath);
				foreach(KeyValuePair<string, string> pair in itemsAndValues) {
					SetLocalizedItemValue(groupNode, pair.Key, pair.Value);
				}
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public static void SetLocalizedText(string groupPath, IList<string> itemNames, IList<string> itemValues) {
				if(itemNames.Count != itemValues.Count)
					throw new ArgumentException(SystemExceptionLocalizer.GetExceptionMessage(ExceptionId.NameAndValueCollectionsShouldHaveEqualLengths));
				IModelLocalizationGroup groupNode = GetModelLocalizationGroup(groupPath);
				for(int i = 0; i < itemNames.Count; i++) {
					SetLocalizedItemValue(groupNode, itemNames[i], itemValues[i]);
				}
			}
		public static void SetLocalizedText(IModelLocalizationGroup node, IList<string> itemNames, IList<string> itemValues) {
			if(itemNames.Count != itemValues.Count)
				throw new ArgumentException(SystemExceptionLocalizer.GetExceptionMessage(ExceptionId.NameAndValueCollectionsShouldHaveEqualLengths));
			for(int i = 0; i < itemNames.Count; i++) {
				SetLocalizedItemValue(node, itemNames[i], itemValues[i]);
			}
		}
		public static void Setup(IModelApplication applicationModel) {
			ValueManager.GetValueManager<IModelApplication>("CaptionHelper_IModelApplication").Value = applicationModel;
		}
		public static void RemoveModelApplicationIfNeed(IModelApplication applicationModel) {
			IValueManager<IModelApplication> manager = ValueManager.GetValueManager<IModelApplication>("CaptionHelper_IModelApplication");
			if(manager.Value == applicationModel) {
				manager.Value = null;
			}
		}
		public static IModelApplication ApplicationModel {
			get {
				return ValueManager.GetValueManager<IModelApplication>("CaptionHelper_IModelApplication").Value;
			}
		}
		public static bool RemoveAcceleratorSymbol {
			get { return removeAcceleratorSymbol; }
			set { removeAcceleratorSymbol = value; }
		}
		public static string NullValueText {
			get { return GetLocalizedText("Texts", NullValueTextNodeName, DefaultNullValueText); }
			set { SetLocalizedText("Texts", NullValueTextNodeName, value); }
		}
		public static event EventHandler<CustomizeConvertCompoundNameEventArgs> CustomizeConvertCompoundName;
		public static string GetAspectByCultureInfo(System.Globalization.CultureInfo culture) {
			return culture.ToString() == "" ? DefaultLanguage : culture.ToString();
		}
		public static string GetParentAspect(string aspect) {
			string result = null;
			if(aspect != DefaultLanguage && !string.IsNullOrEmpty(aspect)) {
				CultureInfo cultureInfo = new CultureInfo(aspect, false);
				return cultureInfo.Parent.Name;
			}
			else {
				result = DefaultLanguage;
			}
			return result;
		}
		public static bool IsAncestorAspect(string ancestorAspect, string descendantAspect) {
			bool result = false;
			if(ancestorAspect != descendantAspect) {
				if(descendantAspect != DefaultLanguage) {
					string testAspect = descendantAspect;
					while(testAspect != null && testAspect != ancestorAspect) {
						testAspect = GetParentAspect(testAspect);
					}
					if(testAspect == ancestorAspect)
						result = true;
				}
			}
			return result;
		}
		public static CultureInfo GetCultureInfoByAspect(string aspect) {
			if(aspect == DefaultLanguage) {
				aspect = "";
			}
			return new CultureInfo(aspect);
		}
	}
}
