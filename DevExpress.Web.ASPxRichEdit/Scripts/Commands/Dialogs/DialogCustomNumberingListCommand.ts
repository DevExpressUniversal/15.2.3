module __aspxRichEdit {
    export class DialogCustomNumberingListCommand extends ShowDialogCommandBase {
        listType: NumberingType;

        createParameters(parameters: DialogCustomNumberingListParameters): DialogCustomNumberingListParameters {
            this.listType = parameters.listType;
            return parameters;
        }
        applyParameters(state: IntervalCommandState, newParams: DialogCustomNumberingListParameters) {
            var modelManipulator: ModelManipulator = this.control.modelManipulator;
            var activeSubDocument: SubDocument = modelManipulator.model.activeSubDocument;
            var history: IHistory = this.control.history;

            var initParams: DialogCustomNumberingListParameters = new DialogCustomNumberingListParameters();
            initParams.init(newParams.initAbstractNumberingList);
            if(initParams.equals(newParams)) {
                this.control.commandManager.getCommand(RichEditClientCommand.InsertNumerationToParagraphs).execute(newParams.initAbstractNumberingList);
                return;
            }

            history.beginTransaction();

            var abstractNumberingList = new AbstractNumberingList(this.control.model);
            abstractNumberingList.copyFrom(newParams.initAbstractNumberingList);
            history.addAndRedo(new AddAbstractNumberingListHistoryItem(this.control.modelManipulator, this.control.model.activeSubDocument, abstractNumberingList));
            var abstractNumberingListIndex = this.control.model.abstractNumberingLists.length - 1;

            for(var i = 0, length = newParams.levels.length; i < length; i++) {
                var level = newParams.levels[i];
                var initLevel = initParams.levels[i];
                if(level.displayFormatString != initLevel.displayFormatString)
                    history.addAndRedo(new ListLevelDisplayFormatStringHistoryItem(modelManipulator, activeSubDocument, true, abstractNumberingListIndex, i, level.displayFormatString));
                if(level.format != initLevel.format)
                    history.addAndRedo(new ListLevelFormatHistoryItem(modelManipulator, activeSubDocument, true, abstractNumberingListIndex, i, level.format));
                if(level.start != initLevel.start)
                    history.addAndRedo(new ListLevelStartHistoryItem(modelManipulator, activeSubDocument, true, abstractNumberingListIndex, i, level.start));
                if(level.alignment != initLevel.alignment)
                    history.addAndRedo(new ListLevelAlignmentHistoryItem(modelManipulator, activeSubDocument, true, abstractNumberingListIndex, i, level.alignment));
                if(level.separator != initLevel.separator)
                    history.addAndRedo(new ListLevelSeparatorHistoryItem(modelManipulator, activeSubDocument, true, abstractNumberingListIndex, i, level.separator));

                if(level.leftIndent != initLevel.leftIndent)
                    history.addAndRedo(new ListLevelParagraphLeftIndentHistoryItem(modelManipulator, activeSubDocument, true, abstractNumberingListIndex, i, level.leftIndent, true));
                if(level.firstLineIndent != initLevel.firstLineIndent)
                    history.addAndRedo(new ListLevelParagraphFirstLineIndentHistoryItem(modelManipulator, activeSubDocument, true, abstractNumberingListIndex, i, level.firstLineIndent, true));
                if(level.firstLineIndentType != initLevel.firstLineIndentType)
                    history.addAndRedo(new ListLevelParagraphFirstLineIndentTypeHistoryItem(modelManipulator, activeSubDocument, true, abstractNumberingListIndex, i, level.firstLineIndentType, true));

                var fontName = this.control.model.cache.fontInfoCache.findItem((fi: FontInfo) => { return fi.name == level.fontName; });
                var initFontName = this.control.model.cache.fontInfoCache.findItem((fi: FontInfo) => { return fi.name == initLevel.fontName; });
                if(fontName != initFontName)
                    history.addAndRedo(new ListLevelFontNameHistoryItem(modelManipulator, activeSubDocument, true, abstractNumberingListIndex, i, fontName, true));
                if(ColorHelper.hashToColor(level.fontColor) != ColorHelper.hashToColor(initLevel.fontColor))
                    history.addAndRedo(new ListLevelFontForeColorHistoryItem(modelManipulator, activeSubDocument, true, abstractNumberingListIndex, i, ColorHelper.hashToColor(level.fontColor), true));
                if(level.fontSize != initLevel.fontSize)
                    history.addAndRedo(new ListLevelFontSizeHistoryItem(modelManipulator, activeSubDocument, true, abstractNumberingListIndex, i, level.fontSize, true));
                var bold = !!(level.fontStyle & 1);
                var initBold = !!(initLevel.fontStyle & 1);
                if(bold != initBold)
                    history.addAndRedo(new ListLevelFontBoldHistoryItem(modelManipulator, activeSubDocument, true, abstractNumberingListIndex, i, bold, true));
                var italic = !!(level.fontStyle & 2);
                var initItalic = !!(initLevel.fontStyle & 2);
                if(italic != initItalic)
                    history.addAndRedo(new ListLevelFontItalicHistoryItem(modelManipulator, activeSubDocument, true, abstractNumberingListIndex, i, italic, true));
            }
            this.control.commandManager.getCommand(RichEditClientCommand.InsertNumerationToParagraphs).execute(abstractNumberingList);

            history.endTransaction();
        }
        getDialogName() {
            switch(this.listType) {
                case NumberingType.Bullet:
                    return "BulletedList";
                case NumberingType.Simple:
                    return "SimpleNumberingList";
                case NumberingType.MultiLevel:
                    return "MultiLevelNumberingList";
            }
            return null;
        }
    }

    export class DialogCustomNumberingListParameters extends DialogParametersBase {
        currentLevel: number = 0;
        listType: NumberingType;
        levels: CustomListlevel[] = [];
        initAbstractNumberingList: AbstractNumberingList;

        init(abstractNumberingList?: AbstractNumberingList, currentLevel?: number) {
            if(currentLevel != null)
                this.currentLevel = currentLevel;
            if(abstractNumberingList != null) {
                this.listType = abstractNumberingList.getListType();
                this.initAbstractNumberingList = abstractNumberingList;

                for(var i = 0; i < abstractNumberingList.levels.length; i++) {
                    var level = new CustomListlevel();

                    var listLevelProperties = abstractNumberingList.levels[i].getListLevelProperties();
                    level.displayFormatString = listLevelProperties.displayFormatString;
                    level.format = listLevelProperties.format;
                    level.start = listLevelProperties.start;
                    level.alignment = listLevelProperties.alignment;
                    level.separator = listLevelProperties.separator;

                    var paragraphProperties = abstractNumberingList.levels[i].getParagraphProperties();
                    level.leftIndent = paragraphProperties.leftIndent;
                    level.firstLineIndent = paragraphProperties.firstLineIndent;
                    level.firstLineIndentType = paragraphProperties.firstLineIndentType;

                    var characterProperties = abstractNumberingList.levels[i].getCharacterProperties();
                    level.fontName = characterProperties.fontInfo.name;
                    level.fontColor = ColorHelper.colorToHash(characterProperties.foreColor);
                    level.fontSize = characterProperties.fontSize;
                    level.fontStyle = (characterProperties.fontBold ? 1 : 0) | (characterProperties.fontItalic ? 2 : 0);

                    this.levels.push(level);
                }
            }
        }

        getNewInstance(): DialogParametersBase {
            return new DialogCustomNumberingListParameters();
        }

        toAnotherMeasuringSystem(converterFunc: (val: any) => any) {
        }

        copyFrom(obj: DialogCustomNumberingListParameters) {
            this.currentLevel = obj.currentLevel;
            this.listType = obj.listType;
            this.copyLevelsFrom(obj.levels);
        }
        copyLevelsFrom(levels: CustomListlevel[]) {
            this.levels = [];
            for(var i = 0, length = levels.length; i < length; i++) {
                var level = new CustomListlevel();
                level.copyFrom(levels[i]);
                this.levels.push(level);
            }
        }
        equals(obj: DialogCustomNumberingListParameters): boolean {
            for(var i = 0, level: CustomListlevel; level = obj.levels[i]; i++) {
                if(!level.equals(this.levels[i]))
                    return false;
            }
            return true;
        }
    }

    export class CustomListlevel implements ISupportCopyFrom <CustomListlevel> {
        displayFormatString: string;
        format: NumberingFormat;
        start: number;
        alignment: ListNumberAlignment;
        separator: string;
        leftIndent: number;
        firstLineIndent: number;
        firstLineIndentType: ParagraphFirstLineIndent;

        fontName: string;
        fontColor: string;
        fontSize: number;
        fontStyle: number;

        copyFrom(obj: CustomListlevel) {
            this.displayFormatString = obj.displayFormatString;
            this.format = obj.format;
            this.start = obj.start;
            this.alignment = obj.alignment;
            this.separator = obj.separator;
            this.leftIndent = obj.leftIndent;
            this.firstLineIndent = obj.firstLineIndent;
            this.firstLineIndentType = obj.firstLineIndentType;
            this.fontName = obj.fontName;
            this.fontColor = obj.fontColor;
            this.fontSize = obj.fontSize;
            this.fontStyle = obj.fontStyle;
        }
        equals(obj: CustomListlevel): boolean {
            return this.displayFormatString == obj.displayFormatString &&
                this.format == obj.format &&
                this.start == obj.start &&
                this.alignment == obj.alignment &&
                this.separator == obj.separator &&
                this.leftIndent == obj.leftIndent &&
                this.firstLineIndent == obj.firstLineIndent &&
                this.firstLineIndentType == obj.firstLineIndentType &&
                this.fontName == obj.fontName &&
                this.fontColor == obj.fontColor &&
                this.fontSize == obj.fontSize &&
                this.fontStyle == obj.fontStyle;
        }
    }

    export class NumberingListFormPreviewHelper {
        private abstractNumberingList: AbstractNumberingList;
        private richEdit: RichEditCore;

        static depth = 3;
        constructor(richEdit: RichEditCore, abstractNumberingList: AbstractNumberingList) {
            this.richEdit = richEdit;
            this.abstractNumberingList = abstractNumberingList;
        }
        createPreview(): HTMLElement {
            var preview = document.createElement("div");
            for(var i = 0; i < 4; i++)
                preview.appendChild(this.createRowElement(i));
            return preview;
        }
        // TODO refactor it!
        private createRowElement(index): HTMLElement {
            var separatorWidth = 7;
            var rowHeight = 25;
            var margin = 10;
            var foreColor = 0xbbbbbbbb;
            var fakeString = "▬▬▬▬▬▬▬▬▬";

            var isMultiLevel = this.abstractNumberingList.getListType() == NumberingType.MultiLevel;
            var currentLevelIndex = isMultiLevel ? index % NumberingListFormPreviewHelper.depth : 0;
            var currentMajorIndex = isMultiLevel ? Math.floor(index / NumberingListFormPreviewHelper.depth) : index;

            var twipsUnitConverter = new TwipsUnitConverter();
            var paragraphProperties = this.abstractNumberingList.levels[currentLevelIndex].getParagraphProperties();
            var characterProperties = this.abstractNumberingList.levels[currentLevelIndex].getCharacterProperties();

            var listBoxText = this.getNumberingListBoxText(currentLevelIndex, currentMajorIndex);
            var layoutNumberingListBox = new LayoutNumberingListBox(characterProperties, listBoxText, "");
            LayoutBox.initializeWithMeasurer([layoutNumberingListBox.textBox], this.richEdit.measurer, false);

            var textBoxCharacterProperties = this.richEdit.model.defaultCharacterProperties.clone();
            textBoxCharacterProperties.foreColor = foreColor;
            var layoutTextBox = new LayoutTextBox(textBoxCharacterProperties, fakeString);
            LayoutBox.initializeWithMeasurer([layoutTextBox], this.richEdit.measurer, false);
            layoutTextBox.x = layoutNumberingListBox.textBox.width + separatorWidth;
            
            var layoutRow = new LayoutRow();
            layoutRow.numberingListBox = layoutNumberingListBox;
            layoutRow.boxes.push(layoutTextBox);
            layoutRow.height = Math.max(layoutNumberingListBox.textBox.height, rowHeight);
            layoutRow.width = layoutNumberingListBox.textBox.width + layoutTextBox.width;
            layoutRow.x = isMultiLevel ? twipsUnitConverter.toPixels(paragraphProperties.leftIndent - paragraphProperties.firstLineIndent) + margin : margin;
            layoutRow.y = index * layoutRow.height + margin;

            return this.richEdit.getDocumentRenderer().renderRow(layoutRow);
        }
        private getNumberingListBoxText(levelIndex, majorIndex): string {
            var items = [];
            for(var j = 0; j <= levelIndex; j++) {
                var listLevelProperties = this.abstractNumberingList.levels[j].getListLevelProperties();
                var converter = OrdinalBasedNumberConverter.createConverter(listLevelProperties.format);
                items.push(converter.convertNumber(listLevelProperties.start + majorIndex));
            }
            var displayFormatString = this.abstractNumberingList.levels[levelIndex].getListLevelProperties().displayFormatString;
            return ASPx.Formatter.Format(displayFormatString, items);
        }
    }
}