import { DatePipe } from '@angular/common'
import { HttpClientTestingModule } from '@angular/common/http/testing'
import {
  ComponentFixture,
  TestBed,
  fakeAsync,
  tick,
} from '@angular/core/testing'
import { FormsModule, ReactiveFormsModule } from '@angular/forms'
import { By } from '@angular/platform-browser'
import {
  NgbModal,
  NgbModalModule,
  NgbModalRef,
  NgbPaginationModule,
} from '@ng-bootstrap/ng-bootstrap'
import { of, throwError } from 'rxjs'
import { PaperlessTag } from 'src/app/data/paperless-tag'
import { IfPermissionsDirective } from 'src/app/directives/if-permissions.directive'
import { SortableDirective } from 'src/app/directives/sortable.directive'
import { SafeHtmlPipe } from 'src/app/pipes/safehtml.pipe'
import { TagService } from 'src/app/services/rest/tag.service'
import { PageHeaderComponent } from '../../common/page-header/page-header.component'
import { TagListComponent } from '../tag-list/tag-list.component'
import { ManagementListComponent } from './management-list.component'
import { PermissionsService } from 'src/app/services/permissions.service'
import { ToastService } from 'src/app/services/toast.service'
import { EditDialogComponent } from '../../common/edit-dialog/edit-dialog.component'
import { ConfirmDialogComponent } from '../../common/confirm-dialog/confirm-dialog.component'
import { DocumentListViewService } from 'src/app/services/document-list-view.service'
import { FILTER_HAS_TAGS_ALL } from 'src/app/data/filter-rule-type'
import { RouterTestingModule } from '@angular/router/testing'
import { routes } from 'src/app/app-routing.module'
import { PermissionsGuard } from 'src/app/guards/permissions.guard'
import { MATCH_AUTO } from 'src/app/data/matching-model'
import { MATCH_NONE } from 'src/app/data/matching-model'
import { MATCH_LITERAL } from 'src/app/data/matching-model'
import { PermissionsDialogComponent } from '../../common/permissions-dialog/permissions-dialog.component'

const tags: PaperlessTag[] = [
  {
    id: 1,
    name: 'Tag1 Foo',
    matching_algorithm: MATCH_LITERAL,
    match: 'foo',
  },
  {
    id: 2,
    name: 'Tag2',
    matching_algorithm: MATCH_NONE,
  },
  {
    id: 3,
    name: 'Tag3',
    matching_algorithm: MATCH_AUTO,
  },
]

