import { DatePipe } from '@angular/common'
import { HttpClientTestingModule } from '@angular/common/http/testing'
import {
  ComponentFixture,
  TestBed,
  fakeAsync,
  tick,
  discardPeriodicTasks,
} from '@angular/core/testing'
import { FormsModule, ReactiveFormsModule } from '@angular/forms'
import { By } from '@angular/platform-browser'
import { Router, ActivatedRoute, convertToParamMap } from '@angular/router'
import { RouterTestingModule } from '@angular/router/testing'
import {
  NgbModal,
  NgbModule,
  NgbModalModule,
  NgbModalRef,
  NgbDateStruct,
} from '@ng-bootstrap/ng-bootstrap'
import { NgSelectModule } from '@ng-select/ng-select'
import { PdfViewerComponent } from 'ng2-pdf-viewer'
import { of, throwError } from 'rxjs'
import { routes } from 'src/app/app-routing.module'
import {
  FILTER_FULLTEXT_MORELIKE,
  FILTER_CORRESPONDENT,
  FILTER_DOCUMENT_TYPE,
  FILTER_STORAGE_PATH,
  FILTER_HAS_TAGS_ALL,
  FILTER_CREATED_AFTER,
  FILTER_CREATED_BEFORE,
} from 'src/app/data/filter-rule-type'
import { PaperlessCorrespondent } from 'src/app/data/paperless-correspondent'
import { PaperlessDocument } from 'src/app/data/paperless-document'
import { PaperlessDocumentType } from 'src/app/data/paperless-document-type'
import { PaperlessStoragePath } from 'src/app/data/paperless-storage-path'
import { PaperlessTag } from 'src/app/data/paperless-tag'
import { IfOwnerDirective } from 'src/app/directives/if-owner.directive'
import { IfPermissionsDirective } from 'src/app/directives/if-permissions.directive'
import { PermissionsGuard } from 'src/app/guards/permissions.guard'
import { CustomDatePipe } from 'src/app/pipes/custom-date.pipe'
import { DocumentTitlePipe } from 'src/app/pipes/document-title.pipe'
import { SafeHtmlPipe } from 'src/app/pipes/safehtml.pipe'
import { SafeUrlPipe } from 'src/app/pipes/safeurl.pipe'
import { DocumentListViewService } from 'src/app/services/document-list-view.service'
import { OpenDocumentsService } from 'src/app/services/open-documents.service'
import { PermissionsService } from 'src/app/services/permissions.service'
import { CorrespondentService } from 'src/app/services/rest/correspondent.service'
import { DocumentTypeService } from 'src/app/services/rest/document-type.service'
import { DocumentService } from 'src/app/services/rest/document.service'
import { StoragePathService } from 'src/app/services/rest/storage-path.service'
import { UserService } from 'src/app/services/rest/user.service'
import { SettingsService } from 'src/app/services/settings.service'
import { ToastService } from 'src/app/services/toast.service'
import { ConfirmDialogComponent } from '../common/confirm-dialog/confirm-dialog.component'
import { CorrespondentEditDialogComponent } from '../common/edit-dialog/correspondent-edit-dialog/correspondent-edit-dialog.component'
import { DocumentTypeEditDialogComponent } from '../common/edit-dialog/document-type-edit-dialog/document-type-edit-dialog.component'
import { StoragePathEditDialogComponent } from '../common/edit-dialog/storage-path-edit-dialog/storage-path-edit-dialog.component'
import { DateComponent } from '../common/input/date/date.component'
import { NumberComponent } from '../common/input/number/number.component'
import { PermissionsFormComponent } from '../common/input/permissions/permissions-form/permissions-form.component'
import { SelectComponent } from '../common/input/select/select.component'
import { TagsComponent } from '../common/input/tags/tags.component'
import { TextComponent } from '../common/input/text/text.component'
import { PageHeaderComponent } from '../common/page-header/page-header.component'
import { DocumentNotesComponent } from '../document-notes/document-notes.component'
import { DocumentDetailComponent } from './document-detail.component'
import { ShareLinksDropdownComponent } from '../common/share-links-dropdown/share-links-dropdown.component'

