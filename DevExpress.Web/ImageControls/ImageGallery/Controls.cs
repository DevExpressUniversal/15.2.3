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
using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.WebControls;
namespace DevExpress.Web.Internal {
	public class ImageGalleryMainControl : DVMainControl {
		protected ASPxImageGallery ImageGallery {
			get { return DataView as ASPxImageGallery; }
		}
		protected bool IsVisibleFullscreenViewer {
			get { return ImageGallery.SettingsFullscreenViewer.Visible && ImageGallery.Enabled; }
		}
		protected bool HasVisibleItems {
			get { return ImageGallery.HasVisibleItems(); }
		}
		public ImageGalleryMainControl(ASPxImageGallery imageGallery)
			: base(imageGallery) {
		}
		protected override void PopulateTableRow(TableRow row) {
			base.PopulateTableRow(row);
			TableCell cell = RenderUtils.CreateTableCell();
			cell.ID = ImageGalleryContstants.FullscreenViewerCell;
			if(!DesignMode && HasVisibleItems && IsVisibleFullscreenViewer)
				cell.Controls.Add(new FullscreenViewer(ImageGallery));
			row.Cells.Add(cell);
		}
	}
	public class ImageGalleryPager : DVPager {
		public ImageGalleryPager(ASPxImageGallery imageGallery)
			: base(imageGallery) {
		}
		protected override PagerSettingsEx CreatePagerSettings(ASPxPagerBase pager) {
			return new ImageGalleryPagerSettings(pager);
		}
	}
	public abstract class ImageGalleryBaseControl : ASPxInternalWebControl {
		protected ASPxImageGallery ImageGallery { get; private set; }
		protected ImageGalleryDataHelper DataHelper { get { return ImageGallery.GalleryDataHelper; } }
		public ImageGalleryBaseControl(ASPxImageGallery imageGallery) {
			ImageGallery = imageGallery;
		}
	}
	public abstract class ThumbnailItemControlBase : ImageGalleryBaseControl {
		protected ImageGalleryItem Item { get; private set; }
		public ThumbnailItemControlBase(ImageGalleryItem item)
			: base(item.Collection.ImageGallery) {
			Item = item;
		}
	}
	[ToolboxItem(false)]
	public class ThumbnailItemControlDesignMode : ThumbnailItemControlBase {
		protected WebControl ItemElement { get; private set; }
		public ThumbnailItemControlDesignMode(ImageGalleryItem item)
			: base(item) {
		}
		protected override void CreateControlHierarchy() {
			base.CreateControlHierarchy();
			ItemElement = RenderUtils.CreateDiv();
			Controls.Add(ItemElement);
		}
		protected override void PrepareControlHierarchy() {
			base.PrepareControlHierarchy();
			ItemElement.CssClass = ImageGallery.Styles.GetCssClassNamePrefix() + "-ti";
			ItemElement.Width = Convert.ToInt32(ImageGallery.GetItemWidth().Value);
			ItemElement.Height = Convert.ToInt32(ImageGallery.GetItemHeight().Value);
			ItemElement.Style["background-image"] = string.Format("url({0})", GetItemImageUrl());
		}
		protected string GetItemImageUrl() {
			return ImageGallery.Images.GetImageProperties(Page, ImageGalleryImages.DesignTimeItemImageName).Url;
		}
	}
	[ToolboxItem(false)]
	public class ThumbnailItemControl : ThumbnailItemControlBase {
		private const string ImageCssClassName = "img";
		protected WebControl Container { get { return ThumbnailWrapper ?? this; } }
		protected Image Image { get; private set; }
		protected InternalHyperLink Link { get; private set; }
		protected WebControl TextArea { get; private set; }
		protected WebControl BorderElement { get; private set; }
		protected WebControl ThumbnailWrapper { get; private set; }
		protected ElementVisibilityMode TextVisibility { get { return DataHelper.GetThumbnailTextVisibility(); } }
		public ThumbnailItemControl(ImageGalleryItem item)
			: base(item) {
		}
		protected override void CreateControlHierarchy() {
			base.CreateControlHierarchy();
			CreateThumbnailWrapper();
			CreateHyperLink();
			CreateImage();
			CreateBorder();
			CreateTextArea();
		}
		protected virtual void CreateBorder() {
			BorderElement = RenderUtils.CreateWebControl(HtmlTextWriterTag.Span);
			Link.Controls.Add(BorderElement);
		}
		protected virtual void CreateThumbnailWrapper() {
			if(ImageGallery.LayoutInternal == Layout.Table) {
				ThumbnailWrapper = RenderUtils.CreateDiv();
				ImageGallery.GetThumbnailWrapperStyle().AssignToControl(ThumbnailWrapper);
				Controls.Add(ThumbnailWrapper);
			}
		}
		protected virtual void CreateHyperLink() {
			Link = RenderUtils.CreateHyperLink(true, true);
			Container.Controls.Add(Link);
		}
		protected virtual void CreateImage() {
			Image = RenderUtils.CreateImage();
			Link.Controls.Add(Image);
		}
		protected virtual void CreateTextArea() {
			if(TextVisibility == ElementVisibilityMode.None)
				return;
			ITemplate template = DataHelper.GetThumbnailTextTemplate(Item);
			if(DataHelper.GetThumbnailText(Item) != string.Empty || template != null) {
				TextArea = RenderUtils.CreateDiv();
				if(template != null)
					CreateTemplate(template);
				else
					TextArea.Controls.Add(RenderUtils.CreateLiteralControl(DataHelper.GetThumbnailText(Item)));
				Container.Controls.Add(TextArea);
			}
		}
		protected virtual void CreateTemplate(ITemplate template) {
			ImageGalleryThumbnailTemplateContainer templateContainer = new ImageGalleryThumbnailTemplateContainer(Item);
			templateContainer.AddToHierarchy(TextArea, GetTemplateContainerID());
			template.InstantiateIn(templateContainer);
		}
		protected virtual string GetTemplateContainerID() {
			return "PTT" + Item.Index;
		}
		protected override void PrepareControlHierarchy() {
			base.PrepareControlHierarchy();
			PrepareLink();
			PrepareImage();
			PrepareBorder();
			PrepapreTextArea();
		}
		protected virtual void PrepareBorder() {
			ImageGallery.GetThumbnailBorderStyle().AssignToControl(BorderElement);
		}
		protected virtual void PrepareLink() {
			Link.NavigateUrl = DataHelper.GetNavigateUrl(Item);
			if(!string.IsNullOrEmpty(Link.NavigateUrl))
				Link.Target = ImageGallery.Target;
		}
		protected virtual void PrepareImage() {
			RenderUtils.AppendDefaultDXClassName(Image, GetImageCssClassName());
			Image.ImageUrl = ImageUtilsHelper.EncodeImageUrl(DataHelper.GetThumbnailUrl(Item));
			if(TextVisibility != ElementVisibilityMode.None)
				Image.AlternateText = DataHelper.GetThumbnailText(Item);
			if(ImageGallery.AccessibilityCompliant && string.IsNullOrEmpty(Image.AlternateText))
				Image.AlternateText = ImageUtils.GetAlternateTextByUrl(Image.ImageUrl);				
		}
		protected virtual void PrepapreTextArea() {
			if(TextArea != null)
				ImageGallery.GetThumbnailTextAreaStyle().AssignToControl(TextArea);
		}
		private string GetImageCssClassName() {
			return string.Format("{0}-{1}", ImageGallery.GetCssClassNamePrefix(), ImageCssClassName);
		}
	}
	[ToolboxItem(false)]
	public class ImageSliderInternal : ASPxImageSlider {
		public ImageSliderInternal(ASPxWebControl owner, string id)
			: base(owner) {
			ID = id;
			ParentSkinOwner = owner;
			EncodeHtml = owner.EncodeHtml;
		}
		protected override bool CanAddItemsInViewState { get { return false; } }
	}
	[ToolboxItem(false)]
	public class FullscreenViewer : ASPxPopupControlBase {
		protected ASPxImageGallery ImageGallery { get { return OwnerControl as ASPxImageGallery; } }
		protected ImageGalleryFullscreenViewerSettings Settings { get { return ImageGallery.SettingsFullscreenViewer; } }
		protected FullscreenViewerMainPanel fMainPanel;
		protected FullscreenViewerBottomPanel fBottomPanel;
		protected bool MainPanelCreated { get { return fMainPanel != null; } }
		protected bool BottomPanelCreated { get { return fBottomPanel != null; } }
		protected FullscreenViewerMainPanel MainPanel {
			get {
				if(fMainPanel == null)
					fMainPanel = new FullscreenViewerMainPanel(ImageGallery);
				return fMainPanel;
			}
		}
		protected FullscreenViewerBottomPanel BottomPanel {
			get {
				if(fBottomPanel == null)
					fBottomPanel = new FullscreenViewerBottomPanel(ImageGallery);
				return fBottomPanel;
			}
		}
		public FullscreenViewer(ASPxImageGallery imageGallery)
			: base(imageGallery) {
			Maximized = true;
			ShowHeader = false;
			ShowShadow = false;
			EnableTheming = false;
			PopupAnimationType = AnimationType.Fade;
			CloseOnEscapeInternal = imageGallery.SettingsFullscreenViewer.KeyboardSupport;
			ID = ImageGalleryContstants.FullscreenViewerPopupID;
			ParentSkinOwner = imageGallery;
		}
		protected override void CreateControlHierarchy() {
			base.CreateControlHierarchy();
			if(!MainPanelCreated)
				Controls.Add(MainPanel);
			if(!BottomPanelCreated && (Settings.ShowTextArea || Settings.NavigationBarVisibility != ElementVisibilityMode.None))
				Controls.Add(BottomPanel);
		}
	}
	[ToolboxItem(false)]
	public abstract class FullscreenViewerPanelBase : ImageGalleryBaseControl {
		protected ImageGalleryItemCollection Items {
			get { return ImageGallery.Items; }
		}
		protected ImageGalleryFullscreenViewerSettings Settings {
			get { return ImageGallery.SettingsFullscreenViewer; }
		}
		public FullscreenViewerPanelBase(ASPxImageGallery imageGallery)
			: base(imageGallery) {
		}
		protected override void CreateControlHierarchy() {
			base.CreateControlHierarchy();
			CreateControlHierarchyInternal();
			Items.ForEach(item => BindItem(item as ImageGalleryItem));
		}
		protected virtual void CreateControlHierarchyInternal() {
		}
		protected virtual void BindItem(ImageGalleryItem item) {
		}
	}
	[ToolboxItem(false)]
	public class FullscreenViewerMainPanel : FullscreenViewerPanelBase {
		protected ImageSliderInternal ImageSlider { get; private set; }
		protected WebControl ImageSliderWrapper { get; private set; }
		protected WebControl PrevButton { get; private set; }
		protected WebControl NextButton { get; private set; }
		protected WebControl PrevButtonArea { get; private set; }
		protected WebControl NextButtonArea { get; private set; }
		protected WebControl CloseButtonWrapper { get; private set; }
		protected WebControl CloseButton { get; private set; }
		protected WebControl PlayPauseButtonWrapper { get; private set; }
		protected WebControl PlayButton { get; private set; }
		protected WebControl PauseButton { get; private set; }
		public FullscreenViewerMainPanel(ASPxImageGallery imageGallery)
			: base(imageGallery) {
		}
		protected override void CreateControlHierarchyInternal() {
			base.CreateControlHierarchyInternal();
			CreateImageSlider();
			CreatePrevButton();
			CreateNextButton();
			CreateCloseButton();
			CreatePlayPauseButton();
		}
		protected override void BindItem(ImageGalleryItem item) {
			ImageSliderItem sliderItem = new ImageSliderItem();
			sliderItem.Name = item.Name;
			sliderItem.ImageUrl = DataHelper.GetFullscreenViewerImageUrl(item);
			if(!DataHelper.HasFullscreenViewerTemplates)
				sliderItem.Text = DataHelper.GetFullscreenViewerText(item);
			ImageSlider.ItemsInternal.Add(sliderItem);
		}
		protected virtual void CreateImageSlider() {
			ImageSliderWrapper = RenderUtils.CreateDiv();
			Controls.Add(ImageSliderWrapper);
			ImageSlider = new ImageSliderInternal(ImageGallery, ImageGalleryContstants.FullscreenViewerImageSliderID);
			ImageSliderWrapper.Controls.Add(ImageSlider);
			ImageSlider.KeyboardSupport = Settings.KeyboardSupport;
			ImageSlider.SettingsImageArea.AnimationType = Settings.AnimationType;
			ImageSlider.SettingsSlideShow.Interval = Settings.SlideShowInterval;
			ImageSlider.SettingsBehavior.EnablePagingGestures = Settings.EnablePagingGestures;
			ImageSlider.SettingsBehavior.ImageLoadMode = Settings.ImageLoadMode;
			ImageSlider.SettingsImageArea.ImageSizeMode = Settings.ImageSizeMode;
			ImageSlider.ShowNavigationBar = false;
			ImageSlider.SettingsBehavior.AllowMouseWheel = true;
			ImageSlider.SettingsImageArea.ItemTextVisibility = ElementVisibilityMode.None;
			ImageSlider.SettingsImageArea.NavigationButtonVisibility = ElementVisibilityMode.None;
		}
		protected virtual void CreateCloseButton() {
			if(Settings.ShowCloseButton) {
				CloseButtonWrapper = RenderUtils.CreateDiv();
				Controls.Add(CloseButtonWrapper);
				CloseButton = RenderUtils.CreateDiv();
				CloseButton.ID = ImageGalleryContstants.FullscreenViewerCloseButtonID;
				CloseButtonWrapper.Controls.Add(CloseButton);
			}
		}
		protected virtual void CreatePlayPauseButton() {
			if(Settings.ShowPlayPauseButton) {
				PlayPauseButtonWrapper = RenderUtils.CreateDiv();
				Controls.Add(PlayPauseButtonWrapper);
				PlayButton = RenderUtils.CreateDiv();
				PlayPauseButtonWrapper.Controls.Add(PlayButton);
				PauseButton = RenderUtils.CreateDiv();
				PlayPauseButtonWrapper.Controls.Add(PauseButton);
			}
		}
		protected virtual void CreatePrevButton() {
			if(Settings.NavigationButtonVisibility != ElementVisibilityMode.None) {
				PrevButtonArea = RenderUtils.CreateDiv();
				Controls.Add(PrevButtonArea);
				PrevButton = RenderUtils.CreateDiv();
				PrevButton.ID = ImageGalleryContstants.FullscreenViewerPrevButtonID;
				PrevButtonArea.Controls.Add(PrevButton);
			}
		}
		protected virtual void CreateNextButton() {
			if(Settings.NavigationButtonVisibility != ElementVisibilityMode.None) {
				NextButtonArea = RenderUtils.CreateDiv();
				Controls.Add(NextButtonArea);
				NextButton = RenderUtils.CreateDiv();
				NextButton.ID = ImageGalleryContstants.FullscreenViewerNextButtonID;
				NextButtonArea.Controls.Add(NextButton);
			}
		}
		protected override void PrepareControlHierarchy() {
			base.PrepareControlHierarchy();
			PrepareImageSlider();
			PrepareCloseButton();
			PreparePlayPauseButton();
			PreparePrevButton();
			PrepareNextButton();
		}
		protected virtual void PrepareImageSlider() {
			ImageSlider.Styles.Assign(ImageGallery.StylesFullscreenViewer);
			ImageSlider.Width = new Unit("100%");
			ImageSlider.Height = new Unit("100%");
			ImageGallery.GetImageSliderWrapperStyle().AssignToControl(ImageSliderWrapper);
			ImageSlider.ClientSideEvents.ItemClick = string.Format(ImageGalleryContstants.OnImageSliderItemClick, ImageGallery.ClientID);
			ImageSlider.AccessibilityCompliant = ImageGallery.AccessibilityCompliant;
		}
		protected virtual void PrepareCloseButton() {
			if(Settings.ShowCloseButton) {
				ImageGallery.GetCloseButtonStyle().AssignToControl(CloseButton);
				ImageGallery.GetCloseButtonImage().AssignToControl(CloseButton, DesignMode, !Enabled);
				ImageGallery.GetCloseButtonWrapperStyle().AssignToControl(CloseButtonWrapper);
			}
		}
		protected virtual void PreparePlayPauseButton() {
			if(Settings.ShowPlayPauseButton) {
				AppearanceStyle buttonStyle = ImageGallery.GetPlayPauseButtonStyle();
				buttonStyle.AssignToControl(PlayButton);
				buttonStyle.AssignToControl(PauseButton);
				ImageGallery.GetPlayButtonImage().AssignToControl(PlayButton, DesignMode, !Enabled);
				ImageGallery.GetPauseButtonImage().AssignToControl(PauseButton, DesignMode, !Enabled);
				ImageGallery.GetPlayPauseButtonWrapperStyle().AssignToControl(PlayPauseButtonWrapper);
			}
		}
		protected virtual void PreparePrevButton() {
			if(Settings.NavigationButtonVisibility != ElementVisibilityMode.None) {
				ImageGallery.GetPrevButtonStyle().AssignToControl(PrevButton);
				ImageGallery.GetPrevButtonImage().AssignToControl(PrevButton, DesignMode, !Enabled);
			}
		}
		protected virtual void PrepareNextButton() {
			if(Settings.NavigationButtonVisibility != ElementVisibilityMode.None) {
				ImageGallery.GetNextButtonStyle().AssignToControl(NextButton);
				ImageGallery.GetNextButtonImage().AssignToControl(NextButton, DesignMode, !Enabled);
			}
		}
	}
	[ToolboxItem(false)]
	public class FullscreenViewerBottomPanel : FullscreenViewerPanelBase {
		protected ImageSliderInternal NavigationBar { get; private set; }
		protected WebControl NavigationBarMarker { get; private set; }
		protected WebControl OverflowPanel { get; private set; }
		protected WebControl BottomPanel { get; private set; }
		protected WebControl TextArea { get; private set; }
		protected bool ShowTextArea { get { return DataHelper.CanRenderFullscreenViewerTextArea; } }
		protected bool ShowNavigationBar { get { return Settings.NavigationBarVisibility != ElementVisibilityMode.None; } }
		public FullscreenViewerBottomPanel(ASPxImageGallery imageGallery)
			: base(imageGallery) {
			CreateBottomPanel(); 
		}
		protected override void CreateControlHierarchyInternal() {
			base.CreateControlHierarchyInternal();
			CreateNavigationBar();
			CreateNavigationBarMarker();
			CreateTextArea();
		}
		protected override void BindItem(ImageGalleryItem item) {
			if(ShowNavigationBar) {
				ImageSliderItem sliderItem = new ImageSliderItem();
				sliderItem.ThumbnailUrl = DataHelper.GetFullscreenViewerThumbnailUrl(item);
				if(ImageGallery.AccessibilityCompliant && !DataHelper.HasFullscreenViewerTemplates)
					sliderItem.Text = DataHelper.GetFullscreenViewerText(item);
				NavigationBar.Items.Add(sliderItem);
			}
		}
		protected virtual void CreateBottomPanel() {
			BottomPanel = RenderUtils.CreateDiv();
			Controls.Add(BottomPanel);
		}
		protected virtual void CreateNavigationBar() {
			if(!ShowNavigationBar)
				return;
			if(Settings.NavigationBarVisibility != ElementVisibilityMode.Always)
				CreateOverflowPanel();
			WebControl container = OverflowPanel ?? BottomPanel;
			NavigationBar = new ImageSliderInternal(ImageGallery, ImageGalleryContstants.FullscreenViewerNavigationBarID);
			container.Controls.Add(NavigationBar);
			NavigationBar.KeyboardSupport = Settings.KeyboardSupport;
			NavigationBar.SettingsBehavior.ImageLoadMode = Settings.ImageLoadMode;
			NavigationBar.SettingsBehavior.EnablePagingGestures = Settings.EnablePagingGestures;
			NavigationBar.ShowImageArea = false;
			NavigationBar.SettingsNavigationBar.ThumbnailsModeNavigationButtonVisibility = ElementVisibilityMode.Always;
		}
		protected virtual void CreateOverflowPanel() {
			OverflowPanel = RenderUtils.CreateDiv();
			BottomPanel.Controls.Add(OverflowPanel);
		}
		protected virtual void CreateNavigationBarMarker() {
			if(ShowNavigationBar && !RenderUtils.Browser.Platform.IsTouchUI && Settings.NavigationBarVisibility != ElementVisibilityMode.Always) {
				NavigationBarMarker = RenderUtils.CreateDiv();
				OverflowPanel.Controls.Add(NavigationBarMarker);
			}
		}
		protected virtual void CreateTextArea() {
			if(ShowTextArea) {
				TextArea = RenderUtils.CreateDiv();
				BottomPanel.Controls.Add(TextArea);
				if(DataHelper.HasFullscreenViewerStaticTextTemplate)
					CreateStaticTextTemplate();
				else if(DataHelper.HasFullscreenViewerTextTemplate)
					CreateItemTextTemplates();
			}
		}
		protected virtual void CreateStaticTextTemplate() {
			ITemplate template = DataHelper.GetFullscreenViewerStaticTextTemplate();
			ImageGalleryFullscreenViewerItemTemplateContainer templateContainer = new ImageGalleryFullscreenViewerItemTemplateContainer();
			templateContainer.AddToHierarchy(TextArea, "IVSTT");
			template.InstantiateIn(templateContainer);
		}
		protected virtual void CreateItemTextTemplates() {
			foreach(ImageGalleryItem item in Items) {
				WebControl wrapper = CreateItemTemplateWrapper();
				ITemplate template = DataHelper.GetFullscreenViewerTextTemplate(item);
				if(template != null) {
					ImageGalleryFullscreenViewerItemTemplateContainer templateContainer = new ImageGalleryFullscreenViewerItemTemplateContainer(item);
					templateContainer.AddToHierarchy(wrapper, "IVTT" + item.Index);
					template.InstantiateIn(templateContainer);
				} else
					AddText(wrapper, DataHelper.GetFullscreenViewerText(item));
			}
		}
		protected virtual WebControl CreateItemTemplateWrapper() {
			WebControl wrapper = RenderUtils.CreateDiv();
			TextArea.Controls.Add(wrapper);
			wrapper.CssClass = ImageGalleryContstants.FullscreenViewerItemTextTemplateClass;
			return wrapper;
		}
		protected override void PrepareControlHierarchy() {
			base.PrepareControlHierarchy();
			PrepareTextArea();
			PrepareBottomPanel();
			PrepareNavigationBar();
			PrepareOverflowPanel();
			PrepareNavigationBarMarker();
		}
		protected virtual void PrepareBottomPanel() {
			ImageGallery.GetBottomPanelStyle().AssignToControl(BottomPanel);
		}
		protected virtual void PrepareNavigationBar() {
			if(ShowNavigationBar) {
				NavigationBar.Styles.Assign(ImageGallery.StylesFullscreenViewerNavigationBar);
				NavigationBar.Width = new Unit("100%");
				NavigationBar.Styles.Thumbnail.Width = Settings.ThumbnailWidth;
				NavigationBar.Styles.Thumbnail.Height = Settings.ThumbnailHeight;
				NavigationBar.AccessibilityCompliant = ImageGallery.AccessibilityCompliant;
			}
		}
		protected virtual void PrepareOverflowPanel() {
			if(OverflowPanel != null)
				ImageGallery.GetOverflowPanelStyle().AssignToControl(OverflowPanel);
		}
		protected virtual void PrepareNavigationBarMarker() {
			if(NavigationBarMarker != null) {
				ImageGallery.GetNavigationBarMarkerStyle().AssignToControl(NavigationBarMarker);
				ImageGallery.GetNavigationBarMarkerImage().AssignToControl(NavigationBarMarker, DesignMode, !Enabled);
			}
		}
		protected virtual void PrepareTextArea() {
			if(TextArea != null)
				ImageGallery.GetFullscrenViewerTextAreaStyle().AssignToControl(TextArea);
		}
		protected void AddText(WebControl control, string text) {
			control.Controls.Add(RenderUtils.CreateLiteralControl(text));
		}
	}
}
