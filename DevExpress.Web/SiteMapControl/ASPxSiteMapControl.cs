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
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.Web;
using DevExpress.Web.Internal;
namespace DevExpress.Web {
	using DevExpress.Web.Internal;
	using DevExpress.Utils;
	using DevExpress.Web.Design;
	[DefaultEvent("ItemClick")
]
	public abstract class ASPxSiteMapControlBase: ASPxHierarchicalDataWebControl, IControlDesigner {
		private static NodeBulletStyle[] DefaultNodeBulletStyles =
			new NodeBulletStyle[] { NodeBulletStyle.Disc, NodeBulletStyle.Circle, NodeBulletStyle.Square };
		protected SiteMapNodeCollection fRootNodes = null;
		private SiteMapColumnCollection fColumns = null;
		private SiteMapColumnCollection fDummyColumns = null;
		private bool fInEnsureChildControls = false;
		private bool fInPerformDataBinding = false;
		private SMCMainControlBase fMainControl = null;
		private ITemplate fColumnSeparatorTemplate = null;
		private ITemplate fNodeTemplate = null;
		private ITemplate fNodeTextTemplate = null;
		private static readonly object EventNodeCommand = new object();
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxSiteMapControlBaseColumnCount"),
#endif
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		NotifyParentProperty(true), DefaultValue(typeof(byte), "0"),
		RefreshProperties(RefreshProperties.Repaint), AutoFormatDisable]
		public byte ColumnCount {
			get { return (byte)Columns.Count; }
			set {
				if(!IsLoading())
					SetColumnCount(value);
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxSiteMapControlBaseColumns"),
#endif
		NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		RefreshProperties(RefreshProperties.Repaint), PersistenceMode(PersistenceMode.InnerProperty),
		MergableProperty(false), AutoFormatDisable,
		Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.Drawing.Design.UITypeEditor))]
		public SiteMapColumnCollection Columns {
			get {
				if(fColumns == null)
					fColumns = new SiteMapColumnCollection(this);
				return fColumns;
			}
		}
		[Browsable(false)]
		public virtual SiteMapNodeCollection RootNodes {
			get { return fRootNodes; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxSiteMapControlBaseCategorized"),
#endif
		Category("Layout"), NotifyParentProperty(true), DefaultValue(false), AutoFormatEnable]
		public bool Categorized {
			get { return GetBoolProperty("Categorized", false); }
			set { SetBoolProperty("Categorized", false, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxSiteMapControlBaseFlowLayoutLevel"),
#endif
		Category("Layout"), NotifyParentProperty(true), DefaultValue(-1), AutoFormatEnable]
		public int FlowLayoutLevel {
			get { return GetIntProperty("FlowLayoutLevel", -1); }
			set {
				CommonUtils.CheckMinimumValue(value, -1, "FlowLayoutLevel");
				SetIntProperty("FlowLayoutLevel", -1, value);
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxSiteMapControlBaseFlowLayoutTextLineHeight"),
#endif
		Category("Layout"), DefaultValue(typeof(Unit), ""), AutoFormatEnable]
		public Unit FlowLayoutTextLineHeight {
			get { return GetUnitProperty("FlowLayoutTextLineHeight", Unit.Empty); }
			set {
				UnitUtils.CheckNegativeUnit(value, "FlowLayoutTextLineHeight");
				SetUnitProperty("FlowLayoutTextLineHeight", Unit.Empty, value);
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxSiteMapControlBaseRepeatDirection"),
#endif
		Category("Layout"), AutoFormatEnable, DefaultValue(RepeatDirection.Vertical)]
		public RepeatDirection RepeatDirection {
			get { return (RepeatDirection)GetEnumProperty("RepeatDirection", RepeatDirection.Vertical); }
			set { SetEnumProperty("RepeatDirection", RepeatDirection.Vertical, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxSiteMapControlBaseSpriteImageUrl"),
#endif
		Category("Images"), DefaultValue(""), Localizable(false), UrlProperty,
		AutoFormatEnable, AutoFormatUrlProperty,
		Editor(typeof(System.Web.UI.Design.UrlEditor), typeof(System.Drawing.Design.UITypeEditor))]
		public string SpriteImageUrl {
			get { return SpriteImageUrlInternal; }
			set { SpriteImageUrlInternal = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxSiteMapControlBaseSpriteCssFilePath"),
#endif
		Category("Images"), DefaultValue(""), Localizable(false), UrlProperty,
		AutoFormatEnable, AutoFormatUrlProperty,
		Editor(typeof(System.Web.UI.Design.UrlEditor), typeof(System.Drawing.Design.UITypeEditor))]
		public string SpriteCssFilePath {
			get { return SpriteCssFilePathInternal; }
			set { SpriteCssFilePathInternal = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxSiteMapControlBasePaddings"),
#endif
		Category("Layout"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public Paddings Paddings {
			get { return (ControlStyle as AppearanceStyle).Paddings; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxSiteMapControlBaseColumnSeparatorStyle"),
#endif
		Category("Styles"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ColumnSeparatorStyle ColumnSeparatorStyle {
			get { return Styles.ColumnSeparator; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxSiteMapControlBaseColumnStyle"),
#endif
		Category("Styles"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ColumnStyle ColumnStyle {
			get { return Styles.Column; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxSiteMapControlBaseLinkStyle"),
#endif
		Category("Styles"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new LinkStyle LinkStyle {
			get { return base.LinkStyle; }
		}
		[Browsable(false), DefaultValue(null), AutoFormatEnable,
		PersistenceMode(PersistenceMode.InnerProperty),
		TemplateContainer(typeof(SiteMapControlColumnSeparatorTemplateContainer)),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual ITemplate ColumnSeparatorTemplate {
			get { return fColumnSeparatorTemplate; }
			set {
				fColumnSeparatorTemplate = value;
				TemplatesChanged();
			}
		}
		[Browsable(false), DefaultValue(null), AutoFormatEnable,
		PersistenceMode(PersistenceMode.InnerProperty),
		TemplateContainer(typeof(NodeTemplateContainer)),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual ITemplate NodeTemplate {
			get { return fNodeTemplate; }
			set {
				fNodeTemplate = value;
				TemplatesChanged();
			}
		}
		[Browsable(false), DefaultValue(null), AutoFormatEnable,
		PersistenceMode(PersistenceMode.InnerProperty),
		TemplateContainer(typeof(NodeTemplateContainer)),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual ITemplate NodeTextTemplate {
			get { return fNodeTextTemplate; }
			set {
				fNodeTextTemplate = value;
				TemplatesChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxSiteMapControlBaseNodeCommand"),
#endif
		Category("Action")]
		public event SiteMapNodeCommandEventHandler NodeCommand
		{
			add
			{
				Events.AddHandler(EventNodeCommand, value);
			}
			remove
			{
				Events.RemoveHandler(EventNodeCommand, value);
			}
		}
		protected internal int ColumnCountActual { 
			get {
				int count = GetColumns().Count;
				if(count < 1)
					count = 1;
				if(Categorized) {
					if(GetMaximumColumnCount() == 0)
						count = 0;
					else
						if((count == 0) || (count > GetMaximumColumnCount()))
							count = GetMaximumColumnCount();
					if(ColumnCount == 0)
						count = GetMaximumColumnCount();
				}
				else
					if(count >= RootNodes.Count)
						count = RootNodes.Count;
				if(FlowLayoutLevel == 0)
					count = 1;
				return count;
			}
		}
		protected SiteMapStyles Styles {
			get { return StylesInternal as SiteMapStyles; }
		}
		public ASPxSiteMapControlBase()
			: base() {
		}
		public Control FindControl(SiteMapNode node, string id) {
			return TemplateContainerBase.FindTemplateControl(this, GetNodeTextTemplateContainerID(node), id)
				?? TemplateContainerBase.FindTemplateControl(this, GetNodeTemplateContainerID(node), id);
		}
		protected override bool OnBubbleEvent(object source, EventArgs e) {
			if(e is SiteMapNodeCommandEventArgs) {
				OnNodeCommand(e as SiteMapNodeCommandEventArgs);
				return true;
			}
			else
				return false;
		}
		protected internal bool HasVisibleNodes() {
			return RootNodes != null && RootNodes.Count != 0;
		}
		protected override IStateManager[] GetStateManagedObjects() {
			return ViewStateUtils.GetMergedStateManagedObjects(base.GetStateManagedObjects(),
				new IStateManager[] { Columns });
		}
		protected override bool HasHoverScripts() {
			if(!Categorized) {
				for(int i = 0; i < GetColumns().Count; i++) {
					if(CanColumnHotTrack(i))
						return true;
				}
			}
			return false;
		}
		protected override void AddHoverItems(StateScriptRenderHelper helper) {
			for(int i = 0; i < GetColumns().Count; i++) {
				if(CanColumnHotTrack(i))
					helper.AddStyle(GetColumnHoverCssStyle(i), GetColumnIDPrefix(i), IsEnabled());
			}
		}
		protected override bool NeedVerifyRenderingInServerForm() {
			return false;
		}
		protected override bool HasContent() {
			return HasVisibleNodes();
		}
		protected override void ClearControlFields() {
			fMainControl = null;
		}
		protected override void CreateControlHierarchy() {
			fMainControl = CreateMainControl();
			Controls.Add(fMainControl);
		}
		protected override void EnsureChildControls() {
			if(fInEnsureChildControls) return;
			fInEnsureChildControls = true;
			try {
				if(!DesignMode && !fInPerformDataBinding)
					EnsureDataBound();
				base.EnsureChildControls();
			}
			finally {
				fInEnsureChildControls = false;
			}
		}
		protected abstract SMCMainControlBase CreateMainControl();
		protected internal string GetColumnIDPrefix(int columnIndex) {
			return "C" + columnIndex;
		}
		protected internal string GetColumnID(int columnIndex, int rowIndex) {
			return GetColumnIDPrefix(columnIndex);
		}
		protected internal string GetNodeIndexPath(SiteMapNode node) {
			SiteMapNode curNode = node;
			string ret = GetNodeIndexInLevel(curNode).ToString();
			curNode = GetParentNode(curNode);
			while(curNode != null) {
				ret = GetNodeIndexInLevel(curNode).ToString() + "_" + ret;
				curNode = GetParentNode(curNode);
			}
			return ret;
		}
		protected internal string GetNodeTemplateContainerID(SiteMapNode node) {
			return "NTC" + "_" + GetNodeIndexPath(node);
		}
		protected internal string GetNodeTextTemplateContainerID(SiteMapNode node) {
			return "NTTC" + "_" + GetNodeIndexPath(node);
		}
		protected bool CanColumnHotTrack(int columnIndex) {
			return !GetColumnHoverStyleInternal(columnIndex).IsEmpty;
		}
		protected internal abstract ITemplate GetNodeTemplate(SiteMapNode node);
		protected internal abstract ITemplate GetNodeTextTemplate(SiteMapNode node);
		private void OnNodeCommand(SiteMapNodeCommandEventArgs e) {
			SiteMapNodeCommandEventHandler handler = (SiteMapNodeCommandEventHandler)Events[EventNodeCommand];
			if(handler != null)
				handler(this, e);
		}
		protected internal void ColumnChanged() {
			if(!IsLoading())
				ResetControlHierarchy();
		}
		protected override Style CreateControlStyle() {
			return new AppearanceStyle();
		}
		protected override StylesBase CreateStyles() {
			return new SiteMapStyles(this);
		}
		protected abstract LevelProperties GetLevelProperties(int level);
		protected abstract DefaultLevelProperties GetDefaultLevelProperties();
		protected Paddings GetColumnPaddings() {
			Paddings paddings = new Paddings();
			paddings.CopyFrom(Styles.GetDefaultColumnStyle().Paddings);
			paddings.CopyFrom(ColumnStyle.Paddings);
			return paddings;
		}
		protected internal Paddings GetColumnPaddings(int columnIndex) {
			Paddings paddings = GetColumnPaddings();
			if(IsValidColumnIndex(columnIndex))
				paddings.CopyFrom(GetColumns()[columnIndex].Paddings);
			return paddings;
		}
		protected AppearanceStyleBase GetColumnHoverStyleInternal(int columnIndex) {
			AppearanceStyleBase style = new AppearanceStyleBase();
			style.CopyFrom(ColumnStyle.HoverStyle);
			if(IsValidColumnIndex(columnIndex))
				style.CopyFrom(GetColumns()[columnIndex].HoverStyle);
			return style;
		}
		protected AppearanceStyleBase GetColumnHoverStyle(int columnIndex) {
			AppearanceStyleBase style = new AppearanceStyleBase();
			style.CopyBordersFrom(GetColumnStyle(columnIndex));
			style.CopyFrom(GetColumnHoverStyleInternal(columnIndex));
			return style;
		}
		protected internal AppearanceStyle GetColumnHoverCssStyle(int columnIndex) {
			AppearanceStyle style = new AppearanceStyle();
			style.CopyFrom(GetColumnHoverStyle(columnIndex));
			style.Paddings.CopyFrom(GetColumnHoverStylePaddings(columnIndex));
			return style;
		}
		protected Paddings GetColumnHoverStylePaddings(int columnIndex) {
			ColumnStyle style = GetColumnStyle(columnIndex);
			Paddings paddings = GetColumnPaddings(columnIndex);
			return UnitUtils.GetSelectedCssStylePaddings(style,
				GetColumnHoverStyle(columnIndex), paddings);
		}
		protected internal Paddings GetColumnSeparatorPaddings() {
			return GetColumnSeparatorStyle().Paddings;
		}
		protected internal ColumnSeparatorStyle GetColumnSeparatorStyle() {
			ColumnSeparatorStyle style = new ColumnSeparatorStyle();
			style.CopyFrom(Styles.GetDefaultColumnSeparatorStyle());
			style.CopyFrom(ColumnSeparatorStyle);
			return style;
		}
		protected internal Unit GetColumnSeparatorWidth() {
			return !ColumnSeparatorStyle.Width.IsEmpty ?
				ColumnSeparatorStyle.Width : Styles.GetDefaultColumnSeparatorStyle().Width;
		}
		protected internal ColumnStyle GetColumnStyle(int columnIndex) {
			ColumnStyle style = new ColumnStyle();
			style.CopyFrom(ColumnStyle);
			if(IsValidColumnIndex(columnIndex))
				style.CopyFrom(GetColumns()[columnIndex].Style);
			MergeDisableStyle(style);
			return style;
		}
		protected double GetColumnsWidth() {
			double columnsWidth = Width.Value;
			if(GetColumnSeparatorWidth().Type == Width.Type)
				columnsWidth -= GetColumnSeparatorWidth().Value * (ColumnCountActual - 1);
			if(GetColumnSeparatorPaddings().GetPaddingLeft().Type == Width.Type)
				columnsWidth -= GetColumnSeparatorPaddings().GetPaddingLeft().Value * (ColumnCountActual - 1);
			if(GetColumnSeparatorPaddings().GetPaddingRight().Type == Width.Type)
				columnsWidth -= GetColumnSeparatorPaddings().GetPaddingRight().Value * (ColumnCountActual - 1);
			if(GetColumnPaddings().GetPaddingLeft().Type == Width.Type)
				columnsWidth -= GetColumnPaddings().GetPaddingLeft().Value;
			if(GetColumnPaddings().GetPaddingRight().Type == Width.Type)
				columnsWidth -= GetColumnPaddings().GetPaddingRight().Value;
			if(GetPaddings().GetPaddingLeft().Type == Width.Type)
				columnsWidth -= GetPaddings().GetPaddingLeft().Value;
			if(GetPaddings().GetPaddingRight().Type == Width.Type)
				columnsWidth -= GetPaddings().GetPaddingRight().Value;
			return columnsWidth;
		}
		protected bool IsColumnsWidthsEmpty() {
			for(int i = 0; i < Columns.Count; i++) {
				if(!Columns[i].Width.IsEmpty)
					return false;
			}
			return true;
		}
		protected internal Unit GetColumnWidth(int columnIndex) {
			Unit columnWidth = Unit.Empty;
			columnWidth = IsValidColumnIndex(columnIndex) ? GetColumns()[columnIndex].Width :
				Unit.Empty;
			bool isEmptyWidth = Width.IsEmpty || Width.Type == UnitType.Percentage;
			double widthValue = isEmptyWidth ? 100 : GetColumnsWidth();
			UnitType widthType = isEmptyWidth ? UnitType.Percentage : Width.Type;
			if(widthValue < 0) 
				return GetActualColumnWidthInternal(Browser.Family.IsNetscape ? 100 / ColumnCountActual + 1 : 100 / ColumnCountActual, UnitType.Percentage);
			if(columnWidth.IsEmpty) {
				if(IsColumnsWidthsEmpty())
					return GetActualColumnWidthInternal(Browser.Family.IsNetscape ? widthValue / ColumnCountActual + 1 : widthValue / ColumnCountActual, widthType);
				if(widthType == UnitType.Percentage)
					return Unit.Empty;
			}
			else {
				if(!Width.IsEmpty) {
					bool canConvert = true;
					double columnsSumWidth = columnWidth.Value;
					for(int i = 0; i < ColumnCountActual; i++) {
						if(columnIndex != i) {
							Unit width = GetColumns()[i].Width;
							if(width.Type != columnWidth.Type) {
								canConvert = false;
								break;
							}
							columnsSumWidth += width.Value;
						}
					}
					if(canConvert) {
						return GetActualColumnWidthInternal(columnWidth.Value * widthValue / columnsSumWidth, widthType);
					}
				}
			}
			return columnWidth;
		}
		protected void SetColumnCount(byte count) {
			while(count < Columns.Count)
				Columns.RemoveAt(Columns.Count - 1);
			while(count > Columns.Count)
				Columns.Add();
			PropertyChanged("Columns");
		}
		protected internal Unit GetBulletIndent(SiteMapNode node) {
			return Styles.GetBulletIndent();
		}
		protected internal NodeBulletStyle GetNodeBulletStyle(SiteMapNode node) {
			NodeBulletStyle ret = GetNodeBulletStyleInternal(GetNodeLevel(node));
			if(ret == NodeBulletStyle.Auto)
				ret = GetNodeBulletStyleAuto(node);
			return ret;
		}
		protected internal NodeBulletStyle GetNodeBulletStyleInternal(int level) {
			NodeBulletStyle bulletStyle = Styles.GetLevelDefaultProperties(level, Categorized,
					RepeatDirection == RepeatDirection.Horizontal, IsFlowLayoutLevel(level)).BulletStyle;
			if(IsValidLevelPropertyIndex(level) &&
				(GetLevelProperties(level).BulletStyle != NodeBulletStyle.NotSet))
				bulletStyle = GetLevelProperties(level).BulletStyle;
			else if(GetDefaultLevelProperties().BulletStyle != NodeBulletStyle.NotSet) {
				bulletStyle = GetDefaultLevelProperties().BulletStyle;
			}
			return bulletStyle;
		}
		protected NodeBulletStyle GetNodeBulletStyleAuto(SiteMapNode node) {
			NodeBulletStyle ret = NodeBulletStyle.NotSet;
			int nodeBulletStyleLevel = GetNodeBulletStyleIndex(node);
			if(nodeBulletStyleLevel < DefaultNodeBulletStyles.Length)
				ret = DefaultNodeBulletStyles[nodeBulletStyleLevel];
			else
				ret = DefaultNodeBulletStyles[DefaultNodeBulletStyles.Length - 1];
			return ret;
		}
		protected int GetNodeBulletStyleIndex(SiteMapNode node) {
			int result = 0;
			int i = GetNodeLevel(node);
			while((i >= 0) && (GetNodeBulletStyleInternal(i) == NodeBulletStyle.Auto)) {
				result++;
				i--;
			}
			return result != 0 ? result - 1 : 0;
		}
		protected internal Unit GetImageSpacing(SiteMapNode node) {
			int level = GetNodeLevel(node);
			if(IsValidLevelPropertyIndex(level) && (!GetLevelProperties(level).ImageSpacing.IsEmpty))
				return GetLevelProperties(level).ImageSpacing;
			else
				return !GetDefaultLevelProperties().ImageSpacing.IsEmpty ?
					GetDefaultLevelProperties().ImageSpacing : Styles.GetLevelDefaultProperties(level,
					Categorized, RepeatDirection == RepeatDirection.Horizontal, IsFlowLayoutLevel(level)).ImageSpacing;
		}
		protected internal virtual ImageProperties GetNodeImage(SiteMapNode node) {
			return new ImageProperties();
		}
		protected internal Paddings GetChildNodesPaddings(int level) {
			Paddings paddings = new Paddings();
			paddings.CopyFrom(Styles.GetLevelDefaultProperties(level, Categorized,
				RepeatDirection == RepeatDirection.Horizontal, IsFlowLayoutLevel(level)).ChildNodesPaddings);
			paddings.CopyFrom(GetDefaultLevelProperties().ChildNodesPaddings);
			if(IsValidLevelPropertyIndex(level))
				paddings.CopyFrom(GetLevelProperties(level).ChildNodesPaddings);
			return paddings;
		}
		protected internal Paddings GetNodeMarginsInternal(SiteMapNode node, bool isFirstInColumn,
			bool isLastNodeInColumn) {
			Paddings margins = new Paddings();
			int level = GetNodeLevel(node);
			if(level > 0) {
				Paddings childPaddings = GetChildNodesPaddings(GetNodeLevel(GetParentNode(node)));
				if(IsRightToLeft()) {
					margins.PaddingRight = childPaddings.GetPaddingLeft();
					margins.PaddingLeft = childPaddings.GetPaddingRight();
				} else {
					margins.PaddingLeft = childPaddings.GetPaddingLeft();
					margins.PaddingRight = childPaddings.GetPaddingRight();
				}
				if(!IsFlowLayoutLevel(level)) {
					if(IsFirstChildNode(node))
						margins.PaddingTop = childPaddings.GetPaddingTop();
					else
						margins.PaddingTop = GetNodeSpacing(level);
					if(IsLatestChildNode(node)) {
						SiteMapNode nextNode = GetNextNode(node);
						if(nextNode != null)
							margins.PaddingBottom = GetChildNodesPaddings(GetNodeLevel(nextNode)).GetPaddingBottom();
						else
							margins.PaddingBottom = GetChildNodesPaddings(0).GetPaddingBottom();
					}
				}
				else {
					margins.PaddingBottom = childPaddings.GetPaddingBottom();
					margins.PaddingTop = childPaddings.GetPaddingTop();
				}
			}
			else {
				if(!IsFirstNode(node))
					margins.PaddingTop = GetNodeSpacing(level);
			}
			if(isFirstInColumn)
				margins.PaddingTop = 0;
			if((isLastNodeInColumn) && (Categorized))
				margins.PaddingBottom = 0;
			return margins;
		}
		protected internal Paddings GetNodePaddings(SiteMapNode node) {
			Paddings paddings = new Paddings();
			int level = GetNodeLevel(node);
			paddings.CopyFrom(Styles.GetLevelDefaultProperties(level, Categorized,
				RepeatDirection == RepeatDirection.Horizontal, IsFlowLayoutLevel(level)).NodePaddings);
			paddings.CopyFrom(GetDefaultLevelProperties().NodePaddings);
			if(IsValidLevelPropertyIndex(level))
				paddings.CopyFrom(GetLevelProperties(level).NodePaddings);
			return paddings;
		}
		protected internal Unit GetNodeSpacing(int level) {
			if(IsValidLevelPropertyIndex(level) && (!GetLevelProperties(level).NodeSpacing.IsEmpty))
				return GetLevelProperties(level).NodeSpacing;
			else
				return !GetDefaultLevelProperties().NodeSpacing.IsEmpty ?
					GetDefaultLevelProperties().NodeSpacing : Styles.GetLevelDefaultProperties(level, Categorized,
					RepeatDirection == RepeatDirection.Horizontal, IsFlowLayoutLevel(level)).NodeSpacing;
		}
		protected AppearanceSelectedStyle GetCurrentNodeStyle(SiteMapNode node) {
			int level = GetNodeLevel(node);
			AppearanceSelectedStyle style = new AppearanceSelectedStyle();
			style.CopyFrom(GetDefaultLevelProperties().CurrentNodeStyle);
			if(IsValidLevelPropertyIndex(level))
				style.CopyFrom(GetLevelProperties(level).CurrentNodeStyle);
			return style;
		}
		protected AppearanceStyleBase GetCustomNodeStyle(int level) {
			AppearanceStyleBase style = new AppearanceStyleBase();
			style.CopyFrom(GetDefaultLevelProperties().Style);
			if(IsValidLevelPropertyIndex(level))
				style.CopyFrom(GetLevelProperties(level).Style);
			return style;
		}
		protected AppearanceStyleBase GetDefaultNodeStyle(int level) {
			return Styles.GetLevelDefaultStyle(level, Categorized,
				RepeatDirection == RepeatDirection.Horizontal, IsFlowLayoutLevel(level));
		}
		protected internal AppearanceStyleBase GetNodeStyle(int level) {
			AppearanceStyleBase style = new AppearanceStyleBase();
			style.CopyFrom(GetDefaultNodeStyle(level));
			style.CopyFrom(GetCustomNodeStyle(level));
			return style;
		}
		protected internal AppearanceStyleBase GetNodeStyle(SiteMapNode node) {
			AppearanceStyleBase style = GetNodeStyle(GetNodeLevel(node));
			if(style.Wrap == DefaultBoolean.Default)
				style.Wrap = Styles.GetDefaultControlStyle().Wrap;
			if(IsCurrentNode(node))
				style.CopyFrom(GetCurrentNodeStyle(node));
			MergeDisableStyle(style);
			return style;
		}
		protected internal LinkStyle GetCustomNodeLinkStyle(SiteMapNode node) {
			return LinkStyle;
		}
		protected internal AppearanceStyleBase GetNodeLinkStyle(SiteMapNode node) {
			AppearanceStyleBase style = new AppearanceStyleBase();
			int level = GetNodeLevel(node);
			style.CopyFontAndCursorFrom(GetDefaultNodeStyle(level));
			style.CopyFontAndCursorFrom(GetCustomNodeStyle(level));
			style.CopyFrom(GetCustomNodeLinkStyle(node).Style);
			MergeDisableStyle(style);
			return style;
		}
		protected internal override bool HasDataInViewState() {
			return false;
		}
		protected internal override void PerformDataBinding(string dataHelperName) {
			fInPerformDataBinding = true;
			try {
				if(!DesignMode && string.IsNullOrEmpty(DataSourceID) && (DataSource == null))
					ClearRootNodes();
				else if(!string.IsNullOrEmpty(DataSourceID) || (DataSource != null)) {
					DataBindNodes();
					ResetControlHierarchy();
				}
			}
			finally {
				fInPerformDataBinding = false;
			}
		}
		protected override DataHelperBase CreateDataHelper(string name) {
			return new SiteMapDataHelper(this, name);
		}
		private void DataBindNodes() {
			HierarchicalDataSourceView view = GetData("");
			if(view != null) {
				IHierarchicalEnumerable enumerable = view.Select();
				fRootNodes = enumerable as SiteMapNodeCollection;
			}
		}
		protected internal void ClearRootNodes() {
			fRootNodes = null;
		}
		protected internal BackgroundImage GetBackgroundImage(SiteMapNode node) {
			int level = GetNodeLevel(node);
			if(IsValidLevelPropertyIndex(level) && (!GetLevelProperties(level).BackgroundImage.IsEmpty))
				return GetLevelProperties(level).BackgroundImage;
			else
				return GetDefaultLevelProperties().BackgroundImage;
		}
		protected internal HorizontalAlign GetNodeHorizontalAlign(SiteMapNode node) {
			HorizontalAlign align = HorizontalAlign.NotSet;
			if(IsValidLevelPropertyIndex(GetNodeLevel(node)))
				align = GetLevelProperties(GetNodeLevel(node)).HorizontalAlign;
			else
				align = GetDefaultLevelProperties().HorizontalAlign;
			return align;
		}
		protected internal string GetNodeUrl(SiteMapNode node) {
			return !IsCurrentNode(node) ? node.Url : "";
		}
		protected internal string GetNodeTarget(SiteMapNode node) {
			string target = "";
			int level = GetNodeLevel(node);
			if(IsValidLevelPropertyIndex(level) && (GetLevelProperties(level).Target != ""))
				target = GetLevelProperties(level).Target;
			else
				target = GetDefaultLevelProperties().Target;
			return target;
		}
		protected internal SiteMapColumnCollection GetColumns() {
			if(Columns.Count != 0)
				return Columns;
			else {
				if(fDummyColumns == null) {
					fDummyColumns = new SiteMapColumnCollection();
					fDummyColumns.Add(new SiteMapColumn());
				}
				return fDummyColumns;
			}
		}
		protected internal int GetNodeIndexInLevel(SiteMapNode node) {
			SiteMapNode parentNode = GetParentNode(node);
			return parentNode != null ? parentNode.ChildNodes.IndexOf(node) : RootNodes.IndexOf(node);
		}
		protected internal virtual int GetNodeLevel(SiteMapNode node) {
			int ret = 0;
			SiteMapNode curNode = node;
			while(!IsRootNode(curNode) && curNode != null) {
				ret++;
				curNode = GetParentNode(curNode);
			}
			return ret;
		}
		protected internal SiteMapNode GetNextNode(SiteMapNode node) {
			SiteMapNode nextNode = null;
			if(HasChildNodes(node))
				nextNode = node.ChildNodes[0];
			else
				nextNode = GetNextSibling(node);
			if(nextNode == null) {
				SiteMapNode curNode = GetParentNode(node);
				while(curNode != null) {
					nextNode = GetNextSibling(curNode);
					if(nextNode != null)
						break;
					else
						curNode = GetParentNode(curNode);
				}
			}
			return ReturnNodeIfDisplayed(nextNode);
		}
		protected internal SiteMapNode GetNextSibling(SiteMapNode node) {
			if((IsRootNode(node)) && (RootNodes.Count == 1))
				return null;
			else
				return node.NextSibling;
		}
		protected internal SiteMapNode GetParentNode(SiteMapNode node) {
			if(IsRootNode(node))
				return null;
			else
				return node != null ? node.ParentNode : null;
		}
		protected SiteMapNode GetPreviousNode(SiteMapNode node) {
			SiteMapNode prevNode = null;
			if(!IsRootNode(node)) {
				SiteMapNode prevSiblNode = GetPreviousSibling(node);
				if(prevSiblNode != null) {
					prevNode = prevSiblNode;
					while(HasChildNodes(prevNode)) {
						prevNode = prevNode.ChildNodes[prevNode.ChildNodes.Count - 1];
					}
				}
				else
					prevNode = GetParentNode(node);
			}
			else
				prevNode = GetPreviousSibling(node);
			return ReturnNodeIfDisplayed(prevNode);
		}
		protected internal SiteMapNode GetPreviousSibling(SiteMapNode node) {
			if((IsRootNode(node)) && (RootNodes.Count == 1))
				return null;
			else
				return node.PreviousSibling;
		}
		protected internal int GetMaximumColumnCount() {
			int max = 0;
			foreach(SiteMapNode node in RootNodes) {
				if(HasChildNodes(node) && (node.ChildNodes.Count > max))
					max = node.ChildNodes.Count;
			}
			return max;
		}
		protected internal bool HasChildNodes(SiteMapNode node) {
			return node.HasChildNodes && IsLevelDisplayed(GetNodeLevel(node.ChildNodes[0]));
		}
		protected internal bool IsBulletMode(SiteMapNode node) {
			if(GetNodeImage(node).IsEmpty) {
				NodeBulletStyle bulletStyle = GetNodeBulletStyle(node);
				return (bulletStyle != NodeBulletStyle.NotSet) &&
					(bulletStyle != NodeBulletStyle.None);
			}
			else
				return false;
		}
		protected virtual bool IsValidLevelPropertyIndex(int index) {
			return true;
		}
		protected internal bool IsCurrentNode(SiteMapNode node) {
			return !DesignMode && GetDataSource() != null && node == node.Provider.CurrentNode;
		}
		protected bool IsFirstChildNode(SiteMapNode node) {
			SiteMapNode parentNode = GetParentNode(node);
			return (parentNode != null) && (parentNode.ChildNodes[0] == node);
		}
		protected internal virtual bool IsFlowLayoutLevel(int level) {
			return false;
		}
		protected bool IsFirstNode(SiteMapNode node) {
			return RootNodes.IndexOf(node) == 0;
		}
		protected internal bool IsLatestChildNode(SiteMapNode node) {
			return !HasChildNodes(node) && (GetParentNode(node) != null) && (GetNextSibling(node) == null);
		}
		protected virtual bool IsLevelDisplayed(int level) {
			return true;
		}
		protected internal bool IsRootNode(SiteMapNode node) {
			return RootNodes.Contains(node);
		}
		protected internal bool IsShowChildNodeAsFlowLayoutItem(SiteMapNode parentNode) {
			int parentNodeLevel = GetNodeLevel(parentNode);
			return IsFlowLayoutLevel(parentNodeLevel + 1);
		}
		protected internal bool IsValidColumnIndex(int index) {
			return (0 <= index) && (index < Columns.Count) ? true : false;
		}
		protected internal bool NeedTableNodeRender(SiteMapNode node) {
			if(!GetNodeImage(node).IsEmpty)
				return Browser.Family.IsNetscape || Browser.Family.IsWebKit || GetNodeStyle(node).Wrap == DefaultBoolean.True;
			else
				return false;
		}
		protected virtual SiteMapNode ReturnNodeIfDisplayed(SiteMapNode node) {
			return node;
		}
		private Unit GetActualColumnWidthInternal(double columnWidth, UnitType unitType) {
			return new Unit(Browser.Family.IsNetscape && unitType == UnitType.Percentage ?
				Math.Floor(columnWidth) : columnWidth, unitType);
		}
		string IControlDesigner.DesignerType { get { return "DevExpress.Web.Design.SiteMapCommonFormDesigner"; } }
	}
	[DXWebToolboxItem(true),
	Designer("DevExpress.Web.Design.ASPxSiteMapControlDesigner, " + AssemblyInfo.SRAssemblyWebDesignFull),
	DevExpress.Utils.ToolboxTabName(AssemblyInfo.DXTabNavigation),
	System.Drawing.ToolboxBitmap(typeof(ToolboxBitmapAccess), ToolboxBitmapAccess.BitmapPath + "ASPxSiteMapControl.bmp")
	]   
	public class ASPxSiteMapControl: ASPxSiteMapControlBase {
		private LevelPropertiesCollection fLevelProperties = null;
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxSiteMapControlDefaultLevelProperties"),
#endif
		NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable]
		public DefaultLevelProperties DefaultLevelProperties {
			get { return Styles.DefaultLevel; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxSiteMapControlFlowLayoutItemSeparator"),
#endif
		NotifyParentProperty(true), DefaultValue(",&nbsp; "), Localizable(false), AutoFormatEnable]
		public string FlowLayoutItemSeparator {
			get { return GetStringProperty("FlowLayoutItemSeparator", ",&nbsp; "); }
			set { SetStringProperty("FlowLayoutItemSeparator", ",&nbsp; ", value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxSiteMapControlLevelProperties"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), MergableProperty(false),
		Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.Drawing.Design.UITypeEditor))]
		public LevelPropertiesCollection LevelProperties {
			get {
				if(fLevelProperties == null)
					fLevelProperties = new LevelPropertiesCollection(this);
				return fLevelProperties;
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxSiteMapControlFlowLayoutMaximumDisplayItems"),
#endif
		Category("Behavior"), NotifyParentProperty(true), DefaultValue(10), AutoFormatDisable]
		public int FlowLayoutMaximumDisplayItems {
			get { return GetIntProperty("FlowLayoutMaximumDisplayItems", 10); }
			set {
				CommonUtils.CheckNegativeValue(value, "FlowLayoutMaximumDisplayItems");
				SetIntProperty("FlowLayoutMaximumDisplayItems", 10, value);
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxSiteMapControlMaximumDisplayLevels"),
#endif
		Category("Behavior"), NotifyParentProperty(true), DefaultValue(0), AutoFormatDisable]
		public int MaximumDisplayLevels {
			get { return GetIntProperty("MaximumDisplayLevels", 0); }
			set {
				CommonUtils.CheckNegativeValue(value, "MaximumDisplayLevels");
				SetIntProperty("MaximumDisplayLevels", 0, value);
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxSiteMapControlRightToLeft"),
#endif
		Category("Layout"), DefaultValue(DefaultBoolean.Default), AutoFormatDisable]
		public DefaultBoolean RightToLeft {
			get { return RightToLeftInternal; }
			set { RightToLeftInternal = value; }
		}
		public ASPxSiteMapControl()
			: base() {
		}
		protected override SMCMainControlBase CreateMainControl() {
			return new SMCMainControl(this);
		}
		protected override DefaultLevelProperties GetDefaultLevelProperties() {
			return DefaultLevelProperties;
		}
		protected override LevelProperties GetLevelProperties(int level) {
			return LevelProperties[level];
		}
		protected internal override ImageProperties GetNodeImage(SiteMapNode node) {
			ImageProperties ret = new ImageProperties();
			int nodeLevel = GetNodeLevel(node);
			if(IsValidLevelPropertyIndex(nodeLevel))
				ret.CopyFrom(GetLevelProperties(nodeLevel).Image);
			ret.MergeWith(GetDefaultLevelProperties().Image);
			if(HasChildNodes(node))
				ret.CopyFrom(GetParentNodeImage(node));
			return ret;
		}
		private ImageProperties GetParentNodeImage(SiteMapNode node) {
			ImageProperties ret = new ImageProperties();
			int nodeLevel = GetNodeLevel(node);
			ret.CopyFrom(GetDefaultLevelProperties().ParentImage);
			if(IsValidLevelPropertyIndex(nodeLevel))
				ret.CopyFrom(GetLevelProperties(nodeLevel).ParentImage);
			return ret;
		}
		protected internal override ITemplate GetNodeTemplate(SiteMapNode node) {
			int level = GetNodeLevel(node);
			if(IsValidLevelPropertyIndex(level))
				return (LevelProperties[level].NodeTemplate != null) ?
					LevelProperties[level].NodeTemplate : NodeTemplate;
			else
				return NodeTemplate;
		}
		protected internal override ITemplate GetNodeTextTemplate(SiteMapNode node) {
			int level = GetNodeLevel(node);
			if(IsValidLevelPropertyIndex(level))
				return (LevelProperties[level].NodeTextTemplate != null) ?
					LevelProperties[level].NodeTextTemplate : NodeTextTemplate;
			else
				return NodeTextTemplate;
		}
		protected override IStateManager[] GetStateManagedObjects() {
			return ViewStateUtils.GetMergedStateManagedObjects(base.GetStateManagedObjects(),
				new IStateManager[] { LevelProperties });
		}
		protected override bool HasHoverScripts() {
			return base.HasHoverScripts() && (RepeatDirection != RepeatDirection.Horizontal);
		}
		protected internal override int GetNodeLevel(SiteMapNode node) {
			int ret = base.GetNodeLevel(node);
			if((ret > FlowLayoutLevel) && (FlowLayoutLevel != -1)) {
				if(Categorized) {
					if(FlowLayoutLevel > 1)
						ret = FlowLayoutLevel;
				}
				else
					ret = FlowLayoutLevel;
			}
			return ret;
		}
		protected internal RepeatDirection GetRepeatDirection() {
			return RepeatDirection;
		}
		protected internal override bool IsFlowLayoutLevel(int level) {
			return (FlowLayoutLevel != -1 && FlowLayoutLevel <= level);
		}
		protected internal string GetFlowLayoutLastItemText() {
			return "&hellip;";
		}
		protected internal int GetFlowLayoutMaximumDisplayItems() {
			return FlowLayoutMaximumDisplayItems;
		}
		protected internal Unit GetFlowLayoutTextLineHeight() {
			return FlowLayoutTextLineHeight.IsEmpty ? Styles.GetFlowLayoutLineTextHeight(FlowLayoutLevel) :
				FlowLayoutTextLineHeight;
		}
		protected internal string GetFlowLayoutItemSeparatorText() {
			return FlowLayoutItemSeparator;
		}
		protected override bool IsLevelDisplayed(int level) {
			if(MaximumDisplayLevels > 0)
				return level < MaximumDisplayLevels;
			else
				return true;
		}
		protected override bool IsValidLevelPropertyIndex(int index) {
			return (0 <= index) && (index < LevelProperties.Count) ? true : false;
		}
		protected override SiteMapNode ReturnNodeIfDisplayed(SiteMapNode node) {
			if(MaximumDisplayLevels > 0) {
				if(node != null && (GetNodeLevel(node) < MaximumDisplayLevels))
					return node;
				else
					return null;
			}
			else
				return node;
		}
	}
	public class SiteMapDataHelper: HierarchicalDataHelper {
		public SiteMapDataHelper(ASPxSiteMapControlBase siteMapControl, string name)
			: base(siteMapControl, name) {
		}
		protected override void ValidateDataSource(object dataSource) {
			if(dataSource != null && !(dataSource is SiteMapNodeCollection) && !(dataSource is SiteMapDataSource))
				throw new InvalidOperationException(StringResources.SiteMapControl_InvalidDataSourceType);
		}
		protected override IHierarchicalDataSource GetValidDataSourceControl(Control dataSourceControl) {
			if(!(dataSourceControl is SiteMapDataSource))
				throw new HttpException(string.Format(StringResources.SiteMapControl_DataSourceIDMustBeSiteMapDataSourceControl, Control.ID, DataSourceID));
			return base.GetValidDataSourceControl(dataSourceControl);
		}
	}
}
