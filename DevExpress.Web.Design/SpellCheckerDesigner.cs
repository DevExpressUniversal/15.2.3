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
using System.ComponentModel;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.Globalization;
using System.Reflection;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.Web.Design;
using DevExpress.Web.Internal;
using System.Collections;
using System.Windows.Forms.Design;
using System.Drawing.Design;
using System.Windows.Forms;
using System.Drawing;
namespace DevExpress.Web.ASPxSpellChecker.Design {
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2117")]
	public class ASPxSpellCheckerDesigner : ASPxWebControlDesigner {
		public static FormsInfo FormsInfo = new SpellCheckerFormsInfo(); 
		private ASPxSpellChecker spellChecker = null;
		public ASPxSpellChecker SpellChecker {
			get { return spellChecker; }
		}
		protected override bool UsePreviewControl {
			get { return !IsRootDesignerDummy(); } 
		}
		public override void Initialize(IComponent component) {
			this.spellChecker = (ASPxSpellChecker)component;
			base.Initialize(component);
			EnsureReferences(
				AssemblyInfo.SRAssemblySpellCheckerCore,
				AssemblyInfo.SRAssemblySpellCheckerWeb
			);
		}
		protected override void FillPropertyNameToCaptionMap(Dictionary<string, string> propertyNameToCaptionMap) {
			base.FillPropertyNameToCaptionMap(propertyNameToCaptionMap);
			propertyNameToCaptionMap.Add("Dictionaries", "Dictionaries");
		}
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2116")]
		protected override string GetDesignTimeHtmlInternal() {
			if(!((ASPxSpellChecker)ViewControl).IsAutoFormatPreview)
				return CreatePlaceHolderDesignTimeHtml();
			return base.GetDesignTimeHtmlInternal();
		}
		protected override FormsInfo[] GetFormsInfoArray() {
			return new FormsInfo[] { FormsInfo };
		}
		protected override bool NeedCopyFormsOnInitialize() {
			return false;
		}
		protected override Object GetControlSettingsForms(FormsInfo formsInfo) {
			if(formsInfo == FormsInfo)
				return SpellChecker.SettingsForms;
			return base.GetControlSettingsForms(formsInfo);
		}
		protected override ASPxWebControlDesignerActionList CreateCommonActionList() {
			return new SpellCheckerDesignerActionList(this);
		}
		public override bool HasCopyDefaultDialogFormsToTheProject() {
			return true;
		}
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2116")]
		public override void ShowAbout() {
			SpellCheckerAboutDialogHelper.ShowAbout(Component.Site);
		}
		public void EditDictionaries() {
			ShowDialog(CreateEditorForm("Dictionaries", SpellChecker.Dictionaries));
		}
		public override void RunDesigner() {
			ShowDialog(new WrapperEditorForm(new SpellCheckerCommonFormDesigner(spellChecker, DesignerHost)));
		}
	}
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2117")]
	public class SpellCheckerDesignerActionList : ASPxWebControlDesignerActionList {
		private ASPxSpellCheckerDesigner designer;
		public SpellCheckerDesignerActionList(ASPxSpellCheckerDesigner designer)
			: base(designer) {
			this.designer = designer;
		}
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2116")]
		public override DesignerActionItemCollection GetSortedActionItems() {
			DesignerActionItemCollection collection = base.GetSortedActionItems();
			collection.Add(new DesignerActionMethodItem(this, "OpenHelp",
				"How to setup dictionaries", true));			
			return collection;
		}
		protected void EditDictionaries() {
			this.designer.EditDictionaries();
		}
		public void OpenHelp() {
			SpellCheckerHelper.OpenHowToSetupDictionaries();
		}
	}
	public class SpellCheckerFormsInfo : FormsInfo {
		public override string ControlName { get { return "ASPxSpellChecker"; } }
		public override bool NeedCopyDesignerFileFromResource { get { return true; } }
		public override string[] FormNames { get { return ASPxSpellChecker.FormNames; } }
		public override Type Type { get { return typeof(ASPxSpellChecker); } }
	}
	public static class SpellCheckerHelper {
		public static void OpenHowToSetupDictionaries() { 
			System.Diagnostics.Process.Start("http://documentation.devexpress.com/#AspNet/CustomDocument4089/AddDictionaries");
		}
	}
	public class SpellCheckerCommonFormDesigner : CommonFormDesigner {
		public SpellCheckerCommonFormDesigner(ASPxSpellChecker spellChecker, IServiceProvider provider)
			: base(new SpellCheckerDictionaryOwner(spellChecker, provider, spellChecker.Dictionaries)) {
			ItemsImageIndex = DictionariesImageIndex;
		}
	}
	public class SpellCheckerDictionaryOwner : FlatCollectionOwner {
		public SpellCheckerDictionaryOwner(object component, IServiceProvider provider, SpellCheckerDictionaryCollection dictionaries)
			: base(component, "Dictionaries", provider, dictionaries) {
		}
		protected override void FillItemTypes() {
			AddItemType(typeof(ASPxSpellCheckerDictionary), "Simple Dictionary", SpellCheckerDictionaryImageResource);
			AddItemType(typeof(ASPxSpellCheckerCustomDictionary), "Custom Dictionary", SpellCheckerDictionaryImageResource);
			AddItemType(typeof(ASPxSpellCheckerISpellDictionary), "ISpell Dictionary", SpellCheckerDictionaryImageResource);
			AddItemType(typeof(ASPxSpellCheckerOpenOfficeDictionary), "Open Office Dictionary", SpellCheckerDictionaryImageResource);
			AddItemType(typeof(ASPxHunspellDictionary), "Hunspell Dictionary", SpellCheckerDictionaryImageResource);
		}
		public override Type GetDefaultItemType() {
			return typeof(ASPxSpellCheckerDictionary);
		}
		protected override DesignEditorDescriptorItem CreateAddItemMenuItem() {
			var item = base.CreateAddItemMenuItem();
			item.EditorType = DesignEditorDescriptorItemType.DropDownButton;
			return item;
		}
		protected override DesignEditorDescriptorItem CreateInsertBeforeMenuItem() {
			var item = base.CreateInsertBeforeMenuItem();
			item.EditorType = DesignEditorDescriptorItemType.DropDownButton;
			return item;
		}
		protected internal override string GetNavBarItemsGroupName() {
			return CollectionPropertyName;
		}
	}
}
