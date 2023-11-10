import { Component, Input, Output, EventEmitter } from '@angular/core'
import { DocumentNotesService } from 'src/app/services/rest/document-notes.service'
import { PaperlessDocumentNote } from 'src/app/data/paperless-document-note'
import { FormControl, FormGroup } from '@angular/forms'
import { ToastService } from 'src/app/services/toast.service'
import { ComponentWithPermissions } from '../with-permissions/with-permissions.component'
import { UserService } from 'src/app/services/rest/user.service'
import { PaperlessUser } from 'src/app/data/paperless-user'

@Component({
  selector: 'pngx-document-notes',
  templateUrl: './document-notes.component.html',
  styleUrls: ['./document-notes.component.scss'],
})
export class DocumentNotesComponent extends ComponentWithPermissions {
  noteForm: FormGroup = new FormGroup({
    newNote: new FormControl(''),
  })

  networkActive = false
  newNoteError: boolean = false

  @Input()
  documentId: number

  @Input()
  notes: PaperlessDocumentNote[] = []

  @Input()
  addDisabled: boolean = false

  @Output()
  updated: EventEmitter<PaperlessDocumentNote[]> = new EventEmitter()
  users: PaperlessUser[]

  constructor(
    private notesService: DocumentNotesService,
    private toastService: ToastService,
    private usersService: UserService
  ) {
    super()
    this.usersService.listAll().subscribe({
      next: (users) => {
        this.users = users.results
      },
    })
  }

  addNote() {
    const note: string = this.noteForm.get('newNote').value.toString().trim()
    if (note.length == 0) {
      this.newNoteError = true
      return
    }
    this.newNoteError = false
    this.networkActive = true
    this.notesService.addNote(this.documentId, note).subscribe({
      next: (result) => {
        this.notes = result
        this.noteForm.get('newNote').reset()
        this.networkActive = false
        this.updated.emit(this.notes)
      },
      error: (e) => {
        this.networkActive = false
        this.toastService.showError($localize`Error saving note`, e)
      },
    })
  }

  deleteNote(noteId: number) {
    this.notesService.deleteNote(this.documentId, noteId).subscribe({
      next: (result) => {
        this.notes = result
        this.networkActive = false
        this.updated.emit(this.notes)
      },
      error: (e) => {
        this.networkActive = false
        this.toastService.showError($localize`Error deleting note`, e)
      },
    })
  }

  displayName(note: PaperlessDocumentNote): string {
    if (!note.user) return ''
    const user = this.users?.find((u) => u.id === note.user)
    if (!user) return ''
    const nameComponents = []
    if (user.first_name) nameComponents.push(user.first_name)
    if (user.last_name) nameComponents.push(user.last_name)
    if (user.username) {
      if (nameComponents.length > 0) nameComponents.push(`(${user.username})`)
      else nameComponents.push(user.username)
    }
    return nameComponents.join(' ')
  }

  noteFormKeydown(event: KeyboardEvent) {
    if ((event.metaKey || event.ctrlKey) && event.key === 'Enter') {
      this.addNote()
    }
  }
}
