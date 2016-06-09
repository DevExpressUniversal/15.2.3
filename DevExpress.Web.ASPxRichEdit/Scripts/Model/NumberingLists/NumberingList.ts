module __aspxRichEdit {
    export class NumberingListBase<T extends IListLevel> implements IEquatable<NumberingListBase<T>>, ISupportCopyFrom<NumberingListBase<T>> {
        static NoNumberingListIndex: number = -2;
        static NumberingListNotSettedIndex: number = -1;

        innerId: number = -1;

        constructor(documentModel: DocumentModel, levelCount: number) {
            this.documentModel = documentModel;
            this.initLevels(levelCount);
        }

        levels: T[] = [];
        deleted: boolean;
        documentModel: DocumentModel;

        getId(): number {
            if(this.innerId === -1)
                this.innerId = this.generateNewId();
            return this.innerId;
        }
        generateNewId(): number {
            throw new Error(Errors.NotImplemented);
        }
        initLevels(levelCount: number) {
            for(var i = 0; i < levelCount; i++) {
                var listLevel = this.createLevel(i);
                this.levels.push(listLevel);
            }
        }
        createLevel(index: number): T {
            throw new Error(Errors.NotImplemented);
        }
        getLevelType(listLevelIndex: number): NumberingType {
            if(this.isBulletListLevel(this.levels[listLevelIndex]))
                return NumberingType.Bullet;
            else if(!this.isHybridList())
                return NumberingType.MultiLevel;
            else
                return NumberingType.Simple;
        }
        getListType(): NumberingType {
            if(!this.isHybridList())
                return NumberingType.MultiLevel;
            if(this.isBulletListLevel(this.levels[0]))
                return NumberingType.Bullet;
            else
                return NumberingType.Simple;
        }
        equals(obj: NumberingListBase<T>): boolean {
            for(var i = 0, level: T; level = obj.levels[i]; i++) {
                if(!level.equals(this.levels[i]))
                    return false;
            }
            return true;
        }
        externallyEquals(obj: NumberingListBase<T>): boolean {
            if(this.getListType() !== obj.getListType())
                return false;
            var depth = this.getListType() == NumberingType.MultiLevel ? NumberingListFormPreviewHelper.depth : 1;
            for (var i = 0; i < depth; i++) {
                if(!this.levels[i].externallyEquals(obj.levels[i]))
                    return false;
            }
            return true;
        }
        copyFrom(obj: NumberingListBase<T>) {
            this.innerId = obj.innerId;
            this.copyLevelsFrom(obj.levels);
        }
        copyLevelsFrom(levels: T[]) {
            throw new Error(Errors.NotImplemented);
        }

        private isHybridList(): boolean {
            for(var i = 0, listLevel: T; listLevel = this.levels[i]; i++) {
                if(listLevel.getListLevelProperties().templateCode !== 0)
                    return true;
            }
            return false;
        }
        private isBulletListLevel(level: T) {
            return level.getListLevelProperties().displayFormatString.length === 1;
        }
    }

    export class AbstractNumberingList extends NumberingListBase<ListLevel> {
        constructor(documentModel: DocumentModel) {
            super(documentModel, 9);
        }
        generateNewId(): number {
            return this.documentModel.abstractNumberingListsIdProvider.getNextId();
        }
        createLevel(index: number): ListLevel {
            var characterProperties = this.documentModel.defaultCharacterProperties.clone();
            characterProperties.useValue = 0;
            var paragraphProperties = this.documentModel.defaultParagraphProperties.clone();
            paragraphProperties.useValue = 0;
            return new ListLevel(this.documentModel, characterProperties, paragraphProperties, new ListLevelProperties());
        }
        copyLevelsFrom(levels: ListLevel[]) {
            for(var i = 0, level: ListLevel; level = this.levels[i]; i++) {
                level.copyFrom(levels[i]);
            }
        }
    }

    export class NumberingList extends NumberingListBase<IOverrideListLevel> {
        abstractNumberingListIndex: number;
        constructor(documentModel: DocumentModel, abstractNumberingListIndex: number) {
            super(documentModel, 9);
            if(abstractNumberingListIndex < 0 || abstractNumberingListIndex >= documentModel.abstractNumberingLists.length)
                throw "abstractNumberingListIndex should be positive and less than length of the abstractNumberingLists array";
            this.abstractNumberingListIndex = abstractNumberingListIndex;
        }

        getAbstractNumberingList(): AbstractNumberingList {
            return this.documentModel.abstractNumberingLists[this.abstractNumberingListIndex];
        }
        generateNewId(): number {
            return this.documentModel.numberingListsIdProvider.getNextId();
        }

        createLevel(index: number): IOverrideListLevel {
            return new NumberingListReferenceLevel(this, index);
        }

        copyLevelsFrom(levels: IOverrideListLevel[]) {
            for(var i = 0, level: IOverrideListLevel; level = this.levels[i]; i++) {
                var sourceLevel = levels[i];
                if(this.levels[i].constructor !== sourceLevel.constructor) {
                    if(sourceLevel instanceof OverrideListLevel)
                        this.levels[i] = new OverrideListLevel(this.documentModel, sourceLevel.getCharacterProperties(), sourceLevel.getParagraphProperties(), sourceLevel.getListLevelProperties());
                    else
                        this.levels[i] = new NumberingListReferenceLevel(this, i);
                }
                this.levels[i].copyFrom(sourceLevel);
            }
        }
    }

    export enum NumberingType {
        MultiLevel,
        Simple,
        Bullet
    }
}