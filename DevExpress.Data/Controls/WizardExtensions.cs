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
using System.Windows;
using System.Windows.Forms;
namespace DevExpress.DXTemplateGallery.Extensions {
	public interface IGalleryExtension {
		UIElement CreateCustomizationControl();
		void Initialize(GalleryExtensionArgs args, string lang);
		void AddItem(IGalleryContext context);
		string InitializeParam(GalleryExtensionArgs args, string key, string value);
		IEnumerable<string> GetUsings(GalleryExtensionArgs args);
		IEnumerable<string> GetXmlns(GalleryExtensionArgs args);
		void ProjectItemAdded(object projectItem);
		void InitOptionsPanel(IOptionsPanel panelOptions);
		int? OptionsAreaWidth { get; }
	}
	public interface IGalleryExtededFieldSupports {
		string DefaultPrimaryItemName { get; }
		string DefaultSecondaryItemName { get; }
		string PrimaryFieldLabel { get; }
		string SecondaryFieldLabel { get; }
		string GetSecondaryFieldTextOnPrimaryFieldChanged(string text);
	}
	public interface IGalleryProjectExtension : IGalleryExtension {
		void DoWork(object dte, object solution, string projectName);
	}
	public interface IOptionsPanel {
		void SetPanelMargin(Thickness margin);
		void SetItemNameTextBoxMargin(Thickness margin);
	}
	public interface IGalleryNotificationSupports {
		void OnInitialized(string targetName);
		void OnTargetNameChanged(string oldName, string newName);
	}
	public class GalleryExtensionArgs {
		public object DTE { get; private set; }
		public IGalleryContext ParentContext { get; private set; }
		public IGalleryExtension ParentExtensionObj { get; private set; }
		public GalleryExtensionArgs(object dte) : this(dte, null, null) { }
		public GalleryExtensionArgs(object dte, IGalleryContext parentContext, IGalleryExtension extensionObj) {
			DTE = dte;
			ParentContext = parentContext;
			ParentExtensionObj = extensionObj;
		}
	}
	public interface IGalleryContext {
		Action<IGalleryContext, string> DoDefault { get; }
		void CancelDefault();
		object RootFolder { get; }
		void ChangeRootFolder(object rootFolder);
		bool IsCSharp { get; }
		bool IsVb { get; }
		void AddItem(object rootFolder, string strongName, string itemName);
		void AddItem(object rootFolder, string strongName, string itemName, IGalleryExtension parentExtensionObj, bool syncCall);
		IGalleryContext ParentContext { get; }
		IGalleryExtension ParentExtensionObj { get; }
		string ExtendedFieldValue { get; }
		object Dte { get; }
	}
	public class BaseGalleryExtensionException : Exception {
		public BaseGalleryExtensionException() : base(string.Empty) { }
		public BaseGalleryExtensionException(string msg)
			: base(msg) {
		}
	}
	public class TemplateNotFoundGalleryExtensionException : BaseGalleryExtensionException {
		public TemplateNotFoundGalleryExtensionException() {
		}
	}
}
