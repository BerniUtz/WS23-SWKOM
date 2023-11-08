import { Component, OnInit, OnDestroy, ViewChild } from '@angular/core'
import { FormControl, FormGroup } from '@angular/forms'
import { ActivatedRoute, Router } from '@angular/router'
import {
  NgbDateStruct,
  NgbModal,
  NgbNav,
  NgbNavChangeEvent,
} from '@ng-bootstrap/ng-bootstrap'
import { PaperlessCorrespondent } from 'src/app/data/paperless-correspondent'
import { PaperlessDocument } from 'src/app/data/paperless-document'
import { PaperlessDocumentMetadata } from 'src/app/data/paperless-document-metadata'
import { PaperlessDocumentType } from 'src/app/data/paperless-document-type'
import { PaperlessTag } from 'src/app/data/paperless-tag'
import { DocumentTitlePipe } from 'src/app/pipes/document-title.pipe'
import { DocumentListViewService } from 'src/app/services/document-list-view.service'
import { OpenDocumentsService } from 'src/app/services/open-documents.service'
import { CorrespondentService } from 'src/app/services/rest/correspondent.service'
import { DocumentTypeService } from 'src/app/services/rest/document-type.service'
import { DocumentService } from 'src/app/services/rest/document.service'
import { ConfirmDialogComponent } from '../common/confirm-dialog/confirm-dialog.component'
import { CorrespondentEditDialogComponent } from '../common/edit-dialog/correspondent-edit-dialog/correspondent-edit-dialog.component'
import { DocumentTypeEditDialogComponent } from '../common/edit-dialog/document-type-edit-dialog/document-type-edit-dialog.component'
import { PDFDocumentProxy } from 'ng2-pdf-viewer'
import { ToastService } from 'src/app/services/toast.service'
import { TextComponent } from '../common/input/text/text.component'
import { SettingsService } from 'src/app/services/settings.service'
import { dirtyCheck, DirtyComponent } from '@ngneat/dirty-check-forms'
import { Observable, Subject, BehaviorSubject } from 'rxjs'
import {
  first,
  takeUntil,
  switchMap,
  map,
  debounceTime,
  distinctUntilChanged,
} from 'rxjs/operators'
import { PaperlessDocumentSuggestions } from 'src/app/data/paperless-document-suggestions'
import {
  FILTER_CORRESPONDENT,
  FILTER_CREATED_AFTER,
  FILTER_CREATED_BEFORE,
  FILTER_DOCUMENT_TYPE,
  FILTER_FULLTEXT_MORELIKE,
  FILTER_HAS_TAGS_ALL,
  FILTER_STORAGE_PATH,
} from 'src/app/data/filter-rule-type'
import { StoragePathService } from 'src/app/services/rest/storage-path.service'
import { PaperlessStoragePath } from 'src/app/data/paperless-storage-path'
import { StoragePathEditDialogComponent } from '../common/edit-dialog/storage-path-edit-dialog/storage-path-edit-dialog.component'
import { SETTINGS_KEYS } from 'src/app/data/paperless-uisettings'
import {
  PermissionAction,
  PermissionsService,
  PermissionType,
} from 'src/app/services/permissions.service'
import { PaperlessUser } from 'src/app/data/paperless-user'
import { UserService } from 'src/app/services/rest/user.service'
import { PaperlessDocumentNote } from 'src/app/data/paperless-document-note'
import { HttpClient } from '@angular/common/http'
import { ComponentWithPermissions } from '../with-permissions/with-permissions.component'
import { EditDialogMode } from '../common/edit-dialog/edit-dialog.component'
import { ObjectWithId } from 'src/app/data/object-with-id'
import { FilterRule } from 'src/app/data/filter-rule'
import { ISODateAdapter } from 'src/app/utils/ngb-iso-date-adapter'
import { ShareLinksDropdownComponent } from '../common/share-links-dropdown/share-links-dropdown.component'

enum DocumentDetailNavIDs {
  Details = 1,
  Content = 2,
  Metadata = 3,
  Preview = 4,
  Notes = 5,
  Permissions = 6,
}

