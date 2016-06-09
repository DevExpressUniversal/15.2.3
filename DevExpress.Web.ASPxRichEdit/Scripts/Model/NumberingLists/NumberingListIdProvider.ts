module __aspxRichEdit {
    export class ListIdProviderBase {
        documentModel: DocumentModel;
        constructor(documentModel: DocumentModel) {
            this.documentModel = documentModel;
        }
        getNextId(): number {
            throw new Error(Errors.NotImplemented);
        }
    }

    export class NumberingListIdProvider extends ListIdProviderBase {
        lastId: number = 0;
        getNextId(): number {
            this.lastId++;
            return this.lastId;
        }
    }

    export class AbstractNumberingListIdProvider extends ListIdProviderBase {
        private map: { [id: number]: boolean } = {};
        private getMap(): { [id: number]: boolean } {
            if(!this.map) {
                for(var i = 0, list: AbstractNumberingList; list = this.documentModel.abstractNumberingLists[i]; i++)
                    this.map[list.innerId] = true;
            }
            return this.map;
        }
        getNextId(): number {
            var id: number = -1;
            var map = this.getMap();
            do {
                id = Utils.getRandomInt(0, Number.MAX_VALUE);
            } while(map[id]);
            map[id] = true;
            return id;
        }
    }
}