const doc: PaperlessDocument = {
  id: 3,
  title: 'Doc 3',
  correspondent: 11,
  document_type: 21,
  storage_path: 31,
  tags: [41, 42, 43],
  content: 'text content',
  added: new Date(),
  created: new Date(),
  archive_serial_number: null,
  original_file_name: 'file.pdf',
  owner: null,
  user_can_change: true,
  notes: [
    {
      created: new Date(),
      note: 'note 1',
      user: 1,
    },
    {
      created: new Date(),
      note: 'note 2',
      user: 2,
    },
  ],
}

describe('DocumentDetailComponent', () => {
  let component: DocumentDetailComponent
  let fixture: ComponentFixture<DocumentDetailComponent>
  let router: Router
  let activatedRoute: ActivatedRoute
  let documentService: DocumentService
  let openDocumentsService: OpenDocumentsService
  let modalService: NgbModal
  let toastService: ToastService
  let documentListViewService: DocumentListViewService
  let settingsService: SettingsService

  let currentUserCan = true
  let currentUserHasObjectPermissions = true
  let currentUserOwnsObject = true

  beforeEach(async () => {
    TestBed.configureTestingModule({
      declarations: [
        DocumentDetailComponent,
        DocumentTitlePipe,
        PageHeaderComponent,
        IfPermissionsDirective,
        TagsComponent,
        SelectComponent,
        TextComponent,
        NumberComponent,
        DateComponent,
        DocumentNotesComponent,
        CustomDatePipe,
        DocumentTypeEditDialogComponent,
        CorrespondentEditDialogComponent,
        StoragePathEditDialogComponent,
        IfOwnerDirective,
        PermissionsFormComponent,
        SafeHtmlPipe,
        ConfirmDialogComponent,
        PdfViewerComponent,
        SafeUrlPipe,
        ShareLinksDropdownComponent,
      ],
      providers: [
        DocumentTitlePipe,
        {
          provide: CorrespondentService,
          useValue: {
            listAll: () =>
              of({
                results: [
                  {
                    id: 11,
                    name: 'Correspondent11',
                  },
                ],
              }),
          },
        },
        {
          provide: DocumentTypeService,
          useValue: {
            listAll: () =>
              of({
                results: [
                  {
                    id: 21,
                    name: 'DocumentType21',
                  },
                ],
              }),
          },
        },
        {
          provide: StoragePathService,
          useValue: {
            listAll: () =>
              of({
                results: [
                  {
                    id: 31,
                    name: 'StoragePath31',
                  },
                ],
              }),
          },
        },
        {
          provide: UserService,
          useValue: {
            listAll: () =>
              of({
                results: [
                  {
                    id: 1,
                    username: 'user1',
                  },
                  {
                    id: 2,
                    username: 'user2',
                  },
                ],
              }),
          },
        },
        {
          provide: PermissionsService,
          useValue: {
            currentUserCan: () => currentUserCan,
            currentUserHasObjectPermissions: () =>
              currentUserHasObjectPermissions,
            currentUserOwnsObject: () => currentUserOwnsObject,
          },
        },
        PermissionsGuard,
        CustomDatePipe,
        DatePipe,
      ],
      imports: [
        RouterTestingModule.withRoutes(routes),
        HttpClientTestingModule,
        NgbModule,
        NgSelectModule,
        FormsModule,
        ReactiveFormsModule,
        NgbModalModule,
      ],
    }).compileComponents()

    router = TestBed.inject(Router)
    activatedRoute = TestBed.inject(ActivatedRoute)
    jest
      .spyOn(activatedRoute, 'paramMap', 'get')
      .mockReturnValue(of(convertToParamMap({ id: 3 })))
    openDocumentsService = TestBed.inject(OpenDocumentsService)
    documentService = TestBed.inject(DocumentService)
    modalService = TestBed.inject(NgbModal)
    toastService = TestBed.inject(ToastService)
    documentListViewService = TestBed.inject(DocumentListViewService)
    settingsService = TestBed.inject(SettingsService)
    fixture = TestBed.createComponent(DocumentDetailComponent)
    component = fixture.componentInstance
  })

  it('should load four tabs via url params', () => {
    jest
      .spyOn(activatedRoute, 'paramMap', 'get')
      .mockReturnValue(of(convertToParamMap({ id: 3, section: 'notes' })))
    jest.spyOn(openDocumentsService, 'getOpenDocument').mockReturnValue(null)
    jest
      .spyOn(openDocumentsService, 'openDocument')
      .mockReturnValueOnce(of(true))
    fixture.detectChanges()
    expect(component.activeNavID).toEqual(5) // DocumentDetailNavIDs.Notes
  })

  it('should change url on tab switch', () => {
    initNormally()
    const navigateSpy = jest.spyOn(router, 'navigate')
    component.nav.select(5)
    component.nav.navChange.next({
      activeId: 1,
      nextId: 5,
      preventDefault: () => {},
    })
    fixture.detectChanges()
    expect(navigateSpy).toHaveBeenCalledWith(['documents', 3, 'notes'])
  })

  it('should update title after debounce', fakeAsync(() => {
    initNormally()
    component.titleInput.value = 'Foo Bar'
    component.titleSubject.next('Foo Bar')
    tick(1000)
    expect(component.documentForm.get('title').value).toEqual('Foo Bar')
    discardPeriodicTasks()
  }))

  it('should update title before doc change if wasnt updated via debounce', fakeAsync(() => {
    initNormally()
    component.titleInput.value = 'Foo Bar'
    component.titleInput.inputField.nativeElement.dispatchEvent(
      new Event('change')
    )
    tick(1000)
    expect(component.documentForm.get('title').value).toEqual('Foo Bar')
  }))

  it('should load non-open document via param', () => {
    initNormally()
    expect(component.document).toEqual(doc)
  })

  it('should load already-opened document via param', () => {
    jest.spyOn(documentService, 'get').mockReturnValueOnce(of(doc))
    jest.spyOn(openDocumentsService, 'getOpenDocument').mockReturnValue(doc)
    fixture.detectChanges() // calls ngOnInit
    expect(component.document).toEqual(doc)
  })

  it('should disable form if user cannot edit', () => {
    currentUserHasObjectPermissions = false
    initNormally()
    expect(component.documentForm.disabled).toBeTruthy()
  })

  it('should support creating document type', () => {
    initNormally()
    let openModal: NgbModalRef
    modalService.activeInstances.subscribe((modal) => (openModal = modal[0]))
    const modalSpy = jest.spyOn(modalService, 'open')
    component.createDocumentType('NewDocType2')
    expect(modalSpy).toHaveBeenCalled()
    openModal.componentInstance.succeeded.next({ id: 12, name: 'NewDocType12' })
    expect(component.documentForm.get('document_type').value).toEqual(12)
  })

  it('should support creating correspondent', () => {
    initNormally()
    let openModal: NgbModalRef
    modalService.activeInstances.subscribe((modal) => (openModal = modal[0]))
    const modalSpy = jest.spyOn(modalService, 'open')
    component.createCorrespondent('NewCorrrespondent12')
    expect(modalSpy).toHaveBeenCalled()
    openModal.componentInstance.succeeded.next({
      id: 12,
      name: 'NewCorrrespondent12',
    })
    expect(component.documentForm.get('correspondent').value).toEqual(12)
  })

  it('should support creating storage path', () => {
    initNormally()
    let openModal: NgbModalRef
    modalService.activeInstances.subscribe((modal) => (openModal = modal[0]))
    const modalSpy = jest.spyOn(modalService, 'open')
    component.createStoragePath('NewStoragePath12')
    expect(modalSpy).toHaveBeenCalled()
    openModal.componentInstance.succeeded.next({
      id: 12,
      name: 'NewStoragePath12',
    })
    expect(component.documentForm.get('storage_path').value).toEqual(12)
  })

  it('should allow dischard changes', () => {
    initNormally()
    component.title = 'Foo Bar'
    fixture.detectChanges()
    jest.spyOn(documentService, 'get').mockReturnValueOnce(of(doc))
    component.discard()
    fixture.detectChanges()
    expect(component.title).toEqual(doc.title)
    expect(openDocumentsService.hasDirty()).toBeFalsy()
    // this time with error, mostly for coverage
    component.title = 'Foo Bar'
    fixture.detectChanges()
    const navigateSpy = jest.spyOn(router, 'navigate')
    jest
      .spyOn(documentService, 'get')
      .mockReturnValueOnce(throwError(() => new Error('unable to discard')))
    component.discard()
    fixture.detectChanges()
    expect(navigateSpy).toHaveBeenCalledWith(['404'], { replaceUrl: true })
  })

  it('should 404 on invalid id', () => {
    jest.spyOn(documentService, 'get').mockReturnValueOnce(of(null))
    const navigateSpy = jest.spyOn(router, 'navigate')
    fixture.detectChanges()
    expect(navigateSpy).toHaveBeenCalledWith(['404'], { replaceUrl: true })
  })

  it('should support save, close and show success toast', () => {
    initNormally()
    component.title = 'Foo Bar'
    const closeSpy = jest.spyOn(component, 'close')
    const updateSpy = jest.spyOn(documentService, 'update')
    const toastSpy = jest.spyOn(toastService, 'showInfo')
    updateSpy.mockImplementation((o) => of(doc))
    component.save(true)
    expect(updateSpy).toHaveBeenCalled()
    expect(closeSpy).toHaveBeenCalled()
    expect(toastSpy).toHaveBeenCalledWith('Document saved successfully.')
  })

  it('should support save without close and show success toast', () => {
    initNormally()
    component.title = 'Foo Bar'
    const closeSpy = jest.spyOn(component, 'close')
    const updateSpy = jest.spyOn(documentService, 'update')
    const toastSpy = jest.spyOn(toastService, 'showInfo')
    updateSpy.mockImplementation((o) => of(doc))
    component.save()
    expect(updateSpy).toHaveBeenCalled()
    expect(closeSpy).not.toHaveBeenCalled()
    expect(toastSpy).toHaveBeenCalledWith('Document saved successfully.')
  })

  it('should show toast error on save if error occurs', () => {
    currentUserHasObjectPermissions = true
    initNormally()
    component.title = 'Foo Bar'
    const closeSpy = jest.spyOn(component, 'close')
    const updateSpy = jest.spyOn(documentService, 'update')
    const toastSpy = jest.spyOn(toastService, 'showError')
    const error = new Error('failed to save')
    updateSpy.mockImplementation(() => throwError(() => error))
    component.save()
    expect(updateSpy).toHaveBeenCalled()
    expect(closeSpy).not.toHaveBeenCalled()
    expect(toastSpy).toHaveBeenCalledWith('Error saving document', error)
  })

  it('should show error toast on save but close if user can no longer edit', () => {
    currentUserHasObjectPermissions = false
    initNormally()
    component.title = 'Foo Bar'
    const closeSpy = jest.spyOn(component, 'close')
    const updateSpy = jest.spyOn(documentService, 'update')
    const toastSpy = jest.spyOn(toastService, 'showInfo')
    updateSpy.mockImplementation(() =>
      throwError(() => new Error('failed to save'))
    )
    component.save(true)
    expect(updateSpy).toHaveBeenCalled()
    expect(closeSpy).toHaveBeenCalled()
    expect(toastSpy).toHaveBeenCalledWith('Document saved successfully.')
  })

  it('should allow save and next', () => {
    initNormally()
    const nextDocId = 100
    component.title = 'Foo Bar'
    const updateSpy = jest.spyOn(documentService, 'update')
    updateSpy.mockReturnValue(of(doc))
    const nextSpy = jest.spyOn(documentListViewService, 'getNext')
    nextSpy.mockReturnValue(of(nextDocId))
    const closeSpy = jest.spyOn(openDocumentsService, 'closeDocument')
    closeSpy.mockReturnValue(of(true))
    const navigateSpy = jest.spyOn(router, 'navigate')

    component.saveEditNext()
    expect(updateSpy).toHaveBeenCalled()
    expect(navigateSpy).toHaveBeenCalledWith(['documents', nextDocId])
    expect
  })

  it('should show toast error on save & next if error occurs', () => {
    currentUserHasObjectPermissions = true
    initNormally()
    component.title = 'Foo Bar'
    const closeSpy = jest.spyOn(component, 'close')
    const updateSpy = jest.spyOn(documentService, 'update')
    const toastSpy = jest.spyOn(toastService, 'showError')
    const error = new Error('failed to save')
    updateSpy.mockImplementation(() => throwError(() => error))
    component.saveEditNext()
    expect(updateSpy).toHaveBeenCalled()
    expect(closeSpy).not.toHaveBeenCalled()
    expect(toastSpy).toHaveBeenCalledWith('Error saving document', error)
  })

  it('should show save button and save & close or save & next', () => {
    const nextSpy = jest.spyOn(component, 'hasNext')
    nextSpy.mockReturnValueOnce(false)
    fixture.detectChanges()
    expect(
      fixture.debugElement
        .queryAll(By.css('button'))
        .find((b) => b.nativeElement.textContent === 'Save')
    ).not.toBeUndefined()
    expect(
      fixture.debugElement
        .queryAll(By.css('button'))
        .find((b) => b.nativeElement.textContent === 'Save & close')
    ).not.toBeUndefined()
    expect(
      fixture.debugElement
        .queryAll(By.css('button'))
        .find((b) => b.nativeElement.textContent === 'Save & next')
    ).toBeUndefined()
    nextSpy.mockReturnValue(true)
    fixture.detectChanges()
    expect(
      fixture.debugElement
        .queryAll(By.css('button'))
        .find((b) => b.nativeElement.textContent === 'Save & close')
    ).toBeUndefined()
    expect(
      fixture.debugElement
        .queryAll(By.css('button'))
        .find((b) => b.nativeElement.textContent === 'Save & next')
    ).not.toBeUndefined()
  })

  it('should allow close and navigate to documents by default', () => {
    initNormally()
    const navigateSpy = jest.spyOn(router, 'navigate')
    component.close()
    expect(navigateSpy).toHaveBeenCalledWith(['documents'])
  })

  it('should allow close and navigate to documents by default', () => {
    initNormally()
    jest
      .spyOn(documentListViewService, 'activeSavedViewId', 'get')
      .mockReturnValue(77)
    const navigateSpy = jest.spyOn(router, 'navigate')
    component.close()
    expect(navigateSpy).toHaveBeenCalledWith(['view', 77])
  })

  it('should not close if e.g. user-cancelled', () => {
    initNormally()
    jest.spyOn(openDocumentsService, 'closeDocument').mockReturnValue(of(false))
    const navigateSpy = jest.spyOn(router, 'navigate')
    component.close()
    expect(navigateSpy).not.toHaveBeenCalled()
  })

  it('should support delete, ask for confirmation', () => {
    initNormally()
    let openModal: NgbModalRef
    modalService.activeInstances.subscribe((modal) => (openModal = modal[0]))
    const modalSpy = jest.spyOn(modalService, 'open')
    const deleteSpy = jest.spyOn(documentService, 'delete')
    deleteSpy.mockReturnValue(of(true))
    component.delete()
    expect(modalSpy).toHaveBeenCalled()
    const modalCloseSpy = jest.spyOn(openModal, 'close')
    openModal.componentInstance.confirmClicked.next()
    expect(deleteSpy).toHaveBeenCalled()
    expect(modalCloseSpy).toHaveBeenCalled()
  })

  it('should allow retry delete if error', () => {
    initNormally()
    let openModal: NgbModalRef
    modalService.activeInstances.subscribe((modal) => (openModal = modal[0]))
    const modalSpy = jest.spyOn(modalService, 'open')
    const deleteSpy = jest.spyOn(documentService, 'delete')
    deleteSpy.mockReturnValueOnce(throwError(() => new Error('one time')))
    component.delete()
    expect(modalSpy).toHaveBeenCalled()
    const modalCloseSpy = jest.spyOn(openModal, 'close')
    openModal.componentInstance.confirmClicked.next()
    expect(deleteSpy).toHaveBeenCalled()
    expect(modalCloseSpy).not.toHaveBeenCalled()
    deleteSpy.mockReturnValueOnce(of(true))
    // retry
    openModal.componentInstance.confirmClicked.next()
    expect(deleteSpy).toHaveBeenCalled()
    expect(modalCloseSpy).toHaveBeenCalled()
  })

  it('should support more like quick filter', () => {
    initNormally()
    const qfSpy = jest.spyOn(documentListViewService, 'quickFilter')
    component.moreLike()
    expect(qfSpy).toHaveBeenCalledWith([
      {
        rule_type: FILTER_FULLTEXT_MORELIKE,
        value: doc.id.toString(),
      },
    ])
  })

  it('should support redo ocr, confirm and close modal after started', () => {
    initNormally()
    const bulkEditSpy = jest.spyOn(documentService, 'bulkEdit')
    bulkEditSpy.mockReturnValue(of(true))
    let openModal: NgbModalRef
    modalService.activeInstances.subscribe((modal) => (openModal = modal[0]))
    const modalSpy = jest.spyOn(modalService, 'open')
    const toastSpy = jest.spyOn(toastService, 'showInfo')
    component.redoOcr()
    const modalCloseSpy = jest.spyOn(openModal, 'close')
    openModal.componentInstance.confirmClicked.next()
    expect(bulkEditSpy).toHaveBeenCalledWith([doc.id], 'redo_ocr', {})
    expect(modalSpy).toHaveBeenCalled()
    expect(toastSpy).toHaveBeenCalled()
    expect(modalCloseSpy).toHaveBeenCalled()
  })

  it('should show error if redo ocr call fails', () => {
    initNormally()
    const bulkEditSpy = jest.spyOn(documentService, 'bulkEdit')
    let openModal: NgbModalRef
    modalService.activeInstances.subscribe((modal) => (openModal = modal[0]))
    const toastSpy = jest.spyOn(toastService, 'showError')
    component.redoOcr()
    const modalCloseSpy = jest.spyOn(openModal, 'close')
    bulkEditSpy.mockReturnValue(throwError(() => new Error('error occurred')))
    openModal.componentInstance.confirmClicked.next()
    expect(toastSpy).toHaveBeenCalled()
    expect(modalCloseSpy).not.toHaveBeenCalled()
  })

  it('should support next doc', () => {
    initNormally()
    const serviceSpy = jest.spyOn(documentListViewService, 'getNext')
    const routerSpy = jest.spyOn(router, 'navigate')
    serviceSpy.mockReturnValue(of(100))
    component.nextDoc()
    expect(serviceSpy).toHaveBeenCalled()
    expect(routerSpy).toHaveBeenCalledWith(['documents', 100])
  })

  it('should support previous doc', () => {
    initNormally()
    const serviceSpy = jest.spyOn(documentListViewService, 'getPrevious')
    const routerSpy = jest.spyOn(router, 'navigate')
    serviceSpy.mockReturnValue(of(100))
    component.previousDoc()
    expect(serviceSpy).toHaveBeenCalled()
    expect(routerSpy).toHaveBeenCalledWith(['documents', 100])
  })

  it('should support password-protected PDFs with a password field', () => {
    initNormally()
    component.onError({ name: 'PasswordException' }) // normally dispatched by pdf viewer
    expect(component.requiresPassword).toBeTruthy()
    fixture.detectChanges()
    expect(
      fixture.debugElement.query(By.css('input[type=password]'))
    ).not.toBeUndefined()
    component.password = 'foo'
    component.pdfPreviewLoaded({ numPages: 1000 } as any)
    expect(component.requiresPassword).toBeFalsy()
  })

  it('should support Enter key in password field', () => {
    initNormally()
    component.onError({ name: 'PasswordException' }) // normally dispatched by pdf viewer
    fixture.detectChanges()
    expect(component.password).toBeUndefined()
    const pwField = fixture.debugElement.query(By.css('input[type=password]'))
    pwField.nativeElement.value = 'foobar'
    pwField.nativeElement.dispatchEvent(
      new KeyboardEvent('keyup', { key: 'Enter' })
    )
    expect(component.password).toEqual('foobar')
  })

  it('should update n pages after pdf loaded', () => {
    initNormally()
    component.pdfPreviewLoaded({ numPages: 1000 } as any)
    expect(component.previewNumPages).toEqual(1000)
  })

  it('should support updating notes dynamically', () => {
    const notes = [
      {
        id: 1,
        note: 'hello world',
      },
    ]
    initNormally()
    const refreshSpy = jest.spyOn(openDocumentsService, 'refreshDocument')
    component.notesUpdated(notes) // called by notes component
    expect(component.document.notes).toEqual(notes)
    expect(refreshSpy).toHaveBeenCalled()
  })

  it('should support quick filtering by correspondent', () => {
    initNormally()
    const object = {
      id: 22,
      name: 'Correspondent22',
      last_correspondence: new Date().toISOString(),
    } as PaperlessCorrespondent
    const qfSpy = jest.spyOn(documentListViewService, 'quickFilter')
    component.filterDocuments([object])
    expect(qfSpy).toHaveBeenCalledWith([
      {
        rule_type: FILTER_CORRESPONDENT,
        value: object.id.toString(),
      },
    ])
  })

  it('should support quick filtering by doc type', () => {
    initNormally()
    const object = { id: 22, name: 'DocumentType22' } as PaperlessDocumentType
    const qfSpy = jest.spyOn(documentListViewService, 'quickFilter')
    component.filterDocuments([object])
    expect(qfSpy).toHaveBeenCalledWith([
      {
        rule_type: FILTER_DOCUMENT_TYPE,
        value: object.id.toString(),
      },
    ])
  })

  it('should support quick filtering by storage path', () => {
    initNormally()
    const object = {
      id: 22,
      name: 'StoragePath22',
      path: '/foo/bar/',
    } as PaperlessStoragePath
    const qfSpy = jest.spyOn(documentListViewService, 'quickFilter')
    component.filterDocuments([object])
    expect(qfSpy).toHaveBeenCalledWith([
      {
        rule_type: FILTER_STORAGE_PATH,
        value: object.id.toString(),
      },
    ])
  })

  it('should support quick filtering by all tags', () => {
    initNormally()
    const object1 = {
      id: 22,
      name: 'Tag22',
      is_inbox_tag: true,
      color: '#ff0000',
      text_color: '#000000',
    } as PaperlessTag
    const object2 = {
      id: 23,
      name: 'Tag22',
      is_inbox_tag: true,
      color: '#ff0000',
      text_color: '#000000',
    } as PaperlessTag
    const qfSpy = jest.spyOn(documentListViewService, 'quickFilter')
    component.filterDocuments([object1, object2])
    expect(qfSpy).toHaveBeenCalledWith([
      {
        rule_type: FILTER_HAS_TAGS_ALL,
        value: object1.id.toString(),
      },
      {
        rule_type: FILTER_HAS_TAGS_ALL,
        value: object2.id.toString(),
      },
    ])
  })

  it('should support quick filtering by date after - 1d and before +1d', () => {
    initNormally()
    const object = { year: 2023, month: 5, day: 14 } as NgbDateStruct
    const qfSpy = jest.spyOn(documentListViewService, 'quickFilter')
    component.filterDocuments([object])
    expect(qfSpy).toHaveBeenCalledWith([
      {
        rule_type: FILTER_CREATED_AFTER,
        value: '2023-05-13',
      },
      {
        rule_type: FILTER_CREATED_BEFORE,
        value: '2023-05-15',
      },
    ])
  })

  it('should detect RTL languages and add css class to content textarea', () => {
    initNormally()
    component.metadata = { lang: 'he' }
    component.nav.select(2) // content
    fixture.detectChanges()
    expect(component.isRTL).toBeTruthy()
    expect(fixture.debugElement.queryAll(By.css('textarea.rtl'))).not.toBeNull()
  })

  it('should display built-in pdf viewer if not disabled', () => {
    initNormally()
    component.metadata = { has_archive_version: true }
    jest.spyOn(settingsService, 'get').mockReturnValue(false)
    expect(component.useNativePdfViewer).toBeFalsy()
    fixture.detectChanges()
    expect(fixture.debugElement.query(By.css('pdf-viewer'))).not.toBeNull()
  })

  it('should display native pdf viewer if enabled', () => {
    initNormally()
    component.metadata = { has_archive_version: true }
    jest.spyOn(settingsService, 'get').mockReturnValue(true)
    expect(component.useNativePdfViewer).toBeTruthy()
    fixture.detectChanges()
    expect(fixture.debugElement.query(By.css('object'))).not.toBeNull()
  })

  it('should attempt to retrieve metadata', () => {
    const metadataSpy = jest.spyOn(documentService, 'getMetadata')
    metadataSpy.mockReturnValue(of({ has_archive_version: true }))
    initNormally()
    expect(metadataSpy).toHaveBeenCalled()
  })

  it('should show an error if failed metadata retrieval', () => {
    const error = new Error('metadata error')
    jest
      .spyOn(documentService, 'getMetadata')
      .mockReturnValue(throwError(() => error))
    const toastSpy = jest.spyOn(toastService, 'showError')
    initNormally()
    expect(toastSpy).toHaveBeenCalledWith('Error retrieving metadata', error)
  })

  function initNormally() {
    jest.spyOn(documentService, 'get').mockReturnValueOnce(of(doc))
    jest.spyOn(openDocumentsService, 'getOpenDocument').mockReturnValue(null)
    jest
      .spyOn(openDocumentsService, 'openDocument')
      .mockReturnValueOnce(of(true))
    fixture.detectChanges()
  }
})
