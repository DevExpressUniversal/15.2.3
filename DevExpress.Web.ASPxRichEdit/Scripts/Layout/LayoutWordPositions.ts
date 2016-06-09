module __aspxRichEdit {
    export class LayoutWordPositionHelper {
        public static getNextWordStartPosition(layout: DocumentLayout, subDocument: SubDocument, selection: Selection, startPosition: number, withSpacesInTheEndPrevWord: boolean): number {
            let endPosition = (subDocument.isMain() ? layout.getLastValidPage() : layout.pages[selection.pageIndex]).getEndPosition(subDocument);
            let boxIterator: LayoutBoxIteratorBase = subDocument.isMain() ? new LayoutBoxIteratorMainSubDocument(subDocument, layout, startPosition, endPosition) : new LayoutBoxIteratorOtherSubDocument(subDocument, layout, startPosition, endPosition, selection.pageIndex);
            if(!boxIterator.isInitialized())
                return -1;

            var findDiffersGroup: boolean = false;
            var stopOnNextPosition: boolean = false;
            var groupMask: NextPrevWordGroupMask = NextPrevWordGroupMask.NoOne;
            var previousGroupDiffersFromCurrent = function (mask: NextPrevWordGroupMask): boolean {
                groupMask |= mask;
                return (groupMask & ~mask) != NextPrevWordGroupMask.NoOne;
            }

            while (boxIterator.moveNext(new LayoutPositionCreatorConflictFlags().setDefault(false), new LayoutPositionCreatorConflictFlags().setDefault(false))) {
                if (stopOnNextPosition)
                    break;
                switch (boxIterator.position.box.getType()) {
                    case LayoutBoxType.Text:
                        var currentBoxText: string = boxIterator.position.box.renderGetContent(null, null, -1);
                        for (var character: string, charIndex: number = boxIterator.position.charOffset; character = currentBoxText[charIndex]; charIndex++) {
                            switch (character) {
                                case Utils.specialCharacters.QmSpace: case Utils.specialCharacters.EmSpace: case Utils.specialCharacters.EnSpace: case Utils.specialCharacters.NonBreakingSpace:
                                    // here symbols what we ignore
                                    if (withSpacesInTheEndPrevWord)
                                        previousGroupDiffersFromCurrent(NextPrevWordGroupMask.Space);  // this and next function differs at least in THIS ROW!
                                    else
                                        findDiffersGroup = previousGroupDiffersFromCurrent(NextPrevWordGroupMask.Space);
                                    break;
                                case Utils.specialCharacters.LeftSingleQuote:
                                    findDiffersGroup = previousGroupDiffersFromCurrent(NextPrevWordGroupMask.LeftSingleQuote); break;
                                //case Utils.specialCharacters.RightSingleQuote:
                                //    findDiffersGroup = previousGroupDiffersFromCurrent(NextPrevWordGroupMask.RightSingleQuote); break;
                                //case "'":
                                //findDiffersGroup = previousGroupDiffersFromCurrent(NextPrevWordGroupMask.SingleQuote); break;

                                case Utils.specialCharacters.LeftDoubleQuote:
                                    findDiffersGroup = previousGroupDiffersFromCurrent(NextPrevWordGroupMask.LeftDoubleQuote); break;
                                case Utils.specialCharacters.RightDoubleQuote:
                                    findDiffersGroup = previousGroupDiffersFromCurrent(NextPrevWordGroupMask.RightDoubleQuote); break;
                                case '"':
                                    findDiffersGroup = previousGroupDiffersFromCurrent(NextPrevWordGroupMask.DoubleQuote); break;

                                case "(": case ")": case "«": case "»": case "<": case ">": case "/":
                                case "№": case "%": case "!": case ":": case "?": case ";": case "|":
                                case "+": case ",": case ".": case "*": case "=": case "\\":
                                    findDiffersGroup = previousGroupDiffersFromCurrent(NextPrevWordGroupMask.PunctuationMark); break;
                                default:
                                    findDiffersGroup = previousGroupDiffersFromCurrent(NextPrevWordGroupMask.Others); break;
                            }
                            if (findDiffersGroup)
                                return boxIterator.position.getLogPosition(DocumentLayoutDetailsLevel.Box) + charIndex;
                        }
                        break;
                    case LayoutBoxType.Space:
                        if (withSpacesInTheEndPrevWord)
                            previousGroupDiffersFromCurrent(NextPrevWordGroupMask.Space);
                        else
                            findDiffersGroup = previousGroupDiffersFromCurrent(NextPrevWordGroupMask.Space);
                        break;
                    case LayoutBoxType.Dash:
                        findDiffersGroup = previousGroupDiffersFromCurrent(NextPrevWordGroupMask.PunctuationMark);
                        break;
                    case LayoutBoxType.Picture:
                    case LayoutBoxType.ParagraphMark:
                    case LayoutBoxType.PageBreak:
                    case LayoutBoxType.ColumnBreak:
                    case LayoutBoxType.SectionMark:
                    case LayoutBoxType.TabSpace:
                    case LayoutBoxType.LineBreak:
                    case LayoutBoxType.NumberingList:
                    default:
                        findDiffersGroup = previousGroupDiffersFromCurrent(NextPrevWordGroupMask.DiffersFromAll);
                        stopOnNextPosition = true;
                        break;
                }
                if (findDiffersGroup)
                    break;
            }

            var finalResult: number = boxIterator.position.getLogPosition();
            if (finalResult == subDocument.getDocumentEndPosition())
                finalResult--;

            return finalResult;
        }

        // call it from double click as @pos@
        // call it from command prev word as @pos - 1@
        // [word]{space}[word]{parMark}
        //                    ^ it start and it return
        public static getPrevWordEndPosition(layout: DocumentLayout, subDocument: SubDocument, selection: Selection, startPosition: number): number {
            if (layout.validPageCount < 1)
                return -1;

            var firstPagePosition: number = layout.pages[0].contentIntervals[0].start;
            if (startPosition <= firstPagePosition)
                return firstPagePosition;

            let boxIterator: LayoutBoxIteratorBase = subDocument.isMain() ? new LayoutBoxIteratorMainSubDocument(subDocument, layout, firstPagePosition, startPosition) : new LayoutBoxIteratorOtherSubDocument(subDocument, layout, firstPagePosition, startPosition, selection.pageIndex);
            if(!boxIterator.isInitialized())
                return -1;

            var endPosLastValigPage: number = (subDocument.isMain() ? layout.getLastValidPage() : layout.pages[selection.pageIndex]).getEndPosition(subDocument);
            if (startPosition >= endPosLastValigPage)
                return endPosLastValigPage;

            var findDiffersGroup: boolean = false;
            var groupMask: NextPrevWordGroupMask = NextPrevWordGroupMask.NoOne;
            var previousGroupDiffersFromCurrent = function (mask: NextPrevWordGroupMask): boolean {
                groupMask |= mask;
                return (groupMask & ~mask & ~NextPrevWordGroupMask.Space) != NextPrevWordGroupMask.NoOne;
            }

            var iterationCount: number = 0;
            var isFirstPositionBeforeFirstBoxInRow: boolean = false;
            while (boxIterator.movePrev(new LayoutPositionCreatorConflictFlags().setDefault(false), new LayoutPositionCreatorConflictFlags().setDefault(false))) {
                if(iterationCount == 0 && boxIterator.position.box.getLength() == boxIterator.position.charOffset) { // for cases when between rows hidden run and startPosition < hiddenRun.endPos
                    const tmpBoxIterator: LayoutBoxIteratorBase = subDocument.isMain() ? new LayoutBoxIteratorMainSubDocument(subDocument, layout, boxIterator.position.getLogPosition(), endPosLastValigPage) : new LayoutBoxIteratorOtherSubDocument(subDocument, layout, boxIterator.position.getLogPosition(), endPosLastValigPage, selection.pageIndex);
                    tmpBoxIterator.moveNext(new LayoutPositionCreatorConflictFlags().setDefault(false), new LayoutPositionCreatorConflictFlags().setDefault(true));
                    boxIterator.resetToInterval(firstPagePosition, tmpBoxIterator.position.getLogPosition())
                    isFirstPositionBeforeFirstBoxInRow = true;
                    continue;
                }
                iterationCount++;
                switch (boxIterator.position.box.getType()) {
                    case LayoutBoxType.Text:
                        var currPosition: number = boxIterator.position.getLogPosition();
                        var charIndex: number;
                        if (iterationCount == 1 && boxIterator.position.charOffset != boxIterator.position.box.getLength())
                            charIndex = boxIterator.position.charOffset;
                        else
                            charIndex = boxIterator.position.box.getLength() - 1;

                        var currentBoxText: string = boxIterator.position.box.renderGetContent(null, null, -1);
                        for (var character: string; character = currentBoxText[charIndex]; charIndex--) {
                            switch (character) {
                                case Utils.specialCharacters.QmSpace: case Utils.specialCharacters.EmSpace: case Utils.specialCharacters.EnSpace: case Utils.specialCharacters.NonBreakingSpace:
                                    findDiffersGroup = previousGroupDiffersFromCurrent(NextPrevWordGroupMask.Space); break;

                                case Utils.specialCharacters.LeftSingleQuote:
                                    findDiffersGroup = previousGroupDiffersFromCurrent(NextPrevWordGroupMask.LeftSingleQuote); break;
                                //case Utils.specialCharacters.RightSingleQuote:
                                //    findDiffersGroup = previousGroupDiffersFromCurrent(NextPrevWordGroupMask.RightSingleQuote); break;
                                //case "'":
                                //findDiffersGroup = previousGroupDiffersFromCurrent(NextPrevWordGroupMask.SingleQuote); break;

                                case Utils.specialCharacters.LeftDoubleQuote:
                                    findDiffersGroup = previousGroupDiffersFromCurrent(NextPrevWordGroupMask.LeftDoubleQuote); break;
                                case Utils.specialCharacters.RightDoubleQuote:
                                    findDiffersGroup = previousGroupDiffersFromCurrent(NextPrevWordGroupMask.RightDoubleQuote); break;
                                case '"':
                                    findDiffersGroup = previousGroupDiffersFromCurrent(NextPrevWordGroupMask.DoubleQuote); break;

                                case "(": case ")": case "«": case "»": case "<": case ">": case "/":
                                case "№": case "%": case "!": case ":": case "?": case ";": case "|":
                                case "+": case ",": case ".": case "*": case "=": case "\\":
                                    findDiffersGroup = previousGroupDiffersFromCurrent(NextPrevWordGroupMask.PunctuationMark); break;
                                default:
                                    findDiffersGroup = previousGroupDiffersFromCurrent(NextPrevWordGroupMask.Others); break;
                            }
                            if (findDiffersGroup)
                                return boxIterator.position.getLogPosition(DocumentLayoutDetailsLevel.Box) + charIndex + 1;
                        }
                        break;
                    case LayoutBoxType.Space:
                        findDiffersGroup = previousGroupDiffersFromCurrent(NextPrevWordGroupMask.Space);
                        break;
                    case LayoutBoxType.Dash:
                        findDiffersGroup = previousGroupDiffersFromCurrent(NextPrevWordGroupMask.PunctuationMark);
                        break;
                    case LayoutBoxType.Picture:
                    case LayoutBoxType.TabSpace:
                    case LayoutBoxType.ParagraphMark:
                    case LayoutBoxType.PageBreak:
                    case LayoutBoxType.ColumnBreak:
                    case LayoutBoxType.SectionMark:
                    case LayoutBoxType.LineBreak:
                    case LayoutBoxType.NumberingList:
                    default:
                        if (iterationCount == 1 || iterationCount == 2 && isFirstPositionBeforeFirstBoxInRow)
                            return Field.correctWhenPositionInStartCode(subDocument.fields, boxIterator.position.getLogPosition());
                        else
                            findDiffersGroup = true;
                }
                if (findDiffersGroup)
                    return Field.correctWhenPositionInStartCode(subDocument.fields, boxIterator.position.getLogPosition() + boxIterator.position.box.getLength());
            }
            return Field.correctWhenPositionInStartCode(subDocument.fields, boxIterator.position.getLogPosition());
        }
    }

    enum NextPrevWordGroupMask {
        NoOne = 0x00000000,
        Space = 0x00000001,

        //SingleQuote = 0x00000002, // deleted. No need separate
        LeftSingleQuote = 0x00000004,
        RightSingleQuote = 0x00000008,

        LeftDoubleQuote = 0x00000010,
        RightDoubleQuote = 0x00000020,
        DoubleQuote = 0x00000040,

        PunctuationMark = 0x00000080,
        DiffersFromAll = 0x00000100,
        Others = 0x00000200,
    }
}