describe('ManagementListComponent', () => {
  let component: ManagementListComponent<PaperlessTag>
  let fixture: ComponentFixture<ManagementListComponent<PaperlessTag>>
  let tagService: TagService
  let modalService: NgbModal
  let toastService: ToastService
  let documentListViewService: DocumentListViewService

  beforeEach(async () => {
    TestBed.configureTestingModule({
      declarations: [
        TagListComponent,
        SortableDirective,
        PageHeaderComponent,
        IfPermissionsDirective,
        SafeHtmlPipe,
        ConfirmDialogComponent,
        PermissionsDialogComponent,
      ],
      providers: [
        {
          provide: PermissionsService,
          useValue: {
            currentUserCan: () => true,
            currentUserHasObjectPermissions: () => true,
            currentUserOwnsObject: () => true,
          },
        },
        DatePipe,
        PermissionsGuard,
      ],
      imports: [
        HttpClientTestingModule,
        NgbPaginationModule,
        FormsModule,
        ReactiveFormsModule,
        NgbModalModule,
        RouterTestingModule.withRoutes(routes),
      ],
    }).compileComponents()

    tagService = TestBed.inject(TagService)
    jest
      .spyOn(tagService, 'listFiltered')
      .mockImplementation(
        (page, pageSize, sortField, sortReverse, nameFilter, fullPerms) => {
          const results = nameFilter
            ? tags.filter((t) => t.name.toLowerCase().includes(nameFilter))
            : tags
          return of({
            count: results.length,
            all: results.map((o) => o.id),
            results,
          })
        }
      )
    modalService = TestBed.inject(NgbModal)
    toastService = TestBed.inject(ToastService)
    documentListViewService = TestBed.inject(DocumentListViewService)
    fixture = TestBed.createComponent(TagListComponent)
    component = fixture.componentInstance
    fixture.detectChanges()
  })

  // These tests are shared among all management list components

  it('should support filtering, clear on Esc key', fakeAsync(() => {
    const nameFilterInput = fixture.debugElement.query(By.css('input'))
    nameFilterInput.nativeElement.value = 'foo'
    // nameFilterInput.nativeElement.dispatchEvent(new Event('input'))
    component.nameFilter = 'foo' // subject normally triggered by ngModel
    tick(400) // debounce
    fixture.detectChanges()
    expect(component.data).toEqual([tags[0]])

    nameFilterInput.nativeElement.dispatchEvent(
      new KeyboardEvent('keyup', { code: 'Escape' })
    )
    tick(400) // debounce
    fixture.detectChanges()
    expect(component.nameFilter).toBeNull()
    expect(component.data).toEqual(tags)
  }))

  it('should support create, show notification on error / success', () => {
    let modal: NgbModalRef
    modalService.activeInstances.subscribe((m) => (modal = m[m.length - 1]))
    const toastErrorSpy = jest.spyOn(toastService, 'showError')
    const toastInfoSpy = jest.spyOn(toastService, 'showInfo')
    const reloadSpy = jest.spyOn(component, 'reloadData')

    const createButton = fixture.debugElement.queryAll(By.css('button'))[2]
    createButton.triggerEventHandler('click')

    expect(modal).not.toBeUndefined()
    const editDialog =
      modal.componentInstance as EditDialogComponent<PaperlessTag>

    // fail first
    editDialog.failed.emit({ error: 'error creating item' })
    expect(toastErrorSpy).toHaveBeenCalled()
    expect(reloadSpy).not.toHaveBeenCalled()

    // succeed
    editDialog.succeeded.emit()
    expect(toastInfoSpy).toHaveBeenCalled()
    expect(reloadSpy).toHaveBeenCalled()
  })

  it('should support edit, show notification on error / success', () => {
    let modal: NgbModalRef
    modalService.activeInstances.subscribe((m) => (modal = m[m.length - 1]))
    const toastErrorSpy = jest.spyOn(toastService, 'showError')
    const toastInfoSpy = jest.spyOn(toastService, 'showInfo')
    const reloadSpy = jest.spyOn(component, 'reloadData')

    const editButton = fixture.debugElement.queryAll(By.css('button'))[6]
    editButton.triggerEventHandler('click')

    expect(modal).not.toBeUndefined()
    const editDialog =
      modal.componentInstance as EditDialogComponent<PaperlessTag>
    expect(editDialog.object).toEqual(tags[0])

    // fail first
    editDialog.failed.emit({ error: 'error editing item' })
    expect(toastErrorSpy).toHaveBeenCalled()
    expect(reloadSpy).not.toHaveBeenCalled()

    // succeed
    editDialog.succeeded.emit()
    expect(toastInfoSpy).toHaveBeenCalled()
    expect(reloadSpy).toHaveBeenCalled()
  })

  it('should support delete, show notification on error / success', () => {
    let modal: NgbModalRef
    modalService.activeInstances.subscribe((m) => (modal = m[m.length - 1]))
    const toastErrorSpy = jest.spyOn(toastService, 'showError')
    const deleteSpy = jest.spyOn(tagService, 'delete')
    const reloadSpy = jest.spyOn(component, 'reloadData')

    const deleteButton = fixture.debugElement.queryAll(By.css('button'))[7]
    deleteButton.triggerEventHandler('click')

    expect(modal).not.toBeUndefined()
    const editDialog = modal.componentInstance as ConfirmDialogComponent

    // fail first
    deleteSpy.mockReturnValueOnce(throwError(() => new Error('error deleting')))
    editDialog.confirmClicked.emit()
    expect(toastErrorSpy).toHaveBeenCalled()
    expect(reloadSpy).not.toHaveBeenCalled()

    // succeed
    deleteSpy.mockReturnValueOnce(of(true))
    editDialog.confirmClicked.emit()
    expect(reloadSpy).toHaveBeenCalled()
  })

  it('should support quick filter for objects', () => {
    const qfSpy = jest.spyOn(documentListViewService, 'quickFilter')
    const filterButton = fixture.debugElement.queryAll(By.css('button'))[5]
    filterButton.triggerEventHandler('click')
    expect(qfSpy).toHaveBeenCalledWith([
      { rule_type: FILTER_HAS_TAGS_ALL, value: tags[0].id.toString() },
    ]) // subclasses set the filter rule type
  })

  it('should reload on sort', () => {
    const reloadSpy = jest.spyOn(component, 'reloadData')
    const sortable = fixture.debugElement.query(By.directive(SortableDirective))
    sortable.triggerEventHandler('click')
    expect(reloadSpy).toHaveBeenCalled()
  })

  it('should support toggle all items in view', () => {
    expect(component.selectedObjects.size).toEqual(0)
    const toggleAllSpy = jest.spyOn(component, 'toggleAll')
    const checkButton = fixture.debugElement.queryAll(
      By.css('input.form-check-input')
    )[0]
    checkButton.nativeElement.dispatchEvent(new Event('click'))
    checkButton.nativeElement.checked = true
    checkButton.nativeElement.dispatchEvent(new Event('click'))
    expect(toggleAllSpy).toHaveBeenCalled()
    expect(component.selectedObjects.size).toEqual(tags.length)
  })

  it('should support bulk edit permissions', () => {
    const bulkEditPermsSpy = jest.spyOn(tagService, 'bulk_update_permissions')
    component.toggleSelected(tags[0])
    component.toggleSelected(tags[1])
    component.toggleSelected(tags[2])
    component.toggleSelected(tags[2]) // uncheck, for coverage
    const selected = new Set([tags[0].id, tags[1].id])
    expect(component.selectedObjects).toEqual(selected)
    let modal: NgbModalRef
    modalService.activeInstances.subscribe((m) => (modal = m[m.length - 1]))
    fixture.detectChanges()
    component.setPermissions()
    expect(modal).not.toBeUndefined()

    // fail first
    bulkEditPermsSpy.mockReturnValueOnce(
      throwError(() => new Error('error setting permissions'))
    )
    const errorToastSpy = jest.spyOn(toastService, 'showError')
    modal.componentInstance.confirmClicked.emit()
    expect(bulkEditPermsSpy).toHaveBeenCalled()
    expect(errorToastSpy).toHaveBeenCalled()

    const successToastSpy = jest.spyOn(toastService, 'showInfo')
    bulkEditPermsSpy.mockReturnValueOnce(of('OK'))
    modal.componentInstance.confirmClicked.emit()
    expect(bulkEditPermsSpy).toHaveBeenCalled()
    expect(successToastSpy).toHaveBeenCalled()
  })
})
