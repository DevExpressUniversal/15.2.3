module __aspxRichEdit {
    export class ParagraphStyle extends StyleBase implements ICloneable<ParagraphStyle> {
        parent: ParagraphStyle = null;
        linkedStyle: CharacterStyle = null;
        nextParagraphStyle: ParagraphStyle = null;

        maskedCharacterProperties: MaskedCharacterProperties;
        maskedParagraphProperties: MaskedParagraphProperties;
        tabs: TabProperties;

        autoUpdate: boolean;
        numberingListIndex: number;
        listLevelIndex: number

        constructor(styleName: string, localizedName: string, deleted: boolean, hidden: boolean, semihidden: boolean, isDefault: boolean, maskedCharacterProperties: MaskedCharacterProperties, maskedParagraphProperties: MaskedParagraphProperties, tabs: TabInfo[], autoUpdate: boolean, numberingListIndex: number, listLevelIndex: number) {
            super(styleName, localizedName, deleted, hidden, semihidden, isDefault);
            this.maskedCharacterProperties = maskedCharacterProperties;
            this.maskedParagraphProperties = maskedParagraphProperties;

            this.tabs = new TabProperties();
            for (var i= 0, tab: TabInfo; tab = tabs[i]; i++)
                this.tabs.tabsInfo.push(tab.clone());

            this.autoUpdate = autoUpdate;
            this.numberingListIndex = numberingListIndex;
            this.listLevelIndex = listLevelIndex;
        }
        getNumberingListIndex(): number {
            if(this.numberingListIndex >= 0 || this.numberingListIndex === NumberingList.NoNumberingListIndex || !this.parent)
                return this.numberingListIndex;
            else
                return this.parent.getNumberingListIndex();
        }
        getListLevelIndex(): number {
            if(this.numberingListIndex >= 0)
                return this.numberingListIndex;
            if(this.numberingListIndex === NumberingList.NoNumberingListIndex || !this.parent)
                return 0;
            else
                return this.parent.getListLevelIndex();
        }

        public clone(): ParagraphStyle {
            return new ParagraphStyle(this.styleName, this.localizedName, this.deleted, this.hidden, this.semihidden, this.isDefault, this.maskedCharacterProperties, this.maskedParagraphProperties, this.tabs.tabsInfo, this.autoUpdate, this.numberingListIndex, this.listLevelIndex);
        }
    }

    export class TabProperties implements IEquatable<TabProperties>, ICloneable<TabProperties> {
        public tabsInfo: TabInfo[] = [];

        public clone(): TabProperties {
            var tabProperties = new TabProperties();
            for (var i: number = 0, tab: TabInfo; tab = this.tabsInfo[i]; i++)
                tabProperties.tabsInfo.push(tab.clone());
            return tabProperties;
        }

        public equals(obj: TabProperties): boolean {
            for(var i: number = 0, tab: TabInfo; tab = this.tabsInfo[i]; i++)
                if(!tab.equals(obj.tabsInfo[i]))
                    return false;
            return true;
        }
    }

    export class TabInfo implements IEquatable<TabInfo>, ICloneable<TabInfo>{
        public alignment: TabAlign;
        public leader: TabLeaderType;
        public position: number;
        public deleted: boolean;
        public isDefault: boolean;

        constructor(position: number, alignment: TabAlign, leader: TabLeaderType, deleted: boolean, isDefault: boolean) {
            this.position = position;
            this.alignment = alignment;
            this.leader = leader;
            this.deleted = deleted;
            this.isDefault = isDefault;
        }

        public clone(): TabInfo {
            return new TabInfo(this.position, this.alignment, this.leader, this.deleted, this.isDefault);
        }

        public equals(obj: TabInfo): boolean {
            if(!obj)
                return false;
            return this.alignment == obj.alignment &&
                this.leader == obj.leader &&
                this.position == obj.position &&
                this.deleted == obj.deleted &&
                this.isDefault == obj.isDefault;
        }
    }

    export enum TabLeaderType {
        None,
        Dots,
        MiddleDots,
        Hyphens,
        Underline,
        ThickLine,
        EqualSign
    }
}   