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
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using System.Reflection;
using DevExpress.Utils.Design;
using DevExpress.Utils.Frames;
namespace DevExpress.XtraEditors.FeatureBrowser {
	public enum FeatureBrowserLinkErrorType {ItemLink, PropertyLink, PropertyName, ReferenceLink, SelectedPropetyOnStart};
	public class FeatureBrowserLinkError {
		FeatureBrowserItem item;
		FeatureBrowserLinkErrorType errorType;
		string link;
		public FeatureBrowserLinkError(FeatureBrowserItem item, FeatureBrowserLinkErrorType errorType, string link) {
			this.item = item;
			this.errorType = errorType;
			this.link = link;
		}
		public FeatureBrowserItem Item { get { return item; } }
		public FeatureBrowserLinkErrorType ErrorType { get { return errorType; } }
		public string Link { get { return link; } }
		public string Text {
			get {
				return "Item Id: " + Item.Id + ", Item Name: " + Item.Name +
					", ErrorType: " + ErrorType.ToString() + ", Link Name: " + Link;
			}
		}
	}
	public class FeatureBrowserLinkErrorCollection : CollectionBase {
		public FeatureBrowserLinkErrorCollection() {
		}
		public FeatureBrowserLinkError this[int index] { get { return InnerList[index] as FeatureBrowserLinkError; } }
		public void Add(FeatureBrowserItem item, FeatureBrowserLinkErrorType errorType, string link) {
			InnerList.Add(new FeatureBrowserLinkError(item, errorType, link));
		}
	}
	public class FeatureBrowserLinkChecker	{
		FeatureBrowserLinkErrorCollection errors;
		FeatureBrowserItem root;
		object sourceObject;
		ArrayList items;
		FeatureLabelInfo labelInfo;
		public FeatureBrowserLinkChecker(FeatureBrowserItem root, object sourceObject) {
			this.root = root;
			this.sourceObject = sourceObject;
			this.errors = new FeatureBrowserLinkErrorCollection();
			this.items = new ArrayList();
			this.labelInfo = new FeatureLabelInfo();
			this.labelInfo.SourceObject = sourceObject;
		}
		public FeatureBrowserItem Root { get { return root; } }
		public object SourceObject { get { return sourceObject; } }
		public FeatureBrowserLinkErrorCollection Errors { get { return errors; } }
		public void Run() {
			CheckErrors(Root);
			if(Errors.Count > 0)
				ShowErrors();
			else MessageBox.Show("No errors were found!");
		}
		void CheckErrors(FeatureBrowserItem item) {
			CheckReferenceLink(item);
			CheckProperties(item);
			CheckDescription(item);
			for(int i = 0; i < item.Count; i ++)
				CheckErrors(item[i]);
		}
		void CheckReferenceLink(FeatureBrowserItem item) {
			if(item.ReferenceId != string.Empty && item.ReferenceItem == null)
				Errors.Add(item, FeatureBrowserLinkErrorType.ReferenceLink, item.ReferenceId);
		}
		void CheckProperties(FeatureBrowserItem item) {
			for(int i = 0; i < item.Pages.Count; i ++)
				CheckProperties(item, item.Pages[i]);
		}
		void CheckProperties(FeatureBrowserItem item, FeatureBrowserItemPage page) {
			if(page.Properties.Length == 0) return;
			string pageName = page.Name != string.Empty ? page.Name : "Default";
			pageName = "Page: "  + pageName + ", Property: ";
			FilterObject filterObject = GetFilterObject(item, page);
			if(!IsPropertyExists(filterObject, page.SelectedPropertyOnStart))
				Errors.Add(item, FeatureBrowserLinkErrorType.SelectedPropetyOnStart, page.SelectedPropertyOnStart); 
			for(int i = 0; i < page.Properties.Length; i ++) {
				if(!IsPropertyExists(filterObject, page.Properties[i]))
					Errors.Add(item, FeatureBrowserLinkErrorType.PropertyName, pageName + page.Properties[i]); 
			}
		}
		void CheckDescription(FeatureBrowserItem item) {
			for(int i = 0; i < item.Pages.Count; i ++)
				CheckDescription(item, item.Pages[i]);		}
		void CheckDescription(FeatureBrowserItem item, FeatureBrowserItemPage page) {
			if(page.Description == string.Empty) return;
			labelInfo.Text = page.Description;
			for(int i = 0; i < labelInfo.Texts.Count; i ++) {
				FeatureLabelInfoText featureInfo = labelInfo.Texts[i].Tag as FeatureLabelInfoText;
				if(featureInfo != null)
					CheckLabelInfoText(item, featureInfo);
			}
		}
		void CheckLabelInfoText(FeatureBrowserItem item, FeatureLabelInfoText featureInfo) {
			CheckDescriptionGotoFeature(item, featureInfo);
			CheckDescriptionGoto(item, featureInfo);
			CheckDescriptionPropertyName(item, featureInfo);
		}
		void CheckDescriptionGotoFeature(FeatureBrowserItem item, FeatureLabelInfoText featureInfo) {
			if(featureInfo.GotoFeatureName == string.Empty) return;
			if(Root.FindItemById(featureInfo.GotoFeatureName ) == null)
				Errors.Add(item, FeatureBrowserLinkErrorType.ItemLink, featureInfo.OriginalText);
		}
		void CheckDescriptionGoto(FeatureBrowserItem item, FeatureLabelInfoText featureInfo) {
			if(featureInfo.GotoName == string.Empty) return;
			if(!IsPropertyExists(item, item.Pages[featureInfo.GotoName], featureInfo.GotoValue))
				Errors.Add(item, FeatureBrowserLinkErrorType.PropertyLink, featureInfo.OriginalText);
		}
		void CheckDescriptionPropertyName(FeatureBrowserItem item, FeatureLabelInfoText featureInfo) {
			if(featureInfo.PropertyName == string.Empty) return;
			if(!IsPropertyExists(item, item.Pages[string.Empty], featureInfo.PropertyName))
				Errors.Add(item, FeatureBrowserLinkErrorType.PropertyLink, featureInfo.OriginalText);
		}
		bool IsPropertyExists(FilterObject filterObject, string propertyName) {
			if(propertyName == string.Empty) return true;
			if(filterObject == null) return false;
			if(new ObjectValueGetter(filterObject).GetPropertyDescriptor(propertyName) != null)
				return true;
			return TypeDescriptor.GetEvents(filterObject).Find(propertyName, false) != null;
		}
		bool IsPropertyExists(FeatureBrowserItem item, FeatureBrowserItemPage page, string propertyName) {
			return IsPropertyExists(GetFilterObject(item, page), propertyName);
		}
		FilterObject GetFilterObject(FeatureBrowserItem item, FeatureBrowserItemPage page) {
			if(page == null) return null;
			object inspectedObject = item.SourceProperty != string.Empty ? new ObjectValueGetter(SourceObject).GetValue(item.SourceProperty) : SourceObject;
			if(page.Name != string.Empty) {
				inspectedObject = new ObjectValueGetter(inspectedObject).GetValue(page.Name);
				if(inspectedObject != null && inspectedObject is IList && (inspectedObject as IList).Count > 0) {
					inspectedObject = ((IList)inspectedObject)[0];
				}
			}
			if(inspectedObject == null) return null;
			return new FilterObject(inspectedObject, page.Properties);
		}
		void ShowErrors() {
			Form form = new Form();
			form.Size = new Size(500, 400);
			MemoEdit memo = new MemoEdit();
			memo.Parent = form;
			memo.Dock = DockStyle.Fill;
			ArrayList list = new ArrayList();
			list.Add("Error count: " + Errors.Count.ToString());
			for(int i = 0; i < Errors.Count; i ++) {
				list.Add(Errors[i].Text);
			}
			memo.Lines = (string[])list.ToArray(typeof(string));
			form.ShowDialog();
		}
	}
}