@Component({
  selector: 'pngx-document-detail',
  templateUrl: './document-detail.component.html',
  styleUrls: ['./document-detail.component.scss'],
})
export class DocumentDetailComponent
  extends ComponentWithPermissions
  implements OnInit, OnDestroy, DirtyComponent
{
  @ViewChild('inputTitle')
  titleInput: TextComponent

  expandOriginalMetadata = false
  expandArchivedMetadata = false

  error: any

  networkActive = false

  documentId: number
  document: PaperlessDocument
  metadata: PaperlessDocumentMetadata
  suggestions: PaperlessDocumentSuggestions
  users: PaperlessUser[]

  title: string
  titleSubject: Subject<string> = new Subject()
  previewUrl: string
  previewText: string
  downloadUrl: string
  downloadOriginalUrl: string

  correspondents: PaperlessCorrespondent[]
  documentTypes: PaperlessDocumentType[]
  storagePaths: PaperlessStoragePath[]

  documentForm: FormGroup = new FormGroup({
    title: new FormControl(''),
    content: new FormControl(''),
    created_date: new FormControl(),
    correspondent: new FormControl(),
    document_type: new FormControl(),
    storage_path: new FormControl(),
    archive_serial_number: new FormControl(),
    tags: new FormControl([]),
    permissions_form: new FormControl(null),
  })

  previewCurrentPage: number = 1
  previewNumPages: number = 1

  store: BehaviorSubject<any>
  isDirty$: Observable<boolean>
  unsubscribeNotifier: Subject<any> = new Subject()
  docChangeNotifier: Subject<any> = new Subject()

  requiresPassword: boolean = false
  password: string

  ogDate: Date

  @ViewChild('nav') nav: NgbNav
  @ViewChild('pdfPreview') set pdfPreview(element) {
    // this gets called when compontent added or removed from DOM
    if (
      element &&
      element.nativeElement.offsetParent !== null &&
      this.nav?.activeId == 4
    ) {
      // its visible
      setTimeout(() => this.nav?.select(1))
    }
  }

  DocumentDetailNavIDs = DocumentDetailNavIDs
  activeNavID: number

  constructor(
    private documentsService: DocumentService,
    private route: ActivatedRoute,
    private correspondentService: CorrespondentService,
    private documentTypeService: DocumentTypeService,
    private router: Router,
    private modalService: NgbModal,
    private openDocumentService: OpenDocumentsService,
    private documentListViewService: DocumentListViewService,
    private documentTitlePipe: DocumentTitlePipe,
    private toastService: ToastService,
    private settings: SettingsService,
    private storagePathService: StoragePathService,
    private permissionsService: PermissionsService,
    private userService: UserService,
    private http: HttpClient
  ) {
    super()
  }

  titleKeyUp(event) {
    this.titleSubject.next(event.target?.value)
  }

  get useNativePdfViewer(): boolean {
    return this.settings.get(SETTINGS_KEYS.USE_NATIVE_PDF_VIEWER)
  }

  getContentType() {
    return this.metadata?.has_archive_version
      ? 'application/pdf'
      : this.metadata?.original_mime_type
  }

  get renderAsPlainText(): boolean {
    return ['text/plain', 'application/csv', 'text/csv'].includes(
      this.getContentType()
    )
  }

  get isRTL() {
    if (!this.metadata || !this.metadata.lang) return false
    else {
      return ['ar', 'he', 'fe'].includes(this.metadata.lang)
    }
  }

  ngOnInit(): void {
    this.documentForm.valueChanges
      .pipe(takeUntil(this.unsubscribeNotifier))
      .subscribe(() => {
        this.error = null
        const docValues = Object.assign({}, this.documentForm.value)
        docValues['owner'] =
          this.documentForm.get('permissions_form').value['owner']
        docValues['set_permissions'] =
          this.documentForm.get('permissions_form').value['set_permissions']
        delete docValues['permissions_form']
        Object.assign(this.document, docValues)
      })

    this.correspondentService
      .listAll()
      .pipe(first(), takeUntil(this.unsubscribeNotifier))
      .subscribe((result) => (this.correspondents = result.results))

    this.documentTypeService
      .listAll()
      .pipe(first(), takeUntil(this.unsubscribeNotifier))
      .subscribe((result) => (this.documentTypes = result.results))

    this.storagePathService
      .listAll()
      .pipe(first(), takeUntil(this.unsubscribeNotifier))
      .subscribe((result) => (this.storagePaths = result.results))

    this.userService
      .listAll()
      .pipe(first(), takeUntil(this.unsubscribeNotifier))
      .subscribe((result) => (this.users = result.results))

    this.route.paramMap
      .pipe(
        takeUntil(this.unsubscribeNotifier),
        switchMap((paramMap) => {
          const documentId = +paramMap.get('id')
          this.docChangeNotifier.next(documentId)
          return this.documentsService.get(documentId)
        })
      )
      .pipe(
        switchMap((doc) => {
          this.documentId = doc.id
          this.previewUrl = this.documentsService.getPreviewUrl(this.documentId)
          this.http.get(this.previewUrl, { responseType: 'text' }).subscribe({
            next: (res) => {
              this.previewText = res.toString()
            },
            error: (err) => {
              this.previewText = $localize`An error occurred loading content: ${
                err.message ?? err.toString()
              }`
            },
          })
          this.downloadUrl = this.documentsService.getDownloadUrl(
            this.documentId
          )
          this.downloadOriginalUrl = this.documentsService.getDownloadUrl(
            this.documentId,
            true
          )
          this.suggestions = null
          const openDocument = this.openDocumentService.getOpenDocument(
            this.documentId
          )
          if (openDocument) {
            if (this.documentForm.dirty) {
              Object.assign(openDocument, this.documentForm.value)
              openDocument['owner'] =
                this.documentForm.get('permissions_form').value['owner']
              openDocument['permissions'] =
                this.documentForm.get('permissions_form').value[
                  'set_permissions'
                ]
              delete openDocument['permissions_form']
            }
            this.updateComponent(openDocument)
          } else {
            this.openDocumentService.openDocument(doc)
            this.updateComponent(doc)
          }

          this.titleSubject
            .pipe(
              debounceTime(1000),
              distinctUntilChanged(),
              takeUntil(this.docChangeNotifier),
              takeUntil(this.unsubscribeNotifier)
            )
            .subscribe({
              next: (titleValue) => {
                // In the rare case when the field changed just after debounced event was fired.
                // We dont want to overwrite whats actually in the text field, so just return
                if (titleValue !== this.titleInput.value) return

                this.title = titleValue
                this.documentForm.patchValue({ title: titleValue })
              },
              complete: () => {
                // doc changed so we manually check dirty in case title was changed
                if (
                  this.store.getValue().title !==
                  this.documentForm.get('title').value
                ) {
                  this.openDocumentService.setDirty(doc, true)
                }
              },
            })

          // Initialize dirtyCheck
          this.store = new BehaviorSubject({
            title: doc.title,
            content: doc.content,
            created_date: doc.created_date,
            correspondent: doc.correspondent,
            document_type: doc.document_type,
            storage_path: doc.storage_path,
            archive_serial_number: doc.archive_serial_number,
            tags: [...doc.tags],
            permissions_form: {
              owner: doc.owner,
              set_permissions: doc.permissions,
            },
          })

          this.isDirty$ = dirtyCheck(
            this.documentForm,
            this.store.asObservable()
          )

          return this.isDirty$.pipe(
            takeUntil(this.unsubscribeNotifier),
            map((dirty) => ({ doc, dirty }))
          )
        })
      )
      .subscribe({
        next: ({ doc, dirty }) => {
          this.openDocumentService.setDirty(doc, dirty)
        },
        error: (error) => {
          this.router.navigate(['404'], {
            replaceUrl: true,
          })
        },
      })

    this.route.paramMap.subscribe((paramMap) => {
      const section = paramMap.get('section')
      if (section) {
        const navIDKey: string = Object.keys(DocumentDetailNavIDs).find(
          (navID) => navID.toLowerCase() == section
        )
        if (navIDKey) {
          this.activeNavID = DocumentDetailNavIDs[navIDKey]
        }
      } else if (paramMap.get('id')) {
        this.router.navigate(['documents', +paramMap.get('id'), 'details'], {
          replaceUrl: true,
        })
      }
    })
  }

  ngOnDestroy(): void {
    this.unsubscribeNotifier.next(this)
    this.unsubscribeNotifier.complete()
  }

  onNavChange(navChangeEvent: NgbNavChangeEvent) {
    const [foundNavIDkey] = Object.entries(DocumentDetailNavIDs).find(
      ([, navIDValue]) => navIDValue == navChangeEvent.nextId
    )
    if (foundNavIDkey)
      this.router.navigate([
        'documents',
        this.documentId,
        foundNavIDkey.toLowerCase(),
      ])
  }

  updateComponent(doc: PaperlessDocument) {
    this.document = doc
    this.requiresPassword = false
    this.documentsService
      .getMetadata(doc.id)
      .pipe(first())
      .subscribe({
        next: (result) => {
          this.metadata = result
        },
        error: (error) => {
          this.metadata = null
          this.toastService.showError(
            $localize`Error retrieving metadata`,
            error
          )
        },
      })
    if (
      this.permissionsService.currentUserHasObjectPermissions(
        PermissionAction.Change,
        doc
      )
    ) {
      this.documentsService
        .getSuggestions(doc.id)
        .pipe(first(), takeUntil(this.unsubscribeNotifier))
        .subscribe({
          next: (result) => {
            this.suggestions = result
          },
          error: (error) => {
            this.suggestions = null
            this.toastService.showError(
              $localize`Error retrieving suggestions.`,
              error
            )
          },
        })
    }
    this.title = this.documentTitlePipe.transform(doc.title)
    const docFormValues = Object.assign({}, doc)
    docFormValues['permissions_form'] = {
      owner: doc.owner,
      set_permissions: doc.permissions,
    }

    this.documentForm.patchValue(docFormValues, { emitEvent: false })
    if (!this.userCanEdit) this.documentForm.disable()
  }

  createDocumentType(newName: string) {
    var modal = this.modalService.open(DocumentTypeEditDialogComponent, {
      backdrop: 'static',
    })
    modal.componentInstance.dialogMode = EditDialogMode.CREATE
    if (newName) modal.componentInstance.object = { name: newName }
    modal.componentInstance.succeeded
      .pipe(
        switchMap((newDocumentType) => {
          return this.documentTypeService
            .listAll()
            .pipe(map((documentTypes) => ({ newDocumentType, documentTypes })))
        })
      )
      .pipe(takeUntil(this.unsubscribeNotifier))
      .subscribe(({ newDocumentType, documentTypes }) => {
        this.documentTypes = documentTypes.results
        this.documentForm.get('document_type').setValue(newDocumentType.id)
      })
  }

  createCorrespondent(newName: string) {
    var modal = this.modalService.open(CorrespondentEditDialogComponent, {
      backdrop: 'static',
    })
    modal.componentInstance.dialogMode = EditDialogMode.CREATE
    if (newName) modal.componentInstance.object = { name: newName }
    modal.componentInstance.succeeded
      .pipe(
        switchMap((newCorrespondent) => {
          return this.correspondentService
            .listAll()
            .pipe(
              map((correspondents) => ({ newCorrespondent, correspondents }))
            )
        })
      )
      .pipe(takeUntil(this.unsubscribeNotifier))
      .subscribe(({ newCorrespondent, correspondents }) => {
        this.correspondents = correspondents.results
        this.documentForm.get('correspondent').setValue(newCorrespondent.id)
      })
  }

  createStoragePath(newName: string) {
    var modal = this.modalService.open(StoragePathEditDialogComponent, {
      backdrop: 'static',
    })
    modal.componentInstance.dialogMode = EditDialogMode.CREATE
    if (newName) modal.componentInstance.object = { name: newName }
    modal.componentInstance.succeeded
      .pipe(
        switchMap((newStoragePath) => {
          return this.storagePathService
            .listAll()
            .pipe(map((storagePaths) => ({ newStoragePath, storagePaths })))
        })
      )
      .pipe(takeUntil(this.unsubscribeNotifier))
      .subscribe(({ newStoragePath, storagePaths }) => {
        this.storagePaths = storagePaths.results
        this.documentForm.get('storage_path').setValue(newStoragePath.id)
      })
  }

  discard() {
    this.documentsService
      .get(this.documentId)
      .pipe(first())
      .subscribe({
        next: (doc) => {
          Object.assign(this.document, doc)
          doc['permissions_form'] = {
            owner: doc.owner,
            set_permissions: doc.permissions,
          }
          this.title = doc.title
          this.documentForm.patchValue(doc)
          this.openDocumentService.setDirty(doc, false)
        },
        error: () => {
          this.router.navigate(['404'], {
            replaceUrl: true,
          })
        },
      })
  }

  save(close: boolean = false) {
    this.networkActive = true
    this.documentsService
      .update(this.document)
      .pipe(first())
      .subscribe({
        next: () => {
          this.store.next(this.documentForm.value)
          this.toastService.showInfo($localize`Document saved successfully.`)
          close && this.close()
          this.networkActive = false
          this.error = null
        },
        error: (error) => {
          this.networkActive = false
          if (!this.userCanEdit) {
            this.toastService.showInfo($localize`Document saved successfully.`)
            close && this.close()
          } else {
            this.error = error.error
            this.toastService.showError($localize`Error saving document`, error)
          }
        },
      })
  }

  saveEditNext() {
    this.networkActive = true
    this.store.next(this.documentForm.value)
    this.documentsService
      .update(this.document)
      .pipe(
        switchMap((updateResult) => {
          return this.documentListViewService
            .getNext(this.documentId)
            .pipe(map((nextDocId) => ({ nextDocId, updateResult })))
        })
      )
      .pipe(
        switchMap(({ nextDocId, updateResult }) => {
          if (nextDocId && updateResult)
            return this.openDocumentService
              .closeDocument(this.document)
              .pipe(
                map((closeResult) => ({ updateResult, nextDocId, closeResult }))
              )
        })
      )
      .pipe(first())
      .subscribe({
        next: ({ updateResult, nextDocId, closeResult }) => {
          this.error = null
          this.networkActive = false
          if (closeResult && updateResult && nextDocId) {
            this.router.navigate(['documents', nextDocId])
            this.titleInput?.focus()
          }
        },
        error: (error) => {
          this.networkActive = false
          this.error = error.error
          this.toastService.showError($localize`Error saving document`, error)
        },
      })
  }

  close() {
    this.openDocumentService
      .closeDocument(this.document)
      .pipe(first())
      .subscribe((closed) => {
        if (!closed) return
        if (this.documentListViewService.activeSavedViewId) {
          this.router.navigate([
            'view',
            this.documentListViewService.activeSavedViewId,
          ])
        } else {
          this.router.navigate(['documents'])
        }
      })
  }

  delete() {
    let modal = this.modalService.open(ConfirmDialogComponent, {
      backdrop: 'static',
    })
    modal.componentInstance.title = $localize`Confirm delete`
    modal.componentInstance.messageBold = $localize`Do you really want to delete document "${this.document.title}"?`
    modal.componentInstance.message = $localize`The files for this document will be deleted permanently. This operation cannot be undone.`
    modal.componentInstance.btnClass = 'btn-danger'
    modal.componentInstance.btnCaption = $localize`Delete document`
    this.subscribeModalDelete(modal) // so can be re-subscribed if error
  }

  subscribeModalDelete(modal) {
    modal.componentInstance.confirmClicked
      .pipe(
        switchMap(() => {
          modal.componentInstance.buttonsEnabled = false
          return this.documentsService.delete(this.document)
        })
      )
      .pipe(takeUntil(this.unsubscribeNotifier))
      .subscribe({
        next: () => {
          modal.close()
          this.close()
        },
        error: (error) => {
          this.toastService.showError($localize`Error deleting document`, error)
          modal.componentInstance.buttonsEnabled = true
          this.subscribeModalDelete(modal)
        },
      })
  }

  moreLike() {
    this.documentListViewService.quickFilter([
      {
        rule_type: FILTER_FULLTEXT_MORELIKE,
        value: this.documentId.toString(),
      },
    ])
  }

  redoOcr() {
    let modal = this.modalService.open(ConfirmDialogComponent, {
      backdrop: 'static',
    })
    modal.componentInstance.title = $localize`Redo OCR confirm`
    modal.componentInstance.messageBold = $localize`This operation will permanently redo OCR for this document.`
    modal.componentInstance.message = $localize`This operation cannot be undone.`
    modal.componentInstance.btnClass = 'btn-danger'
    modal.componentInstance.btnCaption = $localize`Proceed`
    modal.componentInstance.confirmClicked.subscribe(() => {
      modal.componentInstance.buttonsEnabled = false
      this.documentsService
        .bulkEdit([this.document.id], 'redo_ocr', {})
        .subscribe({
          next: () => {
            this.toastService.showInfo(
              $localize`Redo OCR operation will begin in the background. Close and re-open or reload this document after the operation has completed to see new content.`
            )
            if (modal) {
              modal.close()
            }
          },
          error: (error) => {
            if (modal) {
              modal.componentInstance.buttonsEnabled = true
            }
            this.toastService.showError(
              $localize`Error executing operation`,
              error
            )
          },
        })
    })
  }

  hasNext() {
    return this.documentListViewService.hasNext(this.documentId)
  }

  hasPrevious() {
    return this.documentListViewService.hasPrevious(this.documentId)
  }

  nextDoc() {
    this.documentListViewService
      .getNext(this.document.id)
      .subscribe((nextDocId: number) => {
        this.router.navigate(['documents', nextDocId])
      })
  }

  previousDoc() {
    this.documentListViewService
      .getPrevious(this.document.id)
      .subscribe((prevDocId: number) => {
        this.router.navigate(['documents', prevDocId])
      })
  }

  pdfPreviewLoaded(pdf: PDFDocumentProxy) {
    this.previewNumPages = pdf.numPages
    if (this.password) this.requiresPassword = false
  }

  onError(event) {
    if (event.name == 'PasswordException') {
      this.requiresPassword = true
    }
  }

  onPasswordKeyUp(event: KeyboardEvent) {
    if ('Enter' == event.key) {
      this.password = (event.target as HTMLInputElement).value
    }
  }

  get showPermissions(): boolean {
    return (
      this.permissionsService.currentUserCan(
        PermissionAction.View,
        PermissionType.User
      ) && this.userIsOwner
    )
  }

  get notesEnabled(): boolean {
    return (
      this.settings.get(SETTINGS_KEYS.NOTES_ENABLED) &&
      this.permissionsService.currentUserCan(
        PermissionAction.View,
        PermissionType.Note
      )
    )
  }

  notesUpdated(notes: PaperlessDocumentNote[]) {
    this.document.notes = notes
    this.openDocumentService.refreshDocument(this.documentId)
  }

  get userIsOwner(): boolean {
    let doc: PaperlessDocument = Object.assign({}, this.document)
    // dont disable while editing
    if (this.document && this.store?.value.permissions_form?.owner) {
      doc.owner = this.store?.value.permissions_form?.owner
    }
    return !this.document || this.permissionsService.currentUserOwnsObject(doc)
  }

  get userCanEdit(): boolean {
    let doc: PaperlessDocument = Object.assign({}, this.document)
    // dont disable while editing
    if (this.document && this.store?.value.permissions_form?.owner) {
      doc.owner = this.store?.value.permissions_form?.owner
    }
    return (
      !this.document ||
      this.permissionsService.currentUserHasObjectPermissions(
        PermissionAction.Change,
        doc
      )
    )
  }

  filterDocuments(items: ObjectWithId[] | NgbDateStruct[]) {
    const filterRules: FilterRule[] = items.flatMap((i) => {
      if (i.hasOwnProperty('year')) {
        const isoDateAdapter = new ISODateAdapter()
        const dateAfter: Date = new Date(isoDateAdapter.toModel(i))
        dateAfter.setDate(dateAfter.getDate() - 1)
        const dateBefore: Date = new Date(isoDateAdapter.toModel(i))
        dateBefore.setDate(dateBefore.getDate() + 1)
        // Created Date
        return [
          {
            rule_type: FILTER_CREATED_AFTER,
            value: dateAfter.toISOString().substring(0, 10),
          },
          {
            rule_type: FILTER_CREATED_BEFORE,
            value: dateBefore.toISOString().substring(0, 10),
          },
        ]
      } else if (i.hasOwnProperty('last_correspondence')) {
        // Correspondent
        return {
          rule_type: FILTER_CORRESPONDENT,
          value: (i as PaperlessCorrespondent).id.toString(),
        }
      } else if (i.hasOwnProperty('path')) {
        // Storage Path
        return {
          rule_type: FILTER_STORAGE_PATH,
          value: (i as PaperlessStoragePath).id.toString(),
        }
      } else if (i.hasOwnProperty('is_inbox_tag')) {
        // Tag
        return {
          rule_type: FILTER_HAS_TAGS_ALL,
          value: (i as PaperlessTag).id.toString(),
        }
      } else {
        // Document Type, has no specific props
        return {
          rule_type: FILTER_DOCUMENT_TYPE,
          value: (i as PaperlessDocumentType).id.toString(),
        }
      }
    })

    this.documentListViewService.quickFilter(filterRules)
  }
}
