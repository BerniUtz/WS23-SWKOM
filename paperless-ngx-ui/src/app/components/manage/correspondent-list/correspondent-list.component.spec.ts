import { ComponentFixture, TestBed } from '@angular/core/testing'
import { CorrespondentListComponent } from './correspondent-list.component'
import { HttpClientTestingModule } from '@angular/common/http/testing'
import { DatePipe } from '@angular/common'
import { SortableDirective } from 'src/app/directives/sortable.directive'
import { NgbPaginationModule } from '@ng-bootstrap/ng-bootstrap'
import { FormsModule, ReactiveFormsModule } from '@angular/forms'
import { PageHeaderComponent } from '../../common/page-header/page-header.component'
import { IfPermissionsDirective } from 'src/app/directives/if-permissions.directive'
import { CorrespondentService } from 'src/app/services/rest/correspondent.service'
import { of } from 'rxjs'

describe('CorrespondentListComponent', () => {
  let component: CorrespondentListComponent
  let fixture: ComponentFixture<CorrespondentListComponent>
  let correspondentsService: CorrespondentService

  beforeEach(async () => {
    TestBed.configureTestingModule({
      declarations: [
        CorrespondentListComponent,
        SortableDirective,
        PageHeaderComponent,
        IfPermissionsDirective,
      ],
      providers: [DatePipe],
      imports: [
        HttpClientTestingModule,
        NgbPaginationModule,
        FormsModule,
        ReactiveFormsModule,
      ],
    }).compileComponents()
    correspondentsService = TestBed.inject(CorrespondentService)
  })

  // Tests are included in management-list.compontent.spec.ts

  it('should use correct delete message', () => {
    jest.spyOn(correspondentsService, 'listFiltered').mockReturnValue(
      of({
        count: 3,
        all: [1, 2, 3],
        results: [
          {
            id: 1,
            name: 'Correspondent1',
          },
          {
            id: 2,
            name: 'Correspondent2',
          },
          {
            id: 3,
            name: 'Correspondent3',
          },
        ],
      })
    )
    fixture = TestBed.createComponent(CorrespondentListComponent)
    component = fixture.componentInstance
    fixture.detectChanges()

    expect(
      component.getDeleteMessage({ id: 1, name: 'Correspondent1' })
    ).toEqual(
      'Do you really want to delete the correspondent "Correspondent1"?'
    )
  })

  it('should support very old date strings', () => {
    jest.spyOn(correspondentsService, 'listFiltered').mockReturnValue(
      of({
        count: 1,
        all: [1],
        results: [
          {
            id: 1,
            name: 'Correspondent1',
            last_correspondence: '1832-12-31T15:32:54-07:52:58',
          },
        ],
      })
    )
    fixture = TestBed.createComponent(CorrespondentListComponent)
    component = fixture.componentInstance
    fixture.detectChanges()
  })
})
