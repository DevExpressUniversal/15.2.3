module __aspxRichEdit {
    export interface ITextBoxIteratorRequestsListener extends IEventListener {
        NotifyNextChunkRequired(subDocument: SubDocument, chunkIndex: number);
    }
}  