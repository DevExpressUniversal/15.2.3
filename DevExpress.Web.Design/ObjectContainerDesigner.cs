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
using System.ComponentModel.Design;
using System.Web.UI;
using System.Web.UI.Design;
using System.Web.UI.WebControls;
using DevExpress.Web.Internal;
namespace DevExpress.Web.Design {
	public class ASPxObjectContainerDesigner : ASPxWebControlDesigner {
		private ASPxObjectContainer fObjectContainer = null;
		public ASPxObjectContainer ObjectContainer {
			get { return fObjectContainer; }
		}
		public override void Initialize(IComponent component) {
			fObjectContainer = (ASPxObjectContainer)component;
			base.Initialize(component);
		}
		protected override string GetDesignTimeHtmlInternal() {
			string imageUrl = GetDesignImageUrl();
			if(!string.IsNullOrEmpty(imageUrl)) {
				Unit width = GetDesignImageWidth();
				Unit height = GetDesignImageHeight();
				return GetDesignTimeHtmlByObjectType(width, height, imageUrl);
			}
			return base.GetDesignTimeHtmlInternal();
		}
		private Unit GetDesignImageHeight() {
			if(!ObjectContainer.Height.IsEmpty)
				return ObjectContainer.Height;
			else if(!ObjectContainer.Width.IsEmpty)
				return ObjectContainer.Width;
			return 150;
		}
		private Unit GetDesignImageWidth() {
			if(!ObjectContainer.Width.IsEmpty)
				return ObjectContainer.Width;
			else if(!ObjectContainer.Height.IsEmpty)
				return ObjectContainer.Height;
			return 150;
		}
		private string GetDesignImageUrl() {
			if(ObjectContainer.ObjectPropertiesInternal.RenderAsObject()) {
				switch(ObjectContainer.ActualObjectType) {
					case ObjectType.Image:
						return ObjectContainer.GetImageImage().Url;
					case ObjectType.Flash:
						return ObjectContainer.GetFlashImage().Url;
					case ObjectType.Video:
						return ObjectContainer.GetVideoImage().Url;
					case ObjectType.Audio:
						return ObjectContainer.GetAudioImage().Url;
					case ObjectType.QuickTime:
						return ObjectContainer.GetQuickTimeImage().Url;
				}
			}
			else
				switch(ObjectContainer.ObjectType) {
					case ObjectType.Html5Video:
						return ObjectContainer.GetHtml5VideoImage().Url;
					case ObjectType.Html5Audio:
						return ObjectContainer.GetHtml5AudioImage().Url;
					case ObjectType.Auto:
						if(ObjectContainer.ObjectUrl != "" && !ObjectTypeManager.HasType(ObjectContainer.ObjectUrl, false))
							return ObjectContainer.GetErrorImage().Url;
						break;
				}
			return "";
		}
		private string GetDesignTimeHtmlByObjectType(Unit width, Unit height, string imageUrl) {
			return string.Format("<table width=\"{0}\" height=\"{1}\" cellpadding=\"4\" cellspacing=\"0\" style=\"font: messagebox; color: buttontext; background-color: buttonface; border: solid 1px; border-top-color: buttonhighlight; border-left-color: buttonhighlight; border-bottom-color: buttonshadow; border-right-color: buttonshadow;\"><tr><td style=\"white-space: nowrap; background-image: url({2}); background-repeat: no-repeat; background-position: center center;\"><div style=\"width: 0px; height: 0px; overflow: hidden;\"></div></td></tr></table>",
				width.ToString(), height.ToString(), imageUrl);
		}
		protected override string GetBaseProperty() {
			return "ObjectUrl";
		}
		public override bool IsThemableControl() {
			return false;
		}
		protected override ASPxWebControlDesignerActionList CreateCommonActionList() {
			return new ObjectContainerDesignerActionList(this);
		}
	}
	public class ObjectContainerDataBindingHandler : DataBindingHandler {
		public ObjectContainerDataBindingHandler() {
		}
		public override void DataBindControl(IDesignerHost designerHost, Control control) {
			ASPxObjectContainer objectContainer = control as ASPxObjectContainer;
			if (objectContainer == null)
				return;
			DataBinding bindingText = ((IDataBindingsAccessor)control).DataBindings["ObjectUrl"];
			if (bindingText != null)
				objectContainer.ObjectUrl = StringResources.ObjectContainer_SampleObjectUrl;
		}
	}
	public class ObjectContainerDesignerActionList : ASPxWebControlDesignerActionList {
		public ObjectContainerDesignerActionList(ASPxObjectContainerDesigner designer)
			: base(designer) {
		}
		[UrlProperty, Editor(typeof(System.Web.UI.Design.UrlEditor), typeof(System.Drawing.Design.UITypeEditor))]
		public string ObjectUrl {
			get { return (Designer as ASPxObjectContainerDesigner).ObjectContainer.ObjectUrl; }
			set {
				(Designer as ASPxObjectContainerDesigner).ObjectContainer.ObjectUrl = value;
				Designer.PropertyChanged("ObjectUrl");
			}
		}
		public ObjectType ObjectType {
			get { return (Designer as ASPxObjectContainerDesigner).ObjectContainer.ObjectType; }
			set { (Designer as ASPxObjectContainerDesigner).ObjectContainer.ObjectType = value; }
		}
		public override DesignerActionItemCollection GetSortedActionItems() {
			DesignerActionItemCollection actionItems = base.GetSortedActionItems();
			actionItems.Add(new DesignerActionPropertyItem("ObjectUrl", "Object Url"));
			actionItems.Add(new DesignerActionPropertyItem("ObjectType", "Object Type"));
			return actionItems;
		}
	}
}
