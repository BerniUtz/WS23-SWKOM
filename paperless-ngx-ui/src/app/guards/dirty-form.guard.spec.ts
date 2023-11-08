import { TestBed } from '@angular/core/testing'
import { NgbModal, NgbModule } from '@ng-bootstrap/ng-bootstrap'
import { RouterTestingModule } from '@angular/router/testing'
import { routes } from '../app-routing.module'
import { ConfirmDialogComponent } from '../components/common/confirm-dialog/confirm-dialog.component'
import { DirtyFormGuard } from './dirty-form.guard'
import { DirtyComponent } from '@ngneat/dirty-check-forms'
import { ActivatedRoute } from '@angular/router'
import { Component } from '@angular/core'

@Component({})
class GenericDirtyComponent implements DirtyComponent {
  isDirty$: boolean
}

describe('DirtyFormGuard', () => {
  let guard: DirtyFormGuard
  let component: DirtyComponent
  let route: ActivatedRoute

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [
        DirtyFormGuard,
        NgbModal,
        {
          provide: ActivatedRoute,
          useValue: {
            snapshot: {},
          },
        },
        GenericDirtyComponent,
      ],
      imports: [RouterTestingModule.withRoutes(routes), NgbModule],
      declarations: [ConfirmDialogComponent, GenericDirtyComponent],
    }).compileComponents()

    guard = TestBed.inject(DirtyFormGuard)
    route = TestBed.inject(ActivatedRoute)
    const fixture = TestBed.createComponent(GenericDirtyComponent)
    component = fixture.componentInstance

    fixture.detectChanges()
  })

  it('should deactivate if component is not dirty', () => {
    component.isDirty$ = false
    const confirmSpy = jest.spyOn(guard, 'confirmChanges')
    const canDeactivate = guard.canDeactivate(component, route.snapshot)
    canDeactivate.subscribe()

    expect(canDeactivate).toBeTruthy()
    expect(confirmSpy).not.toHaveBeenCalled()
  })

  it('should offer confirm before deactivate if component is dirty', () => {
    component.isDirty$ = true
    const confirmSpy = jest.spyOn(guard, 'confirmChanges')
    const canDeactivate = guard.canDeactivate(component, route.snapshot)
    canDeactivate.subscribe()

    expect(canDeactivate).toHaveProperty('source') // Observable
    expect(confirmSpy).toHaveBeenCalled()
  })
})
